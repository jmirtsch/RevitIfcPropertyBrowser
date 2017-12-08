using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Windows;

using GeometryGym.Ifc;

namespace RevitIfcPropertyBrowser
{
	public class PropertyBrowser : IExternalApplication
	{
		private ExternalEvent mExEvent;
		internal static DockablePaneId mPropertyPanel = null;
		public Result OnStartup(UIControlledApplication a)
		{
			string tabName = "IFC";
			RibbonControl myRibbon = ComponentManager.Ribbon;
			RibbonTab ggTab = null;

			foreach (RibbonTab tab in myRibbon.Tabs)
			{
				if (string.Compare(tab.Id, tabName, true) == 0)
				{
					ggTab = tab;
					break;
				}
			}
			if (ggTab == null)
				a.CreateRibbonTab(tabName);
			Autodesk.Revit.UI.RibbonPanel rp = a.CreateRibbonPanel(tabName, "Browser");
			PushButtonData pbd = new PushButtonData("propBrowser", "Ifc Property Browser", Assembly.GetExecutingAssembly().Location, "RevitIfcPropertyBrowser.ShowBrowser");
			pbd.ToolTip = "Show Property Browser";

			rp.AddItem(pbd);
			DockablePaneProviderData data = new DockablePaneProviderData();
			Browser browser = new Browser();
			data.FrameworkElement = browser as System.Windows.FrameworkElement;
			data.InitialState = new DockablePaneState();
			data.InitialState.DockPosition = DockPosition.Tabbed;

			mPropertyPanel = new DockablePaneId(new Guid("{C7C70722-1B9B-4454-A054-DFD142F23580}"));
			a.RegisterDockablePane(mPropertyPanel, "IFC Properties", browser);


			foreach (Autodesk.Windows.RibbonTab tab in Autodesk.Windows.ComponentManager.Ribbon.Tabs)
			{
				if (tab.Id == "Modify")
				{
					tab.PropertyChanged += PanelEvent;
					break;
				}
			}
			RequestHandler handler = new RequestHandler(browser);
			mExEvent = ExternalEvent.Create(handler);
			return Result.Succeeded;
		}

		public Result OnShutdown(UIControlledApplication a)
		{
			return Result.Succeeded;
		}

		void PanelEvent(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Title")
			{
				mExEvent.Raise();
			}
		}
	}
	[Transaction(TransactionMode.Manual)]
	public class ShowBrowser : IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			DockablePane dp = commandData.Application.GetDockablePane(PropertyBrowser.mPropertyPanel);
			dp.Show();
			return Result.Succeeded;
		}
	}

	public class RequestHandler : IExternalEventHandler
	{
		private Request mRequest = new Request();
		public Request Request { get { return mRequest; } }

		private Browser mBrowser = null;
		public RequestHandler(Browser browser)
		{
			mBrowser = browser;
		}

		public String GetName() { return "IFC Property Browser"; }
		public void Execute(UIApplication uiapp)
		{
			try
			{
				ICollection<ElementId> elementIds = uiapp.ActiveUIDocument.Selection.GetElementIds();
				string path = Path.GetTempPath();
				Document document = uiapp.ActiveUIDocument.Document;
				if (elementIds.Count == 0)
				{
					//mBrowser.mPropertyGrid.SelectedObject = null;
					return;
				}
				string ids = string.Join(";", elementIds.ToList().ConvertAll(x => x.ToString()));
				IFCExportOptions options = new IFCExportOptions();
				options.AddOption("ElementsForExport",ids);
				string fileName = Path.GetFileNameWithoutExtension(document.PathName) + ".ifc";
				Transaction transaction = new Transaction(document, "Export IFC");
				transaction.Start();

				document.Export(path, fileName, options);
				transaction.RollBack();
				DatabaseIfc db = new DatabaseIfc(Path.Combine(path, fileName));

				List<IfcElement> elements = db.Context.Extract<IfcElement>();
				IEnumerable<object> properties = elements.ConvertAll(x => new ObjectIfcProperties(x));
				mBrowser.mPropertyGrid.SelectedObjects = properties.ToArray();	
			}
			finally
			{

			}
			return;
		}
	}
	public class CustomProperty
	{
		private object mValue = null;
		public CustomProperty(string name, object value, Type type, bool readOnly, bool visible)
		{
			Name = name;
			mValue = value;
			Type = type;
			ReadOnly = readOnly;
			Visible = visible;
		}
		public Type Type { get; set; } = null;
		public bool ReadOnly { get; set; } = false;
		public string Name { get; set; } = "";
		public string Group { get; set; } = "Unknown";
		public bool Visible { get; set; } = true;
		public object Value
		{
			get => mValue;
			set
			{
				//To Implement so that property can be stored on revit parameter.
			}
		}
	}

	public class CustomPropertyDescriptor : PropertyDescriptor
	{
		CustomProperty mProperty;
		public CustomPropertyDescriptor(ref CustomProperty myProperty, Attribute[] attrs) : base(myProperty.Name, attrs)
		{
			mProperty = myProperty;
		}

		#region PropertyDescriptor specific

		public override bool CanResetValue(object component)
		{
			return false;
		}

		public override Type ComponentType
		{
			get { return null; }
		}

		public override object GetValue(object component)
		{
			return mProperty.Value;
		}

		public override string Description
		{
			get { return mProperty.Name; }
		}

		public override string Category
		{
			get { return mProperty.Group; }
		}

		public override string DisplayName
		{
			get { return mProperty.Name; }
		}

		public override bool IsReadOnly
		{
			get { return mProperty.ReadOnly; }
		}

		public override void ResetValue(object component)
		{
			//Have to implement
		}

		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}

		public override void SetValue(object component, object value)
		{
			mProperty.Value = value;
		}

		public override Type PropertyType
		{
			get { return mProperty.Type; }
		}
		#endregion
	}
	internal class ObjectIfcProperties : CollectionBase, ICustomTypeDescriptor
	{
		internal ObjectIfcProperties(IfcElement element)
		{
			bool readOnly = true;
			Add(new CustomProperty("GlobalId", element.GlobalId, typeof(string), true, true) { Group = "Element" });
			Add(new CustomProperty("Name", element.Name, typeof(string), readOnly, true) { Group = "Element" });
			Add(new CustomProperty("Description", element.Description, typeof(string), readOnly, true) { Group = "Element" });
			Add(new CustomProperty("ObjectType", element.ObjectType, typeof(string), readOnly, true) { Group = "Element" });
			Add(new CustomProperty("Type", element.GetType().Name, typeof(string), true, true) { Group = "Element" });
			Add(new CustomProperty("Tag", element.Tag, typeof(string), readOnly, true) { Group = "Element" });

			foreach (IfcPropertySet pset in element.IsDefinedBy.ToList().ConvertAll(x => x.RelatingPropertyDefinition).OfType<IfcPropertySet>())
			{
				foreach (IfcPropertySingleValue psv in pset.HasProperties.Values.ToList().OfType<IfcPropertySingleValue>())
				{
					IfcValue val = psv.NominalValue;
					if (val == null)
						Add(new CustomProperty(psv.Name, "", typeof(string), readOnly, true) { Group = pset.Name });
					else
					{
						Object obj = val.Value;
						Add(new CustomProperty(psv.Name, obj, obj.GetType(), readOnly, true) { Group = pset.Name });
					}
				}
			}
				
			
		}

		public string Name { get; set; } = "Selected Object(s)";
		public void Add(CustomProperty Value)
		{
			base.List.Add(Value);
		}
		public void Remove(string name)
		{
			foreach (CustomProperty prop in base.List)
			{
				if (string.Compare(prop.Name, name, true) == 0)
				{
					base.List.Remove(prop);
					return;
				}
			}
		}
		public CustomProperty this[string name]
		{
			get
			{
				foreach (CustomProperty prop in base.List)
				{
					if (string.Compare(prop.Name, name, true) == 0)
					{
						return prop;
					}
				}
				return null;
			}
		}
		public CustomProperty this[int index]
		{
			get
			{
				return (CustomProperty)base.List[index];
			}
			set
			{
				base.List[index] = (CustomProperty)value;
			}
		}

		public String GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}

		public AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}

		public String GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		public EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}

		public PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}

		public object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}
		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}
		public EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}
		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptor[] newProps = new PropertyDescriptor[this.Count];
			for (int i = 0; i < this.Count; i++)
			{
				CustomProperty prop = (CustomProperty)this[i];
				newProps[i] = new CustomPropertyDescriptor(ref prop, attributes);
			}
			return new PropertyDescriptorCollection(newProps);
		}
		public PropertyDescriptorCollection GetProperties()
		{
			return TypeDescriptor.GetProperties(this, true);
		}
		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}
	}


	public class Request
	{
		//public string Take()
		//{
		//   string str = "";
		//   return Interlocked.Exchange(ref mJson, str);
		//}
		//public void Make(string json)
		//{
		//   Interlocked.Exchange(ref mJson, json);
		//}
	}
	[Transaction(TransactionMode.Manual)]
	public class RegisterDockableWindow : IExternalCommand
	{
		public Result Execute(
		 ExternalCommandData commandData,
		 ref string message,
		 ElementSet elements)
		{

			return Result.Succeeded;
		}
	}
}

