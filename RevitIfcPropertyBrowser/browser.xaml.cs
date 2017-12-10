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
		internal BrowserControl mBrowserControl = null;
		public Browser()
		{
			InitializeComponent();
			mBrowserControl = new BrowserControl();
			WindowsFormsHost host = new WindowsFormsHost() { Child = mBrowserControl };
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
