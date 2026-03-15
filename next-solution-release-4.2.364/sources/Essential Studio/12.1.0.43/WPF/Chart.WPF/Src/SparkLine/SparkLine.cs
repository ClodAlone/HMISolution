#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections;
using System.Collections.ObjectModel;
using System.Security.Permissions;

namespace Syncfusion.Windows.Chart
{
    ///  <summary>
    ///  Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    /// 
    ///  Step 1a) Using this custom control in a XAML file that exists in the current project.
    ///  Add this XmlNamespace attribute to the root element of the markup file where it is 
    ///  to be used:
    /// 
    ///      xmlns:MyNamespace="clr-namespace:SparkLine"
    /// 
    /// 
    ///  Step 1b) Using this custom control in a XAML file that exists in a different project.
    ///  Add this XmlNamespace attribute to the root element of the markup file where it is 
    ///  to be used:
    /// 
    ///      xmlns:MyNamespace="clr-namespace:SparkLine;assembly=SparkLine"
    /// 
    ///  You will also need to add a project reference from the project where the XAML file lives
    ///  to this project and Rebuild to avoid compilation errors:
    /// 
    ///      Right click on the target project in the Solution Explorer and
    ///      "Add Reference"->"Projects"->[Select this project]
    /// 
    /// 
    ///  Step 2)
    ///  Go ahead and use your control in the XAML file.
    /// 
    ///   
    ///  </summary>
    public class SparkLine : Control
    {
        #region Constructors

        static SparkLine()
        {
            Syncfusion.Licensing.EnvironmentTest.ValidateLicense(typeof(Chart));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SparkLine), new FrameworkPropertyMetadata(typeof(SparkLine)));
        }

        /// <summary>
        /// Constructor for SparkLine
        /// </summary>
        public SparkLine()
        {
		    if (IsSecurityGranted)
            {
                //ValidateLicense();
            }
            Data = new ObservableCollection<double>();
            Points = new PointCollection();
            m_DataItemsList = new ObservableCollection<object>();
        }

        private void InitializeHighlightBrushes()
        {
            firstPointBrush = lastPointBrush = negetivePointBrush = highPointBrush = lastPointBrush = this.Interior;
        }

		        /// <summary>
        /// Checks whether security permission can be granted. Read-only.
        /// </summary>
        internal static bool IsSecurityGranted
        {
            get
            {
                SecurityPermission perm = new SecurityPermission(PermissionState.Unrestricted);
                bool bResult = false;
                try
                {
                    perm.Demand();
                    bResult = true;
                }
                catch (Exception) { }
                return bResult;
            }
        }

        /// <summary>
        /// Checks whether license is valid.
        /// </summary>
        internal static void ValidateLicense()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
#if AllowUnsafeCode
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(SparkLine));
#endif
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
        }

        #endregion

        #region Properties
        /// <summary>
        /// Get and Set IsHighPointHighlightedProperty
        /// </summary>
        public bool IsHighPointHighlighted
        {
            get
            {
                return (bool)GetValue(SparkLine.IsHighPointHighlightedProperty);
            }

            set
            {
                SetValue(SparkLine.IsHighPointHighlightedProperty, value);
            }
        }

        /// <summary>
        ///  Identifies the IsHighPointHighlighted dependency property.
        /// </summary>
        public static readonly DependencyProperty IsHighPointHighlightedProperty = DependencyProperty.Register("IsHighPointHighlighted", typeof(bool), typeof(SparkLine), new PropertyMetadata(new PropertyChangedCallback(OnIsHighPointHighlightedChanged)));

        /// <summary>
        ///  Identifies the BandRange dependency property.
        /// </summary>
        public static DependencyProperty
            BandRangeProperty = DependencyProperty.Register("BandRange", typeof(DoubleRange), typeof(SparkLine), new PropertyMetadata(DoubleRange.Empty, OnBandRangeChanged));

        private static void OnBandRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sparkLine = d as SparkLine;
            if (sparkLine != null && sparkLine._LinePresenter != null)
            {
                sparkLine._LinePresenter.InvalidateVisual();
            }
        }

        /// <summary>
        /// Get and Set BandRangeProperty
        /// </summary>
        public DoubleRange BandRange
        {
            get
            {
                return (DoubleRange)this.GetValue(SparkLine.BandRangeProperty);
            }
            set
            {
                SetValue(SparkLine.BandRangeProperty, value);
            }
        }
        /// <summary>
        /// Dependency proeprty for RangeBandInterior
        /// </summary>
        public static DependencyProperty RangeBandInteriorProperty = DependencyProperty.Register("RangeBandInterior", typeof(Brush), typeof(SparkLine), new PropertyMetadata(new SolidColorBrush(Colors.LightGray), new PropertyChangedCallback(OnRangeBandInteriorChanged)));

        private static void OnRangeBandInteriorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sparkLine = d as SparkLine;
            if (sparkLine != null && sparkLine._LinePresenter != null)
            {
                sparkLine._LinePresenter.InvalidateVisual();
            }
        }

        /// <summary>
        /// Get and Set the RangeBandInteriorProperty
        /// </summary>
        public Brush RangeBandInterior
        {
            get
            {
                return (Brush)this.GetValue(SparkLine.RangeBandInteriorProperty);
            }
            set
            {
                SetValue(SparkLine.RangeBandInteriorProperty, value);
            }
        }

        /// <summary>
        /// dependency property for Check RangeBand is enabled or not
        /// </summary>
        public static DependencyProperty IsEnableRangeBandProperty = DependencyProperty.Register("IsEnableRangeBand", typeof(bool), typeof(SparkLine), new PropertyMetadata(false, new PropertyChangedCallback(OnIsEnableRangeBandChanged)));

        private static void OnIsEnableRangeBandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sparkLine = d as SparkLine;
            if (sparkLine != null && sparkLine._LinePresenter != null)
            {
                sparkLine._LinePresenter.InvalidateVisual();
            }
        }

        /// <summary>
        /// Get and Set IsEnableRangeBand
        /// </summary>
        public bool IsEnableRangeBand
        {
            get
            {
                return (bool)this.GetValue(SparkLine.IsEnableRangeBandProperty);
            }
            set
            {
                SetValue(SparkLine.IsEnableRangeBandProperty, value);
            }
        }
        private static void OnIsHighPointHighlightedChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sparkLine = d as SparkLine;
            if (sparkLine != null && sparkLine._LinePresenter != null)
            {
                sparkLine._LinePresenter.InvalidateVisual();
            }
        }

        /// <summary>
        /// Get and Set the IsLowPointHighlightedProperty
        /// </summary>
        public bool IsLowPointHighlighted
        {
            get
            {
                return (bool)this.GetValue(SparkLine.IsLowPointHighlightedProperty);
            }

            set
            {
                SetValue(SparkLine.IsLowPointHighlightedProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for IsLowPointHighlighted
        /// </summary>
        public static readonly DependencyProperty IsLowPointHighlightedProperty = DependencyProperty.Register("IsLowPointHighlighted", typeof(bool), typeof(SparkLine), new PropertyMetadata(new PropertyChangedCallback(OnIsLowPointHighlightedChanged)));

        private static void OnIsLowPointHighlightedChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sparkLine = d as SparkLine;
            if (sparkLine != null && sparkLine._LinePresenter != null)
            {
                sparkLine._LinePresenter.InvalidateVisual();
            }
        }

        /// <summary>
        /// get and Set IsFirstPointHighlightedProperty
        /// </summary>
        public bool IsFirstPointHighlighted
        {
            get
            {
                return (bool)this.GetValue(SparkLine.IsFirstPointHighlightedProperty);
            }

            set
            {
                SetValue(SparkLine.IsFirstPointHighlightedProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for IsFirstPointHighlighted
        /// </summary>
        public static readonly DependencyProperty IsFirstPointHighlightedProperty = DependencyProperty.Register("IsFirstPointHighlighted", typeof(bool), typeof(SparkLine), new PropertyMetadata(new PropertyChangedCallback(OnIsFirstPointHighlightedChanged)));

        private static void OnIsFirstPointHighlightedChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sparkLine = d as SparkLine;
            if (sparkLine != null && sparkLine._LinePresenter != null)
            {
                sparkLine._LinePresenter.InvalidateVisual();
            }
        }

        //private void SetfirstPointHighlightBrush(DependencyObject d)
        //{   
        //        firstPointBrush = (IsFirstPointHighlighted) ? FirstPointHighlightBrush : Interior;
        //}

        /// <summary>
        /// Get and Set the IsLastPointHighlightedProperty
        /// </summary>
        public bool IsLastPointHighlighted
        {
            get
            {
                return (bool)this.GetValue(SparkLine.IsLastPointHighlightedProperty);
            }

            set
            {
                SetValue(SparkLine.IsLastPointHighlightedProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for IsLastPointHighlighted
        /// </summary>
        public static readonly DependencyProperty IsLastPointHighlightedProperty = DependencyProperty.Register("IsLastPointHighlighted", typeof(bool), typeof(SparkLine), new PropertyMetadata(new PropertyChangedCallback(OnIsLastPointHighlightedChanged)));

        private static void OnIsLastPointHighlightedChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sparkLine = d as SparkLine;
            if (sparkLine != null && sparkLine._LinePresenter != null)
            {
                sparkLine._LinePresenter.InvalidateVisual();
            }
        }

        /// <summary>
        /// Get and set IsNegativePointsHighlightedProperty
        /// </summary>
        public bool IsNegativePointsHighlighted
        {
            get
            {
                return (bool)this.GetValue(SparkLine.IsNegativePointsHighlightedProperty);
            }

            set
            {
                SetValue(SparkLine.IsNegativePointsHighlightedProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for IsNegativePointsHighlighted
        /// </summary>
        public static readonly DependencyProperty IsNegativePointsHighlightedProperty = DependencyProperty.Register("IsNegativePointsHighlighted", typeof(bool), typeof(SparkLine), new PropertyMetadata(new PropertyChangedCallback(OnIsNegativePointsHighlightedChanged)));

        private static void OnIsNegativePointsHighlightedChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sparkLine = d as SparkLine;
            if (sparkLine != null && sparkLine._LinePresenter != null)
            {
                sparkLine._LinePresenter.InvalidateVisual();
            }
        }

        /// <summary>
        /// Get and Set IsMarkerEnabledProperty
        /// </summary>
        public bool IsMarkerEnabled
        {
            get
            {
                return (bool)this.GetValue(SparkLine.IsMarkerEnabledProperty);
            }

            set
            {
                SetValue(SparkLine.IsMarkerEnabledProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for IsMarkerEnabled
        /// </summary>
        public static readonly DependencyProperty IsMarkerEnabledProperty = DependencyProperty.Register("IsMarkerEnabled", typeof(bool), typeof(SparkLine), new PropertyMetadata(new PropertyChangedCallback(OnIsMarkerEnabledChanged)));

        private static void OnIsMarkerEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sparkLine = d as SparkLine;
            if (sparkLine != null && sparkLine._LinePresenter != null)
            {
                sparkLine._LinePresenter.InvalidateVisual();
            }
        }


        /// <summary>
        /// Get and Set HighPointHighlightBrushProperty
        /// </summary>
        public Brush HighPointHighlightBrush
        {
            get
            {
                return (Brush)this.GetValue(SparkLine.HighPointHighlightBrushProperty);
            }

            set
            {
                SetValue(SparkLine.HighPointHighlightBrushProperty, value);
            }
        }

        /// <summary>
        ///  Identifies the HighPointHighlightBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty HighPointHighlightBrushProperty = DependencyProperty.Register("HighPointHighlightBrush", typeof(Brush), typeof(SparkLine), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0XFF, 0X4F, 0X81, 0XBD)), new PropertyChangedCallback(OnHighPointHighlightBrushChanged)));

        private static void OnHighPointHighlightBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sparkLine = d as SparkLine;
            if (sparkLine != null && sparkLine._LinePresenter != null)
            {
                sparkLine._LinePresenter.InvalidateVisual();
            }
        }

        /// <summary>
        /// Get and Set LowPointHighlightBrushProperty
        /// </summary>
        public Brush LowPointHighlightBrush
        {
            get
            {
                return (Brush)this.GetValue(SparkLine.LowPointHighlightBrushProperty);
            }

            set
            {
                SetValue(SparkLine.LowPointHighlightBrushProperty, value);
            }
        }

        /// <summary>
        ///  Identifies the LowPointHighlightBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty LowPointHighlightBrushProperty = DependencyProperty.Register("LowPointHighlightBrush", typeof(Brush), typeof(SparkLine), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0XFF, 0X4F, 0X81, 0XBD)), new PropertyChangedCallback(OnLowPointHighlightBrushChanged)));

        private static void OnLowPointHighlightBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sparkLine = d as SparkLine;
            if (sparkLine != null && sparkLine._LinePresenter != null)
            {
                sparkLine._LinePresenter.InvalidateVisual();
            }
        }

        /// <summary>
        /// Get and Set FirstPointHighlightBrushProperty
        /// </summary>
        public Brush FirstPointHighlightBrush
        {
            get
            {
                return (Brush)this.GetValue(SparkLine.FirstPointHighlightBrushProperty);
            }

            set
            {
                SetValue(SparkLine.FirstPointHighlightBrushProperty, value);
            }
        }

        /// <summary>
        ///  Identifies the FirstPointHighlightBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty FirstPointHighlightBrushProperty = DependencyProperty.Register("FirstPointHighlightBrush", typeof(Brush), typeof(SparkLine), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0XFF, 0X95, 0XB3, 0XD7)),new PropertyChangedCallback(OnFirstPointHighlightBrushChanged)));

        private static void OnFirstPointHighlightBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sparkLine = d as SparkLine;
            if (sparkLine != null && sparkLine._LinePresenter != null)
            {
                sparkLine._LinePresenter.InvalidateVisual();
            }
        }

        /// <summary>
        /// Get and Set LastPointHighlightBrushProperty
        /// </summary>
        public Brush LastPointHighlightBrush
        {
            get
            {
                return (Brush)this.GetValue(SparkLine.LastPointHighlightBrushProperty);
            }

            set
            {
                SetValue(SparkLine.LastPointHighlightBrushProperty, value);
            }
        }

        /// <summary>
        ///  Identifies the LastPointHighlightBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty LastPointHighlightBrushProperty = DependencyProperty.Register("LastPointHighlightBrush", typeof(Brush), typeof(SparkLine), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0XFF, 0X95, 0XB3, 0XD7)), new PropertyChangedCallback(OnLastPointHighlightBrushChanged)));

        private static void OnLastPointHighlightBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sparkLine = d as SparkLine;
            if (sparkLine != null && sparkLine._LinePresenter != null)
            {
                sparkLine._LinePresenter.InvalidateVisual();
            }
                
        }

        /// <summary>
        /// Get and Set NegativePointsHighlightBrushProperty
        /// </summary>
        public Brush NegativePointsHighlightBrush
        {
            get
            {
                return (Brush)this.GetValue(SparkLine.NegativePointsHighlightBrushProperty);
            }

            set
            {
                SetValue(SparkLine.NegativePointsHighlightBrushProperty, value);
            }
        }

        /// <summary>
        ///  Identifies the NegativePointsHighlightBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty NegativePointsHighlightBrushProperty = DependencyProperty.Register("NegativePointsHighlightBrush", typeof(Brush), typeof(SparkLine), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0XFF, 0XC0, 0X50, 0X4D)), new PropertyChangedCallback(OnNegativePointsHighlightBrushChanged)));

        private static void OnNegativePointsHighlightBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sparkLine = d as SparkLine;
            if (sparkLine != null && sparkLine._LinePresenter != null)
            {
                sparkLine._LinePresenter.InvalidateVisual();
            }
        }



        /// <summary>
        /// Get and Set LineMarkerTypeProperty
        /// </summary>
        public LineMarkerTypes LineMarkerType
        {
            get { return (LineMarkerTypes)GetValue(LineMarkerTypeProperty); }
            set { SetValue(LineMarkerTypeProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for LineMarkerType.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LineMarkerTypeProperty =
            DependencyProperty.Register("LineMarkerType", typeof(LineMarkerTypes), typeof(SparkLine), new UIPropertyMetadata(LineMarkerTypes.Square, new PropertyChangedCallback(OnLineMarkerTypeChanged)));

        private static void OnLineMarkerTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sparkLine = d as SparkLine;
            if (sparkLine != null && sparkLine._LinePresenter != null)
            {
                sparkLine._LinePresenter.InvalidateVisual();
            }
        }


        /// <summary>
        /// Get and Set MarkerColorProperty
        /// </summary>
        public Brush MarkerColor
        {
            get
            {
                return (Brush)this.GetValue(SparkLine.MarkerColorProperty);
            }

            set
            {
                SetValue(SparkLine.MarkerColorProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for MarkerColorProperty
        /// </summary>
        public static readonly DependencyProperty MarkerColorProperty = DependencyProperty.Register("MarkerColor", typeof(Brush), typeof(SparkLine), new PropertyMetadata(new PropertyChangedCallback(OnMarkerColorChanged)));

        private static void OnMarkerColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sparkLine = d as SparkLine;
            if(sparkLine._LinePresenter == null)
                sparkLine._LinePresenter = new LinePresenter() { Points = sparkLine.Points, SparkLine = sparkLine };
            if (sparkLine != null && sparkLine._LinePresenter != null)
            {
                sparkLine._LinePresenter.InvalidateVisual();
            }
        }

        /// <summary>
        /// Get and Set InteriorProperty
        /// </summary>
        public Brush Interior
        {
            get
            {
                return (Brush)this.GetValue(SparkLine.InteriorProperty);
            }

            set
            {
                SetValue(SparkLine.InteriorProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for Sparkline Interior
        /// </summary>
        public static readonly DependencyProperty InteriorProperty = DependencyProperty.Register("Interior", typeof(Brush), typeof(SparkLine), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0XFF, 0X21, 0X59, 0X67)), new PropertyChangedCallback(OnInteriorChanged)));

        private static void OnInteriorChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sparkLine = d as SparkLine;
            if (sparkLine != null && sparkLine._LinePresenter != null)
            {
                sparkLine._LinePresenter.InvalidateVisual();
            }
        }

        /// <summary>
        /// Get and Set StrokeProperty
        /// </summary>
        public Brush Stroke
        {
            get
            {
                return (Brush)this.GetValue(SparkLine.StrokeProperty);
            }

            set
            {
                SetValue(SparkLine.StrokeProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for Sparkline Stroke
        /// </summary>
        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register("Stroke", typeof(Brush), typeof(SparkLine), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0XFF, 0X21, 0X59, 0X67)), new PropertyChangedCallback(OnStrokeChanged)));

        private static void OnStrokeChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sparkLine = d as SparkLine;
            if (sparkLine != null && sparkLine._LinePresenter != null)
            {
                sparkLine._LinePresenter.InvalidateVisual();
            }
        }

        /// <summary>
        /// Get and Set StrokeThicknessProperty
        /// </summary>
        public double StrokeThickness
        {
            get
            {
                return (double)this.GetValue(SparkLine.StrokeThicknessProperty);
            }

            set
            {
                SetValue(SparkLine.StrokeThicknessProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for Sparkline StrokeThickness
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register("StrokeThickness", typeof(double), typeof(SparkLine), new PropertyMetadata(1d, new PropertyChangedCallback(OnStrokeThicknessChanged)));

        private static void OnStrokeThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {

        }

        internal bool IsInversed
        {
            get
            {
                return (bool)this.GetValue(SparkLine.IsInversedProperty);
            }

            set
            {
                SetValue(SparkLine.IsInversedProperty, value);
            }
        }

        internal static readonly DependencyProperty IsInversedProperty = DependencyProperty.Register("IsInversed", typeof(bool), typeof(SparkLine), new PropertyMetadata(new PropertyChangedCallback(OnIsInversedChanged)));

        private static void OnIsInversedChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {

        }

        /// <summary>
        /// Get and Set SparkLineTypeProperty
        /// </summary>
        public SparkLineTypes SparkLineType
        {
            get
            {
                return (SparkLineTypes)GetValue(SparkLine.SparkLineTypeProperty);
            }

            set
            {
                SetValue(SparkLine.SparkLineTypeProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for Type for Sparkline
        /// </summary>
        public static readonly DependencyProperty SparkLineTypeProperty = DependencyProperty.Register("SparkLineType", typeof(SparkLineTypes), typeof(SparkLine), new PropertyMetadata(SparkLineTypes.Line, OnSparkLineTypeChanged));

        private static void OnSparkLineTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sparkLine = (SparkLine)d;
            if (sparkLine != null)
                sparkLine.InvalidateVisual();
        }



        internal double Origin
        {
            get { return (double)GetValue(OriginProperty); }
            set { SetValue(OriginProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for Origin.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty OriginProperty =
            DependencyProperty.Register("Origin", typeof(double), typeof(SparkLine), new PropertyMetadata(0d, OnOriginChanged));

        private static void OnOriginChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sparkLine = (SparkLine)d;
            if (sparkLine != null)
                sparkLine.InvalidateVisual();
        }



        /// <summary>
        /// Get and Set ShowAxisProperty
        /// </summary>
        public bool ShowAxis
        {
            get { return (bool)GetValue(ShowAxisProperty); }
            set { SetValue(ShowAxisProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowAxis.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowAxisProperty =
            DependencyProperty.Register("ShowAxis", typeof(bool), typeof(SparkLine), new UIPropertyMetadata(false, OnShowAxisChanged));

        private static void OnShowAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sparkLine = (SparkLine)d;
            if (sparkLine != null)
                sparkLine.InvalidateVisual();
        }

        /// <summary>
        /// Get and Set OriginLineStrokeProperty
        /// </summary>
        public Pen OriginLineStroke
        {
            get { return (Pen)GetValue(OriginLineStrokeProperty); }
            set { SetValue(OriginLineStrokeProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for OriginLineStroke.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty OriginLineStrokeProperty =
            DependencyProperty.Register("OriginLineStroke", typeof(Pen), typeof(SparkLine), new UIPropertyMetadata(new Pen(Brushes.Black, 1), OnOriginLineStrokeChanged));



        private static void OnOriginLineStrokeChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sparkLine = (SparkLine)d;
            if (sparkLine != null)
                sparkLine.InvalidateVisual();
        }




        /// <summary>
        /// Get and Set VerticalAxisEndPointModeProperty
        /// </summary>
        public AxisEndPointMode VerticalAxisEndPointMode
        {
            get { return (AxisEndPointMode)GetValue(VerticalAxisEndPointModeProperty); }
            set { SetValue(VerticalAxisEndPointModeProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for VerticalAxisEndPointMode.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty VerticalAxisEndPointModeProperty =
            DependencyProperty.Register("VerticalAxisEndPointMode", typeof(AxisEndPointMode), typeof(SparkLine), new UIPropertyMetadata(AxisEndPointMode.Auto, OnVerticalAxisEndPointModeChanged));

        private static void OnVerticalAxisEndPointModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sparkline = (SparkLine)d;
            if (sparkline != null)
            {
                sparkline.InvalidateVisual();
            }
        }



        /// <summary>
        /// Get and Set VerticalAxisMinimumValueProperty
        /// </summary>
        public double VerticalAxisMinimumValue
        {
            get { return (double)GetValue(VerticalAxisMinimumValueProperty); }
            set { SetValue(VerticalAxisMinimumValueProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for VerticalAxisMinimumValue.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty VerticalAxisMinimumValueProperty =
            DependencyProperty.Register("VerticalAxisMinimumValue", typeof(double), typeof(SparkLine), new UIPropertyMetadata(OnVerticalAxisMinimumValueChanged));

        private static void OnVerticalAxisMinimumValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sparkline = (SparkLine)d;
            if (sparkline != null)
            {
                sparkline.InvalidateVisual();
            }
        }




        /// <summary>
        /// Get and set VerticalAxisMaximumValueProperty
        /// </summary>
        public double VerticalAxisMaximumValue
        {
            get { return (double)GetValue(VerticalAxisMaximumValueProperty); }
            set { SetValue(VerticalAxisMaximumValueProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for VerticalAxisMaximumValue.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty VerticalAxisMaximumValueProperty =
            DependencyProperty.Register("VerticalAxisMaximumValue", typeof(double), typeof(SparkLine), new UIPropertyMetadata(OnVerticalAxisMaximumValueChanged));


        private static void OnVerticalAxisMaximumValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sparkline = (SparkLine)d;
            if (sparkline != null)
            {
                sparkline.InvalidateVisual();
            }
        }
        
        #endregion

        #region DataProperties

        /// <summary>
        /// Get and Set DataMemberPathProperty
        /// </summary>
        public string DataMemberPath
        {
            get
            {
                return (string)GetValue(SparkLine.DataMemberPathProperty);
            }

            set
            {
                SetValue(SparkLine.DataMemberPathProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for DataMemberPath
        /// </summary>
        public static readonly DependencyProperty DataMemberPathProperty = DependencyProperty.Register("DataMemberPath", typeof(string), typeof(SparkLine), new PropertyMetadata(string.Empty, OnDataMemberPathChanged));

        private static void OnDataMemberPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
        }

        /// <summary>
        /// Get and Set ItemsSourceProperty
        /// </summary>
        public IEnumerable ItemsSource
        {
            get
            {
                return (IEnumerable)GetValue(SparkLine.ItemsSourceProperty);
            }

            set
            {
                SetValue(SparkLine.ItemsSourceProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for ItemsSource for Sparkline
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(SparkLine), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnItemsSourceChanged));
        /// <summary>
        /// Method for Itemssource changes in Sparkline
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        protected static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ((SparkLine) d).GetItemsList(args.NewValue);
            (d as SparkLine).UnWireEvents(args.OldValue);
            (d as SparkLine).WireEvents(args.NewValue);
        }

        private void WireEvents(object  source)
        {
            if (source == null)
                return;

                if (source is INotifyCollectionChanged)
                {
                    var notifyCollectionchanged = source as INotifyCollectionChanged;
                    notifyCollectionchanged.CollectionChanged += OnItemsCollectionChanged;
                }

                else if (source is IBindingList)
                {
                    var listChanged = source as IBindingList;
                    listChanged.ListChanged += OnItemsListChanged;
                }
        }

        private void UnWireEvents(object source)
        {
            if (source == null)
                return;

            if (source is INotifyCollectionChanged)
            {
                var notifyCollectionchanged = source as INotifyCollectionChanged;
                notifyCollectionchanged.CollectionChanged -= OnItemsCollectionChanged;
            }

            else if (source is IBindingList)
            {
                var listChanged = source as IBindingList;
                listChanged.ListChanged -= OnItemsListChanged;
            }
        }


        void OnItemsListChanged(object sender, ListChangedEventArgs e)
        {
            
        }

        void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            
            if(e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    var d = GetItem(item);
                    this.Data.Add(d);
                }

            }
            SetPoints();
            //InvalidateVisual();
            
        }


        #endregion

        #region Helpers

        /// <summary>
        /// Return the point value from double value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public Point ValueToPoint(double value, int index)
        {
            if (_AreaPresenter != null)
                return TranslatePoint(new Point(ValueToIndex(index) * _AreaPresenter.ActualWidth, (1 - ValueToCoefficient(value)) * _AreaPresenter.ActualHeight), _AreaPresenter);
            return new Point(double.NaN, double.NaN);
        }

        /// <summary>
        /// Return double value of Coefficient 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public double ValueToCoefficient(double value)
        {
            var delta = Data.Max() - Data.Min();
            delta = (delta ==0 ? 1 : delta);
            double result = (value - Data.Min()) / (delta);
            return IsInversed ? 1d - result : result;
        }

        /// <summary>
        /// Return double value from int value
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public double ValueToIndex(int index)
        {
            double result = (double)(index + 1) / (Data.Count + 1);
            return IsInversed ? 1d - result : result;
        }

        #endregion
        
        #region Methods

        private void GetItemsList(object source)
        {
            m_DataItemsList.Clear();
            Data.Clear();
            if (source != null)
            {
                if (source is DataTable)
                {
                    IEnumerable values = ((DataTable)source).Rows;
                    foreach (object o in values)
                    {
                        if (o is DataRow)
                        {
                            object x = ((DataRow)o)[0];
                            m_DataItemsList.Add(x);
                        }
                    }
                }

                if (source is ICollection)
                {
                    var collection = source as IEnumerable;
                    foreach (object item in collection)
                    {
                        if (!double.IsNaN(GetItem(item)))
                        {
                            Data.Add(GetItem(item));
                            m_DataItemsList.Add(item);
                        }
                    }
                }
            }
        }

        private double GetItem(object item)
        {
            object x;
            if (DataMemberPath == null || DataMemberPath == string.Empty)
            {
                x = item;
            }
            else
            {
                PropertyInfo propertyInfo = item.GetType().GetProperty(DataMemberPath);
                x = propertyInfo != null ? propertyInfo.GetValue(item, BindingFlags.Default, null, null, null) : null;
            }
            double val;
            if (x == null)
                return 0d;
            if (x.GetType() == typeof(double))
                val = Convert.ToDouble(x);
            else if (x.ToString() == string.Empty || x.ToString() == null)
                val = double.NaN;
            else
                val = 0d;
            return val;
        }

        private void SetPoints()
        { 
            if(Points != null)
                Points.Clear();
            else 
                Points = new PointCollection();
            for (int i = 0; i < Data.Count;i++ )
            {
                Points.Add(new Point(i,Data[i]));
            }
        }

        #endregion

        #region Private Members

        private ObservableCollection<object> m_DataItemsList;

        private PointCollection Points
        {
            get;
            set;
        }

        /// <summary>
        /// Declaration of collection variable.
        /// </summary>
        public ObservableCollection<double> Data;
        internal Brush firstPointBrush;
        internal Brush lastPointBrush;
        internal Brush highPointBrush;
        internal Brush negetivePointBrush;



        private ContentPresenter _AreaPresenter;
        private LinePresenter _LinePresenter;
        

        #endregion

        #region Overrides

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            InitializeHighlightBrushes();

            _AreaPresenter = this.GetTemplateChild("m_areaPresenter") as ContentPresenter;
            if (_AreaPresenter != null)
            {
                _LinePresenter = new LinePresenter() { Points = Points, SparkLine = this };
                _AreaPresenter.Content = _LinePresenter;
            }

            base.OnApplyTemplate();
        }

        /// <summary>
        /// When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing. 
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            SetPoints();
            base.OnRender(drawingContext);
        }

        #endregion
    }

   

  
}
