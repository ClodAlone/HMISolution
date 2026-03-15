#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    ///  ColorDlgAdv class.
    /// </summary>
    public class ColorDlgAdv :
        Office2007Form
    {
        private Syncfusion.Windows.Forms.Tools.TabControlAdv tabControl1;
        private Syncfusion.Windows.Forms.Tools.HexagonColorControl hexagonColorControl1;
        private Syncfusion.Windows.Forms.Tools.TabPageAdv standard;
        private Syncfusion.Windows.Forms.Tools.TabPageAdv custom;
        private Syncfusion.Windows.Forms.ButtonAdv buttonAdv1;
        private Syncfusion.Windows.Forms.ButtonAdv buttonAdv2;
        private Syncfusion.Windows.Forms.Tools.AutoLabel autoLabel1;
        private Syncfusion.Windows.Forms.Tools.AutoLabel autoLabel2;
        private ColorPickerUIAdv m_colorPicker = null;
        private Color m_color = Color.Empty;
        private Syncfusion.Windows.Forms.Tools.GradientColorControl gradientColorControl1;
        private Syncfusion.Windows.Forms.Tools.AutoLabel autoLabel3;
        private Syncfusion.Windows.Forms.Tools.AutoLabel autoLabel4;
        private Syncfusion.Windows.Forms.Tools.AutoLabel autoLabel5;
        private Syncfusion.Windows.Forms.Tools.AutoLabel autoLabel6;
        private Syncfusion.Windows.Forms.Tools.ComboBoxAdv comboBox1;
        private Syncfusion.Windows.Forms.Tools.NumericUpDownExt numericUpDown1;
        private Syncfusion.Windows.Forms.Tools.NumericUpDownExt numericUpDown2;
        private Syncfusion.Windows.Forms.Tools.NumericUpDownExt numericUpDown3;
        private Syncfusion.Windows.Forms.Tools.AutoLabel autoLabel7;
        private Syncfusion.Windows.Forms.Tools.GradientBar gradientBar1;
        private System.ComponentModel.IContainer components = null;

        private bool m_bShouldChangeSelection = true;

        public Color Color
        {
            get { return m_color; }
        }

        public ColorDlgAdv(ColorPickerUIAdv colorPicker)
        {
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            InitializeComponent();

            m_colorPicker = colorPicker;

            if (m_colorPicker.UseOffice2007Style)
            {
                SetOffice2007Style();
            }

            LocalizeComponent();
        }

        private void LocalizeComponent()
        {
            this.standard.Text = SR.GetString(SR.ColorEditorStandardTab);
            this.custom.Text = SR.GetString(SR.ColorEditorCustomTab);
            this.autoLabel7.Text = SR.GetString(SR.ColorEditorBlueLabel);
            this.autoLabel6.Text = SR.GetString(SR.ColorEditorGreenLabel);
            this.autoLabel5.Text = SR.GetString(SR.ColorEditorRedLabel);
            this.autoLabel4.Text = SR.GetString(SR.ColorEditorColorModelLabel);
            this.autoLabel3.Text = SR.GetString(SR.ColorEditorColorsLabel);
            this.buttonAdv1.Text = SR.GetString(SR.ColorEditorOKButton);
            this.buttonAdv2.Text = SR.GetString(SR.ColorEditorCancelButton);
            this.autoLabel1.Text = SR.GetString(SR.ColorEditorNewLabel);
            this.autoLabel2.Text = SR.GetString(SR.ColorEditorCurrentLabel);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">Bool value for Dissposing</param>
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

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Rectangle rect = new Rectangle(this.buttonAdv1.Location.X + 10, this.autoLabel1.Location.Y + 20, 50, 40);

            if (m_colorPicker.SelectedItem != null)
            {

                using (Brush brush = new SolidBrush(this.Color))
                    e.Graphics.FillRectangle(brush, new Rectangle(rect.Location, new Size(rect.Width, rect.Height / 2)));
                using (Brush brush = new SolidBrush(m_colorPicker.SelectedItem.Color))
                    e.Graphics.FillRectangle(brush, new Rectangle(new Point(rect.X, rect.Y + 20), new Size(rect.Width, rect.Height / 2)));
            }
            else
                e.Graphics.FillRectangle(new SolidBrush(this.Color), rect);

            e.Graphics.DrawRectangle(new Pen(Color.FromArgb(99, 101, 99)), rect);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ColorDlgAdv));
            this.tabControl1 = new Syncfusion.Windows.Forms.Tools.TabControlAdv();
            this.standard = new Syncfusion.Windows.Forms.Tools.TabPageAdv();
            this.hexagonColorControl1 = new Syncfusion.Windows.Forms.Tools.HexagonColorControl();
            this.custom = new Syncfusion.Windows.Forms.Tools.TabPageAdv();
            this.gradientBar1 = new Syncfusion.Windows.Forms.Tools.GradientBar();
            this.autoLabel7 = new Syncfusion.Windows.Forms.Tools.AutoLabel();
            this.numericUpDown3 = new Syncfusion.Windows.Forms.Tools.NumericUpDownExt();
            this.numericUpDown2 = new Syncfusion.Windows.Forms.Tools.NumericUpDownExt();
            this.numericUpDown1 = new Syncfusion.Windows.Forms.Tools.NumericUpDownExt();
            this.comboBox1 = new Syncfusion.Windows.Forms.Tools.ComboBoxAdv();
            this.autoLabel6 = new Syncfusion.Windows.Forms.Tools.AutoLabel();
            this.autoLabel5 = new Syncfusion.Windows.Forms.Tools.AutoLabel();
            this.autoLabel4 = new Syncfusion.Windows.Forms.Tools.AutoLabel();
            this.autoLabel3 = new Syncfusion.Windows.Forms.Tools.AutoLabel();
            this.gradientColorControl1 = new Syncfusion.Windows.Forms.Tools.GradientColorControl();
            this.buttonAdv1 = new Syncfusion.Windows.Forms.ButtonAdv();
            this.buttonAdv2 = new Syncfusion.Windows.Forms.ButtonAdv();
            this.autoLabel1 = new Syncfusion.Windows.Forms.Tools.AutoLabel();
            this.autoLabel2 = new Syncfusion.Windows.Forms.Tools.AutoLabel();
            ((System.ComponentModel.ISupportInitialize)(this.tabControl1)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.standard.SuspendLayout();
            this.custom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.comboBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.ActiveTabFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.tabControl1.Controls.Add(this.standard);
            this.tabControl1.Controls.Add(this.custom);
            this.tabControl1.Location = new System.Drawing.Point(8, 8);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.Size = new System.Drawing.Size(264, 312);
            this.tabControl1.TabGap = 10;
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.TabControl1_SelectedIndexChanged);
            // 
            // Standart
            // 
            this.standard.Controls.Add(this.hexagonColorControl1);
            this.standard.Location = new System.Drawing.Point(1, 32);
            this.standard.IsTransparent = true;
            this.standard.Name = "Standard";
            this.standard.Size = new System.Drawing.Size(261, 278);
            this.standard.TabIndex = 1;
            this.standard.Text = "Standard";
            this.standard.ThemesEnabled = false;
            // 
            // hexagonColorControl1
            // 
            this.hexagonColorControl1.BackColor = System.Drawing.Color.FromArgb(((byte)(255)), ((byte)(251)), ((byte)(255)));
            this.hexagonColorControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hexagonColorControl1.Location = new System.Drawing.Point(0, 0);
            this.hexagonColorControl1.Name = "hexagonColorControl1";
            this.hexagonColorControl1.SelectedColor = System.Drawing.Color.Empty;
            this.hexagonColorControl1.Size = new System.Drawing.Size(261, 278);
            this.hexagonColorControl1.TabIndex = 0;
            this.hexagonColorControl1.Picked += new System.EventHandler(this.HexagonColorControl1_Picked);
            // 
            // Custom
            // 
            this.custom.Controls.Add(this.gradientBar1);
            this.custom.Controls.Add(this.autoLabel7);
            this.custom.Controls.Add(this.numericUpDown3);
            this.custom.Controls.Add(this.numericUpDown2);
            this.custom.Controls.Add(this.numericUpDown1);
            this.custom.Controls.Add(this.comboBox1);
            this.custom.Controls.Add(this.autoLabel6);
            this.custom.Controls.Add(this.autoLabel5);
            this.custom.Controls.Add(this.autoLabel4);
            this.custom.Controls.Add(this.autoLabel3);
            this.custom.Controls.Add(this.gradientColorControl1);
            this.custom.Location = new System.Drawing.Point(1, 32);
            this.custom.Name = "Custom";
            this.custom.Size = new System.Drawing.Size(261, 278);
            this.custom.TabFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.custom.TabIndex = 1;
            this.custom.Text = "Custom";
            this.custom.ThemesEnabled = false;
            // 
            // gradientBar1
            // 
            this.gradientBar1.Location = new System.Drawing.Point(224, 24);
            this.gradientBar1.MaxValue = 255;
            this.gradientBar1.MiddleColor = System.Drawing.Color.Gray;
            this.gradientBar1.Name = "gradientBar1";
            this.gradientBar1.Position = 142;
            this.gradientBar1.Size = new System.Drawing.Size(20, 150);
            this.gradientBar1.TabIndex = 12;
            this.gradientBar1.PositionChanged += new System.EventHandler(this.GradientBar1_PositionChanged);
            // 
            // autoLabel7
            // 
            this.autoLabel7.DX = 0;
            this.autoLabel7.DY = 0;
            this.autoLabel7.Location = new System.Drawing.Point(16, 256);
            this.autoLabel7.Name = "autoLabel7";
            this.autoLabel7.Size = new System.Drawing.Size(33, 16);
            this.autoLabel7.TabIndex = 11;
            this.autoLabel7.Text = "Blue";
            // 
            // numericUpDown3
            // 
            this.numericUpDown3.Location = new System.Drawing.Point(104, 256);
            this.numericUpDown3.Maximum = new decimal(new int[] {
																		   255,
																		   0,
																		   0,
																		   0});
            this.numericUpDown3.Name = "numericUpDown3";
            this.numericUpDown3.Size = new System.Drawing.Size(72, 20);
            this.numericUpDown3.TabIndex = 10;
            this.numericUpDown3.ValueChanged += new System.EventHandler(this.NumericUpDown1_ValueChanged);
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.Location = new System.Drawing.Point(104, 232);
            this.numericUpDown2.Maximum = new decimal (new int[] {
																		   255,
																		   0,
																		   0,
																		   0});
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(72, 20);
            this.numericUpDown2.TabIndex = 9;
            this.numericUpDown2.ValueChanged += new System.EventHandler(this.NumericUpDown1_ValueChanged);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(104, 208);
            this.numericUpDown1.Maximum = new decimal (new int[] {
																		   255,
																		   0,
																		   0,
																		   0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(72, 20);
            this.numericUpDown1.TabIndex = 8;
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.NumericUpDown1_ValueChanged);
            // 
            // comboBox1
            // 
            this.comboBox1.AllowNewText = false;
            this.comboBox1.IgnoreThemeBackground = true;
            this.comboBox1.Items.AddRange(new object[] {
														   "RGB",
														   "HSL"});
            this.comboBox1.Location = new System.Drawing.Point(104, 184);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(72, 21);
            this.comboBox1.SuppressDropDownEvent = false;
            this.comboBox1.TabIndex = 7;
            this.comboBox1.Text = "RGB";
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.ComboBox1_SelectedIndexChanged);
            // 
            // autoLabel6
            // 
            this.autoLabel6.DX = 0;
            this.autoLabel6.DY = 0;
            this.autoLabel6.Location = new System.Drawing.Point(16, 232);
            this.autoLabel6.Name = "autoLabel6";
            this.autoLabel6.Size = new System.Drawing.Size(42, 16);
            this.autoLabel6.TabIndex = 5;
            this.autoLabel6.Text = "Green";
            // 
            // autoLabel5
            // 
            this.autoLabel5.DX = 0;
            this.autoLabel5.DY = 0;
            this.autoLabel5.Location = new System.Drawing.Point(16, 208);
            this.autoLabel5.Name = "autoLabel5";
            this.autoLabel5.Size = new System.Drawing.Size(31, 16);
            this.autoLabel5.TabIndex = 4;
            this.autoLabel5.Text = "Red";
            // 
            // autoLabel4
            // 
            this.autoLabel4.DX = 0;
            this.autoLabel4.DY = 0;
            this.autoLabel4.Location = new System.Drawing.Point(16, 184);
            this.autoLabel4.Name = "autoLabel4";
            this.autoLabel4.Size = new System.Drawing.Size(71, 16);
            this.autoLabel4.TabIndex = 2;
            this.autoLabel4.Text = "Color model";
            // 
            // autoLabel3
            // 
            this.autoLabel3.DX = 0;
            this.autoLabel3.DY = 0;
            this.autoLabel3.Location = new System.Drawing.Point(8, 8);
            this.autoLabel3.Name = "autoLabel3";
            this.autoLabel3.Size = new System.Drawing.Size(43, 16);
            this.autoLabel3.TabIndex = 1;
            this.autoLabel3.Text = "Colors";
            // 
            // gradientColorControl1
            // 
            this.gradientColorControl1.HStep = 7;
            this.gradientColorControl1.Location = new System.Drawing.Point(8, 24);
            this.gradientColorControl1.Name = "gradientColorControl1";
            this.gradientColorControl1.SelectedColor = System.Drawing.Color.Empty;
            this.gradientColorControl1.SelectedPoint = new System.Drawing.Point(0, 0);
            this.gradientColorControl1.Size = new System.Drawing.Size(210, 150);
            this.gradientColorControl1.TabIndex = 0;
            this.gradientColorControl1.VStep = 9;
            this.gradientColorControl1.Picked += new System.EventHandler(this.GradientColorControl1_Picked);
            // 
            // buttonAdv1
            // 
            this.buttonAdv1.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Classic;
            this.buttonAdv1.ComboEditBackColor = System.Drawing.Color.Empty;
            this.buttonAdv1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonAdv1.IsMouseDown = false;
            this.buttonAdv1.Location = new System.Drawing.Point(280, 8);
            this.buttonAdv1.Name = "buttonAdv1";
            this.buttonAdv1.TabIndex = 1;
            this.buttonAdv1.Text = "OK";
            this.buttonAdv1.UseVisualStyle = true;
            this.buttonAdv1.Click += new System.EventHandler(this.ButtonAdv1_Click);
            // 
            // buttonAdv2
            // 
            this.buttonAdv2.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Classic;
            this.buttonAdv2.ComboEditBackColor = System.Drawing.Color.Empty;
            this.buttonAdv2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonAdv2.IsMouseDown = false;
            this.buttonAdv2.Location = new System.Drawing.Point(280, 38);
            this.buttonAdv2.Name = "buttonAdv2";
            this.buttonAdv2.TabIndex = 2;
            this.buttonAdv2.Text = "Cancel";
            this.buttonAdv2.UseVisualStyle = true;
            this.buttonAdv2.Click += new System.EventHandler(this.ButtonAdv2_Click);
            // 
            // autoLabel1
            // 
            this.autoLabel1.BackColor = Color.Transparent;
            this.autoLabel1.DX = 0;
            this.autoLabel1.DY = 0;
            this.autoLabel1.Location = new System.Drawing.Point(304, 240);
            this.autoLabel1.Name = "autoLabel1";
            this.autoLabel1.Size = new System.Drawing.Size(27, 16);
            this.autoLabel1.TabIndex = 3;
            this.autoLabel1.Text = "New";
            // 
            // autoLabel2
            // 
            this.autoLabel2.BackColor = Color.Transparent;
            this.autoLabel2.DX = 0;
            this.autoLabel2.DY = 0;
            this.autoLabel2.Location = new System.Drawing.Point(296, 304);
            this.autoLabel2.Name = "autoLabel2";
            this.autoLabel2.Size = new System.Drawing.Size(42, 16);
            this.autoLabel2.TabIndex = 4;
            this.autoLabel2.Text = "Current";
            // 
            // ColorDlgAdv
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(360, 326);
            this.Controls.Add(this.autoLabel2);
            this.Controls.Add(this.autoLabel1);
            this.Controls.Add(this.buttonAdv2);
            this.Controls.Add(this.buttonAdv1);
            this.Controls.Add(this.tabControl1);
            this.DisableOffice2007Style = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ColorDlgAdv";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = SR.GetString(SR.ColorDialog );
            this.Load += new System.EventHandler(this.ColorDlgAdv_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tabControl1)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.standard.ResumeLayout(false);
            this.custom.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.comboBox1)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private void ButtonAdv1_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void ButtonAdv2_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void HexagonColorControl1_Picked(object sender, EventArgs e)
        {
            m_color = this.hexagonColorControl1.SelectedColor;

            this.Invalidate();
        }

        public struct HSL
        {
            public int H;
            public int S;
            public int L;

            public HSL(int h, int s, int l)
            {
                this.H = h;
                this.S = s;
                this.L = l;
            }
        }

        private ColorDlgAdv.HSL ConvertRGBToHSL(int red, int green, int blue)
        {
            double h = 0, s, l, max, min;

            double r = red / 255.0;
            double g = green / 255.0;
            double b = blue / 255.0;

            max = Math.Max(Math.Max(r, g), b);
            min = Math.Min(Math.Min(r, g), b);

            double delta = max - min;

            l = (max + min) / 2;

            if (l > 0 && l <= 0.5)
                s = delta / (2 * l);
            else
                s = delta / (2 - 2 * l);

            if (delta == 0 || l == 0)
            {
                h = 0;
            }
            else if (max == r)
            {
                if (g >= b)
                    h = (g - b) / delta * 60;
                else
                    h = (g - b) / delta * 60 + 360;
            }
            else if (max == g)
            {
                h = (b - r) / delta * 60 + 120;
            }
            else if (max == b)
            {
                h = (r - g) / delta * 60 + 240;
            }

            return new ColorDlgAdv.HSL((int)(h * 255 / 360), (int)(s * 255), (int)(l * 255));
        }

        private Color ConvertHSLToRGB(int hue, int sat, int lum)
        {
            double h, s, l, temp1, temp2, temp3R, temp3G, temp3B;
            double r, g, b;

            h = hue / 255.0;
            s = sat / 255.0;
            l = lum / 255.0;

            if (s == 0.0)
            {
                r = l;
                g = l;
                b = l;
            }
            else
            {
                if (l < 0.5)
                    temp2 = l * (1.0 + s);
                else
                    temp2 = l + s - (l * s);

                temp1 = 2 * l - temp2;

                temp3R = h + 1.0 / 3;
                temp3G = h;
                temp3B = h - 1.0 / 3;

                r = this.HueToRGB(temp1, temp2, temp3R);
                g = this.HueToRGB(temp1, temp2, temp3G);
                b = this.HueToRGB(temp1, temp2, temp3B);
            }

            return Color.FromArgb((int)(r * 255), (int)(g * 255), (int)(b * 255));
        }

       private double HueToRGB(double n1, double n2, double hue)
        {
            if (hue < 0)
                hue += 1.0;

            if (hue > 1.0)
                hue -= 1.0;

            if (hue < 1.0 / 6)
                return n1 + ((n2 - n1) * 6.0 * hue);
            else if (hue >= 1.0 / 6 && hue < 1.0 / 2)
                return n2;
            else if (hue >= 1.0 / 2 && hue < 2.0 / 3)
                return n1 + ((n2 - n1) * (2.0 / 3 - hue) * 6.0);
            else
                return n1;
        }

        private void GradientColorControl1_Picked(object sender, EventArgs e)
        {
            if (!m_bShouldChangeSelection)
                return;

            this.gradientBar1.MiddleColor = this.gradientColorControl1.SelectedColor;

            Point p = this.gradientColorControl1.SelectedPoint;
            int pos = this.gradientBar1.Position;

            if (p.X > this.gradientColorControl1.Width - increaseMultX)
                p.X = this.gradientColorControl1.Width;

            if (p.Y > this.gradientColorControl1.Height - increaseMultY)
                p.Y = this.gradientColorControl1.Height;

            m_color = this.ConvertHSLToRGB((int)(p.X * increaseMultX), 255 - (int)(p.Y * increaseMultY), this.gradientBar1.MaxValue - (int)(pos * increaseMultLum));

            if (this.comboBox1.SelectedIndex == 0)
            {
                m_bShouldChangeSelection = false;

                this.numericUpDown1.Value = m_color.R;
                this.numericUpDown2.Value = m_color.G;
                this.numericUpDown3.Value = m_color.B;

                m_bShouldChangeSelection = true;
            }
            else
            {
                int xValue = Convert.ToInt32(p.X * increaseMultX);
                int yValue = Convert.ToInt32(255 - p.Y * increaseMultY);
                int lumValue = Convert.ToInt32(this.gradientBar1.MaxValue - pos * increaseMultLum);

                m_bShouldChangeSelection = false;

                this.numericUpDown1.Value = xValue;
                this.numericUpDown2.Value = yValue;
                this.numericUpDown3.Value = lumValue;

                m_bShouldChangeSelection = true;
            }

            this.Invalidate();
        }

        private void ComboBox1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            Point p = this.gradientColorControl1.SelectedPoint;
            int pos = this.gradientBar1.Position;

            if (p.X > this.gradientColorControl1.Width - increaseMultX)
                p.X = this.gradientColorControl1.Width;

            if (p.Y > this.gradientColorControl1.Height - increaseMultY)
                p.Y = this.gradientColorControl1.Height;

            m_color = this.ConvertHSLToRGB((int)(p.X * increaseMultX), 255 - (int)(p.Y * increaseMultY), 255 - (int)(pos * increaseMultLum));

            if (this.comboBox1.SelectedIndex == 0)
            {
                this.autoLabel5.Text = SR.GetString(SR.ColorEditorRedLabel);
                this.autoLabel6.Text = SR.GetString(SR.ColorEditorGreenLabel);
                this.autoLabel7.Text = SR.GetString(SR.ColorEditorBlueLabel);

                m_bShouldChangeSelection = false;

                this.numericUpDown1.Value = m_color.R;
                this.numericUpDown2.Value = m_color.G;
                this.numericUpDown3.Value = m_color.B;

                m_bShouldChangeSelection = true;
            }
            else
            {
                this.autoLabel5.Text = SR.GetString(SR.ColorEditorHueLabel);
                this.autoLabel6.Text = SR.GetString(SR.ColorEditorSatLabel);
                this.autoLabel7.Text = SR.GetString(SR.ColorEditorLumLabel);

                int xValue = Convert.ToInt32(p.X * increaseMultX);
                int yValue = 255 - Convert.ToInt32(p.Y * increaseMultY);

                m_bShouldChangeSelection = false;

                this.numericUpDown1.Value = xValue;
                this.numericUpDown2.Value = yValue;
                this.numericUpDown3.Value = (decimal)(this.gradientBar1.MaxValue - this.gradientBar1.Position * increaseMultLum);

                m_bShouldChangeSelection = true;
            }
        }

        private void NumericUpDown1_ValueChanged(object sender, System.EventArgs e)
        {
            if (!m_bShouldChangeSelection)
                return;

            if (this.comboBox1.SelectedIndex == 0)
            {
                ColorDlgAdv.HSL hsl = this.ConvertRGBToHSL((int)this.numericUpDown1.Value, (int)this.numericUpDown2.Value, (int)this.numericUpDown3.Value);

                Point p = new Point((int)(hsl.H * decreaseMultX), this.gradientBar1.BitmapHeight - (int)(hsl.S * decreaseMultY - 8));
                int pos = (int)((255 - hsl.L) * decreaseMultLum);

                m_color = Color.FromArgb((int)this.numericUpDown1.Value, (int)this.numericUpDown2.Value, (int)this.numericUpDown3.Value);

                this.gradientBar1.MiddleColor = this.gradientColorControl1.SelectedColor;

                m_bShouldChangeSelection = false;

                this.gradientColorControl1.ChangeSelection(p);
                this.gradientBar1.ChangePosition(pos);

                m_bShouldChangeSelection = true;
            }
            else
            {
                int x = (int)((int)this.numericUpDown1.Value * decreaseMultX);
                int y = this.gradientColorControl1.Height - (int)((int)this.numericUpDown2.Value * decreaseMultY);
                int lum = (int)((double)(255 - this.numericUpDown3.Value) * decreaseMultLum);

                this.gradientColorControl1.ChangeSelection(new Point(x, y));
                this.gradientBar1.ChangePosition(lum + 3);
            }

            this.Invalidate();
        }

        private void GradientBar1_PositionChanged(object sender, System.EventArgs e)
        {
            if (!m_bShouldChangeSelection)
                return;

            Point p = this.gradientColorControl1.SelectedPoint;
            int pos = this.gradientBar1.Position;

            if (p.X > this.gradientColorControl1.Width - increaseMultX)
                p.X = this.gradientColorControl1.Width;

            if (p.Y > this.gradientColorControl1.Height - increaseMultY)
                p.Y = this.gradientColorControl1.Height;

            m_color = this.ConvertHSLToRGB((int)(p.X * increaseMultX), 255 - (int)(p.Y * increaseMultY), this.gradientBar1.MaxValue - (int)(pos * increaseMultLum));

            if (this.comboBox1.SelectedIndex == 0)
            {
                m_bShouldChangeSelection = false;

                this.numericUpDown1.Value = m_color.R;
                this.numericUpDown2.Value = m_color.G;
                this.numericUpDown3.Value = m_color.B;

                m_bShouldChangeSelection = true;
            }
            else
            {
                m_bShouldChangeSelection = false;

                this.numericUpDown3.Value = (decimal)((this.gradientBar1.BitmapHeight - pos) * increaseMultLum);

                m_bShouldChangeSelection = true;

                m_color = this.ConvertHSLToRGB((int)this.numericUpDown1.Value, (int)this.numericUpDown2.Value, (int)this.numericUpDown3.Value);
            }

            this.Invalidate();
        }

        private void ColorDlgAdv_Load(object sender, System.EventArgs e)
        {
            this.CalcMultipliers();

            this.tabControl1.SelectedIndex = m_colorPicker.SelectedTabIndex;

            this.gradientBar1.MiddleColor = Color.Red;

            if (m_colorPicker.SelectedItem == null)
                return;

            m_color = m_colorPicker.SelectedItem.Color;

            if (this.tabControl1.SelectedIndex == 0)
            {
                foreach (ColorCell cell in this.hexagonColorControl1.Items)
                    if (m_colorPicker.SelectedItem.Color == cell.Color)
                    {
                        this.hexagonColorControl1.ChangeSelection(cell.Index);
                        break;
                    }
            }
            else
            {
                Color selColor = this.m_colorPicker.SelectedItem.Color;

                if (this.comboBox1.SelectedIndex == 0)
                {
                    this.numericUpDown1.Value = selColor.R;
                    this.numericUpDown2.Value = selColor.G;
                    this.numericUpDown3.Value = selColor.B;
                }
                else
                {
                    ColorDlgAdv.HSL hsl = this.ConvertRGBToHSL(selColor.R, selColor.G, selColor.B);

                    Point p = new Point((int)(hsl.H * decreaseMultX), this.gradientBar1.BitmapHeight - (int)(hsl.S * decreaseMultY - 8));
                    int pos = (int)((255 - hsl.L) * decreaseMultLum);

                    this.gradientColorControl1.ChangeSelection(p);
                    this.gradientBar1.ChangePosition(pos);
                }

                this.gradientBar1.MiddleColor = selColor;
            }
        }

        private void SetOffice2007Style()
        {
            Office2007Theme styleColor = this.m_colorPicker.Office2007Theme;
            Office2007Colors colorTable = Office2007Colors.GetColorTable(styleColor);
            Color formBackColor = colorTable.FormBackground;

            this.DisableOffice2007Style = false;
            this.ColorScheme = styleColor;
            this.UseOffice2007SchemeBackColor = true;

            ApplyOffice2007Style(this, styleColor);

            this.tabControl1.BackColor = formBackColor;
            this.tabControl1.TabPanelBackColor = formBackColor;
        }

        private void ApplyOffice2007Style(Control ctl, Office2007Theme styleColor)
        {
            foreach (Control c in ctl.Controls)
            {
                ISupportOffice2007Theme support = c as ISupportOffice2007Theme;

                if (support != null)
                {
                    support.EnableOffice2007Style();
                    support.Office2007ColorTheme = styleColor;
                }

                Label label = c as Label;

                if (label != null)
                {
                    label.ForeColor = (this.ColorScheme == Office2007Theme.Black) ? Color.White : Color.Black;
                }

                ApplyOffice2007Style(c, styleColor);
            }
        }

        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBox1.SelectedIndex == -1)
                this.comboBox1.SelectedIndex = 0;

            if (this.tabControl1.SelectedIndex == 0 && this.gradientColorControl1.SelectedColor != Color.Empty)
            {
                this.hexagonColorControl1.SelectedCell = null;
                this.hexagonColorControl1.Invalidate();

                Color color = Color.FromArgb((int)this.numericUpDown1.Value, (int)this.numericUpDown2.Value, (int)this.numericUpDown3.Value);

                foreach (ColorCell cell in this.hexagonColorControl1.Items)
                    if (color == cell.Color)
                    {
                        this.hexagonColorControl1.ChangeSelection(cell.Index);
                        break;
                    }
            }
            else if (this.tabControl1.SelectedIndex == 1 &&
                (this.hexagonColorControl1.SelectedCell != null || m_colorPicker.SelectedItem != null ||
                m_color != Color.FromArgb(255, 0, 0, 0)))
            {
                Color selColor = Color.Empty;

                if (this.hexagonColorControl1.SelectedCell != null)
                    selColor = this.hexagonColorControl1.SelectedCell.Color;
                else if (m_color != Color.Empty)
                    selColor = m_color;
                else if (m_colorPicker.SelectedItem != null)
                    selColor = m_colorPicker.SelectedItem.Color;

                ColorDlgAdv.HSL hsl = this.ConvertRGBToHSL(selColor.R, selColor.G, selColor.B);

                Point p = new Point((int)(hsl.H * decreaseMultX), this.gradientBar1.BitmapHeight - (int)(hsl.S * decreaseMultY - 8));
                int pos = (int)((255 - hsl.L) * decreaseMultLum);

                if (this.comboBox1.SelectedIndex == 0)
                {
                    this.numericUpDown1.Value = selColor.R;
                    this.numericUpDown2.Value = selColor.G;
                    this.numericUpDown3.Value = selColor.B;
                }
                else
                {
                    this.gradientColorControl1.ChangeSelection(p);
                    this.gradientBar1.ChangePosition(pos);
                }

                this.gradientBar1.MiddleColor = selColor;
            }

            m_colorPicker.SelectedTabIndex = this.tabControl1.SelectedIndex;
        }

        private double decreaseMultX, decreaseMultY, decreaseMultLum, increaseMultX, increaseMultY, increaseMultLum;

        private void CalcMultipliers()
        {
            Rectangle bounds = this.gradientColorControl1.Bounds;

            decreaseMultX = (double)bounds.Width / 255;
            decreaseMultY = (double)bounds.Height / 255;
            decreaseMultLum = (double)this.gradientBar1.BitmapHeight / 255;

            increaseMultX = 255 / (double)bounds.Width;
            increaseMultY = 255 / (double)bounds.Height;
            increaseMultLum = 255 / (double)this.gradientBar1.BitmapHeight;
        }
    }
}