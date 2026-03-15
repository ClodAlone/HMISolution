#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion


namespace Syncfusion.Windows.Chart.Olap
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Media;
    using Syncfusion.Olap.Reports;
    using Syncfusion.Windows.Chart;
    using Syncfusion.Windows.Shared;
    /// <summary>
    /// Interaction logic for Appearance.xaml
    /// </summary>
    /// 
#if SyncfusionFramework4_0
    [DesignTimeVisible(false)]
#endif
    public partial class Appearance : ChromelessWindow
    {
        #region Members
        private ChartAppearanceSettings _ChartAppearance;
        SolidColorBrush _solidBrush=null;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Appearance"/> class.
        /// </summary>
        /// <param name="m_chartAppearance">The m_chart appearance.</param>
        public Appearance(ChartAppearanceSettings m_chartAppearance)
        {
            InitializeComponent();
            this.ChartAppearance = m_chartAppearance;
            this.combo_legendposition.ItemsSource = Enum.GetValues(typeof(ChartDock));

            //Appearance tab
            double[] borderwidth = new double[6] { 0, 1, 2, 3, 4, 5 };
            for (int i = 0; i < 5; i++)
                this.cmb_borderwidth.Items.Add(borderwidth[i]);

            //Axis Labels

            cmb_XFontStyle.Items.Add("Bold");
            cmb_XFontStyle.Items.Add("Normal");
            cmb_YFontStyle.Items.Add("Bold");
            cmb_YFontStyle.Items.Add("Normal");
            LoadControls();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the chart appearance.
        /// </summary>
        /// <value>The chart appearance.</value>
        public ChartAppearanceSettings ChartAppearance
        {
            get
            {
                return _ChartAppearance;
            }
            set
            {
                _ChartAppearance = value;
            }
        }
        #endregion

        #region Events
        private void CancelKBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void chkboxSymbol_Checked(object sender, RoutedEventArgs e)
        {
            rbtn_label5.IsEnabled = true;
           // rbtn_label5.IsChecked = true;
            rbtn_label6.IsEnabled = true;
            rbtn_label7.IsEnabled = true;
        }

        private void chkboxSymbol_Unchecked(object sender, RoutedEventArgs e)
        {
            rbtn_label5.IsEnabled = false;
            rbtn_label6.IsEnabled = false;
            rbtn_label7.IsEnabled = false;

            rbtn_label5.IsChecked = false;
            rbtn_label6.IsChecked = false;
            rbtn_label7.IsChecked = false;
        }

        private void chkLabelText_Checked(object sender, RoutedEventArgs e)
        {
            rbtn_label1.IsEnabled = true;
           // rbtn_label1.IsChecked = true;
            rbtn_label2.IsEnabled = true;
            rbtn_label3.IsEnabled = true;
        }

        private void chkLabelText_Unchecked(object sender, RoutedEventArgs e)
        {
            rbtn_label1.IsEnabled = false;
            rbtn_label2.IsEnabled = false;
            rbtn_label3.IsEnabled = false;

            rbtn_label1.IsChecked = false;
            rbtn_label2.IsChecked = false;
            rbtn_label3.IsChecked = false;
        }

        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadDatasFromControls();
            this.DialogResult = true;
            this.Close();

        }
        #endregion

        #region Loading Controls

        private int GetSelectedIndex(string ItemType,string ItemValue)
        {
            if (ItemType == "SeriesType")
            {
                for (int i = 0; i < combo_ChartType.Items.Count; i++)
                {
                    if (((ImageData)combo_ChartType.Items[i]).Text == ItemValue)
                    {
                        return i;
                    }
                }
            }
            else if (ItemType == "ChartPalette")
            {
                for (int i = 0; i < combo_ChartPalette.Items.Count; i++)
                {
                    if (((ImageData)combo_ChartPalette.Items[i]).Text == ItemValue)
                    {
                        return i;
                    }
                }
            }
            return 0;
        }
        private void LoadControls()
        {
            //Load controls in Chart tab
            foreach (ChartTypes chartType in Enum.GetValues(typeof(ChartTypes)))
            {
                if (chartType.ToString() == this.ChartAppearance.ChartType.ToString())
                {
                    combo_ChartType.SelectedIndex = GetSelectedIndex("SeriesType",chartType.ToString());
                }
            }
            foreach (ChartColorPalette colorPalette in Enum.GetValues(typeof(ChartColorPalette)))
            {
                if (colorPalette.ToString() == this.ChartAppearance.ChartColorPalette.ToString())
                    combo_ChartPalette.SelectedIndex = GetSelectedIndex("ChartPalette", colorPalette.ToString());
            }

            if (this.ChartAppearance.LegendVisibility == true)
                ChkBox_legendVisible.IsChecked = true;
            else
                ChkBox_legendVisible.IsChecked = false;

            if (this.ChartAppearance.LegendCheckBoxVisibility == true)
                chkBox_legendchkBox.IsChecked = true;
            else
                chkBox_legendchkBox.IsChecked = false;

            foreach (ChartDock chartDock in combo_legendposition.Items)
            {
                if (chartDock.ToString() == this.ChartAppearance.ChartDockLegendPosition.ToString())
                    combo_legendposition.SelectedItem = chartDock;
            }

            //Load controls in Appearance tab
            foreach (double borderWidth in cmb_borderwidth.Items)
            {
                if (borderWidth.ToString() == this.ChartAppearance.StrokeThickness.ToString())
                    cmb_borderwidth.SelectedItem = borderWidth;
            }

            System.Drawing.Color borderColor = this.ChartAppearance.BorderColor;
            borderColorPicker.Color = (Color)Color.FromArgb(borderColor.A, borderColor.R, borderColor.G, borderColor.B);

            System.Drawing.Color chartBackground = this.ChartAppearance.ChartBackground;
            ChartColorPicker1.Color = (Color)Color.FromArgb(chartBackground.A, chartBackground.R, chartBackground.G, chartBackground.B);

            System.Drawing.Color interiorBackground = this.ChartAppearance.InteriorBackground;
            InteriorColorPicker1.Color = (Color)Color.FromArgb(interiorBackground.A, interiorBackground.R, interiorBackground.G, interiorBackground.B);

            //Load Labels tab
            this.rbtn_label1.IsChecked = this.ChartAppearance.IsXValues;
            this.rbtn_label2.IsChecked = this.ChartAppearance.IsYValues;
            this.rbtn_label3.IsChecked = this.ChartAppearance.IsSeriesName;
            this.chkLabelText.IsChecked = this.ChartAppearance.LabelsVisibility;

            if (this.chkLabelText.IsChecked == false)
            {
                rbtn_label1.IsEnabled = false;
                rbtn_label2.IsEnabled = false;
                rbtn_label3.IsEnabled = false;
            }
            else
            {
                rbtn_label1.IsEnabled = true;
                rbtn_label2.IsEnabled = true;
                rbtn_label3.IsEnabled = true;
            }

            this.rbtn_label5.IsChecked = this.ChartAppearance.IsCircleSymbol;
            this.rbtn_label6.IsChecked = this.ChartAppearance.IsRectangleSymbol;
            this.rbtn_label7.IsChecked = this.ChartAppearance.IsTriangleSymbol;
            this.chkboxSymbol.IsChecked = this.ChartAppearance.SymbolsVisibility;

            if (this.chkboxSymbol.IsChecked == false)
            {
                rbtn_label5.IsEnabled = false;
                rbtn_label6.IsEnabled = false;
                rbtn_label7.IsEnabled = false;
            }
            else
            {
                rbtn_label5.IsEnabled = true;
                rbtn_label6.IsEnabled = true;
                rbtn_label7.IsEnabled = true;
            }

          

            //Load controls in Axis Labels
            FontBox1.SelectedFontFamily = new FontFamily(this.ChartAppearance.XAxisFontFace);
            FontBox2.SelectedFontFamily = new FontFamily(this.ChartAppearance.YAxisFontFace);

            System.Drawing.Color xAxisForeGround = this.ChartAppearance.XAxisForeGround;
            labelColorPicker1.Color = (Color)Color.FromArgb(xAxisForeGround.A, xAxisForeGround.R, xAxisForeGround.G, xAxisForeGround.B);


            System.Drawing.Color yAxisForeGround = this.ChartAppearance.YAxisForeGround;
            labelColorPicker2.Color = (Color)Color.FromArgb(yAxisForeGround.A, yAxisForeGround.R, yAxisForeGround.G, yAxisForeGround.B);

            foreach (string fontweight in this.cmb_XFontStyle.Items)
            {
                if (fontweight == this.ChartAppearance.XLabelFontWeight.ToString())
                    cmb_XFontStyle.SelectedItem = fontweight;

            }

            foreach (string fontweight in this.cmb_YFontStyle.Items)
            {
                if (fontweight == this.ChartAppearance.YLabelFontWeight.ToString())
                    cmb_YFontStyle.SelectedItem = fontweight;
            }
        }
        #endregion

        #region LoadDatasFromControls
        private void LoadDatasFromControls()
        {
            //Load controls in Chart tab
            foreach (ChartTypes chartType in Enum.GetValues(typeof(ChartTypes)))
            {
                if (chartType.ToString() == ((ImageData)this.combo_ChartType.SelectedItem).Text)
                    this.ChartAppearance.ChartType = chartType.ToString();
            }
            foreach (ChartColorPalette colorPalette in Enum.GetValues(typeof(ChartColorPalette)))
            {
                if (colorPalette.ToString() == ((ImageData)this.combo_ChartPalette.SelectedItem).Text)
                    this.ChartAppearance.ChartColorPalette = colorPalette.ToString();
            }
            if (ChkBox_legendVisible.IsChecked == true)
                this.ChartAppearance.LegendVisibility = true;
            else
                this.ChartAppearance.LegendVisibility = false;

            if (chkBox_legendchkBox.IsChecked == true)
                this.ChartAppearance.LegendCheckBoxVisibility = true;
            else
                this.ChartAppearance.LegendCheckBoxVisibility = false;

            this.ChartAppearance.ChartDockLegendPosition = combo_legendposition.SelectedItem.ToString();

            //Appearance tab

            this.ChartAppearance.StrokeThickness = (double)cmb_borderwidth.SelectedItem;
            this.ChartAppearance.BorderColor = (System.Drawing.Color)System.Drawing.Color.FromArgb(borderColorPicker.Color.A, borderColorPicker.Color.R, borderColorPicker.Color.G, borderColorPicker.Color.B);
            this.ChartAppearance.InteriorBackground = (System.Drawing.Color)System.Drawing.Color.FromArgb(InteriorColorPicker1.Color.A, InteriorColorPicker1.Color.R, InteriorColorPicker1.Color.G, InteriorColorPicker1.Color.B);
            this.ChartAppearance.ChartBackground = (System.Drawing.Color)System.Drawing.Color.FromArgb(ChartColorPicker1.Color.A, ChartColorPicker1.Color.R, ChartColorPicker1.Color.G, ChartColorPicker1.Color.B);

            //Labels tab
            if (this.chkLabelText.IsChecked == true)
            {
                rbtn_label1.IsEnabled = true;
                rbtn_label2.IsEnabled = true;
                rbtn_label3.IsEnabled = true;

                this.ChartAppearance.LabelsVisibility = true;

                if (rbtn_label1.IsChecked == true)
                    this.ChartAppearance.IsXValues = true;
                else
                    this.ChartAppearance.IsXValues = false;
                if (rbtn_label2.IsChecked == true)
                    this.ChartAppearance.IsYValues = true;
                else
                    this.ChartAppearance.IsYValues = false;
                if (rbtn_label3.IsChecked == true)
                    this.ChartAppearance.IsSeriesName = true;
                else
                    this.ChartAppearance.IsSeriesName = false;
            }
            else
            {
                this.ChartAppearance.LabelsVisibility = false;
                rbtn_label1.IsEnabled = false;
                rbtn_label2.IsEnabled = false;
                rbtn_label3.IsEnabled = false;
            }

            if (chkboxSymbol.IsChecked == true)
            {
                this.ChartAppearance.SymbolsVisibility = true;
                rbtn_label5.IsEnabled = true;
                rbtn_label7.IsEnabled = true;
                rbtn_label7.IsEnabled = true;

                if (rbtn_label5.IsChecked == true)
                    this.ChartAppearance.IsCircleSymbol = true;
                else
                    this.ChartAppearance.IsCircleSymbol = false;
                if (rbtn_label6.IsChecked == true)
                    this.ChartAppearance.IsRectangleSymbol = true;
                else
                    this.ChartAppearance.IsRectangleSymbol = false;
                if (rbtn_label7.IsChecked == true)
                    this.ChartAppearance.IsTriangleSymbol = true;
                else
                    this.ChartAppearance.IsTriangleSymbol = false;
            }
            else
            {
                this.ChartAppearance.SymbolsVisibility = false;
                rbtn_label5.IsEnabled = false;
                rbtn_label7.IsEnabled = false;
                rbtn_label7.IsEnabled = false;
            }

          

            //Axis Tab
            this.ChartAppearance.XAxisFontFace = FontBox1.SelectedFontFamily.ToString();
            this.ChartAppearance.YAxisFontFace = FontBox2.SelectedFontFamily.ToString();
            this.ChartAppearance.XAxisForeGround = System.Drawing.Color.FromArgb(labelColorPicker1.Color.A, labelColorPicker1.Color.R, labelColorPicker1.Color.G, labelColorPicker1.Color.B);
            this.ChartAppearance.YAxisForeGround = System.Drawing.Color.FromArgb(labelColorPicker2.Color.A, labelColorPicker2.Color.R, labelColorPicker2.Color.G, labelColorPicker2.Color.B);
            this.ChartAppearance.XLabelFontWeight = cmb_XFontStyle.SelectedItem.ToString();
            this.ChartAppearance.YLabelFontWeight = cmb_YFontStyle.SelectedItem.ToString();
        }
        #endregion

    }
}
