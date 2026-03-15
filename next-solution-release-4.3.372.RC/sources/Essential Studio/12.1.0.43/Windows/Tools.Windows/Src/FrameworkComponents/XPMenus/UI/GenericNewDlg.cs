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

namespace Syncfusion.Windows.Forms.Tools.XPMenus
{
	[Syncfusion.Documentation.DocumentationExclude()]
	public class GenericNewDlg : System.Windows.Forms.Form, IDontCallKillFocus, 
		IDontCallSetFocus,
		IAmACustomizationForm
	{
		private System.Windows.Forms.Label nameLabel;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button OK;
		private System.Windows.Forms.Button cancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public string NameLabel
		{
			get{return this.nameLabel.Text;}
			set{this.nameLabel.Text = value;}
		}
		public string OKButtonLabel
		{
			get{return this.OK.Text;}
			set{this.OK.Text = value;}
		}
		public string CancelButtonLabel
		{
			get{return this.cancel.Text;}
			set{this.cancel.Text = value;}
		}
		public string NewName
		{
			get{return this.textBox1.Text;}
			set{this.textBox1.Text = value;}
		}
		public GenericNewDlg()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.OK = new System.Windows.Forms.Button();
			this.nameLabel = new System.Windows.Forms.Label();
			this.cancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(8, 40);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(272, 20);
			this.textBox1.TabIndex = 0;
			this.textBox1.Text = "";
			// 
			// OK
			// 
			this.OK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.OK.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.OK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.OK.Location = new System.Drawing.Point(128, 64);
			this.OK.Name = "OK";
			this.OK.Size = new System.Drawing.Size(72, 24);
			this.OK.TabIndex = 1;
			this.OK.Text = "&OK";
			this.OK.Click += new System.EventHandler(this.OK_Click);
			// 
			// nameLabel
			// 
			this.nameLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.nameLabel.Location = new System.Drawing.Point(8, 16);
			this.nameLabel.Name = "nameLabel";
			this.nameLabel.Size = new System.Drawing.Size(272, 16);
			this.nameLabel.TabIndex = 0;
			this.nameLabel.Text = "Name:";
			// 
			// cancel
			// 
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.cancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.cancel.Location = new System.Drawing.Point(208, 64);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(72, 24);
			this.cancel.TabIndex = 2;
			this.cancel.Text = "&Cancel";
			this.cancel.Click += new System.EventHandler(this.cancel_Click);
			// 
			// GenericNewDlg
			// 
			this.AcceptButton = this.OK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(292, 101);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.cancel,
																		  this.OK,
																		  this.textBox1,
																		  this.nameLabel});
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "GenericNewDlg";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "New Item Name";
			this.ResumeLayout(false);

		}
		#endregion

		private void OK_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void cancel_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}
	}
}
