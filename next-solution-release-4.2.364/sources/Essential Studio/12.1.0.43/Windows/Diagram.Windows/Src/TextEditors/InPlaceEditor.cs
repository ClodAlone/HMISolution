#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.Diagram;
using System.ComponentModel;
using System.Drawing.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using Syncfusion.Windows.Forms.Tools;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// In place editor form
    /// </summary>
    public class InPlaceEditor : Form
    {
        #region Constants
        #endregion

        #region Fields
        private Label m_label;
        private Node m_nodeToEdit;
        private ColorUIControl m_fontColorPicker;
        private ColorUIControl m_backColorPicker;
        private DiagramController m_controller;
        /// <summary>
        /// Occurs when the label text changed.
        /// </summary>
        public event EventHandler LabelTextChanged;
        #endregion

        #region Form controls
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel container;
        private System.Windows.Forms.TextBox txtLabel;
        private System.Windows.Forms.ToolStrip toolBar;

        private System.Windows.Forms.ToolStripButton btnBold;
        private System.Windows.Forms.ToolStripButton btnItalic;
        private System.Windows.Forms.ToolStripButton btnUnderline;
        private System.Windows.Forms.ToolStripButton btnStrikeout;
        private System.Windows.Forms.ToolStripButton btnAlignLeft;
        private System.Windows.Forms.ToolStripButton btnAlignCenter;
        private System.Windows.Forms.ToolStripButton btnAlignRight;
        private System.Windows.Forms.ToolStripDropDownButton btnFontColor;
        private System.Windows.Forms.ToolStripDropDownButton btnFontBackColor;
        private System.Windows.Forms.ToolStripComboBox comboFontFamily;
        private System.Windows.Forms.ToolStripComboBox comboFontSize;

        #endregion

        #region Initializie / Finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="InPlaceEditor"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public InPlaceEditor(DiagramController controller)
            : base()
        {
            InitializeComponent();
            InitializeColorPickers();
            InitializeComboBoxes();
            m_controller = controller;
            this.Deactivate += new EventHandler(InPlaceEditor_Deactivate);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Required method for Designer support 
        /// </summary>
        private void InitializeComponent()
        {
            //access to the resource Images
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager("Syncfusion.Diagram.Base.Resources.Images", typeof(Resources).Assembly);
            this.container = new BorderPanel();
            this.txtLabel = new System.Windows.Forms.TextBox();
            this.toolBar = new System.Windows.Forms.ToolStrip();
            this.btnBold = new System.Windows.Forms.ToolStripButton();
            this.btnItalic = new System.Windows.Forms.ToolStripButton();
            this.btnUnderline = new System.Windows.Forms.ToolStripButton();
            this.btnStrikeout = new System.Windows.Forms.ToolStripButton();
            this.btnAlignLeft = new System.Windows.Forms.ToolStripButton();
            this.btnAlignCenter = new System.Windows.Forms.ToolStripButton();
            this.btnAlignRight = new System.Windows.Forms.ToolStripButton();
            this.btnFontColor = new System.Windows.Forms.ToolStripDropDownButton();
            this.btnFontBackColor = new System.Windows.Forms.ToolStripDropDownButton();
            this.comboFontFamily = new System.Windows.Forms.ToolStripComboBox();
            this.comboFontSize = new System.Windows.Forms.ToolStripComboBox();
            this.container.SuspendLayout();
            this.toolBar.SuspendLayout();

            // 
            // panel1
            //            
            this.container.Dock = System.Windows.Forms.DockStyle.Fill;
            this.container.Location = new System.Drawing.Point(1, 1);
            this.container.Name = "container";
            this.container.Size = new System.Drawing.Size(305, 52);
            this.container.TabIndex = 0;
            // 
            // txtLabel
            // 
            this.txtLabel.Location = new System.Drawing.Point(2, 3);
            this.txtLabel.Multiline = true;
            this.txtLabel.Name = "txtLabel";
            this.txtLabel.Size = new System.Drawing.Size(105, 46);
            this.txtLabel.TabIndex = 0;
            this.txtLabel.Dock = DockStyle.Left;
            this.txtLabel.TextChanged += new EventHandler(OnTextChanged);
            // 
            // toolBar
            // 
            this.toolBar.AllowItemReorder = true;
            this.toolBar.AutoSize = false;
            this.toolBar.BackColor = System.Drawing.Color.White;
            this.toolBar.Dock = System.Windows.Forms.DockStyle.Right;
            this.toolBar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.comboFontFamily,
            this.comboFontSize,
            this.btnFontColor,
            this.btnFontBackColor,
            this.btnBold,
            this.btnItalic,
            this.btnUnderline,
            this.btnStrikeout,
            this.btnAlignLeft,
            this.btnAlignCenter,
            this.btnAlignRight});
            this.toolBar.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.toolBar.Location = new System.Drawing.Point(110, 3);
            this.toolBar.Name = "toolBar";
            this.toolBar.Size = new System.Drawing.Size(202, 48);
            this.toolBar.TabIndex = 3;
            // 
            // btnBold
            // 
            this.btnBold.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnBold.Image = ((System.Drawing.Image)(resources.GetObject("Bold")));
            this.btnBold.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnBold.Name = "btnBold";
            this.btnBold.Size = new System.Drawing.Size(23, 20);
            this.btnBold.Text = "Bold";
            this.btnBold.CheckOnClick = true;
            this.btnBold.Click += new EventHandler(btnBold_Click);
            // 
            // btnItalic
            // 
            this.btnItalic.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnItalic.Image = ((System.Drawing.Image)(resources.GetObject("Italic")));
            this.btnItalic.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnItalic.Name = "btnItalic";
            this.btnItalic.Size = new System.Drawing.Size(23, 20);
            this.btnItalic.Text = "Italic";
            this.btnItalic.CheckOnClick = true;
            this.btnItalic.Click += new EventHandler(btnItalic_Click);
            // 
            // btnUnderline
            // 
            this.btnUnderline.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnUnderline.Image = ((System.Drawing.Image)(resources.GetObject("Underline")));
            this.btnUnderline.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUnderline.Name = "btnUnderline";
            this.btnUnderline.Size = new System.Drawing.Size(23, 20);
            this.btnUnderline.Text = "Underline";
            this.btnUnderline.CheckOnClick = true;
            this.btnUnderline.Click += new EventHandler(btnUnderline_Click);
            // 
            // btnStrikeout
            // 
            this.btnStrikeout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnStrikeout.Image = ((System.Drawing.Image)(resources.GetObject("strikeout")));
            this.btnStrikeout.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStrikeout.Name = "btnStrikeout";
            this.btnStrikeout.Size = new System.Drawing.Size(23, 20);
            this.btnStrikeout.Text = "Strikeout";
            this.btnStrikeout.CheckOnClick = true;
            this.btnStrikeout.Click += new EventHandler(btnStrikeout_Click);
            // 
            // btnAlignLeft
            // 
            this.btnAlignLeft.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAlignLeft.Image = ((System.Drawing.Image)(resources.GetObject("AlignTextLeft")));
            this.btnAlignLeft.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAlignLeft.Name = "btnAlignLeft";
            this.btnAlignLeft.Size = new System.Drawing.Size(23, 20);
            this.btnAlignLeft.Text = "Near";
            this.btnAlignLeft.CheckOnClick = true;
            this.btnAlignLeft.Click += new EventHandler(btnAlignLeft_Click);
            // 
            // btnAlignCenter
            // 
            this.btnAlignCenter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAlignCenter.Image = ((System.Drawing.Image)(resources.GetObject("CenterText")));
            this.btnAlignCenter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAlignCenter.Name = "btnAlignCenter";
            this.btnAlignCenter.Size = new System.Drawing.Size(23, 20);
            this.btnAlignCenter.Text = "Center";
            this.btnAlignCenter.CheckOnClick = true;
            this.btnAlignCenter.Click += new EventHandler(btnAlignCenter_Click);
            // 
            // btnAlignRight
            // 
            this.btnAlignRight.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAlignRight.Image = ((System.Drawing.Image)(resources.GetObject("AlignTextRight")));
            this.btnAlignRight.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAlignRight.Name = "btnAlignRight";
            this.btnAlignRight.Size = new System.Drawing.Size(23, 20);
            this.btnAlignRight.Text = "Far";
            this.btnAlignRight.CheckOnClick = true;
            this.btnAlignRight.Click += new EventHandler(btnAlignRight_Click);
            // 
            // btnFontColor
            // 
            this.btnFontColor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnFontColor.Image = ((System.Drawing.Image)(resources.GetObject("FontColor")));
            this.btnFontColor.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFontColor.Name = "btnFontColor";
            this.btnFontColor.Size = new System.Drawing.Size(29, 20);
            this.btnFontColor.Text = "Font Color";
            // 
            // backColor
            // 
            this.btnFontBackColor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnFontBackColor.Image = ((System.Drawing.Image)(resources.GetObject("BackColor")));
            this.btnFontBackColor.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFontBackColor.Name = "btnFontBackColor";
            this.btnFontBackColor.Size = new System.Drawing.Size(29, 20);
            this.btnFontBackColor.Text = "Font BackColor";
            // 
            // comboFontFamily
            // 
            this.comboFontFamily.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.comboFontFamily.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboFontFamily.AutoSize = false;
            this.comboFontFamily.DropDownHeight = 100;
            this.comboFontFamily.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboFontFamily.DropDownWidth = 150;
            this.comboFontFamily.IntegralHeight = false;
            this.comboFontFamily.Name = "comboFontFamily";
            this.comboFontFamily.Size = new System.Drawing.Size(100, 23);
            this.comboFontFamily.SelectedIndexChanged += new EventHandler(comboFontFamily_SelectedIndexChanged);
            // 
            // comboFontSize
            // 
            this.comboFontSize.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.comboFontSize.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboFontSize.AutoSize = false;
            this.comboFontSize.DropDownHeight = 100;
            this.comboFontSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboFontSize.IntegralHeight = false;
            this.comboFontSize.Name = "comboFontSize";
            this.comboFontSize.Size = new System.Drawing.Size(33, 23);
            this.comboFontSize.SelectedIndexChanged += new EventHandler(comboFontSize_SelectedIndexChanged);
            // 
            // InPlaceEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(307, 52);
            this.ShowInTaskbar = false;
            this.container.ResumeLayout(false);
            this.container.PerformLayout();
            this.toolBar.ResumeLayout(false);
            this.toolBar.PerformLayout();

            this.container.Controls.Add(this.toolBar);
            this.container.Controls.Add(this.txtLabel);
            this.Controls.Add(this.container);

        }
        #endregion

        #region Public methods
        /// <summary>
        /// Determines whether the specified node to edit is editable.
        /// </summary>
        /// <param name="nodeToEdit">The node to edit.</param>
        /// <returns>
        /// <c>true</c> if the specified node to edit is editable; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEditable(Node nodeToEdit)
        {
            Label lbl = null;
            if (nodeToEdit is PathNode || nodeToEdit is Group)
            {
                PathNode node = nodeToEdit as PathNode;
                if (node != null)
                {
                    if (node.Labels.IsEmpty)
                    {
                        lbl = new Label(node, string.Empty);
                        node.Labels.Add(lbl);
                    }
                    else
                        lbl = node.Labels[0];
                }
                else
                {
                    Group group = nodeToEdit as Group;
                    if (group.Labels.IsEmpty)
                    {
                        lbl = new Label(group, string.Empty);
                        group.Labels.Add(lbl);
                    }
                    else
                        lbl = group.Labels[0];
                }
                m_label = lbl;
                m_nodeToEdit = nodeToEdit;
                return !lbl.ReadOnly;
            }
            return false;
        }

        /// <summary>
        /// Begins the edit.
        /// </summary>
        public void BeginEdit()
        {
            this.Location = Control.MousePosition;
            txtLabel.Text = m_label.Text;
            comboFontFamily.SelectedItem = m_label.FontStyle.Family;
            comboFontSize.SelectedItem = m_label.FontStyle.Size;
            btnBold.Checked = m_label.FontStyle.Bold;
            btnItalic.Checked = m_label.FontStyle.Italic;
            btnUnderline.Checked = m_label.FontStyle.Underline;
            btnStrikeout.Checked = m_label.FontStyle.Strikeout;
            btnAlignLeft.Checked = (m_label.HorizontalAlignment == StringAlignment.Near) ? true : false;
            btnAlignCenter.Checked = (m_label.HorizontalAlignment == StringAlignment.Center) ? true : false;
            btnAlignRight.Checked = (m_label.HorizontalAlignment == StringAlignment.Far) ? true : false;
            this.Show();
        }

        /// <summary>
        /// Ends the edit.
        /// </summary>
        public void EndEdit()
        {
            this.Hide();
        }
        #endregion

        #region Event handlers
        private void m_backColorPicker_ColorSelected(object sender, EventArgs e)
        {
            m_label.BackgroundStyle.Color = m_backColorPicker.SelectedColor;
            m_controller.Viewer.UpdateView(m_nodeToEdit);
            btnFontBackColor.HideDropDown();
        }

        private void m_fontColorPicker_ColorSelected(object sender, EventArgs e)
        {
            m_label.FontColorStyle.Color = m_fontColorPicker.SelectedColor;
            m_controller.Viewer.UpdateView(m_nodeToEdit);
            btnFontColor.HideDropDown();
        }

        private void InPlaceEditor_Deactivate(object sender, EventArgs e)
        {
            if (!(btnFontBackColor.Pressed || btnFontColor.Pressed))
                this.Hide();
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            m_label.Text = txtLabel.Text;
            if (this.LabelTextChanged != null)
            {
                this.LabelTextChanged(this, e);
            }
        }

        private void comboFontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_label.FontStyle.Size = (float)comboFontSize.SelectedItem;
        }

        private void comboFontFamily_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_label.FontStyle.Family = comboFontFamily.SelectedItem.ToString();
        }

        private void btnAlignRight_Click(object sender, EventArgs e)
        {
            UpdateAlignment(btnAlignRight);
        }

        private void btnAlignCenter_Click(object sender, EventArgs e)
        {
            UpdateAlignment(btnAlignCenter);
        }

        private void btnAlignLeft_Click(object sender, EventArgs e)
        {
            UpdateAlignment(btnAlignLeft);
        }

        private void btnStrikeout_Click(object sender, EventArgs e)
        {
            m_label.FontStyle.Strikeout = btnStrikeout.Checked;
        }

        private void btnUnderline_Click(object sender, EventArgs e)
        {
            m_label.FontStyle.Underline = btnUnderline.Checked;
        }

        private void btnItalic_Click(object sender, EventArgs e)
        {
            m_label.FontStyle.Italic = btnItalic.Checked;
        }

        private void btnBold_Click(object sender, EventArgs e)
        {
            m_label.FontStyle.Bold = btnBold.Checked;
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Initializes the color picker controls
        /// </summary>
        private void InitializeColorPickers()
        {
            m_fontColorPicker = new ColorUIControl();
            m_fontColorPicker.BorderStyle = BorderStyle.None;
            m_fontColorPicker.BackColor = Color.White;
            m_fontColorPicker.Size = new Size(200, 215);
            m_fontColorPicker.ColorSelected += new EventHandler(m_fontColorPicker_ColorSelected);
            ToolStripControlHost host = new ToolStripControlHost(m_fontColorPicker);
            ToolStripDropDown dropDown = new ToolStripDropDown();
            dropDown.Items.Add(host);
            btnFontColor.DropDown = dropDown;
            m_backColorPicker = new ColorUIControl();
            m_backColorPicker.Size = new Size(200, 215);
            m_backColorPicker.BackColor = Color.White;
            m_backColorPicker.BorderStyle = BorderStyle.None;
            m_backColorPicker.ColorSelected += new EventHandler(m_backColorPicker_ColorSelected);
            host = new ToolStripControlHost(m_backColorPicker);
            dropDown = new ToolStripDropDown();
            dropDown.Items.Add(host);
            btnFontBackColor.DropDown = dropDown;
        }

        /// <summary>
        /// Initializes the combo boxes
        /// </summary>
        private void InitializeComboBoxes()
        {
            //Add installed fonts in to comboFontFamily
            InstalledFontCollection fonts = new InstalledFontCollection();
            foreach (FontFamily family in fonts.Families)
                comboFontFamily.Items.Add(family.Name);

            for (float i = 6; i <= 72; i++)
                comboFontSize.Items.Add(i);
        }

        /// <summary>
        /// Updates the label alignment
        /// </summary>
        /// <param name="btnAlign">The ToolStripButton</param>
        private void UpdateAlignment(ToolStripButton btnAlign)
        {
            if (btnAlign.Checked)
            {
                if (btnAlign.Text == "Near")
                {
                    m_label.HorizontalAlignment = StringAlignment.Near;
                    m_label.VerticalAlignment = StringAlignment.Near;
                    btnAlignCenter.Checked = false;
                    btnAlignRight.Checked = false;
                }
                else if (btnAlign.Text == "Center")
                {
                    m_label.HorizontalAlignment = StringAlignment.Center;
                    m_label.VerticalAlignment = StringAlignment.Center;
                    btnAlignLeft.Checked = false;
                    btnAlignRight.Checked = false;
                }
                else if (btnAlign.Text == "Far")
                {
                    m_label.HorizontalAlignment = StringAlignment.Far;
                    m_label.VerticalAlignment = StringAlignment.Far;
                    btnAlignCenter.Checked = false;
                    btnAlignLeft.Checked = false;
                }
            }
            else
            {
                m_label.HorizontalAlignment = StringAlignment.Near;
                m_label.VerticalAlignment = StringAlignment.Near;
                btnAlignLeft.Checked = true;
                btnAlignCenter.Checked = false;
                btnAlignRight.Checked = false;
            }
        }
        #endregion
    }

    /// <summary>
    /// Custom panel class
    /// </summary>
    internal class BorderPanel : Panel
    {
        #region Overrides
        /// <summary>
        /// Called when the control needs to paint the window.
        /// </summary>
        /// <param name="e">The paint event argument.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics gfx = e.Graphics;
            gfx.DrawRectangle(Pens.LightGray, new System.Drawing.Rectangle(0, 0, this.Width - 1, this.Height - 1));
        }
        #endregion
    }
}
