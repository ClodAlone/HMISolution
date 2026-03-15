#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Linq;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.Collections.Specialized;

using System.ComponentModel;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class represents chart area.
    /// </summary>
    /// <remarks>
    /// The Chart Contains any number of Areas. Each area has its own ChartAxis,
    /// ChartSeries, ChartLegends.
    /// </remarks>
    /// <seealso cref="Chart">Chart class specification</seealso>
    /// <seealso cref="ChartSeries">ChartSeries class specification</seealso>
    /// <seealso cref="ChartArea">ChartArea class specification</seealso>
    /// <seealso cref="ChartAxis">ChartAxis class specification</seealso>
    /// <seealso cref="ChartTypes">ChartTypes enumeration</seealso>
    [ContentProperty("Series")]
    public class ChartArea : Control,IDisposable
    {
        #region visual Styles
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
        /// Called LegendStyle is changed in ChartArea
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        public static void OnLegendStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartArea area = d as ChartArea;
            if (area != null)
            {

                if (area.Legends != null)
                {
                    area.Legends.Style = area.LegendStyle;
                }

            }
        }
        /// <summary>
        /// Identifies the PrimaryAxisStyle dependency property. 
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
        /// Called when PrimaryAxisStyle changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        public static void OnPrimaryAxisStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartArea area = d as ChartArea;
            if (area != null)
            {
                //if (area.PrimaryAxis != null)
                //{
                //    area.PrimaryAxis.Style = area.PrimaryAxisStyle;
                //}

                foreach (ChartAxis axis in area.Axes)
                {
                    if (axis.Orientation == Orientation.Horizontal)
                    {
                        axis.Style = area.PrimaryAxisStyle;
                    }
                }
            }
        }
        /// <summary>
        /// Identifies the SecondaryAxisStyle dependency property.
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
        /// Called when SecondaryAxisStyle changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        public static void OnSecondaryAxisStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartArea area = d as ChartArea;
            if (area != null)
            {
                foreach (ChartAxis axis in area.Axes)
                {
                    if (axis.Orientation == Orientation.Vertical)
                    {
                        axis.Style = area.SecondaryAxisStyle;
                    }
                }
            }
        }

        /// <summary>
        /// Identifies the SeriesStyle dependency property.
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
        /// Called when SeriesStyle is changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        public static void OnSeriesStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartArea area = d as ChartArea;
            if (area != null)
            {
                if (area.Series != null)
                {
                    foreach (ChartSeries series in area.Series)
                    {
                        series.Style = area.SeriesStyle;
                    }
                }
            }
        }
#endregion
        internal double pre_ht=0;
		internal string m_watermarkText = string.Empty;
        internal ImageSource m_watermarkimageSource = null;
        internal bool isZoomactivated = false, indexedarea = false, isFirstTimeAnimate = true;
        internal ChartAxesType firstseriestype = ChartAxesType.Null;
        internal Point centerPoint = new Point();
        internal double angle = 0d;
        internal double maxRadius = 0d;
        private bool m_isresize = true;
        internal bool IsBeginInit = false;
        internal double AreaPaddingBottom = 0d;
        internal bool updated = false;
        internal Canvas splitcanvas;
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
        /// <summary>
        /// Identifies the ChartAreaParent dependency property.
        /// </summary>
        internal static readonly DependencyProperty ChartAreaParentProperty =
           DependencyProperty.Register("ChartAreaParent", typeof(SyncChartAreas), typeof(ChartArea), new PropertyMetadata(null));

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
        internal bool flag = false;
        internal bool IsSplitterDrag = false;
        internal bool isIndicatorEnabled = false;
        //private ContextMenuItemsCollection DefaultChartAreaContextMenuItems = new ContextMenuItemsCollection();
        private ContextMenuItemsCollection ChartAreaContextMenuItems = new ContextMenuItemsCollection();
        private ContextMenuAdv ChartAreaContextMenu = new ContextMenuAdv() { Visibility = Visibility.Collapsed };

        internal bool IsSync = false;

        internal bool hasMinWidth = false;
        internal int index = 0;

        internal Rect axisRect = new Rect();

        internal bool hasMinWidthInLeft = false;

        internal bool hasMinWidthInRight = false;
        private Host host = Host.Chart;
        /// <summary>
        /// Get or Set HostProperty
        /// </summary>
        protected internal Host Host
        {
            get
            {
                return host;
            }

            set
            {
                host = value;
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

        /// <summary>
        /// BeginInit method implementation for performance
        /// </summary>
        public void BeginInit()
        {
            this.IsBeginInit = true;
        }

        /// <summary>
        /// EndInit method implementation for performance
        /// </summary>
        public void EndInit()
        {
            this.IsBeginInit = false;
            this.LoadArea();
        }
        internal Segment dragSegment = null;
        internal Point dragXY = new Point();
        internal bool isAllowSegmentDragDrop = false;
        /// <summary>
        /// Identifies the AlternatingGridBackground dependency property.
        /// </summary>
        public static readonly DependencyProperty AlternatingGridBackgroundProperty =
            DependencyProperty.Register("AlternatingGridBackground", typeof(Brush), typeof(ChartArea), new PropertyMetadata(new SolidColorBrush(Colors.Transparent), new PropertyChangedCallback(OnValueChanged)));
        /// <summary>
        /// Get or Set AlternatingGridBackgroundProperty
        /// </summary>
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
        /// Identifies the AlternatingFillMode dependency property.
        /// </summary>
        public static readonly DependencyProperty AlternatingFillModeProperty =
            DependencyProperty.Register("AlternatingFillMode", typeof(AlternatingFillMode), typeof(ChartArea), new PropertyMetadata(AlternatingFillMode.Even, new PropertyChangedCallback(OnValueChanged)));
       /// <summary>
       /// Get or Set AlternatingFillMode
       /// </summary>
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
        /// Identifies the AlternatingFillDirection dependency property.
        /// </summary>
        public static readonly DependencyProperty AlternatingFillDirectionProperty =
            DependencyProperty.Register("AlternatingFillDirection", typeof(Orientation), typeof(ChartArea), new PropertyMetadata(Orientation.Horizontal, new PropertyChangedCallback(OnValueChanged)));
        /// <summary>
        /// Get or Set AlternatingFillDirection
        /// </summary>
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
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartArea area = d as ChartArea;
            if (area != null)
            {
                //Fix for the issue in binding Alternative GridBackground properties.
                area.LoadArea();
                //foreach (ChartAxis axis in area.Axes)
                //{
                //    axis.UpdateAxis();
                //}
            }
        }
        /// <summary>
        /// Enable Chartarea AllowSegmentDragDrop of the Chart Area Depedency Property
        /// </summary>
        public static readonly DependencyProperty AllowSegmentDragDropProperty =
           DependencyProperty.Register("AllowSegmentDragDrop", typeof(bool), typeof(ChartArea), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the AllowSegmentDragDrop
        /// </summary>
        public bool AllowSegmentDragDrop
        {
            get { return (bool)GetValue(AllowSegmentDragDropProperty); }
            set { SetValue(AllowSegmentDragDropProperty, value); }
        }

        /// <summary>
        /// Enable Chartarea EnableMouseDragZooming of the Chart Area Depedency Property
        /// </summary>
        public static readonly DependencyProperty EnableMouseDragZoomingProperty =
           DependencyProperty.Register("EnableMouseDragZooming", typeof(bool), typeof(ChartArea), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets the EnableMouseDragZooming
        /// </summary>
        public bool EnableMouseDragZooming
        {
            get { return (bool)GetValue(EnableMouseDragZoomingProperty); }
            set { SetValue(EnableMouseDragZoomingProperty, value); }
        }

        /// <summary>
        /// Enable Chartarea Effects visibility of the Chart Area Depedency Property
        /// </summary>
        public static readonly DependencyProperty EnableChartAreaEffectsProperty =
           DependencyProperty.Register("EnableChartAreaEffects", typeof(bool), typeof(ChartArea), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the Effects brush visibility
        /// </summary>
        public bool EnableChartAreaEffects
        {
            get { return (bool)GetValue(EnableChartAreaEffectsProperty); }
            set { SetValue(EnableChartAreaEffectsProperty, value); }
        }

        /// <summary>
        /// Effects brush of the Chart Area Depedency Property
        /// </summary>
        public static readonly DependencyProperty EffectsBrushProperty =
           DependencyProperty.Register("EffectsBrush", typeof(Brush), typeof(ChartArea), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        /// <summary>
        /// Gets or sets the Effects brush for chart area
        /// </summary>
        public Brush EffectsBrush
        {
            get { return (Brush)GetValue(EffectsBrushProperty); }
            set { SetValue(EffectsBrushProperty, value); }
        }

        internal static readonly DependencyProperty seriescanvaswidthProperty =
    DependencyProperty.Register("seriescanvaswidth", typeof(double), typeof(ChartArea),
    new PropertyMetadata(0d));
        internal double seriescanvaswidth
        {
            get { return (double)GetValue(seriescanvaswidthProperty); }
            set { SetValue(seriescanvaswidthProperty, value); }
        }

        internal static readonly DependencyProperty seriescanvasheightProperty =
            DependencyProperty.Register("seriescanvasheight", typeof(double), typeof(ChartArea),
            new PropertyMetadata(0d));
        internal double seriescanvasheight
        {
            get { return (double)GetValue(seriescanvasheightProperty); }
            set { SetValue(seriescanvasheightProperty, value); }
        }
        /// <summary>
        /// Header of the Chart Area Depedency Property
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(ChartArea), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the Header for chart area
        /// </summary>
        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Footer of the Chart Area Depedency Property
        /// </summary>
        public static readonly DependencyProperty FooterProperty =
            DependencyProperty.Register("Footer", typeof(object), typeof(ChartArea), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the Footer for chart area
        /// </summary>
        public object Footer
        {
            get { return (object)GetValue(FooterProperty); }
            set { SetValue(FooterProperty, value); }
        }

        /// <summary>
        /// HeaderTemplate of the Chart Area Depedency Property
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(ChartArea), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the HeaderTemplate for chart area
        /// </summary>
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        /// <summary>
        /// CornerRadius of the Chart Area Depedency Property
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ChartArea), new PropertyMetadata(new CornerRadius(0)));

        /// <summary>
        /// Gets or sets the CornerRadius for chart area
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        /// <summary>
        /// Identifies the SideBySideSeriesPlacement dependency property.
        /// </summary>
        public static readonly DependencyProperty SideBySideSeriesPlacementProperty =
            DependencyProperty.Register("SideBySideSeriesPlacement", typeof(bool), typeof(ChartArea), new PropertyMetadata(true, new PropertyChangedCallback(OnSideBySideSeriesPlacementPropertyChanged)));

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
        /// Identifies the SplitterMaxResizeHeight dependency property.
        /// </summary>
        public static readonly DependencyProperty SplitterMaxResizeHeightProperty =
            DependencyProperty.Register("SplitterMaxResizeHeight", typeof(double), typeof(ChartArea), new PropertyMetadata(0d));

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="ChartSeries"/> area's max resize height.
        /// </summary>      
        /// <value>The SplitterMaxResizeHeight.</value>
        public double SplitterMaxResizeHeight
        {
            get { return (double)GetValue(SplitterMaxResizeHeightProperty); }
            set { SetValue(SplitterMaxResizeHeightProperty, value); }
        }

        /// <summary>
        /// Identifies the SplitterMinResizeHeight dependency property.
        /// </summary>
        public static readonly DependencyProperty SplitterMinResizeHeightProperty =
            DependencyProperty.Register("SplitterMinResizeHeight", typeof(double), typeof(ChartArea), new PropertyMetadata(0d));

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="ChartSeries"/>  area's min resize height.
        /// <value>The SplitterMaxResizeHeight.</value>
        /// </summary>
        public double SplitterMinResizeHeight
        {
            get { return (double)GetValue(SplitterMinResizeHeightProperty); }
            set { SetValue(SplitterMinResizeHeightProperty, value); }
        }

        /// <summary>
        /// Identifies the IsBottomDraggable dependency property.
        /// </summary>
        internal static readonly DependencyProperty IsBottomDraggableProperty =
            DependencyProperty.Register("IsBottomDraggable", typeof(bool), typeof(ChartArea), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="ChartSeries"/>  area's min resize height.
        /// <value>The SplitterMaxResizeHeight.</value>
        /// </summary>
        internal bool IsBottomDraggable
        {
            get { return (bool)GetValue(IsBottomDraggableProperty); }
            set { SetValue(IsBottomDraggableProperty, value); }
        }

      
        /// <summary>
        /// Gets/sets the color model of area.
        /// </summary>
        /// <value>The color model.</value>
        /// <seealso cref="ChartStyleModel"/>
        public ChartStyleModel ColorModel
        {
            get { return (ChartStyleModel)GetValue(ColorModelProperty); }

            set { SetValue(ColorModelProperty, value); }
        }


        /// <summary>
        /// ColorPalette Dependency property for Chart Area.
        /// </summary>
        public static readonly DependencyProperty ColorModelProperty =
           DependencyProperty.Register("ColorModel", typeof(ChartStyleModel), typeof(ChartArea), new PropertyMetadata(null));

        /// <summary>
        /// Idenfities AreaTemplate Collection dependency property.
        /// </summary>
        public static readonly DependencyProperty AreaTemplatesProperty =
           DependencyProperty.Register("AreaTemplates", typeof(ChartAreaTemplates), typeof(ChartArea), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the AreaTemplates Depedency property.  Intented to use Internal Purpose
        /// </summary>
        /// <value>The ChartAreaTemplates.</value>
        internal ChartAreaTemplates AreaTemplates
        {
            get
            {
                return (ChartAreaTemplates)GetValue(AreaTemplatesProperty);
            }

            set
            {
                SetValue(AreaTemplatesProperty, value);
            }
        }

        /// <summary>
        /// Idenfities AreaType dependency property used to inticate the Axes type.
        /// </summary>
        public static readonly DependencyProperty AreaTypeProperty = 
            DependencyProperty.Register("AreaType", typeof(ChartAxesType), typeof(ChartArea), new PropertyMetadata(ChartAxesType.Null, new PropertyChangedCallback(OnAreaTypeChanged)));

        /// <summary>
        /// Gets or sets the AreaType Depedency property.  Intented to use Internal Purpose
        /// </summary>
        /// <value>The ChartAxesType.</value>
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
        /// Idenfities HorizontalScrollBarVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty =
            DependencyProperty.Register("HorizontalScrollBarVisibility", typeof(Visibility), typeof(ChartArea), new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Gets or sets the HorizontalScrollBarVisibility. This is dependency property.
        /// </summary>
        /// <value>The Visibility.</value>
        public Visibility HorizontalScrollBarVisibility
        {
            get
            {
                return (Visibility)GetValue(HorizontalScrollBarVisibilityProperty);
            }

            internal set
            {
                SetValue(HorizontalScrollBarVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Idenfities VerticalScrollBarVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty =
            DependencyProperty.Register("VerticalScrollBarVisibility", typeof(Visibility), typeof(ChartArea), new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Gets or sets the VerticalScrollBarVisibility. This is dependency property.
        /// </summary>
        /// <value>The Visibility.</value>
        public Visibility VerticalScrollBarVisibility
        {
            get
            {
                return (Visibility)GetValue(VerticalScrollBarVisibilityProperty);
            }

            internal set
            {
                SetValue(VerticalScrollBarVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Idenfities IsZoomAllAxes dependency property.
        /// </summary>
        public static readonly DependencyProperty IsZoomAllAxesProperty =
            DependencyProperty.Register("IsZoomAllAxes", typeof(bool), typeof(ChartArea), new PropertyMetadata(false, new PropertyChangedCallback(OnIsZoomAllAxesChanged)));

        /// <summary>
        /// Gets or sets a value indicating whether IsZoomAllAxes. This is dependency property.
        /// </summary>
        /// <value>The Bool Type.</value>
        public bool IsZoomAllAxes
        {
            get
            {
                return (bool)GetValue(IsZoomAllAxesProperty);
            }

            set
            {
                SetValue(IsZoomAllAxesProperty, value);
            }
        }

        /// <summary>
        /// Idenfities SplitterVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty SplitterVisibilityProperty =
            DependencyProperty.Register("SplitterVisibility", typeof(SplitterBarVisibility), typeof(ChartArea), new PropertyMetadata(SplitterBarVisibility.Hide,  new PropertyChangedCallback(OnSplitterVisibilityChanged)));

        /// <summary>
        /// Gets or sets a value indicating whether SplitterVisibility. This is dependency property.
        /// </summary>
        /// <value>The Visibility Type.</value>
        public SplitterBarVisibility SplitterVisibility
        {
            get
            {
                return (SplitterBarVisibility)GetValue(SplitterVisibilityProperty);
            }

            set
            {
                SetValue(SplitterVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Idenfities InternalSplitterVisibility dependency property.
        /// </summary>
        internal static readonly DependencyProperty InternalSplitterVisibilityProperty =
            DependencyProperty.Register("InternalSplitterVisibility", typeof(Visibility), typeof(ChartArea), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Gets or sets a value indicating whether InternalSplitterVisibility. This is dependency property.
        /// </summary>
        /// <value>The Visibility Type.</value>
        internal Visibility InternalSplitterVisibility
        {
            get
            {
                return (Visibility)GetValue(InternalSplitterVisibilityProperty);
            }

            set
            {
                SetValue(InternalSplitterVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Idenfities SplitterWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty SplitterWidthProperty =
            DependencyProperty.Register("SplitterWidth", typeof(double), typeof(ChartArea), new PropertyMetadata(4.0));

        /// <summary>
        /// Gets or sets a value indicating whether SplitterWidth. This is dependency property.
        /// </summary>
        /// <value>The double Type.</value>
        public double SplitterWidth
        {
            get
            {
                return (double)GetValue(SplitterWidthProperty);
            }

            set
            {
                SetValue(SplitterWidthProperty, value);
            }
        }

        /// <summary>
        /// Idenfities SplitterBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty SplitterBrushProperty =
            DependencyProperty.Register("SplitterBrush", typeof(SolidColorBrush), typeof(ChartArea), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        /// <summary>
        /// Gets or sets a value indicating whether SplitterBrush. This is dependency property.
        /// </summary>
        /// <value>The SolidColorBrush Type.</value>
        public SolidColorBrush SplitterBrush
        {
            get
            {
                return (SolidColorBrush)GetValue(SplitterBrushProperty);
            }

            set
            {
                SetValue(SplitterBrushProperty, value);
            }
        }
        /// <summary>
        /// Identifies the WatermarkType dependency property.
        /// </summary>
        public static readonly DependencyProperty WatermarkTypeProperty =
            DependencyProperty.Register("WatermarkType", typeof(WatermarkTypes), typeof(ChartArea), new PropertyMetadata(WatermarkTypes.Text, new PropertyChangedCallback(OnWatermarkChanged)));
        /// <summary>
        /// Get or Set WatermarkTypeProperty
        /// </summary>
        public WatermarkTypes WatermarkType
        {
            get { return (WatermarkTypes)GetValue(WatermarkTypeProperty); }
            set { SetValue(WatermarkTypeProperty, value); }
        }
        /// <summary>
        /// Identifies the WatermarkImageSource dependency property.
        /// </summary>
        public static readonly DependencyProperty WatermarkImageSourceProperty =
            DependencyProperty.Register("WatermarkImageSource", typeof(ImageSource), typeof(ChartArea), new PropertyMetadata(null, new PropertyChangedCallback(OnWatermarkChanged)));
        /// <summary>
        /// Get or Set WatermartImagesourceProperty
        /// </summary>
        public ImageSource WatermarkImageSource
        {
            get { return (ImageSource)GetValue(WatermarkImageSourceProperty); }
            set { SetValue(WatermarkImageSourceProperty, value); }
        }
        /// <summary>
        /// Identifies the WatermarkImageStretch dependency property.
        /// </summary>
        public static readonly DependencyProperty WatermarkImageStretchProperty =
            DependencyProperty.Register("WatermarkImageStretch", typeof(Stretch), typeof(ChartArea), new PropertyMetadata(Stretch.None, new PropertyChangedCallback(OnWatermarkImageStretchChanged)));
        /// <summary>
        /// Get or Set WatermarkImagestrechProperty
        /// </summary>
        public Stretch WatermarkImageStretch
        {
            get { return (Stretch)GetValue(WatermarkImageStretchProperty); }
            set { SetValue(WatermarkImageStretchProperty, value); }
        }
        /// <summary>
        /// Identifies the WaterText dependency property.
        /// </summary>
        public static readonly DependencyProperty WatermarkTextProperty =
            DependencyProperty.Register("WatermarkText", typeof(string), typeof(ChartArea), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnWatermarkChanged)));
        /// <summary>
        /// Get or Set WatermarkTextProperty
        /// </summary>
        public string WatermarkText
        {
            get { return (string)GetValue(WatermarkTextProperty); }
            set { SetValue(WatermarkTextProperty, value); }
        }
        /// <summary>
        /// Identifies the WatermarkAlignmentX dependency property.
        /// </summary>
        public static readonly DependencyProperty WatermarkAlignmentXProperty =
           DependencyProperty.Register("WatermarkAlignmentX", typeof(AlignmentX), typeof(ChartArea), new PropertyMetadata(AlignmentX.Center, new PropertyChangedCallback(OnWatermarkAlignmentXChanged)));
        /// <summary>
        /// Get or Set WatermarkAlignmentXProperty
        /// </summary>
        public AlignmentX WatermarkAlignmentX
        {
            get { return (AlignmentX)GetValue(WatermarkAlignmentXProperty); }
            set { SetValue(WatermarkAlignmentXProperty, value); }
        }
        /// <summary>
        /// Identifies the WatermarkAlignmentY dependency property.
        /// </summary>
        public static readonly DependencyProperty WatermarkAlignmentYProperty =
           DependencyProperty.Register("WatermarkAlignmentY", typeof(AlignmentY), typeof(ChartArea), new PropertyMetadata(AlignmentY.Center, new PropertyChangedCallback(OnWatermarkAlignmentYChanged)));
        /// <summary>
        /// Get or Set WatermarkAlignmentYProperty
        /// </summary>
        public AlignmentY WatermarkAlignmentY
        {
            get { return (AlignmentY)GetValue(WatermarkAlignmentYProperty); }
            set { SetValue(WatermarkAlignmentYProperty, value); }
        }
        /// <summary>
        /// Identifies the WatermarkOpacity dependency property.
        /// </summary>
        public static readonly DependencyProperty WatermarkOpacityProperty =
           DependencyProperty.Register("WatermarkOpacity", typeof(double), typeof(ChartArea), new PropertyMetadata(0.3d, new PropertyChangedCallback(OnWatermarkOpacityChanged)));
        /// <summary>
        /// Get or Set WatermarkOpacity
        /// </summary>
        public double WatermarkOpacity
        {
            get { return (double)GetValue(WatermarkOpacityProperty); }
            set { SetValue(WatermarkOpacityProperty, value); }
        }
        /// <summary>
        /// Identifies the WatermarkRotationAngle dependency property.
        /// </summary>
        public static readonly DependencyProperty WatermarkRotationAngleProperty =
          DependencyProperty.Register("WatermarkRotationAngle", typeof(double), typeof(ChartArea), new PropertyMetadata(0d, new PropertyChangedCallback(OnWatermarkRotationAngleChanged)));
        /// <summary>
        /// Get or Set WatermarkRotationAngle property
        /// </summary>
        public double WatermarkRotationAngle
        {
            get { return (double)GetValue(WatermarkRotationAngleProperty); }
            set { SetValue(WatermarkRotationAngleProperty, value); }
        }
        /// <summary>
        /// Identifies the WatermarkTextFontSize dependency property.
        /// </summary>
        public static readonly DependencyProperty WatermarkTextFontSizeProperty =
          DependencyProperty.Register("WatermarkTextFontSize", typeof(double), typeof(ChartArea), new PropertyMetadata(64d, new PropertyChangedCallback(OnWatermarkTextFontSizeChanged)));
        /// <summary>
        /// Get or Set WatermarkTextFontSizeProperty
        /// </summary>
        public double WatermarkTextFontSize
        {
            get { return (double)GetValue(WatermarkTextFontSizeProperty); }
            set { SetValue(WatermarkTextFontSizeProperty, value); }
        }
        /// <summary>
        /// Identifies the WatermarkTextFontFamily dependency property.
        /// </summary>
        public static readonly DependencyProperty WatermarkTextFontFamilyProperty =
          DependencyProperty.Register("WatermarkTextFontFamily", typeof(FontFamily), typeof(ChartArea), new PropertyMetadata(new FontFamily("Arial"), new PropertyChangedCallback(OnWatermarkTextFontFamilyChanged)));
        /// <summary>
        /// Get or Set WatermarkTextFontFamilyProperty
        /// </summary>
        public FontFamily WatermarkTextFontFamily
        {
            get { return (FontFamily)GetValue(WatermarkTextFontFamilyProperty); }
            set { SetValue(WatermarkTextFontFamilyProperty, value); }
        }
        /// <summary>
        /// Identifies the WatermarkTextFontWeight dependency property.
        /// </summary>
        public static readonly DependencyProperty WatermarkTextFontWeightProperty =
          DependencyProperty.Register("WatermarkTextFontWeight", typeof(FontWeight), typeof(ChartArea), new PropertyMetadata(FontWeights.Bold, new PropertyChangedCallback(OnWatermarkTextFontWeightChanged)));
        /// <summary>
        /// Get or Set WatermarkTextFontWeight Property
        /// </summary>
        public FontWeight WatermarkTextFontWeight
        {
            get { return (FontWeight)GetValue(WatermarkTextFontWeightProperty); }
            set { SetValue(WatermarkTextFontWeightProperty, value); }
        }
        /// <summary>
        /// Identifies the WatermarkTextFontColor dependency property.
        /// </summary>
        public static readonly DependencyProperty WatermarkTextFontColorProperty =
          DependencyProperty.Register("WatermarkTextFontColor", typeof(Brush), typeof(ChartArea), new PropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnWatermarkTextFontColorChanged)));
        /// <summary>
        /// Get or Set WatermarkFontColorProperty
        /// </summary>
        public Brush WatermarkTextFontColor
        {
            get { return (Brush)GetValue(WatermarkTextFontColorProperty); }
            set { SetValue(WatermarkTextFontColorProperty, value); }
        }

        /// <summary>
        /// Idenfities Series dependency property.
        /// </summary>
        public static readonly DependencyProperty AxesProperty =
            DependencyProperty.Register("Axes", typeof(AxesCollection), typeof(ChartArea), new PropertyMetadata(null));

        /// <summary>
        /// Idenfities AxesThickness dependency property.
        /// </summary>
        public static readonly DependencyProperty AxesThicknessProperty =
            DependencyProperty.Register("AxesThickness", typeof(Thickness), typeof(ChartArea), new PropertyMetadata(new Thickness(0)));

        /// <summary>
        /// Idenfities AxesThickness dependency property.
        /// </summary>
        public static readonly DependencyProperty ScrollBarAxesThicknessProperty =
            DependencyProperty.Register("ScrollBarAxesThickness", typeof(Thickness), typeof(ChartArea), new PropertyMetadata(new Thickness(0)));

        ChartLegend chartlegent;
        internal Grid grid;
        internal Canvas secaxi;
        /// <summary>
        /// Idenfities GridBackground dependency property.
        /// </summary>
        public static readonly DependencyProperty GridBackgroundProperty =
DependencyProperty.Register("GridBackground", typeof(Brush), typeof(ChartArea), new PropertyMetadata(new SolidColorBrush(Colors.Transparent), new PropertyChangedCallback(OnGridBackgroundChanged)));

        /// <summary>
        /// Idenfities Legends dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendsProperty =
                    DependencyProperty.Register("Legends", typeof(ChartLegend), typeof(ChartArea), new PropertyMetadata(null));

        internal int noofseries;
        internal Size axissize = new Size();

        /// <summary>
        /// Idenfities PrimaryAxis dependency property.
        /// </summary>
        public static readonly DependencyProperty PrimaryAxisProperty =
            DependencyProperty.Register("PrimaryAxis", typeof(ChartAxis), typeof(ChartArea), new PropertyMetadata(null));

        /// <summary>
        /// Idenfities SecondaryAxis dependency property.
        /// </summary>
        public static readonly DependencyProperty SecondaryAxisProperty =
            DependencyProperty.Register("SecondaryAxis", typeof(ChartAxis), typeof(ChartArea), new PropertyMetadata(null));

        /// <summary>
        /// Idenfities SecondaryAxis dependency property.
        /// </summary>
        public static readonly DependencyProperty BreaksProperty =
            DependencyProperty.Register("Breaks", typeof(ChartScaleBreaksCollection), typeof(ChartArea), new PropertyMetadata(null));

        /// <summary>
        /// Idenfities Series dependency property.
        /// </summary>
        public static readonly DependencyProperty SeriesProperty =
            DependencyProperty.Register("Series", typeof(SeriesCollection), typeof(ChartArea), new PropertyMetadata(null, new PropertyChangedCallback(OnSeriesChanged)));
        
        /// <summary>
        /// Idenfities ContextMenuType dependency property.
        /// </summary>
        public static readonly DependencyProperty ContextMenuTypeProperty =
            DependencyProperty.Register("ContextMenuType", typeof(ContextMenuTypes), typeof(ChartArea), new PropertyMetadata(ContextMenuTypes.Default, OnContextMenuTypeChanged));
        /// <summary>
        /// Idenfities AddCustomContextMenuItems dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomContextMenuItemsProperty =
            DependencyProperty.Register("CustomContextMenuItems", typeof(ContextMenuItemsCollection), typeof(ChartArea), new PropertyMetadata(null));

        /// <summary>
        /// Set the Panning property
        /// </summary>
        public static readonly DependencyProperty IsPanningProperty =
          DependencyProperty.RegisterAttached("IsPanning", typeof(bool), typeof(ChartArea), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether this instance is panning.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is panning; otherwise, <c>false</c>.
        /// </value>
        public bool IsPanning
        {
            get { return (bool)GetValue(IsPanningProperty); }
            set { SetValue(IsPanningProperty, value); }
        }

        /// <summary>
        /// Identifies the X-Range on Panning.
        /// </summary>
        internal static readonly DependencyProperty PanningXRangeProperty =
         DependencyProperty.RegisterAttached("PanningXRange", typeof(DoubleRange), typeof(ChartArea), new PropertyMetadata(DoubleRange.Empty));

        /// <summary>
        /// Identifies the Y-Range on Panning.
        /// </summary>
        internal static readonly DependencyProperty PanningYRangeProperty =
            DependencyProperty.RegisterAttached("PanningYRange", typeof(DoubleRange), typeof(ChartArea), new PropertyMetadata(DoubleRange.Empty));


        /// <summary>
        /// Gets or sets the panning range_ X.
        /// </summary>
        /// <value>The panning range_ X.</value>
        internal DoubleRange PanningXRange
        {
            get { return (DoubleRange)GetValue(PanningXRangeProperty); }
            set { SetValue(PanningXRangeProperty, value); }
        }
        /// <summary>
        /// Gets or sets the panning range_ Y.
        /// </summary>
        /// <value>The panning range_ Y.</value>
        internal DoubleRange PanningYRange
        {
            get { return (DoubleRange)GetValue(PanningYRangeProperty); }
            set { SetValue(PanningYRangeProperty, value); }
        }
        /// <summary>
        /// Idenfities Series dependency property.
        /// </summary>
        public static readonly DependencyProperty InteractiveCursorsProperty =
            DependencyProperty.Register("InteractiveCursors", typeof(InteractiveCursorCollection), typeof(ChartArea), new PropertyMetadata(null));
        /// <summary>
        /// Gets or sets the interactive Cursor collection. This is dependency property.
        /// </summary>
        /// <value>The InteractiveCursorCollection.</value>
        public InteractiveCursorCollection InteractiveCursors
        {
            get
            {
                return (InteractiveCursorCollection)GetValue(InteractiveCursorsProperty);
            }

            set
            {
                SetValue(InteractiveCursorsProperty, value);
            }
        }
        /// <summary>
        /// Called when instance created for ChartArea
        /// </summary>
        public ChartArea()
        {
            DefaultStyleKey = typeof(ChartArea);
            this.PrimaryAxis = new ChartAxis() { Area = this };
            this.SecondaryAxis = new ChartAxis() { Area = this };
            Series = new SeriesCollection();
            Axes = new AxesCollection();
            Breaks = new ChartScaleBreaksCollection();
            CustomContextMenuItems = new ContextMenuItemsCollection();
            this.AreaTemplates = new ChartAreaTemplates();
            Series.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Series_CollectionChanged);
            this.Loaded += new RoutedEventHandler(Area_Loaded);
            this.ColorModel = new ChartStyleModel(this);
            this.InteractiveCursors = new InteractiveCursorCollection();
            this.InteractiveCursors.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(InteractiveCursorCollections_CollectionChanged);
        }

       internal void InteractiveCursorCollections_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            InteractiveCursorCollection obj = sender as InteractiveCursorCollection;

            foreach (InteractiveCursor ic in InteractiveCursors)
                ic.Area = this;
        }

        /// <summary>
        /// Gets or sets the Axes collection. This is dependency property.
        /// </summary>
        /// <value>The series.</value>
        public AxesCollection Axes
        {
            get
            {
                return (AxesCollection)GetValue(AxesProperty);
            }

            set 
            { 
                SetValue(AxesProperty, value); 
            }
        }

        /// <summary>
        /// get or set BreaksProperty
        /// </summary>
        public ChartScaleBreaksCollection Breaks
        {
            get
            {
                return (ChartScaleBreaksCollection)GetValue(BreaksProperty);
            }

            internal set
            {
                SetValue(BreaksProperty, value);
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
        /// Gets or sets the axes thickness.
        /// </summary>
        /// <value>The axes thickness.</value>
        internal Thickness ScrollBarAxesThickness
        {
            get
            {
                return (Thickness)GetValue(ScrollBarAxesThicknessProperty);
            }

            set
            {
                SetValue(ScrollBarAxesThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the GridBackground.
        /// </summary>
        /// <value>The GridBackground Brush.</value>
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
        /// Gets or sets the Legends.
        /// </summary>
        /// <value>The Legends.</value>
        public ChartLegend Legends
        {
            get
            {
                return (ChartLegend)GetValue(LegendsProperty);
            }

            set
            {
                SetValue(LegendsProperty, value);
            }
        }

        /// <summary>
        /// Gets the PreferredHeight.
        /// </summary>
        internal double PreferredHeight
        {
            get
            {
                if (grid != null)
                {
                    return 500;
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets the PreferredWidth.
        /// </summary>
        internal double PreferredWidth
        {
            get
            {
                if (grid != null)
                {
                    return 1000;
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets or sets the PrimaryAxis.
        /// </summary>
        /// <value>The PrimaryAxis.</value>
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
        /// Gets the PrimarySeries.
        /// </summary>
        internal ChartSeries PrimarySeries
        {
            get
            {
                return Series.Count > 0 ? Series[0] : null;
            }
        }

        /// <summary>
        /// Gets or sets the SecondaryAxis.
        /// </summary>
        /// <value>The SecondaryAxis.</value>
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
        /// Gets or sets the series collection. This is dependency property.
        /// </summary>
        /// <value>The series.</value>
        public SeriesCollection Series
        {
            get
            {
                return (SeriesCollection)GetValue(SeriesProperty);
            }

            set
            {
                SetValue(SeriesProperty, value);
            }
        }


        /// <summary>
        /// Get or Set IsContextMenuEnabledProperty
        /// </summary>
        public bool IsContextMenuEnabled
        {
            get { return (bool)GetValue(IsContextMenuEnabledProperty); }
            set { SetValue(IsContextMenuEnabledProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for IsContextMenuEnabled.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsContextMenuEnabledProperty =
            DependencyProperty.Register("IsContextMenuEnabled", typeof(bool), typeof(ChartArea), new PropertyMetadata(false, OnIsContextMenuEnabledChanged));

        private static void OnIsContextMenuEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if ((bool)args.NewValue)
            {
                (d as ChartArea).ChartAreaContextMenu.Visibility = Visibility.Visible;
                (d as ChartArea).LoadContextMenu();
            }
            else 
            {
                (d as ChartArea).ChartAreaContextMenu.Visibility = Visibility.Collapsed; 
            }
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
        /// Gets or sets the LegendStyle value.
        /// </summary>
        /// <value>The LegendStyle.</value>
        public ObservableCollection<ContextMenuItemAdv> CustomContextMenuItems
        {
            get
            {
                return (ObservableCollection<ContextMenuItemAdv>)GetValue(CustomContextMenuItemsProperty);
            }

            internal set
            {
                SetValue(CustomContextMenuItemsProperty, value);
            }
        }

        internal bool isAreaLoaded
        {
            get;
            set;
        }

        internal bool isUpdateArea
        {
            get;
            set;
        }
        /// <summary>
        /// Idenfities RangeSelectionTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty RangeSelectionTemplateProperty =
            DependencyProperty.Register("RangeSelectionTemplate", typeof(DataTemplate), typeof(ChartArea), new PropertyMetadata(null, new PropertyChangedCallback(OnRangeSelectionTemplateChanged)));

        /// <summary>
        /// Gets or sets the template for the RangeSelection. This is dependency property.
        /// </summary>
        /// <value>The DataTemplate Type.</value>
        public DataTemplate RangeSelectionTemplate
        {
            get
            {
                return (DataTemplate)GetValue(RangeSelectionTemplateProperty);
            }

            set
            {
                SetValue(RangeSelectionTemplateProperty, value);
            }
        }

        /// <summary>
        /// Idenfities RangeSelectionOnMouseOverTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty RangeSelectionOnMouseOverTemplateProperty =
            DependencyProperty.Register("RangeSelectionOnMouseOverTemplate", typeof(DataTemplate), typeof(ChartArea), new PropertyMetadata(null, new PropertyChangedCallback(OnRangeSelectionTemplateChanged)));

        /// <summary>
        /// Gets or sets the template for the RangeSelectionOnMouseOver. This is dependency property.
        /// </summary>
        /// <value>The DataTemplate Type.</value>
        public DataTemplate RangeSelectionOnMouseOverTemplate
        {
            get
            {
                return (DataTemplate)GetValue(RangeSelectionOnMouseOverTemplateProperty);
            }

            set
            {
                SetValue(RangeSelectionOnMouseOverTemplateProperty, value);
            }
        }

        /// <summary>
        /// Idenfities RangeSelectionHeight dependency property.
        /// </summary>
        internal static readonly DependencyProperty RangeSelectionHeightProperty =
            DependencyProperty.Register("RangeSelectionHeight", typeof(double), typeof(ChartArea), new PropertyMetadata(0d));

        /// <summary>
        /// Gets or sets the height for the RangeSelection. This is dependency property.
        /// </summary>
        /// <value>The double Type.</value>
        internal double RangeSelectionHeight
        {
            get
            {
                return (double)GetValue(RangeSelectionHeightProperty);
            }

            set
            {
                SetValue(RangeSelectionHeightProperty, value);
            }
        }

        /// <summary>
        /// Idenfities RangeSelectionWidth dependency property.
        /// </summary>
        internal static readonly DependencyProperty RangeSelectionWidthProperty =
            DependencyProperty.Register("RangeSelectionWidth", typeof(double), typeof(ChartArea), new PropertyMetadata(0d));

        /// <summary>
        /// Gets or sets the width for the RangeSelection. This is dependency property.
        /// </summary>
        /// <value>The double Type.</value>
        internal double RangeSelectionWidth
        {
            get
            {
                return (double)GetValue(RangeSelectionWidthProperty);
            }

            set
            {
                SetValue(RangeSelectionWidthProperty, value);
            }
        }

        /// <summary>
        /// Idenfities RangeSelectionMouseOverHeight dependency property.
        /// </summary>
        internal static readonly DependencyProperty RangeSelectionMouseOverHeightProperty =
            DependencyProperty.Register("RangeSelectionMouseOverHeight", typeof(double), typeof(ChartArea), new PropertyMetadata(0d));

        /// <summary>
        /// Gets or sets the height for the RangeSelectionMouseOverHeight. This is dependency property.
        /// </summary>
        /// <value>The double Type.</value>
        internal double RangeSelectionMouseOverHeight
        {
            get
            {
                return (double)GetValue(RangeSelectionMouseOverHeightProperty);
            }

            set
            {
                SetValue(RangeSelectionMouseOverHeightProperty, value);
            }
        }

        /// <summary>
        /// Idenfities RangeSelectionMouseOverWidth dependency property.
        /// </summary>
        internal static readonly DependencyProperty RangeSelectionMouseOverWidthProperty =
            DependencyProperty.Register("RangeSelectionMouseOverWidth", typeof(double), typeof(ChartArea), new PropertyMetadata(0d));

        /// <summary>
        /// Gets or sets the width for the RangeSelectionMouseOverWidth. This is dependency property.
        /// </summary>
        /// <value>The double Type.</value>
        internal double RangeSelectionMouseOverWidth
        {
            get
            {
                return (double)GetValue(RangeSelectionMouseOverWidthProperty);
            }

            set
            {
                SetValue(RangeSelectionMouseOverWidthProperty, value);
            }
        }

        /// <summary>
        /// Idenfities RangeSelectionWidth dependency property.
        /// </summary>
        internal static readonly DependencyProperty CloseButtonVisibilityProperty =
            DependencyProperty.Register("CloseButtonVisibility", typeof(Visibility), typeof(ChartArea), new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Gets or sets the visibility for the close button of RangeSelection. This is dependency property.
        /// </summary>
        /// <value>The double Type.</value>
        internal Visibility CloseButtonVisibility
        {
            get
            {
                return (Visibility)GetValue(CloseButtonVisibilityProperty);
            }

            set
            {
                SetValue(CloseButtonVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Idenfities RangeSelectionMargin dependency property.
        /// </summary>
        internal static readonly DependencyProperty RangeSelectionMarginProperty =
            DependencyProperty.Register("RangeSelectionMargin", typeof(Thickness), typeof(ChartArea), new PropertyMetadata(new Thickness(0, 0, 0, 0)));

        /// <summary>
        /// Gets or sets the margin for the RangeSelection. This is dependency property.
        /// </summary>
        /// <value>The Thickness Type.</value>
        internal Thickness RangeSelectionMargin
        {
            get
            {
                return (Thickness)GetValue(RangeSelectionMarginProperty);
            }

            set
            {
                SetValue(RangeSelectionMarginProperty, value);
            }
        }

        /// <summary>
        /// Idenfities RangeSelectionMouseMoveMargin dependency property.
        /// </summary>
        internal static readonly DependencyProperty RangeSelectionMouseMoveMarginProperty =
            DependencyProperty.Register("RangeSelectionMouseMoveMargin", typeof(Thickness), typeof(ChartArea), new PropertyMetadata(new Thickness(0, 0, 0, 0)));

        /// <summary>
        /// Gets or sets the margin for the RangeSelectionMouseMoveMargin. This is dependency property.
        /// </summary>
        /// <value>The Thickness Type.</value>
        internal Thickness RangeSelectionMouseMoveMargin
        {
            get
            {
                return (Thickness)GetValue(RangeSelectionMouseMoveMarginProperty);
            }

            set
            {
                SetValue(RangeSelectionMouseMoveMarginProperty, value);
            }
        }

        /// <summary>
        /// Idenfities CloseButtonMargin dependency property.
        /// </summary>
        internal static readonly DependencyProperty CloseButtonMarginProperty =
            DependencyProperty.Register("CloseButtonMargin", typeof(Thickness), typeof(ChartArea), new PropertyMetadata(new Thickness(0, 0, 0, 0)));

        /// <summary>
        /// Gets or sets the margin for the CloseButton. This is dependency property.
        /// </summary>
        /// <value>The Thickness Type.</value>
        internal Thickness CloseButtonMargin
        {
            get
            {
                return (Thickness)GetValue(CloseButtonMarginProperty);
            }

            set
            {
                SetValue(CloseButtonMarginProperty, value);
            }
        }
        private static void OnRangeSelectionTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartArea area = d as ChartArea;
            area.LoadArea();
        }
        /// <summary>
        /// Area Loaded Event
        /// </summary>
        void Area_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Series.Count > 0)
            {
                Binding areaTypeBinding = new Binding();
                areaTypeBinding.Source = this.Series[0];
                areaTypeBinding.Path = new PropertyPath("Type");
                areaTypeBinding.Converter = new AreaTypeConverter();
                areaTypeBinding.Mode = System.Windows.Data.BindingMode.OneWay;
                areaTypeBinding.ConverterParameter = this;
                BindingOperations.SetBinding(this, ChartArea.AreaTypeProperty, areaTypeBinding);
            }
            else
            {
                this.AreaType = ChartAxesType.CartesianAxes;
            }

            if (this.IsZoomAllAxes == true)
            {
                for (int i = 0; i < this.Axes.Count; i++)
                {
                    this.Axes[i].EnableZooming = true;
                }
            }

            this.isAreaLoaded = true;
            this.m_isresize = true;
            this.isFirstTimeAnimate = false;
            
        }

        /// <summary>
        /// Generate Chart Legends.
        /// </summary>        
        internal void LoadLegends()
        {
            if (chartlegent != null && Legends != null)
            {
                if (chartlegent.IsSegmentsLegend && Legends.Items.Count == 0)
                    chartlegent.GenerateItems();
                switch (Legends.DockPosition)
                {
                    case ChartDock.Top:
                        break;
                    case ChartDock.Bottom:
                        chartlegent.Orientation = Orientation.Horizontal;
                        break;
                    case ChartDock.Left:
                    case ChartDock.Right:
                        chartlegent.Orientation = Orientation.Vertical;
                        break;
                    default:
                        chartlegent.Orientation = Orientation.Horizontal;
                        break;
                }
               
                chartlegent.UpdateLegends();
            }
            else if (chartlegent !=null)
            {
                chartlegent.Visibility = Visibility.Collapsed;
            }
        }
        /// <summary>
        /// Load context Menu Items - Default / Custom menu Items 
        /// </summary>
        private void LoadContextMenu()
        {
            if (this != null && this.IsContextMenuEnabled)
            {
                this.ChartAreaContextMenuItems.Clear();
                if (this.ContextMenuType == ContextMenuTypes.Default)
                {
                    AddDefaultContextMenuItems();
                }
                else if (this.ContextMenuType == ContextMenuTypes.Custom)
                {
                    AddCustomContextMenuItems();
                }
                else if (this.ContextMenuType == ContextMenuTypes.DefaultWithCustom)
                {
                    AddDefaultContextMenuItems();
                    AddCustomContextMenuItems();
                }
                this.ChartAreaContextMenu.ItemsSource = ChartAreaContextMenuItems;                
            }

        }

        private void AddDefaultContextMenuItems()
        {
            ContextMenuItemAdv ZoomingContextMenu = new ContextMenuItemAdv() { Header = "Zooming", Command = new ContextMenuCommand(ZoomingCommandMethod), Icon = new Image() { Source = new BitmapImage() {UriSource = new Uri("/Syncfusion.Chart.Silverlight;component/Resources/magnifier.png", UriKind.Relative)}}};
            ContextMenuItemAdv PaletteContextMenu = new ContextMenuItemAdv() { Header = "Palette", Icon = new Image() { Source = new BitmapImage() { UriSource = new Uri("/Syncfusion.Chart.Silverlight;component/Resources/pallet.png", UriKind.Relative) } } };
            List<string> pallettenames = new List<string>() { "Default", "DefaultAlpha", "EarthTone", "Analog", "Colorful" , "Nature" , "Pastel" , "Triad", "WarmCold", "Grayscale","Metro", "Palette1", 
                "Palette2", "Palette3" , "Palette4", "Palette5", "Palette6", "Palette7", "Palette8" };
            foreach(string palettename in pallettenames)
            {
                PaletteContextMenu.Items.Add( new ContextMenuItemAdv(){ Header= palettename, CommandParameter=palettename, Command = new ContextMenuCommand(PalletteCommandMethod)});
            }
            ContextMenuItemAdv StylesContextMenu = new ContextMenuItemAdv() { Header = "Styles", Icon = new Image() { Source = new BitmapImage() { UriSource = new Uri("/Syncfusion.Chart.Silverlight;component/Resources/style.png", UriKind.Relative) } } };
            List<string> stylesnames = new List<string>() {"GrayScale", "MixedFantacy", "BlueScale", "MaroonRed", "GreenScale", "MixedViolet", "CoolBlueScale", "ChocolateOrange", "GrayWithBorder", 
                "MixedWithBorder", "BlueWithBorder", "RedWithBorder", "GreenWithBorder", "VioletWithBorder", "CoolBlueWithBorder", "ChocolateWithBorder", "AlphaGray", "AlphaFantacy", "AlphaBlue",
                "AlphaRed", "AlphaGreen", "AlphaViolet", "AlphaCoolBlue", "AlphaOrange", "EnabledGray", "EnabledMixed", "EnabledBlue", "EnabledRed", "EnabledGreen", "EnabledViolet", "EnabledCoolBlue",
                "EnabledChocolate", "GrayScreen", "MixedScreen", "BlueScreen", "RedScreen", "GreenScreen", "VioletScreen", "CoolBlueScreen", "ChocolateScreen", "BlendGray", "MixedBlend", "BlueBlend", 
                "RedBlend", "GreenBlend", "VioletBlend", "CoolBlueBlend", "ChocolateBlend", "Default", "Blend", "Office2003", "Office2007Blue", "Office2007Black", "Office2007Silver","Metro","VS2010"};

            foreach (string stylesname in stylesnames)
            {
                StylesContextMenu.Items.Add(new ContextMenuItemAdv() { Header = stylesname, CommandParameter = stylesname, Command = new ContextMenuCommand(StyleCommandMethod) });
            }            
            ContextMenuItemAdv SeriesMenu = new ContextMenuItemAdv() { Header = "Series", Icon = new Image() { Source = new BitmapImage() { UriSource = new Uri("/Syncfusion.Chart.Silverlight;component/Resources/series.png", UriKind.Relative) } } };            
            foreach (ChartSeries series in this.Series)
            {
                int seriesindex = this.Series.IndexOf(series);
                ContextMenuItemAdv seriesmenu = new ContextMenuItemAdv() { Header = series.Label == null ? "Series" + seriesindex : series.Label};
                List<string> seriestypes = new List<string>() { "Column", "Line", "Scatter", "Bar", "StackingColumn", "StackingBar", "Area", "StackingArea","StackingArea100", "FastLine", "Pie", "Doughnut", "Bubble",
                    "HiLo", "HiLoOpenClose", "Gantt", "BoxAndWhisker", "Pyramid", "Candle", "RangeColumn", "RangeArea", "Tornado", "Funnel", "StepLine", "StepArea", "StackingColumn100", 
                    "StackingBar100", "Spline", "SplineArea", "RotatedSpline", "Renko", "ThreeLineBreak", "Kagi", "PointAndFigure", "Radar", "Polar", "Histogram", "FastScatter", "FastColumn" };
                foreach (string seriestype in seriestypes)
                {
                    seriesmenu.Items.Add(new ContextMenuItemAdv() { Header = seriestype, CommandParameter = new List<object>() { seriesindex, seriestype }, Command = new ContextMenuCommand(SetSeriesCommandMethod) });
                }
                SeriesMenu.Items.Add(seriesmenu);
            }

            ChartAreaContextMenuItems.Add(ZoomingContextMenu);
            ChartAreaContextMenuItems.Add(SeriesMenu);
            ChartAreaContextMenuItems.Add(PaletteContextMenu);
            ChartAreaContextMenuItems.Add(StylesContextMenu);
        }

        private void AddCustomContextMenuItems()
        {
            foreach (ContextMenuItemAdv menuItem in this.CustomContextMenuItems)
            {
                ChartAreaContextMenuItems.Add(menuItem);
            }
        }

        internal bool IsResizing = false;
        private bool m_panning = false;

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event. </param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            
            base.OnMouseLeftButtonDown(e);
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseMove"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event. </param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            
            base.OnMouseMove(e);
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseLeftButtonUp"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event. </param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {            
            base.OnMouseLeftButtonUp(e);
        }


        /// <summary>
        /// Arrange Chart Areas
        /// </summary>
        /// <returns>
        /// Size of Area
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            return base.ArrangeOverride(finalSize);
        }
        /// <summary>
        /// return Bool value based upon the ChartType
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        protected bool IsSideBySide(ChartTypes types)
        {
            return (types == ChartTypes.StackingBar || types == ChartTypes.StackingColumn || types == ChartTypes.Column || types == ChartTypes.Bar || types == ChartTypes.HiLo || types == ChartTypes.HiLoOpenClose ||
                types == ChartTypes.Gantt || types == ChartTypes.BoxAndWhisker || types == ChartTypes.Candle || types == ChartTypes.RangeColumn) && this.SideBySideSeriesPlacement == true;
        }
        /// <summary>
        /// MOlapAxisThickness Thickness type variable declaration
        /// </summary>
        protected Thickness MOlapAxisThickness;
        /// <summary>
        /// Get or Set OlapAxisThickness property
        /// </summary>
        protected internal Thickness OlapAxisThickness
        {
            get
            {
                return MOlapAxisThickness;
            }
            set
            {
                MOlapAxisThickness = value;
            }
        }

        internal double MinDataInterval
        {
            get;
            set;
        }

        double GetDataInterval()
        {
            double result = double.MaxValue;
            foreach (ChartSeries series in this.Series)
            {
                for (int i = 1; i <= 5; i++)
                {
                    if (series.Data.Count > i)
                    {
                        result = Math.Min(result, Math.Abs(series.Data[i].X - series.Data[i - 1].X));
                    }
                }
            }

            this.MinDataInterval = (result == double.MaxValue || result >= 1 ? 1 : result);
            return this.MinDataInterval;
        }

        /// <summary>
        /// Used to find the Column and Bar size
        /// </summary>
        /// <returns>
        /// DoubleRange value
        /// </returns>
        protected internal virtual DoubleRange GetSideBySideInfo(ChartSeries series)
        {
            double width = 1 - 0.2;
            double minWidth = GetDataInterval();
            int pos = -1;
            int all = 0;
            int RenkoCount = 0;
            int totalCount = 0;
            int histogramCount = 0;
            double div = 0;
            double start = 0;
            double end = 0;
            Dictionary<Type, int> stackedColumns = new Dictionary<Type, int>();
            int segcount = 0;
            Dictionary<Type, int> types = new Dictionary<Type, int>();
            foreach (ChartSeries ser in Series)
            {
                if (ser.Visibility == Visibility.Visible && this.IsSideBySide(ser.Type) == true && ser.Data != null)
                {
                    if (this.IsStackType(ser.Type))
                    {
                        if (!types.ContainsKey(ser.Type.GetType()))
                        {
                            all++;
                            types.Add(ser.Type.GetType(), all);
                        }
                        if (series == ser)
                        {
                            pos = types[ser.Type.GetType()];
                        }
                    }
                    else
                    {
                        all++;
                        segcount = Math.Max(segcount, ser.Data.Count);
                        if (series == ser)
                        {
                            pos = all;
                        }
                    }
                }
                if (ser.Type == ChartTypes.Renko)
                {
                    RenkoCount++;
                }
                if (ser.Type == ChartTypes.Histogram)
                {
                    histogramCount++;
                }
                totalCount++;
            }

            if (series.Type == ChartTypes.Renko)
            {
                if (all == 0)
                {
                    all = 1;
                    pos = 1;
                }
                minWidth = (minWidth == double.MaxValue) ? 1d : minWidth;
                div = minWidth * width / all;
                if (totalCount != RenkoCount)
                    start = (-1/RenkoCount)+1;
                else
                    start = div * (pos - 1);
                end = start + div;
            }
            else if (series.Type == ChartTypes.Histogram)
            {

                if (all == 0)
                {
                    all = 1;
                    pos = 1;
                }
                minWidth = (minWidth == double.MaxValue) ? 1d : minWidth;
                div = minWidth * width / all;
                if (totalCount != histogramCount)
                    start = -1 / histogramCount;
                else
                    start = div * (pos - 1);
                end = start + div;



            }
            else
            {
                if (all == 0)
                {
                    all = 1;
                    pos = 1;
                }
                minWidth = (minWidth == double.MaxValue) ? 1d : minWidth;
                div = minWidth * width / all;
                start = div * (pos - 1);
                end = start + div;

            }


            if (this.Host == Host.Chart)
            {
                return new DoubleRange(start, end);
            }
            else
            {
                return new DoubleRange(start - 0.5, end - 0.5);
            }
        }

        /// <summary>
        /// Find the given type of the chart is Stacked Type.
        /// </summary>
        /// <param name="type"></param>
        private bool IsStackType(ChartTypes type)
        {
            if (type == ChartTypes.StackingColumn || type == ChartTypes.StackingArea || type == ChartTypes.StackingBar || type == ChartTypes.StackingBar100 || type == ChartTypes.StackingColumn100 || type==ChartTypes.StackingArea100)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal ChartAxesType GetSeriesAxesType(ChartTypes types)
        {
            if (types == ChartTypes.Pie || types == ChartTypes.Doughnut || types==ChartTypes.Pyramid || types==ChartTypes.Funnel)
            {
                return ChartAxesType.None;
            }
            else if (types == ChartTypes.Radar)
            {
                if ((secaxi != null) && ChartRadarType.GetDrawType(this) != ChartPolarDrawType.Area)
                {
                    secaxi.Visibility = System.Windows.Visibility.Collapsed;
                }
                else if (secaxi != null)
                {
                    secaxi.Visibility = System.Windows.Visibility.Visible;
                }
                return ChartAxesType.RadarAxes;
            }
            else if (types == ChartTypes.Polar)
            {
                if ((secaxi != null) && ChartPolarType.GetDrawType(this) != ChartPolarDrawType.Area)
                {
                    secaxi.Visibility = System.Windows.Visibility.Collapsed;
                }
                else if (secaxi != null)
                {
                    secaxi.Visibility = System.Windows.Visibility.Visible;
                }
                return ChartAxesType.PolarAxes;
            }
            else
            {
                return ChartAxesType.CartesianAxes;
            }
        }

        /// <summary>
        /// Populate Chart from the scratch
        /// </summary>
        internal void LoadArea()
        {
            if (this.IsBeginInit)
            {
                return;
            }

            noofseries = Series.Count;
            if (this.PrimaryAxis == null)
            {
                this.PrimaryAxis = new ChartAxis();
                this.PrimaryAxis.Header = "";
            }

            if (this.SecondaryAxis == null)
            {
                this.SecondaryAxis = new ChartAxis();
                this.SecondaryAxis.Header = "";
            }

            this.PrimaryAxis.Orientation = Orientation.Horizontal;
            this.SecondaryAxis.Orientation = Orientation.Vertical;

            for (int i = 0; i < this.Axes.Count; i++)
            {
                this.Axes[i].Area = null;
            }

            this.Axes.Clear();
            this.Axes.Add(this.PrimaryAxis);
            this.Axes.Add(this.SecondaryAxis);


            if (this.Series.Count >= 1)
            {
                if (this.Series[0] == null)
                    return;
                this.firstseriestype = this.GetSeriesAxesType(this.Series[0].Type);
            }
           
            if (!(this is SyncChartAreas))
            {
                foreach (ChartSeries series in Series)
                {
                    ////If First Series Axes Type is None.  then no other series would display
                    series.isseriesvisible = true;
                    if (this.firstseriestype == ChartAxesType.None)
                    {
                        series.isseriesvisible = false;
                        this.Series[0].isseriesvisible = true;
                    }
                    else if (firstseriestype == ChartAxesType.RadarAxes && (this.GetSeriesAxesType(series.Type) != ChartAxesType.RadarAxes))
                    {
                        series.isseriesvisible = false;
                    }
                    else if (firstseriestype == ChartAxesType.PolarAxes && (this.GetSeriesAxesType(series.Type) != ChartAxesType.PolarAxes))
                    {
                        series.isseriesvisible = false;
                    }
                    else if (this.firstseriestype == ChartAxesType.CartesianAxes && (this.GetSeriesAxesType(series.Type) != ChartAxesType.CartesianAxes))
                    {
                        series.isseriesvisible = false;
                    }

                    ////&& series.Visibility==Visibility.Visible)
                    if (series.Data != null && series.IsRequiredYPoints())
                    {
                        series.Area = this;                        
                        series.CalculateSegments();
                    }
                }
            }
            if (this.ChartAreaParent != null)
            {
                this.ChartAreaParent.SetAreaProperties();
            }
            LoadLegends();
        }

        /// <summary>
        /// populate only series
        /// </summary>
        internal void LoadOnlySeries()
        {
            foreach (ChartSeries series in Series)
            {
                series.Area = this;
                series.CalculateSegments();
            }
        }

        /// <summary>
        /// Measure the Chart Area size
        /// </summary>
        /// <returns>
        /// The Size value</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.AreaType == ChartAxesType.None)
            {
                this.axissize = availableSize;
            }

            return base.MeasureOverride(availableSize);
        }

        internal ContentPresenter chartareaheader, legendcontent;
        internal ZoomingScrollBar HorizontalBar, VerticalBar;
        internal ZoomingToolKit ZoomTool;
        internal Grid seriesGrid;
        bool isMousemove = false, isMouseLbuttonDown = false, isMouseLeave=true, isactivatemousezoom=false;
        Rectangle zoomrectangle = new Rectangle();
        Point zoomRectStart = new Point(0, 0);
        Point zoomRectEnd = new Point(0, 0);
        internal ItemsControl InteractiveCursorItemsControl = null;
        internal ResizeBehavior rb;
        private ChartAxesGridLinesPanel axesGridLinePanel = null;
        #region revamp
        internal ChartAxesGridLinesPanel gridLinePanel = null;

        internal ItemsControl seriesItemControl = null;

        internal StripLinePanel stripLinePanel = null;

        internal SmallTickLinesPanel smallTickLinesPanel = null;

		private ChartAreaWatermarkControl watermarkControl = null;

        internal ChartScaleBreakPresenter ScaleBreakItemsControl = null;
        #endregion
        Border maingrid;
        internal Rectangle splittergrid;
        Button m_RangeSelectionClose = null;
        /// <summary>
        /// Invoke to render chart Area.
        /// </summary>
        public override void OnApplyTemplate()
        {
            #region revamp
            gridLinePanel = this.GetTemplateChild("gridLinesPanel") as ChartAxesGridLinesPanel;
            axesGridLinePanel = this.GetTemplateChild("gridLinesPanel") as ChartAxesGridLinesPanel;
            smallTickLinesPanel = this.GetTemplateChild("smalltickgridLinesPanel") as SmallTickLinesPanel;
            Grid rangeGrid = GetTemplateChild("rangeSelectionGrid") as Grid;
            if (rangeGrid != null && VisualTreeHelper.GetChildrenCount(rangeGrid) > 0)
            {
                DependencyObject obj = VisualTreeHelper.GetChild(rangeGrid, 0);
                m_RangeSelectionClose = obj as Button;
                if (m_RangeSelectionClose != null)
                {
                    m_RangeSelectionClose.Click += new RoutedEventHandler(m_RangeSelectionClose_Click);
                }
            }

            if (this.Series.Count > 0)
            {
                this.AreaType = this.GetSeriesAxesType(this.Series[0].Type);
                if (this.PrimaryAxis != null)
                {
                    switch (this.AreaType)
                    {
                        case ChartAxesType.CartesianAxes:
                            this.PrimaryAxis.ChartAxesProvider = new ChartCartesianAxesGenerator();
                            break;
                        case ChartAxesType.RadarAxes:
                            this.PrimaryAxis.ChartAxesProvider = new ChartRadarAxesGenerator();
                            break;
                        case ChartAxesType.PolarAxes:
                            this.PrimaryAxis.ChartAxesProvider = new ChartPolarAxesGenerator();
                            break;
                    }
                }
                if (this.SecondaryAxis != null)
                {
                    switch (this.AreaType)
                    {
                        case ChartAxesType.CartesianAxes:
                            this.SecondaryAxis.ChartAxesProvider = new ChartCartesianAxesGenerator();
                            break;
                        case ChartAxesType.RadarAxes:
                            this.SecondaryAxis.ChartAxesProvider = new ChartRadarAxesGenerator();
                            break;
                        case ChartAxesType.PolarAxes:
                            this.SecondaryAxis.ChartAxesProvider = new ChartPolarAxesGenerator();
                            break;
                    }
                }
            }

            #endregion

            
           

            maingrid = GetTemplateChild("MainBorder") as Border;

            ////Zooming Scrollbars
            HorizontalBar = GetTemplateChild("HorizontalBar") as ZoomingScrollBar;
            VerticalBar = GetTemplateChild("VerticalBar") as ZoomingScrollBar;

            ////Zooming Toolkit
            ZoomTool = GetTemplateChild("ZoomTool") as ZoomingToolKit;
                      
            AxesCollection ac = this.Axes;
            InteractiveCursorItemsControl = GetTemplateChild("interactiveCursor") as ItemsControl;
            if (this.ChartAreaParent != null)
            {
               

                 rb = new ResizeBehavior();
                rb.IsBottomDraggable = true;
                rb.IsBottomLeftDraggable = false;
                rb.IsBottomRightDraggable = false;
                rb.IsLeftDraggable = false;
                rb.IsRightDraggable = false;
                rb.IsTopDraggable = false;
                rb.IsTopLeftDraggable = false;
                rb.IsTopRightDraggable = false;
                rb.StayInParent = true;
                if (this.ChartAreaParent.Areas.IndexOf(this) != (this.ChartAreaParent.Areas.Count-1) && !this.isResizable)
                {
                    this.HorizontalBar.Visibility = Visibility.Collapsed;
                    //this.HorizontalBar.Height = 0d;
                    Binding Maxresizeht = new Binding();
                    Maxresizeht.Path = new PropertyPath("SplitterMaxResizeHeight");
                    Maxresizeht.Source = this;

                    Binding Minresizeht = new Binding();
                    Minresizeht.Path = new PropertyPath("SplitterMinResizeHeight");
                    Minresizeht.Source = this;

                    BindingOperations.SetBinding(rb, ResizeBehavior.MinHeightProperty, Minresizeht);
                    BindingOperations.SetBinding(rb, ResizeBehavior.MaxHeightProperty, Maxresizeht);
                    this.isResizable = true;
                    splittergrid = GetTemplateChild("splitter") as Rectangle;

                    if (this.SplitterVisibility == SplitterBarVisibility.Hide)
                    {
                        splittergrid.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    else
                    {
                        splittergrid.Visibility = System.Windows.Visibility.Visible;
                    }
                    this.splittergrid.MouseLeave += new MouseEventHandler(splittergrid_MouseLeave);
                    
                    rb.Attach(maingrid);
                }
                else if (this.ChartAreaParent.Areas.IndexOf(this) == (this.ChartAreaParent.Areas.Count - 1))
                {
                    foreach (ChartArea area in this.ChartAreaParent.Areas)
                    {
                        if (area != this && !area.isResizable)
                        {
                            Binding Maxresizeht = new Binding();
                            Maxresizeht.Path = new PropertyPath("SplitterMaxResizeHeight");
                            Maxresizeht.Source = area;

                            Binding Minresizeht = new Binding();
                            Minresizeht.Path = new PropertyPath("SplitterMinResizeHeight");
                            Minresizeht.Source = area;

                            BindingOperations.SetBinding(area.rb, ResizeBehavior.MinHeightProperty, Minresizeht);
                            BindingOperations.SetBinding(area.rb, ResizeBehavior.MaxHeightProperty, Maxresizeht);
                            area.isResizable = true;
                            area.rb = this.rb;
                            area.rb.Attach(area.maingrid);
 
                        }
 
                    }
                }
            }
            grid = GetTemplateChild("LayoutRoot") as Grid;
            if(grid!=null)
            grid.SizeChanged += new SizeChangedEventHandler(grid_SizeChanged);

            seriesItemControl = GetTemplateChild("ItemsControlSeries") as ItemsControl;
            stripLinePanel = GetTemplateChild("stripLinePanel") as StripLinePanel;
            if(stripLinePanel != null)
            stripLinePanel.Area = this;
            ScaleBreakItemsControl = GetTemplateChild("scaleBreakItemsControl") as ChartScaleBreakPresenter;
           
            watermarkControl = GetTemplateChild("watermarkControl") as ChartAreaWatermarkControl;

            secaxi = GetTemplateChild("secondaryaxis") as Canvas;
            legendcontent = GetTemplateChild("ChartLegends") as ContentPresenter;
            ////chartlegent = GetTemplateChild("ChartLegends") as ChartLegend;

            if (legendcontent != null && legendcontent.Content != null)
            {
                chartlegent = legendcontent.Content as ChartLegend;
            }
            chartareaheader = GetTemplateChild("ChartHeader") as ContentPresenter;
            
            
            ////For Mouse Drag Zoom Feature
            seriesGrid = GetTemplateChild("InternalCanvas") as Grid;
            //if(seriesGrid!=null)
            //    seriesGrid.SizeChanged += new SizeChangedEventHandler(seriesGrid_SizeChanged);
            this.splitcanvas = this.GetTemplateChild("splitter") as Canvas;
            zoomrectangle.Fill = new SolidColorBrush(Colors.White);
            zoomrectangle.Opacity = 0.75;
            zoomrectangle.Width = 0;
            zoomrectangle.Height = 0;
            Canvas.SetLeft(zoomrectangle, 0);
            Canvas.SetTop(zoomrectangle, 0);
            if (seriesGrid != null)
            {
                seriesGrid.MouseLeftButtonDown += new MouseButtonEventHandler(seriescanvas_MouseLeftButtonDown);
                seriesGrid.MouseLeftButtonUp += new MouseButtonEventHandler(seriescanvas_MouseLeftButtonUp);
                seriesGrid.MouseMove += new MouseEventHandler(seriescanvas_MouseMove);
                seriesGrid.MouseLeave += new MouseEventHandler(seriescanvas_MouseLeave);
            }

            SetZoomingScrollBarPosition();
            LoadContextMenu();
            ContextMenuAdvService.SetContextMenuAdv(this, this.ChartAreaContextMenu);
            this.CustomContextMenuItems.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(CustomContextMenuItems_CollectionChanged);
            base.OnApplyTemplate();
        }
        void m_RangeSelectionClose_Click(object sender, RoutedEventArgs e)
        {
            this.RangeSelectionWidth = 0d;
            this.RangeSelectionHeight = 0d;
            this.CloseButtonVisibility = System.Windows.Visibility.Collapsed;
            foreach (ChartAxis axis in this.Axes)
            {
                axis.SelectedRange = new DoubleRange();
            }
        }


        void CustomContextMenuItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            LoadContextMenu();
        }

        //void seriesGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    foreach (ChartSeries series in this.Series)
        //    {
        //        if (series.Presenter != null)
        //        {
        //            series.Presenter.update();
        //        }

        //        if (series.InteractiveCursor != null)
        //        {
        //            series.SetValuesForCursor();
        //        }
        //    }
        //}

        void grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.seriescanvaswidth = seriesGrid.ActualWidth;
            this.seriescanvasheight = seriesGrid.ActualHeight;
            int count = (from series in this.Series where this.m_isresize == true && (series.Segments.Count == 0 || series.Adornments.Count != 0) select series).Count<ChartSeries>();
            //this.isFirstTimeAnimate = true;
            if (this.m_isresize == true && (count != 0 || this.Series.Count == 0))
            {
                this.LoadArea();
                this.m_isresize = false;
            }
            else
            {
                foreach (ChartSeries series in this.Series)
                {
                    if (series.Presenter != null)
                    {
                        series.Presenter.update();
                    }

                    //if (series.InteractiveCursor != null)
                    //{
                    //    series.SetValuesForCursor();
                    //}

                    if(series.Indicators != null)
                    {                        
                       series.Presenter.LoadIndicators(series);                        
                    }
                }

                if (this.Series.Count > 0 && this.Series[this.Series.Count - 1].Presenter != null)
                {
                    this.Series[this.Series.Count - 1].Presenter.LoadAdornments(this.Series);
                }                
            }

            foreach (ChartAxis axis in this.Axes)
            {
                if (axis.EnableRangeSelection)
                {
                    axis.m_enableRangeSelection = true;
                    if (axis.Orientation == Orientation.Horizontal && !axis.SelectedRange.IsEmpty)
                    {
                        this.m_RangeSelectionStartPoint = new Point(this.ValueToPoint(axis, axis.SelectedRange.Start) + axis.Area.AxesThickness.Left + 5, 0);
                        this.m_RangeSelectionEndPoint = this.ValueToPoint(axis, axis.SelectedRange.End);
                        this.RangeSelection(new Point(this.m_RangeSelectionEndPoint + axis.Area.AxesThickness.Left + 5, this.m_RangeSelectionEndPoint + axis.Area.AxesThickness.Left + 5));
                    }
                    else
                    {
                        this.m_RangeSelectionStartPoint = new Point(0, this.ValueToPoint(axis, axis.SelectedRange.Start) + this.AxesThickness.Bottom - 5);
                        this.m_RangeSelectionEndPoint = this.ValueToPoint(axis, axis.SelectedRange.End);
                        this.RangeSelection(new Point(this.m_RangeSelectionEndPoint + this.AxesThickness.Bottom - 5, this.m_RangeSelectionEndPoint + this.AxesThickness.Bottom - 5));
                    }
                    axis.m_enableRangeSelection = false;
                }
            }
        }
        internal bool IsZoomInCommand = false;
        internal bool isSwitchzoom=false;
        internal bool istoolkitvisible = false, isinternalinitialize=false;
        internal Visibility toolkitvisible = Visibility.Collapsed;

        /// <summary>
        /// Used to Enable or Disable Zooming operations. Such as Zoom-In, Zoom-Out, Zoom-Reset
        /// </summary>
        public void SwitchZooming()
        {
            if (this is SyncChartAreas)
            {
                if ((this as SyncChartAreas).Areas.Count > 0)
                {
                    if ((this as SyncChartAreas).Areas[0].isSwitchzoom == true)
                    {
                        (this as SyncChartAreas).Areas[0].isinternalinitialize = true;
                        ZoomingToolKit.SetZoomingToolkitVisibility((this as SyncChartAreas).Areas[0], Visibility.Collapsed);
                        (this as SyncChartAreas).Areas[0].isinternalinitialize = false;
                        (this as SyncChartAreas).Areas[0].isSwitchzoom = false;
                    }
                    else
                    {
                        (this as SyncChartAreas).Areas[0].isSwitchzoom = true;
                        ZoomingToolKit.SetZoomingToolkitVisibility((this as SyncChartAreas).Areas[0], this.toolkitvisible);
                    }
                    (this as SyncChartAreas).Areas[0].EnableMouseDragZooming = false;
                }
            }
            else if(this.ChartAreaParent == null)
            {
                if (this.isSwitchzoom == true)
                {
                    this.isinternalinitialize = true;
                    ZoomingToolKit.SetZoomingToolkitVisibility(this, Visibility.Collapsed);
                    this.isinternalinitialize = false;
                    this.isSwitchzoom = false;
                }
                else
                {
                    this.isSwitchzoom = true;
                    ZoomingToolKit.SetZoomingToolkitVisibility(this, this.toolkitvisible);
                }
            }
        }

        /// <summary>
        /// To Perform Zoomin operation
        /// </summary>
        /// <remarks>
        /// SwitchZooming must be invoked to enable zooming operations
        /// </remarks>
        /// <seealso cref="SwitchZooming"/>
        public void ZoomInCommand()
        {
            if (this.isSwitchzoom == true)
            {
                if (this.ChartAreaParent != null)
                {
                    for (int index = 0; index < this.ChartAreaParent.Areas.Count; index++)
                    {
                        if (this.ChartAreaParent.Areas[index].PrimaryAxis.EnableZooming == true)
                        {
                            if (this.ChartAreaParent.Areas[index].PrimaryAxis.ZoomFactor / 2.0 >= 0.001)
                            {
                                this.ChartAreaParent.Areas[index].PrimaryAxis.ZoomFactor /= 2.0;
                            }
                            else
                            {
                                this.ChartAreaParent.Areas[index].PrimaryAxis.ZoomFactor = 0.001;
                            }
                        }
                       
                    }
                }
                else
                {
                    for (int index = 0; index < this.Axes.Count; index++)
                    {
                        if (Axes[index].EnableZooming == true)
                        {
                            if (this.Axes[index].ZoomFactor / 2.0 >= 0.001)
                            {
                                this.Axes[index].ZoomFactor /= 2.0;
                            }
                            else
                            {
                                this.Axes[index].ZoomFactor = 0.001;
                            }
                        }
                    }
                }
                if (this.ChartAreaParent != null)
                {
                    foreach (ChartSeries ser in this.ChartAreaParent.Series)
                    {
                        ser.InvalidateMeasure();
                        ser.InvalidateArrange();
                    }
                }
            }
        }

        /// <summary>
        /// To Perform Disable Zooming operation
        /// </summary>
        public void ZoomCancelCommand()
        {
            this.isSwitchzoom = false;
        }

        /// <summary>
        /// To Perform Zoomout operation
        /// </summary>
        /// <remarks>
        /// SwitchZooming must be invoked to enable zooming operations
        /// </remarks>
        /// <seealso cref="SwitchZooming"/>
        public void ZoomOutCommand()
        {
            if (this.isSwitchzoom == true)
            {
                if (this.ChartAreaParent != null)
                {
                    for (int index = 0; index < this.ChartAreaParent.Areas.Count; index++)
                    {
                        if (this.ChartAreaParent.Areas[index].PrimaryAxis.EnableZooming == true)
                        {
                            if (this.ChartAreaParent.Areas[index].PrimaryAxis.ZoomFactor * 2.0 <= 1)
                            {
                                this.ChartAreaParent.Areas[index].PrimaryAxis.ZoomFactor *= 2.0;
                            }
                            else
                            {
                                this.ChartAreaParent.Areas[index].PrimaryAxis.ZoomFactor = 1;
                                this.ChartAreaParent.Areas[index].HorizontalBar.Visibility = Visibility.Collapsed;
                            }
                        }
                       
                    }
                   
                }
                else
                {
                    for (int index = 0; index < this.Axes.Count; index++)
                    {
                        if (Axes[index].EnableZooming == true)
                        {
                            if (this.Axes[index].ZoomFactor * 2.0 <= 1.0)
                            {
                                this.Axes[index].ZoomFactor *= 2.0;
                            }
                            else
                            {
                                this.Axes[index].ZoomFactor = 1.0;
                            }
                        }
                    }
                }
                if (this.ChartAreaParent != null)
                {
                    foreach (ChartSeries ser in this.ChartAreaParent.Series)
                    {
                        ser.InvalidateMeasure();
                        ser.InvalidateArrange();
                    }
                }
            }
        }

        /// <summary>
        /// To Perform Zoom-Reset operation
        /// </summary>
        /// <remarks>
        /// SwitchZooming must be invoked to enable zooming operations
        /// </remarks>
        /// <seealso cref="SwitchZooming"/>
        public void ZoomResetCommand()
        {
            if (this.isSwitchzoom == true)
            {
                for (int index = 0; index < this.Axes.Count; index++)
                {
                    if (Axes[index].EnableZooming == true)
                    {
                        this.Axes[index].ZoomFactor = 1;
                    }
                }
                if (this.ChartAreaParent != null)
                {
                    foreach(ChartArea area in this.ChartAreaParent.Areas)
                    {
                        if(area != this)
                        area.PrimaryAxis.ZoomFactor = 1;
                    }
                    this.ChartAreaParent.Areas[this.ChartAreaParent.Areas.Count - 1].HorizontalBar.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// To Perform Zooming Scrollbar visibility status
        /// </summary>
        internal void IntializeZoomingScrollBarVisibility()
        {
            bool findhorizontal = false, findvertical = false;
            
            for (int i = 0; i < this.Axes.Count; i++)
            {
                if (this.Axes[i].Orientation == Orientation.Horizontal  && findhorizontal == false && this.Axes[i].ZoomFactor < 1.0 && this.Axes[i].EnableZooming)
                {
                    if (this.HorizontalBar != null)
                    {
                        this.HorizontalBar.Visibility = Visibility.Visible;
                        findhorizontal = true;
                    }
                }
                else if (this.Axes[i].Orientation == Orientation.Vertical && findvertical == false && this.Axes[i].ZoomFactor < 1.0 && this.Axes[i].EnableZooming)
                {
                    if (this.VerticalBar != null)
                    {
                        this.VerticalBar.Visibility = Visibility.Visible;
                        findvertical = true;
                    }
                }

                if (findhorizontal == true && findvertical == true)
                {
                    break;
                }
            }

            if (findhorizontal == false)
            {
                if (this.HorizontalBar != null)
                {
                    this.HorizontalBar.Visibility = Visibility.Collapsed;
                }
            }

            if (findvertical == false)
            {
                if (this.VerticalBar != null)
                {
                    this.VerticalBar.Visibility = Visibility.Collapsed;
                }
            }

            if (this.ZoomTool != null)
            {
                int i;
                for (i = 0; i < this.Axes.Count; i++)
                {
                    if (this.Axes[i].EnableZooming == true)
                    {
                        this.isZoomactivated = true;
                        break;
                    }
                }

                if (i == this.Axes.Count)
                {
                    this.isZoomactivated = false;
                }
            }
        }

        /// <summary>
        /// Executes when mouse leave from chart series present area
        /// </summary>
        void seriescanvas_MouseLeave(object sender, MouseEventArgs e)
        {
            isMouseLeave = true;
            if (isSwitchzoom == false && this.AreaType == ChartAxesType.CartesianAxes)
            {
                isMouseLbuttonDown = false;
            }
            Point pt = e.GetPosition(this.seriesGrid);
            foreach (ChartAxis axis in this.Axes)
            {
                if (axis.m_enableRangeSelection == true)
                {
                    axis.m_enableRangeSelection = false;
                    if (this.RangeSelectionWidth > 2)
                    {
                        this.CloseButtonVisibility = Visibility.Visible;
                    }
                    this.m_RangeSelectionStartPoint = new Point();
                    this.m_RangeSelectionEndPoint = 0d;
                }
            }
            ////isMousemove = false;
            ////seriescanvas.Children.Remove(zoomrectangle);
            ////zoomrectangle = null;
            ////isMouseLbuttonDown = false;
            ////isactivatemousezoom = false;
        }
        internal bool IsRangeSetOnZooming = false;
        internal void RangeSelectionForBothAxis(Point pt)
        {
            axisCollection = new ObservableCollection<ChartAxis>();
            foreach (ChartAxis axis in this.Axes)
            {
                if (axis.EnableRangeSelection)
                {
                    axisCollection.Add(axis);
                }
            }
            double marginLeft = pt.X > this.m_RangeSelectionStartPoint.X ? (this.m_RangeSelectionStartPoint.X - this.AxesThickness.Left - 5) : (pt.X - this.AxesThickness.Left - 5);
            double marginTop = pt.Y > this.m_RangeSelectionStartPoint.Y ? (this.m_RangeSelectionStartPoint.Y + 5 - this.AxesThickness.Bottom) : (pt.Y + 5 - this.AxesThickness.Bottom);
            this.RangeSelectionMargin = new Thickness(marginLeft, marginTop, 0, 0);
            this.RangeSelectionWidth = Math.Abs(pt.X - this.m_RangeSelectionStartPoint.X);
            this.RangeSelectionHeight = Math.Abs(pt.Y - this.m_RangeSelectionStartPoint.Y);
            foreach (ChartAxis axis in axisCollection)
            {
                if (axis.Orientation == Orientation.Horizontal)
                {
                    double start = this.PointToValue(axis, new Point(marginLeft, 0));
                    double end = this.PointToValue(axis, new Point(marginLeft + this.RangeSelectionWidth, 0));
                    axis.SelectedRange = new DoubleRange(start, end);
                }
                else
                {
                    double end = this.PointToValue(axis, new Point(0, marginTop));
                    double start = this.PointToValue(axis, new Point(0, marginTop + this.RangeSelectionHeight));
                    axis.SelectedRange = new DoubleRange(start, end);
                }
            }
        }

        internal void RangeSelection(Point pt)
        {
            foreach (ChartAxis axis in this.Axes)
            {
                if (axis.m_enableRangeSelection)
                {
                    this.CloseButtonVisibility = System.Windows.Visibility.Visible;
                    if (axis.Orientation == Orientation.Horizontal)
                    {
                        this.m_RangeSelectionEndPoint = pt.X;
                        double marginLeft = this.m_RangeSelectionEndPoint > this.m_RangeSelectionStartPoint.X ? (this.m_RangeSelectionStartPoint.X - this.AxesThickness.Left - 5) : (this.m_RangeSelectionEndPoint - this.AxesThickness.Left - 5);

                        this.RangeSelectionMargin = new Thickness(marginLeft < 0 ? 0 : marginLeft, axis.GridLineStrokeThickness, 0, 0);
                        this.RangeSelectionHeight = this.ActualHeight;
                        this.RangeSelectionWidth = Math.Abs(this.m_RangeSelectionEndPoint - this.m_RangeSelectionStartPoint.X);
                        double start = this.PointToValue(axis, new Point(marginLeft, 0));
                        double end = this.PointToValue(axis, new Point(marginLeft + this.RangeSelectionWidth, 0));
                        axis.SelectedRange = new DoubleRange(start, end);
                    }
                    else
                    {
                        this.m_RangeSelectionEndPoint = pt.Y;
                        double marginTop = this.m_RangeSelectionEndPoint > this.m_RangeSelectionStartPoint.Y ? (this.m_RangeSelectionStartPoint.Y - this.AxesThickness.Bottom + 5) : (this.m_RangeSelectionEndPoint - this.AxesThickness.Bottom + 5);
                        //if (Math.Abs(this.m_RangeSelectionEndPoint - this.m_RangeSelectionStartPoint.Y) > 2)
                        {
                            this.RangeSelectionMargin = new Thickness(0, marginTop < 0 ? 0 : marginTop, axis.GridLineStrokeThickness, 0);
                            this.RangeSelectionHeight = Math.Abs(this.m_RangeSelectionEndPoint - this.m_RangeSelectionStartPoint.Y);
                            this.RangeSelectionWidth = this.seriesGrid.ActualWidth;
                            double end = this.PointToValue(axis, new Point(0, marginTop));
                            double start = this.PointToValue(axis, new Point(0, marginTop + this.RangeSelectionHeight));
                            axis.SelectedRange = new DoubleRange(start, end);
                        }
                    }
                }
            }
        }
        private ObservableCollection<ChartAxis> axisCollection = new ObservableCollection<ChartAxis>();
        private DoubleRange PrevPanningXRange = DoubleRange.Empty;
        private DoubleRange prevPanningYRange = DoubleRange.Empty;
        /// <summary>
        /// Executes when mouse move on chart series present area
        /// </summary>
        void seriescanvas_MouseMove(object sender, MouseEventArgs e)
        {
           
            bool IsPrimaryAxisEnabledOnMouseMove = false;
            bool IsSecondaryAxisEnabledOnMouseMove = false;
            bool IsSecondaryAxisEnabled = false;
            bool IsPrimaryAxisEnabled = false;
            axisCollection = new ObservableCollection<ChartAxis>();
            foreach (ChartAxis axis in this.Axes)
            {
                if (axis.m_enableRangeSelection)
                {
                    if (axis.Orientation == Orientation.Vertical)
                    {
                        IsSecondaryAxisEnabled = true;
                    }
                    else if (axis.Orientation == Orientation.Horizontal)
                    {
                        IsPrimaryAxisEnabled = true;
                    }
                    axisCollection.Add(axis);
                }
            }
            if (IsSecondaryAxisEnabled == true && IsPrimaryAxisEnabled == true)
            {
                this.RangeSelectionForBothAxis(e.GetPosition(this));
            }
            else
            {
                this.RangeSelection(e.GetPosition(this));
            }
            if (m_panning == true && (IsSecondaryAxisEnabled == false && IsPrimaryAxisEnabled == false))
            {                
                this.isactivatemousezoom = false;
                foreach (ChartAxis axis in this.Axes)
                {
                    Point newPosition1 = e.GetPosition((sender as Grid));
                    if (axis.ZoomFactor < 1)
                    {
                        this.Cursor = Cursors.Hand;
                        if (axis.Orientation == Orientation.Horizontal)
                        {
                            double newpt = PointToValue(axis, newPosition1);

                            double Xvalue = axis.lastPosition_X - newpt;
                            double Pstart =double.IsNaN(this.PanningXRange.Start)? axis.ActualVisibleRange.Start : this.PanningXRange.Start;
                            double Pend = double.IsNaN(this.PanningXRange.End) ? axis.ActualVisibleRange.End : this.PanningXRange.End;

                            Pstart = Pstart + Xvalue;
                            Pend = Pend + Xvalue;
                                                        
                            this.PanningXRange = new DoubleRange(Math.Round(Pstart, 2), Math.Round(Pend, 2));
                            double start = Pstart;
                            double end = Pend;
                            
                            double zoomvalue = (axis.Range.End - axis.Range.Start) * axis.ZoomFactor/2 ;
                            double actualscrollbarvalue = start + zoomvalue;
                            double value = (actualscrollbarvalue - axis.Range.Start) / (axis.Range.End - axis.Range.Start);

                            if (value < 0)
                            {
                                if (this.PrevPanningXRange.IsEmpty)
                                    this.PrevPanningXRange = this.PanningXRange;
                                value = 0;
                                this.PanningXRange = this.PrevPanningXRange;    
                            }
                            if (value > 1)
                            {
                                if (this.PrevPanningXRange.IsEmpty)
                                    this.PrevPanningXRange = this.PanningXRange;
                                value = 1;
                                this.PanningXRange = this.PrevPanningXRange;                                
                            }                            
                            if(value <1 && value>0)
                            {
                                this.PrevPanningXRange = DoubleRange.Empty;
                            }
                            this.HorizontalBar.Value = value;                            
                        }
                        else
                        {
                            double newpt = PointToValue(axis, newPosition1);
                            double Yvalue = axis.lasPosition_Y - newpt;
                            double Pstart = double.IsNaN(PanningYRange.Start)? axis.ActualVisibleRange.Start: PanningYRange.Start;
                            double Pend = double.IsNaN(PanningYRange.End)?axis.ActualVisibleRange.End :PanningYRange.End;

                            Pstart = Pstart + (axis.IsFractionEnabledOnZoom ? Yvalue : Math.Floor(Yvalue));
                            Pend = Pend + (axis.IsFractionEnabledOnZoom ? Yvalue : Math.Floor(Yvalue));

                            this.PanningYRange = new DoubleRange(Pstart, Pend);
                            double start = this.PanningYRange.Start;
                            double end = this.PanningYRange.End;
                            double zoomvalue = (axis.Range.End - axis.Range.Start) * axis.ZoomFactor / 2;
                            double autoscrollbarvalue = start + zoomvalue;
                            double verticalbarvalue = (autoscrollbarvalue - axis.Range.Start)/ (axis.Range.End - axis.Range.Start);
                            double value = 1 - verticalbarvalue;
                            if (value < 0)
                            {
                                if (this.prevPanningYRange.IsEmpty)
                                    this.prevPanningYRange = this.PanningYRange;
                                value = 0;
                                this.PanningYRange = this.prevPanningYRange;
                            }
                            if (value > 1)
                            {
                                if (this.prevPanningYRange.IsEmpty)
                                    this.prevPanningYRange = this.PanningYRange;
                                value = 1;
                                this.PanningYRange = this.prevPanningYRange;
                            }
                            if (value < 1 && value > 0)
                            {
                                this.prevPanningYRange = DoubleRange.Empty;
                            }
                            this.VerticalBar.Value = value;
                        }
                    }
                }
            }           
            if (this.EnableMouseDragZooming && isMouseLbuttonDown == true && this.isSwitchzoom == true && this.AreaType == ChartAxesType.CartesianAxes && this.isactivatemousezoom&& (IsSecondaryAxisEnabled == false && IsPrimaryAxisEnabled == false))
            {
                this.RangeSelectionMouseOverHeight = 0d;
                this.RangeSelectionMouseOverWidth = 0d;                
                isactivatemousezoom = true;
                seriesGrid.Children.Remove(zoomrectangle);
                zoomrectangle = new Rectangle();
                zoomRectEnd.X = ((Point)e.GetPosition(seriesGrid)).X;
                zoomRectEnd.Y = ((Point)e.GetPosition(seriesGrid)).Y;
                double diffy = 0, diffx = 0;
                if (zoomRectEnd.Y - zoomRectStart.Y < 0)
                {
                    diffy = 0 - (zoomRectEnd.Y - zoomRectStart.Y);
                }

                if (zoomRectEnd.X - zoomRectStart.X < 0)
                {
                    diffx = 0 - (zoomRectEnd.X - zoomRectStart.X);
                }

                zoomrectangle.Width = Math.Abs(zoomRectEnd.X - zoomRectStart.X);
                zoomrectangle.Height = Math.Abs(zoomRectEnd.Y - zoomRectStart.Y);
                //Canvas.SetLeft(zoomrectangle, zoomRectStart.X-diffx);
                //Canvas.SetTop(zoomrectangle, zoomRectStart.Y-diffy);
                zoomrectangle.HorizontalAlignment = HorizontalAlignment.Left;
                zoomrectangle.VerticalAlignment = VerticalAlignment.Top;
                zoomrectangle.Margin = new Thickness(zoomRectStart.X - diffx, zoomRectStart.Y - diffy, 0, 0);


                zoomrectangle.Stroke = new SolidColorBrush(Colors.Black);
                zoomrectangle.StrokeThickness = 1;
                zoomrectangle.Fill = new SolidColorBrush(Colors.White);
                zoomrectangle.Opacity = 0.75;
                //seriesGrid.Children.Add(zoomrectangle);
                seriesGrid.Children.Insert(1, zoomrectangle);
            }

            FrameworkElement fe = e.OriginalSource as FrameworkElement;
            Segment segment = fe != null ? fe.DataContext as Segment : null;
            if (segment != null)
            {
                ChartSeries series = segment.Series;
                series.OnMouseMove(series, new ChartMouseEventArgs(e, segment));
            }

            isMouseLeave = false;
            isMousemove = true;

            if (this.AllowSegmentDragDrop && dragSegment != null)
            {
                Point mousePoint = e.GetPosition(this);
                
                if (this.PrimaryAxis.VisibleRange.Inside(this.PointToValue(PrimaryAxis, new Point(mousePoint.X-this.AxesThickness.Left, mousePoint.Y))) && this.SecondaryAxis.VisibleRange.Inside(this.PointToValue(SecondaryAxis, new Point(mousePoint.X, mousePoint.Y-this.AxesThickness.Bottom))))
                {
                    int index = this.dragSegment.Series.Segments.IndexOf(dragSegment);
                    if (this.dragSegment.Series.Presenter != null && this.dragSegment.Series.Segments.Count > index)
                    {
                        DependencyObject obj = VisualTreeHelper.GetChild(this.dragSegment.Series.Presenter, index);
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
                                    popup.VerticalOffset = ((mousePoint.Y ) - (this.AxesThickness.Bottom+this.AxesThickness.Left) );
                                    popup.HorizontalOffset = mousePoint.X - this.AxesThickness.Left;
                                }
                            }
                        }
                    }
                }
                else
                {
                    int index = this.dragSegment.Series.Segments.IndexOf(dragSegment);
                    if (this.dragSegment.Series.Presenter != null && this.dragSegment.Series.Segments.Count > index)
                    {
                        DependencyObject obj = VisualTreeHelper.GetChild(this.dragSegment.Series.Presenter, index);
                        obj = VisualTreeHelper.GetChild(obj, 0);

                        if (obj is Canvas)
                        {
                            Canvas canvas = obj as Canvas;
                            if (canvas.Children.Count > 1 && canvas.Children[canvas.Children.Count - 1] is Popup)
                            {
                                Popup popup = canvas.Children[(canvas.Children.Count - 1)] as Popup;
                                if (popup != null)
                                {
                                    popup.IsOpen = false;
                                    dragSegment = null;
                                }
                            }
                        }
                    }
                }
            }
            ObservableCollection<ChartAxis> axisCollection1 = new ObservableCollection<ChartAxis>();
            foreach (ChartAxis axis in this.Axes)
            {
                if (axis.EnableRangeSelectionOnMouseOver)
                {
                    if (axis.Orientation == Orientation.Vertical)
                    {
                        IsSecondaryAxisEnabledOnMouseMove = true;
                    }
                    else if (axis.Orientation == Orientation.Horizontal)
                    {
                        IsPrimaryAxisEnabledOnMouseMove = true;
                    }
                    axisCollection1.Add(axis);
                }
            }
            if (IsSecondaryAxisEnabledOnMouseMove && IsPrimaryAxisEnabledOnMouseMove && !isactivatemousezoom)
            {
                this.m_RangeSelectionStartPointMouseOver = e.GetPosition(this);
                double bottomCanvas = this.m_RangeSelectionStartPointMouseOver.Y + 5 - this.AxesThickness.Bottom;
                double marginLeft = this.m_RangeSelectionStartPointMouseOver.X - this.AxesThickness.Left;
                double width = 0d, height = 0d;
                double marginVertical = 0d, marginHorizontal = 0d;
                foreach (ChartAxis axis in axisCollection1)
                {
                    if (!axis.m_enableRangeSelection && axis.ZoomFactor == 1)
                    {
                        if (axis.Orientation == Orientation.Vertical)
                        {
                            double value = this.PointToValue(axis, new Point(0, bottomCanvas));
                            double start = axis.VisibleRange.Start;
                            double margin = 0d;
                            while (start <= axis.VisibleRange.End)
                            {
                                if (value >= start && value <= start + axis.VisibleInterval)
                                {
                                    margin = start + axis.VisibleInterval;
                                }
                                start = start + axis.VisibleInterval;
                            }
                            if (axis.IsInversed)
                                margin = ((axis.ActualVisibleRange.End) - margin) + axis.ActualVisibleInterval;
                            marginVertical = this.ValueToPoint(axis, margin);
                            height = seriesGrid.ActualHeight - this.ValueToPoint(axis, axis.VisibleInterval);
                        }
                        else if (axis.Orientation == Orientation.Horizontal)
                        {
                            double value = this.PointToValue(axis, new Point(marginLeft, 0));
                            double start = axis.VisibleRange.Start;
                            double margin = 0d;
                            int count = 0;
                            while (start <= axis.VisibleRange.End)
                            {
                                if (value >= start && value <= start + axis.VisibleInterval)
                                {
                                    margin = start;
                                }
                                count = count + 1;
                                start = start + axis.ActualVisibleInterval;
                            }
                            if (axis.IsInversed)
                                margin = (axis.ActualVisibleRange.End) - margin;
                            marginHorizontal = this.ValueToPoint(axis, margin);
                            width = Math.Abs(this.ValueToPoint(axis, axis.ActualVisibleRange.Start) - this.ValueToPoint(axis, axis.ActualVisibleRange.Start + axis.ActualVisibleInterval));
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                this.RangeSelectionMouseOverHeight = height;
                this.RangeSelectionMouseOverWidth = width;
                this.RangeSelectionMouseMoveMargin = new Thickness(marginHorizontal, marginVertical, 0, 0);

            }
            else
            {
                foreach (ChartAxis axis in this.Axes)
                {
                    if (axis.EnableRangeSelectionOnMouseOver && !axis.m_enableRangeSelection && !isactivatemousezoom && axis.ZoomFactor == 1)
                    {
                        axis.EnableRangeSelectionOnMouseOver = true;
                        this.m_RangeSelectionStartPointMouseOver = e.GetPosition(this);
                        {
                            if (axis.Orientation == Orientation.Vertical)
                            {
                                double bottomCanvas = this.m_RangeSelectionStartPointMouseOver.Y + 5 - this.AxesThickness.Bottom;
                                double value = this.PointToValue(axis, new Point(0, bottomCanvas));
                                double start = axis.ActualVisibleRange.Start;
                                double margin = 0d;
                                while (start <= axis.ActualVisibleRange.End)
                                {
                                    if (value >= start && value <= start + axis.ActualVisibleInterval)
                                    {
                                        margin = start + axis.ActualVisibleInterval;
                                    }
                                    start = start + axis.ActualVisibleInterval;
                                }
                                if (axis.IsInversed)
                                    margin = ((axis.ActualVisibleRange.End) - margin) + axis.ActualVisibleInterval;
                                double seriesGridHeight = this.ValueToPoint(axis, 0);
                                this.RangeSelectionMouseOverHeight = Math.Abs(seriesGridHeight - this.ValueToPoint(axis, axis.ActualVisibleInterval));
                                this.RangeSelectionMouseOverWidth = seriesGrid.ActualWidth;
                                this.RangeSelectionMouseMoveMargin = new Thickness(0, this.ValueToPoint(axis, margin), 0, 0);
                            }
                            else
                            {
                                double marginLeft = this.m_RangeSelectionStartPointMouseOver.X - this.AxesThickness.Left - 5;
                                double value = this.PointToValue(axis, new Point(marginLeft, 0));
                                double start = axis.ActualVisibleRange.Start;
                                double margin = 0d;
                                int count = 0;
                                while (start <= axis.ActualVisibleRange.End)
                                {
                                    if (value >= start && value <= start + axis.ActualVisibleInterval)
                                    {
                                        margin = start;
                                    }
                                    count = count + 1;
                                    start = start + axis.ActualVisibleInterval;
                                }
                                if (axis.IsInversed)
                                    margin = (axis.ActualVisibleRange.End) - margin;
                                this.RangeSelectionMouseMoveMargin = new Thickness(this.ValueToPoint(axis, margin), 0, 0, 0);
                                this.RangeSelectionMouseOverHeight = this.ActualHeight;
                                double width = Math.Abs(this.ValueToPoint(axis, axis.ActualVisibleRange.Start) - this.ValueToPoint(axis, axis.ActualVisibleRange.Start + axis.ActualVisibleInterval));
                                this.RangeSelectionMouseOverWidth = width;
                            }
                        }
                    }
                    else if (axis.m_enableRangeSelection || axis.ZoomFactor < 1)
                    {
                        this.RangeSelectionMouseOverHeight = 0d;
                        this.RangeSelectionMouseOverWidth = 0d;
                    }
                }
            }
        }
        internal Point dragXY_new = new Point();
        /// <summary>
        /// Executes when mouse Left button up on chart series present area
        /// </summary>
        void seriescanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(this.seriesGrid);
            foreach (ChartAxis axis in this.Axes)
            {
                if (axis.m_enableRangeSelection == true)
                {
                    axis.m_enableRangeSelection = false;
                    if (this.RangeSelectionWidth > 2)
                    {
                        this.CloseButtonVisibility = Visibility.Visible;
                    }
                    this.m_RangeSelectionStartPoint = new Point();
                    this.m_RangeSelectionEndPoint = 0d;
                }
            }
            ////Set the MouseDrag Zoom range in Chart
            if (isactivatemousezoom == true && this.EnableMouseDragZooming)
            {
                MouseDragZoom(seriesGrid.ActualWidth, seriesGrid.ActualHeight, zoomRectStart, zoomRectEnd);
                seriesGrid.Children.Remove(zoomrectangle);
                zoomrectangle = new Rectangle();
            }

            if (pt.X >= 0 || pt.X <= this.seriesGrid.ActualWidth || pt.Y >= 0 || pt.Y <= seriesGrid.ActualHeight)
            {
                FrameworkElement fe = e.OriginalSource as FrameworkElement;
                if (fe != null)
                {
                    Segment segment = fe.DataContext as Segment;
                    if (segment != null)
                    {
                        ChartSeries series = segment.Series;
                        series.OnMouseLeftButtonUp(series, new ChartMouseEventArgs(e, segment));
                    }
                }
            }
            seriesGrid.ReleaseMouseCapture();
            isMouseLbuttonDown = false;
            isactivatemousezoom = false;
            if (this.AllowSegmentDragDrop && dragSegment != null && this.dragSegment.Series != null)
            {
                int index = this.dragSegment.Series.Segments.IndexOf(dragSegment);
                if (this.dragSegment.Series.Presenter != null && this.dragSegment.Series.Segments.Count > index)
                {
                    DependencyObject obj = VisualTreeHelper.GetChild(this.dragSegment.Series.Presenter, index);
                    obj = VisualTreeHelper.GetChild(obj, 0);

                    if (obj is Canvas)
                    {
                        Canvas canvas = obj as Canvas;
                        if (canvas.Children.Count > 1 && canvas.Children[canvas.Children.Count - 1] is Popup)
                        {
                            Popup popup = canvas.Children[(canvas.Children.Count - 1)] as Popup;
                            if (popup != null)
                            {
                                popup.IsOpen = false;
                            }
                        }
                    }
                }
                Point pt1 = e.GetPosition(this);
                double newptX = this.PointToValue(this.PrimaryAxis, pt1);
                double newptY = this.PointToValue(this.SecondaryAxis, new Point(pt1.X, (pt1.Y- (this.AxesThickness.Bottom+this.AxesThickness.Left))));
                double oldptX = this.PointToValue(this.PrimaryAxis, this.dragXY);
                double oldpty = this.PointToValue(this.SecondaryAxis, this.dragXY);
                if (this.dragSegment.Series.Data.Count > index)
                {
                    double prevY1 = this.dragSegment.Series.Data[index].Values[0];
                    dragXY_new = new Point(newptX - oldptX, newptY);
                    if (this.PrimaryAxis.Range.Start < (this.dragSegment.Series.Data[index].X + dragXY_new.X))
                    {
                        this.dragSegment.Series.Data[index].X = this.dragSegment.Series.Data[index].X + dragXY_new.X;
                    }
                    if (this.SecondaryAxis.Range.End > dragXY_new.Y)
                    {
                        this.dragSegment.Series.Data[index].Y = dragXY_new.Y;
                    }
                    for (int j = 0; j < this.dragSegment.Series.Data[index].Values.Length; j++)
                    {
                        if (j != 0)
                        {
                            this.dragSegment.Series.Data[index].Values[j] = this.dragSegment.Series.Data[index].Values[j - 1] + (this.dragSegment.Series.Data[index].Values[j] - prevY1);
                        }
                    }
                }
                this.LoadArea();

                this.dragSegment = null;
            }
            m_panning = false;
        }

        /// <summary>
        /// Executes when mouse left button down on chart series present area
        /// </summary>
        void seriescanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (isMouseLeave == false && isMousemove == true)
            {
                zoomRectStart.X = ((Point)e.GetPosition(seriesGrid)).X;
                zoomRectStart.Y = ((Point)e.GetPosition(seriesGrid)).Y;
            }

            FrameworkElement fe = e.OriginalSource as FrameworkElement;
            Segment segment = fe.DataContext as Segment;
            if (segment != null)
            {
                ChartSeries series = segment.Series;
                series.OnMouseLeftButtonDown(series, new ChartMouseEventArgs(e, segment));
            }

            if (this.isSwitchzoom == true)
            {
                seriesGrid.CaptureMouse();
            }

            isMouseLbuttonDown = true;

            if (this.AllowSegmentDragDrop && dragSegment != null)
            {
                dragXY = e.GetPosition(this);
            }
            if (this.EnableMouseDragZooming && isMouseLbuttonDown == true && this.isSwitchzoom == true && this.AreaType == ChartAxesType.CartesianAxes)
            {
                isactivatemousezoom = true;
            }


            if (IsPanning && isZoomactivated)
            {
                Point point = e.GetPosition(sender as Grid);
                foreach (ChartAxis axis in this.Axes)
                {
                    if (axis.ZoomFactor != 1)
                    {
                        if (axis.Orientation == Orientation.Horizontal)
                        {
                            axis.lastPosition_X = this.PointToValue(axis, point);
                        }
                        else
                        {
                            axis.lasPosition_Y = this.PointToValue(axis, point);
                        }
                        m_panning = true;
                        this.isactivatemousezoom = false;
                        this.Cursor = Cursors.Hand;
                    }

                }
            }
            foreach (ChartAxis axis in this.Axes)
            {
                if (axis.EnableRangeSelection)
                {
                    this.CloseButtonVisibility = System.Windows.Visibility.Collapsed;
                    axis.m_enableRangeSelection = true;
                    this.m_RangeSelectionStartPoint = e.GetPosition(this);
                }
            }
        }
        internal Point m_RangeSelectionStartPoint = new Point();
        internal double m_RangeSelectionEndPoint = 0d;

        internal Point m_RangeSelectionStartPointMouseOver = new Point();
        internal DoubleRange dragzoomrange = DoubleRange.Empty;

        /// <summary>
        /// Executes when mouse Click and Drag chart series present area
        /// </summary>
        private void MouseDragZoom(double width, double height, Point start, Point end)
        {
            //this.BeginInit();
            dragzoomrange = DoubleRange.Empty;
            int rounddigits = 3;
            for (int i = 0; i < this.Axes.Count; i++)
            {
                if (Axes[i].IsFractionEnabledOnZoom == false)
                {
                    rounddigits = 0;
                }
                else
                {
                    rounddigits = 3;
                }

                if (Axes[i].Orientation == Orientation.Horizontal && Axes[i].EnableZooming == true)
                {
                    dragzoomrange = XSeriesPointToDataValues(width, height, start.X, end.X, Axes[i].ActualVisibleRange, rounddigits);
                    double newzoomposition = dragzoomrange.Start + ((dragzoomrange.End - dragzoomrange.Start) / 2);
                    double newzoomfactor = ((dragzoomrange.End - newzoomposition) / (Axes[i].Range.End - Axes[i].Range.Start)) * 2.0;
                    Axes[i].ZoomPosition = newzoomposition;
                    Axes[i].isUpdateScrollbar = true;
                    Axes[i].isUpdateViewportsize = true;
                    Axes[i].ZoomFactor = newzoomfactor;
                }
                else if (Axes[i].Orientation == Orientation.Vertical && Axes[i].EnableZooming == true)
                {
                    dragzoomrange = YSeriesPointToDataValues(width, height, start.Y, end.Y, Axes[i].ActualVisibleRange, rounddigits);
                    double newzoomposition = dragzoomrange.Start + ((dragzoomrange.End - dragzoomrange.Start) / 2);
                    double newzoomfactor = ((dragzoomrange.End - newzoomposition) / (Axes[i].Range.End - Axes[i].Range.Start)) * 2.0;
                    Axes[i].ZoomPosition = newzoomposition;
                    Axes[i].isUpdateScrollbar = true;
                    Axes[i].isUpdateViewportsize = true;
                    Axes[i].ZoomFactor = newzoomfactor;
                }
            }

            //////this.LoadArea();
            //this.EndInit();
        }

        /// <summary>
        /// Find the Chart points from the Mouse Drag X'positions
        /// </summary>
        /// <returns>
        /// DoubleRange Values
        /// </returns>
        private DoubleRange XSeriesPointToDataValues(double width, double height, double startX, double endX, DoubleRange range, int rounddigits)
        {
            double start, end;
            if (endX > width)
            {
                endX = width;
            }

            if (endX < 0)
            {
                endX = 0;
            }

            start = startX / width * (range.End - range.Start);
            end = endX / width * (range.End - range.Start);
            DoubleRange zoomXrange = new DoubleRange(Math.Round(range.Start + start, rounddigits), Math.Round(range.Start+end, rounddigits));
            if (zoomXrange.Start == zoomXrange.End)
            {
                zoomXrange = new DoubleRange(Math.Floor(range.Start + start), Math.Ceiling(range.Start + end));
            }

            return zoomXrange;
        }

        /// <summary>
        /// Find the Chart points from the Mouse Drag y'positions
        /// </summary>
        /// <returns>
        /// DoubleRange Values
        /// </returns>
        private DoubleRange YSeriesPointToDataValues(double width, double height, double startY, double endY, DoubleRange range, int rounddigits)
        {
            double start, end;
            if (endY > height)
            {
                endY = height;
            }

            if (endY < 0)
            {
                endY = 0;
            }

            start = (((startY / height) * (range.End - range.Start)) - (range.End - range.End)) * -1;
            end = (((endY / height) * (range.End - range.Start)) - (range.End - range.End)) * -1;
            DoubleRange zoomYrange = new DoubleRange(Math.Round(range.End + start, rounddigits), Math.Round(range.End + end, rounddigits));
            if (zoomYrange.Start == zoomYrange.End)
            {
                zoomYrange = new DoubleRange(Math.Floor(range.End + start), Math.Ceiling(range.End + end));
            }

            return zoomYrange;
        }

        /// <summary>
        /// Converts Value to point.
        /// </summary>
        /// <param name="axis">The Chart axis .</param>
        /// <param name="value">The value.</param>
        /// <returns>The double value to point</returns>
        public double ValueToPoint(ChartAxis axis, double value)
        {
            double result = double.NaN;
            if (seriesGrid != null && axis != null)
            {
                if (axis.Orientation == Orientation.Horizontal)
                {
                    result = seriesGrid.ActualWidth / (axis.ActualVisibleRange.End - axis.ActualVisibleRange.Start) * (value + (axis.ActualVisibleRange.Start * (-1)));
                }
                else
                {
                    result = seriesGrid.ActualHeight * (1 - ((1 / (axis.ActualVisibleRange.End - axis.ActualVisibleRange.Start)) * (value + (axis.ActualVisibleRange.Start * (-1)))));
                }
            }

            return result;
        }

        /// <summary>
        /// Converts point to value.
        /// </summary>
        /// <param name="axis">The axis value.</param>
        /// <param name="point">The point.</param>
        /// <returns>The double point to value</returns>
        public double PointToValue(ChartAxis axis, Point point)
        {
            double result = double.NaN;
            if (seriesGrid != null && axis != null)
            {
                if (axis.Orientation == Orientation.Horizontal)
                {
                    result = axis.ActualVisibleRange.Start + (point.X / seriesGrid.ActualWidth * (axis.ActualVisibleRange.End - axis.ActualVisibleRange.Start));
                    if (axis.IsInversed)
                        result = (axis.ActualVisibleRange.Start + axis.ActualVisibleRange.End) - result;
                }
                else
                {
                    result = axis.ActualVisibleRange.End + ((((point.Y / seriesGrid.ActualHeight) * (axis.ActualVisibleRange.End - axis.ActualVisibleRange.Start)) - (axis.ActualVisibleRange.End - axis.ActualVisibleRange.End)) * -1);
                    if (axis.IsInversed)
                        result = (axis.ActualVisibleRange.Start + axis.ActualVisibleRange.End) - result;
                }
            }

            return result;
        }

        /// <summary>
        /// Used to set the zooming scrollbar positions.
        /// </summary>
        void SetZoomingScrollBarPosition()
        {
            if (HorizontalBar != null && VerticalBar != null)
            {
                if (HorizontalBar.OpposedPosition == true)
                {
                    HorizontalBar.VerticalAlignment = VerticalAlignment.Top;
                }
                else
                {
                    HorizontalBar.VerticalAlignment = VerticalAlignment.Bottom;
                }

                if (VerticalBar.OpposedPosition == true)
                {
                    VerticalBar.HorizontalAlignment = HorizontalAlignment.Left;
                }
                else
                {
                    VerticalBar.HorizontalAlignment = HorizontalAlignment.Right;
                }
            }
        }

        /// <summary>
        /// Executes when series collection values changed
        /// </summary>
        void Series_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ChartSeries series in e.NewItems)
                {
                    series.Area = this;
                   
                }
            }

            if (e.OldItems != null)
            {
                this.LoadArea();
            }
            else if (this.isAreaLoaded == true )
            {
                this.isUpdateArea = true;

                if (this.Legends != null)
                {         
                    this.Legends.InvalidateMeasure();
                    
                }
            }
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                if (this.Legends != null)
                {
                    this.Legends.InvalidateMeasure();
                }
            }
        }

        private static void OnGridBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
        }

        private static void OnSideBySideSeriesPlacementPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartArea area = (ChartArea)d;
            if (area != null)
            {
                area.LoadArea();
            }
        }

        private static void OnSeriesChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartArea area = (ChartArea)d;
            if (area != null)
            {
                if (args.NewValue != null)
                {
                    foreach (ChartSeries series in args.NewValue as SeriesCollection)
                    {
                        series.Area = area;
                    }
                }
            }
        }

        /// <summary>
        /// Executes when AreaType property value changed
        /// </summary>
        private static void OnAreaTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartArea area = (ChartArea)d;
            if (area != null)
            {
                area.GoToAreaVisualState();
            }
        }

        private static void OnWatermarkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartArea area = d as ChartArea;
            if (area != null && e.OldValue != e.NewValue)
            {
                ChartAreaWatermarkControl watermarkGrid = area.GetTemplateChild("watermarkControl") as ChartAreaWatermarkControl;
                if (watermarkGrid != null)
                {
                    Binding contentBinding = new Binding();
                    contentBinding.RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent);
                    contentBinding.Converter = new WatermarkContentConverter();
                    if (area.WatermarkType == WatermarkTypes.Text)
                    {
                        (watermarkGrid.Background as ImageBrush).ImageSource = null;
                        area.m_watermarkText = area.WatermarkText;
                        contentBinding.ConverterParameter = "Text";
                        BindingOperations.SetBinding((watermarkGrid.Children[0] as TextBlock), TextBlock.TextProperty, contentBinding);
                    }
                    else
                    {
                        (watermarkGrid.Children[0] as TextBlock).Text = string.Empty;
                        (watermarkGrid.Background as ImageBrush).Stretch = area.WatermarkImageStretch;
                        area.m_watermarkimageSource = area.WatermarkImageSource;
                        contentBinding.ConverterParameter = "Image";
                        BindingOperations.SetBinding((watermarkGrid.Background as ImageBrush), ImageBrush.ImageSourceProperty, contentBinding);
                    }
                }
            }
        }

        private static void OnWatermarkAlignmentXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartArea area = d as ChartArea;
            if (area != null && e.OldValue != e.NewValue)
            {
                ChartAreaWatermarkControl watermarkGrid = area.GetTemplateChild("watermarkControl") as ChartAreaWatermarkControl;
                if (watermarkGrid != null)
                {
                    Binding alignmentBinding = new Binding();
                    alignmentBinding.RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent);
                    alignmentBinding.Converter = new WatermarkAlignmentConverter();
                    alignmentBinding.ConverterParameter = "AlignmentX";
                    BindingOperations.SetBinding((watermarkGrid.Children[0] as TextBlock), TextBlock.HorizontalAlignmentProperty, alignmentBinding);
                    (watermarkGrid.Background as ImageBrush).AlignmentX = area.WatermarkAlignmentX;
                }
            }
        }

        private static void OnWatermarkAlignmentYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartArea area = d as ChartArea;
            if (area != null && e.OldValue != e.NewValue)
            {
                ChartAreaWatermarkControl watermarkGrid = area.GetTemplateChild("watermarkControl") as ChartAreaWatermarkControl;
                if (watermarkGrid != null)
                {
                    Binding alignmentBinding = new Binding();
                    alignmentBinding.RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent);
                    alignmentBinding.Converter = new WatermarkAlignmentConverter();
                    alignmentBinding.ConverterParameter = "AlignmentY";
                    BindingOperations.SetBinding((watermarkGrid.Children[0] as TextBlock), TextBlock.VerticalAlignmentProperty, alignmentBinding);
                    (watermarkGrid.Background as ImageBrush).AlignmentY = area.WatermarkAlignmentY;
                }
            }
        }

        private static void OnWatermarkOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartArea area = d as ChartArea;
            if (area != null && e.OldValue != e.NewValue)
            {
                ChartAreaWatermarkControl watermarkGrid = area.GetTemplateChild("watermarkControl") as ChartAreaWatermarkControl;
                if (watermarkGrid != null)
                {
                    (watermarkGrid.Children[0] as TextBlock).Opacity = area.WatermarkOpacity;
                    (watermarkGrid.Background as ImageBrush).Opacity = area.WatermarkOpacity;
                }
            }
        }

        private static void OnWatermarkImageStretchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartArea area = d as ChartArea;
            if (area != null && e.OldValue != e.NewValue)
            {
                ChartAreaWatermarkControl watermarkGrid = area.GetTemplateChild("watermarkControl") as ChartAreaWatermarkControl;
                if (watermarkGrid != null)
                {
                    (watermarkGrid.Background as ImageBrush).Stretch = area.WatermarkImageStretch;
                }
            }
        }

        private static void OnWatermarkRotationAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartArea area = d as ChartArea;
            if (area != null && e.OldValue != e.NewValue)
            {
                ChartAreaWatermarkControl watermarkGrid = area.GetTemplateChild("watermarkControl") as ChartAreaWatermarkControl;
                if (watermarkGrid != null)
                {
                    watermarkGrid.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, watermarkGrid.ActualWidth, watermarkGrid.ActualHeight) };
                    (watermarkGrid.Children[0] as TextBlock).RenderTransform = new RotateTransform() { Angle = area.WatermarkRotationAngle };
                    (watermarkGrid.Background as ImageBrush).RelativeTransform = new RotateTransform() { Angle = area.WatermarkRotationAngle, CenterX = 0.5, CenterY = 0.5 };
                }
            }
        }

        private static void OnWatermarkTextFontColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartArea area = d as ChartArea;
            if (area != null && e.OldValue != e.NewValue)
            {
                ChartAreaWatermarkControl watermarkGrid = area.GetTemplateChild("watermarkControl") as ChartAreaWatermarkControl;
                if (watermarkGrid != null)
                    (watermarkGrid.Children[0] as TextBlock).Foreground = area.WatermarkTextFontColor;
            }
        }

        private static void OnWatermarkTextFontSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartArea area = d as ChartArea;
            if (area != null && e.OldValue != e.NewValue)
            {
                ChartAreaWatermarkControl watermarkGrid = area.GetTemplateChild("watermarkControl") as ChartAreaWatermarkControl;
                if (watermarkGrid != null)
                    (watermarkGrid.Children[0] as TextBlock).FontSize = area.WatermarkTextFontSize;
            }
        }

        private static void OnWatermarkTextFontFamilyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartArea area = d as ChartArea;
            if (area != null && e.OldValue != e.NewValue)
            {
                ChartAreaWatermarkControl watermarkGrid = area.GetTemplateChild("watermarkControl") as ChartAreaWatermarkControl;
                if(watermarkGrid != null)
                    (watermarkGrid.Children[0] as TextBlock).FontFamily = area.WatermarkTextFontFamily;
            }
        }

        private static void OnWatermarkTextFontWeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartArea area = d as ChartArea;
            if (area != null && e.OldValue != e.NewValue)
            {
                ChartAreaWatermarkControl watermarkGrid = area.GetTemplateChild("watermarkControl") as ChartAreaWatermarkControl;
                if (watermarkGrid != null)
                    (watermarkGrid.Children[0] as TextBlock).FontWeight = area.WatermarkTextFontWeight;
            }
        }
        /// <summary>
        /// Method to choose ChartAxesProvider value based upon the AreaType
        /// </summary>
        public void GoToAreaVisualState()
        {
            switch (this.AreaType)
            {
                case ChartAxesType.CartesianAxes:
                    //VisualStateManager.GoToState(this, "CartesianAxes", true);
                    foreach (ChartAxis axis in this.Axes)
                    {
                        axis.ChartAxesProvider = new ChartCartesianAxesGenerator();
                    }
                    break;
                case ChartAxesType.None:
                    foreach (ChartAxis axis in this.Axes)
                    {
                        axis.ChartAxesProvider = null;
                    }
                    break;
                case ChartAxesType.RadarAxes:
                    foreach (ChartAxis axis in this.Axes)
                    {
                        axis.ChartAxesProvider = this.PrimaryAxis.Equals(axis) || this.SecondaryAxis.Equals(axis) ? new ChartRadarAxesGenerator() : null;
                    }
                    
                    break;
                case ChartAxesType.PolarAxes:
                    foreach (ChartAxis axis in this.Axes)
                    {
                        axis.ChartAxesProvider = this.PrimaryAxis.Equals(axis) || this.SecondaryAxis.Equals(axis) ? new ChartPolarAxesGenerator() : null;
                    }
                    
                    break;
            }
        }

        private static void OnContextMenuTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartArea area = (ChartArea)d;
            area.LoadContextMenu();
        }

        /// <summary>
        /// Executes when IsZoomAllAxes property value changed
        /// </summary>
        private static void OnIsZoomAllAxesChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartArea area = (ChartArea)d;
            if (area != null && area.Axes != null)
            {
                for (int i = 0; i < area.Axes.Count; i++)
                {
                    area.Axes[i].EnableZooming = (bool)args.NewValue;
                }
            }
        }

        /// <summary>
        /// Executes when OnSplitterVisibility property value changed
        /// </summary>
        private static void OnSplitterVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
              ChartArea area = (ChartArea)d;
              if (area.SplitterVisibility == SplitterBarVisibility.Hide)
              {
                  if (area.splittergrid != null)
                      area.splittergrid.Visibility = Visibility.Collapsed;
              }
              else
              {
                  if (area.splittergrid != null)
                      area.splittergrid.Visibility = Visibility.Visible;

              }
              if (area.SplitterVisibility == SplitterBarVisibility.AlwaysVisible)
              {
                  if(area.splittergrid != null)
                  area.splittergrid.Fill = area.SplitterBrush;                  

              }
              else
              {
                  if (area.splittergrid != null)
                  area.splittergrid.Fill = new SolidColorBrush(Colors.Transparent);                  
              }           
        }

        static void splittergrid_MouseLeave(object sender, MouseEventArgs e)
        {

            Rectangle splitter = sender as Rectangle;
            var obj = VisualTreeHelper.GetParent(splitter);
            while (obj.GetType() != typeof(ChartArea))
            {
                obj = VisualTreeHelper.GetParent(obj);
            }

            if (!(obj as ChartArea).IsResizing &&(obj as ChartArea).SplitterVisibility != SplitterBarVisibility.AlwaysVisible)
            {
                splitter.Fill = new SolidColorBrush(Colors.Transparent);
            }
            
        }
       

        #region ContextMenuCommands

        private void ZoomingCommandMethod(object parameter)
        {
            this.SwitchZooming();
            ZoomingToolKit.SetZoomingToolkitVisibility(this, Visibility.Visible);
        }
        private void SeriesCommandMethod(object parameter)
        {
            this.SwitchZooming();
            ZoomingToolKit.SetZoomingToolkitVisibility(this, Visibility.Visible);
        }
        private void PalletteCommandMethod(object parameter)
        {
            ChartColorPalette palette = (ChartColorPalette)Enum.Parse(typeof(ChartColorPalette), parameter.ToString(), true);
            this.ColorModel.Palette = palette;
            this.LoadArea();
            foreach (ChartSeries series in this.Series)
            {
                if (series.Presenter != null && series.ColorEachDependent && series.ColorEach != null)
                {
                    series.UpdateColorEachSegments();
                }
            }
            if (this.Legends != null)
            {
                this.Legends.GenerateItems();
            }
        }
        private void StyleCommandMethod(object parameter)
        {
            ChartStyles style = (ChartStyles)Enum.Parse(typeof(ChartStyles), parameter.ToString(), true);
            Chart chart = GetChartParent();
            chart.ChartVisualStyle = style;
        }
        private void SetSeriesCommandMethod(object parameter)
        {
            List<object> value = parameter as List<object>;
            this.Series[(int)value[0]].Type = (ChartTypes)Enum.Parse(typeof(ChartTypes), value[1].ToString(), true);
        }

        private Chart GetChartParent()
        {
            UIElement obj = this;
            while (typeof(Chart) != obj.GetType())
            {
                obj = VisualTreeHelper.GetParent(obj) as UIElement;
            }
            return obj as Chart;
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.zoomrectangle = null;            
            this.ClearValue(ChartArea.AreaTypeProperty);
            this.Loaded -= new RoutedEventHandler(Area_Loaded);

            if (this.Template != null)
                this.Template = null;

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

            if (this.Series != null)
            {
                Series.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Series_CollectionChanged);
                for (int temp = 0; temp < this.Series.Count; temp++)
                    this.Series[temp].Dispose();
                this.Series.Clear();
                this.Series = null;
            }

            if(this.InteractiveCursors !=null)
            {
                this.InteractiveCursors.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(InteractiveCursorCollections_CollectionChanged);
                this.InteractiveCursors.Clear();
                this.InteractiveCursors = null;
            }
            if (this.Axes != null)
            {
                for (int temp = 0; temp < this.Axes.Count; temp++)
                    this.Axes[temp].Dispose();
                this.Axes.Clear();
                this.Axes = null;
            }

            if (this.chartareaheader != null)
            {
                this.chartareaheader.Content = null;
                this.chartareaheader.ContentTemplate = null;
                this.chartareaheader = null;
            }

            if (this.legendcontent != null)
            {
                legendcontent.ClearValue(ContentPresenter.VisibilityProperty);
                legendcontent.ClearValue(ContentPresenter.ContentProperty);
                this.legendcontent.Content = null;
                this.legendcontent.ContentTemplate = null;
                this.legendcontent = null;
            }

            if (this.Legends != null)
            {
                this.Legends.Dispose();
                this.Legends = null;
            }
            this.ClearValue(ChartArea.LegendsProperty);

            if (this.seriesGrid != null)
            {
                seriesGrid.MouseLeftButtonDown -= new MouseButtonEventHandler(seriescanvas_MouseLeftButtonDown);
                seriesGrid.MouseLeftButtonUp -= new MouseButtonEventHandler(seriescanvas_MouseLeftButtonUp);
                seriesGrid.MouseMove -= new MouseEventHandler(seriescanvas_MouseMove);
                seriesGrid.MouseLeave -= new MouseEventHandler(seriescanvas_MouseLeave);
                this.seriesGrid.Children.Clear();
                this.seriesGrid = null;
            }

            if (this.ZoomTool != null)
            {
                ZoomTool.Dispose();
                ZoomTool = null;
            }

            if (this.grid != null)
            {
                this.grid.SizeChanged -= new SizeChangedEventHandler(grid_SizeChanged);
                this.grid.Children.Clear();
                this.grid = null;
            }

            if (this.chartlegent != null)
                this.chartlegent = null;
            
            if (this.chartareaheader != null)
            {
                this.chartareaheader.Content = null;
                this.chartareaheader.ContentTemplate = null;
                this.chartareaheader = null;
            }

            if (this.HorizontalBar != null)
            {
                this.HorizontalBar.Dispose();
                this.HorizontalBar = null;
            }
            if (this.VerticalBar != null)
            {
                this.VerticalBar.Dispose();
                this.VerticalBar = null;
            }
            if (this.ColorModel != null)
            {
                this.ColorModel.Dispose();
            }
            if (this.stripLinePanel != null)
            {                
                this.stripLinePanel = null;
            }
            if (Breaks != null)
            {
                Breaks.Clear();
                Breaks = null;
            }
            this.ClearValue(ContextMenuTypeProperty);
            this.Resources.Clear();
            this.Resources = null;
            GC.Collect();
            GC.SuppressFinalize(this);
        }

    
        #endregion
    }

    /// <summary>
    /// Class implementation for SyncAreasPanel
    /// </summary>
    [System.ComponentModel.DesignTimeVisible(false)]
    public class SyncAreasPanel : Panel
    {
        internal double oldLegendValue = 0d;
        internal bool splitterPositionFlag = false;

        /// <summary>
        /// Provides the behavior for the Arrange pass of Silverlight layout. Classes can override this method to define their own Arrange pass behavior.
        /// </summary>
        /// <returns>
        /// The actual size that is used after the element is arranged in layout.
        /// </returns>
        /// <param name="arrangeSize">The final area within the parent that this object should use to arrange itself and its children.</param>
        protected override Size ArrangeOverride(Size arrangeSize)
        { 
            
            double startY = 0d;
            double actualheight = 0d;
            foreach (Control control in this.Children)
            {                
                ChartArea area = control as ChartArea;
                
                if (area != null )
                {
                    actualheight = area.Height > 0 && !double.IsNaN(area.Height) && !double.IsPositiveInfinity(area.Height) ? area.Height : 0;                    
                    control.Arrange(new Rect(0, startY , arrangeSize.Width, actualheight));
                    startY += actualheight ;
                }               
            }

            return arrangeSize;
        }

        /// <summary>
        /// Measures the child elements of a <see cref="T:System.Windows.Controls.StackPanel"/> in anticipation of arranging them during the <see cref="M:System.Windows.Controls.StackPanel.ArrangeOverride(System.Windows.Size)"/> pass.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Windows.Size"/> that represents the desired size of the element.
        /// </returns>
        internal Grid GetGrid()
        {
            DependencyObject element = this;
            while (!(element is Grid) && element != null)
            {
                element = VisualTreeHelper.GetParent(element);
            }

            if (element != null)
            {
                return element as Grid;
            }

            return null;
        }
        private double? prev_ht = null;

        /// <summary>
        /// Provides the behavior for the Measure pass of Silverlight layout. Classes can override this method to define their own Measure pass behavior.
        /// </summary>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of the allocated sizes for child objects; or based on other considerations, such as a fixed container size.
        /// </returns>
       /// <param name="constraint1"></param>
        protected override Size MeasureOverride(Size constraint1)
        {            
            Size constraint = new Size ();
            SyncChartAreas syncarea = (this.Children[0] as ChartArea).ChartAreaParent;
            constraint.Width = double.IsNaN(constraint1.Width) || double.IsPositiveInfinity(constraint1.Width) ? 1  : constraint1.Width;
            constraint.Height = double.IsNaN(constraint1.Height) || double.IsPositiveInfinity(constraint1.Height) ? 1 : constraint1.Height;
            constraint.Width = constraint.Width <= 0 ? 1 : constraint.Width;
            constraint.Height = constraint.Height <= 0 ? 1 : constraint.Height;
            bool indicatorenabled = false;
            foreach (Control control in this.Children)
            {         
                ChartArea area = control as ChartArea;
                if (area.isIndicatorEnabled)
                {
                    indicatorenabled = true;
                }
            }
            var data = (from area in this.Children.OfType<ChartArea>() where area.isLoadedFirst && !double.IsNaN(area.Height) select area.Height);
            if (!indicatorenabled)
            {
                UpdateSplitRatio(constraint, data.Sum(), data.Count<double>());

            }
            double? adjustment_ht=0;
            if (prev_ht != null && prev_ht != constraint.Height)
            {                
                adjustment_ht= constraint.Height - prev_ht;                               
            }
             double startY = 0d;
            double actualheight = 0d;
            double totalratio = 1d;
            double areaRatio = 0d;
            
            if (syncarea.areaht_modified)
            {
                double diffht = 0;
                foreach (Control control in this.Children)
                {
                    ChartArea area = control as ChartArea;
                    Thickness common_areathickness = (this.Children[0] as ChartArea).AxesThickness; 
                    if (this.Children.IndexOf(area) == this.Children.Count - 1)
                    {
                        area.MinHeight = 80;
                       // area.AxesThickness = new Thickness(area.AxesThickness.Left, area.AxesThickness.Top, common_areathickness.Right, area.AxesThickness.Bottom);
                    }
                    if (area != null)
                    {              

                        if (!area.IsSplitterDrag)
                        {
                            if (((this.Children.IndexOf(area) - 1) >= 0 && ((this.Children.IndexOf(area) - 1) == syncarea.modifiedarea_idx)) || ((this.Children.IndexOf(area) - 1) >= 0 && (this.Children[this.Children.IndexOf(area) - 1] as ChartArea).Height == 0 && (this.Children.IndexOf(area) - 2) == syncarea.modifiedarea_idx))
                            {
                                if ((area.Height + diffht) < 0)
                                {
                                    area.Height = 0;
                                }
                                else
                                {
                                    area.Height = area.Height + diffht;
                                }
                            }
                            else
                            {
                                area.Height = area.Height;
                            }                           
                        }
                        else
                        {
                            diffht = area.pre_ht - area.Height;
                            area.Height = area.Height;
                            area.IsSplitterDrag = false;                           
                        }
                        double marginHeight = area.Margin.Bottom + area.Margin.Top;
                        marginHeight = this.Children.Count <= 1 ? marginHeight + 60 : marginHeight;
                        double ht = ((constraint.Height - marginHeight) * ((area.Height / (constraint.Height - marginHeight)))) + (double)(adjustment_ht / this.Children.Count);
                        if (ht < 0)
                        {
                            actualheight = 0;//area.Height > 0 && !double.IsNaN(area.Height) && !double.IsPositiveInfinity(area.Height) ? area.Height : 0;
                            if (this.Children.IndexOf(area) == this.Children.Count - 1)
                            {
                                if (startY + actualheight != constraint.Height)
                                {
                                    actualheight += (constraint.Height - (startY + actualheight));
                                }
                            }

                            startY += actualheight;
                            if (actualheight < area.MinHeight)
                            {
                                area.Height = area.MinHeight;
                            }
                            else
                            {
                                area.Height = actualheight;
                            }
                        }
                        else
                        {
                            actualheight = ht;// > 0 && !double.IsNaN(area.Height) && !double.IsPositiveInfinity(area.Height) ? area.Height : 0;
                            if (this.Children.IndexOf(area) == this.Children.Count - 1)
                            {
                                if (startY + actualheight != constraint.Height)
                                {
                                    actualheight += (constraint.Height - (startY + actualheight));
                                }
                            }

                            startY += actualheight;
                            if (ht < area.MinHeight)
                            {
                                area.Height = area.MinHeight;
                            }
                            else
                            {
                                area.Height = actualheight;
                            }
                        }
                        area.Measure(constraint);
                    }                    
                }               
            }
            else
            {
                Thickness common_areathickness = (this.Children[0] as ChartArea).AxesThickness; 
                foreach (Control control in this.Children)
                {
                    ChartArea area = control as ChartArea;                    
                    if (this.Children.IndexOf(area) == this.Children.Count - 1)
                    {
                        area.MinHeight = 20;
                       // area.AxesThickness = new Thickness(area.AxesThickness.Left, area.AxesThickness.Top, common_areathickness.Right, area.AxesThickness.Bottom);
                    }
                    if (area != null)
                    {
                            if (this.Children.IndexOf(area) == 0)
                            {
                                totalratio = (double.IsNaN(area.SplitRatio) || double.IsPositiveInfinity(totalratio)) ? 1d / this.Children.Count : area.SplitRatio;
                                areaRatio = totalratio;
                            }
                            else
                            {
                                totalratio = (1 - areaRatio) / (this.Children.Count - 1);
                            }
                            double marginHeight = area.Margin.Bottom + area.Margin.Top+ area.AxesThickness.Top+ area.AxesThickness.Bottom;
                            marginHeight = this.Children.Count <= 1 ? marginHeight + 60 : marginHeight;
                            area.Height = (constraint.Height - marginHeight) * totalratio;
                            area.Measure(constraint);                      

                    }
                   
                }
            }
            
            this.prev_ht = constraint.Height;
            return constraint;
        }
        

        private void UpdateSplitRatio(ChartArea area, Size TotalSize, double areaHeight)
        {
            if (!(double.IsNaN(areaHeight)))
            {
                area.SplitRatio = Math.Round(areaHeight, 0) / TotalSize.Height;
                area.ChartAreaParent.splitterFlag = true;
            }
        }
        internal bool _flag = false;
        internal bool _splitterStartFlag = true;
        internal double splitterMinValue = 0.2;
        internal double splitterMaxValue = 0.8;

        /// <summary>
        /// Update the split ratio value At the time of initialize alone.
        /// </summary>
        /// <param name="totalSize"></param>
        /// <param name="totalheight"></param>
        /// <param name="totalcount"></param>

        private void UpdateSplitRatio(Size totalSize, double totalheight, double totalcount)
        {
            double totoalheight = totalheight;
            double remainingheight = totalSize.Height - totoalheight;

            foreach (Control control in this.Children)
            {
                ChartArea area = control as ChartArea;
                if (this.Children.Count == 1)
                {
                    area.SplitRatio = 1;
                    break;
                }
                if (area != null && !double.IsNaN(area.Height) && area.flag == true)
                {
                    if (area.index == 0)
                    {
                        area.SplitRatio = area.Height / totalSize.Height;
                        area.flag = false;
                    }
                    else
                    {
                        _flag = true;
                    }
                    area.flag = false;
                }
                else if (area != null && area.isLoadedFirst)
                {
                    if (area.index == 0)
                    {
                        SyncChartAreas sArea = area.ChartAreaParent as SyncChartAreas;
                        double value = 0d;
                        if (sArea != null)
                        {
                            for (int i = 1; i < sArea.Areas.Count; i++)
                            {
                                //if (sArea.Areas[i].SplitpositionFlag == true)
                                //    value = sArea.Areas[i].SplitterPosition + value;
                            }
                            if (value != 0d)
                            {
                                value = (1 - value) / (sArea.Areas.Count - 1);
                            }
                            else
                            {
                                //value = area.SplitterPosition;
                            }
                            if (value < splitterMinValue)
                                value = splitterMinValue;
                            else if (value > splitterMaxValue)
                                value = splitterMaxValue;
                        }
                        //if (this.Children.Count != totalcount)
                        //    area.SplitRatio = area.SplitterPosition == 0d ? (remainingheight / totalSize.Height) / (this.Children.Count - totalcount) : area.SplitterPosition;
                    }
                }
                else
                {

                    if (area.index == 0)
                    {
                        if (!_splitterStartFlag)
                        {
                            SyncChartAreas sArea = area.ChartAreaParent as SyncChartAreas;
                            double value = 0d;
                            for (int i = 1; i < sArea.Areas.Count; i++)
                            {
                                //if (splitterPositionFlag == true)
                                //    value = sArea.Areas[i].SplitterPosition + value;
                            }
                            if (value != 0d)
                            {
                                value = (1 - value) / (sArea.Areas.Count - 1);
                                if (value < splitterMinValue)
                                    value = splitterMinValue;
                                else if (value > splitterMaxValue)
                                    value = splitterMaxValue;
                               // area.SplitterPosition = value;
                            }
                            double _value = 0d;

                            SyncChartAreas sArea1 = area.ChartAreaParent as SyncChartAreas;
                            for (int i = 1; i < sArea1.Areas.Count; i++)
                            {
                                //minvalue = sArea1.Areas[i].SplitterBottomSpace + sArea1.Areas[i].MinHeight + 10;

                                //double newheight = sArea1.Areas[i].SplitterPosition * sArea1.Areas[i].TotalSize.Height;

                                //if (newheight < minvalue)
                                //{
                                //    _value = sArea1.Areas[i].SplitterPosition = minvalue / sArea1.Areas[i].TotalSize.Height;
                                //    splitterMinValue = sArea1.Areas[i].SplitterPosition;
                                //}
                            }
                            if (_value != 0d)
                            {
                                //splitterMaxValue = area.SplitterPosition = _value = (1 - _value) / (sArea.Areas.Count - 1);
                            }

                        }

                    }
                }

            }

        }

    }


    /// <summary>
    /// Class implementation for SyncChartAreas
    /// </summary>
    [System.ComponentModel.DesignTimeVisible(false)]

    public class SyncChartAreas : ChartArea
    {
        internal bool areaht_modified = false;
        internal int? modifiedarea_idx = null;
        internal bool SyncInteractiveCursorMove = false;
        internal bool minimum = false;
        internal double value = 0d;
        internal double value1 = 0d;
        internal bool splitterFlag = false;
        //internal InteractiveCursorCollection interactiveCursors = new InteractiveCursorCollection();

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseLeave"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event. </param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);            
        }
       
        internal static readonly DependencyProperty CursorSeriesProperty = DependencyProperty.Register("CursorSeries", typeof(ChartSeries), typeof(SyncChartAreas), new PropertyMetadata(null));
        /// <summary>
        /// Gets or Sets value fro the Cursor Visibility
        /// </summary>
        internal ChartSeries CursorSeries
        {
            get { return (ChartSeries)GetValue(CursorSeriesProperty); }
            set { SetValue(CursorSeriesProperty, value); }
        }

        #region Dependancy Properties

        /// <summary>
        /// Identifies the Panning range, It is a dependencyProperty
        /// </summary>
        public static readonly DependencyProperty PanningRange_SyncProperty =
         DependencyProperty.RegisterAttached("PanningRange_Sync", typeof(DoubleRange), typeof(SyncChartAreas), new PropertyMetadata(DoubleRange.Empty));

        /// <summary>
        /// Identifies the Panning is set or not, It is a dependencyProperty
        /// </summary>
        public static readonly DependencyProperty IsPanning_SyncProperty =
          DependencyProperty.RegisterAttached("IsPanning_Sync", typeof(bool), typeof(SyncChartAreas), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the Margin, It is a dependencyProperty
        /// </summary>
        public new static readonly DependencyProperty MarginProperty =
 DependencyProperty.Register("Margin", typeof(Thickness), typeof(SyncChartAreas), new PropertyMetadata(new Thickness()));

        /// <summary>
        /// Identifies the Area, It is a dependencyProperty
        /// </summary>
        public static readonly DependencyProperty AreasProperty =
   DependencyProperty.Register("Areas", typeof(AreasCollection), typeof(SyncChartAreas), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the AreasPanel, It is a dependencyProperty
        /// </summary>
        public static readonly DependencyProperty AreasPanelProperty =
          DependencyProperty.Register("AreasPanel", typeof(ItemsPanelTemplate), typeof(SyncChartAreas), new PropertyMetadata(null, OnAreasPanelChanged));

        /// <summary>
        /// Identifies the SyncChartArea, It is a dependencyProperty
        /// </summary>
        internal static readonly DependencyProperty IsSyncChartAreaProperty =
            DependencyProperty.Register("IsSyncChartArea", typeof(bool), typeof(SyncChartAreas), new PropertyMetadata(false));

        //public static readonly DependencyProperty SplitterColorProperty =
        //    DependencyProperty.Register("SplitterColor", typeof(Brush), typeof(SyncChartAreas), new PropertyMetadata(Brushes.Gray));

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the panning range_ sync.
        /// </summary>
        /// <value>The panning range_ sync.</value>
        public DoubleRange PanningRange_Sync
        {
            get { return (DoubleRange)GetValue(PanningRange_SyncProperty); }
            set { SetValue(PanningRange_SyncProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is panning_ sync.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is panning_ sync; otherwise, <c>false</c>.
        /// </value>
        public bool IsPanning_Sync
        {
            get { return (bool)GetValue(IsPanning_SyncProperty); }
            set { SetValue(IsPanning_SyncProperty, value); }
        }
        /// <summary>
        /// Gets or sets the areas.
        /// </summary>
        /// <value>The areas.</value>
        public AreasCollection Areas
        {
            get { return (AreasCollection)GetValue(AreasProperty); }
            set { SetValue(AreasProperty, value); }
        }


        /// <summary>
        /// Gets or sets the outer margin of an element.  This is a dependency property.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// Provides margin values for the element. The default value is a <see cref="T:System.Windows.Thickness"/> with all properties equal to 0 (zero).
        /// </returns>
        public new Thickness Margin
        {
            get { return (Thickness)GetValue(MarginProperty); }
            set { SetValue(MarginProperty, value); }
        }


        /// <summary>
        /// Gets or sets the areas panel.
        /// </summary>
        /// <value>The areas panel.</value>
        public ItemsPanelTemplate AreasPanel
        {
            get
            {
                return (ItemsPanelTemplate)GetValue(AreasPanelProperty);
            }

            set
            {
                SetValue(AreasPanelProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is sync chart area.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is sync chart area; otherwise, <c>false</c>.
        /// </value>
        internal bool IsSyncChartArea
        {
            get
            {
                return (bool)GetValue(IsSyncChartAreaProperty);
            }
            set
            {
                SetValue(IsSyncChartAreaProperty, value);
            }
        }
        #endregion

        /// <summary>
        /// Initializes the <see cref="SyncChartAreas"/> class.
        /// </summary>
        /// <remarks>
        /// Primary and secondary axes are being created automatically.
        /// </remarks>
        static SyncChartAreas()
        {
            
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(SyncChartAreas), new FrameworkPropertyMetadata(typeof(SyncChartAreas)));

            //ItemsControl.ItemsSourceProperty.OverrideMetadata(typeof(SyncChartAreas), new FrameworkPropertyMetadata(null));
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="SyncChartAreas"/> class.
        /// </summary>
        /// <remarks>
        /// Primary and secondary axes are being created automatically.
        /// </remarks>
        public SyncChartAreas():base()
        {
                      
            Areas = new AreasCollection();
            Areas.CollectionChanged += new NotifyCollectionChangedEventHandler(Areas_CollectionChanged);
            this.IsSyncChartArea = true;
            this.DefaultStyleKey = typeof(SyncChartAreas);  
            //ResourceDictionary rd = new ResourceDictionary()
            //{
            //    Source = new Uri("/Syncfusion.Chart.Wpf;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
            //};

            //if (rd != null)
            //{
            //    this.Style = rd["syncChartArea"] as Style;
            //}
            
            //SetValue(SyncAreasPanel.OrientationProperty, Orientation.Vertical);
            this.Loaded += new RoutedEventHandler(SyncChartAreas_Loaded);
            //InteractiveCursors.CollectionChanged += new NotifyCollectionChangedEventHandler(SyncInteractiveCursorCollection_CollectionChanged);
            this.SizeChanged += new SizeChangedEventHandler(SyncChartAreas_SizeChanged);
        }

        /// <summary>
        /// To Perform Zooming Scrollbar visibility status
        /// </summary>
        internal new void IntializeZoomingScrollBarVisibility()
        {
            bool findhorizontal = false, findvertical = false;

            for (int i = 0; i < this.Areas.Count; i++)
            {
                if(i !=0)
                {
                    this.Areas[i].PrimaryAxis.ZoomFactor = this.Areas[0].PrimaryAxis.ZoomFactor;                    
                }
                if (this.Areas[i].PrimaryAxis.Orientation == Orientation.Horizontal && findhorizontal == false && this.Areas[i].PrimaryAxis.ZoomFactor < 1.0 && this.Areas[i].PrimaryAxis.EnableZooming)
                {
                    if (this.Areas[i].HorizontalBar != null && i == this.Areas.Count-1)
                    {
                        this.Areas[i].HorizontalBar.Visibility = Visibility.Visible;                       
                        findhorizontal = true;
                    }
                }
                else if (this.Areas[i].SecondaryAxis.Orientation == Orientation.Vertical && findvertical == false && this.Areas[i].SecondaryAxis.ZoomFactor < 1.0 && this.Areas[i].SecondaryAxis.EnableZooming)
                {
                    if (this.Areas[i].VerticalBar != null)
                    {
                        this.Areas[i].VerticalBar.Visibility = Visibility.Collapsed;
                        findvertical = true;
                    }
                }

                if (findhorizontal == true && findvertical == true)
                {
                    break;
                }
            }

            if (findhorizontal == false)
            {
                if (this.HorizontalBar != null)
                {
                    this.HorizontalBar.Visibility = Visibility.Collapsed;
                }
            }

            if (findvertical == false)
            {
                if (this.VerticalBar != null)
                {
                    this.VerticalBar.Visibility = Visibility.Collapsed;
                }
            }

            if (this.ZoomTool != null)
            {
                int j;
                for (j = 0; j < this.Axes.Count; j++)
                {
                    if (this.Axes[j].EnableZooming == true)
                    {
                        this.isZoomactivated = true;
                        break;
                    }
                }

                if (j == this.Axes.Count)
                {
                    this.isZoomactivated = false;
                }
            }
        }
       
        internal ResourceDictionary tempalteRD = null;
        void SyncChartAreas_Loaded(object sender, RoutedEventArgs e)        
        {
            foreach (ChartArea area in this.Areas)
            {
                area.PrimaryAxis.RangeChanged += new PropertyChangedCallback(PrimaryAxis_RangeChanged);
            }
            //this.DefaultStyleKey = typeof(SyncChartAreas);   
            //if (tempalteRD == null)
            //{
            //    tempalteRD = new SharedResourceDictionary()
            //    {
            //        Source = new Uri("/Syncfusion.Chart.Silverlight;component/Themes/generic.xaml", UriKind.RelativeOrAbsolute)
            //    };
            //}

            //if (tempalteRD != null)
            //{
            //    this.Style = tempalteRD["syncChartArea"] as Style;
            //}
            if (this.PrimaryAxis.IsAutoSetRange)
            {
                foreach (ChartArea area in this.Areas)
                {

                }
            }
            //SyncInteractiveCursor_Changed();

            //Binding Properties for Splitter
        }

        void PrimaryAxis_RangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //this.SetPrimaryAxisRange();
            //this.SetAreaProperties();
        }

        void SyncChartAreas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SyncChartAreas sarea = sender as SyncChartAreas;           
        }

        
        #region Events

        internal bool isCustomPanel = false;
        private static void OnAreasPanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            SyncChartAreas area = d as SyncChartAreas;
            if (area != null)
            {
                area.isCustomPanel = true;
            }
        }
        internal void SetEnableZoomingForYAxis(ChartArea area)
        {
            if (area != null)
            {
                var axes = (from axis in area.Axes
                            where axis.Orientation == Orientation.Vertical
                            select axis).ToList<ChartAxis>();

                foreach (var item in axes)
                {
                    item.EnableZooming = false;
                }
            }
        }

        void Areas_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            AreasCollection areas = sender as AreasCollection;
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if(!(e.OldItems[0] as ChartArea).isResizable)
                {
                    if (areas.Count > 0)
                    {
                        areas[areas.Count - 1].isResizable = false;
                        areas[areas.Count - 1].rb.Detach();
                    }
                }
                this.SetAreaProperties();
                foreach (ChartArea area in areas)
                {
                    area.LoadArea();
                }
                
            }
            if (areas != null && e.NewItems != null)
            {
                foreach (var item in e.NewItems.OfType<ChartArea>())
                {                    
                        item.hasMinWidth = false;
                        item.hasMinWidthInLeft = false;
                        item.hasMinWidthInRight = false;
                        this.SetAreaProperties();
                        item.LoadArea();
                        item.Series.CollectionChanged += new NotifyCollectionChangedEventHandler(Series_CollectionChanged);                    
                }
            }
            else if (areas != null)
            {
                this.SetAreaProperties();
            }
        }

        private void Series_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ChartSeries series in e.NewItems)
                {
                    series.IsIndexed = false;
                    Syncfusion.Windows.Chart.Chart parent = base.Parent as Syncfusion.Windows.Chart.Chart;

                    if (series.YAxis != null)
                    {
                        if (series.YAxis.Area != null)
                        {
                            if (series.YAxis.Area.index != 0)
                            {                               
                            }
                        }
                    }                    
                }
            }
            if (e.OldItems != null)
            {
                Syncfusion.Windows.Chart.Chart parent = base.Parent as Syncfusion.Windows.Chart.Chart;
                //parent..m_internalSeriesList.Clear();
            }

        }
        void item_AreaPresenterEvent(object sender)
        {
            ChartArea area = sender as ChartArea;            
        }

        #endregion

        #region Methods


        void ChartLegendBinding(object source, PropertyPath path, BindingMode mode, DependencyObject target, DependencyProperty targetProperty)
        {
            Binding binding = new Binding();
            binding.Source = source;
            binding.Path = path;
            binding.Mode = mode;
            BindingOperations.SetBinding(target, targetProperty, binding);
        }

       

        /// <summary>
        /// Sets the area properties.
        /// </summary>
        internal void SetAreaProperties()
        {

            if (this.Areas == null)
                return;

            if (this.Areas != null)
            {

               
                this.IsSync = true;
                
                foreach (ChartArea area in this.Areas)
                {
                    area.BorderBrush = new SolidColorBrush(Colors.Transparent);
                    area.Padding = new Thickness(0, 10, 0, 0);
                    area.IsSync = true;
                    area.index = this.Areas.IndexOf(area);
                    area.updated = true;
                    //if (this.Areas.IndexOf(area) == 0)
                    //{
                    //    area.PrimaryAxis.ZoomFactor = this.Areas[0].PrimaryAxis.ZoomFactor;
                    //    this.PrimaryAxis.ZoomFactor = this.Areas[0].PrimaryAxis.ZoomFactor;
                    //}
                    //if (this.Areas.IndexOf(area) != 0)
                    //{
                    //    area.PrimaryAxis.ZoomInterval = this.Areas[0].PrimaryAxis.ZoomInterval;
                    //    this.PrimaryAxis.ZoomInterval = this.Areas[0].PrimaryAxis.ZoomInterval;
                    //    area.PrimaryAxis.ZoomPosition = this.Areas[0].PrimaryAxis.ZoomPosition;
                    //    this.PrimaryAxis.ZoomPosition = this.Areas[0].PrimaryAxis.ZoomPosition;
                    //    area.PrimaryAxis.ZoomRange = this.Areas[0].PrimaryAxis.ZoomRange;
                    //    this.PrimaryAxis.ZoomRange = this.Areas[0].PrimaryAxis.ZoomRange;
                    //    area.PrimaryAxis.ZoomVisibleRange = this.Areas[0].PrimaryAxis.ZoomVisibleRange;
                    //    this.PrimaryAxis.ZoomVisibleRange = this.Areas[0].PrimaryAxis.ZoomVisibleRange;
                    //    area.PrimaryAxis.ZoomVisisbleInterval = this.Areas[0].PrimaryAxis.ZoomVisisbleInterval;
                    //    this.PrimaryAxis.ZoomVisisbleInterval = this.Areas[0].PrimaryAxis.ZoomVisisbleInterval;
                    //}
                    Clone(area.PrimaryAxis, this.PrimaryAxis);                    
                }
                this.Series.Clear();
                foreach (ChartArea area in this.Areas)
                {                   
                    //This condition is to show the primary axis when only one area is added to syncChart.
                    if (this.Areas.Count == 1)
                    {
                        area.PrimaryAxis.AxisVisibility = Visibility.Visible;
                    }
                    else if (this.Areas[this.Areas.Count - 1] != area)
                    {
                        area.PrimaryAxis.AxisVisibility = Visibility.Collapsed;
                        //if (area != null && VisualTreeHelper.GetChildrenCount(area.m_areaPresenter) > 3)
                        //{
                        //    DependencyObject obj = VisualTreeHelper.GetChild(area.m_areaPresenter, 0);
                        //    obj = VisualTreeHelper.GetChild(obj, 1);
                        //    obj = VisualTreeHelper.GetChild(obj, 3);
                        //    if (obj is ChartZoomingScrollBar)
                        //    {
                        //        (obj as ChartZoomingScrollBar).Visibility = System.Windows.Visibility.Collapsed;
                        //        (obj as ChartZoomingScrollBar).Height = 0d;
                        //    }
                        //}
                    }
                    area.PrimaryAxis.InvalidateMeasure();
                    area.PrimaryAxis.InvalidateArrange();
                    area.IsContextMenuEnabled = false;
                    area.ChartAreaParent = this;                   
                    area.IsContextMenuEnabled = this.IsContextMenuEnabled;
                    area.IsBeginInit = this.IsBeginInit;                   
                    foreach (ChartSeries ser in area.Series)
                    {
                        this.Series.Add(ser);
                    }
                    Binding binding = new Binding();
                    binding.Path = new PropertyPath("DataContext");
                    binding.Source = this;
                    BindingOperations.SetBinding(area, ChartArea.DataContextProperty, binding);                   
                }
            }

        }

        /// <summary>
        /// Invoke to render chart Area.
        /// </summary>
        public override void OnApplyTemplate()
        {
            //foreach (ChartArea area in this.Areas)
            //{
            //    // Grid g = GetTemplateChild("areagrid") as Grid;
            //    ResizeBehavior rb = new ResizeBehavior();
            //    rb.IsBottomDraggable = true;
            //    rb.MinHeight = 20;
            //    rb.MaxHeight = 450;
            //    rb.Attach(area);
            //}
            base.OnApplyTemplate();
        }

        internal void SetPrimaryAxisRange()
        {

            var startRange = (from item in this.Areas
                              where item.PrimaryAxis != null && item != null && item.PrimaryAxis.VisibleRange.Start != double.NaN
                              orderby item.PrimaryAxis.VisibleRange.Start ascending
                              select item.PrimaryAxis.VisibleRange.Start).ToList<double>();


            var endRange = (from item in this.Areas
                            where item.PrimaryAxis != null && item != null && item.PrimaryAxis.VisibleRange.End != double.NaN
                            orderby item.PrimaryAxis.VisibleRange.End descending
                            select item.PrimaryAxis.VisibleRange.End).ToList<double>();


            if (startRange.Count() > 0 && endRange.Count() > 0)
            {
                DoubleRange range = new DoubleRange(startRange.ElementAt(0), endRange.ElementAt(0));
                //1 line comm
                this.PrimaryAxis.VisibleRange = range;
            }

        }


        /// <summary>
        /// Clones the specified axis.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="parentAxis">The parent axis.</param>
        /// <returns></returns>
        public object Clone(ChartAxis axis, ChartAxis parentAxis)
        {

            
            axis.AllowDrop = parentAxis.AllowDrop;
            axis.AxisVisibility = parentAxis.AxisVisibility;
            axis.ContentPath = parentAxis.ContentPath;
            axis.DataContext = parentAxis.DataContext;
            axis.DateTimeInterval = parentAxis.DateTimeInterval;
            axis.DateTimeRange = parentAxis.DateTimeRange;
            axis.DesiredIntervalsCount = parentAxis.DesiredIntervalsCount;
            axis.EdgeLabelsDrawingMode = parentAxis.EdgeLabelsDrawingMode;
            axis.EnableZooming = parentAxis.EnableZooming;          
            axis.Header = parentAxis.Header;
            axis.HeaderPosition = parentAxis.HeaderPosition;           
            axis.HidePartialLabel = parentAxis.HidePartialLabel;
            axis.IgnoreRangePaddingsOnZoom = parentAxis.IgnoreRangePaddingsOnZoom;
            axis.IntersectAction = parentAxis.IntersectAction;
            axis.Interval = parentAxis.Interval;          
            axis.IsAutoSetRange = parentAxis.IsAutoSetRange;
            axis.IsInversed = parentAxis.IsInversed;
            axis.IsLogarithmic = parentAxis.IsLogarithmic;          
            axis.LabelBackground = parentAxis.LabelBackground;
            axis.LabelBorderBrush = parentAxis.LabelBorderBrush;
            axis.LabelBorderThickness = parentAxis.LabelBorderThickness;
            axis.LabelCornerRadius = parentAxis.LabelCornerRadius;
            axis.LabelDateTimeFormat = parentAxis.LabelDateTimeFormat;
            axis.LabelFontFamily = parentAxis.LabelFontFamily;
            axis.LabelFontSize = parentAxis.LabelFontSize;
            axis.LabelFontWeight = parentAxis.LabelFontWeight;
            axis.LabelForeground = parentAxis.LabelForeground;
            axis.LabelFormat = parentAxis.LabelFormat;
            axis.LabelRotateAngle = parentAxis.LabelRotateAngle;            
            axis.LabelsSource = parentAxis.LabelsSource;
            axis.LabelTemplate = parentAxis.LabelTemplate;
            axis.Language = parentAxis.Language;
            axis.LineStroke = parentAxis.LineStroke;
            axis.LogarithmicBase = parentAxis.LogarithmicBase;
            axis.VisibleInterval = parentAxis.VisibleInterval;
            axis.AxisLabels = parentAxis.AxisLabels;
           axis.IsFractionEnabledOnZoom= parentAxis.IsFractionEnabledOnZoom;
            axis.ValueType = parentAxis.ValueType;
            axis.Range = parentAxis.Range;          
            axis.ZoomFactor = parentAxis.ZoomFactor;
            axis.RangeCalculationMode = parentAxis.RangeCalculationMode;
            axis.RangePadding = parentAxis.RangePadding;
            axis.SmallTickSize = parentAxis.SmallTickSize;
            axis.SmallTicksPerInterval = parentAxis.SmallTicksPerInterval;
            axis.TickLineStroke = parentAxis.TickLineStroke;           
            axis.TickSize = parentAxis.TickSize;
            axis.SetValue(ChartAxis.ShowGridLinesProperty, parentAxis.GetValue(ChartAxis.ShowGridLinesProperty));
            axis.SetValue(ChartAxis.GridLineStrokeProperty, parentAxis.GetValue(ChartAxis.GridLineStrokeProperty));
           
            return axis;

        }

        #endregion
    }

    
   
}
