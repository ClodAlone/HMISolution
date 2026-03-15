#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections.Specialized;
using System.ComponentModel;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
#else
using Windows.UI;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.Data.Xml.Dom;
using System.Threading.Tasks;
using Windows.ApplicationModel;
#endif

namespace Syncfusion.UI.Xaml.Charts
{

    /// <summary>
    /// Enables plotting of data points in a chart control.
    /// </summary>
    /// <remarks>
    /// The Chart requires a minimum of two axes namely primary axis and secondary
    /// axis to plot data points. Values / data in the chart are plotted against these two
    /// axes. Chart WINRT also supports adding multiple axes to the chart and the
    /// series can be drawn with reference to any x-axis,y-axis added to <see cref="SfChart"/>
    /// </remarks>
    [TemplateVisualStateAttribute(Name = "CommonStyle", GroupName = "StyleMode")]
    [TemplateVisualStateAttribute(Name = "TouchModeStyle", GroupName = "StyleMode")]
    [TemplateVisualStateAttribute(Name = "VerticalTouchModeStyle", GroupName = "StyleMode")]
    [ClassReference(IsReviewed = false)]
    public abstract class ChartAxis : Control,ICloneable
    {
        #region fields
        internal bool IsScrolling { get; set; }

        internal bool IsDataChanged { get; set; }

        /// <summary>
        /// CRoundDecimals const variable declarations
        /// </summary>
        protected const int CRoundDecimals = 6;
        /// <summary>
        /// MaxPixelsCount  variable declarations
        /// </summary>
        protected double MaxPixelsCount = 100;

        private double actualPlotOffset = 0;

        public double ActualPlotOffset
        {
            get { return actualPlotOffset; }
            internal set { actualPlotOffset = value; }
        }

        internal double InsidePadding { get; set; }
	    bool isChecked = false;
#if WINDOWS_PHONE
        //bool isUpdateStripDispatched = false;
#else
        IAsyncAction checkRegisterAction;
#endif

        /// <summary>
        /// Contains Actual VisibleRange
        /// </summary>
        DoubleRange m_actualVisibleRange = DoubleRange.Empty;
        Rect arrangeRect;

        internal bool axisElementsUpdateRequired = false;
        /// <summary>
        /// isInversed variable declarations
        /// </summary>
        protected bool isInversed = false;

        internal ILayoutCalculator AxisLayoutPanel;

        /// <summary>
        /// Initializes c_intervalDivs
        /// </summary>
        internal readonly static int[] c_intervalDivs = new int[] { 10, 5, 2, 1 };

        /// <summary>
        /// Contains  actual Range WithoutPadding
        /// </summary>
        private DoubleRange m_actualRange = DoubleRange.Empty;

        internal ILayoutCalculator axisLabelsPanel;

        internal ILayoutCalculator axisElementsPanel;

        internal UIElementsRecycler<Line> GridLinesRecycler;

        internal UIElementsRecycler<Line> MinorGridLinesRecycler;

        internal List<Line> GridLines;

        internal List<Line> MinorGridLines;

        internal ChartBase Area;

        internal ContentControl headerContent;

        internal List<double> m_smalltickPoints = new List<double>();

        internal event EventHandler<VisibleRangeChangedEventArgs> VisibleRangeChanged;

        public event EventHandler<ChartAxisBoundsEventArgs> AxisBoundsChanged;

        internal List<double> SmallTickPoints
        {
            get { return m_smalltickPoints; }
        }

        internal Size AvailableSize { get; set; }

        internal bool isManipulated = false;

        #endregion

        #region events

        /// <summary>
        /// Occurs when [actual range changed].
        /// </summary>
        public event EventHandler<ActualRangeChangedEventArgs> ActualRangeChanged;

        #endregion

        #region delegate

        internal delegate double ValueToCoefficientHandler(double value);

        internal ValueToCoefficientHandler ValueToCoefficientCalc { get; set; }

        internal ValueToCoefficientHandler CoefficientToValueCalc { get; set; }

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets the maximum number of labels per 100 pixels.
        /// </summary>
        /// <value>
        /// The maximum labels.
        /// </value>
        public int MaximumLabels
        {
            get { return (int)GetValue(MaximumLabelsProperty); }
            set { SetValue(MaximumLabelsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaximumLabels.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximumLabelsProperty =
            DependencyProperty.Register("MaximumLabels", typeof(int), typeof(ChartAxis), new PropertyMetadata(3, OnPropertyChanged));

        internal Size ComputedDesiredSize { get; set; }

        /// <summary>
        /// Gets the visible range.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public DoubleRange VisibleRange
        {
            get
            {
                return m_actualVisibleRange;
            }
            protected internal set
            {
                DoubleRange oldRange = m_actualVisibleRange;
                m_actualVisibleRange = value;
                OnAxisVisibleRangeChanged(new VisibleRangeChangedEventArgs() { OldRange = oldRange, NewRange = value });
            }
        }

        internal double VisibleInterval { get; set; }

        internal double ActualInterval { get; set; }

        internal bool IsRangeCalculating { get; set; }

        internal DoubleRange ActualRange
        {
            get { return m_actualRange; } 
            set { m_actualRange = value; }
        }

        /// <summary>
        /// Gets the bounds of the ChartAxis.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Rect ArrangeRect
        {
            get
            {
                return arrangeRect;
            }
            internal set
            {
                arrangeRect = value;
                if (Orientation == Orientation.Horizontal)
                {
                    double width = Math.Max(0, arrangeRect.Width - (ActualPlotOffset * 2));
                    RenderedRect = new Rect(arrangeRect.Left + ActualPlotOffset, arrangeRect.Top, width, arrangeRect.Height);
                }
                else
                {
                    double height = Math.Max(0, arrangeRect.Height - (ActualPlotOffset * 2));
                    RenderedRect = new Rect(arrangeRect.Left, arrangeRect.Top + ActualPlotOffset, arrangeRect.Width, height);
                }
            }
        }

        private Rect renderedRect;
        internal Rect RenderedRect
        {
            get
            {
                return renderedRect;
            }
            set
            {
                Rect oldRect = renderedRect;
                renderedRect = value;
                OnAxisBoundsChanged(new ChartAxisBoundsEventArgs() { NewBounds = value, OldBounds = oldRect });
            }
        }

        protected internal virtual void OnAxisBoundsChanged(ChartAxisBoundsEventArgs args)
        {
            if (AxisBoundsChanged != null && args != null)
            {
                AxisBoundsChanged(this, args);
            }
        }

        protected internal virtual void OnAxisVisibleRangeChanged(VisibleRangeChangedEventArgs args)
        {
            if (VisibleRangeChanged != null && args != null)
                VisibleRangeChanged(this, args);
        }


        /// <summary>
        /// Gets or Sets property path which contains the position of a label in axis
        /// </summary>
        /// <remarks>
        /// When <see cref="LabelsSource"/> property is set, position path used to determine value of label's position.
        /// </remarks>
        /// <value>The position path.</value>
        /// <seealso cref="LabelsSource"/>
        [ClassReference(IsReviewed = false)]
        public string PositionPath
        {
            get { return (string)GetValue(PositionPathProperty); }
            set { SetValue(PositionPathProperty, value); }
        }

        /// <summary>
        /// Gets or Sets property path to retrieve label content from LabelsSource
        /// </summary>
        /// <remarks>
        /// When <see cref="LabelsSource"/> property is set, content path used to determine content for label.
        /// </remarks>
        [ClassReference(IsReviewed = false)]
        public string ContentPath
        {
            get { return (string)GetValue(ContentPathProperty); }
            set { SetValue(ContentPathProperty, value); }
        }

        /// <summary>
        /// Gets or Sets label format.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public string LabelFormat
        {
            get { return (string)GetValue(LabelFormatProperty); }
            set { SetValue(LabelFormatProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for LabelFormat.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LabelFormatProperty =
            DependencyProperty.Register("LabelFormat", typeof(string), typeof(ChartAxis), new PropertyMetadata(string.Empty,OnPropertyChanged));


        /// <summary>
        /// Gets or Sets collection of objects to look for the label content at particular position in axis.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public object LabelsSource
        {
            get { return (object)GetValue(LabelsSourceProperty); }
            set { SetValue(LabelsSourceProperty, value); }
        }


        /// <summary>
        /// Gets or Sets the postfix label template
        /// </summary>
       [ClassReference(IsReviewed = false)]
        public DataTemplate PostfixLabelTemplate
        {
            get { return (DataTemplate)GetValue(PostfixLabelTemplateProperty); }
            set { SetValue(PostfixLabelTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or Sets the prefix label template
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public DataTemplate PrefixLabelTemplate
        {
            get { return (DataTemplate)GetValue(PrefixLabelTemplateProperty); }
            set { SetValue(PrefixLabelTemplateProperty, value); }
        }

         /// <summary>
        /// Using a DependencyProperty as the backing store for PrefixLabelTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PrefixLabelTemplateProperty =
            DependencyProperty.Register("PrefixLabelTemplate", typeof(DataTemplate), typeof(ChartAxis), new PropertyMetadata(null, new PropertyChangedCallback(OnLabelTemplateChanged)));

       /// <summary>
        /// Using a DependencyProperty as the backing store for PostfixLabeltemplate.  This enables animation, styling, binding, etc...
       /// </summary>
        public static readonly DependencyProperty PostfixLabelTemplateProperty =
            DependencyProperty.Register("PostfixLabelTemplate", typeof(DataTemplate), typeof(ChartAxis), new PropertyMetadata(null, new PropertyChangedCallback(OnLabelTemplateChanged)));
         ///<summary>
        /// Identifies the LabelsSource dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelsSourceProperty =
          DependencyProperty.Register("LabelsSource", typeof(object), typeof(ChartAxis), new PropertyMetadata(null));

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
        /// Gets or Sets a value from which the axis elements like label, ticklines will be drawn inside axis bounds.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double PlotOffset
        {
            get { return (double)GetValue(PlotOffsetProperty); }
            set { SetValue(PlotOffsetProperty, value); }
        }

        
        /// <summary>
        ///  Using a DependencyProperty as the backing store for LableOffset.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PlotOffsetProperty =
            DependencyProperty.Register("PlotOffset", typeof(double), typeof(ChartAxis), new PropertyMetadata(0d,OnPropertyChanged));

        /// <summary>
        /// Gets or Sets a value from which the axis line will be drawn inside axis bounds
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double AxisLineOffset
        {
            get { return (double)GetValue(AxisLineOffsetProperty); }
            set { SetValue(AxisLineOffsetProperty, value); }
        }

        
        /// <summary>
        ///  Using a DependencyProperty as the backing store for AxisLineOffset.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AxisLineOffsetProperty =
            DependencyProperty.Register("AxisLineOffset", typeof(double), typeof(ChartAxis), new PropertyMetadata(0d));

        /// <summary>
        /// Gets or Sets LabelsPosition
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public AxisElementPosition LabelsPosition
        {
            get { return (AxisElementPosition)GetValue(LabelsPositionProperty); }
            set { SetValue(LabelsPositionProperty, value); }
        }

       
        /// <summary>
        ///  Using a DependencyProperty as the backing store for LabelsPosition.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LabelsPositionProperty =
            DependencyProperty.Register("LabelsPosition", typeof(AxisElementPosition), typeof(ChartAxis), new PropertyMetadata(AxisElementPosition.Outside,OnPropertyChanged));

        /// <summary>
        /// Gets or Sets the actions to be taken when two labels intersects in bounds
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public AxisLabelsIntersectAction LabelsIntersectAction
        {
            get { return (AxisLabelsIntersectAction)GetValue(LabelsIntersectActionProperty); }
            set { SetValue(LabelsIntersectActionProperty, value); }
        }


        
        /// <summary>
        ///  Using a DependencyProperty as the backing store for LabelsIntersectAction.
        /// </summary>
        public static readonly DependencyProperty LabelsIntersectActionProperty =
            DependencyProperty.Register("LabelsIntersectAction", typeof(AxisLabelsIntersectAction), typeof(ChartAxis), new PropertyMetadata(AxisLabelsIntersectAction.Hide,OnPropertyChanged));

        /// <summary>
        /// Gets or Sets  the distance between the label header from the ChartAxis.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double LabelExtent
        {
            get { return (double)GetValue(LabelExtentProperty); }
            set { SetValue(LabelExtentProperty, value); }
        }

       
        /// <summary>
        ///  Using a DependencyProperty as the backing store for LabelExtent.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LabelExtentProperty =
            DependencyProperty.Register("LabelExtent", typeof(double), typeof(ChartAxis), new PropertyMetadata(0.0,OnPropertyChanged));

        /// <summary>
        /// Gets or Sets label rotation angle.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double LabelRotationAngle
        {
            get { return (double)GetValue(LableRotationAngleProperty); }
            set { SetValue(LableRotationAngleProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for LableRotationAngle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LableRotationAngleProperty =
            DependencyProperty.Register("LableRotationAngle", typeof(double), typeof(ChartAxis), new PropertyMetadata(0d, new PropertyChangedCallback(OnLabelRotationChanged)));

        /// <summary>
        /// Get or Set AxislineStyle property
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Style AxisLineStyle
        {
            get { return (Style)GetValue(AxisLineStyleProperty); }
            set { SetValue(AxisLineStyleProperty, value); }
        }

        
        /// <summary>
        ///  Using a DependencyProperty as the backing store for AxisLineStyle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AxisLineStyleProperty =
            DependencyProperty.Register("AxisLineStyle", typeof(Style), typeof(ChartAxis), null);

        [ClassReference(IsReviewed = false)]
        internal Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        /// </summary>
        internal static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ChartAxis), new PropertyMetadata(Orientation.Horizontal, OnPropertyChanged));

        /// <summary>
        /// Gets or Sets a value that determines whether to draw axis at the opposite side of Chart
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public bool OpposedPosition
        {
            get { return (bool)GetValue(OpposedPositionProperty); }
            set { SetValue(OpposedPositionProperty, value); }
        }

        
        /// <summary>
        ///  Using a DependencyProperty as the backing store for OpposedPosition.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty OpposedPositionProperty =
            DependencyProperty.Register("OpposedPosition", typeof(bool), typeof(ChartAxis), new PropertyMetadata(false, OnOpposedPositionChanged));

        private static void OnOpposedPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var axis = d as ChartAxisBase2D;
            if (axis != null)
            {
#if NETFX_CORE || SILVERLIGHT_UNCOMMON || WPF
#if NETFX_CORE
            //For IR 17848 -  	Designer crashes when rebuild the SfChart
            if (!DesignMode.DesignModeEnabled)
#endif
                axis.ChangeStyle(axis.EnableTouchMode);
#endif
            }
            (d as ChartAxis).OnPropertyChanged();
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartAxis).OnPropertyChanged();
        }

        protected virtual void OnPropertyChanged()
        {
            if (this.Area != null)
            {
                this.Area.ScheduleUpdate();
            }
        }


        /// <summary>
        /// Gets or sets the type of the value that chart axis displays.
        /// </summary>       
        [ClassReference(IsReviewed = false)]
        internal ChartValueType ValueType
        {
            get { return (ChartValueType)GetValue(ValueTypeProperty); }
            set { SetValue(ValueTypeProperty, value); }
        }

        
       /// <summary>
        /// Using a DependencyProperty as the backing store for ValueType.  This enables animation, styling, binding, etc...
       /// </summary>
        internal static readonly DependencyProperty ValueTypeProperty =
            DependencyProperty.Register("ValueType", typeof(ChartValueType), typeof(ChartAxis), new PropertyMetadata(ChartValueType.Double, OnValuetypeChanged));

#if NETFX_CORE
        /// <summary>
        ///Gets or Sets approximate number of labels to be displayed in axis
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public object DesiredIntervalsCount
        {
            get { return (object)GetValue(DesiredIntervalsCountProperty); }
            set { SetValue(DesiredIntervalsCountProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for DesiredIntervalsCount.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DesiredIntervalsCountProperty =
            DependencyProperty.Register("DesiredIntervalsCount", typeof(object), typeof(ChartAxis), new PropertyMetadata(null, OnDesiredIntervalsCountPropertyChanged));
#else
        /// <summary>
        ///Gets or Sets approximate number of labels to be displayed in axis
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public int? DesiredIntervalsCount
        {
            get { return (int?)GetValue(DesiredIntervalsCountProperty); }
            set { SetValue(DesiredIntervalsCountProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for DesiredIntervalsCount.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DesiredIntervalsCountProperty =
            DependencyProperty.Register("DesiredIntervalsCount", typeof(int?), typeof(ChartAxis), new PropertyMetadata(null, OnDesiredIntervalsCountPropertyChanged));
#endif
#if NETFX_CORE || WPF || SILVERLIGHT_UNCOMMON
        /// <summary>
        /// Gets or Sets visibility of label.
        /// </summary>
        public Visibility ThumbLabelVisibility
        {
            get { return (Visibility)GetValue(ThumbLabelVisibilityProperty); }
            set { SetValue(ThumbLabelVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableThumbLabel. 
        public static readonly DependencyProperty ThumbLabelVisibilityProperty =
            DependencyProperty.Register("ThumbLabelVisibility", typeof(Visibility), typeof(ChartAxis), new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Gets or Sets template for label.
        /// </summary>
        public DataTemplate ThumbLabelTemplate
        {
            get { return (DataTemplate)GetValue(ThumbLabelTemplateProperty); }
            set { SetValue(ThumbLabelTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Template. 
        public static readonly DependencyProperty ThumbLabelTemplateProperty =
            DependencyProperty.Register("ThumbLabelTemplate", typeof(DataTemplate), typeof(ChartAxis), new PropertyMetadata(null));
#endif
        /// <summary>
        /// Gets or Sets axis header
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(ChartAxis), new PropertyMetadata(null, OnPropertyChanged));

        /// <summary>
        /// Gets or Sets DataTemplate for header.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

      
        /// <summary>
        ///  Using a DependencyProperty as the backing store for HeaderTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(ChartAxis), new PropertyMetadata(null, OnPropertyChanged));


     
        /// <summary>
        /// Gets or Sets size of tickline.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double TickLineSize
        {
            get { return (double)GetValue(TickLineSizeProperty); }
            set { SetValue(TickLineSizeProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for TickSize.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TickLineSizeProperty =
            DependencyProperty.Register("TickLineSize", typeof(double), typeof(ChartAxis), new PropertyMetadata(6d,OnPropertyChanged));

        internal ObservableCollection<ChartAxisLabel> m_VisibleLabels;

        /// <summary>
        /// Gets the collection of visible labels.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public ObservableCollection<ChartAxisLabel> VisibleLabels
        {
            get { return m_VisibleLabels; }

        }

        /// <summary>
        /// Gets or sets a value indicating whether the axis should be reversed. 
        /// When reversed, the axis will render points from right to left if horizontal, top to bottom when vertical and clockwise if radial.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public bool IsInversed
        {
            get { return (bool)GetValue(IsInversedProperty); }
            set { SetValue(IsInversedProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for IsInversed.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsInversedProperty =
            DependencyProperty.Register("IsInversed", typeof(bool), typeof(ChartAxis), new PropertyMetadata(false, OnIsInversedChanged));

        private static void OnIsInversedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartAxis).OnIsInversedChanged(e);
        }

        private void OnIsInversedChanged(DependencyPropertyChangedEventArgs e)
        {
            isInversed = this.IsInversed;
            if (this.Area != null)
            {
                this.Area.ScheduleUpdate();
            }
        }

        /// <summary>
        /// Gets or Sets origin value.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double Origin
        {
            get { return (double)GetValue(OriginProperty); }
            set { SetValue(OriginProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for Origin.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty OriginProperty =
            DependencyProperty.Register("Origin", typeof(double), typeof(ChartAxis), new PropertyMetadata(0d, new PropertyChangedCallback(OnPropertyChanged)));

        /// <summary>
        /// Get or Set ShowOrigin property
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public bool ShowOrigin
        {
            get { return (bool)GetValue(ShowOriginProperty); }
            set { SetValue(ShowOriginProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowOrigin.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowOriginProperty =
            DependencyProperty.Register("ShowOrigin", typeof(bool), typeof(ChartAxis), new PropertyMetadata(false));

        /// <summary>
        /// Gets or Sets tick lines position.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public AxisElementPosition TickLinesPosition
        {
            get { return (AxisElementPosition)GetValue(TickLinesPositionProperty); }
            set { SetValue(TickLinesPositionProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for TickLinesPosition.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TickLinesPositionProperty =
            DependencyProperty.Register("TickLinesPosition", typeof(AxisElementPosition), typeof(ChartAxis), new PropertyMetadata(AxisElementPosition.Outside,OnPropertyChanged));


        /// <summary>
        /// Get or Set ShowAxisNextToOriginProperty
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public bool ShowAxisNextToOrigin
        {
            get { return (bool)GetValue(ShowAxisNextToOriginProperty); }
            set { SetValue(ShowAxisNextToOriginProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowAxisNextToOrigin.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowAxisNextToOriginProperty =
            DependencyProperty.Register("ShowAxisNextToOrigin", typeof(bool), typeof(ChartAxis), new PropertyMetadata(false, new PropertyChangedCallback(OnShowAxisNextToOriginChanged)));

        /// <summary>
        /// Get or Set EdgeLabelDrawingMode
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public EdgeLabelsDrawingMode EdgeLabelsDrawingMode
        {
            get { return (EdgeLabelsDrawingMode)GetValue(EdgeLabelsDrawingModeProperty); }
            set { SetValue(EdgeLabelsDrawingModeProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for EdgeLabelsDrawingMode.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EdgeLabelsDrawingModeProperty =
            DependencyProperty.Register("EdgeLabelsDrawingMode", typeof(EdgeLabelsDrawingMode), typeof(ChartAxis), new PropertyMetadata(EdgeLabelsDrawingMode.Center, OnPropertyChanged));

        /// <summary>
        /// Gets or Sets major grid line style.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Style MajorGridLineStyle
        {
            get { return (Style)GetValue(MajorGridLineStyleProperty); }
            set { SetValue(MajorGridLineStyleProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for MajorGridLineStyle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MajorGridLineStyleProperty =
            DependencyProperty.Register("MajorGridLineStyle", typeof(Style), typeof(ChartAxis), null);

        /// <summary>
        /// Gets or Sets minor grid line style
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Style MinorGridLineStyle
        {
            get { return (Style)GetValue(MinorGridLineStyleProperty); }
            set { SetValue(MinorGridLineStyleProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for MinorGridLineStyle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinorGridLineStyleProperty =
            DependencyProperty.Register("MinorGridLineStyle", typeof(Style), typeof(ChartAxis), new PropertyMetadata(null, OnPropertyChanged));

        /// <summary>
        /// Gets or Sets major tick line style.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Style MajorTickLineStyle
        {
            get { return (Style)GetValue(MajorTickLineStyleProperty); }
            set { SetValue(MajorTickLineStyleProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for MajorTickLineStyle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MajorTickLineStyleProperty =
            DependencyProperty.Register("MajorTickLineStyle", typeof(Style), typeof(ChartAxis), new PropertyMetadata(null,OnPropertyChanged));


        /// <summary>
        /// Gets or Sets minor tick line style.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Style MinorTickLineStyle
        {
            get { return (Style)GetValue(MinorTickLineStyleProperty); }
            set { SetValue(MinorTickLineStyleProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for MinorTickLineStyle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinorTickLineStyleProperty =
            DependencyProperty.Register("MinorTickLineStyle", typeof(Style), typeof(ChartAxis), null);

        /// <summary>
        /// Gets or Sets a value that indicates whether to show track ball label for this axis.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public bool ShowTrackBallInfo
        {
            get { return (bool)GetValue(ShowTrackBallInfoProperty); }
            set { SetValue(ShowTrackBallInfoProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowTrackBallInfo.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowTrackBallInfoProperty =
            DependencyProperty.Register("ShowTrackBallInfo", typeof(bool), typeof(ChartAxis), new PropertyMetadata(false));

        /// <summary>
        /// Gets or Sets DataTemplate for track ball label.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public DataTemplate TrackBallLabelTemplate
        {
            get { return (DataTemplate)GetValue(TrackBallLabelTemplateProperty); }
            set { SetValue(TrackBallLabelTemplateProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for TrackBallLabelTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TrackBallLabelTemplateProperty =
            DependencyProperty.Register("TrackBallLabelTemplate", typeof(DataTemplate), typeof(ChartAxis), null);

        /// <summary>
        /// Gets or Sets a value that indicates whether to show grid lines
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public bool ShowGridLines
        {
            get { return (bool)GetValue(ShowGridLinesProperty); }
            set { SetValue(ShowGridLinesProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowGridLines.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowGridLinesProperty =
            DependencyProperty.Register("ShowGridLines", typeof(bool), typeof(ChartAxis), new PropertyMetadata(true, new PropertyChangedCallback(OnShowGridLinePropertyChanged)));

        private static void OnShowGridLinePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartAxis).OnShowGridLines((bool)e.NewValue);
        }

        private void OnShowGridLines(bool value)
        {
            if (Area != null &&
                 Area.GridLinesLayout != null)
            {
                if (!value && GridLinesRecycler != null)
                {
                    GridLinesRecycler.Clear();
                }
                else if (value && VisibleLabels.Count > 0)
                {
                    if (!double.IsInfinity(Area.AvailableSize.Height) && !double.IsInfinity(Area.AvailableSize.Width))
                    {
                        Area.GridLinesLayout.UpdateElements();
                        Area.GridLinesLayout.Measure(Area.AvailableSize);
                        Area.GridLinesLayout.Arrange(Area.AvailableSize);
                    }
                }
                Area.UpdateAxisLayoutPanels();
                if (Area is SfChart)
                    (Area as SfChart).AddOrRemoveBitmap();
                Area.ScheduleUpdate();
            }

        }

        internal Visibility AxisVisibility
        {
            get { return (Visibility)GetValue(AxisVisibilityProperty); }
            set { SetValue(AxisVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AxisVisibility.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty AxisVisibilityProperty =
            DependencyProperty.Register("AxisVisibility", typeof(Visibility), typeof(ChartAxis), new PropertyMetadata(Visibility.Visible,OnPropertyChanged));

        /// <summary>
        /// Get or Set EnableAutoIntervalOnZoomingProperty
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public bool EnableAutoIntervalOnZooming
        {
            get { return (bool)GetValue(EnableAutoIntervalOnZoomingProperty); }
            set { SetValue(EnableAutoIntervalOnZoomingProperty, value); }
        }

        /// <summary>
        /// Gets or Sets DataTemplate for axis label.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public DataTemplate LabelTemplate
        {
            get { return (DataTemplate)GetValue(LabelTemplateProperty); }
            set { SetValue(LabelTemplateProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for LabelTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LabelTemplateProperty =
            DependencyProperty.Register("LabelTemplate", typeof(DataTemplate), typeof(ChartAxis), new PropertyMetadata(null, new PropertyChangedCallback(OnLabelTemplateChanged)));

        /// <summary>
        /// Using a DependencyProperty as the backing store for EnableAutoIntervalOnZooming.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EnableAutoIntervalOnZoomingProperty =
            DependencyProperty.Register("EnableAutoIntervalOnZooming", typeof(bool), typeof(ChartAxis), new PropertyMetadata(true));

        internal bool smallTicksRequired = false;

        private ChartAxisLabelCollection m_customLabels;

        /// <summary>
        /// Get CLR property as Customlabels
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public ChartAxisLabelCollection CustomLabels
        {
            get
            {
                if (m_customLabels == null)
                {
                    m_customLabels = new ChartAxisLabelCollection();
                    m_customLabels.CollectionChanged += CustomLables_CollectionChanged;
                }
                return m_customLabels;
            }
        }

        private ObservableCollection<ISupportAxes> registeredSeries;

        internal ObservableCollection<ISupportAxes> RegisteredSeries
        {
            get
            {
                if (registeredSeries == null)
                {
                    registeredSeries = new ObservableCollection<ISupportAxes>();
                    associatedAxes = new List<ChartAxis>();
                    registeredSeries.CollectionChanged += OnRegisteredSeriesCollectionChanged;
                }
                return registeredSeries;
            }
        }

        private List<ChartAxis> associatedAxes;

        internal List<ChartAxis> AssociatedAxes
        {
            get { return associatedAxes; }  
            set { associatedAxes = value; }
        }
        

        #endregion

        #region ctor

        /// <summary>
        /// Called when instance created for ChartAxis
        /// </summary>
        public ChartAxis()
        {
            ValueToCoefficientCalc = ValueToCoefficient;
            CoefficientToValueCalc = CoefficientToValue;
            DefaultStyleKey = typeof(ChartAxis);
            GridLines = new List<Line>();
            MinorGridLines = new List<Line>();
            m_VisibleLabels = new ObservableCollection<ChartAxisLabel>();
            Binding visibilityBinding = new Binding();
            visibilityBinding.Source = this;
            visibilityBinding.Path = new PropertyPath("Visibility");
            BindingOperations.SetBinding(this, AxisVisibilityProperty, visibilityBinding);
        }

        protected virtual void OnRegisteredSeriesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            ISupportAxes series = null;
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                series = e.NewItems[0] as ISupportAxes;
                if (series == null) return;
                ChartAxis yAxis = null;
                if (series is ISupportAxes2D)
                    yAxis = (series as ISupportAxes2D).YAxis;
                else
                    yAxis = (series as ISupportAxes3D).YAxis;
                ChartAxis xAxis = null;
                if (series is ISupportAxes2D)
                    xAxis = (series as ISupportAxes2D).XAxis;
                else
                    xAxis = (series as ISupportAxes3D).XAxis;
                if (xAxis != null && yAxis != null && xAxis.associatedAxes != null && yAxis.associatedAxes != null)
                {
                    if (!(xAxis.associatedAxes.Contains(yAxis)))
                    {
                        xAxis.associatedAxes.Add(yAxis);
                    }
                    if ((!yAxis.associatedAxes.Contains(xAxis)))
                    {
                        yAxis.associatedAxes.Add(xAxis);
                    }
                }
                ScheduleCheck();
            }
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                series = e.OldItems[0] as ISupportAxes;
                if (series == null) return;
                var yAxis = series.ActualYAxis;
                var xAxis = series.ActualXAxis;
                if (Area != null && xAxis != null && yAxis != null && xAxis.associatedAxes != null && yAxis.associatedAxes != null)
                {
                    if (!Area.Axes.Contains(yAxis))
                    {
                        xAxis.associatedAxes.Remove(yAxis);
                    }
                    if (!Area.Axes.Contains(xAxis))
                    {
                        yAxis.associatedAxes.Remove(xAxis);
                    }
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                if (associatedAxes != null)
                    associatedAxes.Clear();
            }
        }

        private void ScheduleCheck()
        {
            if (!isChecked)
            {
#if WINDOWS_PHONE
            
#if WPF 
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(CheckRegisterSeries));
#else
                Dispatcher.BeginInvoke(CheckRegisterSeries);
#endif
          
#else
                if (DesignMode.DesignModeEnabled)
                    CheckRegisterSeries();
                else
                    checkRegisterAction = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, CheckRegisterSeries);
#endif
                isChecked = true;
            }
        }

        private void CheckRegisterSeries()
        {
            foreach (ChartSeriesBase chartSeries in RegisteredSeries)
            {
                if ((RegisteredSeries[0] as ChartSeriesBase).IsActualTransposed != (chartSeries as ChartSeriesBase).IsActualTransposed)
                {
                    throw new InvalidOperationException("The Axis of " + RegisteredSeries[0].GetType() +
                                                    " is not compatible with the Axis of " +
                                                    chartSeries.GetType());
                }
            }
            
       }

        void CustomLables_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (this.VisibleRange.Inside(e.NewStartingIndex))
                    {
                        m_VisibleLabels.Insert(e.NewStartingIndex, e.NewItems[0] as ChartAxisLabel);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (ChartAxisLabel label in e.OldItems)
                    {
                        m_VisibleLabels.Remove(label);
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    break;

                default:
                    break;
            }
        }

        #endregion

        #region methods

        /// <summary>
        /// Converts co-ordinate of point related to chart control to axis units.
        /// </summary>
        /// <param name="value">The absolute point value.</param>
        /// <returns>The value of point on axis.</returns>
        /// <seealso cref="ChartAxis.ValueToCoefficientCalc(double)"/>
        [ClassReference(IsReviewed = false)]
        public virtual double CoefficientToValue(double value)
        {
            double result = double.NaN;

            value = this.IsInversed ? 1d - value : value;

            result = VisibleRange.Start + VisibleRange.Delta * value;

            return result;
        }

        /// <summary>
        /// Converts Coefficient of Value related to chart control to Polar/Radar type axis unit.
        /// </summary>
        /// <param name="value"> Polar/Radar type axis Coefficient Value</param>
        /// <returns> The value of point on Polar/Radar type axis</returns>
        [ClassReference(IsReviewed = false)]
        public virtual double PolarCoefficientToValue(double value)
        {
            double result = double.NaN;

            value = this.IsInversed ? 1d - value : value;

            value /= 1 - 1 / (VisibleRange.Delta + 1);

            result = VisibleRange.Start + VisibleRange.Delta * value;

            return result;
        }

        /// <summary>
        /// Converts co-ordinate of point related to chart control to axis units. It returns actual value instead of visible value.
        /// </summary>
        /// <param name="value">The absolute point value.</param>
        /// <returns>The value of point on axis.</returns>
        public virtual double CoefficientToActualValue(double value)
        {
            double result = double.NaN;

            value = this.IsInversed ? 1d - value : value;

            result = ActualRange.Start + ActualRange.Delta * value;

            return result;
        }

        internal double PixelToCoefficientValue(double value)
        {
            double actualSize = (Orientation == Orientation.Horizontal) ? renderedRect.Width : renderedRect.Height;
            return (value * (VisibleRange.End - VisibleRange.Start)) / actualSize;
        }

        /// <summary>
        /// Get or Set IsLogarithmic property
        /// </summary>
        public bool IsLogarithmic { get; internal set; }

       
        private static void OnShowAxisNextToOriginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        private static void OnLabelTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis.Area != null)
            {
               axis.Area.ScheduleUpdate();
            }
            
        }

        private static void OnLabelRotationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            axis.IsLabelRotateRequired = true;
            if (axis.Area != null)
                axis.Area.ScheduleUpdate();
        }

        internal bool IsLabelRotateRequired { get; set; }

        private static void OnValuetypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            ChartAxis axis = d as ChartAxis;
            if (axis.ValueType == ChartValueType.Logarithmic)
            {
                axis.IsLogarithmic = true;
            }

        }

        private static void OnDesiredIntervalsCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        internal virtual void CreateLineRecycler()
        {
           
        }

        /// <summary>
        /// Converts value of passed point co-ordinate to control related co-ordinate.
        /// </summary>
        /// <param name="value">The value of point on axis.</param>
        /// <returns>The value of point on axis.</returns>
        /// <seealso cref="ChartAxis.CoefficientToValueCalc"/>
        [ClassReference(IsReviewed = false)]
        public virtual double ValueToCoefficient(double value)
        {
            double result = double.NaN;

            var start = VisibleRange.Start;
            var delta = VisibleRange.Delta;

            result = (value - start) / delta;

            return this.isInversed ? 1d - result : result;
        }


        /// <summary>
        /// Converts co-ordinate of point related to chart control to Polar/Radar type axis unit.
        /// </summary>
        /// <param name="value">The absolute point value.</param>
        /// <returns>The value of point on axis.</returns>
        /// <seealso cref="ChartAxis.ValueToPolarCoefficient"/>
        [ClassReference(IsReviewed = false)]
        public virtual double ValueToPolarCoefficient(double value)
        {
            double result = double.NaN;

            var start = VisibleRange.Start;
            var delta = VisibleRange.Delta;

            result = (value - start) / delta;

            result *= 1 - 1 / (delta + 1);

            return this.isInversed ? 1d - result : result;
        }

        /// <summary>
        /// Converts value of passed point co-ordinate to control related co-ordinate.
        /// </summary>
        /// <param name="value">The value of point on axis.</param>
        /// <param name="isInversed">The value indicates whether <see cref="ChartAxis.IsInversed"/> is e/></param>
        /// <returns>Co-ordinate of point related to chart control.</returns>
        /// <seealso cref="ChartAxis.CoefficientToValueCalc"/>
       [ClassReference(IsReviewed = false)]
        public virtual double ValueToCoefficient(double value, bool isInversed)
        {
            double result = double.NaN;

            var start = VisibleRange.Start;
            var delta = VisibleRange.Delta;

            result = (value - start) / delta;
            return isInversed ? 1d - result : result;
        }

        /// <summary>
        /// Method implementation for Generate Labels in ChartAxis
        /// </summary>
        protected virtual void GenerateVisibleLabels()
        {
            double interval = VisibleInterval;
            double position = VisibleRange.Start - (VisibleRange.Start % ActualInterval);

            for (; position <= VisibleRange.End; position += interval)
            {
                if (VisibleRange.Inside(position))
                {
                    VisibleLabels.Add(new ChartAxisLabel(position, GetLabelContent(position), position));
                }
            }
        }

        /// <summary>
        /// Return Object value from the given position value
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public virtual object GetLabelContent(double position)
        {
            return Math.Round(position, CRoundDecimals).ToString(this.LabelFormat, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Calculates actual range and actual interval
        /// </summary>
        /// <param name="availableSize"></param>
        /// <summary>
        /// Calculates actual range and actual interval
        /// </summary>
        /// <param name="availableSize"></param>
        internal void CalculateRangeAndInterval(Size availableSize)
        {
            if (!IsScrolling || IsDataChanged)
            {
                DoubleRange range = CalculateActualRange();

                if (range.IsEmpty)
                    range = new DoubleRange(0, 1);

                if (range.Start == range.End)
                    range = new DoubleRange(range.Start, range.End + 1);

                ActualInterval = CalculateActualInterval(range, availableSize);

                ActualRange = ApplyRangePadding(range, ActualInterval);

                if (ActualRangeChanged != null)
                {
                    ApplyCustomVisibleRange(availableSize);
                }
                else
                {
                    CalculateVisibleRange(availableSize);
                }
                IsDataChanged = false;
            }
            else
            {
                CalculateVisibleRange(availableSize);
            }
        }

        private void ApplyCustomVisibleRange(Size availableSize)
        {
            var rangeChangeArgs = new ActualRangeChangedEventArgs(this) { IsScrolling = IsScrolling, ActualMinimum = ActualRange.Start, ActualMaximum = ActualRange.End };
            ActualRangeChanged(this, rangeChangeArgs);
        
            var customActualRange = rangeChangeArgs.GetActualRange();
            if (customActualRange != ActualRange)
            {
                ActualRange = customActualRange;
                ActualInterval = CalculateActualInterval(customActualRange, availableSize);
            }
        
            var visibleRange = rangeChangeArgs.GetVisibleRange();
            if (visibleRange.IsEmpty)
            {
                CalculateVisibleRange(availableSize);
            }
            else
            {
                VisibleRange = visibleRange;
                VisibleInterval = EnableAutoIntervalOnZooming
                                        ? CalculateNiceInterval(VisibleRange, availableSize)
                                        : ActualInterval;
                if (this is ChartAxisBase2D)
                {
                    (this as ChartAxisBase2D).ZoomPosition = (VisibleRange.Start - ActualRange.Start) / ActualRange.Delta;
                    (this as ChartAxisBase2D).ZoomFactor = (VisibleRange.End - VisibleRange.Start) / ActualRange.Delta;
                }
            }
        }

        /// <summary>
        /// Method implementation for Add SamllTicksPoint
        /// </summary>
        /// <param name="position"></param>
        internal protected virtual void AddSmallTicksPoint(double position)
        { }

        /// <summary>
        /// Method implementation for Add smallTicks to axis
        /// </summary>
        /// <param name="postion"></param>
        /// <param name="logarithmicbase"></param>
        internal protected virtual void AddSmallTicksPoint(double position, double interval)
        { }

        /// <summary>
        /// Calculates actual range
        /// </summary>
        /// <returns></returns>
        protected virtual DoubleRange CalculateActualRange()
        {
            if (Area != null)
            {
                var technicalIndicators = new List<ChartSeriesBase>();

                if (Area is SfChart)
                {
                    foreach (ChartSeries indicator in (Area as SfChart).TechnicalIndicators)
                    {
                        technicalIndicators.Add(indicator as ChartSeriesBase);
                    }
                }

                return (Area is SfChart ? Area.VisibleSeries.Union(technicalIndicators).OfType<ISupportAxes>() : 
                    Area.VisibleSeries.OfType<ISupportAxes>())
                    .Select
                    (
                        series =>
                        {
                            if (series.ActualXAxis == this)
                                return series.XRange;
                            if (series.ActualYAxis == this)
                                return series.YRange;
                            return DoubleRange.Empty;
                        }
                    ).Sum();
            }
            return DoubleRange.Empty;
        }

        /// <summary>
        /// Calculates actual interval
        /// </summary>
        /// <param name="range"></param>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        internal protected virtual double CalculateActualInterval(DoubleRange range, Size availableSize)
        {
            return 1.0;
        }

        /// <summary>
        /// Apply padding based on interval
        /// </summary>
        /// <param name="range"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        protected virtual DoubleRange ApplyRangePadding(DoubleRange range, double interval)
        {
            if (RegisteredSeries.Count > 0 && RegisteredSeries[0] is PolarRadarSeriesBase)
            {
                double minimum = Math.Floor(range.Start / interval) * interval;
                double maximum = Math.Ceiling(range.End / interval) * interval;
                return new DoubleRange(minimum, maximum);
            }

            return range;
        }

        /// <summary>
        /// Returns the maximum desired intervals count.
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        internal protected double GetActualDesiredIntervalsCount(Size availableSize)
        {
            double size = Orientation == Orientation.Horizontal
                ? availableSize.Width : availableSize.Height;

            double actualDesiredIntervalsCount = DesiredIntervalsCount != null
#if NETFX_CORE
 ? Convert.ToInt32(DesiredIntervalsCount)
#else
 ? DesiredIntervalsCount.Value
#endif
 : 0;

            if (DesiredIntervalsCount == null)
            {
                double adjustedDesiredIntervalsCount = (Orientation == Orientation.Horizontal ? 0.54 : 1.0) *
                                                              MaximumLabels;
                actualDesiredIntervalsCount = Math.Max(size * adjustedDesiredIntervalsCount / MaxPixelsCount, 1.0);
            }

            return actualDesiredIntervalsCount;
        }

        /// <summary>
        /// Calculates nice interval
        /// </summary>
        /// <param name="actualRange"></param>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected internal virtual double CalculateNiceInterval(DoubleRange actualRange, Size availableSize)
        {
            double delta = actualRange.Delta;

            double actualDesiredIntervalsCount = GetActualDesiredIntervalsCount(availableSize);

            double niceInterval = delta / actualDesiredIntervalsCount;

            double minInterval = Math.Pow(10, Math.Floor(Math.Log10(niceInterval)));

            foreach (int mul in c_intervalDivs)
            {
                double currentInterval = minInterval * mul;
                if (actualDesiredIntervalsCount < (delta / currentInterval))
                {
                    break;
                }

                niceInterval = currentInterval;
            }

            return niceInterval;
        }

        /// <summary>
        /// Recalculates visible range and visible labels.
        /// </summary>
        internal void Invalidate()
        {
            if (RegisteredSeries.Count > 0 
                && RegisteredSeries[0] is PolarRadarSeriesBase)
            {
                CalculateRangeAndInterval(AvailableSize);
                UpdateLabels();
            }
        }
        /// <summary>
        /// Provides the behavior for the Measure pass of Silverlight layout. Classes can override this method to define their own Measure pass behavior.
        /// </summary>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of the allocated sizes for child objects; or based on other considerations, such as a fixed container size.
        /// </returns>
        /// <param name="availableSize"></param>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (Area != null && Area.VisibleSeries.Count > 0 && Area.VisibleSeries[0] is AccumulationSeriesBase)
                return new Size(0, 0);

            base.MeasureOverride(availableSize);
           
            return new Size(ArrangeRect.Width, ArrangeRect.Height);
        }

        internal virtual void ComputeDesiredSize(Size size)
        {
           
        }

        internal void UpdateLabels()
        {
            if (VisibleRange.Delta > 0)
            {
                VisibleLabels.Clear();
                m_smalltickPoints.Clear();

                if (this.CustomLabels.Count > 0)
                {
                    PopulateVisibleLabelForCustomLabels();
                }
                else if (this.LabelsSource != null)
                {
                    PopulateVisibleLabelsForLabelSource();
                }
                else
                {
                    GenerateVisibleLabels();
                }

                if (axisLabelsPanel != null)
                {
                    if (axisElementsPanel != null)
                        axisElementsPanel.UpdateElements();
                    axisLabelsPanel.UpdateElements();
                }
                else
                {
                    axisElementsUpdateRequired = true;
                }
            }
        }

        private void PopulateVisibleLabelsForLabelSource()
        {
            object labelsSource = null;

#if WINDOWS_PHONE
            labelsSource = this.LabelsSource;
#else
            if (!(this.LabelsSource is XmlElement))
            {
                labelsSource = this.LabelsSource;
            }
            else
            {
                IList source = new List<IXmlNode>();
                CreateXmlSourceListWrapper((this.LabelsSource as XmlElement), ref source);
                labelsSource = source;
            }
#endif

            foreach (object obj in GetSourceList(labelsSource))
            {
                double position = (int)ChartDataUtils.GetPositionalPathValue(obj, this.PositionPath);
                if (this.VisibleRange.Inside(position))
                {
                    if (string.IsNullOrEmpty(this.ContentPath))
                    {
                        VisibleLabels.Add(new ChartAxisLabel(position, obj, position));
                    }
                    else
                    {
                        VisibleLabels.Add(new ChartAxisLabel(position, ChartDataUtils.GetObjectByPath(obj, this.ContentPath), position));
                    }
                }
            }
        }

        internal static IEnumerable GetSourceList(object source)
        {
            IEnumerable result = null;
            if (source != null)
            {
                if (source is CollectionViewSource)
                {
                    var cvs = source as CollectionViewSource;
                    if (cvs.View != null)
                    {
#if WINDOWS_PHONE
                        result = GetSourceList(cvs.View.Groups);
#else
                        result = GetSourceList(cvs.View.CollectionGroups);
#endif
                    }
                }
                else if (source is ICollectionView)
                {
#if WINDOWS_PHONE
                    var sourceList = ((ICollectionView)source).Groups;
#else
                    var sourceList = ((ICollectionView)source).CollectionGroups;
#endif
                    result = GetSourceList(sourceList);
                }
                else
                {
                    result = source as IEnumerable;
                }

            }

            return result;
        }

#if !WINDOWS_PHONE

        /// <summary>
        /// Iterates the XMLNodes to poulate the collection in a IList source
        /// </summary>
        /// <param name="itemsSource"></param>
        /// <param name="source"></param>
        private static void CreateXmlSourceListWrapper(IXmlNode itemsSource, ref IList source)
        {
            IXmlNode xmlData = itemsSource as IXmlNode;
            if (xmlData != null)
            {
                source.Add(xmlData);
                CreateXmlSourceListWrapper(xmlData.NextSibling, ref source);
            }
        }
#endif

        /// <summary>
        /// Sets the Custom Labels to Visible Labels Collection
        /// </summary>        
        private void PopulateVisibleLabelForCustomLabels()
        {
            GenerateVisibleLabels();

            var visibleLabelsCollection = VisibleLabels.ToDictionary(label => label.Position);

            VisibleLabels.Clear();

            foreach (ChartAxisLabel label in m_customLabels
                .Where(label => this.VisibleRange.Inside(label.Position)))
            {
                visibleLabelsCollection[label.Position] = new ChartAxisLabel()
                {
                    Position = label.Position,
                    LabelContent = label.LabelContent,
                    ActualValue = label.Position
                };
            }

            foreach (var label in visibleLabelsCollection.Values)
            {
                VisibleLabels.Add(label);
            }
        }

        /// <summary>
        /// Gets the actual rect co-ordinates of an ChartAxis.
        /// </summary>
        /// <returns>returns rect</returns>
        [ClassReference(IsReviewed = false)]
        public Rect GetRenderedRect()
        {
            return RenderedRect;
        }

        /// <summary>
        /// Gets the rect co-ordinates of an axis excluding its value of LabelOffset and AxisLineOffset.
        /// </summary>
        /// <returns>returns rect</returns>
        [ClassReference(IsReviewed = false)]
        public Rect GetArrangeRect()
        {
            return ArrangeRect;
        }

        /// <summary>
        /// Calculates the visible range.
        /// </summary>
        protected internal virtual void CalculateVisibleRange(Size availableSize)
        {
            VisibleRange = ActualRange;
            VisibleInterval = ActualInterval;
        }

        protected virtual DependencyObject CloneAxis(DependencyObject obj)
        {
            ChartAxis newAxis = obj as ChartAxis;
            ChartCloning.CloneControl(this, newAxis);
            SfChart.SetRow(newAxis, SfChart.GetRow(this));
            SfChart.SetColumn(newAxis, SfChart.GetRow(this));
            newAxis.ContentPath = this.ContentPath;
            newAxis.Header = this.Header;
            newAxis.HeaderTemplate = this.HeaderTemplate;
            newAxis.IsInversed = this.IsInversed;
            newAxis.LabelExtent = this.LabelExtent;
            newAxis.LabelRotationAngle = this.LabelRotationAngle;
            newAxis.LabelsIntersectAction = this.LabelsIntersectAction;
            newAxis.LabelsPosition = this.LabelsPosition;
            newAxis.LabelsSource = this.LabelsSource;
            newAxis.LabelTemplate = this.LabelTemplate;
            newAxis.MajorGridLineStyle = this.MajorGridLineStyle;
            newAxis.MajorTickLineStyle = this.MajorTickLineStyle;
            newAxis.MinorGridLineStyle = this.MinorGridLineStyle;
            newAxis.MinorTickLineStyle = this.MinorTickLineStyle;
            newAxis.OpposedPosition = this.OpposedPosition;
            newAxis.PositionPath = this.PositionPath;
            newAxis.PostfixLabelTemplate = this.PostfixLabelTemplate;
            newAxis.PrefixLabelTemplate = this.PrefixLabelTemplate;
            newAxis.ShowAxisNextToOrigin = this.ShowAxisNextToOrigin;
            newAxis.ShowGridLines = this.ShowGridLines;
            newAxis.ShowOrigin = this.ShowOrigin;
            newAxis.TickLineSize = this.TickLineSize;
            newAxis.EnableAutoIntervalOnZooming = this.EnableAutoIntervalOnZooming;
            newAxis.ShowTrackBallInfo = this.ShowTrackBallInfo;
            newAxis.TrackBallLabelTemplate = this.TrackBallLabelTemplate;
            newAxis.EdgeLabelsDrawingMode = this.EdgeLabelsDrawingMode;
            newAxis.TickLinesPosition = this.TickLinesPosition;
            newAxis.Origin = this.Origin;
            newAxis.DesiredIntervalsCount = this.DesiredIntervalsCount;
            newAxis.Orientation = this.Orientation;
            newAxis.AxisLineStyle = this.AxisLineStyle;
            newAxis.AxisLineOffset = this.AxisLineOffset;
            return newAxis;
        }

        public DependencyObject Clone()
        {
            return CloneAxis(null);
        }

        #endregion
    }

    /// <summary>
    /// Represents an axis label element.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class ChartAxisLabel
    {
        #region properties

        /// <summary>
        /// Get or Set LabelContent property
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public object LabelContent { get; set; }

        /// <summary>
        /// Get or Set Position property
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double Position { get; set; }

        [ClassReference(IsReviewed = false)]
        internal double ActualValue { get; set; }

        /// <summary>
        /// Get or Set PrefixLabelTemplate
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public DataTemplate PrefixLabelTemplate
        {
            get;
            set;
        }

        /// <summary>
        /// Get or Set PostfixLabelTemplate property
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public DataTemplate PostfixLabelTemplate
        {
            get;
            set;
        }

        #endregion

        #region ctor

        /// <summary>
        /// Constructor
        /// </summary>
        public ChartAxisLabel()
        {

        }

        /// <summary>
        /// Called when instance created for ChartAxisLabel with following arguments
        /// </summary>
        /// <param name="position"></param>
        /// <param name="labelContent"></param>
        /// <param name="actualValue"></param>
        public ChartAxisLabel(double position, object labelContent, double actualValue)
        {
            this.Position = position;
            this.LabelContent = labelContent;
            this.ActualValue = actualValue;
        }

        /// <summary>
        /// Called when instance created for ChartAxisLabel with following arguments
        /// </summary>
        /// <param name="position"></param>
        /// <param name="labelContent"></param>
        public ChartAxisLabel(double position, object labelContent)
        {
            this.Position = position;
            this.LabelContent = labelContent;
        }

        #endregion
    }

    /// <summary>
    /// Represents chart series bounds changed event arguments.
    /// </summary>
    ///<remarks>
    /// It contains information like old bounds and new bounds.
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public class ChartAxisBoundsEventArgs : EventArgs
    {
        public Rect NewBounds { get; set; }
        public Rect OldBounds { get; set; }
    }

    /// <summary>
    /// Represents chart series bounds changed event arguments.
    /// </summary>
    ///<remarks>
    /// It contains information like old bounds and new bounds.
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public class VisibleRangeChangedEventArgs : EventArgs
    {
        public DoubleRange NewRange { get; set; }
        public DoubleRange OldRange { get; set; }
    }

    /// <summary>
    /// Represents chart axis actual range changed event arguments.
    /// </summary>
    public class ActualRangeChangedEventArgs : EventArgs
    {

        private readonly ChartAxis axis;

        private object GetConvertedValue(object actualValue)
        {
            if (!(actualValue is double)) return actualValue;
            if (axis is DateTimeAxis)
            {
                return ((double)actualValue).FromOADate();
            }
            else if (axis is TimeSpanAxis)
            {
                return TimeSpan.FromMilliseconds((double)actualValue);
            }
            else if (axis is LogarithmicAxis)
            {
                return Math.Pow((axis as LogarithmicAxis).LogarithmicBase, (double)actualValue);
            }
            else
                return actualValue;
        }

        private double ToDouble(object actualValue)
        {
            if (actualValue is DateTime)
            {
                return ((DateTime)actualValue).ToOADate();
            }
            else if (actualValue is TimeSpan)
            {
                return ((TimeSpan)actualValue).TotalMilliseconds;
            }
            else
            {
                return Convert.ToDouble(actualValue);
            }

        }

        internal DoubleRange GetVisibleRange()
        {
            double start, end;

            if (VisibleMinimum == null && VisibleMaximum == null)
                return DoubleRange.Empty;

            if (VisibleMaximum == null)
            {
                end = ToDouble(ActualMaximum);
                start = ToDouble(VisibleMinimum);
            }
            else if (VisibleMinimum == null)
            {
                end = ToDouble(VisibleMaximum);
                start = ToDouble(ActualMinimum);
            }
            else
            {
                end = ToDouble(VisibleMaximum);
                start = ToDouble(VisibleMinimum);
            }
            var actualRange = GetActualRange();
            if (start < actualRange.Start)
                start = actualRange.Start;
            if (end > actualRange.End)
                end = actualRange.End;
            if (start == end)
                end += 1;
            return new DoubleRange(start, end);
        }

        internal DoubleRange GetActualRange()
        {
            return new DoubleRange(ToDouble(ActualMinimum), ToDouble(ActualMaximum));
        }

        public bool IsScrolling { get; set; }

        object actualMinimum;
        /// <summary>
        /// Gets the actual minimum.
        /// </summary>
        /// <value>
        /// The actual minimum.
        /// </value>
        public object ActualMinimum
        {
            get { return actualMinimum; }
            set { actualMinimum = GetConvertedValue(value); }
        }

        object actualMaximum;
        /// <summary>
        /// Gets the actual maximum.
        /// </summary>
        /// <value>
        /// The maximum.
        /// </value>
        public object ActualMaximum
        {
            get { return actualMaximum; }
            set { actualMaximum = GetConvertedValue(value); }
        }

        /// <summary>
        /// Gets or sets the visible minimum.
        /// </summary>
        /// <value>
        /// The minimum.
        /// </value>
        public object VisibleMinimum { get; set; }

        /// <summary>
        /// Gets or sets the maximum.
        /// </summary>
        /// <value>
        /// The maximum.
        /// </value>
        public object VisibleMaximum { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActualRangeChangedEventArgs"/> class.
        /// </summary>
        /// <param name="axis">The axis.</param>
        public ActualRangeChangedEventArgs(ChartAxis axis)
        {
            this.axis = axis;
        }
    }
}
