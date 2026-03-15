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

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using System.Windows.Forms;
namespace Syncfusion.Windows.Forms.Tools
{
    partial class Office2013CustomizeQuickItemsDialog
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if( disposing && ( components != null ) )
			{
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		internal class QATListView:
			ListView
		{
			new public void RecreateHandle()
			{
				base.RecreateHandle();
			}

			public QATListView()
				: base()
			{
				this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			}
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Office2013CustomizeQuickItemsDialog));
            this.lblChooseCommandsFrom = new System.Windows.Forms.Label();
            this.comboPanels = new Syncfusion.Windows.Forms.Tools.ComboBoxAdv();
            this.lstAvailableItems = new Syncfusion.Windows.Forms.Tools.Office2013CustomizeQuickItemsDialog.QATListView();
            this.lstChosenItems = new Syncfusion.Windows.Forms.Tools.Office2013CustomizeQuickItemsDialog.QATListView();
            this.btnAdd = new Syncfusion.Windows.Forms.ButtonAdv();
            this.btnRemove = new Syncfusion.Windows.Forms.ButtonAdv();
            this.chkPlaceBelowRibbon = new Syncfusion.Windows.Forms.Tools.CheckBoxAdv();
            this.btnCancel = new Syncfusion.Windows.Forms.ButtonAdv();
            this.btnOK = new Syncfusion.Windows.Forms.ButtonAdv();
            this.btnReset = new Syncfusion.Windows.Forms.ButtonAdv();
            this.btnUp = new Syncfusion.Windows.Forms.ButtonAdv();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.btnDown = new Syncfusion.Windows.Forms.ButtonAdv();
            this.sfAvailableItems = new Syncfusion.Windows.Forms.ScrollersFrame(this.components);
            this.sfChosenItems = new Syncfusion.Windows.Forms.ScrollersFrame(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.comboPanels)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPlaceBelowRibbon)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblChooseCommandsFrom
            // 
            resources.ApplyResources(this.lblChooseCommandsFrom, "lblChooseCommandsFrom");
            this.lblChooseCommandsFrom.BackColor = System.Drawing.Color.White;
            this.lblChooseCommandsFrom.Name = "lblChooseCommandsFrom";
            // 
            // comboPanels
            // 
            this.comboPanels.BackColor = System.Drawing.Color.White;
            this.comboPanels.DisplayMember = "Text";
            this.comboPanels.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboPanels, "comboPanels");
            this.comboPanels.MetroColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(158)))), ((int)(((byte)(218)))));
            this.comboPanels.Name = "comboPanels";
            this.comboPanels.Style = Syncfusion.Windows.Forms.VisualStyle.OfficeXP;
            this.comboPanels.SelectedIndexChanged += new System.EventHandler(this.OnComboToolstripsSelectedIndexChanged);
            // 
            // lstAvailableItems
            // 
            resources.ApplyResources(this.lstAvailableItems, "lstAvailableItems");
            this.lstAvailableItems.Name = "lstAvailableItems";
            this.lstAvailableItems.UseCompatibleStateImageBehavior = false;
            this.lstAvailableItems.View = System.Windows.Forms.View.List;
            this.lstAvailableItems.SelectedIndexChanged += new System.EventHandler(this.OnLstAvailableItemsSelectedIndexChanged);
            this.lstAvailableItems.DoubleClick += new System.EventHandler(this.OnLstAvailableItemsDoubleClick);
            // 
            // lstChosenItems
            // 
            resources.ApplyResources(this.lstChosenItems, "lstChosenItems");
            this.lstChosenItems.HideSelection = false;
            this.lstChosenItems.Name = "lstChosenItems";
            this.lstChosenItems.UseCompatibleStateImageBehavior = false;
            this.lstChosenItems.View = System.Windows.Forms.View.List;
            this.lstChosenItems.SelectedIndexChanged += new System.EventHandler(this.OnLstChosenItemsSelectedIndexChanged);
            this.lstChosenItems.DoubleClick += new System.EventHandler(this.OnLstChosenItemsDoubleClick);
            // 
            // btnAdd
            // 
            resources.ApplyResources(this.btnAdd, "btnAdd");
            this.btnAdd.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Office2007;
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.UseVisualStyle = false;
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.OnBtnAddClick);
            // 
            // btnRemove
            // 
            resources.ApplyResources(this.btnRemove, "btnRemove");
            this.btnRemove.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Office2007;
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.UseVisualStyle = false;
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.OnBtnRemoveClick);
            // 
            // chkPlaceBelowRibbon
            // 
            resources.ApplyResources(this.chkPlaceBelowRibbon, "chkPlaceBelowRibbon");
            this.chkPlaceBelowRibbon.BackColor = System.Drawing.Color.White;
            this.chkPlaceBelowRibbon.MetroColor = System.Drawing.Color.SkyBlue;
            this.chkPlaceBelowRibbon.Name = "chkPlaceBelowRibbon";
            this.chkPlaceBelowRibbon.ThemesEnabled = false;
            this.chkPlaceBelowRibbon.CheckStateChanged += new System.EventHandler(this.chkPlaceBelowRibbon_CheckStateChanged);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Office2007;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyle = false;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Office2007;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyle = false;
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.OnBtnOKClick);
            // 
            // btnReset
            // 
            resources.ApplyResources(this.btnReset, "btnReset");
            this.btnReset.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Office2007;
            this.btnReset.Name = "btnReset";
            this.btnReset.UseVisualStyle = false;
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnUp
            // 
            resources.ApplyResources(this.btnUp, "btnUp");
            this.btnUp.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Office2007;
            this.btnUp.ImageList = this.imageList;
            this.btnUp.Name = "btnUp";
            this.btnUp.UseVisualStyle = false;
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Magenta;
            this.imageList.Images.SetKeyName(0, "");
            this.imageList.Images.SetKeyName(1, "");
            // 
            // btnDown
            // 
            resources.ApplyResources(this.btnDown, "btnDown");
            this.btnDown.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Office2007;
            this.btnDown.ImageList = this.imageList;
            this.btnDown.Name = "btnDown";
            this.btnDown.UseVisualStyle = false;
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // sfAvailableItems
            // 
            this.sfAvailableItems.AttachedTo = this.lstAvailableItems;
            this.sfAvailableItems.CustomRender = null;
            this.sfAvailableItems.MetroColorScheme = Syncfusion.Windows.Forms.MetroColorScheme.Managed;
            this.sfAvailableItems.SizeGripperVisibility = Syncfusion.Windows.Forms.SizeGripperVisibility.Auto;
            this.sfAvailableItems.VisualStyle = Syncfusion.Windows.Forms.ScrollBarCustomDrawStyles.Metro;
            // 
            // sfChosenItems
            // 
            this.sfChosenItems.AttachedTo = this.lstChosenItems;
            this.sfChosenItems.CustomRender = null;
            this.sfChosenItems.MetroColorScheme = Syncfusion.Windows.Forms.MetroColorScheme.Managed;
            this.sfChosenItems.SizeGripperVisibility = Syncfusion.Windows.Forms.SizeGripperVisibility.Auto;
            this.sfChosenItems.VisualStyle = Syncfusion.Windows.Forms.ScrollBarCustomDrawStyles.Metro;
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.comboPanels);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.lblChooseCommandsFrom);
            this.panel1.Controls.Add(this.btnReset);
            this.panel1.Controls.Add(this.lstChosenItems);
            this.panel1.Controls.Add(this.btnRemove);
            this.panel1.Controls.Add(this.btnDown);
            this.panel1.Controls.Add(this.btnUp);
            this.panel1.Controls.Add(this.btnAdd);
            this.panel1.Controls.Add(this.lstAvailableItems);
            this.panel1.Controls.Add(this.chkPlaceBelowRibbon);
            this.panel1.Name = "panel1";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // Office2013CustomizeQuickItemsDialog
            // 
            this.AcceptButton = this.btnOK;
            
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Office2013CustomizeQuickItemsDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(Office2013CustomizeQuickItemsDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.comboPanels)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPlaceBelowRibbon)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

		}

     

    

		#endregion

		private System.Windows.Forms.Label lblChooseCommandsFrom;
		private ComboBoxAdv comboPanels;
		private QATListView lstAvailableItems;
		private QATListView lstChosenItems;
		private ButtonAdv btnAdd;
		private ButtonAdv btnRemove;
		private CheckBoxAdv chkPlaceBelowRibbon;
		private ButtonAdv btnCancel;
		private ButtonAdv btnOK;
		private ButtonAdv btnReset;
		private ButtonAdv btnUp;
		private ButtonAdv btnDown;
		private System.Windows.Forms.ImageList imageList;
		private ScrollersFrame sfAvailableItems;
		private ScrollersFrame sfChosenItems;
        private Panel panel1;
        private Label label1;
        private PictureBox pictureBox1;
        private Label label2;
        private Label label3;
	}
}
#endif