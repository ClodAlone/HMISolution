#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Syncfusion.Styles
{
	/// <summary>
	/// A form that displays a <see cref="StyleInfoPropertyGrid"/> with Apply and OK buttons.
	/// </summary>
	/// <example>
	/// </example>
	public class StyleInfoForm : System.Windows.Forms.Form
	{
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button applyButton;
		private Syncfusion.Styles.StyleInfoPropertyGrid grid;
		private object customizer;

		/// <summary>
		/// Initializes a new <see cref="StyleInfoForm"/> with an object whose properties should be displayed.
		/// </summary>
		/// <param name="customizer">The object for which properties should be displayed.</param>
		public StyleInfoForm(object customizer)
		{
			InitializeComponent();
			this.customizer = customizer;
			grid.SelectedObject = customizer;
		}

		/// <summary>
		/// Returns the apply button. You should set up a listener for a Click event.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Button ApplyButton
		{
			get
			{
				return applyButton;
			}
		}

		/// <summary>
		/// Cleans up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.okButton = new System.Windows.Forms.Button();
			this.applyButton = new System.Windows.Forms.Button();
			this.grid = new Syncfusion.Styles.StyleInfoPropertyGrid();

#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			this.AutoScaleBaseSize = new Size(5, 13);
#endif
			this.Text = "Options";
			this.MaximizeBox = false;
			this.StartPosition = FormStartPosition.CenterParent;
			this.CancelButton = okButton;
			this.Icon = null;
			this.AcceptButton = okButton;
			this.ControlBox = false;
			this.MinimizeBox = false;
			this.ClientSize = new Size(328, 325);

			okButton.Location = new Point(232, 297);
			okButton.DialogResult = DialogResult.OK;
			okButton.FlatStyle = FlatStyle.Flat;
			okButton.Size = new Size(75, 23);
			okButton.TabIndex = 0;
			okButton.Anchor = AnchorStyles.Bottom|AnchorStyles.Right;
			okButton.Text = "OK";

			applyButton.Location = new Point(132, 297);
			applyButton.FlatStyle = FlatStyle.Flat;
			applyButton.Size = new Size(75, 23);
			applyButton.TabIndex = 1;
			applyButton.Anchor = AnchorStyles.Bottom|AnchorStyles.Right;
			applyButton.Text = "Apply";

			grid.Location = new Point(8, 8);
			grid.Text = "PropertyGrid";
			grid.Size = new Size(312, 281);
			grid.CommandsVisibleIfAvailable = true;
			grid.TabIndex = 1;
			grid.AutoScrollMinSize = new Size(0, 0);
			grid.Anchor = AnchorStyles.Left|AnchorStyles.Bottom|AnchorStyles.Top|AnchorStyles.Right;
			grid.ToolbarVisible = false;

			this.Controls.Add(okButton);
			this.Controls.Add(applyButton);
			this.Controls.Add(grid);
		}
		#endregion
	}
}
