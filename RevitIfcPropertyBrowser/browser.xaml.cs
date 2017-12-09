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
			WindowsFormsHost host = new WindowsFormsHost() { Child = mPropertyGrid };
			grid.Children.Add(host);
		}

		public void SetupDockablePane(DockablePaneProviderData data)
		{
			data.FrameworkElement = this as FrameworkElement;
			data.InitialState = new DockablePaneState();
			data.InitialState.DockPosition = DockPosition.Tabbed;
			
			data.InitialState.TabBehind = DockablePanes.BuiltInDockablePanes.ProjectBrowser;
		}
	}
}
