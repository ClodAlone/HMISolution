#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.Diagram;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Class containing Shadow Style Dialog.
    /// </summary>
    public class ShadowStyleDialog : System.Windows.Forms.Form
    {
        #region Fields
        private ShadowStyle m_style = new ShadowStyle();
        private ColorDialog m_clrdlg = new ColorDialog();
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button chooseForeColorButton;
        private System.Windows.Forms.Button chooseColorButton;
        private System.Windows.Forms.TextBox colorAlphaBox;
        private System.Windows.Forms.TrackBar colorAlphaBar;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox foreColorAlphaBox;
        private System.Windows.Forms.TrackBar foreColorAlphaBar;
        private System.Windows.Forms.Label foreColorLabel;
        private System.Windows.Forms.Label colorLabel;
        private System.Windows.Forms.GroupBox previewGroupBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.NumericUpDown offsetXUpDown;
        private System.Windows.Forms.NumericUpDown offsetYUpDown;
        private System.Windows.Forms.CheckBox shadowCheckBox;
        #region Components
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        #endregion
        #endregion

        #region Class Construction/Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="ShadowStyleDialog"/> class.
        /// </summary>
        public ShadowStyleDialog()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
            InitializeComponent();

            SetUp();
        }

        /// <summary>
        /// Setting up bindings and filling comboBoxes with values
        /// </summary>
        private void SetUp()
        {
            foreColorAlphaBar.DataBindings.Add("Value", m_style, "ForeColorAlphaFactor");
            colorAlphaBar.DataBindings.Add("Value", m_style, "ColorAlphaFactor");
            foreColorAlphaBox.DataBindings.Add("Text", m_style, "ForeColorAlphaFactor");
            colorAlphaBox.DataBindings.Add("Text", m_style, "ColorAlphaFactor");
            offsetXUpDown.DataBindings.Add("Value", m_style, "OffsetX");
            offsetYUpDown.DataBindings.Add("Value", m_style, "OffsetY");
            shadowCheckBox.DataBindings.Add("Checked", m_style, "Visible");
        }

        #endregion

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShadowStyleDialog));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chooseForeColorButton = new System.Windows.Forms.Button();
            this.chooseColorButton = new System.Windows.Forms.Button();
            this.colorAlphaBox = new System.Windows.Forms.TextBox();
            this.colorAlphaBar = new System.Windows.Forms.TrackBar();
            this.label7 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.foreColorAlphaBox = new System.Windows.Forms.TextBox();
            this.foreColorAlphaBar = new System.Windows.Forms.TrackBar();
            this.foreColorLabel = new System.Windows.Forms.Label();
            this.colorLabel = new System.Windows.Forms.Label();
            this.previewGroupBox = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.offsetXUpDown = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.offsetYUpDown = new System.Windows.Forms.NumericUpDown();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.shadowCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.colorAlphaBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.foreColorAlphaBar)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.offsetXUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.offsetYUpDown)).BeginInit();
            this.SuspendLayout();
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
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
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
            // previewGroupBox
            // 
            resources.ApplyResources(this.previewGroupBox, "previewGroupBox");
            this.previewGroupBox.Name = "previewGroupBox";
            this.previewGroupBox.TabStop = false;
            this.previewGroupBox.Paint += new System.Windows.Forms.PaintEventHandler(this.PreviewPaint);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.offsetXUpDown);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.offsetYUpDown);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // offsetXUpDown
            // 
            resources.ApplyResources(this.offsetXUpDown, "offsetXUpDown");
            this.offsetXUpDown.Name = "offsetXUpDown";
            this.offsetXUpDown.ValueChanged += new System.EventHandler(this.offsetXUpDown_ValueChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // offsetYUpDown
            // 
            resources.ApplyResources(this.offsetYUpDown, "offsetYUpDown");
            this.offsetYUpDown.Name = "offsetYUpDown";
            this.offsetYUpDown.ValueChanged += new System.EventHandler(this.offsetYUpDown_ValueChanged);
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.button2, "button2");
            this.button2.Name = "button2";
            // 
            // shadowCheckBox
            // 
            resources.ApplyResources(this.shadowCheckBox, "shadowCheckBox");
            this.shadowCheckBox.Name = "shadowCheckBox";
            this.shadowCheckBox.CheckedChanged += new System.EventHandler(this.shadowCheckBox_CheckedChanged);
            // 
            // ShadowStyleDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.shadowCheckBox);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.previewGroupBox);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ShadowStyleDialog";
            this.ShowInTaskbar = false;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.colorAlphaBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.foreColorAlphaBar)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.offsetXUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.offsetYUpDown)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets FillStyle property
        /// </summary>
        public ShadowStyle ShadowStyle
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
        /// <returns>The shadow color.</returns>
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
            this.ShadowStyle.Color = OpenColorDialog(this.ShadowStyle.Color);
            this.ValueChanged(sender, e);
        }
        private void chooseForeColorButton_Click(object sender, System.EventArgs e)
        {
            this.ShadowStyle.ForeColor = OpenColorDialog(this.ShadowStyle.ForeColor);
            this.ValueChanged(sender, e);
        }
        private void ValueChanged(object sender, System.EventArgs e)
        {
            this.Invalidate(true);
        }
        private void PreviewPaint(object sender, PaintEventArgs e)
        {
            RectangleF rect = new RectangleF(
                new PointF(previewGroupBox.ClientRectangle.Location.X + 100, previewGroupBox.ClientRectangle.Location.Y + 40),
                new SizeF(previewGroupBox.ClientRectangle.Size.Width - 200, previewGroupBox.ClientRectangle.Size.Height - 80));
            RectangleF back = new RectangleF(
                new PointF(previewGroupBox.ClientRectangle.Location.X + 16, previewGroupBox.ClientRectangle.Location.Y + 16),
                new SizeF(previewGroupBox.ClientRectangle.Size.Width - 32, previewGroupBox.ClientRectangle.Size.Height - 24));
            RectangleF shadowrect = rect;
            shadowrect.Offset(this.m_style.OffsetX, this.m_style.OffsetY);

            using (Brush br = this.ShadowStyle.CreateBrush(e.Graphics, shadowrect))
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
                if (shadowCheckBox.Checked)
                {
                    e.Graphics.FillRectangle(br, shadowrect);
                }
                e.Graphics.FillRectangle(new SolidBrush(Color.LightGreen), rect);
                e.Graphics.DrawRectangle(Pens.Black, rect.X, rect.Y, rect.Width, rect.Height);
            }
        }
        private void colorAlphaBar_ValueChanged(object sender, System.EventArgs e)
        {
            this.ShadowStyle.ColorAlphaFactor = (sender as TrackBar).Value;
            this.colorAlphaBox.Text = this.ShadowStyle.ColorAlphaFactor.ToString();
            this.ValueChanged(sender, e);
        }
        private void foreColorAlphaBar_ValueChanged(object sender, System.EventArgs e)
        {
            this.ShadowStyle.ForeColorAlphaFactor = (sender as TrackBar).Value;
            this.foreColorAlphaBox.Text = this.ShadowStyle.ForeColorAlphaFactor.ToString();
            this.ValueChanged(sender, e);
        }
        private void colorLabel_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            using (SolidBrush br = new SolidBrush(this.ShadowStyle.Color))
            {
                e.Graphics.FillRectangle(br, this.colorLabel.ClientRectangle);
            }
        }
        private void foreColorLabel_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            using (SolidBrush br = new SolidBrush(this.ShadowStyle.ForeColor))
            {
                e.Graphics.FillRectangle(br, this.foreColorLabel.ClientRectangle);
            }
        }
        private void offsetYUpDown_ValueChanged(object sender, System.EventArgs e)
        {
            this.m_style.OffsetY = (float)(sender as NumericUpDown).Value;
            this.ValueChanged(sender, e);
        }
        private void offsetXUpDown_ValueChanged(object sender, System.EventArgs e)
        {
            this.m_style.OffsetX = (float)(sender as NumericUpDown).Value;
            this.ValueChanged(sender, e);
        }
        #endregion

        private void shadowCheckBox_CheckedChanged(object sender, System.EventArgs e)
        {
            this.m_style.Visible = (sender as CheckBox).Checked;
            this.ValueChanged(sender, e);
        }
    }
}
