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
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Syncfusion.Olap.Engine;
using Syncfusion.Windows.Gauge.Olap.Resources;
//using Syncfusion.Windows.Gauge.Olap.Control;

namespace Syncfusion.Windows.Gauge.Olap
{
    /// <summary>
    /// Represent OLAP Gauge Control, derived from CircularGauge <see cref="CircularGauge"/>
    /// </summary>
    
#if SyncfusionFramework4_0
    [DesignTimeVisible(false)]
#endif
    public class OlapCircularGauge : CircularGauge
    {
        #region Private Members

        private CircularScale m_seriesScale;
        private CircularCustomLabel m_customMemberLabel, m_customFactorLabel;
        private PointerCap m_pointercap;
        private GaugeImage m_statusImage, m_trendImage, gaugeIcon;
        private CircularGauge m_circularGauge;
        public GaugeImage gaugeImage;

        #endregion

        #region Dependency Properties

        /// <summary>
        /// ShowMarkersTooltip Dependency Property
        /// </summary>
        public static readonly DependencyProperty ShowMarkersTooltipProperty =
            DependencyProperty.Register("ShowMarkersTooltip", typeof (bool), typeof (OlapCircularGauge),
                                        new UIPropertyMetadata());

        /// <summary>
        /// ShowPointersTooltip Dependency Property
        /// </summary>
        public static readonly DependencyProperty ShowPointersTooltipProperty =
            DependencyProperty.Register("ShowPointersTooltip", typeof (bool), typeof (OlapCircularGauge),
                                        new UIPropertyMetadata());

        /// <summary>
        /// ShowGaugeLabels Dependency Property
        /// </summary>
        public static readonly DependencyProperty ShowGaugeLabelsProperty =
            DependencyProperty.Register("ShowGaugeLabels", typeof (bool), typeof (OlapCircularGauge),
                                        new UIPropertyMetadata(true));

        /// <summary>
        /// ShowGaugeFactors Dependency Property
        /// </summary>
        public static readonly DependencyProperty ShowGaugeFactorsProperty =
            DependencyProperty.Register("ShowGaugeFactors", typeof (bool), typeof (OlapCircularGauge),
                                        new UIPropertyMetadata(true));

        #endregion

        #region Constructor

        static OlapCircularGauge()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (OlapCircularGauge),
                                                     new FrameworkPropertyMetadata(typeof (OlapCircularGauge)));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the KPI info which contains the information to render the OlapGauge.
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
        /// Gets or sets a value indicating whether [show markers tooltip].
        /// </summary>
        /// <value><c>true</c> if [show markers tooltip]; otherwise, <c>false</c>.</value>
        [DefaultValue(true)]
        public bool ShowMarkersTooltip
        {
            get { return (bool) GetValue(ShowMarkersTooltipProperty); }
            set { SetValue(ShowMarkersTooltipProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show pointers tooltip].
        /// </summary>
        /// <value><c>true</c> if [show pointers tooltip]; otherwise, <c>false</c>.</value>
        [DefaultValue(true)]
        public bool ShowPointersTooltip
        {
            get { return (bool) GetValue(ShowPointersTooltipProperty); }
            set { SetValue(ShowPointersTooltipProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show gauge labels].
        /// </summary>
        /// <value><c>true</c> if [show gauge labels]; otherwise, <c>false</c>.</value>
        [DefaultValue(true)]
        public bool ShowGaugeLabels
        {
            get { return (bool) GetValue(ShowGaugeLabelsProperty); }
            set { SetValue(ShowGaugeLabelsProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show gauge factors].
        /// </summary>
        /// <value><c>true</c> if [show gauge factors]; otherwise, <c>false</c>.</value>
        [DefaultValue(true)]
        public bool ShowGaugeFactors
        {
            get { return (bool) GetValue(ShowGaugeFactorsProperty); }
            set { SetValue(ShowGaugeFactorsProperty, value); }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Adds the danger range according to the Goal Value (ie., Lesser than Goal Value is considered to be in danger).
        /// </summary>
        /// <param name="startValue">The starting value of the Danger range</param>
        /// <param name="mark">The mark is the Goal value(ie., The end value of Danger Range)</param>
        private void AddDangerRange(double startValue, double mark)
        {
            m_seriesScale.Ranges[0].BackgroundBrush = Brushes.OrangeRed;
            m_seriesScale.Ranges[0].BorderBrush = Brushes.OrangeRed;
            m_seriesScale.Ranges[0].StartValue = startValue;
            m_seriesScale.Ranges[0].EndValue = mark;
            m_seriesScale.Ranges[0].BorderWidth = (Radius/100);
        }

        /// <summary>
        /// Adds the safer range according to the Goal Value (ie., Greater than Goal Value is considered to be safer).
        /// </summary>
        /// <param name="mark">The mark is the Goal value</param>
        private void AddSaferRange(double mark)
        {
            m_seriesScale.Ranges[1].BorderWidth = (Radius/100);
            m_seriesScale.Ranges[1].BackgroundBrush = Brushes.LightGreen;
            m_seriesScale.Ranges[1].BorderBrush = Brushes.LightGreen;
            m_seriesScale.Ranges[1].EndValue = m_seriesScale.Maximum;
            m_seriesScale.Ranges[1].StartValue = mark;
        }

        /// <summary>
        /// Adds the factor label.
        /// </summary>      
        private void AddFactorLabel(int factor)
        {
            if (factor >= 10)
            {
                m_customFactorLabel.LabelValue = "X " + factor.ToString(CultureInfo.CurrentCulture);
                m_customFactorLabel.HorizontalAlignment = HorizontalAlignment.Center;
                m_customFactorLabel.FontSize = 9;
                m_customFactorLabel.VerticalAlignment = VerticalAlignment.Center;
                m_customFactorLabel.ToolTip = KpiInfo.MemberName;
                if (FrameType == GaugeFrameType.HalfCircle)
                {
                    m_customFactorLabel.Location = new Point(50, 42);
                }
                else
                    m_customFactorLabel.Location = new Point(50, 70);
            }
        }

        /// <summary>
        /// Adds the member label which displays the Member information.
        /// </summary>     
        private void AddMemberLabel()
        {
            //if (FrameType != GaugeFrameType.HalfCircle)
            //{
                double d = 50 + m_seriesScale.PointerCap.PointerCapRadius;
                double r = Radius;
                m_customMemberLabel.FontSize = 9;
                double length = (2*Math.Sqrt(Math.Pow(r, 2) - Math.Pow(d, 2)))/8.0;
                int intlength = Convert.ToInt32(length);
                if (KpiInfo.MemberName.Length > length)
                {
                    m_customMemberLabel.LabelValue = KpiInfo.MemberName.Substring(0, intlength) + "...";
                }
                else
                {
                    m_customMemberLabel.LabelValue = KpiInfo.MemberName;
                }
                m_customMemberLabel.ToolTip = KpiInfo.MemberName;
                if (FrameType == GaugeFrameType.HalfCircle)
                {
                    m_customMemberLabel.Location = new Point(50, 52);
                }
                else
                    m_customMemberLabel.Location = new Point(50, 78);
            //}
        }


        /// <summary>
        /// Adds the trend image.
        /// </summary>
        /// <param name="uriPath">The URI path.</param>
        private void AddTrendImage(string uriPath)
        {
            m_trendImage.ImageSource = new BitmapImage(new Uri(uriPath, UriKind.RelativeOrAbsolute));
            if (FrameType == GaugeFrameType.HalfCircle)
            {
                m_trendImage.Location = new Point(52, 52 + m_seriesScale.PointerCap.PointerCapRadius);
            }
            else
                m_trendImage.Location = new Point(52, (62 + m_seriesScale.PointerCap.PointerCapRadius + 15));

        }

        /// <summary>
        /// Adds the status image.
        /// </summary>
        /// <param name="uriPath">The URI path.</param>
        private void AddStatusImage(string uriPath)
        {
            m_statusImage.ImageSource = new BitmapImage(new Uri(uriPath, UriKind.RelativeOrAbsolute));
            if (FrameType == GaugeFrameType.HalfCircle)
            {
                m_statusImage.Location = new Point(42, (52 + m_seriesScale.PointerCap.PointerCapRadius));
            }
            else
                m_statusImage.Location = new Point(42, (62 + m_seriesScale.PointerCap.PointerCapRadius + 15));
        }

        /// <summary>
        /// Adds the status indicator.
        /// </summary>
        private void AddStatusIndicator()
        {
            if (KpiInfo.StatusGraphic == "Traffic Signals")
            {
                if (KpiInfo.StatusValue == -1)
                {
                    AddStatusImage(@"pack://application:,,,/Syncfusion.OlapGauge.WPF;component//Images/KPI/Red.png");
                }
                else if (KpiInfo.StatusValue == 0)
                {
                    AddStatusImage(
                        @"pack://application:,,,/Syncfusion.OlapGauge.WPF;component//Images/KPI/ThreeColor.png");
                }
                else if (KpiInfo.StatusValue == 1)
                {
                    AddStatusImage(@"pack://application:,,,/Syncfusion.OlapGauge.WPF;component//Images/KPI/Green.png");
                }
            }
            else
            {
                if (KpiInfo.StatusValue == -1)
                {
                    AddStatusImage(@"pack://application:,,,/Syncfusion.OlapGauge.WPF;component//Images/KPI/Diamond.png");
                }
                else if (KpiInfo.StatusValue == 0)
                {
                    AddStatusImage(@"pack://application:,,,/Syncfusion.OlapGauge.WPF;component//Images/KPI/Triangle.png");
                }
                else if (KpiInfo.StatusValue == 1)
                {
                    AddStatusImage(@"pack://application:,,,/Syncfusion.OlapGauge.WPF;component//Images/KPI/Circle.png");
                }
            }

            if ((!string.IsNullOrEmpty(KpiInfo.TrendGraphic) && KpiInfo.TrendGraphic.ToLower() == ("Standard arrow").ToLower()) || string.IsNullOrEmpty(KpiInfo.TrendGraphic))
            {
                if (KpiInfo.TrendValue == -1)
                {
                    AddTrendImage(@"pack://application:,,,/Syncfusion.OlapGauge.WPF;component//Images/KPI/DownArrow.png");
                }
                else if (KpiInfo.TrendValue == 0)
                {
                    AddTrendImage(
                        @"pack://application:,,,/Syncfusion.OlapGauge.WPF;component//Images/KPI/RightArrow.png");
                }
                else if (KpiInfo.TrendValue == 1)
                {
                    AddTrendImage(@"pack://application:,,,/Syncfusion.OlapGauge.WPF;component//Images/KPI/UpArrow.png");
                }
            }
        }

        /// <summary>
        /// Finds the nearest upper limit.
        /// </summary>
        /// <returns>integer value to the nearest factor</returns>
        private static int FindNearestUpperLimit(double number, int factor)
        {
            string convertedNumber = number.ToString(CultureInfo.InvariantCulture);
            string[] s = convertedNumber.Split('.');
            int j = int.Parse(s[0], CultureInfo.InvariantCulture);
            int k = j%factor;
            return j + (factor - k);
        }

        /// <summary>
        /// Gets the calculated Factor value.
        /// </summary>
        /// <param name="s">The string s contains double value in form of string</param>
        /// <returns>the factor by which the scales should be rendered</returns>
        private int GetCalculatedValue(string s)
        {
            int factor = 1;
            int len, dividend;
            if (s.Contains("."))
            {
                string[] str = s.Split('.');
                len = str[0].Length;
                dividend = int.Parse(Convert.ToString(str[0], CultureInfo.CurrentCulture), CultureInfo.CurrentCulture);
            }
            else
            {
                dividend = int.Parse(Convert.ToString(s, CultureInfo.CurrentCulture), CultureInfo.CurrentCulture);
                len = s.Length;
            }
            for (int i = 0; i < len; i++)
            {
                if (dividend > MaxValue)
                {
                    dividend = dividend/10;
                    factor = factor*10;
                }
                else
                {
                    break;
                }
            }

            return factor;
        }

        /// <summary>
        /// Overring templates for applying styles in gauge
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            EnableEffects = false;
            m_seriesScale = GetTemplateChild("PART_Scale") as CircularScale;
            m_customMemberLabel = GetTemplateChild("m_memberLabel") as CircularCustomLabel;
            m_customFactorLabel = GetTemplateChild("m_factorLabel") as CircularCustomLabel;
            m_statusImage = GetTemplateChild("image1") as GaugeImage;
            m_trendImage = GetTemplateChild("image2") as GaugeImage;
            gaugeIcon = GetTemplateChild("GuageIcon") as GaugeImage;
            m_pointercap = GetTemplateChild("PART_pointerCap") as PointerCap;
            m_circularGauge = GetTemplateChild("PART_OlapCircularGauge") as CircularGauge;

            if (m_seriesScale != null)
            {
                DataBind();
            }
        }

        /// <summary>
        /// Sets the max scale value based on either the goal or the measure value.
        /// </summary>
        /// <param name="goal">The goal value</param>
        /// <param name="value">The measure value</param>
        /// <returns>The factorized value of Goal and Value</returns>
        private int SetMaxScaleValue(double goal, double value)
        {
            int factor;
            if (value > goal)
            {
                factor = GetCalculatedValue(Convert.ToString(value, CultureInfo.InvariantCulture));
                m_seriesScale.Maximum = FindNearestUpperLimit((value/factor), 10);
            }
            else
            {
                factor = GetCalculatedValue(Convert.ToString(goal, CultureInfo.InvariantCulture));
                m_seriesScale.Maximum = FindNearestUpperLimit((((goal/factor)*4)/3), 10);
            }
            return factor;
        }

        public void AddGaugeImage(GaugeImage tempGaugeImage)
        {
            gaugeImage = tempGaugeImage;
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Binds the Values in KpiInfo to the OlapGauge
        /// </summary>
        public void DataBind()
        {
            EnableEffects = false;
            SizeToContainer = false;
            MaxValue = 99;
            m_pointercap.PointerCapRadius = (Radius/22);

            m_seriesScale.PointerCap = m_pointercap;
            m_seriesScale.Radius = (Radius - (FirstFrameThickness.Left + SecondFrameThickness.Left)) -
                                   SecondFrameThickness.Left;

            if (FrameType == GaugeFrameType.HalfCircle)
            {
                m_circularGauge.FirstFrameThickness = new Thickness(1);
                EnableEffects = false;
                m_seriesScale.StartAngle = 180;
                m_seriesScale.GapSweepAngle = 180;
                m_seriesScale.Ticks[0].DistanceFromScale = 2;
                m_seriesScale.Ticks[1].DistanceFromScale = 2;
                m_seriesScale.Ticks[2].DistanceFromScale = 5;
                m_seriesScale.Ranges[0].DistanceFromScale = -5;
                m_seriesScale.Ranges[1].DistanceFromScale = -2;
                m_seriesScale.Location = new Point(50, 88);
                m_seriesScale.PointerCap.PointerCapRadius = Radius/15;
                m_pointercap.PointerCapRadius = (Radius/18);
            }

            double goal;
            double value;
            double endValue = 0.0;
            if (string.IsNullOrEmpty(KpiInfo.MeasureValue) ||
                !Double.TryParse(KpiInfo.MeasureValue, out value))
            {
                value = 0.0;
            }


            if (string.IsNullOrEmpty(KpiInfo.GoalValue) ||
                !Double.TryParse(KpiInfo.GoalValue, out goal))
            {
                goal = 0.0;
            }

            int factor = SetMaxScaleValue(goal, value);
            double dValue = value;
            double mark = goal/factor;
            if (FrameType == GaugeFrameType.HalfCircle)
            {
                (m_seriesScale.Pointers[0]).PointerLength = m_seriesScale.Radius;
                (m_seriesScale.Pointers[0]).PointerWidth = Radius/12;
            }
            else
            {
                (m_seriesScale.Pointers[0]).PointerLength = (Radius - (FirstFrameThickness.Left));
                (m_seriesScale.Pointers[0]).PointerWidth = Radius/12;
            }

            if (FrameType == GaugeFrameType.HalfCircle)
            {
                (m_seriesScale.Pointers[1]).PointerLength = SecondFrameThickness.Left + 5;
                (m_seriesScale.Pointers[1]).PointerWidth = Radius/8;
                (m_seriesScale.Pointers[1]).PointerPlacement = ScalePlacement.Outside;
            }
            else
            {
                (m_seriesScale.Pointers[1]).PointerLength = FirstFrameThickness.Left;
                (m_seriesScale.Pointers[1]).PointerWidth = Radius/8;
            }

            double percentageValue = (dValue/factor);

            (m_seriesScale.Pointers[0]).Value = endValue + percentageValue;
            (m_seriesScale.Pointers[1]).Value = mark + endValue;

             if (this.FlowDirection.ToString()=="RightToLeft")
           {
                (m_seriesScale.Pointers[0]).ToolTip = ShowPointersTooltip ? string.Format("Revenue\n :{1}{0}", value, SR.GetString(CultureInfo.CurrentUICulture, "txtPointerTooltip")) : null;
               

                (m_seriesScale.Pointers[1]).ToolTip = ShowMarkersTooltip ? string.Format("Revenue\n :{1}{0}", goal, SR.GetString(CultureInfo.CurrentUICulture, "txtMarkerTooltip")) : null;
           }

           else
           {
               (m_seriesScale.Pointers[0]).ToolTip = ShowPointersTooltip ? string.Format("Revenue Value\n Value :{0}", value, SR.GetString(CultureInfo.CurrentUICulture, "txtPointerTooltip")) : null;
               

                (m_seriesScale.Pointers[1]).ToolTip = ShowMarkersTooltip ? string.Format("Revenue Goal\nGoal :{0}", goal, SR.GetString(CultureInfo.CurrentUICulture, "txtMarkerTooltip")) : null;
            }
        double startValue = endValue;
            endValue = endValue + percentageValue;


            AddDangerRange(startValue, mark);

            AddSaferRange(mark);

            ////Adding Markers
            if (ShowGaugeLabels)
            {
                AddMemberLabel();
            }

            if (ShowGaugeFactors)
            {
                AddFactorLabel(factor);
            }

            //if (FrameType != GaugeFrameType.HalfCircle)
            //{
                AddStatusIndicator();
            //}
            Scales.Add(m_seriesScale);
            if (gaugeImage != null)
            {
                gaugeIcon.ImageSource = gaugeImage.ImageSource;
                gaugeIcon.Location = gaugeImage.Location;
            }
        }

        #endregion
    }
}