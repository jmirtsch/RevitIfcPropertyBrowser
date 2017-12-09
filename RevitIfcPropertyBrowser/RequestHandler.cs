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
				if (elementIds.Count == 0)
				{
					mBrowser.mPropertyGrid.SelectedObject = null;
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
				IEnumerable<object> properties = elements.ConvertAll(x => new ObjectIfcProperties(x));
				mBrowser.mPropertyGrid.SelectedObjects = properties.ToArray();
			}
			finally
			{

			}
			return;
		}
	}
}
