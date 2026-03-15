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
	/// <summary>
	/// Summary description for NewBarItemDialog.
	/// </summary>
	[Syncfusion.Documentation.DocumentationExclude()]
	public class NewBarItemDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label3;
		public System.Windows.Forms.ComboBox typeBox;
		private System.Windows.Forms.TextBox nameBox;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public string NewName
		{
			get{return this.nameBox.Text;}
			set{this.nameBox.Text = value;}
		}

		public int Type
		{
			get
			{
				TypeBoxItem item = (TypeBoxItem) this.typeBox.SelectedItem;
				return (int) item.Type;
			}
		}

		public NewBarItemDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			InitializeTypeBoxItems();
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

		private void InitializeTypeBoxItems()
		{
			this.typeBox.Items.Add( new TypeBoxItem( "BarItem", ItemType.BarItem ) );
			this.typeBox.Items.Add( new TypeBoxItem( "ParentBarItem", ItemType.ParentBarItem ) );
			this.typeBox.Items.Add( new TypeBoxItem( "DropDownBarItem", ItemType.DropDownBarItem ) );
			this.typeBox.Items.Add( new TypeBoxItem( "ComboBoxBarItem", ItemType.ComboBoxBarItem ) );
			this.typeBox.Items.Add( new TypeBoxItem( "ListBarItem", ItemType.ListBarItem ) );
			this.typeBox.Items.Add( new TypeBoxItem( "StaticBarItem", ItemType.StaticBarItem ) );
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.typeBox = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.nameBox = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(48, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Type:";
			// 
			// typeBox
			// 
			this.typeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.typeBox.DropDownWidth = 136;
			this.typeBox.Location = new System.Drawing.Point(88, 8);
			this.typeBox.Name = "typeBox";
			this.typeBox.Size = new System.Drawing.Size(136, 21);
			this.typeBox.TabIndex = 1;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 40);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(72, 16);
			this.label3.TabIndex = 0;
			this.label3.Text = "Name:";
			// 
			// button1
			// 
			this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.button1.Location = new System.Drawing.Point(56, 80);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(80, 24);
			this.button1.TabIndex = 3;
			this.button1.Text = "&OK";
			// 
			// button2
			// 
			this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.button2.Location = new System.Drawing.Point(152, 80);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(72, 24);
			this.button2.TabIndex = 4;
			this.button2.Text = "&Cancel";
			// 
			// nameBox
			// 
			this.nameBox.Location = new System.Drawing.Point(88, 40);
			this.nameBox.Name = "nameBox";
			this.nameBox.Size = new System.Drawing.Size(136, 20);
			this.nameBox.TabIndex = 2;
			this.nameBox.Text = "";
			this.nameBox.Validating += new System.ComponentModel.CancelEventHandler(this.nameBox_Validating);
			// 
			// NewBarItemDialog
			// 
			this.AcceptButton = this.button1;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.button2;
			this.ClientSize = new System.Drawing.Size(244, 113);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.button2,
																		  this.button1,
																		  this.nameBox,
																		  this.label3,
																		  this.typeBox,
																		  this.label1});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "NewBarItemDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Add New BarItem";
			this.Load += new System.EventHandler(this.NewBarItemDialog_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void NewBarItemDialog_Load(object sender, System.EventArgs e)
		{
			this.typeBox.SelectedIndex = 0;
		}

		private void nameBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if(this.nameBox.Text == "-")
			{
				MessageBox.Show("- is not a valid Text. To create separators, right click on the BarItem in a toolbar or submenu in design time and select \"Begin Group At\" menu item in the context menu.", "XPMenus design time warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
				e.Cancel = true;
				this.nameBox.Text = "";
			}
		}
	}

	internal enum ItemType
	{
		BarItem = 0,
		ParentBarItem = 1,
		DropDownBarItem = 2,
		ComboBoxBarItem = 3,
		ListBarItem = 4,
		StaticBarItem = 5,
		MdiListBarItem = 6,
		ToolbarListBarItem = 7,
		TextBoxBarItem = 8,
	}

	internal class TypeBoxItem
	{
		private string m_sText = string.Empty;
		private ItemType m_itType = ItemType.BarItem;

		public TypeBoxItem( string text, ItemType type )
		{
			this.Text = text;
			this.Type = type;
		}

		public string Text
		{
			get
			{
				return m_sText;
			}
			set
			{
				if( m_sText != value )
				{
					m_sText = value;
				}
			}
		}

		public ItemType Type
		{
			get
			{
				return m_itType;
			}
			set
			{
				if( m_itType != value )
				{
					m_itType = value;
				}
			}
		}
		
		public override string ToString()
		{
			return this.Text;
		}
	}
}