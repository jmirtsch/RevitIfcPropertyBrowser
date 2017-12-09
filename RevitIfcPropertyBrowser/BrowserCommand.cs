using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;


namespace RevitIfcPropertyBrowser
{
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
}
