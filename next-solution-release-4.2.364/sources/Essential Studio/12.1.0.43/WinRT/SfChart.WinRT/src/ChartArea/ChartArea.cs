#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Diagnostics;
using System.Collections;
#if SILVERLIGHT_UNCOMMON
using System.Windows.Printing;
#endif
#else
using System.Threading.Tasks;
using Windows.UI;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Collections;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents the Chart control which is used to visualize the data graphically.
    /// </summary>
    /// <remarks>
    /// The Chart is often used to make it easier to
    /// understand large amount of data and the relationship between different parts
    /// of the data. Chart can usually be read more quickly than the raw data that they
    /// come from. <para> Certain <see cref="ChartSeriesBase" /> are more useful for
    /// presenting a given data set than others. For example, data that presents
    /// percentages in different groups (such as "satisfied, not satisfied, unsure") are
    /// often displayed in a <see cref="PieSeries" /> chart, but are more easily
    /// understood when presented in a horizontal <see cref="BarSeries" /> chart.
    /// On the other hand, data that represents numbers that change over a period of
    /// time (such as "annual revenue from 20011 to 2012") might be best shown as a <see
    /// cref="LineSeries" /> chart. </para>
    /// </remarks>
    /// <seealso cref="ChartSeriesBase"/>
    /// <seealso cref="ChartLegend"/>
    /// <seealso cref="ChartAxis"/>
#if WINDOWS_PHONE
    [ContentProperty("Series")]
#else
    [ContentProperty(Name = "Series")]
#endif
    [ClassReference(IsReviewed = false)]
    public class SfChart : ChartBase
    {
        #region fields

#if WINDOWS_PHONE
       internal bool isRenderSeriesDispatched = false;
#else
        internal IAsyncAction renderSeriesAction;
#endif

        private Panel gridLinesPanel;

        internal Panel chartAxisPanel;

        private bool clearPixels = false;       

        private Panel seriesPresenter;

        private ChartRootPanel rootPanel;

        List<double> sumItems = new List<double>();

        internal bool CanRenderToBuffer
        {
            get;
            set;
        }

        bool HasBitmapSeries
        {
            get
            {
                return VisibleSeries.Any(ser => ser.GetType().ToString().Contains("Bitmap") || ser.GetType().ToString().Contains("MACD"));
            }
        }

        private ChartBehaviorsCollection behaviors;

        private Panel internalCanvas = null;

        internal Panel InternalCanvas
        {
            get
            {
                return internalCanvas;
            }
            set
            {
                internalCanvas = value;
            }
        }

        internal Panel GridLinesPanel
        {
            get
            {
                return gridLinesPanel;
            }
        }

        internal bool HoldUpdate = false;

        internal Canvas ChartAnnotationCanvas
        {
            get;
            set;
        }

        internal Canvas SeriesAnnotationCanvas
        {
            get;
            set;
        }

        internal AnnotationManager AnnotationManager
        {
            get;
            set;
        }
       
        private Image fastRenderDevice = new Image();

        private WriteableBitmap fastRenderSurface;

        private byte[] fastBuffer;

        public EventHandler AreaUpdated;
#if !WINDOWS_PHONE
        private Stream fastRenderSurfaceStream;
#endif
        #endregion

        #region ctor
        /// <summary>
        /// Constructor
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public SfChart()
        {
#if WPF
            EnvironmentTest.ValidateLicense(typeof(SfChart));
#endif
            DefaultStyleKey = typeof(SfChart);
            UpdateAction = UpdateAction.Invalidate;
#if !WPF
            Series = new ChartSeriesCollection();
            TechnicalIndicators = new ObservableCollection<ChartSeries>();
#endif
            VisibleSeries = new ChartVisibleSeriesCollection();
            Axes = new ChartAxisCollection();
            Annotations = new AnnotationCollection();
#if NETFX_CORE || SILVERLIGHT_UNCOMMON || WPF
            Printing = new Printing(this);
#endif
#if !WINDOWS_PHONE
            ManipulationMode = ManipulationModes.Scale
                | ManipulationModes.TranslateRailsX
                | ManipulationModes.TranslateRailsY
                | ManipulationModes.TranslateX
                | ManipulationModes.TranslateY
                | ManipulationModes.TranslateInertia
                | ManipulationModes.Rotate;
#endif
            Behaviors = new ChartBehaviorsCollection(this);
            ColorModel = new ChartColorModel(this.Palette);
        }

        #endregion

        #region dependency property

        /// <summary>
        /// Gets or Sets primary axis.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public ChartAxisBase2D PrimaryAxis
        {
            get { return (ChartAxisBase2D)GetValue(PrimaryAxisProperty); }
            set { SetValue(PrimaryAxisProperty, value); }
        }


        /// <summary>
        ///  Using a DependencyProperty as the backing store for PrimaryAxis.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PrimaryAxisProperty =
            DependencyProperty.Register("PrimaryAxis", typeof(ChartAxisBase2D), typeof(SfChart), new PropertyMetadata(null, OnPrimaryAxisChanged));

        private static void OnPrimaryAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                ((SfChart)d).InternalPrimaryAxis = (ChartAxis)e.NewValue;
                ((ChartAxis)e.NewValue).Orientation = Orientation.Horizontal;
            }

            ((SfChart)d).OnAxisChanged(e);

        }

        /// <summary>
        /// Gets or Sets secondary axis.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public RangeAxisBase SecondaryAxis
        {
            get { return (RangeAxisBase)GetValue(SecondaryAxisProperty); }
            set { SetValue(SecondaryAxisProperty, value); }
        }


        /// <summary>
        ///  Using a DependencyProperty as the backing store for SecondaryAxis.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SecondaryAxisProperty =
            DependencyProperty.Register("SecondaryAxis", typeof(RangeAxisBase), typeof(SfChart), new PropertyMetadata(null, OnSecondaryAxisChanged));

        private static void OnSecondaryAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                ((SfChart)d).InternalSecondaryAxis = (ChartAxis)e.NewValue;
                ((ChartAxis)e.NewValue).Orientation = Orientation.Vertical;
            }

            ((SfChart)d).OnAxisChanged(e);

        }

        internal Canvas BottomAdorningCanvas
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the chart watermark.
        /// </summary>
        public Watermark Watermark
        {
            get { return (Watermark)GetValue(ContentControlProperty); }
            set { SetValue(ContentControlProperty, value); }
        }


        /// <summary>
        ///  Using a DependencyProperty as the backing store for WatermarkContent.
        /// </summary>
        public static readonly DependencyProperty ContentControlProperty =
            DependencyProperty.Register("ContentControl", typeof(Watermark), typeof(SfChart), new PropertyMetadata(null, OnWatermarkChanged));

        private static void OnWatermarkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SfChart).OnWaterMarkChanged(e);
        }

        private void OnWaterMarkChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.ChartDockPanel != null)
            {
                AddOrRemoveWatermark(e.NewValue as Watermark, e.OldValue as Watermark);
            }
        }

        /// <summary>
        /// Gets or Sets the color to paint the outline of chart area
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Brush AreaBorderBrush
        {
            get { return (Brush)GetValue(AreaBorderBrushProperty); }
            set { SetValue(AreaBorderBrushProperty, value); }
        }


        /// <summary>
        ///  Using a DependencyProperty as the backing store for AreaBorderBrush.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AreaBorderBrushProperty =
            DependencyProperty.Register("AreaBorderBrush", typeof(Brush), typeof(SfChart), new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets the outline thickness of chart area.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Thickness AreaBorderThickness
        {
            get { return (Thickness)GetValue(AreaBorderThicknessProperty); }
            set { SetValue(AreaBorderThicknessProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AreaBorderThickness.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AreaBorderThicknessProperty =
            DependencyProperty.Register("AreaBorderThickness", typeof(Thickness), typeof(SfChart), new PropertyMetadata(new Thickness(1)));


        /// <summary>
        /// Gets or Sets the color to paint the Background of chart area
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Brush AreaBackground
        {
            get { return (Brush)GetValue(AreaBackgroundProperty); }
            set { SetValue(AreaBackgroundProperty, value); }
        }


        /// <summary>
        ///  Using a DependencyProperty as the backing store for AreaBackground.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AreaBackgroundProperty =
            DependencyProperty.Register("AreaBackground", typeof(Brush), typeof(SfChart), new PropertyMetadata(null));

    

        /// <summary>
        /// Gets the collection of ChartBehaviors
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public ChartBehaviorsCollection Behaviors
        {
            get { return behaviors; }
            set { behaviors = value; }
        }
      
        /// <summary>
        /// Get or Set series property
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public ChartSeriesCollection Series
        {
            get { return (ChartSeriesCollection)GetValue(SeriesProperty); }
            set { SetValue(SeriesProperty, value); }
        }

        
       /// <summary>
        /// Using a DependencyProperty as the backing store for Series.  This enables animation, styling, binding, etc...
       /// </summary>
        public static readonly DependencyProperty SeriesProperty =
            DependencyProperty.Register("Series", typeof(ChartSeriesCollection), typeof(SfChart), new PropertyMetadata(null, OnSeriesPropertyCollectionChanged));

        private static void OnSeriesPropertyCollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SfChart).OnSeriesPropertyCollectionChanged(e);
        }

        private void OnSeriesPropertyCollectionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                RemoveVisualChild();
                AddOrRemoveBitmap();
                (e.OldValue as ChartSeriesCollection).CollectionChanged -= OnSeriesCollectionChanged;
                
            }
            if (Series == null) return;
            Series.CollectionChanged += OnSeriesCollectionChanged;
            if (Series.Count > 0)
            {
                if (Series[0] is PolarSeries || Series[0] is RadarSeries)
                    AreaType = ChartAreaType.PolarAxes;
                else if (Series[0] is AccumulationSeriesBase)
                    AreaType = ChartAreaType.None;
                else
                    AreaType = ChartAreaType.CartesianAxes;

                foreach (ChartSeries series in Series)
                {
                    series.UpdateLegendIconTemplate(false);
                    if (Axes.Count == 0)
                    SetAxisForChartSeries(series as ISupportAxes2D);
                    series.Area = this;
                    if (series.ActualXAxis != null && !this.Axes.Contains(series.ActualXAxis))
                    {
                        series.ActualXAxis.Area = this;
                        this.Axes.Add(series.ActualXAxis);
                    }
                    if (series.ActualYAxis != null && !this.Axes.Contains(series.ActualYAxis))
                    {
                        series.ActualYAxis.Area = this;
                        this.Axes.Add(series.ActualYAxis);
                    }
                    if (this.seriesPresenter != null && !this.seriesPresenter.Children.Contains(series))
                    {
                        this.seriesPresenter.Children.Add(series);
                    }
                    if (series.IsSeriesVisible)
                    {
                        if (AreaType == ChartAreaType.PolarAxes && (series is PolarSeries || series is RadarSeries))
                        {
                            VisibleSeries.Add(series);
                        }
                        else if (AreaType == ChartAreaType.None && (series is AccumulationSeriesBase))
                        {
                            VisibleSeries.Add(series);
                        }
                        else if(AreaType == ChartAreaType.CartesianAxes && (series is CartesianSeries || series is HistogramSeries))
                        {
                            VisibleSeries.Add(series);
                        }
                    }
                    base.ActualSeries.Add(series);
                }
                UpdateLegend(Legend, false);
                AddOrRemoveBitmap();
                ScheduleUpdate();
            }
        }

        private void RemoveVisualChild()
        {
            if (seriesPresenter != null)
            {
                for (int i = seriesPresenter.Children.Count - 1; i >= 0; i--)
                {
                    if (seriesPresenter.Children[i] is AdornmentSeries ||
                        seriesPresenter.Children[i] is HistogramSeries)
                    {
                        var series = seriesPresenter.Children[i] as ISupportAxes;
                        if (series != null)
                        {
                            if (series is CartesianSeries)
                            {
                                if (((CartesianSeries)series).YAxis== SecondaryAxis)
                                ((CartesianSeries)series).YAxis = null;
                                if (((CartesianSeries)series).XAxis == PrimaryAxis)
                                ((CartesianSeries)series).XAxis = null;
                            }
                            else if (series is PolarRadarSeriesBase)
                            {
                                if (((PolarRadarSeriesBase)series).YAxis == SecondaryAxis)
                                    ((PolarRadarSeriesBase)series).YAxis = null;
                                if (((PolarRadarSeriesBase)series).XAxis == PrimaryAxis)
                                    ((PolarRadarSeriesBase)series).XAxis = null;
                            }
             
                        }
                        seriesPresenter.Children.RemoveAt(i);
                    }
                }
            }
            Axes.Clear();
            VisibleSeries.Clear();
            ActualSeries.Clear();
        }

        /// <summary>
        /// Get or Set TechnicalIndicatorsProperty
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public ObservableCollection<ChartSeries> TechnicalIndicators
        {
            get { return (ObservableCollection<ChartSeries>)GetValue(TechnicalIndicatorsProperty); }
            set { SetValue(TechnicalIndicatorsProperty, value); }
        }

       
        /// <summary>
        ///  Using a DependencyProperty as the backing store for Series.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TechnicalIndicatorsProperty =
            DependencyProperty.Register("TechnicalIndicators", typeof(ObservableCollection<ChartSeries>), typeof(SfChart), new PropertyMetadata(null, OnTechnicalIndicatorsPropertyChanged));

        private static void OnTechnicalIndicatorsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SfChart).OnTechnicalIndicatorsPropertyChanged(e);
        }

        private void OnTechnicalIndicatorsPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                if (seriesPresenter != null)
                {
                    for (int i = seriesPresenter.Children.Count - 1; i >= 0; i--)
                    {
                        if (seriesPresenter.Children[i] is FinancialTechnicalIndicator)
                        {
                            var series = seriesPresenter.Children[i] as ISupportAxes;
                            seriesPresenter.Children.RemoveAt(i);
                        }
                    }
                }
                Axes.Clear();
                (e.OldValue as ObservableCollection<ChartSeries>).CollectionChanged -= OnTechnicalIndicatorsCollectionChanged;
            }

            if (TechnicalIndicators != null)
            {
                TechnicalIndicators.CollectionChanged += OnTechnicalIndicatorsCollectionChanged;
                if (TechnicalIndicators.Count > 0)
                {
                    foreach (var indicator in TechnicalIndicators)
                    {
                        SetAxisForChartSeries(indicator as ISupportAxes2D);
                        indicator.Area = this;
                        if (indicator.ActualXAxis != null && !Axes.Contains(indicator.ActualXAxis))
                        {
                            indicator.ActualXAxis.Area = this;
                            Axes.Add(indicator.ActualXAxis);
                        }
                        if (indicator.ActualYAxis != null && !Axes.Contains(indicator.ActualYAxis))
                        {
                            indicator.ActualYAxis.Area = this;
                            Axes.Add(indicator.ActualYAxis);
                        }
                        if (seriesPresenter != null && !seriesPresenter.Children.Contains(indicator))
                        {
                            seriesPresenter.Children.Add(indicator);
                        }
                    }
                }
            }
            ScheduleUpdate();
        }

        /// <summary>
        /// Gets or sets the annotations.
        /// </summary>
        /// <value>
        /// The annotations.
        /// </value>
        public AnnotationCollection Annotations
        {
            get { return (AnnotationCollection)GetValue(AnnotationsProperty); }
            set { SetValue(AnnotationsProperty, value); }
        }

        /// <summary>
        /// The annotations property
        /// </summary>
        public static readonly DependencyProperty AnnotationsProperty =
            DependencyProperty.Register("Annotations", typeof(AnnotationCollection), typeof(SfChart), new PropertyMetadata(null, OnAnnotationsChanged));

        private static void OnAnnotationsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as SfChart).AnnotationsChanged();
        }
        internal void AnnotationsChanged()
        {
            if (this.AnnotationManager != null)
                AnnotationManager.Annotations = this.Annotations;
        }
        #endregion

        #region methods

        private void OnAxisChanged(DependencyPropertyChangedEventArgs e)
        {
            var chartAxis = e.NewValue as ChartAxis;

            var oldAxis = e.OldValue as ChartAxis;

            if (Series != null)
                foreach (var series in Series)
                {
                    SetAxisForChartSeries(series as ISupportAxes2D); // Set XAxis and YAxis for each series
                }

            if (oldAxis != null && Axes.Contains(oldAxis))
            {
                Axes.Remove(oldAxis);

                if (oldAxis.RegisteredSeries.Count > 0)
                {
                    var registeredSeriesCol = oldAxis.RegisteredSeries.Cast<ChartSeriesBase>().ToList();
                    foreach (var series in registeredSeriesCol.OfType<ISupportAxes>())
                    {
                        if (((ISupportAxes2D)series).XAxis == oldAxis)
                        {
                            ((ISupportAxes2D)series).XAxis = null;
                        }
                        else if ((series as ISupportAxes2D).YAxis == oldAxis)
                        {
                            (series as ISupportAxes2D).YAxis = null;
                        }
                    }
                }
            }
            if (Axes!=null && chartAxis != null && !Axes.Contains(chartAxis))
            {
                chartAxis.Area = this;
                Axes.Insert(0,chartAxis);
            }
            if (AnnotationManager != null)
                AnnotationManager.Annotations = Annotations;
            ScheduleUpdate();
        }

        /// <summary>
        /// Raises the <see cref="E:SeriesBoundsChanged" /> event.
        /// </summary>
        /// <param name="args">The <see cref="ChartSeriesBoundsEventArgs"/> instance containing the event data.</param>
        protected internal override void OnSeriesBoundsChanged(ChartSeriesBoundsEventArgs args)
        {
            CreateFastRenderSurface();

            if (InternalCanvas != null)
            {
                InternalCanvas.Clip = new RectangleGeometry()
                    {
                        Rect =
                            new Rect(0, 0, SeriesClipRect.Width + 0.5, SeriesClipRect.Height + 0.5)
                    };
            }
            base.OnSeriesBoundsChanged(args);
        }

        internal void CreateFastRenderSurface()
        {
            try
            {
                if (this.seriesPresenter != null && this.seriesPresenter.Children.Contains(fastRenderDevice) && !SeriesClipRect.IsEmpty && SeriesClipRect.Width > 0 && SeriesClipRect.Height > 0)
                {
#if WPF
                    this.fastRenderSurface = new WriteableBitmap((int)SeriesClipRect.Width, (int)SeriesClipRect.Height, 96.0, 96.0, PixelFormats.Pbgra32, null);
#else
                this.fastRenderSurface = new WriteableBitmap((int)SeriesClipRect.Width, (int)SeriesClipRect.Height);
#endif
                    this.fastRenderDevice.Source = this.fastRenderSurface;
#if NETFX_CORE
                this.fastRenderSurfaceStream = this.fastRenderSurface.PixelBuffer.AsStream();
                CreateBuffer(new Size(SeriesClipRect.Width, SeriesClipRect.Height));
#endif
                }
            }
            catch
            {

            }
        }

        private void OnTechnicalIndicatorsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                if (seriesPresenter != null)
                {
                    for (int i = seriesPresenter.Children.Count - 1; i >= 0; i--)
                    {
                        if (seriesPresenter.Children[i] is FinancialTechnicalIndicator)
                        {
                            var series = seriesPresenter.Children[i] as ISupportAxes;
                            (series as ISupportAxes2D).YAxis = null;
                            (series as ISupportAxes2D).XAxis = null;
                            seriesPresenter.Children.RemoveAt(i);
                        }
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                FinancialTechnicalIndicator indicator = e.OldItems[0] as FinancialTechnicalIndicator;
                if (indicator == null) return;
                if (indicator.ActualYAxis.RegisteredSeries != null &&
                    indicator.ActualYAxis.RegisteredSeries.Contains(indicator))
                {
                    indicator.YAxis = null;
                    indicator.XAxis = null;
                }
                if (seriesPresenter.Children.Contains(indicator))
                    seriesPresenter.Children.Remove(indicator);
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (FinancialTechnicalIndicator indicator in e.NewItems)
                {
                    SetAxisForChartSeries(indicator as ISupportAxes2D);
                    indicator.UpdateLegendIconTemplate(false);
                    indicator.Area = this;
                    if (indicator.XAxis != null && !this.Axes.Contains(indicator.XAxis))
                    {
                        indicator.XAxis.Area = this;
                        this.Axes.Add(indicator.XAxis);
                    }
                    if (indicator.YAxis != null && !this.Axes.Contains(indicator.YAxis))
                    {
                        indicator.YAxis.Area = this;
                        this.Axes.Add(indicator.YAxis);
                    }
                    if (this.seriesPresenter != null && !this.seriesPresenter.Children.Contains(indicator))
                    {
                        this.seriesPresenter.Children.Add(indicator);
                    }
                }
            }
            ScheduleUpdate();
        }

        internal void AddOrRemoveBitmap()
        {
            if (this.seriesPresenter!=null && this.seriesPresenter.Children.Contains(fastRenderDevice) && !HasBitmapSeries)
            {
                this.seriesPresenter.Children.Remove(fastRenderDevice);
                this.fastRenderSurface = null;
#if NETFX_CORE
                this.fastRenderSurfaceStream = null;
#endif
                this.fastBuffer = null;
            }
            else if (this.seriesPresenter != null && !this.seriesPresenter.Children.Contains(fastRenderDevice) && HasBitmapSeries)
            {
                this.seriesPresenter.Children.Insert(0, fastRenderDevice);
                if (fastRenderSurface==null)
                    this.CreateFastRenderSurface();
            }
        }

        void OnSeriesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                if (seriesPresenter != null)
                {
                    for (int i = seriesPresenter.Children.Count - 1; i >= 0; i--)
                    {
                        if (seriesPresenter.Children[i] is AdornmentSeries ||
                            seriesPresenter.Children[i] is HistogramSeries)
                        {
                            var series = seriesPresenter.Children[i] as ISupportAxes;
                            if (series != null)
                            {
                                if (series is CartesianSeries)
                                {
                                    ((CartesianSeries)series).YAxis = null;
                                    ((CartesianSeries)series).XAxis = null;
                                }
                                else if (series is PolarRadarSeriesBase)
                                {
                                    ((PolarRadarSeriesBase)series).YAxis = null;
                                    ((PolarRadarSeriesBase)series).XAxis = null;
                                }
                                else if (series is FinancialTechnicalIndicator)
                                {
                                    ((FinancialTechnicalIndicator)series).YAxis = null;
                                    ((FinancialTechnicalIndicator)series).XAxis = null;
                                }
                            }
                            seriesPresenter.Children.RemoveAt(i);
                        }
                    }
                }
                ActualSeries.Clear();
                VisibleSeries.Clear();
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                ChartSeriesBase series = e.OldItems[0] as ChartSeriesBase;
                if (series is ISupportAxes && series.ActualYAxis.RegisteredSeries != null &&
                    series.ActualYAxis.RegisteredSeries.Contains(series as ISupportAxes))
                {
                    if (series is CartesianSeries)
                    {
                        ((CartesianSeries)series).YAxis = null;
                        ((CartesianSeries)series).XAxis = null;
                    }
                    else if (series is PolarRadarSeriesBase)
                    {
                        ((PolarRadarSeriesBase)series).YAxis = null;
                        ((PolarRadarSeriesBase)series).XAxis = null;
                    }
                    else if (series is HistogramSeries)
                    {
                        ((HistogramSeries)series).YAxis = null;
                        ((HistogramSeries)series).XAxis = null;
                    }
                    else if (series is FinancialTechnicalIndicator)
                    {
                        ((FinancialTechnicalIndicator)series).YAxis = null;
                        ((FinancialTechnicalIndicator)series).XAxis = null;
                    }
                }

                if (VisibleSeries.Contains(series))
                    VisibleSeries.Remove(series);
                if (ActualSeries.Contains(series))
                    ActualSeries.Remove(series);
                this.seriesPresenter.Children.Remove(series);
                series.RemoveTooltip();
                if (VisibleSeries.Count == 0 && Series.Count > 0)
                {
                    if (Series[0] is PolarRadarSeriesBase)
                        AreaType = ChartAreaType.PolarAxes;
                    else if (Series[0] is AccumulationSeriesBase)
                        AreaType = ChartAreaType.None;
                    else
                        AreaType = ChartAreaType.CartesianAxes;
                    UpdateVisibleSeries(Series);
                }
                else if (VisibleSeries.Count == 0 && Series.Count == 0)
                {
                    AreaType = ChartAreaType.CartesianAxes;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewStartingIndex == 0)
                {
                    if (Series[0] is PolarRadarSeriesBase)
                        AreaType = ChartAreaType.PolarAxes;
                    else if (Series[0] is AccumulationSeriesBase)
                        AreaType = ChartAreaType.None;
                    else
                        AreaType = ChartAreaType.CartesianAxes;
                }
                UpdateVisibleSeries(e.NewItems);
            }
            var canvas = this.GetAdorningCanvas();
            if (canvas != null)
            {
                foreach (var item in canvas.Children)
                {
                    if (item is ChartTooltip)
                        canvas.Children.Remove(item as UIElement);
                }
            }
            IsUpdateLegend = true;
            AddOrRemoveBitmap();
            this.ScheduleUpdate();
            SBSInfoCalculated = false;
        }

        private void UpdateVisibleSeries(IList seriesColl)
        {
            foreach (ChartSeries series in seriesColl)
            {
                series.UpdateLegendIconTemplate(false);
                SetAxisForChartSeries(series as ISupportAxes2D);
                series.Area = this;
                if (series.ActualXAxis != null && !this.Axes.Contains(series.ActualXAxis))
                {
                    series.ActualXAxis.Area = this;
                    Axes.Add(series.ActualXAxis);
                }
                if (series.ActualYAxis != null && !this.Axes.Contains(series.ActualYAxis))
                {
                    series.ActualYAxis.Area = this;
                    Axes.Add(series.ActualYAxis);
                }
                if (seriesPresenter != null && !this.seriesPresenter.Children.Contains(series))
                {
                    seriesPresenter.Children.Add(series);
                }
                if (series.IsSeriesVisible)
                {
                    if (AreaType == ChartAreaType.PolarAxes && series is PolarRadarSeriesBase)
                    {
                        VisibleSeries.Add(series);
                    }
                    else if (AreaType == ChartAreaType.None && series is AccumulationSeriesBase)
                    {
                        VisibleSeries.Add(series);
                    }
                    else if (AreaType == ChartAreaType.CartesianAxes && (series is CartesianSeries || series is HistogramSeries))
                    {
                        VisibleSeries.Add(series);
                    }
                }
                base.ActualSeries.Add(series);
            }
        }

        internal void UpdateStripLines()
        {
            if (GridLinesLayout != null && (GridLinesLayout is ChartCartesianGridLinesPanel))
                (GridLinesLayout as ChartCartesianGridLinesPanel).UpdateStripLines();
        }

#if WINDOWS_PHONE
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        public override void OnApplyTemplate()
#else
        /// <summary>
        /// Invoke to render sfchart
        /// </summary>
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            
            this.SizeChanged += OnSfChartSizeChanged;
            if (seriesPresenter != null &&
                seriesPresenter.Children.Contains(fastRenderDevice))
            {
                seriesPresenter.Children.Remove(fastRenderDevice);
            }

            seriesPresenter = GetTemplateChild("seriesPresenter") as Panel;
            chartAxisPanel = GetTemplateChild("PART_chartAxisPanel") as Panel;
            gridLinesPanel = GetTemplateChild("gridLines") as Panel;
            InternalCanvas = GetTemplateChild("InternalCanvas") as Panel;
            AdorningCanvas = GetTemplateChild("adorningCanvas") as Canvas;
            ChartDockPanel = GetTemplateChild("Part_DockPanel") as ChartDockPanel;
            rootPanel = GetTemplateChild("LayoutRoot") as ChartRootPanel;
            rootPanel.Area = this;
            ChartAnnotationCanvas = this.GetTemplateChild("Part_ChartAnnotationCanvas") as Canvas;
            SeriesAnnotationCanvas = this.GetTemplateChild("Part_SeriesAnnotationCanvas") as Canvas;
            AnnotationManager = new AnnotationManager {Chart = this, Annotations = this.Annotations};
            BottomAdorningCanvas = this.GetTemplateChild("bottomAdorningCanvas") as Canvas;
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.AdorningCanvas = AdorningCanvas;
                behavior.BottomAdorningCanvas = BottomAdorningCanvas;
                behavior.InternalAttachElements();
            }

            if (Series != null)
            {
                foreach (ChartSeries series in Series)
                {
                    series.Area = this;
                    if (series.ShowTooltip)
                        ShowTooltip = true;
                }
            }
            if (ShowTooltip)
                this.Tooltip = new ChartTooltip();
            if (TechnicalIndicators != null)
            {
                foreach (FinancialTechnicalIndicator indicator in TechnicalIndicators)
                {
                    indicator.Area = this;
                }
            }
            foreach (ChartAxis axis in Axes)
            {
                axis.Area = this;
            }
            UpdateAxisLayoutPanels();

            if (seriesPresenter != null)
            {
                AddOrRemoveBitmap();
                if (Series != null)
                {
                    foreach (ChartSeriesBase series in Series)
                    {
                        this.seriesPresenter.Children.Add(series);
                    }
                }   
                if (TechnicalIndicators != null)
                {
                    foreach (FinancialTechnicalIndicator indicator in TechnicalIndicators)
                    {
                        if (!this.seriesPresenter.Children.Contains(indicator))
                            this.seriesPresenter.Children.Add(indicator);
                    }
                }
            }
            UpdateLegend(Legend, true);
            if (Watermark != null)
                AddOrRemoveWatermark(Watermark, null);
            IsTemplateApplied = true;
        }

        private void AddOrRemoveWatermark(Watermark newWatermark, Watermark oldWatermark)
        {
            if (this.ChartDockPanel.Children.Contains(oldWatermark))
                this.ChartDockPanel.Children.Remove(oldWatermark);
            if (newWatermark != null && !this.rootPanel.Children.Contains(newWatermark))
            {
                this.Watermark.SetValue(ChartDockPanel.DockProperty, ChartDock.Floating);
                this.ChartDockPanel.Children.Add(newWatermark);
            }
        }

        internal override void UpdateArea(bool forceUpdate)
        {
#if WINDOWS_PHONE
            if (isUpdateDispatched || forceUpdate)
#else
            if (updateAreaAction != null || forceUpdate)
#endif
            {
                if (AreaType == ChartAreaType.CartesianAxes)
                {
                    if (ColumnDefinitions.Count == 0)
                        ColumnDefinitions.Add(new ChartColumnDefinition());
                    if (RowDefinitions.Count == 0)
                        RowDefinitions.Add(new ChartRowDefinition());
                }
                if (VisibleSeries == null)
                    return;
                if (AreaType == ChartAreaType.None)
                {
                    if (VisibleSeries.Count > 0)
                    {
                        CircularSegmentRadius = new double[VisibleSeries.Count];
                    }
                }


                if ((UpdateAction & UpdateAction.Create) == UpdateAction.Create)
                {
                    foreach (ChartSeriesBase series in VisibleSeries)
                    {
                        if (!series.IsPointGenerated)
                            series.GeneratePoints();
                        if (series.ShowTooltip)
                            ShowTooltip = true;
                    }
                   
                   //Initialize default axes for SfChart when PrimaryAxis or SecondayAxis is not set
                    InitializeDefaultAxes();

                    foreach (ChartSeriesBase series in VisibleSeries)
                    {
                        series.Invalidate();
                    }
                    if (ShowTooltip)
                        this.Tooltip = new ChartTooltip();

                    if (TechnicalIndicators != null && AreaType==ChartAreaType.CartesianAxes)
                    {
                        foreach (FinancialTechnicalIndicator indicator in TechnicalIndicators)
                        {
                            if (!indicator.IsPointGenerated)
                            {
                                if (indicator.ItemsSource == null && VisibleSeries.Count > 0)
                                {
                                    ChartSeriesBase series = this.Series[indicator.Name] ?? this.Series[0];
                                    indicator.SetSeriesItemSource(series);
                                }
                                else
                                    indicator.GeneratePoints();
                            }
                            indicator.Invalidate();
                        }
                    }
                }

                if (IsUpdateLegend && (this.ChartDockPanel != null))
                {
                    UpdateLegend(Legend, false);
                    IsUpdateLegend = false;
                }
                if ((UpdateAction & UpdateAction.UpdateRange) == UpdateAction.UpdateRange)
                {
                    foreach (ChartSeriesBase series in VisibleSeries)
                    {
                        series.UpdateRange();
                    }

                    if (TechnicalIndicators != null)
                    {
                        foreach (FinancialTechnicalIndicator indicator in TechnicalIndicators)
                        {
                            indicator.UpdateRange();
                        }
                    }
                }

                if (RootPanelDesiredSize != null)
                {
                    if ((UpdateAction & UpdateAction.Layout) == UpdateAction.Layout)
                        LayoutAxis(RootPanelDesiredSize.Value);
                    UpdateLegendArrangeRect();
                    if ((UpdateAction & UpdateAction.Render) == UpdateAction.Render)
                    {
                        if (!isLoaded)
                        {
                            ScheduleRenderSeries();
                            isLoaded = true;
                        }
#if WINDOWS_PHONE
                        else if (!isRenderSeriesDispatched)
#else
                        else if (renderSeriesAction == null)
#endif
                        {
                            RenderSeries();
                        }
                    }
                }

                UpdateAction = UpdateAction.Invalidate;

#if WINDOWS_PHONE
                isUpdateDispatched = false;
                AreaUpdated?.Invoke(this, null);
#else
                updateAreaAction = null;
#endif
                if (Behaviors != null)
                {
                    foreach (var behavior in Behaviors)
                    {
                        behavior.OnLayoutUpdated();
                    }
                }
            }
        }

        /// <summary>
        /// Set default axes for SfChart
        /// </summary>
        internal void InitializeDefaultAxes()
        {
            if (PrimaryAxis == null)
            {
                if (Series!=null && Series.Count == 0)
                    PrimaryAxis = new NumericalAxis();
#if WPF || Silverlight
                else if (isLoaded) 
#else
                else  if (RootPanelDesiredSize!=null)
#endif
                {
                    //get the XAxisValueType from the each series in Series collection 
                    var valueTypes = (from series in Series
                                      where (series is HistogramSeries) || (series is PolarRadarSeriesBase) || (series is CartesianSeries && series.ActualXAxis == null)
                                      select series.XAxisValueType).ToList();

                    if (valueTypes.Count > 0)
                        SetPrimaryAxis(valueTypes[0]);//Set PrimaryAxis for SfChart based on XAxisValueType
                    else
                        InternalPrimaryAxis = Series[0].ActualXAxis;
                }
            }

            if (SecondaryAxis == null) SecondaryAxis = new NumericalAxis();
        }

        /// <summary>
        ///Set PrimaryAxis for SfChart
        /// </summary>
        internal void SetPrimaryAxis(ChartValueType type)
        {
            switch (type)
            {
                case ChartValueType.Double:
                    PrimaryAxis = new NumericalAxis();
                    break;
                case ChartValueType.DateTime:
                    PrimaryAxis = new DateTimeAxis();
                    break;
                case ChartValueType.String:
                    PrimaryAxis = new CategoryAxis();
                    break;
                case ChartValueType.TimeSpan:
                    PrimaryAxis = new TimeSpanAxis();
                    break;
            }
        }

       

        internal void RenderSeries()
        {
            if (RootPanelDesiredSize != null)
            {
                clearPixels = true;

                byte[] previousBuffer = this.fastBuffer;


                var size = AreaType != ChartAreaType.None
                               ? new Size(SeriesClipRect.Width, SeriesClipRect.Height)
                               : RootPanelDesiredSize.Value;


                if (VisibleSeries != null)
                {
                    foreach (ChartSeriesBase series in VisibleSeries.Where(item => item.Visibility == Visibility.Visible))
                    {
                        series.UpdateOnSeriesBoundChanged(size);
                    }
                }

                if (TechnicalIndicators != null)
                {
                    foreach (FinancialTechnicalIndicator indicator in TechnicalIndicators)
                    {
                        indicator.UpdateOnSeriesBoundChanged(size);
                    }
                }

                if (!CanRenderToBuffer)
                    this.fastBuffer = previousBuffer;
                RenderToBuffer();
            }

#if WINDOWS_PHONE
            isRenderSeriesDispatched = false;
#else
            renderSeriesAction = null;
#endif
            StackedValues = null;
        }
        internal override void UpdateAxisLayoutPanels()
        {
            if (internalCanvas != null)
                internalCanvas.Clip = null;
            AxisThickness = new Thickness(0);
            if (ChartAxisLayoutPanel != null)
            {
                ChartAxisLayoutPanel.DetachElements();
            }

            if (GridLinesLayout != null)
            {
                GridLinesLayout.DetachElements();
            }

            if (chartAxisPanel != null)
            {
                if (AreaType == ChartAreaType.PolarAxes)
                {
                    ChartAxisLayoutPanel = new ChartPolarAxisLayoutPanel(chartAxisPanel)
                        {
                            Area = this
                        };
                    ChartAxisLayoutPanel.UpdateElements();
                    GridLinesLayout = new ChartPolarGridLinesPanel(gridLinesPanel)
                        {
                            Area = this
                        };
                }
                else if (AreaType == ChartAreaType.CartesianAxes)
                {
                    ChartAxisLayoutPanel = new ChartCartesianAxisLayoutPanel(chartAxisPanel)
                        {
                            Area = this
                        };
                    ChartAxisLayoutPanel.UpdateElements();
                    GridLinesLayout = new ChartCartesianGridLinesPanel(gridLinesPanel)
                        {
                            Area = this
                        };
                }
                else
                {
                    ChartAxisLayoutPanel = null;
                    GridLinesLayout = null;
                }
            }
        }

        void OnSfChartSizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var behavior in Behaviors)
            {
                behavior.OnSizeChanged(e);
            }
        }

        /// <summary>
        /// Converts Value to Log point.
        /// </summary>
        /// <param name="axis">The Logarithmic axis .</param>
        /// <param name="value">The value.</param>
        /// <returns>The double value to point</returns>
        [ClassReference(IsReviewed = false)]
        internal override double ValueToLogPoint(ChartAxis axis, double value)
        {
            if (axis != null)
            {
               value = axis is LogarithmicAxis ?Math.Log(value,(axis as LogarithmicAxis).LogarithmicBase):value;
                return ValueToPoint(axis, value);
            }
            return double.NaN;
        }

        /// <summary>
        /// Converts Value to point.
        /// </summary>
        /// <param name="axis">The Chart axis .</param>
        /// <param name="value">The value.</param>
        /// <returns>The double value to point</returns>
        [ClassReference(IsReviewed = false)]
        public override double ValueToPoint(ChartAxis axis, double value)
        {
            if (axis != null)
            {
                if (axis.Orientation == Orientation.Horizontal)
                {
                    return (axis.RenderedRect.Left - axis.Area.SeriesClipRect.Left)
                        + (axis.ValueToCoefficientCalc(value) * axis.RenderedRect.Width);
                }
                return (axis.RenderedRect.Top - axis.Area.SeriesClipRect.Top) + (1 - axis.ValueToCoefficientCalc(value)) * axis.RenderedRect.Height;
            }

            return double.NaN;
        }

        /// <summary>
        /// Converts Value to point.
        /// </summary>
        /// <param name="axis">The Chart axis .</param>
        /// <param name="value">The value.</param>
        /// <returns>The double value to point</returns>
        [ClassReference(IsReviewed = false)]
        public double ValueToPointRelativeToAnnotation(ChartAxis axis, double value)
        {
            if (axis != null)
            {
                if (axis.Orientation == Orientation.Horizontal)
                {
                    return (axis.RenderedRect.Left)
                        + (axis.ValueToCoefficientCalc(value) * axis.RenderedRect.Width);
                }
                return (axis.RenderedRect.Top) + (1 - axis.ValueToCoefficientCalc(value)) * axis.RenderedRect.Height;
            }

            return double.NaN;
        }

        internal double PointToAnnotationValue(ChartAxis axis, Point point)
        {
            if (axis != null)
            {
                if (axis.Orientation == Orientation.Horizontal)
                {
                    return axis.CoefficientToValueCalc((point.X - axis.Area.SeriesClipRect.Left - axis.PlotOffset) / axis.RenderedRect.Width);
                }
                else
                {
                    return axis.CoefficientToValueCalc(1d - ((point.Y - axis.PlotOffset - axis.RenderedRect.Top) / axis.RenderedRect.Height));
                }

            }

            return double.NaN;
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
            var needForceSizeChanged = false;
            double width = availableSize.Width, height = availableSize.Height;

            if (double.IsInfinity(width))
            {
                width = ActualWidth == 0d ? 500d : ActualWidth;
                needForceSizeChanged = true;
            }
            if (double.IsInfinity(height))
            {
                height = ActualHeight == 0d ? 300d : ActualHeight;
                needForceSizeChanged = true;
            }
            if (needForceSizeChanged)
            {
                SizeChanged -= OnSizeChanged;
                SizeChanged += OnSizeChanged;
                AvailableSize = new Size(width, height);
            }
            else
                AvailableSize = availableSize;

            return base.MeasureOverride(AvailableSize);
        }

        void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize != AvailableSize)
                InvalidateMeasure();
        }

        internal void SetAxisForChartSeries(ISupportAxes2D series)
        {
            if (series == null) return;
            var xAxis = series.XAxis;
            ChartAxis yAxis = series.YAxis;
            if (xAxis == null && InternalPrimaryAxis != null)
            {
                series.XAxis = InternalPrimaryAxis as ChartAxisBase2D;
            }
            else if (xAxis != null && xAxis != InternalPrimaryAxis)
            {
                if (!Axes.Contains(xAxis))
                {
                    Axes.Add(xAxis);
                }
            }

            if (yAxis == null && InternalSecondaryAxis != null && !(series is FinancialTechnicalIndicator))
            {
                series.YAxis = this.InternalSecondaryAxis as RangeAxisBase;
            }
            else if (yAxis == null && (series is FinancialTechnicalIndicator))
            {
                if ((series is SimpleAverageIndicator) || series is TriangularAverageIndicator || series is BollingerBandIndicator || series is ExponentialAverageIndicator)
                    series.YAxis = this.InternalSecondaryAxis as RangeAxisBase;
                else
                    series.YAxis = new NumericalAxis() { OpposedPosition = true, RangePadding = NumericalPadding.Round };
            }
            else if (yAxis != null && yAxis != InternalSecondaryAxis)
            {
                if (!Axes.Contains(yAxis))
                {
                    Axes.Add(yAxis);
                }
            }
            if (series.XAxis != null)
                series.XAxis.Area = this;
            if (series.YAxis != null)
                series.YAxis.Area = this;
        }

        internal byte[] GetFastBuffer()
        {
            return this.fastBuffer;
        }

        internal WriteableBitmap GetFastRenderSurface()
        {
            return this.fastRenderSurface;
        }

        internal void CreateBuffer(Size size)
        {
            CanRenderToBuffer = false;
            this.fastBuffer = new byte[(int)(size.Width) * (int)(size.Height) * 4];
        }

        private void LayoutAxis(Size availableSize)
        {
            if (ChartAxisLayoutPanel != null)
            {
                ChartAxisLayoutPanel.UpdateElements();
                ChartAxisLayoutPanel.Measure(availableSize);
                ChartAxisLayoutPanel.Arrange(availableSize);
            }

            foreach (var item in ColumnDefinitions)
            {
                if (gridLinesPanel != null && item != null && item.BorderLine != null && !gridLinesPanel.Children.Contains(item.BorderLine) )
                    gridLinesPanel.Children.Add(item.BorderLine);
            }
            foreach (var item in RowDefinitions)
            {
                if (gridLinesPanel != null && item != null && item.BorderLine != null && !gridLinesPanel.Children.Contains(item.BorderLine))
                    gridLinesPanel.Children.Add(item.BorderLine);
            }

            if (GridLinesLayout != null)
            {
                GridLinesLayout.UpdateElements();
                GridLinesLayout.Measure(availableSize);
                GridLinesLayout.Arrange(availableSize);
#if WPF
                gridLinesPanel.Measure(availableSize);
#endif
            }
        }

        internal void ClearBuffer()
        {
            if (clearPixels)
            {
#if WINDOWS_PHONE

                if (this.fastRenderSurface != null)
                {
#if WPF
                    unsafe
                    {
                        this.fastRenderSurface.Lock();

                        this.fastRenderSurface.Clear();

                        this.fastRenderSurface.Unlock();
                    }
#else
                var pixels = this.fastRenderSurface.Pixels;

                Array.Clear(pixels, 0, pixels.Length);

                this.fastRenderSurface.Invalidate();
#endif
                    
                }

#else
                if (this.fastRenderSurfaceStream != null)
                {
                    CreateBuffer(new Size(SeriesClipRect.Width, SeriesClipRect.Height));
                    this.fastRenderSurface.Clear(this.fastRenderSurfaceStream, this.fastBuffer);
                    this.fastRenderSurface.Invalidate();
                }
#endif
                clearPixels = false;
            }
        }

        internal void RenderToBuffer()
        {
#if!WPF
#if WINDOWS_PHONE
            if (this.fastRenderSurface != null)
            {
                this.fastRenderSurface.Invalidate();
            }
#else
            if (this.fastRenderSurfaceStream != null && this.fastBuffer != null)
            {
                this.fastRenderSurfaceStream.Position = 0;
                this.fastRenderSurfaceStream.Write(this.fastBuffer, 0, this.fastBuffer.Count());
                this.fastRenderSurface.Invalidate();
            }
#endif
#endif
            CanRenderToBuffer = false;
        }

        internal void ScheduleRenderSeries()
        {
#if WINDOWS_PHONE
            if (!isRenderSeriesDispatched)
            {
#if WPF 
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(RenderSeries));
#else
                Dispatcher.BeginInvoke(RenderSeries);
#endif
                isRenderSeriesDispatched = true;
            }
#else

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                RenderSeries();
            else if (renderSeriesAction == null)
                renderSeriesAction = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, RenderSeries);
#endif
        }

        internal double GetPercentage(IList<ISupportAxes> seriesColl, double item, int index, bool reCalculation)
        {
            double totalValues = 0;
            if (reCalculation)
            {
                if (index == 0)
                    sumItems.Clear();
                foreach (var stackingSeries in seriesColl)
                {
                    StackingSeriesBase stackingChart = stackingSeries as StackingSeriesBase;
                    if(!stackingChart.IsSeriesVisible) continue;
                    if (stackingChart != null && stackingChart.YValues.Count != 0)
                        totalValues += Math.Abs(stackingChart.YValues[index]);
                }
                sumItems.Add(totalValues);
            }
            if (sumItems.Count != 0)
                item = (item / sumItems[index]) * 100;
            return item;
        }
     
        /// <summary>
        /// called when lost focus from the sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnLostFocus(e);
            }

            base.OnLostFocus(e);
        }
        /// <summary>
        /// Called when got focus in sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnGotFocus(e);
            }

            base.OnGotFocus(e);
        }

#if WINDOWS_PHONE

#if !WPF

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.DoubleTap"/> event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnDoubleTap(GestureEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnDoubleTap(e);
            }

            base.OnDoubleTap(e);
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.Tap"/> event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnTap(GestureEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnTap(e);
            }

            base.OnTap(e);
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.Hold"/> event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnHold(GestureEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnHold(e);
            }

            base.OnHold(e);
        }

#endif

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseEnter"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event. </param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnMouseEnter(e);
            }

            base.OnMouseEnter(e);
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseLeave"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event. </param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnMouseLeave(e);
            }

            base.OnMouseLeave(e);
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseWheel"/> event occurs to provide handling for the event in a derived class without attaching a delegate. 
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Input.MouseWheelEventArgs"/> that contains the event data.</param>
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnMouseWheel(e);
            }

            base.OnMouseWheel(e);
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseMove"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event. </param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnMouseMove(e);
            }

            base.OnMouseMove(e);
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.KeyUp"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event. </param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnKeyUp(e);
            }

            base.OnKeyUp(e);
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.KeyDown"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event. </param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnKeyDown(e);
            }

            base.OnKeyDown(e);
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event. </param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            #if SILVERLIGHT_UNCOMMON || WPF
            foreach (ChartAxisBase2D axis in Axes)
            {
                axis.isManipulated = (axis.EnableTouchMode) ? true : false;
            }
            #endif
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnMouseLeftButtonDown(e);
            }

            base.OnMouseLeftButtonDown(e);
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseLeftButtonUp"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event. </param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            #if SILVERLIGHT_UNCOMMON || WPF
            foreach (ChartAxisBase2D axis in Axes)
            {
                axis.isManipulated = (axis.EnableTouchMode)? false : true;
            }
            #endif
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnMouseLeftButtonUp(e);
            }

            base.OnMouseLeftButtonUp(e);
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.ManipulationStarted"/> event occurs. 
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnManipulationStarted(ManipulationStartedEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnManipulationStarted(e);
            }

            base.OnManipulationStarted(e);
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.ManipulationCompleted"/>  event occurs. 
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnManipulationCompleted(ManipulationCompletedEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnManipulationCompleted(e);
            }

            base.OnManipulationCompleted(e);
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.ManipulationDelta"/> event occurs. 
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnManipulationDelta(ManipulationDeltaEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnManipulationDelta(e);
            }

            base.OnManipulationDelta(e);
        }

#else
        /// <summary>
        /// Called when Pointercapture lost in sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerCaptureLost(PointerRoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnPointerCaptureLost(e);
            }

            base.OnPointerCaptureLost(e);
        }
        /// <summary>
        /// Called when Tapped in sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnTapped(e);
            }

            base.OnTapped(e);
        }
        /// <summary>
        /// Called when RightTap click in sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRightTapped(RightTappedRoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnRightTapped(e);
            }

            base.OnRightTapped(e);
        }
        /// <summary>
        /// Called when Pointerwheel changed in sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerWheelChanged(PointerRoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnPointerWheelChanged(e);
            }

            base.OnPointerWheelChanged(e);
        }
        /// <summary>
        /// Called when PointerExited from sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnPointerExited(e);
            }

            base.OnPointerExited(e);
        }
        /// <summary>
        /// Called when pointer entered in sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnPointerEntered(e);
            }

            base.OnPointerEntered(e);
        }
        /// <summary>
        /// Called when PointerCancelled in sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerCanceled(PointerRoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnPointerCanceled(e);
            }

            base.OnPointerCanceled(e);
        }
        /// <summary>
        /// called when pointer key up in the sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyUp(KeyRoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnKeyUp(e);
            }

            base.OnKeyUp(e);
        }
        /// <summary>
        /// Called when key down in the sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnKeyDown(e);
            }

            base.OnKeyDown(e);
        }
        /// <summary>
        /// Called when holding the pointer in sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnHolding(HoldingRoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnHolding(e);
            }

            base.OnHolding(e);
        }

        /// <summary>
        /// called when ManipulationStarting in sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnManipulationStarting(ManipulationStartingRoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnManipulationStarting(e);
            }

            base.OnManipulationStarting(e);
        }
        /// <summary>
        /// called when Manipulation Started from sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnManipulationStarted(ManipulationStartedRoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnManipulationStarted(e);
            }

            base.OnManipulationStarted(e);
        }
        /// <summary>
        /// called when manipulation InertiaStarting in sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnManipulationInertiaStarting(ManipulationInertiaStartingRoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnManipulationInertiaStarting(e);
            }

            base.OnManipulationInertiaStarting(e);
        }
        /// <summary>
        /// Called when manipulation completed in sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnManipulationCompleted(ManipulationCompletedRoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnManipulationCompleted(e);
            }

            base.OnManipulationCompleted(e);
        }
        /// <summary>
        /// Called when Manipulation delta changed
        /// </summary>
        /// <param name="e"></param>
        protected override void OnManipulationDelta(Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnManipulationDelta(e);
            }

            base.OnManipulationDelta(e);
        }
        /// <summary>
        /// Called when pointer pressed in sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnPointerPressed(e);
            }

            base.OnPointerPressed(e);
        }
        /// <summary>
        /// Called when pointer moved from sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnPointerMoved(e);
            }

            base.OnPointerMoved(e);
        }
        /// <summary>
        /// Called when pointer released from sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnPointerReleased(e);
            }

            base.OnPointerReleased(e);
        }  
        /// <summary>
        /// Called when Double Tapped the Keys in sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDoubleTapped(DoubleTappedRoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnDoubleTapped(e);
            }

            base.OnDoubleTapped(e);
        }

#endif
        /// <summary>
        /// Called when drop the pointer in sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDrop(DragEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnDrop(e);
            }

            base.OnDrop(e);
        }
        /// <summary>
        /// Called when Drag over from the sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDragOver(DragEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnDragOver(e);
            }

            base.OnDragOver(e);
        }
        /// <summary>
        /// Called when Drag leave from the sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDragLeave(DragEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnDragLeave(e);
            }

            base.OnDragLeave(e);
        }
        /// <summary>
        /// Called when drag enter into the sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDragEnter(DragEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnDragEnter(e);
            }

            base.OnDragEnter(e);
        }

        internal override DependencyObject CloneChart()
        {
            SfChart chart = new SfChart();
            ChartCloning.CloneControl(this, chart);
            chart.Height = double.IsNaN(this.Height) ? this.ActualHeight : this.Height;
            chart.Width = double.IsNaN(this.Width) ? this.ActualWidth : this.Width;
            chart.Header = this.Header;
            chart.Palette = this.Palette;
            chart.AxisThickness = this.AxisThickness;
            chart.AreaBorderBrush = this.AreaBorderBrush;
            chart.AreaBackground = this.AreaBackground;
            chart.AreaBorderThickness = this.AreaBorderThickness;
            chart.SideBySideSeriesPlacement = this.SideBySideSeriesPlacement;
            chart.PrimaryAxis = (ChartAxisBase2D)(this.PrimaryAxis as ICloneable).Clone();
            chart.SecondaryAxis = (RangeAxisBase)(this.SecondaryAxis as ICloneable).Clone();
            if (this.Legend != null)
                chart.Legend = (ChartLegend)(this.Legend as ICloneable).Clone();
            foreach (ChartSeriesBase series in this.Series)
            {
                chart.Series.Add((ChartSeries)(series as ICloneable).Clone());
            }
            foreach (ChartRowDefinition rowDefinition in this.RowDefinitions)
            {
                chart.RowDefinitions.Add((ChartRowDefinition)(rowDefinition as ICloneable).Clone());
            }
            foreach (ChartColumnDefinition columnDefinition in this.ColumnDefinitions)
            {
                chart.ColumnDefinitions.Add((ChartColumnDefinition)(columnDefinition as ICloneable).Clone());
            }
            foreach (Annotation annotation in Annotations)
            {
                chart.Annotations.Add((Annotation)annotation.Clone());
            }
            foreach (ChartBehavior behavior in this.Behaviors)
            {
                if (behavior is ChartTrackBallBehavior || behavior is ChartCrossHairBehavior)
                    chart.Behaviors.Add((ChartBehavior)(behavior as ICloneable).Clone());
                else if (behavior is ChartSelectionBehavior)
                    chart.Behaviors.Add(new ChartSelectionBehavior());
                else if (behavior is ChartZoomPanBehavior)
                    chart.Behaviors.Add(new ChartZoomPanBehavior());
            }
            chart.UpdateArea(true);
            return chart;
        }

        #endregion
    }

    /// <summary>
    /// Represents chart segment selection changed event arguments.
    /// </summary>
    ///<remarks>
    /// It contains information like selected segment and series.
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public class ChartSelectionChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Get or Set selectedSeries property
        /// </summary>
        public ChartSeriesBase SelectedSeries
        {
            get;
            set;
        }
        /// <summary>
        /// Get or Set SelectedSegment property
        /// </summary>
        public ChartSegment SelectedSegment
        {
            get;
            set;
        }
    }


    /// <summary>
    /// Represents chart series bounds changed event arguments.
    /// </summary>
    ///<remarks>
    /// It contains information like old bounds and new bounds.
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public class ChartSeriesBoundsEventArgs : EventArgs
    {
        public ChartSeriesBoundsEventArgs()
        {

        }

        public Rect NewBounds { get; set; }
        public Rect OldBounds { get; set; }
    }

#if WPF

    public delegate void OnPrintingEventHandler(object sender, PrintingEventArgs args);

    public class PrintingEventArgs : EventArgs
    {
        public PrintingEventArgs()
        {
            this.ShowPrintDialog = true;
        }

        private bool showPrintDialog;
        public bool ShowPrintDialog
        {
            get { return showPrintDialog; }
            set { showPrintDialog = value; }
        }

        private bool cancelPrinting;
        public bool CancelPrinting
        {
            get { return cancelPrinting; }
            set { cancelPrinting = value; }
        }

        private Window printDialog;
        public Window PrintDialog
        {
            get { return printDialog; }
            set { printDialog = value; }
        }

        private Visual printVisual;
        public Visual PrintVisual
        {
            get { return printVisual; }
            set { printVisual = value; }
        }
    }
#endif
}
