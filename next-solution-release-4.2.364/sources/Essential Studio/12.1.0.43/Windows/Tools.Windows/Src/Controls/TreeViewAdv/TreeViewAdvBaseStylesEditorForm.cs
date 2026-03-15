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
using System.ComponentModel.Design;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// Summary description for TreeViewAdvBaseStylesEditorForm.
	/// </summary>
	[Documentation.DocumentationExclude()]
	public class TreeViewAdvBaseStylesEditorForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.PropertyGrid propertyGrid1;
		private System.Windows.Forms.Label propLabel;
		private System.Windows.Forms.Label memLabel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button addBtn;
		private System.Windows.Forms.Button removeBtn;
		private static string ResetHintString = "Right Click to Reset";
		private string propBaseString;
		private Hashtable baseStyles;
		private TreeViewAdv treeView;
		private Syncfusion.Windows.Forms.Tools.EditableList styleNamesList;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Button closeBtn;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.Button addNodeLevelBtn;
		private PropertyGridContextMenu pgMenu;

		protected TreeViewAdvBaseStylesEditorForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}
		public TreeViewAdvBaseStylesEditorForm(TreeViewAdv treeView)
		{
			InitializeComponent();

			this.treeView = treeView;
			this.baseStyles = treeView.BaseStyles;
			this.pgMenu = new PropertyGridContextMenu(this.propertyGrid1);
		}

		/// <summary>
		/// Cleans up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(this.pgMenu != null)
				{
					this.pgMenu.Dispose();
					this.pgMenu = null;
				}
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
			this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
			this.propLabel = new System.Windows.Forms.Label();
			this.memLabel = new System.Windows.Forms.Label();
			this.styleNamesList = new Syncfusion.Windows.Forms.Tools.EditableList();
			this.label1 = new System.Windows.Forms.Label();
			this.addBtn = new System.Windows.Forms.Button();
			this.removeBtn = new System.Windows.Forms.Button();
			this.closeBtn = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.addNodeLevelBtn = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// propertyGrid1
			// 
			this.propertyGrid1.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.propertyGrid1.CommandsVisibleIfAvailable = true;
			this.propertyGrid1.LargeButtons = false;
			this.propertyGrid1.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.propertyGrid1.Location = new System.Drawing.Point(224, 48);
			this.propertyGrid1.Name = "propertyGrid1";
			this.propertyGrid1.Size = new System.Drawing.Size(240, 368);
			this.propertyGrid1.TabIndex = 6;
			this.propertyGrid1.Text = "propertyGrid1";
			this.propertyGrid1.ToolbarVisible = false;
			this.propertyGrid1.ViewBackColor = System.Drawing.SystemColors.Window;
			this.propertyGrid1.ViewForeColor = System.Drawing.SystemColors.WindowText;
			this.propertyGrid1.SelectedObjectsChanged += new System.EventHandler(this.propertyGrid1_SelectedObjectsChanged);
			// 
			// propLabel
			// 
			this.propLabel.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.propLabel.Location = new System.Drawing.Point(224, 8);
			this.propLabel.Name = "propLabel";
			this.propLabel.Size = new System.Drawing.Size(240, 32);
			this.propLabel.TabIndex = 5;
			this.propLabel.Text = "&Properties";
			// 
			// memLabel
			// 
			this.memLabel.Location = new System.Drawing.Point(8, 8);
			this.memLabel.Name = "memLabel";
			this.memLabel.Size = new System.Drawing.Size(200, 32);
			this.memLabel.TabIndex = 1;
			this.memLabel.Text = "&BaseStyles: (F2 or Mouse Click to edit)";
			// 
			// styleNamesList
			// 
			this.styleNamesList.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left);
			// 
			// styleNamesList.Button
			// 
			this.styleNamesList.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.styleNamesList.Button.Location = new System.Drawing.Point(112, 120);
			this.styleNamesList.Button.Name = "button";
			this.styleNamesList.Button.Size = new System.Drawing.Size(30, 20);
			this.styleNamesList.Button.TabIndex = 2;
			this.styleNamesList.Button.Text = "...";
			this.styleNamesList.Button.Visible = false;
			this.styleNamesList.Controls.AddRange(new System.Windows.Forms.Control[] {
																						 this.styleNamesList.Button,
																						 this.styleNamesList.TextBox,
																						 this.styleNamesList.ListBox});
			// 
			// styleNamesList.ListBox
			// 
			this.styleNamesList.ListBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.styleNamesList.ListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.styleNamesList.ListBox.Name = "listBox";
			this.styleNamesList.ListBox.Size = new System.Drawing.Size(200, 303);
			this.styleNamesList.ListBox.TabIndex = 0;
			this.styleNamesList.ListBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.styleNamesList_ListBox_MouseMove);
			this.styleNamesList.ListBox.SelectedIndexChanged += new System.EventHandler(this.styleNamesList_ListBox_SelectedIndexChanged);
			this.styleNamesList.Location = new System.Drawing.Point(8, 48);
			this.styleNamesList.Name = "styleNamesList";
			this.styleNamesList.Size = new System.Drawing.Size(200, 312);
			this.styleNamesList.TabIndex = 2;
			// 
			// styleNamesList.TextBox
			// 
			this.styleNamesList.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.styleNamesList.TextBox.Location = new System.Drawing.Point(8, 120);
			this.styleNamesList.TextBox.Name = "textBox";
			this.styleNamesList.TextBox.TabIndex = 2;
			this.styleNamesList.TextBox.Visible = false;
			this.styleNamesList.WantButton = false;
			this.styleNamesList.ItemChanging += new Syncfusion.Windows.Forms.Tools.ListBoxTextChangingEventHandler(this.styleNamesList_ItemChanging);
			this.styleNamesList.BeforeListItemEdit += new System.ComponentModel.CancelEventHandler(this.styleNamesList_BeforeListItemEdit);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label1.Location = new System.Drawing.Point(8, 424);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(456, 3);
			this.label1.TabIndex = 8;
			// 
			// addBtn
			// 
			this.addBtn.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.addBtn.Location = new System.Drawing.Point(112, 360);
			this.addBtn.Name = "addBtn";
			this.addBtn.Size = new System.Drawing.Size(96, 23);
			this.addBtn.TabIndex = 3;
			this.addBtn.Text = "&Add Base Style";
			this.addBtn.Click += new System.EventHandler(this.addBtn_Click);
			// 
			// removeBtn
			// 
			this.removeBtn.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.removeBtn.Location = new System.Drawing.Point(72, 392);
			this.removeBtn.Name = "removeBtn";
			this.removeBtn.TabIndex = 4;
			this.removeBtn.Text = "&Remove";
			this.removeBtn.Click += new System.EventHandler(this.removeBtn_Click);
			// 
			// closeBtn
			// 
			this.closeBtn.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.closeBtn.Location = new System.Drawing.Point(376, 432);
			this.closeBtn.Name = "closeBtn";
			this.closeBtn.TabIndex = 7;
			this.closeBtn.Text = "Close";
			this.closeBtn.Click += new System.EventHandler(this.closeBtn_Click);
			// 
			// addNodeLevelBtn
			// 
			this.addNodeLevelBtn.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.addNodeLevelBtn.Location = new System.Drawing.Point(8, 360);
			this.addNodeLevelBtn.Name = "addNodeLevelBtn";
			this.addNodeLevelBtn.Size = new System.Drawing.Size(96, 23);
			this.addNodeLevelBtn.TabIndex = 9;
			this.addNodeLevelBtn.Text = "&Add Level Style";
			this.addNodeLevelBtn.Click += new System.EventHandler(this.addNodeLevelBtn_Click);
			// 
			// TreeViewAdvBaseStylesEditorForm
			// 
			this.AcceptButton = this.closeBtn;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(472, 461);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.addNodeLevelBtn,
																		  this.closeBtn,
																		  this.removeBtn,
																		  this.addBtn,
																		  this.label1,
																		  this.styleNamesList,
																		  this.memLabel,
																		  this.propLabel,
																		  this.propertyGrid1});
			this.Name = "TreeViewAdvBaseStylesEditorForm";
			this.Text = "BaseStyles Collection Editor";
			this.Load += new System.EventHandler(this.TreeViewAdvBaseStylesEditorForm_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private ListBox ListBox
		{
			get{return this.styleNamesList.ListBox;}
		}
		private void TreeViewAdvBaseStylesEditorForm_Load(object sender, System.EventArgs e)
		{
			this.propBaseString = this.propLabel.Text;

			this.FillEditor();
		}

		private void FillEditor()
		{
			foreach(string name in this.baseStyles.Keys)
			{
				this.styleNamesList.ListBox.Items.Add(name);
			}
			if(this.ListBox.Items.Count > 0)
				this.ListBox.SelectedIndex = 0;
		}

		#region UTILS
		private int HitTestListBox(Point ptClient)
		{
			int ht = 0;
			int i = 0;
			for(i = 0; i < this.ListBox.Items.Count; i++)
			{
				ht += this.ListBox.ItemHeight;
				if(ht < ptClient.Y)
					continue;
				else
					break;
			}
			if(i >= this.ListBox.Items.Count)
				return -1;
			else
				return i;
		}
		private void AddNewBaseStyle(int type)
		{
			string newName = this.GenerateNewBaseStyleName(type);
			this.baseStyles[newName] = new TreeNodeAdvStyleInfo(new TreeViewAdvStyleInfoIdentity(this.treeView));
			int newIndex = this.ListBox.Items.Add(newName);
			this.ListBox.SelectedIndex = newIndex;
		}
		
		private string GenerateNewBaseStyleName(int type)
		{
			string baseStyleName = TreeViewAdv.BaseStyleBaseName;
			if(type == 1)
				baseStyleName = TreeViewAdv.NodeLevelStyleBaseName;
			string newName = String.Empty;
			int i = 0;
			while(true)
			{
				i++;
				newName = baseStyleName + i.ToString();
				if(this.baseStyles.Contains(newName))
					continue;
				else
					break;
			}
			return newName;
		}
		private bool IsValidBaseStyleName(string name)
		{
			if(name == null || name.Length == 0)
				return false;

			if(this.baseStyles.Contains(name))
				return false;
			else return true;
		}
		private int tooltipIndex;
		private int ToolTipIndex
		{
			get{return this.tooltipIndex;}
			set
			{
				if(this.tooltipIndex != value)
				{
					this.tooltipIndex = value;
					string tooltip = String.Empty;
					if(tooltipIndex != -1)
						tooltip = this.treeView.GetHintTextForStyle(this.ListBox.Items[tooltipIndex].ToString());

					this.toolTip1.SetToolTip(this.ListBox, tooltip);
				}
			}
		}
		#endregion UTILS
		#region EVENTS
		private void styleNamesList_BeforeListItemEdit(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if(!this.treeView.IsBaseStyleRemoveable(this.ListBox.SelectedItem.ToString()))
			{
				e.Cancel = true;
				MessageBox.Show("Cannot edit this base style.");
			}
		}

		private void styleNamesList_ItemChanging(object sender, Syncfusion.Windows.Forms.Tools.ListBoxTextChangingEventArgs e)
		{
			if(!this.IsValidBaseStyleName(e.NewText))
			{
				e.Cancel = true;
				MessageBox.Show("New Style Name is not unique, please provide a unique style name.");
			}
			else
			{
				string oldStyleName = this.ListBox.SelectedItem.ToString();
				this.baseStyles[e.NewText] = this.baseStyles[oldStyleName];
				this.baseStyles.Remove(oldStyleName);

				this.ToolTipIndex = -1;
			}
		}

		private void styleNamesList_ListBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if(this.ListBox.SelectedIndex == -1)
				return;

			this.propertyGrid1.SelectedObject = this.baseStyles[this.ListBox.SelectedItem.ToString()];
		}

		private void addBtn_Click(object sender, System.EventArgs e)
		{
			this.AddNewBaseStyle(0);
		}
		
		private void addNodeLevelBtn_Click(object sender, System.EventArgs e)
		{
			this.AddNewBaseStyle(1);
		}

		private void removeBtn_Click(object sender, System.EventArgs e)
		{
			if(this.ListBox.SelectedIndex == -1)
				return;

			string styleName = this.ListBox.SelectedItem.ToString();
			if(this.treeView.IsBaseStyleRemoveable(styleName))
			{
				this.baseStyles.Remove(styleName);
				this.ListBox.Items.RemoveAt(this.ListBox.SelectedIndex);
				this.ListBox.SelectedIndex = this.ListBox.Items.Count - 1;
			}
			else
				MessageBox.Show("Cannot remove selected base style.");

			this.ToolTipIndex = -1;
		}
		
		private void closeBtn_Click(object sender, System.EventArgs e)
		{
			this.treeView.MakeDirty();
			this.Close();
		}
		private void propertyGrid1_SelectedObjectsChanged(object sender, System.EventArgs e)
		{
			if(this.ListBox.SelectedIndex == -1)
				this.propLabel.Text = this.propBaseString + ":" + "(" + ResetHintString + ")";
			else
				this.propLabel.Text = this.propBaseString + " for style " + this.ListBox.SelectedItem.ToString() + ":"
					+ "(" + ResetHintString + ")";
		}

		private void styleNamesList_ListBox_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			this.ToolTipIndex = this.HitTestListBox(new Point(e.X, e.Y));
		}
		#endregion EVENTS
	}
}
