#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using Syncfusion.Windows.Forms.Tools;

namespace Syncfusion.Windows.Forms
{
    [Syncfusion.Documentation.DocumentationExclude()]
    public class MdiWindowDialog : System.Windows.Forms.Form
    {
        public System.Windows.Forms.ListBox ItemList;
        private Form active;
        public System.Windows.Forms.Button CancelBtn;
        public System.Windows.Forms.Button OkBtn;

        private System.ComponentModel.Container components = null;

        public MdiWindowDialog()
        {
            // Required for Windows Form Designer support
            InitializeComponent();

            InitLocalizedResources();

            // TODO: Add any constructor code after InitializeComponent call
        }

        protected virtual void InitLocalizedResources()
        {
            this.Text = SR.GetString(SR.MdiWindowDialogCaption,this);
            OkBtn.Text = SR.GetString(SR.MdiListActivateButton, this);
            CancelBtn.Text = SR.GetString(SR.MdiListCancelButton, this);
        }

        // Properties
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Form ActiveChildForm
        {
            get
            {
                return this.active;
            }
        }
        public void SetItems(Form active, Form[] allChildForms)
        {
            int activeFormsIndex = -1;
            foreach (Form childForm in allChildForms)
            {
                int curItemIndex = this.ItemList.Items.Add(new ListItem(childForm));
                if (childForm == active)
                    activeFormsIndex = curItemIndex;
            }

            this.active = active;
            this.ItemList.SelectedIndex = activeFormsIndex;
        }

        private void ItemList_doubleClick(object source, EventArgs e)
        {
            this.OkBtn.PerformClick();
        }

        private void ItemList_selectedIndexChanged(object source, EventArgs e)
        {
            ListItem listItem = (ListItem)this.ItemList.SelectedItem;
            if (listItem != null)
                this.active = listItem.Form;
        }

        /// <summary>
        /// Cleans up any resources being used.
        /// </summary>
        /// <param name="disposing"> Bool property disposing.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MdiWindowDialog));
            this.ItemList = new System.Windows.Forms.ListBox();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.OkBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // itemList
            // 
            this.ItemList.AccessibleDescription = ((string)(resources.GetObject("itemList.AccessibleDescription")));
            this.ItemList.AccessibleName = ((string)(resources.GetObject("itemList.AccessibleName")));
            this.ItemList.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("itemList.Anchor")));
            this.ItemList.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("itemList.BackgroundImage")));
            this.ItemList.ColumnWidth = ((int)(resources.GetObject("itemList.ColumnWidth")));
            this.ItemList.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("itemList.Dock")));
            this.ItemList.Enabled = ((bool)(resources.GetObject("itemList.Enabled")));
            this.ItemList.Font = ((System.Drawing.Font)(resources.GetObject("itemList.Font")));
            this.ItemList.HorizontalExtent = ((int)(resources.GetObject("itemList.HorizontalExtent")));
            this.ItemList.HorizontalScrollbar = ((bool)(resources.GetObject("itemList.HorizontalScrollbar")));
            this.ItemList.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("itemList.ImeMode")));
            this.ItemList.IntegralHeight = ((bool)(resources.GetObject("itemList.IntegralHeight")));
            this.ItemList.ItemHeight = ((int)(resources.GetObject("itemList.ItemHeight")));
            this.ItemList.Location = ((System.Drawing.Point)(resources.GetObject("itemList.Location")));
            this.ItemList.Name = "itemList";
            this.ItemList.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("itemList.RightToLeft")));
            this.ItemList.ScrollAlwaysVisible = ((bool)(resources.GetObject("itemList.ScrollAlwaysVisible")));
            this.ItemList.Size = ((System.Drawing.Size)(resources.GetObject("itemList.Size")));
            this.ItemList.TabIndex = ((int)(resources.GetObject("itemList.TabIndex")));
            this.ItemList.Visible = ((bool)(resources.GetObject("itemList.Visible")));
            this.ItemList.DoubleClick += new System.EventHandler(this.ItemList_doubleClick);
            this.ItemList.SelectedIndexChanged += new System.EventHandler(this.ItemList_selectedIndexChanged);
            // 
            // CancelBtn
            // 
            this.CancelBtn.AccessibleDescription = ((string)(resources.GetObject("CancelBtn.AccessibleDescription")));
            this.CancelBtn.AccessibleName = ((string)(resources.GetObject("CancelBtn.AccessibleName")));
            this.CancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("CancelBtn.Anchor")));
            this.CancelBtn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("CancelBtn.BackgroundImage")));
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBtn.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("CancelBtn.Dock")));
            this.CancelBtn.Enabled = ((bool)(resources.GetObject("CancelBtn.Enabled")));
            this.CancelBtn.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("CancelBtn.FlatStyle")));
            this.CancelBtn.Font = ((System.Drawing.Font)(resources.GetObject("CancelBtn.Font")));
            this.CancelBtn.Image = ((System.Drawing.Image)(resources.GetObject("CancelBtn.Image")));
            this.CancelBtn.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("CancelBtn.ImageAlign")));
            this.CancelBtn.ImageIndex = ((int)(resources.GetObject("CancelBtn.ImageIndex")));
            this.CancelBtn.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("CancelBtn.ImeMode")));
            this.CancelBtn.Location = ((System.Drawing.Point)(resources.GetObject("CancelBtn.Location")));
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("CancelBtn.RightToLeft")));
            this.CancelBtn.Size = ((System.Drawing.Size)(resources.GetObject("CancelBtn.Size")));
            this.CancelBtn.TabIndex = ((int)(resources.GetObject("CancelBtn.TabIndex")));
            this.CancelBtn.Text = resources.GetString("CancelBtn.Text");
            this.CancelBtn.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("CancelBtn.TextAlign")));
            this.CancelBtn.Visible = ((bool)(resources.GetObject("CancelBtn.Visible")));
            // 
            // OkBtn
            // 
            this.OkBtn.AccessibleDescription = ((string)(resources.GetObject("OkBtn.AccessibleDescription")));
            this.OkBtn.AccessibleName = ((string)(resources.GetObject("OkBtn.AccessibleName")));
            this.OkBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("OkBtn.Anchor")));
            this.OkBtn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("OkBtn.BackgroundImage")));
            this.OkBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OkBtn.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("OkBtn.Dock")));
            this.OkBtn.Enabled = ((bool)(resources.GetObject("OkBtn.Enabled")));
            this.OkBtn.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("OkBtn.FlatStyle")));
            this.OkBtn.Font = ((System.Drawing.Font)(resources.GetObject("OkBtn.Font")));
            this.OkBtn.Image = ((System.Drawing.Image)(resources.GetObject("OkBtn.Image")));
            this.OkBtn.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("OkBtn.ImageAlign")));
            this.OkBtn.ImageIndex = ((int)(resources.GetObject("OkBtn.ImageIndex")));
            this.OkBtn.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("OkBtn.ImeMode")));
            this.OkBtn.Location = ((System.Drawing.Point)(resources.GetObject("OkBtn.Location")));
            this.OkBtn.Name = "OkBtn";
            this.OkBtn.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("OkBtn.RightToLeft")));
            this.OkBtn.Size = ((System.Drawing.Size)(resources.GetObject("OkBtn.Size")));
            this.OkBtn.TabIndex = ((int)(resources.GetObject("OkBtn.TabIndex")));
            this.OkBtn.Text = resources.GetString("OkBtn.Text");
            this.OkBtn.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("OkBtn.TextAlign")));
            this.OkBtn.Visible = ((bool)(resources.GetObject("OkBtn.Visible")));
            // 
            // MdiWindowDialog
            // 
            this.AcceptButton = this.OkBtn;
            this.AccessibleDescription = ((string)(resources.GetObject("$this.AccessibleDescription")));
            this.AccessibleName = ((string)(resources.GetObject("$this.AccessibleName")));
            this.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("$this.Anchor")));
            this.AutoScaleBaseSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScaleBaseSize")));
            this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
            this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
            this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.CancelButton = this.CancelBtn;
            this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.OkBtn,
																		  this.CancelBtn,
																		  this.ItemList});
            this.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("$this.Dock")));
            this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
            this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
            this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
            this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
            this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
            this.Name = "MdiWindowDialog";
            this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
            this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
            this.Text = resources.GetString("$this.Text");
            this.Visible = ((bool)(resources.GetObject("$this.Visible")));
            this.ResumeLayout(false);

        }
        #endregion
    }
    public class ListItem
    {
        // Fields
        public Form Form;

        // Constructors
        public ListItem(Form f)
        {
            this.Form = f;
        }

        // Methods
        public override /*Object*/ string ToString()
        {
            return this.Form.Text;
        }
    }
}