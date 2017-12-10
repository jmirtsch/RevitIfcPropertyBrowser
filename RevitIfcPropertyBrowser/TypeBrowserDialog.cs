using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using GeometryGym.Ifc;

namespace RevitIfcPropertyBrowser
{
	public partial class TypeBrowserDialog : Form
	{
		public TypeBrowserDialog(IfcElementType elementType)
		{
			InitializeComponent();
			ElementTypeIfcProperties property = new ElementTypeIfcProperties(elementType);
			propertyGrid1.SelectedObject = property;
		}
	}
}
