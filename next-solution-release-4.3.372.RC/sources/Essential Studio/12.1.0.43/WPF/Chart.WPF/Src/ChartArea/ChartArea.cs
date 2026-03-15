// <copyright file="ChartArea.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System.Globalization;

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.ComponentModel;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using System.Windows.Shapes;
    using Syncfusion.Licensing;
    using System.Linq;
    using System.Xml.Serialization;
    using System.Text;
    using System.Xml;
    using System.Windows.Threading;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation;
    /// <summary>
    /// Class represents chart area.
    /// </summary>
    /// <remarks>
    /// As per WPF Chart logic, area is a child of chart control and it's the part where
    /// series are drawn. <para> Single chart control instance may contain multiple
    /// chart areas assigned to its <see cref="Chart.Areas" /> collection. </para>
    /// <para> Layout of areas on chart control provides <see cref="ChartPanel" />
    /// panel. </para>
    /// </remarks>
    /// <example>
    /// Example demonstrates creation of area on chart control. <para> C#: </para> <code
    /// language="C#">
    /// //Creating new chart control instance.
    /// Chart chart = new Chart();
    /// //Creating chart area instance.
    /// ChartArea chartArea = new ChartArea();
    /// //Creating chart series instance.
    /// ChartSeries series = new ChartSeries();
    /// //Creating chart list data instance.
    /// ChartListData data = new ChartListData();
    /// //Filling data with points.
    /// data.Add(new ChartPoint(1, 1));
    /// data.Add(new ChartPoint(2, 2));
    /// data.Add(new ChartPoint(3, 3));
    /// data.Add(new ChartPoint(4, 4));
    /// //Assigning data points collection.
    /// series.Data = data;
    /// //Adding series to area.
    /// chartArea.Series.Add(series);
    /// //Adding area to chart.
    /// chart.Areas.Add(chartArea);
    /// </code> XAML: <code language="XAML">
    /// &lt;syncfusion:Chart
    /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
    /// &lt;syncfusion:Chart.Areas&gt;
    /// &lt;!--Adding char area to chart areas collection--&gt;
    /// &lt;syncfusion:ChartArea&gt;
    /// &lt;!--Adding chart series to area--&gt;
    /// &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3 4 4"/&gt;
    /// &lt;/syncfusion:ChartArea&gt;
    /// &lt;/syncfusion:Chart.Areas&gt;
    /// &lt;/syncfusion:Chart&gt;
    /// </code>
    /// </example>
    /// <seealso cref="Chart">Chart class specification</seealso>
    /// <seealso cref="ChartArea">ChartArea class specification</seealso>
    /// <seealso cref="ChartPoint">ChartPoint class specification</seealso>
    /// <seealso cref="ChartTypes">ChartTypes enumeration</seealso>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [StyleTypedProperty(Property = "SeriesStyle", StyleTargetType = typeof(ChartSeries))]
    [StyleTypedProperty(Property = "LegendStyle", StyleTargetType = typeof(ChartLegend))]
    [ContentProperty("Series")]
    public class ChartArea : Control, IDisposable, IChartSerializer
    {
        private class ChartAreaAutomationPeer : FrameworkElementAutomationPeer
        {
            public ChartAreaAutomationPeer(ChartArea control)
                : base(control)
            {
            }

            protected override string GetClassNameCore()
            {
                return "ChartArea";
            }

            protected override AutomationControlType GetAutomationControlTypeCore()
            {
                return AutomationControlType.Custom;
            }
            
            public override object GetPattern(PatternInterface patternInterface)
            {               
                    return this;
            }
           

            private ChartArea MyOwner
            {
                get
                {
                    return (ChartArea)base.Owner;
                }
            }
        }

       /// <summary>
       /// 
       /// </summary>
       /// <returns></returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ChartAreaAutomationPeer(this);
        }

        /// <summary>
        /// Invoked whenever the effective value of any dependency property on this <see cref="T:System.Windows.FrameworkElement"/> has been updated. The specific dependency property that changed is reported in the arguments parameter. Overrides <see cref="M:System.Windows.DependencyObject.OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs)"/>.
        /// </summary>
        /// <param name="e">The event data that describes the property that changed, as well as old and new values.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == ChartArea.BackgroundProperty || e.Property == ChartArea.ForegroundProperty || e.Property == ChartArea.IsContextMenuEnabledProperty)
            {
                string backgrnd, foregrnd;
                if (this.Background == null)
                {
                    backgrnd = string.Empty;
                }
                else
                {
                    backgrnd = this.Background.ToString();
                }
                if (this.Foreground == null)
                {
                    foregrnd = string.Empty;
                }
                else
                {
                    foregrnd = this.Foreground.ToString();
                }
                AutomationProperties.SetItemStatus(this, backgrnd + ";" + foregrnd + ";" + string.Empty + ";" + string.Empty + ";" + string.Empty + ";" + string.Empty + ";" + string.Empty + ";" + this.IsContextMenuEnabled.ToString() +";");

            }
            base.OnPropertyChanged(e);
        }
        internal event AreaPresenterEventHandler AreaPresenterEvent;
        internal delegate void AreaPresenterEventHandler(object sender);

        internal virtual void OnAreaPresenterEvent(object obj)
        {
            if (AreaPresenterEvent != null)
            {
                AreaPresenterEvent(this);
            }
        }

        internal double BottomSpace
        {
            get;
            set;
        }

        double m_actualMinHeight = 0;
        internal bool m_scrolling = false;
        internal double ActualMinHeight
        {
            get
            {
                return m_actualMinHeight;
            }
            set
            {
                m_actualMinHeight = value;
            }
        }

        Size totalSize = new Size();
        internal Size TotalSize
        {
            get
            {
                return totalSize;
            }

            set
            {
                totalSize = value;
            }
        }

        internal double AreaPaddingBottom = 0d;
        internal static readonly DependencyProperty SplitRatioProperty = DependencyProperty.Register("SplitRatio", typeof(double), typeof(ChartArea), new PropertyMetadata(double.NaN));
        /// <summary>
        /// Gets or sets the sync chart area.
        /// </summary>
        /// <value>The sync chart area.</value>
        internal double SplitRatio
        {
            set { SetValue(SplitRatioProperty, value); }
            get { return (double)GetValue(SplitRatioProperty); }
        }

        internal bool flag = false;
        internal bool BottomAreaSplitterFlag = false;
        internal bool TopAreaSplitterFlag = false;
        internal ChartSegment DragSegment = null;
        internal bool isSpliterDragged = false;
        internal bool IsSplitterDrag = false;
        internal bool isIndicatorEnabled = false;
        internal bool hasStack100NegValues = false;
        //public static readonly DependencyProperty InteractiveCursorProperty = DependencyProperty.Register("InteractiveCursorCollection", typeof(InteractiveCursorCollection), typeof(ChartArea), new PropertyMetadata(null));

        //internal bool m_isFontFamilySet = false;
        //internal bool m_isFontSizeSet = false;
        //internal bool m_isFontWeightSet = false;
        //internal bool m_isForegroundSet = false;

        /// <summary>
        /// Identifies the InteractiveCursor collections
        /// </summary>
        private InteractiveCursorCollection m_interactivecursor = new InteractiveCursorCollection();
        /// <summary>
        /// Gets or sets the interactive cursors.
        /// </summary>
        /// <value>The interactive cursors.</value>
        /// System.StackOverflowException was unhandled
        [XmlIgnore]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public InteractiveCursorCollection InteractiveCursors
        {
            get { return m_interactivecursor; }
            set { m_interactivecursor = value; }
        }

        /// <summary>
        /// Identifies the SyncChartArea
        /// </summary>
        internal static readonly DependencyProperty SyncChartAreaProperty = DependencyProperty.Register("SyncChartArea", typeof(SyncChartAreas), typeof(ChartArea), new PropertyMetadata(null));
        /// <summary>
        /// Gets or sets the sync chart area.
        /// </summary>
        /// <value>The sync chart area.</value>
        internal SyncChartAreas SyncChartArea
        {
            set { SetValue(SyncChartAreaProperty, value); }
            get { return (SyncChartAreas)GetValue(SyncChartAreaProperty); }
        }
        [XmlIgnore]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        ObservableCollection<MenuItem> customContextMenuItems = new ObservableCollection<MenuItem>();
        /// <summary>
        /// 
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ObservableCollection<MenuItem> CustomContextMenuItems
        {
            get { return customContextMenuItems; }
            set { customContextMenuItems = value; }
        }

        #region events

        /// <summary>
        /// 
        /// </summary>
        public event ChartSegmentDragEventHandler SegmentDragging;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        internal void OnSegmentDragging(SegmentDragEventArgs args)
        {
            if (SegmentDragging != null)
            {
                SegmentDragging(this, args);
            }
        }

        /// <summary>
        /// Event for ChartArea segmentDragged
        /// </summary>
        public event ChartSegmentDragEventHandler SegmentDragged;

        internal void OnSegmentDragged(SegmentDragEventArgs args)
        {
            if (SegmentDragged != null)
            {
                SegmentDragged(this, args);
            }
        }


        /// <summary>
        /// Event for Chartarea segmentDropping
        /// </summary>
        public event ChartSegmentDropEventHandler SegmentDropping;
        internal void OnSegmentDropping(SegmentDropEventArgs args)
        {
            if (SegmentDropping != null)
            {
                SegmentDropping(this, args);
            }
        }


        /// <summary>
        /// Event for ChartArea segmentDropped
        /// </summary>
        public event ChartSegmentDropEventHandler SegmentDropped;
        internal void OnSegmentDropped(SegmentDropEventArgs args)
        {
            if (SegmentDropped != null)
            {
                SegmentDropped(this, args);
            }
        }

        #endregion


        #region Constants
        /// <summary>
        /// Declares c_colorPaletteSize
        /// </summary>
        private const int C_colorPaletteSize = 100;


        #endregion

        #region Members

        private double lastPosition_X = 0d;
        internal bool m_isRadar = false;
        /// <summary>
        /// Initializes m_dockPanel
        /// </summary>
        private ChartDockPanel _mDockPanel;

        /// <summary>
        /// Initializes m_visibleSeriesSegmentsRecountRequired
        /// </summary>
        internal bool m_visibleSeriesSegmentsRecountRequired;

        /// <summary>
        /// Initializes m_segmentsResetRequired
        /// </summary>
        private bool m_segmentsResetRequired;

        /// <summary>
        /// Identify the Zoom operation is performed using Mouse drag.
        /// </summary>
        private bool m_isMouseDragZooming = false;

        /// <summary>
        /// Initializes m_areaPresenter
        /// </summary>
        internal ChartAreaPresenter m_areaPresenter;

        /// <summary>
        /// Initializes m_colorModel
        /// </summary>
        private ChartStyleModel m_colorModel = new ChartStyleModel();

        /// <summary>
        /// Initializes m_series
        /// </summary>
        private ChartSeriesCollection m_series = new ChartSeriesCollection();

        /// <summary>
        /// Initializes m_axes
        /// </summary>
        private ChartAxesCollection m_axes = new ChartAxesCollection();

        /// <summary>
        /// Initializes m_visibleSeries
        /// </summary>
        private VisibleSeriesCollection m_visibleSeries;

        /// <summary>
        /// Initializes m_highlightedSegment
        /// </summary>
        private ChartSegment m_highlightedSegment;

        /// <summary>
        /// Initializes m_minPointsDelta
        /// </summary>
        private double m_minPointsDelta = double.NaN;

        /// <summary>
        /// Initializes m_exMinPointsDelta
        /// </summary>
        private double m_exMinPointsDelta = double.NaN;

        /// <summary>
        /// Initializes m_mouseCaptureLocation
        /// </summary>
        private Point m_mouseCaptureLocation;

        /// <summary>
        /// Initializes m_capturedCursor
        /// </summary>
        private Cursor m_capturedCursor;

        /// <summary>
        /// Initializes m_ChartTargetCameraController
        /// </summary>
        private ChartTargetCameraController m_chartTargetCameraController;

        /// <summary>
        /// Initializes m_3DAxesShiftLeftOrientation
        /// </summary>
        private Point3D m_axes3DShiftLeftOrientation;

        /// <summary>
        /// Initializes m_3DAxesShiftRightOrientation
        /// </summary>
        private Point3D m_axes3DShiftRightOrientation;
        
        internal bool IsSync = false;

        internal bool hasMinWidth = false;

        internal Thickness areaThickness = new Thickness();

        internal Thickness axisThickness = new Thickness();

        internal double marginValueForCursorLabel = 0.0;

        //internal double rightThikness = 0.0d;

        internal double differenceInWidth = 0.0d;

        internal int index = 0;

        internal Rect axisRect = new Rect();

        internal bool hasMinWidthInLeft = false;

        internal bool hasMinWidthInRight = false;

        internal int axesInLeft = 0;

        internal int axesInRight = 0;

        internal bool updated = false;

        #endregion

        #region Protected Members

        internal bool disableIsIndexedForOLAP = false;
        /// <summary>
        /// Identifies the DisableIsIndexedForOLAP
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [disable is indexed for OLAP]; otherwise, <c>false</c>.
        /// </value>
        protected bool DisableIsIndexedForOLAP
        {
            get
            {
                return disableIsIndexedForOLAP;
            }
            set
            {
                disableIsIndexedForOLAP = value;
            }
        }

        internal bool isAreaHeightSet
        {
            get;
            set;
        }

        internal bool isResizable
        {
            get;
            set;
        }
        internal bool isLoadedFirst
        {
            get;
            set;
        }
        internal double proportion
        {
            get;
            set;
        }

        #endregion

        #region Dependency properties

        //DependencyPropertyDescriptor FontFamilyDescriptor = DependencyPropertyDescriptor.FromProperty(ChartArea.FontFamilyProperty, typeof(ChartArea));
        //DependencyPropertyDescriptor FontSizeDescriptor = DependencyPropertyDescriptor.FromProperty(ChartArea.FontSizeProperty, typeof(ChartArea));
        //DependencyPropertyDescriptor FontWeightDescriptor = DependencyPropertyDescriptor.FromProperty(ChartArea.FontWeightProperty, typeof(ChartArea));
        //DependencyPropertyDescriptor ForegroundDescriptor = DependencyPropertyDescriptor.FromProperty(ChartArea.ForegroundProperty, typeof(ChartArea));
        /// <summary>
        /// Identifies the Watermark dependency property.
        /// </summary>
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register("Watermark", typeof(Brush), typeof(Chart), new PropertyMetadata(Brushes.Transparent));

        /// <summary>
        /// Gets or sets the watermark for chart.
        /// </summary>
        /// <value>The Watermark.</value>
        public Brush Watermark
        {
            get { return (Brush)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        /// <summary>
        /// Identifies the OriginLineStroke dependency property.
        /// </summary>
        public static readonly DependencyProperty IsRetainAxisPositionProperty =
          DependencyProperty.RegisterAttached("IsRetainAxisPosition", typeof(bool), typeof(ChartArea), new PropertyMetadata(false));


        private static void OnRetainAxisPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartArea area = d as ChartArea;

            if (area != null)
            {
                if (area is SyncChartAreas)
                {
                    if ((area as SyncChartAreas).Areas != null)
                    {
                        foreach (var item in (area as SyncChartAreas).Areas)
                        {
                            area.IsRetainAxisPosition = (bool)args.NewValue;
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Identifies the X-Range on Panning.
        /// </summary>
        internal static readonly DependencyProperty PanningRange_XProperty =
         DependencyProperty.RegisterAttached("PanningRange_X", typeof(DoubleRange), typeof(ChartArea), new PropertyMetadata(DoubleRange.Empty));

        /// <summary>
        /// Identifies the Y-Range on Panning.
        /// </summary>
        internal static readonly DependencyProperty PanningRange_YProperty =
            DependencyProperty.RegisterAttached("PanningRange_Y", typeof(DoubleRange), typeof(ChartArea), new PropertyMetadata(DoubleRange.Empty));

        /// <summary>
        /// Set the Panning property
        /// </summary>
        internal static readonly DependencyProperty IsPanningProperty =
          DependencyProperty.RegisterAttached("IsPanning", typeof(bool), typeof(ChartArea), new UIPropertyMetadata(false));

        /// <summary>
        /// Set the PrimaryChartArea
        /// </summary>
        internal static readonly DependencyProperty PrimaryChartAreaProperty =
DependencyProperty.Register("PrimaryChartArea", typeof(ChartArea), typeof(SyncChartAreas), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies the SideBySideSeriesPlacement dependency property.
        /// </summary>
        public static readonly DependencyProperty SideBySideSeriesPlacementProperty =
            DependencyProperty.Register("SideBySideSeriesPlacement", typeof(bool), typeof(ChartArea), new UIPropertyMetadata(true, new PropertyChangedCallback(OnSideBySideSeriesPlacementPropertyChanged)));

        /// <summary>
        /// Identifies the OriginLineStroke dependency property.
        /// </summary>
        public static readonly DependencyProperty OriginLineStrokeProperty =
          DependencyProperty.RegisterAttached("OriginLineStroke", typeof(Pen), typeof(ChartArea), new PropertyMetadata(new Pen(Brushes.Black, 1)));

        /// <summary>
        /// Identifies the ShowOriginLine dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowOriginLineProperty =
          DependencyProperty.RegisterAttached("ShowOriginLine", typeof(bool), typeof(ChartArea), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowOriginChanged), new CoerceValueCallback(OnCoerceShowOriginLines)));

        /// <summary>
        /// Identifies the GridLineStroke dependency property.
        /// </summary>
        public static readonly DependencyProperty GridLineStrokeProperty =
          DependencyProperty.RegisterAttached("GridLineStroke", typeof(Pen), typeof(ChartArea), new FrameworkPropertyMetadata(new Pen(new SolidColorBrush(Color.FromArgb(0XFF,0XBA,0XBA,0XBA)), 1), FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnGridLineStrokeChanged)));

        /// <summary>
        /// Identifies the SmallGridLineStroke dependency property.
        /// </summary>
        public static readonly DependencyProperty SmallGridLineStrokeProperty =
      DependencyProperty.RegisterAttached("SmallGridLineStroke", typeof(Pen), typeof(ChartArea), new FrameworkPropertyMetadata(new Pen(new SolidColorBrush(Color.FromArgb(0XFF,0XBA,0XBA,0XBA)), 1), FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnGridLineStrokeChanged)));


        /// <summary>
        /// Identifies the ShowGridLines dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowGridLinesProperty =
          DependencyProperty.RegisterAttached("ShowGridLines", typeof(bool), typeof(ChartArea), new UIPropertyMetadata(true, null, new CoerceValueCallback(OnCoerceShowGridLines)));

        /// <summary>
        ///  Identifies the ShowMajorGridLines dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowMajorGridLinesProperty =
         DependencyProperty.RegisterAttached("ShowMajorGridLines", typeof(bool), typeof(ChartArea), new UIPropertyMetadata(true, null, new CoerceValueCallback(OnCoerceShowMajorGridLines)));

        /// <summary>
        /// Identifies the ElementMargin dependency property.
        /// </summary>
        public static readonly DependencyProperty ElementMarginProperty =
          DependencyProperty.Register("ElementMargin", typeof(Thickness), typeof(ChartArea), new PropertyMetadata(new Thickness(4)));

        /// <summary>
        /// Identifies the GridBackground dependency property.
        /// </summary>
        public static readonly DependencyProperty GridBackgroundProperty =
            DependencyProperty.Register("GridBackground", typeof(Brush), typeof(ChartArea), new UIPropertyMetadata(Brushes.Transparent));

        /// <summary>
        /// Identifies the HoldUpdateProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty HoldUpdateProperty =
            DependencyProperty.Register("HoldUpdate", typeof(bool), typeof(ChartArea), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies the CornerRadius dependency property.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty = Border.CornerRadiusProperty.AddOwner(typeof(ChartArea), new PropertyMetadata(new CornerRadius(2)));

        /// <summary>
        /// Identifies the Header dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = HeaderedContentControl.HeaderProperty.AddOwner(typeof(ChartArea), new PropertyMetadata(null));

		/// <summary>
        /// Identifies the Footer dependency property.
        /// </summary>"
        public static readonly DependencyProperty FooterProperty = DependencyProperty.Register("Footer", typeof(object), typeof(ChartArea), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the SecondaryAxis dependency property.
        /// </summary>
        public static readonly DependencyProperty DepthAxisProperty = DependencyProperty.Register("DepthAxis", typeof(ChartAxis), typeof(ChartArea), new PropertyMetadata(null, new PropertyChangedCallback(OnDepthAxisChanged), new CoerceValueCallback(OnCoerceAxis)));

        /// <summary>
        /// Identifies the SecondaryAxis dependency property.
        /// </summary>
        public static readonly DependencyProperty SecondaryAxisProperty = DependencyProperty.Register("SecondaryAxis", typeof(ChartAxis), typeof(ChartArea), new PropertyMetadata(null, new PropertyChangedCallback(OnSecondaryAxisChanged), new CoerceValueCallback(OnCoerceAxis)));

        /// <summary>
        /// Identifies the PrimaryAxis dependency property.
        /// </summary>
        public static readonly DependencyProperty PrimaryAxisProperty = DependencyProperty.Register("PrimaryAxis", typeof(ChartAxis), typeof(ChartArea), new PropertyMetadata(null, new PropertyChangedCallback(OnPrimaryAxisChanged), new CoerceValueCallback(OnCoerceAxis)));

        /// <summary>
        /// Identifies the AxesThickness dependency property.
        /// </summary>
        public static readonly DependencyProperty AxesThicknessProperty = DependencyProperty.Register("AxesThickness", typeof(Thickness), typeof(ChartArea), new PropertyMetadata(new Thickness(0)));

        /// <summary>
        /// Identifies the AreaType dependency property.
        /// </summary>
        public static readonly DependencyProperty AreaTypeProperty = DependencyProperty.Register("AreaType", typeof(ChartAxesType), typeof(ChartArea), new PropertyMetadata(ChartAxesType.CartesianAxes, new PropertyChangedCallback(OnAxesTypeChanged)));

        /// <summary>
        /// Identifies the HorizontalScrollingAxis dependency property.
        /// </summary>
        internal static readonly DependencyProperty HorizontalScrollingAxisProperty =
            DependencyProperty.Register("HorizontalScrollingAxis", typeof(ChartAxis), typeof(ChartArea), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnScrollingAxisChanged), new CoerceValueCallback(OnCoerceHorizontalScrollingAxis)));

        /// <summary>
        /// Identifies the VerticalScrollingAxis dependency property.
        /// </summary>
        internal static readonly DependencyProperty VerticalScrollingAxisProperty =
            DependencyProperty.Register("VerticalScrollingAxis", typeof(ChartAxis), typeof(ChartArea), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnScrollingAxisChanged), new CoerceValueCallback(OnCoerceVerticalScrollingAxis)));

        /// <summary>
        /// Identifies the ZoomAllAxes dependency property.
        /// </summary>
        public static readonly DependencyProperty ZoomAllAxesProperty =
            DependencyProperty.Register("ZoomAllAxes", typeof(bool), typeof(ChartArea), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnZoomAllAxesChanged)));

        /// <summary>
        /// Identifies the Legend dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendProperty =
            DependencyProperty.Register("Legend", typeof(ChartLegend), typeof(ChartArea), new UIPropertyMetadata(null, OnLegendPropertyChanged));

        /// <summary>
        /// Identifies the AlternatingGridBackground dependency property.
        /// </summary>
        public static readonly DependencyProperty AlternatingGridBackgroundProperty =
            DependencyProperty.Register("AlternatingGridBackground", typeof(Brush), typeof(ChartArea), new UIPropertyMetadata(Brushes.Transparent));

        /// <summary>
        /// Identifies the AlternatingFillMode dependency property.
        /// </summary>
        public static readonly DependencyProperty AlternatingFillModeProperty =
            DependencyProperty.Register("AlternatingFillMode", typeof(AlternatingFillMode), typeof(ChartArea), new UIPropertyMetadata(AlternatingFillMode.Even));

        /// <summary>
        /// Identifies the AlternatingFillDirection dependency property.
        /// </summary>
        public static readonly DependencyProperty AlternatingFillDirectionProperty =
            DependencyProperty.Register("AlternatingFillDirection", typeof(Orientation), typeof(ChartArea), new UIPropertyMetadata(Orientation.Horizontal));

        /// <summary>
        /// Identifies the ZoomSwitched dependency property.
        /// </summary>
        public static readonly DependencyProperty ZoomSwitchedProperty =
            DependencyProperty.Register("ZoomSwitched", typeof(bool), typeof(ChartArea), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies the View3DMode dependency property.
        /// </summary>
        public static readonly DependencyProperty View3DModeProperty =
            DependencyProperty.Register("View3DMode", typeof(bool), typeof(ChartArea), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnView3DModeChanged)));

               
        /// <summary>
        /// Using a DependencyProperty as the backing store for EnableDepthAxis.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EnableDepthAxisProperty =
            DependencyProperty.Register("EnableDepthAxis", typeof(bool), typeof(ChartArea), new UIPropertyMetadata(false, new PropertyChangedCallback(OnEnableDepthAxisChanged)));


        /// <summary>
        /// Using a DependencyProperty as the backing store for AxiseContent.  This enables animation, styling, binding, etc...
        /// </summary>
        internal static readonly DependencyProperty AxisContentProperty =
            DependencyProperty.Register("AxisContent", typeof(Model3D), typeof(ChartArea), new UIPropertyMetadata(null));

        /// <summary>
        /// Using a DependencyProperty as the backing store for AxiseContent.  This enables animation, styling, binding, etc...
        /// </summary>
        internal static readonly DependencyProperty Labels3DContentProperty =
            DependencyProperty.Register("Labels3DContent", typeof(Model3DGroup), typeof(ChartArea), new UIPropertyMetadata(null));

        /// <summary>
        /// Using a DependencyProperty as the backing store for Content3D.  This enables animation, styling, binding, etc...
        /// </summary>
        internal static readonly DependencyProperty Content3DProperty =
            DependencyProperty.Register("Content3D", typeof(Model3D), typeof(ChartArea), new UIPropertyMetadata(null));

        /// <summary>
        /// Using a DependencyProperty as the backing store for Camera3D.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty Camera3DProperty =
            DependencyProperty.Register("Camera3D", typeof(Camera), typeof(ChartArea), new UIPropertyMetadata(null));

        /// <summary>
        /// Using a DependencyProperty as the backing store for LightContent.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LightContentProperty =
            DependencyProperty.Register("LightContent", typeof(Model3D), typeof(ChartArea), new UIPropertyMetadata(null));

        /// <summary>
        /// Using a DependencyProperty as the backing store for LightContent.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty GridContentProperty =
            DependencyProperty.Register("GridContent", typeof(Model3D), typeof(ChartArea), new UIPropertyMetadata(null));

        /// <summary>
        /// Using a DependencyProperty as the backing store for Chart3DSettings.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty Chart3DSettingsProperty =
            DependencyProperty.Register("Chart3DSettings", typeof(Chart3D), typeof(ChartArea), new UIPropertyMetadata(null, new PropertyChangedCallback(OnChart3DSettingsChanged), CoerceChart3DSettingsProperty));

        /// <summary>
        /// Identifies the IsContextMenuEnabled dependency property.
        /// </summary>
        public static readonly DependencyProperty IsContextMenuEnabledProperty =
            DependencyProperty.Register("IsContextMenuEnabled", typeof(bool), typeof(ChartArea), new FrameworkPropertyMetadata(false, OnIsContextMenuEnabledChanged));

        /// <summary>
        /// Identifies the ZoomedXRange dependency property.
        /// </summary>
        public static readonly DependencyProperty ZoomedXRangeProperty =
               DependencyProperty.Register("ZoomedXRange", typeof(DoubleRange), typeof(ChartArea), new UIPropertyMetadata(DoubleRange.Empty));

        /// <summary>
        /// Identifies the ZoomedYRange dependency property.
        /// </summary>
        public static readonly DependencyProperty ZoomedYRangeProperty =
            DependencyProperty.Register("ZoomedYRange", typeof(DoubleRange), typeof(ChartArea), new UIPropertyMetadata(DoubleRange.Empty));

        /// <summary>
        /// Identifies the ChartAreaParent dependency property.
        /// </summary>
        internal static readonly DependencyProperty ChartAreaParentProperty =
           DependencyProperty.Register("ChartAreaParent", typeof(SyncChartAreas), typeof(ChartArea), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the ChartAreaIndex dependency property.
        /// </summary>
        internal static readonly DependencyProperty ChartAreaIndexProperty =
        DependencyProperty.Register("ChartAreaIndex", typeof(int), typeof(ChartArea), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the ChartAreaCount dependency property.
        /// </summary>
        internal static readonly DependencyProperty ChartAreaCountProperty =
        DependencyProperty.Register("ChartAreaCount", typeof(int), typeof(ChartArea), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the SplitterDelta dependency property.
        /// </summary>
        internal static readonly DependencyProperty SplitterDeltaProperty =
            DependencyProperty.Register("SplitterDelta", typeof(double), typeof(ChartArea), new PropertyMetadata(0.0));

        /// <summary>
        /// Identifies the SplitterVisiblity dependency property.
        /// </summary>
        public static readonly DependencyProperty SplitterVisiblityProperty =
         DependencyProperty.Register("SplitterVisiblity", typeof(SpliterVisibility), typeof(ChartArea), new PropertyMetadata(SpliterVisibility.ShowOnMouseHover, OnSplitterVisibiltyChanged));

        /// <summary>
        /// Identifies the SplitterWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty SplitterWidthProperty =
       DependencyProperty.Register("SplitterWidth", typeof(double), typeof(ChartArea), new PropertyMetadata(2.0d));

        /// <summary>
        /// Identifies the SplitterColor dependency property.
        /// </summary>
        public static readonly DependencyProperty SplitterColorProperty =
                 DependencyProperty.Register("SplitterColor", typeof(Brush), typeof(ChartArea), new PropertyMetadata(Brushes.Gray));

        /// <summary>
        /// Identifies the SplitterStroke dependency property.
        /// </summary>
        public static readonly DependencyProperty SplitterStrokeProperty =
               DependencyProperty.Register("SplitterStroke", typeof(Brush), typeof(ChartArea), new PropertyMetadata(Brushes.Gray));

        /// <summary>
        /// Identifies the LegendStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty EnableZoomOnScrollProperty =
         DependencyProperty.Register("EnableZoomOnScroll", typeof(bool), typeof(ChartArea), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the EnableZoomOnScroll value.
        /// </summary>
        /// <value>The LegendStyle.</value>
        public bool EnableZoomOnScroll
        {
            get
            {
                return (bool)GetValue(EnableZoomOnScrollProperty);
            }

            set
            {
                SetValue(EnableZoomOnScrollProperty, value);
            }
        }      

        /// <summary>
        /// Get and Set HoldUpdateProperty
        /// </summary>
        public bool HoldUpdate
        {
            get
            {
                return (bool)GetValue(HoldUpdateProperty);
            }

            set
            {
                SetValue(HoldUpdateProperty, value);
            }
        }

        /// <summary>
        /// Identifies the LegendStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty EnableMouseDragZoomingProperty =
         DependencyProperty.Register("EnableMouseDragZooming", typeof(bool), typeof(ChartArea), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets the EnableZoomOnScroll value.
        /// </summary>
        /// <value>The LegendStyle.</value>
        public bool EnableMouseDragZooming
        {
            get
            {
                return (bool)GetValue(EnableMouseDragZoomingProperty);
            }

            set
            {
                SetValue(EnableMouseDragZoomingProperty, value);
            }
        }
        /// <summary>
        /// Identifies the LegendStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendStyleProperty =
         DependencyProperty.Register("LegendStyle", typeof(Style), typeof(ChartArea), new PropertyMetadata(null, new PropertyChangedCallback(OnLegendStyleChanged)));

        /// <summary>
        /// Gets or sets the LegendStyle value.
        /// </summary>
        /// <value>The LegendStyle.</value>
        public Style LegendStyle
        {
            get
            {
                return (Style)GetValue(LegendStyleProperty);
            }

            set
            {
                SetValue(LegendStyleProperty, value);
            }
        }

        /// <summary>
        ///  Identifies the PrimaryAxisStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty PrimaryAxisStyleProperty =
         DependencyProperty.Register("PrimaryAxisStyle", typeof(Style), typeof(ChartArea), new PropertyMetadata(null, new PropertyChangedCallback(OnPrimaryAxisStyleChanged)));

        /// <summary>
        /// Gets or sets the LegendStyle value.
        /// </summary>
        /// <value>The LegendStyle.</value>
        public Style PrimaryAxisStyle
        {
            get
            {
                return (Style)GetValue(PrimaryAxisStyleProperty);
            }

            set
            {
                SetValue(PrimaryAxisStyleProperty, value);
            }
        }
        /// <summary>
        ///  Identifies the SecondaryAxisStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty SecondaryAxisStyleProperty =
        DependencyProperty.Register("SecondaryAxisStyle", typeof(Style), typeof(ChartArea), new PropertyMetadata(null, new PropertyChangedCallback(OnSecondaryAxisStyleChanged)));

        /// <summary>
        /// Gets or sets the LegendStyle value.
        /// </summary>
        /// <value>The LegendStyle.</value>
        public Style SecondaryAxisStyle
        {
            get
            {
                return (Style)GetValue(SecondaryAxisStyleProperty);
            }

            set
            {
                SetValue(SecondaryAxisStyleProperty, value);
            }
        }


        /// <summary>
        ///  Identifies the SeriesStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty SeriesStyleProperty =
       DependencyProperty.Register("SeriesStyle", typeof(Style), typeof(ChartArea), new PropertyMetadata(null, new PropertyChangedCallback(OnSeriesStyleChanged)));

        /// <summary>
        /// Gets or sets the LegendStyle value.
        /// </summary>
        /// <value>The LegendStyle.</value>
        public Style SeriesStyle
        {
            get
            {
                return (Style)GetValue(SeriesStyleProperty);
            }

            set
            {
                SetValue(SeriesStyleProperty, value);
            }
        }

        /// <summary>
        ///  Identifies the EnableRangeSelection dependency property.
        /// </summary>
        public static readonly DependencyProperty EnableRangeSelectionProperty =
      DependencyProperty.Register("EnableRangeSelection", typeof(bool), typeof(ChartArea), new PropertyMetadata(false, new PropertyChangedCallback(OnEnabelSeriesInterativeChanged)));

        /// <summary>
        /// Gets or sets the LegendStyle value.
        /// </summary>
        /// <value>The LegendStyle.</value>
        public bool EnableRangeSelection
        {
            get
            {
                return (bool)GetValue(EnableRangeSelectionProperty);
            }

            set
            {
                SetValue(EnableRangeSelectionProperty, value);
            }
        }

        /// <summary>
        ///  Identifies the LowerRangeLabel dependency property.
        /// </summary>
        public static readonly DependencyProperty LowerRangeLabelProperty = DependencyProperty.Register("LowerRangeLabel", typeof(object), typeof(ChartArea), new PropertyMetadata(null));
        /// <summary>
        /// Get and Set LowerRangeLabelProperty
        /// </summary>
        public object LowerRangeLabel
        {
            get { return (object)GetValue(LowerRangeLabelProperty); }
            set { SetValue(LowerRangeLabelProperty, value); }
        }

        /// <summary>
        ///  Identifies the UpperRangeLabel dependency property.
        /// </summary>
        public static readonly DependencyProperty UpperRangeLabelProperty = DependencyProperty.Register("UpperRangeLabel", typeof(object), typeof(ChartArea), new PropertyMetadata(null));
        /// <summary>
        /// Get and Set UpperRangelabel
        /// </summary>
        public object UpperRangeLabel
        {
            get { return (object)GetValue(UpperRangeLabelProperty); }
            set { SetValue(UpperRangeLabelProperty, value); }
        }

        /// <summary>
        /// Get and Set RangeSelectionOrientationProperty
        /// </summary>
        public Orientation RangeSelectionOrientation
        {
            get { return (Orientation)GetValue(RangeSelectionOrientationProperty); }
            set { SetValue(RangeSelectionOrientationProperty, value); }
        }

        
       /// <summary>
        /// Using a DependencyProperty as the backing store for InteractiveCursorMargin.  This enables animation, styling, binding, etc...
       /// </summary>
        public static readonly DependencyProperty RangeSelectionOrientationProperty =
            DependencyProperty.Register("RangeSelectionOrientation", typeof(Orientation), typeof(ChartArea), new UIPropertyMetadata(Orientation.Vertical, new PropertyChangedCallback(OnvalueChanged)));

        /// <summary>
        ///  Identifies the LineStroke dependency property.
        /// </summary>
        public static readonly DependencyProperty LineStrokeProperty =
      DependencyProperty.Register("LineStroke", typeof(Brush), typeof(ChartArea), new PropertyMetadata(Brushes.DarkGreen, new PropertyChangedCallback(OnLineStrokeChanged)));

        /// <summary>
        /// Gets or sets the LegendStyle value.
        /// </summary>
        /// <value>The LegendStyle.</value>
        public Brush LineStroke
        {
            get
            {
                return (Brush)GetValue(LineStrokeProperty);
            }

            set
            {
                SetValue(LineStrokeProperty, value);
            }
        }

        /// <summary>
        ///  Identifies the SelectionStroke dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionStrokeProperty =
   DependencyProperty.Register("SelectionStroke", typeof(Brush), typeof(ChartArea), new PropertyMetadata(Brushes.LightGreen, new PropertyChangedCallback(OnSelectionStrokeChanged)));

        /// <summary>
        /// Gets or sets the LegendStyle value.
        /// </summary>
        /// <value>The LegendStyle.</value>
        public Brush SelectionStroke
        {
            get
            {
                return (Brush)GetValue(SelectionStrokeProperty);
            }

            set
            {
                SetValue(SelectionStrokeProperty, value);
            }
        }
        /// <summary>
        ///  Identifies the StartValue dependency property.
        /// </summary>
        public static readonly DependencyProperty StartValueProperty =
     DependencyProperty.Register("StartValue", typeof(double), typeof(ChartArea), new PropertyMetadata(0d, new PropertyChangedCallback(OnStartRangeChanged)));

        /// <summary>
        /// Gets or sets the LegendStyle value.
        /// </summary>
        /// <value>The LegendStyle.</value>
        public double StartValue
        {
            get
            {
                return (double)GetValue(StartValueProperty);
            }

            set
            {
                SetValue(StartValueProperty, value);
            }
        }

        /// <summary>
        ///  Identifies the EndValue dependency property.
        /// </summary>
        public static readonly DependencyProperty EndValueProperty =
     DependencyProperty.Register("EndValue", typeof(double), typeof(ChartArea), new PropertyMetadata(0d, new PropertyChangedCallback(OnEndRangeChanged)));

        /// <summary>
        /// Gets or sets the LegendStyle value.
        /// </summary>
        /// <value>The LegendStyle.</value>
        public double EndValue
        {
            get
            {
                return (double)GetValue(EndValueProperty);
            }

            set
            {
                SetValue(EndValueProperty, value);
            }
        }
        /// <summary>
        /// Select the Proeprty for ContextMenuType as Default,Custom or Default With custom
        /// </summary>
        public static readonly DependencyProperty ContextMenuTypeProperty =
     DependencyProperty.Register("ContextMenuType", typeof(ContextMenuTypes), typeof(ChartArea), new PropertyMetadata(ContextMenuTypes.Default));

        /// <summary>
        /// Gets or sets the LegendStyle value.
        /// </summary>
        /// <value>The LegendStyle.</value>
        public ContextMenuTypes ContextMenuType
        {
            get
            {
                return (ContextMenuTypes)GetValue(ContextMenuTypeProperty);
            }

            set
            {
                SetValue(ContextMenuTypeProperty, value);
            }
        }


        /// <summary>
        /// Get and Set ShowLegendProperty
        /// </summary>
        public bool ShowLegend
        {
            get { return (bool)GetValue(ShowLegendProperty); }
            set { SetValue(ShowLegendProperty, value); }
        }

        
        /// <summary>
        ///Using a DependencyProperty as the backing store for ShowLegend.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty ShowLegendProperty =
            DependencyProperty.Register("ShowLegend", typeof(bool), typeof(ChartArea), new UIPropertyMetadata(false,OnShowLegendChanged));

        private static void OnShowLegendChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartArea area = (ChartArea)d;
                if(area !=null)
                {
                    if ((bool)args.NewValue)
                    {
                        if (area.Legend == null)
                            area.Legend = new ChartLegend() { Visibility = Visibility.Visible };
                        else
                            area.Legend.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        if (area.Legend != null)
                            area.Legend.Visibility = Visibility.Collapsed;
                    }
                }
        }



        /// <summary>
        /// Get and Set PaletteProperty
        /// </summary>
        public ChartColorPalette Palette
        {
            get { return (ChartColorPalette)GetValue(PaletteProperty); }
            set { SetValue(PaletteProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for Palette.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PaletteProperty =
            DependencyProperty.Register("Palette", typeof(ChartColorPalette), typeof(ChartArea), new UIPropertyMetadata(ChartColorPalette.Default, OnPaletteChanged));

        private static void OnPaletteChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartArea area = d as ChartArea;
            if(area != null)
            {
                area.ColorModel.Palette= area.Palette;
                foreach (var series in area.Series)
                {
                    if (series.Type == ChartTypes.Pie || series.Type == ChartTypes.Pyramid || series.Type == ChartTypes.Funnel  || series.Type == ChartTypes.Doughnut )
                    {
                        series.Invalidate();
                    }
                }

            }

        }

        /// <summary>
        /// Get and Set StrokePaletteProperty
        /// </summary>
        public ChartColorPalette StrokePalette
        {
            get { return (ChartColorPalette)GetValue(StrokePaletteProperty); }
            set { SetValue(StrokePaletteProperty, value); }
        }

        
        /// <summary>
        ///  Using a DependencyProperty as the backing store for StrokePalette.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StrokePaletteProperty =
            DependencyProperty.Register("StrokePalette", typeof(ChartColorPalette), typeof(ChartArea), new UIPropertyMetadata(ChartColorPalette.DefaultDark));

        
        
        /// <summary>
        ///  Identifies the SplitterPosition dependency property.
        /// </summary>
        public static readonly DependencyProperty SplitterPositionProperty =
    DependencyProperty.Register("SplitterPosition", typeof(double), typeof(ChartArea), new PropertyMetadata(0d, new PropertyChangedCallback(OnSplitterPositionChanged)));
        /// <summary>
        /// Gets or sets the LegendStyle value.
        /// </summary>
        /// <value>The LegendStyle.</value>
        public double SplitterPosition
        {
            get
            {
                return (double)GetValue(SplitterPositionProperty);
            }

            set
            {
                SetValue(SplitterPositionProperty, value);
            }
        }
        /// <summary>
        /// Property for AllowSegmentDragDrop
        /// </summary>
        public static readonly DependencyProperty AllowSegmentDragDropProperty =
         DependencyProperty.Register("AllowSegmentDragDrop", typeof(bool), typeof(ChartArea), new PropertyMetadata(false));
        /// <summary>
        /// get and set AllowSegmentDragDrop property
        /// </summary>
        public bool AllowSegmentDragDrop
        {
            get { return (bool)GetValue(AllowSegmentDragDropProperty); }
            set { SetValue(AllowSegmentDragDropProperty, value); }
        }

        /// <summary>
        /// Proeprty for Allow to Rotate the Chart in 3D Mode
        /// </summary>
        public static readonly DependencyProperty Allow3DRotateProperty = DependencyProperty.Register("Allow3DRotate", typeof(Boolean), typeof(ChartArea), new PropertyMetadata(false));
        
        /// <summary>
        /// Get and set the Allow3DRotate 
        /// </summary>
        public bool Allow3DRotate
        {
            get { return (bool)GetValue(Allow3DRotateProperty); }
            set { SetValue(Allow3DRotateProperty, value); }
        }
        

        internal bool isClustered = true;
        /// <summary>
        /// Identifies the IsClustered dependency property.
        /// </summary>
        public static readonly DependencyProperty IsClusteredProperty = DependencyProperty.Register("IsClustered", typeof(bool), typeof(ChartArea), new PropertyMetadata(true, new PropertyChangedCallback(OnIsClusterChanged)));

        /// <summary>
        /// Gets or sets the IsClustered value.
        /// </summary>
        /// <value>The IsClustered.</value>
        public bool IsClustered
        {
            get
            {
                return (bool)GetValue(IsClusteredProperty);
            }
            set
            {
                SetValue(IsClusteredProperty, value);
            }
        }

        /// <summary>
        /// Called when IsCluster property changed.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnIsClusterChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ChartArea area = obj as ChartArea;
            if (area.View3DMode == true)
            {
                if (area.IsClustered == true)
                {
                    area.isClustered = true;
                }
                else
                {
                    area.isClustered = false;
                }
                area.Chart3DSettings.ShowDepthAxis = false;
                area.UpdateArea();
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Declares m_ZoomInCoefficient
        /// </summary>
        private double m_ZoomInCoefficient = 0.5d;
        /// <summary>
        /// Get and Set ZoomInCoefficient
        /// </summary>
        public double ZoomInCoefficient
        {
            get { return m_ZoomInCoefficient; }
            set
            {
                m_ZoomInCoefficient = value > 1 ? 1 : (value < 0 ? 0 : value);
            }
        }

        /// <summary>
        /// Declares m_ZoomOutCoefficient
        /// </summary>
        private double m_ZoomOutCoefficient = 2d;
        /// <summary>
        /// Get and Set ZoomOutCoefficient
        /// </summary>
        public double ZoomOutCoefficient
        {
            get { return m_ZoomOutCoefficient; }
            set
            {
                if (value != double.NaN)
                {
                    m_ZoomOutCoefficient = value < 1 ? 1 : value;
                }
            }
        }


        /// <summary>
        /// Gets or sets the splitter stroke.
        /// </summary>
        /// <value>The splitter stroke.</value>
        public Brush SplitterStroke
        {
            get { return (Brush)GetValue(SplitterStrokeProperty); }
            set { SetValue(SplitterStrokeProperty, value); }
        }
        /// <summary>
        /// Gets or sets the color of the splitter.
        /// </summary>
        /// <value>The color of the splitter.</value>
        public Brush SplitterColor
        {
            get { return (Brush)GetValue(SplitterColorProperty); }
            set { SetValue(SplitterColorProperty, value); }
        }
        /// <summary>
        /// Gets or sets the panning range_ X.
        /// </summary>
        /// <value>The panning range_ X.</value>
        internal DoubleRange PanningRange_X
        {
            get { return (DoubleRange)GetValue(PanningRange_XProperty); }
            set { SetValue(PanningRange_XProperty, value); }
        }
        /// <summary>
        /// Gets or sets the panning range_ Y.
        /// </summary>
        /// <value>The panning range_ Y.</value>
        internal DoubleRange PanningRange_Y
        {
            get { return (DoubleRange)GetValue(PanningRange_YProperty); }
            set { SetValue(PanningRange_YProperty, value); }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is panning.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is panning; otherwise, <c>false</c>.
        /// </value>
        internal bool IsPanning
        {
            get { return (bool)GetValue(IsPanningProperty); }
            set { SetValue(IsPanningProperty, value); }
        }

        /// <summary>
        /// Gets or sets the primary chart area.
        /// </summary>
        /// <value>The primary chart area.</value>
        internal ChartArea PrimaryChartArea
        {
            get { return (ChartArea)GetValue(PrimaryChartAreaProperty); }
            set { SetValue(PrimaryChartAreaProperty, value); }
        }


        /// <summary>
        /// Gets or sets the element margin. This is a dependency property.
        /// </summary>
        /// <value>The element margin.</value>
        public Thickness ElementMargin
        {
            get
            {
                return (Thickness)GetValue(ChartArea.ElementMarginProperty);
            }

            set
            {
                SetValue(ChartArea.ElementMarginProperty, value);
            }
        }


        #region Internal properties
        /// <summary>
        /// Gets or sets a value indicating whether area could be represented with indexed series..
        /// </summary>
        /// <value>
        ///  <c>true</c> if this visible series are compatible to be indexed; otherwise, <c>false</c>.
        /// </value>
        internal bool IsIndexedCompatible
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the content of the axis.
        /// </summary>
        /// <value>The content of the axis.</value>
        internal Model3D AxisContent
        {
            get
            {
                return (Model3D)GetValue(AxisContentProperty);
            }

            set
            {
                SetValue(AxisContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the content of the labels3D.
        /// </summary>
        /// <value>The content of the labels3D.</value>
        internal Model3D Labels3DContent
        {
            get
            {
                return (Model3D)GetValue(Labels3DContentProperty);
            }

            set
            {
                SetValue(Labels3DContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the content3D.
        /// </summary>
        /// <value>The content3D.</value>
        internal Model3D Content3D
        {
            get
            {
                return (Model3D)GetValue(Content3DProperty);
            }

            set
            {
                SetValue(Content3DProperty, value);
            }
        }

        /// <summary>
        /// Gets the primary series.
        /// </summary>
        /// <value>The primary series.</value>
        internal ChartSeries PrimarySeries
        {
            get
            {
                if (VisibleSeries != null)
                    return VisibleSeries.Count > 0 ? VisibleSeries[0] : null;
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating series that currently got a mouse pressed upon it.
        /// </summary>
        internal ChartSeries MousePressedSeries
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating series that currently got a mouse pointer above it.
        /// </summary>
        internal ChartSegment MouseEnteredSegment
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether chart area is currently zooming.
        /// </summary>
        public bool ZoomSwitched
        {
            get
            {
                return (bool)GetValue(ZoomSwitchedProperty);
            }
        }

        /// <summary>
        /// Gets or sets the axes thickness.
        /// </summary>
        /// <value>The axes thickness.</value>
        public Thickness AxesThickness
        {
            get
            {
                return (Thickness)GetValue(AxesThicknessProperty);
            }

            set
            {
                SetValue(AxesThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets type of area.
        /// </summary>
        internal ChartAxesType AreaType
        {
            get
            {
                return (ChartAxesType)this.GetValue(ChartArea.AreaTypeProperty);
            }

            set
            {
                SetValue(ChartArea.AreaTypeProperty, value);
            }
        }
        #endregion

        /// <summary>
        /// Gets or sets a value indicating whether default context menu should be added to <see cref="ChartArea"/>.
        /// </summary>
        /// <value>
        ///  <c>true</c> if <see cref="ChartArea "/> has  a default context menu enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsContextMenuEnabled
        {
            get { return (bool)GetValue(IsContextMenuEnabledProperty); }
            set { SetValue(IsContextMenuEnabledProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="ChartSeries"/> added to area should be plotted side-by-side. 
        /// </summary>
        /// <remarks>
        /// Gets or sets the SideBySideSeriesPlacement. This is a dependency property. Property affects side-by-side chart types only.
        /// </remarks>
        /// <value>The SideBySideSeriesPlacement.</value>
        public bool SideBySideSeriesPlacement
        {
            get { return (bool)GetValue(SideBySideSeriesPlacementProperty); }
            set { SetValue(SideBySideSeriesPlacementProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is retain axis position.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is retain axis position; otherwise, <c>false</c>.
        /// </value>
        public bool IsRetainAxisPosition
        {
            get { return (bool)GetValue(IsRetainAxisPositionProperty); }
            set { SetValue(IsRetainAxisPositionProperty, value); }
        }

        /// <summary>
        /// Gets the chart area adorner layer.
        /// </summary>
        /// <value>The chart area adorner layer.</value>
        public AdornerLayer ChartAreaAdornerLayer
        {
            get
            {
                if (m_areaPresenter != null)
                    return AdornerLayer.GetAdornerLayer(m_areaPresenter);
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets or sets the camera3 D.
        /// </summary>
        /// <value>The camera3 D.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Camera Camera3D
        {
            get
            {
                return (Camera)GetValue(Camera3DProperty);
            }

            set
            {
                SetValue(Camera3DProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the chart3D settings.
        /// </summary>
        /// <value>The chart3D settings.</value>
        public Chart3D Chart3DSettings
        {
            get
            {
                return (Chart3D)GetValue(Chart3DSettingsProperty);
            }

            set
            {
                SetValue(Chart3DSettingsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the content of the light.
        /// </summary>
        /// <value>The content of the light.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Model3D LightContent
        {
            get
            {
                return (Model3D)GetValue(LightContentProperty);
            }

            set
            {
                SetValue(LightContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the content of the grid.
        /// </summary>
        /// <value>The content of the grid.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Model3D GridContent
        {
            get
            {
                return (Model3D)GetValue(GridContentProperty);
            }

            set
            {
                SetValue(GridContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets alternating grid lines direction.
        /// </summary>
        /// <remarks>
        /// In some cases it is required to fill charts area's background with repeating
        /// lines. This property allows to set alternating lines direction.
        /// </remarks>
        /// <value>
        /// The alternating fill mode can be set from one of values from <see
        /// cref="Orientation" /> enumeration.
        /// </value>
        /// <example>
        /// C#: <code language="C#">
        /// //Creating new chart instance.
        /// Chart chart = new Chart();
        /// //Adding new area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating chart data points.
        /// ChartListData data = new ChartListData();
        /// data.Add(new ChartPoint(1, 1));
        /// data.Add(new ChartPoint(2, 2));
        /// data.Add(new ChartPoint(3, 3));
        /// data.Add(new ChartPoint(4, 4));
        /// data.Add(new ChartPoint(5, 5));
        /// //Adding new series.
        /// chart.Areas[0].Series.Add(new ChartSeries());
        /// //Assigning data to series.
        /// chart.Areas[0].Series[0].Data = data;
        /// //Setting alternating grid background brush.
        /// chart.Areas[0].AlternatingGridBackground = Brushes.White;
        /// //Setting alternating grid background direction mode.
        /// chart.Areas[0].AlternatingFillDirection = Orientation.Vertical;
        /// </code> XAML: <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Width="300" Height="300"&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea AlternatingGridBackground="White"
        /// AlternatingFillDirection="Vertical"&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3 4 4 5 5 "/&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        public Orientation AlternatingFillDirection
        {
            get
            {
                return (Orientation)GetValue(AlternatingFillDirectionProperty);
            }

            set
            {
                SetValue(AlternatingFillDirectionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets fill mode for alternating grid lines.
        /// </summary>
        /// <remarks>
        /// In some cases it is required to fill charts area's background with repeating
        /// lines. This property allows to set alternating lines fill mode.
        /// </remarks>
        /// <value>
        /// The alternating fill mode can be set from one of values from <see
        /// cref="AlternatingFillMode" /> enumeration.
        /// </value>
        /// <example>
        /// C#: <code language="C#">
        /// //Creating new chart instance.
        /// Chart chart = new Chart();
        /// //Adding new area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating chart data points.
        /// ChartListData data = new ChartListData();
        /// data.Add(new ChartPoint(1, 1));
        /// data.Add(new ChartPoint(2, 2));
        /// data.Add(new ChartPoint(3, 3));
        /// data.Add(new ChartPoint(4, 4));
        /// data.Add(new ChartPoint(5, 5));
        /// //Adding new series.
        /// chart.Areas[0].Series.Add(new ChartSeries());
        /// //Assigning data to series.
        /// chart.Areas[0].Series[0].Data = data;
        /// //Setting alternating grid background brush.
        /// chart.Areas[0].AlternatingGridBackground = Brushes.White;
        /// //Setting alternating grid background fill mode.
        /// chart.Areas[0].AlternatingFillMode = AlternatingFillMode.Odd;
        /// </code> XAML: <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Width="300" Height="300"&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea AlternatingGridBackground="White"
        /// AlternatingFillMode="Odd"&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3 4 4 5 5 "/&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        public AlternatingFillMode AlternatingFillMode
        {
            get
            {
                return (AlternatingFillMode)GetValue(AlternatingFillModeProperty);
            }

            set
            {
                SetValue(AlternatingFillModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets alternating grid lines brush.
        /// </summary>
        /// <remarks>
        /// In some cases it is required to fill charts area's background with repeating
        /// lines. This property allows to set alternating lines brush.
        /// </remarks>
        /// <value>
        /// Alternating grid background.
        /// </value>
        /// <example>
        /// C#: <code language="C#">
        /// //Creating new chart instance.
        /// Chart chart = new Chart();
        /// //Adding new area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating chart data points.
        /// ChartListData data = new ChartListData();
        /// data.Add(new ChartPoint(1, 1));
        /// data.Add(new ChartPoint(2, 2));
        /// data.Add(new ChartPoint(3, 3));
        /// data.Add(new ChartPoint(4, 4));
        /// data.Add(new ChartPoint(5, 5));
        /// //Adding new series.
        /// chart.Areas[0].Series.Add(new ChartSeries());
        /// //Assigning data to series.
        /// chart.Areas[0].Series[0].Data = data;
        /// //Setting alternating grid background brush.
        /// chart.Areas[0].AlternatingGridBackground = Brushes.White;
        /// </code> XAML: <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Width="300" Height="300"&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;!--Creates new chart area and assigns its AlternatingGridBackground
        /// property--&gt;
        /// &lt;syncfusion:ChartArea AlternatingGridBackground="White"&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3 4 4 5 5 "/&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        public Brush AlternatingGridBackground
        {
            get
            {
                return (Brush)GetValue(AlternatingGridBackgroundProperty);
            }

            set
            {
                SetValue(AlternatingGridBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets <see cref="ChartLegend" /> for ChartArea. This is a dependency
        /// property.
        /// </summary>
        /// <remarks>
        /// Chart legend is used to give user more idea about what he sees on the chart.
        /// <para> <see cref="ChartLegend">Chart Legend's</see> values are displaying
        /// series' labels except of <see cref="ChartTypes.Pie" />, <see
        /// cref="ChartTypes.Doughnut" />, <see cref="ChartTypes.Funnel" /> and <see
        /// cref="ChartTypes.Pyramid" /> chart types. For such specific types legend is
        /// displaying segments of series. </para>
        /// </remarks>
        /// <value>
        /// Instance of <see cref="ChartLegend" /> should be assigned.
        /// </value>
        /// <example>
        /// C#: <code language="C#">
        /// //Creating new chart instance.
        /// Chart chart = new Chart();
        /// //Adding new area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating chart data points.
        /// ChartListData data = new ChartListData();
        /// data.Add(new ChartPoint(1, 1));
        /// data.Add(new ChartPoint(2, 2));
        /// data.Add(new ChartPoint(3, 3));
        /// data.Add(new ChartPoint(4, 4));
        /// data.Add(new ChartPoint(5, 5));
        /// //Adding new series.
        /// chart.Areas[0].Series.Add(new ChartSeries());
        /// //Assigning data to series.
        /// chart.Areas[0].Series[0].Data = data;
        /// //Assigning legend to area.
        /// chart.Areas[0].Legend = new ChartLegend();
        /// </code> XAML: <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// Width="300" Height="300"&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3 4 4 5 5 "/&gt;
        /// &lt;syncfusion:ChartArea.Legend&gt;
        /// &lt;syncfusion:ChartLegend/&gt;
        /// &lt;/syncfusion:ChartArea.Legend&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ChartLegend Legend
        {
            get
            {
                return (ChartLegend)GetValue(LegendProperty);
            }

            set
            {
                SetValue(LegendProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [view3 D mode].
        /// </summary>
        /// <value><c>true</c> if [view3 D mode]; otherwise, <c>false</c>.</value>
        public bool View3DMode
        {
            get
            {
                return (bool)GetValue(View3DModeProperty);
            }

            set
            {
                SetValue(View3DModeProperty, value);
            }
        }


        /// <summary>
        /// Get and set EnableDepthAxis property
        /// </summary>
        public bool EnableDepthAxis
        {
            get { return (bool)GetValue(EnableDepthAxisProperty); }
            set { SetValue(EnableDepthAxisProperty, value); }
        }


        

        /// <summary>
        /// Gets or sets horizontal scrolling axis. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// When chart area gets zoomed, horizontal scrollbar is shown. This property determines which axis should be scrolled.
        /// <para>
        /// HorizontalScrollingAxis is generally used for multiple axes scenarios.
        /// </para>
        /// </remarks>
        /// <value>
        /// <see cref="ChartAxis"/> should be a member of chart area <see cref="ChartArea.Axes"/> collection in order to work properly.
        /// </value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal ChartAxis HorizontalScrollingAxis
        {
            get
            {
                return (ChartAxis)GetValue(HorizontalScrollingAxisProperty);
            }

            set
            {
                SetValue(HorizontalScrollingAxisProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets vertical scrolling axis. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// When chart area gets zoomed, vertical scrollbar is shown. This property determines which axis should be scrolled.
        /// <para>
        /// VerticalScrollingAxis is generally used for multiple axes scenarios.
        /// </para>
        /// </remarks>
        /// <value>
        /// <see cref="ChartAxis"/> should be a member of chart area <see cref="ChartArea.Axes"/> collection in order to work properly.
        /// </value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal ChartAxis VerticalScrollingAxis
        {
            get
            {
                return (ChartAxis)GetValue(VerticalScrollingAxisProperty);
            }

            set
            {
                SetValue(VerticalScrollingAxisProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether all axes should be zoomed in zooming mode. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// This property is being changed automatically by WPF Chart zooming system.
        /// </remarks>
        /// <value><c>true</c> if all area's axes should be zoomed. All <see cref="ChartSeries.IsZoomable"/> properties are ignored.</value>
        public bool ZoomAllAxes
        {
            get
            {
                return (bool)GetValue(ZoomAllAxesProperty);
            }

            set
            {
                SetValue(ZoomAllAxesProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the grid background. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Represents the brush that fills area's grid background.
        /// </remarks>
        /// <value>
        /// The grid <see cref="Brush">background</see>.
        /// </value>
        /// <example>
        /// C#: <code>
        /// //Creating new chart instance.
        /// Chart chart = new Chart();
        /// //Adding new area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating chart data points.
        /// ChartListData data = new ChartListData();
        /// data.Add(new ChartPoint(1, 1));
        /// data.Add(new ChartPoint(2, 2));
        /// data.Add(new ChartPoint(3, 3));
        /// data.Add(new ChartPoint(4, 4));
        /// data.Add(new ChartPoint(5, 5));
        /// //Adding new series.
        /// chart.Areas[0].Series.Add(new ChartSeries());
        /// //Assigning data to series.
        /// chart.Areas[0].Series[0].Data = data;
        /// //Assigning a new background brush.
        /// chart.Areas[0].GridBackground = new LinearGradientBrush(Colors.White, Colors.Red, new Point(0, 0), new Point(1, 1));
        /// </code> XAML: <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// Width="300" Height="300"&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:Chart.Resources&gt;
        /// &lt;LinearGradientBrush x:Key="backgroundBrush"&gt;
        /// &lt;GradientStop Color="White" Offset="0"/&gt;
        /// &lt;GradientStop Color="LightCoral" Offset="0.6"/&gt;
        /// &lt;GradientStop Color="White" Offset="1"/&gt;
        /// &lt;/LinearGradientBrush&gt;
        /// &lt;/syncfusion:Chart.Resources&gt;
        /// &lt;syncfusion:ChartArea GridBackground="{StaticResource backgroundBrush}"&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3 4 4 5 5 "/&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        public Brush GridBackground
        {
            get
            {
                return (Brush)GetValue(GridBackgroundProperty);
            }

            set
            {
                SetValue(GridBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the primary axis. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// By default, chart area has two axes: Primary (X-axis with <see cref="Orientation.Horizontal"/> orientation) and Secondary (Y-Axis with <see cref="Orientation.Vertical"/> orientation) axes.
        /// </remarks>
        /// <value>The primary axis.</value>
        /// <seealso cref="ChartAxis"/>
        /// <seealso cref="ChartArea.Axes">Chart area axes collection</seealso>
        public ChartAxis PrimaryAxis
        {
            get
            {
                return (ChartAxis)GetValue(PrimaryAxisProperty);
            }

            set
            {
                SetValue(PrimaryAxisProperty, value);
            }
        }




        /// <summary>
        /// Gets or sets the secondary axis.This is a dependency property.
        /// </summary>
        /// <remarks>
        /// By default, chart area has two axes: Primary (X-axis with <see cref="Orientation.Horizontal"/> orientation) and Secondary (Y-Axis with <see cref="Orientation.Vertical"/> orientation) axes.
        /// </remarks>
        /// <value>The secondary axis.</value>
        /// <seealso cref="ChartAxis"/>
        /// <seealso cref="ChartArea.Axes">Chart area axes collection</seealso>
        public ChartAxis SecondaryAxis
        {
            get
            {
                return (ChartAxis)GetValue(SecondaryAxisProperty);
            }

            set
            {
                SetValue(SecondaryAxisProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Depth(Z) axis.This is a dependency property.
        /// </summary>
        /// <remarks>
        /// For 3D Chart, chart area has three axes: Primary (X-axis with <see cref="Orientation.Horizontal"/> orientation) , Secondary (Y-Axis with <see cref="Orientation.Vertical"/> orientation) and Depth (Z-Axis) axes.
        /// </remarks>
        /// <value>The depth axis.</value>
        /// <seealso cref="ChartAxis"/>
        /// <seealso cref="ChartArea.Axes">Chart area axes collection</seealso>
        public ChartAxis DepthAxis
        {
            get
            {
                return (ChartAxis)GetValue(DepthAxisProperty);
            }

            set
            {
                SetValue(DepthAxisProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets corner radius of area's border. This is a dependency property.
        /// </summary>
        /// <example>
        /// C#: <code language="C#">
        /// //Creating new chart instance.
        /// Chart chart = new Chart();
        /// //Adding new area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating chart data points.
        /// ChartListData data = new ChartListData();
        /// data.Add(new ChartPoint(1, 1));
        /// data.Add(new ChartPoint(2, 2));
        /// data.Add(new ChartPoint(3, 3));
        /// data.Add(new ChartPoint(4, 4));
        /// data.Add(new ChartPoint(5, 5));
        /// //Adding new series.
        /// chart.Areas[0].Series.Add(new ChartSeries());
        /// //Assigning data to series.
        /// chart.Areas[0].Series[0].Data = data;
        /// //Setting corner radius of area.
        /// chart.Areas[0].CornerRadius = new CornerRadius(10, 5, 10, 5);
        /// </code> XAML: <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Width="300" Height="300"&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea CornerRadius="10,5,10,5"&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3 4 4 5 5 "/&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        public CornerRadius CornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(ChartArea.CornerRadiusProperty);
            }

            set
            {
                SetValue(ChartArea.CornerRadiusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the collection of <see cref="ChartSeries"/>.
        /// </summary>
        /// <example>
        /// XAML:
        /// <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        ///    Width="300" Height="300"&gt;
        /// &lt;syncfusion:Chart xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        ///    &lt;syncfusion:ChartArea&gt;
        ///        &lt;syncfusion:ChartArea.Series&gt;
        ///            &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3 4 4 5 5"/&gt;
        ///        &lt;/syncfusion:ChartArea.Series&gt;
        ///    &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        /// <value>The series.</value>
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ChartSeriesCollection Series
        {
            get
            {
                return m_series;
            }
            set
            {
                m_series = value;
            }
        }

        /// <summary>
        /// Gets the color model of area.
        /// </summary>
        /// <value>The color model.</value>
        /// <seealso cref="ChartStyleModel"/>
        public ChartStyleModel ColorModel
        {
            get { return (ChartStyleModel)GetValue(ColorModelProperty); }
            set { SetValue(ColorModelProperty, value); }
        }

        
       /// <summary>
        ///  Using a DependencyProperty as the backing store for ColorModel.  This enables animation, styling, binding, etc...
       /// </summary>
        public static readonly DependencyProperty ColorModelProperty =
            DependencyProperty.Register("ColorModel", typeof(ChartStyleModel), typeof(ChartArea), new FrameworkPropertyMetadata(new ChartStyleModel(),FrameworkPropertyMetadataOptions.AffectsRender));


        ///// <summary>
        ///// Gets the color model of area.
        ///// </summary>
        ///// <value>The color model.</value>
        ///// <seealso cref="ChartStyleModel"/>
        //public ChartStyleModel ColorModel
        //{
        //    get
        //    {
        //        return m_colorModel;
        //    }
        //    set
        //    {
        //        m_colorModel = value;
        //    }
        //}

        internal bool IsMouseDragZooming
        {
            get
            {
                return m_isMouseDragZooming;
            }
            set
            {
                m_isMouseDragZooming = value;
            }
        }

        /// <summary>
        /// Gets or sets header of chart. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Header is used to mark area.
        /// </remarks>
        /// <example>
        /// C#: <code language="C#">
        /// //Creating new chart instance.
        /// chart chart = new chart();
        /// //Adding new area.
        /// chart.areas.add(new chartarea());
        /// //Creating chart data points.
        /// chartlistdata data = new chartlistdata();
        /// data.add(new chartpoint(1, 1));
        /// data.add(new chartpoint(2, 2));
        /// data.add(new chartpoint(3, 3));
        /// data.add(new chartpoint(4, 4));
        /// data.add(new chartpoint(5, 5));
        /// //Adding new series.
        /// chart.areas[0].series.add(new chartseries());
        /// //Assigning data to series.
        /// chart.areas[0].series[0].data = data;
        /// //Setting corner radius of area.
        /// chart.areas[0].Header = "Main area";
        /// </code> XAML: <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Width="300" Height="300"&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea Header="Main Area"&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3 4 4 5 5 "/&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        public object Header
        {
            get
            {
                return this.GetValue(ChartArea.HeaderProperty);
            }

            set
            {
                this.SetValue(ChartArea.HeaderProperty, value);
            }
        }



        /// <summary>
        /// Get and set GridHeaderProperty
        /// </summary>
        public object GridHeader
        {
            get { return (object)GetValue(GridHeaderProperty); }
            set { SetValue(GridHeaderProperty, value); }
        }

        
       /// <summary>
        /// Using a DependencyProperty as the backing store for GridHeader.  This enables animation, styling, binding, etc...
       /// </summary>
        public static readonly DependencyProperty GridHeaderProperty =
            DependencyProperty.Register("GridHeader", typeof(object), typeof(ChartArea), new UIPropertyMetadata(null));


        /// <summary>
        /// Get and Set GridHeaderAlignmentProperty
        /// </summary>
        public HorizontalAlignment GridHeaderAlignment
        {
            get { return (HorizontalAlignment)GetValue(GridHeaderAlignmentProperty); }
            set { SetValue(GridHeaderAlignmentProperty, value); }
        }

        
       /// <summary>
        /// Using a DependencyProperty as the backing store for GridHeaderAlignmentProperty.  This enables animation, styling, binding, etc...
       /// </summary>
        public static readonly DependencyProperty GridHeaderAlignmentProperty =
            DependencyProperty.Register("GridHeaderAlignment", typeof(HorizontalAlignment), typeof(ChartArea), new UIPropertyMetadata(HorizontalAlignment.Center));



     

		/// <summary>
        /// Gets or sets the footer of chart area. This is a dependency property.
        /// </summary>
        public object Footer
        {
            get
            {
                return this.GetValue(FooterProperty);
            }

            set
            {
                this.SetValue(FooterProperty, value);
            }
        }
        /// <summary>
        /// Gets the chart area axes collection.
        /// </summary>
        /// <remarks>
        /// This property should be used for multiple axes scenario.
        /// </remarks>
        /// <value>By default property contains 2 axes: <see cref="ChartArea.PrimaryAxis">Primary</see> and <see cref="ChartArea.SecondaryAxis">Secondary</see>.</value>
        /// <seealso cref="ChartSeries.XAxis">X Axis on Series</seealso>
        /// <seealso cref="ChartSeries.YAxis">Y Axis on Series</seealso>
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public ChartAxesCollection Axes
        {
            get
            {
                return m_axes;
            }
            private set
            {
 
            }
        }

        /// <summary>
        /// Gets visible series of chart area.
        /// </summary>
        /// <remarks>
        /// This property is intended to be used for custom <see cref="ChartArea"/> templates.
        /// </remarks>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public VisibleSeriesCollection VisibleSeries
        {
            get
            {
                return m_visibleSeries;
            }
        }

        /// <summary>
        /// Gets the minimal points delta.
        /// </summary>
        /// <value>The min points delta.</value>
        private double MinPointsDelta
        {
            get
            {
                if (double.IsNaN(m_minPointsDelta))
                {
                    ////double exMinPointsDelta = m_minPointsDelta;
                    m_minPointsDelta = double.MaxValue;

                    foreach (ChartSeries series in m_visibleSeries)
                    {
                        if (series.ChartType != null && series.ChartType.IsSideBySide)
                        {
                            double[] xValues = new double[series.PointsCount];

                            for (int i = 0; i < series.PointsCount; i++)
                            {
                                IChartDataPoint point = series.GetPoint(i);
                                if (point != null)
                                {
                                    xValues[i] = point.X;
                                    ////Making sure to set index of point if X is NaN.
                                    if (double.IsNaN(xValues[i]))
                                    {
                                        xValues[i] = i;
                                    }
                                }
                            }
                            if (!series.IsIndexed)
                            {
                                Array.Sort(xValues);
                            }
                            ////If sorting is required by user, perform sorting
                            //if (series.IsSortData == true)
                            //{
                            //    Array.Sort(xValues);
                            //    if (series.SortDirection == Direction.Descending)
                            //        Array.Reverse(xValues);
                            //}

                            for (int i = 1; i < xValues.Length; i++)
                            {
                                double delta = xValues[i] - xValues[i - 1];

                                if (delta != 0)
                                {
                                    m_minPointsDelta = Math.Min(m_minPointsDelta, delta);
                                }
                            }
                        }
                    }

                    if (m_exMinPointsDelta != m_minPointsDelta)
                    {
                        foreach (ChartSeries series in m_visibleSeries)
                        {
                            if (series.Segments != null)
                            {
                                series.Segments.Clear();
                                series.Adornments.Clear();
                                series.Invalidate();
                            }
                        }

                        m_exMinPointsDelta = m_minPointsDelta;
                    }
                }
                foreach (ChartSeries series in m_visibleSeries)
                {
                    if (series.Type == ChartTypes.Column && series.Area.PrimaryAxis.ValueType != ChartValueType.DateTime && m_minPointsDelta < 0)
                        m_minPointsDelta = (m_minPointsDelta == double.MaxValue || m_minPointsDelta >= 1 ? 1 : (m_minPointsDelta < 0 ? 1 : m_minPointsDelta));
                }

                m_minPointsDelta = (m_minPointsDelta == double.MaxValue || m_minPointsDelta >= 1 ? 1 : m_minPointsDelta);

                return m_minPointsDelta;
            }
        }

        /// <summary>
        /// Gets an enumerator for logical child elements of this element.
        /// </summary>
        /// <value></value>
        /// <returns>An enumerator for logical child elements of this element.</returns>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                if (m_series != null)
                {
                    return m_series.GetEnumerator();
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// Get and Set ZoomedXRangeProperty
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public DoubleRange ZoomedXRange
        {
            get
            {
                return (DoubleRange)GetValue(ZoomedXRangeProperty);
            }
            set
            {
                SetValue(ZoomedXRangeProperty, value);
            }
        }
        /// <summary>
        /// Get  and Set ZoomedYRangeProperty
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public DoubleRange ZoomedYRange
        {
            get
            {
                return (DoubleRange)GetValue(ZoomedYRangeProperty);
            }
            set
            {
                SetValue(ZoomedYRangeProperty, value);
            }
        }
        internal SyncChartAreas ChartAreaParent
        {
            get
            {
                return (SyncChartAreas)GetValue(ChartAreaParentProperty);
            }
            set
            {
                SetValue(ChartAreaParentProperty, value);
            }
        }
        internal int ChartAreaIndex
        {
            get { return (int)GetValue(ChartAreaIndexProperty); }
            set { SetValue(ChartAreaIndexProperty, value); }
        }
        internal int ChartAreaCount
        {
            get { return (int)GetValue(ChartAreaCountProperty); }
            set { SetValue(ChartAreaCountProperty, value); }
        }

        internal double SplitterDelta
        {
            get { return (double)GetValue(SplitterDeltaProperty); }
            set { SetValue(SplitterDeltaProperty, value); }
        }
        /// <summary>
        /// Get and Set SplitterVisiblityProperty
        /// </summary>
        public SpliterVisibility SplitterVisiblity
        {
            get { return (SpliterVisibility)GetValue(SplitterVisiblityProperty); }
            set { SetValue(SplitterVisiblityProperty, value); }
        }
        /// <summary>
        /// Get and Set SplitterWidthProperty
        /// </summary>
        public double SplitterWidth
        {
            get { return (double)GetValue(SplitterWidthProperty); }
            set { SetValue(SplitterWidthProperty, value); }
        }


        internal ChartArea selectedChartAreaForIC = null;
        internal ChartSeries selectedChartSeriesForIC = null;


        #endregion

        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="ChartArea"/> class.
        /// </summary>
        static ChartArea()
        {
            Syncfusion.Licensing.EnvironmentTest.ValidateLicense(typeof(ChartArea));
            ////This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            ////This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChartArea), new FrameworkPropertyMetadata(typeof(ChartArea)));
            //////Overriding context menu property metadata.
            ContextMenuProperty.OverrideMetadata(typeof(ChartArea), new FrameworkPropertyMetadata(null, null, OnCoerceContextMenu));
            ////ContextMenuProperty.GetMetadata(typeof(ChartArea)).CoerceValueCallback = new CoerceValueCallback(OnCoerceContextMenu);
            //// Commands
            CommandBinding zoomInBinding = new CommandBinding(ChartAreaCommands.ZoomIn, new ExecutedRoutedEventHandler(OnZoomInCommand), new CanExecuteRoutedEventHandler(CanExecuteZoomingCommands));
            CommandBinding zoomOutBinding = new CommandBinding(ChartAreaCommands.ZoomOut, new ExecutedRoutedEventHandler(OnZoomOutCommand), new CanExecuteRoutedEventHandler(CanExecuteZoomingCommands));
            CommandBinding zoomResetBinding = new CommandBinding(ChartAreaCommands.ZoomReset, new ExecutedRoutedEventHandler(OnZoomResetCommand), new CanExecuteRoutedEventHandler(CanExecuteZoomingCommands));
            CommandBinding zoomSectorBinding = new CommandBinding(ChartAreaCommands.ZoomSector, new ExecutedRoutedEventHandler(OnZoomSectorCommand), new CanExecuteRoutedEventHandler(CanExecuteZoomingCommands));
            CommandBinding switchZoomingBinding = new CommandBinding(ChartAreaCommands.SwitchZooming, new ExecutedRoutedEventHandler(OnSwitchZoomingCommand), new CanExecuteRoutedEventHandler(CanExecuteZoomingCommands));
            CommandBinding changePaletteBinding = new CommandBinding(ChartAreaCommands.ChangePalette, new ExecutedRoutedEventHandler(OnChangePaletteCommand));
            CommandBinding cancelZoomingBinding = new CommandBinding(ChartAreaCommands.CancelZooming, new ExecutedRoutedEventHandler(OnCancelZoomingCommand));
            CommandBinding zoomPanningBinding = new CommandBinding(ChartAreaCommands.ZoomPanning, new ExecutedRoutedEventHandler(OnZoomPaningCommand), new CanExecuteRoutedEventHandler(CanExecuteZoomingCommands));
            CommandBinding changeStyleBinding = new CommandBinding(ChartAreaCommands.ChangeStyle, new ExecutedRoutedEventHandler(OnchangeStyleCommand), new CanExecuteRoutedEventHandler(CanExecutechangeStyleCommand));


            CommandManager.RegisterClassCommandBinding(typeof(ChartArea), zoomInBinding);
            CommandManager.RegisterClassCommandBinding(typeof(ChartArea), zoomOutBinding);
            CommandManager.RegisterClassCommandBinding(typeof(ChartArea), zoomResetBinding);
            CommandManager.RegisterClassCommandBinding(typeof(ChartArea), zoomPanningBinding);
            CommandManager.RegisterClassCommandBinding(typeof(ChartArea), zoomSectorBinding);
            CommandManager.RegisterClassCommandBinding(typeof(ChartArea), switchZoomingBinding);
            CommandManager.RegisterClassCommandBinding(typeof(ChartArea), changePaletteBinding);
            CommandManager.RegisterClassCommandBinding(typeof(ChartArea), cancelZoomingBinding);
            CommandManager.RegisterClassCommandBinding(typeof(ChartArea), changeStyleBinding);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartArea"/> class.
        /// </summary>
        /// <remarks>
        /// Primary and secondary axes are being created automatically.
        /// </remarks>
        public ChartArea()
        {
            this.DefaultStyleKey = typeof(ChartArea);
            m_visibleSeries = new VisibleSeriesCollection(m_series);
            //FontFamilyDescriptor.AddValueChanged(this, new EventHandler(FontFamilyChanged));
            //FontSizeDescriptor.AddValueChanged(this, new EventHandler(FontSizeChanged));
            //FontWeightDescriptor.AddValueChanged(this, new EventHandler(FontWeightChanged));
            //ForegroundDescriptor.AddValueChanged(this, new EventHandler(ForegroundChanged));

            ////Subscribing for changes in visible series.
            m_visibleSeries.CollectionChanged += new NotifyCollectionChangedEventHandler(OnVisibleSeriesCollectionChanged);
            m_series.CollectionChanged += new NotifyCollectionChangedEventHandler(OnSeriesCollectionChanged);
            m_axes.CollectionChanged += new NotifyCollectionChangedEventHandler(OnAxesCollectionChanged);
            this.ColorModel.PropertyChanged += new PropertyChangedEventHandler(ColorModel_PropertyChanged);
            //m_colorModel = new ChartStyleModel(this, C_colorPaletteSize);
            this.isResizable = true;
            this.isLoadedFirst = true;

            this.SizeChanged += new SizeChangedEventHandler(ChartArea_SizeChanged);

            updateArea = false;
            this.CoerceValue(PrimaryAxisProperty);
            this.CoerceValue(SecondaryAxisProperty);
            //this.CoerceValue(DepthAxisProperty);
            updateArea = true;
            //if(this.ColorModel != null)
            //    BindingUtils.SetBinding(this, this.ColorModel, ChartArea.PaletteProperty, new PropertyPath("Palette"));
            //   UpdateArea();
            this.Loaded += new RoutedEventHandler(ChartArea_Loaded);
            InteractiveCursors.CollectionChanged += new NotifyCollectionChangedEventHandler(InteractiveCursorCollection_CollectionChanged);

            timer.Interval = new TimeSpan(0, 0, 0, 0, 250);
            timer.Tick += new EventHandler(timer_Tick);
        }
        //private ChartColorPalette colorPalette = ChartColorPalette.Gradient;
        void ColorModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

            if (e.PropertyName == "CurrentPalette")
            {
                if (this.Parent != null && this.ColorModel.Palette != ChartColorPalette.Custom)
                {
                    this.Palette = this.ColorModel.Palette;
                    this.ColorModel = new ChartStyleModel(this, C_colorPaletteSize);
                    this.ColorModel.Palette = this.Palette;
                    this.ColorModel.ApplyPalette(this.Palette);
                }
               
                if (this.Parent != null && this.ColorModel.Palette == ChartColorPalette.Custom && this.ColorModel.CustomPalette != null)
                {
                    /*SD13013- In the below code new ChartStyleModel instance is created with same values as before, has been assigned to the existing ColorModel Property.
                     This has been included because the segment/series interior is bind with the local CLR variable of ChartStyleModel class. 
                     Due to this the last area's custom palette will be applied for all areas*/                     
                    this.ColorModel.PropertyChanged -=new PropertyChangedEventHandler(ColorModel_PropertyChanged);
                    this.ColorModel = new ChartStyleModel(this.ColorModel.CustomPalette.Length) { m_brushes = this.ColorModel.m_brushes, Palette = this.ColorModel.Palette, CustomPalette = this.ColorModel.CustomPalette };
                    this.ColorModel.PropertyChanged += new PropertyChangedEventHandler(ColorModel_PropertyChanged);
                    foreach (ChartSeries series in this.Series)
                    {
                        series.Invalidate();
                    }
                }
            }
            //throw new NotImplementedException();
        }
        
        // private void FontFamilyChanged(object sender, EventArgs e)
        //{
        //    var area = sender as ChartArea;
        //    area.m_isFontFamilySet = true;
        //    //if (area.Header != null)
        //        //area.SetHeaderStyle();
        //}

        //private void FontWeightChanged(object sender, EventArgs e)
        //{
        //    ChartArea area = sender as ChartArea;
        //    area.m_isFontWeightSet = true;
        //   // if (area.Header != null)
        //       // area.SetHeaderStyle();
        //}

        //private void FontSizeChanged(object sender, EventArgs e)
        //{
        //    ChartArea area = sender as ChartArea;
        //    area.m_isFontSizeSet = true;
        //    //if (area.Header != null)
        //    //    area.SetHeaderStyle();
        //}

        //private void ForegroundChanged(object sender, EventArgs e)
        //{
        //    ChartArea area = sender as ChartArea;
        //    area.m_isForegroundSet = true;
        //    //if (area.Header != null)
        //    //{
        //    //    area.SetHeaderStyle();                
        //    //}
        //}

        void ChartArea_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ChartArea area = sender as ChartArea;
            if (area != null)
            {
                if (area.Series != null)
                {
                    foreach (ChartSeries ser in area.Series)
                    {
                        if (ser.Presenter != null)
                        ser.Presenter.InvalidateArrange();
                        if (ser.Indicators != null && ser.Presenter != null && ser.Presenter.m_IndicatorPresenter != null)
                        {
                            ser.Presenter.m_IndicatorPresenter.InvalidateVisual();
                        }
                    }
                }

                if (this.InteractiveCursors != null)
                {
                    foreach (InteractiveCursor cursor in this.InteractiveCursors)
                    {
                        if (EnableRangeSelection == false)
                        {
                            cursor.SetValueForInteractiveCursor(true);
                        }
                    }
                }
                if (EnableRangeSelection == true)
                {
                    foreach (ChartAxis axis in area.Axes)
                    {
                        if (area.RangeSelectionOrientation == Orientation.Vertical && axis.Orientation == Orientation.Horizontal)
                        {
                            this.BeginInit();
                            SetValueForEndSelectionRange(area, area.PrimaryAxis);
                            SetValueForStartSelectionRange(area, area.PrimaryAxis);
                            this.EndInit();
                        }
                        else if(area.RangeSelectionOrientation == Orientation.Horizontal && axis.Orientation == Orientation.Vertical)
                        {
                            this.BeginInit();
                            SetValueForBottomSelectionRange(area, area.SecondaryAxis);
                            SetValueForTopSelectionRange(area, area.SecondaryAxis);
                            this.EndInit();
                        }

                    }
                }
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ChartArea"/> class for designer.
        /// </summary>
        /// <param name="withDefaultSeries">if set to <c>true</c> [with default series].</param>
        [Obsolete("Use the default constructor of chart area")]
        public ChartArea(bool withDefaultSeries)
            : base()
        {
            #region SetData
            if (withDefaultSeries == true)
            {
                this.Series.Add(new ChartSeries());
                this.Series.Add(new ChartSeries());

                int i = 5;
                ArrayList a = new ArrayList(i);
                a.Add(0.1);
                a.Add(0.2);
                a.Add(0.3);
                a.Add(0.4);
                a.Add(0.5);
                a.Add(0.8);

                this.Series[0].DataSource = a;
                this.Series[0].ClearValue(ChartSeries.XAxisProperty);
                this.Series[0].ClearValue(ChartSeries.YAxisProperty);

                a = new ArrayList(i);
                a.Add(0.2);
                a.Add(0.4);
                a.Add(0.1);
                a.Add(0.5);
                a.Add(0.6);
                a.Add(0.3);

                this.Series[1].DataSource = a;
                this.Series[1].ClearValue(ChartSeries.XAxisProperty);
                this.Series[1].ClearValue(ChartSeries.YAxisProperty);

                this.Series[0].ClearValue(ChartSeries.DataProperty);
                this.Series[1].ClearValue(ChartSeries.DataProperty);


                this.UpdateArea();
            }
            #endregion



        }

        void InteractiveCursorCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            InteractiveCursorCollection obj = sender as InteractiveCursorCollection;
            foreach (InteractiveCursor ic in InteractiveCursors)
                ic.chartarea = this;

        }


        internal bool updateArea = true;
        internal bool InterupdateArea = true;
        internal double SplitterBottomSpace = 0d;

        void ChartArea_Loaded(object sender, RoutedEventArgs e)
        {
            if (EnableLazyLoading == true)
            {
                Cursor previousCursor = Mouse.OverrideCursor;
                Dispatcher.BeginInvoke(new Action(delegate
                {

                    Mouse.OverrideCursor = Cursors.Wait;
                    updateArea = true;
                    this.UpdateArea();
                    Mouse.OverrideCursor = previousCursor;
                }));

            }

            this.BottomSpace = this.AxesThickness.Bottom;
            if (this is SyncChartAreas)
            {
                foreach (ChartArea area in (this as SyncChartAreas).Areas)
                {
                    area.SplitterBottomSpace = this.AxesThickness.Bottom;
                }
            }
        }

        /// <summary>
        /// Starts the initialization process for this element.
        /// </summary>
        public new void BeginInit()
        {
            //var series = m_series;
            updateArea = false;
            //Parallel.For(0, series.Count, i => { series[i].BeginUpdate(); });
            //Commented for performance fix
            //for (int i = 0; i < series.Count; i++)
            //{
            //    series[i].BeginUpdate();
            //}
        }

        internal void InternalBeginInit()
        {
            InterupdateArea = false;            
        }

        /// <summary>
        /// Indicates that the initialization process for the element is complete.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">
        /// 	<see cref="M:System.Windows.FrameworkElement.EndInit"/> was called without <see cref="M:System.Windows.FrameworkElement.BeginInit"/> having previously been called on the element.
        /// </exception>
        public new void EndInit()
        {
            //var series = m_series;
            //Parallel.For(0, series.Count, i => { series[i].EndUpdate(); });
            //Commented for performance 
            //for (int i = 0; i < m_series.Count; i++)
            //{
            //    m_series[i].EndUpdate();

            //}
            updateArea = true;
            UpdateArea();
            
        }
        #endregion

        #region Public methods

        ObservableCollection<object> m_BoundsDataSource = null;
        Chart m_Chart = null;
        ObservableCollection<IChartDataPoint> m_BoundsPoints = null;

        /// <summary>
        /// Get the collection of underlying objects which exist inside given Rect region
        /// </summary>
        /// <param name="rect">Bounds Region</param>
        /// <param name="series">Chart Series</param>
        /// <returns>Collection of underlying objects</returns>
        public ObservableCollection<object> BoundsToDataSource(Rect rect, ChartSeries series)
        {
            m_BoundsDataSource = new ObservableCollection<object>();

            foreach (object obj in (from result in BoundsToPoints(rect, series) select result.Item))
            {
                m_BoundsDataSource.Add(obj);
            }

            return m_BoundsDataSource;
        }

        /// <summary>
        /// Get the collection of ChartPoints objects which exist inside given Rect region
        /// </summary>
        /// <param name="rect">Rect</param>
        /// <param name="series">Chart Series</param>
        /// <returns>Collection of ChartPoints</returns>
        public ObservableCollection<IChartDataPoint> BoundsToPoints(Rect rect, ChartSeries series)
        {
            if (series != null && series.XAxis != null && series.YAxis != null)
            {
                m_BoundsPoints = new ObservableCollection<IChartDataPoint>();
                Rect axisRangeRect = this.ConvertBoundsToAxesRangeValues(series.XAxis, series.YAxis, rect);
                IChartData data = series.Data;
                if (data != null)
                {
                    for (int i = 0; i < data.Count; i++)
                    {
                        IChartDataPoint point = series.GetPoint(i);
                        if (axisRangeRect.Left <= point.X && axisRangeRect.Right >= point.X &&
                            axisRangeRect.Top <= point.Y && axisRangeRect.Bottom >= point.Y)
                        {
                            m_BoundsPoints.Add(point);
                        }
                    }

                    return m_BoundsPoints;
                }
            }

            return new ObservableCollection<IChartDataPoint>();
        }

        /// <summary>
        /// Get the Parent chart of correspoinding Chart Area
        /// </summary>
        /// <param name="obj">Dependency object</param>
        /// <returns>Chart</returns>
        Chart GetChart(DependencyObject obj)
        {
            while ((obj is Chart) != true && obj != null)
            {
                obj = VisualTreeHelper.GetParent(obj);
            }

            return obj as Chart;
        }

        /// <summary>
        /// Convert the Mouse co-oridianate points to ChartAxis range values.
        /// </summary>
        /// <param name="xAxis">Chart Series X-Axis</param>
        /// <param name="yAxis">Chart Series Y-Axis</param>
        /// <param name="actualRect">Acutal Mouse rect Points</param>
        /// <returns>Range Rect value</returns>
        public Rect ConvertBoundsToAxesRangeValues(ChartAxis xAxis, ChartAxis yAxis, Rect actualRect)
        {
            Rect result;

            if (xAxis == null || yAxis == null)
            {
                return new Rect();
            }

            if (m_Chart == null)
            {
                m_Chart = GetChart(this);
            }
            else if (m_Chart != null)
            {
                actualRect = new Rect(m_Chart.TranslatePoint(actualRect.TopLeft, this), m_Chart.TranslatePoint(actualRect.BottomRight, this));
            }

            double startX = this.PointToValue(xAxis, actualRect.TopLeft);
            double endX = this.PointToValue(xAxis, actualRect.BottomRight);
            double startY = this.PointToValue(yAxis, actualRect.BottomRight);
            double endY = this.PointToValue(yAxis, actualRect.TopLeft);
            result = new Rect(new Point(startX, startY), new Point(endX, endY));

            return result;
        }

        /// <summary>
        /// Event that is raised when View3DMode property is changed.
        /// </summary>
        public event PropertyChangedCallback View3DModeChanged;

        /// <summary>
        /// Calls OnView3DModeChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnView3DModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartArea instance = (ChartArea)d;

            if ((bool)e.NewValue == true)
            {
                instance.PrimaryAxis.LabelFontSize += 6;
                instance.SecondaryAxis.LabelFontSize += 6;
                if (instance.DepthAxis != null)
                    instance.DepthAxis.LabelFontSize += 6;
            }
            else
            {
                if (instance.PrimaryAxis != null && instance.SecondaryAxis != null)
                {
                    instance.PrimaryAxis.LabelFontSize -= 6;
                    instance.SecondaryAxis.LabelFontSize -= 6;
                    if (instance.DepthAxis != null)
                        instance.DepthAxis.LabelFontSize -= 6;
                }
            }

            if (instance.m_areaPresenter != null)
            {
                instance.m_areaPresenter.InvalidateTemplate();
            }

            instance.OnView3DModeChanged(e);
        }

        /// <summary>
        /// Coerces the chart3D settings property.
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="baseValue">The base value.</param>
        /// <returns>The base value</returns>
        private static object CoerceChart3DSettingsProperty(DependencyObject d, object baseValue)
        {
            return baseValue ?? new Chart3D();
        }

        /// <summary>
        /// Gets or sets the camera controller.
        /// </summary>
        /// <value>The camera controller.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ChartTargetCameraController CameraController
        {
            get
            {
                return m_chartTargetCameraController;
            }

            set
            {
                m_chartTargetCameraController = value;
            }
        }

        /// <summary>
        /// Updates property value cache and raises View3DModeChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnView3DModeChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                OnZoomResetCommand(this, null);
                OnCancelZoomingCommand(this, null);
                updateArea = false;
                CoerceValue(DepthAxisProperty);
                DepthAxis.depthaxisflag = true;
                DepthAxis.Orientation = Orientation.Horizontal;
				updateArea = true;
                CoerceValue(Chart3DSettingsProperty);
            }
            else
            {
                this.ClearValue(DepthAxisProperty);
            }

            this.UpdateArea();

            if (View3DModeChanged != null)
            {
                View3DModeChanged(this, e);
            }

            if (!(bool)e.NewValue)
            {
                foreach (ChartSeries series in m_visibleSeries)
                {
                    series.Recalculate3D();
                }
            }
        }

        private static void OnEnableDepthAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartArea instance = (ChartArea)d;
            if (instance != null && (bool)args.NewValue)
            {
                foreach (ChartSeries series in instance.Series)
                {
                    series.Invalidate();
                }
            }
            instance.UpdateArea();
        }
        /// <summary>
        /// Method for hooking the Chartscrolling event
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="parentAxis"></param>
        public void Clone(ChartAxis axis, ChartAxis parentAxis)
        {
            if(parentAxis.Area.ChartScrolling!=null)
            axis.Area.ChartScrolling += parentAxis.Area.ChartScrolling;
        }

        internal double _backWallThickness = 0d;
        /// <summary>
        /// Creates the content of the axis.
        /// </summary>
        internal void CreateAxisContent()
        {
            Model3DGroup group = new Model3DGroup();
            Chart3D settings = this.Chart3DSettings ?? new Chart3D();
            MaterialGroup materialGroup;

            GeometryModel3D geometryBottomAxis = new GeometryModel3D();
            GeometryModel3D geometryLeftAxis = new GeometryModel3D();
            GeometryModel3D geometryRightAxis = new GeometryModel3D();
            GeometryModel3D geometryTopAxis = new GeometryModel3D();
            GeometryModel3D geometryBackAxis = new GeometryModel3D();

            double bottomShift;
            double leftShift;
            double topShift;
            double rightShift;

            double margin = 0.01;

            bottomShift = settings.ReadLocalValue(Chart3D.BottomWallThicknessProperty) == DependencyProperty.UnsetValue ? m_axes3DShiftLeftOrientation.X + margin : settings.BottomWallThickness;
            leftShift = settings.ReadLocalValue(Chart3D.LeftWallThicknessProperty) == DependencyProperty.UnsetValue ? m_axes3DShiftLeftOrientation.Y + margin : settings.LeftWallThickness;
            topShift = settings.ReadLocalValue(Chart3D.TopWallThicknessProperty) == DependencyProperty.UnsetValue ? m_axes3DShiftRightOrientation.X + margin : settings.TopWallThickness;
            rightShift = settings.ReadLocalValue(Chart3D.RightWallThicknessProperty) == DependencyProperty.UnsetValue ? m_axes3DShiftRightOrientation.Y + margin : settings.RightWallThickness;

            bottomShift = settings.ShowBottomWall ? bottomShift : 0d;
            topShift = settings.ShowTopWall ? topShift : 0d;
            leftShift = settings.ShowLeftWall ? leftShift : 0d;
            rightShift = settings.ShowRightWall ? rightShift : 0d;

            margin = 0d;

            if (settings.ShowLeftWall && settings.LeftWallThickness != 0d)
            {
                materialGroup = new MaterialGroup();
                materialGroup.Children.Add(new DiffuseMaterial(settings.LeftWallBackground));
                if (!this.isClustered)
                    geometryLeftAxis.Geometry = MeshGenerator.ColumnParallelotope(leftShift + margin, 1, 0.30 * this.Series.Count);
                else
                    geometryLeftAxis.Geometry = MeshGenerator.ColumnParallelotope(leftShift + margin, 1, this.EnableDepthAxis ? 0.999 : 0.30);
                geometryLeftAxis.Material = materialGroup;
                geometryLeftAxis.BackMaterial = materialGroup;

                if (!this.IsClustered && !this.EnableDepthAxis)
                    geometryLeftAxis.Transform = new TranslateTransform3D(-0.501 - leftShift / 2 - margin / 2, 0, 0.15);
                else
                    geometryLeftAxis.Transform = new TranslateTransform3D(-0.501 - leftShift / 2 - margin / 2, 0, this.EnableDepthAxis ? +0.500 : 0.15);

                group.Children.Add(geometryLeftAxis);
                ////LeftShift += margin;
            }

            if (settings.ShowBottomWall && settings.BottomWallThickness != 0d)
            {
                materialGroup = new MaterialGroup();
                materialGroup.Children.Add(new DiffuseMaterial(settings.BottomWallBackground));
                if (!this.isClustered)
                    geometryBackAxis.Geometry = MeshGenerator.ColumnParallelotope(1 + margin * 2 + leftShift + rightShift, bottomShift + margin, 0.30 * this.Series.Count);
                else
                    geometryBackAxis.Geometry = MeshGenerator.ColumnParallelotope(1 + margin * 2 + leftShift + rightShift, bottomShift + margin, this.EnableDepthAxis ? 0.999 : 0.30);
                geometryBackAxis.Material = materialGroup;
                geometryBackAxis.BackMaterial = materialGroup;

                if (!this.IsClustered)
                    geometryBackAxis.Transform = new TranslateTransform3D(-leftShift / 2 + rightShift / 2, -0.502 - bottomShift / 2 - margin / 2, 0.15);
                else
                    geometryBackAxis.Transform = new TranslateTransform3D(-leftShift / 2 + rightShift / 2, -0.502 - bottomShift / 2 - margin / 2, this.EnableDepthAxis ? +0.499 : 0.15);

                group.Children.Add(geometryBackAxis);
            }

            if (settings.ShowRightWall && settings.RightWallThickness != 0d)
            {
                materialGroup = new MaterialGroup();
                materialGroup.Children.Add(new DiffuseMaterial(settings.RightWallBackground));
                if (!this.isClustered)
                    geometryRightAxis.Geometry = MeshGenerator.ColumnParallelotope(rightShift + margin, 1, 0.30 * this.Series.Count);
                else
                    geometryRightAxis.Geometry = MeshGenerator.ColumnParallelotope(rightShift + margin, 1, 0.30);
                geometryRightAxis.Material = materialGroup;
                geometryRightAxis.BackMaterial = materialGroup;

                geometryRightAxis.Transform = new TranslateTransform3D(+0.501 + rightShift / 2 + margin / 2, 0, 0.15);

                group.Children.Add(geometryRightAxis);
            }

            if (settings.ShowTopWall && settings.TopWallThickness != 0d)
            {
                materialGroup = new MaterialGroup();
                materialGroup.Children.Add(new DiffuseMaterial(settings.TopWallBackground));
                if (!this.isClustered)
                    geometryTopAxis.Geometry = MeshGenerator.ColumnParallelotope(1 + margin * 2 + leftShift + rightShift, topShift + margin, 0.30 * this.Series.Count);
                else
                    geometryTopAxis.Geometry = MeshGenerator.ColumnParallelotope(1 + margin * 2 + leftShift + rightShift, topShift + margin, 0.30);
                geometryTopAxis.Material = materialGroup;
                geometryTopAxis.BackMaterial = materialGroup;

                geometryTopAxis.Transform = new TranslateTransform3D(-leftShift / 2 + rightShift / 2, +0.500 + topShift / 2 + margin / 2, 0.15);

                group.Children.Add(geometryTopAxis);
            }

            if (settings.ShowBackWall)
            {
                materialGroup = new MaterialGroup();
                materialGroup.Children.Add(new DiffuseMaterial(settings.BackWallBackground));

                geometryBottomAxis.Geometry = MeshGenerator.ColumnParallelotope(1 + margin * 2 + leftShift + rightShift, 1 + margin * 2 + bottomShift + topShift, settings.BackWallThickness);
                geometryBottomAxis.Material = materialGroup;
                geometryBottomAxis.BackMaterial = materialGroup;

                if (!this.isClustered)
                    geometryBottomAxis.Transform = new TranslateTransform3D(-leftShift / 2 + rightShift / 2, -bottomShift / 2 + topShift / 2, ((geometryBackAxis.Geometry.Bounds.Z + (settings.BackWallThickness * 7))));
                else
                    geometryBottomAxis.Transform = new TranslateTransform3D(-leftShift / 2 + rightShift / 2, -bottomShift / 2 + topShift / 2, this.EnableDepthAxis ? -settings.BackWallThickness / 2 : settings.BackWallThickness / 2);

                if (!this.IsClustered && !this.EnableDepthAxis)
                    this._backWallThickness = geometryBottomAxis.Transform.Value.OffsetZ + 0.04;
                else
                    this._backWallThickness = geometryBottomAxis.Transform.Value.OffsetZ;
                group.Children.Add(geometryBottomAxis);
            }

            AxisContent = group;
        }

        /// <summary>
        /// Gets the grid line stroke attached property.
        /// </summary>
        /// <param name="axis">The <see cref="ChartAxis"/>.</param>
        /// <returns>Grid line stroke.</returns>
        public static Pen GetGridLineStroke(ChartAxis axis)
        {
            if (axis == null)
            {
                throw new ArgumentNullException("axis");
            }

            return (Pen)axis.GetValue(ChartArea.GridLineStrokeProperty);
        }

        /// <summary>
        /// Sets the grid line stroke attached property.
        /// </summary>
        /// <param name="axis">The <see cref="ChartAxis"/>.</param>
        /// <param name="stroke">The grid line stroke.</param>
        /// <seealso cref="ChartArea"/>
        public static void SetGridLineStroke(ChartAxis axis, Pen stroke)
        {
            if (axis == null)
            {
                throw new ArgumentNullException("axis");
            }

            axis.SetValue(ChartArea.GridLineStrokeProperty, stroke);
            axis.Area.UpdateArea();
        }


        /// <summary>
        /// Method for GetSmallGridLineStroke
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Pen GetSmallGridLineStroke(ChartAxis axis)
        {
            if (axis == null)
            {
                throw new ArgumentNullException("axis");
            }

            return (Pen)axis.GetValue(ChartArea.SmallGridLineStrokeProperty);
        }

        /// <summary>
        /// Sets the grid line stroke attached property.
        /// </summary>
        /// <param name="axis">The <see cref="ChartAxis"/>.</param>
        /// <param name="stroke">The grid line stroke.</param>
        /// <seealso cref="ChartArea"/>
        public static void SetSmallGridLineStroke(ChartAxis axis, Pen stroke)
        {
            if (axis == null)
            {
                throw new ArgumentNullException("axis");
            }

            axis.SetValue(ChartArea.SmallGridLineStrokeProperty, stroke);
            axis.Area.UpdateArea();
        }

        /// <summary>
        /// Gets the value of show grid lines attached property.
        /// </summary>
        /// <param name="axis">The <see cref="ChartAxis"/>.</param>
        /// <returns>The grid line stroke.</returns>
        public static bool GetShowGridLines(ChartAxis axis)
        {
            if (axis == null)
            {
                throw new ArgumentNullException("axis");
            }

            if (axis.ReadLocalValue(ChartArea.ShowGridLinesProperty) == DependencyProperty.UnsetValue)
            {
                return axis.Area.PrimaryAxis == axis || axis.Area.SecondaryAxis == axis || axis.Area.DepthAxis == axis;
            }

            return (bool)axis.GetValue(ChartArea.ShowGridLinesProperty);
        }

        /// <summary>
        /// return bool value for check MajorGridlines showing or not
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public static bool GetShowMajorGridLines(ChartAxis axis)
        {
            if (axis == null)
            {
                throw new ArgumentNullException("axis");
            }

            if (axis.ReadLocalValue(ChartArea.ShowMajorGridLinesProperty) == DependencyProperty.UnsetValue)
            {
                return axis.Area.PrimaryAxis == axis || axis.Area.SecondaryAxis == axis;
            }

            return (bool)axis.GetValue(ChartArea.ShowMajorGridLinesProperty);
        }

        /// <summary>
        /// Sets the show grid lines attached property.
        /// </summary>
        /// <param name="axis">The <see cref="ChartAxis"/>.</param>
        /// <param name="show">if set to <c>true</c>grid lines will be shown for axis.</param>
        /// <seealso cref="ChartArea"/>
        public static void SetShowGridLines(ChartAxis axis, bool show)
        {
            if (axis == null)
            {
                axis = new ChartAxis();
                // throw new ArgumentNullException("axis");

            }
            if (axis != null)
            {
                axis.SetValue(ChartArea.ShowGridLinesProperty, show);
                if (axis.Area != null)
                    axis.Area.UpdateArea();
            }
        }

        /// <summary>
        /// Method for SetShowMajorGridLines
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="show"></param>
        public static void SetShowMajorGridLines(ChartAxis axis, bool show)
        {
            if (axis == null)
            {
                axis = new ChartAxis();
                // throw new ArgumentNullException("axis");

            }
            if (axis != null)
            {
                axis.SetValue(ChartArea.ShowMajorGridLinesProperty, show);
                if (axis.Area != null)
                    axis.Area.UpdateArea();
            }
        }

        /// <summary>
        /// Gets the origin line stroke attached property.
        /// </summary>
        /// <param name="axis">The <see cref="ChartAxis"/>.</param>
        /// <returns>The OriginLineStroke pen</returns>
        public static Pen GetOriginLineStroke(ChartAxis axis)
        {
            if (axis == null)
            {
                throw new ArgumentNullException("axis");
            }

            return (Pen)axis.GetValue(ChartArea.OriginLineStrokeProperty);
        }

        /// <summary>
        /// Sets the origin line stroke attached property.
        /// </summary>
        /// <param name="axis">The <see cref="ChartAxis"/>.</param>
        /// <param name="stroke">The stroke <see cref="Pen"/>.</param>
        /// <seealso cref="ChartArea"/>
        public static void SetOriginLineStroke(ChartAxis axis, Pen stroke)
        {
            if (axis == null)
            {
                throw new ArgumentNullException("axis");
            }

            axis.SetValue(ChartArea.OriginLineStrokeProperty, stroke);
        }

        /// <summary>
        /// Gets the show origin line attached property.
        /// </summary>
        /// <param name="axis">The <see cref="ChartAxis"/>.</param>
        /// <returns>Bool value to show grid lines</returns>
        public static bool GetShowOriginLine(ChartAxis axis)
        {
            if (axis == null)
            {
                throw new ArgumentNullException("axis");
            }

            return (bool)axis.GetValue(ChartArea.ShowOriginLineProperty);
        }

        /// <summary>
        /// Sets the show origin line attached property.
        /// </summary>
        /// <param name="axis">The <see cref="ChartAxis"/>.</param>
        /// <param name="show">if set to <c>true</c> Origin should be shown on axis.</param>
        /// <seealso cref="ChartArea"/>
        public static void SetShowOriginLine(ChartAxis axis, bool show)
        {
            if (axis == null)
            {
                throw new ArgumentNullException("axis");
            }

            axis.SetValue(ChartArea.ShowOriginLineProperty, show);
            axis.Area.UpdateArea();
        }

        /// <summary>
        /// Converts Value to point.
        /// </summary>
        /// <param name="axis">The Chart axis .</param>
        /// <param name="value">The value.</param>
        /// <returns>The double value to point</returns>
        public double ValueToPoint(ChartAxis axis, double value)
        {
            if (m_areaPresenter != null && m_areaPresenter.AxesContainer != null)
            {
                ItemsControl axesContainer = m_areaPresenter.AxesContainer;
                FrameworkElement element = axesContainer.ItemContainerGenerator.ContainerFromItem(axis) as FrameworkElement;

                if (element != null)
                {
                    if (axis.Orientation == Orientation.Horizontal)
                    {
                        return axis.ValueToCoefficient(value) * element.ActualWidth - this.TranslatePoint(new Point(), element).X;
                    }
                    else
                    {
                        return (1 - axis.ValueToCoefficient(value)) * element.ActualHeight - this.TranslatePoint(new Point(), element).Y;
                    }
                }
            }

            return double.NaN;
        }

        /// <summary>
        /// Method for return double value from series,axis values
        /// </summary>
        /// <param name="series"></param>
        /// <param name="axis"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public double ValueToPoint(ChartSeries series, ChartAxis axis, double value)
        {
            if (m_areaPresenter != null && m_areaPresenter.AxesContainer != null)
            {
                ItemsControl axesContainer = m_areaPresenter.AxesContainer;
                FrameworkElement element = axesContainer.ItemContainerGenerator.ContainerFromItem(axis) as FrameworkElement;
                double width = (series.Presenter != null && series.Presenter.ActualWidth != element.ActualWidth) ?
                    series.Presenter.ActualWidth : element.ActualWidth;
                double height = (series.Presenter != null && series.Presenter.ActualHeight != element.ActualHeight) ?
                   series.Presenter.ActualHeight : element.ActualHeight;

                if (element != null)
                {
                    if (axis.Orientation == Orientation.Horizontal)
                    {
                        return axis.ValueToCoefficient(value) * width - this.TranslatePoint(new Point(), element).X;
                    }
                    else
                    {
                        return (1 - axis.ValueToCoefficient(value)) * height - this.TranslatePoint(new Point(), element).Y;
                    }
                }
            }

            return double.NaN;
        }


        /// <summary>
        /// Converts point to value.
        /// </summary>
        /// <param name="axis">The axis value.</param>
        /// <param name="point">The point.</param>
        /// <returns>The double point to value</returns>
        public double PointToValue(ChartAxis axis, Point point)
        {
            if (m_areaPresenter != null && m_areaPresenter.AxesContainer != null)
            {
                ItemsControl axesContainer = m_areaPresenter.AxesContainer;
                FrameworkElement element = axesContainer.ItemContainerGenerator.ContainerFromItem(axis) as FrameworkElement;

                if (element != null)
                {
                    point = this.TranslatePoint(point, element);

                    if (axis.Orientation == Orientation.Horizontal)
                    {
                        return axis.CoefficientToValue(point.X / element.ActualWidth);
                    }
                    else
                    {

                        return axis.CoefficientToValue(1d - (point.Y / element.ActualHeight));

                    }
                }
            }

            return double.NaN;
        }

        /// <summary>
        /// This method is used when series are rendered stacked. 
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="position">The position.</param>
        /// <param name="forPositive">Specify null to stack all series in one stack. True to return the stack info for just
        /// the positive series. False to return the stack info for just the negative series.</param>
        /// <returns>
        /// The value returned is a cumulative value of
        /// Y from all series that are below the series passed in the contained <see cref="Series">series collection</see>.</returns>
        public double GetStackInfo(ChartSeries series, int position, bool? forPositive)
        {
            IChartDataPoint point = series.GetPoint(position);
            if (point != null && (series.Type == ChartTypes.StackingArea100 || series.Type == ChartTypes.StackingLine100 || series.Type == ChartTypes.StackingSpline100 || series.Type == ChartTypes.StackingSplineArea100))
                return GetStackingArea100Info(series, point.X, forPositive, position);
            else if (point != null)
                return GetStackInfo(series, point.X, forPositive);
            else
                return -1;
        }
        internal List<ChartAxis> stackAxis = new List<ChartAxis>();
        /// <summary>
        /// This method is used when series are rendered stacked.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="position">The position.</param>
        /// <param name="forPositive">Specify null to stack all series in one stack. True to return the stack info for just
        /// the positive series. False to return the stack info for just the negative series.</param>
        /// <param name="value">The series.</param>
        /// <returns>The value returned is a cumulative value of
        /// Y from all series that are below the series passed in the contained <see cref="Series"/>.</returns>
        /// 

        public double GetStackingArea100Info(ChartSeries series, double position, bool? forPositive, int value)
        {
            var y = series.ActualYAxis.Origin;
            stackAxis = new List<ChartAxis>();
            foreach (var ser in m_visibleSeries)
            {
                if (series == ser)
                {
                    stackAxis.Add(ser.YAxis);
                    break;
                }

                else if (series.YAxis != ser.YAxis)
                {
                    foreach (ChartAxis axis in stackAxis)
                    {
                        if (axis != ser.YAxis)
                        {
                            break;
                        }
                    }
                }
                else if (ser.IsVisible && ser.ChartType != null && ser.ChartType.IsStacked)
                {
                    stackAxis.Add(ser.YAxis); 
                    IChartDataPoint cdpt = (from point in ser.DataModel.ChartPoints where !point.IsEmpty && point.X == position select point).FirstOrDefault();

                    if (cdpt == null)
                    {
                        for (int i = 0; i < ser.PointsCount; i++)
                        {
                            cdpt = ser.GetPoint(i);

                            if ((cdpt != null) && (!cdpt.IsEmpty) && (cdpt.X == position))
                            {
                                break;
                            }
                        }
                    }
                    if (cdpt != null && (ser.Type == ChartTypes.StackingArea100 || ser.Type == ChartTypes.StackingLine100 || ser.Type == ChartTypes.StackingSpline100 || ser.Type == ChartTypes.StackingSplineArea100))
                    {
                        var sum = series.Area.Series.Where(chartSeries => chartSeries.Data != null && chartSeries.Data.Count != 0 && chartSeries.ActualYAxis==ser.ActualYAxis).Sum(chartSeries => Math.Abs(chartSeries.Data[value].Y));
                        var yVal = cdpt.Values[0];
                        if (forPositive == null)
                        {
                            if (ChartStackingArea100Type.GetShowValueAsProbability(series.Area) || ChartStackingLine100Type.GetShowValueAsProbability(series.Area) || ChartStackingSpline100Type.GetShowValueAsProbability(series.Area) || ChartStackingSplineArea100Type.GetShowValueAsProbability(series.Area))
                            {
                                y += ((Math.Abs(yVal) / sum) * 100) / 100;
                            }
                            else
                            {
                                y += Math.Abs((yVal / sum) * 100);
                            }
                        }
                        else
                        {
                            if (ChartStackingArea100Type.GetShowValueAsProbability(series.Area) || ChartStackingLine100Type.GetShowValueAsProbability(series.Area) || ChartStackingSpline100Type.GetShowValueAsProbability(series.Area) || ChartStackingSplineArea100Type.GetShowValueAsProbability(series.Area))
                            {
                                y += ((yVal / sum) * 100) / 100;
                            }
                            else
                            {
                                y += (yVal / sum) * 100;
                            }
                        }
                    }
                }
            }
            
            stackAxis.Clear();
            stackAxis = null;
            return y;
        }

        private int pos = 0;
        /// <summary>
        /// Return double value Based on given series collection
        /// </summary>
        /// <param name="series"></param>
        /// <param name="position"></param>
        /// <param name="forPositive"></param>
        /// <returns></returns>
        public double GetStackInfo(ChartSeries series, double position, bool? forPositive)
        {
            double y = series.ActualYAxis.Origin;
            stackAxis = new List<ChartAxis>();
            IChartDataPoint cdpt = null;
            foreach (ChartSeries ser in m_visibleSeries)
            {
                if (series == ser)
                {
                    stackAxis.Add(ser.YAxis);
                    break;
                }

                else if (series.YAxis != ser.YAxis)
                {
                    foreach (ChartAxis axis in stackAxis)
                    {
                        if (axis != ser.YAxis)
                        {
                            break;
                        }
                    }
                }
                else if (ser.IsVisible && ser.ChartType != null && ser.ChartType.IsStacked)
                {
                    stackAxis.Add(ser.YAxis);
                    //if (series.ChartType.GetType() == series.ChartType.GetType())
                    //{
                    //    for (int i = 0; i < ser.PointsCount; i++)
                    //    {
                    //        IChartDataPoint cdpt = ser.GetPoint(i);

                    //        if ((cdpt != null) && (!cdpt.IsEmpty) && (cdpt.X == position))
                    //        {                          
                    //List<IChartDataPoint> _pointlist= (from point in ser.DataModel.ChartPoints where !point.IsEmpty select point ).ToList();
                    if (pos>=ser.DataModel.ChartPoints.Count)
                     pos = 0;

                    if (ser.DataModel.ChartPoints.Count > pos)
                    {
                         cdpt = ser.DataModel.ChartPoints[pos];
                    }
                    else
                    {
                        cdpt = null;
                    }

                    //Chart points 
                    if (cdpt == null)
                    {
                        for (int i = 0; i < ser.PointsCount; i++)
                        {
                            cdpt = ser.GetPoint(i);

                            if ((cdpt != null) && (!cdpt.IsEmpty) && (cdpt.X == position))
                            {
                                break;
                            }
                        }
                    }

                    if (cdpt != null && (ser.Type != ChartTypes.StackingArea && ser.Type != ChartTypes.StackingLine && ser.Type != ChartTypes.StackingSpline && ser.Type != ChartTypes.StackingSplineArea))
                    {
                        double yVal = cdpt.Values[0];
                        if (!(ser.Data.Count.Equals(series.Data.Count)) && position >= Math.Min(ser.Data.Count, series.Data.Count))
                        {
                            yVal = 0;
                        }
                        if (forPositive == null)
                        {
                            y += Math.Abs(yVal);
                        }
                        else if (forPositive == true)
                        {
                            if (yVal > 0)
                            {
                                y += yVal;
                            }
                        }
                        else if (forPositive == false)
                        {
                            if (yVal < 0)
                            {
                                y += yVal;
                            }
                        }
                    }
                    else if (cdpt != null && (ser.Type == ChartTypes.StackingArea || ser.Type == ChartTypes.StackingLine || ser.Type == ChartTypes.StackingSpline || ser.Type == ChartTypes.StackingSplineArea))
                    {
                        double yVal = (cdpt.EmptyPoint && !ser.ShowEmptyPoints && ser.Area != null && ser.Area.SecondaryAxis != null) ? series.Area.SecondaryAxis.Origin : cdpt.Values[0];
                        if(forPositive == null)
                            y += Math.Abs(yVal);
                        else
                            y += yVal;                       
                    }

                    //            break;
                    //        }
                    //    }
                    //}
                }
            }
            pos++;
            stackAxis.Clear();
            stackAxis = null;
            return y;
        }

        /// <summary>
        /// Gets the percentage stack info.
        /// </summary>
        /// <remarks>
        /// Used to calculate <see cref="ChartStackingColumnSegment"/> column shift.
        /// </remarks>
        /// <param name="series">The series.</param>
        /// <param name="point">The point.</param>
        /// <returns>The double Percentage Stack Info</returns>
        public DoubleRange GetPercentageStackInfo(ChartSeries series, ChartIndexedDataPoint point)
        {
            int seriesIndex = series.Area.VisibleSeries.IndexOf(series);
            double yStart = 0;
            double yEnd = 0;
            double sum = 0;
            int coeff = ChartStackingColumn100Type.GetShowValueAsProbability(this) ? 1 : 100;
            bool reqNegativeStack = ChartStackingColumn100Type.GetRequiresNegativeSeriesStack(series.Area);

            if (reqNegativeStack)
            {
                foreach (ChartSeries ser in series.Area.VisibleSeries)
                {
                    if (ser.PointsCount > point.Index && ser.GetPoint(point.Index).Y < 0)
                    {
                        ser.Area.hasStack100NegValues = true;
                        coeff = ChartStackingColumn100Type.GetShowValueAsProbability(this) ? -1 : -100;
                    }
                }
            }
            foreach (ChartSeries chartSeries in series.Area.VisibleSeries)
            {
                if (chartSeries.PointsCount > point.Index)
                {
                    if (coeff < 0 || !reqNegativeStack)
                        sum += Math.Abs(chartSeries.GetPoint(point.Index).Y);
                    else
                        sum += chartSeries.GetPoint(point.Index).Y;
                }
            }

            if (reqNegativeStack)
            {
                if (coeff > 0)
                    yEnd = point.DataPoint.Y / sum * coeff;
                else
                    yEnd = point.DataPoint.Y / sum * (-coeff);
            }
            else
                yEnd = Math.Abs(point.DataPoint.Y) / sum * coeff;

            for (int i = seriesIndex - 1; i >= 0; i--)
            {
                ChartSeries targetSeries = VisibleSeries[i];
                if (targetSeries.PointsCount > point.Index)
                {
                    double columnShift = 0;
                    if (reqNegativeStack)
                    {
                        if (coeff > 0)
                        {
                            columnShift = targetSeries.GetPoint(point.Index).Y / sum * coeff;
                        }
                        else if (coeff < 0 && ((point.DataPoint.Y > 0 && targetSeries.GetPoint(point.Index).Y > 0) ||
                                (point.DataPoint.Y < 0 && targetSeries.GetPoint(point.Index).Y < 0)))
                        {
                            columnShift = targetSeries.GetPoint(point.Index).Y / sum * (-coeff);
                        }
                    }
                    else
                    {
                        columnShift = Math.Abs(targetSeries.GetPoint(point.Index).Y) / sum * coeff;
                    }
                    yStart += columnShift;
                    yEnd += columnShift;
                }
            }

            return new DoubleRange(yStart, yEnd);
        }


        //string _tempText = null;


        //private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var area = d as ChartArea;
        //    if (area != null)
        //    {
        //        if (area.Header != null)
        //        {
        //            if (area.Header is String)
        //            {
        //                area._tempText = area.Header.ToString();
        //                //area.SetHeaderStyle();
        //            }
        //        }
        //    }
        //}

        //internal void SetHeaderStyle()
        //{
        //    var chart = this.Parent as Chart;
        //    if (this.Header is String)
        //    {
        //        _tempText = this.Header.ToString();
        //        if (this.Header != null)
        //        {
        //            {
        //                var text = new TextBlock
        //                    {
        //                        Text = this.Header.ToString(),
        //                        FontSize =
        //                            (this.m_isFontSizeSet
        //                                 ? this.FontSize
        //                                 : (chart != null && chart.m_isFontSizeSet) ? chart.FontSize : 39),
        //                        FontWeight =
        //                            (this.m_isFontWeightSet
        //                                 ? this.FontWeight
        //                                 : (chart != null && chart.m_isFontWeightSet) ? chart.FontWeight : FontWeights.Light),
        //                        FontFamily =
        //                            (this.m_isFontFamilySet
        //                                 ? this.FontFamily
        //                                 : (chart != null && chart.m_isFontFamilySet)
        //                                       ? chart.FontFamily
        //                                       : new FontFamily("Segoe UI")),
        //                        Foreground = this.Foreground
        //                    };
        //                this.Header = text;
        //            }
        //        }
        //    }
        //    else if(this.Header is TextBlock)
        //    {
        //        _tempText = ((TextBlock)this.Header).Text.ToString(CultureInfo.InvariantCulture);
        //        if (this.Header != null)
        //        {
        //            {
        //                var text = this.Header as TextBlock;                       
        //                text.FontSize = (this.m_isFontSizeSet ? this.FontSize : (chart != null && chart.m_isFontSizeSet) ? chart.FontSize : 39);
        //                text.FontWeight = (this.m_isFontWeightSet ? this.FontWeight : (chart != null && chart.m_isFontWeightSet) ? chart.FontWeight : FontWeights.Light);
        //                text.FontFamily = (this.m_isFontFamilySet ? this.FontFamily : (chart != null && chart.m_isFontFamilySet) ? chart.FontFamily : new FontFamily("Segoe UI"));
        //                text.Foreground = this.Foreground;
        //                this.Header = text;
        //            }
        //        }
        //    }

        //    else if (_tempText != null)
        //    {

        //        var text = new TextBlock
        //            {
        //                Text = _tempText,
        //                FontSize =
        //                    (this.m_isFontSizeSet
        //                         ? this.FontSize
        //                         : (chart != null && chart.m_isFontSizeSet) ? chart.FontSize : 39),
        //                FontWeight =
        //                    (this.m_isFontWeightSet
        //                         ? this.FontWeight
        //                         : (chart != null && chart.m_isFontWeightSet) ? chart.FontWeight : FontWeights.Light),
        //                FontFamily =
        //                    (this.m_isFontFamilySet
        //                         ? this.FontFamily
        //                         : (chart != null && chart.m_isFontFamilySet)
        //                               ? chart.FontFamily
        //                               : new FontFamily("Segoe UI")),
        //                Foreground = this.Foreground
        //            };
        //        this.Header = text;
        //    }
        //    else
        //    {
        //    }

        //}



        internal ResourceDictionary tempalteRD = null;
        /// <summary>
        /// Invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        /// <seealso cref="ChartArea"/>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (View3DMode)
            {
                Chart3D settings = this.Chart3DSettings ?? new Chart3D();
                this.CameraController.Rotate = settings.ViewDefaultRotate;
                this.CameraController.Tilt = settings.ViewDefaultTilt;
                this.CameraController.Turn = settings.ViewDefaultTurn;
            }

            _mDockPanel = Template.FindName("PART_CHARTDOCKPANEL", this) as ChartDockPanel;
            if (_mDockPanel != null)
            {
                if (this.IsSync == true || this is SyncChartAreas)
                {
                    _mDockPanel.ElementMargin = new Thickness();
                }
                if (_mDockPanel.RootElement != null)
                {
                    Binding seriesBinding = new Binding();
                    seriesBinding.Source = this;
                    BindingOperations.SetBinding(_mDockPanel.RootElement, ChartAreaPresenter.ContentProperty, seriesBinding);

                    ChartAreaPresenter presenter = _mDockPanel.RootElement as ChartAreaPresenter;
                    if (presenter != null)
                    {
                        if (presenter.ContentTemplateSelector == null)
                        {
                            if (tempalteRD == null)
                            {
                                //tempalteRD = ChartDictionaries.GenericDictionary;
                                tempalteRD = new SharedResourceDictionary()
                                {
                                    Source = new Uri("/Syncfusion.Chart.Wpf;component/Themes/generic.xaml", UriKind.RelativeOrAbsolute)
                                };
                            }

                            if (tempalteRD != null)
                            {
                                presenter.ContentTemplateSelector = tempalteRD["ChartAreaTemplateSelectorKey"] as DataTemplateSelector;
                            }

                        }
                    }
                }
            }

            UpdateLegend(null, Legend);

        }
        #endregion
        
        #region EventHandlers


        /// <summary>
        /// Event for ChartScrolling in ChartArea
        /// </summary>
        public event QtpChartScrollPositionEventHandler ChartScrolling;

        internal void OnChartScrolling(ChartScrollEventArgs args)
        {
            if (this.EnableRangeSelection)
            {
                if (this.RangeSelectionOrientation == Orientation.Vertical)
                    ResetVerticalSelectedRange();
                else
                    ResetHorizontalSelectedRange();
            }
            
            if (ChartScrolling != null)
            {
                ChartScrolling(this, args);
            }
        }

        /// <summary>
        /// event for Chartzoomed in ChartArea
        /// </summary>
        public event QtpChartZoomedEventHandler ChartZoomed;

        internal void OnChartZoomed(ChartZoomedEventArgs args)
        {
            if (ChartZoomed != null)
            {
                ChartZoomed(this, args);
            }
        }

        /// <summary>
        /// Event for ChartZoomSector in ChartArea
        /// </summary>
        public event QtpChartZoomSectorEventHandler ChartZoomSector;

        internal void OnChartZoomSector(ChartZoomSectorEventArgs args)
        {
            if (ChartZoomSector != null)
            {
                ChartZoomSector(this, args);
            }
            if (this.Series != null)
            {
                foreach (var Ser in this.Series)
                {
                    Ser.Zoomactionenabled = true;
                }
            }
        }
        /// <summary>
        /// Event for ChartZoomedOut in ChartArea
        /// </summary>
        public event QtpChartZoomedOutEventHandler ChartZoomedOut;

        internal void OnChartZoomedOut(ChartZoomedOutEventArgs args)
        {
            if (ChartZoomedOut != null)
            {
                ChartZoomedOut(this, args);
            }
            if (this.Series != null)
            {
                foreach (var Ser in this.Series)
                {
                    Ser.Zoomactionenabled = true;
                }
            }
        }
        /// <summary>
        /// Event for ChartPanning in ChartArea
        /// </summary>
        public event QtpChartPanningEventHandler ChartPanning;

        internal void OnChartPanning(ChartPanningEventArgs args)
        {
            if (ChartPanning != null)
            {
                ChartPanning(this, args);
            }
            if (this.Series != null)
            {
                foreach (var Ser in this.Series)
                {
                    Ser.Zoomactionenabled = true;
                }
            }
        }
        /// <summary>
        /// Event for ChartZoomedReset in ChartArea
        /// </summary>
        public event QtpChartZoomResetEventHandler ChartZoomReset;

        internal void OnChartZoomReset(ChartZoomReseteventArgs args)
        {
            if (ChartZoomReset != null)
            {
                ChartZoomReset(this, args);
            }
        }
        #endregion
        internal bool clusterd = false;
        #region Implementation
        /// <summary>
        /// Returns the value of side by side displacement.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns>The Doublerange Side by side Info</returns>
        public DoubleRange GetSideBySideInfo(ChartSeries series)
        {
            double width = 1 - ChartType.GetSpacing(this);
            double minWidth = 0d;
            minWidth = this.MinPointsDelta;
            if (this.isAllowDragDrop)
            {
                minWidth = 1;
            }
            if (!SideBySideSeriesPlacement)
            {
                return new DoubleRange(-width / 2, width / 2);
            }

            int pos = -1;
            int all = 0;
            if (series.ChartType.IsSideBySide && !clusterd)
            {
                Dictionary<Type, int> stackedColumns = new Dictionary<Type, int>();

                foreach (ChartSeries ser in m_visibleSeries)
                {
                    if (ser.IsVisible && ser.ChartType != null && ser.ChartType.IsSideBySide)
                    {
                        if (ser.ChartType.IsStacked)
                        {
                            if (!stackedColumns.ContainsKey(ser.ChartType.GetType()))
                            {
                                all++;
                                stackedColumns.Add(ser.ChartType.GetType(), all);
                            }

                            if (series == ser)
                            {
                                pos = stackedColumns[ser.ChartType.GetType()];
                            }
                        }
                        else
                        {
                            all++;

                            if (series == ser)
                            {
                                pos = all;
                            }
                        }
                    }
                }
            }

            if (all == 0)
            {
                all = 1;
                pos = 1;
            }

            //  minWidth = (minWidth == double.MaxValue) ? 1d : minWidth;

            double div = minWidth * width / all;
            double start = div * (pos - 1) - minWidth * width / 2;
            double end = start + div;

            return new DoubleRange(start, end);
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty EnableLazyLoadingProperty =
       DependencyProperty.Register("EnableLazyLoading", typeof(bool), typeof(ChartArea), new UIPropertyMetadata(false, OnEnableLazyLoadingChanged));

        /// <summary>
        ///  Identifies the IsBeginInitIsActive dependency property.
        /// </summary>
        public static readonly DependencyProperty IsBeginInitIsActiveProperty =
    DependencyProperty.Register("IsBeginInitIsActive", typeof(bool), typeof(ChartArea), new UIPropertyMetadata(false, OnIsBeginInitIsActiveChanged));


        /// <summary>
        /// Gets or sets a value indicating whether [enable lazy loading].
        /// </summary>
        /// <value><c>true</c> if [enable lazy loading]; otherwise, <c>false</c>.</value>
        public bool EnableLazyLoading
        {
            get
            {
                return (bool)GetValue(EnableLazyLoadingProperty);
            }

            set
            {
                SetValue(EnableLazyLoadingProperty, value);
            }
        }

        /// <summary>
        /// Get and set IsBeginInitIsActiveProperty
        /// </summary>
        public bool IsBeginInitIsActive
        {
            get
            {
                return (bool)GetValue(IsBeginInitIsActiveProperty);
            }

            set
            {
                SetValue(IsBeginInitIsActiveProperty, value);
            }
        }

        private static void OnIsBeginInitIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartArea area = d as ChartArea;
            if (area != null)
            {
                area.updateArea = !(bool)args.NewValue;
            }

            if (area is SyncChartAreas)
            {
                foreach (var item in (area as SyncChartAreas).Areas)
                {
                    item.IsBeginInitIsActive = (bool)args.NewValue;
                }
            }

            if (area != null)
            {
                if ((bool)args.NewValue == false)
                {
                    area.updateArea = true;
                    area.UpdateArea();
                }
            }
        }
        /// <summary>
        /// Called when [enable lazy loading changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnEnableLazyLoadingChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartArea area = d as ChartArea;
            if (area != null)
            {
                area.updateArea = !(bool)args.NewValue;
            }

            if (area is SyncChartAreas)
            {
                foreach (var item in (area as SyncChartAreas).Areas)
                {
                    item.EnableLazyLoading = (bool)args.NewValue;
                }
            }
        }


        /// <summary>
        /// Occurs when Vertical Scrolling axis gets changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnVerticalScrollingAxisChanged(object sender, EventArgs e)
        {

            ChartAxis chartAxis = sender as ChartAxis;
            if (chartAxis != null)
            {
                if (chartAxis.Area != null)
                {
                    if (this.ZoomAllAxes)
                    {
                        ////If we're zooming whole area - scroll all axes along with vertical one.
                        foreach (ChartAxis axis in this.Axes)
                        {
                            if (axis != null)
                            {
                                if (axis.Orientation == Orientation.Vertical && (axis.Equals(chartAxis) == true || !chartAxis.Area.IsMouseDragZooming))
                                {

                                    axis.ZoomPosition = (sender as ChartAxis).ZoomPosition;
                                    VisibleRangeForZoomVerticalAxis(chartAxis.Area);
                                    if (this.IsLoaded)
                                   OnChartScrolling(new ChartScrollEventArgs(chartAxis.Area, chartAxis));
                                }
                            }
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Occurs when Horizontal Scrolling axis gets changed.
        /// </summary>
        /// <param name="sender">The object sender</param>
        /// <param name="e">The Event Arguments e</param>
        private void OnHorizontalScrollingAxisChanged(object sender, EventArgs e)
        {
            ChartAxis chartAxis = sender as ChartAxis;
            if (chartAxis != null)
            {
                if (chartAxis.Area != null)
                {
                    if (chartAxis.Area != null)
                    {
                        if (chartAxis.Area.IsSync == true)
                        {
                            if (chartAxis.Area.ChartAreaParent != null)
                            {
                                SyncChartAreas syncChartArea = chartAxis.Area.ChartAreaParent;
                                if (syncChartArea != null)
                                {
                                    foreach (ChartArea chartArea_sync in syncChartArea.Areas)
                                    {
                                        if (chartArea_sync != null)
                                        {
                                            foreach (ChartAxis axis in chartArea_sync.Axes)
                                            {
                                                if (axis != null)
                                                {
                                                    if (axis.Orientation == Orientation.Horizontal)
                                                    {
                                                        axis.ZoomPosition = (sender as ChartAxis).ZoomPosition;
                                                        VisibleRangeForZoomHorizontalAxis(chartAxis.Area);
                                                        if(this.IsLoaded)
                                                        OnChartScrolling(new ChartScrollEventArgs(chartAxis.Area, chartAxis));
                                                    }
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                        }
                        else
                        {
                            if (this.ZoomAllAxes)
                            {
                                if (this.Axes != null)
                                {
                                    if (this.Axes.Count > 0)
                                    {
                                        ////If we're zooming whole area - scroll all axes along with horizontal one.
                                        foreach (ChartAxis axis in this.Axes)
                                        {
                                            if (axis != null && (sender as ChartAxis) != null)
                                            {
                                                if (axis.Orientation == Orientation.Horizontal && (axis.Equals(chartAxis) == true || !chartAxis.Area.IsMouseDragZooming))
                                                {

                                                    axis.ZoomPosition = (sender as ChartAxis).ZoomPosition;
                                                    VisibleRangeForZoomHorizontalAxis(chartAxis.Area);
                                                    if (this.IsLoaded)
                                                    OnChartScrolling(new ChartScrollEventArgs(axis.Area, axis));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Gets the chart segment for visuals.
        /// </summary>
        /// <param name="segment">The segment.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private ChartSegment GetChartSegmentForVisuals(ChartSegment segment, int index)
        {

            if (segment != null)
            {
                var correspondingPoint = (from point in segment.CorrespondingPoints
                                          where point.DataPoint.EmptyPoint == false
                                          select point).ToList<ChartIndexedDataPoint>();


                if (correspondingPoint.Count > 0)
                {
                    if (segment is ChartFastStackingColumnSegment)
                    {
                        segment = new ChartFastStackingColumnSegment(new ChartIndexedDataPoint[] { correspondingPoint[index] }, segment.Series);
                    }

                    else if (segment is ChartFastHiLoOpenCloseSegment)
                    {
                        segment = new ChartFastHiLoOpenCloseSegment(new ChartIndexedDataPoint[] { correspondingPoint[index] }, segment.Series);
                    }
                }
            }

            return segment;

        }
        internal Point value = new Point();
        internal bool isAllowDragDrop = false;
        private double horizontalOffset = 0d;
        private double verticalOffset = 0d;
        private Point mousePoint = new Point();
        private ObservableCollection<ResourceDictionary> fastsegRd_coll = new ObservableCollection<ResourceDictionary>();
        int fastrdcount = 0;
        private ObservableCollection<ResourceDictionary> segRd_coll = new ObservableCollection<ResourceDictionary>();
        int segrdcount = 0;
        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseMove"></see>�attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"></see> that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if ((IsPanning == true) && (this is SyncChartAreas == false))
            {

                foreach (ChartAxis axis in this.Axes)
                {
                    Point newPosition1 = e.GetPosition(this);
                    if (axis.ZoomFactor < 1)
                    {
                        if (axis.Orientation == Orientation.Horizontal)
                        {
                            double newpt = PointToValue(axis, newPosition1);
                            double Xvalue = axis.lastPosition_X - newpt;
                            double Pstart = axis.IsFractionEnabledOnZoom ? axis.VisibleRange.Start : axis.m_actualRangeValue.Start;
                            double Pend = axis.IsFractionEnabledOnZoom ? axis.VisibleRange.End : axis.m_actualRangeValue.End;

                            Pstart = Pstart + (axis.IsFractionEnabledOnZoom ? Xvalue : Math.Floor(Xvalue));
                            Pend = Pend + (axis.IsFractionEnabledOnZoom ? Xvalue : Math.Floor(Xvalue));

                            this.PanningRange_X = new DoubleRange(Pstart, Pend);
                            axis.ZoomRange(PanningRange_X);
                        }
                        else
                        {
                            double newpt = PointToValue(axis, newPosition1);
                            double Yvalue = axis.lasPosition_Y - newpt;
                            double Pstart = axis.IsFractionEnabledOnZoom ? axis.VisibleRange.Start : axis.m_actualRangeValue.Start;
                            double Pend = axis.IsFractionEnabledOnZoom ? axis.VisibleRange.End : axis.m_actualRangeValue.End;

                            Pstart = Pstart + (axis.IsFractionEnabledOnZoom ? Yvalue : Math.Floor(Yvalue));
                            Pend = Pend + (axis.IsFractionEnabledOnZoom ? Yvalue : Math.Floor(Yvalue));

                            this.PanningRange_Y = new DoubleRange(Pstart, Pend);
                            axis.ZoomRange(this.PanningRange_Y);
                        }
                    }
                }
            }
            else if (this is SyncChartAreas == true)
            {
                SyncChartAreas syncChartArea = this as SyncChartAreas;
                if ((syncChartArea != null) && (IsPanning == true))
                {

                    foreach (ChartArea chartarea_Sync in syncChartArea.Areas)
                    {
                        if (chartarea_Sync.index == 0)
                        {
                            foreach (ChartAxis axis in chartarea_Sync.Axes)
                            {
                                Point newpoint = e.GetPosition(this);
                                if (axis.Orientation == Orientation.Horizontal)
                                {
                                    double newpt = chartarea_Sync.PointToValue(axis, newpoint);
                                    double Xvalue = chartarea_Sync.lastPosition_X - newpt;
                                    double Pstart = axis.VisibleRange.Start;
                                    double Pend = axis.VisibleRange.End;

                                    Pstart = Pstart + Xvalue;
                                    Pend = Pend + Xvalue;

                                    syncChartArea.PanningRange_Sync = new DoubleRange(Pstart, Pend);
                                    axis.ZoomRange(syncChartArea.PanningRange_Sync);

                                }

                            }
                        }

                    }


                }
            }
            FrameworkElement fElement = e.Device.Target as FrameworkElement;
            if (m_highlightedSegment != null)
            {
                m_highlightedSegment.Highlighted = false;
            }

            if (fElement != null && fElement.DataContext is ChartSegment)
            {
                HitTestResult htr = VisualTreeHelper.HitTest(this, e.GetPosition(this));
                ChartSegment fastSegment = null;
                if (htr != null && htr.VisualHit is DrawingVisual)
                {
                    int index = ChartFastSeriesPresenter.GetIndex(htr.VisualHit);
                    ChartSegment segment = fElement.DataContext as ChartSegment;
                    if (segment != null)
                        fastSegment = GetChartSegmentForVisuals(segment, index);
                }

                ////Rasing Mouse move event if mouse over the series.
                if (fastSegment != null)
                {
                    this.fastsegRd_coll.Add(
                     new SharedResourceDictionary()
                    {
                        Source = new Uri("/Syncfusion.Chart.Wpf;component/ChartSeries/SeriesGUI.xaml", UriKind.RelativeOrAbsolute)
                    });
                    
                    if (fastSegment.Series.ShowToolTip == true)
                    {
                        if (fastSegment.Series.ToolTip == null)
                        {
                            var style = this.Parent as Chart;
                            if (style != null && style.ChartVisualStyle == ChartStyles.Vs2010)
                                fastSegment.Series.ToolTip = this.fastsegRd_coll[fastrdcount]["VS2010TooltipStyle"] as ToolTip;
                            else if (style != null && style.ChartVisualStyle == ChartStyles.Metro)
                                fastSegment.Series.ToolTip = this.fastsegRd_coll[fastrdcount]["MetroThemeTooltipStyle"] as ToolTip;
                            else
                                fastSegment.Series.ToolTip = this.fastsegRd_coll[fastrdcount]["DefaultTooltips"] as ToolTip;
                        }

                        fastSegment.Series.TooltipString = fastSegment.CorrespondingPoints[0].DataPoint.Y.ToString();
                    }
                    fastrdcount++;
                    fastSegment.Series.OnMouseMove(fastSegment.Series, new ChartMouseEventArgs(e, fastSegment));
                }
                else
                {
                    if ((fElement.DataContext as ChartSegment).Series != null && (fElement.DataContext as ChartSegment).Series.ShowToolTip == true)
                    {
                        Point pt = e.GetPosition((fElement.DataContext as ChartSegment).Series);
                        double X = (fElement.DataContext as ChartSegment).Series.Area.PointToValue((fElement.DataContext as ChartSegment).Series.Area.PrimaryAxis, pt);
                        double Y = (fElement.DataContext as ChartSegment).Series.Area.PointToValue((fElement.DataContext as ChartSegment).Series.Area.SecondaryAxis, pt);
                        this.segRd_coll.Add( new SharedResourceDictionary()
                        {
                            Source = new Uri("/Syncfusion.Chart.Wpf;component/ChartSeries/SeriesGUI.xaml", UriKind.RelativeOrAbsolute)
                        });
                        if ((fElement.DataContext as ChartSegment).Series.ToolTip == null)
                        {
                            var style = this.Parent as Chart;
                            if (style != null && style.ChartVisualStyle == ChartStyles.Vs2010)
                                (fElement.DataContext as ChartSegment).Series.ToolTip = this.segRd_coll[segrdcount]["VS2010TooltipStyle"] as ToolTip;
                            else if (style != null && style.ChartVisualStyle == ChartStyles.Metro)
                                (fElement.DataContext as ChartSegment).Series.ToolTip = this.segRd_coll[segrdcount]["MetroThemeTooltipStyle"] as ToolTip;
                            else
                                (fElement.DataContext as ChartSegment).Series.ToolTip = this.segRd_coll[segrdcount]["DefaultTooltips"] as ToolTip;
                        }
                        if ((fElement.DataContext as ChartSegment).Series.ShowToolTip)
                        {
                            if ((double)(fElement.DataContext as ChartSegment).CorrespondingPoints.Count() > 1 && ((fElement.DataContext as ChartSegment) is ChartLineSegment))
                            {
                                if (e.OriginalSource is Line)
                                {
                                    Line line = (Line)e.OriginalSource;
                                    Point mousePos = e.GetPosition(line);
                                    if (line.X1 < line.X2)
                                    {
                                        if (mousePos.X < line.X1 + (line.X2 - line.X1) / 2)
                                            (fElement.DataContext as ChartSegment).Series.TooltipString = (fElement.DataContext as ChartSegment).CorrespondingPoints[0].DataPoint.Y;
                                        else
                                            (fElement.DataContext as ChartSegment).Series.TooltipString = (fElement.DataContext as ChartSegment).CorrespondingPoints[1].DataPoint.Y;
                                    }
                                    else
                                    {
                                        if (mousePos.X > line.X2 + (line.X1 - line.X2) / 2)
                                            (fElement.DataContext as ChartSegment).Series.TooltipString = (fElement.DataContext as ChartSegment).CorrespondingPoints[0].DataPoint.Y;
                                        else
                                            (fElement.DataContext as ChartSegment).Series.TooltipString = (fElement.DataContext as ChartSegment).CorrespondingPoints[1].DataPoint.Y;
                                    }
                                }
                                else 
                                if (e.OriginalSource is Ellipse)
                                {
                                    Point p = e.GetPosition(this);
                                    int count = (fElement.DataContext as ChartSegment).Series.Segments.Count;
                                    if ((fElement.DataContext as ChartSegment).CorrespondingPoints[1].Index == count)
                                    {
                                        int pX = (int)(fElement.DataContext as ChartSegment).Series.Area.ValueToPoint((fElement.DataContext as ChartSegment).Series.Area.PrimaryAxis, (fElement.DataContext as ChartSegment).Series.Data[count].X);
                                        int pY = (int)(fElement.DataContext as ChartSegment).Series.Area.ValueToPoint((fElement.DataContext as ChartSegment).Series.Area.SecondaryAxis, (fElement.DataContext as ChartSegment).Series.Data[count].Y);
                                        System.Drawing.Rectangle hitBox = new System.Drawing.Rectangle(pX - 5, pY - 5, 10, 10);
                                        if (hitBox.Contains((int)p.X, (int)p.Y))
                                        {
                                            (fElement.DataContext as ChartSegment).Series.TooltipString = (fElement.DataContext as ChartSegment).CorrespondingPoints[1].DataPoint.Y;
                                        }
                                        else
                                        {
                                            (fElement.DataContext as ChartSegment).Series.TooltipString = (fElement.DataContext as ChartSegment).CorrespondingPoints[0].DataPoint.Y;
                                        }

                                    }
                                    else
                                    {
                                        (fElement.DataContext as ChartSegment).Series.TooltipString = (fElement.DataContext as ChartSegment).CorrespondingPoints[0].DataPoint.Y;
                                    }

                                }
                               
                            }
                            else if ((double)(fElement.DataContext as ChartSegment).CorrespondingPoints.Count() > 1 && ((fElement.DataContext as ChartSegment) is ChartStepLineSegment))
                            {
                                double PosY = Math.Round((fElement.DataContext as ChartSegment).Series.Area.ValueToPoint((fElement.DataContext as ChartSegment).Series.Area.SecondaryAxis, (double)(fElement.DataContext as ChartSegment).CorrespondingPoints[1].DataPoint.Y));
                                if (PosY <= pt.Y + (fElement.DataContext as ChartSegment).StrokeThickness / 2 && PosY >= pt.Y || PosY >= pt.Y - (fElement.DataContext as ChartSegment).StrokeThickness / 2 && PosY <= pt.Y)
                                    (fElement.DataContext as ChartSegment).Series.TooltipString = (fElement.DataContext as ChartSegment).CorrespondingPoints[1].DataPoint.Y;
                                else
                                    (fElement.DataContext as ChartSegment).Series.TooltipString = (fElement.DataContext as ChartSegment).CorrespondingPoints[0].DataPoint.Y;
                            }
                            else if ((double)(fElement.DataContext as ChartSegment).CorrespondingPoints.Count() > 1 && Math.Round(Y) == (double)(fElement.DataContext as ChartSegment).CorrespondingPoints[1].DataPoint.Y && !((fElement.DataContext as ChartSegment) is ChartHistogramSegment))
                            {
                                (fElement.DataContext as ChartSegment).Series.TooltipString = (fElement.DataContext as ChartSegment).CorrespondingPoints[1].DataPoint.Y;
                            }
                            else if ((fElement.DataContext as ChartSegment) is ChartHistogramSegment)
                            {
                                (fElement.DataContext as ChartSegment).Series.TooltipString = (fElement.DataContext as ChartSegment).CorrespondingPoints.Count();
                            }
                            else
                            {
                                (fElement.DataContext as ChartSegment).Series.TooltipString = (fElement.DataContext as ChartSegment).CorrespondingPoints[0].DataPoint.Y;
                            }
                        }
                        else
                        {
                            (fElement.DataContext as ChartSegment).Series.TooltipString = null;
                            (fElement.DataContext as ChartSegment).Series.ToolTip = null;
                        }
                        segrdcount++;
                    }
                    else if ((fElement.DataContext as ChartSegment).Series != null)
                    {
                        (fElement.DataContext as ChartSegment).Series.TooltipString = null;
                        (fElement.DataContext as ChartSegment).Series.ToolTip = null;
                    }
                    // (fElement.DataContext as ChartSegment).Series.ToolTip = (fElement.DataContext as ChartSegment).CorrespondingPoints[0].DataPoint.Y.ToString();
                    if ((fElement.DataContext as ChartSegment).Series != null)
                        (fElement.DataContext as ChartSegment).Series.OnMouseMove((fElement.DataContext as ChartSegment).Series, new ChartMouseEventArgs(e, fElement.DataContext as ChartSegment));


                }

                ////Mouse pointer was not over the series before.
                if (this.MouseEnteredSegment == null)
                {
                    MouseEnteredSegment = fElement.DataContext as ChartSegment;
                    if (MouseEnteredSegment != null)
                    {
                        //Fix for moving interactive cursor from one series to another series in IsBindWithSegment as true.
                        if (this.InteractiveCursors != null)
                        {
                            foreach (InteractiveCursor cursor in this.InteractiveCursors)
                            {
                                if (cursor.IsBindWithSegment)
                                    cursor.CurrentSeries = MouseEnteredSegment.Series;
                            }
                        }
                        MouseEnteredSegment.Highlighted = true;
                        ////Rasing mouse enter event.
                        if (fastSegment != null)
                        {
                            fastSegment.Highlighted = true;
                            fastSegment.Series.OnMouseEnter(fastSegment.Series, new ChartMouseEventArgs(e, fastSegment));
                        }
                        else
                        {
                            if (MouseEnteredSegment.Series != null)
                                MouseEnteredSegment.Series.OnMouseEnter(MouseEnteredSegment.Series, new ChartMouseEventArgs(e, MouseEnteredSegment));
                        }
                    }
                }
                else
                {
                    ////Mouse was over the series/segment before.
                    if (MouseEnteredSegment != fElement.DataContext as ChartSegment)
                    {
                        ////We still having this series alive.
                        if (MouseEnteredSegment.Series != null)
                        {
                            ////Rasing mouse leave event.
                            MouseEnteredSegment.Series.OnMouseLeave(MouseEnteredSegment.Series, new ChartMouseEventArgs(e, MouseEnteredSegment));
                        }

                        MouseEnteredSegment.Highlighted = false;
                        ////Assigning a new entered segment.
                        MouseEnteredSegment = fElement.DataContext as ChartSegment;
                        ////Rasing mouse enter on a new segment/series.

                        if (MouseEnteredSegment != null && MouseEnteredSegment.Series != null)
                        {
                            MouseEnteredSegment.Series.OnMouseEnter(MouseEnteredSegment.Series, new ChartMouseEventArgs(e, MouseEnteredSegment));
                            MouseEnteredSegment.Highlighted = true;
                        }
                    }
                }
            }
            else
            {
                m_highlightedSegment = null;

                if (MouseEnteredSegment != null && MouseEnteredSegment.Series != null && ChartZoomingToolkit.GetZoomingToolkitVisibility(e.Device.Target as DependencyObject) != Visibility.Visible)
                {
                    MouseEnteredSegment.Series.OnMouseLeave(MouseEnteredSegment.Series, new ChartMouseEventArgs(e, MouseEnteredSegment));
                    MouseEnteredSegment.Highlighted = false;
                    this.MouseEnteredSegment = null;
                }
            }

            if (m_highlightedSegment != null)
            {
                m_highlightedSegment.Highlighted = true;
            }

            if (e.MouseDevice.Captured == this && View3DMode && e.LeftButton == MouseButtonState.Pressed && this.Allow3DRotate)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    double coff;
                    if (Camera3D is PerspectiveCamera)
                    {
                        coff = CameraController.Length * 100;
                    }
                    else
                    {
                        OrthographicCamera camera = Camera3D as OrthographicCamera;
                        coff = camera.Width * 100;
                    }

                    double dX = CameraController.Target.X + (m_mouseCaptureLocation.X - e.GetPosition(this).X) / 500;
                    double dY = CameraController.Target.Y - (m_mouseCaptureLocation.Y - e.GetPosition(this).Y) / 500;
                    Vector3D vector = new Vector3D(dX, dY, 0);
                    CameraController.Target = vector;
                    m_mouseCaptureLocation = e.GetPosition(this);
                }
                else
                {
                    double dX = m_mouseCaptureLocation.X - e.GetPosition(this).X;
                    double dY = m_mouseCaptureLocation.Y - e.GetPosition(this).Y;
                    Chart3DSettings.ViewDefaultRotate += dX;
                    Chart3DSettings.ViewDefaultTilt -= dY;
                    m_mouseCaptureLocation = e.GetPosition(this);
                }
            }
            if (this.AllowSegmentDragDrop && (this.DragSegment != null || fElement != null && fElement.DataContext is ChartSegment))
            {
                Mouse.OverrideCursor = Cursors.Cross;
            }
            else
            {
                Mouse.OverrideCursor = null;
            }

            if (this.AllowSegmentDragDrop && this.DragSegment != null && this.DragSegment.Series != null)
            {
                mousePoint = e.GetPosition(this);
                if (this.PrimaryAxis.m_visibleRange.Inside(this.PointToValue(PrimaryAxis, mousePoint)) && this.SecondaryAxis.m_visibleRange.Inside(this.PointToValue(this.SecondaryAxis, mousePoint)))
                {
                    int i = 0, index = 0;
                    foreach (Syncfusion.Windows.Chart.ChartSeriesPresenter.ChartSegmentPresenter presenter in this.DragSegment.Series.Presenter.m_visibleElements)
                    {
                        if (presenter.Segment != null && presenter.Segment == this.DragSegment)
                        {
                            index = i;
                        }
                        i++;
                    }
                    //int index = this.DragSegment.Series.Segments.IndexOf(DragSegment);
                    if (this.DragSegment.Series.Presenter != null && this.DragSegment.Series.Segments.Count > index)
                    {
                        DependencyObject obj = VisualTreeHelper.GetChild(this.DragSegment.Series.Presenter, index);
                        obj = VisualTreeHelper.GetChild(obj, 0);
                        if (obj is Canvas)
                        {
                            Canvas canvas = obj as Canvas;
                            if (canvas.Children.Count > 1 && canvas.Children[canvas.Children.Count - 1] is Popup)
                            {
                                Popup popup = canvas.Children[(canvas.Children.Count - 1)] as Popup;
                                if (popup != null)
                                {
                                    popup.IsOpen = true;
                                    popup.VerticalOffset = ((mousePoint.Y - this.ActualHeight) + (this.ValueToPoint(this.SecondaryAxis, this.DragSegment.YDataMeasure.End) - this.dragPointXY.Y) + this.AxesThickness.Bottom + 15);
                                    popup.HorizontalOffset = (mousePoint.X - (this.dragPointXY.X - this.ValueToPoint(this.PrimaryAxis, this.DragSegment.XDataMeasure.Start))) - this.AxesThickness.Left;
                                    this.horizontalOffset = mousePoint.X - (this.dragPointXY.X - this.ValueToPoint(this.PrimaryAxis, this.DragSegment.XDataMeasure.Start));
                                    this.verticalOffset = mousePoint.Y + (this.ValueToPoint(this.SecondaryAxis, this.DragSegment.YDataMeasure.End) - this.dragPointXY.Y);
                                  
                                }
                            }
                        }
                        if (obj is Grid)
                        {
                            Grid grid = obj as Grid;
                            if (grid.Children.Count > 1 && grid.Children[grid.Children.Count - 1] is Popup)
                            {
                                Popup popup = grid.Children[(grid.Children.Count - 1)] as Popup;
                                if (popup != null)
                                {
                                    popup.IsOpen = true;
                                    if (this.DragSegment.Series.Type != ChartTypes.StackingBar100 && this.DragSegment.Series.Type != ChartTypes.Bar && this.DragSegment.Series.Type != ChartTypes.Gantt && this.DragSegment.Series.Type != ChartTypes.StackingBar && this.DragSegment.Series.Type != ChartTypes.Tornado)
                                    {
                                        popup.VerticalOffset = ((mousePoint.Y - this.ActualHeight) + (this.ValueToPoint(this.SecondaryAxis, this.DragSegment.YDataMeasure.End) - this.dragPointXY.Y) + this.AxesThickness.Bottom) + 15;
                                        popup.HorizontalOffset = (mousePoint.X - (this.dragPointXY.X));
                                        this.horizontalOffset = mousePoint.X - (this.dragPointXY.X - this.ValueToPoint(this.PrimaryAxis, this.DragSegment.XDataMeasure.Start));
                                        this.verticalOffset = mousePoint.Y + (this.ValueToPoint(this.SecondaryAxis, this.DragSegment.YDataMeasure.End) - this.dragPointXY.Y);
                                    }
                                    else
                                    {
                                        popup.VerticalOffset = ((mousePoint.Y) - this.dragPointXY.Y);
                                        popup.HorizontalOffset = (mousePoint.X  - (this.dragPointXY.X - this.ValueToPoint(this.PrimaryAxis, this.DragSegment.XDataMeasure.Start))) - this.AxesThickness.Left;
                                        this.horizontalOffset = mousePoint.X - (this.dragPointXY.X - this.ValueToPoint(this.PrimaryAxis, this.DragSegment.XDataMeasure.Start));
                                        this.verticalOffset = mousePoint.Y;
                                    }
                                }
                            }
                        }
                    }
                    this.OnSegmentDragged(new SegmentDragEventArgs(this.DragSegment));
                }
                else
                {
                    if (this.DragSegment != null && this.DragSegment.Series != null && this.DragSegment.Series.Segments != null)
                    {
                        int index = this.DragSegment.Series.Segments.IndexOf(DragSegment);
                        if (this.DragSegment.Series.Presenter != null && this.DragSegment.Series.Segments.Count > index)
                        {
                            DependencyObject obj = VisualTreeHelper.GetChild(this.DragSegment.Series.Presenter, index);
                            obj = VisualTreeHelper.GetChild(obj, 0);
                            if (obj is Canvas)
                            {
                                Canvas canvas = obj as Canvas;
                                if (canvas.Children.Count > 1 && canvas.Children[canvas.Children.Count - 1] is Popup)
                                {
                                    Popup popup = canvas.Children[(canvas.Children.Count - 1)] as Popup;
                                    popup.IsOpen = false;
                                    this.DragSegment = null;
                                }
                            }
                            if (obj is Grid)
                            {
                                Grid grid = obj as Grid;
                                if (grid.Children.Count > 1 && grid.Children[grid.Children.Count - 1] is Popup)
                                {
                                    Popup popup = grid.Children[(grid.Children.Count - 1)] as Popup;
                                    popup.IsOpen = false;
                                    this.DragSegment = null;
                                }
                            }
                        }

                    }
                    this.OnSegmentDropped(new SegmentDropEventArgs(this.DragSegment));
                }

            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseDown"/>�attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. This event data reports details about the mouse was left and the handled state.</param>
        /// 
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (e.Source != null && e.Source is ChartArea)
            {
                ChartArea area = e.Source as ChartArea;
                if(area != null && area.Series != null)
                foreach (ChartSeries series in area.Series)
                {
                    if(series.Highlighted)
                    {
                        foreach (ChartSegment segment in series.Segments)
                        {
                            if(segment.Highlighted)
                                series.OnMouseLeave(series, new ChartMouseEventArgs(e, segment));
                        }
                    }
                }
            }
        }
 

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseDown"/>�attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. This event data reports details about the mouse button that was pressed and the handled state.</param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            FrameworkElement fElement = e.Device.Target as FrameworkElement;

            if (fElement != null && fElement.DataContext != null && fElement.DataContext is ChartSegment)
            {
                HitTestResult htr = VisualTreeHelper.HitTest(this, e.GetPosition(this));
                ChartSegment fastSegment = null;
                if (htr != null && htr.VisualHit is DrawingVisual)
                {
                    int index = ChartFastSeriesPresenter.GetIndex(htr.VisualHit);
                    ChartSegment segment = fElement.DataContext as ChartSegment;

                    if (segment != null)
                        fastSegment = GetChartSegmentForVisuals(segment, index);
                }
                if (fElement.DataContext != null)
                {
                    ChartSeries chartSeries = (fElement.DataContext as ChartSegment).Series;
                    if (fastSegment != null)
                    {
                        chartSeries.OnMouseDown(chartSeries, new ChartMouseEventArgs(e, fastSegment));
                    }
                    else
                    {
                        chartSeries.OnMouseDown(chartSeries, new ChartMouseEventArgs(e, fElement.DataContext as ChartSegment));
                    }
                    this.MousePressedSeries = chartSeries;
                }
            }

            base.OnMouseDown(e);
        }


        /// <summary>
        /// Raises the <see cref="E:System.Windows.Controls.Control.MouseDoubleClick"/> routed event. 
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            ChartCartesianAxisPanel v =Chart.FindAnchestor<ChartCartesianAxisPanel>((DependencyObject)e.OriginalSource);
            if (v !=null && (e.Source is ChartArea))
                    e.Handled = true;
            FrameworkElement fElement = e.Device.Target as FrameworkElement;

            if (fElement != null && fElement.DataContext != null && fElement.DataContext is ChartSegment)
            {
                HitTestResult htr = VisualTreeHelper.HitTest(this, e.GetPosition(this));
                ChartSegment fastSegment = null;
                if (htr != null && htr.VisualHit is DrawingVisual)
                {
                    int index = ChartFastSeriesPresenter.GetIndex(htr.VisualHit);
                    ChartSegment segment = fElement.DataContext as ChartSegment;

                    if (segment != null)
                        fastSegment = GetChartSegmentForVisuals(segment, index);
                }
                if (fElement.DataContext != null)
                {
                    ChartSeries chartSeries = (fElement.DataContext as ChartSegment).Series;
                    if (fastSegment != null)
                    {
                        chartSeries.OnMouseDoubleClick(chartSeries, new ChartMouseEventArgs(e, fastSegment));
                    }
                    else
                    {
                        if (chartSeries.Type == ChartTypes.Gantt)
                        {

                        }
                        chartSeries.OnMouseDoubleClick(chartSeries, new ChartMouseEventArgs(e, fElement.DataContext as ChartSegment));
                    }
                    this.MousePressedSeries = chartSeries;
                }
            }
            if (DragSegment != null && AllowSegmentDragDrop)
            {
                DragSegment = null;
                //AllowSegmentDragDrop = false;
            }
            base.OnMouseDoubleClick(e);
        }
       List<DependencyObject> hitTestList = null;      
      //This Is the HitTestResultCallBack method which returns the list of HitTest elements under Mouse Pointer.
      HitTestResultBehavior CollectAllVisuals_Callback(HitTestResult result)
      {
          if (result == null || result.VisualHit == null)
              return HitTestResultBehavior.Stop;

           hitTestList.Add(result.VisualHit);           
          return HitTestResultBehavior.Continue;
       }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseUp"/>�routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the mouse button was released.</param>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (e.Device != null)
            {
                FrameworkElement fElement = e.Device.Target as FrameworkElement;           

                

                if (fElement != null && fElement.DataContext is ChartSegment)
                {
                    int index = 0;
                    HitTestResult htr = VisualTreeHelper.HitTest(this, e.GetPosition(this));
                    ChartSegment fastSegment = null;
                    if (htr != null && htr.VisualHit is DrawingVisual)
                    {
                        index = ChartFastSeriesPresenter.GetIndex(htr.VisualHit);
                        ChartSegment segment = fElement.DataContext as ChartSegment;
                        if (segment != null)
                            fastSegment = GetChartSegmentForVisuals(segment, index);
                    }

                    ChartSeries chartSeries = (fElement.DataContext as ChartSegment).Series;
                    if (chartSeries != null)
                    {
                        if (fastSegment != null)
                        {
                            chartSeries.OnMouseUp(chartSeries, new ChartMouseEventArgs(e, fastSegment));
                        }
                        else
                        {
                            chartSeries.OnMouseUp(chartSeries, new ChartMouseEventArgs(e, (fElement.DataContext as ChartSegment)));
                        }
                        if (this.MousePressedSeries != null && this.MousePressedSeries == chartSeries)
                        {
                            if (fastSegment != null)
                            {
                                this.MousePressedSeries.OnMouseClick(this.MousePressedSeries, new ChartMouseEventArgs(e, fastSegment));
                            }
                            else
                            {
                                this.MousePressedSeries.OnMouseClick(this.MousePressedSeries, new ChartMouseEventArgs(e, (fElement.DataContext as ChartSegment)));
                            }
                            this.MousePressedSeries = null;
                        }
                    }
                }
                //SD12441 This Condition is included to make ChartSeries MouseUp and MouseClick events to listen even when Zooming Is enabled. 
                if (e.Device.Target != null && ChartZoomingToolkit.GetZoomingToolkitVisibility(e.Device.Target as DependencyObject) == Visibility.Visible)
                {
                    hitTestList = new List<DependencyObject>();

                    Point pt = e.GetPosition(null);

                    VisualTreeHelper.HitTest(this, null, CollectAllVisuals_Callback, new PointHitTestParameters(pt));

                    hitTestList.Reverse();

                    ChartSegment elementToFind = null;
                    foreach (object element in hitTestList)
                    {
                        FrameworkElement f_element = element as FrameworkElement;
                        if (f_element != null)
                            elementToFind = f_element.DataContext as ChartSegment;
                    }

                    if (this.MousePressedSeries != null && elementToFind != null)
                    {
                        this.MousePressedSeries.OnMouseUp(this.MousePressedSeries, new ChartMouseEventArgs(e, elementToFind));
                        this.MousePressedSeries.OnMouseClick(this.MousePressedSeries, new ChartMouseEventArgs(e, elementToFind));                        
                    }
                }
            }
            if (IsSync == true)
            {
                SyncChartAreas syncChartArea = this.ChartAreaParent;
                //syncChartArea.IsPanning_Sync = false;
                //Panning property value has been set for each area in SyncChartAreas
                foreach (ChartArea _area in syncChartArea.Areas)
                {
                    _area.IsPanning = false;
                }

            }
            timer.Stop();
            base.OnMouseUp(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonUp"/>�routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the left mouse button was released.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            _rangeSelection1 = false;
            _rangeSelection = false;

            if (this is SyncChartAreas)
            {
                SyncChartAreas syncChartArea = this as SyncChartAreas;
                syncChartArea.IsPanning = false;
                foreach (ChartArea area in syncChartArea.Areas)
                {
                    area.IsPanning = false;
                }
            }

            else if (IsSync == true)
            {
                if (this.ChartAreaParent != null)
                {
                    SyncChartAreas syncChartArea = this.ChartAreaParent;
                    foreach (ChartArea area in syncChartArea.Areas)
                    {
                        // syncChartArea.Cursor = Cursors.Arrow;
                        area.IsPanning = false;
                        //syncChartArea.SetAreaProperties();
                    }
                }
            }
            else
            {
                //this.Cursor = Cursors.Arrow;
                IsPanning = false;
            }
            if (View3DMode && this.Allow3DRotate)
            {
                this.ReleaseMouseCapture();
                if (m_mouseCaptureLocation != null)
                {
                    m_mouseCaptureLocation.X = double.NaN;
                    m_mouseCaptureLocation.Y = double.NaN;
                }
                if(m_capturedCursor !=null)
                Mouse.OverrideCursor = Cursors.Arrow;
            }

            if (e.Device != null)
            {
                FrameworkElement fElement = e.Device.Target as FrameworkElement;

                if (fElement != null && fElement.DataContext is ChartSegment)
                {
                    int index = 0;
                    HitTestResult htr = VisualTreeHelper.HitTest(this, e.GetPosition(this));
                    ChartSegment fastSegment = null;
                    if (htr != null && htr.VisualHit is DrawingVisual)
                    {
                        index = ChartFastSeriesPresenter.GetIndex(htr.VisualHit);
                        ChartSegment segment = fElement.DataContext as ChartSegment;
                        if (segment != null)
                            fastSegment = GetChartSegmentForVisuals(segment, index);
                    }

                    ChartSeries chartSeries = (fElement.DataContext as ChartSegment).Series;
                    if (chartSeries != null)
                    {
                        if (fastSegment != null)
                        {
                            chartSeries.OnMouseLeftButtonUp(chartSeries, new ChartMouseEventArgs(e, fastSegment));
                        }
                        else
                        {
                            chartSeries.OnMouseLeftButtonUp(chartSeries, new ChartMouseEventArgs(e, fElement.DataContext as ChartSegment));
                        }
                    }
                }
                //SD12441 This Condition is included to make ChartSeries MouseLeftUp events to listen even when Zooming Is enabled. 
                if (e.Device.Target != null && ChartZoomingToolkit.GetZoomingToolkitVisibility(e.Device.Target as DependencyObject) == Visibility.Visible)
                {
                    hitTestList = new List<DependencyObject>();

                    Point pt = e.GetPosition(null);

                    VisualTreeHelper.HitTest(this, null, CollectAllVisuals_Callback, new PointHitTestParameters(pt));

                    hitTestList.Reverse();

                    ChartSegment elementToFind = null;
                    foreach (object element in hitTestList)
                    {
                        FrameworkElement f_element = element as FrameworkElement;
                        if (f_element != null)
                            elementToFind = f_element.DataContext as ChartSegment;
                    }

                    if (this.MousePressedSeries != null && elementToFind != null)
                    {                        
                        this.MousePressedSeries.OnMouseLeftButtonUp(this.MousePressedSeries, new ChartMouseEventArgs(e, elementToFind));
                    }
                }
            }
            if (DragSegment != null && AllowSegmentDragDrop && m_areaPresenter != null && m_areaPresenter.AxesContainer != null)
            {
                if (DragSegment.CorrespondingPoints != null)
                {

                    DependencyObject obj = VisualTreeHelper.GetChild(this.DragSegment.Series.Presenter, 0);
                    obj = VisualTreeHelper.GetChild(obj, 0);

                     if (obj is Grid)
                       {
                            Grid grid = obj as Grid;
                            if (grid.Children.Count > 1 && grid.Children[1] is Popup)
                            {
                                Popup popup = grid.Children[1] as Popup;
                                popup.IsOpen = false;
                            }
                       } 
                   
                    
                    if (obj is Canvas)
                    {
                        Canvas canvas = obj as Canvas;
                        if (canvas.Children.Count > 1 && canvas.Children[1] is Popup)
                        {
                            Popup popup = canvas.Children[1] as Popup;
                            popup.IsOpen = false;
                        }
                    }
                    Random r1 = new Random();
                    Point pt1 = e.GetPosition(this);
                    double newptX = this.PointToValue(this.PrimaryAxis, pt1);
                    double newptY = this.PointToValue(this.SecondaryAxis, pt1);
                    double oldptX = this.PointToValue(this.PrimaryAxis, this.dragPointXY);
                    double oldpty = this.PointToValue(this.SecondaryAxis, this.dragPointXY);
                    this.OnSegmentDropping(new SegmentDropEventArgs(this.DragSegment));
                    if (this.DragSegment.Series.DataSource != null)
                    {
                        for (int i = 0; i < this.DragSegment.Series.Data.Count; i++)
                        {
                            if (this.DragSegment.Series.Data[i].X == DragSegment.CorrespondingPoints[0].DataPoint.X && this.DragSegment.Series.Data[i].Y == DragSegment.CorrespondingPoints[0].DataPoint.Y)
                            {
                                DependencyObject obj1 = VisualTreeHelper.GetChild(this.DragSegment.Series.Presenter, index);
                                obj1 = VisualTreeHelper.GetChild(obj1, 0);

                                if (obj1 is Grid)
                                {
                                    Grid grid = obj1 as Grid;
                                    if (grid.Children.Count > 1 && grid.Children[grid.Children.Count - 1] is Popup)
                                    {
                                        Popup popup = grid.Children[(grid.Children.Count - 1)] as Popup;
                                        object shapeObj = grid.Children[0] as object;
                                        double X = 0d, Y = 0d;
                                        if (this.DragSegment.Series.Type == ChartTypes.Gantt || this.DragSegment.Series.Type == ChartTypes.Tornado)
                                        {
                                            X = this.PointToValue(this.PrimaryAxis, e.GetPosition(this));
                                            X = X - (((this.DragSegment.XDataMeasure.End - this.DragSegment.XDataMeasure.Start) / 2) - (this.DragSegment.XDataMeasure.End - this.PointToValue(this.PrimaryAxis, this.dragPointXY)));
                                            Y = this.PointToValue(this.SecondaryAxis, new Point(this.horizontalOffset, this.verticalOffset + this.AxesThickness.Left)) - this.PointToValue(this.SecondaryAxis, new Point(0, this.ValueToPoint(this.SecondaryAxis, this.DragSegment.YDataMeasure.End) - this.dragPointXY.Y));
                                        }
                                        else if (this.DragSegment.Series.Type == ChartTypes.Bar)
                                        {
                                            X = this.PointToValue(this.PrimaryAxis, e.GetPosition(this));
                                            X = X - (((this.DragSegment.XDataMeasure.End - this.DragSegment.XDataMeasure.Start) / 2) - (this.DragSegment.XDataMeasure.End - this.PointToValue(this.PrimaryAxis, this.dragPointXY)));
                                            Y = this.PointToValue(this.SecondaryAxis, this.mousePoint) + (this.DragSegment.YDataMeasure.End - this.PointToValue(this.SecondaryAxis, this.dragPointXY));
                                        }
                                        // Drag and Drop values for Range Column was calculated.
                                        else if (this.DragSegment.Series.Type == ChartTypes.RangeColumn)
                                        {
                                            X = this.PointToValue(this.PrimaryAxis, new Point(this.horizontalOffset + (((System.Windows.Shapes.Shape)(shapeObj)).ActualWidth / 2), this.verticalOffset));
                                            Y = this.PointToValue(this.SecondaryAxis, new Point(this.horizontalOffset, this.verticalOffset));

                                            Y = this.PointToValue(this.SecondaryAxis, new Point(0, this.dragPointXY.Y)) - this.DragSegment.Series.Data[i].Values[0];
                                            Y = this.PointToValue(this.SecondaryAxis, e.GetPosition(this)) - Y;
                                        }
                                        else
                                        {
                                            X = this.PointToValue(this.PrimaryAxis, new Point(this.horizontalOffset + (((System.Windows.Shapes.Shape)(shapeObj)).ActualWidth / 2), this.verticalOffset));
                                            Y = this.PointToValue(this.SecondaryAxis, new Point(this.horizontalOffset + (((System.Windows.Shapes.Shape)(shapeObj)).ActualWidth / 2), this.verticalOffset));
                                        }
                                        double prevY1 = this.DragSegment.Series.Data[i].Values[0];
                                        value = new Point(newptX, newptY);
                                        if (this.DragSegment.Series.IsIndexed == false)
                                            this.DragSegment.Series.Data[i].X = X;

                                        this.DragSegment.Series.Data[i].Y = Y;
                                        double thisValue = 0d;
                                        for (int j = 0; j < this.DragSegment.Series.Data[i].Values.Length; j++)
                                        {
                                            if (j != 0)
                                            {
                                                thisValue = Y + (this.DragSegment.Series.Data[i].Values[j] > prevY1 ? (this.DragSegment.Series.Data[i].Values[j] - prevY1) : (prevY1 - this.DragSegment.Series.Data[i].Values[j]));
                                                prevY1 = this.DragSegment.Series.Data[i].Values[j];
                                                this.DragSegment.Series.Data[i].Values[j] = thisValue;
                                            }
                                        }
                                    }
                                }
                                    if (obj1 is Canvas)
                                    {
                                        Canvas canvas = obj1 as Canvas;
                                        if (canvas.Children.Count > 1 && canvas.Children[canvas.Children.Count - 1] is Popup)
                                        {
                                            Popup popup = canvas.Children[(canvas.Children.Count - 1)] as Popup;
                                            object shapeObj = canvas.Children[0] as object;
                                            double X = 0d, Y = 0d;

                                            X = this.PointToValue(this.PrimaryAxis, new Point(this.horizontalOffset + (((System.Windows.Shapes.Shape)(shapeObj)).ActualWidth / 2), this.verticalOffset));
                                            Y = this.PointToValue(this.SecondaryAxis, new Point(this.horizontalOffset + (((System.Windows.Shapes.Shape)(shapeObj)).ActualWidth / 2), this.verticalOffset));

                                            double prevY1 = this.DragSegment.Series.Data[i].Values[0];
                                            value = new Point(newptX, newptY);
                                            if (this.DragSegment.Series.IsIndexed == false)
                                                this.DragSegment.Series.Data[i].X = X;

                                            this.DragSegment.Series.Data[i].Y = Y;
                                            double thisValue = 0d;
                                            for (int j = 0; j < this.DragSegment.Series.Data[i].Values.Length; j++)
                                            {
                                                if (j != 0)
                                                {
                                                    thisValue = Y + (this.DragSegment.Series.Data[i].Values[j] > prevY1 ? (this.DragSegment.Series.Data[i].Values[j] - prevY1) : (prevY1 - this.DragSegment.Series.Data[i].Values[j]));
                                                    prevY1 = this.DragSegment.Series.Data[i].Values[j];
                                                    this.DragSegment.Series.Data[i].Values[j] = thisValue;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    
                    //double prevY1 = this.DragSegment.Series.Data[i].Values[0];
                    //value = new Point(newptX, newptY);
                    //if (this.DragSegment.Series.IsIndexed == false)
                    //    this.DragSegment.Series.Data[i].X = value.X;
                    //this.DragSegment.Series.Data[i].Y = value.Y;
                    //for (int j = 0; j < this.DragSegment.Series.Data[index].Values.Length; j++)
                    //{
                    //    if (j != 0)
                    //    {
                    //        this.DragSegment.Series.Data[i].Values[j] = this.DragSegment.Series.Data[i].Values[j - 1] + (this.DragSegment.Series.Data[i].Values[j] - prevY1);
                    //    }
                    //}

                    else
                    {
                        if (this.DragSegment.Series.IsIndexed)
                        {
                            for (int i = 0; i < this.DragSegment.Series.Data.Count; i++)
                            {
                                if (i == DragSegment.CorrespondingPoints[0].Index && this.DragSegment.Series.Data[i].Y == DragSegment.CorrespondingPoints[0].DataPoint.Y)
                                {
                                    DependencyObject obj1 = VisualTreeHelper.GetChild(this.DragSegment.Series.Presenter, index);
                                    obj1 = VisualTreeHelper.GetChild(obj1, 0);
                                    if (obj1 is Grid)
                                    {
                                        Grid grid = obj1 as Grid;
                                        if (grid.Children.Count > 1 && grid.Children[grid.Children.Count - 1] is Popup)
                                        {
                                            Popup popup = grid.Children[(grid.Children.Count - 1)] as Popup;
                                            object shapeObj = grid.Children[0] as object;
                                            double X = 0d, Y = 0d;
                                            if (this.DragSegment.Series.Type == ChartTypes.Gantt || this.DragSegment.Series.Type == ChartTypes.Tornado)
                                            {
                                                X = this.PointToValue(this.PrimaryAxis, e.GetPosition(this));
                                                X = X - (((this.DragSegment.XDataMeasure.End - this.DragSegment.XDataMeasure.Start) / 2) - (this.DragSegment.XDataMeasure.End - this.PointToValue(this.PrimaryAxis, this.dragPointXY)));
                                                Y = this.PointToValue(this.SecondaryAxis, new Point(this.horizontalOffset, this.verticalOffset + this.AxesThickness.Left)) - this.PointToValue(this.SecondaryAxis, new Point(0, this.ValueToPoint(this.SecondaryAxis, this.DragSegment.YDataMeasure.End) - this.dragPointXY.Y));
                                            }
                                            else if (this.DragSegment.Series.Type == ChartTypes.Bar)
                                            {
                                                X = this.PointToValue(this.PrimaryAxis, e.GetPosition(this));
                                                X = X - (((this.DragSegment.XDataMeasure.End - this.DragSegment.XDataMeasure.Start) / 2) - (this.DragSegment.XDataMeasure.End - this.PointToValue(this.PrimaryAxis, this.dragPointXY)));
                                                Y = this.PointToValue(this.SecondaryAxis, this.mousePoint) + (this.DragSegment.YDataMeasure.End - this.PointToValue(this.SecondaryAxis, this.dragPointXY));
                                            }
                                            // Drag and Drop values for Range Column was calculated.
                                            else if (this.DragSegment.Series.Type == ChartTypes.RangeColumn)
                                            {
                                                X = this.PointToValue(this.PrimaryAxis, new Point(this.horizontalOffset + (((System.Windows.Shapes.Shape)(shapeObj)).ActualWidth / 2), this.verticalOffset));
                                                Y = this.PointToValue(this.SecondaryAxis, new Point(this.horizontalOffset, this.verticalOffset));

                                                Y = this.PointToValue(this.SecondaryAxis, new Point(0, this.dragPointXY.Y)) - this.DragSegment.Series.Data[i].Values[0];
                                                Y = this.PointToValue(this.SecondaryAxis, e.GetPosition(this)) - Y;
                                            }

                                            else
                                            {
                                                X = this.PointToValue(this.PrimaryAxis, new Point(this.horizontalOffset + (((System.Windows.Shapes.Shape)(shapeObj)).ActualWidth / 2), this.verticalOffset));
                                                Y = this.PointToValue(this.SecondaryAxis, new Point(this.horizontalOffset + (((System.Windows.Shapes.Shape)(shapeObj)).ActualWidth / 2), this.verticalOffset));
                                            }

                                            double prevY1 = this.DragSegment.Series.Data[i].Values[0];
                                            value = new Point(newptX, newptY);
                                            if (this.DragSegment.Series.IsIndexed == false)
                                                this.DragSegment.Series.Data[i].X = X;

                                            this.DragSegment.Series.Data[i].Y = Y;
                                            double thisValue = 0d;
                                            for (int j = 0; j < this.DragSegment.Series.Data[i].Values.Length; j++)
                                            {
                                                if (j != 0)
                                                {
                                                    thisValue = Y + (this.DragSegment.Series.Data[i].Values[j] > prevY1 ? (this.DragSegment.Series.Data[i].Values[j] - prevY1) : (prevY1 - this.DragSegment.Series.Data[i].Values[j]));
                                                    prevY1 = this.DragSegment.Series.Data[i].Values[j];
                                                    this.DragSegment.Series.Data[i].Values[j] = thisValue;
                                                }
                                            }
                                        }
                                    }
                                    if (obj1 is Canvas)
                                    {
                                        Canvas canvas = obj1 as Canvas;
                                        if (canvas.Children.Count > 1 && canvas.Children[canvas.Children.Count - 1] is Popup)
                                        {
                                            Popup popup = canvas.Children[(canvas.Children.Count - 1)] as Popup;
                                            object shapeObj = canvas.Children[0] as object;
                                            double X = 0d, Y = 0d;
                                            
                                            X = this.PointToValue(this.PrimaryAxis, new Point(this.horizontalOffset + (((System.Windows.Shapes.Shape)(shapeObj)).ActualWidth / 2), this.verticalOffset));
                                            Y = this.PointToValue(this.SecondaryAxis, new Point(this.horizontalOffset + (((System.Windows.Shapes.Shape)(shapeObj)).ActualWidth / 2), this.verticalOffset));

                                            double prevY1 = this.DragSegment.Series.Data[i].Values[0];
                                            value = new Point(newptX, newptY);
                                            if (this.DragSegment.Series.IsIndexed == false)
                                                this.DragSegment.Series.Data[i].X = X;

                                            this.DragSegment.Series.Data[i].Y = Y;
                                            double thisValue = 0d;
                                            for (int j = 0; j < this.DragSegment.Series.Data[i].Values.Length; j++)
                                            {
                                                if (j != 0)
                                                {
                                                    thisValue = Y + (this.DragSegment.Series.Data[i].Values[j] > prevY1 ? (this.DragSegment.Series.Data[i].Values[j] - prevY1) : (prevY1 - this.DragSegment.Series.Data[i].Values[j]));
                                                    prevY1 = this.DragSegment.Series.Data[i].Values[j];
                                                    this.DragSegment.Series.Data[i].Values[j] = thisValue;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < this.DragSegment.Series.Data.Count; i++)
                            {
                                if (this.DragSegment.Series.Data[i].X == DragSegment.CorrespondingPoints[0].DataPoint.X && this.DragSegment.Series.Data[i].Y == DragSegment.CorrespondingPoints[0].DataPoint.Y)
                                {
                                    DependencyObject obj1 = VisualTreeHelper.GetChild(this.DragSegment.Series.Presenter, index);
                                    obj1 = VisualTreeHelper.GetChild(obj1, 0);
                                    if (obj1 is Grid)
                                    {
                                        Grid grid = obj1 as Grid;
                                        if (grid.Children.Count > 1 && grid.Children[grid.Children.Count - 1] is Popup)
                                        {
                                            Popup popup = grid.Children[(grid.Children.Count - 1)] as Popup;
                                            object shapeObj = grid.Children[0] as object;
                                            double X = 0d, Y = 0d;
                                            if (this.DragSegment.Series.Type == ChartTypes.Gantt || this.DragSegment.Series.Type == ChartTypes.Tornado)
                                            {
                                                X = this.PointToValue(this.PrimaryAxis, e.GetPosition(this));
                                                X = X - (((this.DragSegment.XDataMeasure.End - this.DragSegment.XDataMeasure.Start) / 2) - (this.DragSegment.XDataMeasure.End - this.PointToValue(this.PrimaryAxis, this.dragPointXY)));
                                                Y = this.PointToValue(this.SecondaryAxis, new Point(this.horizontalOffset, this.verticalOffset + this.AxesThickness.Left)) - this.PointToValue(this.SecondaryAxis, new Point(0, this.ValueToPoint(this.SecondaryAxis, this.DragSegment.YDataMeasure.End) - this.dragPointXY.Y));
                                            }
                                            else if (this.DragSegment.Series.Type == ChartTypes.Bar)
                                            {
                                                X = this.PointToValue(this.PrimaryAxis, e.GetPosition(this));
                                                X = X - (((this.DragSegment.XDataMeasure.End - this.DragSegment.XDataMeasure.Start) / 2) - (this.DragSegment.XDataMeasure.End - this.PointToValue(this.PrimaryAxis, this.dragPointXY)));
                                                Y = this.PointToValue(this.SecondaryAxis, this.mousePoint) + (this.DragSegment.YDataMeasure.End - this.PointToValue(this.SecondaryAxis, this.dragPointXY));
                                            }
                                            // Drag and Drop values for Range Column was calculated.
                                            else if (this.DragSegment.Series.Type == ChartTypes.RangeColumn)
                                            {
                                                X = this.PointToValue(this.PrimaryAxis, new Point(this.horizontalOffset + (((System.Windows.Shapes.Shape)(shapeObj)).ActualWidth / 2), this.verticalOffset));
                                                Y = this.PointToValue(this.SecondaryAxis, new Point(this.horizontalOffset, this.verticalOffset));

                                                Y = this.PointToValue(this.SecondaryAxis, new Point(0, this.dragPointXY.Y)) - this.DragSegment.Series.Data[i].Values[0];
                                                Y = this.PointToValue(this.SecondaryAxis, e.GetPosition(this)) - Y;
                                            }

                                            else
                                            {
                                                X = this.PointToValue(this.PrimaryAxis, new Point(this.horizontalOffset + (((System.Windows.Shapes.Shape)(shapeObj)).ActualWidth / 2), this.verticalOffset));
                                                Y = this.PointToValue(this.SecondaryAxis, new Point(this.horizontalOffset + (((System.Windows.Shapes.Shape)(shapeObj)).ActualWidth / 2), this.verticalOffset));
                                            }

                                            double prevY1 = this.DragSegment.Series.Data[i].Values[0];
                                            value = new Point(newptX, newptY);
                                            if (this.DragSegment.Series.IsIndexed == false)
                                                this.DragSegment.Series.Data[i].X = X;

                                            this.DragSegment.Series.Data[i].Y = Y;
                                            double thisValue = 0d;
                                            for (int j = 0; j < this.DragSegment.Series.Data[i].Values.Length; j++)
                                            {
                                                if (j != 0)
                                                {
                                                    thisValue = Y + (this.DragSegment.Series.Data[i].Values[j] > prevY1 ? (this.DragSegment.Series.Data[i].Values[j] - prevY1) : (prevY1 - this.DragSegment.Series.Data[i].Values[j]));
                                                    prevY1 = this.DragSegment.Series.Data[i].Values[j];
                                                    this.DragSegment.Series.Data[i].Values[j] = thisValue;
                                                }
                                            }
                                        }
                                    }
                                    if (obj1 is Canvas)
                                    {
                                        Canvas canvas = obj1 as Canvas;
                                        if (canvas.Children.Count > 1 && canvas.Children[canvas.Children.Count - 1] is Popup)
                                        {
                                            Popup popup = canvas.Children[(canvas.Children.Count - 1)] as Popup;
                                            object shapeObj = canvas.Children[0] as object;
                                            double X = 0d, Y = 0d;
                                            
                                                X = this.PointToValue(this.PrimaryAxis, new Point(this.horizontalOffset + (((System.Windows.Shapes.Shape)(shapeObj)).ActualWidth / 2), this.verticalOffset));
                                                Y = this.PointToValue(this.SecondaryAxis, new Point(this.horizontalOffset + (((System.Windows.Shapes.Shape)(shapeObj)).ActualWidth / 2), this.verticalOffset));
                                            
                                            double prevY1 = this.DragSegment.Series.Data[i].Values[0];
                                            value = new Point(newptX, newptY);
                                            if (this.DragSegment.Series.IsIndexed == false)
                                                this.DragSegment.Series.Data[i].X = X;

                                            this.DragSegment.Series.Data[i].Y = Y;
                                            double thisValue = 0d;
                                            for (int j = 0; j < this.DragSegment.Series.Data[i].Values.Length; j++)
                                            {
                                                if (j != 0)
                                                {
                                                    thisValue = Y + (this.DragSegment.Series.Data[i].Values[j] > prevY1 ? (this.DragSegment.Series.Data[i].Values[j] - prevY1) : (prevY1 - this.DragSegment.Series.Data[i].Values[j]));
                                                    prevY1 = this.DragSegment.Series.Data[i].Values[j];
                                                    this.DragSegment.Series.Data[i].Values[j] = thisValue;
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                        }
                    }


                    this.isAllowDragDrop = true;
                    m_visibleSeriesSegmentsRecountRequired = true;

                    this.UpdateArea();

                    m_visibleSeriesSegmentsRecountRequired = false;
                }
                this.OnSegmentDropped(new SegmentDropEventArgs(this.DragSegment));
                DragSegment = null;
                this.isAllowDragDrop = false;

            }
            base.OnMouseLeftButtonUp(e);
        }

        internal Point dragPointXY = new Point();        
        DispatcherTimer timer = new DispatcherTimer();
        Point args = new Point();
        FrameworkElement element = null;
        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/>�routed event is raised on this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the left mouse button was pressed.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (View3DMode && (this.Chart3DSettings ?? new Chart3D()).RotateOnMouseDown && this.Allow3DRotate)
            {
                m_mouseCaptureLocation = e.GetPosition(this);
                m_capturedCursor = Mouse.OverrideCursor;
                if (Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    Mouse.OverrideCursor = Cursors.Hand;
                }
                else
                {
                    Mouse.OverrideCursor = Cursors.SizeAll;
                }

                this.CaptureMouse();
            }

            FrameworkElement fElement = e.Device.Target as FrameworkElement;

            if (fElement != null && fElement.DataContext is ChartSegment)
            {
                int index = 0;
                HitTestResult htr = VisualTreeHelper.HitTest(this, e.GetPosition(this));
                ChartSegment fastSegment = null;
                if (htr != null && htr.VisualHit is DrawingVisual)
                {
                    index = ChartFastSeriesPresenter.GetIndex(htr.VisualHit);
                    ChartSegment segment = fElement.DataContext as ChartSegment;
                    if (segment != null)
                        fastSegment = GetChartSegmentForVisuals(segment, index);
                }

                ChartSeries chartSeries = (fElement.DataContext as ChartSegment).Series;
                if (chartSeries != null)
                {
                    if (fastSegment != null)
                    {
                        chartSeries.OnMouseLeftButtonDown(chartSeries, new ChartMouseEventArgs(e, fastSegment));
                    }
                    else
                    {
                        chartSeries.OnMouseLeftButtonDown(chartSeries, new ChartMouseEventArgs(e, fElement.DataContext as ChartSegment));
                    }
                }
            }

            SyncChartAreas syncChartArea = this.ChartAreaParent;

            if ((IsSync == true) && (syncChartArea.ZoomAllAxes))
            {

                if ((syncChartArea != null) && (syncChartArea.ZoomedXRange.Delta != 0.0) && (syncChartArea.Cursor == Cursors.Hand))
                {
                    foreach (ChartArea area in syncChartArea.Areas)
                    {

                        this.IsPanning = true;
                        if (area.index == 0)
                        {
                            foreach (ChartAxis axis in area.Axes)
                            {
                                if (axis.Orientation == Orientation.Horizontal)
                                {

                                    Point point = e.GetPosition(this);
                                    area.lastPosition_X = area.PointToValue(axis, point);
                                }

                            }
                        }
                    }


                }

            }
            else if ((this.ZoomAllAxes) && (this.IsSync == false) && (this.Cursor == Cursors.Hand))
            {
                if (this.ZoomedXRange.Delta != 0.0)
                {
                    this.IsPanning = true;
                    Point point = e.GetPosition(this);
                    foreach (ChartAxis axis in this.Axes)
                    {
                        if (axis.Orientation == Orientation.Horizontal)
                        {
                            axis.lastPosition_X = this.PointToValue(axis, point);
                        }
                        else
                        {
                            axis.lasPosition_Y = this.PointToValue(axis, point);
                        }

                    }
                }
            }
            
            args = e.GetPosition(this);
            element = e.Device.Target as FrameworkElement;
            timer.Start();         
            
            
            base.OnMouseLeftButtonDown(e);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (args != null)
            {
                if (element != null && element.DataContext is ChartSegment)
                {            
                    this.DragSegment = (element.DataContext as ChartSegment);
                    this.dragPointXY = new Point();
                    this.dragPointXY = args;
                    this.OnSegmentDragging(new SegmentDragEventArgs(this.DragSegment));
                    
                }
            }
            timer.Stop();
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseWheel"/>�attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseWheelEventArgs"/> that contains the event data.</param>
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (View3DMode)
            {
                if (Camera3D is PerspectiveCamera)
                {
                    CameraController.Length -= (double)e.Delta / 1000;
                    if (CameraController.Length < 0.5)
                    {
                        CameraController.Length = 0.5;
                    }
                }
                else if (Camera3D is OrthographicCamera)
                {
                    OrthographicCamera camera = Camera3D as OrthographicCamera;
                    camera.Width -= (double)e.Delta / 1000;
                    if (camera.Width < 0.5)
                    {
                        camera.Width = 0.5;
                    }
                }
            }

            if (this.EnableZoomOnScroll)
            {
                if (e.Delta < 0)
                {
                    ChartAreaCommands.ZoomIn.Execute(null, this);
                }
                else
                {
                    ChartAreaCommands.ZoomOut.Execute(null, this);
                }
            }

            base.OnMouseWheel(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseRightButtonUp"/>�routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the right mouse button was released.</param>
        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            FrameworkElement fElement = e.Device.Target as FrameworkElement;

            if (fElement != null && fElement.DataContext is ChartSegment)
            {
                ChartSeries chartSeries = (fElement.DataContext as ChartSegment).Series;
                chartSeries.OnMouseRightButtonUp(chartSeries, new ChartMouseEventArgs(e, fElement.DataContext as ChartSegment));
            }

            base.OnMouseRightButtonUp(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseRightButtonDown"/>�routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the right mouse button was pressed.</param>
        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            FrameworkElement fElement = e.Device.Target as FrameworkElement;

            if (fElement != null && fElement.DataContext is ChartSegment)
            {
                ChartSeries chartSeries = (fElement.DataContext as ChartSegment).Series;
                chartSeries.OnMouseRightButtonDown(chartSeries, new ChartMouseEventArgs(e, fElement.DataContext as ChartSegment));
            }

            base.OnMouseRightButtonDown(e);
        }
        internal ChartAnnotationsAdorner annotationAdorner = null;
        private Adorner adorner = null;
        /// <summary>
        /// Set area presenter
        /// </summary>
        /// <param name="areaPresenter">The ChartAreaPresenter</param>
        internal void SetAreaPresenter(ChartAreaPresenter areaPresenter)
        {
            if (m_areaPresenter == null)
            {
                m_areaPresenter = areaPresenter;
                OnAreaPresenterEvent(this);
            }

            ////Adding chart annotations adorner.
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(m_areaPresenter);
            if (m_areaPresenter.SeriesContainer != null)
            {
                try
                {
                    if ((Visibility)this.GetValue(ChartZoomingToolkit.ZoomingToolkitVisibilityProperty) == Visibility.Visible)
                    {
                        ChartAreaCommands.SwitchZooming.Execute(null, this);
                    }

                    if(layer != null)
                    {
                    if (adorner == null)
                    {
                        adorner = new ChartAnnotationsAdorner(m_areaPresenter.SeriesContainer, this);
                        annotationAdorner = adorner as ChartAnnotationsAdorner;
                        layer.Add(adorner);
                    }
                    else
                    {
                        annotationAdorner = adorner as ChartAnnotationsAdorner;
                        layer.Add(adorner);
                    }
                    }
                   
                   
                  
                    
                }
                catch 
                {
                }
            }

            if (this.IsSync && this.ChartAreaParent != null && this.ChartAreaParent is SyncChartAreas)
            {
                this.ChartAreaParent.SetBindings(this);
            }

        }

        /// <summary>
        /// Raised on SeriesCollectionChanged
        /// </summary>
        /// <param name="sender">The Object sender</param>
        /// <param name="args">The NotifyCollectionChangedEventArguments</param>
        private void OnSeriesCollectionChanged(Object sender, NotifyCollectionChangedEventArgs args)
        {

            ChartSeriesCollection collection = sender as ChartSeriesCollection;
            if (collection != null)
            {
                foreach (ChartSeries seriesStyle in this.Series)
                {
                    seriesStyle.legendicon = seriesStyle.LegendIcon;
                    if (this.SeriesStyle != null)
                        setSeriesStyle(this.SeriesStyle, this);
                }
            }
            if (args.NewItems != null)
            {
                foreach (ChartSeries series in args.NewItems)
                {
                    try
                    {
                        AddLogicalChild(series);
                    }
                    catch
                    {
                    }
                    series.Area = this;
                    
                    series.CoerceValue(ChartSeries.XAxisProperty);
                    series.CoerceValue(ChartSeries.YAxisProperty);
                    series.CoerceValue(ChartSeries.ZAxisProperty);

                    if (series.XAxis == null)
                    {
                        Binding binding = new Binding();

                        binding.Path = new PropertyPath(ChartArea.PrimaryAxisProperty);
                        binding.Source = this;

                        BindingOperations.SetBinding(series, ChartSeries.XAxisProperty, binding);
                    }
                    else
                    {
                        if (!series.Area.Axes.Contains(series.XAxis))
                        {
                            series.Area.Axes.Add(series.XAxis);
                        }
                    }

                    if (series.YAxis == null)
                    {
                        Binding binding = new Binding();

                        binding.Path = new PropertyPath(ChartArea.SecondaryAxisProperty);
                        binding.Source = this;

                        BindingOperations.SetBinding(series, ChartSeries.YAxisProperty, binding);
                    }
                    else
                    {
                        if (!series.Area.Axes.Contains(series.YAxis))
                        {
                            series.Area.Axes.Add(series.YAxis);
                        }
                    }
                    //For 3D Chart 
                    if (series.ZAxis == null)
                    {
                        Binding binding = new Binding();

                        binding.Path = new PropertyPath(ChartArea.DepthAxisProperty);
                        binding.Source = this;

                        BindingOperations.SetBinding(series, ChartSeries.ZAxisProperty, binding);
                    }
                    else
                    {
                        if (!series.Area.Axes.Contains(series.ZAxis))
                        {
                            series.Area.Axes.Add(series.ZAxis);
                        }
                    }
                    ////Data changes are handeled cuz Appearance changed does it all.
                    ////series.DataChanged += new EventHandler(OnSeriesDataChanged);
                    series.AppearanceChanged += new EventHandler(OnSeriesAppearanceChanged);
                }
            }

            if (args.OldItems != null)
            {
                foreach (ChartSeries series in args.OldItems)
                {
                    RemoveLogicalChild(series);
                }
            }

            m_visibleSeriesSegmentsRecountRequired = true;
            this.UpdateArea();
        }
        /// <summary>
        /// Raised on SeriesAppearanceChanged
        /// </summary>
        /// <param name="sender">The object sender</param>
        /// <param name="e">The EventArguments e</param>
        private void OnSeriesAppearanceChanged(object sender, EventArgs e)
        {
            ChartSeries series = sender as ChartSeries;
            if (series != null)
            {
                m_visibleSeriesSegmentsRecountRequired = true;
                this.UpdateArea();
            }
        }

        /// <summary>
        /// Raised on AxesCollectionChanged
        /// </summary>
        /// <param name="sender">The object sender</param>
        /// <param name="e">The NotifyCollectionChangedEventArguments e</param>
        private void OnAxesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            if (e.NewItems != null)
            {
                foreach (ChartAxis axis in e.NewItems)
                {
                    if (axis != null)
                    {
                        //if (baseRd == null)
                        //{
                        //    baseRd = new ResourceDictionary()
                        //    {
                        //        Source = new Uri("/Syncfusion.Chart.Wpf;component/Themes/Generic.Base.xaml", UriKind.RelativeOrAbsolute)
                        //    };
                        //}
                        //if (baseRd != null)
                        //    axis.LabelTemplate = baseRd["defaultLableTemplate"] as DataTemplate;

                        try
                        {
                            axis.Area = this;
                            axis.Changed += new EventHandler(OnAxisChanged);

                            Binding dataContext = new Binding();
                            dataContext.Path = new PropertyPath("DataContext");
                            dataContext.Source = this;
                            BindingOperations.SetBinding(axis, ChartAxis.DataContextProperty, dataContext);

                            this.AddLogicalChild(axis);
                        }
                        catch 
                        {
                        }
                    }
                }
            }

            if (e.OldItems != null)
            {
                foreach (ChartAxis axis in e.OldItems)
                {
                    axis.Area = null;
                    axis.Changed -= new EventHandler(OnAxisChanged);
                    this.RemoveLogicalChild(axis);
                }
            }

            //ChartAxesCollection collection = sender as ChartAxesCollection;
            //SetAxesStyle(collection);

            //foreach (ChartAxis styleaxis in collection)
            //{
            //    if (SecondaryAxisStyle != null)
            //    {
            //        if (styleaxis.Orientation == Orientation.Vertical)
            //        {
            //            styleaxis.Style = SecondaryAxisStyle;
            //        }
            //        else
            //        {
            //            styleaxis.Style = PrimaryAxisStyle;
            //        }
            //    }
            //}
        }

        private ResourceDictionary rd = null;
        private ResourceDictionary baseRd = null;
        internal void SetAxesStyle(ChartAxesCollection collection)
        {

            if (collection == null)
                return;
            if (baseRd == null)
            {
                baseRd = ChartDictionaries.GenericBaseDictionary;
               // baseRd = new SharedResourceDictionary()
               //{
               //    Source = new Uri("/Syncfusion.Chart.Wpf;component/Themes/Generic.Base.xaml", UriKind.RelativeOrAbsolute)
               //};
            }
            if (rd != null)
            {
                foreach (ChartAxis styleaxis in collection)
                {
                    Chart chart = this.Parent as Chart;
                    if (chart != null)
                    {
                        if (styleaxis.VisibleLabels.Count > 0)
                        {
                            //object obj1 = Enum.GetName(typeof(ChartStyles), chart.StyleIndexValue);
                            //if (obj1.ToString() == "Blend" || obj1.ToString() == "Office2007Black")
                            //{
                            //    styleaxis.tempForeground = styleaxis.LabelForeground;

                            //    styleaxis.Style = rd["BlendAxis"] as Style;
                            //    styleaxis.SetHeaderStyle();

                            //}
                            //else
                            //{
                            //    styleaxis.Style = rd["OtherSkinAxis"] as Style;
                            //    styleaxis.SetHeaderStyle();
                            //}
                        }
                    }
                }

            }
        }

        /// <summary>
        /// Called when axis gets changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnAxisChanged(object sender, EventArgs e)
        {

            AxisChangedEventArgs axisArgs = e as AxisChangedEventArgs;

            if (axisArgs != null && axisArgs.DependencyPropertyEventArgs.Property == ChartAxis.InternalRangeProperty)
            {
                if (m_visibleSeriesSegmentsRecountRequired == false)
                {
                    m_visibleSeriesSegmentsRecountRequired = true;
                    UpdateArea();
                }
            }
            else if (m_areaPresenter != null)
            {
                m_areaPresenter.InvalidateChildren();
            }
        }

        /// <summary>
        /// Raised on SeriesDataChanged
        /// </summary>
        /// <param name="sender">The object sender</param>
        /// <param name="e">The EventArguments e</param>
        private void OnSeriesDataChanged(object sender, EventArgs e)
        {
            m_visibleSeriesSegmentsRecountRequired = true;
            this.Redraw();
            this.UpdateArea();
        }

        private static void OnGridLineStrokeChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartArea area = d as ChartArea;
            if (area != null)
            {
                if (area.View3DMode)
                {
                    area.UpdateArea();
                }
            }
        }
        /// <summary>
        /// Verifies whether chart type is either Pie, Doughnut, Funnel or Pyramid.
        /// </summary>
        /// <param name="chartType">The <see cref="ChartType"/></param>
        /// <returns>
        /// True if chart type is either Pie, Doughnut, Funnel or Pyramid.
        /// </returns>
        internal static bool CheckCompatibility(ChartType chartType)
        {
            return chartType is ChartPieType || chartType is ChartFunnelType || chartType is ChartPyramidType;
        }

        /// <summary>
        /// Redraws this instance.
        /// </summary>
        internal void Redraw()
        {
            if (m_areaPresenter != null)
            {
                m_areaPresenter.InvalidateChildren();
            }
        }


        /// <summary>
        /// Updates the chart area.
        /// </summary>
        public void UpdateChartArea()
        {
            if (this.Series != null)
            {
                if (this.Series.Count > 0)
                {
                    foreach (ChartSeries item in this.Series)
                    {
                        if (item != null)
                        {
                            if ((item.Data as ChartBindingData) != null)
                            {
                                (item.Data as ChartBindingData).EndInit();
                            }
                        }
                    }
                }
            }
        }

        internal bool m_IsUpdateArea = true;

        /// <summary>
        /// Updates the area.
        /// </summary>
        internal void UpdateArea()
        {
            
            if (this.updateArea == true && this.m_IsUpdateArea&& this.HoldUpdate == false )
            {
                var visibleSeries = m_visibleSeries;
                m_minPointsDelta = double.NaN;
                if (visibleSeries != null && visibleSeries.Count > 0 && visibleSeries[0].ChartType != null)
                {
                    this.AreaType = visibleSeries[0].ChartType.AxesType;
                }

                if (m_visibleSeriesSegmentsRecountRequired)
                {
                    if (visibleSeries != null)
                    {
                        foreach (ChartSeries series in visibleSeries)
                        {
                            
                                if (((m_series.Count > 1) || m_segmentsResetRequired) && series.Segments != null)
                                {
                                    if ((series.Segments.Count > 0 && !(series.Segments[0] is ChartFastLineSegment)) || series.internaldata_modified)
                                    {
                                        series.Segments.Clear();
                                        if (series.Adornments != null)
                                        {
                                            series.Adornments.Clear();
                                        }
                                    }

                                }

                                m_segmentsResetRequired = false;

                                if ((series.Data != null && series.Data.Count >= 0) || series.DataSource == null)
                                    series.Invalidate();
                                //if ((series.Segments.Count > 0 && (series.Segments[0] is ChartFastLineSegment) && !this.SecondaryAxis.IsAutoSetRange))
                                //{
                                //    series.Recalculate();
                                //}
                            
                        }

                        this.m_visibleSeriesSegmentsRecountRequired = false;
                    }
                }

                if (this.Series != null)
                {
                    //if (this.AreaSegments.Count > 0)
                    //    this.AreaSegments.Clear();
                    foreach (ChartAxis axis in this.Axes)
                    {
                        if (axis.IsAutoSetRange)
                        {
                            this.CalculateAxisRange(axis);
                        }
                            // Below condition is included to improve the autoscrolling performance
                        else if (axis.EnableAutoScrolling == null || axis.Orientation != Orientation.Vertical || axis.isNeedUpdate)
                        {
                            axis.Invalidate();
                            //axis.RaiseChanged(axis, EventArgs.Empty);
                            axis.isNeedUpdate = false;
                        }
                       
                    }
                    foreach(ChartSeries series in this.Series)
                    {
                        foreach (ChartAxis axis in this.Axes)
                        {
                            if (axis.Orientation == Orientation.Vertical)
                            {
                                if (axis.IsAutoSetRange)
                                {
                                    if (series.ChartType.ToString() == "Histogram")
                                    {
                                        ChartHistogramType.SetIntervalOfHistogram(series, this.Axes[0].m_visibleInterval);
                                        {
                                            CalculateAxisRange(axis);
                                        }
                                    }
                                }
                                else
                                {
                                      if (series.ChartType.ToString() == "Histogram")
                                      {
                                            ChartHistogramType.SetIntervalOfHistogram(series, Axes[0].BaseInterval);
                                      }
                                 }
                             }
                        }
                    }
                    foreach (ChartSeries series in this.Series)
                    {
                        if (series.Indicators != null)
                        {
                            foreach (var item in series.Indicators.Items)
                            {
                                if (item.IndicatorType == IndicatorTypes.BollingerBands)
                                {
                                    foreach (ChartAxis axis in this.Axes)
                                    {
                                        if (axis.Orientation == Orientation.Vertical && item.BollingerIndicator != null && (item.BollingerIndicator.MaxYValue > axis.m_visibleRange.End || item.BollingerIndicator.MinYValue > axis.m_visibleRange.Start))
                                        {
                                            double valueX = item.BollingerIndicator.MinYValue == 0 ? axis.m_visibleRange.Start : item.BollingerIndicator.MinYValue + axis.m_visibleInterval;
                                            double valueY = item.BollingerIndicator.MaxYValue == 0 ? axis.m_visibleRange.End : item.BollingerIndicator.MaxYValue + axis.m_visibleInterval;
                                            axis.m_visibleRange = new DoubleRange(valueX, valueY);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (View3DMode)
                {
                    CreateCamera();
                    Update3DContent();
                    UpdateLight();
                    if (this.AreaType != ChartAxesType.None)
                    {
                        Update3DAxis();
                        CreateAxisContent();
                        UpdateGridLines();
                    }
                    else
                    {
                        Labels3DContent = null;
                        GridContent = null;
                        AxisContent = null;
                    }
                }


                if (this is SyncChartAreas)
                {
                    if ((this as SyncChartAreas) != null)
                    {
                        (this as SyncChartAreas).SetAreaProperties();

                    }
                }

                if (this is TimeLineControl)
                {
                    TimeLineControl instance = this as TimeLineControl;
                    if (instance != null)
                    {
                        if (instance.LeftOffsetX != instance.RightOffsetX && instance.SelectedRegionWidth > 1)
                        {
                            Rect rect = new Rect(new Point(instance.LeftOffsetX + 17, 0), new Point(instance.RightOffsetX + 17, instance.ActualHeight));
                            instance.SelectedData = instance.BoundsToDataSource(rect, instance.PrimarySeries);
                        }
                    }
                }

                if (m_areaPresenter != null)
                {
                    m_areaPresenter.InvalidateChildren();
                }

                if (this.AreaSegments != null)
                {
                    this.AreaSegments.Clear();
                }
                if (this.VisibleSeries != null)
                {
                    foreach (ChartSeries ser in this.VisibleSeries)
                    {
                        //this.AreaSegments.Add(ser);
                        //Included Null check to avoid null reference exception
                        if (ser.Segments != null)
                        {
                            foreach (ChartSegment seg in ser.Segments)
                            {
                                this.AreaSegments.Add(seg);
                            }
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Updates the grid lines.
        /// </summary>
        /// <seealso cref="ChartArea"/>
        private void UpdateGridLines()
        {
            if (this.Axes != null)
            {
                if (!this.IsClustered)
                {
                    Model3DGroup group = new Model3DGroup();

                    Chart3DGrid grid = new Chart3DGrid(this.PrimaryAxis, this.SecondaryAxis);
                    grid.Side = GridSide.Back;
                    grid.Height = 500;
                    grid.Width = 500;

                    MaterialGroup materialGroup = new MaterialGroup();
                    VisualBrush brush = new VisualBrush(grid);
                    RenderOptions.SetCachingHint(brush, CachingHint.Cache);
                    materialGroup.Children.Add(new DiffuseMaterial(brush));
                    materialGroup.Children.Add(new EmissiveMaterial(brush));

                    GeometryModel3D geometry = new GeometryModel3D();

                    geometry.Geometry = MeshGenerator.PlaneZ(1, 1);
                    geometry.Material = materialGroup;
                    geometry.BackMaterial = materialGroup;
                    geometry.Transform = new TranslateTransform3D(0, 0, this._backWallThickness - 0.02);

                    group.Children.Add(geometry);

                    grid = new Chart3DGrid(this.PrimaryAxis, this.SecondaryAxis);
                    grid.Side = GridSide.Left;
                    grid.Height = 500;
                    grid.Width = 500;

                    materialGroup = new MaterialGroup();
                    brush = new VisualBrush(grid);
                    materialGroup.Children.Add(new DiffuseMaterial(brush));
                    materialGroup.Children.Add(new EmissiveMaterial(brush));

                    geometry = new GeometryModel3D();
                    geometry.Geometry = MeshGenerator.PlaneX(1, 0.30 * Series.Count);
                    geometry.Material = materialGroup;
                    geometry.BackMaterial = materialGroup;

                    geometry.Transform = new TranslateTransform3D(this.SecondaryAxis.OpposedPosition == false ? -0.499 : +0.499, 0, 0.15);

                    group.Children.Add(geometry);

                    grid = new Chart3DGrid(this.PrimaryAxis, this.SecondaryAxis);
                    grid.Side = GridSide.Bottom;
                    grid.Height = 500;
                    grid.Width = 500;

                    materialGroup = new MaterialGroup();
                    brush = new VisualBrush(grid);
                    materialGroup.Children.Add(new DiffuseMaterial(brush));
                    materialGroup.Children.Add(new EmissiveMaterial(brush));

                    geometry = new GeometryModel3D();
                    geometry.Geometry = MeshGenerator.PlaneY(1, 0.30 * Series.Count);
                    geometry.Material = materialGroup;
                    geometry.BackMaterial = materialGroup;

                    Transform3DGroup groupTransform = new Transform3DGroup();
                    geometry.Transform = new TranslateTransform3D(0, PrimaryAxis.OpposedPosition == false ? -0.499 : +0.499, 0.15);

                    group.Children.Add(geometry);

                    GridContent = group;
                }
                else
                {
                    Model3DGroup group = new Model3DGroup();
                    #region BackWall

                    Chart3DGrid grid;
                    if (this.EnableDepthAxis)
                        grid = new Chart3DGrid(this.PrimaryAxis, this.SecondaryAxis, this.DepthAxis);
                    else
                        grid = new Chart3DGrid(this.PrimaryAxis, this.SecondaryAxis);

                    grid.Side = GridSide.Back;
                    grid.Height = 500;
                    grid.Width = 500;

                    MaterialGroup materialGroup = new MaterialGroup();
                    VisualBrush brush = new VisualBrush(grid);
                    RenderOptions.SetCachingHint(brush, CachingHint.Cache);
                    materialGroup.Children.Add(new DiffuseMaterial(brush));
                    materialGroup.Children.Add(new EmissiveMaterial(brush));

                    GeometryModel3D geometry = new GeometryModel3D();
                    geometry.Geometry = MeshGenerator.PlaneZ(1, 1);
                    geometry.Material = materialGroup;
                    geometry.BackMaterial = materialGroup;
                    if (this.isClustered == false)
                        geometry.Transform = new TranslateTransform3D(0, 0, this._backWallThickness);
                    else
                        geometry.Transform = new TranslateTransform3D(0, 0, this.EnableDepthAxis ? 0.01 : 0.028);

                    group.Children.Add(geometry);
                    #endregion
                    #region LeftSideWall

                    if (this.EnableDepthAxis)
                        grid = new Chart3DGrid(this.PrimaryAxis, this.SecondaryAxis, this.DepthAxis);
                    else
                        grid = new Chart3DGrid(this.PrimaryAxis, this.SecondaryAxis);

                    grid.Side = GridSide.Left;
                    grid.Height = 500;
                    grid.Width = 500;

                    materialGroup = new MaterialGroup();
                    brush = new VisualBrush(grid);
                    materialGroup.Children.Add(new DiffuseMaterial(brush));
                    materialGroup.Children.Add(new EmissiveMaterial(brush));

                    geometry = new GeometryModel3D();
                    geometry.Geometry = MeshGenerator.PlaneX(1, this.EnableDepthAxis ? 1 : 0.30);
                    geometry.Material = materialGroup;
                    geometry.BackMaterial = materialGroup;

                    geometry.Transform = new TranslateTransform3D(this.SecondaryAxis.OpposedPosition == false ? -0.499 : +0.499, 0, this.EnableDepthAxis ? 0.499 : 0.15);

                    group.Children.Add(geometry);
                    #endregion
                    #region BottomFloor(X)

                    if (this.EnableDepthAxis)
                        grid = new Chart3DGrid(this.PrimaryAxis, this.SecondaryAxis, this.DepthAxis);
                    else
                        grid = new Chart3DGrid(this.PrimaryAxis, this.SecondaryAxis);

                    grid.Side = GridSide.Bottom;
                    grid.Height = 500;
                    grid.Width = 500;

                    materialGroup = new MaterialGroup();
                    brush = new VisualBrush(grid);
                    materialGroup.Children.Add(new DiffuseMaterial(brush));
                    materialGroup.Children.Add(new EmissiveMaterial(brush));

                    geometry = new GeometryModel3D();
                    geometry.Geometry = MeshGenerator.PlaneY(1, this.EnableDepthAxis ? 1 : 0.30);
                    geometry.Material = materialGroup;
                    geometry.BackMaterial = materialGroup;

                    Transform3DGroup groupTransform = new Transform3DGroup();
                    geometry.Transform = new TranslateTransform3D(0, PrimaryAxis.OpposedPosition == false ? -0.501 : +0.499, this.EnableDepthAxis ? 0.498 : 0.15);

                    group.Children.Add(geometry);
                    #endregion

                    if (this.EnableDepthAxis)
                    {
                        #region BottomFloor(Z)
                        //grid = new Chart3DGrid(this.PrimaryAxis, this.SecondaryAxis, this.DepthAxis);
                        //grid.Side = GridSide.Bottom;
                        //grid.Height = 500;
                        //grid.Width = 500;

                        //materialGroup = new MaterialGroup();
                        //brush = new VisualBrush(grid);
                        //materialGroup.Children.Add(new DiffuseMaterial(brush));
                        //materialGroup.Children.Add(new EmissiveMaterial(brush));

                        //geometry = new GeometryModel3D();
                        //geometry.Geometry = MeshGenerator.PlaneY(1, 1);
                        //geometry.Material = materialGroup;
                        //geometry.BackMaterial = materialGroup;

                        //Transform3DGroup groupTransform1 = new Transform3DGroup();
                        //TranslateTransform3D translate2 = new TranslateTransform3D(0.499, +0.489, 0);
                        //RotateTransform3D rotate2 = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 1), 180));
                        //groupTransform1.Children.Add(translate2);
                        //groupTransform1.Children.Add(rotate2);
                        //geometry.Transform = groupTransform1;
                        //group.Children.Add(geometry);

                        #endregion
                        #region LeftSideWall(Z)

                        grid = new Chart3DGrid(this.PrimaryAxis, this.SecondaryAxis, this.DepthAxis);
                        grid.Side = GridSide.Left;
                        grid.Height = 1000;
                        grid.Width = 500;

                        materialGroup = new MaterialGroup();
                        brush = new VisualBrush(grid);
                        materialGroup.Children.Add(new DiffuseMaterial(brush));
                        materialGroup.Children.Add(new EmissiveMaterial(brush));

                        geometry = new GeometryModel3D();
                        geometry.Geometry = MeshGenerator.PlaneX(1, 1);
                        geometry.Material = materialGroup;
                        geometry.BackMaterial = materialGroup;

                        Transform3DGroup groupTransform2 = new Transform3DGroup();
                        TranslateTransform3D translate1 = new TranslateTransform3D(-0.489, 0.489, 0);
                        RotateTransform3D rotate1 = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), 90));
                        groupTransform2.Children.Add(translate1);
                        groupTransform2.Children.Add(rotate1);
                        geometry.Transform = groupTransform2;//new TranslateTransform3D(this.SecondaryAxis.OpposedPosition == false ? -0.499 : +0.499, 0, 0.15);

                        group.Children.Add(geometry);

                        #endregion
                    }
                    GridContent = group;
                }
            }
        }

        /// <summary>
        /// Creates the camera.
        /// </summary>
        private void CreateCamera()
        {
            Chart3D settings = this.Chart3DSettings ?? new Chart3D();
            if (settings.CameraProjection == CameraProjection.Perspective)
            {
                if (this.Camera3D is OrthographicCamera)
                {
                    PerspectiveCamera myPCamera = new PerspectiveCamera();
                    myPCamera.FieldOfView = 90;
                    ChartTargetCameraController camer = new ChartTargetCameraController(myPCamera);
                    camer.Length = 1.3;
                    camer.Rotate = this.CameraController.Rotate;
                    camer.Tilt = this.CameraController.Tilt;
                    camer.Turn = this.CameraController.Turn;
                    this.CameraController = camer;
                    this.CameraController.Target = new Vector3D(0, -0.05, 0);

                    this.Camera3D = myPCamera;
                }
            }
            else if (settings.CameraProjection == CameraProjection.Orthographic)
            {
                if (this.Camera3D is PerspectiveCamera)
                {
                    OrthographicCamera myPCamera = new OrthographicCamera();
                    myPCamera.Width = 2.3;
                    ChartTargetCameraController camer = new ChartTargetCameraController(myPCamera);
                    ////camer.Length = 10;
                    camer.Rotate = this.CameraController.Rotate;
                    camer.Tilt = this.CameraController.Tilt;
                    camer.Turn = this.CameraController.Turn;
                    this.CameraController = camer;
                    this.CameraController.Target = new Vector3D(0, -0.05, 0);
                    this.Camera3D = myPCamera;
                }
            }
        }

        /// <summary>
        /// Updetes the light.
        /// </summary>
        private void UpdateLight()
        {
            Chart3D settings = this.Chart3DSettings ?? new Chart3D();
            if (settings.ChartLight == null)
            {
                Model3DGroup group = new Model3DGroup();

                //AmbientLight ambLight = new AmbientLight(Color.FromArgb(200, 200, 200, 200));
                //group.Children.Add(ambLight);
                //<DirectionalLight Direction="-.3,-1,-.4" Color="#999999"/>
                //                <DirectionalLight Direction="-.2,0,-1" Color="#999999"/>
                //                <DirectionalLight Direction="-.3,1,-1" Color="#999999"/>
                //                <AmbientLight Color="#555555"/>
                //DirectionalLight directionalLight = new DirectionalLight();
                //directionalLight.Color = Colors.LightGray;
                //directionalLight.Direction = new Vector3D(-0.5, -0.5, -0.5);

                DirectionalLight directionalLight1 = new DirectionalLight();
                directionalLight1.Color = Color.FromRgb(153, 153, 153);
                directionalLight1.Direction = new Vector3D(-0.3, -1, -0.4);

                DirectionalLight directionalLight2 = new DirectionalLight();
                directionalLight2.Color = Color.FromRgb(153, 153, 153);
                directionalLight2.Direction = new Vector3D(-0.2, 0, -1);
                
                
                DirectionalLight directionalLight3 = new DirectionalLight();
                directionalLight2.Color = Color.FromRgb(153, 153, 153);
                directionalLight3.Direction = new Vector3D(-0.3, 1, -1);

                AmbientLight amblight = new AmbientLight(Color.FromRgb(85, 85, 85));

                group.Children.Add(directionalLight1);
                group.Children.Add(directionalLight2);
                group.Children.Add(directionalLight3);
                group.Children.Add(amblight);

                LightContent = group;
            }
            else
            {
                LightContent = settings.ChartLight;
            }
        }

        /// <summary>
        /// Updates the content of the 3D.
        /// </summary>
        /// <seealso cref="ChartArea"/>
        private void Update3DContent()
        {
            Model3DGroup group = new Model3DGroup();
            {
                if (m_visibleSeries != null)
                {
                    foreach (ChartSeries series in m_visibleSeries)
                    {
                        series.Recalculate3D();
                        group.Children.Add(series.Segments3D);
                    }
                }
            }

            Content3D = group;
        }

        /// <summary>
        /// Updates the 3D axis.
        /// </summary>
        /// <seealso cref="ChartArea"/>
        private void Update3DAxis()
        {
            if (this.Axes != null)
            {
                Model3DGroup group = new Model3DGroup();
                m_axes3DShiftLeftOrientation = new Point3D();
                m_axes3DShiftRightOrientation = new Point3D();

                Chart3D settings = this.Chart3DSettings ?? new Chart3D();

                if (settings.ShowPrimaryAxis)
                {
                    group.Children.Add(DrawAxis(this.PrimaryAxis));
                }

                if (settings.ShowSecondaryAxis)
                {
                    group.Children.Add(DrawAxis(this.SecondaryAxis));
                }

                if (this.EnableDepthAxis)
                {
                    if (settings.ShowDepthAxis)
                    {
                        group.Children.Add(DrawAxis(this.DepthAxis));
                    }
                }

                foreach (ChartAxis axis in this.Axes)
                {
                    if (axis != this.PrimaryAxis && axis != this.SecondaryAxis && axis != this.DepthAxis)
                    {
                        group.Children.Add(DrawAxis(axis));
                    }
                }

                Labels3DContent = group;
            }
        }

        /// <summary>
        /// Drawes the axis.
        /// </summary>
        /// <param name="axis">The axis value.</param>
        /// <returns>The geometry</returns>
        private GeometryModel3D DrawAxis(ChartAxis axis)
        {
            GeometryModel3D geometry = new GeometryModel3D();
            CartesianAxis3D m_axis3d = new CartesianAxis3D(axis);
            m_axis3d.FontSize = 12d;
            m_axis3d.FontWeight = FontWeights.Bold;
           //SD17347 Serialization problem in 3DChart this.AddLogicalChild(m_axis3d);
            SharedResourceDictionary baseRD = new SharedResourceDictionary()
            {
                Source = new Uri("/Syncfusion.Chart.Wpf;component/3DChart/CartesianAxis3D.xaml", UriKind.RelativeOrAbsolute)
            };
            if (baseRD != null)
            {
                m_axis3d.Style =(Style)baseRD["CartesianAxis3DStyle"];
            }
            bool x = m_axis3d.ApplyTemplate();

            if (EnvironmentTest.IsSecurityGranted)
            {
                Assembly asm = Assembly.GetAssembly(typeof(System.Windows.Shapes.Ellipse));
                string typename = Assembly.CreateQualifiedName(asm.FullName, "MS.Internal.Data.DataBindEngine");
                Type type = Type.GetType(typename);

                PropertyInfo prop = type.GetProperty("CurrentDataBindEngine", BindingFlags.NonPublic | BindingFlags.Static);

                object value = prop.GetValue(null, null);

                MethodInfo methodRun = type.GetMethod("Run", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(object) }, null);

                methodRun.Invoke(value, new object[] { true });
            }

            if (!this.IsClustered)
            {

                Size size = new Size();
                size.Height = 500;
                size.Width = 500;
                m_axis3d.Measure(size);
                if (axis.Orientation == Orientation.Vertical)
                {
                    double shift;

                    m_axis3d.Height = 500;
                    m_axis3d.Width = 1000;
                    shift = m_axis3d.DesiredSize.Width / 500;

                    geometry.Geometry = MeshGenerator.PlaneZ(shift, 1);
                    if (!axis.OpposedPosition)
                    {
                        geometry.Transform = new TranslateTransform3D(-0.500 - m_axes3DShiftLeftOrientation.Y - shift / 2, 0, 0.301 + (0.15 * (Series.Count - 1)));
                        m_axes3DShiftLeftOrientation.Y += shift;
                    }
                    else
                    {
                        geometry.Transform = new TranslateTransform3D(0.500 + m_axes3DShiftRightOrientation.Y + shift / 2, 0, 0.301 + (0.15 * (Series.Count - 1)));
                        m_axes3DShiftRightOrientation.Y += shift;
                    }
                }
                else
                {
                    double shift;

                    m_axis3d.Width = 500;
                    m_axis3d.Height = 1000;
                    shift = m_axis3d.DesiredSize.Height / 500;

                    geometry.Geometry = MeshGenerator.PlaneZ(1, shift);
                    if (!axis.OpposedPosition)
                    {
                        geometry.Transform = new TranslateTransform3D(0, -0.500 - m_axes3DShiftLeftOrientation.X - shift / 2, 0.301 + (0.15 * (Series.Count - 1)));
                        m_axes3DShiftLeftOrientation.X += shift;
                    }
                    else
                    {
                        geometry.Transform = new TranslateTransform3D(0, 0.500 + m_axes3DShiftRightOrientation.X + shift / 2, 0.301 + (0.15 * (Series.Count - 1)));
                        m_axes3DShiftRightOrientation.X += shift;
                    }
                }
            }
            else
            {
                Size size = new Size();
                size.Height = 500;
                size.Width = 500;
                m_axis3d.Measure(size);
                if (axis.Orientation == Orientation.Vertical)
                {
                    double shift;

                    m_axis3d.Height = 500;
                    m_axis3d.Width = 1000;
                    shift = m_axis3d.DesiredSize.Width / 500;

                    geometry.Geometry = MeshGenerator.PlaneZ(shift, 1);
                    if (!axis.OpposedPosition)
                    {
                        geometry.Transform = new TranslateTransform3D(-0.500 - m_axes3DShiftLeftOrientation.Y - shift / 2, 0, this.EnableDepthAxis ? 0.999 : 0.301);
                        m_axes3DShiftLeftOrientation.Y += shift;
                    }
                    else
                    {
                        geometry.Transform = new TranslateTransform3D(0.500 + m_axes3DShiftRightOrientation.Y + shift / 2, 0, this.EnableDepthAxis ? 0.999 : 0.301);
                        m_axes3DShiftRightOrientation.Y += shift;
                    }
                }
                else
                {
                    double shift;
                    if (axis.depthaxisflag)
                    {
                        m_axis3d.Width = 500;
                        m_axis3d.Height = 1000;
                        shift = m_axis3d.DesiredSize.Height / 500;
                        Transform3DGroup transGrp = new Transform3DGroup();
                        geometry.Geometry = MeshGenerator.PlaneZ(1, shift);
                        if (!axis.OpposedPosition)
                        {
                            RotateTransform3D rotate = new RotateTransform3D(new AxisAngleRotation3D() { Angle = 90, Axis = new Vector3D(0, 1, 0) });
                            TranslateTransform3D translate = new TranslateTransform3D(0.500 + m_axes3DShiftRightOrientation.X, -0.500 - m_axes3DShiftRightOrientation.X - shift / 2, 0.500 + m_axes3DShiftRightOrientation.Z);
                            transGrp.Children.Add(rotate);
                            transGrp.Children.Add(translate);
                            geometry.Transform = transGrp;
                            m_axes3DShiftLeftOrientation.Z += shift;
                        }
                        else
                        {
                            geometry.Transform = new TranslateTransform3D(0, 0.500 + m_axes3DShiftRightOrientation.X + shift / 2, 0.301);
                            m_axes3DShiftRightOrientation.X += shift;
                        }
                    }
                    else
                    {
                        m_axis3d.Width = 500;
                        m_axis3d.Height = 1000;
                        shift = m_axis3d.DesiredSize.Height / 500;

                        geometry.Geometry = MeshGenerator.PlaneZ(1, shift);
                        if (!axis.OpposedPosition)
                        {
                            geometry.Transform = new TranslateTransform3D(0, -0.500 - m_axes3DShiftLeftOrientation.X - shift / 2, this.EnableDepthAxis ? 0.999 : 0.301);
                            m_axes3DShiftLeftOrientation.X += shift;
                        }
                        else
                        {
                            geometry.Transform = new TranslateTransform3D(0, 0.500 + m_axes3DShiftRightOrientation.X + shift / 2, this.EnableDepthAxis ? 0.999 : 0.301);
                            m_axes3DShiftRightOrientation.X += shift;
                        }
                    }
                }
            }
            MaterialGroup materialGroup = new MaterialGroup();
            VisualBrush brush = new VisualBrush(m_axis3d);
            //Added to increase the performance.
            RenderOptions.SetCachingHint(brush, CachingHint.Cache);
            materialGroup.Children.Add(new DiffuseMaterial(brush));
            materialGroup.Children.Add(new EmissiveMaterial(brush));

            geometry.Material = materialGroup;
            geometry.BackMaterial = materialGroup;
            return geometry;
        }

        /// <summary>
        /// Computes range of axis by series.
        /// </summary>
        /// <param name="axis">The axis value.</param>
        private void CalculateAxisRange(ChartAxis axis)
        {
            DoubleRange range = DoubleRange.Empty;
            if (axis != null && m_visibleSeries != null)
            {
                foreach (ChartSeries series in m_visibleSeries)
                {
                    if (axis == series.ActualXAxis)
                    {
                        if (series.Data != null && axis.IsSetDataValueRange)
                        {
                            range += CalculateVisibleDataRange(axis, series);
                        }
                        else
                            range += series.XRange;
                        //range = CalculateXRange(axis, series, range);
                    }
                    if (axis == series.ActualYAxis)
                    {
                        if (series.Data != null && axis.IsSetDataValueRange)
                        {
                            range += CalculateVisibleDataRange(axis, series);
                        }
                        else
                        {
                            if (series.ChartType.ToString()=="Histogram")
                            {
                                if (series.Segments.Count > 0)
                                {
                                    foreach (var seg in series.Segments)
                                    {
                                        range += seg.yRange;
                                    }
                                }
                            }
                            else
                             {
                                  range += series.YRange;
                             }
                        }
                        //range = CalculateYRange(axis,series, range);
                    }
                    if (axis.Area.EnableDepthAxis)
                    {
                        if (axis == series.ActualZAxis)
                        {
                            if (series.Data != null && axis.IsSetDataValueRange)
                            {
                                range += CalculateVisibleDataRange(axis, series);
                            }
                            else
                                range += series.ZRange;
                        }
                    }
                }
            }
            if (range.Delta == 0)
            {
                range = new DoubleRange(range.Start, range.End + 1);
            }
            axis.SetNiceRange(range.IsEmpty ? new DoubleRange(0, 1) : range);
        }

        private  DoubleRange CalculateYRange(ChartAxis axis, ChartSeries series, DoubleRange range)
        {
            switch (series.ActualYAxis.RangeCalculationMode)
            {
                case RangeCalculationMode.AdjustAcrossChartTypes:
                case RangeCalculationMode.ConsistentAcrossChartTypes:
                    range = CalculatePaddingforDefaultandAdjustAcross(axis, series, range, axis.VisibleInterval);
                    break;
                default:
                    break;
            }
            return range;

        }
        private DoubleRange CalculateXRange(ChartAxis axis, ChartSeries series, DoubleRange range)
        {
            switch (series.ActualXAxis.RangeCalculationMode)
            {
                case RangeCalculationMode.AdjustAcrossChartTypes:
                   range= CalculatePaddingforDefaultandAdjustAcross(axis, series, range,axis.VisibleInterval);
                    break;
                case RangeCalculationMode.ConsistentAcrossChartTypes:
                    range = CalculatePaddingforConsistentMode(axis,series,range);
                    break;
                default:
                    break;
            }
            return range;
        }

        private DoubleRange CalculatePaddingforConsistentMode(ChartAxis axis, ChartSeries series, DoubleRange range)
        {
            if (series.ActualXAxis.RangePadding != ChartRangePaddingType.None)
            {
                switch (series.Type)
                {
                    case ChartTypes.Column:
                    case ChartTypes.Bar:
                    case ChartTypes.Gantt:
                    case ChartTypes.Histogram:
                    case ChartTypes.RangeColumn:
                    case ChartTypes.StackingBar:
                    case ChartTypes.StackingBar100:
                    case ChartTypes.StackingColumn:
                    case ChartTypes.StackingColumn100:
                        //range = new DoubleRange(ChartType.GetColumnInitialSegmentWidth(series), ChartType.GetEndSegmentWidth(series));
                        break;
                    case ChartTypes.Scatter:
                    case ChartTypes.FastScatter:
                    case ChartTypes.Bubble:
                        //range += range.Start - axis.VisibleInterval;
                        //range += range.End + axis.VisibleInterval;
                        break;
                    default:
                        break;
                }
            }
            return range;
        }


        private DoubleRange CalculatePaddingforDefaultandAdjustAcross(ChartAxis axis, ChartSeries series, DoubleRange range,double interval)
        {
            switch (series.Type)
            {
                case ChartTypes.Scatter:
                case ChartTypes.FastScatter:
                case ChartTypes.Bubble:
                        //range += range.Start-interval;
                        //range += range.End + interval;
                    break;
                default:
                    break;
            }
            return range;
        }
       
        /// <summary>
        /// Calculates the Range based on datavalue if IsSetDataValue=true and IsAutoSetRange=true
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="series"></param>
        /// <returns></returns>
        private DoubleRange CalculateVisibleDataRange(ChartAxis axis, ChartSeries series)
        {
             var startrange = 0d;
             if (series.Data.Count > 0)
             {
                 IEnumerable<IChartDataPoint> point = (IEnumerable<IChartDataPoint>)series.Data;
                 if (axis == series.ActualXAxis)
                 {
                     startrange = double.IsNaN(series.YAxis.ActualRange.Start) ? 0 : series.YAxis.ActualRange.Start;
                     _endrange = double.IsNaN(series.YAxis.ActualRange.End) ? 0 : series.YAxis.ActualRange.End;
                     if (point != null)
                     {
                         var xrange = (from pt in point
                                       where (pt.Y >= startrange && pt.Y <= _endrange)
                                       select pt.X);
                         return (new DoubleRange(xrange.Min(), xrange.Max()));
                     }

                 }
                 if (axis == series.ActualYAxis)
                 {
                     startrange = double.IsNaN(series.XAxis.ActualRange.Start) ? 0 : series.XAxis.ActualRange.Start;
                     _endrange = double.IsNaN(series.XAxis.ActualRange.End) ? 0 : series.XAxis.ActualRange.End;
                     if (point != null)
                     {
                         var yrange = (from pt in point
                                       where (pt.X >= startrange && pt.X <= _endrange)
                                       select pt.Y);
                         var rangepadvalue = (axis.Orientation == Orientation.Horizontal && axis.RangePadding == ChartRangePaddingType.None) ? axis.BaseInterval / 2 : 0d;
                         return (new DoubleRange(yrange.Min() - rangepadvalue, yrange.Max()));
                     }
                 }


                 if (axis == series.ActualZAxis)
                 {
                     startrange = double.IsNaN(series.ZAxis.ActualRange.Start) ? 0 : series.ZAxis.ActualRange.Start;
                     _endrange = double.IsNaN(series.ZAxis.ActualRange.End) ? 0 : series.ZAxis.ActualRange.End;
                     if (point != null)
                     {
                         var yrange = (from pt in point
                                       where (pt.Values[2] >= startrange && pt.Values[2] <= _endrange)
                                       select pt.Values[2]);
                         var rangepadvalue = (axis.Orientation == Orientation.Horizontal && axis.RangePadding == ChartRangePaddingType.None) ? axis.BaseInterval / 2 : 0d;
                         return (new DoubleRange(yrange.Min() - rangepadvalue, yrange.Max()));
                     }
                 }
             }
            return (DoubleRange.Empty);
        }

        /// <summary>
        /// Called when axis need coerce.
        /// </summary>
        /// <param name="dObj">The d obj.</param>
        /// <param name="value">The value.</param>
        /// <returns>The Axis value</returns>
        private static object OnCoerceAxis(DependencyObject dObj, object value)
        {
            ChartArea area = dObj as ChartArea;
            if (area.isDisposed == false)
            {
                return value == null ? new ChartAxis() : value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Called when [coerce show grid lines].
        /// </summary>
        /// <param name="dObj">The DependencyObject d.</param>
        /// <param name="value">The value.</param>
        /// <returns>The Grid lines value</returns>
        private static object OnCoerceShowGridLines(DependencyObject dObj, object value)
        {
            ChartAxis axis = dObj as ChartAxis;

            if (axis != null)
            {
                if (axis.ReadLocalValue(ChartArea.ShowGridLinesProperty) == null)
                {
                    return axis.Area.PrimaryAxis == axis || axis.Area.SecondaryAxis == axis;
                }

                return value;
            }

            return value;
        }

        private static object OnCoerceShowMajorGridLines(DependencyObject dObj, object value)
        {
            ChartAxis axis = dObj as ChartAxis;

            if (axis != null)
            {
                if (axis.ReadLocalValue(ChartArea.ShowMajorGridLinesProperty) == null)
                {
                    return axis.Area.PrimaryAxis == axis || axis.Area.SecondaryAxis == axis;
                }

                return value;
            }

            return value;
        }

        /// <summary>
        /// Called when [coerce show origin lines].
        /// </summary>
        /// <param name="dObj">The DependencyObject d.</param>
        /// <param name="value">The value.</param>
        /// <returns>The Origin lines value</returns>
        private static object OnCoerceShowOriginLines(DependencyObject dObj, object value)
        {
            ChartAxis axis = dObj as ChartAxis;

            if (axis != null)
            {
                if (axis.ReadLocalValue(ChartArea.ShowOriginLineProperty) == null)
                {
                    return axis.Area.PrimaryAxis == axis || axis.Area.SecondaryAxis == axis;
                }

                //axis.Area.UpdateArea();
                return value;
            }

            return value;
        }

        /// <summary>
        /// Zooms in sector regarding IsZooming property on series. IsZooming is ignored if ZoomAllAxes is true.
        /// </summary>
        /// <param name="target">The object target</param>
        /// <param name="args">The ExecutedRoutedEventArgs args</param>
        private static void OnZoomInCommand(object target, ExecutedRoutedEventArgs args)
        {
            ChartArea chartArea = target as ChartArea;
            chartArea.m_scrolling = false;
            SyncChartAreas syncChartArea = chartArea.ChartAreaParent;
            if (syncChartArea == null && chartArea is SyncChartAreas)
            {
                syncChartArea = chartArea as SyncChartAreas;
            }
            int chartAreaCount;
            if ((syncChartArea != null) || (chartArea != null))
            {
                chartArea.PanningFlag = false;
                if (chartArea.IsSync == true)
                {
                    foreach (ChartArea _area in syncChartArea.Areas)
                    {
                        _area.Cursor = Cursors.Arrow;
                    }
                    //syncChartArea.Cursor = Cursors.Arrow;
                }
                else
                {
                    chartArea.Cursor = Cursors.Arrow;
                }
            }
            if (syncChartArea != null)
            {
                syncChartArea.Cursor = Cursors.Arrow;
                chartAreaCount = syncChartArea.Areas.Count;
                if (syncChartArea.IsSyncChartArea == true)
                {
                    foreach (ChartArea chartArea_sync in syncChartArea.Areas)
                    {
                        syncChartArea.SetEnableZoomingForYAxis(chartArea_sync);
                        ZoomInChartArea(chartArea_sync, chartAreaCount, syncChartArea.IsSyncChartArea);
                    }
                }

                //  syncChartArea.SetAreaProperties();

            }
            else if (chartArea != null)
            {
                chartArea.Cursor = Cursors.Arrow;
                ZoomInChartArea(chartArea, 0, false);
            }
            chartArea.OnChartZoomed(new ChartZoomedEventArgs(chartArea));
            chartArea.ResetFlag = false;
        }
        internal bool isIndicator = false;
        internal DoubleRange VerticalUnPanZoomPosition = new DoubleRange();
        internal DoubleRange HorizontalUnPanZoomPosition = new DoubleRange();
        private static void ZoomInChartArea(ChartArea chartArea, int chartAreaCount, bool isSyncChartArea)
        {
            ////If no target is passed.
            if (chartArea != null)
            {
                chartArea.ChartAreaIndex = chartArea.index;
                chartArea.ChartAreaCount = chartAreaCount;

                if (isSyncChartArea == true)
                {
                    if (chartArea.HorizontalScrollingAxis.ZoomFactor > chartArea.HorizontalScrollingAxis.MinimalZoomFactor)
                    {
                        if (chartArea.isIndicator == true)
                        {
                            chartArea.HorizontalScrollingAxis.MulZoomCenter(chartArea.m_ZoomInCoefficient);
                        }
                        else
                        {
                            chartArea.HorizontalScrollingAxis.MulZoomCenter(chartArea.m_ZoomInCoefficient);
                        }
                        chartArea.VisibleRangeForZoomHorizontalAxis(chartArea);
                    }

                }
                else
                {
                    ////If we're up to zoom all axes.
                    if (chartArea.ZoomAllAxes)
                    {
                        foreach (ChartAxis chartAxis in chartArea.Axes)
                        {
                            ////Zoom in all axes in chart axes collection.
                            if (chartAxis.EnableZooming && (chartAxis.ZoomFactor > chartAxis.MinimalZoomFactor))
                            {

                                chartAxis.MulZoomCenter(chartArea.m_ZoomInCoefficient);
                                chartArea.VisibileRangeForZoomAllAxis(chartArea, chartAxis);
                                if (chartAxis.Orientation == Orientation.Vertical)
                                {
                                    chartArea.VerticalUnPanZoomPosition = chartAxis.VisibleRange;
                                }
                                else
                                {
                                    chartArea.HorizontalUnPanZoomPosition = chartAxis.VisibleRange;
                                }
                            }
                        }
                    }
                    else
                    {
                        ////Otherwise zoom in only currently selected axis. 
                        if (chartArea.VerticalScrollingAxis.EnableZooming && chartArea.VerticalScrollingAxis.ZoomFactor > chartArea.VerticalScrollingAxis.MinimalZoomFactor)
                        {
                            chartArea.VerticalScrollingAxis.MulZoomCenter(chartArea.m_ZoomInCoefficient);
                            chartArea.VisibleRangeForZoomVerticalAxis(chartArea);
                        }
                        if (chartArea.HorizontalScrollingAxis.EnableZooming && chartArea.HorizontalScrollingAxis.ZoomFactor > chartArea.HorizontalScrollingAxis.MinimalZoomFactor)
                        {
                            chartArea.HorizontalScrollingAxis.MulZoomCenter(chartArea.m_ZoomInCoefficient);
                            chartArea.VisibleRangeForZoomHorizontalAxis(chartArea);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Raised on ZoomOutCommand
        /// </summary>
        /// <param name="target">The object target</param>
        /// <param name="args">The ExecutedRoutedEventArgs args</param>
        private static void OnZoomOutCommand(object target, ExecutedRoutedEventArgs args)
        {

            ChartArea chartArea = target as ChartArea;
            chartArea.m_scrolling = false;
            SyncChartAreas syncChartArea = chartArea.ChartAreaParent;
            if (syncChartArea == null && chartArea is SyncChartAreas)
            {
                syncChartArea = chartArea as SyncChartAreas;
            }
            if ((syncChartArea != null) || (chartArea != null))
            {
                chartArea.PanningFlag = false;
                if (chartArea.IsSync == true)
                {
                    foreach (ChartArea _area in syncChartArea.Areas)
                    {
                        _area.Cursor = Cursors.Arrow;
                    }
                    //syncChartArea.Cursor = Cursors.Arrow;
                }
                else
                {
                    chartArea.Cursor = Cursors.Arrow;
                }
            }


            int chartAreaCount;
            if (syncChartArea != null)
            {
                chartAreaCount = syncChartArea.Areas.Count;
                if (syncChartArea.IsSyncChartArea == true)
                {
                    foreach (ChartArea chartArea_sync in syncChartArea.Areas)
                    {
                        ZoomOutChartArea(chartArea_sync, chartAreaCount, syncChartArea.IsSyncChartArea);
                    }
                }

                //  syncChartArea.SetAreaProperties();
            }
            else if (chartArea != null)
            {
                ZoomOutChartArea(chartArea, 0, false);
            }
            chartArea.OnChartZoomedOut(new ChartZoomedOutEventArgs(chartArea));
        }
        private static void ZoomOutChartArea(ChartArea chartArea, int chartAreaCount, bool isSyncChartArea)
        {
            ////If no target is passed.
            if (chartArea != null)
            {
                chartArea.ChartAreaIndex = chartArea.index;
                chartArea.ChartAreaCount = chartAreaCount;
                if (isSyncChartArea == true)
                {
                    chartArea.HorizontalScrollingAxis.MulZoomCenter(chartArea.m_ZoomOutCoefficient);
                    chartArea.VisibleRangeForZoomHorizontalAxis(chartArea);
                }
                else
                {
                    ////If we're up to zoom all axes.
                    if (chartArea.ZoomAllAxes)
                    {
                        foreach (ChartAxis chartAxis in chartArea.Axes)
                        {
                            ////If zooming on current axis is enabled.
                            if (chartAxis.EnableZooming)
                            {
                                ////Zoom out all axes in chart axes collection.
                                chartAxis.MulZoomCenter(chartArea.m_ZoomOutCoefficient);
                                chartArea.VisibileRangeForZoomAllAxis(chartArea, chartAxis);
                                if (chartAxis.Orientation == Orientation.Vertical)
                                {
                                    chartArea.VerticalUnPanZoomPosition = chartAxis.VisibleRange;
                                }
                                else
                                {
                                    chartArea.HorizontalUnPanZoomPosition = chartAxis.VisibleRange;
                                }
                            }
                        }
                    }
                    else
                    {
                        ////Otherwise zoom out only currently selected axes. 
                        if (chartArea.VerticalScrollingAxis.EnableZooming)
                        {
                            chartArea.VerticalScrollingAxis.MulZoomCenter(chartArea.m_ZoomOutCoefficient);
                            chartArea.VisibleRangeForZoomVerticalAxis(chartArea);
                        }

                        if (chartArea.HorizontalScrollingAxis.EnableZooming)
                        {
                            chartArea.HorizontalScrollingAxis.MulZoomCenter(chartArea.m_ZoomOutCoefficient);
                            chartArea.VisibleRangeForZoomHorizontalAxis(chartArea);
                        }
                    }
                }
            }
        }
        internal bool PanningFlag = false;
        internal bool ResetFlag = true;
        private static void OnZoomPaningCommand(object target, ExecutedRoutedEventArgs args)
        {
            ChartArea area = target as ChartArea;            
            if (area != null)
                area.OnChartPanning(new ChartPanningEventArgs(area));
            // area.IsPanning = true;
            SyncChartAreas syncarea = area.ChartAreaParent;
            if ((syncarea != null) || (area != null))
            {
                if (area.IsSync == true)
                {
                    //if ((area.ZoomAllAxes) && (area.ZoomedXRange.Delta != 0.0))
                    //{
                    //    syncarea.Cursor = Cursors.Hand;
                    //}
                    if (!(area.PanningFlag) && syncarea.ZoomedXRange.Delta != 0.0)
                    {
                        syncarea.Cursor = Cursors.Hand;
                        area.PanningFlag = true;
                        foreach (ChartArea _area in syncarea.Areas)
                        {
                            _area.PanningFlag = true;
                            _area.Cursor = Cursors.Hand;
                        }
                    }
                    else
                    {
                        syncarea.Cursor = Cursors.Arrow;
                        area.PanningFlag = false;
                        foreach (ChartArea _area in syncarea.Areas)
                        {
                            _area.PanningFlag = false;
                            _area.Cursor = Cursors.Arrow;
                        }

                    }
                }
                else
                {
                    if (!(area.PanningFlag) && area.ZoomedXRange.Delta != 0.0)
                    {
                        area.Cursor = Cursors.Hand;
                        area.PanningFlag = true;
                    }
                    else
                    {
                        area.Cursor = Cursors.Arrow;
                        area.PanningFlag = false;
                        //foreach (ChartAxis axis in area.Axes)
                        //{
                        //    if (axis.Orientation == Orientation.Vertical)
                        //    {
                        //        if (area.VerticalUnPanZoomPosition.Delta != 0 && !area.ResetFlag)
                        //            axis.ZoomRange(area.VerticalUnPanZoomPosition);
                        //    }
                        //    else
                        //    {
                        //        if (area.HorizontalUnPanZoomPosition.Delta != 0 && !area.ResetFlag)
                        //        axis.ZoomRange(area.HorizontalUnPanZoomPosition);
                        //    }
                        //}

                    }

                }
            }
        }


        private static void CanExecutechangeStyleCommand(Object sender, CanExecuteRoutedEventArgs args)
        {
            ChartArea area = sender as ChartArea;

            if (area != null)
            {
                args.CanExecute = true;// (area.AreaType == ChartAxesType.CartesianAxes);

            }
        }
        internal object stylename = null;
        internal bool StyleGridLine = false;


        internal ChartColorPalette defaultPalette = ChartColorPalette.Default;

        private static void OnchangeStyleCommand(object target, ExecutedRoutedEventArgs args)
        {
            ChartArea area = target as ChartArea;
            if (area != null)
            {
                if (area.ChartAreaParent != null)
                {
                    if ((area.ChartAreaParent as SyncChartAreas) != null)
                    {
                        Chart parent = area.ChartAreaParent.Parent as Chart;
                        foreach (ChartArea item in (area.ChartAreaParent as SyncChartAreas).Areas)
                        {
                            if (item != null)
                            {
                                SetStyleToChartArea(args, item, parent);
                            }
                        }
                    }
                }
                else
                {

                    SetStyleToChartArea(args, area, (Chart)area.Parent);
                }

            }
        }


        private static void SetStyleToChartArea(ExecutedRoutedEventArgs args, ChartArea area, Chart areaParent)
        {
            if (areaParent != null && areaParent is Chart)
            {
                if ((areaParent as Chart) != null)
                {
                    (areaParent as Chart).ChartVisualStyle = (ChartStyles)Enum.Parse(typeof(ChartStyles), args.Parameter.ToString());
                }
            }
        }




        /// <summary>
        /// Raised on ZoomResetCommand
        /// </summary>
        /// <param name="target">The object target</param>
        /// <param name="args">The ExecutedRoutedEventArgs args</param>
        private static void OnZoomResetCommand(object target, ExecutedRoutedEventArgs args)
        {
            ChartArea chartArea = target as ChartArea;
            chartArea.m_scrolling = false;
            SyncChartAreas syncChartArea = chartArea.ChartAreaParent;
            if (syncChartArea == null && chartArea is SyncChartAreas)
            {
                syncChartArea = chartArea as SyncChartAreas;
            }
            if ((syncChartArea != null) || (chartArea != null))
            {
                chartArea.PanningFlag = false;
                if (chartArea.IsSync == true)
                {
                    foreach (ChartArea _area in syncChartArea.Areas)
                    {
                        _area.Cursor = Cursors.Arrow;
                        _area.IsPanning = false;
                    }
                    syncChartArea.Cursor = Cursors.Arrow;
                    chartArea.IsPanning = false;
                    chartArea.VerticalUnPanZoomPosition = DoubleRange.Empty;
                    chartArea.VerticalUnPanZoomPosition = DoubleRange.Empty;
                }
                else
                {
                    chartArea.Cursor = Cursors.Arrow;
                    chartArea.IsPanning = false;
                    chartArea.VerticalUnPanZoomPosition = DoubleRange.Empty;
                    chartArea.VerticalUnPanZoomPosition = DoubleRange.Empty;
                }
            }


            if (syncChartArea != null)
            {
                if (syncChartArea.IsSyncChartArea == true)
                {
                    foreach (ChartArea chartArea_sync in syncChartArea.Areas)
                    {
                        ZoomResetChartArea(chartArea_sync, syncChartArea.IsSyncChartArea);
                    }

                }
            }
            else if (chartArea != null)
            {
                ZoomResetChartArea(chartArea, false);
            }

            chartArea.OnChartZoomReset(new ChartZoomReseteventArgs(chartArea));
            chartArea.ResetFlag = true;
        }
        private static void ZoomResetChartArea(ChartArea chartArea, bool isSyncChartArea)
        {
            if (chartArea != null)
            {
                if (isSyncChartArea == true)
                {
                    chartArea.HorizontalScrollingAxis.ZoomReset();
                    chartArea.VisibleRangeForZoomHorizontalAxis(chartArea);
                }
                else
                {
                    if (chartArea.ZoomAllAxes)
                    {
                        foreach (ChartAxis chartAxis in chartArea.Axes)
                        {
                            if (chartAxis.EnableZooming)
                            {
                                chartAxis.ZoomReset();
                                chartArea.VisibileRangeForZoomAllAxis(chartArea, chartAxis);
                            }
                        }
                    }
                    else
                    {
                        chartArea.VerticalScrollingAxis.ZoomReset();
                        chartArea.VisibleRangeForZoomVerticalAxis(chartArea);
                        chartArea.HorizontalScrollingAxis.ZoomReset();
                        chartArea.VisibleRangeForZoomHorizontalAxis(chartArea);
                    }
                }
            }
        }
        /// <summary>
        /// Zooms area sector regarding IsZooming property on series. IsZooming is ignored if ZoomAllAxes is true.
        /// </summary>
        /// <param name="target">The object target</param>
        /// <param name="args">The ExecutedRoutedEventArgs args</param>
        private static void OnZoomSectorCommand(object target, ExecutedRoutedEventArgs args)
        {
            ChartArea chartArea = target as ChartArea;
            chartArea.m_scrolling = false;
            SyncChartAreas syncChartArea = chartArea.ChartAreaParent;
            DoubleRange zoomrangevalue = new DoubleRange(0, 0);
            //if ((chartArea.Cursor != Cursors.Hand) && (chartArea.IsSync != true) && (chartArea.EnableRangeSelection == false) && chartArea.EnableMouseDragZooming)
            if ((chartArea.Cursor != Cursors.Hand) && (chartArea.EnableRangeSelection == false) && chartArea.EnableMouseDragZooming)
            {
                if (chartArea.IsSync != true)
                {

                    if (chartArea != null)
                    {
                        chartArea.PanningFlag = false;
                        if (args.Parameter is Rect)
                        {
                            Rect rect = (Rect)args.Parameter;
                            double start, end;
                            if (chartArea.ZoomAllAxes)
                            {
                                ////If we need to zoom all axes.
                                foreach (ChartAxis chartAxis in chartArea.Axes)
                                {
                                    ////Go thru all chart axes and zoom them regarding orientation.
                                    if (chartAxis.EnableZooming && chartAxis.ZoomFactor > chartAxis.MinimalZoomFactor)
                                    {
                                        if (chartAxis.Orientation == Orientation.Horizontal)
                                        {
                                            start = chartArea.PointToValue(chartAxis, rect.TopLeft);
                                            end = chartArea.PointToValue(chartAxis, rect.BottomRight);
                                            chartAxis.ZoomRange(new DoubleRange(start, end));

                                        zoomrangevalue = new DoubleRange(start, end); ;
                                        chartArea.ZoomedXRange = zoomrangevalue;
                                        chartArea.HorizontalUnPanZoomPosition = chartAxis.VisibleRange;
                                    }
                                    else
                                    {
                                        start = chartArea.PointToValue(chartAxis, rect.TopLeft);
                                        end = chartArea.PointToValue(chartAxis, rect.BottomRight);
                                        chartAxis.ZoomRange(new DoubleRange(start, end));

                                        zoomrangevalue = new DoubleRange(start, end);
                                        chartArea.ZoomedYRange = zoomrangevalue;
                                        chartArea.VerticalUnPanZoomPosition = chartAxis.VisibleRange;
                                    }
                                }
                            }
                        }
                        else
                        {
                            ////Zoom 1 axis only. Zooming axis is set via IsZooming property on series.
                            if (chartArea.HorizontalScrollingAxis.EnableZooming && chartArea.HorizontalScrollingAxis.ZoomFactor > chartArea.HorizontalScrollingAxis.MinimalZoomFactor)
                            {
                                start = chartArea.PointToValue(chartArea.HorizontalScrollingAxis, rect.TopLeft);
                                end = chartArea.PointToValue(chartArea.HorizontalScrollingAxis, rect.BottomRight);
                                chartArea.HorizontalScrollingAxis.ZoomRange(new DoubleRange(start, end));

                                zoomrangevalue = new DoubleRange(Math.Round(start, 2), Math.Round(end));
                                chartArea.ZoomedXRange = zoomrangevalue;
                            }

                            if (chartArea.VerticalScrollingAxis.EnableZooming && chartArea.HorizontalScrollingAxis.ZoomFactor > chartArea.HorizontalScrollingAxis.MinimalZoomFactor)
                            {
                                start = chartArea.PointToValue(chartArea.VerticalScrollingAxis, rect.TopLeft);
                                end = chartArea.PointToValue(chartArea.VerticalScrollingAxis, rect.BottomRight);
                                chartArea.VerticalScrollingAxis.ZoomRange(new DoubleRange(start, end));

                                zoomrangevalue = new DoubleRange(Math.Round(start, 2), Math.Round(end));
                                chartArea.ZoomedYRange = zoomrangevalue;
                            }
                        }
                    }
                }
            }
                else  //code for SyncChart Area
                {
                    foreach (ChartArea SyncArea in syncChartArea.Areas)
                    {
                        if (SyncArea != null)
                        {
                            SyncArea.PanningFlag = false;
                            if (args.Parameter is Rect)
                            {
                                Rect rect = (Rect)args.Parameter;
                                double start, end;

                                if (SyncArea.HorizontalScrollingAxis.EnableZooming && SyncArea.HorizontalScrollingAxis.ZoomFactor > SyncArea.HorizontalScrollingAxis.MinimalZoomFactor)
                                {
                                    start = SyncArea.PointToValue(SyncArea.HorizontalScrollingAxis, rect.TopLeft);
                                    end = SyncArea.PointToValue(SyncArea.HorizontalScrollingAxis, rect.BottomRight);
                                    SyncArea.HorizontalScrollingAxis.ZoomRange(new DoubleRange(start, end));
                                    SyncArea.ZoomedXRange = new DoubleRange(start, end);
                                    zoomrangevalue = new DoubleRange(Math.Round(start, 2), Math.Round(end));
                                    SyncArea.ZoomedXRange = zoomrangevalue;
                                }

                            }

                        }
                    }

                }
            }
            chartArea.OnChartZoomSector(new ChartZoomSectorEventArgs(chartArea,(Rect)args.Parameter));
            chartArea.ResetFlag = false;
        }

        /// <summary>
        /// Turns on zooming on area.
        /// </summary>
        /// <param name="target">The object target</param>
        /// <param name="args">The ExecutedRoutedEventArgs args</param>
        private static void OnSwitchZoomingCommand(object target, ExecutedRoutedEventArgs args)
        {
            ChartArea chartArea = target as ChartArea;
            ChartZoomingToolkit.SetZoomingToolkitVisibility(chartArea, Visibility.Visible);
            SyncChartAreas syncChartArea = target as SyncChartAreas;
            syncChartArea = chartArea.SyncChartArea != null ? chartArea.SyncChartArea : syncChartArea;
            int chartAreaCount;
            if (syncChartArea != null)
            {
                chartAreaCount = syncChartArea.Areas.Count;
                if (syncChartArea.IsSyncChartArea == true)
                {
                    ChartZoomingToolkit.SetZoomingToolkitVisibility(syncChartArea, Visibility.Visible);
                    foreach (ChartArea chartArea_sync in syncChartArea.Areas)
                    {
                        ChartZoomingToolkit.SetZoomingToolkitVisibility(chartArea_sync, Visibility.Visible);
                        SwitchZoomingChartArea(chartArea_sync, chartArea_sync.index, chartAreaCount);
                        foreach (ChartAxis chartAxis in chartArea_sync.Axes)
                            chartArea.VisibileRangeForZoomAllAxis(chartArea, chartAxis);
                    }
                }
            }
            else
            {
                SyncChartAreas syncChartArea1 = chartArea.ChartAreaParent;
                if (syncChartArea1 != null)
                {
                    chartAreaCount = syncChartArea1.Areas.Count;
                    if (syncChartArea1.IsSyncChartArea == true)
                    {
                        foreach (ChartArea chartArea_sync in syncChartArea1.Areas)
                        {
                            ChartZoomingToolkit.SetZoomingToolkitVisibility(chartArea_sync, Visibility.Visible);
                            SwitchZoomingChartArea(chartArea_sync, chartArea_sync.index, chartAreaCount);
                            foreach (ChartAxis chartAxis in chartArea_sync.Axes)
                                chartArea.VisibileRangeForZoomAllAxis(chartArea, chartAxis);
                        }
                    }
                }
                else if (chartArea != null)
                {
                    SwitchZoomingChartArea(chartArea, chartArea.index, 0);
                    foreach (ChartAxis chartAxis in chartArea.Axes)
                        chartArea.VisibileRangeForZoomAllAxis(chartArea, chartAxis);
                }
            }
        }
        private static void SwitchZoomingChartArea(ChartArea chartArea, int chartAreaIndex, int chartAreaCount)
        {
            foreach (ChartAxis chartAxis in chartArea.Axes)
                chartArea.VisibileRangeForZoomAllAxis(chartArea, chartAxis);
            if (chartArea != null)
            {
                bool hasAdorner = false;
                if (chartArea.m_areaPresenter == null)
                    return;
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(chartArea.m_areaPresenter);
                if (adornerLayer != null)
                {
                    Adorner[] adorners = adornerLayer.GetAdorners(chartArea.m_areaPresenter);
                    chartArea.ChartAreaCount = chartAreaCount;

                    if (adorners != null)
                    {
                        foreach (Adorner adorner in adorners)
                        {
                            ////Looking for Zooming adorner.
                            if (adorner is ChartZoomingAdorner)
                            {
                                hasAdorner = true;
                            }
                        }
                    }
                    SyncChartAreas syncChartArea = chartArea.ChartAreaParent;
                    if (!hasAdorner)
                    {
                        if (syncChartArea != null)
                        {
                            if ((Visibility)chartArea.GetValue(ChartZoomingToolkit.ZoomingToolkitVisibilityProperty) == Visibility.Visible)
                            {
                                if (chartArea.index == 0)
                                {
                                    adornerLayer.Add(new ChartZoomingAdorner(syncChartArea.Areas[0], syncChartArea.Areas[0].m_areaPresenter));
                                    chartArea.SetValue(ChartArea.ZoomSwitchedProperty, true);
                                }
                            }
                        }
                        else
                            adornerLayer.Add(new ChartZoomingAdorner(chartArea, chartArea.m_areaPresenter));
                        chartArea.SetValue(ChartArea.ZoomSwitchedProperty, true);
                    }
                    if (syncChartArea != null)
                    {
                       
                        foreach (ChartArea area in syncChartArea.Areas)
                        {
                            area.ZoomAllAxes = true;
                        }
                    }
                }

                foreach (ChartSeries chartSeries in chartArea.Series)
                {
                    if (chartSeries.IsZoomable)
                    {
                        chartArea.HorizontalScrollingAxis =
                          (chartSeries.XAxis.Orientation == Orientation.Horizontal) ? chartSeries.XAxis : chartSeries.YAxis;
                        chartArea.VerticalScrollingAxis =
                          (chartSeries.YAxis.Orientation == Orientation.Vertical) ? chartSeries.YAxis : chartSeries.XAxis;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Cacnels currently running switch zooming command.
        /// </summary>
        /// <param name="target">The object target</param>
        /// <param name="args">The ExecutedRoutedEventArgs args</param>
        private static void OnCancelZoomingCommand(object target, ExecutedRoutedEventArgs args)
        {
            ChartArea area = target as ChartArea;
            bool isSync = false;
            if (area != null)
            {
                if (area is ChartArea)
                {
                    if (area.ChartAreaParent != null)
                    {
                        isSync = true;
                    }
                }
            }
            if (target is SyncChartAreas || isSync == true)
            {
                SyncChartAreas syncChartArea = null;

                if (!isSync)
                {
                    syncChartArea = target as SyncChartAreas;
                }
                else
                {
                    syncChartArea = area.ChartAreaParent as SyncChartAreas;
                }
                if (syncChartArea != null)
                {
                    if (syncChartArea.IsSyncChartArea == true)
                    {
                        foreach (ChartArea chartArea_sync in syncChartArea.Areas)
                        {
                            chartArea_sync.PanningFlag = false;
                            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(chartArea_sync.m_areaPresenter);
                            Adorner[] adorners = adornerLayer.GetAdorners(chartArea_sync.m_areaPresenter);

                            if (adorners != null)
                            {
                                foreach (Adorner adorner in adorners)
                                {
                                    ////Searches for all adorners till find zooming adorner.
                                    if (adorner is ChartZoomingAdorner)
                                    {
                                        ////Removes and disposes adorner.
                                        adornerLayer.Remove(adorner);
                                        (adorner as IDisposable).Dispose();
                                        chartArea_sync.SetValue(ChartArea.ZoomSwitchedProperty, false);

                                        chartArea_sync.Cursor = null;
                                    }
                                }
                            }

                        }
                        syncChartArea.Cursor = null;

                    }

                }

            }
            else
            {
                ChartArea chartArea = target as ChartArea;

                SyncChartAreas syncChartArea = chartArea.ChartAreaParent;
                if (chartArea.m_areaPresenter != null)
                {
                    AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(chartArea.m_areaPresenter);
                    Adorner[] adorners = adornerLayer.GetAdorners(chartArea.m_areaPresenter);

                    if (adorners != null)
                    {
                        foreach (Adorner adorner in adorners)
                        {
                            ////Searches for all adorners till find zooming adorner.
                            if (adorner is ChartZoomingAdorner)
                            {
                                ////Removes and disposes adorner.
                                adornerLayer.Remove(adorner);
                                (adorner as IDisposable).Dispose();
                                if (syncChartArea != null)
                                {
                                    if (syncChartArea.IsSyncChartArea == true)
                                    {
                                        foreach (ChartArea chartArea_sync in syncChartArea.Areas)
                                        {
                                            chartArea_sync.PanningFlag = false;
                                            chartArea_sync.SetValue(ChartArea.ZoomSwitchedProperty, false);
                                        }

                                    }

                                }
                                else if (chartArea != null)
                                {
                                    chartArea.PanningFlag = false;
                                    chartArea.SetValue(ChartArea.ZoomSwitchedProperty, false);
                                    chartArea.SetValue(ChartZoomingToolkit.ZoomingToolkitVisibilityProperty, Visibility.Collapsed);
                                    chartArea.Cursor = null;
                                }
                                ////Sets zooming all on parent area to true.
                                ////chartArea.ZoomAllAxes = true;//Per Venugopal's request.
                                ////Goes thru all area's series.
                                ////foreach (ChartSeries chartSeries in chartArea.Series)//Per Venugopal's request.
                                ////{//Per Venugopal's request.
                                ////  //Calls coerce value to get back normal opacity on every series.
                                ////  chartSeries.CoerceValue(ChartSeries.InteriorProperty);//Per Venugopal's request.
                                ////}
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Raised on ChangePaletteCommand
        /// </summary>
        /// <param name="target">The object target</param>
        /// <param name="args">The ExecutedRoutedEventArgs args</param>
        private static void OnChangePaletteCommand(object target, ExecutedRoutedEventArgs args)
        {
            ChartArea chartArea = target as ChartArea;


            if (chartArea != null)
            {
                if (chartArea.ChartAreaParent != null)
                {
                    if ((chartArea.ChartAreaParent as SyncChartAreas) != null)
                    {
                        foreach (ChartArea item in (chartArea.ChartAreaParent as SyncChartAreas).Areas)
                        {
                            if (item != null)
                            {
                                item.ColorModel.Palette = (ChartColorPalette)args.Parameter;
                                item.defaultPalette = item.ColorModel.Palette;
                            }
                        }
                    }
                }
                else
                {

                    chartArea.ColorModel.Palette = (ChartColorPalette)args.Parameter;
                    chartArea.defaultPalette = chartArea.ColorModel.Palette;
                }
            }
        }

        /// <summary>
        /// Determines whether this instance can execute zooming commands the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private static void CanExecuteZoomingCommands(Object sender, CanExecuteRoutedEventArgs args)
        {
            ChartArea area = sender as ChartArea;

            if (area != null && area.PrimaryAxis != null && area.SecondaryAxis != null)
            {
                args.CanExecute = (area.AreaType == ChartAxesType.CartesianAxes)
                  && (area.PrimaryAxis.EnableZooming || area.SecondaryAxis.EnableZooming);
            }
        }

        private void DisposeSeries(ChartSeries series)
        {
            if (series == null)
                return;
            foreach (ChartSegment item in series.Segments)
            {
                item.seriesCorrespondingPoints = null;
            }

            series.Data = null;
            series.DataSource = null;
            if (series.Adornments != null)
            {
                series.Adornments.Clear();
            }

            if (series.Segments != null)
            {
                series.Segments.Clear();
                //series.Segments = null;
            }
        }

        /// <summary>
        /// Raised on AxesTypeChanged
        /// </summary>
        /// <param name="d">The DependencyObject d</param>
        /// <param name="args">The DependencyPropertyChangedEventArgs args</param>
        private static void OnAxesTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartArea area = d as ChartArea;

            if (area != null && area.m_areaPresenter != null)
            {
                if (!object.Equals(args.NewValue, ChartAxesType.CartesianAxes))
                {
                    foreach (ChartAxis axis in area.Axes)
                    {
                        axis.ZoomReset();
                    }

                    AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(area.m_areaPresenter);
                    Adorner[] adorners = adornerLayer.GetAdorners(area.m_areaPresenter);

                    if (adorners != null)
                    {
                        foreach (Adorner adorner in adorners)
                        {
                            if (adorner is ChartZoomingAdorner)
                            {
                                adornerLayer.Remove(adorner);
                                (adorner as IDisposable).Dispose();
                            }
                        }
                    }
                }
                 else
                {
                   //Sets the Adorner to null to initilaize the Annotations adorner layer on switching back to cartestian axes type
                    area.adorner = null;                
                }
                area.m_areaPresenter.InvalidateTemplate();
            }
        }

        /// <summary>
        /// Called when [chart3D settings changed].
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnChart3DSettingsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartArea area = d as ChartArea;

            if (area != null)
            {
                Chart3D settings = args.NewValue as Chart3D;

                if (settings != null)
                {
                    settings.Parent = area;
                    ///////////////////////////////////////////////////////////////////////////
                    if (settings.CameraProjection == CameraProjection.Perspective)
                    {
                        PerspectiveCamera myPCamera = new PerspectiveCamera();
                        myPCamera.FieldOfView = 90;
                        area.CameraController = new ChartTargetCameraController(myPCamera);
                        area.CameraController.Length = 1;
                        area.CameraController.Target = new Vector3D(0, -0.05, 0);
                        area.Camera3D = myPCamera;
                    }
                    else if (settings.CameraProjection == CameraProjection.Orthographic)
                    {
                        OrthographicCamera myPCamera = new OrthographicCamera();
                        myPCamera.Width = 2.3;
                        area.CameraController = new ChartTargetCameraController(myPCamera);
                        area.CameraController.Target = new Vector3D(0, -0.05, 0);

                        area.Camera3D = myPCamera;
                    }

                    SetCameraBindings(area.CameraController, settings);
                    /////////////////////////////////////////////////////////////////
                    area.UpdateArea();
                }
            }
        }

        /// <summary>
        /// Sets the camera bindings.
        /// </summary>
        /// <param name="nativeCameraController">The native camera controller.</param>
        /// <param name="viewSettings">The view settings.</param>
        private static void SetCameraBindings(ChartTargetCameraController nativeCameraController, Chart3D viewSettings)
        {
            Binding bindingProvider = new Binding();
            ////Tilt binding
            bindingProvider.Path = new PropertyPath(Chart3D.ViewDefaultTiltProperty);
            bindingProvider.Source = viewSettings;
            bindingProvider.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(nativeCameraController, ChartTargetCameraController.TiltProperty, bindingProvider);

            ////Turn binding
            bindingProvider = new Binding();
            bindingProvider.Path = new PropertyPath(Chart3D.ViewDefaultTurnProperty);
            bindingProvider.Source = viewSettings;
            bindingProvider.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(nativeCameraController, ChartTargetCameraController.TurnProperty, bindingProvider);

            ////Rotate binding
            bindingProvider = new Binding();
            bindingProvider.Path = new PropertyPath(Chart3D.ViewDefaultRotateProperty);
            bindingProvider.Source = viewSettings;
            bindingProvider.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(nativeCameraController, ChartTargetCameraController.RotateProperty, bindingProvider);
        }



        private static void OnSplitterVisibiltyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {

            SyncChartAreas syncArea = d as SyncChartAreas;
            if (syncArea != null)
            {

                foreach (ChartArea area in syncArea.Areas)
                {
                    if (area.m_areaPresenter != null)
                    {
                        if (area.m_areaPresenter.m_splitter != null && area.m_areaPresenter.m_splitter.Children[0] != null)
                        {
                            if (area.index != syncArea.Areas.Count - 1)
                            {
                                switch (syncArea.SplitterVisiblity)
                                {
                                    case SpliterVisibility.Hide:
                                        area.m_areaPresenter.m_splitter.Children[0].Visibility = Visibility.Collapsed;
                                        break;
                                    case SpliterVisibility.ShowAlways:
                                        area.m_areaPresenter.m_splitter.Children[0].Visibility = Visibility.Visible;
                                        area.m_areaPresenter.m_splitter.Children[0].Opacity = 1;
                                        break;
                                    case SpliterVisibility.ShowOnMouseHover:
                                        area.m_areaPresenter.m_splitter.Children[0].Visibility = Visibility.Visible;
                                        area.m_areaPresenter.m_splitter.Children[0].Opacity = 0;
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Called when [primary axis changed].
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnPrimaryAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartArea area = d as ChartArea;

            if (area != null)
            {
                ChartAxis newAxis = args.NewValue as ChartAxis;
                ChartAxis oldAxis = args.OldValue as ChartAxis;
                if (area != null && area.m_axes != null)
                {
                    area.m_axes.Remove(oldAxis);
                    if (newAxis != null)
                     {
                         Binding primaryStyle = new Binding();
                         primaryStyle.Source = area;
                         primaryStyle.Path = new PropertyPath(ChartArea.PrimaryAxisStyleProperty);
                         primaryStyle.Mode = BindingMode.TwoWay;
                         BindingOperations.SetBinding(newAxis, ChartAxis.StyleProperty, primaryStyle);
                         area.m_axes.Add(newAxis);
                         newAxis.isNeedUpdate = true;
                         if (oldAxis != null)
                         {
                            newAxis.Orientation = oldAxis.Orientation;
                            if (newAxis.Orientation == Orientation.Horizontal)
                            {
                                area.HorizontalScrollingAxis = newAxis;
                            }
                            else
                            {
                                area.VerticalScrollingAxis = newAxis;
                            }
                        }
                        else
                        {
                            newAxis.Orientation = Orientation.Horizontal;
                            area.HorizontalScrollingAxis = newAxis;
                        }
                    }

                    area.UpdateArea();
                }
            }
        }

        /// <summary>
        /// Called when [secondary axis changed].
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSecondaryAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartArea area = d as ChartArea;

            if (area.m_axes != null)
            {
                area.m_axes.Remove(args.OldValue as ChartAxis);
            }

            if (area != null && area.m_axes != null)
            {
                ChartAxis newAxis = args.NewValue as ChartAxis;
                ChartAxis oldAxis = args.OldValue as ChartAxis;

                area.m_axes.Remove(oldAxis);

                if (newAxis != null)
                 {
                    Binding SecondaryStyle = new Binding();
                    SecondaryStyle.Source = area;
                    SecondaryStyle.Path = new PropertyPath(ChartArea.SecondaryAxisStyleProperty);
                    SecondaryStyle.Mode = BindingMode.TwoWay;
                    BindingOperations.SetBinding(newAxis, ChartAxis.StyleProperty, SecondaryStyle);
                    area.m_axes.Add(newAxis);
                    newAxis.isNeedUpdate = true;
                    if (oldAxis != null)
                    {
                        newAxis.Orientation = oldAxis.Orientation;
                        if (newAxis.Orientation == Orientation.Horizontal)
                        {
                            area.HorizontalScrollingAxis = newAxis;
                        }
                        else
                        {
                            area.VerticalScrollingAxis = newAxis;
                        }
                    }
                    else
                    {
                        newAxis.Orientation = Orientation.Vertical;
                        area.VerticalScrollingAxis = newAxis;
                    }
                }

                area.UpdateArea();
            }
        }
        
             /// <summary>
        /// Called when [depth axis changed].
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDepthAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartArea area = d as ChartArea;

            if (area.m_axes != null)
            {
                area.m_axes.Remove(args.OldValue as ChartAxis);
            }

            if (area != null && area.m_axes != null)
            {
                ChartAxis newAxis = args.NewValue as ChartAxis;
                ChartAxis oldAxis = args.OldValue as ChartAxis;

                area.m_axes.Remove(oldAxis);

                if (newAxis != null)
                {
                    area.m_axes.Add(newAxis);
                    if (oldAxis != null)
                    {
                        newAxis.Orientation = oldAxis.Orientation;
                        if (newAxis.Orientation == Orientation.Horizontal)
                        {
                            area.HorizontalScrollingAxis = newAxis;
                        }
                        else
                        {
                            area.VerticalScrollingAxis = newAxis;
                        }
                    }
                    else
                    {
                        newAxis.Orientation = Orientation.Vertical;
                        area.VerticalScrollingAxis = newAxis;
                    }
                }

                //area.UpdateArea();
            }
        }
        /// <summary>
        /// Called when [scrolling axis changed].
        /// </summary>
        /// <param name="dpObj">The DependencyObject dpobj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnScrollingAxisChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs args)
        {
            ChartArea area = dpObj as ChartArea;
            if (area != null)
            {
                ////Get old axis value.
                ChartAxis chartAxis = args.OldValue as ChartAxis;
                ////If axis has horizontal position.
                if (args.Property == ChartArea.VerticalScrollingAxisProperty)
                {
                    if (chartAxis != null)
                    {
                        ////Unsubscribe from Changed event.
                        chartAxis.Changed -= new EventHandler(area.OnVerticalScrollingAxisChanged);

                    }

                    chartAxis = args.NewValue as ChartAxis;
                    ////If new value is not null.
                    if (chartAxis != null)
                    {
                        ////Subscribe for new axis' Changed event.
                        chartAxis.Changed += new EventHandler(area.OnVerticalScrollingAxisChanged);
                        
                    }
                }
                else
                {
                    ////If axis has vertical position.
                    if (chartAxis != null)
                    {
                        ////Unsubscribe from Changed event.
                        chartAxis.Changed -= new EventHandler(area.OnHorizontalScrollingAxisChanged);
                    }

                    chartAxis = args.NewValue as ChartAxis;
                    ////If new value is not null.
                    if (chartAxis != null)
                    {
                        ////Subscribe for new axis' Changed event.
                        chartAxis.Changed += new EventHandler(area.OnHorizontalScrollingAxisChanged);
                    }
                }
            }
        }

        /// <summary>
        /// Called when [zoom all axes changed].
        /// </summary>
        /// <param name="dpObj">The dp obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnZoomAllAxesChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs args)
        {
            ChartArea area = dpObj as ChartArea;
            SyncChartAreas syncChartArea = area.ChartAreaParent;
            if (syncChartArea != null)
            {
                if (syncChartArea.IsSyncChartArea == true)
                {
                    foreach (ChartArea chartArea in syncChartArea.Areas)
                    {
                        chartArea.ZoomAllAxes = syncChartArea.Areas[0].ZoomAllAxes;
                        syncChartArea.Areas[0].ZoomAllAxes = area.ZoomAllAxes;
                    }
                    foreach (ChartSeries chartSeries in area.Series)
                    {
                        chartSeries.CoerceValue(ChartSeries.InteriorProperty);
                    }
                }
            }
            else
            {
                if (area != null)
                {
                    ////Go thru all series and coercing their Interior property to update opatity.
                    foreach (ChartSeries chartSeries in area.Series)
                    {
                        chartSeries.CoerceValue(ChartSeries.InteriorProperty);
                    }
                    ////If ZoomAllAxes is changed to TRUE
                    if ((bool)args.NewValue)
                    {
                        ////If all series have their IsZooming to FALSE
                        foreach (ChartAxis chartAxis in area.Axes)
                        {
                            if (!chartAxis.EnableZooming)
                            {
                                chartAxis.EnableZooming = true;
                            }
                        }
                        ////Set ZoomAllAxes to true even if it was false.
                        //area.ZoomAllAxes = true;
                    }
                    else
                    {
                        ////If all series have their IsZooming to FALSE
                        foreach (ChartSeries chartSeries in area.Series)
                        {
                            if (chartSeries.IsZoomable)
                            {
                                return;
                            }
                        }
                        ////Set ZoomAllAxes to true even if it was false.
                      // area.ZoomAllAxes = true;
                    }
                }
            }
        }


        /// <summary>
        /// Called when SplitterPosition property changed.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        public static void OnSplitterPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartArea area = d as ChartArea;
            //if (area.SplitterPosition < 0.15)
            //    area.SplitterPosition = 0.15;
            //else if (area.SplitterPosition > 0.833)
            //    area.SplitterPosition = 0.833;


            Binding splitterPositionbinding = new Binding();
            splitterPositionbinding.Path = new PropertyPath(ChartArea.SplitterPositionProperty);
            splitterPositionbinding.Source = area;
            splitterPositionbinding.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(area, SplitRatioProperty, splitterPositionbinding);

            SyncChartAreas sArea = area.SyncChartArea as SyncChartAreas;
            if (sArea == null)
            {
                area.SplitpositionFlag = true;
                return;
            }
            DependencyObject obj = VisualTreeHelper.GetParent(area);
            if (obj is SyncAreasPanel)
            {
                foreach (ChartArea area1 in sArea.Areas)
                {
                    if (area.index != 0 && sArea.Areas[0].SplitterPosition != area.SplitterPosition)
                        (obj as SyncAreasPanel).SplitterPositionFlag = true;
                    else if (area.index == 0)
                        (obj as SyncAreasPanel).SplitterPositionFlag = false;
                }
                (obj as SyncAreasPanel).InvalidateMeasure();
            }

        }
        internal bool SplitpositionFlag = false;
        /// <summary>
        /// Called when EndValue property changed.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        public static void OnEndRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartArea area = d as ChartArea;

            if (area != null)
            {                
                foreach (ChartAxis axis in area.Axes)
                {
                    if (area.RangeSelectionOrientation == Orientation.Vertical && axis.Orientation == Orientation.Horizontal)
                    {
                        area.BeginInit();
                        SetValueForEndSelectionRange(area, axis);
                        //SetValueForStartSelectionRange(area);
                        area.EndInit();
                    }
                    else if(area.RangeSelectionOrientation == Orientation.Horizontal && axis.Orientation == Orientation.Vertical)
                    {
                        area.BeginInit();
                        SetValueForBottomSelectionRange(area, axis);
                        //SetValueForStartSelectionRange(area);
                        area.EndInit();
                    }
                }
            }
        }
        private static void SetValueForTopSelectionRange(ChartArea area, ChartAxis axis)
        {
            double range = area.ValueToPoint(axis, area.StartValue); //4 is the element margine of the chart area
            if (area.isLoadRectangleAtFirstTime)
            {
                area._cursortop.OffsetY = 0;
            }
            else
            {
                area._cursortop.OffsetY = range;
            }
            if (VisualTreeHelper.GetChildrenCount(area._cursortop) > 0)
            {
                DependencyObject obj = VisualTreeHelper.GetChild(area._cursortop, 0);
                Rectangle rect = VisualTreeHelper.GetChild(obj, 3) as Rectangle;
                area._recttop = rect;
                area._recttop.Fill = area.SelectionStroke;
                area._recttop.Opacity = 0.25;
                area._recttop.Visibility = Visibility.Visible;
                Binding heightBinding = new Binding();
                heightBinding.Path = new PropertyPath(InteractiveCursor.ActualWidthProperty);
                heightBinding.Source = area._cursortop;
                BindingOperations.SetBinding(area._recttop, WidthProperty, heightBinding);
                if (area._cursortop.OffsetY >= 0 )
                {
                    area._recttop.Height = area._cursortop.OffsetY;
                }
                else
                {
                    area._recttop.Height = 0;
                }
                if (area._cursortop.chartarea._toppresenter != null)
                {
                    area._cursortop.HorizontalCursorStroke = area.LineStroke;
                    area._cursortop.chartarea._toppresenter.Margin = new Thickness((area._cursortop.chartarea.ActualWidth / 2 - (area._cursortop.chartarea._toppresenter.ActualWidth / 2)), area._cursortop.OffsetY / 2, 0, 0);
                }
            }
        }
        private bool isLoadRectangleAtFirstTime = true;

        private static void SetValueForEndSelectionRange(ChartArea area, ChartAxis axis)
        {
            double range = area.ValueToPoint(axis, area.EndValue) - (area.AxesThickness.Left + 4);

            double endvalue = area.ValueToPoint(axis, axis.VisibleRange.End) - (area.AxesThickness.Left + 4);
            area._cursor2.OffsetX = range;
            if (VisualTreeHelper.GetChildrenCount(area._cursor2) > 0)
            {
                DependencyObject obj = VisualTreeHelper.GetChild(area._cursor2, 0);
                Rectangle rect = VisualTreeHelper.GetChild(obj, 4) as Rectangle;
                area._rect2 = rect;
                area._rect2.Fill = area.SelectionStroke;
                area._rect2.Opacity = 0.25;
                area._rect2.Visibility = Visibility.Visible;
            }
            Binding heightBinding = new Binding();
            heightBinding.Path = new PropertyPath(InteractiveCursor.ActualHeightProperty);
            heightBinding.Source = area._cursor2;
            BindingOperations.SetBinding(area._rect2, HeightProperty, heightBinding);
            if (VisualTreeHelper.GetChildrenCount(area._cursor2) > 0)
            {
                DependencyObject obj1 = VisualTreeHelper.GetChild(area._cursor2, 0);
                Line line = Chart.FindInVisualTreeDown(VisualTreeHelper.GetChild(obj1, 0), typeof(Line)) as Line;
                Line line2 = Chart.FindInVisualTreeDown(VisualTreeHelper.GetChild(obj1, 1), typeof(Line)) as Line;

                if (range >= 0 )
                {
                    area._rect2.Width = (range >= endvalue ? (range - endvalue) : endvalue - range);

                }
                else
                {
                    area._rect2.Width = 0;

                }
                Canvas.SetLeft(area._rect2, line.X2);
                if (area._cursor2.chartarea._bottompresenter != null)
                {
                    area._cursor2.VerticalCursorStroke = area.LineStroke;
                    area._cursor2.chartarea._bottompresenter.Margin = new Thickness(area._cursor2.OffsetX / 2, (area._cursor2.chartarea.ActualHeight / 2) - (area._cursor2.chartarea._bottompresenter.ActualHeight / 2), 0, 0);
                    Canvas.SetLeft(area._cursor2.chartarea._bottompresenter, area._cursor2.OffsetX / 2);
                }
            }
        }
        /// <summary>
        /// Called when LineStroke property changed.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        public static void OnLineStrokeChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartArea area = d as ChartArea;
            if (area != null)
            {
                if (area.EnableRangeSelection == true)
                {
                    foreach (ChartAxis axis in area.Axes)
                    {
                        if (area.RangeSelectionOrientation == Orientation.Vertical && axis.Orientation == Orientation.Horizontal)
                        {
                            area.BeginInit();
                            SetValueForEndSelectionRange(area, area.PrimaryAxis);
                            SetValueForStartSelectionRange(area, area.PrimaryAxis);
                            area.EndInit();
                        }
                        else if (area.RangeSelectionOrientation == Orientation.Horizontal && axis.Orientation == Orientation.Vertical)
                        {
                            area.BeginInit();
                            SetValueForBottomSelectionRange(area, area.SecondaryAxis);
                            SetValueForTopSelectionRange(area, area.SecondaryAxis);
                            area.EndInit();
                        }

                    }
                }
            }
        }
        /// <summary>
        /// Called when SelectionStroke property changed.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        public static void OnSelectionStrokeChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartArea area = d as ChartArea;
            if (area != null)
            {
                if (area.EnableRangeSelection == true)
                {
                    foreach (ChartAxis axis in area.Axes)
                    {
                        if (area.RangeSelectionOrientation == Orientation.Vertical && axis.Orientation == Orientation.Horizontal)
                        {
                            area.BeginInit();
                            SetValueForEndSelectionRange(area, area.PrimaryAxis);
                            SetValueForStartSelectionRange(area, area.PrimaryAxis);
                            area.EndInit();
                        }
                        else if (area.RangeSelectionOrientation == Orientation.Horizontal && axis.Orientation == Orientation.Vertical)
                        {
                            area.BeginInit();
                            SetValueForBottomSelectionRange(area, area.SecondaryAxis);
                            SetValueForTopSelectionRange(area, area.SecondaryAxis);
                            area.EndInit();
                        }

                    }
                }
            }
        }
        /// <summary>
        /// Called when StartValue property changed.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        public static void OnStartRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartArea area = d as ChartArea;

            if (area != null)
            {
                foreach (ChartAxis axis in area.Axes)
                {
                    if (axis.Orientation == Orientation.Horizontal && area.RangeSelectionOrientation == Orientation.Vertical)
                    {
                        area.BeginInit();
                        SetValueForStartSelectionRange(area, axis);
                        //SetValueForEndSelectionRange(area);SetValueForEndSelectionRangeForTopCursor
                        area.EndInit();
                    }
                    else if (axis.Orientation == Orientation.Vertical && area.RangeSelectionOrientation == Orientation.Horizontal)
                    {
                        area.BeginInit();
                        SetValueForTopSelectionRange(area, axis);
                        //SetValueForEndSelectionRange(area);SetValueForEndSelectionRangeForTopCursor
                        area.EndInit();
                    }
                }
            }
        }
        private static void SetValueForBottomSelectionRange(ChartArea area, ChartAxis axis)
        {
            double range = area.ValueToPoint(axis, area.EndValue);

            double endvalue = area.ValueToPoint(axis, axis.VisibleRange.Start);
            area._cursorbottom.OffsetY = range;
            if (VisualTreeHelper.GetChildrenCount(area._cursorbottom) > 0)
            {
                DependencyObject obj = VisualTreeHelper.GetChild(area._cursorbottom, 0);
                Rectangle rect = VisualTreeHelper.GetChild(obj, 4) as Rectangle;
                area._rectbottom = rect;
                area._rectbottom.Fill = area.SelectionStroke;
                area._rectbottom.Opacity = 0.25;
                area._rectbottom.Visibility = Visibility.Visible;
                Binding heightBinding = new Binding();
                heightBinding.Path = new PropertyPath(InteractiveCursor.ActualWidthProperty);
                heightBinding.Source = area._cursorbottom;
                BindingOperations.SetBinding(area._rectbottom, WidthProperty, heightBinding);
                DependencyObject obj1 = VisualTreeHelper.GetChild(area._cursorbottom, 0);
                Line line = Chart.FindInVisualTreeDown(VisualTreeHelper.GetChild(obj1, 0), typeof(Line)) as Line;
                Line line2 = Chart.FindInVisualTreeDown(VisualTreeHelper.GetChild(obj1, 1), typeof(Line)) as Line;

                if (range >= 0 && (endvalue - range) > 0)
                {
                    area._rectbottom.Height = endvalue - range;
                }
                else
                {
                    area._rectbottom.Height = 0;

                }
                Canvas.SetTop(area._rectbottom, line.Y2);
                if (area._cursorbottom.chartarea._bottompresenter != null)
                {
                    area._cursorbottom.HorizontalCursorStroke = area.LineStroke;
                    area._cursorbottom.chartarea._bottompresenter.Margin = new Thickness(((area._cursorbottom.ActualWidth / 2) - (area._cursorbottom.chartarea._bottompresenter.ActualWidth / 2)), area._rectbottom.Height / 2, 0, 0);
                    Canvas.SetTop(area._bottompresenter, range);
                }
            }
        }
        private static void SetValueForStartSelectionRange(ChartArea area, ChartAxis axis)
        {
            double range = area.ValueToPoint(axis, area.StartValue) - (area.AxesThickness.Left + 4); //4 is the element margine of the chart area
            area._cursor1.OffsetX = range;
            if (VisualTreeHelper.GetChildrenCount(area._cursor1) > 0)
            {
                DependencyObject obj = VisualTreeHelper.GetChild(area._cursor1, 0);
                Rectangle rect = VisualTreeHelper.GetChild(obj, 3) as Rectangle;
                area._rect1 = rect;
                area._rect1.Fill = area.SelectionStroke;
                area._rect1.Opacity = 0.25;
                area._rect1.Visibility = Visibility.Visible;
                var heightBinding = new Binding
                    {
                        Path = new PropertyPath(FrameworkElement.ActualHeightProperty),
                        Source = area._cursor1
                    };
                BindingOperations.SetBinding(area._rect1, HeightProperty, heightBinding);
                area._rect1.Width = (area._cursor1.OffsetX >= 0&&!double.IsInfinity(area._cursor1.OffsetX)) ? area._cursor1.OffsetX : 0;
                if (area._cursor1.chartarea._toppresenter != null)
                {
                    area._cursor1.VerticalCursorStroke = area.LineStroke;
                    area._cursor1.chartarea._toppresenter.Margin = new Thickness((area._cursor1.OffsetX) - (area._cursor1.chartarea._toppresenter.ActualWidth), (area._cursor1.chartarea.ActualHeight / 2 - (area._cursor1.chartarea._toppresenter.ActualHeight / 2)), 0, 0);
                }
            }
        }

        private Rectangle _rect1 = new Rectangle();
        private double _endrange = 0d;
        private bool _rangeSelection = false;
        private bool _rangeSelection1 = false;
        private bool _rangeSelectiontop = false;
        private bool _rangeSelectionbottom = false;
        private InteractiveCursor _cursor1 = new InteractiveCursor();
        private InteractiveCursor _cursor2 = new InteractiveCursor();
        private InteractiveCursor _cursortop = new InteractiveCursor();
        private InteractiveCursor _cursorbottom = new InteractiveCursor();
        private Rectangle _recttop = new Rectangle();
        private Rectangle _rectbottom = new Rectangle();
        private Rectangle _rect2 = new Rectangle();
        private ContentPresenter _toppresenter = null;
        private ContentPresenter _bottompresenter = null;
        /// <summary>
        /// Called when EnableRangeSelection property changed.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        public static void OnEnabelSeriesInterativeChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartArea area = d as ChartArea;
            if (area != null) area.EnableSeriesInteractive(area);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        public static void OnvalueChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartArea area = d as ChartArea;
            if (area != null) area.EnableSeriesInteractive(area);
        }

        internal void EnableSeriesInteractive(ChartArea area)
        {
            foreach (ChartAxis chartaxis in area.Axes)
            {
                if (chartaxis.Orientation == Orientation.Horizontal)
                {
                    if (area.EnableRangeSelection == true && area.RangeSelectionOrientation == Orientation.Vertical)
                    {
                        //area.SetValue(ChartZoomingToolkit.ZoomingToolkitVisibilityProperty, Visibility.Collapsed);
                        area.EnableMouseDragZooming = false;
                        area.ResetOnRangeSelection(area);
                        area.CancelZoomingOnRangeSelection(area);
                        area.ZoomAllAxes = false;
                        area._cursor1.Name = "RangeSelectioncursor1";
                        area._cursor1.CursorVisibility = Visibility.Visible;
                        area._cursor1.CursorStrokeThickness = 3;
                        area._cursor1.HorizontalCursorStroke = Brushes.Transparent;
                        area._cursor1.Cursor = Cursors.SizeWE;
                        area._cursor1.OffsetX = 0;
                        area._cursor1.IsBindWithSegment = false;
                        area._cursor1.BindWithMouseMoveOnSegment = false;
                        area._cursor1.InteractiveCursorSymbolVisibility = Visibility.Collapsed;
                        area._cursor1.HorizontalLabelVisibility = Visibility.Collapsed;
                        area._cursor1.VerticalLabelVisibility = Visibility.Collapsed;
                        area._cursor1.LabelVisibility = Visibility.Collapsed;
                        foreach (ChartAxis axis in area.Axes)
                        {
                            axis.InteractiveCursorContentVisibility = false;
                        }
                        if (area._toppresenter == null)
                        {
                            area._toppresenter = new ContentPresenter();
                        }
                        area._cursor1.Loaded += new RoutedEventHandler(cursor1_Loaded);
                        area._cursor1.MouseLeftButtonDown += new MouseButtonEventHandler(cursor1_MouseLeftButtonDown);
                        area._cursor1.MouseMove += new MouseEventHandler(cursor1_MouseMove);
                        area._cursor1.MouseLeftButtonUp += new MouseButtonEventHandler(cursor1_MouseLeftButtonUp);
                        area._rect1.Fill = Brushes.Transparent;
                        area._rect1 = new Rectangle();

                        area._cursor2.Name = "RangeSelectioncursor2";
                        area._cursor2.CursorVisibility = Visibility.Visible;
                        area._cursor2.CursorStrokeThickness = 3;
                        area._cursor2.HorizontalCursorStroke = Brushes.Transparent;
                        area._cursor2.VerticalCursorStroke = area.LineStroke;
                        area._cursor2.Cursor = Cursors.SizeWE;
                        area._cursor2.IsBindWithSegment = false;
                        area._cursor2.BindWithMouseMoveOnSegment = false;
                        area._cursor2.HorizontalLabelVisibility = Visibility.Collapsed;
                        area._cursor2.VerticalLabelVisibility = Visibility.Collapsed;
                        area._cursor2.InteractiveCursorSymbolVisibility = Visibility.Collapsed;
                        area._cursor2.HorizontalLabelVisibility = Visibility.Collapsed;
                        area._cursor2.VerticalLabelVisibility = Visibility.Collapsed;
                        area._cursor2.LabelVisibility = Visibility.Collapsed;
                        foreach (ChartAxis axis in area.Axes)
                        {
                            axis.InteractiveCursorContentVisibility = false;
                        }
                        if (area._bottompresenter == null)
                        {
                            area._bottompresenter = new ContentPresenter();
                        }
                        area._cursor2.Loaded += new RoutedEventHandler(cursor2_Loaded);
                        area._cursor2.MouseLeftButtonDown += new MouseButtonEventHandler(cursor2_MouseLeftButtonDown);
                        area._cursor2.MouseMove += new MouseEventHandler(cursor2_MouseMove);
                        area._cursor2.MouseLeftButtonUp += new MouseButtonEventHandler(cursor2_MouseLeftButtonUp);
                        area._rect2.Fill = Brushes.Transparent;
                        area._rect2 = new Rectangle();

                        area.InteractiveCursors.Add(area._cursor1);
                        area.InteractiveCursors.Add(area._cursor2);
                        area._cursor1.HorizontalLabelVisibility = Visibility.Collapsed;
                        area._cursor1.VerticalLabelVisibility = Visibility.Collapsed;
                    }
                    else
                    {
                        area.InteractiveCursors.Remove(area._cursor1);
                        area.InteractiveCursors.Remove(area._cursor2);
                        //area.SetValue(ChartZoomingToolkit.ZoomingToolkitVisibilityProperty, Visibility.Visible);
                        area._rect1.Width = 0;
                        area._rect2.Width = 0;
                        if (_toppresenter != null && _bottompresenter != null)
                        {
                            area._toppresenter.Visibility = Visibility.Collapsed;
                            area._bottompresenter.Visibility = Visibility.Collapsed;
                        }
                        area._toppresenter = null;
                        area._bottompresenter = null;
                    }
                }
                else if (chartaxis.Orientation == Orientation.Vertical)
                {
                    if (area.EnableRangeSelection == true && area.RangeSelectionOrientation == Orientation.Horizontal)
                    {
                        //area.SetValue(ChartZoomingToolkit.ZoomingToolkitVisibilityProperty, Visibility.Collapsed);
                        area.EnableMouseDragZooming = false;
                        area.ResetOnRangeSelection(area);
                        area.CancelZoomingOnRangeSelection(area);
                        area.ZoomAllAxes = false;
                        area._cursortop.Name = "RangeSelectioncursorTop";
                        area._cursortop.CursorVisibility = Visibility.Visible;
                        area._cursortop.CursorStrokeThickness = 3;
                        area._cursortop.VerticalCursorStroke = Brushes.Transparent;
                        area._cursortop.Cursor = Cursors.SizeNS;
                        area._cursortop.OffsetY = 0;
                        area._cursortop.IsBindWithSegment = false;
                        area._cursortop.BindWithMouseMoveOnSegment = false;
                        area._cursortop.HorizontalLabelVisibility = Visibility.Collapsed;
                        area._cursortop.VerticalLabelVisibility = Visibility.Collapsed;
                        area._cursortop.InteractiveCursorSymbolVisibility = System.Windows.Visibility.Collapsed;
                        area._cursortop.LabelVisibility = Visibility.Collapsed;
                        foreach (ChartAxis axis in area.Axes)
                        {
                            axis.InteractiveCursorContentVisibility = false;
                        }
                        if (area._toppresenter == null)
                        {
                            area._toppresenter = new ContentPresenter();
                        }
                        area._cursortop.Loaded += new RoutedEventHandler(cursortop_Loaded);
                        area._cursortop.MouseLeftButtonDown += new MouseButtonEventHandler(cursortop_MouseLeftButtonDown);
                        area._cursortop.MouseMove += new MouseEventHandler(cursortop_MouseMove);
                        area._cursortop.MouseLeftButtonUp += new MouseButtonEventHandler(cursortop_MouseLeftButtonUp);
                        area._recttop.Fill = Brushes.Transparent;
                        area._recttop = new Rectangle();


                        area._cursorbottom.Name = "RangeSelectioncursorbottom";
                        area._cursorbottom.CursorVisibility = Visibility.Visible;
                        area._cursorbottom.CursorStrokeThickness = 3;
                        area._cursorbottom.VerticalCursorStroke = Brushes.Transparent;
                        area._cursorbottom.HorizontalCursorStroke = area.LineStroke;
                        area._cursorbottom.Cursor = Cursors.SizeNS;
                        area._cursorbottom.IsBindWithSegment = false;
                        area._cursorbottom.BindWithMouseMoveOnSegment = false;
                        area._cursorbottom.HorizontalLabelVisibility = Visibility.Collapsed;
                        area._cursorbottom.VerticalLabelVisibility = Visibility.Collapsed;
                        area._cursorbottom.InteractiveCursorSymbolVisibility = System.Windows.Visibility.Collapsed;
                        area._cursorbottom.LabelVisibility = Visibility.Collapsed;
                        foreach (ChartAxis axis in area.Axes)
                        {
                            axis.InteractiveCursorContentVisibility = false;
                        }
                        if (area._bottompresenter == null)
                        {
                            area._bottompresenter = new ContentPresenter();
                        }
                        area._cursorbottom.Loaded += new RoutedEventHandler(cursorbottom_Loaded);
                        area._cursorbottom.MouseLeftButtonDown += new MouseButtonEventHandler(cursorbottom_MouseLeftButtonDown);
                        area._cursorbottom.MouseMove += new MouseEventHandler(cursorbottom_MouseMove);
                        area._cursorbottom.MouseLeftButtonUp += new MouseButtonEventHandler(cursorbottom_MouseLeftButtonUp);
                        area._rectbottom.Fill = Brushes.Transparent;
                        area._rectbottom = new Rectangle();

                        area.InteractiveCursors.Add(area._cursortop);
                        area.InteractiveCursors.Add(area._cursorbottom);
                    }
                    else
                    {
                        area.InteractiveCursors.Remove(area._cursortop);
                        area.InteractiveCursors.Remove(area._cursorbottom);
                        //area.SetValue(ChartZoomingToolkit.ZoomingToolkitVisibilityProperty, Visibility.Visible);
                        area._rect1.Width = 0;
                        area._rect2.Width = 0;
                        if (_toppresenter != null && _bottompresenter != null)
                        {
                            area._toppresenter.Visibility = Visibility.Collapsed;
                            area._bottompresenter.Visibility = Visibility.Collapsed;
                        }
                        area._toppresenter = null;
                        area._bottompresenter = null;
                    }
                }
            }
        }

        static void cursorbottom_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            InteractiveCursor cursor1 = sender as InteractiveCursor;
            cursor1.InteractiveCursorSymbolVisibility = Visibility.Collapsed;
            cursor1.HorizontalLabelVisibility = Visibility.Collapsed;
            cursor1.VerticalLabelVisibility = Visibility.Collapsed;
            cursor1.LabelVisibility = Visibility.Collapsed;            
            cursor1.chartarea._rectbottom.Visibility = Visibility.Visible;            
            Point pt = e.GetPosition(cursor1.chartarea);
            if (pt.Y > 0 && e.GetPosition(cursor1.chartarea).Y < (cursor1.chartarea.ValueToPoint(cursor1.chartarea.SecondaryAxis, cursor1.chartarea.SecondaryAxis.VisibleRange.Start)) && pt.Y > (cursor1.chartarea.ValueToPoint(cursor1.chartarea.SecondaryAxis, cursor1.chartarea.SecondaryAxis.VisibleRange.End)))
            {
                double value = cursor1.chartarea.PointToValue(cursor1.chartarea.SecondaryAxis, pt);
                cursor1.chartarea.EndValue = value;
            }
            cursorbottom_MouseLeftButtonDown(cursor1, e);
            cursorbottom_MouseMove(cursor1, e);
            cursor1.ReleaseMouseCapture();
            cursor1.chartarea._rangeSelectionbottom = false;
        }

        static void cursorbottom_MouseMove(object sender, MouseEventArgs e)
        {
            InteractiveCursor cursor1 = sender as InteractiveCursor;
            if (cursor1.chartarea._rangeSelectionbottom == true)
            {
                cursor1.LabelVisibility = Visibility.Collapsed;
                Point p = e.GetPosition(cursor1);
                DependencyObject obj = VisualTreeHelper.GetChild(cursor1, 0);
                Line line = VisualTreeHelper.GetChild(obj, 0) as Line;
                Line line2 = VisualTreeHelper.GetChild(obj, 1) as Line;
                foreach (ChartAxis axis in cursor1.chartarea.Axes)
                {
                    axis.InteractiveCursorContentVisibility = false;
                }
                if (p.Y < cursor1.ActualHeight && (p.Y -1) > 0)
                {
                    cursor1.chartarea._rectbottom.Height = cursor1.chartarea.ValueToPoint(cursor1.chartarea.SecondaryAxis, cursor1.chartarea.SecondaryAxis.VisibleRange.Start) - p.Y;
                    cursor1.OffsetY = p.Y;
                    cursor1.chartarea._bottompresenter.Margin = new Thickness(((cursor1.chartarea.ActualWidth / 2)-(cursor1.chartarea._bottompresenter.ActualWidth/2)), cursor1.chartarea._rectbottom.Height / 2, 0, 0);
                    Canvas.SetTop(cursor1.chartarea._rectbottom, p.Y);
                    Canvas.SetTop(cursor1.chartarea._bottompresenter, p.Y);
                }
            }
        }

        static void cursorbottom_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            InteractiveCursor cursor2 = sender as InteractiveCursor;
            cursor2.InteractiveCursorSymbolVisibility = Visibility.Collapsed;
            cursor2.HorizontalLabelVisibility = Visibility.Collapsed;
            cursor2.VerticalLabelVisibility = Visibility.Collapsed;
            cursor2.LabelVisibility = Visibility.Collapsed;
            if (cursor2 != null)
            {
                cursor2.chartarea.ZoomAllAxes = false;
                DependencyObject obj = VisualTreeHelper.GetChild(cursor2, 0);                                
                cursor2.chartarea._rangeSelectionbottom = true;                
                Rectangle rect = VisualTreeHelper.GetChild(obj, 4) as Rectangle;
                cursor2.chartarea._rectbottom = rect;
                cursor2.chartarea._rectbottom.Fill = cursor2.chartarea.SelectionStroke;
                cursor2.chartarea._rectbottom.Opacity = 0.25;
                cursor2.chartarea._rectbottom.Visibility = Visibility.Visible;
                cursor2.chartarea._rectbottom.MouseEnter += new MouseEventHandler(rectbottom_MouseEnter);              
                ContentPresenter presenter = VisualTreeHelper.GetChild(obj, 6) as ContentPresenter;
                cursor2.chartarea._bottompresenter = presenter;
                cursor2.chartarea._bottompresenter.Visibility = Visibility.Visible;
                Binding lowlabelbin = new Binding();
                lowlabelbin.Source = cursor2.chartarea;
                lowlabelbin.Path = new PropertyPath(ChartArea.LowerRangeLabelProperty);
                lowlabelbin.Mode = BindingMode.TwoWay;
                BindingOperations.SetBinding(cursor2.chartarea._bottompresenter, ContentPresenter.ContentProperty, lowlabelbin);
                cursor2.chartarea._bottompresenter.HorizontalAlignment = HorizontalAlignment.Center;
                Binding heightBinding = new Binding();
                heightBinding.Path = new PropertyPath(InteractiveCursor.ActualWidthProperty);
                heightBinding.Source = cursor2;
                BindingOperations.SetBinding(cursor2.chartarea._rectbottom, WidthProperty, heightBinding);
                cursor2.CaptureMouse();
            }
        }

        static void rectbottom_MouseEnter(object sender, MouseEventArgs e)
        {
            Rectangle rec = sender as Rectangle;
            if (rec != null)
            {
                rec.Cursor = Cursors.Arrow;
            }
        }

        static void cursorbottom_Loaded(object sender, RoutedEventArgs e)
        {
            InteractiveCursor cursor2 = sender as InteractiveCursor;
            DependencyObject obj1 = VisualTreeHelper.GetChild(cursor2, 0);
            ContentPresenter presenter = VisualTreeHelper.GetChild(obj1, 6) as ContentPresenter;
            cursor2.chartarea._bottompresenter = presenter;     
            cursor2.InteractiveCursorSymbolVisibility = Visibility.Collapsed;
            cursor2.LabelVisibility = Visibility.Collapsed;
            cursor2.HorizontalLabelVisibility = Visibility.Collapsed;
            cursor2.VerticalLabelVisibility = Visibility.Collapsed;
            cursor2.LabelVisibility = Visibility.Collapsed;
            Binding lowlabelbin = new Binding();
            lowlabelbin.Source = cursor2.chartarea;
            lowlabelbin.Path = new PropertyPath(ChartArea.LowerRangeLabelProperty);
            lowlabelbin.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(cursor2.chartarea._bottompresenter, ContentPresenter.ContentProperty, lowlabelbin);
            cursor2.HorizontalCursorStroke = cursor2.chartarea.LineStroke;
            SetValueForBottomSelectionRange(cursor2.chartarea, cursor2.chartarea.SecondaryAxis); 
            DependencyObject obj = VisualTreeHelper.GetChild(cursor2, 0);
            Line line = Chart.FindInVisualTreeDown(VisualTreeHelper.GetChild(obj, 0), typeof(Line)) as Line;
            Line line2 = Chart.FindInVisualTreeDown(VisualTreeHelper.GetChild(obj, 1), typeof(Line)) as Line;
            cursor2.OffsetY = line.Y2;
          //  cursor2.chartarea.cursorOffset_x = cursor2.OffsetX;
        }

        static void cursortop_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            InteractiveCursor cursor1 = sender as InteractiveCursor;
            cursor1.InteractiveCursorSymbolVisibility = Visibility.Collapsed;
            cursor1.HorizontalLabelVisibility = Visibility.Collapsed;
            cursor1.VerticalLabelVisibility = Visibility.Collapsed;
            cursor1.LabelVisibility = Visibility.Collapsed;
            cursor1.chartarea._recttop.Visibility = Visibility.Visible;            
            Point pt = e.GetPosition(cursor1.chartarea);
            if (pt.Y > 0 && e.GetPosition(cursor1.chartarea).Y < (cursor1.chartarea.ValueToPoint(cursor1.chartarea.SecondaryAxis, cursor1.chartarea.SecondaryAxis.VisibleRange.Start)) && pt.Y > (cursor1.chartarea.ValueToPoint(cursor1.chartarea.SecondaryAxis, cursor1.chartarea.SecondaryAxis.VisibleRange.End)))
            {
                double value = cursor1.chartarea.PointToValue(cursor1.chartarea.SecondaryAxis, pt);
                cursor1.chartarea.StartValue = value;
            }
            cursortop_MouseLeftButtonDown(cursor1, e);
            cursortop_MouseMove(cursor1, e);
            cursor1.ReleaseMouseCapture();
            cursor1.chartarea._rangeSelectiontop = false;
        }

        static void cursortop_MouseMove(object sender, MouseEventArgs e)
        {
            InteractiveCursor cursor1 = sender as InteractiveCursor;
            cursor1.InteractiveCursorSymbolVisibility = Visibility.Collapsed;
            cursor1.HorizontalLabelVisibility = Visibility.Collapsed;
            cursor1.VerticalLabelVisibility = Visibility.Collapsed;
            cursor1.LabelVisibility = Visibility.Collapsed;
            if (cursor1.chartarea._rangeSelectiontop == true)
            {
                cursor1.LabelVisibility = Visibility.Collapsed;
                Point p = e.GetPosition(cursor1);
                DependencyObject obj = VisualTreeHelper.GetChild(cursor1, 0);
                Line line = Chart.FindInVisualTreeDown(VisualTreeHelper.GetChild(obj, 0), typeof(Line)) as Line;
                foreach (ChartAxis axis in cursor1.chartarea.Axes)
                {
                    axis.InteractiveCursorContentVisibility = false;
                }
                if (p.Y > 0 && e.GetPosition(cursor1.chartarea).Y < (cursor1.chartarea.ValueToPoint(cursor1.chartarea.SecondaryAxis, cursor1.chartarea.SecondaryAxis.VisibleRange.Start)) && p.Y > (cursor1.chartarea.ValueToPoint(cursor1.chartarea.SecondaryAxis, cursor1.chartarea.SecondaryAxis.VisibleRange.End)))
                {
                    cursor1.chartarea._toppresenter.Visibility = Visibility.Visible;
                    cursor1.chartarea._recttop.Height = p.Y;
                    cursor1.OffsetY = cursor1.chartarea._recttop.Height;
                    cursor1.chartarea._toppresenter.Margin = new Thickness((cursor1.chartarea.ActualWidth / 2 - (cursor1.chartarea._toppresenter.ActualWidth / 2)), cursor1.OffsetY / 2, 0, 0);
                    cursor1.chartarea.isLoadRectangleAtFirstTime = false;
                }
                else
                {
                    //cursor1.chartarea.toppresenter.Visibility = Visibility.Collapsed;
                    //cursor1.chartarea.recttop.Height = 1;
                }
            }
        }

        static void cursortop_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            InteractiveCursor cursor1 = sender as InteractiveCursor;
            cursor1.InteractiveCursorSymbolVisibility = Visibility.Collapsed;
            cursor1.HorizontalLabelVisibility = Visibility.Collapsed;
            cursor1.VerticalLabelVisibility = Visibility.Collapsed;
            cursor1.LabelVisibility = Visibility.Collapsed;
            cursor1.chartarea._rangeSelectiontop = true;
            cursor1.chartarea.ZoomAllAxes = false;
            DependencyObject obj = VisualTreeHelper.GetChild(cursor1, 0);
            Rectangle rect = VisualTreeHelper.GetChild(obj, 3) as Rectangle;
            ContentPresenter presenter = VisualTreeHelper.GetChild(obj, 5) as ContentPresenter;
            cursor1.chartarea._recttop = rect;
            cursor1.chartarea._recttop.Fill = cursor1.chartarea.SelectionStroke;
            cursor1.chartarea._recttop.Opacity = 0.25;
            cursor1.chartarea._recttop.Visibility = Visibility.Visible;
            cursor1.chartarea._recttop.MouseEnter += new MouseEventHandler(recttop_MouseEnter);
            cursor1.chartarea._toppresenter = presenter;
            cursor1.chartarea._toppresenter.Visibility = Visibility.Visible;
            Binding toplabelbin = new Binding();
            toplabelbin.Source = cursor1.chartarea;
            toplabelbin.Path = new PropertyPath(ChartArea.UpperRangeLabelProperty);
            toplabelbin.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(cursor1.chartarea._toppresenter, ContentPresenter.ContentProperty, toplabelbin);
            cursor1.chartarea._toppresenter.HorizontalAlignment = HorizontalAlignment.Center;
            Binding heightBinding = new Binding();
            heightBinding.Path = new PropertyPath(InteractiveCursor.ActualWidthProperty);
            heightBinding.Source = cursor1;
            BindingOperations.SetBinding(cursor1.chartarea._recttop, WidthProperty, heightBinding);

            cursor1.CaptureMouse();
        }

        static void recttop_MouseEnter(object sender, MouseEventArgs e)
        {
            Rectangle rec = sender as Rectangle;
            if (rec != null)
            {
                rec.Cursor = Cursors.Arrow;
            }
        }

       static void cursortop_Loaded(object sender, RoutedEventArgs e)
        {
            InteractiveCursor cursor = sender as InteractiveCursor;
            DependencyObject obj = VisualTreeHelper.GetChild(cursor, 0);
            Rectangle rect = VisualTreeHelper.GetChild(obj, 3) as Rectangle;
            ContentPresenter presenter = VisualTreeHelper.GetChild(obj, 5) as ContentPresenter;
            cursor.chartarea._toppresenter = presenter;
            Binding toplabelbin = new Binding();
            toplabelbin.Source = cursor.chartarea;
            toplabelbin.Path = new PropertyPath(ChartArea.UpperRangeLabelProperty);
            toplabelbin.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(cursor.chartarea._toppresenter, ContentPresenter.ContentProperty, toplabelbin);
            cursor.HorizontalCursorStroke = cursor.chartarea.LineStroke;            
            SetValueForTopSelectionRange(cursor.chartarea, cursor.chartarea.SecondaryAxis);
        }
        private void CancelZoomingOnRangeSelection(ChartArea chartArea)
        {
            if (chartArea.m_areaPresenter != null)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(chartArea.m_areaPresenter);
                Adorner[] adorners = adornerLayer.GetAdorners(chartArea.m_areaPresenter);

                if (adorners != null)
                {
                    foreach (Adorner adorner in adorners)
                    {
                        ////Searches for all adorners till find zooming adorner.
                        if (adorner is ChartZoomingAdorner)
                        {
                            ////Removes and disposes adorner.
                            adornerLayer.Remove(adorner);
                            (adorner as IDisposable).Dispose();
                            chartArea.SetValue(ChartArea.ZoomSwitchedProperty, false);
                            chartArea.Cursor = null;
                        }
                    }
                }
            }
        }

        private void ResetOnRangeSelection(ChartArea chartArea)
        {
            if (chartArea != null)
            {
                if (chartArea.ZoomAllAxes)
                {
                    foreach (ChartAxis chartAxis in chartArea.Axes)
                    {
                        if (chartAxis.EnableZooming)
                        {
                            chartAxis.ZoomReset();
                            chartArea.VisibileRangeForZoomAllAxis(chartArea, chartAxis);
                        }
                    }
                }
                else
                {
                    chartArea.VerticalScrollingAxis.ZoomReset();
                    chartArea.VisibleRangeForZoomVerticalAxis(chartArea);
                    chartArea.HorizontalScrollingAxis.ZoomReset();
                    chartArea.VisibleRangeForZoomHorizontalAxis(chartArea);
                }
            }
        }

        static void cursor1_Loaded(object sender, RoutedEventArgs e)
        {
            InteractiveCursor cursor = sender as InteractiveCursor;
            cursor.VerticalCursorStroke = cursor.chartarea.LineStroke;
            SetValueForStartSelectionRange(cursor.chartarea, cursor.chartarea.PrimaryAxis);
            DependencyObject obj = VisualTreeHelper.GetChild(cursor, 0);
            ContentPresenter presenter = VisualTreeHelper.GetChild(obj, 5) as ContentPresenter;
            cursor.chartarea._toppresenter = presenter;
            cursor.chartarea._toppresenter.Visibility = Visibility.Visible;
            Binding upplabelbin = new Binding();
            upplabelbin.Source = cursor.chartarea;
            upplabelbin.Path = new PropertyPath(ChartArea.UpperRangeLabelProperty);
            upplabelbin.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(cursor.chartarea._toppresenter, ContentPresenter.ContentProperty, upplabelbin);
            cursor.chartarea._toppresenter.HorizontalAlignment = HorizontalAlignment.Center;
            Line line = Chart.FindInVisualTreeDown(VisualTreeHelper.GetChild(obj, 0), typeof(Line)) as Line;
            cursor.OffsetX = line.X2;
            cursor.chartarea._toppresenter.Margin = new Thickness((cursor.OffsetX) - (cursor.chartarea._toppresenter.ActualWidth), (cursor.chartarea.ActualHeight / 2 - (cursor.chartarea._toppresenter.ActualHeight / 2)), 0, 0);
        }

        static void cursor2_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            InteractiveCursor cursor1 = sender as InteractiveCursor;
            cursor1.InteractiveCursorSymbolVisibility = Visibility.Collapsed;
            cursor1.HorizontalLabelVisibility = Visibility.Collapsed;
            cursor1.VerticalLabelVisibility = Visibility.Collapsed;
            cursor1.LabelVisibility = Visibility.Collapsed;
            cursor1.chartarea._rect2.Visibility = Visibility.Visible;
            cursor1.chartarea._rangeSelection = false;
            Point pt = e.GetPosition(cursor1.chartarea);
            double value = cursor1.chartarea.PointToValue(cursor1.chartarea.PrimaryAxis, pt);            
            cursor1.chartarea.EndValue = value;            
            cursor2_MouseLeftButtonDown(cursor1, e);
            cursor2_MouseMove(cursor1, e);
            cursor1.ReleaseMouseCapture();
        }

        static void cursor2_MouseMove(object sender, MouseEventArgs e)
        {
            InteractiveCursor cursor1 = sender as InteractiveCursor;
            cursor1.InteractiveCursorSymbolVisibility = Visibility.Collapsed;
            cursor1.HorizontalLabelVisibility = Visibility.Collapsed;
            cursor1.VerticalLabelVisibility = Visibility.Collapsed;
            cursor1.HorizontalLabelVisibility = Visibility.Collapsed;
            cursor1.VerticalLabelVisibility = Visibility.Collapsed;
            cursor1.LabelVisibility = Visibility.Collapsed;
            if (cursor1.chartarea._rangeSelection == true)
            {
                //Fix for moving to Thumb after zooming , while dragging.
                Point pt = e.GetPosition(cursor1.chartarea);
                double value = cursor1.chartarea.PointToValue(cursor1.chartarea.PrimaryAxis, pt);
                cursor1.chartarea.EndValue = value; 

                Point p = e.GetPosition(cursor1);
                DependencyObject obj = VisualTreeHelper.GetChild(cursor1, 0);
                Line line = Chart.FindInVisualTreeDown(VisualTreeHelper.GetChild(obj, 0), typeof(Line)) as Line;
                Line line2 = Chart.FindInVisualTreeDown(VisualTreeHelper.GetChild(obj, 1), typeof(Line)) as Line;

                if ((line2.X2 - p.X) > 0)
                {
                    cursor1.chartarea._rect2.Width = (line2.X2 - p.X);
                    cursor1.OffsetX = p.X;
                    cursor1.chartarea._bottompresenter.Margin = new Thickness(p.X /2, (cursor1.chartarea.ActualHeight / 2) - (cursor1.chartarea._bottompresenter.ActualHeight / 2), 0, 0);
                }
                Canvas.SetLeft(cursor1.chartarea._rect2, p.X);
                Canvas.SetLeft(cursor1.chartarea._bottompresenter, p.X / 2 );
            }
        }

        static void cursor2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            InteractiveCursor cursor2 = sender as InteractiveCursor;
            if (cursor2 != null)
            {
                cursor2.InteractiveCursorSymbolVisibility = Visibility.Collapsed;
                cursor2.HorizontalLabelVisibility = Visibility.Collapsed;
                cursor2.VerticalLabelVisibility = Visibility.Collapsed;
                cursor2.chartarea._rangeSelection = true;
                cursor2.chartarea.ZoomAllAxes = false;
                cursor2.LabelVisibility = Visibility.Collapsed;
                DependencyObject obj = VisualTreeHelper.GetChild(cursor2, 0);
                Rectangle rect = VisualTreeHelper.GetChild(obj, 4) as Rectangle;
                cursor2.chartarea._rect2 = rect;
                
                cursor2.chartarea._rect2.Fill = cursor2.chartarea.SelectionStroke;
                cursor2.chartarea._rect2.Opacity = 0.25;
                cursor2.chartarea._rect2.Visibility = Visibility.Visible;
                cursor2.chartarea._rect2.MouseEnter += new MouseEventHandler(rect2_MouseEnter);
                ContentPresenter presenter = VisualTreeHelper.GetChild(obj, 6) as ContentPresenter;
                cursor2.chartarea._bottompresenter = presenter;
                cursor2.chartarea._bottompresenter.Visibility = Visibility.Visible;
                cursor2.chartarea._bottompresenter.Visibility = Visibility.Visible;            
                Binding lowlabelbin = new Binding();
                lowlabelbin.Source = cursor2.chartarea;
                lowlabelbin.Path = new PropertyPath(ChartArea.LowerRangeLabelProperty);
                lowlabelbin.Mode = BindingMode.TwoWay;
                BindingOperations.SetBinding(cursor2.chartarea._bottompresenter, ContentPresenter.ContentProperty, lowlabelbin);
                cursor2.chartarea._bottompresenter.HorizontalAlignment = HorizontalAlignment.Center;
                Binding heightBinding = new Binding();
                heightBinding.Path = new PropertyPath(InteractiveCursor.ActualHeightProperty);
                heightBinding.Source = cursor2;
                BindingOperations.SetBinding(cursor2.chartarea._rect2, HeightProperty, heightBinding);
                cursor2.CaptureMouse();
            }
        }

        static void rect2_MouseEnter(object sender, MouseEventArgs e)
        {
            Rectangle rec = sender as Rectangle;

            if (rec != null)
            {
                rec.Cursor = Cursors.Arrow;
            }
        }


        static void cursor1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            InteractiveCursor cursor1 = sender as InteractiveCursor;
            cursor1.InteractiveCursorSymbolVisibility = Visibility.Collapsed;
            cursor1.HorizontalLabelVisibility = Visibility.Collapsed;
            cursor1.VerticalLabelVisibility = Visibility.Collapsed;
            cursor1.LabelVisibility = Visibility.Collapsed;
            cursor1.chartarea._rect1.Visibility = Visibility.Visible;
            cursor1.chartarea._rangeSelection1 = false;
            Point pt = e.GetPosition(cursor1.chartarea);
            double value = cursor1.chartarea.PointToValue(cursor1.chartarea.PrimaryAxis, pt);
            cursor1.chartarea.StartValue = value;
            cursor1_MouseLeftButtonDown(cursor1, e);
            cursor1_MouseMove(cursor1, e);
            cursor1.ReleaseMouseCapture();
        }

        static void cursor1_MouseMove(object sender, MouseEventArgs e)
        {
            InteractiveCursor cursor1 = sender as InteractiveCursor;
            cursor1.InteractiveCursorSymbolVisibility = Visibility.Collapsed;
            cursor1.HorizontalLabelVisibility = Visibility.Collapsed;
            cursor1.VerticalLabelVisibility = Visibility.Collapsed;
            cursor1.LabelVisibility = Visibility.Collapsed;
            if (cursor1.chartarea._rangeSelection1 == true)
            {
                Point p = e.GetPosition(cursor1);
				//Fix for SD13383 - thumb indicator problem in zooming 
                Point pt = e.GetPosition(cursor1.chartarea);
                double value = cursor1.chartarea.PointToValue(cursor1.chartarea.PrimaryAxis, pt);
                cursor1.chartarea.StartValue = value;
                DependencyObject obj = VisualTreeHelper.GetChild(cursor1, 0);
                Line line = Chart.FindInVisualTreeDown(VisualTreeHelper.GetChild(obj, 0), typeof(Line)) as Line;
                if (p.X > 0)
                {
                    cursor1.chartarea._rect1.Width = p.X;
                    cursor1.OffsetX = cursor1.chartarea._rect1.Width;
                    cursor1.chartarea._toppresenter.Margin = new Thickness((cursor1.OffsetX) - (cursor1.chartarea._toppresenter.ActualWidth), (cursor1.chartarea.ActualHeight / 2 - (cursor1.chartarea._toppresenter.ActualHeight / 2)), 0, 0);
                    cursor1.chartarea.isLoadRectangleAtFirstTime = false;
                }
            }
        }

        static void cursor1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            InteractiveCursor cursor1 = sender as InteractiveCursor;
            cursor1.InteractiveCursorSymbolVisibility = Visibility.Collapsed;
            cursor1.HorizontalLabelVisibility = Visibility.Collapsed;
            cursor1.VerticalLabelVisibility = Visibility.Collapsed;
            cursor1.LabelVisibility = Visibility.Collapsed;
            cursor1.chartarea._rangeSelection1 = true;
            cursor1.chartarea.ZoomAllAxes = false;
            DependencyObject obj = VisualTreeHelper.GetChild(cursor1, 0);
            Rectangle rect = VisualTreeHelper.GetChild(obj, 3) as Rectangle;
            cursor1.chartarea._rect1 = rect;
            cursor1.chartarea._rect1.Fill = cursor1.chartarea.SelectionStroke;
            cursor1.chartarea._rect1.Opacity = 0.25;
            cursor1.chartarea._rect1.Visibility = Visibility.Visible;
            cursor1.chartarea._rect1.MouseEnter += new MouseEventHandler(rect1_MouseEnter);

            ContentPresenter presenter = VisualTreeHelper.GetChild(obj, 5) as ContentPresenter;
            cursor1.chartarea._toppresenter = presenter;
            cursor1.chartarea._toppresenter.Visibility = Visibility.Visible;
            Binding uplabelbin = new Binding();
            uplabelbin.Source = cursor1.chartarea;
            uplabelbin.Path = new PropertyPath(ChartArea.UpperRangeLabelProperty);
            uplabelbin.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(cursor1.chartarea._toppresenter, ContentPresenter.ContentProperty, uplabelbin);            
            cursor1.chartarea._toppresenter.HorizontalAlignment = HorizontalAlignment.Center;
        
            Binding heightBinding = new Binding();
            heightBinding.Path = new PropertyPath(InteractiveCursor.ActualHeightProperty);
            heightBinding.Source = cursor1;
            BindingOperations.SetBinding(cursor1.chartarea._rect1, HeightProperty, heightBinding);

            cursor1.CaptureMouse();
        }

        static void rect1_MouseEnter(object sender, MouseEventArgs e)
        {
            Rectangle rec = sender as Rectangle;
            if (rec != null)
            {
                rec.Cursor = Cursors.Arrow;
            }
        }


        internal double cursorOffset_x = 0d;
        static void cursor2_Loaded(object sender, RoutedEventArgs e)
        {
            InteractiveCursor cursor2 = sender as InteractiveCursor;
            cursor2.InteractiveCursorSymbolVisibility = Visibility.Collapsed;
            cursor2.HorizontalLabelVisibility = Visibility.Collapsed;
            cursor2.VerticalLabelVisibility = Visibility.Collapsed;
            cursor2.LabelVisibility = Visibility.Collapsed;
            cursor2.VerticalCursorStroke = cursor2.chartarea.LineStroke;
            DependencyObject obj = VisualTreeHelper.GetChild(cursor2, 0);
            Line line = Chart.FindInVisualTreeDown(VisualTreeHelper.GetChild(obj, 0), typeof(Line)) as Line;
            Line line2 = Chart.FindInVisualTreeDown(VisualTreeHelper.GetChild(obj, 1), typeof(Line)) as Line;
            cursor2.OffsetX = line.X2;
            cursor2.chartarea.cursorOffset_x = cursor2.OffsetX;
            SetValueForEndSelectionRange(cursor2.chartarea, cursor2.chartarea.PrimaryAxis);
            ContentPresenter presenter = VisualTreeHelper.GetChild(obj, 6) as ContentPresenter;
            cursor2.chartarea._bottompresenter = presenter;
            cursor2.chartarea._bottompresenter.Visibility = Visibility.Visible;
            Binding lowlabelbin = new Binding();
            lowlabelbin.Source = cursor2.chartarea;
            lowlabelbin.Path = new PropertyPath(ChartArea.LowerRangeLabelProperty);
            lowlabelbin.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(cursor2.chartarea._bottompresenter, ContentPresenter.ContentProperty, lowlabelbin);
            cursor2.chartarea._bottompresenter.HorizontalAlignment = HorizontalAlignment.Center;
            cursor2.chartarea._bottompresenter.Margin = new Thickness(cursor2.OffsetX / 2, (cursor2.chartarea.ActualHeight / 2) - (cursor2.chartarea._bottompresenter.ActualHeight / 2), 0, 0);
        }
        private static void setSeriesStyle(Style style, ChartArea area)
        {
            if (area.SeriesStyle != null)
            {
                foreach (ChartSeries series in area.Series)
                {
                    if (series != null)
                    {
                        if (area.stylename != null && (!(area.stylename.ToString().Contains("Default"))))
                        {
                            foreach (Setter set in style.Setters)
                            {
                                if (set.Property.ToString().Equals("LegendIcon"))
                                {
                                    series.LegendIcon = (ChartLegendIcon)set.Value;
                                }
                            }
                        }
                        series.Style = style;
                    }
                }
            }
            else
            {
                foreach (ChartSeries ser in area.Series)
                {
                    if (ser != null)
                    {
                        ser.LegendIcon = ser.legendicon;
                    }
                }
            }
        }

        private static void OnSeriesStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartArea area = d as ChartArea;
            if (area != null)
            {
                Style style = area.SeriesStyle;
                setSeriesStyle(style, area);

            }
        }
        private static void OnSecondaryAxisStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartArea area = d as ChartArea;
            if (area != null)
            {
                Style style = area.SecondaryAxisStyle;
                foreach (ChartAxis axis in area.Axes)
                {
                    if (axis != null)
                    {
                        if (axis.Orientation == Orientation.Vertical)
                        {
                            //axis.Style = style;
                            //if (style != null)
                            //{
                            //    foreach (Setter set in style.Setters)
                            //    {
                            //        if (set.Property.Name.ToString().Contains("LineStroke"))
                            //        {
                            //            Pen pen1 = set.Value as Pen;
                            //            axis.LineStroke = pen1;
                            //            ChartArea.SetGridLineStroke(axis, pen1);
                            //            ChartArea.SetOriginLineStroke(axis, pen1);
                            //        }
                            //    }
                            //}
                            //axis.SetHeaderStyle();
                        }
                    }
                }
            }
        }

        private static void OnPrimaryAxisStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartArea area = d as ChartArea;
            if (area != null)
            {
                Style style = area.PrimaryAxisStyle;
                //Below Condition is removed because it avoids the refresh of primary axis when ChartVisualStyle is Changed to None or Default. And more over there is no chance of NullReferenceException due to this.
                //if (style != null)
                //{
                    foreach (ChartAxis axis in area.Axes)
                    {
                        if (axis != null)
                        {
                            //bool value = ChartArea.GetShowGridLines(axis);
                            if (axis.Orientation == Orientation.Horizontal)
                            {
                                //axis.Style = style;
                                //axis.SetHeaderStyle();
                                //foreach (Setter set in style.Setters)
                                //{
                                //    if (set.Property.Name.ToString().Contains("LineStroke"))
                                //    {
                                //        Pen pen1 = set.Value as Pen;
                                //        axis.LineStroke = pen1;
                                //        ChartArea.SetGridLineStroke(axis, pen1);
                                //        ChartArea.SetOriginLineStroke(axis, pen1);
                                //    }
                                //}
                            }
                        }
                    }
                //}

            }
        }

        private static void OnLegendStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartArea area = d as ChartArea;
            Style style = area.LegendStyle;
            if (area.Legend != null)
            {
                area.Legend.Style = area.LegendStyle;
            }

        }
        /// <summary>
        /// Called when legend property was changed.
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnLegendPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartArea area = d as ChartArea;
            ////Making sure we've got the right calling object.
            if (area != null)
            {
                ////Coercing area's legend. 

                area.UpdateLegend(args.OldValue as ChartLegend, args.NewValue as ChartLegend);
            }

            //ChartLegendsCollection coll = d as ChartLegendsCollection;
            //foreach (ChartLegend legend in coll)
            //{
            //    if (this.StyleIndexValue >= 48)
            //    {
            //        ResourceDictionary rd = new ResourceDictionary()
            //        {
            //            Source = new Uri("/Syncfusion.Chart.Wpf;component/Themes/Generic.Chart.Templates.xaml", UriKind.RelativeOrAbsolute)
            //        };
            //        object obj1 = Enum.GetName(typeof(ChartStyles), this.StyleIndexValue);

            //        object obj = obj1 + "Legend";

            //        legend.Style = rd[obj] as Style;
            //    }
            //}

            Chart chart = area.Parent as Chart;
            if (chart != null)
            {
                if (chart.StyleIndexValue >= 48)
                {
                    if (area.rd == null)
                    {
                        string str = chart.ChartVisualStyle == ChartStyles.None || chart.ChartVisualStyle == ChartStyles.Default ? "Classic" : chart.ChartVisualStyle.ToString();
                        string uriSource = chart.StyleIndexValue >= 48 ? "/Syncfusion.Chart.Wpf;component/Themes/" + str + "Style.xaml" : "/Syncfusion.Chart.Wpf;component/Themes/Generic.Chart.Templates.xaml";

                        area.rd = ChartDictionaries.GetResourceDictionary(uriSource);
                        //area.rd = new SharedResourceDictionary()
                        //   {
                        //       Source = new Uri(uriSource, UriKind.RelativeOrAbsolute)
                        //   };
                    }
                    object obj1 = Enum.GetName(typeof(ChartStyles), chart.StyleIndexValue);

                    object obj = obj1 + "ChartLegendStyle";

                    area.Legend.Style = area.rd[obj] as Style;
                }

            }
        }

        /// <summary>
        /// Called when [coerce vertical scrolling axis].
        /// </summary>
        /// <param name="dpObj">The dp obj.</param>
        /// <param name="value">The value.</param>
        /// <returns>The vertical scrolling axis</returns>
        private static object OnCoerceVerticalScrollingAxis(DependencyObject dpObj, object value)
        {
            ChartArea area = dpObj as ChartArea;
            if (area != null && value == null)
            {
                ////If no value yet assigned to scrolling axis.
                return area.SecondaryAxis;
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// Called when [coerce horizontal scrolling axis].
        /// </summary>
        /// <param name="dpObj">The dp obj.</param>
        /// <param name="value">The value.</param>
        /// <returns>the Horizontal Scrolling Axis</returns>
        private static object OnCoerceHorizontalScrollingAxis(DependencyObject dpObj, object value)
        {
            ChartArea area = dpObj as ChartArea;

            if (area != null && value == null)
            {
                ////If no value yet assigned to scrolling axis.
                return area.PrimaryAxis;
            }
            else
            {
                return value;
            }
        }



        /// <summary>
        /// Called to coerce context menu.
        /// </summary>
        /// <param name="dpObj">The dp obj.</param>
        /// <param name="value">The value.</param>
        /// <returns>The contextmenu</returns>
        private static object OnCoerceContextMenu(DependencyObject dpObj, object value)
        {
            ChartArea area = dpObj as ChartArea;

            if (area.IsContextMenuEnabled == false || value != null)
            {
                if (area.CustomContextMenuItems != null)
                {
                    area.CustomContextMenuItems.Clear();
                }
            }
            if (area.isDisposed == false)
            {
                return area.IsContextMenuEnabled ? (value ?? new ChartAreaContextMenu()) : null;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Called when IsContextMenuEnabled changed.
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsContextMenuEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartArea area = d as ChartArea;
            area.CoerceValue(ContextMenuProperty);
        }

        /// <summary>
        /// Handles visibles the series collection changes.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnVisibleSeriesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (PrimarySeries != null && PrimarySeries.ChartType != null)
            {
                if (PrimarySeries.ChartType.IsRotated)
                {
                    PrimaryAxis.Orientation = Orientation.Vertical;
                    SecondaryAxis.Orientation = Orientation.Horizontal;
                    if (this.View3DMode)
                    {
                        DepthAxis.depthaxisflag = true;
                        DepthAxis.Orientation = Orientation.Horizontal;
                    }
                    HorizontalScrollingAxis = SecondaryAxis;
                    VerticalScrollingAxis = PrimaryAxis;
                }
                else
                {
                    PrimaryAxis.Orientation = Orientation.Horizontal;
                    SecondaryAxis.Orientation = Orientation.Vertical;
                    if (this.View3DMode)
                    {
                        DepthAxis.depthaxisflag = true;
                        DepthAxis.Orientation = Orientation.Horizontal;
                    }
                    HorizontalScrollingAxis = PrimaryAxis;
                    VerticalScrollingAxis = SecondaryAxis;
                }
            }

            VisibleSeriesCollection visibleSeries = sender as VisibleSeriesCollection;
            //If there are visible series.
            if (visibleSeries.Count > 0 && Legend != null)
            {
                ChartSeries chartSeries = visibleSeries[0] as ChartSeries;
                ////If only 1 series is visible.
                if (visibleSeries.Count == 1)
                {
                    if (CheckCompatibility(chartSeries.ChartType) && Legend.IsSegmentsLegend)
                    {
                        if (Legend != null)
                        {
                            if (chartSeries.Type == ChartTypes.Pyramid || chartSeries.Type == ChartTypes.Funnel)
                            {
                                Legend.ItemsSource = chartSeries.Segments;
                            }
                            else if (this.VisibleSeries.Count == 1)
                            {
                                Legend.ItemsSource = chartSeries.Segments;
                            }
                            else
                            {
                                Legend.ItemsSource = this.AreaSegments;
                            }
                        }

                        return;
                    }
                }
                // if (Legend.Items.Count == 0)
                {
                    ChartSeriesCollection collection = new ChartSeriesCollection();

                    foreach (ChartSeries item in Series)
                    {
                        if (item.IsVisibleOnLegend == true && item.VisibilityOnLegend != Visibility.Collapsed)
                        {
                            collection.Add(item);
                        }
                        if (item.IsVisibleOnLegend == false)
                        {
                            collection.Remove(item);
                        }
                    }
                    if (Legend != null && Legend.Items.Count > 0 && Legend.ItemsSource == null)
                    {
                        Legend.Items.Clear();
                    }
                    if (Legend != null)
                    {
                        if (collection.Count != 0)
                        {
                            if (Legend.IsSegmentsLegend && CheckCompatibility(chartSeries.ChartType))
                                Legend.ItemsSource = this.AreaSegments;
                            else
                                Legend.ItemsSource = collection;
                        }
                        else
                        {
                            Legend.ItemsSource = null;
                        }
                    }
                }
            }

            if (Series.Count == 0 && Legend != null)
            {
                if (Legend.Items.Count != 0)
                    Legend.ItemsSource = null;
            }
        }



        private static void OnShowOriginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;

            if (axis != null && axis.Area != null)
            {
                axis.Area.UpdateArea();
            }

        }




        /// <summary>
        /// Called when SideBySideSeriesPlacement property changed.
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSideBySideSeriesPlacementPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartArea area = d as ChartArea;
            if (area != null)
            {
                area.m_visibleSeriesSegmentsRecountRequired = true;
                area.UpdateArea();
            }
        }

        /// <summary>
        /// Requests for segments reset.
        /// </summary>
        internal void RequestForSegmentsReset()
        {
            this.m_visibleSeriesSegmentsRecountRequired = true;
            this.m_segmentsResetRequired = true;
        }
        internal ObservableCollection<object> AreaSegments = new ObservableCollection<object>();
        
        /// <summary>
        /// Updates the legend.
        /// </summary>
        /// <param name="oldLegend">The old legend</param>
        /// <param name="newLegend">The new legend</param>
        /// <seealso cref="ChartArea"/>
        private void UpdateLegend(ChartLegend oldLegend, ChartLegend newLegend)
        {

            if (_mDockPanel != null)
            {
                if (oldLegend != null)
                {
                    _mDockPanel.Children.Remove(oldLegend);
                }

                if (newLegend != null)
                {
                    SetLegendSource(newLegend);
                }
            }
        }

        internal void SetLegendSource(ChartLegend newLegend)
        {
            if (newLegend.Parent != null)
            {
                (newLegend.Parent as ChartDockPanel).Children.Remove(newLegend);
            }

            if (this.IsSync == true)
            {
                Legend.Margin = new Thickness(4);
            }
            _mDockPanel.Children.Add(Legend);
            if (this.m_visibleSeries.Count > 0)
            {
                ////Getting visible item.
                ChartSeries chartSeries = m_visibleSeries[0];

                ////If segments should be shown on legend. 
                if (CheckCompatibility(chartSeries.ChartType) && Legend.IsSegmentsLegend)
                {
                    if (Legend != null)
                    {
                        ////Making source as segments.
                        // if (Legend.Items.Count == 0)
                        if (chartSeries.Type == ChartTypes.Funnel || chartSeries.Type == ChartTypes.Pyramid)
                        {
                            Legend.ItemsSource = chartSeries.Segments;
                        }
                        else if (this.m_visibleSeries.Count == 1)
                        {
                            Legend.ItemsSource = chartSeries.Segments;
                        }
                        else
                        {
                            Legend.ItemsSource = AreaSegments;
                        }
                    }
                }
                else
                {
                    ////Making source as series.
                    if (chartSeries.Area != null)
                    {
                        SetLegendItemSource(chartSeries);
                    }
                }

                Style style = LegendStyle;
                if (LegendStyle != null)
                {
                    Legend.Style = LegendStyle;
                }
            }
            Chart chart = this.Parent as Chart;
            //Legend.FontSize = Legend.m_isFontSizeSet ? Legend.FontSize : (this.m_isFontSizeSet ? this.FontSize : (chart != null && chart.m_isFontSizeSet) ? chart.FontSize : 12);
            //Legend.FontWeight = Legend.m_isFontWeightSet ? Legend.FontWeight : (this.m_isFontWeightSet ? this.FontWeight : (chart != null && chart.m_isFontWeightSet) ? chart.FontWeight : FontWeights.Normal);
            //Legend.FontFamily = Legend.m_isFontFamilySet ? Legend.FontFamily : (this.m_isFontFamilySet ? this.FontFamily : (chart != null && chart.m_isFontFamilySet) ? chart.FontFamily : new FontFamily("Segoe UI"));

            Legend.m_area = this;

        }

        /// <summary>
        /// Sets the legend item source.
        /// </summary>
        /// <param name="chartSeries">The chart series.</param>
        internal void SetLegendItemSource(ChartSeries chartSeries)
        {
            ChartSeriesCollection collection = new ChartSeriesCollection();
            foreach (ChartSeries item in chartSeries.Area.Series)
            {
                if (item.IsVisibleOnLegend == true && item.VisibilityOnLegend != Visibility.Collapsed)
                {
                    collection.Add(item);
                }
                if (!item.IsVisibleOnLegend)
                {
                    collection.Remove(item);
                }
            }

            if (Legend != null)
                Legend.ItemsSource = collection;
        }

        /// <summary>
        /// Visibiles the range for zoom all axis.
        /// </summary>
        /// <param name="chartArea">The chart area.</param>
        /// <param name="chartAxis">The chart axis.</param>
        /// <seealso cref="ChartArea"/>
        public void VisibileRangeForZoomAllAxis(ChartArea chartArea, ChartAxis chartAxis)
        {
            DoubleRange zoomrangevalue = new DoubleRange(0, 0);
            if (chartAxis == chartArea.PrimaryAxis)
            {
                Point start = new Point(chartArea.ValueToPoint(chartAxis, chartAxis.VisibleRange.Start), 0);
                Point end = new Point(chartArea.ValueToPoint(chartAxis, chartAxis.VisibleRange.End), 0);
                double d1 = chartArea.PointToValue(chartAxis, start);
                double d2 = chartArea.PointToValue(chartAxis, end);

                if (double.IsNaN(d1) == false && double.IsNaN(d2) == false)
                    zoomrangevalue = new DoubleRange(d1, d2);

                chartArea.ZoomedXRange = zoomrangevalue;

            }
            else if (chartAxis == chartArea.SecondaryAxis)
            {
                Point start = new Point(0, chartArea.ValueToPoint(chartAxis, chartAxis.VisibleRange.Start));
                Point end = new Point(0, chartArea.ValueToPoint(chartAxis, chartAxis.VisibleRange.End));
                double d1 = chartArea.PointToValue(chartAxis, start);
                double d2 = chartArea.PointToValue(chartAxis, end);

                if (double.IsNaN(d1) == false && double.IsNaN(d2) == false)
                    zoomrangevalue = new DoubleRange(d1, d2);

                chartArea.ZoomedYRange = zoomrangevalue;
            }
        }

        internal void VisibleRangeForZoomHorizontalAxis(ChartArea chartArea)
        {
            DoubleRange zoomrangevalue = new DoubleRange(0, 0);
            Point start = new Point(chartArea.ValueToPoint(chartArea.HorizontalScrollingAxis, chartArea.HorizontalScrollingAxis.VisibleRange.Start), 0);
            Point end = new Point(chartArea.ValueToPoint(chartArea.HorizontalScrollingAxis, chartArea.HorizontalScrollingAxis.VisibleRange.End), 0);
            double d1 = chartArea.PointToValue(chartArea.HorizontalScrollingAxis, start);
            double d2 = chartArea.PointToValue(chartArea.HorizontalScrollingAxis, end);

            if (double.IsNaN(d1) == false && double.IsNaN(d2) == false)
                zoomrangevalue = new DoubleRange(d1, d2);


            if (chartArea.SyncChartArea != null)
            {
                SyncChartAreas area = chartArea.ChartAreaParent;
                if (area != null)
                {
                    area.ZoomedXRange = zoomrangevalue;
                }
            }
            else
            {
                chartArea.ZoomedXRange = zoomrangevalue;
            }
        }
        private void VisibleRangeForZoomVerticalAxis(ChartArea chartArea)
        {
            DoubleRange zoomrangevalue = new DoubleRange(0, 0);
            Point start = new Point(0, chartArea.ValueToPoint(chartArea.VerticalScrollingAxis, chartArea.VerticalScrollingAxis.VisibleRange.Start));
            Point end = new Point(0, chartArea.ValueToPoint(chartArea.VerticalScrollingAxis, chartArea.VerticalScrollingAxis.VisibleRange.End));
            double d1 = chartArea.PointToValue(chartArea.VerticalScrollingAxis, start);
            double d2 = chartArea.PointToValue(chartArea.VerticalScrollingAxis, end);

            if (double.IsNaN(d1) == false && double.IsNaN(d2) == false)
                zoomrangevalue = new DoubleRange(d1, d2);

            chartArea.ZoomedYRange = zoomrangevalue;

        }

        /// <summary>
        /// Method to reset the RangeSelection cursor and Labels, when Orientation is Vertical
        /// </summary>
        private void ResetVerticalSelectedRange()
        {
            if (m_areaPresenter == null)
                return;
			//Fix for SD13383 - thumb indicator problem in zooming 
            var vScrollBar = ((m_areaPresenter.ContentTemplateSelector as ChartAreaTemplateSelector).CartesianTemplate.FindName("VerticalScrollBar", m_areaPresenter) as ChartZoomingScrollBar);
            vScrollBar.UpdateLayout();
         
            //Left Thumb positioning
            double range = this.ValueToPoint(this.PrimaryAxis, this.StartValue) - (this.AxesThickness.Left + this.ElementMargin.Left);
            this._cursor1.OffsetX = range;
            if (this._cursor1.OffsetX >= 0)
            {
                this._rect1.Width = this._cursor1.OffsetX;
            }
            else
            {
                this._rect1.Width = 0;
            }
            if (this._cursor1.chartarea != null && this._cursor1.chartarea._bottompresenter != null && !double.IsNaN(this._cursor1.OffsetX))
            {
                this._cursor1.VerticalCursorStroke = this.LineStroke;
                this._cursor1.chartarea._toppresenter.Margin = new Thickness(this._cursor1.OffsetX / 2, (this._cursor1.chartarea.ActualHeight / 2) - (this._cursor1.chartarea._bottompresenter.ActualHeight / 2), 0, 0);
            }

            //Right Thumb positioning
            range = this.ValueToPoint(this.PrimaryAxis, this.EndValue) - (this.AxesThickness.Left + this.ElementMargin.Left);
            double endvalue = this.ValueToPoint(this.PrimaryAxis, this.PrimaryAxis.VisibleRange.End) - (this.AxesThickness.Left + this.ElementMargin.Left);
            this._cursor2.OffsetX = range;
            this._rect2.Width = (range >= endvalue ? (range - endvalue) : endvalue - range);
            Canvas.SetLeft(this._rect2, this._cursor2.OffsetX);
            if (this._cursor2.chartarea != null && this._cursor2.chartarea._bottompresenter != null && !double.IsNaN(this._cursor2.OffsetX))
            {
                this._cursor2.VerticalCursorStroke = this.LineStroke;
                this._cursor2.chartarea._bottompresenter.Margin = new Thickness(this._cursor2.OffsetX / 2, (this._cursor2.chartarea.ActualHeight / 2) - (this._cursor2.chartarea._bottompresenter.ActualHeight / 2), 0, 0);
            }
        }

        /// <summary>
        /// Method to reset the RangeSelection cursor and Labels, when Orientation is Horizontal
        /// </summary>
        private void ResetHorizontalSelectedRange()
        {
            if (m_areaPresenter == null)
                return;
		    //Fix for SD13383 - thumb indicator problem in zooming 
            var hScrollBar = ((m_areaPresenter.ContentTemplateSelector as ChartAreaTemplateSelector).CartesianTemplate.FindName("HorizontalScrollBar", m_areaPresenter) as ChartZoomingScrollBar);
            hScrollBar.UpdateLayout();
         
            //Top Thumb positioning
            double range = this.ValueToPoint(this.SecondaryAxis, this.StartValue); //4 is the element margine of the chart area
            this._cursortop.OffsetY = range-this.DesiredSize.Height/2;
            if (this._cursortop.OffsetY >= 0)
            {
                this._recttop.Height = this._cursortop.OffsetY;
            }
            else
            {
                this._recttop.Height = 0;
            }
            if (this._cursortop.chartarea != null && this._cursortop.chartarea._toppresenter != null)
            {
                this._cursortop.HorizontalCursorStroke = this.LineStroke;
                this._cursortop.chartarea._toppresenter.Margin = new Thickness((this._cursortop.chartarea.ActualWidth / 2 - (this._cursortop.chartarea._toppresenter.ActualWidth / 2)), this._cursortop.OffsetY / 2, 0, 0);
            }

            //Bottom Thumb positioning
            range = this.ValueToPoint(this.SecondaryAxis, this.EndValue);
            double endvalue = this.ValueToPoint(this.SecondaryAxis, this.SecondaryAxis.VisibleRange.Start);
            this._cursorbottom.OffsetY = range - 13;
            this._rectbottom.Height = (range >= endvalue ? (range - endvalue) : endvalue - range);

            Canvas.SetTop(this._rectbottom, this._cursorbottom.OffsetY);
            if (this._cursorbottom.chartarea != null && this._cursorbottom.chartarea._bottompresenter != null && !double.IsNaN(this._cursorbottom.OffsetY))
            {
                this._cursorbottom.HorizontalCursorStroke = this.LineStroke;
                this._cursorbottom.chartarea._bottompresenter.Margin = new Thickness(((this._cursorbottom.ActualWidth / 2) - (this._cursorbottom.chartarea._bottompresenter.ActualWidth / 2)), this._rectbottom.Height / 2, 0, 0);
                Canvas.SetTop(this._bottompresenter, range);
            }
        }

        #endregion

        internal bool isDisposed = false;
        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public virtual void Dispose()
        {
            if (adorner != null)
            {
                adorner = null;
            }
           // this.ClearValue(ChartArea.ColorModelProperty);
            isDisposed = true;
            //if (this.m_colorModel != null)
            //{
                this.m_colorModel.Dispose();
           // }
            if (this.annotationAdorner != null)
            {
                this.annotationAdorner.Dispose();
                this.annotationAdorner = null;
            }

            //FontFamilyDescriptor.RemoveValueChanged(this, new EventHandler(FontFamilyChanged));
            //FontSizeDescriptor.RemoveValueChanged(this, new EventHandler(FontSizeChanged));
            //FontWeightDescriptor.RemoveValueChanged(this, new EventHandler(FontWeightChanged));
            //ForegroundDescriptor.RemoveValueChanged((this), new EventHandler(ForegroundChanged));

            if (Series != null)
            {
                foreach (ChartSeries series in Series)
                {
                    series.AppearanceChanged -= new EventHandler(OnSeriesAppearanceChanged);
                }
            }
           
            if (Axes != null)
            {

                foreach (ChartAxis axis in Axes)
                {
                    axis.Changed -= new EventHandler(OnAxisChanged);
                    if (axis.Area != null)
                    {
                        axis.Changed -= new EventHandler(axis.Area.OnVerticalScrollingAxisChanged);
                        axis.Changed -= new EventHandler(axis.Area.OnHorizontalScrollingAxisChanged);                        
                    }
                }                
            }

            if (this.m_visibleSeries != null)
            {
                foreach (ChartSeries item in m_visibleSeries)
                {
                    item.Dispose();
                }
                this.m_visibleSeries.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnVisibleSeriesCollectionChanged);
                this.m_visibleSeries.Clear();
                m_visibleSeries = null;
            }

            if (this.m_series != null)
            {
                foreach (ChartSeries item in m_series)
                {
                    item.Dispose();
                }
                m_series.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnSeriesCollectionChanged);
                this.m_series.Clear();
                m_series = null;
            }


            if (this.m_axes != null)
            {
                this.m_axes.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnAxesCollectionChanged);
                for (int i = 0; i < m_axes.Count; i++)
                {
                    m_axes[i].Dispose();
                    m_axes[i] = null;
                }               
                this.m_axes.Clear();
                m_axes = null;
            }
            if (this.Axes != null)
            {
                this.Axes.Clear();
                Axes = null;
            }
            //this.PrimaryAxis = null;
            

            if (this.ColorModel != null)
            {
                this.ColorModel.PropertyChanged -= new PropertyChangedEventHandler(ColorModel_PropertyChanged);
            }
           
            this.m_axes = null;
            this._mDockPanel = null;
            this.m_highlightedSegment = null;
            this.m_series = null;
            this.m_visibleSeries = null;
           
            if (this.Resources != null)
            {
                this.Resources.MergedDictionaries.Clear();
                this.Resources.Clear();                
                this.Resources = null;
            }
           
            if (this.ContextMenu != null)
            {
                if ((this.ContextMenu as ChartAreaContextMenu) != null)
                    (this.ContextMenu as ChartAreaContextMenu).Dispose();

                if (this.ContextMenu.Items != null)
                {
                    this.ContextMenu.Items.Clear();
                }
                this.ContextMenu.Template = null;
                this.ContextMenu = null;
            }
            if (this.Template != null)
            {
                if (((ControlTemplate)(this.Template)).Resources != null)
                {
                    ((ControlTemplate)(this.Template)).Resources.MergedDictionaries.Clear();
                }
            }           

            if (this.InteractiveCursors != null)
            {
                this.InteractiveCursors.Clear();
                InteractiveCursors.CollectionChanged -= new NotifyCollectionChangedEventHandler(InteractiveCursorCollection_CollectionChanged);
                this.InteractiveCursors = null;
            }
            if (this._cursorbottom != null)
            {
                this._cursorbottom = null;
            }
            if (this._cursortop != null)
            {
                this._cursortop = null;
            }
           // this.tempalteRD = null;
            this.Template = null;
            this._cursor1 = null;
            this._cursor2 = null;

            if (this.PrimaryAxis != null)
            {
                this.PrimaryAxis.Dispose();
                this.PrimaryAxis = null;
            }

            if (this.SecondaryAxis != null)
            {
                this.SecondaryAxis.Dispose();
                this.SecondaryAxis = null;
            }

            if (this.VerticalScrollingAxis != null)
            {
                this.VerticalScrollingAxis.Dispose();
                this.VerticalScrollingAxis = null;
            }
            if (this.HorizontalScrollingAxis != null)
            {
                this.HorizontalScrollingAxis.Dispose();
                this.HorizontalScrollingAxis = null;
            }
            //if (this.ColorModel != null)
            //{
            //    this.ColorModel.Dispose();

            //    this.ColorModel = null;
            //}
            //this.m_colorModel = null;

            if (this.m_areaPresenter != null)
            {
                Type t = typeof(ChartAdornmentsPresenter);
                var adornmentpresenter = Chart.FindInVisualTreeDown(this.m_areaPresenter, t);
                if (adornmentpresenter != null)
                    (adornmentpresenter as ChartAdornmentsPresenter).Dispose();
            }
            //this.m_areaPresenter = null;
            if (this.m_areaPresenter != null)
            {
                this.m_areaPresenter.Dispose();
                this.m_areaPresenter = null;
            }
            if (this.rd != null)
            {
                this.rd.MergedDictionaries.Clear();
                this.rd.Clear();
                (this.rd as SharedResourceDictionary)._sourceUri = null;
                this.rd = null;
            }
            if (this.baseRd != null)
            {
                this.baseRd.MergedDictionaries.Clear();
                this.baseRd.Clear();
                (this.baseRd as SharedResourceDictionary)._sourceUri = null;
                this.baseRd = null;
            }
            if (this.tempalteRD != null)
            {
                for (int i = 0; i < this.tempalteRD.MergedDictionaries.Count; i++)
                {
                    this.tempalteRD.MergedDictionaries[i].MergedDictionaries.Clear();                    
                    this.tempalteRD.MergedDictionaries[i].Clear();                   
                }
                this.tempalteRD.MergedDictionaries.Clear();
                this.tempalteRD.Clear();
                (this.tempalteRD as SharedResourceDictionary)._sourceUri = null;
                this.tempalteRD = null;
            }
            if (this.fastsegRd_coll != null)
            {
                for (int i = 0; i < this.fastsegRd_coll.Count; i++)
                {
                    this.fastsegRd_coll[i].MergedDictionaries.Clear();
                    this.fastsegRd_coll[i].Clear();
                    (this.fastsegRd_coll[i] as SharedResourceDictionary)._sourceUri = null;
                    this.fastsegRd_coll[i] = null;
                }
                this.fastsegRd_coll.Clear();
                this.fastsegRd_coll = null;
            }
            if (this.segRd_coll != null)
            {
                for (int i = 0; i < this.segRd_coll.Count; i++)
                {
                    this.segRd_coll[i].MergedDictionaries.Clear();
                    this.segRd_coll[i].Clear();
                    (this.segRd_coll[i] as SharedResourceDictionary)._sourceUri = null;
                    this.segRd_coll[i] = null;
                }
                this.segRd_coll.Clear();
                this.segRd_coll = null;
            }
            if (this.hitTestList != null)
            {
                this.hitTestList.Clear();
                this.hitTestList = null;
            }
            if(this.AreaSegments!=null)
            this.AreaSegments.Clear();
            if (this.Legend != null)
            {
                this.Legend.m_area = null;
                this.Legend.m_highlightedElement = null;
                this.Legend.ItemsSource = null;
                this.Legend.Items.Clear();
                this.Legend = null;
            }
            
        }




        #endregion

        #region IChartSerializer Members
        /// <summary>
        /// Converts ChartArea into xaml String
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            //string _xamlString;
            EditorHelper.Register<BindingExpression, BindingConvertor>();
           
            PrimaryAxis = (ChartAxis)GetValue(PrimaryAxisProperty);
            SecondaryAxis = (ChartAxis)GetValue(SecondaryAxisProperty);
            StringBuilder outstr = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;
            XamlDesignerSerializationManager dsm = new XamlDesignerSerializationManager(XmlWriter.Create(outstr, settings));
            //this string need for turning on expression saving mode 
            dsm.XamlWriterMode = XamlWriterMode.Expression;
            XamlWriter.Save(this, dsm);
            //_xamlString = outstr.ToString();
            //if (outstr.ToString().Contains("</ChartArea>"))
            //{
            //    outstr = outstr.Remove(outstr.Length - 13, 13);
            //}
            //else
            //{
            //    outstr = outstr.Remove(outstr.Length - 2, 1);
            //}

            //#region Legend

            //if (this.Legend != null)
            //{
            //    StringBuilder _legendString = new StringBuilder("<ChartArea.Legend>");
            //    _legendString.Append(this.Legend.Serialize());
            //    _legendString.Append("</ChartArea.Legend>");
            //    outstr = outstr.Replace(Chart.SubString(outstr.ToString(), "<ChartArea.Legend>", "</ChartArea.Legend>"), _legendString.ToString());
            //}

            //#endregion

            #region Series

            if (this.Series != null || this.Series.Count != 0)
            {
                StringBuilder _seriesString = new StringBuilder("<ChartArea.Series>");
                foreach (ChartSeries _series in this.Series)
                {
                    _seriesString.Append(_series.Serialize());
                }
                _seriesString.Append("</ChartArea.Series>");
                outstr = outstr.Replace(Chart.SubString(outstr.ToString(), "<ChartArea.Series>", "</ChartArea.Series>"), _seriesString.ToString());
            }
            #endregion

            //outstr.Append("</ChartArea>");
            return (outstr.ToString());

        }
        /// <summary>
        /// Converts propert xaml string into ChartArea
        /// </summary>
        /// <param name="xamlString"></param>
        /// <returns></returns>
        public object Deserialize(string xamlString)
        {
            return XamlReader.Parse(xamlString);
        }

        #endregion
    }

}