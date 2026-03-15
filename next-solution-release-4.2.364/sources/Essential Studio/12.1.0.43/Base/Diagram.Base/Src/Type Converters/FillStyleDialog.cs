#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Class for Fill Style Dialog.
    /// </summary>
    public class FillStyleDialog : Form
    {
        #region Fields
        private FillStyle m_style = new FillStyle();

        #region Components
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private System.Windows.Forms.Label label3;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components = null;
        private TrackBar colorAlphaBar;
        private TextBox colorAlphaBox;
        private TextBox foreColorAlphaBox;
        private Button chooseForeColorButton;
        private Button chooseColorButton;
        private ComboBox brushTypeComboBox;
        private GroupBox previewGroupBox;
        private Button okButton;
        private Button cancelButton;
        private TrackBar foreColorAlphaBar;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox angleBox;
        private System.Windows.Forms.TrackBar angleBar;
        private System.Windows.Forms.Panel linearGradientPanel;
        private System.Windows.Forms.TrackBar centerBar;
        private System.Windows.Forms.TextBox centerBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox pathBrushBox;
        private System.Windows.Forms.Panel parentPanel;
        private System.Windows.Forms.Panel pathGradientPanel;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox hatchComboBox;
        private System.Windows.Forms.Panel hatchPanel;
        private System.Windows.Forms.Label foreColorLabel;
        private System.Windows.Forms.Label colorLabel;
        private System.Windows.Forms.Panel texturedBrushPanel;
        private System.Windows.Forms.ComboBox wrapModeComboBox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button fileOpenButton;
        private System.Windows.Forms.TextBox imageNameBox;
        private ColorDialog m_clrdlg = new ColorDialog();

        #endregion
        #endregion

        #region Class Construction/Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="FillStyleDialog"/> class.
        /// </summary>
        public FillStyleDialog()
        {
            InitializeComponent();

            SetUp();
        }

        /// <summary>
        /// Setting up bindings and filling comboBoxes with values
        /// </summary>
        private void SetUp()
        {
            // filling brushTypecombobox with values from FillStyleType enum
            foreach (object o in Enum.GetValues(typeof(FillStyleType)))
            {
                this.brushTypeComboBox.Items.Add(o);
            }

            // filling pathBrushBox with values from PathGradientBrushStyle enum
            foreach (object o in Enum.GetValues(typeof(PathGradientBrushStyle)))
            {
                this.pathBrushBox.Items.Add(o);
            }

            // filling hatchComboBox with values from HatchStyle enum
            foreach (string name in Enum.GetNames(typeof(HatchStyle)))
            {
                this.hatchComboBox.Items.Add(Enum.Parse(typeof(HatchStyle), name));
            }
            
            // filling wrapModeComboBox with values from WrapMode enum
            foreach (object o in Enum.GetValues(typeof(WrapMode)))
            {
                this.wrapModeComboBox.Items.Add(o);
            }
            brushTypeComboBox.DataBindings.Add("SelectedItem", m_style, "Type");
            pathBrushBox.DataBindings.Add("SelectedItem", m_style, "PathBrushStyle");
            Binding hatchbind = new Binding("SelectedItem", m_style, "HatchBrushStyle");
            
            // hatchbind.Format += new ConvertEventHandler(hatchFormat);
            // hatchbind.Parse += new ConvertEventHandler(hatchParse);
            hatchComboBox.DataBindings.Add(hatchbind);
            wrapModeComboBox.DataBindings.Add("SelectedItem", m_style, "TextureWrapMode");
            foreColorAlphaBar.DataBindings.Add("Value", m_style, "ForeColorAlphaFactor");
            colorAlphaBar.DataBindings.Add("Value", m_style, "ColorAlphaFactor");
            foreColorAlphaBox.DataBindings.Add("Text", m_style, "ForeColorAlphaFactor");
            colorAlphaBox.DataBindings.Add("Text", m_style, "ColorAlphaFactor");
            angleBox.DataBindings.Add("Text", m_style, "GradientAngle");
            angleBar.DataBindings.Add("Value", m_style, "GradientAngle");
            Binding b = new Binding("Value", m_style, "GradientCenter");
            b.Format += new ConvertEventHandler(centerFormat);
            b.Parse += new ConvertEventHandler(centerParse);
            centerBar.DataBindings.Add(b);
            centerBox.DataBindings.Add("Text", m_style, "GradientCenter");
        }

        private void hatchParse(object sender, ConvertEventArgs e)
        {
            if (e.DesiredType == typeof(HatchStyle))
            {
                e.Value = Enum.Parse(typeof(HatchStyle), e.Value as string);
            }
            else
            {
                e.Value = e.Value.ToString();
            }
        }

        // private void hatchFormat(object sender, ConvertEventArgs e)
        // {
        //    e.Value = e.Value.ToString();
        // }
        #endregion

        #region Finalization
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Panel tmpPanel = (parentPanel.Controls.Count > 0) ? parentPanel.Controls[0] as Panel : null;
                this.FillStyle.Type = (FillStyleType)brushTypeComboBox.SelectedItem;
                if (tmpPanel != null)
                {
                    tmpPanel = parentPanel.Controls[0] as Panel;
                    tmpPanel.Visible = false;
                    tmpPanel.Parent = null;
                }
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FillStyleDialog));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chooseForeColorButton = new System.Windows.Forms.Button();
            this.chooseColorButton = new System.Windows.Forms.Button();
            this.colorAlphaBox = new System.Windows.Forms.TextBox();
            this.colorAlphaBar = new System.Windows.Forms.TrackBar();
            this.foreColorAlphaBox = new System.Windows.Forms.TextBox();
            this.foreColorAlphaBar = new System.Windows.Forms.TrackBar();
            this.foreColorLabel = new System.Windows.Forms.Label();
            this.colorLabel = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.brushTypeComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.parentPanel = new System.Windows.Forms.Panel();
            this.texturedBrushPanel = new System.Windows.Forms.Panel();
            this.imageNameBox = new System.Windows.Forms.TextBox();
            this.fileOpenButton = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.wrapModeComboBox = new System.Windows.Forms.ComboBox();
            this.hatchPanel = new System.Windows.Forms.Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.hatchComboBox = new System.Windows.Forms.ComboBox();
            this.pathGradientPanel = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.pathBrushBox = new System.Windows.Forms.ComboBox();
            this.linearGradientPanel = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.angleBar = new System.Windows.Forms.TrackBar();
            this.angleBox = new System.Windows.Forms.TextBox();
            this.centerBar = new System.Windows.Forms.TrackBar();
            this.centerBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.previewGroupBox = new System.Windows.Forms.GroupBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.colorAlphaBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.foreColorAlphaBar)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.parentPanel.SuspendLayout();
            this.texturedBrushPanel.SuspendLayout();
            this.hatchPanel.SuspendLayout();
            this.pathGradientPanel.SuspendLayout();
            this.linearGradientPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.angleBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.centerBar)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chooseForeColorButton);
            this.groupBox1.Controls.Add(this.chooseColorButton);
            this.groupBox1.Controls.Add(this.colorAlphaBox);
            this.groupBox1.Controls.Add(this.colorAlphaBar);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.foreColorAlphaBox);
            this.groupBox1.Controls.Add(this.foreColorAlphaBar);
            this.groupBox1.Controls.Add(this.foreColorLabel);
            this.groupBox1.Controls.Add(this.colorLabel);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // chooseForeColorButton
            // 
            resources.ApplyResources(this.chooseForeColorButton, "chooseForeColorButton");
            this.chooseForeColorButton.Name = "chooseForeColorButton";
            this.chooseForeColorButton.Click += new System.EventHandler(this.chooseForeColorButton_Click);
            // 
            // chooseColorButton
            // 
            resources.ApplyResources(this.chooseColorButton, "chooseColorButton");
            this.chooseColorButton.Name = "chooseColorButton";
            this.chooseColorButton.Click += new System.EventHandler(this.chooseColorButton_Click);
            // 
            // colorAlphaBox
            // 
            resources.ApplyResources(this.colorAlphaBox, "colorAlphaBox");
            this.colorAlphaBox.Name = "colorAlphaBox";
            this.colorAlphaBox.ReadOnly = true;
            // 
            // colorAlphaBar
            // 
            this.colorAlphaBar.LargeChange = 8;
            resources.ApplyResources(this.colorAlphaBar, "colorAlphaBar");
            this.colorAlphaBar.Maximum = 255;
            this.colorAlphaBar.Name = "colorAlphaBar";
            this.colorAlphaBar.TickFrequency = 16;
            this.colorAlphaBar.ValueChanged += new System.EventHandler(this.colorAlphaBar_ValueChanged);
            // 
            // foreColorAlphaBox
            // 
            resources.ApplyResources(this.foreColorAlphaBox, "foreColorAlphaBox");
            this.foreColorAlphaBox.Name = "foreColorAlphaBox";
            this.foreColorAlphaBox.ReadOnly = true;
            // 
            // foreColorAlphaBar
            // 
            this.foreColorAlphaBar.LargeChange = 8;
            resources.ApplyResources(this.foreColorAlphaBar, "foreColorAlphaBar");
            this.foreColorAlphaBar.Maximum = 255;
            this.foreColorAlphaBar.Name = "foreColorAlphaBar";
            this.foreColorAlphaBar.TickFrequency = 16;
            this.foreColorAlphaBar.ValueChanged += new System.EventHandler(this.foreColorAlphaBar_ValueChanged);
            // 
            // foreColorLabel
            // 
            resources.ApplyResources(this.foreColorLabel, "foreColorLabel");
            this.foreColorLabel.Name = "foreColorLabel";
            this.foreColorLabel.Paint += new System.Windows.Forms.PaintEventHandler(this.foreColorLabel_Paint);
            // 
            // colorLabel
            // 
            resources.ApplyResources(this.colorLabel, "colorLabel");
            this.colorLabel.Name = "colorLabel";
            this.colorLabel.Paint += new System.Windows.Forms.PaintEventHandler(this.colorLabel_Paint);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.brushTypeComboBox);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.parentPanel);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // brushTypeComboBox
            // 
            this.brushTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.brushTypeComboBox, "brushTypeComboBox");
            this.brushTypeComboBox.Name = "brushTypeComboBox";
            this.brushTypeComboBox.SelectedValueChanged += new System.EventHandler(this.brushTypeComboBox_SelectedValueChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // parentPanel
            // 
            this.parentPanel.Controls.Add(this.texturedBrushPanel);
            this.parentPanel.Controls.Add(this.hatchPanel);
            this.parentPanel.Controls.Add(this.pathGradientPanel);
            this.parentPanel.Controls.Add(this.linearGradientPanel);
            resources.ApplyResources(this.parentPanel, "parentPanel");
            this.parentPanel.Name = "parentPanel";
            // 
            // texturedBrushPanel
            // 
            this.texturedBrushPanel.Controls.Add(this.imageNameBox);
            this.texturedBrushPanel.Controls.Add(this.fileOpenButton);
            this.texturedBrushPanel.Controls.Add(this.label10);
            this.texturedBrushPanel.Controls.Add(this.wrapModeComboBox);
            resources.ApplyResources(this.texturedBrushPanel, "texturedBrushPanel");
            this.texturedBrushPanel.Name = "texturedBrushPanel";
            // 
            // imageNameBox
            // 
            resources.ApplyResources(this.imageNameBox, "imageNameBox");
            this.imageNameBox.Name = "imageNameBox";
            // 
            // fileOpenButton
            // 
            resources.ApplyResources(this.fileOpenButton, "fileOpenButton");
            this.fileOpenButton.Name = "fileOpenButton";
            this.fileOpenButton.Click += new System.EventHandler(this.fileOpenButton_Click);
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // wrapModeComboBox
            // 
            this.wrapModeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.wrapModeComboBox, "wrapModeComboBox");
            this.wrapModeComboBox.Name = "wrapModeComboBox";
            this.wrapModeComboBox.SelectedValueChanged += new System.EventHandler(this.wrapModeComboBox_SelectedValueChanged);
            // 
            // hatchPanel
            // 
            this.hatchPanel.Controls.Add(this.label9);
            this.hatchPanel.Controls.Add(this.hatchComboBox);
            resources.ApplyResources(this.hatchPanel, "hatchPanel");
            this.hatchPanel.Name = "hatchPanel";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // hatchComboBox
            // 
            this.hatchComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.hatchComboBox, "hatchComboBox");
            this.hatchComboBox.Name = "hatchComboBox";
            this.hatchComboBox.Sorted = true;
            this.hatchComboBox.SelectedValueChanged += new System.EventHandler(this.hatchComboBox_SelectedValueChanged);
            // 
            // pathGradientPanel
            // 
            this.pathGradientPanel.Controls.Add(this.label8);
            this.pathGradientPanel.Controls.Add(this.pathBrushBox);
            resources.ApplyResources(this.pathGradientPanel, "pathGradientPanel");
            this.pathGradientPanel.Name = "pathGradientPanel";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // pathBrushBox
            // 
            this.pathBrushBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.pathBrushBox, "pathBrushBox");
            this.pathBrushBox.Name = "pathBrushBox";
            this.pathBrushBox.SelectedValueChanged += new System.EventHandler(this.pathBrushBox_SelectedValueChanged);
            // 
            // linearGradientPanel
            // 
            this.linearGradientPanel.Controls.Add(this.label4);
            this.linearGradientPanel.Controls.Add(this.angleBar);
            this.linearGradientPanel.Controls.Add(this.angleBox);
            this.linearGradientPanel.Controls.Add(this.centerBar);
            this.linearGradientPanel.Controls.Add(this.centerBox);
            this.linearGradientPanel.Controls.Add(this.label5);
            resources.ApplyResources(this.linearGradientPanel, "linearGradientPanel");
            this.linearGradientPanel.Name = "linearGradientPanel";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // angleBar
            // 
            this.angleBar.LargeChange = 10;
            resources.ApplyResources(this.angleBar, "angleBar");
            this.angleBar.Maximum = 360;
            this.angleBar.Minimum = -360;
            this.angleBar.Name = "angleBar";
            this.angleBar.TickFrequency = 45;
            this.angleBar.ValueChanged += new System.EventHandler(this.angleBar_ValueChanged);
            // 
            // angleBox
            // 
            resources.ApplyResources(this.angleBox, "angleBox");
            this.angleBox.Name = "angleBox";
            this.angleBox.ReadOnly = true;
            // 
            // centerBar
            // 
            this.centerBar.LargeChange = 10;
            resources.ApplyResources(this.centerBar, "centerBar");
            this.centerBar.Maximum = 100;
            this.centerBar.Name = "centerBar";
            this.centerBar.TickFrequency = 10;
            this.centerBar.ValueChanged += new System.EventHandler(this.centerBar_ValueChanged);
            // 
            // centerBox
            // 
            resources.ApplyResources(this.centerBox, "centerBox");
            this.centerBox.Name = "centerBox";
            this.centerBox.ReadOnly = true;
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // previewGroupBox
            // 
            resources.ApplyResources(this.previewGroupBox, "previewGroupBox");
            this.previewGroupBox.Name = "previewGroupBox";
            this.previewGroupBox.TabStop = false;
            this.previewGroupBox.Paint += new System.Windows.Forms.PaintEventHandler(this.PreviewPaint);
            // 
            // OKButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.okButton, "OKButton");
            this.okButton.Name = "OKButton";
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.Name = "cancelButton";
            // 
            // FillStyleDialog
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.previewGroupBox);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FillStyleDialog";
            this.ShowInTaskbar = false;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.colorAlphaBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.foreColorAlphaBar)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.parentPanel.ResumeLayout(false);
            this.texturedBrushPanel.ResumeLayout(false);
            this.texturedBrushPanel.PerformLayout();
            this.hatchPanel.ResumeLayout(false);
            this.pathGradientPanel.ResumeLayout(false);
            this.linearGradientPanel.ResumeLayout(false);
            this.linearGradientPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.angleBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.centerBar)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets FillStyle property
        /// </summary>
        public FillStyle FillStyle
        {
            get
            {
                return this.m_style;
            }
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Common Color dialog
        /// </summary>
        /// <param name="initialColor">The initial color.</param>
        /// <returns>The fill color.</returns>
        private Color OpenColorDialog(Color initialColor)
        {
            Color retColor = initialColor;
            m_clrdlg.Color = initialColor;
            if (m_clrdlg.ShowDialog() == DialogResult.OK)
            {
                retColor = m_clrdlg.Color;
            }
            return retColor;
        }

        #endregion

        #region EventHandlers
        private void chooseColorButton_Click(object sender, EventArgs e)
        {
            this.FillStyle.Color = OpenColorDialog(this.FillStyle.Color);
            this.ValueChanged(sender, e);
        }
        private void chooseForeColorButton_Click(object sender, System.EventArgs e)
        {
            this.FillStyle.ForeColor = OpenColorDialog(this.FillStyle.ForeColor);
            this.ValueChanged(sender, e);
        }
        private void ValueChanged(object sender, System.EventArgs e)
        {
            this.Invalidate(true);
        }
        private void PreviewPaint(object sender, PaintEventArgs e)
        {
            RectangleF rect = new RectangleF(
                new PointF(previewGroupBox.ClientRectangle.Location.X + 70, previewGroupBox.ClientRectangle.Location.Y + 30),
                new SizeF(previewGroupBox.ClientRectangle.Size.Width - 140, previewGroupBox.ClientRectangle.Size.Height - 60));
            RectangleF back = new RectangleF(
                new PointF(previewGroupBox.ClientRectangle.Location.X + 16, previewGroupBox.ClientRectangle.Location.Y + 16),
                new SizeF(previewGroupBox.ClientRectangle.Size.Width - 32, previewGroupBox.ClientRectangle.Size.Height - 24));

            using (Brush br = this.FillStyle.CreateBrush(e.Graphics, rect))
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.White), back);
                Bitmap bmp = new Bitmap(8, 8);
                Graphics gr = Graphics.FromImage(bmp);
                using (Brush brush = new SolidBrush(Color.Black))
                {
                    gr.FillRectangle(brush, 0, 0, 1, 1);
                }
                gr.Dispose();

                // Location of three edges of rectangle for drawing.
                PointF p1 = back.Location;
                PointF p2 = new PointF(p1.X + back.Width, p1.Y);
                PointF p3 = new PointF(p1.X, p1.Y + back.Height);
                PointF[] destPoints = { p1, p2, p3 };

                using (ImageAttributes attr = new ImageAttributes())
                {
                    attr.SetWrapMode(WrapMode.Tile);
                    e.Graphics.DrawImage(bmp, destPoints, back, GraphicsUnit.Pixel, attr);
                }
                e.Graphics.FillRectangle(br, rect);
                e.Graphics.DrawRectangle(Pens.Black, rect.X, rect.Y, rect.Width, rect.Height);
            }
        }
        private void colorAlphaBar_ValueChanged(object sender, System.EventArgs e)
        {
            this.FillStyle.ColorAlphaFactor = (sender as TrackBar).Value;
            this.colorAlphaBox.Text = this.FillStyle.ColorAlphaFactor.ToString();
            this.ValueChanged(sender, e);
        }
        private void foreColorAlphaBar_ValueChanged(object sender, System.EventArgs e)
        {
            this.FillStyle.ForeColorAlphaFactor = (sender as TrackBar).Value;
            this.foreColorAlphaBox.Text = this.FillStyle.ForeColorAlphaFactor.ToString();
            this.ValueChanged(sender, e);
        }
        private void brushTypeComboBox_SelectedValueChanged(object sender, System.EventArgs e)
        {
            foreach (Panel panel in parentPanel.Controls)
            {
                if (panel.Visible)
                {
                    panel.Visible = false;
                    break;
                }
            }

            this.FillStyle.Type = (FillStyleType)(sender as ComboBox).SelectedItem;

            switch (m_style.Type)
            {
                case FillStyleType.LinearGradient:
                    linearGradientPanel.Visible = true;
                    break;
                case FillStyleType.PathGradient:
                    pathGradientPanel.Visible = true;
                    break;
                case FillStyleType.Solid:
                    break;
                case FillStyleType.Hatch:
                    hatchPanel.Visible = true;
                    break;
                case FillStyleType.Texture:
                    texturedBrushPanel.Visible = true;
                    break;
                default:
                    break;
            }
            this.ValueChanged(sender, e);
        }
        private void angleBar_ValueChanged(object sender, System.EventArgs e)
        {
            this.FillStyle.GradientAngle = (sender as TrackBar).Value;
            this.angleBox.Text = this.FillStyle.GradientAngle.ToString();
            this.ValueChanged(sender, e);
        }
        private void centerBar_ValueChanged(object sender, System.EventArgs e)
        {
            if (this.FillStyle.GradientCenter != (((float)(sender as TrackBar).Value) / 100))
            {
                this.FillStyle.GradientCenter = ((float)(sender as TrackBar).Value) / 100;
                this.centerBox.Text = this.FillStyle.GradientCenter.ToString();
                this.ValueChanged(sender, e);
            }
        }
        private void centerFormat(object sender, ConvertEventArgs e)
        {
            e.Value = (int)Math.Ceiling((float)e.Value * 100);
        }
        private void centerParse(object sender, ConvertEventArgs e)
        {
            e.Value = (float)e.Value / 100;
        }
        private void pathBrushBox_SelectedValueChanged(object sender, System.EventArgs e)
        {
            this.FillStyle.PathBrushStyle = (PathGradientBrushStyle)(sender as ComboBox).SelectedItem;
            this.ValueChanged(sender, e);
        }
        private void hatchComboBox_SelectedValueChanged(object sender, System.EventArgs e)
        {
            this.FillStyle.HatchBrushStyle = (HatchStyle)(sender as ComboBox).SelectedItem;
            this.ValueChanged(sender, e);
        }
        private void colorLabel_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            using (SolidBrush br = new SolidBrush(this.FillStyle.Color))
            {
                e.Graphics.FillRectangle(br, this.colorLabel.ClientRectangle);
            }
        }
        private void foreColorLabel_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            using (SolidBrush br = new SolidBrush(this.FillStyle.ForeColor))
            {
                e.Graphics.FillRectangle(br, this.foreColorLabel.ClientRectangle);
            }
        }
        private void wrapModeComboBox_SelectedValueChanged(object sender, System.EventArgs e)
        {
            this.FillStyle.TextureWrapMode = (WrapMode)(sender as ComboBox).SelectedItem;
            this.ValueChanged(sender, e);
        }
        private void fileOpenButton_Click(object sender, System.EventArgs e)
        {
            OpenFileDialog dlgImage = new OpenFileDialog();
            dlgImage.Filter = "Windows Bitmaps (*.bmp)|*.bmp|JPEG files (*.jpg)|*.jpg|Graphics Interchange Format files (*.gif)|*.gif|Portable Network Graphics files (*.png)|*.png| Enhanced Metafiles (*.emf)|*.emf|All files (*.*)|*.*";
            dlgImage.DefaultExt = "*.bmp;*.jpg;*.gif;*.png;*.emf";
            dlgImage.Title = "Select an image file";

            if (dlgImage.ShowDialog() == DialogResult.OK)
            {
                Image imgToReturn = null;
                if (File.Exists(dlgImage.FileName))
                    imgToReturn = Image.FromFile(dlgImage.FileName);
                if (imgToReturn != null)
                {
                    this.FillStyle.Texture = imgToReturn;
                }
                this.imageNameBox.Text = dlgImage.FileName;
                this.ValueChanged(sender, e);
            }
        }
        #endregion
    }
}
