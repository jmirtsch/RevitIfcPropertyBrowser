using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;

using GeometryGym.Ifc;

namespace RevitIfcPropertyBrowser
{

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
			get { return mValue; }
			set
			{
				mValue = value;
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
}
