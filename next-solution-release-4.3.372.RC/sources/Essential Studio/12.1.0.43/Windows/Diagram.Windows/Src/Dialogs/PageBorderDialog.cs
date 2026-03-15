#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.Diagram;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// The PageBorderDialog provides an interactive form-based interface for setting the page borders of a diagram. Initializing the 
    /// PageBorderDialog's <see cref="Syncfusion.Windows.Forms.Diagram.PageBorderDialog.PageBorderStyle"/> property with the corresponding 
    /// <see cref="Syncfusion.Windows.Forms.Diagram.View.PageBorderStyle"/> member of the diagram's view will let users configure the 
    /// page border settings using the dialog controls.
    /// <p>
    /// Please refer to the DiagramBuilder sample to see the PageBorderDialog in use.
    /// </p>
    /// </summary>
    public class PageBorderDialog : Form
    {
        #region Constants
        private const int c_nBOUNDS_OFFSET = 2;
        private const int c_nONE_PIXEL = 1;
        private const int c_nTWO_PIXELS = 2;
        private const int c_nPREVIEW_BORDER_OFFSET_X = 10;
        private const int c_nPREVIEW_BORDER_OFFSET_Y = 15;
        private const int c_nPREVIEW_LINE_LENGHT = 80;
        #endregion Constants

        #region Class members
        private Button m_btnCancel;
        private Button m_btnOK;
        private System.Windows.Forms.Label m_lblBorderDashStyle;
        private System.Windows.Forms.Label m_lblBorderColor;
        private System.Windows.Forms.Label m_lblBorderDashWeight;
        private System.Windows.Forms.Label m_lblBorderTransparency;
        private PictureBox m_pictureBorderPreview;
        private ComboBox m_comboBSDashStyle;
        private ComboBox m_comboBSWeight;
        private TrackBar m_trackBSTransparency;
        private Button m_btnBSChooseColor;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components = null;
        private PageBorderStyle m_pageBorderStyle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private RoundingButton m_checkBSRoundingNone;
        private RoundingButton m_checkBSRoundingSmallest;
        private RoundingButton m_checkBSRoundingVerySmall;
        private RoundingButton m_checkBSRoundingSmall;
        private RoundingButton m_checkBSRoundingMedium;
        private RoundingButton m_checkBSRoundingBiggest;
        private RoundingButton m_checkBSRoundingVeryBig;
        private RoundingButton m_checkBSRoundingBig;
        private bool m_bInitializing;
        private CheckBox m_checkBSShowPageBounds;
        private CheckBox m_checkPushed;
        private bool m_bShowPageBorder;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="PageBorderDialog"/> class.
        /// </summary>
        public PageBorderDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
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
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(PageBorderDialog));
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_btnOK = new System.Windows.Forms.Button();
            this.m_comboBSDashStyle = new System.Windows.Forms.ComboBox();
            this.m_comboBSWeight = new System.Windows.Forms.ComboBox();
            this.m_lblBorderDashStyle = new System.Windows.Forms.Label();
            this.m_lblBorderColor = new System.Windows.Forms.Label();
            this.m_lblBorderDashWeight = new System.Windows.Forms.Label();
            this.m_lblBorderTransparency = new System.Windows.Forms.Label();
            this.m_trackBSTransparency = new System.Windows.Forms.TrackBar();
            this.m_pictureBorderPreview = new System.Windows.Forms.PictureBox();
            this.m_btnBSChooseColor = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.m_checkBSRoundingNone = new Syncfusion.Windows.Forms.Diagram.RoundingButton();
            this.m_checkBSRoundingSmallest = new Syncfusion.Windows.Forms.Diagram.RoundingButton();
            this.m_checkBSRoundingVerySmall = new Syncfusion.Windows.Forms.Diagram.RoundingButton();
            this.m_checkBSRoundingSmall = new Syncfusion.Windows.Forms.Diagram.RoundingButton();
            this.m_checkBSRoundingMedium = new Syncfusion.Windows.Forms.Diagram.RoundingButton();
            this.m_checkBSRoundingBiggest = new Syncfusion.Windows.Forms.Diagram.RoundingButton();
            this.m_checkBSRoundingVeryBig = new Syncfusion.Windows.Forms.Diagram.RoundingButton();
            this.m_checkBSRoundingBig = new Syncfusion.Windows.Forms.Diagram.RoundingButton();
            this.m_checkBSShowPageBounds = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.m_trackBSTransparency)).BeginInit();
            this.SuspendLayout();
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.AccessibleDescription = resources.GetString("m_btnCancel.AccessibleDescription");
            this.m_btnCancel.AccessibleName = resources.GetString("m_btnCancel.AccessibleName");
            this.m_btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("m_btnCancel.Anchor")));
            this.m_btnCancel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("m_btnCancel.BackgroundImage")));
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("m_btnCancel.Dock")));
            this.m_btnCancel.Enabled = ((bool)(resources.GetObject("m_btnCancel.Enabled")));
            this.m_btnCancel.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("m_btnCancel.FlatStyle")));
            this.m_btnCancel.Font = ((System.Drawing.Font)(resources.GetObject("m_btnCancel.Font")));
            this.m_btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("m_btnCancel.Image")));
            this.m_btnCancel.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("m_btnCancel.ImageAlign")));
            this.m_btnCancel.ImageIndex = ((int)(resources.GetObject("m_btnCancel.ImageIndex")));
            this.m_btnCancel.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("m_btnCancel.ImeMode")));
            this.m_btnCancel.Location = ((System.Drawing.Point)(resources.GetObject("m_btnCancel.Location")));
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("m_btnCancel.RightToLeft")));
            this.m_btnCancel.Size = ((System.Drawing.Size)(resources.GetObject("m_btnCancel.Size")));
            this.m_btnCancel.TabIndex = ((int)(resources.GetObject("m_btnCancel.TabIndex")));
            this.m_btnCancel.Text = resources.GetString("m_btnCancel.Text");
            this.m_btnCancel.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("m_btnCancel.TextAlign")));
            this.m_btnCancel.Visible = ((bool)(resources.GetObject("m_btnCancel.Visible")));
            // 
            // m_btnOK
            // 
            this.m_btnOK.AccessibleDescription = resources.GetString("m_btnOK.AccessibleDescription");
            this.m_btnOK.AccessibleName = resources.GetString("m_btnOK.AccessibleName");
            this.m_btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("m_btnOK.Anchor")));
            this.m_btnOK.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("m_btnOK.BackgroundImage")));
            this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOK.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("m_btnOK.Dock")));
            this.m_btnOK.Enabled = ((bool)(resources.GetObject("m_btnOK.Enabled")));
            this.m_btnOK.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("m_btnOK.FlatStyle")));
            this.m_btnOK.Font = ((System.Drawing.Font)(resources.GetObject("m_btnOK.Font")));
            this.m_btnOK.Image = ((System.Drawing.Image)(resources.GetObject("m_btnOK.Image")));
            this.m_btnOK.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("m_btnOK.ImageAlign")));
            this.m_btnOK.ImageIndex = ((int)(resources.GetObject("m_btnOK.ImageIndex")));
            this.m_btnOK.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("m_btnOK.ImeMode")));
            this.m_btnOK.Location = ((System.Drawing.Point)(resources.GetObject("m_btnOK.Location")));
            this.m_btnOK.Name = "m_btnOK";
            this.m_btnOK.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("m_btnOK.RightToLeft")));
            this.m_btnOK.Size = ((System.Drawing.Size)(resources.GetObject("m_btnOK.Size")));
            this.m_btnOK.TabIndex = ((int)(resources.GetObject("m_btnOK.TabIndex")));
            this.m_btnOK.Text = resources.GetString("m_btnOK.Text");
            this.m_btnOK.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("m_btnOK.TextAlign")));
            this.m_btnOK.Visible = ((bool)(resources.GetObject("m_btnOK.Visible")));
            // 
            // m_comboBSDashStyle
            // 
            this.m_comboBSDashStyle.AccessibleDescription = resources.GetString("m_comboBSDashStyle.AccessibleDescription");
            this.m_comboBSDashStyle.AccessibleName = resources.GetString("m_comboBSDashStyle.AccessibleName");
            this.m_comboBSDashStyle.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("m_comboBSDashStyle.Anchor")));
            this.m_comboBSDashStyle.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("m_comboBSDashStyle.BackgroundImage")));
            this.m_comboBSDashStyle.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("m_comboBSDashStyle.Dock")));
            this.m_comboBSDashStyle.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.m_comboBSDashStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_comboBSDashStyle.Enabled = ((bool)(resources.GetObject("m_comboBSDashStyle.Enabled")));
            this.m_comboBSDashStyle.Font = ((System.Drawing.Font)(resources.GetObject("m_comboBSDashStyle.Font")));
            this.m_comboBSDashStyle.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("m_comboBSDashStyle.ImeMode")));
            this.m_comboBSDashStyle.IntegralHeight = ((bool)(resources.GetObject("m_comboBSDashStyle.IntegralHeight")));
            this.m_comboBSDashStyle.ItemHeight = ((int)(resources.GetObject("m_comboBSDashStyle.ItemHeight")));
            this.m_comboBSDashStyle.Location = ((System.Drawing.Point)(resources.GetObject("m_comboBSDashStyle.Location")));
            this.m_comboBSDashStyle.MaxDropDownItems = ((int)(resources.GetObject("m_comboBSDashStyle.MaxDropDownItems")));
            this.m_comboBSDashStyle.MaxLength = ((int)(resources.GetObject("m_comboBSDashStyle.MaxLength")));
            this.m_comboBSDashStyle.Name = "m_comboBSDashStyle";
            this.m_comboBSDashStyle.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("m_comboBSDashStyle.RightToLeft")));
            this.m_comboBSDashStyle.Size = ((System.Drawing.Size)(resources.GetObject("m_comboBSDashStyle.Size")));
            this.m_comboBSDashStyle.TabIndex = ((int)(resources.GetObject("m_comboBSDashStyle.TabIndex")));
            this.m_comboBSDashStyle.Text = resources.GetString("m_comboBSDashStyle.Text");
            this.m_comboBSDashStyle.Visible = ((bool)(resources.GetObject("m_comboBSDashStyle.Visible")));
            this.m_comboBSDashStyle.SelectedIndexChanged += new System.EventHandler(this.BSDashStyle_SelectedIndexChanged);
            this.m_comboBSDashStyle.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.BSDashStyle_DrawItem);
            // 
            // m_comboBSWeight
            // 
            this.m_comboBSWeight.AccessibleDescription = resources.GetString("m_comboBSWeight.AccessibleDescription");
            this.m_comboBSWeight.AccessibleName = resources.GetString("m_comboBSWeight.AccessibleName");
            this.m_comboBSWeight.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("m_comboBSWeight.Anchor")));
            this.m_comboBSWeight.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("m_comboBSWeight.BackgroundImage")));
            this.m_comboBSWeight.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("m_comboBSWeight.Dock")));
            this.m_comboBSWeight.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.m_comboBSWeight.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_comboBSWeight.Enabled = ((bool)(resources.GetObject("m_comboBSWeight.Enabled")));
            this.m_comboBSWeight.Font = ((System.Drawing.Font)(resources.GetObject("m_comboBSWeight.Font")));
            this.m_comboBSWeight.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("m_comboBSWeight.ImeMode")));
            this.m_comboBSWeight.IntegralHeight = ((bool)(resources.GetObject("m_comboBSWeight.IntegralHeight")));
            this.m_comboBSWeight.ItemHeight = ((int)(resources.GetObject("m_comboBSWeight.ItemHeight")));
            this.m_comboBSWeight.Location = ((System.Drawing.Point)(resources.GetObject("m_comboBSWeight.Location")));
            this.m_comboBSWeight.MaxDropDownItems = ((int)(resources.GetObject("m_comboBSWeight.MaxDropDownItems")));
            this.m_comboBSWeight.MaxLength = ((int)(resources.GetObject("m_comboBSWeight.MaxLength")));
            this.m_comboBSWeight.Name = "m_comboBSWeight";
            this.m_comboBSWeight.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("m_comboBSWeight.RightToLeft")));
            this.m_comboBSWeight.Size = ((System.Drawing.Size)(resources.GetObject("m_comboBSWeight.Size")));
            this.m_comboBSWeight.TabIndex = ((int)(resources.GetObject("m_comboBSWeight.TabIndex")));
            this.m_comboBSWeight.Text = resources.GetString("m_comboBSWeight.Text");
            this.m_comboBSWeight.Visible = ((bool)(resources.GetObject("m_comboBSWeight.Visible")));
            this.m_comboBSWeight.SelectedIndexChanged += new System.EventHandler(this.BSWeight_SelectedIndexChanged);
            this.m_comboBSWeight.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.BSWeight_DrawItem);
            // 
            // m_lblBorderDashStyle
            // 
            this.m_lblBorderDashStyle.AccessibleDescription = resources.GetString("m_lblBorderDashStyle.AccessibleDescription");
            this.m_lblBorderDashStyle.AccessibleName = resources.GetString("m_lblBorderDashStyle.AccessibleName");
            this.m_lblBorderDashStyle.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("m_lblBorderDashStyle.Anchor")));
            this.m_lblBorderDashStyle.AutoSize = ((bool)(resources.GetObject("m_lblBorderDashStyle.AutoSize")));
            this.m_lblBorderDashStyle.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("m_lblBorderDashStyle.Dock")));
            this.m_lblBorderDashStyle.Enabled = ((bool)(resources.GetObject("m_lblBorderDashStyle.Enabled")));
            this.m_lblBorderDashStyle.Font = ((System.Drawing.Font)(resources.GetObject("m_lblBorderDashStyle.Font")));
            this.m_lblBorderDashStyle.Image = ((System.Drawing.Image)(resources.GetObject("m_lblBorderDashStyle.Image")));
            this.m_lblBorderDashStyle.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("m_lblBorderDashStyle.ImageAlign")));
            this.m_lblBorderDashStyle.ImageIndex = ((int)(resources.GetObject("m_lblBorderDashStyle.ImageIndex")));
            this.m_lblBorderDashStyle.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("m_lblBorderDashStyle.ImeMode")));
            this.m_lblBorderDashStyle.Location = ((System.Drawing.Point)(resources.GetObject("m_lblBorderDashStyle.Location")));
            this.m_lblBorderDashStyle.Name = "m_lblBorderDashStyle";
            this.m_lblBorderDashStyle.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("m_lblBorderDashStyle.RightToLeft")));
            this.m_lblBorderDashStyle.Size = ((System.Drawing.Size)(resources.GetObject("m_lblBorderDashStyle.Size")));
            this.m_lblBorderDashStyle.TabIndex = ((int)(resources.GetObject("m_lblBorderDashStyle.TabIndex")));
            this.m_lblBorderDashStyle.Text = resources.GetString("m_lblBorderDashStyle.Text");
            this.m_lblBorderDashStyle.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("m_lblBorderDashStyle.TextAlign")));
            this.m_lblBorderDashStyle.Visible = ((bool)(resources.GetObject("m_lblBorderDashStyle.Visible")));
            // 
            // m_lblBorderColor
            // 
            this.m_lblBorderColor.AccessibleDescription = resources.GetString("m_lblBorderColor.AccessibleDescription");
            this.m_lblBorderColor.AccessibleName = resources.GetString("m_lblBorderColor.AccessibleName");
            this.m_lblBorderColor.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("m_lblBorderColor.Anchor")));
            this.m_lblBorderColor.AutoSize = ((bool)(resources.GetObject("m_lblBorderColor.AutoSize")));
            this.m_lblBorderColor.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("m_lblBorderColor.Dock")));
            this.m_lblBorderColor.Enabled = ((bool)(resources.GetObject("m_lblBorderColor.Enabled")));
            this.m_lblBorderColor.Font = ((System.Drawing.Font)(resources.GetObject("m_lblBorderColor.Font")));
            this.m_lblBorderColor.Image = ((System.Drawing.Image)(resources.GetObject("m_lblBorderColor.Image")));
            this.m_lblBorderColor.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("m_lblBorderColor.ImageAlign")));
            this.m_lblBorderColor.ImageIndex = ((int)(resources.GetObject("m_lblBorderColor.ImageIndex")));
            this.m_lblBorderColor.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("m_lblBorderColor.ImeMode")));
            this.m_lblBorderColor.Location = ((System.Drawing.Point)(resources.GetObject("m_lblBorderColor.Location")));
            this.m_lblBorderColor.Name = "m_lblBorderColor";
            this.m_lblBorderColor.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("m_lblBorderColor.RightToLeft")));
            this.m_lblBorderColor.Size = ((System.Drawing.Size)(resources.GetObject("m_lblBorderColor.Size")));
            this.m_lblBorderColor.TabIndex = ((int)(resources.GetObject("m_lblBorderColor.TabIndex")));
            this.m_lblBorderColor.Text = resources.GetString("m_lblBorderColor.Text");
            this.m_lblBorderColor.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("m_lblBorderColor.TextAlign")));
            this.m_lblBorderColor.Visible = ((bool)(resources.GetObject("m_lblBorderColor.Visible")));
            // 
            // m_lblBorderDashWeight
            // 
            this.m_lblBorderDashWeight.AccessibleDescription = resources.GetString("m_lblBorderDashWeight.AccessibleDescription");
            this.m_lblBorderDashWeight.AccessibleName = resources.GetString("m_lblBorderDashWeight.AccessibleName");
            this.m_lblBorderDashWeight.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("m_lblBorderDashWeight.Anchor")));
            this.m_lblBorderDashWeight.AutoSize = ((bool)(resources.GetObject("m_lblBorderDashWeight.AutoSize")));
            this.m_lblBorderDashWeight.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("m_lblBorderDashWeight.Dock")));
            this.m_lblBorderDashWeight.Enabled = ((bool)(resources.GetObject("m_lblBorderDashWeight.Enabled")));
            this.m_lblBorderDashWeight.Font = ((System.Drawing.Font)(resources.GetObject("m_lblBorderDashWeight.Font")));
            this.m_lblBorderDashWeight.Image = ((System.Drawing.Image)(resources.GetObject("m_lblBorderDashWeight.Image")));
            this.m_lblBorderDashWeight.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("m_lblBorderDashWeight.ImageAlign")));
            this.m_lblBorderDashWeight.ImageIndex = ((int)(resources.GetObject("m_lblBorderDashWeight.ImageIndex")));
            this.m_lblBorderDashWeight.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("m_lblBorderDashWeight.ImeMode")));
            this.m_lblBorderDashWeight.Location = ((System.Drawing.Point)(resources.GetObject("m_lblBorderDashWeight.Location")));
            this.m_lblBorderDashWeight.Name = "m_lblBorderDashWeight";
            this.m_lblBorderDashWeight.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("m_lblBorderDashWeight.RightToLeft")));
            this.m_lblBorderDashWeight.Size = ((System.Drawing.Size)(resources.GetObject("m_lblBorderDashWeight.Size")));
            this.m_lblBorderDashWeight.TabIndex = ((int)(resources.GetObject("m_lblBorderDashWeight.TabIndex")));
            this.m_lblBorderDashWeight.Text = resources.GetString("m_lblBorderDashWeight.Text");
            this.m_lblBorderDashWeight.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("m_lblBorderDashWeight.TextAlign")));
            this.m_lblBorderDashWeight.Visible = ((bool)(resources.GetObject("m_lblBorderDashWeight.Visible")));
            // 
            // m_lblBorderTransparency
            // 
            this.m_lblBorderTransparency.AccessibleDescription = resources.GetString("m_lblBorderTransparency.AccessibleDescription");
            this.m_lblBorderTransparency.AccessibleName = resources.GetString("m_lblBorderTransparency.AccessibleName");
            this.m_lblBorderTransparency.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("m_lblBorderTransparency.Anchor")));
            this.m_lblBorderTransparency.AutoSize = ((bool)(resources.GetObject("m_lblBorderTransparency.AutoSize")));
            this.m_lblBorderTransparency.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("m_lblBorderTransparency.Dock")));
            this.m_lblBorderTransparency.Enabled = ((bool)(resources.GetObject("m_lblBorderTransparency.Enabled")));
            this.m_lblBorderTransparency.Font = ((System.Drawing.Font)(resources.GetObject("m_lblBorderTransparency.Font")));
            this.m_lblBorderTransparency.Image = ((System.Drawing.Image)(resources.GetObject("m_lblBorderTransparency.Image")));
            this.m_lblBorderTransparency.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("m_lblBorderTransparency.ImageAlign")));
            this.m_lblBorderTransparency.ImageIndex = ((int)(resources.GetObject("m_lblBorderTransparency.ImageIndex")));
            this.m_lblBorderTransparency.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("m_lblBorderTransparency.ImeMode")));
            this.m_lblBorderTransparency.Location = ((System.Drawing.Point)(resources.GetObject("m_lblBorderTransparency.Location")));
            this.m_lblBorderTransparency.Name = "m_lblBorderTransparency";
            this.m_lblBorderTransparency.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("m_lblBorderTransparency.RightToLeft")));
            this.m_lblBorderTransparency.Size = ((System.Drawing.Size)(resources.GetObject("m_lblBorderTransparency.Size")));
            this.m_lblBorderTransparency.TabIndex = ((int)(resources.GetObject("m_lblBorderTransparency.TabIndex")));
            this.m_lblBorderTransparency.Text = resources.GetString("m_lblBorderTransparency.Text");
            this.m_lblBorderTransparency.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("m_lblBorderTransparency.TextAlign")));
            this.m_lblBorderTransparency.Visible = ((bool)(resources.GetObject("m_lblBorderTransparency.Visible")));
            // 
            // m_trackBSTransparency
            // 
            this.m_trackBSTransparency.AccessibleDescription = resources.GetString("m_trackBSTransparency.AccessibleDescription");
            this.m_trackBSTransparency.AccessibleName = resources.GetString("m_trackBSTransparency.AccessibleName");
            this.m_trackBSTransparency.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("m_trackBSTransparency.Anchor")));
            this.m_trackBSTransparency.AutoSize = false;
            this.m_trackBSTransparency.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("m_trackBSTransparency.BackgroundImage")));
            this.m_trackBSTransparency.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("m_trackBSTransparency.Dock")));
            this.m_trackBSTransparency.Enabled = ((bool)(resources.GetObject("m_trackBSTransparency.Enabled")));
            this.m_trackBSTransparency.Font = ((System.Drawing.Font)(resources.GetObject("m_trackBSTransparency.Font")));
            this.m_trackBSTransparency.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("m_trackBSTransparency.ImeMode")));
            this.m_trackBSTransparency.Location = ((System.Drawing.Point)(resources.GetObject("m_trackBSTransparency.Location")));
            this.m_trackBSTransparency.Maximum = 255;
            this.m_trackBSTransparency.Name = "m_trackBSTransparency";
            this.m_trackBSTransparency.Orientation = ((System.Windows.Forms.Orientation)(resources.GetObject("m_trackBSTransparency.Orientation")));
            this.m_trackBSTransparency.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("m_trackBSTransparency.RightToLeft")));
            this.m_trackBSTransparency.Size = ((System.Drawing.Size)(resources.GetObject("m_trackBSTransparency.Size")));
            this.m_trackBSTransparency.TabIndex = ((int)(resources.GetObject("m_trackBSTransparency.TabIndex")));
            this.m_trackBSTransparency.Text = resources.GetString("m_trackBSTransparency.Text");
            this.m_trackBSTransparency.TickFrequency = 10;
            this.m_trackBSTransparency.Visible = ((bool)(resources.GetObject("m_trackBSTransparency.Visible")));
            this.m_trackBSTransparency.Scroll += new System.EventHandler(this.BSTransparency_Scroll);
            // 
            // m_pictureBorderPreview
            // 
            this.m_pictureBorderPreview.AccessibleDescription = resources.GetString("m_pictureBorderPreview.AccessibleDescription");
            this.m_pictureBorderPreview.AccessibleName = resources.GetString("m_pictureBorderPreview.AccessibleName");
            this.m_pictureBorderPreview.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("m_pictureBorderPreview.Anchor")));
            this.m_pictureBorderPreview.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.m_pictureBorderPreview.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("m_pictureBorderPreview.BackgroundImage")));
            this.m_pictureBorderPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_pictureBorderPreview.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("m_pictureBorderPreview.Dock")));
            this.m_pictureBorderPreview.Enabled = ((bool)(resources.GetObject("m_pictureBorderPreview.Enabled")));
            this.m_pictureBorderPreview.Font = ((System.Drawing.Font)(resources.GetObject("m_pictureBorderPreview.Font")));
            this.m_pictureBorderPreview.Image = ((System.Drawing.Image)(resources.GetObject("m_pictureBorderPreview.Image")));
            this.m_pictureBorderPreview.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("m_pictureBorderPreview.ImeMode")));
            this.m_pictureBorderPreview.Location = ((System.Drawing.Point)(resources.GetObject("m_pictureBorderPreview.Location")));
            this.m_pictureBorderPreview.Name = "m_pictureBorderPreview";
            this.m_pictureBorderPreview.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("m_pictureBorderPreview.RightToLeft")));
            this.m_pictureBorderPreview.Size = ((System.Drawing.Size)(resources.GetObject("m_pictureBorderPreview.Size")));
            this.m_pictureBorderPreview.SizeMode = ((System.Windows.Forms.PictureBoxSizeMode)(resources.GetObject("m_pictureBorderPreview.SizeMode")));
            this.m_pictureBorderPreview.TabIndex = ((int)(resources.GetObject("m_pictureBorderPreview.TabIndex")));
            this.m_pictureBorderPreview.TabStop = false;
            this.m_pictureBorderPreview.Text = resources.GetString("m_pictureBorderPreview.Text");
            this.m_pictureBorderPreview.Visible = ((bool)(resources.GetObject("m_pictureBorderPreview.Visible")));
            // 
            // m_btnBSChooseColor
            // 
            this.m_btnBSChooseColor.AccessibleDescription = resources.GetString("m_btnBSChooseColor.AccessibleDescription");
            this.m_btnBSChooseColor.AccessibleName = resources.GetString("m_btnBSChooseColor.AccessibleName");
            this.m_btnBSChooseColor.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("m_btnBSChooseColor.Anchor")));
            this.m_btnBSChooseColor.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("m_btnBSChooseColor.BackgroundImage")));
            this.m_btnBSChooseColor.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("m_btnBSChooseColor.Dock")));
            this.m_btnBSChooseColor.Enabled = ((bool)(resources.GetObject("m_btnBSChooseColor.Enabled")));
            this.m_btnBSChooseColor.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("m_btnBSChooseColor.FlatStyle")));
            this.m_btnBSChooseColor.Font = ((System.Drawing.Font)(resources.GetObject("m_btnBSChooseColor.Font")));
            this.m_btnBSChooseColor.Image = ((System.Drawing.Image)(resources.GetObject("m_btnBSChooseColor.Image")));
            this.m_btnBSChooseColor.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("m_btnBSChooseColor.ImageAlign")));
            this.m_btnBSChooseColor.ImageIndex = ((int)(resources.GetObject("m_btnBSChooseColor.ImageIndex")));
            this.m_btnBSChooseColor.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("m_btnBSChooseColor.ImeMode")));
            this.m_btnBSChooseColor.Location = ((System.Drawing.Point)(resources.GetObject("m_btnBSChooseColor.Location")));
            this.m_btnBSChooseColor.Name = "m_btnBSChooseColor";
            this.m_btnBSChooseColor.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("m_btnBSChooseColor.RightToLeft")));
            this.m_btnBSChooseColor.Size = ((System.Drawing.Size)(resources.GetObject("m_btnBSChooseColor.Size")));
            this.m_btnBSChooseColor.TabIndex = ((int)(resources.GetObject("m_btnBSChooseColor.TabIndex")));
            this.m_btnBSChooseColor.Text = resources.GetString("m_btnBSChooseColor.Text");
            this.m_btnBSChooseColor.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("m_btnBSChooseColor.TextAlign")));
            this.m_btnBSChooseColor.Visible = ((bool)(resources.GetObject("m_btnBSChooseColor.Visible")));
            this.m_btnBSChooseColor.Click += new System.EventHandler(this.BSChooseColor_Click);
            // 
            // label1
            // 
            this.label1.AccessibleDescription = resources.GetString("label1.AccessibleDescription");
            this.label1.AccessibleName = resources.GetString("label1.AccessibleName");
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label1.Anchor")));
            this.label1.AutoSize = ((bool)(resources.GetObject("label1.AutoSize")));
            this.label1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label1.Dock")));
            this.label1.Enabled = ((bool)(resources.GetObject("label1.Enabled")));
            this.label1.Font = ((System.Drawing.Font)(resources.GetObject("label1.Font")));
            this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.label1.Image = ((System.Drawing.Image)(resources.GetObject("label1.Image")));
            this.label1.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label1.ImageAlign")));
            this.label1.ImageIndex = ((int)(resources.GetObject("label1.ImageIndex")));
            this.label1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label1.ImeMode")));
            this.label1.Location = ((System.Drawing.Point)(resources.GetObject("label1.Location")));
            this.label1.Name = "label1";
            this.label1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label1.RightToLeft")));
            this.label1.Size = ((System.Drawing.Size)(resources.GetObject("label1.Size")));
            this.label1.TabIndex = ((int)(resources.GetObject("label1.TabIndex")));
            this.label1.Text = resources.GetString("label1.Text");
            this.label1.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label1.TextAlign")));
            this.label1.Visible = ((bool)(resources.GetObject("label1.Visible")));
            // 
            // label2
            // 
            this.label2.AccessibleDescription = resources.GetString("label2.AccessibleDescription");
            this.label2.AccessibleName = resources.GetString("label2.AccessibleName");
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label2.Anchor")));
            this.label2.AutoSize = ((bool)(resources.GetObject("label2.AutoSize")));
            this.label2.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label2.Dock")));
            this.label2.Enabled = ((bool)(resources.GetObject("label2.Enabled")));
            this.label2.Font = ((System.Drawing.Font)(resources.GetObject("label2.Font")));
            this.label2.ForeColor = System.Drawing.SystemColors.ActiveBorder;
            this.label2.Image = ((System.Drawing.Image)(resources.GetObject("label2.Image")));
            this.label2.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label2.ImageAlign")));
            this.label2.ImageIndex = ((int)(resources.GetObject("label2.ImageIndex")));
            this.label2.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label2.ImeMode")));
            this.label2.Location = ((System.Drawing.Point)(resources.GetObject("label2.Location")));
            this.label2.Name = "label2";
            this.label2.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label2.RightToLeft")));
            this.label2.Size = ((System.Drawing.Size)(resources.GetObject("label2.Size")));
            this.label2.TabIndex = ((int)(resources.GetObject("label2.TabIndex")));
            this.label2.Text = resources.GetString("label2.Text");
            this.label2.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label2.TextAlign")));
            this.label2.Visible = ((bool)(resources.GetObject("label2.Visible")));
            // 
            // label3
            // 
            this.label3.AccessibleDescription = resources.GetString("label3.AccessibleDescription");
            this.label3.AccessibleName = resources.GetString("label3.AccessibleName");
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label3.Anchor")));
            this.label3.AutoSize = ((bool)(resources.GetObject("label3.AutoSize")));
            this.label3.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label3.Dock")));
            this.label3.Enabled = ((bool)(resources.GetObject("label3.Enabled")));
            this.label3.Font = ((System.Drawing.Font)(resources.GetObject("label3.Font")));
            this.label3.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.label3.Image = ((System.Drawing.Image)(resources.GetObject("label3.Image")));
            this.label3.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label3.ImageAlign")));
            this.label3.ImageIndex = ((int)(resources.GetObject("label3.ImageIndex")));
            this.label3.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label3.ImeMode")));
            this.label3.Location = ((System.Drawing.Point)(resources.GetObject("label3.Location")));
            this.label3.Name = "label3";
            this.label3.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label3.RightToLeft")));
            this.label3.Size = ((System.Drawing.Size)(resources.GetObject("label3.Size")));
            this.label3.TabIndex = ((int)(resources.GetObject("label3.TabIndex")));
            this.label3.Text = resources.GetString("label3.Text");
            this.label3.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label3.TextAlign")));
            this.label3.Visible = ((bool)(resources.GetObject("label3.Visible")));
            // 
            // label4
            // 
            this.label4.AccessibleDescription = resources.GetString("label4.AccessibleDescription");
            this.label4.AccessibleName = resources.GetString("label4.AccessibleName");
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label4.Anchor")));
            this.label4.AutoSize = ((bool)(resources.GetObject("label4.AutoSize")));
            this.label4.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label4.Dock")));
            this.label4.Enabled = ((bool)(resources.GetObject("label4.Enabled")));
            this.label4.Font = ((System.Drawing.Font)(resources.GetObject("label4.Font")));
            this.label4.ForeColor = System.Drawing.SystemColors.ActiveBorder;
            this.label4.Image = ((System.Drawing.Image)(resources.GetObject("label4.Image")));
            this.label4.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label4.ImageAlign")));
            this.label4.ImageIndex = ((int)(resources.GetObject("label4.ImageIndex")));
            this.label4.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label4.ImeMode")));
            this.label4.Location = ((System.Drawing.Point)(resources.GetObject("label4.Location")));
            this.label4.Name = "label4";
            this.label4.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label4.RightToLeft")));
            this.label4.Size = ((System.Drawing.Size)(resources.GetObject("label4.Size")));
            this.label4.TabIndex = ((int)(resources.GetObject("label4.TabIndex")));
            this.label4.Text = resources.GetString("label4.Text");
            this.label4.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label4.TextAlign")));
            this.label4.Visible = ((bool)(resources.GetObject("label4.Visible")));
            // 
            // label5
            // 
            this.label5.AccessibleDescription = resources.GetString("label5.AccessibleDescription");
            this.label5.AccessibleName = resources.GetString("label5.AccessibleName");
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label5.Anchor")));
            this.label5.AutoSize = ((bool)(resources.GetObject("label5.AutoSize")));
            this.label5.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label5.Dock")));
            this.label5.Enabled = ((bool)(resources.GetObject("label5.Enabled")));
            this.label5.Font = ((System.Drawing.Font)(resources.GetObject("label5.Font")));
            this.label5.ForeColor = System.Drawing.SystemColors.ActiveBorder;
            this.label5.Image = ((System.Drawing.Image)(resources.GetObject("label5.Image")));
            this.label5.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label5.ImageAlign")));
            this.label5.ImageIndex = ((int)(resources.GetObject("label5.ImageIndex")));
            this.label5.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label5.ImeMode")));
            this.label5.Location = ((System.Drawing.Point)(resources.GetObject("label5.Location")));
            this.label5.Name = "label5";
            this.label5.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label5.RightToLeft")));
            this.label5.Size = ((System.Drawing.Size)(resources.GetObject("label5.Size")));
            this.label5.TabIndex = ((int)(resources.GetObject("label5.TabIndex")));
            this.label5.Text = resources.GetString("label5.Text");
            this.label5.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label5.TextAlign")));
            this.label5.Visible = ((bool)(resources.GetObject("label5.Visible")));
            // 
            // label6
            // 
            this.label6.AccessibleDescription = resources.GetString("label6.AccessibleDescription");
            this.label6.AccessibleName = resources.GetString("label6.AccessibleName");
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label6.Anchor")));
            this.label6.AutoSize = ((bool)(resources.GetObject("label6.AutoSize")));
            this.label6.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label6.Dock")));
            this.label6.Enabled = ((bool)(resources.GetObject("label6.Enabled")));
            this.label6.Font = ((System.Drawing.Font)(resources.GetObject("label6.Font")));
            this.label6.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.label6.Image = ((System.Drawing.Image)(resources.GetObject("label6.Image")));
            this.label6.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label6.ImageAlign")));
            this.label6.ImageIndex = ((int)(resources.GetObject("label6.ImageIndex")));
            this.label6.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label6.ImeMode")));
            this.label6.Location = ((System.Drawing.Point)(resources.GetObject("label6.Location")));
            this.label6.Name = "label6";
            this.label6.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label6.RightToLeft")));
            this.label6.Size = ((System.Drawing.Size)(resources.GetObject("label6.Size")));
            this.label6.TabIndex = ((int)(resources.GetObject("label6.TabIndex")));
            this.label6.Text = resources.GetString("label6.Text");
            this.label6.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label6.TextAlign")));
            this.label6.Visible = ((bool)(resources.GetObject("label6.Visible")));
            // 
            // m_checkBSRoundingNone
            // 
            this.m_checkBSRoundingNone.AccessibleDescription = resources.GetString("m_checkBSRoundingNone.AccessibleDescription");
            this.m_checkBSRoundingNone.AccessibleName = resources.GetString("m_checkBSRoundingNone.AccessibleName");
            this.m_checkBSRoundingNone.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("m_checkBSRoundingNone.Anchor")));
            this.m_checkBSRoundingNone.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("m_checkBSRoundingNone.Appearance")));
            this.m_checkBSRoundingNone.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("m_checkBSRoundingNone.BackgroundImage")));
            this.m_checkBSRoundingNone.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("m_checkBSRoundingNone.CheckAlign")));
            this.m_checkBSRoundingNone.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("m_checkBSRoundingNone.Dock")));
            this.m_checkBSRoundingNone.Enabled = ((bool)(resources.GetObject("m_checkBSRoundingNone.Enabled")));
            this.m_checkBSRoundingNone.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("m_checkBSRoundingNone.FlatStyle")));
            this.m_checkBSRoundingNone.Font = ((System.Drawing.Font)(resources.GetObject("m_checkBSRoundingNone.Font")));
            this.m_checkBSRoundingNone.Image = ((System.Drawing.Image)(resources.GetObject("m_checkBSRoundingNone.Image")));
            this.m_checkBSRoundingNone.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("m_checkBSRoundingNone.ImageAlign")));
            this.m_checkBSRoundingNone.ImageIndex = ((int)(resources.GetObject("m_checkBSRoundingNone.ImageIndex")));
            this.m_checkBSRoundingNone.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("m_checkBSRoundingNone.ImeMode")));
            this.m_checkBSRoundingNone.Location = ((System.Drawing.Point)(resources.GetObject("m_checkBSRoundingNone.Location")));
            this.m_checkBSRoundingNone.Name = "m_checkBSRoundingNone";
            this.m_checkBSRoundingNone.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("m_checkBSRoundingNone.RightToLeft")));
            this.m_checkBSRoundingNone.Rounding = Syncfusion.Windows.Forms.Diagram.BorderStyleCornerRounding.None;
            this.m_checkBSRoundingNone.Size = ((System.Drawing.Size)(resources.GetObject("m_checkBSRoundingNone.Size")));
            this.m_checkBSRoundingNone.TabIndex = ((int)(resources.GetObject("m_checkBSRoundingNone.TabIndex")));
            this.m_checkBSRoundingNone.Text = resources.GetString("m_checkBSRoundingNone.Text");
            this.m_checkBSRoundingNone.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("m_checkBSRoundingNone.TextAlign")));
            this.m_checkBSRoundingNone.Visible = ((bool)(resources.GetObject("m_checkBSRoundingNone.Visible")));
            this.m_checkBSRoundingNone.CheckedChanged += new System.EventHandler(this.BSRoundingNone_CheckedChanged);
            // 
            // m_checkBSRoundingSmallest
            // 
            this.m_checkBSRoundingSmallest.AccessibleDescription = resources.GetString("m_checkBSRoundingSmallest.AccessibleDescription");
            this.m_checkBSRoundingSmallest.AccessibleName = resources.GetString("m_checkBSRoundingSmallest.AccessibleName");
            this.m_checkBSRoundingSmallest.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("m_checkBSRoundingSmallest.Anchor")));
            this.m_checkBSRoundingSmallest.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("m_checkBSRoundingSmallest.Appearance")));
            this.m_checkBSRoundingSmallest.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("m_checkBSRoundingSmallest.BackgroundImage")));
            this.m_checkBSRoundingSmallest.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("m_checkBSRoundingSmallest.CheckAlign")));
            this.m_checkBSRoundingSmallest.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("m_checkBSRoundingSmallest.Dock")));
            this.m_checkBSRoundingSmallest.Enabled = ((bool)(resources.GetObject("m_checkBSRoundingSmallest.Enabled")));
            this.m_checkBSRoundingSmallest.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("m_checkBSRoundingSmallest.FlatStyle")));
            this.m_checkBSRoundingSmallest.Font = ((System.Drawing.Font)(resources.GetObject("m_checkBSRoundingSmallest.Font")));
            this.m_checkBSRoundingSmallest.Image = ((System.Drawing.Image)(resources.GetObject("m_checkBSRoundingSmallest.Image")));
            this.m_checkBSRoundingSmallest.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("m_checkBSRoundingSmallest.ImageAlign")));
            this.m_checkBSRoundingSmallest.ImageIndex = ((int)(resources.GetObject("m_checkBSRoundingSmallest.ImageIndex")));
            this.m_checkBSRoundingSmallest.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("m_checkBSRoundingSmallest.ImeMode")));
            this.m_checkBSRoundingSmallest.Location = ((System.Drawing.Point)(resources.GetObject("m_checkBSRoundingSmallest.Location")));
            this.m_checkBSRoundingSmallest.Name = "m_checkBSRoundingSmallest";
            this.m_checkBSRoundingSmallest.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("m_checkBSRoundingSmallest.RightToLeft")));
            this.m_checkBSRoundingSmallest.Rounding = Syncfusion.Windows.Forms.Diagram.BorderStyleCornerRounding.Smallest;
            this.m_checkBSRoundingSmallest.Size = ((System.Drawing.Size)(resources.GetObject("m_checkBSRoundingSmallest.Size")));
            this.m_checkBSRoundingSmallest.TabIndex = ((int)(resources.GetObject("m_checkBSRoundingSmallest.TabIndex")));
            this.m_checkBSRoundingSmallest.Text = resources.GetString("m_checkBSRoundingSmallest.Text");
            this.m_checkBSRoundingSmallest.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("m_checkBSRoundingSmallest.TextAlign")));
            this.m_checkBSRoundingSmallest.Visible = ((bool)(resources.GetObject("m_checkBSRoundingSmallest.Visible")));
            this.m_checkBSRoundingSmallest.CheckedChanged += new System.EventHandler(this.BSRoundingSmallest_CheckedChanged);
            // 
            // m_checkBSRoundingVerySmall
            // 
            this.m_checkBSRoundingVerySmall.AccessibleDescription = resources.GetString("m_checkBSRoundingVerySmall.AccessibleDescription");
            this.m_checkBSRoundingVerySmall.AccessibleName = resources.GetString("m_checkBSRoundingVerySmall.AccessibleName");
            this.m_checkBSRoundingVerySmall.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("m_checkBSRoundingVerySmall.Anchor")));
            this.m_checkBSRoundingVerySmall.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("m_checkBSRoundingVerySmall.Appearance")));
            this.m_checkBSRoundingVerySmall.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("m_checkBSRoundingVerySmall.BackgroundImage")));
            this.m_checkBSRoundingVerySmall.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("m_checkBSRoundingVerySmall.CheckAlign")));
            this.m_checkBSRoundingVerySmall.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("m_checkBSRoundingVerySmall.Dock")));
            this.m_checkBSRoundingVerySmall.Enabled = ((bool)(resources.GetObject("m_checkBSRoundingVerySmall.Enabled")));
            this.m_checkBSRoundingVerySmall.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("m_checkBSRoundingVerySmall.FlatStyle")));
            this.m_checkBSRoundingVerySmall.Font = ((System.Drawing.Font)(resources.GetObject("m_checkBSRoundingVerySmall.Font")));
            this.m_checkBSRoundingVerySmall.Image = ((System.Drawing.Image)(resources.GetObject("m_checkBSRoundingVerySmall.Image")));
            this.m_checkBSRoundingVerySmall.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("m_checkBSRoundingVerySmall.ImageAlign")));
            this.m_checkBSRoundingVerySmall.ImageIndex = ((int)(resources.GetObject("m_checkBSRoundingVerySmall.ImageIndex")));
            this.m_checkBSRoundingVerySmall.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("m_checkBSRoundingVerySmall.ImeMode")));
            this.m_checkBSRoundingVerySmall.Location = ((System.Drawing.Point)(resources.GetObject("m_checkBSRoundingVerySmall.Location")));
            this.m_checkBSRoundingVerySmall.Name = "m_checkBSRoundingVerySmall";
            this.m_checkBSRoundingVerySmall.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("m_checkBSRoundingVerySmall.RightToLeft")));
            this.m_checkBSRoundingVerySmall.Rounding = Syncfusion.Windows.Forms.Diagram.BorderStyleCornerRounding.Smaller;
            this.m_checkBSRoundingVerySmall.Size = ((System.Drawing.Size)(resources.GetObject("m_checkBSRoundingVerySmall.Size")));
            this.m_checkBSRoundingVerySmall.TabIndex = ((int)(resources.GetObject("m_checkBSRoundingVerySmall.TabIndex")));
            this.m_checkBSRoundingVerySmall.Text = resources.GetString("m_checkBSRoundingVerySmall.Text");
            this.m_checkBSRoundingVerySmall.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("m_checkBSRoundingVerySmall.TextAlign")));
            this.m_checkBSRoundingVerySmall.Visible = ((bool)(resources.GetObject("m_checkBSRoundingVerySmall.Visible")));
            this.m_checkBSRoundingVerySmall.CheckedChanged += new System.EventHandler(this.BSRoundingVerySmall_CheckedChanged);
            // 
            // m_checkBSRoundingSmall
            // 
            this.m_checkBSRoundingSmall.AccessibleDescription = resources.GetString("m_checkBSRoundingSmall.AccessibleDescription");
            this.m_checkBSRoundingSmall.AccessibleName = resources.GetString("m_checkBSRoundingSmall.AccessibleName");
            this.m_checkBSRoundingSmall.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("m_checkBSRoundingSmall.Anchor")));
            this.m_checkBSRoundingSmall.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("m_checkBSRoundingSmall.Appearance")));
            this.m_checkBSRoundingSmall.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("m_checkBSRoundingSmall.BackgroundImage")));
            this.m_checkBSRoundingSmall.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("m_checkBSRoundingSmall.CheckAlign")));
            this.m_checkBSRoundingSmall.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("m_checkBSRoundingSmall.Dock")));
            this.m_checkBSRoundingSmall.Enabled = ((bool)(resources.GetObject("m_checkBSRoundingSmall.Enabled")));
            this.m_checkBSRoundingSmall.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("m_checkBSRoundingSmall.FlatStyle")));
            this.m_checkBSRoundingSmall.Font = ((System.Drawing.Font)(resources.GetObject("m_checkBSRoundingSmall.Font")));
            this.m_checkBSRoundingSmall.Image = ((System.Drawing.Image)(resources.GetObject("m_checkBSRoundingSmall.Image")));
            this.m_checkBSRoundingSmall.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("m_checkBSRoundingSmall.ImageAlign")));
            this.m_checkBSRoundingSmall.ImageIndex = ((int)(resources.GetObject("m_checkBSRoundingSmall.ImageIndex")));
            this.m_checkBSRoundingSmall.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("m_checkBSRoundingSmall.ImeMode")));
            this.m_checkBSRoundingSmall.Location = ((System.Drawing.Point)(resources.GetObject("m_checkBSRoundingSmall.Location")));
            this.m_checkBSRoundingSmall.Name = "m_checkBSRoundingSmall";
            this.m_checkBSRoundingSmall.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("m_checkBSRoundingSmall.RightToLeft")));
            this.m_checkBSRoundingSmall.Rounding = Syncfusion.Windows.Forms.Diagram.BorderStyleCornerRounding.Small;
            this.m_checkBSRoundingSmall.Size = ((System.Drawing.Size)(resources.GetObject("m_checkBSRoundingSmall.Size")));
            this.m_checkBSRoundingSmall.TabIndex = ((int)(resources.GetObject("m_checkBSRoundingSmall.TabIndex")));
            this.m_checkBSRoundingSmall.Text = resources.GetString("m_checkBSRoundingSmall.Text");
            this.m_checkBSRoundingSmall.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("m_checkBSRoundingSmall.TextAlign")));
            this.m_checkBSRoundingSmall.Visible = ((bool)(resources.GetObject("m_checkBSRoundingSmall.Visible")));
            this.m_checkBSRoundingSmall.CheckedChanged += new System.EventHandler(this.BSRoundingSmall_CheckedChanged);
            // 
            // m_checkBSRoundingMedium
            // 
            this.m_checkBSRoundingMedium.AccessibleDescription = resources.GetString("m_checkBSRoundingMedium.AccessibleDescription");
            this.m_checkBSRoundingMedium.AccessibleName = resources.GetString("m_checkBSRoundingMedium.AccessibleName");
            this.m_checkBSRoundingMedium.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("m_checkBSRoundingMedium.Anchor")));
            this.m_checkBSRoundingMedium.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("m_checkBSRoundingMedium.Appearance")));
            this.m_checkBSRoundingMedium.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("m_checkBSRoundingMedium.BackgroundImage")));
            this.m_checkBSRoundingMedium.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("m_checkBSRoundingMedium.CheckAlign")));
            this.m_checkBSRoundingMedium.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("m_checkBSRoundingMedium.Dock")));
            this.m_checkBSRoundingMedium.Enabled = ((bool)(resources.GetObject("m_checkBSRoundingMedium.Enabled")));
            this.m_checkBSRoundingMedium.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("m_checkBSRoundingMedium.FlatStyle")));
            this.m_checkBSRoundingMedium.Font = ((System.Drawing.Font)(resources.GetObject("m_checkBSRoundingMedium.Font")));
            this.m_checkBSRoundingMedium.Image = ((System.Drawing.Image)(resources.GetObject("m_checkBSRoundingMedium.Image")));
            this.m_checkBSRoundingMedium.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("m_checkBSRoundingMedium.ImageAlign")));
            this.m_checkBSRoundingMedium.ImageIndex = ((int)(resources.GetObject("m_checkBSRoundingMedium.ImageIndex")));
            this.m_checkBSRoundingMedium.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("m_checkBSRoundingMedium.ImeMode")));
            this.m_checkBSRoundingMedium.Location = ((System.Drawing.Point)(resources.GetObject("m_checkBSRoundingMedium.Location")));
            this.m_checkBSRoundingMedium.Name = "m_checkBSRoundingMedium";
            this.m_checkBSRoundingMedium.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("m_checkBSRoundingMedium.RightToLeft")));
            this.m_checkBSRoundingMedium.Rounding = Syncfusion.Windows.Forms.Diagram.BorderStyleCornerRounding.Medium;
            this.m_checkBSRoundingMedium.Size = ((System.Drawing.Size)(resources.GetObject("m_checkBSRoundingMedium.Size")));
            this.m_checkBSRoundingMedium.TabIndex = ((int)(resources.GetObject("m_checkBSRoundingMedium.TabIndex")));
            this.m_checkBSRoundingMedium.Text = resources.GetString("m_checkBSRoundingMedium.Text");
            this.m_checkBSRoundingMedium.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("m_checkBSRoundingMedium.TextAlign")));
            this.m_checkBSRoundingMedium.Visible = ((bool)(resources.GetObject("m_checkBSRoundingMedium.Visible")));
            this.m_checkBSRoundingMedium.CheckedChanged += new System.EventHandler(this.BSRoundingMedium_CheckedChanged);
            // 
            // m_checkBSRoundingBiggest
            // 
            this.m_checkBSRoundingBiggest.AccessibleDescription = resources.GetString("m_checkBSRoundingBiggest.AccessibleDescription");
            this.m_checkBSRoundingBiggest.AccessibleName = resources.GetString("m_checkBSRoundingBiggest.AccessibleName");
            this.m_checkBSRoundingBiggest.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("m_checkBSRoundingBiggest.Anchor")));
            this.m_checkBSRoundingBiggest.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("m_checkBSRoundingBiggest.Appearance")));
            this.m_checkBSRoundingBiggest.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("m_checkBSRoundingBiggest.BackgroundImage")));
            this.m_checkBSRoundingBiggest.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("m_checkBSRoundingBiggest.CheckAlign")));
            this.m_checkBSRoundingBiggest.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("m_checkBSRoundingBiggest.Dock")));
            this.m_checkBSRoundingBiggest.Enabled = ((bool)(resources.GetObject("m_checkBSRoundingBiggest.Enabled")));
            this.m_checkBSRoundingBiggest.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("m_checkBSRoundingBiggest.FlatStyle")));
            this.m_checkBSRoundingBiggest.Font = ((System.Drawing.Font)(resources.GetObject("m_checkBSRoundingBiggest.Font")));
            this.m_checkBSRoundingBiggest.Image = ((System.Drawing.Image)(resources.GetObject("m_checkBSRoundingBiggest.Image")));
            this.m_checkBSRoundingBiggest.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("m_checkBSRoundingBiggest.ImageAlign")));
            this.m_checkBSRoundingBiggest.ImageIndex = ((int)(resources.GetObject("m_checkBSRoundingBiggest.ImageIndex")));
            this.m_checkBSRoundingBiggest.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("m_checkBSRoundingBiggest.ImeMode")));
            this.m_checkBSRoundingBiggest.Location = ((System.Drawing.Point)(resources.GetObject("m_checkBSRoundingBiggest.Location")));
            this.m_checkBSRoundingBiggest.Name = "m_checkBSRoundingBiggest";
            this.m_checkBSRoundingBiggest.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("m_checkBSRoundingBiggest.RightToLeft")));
            this.m_checkBSRoundingBiggest.Rounding = Syncfusion.Windows.Forms.Diagram.BorderStyleCornerRounding.Biggest;
            this.m_checkBSRoundingBiggest.Size = ((System.Drawing.Size)(resources.GetObject("m_checkBSRoundingBiggest.Size")));
            this.m_checkBSRoundingBiggest.TabIndex = ((int)(resources.GetObject("m_checkBSRoundingBiggest.TabIndex")));
            this.m_checkBSRoundingBiggest.Text = resources.GetString("m_checkBSRoundingBiggest.Text");
            this.m_checkBSRoundingBiggest.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("m_checkBSRoundingBiggest.TextAlign")));
            this.m_checkBSRoundingBiggest.Visible = ((bool)(resources.GetObject("m_checkBSRoundingBiggest.Visible")));
            this.m_checkBSRoundingBiggest.CheckedChanged += new System.EventHandler(this.BSRoundingBiggest_CheckedChanged);
            // 
            // m_checkBSRoundingVeryBig
            // 
            this.m_checkBSRoundingVeryBig.AccessibleDescription = resources.GetString("m_checkBSRoundingVeryBig.AccessibleDescription");
            this.m_checkBSRoundingVeryBig.AccessibleName = resources.GetString("m_checkBSRoundingVeryBig.AccessibleName");
            this.m_checkBSRoundingVeryBig.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("m_checkBSRoundingVeryBig.Anchor")));
            this.m_checkBSRoundingVeryBig.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("m_checkBSRoundingVeryBig.Appearance")));
            this.m_checkBSRoundingVeryBig.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("m_checkBSRoundingVeryBig.BackgroundImage")));
            this.m_checkBSRoundingVeryBig.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("m_checkBSRoundingVeryBig.CheckAlign")));
            this.m_checkBSRoundingVeryBig.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("m_checkBSRoundingVeryBig.Dock")));
            this.m_checkBSRoundingVeryBig.Enabled = ((bool)(resources.GetObject("m_checkBSRoundingVeryBig.Enabled")));
            this.m_checkBSRoundingVeryBig.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("m_checkBSRoundingVeryBig.FlatStyle")));
            this.m_checkBSRoundingVeryBig.Font = ((System.Drawing.Font)(resources.GetObject("m_checkBSRoundingVeryBig.Font")));
            this.m_checkBSRoundingVeryBig.Image = ((System.Drawing.Image)(resources.GetObject("m_checkBSRoundingVeryBig.Image")));
            this.m_checkBSRoundingVeryBig.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("m_checkBSRoundingVeryBig.ImageAlign")));
            this.m_checkBSRoundingVeryBig.ImageIndex = ((int)(resources.GetObject("m_checkBSRoundingVeryBig.ImageIndex")));
            this.m_checkBSRoundingVeryBig.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("m_checkBSRoundingVeryBig.ImeMode")));
            this.m_checkBSRoundingVeryBig.Location = ((System.Drawing.Point)(resources.GetObject("m_checkBSRoundingVeryBig.Location")));
            this.m_checkBSRoundingVeryBig.Name = "m_checkBSRoundingVeryBig";
            this.m_checkBSRoundingVeryBig.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("m_checkBSRoundingVeryBig.RightToLeft")));
            this.m_checkBSRoundingVeryBig.Rounding = Syncfusion.Windows.Forms.Diagram.BorderStyleCornerRounding.Bigger;
            this.m_checkBSRoundingVeryBig.Size = ((System.Drawing.Size)(resources.GetObject("m_checkBSRoundingVeryBig.Size")));
            this.m_checkBSRoundingVeryBig.TabIndex = ((int)(resources.GetObject("m_checkBSRoundingVeryBig.TabIndex")));
            this.m_checkBSRoundingVeryBig.Text = resources.GetString("m_checkBSRoundingVeryBig.Text");
            this.m_checkBSRoundingVeryBig.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("m_checkBSRoundingVeryBig.TextAlign")));
            this.m_checkBSRoundingVeryBig.Visible = ((bool)(resources.GetObject("m_checkBSRoundingVeryBig.Visible")));
            this.m_checkBSRoundingVeryBig.CheckedChanged += new System.EventHandler(this.BSRoundingVeryBig_CheckedChanged);
            // 
            // m_checkBSRoundingBig
            // 
            this.m_checkBSRoundingBig.AccessibleDescription = resources.GetString("m_checkBSRoundingBig.AccessibleDescription");
            this.m_checkBSRoundingBig.AccessibleName = resources.GetString("m_checkBSRoundingBig.AccessibleName");
            this.m_checkBSRoundingBig.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("m_checkBSRoundingBig.Anchor")));
            this.m_checkBSRoundingBig.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("m_checkBSRoundingBig.Appearance")));
            this.m_checkBSRoundingBig.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("m_checkBSRoundingBig.BackgroundImage")));
            this.m_checkBSRoundingBig.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("m_checkBSRoundingBig.CheckAlign")));
            this.m_checkBSRoundingBig.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("m_checkBSRoundingBig.Dock")));
            this.m_checkBSRoundingBig.Enabled = ((bool)(resources.GetObject("m_checkBSRoundingBig.Enabled")));
            this.m_checkBSRoundingBig.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("m_checkBSRoundingBig.FlatStyle")));
            this.m_checkBSRoundingBig.Font = ((System.Drawing.Font)(resources.GetObject("m_checkBSRoundingBig.Font")));
            this.m_checkBSRoundingBig.Image = ((System.Drawing.Image)(resources.GetObject("m_checkBSRoundingBig.Image")));
            this.m_checkBSRoundingBig.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("m_checkBSRoundingBig.ImageAlign")));
            this.m_checkBSRoundingBig.ImageIndex = ((int)(resources.GetObject("m_checkBSRoundingBig.ImageIndex")));
            this.m_checkBSRoundingBig.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("m_checkBSRoundingBig.ImeMode")));
            this.m_checkBSRoundingBig.Location = ((System.Drawing.Point)(resources.GetObject("m_checkBSRoundingBig.Location")));
            this.m_checkBSRoundingBig.Name = "m_checkBSRoundingBig";
            this.m_checkBSRoundingBig.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("m_checkBSRoundingBig.RightToLeft")));
            this.m_checkBSRoundingBig.Rounding = Syncfusion.Windows.Forms.Diagram.BorderStyleCornerRounding.Big;
            this.m_checkBSRoundingBig.Size = ((System.Drawing.Size)(resources.GetObject("m_checkBSRoundingBig.Size")));
            this.m_checkBSRoundingBig.TabIndex = ((int)(resources.GetObject("m_checkBSRoundingBig.TabIndex")));
            this.m_checkBSRoundingBig.Text = resources.GetString("m_checkBSRoundingBig.Text");
            this.m_checkBSRoundingBig.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("m_checkBSRoundingBig.TextAlign")));
            this.m_checkBSRoundingBig.Visible = ((bool)(resources.GetObject("m_checkBSRoundingBig.Visible")));
            this.m_checkBSRoundingBig.CheckedChanged += new System.EventHandler(this.BSRoundingBig_CheckedChanged);
            // 
            // m_checkBSShowPageBounds
            // 
            this.m_checkBSShowPageBounds.AccessibleDescription = resources.GetString("m_checkBSShowPageBounds.AccessibleDescription");
            this.m_checkBSShowPageBounds.AccessibleName = resources.GetString("m_checkBSShowPageBounds.AccessibleName");
            this.m_checkBSShowPageBounds.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("m_checkBSShowPageBounds.Anchor")));
            this.m_checkBSShowPageBounds.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("m_checkBSShowPageBounds.Appearance")));
            this.m_checkBSShowPageBounds.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("m_checkBSShowPageBounds.BackgroundImage")));
            this.m_checkBSShowPageBounds.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("m_checkBSShowPageBounds.CheckAlign")));
            this.m_checkBSShowPageBounds.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("m_checkBSShowPageBounds.Dock")));
            this.m_checkBSShowPageBounds.Enabled = ((bool)(resources.GetObject("m_checkBSShowPageBounds.Enabled")));
            this.m_checkBSShowPageBounds.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("m_checkBSShowPageBounds.FlatStyle")));
            this.m_checkBSShowPageBounds.Font = ((System.Drawing.Font)(resources.GetObject("m_checkBSShowPageBounds.Font")));
            this.m_checkBSShowPageBounds.Image = ((System.Drawing.Image)(resources.GetObject("m_checkBSShowPageBounds.Image")));
            this.m_checkBSShowPageBounds.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("m_checkBSShowPageBounds.ImageAlign")));
            this.m_checkBSShowPageBounds.ImageIndex = ((int)(resources.GetObject("m_checkBSShowPageBounds.ImageIndex")));
            this.m_checkBSShowPageBounds.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("m_checkBSShowPageBounds.ImeMode")));
            this.m_checkBSShowPageBounds.Location = ((System.Drawing.Point)(resources.GetObject("m_checkBSShowPageBounds.Location")));
            this.m_checkBSShowPageBounds.Name = "m_checkBSShowPageBounds";
            this.m_checkBSShowPageBounds.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("m_checkBSShowPageBounds.RightToLeft")));
            this.m_checkBSShowPageBounds.Size = ((System.Drawing.Size)(resources.GetObject("m_checkBSShowPageBounds.Size")));
            this.m_checkBSShowPageBounds.TabIndex = ((int)(resources.GetObject("m_checkBSShowPageBounds.TabIndex")));
            this.m_checkBSShowPageBounds.Text = resources.GetString("m_checkBSShowPageBounds.Text");
            this.m_checkBSShowPageBounds.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("m_checkBSShowPageBounds.TextAlign")));
            this.m_checkBSShowPageBounds.Visible = ((bool)(resources.GetObject("m_checkBSShowPageBounds.Visible")));
            // 
            // PageBorderDialog
            // 
            this.AcceptButton = this.m_btnOK;
            this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
            this.AccessibleName = resources.GetString("$this.AccessibleName");
#if SyncfusionFramework2_0
            this.AutoScaleDimensions = new SizeF(5, 13);
#else
			this.AutoScaleBaseSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScaleBaseSize")));
#endif
            this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
            this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
            this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
            this.Controls.Add(this.m_checkBSShowPageBounds);
            this.Controls.Add(this.m_checkBSRoundingNone);
            this.Controls.Add(this.m_pictureBorderPreview);
            this.Controls.Add(this.m_trackBSTransparency);
            this.Controls.Add(this.m_lblBorderDashStyle);
            this.Controls.Add(this.m_comboBSDashStyle);
            this.Controls.Add(this.m_comboBSWeight);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOK);
            this.Controls.Add(this.m_lblBorderColor);
            this.Controls.Add(this.m_lblBorderDashWeight);
            this.Controls.Add(this.m_lblBorderTransparency);
            this.Controls.Add(this.m_btnBSChooseColor);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.m_checkBSRoundingSmallest);
            this.Controls.Add(this.m_checkBSRoundingVerySmall);
            this.Controls.Add(this.m_checkBSRoundingSmall);
            this.Controls.Add(this.m_checkBSRoundingMedium);
            this.Controls.Add(this.m_checkBSRoundingBiggest);
            this.Controls.Add(this.m_checkBSRoundingVeryBig);
            this.Controls.Add(this.m_checkBSRoundingBig);
            this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
            this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
            this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
            this.MaximizeBox = false;
            this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
            this.MinimizeBox = false;
            this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
            this.Name = "PageBorderDialog";
            this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
            this.ShowInTaskbar = false;
            this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
            this.Text = resources.GetString("$this.Text");
            this.Load += new System.EventHandler(this.BorderStyleDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.m_trackBSTransparency)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the dialog's <see cref="Syncfusion.Windows.Forms.Diagram.PageBorderStyle"/> object.
        /// </summary>
        public PageBorderStyle PageBorderStyle
        {
            get 
            { 
                return m_pageBorderStyle; 
            }
            set
            {
                if (m_pageBorderStyle != value)
                {
                    if (m_pageBorderStyle == null)
                    {
                        m_pageBorderStyle = (PageBorderStyle)value.Clone();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether page border is shown.
        /// </summary>
        /// <value><c>true</c> if show page border; otherwise, <c>false</c>.</value>
        [Documentation.DocumentationExclude()]
        [Browsable(false)]
        public bool ShowPageBorder
        {
            get { return m_bShowPageBorder; }
            set { m_bShowPageBorder = value; }
        }
        #endregion

        #region Helper Methods
        private void DoBSRelatedActions()
        {
            SetRoundingButton();
            FillCombos();
            BindTo(m_comboBSWeight, "SelectedItem", this.PageBorderStyle, "BorderWeight");
            BindTo(m_comboBSDashStyle, "SelectedItem", this.PageBorderStyle, "BorderDashStyle");
            BindTo(m_trackBSTransparency, "Value", this.PageBorderStyle, "AlphaFactor");
            BindTo(m_checkBSShowPageBounds, "Checked", this.PageBorderStyle, "ShowBorder");
        }

        private void SetRoundingButton()
        {
            BorderStyleCornerRounding rounding = this.PageBorderStyle.BorderCornerRounding;
            switch (rounding)
            {
                case BorderStyleCornerRounding.None:
                    m_checkBSRoundingNone.Checked = true;
                    break;
                case BorderStyleCornerRounding.Smallest:
                    m_checkBSRoundingSmallest.Checked = true;
                    break;
                case BorderStyleCornerRounding.Smaller:
                    m_checkBSRoundingVerySmall.Checked = true;
                    break;
                case BorderStyleCornerRounding.Small:
                    m_checkBSRoundingSmall.Checked = true;
                    break;
                case BorderStyleCornerRounding.Medium:
                    m_checkBSRoundingMedium.Checked = true;
                    break;
                case BorderStyleCornerRounding.Big:
                    m_checkBSRoundingBig.Checked = true;
                    break;
                case BorderStyleCornerRounding.Bigger:
                    m_checkBSRoundingVeryBig.Checked = true;
                    break;
                case BorderStyleCornerRounding.Biggest:
                    m_checkBSRoundingBiggest.Checked = true;
                    break;
            }
        }

        private void BindTo(Control ctrlToBindTo, string strControlPropertyName, object objDataSource, string strDataSourcePropertyName)
        {
            if (ctrlToBindTo.DataBindings[strControlPropertyName] != null)
                ctrlToBindTo.DataBindings.Remove(ctrlToBindTo.DataBindings[strControlPropertyName]);
            ctrlToBindTo.DataBindings.Add(strControlPropertyName, objDataSource, strDataSourcePropertyName);
        }

        private void FillCombos()
        {
            // Weight combo
            Array arrProp = Enum.GetValues(typeof(BorderWeight));
            foreach (object obj in arrProp)
            {
                m_comboBSWeight.Items.Add(obj);
            }
            m_comboBSWeight.SelectedItem = BorderWeight.Thin;

            // Dash Style combo
            arrProp = Enum.GetValues(typeof(DashStyle));
            foreach (object obj in arrProp)
            {
                if (obj.ToString() != "Custom")
                    m_comboBSDashStyle.Items.Add(obj);
            }
            m_comboBSDashStyle.SelectedItem = DashStyle.Solid;
        }

        #region Combo Drawing
        private int GetPenWidth(BorderWeight enumWeight)
        {
            int nPenWidthToReturn = 0;

            switch (enumWeight)
            {
                case BorderWeight.Thin:
                    nPenWidthToReturn = 1;
                    break;
                case BorderWeight.Medium:
                    nPenWidthToReturn = 2;
                    break;
                case BorderWeight.Thick:
                    nPenWidthToReturn = 4;
                    break;
            }

            return nPenWidthToReturn;
        }

        private RectangleF CalculateDrawingRectangle(System.Drawing.Rectangle rectItem)
        {
            return new RectangleF(
                new PointF(rectItem.X + c_nBOUNDS_OFFSET, rectItem.Y + c_nBOUNDS_OFFSET),
                new SizeF(rectItem.Width - (c_nBOUNDS_OFFSET * 2), rectItem.Height - (c_nBOUNDS_OFFSET * 2)));
        }

        private PointF CalculateStartPoint(RectangleF rectBorder)
        {
            return new PointF(rectBorder.X, rectBorder.Y + (rectBorder.Height / 2));
        }

        private PointF CalculateEndPoint(RectangleF rectBorder)
        {
            return new PointF(rectBorder.X + rectBorder.Width, rectBorder.Y + (rectBorder.Height / 2));
        }

        #endregion Combo's Drawing

        #region Border Style Preview
        private void ChangeCurrentRounding(RoundingButton btnRounding)
        {
            if (m_checkPushed == null)
            {
                m_checkPushed = btnRounding;
                this.PageBorderStyle.BorderCornerRounding = btnRounding.Rounding;
            }
            else
            {
                if (Equals(m_checkPushed, btnRounding))
                {
                    m_checkPushed = null;
                    this.PageBorderStyle.BorderCornerRounding = BorderStyleCornerRounding.None;
                }
                else
                {
                    m_checkPushed.Checked = false;
                    m_checkPushed = btnRounding;
                    this.PageBorderStyle.BorderCornerRounding = btnRounding.Rounding;
                }
            }

            RefreshPreview();
        }

        private void RefreshPreview()
        {
            if (!m_bInitializing)
            {
                Graphics gph = CreateGraphicsToRender();
                gph.SmoothingMode = SmoothingMode.AntiAlias;
                DrawPreview(gph);
            }
        }

        private Graphics CreateGraphicsToRender()
        {
            Image img = new Bitmap(m_pictureBorderPreview.Bounds.Width, m_pictureBorderPreview.Bounds.Height);
            m_pictureBorderPreview.Image = img;
            return Graphics.FromImage(m_pictureBorderPreview.Image);
        }

        private void DrawPreview(Graphics gph)
        {
            int nRounding = (int)this.PageBorderStyle.BorderCornerRounding * 3;
            int nFirstAndSecondLinesConnectionPointX = c_nPREVIEW_BORDER_OFFSET_X + c_nPREVIEW_LINE_LENGHT;
            int nFirstAndSecondLinesConnectionPointY = m_pictureBorderPreview.Height - c_nPREVIEW_BORDER_OFFSET_Y;
            int nSecondAndThirdLinesConnectionPointX = nFirstAndSecondLinesConnectionPointX;
            int nSecondAndThirdLinesConnectionPointY = c_nPREVIEW_BORDER_OFFSET_Y;

            RectangleF rectArc = new RectangleF(new Point(0, 0), new Size(nRounding * 2, nRounding * 2));
            Pen pen = this.PageBorderStyle.CreatePen();

            PointF ptEnd = new PointF(c_nPREVIEW_BORDER_OFFSET_X, nFirstAndSecondLinesConnectionPointY);
            PointF ptStart = new PointF(nFirstAndSecondLinesConnectionPointX + (pen.Width / 2) - nRounding, nFirstAndSecondLinesConnectionPointY);

            // Draw first line
            gph.DrawLine(pen, ptStart, ptEnd);

            // Draw first round corner
            if (rectArc.Width != 0)
            {
                rectArc.X = ptStart.X - nRounding - (pen.Width / 2);
                rectArc.Y = ptStart.Y - (nRounding * 2);
                gph.DrawArc(pen, rectArc, 0, 90);
            }

            // Draw second line
            ptStart.X = nFirstAndSecondLinesConnectionPointX;
            ptStart.Y = nFirstAndSecondLinesConnectionPointY + (pen.Width / 2) - nRounding;
            ptEnd.X = nSecondAndThirdLinesConnectionPointX;
            ptEnd.Y = nSecondAndThirdLinesConnectionPointY - (pen.Width / 2) + nRounding;

            gph.DrawLine(pen, ptStart, ptEnd);

            // Draw second round corner
            if (rectArc.Width != 0)
            {
                rectArc.X = ptEnd.X;
                rectArc.Y = ptEnd.Y - nRounding;
                gph.DrawArc(pen, rectArc, 180, 90);
            }

            // Draw third line
            ptStart.X = nSecondAndThirdLinesConnectionPointX + nRounding;
            ptStart.Y = nSecondAndThirdLinesConnectionPointY - (pen.Width / 2);
            ptEnd.X = m_pictureBorderPreview.Width - c_nPREVIEW_BORDER_OFFSET_X;
            ptEnd.Y = nSecondAndThirdLinesConnectionPointY - (pen.Width / 2);
            gph.DrawLine(pen, ptStart, ptEnd);
        }

        #endregion Border Style Preview

        #endregion Helper Methods

        #region Event Handlers
        private void BorderStyleDialog_Load(object sender, EventArgs e)
        {
            m_bInitializing = true;
            DoBSRelatedActions();
            m_bInitializing = false;

            RefreshPreview();
        }

        private void BSWeight_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.DrawFocusRectangle();

            if (e.Index >= 0)
            {
                Pen pen;
                int nPenWidth;

                // Get value of current drawing item and convert it to pen penWidth( int )
                BorderWeight borderWeight =
                    (BorderWeight)Enum.Parse(typeof(BorderWeight), m_comboBSWeight.Items[e.Index].ToString());
                nPenWidth = GetPenWidth(borderWeight);

                // Calculate border rectangle
                RectangleF rectBorder = CalculateDrawingRectangle(e.Bounds);

                using (pen = new Pen(Color.Black, c_nONE_PIXEL))
                {
                    e.Graphics.DrawRectangle(pen, rectBorder.X, rectBorder.Y, rectBorder.Width, rectBorder.Height);

                    // Draw Weight style
                    PointF ptStart = CalculateStartPoint(rectBorder);
                    PointF ptEnd = CalculateEndPoint(rectBorder);
                    pen.Width = nPenWidth;
                    e.Graphics.DrawLine(pen, ptStart, ptEnd);
                }
            }
        }
        private void BSDashStyle_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.DrawFocusRectangle();

            if (e.Index >= 0)
            {
                Pen pen;

                DashStyle style =
                    (DashStyle)Enum.Parse(typeof(DashStyle), m_comboBSDashStyle.Items[e.Index].ToString());

                RectangleF rectBorder = CalculateDrawingRectangle(e.Bounds);

                using (pen = new Pen(Color.Black, c_nONE_PIXEL))
                {
                    e.Graphics.DrawRectangle(pen, rectBorder.X, rectBorder.Y, rectBorder.Width, rectBorder.Height);

                    PointF ptStart = CalculateStartPoint(rectBorder);
                    PointF ptEnd = CalculateEndPoint(rectBorder);
                    pen.DashStyle = style;
                    pen.Width = c_nTWO_PIXELS;
                    e.Graphics.DrawLine(pen, ptStart, ptEnd);
                }
            }
        }

        private void BSDashStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_bInitializing)
            {
                ComboBox combo = sender as ComboBox;
                this.PageBorderStyle.BorderDashStyle = (DashStyle)Enum.Parse(typeof(DashStyle), combo.SelectedItem.ToString());
                RefreshPreview();
            }
        }

        private void BSWeight_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_bInitializing)
            {
                ComboBox combo = sender as ComboBox;
                this.PageBorderStyle.BorderWeight = (BorderWeight)Enum.Parse(typeof(BorderWeight), combo.SelectedItem.ToString());
                RefreshPreview();
            }
        }

        private void BSChooseColor_Click(object sender, EventArgs e)
        {
            ColorDialog dlgColor = new ColorDialog();
            dlgColor.Color = this.PageBorderStyle.BorderColor;
            if (dlgColor.ShowDialog() == DialogResult.OK)
            {
                this.PageBorderStyle.BorderColor = dlgColor.Color;
                RefreshPreview();
            }
        }

        private void BSTransparency_Scroll(object sender, EventArgs e)
        {
            TrackBar track = sender as TrackBar;
            this.PageBorderStyle.AlphaFactor = track.Value;
            RefreshPreview();
        }

        private void BSRoundingNone_CheckedChanged(object sender, EventArgs e)
        {
            ChangeCurrentRounding(sender as RoundingButton);
        }

        private void BSRoundingSmallest_CheckedChanged(object sender, EventArgs e)
        {
            ChangeCurrentRounding(sender as RoundingButton);
        }

        private void BSRoundingVerySmall_CheckedChanged(object sender, EventArgs e)
        {
            ChangeCurrentRounding(sender as RoundingButton);
        }

        private void BSRoundingSmall_CheckedChanged(object sender, EventArgs e)
        {
            ChangeCurrentRounding(sender as RoundingButton);
        }

        private void BSRoundingMedium_CheckedChanged(object sender, EventArgs e)
        {
            ChangeCurrentRounding(sender as RoundingButton);
        }

        private void BSRoundingBig_CheckedChanged(object sender, EventArgs e)
        {
            ChangeCurrentRounding(sender as RoundingButton);
        }

        private void BSRoundingVeryBig_CheckedChanged(object sender, EventArgs e)
        {
            ChangeCurrentRounding(sender as RoundingButton);
        }

        private void BSRoundingBiggest_CheckedChanged(object sender, EventArgs e)
        {
            ChangeCurrentRounding(sender as RoundingButton);
        }

        #endregion Event Handlers
    }

    [Documentation.DocumentationExclude()]
    [ToolboxItem(false)]
    internal class RoundingButton : CheckBox
    {
        #region Constants
        private const int c_nROUNDING_OFFSET = 4;
        private const int c_nPEN_WIDTH = 2;
        #endregion Constants

        #region Fields
        private BorderStyleCornerRounding m_bsRounding = BorderStyleCornerRounding.Medium;
        #endregion Fields

        #region Properties
        public BorderStyleCornerRounding Rounding
        {
            get
            {
                return m_bsRounding;
            }
            set
            {
                if (m_bsRounding != value)
                    m_bsRounding = value;
            }
        }
        #endregion Properties

        #region Override
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Pen pen;
            int nRounding = (int)this.Rounding;

            RectangleF rectArc = new RectangleF(c_nROUNDING_OFFSET, c_nROUNDING_OFFSET, nRounding * 2, nRounding * 2);

            using (pen = new Pen(Color.Black, c_nPEN_WIDTH))
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                if (nRounding > 0)
                    e.Graphics.DrawArc(pen, rectArc, 180, 90);

                e.Graphics.DrawLine(
                    pen, 
                    new PointF(rectArc.X + nRounding, rectArc.Y),
                    new PointF(e.ClipRectangle.Right - c_nROUNDING_OFFSET, rectArc.Y));

                e.Graphics.DrawLine(
                    pen, 
                    new PointF(rectArc.X, rectArc.Y + nRounding),
                    new PointF(rectArc.X, e.ClipRectangle.Height - c_nROUNDING_OFFSET));
            }
        }

        #endregion Override
    }
}