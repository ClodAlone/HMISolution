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
using System.Collections.Specialized;
using System.Linq;

#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Diagnostics;
using System.IO;
using System.Collections.ObjectModel;
#if SILVERLIGHT_UNCOMMON
using System.Windows.Printing;
#endif
#else
using Windows.ApplicationModel;
using Windows.UI.Core;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
#endif
#if NETFX_CORE8_1
using Windows.Storage.Pickers;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    public abstract partial class ChartBase : Control, ICloneable
    {
        #region fields
        /// <summary>
        /// Intermediate PrimaryAxis object used for internal calculation
        /// </summary>
        internal ChartAxis InternalPrimaryAxis { get; set; }
        /// <summary>
        /// Intermediate SecondaryAxis object used for internal calculation
        /// </summary>
        internal ChartAxis InternalSecondaryAxis { get; set; }

#if WINDOWS_PHONE
       internal bool isUpdateDispatched = false;
#else
        internal IAsyncAction updateAreaAction;
#endif

        internal bool isLoaded;

        internal Canvas AdorningCanvas;

        internal ChartDockPanel ChartDockPanel { get; set; }

        private ChartRowDefinitions rowDefinitions;

        internal bool ShowTooltip = false;

        private ChartColumnDefinitions columnDefinitions;

        private ILayoutCalculator gridLinesLayout;

        private Rect seriesClipRect;

        private ILayoutCalculator chartAxisLayoutPanel;

        internal bool IsTemplateApplied = false;

        private double m_minPointsDelta = double.NaN;

        internal bool IsUpdateLegend = false;

        private Size? rootPanelDesiredSize;

        private ChartAreaType areaType = ChartAreaType.CartesianAxes;

#if NETFX_CORE || SILVERLIGHT_UNCOMMON || WPF
        protected Printing Printing;
#endif
#if NETFX_CORE8_1
        const double imageResolution = 96.0;
#endif

        internal List<ChartSeriesBase> ActualSeries = new List<ChartSeriesBase>();

        #endregion

        #region events

        /// <summary>
        /// occurs when selection changed
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public event EventHandler<ChartSelectionChangedEventArgs> SelectionChanged;

        /// <summary>
        /// Occurs when [series bounds changed].
        /// </summary>
        public event EventHandler<ChartSeriesBoundsEventArgs> SeriesBoundsChanged;

        #endregion

        #region properties

        /// <summary>
        /// Get or Set AxisThickness property
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Thickness AxisThickness
        {
            get { return (Thickness)GetValue(AxisThicknessProperty); }
            internal set { SetValue(AxisThicknessProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AxisThickness.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AxisThicknessProperty =
            DependencyProperty.Register("AxisThickness", typeof(Thickness), typeof(ChartBase), new PropertyMetadata(new Thickness(0)));

        internal bool SBSInfoCalculated //sbs - sidebyside
        {
            get;
            set;
        }

        /// <summary>
        /// return int value from the given ChartAxis
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [ClassReference(IsReviewed = false)]
        public static int GetRow(UIElement obj)
        {
            return (int)obj.GetValue(RowProperty);
        }

        /// <summary>
        /// Return actual row value from the given ChartAxis
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [ClassReference(IsReviewed = false)]
        internal int GetActualRow(UIElement obj)
        {
            var actualPos = RowDefinitions.Count;
            var pos = GetRow(obj);
            var result = pos >= actualPos ? actualPos - 1 : (pos < 0 ? 0 : pos);
            return result < 0 ? 0 : result;
        }

        /// <summary>
        /// Method implementation for Set Row value to ChartAxis
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        [ClassReference(IsReviewed = false)]
        public static void SetRow(UIElement obj, int value)
        {
            obj.SetValue(RowProperty, value);
        }


        /// <summary>
        ///  Using a DependencyProperty as the backing store for Row.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RowProperty =
            DependencyProperty.RegisterAttached("Row", typeof(int), typeof(ChartBase), new PropertyMetadata(0));

        /// <summary>
        /// Return int value from the given ChartAxis
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [ClassReference(IsReviewed = false)]
        public static int GetColumn(UIElement obj)
        {
            return (int)obj.GetValue(ColumnProperty);
        }

        /// <summary>
        /// Gets the value of the Syncfusion.UI.Xaml.Charts.ColumnSpan attached property from a given UIElement. 
        /// </summary>
        /// <param name="element">The element from which to read the property value.</param>
        /// <returns>The value of the Syncfusion.UI.Xaml.Charts.ColumnSpan attached property.</returns>
        [ClassReference(IsReviewed = false)]
        public static int GetColumnSpan(UIElement element)
        {
            return (int)element.GetValue(ColumnSpanProperty);
        }

        /// <summary>
        /// Gets the value of the Syncfusion.UI.Xaml.Charts.RowSpan attached property from a given UIElement.
        /// </summary>
        /// <param name="element">The element from which to read the property value.</param>
        /// <returns>The value of the Syncfusion.UI.Xaml.Charts.RowSpan attached property.</returns>
        [ClassReference(IsReviewed = false)]
        public static int GetRowSpan(UIElement element)
        {
            return (int)element.GetValue(RowSpanProperty);
        }

        /// <summary>
        /// Return actual column value from the given ChartAxis
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [ClassReference(IsReviewed = false)]
        internal int GetActualColumn(UIElement obj)
        {
            var actualPos = ColumnDefinitions.Count;
            var pos = GetColumn(obj);
            var result = pos >= actualPos ? actualPos - 1 : (pos < 0 ? 0 : pos);
            return result < 0 ? 0 : result;
        }

        /// <summary>
        /// Gets the actual value of the Syncfusion.UI.Xaml.Charts.ColumnSpan attached property from a given UIElement.
        /// </summary>
        /// <param name="element">The element from which to read the property value.</param>
        /// <returns>The value of the Syncfusion.UI.Xaml.Charts.ColumnSpan attached property.</returns>
        [ClassReference(IsReviewed = false)]
        internal int GetActualColumnSpan(UIElement element)
        {
            var count = ColumnDefinitions.Count;
            var span = GetColumnSpan(element);
            return span > count ? count : (span < 0 ? 0 : span);
        }

        /// <summary>
        /// Gets the actual value of the Syncfusion.UI.Xaml.Charts.RowSpan attached property from a given UIElement.
        /// </summary>
        /// <param name="element">The element from which to read the property value.</param>
        /// <returns>The value of the Syncfusion.UI.Xaml.Charts.RowSpan attached property.</returns>
        [ClassReference(IsReviewed = false)]
        internal int GetActualRowSpan(UIElement obj)
        {
            var count = RowDefinitions.Count;
            var span = GetRowSpan(obj);
            return span > count ? count : (span < 0 ? 0 : span);
        }

        /// <summary>
        /// Set column to ChartAxis
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        [ClassReference(IsReviewed = false)]
        public static void SetColumn(UIElement obj, int value)
        {
            obj.SetValue(ColumnProperty, value);
        }

        /// <summary>
        /// Sets the value of the Syncfusion.UI.Xaml.Charts.ColumnSpan attached property
        //     to a given UIElement.
        /// </summary>
        /// <param name="element"> The element on which to set the Syncfusion.UI.Xaml.Charts.ColumnSpan attached property.</param>
        /// <param name="value">The property value to set.</param>
        [ClassReference(IsReviewed = false)]
        public static void SetColumnSpan(UIElement element, int value)
        {
            element.SetValue(ColumnSpanProperty, value);
        }

        /// <summary>
        /// Sets the value of the Syncfusion.UI.Xaml.Charts.RowSpan attached property
        //     to a given UIElement.
        /// </summary>
        /// <param name="element"> The element on which to set the Syncfusion.UI.Xaml.Charts.RowSpan attached property.</param>
        /// <param name="value">The property value to set.</param>
        [ClassReference(IsReviewed = false)]
        public static void SetRowSpan(UIElement element, int value)
        {
            element.SetValue(RowSpanProperty, value);
        }


        /// <summary>
        ///  Using a DependencyProperty as the backing store for Column.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ColumnProperty =
            DependencyProperty.RegisterAttached("Column", typeof(int), typeof(ChartBase), new PropertyMetadata(0));

        /// <summary>
        ///  Using a DependencyProperty as the backing store for ColumnSpan.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ColumnSpanProperty =
            DependencyProperty.RegisterAttached("ColumnSpan", typeof(int), typeof(ChartBase), new PropertyMetadata(1,OnSpanChanged));

        private static void OnSpanChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ChartAxis && (d as ChartAxis).Area != null)
            (d as ChartAxis).Area.ScheduleUpdate();
            else if (d is ChartLegend && (d as ChartLegend).XAxis != null && (d as ChartLegend).XAxis.Area != null)
            (d as ChartLegend).XAxis.Area.ScheduleUpdate();
        }

        
        /// <summary>
        ///  Using a DependencyProperty as the backing store for RowSpan.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RowSpanProperty =
            DependencyProperty.RegisterAttached("RowSpan", typeof(int), typeof(ChartBase), new PropertyMetadata(1, OnSpanChanged));

        internal Size? RootPanelDesiredSize
        {
            get { return rootPanelDesiredSize; }
            set
            {
                if (rootPanelDesiredSize == value) return;
                rootPanelDesiredSize = value;

                OnRootPanelSizeChanged(value != null ? value.Value : new Size());
            }
        }

        /// <summary>
        /// Get a bounding rectangle of chart excluding axis and chart header..
        /// </summary>
        public Rect SeriesClipRect
        {
            get
            {
                return seriesClipRect;
            }
            internal set
            {
                if (seriesClipRect == value) return;
                var oldRect = seriesClipRect;
                seriesClipRect = value;
                OnSeriesBoundsChanged(new ChartSeriesBoundsEventArgs { OldBounds = oldRect, NewBounds = value });
            }
        }

        protected internal virtual void OnSeriesBoundsChanged(ChartSeriesBoundsEventArgs args)
        {
            if (SeriesBoundsChanged != null && args != null)
                SeriesBoundsChanged(this, args);
        }

        internal Size AvailableSize
        {
            get;
            set;
        }

        internal UpdateAction UpdateAction
        {
            get;
            set;
        }

        /// <summary>
        /// Contains the radius for each PieSeries in the chart area.
        /// </summary>
        internal double[] CircularSegmentRadius
        {
            get;
            set;
        }

        internal Dictionary<object, StackingValues> StackedValues { get; set; }

        private Dictionary<object, int> seriesPosition = new Dictionary<object, int>();

        internal Dictionary<object, int> SeriesPosition
        {
            get { return seriesPosition; }
            set { seriesPosition = value; }
        }

        internal int[,] SbsSeriesCount   //sbs  - sidebyside
        {
            get;
            set;
        }

        /// <summary>
        /// Calculates the minimum delta value
        /// </summary>
        internal double MinPointsDelta
        {
            get
            {
                m_minPointsDelta = double.MaxValue;

                foreach (var xValues in from series in VisibleSeries let xValues = series.ActualXValues as List<double> where !series.IsIndexed && xValues != null select xValues)
                {
                    for (var i = 1; i < xValues.Count; i++)
                    {
                        var delta = xValues[i] - xValues[i - 1];
                        if (delta != 0)
                        {
                            m_minPointsDelta = Math.Min(m_minPointsDelta, delta);
                        }
                    }
                }
                m_minPointsDelta = ((m_minPointsDelta == double.MaxValue || m_minPointsDelta >= 1 || m_minPointsDelta < 0) ? 1 : m_minPointsDelta);

                return m_minPointsDelta;
            }
        }

        /// <summary>
        /// Gets visible series of chart area.
        /// </summary>
        /// <remarks>
        /// This property is intended to be used for custom <see>
        /// <cref>ChartArea</cref>
        /// </see>
        /// templates.
        /// </remarks>
        [ClassReference(IsReviewed = false)]
        public ChartVisibleSeriesCollection VisibleSeries
        {
            get { return (ChartVisibleSeriesCollection)GetValue(VisibleSeriesProperty); }
            internal set { SetValue(VisibleSeriesProperty, value); }
        }


        /// <summary>
        /// Using a DependencyProperty as the backing store for VisibleSeries.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty VisibleSeriesProperty =
            DependencyProperty.Register("VisibleSeries", typeof(ChartVisibleSeriesCollection), typeof(ChartBase), new PropertyMetadata(null));

        internal ILayoutCalculator GridLinesLayout
        {
            get { return gridLinesLayout; }
            set { gridLinesLayout = value; }
        }

        /// <summary>
        /// Gets or sets the type of the area.
        /// </summary>
        /// <value>
        /// The type of the area.
        /// </value>
        internal ChartAreaType AreaType
        {
            get
            {
                return areaType;
            }
            set
            {
                if (areaType == value) return;
                areaType = value;
                OnAreaTypeChanged();
            }
        }

        /// <summary>
        /// Gets or Sets ChartPalette. By default, it is Metro.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public ChartColorPalette Palette
        {
            get { return (ChartColorPalette)GetValue(PaletteProperty); }
            set { SetValue(PaletteProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Pallete.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PaletteProperty =
            DependencyProperty.Register("Palette", typeof(ChartColorPalette), typeof(ChartBase), new PropertyMetadata(ChartColorPalette.Metro, OnPaletteChanged));


        /// <summary>
        /// Gets the collection of ChartColumnDefinition objects defined in Chart.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public ChartColumnDefinitions ColumnDefinitions
        {
            get
            {
                if (columnDefinitions != null) return columnDefinitions;
                columnDefinitions = new ChartColumnDefinitions();
                columnDefinitions.CollectionChanged += OnRowColChanged;

                return columnDefinitions;
            }
            set
            {
                if (columnDefinitions != null)
                {
                    columnDefinitions.CollectionChanged -= OnRowColChanged;
                }
                columnDefinitions = value;
                if (columnDefinitions != null)
                {
                    columnDefinitions.CollectionChanged -= OnRowColChanged;
                }
                ScheduleUpdate();
            }
        }

        /// <summary>
        /// Gets or sets the chart axis layout panel.
        /// </summary>
        /// <value>
        /// The chart axis layout panel.
        /// </value>
        internal ILayoutCalculator ChartAxisLayoutPanel
        {
            get
            {
                return chartAxisLayoutPanel;
            }
            set
            {
                chartAxisLayoutPanel = value;
            }
        }

        /// <summary>
        /// Gets the collection of ChartRowDefinition objects defined in Chart
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public ChartRowDefinitions RowDefinitions
        {
            get
            {
                if (rowDefinitions != null) return rowDefinitions;
                rowDefinitions = new ChartRowDefinitions();
                rowDefinitions.CollectionChanged += OnRowColChanged;

                return rowDefinitions;
            }
            set
            {
                if (rowDefinitions != null)
                {
                    rowDefinitions.CollectionChanged -= OnRowColChanged;
                }
                rowDefinitions = value;
                if (rowDefinitions != null)
                {
                    rowDefinitions.CollectionChanged += OnRowColChanged;
                }
                ScheduleUpdate();
            }
        }

        /// <summary>
        /// Gets the collection of horizontal and vertical axis
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public ChartAxisCollection Axes { get; internal set; }

        /// <summary>
        /// To hold the Current Tooltip object, which is displaying in Chart
        /// </summary>
        internal ChartTooltip Tooltip
        {
            get { return (ChartTooltip)GetValue(TooltipProperty); }
            set { SetValue(TooltipProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Tooltip.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty TooltipProperty =
            DependencyProperty.Register("Tooltip", typeof(ChartTooltip), typeof(ChartBase), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets a value indicating whether ChartSeries added to area should be plotted side-by-side.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public bool SideBySideSeriesPlacement
        {
            get { return (bool)GetValue(SideBySideSeriesPlacementProperty); }
            set { SetValue(SideBySideSeriesPlacementProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SideBySideSeriesPlacement.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SideBySideSeriesPlacementProperty =
            DependencyProperty.Register("SideBySideSeriesPlacement", typeof(bool), typeof(ChartBase), new PropertyMetadata(true, OnSideBySideSeriesPlacementProperty));

        private static void OnSideBySideSeriesPlacementProperty(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ((ChartBase)d).ScheduleUpdate();
        }

        /// <summary>
        /// Gets or Sets title for chart
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }


        /// <summary>
        /// Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(ChartBase), new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets ChartColorModel for entire chart
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public ChartColorModel ColorModel
        {
            get { return (ChartColorModel)GetValue(ColorModelProperty); }
            set { SetValue(ColorModelProperty, value); }
        }


        /// <summary>
        ///  Using a DependencyProperty as the backing store for ColorModel.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ColorModelProperty =
            DependencyProperty.Register("ColorModel", typeof(ChartColorModel), typeof(ChartBase), new PropertyMetadata(null, OnColorModelChanged));

        #endregion

        #region callBack

        private static void OnColorModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (((ChartBase)d).ColorModel != null)
                ((ChartBase)d).ColorModel.Palette = ((ChartBase)d).Palette;
        }

        private static void OnPaletteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ChartBase)d).OnPaletteChanged(e);
        }

        private void OnPaletteChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ColorModel != null)
                ColorModel.Palette = Palette;
            else
                ColorModel = new ChartColorModel(Palette);
        }

        /// <summary>
        /// Called when [root panel size changed].
        /// </summary>
        /// <param name="size">The size.</param>
        protected virtual void OnRootPanelSizeChanged(Size size)
        {
            if (!IsTemplateApplied || !RootPanelDesiredSize.HasValue) return;
            UpdateAction |= UpdateAction.LayoutAndRender;

            UpdateArea(true);
        }

        internal Canvas GetAdorningCanvas()
        {
            return AdorningCanvas;
        }

        #endregion

        #region methods

        /// <summary>
        /// This method will suspend all the series from updating the data till ResumeNotification is called. This is specifically used when we need to append collection of datas.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public void SuspendSeriesNotification()
        {
            if (ActualSeries != null)
                foreach (ChartSeriesBase series in this.ActualSeries)
                {
                    series.SuspendNotification();
                }
        }

        /// <summary>
        /// Processes the data that is added to data source after SuspendSeriesNotification.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public void ResumeSeriesNotification()
        {
            if (ActualSeries != null)
                foreach (ChartSeriesBase series in this.ActualSeries)
                {
                    series.ResumeNotification();
                }
        }

        internal int GetSeriesIndex(ChartSeriesBase series)
        {
            return ActualSeries.IndexOf(series);
        }

        /// <summary>
        /// Clone the entire chart
        /// </summary>
        internal virtual DependencyObject CloneChart()
        {
            return null;
        }

        /// <summary>
        /// Update the chart area
        /// </summary>
        internal virtual void UpdateArea(bool forceUpdate)
        {

        }

        /// <summary>
        /// Converts Value to point.
        /// </summary>
        /// <param name="axis">The Chart axis .</param>
        /// <param name="value">The value.</param>
        /// <returns>The double value to point</returns>
        [ClassReference(IsReviewed = false)]
        public virtual double ValueToPoint(ChartAxis axis, double value)
        {
            if (axis != null)
            {
                if (axis.Orientation == Orientation.Horizontal)
                {
                    if (axis.ActualWidth == 0)
                        return axis.ValueToCoefficientCalc(value) * axis.Area.SeriesClipRect.Width;
                    return (axis.RenderedRect.Left - axis.Area.SeriesClipRect.Left)
                        + (axis.ValueToCoefficientCalc(value) * axis.RenderedRect.Width);
                }
                return (axis.RenderedRect.Top - axis.Area.SeriesClipRect.Top) + (1 - axis.ValueToCoefficientCalc(value)) * axis.RenderedRect.Height;
            }
            return double.NaN;
        }

        /// <summary>
        /// Called when Selection changed in sfchart
        /// </summary>
        /// <param name="eventArgs"></param>
        protected internal virtual void OnSelectionChanged(ChartSelectionChangedEventArgs eventArgs)
        {
            if (SelectionChanged != null && eventArgs != null)
                SelectionChanged(this, eventArgs);
        }


        /// <summary>
        /// Converts point to value.
        /// </summary>
        /// <param name="axis">The axis value.</param>
        /// <param name="point">The point.</param>
        /// <returns>The double point to value</returns>
        [ClassReference(IsReviewed = false)]
        public virtual double PointToValue(ChartAxis axis, Point point)
        {
            if (axis != null)
            {
                if (axis.Orientation == Orientation.Horizontal)
                {
                    return axis.CoefficientToValueCalc((point.X - (axis.RenderedRect.Left - axis.Area.SeriesClipRect.Left)) / axis.RenderedRect.Width);
                }
                return axis.CoefficientToValueCalc(1d - ((point.Y - (axis.RenderedRect.Top - axis.Area.SeriesClipRect.Top)) / axis.RenderedRect.Height));
            }
            return double.NaN;
        }

        /// <summary>
        /// Converts Value to Log point.
        /// </summary>
        /// <param name="axis">The Logarithmic axis .</param>
        /// <param name="value">The value.</param>
        /// <returns>The double value to point</returns>
        [ClassReference(IsReviewed = false)]
        internal virtual double ValueToLogPoint(ChartAxis axis, double value)
        {
            return double.NaN;
        }

        /// <summary>
        /// Updates the entire chart series and axis
        /// </summary>
        internal virtual void UpdateAxisLayoutPanels()
        {

        }

        /// <summary>
        /// Clone the entire chart control
        /// </summary>
        public DependencyObject Clone()
        {
            return CloneChart();
        }

        internal void ScheduleUpdate()
        {

#if WINDOWS_PHONE
            if (!isUpdateDispatched)
            {
#if WPF 
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(UpdateArea));
#else
                Dispatcher.BeginInvoke(UpdateArea);
#endif
                isUpdateDispatched = true;
            }
#else
            var _isInDesignMode = DesignMode.DesignModeEnabled;
            if (updateAreaAction == null && !_isInDesignMode)
            {
                updateAreaAction = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, UpdateArea);
            }
            else if (_isInDesignMode)
                UpdateArea(true);
#endif
        }

        private void OnAreaTypeChanged()
        {
            UpdateAxisLayoutPanels();
        }

        internal void UpdateArea()
        {
            UpdateArea(false);
        }

        /// <summary>
        /// Returns the stacked value of the series.
        /// </summary>
        /// <param name="series">ChartSeries</param>
        /// <param name="reqNegStack">RequiresNegativeStack</param>
        /// <returns>StackedYValues collection</returns>
        [ClassReference(IsReviewed = false)]
        public List<double> GetCumulativeStackInfo(ChartSeriesBase series, bool reqNegStack)
        {
            if (series != null)
            {
                var y = series.ActualYAxis.Origin;
                double currtY;
                var calcYValues = new List<double>();

                foreach (var ser in VisibleSeries)
                {
                    var yValues = ((XyDataSeries)ser).YValues;
                    if (ser.ActualXValues != null)
                    {
                        if (calcYValues.Count > 0)
                        {
                            for (var i = 0; i < ser.DataCount; i++)
                            {
                                currtY = reqNegStack ? Math.Abs(yValues[i]) : yValues[i];
                                if (i < calcYValues.Count)
                                    calcYValues[i] += currtY + y;
                                else
                                    calcYValues.Add(currtY + y);
                            }
                        }
                        else
                        {
                            for (var i = 0; i < ser.DataCount; i++)
                            {
                                currtY = reqNegStack ? Math.Abs(yValues[i]) : yValues[i];
                                calcYValues.Add(currtY + y);
                            }
                        }
                        if (series == ser)
                            break;
                    }
                }
                return calcYValues;
            }
            return null;
        }

        void OnRowColChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ScheduleUpdate();
        }

#if WPF
        public void Save(string fileName)
        {
            if (this is SfChart)
            {
                if ((this as SfChart).isRenderSeriesDispatched)
                {
                    (this as SfChart).RenderSeries();
                    foreach (var legend in LegendCollection)
                    {
                        legend.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    }
                    UpdateLayout();
                }
            }
            else
            {
                if ((this as SfChart3D).isRenderSeriesDispatched)
                {
                    (this as SfChart3D).RenderSeries();
                    foreach (var legend in LegendCollection)
                    {
                        legend.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    }
                    UpdateLayout();
                }
            }
            ChartBase area = this as SfChart;
            area = area ?? this as SfChart3D;
           
            FrameworkElement element = this;
            string imageExtension = null;
            imageExtension = new FileInfo(fileName).Extension.ToLower(System.Globalization.CultureInfo.InvariantCulture);
            BitmapEncoder imgEncoder = null;
            switch (imageExtension)
            {
                case ".bmp":
                    imgEncoder = new BmpBitmapEncoder();
                    break;
                case ".jpg":
                case ".jpeg":
                    imgEncoder = new JpegBitmapEncoder();
                    break;
                case ".png":
                    imgEncoder = new PngBitmapEncoder();
                    break;
                case ".gif":
                    imgEncoder = new GifBitmapEncoder();
                    break;
                case ".tif":
                case ".tiff":
                    imgEncoder = new TiffBitmapEncoder();
                    break;
                case ".wdp":
                    imgEncoder = new WmpBitmapEncoder();
                    break;
                default:
                    imgEncoder = new BmpBitmapEncoder();
                    break;
            }
            if (element != null)
            {
                RenderTargetBitmap bmpSource = new RenderTargetBitmap((int)element.ActualWidth, (int)element.ActualHeight, 96, 96, PixelFormats.Pbgra32);
                bmpSource.Render(element);
                imgEncoder.Frames.Add(BitmapFrame.Create(bmpSource));
                using (Stream stream = File.Create(fileName))
                {
                    imgEncoder.Save(stream);
                    stream.Close();
                }
            }
        }
#endif
#if SILVERLIGHT_UNCOMMON
        public void Save()
        {
            if (this is SfChart)
            {
                if ((this as SfChart).isRenderSeriesDispatched)
                {
                    (this as SfChart).RenderSeries();
                    foreach (var legend in LegendCollection)
                    {
                        legend.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    }
                    UpdateLayout();
                }
            }
            else
            {
                if ((this as SfChart3D).isRenderSeriesDispatched)
                {
                    (this as SfChart3D).RenderSeries();
                    foreach (var legend in LegendCollection)
                    {
                        legend.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    }
                    UpdateLayout();
                }
            }
            WriteableBitmap _bitmap = new WriteableBitmap(this, null);
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Bitmap(*.bmp)|*.bmp|JPEG(*.jpg,*.jpeg)|*.jpg;*.jpeg|Gif (*.gif)|*.gif|PNG(*.png)|*.png|All files (*.*)|*.*";
            if (sfd.ShowDialog() == true)
            {
                using (Stream fs = sfd.OpenFile())
                {
                    int width = _bitmap.PixelWidth;
                    int height = _bitmap.PixelHeight;

                    ChartImage ei = new ChartImage(width, height);

                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            int pixel = _bitmap.Pixels[(i * width) + j];
                            ei.SetPixel(j, i,
                                        (byte)((pixel >> 16) & 0xFF),
                                        (byte)((pixel >> 8) & 0xFF),
                                        (byte)(pixel & 0xFF),
                                        (byte)((pixel >> 24) & 0xFF)
                            );
                        }
                    }
                    Stream png = ei.GetStream();
                    int len = (int)png.Length;
                    byte[] bytes = new byte[len];
                    png.Read(bytes, 0, len);
                    fs.Write(bytes, 0, len);
                }
            }
        }
#endif

#if WPF
        public void Save(Stream stream,BitmapEncoder imgEncoder)
        {
            if (this is SfChart)
            {
                if ((this as SfChart).isRenderSeriesDispatched)
                {
                    (this as SfChart).RenderSeries();
                    foreach (var legend in LegendCollection)
                    {
                        legend.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    }
                    UpdateLayout();
                }
            }
            else
            {
                if ((this as SfChart3D).isRenderSeriesDispatched)
                {
                    (this as SfChart3D).RenderSeries();
                    foreach (var legend in LegendCollection)
                    {
                        legend.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    }
                    UpdateLayout();
                }
            }
            FrameworkElement element = this;            
            if (element != null && stream !=null && imgEncoder!=null)
            {
                RenderTargetBitmap bmpSource = new RenderTargetBitmap((int)element.ActualWidth, (int)element.ActualHeight, 96, 96, PixelFormats.Pbgra32);
                bmpSource.Render(element);
                imgEncoder.Frames.Add(BitmapFrame.Create(bmpSource));
                imgEncoder.Save(stream);
            }
    }
#endif
#if SILVERLIGHT_UNCOMMON
        public void Save(Stream stream)
        {
            if (this is SfChart)
            {
                if ((this as SfChart).isRenderSeriesDispatched)
                {
                    (this as SfChart).RenderSeries();
                    foreach (var legend in LegendCollection)
                    {
                        legend.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    }
                    UpdateLayout();
                }
            }
            else
            {
                if ((this as SfChart3D).isRenderSeriesDispatched)
                {
                    (this as SfChart3D).RenderSeries();
                    foreach (var legend in LegendCollection)
                    {
                        legend.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    }
                    UpdateLayout();
                }
            }
            WriteableBitmap _bitmap = new WriteableBitmap(this, null);
            using (Stream fs = stream)
            {
                int width = _bitmap.PixelWidth;
                int height = _bitmap.PixelHeight;

                ChartImage ei = new ChartImage(width, height);

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        int pixel = _bitmap.Pixels[(i * width) + j];
                        ei.SetPixel(j, i,
                                    (byte)((pixel >> 16) & 0xFF),
                                    (byte)((pixel >> 8) & 0xFF),
                                    (byte)(pixel & 0xFF),
                                    (byte)((pixel >> 24) & 0xFF)
                        );
                    }
                }
                Stream png = ei.GetStream();
                int len = (int)png.Length;
                byte[] bytes = new byte[len];
                png.Read(bytes, 0, len);
                fs.Write(bytes, 0, len);
            }
        }
#endif

        //Printing in WPF and Silverlight
#if WPF || SILVERLIGHT_UNCOMMON
        public void Print()
        {
            if (this is SfChart)
            {
                if ((this as SfChart).isRenderSeriesDispatched)
                {
                    (this as SfChart).RenderSeries();
                    foreach (var legend in LegendCollection)
                    {
                        legend.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    }
                    UpdateLayout();
                }
            }
            else
            {
                if ((this as SfChart3D).isRenderSeriesDispatched)
                {
                    (this as SfChart3D).RenderSeries();
                    foreach (var legend in LegendCollection)
                    {
                        legend.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    }
                    UpdateLayout();
                }
            }
#if SILVERLIGHT_UNCOMMON
            PrintDocument document = new PrintDocument();
            document.PrintPage += (s, args) =>
            {
                args.PageVisual = this;
            };
            document.Print("Syncfusion SfChart Silverlight is Printing");                        

#endif
#if WPF
            ChartPrintDialog printDialog = new ChartPrintDialog();
            PrintingEventArgs args = new PrintingEventArgs() { PrintDialog = printDialog, PrintVisual = printDialog.GetPrintVisual(this) };
            RaiseOnPrinting(args);
            if (args.ShowPrintDialog)
            {
                bool retValue = (bool)printDialog.ShowPrintDialog(this, Rect.Empty, this.ActualHeight, this.ActualWidth);
                Keyboard.Focus(this);
            }
            else if (!args.CancelPrinting)
            {
                PrintDialog pd = new PrintDialog();
                pd.PrintVisual(this, "Syncfusion Sfchart WPF is Printing");
            }
#endif
        }
#endif
#if WPF || SILVERLIGHT_UNCOMMON
        public void Print(HorizontalAlignment HorizontalAlignment, VerticalAlignment VerticalAlignment, Thickness PageMargin, bool PrintLandscape, bool ShrinkToFit)
        {
            if (this is SfChart)
            {
                if ((this as SfChart).isRenderSeriesDispatched)
                {
                    (this as SfChart).RenderSeries();
                    foreach (var legend in LegendCollection)
                    {
                        legend.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    }
                    UpdateLayout();
                }
            }
            else
            {
                if ((this as SfChart3D).isRenderSeriesDispatched)
                {
                    (this as SfChart3D).RenderSeries();
                    foreach (var legend in LegendCollection)
                    {
                        legend.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    }
                    UpdateLayout();
                }
            }
#if SILVERLIGHT_UNCOMMON
            PrintDocument document = new PrintDocument();
            document.PrintPage += (s, args) =>
            {
                args.PageVisual = Printing.Layout(this,args.PrintableArea, "Syncfusion Sfchart Silverlight is Printing", HorizontalAlignment, VerticalAlignment, PageMargin, PrintLandscape, ShrinkToFit);
            };
            document.Print("Syncfusion SfChart Silverlight is Printing");   
            
#endif
#if WPF

            PrintDialog pd = new PrintDialog();
            if (pd.ShowDialog() == true)
            {
                pd.PrintVisual(Printing.Layout(this, new Size(pd.PrintableAreaWidth, pd.PrintableAreaHeight), "Syncfusion Sfchart WPF is Printing", HorizontalAlignment, VerticalAlignment, PageMargin, PrintLandscape, ShrinkToFit), "Syncfusion Sfchart WPF is Printing");
            }
#endif
        }
#endif
#if WPF

        private void RaiseOnPrinting(PrintingEventArgs args)
        {
            if (OnPrinting != null)
            {
                OnPrinting(this, args);
            }
        }

        public event OnPrintingEventHandler OnPrinting;
#endif
#if NETFX_CORE

        public void Print()
        {
            Printing.Print();
        }
#endif

#if NETFX_CORE8_1
        /// <summary>
        /// Exports the SfChart image in the selected location by using FileSavePicker
        /// </summary>
        public async void Save()
        {
            var renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(this);
            var pixels = await renderTargetBitmap.GetPixelsAsync();

            //Initialize fileSavePicker with the image formates and its default file name
            var fileSavePicker = new FileSavePicker();
            fileSavePicker.FileTypeChoices.Add("BMP", new List<string>() { ".bmp" });
            fileSavePicker.FileTypeChoices.Add("GIF", new List<string>() { ".gif" });
            fileSavePicker.FileTypeChoices.Add("PNG", new List<string>() { ".png" });
            fileSavePicker.FileTypeChoices.Add("JPG", new List<string>() { ".jpg" });
            fileSavePicker.FileTypeChoices.Add("JPG-XR", new List<string>() { ".jxr" });
            fileSavePicker.FileTypeChoices.Add("TIFF", new List<string>() { ".tiff" });
            fileSavePicker.SuggestedFileName = "untitled";

            var file = await fileSavePicker.PickSaveFileAsync();
            if (file != null)
            {
                Guid encoderId = GetBitmapEncoderId(file.FileType);
                using (var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
                {
                    var encoder = await BitmapEncoder.CreateAsync(encoderId, stream);
                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)renderTargetBitmap.PixelWidth,
                        (uint)renderTargetBitmap.PixelHeight, imageResolution, imageResolution, pixels.ToArray());
                    await encoder.FlushAsync();
                }
            }
        }

        private Guid GetBitmapEncoderId(string type)
        {
            switch (type)
            {
                case ".bmp":
                    return BitmapEncoder.BmpEncoderId;
                case ".jpg":
                case ".jpeg":
                    return BitmapEncoder.JpegEncoderId;
                case ".jxr":
                    return BitmapEncoder.JpegXREncoderId;
                case ".png":
                    return BitmapEncoder.PngEncoderId;
                case ".tif":
                case ".tiff":
                    return BitmapEncoder.TiffEncoderId;
            }
            return BitmapEncoder.BmpEncoderId;
        }

        /// <summary>
        /// Export the SfChart image with the given name and the specified location.
        /// </summary>
        /// <param name="fileName">Name of the image file.</param>
        /// <param name="folderLocation">Specifies the location to save. Default location:Installed Location.</param>
        public async void Save(string fileName, StorageFolder folderLocation)
        {
            var renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(this);
            var pixels = await renderTargetBitmap.GetPixelsAsync();
            StorageFolder storageFolder = folderLocation != null ? folderLocation : Windows.ApplicationModel.Package.Current.InstalledLocation;
            var file = await storageFolder.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);
            Guid encoderId = GetBitmapEncoderId(file.FileType.ToLower());
            using (var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
            {
                var encoder = await BitmapEncoder.CreateAsync(encoderId, stream);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)renderTargetBitmap.PixelWidth,
                    (uint)renderTargetBitmap.PixelHeight, imageResolution, imageResolution, pixels.ToArray());
                await encoder.FlushAsync();
            }
        }

        /// <summary>
        /// Export the SfChart image using the stream and encoder.
        /// </summary>
        /// <param name="stream">Image Stream</param>
        /// <param name="bitmapEncoderID">BitmapEncoder ID</param>
        public async void Save(IRandomAccessStream stream, Guid bitmapEncoderID)
        {
            var renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(this);
            var pixelBuffer = await renderTargetBitmap.GetPixelsAsync();
            var dataWriter = new Windows.Storage.Streams.DataWriter(stream);
            var encoder = await BitmapEncoder.CreateAsync(bitmapEncoderID, stream);
            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)renderTargetBitmap.PixelWidth,
                                (uint)renderTargetBitmap.PixelHeight, imageResolution, imageResolution, pixelBuffer.ToArray());
            await encoder.FlushAsync();
        }

#endif
        #endregion
    }
}
