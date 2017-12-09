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


			foreach (RibbonTab tab in ComponentManager.Ribbon.Tabs)
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
}

