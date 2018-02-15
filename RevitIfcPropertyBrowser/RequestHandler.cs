using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;

using GeometryGym.Ifc;

namespace RevitIfcPropertyBrowser
{
	public class RequestHandler : IExternalEventHandler
	{
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
				if (document.IsFamilyDocument || elementIds.Count == 0)
				{
					mBrowser.mBrowserControl.propertyGrid.SelectedObject = null;
					return;
				}
				string ids = string.Join(";", elementIds.ToList().ConvertAll(x => x.ToString()));
				IFCExportOptions options = new IFCExportOptions();
				options.AddOption("ElementsForExport", ids);
				string fileName = Path.GetFileNameWithoutExtension(document.PathName) + ".ifc";
				Transaction transaction = new Transaction(document, "Export IFC");
				transaction.Start();

				document.Export(path, fileName, options);
				transaction.RollBack();
				DatabaseIfc db = new DatabaseIfc(Path.Combine(path, fileName));

				List<IfcElement> elements = db.Context.Extract<IfcElement>();
				if (elements.Count > 0)
				{
					IEnumerable<object> properties = elements.ConvertAll(x => new ElementIfcProperties(x));
					IfcElementType type = elements[0].RelatingType as IfcElementType;
					if (type != null)
					{
						foreach (IfcElement e in elements)
						{
							IfcElementType t = e.RelatingType as IfcElementType;
							if (t == null || type.Index != t.Index)
							{
								type = null;
								break;
							}
						}
					}
					mBrowser.mBrowserControl.propertyGrid.SelectedObjects = properties.ToArray();
					mBrowser.mBrowserControl.ElementType = type;
				}
			}
			finally
			{

			}
			return;
		}

		internal void ControlledApplication_DocumentChanged(object sender, DocumentChangedEventArgs e)
		{
			Document doc = e.GetDocument();
			
			
			ICollection<ElementId> modified = e.GetModifiedElementIds();
			foreach(ElementId id in modified)
			{

			}
		}
	}
}
