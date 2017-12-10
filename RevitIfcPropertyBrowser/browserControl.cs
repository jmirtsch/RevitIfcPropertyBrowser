using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using GeometryGym.Ifc;

namespace RevitIfcPropertyBrowser
{
	public partial class BrowserControl : UserControl
	{
		private IfcElementType mElementType = null;

		internal IfcElementType ElementType
		{
			set
			{
				mElementType = value;
				buttonIfcType.Enabled = value != null;
			}
		}

		public BrowserControl()
		{
			InitializeComponent();
		}

		private void buttonIfcType_Click(object sender, EventArgs e)
		{
			if(mElementType != null)
			{
				TypeBrowserDialog dlg = new TypeBrowserDialog(mElementType);
				dlg.ShowDialog();

			}
		}
	}
}
