using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

using Autodesk.Revit.UI;

namespace RevitIfcPropertyBrowser
{
	public partial class Browser : Page, IDockablePaneProvider
	{
		internal PropertyGrid mPropertyGrid = null;
		public Browser()
		{
			InitializeComponent();
			mPropertyGrid = new PropertyGrid();
			System.Windows.Forms.TextBox tb = new System.Windows.Forms.TextBox() { Text = "jon" };
			WindowsFormsHost host = new WindowsFormsHost() { Child = mPropertyGrid };
			grid.Children.Add(host);
		}

		public void SetupDockablePane(DockablePaneProviderData data)
		{
			data.FrameworkElement = this as FrameworkElement;
			data.InitialState = new DockablePaneState();
			data.InitialState.DockPosition = DockPosition.Tabbed;
			//DockablePaneId targetPane;
			//if (m_targetGuid == Guid.Empty)
			//    targetPane = null;
			//else targetPane = new DockablePaneId(m_targetGuid);
			//if (m_position == DockPosition.Tabbed)

			data.InitialState.TabBehind = Autodesk.Revit.UI.DockablePanes.BuiltInDockablePanes.ProjectBrowser;
			//if (m_position == DockPosition.Floating)
			//{
			//data.InitialState.SetFloatingRectangle(new Autodesk.Revit.UI.Rectangle(10, 710, 10, 710));
			//data.InitialState.DockPosition = DockPosition.Tabbed;
			//}
			//Log.Message("***Intial docking parameters***");
			//Log.Message(APIUtility.GetDockStateSummary(data.InitialState));
		}
	}
}
