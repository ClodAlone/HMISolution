#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Any infringement will be prosecuted under
//  applicable laws. 
//
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Syncfusion.GridHelperClasses
{
	/// <summary>
	/// Provides editing support for Xhtml text. The <see cref="XhtmlCellRenderer"/>
	/// will display the panel inside a dropdown container.
	/// </summary>
	public class XhtmlEntryPanel : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.RichTextBox richTextBox1;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.Button btnCancel;
		private System.ComponentModel.IContainer components;

		/// <summary>
		/// Initializes a new <see cref="XhtmlEntryPanel"/> object.
		/// </summary>
		public XhtmlEntryPanel()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitForm call
		}

		/// <summary> 
		/// Clean up any resources being used.
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(XhtmlEntryPanel));
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			this.btnSave = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// richTextBox1
			// 
			this.richTextBox1.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.Size = new System.Drawing.Size(352, 192);
			this.richTextBox1.TabIndex = 7;
			this.richTextBox1.Text = "";
			this.richTextBox1.ZoomFactor = 1.2f;
			// 
			// btnSave
			// 
			this.btnSave.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnSave.Location = new System.Drawing.Point(176, 200);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(72, 24);
			this.btnSave.TabIndex = 8;
			this.btnSave.Text = "&Save";
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.btnCancel.CausesValidation = false;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnCancel.Location = new System.Drawing.Point(264, 200);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(72, 24);
			this.btnCancel.TabIndex = 8;
			this.btnCancel.Text = "&Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// XhtmlEntryPanel
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.btnSave,
																		  this.richTextBox1,
																		  this.btnCancel});
			this.Name = "XhtmlEntryPanel";
			this.Size = new System.Drawing.Size(352, 232);
			this.ResumeLayout(false);

		}
		#endregion
        /// <summary>
        /// Processes the DialogKey
        /// </summary>
        /// <param name="keyData">Keys</param>
        /// <returns>bool</returns>
		protected override bool ProcessDialogKey(Keys keyData)
		{
			//Syncfusion.Diagnostics.TraceUtil.TraceCurrentMethodInfo(keyData);
			if (this.ActiveControl == this.richTextBox1)
			{
				if (keyData == Keys.Enter)
					return false;
			}
			return base.ProcessDialogKey (keyData);
		}


		private void btnSave_Click(object sender, System.EventArgs e)
		{
			if (Save != null)
				Save(this, EventArgs.Empty);
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			if (Cancel != null)
				Cancel(this, EventArgs.Empty);
		}

		/// <summary>
		/// Occurs when user clicked the "Save" button.
		/// </summary>
		public event EventHandler Save;

		/// <summary>
		/// Occurs when user clicked the "Cancel" button.
		/// </summary>
		public event EventHandler Cancel;

		/// <summary>
		/// A reference to the <see cref="RichTextBox"/> that is beeing displayed.
		/// </summary>
		public System.Windows.Forms.RichTextBox RichTextBox
		{
			get
			{
				return this.richTextBox1;
			}
		}

	}
}
