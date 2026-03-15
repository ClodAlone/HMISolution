#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Syncfusion.Windows.Shared;
using System.ComponentModel;
using Syncfusion.RDL.DOM;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for ReportPageSetup.xaml
    /// </summary>
#if SyncfusionFramework4_0
        [DesignTimeVisible(false)]
#endif
    internal partial class ReportPageSetup
        : UserControl
    {

        #region private properties

        bool orientationChanged;

        private Dictionary<string, PageSize> paperSizes = null;

        #endregion

        #region Public properties

        public RDL.DOM.ReportUnitType UnitType
        {
            get;
            set;
        }

        public RadioButton PageUnitInches
        {
            get
            {
                return this.rbtn_PageUnitInches;
            }
            set
            {
                this.rbtn_PageUnitInches = value;
            }
        }

        public RadioButton PortraitOrientation
        {
            get
            {
                return this.rbtn_Portrait;
            }
            set
            {
                this.rbtn_Portrait = value;
            }
        }

        public RadioButton LandscapeOrientation
        {
            get
            {
                return this.rbtn_Landscape;
            }
            set
            {
                this.rbtn_Landscape = value;
            }
        }

        public ComboBox PaperSize
        {
            get
            {
                return this.cmb_PaperSize;
            }
            set
            {
                this.cmb_PaperSize = value;
            }
        }

        public new UpDown Width
        {
            get
            {
                return this.updwn_Width;
            }
            set
            {
                this.updwn_Width = value;
            }
        }

        public new UpDown Height
        {
            get
            {
                return this.updwn_Height;
            }
            set
            {
                this.updwn_Height = value;
            }
        }

        public TextBlock WidthUnit
        {
            get
            {
                return this.txt_WidthUnit;
            }
            set
            {
                this.txt_WidthUnit = value;
            }
        }

        public TextBlock HeightUnit
        {
            get
            {
                return this.txt_HeightUnit;
            }
            set
            {
                this.txt_HeightUnit = value;
            }
        }

        public UpDown Bottom
        {
            get
            {
                return this.updwn_Bottom;
            }
            set
            {
                this.updwn_Bottom = value;
            }
        }

        public UpDown Top
        {
            get
            {
                return this.updwn_Top;
            }
            set
            {
                this.updwn_Top = value;
            }
        }

        public UpDown Right
        {
            get
            {
                return this.updwn_Right;
            }
            set
            {
                this.updwn_Right = value;
            }
        }

        public UpDown Left
        {
            get
            {
                return this.updwn_Left;
            }
            set
            {
                this.updwn_Left = value;
            }
        }

        public TextBlock BottomUnit
        {
            get
            {
                return this.txt_BottomUnit;
            }
            set
            {
                this.txt_BottomUnit = value;
            }
        }

        public TextBlock TopUnit
        {
            get
            {
                return this.txt_TopUnit;
            }
            set
            {
                this.txt_TopUnit = value;
            }
        }

        public TextBlock RightUnit
        {
            get
            {
                return this.txt_RightUnit;
            }
            set
            {
                this.txt_RightUnit = value;
            }
        }

        public TextBlock LeftUnit
        {
            get
            {
                return this.txt_LeftUnit;
            }
            set
            {
                this.txt_LeftUnit = value;
            }
        }
        #endregion

        #region constructor
        public ReportPageSetup()
        {
            InitializeComponent();
        }

        public ReportPageSetup(RDL.DOM.Page page, RDL.DOM.ReportUnitType type)
        {
            InitializeComponent();
            this.UnitType = type;
            if (type == RDL.DOM.ReportUnitType.Cm)
            {
                this.rbtn_PageUnitCm.IsChecked = true;
            }
            this.rbtn_PageUnitCm.Checked+=rbtn_PageUnitCm_Checked;
            this.rbtn_PageUnitInches.Checked += rbtn_PageUnitCm_Checked;
            this.rbtn_Landscape.GotFocus += new RoutedEventHandler(OrientationFocused);
            this.rbtn_Portrait.GotFocus += new RoutedEventHandler(OrientationFocused);
            this.rbtn_Landscape.Checked += new RoutedEventHandler(OrientationChanged);
            this.rbtn_Portrait.Checked += new RoutedEventHandler(OrientationChanged);
            this.updwn_Width.LostFocus += new RoutedEventHandler(ReportSizeChanged);
            this.updwn_Height.LostFocus += new RoutedEventHandler(ReportSizeChanged);
           
            this.updwn_Top.MinValue = 0;
            this.updwn_Bottom.MinValue = 0;
            this.updwn_Left.MinValue = 0;
            this.updwn_Right.MinValue = 0;

            if (page.PageWidth != null && page.PageHeight != null)
            {
                this.rbtn_Landscape.IsChecked = page.PageWidth.FloatValue > page.PageHeight.FloatValue;
                this.rbtn_Portrait.IsChecked = !this.rbtn_Landscape.IsChecked;
            }
            if (page.PageWidth != null)
            {
                this.updwn_Width.Value = page.PageWidth.FloatValue;
                this.WidthUnit.Text = page.PageWidth.MeasurementUnit.ToString();
            }
            if (page.PageHeight != null)
            {
                this.updwn_Height.Value = page.PageHeight.FloatValue;
                this.HeightUnit.Text = page.PageHeight.MeasurementUnit.ToString();
            }
            if (page.BottomMargin != null)
            {
                this.updwn_Bottom.Value = page.BottomMargin.FloatValue;
                this.BottomUnit.Text = page.BottomMargin.MeasurementUnit.ToString();
            }
            if (page.TopMargin != null)
            {
                this.updwn_Top.Value = page.TopMargin.FloatValue;
                this.TopUnit.Text = page.TopMargin.MeasurementUnit.ToString();
            }
            if (page.LeftMargin != null)
            {
                this.updwn_Left.Value = page.LeftMargin.FloatValue;
                this.LeftUnit.Text = page.LeftMargin.MeasurementUnit.ToString();
            }
            if (page.RightMargin != null)
            {
                this.updwn_Right.Value = page.RightMargin.FloatValue;
                this.RightUnit.Text = page.RightMargin.MeasurementUnit.ToString();
            }

            UpdatePageDimention();
            UpdatePaperTypeValues();
        }

        private void UpdatePaperTypeValues()
        {
            double? width = 0; double? height = 0;
            if (this.rbtn_Landscape.IsChecked == true)
            {
                width = this.ConvertValue(this.Height.Value, this.txt_HeightUnit.Text, ReportUnitType.In);
                height = this.ConvertValue(this.Width.Value, this.txt_WidthUnit.Text, ReportUnitType.In);
            }
            else
            {
                width = this.ConvertValue(this.Width.Value, this.txt_WidthUnit.Text, ReportUnitType.In);
                height = this.ConvertValue(this.Height.Value, this.txt_HeightUnit.Text, ReportUnitType.In);
            }
            
            width = Math.Round((double)width, 2);
            height = Math.Round((double)height, 2);
            var papertype = (from paper in this.paperSizes where paper.Value != null && paper.Value.Height == height && paper.Value.Width == width select paper.Key).FirstOrDefault();

            if (papertype != null)
            {
                this.cmb_PaperSize.SelectedValue = papertype;
            }
            else
            {
                this.cmb_PaperSize.Text = "custom";
            }
        }

        void OrientationFocused(object sender, RoutedEventArgs e)
        {
            this.orientationChanged = true;
        }

        void ReportSizeChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                this.orientationChanged = false;
                this.rbtn_Landscape.IsChecked = this.updwn_Width.Value > this.updwn_Height.Value;
                this.rbtn_Portrait.IsChecked = !this.rbtn_Landscape.IsChecked;
                UpdatePaperTypeValues();
            }
            catch { }
        }

        void OrientationChanged(object sender, RoutedEventArgs e)
        {
            if (this.orientationChanged)
            {
                var temp = this.updwn_Height.Value;
                this.updwn_Height.Value = this.updwn_Width.Value;
                this.updwn_Width.Value = temp;
                string txt1 = this.txt_HeightUnit.Text;
                this.txt_HeightUnit.Text = this.txt_WidthUnit.Text;
                this.txt_WidthUnit.Text = txt1;
                string txt2;

                if (rbtn_Landscape.IsChecked == true)
                {
                    var temp1 = this.updwn_Left.Value;
                    this.updwn_Left.Value = this.updwn_Top.Value;
                    var temp2 = this.updwn_Bottom.Value;
                    this.updwn_Bottom.Value = temp1;
                    temp1 = this.updwn_Right.Value;
                    this.updwn_Right.Value = temp2;
                    this.updwn_Top.Value = temp1;

                    txt1 = txt_LeftUnit.Text;
                    this.txt_LeftUnit.Text = this.txt_TopUnit.Text;
                    txt2 = this.txt_BottomUnit.Text;
                    this.txt_BottomUnit.Text = txt1;
                    txt1 = this.txt_RightUnit.Text;
                    this.txt_RightUnit.Text = txt2;
                    this.txt_TopUnit.Text = txt1;
                }
                else
                {
                    var temp1 = this.updwn_Right.Value;
                    this.updwn_Right.Value = this.updwn_Top.Value;
                    var temp2 = this.updwn_Bottom.Value;
                    this.updwn_Bottom.Value = temp1;
                    temp1 = this.updwn_Left.Value;
                    this.updwn_Left.Value = temp2;
                    this.updwn_Top.Value = temp1;

                    txt1 = txt_RightUnit.Text;
                    this.txt_RightUnit.Text = this.txt_TopUnit.Text;
                    txt2 = this.txt_BottomUnit.Text;
                    this.txt_BottomUnit.Text = txt1;
                    txt1 = this.txt_LeftUnit.Text;
                    this.txt_LeftUnit.Text = txt2;
                    this.txt_TopUnit.Text = txt1;
                }
            }
        }

        #endregion

        public double SetUnitType()
        {
            this.Left.Value = this.ConvertValue(this.Left.Value, this.txt_LeftUnit.Text, this.UnitType);
            this.Top.Value = this.ConvertValue(this.Top.Value, this.txt_TopUnit.Text, this.UnitType);
            this.Right.Value = this.ConvertValue(this.Right.Value, this.txt_RightUnit.Text, this.UnitType);
            this.Bottom.Value = this.ConvertValue(this.Bottom.Value, this.txt_BottomUnit.Text, this.UnitType);
            this.Width.Value = this.ConvertValue(this.Width.Value, this.txt_WidthUnit.Text, this.UnitType);
            this.Height.Value = this.ConvertValue(this.Height.Value, this.txt_HeightUnit.Text, this.UnitType);

            if (this.UnitType == RDL.DOM.ReportUnitType.In)
            {
                this.LeftUnit.Text = this.TopUnit.Text = this.RightUnit.Text = this.BottomUnit.Text = 
                    this.WidthUnit.Text = this.HeightUnit.Text = "in";
            }

            else
            {
                this.LeftUnit.Text = this.TopUnit.Text = this.RightUnit.Text = this.BottomUnit.Text = 
                    this.WidthUnit.Text = this.HeightUnit.Text = "cm";
            }

            return 0;
        }

        private double? ConvertValue(double? value, string unit, RDL.DOM.ReportUnitType unitType)
        {
            if (value != null)
            {
                if (string.Equals(unit, MeasurementUnits.In.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    return unitType == RDL.DOM.ReportUnitType.In ? value : value * 2.540;
                }
                else if (string.Equals(unit, MeasurementUnits.Cm.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    return unitType == RDL.DOM.ReportUnitType.In ? value * 0.3937 : value;
                }
                else if (string.Equals(unit, MeasurementUnits.Pt.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    return unitType == RDL.DOM.ReportUnitType.In ? value * 0.0139 : value * 0.0353;
                }
                else if (string.Equals(unit, MeasurementUnits.Mm.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    return unitType == RDL.DOM.ReportUnitType.In ? value * 0.03937 : value * 0.1;
                }
                else if (string.Equals(unit, MeasurementUnits.Pc.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    return unitType == RDL.DOM.ReportUnitType.In ? value * 0.01042 : value * 0.02645;
                }
            }
            return value;
        }

        private void rbtn_PageUnitCm_Checked(object sender, RoutedEventArgs e)
        {
            if ((sender as RadioButton).Name == "rbtn_PageUnitInches")
            {
                this.UnitType = RDL.DOM.ReportUnitType.In;
            }
            else
            {
                this.UnitType = RDL.DOM.ReportUnitType.Cm;
            }

            SetUnitType();
        }

        private void UpdatePageDimention()
        {
            paperSizes = new Dictionary<string, PageSize>();
            paperSizes.Add("A3", new PageSize(11.7, 16.5));
            paperSizes.Add("A4", new PageSize(8.27, 11.69));
            paperSizes.Add("B4(JIS)", new PageSize(10.12, 14.33));
            paperSizes.Add("B5(JIS)", new PageSize(7.17, 10.12));
            paperSizes.Add("Envelope #10", new PageSize(4.125, 9.5));
            paperSizes.Add("Envelope Monarch", new PageSize(3.875, 7.5));
            paperSizes.Add("Executive", new PageSize(7.25, 10.5));
            paperSizes.Add("Legal", new PageSize(8.5, 14));
            paperSizes.Add("Letter", new PageSize(8.5, 11));
            paperSizes.Add("Tabloid", new PageSize(11, 17));
            paperSizes.Add("Postcard", new PageSize(4.5, 6));
            paperSizes.Add("A5", new PageSize(5.83, 8.27));
            paperSizes.Add("5X7", new PageSize(5, 7));
            paperSizes.Add("215X330", new PageSize(8.5, 13));
            paperSizes.Add("9X11", new PageSize(9, 11));
            paperSizes.Add("8X10", new PageSize(8, 10));

            foreach (var paper in paperSizes)
            {
                this.cmb_PaperSize.Items.Add(paper.Key);
            }
            this.cmb_PaperSize.Items.Add("custom");
        }

        private void PaperSizeChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var selected = (sender as ComboBox).SelectedValue;
                if (selected != null && selected.ToString().ToLower() != "custom")
                {
                    var paperValue = this.paperSizes[selected.ToString()];
                    if (this.rbtn_Landscape.IsChecked == true)
                    {
                        this.Width.Value = this.ConvertValue(paperValue.Height, "in", this.UnitType);
                        this.Height.Value = this.ConvertValue(paperValue.Width, "in", this.UnitType);
                    }
                    else
                    {
                        this.Width.Value = this.ConvertValue(paperValue.Width, "in", this.UnitType);
                        this.Height.Value = this.ConvertValue(paperValue.Height, "in", this.UnitType);
                    }
                }
            }
            catch { }
        }
    }

    class PageSize
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PageSize"/> class.
        /// </summary>
        /// <param name="width">The x.</param>
        /// <param name="height">The y.</param>
        public PageSize(double width, double height)
        {
            this.Width = width;
            this.Height = height;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public double Width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public double Height { get; set; }

        #endregion
    }
}
