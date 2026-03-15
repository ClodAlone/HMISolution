//-------------------------------------------------------------------------------------------------
// <copyright file="GridRichTextEntryPanel.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using Syncfusion.Windows.Forms;
using Syncfusion.Drawing;
using System.Collections.Generic;
using System.Reflection;
using Syncfusion.Windows.Forms.Tools;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Provides editing support for RichText. The <see cref="GridRichTextBoxCellRenderer"/>
    /// will display the panel inside a drop-down container.
    /// </summary>
    [ToolboxItem(false)]
    public class GridRichTextEntryPanel : System.Windows.Forms.UserControl
    {
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Panel pnlFonts;
        private System.Windows.Forms.ComboBox fontComboBox;
        private System.Windows.Forms.ComboBox fontSizeComboBox;
        private System.Windows.Forms.Panel pnlFontsTB;
        private System.Windows.Forms.ToolBar tbFont;
        private System.Windows.Forms.ToolBarButton boldButton;
        private System.Windows.Forms.ToolBarButton italicButton;
        private System.Windows.Forms.ToolBarButton underLineButton;
        private System.Windows.Forms.ToolBar tbAlign;
        private System.Windows.Forms.ToolBarButton laButton;
        private System.Windows.Forms.ToolBarButton centerButton;
        private System.Windows.Forms.ToolBarButton raButton;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private Syncfusion.Windows.Forms.PopupControlContainer popupControlContainer1;
        private ColorPickerButton colorPickerUIAdv1;
        private ContextMenu contextMenu;
        private ButtonAdv btnSave;
        private ButtonAdv btnCancel;
        private ButtonAdv btnLoad;
        private System.ComponentModel.IContainer components;
        private ComboBox colorCombo;

        bool initDone = false;

        List<string> colorslist = new List<string>();
        /// <summary>
        /// Initializes a new <see cref="GridRichTextEntryPanel"/> object.
        /// </summary>
        public GridRichTextEntryPanel()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitForm call.
            this.fontComboBox.DisplayMember = "Name";
            this.fontComboBox.ValueMember = "Name";
            this.fontComboBox.Items.AddRange(FontFamily.Families);
            this.fontComboBox.SelectedIndex = 2;
            this.fontSizeComboBox.SelectedIndex = 2;

            Type colorType = typeof(System.Drawing.Color);
            PropertyInfo[] propInfoList = colorType.GetProperties(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public);
            foreach (PropertyInfo c in propInfoList)
            {
                this.colorCombo.Items.Add(c.Name);
            }
            this.BackColor = Color.FromName("control");//
            
            this.contextMenu = new ContextMenu();
            item = new MenuItem("Copy");
            contextMenu.MenuItems.Add(item);
            item = new MenuItem("Paste");
            contextMenu.MenuItems.Add(item);
            item = new MenuItem("Delete");
            contextMenu.MenuItems.Add(item);
            item = new MenuItem("Bullets");
            contextMenu.MenuItems.Add(item);
            item = new MenuItem("StrikeOut");
            contextMenu.MenuItems.Add(item);            
            contextMenu.MenuItems[0].Click += new EventHandler(GridRichTextEntryPanel_Click);
            contextMenu.MenuItems[1].Click+=new EventHandler(GridRichTextEntryPanel_Click1);
            contextMenu.MenuItems[2].Click += new EventHandler(GridRichTextEntryPanel_Click2);
            contextMenu.MenuItems[3].Click += new EventHandler(GridRichTextEntryPanel_Click3);    
            contextMenu.MenuItems[4].Click+=new EventHandler(GridRichTextEntryPanel_Click4);
         
            this.richTextBox1.AllowDrop = true;
            //this.customOption.SelectedIndexChanged += new EventHandler(customOption_SelectedIndexChanged);
            this.BackColorChanged += new EventHandler(GridRichTextEntryPanel_BackColorChanged);
            this.richTextBox1.TextChanged += new EventHandler(richTextBox1_TextChanged);
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(161)))), ((int)(((byte)(226))))); //
            this.richTextBox1.BackColorChanged += new EventHandler(richTextBox1_BackColorChanged);
            this.richTextBox1.MouseDown += new MouseEventHandler(richTextBox1_MouseDown);
            this.richTextBox1.DetectUrls = true;
            this.richTextBox1.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.richTextBox1_LinkClicked);
            this.richTextBox1.DragEnter += new System.Windows.Forms.DragEventHandler(this.richTextBox1_DragEnter);
            this.richTextBox1.DragDrop += new System.Windows.Forms.DragEventHandler(this.richTextBox1_DragDrop);

            this.btnLoad.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            initDone = true;
        }

        void colorCombo_DrawItem(object sender, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle rect = e.Bounds;
            if (e.Index >= 0)
            {
                string n = ((ComboBox)sender).Items[e.Index].ToString();
                Font f = new Font("Arial", 9, FontStyle.Regular);
                Color c = Color.FromName(n);
                Brush b = new SolidBrush(c);
                g.DrawString(n, f, Brushes.Black, rect.X, rect.Top);
                g.FillRectangle(b, rect.X + 110, rect.Y + 5,
                                rect.Width - 10, rect.Height - 10);
            }
        }

        void GridRichTextEntryPanel_Click(object sender, EventArgs e)
        {
            this.richTextBox1.Copy();
        }
        void GridRichTextEntryPanel_Click1(object sender, EventArgs e)
        {
            this.richTextBox1.Paste();
        }
        void GridRichTextEntryPanel_Click2(object sender, EventArgs e)
        {
            this.richTextBox1.SelectedText = "";
        }
        void GridRichTextEntryPanel_Click3(object sender, EventArgs e)
        {
            this.richTextBox1.SelectionBullet=true;
        }
        void GridRichTextEntryPanel_Click4(object sender, EventArgs e)
        {
            this.richTextBox1.SelectionFont = new Font(this.richTextBox1.SelectionFont, FontStyle.Strikeout);
        }

        
        public System.Diagnostics.Process p = new System.Diagnostics.Process();

        private void richTextBox1_LinkClicked(object sender,
        System.Windows.Forms.LinkClickedEventArgs e)
        {
            try
            {
                // Call Process.Start method to open a browser
                // with link text as URL.
                p = System.Diagnostics.Process.Start("IExplore.exe", e.LinkText);
            }
            catch
            {
            }
        }
        public void StopWebProcess()
        {
            p.Kill();
        }

        private void richTextBox1_DragEnter(object sender,System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }
        private void richTextBox1_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            int i;
            String s;

            // Get start position to drop the text.
            i = richTextBox1.SelectionStart;
            s = richTextBox1.Text.Substring(i);
            richTextBox1.Text = richTextBox1.Text.Substring(0, i);

            // Drop the text on to the RichTextBox.
            richTextBox1.Text = richTextBox1.Text +
               e.Data.GetData(DataFormats.Text).ToString();
            richTextBox1.Text = richTextBox1.Text + s;
        }
        void item_Select(object sender, EventArgs e)
        {

        }

        void ContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {

        }

        MenuItem item;
        public MenuItem menuItem
        {
            get
            {
                return item;
            }
            set
            {
                item.MenuItems.Clear();
                item = value;
            }
        }

        public ContextMenu context
        {
            get
            {
                return contextMenu;
            }
            set
            {
                contextMenu = value;
            }
        }

        void richTextBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (ContextMenu == null)
                {
                    if (context != null)
                    {
                        ContextMenu = context;
                    }
                    else
                    {
                        ContextMenu = contextMenu;
                    }                    
                }
                ContextMenu.Show(this, e.Location);
            }
        }

        void item_Click(object sender, EventArgs e)
        {
        }

        void ContextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }     


        void richTextBox1_BackColorChanged(object sender, EventArgs e)
        {
            Color temp = this.richTextBox1.BackColor;
            this.richTextBox1.BackColor = Color.White;
            this.BackColor = ColorTranslator.FromHtml("#F5F5F5");//temp;
           
            this.btnSave.UseVisualStyle = true;
            this.btnSave.Appearance = ButtonAppearance.Metro;
            this.btnSave.BackColor = ColorTranslator.FromHtml("#1DAEC3");
            this.btnLoad.BackColor = ColorTranslator.FromHtml("#1DAEC3");
            this.btnCancel.BackColor = ColorTranslator.FromHtml("#1DAEC3");
        }

        void GridRichTextEntryPanel_BackColorChanged(object sender, EventArgs e)
        {
            if (this.BackColor.Name == "ff1ba1e2")
            {
                //this.boldButton.ImageIndex = 6;
                //this.italicButton.ImageIndex = 7;
                //this.underLineButton.ImageIndex = 8;
                //this.laButton.ImageIndex = 9;
                //this.centerButton.ImageIndex = 10;
                //this.raButton.ImageIndex = 11;
            }
        }
        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
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

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GridRichTextEntryPanel));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.pnlFonts = new System.Windows.Forms.Panel();
            this.colorCombo = new System.Windows.Forms.ComboBox();
            this.pnlFontsTB = new System.Windows.Forms.Panel();
            this.tbFont = new System.Windows.Forms.ToolBar();
            this.boldButton = new System.Windows.Forms.ToolBarButton();
            this.italicButton = new System.Windows.Forms.ToolBarButton();
            this.underLineButton = new System.Windows.Forms.ToolBarButton();
            this.fontSizeComboBox = new System.Windows.Forms.ComboBox();
            this.fontComboBox = new System.Windows.Forms.ComboBox();
            this.tbAlign = new System.Windows.Forms.ToolBar();
            this.laButton = new System.Windows.Forms.ToolBarButton();
            this.centerButton = new System.Windows.Forms.ToolBarButton();
            this.raButton = new System.Windows.Forms.ToolBarButton();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.btnSave = new Syncfusion.Windows.Forms.ButtonAdv();
            this.btnCancel = new Syncfusion.Windows.Forms.ButtonAdv();
            this.popupControlContainer1 = new Syncfusion.Windows.Forms.PopupControlContainer();
            this.colorPickerUIAdv1 = new Syncfusion.Windows.Forms.ColorPickerButton();
            this.btnLoad = new Syncfusion.Windows.Forms.ButtonAdv();
            this.pnlFonts.SuspendLayout();
            this.pnlFontsTB.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "");
            this.imageList1.Images.SetKeyName(1, "");
            this.imageList1.Images.SetKeyName(2, "");
            this.imageList1.Images.SetKeyName(3, "");
            this.imageList1.Images.SetKeyName(4, "");
            this.imageList1.Images.SetKeyName(5, "");
            // 
            // pnlFonts
            // 
            this.pnlFonts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlFonts.Controls.Add(this.colorCombo);
            this.pnlFonts.Controls.Add(this.pnlFontsTB);
            this.pnlFonts.Controls.Add(this.fontSizeComboBox);
            this.pnlFonts.Controls.Add(this.fontComboBox);
            this.pnlFonts.Controls.Add(this.tbAlign);
            this.pnlFonts.Location = new System.Drawing.Point(0, 0);
            this.pnlFonts.Name = "pnlFonts";
            this.pnlFonts.Size = new System.Drawing.Size(467, 29);
            this.pnlFonts.TabIndex = 6;
            // 
            // colorCombo
            // 
            this.colorCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.colorCombo.BackColor = System.Drawing.SystemColors.Window;
            this.colorCombo.DisplayMember = "Name";
            this.colorCombo.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.colorCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.colorCombo.DropDownWidth = 200;
            this.colorCombo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colorCombo.Location = new System.Drawing.Point(246, 2);
            this.colorCombo.Name = "colorCombo";
            this.colorCombo.Size = new System.Drawing.Size(61, 21);
            this.colorCombo.TabIndex = 9;
            this.colorCombo.ValueMember = "Name";
            this.colorCombo.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.colorCombo_DrawItem);
            this.colorCombo.SelectedIndexChanged += new System.EventHandler(this.colorCombo_SelectedIndexChanged);
            // 
            // pnlFontsTB
            // 
            this.pnlFontsTB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlFontsTB.Controls.Add(this.tbFont);
            this.pnlFontsTB.Location = new System.Drawing.Point(313, 3);
            this.pnlFontsTB.Name = "pnlFontsTB";
            this.pnlFontsTB.Size = new System.Drawing.Size(73, 26);
            this.pnlFontsTB.TabIndex = 5;
            // 
            // tbFont
            // 
            this.tbFont.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFont.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
            this.tbFont.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.boldButton,
            this.italicButton,
            this.underLineButton});
            this.tbFont.Divider = false;
            this.tbFont.Dock = System.Windows.Forms.DockStyle.None;
            this.tbFont.DropDownArrows = true;
            this.tbFont.ImageList = this.imageList1;
            this.tbFont.Location = new System.Drawing.Point(1, 0);
            this.tbFont.Name = "tbFont";
            this.tbFont.ShowToolTips = true;
            this.tbFont.Size = new System.Drawing.Size(72, 26);
            this.tbFont.TabIndex = 4;
            this.tbFont.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.tbFont_ButtonClick);
            // 
            // boldButton
            // 
            this.boldButton.ImageIndex = 3;
            this.boldButton.Name = "boldButton";
            this.boldButton.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
            this.boldButton.ToolTipText = "Bold";
            // 
            // italicButton
            // 
            this.italicButton.ImageIndex = 4;
            this.italicButton.Name = "italicButton";
            this.italicButton.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
            this.italicButton.ToolTipText = "Italic";
            // 
            // underLineButton
            // 
            this.underLineButton.ImageIndex = 5;
            this.underLineButton.Name = "underLineButton";
            this.underLineButton.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
            this.underLineButton.ToolTipText = "Underline";
            // 
            // fontSizeComboBox
            // 
            this.fontSizeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fontSizeComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.fontSizeComboBox.Items.AddRange(new object[] {
            "8",
            "9",
            "10",
            "11",
            "12",
            "14",
            "15"});
            this.fontSizeComboBox.Location = new System.Drawing.Point(194, 3);
            this.fontSizeComboBox.Name = "fontSizeComboBox";
            this.fontSizeComboBox.Size = new System.Drawing.Size(46, 21);
            this.fontSizeComboBox.TabIndex = 4;
            this.fontSizeComboBox.SelectedIndexChanged += new System.EventHandler(this.fontSizeComboBox_SelectedIndexChanged);
            // 
            // fontComboBox
            // 
            this.fontComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fontComboBox.DisplayMember = "Name";
            this.fontComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fontComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.fontComboBox.Location = new System.Drawing.Point(3, 2);
            this.fontComboBox.Name = "fontComboBox";
            this.fontComboBox.Size = new System.Drawing.Size(185, 21);
            this.fontComboBox.TabIndex = 5;
            this.fontComboBox.ValueMember = "Name";
            this.fontComboBox.SelectedIndexChanged += new System.EventHandler(this.fontComboBox_SelectedIndexChanged);
            // 
            // tbAlign
            // 
            this.tbAlign.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbAlign.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
            this.tbAlign.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.laButton,
            this.centerButton,
            this.raButton});
            this.tbAlign.ButtonSize = new System.Drawing.Size(23, 22);
            this.tbAlign.Divider = false;
            this.tbAlign.Dock = System.Windows.Forms.DockStyle.None;
            this.tbAlign.DropDownArrows = true;
            this.tbAlign.ImageList = this.imageList1;
            this.tbAlign.Location = new System.Drawing.Point(392, 3);
            this.tbAlign.Name = "tbAlign";
            this.tbAlign.ShowToolTips = true;
            this.tbAlign.Size = new System.Drawing.Size(72, 26);
            this.tbAlign.TabIndex = 7;
            this.tbAlign.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.tbAlign_ButtonClick);
            // 
            // laButton
            // 
            this.laButton.ImageIndex = 0;
            this.laButton.Name = "laButton";
            this.laButton.Pushed = true;
            this.laButton.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
            this.laButton.ToolTipText = "Left Aligned";
            // 
            // centerButton
            // 
            this.centerButton.ImageIndex = 1;
            this.centerButton.Name = "centerButton";
            this.centerButton.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
            this.centerButton.ToolTipText = "Center Aligned";
            // 
            // raButton
            // 
            this.raButton.ImageIndex = 2;
            this.raButton.Name = "raButton";
            this.raButton.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
            this.raButton.ToolTipText = "Right Aligned";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.Location = new System.Drawing.Point(0, 29);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(467, 168);
            this.richTextBox1.TabIndex = 7;
            this.richTextBox1.Text = "";
            this.richTextBox1.SelectionChanged += new System.EventHandler(this.richTextBox1_SelectionChanged);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(291, 200);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(72, 24);
            this.btnSave.TabIndex = 8;
            this.btnSave.Text = "&Save";
            this.btnSave.UseVisualStyle = true;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(379, 200);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(72, 24);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyle = true;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // popupControlContainer1
            // 
            this.popupControlContainer1.BackColor = System.Drawing.Color.Transparent;
            this.popupControlContainer1.Location = new System.Drawing.Point(710, 509);
            this.popupControlContainer1.Name = "popupControlContainer1";
            this.popupControlContainer1.Size = new System.Drawing.Size(173, 195);
            this.popupControlContainer1.TabIndex = 122;
            this.popupControlContainer1.Visible = false;
            // 
            // colorPickerUIAdv1
            // 
            this.colorPickerUIAdv1.ColorUISize = new System.Drawing.Size(208, 230);
            this.colorPickerUIAdv1.Dock = System.Windows.Forms.DockStyle.Top;
            this.colorPickerUIAdv1.Location = new System.Drawing.Point(0, 0);
            this.colorPickerUIAdv1.MinimumSize = new System.Drawing.Size(136, 195);
            this.colorPickerUIAdv1.Name = "colorPickerUIAdv1";
            this.colorPickerUIAdv1.Size = new System.Drawing.Size(467, 195);
            this.colorPickerUIAdv1.TabIndex = 0;
            this.colorPickerUIAdv1.Text = "colorPickerUIAdv1";
            // 
            // btnLoad
            // 
            this.btnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoad.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.btnLoad.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.btnLoad.ForeColor = System.Drawing.Color.White;
            this.btnLoad.Location = new System.Drawing.Point(201, 200);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(72, 24);
            this.btnLoad.TabIndex = 9;
            this.btnLoad.Text = "&Load";
            this.btnLoad.UseVisualStyle = true;
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // GridRichTextEntryPanel
            // 
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.pnlFonts);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.richTextBox1);
            this.Name = "GridRichTextEntryPanel";
            this.Size = new System.Drawing.Size(467, 232);
            this.pnlFonts.ResumeLayout(false);
            this.pnlFonts.PerformLayout();
            this.pnlFontsTB.ResumeLayout(false);
            this.pnlFontsTB.PerformLayout();
            this.ResumeLayout(false);

        }

        void colorSelection_DropDown(object sender, EventArgs e)
        {
        }

        void colorCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!initDone)
            {
                return;
            }

            this.richTextBox1.Focus();
            this.UpdateFont();
        }
        #endregion

        private FontStyle curStyle = FontStyle.Regular;
        private HorizontalAlignment curAlignment = HorizontalAlignment.Left;

        private void tbFont_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
        {
            if (!initDone)
            {
                return;
            }

            tbAlign_ButtonClick(sender, e);
        }

        private void tbAlign_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
        {
            if (!initDone)
            {
                return;
            }

            if (e.Button == this.boldButton)
            {
                if (this.boldButton.Pushed)
                {
                    this.curStyle = this.curStyle | FontStyle.Bold;
                }
                else
                {
                    this.curStyle &= ~FontStyle.Bold;
                }
            }
            else if (e.Button == this.italicButton)
            {
                if (this.italicButton.Pushed)
                {
                    this.curStyle = this.curStyle | FontStyle.Italic;
                }
                else
                {
                    this.curStyle &= ~FontStyle.Italic;
                }
            }
            else if (e.Button == this.underLineButton)
            {
                if (this.underLineButton.Pushed)
                {
                    this.curStyle = this.curStyle | FontStyle.Underline;
                }
                else
                {
                    this.curStyle &= ~FontStyle.Underline;
                }
            }
            else if (e.Button == this.laButton)
            {
                if (this.laButton.Pushed)
                {
                    this.curAlignment = HorizontalAlignment.Left;
                    this.centerButton.Pushed = false;
                    this.raButton.Pushed = false;
                }

                this.richTextBox1.SelectionAlignment = this.curAlignment;
            }
            else if (e.Button == this.centerButton)
            {
                if (this.centerButton.Pushed)
                {
                    this.curAlignment = HorizontalAlignment.Center;
                    this.laButton.Pushed = false;
                    this.raButton.Pushed = false;
                }

                this.richTextBox1.SelectionAlignment = this.curAlignment;
            }
            else if (e.Button == this.raButton)
            {
                if (this.raButton.Pushed)
                {
                    this.curAlignment = HorizontalAlignment.Right;
                    this.laButton.Pushed = false;
                    this.centerButton.Pushed = false;
                }

                this.richTextBox1.SelectionAlignment = this.curAlignment;
            }

            this.richTextBox1.Focus();
            this.UpdateFont();
        }

        private void fontComboBox_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (!initDone)
            {
                return;
            }

            this.richTextBox1.Focus();
            this.UpdateFont();
        }

        private void fontSizeComboBox_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (!initDone)
            {
                return;
            }

            this.richTextBox1.Focus();
            this.UpdateFont();
        }

        private void UpdateFont()
        {
            try
            {
                string family = this.fontComboBox.SelectedItem.ToString();
                int n2 = family.IndexOf("]");
                int n3 = family.IndexOf("=");
                if (n2 != -1 && n2 > n3)
                {
                    family = family.Substring(family.IndexOf("=") + 1, family.IndexOf("]") - family.IndexOf("=") - 1);
                }
                Font ft = FontUtil.CreateFont(family, float.Parse(this.fontSizeComboBox.Text), this.curStyle);
                this.richTextBox1.SelectionFont = FontUtil.CreateFont(family, float.Parse(this.fontSizeComboBox.Text), this.curStyle);
                string clrName = this.colorCombo.SelectedItem.ToString();
                this.richTextBox1.SelectionColor = Color.FromName(clrName);                
            }
            catch 
            { 
            }
        }

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            if (Save != null)
            {
                Save(this, EventArgs.Empty);
            }
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            if (Cancel != null)
            {
                Cancel(this, EventArgs.Empty);
            }
        }

        private void richTextBox1_SelectionChanged(object sender, System.EventArgs e)
        {
            if (!initDone)
            {
                return;
            }

            initDone = false;
            Font f = richTextBox1.SelectionFont;

            if (f != null)
            {
                try
                {
                    curStyle = f.Style;

                    this.fontComboBox.SelectedIndex = this.fontComboBox.FindString(f.Name);
                    this.fontSizeComboBox.Text = f.SizeInPoints.ToString();
                    this.boldButton.Pushed = (f.Style & FontStyle.Bold) != 0;
                    this.underLineButton.Pushed = (f.Style & FontStyle.Underline) != 0;
                    this.italicButton.Pushed = (f.Style & FontStyle.Italic) != 0;

                    curAlignment = this.richTextBox1.SelectionAlignment;
                    this.laButton.Pushed = curAlignment == HorizontalAlignment.Left;
                    this.centerButton.Pushed = curAlignment == HorizontalAlignment.Center;
                    this.raButton.Pushed = curAlignment == HorizontalAlignment.Right;
                }
                catch
                {
                }
            }
            // TODO: What about text color?
            initDone = true;
        }

        /// <returns>
        /// true if the key was processed by the control; otherwise, false.
        /// </returns>
        /// <override/>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            ////Syncfusion.Diagnostics.TraceUtil.TraceCurrentMethodInfo(keyData);
            if (this.ActiveControl == this.richTextBox1)
            {
                if (keyData == Keys.Enter)
                {
                    return false;
                }
            }

            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        /// Occurs when user clicks the "Save" button.
        /// </summary>
        public event EventHandler Save;

        /// <summary>
        /// Occurs when user clicks the "Cancel" button.
        /// </summary>
        public event EventHandler Cancel;

        /// <summary>
        /// Gets a reference to the <see cref="RichTextBox"/> that is being displayed.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public System.Windows.Forms.RichTextBox RichTextBox
        {
            get
            {
                return this.richTextBox1;
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1_SelectionChanged(sender, e);
            this.richTextBox1.Font = Font;
        }

        private void buttonAdv1_Click(object sender, EventArgs e)
        {
            this.richTextBox1.SelectionFont = new Font(this.richTextBox1.SelectionFont, FontStyle.Underline);
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.richTextBox1.Copy();
            this.Show();
            this.BringToFront();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.richTextBox1.Paste();
        }
        OpenFileDialog openFileDialog1 = new OpenFileDialog();
        private void btnLoad_Click(object sender, EventArgs e)
        {
            string text = this.richTextBox1.Text;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.LoadFile(openFileDialog1.FileName, RichTextBoxStreamType.PlainText);
            }
        }

    }
}
