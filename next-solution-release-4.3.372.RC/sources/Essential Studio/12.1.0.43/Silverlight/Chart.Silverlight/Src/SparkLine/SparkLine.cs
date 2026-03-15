#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class implementation for sparkline inherited from control
    /// </summary>
    public class SparkLine : Control
    {
        #region Constructors

        static SparkLine()
        {
            //Syncfusion.Licensing.EnvironmentTest.ValidateLicense(typeof(Chart));
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(SparkLine), new FrameworkPropertyMetadata(typeof(SparkLine)));
        }
        /// <summary>
        /// Called when instance created for SparkLine
        /// </summary>
        public SparkLine()
        {
            this.DefaultStyleKey = typeof(SparkLine);
            Data = new List<double>();
            m_DataItemsList = new ObservableCollection<object>();
        }

        

        //private void InitializeHighlightBrushes()
        //{
        //    firstPointBrush = lastPointBrush = negetivePointBrush = highPointBrush = lastPointBrush = this.Interior;
        //}

       
        //internal static bool IsSecurityGranted
        //{
        //    get
        //    {
        //        SecurityPermission perm = new SecurityPermission(PermissionState.Unrestricted);
        //        bool bResult = false;
        //        try
        //        {
        //            perm.Demand();
        //            bResult = true;
        //        }
        //        catch (Exception) { }
        //        return bResult;
        //    }
        //}

        /// <summary>
        /// Checks whether license is valid.
        /// </summary>
        internal static void ValidateLicense()
        {
//            try
//            {
//                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
//#if AllowUnsafeCode
//                new Syncfusion.Core.Licensing.LicensedComponent(typeof(SparkLine));
//#endif
//            }
//            finally
//            {
//                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
//            }
        }

        #endregion

        #region Properties
        /// <summary>
        /// Get or Set IsHighpointHighLighted property
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
        ///  Identifies the IsHighPointHighLighted dependency property.
        /// </summary>
        public static readonly DependencyProperty IsHighPointHighlightedProperty = DependencyProperty.Register("IsHighPointHighlighted", typeof(bool), typeof(SparkLine), new PropertyMetadata(new PropertyChangedCallback(OnIsHighPointHighlightedChanged)));
        /// <summary>
        /// Called when IsHighPointHighLighted property changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        private static void OnIsHighPointHighlightedChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sparkLine = d as SparkLine;
            if (sparkLine != null && sparkLine != null)
            {
                sparkLine.InvalidateArrange();
            }
        }
        /// <summary>
        ///  Identifies the BandRange dependency property.
        /// </summary>
        public static DependencyProperty
           BandRangeProperty = DependencyProperty.Register("BandRange", typeof(DoubleRange), typeof(SparkLine), new PropertyMetadata(DoubleRange.Empty, OnBandRangeChanged));
        /// <summary>
        /// Called when BandRange property is changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnBandRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sparkLine = d as SparkLine;
            if (sparkLine != null && sparkLine != null)
            {
                sparkLine.InvalidateMeasure();
            }
        }
        /// <summary>
        /// Get or Set Bandrange property
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
        ///  Identifies the RangeBandInterior dependency property.
        /// </summary>
        public static DependencyProperty RangeBandInteriorProperty = DependencyProperty.Register("RangeBandInterior", typeof(Brush), typeof(SparkLine), new PropertyMetadata(new SolidColorBrush(Colors.LightGray), new PropertyChangedCallback(OnRangeBandInteriorChanged)));
        /// <summary>
        /// Called when RangeBandInterior property changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnRangeBandInteriorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sparkLine = d as SparkLine;
            if (sparkLine != null && sparkLine != null)
            {
                sparkLine.InvalidateMeasure();
            }
            
        }
        /// <summary>
        /// Get or Set rangeBandInterior
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
        /// Identifies the IsEnabledRangeBand dependency property.
        /// </summary>
        public static DependencyProperty IsEnableRangeBandProperty = DependencyProperty.Register("IsEnableRangeBand", typeof(bool), typeof(SparkLine), new PropertyMetadata(false, new PropertyChangedCallback(OnIsEnableRangeBandChanged)));
        /// <summary>
        /// Called when IsEnabledRangeBand property changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnIsEnableRangeBandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sparkLine = d as SparkLine;
            if (sparkLine != null && sparkLine != null)
            {
                sparkLine.InvalidateMeasure();
            }
            
        }
        /// <summary>
        /// Get or Set IsEnablerangeBandProperty
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
        /// <summary>
        /// Get or Set IsLowpointHighLighted property
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
        ///  Identifies the IsLowPointHighLighted dependency property.
        /// </summary>
        public static readonly DependencyProperty IsLowPointHighlightedProperty = DependencyProperty.Register("IsLowPointHighlighted", typeof(bool), typeof(SparkLine), new PropertyMetadata(new PropertyChangedCallback(OnIsLowPointHighlightedChanged)));
        /// <summary>
        /// Called when isLowpointhighLighted property changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        private static void OnIsLowPointHighlightedChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sparkLine = d as SparkLine;
            if (sparkLine != null && sparkLine != null)
            {
                sparkLine.InvalidateArrange();
            }
        }
        /// <summary>
        /// Get or Set IsFirstpointHighlighted property
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
        ///  Identifies the IsFirstPointHighLighted dependency property.
        /// </summary>
        public static readonly DependencyProperty IsFirstPointHighlightedProperty = DependencyProperty.Register("IsFirstPointHighlighted", typeof(bool), typeof(SparkLine), new PropertyMetadata(new PropertyChangedCallback(OnIsFirstPointHighlightedChanged)));
        /// <summary>
        /// Called when IsFirstpointhighLighted property Changed 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        private static void OnIsFirstPointHighlightedChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sparkLine = d as SparkLine;
            if (sparkLine != null && sparkLine != null)
            {
                sparkLine.InvalidateArrange();
            }
        }
        /// <summary>
        /// Get or Set isLastpointHighLighted property
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
        /// Identifies the IsLastpointHighLighted dependency property.
        /// </summary>
        public static readonly DependencyProperty IsLastPointHighlightedProperty = DependencyProperty.Register("IsLastPointHighlighted", typeof(bool), typeof(SparkLine), new PropertyMetadata(new PropertyChangedCallback(OnIsLastPointHighlightedChanged)));
        /// <summary>
        /// Called when IsLastPointHighLighted property changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        private static void OnIsLastPointHighlightedChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sparkLine = d as SparkLine;
            if (sparkLine != null && sparkLine != null)
            {
                sparkLine.InvalidateArrange();
            }
        }
        /// <summary>
        /// Get or Set IsNegativePointsHighLighted property
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
        ///  Identifies the IsNegativepointsHighLighted dependency property.
        /// </summary>
        public static readonly DependencyProperty IsNegativePointsHighlightedProperty = DependencyProperty.Register("IsNegativePointsHighlighted", typeof(bool), typeof(SparkLine), new PropertyMetadata(new PropertyChangedCallback(OnIsNegativePointsHighlightedChanged)));
        /// <summary>
        /// Called when IsNegativePointsHighLighted property changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        private static void OnIsNegativePointsHighlightedChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sparkLine = d as SparkLine;
            if (sparkLine != null && sparkLine != null)
            {
                sparkLine.InvalidateArrange();
            }
        }
        /// <summary>
        /// Get or Set IsMarkerenabled property
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
        ///  Identifies the IsMerkerEnabled dependency property.
        /// </summary>
        public static readonly DependencyProperty IsMarkerEnabledProperty = DependencyProperty.Register("IsMarkerEnabled", typeof(bool), typeof(SparkLine), new PropertyMetadata(new PropertyChangedCallback(OnIsMarkerEnabledChanged)));
        /// <summary>
        /// Called when IsMarkerEnabled property changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        private static void OnIsMarkerEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sparkLine = d as SparkLine;
            if (sparkLine != null && sparkLine != null)
            {
                sparkLine.InvalidateArrange();
            }
        }

        /// <summary>
        /// Get or Set HighpointHighlightBrush property
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
        ///  Identifies the HighpointHighLightBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty HighPointHighlightBrushProperty = DependencyProperty.Register("HighPointHighlightBrush", typeof(Brush), typeof(SparkLine), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0XFF, 0X4F, 0X81, 0XBD)), new PropertyChangedCallback(OnHighPointHighlightBrushChanged)));
        /// <summary>
        /// Called when HighPointHighLightBrush property  get changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        private static void OnHighPointHighlightBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sparkLine = d as SparkLine;
            if (sparkLine != null && sparkLine != null)
            {
                sparkLine.InvalidateArrange();
            }
        }
        /// <summary>
        /// Get or Set LowpointHighLightBrush property
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
        ///  Identifies the lowPointHighLightBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty LowPointHighlightBrushProperty = DependencyProperty.Register("LowPointHighlightBrush", typeof(Brush), typeof(SparkLine), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0XFF, 0X4F, 0X81, 0XBD)), new PropertyChangedCallback(OnLowPointHighlightBrushChanged)));
        /// <summary>
        /// Called when LowPointHighLightBrush property changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        private static void OnLowPointHighlightBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sparkLine = d as SparkLine;
            if (sparkLine != null && sparkLine != null)
            {
                sparkLine.InvalidateArrange();
            }
        }
        /// <summary>
        /// Get or Set firstPointHighlightbrush property
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
        ///  Identifies the firstPointHighlightBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty FirstPointHighlightBrushProperty = DependencyProperty.Register("FirstPointHighlightBrush", typeof(Brush), typeof(SparkLine), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0XFF, 0X95, 0XB3, 0XD7)), new PropertyChangedCallback(OnFirstPointHighlightBrushChanged)));
        /// <summary>
        /// Called when FirstPointHighlightBrush property changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        private static void OnFirstPointHighlightBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sparkLine = d as SparkLine;
            if (sparkLine != null && sparkLine != null)
            {
                sparkLine.InvalidateArrange();
            }
        }
        /// <summary>
        /// Get or Set LastPointHighLightBrush property
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
        ///  Identifies the LastPointhighLightBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty LastPointHighlightBrushProperty = DependencyProperty.Register("LastPointHighlightBrush", typeof(Brush), typeof(SparkLine), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0XFF, 0X95, 0XB3, 0XD7)), new PropertyChangedCallback(OnLastPointHighlightBrushChanged)));
        /// <summary>
        /// called when LastPointHighLightBrush property
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        private static void OnLastPointHighlightBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sparkLine = d as SparkLine;
            if (sparkLine != null && sparkLine != null)
            {
                sparkLine.InvalidateArrange();
            }

        }
        /// <summary>
        /// Get or Set NegativePointsHighlightBrush property
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
        ///  Identifies the NegativePointsHighLightBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty NegativePointsHighlightBrushProperty = DependencyProperty.Register("NegativePointsHighlightBrush", typeof(Brush), typeof(SparkLine), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0XFF, 0XC0, 0X50, 0X4D)), new PropertyChangedCallback(OnNegativePointsHighlightBrushChanged)));
        /// <summary>
        /// Called when NegativepointHighLightBrush property  get changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        private static void OnNegativePointsHighlightBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sparkLine = d as SparkLine;
            if (sparkLine != null && sparkLine != null)
            {
                sparkLine.InvalidateArrange();
            }
        }


        /// <summary>
        /// Get or Set LinemarkerType property 
        /// </summary>
        public LineMarkerTypes LineMarkerType
        {
            get { return (LineMarkerTypes)GetValue(LineMarkerTypeProperty); }
            set { SetValue(LineMarkerTypeProperty, value); }
        }

       
        /// <summary>
        ///  Using a DependencyProperty as the backing store for LineMarkerType.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LineMarkerTypeProperty =
            DependencyProperty.Register("LineMarkerType", typeof(LineMarkerTypes), typeof(SparkLine), new PropertyMetadata(LineMarkerTypes.Square, new PropertyChangedCallback(OnLineMarkerTypeChanged)));
        /// <summary>
        /// Called when Linemarkertype property is changed 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        private static void OnLineMarkerTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sparkLine = d as SparkLine;
            if (sparkLine != null && sparkLine != null)
            {
                sparkLine.InvalidateArrange();
            }
        }

        /// <summary>
        /// Get or Set MarkerColor property
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
        ///  Identifies the MarkerColor dependency property.
        /// </summary>
        public static readonly DependencyProperty MarkerColorProperty = DependencyProperty.Register("MarkerColor", typeof(Brush), typeof(SparkLine), new PropertyMetadata(new PropertyChangedCallback(OnMarkerColorChanged)));
        /// <summary>
        /// Called when MarkerColor property get changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        private static void OnMarkerColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sparkLine = d as SparkLine;
            //if (sparkLine == null)
            //    sparkLine = new LinePresenter() { Points = sparkLine.Points, SparkLine = sparkLine };
            if (sparkLine != null && sparkLine != null)
            {
                sparkLine.InvalidateArrange();
            }
        }
        /// <summary>
        /// Get or Set Interior property
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
        ///  Identifies the Interior dependency property.
        /// </summary>
        public static readonly DependencyProperty InteriorProperty = DependencyProperty.Register("Interior", typeof(Brush), typeof(SparkLine), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0XFF, 0X21, 0X59, 0X67)), new PropertyChangedCallback(OnInteriorChanged)));
        /// <summary>
        /// called when InteriorProperty get changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        private static void OnInteriorChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sparkLine = d as SparkLine;
            if (sparkLine != null && sparkLine != null)
            {
                sparkLine.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Get or Set Stroke property
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
        ///  Identifies the Stroke dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register("Stroke", typeof(Brush), typeof(SparkLine), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0XFF, 0X21, 0X59, 0X67)), new PropertyChangedCallback(OnStrokeChanged)));
        /// <summary>
        /// Called when stroke property get changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        private static void OnStrokeChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sparkLine = d as SparkLine;
            if (sparkLine != null && sparkLine != null)
            {
                sparkLine.InvalidateArrange();
            }
        }

        /// <summary>
        /// Get or Set StrokeThickness property
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
        ///  Identifies the StrokeThickness dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register("StrokeThickness", typeof(double), typeof(SparkLine), new PropertyMetadata(1d, new PropertyChangedCallback(OnStrokeThicknessChanged)));
        /// <summary>
        /// Called when StrokeThickness property get changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        private static void OnStrokeThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {

        }
        /// <summary>
        /// Get or Set IsInversed property 
        /// </summary>
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
        /// <summary>
        ///  Identifies the IsInversed dependency property.
        /// </summary>
        internal static readonly DependencyProperty IsInversedProperty = DependencyProperty.Register("IsInversed", typeof(bool), typeof(SparkLine), new PropertyMetadata(new PropertyChangedCallback(OnIsInversedChanged)));
        /// <summary>
        /// Called when IsInversed property get changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        private static void OnIsInversedChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {

        }
        /// <summary>
        /// Get or Set SparkLineType property
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
        ///  Identifies the SparkLineType dependency property.
        /// </summary>
        public static readonly DependencyProperty SparkLineTypeProperty = DependencyProperty.Register("SparkLineType", typeof(SparkLineTypes), typeof(SparkLine), new PropertyMetadata(SparkLineTypes.Line, OnSparkLineTypeChanged));
        /// <summary>
        /// Called when SparkLineType property Changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        private static void OnSparkLineTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sparkLine = (SparkLine)d;
            if (sparkLine != null)
                sparkLine.InvalidateMeasure();
        }


        /// <summary>
        /// Get or Set Origin property 
        /// </summary>
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
        /// <summary>
        /// Get or Set OnOrigin property get changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        private static void OnOriginChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sparkLine = (SparkLine)d;
            if (sparkLine != null)
                sparkLine.InvalidateArrange();
        }


        /// <summary>
        /// Get or Set ShowAxis property
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
            DependencyProperty.Register("ShowAxis", typeof(bool), typeof(SparkLine), new PropertyMetadata(false, OnShowAxisChanged));
        /// <summary>
        ///called when ShowAxis property get changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        private static void OnShowAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sparkLine = (SparkLine)d;
            if (sparkLine != null)
                sparkLine.InvalidateArrange();
        }
        /// <summary>
        /// get or Set OriginLineStroke property
        /// </summary>
        public Brush OriginLineStroke
        {
            get { return (Brush)GetValue(OriginLineStrokeProperty); }
            set { SetValue(OriginLineStrokeProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for OriginLineStroke.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty OriginLineStrokeProperty =
            DependencyProperty.Register("OriginLineStroke", typeof(Brush), typeof(SparkLine), new PropertyMetadata(new SolidColorBrush(Colors.Black), OnOriginLineStrokeChanged));


        /// <summary>
        /// Called when OriginLineStroke property get changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        private static void OnOriginLineStrokeChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sparkLine = (SparkLine)d;
            if (sparkLine != null)
                sparkLine.InvalidateArrange();
        }




        /// <summary>
        /// Enum value for VerticalAxisEndPointMode
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
            DependencyProperty.Register("VerticalAxisEndPointMode", typeof(AxisEndPointMode), typeof(SparkLine), new PropertyMetadata(AxisEndPointMode.Auto, OnVerticalAxisEndPointModeChanged));
        /// <summary>
        /// Called when verticalaxisEndPointmode property get changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        private static void OnVerticalAxisEndPointModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sparkline = (SparkLine)d;
            if (sparkline != null)
            {
                sparkline.InvalidateArrange();
            }
        }



        /// <summary>
        /// Get or Set VerticalAxisMinimumValueProperty
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
            DependencyProperty.Register("VerticalAxisMinimumValue", typeof(double), typeof(SparkLine), new PropertyMetadata(OnVerticalAxisMinimumValueChanged));

        private static void OnVerticalAxisMinimumValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sparkline = (SparkLine)d;
            if (sparkline != null)
            {
                sparkline.InvalidateArrange();
            }
        }




        /// <summary>
        /// Get or Set VerticalAxisMaximumValueProperty
        /// </summary>
        public double VerticalAxisMaximumValue
        {
            get { return (double)GetValue(VerticalAxisMaximumValueProperty); }
            set { SetValue(VerticalAxisMaximumValueProperty, value); }
        }

       
        /// <summary>
        ///  Using a DependencyProperty as the backing store for VerticalAxisMaximumValue.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty VerticalAxisMaximumValueProperty =
            DependencyProperty.Register("VerticalAxisMaximumValue", typeof(double), typeof(SparkLine), new PropertyMetadata(OnVerticalAxisMaximumValueChanged));


        private static void OnVerticalAxisMaximumValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sparkline = (SparkLine)d;
            if (sparkline != null)
            {
                sparkline.InvalidateArrange();
            }
        }

        #endregion

        #region DataProperties
        /// <summary>
        /// Get or Set DataMemberPath property
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
        ///  Identifies the DataMemberPath dependency property.
        /// </summary>
        public static readonly DependencyProperty DataMemberPathProperty = DependencyProperty.Register("DataMemberPath", typeof(string), typeof(SparkLine), new PropertyMetadata(string.Empty, OnDataMemberPathChanged));
        /// <summary>
        /// Called when DataMemberPath property changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        private static void OnDataMemberPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
        }
        /// <summary>
        /// Get or Set ItemsSource property
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
        ///  Identifies the ItemsSource dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(SparkLine), new PropertyMetadata(null, OnItemsSourceChanged));
        /// <summary>
        /// called when ItemsSource property changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        protected static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ((SparkLine)d).GetItemsList(args.NewValue);
            (d as SparkLine).UnWireEvents(args.OldValue);
            (d as SparkLine).WireEvents(args.NewValue);
        }

        private void WireEvents(object source)
        {
            if (source == null)
                return;

            if (source is INotifyCollectionChanged)
            {
                var notifyCollectionchanged = source as INotifyCollectionChanged;
                notifyCollectionchanged.CollectionChanged += OnItemsCollectionChanged;
            }

            //else if (source is IBindingList)
            //{
            //    var listChanged = source as IBindingList;
            //    //listChanged.ListChanged += OnItemsListChanged;
            //}
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

            //else if (source is IBindingList)
            //{
            //    var listChanged = source as IBindingList;
            //    listChanged.ListChanged -= OnItemsListChanged;
            //}
        }


        //void OnItemsListChanged(object sender, ListChangedEventArgs e)
        //{

        //}

        void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    var d = GetItem(item);
                    this.Data.Add(d);
                }

            }
            //SetPoints();
            //InvalidateVisual();

        }


        #endregion

        #region Helpers

        //public Point ValueToPoint(double value, int index)
        //{
        //    if (_AreaPresenter != null)
        //        return TranslatePoint(new Point(ValueToIndex(index) * _AreaPresenter.ActualWidth, (1 - ValueToCoefficient(value)) * _AreaPresenter.ActualHeight), _AreaPresenter);
        //    return new Point(double.NaN, double.NaN);
        //}

        //public double ValueToCoefficient(double value)
        //{
        //    var delta = _data.Max() - _data.Min();
        //    delta = (delta == 0 ? 1 : delta);
        //    double result = (value - _data.Min()) / (delta);
        //    return IsInversed ? 1d - result : result;
        //}

        //public double ValueToIndex(int index)
        //{
        //    double result = (double)(index + 1) / (_data.Count + 1);
        //    return IsInversed ? 1d - result : result;
        //}

        #endregion

        #region Methods

        private void GetItemsList(object source)
        {
            m_DataItemsList.Clear();
            Data.Clear();
            if (source != null)
            {
                //if (source is DataTable)
                //{
                //    IEnumerable values = ((DataTable)source).Rows;
                //    foreach (object o in values)
                //    {
                //        if (o is DataRow)
                //        {
                //            object x = ((DataRow)o)[0];
                //            m_DataItemsList.Add(x);
                //        }
                //    }
                //}

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

        //private void SetPoints()
        //{
        //if (Points != null)
        //    Points.Clear();
        //else
        //    Points = new PointCollection();
        //for (int i = 0; i < _data.Count; i++)
        //{
        //    Points.Add(new Point(i, _data[i]));
        //}
        //}

        #endregion

        #region Private Members

        private ObservableCollection<object> m_DataItemsList;

        //private PointCollection Points
        //{
        //    get;
        //    set;
        //}
        /// <summary>
        /// Data collection property initialization
        /// </summary>
        public List<double> Data;



        private Grid _AreaPresenter;
        private Rect ViewPort;
        //private LinePresenter _LinePresenter;


        #endregion

        #region Overrides

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        /// <summary>
        /// Provides the behavior for the Measure pass of Silverlight layout. Classes can override this method to define their own Measure pass behavior.
        /// </summary>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of the allocated sizes for child objects; or based on other considerations, such as a fixed container size.
        /// </returns>
        /// <param name="availableSize">The available size that this object can give to child objects. Infinity (<see cref="F:System.Double.PositiveInfinity"/>) can be specified as a value to indicate that the object will size to whatever content is available.</param>
        protected override Size MeasureOverride(Size availableSize)
        {
            ViewPort = new Rect(new Point(0, 0), availableSize);
            _AreaPresenter = this.GetTemplateChild("m_areaPresenter") as Grid;
            if (_AreaPresenter != null && Data !=null && Data.Count > 0)
            {
                if (this.SparkLineType == SparkLineTypes.Line)
                {
                    DrawLine();
                    //_AreaPresenter.Content = new Polyline() { Points = this.LinePoints, Stroke = this.Interior, StrokeThickness = this.StrokeThickness};
                }
                else if (this.SparkLineType == SparkLineTypes.WinLoss)
                {
                    DrawWinLoss();
                    //_AreaPresenter.Content = new Path() { Data = this.WinLossGeometryGroup, Stroke = this.Stroke, StrokeThickness = this.StrokeThickness, Fill = this.Interior };
                }
                else if (this.SparkLineType == SparkLineTypes.Column)
                {
                    DrawColumn();
                    //_AreaPresenter.Content = new Path() { Data = this.ColumnGeometryGroup, Stroke = this.Stroke, StrokeThickness = this.StrokeThickness, Fill = this.Interior };
                }
            }
            return base.MeasureOverride(availableSize);

        }

        #endregion

        private void DrawSparkLine()
        {
            //if (Points != null)
            //{
                if (this.Data.Count > 0)
                {
                    if (this.SparkLineType == SparkLineTypes.Line)
                    {
                        DrawLine();
                    }
                    else if (this.SparkLineType == SparkLineTypes.Column)
                    {
                        DrawColumn();
                    }
                    else if (this.SparkLineType == SparkLineTypes.WinLoss)
                    {
                        DrawWinLoss();
                    }
                }
            //}
        }

        private void DrawRange()
        {
            if (Data != null && Data.Count > 0)
            {
                double[] sorterData = Data.ToArray();
                Array.Sort(sorterData);
                double mindata = sorterData[0];
                double maxdata = sorterData[sorterData.Length - 1];
                mindata = this.VerticalAxisEndPointMode == AxisEndPointMode.Auto ? mindata : this.VerticalAxisMinimumValue;
                maxdata = this.VerticalAxisEndPointMode == AxisEndPointMode.Auto ? maxdata : this.VerticalAxisMaximumValue;
                double deltaY = maxdata - mindata;
                deltaY = (deltaY == 0 ? 1 : deltaY);
                double val;
                double val1;
                double actualHeight = ViewPort.Height;
                if (this.IsEnableRangeBand)
                {
                    this.RangeBandInterior.Opacity = 0.5;
                    if (this.BandRange.Start < this.BandRange.End)
                    {
                        val = 1 - (this.BandRange.Start - mindata) / deltaY;
                        val1 = 1 - (this.BandRange.End - mindata) / deltaY;
                        _AreaPresenter.Children.Add(new Path() { Data = new RectangleGeometry() { Rect = new Rect(new Point(0, actualHeight * val1), new Point(ViewPort.Width, actualHeight * (val))) }, Fill = this.RangeBandInterior });
                    }
                    else
                    {
                        val1 = 1 - (this.BandRange.Start - mindata) / deltaY;
                        val = 1 - (this.BandRange.End - mindata) / deltaY;
                        _AreaPresenter.Children.Add(new Path() { Data = new RectangleGeometry() { Rect = new Rect(new Point(0, actualHeight * val1), new Point(ViewPort.Width, actualHeight * (val))) }, Fill = this.RangeBandInterior });
                    }
                }
            }
        }

        private void DrawWinLoss()
        {

            if (_AreaPresenter != null && _AreaPresenter.Children != null)
            {
                _AreaPresenter.Children.Clear();
            }
            DrawRange();
            double deltaX = ViewPort.Width / Data.Count;
            double[] sorterData = Data.ToArray();
            Array.Sort(sorterData);
            double mindata = sorterData[0];
            double maxdata = sorterData[sorterData.Length - 1];
            mindata = this.VerticalAxisEndPointMode == AxisEndPointMode.Auto ? mindata : this.VerticalAxisMinimumValue;
            maxdata = this.VerticalAxisEndPointMode == AxisEndPointMode.Auto ? maxdata : this.VerticalAxisMaximumValue;
            double deltaY = maxdata - mindata;
            deltaY = (deltaY == 0 ? 1 : deltaY);
            double delta = ViewPort.Width / Data.Count * 0.6;
            double xPos = (deltaX - delta) / 2;
            double yMin = mindata;
            double actualHeight = ViewPort.Height;
            double midpoint = ViewPort.Height / 2;
            //Pen normalpen = new Pen(SparkLine.Interior, SparkLine.StrokeThickness);
            //Pen negetivepen = new Pen(SparkLine.NegativePointsHighlightBrush, SparkLine.StrokeThickness);
            //double origin = SparkLine.ValueToPoint(0, 1).Y;
            this.WinLossGeometryGroup = new GeometryGroup() { FillRule = FillRule.Nonzero};
            for (var i = 0; i < Data.Count; i++)
            {
                    var val = 1 - (Data[i] - yMin) / deltaY;
                    if (Data[i] > this.Origin)
                    {

                        WinLossGeometryGroup.Children.Add(new RectangleGeometry() { Rect = new Rect(new Point(xPos, 0), new Point(xPos + delta, midpoint)) });
                        
                    }
                    else
                    {
                        if (this.IsNegativePointsHighlighted)
                        {
                            WinLossGeometryGroup.Children.Add(new RectangleGeometry() { Rect = new Rect(new Point(xPos, midpoint), new Point(xPos + delta, actualHeight)) });
                        }
                        else
                            WinLossGeometryGroup.Children.Add(new RectangleGeometry() { Rect = new Rect(new Point(xPos, midpoint), new Point(xPos + delta, actualHeight)) });
                    }
                    xPos += deltaX;
            }

            _AreaPresenter.Children.Add(new Path() { Data = WinLossGeometryGroup, Fill = this.Interior,  Stroke = this.Stroke, StrokeThickness = this.StrokeThickness});

            if (this.IsLastPointHighlighted)
            {
                xPos -= deltaX;
                Path lastsegemnt = new Path() { Fill = this.LastPointHighlightBrush };
                double lastVal = Data[Data.Count-1];
                if (lastVal > 0)
                {
                    var val = 1 - (lastVal - yMin) / deltaY;
                    lastsegemnt.Data = new RectangleGeometry(){Rect = new Rect(new Point(xPos, 0), new Point(xPos + delta, midpoint))};
                                
                }
                else
                {
                    var val = 1 - (lastVal- yMin) / deltaY;
                    lastsegemnt.Data = new RectangleGeometry() { Rect = new Rect(new Point(xPos, 0), new Point(xPos + delta, midpoint)) };
                }

                _AreaPresenter.Children.Add(lastsegemnt);
            }
            if (this.IsFirstPointHighlighted)
            {
                xPos = (deltaX - delta) / 2;
                Path firstsegemnt = new Path() { Fill = this.FirstPointHighlightBrush };
                if (Data.Count > 0 && Data[0] > 0)
                {
                    var val = 1 - (Data[0] - yMin) / deltaY;
                    firstsegemnt.Data = new RectangleGeometry() { Rect = new Rect(new Point(xPos, 0), new Point(xPos + delta, midpoint)) };

                }
                else if (Data.Count != 0)
                {
                    var val = 1 - (Data[0] - yMin) / deltaY;
                    firstsegemnt.Data = new RectangleGeometry() { Rect = new Rect(new Point(xPos, midpoint), new Point(xPos + delta, actualHeight)) };
                }

                _AreaPresenter.Children.Add(firstsegemnt);
            }
            if (this.IsHighPointHighlighted)
            {

                xPos = (Data.IndexOf(maxdata) * deltaX) + (deltaX - delta) / 2;

                Path highsegemnt = new Path() { Fill = this.HighPointHighlightBrush };
                if (maxdata > 0)
                {
                    var val = 1 - (maxdata - yMin) / deltaY;
                    highsegemnt.Data = new RectangleGeometry() { Rect = new Rect(new Point(xPos, 0), new Point(xPos + delta, midpoint)) };
                }
                else
                {
                    var val = 1 - (maxdata - yMin) / deltaY;
                    highsegemnt.Data = new RectangleGeometry() { Rect = new Rect(new Point(xPos, midpoint), new Point(xPos + delta, actualHeight)) };
                }
                _AreaPresenter.Children.Add(highsegemnt);
            }

            if (this.IsLowPointHighlighted)
            {
                xPos = Data.IndexOf(mindata);

                Path lowsegemnt = new Path() { Fill = this.LowPointHighlightBrush };
                if (mindata > 0)
                {
                    var val = 1 - (mindata - yMin) / deltaY;
                    lowsegemnt.Data = new RectangleGeometry() { Rect = new Rect(new Point(xPos, 0), new Point(xPos + delta, midpoint)) };
                }
                else
                {
                    var val = 1 - (mindata - yMin) / deltaY;
                    lowsegemnt.Data = new RectangleGeometry() { Rect = new Rect(new Point(xPos, midpoint), new Point(xPos + delta, actualHeight)) };
                }

                _AreaPresenter.Children.Add(lowsegemnt);
            }
            if (this.ShowAxis)
            {
                _AreaPresenter.Children.Add(new Path() { Data = new LineGeometry() { StartPoint =new Point(0, midpoint), EndPoint = new Point(ActualWidth, midpoint)  }, Stroke = OriginLineStroke, StrokeThickness = 1 });
            }
        }

        private void DrawColumn()
        {
            if (_AreaPresenter != null && _AreaPresenter.Children != null)
            {
                _AreaPresenter.Children.Clear();
            }
            if (Data != null && Data.Count > 0)
            {
                DrawRange();
                double deltaX = ViewPort.Width / Data.Count;
                double[] sorterData = Data.ToArray();
                Array.Sort(sorterData);
                double mindata = sorterData[0];
                double maxdata = sorterData[sorterData.Length - 1];
                mindata = this.VerticalAxisEndPointMode == AxisEndPointMode.Auto ? mindata : this.VerticalAxisMinimumValue;
                maxdata = this.VerticalAxisEndPointMode == AxisEndPointMode.Auto ? maxdata : this.VerticalAxisMaximumValue;
                double deltaY = maxdata - mindata;
                deltaY = (deltaY == 0 ? 1 : deltaY);
                double delta = ViewPort.Width / Data.Count * 0.6;
                double xPos = (deltaX - delta) / 2;
                double yMin = mindata;
                double actualHeight = ViewPort.Height;
                //Pen normalpen = new Pen(SparkLine.Interior, SparkLine.StrokeThickness);
                //Pen negetivepen = new Pen(SparkLine.NegativePointsHighlightBrush, SparkLine.StrokeThickness);
                double origin = this.Origin;
                origin = actualHeight * (1 - (origin - yMin) / deltaY);

                this.ColumnGeometryGroup = new GeometryGroup();
                for (var i = 0; i < Data.Count; i++)
                {
                    var val = 1 - (Data[i] - yMin) / deltaY;
                    if (Data[i] > 0 || !this.IsNegativePointsHighlighted)
                    {
                        this.ColumnGeometryGroup.Children.Add(new RectangleGeometry() { Rect = new Rect(new Point(xPos, actualHeight * val), new Point(xPos + delta, origin)) });

                    }
                    else
                    {
                        this.ColumnGeometryGroup.Children.Add(new RectangleGeometry() { Rect = new Rect(new Point(xPos, actualHeight * val), new Point(xPos + delta, origin)) });
                    }
                    xPos += deltaX;
                }
                _AreaPresenter.Children.Add(new Path() { Data = ColumnGeometryGroup, Fill = this.Interior, Stroke = this.Stroke, StrokeThickness = this.StrokeThickness });

                xPos -= deltaX;
                if (this.IsLastPointHighlighted)
                {
                    var val = 1 - (Data[Data.Count - 1] - yMin) / deltaY;
                    _AreaPresenter.Children.Add(new Path() { Data = new RectangleGeometry() { Rect = new Rect(new Point(xPos, actualHeight * val), new Point(xPos + delta, origin)) }, Fill = LastPointHighlightBrush });
                }
                if (this.IsFirstPointHighlighted)
                {
                    xPos = (deltaX - delta) / 2;
                        if (Data.Count > 0)
                        {
                            var val = 1 - (Data[0]- yMin) / deltaY;
                            _AreaPresenter.Children.Add(new Path() { Data = new RectangleGeometry() { Rect = new Rect(new Point(xPos, actualHeight * val), new Point(xPos + delta, origin)) }, Fill = FirstPointHighlightBrush });
                        }
                }
                if (this.IsHighPointHighlighted)
                {
                    xPos = Data.IndexOf(maxdata);
                    var val = 1 - (maxdata - yMin) / deltaY;
                    _AreaPresenter.Children.Add(new Path() { Data = new RectangleGeometry() { Rect = new Rect(new Point(xPos, actualHeight * val), new Point(xPos + delta, origin)) }, Fill = HighPointHighlightBrush });

                }
                if (this.IsLowPointHighlighted)
                {
                    xPos = Data.IndexOf(mindata) * deltaX + (deltaX - delta) / 2;
                    var val = 1 - (mindata - yMin) / deltaY;
                    _AreaPresenter.Children.Add(new Path() { Data = new RectangleGeometry() { Rect = new Rect(new Point(xPos, actualHeight * val), new Point(xPos + delta, origin)) }, Fill = LowPointHighlightBrush });
                }

                if (this.ShowAxis)
                {
                    _AreaPresenter.Children.Add(new Path() { Data = new LineGeometry(){ StartPoint =new Point(0, origin), EndPoint =new Point(ActualWidth, origin) }, Fill = this.OriginLineStroke, Stroke = this.OriginLineStroke, StrokeThickness=1d});
                }
            }
        }

        private void DrawLine()
        {
            if (_AreaPresenter != null && _AreaPresenter.Children != null)
            {
                _AreaPresenter.Children.Clear();
            }
            DrawRange();
            double deltaX = ViewPort.Width / Data.Count;
            double[] sorterData = Data.ToArray();
            Array.Sort(sorterData);
            double mindata = sorterData[0];
            double maxdata = sorterData[sorterData.Length - 1];
            mindata = this.VerticalAxisEndPointMode == AxisEndPointMode.Auto ? mindata : this.VerticalAxisMinimumValue;
            maxdata = this.VerticalAxisEndPointMode == AxisEndPointMode.Auto ? maxdata : this.VerticalAxisMaximumValue;
            double deltaY = maxdata - mindata;
            deltaY = (deltaY == 0 ? 0.5 : deltaY);
            double xPos = deltaX / 2;
            double yMin = mindata;
            double actualHeight = ViewPort.Height;
            Brush normalBrush = this.Interior;
            Brush negetiveBrush = this.NegativePointsHighlightBrush;
            double origin = this.Origin;

            origin = actualHeight * (1 - ((origin - yMin) / deltaY));

            LinePoints = new PointCollection();
            for (var i = 0; i < Data.Count; i++)
            {
                var val = 1 - (Data[i] - yMin) / deltaY;
                LinePoints.Add(new Point(xPos, actualHeight * val));
                xPos += deltaX;
            }
            _AreaPresenter.Children.Add(new Polyline() { Points = LinePoints, Stroke = this.Interior, StrokeThickness = this.StrokeThickness });
            GeometryGroup negativemarkersgroup = new GeometryGroup() { FillRule = FillRule.Nonzero };
            GeometryGroup positivemarkersgroup = new GeometryGroup() { FillRule = FillRule.Nonzero };
            if (this.IsMarkerEnabled || this.IsNegativePointsHighlighted)
            {
                xPos = deltaX / 2;
                for (var i = 0; i < Data.Count; i++)
                {

                    var val = 1 - (Data[i] - yMin) / deltaY;

                    if (this.IsNegativePointsHighlighted && Data[i] < 0)
                    {
                        if (this.LineMarkerType == LineMarkerTypes.Square)
                        {
                            negativemarkersgroup.Children.Add(new RectangleGeometry() { Rect = new Rect(new Point(xPos - 2.5, actualHeight * val - 2.5), new Size(5, 5)) });

                        }
                        else
                        {
                            negativemarkersgroup.Children.Add(new EllipseGeometry() { Center = new Point(xPos, actualHeight * val), RadiusX = 2.5d, RadiusY = 2.5d });
                        }
                    }

                    else if (this.IsMarkerEnabled)
                    {
                        if (this.LineMarkerType == LineMarkerTypes.Square)
                        {
                            positivemarkersgroup.Children.Add(new RectangleGeometry() { Rect = new Rect(new Point(xPos - 2.5, actualHeight * val - 2.5), new Size(5, 5)) });
                        }
                        else
                        {
                            positivemarkersgroup.Children.Add(new EllipseGeometry() { Center = new Point(xPos, actualHeight * val), RadiusX = 2.5d, RadiusY = 2.5d });
                        }
                    }

                    xPos = xPos + deltaX;

                }
                this._AreaPresenter.Children.Add(new Path() { Data = negativemarkersgroup, Fill = this.NegativePointsHighlightBrush });
                this._AreaPresenter.Children.Add(new Path() { Data = positivemarkersgroup, Fill = this.MarkerColor });
                xPos = xPos - deltaX;
            }
            if (this.IsFirstPointHighlighted)
            {
                {
                    if (Data.Count > 0)
                    {
                        var val = 1 - (Data[0] - yMin) / deltaY;
                        if (this.LineMarkerType == LineMarkerTypes.Square)
                        {
                            this._AreaPresenter.Children.Add(new Path() { Fill = this.FirstPointHighlightBrush, Data = new RectangleGeometry() { Rect = new Rect(new Point(deltaX / 2 - 2.5, actualHeight * val - 2.5), new Size(5, 5)) } });
                        }
                        else
                        {
                            this._AreaPresenter.Children.Add(new Path() { Fill = this.FirstPointHighlightBrush, Data = new EllipseGeometry() { Center = new Point(deltaX / 2, actualHeight * val), RadiusX = 2.5d, RadiusY = 2.5d } });
                        }
                    }
                }
            }
            if (this.IsLastPointHighlighted)
            {

                {
                    var val = 1 - (Data[Data.Count - 1] - yMin) / deltaY;
                    if (this.LineMarkerType == LineMarkerTypes.Square)
                    {
                        this._AreaPresenter.Children.Add(new Path() { Fill = this.LastPointHighlightBrush, Data = new RectangleGeometry() { Rect = new Rect(new Point(xPos - 2.5, actualHeight * val - 2.5), new Size(5, 5)) } });
                    }
                    else
                    {
                        this._AreaPresenter.Children.Add(new Path() { Fill = this.LastPointHighlightBrush, Data = new EllipseGeometry() { Center = new Point(xPos, actualHeight * val), RadiusY = 2.5d, RadiusX = 2.5d } });
                    }
                }
            }

            if (this.IsHighPointHighlighted)
            {

                //Point maxp = Points.OrderByDescending(p => p.Y).FirstOrDefault();
                var val = 1 - (maxdata - yMin) / deltaY;
                if (this.LineMarkerType == LineMarkerTypes.Square)
                {
                    this._AreaPresenter.Children.Add(new Path() { Data = new RectangleGeometry() { Rect = new Rect(new Point(Data.IndexOf(maxdata) * deltaX + (deltaX / 2) - 2.5, actualHeight * val - 2.5), new Size(5, 5)) }, Fill = this.HighPointHighlightBrush });

                }
                else
                {
                    this._AreaPresenter.Children.Add(new Path() { Data = new EllipseGeometry() { Center = new Point(Data.IndexOf(maxdata) * deltaX + (deltaX / 2), actualHeight * val), RadiusX = 2.5d, RadiusY = 2.5d }, Fill = this.HighPointHighlightBrush });
                }
            }

            if (this.IsLowPointHighlighted)
            {
                var val = 1 - (mindata - yMin) / deltaY;
                if (this.LineMarkerType == LineMarkerTypes.Square)
                {
                    this._AreaPresenter.Children.Add(new Path() { Data = new RectangleGeometry() { Rect = new Rect(new Point(Data.IndexOf(mindata) * deltaX + (deltaX / 2) - 2.5, actualHeight * val - 2.5), new Size(5, 5)) }, Fill = this.LowPointHighlightBrush });
                }
                else
                {
                    this._AreaPresenter.Children.Add(new Path() { Data = new EllipseGeometry() { Center = new Point(Data.IndexOf(mindata) * deltaX + (deltaX / 2), actualHeight * val), RadiusX = 2.5d, RadiusY = 2.5d }, Fill = this.LowPointHighlightBrush });
                }
            }
            if (this.ShowAxis)
            {
                this._AreaPresenter.Children.Add(new Path() { Fill = this.OriginLineStroke, Data = new LineGeometry() { StartPoint = new Point(0, origin), EndPoint = new Point(this.ActualWidth, origin) } });
            }
        }

        private PointCollection LinePoints
        {
            get { return (PointCollection)GetValue(LinePointsProperty); }
            set { SetValue(LinePointsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LinePoints.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty LinePointsProperty =
            DependencyProperty.Register("LinePoints", typeof(PointCollection), typeof(SparkLine), new PropertyMetadata(null));


        private GeometryGroup WinLossGeometryGroup
        {
            get { return (GeometryGroup)GetValue(WinLossGeometryGroupProperty); }
            set { SetValue(WinLossGeometryGroupProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WinLossGeomentryGroup.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty WinLossGeometryGroupProperty =
            DependencyProperty.Register("WinLossGeometryGroup", typeof(GeometryGroup), typeof(SparkLine), new PropertyMetadata(null));


        private GeometryGroup ColumnGeometryGroup
        {
            get { return (GeometryGroup)GetValue(ColumnGeometryGroupProperty); }
            set { SetValue(ColumnGeometryGroupProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnGeometryGroup.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty ColumnGeometryGroupProperty =
            DependencyProperty.Register("ColumnGeometryGroup", typeof(GeometryGroup), typeof(SparkLine), new PropertyMetadata(null));


        


        

        
    }
    /// <summary>
    /// Enum values for SparkLineTypes
    /// </summary>
    public enum SparkLineTypes
    {
        /// <summary>
        /// Enum values for Line type
        /// </summary>
        Line,
        /// <summary>
        /// Enum values for Column type
        /// </summary>
        Column,
        /// <summary>
        /// Enum values for WinLoss type
        /// </summary>
        WinLoss
    }
    /// <summary>
    /// Enum values for LineMarkertypes
    /// </summary>
    public enum LineMarkerTypes
    {
        /// <summary>
        /// Enum values for Square type
        /// </summary>
        Square,
        /// <summary>
        /// Enum value for Circle type
        /// </summary>
        Circle
    }
    /// <summary>
    /// Enum  values for AxisEndPointModes
    /// </summary>
    public enum AxisEndPointMode
    {
        /// <summary>
        /// Enum value for Auto mode
        /// </summary>
        Auto,
        /// <summary>
        /// Enum value for Custom mode
        /// </summary>
        Custom
    }

   
}
