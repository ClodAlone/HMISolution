// <copyright file="ChartAxis.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.ComponentModel;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Text;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Input;
    using System.Windows.Markup;
    using Syncfusion.Windows.Shared;
    using System.Xml;
    using System.Data;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;

    /// <summary>
    /// The ChartAxis class represents an axis of the <see cref="ChartArea" />.
    /// </summary>
    /// <remarks>
    /// A ChartArea contains a minimum of two axes namely primary axis and secondary
    /// axis in a Chart control. Values / data in the chart are plotted against these
    /// axes. Chart WPF also supports adding multiple axes to the chart area and the
    /// series can be drawn on any axis in the collection. 
    /// </remarks>
    /// <example>
    /// XAML:
    /// <code language="XAML">
    /// &lt;syncfusion:ChartArea Name="area"&gt;
    ///           &lt;syncfusion:ChartArea.PrimaryAxis&gt;
    ///               &lt;syncfusion:ChartAxis Header="X-Axis"  /&gt;
    ///            &lt;/syncfusion:ChartArea.PrimaryAxis&gt;
    ///            &lt;syncfusion:ChartArea.SecondaryAxis&gt;
    ///                &lt;syncfusion:ChartAxis Header="Y-Axis" /&gt;                      
    ///            &lt;/syncfusion:ChartArea.SecondaryAxis&gt;
    ///            &lt;syncfusion:ChartSeries Name="series" Data=" 1 35 2 45 3 30 4 25 5
    /// 40" /&gt;                                     &lt;/syncfusion:ChartArea&gt; 
    /// </code>
    /// XAML:
    /// <code language="C#">
    ///  ChartAxis axis = new ChartAxis();
    ///             axis.Header = "X-Axis";
    ///             chartArea.PrimaryAxis = axis;
    ///  ChartAxis yaxis = new ChartAxis();
    ///             yaxis.Header = "Y-Axis";
    ///             chartArea.SecondaryAxis = yaxis;
    /// </code>
    /// </example>
    /// <seealso cref="ChartAxesCollection"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartAxis : FrameworkContentElement, IDisposable, IChartSerializer
    {
        /// <summary>
        /// Gets or sets the tick interval.
        /// </summary>
        /// <value>The tick interval.</value>
        public double BaseInterval
        {
            get { return (double)GetValue(BaseIntervalProperty); }
            set { SetValue(BaseIntervalProperty, value); }
        }
        /// <summary>
        /// Identifies the Interval dependency property.
        /// </summary>
        public static readonly DependencyProperty BaseIntervalProperty =
     DependencyProperty.Register("BaseInterval", typeof(double), typeof(ChartAxis), new PropertyMetadata(double.NaN, OnBaseIntervalPropertyChanged, new CoerceValueCallback(OnCoerceInterval)));

        internal DoubleRange InternalRange
        {
            get
            {
                return (DoubleRange)GetValue(InternalRangeProperty);
            }

            set
            {
                SetValue(InternalRangeProperty, value);
            }
        }

        /// <summary>
        /// Identifies the Range dependency property.
        /// </summary>
        internal static readonly DependencyProperty InternalRangeProperty =
         DependencyProperty.Register("InternalRange", typeof(DoubleRange), typeof(ChartAxis), new PropertyMetadata(DoubleRange.Empty, null, new CoerceValueCallback(RangeCoerce)));

        /// <summary>
        /// Enables the SmartAxisLabel
        /// </summary>
        public static readonly DependencyProperty EnableSmartAxisLabelProperty =
          DependencyProperty.Register("EnableSmartAxisLabel", typeof(bool), typeof(ChartAxis), new PropertyMetadata(false, new PropertyChangedCallback(OnEnableSmartAxisLabelChanged)));

        /// <summary>
       /// Gets or Sets the HideRepeatedLabels 
        /// </summary>


        public bool HideRepeatedLabels
        {
            get { return (bool)GetValue(HideRepeatedLabelsProperty); }
            set { SetValue(HideRepeatedLabelsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HideRepeatedLabels.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Hide the RepeatedLabels
        /// </summary>
        public static readonly DependencyProperty HideRepeatedLabelsProperty =
            DependencyProperty.Register("HideRepeatedLabels", typeof(bool), typeof(ChartAxis), new PropertyMetadata(false,OnHideRepeatedLabelsChanged));

        private static void OnHideRepeatedLabelsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null)
            {
                    axis.CalculateVisibleLables();
            }
        }

        /// <summary>
        /// Gets or Sets the SmartAxisLabel 
        /// </summary>
        public bool EnableSmartAxisLabel
        {
            get { return (bool)GetValue(EnableSmartAxisLabelProperty); }
            set { SetValue(EnableSmartAxisLabelProperty, value); }
        }

        /// <summary>
        /// Enables the DoubleDisplayUnit
        /// </summary>
        public static readonly DependencyProperty DoubleDisplayUnitProperty =
          DependencyProperty.Register("DoubleDisplayUnit", typeof(DoubleUnits), typeof(ChartAxis), new PropertyMetadata(DoubleUnits.AutoDetect, new PropertyChangedCallback(OnDoubleDisplayUnitChanged)));

        /// <summary>
        /// Gets or Sets the SmartAxisLabel 
        /// </summary>
        public DoubleUnits DoubleDisplayUnit
        {
            get { return (DoubleUnits)GetValue(DoubleDisplayUnitProperty); }
            set { SetValue(DoubleDisplayUnitProperty, value); }
        }

        /// <summary>
        /// Identifies the LabelTimeSpanFormat
        /// </summary>
        public static readonly DependencyProperty LabelTimeSpanFormatProperty =
          DependencyProperty.Register("LabelTimeSpanFormat", typeof(string), typeof(ChartAxis), new PropertyMetadata("hh:mm:ss",null));

        /// <summary>
        /// Gets or Sets the LabelTimeSpanFormat 
        /// </summary>
        public string LabelTimeSpanFormat
        {
            get { return (string)GetValue(LabelTimeSpanFormatProperty); }
            set { SetValue(LabelTimeSpanFormatProperty, value); }
        }

        /// <summary>
        /// Represents AxisLabelPosition
        /// </summary>
        public static readonly DependencyProperty AxisLabelsPositionProperty =
          DependencyProperty.Register("AxisLabelsPosition", typeof(AxisLabels), typeof(ChartAxis), new PropertyMetadata(AxisLabels.Low, new PropertyChangedCallback(OnOriginPropertyChanged), new CoerceValueCallback(CoerceOriginProperty)));

        /// <summary>
        /// Gets or Sets the AxisLabels 
        /// </summary>
        public AxisLabels AxisLabelsPosition
        {
            get { return (AxisLabels)GetValue(AxisLabelsPositionProperty); }
            set { SetValue(AxisLabelsPositionProperty, value); }
        }


        /// <summary>
        /// Gets or Sets the ShowAllLabels, used to show the labels for all segments not depends on interval.
        /// </summary>
        public bool ShowAllLabels
        {
            get { return (bool)GetValue(ShowAllLabelsProperty); }
            set { SetValue(ShowAllLabelsProperty, value); }
        }

      
        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowAllLabels.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowAllLabelsProperty =
            DependencyProperty.Register("ShowAllLabels", typeof(bool), typeof(ChartAxis), new PropertyMetadata(false,OnShowAllLablesPropertyChanged));

        internal SegmentPositions SegmentPosition
        {
            get { return (SegmentPositions)GetValue(SegmentPositionProperty); }
            set { SetValue(SegmentPositionProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for SegmentPosition.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SegmentPositionProperty =
            DependencyProperty.Register("SegmentPosition", typeof(SegmentPositions), typeof(ChartAxis), new PropertyMetadata(SegmentPositions.BetweenTicks));

        /// <summary>
        /// Get and Set ForceZeroProperty
        /// </summary>
        public bool ForceZero
        {
            get { return (bool)GetValue(ForceZeroProperty); }
            set { SetValue(ForceZeroProperty, value); }
        }

       
       /// <summary>
        ///  Using a DependencyProperty as the backing store for ForceZero.  This enables animation, styling, binding, etc...
       /// </summary>
       public static readonly DependencyProperty ForceZeroProperty =
            DependencyProperty.Register("ForceZero", typeof(bool), typeof(ChartAxis), new PropertyMetadata(false, new PropertyChangedCallback(OnForceZeroPropertyChanged)));

        
      /// <summary>
      /// Handles the Axisline Rendering behavior
      /// </summary>
        public bool AliasedModeRendering
        {
            get { return (bool)GetValue(AliasedModeRenderingProperty); }
            set { SetValue(AliasedModeRenderingProperty, value); }
        }



        internal LogarithmicType LogarithmicIntervalType
        {
            get { return (LogarithmicType)GetValue(LogarithmicIntervalTypeProperty); }
            set { SetValue(LogarithmicIntervalTypeProperty, value); }
        }

        
        /// <summary>
        ///Using a DependencyProperty as the backing store for LogarithmicIntervalType.  This enables animation, styling, binding, etc... 
        /// </summary>
        internal static readonly DependencyProperty LogarithmicIntervalTypeProperty =
            DependencyProperty.Register("LogarithmicIntervalType", typeof(LogarithmicType), typeof(ChartAxis), new UIPropertyMetadata(LogarithmicType.Exponential,OnLogIntervalTypeChanged));



        /// <summary>
        /// Get and Set LogarithmicIntervalProperty
        /// </summary>
        public double LogarithmicInterval
        {
            get { return (double)GetValue(LogarithmicIntervalProperty); }
            set { SetValue(LogarithmicIntervalProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for LogarithmicInterval.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LogarithmicIntervalProperty =
            DependencyProperty.Register("LogarithmicInterval", typeof(double), typeof(ChartAxis), new UIPropertyMetadata(1d,OnLogarithmicIntervalPropertyChanged));



        /// <summary>
        /// Get and Set LabelLogarithmicFormatProperty
        /// </summary>
        public LogarithmicType LabelLogarithmicFormat
        {
            get { return (LogarithmicType)GetValue(LabelLogarithmicFormatProperty); }
            set { SetValue(LabelLogarithmicFormatProperty, value); }
        }

       
       /// <summary>
        ///  Using a DependencyProperty as the backing store for LabelLogarithmicFormat.  This enables animation, styling, binding, etc...
       /// </summary>
        public static readonly DependencyProperty LabelLogarithmicFormatProperty =
            DependencyProperty.Register("LabelLogarithmicFormat", typeof(LogarithmicType), typeof(ChartAxis), new UIPropertyMetadata(LogarithmicType.Exponential,OnLogIntervalTypeChanged));

        
        

        

        // Using a DependencyProperty as the backing store for AliasedModeRendering.  This enables animation, styling, binding, etc...
        /// <summary>
        /// AliasedModeRenderingProperty initialize
        /// </summary>
        public static readonly DependencyProperty AliasedModeRenderingProperty =
            DependencyProperty.Register("AliasedModeRendering", typeof(bool), typeof(ChartAxis), new UIPropertyMetadata(true));

        

        /// <summary>
        /// property for Check DataValueRange is set or not
        /// </summary>
        public bool IsSetDataValueRange
        {
            get { return (bool)GetValue(IsSetDataValueRangeProperty); }
            set { SetValue(IsSetDataValueRangeProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for IsSetDataValueRange.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsSetDataValueRangeProperty =
            DependencyProperty.Register("IsSetDataValueRange", typeof(bool), typeof(ChartAxis), new PropertyMetadata(false, new PropertyChangedCallback(OnIsSetDataValueRangeChanged)));

        /// <summary>
        ///  Identifies the TickLinesPosition dependency property.
        /// </summary>
        public static readonly DependencyProperty TickLinesPositionProperty =
         DependencyProperty.Register("TickLinesPosition", typeof(AxisPositions), typeof(ChartAxis), new PropertyMetadata(AxisPositions.Outside, new PropertyChangedCallback(OnOriginPropertyChanged), new CoerceValueCallback(CoerceOriginProperty)));

        /// <summary>
        /// Gets or Sets the TickLinesPosition 
        /// </summary>
        public AxisPositions TickLinesPosition
        {
            get { return (AxisPositions)GetValue(TickLinesPositionProperty); }
            set { SetValue(TickLinesPositionProperty, value); }
        }

        /// <summary>
        ///  Identifies the TickLinesRange dependency property.
        /// </summary>
        public static readonly DependencyProperty TickLinesRangeProperty =
         DependencyProperty.Register("TickLinesRange", typeof(double), typeof(ChartAxis), new PropertyMetadata(0.5d, new PropertyChangedCallback(OnOriginPropertyChanged), new CoerceValueCallback(CoerceOriginTickProperty)));

        /// <summary>
        /// Gets or Sets the ChartTickLinesPosition 
        /// </summary>
        public double TickLinesRange
        {
            get { return (double)GetValue(TickLinesRangeProperty); }
            set { SetValue(TickLinesRangeProperty, value); }
        }

        /// <summary>
        ///  Identifies the SmallTickLinesRange dependency property.
        /// </summary>
        public static readonly DependencyProperty SmallTickLinesRangeProperty =
         DependencyProperty.Register("SmallTickLinesRange", typeof(double), typeof(ChartAxis), new PropertyMetadata(0.5d, new PropertyChangedCallback(OnOriginPropertyChanged), new CoerceValueCallback(CoerceOriginTickProperty)));

        /// <summary>
        /// Gets or Sets the ChartTickLinesPosition 
        /// </summary>
        public double SmallTickLinesRange
        {
            get { return (double)GetValue(SmallTickLinesRangeProperty); }
            set { SetValue(SmallTickLinesRangeProperty, value); }
        }

        /// <summary>
        ///  Identifies the LabelPosition dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelPositionProperty =
         DependencyProperty.Register("LabelPosition", typeof(LabelPositions), typeof(ChartAxis), new PropertyMetadata(LabelPositions.Outside, new PropertyChangedCallback(OnOriginPropertyChanged), new CoerceValueCallback(CoerceOriginProperty)));

        /// <summary>
        /// Gets or Sets the LabelPosition
        /// </summary>
        public LabelPositions LabelPosition
        {
            get { return (LabelPositions)GetValue(LabelPositionProperty); }
            set { SetValue(LabelPositionProperty, value); }
        }

               
        /// <summary>
        /// Get and Set InteractiveCursorLabelContentProperty
        /// </summary>
        public InteractiveCursorLabelContent InteractiveCursorLabelContent
        {
            get { return (InteractiveCursorLabelContent)GetValue(InteractiveCursorLabelContentProperty); }
            set { SetValue(InteractiveCursorLabelContentProperty, value); }
        }

       
        /// <summary>
        ///  Using a DependencyProperty as the backing store for InteractiveCursorLabelContent.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty InteractiveCursorLabelContentProperty =
            DependencyProperty.Register("InteractiveCursorLabelContent", typeof(InteractiveCursorLabelContent), typeof(ChartAxis), new PropertyMetadata(null));

        internal Visibility InteractiveCursorLabelVisibility
        {
            get { return (Visibility)GetValue(InteractiveCursorLabelVisibilityProperty); }
            set { SetValue(InteractiveCursorLabelVisibilityProperty, value); }
        }

        
        /// <summary>
        ///Using a DependencyProperty as the backing store for InteractiveCursorLabelContent.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty InteractiveCursorLabelVisibilityProperty =
            DependencyProperty.Register("InteractiveCursorLabelVisibility", typeof(Visibility), typeof(ChartAxis), new UIPropertyMetadata(Visibility.Hidden));

        /// <summary>
        /// Get and Set InteractiveCursorLabelLeftPositionProperty
        /// </summary>
        public double InteractiveCursorLabelLeftPosition
        {
            get { return (double)GetValue(InteractiveCursorLabelLeftPositionProperty); }
            set { SetValue(InteractiveCursorLabelLeftPositionProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for InteractiveCursorLabelLeftPosition.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty InteractiveCursorLabelLeftPositionProperty =
            DependencyProperty.Register("InteractiveCursorLabelLeftPosition", typeof(double), typeof(ChartAxis), new UIPropertyMetadata(0d));



        /// <summary>
        /// Get and Set the InteractiveCursorLabelTopPosition property
        /// </summary>
        public double InteractiveCursorLabelTopPosition
        {
            get { return (double)GetValue(InteractiveCursorLabelTopPositionProperty); }
            set { SetValue(InteractiveCursorLabelTopPositionProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for InteractiveCursorLabelTopPosition.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty InteractiveCursorLabelTopPositionProperty =
            DependencyProperty.Register("InteractiveCursorLabelTopPosition", typeof(double), typeof(ChartAxis), new UIPropertyMetadata(0d));


        /// <summary>
        /// Get and Set InteractiveCursorTemplateProperty
        /// </summary>
        public DataTemplate InteractiveCursorTemplate
        {
            get { return (DataTemplate)GetValue(InteractiveCursorTemplateProperty); }
            set { SetValue(InteractiveCursorTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InterractiveCursorTemplate.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty InteractiveCursorTemplateProperty =
            DependencyProperty.Register("InteractiveCursorTemplate", typeof(DataTemplate), typeof(ChartAxis), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or Sets the Interactive cursor LabelForeground color
        /// </summary>
        internal Brush InteractiveCursorLabelForeground
        {
            get { return (Brush)GetValue(InteractiveCursorLabelForegroundProperty); }
            set { SetValue(InteractiveCursorLabelForegroundProperty, value); }
        }
 
        // Using a DependencyProperty as the backing store for InteractiveCursorLabelForeground.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty InteractiveCursorLabelForegroundProperty =
            DependencyProperty.Register("InteractiveCursorLabelForeground", typeof(Brush), typeof(ChartAxis), new UIPropertyMetadata(Brushes.Black));

        /// <summary>
        /// Gets or Sets the Interactive cursor LabelBackground color
        /// </summary>

        internal Brush InteractiveCursorLabelBackground
        {
            get { return (Brush)GetValue(InteractiveCursorLabelBackgroundProperty); }
            set { SetValue(InteractiveCursorLabelBackgroundProperty, value); }
        }

        
        
        // Using a DependencyProperty as the backing store for InteractiveCursorLabelBackground.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty InteractiveCursorLabelBackgroundProperty =
            DependencyProperty.Register("InteractiveCursorLabelBackground", typeof(Brush), typeof(ChartAxis), new UIPropertyMetadata(Brushes.Gray));

       


        /// <summary>
        /// Property to set the IsOpen property of the Interactive cursor label popup
        /// </summary>
        internal bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StaysOpen.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(ChartAxis), new UIPropertyMetadata(false));

        


        internal Thickness InteractiveCursorMargin
        {
            get { return (Thickness)GetValue(InteractiveCursorMarginProperty); }
            set { SetValue(InteractiveCursorMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InteractiveCursorMargin.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty InteractiveCursorMarginProperty =
            DependencyProperty.Register("InteractiveCursorMargin", typeof(Thickness), typeof(ChartAxis), new UIPropertyMetadata(new Thickness()));


        /// <summary>
        /// Gets or sets the ChartAxisPanel Margin Value 
        /// </summary>
        public Thickness Margin
        {
            get { return (Thickness)GetValue(MarginProperty); }
            set { SetValue(MarginProperty, value); }
        }


        /// <summary>
        ///  Identifies the Margin dependency property.
        /// </summary>
        public static readonly DependencyProperty MarginProperty =
            DependencyProperty.Register("Margin", typeof(Thickness), typeof(ChartAxis), new UIPropertyMetadata(new Thickness(0.0)));



        /// <summary>
        /// Property for Move ChartSegmnets 
        /// </summary>
        public DoubleRange AdditionalPadding
        {
            get { return (DoubleRange)GetValue(AdditionalPaddingProperty); }
            set { SetValue(AdditionalPaddingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AdditionalPadding.  This enables animation, styling, binding, etc...
        /// <summary>
        /// AdditionalPaddingProperty initialization
        /// </summary>
        public static readonly DependencyProperty AdditionalPaddingProperty =
            DependencyProperty.Register("AdditionalPadding", typeof(DoubleRange), typeof(ChartAxis), new PropertyMetadata(new DoubleRange(0, 0, false), OnAdditionalPaddingChanged, new CoerceValueCallback(OnCoerceAdditionalPadding)));

       
        private static void OnAdditionalPaddingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        /// <summary>
        /// Identifies the AutoScrollingDelta dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoScrollingDeltaProperty =
          DependencyProperty.Register("AutoScrollingDelta", typeof(double), typeof(ChartAxis), new PropertyMetadata(0d, new PropertyChangedCallback(OnAutoScrollingDeltaChanged)));

        /// <summary>
        /// Gets or sets the delta value which determines the range of value to be visible during autoscrolling
        /// </summary>
        public double AutoScrollingDelta
        {
            get
            {
                return (double)GetValue(AutoScrollingDeltaProperty);
            }

            set
            {
                SetValue(AutoScrollingDeltaProperty, value);
            }
        }

        /// <summary>
        /// Identifies the EnableAutoScrolling dependency property.
        /// </summary>
        public static readonly DependencyProperty EnableAutoScrollingProperty =
          DependencyProperty.Register("EnableAutoScrolling", typeof(bool?), typeof(ChartAxis), new PropertyMetadata(null, new PropertyChangedCallback(OnEnableAutoScrollingChanged)));

        /// <summary>
        /// Gets or sets the EnableAutoScrolling value to enable the auto scrolling
        /// </summary>
        public bool? EnableAutoScrolling
        {
            get
            {
                return (bool?)GetValue(EnableAutoScrollingProperty);
            }

            set
            {
                SetValue(EnableAutoScrollingProperty, value);
            }
        }

		/// <summary>
        /// Identifies the BreakRange dependency property.
        /// </summary>
        public static readonly DependencyProperty BreakRangeProperty =
            DependencyProperty.Register("BreakRange", typeof(ChartBreakRange), typeof(ChartAxis), new PropertyMetadata(null));

		/// <summary>
        /// Gets or sets the BreakRange value to add the break ranges
        /// </summary>
        public ChartBreakRange BreakRange
        {
            get
            {
                return (ChartBreakRange)GetValue(BreakRangeProperty);
            }
            set
            {
                SetValue(BreakRangeProperty, value);
            }
        }

        internal Brush tempForeground = Brushes.Black;
        
        /// <summary>
        /// Method for hooking RangeChanged event
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="parentAxis"></param>
        public void Clone(ChartAxis axis, ChartAxis parentAxis)
        {
            axis.RangeChanged += parentAxis.RangeChanged;
        }

        #region Constants
      
        /// <summary>
        /// Initializes c_roundDecimals
        /// </summary>
        private const int C_roundDecimals = 6;

        /// <summary>
        /// Initializes c_intervalDivs
        /// </summary>
        private readonly static double[] c_intervalDivs = new double[] { 1d, 2d, 3d, 5d };

        /// <summary>
        /// Initializes m_cachedRangeValue
        /// </summary>

        internal DoubleRange m_previousRange;
        private DoubleRange m_cachedRangeValue;
        internal DoubleRange m_actualRangeValue;
        internal bool m_showAllLabelsChanged = false;
        #endregion

        #region Members

        
        internal bool depthaxisflag = false;
        internal double lastPosition_X = 0d;
        internal double lasPosition_Y = 0d;
        internal bool checkDateTimeIntervalChange = false;

        /// <summary>
        /// Initializes m_isUpdating
        /// </summary>
        private bool m_isUpdating;

        /// <summary>
        /// Initializes tempDateTimeInterval
        /// </summary>
        private TimeSpan m_tempDateTimeInterval;


        /// <summary>
        /// Initializes m_customLables
        /// </summary>
        private ChartAxisLabelsCollection m_customLables = new ChartAxisLabelsCollection();

        /// <summary>
        /// Initializes m_visibleLables
        /// </summary>
        internal ChartAxisLabelsCollection m_visibleLables = new ChartAxisLabelsCollection();

        /// <summary>
        /// Initializes m_logarihmicVisibleRange
        /// </summary>
        private DoubleRange m_logarihmicVisibleRange = DoubleRange.Empty;

        /// <summary>
        /// Initializes m_visibleRange
        /// </summary>
        internal DoubleRange m_visibleRange = DoubleRange.Empty;

        /// <summary>
        /// Initializes m_visibleIntervalOffset
        /// </summary>
        private double m_visibleIntervalOffset;

        /// <summary>
        /// Initializes m_visibleInterval
        /// </summary>
        internal double m_visibleInterval = double.NaN;

        /// <summary>
        /// Initializes m_ticksCount
        /// </summary>
        internal int m_ticksCount = 0;

        /// <summary>
        /// Initializes m_niceRange
        /// </summary>
        private DoubleRange m_niceRange = DoubleRange.Empty;

        /// <summary>
        /// Initializes m_niceIntervalOffset
        /// </summary>
        private double m_niceIntervalOffset;

        /// <summary>
        /// Initializes m_niceInterval
        /// </summary>
        private double m_niceInterval = double.NaN;

        /// <summary>
        /// Initializes m_requestedRange
        /// </summary>
        private DoubleRange m_requestedRange = DoubleRange.Empty;

        /// <summary>
        /// Initializes m_owner
        /// </summary>
        private ChartArea m_owner;

        /// <summary>
        /// Initialize the axis Label Source data
        /// </summary>
        private ChartDataModel m_AxisDataModel = new ChartDataModel();

        /// <summary>
        /// Initializes m_ticksPoint
        /// </summary>
        private ArrayList m_ticksPoint = new ArrayList();

        /// <summary>
        /// to set the isIndexed property of series.
        /// </summary>
        private static bool isIndexedOfPrimary = false;

        /// <summary>
        /// to set the arranged property of series.
        /// </summary>
        internal bool arranged = false;

        /// <summary>
        /// hold the axis height
        /// </summary>
        internal double axisHeight = 0.0;

        internal bool isNeedUpdate = false;

        internal double XLabelOffset = 0d;
        internal double YLabelOffset = 0d;
        internal double gridheight = 0d;
        internal double gridWidth = 0d;
        internal double OutOfRangevalue = 1d;
        internal double m_autoScrollingZoomFactor = 0d;
        #endregion

        #region Events
        /// <summary>
        /// Occurs when axis was changed.
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// Occurs when axis range is changed.
        /// </summary>
        public event ChartAxisRangeEventHandler RangeChanged;
        internal void OnRangeChanged(ChartAxisRangeArgs args)
        {
            this.isNeedUpdate = true;
            if (RangeChanged != null)
            {
                if(this.IsLoaded)
                RangeChanged(this, args);
            }
        }
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies the Logarithmic Range dependency property
        /// </summary>
        public static readonly DependencyProperty LogarithmicRangeProperty =
DependencyProperty.Register("LogarithmicRange", typeof(DoubleRange), typeof(ChartAxis), new PropertyMetadata(DoubleRange.Empty, new PropertyChangedCallback(OnLogRangeChanged), new CoerceValueCallback(OnCoerceLogRange)));

        /// <summary>
        /// Identifies the DateTimeInterval dependency property.
        /// </summary>
        public static readonly DependencyProperty IgnoreRangePaddingsOnZoomProperty =
            DependencyProperty.Register("IgnoreRangePaddingsOnZoom", typeof(bool), typeof(ChartAxis), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the EnableLogLabels dependency property.
        /// </summary>
        public static readonly DependencyProperty EnableLogLabelsProperty =
            DependencyProperty.Register("EnableLogLabels", typeof(bool), typeof(ChartAxis), new PropertyMetadata(false, new PropertyChangedCallback(Onchange)));

        /// <summary>
        /// Identifies the DateTimeInterval dependency property.
        /// </summary>
        public static readonly DependencyProperty DateTimeIntervalProperty =
            DependencyProperty.Register("DateTimeInterval", typeof(TimeSpan), typeof(ChartAxis), new UIPropertyMetadata(new TimeSpan(), new PropertyChangedCallback(OnDateTimeIntervalPropertyChanged)));

        /// <summary>
        /// Identifies the MinimumDateTimeInterval dependency property.
        /// </summary>    
        public static readonly DependencyProperty MinimumDateTimeIntervalProperty =
        DependencyProperty.Register("MinimumDateTimeInterval", typeof(TimeSpan), typeof(ChartAxis), new FrameworkPropertyMetadata(TimeSpan.Zero, new PropertyChangedCallback(OnMinimumDateTimeIntervalPropertyChanged)));

        /// <summary>
        /// Identifies the DatTimeRange dependency property.
        /// </summary>
        public static readonly DependencyProperty DateTimeRangeProperty =
            DependencyProperty.Register("DateTimeRange", typeof(DateTimeRange), typeof(ChartAxis), new PropertyMetadata(new DateTimeRange(), new PropertyChangedCallback(OnDateTimeRangeChanged), new CoerceValueCallback(OnCoerceDateTimeRange)));

        /// <summary>
        /// Identifies the Range dependency property.
        /// </summary>
        public static readonly DependencyProperty RangeProperty =
         DependencyProperty.Register("Range", typeof(DoubleRange), typeof(ChartAxis), new PropertyMetadata(DoubleRange.Empty, new PropertyChangedCallback(OnRangeChanged), new CoerceValueCallback(OnCoerceRange)));

        /// <summary>
        /// Identifies the Origin dependency property.
        /// </summary>
        public static readonly DependencyProperty OriginProperty =
          DependencyProperty.Register("Origin", typeof(double), typeof(ChartAxis), new PropertyMetadata(0d, OnOriginPropertyChanged, new CoerceValueCallback(CoerceOriginProperty)));

        /// <summary>
        /// Identifies the ZoomPosition dependency property.
        /// </summary>
        public static readonly DependencyProperty ZoomPositionProperty =
          DependencyProperty.Register("ZoomPosition", typeof(double), typeof(ChartAxis), new PropertyMetadata(0d, null, new CoerceValueCallback(OnCoerceZoomPosition)));

        /// <summary>
        /// Identifies the ZoomFactor dependency property.
        /// </summary>
        public static readonly DependencyProperty ZoomFactorProperty =
          DependencyProperty.Register("ZoomFactor", typeof(double), typeof(ChartAxis), new PropertyMetadata(1d, new PropertyChangedCallback(OnZoomFactorValueChanged), new CoerceValueCallback(OnCoerceZoomFactor)));

        /// <summary>
        /// Identifies the IsInversed dependency property.
        /// </summary>
        public static readonly DependencyProperty IsInversedProperty =
          DependencyProperty.Register("IsInversed", typeof(bool), typeof(ChartAxis), new PropertyMetadata(false, new PropertyChangedCallback(OnIsInversedChanged)));

        private static void OnIsInversedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null)
            {
                axis.isNeedUpdate = true;
                if (axis.Area != null && axis.Area.Series != null && axis.Area.Series.Count > 0)
                {
                    foreach (ChartSeries series in axis.Area.Series)
                    {
                        if (series.Adornments != null)
                        {
                            series.ChartType.Update(series);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Identifies the IsAutoSetRange dependency property.
        /// </summary>
        //public static readonly DependencyProperty IsAutoSetRangeProperty =
        //  DependencyProperty.Register("IsAutoSetRange", typeof(bool), typeof(ChartAxis), new ChartPropertyMetadata(true, ChartPropertyMetadataOptions.AffectsUpdate));

        public static readonly DependencyProperty IsAutoSetRangeProperty =
        DependencyProperty.Register("IsAutoSetRange", typeof(bool), typeof(ChartAxis), new PropertyMetadata(true, new PropertyChangedCallback(OnAutoSetRangeChanged), new CoerceValueCallback(OnCoerceAutoSetRange)));

        
        /// <summary>
        /// Identifies the EnableAutoIntervalOnZooming dependency property.
        /// </summary>
        public static readonly DependencyProperty EnableAutoIntervalOnZoomingProperty =
          DependencyProperty.Register("EnableAutoIntervalOnZooming", typeof(bool), typeof(ChartAxis), new PropertyMetadata(true));

        /// <summary>
        /// Identifies the EnableZooming dependency property.
        /// </summary>
        public static readonly DependencyProperty EnableZoomingProperty =
          DependencyProperty.Register("EnableZooming", typeof(bool), typeof(ChartAxis), new PropertyMetadata(true, OnEnableZoomChanged));

        /// <summary>
        /// Identifies the LogarithmicBase dependency property.
        /// </summary>
        public static readonly DependencyProperty LogarithmicBaseProperty =
            DependencyProperty.Register("LogarithmicBase", typeof(double), typeof(ChartAxis), new PropertyMetadata(10d, OnLogarithmicBasePropertyChanged));

        /// <summary>
        /// Identifies the RangePadding dependency property.
        /// </summary>
        public static readonly DependencyProperty RangePaddingProperty =
          DependencyProperty.Register("RangePadding", typeof(ChartRangePaddingType), typeof(ChartAxis), new PropertyMetadata(ChartRangePaddingType.Normal, OnRangePaddingPropertyChanged));

        /// <summary>
        /// Identifies the LineStroke dependency property.
        /// </summary>
        public static readonly DependencyProperty LineStrokeProperty =
          DependencyProperty.Register("LineStroke", typeof(Pen), typeof(ChartAxis), new FrameworkPropertyMetadata(new Pen(new SolidColorBrush(Color.FromArgb(0xFF, 0X94, 0X94, 0X94)), 1), FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnLineStrokeChanged)));
        
        /// <summary>
        /// Identifies the LineStroke dependency property.
        /// </summary>
        public static readonly DependencyProperty IsOriginCenteredProperty =
          DependencyProperty.Register("IsOriginCentered", typeof(bool), typeof(ChartAxis), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnIsOriginCenteredChanged)));

        /// <summary>
        /// Identifies the TickLineStroke dependency property.
        /// </summary>
        public static readonly DependencyProperty TickLineStrokeProperty =
            DependencyProperty.Register("TickLineStroke", typeof(Pen), typeof(ChartAxis), new FrameworkPropertyMetadata(new Pen(Brushes.Black, 1), FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnTickStrokeChanged)));

        /// <summary>
        ///  Identifies the SmallTickLineStroke dependency property.
        /// </summary>
        public static readonly DependencyProperty SmallTickLineStrokeProperty =
        DependencyProperty.Register("SmallTickLineStroke", typeof(Pen), typeof(ChartAxis), new FrameworkPropertyMetadata(new Pen(Brushes.Black, 1), FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnSmallTickStrokeChanged)));

        /// <summary>
        /// Identifies the Orientation dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
          DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ChartAxis), new PropertyMetadata(Orientation.Horizontal, OnOrientationChanged));

        /// <summary>
        /// Identifies the AxisVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty AxisVisibilityProperty =
   DependencyProperty.Register("AxisVisibility", typeof(Visibility), typeof(ChartAxis), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Identifies the MaxNumberOfLabels dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxNumberOfLabelsProperty =
   DependencyProperty.Register("MaxNumberOfLabels", typeof(int), typeof(ChartAxis), new PropertyMetadata(0));

        //  public static readonly DependencyProperty EnableMaxNumberOfLabelsProperty =
        //DependencyProperty.Register("EnableMaxNumberOfLabels", typeof(bool), typeof(ChartAxis), new PropertyMetadata(false, OnEnableMaxNumberOfLabelsChanged));


        /// <summary>
        /// Identifies the OpposedPosition dependency property.
        /// </summary>
        public static readonly DependencyProperty OpposedPositionProperty =
          DependencyProperty.Register("OpposedPosition", typeof(bool), typeof(ChartAxis), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the TickSize dependency property.
        /// </summary>
        public static readonly DependencyProperty TickSizeProperty =
          DependencyProperty.Register("TickSize", typeof(double), typeof(ChartAxis), new PropertyMetadata(4d, new PropertyChangedCallback(OnTickSizeChange)));

        /// <summary>
        /// Identifies the SmallTickSize dependency property.
        /// </summary>
        public static readonly DependencyProperty SmallTickSizeProperty =
          DependencyProperty.Register("SmallTickSize", typeof(double), typeof(ChartAxis), new PropertyMetadata(3d, new PropertyChangedCallback(OnsizeChange)));

        /// <summary>
        /// Identifies the SmallTicksPerInterval dependency property.
        /// </summary>
        public static readonly DependencyProperty SmallTicksPerIntervalProperty =
          DependencyProperty.Register("SmallTicksPerInterval", typeof(int), typeof(ChartAxis), new FrameworkPropertyMetadata(0, new PropertyChangedCallback(OnSmallTicksPerIntervalPropertyChanged)));

        /// <summary>
        /// Identifies the LabelRotateAngle dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelRotateAngleProperty =
          DependencyProperty.Register("LabelRotateAngle", typeof(double), typeof(ChartAxis), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the LabelHorizontalAlignment dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelHorizontalAlignmentProperty =
            DependencyProperty.Register("LabelHorizontalAlignment", typeof(HorizontalAlignment), typeof(ChartAxis), new UIPropertyMetadata(HorizontalAlignment.Stretch, new PropertyChangedCallback(OnLabelAlignmentChanged)));

        /// <summary>
        /// Identifies the LabelVerticalAlignment dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelVerticalAlignmentProperty =
            DependencyProperty.Register("LabelVerticalAlignment", typeof(VerticalAlignment), typeof(ChartAxis), new UIPropertyMetadata(VerticalAlignment.Stretch, new PropertyChangedCallback(OnLabelAlignmentChanged)));

        /// <summary>
        /// Identifies the LabelHeight dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelHeightProperty =
            DependencyProperty.Register("LabelHeight", typeof(double), typeof(ChartAxis), new UIPropertyMetadata(new PropertyChangedCallback(OnLabelSizeChanged)));

        /// <summary>
        /// Identifies the LabelWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelWidthProperty =
            DependencyProperty.Register("LabelWidth", typeof(double), typeof(ChartAxis), new UIPropertyMetadata(new PropertyChangedCallback(OnLabelSizeChanged)));

        /// <summary>
        /// Identifies the LabelBackground dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelBackgroundProperty =
            DependencyProperty.Register("LabelBackground", typeof(Brush), typeof(ChartAxis), new UIPropertyMetadata(Brushes.Transparent));

        /// <summary>
        /// Identifies the LabelForeground dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelForegroundProperty =
            DependencyProperty.Register("LabelForeground", typeof(Brush), typeof(ChartAxis), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0X33,0X33,0X33))));

        /// <summary>
        /// Identifies the LabelFontFamily dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelFontFamilyProperty =
            DependencyProperty.Register("LabelFontFamily", typeof(FontFamily), typeof(ChartAxis), new UIPropertyMetadata(new FontFamily("Segoe UI")));

        /// <summary>
        /// Identifies the LabelFontWeight dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelFontWeightProperty =
            DependencyProperty.Register("LabelFontWeight", typeof(FontWeight), typeof(ChartAxis), new UIPropertyMetadata(SystemFonts.MessageFontWeight));

        /// <summary>
        /// Identifies the LabelFontSize dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelFontSizeProperty =
            DependencyProperty.Register("LabelFontSize", typeof(double), typeof(ChartAxis), new UIPropertyMetadata(SystemFonts.MessageFontSize));

        /// <summary>
        /// Identifies the LabelBorderThickness dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelBorderThicknessProperty =
            DependencyProperty.Register("LabelBorderThickness", typeof(Thickness), typeof(ChartAxis), new UIPropertyMetadata(new Thickness()));

        /// <summary>
        /// Identifies the LabelBorderBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelBorderBrushProperty =
            DependencyProperty.Register("LabelBorderBrush", typeof(Brush), typeof(ChartAxis), new UIPropertyMetadata(Brushes.Transparent));

        /// <summary>
        /// Identifies the LabelCornerRadius dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelCornerRadiusProperty =
            DependencyProperty.Register("LabelCornerRadius", typeof(CornerRadius), typeof(ChartAxis), new UIPropertyMetadata(new CornerRadius()));

        /// <summary>
        /// Identifies the DesiredIntervalCOunt dependency property.
        /// </summary>
        public static readonly DependencyProperty DesiredIntervalsCountProperty =
          DependencyProperty.Register("DesiredIntervalsCount", typeof(int), typeof(ChartAxis), new PropertyMetadata(6, OnDesiredIntervalsCountPropertyChanged, CoerceDesiredIntervalsCountProperty));

        /// <summary>
        /// Identifies the Header dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
          HeaderedContentControl.HeaderProperty.AddOwner(typeof(ChartAxis), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the ValueType dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueTypeProperty =
          DependencyProperty.Register("ValueType", typeof(ChartValueType), typeof(ChartAxis), new PropertyMetadata(ChartValueType.Double, OnValuetypeChanged));

        /// <summary>
        /// Identifies the IsInversed dependency property.
        /// </summary>
        public static readonly DependencyProperty IsLogarithmicLabelsProperty =
          DependencyProperty.Register("IsLogarithmicLabels", typeof(bool), typeof(ChartAxis), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the IntersectAction dependency property.
        /// </summary>
        public static readonly DependencyProperty IntersectActionProperty =
          DependencyProperty.Register("IntersectAction", typeof(ChartLabelIntersectAction), typeof(ChartAxis), new PropertyMetadata(ChartLabelIntersectAction.Hide));

        /// <summary>
        /// Identifies the Headeralignment dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderAlignmentProperty =
          DependencyProperty.Register("HeaderAlignment", typeof(ChartAlignment), typeof(ChartAxis), new PropertyMetadata(ChartAlignment.Center));

        /// <summary>
        /// Identifies the DoubleDisplayUnitAlignment dependency property.
        /// </summary>
        public static readonly DependencyProperty DoubleDisplayUnitAlignmentProperty =
          DependencyProperty.Register("DoubleDisplayUnitAlignment", typeof(ChartAlignment), typeof(ChartAxis), new PropertyMetadata(ChartAlignment.Near));



        /// <summary>
        /// Get and Set DoubleDisplayUnitVisibilityProperty
        /// </summary>
        public Visibility DoubleDisplayUnitVisibility
        {
            get { return (Visibility)GetValue(DoubleDisplayUnitVisibilityProperty); }
            set { SetValue(DoubleDisplayUnitVisibilityProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for DoubleDisplayUnitVisibility.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DoubleDisplayUnitVisibilityProperty =
            DependencyProperty.Register("DoubleDisplayUnitVisibility", typeof(Visibility), typeof(ChartAxis), new PropertyMetadata(Visibility.Visible));

        

        /// <summary>
        /// Identifies the HeaderPosition dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderPositionProperty =
          DependencyProperty.Register("HeaderPosition", typeof(HeaderPositions), typeof(ChartAxis), new PropertyMetadata(HeaderPositions.Outside));

        /// <summary>
        /// Identifies the LabelTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelTemplateProperty =
          DependencyProperty.Register("LabelTemplate", typeof(DataTemplate), typeof(ChartAxis), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the Interval dependency property.
        /// </summary>
        public static readonly DependencyProperty IntervalProperty =
     DependencyProperty.Register("Interval", typeof(double), typeof(ChartAxis), new PropertyMetadata(double.NaN, OnIntervalPropertyChanged));

        /// <summary>
        /// Identifies the MinimumInterval dependency property.
        /// </summary>    
        public static readonly DependencyProperty MinimumIntervalProperty =
        DependencyProperty.Register("MinimumInterval", typeof(double), typeof(ChartAxis), new FrameworkPropertyMetadata(double.NaN, new PropertyChangedCallback(OnMinimumIntervalPropertyChanged)));

        /// <summary>
        /// Identifies the IntervalOffset dependency property.
        /// </summary>
        public static readonly DependencyProperty IntervalOffsetProperty =
          DependencyProperty.Register("IntervalOffset", typeof(double), typeof(ChartAxis), new PropertyMetadata(double.NaN));

        /// <summary>
        /// Identifies the LabelFormat dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelFormatProperty =
          DependencyProperty.Register("LabelFormat", typeof(string), typeof(ChartAxis), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Identifies the LabelDateTimeFormat dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelDateTimeFormatProperty =
          DependencyProperty.Register("LabelDateTimeFormat", typeof(string), typeof(ChartAxis), new PropertyMetadata("g"));

        /// <summary>
        /// Identifies the LabelsSource dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelsSourceProperty =
          DependencyProperty.Register("LabelsSource", typeof(object), typeof(ChartAxis), new PropertyMetadata(null, OnLabelSourceChanged));

        /// <summary>
        /// Identifies the PositionPath dependency property.
        /// </summary>
        public static readonly DependencyProperty PositionPathProperty =
          DependencyProperty.Register("PositionPath", typeof(string), typeof(ChartAxis), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the ContentPath dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentPathProperty =
          DependencyProperty.Register("ContentPath", typeof(string), typeof(ChartAxis), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the Indexed dependency property.
        /// </summary>
        internal static readonly DependencyProperty IndexedProperty =
          DependencyProperty.Register("Indexed", typeof(bool), typeof(ChartAxis), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the LabelsMode dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelsModeProperty =
          DependencyProperty.Register("LabelsMode", typeof(ChartAxisLabelsMode), typeof(ChartAxis), new PropertyMetadata(ChartAxisLabelsMode.Default));

        /// <summary>
        /// Identifies the LabelsPrefix dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelsPrefixProperty =
        DependencyProperty.Register("LabelsPrefix", typeof(DataTemplate), typeof(ChartAxis), new PropertyMetadata(null));
 
        /// <summary>
        /// Identifies the LabelsPostfix dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelsPostfixProperty =
        DependencyProperty.Register("LabelsPostfix", typeof(DataTemplate), typeof(ChartAxis), new PropertyMetadata(null));
         
        /// <summary>
        /// Identifies the VisibleRange dependency property key.
        /// </summary>
        private static readonly DependencyPropertyKey VisibleRangePropertyKey =
          DependencyProperty.RegisterReadOnly("VisibleRange", typeof(DoubleRange), typeof(ChartAxis), new PropertyMetadata(DoubleRange.Empty, OnVisibleRangeChanged));

        /// <summary>
        /// Identifies the VisibleRange dependency property.
        /// </summary>
        public static readonly DependencyProperty VisibleRangeProperty = VisibleRangePropertyKey.DependencyProperty;

        /// <summary>
        /// Identifies the VisibleInterval dependency property key.
        /// </summary>
        private static readonly DependencyPropertyKey VisibleIntervalPropertyKey =
          DependencyProperty.RegisterReadOnly("VisibleInterval", typeof(double), typeof(ChartAxis), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the VisibleInterval dependency property.
        /// </summary>
        public static readonly DependencyProperty VisibleIntervalProperty = VisibleIntervalPropertyKey.DependencyProperty;

        /// <summary>
        /// Identifies the VisibleIntervalOffset dependency property key.
        /// </summary>
        private static readonly DependencyPropertyKey VisibleIntervalOffsetPropertyKey =
          DependencyProperty.RegisterReadOnly("VisibleIntervalOffset", typeof(double), typeof(ChartAxis), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the VisibleIntervalOffset dependency property.
        /// </summary>
        public static readonly DependencyProperty VisibleIntervalOffsetProperty = VisibleIntervalOffsetPropertyKey.DependencyProperty;

        /// <summary>
        /// Identifies the IsFractionEnabledOnZoom dependency property.
        /// </summary>
        public static readonly DependencyProperty IsFractionEnabledOnZoomProperty =
            DependencyProperty.Register("IsFractionEnabledOnZoom", typeof(bool), typeof(ChartAxis), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the HidePartialLabel dependency property.
        /// </summary>
        public static readonly DependencyProperty HidePartialLabelProperty =
            DependencyProperty.Register("HidePartialLabel", typeof(bool), typeof(ChartAxis), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies the EdgeLabelsDrawingMode dependency property.
        /// </summary>
        public static readonly DependencyProperty EdgeLabelsDrawingModeProperty =
            DependencyProperty.Register("EdgeLabelsDrawingMode", typeof(EdgeLabelsDrawingMode), typeof(ChartAxis), new UIPropertyMetadata(EdgeLabelsDrawingMode.Center));

        /// <summary>
        /// Identifies the RangeCalculationMode dependency property.
        /// </summary>
        public static readonly DependencyProperty RangeCalculationModeProperty =
     DependencyProperty.Register("RangeCalculationMode", typeof(RangeCalculationMode), typeof(ChartAxis), new PropertyMetadata(RangeCalculationMode.ConsistentAcrossChartTypes, OnRangeCalcultaionModeChanged));

        /// <summary>
        ///  Identifies the EnableBreaks dependency property.
        /// </summary>
        public static readonly DependencyProperty EnableBreaksProperty =
            DependencyProperty.Register("EnableBreaks", typeof(bool), typeof(ChartAxis), new PropertyMetadata(false, new PropertyChangedCallback(OnEnableBreaksChanged)));

        /// <summary>
        ///  Identifies the InteractiveCursorContentVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty InteractiveCursorContentVisibilityProperty = DependencyProperty.Register("InteractiveCursorContentVisibility", typeof(bool), typeof(ChartAxis), new PropertyMetadata(true));
        /// <summary>
        /// Gets or sets the User defined template for Vertical Label.
        /// </summary>
        public bool InteractiveCursorContentVisibility
        {
            get { return (bool)GetValue(InteractiveCursorContentVisibilityProperty); }
            set { SetValue(InteractiveCursorContentVisibilityProperty, value); }
        }
        #endregion

        #region Properties

        /// <summary>
        /// Get and Set EnableBreaksProperty
        /// </summary>
        public bool EnableBreaks
        {
            get 
            {
                if (ZoomFactor != 1D || ( this.BreakRange != null && this.BreakRange.BreaksMode == ChartBreaksModes.Auto && this.ValueType != ChartValueType.Double))
                    return false;
                else
                    return (bool)GetValue(EnableBreaksProperty); 
            }
            set 
            { 
                SetValue(EnableBreaksProperty, value);
            }
        }

        #region Appearance
        /// <summary>
        /// Gets or sets a value indicating mode that controls partially visible labels behaviour.
        /// </summary>
        /// <value><c>true</c> if partial labels should be hidden; otherwise, <c>false</c>.</value>
        [Category("Appearance")]
        public EdgeLabelsDrawingMode EdgeLabelsDrawingMode
        {
            get { return (EdgeLabelsDrawingMode)GetValue(EdgeLabelsDrawingModeProperty); }
            set { SetValue(EdgeLabelsDrawingModeProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether labels that appear partially should be hidden.
        /// </summary>
        /// <value><c>true</c> if partial labels should be hidden; otherwise, <c>false</c>.</value>
        [Category("Appearance")]
        public bool HidePartialLabel
        {
            get { return (bool)GetValue(HidePartialLabelProperty); }
            set { SetValue(HidePartialLabelProperty, value); }
        }
        
        /// <summary>
        /// This property only for Indexed axis.
        /// RangeCalculationMode is AdjustAcrossChartTypes - Segment position between ticks by default.
        /// RangeCalculationMode is ConsistentAcrossChartTypes - Segment position on ticks by default.
        /// </summary>
        [Category("Appearance")]
        public RangeCalculationMode RangeCalculationMode
        {
            get { return (RangeCalculationMode)GetValue(RangeCalculationModeProperty); }
            set { SetValue(RangeCalculationModeProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether fractional axis values are shown. This
        /// is a dependency property.
        /// </summary>
        /// <value>
        /// <c>true</c> if fractional labels are enabled on zoom; otherwise, <c>false</c>.
        /// </value>
        /// <example>
        /// C#: <code language="C#:">
        /// public Window1()
        /// {
        /// InitializeComponent();
        /// //Creating new chart instance.
        /// Chart chart = new Chart();
        /// //Adding a new area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating datapoints collection.
        /// ChartListData data = new ChartListData();
        /// data.Add(new ChartPoint(1, 1));
        /// data.Add(new ChartPoint(2, 2));
        /// data.Add(new ChartPoint(3, 3));
        /// data.Add(new ChartPoint(4, 4));
        /// //Creating series.
        /// ChartSeries series = new ChartSeries();
        /// //Assigning points.
        /// series.Data = data;
        /// chart.Areas[0].Series.Add(series);
        /// //Disabling fraction labels on axis.
        /// chart.Areas[0].PrimaryAxis.IsFractionEnabledOnZoom = False;
        /// //Assigning window's content property.
        /// this.Content = chart;
        /// }
        /// </code> XAML: <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Width="300" Height="300"&gt;
        /// &lt;!--Adding chart control to window's content--&gt;
        ///  &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        ///    &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3 4 4"/&gt;
        ///    &lt;syncfusion:ChartArea.PrimaryAxis&gt;
        ///        &lt;!--Disabling fraction labels on axis.--&gt;
        ///        &lt;syncfusion:ChartAxis IsFractionEnabledOnZoom="False"/&gt;
        ///    &lt;/syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        ///  &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        [Category("Appearance")]
        public bool IsFractionEnabledOnZoom
        {
            get { return (bool)GetValue(IsFractionEnabledOnZoomProperty); }
            set { SetValue(IsFractionEnabledOnZoomProperty, value); }
        }

        /// <summary>
        /// Gets or sets the title of the axis. This is a dependency property.
        /// </summary>
        /// <example>
        /// C#: <code language="C#:">
        /// public Window1()
        /// {
        /// InitializeComponent();
        /// //Creating new chart instance.
        /// Chart chart = new Chart();
        /// //Adding a new area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating datapoints collection.
        /// ChartListData data = new ChartListData();
        /// data.Add(new ChartPoint(1, 1));
        /// data.Add(new ChartPoint(2, 2));
        /// data.Add(new ChartPoint(3, 3));
        /// data.Add(new ChartPoint(4, 4));
        /// //Creating series.
        /// ChartSeries series = new ChartSeries();
        /// //Assigning points.
        /// series.Data = data;
        /// chart.Areas[0].Series.Add(series);
        /// ChartAxis chartXAxis = new ChartAxis();
        /// chartXAxis.Header = "X - Axis";
        /// chart.Areas[0].PrimaryAxis = chartXAxis;
        /// //Assigning window's content property.
        /// this.Content = chart;
        /// }
        /// </code> XAML: <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Width="300" Height="300"&gt;
        /// &lt;!--Adding chart control to window's content--&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        ///    &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3 4 4"/&gt;
        ///    &lt;syncfusion:ChartArea.PrimaryAxis&gt;
        ///        &lt;!--Setting header for primary axis--&gt;
        ///        &lt;syncfusion:ChartAxis Header="X - Axis"/&gt;
        ///    &lt;/syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        [Category("Appearance")]
        public object Header
        {
            get
            {
                return (object)GetValue(HeaderProperty);
            }

            set
            {
                SetValue(HeaderProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the axis header position.
        /// </summary>
        /// <value>
        /// The header position is one of the <see cref="ChartAlignment" /> enumeration
        /// values.
        /// </value>
        /// <example>
        /// C#: <code language="C#:">
        /// public Window1()
        /// {
        ///     InitializeComponent();
        ///     //Creating new chart instance.
        ///     Chart chart = new Chart();
        ///     //Adding a new area.
        ///     chart.Areas.Add(new ChartArea());
        ///     //Creating datapoints collection.
        ///     ChartListData data = new ChartListData();
        ///     data.Add(new ChartPoint(1, 1));
        ///     data.Add(new ChartPoint(2, 2));
        ///     data.Add(new ChartPoint(3, 3));
        ///     data.Add(new ChartPoint(4, 4));
        ///     //Creating series.
        ///     ChartSeries series = new ChartSeries();
        ///     //Assigning points.
        ///     series.Data = data;
        ///     chart.Areas[0].Series.Add(series);
        ///     ChartAxis chartXAxis = new ChartAxis();
        ///     chartXAxis.Header = "X - Axis";
        ///     // Setting Header Position.
        ///     chartXAxis.HeaderPosition = AxisHeaderPosition.Far;
        ///     chart.Areas[0].PrimaryAxis = chartXAxis;
        ///     //Assigning window's content property.
        ///     this.Content = chart;
        /// }
        /// </code>
        /// XAML: <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Width="300" Height="300"&gt;
        /// &lt;!--Adding chart control to window's content--&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        ///    &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3 4 4"/&gt;
        ///     &lt;!--Setting Header position for primary axis labels--&gt;
        ///    &lt;syncfusion:ChartArea.PrimaryAxis HeaderPosition="Far" &gt;
        ///    &lt;/syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        [Category("Appearance")]
        public HeaderPositions HeaderPosition
        {
            get
            {
                return (HeaderPositions)GetValue(HeaderPositionProperty);
            }
 
            set
            {
                SetValue(HeaderPositionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the header alignment. This is a dependency property.
        /// </summary>
        /// <value>
        /// The header alignment is one of the <see cref="ChartAlignment" /> enumeration
        /// values.
        /// </value>
        /// <example>
        /// C#: <code language="C#:">
        /// public Window1()
        /// {
        /// InitializeComponent();
        /// //Creating new chart instance.
        /// Chart chart = new Chart();
        /// //Adding a new area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating datapoints collection.
        /// ChartListData data = new ChartListData();
        /// data.Add(new ChartPoint(1, 1));
        /// data.Add(new ChartPoint(2, 2));
        /// data.Add(new ChartPoint(3, 3));
        /// data.Add(new ChartPoint(4, 4));
        /// //Creating series.
        /// ChartSeries series = new ChartSeries();
        /// //Assigning points.
        /// series.Data = data;
        /// chart.Areas[0].Series.Add(series);
        /// ChartAxis chartXAxis = new ChartAxis();
        /// chartXAxis.Header = "X - Axis";
        /// //Setting header's alignment.
        /// chartXAxis.HeaderAlignment = ChartAlignment.Near;
        /// chart.Areas[0].PrimaryAxis = chartXAxis;
        /// //Assigning window's content property.
        /// this.Content = chart;
        /// }
        /// </code> XAML: <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Width="300" Height="300"&gt;
        /// &lt;!--Adding chart control to window's content--&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        ///    &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3 4 4"/&gt;
        ///    &lt;syncfusion:ChartArea.PrimaryAxis&gt;
        ///        &lt;!--Setting header alignment for primary axis--&gt;
        ///        &lt;syncfusion:ChartAxis Header="X - Axis" HeaderAlignment="Near"/&gt;
        ///    &lt;/syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        [Category("Appearance")]
        public ChartAlignment HeaderAlignment
        {
            get
            {
                return (ChartAlignment)GetValue(HeaderAlignmentProperty);
            }

            set
            {
                SetValue(HeaderAlignmentProperty, value);
            }
        }


        /// <summary>
        /// Get and Set DoubleDisplayUnitAlignmentProperty
        /// </summary>
        [Category("Appearance")]
        public ChartAlignment DoubleDisplayUnitAlignment
        {
            get
            {
                return (ChartAlignment)GetValue(DoubleDisplayUnitAlignmentProperty);
            }

            set
            {
                SetValue(DoubleDisplayUnitAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the length of the axis' tick. This is a dependency property.
        /// </summary>
        /// <value>
        /// The length of the axis' tick.
        /// </value>
        /// <example>
        /// C#: <code language="C#:">
        /// public Window1()
        /// {
        ///     InitializeComponent();
        ///     //Creating new chart instance.
        ///     Chart chart = new Chart();
        ///     //Adding a new area.
        ///     chart.Areas.Add(new ChartArea());
        ///     //Creating datapoints collection.
        ///     ChartListData data = new ChartListData();
        ///     data.Add(new ChartPoint(1, 1));
        ///     data.Add(new ChartPoint(2, 2));
        ///     data.Add(new ChartPoint(3, 3));
        ///     data.Add(new ChartPoint(4, 4));
        ///     //Creating series.
        ///     ChartSeries series = new ChartSeries();
        ///     //Assigning points.
        ///     series.Data = data;
        ///     chart.Areas[0].Series.Add(series);
        ///     ChartAxis chartXAxis = new ChartAxis();
        ///     chartXAxis.Header = "X - Axis";
        ///     //Setting axis' tick size.
        ///     chartXAxis.TickSize = 20;
        ///     chart.Areas[0].PrimaryAxis = chartXAxis;
        ///     //Assigning window's content property.
        ///     this.Content = chart;
        /// }
        /// </code> XAML: <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Width="300" Height="300"&gt;
        /// &lt;!--Adding chart control to window's content--&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        ///    &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3 4 4"/&gt;
        ///    &lt;syncfusion:ChartArea.PrimaryAxis&gt;
        ///        &lt;!--Setting tick size for primary axis--&gt;
        ///        &lt;syncfusion:ChartAxis TickSize="20"/&gt;
        ///    &lt;/syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        [Category("Appearance")]
        public double TickSize
        {
            get
            {
                return (double)GetValue(TickSizeProperty);
            }

            set
            {
                SetValue(TickSizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the pen that used to draw the axis line. This is a dependency
        /// property.
        /// </summary>
        /// <value>
        /// The line stroke.
        /// </value>
        /// <example>
        /// C#: <code language="C#:">
        /// public Window1()
        /// {
        ///     InitializeComponent();
        ///     //Creating new chart instance.
        ///     Chart chart = new Chart();
        ///     //Adding a new area.
        ///     chart.Areas.Add(new ChartArea());
        ///     //Creating datapoints collection.
        ///     ChartListData data = new ChartListData();
        ///     data.Add(new ChartPoint(1, 1));
        ///     data.Add(new ChartPoint(2, 2));
        ///     data.Add(new ChartPoint(3, 3));
        ///     data.Add(new ChartPoint(4, 4));
        ///     //Creating series.
        ///     ChartSeries series = new ChartSeries();
        ///     //Assigning points.
        ///     series.Data = data;
        ///     chart.Areas[0].Series.Add(series);
        ///     //Creating a new pen.
        ///     Pen axisPen = new Pen();
        ///     axisPen.Brush = Brushes.Red;
        ///     axisPen.DashCap = PenLineCap.Round;
        ///     axisPen.Thickness = 4;
        ///     axisPen.DashStyle = new DashStyle(new [] {1d, 0d, 1d}, 0d);
        ///     //Assigning new pen to primary axis' LineStroke property.
        ///     chart.Areas[0].PrimaryAxis.LineStroke = axisPen;
        ///     //Assigning window's content property.
        ///     this.Content = chart;
        /// }
        /// </code> XAML: <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Width="300" Height="300"&gt;
        /// &lt;!--Adding chart control to window's content--&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3 4 4"/&gt;
        /// &lt;syncfusion:ChartArea.PrimaryAxis&gt;
        ///    &lt;!--Setting axis pen for primary axis--&gt;
        ///    &lt;syncfusion:ChartAxis&gt;
        ///        &lt;syncfusion:ChartAxis.LineStroke&gt;
        ///            &lt;Pen Brush="Red" DashCap="Round" Thickness="4"&gt;
        ///                &lt;Pen.DashStyle&gt;
        ///                    &lt;DashStyle Dashes="1 0 1"/&gt;
        ///                &lt;/Pen.DashStyle&gt;
        ///            &lt;/Pen&gt;
        ///        &lt;/syncfusion:ChartAxis.LineStroke&gt;
        ///    &lt;/syncfusion:ChartAxis&gt;
        /// &lt;/syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        [Category("Appearance")]
        public Pen LineStroke
        {
            get { return (Pen)GetValue(LineStrokeProperty); }
            set { SetValue(LineStrokeProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsOriginCentered
        {
            get { return (bool)GetValue(IsOriginCenteredProperty); }
            set { SetValue(IsOriginCenteredProperty, value); }
        }

        /// <summary>
        /// Gets or sets the pen that used to draw the axis tick lines. This is a dependency
        /// property.
        /// </summary>
        /// <value>
        /// The tick line stroke.
        /// </value>
        /// <example>
        /// C#: <code language="C#:">
        /// public Window1()
        /// {
        ///     InitializeComponent();
        ///     //Creating new chart instance.
        ///     Chart chart = new Chart();
        ///     //Adding a new area.
        ///     chart.Areas.Add(new ChartArea());
        ///     //Creating datapoints collection.
        ///     ChartListData data = new ChartListData();
        ///     data.Add(new ChartPoint(1, 1));
        ///     data.Add(new ChartPoint(2, 2));
        ///     data.Add(new ChartPoint(3, 3));
        ///     data.Add(new ChartPoint(4, 4));
        ///     //Creating series.
        ///     ChartSeries series = new ChartSeries();
        ///     //Assigning points.
        ///     series.Data = data;
        ///     chart.Areas[0].Series.Add(series);
        ///     //Creating a new pen.
        ///     Pen axisPen = new Pen();
        ///     axisPen.Brush = Brushes.Red;
        ///     axisPen.DashCap = PenLineCap.Round;
        ///     axisPen.Thickness = 4;
        ///     axisPen.DashStyle = new DashStyle(new [] {1d, 0d, 1d}, 0d);
        ///     //Assigning new pen to primary axis' TickLineStroke property.
        ///     chart.Areas[0].PrimaryAxis.TickLineStroke = axisPen;
        ///     //Assigning window's content property.
        ///     this.Content = chart;
        /// }
        /// </code> XAML: <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Width="300" Height="300"&gt;
        /// &lt;!--Adding chart control to window's content--&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3 4 4"/&gt;
        /// &lt;syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;!--Setting axis ticks pen for primary axis--&gt;
        /// &lt;syncfusion:ChartAxis&gt;
        /// &lt;syncfusion:ChartAxis.TickLineStroke&gt;
        /// &lt;Pen Brush="Red" DashCap="Round" Thickness="4"&gt;
        /// &lt;Pen.DashStyle&gt;
        /// &lt;DashStyle Dashes="1 0 1"/&gt;
        /// &lt;/Pen.DashStyle&gt;
        /// &lt;/Pen&gt;
        /// &lt;/syncfusion:ChartAxis.TickLineStroke&gt;
        /// &lt;/syncfusion:ChartAxis&gt;
        /// &lt;/syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        [Category("Appearance")]
        public Pen TickLineStroke
        {
            get { return (Pen)GetValue(TickLineStrokeProperty); }
            set { SetValue(TickLineStrokeProperty, value); }
        }


        /// <summary>
        /// Get and Set SmallTickLineStrokeProperty
        /// </summary>
        [Category("Appearance")]
        public Pen SmallTickLineStroke
        {
            get { return (Pen)GetValue(SmallTickLineStrokeProperty); }
            set { SetValue(SmallTickLineStrokeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the size of the small tick. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Small tick cannot be longer than label tick.
        /// </remarks>
        /// <value>
        /// The size of the small tick.
        /// </value>
        /// <example>
        /// C#: <code language="C#:">
        /// public Window1()
        /// {
        /// InitializeComponent();
        /// //Creating new chart instance.
        /// Chart chart = new Chart();
        /// //Adding a new area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating datapoints collection.
        /// ChartListData data = new ChartListData();
        /// data.Add(new ChartPoint(1, 1));
        /// data.Add(new ChartPoint(2, 2));
        /// data.Add(new ChartPoint(3, 3));
        /// data.Add(new ChartPoint(4, 4));
        /// //Creating series.
        /// ChartSeries series = new ChartSeries();
        /// //Assigning points.
        /// series.Data = data;
        /// chart.Areas[0].Series.Add(series);
        /// //Setting small tick size.
        /// chart.Areas[0].PrimaryAxis.SmallTickSize = 15;
        /// //Assigning window's content property.
        /// this.Content = chart;
        /// }
        /// </code> XAML: <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Width="300" Height="300"&gt;
        /// &lt;!--Adding chart control to window's content--&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3 4 4"/&gt;
        /// &lt;syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;!--Setting small tick size--&gt;
        /// &lt;syncfusion:ChartAxis SmallTickSize="15"/&gt;
        /// &lt;/syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        [Category("Appearance")]
        public double SmallTickSize
        {
            get
            {
                return (double)GetValue(SmallTickSizeProperty);
            }

            set
            {
                SetValue(SmallTickSizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the small ticks per interval. This is a dependency property.
        /// </summary>
        /// <value>
        /// The small ticks per interval.
        /// </value>
        /// <example>
        /// C#: <code language="C#:">
        /// public Window1()
        /// {
        /// InitializeComponent();
        /// //Creating new chart instance.
        /// Chart chart = new Chart();
        /// //Adding a new area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating datapoints collection.
        /// ChartListData data = new ChartListData();
        /// data.Add(new ChartPoint(1, 1));
        /// data.Add(new ChartPoint(2, 2));
        /// data.Add(new ChartPoint(3, 3));
        /// data.Add(new ChartPoint(4, 4));
        /// //Creating series.
        /// ChartSeries series = new ChartSeries();
        /// //Assigning points.
        /// series.Data = data;
        /// chart.Areas[0].Series.Add(series);
        /// //Setting small ticks per interval value.
        /// chart.Areas[0].PrimaryAxis.SmallTicksPerInterval = 2;
        /// //Assigning window's content property.
        /// this.Content = chart;
        /// }
        /// </code> XAML: <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Width="300" Height="300"&gt;
        /// &lt;!--Adding chart control to window's content--&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3 4 4"/&gt;
        /// &lt;syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;!--Setting small ticks per interval value.--&gt;
        /// &lt;syncfusion:ChartAxis SmallTicksPerInterval="2"/&gt;
        /// &lt;/syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        [Category("Appearance")]
        public int SmallTicksPerInterval
        {
            get
            {
                return (int)GetValue(SmallTicksPerIntervalProperty);
            }

            set
            {
                SetValue(SmallTicksPerIntervalProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the label rotate angle. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Property represents the angle that axis' labels should be rotated.
        /// </remarks>
        /// <value>
        /// The label rotate angle in degrees.
        /// </value>
        /// <example>
        /// C#: <code language="C#:">
        /// public Window1()
        /// {
        /// InitializeComponent();
        /// //Creating new chart instance.
        /// Chart chart = new Chart();
        /// //Adding a new area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating datapoints collection.
        /// ChartListData data = new ChartListData();
        /// data.Add(new ChartPoint(1, 1));
        /// data.Add(new ChartPoint(2, 2));
        /// data.Add(new ChartPoint(3, 3));
        /// data.Add(new ChartPoint(4, 4));
        /// //Creating series.
        /// ChartSeries series = new ChartSeries();
        /// //Assigning points.
        /// series.Data = data;
        /// chart.Areas[0].Series.Add(series);
        /// //Setting label rotate angle value.
        /// chart.Areas[0].PrimaryAxis.LabelRotateAngle = 45;
        /// //Assigning window's content property.
        /// this.Content = chart;
        /// }
        /// </code> XAML: <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Width="300" Height="300"&gt;
        /// &lt;!--Adding chart control to window's content--&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3 4 4"/&gt;
        /// &lt;syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;!--Angle to rotate axis' labels.--&gt;
        /// &lt;syncfusion:ChartAxis LabelRotateAngle="45"/&gt;
        /// &lt;/syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        [Category("Appearance")]
        public double LabelRotateAngle
        {
            get { return (double)GetValue(LabelRotateAngleProperty); }
            set { SetValue(LabelRotateAngleProperty, value); }
        }

        /// <summary>
        /// Gets or sets <see cref="ChartAxis" /> label horizontal alignment. 
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// The label's horizontal alignment.
        /// </value>
        [Category("Appearance")]
        public HorizontalAlignment LabelHorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(LabelHorizontalAlignmentProperty); }
            set { SetValue(LabelHorizontalAlignmentProperty, value); }
        }

        /// <summary>
        /// Gets or sets <see cref="ChartAxis" /> label vertical alignment. 
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// The label's vertical alignment.
        /// </value>
        [Category("Appearance")]
        public VerticalAlignment LabelVerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(LabelVerticalAlignmentProperty); }
            set { SetValue(LabelVerticalAlignmentProperty, value); }
        }

        /// <summary>
        /// Gets or sets <see cref="ChartAxis" /> label height. 
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// The label's height.
        /// </value>
        [Category("Appearance")]
        public double LabelHeight
        {
            get 
            { 
                return (double)GetValue(LabelHeightProperty); 
            }
            set 
            {
                SetValue(LabelHeightProperty, value); 
            }
        }

        /// <summary>
        /// Gets or sets <see cref="ChartAxis" /> label width. 
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// The label's width.
        /// </value>
        [Category("Appearance")]
        public double LabelWidth
        {
            get 
            { 
                return (double)GetValue(LabelWidthProperty); 
            }
            set 
            {
                SetValue(LabelWidthProperty, value); 
            }
        }

        /// <summary>
        /// Gets or sets <see cref="ChartAxis" /> label font family. This is a dependency
        /// property.
        /// </summary>
        /// <value>
        /// The label's font family.
        /// </value>
        /// <example>
        /// C#: <code language="C#">
        /// public Window1()
        /// {
        /// InitializeComponent();
        /// //Creating new chart instance.
        /// Chart chart = new Chart();
        /// //Adding a new area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating datapoints collection.
        /// ChartListData data = new ChartListData();
        /// data.Add(new ChartPoint(1, 1));
        /// data.Add(new ChartPoint(2, 2));
        /// data.Add(new ChartPoint(3, 3));
        /// data.Add(new ChartPoint(4, 4));
        /// //Creating series.
        /// ChartSeries series = new ChartSeries();
        /// //Assigning points.
        /// series.Data = data;
        /// chart.Areas[0].Series.Add(series);
        /// //Creating font family converter.
        /// FontFamilyConverter fontConverter = new FontFamilyConverter();
        /// //Assigning new font for axis labels.
        /// chart.Areas[0].PrimaryAxis.LabelFontFamily =
        /// (FontFamily)fontConverter.ConvertFromString("Times New Roman");
        /// //Assigning window's content property.
        /// this.Content = chart;
        /// }
        /// </code> XAML: <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Width="300" Height="300"&gt;
        /// &lt;!--Adding chart control to window's content--&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3 4 4"/&gt;
        /// &lt;syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;!--Setting labels' font family--&gt;
        /// &lt;syncfusion:ChartAxis LabelFontFamily="Times New Roman"/&gt;
        /// &lt;/syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        [Category("Appearance")]
        public FontFamily LabelFontFamily
        {
            get { return (FontFamily)GetValue(LabelFontFamilyProperty); }
            set { SetValue(LabelFontFamilyProperty, value); }
        }

        /// <summary>
        /// Gets or sets Axis labels Background. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Property represents label's border background.
        /// </remarks>
        /// <value>
        /// The label's background brush.
        /// </value>
        /// <example>
        /// C#: <code language="C#:">
        /// public Window1()
        /// {
        /// InitializeComponent();
        /// //Creating new chart instance.
        /// Chart chart = new Chart();
        /// //Adding a new area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating datapoints collection.
        /// ChartListData data = new ChartListData();
        /// data.Add(new ChartPoint(1, 1));
        /// data.Add(new ChartPoint(2, 2));
        /// data.Add(new ChartPoint(3, 3));
        /// data.Add(new ChartPoint(4, 4));
        /// //Creating series.
        /// ChartSeries series = new ChartSeries();
        /// //Assigning points.
        /// series.Data = data;
        /// chart.Areas[0].Series.Add(series);
        /// //Assigning new brush for axis labels background.
        /// chart.Areas[0].PrimaryAxis.LabelBackground = Brushes.Red;
        /// //Assigning window's content property.
        /// this.Content = chart;
        /// }
        /// </code> XAML: <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Width="300" Height="300"&gt;
        /// &lt;!--Adding chart control to window's content--&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3 4 4"/&gt;
        /// &lt;syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;!--Setting labels' background brush--&gt;
        /// &lt;syncfusion:ChartAxis LabelBackground="Red"/&gt;
        /// &lt;/syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        [Category("Appearance")]
        public Brush LabelBackground
        {
            get { return (Brush)GetValue(LabelBackgroundProperty); }
            set { SetValue(LabelBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating the axis labels font size. This is a dependency
        /// property.
        /// </summary>
        /// <value>
        /// The size of the label font.
        /// </value>
        /// <example>
        /// C#: <code language="C#:">
        /// public Window1()
        /// {
        /// InitializeComponent();
        /// //Creating new chart instance.
        /// Chart chart = new Chart();
        /// //Adding a new area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating datapoints collection.
        /// ChartListData data = new ChartListData();
        /// data.Add(new ChartPoint(1, 1));
        /// data.Add(new ChartPoint(2, 2));
        /// data.Add(new ChartPoint(3, 3));
        /// data.Add(new ChartPoint(4, 4));
        /// //Creating series.
        /// ChartSeries series = new ChartSeries();
        /// //Assigning points.
        /// series.Data = data;
        /// chart.Areas[0].Series.Add(series);
        /// //Assigning size of label's font.
        /// chart.Areas[0].PrimaryAxis.LabelFontSize = 20;
        /// //Assigning window's content property.
        /// this.Content = chart;
        /// }
        /// </code> XAML: <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Width="300" Height="300"&gt;
        /// &lt;!--Adding chart control to window's content--&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3 4 4"/&gt;
        /// &lt;syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;!--Setting labels' font size--&gt;
        /// &lt;syncfusion:ChartAxis LabelFontSize="20"/&gt;
        /// &lt;/syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        [Category("Appearance")]
        public double LabelFontSize
        {
            get { return (double)GetValue(LabelFontSizeProperty); }
            set { SetValue(LabelFontSizeProperty, value); }
        }

        /// <summary>
        /// Gets or sets axis labels foreground brush. This is a dependency property.
        /// </summary>
        /// <value>
        /// The label foreground color.
        /// </value>
        /// <example>
        /// C#: <code language="C#:">
        /// public Window1()
        /// {
        /// InitializeComponent();
        /// //Creating new chart instance.
        /// Chart chart = new Chart();
        /// //Adding a new area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating datapoints collection.
        /// ChartListData data = new ChartListData();
        /// data.Add(new ChartPoint(1, 1));
        /// data.Add(new ChartPoint(2, 2));
        /// data.Add(new ChartPoint(3, 3));
        /// data.Add(new ChartPoint(4, 4));
        /// //Creating series.
        /// ChartSeries series = new ChartSeries();
        /// //Assigning points.
        /// series.Data = data;
        /// chart.Areas[0].Series.Add(series);
        /// //Assigning new brush for axis labels foreground.
        /// chart.Areas[0].PrimaryAxis.LabelBackground = Brushes.Green;
        /// //Assigning window's content property.
        /// this.Content = chart;
        /// }
        /// </code> XAML: <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Width="300" Height="300"&gt;
        /// &lt;!--Adding chart control to window's content--&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3 4 4"/&gt;
        /// &lt;syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;!--Setting labels' foreground brush--&gt;
        /// &lt;syncfusion:ChartAxis LabelBackground="Blue"/&gt;
        /// &lt;/syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        [Category("Appearance")]
        public Brush LabelForeground
        {
            get { return (Brush)GetValue(LabelForegroundProperty); }
            set { SetValue(LabelForegroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating the axis labels font weight. This is a
        /// dependency property.
        /// </summary>
        /// <value>
        /// The label font weight.
        /// </value>
        /// <example>
        /// C#: <code language="C#:">
        /// public Window1()
        /// {
        /// InitializeComponent();
        /// //Creating new chart instance.
        /// Chart chart = new Chart();
        /// //Adding a new area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating datapoints collection.
        /// ChartListData data = new ChartListData();
        /// data.Add(new ChartPoint(1, 1));
        /// data.Add(new ChartPoint(2, 2));
        /// data.Add(new ChartPoint(3, 3));
        /// data.Add(new ChartPoint(4, 4));
        /// //Creating series.
        /// ChartSeries series = new ChartSeries();
        /// //Assigning points.
        /// series.Data = data;
        /// chart.Areas[0].Series.Add(series);
        /// //Assigning new brush for axis labels font weight.
        /// chart.Areas[0].PrimaryAxis.LabelFontWeight = FontWeights.Bold;
        /// //Assigning window's content property.
        /// this.Content = chart;
        /// }
        /// </code> XAML: <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Width="300" Height="300"&gt;
        /// &lt;!--Adding chart control to window's content--&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3 4 4"/&gt;
        /// &lt;syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;!--Setting labels' background brush--&gt;
        /// &lt;syncfusion:ChartAxis LabelFontWeight="Bold"/&gt;
        /// &lt;/syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        [Category("Appearance")]
        public FontWeight LabelFontWeight
        {
            get { return (FontWeight)GetValue(LabelFontWeightProperty); }
            set { SetValue(LabelFontWeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets axis labels border thickness. This is a dependency property.
        /// </summary>
        /// <value>
        /// The thickness of label's border .
        /// </value>
        /// <example>
        /// C#: <code language="C#:">
        /// public Window1()
        /// {
        /// InitializeComponent();
        /// //Creating new chart instance.
        /// Chart chart = new Chart();
        /// //Adding a new area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating datapoints collection.
        /// ChartListData data = new ChartListData();
        /// data.Add(new ChartPoint(1, 1));
        /// data.Add(new ChartPoint(2, 2));
        /// data.Add(new ChartPoint(3, 3));
        /// data.Add(new ChartPoint(4, 4));
        /// //Creating series.
        /// ChartSeries series = new ChartSeries();
        /// //Assigning points.
        /// series.Data = data;
        /// chart.Areas[0].Series.Add(series);
        /// //Assigning new brush for axis label's border thickness.
        /// chart.Areas[0].PrimaryAxis.LabelBorderThickness = 3;
        /// //Assigning window's content property.
        /// this.Content = chart;
        /// }
        /// </code> XAML: <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Width="300" Height="300"&gt;
        /// &lt;!--Adding chart control to window's content--&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3 4 4"/&gt;
        /// &lt;syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;!--Setting labels' border thickness--&gt;
        /// &lt;syncfusion:ChartAxis LabelBorderThickness="3"/&gt;
        /// &lt;/syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        [Category("Appearance")]
        public Thickness LabelBorderThickness
        {
            get { return (Thickness)GetValue(LabelBorderThicknessProperty); }
            set { SetValue(LabelBorderThicknessProperty, value); }
        }

        /// <summary>
        /// Gets or sets axis labels border brush. This is a dependency property.
        /// </summary>
        /// <value>
        /// The label's border brush.
        /// </value>
        /// <example>
        /// C#: <code language="C#:">
        /// public Window1()
        /// {
        /// InitializeComponent();
        /// //Creating new chart instance.
        /// Chart chart = new Chart();
        /// //Adding a new area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating datapoints collection.
        /// ChartListData data = new ChartListData();
        /// data.Add(new ChartPoint(1, 1));
        /// data.Add(new ChartPoint(2, 2));
        /// data.Add(new ChartPoint(3, 3));
        /// data.Add(new ChartPoint(4, 4));
        /// //Creating series.
        /// ChartSeries series = new ChartSeries();
        /// //Assigning points.
        /// series.Data = data;
        /// chart.Areas[0].Series.Add(series);
        /// //Assigning new brush for axis label's border brush.
        /// chart.Areas[0].PrimaryAxis.LabelBorderBrush = Brushes.Red;
        /// //Assigning window's content property.
        /// this.Content = chart;
        /// }
        /// </code> XAML: <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Width="300" Height="300"&gt;
        /// &lt;!--Adding chart control to window's content--&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3 4 4"/&gt;
        /// &lt;syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;!--Setting labels' border brush--&gt;
        /// &lt;syncfusion:ChartAxis LabelBorderBrush="Red"/&gt;
        /// &lt;/syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        [Category("Appearance")]
        public Brush LabelBorderBrush
        {
            get { return (Brush)GetValue(LabelBorderBrushProperty); }
            set { SetValue(LabelBorderBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets axis labels corner radius. This is a dependency property.
        /// </summary>
        /// <value>
        /// The label's border corner radius.
        /// </value>
        /// <example>
        /// C#: <code language="C#:">
        /// public Window1()
        /// {
        /// InitializeComponent();
        /// //Creating new chart instance.
        /// Chart chart = new Chart();
        /// //Adding a new area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating datapoints collection.
        /// ChartListData data = new ChartListData();
        /// data.Add(new ChartPoint(1, 1));
        /// data.Add(new ChartPoint(2, 2));
        /// data.Add(new ChartPoint(3, 3));
        /// data.Add(new ChartPoint(4, 4));
        /// //Creating series.
        /// ChartSeries series = new ChartSeries();
        /// //Assigning points.
        /// series.Data = data;
        /// chart.Areas[0].Series.Add(series);
        /// //Assigning new brush for axis label's border corner radius.
        /// chart.Areas[0].PrimaryAxis.LabelCornerRadius = new CornerRadius(10,3,10,2);
        /// //Assigning window's content property.
        /// this.Content = chart;
        /// }
        /// </code> XAML: <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Width="300" Height="300"&gt;
        /// &lt;!--Adding chart control to window's content--&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3 4 4"/&gt;
        /// &lt;syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;!--Setting labels' border radius--&gt;
        /// &lt;syncfusion:ChartAxis LabelCornerRadius="10,3,10,2"/&gt;
        /// &lt;/syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        [Category("Appearance")]
        public CornerRadius LabelCornerRadius
        {
            get { return (CornerRadius)GetValue(LabelCornerRadiusProperty); }
            set { SetValue(LabelCornerRadiusProperty, value); }
        }
        #endregion

        /// <summary>
        /// Gets or sets the LogarithmicRange
        /// </summary>
        public DoubleRange LogarithmicRange
        {
            get { return (DoubleRange)GetValue(LogarithmicRangeProperty); }
            set { SetValue(LogarithmicRangeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the axis visibility.
        /// </summary>
        /// <value>The axis visibility.</value>
        public Visibility AxisVisibility
        {
            get { return (Visibility)GetValue(AxisVisibilityProperty); }
            set { SetValue(AxisVisibilityProperty, value); }
        }


        /// <summary>
        /// Gets or sets a value indicating whether range paddings should be ignored when axis is zoomed.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// <c>true</c> if range paddings are ignored on zoom; otherwise, <c>false</c>.
        /// </value>
        public bool IgnoreRangePaddingsOnZoom
        {
            get { return (bool)GetValue(IgnoreRangePaddingsOnZoomProperty); }
            set { SetValue(IgnoreRangePaddingsOnZoomProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value for EnabelLogLabels
        /// This is a dependency property.
        /// </summary>      
        public bool EnableLogLabels
        {
            get { return (bool)GetValue(EnableLogLabelsProperty); }
            set { SetValue(EnableLogLabelsProperty, value); }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the axis should be reversed. When
        /// reversed, the axis will render points from right to left if horizontal, top to
        /// bottom when vertical and clockwise if radial. This is a dependency property.
        /// </summary>
        /// <value>
        /// <c>true</c> if this axis is inversed; otherwise, <c>false</c>.
        /// </value>
        /// <example>
        /// C#: <code language="C#:">
        /// public Window1()
        /// {
        /// InitializeComponent();
        /// //Creating new chart instance.
        /// Chart chart = new Chart();
        /// //Adding a new area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating datapoints collection.
        /// ChartListData data = new ChartListData();
        /// data.Add(new ChartPoint(1, 1));
        /// data.Add(new ChartPoint(2, 2));
        /// data.Add(new ChartPoint(3, 3));
        /// data.Add(new ChartPoint(4, 4));
        /// //Creating series.
        /// ChartSeries series = new ChartSeries();
        /// //Assigning points.
        /// series.Data = data;
        /// chart.Areas[0].Series.Add(series);
        /// //Making primary axis inversed.
        /// chart.Areas[0].PrimaryAxis.IsInversed = True;
        /// //Assigning window's content property.
        /// this.Content = chart;
        /// }
        /// </code> XAML: <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Width="300" Height="300"&gt;
        /// &lt;!--Adding chart control to window's content--&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3 4 4"/&gt;
        /// &lt;syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;!--Setting primary axis to inversed state.--&gt;
        /// &lt;syncfusion:ChartAxis IsInversed="true"/&gt;
        /// &lt;/syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        public bool IsInversed
        {
            get
            {
                return (bool)GetValue(IsInversedProperty);
            }

            set
            {
                SetValue(IsInversedProperty, value);
            }
        }

        /// <summary>
        /// Gets the value indicating whether this axis is logarithmic. This is a
        /// dependency property.
        /// </summary>
        /// <remarks>
        /// Presentation of data on a logarithmic scale can be helpful when the data covers
        /// a large range of values � the logarithm reduces this to a more manageable range.
        /// </remarks>
        /// <value>
        /// <c>true</c> if this instance is logarithmic; otherwise, <c>false</c>.
        /// </value>
        /// <example>
        /// C#: <code language="C#:">
        /// public Window1()
        /// {
        /// InitializeComponent();
        /// //Creating new chart instance.
        /// Chart chart = new Chart();
        /// //Adding a new area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating datapoints collection.
        /// ChartListData data = new ChartListData();
        /// data.Add(new ChartPoint(1, 1));
        /// data.Add(new ChartPoint(2, 2));
        /// data.Add(new ChartPoint(3, 3));
        /// data.Add(new ChartPoint(4, 4));
        /// //Creating series.
        /// ChartSeries series = new ChartSeries();
        /// //Assigning points.
        /// series.Data = data;
        /// chart.Areas[0].Series.Add(series);
        /// //Setting primary axis to logarithmic state.
        /// chart.Areas[0].PrimaryAxis.IsLogarithmic = True;
        /// //Assigning window's content property.
        /// this.Content = chart;
        /// }
        /// </code> XAML: <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// x:Class="WpfApplication2.Window1"
        /// Width="300" Height="300"&gt;
        /// &lt;!--Adding chart control to window's content--&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3 4 4"/&gt;
        /// &lt;syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;!--Setting primary axis to logarithmic state.--&gt;
        /// &lt;syncfusion:ChartAxis IsLogarithmic="true"/&gt;
        /// &lt;/syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        public bool IsLogarithmic
        {
            get { return (this.ValueType == ChartValueType.Logarithmic ? true : false); }
        }
             
        /// <summary>
        /// Gets or sets the logarithmic base. This is a dependency property.
        /// </summary>
        /// <value>The logarithmic base.</value>
        public double LogarithmicBase
        {
            get { return (double)GetValue(LogarithmicBaseProperty); }
            set { SetValue(LogarithmicBaseProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether range that axis should display is being
        /// set automatically.
        /// </summary>
        /// <remarks>
        /// Property should be used for custom range scenarios.
        /// </remarks>
        /// <value>
        /// <c>true</c> if range for axis should be set automatically; otherwise, <c>false</c>.
        /// </value>
        /// <example>
        /// C#: <code language="C#:">
        /// public Window1()
        /// {
        /// InitializeComponent();
        /// //Creating new chart instance.
        /// Chart chart = new Chart();
        /// //Adding a new area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating datapoints collection.
        /// ChartListData data = new ChartListData();
        /// data.Add(new ChartPoint(1, 1));
        /// data.Add(new ChartPoint(2, 2));
        /// data.Add(new ChartPoint(3, 3));
        /// data.Add(new ChartPoint(4, 4));
        /// //Creating series.
        /// ChartSeries series = new ChartSeries();
        /// //Assigning points.
        /// series.Data = data;
        /// chart.Areas[0].Series.Add(series);
        /// //Disabling automatic range setting.
        /// chart.Areas[0].PrimaryAxis.IsAutoSetRange = False;
        /// //Assigning window's content property.
        /// this.Content = chart;
        /// }
        /// </code> XAML: <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Width="300" Height="300"&gt;
        /// &lt;!--Adding chart control to window's content--&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3 4 4"/&gt;
        /// &lt;syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;!--Disabling automatic range setting.--&gt;
        /// &lt;syncfusion:ChartAxis IsAutoSetRange="false"/&gt;
        /// &lt;/syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        public bool IsAutoSetRange
        {
            get { return (bool)GetValue(IsAutoSetRangeProperty); }
            set { SetValue(IsAutoSetRangeProperty, value); }
        }

        /// <summary>
        /// Gets the visible range.
        /// </summary>
        /// <value>The visible <see cref="DoubleRange"/>.</value>
        public DoubleRange VisibleRange
        {
            get
            {
                if (m_visibleRange.IsEmpty)
                {
                    m_isUpdating = true;
                    this.CalculateVisibleRange();
                    m_isUpdating = false;
                }

                return m_visibleRange;
            }
        }

        /// <summary>
        /// Gets the visible interval.
        /// </summary>
        /// <value>The visible interval.</value>
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public double VisibleInterval
        {
            get
            {
                return m_visibleInterval;
            }
        }

        /// <summary>
        /// Gets the visible interval.
        /// </summary>
        /// <value>The visible interval.</value>
        internal double VisibleIntervalOffset
        {
            get
            {
                return m_visibleIntervalOffset;
            }
        }

        /// <summary>
        /// Gets the logarithmic visible range.
        /// </summary>
        internal DoubleRange LogarithmicVisibleRange
        {
            get
            {
                if (m_logarihmicVisibleRange.IsEmpty || m_visibleRange.IsEmpty)
                {
                    m_logarihmicVisibleRange = new DoubleRange(Math.Log(VisibleRange.Start, LogarithmicBase), Math.Log(VisibleRange.End, LogarithmicBase));
                }

                return m_logarihmicVisibleRange;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the range that axis should display. Note: The Range will be applicable only when the Axis.IsAutoSetRange is set as false.
        /// </summary>
        /// <remarks>
        /// Despite of what range series' data represents, only assigned range will be
        /// shown. <para />  Custom range can be helpful for custom scrolling axis scenarios.
        /// </remarks>
        /// <value>
        /// The range to show on axis.
        /// </value>
        /// <example>
        /// C#: <code language="C#:">
        /// public Window1()
        /// {
        /// InitializeComponent();
        /// //Creating new chart instance.
        /// Chart chart = new Chart();
        /// //Adding a new area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating datapoints collection.
        /// ChartListData data = new ChartListData();
        /// data.Add(new ChartPoint(1, 1));
        /// data.Add(new ChartPoint(2, 2));
        /// data.Add(new ChartPoint(3, 3));
        /// data.Add(new ChartPoint(4, 4));
        /// //Creating series.
        /// ChartSeries series = new ChartSeries();
        /// //Assigning points.
        /// series.Data = data;
        /// chart.Areas[0].Series.Add(series);
        /// //Setting primary axis range from 1 to 3.
        /// chart.Areas[0].PrimaryAxis.Range = new DoubleRange(1, 3);
        /// //Assigning window's content property.
        /// this.Content = chart;
        /// }
        /// </code> XAML: <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Width="300" Height="300"&gt;
        /// &lt;!--Adding chart control to window's content--&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3 4 4"/&gt;
        /// &lt;syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;!--Setting custom range for the axis.--&gt;
        /// &lt;syncfusion:ChartAxis Range="1,3"/&gt;
        /// &lt;/syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        public DoubleRange Range
        {
            get
            {
                return (DoubleRange)GetValue(RangeProperty);
            }

            set
            {
                SetValue(RangeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the DatTimeRange. This is a dependency property. Note: The DateTimeRange will be applicable only when the Axis.IsAutoSetRange is set as false.
        /// </summary>
        /// <value>The DatTimeRange.</value>
        public DateTimeRange DateTimeRange
        {
            get { return (DateTimeRange)GetValue(DateTimeRangeProperty); }
            set
            {
                SetValue(DateTimeRangeProperty, value);
                DateTimeRange dateTimeRange = (DateTimeRange)value;
                if (!dateTimeRange.IsEmpty && this.ValueType == ChartValueType.DateTime)
                {
                    this.InternalRange = new DoubleRange(dateTimeRange.Start.ToOADate(), dateTimeRange.End.ToOADate());
                }
                else if (this.InternalRange != this.m_cachedRangeValue)
                {
                    this.InternalRange = this.m_cachedRangeValue;
                }
            }
        }

        /// <summary>
        /// Gets or sets the origin value. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Property gives ability to specify a custom origin for axis for <see
        /// cref="ChartTypes.Bar">Bar</see>, <see cref="ChartTypes.Column">Column</see>,
        /// <see cref="ChartTypes.StackingColumn">StackingColumn</see> and <see
        /// cref="ChartTypes.Area">Area</see>. The origin can be any specific value in the
        /// axis range or it can also be a specific data point's value.
        /// </remarks>
        /// <value>
        /// The origin value.
        /// </value>
        /// <example>
        /// C#: <code language="C#:">
        /// public Window1()
        /// {
        /// InitializeComponent();
        /// //Creating new chart instance.
        /// Chart chart = new Chart();
        /// //Adding a new area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating datapoints collection.
        /// ChartListData data = new ChartListData();
        /// data.Add(new ChartPoint(1, 1));
        /// data.Add(new ChartPoint(2, 2));
        /// data.Add(new ChartPoint(3, 3));
        /// data.Add(new ChartPoint(4, 4));
        /// //Creating series.
        /// ChartSeries series = new ChartSeries();
        /// series.Type = ChartTypes.Column;
        /// //Assigning points.
        /// series.Data = data;
        /// chart.Areas[0].Series.Add(series);
        /// //Shifting series' Origin to 10 points.
        /// chart.Areas[0].PrimaryAxis.Origin = 10;
        /// //Assigning window's content property.
        /// this.Content = chart;
        /// }
        /// </code> XAML: <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Width="300" Height="300"&gt;
        /// &lt;!--Adding chart control to window's content--&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3 4 4" Type="Column"/&gt;
        /// &lt;syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;!--Setting origin for the axis.--&gt;
        /// &lt;syncfusion:ChartAxis Origin="10"/&gt;
        /// &lt;/syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        public double Origin
        {
            get { return (double)GetValue(OriginProperty); }
            set { SetValue(OriginProperty, value); }
        }

        /// <summary>
        /// Get and Set EnableAutoIntervalOnZoomingProperty
        /// </summary>
        public bool EnableAutoIntervalOnZooming
        {
            get
            {
                return (bool)GetValue(EnableAutoIntervalOnZoomingProperty);
            }

            set
            {
                SetValue(EnableAutoIntervalOnZoomingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the axis can be zoomed. This is a
        /// dependency property.
        /// </summary>
        /// <remarks>
        /// This property is intended to be used for specific zooming scenarios when axis'
        /// series zooming should be prevented.
        /// </remarks>
        /// <value>
        /// <c>true</c> if axis can be zoomed in zooming mode; otherwise, <c>false</c>.
        /// </value>
        /// <example>
        /// C#: <code language="C#:">
        /// public Window1()
        /// {
        /// InitializeComponent();
        /// //Creating new chart instance.
        /// Chart chart = new Chart();
        /// //Adding a new area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating datapoints collection.
        /// ChartListData data = new ChartListData();
        /// data.Add(new ChartPoint(1, 1));
        /// data.Add(new ChartPoint(2, 2));
        /// data.Add(new ChartPoint(3, 3));
        /// data.Add(new ChartPoint(4, 4));
        /// //Creating series.
        /// ChartSeries series = new ChartSeries();
        /// //Assigning points.
        /// series.Data = data;
        /// chart.Areas[0].Series.Add(series);
        /// //Shifting series' Origin to 10 points.
        /// chart.Areas[0].PrimaryAxis.EnableZooming = false;
        /// //Assigning window's content property.
        /// this.Content = chart;
        /// }
        /// </code> XAML: <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Width="300" Height="300"&gt;
        /// &lt;!--Adding chart control to window's content--&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3 4 4"/&gt;
        /// &lt;syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;!--Disabling Zoomingfor the axis.--&gt;
        /// &lt;syncfusion:ChartAxis EnableZooming="false"/&gt;
        /// &lt;/syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        /// <seealso cref="ChartAreaCommands">ChartAreaCommands</seealso>
        /// <seealso
        /// cref="ChartAreaCommands.SwitchZooming">ChartAreaCommands.SwitchZooming</seealso>
        /// <seealso cref="ChartAreaCommands.ZoomIn">ChartAreaCommands.ZoomIn</seealso>
        /// <seealso cref="ChartAreaCommands.ZoomOut">ChartAreaCommands.ZoomOut</seealso>
        /// <seealso
        /// cref="ChartAreaCommands.ZoomReset">ChartAreaCommands.ZoomReset</seealso>
        /// <seealso
        /// cref="ChartAreaCommands.CancelZooming">ChartAreaCommands.CancelZooming</seealso>
        public bool EnableZooming
        {
            get
            {
                return (bool)GetValue(EnableZoomingProperty);
            }

            set
            {
                SetValue(EnableZoomingProperty, value);
            }
        }

        private static void OnEnableZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (!(bool)e.NewValue && axis != null)
                axis.Area.ZoomAllAxes = false;
        }

        /// <summary>
        /// Gets or sets the zoom position.
        /// </summary>
        /// <value>The zoom position.</value>
        public double ZoomPosition
        {
            get { return (double)GetValue(ZoomPositionProperty); }
            set { SetValue(ZoomPositionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the zoom factor.
        /// </summary>
        /// <value>The zoom factor.</value>
        public double ZoomFactor
        {
            get { return (double)GetValue(ZoomFactorProperty); }
            set { SetValue(ZoomFactorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the type of the value that chart axis displays. This is a dependency property.
        /// </summary>
        /// <value>The type of the value.</value>
        /// <seealso cref="ChartValueType"/>
        public ChartValueType ValueType
        {
            get
            {
                return (ChartValueType)GetValue(ValueTypeProperty);
            }
            set
            {
                SetValue(ValueTypeProperty, value);
            }
        }

        /// <summary>
        /// Gets the strip lines collection.
        /// </summary>
        /// <remarks>
        /// Property represents <see cref="ChartStripLine">ChartStripLines</see> that should
        /// be drawn for specified axis.
        /// </remarks>
        /// <value>
        /// The strip lines.
        /// </value>
        /// <example>
        /// C#: <code language="C#:">
        /// public Window1()
        /// {
        /// InitializeComponent();
        /// //Creating new chart instance.
        /// Chart chart = new Chart();
        /// //Adding a new area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating datapoints collection.
        /// ChartListData data = new ChartListData();
        /// data.Add(new ChartPoint(1, 1));
        /// data.Add(new ChartPoint(2, 2));
        /// data.Add(new ChartPoint(3, 3));
        /// data.Add(new ChartPoint(4, 4));
        /// //Creating series.
        /// ChartSeries series = new ChartSeries();
        /// //Assigning points.
        /// series.Data = data;
        /// chart.Areas[0].Series.Add(series);
        /// //Creating a new chart stripline.
        /// ChartStripLine stripline = new ChartStripLine();
        /// //Setting additional properties.
        /// stripline.Interior = Brushes.Black;
        /// stripline.Period = 2;
        /// stripline.Length = 1;
        /// //Adding stripline to collection.
        /// chart.Areas[0].PrimaryAxis.Striplines.Add(stripline);
        /// //Assigning window's content property.
        /// this.Content = chart;
        /// }
        /// </code> XAML: <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Width="300" Height="300"&gt;
        /// &lt;!--Adding chart control to window's content--&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3 4 4"/&gt;
        /// &lt;syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;syncfusion:ChartAxis.StripLines&gt;
        /// &lt;syncfusion:ChartStripLine Interior="Black" Period="2" Length="1"/&gt;
        /// &lt;/syncfusion:ChartAxis.StripLines&gt;
        /// &lt;/syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        /// <seealso cref="ChartStripLine">ChartStripLine</seealso>
        /// <seealso cref="ChartStripLinesCollection">ChartStripLinesCollection</seealso>
        /// <seealso cref="ChartAxis">ChartAxis</seealso>
        /// <seealso cref="ChartArea">ChartArea</seealso>
        /// <seealso cref="ChartSeries">ChartSeries</seealso>
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        //public ChartStripLinesCollection StripLines
        //{
        //    get
        //    {
        //        return m_stripLines;
        //    }
        //}
        //SD15968 Binding StripLine Collection(Replacing CLR property into Dependency property
        public ChartStripLinesCollection StripLines
        {
            get { return (ChartStripLinesCollection)GetValue(StripLinesProperty); }
            set { SetValue(StripLinesProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for StripLines.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StripLinesProperty =
            DependencyProperty.Register("StripLines", typeof(ChartStripLinesCollection), typeof(ChartAxis), new PropertyMetadata(null));
        /// <summary>
        /// Gets or sets the label format. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Represents pattern that describes how axis labels should be displayed.
        /// </remarks>
        /// <value>
        /// The strip lines.
        /// </value>
        /// <example>
        /// C#: <code language="C#:">
        /// public Window1()
        /// {
        /// InitializeComponent();
        /// //Creating new chart instance.
        /// Chart chart = new Chart();
        /// //Adding a new area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating datapoints collection.
        /// ChartListData data = new ChartListData();
        /// data.Add(new ChartPoint(1, 1));
        /// data.Add(new ChartPoint(2, 2));
        /// data.Add(new ChartPoint(3, 3));
        /// data.Add(new ChartPoint(4, 4));
        /// //Creating series.
        /// ChartSeries series = new ChartSeries();
        /// //Assigning points.
        /// series.Data = data;
        /// chart.Areas[0].Series.Add(series);
        /// //Setting axis labels format.
        /// chart.Areas[0].PrimaryAxis.LabelFormat = "1.00";
        /// //Assigning window's content property.
        /// this.Content = chart;
        /// }
        /// </code> XAML: <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Width="300" Height="300"&gt;
        /// &lt;!--Adding chart control to window's content--&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3 4 4"/&gt;
        /// &lt;syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;syncfusion:ChartAxis LabelFormat="1.00"/&gt;
        /// &lt;/syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        /// <seealso cref="ChartAxis">ChartAxis</seealso>
        /// <seealso cref="ChartArea">ChartArea</seealso>
        /// <seealso cref="ChartSeries">ChartSeries</seealso>
        public string LabelFormat
        {
            get { return (string)GetValue(LabelFormatProperty); }
            set { SetValue(LabelFormatProperty, value); }
        }

        /// <summary>
        /// Gets or sets the label date time format. This is a dependency property.
        /// </summary>
        /// <value>
        /// The strip lines.
        /// </value>
        /// <example>
        /// C#: <code language="C#:">
        /// public Window1()
        /// {
        /// InitializeComponent();
        /// //Creating new chart instance.
        /// Chart chart = new Chart();
        /// //Adding a new area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating datapoints collection.
        /// ChartListData data = new ChartListData();
        /// data.Add(new ChartPoint(1, 1));
        /// data.Add(new ChartPoint(2, 2));
        /// data.Add(new ChartPoint(3, 3));
        /// data.Add(new ChartPoint(4, 4));
        /// //Creating series.
        /// ChartSeries series = new ChartSeries();
        /// //Assigning points.
        /// series.Data = data;
        /// chart.Areas[0].Series.Add(series);
        /// //Setting axis labels date time format.
        /// chart.Areas[0].PrimaryAxis.LabelDateTimeFormat = "dd/yy/mm";
        /// //Assigning window's content property.
        /// this.Content = chart;
        /// }
        /// </code> XAML: <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Width="300" Height="300"&gt;
        /// &lt;!--Adding chart control to window's content--&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3 4 4"/&gt;
        /// &lt;syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;syncfusion:ChartAxis LabelDateTimeFormat="dd/yy/mm"/&gt;
        /// &lt;/syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        /// <seealso cref="ChartAxis">ChartAxis</seealso>
        /// <seealso cref="ChartArea">ChartArea</seealso>
        /// <seealso cref="ChartSeries">ChartSeries</seealso>
        public string LabelDateTimeFormat
        {
            get { return (string)GetValue(LabelDateTimeFormatProperty); }
            set { SetValue(LabelDateTimeFormatProperty, value); }
        }

        /// <summary>
        /// Gets or sets the intersecting layout behaviour for the labels of axis. This is a
        /// dependency property.
        /// </summary>
        /// <remarks>
        /// Sometimes it can happen that labels are too large and intersect. Axis supports
        /// several methods to prevent intersections. <para /> LabelIntersectAction is
        /// supported only by rectangular coordinate system. <see
        /// cref="ChartLabelIntersectAction.Wrap" /> and <see
        /// cref="ChartLabelIntersectAction.Rotate" /> modes are ignored by vertical axes
        /// simply because these modes are inefficient. It is recommended to set
        /// LabelTemplate property in Wrap mode via TextWraping property of the TextBlock.
        /// </remarks>
        /// <value>
        /// One of <see cref="ChartLabelIntersectAction" /> enumeration values.
        /// </value>
        /// <example>
        /// C#: <code language="C#:">
        /// public Window1()
        /// {
        /// InitializeComponent();
        /// //Creating new chart instance.
        /// Chart chart = new Chart();
        /// //Adding a new area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating datapoints collection.
        /// ChartListData data = new ChartListData();
        /// data.Add(new ChartPoint(1, 1));
        /// data.Add(new ChartPoint(2, 2));
        /// data.Add(new ChartPoint(3, 3));
        /// data.Add(new ChartPoint(4, 4));
        /// //Creating series.
        /// ChartSeries series = new ChartSeries();
        /// //Assigning points.
        /// series.Data = data;
        /// chart.Areas[0].Series.Add(series);
        /// //Setting intersect action for labels.
        /// chart.Areas[0].PrimaryAxis.IntersectAction = ChartLabelIntersectAction.Rotate;
        /// //Assigning window's content property.
        /// this.Content = chart;
        /// }
        /// </code> XAML: <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Width="300" Height="300"&gt;
        /// &lt;!--Adding chart control to window's content--&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3 4 4"/&gt;
        /// &lt;syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;syncfusion:ChartAxis IntersectAction="Rotate"/&gt;
        /// &lt;/syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        public ChartLabelIntersectAction IntersectAction
        {
            get
            {
                return (ChartLabelIntersectAction)GetValue(IntersectActionProperty);
            }

            set
            {
                SetValue(IntersectActionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the orientation of axis. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// <see cref="ChartArea.PrimaryAxis"/> has always is horizontal orientation, <see cref="ChartArea.SecondaryAxis"/> has vertical as well.
        /// <para/>
        /// Property should be used in multiple axes scenarios. 
        /// </remarks>
        /// <value>The axis orientation.</value>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether axis should be in opposed position.
        /// </summary>
        /// <value>
        /// <c>true</c> if axis should be at the bottom Horizontal orientation) of left (for the Vertical orientation) side of area; otherwise, <c>false</c> if axis should be at the top (for the Horizontal orientation) of right(for the Vertical orientation) side of area.
        /// </value>
        /// <example>
        /// C#: <code language="C#:">
        /// public Window1()
        /// {
        /// InitializeComponent();
        /// //Creating new chart instance.
        /// Chart chart = new Chart();
        /// //Adding a new area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating datapoints collection.
        /// ChartListData data = new ChartListData();
        /// data.Add(new ChartPoint(1, 1));
        /// data.Add(new ChartPoint(2, 2));
        /// data.Add(new ChartPoint(3, 3));
        /// data.Add(new ChartPoint(4, 4));
        /// //Creating series.
        /// ChartSeries series = new ChartSeries();
        /// //Assigning points.
        /// series.Data = data;
        /// chart.Areas[0].Series.Add(series);
        /// //Setting both axes to opposed positions.
        /// chart.Areas[0].PrimaryAxis.OpposedPosition = True;
        /// chart.Areas[0].SecondaryAxis.OpposedPosition = True;
        /// //Assigning window's content property.
        /// this.Content = chart;
        /// }
        /// </code> XAML: <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Width="300" Height="300"&gt;
        /// &lt;!--Adding chart control to window's content--&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3 4 4"/&gt;
        /// &lt;syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;syncfusion:ChartAxis OpposedPosition="True"/&gt;
        /// &lt;/syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;syncfusion:ChartArea.SecondaryAxis&gt;
        /// &lt;syncfusion:ChartAxis OpposedPosition="True"/&gt;
        /// &lt;/syncfusion:ChartArea.SecondaryAxis&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        /// <seealso cref="ChartArea">ChartArea</seealso>
        /// <seealso cref="ChartAxisLabel">ChartAxisLabel</seealso>
        public bool OpposedPosition
        {
            get { return (bool)GetValue(OpposedPositionProperty); }
            set { SetValue(OpposedPositionProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this axis has logarithmic labels.
        /// </summary>
        /// <value>
        ///    <c>true</c> if this axis has logarithmic labels; otherwise, <c>false</c>.
        /// </value>
        internal bool IsLogarithmicLabels
        {
            get { return (bool)GetValue(IsLogarithmicLabelsProperty); }
            set { SetValue(IsLogarithmicLabelsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the range padding for axis' range.
        /// This property works only for Non-Indexed axis.
        /// </summary>
        /// <value>The value should be set from one of <see cref="ChartRangePaddingType"/> enumeration.</value>
        public ChartRangePaddingType RangePadding
        {
            get { return (ChartRangePaddingType)GetValue(RangePaddingProperty); }
            set { SetValue(RangePaddingProperty, value); }
        }

        /// <summary>
        /// Gets or sets the desired intervals count.
        /// </summary>
        /// <remarks>
        /// Property indicates quantity of intervals that axis range should be divided by.
        /// </remarks>
        /// <value>The desired intervals count.</value>
        public int DesiredIntervalsCount
        {
            get { return (int)GetValue(DesiredIntervalsCountProperty); }
            set { SetValue(DesiredIntervalsCountProperty, value); }
        }

        /// <summary>
        /// Gets or sets the tick interval.
        /// </summary>
        /// <value>The tick interval.</value>
        public double Interval
        {
            get { return (double)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

        /// <summary>
        /// Gets or sets the minimum interval.
        /// </summary>
        /// <value>The Minimum interval.</value>
        public double MinimumInterval
        {
            get { return (double)GetValue(MinimumIntervalProperty); }
            set { SetValue(MinimumIntervalProperty, value); }
        }

        /// <summary>
        /// Gets or sets the DateTimeInterval. This is a dependency property.
        /// </summary>
        /// <value>The DateTimeInterval.</value>
        public TimeSpan DateTimeInterval
        {
            get { return (TimeSpan)GetValue(DateTimeIntervalProperty); }
            set
            {
                SetValue(DateTimeIntervalProperty, value);
                this.checkDateTimeIntervalChange = true;
                //if (this.ValueType == ChartValueType.DateTime)
                //{
                //    if ((TimeSpan)value < this.MinimumDateTimeInterval)
                //    {
                //        this.Interval = (DateTime.Now + (TimeSpan)this.MinimumDateTimeInterval).ToOADate() - DateTime.Now.ToOADate();
                //    }
                //    else
                //    {
                //        this.Interval = (DateTime.Now + (TimeSpan)value).ToOADate() - DateTime.Now.ToOADate();
                //    }
                //}
            }
        }



        /// <summary>
        /// Get and Set TimeSpanIntervalProperty
        /// </summary>
        public TimeSpan TimeSpanInterval
        {
            get { return (TimeSpan)GetValue(TimeSpanIntervalProperty); }
            set { SetValue(TimeSpanIntervalProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for TimeSpanInterval.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TimeSpanIntervalProperty =
            DependencyProperty.Register("TimeSpanInterval", typeof(TimeSpan), typeof(ChartAxis), new UIPropertyMetadata(new TimeSpan(), new PropertyChangedCallback(OnDateTimeIntervalPropertyChanged)));

        

        /// <summary>
        /// Gets or sets the minimum DateTime interval.
        /// </summary>
        /// <value>The Minimum DateTime interval.</value>
        public TimeSpan MinimumDateTimeInterval
        {
            get { return (TimeSpan)GetValue(MinimumDateTimeIntervalProperty); }
            set { SetValue(MinimumDateTimeIntervalProperty, value); }
        }

        /// <summary>
        /// Gets or sets the tick offset.
        /// </summary>
        /// <value>The tick offset.</value>
        public double IntervalOffset
        {
            get { return (double)GetValue(IntervalOffsetProperty); }
            set { SetValue(IntervalOffsetProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ChartAxis"/> is indexed.
        /// </summary>
        /// <value><c>true</c> if indexed; otherwise, <c>false</c>.</value>
        internal bool Indexed
        {
            get
            {
                if (m_owner == null)
                {
                    return false;
                }

                if (m_owner.PrimarySeries == null)
                {
                    return false;
                }

                if (m_owner.PrimarySeries.ActualXAxis != this && !(m_owner is TimeLineControl))
                {
                    return false;
                }
                if (m_owner is TimeLineControl && m_owner.PrimaryAxis == this)
                    return false;

                return (m_owner.PrimarySeries.IsIndexed && m_owner.IsIndexedCompatible)||this.ValueType==ChartValueType.String; ////&& m_owner.VisibleSeries.Count <= 1);
				////return (bool)GetValue(IndexedProperty); 
            }

            set
            {
                SetValue(IndexedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the labels mode. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Property indicates how labels should retrieve their values.
        /// </remarks>
        /// <value>The labels mode. One of <see cref="ChartAxisLabelsMode"/> enumeration values.</value>
        public ChartAxisLabelsMode LabelsMode
        {
            get { return (ChartAxisLabelsMode)GetValue(LabelsModeProperty); }
            set { SetValue(LabelsModeProperty, value); }
        }

        /// Gets or sets the labels prefix template. This is a dependency property.
        /// 
        /// <value>
        /// The labels prefix template.
        /// </value>
        /// <example>
        /// C#: <code>
        /// This property is not intended to be used from C#.
        /// </code> XAML: <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Height="300" Width="300"&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="clr-namespace:Syncfusion.Windows.Chart;assembly=Syncfusion.Chart.Wpf"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Type="RangeColumn"
        /// Data="0 {7 4} 1 {8 3} 2 {9 7} 3 {3 6} 4 {1 2} 5 {8 11} 6 {3 2} 7 {8 4}"/&gt;
        /// &lt;syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;syncfusion:ChartAxis&gt;
        /// &lt;syncfusion:ChartAxis.LabelsPrefix&gt;
        /// &lt;DataTemplate&gt;
        /// &lt;TextBlock Text="$" &gt;
        /// &lt;/DataTemplate&gt;
        /// &lt;/syncfusion:ChartAxis.LabelsPrefix&gt;
        /// &lt;/syncfusion:ChartAxis&gt;
        /// &lt;/syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        public DataTemplate LabelsPrefix
        {
            get { return (DataTemplate)GetValue(LabelsPrefixProperty); }
            set { SetValue(LabelsPrefixProperty, value); }
        }
 
        /// <summary>
        /// Gets or sets the labels postfix template. This is a dependency property.
        /// </summary>
        /// <value>
        /// The labels postfix template.
        /// </value>
        /// <example>
        /// C#: <code>
        /// This property is not intended to be used from C#.
        /// </code> XAML: <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Height="300" Width="300"&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="clr-namespace:Syncfusion.Windows.Chart;assembly=Syncfusion.Chart.Wpf"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Type="RangeColumn"
        /// Data="0 {7 4} 1 {8 3} 2 {9 7} 3 {3 6} 4 {1 2} 5 {8 11} 6 {3 2} 7 {8 4}"/&gt;
        /// &lt;syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;syncfusion:ChartAxis&gt;
        /// &lt;syncfusion:ChartAxis.LabelsPostfix&gt;
        /// &lt;DataTemplate&gt;
        /// &lt;TextBlock Text="$" &gt;
        /// &lt;/DataTemplate&gt;
        /// &lt;/syncfusion:ChartAxis.LabelsPostfix&gt;
        /// &lt;/syncfusion:ChartAxis&gt;
        /// &lt;/syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        public DataTemplate LabelsPostfix
        {
            get { return (DataTemplate)GetValue(LabelsPostfixProperty); }
            set { SetValue(LabelsPostfixProperty, value); }
        }

        /// <summary>
        /// Gets or sets the labels template. This is a dependency property.
        /// </summary>
        /// <value>
        /// The labels template.
        /// </value>
        /// <example>
        /// C#: <code>
        /// This property is not intended to be used from C#.
        /// </code> XAML: <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Height="300" Width="300"&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="clr-namespace:Syncfusion.Windows.Chart;assembly=Syncfusion.Chart.Wpf"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Type="RangeColumn"
        /// Data="0 {7 4} 1 {8 3} 2 {9 7} 3 {3 6} 4 {1 2} 5 {8 11} 6 {3 2} 7 {8 4}"/&gt;
        /// &lt;syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;syncfusion:ChartAxis&gt;
        /// &lt;syncfusion:ChartAxis.LabelTemplate&gt;
        /// &lt;DataTemplate&gt;
        /// &lt;Border&gt;
        /// &lt;Button Content="{Binding Content}"/&gt;
        /// &lt;/Border&gt;
        /// &lt;/DataTemplate&gt;
        /// &lt;/syncfusion:ChartAxis.LabelTemplate&gt;
        /// &lt;/syncfusion:ChartAxis&gt;
        /// &lt;/syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        public DataTemplate LabelTemplate
        {
            get { return (DataTemplate)GetValue(LabelTemplateProperty); }
            set { SetValue(LabelTemplateProperty, value); }
        }

        /// <summary>
        /// Gets the custom labels for axis.
        /// </summary>
        /// <remarks>
        /// This property allows add custom <see cref="ChartAxisLabel">labels</see> to axis.
        /// Gridlines are being drawn for custom labels in the same way as for default
        /// labels.
        /// </remarks>
        /// <value>
        /// The custom labels <see cref="ChartAxisLabelsCollection">collection</see>.
        /// </value>
        /// <example>
        /// C#: <code>
        /// public Window1()
        /// {
        /// InitializeComponent();
        /// //Creating new chart instance.
        /// Chart chart = new Chart();
        /// //Adding a new area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating datapoints collection.
        /// ChartListData data = new ChartListData();
        /// data.Add(new ChartPoint(1, 1));
        /// data.Add(new ChartPoint(2, 2));
        /// data.Add(new ChartPoint(3, 3));
        /// data.Add(new ChartPoint(4, 4));
        /// //Creating series.
        /// ChartSeries series = new ChartSeries();
        /// //Assigning points.
        /// series.Data = data;
        /// chart.Areas[0].Series.Add(series);
        /// //Creating custom label.
        /// ChartAxisLabel customLabel = new ChartAxisLabel();
        /// customLabel.Content = "3.5";
        /// customLabel.Position = 3.5;
        /// //Adding custom label to labels collection.
        /// chart.Areas[0].PrimaryAxis.CustomLabels.Add(customLabel);
        /// //Assigning window's content property.
        /// this.Content = chart;
        /// }
        /// </code> XAML: <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Height="300" Width="300"&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="clr-namespace:Syncfusion.Windows.Chart;assembly=Syncfusion.Chart.Wpf"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Type="RangeColumn"
        /// Data="0 {7 4} 1 {8 3} 2 {9 7} 3 {3 6} 4 {1 2} 5 {8 11} 6 {3 2} 7 {8 4}"/&gt;
        /// &lt;syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;syncfusion:ChartAxis&gt;
        /// &lt;syncfusion:ChartAxis.CustomLabels&gt;
        /// &lt;syncfusion:ChartAxisLabel Content="3.5" Position="3.5"/&gt;
        /// &lt;/syncfusion:ChartAxis.CustomLabels&gt;
        /// &lt;/syncfusion:ChartAxis&gt;
        /// &lt;/syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        public ChartAxisLabelsCollection CustomLabels
        {
            get
            {
                return m_customLables;
            }
        }

        #region Data
        /// <summary>
        /// Gets or sets the labels source. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Source for labels may be any source that supports <see cref="IEnumerable"/>.
        /// </remarks>
        /// <value>The labels source.</value>
        /// <example>
        /// <code language="XAML">
        /// &lt;Window
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// Height="300" Width="300"&gt;
        /// &lt;syncfusion:Chart xmlns:syncfusion="clr-namespace:Syncfusion.Windows.Chart;assembly=Syncfusion.Chart.Wpf"&gt;
        ///    &lt;syncfusion:Chart.Resources&gt;
        ///        &lt;syncfusion:ChartAxisLabelsCollection x:Key="labelsSourse"&gt;
        ///            &lt;syncfusion:ChartAxisLabel Position="2.2" Content="2.2"/&gt;
        ///            &lt;syncfusion:ChartAxisLabel Position="4.6" Content="4.6"/&gt;
        ///        &lt;/syncfusion:ChartAxisLabelsCollection&gt;
        ///    &lt;/syncfusion:Chart.Resources&gt;
        ///        &lt;syncfusion:ChartArea&gt;
        ///    &lt;syncfusion:ChartSeries Type="RangeColumn"
        ///                     Data="0 {7 4} 1 {8 3} 2 {9 7} 3 {3 6} 4 {1 2} 5 {8 11} 6 {3 2} 7 {8 4}"/&gt;
        ///      &lt;syncfusion:ChartArea.PrimaryAxis&gt;
        ///                &lt;syncfusion:ChartAxis LabelsSource="{StaticResource labelsSourse}" PositionPath="Position"/&gt;
        ///      &lt;/syncfusion:ChartArea.PrimaryAxis&gt;
        ///  &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        [Category("Data")]
        public object LabelsSource
        {
            get { return (object)GetValue(LabelsSourceProperty); }
            set
            {
                if (value is DataTable)
                {
                    SetValue(LabelsSourceProperty, (value as DataTable).Rows);
                }               
                else
                {
                    SetValue(LabelsSourceProperty, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the position path. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// When <see cref="LabelsSource"/> property is set, position path used to determine value of label's position.
        /// </remarks>
        /// <value>The position path.</value>
        /// <seealso cref="LabelsSource"/>
        [Category("Data")]
        public string PositionPath
        {
            get { return (string)GetValue(PositionPathProperty); }
            set { SetValue(PositionPathProperty, value); }
        }

        /// <summary>
        /// Gets or sets the content path for label. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// When <see cref="LabelsSource"/> property is set, content path used to determine content for label.
        /// </remarks>
        [Category("Data")]
        public string ContentPath
        {
            get { return (string)GetValue(ContentPathProperty); }
            set { SetValue(ContentPathProperty, value); }
        }
        #endregion

        private static void CreateXmlSourceListWrapper(XmlNode itemsSource, ref IList source)
        {
            XmlNode xmlData = itemsSource as XmlNode;
            if (xmlData != null)
            {
                source.Add(xmlData);
                CreateXmlSourceListWrapper(xmlData.NextSibling, ref source);
            }
        }
        /// <summary>
        /// Gets the visible labels. Internal property.
        /// </summary>
        /// <value>The visible labels.</value>
        //[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ChartAxisLabelsCollection VisibleLabels
        {
            get
            {
                return m_visibleLables;
            }
        }


        /// <summary>
        /// Identifies the ActualRange dependency property.
        /// </summary>
        public static readonly DependencyProperty ActualRangeProperty =
          DependencyProperty.Register("ActualRange", typeof(DoubleRange), typeof(ChartAxis), new PropertyMetadata(new DoubleRange(0, 1), null, new CoerceValueCallback(OnCoerceActualRange)));

        /// <summary>
        /// Gets the Actual Range of the Axis
        /// </summary>
        public DoubleRange ActualRange
        {
            get { return (DoubleRange) GetValue(ActualRangeProperty); }
            internal set { SetValue(ActualRangeProperty, value); }
        }

        /// <summary>
        /// Gets the parent area for axis.
        /// </summary>
        /// <remarks>
        /// This property is being set by Chart's system internally.
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ChartArea Area
        {
            [DebuggerStepThrough]
            get
            {
                return m_owner;
            }

            internal set
            {
                if (m_owner != value)
                {
                    if (m_owner == null || value == null)
                    {
                        m_owner = value;
                    }
                    else if (m_owner.IsSync == false)
                    {
                        throw new ArgumentException("Axes already has owner");
                    }
                }
                else
                {
                    m_owner = value;
                }
            }
        }

        /// <summary>
        /// Gets the ticks point.
        /// </summary>
        /// <value>The ticks point.</value>
        internal ArrayList TicksPoint
        {
            get
            {
                return m_ticksPoint;
            }
        }

        //Commented for fixing issue in SD13924
        /// <summary>
        /// Gets the minimal zoom factor.
        /// </summary>
        /// <value>The minimal zoom factor.</value>
        //[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]

        public double MinimalZoomFactor
        {
            get { return (double)GetValue(MinimalZoomFactorProperty); }
            set { SetValue(MinimalZoomFactorProperty, value); }
        }

        /// <summary>
        /// Initializes m_niceRange
        /// </summary>
        private DoubleRange NiceRange
        {
            get { return m_niceRange; }
            set
            {
                m_niceRange = value;
                if (m_niceRange != DoubleRange.Empty)
                    ActualRange = m_niceRange;
            }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for MinimalZoomFactor.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty MinimalZoomFactorProperty =
            DependencyProperty.Register("MinimalZoomFactor", typeof(double), typeof(ChartAxis), new PropertyMetadata(0.001d, new PropertyChangedCallback(OnMinimalZoomFactorChanged), new CoerceValueCallback(OnCoerceMinimalZoomFactor)));

        
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes static members of the <see cref="ChartAxis"/> class.
        /// </summary>
        static ChartAxis()
        {
            Syncfusion.Licensing.EnvironmentTest.ValidateLicense(typeof(ChartAxis));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChartAxis), new FrameworkPropertyMetadata(typeof(ChartAxis)));
            ////Iterating over all mouse events being raised on ChartCartesianAxisPanel in order to map 
            ////their route to corresponding ChartAxis.
            foreach (RoutedEvent mouseEvent in EventManager.GetRoutedEventsForOwner(typeof(Mouse)))
            {
                EventManager.RegisterClassHandler(typeof(ChartCartesianAxisPanel), mouseEvent, new RoutedEventHandler(RaiseMouseEvent));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartAxis"/> class.
        /// </summary>
        public ChartAxis()
        {
            this.StripLines = new ChartStripLinesCollection();
            StripLines.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnStripLinesChanged);
            this.BreakRange = new ChartBreakRange(this);
            this.BreakRange.Changed += new EventHandler(this.OnBreakRangeChanged);
            m_customLables.CollectionChanged += new NotifyCollectionChangedEventHandler(OnCustomLablesCollectionChanged);
            m_customLables.m_chartAxis = this;
            m_visibleLables.m_chartAxis = this;

        }


        #endregion

        #region Public methods



        
          
        /// <summary>
        /// This method compute and set nice range.
        /// </summary>
        /// <param name="range">Base range.</param>
        internal void SetNiceRange(DoubleRange range)
        {
           
            m_isUpdating = true;

            ////if (this.IsLogarithmic && range.Start <= 0)
            ////{
            ////    {
            ////        range = new DoubleRange(1, range.End);
            ////    }
            ////}
            var niceInterval = 0d;
            niceInterval = !double.IsNaN(this.BaseInterval) ? this.BaseInterval : GetNiceInterval(range, this.DesiredIntervalsCount, this.IsLogarithmicLabels, this.Indexed);

            m_niceInterval = niceInterval < this.MinimumInterval ? this.MinimumInterval : niceInterval;

            m_niceIntervalOffset = GetNiceIntervalOffset(range, m_niceInterval, this.IsLogarithmicLabels, this.Indexed);
            if (ValueType== ChartValueType.Logarithmic)
            {
                NiceRange = CalculateLogarithmicPadding(range, this.RangePadding, ref m_niceInterval, ref m_niceIntervalOffset);
            }
            else
            {
                NiceRange = CalculatePadding(range, this.RangePadding, ref m_niceInterval, ref m_niceIntervalOffset);
            }

            m_requestedRange = range;
            m_niceInterval = GetNiceInterval(m_requestedRange, this.DesiredIntervalsCount, this.IsLogarithmicLabels, this.Indexed);
            this.Invalidate();
        }

        /// <summary>
        /// Zooms the range.
        /// </summary>
        /// <param name="zoomRange">The zoom range.</param>
        internal void ZoomRange(DoubleRange? zoomRange)
        {
            // CoerceValue(RangeProperty);
            DoubleRange baseRange = this.InternalRange;
            if (baseRange.IsEmpty)
            {
                baseRange = NiceRange;
            }
            DoubleRange zoomdr = (DoubleRange)zoomRange;
            m_isUpdating = true;
            this.ZoomFactor = zoomdr.Delta / baseRange.Delta;
            this.ZoomPosition = (zoomdr.Start - baseRange.Start) / baseRange.Delta;

            m_isUpdating = false;

            this.Invalidate();
        }

        /// <summary>
        /// Zooms by center.
        /// </summary>
        /// <param name="factor">The factor.</param>
        internal void ZoomCenter(double factor)
        {
            if (this.ZoomFactor != factor)
            {
                m_isUpdating = true;

                double zPos = this.ZoomPosition;
                double zFactor = this.ZoomFactor;

                this.ZoomFactor = factor;
                 if (this.Area.IsSync == true)
                {
                    if (this.Area.PrimaryAxis == this)
                    {
                        if (this.Area.PrimaryChartArea.ChartAreaParent.Areas[0] == this.Area)
                           this.ZoomPosition = (zPos + 0.5 * zFactor) - factor / 2;
                        else
                            this.ZoomPosition = this.Area.PrimaryChartArea.ChartAreaParent.Areas[0].PrimaryAxis.ZoomPosition;
                    }
                }
                else
                {
                    this.ZoomPosition = (zPos + 0.5 * zFactor) - factor / 2;
                }
                m_isUpdating = false;

                this.Invalidate();
            }
        }

        /// <summary>
        /// Multiplies the zoom factor by center.
        /// </summary>
        /// <param name="mulFactor">The mul factor.</param>
        internal void MulZoomCenter(double mulFactor)
        {
            this.ZoomCenter(mulFactor * this.ZoomFactor);
        }

        /// <summary>
        /// Resets the zoom.
        /// </summary>
        internal void ZoomReset()
        {
            if (this.ZoomFactor != 1d || this.ZoomPosition != 0)
            {
                m_isUpdating = true;
                this.ZoomFactor = 1;
                this.ZoomPosition = 0;
                m_isUpdating = false;

                this.Invalidate();
            }
        }

        /// <summary>
        /// Converts value of passed point co-ordinate to control related co-ordinate.
        /// </summary>
        /// <param name="value">The value of point on axis.</param>
        /// <returns>Co-ordinate of point related to chart control.</returns>
        /// <seealso cref="ChartAxis.CoefficientToValue"/>
        public double ValueToCoefficient(double value)
        {
            double result = double.NaN;
            var start = VisibleRange.Start;
            var delta = VisibleRange.Delta;
            var primarySeries = this.Area != null ? this.Area.PrimarySeries : null;
            ////if (this.IsLogarithmic)
            ////{
            ////    double logDelta = LogarithmicVisibleRange.Delta;
            ////    double logStart = LogarithmicVisibleRange.Start;

            ////    if (value >= 0)
            ////    {
            ////        result = (Math.Log(value, LogarithmicBase) - logStart) / logDelta;
            ////    }
            ////    else
            ////    {
            ////        result = double.NaN;
            ////        ////throw new ArgumentException("value should be > 0, for correct Math.Log10(value) output.");
            ////    }
            ////}
            ////else
            ////{
            if ((this.BreakRange.IsEmpty && this.BreakRange.BreaksMode == ChartBreaksModes.Manual) || !EnableBreaks)
            {
                ////if (VisibleRange.Delta == 0)
                ////{
                ////  m_visibleRange = new DoubleRange(m_visibleRange.Start - .5, m_visibleRange.End + .5);
                ////}
                result = (value - start) / delta;
            }
            else if(EnableBreaks)
            {
                switch (this.BreakRange.m_breaksMode)
                {
                    case ChartBreaksModes.None:
                        result = (value - VisibleRange.Start) / VisibleRange.Delta;
                        break;

                    case ChartBreaksModes.Auto:
                        result = this.BreakRange.AutoValueToCoeficient(value);
                        break;

                    case ChartBreaksModes.Manual:
                        result = this.BreakRange.Subtract(value - VisibleRange.Start) / (VisibleRange.Delta - this.BreakRange.SumBreaks);
                        break;
                }
            }

            if (primarySeries != null)
            {      
                //Include condition to check RangeCalculationMode != AdjusantAcrossChartType, because the below calculation is not needed when it is AdjusantacrossChartType.
               if ((primarySeries.Type == ChartTypes.Radar || primarySeries.Type == ChartTypes.Polar) && Area.PrimaryAxis == this && Area.PrimaryAxis.RangeCalculationMode != Windows.Chart.RangeCalculationMode.AdjustAcrossChartTypes)               
                {
                   /*Added 1 to delta is because the first and last labels are at same position, so to include one extra interval as like in Excel added 1 to delta
                     add also based on previous calculation the axis line drawn with uneven angle for radar chart. This also fixes that angle issue   */
                    result *=  1 - 1 / (delta + 1);
                }
            }
            ////}

            return this.IsInversed ? 1d - result : result;
        }

        internal double ValueToCoefficient1(double value, bool isinversed)
        {
            double result = double.NaN;
            var start = VisibleRange.Start;
            var delta = VisibleRange.Delta;
            //var primarySeries = this.Area != null ? this.Area.PrimarySeries : null;
            ////if (this.IsLogarithmic)
            ////{
            ////    double logDelta = LogarithmicVisibleRange.Delta;
            ////    double logStart = LogarithmicVisibleRange.Start;

            ////    if (value >= 0)
            ////    {
            ////        result = (Math.Log(value, LogarithmicBase) - logStart) / logDelta;
            ////    }
            ////    else
            ////    {
            ////        result = double.NaN;
            ////        ////throw new ArgumentException("value should be > 0, for correct Math.Log10(value) output.");
            ////    }
            ////}
            ////else
            ////{
            if (this.BreakRange.IsEmpty)
            {
                ////if (VisibleRange.Delta == 0)
                ////{
                ////  m_visibleRange = new DoubleRange(m_visibleRange.Start - .5, m_visibleRange.End + .5);
                ////}
                result = (value - start) * (1 / delta);
            }
            else
            {
                result = this.BreakRange.Subtract(value - start) / (delta - this.BreakRange.SumBreaks);
            }

            //if (primarySeries != null)
            //{
            //    if (primarySeries.Type == ChartTypes.Radar && Area.PrimaryAxis == this)
            //    {
            //        result *= 1 - 1 / delta;
            //    }
            //}
            ////}

            return isinversed ? 1d - result : result;
        }
        /// <summary>
        /// Converts co-ordinate of point related to chart control to axis units.
        /// </summary>
        /// <param name="value">The absolute point value.</param>
        /// <returns>The value of point on axis.</returns>
        /// <seealso cref="ChartAxis.ValueToCoefficient"/>
        public double CoefficientToValue(double value)
        {
            double result = double.NaN;

            value = this.IsInversed ? 1d - value : value;

            ////if (this.IsLogarithmic)
            ////{
            ////    double logDelta = LogarithmicVisibleRange.Delta;
            ////    double logStart = LogarithmicVisibleRange.Start;

            ////    result = Math.Pow(logStart + logDelta * Math.Log(value, LogarithmicBase), LogarithmicBase);
            ////}
            ////else
            ////{
            if ((this.BreakRange.IsEmpty && this.BreakRange.BreaksMode == ChartBreaksModes.Manual) || !EnableBreaks)
            {
                result = VisibleRange.Start + VisibleRange.Delta * value;
            }
            else if(EnableBreaks)
            {
                switch (this.BreakRange.m_breaksMode)
                {
                    case ChartBreaksModes.None:
                        result = VisibleRange.Start + VisibleRange.Delta * value;
                        break;

                    case ChartBreaksModes.Auto:
                        result = this.BreakRange.AutoCoeficientToValue(value);
                        break;

                    case ChartBreaksModes.Manual:
                        result = this.BreakRange.Subtract(value - VisibleRange.Start) / (VisibleRange.Delta - this.BreakRange.SumBreaks);
                        break;
                }
            }
            ////}

            return result;
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Chaeck whether need for update or not
        /// </summary>
        /// <returns></returns>
        private bool CheckForUpdate(DependencyProperty property)
        {
            bool affectUpdate = (property == OriginProperty || property == LabelsModeProperty || property == IndexedProperty || property == ContentPathProperty ||
                                property == PositionPathProperty || property == PositionPathProperty || property == LabelsSourceProperty || property == LabelDateTimeFormatProperty
                                || property == LabelFormatProperty || property == IntervalOffsetProperty || property == BaseIntervalProperty || property == IntervalProperty || property == IsLogarithmicLabelsProperty
                                || property == DesiredIntervalsCountProperty || property == IgnoreRangePaddingsOnZoomProperty || property == InternalRangeProperty || property == RangeProperty
                                || property == OriginProperty || property == ZoomFactorProperty || property == ZoomPositionProperty || property == IsInversedProperty || property == IgnoreRangePaddingsOnZoomProperty
                                || property == LabelTimeSpanFormatProperty ||
                                property == IsAutoSetRangeProperty ) ? true : false;

            return affectUpdate;

        }
        private double CalculateNiceInternalEx(double desiredInterval)
        {
            double mul = Math.Pow(10d, Math.Floor(Math.Log10(desiredInterval)));
            double intervalDiv = desiredInterval / mul;
            double minDelta = double.MaxValue;

            foreach (double div in c_intervalDivs)
            {
                double delta = Math.Abs(div - intervalDiv);

                if (delta < minDelta)
                {
                    minDelta = delta;
                    desiredInterval = div * mul;
                }
            }

            return desiredInterval;
        }

        internal bool flag = false;
        internal double logZoomfactor = 0d;
        internal double logZoomPosition = 0d;
        internal double scrollHeight2 = 2.5;
        internal double scrollHeight1 = 2.5;
        /// <summary>
        /// Calculates the visible range.
        /// </summary>
        private void CalculateVisibleRange()
        {

            CoerceValue(InternalRangeProperty);
            DoubleRange baseRange = this.InternalRange;

            if (baseRange.IsEmpty)
            {
                baseRange = NiceRange;
            }

            if (baseRange.IsEmpty)
            {
                m_visibleRange = new DoubleRange(0, 1);
            }
            else if (this.ZoomFactor != 1d)
            {
                #region commentedcode
                ////if (IsLogarithmic == true)
                ////{
                ////    if (logZoomfactor == ZoomFactor)
                ////    {
                ////        double value = ZoomPosition;
                ////        if (logZoomPosition > ZoomPosition && ZoomPosition < scrollHeight1)
                ////        {
                ////            double logEnd = m_visibleRange.Start;
                ////            double logStart = logEnd / 10;
                ////            if (logStart >= baseRange.Start)
                ////            {
                ////                m_visibleRange = new DoubleRange(logStart, logEnd);
                ////            }
                ////            if (ZoomPosition - 0.05 > 0)
                ////            {
                ////                scrollHeight1 = (ZoomPosition - 0.05);
                ////            }
                ////        }
                ////        else if (logZoomPosition < ZoomPosition && ZoomPosition < scrollHeight2)
                ////        {
                ////            double logStart = m_visibleRange.End;
                ////            double logEnd = logStart * 10;
                ////            if (logEnd <= baseRange.End)
                ////            {
                ////                m_visibleRange = new DoubleRange(logStart, logEnd);
                ////            }
                ////            scrollHeight2 = (ZoomPosition + 0.05);
                ////        }
                ////        logZoomPosition = ZoomPosition;
                ////    }
                ////    else
                ////    {
                ////        DoubleRange zoomingRange = IgnoreRangePaddingsOnZoom ? m_requestedRange : baseRange;
                ////        double start = zoomingRange.Start + ZoomPosition * zoomingRange.Delta;
                ////        double end = start + ZoomFactor * zoomingRange.Delta;
                ////        m_visibleRange = new DoubleRange(start, end);
                ////        if (RangePadding != ChartRangePaddingType.None)
                ////        {
                ////            double logStart = Math.Log(m_visibleRange.Start, LogarithmicBase);
                ////            double logEnd = Math.Log(m_visibleRange.End, LogarithmicBase);

                ////            double mulS = ChartMath.Round(logStart, 1, false);
                ////            double x = Math.Floor(Math.Log(-logStart, LogarithmicBase));
                ////            double mulE = ChartMath.Round(logEnd, 1, true);
                ////            m_visibleRange = new DoubleRange(Math.Pow(LogarithmicBase, mulS), Math.Pow(LogarithmicBase, mulE));
                ////        }


                ////        logZoomPosition = ZoomPosition;
                ////        logZoomfactor = ZoomFactor;
                ////    }
                ////}
                ////else
                ////{
                #endregion
                DoubleRange zoomingRange = IgnoreRangePaddingsOnZoom ? m_requestedRange : baseRange;
                if (EnableAutoScrolling.HasValue)
                {
                    if (EnableAutoScrolling.Value == true)
                    {
                        m_autoScrollingZoomFactor = AutoScrollingDelta / zoomingRange.Delta;
                        if (this.ZoomFactor != m_autoScrollingZoomFactor)
                        {
                            this.ZoomFactor = m_autoScrollingZoomFactor;
                            this.ZoomPosition = 1;
                        }
                    }
                    else if (EnableAutoScrolling.Value == false)
                    {
                        this.ZoomFactor = 1;
                    }
                }
                double start = zoomingRange.Start + ZoomPosition * zoomingRange.Delta;                
                double end = start + ZoomFactor * zoomingRange.Delta;
                if (start < baseRange.Start) { end = end + (baseRange.Start - start); start = baseRange.Start;}
                if(end > baseRange.End) { start = start - (end - baseRange.End); end = baseRange.End; }
                m_visibleRange = new DoubleRange(start, end);
                ////}

            }
            else if(EnableAutoScrolling.HasValue)
            {
                if (EnableAutoScrolling.Value == true && AutoScrollingDelta != 0)
                {
                    DoubleRange scrollingRange = IgnoreRangePaddingsOnZoom ? m_requestedRange : baseRange;
                    m_autoScrollingZoomFactor = AutoScrollingDelta / scrollingRange.Delta;
                    this.ZoomFactor = m_autoScrollingZoomFactor;
                    this.ZoomPosition = 1;
            	}
            }
            else
            {
                m_visibleRange = baseRange;

                if (this.Orientation == Orientation.Horizontal)
                {
                    if (this.Area != null)
                    {
                        if (this.Area.SyncChartArea != null)
                        {
                            if (this.IsAutoSetRange == true&&this.Area==this.Area.SyncChartArea.Areas[this.Area.SyncChartArea.Areas.Count-1])
                            {
                                this.Area.SyncChartArea.SetPrimaryAxisRange();
                                if (this.Area.SyncChartArea.PrimaryAxis.m_visibleRange.IsEmpty==false)
                                m_visibleRange = this.Area.SyncChartArea.PrimaryAxis.m_visibleRange;
                            }
                        }
                    }
                }
            }

            if (!double.IsNaN(this.BaseInterval))
            {
               if (this.m_visibleRange.Inside(this.BaseInterval) || this.ValueType ==  ChartValueType.DateTime || (!this.IsAutoSetRange && this.VisibleRange.Delta >= this.BaseInterval) || (this.IsAutoSetRange && this.ZoomFactor < 1))
                {
                    if (this.ValueType == ChartValueType.DateTime && checkDateTimeIntervalChange == true)
                    {
                        if ((TimeSpan)DateTimeInterval < this.MinimumDateTimeInterval)
                        {
                            this.BaseInterval = (DateTime.Now + (TimeSpan)this.MinimumDateTimeInterval).ToOADate() - DateTime.Now.ToOADate();
                        }
                        else
                        {
                            this.BaseInterval = (DateTime.Now + (TimeSpan)DateTimeInterval).ToOADate() - DateTime.Now.ToOADate();
                        }
                        m_visibleInterval = this.EnableAutoIntervalOnZooming ? this.BaseInterval * this.ZoomFactor : this.BaseInterval;
                        if (m_owner!=null && m_owner.PrimarySeries == null && IsAutoSetRange == true)
                       {
                           m_visibleInterval = Math.Round(m_visibleInterval,3)>0?m_visibleInterval: 0.2;
                       }
                    }
                    else
                    m_visibleInterval = this.EnableAutoIntervalOnZooming ? this.BaseInterval * this.ZoomFactor : this.BaseInterval;
                }
                else
                {
                    m_visibleInterval = this.EnableAutoIntervalOnZooming ? GetNiceInterval(m_visibleRange, this.DesiredIntervalsCount, this.IsLogarithmicLabels, this.Indexed):this.BaseInterval; //m_niceInterval;
                    if (m_visibleInterval < MinimumInterval)
                        m_visibleInterval = MinimumInterval;
                    m_visibleIntervalOffset = m_niceIntervalOffset;
                }

               // m_visibleInterval = this.Interval * this.ZoomFactor;
                if (Indexed)
                {
                    m_visibleInterval = Math.Max(m_visibleInterval, 1);
                }
                else if(m_visibleInterval<=0)
                {
                    if (MinimumInterval != double.NaN && m_visibleInterval < MinimumInterval)
                    {
                        m_visibleInterval = MinimumInterval;
                    }
                    else
                    {
                        m_visibleInterval = Math.Max(m_visibleInterval, 0.2);
                    }
                }

                if (!double.IsNaN(this.IntervalOffset))
                {
                    m_visibleIntervalOffset = this.IntervalOffset;
                }
                else
                {
                    m_visibleIntervalOffset = (baseRange.Start - m_visibleRange.Start) % m_visibleInterval;
                }
            }
            else if (Double.IsNaN(this.BaseInterval) && this.Area!=null && this.Area.PrimarySeries != null && this == this.Area.SecondaryAxis &&
                (this.Area.PrimarySeries.Type == ChartTypes.StackingColumn100 || this.Area.PrimarySeries.Type == ChartTypes.StackingBar100))
            {
                m_visibleInterval = ChartStackingColumn100Type.GetShowValueAsProbability(this.Area) ? 0.2 : 20;
                m_visibleIntervalOffset = m_niceIntervalOffset;
            }
            else
            {               
                m_visibleInterval = GetNiceInterval(m_visibleRange, this.DesiredIntervalsCount, this.IsLogarithmicLabels, this.Indexed); //m_niceInterval;
                if (m_visibleInterval < MinimumInterval)
                    m_visibleInterval = MinimumInterval;
                m_visibleIntervalOffset = m_niceIntervalOffset;
            }

            if (this.ZoomFactor != 1d && !this.IsFractionEnabledOnZoom)
            {
                m_visibleInterval = Math.Floor(m_visibleInterval);
                if (m_visibleInterval < 1)
                {
                    m_visibleInterval = 1;
                }

                ////if (IsLogarithmic)
                ////{
                ////    m_visibleRange = this.CalculateLogarithmicPadding(m_visibleRange, this.RangePadding, ref m_visibleInterval, ref m_visibleIntervalOffset);
                ////}
                ////else
                ////{
                m_actualRangeValue = m_visibleRange;
                m_visibleRange = this.CalculatePadding(m_visibleRange, this.RangePadding, ref m_visibleInterval, ref m_visibleIntervalOffset);
                ////}
            }

            ////m_logarihmicVisibleRange = new DoubleRange(Math.Log(m_visibleRange.Start, LogarithmicBase), Math.Log(m_visibleRange.End, LogarithmicBase));
            m_logarihmicVisibleRange = m_visibleRange;
            m_visibleInterval = this.IsLogarithmic && !this.IsFractionEnabledOnZoom ? Math.Ceiling(m_visibleInterval) : m_visibleInterval;
            //m_visibleRange = this.CalculatePadding(m_requestedRange, this.RangePadding, ref m_visibleInterval, ref m_visibleIntervalOffset);

            this.SetValue(VisibleRangePropertyKey, m_visibleRange);
            this.SetValue(VisibleIntervalPropertyKey, m_visibleInterval);
            this.SetValue(VisibleIntervalOffsetPropertyKey, m_visibleIntervalOffset);
            m_visibleRange = this.GetOriginCenteredRange(m_visibleRange);
            #region Feature'ShowAllLabels'
            bool isdefault = (Indexed ? (RangeCalculationMode == RangeCalculationMode.AdjustAcrossChartTypes ? (!m_showAllLabelsChanged ? true : false) : false) : false);
            if(isdefault || ShowAllLabels)
            {       
              if(this.Area!=null && this.Area.Series.Count > 0)
                {
                    if (this.Area.Series[0].Type == ChartTypes.Bar ||
                                   this.Area.Series[0].Type == ChartTypes.RotatedSpline ||
                                   this.Area.Series[0].Type == ChartTypes.StackingBar ||
                                   this.Area.Series[0].Type == ChartTypes.StackingBar100 ||
                                   this.Area.Series[0].Type == ChartTypes.Tornado ||
                                   this.Area.Series[0].Type == ChartTypes.Gantt)
                {
                    if (Orientation == Orientation.Vertical)
                    {
                        double pos = m_visibleInterval;
                        m_ticksCount = 0;
                        while (pos < m_visibleRange.End)
                        {
                            m_ticksCount++;
                            pos = pos + m_visibleInterval;
                        }
                    }
                        m_visibleInterval = this.Orientation == Orientation.Horizontal ? m_visibleInterval : (ShowAllLabels ? 1: isdefault?1:m_visibleInterval);
                }
                else
                {
                    if (Orientation == Orientation.Horizontal)
                    {
                        double pos = m_visibleInterval;
                        m_ticksCount = 0;
                        while (pos < m_visibleRange.End)
                        {
                            m_ticksCount++;
                            pos = pos + m_visibleInterval;
                        }
                    }
                        m_visibleInterval = this.Orientation == Orientation.Vertical ? m_visibleInterval : (ShowAllLabels ? 1 : isdefault?1:m_visibleInterval);
                }
            }
          }
            #endregion

        }

        /// <summary>
        /// Calculates the visible lables.
        /// </summary>
        private void CalculateVisibleLables()
        {
            // This is necessary to prevent a crash in the designer when setting Interval to something for Axis.
            if (m_owner == null)
            {
                return;
            }
            else if (ValueType == ChartValueType.Logarithmic && !IsLogarithmic||ValueType== ChartValueType.Logarithmic && LogarithmicBase==1)
            {
                return;
            }
            int temp = 0;
            IChartData data = (m_owner.PrimarySeries != null) ? m_owner.PrimarySeries.Data : null;
            
            for (int i = 0; i < Area.Series.Count; i++)
            {
                if (Area.Series[i].Data != null)
                {
                    if (temp < Area.Series[i].Data.Count)
                    {
                        temp = Area.Series[i].Data.Count;
                        data = Area.Series[i].Data;
                    }
                }
            }
            
            if (data != null && m_owner.PrimarySeries != null && m_owner.PrimarySeries.DataModel != null)
            {
                data.ChartXValueType = m_owner.PrimarySeries.DataModel.XValueType;
            }
            bool smallTicksRequired = true;
            if (data != null && data.Count > 0)
            {
                if (data[0] != null)
                {
                    if ((double.IsNaN(data[0].X) && data.ChartXValueType == ChartValueType.String) || (double.IsNaN(data[0].X) && Indexed && data.ChartXValueType == ChartValueType.String))
                    {
                        smallTicksRequired = false;
                    }
                }
            }
            //SD15248 Chart labels showing same values multiple times in secondary axis.
            Dictionary<double, string> repeatedvalue = new Dictionary<double, string>();
            SortedList<double, ChartAxisLabel> visibleLabels =
              new SortedList<double, ChartAxisLabel>(new ChartDoubleComparer(this.IsInversed));
            ChartAxisLabelsMode labelsMode = this.LabelsMode;
            m_ticksPoint.Clear();
            m_visibleLables.Clear();

            if(this.EnableBreaks && this.BreakRange.m_breaksMode == ChartBreaksModes.Auto)
            {
                double pos1 = 0;
                for (int i = 0, ci = this.BreakRange.m_breakSegments.Count; i < ci; i++)
                {
                    DoubleRange breakSegment = this.BreakRange.m_breakSegments[i];
                    pos1 = breakSegment.Start;

                    while (pos1 <= breakSegment.End)
                    {
                        if (!visibleLabels.ContainsKey(this.ValueToCoefficient(pos1)))
                            visibleLabels.Add(this.ValueToCoefficient(pos1), new ChartAxisLabel(pos1, this.GetLabelContent(pos1)));
                        pos1 += this.VisibleInterval;
                    }
                }
            }
            else
            {
                #region Data Source Labels
                if ((labelsMode & ChartAxisLabelsMode.DataSource) == ChartAxisLabelsMode.DataSource
                   && this.LabelsSource != null)
                {
                    #region Data Source Labels
                    int index = 0;
                    if (!(this.LabelsSource is XmlElement))
                    {
                        foreach (object obj in m_AxisDataModel.GetSourceList(this.LabelsSource))
                        {
                            index++;
                            double position = ChartDataUtils.GetDoubleByPath(obj, this.PositionPath);
                            if (obj is DataRowView || obj is DataRow)
                                position = Indexed ? index : position;
                            //if (!((labelsMode & ChartAxisLabelsMode.DataSource) == ChartAxisLabelsMode.DataSource))
                            {
                            if (position % m_visibleInterval != 0 && this.LabelsMode == ChartAxisLabelsMode.Default && this.ZoomFactor == 1)
                            {
                                continue;
                            }
                            else if (Indexed)
                            {
                                position--;
                            }
                            }

                            if (this.VisibleRange.Inside(position))
                            {
                                if (string.IsNullOrEmpty(this.ContentPath))
                                {
                                    visibleLabels[position] = new ChartAxisLabel(position, obj);
                                }
                                else
                                {
                                    visibleLabels[position] = new ChartAxisLabel(position, ChartDataUtils.GetObjectByPath(obj, this.ContentPath));
                                }
                            }
                        }
                    }
                    else
                    {
                        IList source = new List<XmlNode>();
                        CreateXmlSourceListWrapper((this.LabelsSource as XmlElement), ref source);
                        foreach (object obj in m_AxisDataModel.GetSourceList(source))
                        {
                            index++;
                            double position = ChartDataUtils.GetDoubleByPath(obj, this.PositionPath);
                            if (obj is DataRowView || obj is DataRow)
                                position = Indexed ? index : position;
                            //if (Indexed)
                            //{
                            if (position % m_visibleInterval != 0 && this.LabelsMode == ChartAxisLabelsMode.Default && this.ZoomFactor == 1)
                            {
                                continue;
                            }
                            else if (Indexed)
                            {
                                position--;
                            }
                            //}

                            if (this.VisibleRange.Inside(position))
                            {
                                if (string.IsNullOrEmpty(this.ContentPath))
                                {
                                    visibleLabels[position] = new ChartAxisLabel(position, obj);
                                }
                                else
                                {
                                    visibleLabels[position] = new ChartAxisLabel(position, ChartDataUtils.GetObjectByPath(obj, this.ContentPath));
                                }
                            }
                        }

                    }
                   
                    double start =Math.Round(this.VisibleRange.Start) + m_visibleIntervalOffset;
                    double tickpos = start;
                    double tickinterval = m_visibleInterval / (this.SmallTicksPerInterval+1);
                    if (smallTicksRequired && SmallTicksPerInterval > 0 && !IsLogarithmic && tickinterval != 0)
                    {
                        while (tickpos <= Math.Round(this.VisibleRange.End))
                        {
                            m_ticksPoint.Add(tickpos);
                            tickpos += tickinterval;
                        }
                    }
                    #endregion
                }
                #endregion

                if ((labelsMode & ChartAxisLabelsMode.Auto) == ChartAxisLabelsMode.Auto && visibleLabels.Count <= 0)
                {
                    #region Series labels
                    if (m_owner != null && m_owner.PrimarySeries != null && m_owner.PrimarySeries.ChartType != null
                      && m_owner.PrimarySeries.ActualXAxis == this
                      && m_owner.PrimarySeries.ChartType.CustomAxisLabels)
                    {
                        #region Series labels
                        double lastPos = double.MinValue;

                        foreach (ChartSegment segment in m_owner.PrimarySeries.Segments)
                        {
                            ChartAxisLabelInfo? labelInfo = segment.AxisLabelInfo;

                            if (labelInfo.HasValue)
                            {
                                double pos = labelInfo.Value.Position;

                                if (Math.Abs(pos - lastPos) > m_visibleInterval)
                                {
                                    visibleLabels[pos] = new ChartAxisLabel(pos, this.GetLabelContent(labelInfo.Value.ValueToPresent));
                                    lastPos = pos;
                                }
                            }
                        }

                        #endregion
                    }
                    #endregion

                    else if (this.VisibleRange.Delta > 0)
                    {
                        #region Auto labels
                        CoerceValue(InternalRangeProperty);
                        DoubleRange baseRange = this.InternalRange;

                        if (baseRange.IsEmpty)
                        {
                            baseRange = NiceRange;
                        }

                        int roundingValue = Convert.ToInt32(Math.Max(0, -Math.Floor(Math.Log10(VisibleRange.Delta)) + 2));
                        double start = this.VisibleRange.Start;
                        double end = this.VisibleRange.End;
                        double interval = m_visibleInterval;
                        double pos = start + m_visibleIntervalOffset;

                        //String ss = LabelDateTimeFormat;

                        if (ValueType != ChartValueType.DateTime && ValueType != ChartValueType.TimeSpan )
                        {
                            if (roundingValue < 15 && roundingValue > 0)
                            {                         
                                 end = Math.Round(end, roundingValue, MidpointRounding.AwayFromZero);
                                 start = Math.Round(start, roundingValue, MidpointRounding.AwayFromZero);                                
                            }
                          //EnableAutoIntervalOnZooming to false,When Zooming the chart step of zooming lablelvalues show wrong in secondary axis.(calculation wrong this line for step of zooming)
                            //pos = start + m_visibleIntervalOffset;
                            if (roundingValue < 15 && roundingValue > 0)
                            {
                                pos = Math.Round(pos, roundingValue, MidpointRounding.AwayFromZero);
                            }
                        }

                        if (!Indexed && start == baseRange.Start)
                        {
                            visibleLabels[start] = new ChartAxisLabel(start, this.GetLabelContent(start));
                        }

                        if (!Indexed && end == baseRange.End)
                        {
                            visibleLabels[end] = new ChartAxisLabel(end, this.GetLabelContent(end));
                        }
                        else if (!Indexed && end != baseRange.End && IsLogarithmic)
                        {
                            //StartRange Label not render when set some values in Logarithmic Range(750,1500000).
                            visibleLabels[end] = new ChartAxisLabel(end, Convert.ToDouble(this.GetLabelContent(baseRange.End)));
                            visibleLabels[start] = new ChartAxisLabel(start, Convert.ToDouble(this.GetLabelContent(baseRange.Start)));
                        }

                        double ticksPos = pos;
                        if (!IsLogarithmic)
                        {
                            ticksPos -= interval;
                        }
                        else
                        {
                            pos = VisibleRange.Start;
                        }

                        double tickInterval = interval / (SmallTicksPerInterval + 1);
                        double value1 = 1;
                        while (pos <= end)
                        {
                            ticksPos = pos + tickInterval;

                            if (pos >= start)
                            {
                                if (Indexed && data != null)
                                {
                                    int iPos = ((int)Math.Round(pos, MidpointRounding.AwayFromZero));
                                    iPos = iPos < 0 ? 0 : iPos ;
                                    if (iPos <= end && iPos >= start)
                                    {

                                        if (iPos > -1 && iPos < data.Count)
                                        {
                                            if (data[iPos] != null)
                                            {
                                                if (double.IsNaN(data[iPos].X) || data.ChartXValueType == ChartValueType.String)
                                                {
                                                    DateTime datevalue;
                                                    double value;
                                                    switch (this.ValueType)
                                                    {
                                                        case ChartValueType.DateTime:
                                                            {
                                                                if (DateTime.TryParse(data[iPos].StringItem.ToString(), out datevalue))
                                                                {
                                                                    visibleLabels[iPos] = new ChartAxisLabel(iPos, datevalue.ToString(this.LabelDateTimeFormat, CultureInfo.CurrentCulture));
                                                                }
                                                                else if (double.TryParse(data[iPos].StringItem.ToString(), out value))
                                                                {
                                                                    visibleLabels[iPos] = new ChartAxisLabel(iPos, DateTime.FromOADate(value).ToString(this.LabelDateTimeFormat, CultureInfo.CurrentCulture));
                                                                }
                                                                else
                                                                {
                                                                    visibleLabels[iPos] = new ChartAxisLabel(iPos, data[iPos].StringItem);
                                                                }
                                                            }
                                                            break;
                                                        case ChartValueType.TimeSpan:
                                                            {
                                                                TimeSpan ts;
                                                                if (TimeSpan.TryParse(data[iPos].StringItem.ToString(), out ts))
                                                                {
                                                                    visibleLabels[iPos] = new ChartAxisLabel(iPos, FormatTimeSpan(ts));
                                                                }
                                                                else if (double.TryParse(data[iPos].StringItem.ToString(), out value))
                                                                {
                                                                    visibleLabels[iPos] = new ChartAxisLabel(iPos, DateTime.FromOADate(value).ToString(this.LabelTimeSpanFormat, CultureInfo.CurrentCulture));
                                                                }
                                                                else
                                                                {
                                                                    visibleLabels[iPos] = new ChartAxisLabel(iPos, data[iPos].StringItem);
                                                                }
                                                            }
                                                            break;
                                                        default:
                                                            {
                                                                if (data[iPos].StringItem != null && double.TryParse(data[iPos].StringItem.ToString(), out value))
                                                                {
                                                                    visibleLabels[iPos] = new ChartAxisLabel(iPos, value.ToString(this.LabelFormat, CultureInfo.CurrentCulture));
                                                                }
                                                                else
                                                                {
                                                                    visibleLabels[iPos] = new ChartAxisLabel(iPos, data[iPos].StringItem);
                                                                }
                                                            }
                                                            break;
                                                    }
                                                }
                                                else
                                                {
                                                    visibleLabels[iPos] = new ChartAxisLabel(iPos, this.GetLabelContent(data[iPos].X));
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (ValueType == ChartValueType.Logarithmic)
                                    {
                                        if (LogarithmicIntervalType == LogarithmicType.Value)
                                        {
                                            double logtickstart = Math.Pow(this.LogarithmicBase, start);
                                            double logtickend = Math.Pow(this.LogarithmicBase, end);
                                            double logtickInterval =((logtickend) / (logtickend/this.LogarithmicInterval));
                                            double logtickPos = logtickstart;
                                            
                                            while (logtickPos < logtickend)
                                            {
                                                value1 = Math.Log(logtickPos, LogarithmicBase);
                                                visibleLabels[value1] = new ChartAxisLabel(value1, this.GetLabelContent(value1));
                                                logtickPos += logtickInterval;
                                            }
                                            
                                        }
                                        else
                                        visibleLabels[pos] = new ChartAxisLabel(pos, this.GetLabelContent(pos));                                    

                                    }
                                    else
                                    visibleLabels[pos] = new ChartAxisLabel(pos, this.GetLabelContent(pos)); 
                                }
                               
                              if (this.HideRepeatedLabels)
                              {
                                 if (visibleLabels.Count > 0 && visibleLabels.ContainsKey(pos))
                                     {
                                      repeatedvalue[pos] = visibleLabels[pos].Content.ToString();
                                      double PrePosition = pos - interval;
                                      PrePosition = Math.Round(PrePosition,1, MidpointRounding.AwayFromZero);
                                        if (visibleLabels.ContainsKey(PrePosition))
                                        {
                                            if (repeatedvalue[pos] == repeatedvalue[PrePosition])
                                            {
                                                visibleLabels[pos] = new ChartAxisLabel(pos, "");
                                            }
                                        }
                                    }
                                }
                            }

                            if (IsLogarithmic)
                            {
                                if (LogarithmicIntervalType == LogarithmicType.Value)
                                    pos += end;
                                else
                                    pos += interval;
                            }
                            else
                            {
                                pos += interval;
                            }

                            if (smallTicksRequired && !IsLogarithmic)
                            {
                                bool ticksRoundingRequired = true;
                                int digits = Math.Max(0, -(int)Math.Floor(Math.Log10(tickInterval)) + 2);
                                if (ValueType != ChartValueType.DateTime && ValueType != ChartValueType.TimeSpan)
                                {
                                    if (digits < 15)
                                    {
                                        //comment here
                                        pos = Math.Round(pos, digits, MidpointRounding.AwayFromZero);
                                    }

                                    ticksRoundingRequired = false;
                                }
                                int i = 0;
                                while (ticksPos < pos && ticksPos <= end)
                                {
                                    if (i >= SmallTicksPerInterval)
                                    {
                                        break;
                                    }
                                    m_ticksPoint.Add(ticksPos);
                                    ticksPos += tickInterval;
                                    if (!ticksRoundingRequired && digits < 15)
                                    {
                                        ticksPos = Math.Round(ticksPos, digits, MidpointRounding.AwayFromZero);
                                    }
                                    i++;
                                }
                            }
                            else if (smallTicksRequired && IsLogarithmic)
                            {
                                //if (pos <= end)
                                {
                                    double logtickstart = Math.Pow(this.LogarithmicBase, pos - this.m_visibleInterval);
                                    double logtickend = Math.Pow(this.LogarithmicBase, pos);
                                    double logend = Math.Pow(this.LogarithmicBase, end);
                                    double logtickInterval = (logtickend - logtickstart) / (this.SmallTicksPerInterval + 1);
                                    double logtickPos = logtickstart;
                                    while (logtickPos < logtickend && logtickPos<logend)
                                    {
                                        m_ticksPoint.Add(Math.Log(logtickPos, this.LogarithmicBase));
                                        logtickPos += logtickInterval;

                                    }
                                }
                            }
                        }

                        if (smallTicksRequired && !IsLogarithmic)
                        {
                            while (ticksPos <= end)
                            {
                                m_ticksPoint.Add(ticksPos);
                                ticksPos += tickInterval;
                            }
                        }


                        #endregion
                    }
                }

                if ((labelsMode & ChartAxisLabelsMode.Custom) == ChartAxisLabelsMode.Custom)
                {
                    #region Custom labels
                    foreach (ChartAxisLabel label in m_customLables)
                    {
                        if (this.VisibleRange.Inside(label.Position))
                        {
                            visibleLabels[label.Position] = label;
                        }
                    }
                    #endregion
                }
                int seriesCount = this.Area != null ? this.Area.Series.Count : 0;
                bool isSorted = seriesCount > 0 ? this.Area.Series[seriesCount - 1].IsSortData : false;
                bool isIndexed = seriesCount > 0 ? this.Area.Series[seriesCount - 1].IsIndexed : false;
                Direction sortDirection = seriesCount > 0 ? this.Area.Series[seriesCount - 1].SortDirection : Direction.Ascending;
                if (data != null && m_owner.PrimaryAxis == this && isSorted && isIndexed)
                {
                    for (int i = 0; i < data.Count - 1; i++)
                    {
                        for (int j = i; j < data.Count; j++)
                        {
                            if (sortDirection == Direction.Ascending)
                            {
                                if (data[i].Y > data[j].Y)
                                {
                                     if (visibleLabels.Keys.Contains(i) && visibleLabels.Keys.Contains(j))
                                     {
                                        var Temp = visibleLabels[i].Position;
                                        visibleLabels[i].Position = visibleLabels[j].Position;
                                        visibleLabels[j].Position = Temp;
                                     }

                                }
                            }
                            else
                            {
                                if (data[i].Y < data[j].Y)
                                {
                                    if (visibleLabels.Keys.Contains(i) && visibleLabels.Keys.Contains(j))
                                    {
                                        var Temp = visibleLabels[i].Position;
                                        visibleLabels[i].Position = visibleLabels[j].Position;
                                        visibleLabels[j].Position = Temp;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            double lblcount = 0;
            bool serisindexed=false;
            if(m_owner!=null)
            {
                foreach (var ser in m_owner.Series)
                {
                    if (ser.IsIndexed)
                    {
                        serisindexed = true;
                        break;
                    }

                }
            }
            if (visibleLabels.Count > 2 && ValueType!= ChartValueType.TimeSpan && visibleLabels.Values[visibleLabels.Values.Count - 1].Position.ToString() == visibleLabels.Values[visibleLabels.Values.Count - 2].Position.ToString())
                visibleLabels.RemoveAt(visibleLabels.Count - 1);
            foreach (var item in visibleLabels.Values)
            {
                if(m_owner.Series.Count!=0 && EnableSmartAxisLabel== true )
                {
                    if (ValueType == ChartValueType.DateTime && serisindexed == false)
                    {
                        
                        var dtlable = CalculateDateSmartAxisLabels(lblcount, item.ToString(), visibleLabels.Values.Count - 1,ValueType);
                        m_visibleLables.Add(new ChartAxisLabel(item.Position, dtlable));
                        lblcount++;
                    }
                    else if (ValueType == ChartValueType.TimeSpan && serisindexed == false && (LabelTimeSpanFormat=="hh:mm:ss" || LabelTimeSpanFormat=="dd:hh:mm:ss"))
                    {
                        var dtlable = CalculateTimeSmartAxisLabels(lblcount, item.ToString(), visibleLabels.Values.Count - 1,ValueType);
                        m_visibleLables.Add(new ChartAxisLabel(item.Position, dtlable));
                        lblcount++;
                    }
                    else if (ValueType == ChartValueType.Double)
                    {
                        var dtlabel = CalculateSmartAxisLabels(DoubleDisplayUnit, Convert.ToDouble(item.Content));
                        m_visibleLables.Add(new ChartAxisLabel(item.Position, dtlabel));
                    }
                    else
                        m_visibleLables.Add(item);

                }
                else if (ValueType == ChartValueType.Logarithmic)
                {
                    var dtlabel = CalculateSmartLogAxisLabels(Convert.ToDouble(item.Content));
                    m_visibleLables.Add(new ChartAxisLabel(item.Position, dtlabel));
                }
                else
                {
                    m_visibleLables.Add(item);
                }
            }


        }

        private object CalculateSmartLogAxisLabels(double doublevalue)
        {
            double content = doublevalue;
            Regex checkExpo= new Regex("E+");
            bool IsExpo=false;
            if (LabelLogarithmicFormat == LogarithmicType.Exponential)
            {
                IsExpo = checkExpo.IsMatch(doublevalue.ToString());
                if (doublevalue.ToString().Length > 1 && !IsExpo)
                {
                    double ab = doublevalue.ToString().Length;
                    if (doublevalue % Math.Pow(10, doublevalue.ToString().Length - 1) == 0)
                    {
                        return doublevalue.ToString("0E+00");
                       // return "1E +" + Convert.ToString(doublevalue.ToString().Length - 1);
                    }
                    else
                    {
                        return doublevalue.ToString("0E+00");
                        //return (doublevalue / Math.Pow(10, doublevalue.ToString().Length - 1)).ToString().Substring(0, 2) + "E +" + Convert.ToString(doublevalue.ToString().Length - 1);
                    }
                }
                else
                    return content;
            }
            else
            {
                return content.ToString(this.LabelFormat,CultureInfo.CurrentCulture);
            }
           
        }

        private object CalculateSmartAxisLabels(DoubleUnits unitvalue, double doublevalue)
        {
            double content = doublevalue;
            switch (unitvalue)
            {
                case DoubleUnits.AutoDetect:
                    if (doublevalue > 999 && doublevalue <= 99999)
                    {
                        this.DoubleDisplayUnit = DoubleUnits.Thousands;
                        return doublevalue / 1000 + " K";
                    }
                    else if (doublevalue > 99999 && doublevalue <= 99999999)
                    {
                        this.DoubleDisplayUnit = DoubleUnits.Millions;
                        return doublevalue / 1000000 + " M";
                    }
                    else if (doublevalue > 99999999 && doublevalue <= 99999999999)
                    {
                        this.DoubleDisplayUnit = DoubleUnits.Billions;
                        return doublevalue / 1000000000 + " B";
                    }
                    else if (doublevalue > 99999999999 && doublevalue <= 99999999999999)
                    {
                        this.DoubleDisplayUnit = DoubleUnits.Trillions;
                        return doublevalue / 1000000000000 + " T";
                    }
                    else if (doublevalue > 99999999999999 && doublevalue <= 9999999999999999)
                    {
                        this.DoubleDisplayUnit = DoubleUnits.Trillions;
                        return doublevalue / 1000000000000000 + " T";
                    }
                    else
                        return doublevalue;
                case DoubleUnits.None:
                    break;
                case DoubleUnits.Hundreds:
                    return doublevalue / 100;
                case DoubleUnits.Thousands:
                    return doublevalue / 1000 + " K";
                case DoubleUnits.TenThousands:
                    return doublevalue / 10000;
                case DoubleUnits.HundredThousands:
                    return doublevalue / 100000;
                case DoubleUnits.Millions:
                    return doublevalue / 1000000 + " M";
                case DoubleUnits.TenMillions:
                    return doublevalue / 10000000;
                case DoubleUnits.HundredMillions:
                    return doublevalue / 100000000;
                case DoubleUnits.Billions:
                    return doublevalue / 1000000000 + " B";
                case DoubleUnits.Trillions:
                    return doublevalue / 1000000000000 + " T";     
                case DoubleUnits.Quadrillion:
                    return doublevalue / 1000000000000000 + " Q";
            }
            return content;
        }

        DateTime olddatevalue, firstdatevalue;
        TimeSpan oldtimevalue, firsttimevalue;
        string tempdatevalue;
        private object CalculateDateSmartAxisLabels(double position,string datevalue,double totalLabels, ChartValueType valuetype)
        {
            DateTime newdatevalue = new DateTime();
            TimeSpan timediff, firstdiff;
            double timeinterval, firstinterval;
            Regex checkFrmt = new Regex("MMM/");
            Regex checkmmFrmt = new Regex("MM/");
            bool dtFrmt = false;
            if (LabelDateTimeFormat != null)
            {
                dtFrmt = checkFrmt.IsMatch(LabelDateTimeFormat);
            }
            if (position == 0||position==totalLabels)
            {
                DateTime.TryParse(datevalue, out newdatevalue);
                olddatevalue = newdatevalue;
                if (position == 0)
                    firstdatevalue = newdatevalue;
            }
            else
            {
                DateTime.TryParse(datevalue,CultureInfo.CurrentCulture,DateTimeStyles.AssumeLocal, out newdatevalue);
               // newdatevalue = Convert.ToDateTime(datevalue);
                timediff = newdatevalue.Subtract(olddatevalue);
                timeinterval = Math.Abs(timediff.TotalDays);
                olddatevalue = newdatevalue;
                if (timeinterval >= 1)
                {
                if (timeinterval == 365)
                {
                    //if (dtFrmt)
                    //    return newdatevalue.ToString(LabelDateTimeFormat, CultureInfo.CurrentCulture);
                    //else
                    return newdatevalue.ToString("yyyy", CultureInfo.CurrentCulture);
                }
                else if (Math.Round(timeinterval )/ 365 > 1)
                {
                    if (dtFrmt)
                        return newdatevalue.ToString("MMM/dd/yyyy", CultureInfo.CurrentCulture);
                    else
                    return newdatevalue.ToString("MM/dd/yyyy", CultureInfo.CurrentCulture);
                }
                else if (Math.Round(timeinterval / 30) >= 1 && Math.Round(timeinterval / 30) < 12)
                {
                    if (dtFrmt)
                    {
                        firstdiff = newdatevalue.Subtract(firstdatevalue);
                        firstinterval = Math.Abs(firstdiff.TotalDays);
                        if (firstdatevalue.Year == newdatevalue.Year)
                        {
                            firstdatevalue = newdatevalue;
                            return newdatevalue.ToString("MMM/dd", CultureInfo.CurrentCulture);
                        }
                        else
                        {
                            firstdatevalue = newdatevalue;
                            return newdatevalue.ToString("MMM/dd/yyyy", CultureInfo.CurrentCulture);
                        }
                    }
                    else
                    {
                        firstdiff = newdatevalue.Subtract(firstdatevalue);
                        firstinterval = Math.Abs(firstdiff.TotalDays);
                        if (firstdatevalue.Year == newdatevalue.Year)
                        {
                            firstdatevalue = newdatevalue;
                            return newdatevalue.ToString("MM/dd", CultureInfo.CurrentCulture);
                        }
                        else
                        {
                            firstdatevalue = newdatevalue;
                            return newdatevalue.ToString("MM/dd/yyyy", CultureInfo.CurrentCulture);
                        }
                    }
                }
                else if (timeinterval >= 1 && timeinterval < 31)
                {
                    if (dtFrmt)
                        return newdatevalue.ToString("MMM/dd", CultureInfo.CurrentCulture);
                    else
                    return newdatevalue.ToString("MM/dd", CultureInfo.CurrentCulture);
                }
                }
                else
                {
                    if (Math.Round(timeinterval * 24) >= 1 && Math.Round(timeinterval * 24) < 24)
                    {
                        firstdiff = newdatevalue.Subtract(firstdatevalue);
                        firstinterval = Math.Abs( firstdiff.TotalHours);
                        double firstdateHour = firstdatevalue.Hour >= 12 ? firstdatevalue.Hour - 12 : firstdatevalue.Hour;
                        if (firstdateHour + Math.Round(firstinterval) >= 12)
                        {
                            firstdatevalue = newdatevalue;
                            tempdatevalue = newdatevalue.ToString("hh:mm tt", CultureInfo.CurrentCulture);
                            return tempdatevalue;
                        }
                        else
                        {
                            tempdatevalue=newdatevalue.ToString("hh:mm", CultureInfo.CurrentCulture);
                            return tempdatevalue;
                        }
                    }
                    else if (Math.Round(timeinterval * 24 * 60) >= 1 && Math.Round(timeinterval * 24 * 60) < 60)
                    {
                        tempdatevalue=newdatevalue.ToString("hh:mm", CultureInfo.CurrentCulture);
                        return tempdatevalue;
                    }
                    else if (Math.Round(timeinterval * 24 * 60 * 60) >= 1 && Math.Round(timeinterval * 24 * 60 * 60) < 60)
                    {
                        tempdatevalue = newdatevalue.ToString("hh:mm:ss", CultureInfo.CurrentCulture);
                        return tempdatevalue;
                    }
                    else if (timeinterval == 0)
                    {
                        return tempdatevalue;
                    }
                    
                        
                    
                }

            }
            tempdatevalue = newdatevalue.ToString(LabelDateTimeFormat, CultureInfo.CurrentCulture);           
            return tempdatevalue;
        }


        private object CalculateTimeSmartAxisLabels(double position, string datevalue, double totalLabels, ChartValueType valuetype)
        {
            var newdatevalue = new TimeSpan();
            tempdatevalue = string.Empty;
            Regex checkFrmt = new Regex("/");
            Regex checkTsFrmt = new Regex(":");
            bool dtFrmt = false;
            if (LabelTimeSpanFormat != null)
                dtFrmt = checkTsFrmt.IsMatch(LabelTimeSpanFormat);

            if (position == 0 || position == totalLabels)
            {

                TimeSpan.TryParse(datevalue, out newdatevalue);
                oldtimevalue = newdatevalue;
                if (position == 0)
                    firsttimevalue = newdatevalue;
            }
            else
            {
                TimeSpan.TryParse(datevalue, out newdatevalue);
                // newdatevalue = Convert.ToDateTime(datevalue);
                var timediff = newdatevalue.Subtract(oldtimevalue);
                double timeinterval = Math.Abs(timediff.TotalDays);
                oldtimevalue = newdatevalue;
                if (timeinterval >= 1)
                {
                    return newdatevalue.Hours.ToString(CultureInfo.InvariantCulture) + ":" + newdatevalue.Minutes.ToString(CultureInfo.InvariantCulture);
                    
                }
                else
                {
                    if (Math.Round(timeinterval * 24) >= 1 && Math.Round(timeinterval * 24) < 24)
                    {
                        if (firsttimevalue.Days != newdatevalue.Days)
                        {
                            tempdatevalue = newdatevalue.Days.ToString()+":";
                            firsttimevalue = newdatevalue;
                        }
                        

                            tempdatevalue =tempdatevalue+ newdatevalue.Hours.ToString() + ":" + newdatevalue.Minutes.ToString() + ":" + newdatevalue.Seconds.ToString();
                            return tempdatevalue;
                       
                        
                    }
                    else if (Math.Round(timeinterval * 24 * 60) >= 1 && Math.Round(timeinterval * 24 * 60) < 60)
                    {
                        tempdatevalue = newdatevalue.Minutes.ToString() + ":" + newdatevalue.Seconds.ToString();
                        return tempdatevalue;
                    }
                    else if (Math.Round(timeinterval * 24 * 60 * 60) >= 1 && Math.Round(timeinterval * 24 * 60 * 60) < 60)
                    {
                        tempdatevalue = newdatevalue.Seconds.ToString();
                        return tempdatevalue;
                    }
                    else if (timeinterval == 0)
                    {
                        return tempdatevalue;
                    }



                }

            }
            tempdatevalue = FormatTimeSpan(newdatevalue);
            return tempdatevalue;
        }

        /// <summary>
        /// Gets the content of the label.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <returns>The LAbel content</returns>
        private object GetLabelContent(double pos)
        {
            object content = null;
            ChartValueType valueType = ValueType;

            switch (valueType)
            {
                case ChartValueType.Double:
                    if (IsAutoSetRange == true && (pos < 0 || pos < 1))
                    {
                        content = pos.ToString(this.LabelFormat, CultureInfo.CurrentCulture);
                    }
                    else
                    {
                        content = Math.Round(pos, C_roundDecimals).ToString(this.LabelFormat, CultureInfo.CurrentCulture);
                    }
                    break;

                case ChartValueType.DateTime:
                    try
                    {
                        if(EnableSmartAxisLabel==true)
                            content = DateTime.FromOADate(pos).ToString("g", CultureInfo.CurrentCulture);
                        else
                        content = DateTime.FromOADate(pos).ToString(this.LabelDateTimeFormat, CultureInfo.CurrentCulture);
                        DateTime dt = new DateTime();
                        dt.ToString("d");
                    }
                    catch
                    {
                        if (!double.IsInfinity(pos))
                        {
                            TimeSpan ts1 = new TimeSpan();
                            ts1 = TimeSpan.FromMilliseconds(pos);
                            String str1 = ts1.ToString();
                            if (LabelTimeSpanFormat != null)
                            {
                                str1 = FormatTimeSpan(ts1);
                            }
                            content = str1;
                        }
                    }
                    break;

                case ChartValueType.TimeSpan:
                    TimeSpan ts = new TimeSpan();
                    ts = TimeSpan.FromMilliseconds(pos);
                    String str = ts.ToString();
                    if (LabelTimeSpanFormat != null)
                    {
                        str = FormatTimeSpan(ts);
                    }
                    content = str;
                    break;
                case ChartValueType.Logarithmic:
                    if(pos<0)
                        content = Math.Pow(this.LogarithmicBase, pos).ToString(this.LabelFormat, CultureInfo.CurrentCulture);
                    else
                        content =Math.Round(Math.Pow(this.LogarithmicBase, pos), C_roundDecimals).ToString(this.LabelFormat, CultureInfo.CurrentCulture);
                    break;
                case ChartValueType.String:
                    if (IsAutoSetRange == true && (pos < 0 || pos < 1))
                    {
                        content = pos.ToString(this.LabelFormat, CultureInfo.CurrentCulture);
                    }
                    else
                    {
                        content = Math.Round(pos, C_roundDecimals).ToString(this.LabelFormat, CultureInfo.CurrentCulture);
                    }
                    break;
                   
            }

            return content;
        }

        private string FormatTimeSpan(TimeSpan span)
        {
            string sign = String.Empty;
            String labelformat = LabelTimeSpanFormat.ToLower();
            switch (labelformat)
            {
                case "hh:mm:ss":
                    sign = sign + span.Hours.ToString() + ":" + span.Minutes.ToString() + ":" + span.Seconds.ToString();
                    break;
                case "hh:mm":
                    sign = sign + span.Hours.ToString() + ":" + span.Minutes.ToString();
                    break;
                    case "mm:ss":
                    sign = sign + Math.Truncate(span.TotalMinutes).ToString() + ":" + (span.Seconds).ToString();
                    break;
                case "dd:hh:mm:ss":
                    sign = sign + span.Days.ToString() + ":" + span.Hours.ToString() + ":"
                        + span.Minutes.ToString() + ":" + span.Seconds.ToString();
                    break;
                case "dd":
                    sign = sign + span.Days.ToString();
                    break;
                case "mm":
                    sign = sign + Math.Truncate(span.TotalMinutes).ToString();
                    break;
                case "hh":
                    sign = sign + (Math.Truncate(span.TotalHours)).ToString();
                    break;
                case "ss":
                    sign = sign + (Math.Truncate(span.TotalSeconds)).ToString();
                     break;

            }
            if (sign == string.Empty)
                sign = span.ToString();
            return sign;
            //return sign + span.Days.ToString() + "." +
            //    span.Hours.ToString() + ":" +
            //    span.Minutes.ToString() + ":" +
            //    span.Seconds.ToString() + "." +
            //    span.Milliseconds.ToString();

        }

        /// <summary>
        /// Recalculates visible range and visible labels.
        /// </summary>
        internal void Invalidate()
        {
            m_isUpdating = true;

            this.CalculateVisibleRange();

            if (this.EnableBreaks)
            {
                if (this.BreakRange != null && this.BreakRange.m_breaksMode == ChartBreaksModes.Auto && this.Area != null && this.Area.Series != null)
                {
                    this.BreakRange.Compute(this.Area.Series);
                }
            }

            this.CalculateVisibleLables();

            m_isUpdating = false;

            this.RaiseChanged(this, EventArgs.Empty);
        }

        #region Nice range calculation

        /// <summary>
        /// Gets the nice interval.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="desiredIntervalCount">The desired interval count.</param>
        /// <param name="isLogarithmicLabels">if set to <c>true</c> [is logarithmic labels].</param>
        /// <param name="isIndexed">if set to <c>true</c> [is indexed].</param>
        /// <returns>The NiceInterval</returns>
        private double GetNiceInterval(DoubleRange range, int desiredIntervalCount, bool isLogarithmicLabels, bool isIndexed)
        {
            ////if (isLogarithmicLabels)
            ////{
            ////    if (isIndexed)
            ////    {
            ////        return CalculateNiceIndexedIntervalForLogAxis(range, (int)DesiredIntervalsCountProperty.DefaultMetadata.DefaultValue);
            ////    }
            ////    else
            ////    {
            ////        return CalculateNiceIntervalForLogAxis(range, desiredIntervalCount);
            ////    }
            ////}
            ////else
            ////{
            if (isIndexed)
            {
                return CalculateNiceIndexedInterval(range, (int)DesiredIntervalsCountProperty.DefaultMetadata.DefaultValue);
            }
            else
            {
                return CalculateNiceInterval(range, desiredIntervalCount);
            }
            ////}
        }

        /// <summary>
        /// Gets the nice interval offset.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="interval">The interval.</param>
        /// <param name="isLogarithmicLabels">if set to <c>true</c> [is logarithmic labels].</param>
        /// <param name="isIndexed">if set to <c>true</c> [is indexed].</param>
        /// <returns>The NiceIntervalOffset</returns>
        private double GetNiceIntervalOffset(DoubleRange range, double interval, bool isLogarithmicLabels, bool isIndexed)
        {
            ////if (isLogarithmicLabels)
            ////{
            ////    if (isIndexed)
            ////    {
            ////        return CalculateNiceIndexedIntervalOffsetForLogAxis(range, interval);
            ////    }
            ////    else
            ////    {
            ////        return CalculateNiceIntervalOffsetForLogAxis(range, interval);
            ////    }
            ////}
            ////else
            ////{
            if (isIndexed)
            {
                return CalculateNiceIndexedIntervalOffset(range, interval);
            }
            else
            {
                return CalculateNiceIntervalOffset(range, interval);
            }
            ////}
        }

        /// <summary>
        /// Calculates the logarithmic padding.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="rangePadding">The range padding.</param>
        /// <param name="tickInterval">The tick interval.</param>
        /// <param name="tickOffset">The tick offset.</param>
        /// <returns>The Logarithmic Padding</returns>
        private DoubleRange CalculateLogarithmicPadding(DoubleRange range, ChartRangePaddingType rangePadding, ref double tickInterval, ref double tickOffset)
        {
            if (rangePadding != ChartRangePaddingType.None)
            {
                double logStart = Math.Log(range.Start, LogarithmicBase);
                double logEnd = Math.Log(range.End, LogarithmicBase);

                if (double.IsInfinity(logStart) || double.IsNaN(logStart)) 
                {
                    double mulE1 = ChartMath.Round(logEnd, 1, true);
                    return new DoubleRange(0, mulE1);
                }
                else if (double.IsInfinity(logEnd) || double.IsNaN(logEnd))
                {
                    return new DoubleRange(0, 1);
                }

                double mulS = ChartMath.Round(logStart, 1, false);
                double x = Math.Floor(Math.Log(-logStart, LogarithmicBase));
                double mulE = ChartMath.Round(logEnd, 1, true);
                ////range = new DoubleRange(Math.Pow(LogarithmicBase, mulS), Math.Pow(LogarithmicBase, mulE));
                range = new DoubleRange(mulS, mulE);
            }

            return range;
        }

        /// <summary>
        /// Calculates the padding.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="rangePadding">The range padding.</param>
        /// <param name="tickInterval">The tick interval.</param>
        /// <param name="tickOffset">The tick offset.</param>
        /// <returns>the padding</returns>
        //private DoubleRange CalculatePadding(DoubleRange range, ChartRangePaddingType rangePadding, ref double tickInterval, ref double tickOffset)
        //{
        //    double start, end;
        //    if(m_owner.PrimarySeries !=null)
        //        range = CalculateRelativePadding(m_owner.PrimarySeries.Type, range, rangePadding, tickInterval, ref tickOffset, range.Start, range.End);
        //    tickOffset = 0;            
        //    if (!IsFractionEnabledOnZoom && rangePadding == ChartRangePaddingType.None)
        //    {
        //        start = ChartMath.Round(range.Start, tickInterval, true);
        //        end = ChartMath.Round(range.End, tickInterval, false);
        //        tickOffset = 0;
        //        range = new DoubleRange(start, end);
        //    }
        //    return range;
        //}

        private DoubleRange CalculatePadding(DoubleRange range, ChartRangePaddingType rangePadding, ref double tickInterval, ref double tickOffset)
        {
            double start, end;
            #region IndexedAxis
            if (Indexed)
            {
                if (this.RangeCalculationMode == RangeCalculationMode.AdjustAcrossChartTypes)
                {
                    if (this.Area.Series[0].Type == ChartTypes.Area || this.Area.Series[0].Type == ChartTypes.Line ||
                                   this.Area.Series[0].Type == ChartTypes.StackingColumn || this.Area.Series[0].Type == ChartTypes.FastStackingColumn ||
                                    this.Area.Series[0].Type == ChartTypes.FastLine ||
                                    this.Area.Series[0].Type == ChartTypes.FastScatter ||
                                    this.Area.Series[0].Type == ChartTypes.HiLoArea ||
                                    this.Area.Series[0].Type == ChartTypes.SplineArea ||
                                    this.Area.Series[0].Type == ChartTypes.StackingArea ||
                                    this.Area.Series[0].Type == ChartTypes.StackingLine ||
                                    this.Area.Series[0].Type == ChartTypes.StackingSpline ||
                                    this.Area.Series[0].Type == ChartTypes.StackingSplineArea ||
                                    this.Area.Series[0].Type == ChartTypes.StepArea ||
                                    this.Area.Series[0].Type == ChartTypes.Scatter ||
                                    this.Area.Series[0].Type == ChartTypes.Bubble ||
                                    this.Area.Series[0].Type == ChartTypes.Tornado ||
                                    this.Area.Series[0].Type == ChartTypes.RangeArea || this.Area.Series[0].Type == ChartTypes.StackingArea100)
                    {
                        start = range.Start - 0.49;
                        end = range.End + 0.49;
                    }
                    else
                    {
                        start = range.Start - 0.1;
                        end = range.End + 0.1;
                    }
                    range = new DoubleRange(start, end);
                }
                else
                {
                    switch (rangePadding)
                       {
                            case ChartRangePaddingType.Additional:
                                {
                                    start = ChartMath.Round(range.Start, tickInterval, false);
                                    end = ChartMath.Round(range.End, tickInterval, true);
                                    start -= tickInterval * (this.AdditionalPadding.Start);
                                    end += tickInterval * (this.AdditionalPadding.End);
                                    range = new DoubleRange(start, end);
                                }
                                break;
                            case ChartRangePaddingType.None:
                                {
                                    start = range.Start;
                                    end = range.End;
                                    range = new DoubleRange(start, end);
                                }
                                break;
                            case ChartRangePaddingType.Normal:
                              if (this.Area.AreaType==ChartAxesType.CartesianAxes)
                              {
                                start = ChartMath.Round(range.Start, tickInterval, false);
                                end = ChartMath.Round(range.End, tickInterval, true);
                                start -= tickInterval;
                                end += tickInterval;
                                range = new DoubleRange(start, end);
                              
                             }
                             else
                             {
                                start = range.Start;
                                end = range.End;
                                range = new DoubleRange(start, end);
                             }
                             break;
                        }
                }
                tickOffset = 0;
            }
            #endregion
            else
            #region NonIndexedAxis
            {
                
                switch (rangePadding)
                {
                        
                    case ChartRangePaddingType.Additional:
                        {
                            start = ChartMath.Round(range.Start, tickInterval, false);
                            end = ChartMath.Round(range.End, tickInterval, true);
                            start -= tickInterval*(this.AdditionalPadding.Start);
                            end += tickInterval * (this.AdditionalPadding.End);
                            tickOffset = 0;
                        }
                        range = new DoubleRange(start, end);
                        break;
                    //case ChartRangePaddingType.None:
                    //    {
                    //        start = range.Start;
                    //        range = new DoubleRange(start, end);
                    //    }
                    //    break;
                    case ChartRangePaddingType.Normal:
                        start = ChartMath.Round(range.Start, tickInterval, false);
                        end = ChartMath.Round(range.End, tickInterval, true);
                        range = new DoubleRange(start, end);
                        tickOffset = 0;
                        break;
                }
                
            }
            #endregion
            return range;
        }

        private DoubleRange CalculateRelativePadding(ChartTypes chartType, DoubleRange range, ChartRangePaddingType rangePadding, double tickInterval, ref double tickOffset, double start, double end)
        {
            if (this.RangeCalculationMode != Windows.Chart.RangeCalculationMode.ConsistentAcrossChartTypes)
            {
                switch (chartType)
                {
                    case ChartTypes.Bubble:
                    case ChartTypes.Scatter:
                    case ChartTypes.FastScatter:
                        range += range.Start - tickInterval;
                        range += range.End + tickInterval;
                        break;
                    default:
                        break;
                }
            }
            if(this.RangeCalculationMode != Windows.Chart.RangeCalculationMode.AdjustAcrossChartTypes)
                return SetRange(chartType,range, rangePadding, tickInterval, tickOffset, start, end);
            else
             return new DoubleRange(Math.Truncate(range.Start),Math.Truncate(range.End));
        }

        private DoubleRange SetRange(ChartTypes chartType,DoubleRange range, ChartRangePaddingType rangePadding, double tickInterval,double tickOffset, double start, double end)
        {
            if (rangePadding != ChartRangePaddingType.None && this.RangeCalculationMode != Windows.Chart.RangeCalculationMode.AdjustAcrossChartTypes)
            {
                if (Indexed)
                {
                    start = ChartMath.Round(range.Start, tickInterval, false);
                    end = ChartMath.Round(range.End, tickInterval, true);
                    if (rangePadding == ChartRangePaddingType.Normal)
                    {
                        start -= tickInterval / 2;
                        end += tickInterval / 2;
                    }
                    if (rangePadding == ChartRangePaddingType.Additional)
                    {
                        start -= tickInterval;
                        end += tickInterval;
                    }
                    tickOffset = 0;
                    range = new DoubleRange(start, end);
                }
                else
                {
                    if (this.RangeCalculationMode == Windows.Chart.RangeCalculationMode.ConsistentAcrossChartTypes)
                    {
                        switch (chartType)
                        {
                            case ChartTypes.Bubble:
                                break;
                            case ChartTypes.Column:
                            case ChartTypes.Bar:
                            case ChartTypes.Gantt:
                            case ChartTypes.Histogram:
                            case ChartTypes.RangeColumn:
                            case ChartTypes.StackingBar:
                            case ChartTypes.StackingBar100:
                            case ChartTypes.StackingColumn:
                            case ChartTypes.StackingColumn100:
                                range =new DoubleRange(ChartType.GetColumnInitialSegmentWidth(m_owner.PrimarySeries),ChartType.GetEndSegmentWidth(m_owner.PrimarySeries));
                                break;
                            default:
                                break;
                        }                    
                    }
                    start = ChartMath.Round(range.Start, tickInterval, false);
                    end = ChartMath.Round(range.End, tickInterval, true);
                    if (this.RangePadding == ChartRangePaddingType.Additional)
                    {
                        start -= tickInterval;
                        end += tickInterval;
                    }
                    tickOffset = 0;
                    range = new DoubleRange(start, end);
                }
                return range;
            }            
            return range;
        }

        /// <summary>
        /// Calculates the nice interval.
        /// </summary>
        /// <param name="dr">The DoubleRange dr.</param>
        /// <param name="desiredIntervalCount">The desired interval count.</param>
        /// <param name="interval">The interval.</param>
        /// <param name="intervalStart">The interval start.</param>
        [Obsolete]
        private static void CalculateNiceIntervalOld(DoubleRange dr, int desiredIntervalCount, out double interval, out double intervalStart)
        {
            double desiredInterval = dr.Delta / desiredIntervalCount;
            double exponent10 = Math.Ceiling(Math.Log10(dr.Delta));
            double mantissa = dr.Delta / Math.Pow(10.0, exponent10); ////<= 1.0
            double mantissaInterval = mantissa / desiredIntervalCount;

            double domnoz = Math.Pow(10.0, 1);
            double desiredNiceInteger = (1.0 / mantissaInterval) * domnoz;

            double ln2 = Math.Log10(2);
            double ln5 = Math.Log10(5);
            double lnDesiredNiceInteger = Math.Log10(desiredNiceInteger);

            int max2 = (int)Math.Ceiling(Math.Log(desiredNiceInteger, 2));
            int max5 = (int)Math.Ceiling(Math.Log(desiredNiceInteger, 5));

            double diff = double.MaxValue;
            int opt_n = 0;
            int opt_m = max5;

            for (int n = 0; n <= max2; n++)
            {
                int m_start = (int)Math.Floor((lnDesiredNiceInteger - n * ln2) / ln5);
                for (int m = m_start; (m <= m_start + 1) && (m <= max5); m++)
                {
                    double t_diff = Math.Abs(lnDesiredNiceInteger - n * ln2 - m * ln5);
                    if ((t_diff < diff) && ((max5 < 4) || ((m >= n / 3) && (m != 0))))
                    {
                        diff = t_diff;
                        opt_n = n;
                        opt_m = m;
                    }
                }
            }

            double niceInteger = Math.Pow(2, opt_n) * Math.Pow(5, opt_m) / domnoz;
            interval = (1.0 / niceInteger) * Math.Pow(10.0, exponent10);

            double intExp = Math.Floor(Math.Log10(interval));
            double t_intervalStart = Math.Floor(dr.Start / Math.Pow(10.0, intExp)) * Math.Pow(10.0, intExp);
            intervalStart = t_intervalStart + Math.Floor((dr.Start - t_intervalStart) / interval) * interval;
        }

        /// <summary>
        /// Calculates the nice indexed interval.
        /// </summary>
        /// <param name="dr">The DoubleRange dr.</param>
        /// <param name="desiredIntervalCount">The desired interval count.</param>
        /// <returns>The NiceIndexedInterval</returns>
        private static double CalculateNiceIndexedInterval(DoubleRange dr, int desiredIntervalCount)
        {
            return Math.Max(1d, Math.Truncate(dr.Delta / desiredIntervalCount));
        }

        /// <summary>
        /// Calculates the nice indexed interval.
        /// </summary>
        /// <param name="dr">The DoubleRange dr.</param>
        /// <param name="interval">The interval.</param>
        /// <returns>the NiceIndexedIntervalOffset</returns>
        private static double CalculateNiceIndexedIntervalOffset(DoubleRange dr, double interval)
        {
            return -(dr.Start % interval);
        }

        /// <summary>
        /// Calculates the nice indexed interval for log axis.
        /// </summary>
        /// <param name="dr">The DoubleRange dr.</param>
        /// <param name="desiredIntervalCount">The desired interval count.</param>
        /// <returns>The NiceIndexedIntervalForLogAxis</returns>
        private static double CalculateNiceIndexedIntervalForLogAxis(DoubleRange dr, int desiredIntervalCount)
        {
            double interMult = Math.Pow((dr.End / dr.Start), 1 / ((double)desiredIntervalCount)); ////interMult is always > 1
            double intervalMult = Math.Round(interMult);

            if (intervalMult <= 1.0)
            {
                intervalMult = 2.0;
            }

            return interMult;
        }

        /// <summary>
        /// Calculates the nice indexed interval for log axis.
        /// </summary>
        /// <param name="dr">The DoubleRange dr.</param>
        /// <param name="intervalMult">The interval mult.</param>
        /// <returns>The NiceIndexedIntervalOffsetForLogAxis</returns>
        private static double CalculateNiceIndexedIntervalOffsetForLogAxis(DoubleRange dr, double intervalMult)
        {
            double intervalStart = Math.Floor(dr.Start);

            if (intervalStart <= 0)
            {
                double pow = Math.Floor(Math.Log(dr.Start, intervalMult));
                intervalStart = Math.Pow(intervalMult, pow);
            }

            return intervalStart;
        }

        //double offset = 0.001;

        /// <summary>
        /// Calculates the nice interval.
        /// </summary>
        /// <param name="dr">The DoubleRange dr.</param>
        /// <param name="desiredIntervalCount">The desired interval count.</param>
        /// <returns>The NiceInterval</returns>
        private double CalculateNiceInterval(DoubleRange dr, int desiredIntervalCount)
        {
            double desiredInterval = dr.Delta / desiredIntervalCount;
            double mul = Math.Pow(LogarithmicBase, Math.Floor(Math.Log(desiredInterval, LogarithmicBase)));
            double minDelta = double.MaxValue;
            double minValue = double.MaxValue;
            double loginterval = double.IsNaN(LogarithmicInterval) ? desiredInterval : LogarithmicInterval;

            double interval = 0d;

            //if (mul < offset)
            //{
            //    mul = 1;
            //}

            var isOlapAxis = (this.GetType()).FullName.Equals("Syncfusion.Windows.Chart.Olap.OlapChartAxis");
            
            if (this.IsAutoSetRange || isOlapAxis)
            {
                foreach (double div in c_intervalDivs)
                {
                    double delta = Math.Abs(desiredInterval - div * mul);

                    if (delta < minDelta)
                    {
                        minDelta = delta;
                        interval = div * mul;
                    }
                    minValue = Math.Min(minValue, div);
                }
            }
            else
                interval = desiredInterval;
            if (this.ValueType == ChartValueType.Logarithmic)
            {
                if (LogarithmicIntervalType == LogarithmicType.Value)
                {
                    if(LogarithmicInterval>0)
                    interval = Math.Log(Math.Log(LogarithmicInterval,LogarithmicBase),LogarithmicBase);
                    else
                        interval = minValue * LogarithmicBase * mul;
                }
                else
                {
                    interval =Math.Round( Math.Log(Math.Pow(LogarithmicBase, LogarithmicInterval), LogarithmicBase),14);
                }
            }
            else if (Math.Abs(desiredInterval - minValue * LogarithmicBase * mul) < Math.Abs(desiredInterval - interval))
            {
                interval = minValue * LogarithmicBase * mul;
            }

            //if (this.Area.IsSync == true && this.ValueType != ChartValueType.DateTime)
            //{
            //    interval = ((dr.Start + dr.End) / (double)desiredIntervalCount);
            //    return interval;
            //}

            return interval;
        }

        /// <summary>
        /// Calculates the nice interval offset.
        /// </summary>
        /// <param name="dr">The DoubleRange dr.</param>
        /// <param name="interval">The interval.</param>
        /// <returns>The NiceIntervalOffset</returns>
        private static double CalculateNiceIntervalOffset(DoubleRange dr, double interval)
        {
            return -(dr.Start % interval);
        }

        /// <summary>
        /// Calculates the nice interval for log axis.
        /// </summary>
        /// <param name="dr">The DoubleRange dr.</param>
        /// <param name="desiredIntervalCount">The desired interval count.</param>
        /// <returns>The NiceIntervalForLogAxis</returns>
        private double CalculateNiceIntervalForLogAxis(DoubleRange dr, int desiredIntervalCount)
        {
            double interMult = Math.Pow((dr.End / dr.Start), 1 / ((double)desiredIntervalCount)); ////interMult is always > 1
            double interMultMinus1 = interMult - 1;
            double z = 3; ////z - accuracy of nice interval acquisition. Higher, the better, but numbers will be less nice looking.
            double e10 = Math.Floor(Math.Log10(interMultMinus1));
            double x = z - e10; // exponent of 10, of mulplicative factor, that will  multiply interMult
            double domnoz = Math.Pow(LogarithmicBase, x);

            double desiredNiceInteger = interMultMinus1 * domnoz;

            double ln2 = Math.Log10(2);
            double ln5 = Math.Log10(5);
            double lnDesiredNiceInteger = Math.Log(desiredNiceInteger, LogarithmicBase);

            int max2 = (int)Math.Ceiling(Math.Log(desiredNiceInteger, 2));
            int max5 = (int)Math.Ceiling(Math.Log(desiredNiceInteger, 5));

            double diff = double.MaxValue;
            int opt_n = 0;
            int opt_m = max5;

            for (int n = 0; n <= max2; n++)
            {
                int m_start = (int)Math.Floor((lnDesiredNiceInteger - n * ln2) / ln5);
                for (int m = m_start; (m <= m_start + 1) && (m <= max5); m++)
                {
                    double t_diff = Math.Abs(lnDesiredNiceInteger - n * ln2 - m * ln5);
                    if ((t_diff < diff) && ((max5 < 4) || ((m >= n / 3) && (m != 0))))
                    {
                        diff = t_diff;
                        opt_n = n;
                        opt_m = m;
                    }
                }
            }

            double niceInteger = Math.Pow(2, opt_n) * Math.Pow(5, opt_m);
            double intervalMultiplier = 1 + niceInteger / domnoz;

            return intervalMultiplier;
        }

        /// <summary>
        /// Calculates the nice interval for log axis.
        /// </summary>
        /// <param name="dr">The DoubleRange dr.</param>
        /// <param name="intervalMultiplier">The interval multiplier.</param>
        /// <returns>The NiceIntervalOffsetForLogAxis</returns>
        private double CalculateNiceIntervalOffsetForLogAxis(DoubleRange dr, double intervalMultiplier)
        {
            double intExp = Math.Floor(Math.Log(intervalMultiplier, LogarithmicBase));
            double t_intervalStart = Math.Floor(dr.Start / Math.Pow(LogarithmicBase, intExp)) * Math.Pow(LogarithmicBase, intExp);
            return t_intervalStart; //// + Math.Floor((dr.Start - t_intervalStart) / interval) * interval;
        }
        #endregion

        /// <summary>
        /// Raises the <see cref="Changed"/> event.
        /// </summary>
        /// <param name="sender">Sender of event.</param>
        /// <param name="args">The EventAgrument args.</param>
        private void RaiseChanged(object sender, EventArgs args)
        {
            if (Changed != null)
            {
                Changed(sender, args);
            }
        }

        /// <summary>
        /// Invoked whenever the effective value of any dependency property on this <see cref="T:System.Windows.DependencyObject"></see> has been updated. The specific dependency property that changed is reported in the event data.
        /// </summary>
        /// <param name="e">Event data that will contain the dependency property identifier of interest, the property metadata for the type, and old and new values.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (!m_isUpdating)
            {
                PropertyMetadata propertyMetadata = e.Property.GetMetadata(this.GetType()) as PropertyMetadata;
                if (propertyMetadata != null)
                {
                    bool affectUpdate = CheckForUpdate(e.Property);

                    if (affectUpdate)
                    {
                        //this.Invalidate(); Commented for SD15713 - Hanging issue
                        if (!DesignerProperties.GetIsInDesignMode(this))//Commented for SD15713 - Hanging issue
                        {
                            this.Invalidate();
                        }
                        if (e.Property == ZoomPositionProperty)
                        {
                            if (this.Area != null)
                            {
                                if (this.Area.ChartAreaParent != null && this.Area.ZoomSwitched == true)
                                {
                                    // (this.Area.ChartAreaParent as SyncChartAreas).SetAreaProperties();
								}
                                for (int i = 0; i < this.Area.Series.Count; i++)
                                {
                                    if (this.Area.Series[i].ChartType != null)
                                        this.Area.Series[i].ChartType.Update(this.Area.Series[i]);
                                }
                            }
                        }
                      
                    }
                    if (CheckForReDraw(e.Property))
                    {
                        if (this.Area != null)
                        {
                            this.Area.Redraw();
                        }
                    }

                    RaiseChanged(this, new AxisChangedEventArgs(e));
                }
            }

            //For SyncChartAreas
            if (e.Property == ZoomFactorProperty )
            {
                if (this.Area != null)
                {
                    if (this.Area.ZoomSwitched == true)
                    {
                        if (this.Area.ChartAreaParent != null)
                        {
                            // (this.Area.ChartAreaParent as SyncChartAreas).SetAreaProperties();
                        }
                    }
                }
            }
          
            

            base.OnPropertyChanged(e);
        }

        private bool CheckForReDraw(DependencyProperty property)
        {
            bool redraw = (property == OrientationProperty || property == OpposedPositionProperty) ? true : false;
            return redraw;
        }

        /// <summary>
        /// Called when break range is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        internal void OnBreakRangeChanged(object sender, EventArgs e)
        {
            this.Invalidate();
            this.RaiseChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Called when strip lines collection is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnStripLinesChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.OldItems != null)
            {
                foreach (ChartStripLine stripLine in args.OldItems)
                {
                    stripLine.Axis = null;
                }
            }

            if (args.NewItems != null)
            {
                foreach (ChartStripLine stripLine in args.NewItems)
                {
                    if (stripLine.Axis != null)
                    {
                        Console.Write(string.Format(CultureInfo.CurrentCulture, "{0} already has owner.", stripLine));
                    }
                    else
                    {
                        stripLine.Axis = this;
                        if (stripLine.DataContext == null)
                        {
                            BindingUtils.SetBinding(stripLine, this, FrameworkElement.DataContextProperty, FrameworkElement.DataContextProperty, BindingMode.OneWay);
                        }
                    }
                }
            }

            RaiseChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Called when custom lables collection is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnCustomLablesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if ((this.LabelsMode & ChartAxisLabelsMode.Custom) == ChartAxisLabelsMode.Custom)
            {
                if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    this.Invalidate();
                }
                else
                {
                    if (e.NewItems != null)
                    {
                        bool inversed = this.IsInversed;

                        foreach (ChartAxisLabel label in e.NewItems)
                        {
                            label.m_isCustomLabel = true;
                            if (this.VisibleRange.Inside(label.Position))
                            {
                                bool isAdded = false;

                                for (int i = 0; i < m_visibleLables.Count; i++)
                                {
                                    if ((!inversed && label.Position < m_visibleLables[i].Position)
                                      || (inversed && label.Position > m_visibleLables[i].Position))
                                    {
                                        m_visibleLables.Insert(i, label);
                                        isAdded = true;
                                        break;
                                    }
                                    if ((!inversed && label.Position == m_visibleLables[i].Position)
                                      || (inversed && label.Position == m_visibleLables[i].Position))
                                    {
                                        m_visibleLables[i].Content = label.Content;
                                        isAdded = true;
                                        break;
                                    }
                                }

                                if (!isAdded)
                                {
                                    m_visibleLables.Add(label);
                                }
                            }
                        }
                    }

                    if (e.OldItems != null)
                    {
                        foreach (ChartAxisLabel label in e.OldItems)
                        {
                            m_visibleLables.Remove(label);
                        }
                    }

                    this.RaiseChanged(this, EventArgs.Empty);
					this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Coerces the zoom factor.
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="baseValue">The base value.</param>
        /// <returns>the ZoomFactor</returns>
        private static object OnCoerceZoomFactor(DependencyObject d, object baseValue)
        {
            //Commented for fixing issue in SD13924
           // return ChartMath.MinMax((double)baseValue, C_minimalZooming, 1);
            ChartAxis axis = d as ChartAxis;
            if (axis != null)
                return ChartMath.MinMax((double)baseValue, axis.MinimalZoomFactor, 1);
            else
                return ChartMath.MinMax((double)baseValue, 0, 1);
        }

         /// <summary>
        /// Coerces the minimal zoom factor.
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="baseValue">The base value.</param>
        /// <returns>the MinimalZoomFactor</returns>
        private static object OnCoerceMinimalZoomFactor(DependencyObject d, object baseValue)
        {
            return ChartMath.MinMax((double)baseValue, 0, 1);
        }

        private bool _ZoomFactorisset = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="baseValue"></param>
        /// <returns></returns>
        private static object OnCoerceDateTimeRange(DependencyObject d, object baseValue)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null && axis.Area != null && (object)axis.DateTimeRange == baseValue)
            {
                axis.Area.UpdateArea();
            }
            return baseValue;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="baseValue"></param>
        /// <returns></returns>
        private static object OnCoerceRange(DependencyObject d, object baseValue)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null && axis.Area != null && (object)axis.InternalRange == baseValue)
            {
                axis.Area.UpdateArea();
            }
            return baseValue;

        }

        /// <summary>d
        /// Coerces the actual range.
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="baseValue">The base value.</param>
        /// <returns>The ActualRange</returns>
        private static object OnCoerceActualRange(DependencyObject d, object baseValue)
        {
            var axis = d as ChartAxis;
            if (axis != null)
            {
                DoubleRange baseRange = axis.InternalRange;
                if (baseRange.IsEmpty)
                {
                    baseRange = (!axis.ForceZero) ? axis.NiceRange : new DoubleRange(0, axis.NiceRange.End);
                }
                return baseRange;
            }
            return baseValue;
        }

        /// <summary>
        /// Coerces the zoom position.
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="baseValue">The base value.</param>
        /// <returns>The ZoomPosition</returns>
        private static object OnCoerceZoomPosition(DependencyObject d, object baseValue)
        {
            if ((d as ChartAxis)._ZoomFactorisset)
                return ChartMath.MinMax((double)baseValue, 0, 1 - (d as ChartAxis).ZoomFactor);
            else
                return ChartMath.MinMax((double)baseValue, 0, 1 );
        }

        /// <summary>
        /// Called when [coerce interval].
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="baseValue">The base value.</param>
        /// <returns>The Interval</returns>
        private static object OnCoerceInterval(DependencyObject d, object baseValue)
        {
            ChartAxis axis = d as ChartAxis;
            if ((double)baseValue < axis.MinimumInterval)
            {
                return axis.MinimumInterval;
            }
            else
            {
                return baseValue;
            }
        }
        private static object OnCoerceAdditionalPadding(DependencyObject d, object baseValue)
      {
            DoubleRange doubleRange = (DoubleRange)baseValue;
            if(doubleRange.IsAdditional==true)
            return new DoubleRange(doubleRange.End, doubleRange.Start, false);
            else
                return new DoubleRange(doubleRange.Start, doubleRange.End, false);

        }

        private static void Onchange(DependencyObject dpObj, DependencyPropertyChangedEventArgs args)
        {
            ChartAxis axis = dpObj as ChartAxis;
            if (axis != null && axis.Area != null)
            {
                axis.Area.UpdateArea();
            }
        }
        private static void OnTickSizeChange(DependencyObject dpObj, DependencyPropertyChangedEventArgs args)
        {
            ChartAxis axis = dpObj as ChartAxis;
            if (axis != null && axis.Area != null)
            {
               
                axis.Area.UpdateArea();
                    
            }
        }
        private static void OnsizeChange(DependencyObject dpObj, DependencyPropertyChangedEventArgs args)
        {
            ChartAxis axis = dpObj as ChartAxis;
            if (axis != null && axis.Area != null)
            {
                axis.Area.UpdateArea();
            }
        }
        /// <summary>
        /// Called when LabelHorizontalAlignment and LabelVerticalAlignment properties are changed.
        /// </summary>
        /// <param name="dpObj">The dp obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnLabelAlignmentChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs args)
        {
            ChartAxis axis = dpObj as ChartAxis;

            if (axis != null && axis.Area != null)
                axis.Area.UpdateArea();
        }

        /// <summary>
        /// Called when LabelWidth and LabelHeight properties are changed.
        /// </summary>
        /// <param name="dpObj">The dp obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnLabelSizeChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs args)
        {
            ChartAxis axis = dpObj as ChartAxis;
            axis.isNeedUpdate = true;
            if (axis != null && axis.Area != null && axis.LabelHorizontalAlignment != HorizontalAlignment.Stretch || axis.LabelVerticalAlignment != VerticalAlignment.Stretch)
            {
                axis.Area.UpdateArea();
            }
        }
        /// <summary>
        /// Called when SmallTicksPerInterval property changed.
        /// </summary>
        /// <param name="dpObj">The dp obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSmallTicksPerIntervalPropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs args)
        {
            ChartAxis axis = dpObj as ChartAxis;
            if (axis != null && axis.Area != null)
            {
                axis.isNeedUpdate = true;
                axis.Area.UpdateArea();
            }
        }

        /// <summary>
        /// Raises corresponding mouse event on ChartAxis.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private static void RaiseMouseEvent(object sender, RoutedEventArgs e)
        {
            ////Retrieving panel that sent event.
            ChartCartesianAxisPanel panel = sender as ChartCartesianAxisPanel;
            ////Retrieving content presenter.
            ContentPresenter presenter = VisualTreeHelper.GetParent(panel) as ContentPresenter;
            if (presenter != null && presenter.Content is ChartAxis)
            {
                ////Retrieving corresponding axis.
                ChartAxis axis = presenter.Content as ChartAxis;
                ////Raise required event.
                axis.RaiseEvent(e);
            }
        }

        private static object OnCoerceLogRange(DependencyObject d, object baseValue)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null && axis.Area != null && (object)axis.LogarithmicRange == baseValue)
            {
                axis.Area.UpdateArea();
            }
            return baseValue;
        }

        private static void OnLogRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null)
            {
                DoubleRange logRange = (DoubleRange)e.NewValue;
                if (!logRange.IsEmpty && logRange.Start > 0 && logRange.End > 0)
                {
                    axis.m_previousRange = axis.InternalRange;
                    axis.InternalRange = new DoubleRange(Math.Log(logRange.Start, axis.LogarithmicBase), Math.Log(logRange.End, axis.LogarithmicBase));
                }
                else if (axis.InternalRange != axis.m_cachedRangeValue)
                {
                    axis.m_previousRange = axis.InternalRange;
                    axis.InternalRange = axis.m_cachedRangeValue;
                }
                else
                {
                    if (!axis.m_previousRange.IsEmpty)
                    axis.InternalRange = axis.m_previousRange;
                }
            }
        }

        /// <summary>
        /// Called when date time range changed.
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDateTimeRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null)
            {
                DateTimeRange dateTimeRange = (DateTimeRange)e.NewValue;
                if (!dateTimeRange.IsEmpty && axis.ValueType == ChartValueType.DateTime)
                {
                    axis.InternalRange = new DoubleRange(dateTimeRange.Start.ToOADate(), dateTimeRange.End.ToOADate());
                }
                else if (axis.InternalRange != axis.m_cachedRangeValue)
                {
                    axis.InternalRange = axis.m_cachedRangeValue;
                }
            }
        }

        /// <summary>
        /// Called when DateTimeInterval property was changed.
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDateTimeIntervalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis.ValueType == ChartValueType.DateTime)
            {
                ////Checks whether the DateTimeInterval is less than MinimumDateTimeInterval
                if ((TimeSpan)e.NewValue < axis.MinimumDateTimeInterval)
                {
                    axis.BaseInterval = (DateTime.Now + (TimeSpan)axis.MinimumDateTimeInterval).ToOADate() - DateTime.Now.ToOADate();
                }
                else
                {
                    axis.BaseInterval = (DateTime.Now + (TimeSpan)e.NewValue).ToOADate() - DateTime.Now.ToOADate();
                }
                axis.checkDateTimeIntervalChange = true;
            }
        }


        private static void OnRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null)
            {
                DoubleRange range = (DoubleRange)e.NewValue;
                if (!range.IsEmpty)
                {
                    axis.InternalRange = range;
                }
                
            }
        }

        private static void OnEnableSmartAxisLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis=d as ChartAxis;
            if (axis != null)
            {
                //if (axis.EnableSmartAxisLabel == false)
                //    axis.DoubleDisplayUnit= DoubleUnits.AutoDetect;
                //else
                //    axis.DoubleDisplayUnitVisibility = Visibility.Hidden;
                axis.Invalidate();
            }
        }

        private static void OnDoubleDisplayUnitChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null)
            {
                axis.DoubleDisplayUnit = axis.DoubleDisplayUnit;
                axis.Invalidate();
            }
        }

        private static void OnEnableBreaksChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            axis.Invalidate();
        }

       
        //Included this method to make function of Logarthimic Valuetype.
        private static void OnValuetypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null)
            {
                axis.checkDateTimeIntervalChange = false;
                //if ((ChartValueType)e.NewValue == ChartValueType.Logarithmic)
                //{
                //    axis.IsLogarithmic = true;
                //}
                //else
                //{
                //    axis.IsLogarithmic = false;
                //}


                if (axis.IsAutoSetRange == false)
                {
                    if ((ChartValueType)e.NewValue == ChartValueType.Logarithmic)
                    {
                        axis.BaseInterval = axis.LogarithmicInterval;
                        if (!axis.LogarithmicRange.IsEmpty && axis.LogarithmicRange.Start > 0 && axis.LogarithmicRange.End > 0)
                        {
                            axis.m_previousRange = axis.InternalRange;
                            axis.InternalRange = new DoubleRange(Math.Log(axis.LogarithmicRange.Start, axis.LogarithmicBase), Math.Log(axis.LogarithmicRange.End, axis.LogarithmicBase));
                        }
                        else if (axis.InternalRange != axis.m_cachedRangeValue)
                        {
                            axis.m_previousRange = axis.InternalRange;
                            axis.InternalRange = axis.m_cachedRangeValue;
                        }
                        else
                        {
                            axis.InternalRange = axis.m_previousRange;
                        }
                    }
                    else if ((ChartValueType)e.NewValue == ChartValueType.DateTime)
                    {
                        axis.BaseInterval = double.NaN;
                        axis.InternalRange = DoubleRange.Empty;
                       if (axis.DateTimeInterval != new TimeSpan())
                            axis.BaseInterval = (DateTime.Now + (TimeSpan)axis.DateTimeInterval).ToOADate() - DateTime.Now.ToOADate();
                        axis.InternalRange = new DoubleRange(axis.DateTimeRange.Start.ToOADate(), axis.DateTimeRange.End.ToOADate());
                    }
                    else
                    {
                        if (!double.IsNaN(axis.Interval))
                            axis.BaseInterval = axis.Interval;
                        axis.InternalRange = axis.Range;
                    }
                }
                else if (axis.IsAutoSetRange == true)
                {

                    if (!double.IsNaN(axis.Interval))
                        axis.BaseInterval = axis.Interval;
                    axis.InternalRange = DoubleRange.Empty;
                }

                axis.Invalidate();
            }
        }
       
        /// <summary>
        /// Triggers when MinimalZoomFactor changed.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnMinimalZoomFactorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null && axis.Area != null)
                axis.Area.UpdateArea();
        }

        private static void OnZoomFactorValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null && axis.Area != null)
            {
                if (axis.Orientation == Orientation.Horizontal)
                {
                    if (axis.Area is SyncChartAreas)
                    {
                        foreach (ChartArea area1 in (axis.Area as SyncChartAreas).Areas)
                        {
                            if (area1.PrimaryAxis != null)
                            {
                                area1.PrimaryAxis.ZoomFactor = axis.ZoomFactor;
                                ChartAreaCommands.ZoomIn.Execute(null, area1);
                            }
                        }
                    }

                }
            }
            axis._ZoomFactorisset = true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>

        private static void OnAutoSetRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            ChartAxis axis = d as ChartAxis;

            if (axis != null && axis.Area != null && axis.Area.disableIsIndexedForOLAP == false)
            {
                if (axis.Orientation == Orientation.Horizontal)
                {
                    if (axis.Area.PrimarySeries != null && axis.Area.PrimarySeries.Type != ChartTypes.Bar && axis.Area.PrimarySeries.Type != ChartTypes.StackingBar && axis.Area.PrimarySeries.Type != ChartTypes.StackingBar100)
                    {
                        if ((bool)e.NewValue == false)
                        {
                            isIndexedOfPrimary = axis.Area.PrimarySeries.IsIndexed;
                            if (!(axis.Area is TimeLineControl))
                            {
                                axis.Area.PrimarySeries.IsIndexed = false;
                            }
                        }
                        else
                        {
                            axis.Area.PrimarySeries.IsIndexed = isIndexedOfPrimary;
                        }
                    }
                }
                if ((bool)e.NewValue == true)
                {

                    if (!double.IsNaN(axis.Interval))
                        axis.BaseInterval = axis.Interval;
                    axis.InternalRange = DoubleRange.Empty;
                }
                else
                {
                    if (axis.LogarithmicRange != DoubleRange.Empty && axis.ValueType == ChartValueType.Logarithmic)
                    {
                        if (!double.IsNaN(axis.Interval))
                            axis.BaseInterval = axis.Interval;
                        axis.InternalRange = new DoubleRange(Math.Log(axis.LogarithmicRange.Start, axis.LogarithmicBase), Math.Log(axis.LogarithmicRange.End, axis.LogarithmicBase));
                    }
                    else if (axis.ValueType == ChartValueType.DateTime)
                    {
                       if (axis.DateTimeInterval != new TimeSpan())
                            axis.BaseInterval = (DateTime.Now + (TimeSpan)axis.DateTimeInterval).ToOADate() - DateTime.Now.ToOADate();
                        axis.InternalRange = new DoubleRange(axis.DateTimeRange.Start.ToOADate(), axis.DateTimeRange.End.ToOADate());
                    }
                    else
                    {
                        if (!double.IsNaN(axis.Interval))
                            axis.BaseInterval = axis.Interval;
                        axis.InternalRange = axis.Range;
                    }
                }
                axis.Area.UpdateArea();
            }
        }

        /// <summary>
        /// Called when the EnableAutoScrolling value is changed.
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnEnableAutoScrollingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null && axis.Area != null && axis.Area.disableIsIndexedForOLAP == false)
            {                
                axis.Area.UpdateArea();
            }
        }
        private static void OnForceZeroPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null && axis.Area != null)
            {
                axis.CalculateVisibleRange();
            }
        }
        private static void OnLabelSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            IEnumerator a;
            if (e.NewValue is IEnumerable)
            {
                a = (e.NewValue as IEnumerable).GetEnumerator();
                if (a.MoveNext())
                {
                    do
                    {
                        if (a.Current is INotifyPropertyChanged)
                        {
                            (a.Current as INotifyPropertyChanged).PropertyChanged += (s, args) => axis.LabelSourcePropertyChanged();
                        }
                    }
                    while (a.MoveNext());
                }
                
            }
        }
        internal void LabelSourcePropertyChanged()
        {
            this.CalculateVisibleLables();
        }
        /// <summary>
        /// Called when the ShowAllLables value is changed.
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnShowAllLablesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            axis.m_showAllLabelsChanged = true;
            axis.Invalidate();
        }
  
        /// <summary>
        /// Called when the AutoScrollingDelta value is changed.
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnAutoScrollingDeltaChanged (DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis.EnableAutoScrolling.HasValue)
            {
                DoubleRange scrollingRange = axis.IgnoreRangePaddingsOnZoom ? axis.m_requestedRange :
                     axis.InternalRange.IsEmpty ? axis.NiceRange : axis.InternalRange;
                axis.ZoomFactor = axis.m_autoScrollingZoomFactor;
                axis.ZoomPosition = 1;
            }
        }
        /// <summary>
        /// Called when MinimumInterval property changed.
        /// </summary>
        /// <param name="dpObj">The dp obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnMinimumIntervalPropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs args)
        {
            ChartAxis axis = dpObj as ChartAxis;
            axis.isNeedUpdate = true;
            if (axis.BaseInterval < axis.MinimumInterval)
            {
                axis.BaseInterval = axis.MinimumInterval;
            }
            //else if (double.IsNaN(axis.Interval))
            //{
            //    axis.Interval = axis.MinimumInterval;
            //}
        }

        /// <summary>
        /// Called when MinimumDateTimeInterval property changed.
        /// </summary>
        /// <param name="dpObj">The dp obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnMinimumDateTimeIntervalPropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs args)
        {
            ChartAxis axis = dpObj as ChartAxis;
            if (axis.DateTimeInterval <= axis.MinimumDateTimeInterval)
            {
                axis.m_tempDateTimeInterval = axis.DateTimeInterval;
                axis.DateTimeInterval = axis.MinimumDateTimeInterval;
            }
            else
            {
                axis.DateTimeInterval = axis.m_tempDateTimeInterval;
            }
        }

        /// <summary>
        /// Called when the RangeCalculationMode is changed.
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnRangeCalcultaionModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null && axis.Area != null)
            {
                foreach (ChartSeries series in axis.Area.Series)
                {
                    series.BeginUpdate();
                    series.EndUpdate();
                }
            }
        }

        /// <summary>
        /// Corces Range property.
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="baseValue">The base value.</param>
        /// <returns>The Range value</returns>
        private static object RangeCoerce(DependencyObject d, object baseValue)
        {
            ChartAxis axis = d as ChartAxis;
            axis.m_cachedRangeValue = (DoubleRange)baseValue;
            bool reqNegativeStack = false;
            if (axis != null && axis.Area != null)
            {
                if (axis.Area.PrimarySeries != null && ((axis.Area.PrimarySeries.Type == ChartTypes.StackingColumn100 && axis.Orientation == Orientation.Vertical) ||
                    (axis.Area.PrimarySeries.Type == ChartTypes.StackingBar100 && axis.Orientation == Orientation.Horizontal)))
                {
                    reqNegativeStack = axis.Area.PrimarySeries.Type == ChartTypes.StackingColumn100 ? ChartStackingColumn100Type.GetRequiresNegativeSeriesStack(axis.Area) : ChartStackingBar100Type.GetRequiresNegativeSeriesStack(axis.Area);
                    if(axis.IsAutoSetRange)
                        return ChartStackingColumn100Type.GetShowValueAsProbability(axis.Area) ? 
                            ((!axis.Area.hasStack100NegValues || !reqNegativeStack) ? (new DoubleRange(0, 1)) : (new DoubleRange(-1, 1))) : 
                            ((!axis.Area.hasStack100NegValues || !reqNegativeStack) ? (new DoubleRange(0,100)) : (new DoubleRange(-100, 100)));
                }
                else if (axis.Area.PrimarySeries != null && (axis.Area.PrimarySeries.Type == ChartTypes.StackingArea100 && axis.Orientation == Orientation.Vertical))
                {
                    reqNegativeStack = ChartStackingArea100Type.GetRequiresNegativeSeriesStack(axis.Area) ;
                    if(axis.IsAutoSetRange)
                        return ChartStackingArea100Type.GetShowValueAsProbability(axis.Area) ? 
                            ((!axis.Area.hasStack100NegValues || !reqNegativeStack) ? (new DoubleRange(0, 1)) : (new DoubleRange(-1, 1))) : 
                            ((!axis.Area.hasStack100NegValues || !reqNegativeStack) ? (new DoubleRange(0,100)) : (new DoubleRange(-100, 100)));
                }
                else if (axis.Area.PrimarySeries != null && (axis.Area.PrimarySeries.Type == ChartTypes.StackingLine100 && axis.Orientation == Orientation.Vertical))
                {
                    reqNegativeStack = ChartStackingLine100Type.GetRequiresNegativeSeriesStack(axis.Area);
                    if (axis.IsAutoSetRange)
                        return ChartStackingLine100Type.GetShowValueAsProbability(axis.Area) ?
                            ((!axis.Area.hasStack100NegValues || !reqNegativeStack) ? (new DoubleRange(0, 1)) : (new DoubleRange(-1, 1))) :
                            ((!axis.Area.hasStack100NegValues || !reqNegativeStack) ? (new DoubleRange(0, 100)) : (new DoubleRange(-100, 100)));
                }
                else if (axis.Area.PrimarySeries != null && (axis.Area.PrimarySeries.Type == ChartTypes.StackingSpline100 && axis.Orientation == Orientation.Vertical))
                {
                    reqNegativeStack = ChartStackingSpline100Type.GetRequiresNegativeSeriesStack(axis.Area);
                    if (axis.IsAutoSetRange)
                        return ChartStackingSpline100Type.GetShowValueAsProbability(axis.Area) ?
                            ((!axis.Area.hasStack100NegValues || !reqNegativeStack) ? (new DoubleRange(0, 1)) : (new DoubleRange(-1, 1))) :
                            ((!axis.Area.hasStack100NegValues || !reqNegativeStack) ? (new DoubleRange(0, 100)) : (new DoubleRange(-100, 100)));
                }
                else if (axis.Area.PrimarySeries != null && (axis.Area.PrimarySeries.Type == ChartTypes.StackingSplineArea100 && axis.Orientation == Orientation.Vertical))
                {
                    reqNegativeStack = ChartStackingSplineArea100Type.GetRequiresNegativeSeriesStack(axis.Area);
                    if (axis.IsAutoSetRange)
                        return ChartStackingSplineArea100Type.GetShowValueAsProbability(axis.Area) ?
                            ((!axis.Area.hasStack100NegValues || !reqNegativeStack) ? (new DoubleRange(0, 1)) : (new DoubleRange(-1, 1))) :
                            ((!axis.Area.hasStack100NegValues || !reqNegativeStack) ? (new DoubleRange(0, 100)) : (new DoubleRange(-100, 100)));
                }
                return axis.IsAutoSetRange ? DoubleRange.Empty : baseValue;
            }

            return baseValue;
        }


        /// <summary>
        /// Called when [interval property changed].
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIntervalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null)
            {
                if ((double)e.NewValue < axis.MinimumInterval)
                {
                    axis.BaseInterval = axis.MinimumInterval;
                }
                else
                {
                    axis.BaseInterval = (double)e.NewValue;
                    if(axis.Indexed)
                    axis.Area.UpdateArea();
                }
            }
        }

        /// <summary>
        /// Called when [BaseInterval property changed].
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnBaseIntervalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null)
            {
                axis.CoerceValue(DesiredIntervalsCountProperty);
            }
        }
        private static void OnLogarithmicIntervalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null)
            {
                if (axis.ValueType == ChartValueType.Logarithmic)
                {
                    axis.Invalidate();
                }
                axis.CoerceValue(DesiredIntervalsCountProperty);
            }
        }

        private static void OnLogIntervalTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null)
            {
                axis.Invalidate();
            }
        }

        /// <summary>
        /// Called when DesiredIntervalsCount property was changed.
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDesiredIntervalsCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            
            if (axis != null && axis.Area != null)
            {
                axis.isNeedUpdate = true;
                axis.Area.RequestForSegmentsReset();
                axis.Area.UpdateArea();
            }
        }

        /// <summary>
        /// Called when LogarithmicBase property changed.
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnLogarithmicBasePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null)
            {
                if (axis.ValueType == ChartValueType.Logarithmic)
                {
                    if (axis.IsAutoSetRange == false && axis.LogarithmicRange != DoubleRange.Empty)
                    {
                        axis.InternalRange = new DoubleRange(Math.Log(axis.LogarithmicRange.Start, axis.LogarithmicBase), Math.Log(axis.LogarithmicRange.End, axis.LogarithmicBase));
                    }
                    axis.Area.RequestForSegmentsReset();
                    axis.Area.UpdateArea();
                }
            }
        }
        private static object CoerceOriginTickProperty(DependencyObject d, object baseValue)
        {
            ChartAxis axis = d as ChartAxis;
            double newValue = (double)baseValue;
            if (newValue < 0)
                newValue = 0d;
            else if (newValue > 1)
                newValue = 1d;
            //if (axis.TickLinesRange < 0)
            //{
            //    axis.TickLinesRange = 0d;
            //}
            //if (axis.TickLinesRange > 1)
            //{
            //    axis.TickLinesRange = 1d;
            //}
            //if (axis.SmallTickLinesRange < 0)
            //{
            //    axis.SmallTickLinesRange = 0d;
            //}
            //if (axis.SmallTickLinesRange > 1)
            //{
            //    axis.SmallTickLinesRange = 1d;
            //}
            ////if (axis.IsLogarithmic)
            ////{
            ////    return (axis.Origin <= 0) ? 1d : baseValue;
            ////}
            axis.CalculateVisibleRange();

            return newValue;
        }
        /// <summary>
        /// Coerces the origin property.
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="baseValue">The base value.</param>
        /// <returns>The OriginProperty</returns>
        private static object CoerceOriginProperty(DependencyObject d, object baseValue)
        {
            ChartAxis axis = d as ChartAxis;
            //if (axis.TickLinesRange < 0)
            //{
            //    axis.TickLinesRange = 0d;
            //}
            //if (axis.TickLinesRange > 1)
            //{
            //    axis.TickLinesRange = 1d;
            //}
            //if (axis.SmallTickLinesRange < 0)
            //{
            //    axis.SmallTickLinesRange = 0d;
            //}
            //if (axis.SmallTickLinesRange > 1)
            //{
            //    axis.SmallTickLinesRange = 1d;
            //}
            ////if (axis.IsLogarithmic)
            ////{
            ////    return (axis.Origin <= 0) ? 1d : baseValue;
            ////}
            axis.CalculateVisibleRange();

            return baseValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="baseValue"></param>
        /// <returns></returns>
        private static object OnCoerceAutoSetRange(DependencyObject d, object baseValue)
        {
            ChartAxis axis = d as ChartAxis;

            if (axis != null && axis.Area != null)
            {
                //if (axis.Area.PrimarySeries != null)
                //{
                //axis.Area.PrimarySeries.IsIndexed = (bool)baseValue;
                //}

                axis.Area.UpdateArea();
            }
            return baseValue;
        }

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;

            if (axis != null)
            {

            }
        }


        /// <summary>
        /// Coerces the desired intervals count property.
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="baseValue">The base value.</param>
        /// <returns>The DesiredIntervalsCount</returns>
        private static object CoerceDesiredIntervalsCountProperty(DependencyObject d, object baseValue)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null)
            {
                if (axis.ReadLocalValue(BaseIntervalProperty) != DependencyProperty.UnsetValue)
                {
                    return DependencyProperty.UnsetValue;
                }
            }

            return baseValue;
        }

        /// <summary>
        /// Called when visible range gets changed.
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnVisibleRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis.Area != null && axis.Area.Series != null)
            {
                foreach (var ser in axis.Area.Series)
                {
                    if (ser.Presenter != null)
                        ser.Presenter.rangechanged = true;
                }
            }
            if (axis.RangeChanged != null)
            {
                axis.RangeChanged(axis, new ChartAxisRangeArgs(axis, (DoubleRange)e.NewValue));
            }
            if(axis.IsLoaded)
            axis.OnRangeChanged(new ChartAxisRangeArgs(axis, (DoubleRange)e.NewValue));
        }

        private static void OnLineStrokeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null)
            {
                if (axis.Area != null)
                {
                    axis.Area.UpdateArea();
                }
            }
        }


        private static void OnIsOriginCenteredChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null)
            {
                if (axis.Area != null)
                {
                    axis.Area.UpdateArea();
                }
            }
        }       

        private DoubleRange GetOriginCenteredRange(DoubleRange range)
        {
            if (this.IsOriginCentered)
            {
                if (range.Median != this.Origin)
                {
                    double maxend = (Math.Abs(range.Start) > Math.Abs(range.End)) ? Math.Abs(range.Start) : Math.Abs(range.End);
                    return new DoubleRange(this.Origin - maxend, this.Origin + maxend);
                }
            }
            return range;
        }

        private static void OnSmallTickStrokeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null)
            {
                if (axis.Area != null)
                {
                    axis.Area.UpdateArea();
                }
            }
        }
        private static void OnTickStrokeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null)
            {
                if (axis.Area != null)
                {
                    axis.Area.UpdateArea();
                }
            }
        }
        /// <summary>
        /// Called when RangePadding property was changed.
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnRangePaddingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null)
            {
                axis.SetNiceRange(axis.m_requestedRange);
                axis.RaiseChanged(axis, new AxisChangedEventArgs(e));
                if (axis.Area != null)
                {
                    foreach (ChartSeries series in axis.Area.Series)
                    {
                        series.Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// Called when origin property was changed. 
        /// </summary> 
        /// <param name="d">The DependencyObject d.</param> 
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param> 
        private static void OnOriginPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null && axis.Area != null)
            {
                axis.Area.RequestForSegmentsReset();
                axis.Area.UpdateArea();
            }
        }

        private static void OnIsSetDataValueRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null && axis.Area != null)
            {
                axis.Area.UpdateArea();
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (StripLines != null)
                StripLines.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.OnStripLinesChanged);
            if (this.BreakRange != null)
                this.BreakRange.Changed -= new EventHandler(this.OnBreakRangeChanged);
            if (m_customLables != null)
                m_customLables.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnCustomLablesCollectionChanged);

            Changed = null;
            this.Area = null;
            if (this.CustomLabels != null)
                this.CustomLabels.Clear();
            if (this.VisibleLabels != null)
                this.VisibleLabels.Clear();
            LabelTemplate = null;
            this.Header = null;
            this.m_owner = null;


            m_visibleRange = DoubleRange.Empty;
            m_logarihmicVisibleRange = DoubleRange.Empty;


            if (this.m_customLables != null)
            {
                this.CustomLabels.m_chartAxis = null;
                for (int i = 0; i < this.CustomLabels.Count && this.CustomLabels[i] != null; i++)
                {
                    this.CustomLabels[i].Axis = null;
                    this.CustomLabels[i].Dispose();
                    this.CustomLabels[i] = null;
                }
                this.m_customLables.Clear();
                this.m_customLables = null;
            }


            //this.m_owner = null;

            this.m_ticksPoint = null;

            if (this.m_visibleLables != null)
            {
                this.VisibleLabels.m_chartAxis = null;
                for (int i = 0; i < this.VisibleLabels.Count && this.VisibleLabels[i] != null; i++)
                {
                    this.VisibleLabels[i].Axis = null;
                    this.VisibleLabels[i].Dispose();
                    this.VisibleLabels[i] = null;
                }
                this.m_visibleLables.Clear();
                this.m_visibleLables = null;
            }

            if (this.m_AxisDataModel != null)
            {
                this.m_AxisDataModel.Dispose();
                //this.m_AxisDataModel.ChartPoints.Clear();
                this.m_AxisDataModel.PathX = null;
                this.m_AxisDataModel.ChartPoints = null;
                this.m_AxisDataModel = null;
            }

            if (this.StripLines != null)
            {
                for (int i = 0; i < StripLines.Count; i++)
                {
                    if (StripLines[i] != null)
                    {
                        this.StripLines[i].Dispose();
                        this.StripLines[i] = null;
                    }
                }

                this.StripLines.Clear();
                this.StripLines = null;
            }

            this.Range = DoubleRange.Empty;
            this.InternalRange = DoubleRange.Empty;
            this.DataContext = null;
            this.ClearValue(ChartAxis.RangeProperty);
            this.ClearValue(ChartAxis.InternalRangeProperty);
            this.ClearValue(ChartAxis.DateTimeRangeProperty);
            this.ClearValue(ChartAxis.DataContextProperty);
            this.ClearValue(ChartAxis.DefaultStyleKeyProperty);
            this.ClearValue(ChartAxis.IntersectActionProperty);
            this.ClearValue(ChartAxis.RangePaddingProperty);
            this.ClearValue(ChartAxis.RangeCalculationModeProperty);
            this.ClearValue(ChartAxis.LabelsModeProperty);
            this.ClearValue(ChartAxis.LabelTemplateProperty);
            this.ClearValue(ChartAxis.AxisVisibilityProperty);
            this.ClearValue(ChartAxis.ValueTypeProperty);
            this.ClearValue(ChartAxis.LabelFontFamilyProperty);
            this.ClearValue(ChartAxis.LabelFontWeightProperty);
            this.ClearValue(ChartAxis.LabelForegroundProperty);
            this.ClearValue(ChartAxis.LabelFormatProperty);
            this.ClearValue(ChartAxis.LabelDateTimeFormatProperty);
            this.ClearValue(ChartAxis.OrientationProperty);
            this.LabelFormat = null;
            this.LabelDateTimeFormat = null;
            this.InteractiveCursorTemplate = null;
            this.TickLineStroke = null;
            this.ToolTip = null;
            this.Area = null;

            this.BreakRange = null;

        }

        ///// <summary>
        ///// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        ///// </summary>
        //public void Dispose()
        //{

        //    //m_stripLines.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.OnStripLinesChanged);
        //    //m_breakRange.Changed -= new EventHandler(this.OnBreakRangeChanged);
        //    //m_customLables.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnCustomLablesCollectionChanged);

        //    Changed = null;
        //    this.Area = null;
        //    this.CustomLabels.Clear();
        //    this.VisibleLabels.Clear();
        //    LabelTemplate = null;

        //    //Changed = null;
        //    //this.Area = null;
        //    //this.CustomLabels.Clear();
        //    //this.VisibleLabels.Clear();
        //    //LabelTemplate = null;
        //    //m_visibleRange = DoubleRange.Empty;
        //    //m_logarihmicVisibleRange = DoubleRange.Empty;
        //    //this.m_customLables = null;
        //    //this.m_owner.Series.Clear();
        //    //this.m_owner = null;
        //    //this.TicksPoint.Clear();
        //    //this.m_ticksPoint = null;
        //    //this.m_visibleLables = null;
        //    //this.StripLines.Clear();
        //    //this.m_stripLines = null;
        //    //this.TicksPoint.Clear();
        //    //this.m_ticksPoint.Clear();
        //    //this.m_ticksPoint = null;
        //    //this.Range = DoubleRange.Empty;
        //    //this.Resources.Clear();
        //    //this.Resources = null;
        //    //this.TickLineStroke = null;
        //    //this.ToolTip = null;


        //    //m_breakRange = null;

        //}

        #endregion

        #region IChartSerializer Members

        /// <summary>
        /// Method declaration for Serialize
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            EditorHelper.Register<BindingExpression, BindingConvertor>();
           
            StringBuilder outstr = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;
            XamlDesignerSerializationManager dsm = new XamlDesignerSerializationManager(XmlWriter.Create(outstr, settings));
            //this string need for turning on expression saving mode 
            dsm.XamlWriterMode = XamlWriterMode.Expression;
            XamlWriter.Save(this, dsm);
            return outstr.ToString();
        }

        /// <summary>
        /// Method declaration for DeSerialize
        /// </summary>
        /// <param name="xamlString"></param>
        /// <returns></returns>
        public object Deserialize(string xamlString)
        {
            return XamlReader.Parse(xamlString);
        }

        #endregion
    }

    /// <summary>
    /// Represents label that is shown on axis ticks.
    /// </summary>
    /// <remarks>
    /// Axis labels could be set from various data sources.
    /// </remarks>
    /// <example>
    /// XAML:
    /// <code language="XAML">
    ///  &lt;syncfusion:ChartAxis Header="Year" LabelsSource="{Binding
    /// Source={StaticResource productlist}}" PositionPath="ID" ContentPath="Year" 
    /// LabelFontSize="11"&gt;
    /// </code>
    /// C#:
    /// <code language="C#">
    ///  Chart1.Areas[0].PrimaryAxis.LabelsSource = productlist;
    ///             Chart1.Areas[0].PrimaryAxis.PositionPath="ID";
    ///               Chart1.Areas[0].PrimaryAxis.ContentPath="Year"
    /// </code>
    /// </example>
    /// <seealso cref="ChartAxisLabelsMode"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartAxisLabel : DependencyObject,IDisposable
    {
        #region Members



        //internal ChartCartesianAxisLabelsPanel ParentPanel
        //{
        //    get { return (ChartCartesianAxisLabelsPanel)GetValue(ParentPanelProperty); }
        //    set { SetValue(ParentPanelProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for Parent.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty ParentPanelProperty =
        //    DependencyProperty.Register("ParentPanel", typeof(ChartCartesianAxisLabelsPanel), typeof(ChartAxisLabel), new UIPropertyMetadata(new ChartCartesianAxisLabelsPanel()));


        /// <summary>
        /// Initializes m_position
        /// </summary>
        private double m_position = double.NaN;
        internal bool m_isCustomLabel = false;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the position of label on axis.
        /// </summary>
        /// <value>The position.</value>
        public double Position
        {
            get { return m_position; }
            set { m_position = value; }
        }

        /// <summary>
        /// Gets or sets the content that label should display.
        /// </summary>
        /// <value>Label's content.</value>


        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(ChartAxisLabel), new PropertyMetadata(null));

        

        /// <summary>
        /// Gets the ChartAxis that label resides.
        /// </summary>
        public ChartAxis Axis
        {
            get;
            internal set;
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartAxisLabel"/> class.
        /// </summary>
        public ChartAxisLabel()
        {

            //Binding parentBinding = new Binding();
            //parentBinding.Source = this;
            //parentBinding.Path = new PropertyPath("Parent");
            //BindingOperations.SetBinding(this, ChartAxisLabel.ParentPanelProperty, parentBinding);


        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartAxisLabel"/> class.
        /// </summary>
        /// <param name="position">The position.</param>
        public ChartAxisLabel(double position)
            : this(position, position)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartAxisLabel"/> class.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="label">The label.</param>
        public ChartAxisLabel(double position, object label)
        {
            m_position = position;
            this.Content = label;
        }
        #endregion

        #region Public methdos
        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current axis label.
        /// </returns>
        public override string ToString()
        {
            if (this.Content == null)
                return null;
            return this.Content.ToString();
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            ////Content = null;
            this.Axis = null;
            this.Content = null;
        }

        #endregion
    }

   
}