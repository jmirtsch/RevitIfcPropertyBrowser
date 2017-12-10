namespace RevitIfcPropertyBrowser
{
	partial class BrowserControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.propertyGrid = new System.Windows.Forms.PropertyGrid();
			this.buttonIfcType = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// propertyGrid
			// 
			this.propertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.propertyGrid.Location = new System.Drawing.Point(3, 39);
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.Size = new System.Drawing.Size(402, 507);
			this.propertyGrid.TabIndex = 0;
			// 
			// buttonIfcType
			// 
			this.buttonIfcType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonIfcType.Enabled = false;
			this.buttonIfcType.Location = new System.Drawing.Point(296, 4);
			this.buttonIfcType.Name = "buttonIfcType";
			this.buttonIfcType.Size = new System.Drawing.Size(97, 29);
			this.buttonIfcType.TabIndex = 1;
			this.buttonIfcType.Text = "IfcType";
			this.buttonIfcType.UseVisualStyleBackColor = true;
			this.buttonIfcType.Click += new System.EventHandler(this.buttonIfcType_Click);
			// 
			// BrowserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.buttonIfcType);
			this.Controls.Add(this.propertyGrid);
			this.Name = "BrowserControl";
			this.Size = new System.Drawing.Size(408, 549);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.Button buttonIfcType;
		internal System.Windows.Forms.PropertyGrid propertyGrid;
	}
}
