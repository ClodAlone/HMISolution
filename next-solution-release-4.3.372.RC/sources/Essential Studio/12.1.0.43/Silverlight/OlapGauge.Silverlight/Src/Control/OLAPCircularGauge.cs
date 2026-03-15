//-------------------------------------------------------------------------------------------------
// <copyright file="OlapCircularGauge.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Syncfusion.OlapSilverlight.Data;
using Syncfusion.OlapSilverlight.Engine;
using Syncfusion.OlapSilverlight.Reports;
using System.Windows.Markup;
using Syncfusion.Windows.Gauge;
using Syncfusion.OlapSilverlight.Manager;
using System.Globalization;

namespace Syncfusion.Silverlight.Olap.Gauge
{

    /// <summary>
    /// OlapCircularGauge implementing CircularGauge, helps in applying the default template of 
    /// the Circular Gauge. The various elements of the Gauge appropriately gets its value and
    /// displayed inside gauge. This class is accessed through OlapGauge class. The overriden method 
    /// in this class, helps in Binding the Circular Gauge and its elements with KPI and other usefull
    /// information in Reports.
    /// </summary>

    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
    Type = typeof(OlapCircularGauge), XamlResource = "/Syncfusion.OlapGauge.Silverlight;component/Themes/BlackTheme.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
    Type = typeof(OlapCircularGauge), XamlResource = "/Syncfusion.OlapGauge.Silverlight;component/Themes/BlendTheme.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
    Type = typeof(OlapCircularGauge), XamlResource = "/Syncfusion.OlapGauge.Silverlight;component/Themes/BlueTheme.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
    Type = typeof(OlapCircularGauge), XamlResource = "/Syncfusion.OlapGauge.Silverlight;component/Themes/SilverTheme.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
    Type = typeof(OlapCircularGauge), XamlResource = "/Syncfusion.OlapGauge.Silverlight;component/Themes/Generic.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2003,
    Type = typeof(OlapCircularGauge), XamlResource = "/Syncfusion.OlapGauge.Silverlight;component/Themes/Office2003Theme.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
    Type = typeof(OlapCircularGauge), XamlResource = "/Syncfusion.OlapGauge.Silverlight;component/Themes/MetroTheme.xaml")]

    [TemplatePart(Name = "PART_OlapCircularGauge", Type = typeof(OlapCircularGauge))]
    public class OlapCircularGauge : CircularGauge
    {
        #region Private Members
        private IOlapDataProvider _olapDataManager;
        private CircularGauge m_CircularGauge;
        private GaugeImage statusImage, trendImage;
        #endregion

        #region Dependency Properties
        /// <summary>
        /// ShowMarkersTooltip Dependency Property
        /// </summary>
        public static readonly DependencyProperty ShowMarkersTooltipProperty =
            DependencyProperty.Register("ShowMarkersTooltip", typeof(bool), typeof(OlapCircularGauge), new PropertyMetadata(true));

        /// <summary>
        /// LabelTickForeground Dependency Property
        /// </summary>
        public static readonly DependencyProperty LabelTickForegroundProperty =
           DependencyProperty.Register("LabelTickForeground", typeof(Brush), typeof(OlapCircularGauge), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        /// <summary>
        /// ShowPointersTooltip Dependency Property
        /// </summary>
        public static readonly DependencyProperty ShowPointersTooltipProperty =
            DependencyProperty.Register("ShowPointersTooltip", typeof(bool), typeof(OlapCircularGauge), new PropertyMetadata(true));

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor called when OlapCircular Gauge is needed with given Maximum Gauge Scale value
        /// </summary>
        public OlapCircularGauge(int maxGaugeValue)
        {
            this.MaxValue = maxGaugeValue;
            DefaultStyleKey = typeof(OlapCircularGauge);
        }

        /// <summary>
        /// Constructor called when OlapCircular Gauge is needed with default Maximum Gauge Scale value
        /// </summary>
        public OlapCircularGauge()
        {
            this.MaxValue = 99;
            DefaultStyleKey = typeof(OlapCircularGauge);
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the KPI info which contains the information to render the OlapCircularGauge.
        /// </summary>
        /// <value>The KPI info.</value>
        public KpiInfo KpiInfo { get; set; }

        /// <summary>
        /// Gets or sets MaxValue of the Gauge to which all the values should be converted
        /// to.
        /// </summary>
        /// <remarks>
        /// If you need to render the gauge to 2 digit values then set this property
        /// MaxValue to 99.
        /// </remarks>
        public int MaxValue { get; set; }

        /// <summary>
        /// Gets or sets OlapDataManager which is used to display the Gauge Values.<see cref="OlapDataManager"/>
        /// </summary>
        public IOlapDataProvider OlapDataManager
        {
            get
            {
                return _olapDataManager;
            }

            set
            {
                _olapDataManager = value;
            }
        }

        /// <summary>
        /// Gets and Sets the decision in bool of either showing or hiding Markers tooltip. 
        /// Defaultly is is made to show tooltip by setting it to true.
        /// </summary>
        [DefaultValue(true)]
        public bool ShowMarkersTooltip
        {
            get { return (bool)this.GetValue(ShowMarkersTooltipProperty); }
            set { this.SetValue(ShowMarkersTooltipProperty, value); }
        }

        /// <summary>
        /// Gets and Sets the decision in bool of either showing or hiding Pointer tooltip. 
        /// Defaultly is is made to show tooltip by setting it to true.
        /// </summary>
        [DefaultValue(true)]
        public bool ShowPointersTooltip
        {
            get { return (bool)this.GetValue(ShowPointersTooltipProperty); }
            set { this.SetValue(ShowPointersTooltipProperty, value); }
        }


        /// <summary>
        /// Gets the pointer tool tip value.
        /// </summary>
        /// <value>The pointer tool tip value.</value>
        public string PointerToolTipValue
        {
            get
            {
                if (KpiInfo != null && this.ShowPointersTooltip)
                {
                    string valueText = Syncfusion.Silverlight.Olap.Gauge.Resources.SR.GetString(System.Globalization.CultureInfo.CurrentUICulture, "OlapGauge_ToolTip_Value") + ": ";
                    return this.KpiInfo.Kpi_Name + "\n" + valueText + this.KpiInfo.ActualMeasureValue.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets a Concatenated KPI values to display the Marker ToolTip
        /// </summary>
        public string MarkerToolTipValue
        {
            get
            {
                if (KpiInfo != null && this.ShowMarkersTooltip)
                {
                    string goalText = Syncfusion.Silverlight.Olap.Gauge.Resources.SR.GetString(System.Globalization.CultureInfo.CurrentUICulture, "OlapGauge_ToolTip_Goal") + ": ";
                    return this.KpiInfo.Kpi_Name + "\n" + goalText + this.KpiInfo.ActualGoalValue.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Display of Factor Label is customized
        /// </summary>
        private void ShowFactorLabel(int factor)
        {
            if (factor >= 10)
            {
                m_CircularGauge.CustomLabels[0].Text = "X " + factor.ToString(CultureInfo.CurrentCulture);
                m_CircularGauge.CustomLabels[0].Location = new Point(38, 70);
                if (m_CircularGauge.Radius >= 70)
                {
                    m_CircularGauge.CustomLabels[0].Visibility = Visibility.Visible;
                }
                else
                {
                    m_CircularGauge.CustomLabels[0].Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                m_CircularGauge.CustomLabels[0].Visibility = Visibility.Collapsed;
            }
            m_CircularGauge.CustomLabels[0].FontSize  = m_CircularGauge.Radius / 9;
        }

        /// <summary>
        /// Adding the status image along with customizing its appearance
        /// </summary>
        private void AddStatusImage(string uriPath)
        {
            statusImage.ImageHeight = 20;
            statusImage.ImageWidth = 20;
            statusImage.ImageSource = new BitmapImage(new Uri(uriPath, UriKind.Relative));
            statusImage.Location = new Point(42, (60 + m_CircularGauge.Scales[0].PointerCap.PointerCapRadius + 15));
        }

        /// <summary>
        ///  Adding the trend image along with customizing its appearance
        /// </summary>
        private void AddTrendImage(string uriPath)
        {
            trendImage.ImageWidth = 20;
            trendImage.ImageHeight = 20;
            trendImage.ImageSource = new BitmapImage(new Uri(uriPath, UriKind.RelativeOrAbsolute));
            trendImage.Location = new Point(56 , (60 + m_CircularGauge.Scales[0].PointerCap.PointerCapRadius + 15));

        }

        /// <summary>
        /// Adds the Gauge Images namely Status Indicators and Trend Indicators appropriately.
        /// </summary>
        private void AddStatusIndicator()
        {
            if (this.KpiInfo.StatusGraphic == "Traffic Signals")
            {
                if (this.KpiInfo.StatusValue == -1)
                {
                    AddStatusImage(@"/Syncfusion.OlapGauge.Silverlight;component/Images/KPI/Red.png");
                }
                else if (this.KpiInfo.StatusValue == 0)
                {
                    AddStatusImage(@"/Syncfusion.OlapGauge.Silverlight;component/Images/KPI/ThreeColor.png");
                }
                else if (this.KpiInfo.StatusValue == 1)
                {
                    AddStatusImage(@"/Syncfusion.OlapGauge.Silverlight;component/Images/KPI/Green.png");
                }
            }
            else
            {
                if (this.KpiInfo.StatusValue == -1)
                {
                    AddStatusImage(@"/Syncfusion.OlapGauge.Silverlight;component/Images/KPI/Diamond.png");
                }
                else if (this.KpiInfo.StatusValue == 0)
                {
                    AddStatusImage(@"/Syncfusion.OlapGauge.Silverlight;component/Images/KPI/Triangle.png");
                }
                else if (this.KpiInfo.StatusValue == 1)
                {
                    AddStatusImage(@"/Syncfusion.OlapGauge.Silverlight;component/Images/KPI/Circle.png");
                }
            }

            if (this.KpiInfo.TrendGraphic == "Standard Arrow" || string.IsNullOrEmpty(this.KpiInfo.TrendGraphic))
            {
                if (this.KpiInfo.TrendValue == -1)
                {
                    AddTrendImage(@"/Syncfusion.OlapGauge.Silverlight;component/Images/KPI/DownArrow.png");
                }
                else if (this.KpiInfo.TrendValue == 0)
                {
                    AddTrendImage(@"/Syncfusion.OlapGauge.Silverlight;component/Images/KPI/RightArrow.png");
                }
                else if (this.KpiInfo.TrendValue == 1)
                {
                    AddTrendImage(@"/Syncfusion.OlapGauge.Silverlight;component/Images/KPI/UpArrow.png");
                }
            }
        }


        /// <summary>
        /// Finds the nearest upper limit through appropriate factor conversion
        /// </summary>>
        private static int FindNearestUpperLimit(double number, int factor)
        {
            string convertedNumber = number.ToString(CultureInfo.CurrentCulture);
            string[] s = convertedNumber.Contains(".") ? convertedNumber.Split('.') : convertedNumber.Split(',');
            int j = int.Parse(s[0], CultureInfo.CurrentCulture);
            int k = j % factor;
            return j + (factor - k);
        }

        /// <summary>
        /// The string s contains double value in form of string the factor by which the scales
        /// should be rendered. Does calculations to get the required Factor value.
        /// </summary>
        private int GetCalculatedValue(string s)
        {
            int factor = 1;
            if (s.Contains(".")||s.Contains(","))
            {
                string[] str = s.Contains(".")? s.Split('.'):s.Split(',');
                int len = str[0].Length;
                int dividend = int.Parse(Convert.ToString(str[0],CultureInfo.CurrentCulture),CultureInfo.CurrentCulture);
                for (int i = 0; i < len; i++)
                {
                    if (dividend > MaxValue)
                    {
                        dividend = dividend / 10;
                        factor = factor * 10;
                    }
                    else
                    {
                        break;
                    }
                }

                return factor;
            }
            else
            {
                int dividend = int.Parse(Convert.ToString(s, CultureInfo.CurrentCulture), CultureInfo.CurrentCulture);
                for (int i = 0; i < s.Length; i++)
                {
                    if (dividend > MaxValue)
                    {
                        dividend = dividend / 10;
                        factor = factor * 10;
                    }
                }

                return factor;
            }
        }

        /// <summary>
        /// Sets the max scale value based on either the goal or the measure value.
        /// </summary>
        private int SetMaxScaleValue(double goal, double value)
        {
            int factor;
            if (value > goal)
            {
                factor = GetCalculatedValue(value.ToString(CultureInfo.CurrentCulture));
                this.m_CircularGauge.Scales[0].Maximum = FindNearestUpperLimit((value / factor), 10);
            }
            else
            {
                factor = GetCalculatedValue(goal.ToString(CultureInfo.CurrentCulture));
                this.m_CircularGauge.Scales[0].Maximum = FindNearestUpperLimit((((goal / factor) * 4) / 3), 10);
            }

            return factor;
        }

        #endregion

        #region Applying Template
        /// <summary>
        /// Overrides the Applied Template through generic.xaml
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            m_CircularGauge = GetTemplateChild("PART_OlapCircularGauge") as CircularGauge;
            this.statusImage = GetTemplateChild("img1") as GaugeImage;
            this.trendImage = GetTemplateChild("img2") as GaugeImage;
            if(DesignerProperties.GetIsInDesignMode(this))
                return;
            if (m_CircularGauge != null)
            {
                DataBind();
            }

        }

        #endregion

        #region Implementation
        /// <summary>
        /// Databinding helps in binding the Values in KpiInfo to the OlapCircularGauge and its elements.
        /// </summary>
        public void DataBind()
        {
            if (m_CircularGauge != null)
            {
                EnableEffects = false;

                if (string.IsNullOrEmpty(this.KpiInfo.GoalValue) || string.IsNullOrEmpty(this.KpiInfo.MeasureValue))
                {
                    return;
                }

                m_CircularGauge.Scales[0].Ticks[0].FontSize = m_CircularGauge.Radius / 8;

                if (m_CircularGauge.FrameType == GaugeFrameType.SemiCircular)
                {
                    m_CircularGauge.Scales[0].Ticks[0].FontSize = 9;
                }

                if (m_CircularGauge.FrameType == GaugeFrameType.QuarterCircular)
                {
                    m_CircularGauge.Scales[0].Ticks[0].FontSize = 6;
                }

                //Setting the Radius of the Circular scale
                m_CircularGauge.Scales[0].Radius = (this.Radius - (this.InnerFrameOffset + this.OuterFrameOffset + 10));
                double goal;
                double value;
                //double startValue = 0.0;
                double endValue = 0.0;
                double d = 50 + m_CircularGauge.Scales[0].PointerCap.PointerCapRadius;
                //Setting the location of Factor Label
                m_CircularGauge.CustomLabels[0].Location = new Point(50, d + 5);

                if (string.IsNullOrEmpty(this.KpiInfo.MeasureValue)|| !Double.TryParse(this.KpiInfo.MeasureValue, out value))
                {
                    value = 0.0;
                }

                if (m_CircularGauge.Radius < 70)
                {
                }
               
                if (string.IsNullOrEmpty(this.KpiInfo.GoalValue) || !Double.TryParse(this.KpiInfo.GoalValue, out goal))
                {
                    goal = 0.0;
                }

                int factor;
                //Setting the Factor variable.
                factor = SetMaxScaleValue(goal, value);
                double d_value;
                double mark;
                d_value = value;
                mark = goal / factor;
                double percentage_value = 0.0;
               
                percentage_value = Double.Parse(Convert.ToString((d_value / factor),CultureInfo.CurrentCulture),CultureInfo.CurrentCulture);
                //Setting the attributes of the MemberValue Pointer
                (m_CircularGauge.Scales[0].Pointers[0] as CircularPointer).PointerLength = (this.Radius) - 20;
               // (m_CircularGauge.Scales[0].Pointers[0] as CircularPointer).PointerWidth = 15;
                (m_CircularGauge.Scales[0].Pointers[0] as CircularPointer).Value = endValue + percentage_value;
                //Setting the attributes of the Goal Marker 
                (m_CircularGauge.Scales[0].Pointers[1] as CircularPointer).Value = mark + endValue;
                (m_CircularGauge.Scales[0].Pointers[1] as CircularPointer).PointerLength = this.OuterFrameOffset + this.InnerFrameOffset+2;
                (m_CircularGauge.Scales[0].Pointers[1] as CircularPointer).PointerWidth = this.Radius / 6;
                // (m_CircularGauge.Scales[0].Pointers[1] as CircularPointer).Background = new SolidColorBrush(Colors.Yellow);
                //startValue = endValue;
                endValue = endValue + percentage_value;
                // m_CircularGauge.Scales[0].Ticks[0].l=new SolidColorBrush(Colors.Yellow);
                m_CircularGauge.Scales[0].Ranges[0].EndValue = mark;
                //Displays or hides the Factor label based on the Radius
                ShowFactorLabel(factor);
                //Adds the status indicator based on the KpiInfo Class objects.
                AddStatusIndicator();
            }
        }




        #endregion
    }
}
