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
using System.Windows.Input;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Collections.Specialized;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Diagnostics;
#if WPF
using System.Data;
#endif
#if WINDOWS_PHONE
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Windows.Shapes;
using System.Threading;
#else
using Windows.UI;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Defines members and methods necessary to customize the display of selected segment in a <see cref="ChartSeriesBase"/>.
    ///</summary>
    ///<seealso cref="ChartSelectionBehavior"/>
    [ClassReference(IsReviewed = false)]
    public interface ISegmentSelectable
    {
        /// <summary>
        /// Get or Set SegmentSelectionBrush property
        /// </summary>
        Brush SegmentSelectionBrush
        {
            get;
            set;
        }
    }

    /// <summary>
    /// ChartSeries is the base class for all the series types.
    /// </summary>
    /// <remarks>
    /// Data points for ChartSeries would be populated <see cref="ChartSeriesBase"/> from
    /// <see cref="ChartSeriesBase.ItemsSource"/>property. Specify the binding paths for
    /// X-Values and Y-Values. The number of Y-Values may vary depending on the type of
    /// series. For e.g LineSeries requires only one y-value, whereas CandleSeries
    /// requires four y-values to plot a point.
    /// </remarks>
    /// <seealso cref="ChartSegment"/>
    [ClassReference(IsReviewed = false)]
    public abstract class ChartSeriesBase : Control, ICloneable
    {
        #region fields

        private ObservableCollection<ChartAdornment> m_adornments = new ObservableCollection<ChartAdornment>();

        internal ChartAdornmentInfoBase adornmentInfo;

        internal ChartSegment PreviousSelectedSegment = null;
        string[] XComplexPaths;
        string[][] YComplexPaths;
        /// <summary>
        /// segments variable declarations
        /// </summary>
        protected internal ObservableCollection<ChartSegment> Segments = null;
        internal ChartSeriesPanel SeriesPanel;
        internal Panel SeriesRootPanel { get; set; }
        internal DoubleRange SideBySideInfoRangePad { get; set; }
        internal bool IsPointGenerated = false;
        private bool isActualTransposed= false;
        internal bool IsActualTransposed 
        { 
            get { return isActualTransposed;}
            set 
            { 
                isActualTransposed = value; 
                OnActualTransposeChanged(); 
            } 
        }

        private void OnActualTransposeChanged()
        {
            if ((this is CartesianSeries || this is CartesianSeries3D) && this.ActualXAxis != null && this.ActualYAxis != null)
            {
                this.ActualXAxis.Orientation = IsActualTransposed ? Orientation.Vertical : Orientation.Horizontal;
                this.ActualYAxis.Orientation = IsActualTransposed ? Orientation.Horizontal : Orientation.Vertical;
            }
        }

        internal bool HasDataSource = false;
        /// <summary>
        /// ChartTransformer variable declarations
        /// </summary>
        protected IChartTransformer ChartTransformer;
        internal bool needToCreateSegments = true;

        private int dataCount;
        /// <summary>
        /// YPaths variable declarations
        /// </summary>
        protected string[] YPaths;
        internal ChartBase ActualArea { get; set; }
        private bool isNotificationSuspended = false;
        private int updateStartedIndex = -1;
        private bool isUpdateStarted = false;
        private DataTemplate defaultTooltipTemplate;
        private DataTemplate financialTooltipTemplate;
        private bool isPointValidated;
        private DataTemplate bubbleTooltipTemplate;
        private DataTemplate rangeTooltipTemplate;
        private DataTemplate lineTooltipTemplate;

        internal bool canAnimate;
        internal bool CanAnimate
        {
            get
            {
                return (canAnimate && EnableAnimation) || GetAnimationIsActive();
            }
            set
            {
                canAnimate = value;
            }
        }

        internal DispatcherTimer timer;

        internal DoubleAnimation leftDoubleAnimation = null;
        internal DoubleAnimation topDoubleAnimation = null;
        internal Storyboard storyBoard = null;
#if WPF || NETFX_CORE
        internal readonly Stopwatch _stopwatch = new Stopwatch();
#endif

        internal ChartAdornmentPresenter AdornmentPresenter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets the x values in an unsorted order or in the order the data has been added to series.
        /// </summary>
        IEnumerable XValues
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets the sorted values, if the IsSortData is true.
        /// </summary>
        protected internal IEnumerable ActualXValues
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets the y values in an unsorted order or in the order the data has been added to series.
        /// </summary>
        IList<double>[] SeriesYValues
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets the sorted values, if the IsSortData is true.
        /// </summary>
        internal IList<double>[] ActualSeriesYValues
        {
            get;
            set;
        }


        internal List<object> ActualData { get; set; }
        /// <summary>
        /// Gets the number of points given as input.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public int DataCount
        {
            get
            {
                return dataCount;
            }
        }

        /// <summary>
        /// Gets actual series X-axis.
        /// </summary>
        /// <remarks>
        /// Gets actual XAxis for series with respect to chart type and <see cref="ChartSeriesBase.IsRotated"/> value.
        /// </remarks>
        protected internal ChartAxis ActualXAxis
        {
            get
            {
                if (ActualArea != null && this is ISupportAxes)
                {
                    if (ActualArea is SfChart)
                    {
                        return (this as ISupportAxes2D).XAxis
                            ?? ActualArea.InternalPrimaryAxis;
                    }
                    else
                    {
                        return (this as ISupportAxes3D).XAxis
                               ?? ActualArea.InternalPrimaryAxis;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets actual series Y-axis.
        /// </summary>
        protected internal ChartAxis ActualYAxis
        {
            get
            {
                if (ActualArea != null && this is ISupportAxes)
                {
                    if (ActualArea is SfChart)
                    {
                        return (this as ISupportAxes2D).YAxis
                            ?? ActualArea.InternalSecondaryAxis;
                    }
                    else
                    {
                        return (this as ISupportAxes3D).YAxis
                               ?? ActualArea.InternalSecondaryAxis;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        ///Gets or Sets a value whether to sort the datas.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public bool IsSortData
        {
            get { return (bool)GetValue(IsSortDataProperty); }
            set { SetValue(IsSortDataProperty, value); }
        }

        /// <summary>
        /// Gets or Sets the Sorting Direction.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Direction SortDirection
        {
            get { return (Direction)GetValue(SortDirectionProperty); }
            set { SetValue(SortDirectionProperty, value); }
        }

        /// <summary>
        ///Gets or Sets SortingAxis.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public SortingAxis SortBy
        {
            get { return (SortingAxis)GetValue(SortByProperty); }
            set { SetValue(SortByProperty, value); }
        }

        /// <summary>
        /// Gets the Spacing for the SideBySide segments
        /// </summary>
        /// <param name="obj">ChartSeries object</param>
        /// <returns>returns a double value.</returns>
        [ClassReference(IsReviewed = false)]
        public static double GetSpacing(DependencyObject obj)
        {
            return (double)obj.GetValue(SpacingProperty);
        }
        /// <summary>
        /// Sets the Spacing for the SideBySide segments
        /// </summary>
        /// <param name="obj">ChartSeries object</param>
        /// <param name="value">The value to set for calcaulting the segment width</param>
        [ClassReference(IsReviewed = false)]
        public static void SetSpacing(DependencyObject obj, double value)
        {
            obj.SetValue(SpacingProperty, value);
        }


        /// <summary>
        ///  Using a DependencyProperty as the backing store for Spacing.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SpacingProperty =
            DependencyProperty.RegisterAttached("Spacing", typeof(double), typeof(ChartSeriesBase), new PropertyMetadata(0.2d));


        /// <summary>
        /// Gets or Sets stroke thickness for series.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }


        /// <summary>
        /// Using a DependencyProperty as the backing store for StrokeThickness.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(ChartSeriesBase), new PropertyMetadata(2d, OnAppearanceChanged));

        protected internal virtual bool IsSideBySide
        {
            get
            {
                return false;
            }
        }

        protected virtual bool IsStacked
        {
            get { return false; }
        }

        /// <summary>
        /// Used to indicate whether multipleYValues is needed,will be set internally.
        /// </summary>
        internal virtual bool IsMultipleYPathRequired
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the available size of Chart.
        /// </summary>
        /// <returns>returns size</returns>
        [ClassReference(IsReviewed = false)]
        public Size GetAvialableSize()
        {
            return ActualArea.AvailableSize;
        }



        public DataTemplate TooltipTemplate
        {
            get { return (DataTemplate)GetValue(TooltipTemplateProperty); }
            set { SetValue(TooltipTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TooltipTemplateProperty =
            DependencyProperty.Register("TooltipTemplate", typeof(DataTemplate), typeof(ChartSeriesBase), new PropertyMetadata(null, new PropertyChangedCallback(OnTooltipTemplateChanged)));

        private static void OnTooltipTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var series = (ChartSeriesBase)d;
            if (series != null && series.ActualArea != null && series.ActualArea.Tooltip != null)
                ((ChartTooltip)series.ActualArea.Tooltip).ContentTemplate = args.NewValue as DataTemplate;
        }



        public bool ShowTooltip
        {
            get { return (bool)GetValue(ShowTooltipProperty); }
            set { SetValue(ShowTooltipProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowTooltip.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowTooltipProperty =
            DependencyProperty.Register("ShowTooltip", typeof(bool), typeof(ChartSeriesBase), new PropertyMetadata(false, new PropertyChangedCallback(OnShowTooltipChanged)));

        /// <summary>
        /// Gets or Sets listen property change.
        /// </summary>
        public bool ListenPropertyChange
        {
            get { return (bool)GetValue(ListenPropertyChangeProperty); }
            set { SetValue(ListenPropertyChangeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ListenPropertyChange.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ListenPropertyChangeProperty =
            DependencyProperty.Register("ListenPropertyChange", typeof(bool), typeof(ChartSeriesBase), new PropertyMetadata(false, new PropertyChangedCallback(OnListenPropertyChangeChanged)));

        private static void OnListenPropertyChangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            (d as ChartSeriesBase).HookPropertyChangedEvent((bool)args.NewValue);
        }

        private static void OnShowTooltipChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var instance = (ChartSeriesBase)d;
            if (instance != null && instance.ActualArea != null && (bool)args.NewValue == true)
            {
                instance.ActualArea.Tooltip = new ChartTooltip();
            }
            else if (instance != null && instance.ActualArea != null && instance.ActualArea.Tooltip != null)
            {
                var canvas = (instance.ActualArea as SfChart).GetAdorningCanvas();
                if (canvas.Children.Contains((instance.ActualArea.Tooltip as ChartTooltip)))
                    canvas.Children.Remove(instance.ActualArea.Tooltip as ChartTooltip);
            }
        }

        #endregion

        #region ctor
        /// <summary>
        /// Called when instance created for ChartSeries
        /// </summary>
        public ChartSeriesBase()
        {
            DefaultStyleKey = typeof(ChartSeriesBase);
            Segments = new ObservableCollection<ChartSegment>();
            ColorModel = new ChartColorModel(this.Palette);
            XValues = ActualXValues = new List<double>();
            ActualData = new List<object>();
            CanAnimate = true;
            timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
        }


        /// <summary>
        /// Timer Tick Handler for closing the Tooltip
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timer_Tick(object sender, object e)
        {
            RemoveTooltip();
            timer.Stop();
        }

        internal virtual void RemoveTooltip()
        {
            var canvas = ActualArea.GetAdorningCanvas();
            if (canvas == null) return;
            for (int i = 0; i < canvas.Children.Count; )
            {
                if (canvas.Children[i] is ChartTooltip)
                {
                    canvas.Children.Remove(canvas.Children[i]);
                    storyBoard = null;
                    continue;
                }
                i++;
            }

        }


        /// <summary>
        /// Event to show tooltip
        /// </summary>
        /// <param name="e"> Event Arguments</param>
#if WPF 
        protected override void OnMouseMove(MouseEventArgs e)
#elif WINDOWS_PHONE && !SILVERLIGHT_UNCOMMON
        protected override void OnTap(GestureEventArgs e)
#elif SILVERLIGHT
        protected override void OnMouseMove(MouseEventArgs e)
#else
        protected override void OnPointerMoved(PointerRoutedEventArgs e)
#endif
        {
            var canvas = this.ActualArea.GetAdorningCanvas();
#if !NETFX_CORE
            Point mousePos = e.GetPosition(canvas);
#else
            Point mousePos = e.GetCurrentPoint(canvas).Position;
#endif
            if (!(this is ErrorBarSeries))
            UpdateTooltip(e.OriginalSource, mousePos);
        }

        DataTemplate toolTipTemplate;
        internal void UpdateTooltip(object source, Point mousePos)
        {
            if (ShowTooltip)
            {
                if (source as Shape == null || (source as Shape).Tag == null)
                    return;
                timer.Start();
                timer.Interval = new TimeSpan(0, 0, 0, 1);
                var canvas = ActualArea.GetAdorningCanvas();

                var chartTooltip = this.ActualArea.Tooltip as ChartTooltip;

                if (chartTooltip != null && canvas != null)
                {
                    if (!IsTooltipAvailable(canvas))
                    {
                        chartTooltip.Content = ((Shape)source).Tag;
                        if (chartTooltip.Content == null)
                            return;
                        canvas.Children.Add(chartTooltip);
                        chartTooltip.ContentTemplate = this.TooltipTemplate != null
                                                           ? this.TooltipTemplate
                                                           : this.GetTooltipTemplate();
                        if (chartTooltip.ContentTemplate == null)
                        {
                            if (toolTipTemplate == null)
                            {
                                toolTipTemplate = ChartDictionaries.GenericCommonDictionary["DefaultTooltipTemplate"] as DataTemplate;
                            }
                            chartTooltip.ContentTemplate = toolTipTemplate;
                        }
                        chartTooltip.LeftOffset = Position(mousePos, ref chartTooltip).X;
                        chartTooltip.TopOffset = Position(mousePos, ref chartTooltip).Y;
                        chartTooltip.Margin = ChartTooltip.GetTooltipMargin(this);

                        if (ChartTooltip.GetEnableAnimation(this))
                        {
                            SetDoubleAnimation(chartTooltip);
                            storyBoard.Children.Add(topDoubleAnimation);
                            storyBoard.Children.Add(leftDoubleAnimation);
                            storyBoard.Begin();
#if WPF || NETFX_CORE
                            _stopwatch.Start();
#endif
                        }
                        else
                        {
                            Canvas.SetLeft(chartTooltip, chartTooltip.LeftOffset);
                            Canvas.SetTop(chartTooltip, chartTooltip.TopOffset);
#if WPF || NETFX_CORE
                            _stopwatch.Start();
#endif
                        }
                    }
                    else
                    {
                        foreach (var child in canvas.Children)
                        {
                            if (child is ChartTooltip)
                                chartTooltip = child as ChartTooltip;
                        }
                        chartTooltip.Content = ((Shape)source).Tag;
                        if (chartTooltip.Content == null)
                        {
                            RemoveTooltip();
                            return;
                        }
                        chartTooltip.LeftOffset = Position(mousePos, ref chartTooltip).X;
                        chartTooltip.TopOffset = Position(mousePos, ref chartTooltip).Y;
                        chartTooltip.Margin = ChartTooltip.GetTooltipMargin(this);
#if NETFX_CORE
                        if (_stopwatch.ElapsedMilliseconds > 100)
                        {
#endif
                            if (ChartTooltip.GetEnableAnimation(this))
                            {
#if WPF
                            if (_stopwatch.ElapsedMilliseconds > 100)
                            {
#endif

#if WPF || NETFX_CORE
                                _stopwatch.Restart();
#endif
                                if (leftDoubleAnimation == null || topDoubleAnimation == null || storyBoard == null)
                                {
                                    SetDoubleAnimation(chartTooltip);
                                    storyBoard.Children.Add(topDoubleAnimation);
                                    storyBoard.Children.Add(leftDoubleAnimation);
                                    storyBoard.Begin();
                                }
                                else
                                {
                                    leftDoubleAnimation.To = chartTooltip.LeftOffset;
                                    topDoubleAnimation.To = chartTooltip.TopOffset;
                                    storyBoard.Begin();
                                }
#if WPF
                            }
#endif
                        }
                        else
                        {
#if WPF || NETFX_CORE
                            _stopwatch.Restart();
#endif
                            Canvas.SetLeft(chartTooltip, chartTooltip.LeftOffset);
                            Canvas.SetTop(chartTooltip, chartTooltip.TopOffset);
                        }
#if NETFX_CORE
                        }
                        else if (EnableAnimation == false)
                        {
                            Canvas.SetLeft(chartTooltip, chartTooltip.LeftOffset);
                            Canvas.SetTop(chartTooltip, chartTooltip.TopOffset);
                        }
#endif
                    }
                }
            }
        }

        protected virtual bool IsTooltipAvailable(Canvas canvas)
        {
            foreach (var item in canvas.Children)
            {
                if (item is ChartTooltip)
                    return true;
            }
            return false;
        }

        protected virtual void SetDoubleAnimation(ChartTooltip chartTooltip)
        {
            storyBoard = new Storyboard();
            leftDoubleAnimation = new DoubleAnimation
                {
                    To = chartTooltip.LeftOffset,
                    Duration = new Duration(new TimeSpan(0, 0, 0, 0, 200)),
                    EasingFunction = new SineEase() { EasingMode = EasingMode.EaseOut }
                };
            Storyboard.SetTarget(leftDoubleAnimation, chartTooltip);
#if !NETFX_CORE
            Storyboard.SetTargetProperty(leftDoubleAnimation, new PropertyPath("(Canvas.Left)"));
#else
            Storyboard.SetTargetProperty(leftDoubleAnimation, "(Canvas.Left)");
#endif
            topDoubleAnimation = new DoubleAnimation
                {
                    To = chartTooltip.TopOffset,
                    Duration = new Duration(new TimeSpan(0, 0, 0, 0, 200)),
                    EasingFunction = new SineEase() { EasingMode = EasingMode.EaseOut }
                };
            Storyboard.SetTarget(topDoubleAnimation, chartTooltip);
#if !NETFX_CORE
            Storyboard.SetTargetProperty(topDoubleAnimation, new PropertyPath("(Canvas.Top)"));
#else
            Storyboard.SetTargetProperty(topDoubleAnimation, "(Canvas.Top)");
#endif
           
        }

        private void FadeInAnimation(ref ChartTooltip chartTooltip)
        {
            var storyBoard1 = new Storyboard();
            var fadeInAnimation = new DoubleAnimation()
                {
                    From = 0,
                    To = 1,
                    Duration = new Duration(new TimeSpan(0, 0, 0, 1))
                };
            Storyboard.SetTarget(fadeInAnimation, chartTooltip);
#if !NETFX_CORE
            Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath("Opacity"));
#else
            Storyboard.SetTargetProperty(fadeInAnimation, "Opacity");
#endif
            storyBoard1.Children.Add(fadeInAnimation);
            storyBoard1.Begin();

        }

        /// <summary>
        /// Get the Default Template for Tooltip
        /// </summary>
        /// <returns></returns>
        protected DataTemplate GetTooltipTemplate()
        {
            if (TooltipTemplate != null) return TooltipTemplate;

            if (this is FinancialSeriesBase)
            {
                if (financialTooltipTemplate == null)
                {
                    this.financialTooltipTemplate = ChartDictionaries.GenericCommonDictionary["FinancialTooltipTemplate"] as DataTemplate;
                }
                return financialTooltipTemplate;
            }
            else if (this is BubbleSeries)
            {
                if (bubbleTooltipTemplate == null)
                {
                    this.bubbleTooltipTemplate = ChartDictionaries.GenericCommonDictionary["BubbleTooltipTemplate"] as DataTemplate;
                }
                return bubbleTooltipTemplate;
            }
            else if (this is RangeSeriesBase)
            {
                if (rangeTooltipTemplate == null)
                {
                    this.rangeTooltipTemplate = ChartDictionaries.GenericCommonDictionary["RangeTooltipTemplate"] as DataTemplate;
                }
                return rangeTooltipTemplate;
            }
            else if (this is LineSeries || this is SplineSeries)
            {
                if (lineTooltipTemplate == null)
                {
                    this.lineTooltipTemplate = ChartDictionaries.GenericCommonDictionary["LineTooltipTemplate"] as DataTemplate;
                }
                return lineTooltipTemplate;
            }
            else
                if (defaultTooltipTemplate == null)
                {
                    this.defaultTooltipTemplate = ChartDictionaries.GenericCommonDictionary["DefaultTooltipTemplate"] as DataTemplate;

                }
            return defaultTooltipTemplate;
        }

        /// <summary>
        /// Set the Horizontal and Vertical Alignment for Tooltip
        /// </summary>
        /// <param name="mousePos">Current Position</param>
        /// <param name="tooltip">Tooltip instance</param>
        /// <returns></returns>
        protected Point Position(Point mousePos, ref ChartTooltip tooltip)
        {
            var newPostion = mousePos;
            if ((tooltip as UIElement).DesiredSize.Height == 0 || (tooltip as UIElement).DesiredSize.Width == 0)
                (tooltip as UIElement).UpdateLayout();
            switch (ChartTooltip.GetHorizontalAlignment(this))
            {
                case HorizontalAlignment.Left:
                    newPostion.X = mousePos.X - (tooltip as UIElement).DesiredSize.Width;
                    break;
                case HorizontalAlignment.Center:
                    newPostion.X = mousePos.X - (tooltip as UIElement).DesiredSize.Width / 2;
                    break;
                case HorizontalAlignment.Right:
                    newPostion.X = mousePos.X + (tooltip as UIElement).DesiredSize.Width;
                    break;
            }
            switch (ChartTooltip.GetVerticalAlignment(this))
            {
                case VerticalAlignment.Top:
                    newPostion.Y = mousePos.Y - (tooltip as UIElement).DesiredSize.Height;
                    break;
                case VerticalAlignment.Center:
                    newPostion.Y = mousePos.Y - (tooltip as UIElement).DesiredSize.Height / 2;
                    break;
                case VerticalAlignment.Bottom:
                    newPostion.Y = mousePos.Y + (tooltip as UIElement).DesiredSize.Height;
                    break;
            }
            return newPostion;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the adornments collection.
        /// </summary>
        /// <value>The adornments.</value>
        [ClassReference(IsReviewed = false)]
        public ObservableCollection<ChartAdornment> Adornments
        {
            get
            {
                return m_adornments;
            }
        }

        /// <summary>
        /// Gets or Sets visibility for series.
        /// </summary>
        public bool IsSeriesVisible
        {
            get { return (bool)GetValue(IsSeriesVisibleProperty); }
            set { SetValue(IsSeriesVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSeriesVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSeriesVisibleProperty =
            DependencyProperty.Register("IsSeriesVisible", typeof(bool), typeof(ChartSeriesBase), new PropertyMetadata(true, OnIsSeriesVisibleChanged));

        /// <summary>
        ///  Identifies the XBindingPath dependency property.
        /// </summary>
        public static readonly DependencyProperty XBindingPathProperty =
        DependencyProperty.Register("XBindingPath", typeof(string), typeof(ChartSeriesBase), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnBindingPathXChanged)));


        /// <summary>
        ///  Using a DependencyProperty as the backing store for SortBy.  
        /// </summary>
        public static readonly DependencyProperty SortByProperty =
            DependencyProperty.Register("SortBy", typeof(SortingAxis), typeof(ChartSeriesBase), new PropertyMetadata(SortingAxis.X, new PropertyChangedCallback(OnSortDataOrderChanged)));


        /// <summary>
        /// Using a DependencyProperty as the backing store for SortDirection.  
        /// </summary>
        public static readonly DependencyProperty SortDirectionProperty =
            DependencyProperty.Register("SortDirection", typeof(Direction), typeof(ChartSeriesBase), new PropertyMetadata(Direction.Ascending, new PropertyChangedCallback(OnSortDataOrderChanged)));


        /// <summary>
        /// Using a DependencyProperty as the backing store for IsSortData. 
        /// </summary>
        public static readonly DependencyProperty IsSortDataProperty =
            DependencyProperty.Register("IsSortData", typeof(bool), typeof(ChartSeriesBase), new PropertyMetadata(false, new PropertyChangedCallback(OnSortDataOrderChanged)));

        /// <summary>
        /// Gets or Sets ChartPalette for series.
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
            DependencyProperty.Register("Palette", typeof(ChartColorPalette), typeof(ChartSeriesBase), new PropertyMetadata(ChartColorPalette.None, OnPaletteChanged));
#if WPF
        /// <summary>
        /// Gets or sets an IEnumerable source used to generate Chart.
        /// </summary>
        /// <value>The DataSource value.</value>
        [ClassReference(IsReviewed = false)]
        public object ItemsSource
        {
            get
            {
                return (object)GetValue(ItemsSourceProperty);
            }

            set
            {
                 SetValue(ItemsSourceProperty, value);
            }
        }

        /// <summary>
        ///  Identifies the ItemsSource dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register("ItemsSource", typeof(object), typeof(ChartSeriesBase), new PropertyMetadata(null, new PropertyChangedCallback(OnDataSourceChanged)));
#else
        /// <summary>
        /// Gets or sets an IEnumerable source used to generate Chart.
        /// </summary>
        /// <value>The DataSource value.</value>
        [ClassReference(IsReviewed = false)]
        public IEnumerable ItemsSource
        {
            get
            {
                return (IEnumerable)GetValue(ItemsSourceProperty);
            }

            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        /// <summary>
        ///  Identifies the ItemsSource dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(ChartSeriesBase), new PropertyMetadata(null, new PropertyChangedCallback(OnDataSourceChanged)));
#endif
        /// <summary>
        /// Gets or Sets DataTemplate used to display label, when ChartTrackBallBehavior is used.
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
            DependencyProperty.Register("TrackBallLabelTemplate", typeof(DataTemplate), typeof(ChartSeriesBase), null);

        private ChartValueType xValueType;

        internal ChartValueType XAxisValueType
        {
            get
            {
                return xValueType;
            }
            set
            {
                xValueType = value;
            }
        }

        /// <summary>
        /// Gets or Sets the brush to paint the interior of the series.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Brush Interior
        {
            get { return (Brush)GetValue(InteriorProperty); }
            set { SetValue(InteriorProperty, value); }
        }


        /// <summary>
        /// Using a DependencyProperty as the backing store for Interior.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty InteriorProperty =
            DependencyProperty.Register("Interior", typeof(Brush), typeof(ChartSeriesBase), new PropertyMetadata(null, OnAppearanceChanged));

        /// <summary>
        /// Gets or Sets the label that will be displayed in the associated legend item.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }


        /// <summary>
        ///  Using a DependencyProperty as the backing store for Label.  This enables animation, styling, binding, etc..
        /// </summary>
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(ChartSeriesBase), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or Sets a value that indicates whether to treat x values as categories. 
        /// </summary>
        [ClassReference(IsReviewed = false)]
        protected internal bool IsIndexed
        {
            get { return this.ActualXAxis is CategoryAxis || ActualXAxis is CategoryAxis3D || this.ActualXAxis is DateTimeCategoryAxis; }
        }

        /// <summary>
        /// Gets or Sets ChartLegendIcon to be displayed in associated legend item.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public ChartLegendIcon LegendIcon
        {
            get { return (ChartLegendIcon)GetValue(LegendIconProperty); }
            set { SetValue(LegendIconProperty, value); }
        }


        /// <summary>
        ///  Using a DependencyProperty as the backing store for LegendIcon.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LegendIconProperty =
            DependencyProperty.Register("LegendIcon", typeof(ChartLegendIcon), typeof(ChartSeriesBase), new PropertyMetadata(ChartLegendIcon.Rectangle, OnLegendIconChanged));

        /// <summary>
        /// Gets or Sets DataTemplate for legend icon.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public DataTemplate LegendIconTemplate
        {
            get { return (DataTemplate)GetValue(LegendIconTemplateProperty); }
            set { SetValue(LegendIconTemplateProperty, value); }
        }


        /// <summary>
        ///  Using a DependencyProperty as the backing store for LegendIconTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LegendIconTemplateProperty =
            DependencyProperty.Register("LegendIconTemplate", typeof(DataTemplate), typeof(ChartSeriesBase), new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets a value that determines whether to create a legend item for this series. By default, legend item will be visible for this series.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Visibility VisibilityOnLegend
        {
            get { return (Visibility)GetValue(VisibilityOnLegendProperty); }
            set { SetValue(VisibilityOnLegendProperty, value); }
        }


        /// <summary>
        ///  Using a DependencyProperty as the backing store for VisibilityOnLegend.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty VisibilityOnLegendProperty =
            DependencyProperty.Register("VisibilityOnLegend", typeof(Visibility), typeof(ChartSeriesBase), new PropertyMetadata(Visibility.Visible,new PropertyChangedCallback(OnVisibilityOnLegendChanged)));
      
       private static void OnVisibilityOnLegendChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
       {
           var instance = (ChartSeriesBase)d;
           if (e.NewValue != null && (Visibility)e.NewValue == Visibility.Visible && instance != null && instance.ActualArea !=null )
               instance.ActualArea.UpdateLegend(instance.ActualArea.Legend, false);
       }
        /// <summary>
        /// Gets or Sets the brush to paint outline of the series.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }


        /// <summary>
        /// Using a DependencyProperty as the backing store for Stroke.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(ChartSeriesBase), new PropertyMetadata(null, OnAppearanceChanged));


        /// <summary>
        /// Gets or Sets the ChartColorModel.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public ChartColorModel ColorModel
        {
            get { return (ChartColorModel)GetValue(ColorModelProperty); }
            set { SetValue(ColorModelProperty, value); }
        }


        /// <summary>
        /// Using a DependencyProperty as the backing store for ColorModel.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ColorModelProperty =
            DependencyProperty.Register("ColorModel", typeof(ChartColorModel), typeof(ChartSeriesBase), new PropertyMetadata(null, OnColorModelChanged));



        /// <summary>
        /// Gets or Sets the property path of the x data in ItemsSource.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public string XBindingPath
        {
            get
            {
                return (string)GetValue(XBindingPathProperty);
            }

            set
            {
                SetValue(XBindingPathProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value to animate the chart series on loading and whenever ItemsSource change.
        /// </summary>
        public bool EnableAnimation
        {
            get { return (bool)GetValue(EnableAnimationProperty); }
            set { SetValue(EnableAnimationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableAnimation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableAnimationProperty =
            DependencyProperty.Register("EnableAnimation", typeof(bool), typeof(ChartSeriesBase), new PropertyMetadata(false));

        /// <summary>
        /// Gets or Sets the duration of the animation.
        /// </summary>
        public TimeSpan AnimationDuration
        {
            get { return (TimeSpan)GetValue(AnimationDurationProperty); }
            set { SetValue(AnimationDurationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AnimationDuration.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnimationDurationProperty =
            DependencyProperty.Register("AnimationDuration", typeof(TimeSpan), typeof(ChartSeriesBase), new PropertyMetadata(TimeSpan.FromSeconds(0.8)));

        #endregion

        #region methods

        internal virtual void OnTransposeChanged(bool val)
        {
            IsActualTransposed = val;
        }

        internal object GetActualXValue(int index)
        {
            switch (XAxisValueType)
            {
                case ChartValueType.DateTime:
                    {
                        return ((IList<double>)ActualXValues)[index].FromOADate();
                    }

                case ChartValueType.String:
                    {
                        return ((IList<string>)ActualXValues)[index];
                    }

                case ChartValueType.TimeSpan:
                    {
                        return TimeSpan.FromMilliseconds(((IList<double>)ActualXValues)[index]);
                    }
                default:
                    return ((IList<double>)ActualXValues)[index];
            }
        }

        private static void OnIsSeriesVisibleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            (obj as ChartSeriesBase).IsSeriesVisibleChanged(args);
        }

        private void IsSeriesVisibleChanged(DependencyPropertyChangedEventArgs args)
        {
            if (ActualArea != null)
            {
                if ((bool)args.NewValue)
                {
                    if (ActualArea.ActualSeries.Contains(this) && !ActualArea.VisibleSeries.Contains(this) && !(this is PolarRadarSeriesBase))
                    {
                        int pos = ActualArea.GetSeriesIndex(this);
                        int count = ActualArea.VisibleSeries.Count;
                        ActualArea.VisibleSeries.Insert(pos > count ? count : pos, this);
                    }
                    Visibility = Visibility.Visible;
                    if (this is CartesianSeries)
                    {
                        foreach (var item in (this as CartesianSeries).Trendlines)
                        {
                            item.IsTrendlineVisible = true;
                        }
                    }

                }
                else
                {
                    if (ActualArea.VisibleSeries.Contains(this) && !(this is PolarRadarSeriesBase))
                    {
                        ActualArea.VisibleSeries.Remove(this);
                    }
                    Visibility = Visibility.Collapsed;
                    if (this is CartesianSeries)
                    {
                        foreach (var item in (this as CartesianSeries).Trendlines)
                        {
                            item.IsTrendlineVisible = false;
                        }
                    }
                }
                ActualArea.SBSInfoCalculated = false;
                if (ActualArea is SfChart)
                    (ActualArea as SfChart).AddOrRemoveBitmap();
                UpdateArea();
            }
        }

        internal virtual bool GetAnimationIsActive()
        {
            return false;
        }

        #region Series Utilities

        private bool totalCalculated = false;

        private double grandTotal = 0d;

        internal double GetGrandTotal(IList<double> yValues)
        {
            if (!totalCalculated)
            {
                grandTotal = (from val in yValues select val).Sum();
                totalCalculated = true;
            }
            return grandTotal;
        }

        internal List<double> Clone(IList<double> Values)
        {
            List<double> newValues = new List<double>();
            newValues.AddRange(Values);
            return newValues;
        }
        internal IEnumerable Clone(IEnumerable XValues)
        {
            IEnumerable newValues = new List<double>();

            if (XValues as List<double> != null)
            {
                List<double> newxValues = newValues as List<double>;
                List<double> xValues = XValues as List<double>;
                newxValues.AddRange(xValues);
                dataCount = newxValues.Count;
            }
            else if (XValues as List<string> != null)
            {
                List<string> newxValues = newValues as List<string>;
                List<string> xValues = XValues as List<string>;
                newxValues.AddRange(xValues);
                dataCount = newxValues.Count;
            }


            return newValues;
        }
        #endregion

        /// <summary>
        /// Finds the nearest point in ChartSeries relative to the mouse point/touch position.
        /// </summary>
        /// <param name="point">The co-ordinate point representing the current mouse point /touch position.</param>
        /// <param name="x">x-value of the nearest point.</param>
        /// <param name="y">y-value of the nearest point</param>
        /// <param name="stackedYValue"></param>
        [ClassReference(IsReviewed = false)]
        public virtual void FindNearestChartPoint(Point point, out double x, out double y, out double stackedYValue)
        {
            x = double.NaN;
            y = double.NaN;
            stackedYValue = double.NaN;
            Point nearPoint = new Point();
            if (this.IsIndexed || !(this.ActualXValues is IList<double>))
            {
                if (ActualArea != null)
                {
                    double xStart = ActualXAxis.VisibleRange.Start;
                    double xEnd = ActualXAxis.VisibleRange.End;
                    point = new Point(ActualArea.PointToValue(ActualXAxis, point), ActualArea.PointToValue(ActualYAxis, point));
                    double range = Math.Round(point.X);
                    if (ActualSeriesYValues.Count() > 0)
                    {
                        var count = ActualSeriesYValues[0].Count;
                        if (range <= xEnd && range >= xStart && range < count && range >= 0)
                        {
                            y = ActualSeriesYValues[0][(int)range];
                            x = range;
                    }
                            
                }
            }
            }
            else
            {
                IList<double> xValues = this.ActualXValues as IList<double>;
                nearPoint.X = ActualXAxis.VisibleRange.Start;
                nearPoint.Y = ActualYAxis.VisibleRange.Start;
                point = new Point(ActualArea.PointToValue(ActualXAxis, point), ActualArea.PointToValue(ActualYAxis, point));
                for (int i = 0; i < DataCount; i++)
                {
                    double x1 = xValues[i];
                    double y1 = this.ActualSeriesYValues[0][i];

                    if (Math.Abs((point.X - x1)) <= Math.Abs((point.X - nearPoint.X)))
                    {
                        nearPoint = new Point(x1, y1);
                        x = xValues[i];
                        y = this.ActualSeriesYValues[0][i];
                    }
                }
            }

        }

        //public virtual ChartPoint[] FindNearestChartPoints(Point point)
        //{
        //    ChartPoint nearStartPoint=new ChartPoint();
        //    ChartPoint nearEndPoint=new ChartPoint();
        //    ChartPoint nearChartPoint = FindNearestChartPoint(point);
        //    int index = Data.IndexOf(nearChartPoint);

        //    if (index - 1 > 0 && index + 1 < Data.Count)
        //    {
        //        if (nearChartPoint.Index > Data[index - 1].Index && nearChartPoint.Index < Data[index + 1].Index)
        //        {
        //            nearStartPoint = Data[index - 1];
        //            nearEndPoint = Data[index + 1];
        //        }
        //    }
        //    else
        //    {
        //        if (index - 1 < 0)
        //        {
        //            nearStartPoint = Data[0];
        //            nearEndPoint = Data[1];
        //        }
        //        else
        //        {
        //            nearStartPoint = Data[Data.Count-2];
        //            nearEndPoint = Data[Data.Count-1];
        //        }

        //    }


        //    return new ChartPoint[] { nearStartPoint, nearChartPoint, nearEndPoint };
        //}
        /// <summary>
        /// Method implementation for UpdateArea
        /// </summary>
        protected void UpdateArea()
        {
            if (ActualArea != null)
            {
                ActualArea.ScheduleUpdate();
            }
        }
        /// <summary>
        /// method declaration for generatepoints in Chartseries
        /// </summary>
        protected internal abstract void GeneratePoints();

        /// <summary>
        /// Return the previous series
        /// </summary>
        /// <param name="series"></param>
        /// <returns></returns>
        protected ChartSeriesBase GetPreviousSeries(ChartSeriesBase series)
        {
            int srIndex = this.ActualArea.VisibleSeries.IndexOf(series) - 1;

            if (srIndex == -1)
                return null;
            return this.ActualArea.VisibleSeries[srIndex];
        }

        private static void OnColorModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartSeriesBase).OnColorModelChanged();
        }

        private void OnColorModelChanged()
        {
            if (ColorModel != null)
                ColorModel.Palette = Palette;
            if (GetType().ToString().Contains("Bitmap") || this is ChartSeries3D)
                UpdateArea();
            else
                foreach (var segment in Segments)
                {
                    segment.BindProperties();
                }
        }

        private static void OnLegendIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartSeriesBase).UpdateLegendIconTemplate(true);
        }

        private static void OnSortDataOrderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeriesBase series = d as ChartSeriesBase;
            if (series.IsPointGenerated)
            {
                if (e.Property == IsSortDataProperty)
                {
                    if (series.IsSortData)
                    {
                        series.CreateYValueCollection(series.YPaths.Length);

                        for (int i = 0; i < series.YPaths.Length; i++)
                        {
                            for (int j = 0; j < series.ActualSeriesYValues[i].Count; j++)
                            {
                                series.SeriesYValues[i].Add(series.ActualSeriesYValues[i][j]);
                            }
                        }
                        series.SortActualPoints();
                    }
                    else
                    {
                        for (int i = 0; i < series.YPaths.Length; i++)
                        {
                            series.ActualSeriesYValues[i].Clear();
                            for (int j = 0; j < series.SeriesYValues[i].Count; j++)
                            {
                                series.ActualSeriesYValues[i].Add(series.SeriesYValues[i][j]);
                            }
                        }
                        series.ActualXValues = series.XValues;
                    }
                }
                else
                {
                    if (series.IsSortData)
                        series.SortChartPoints();
                }

                series.UpdateArea();
            }
        }

        private static void OnPaletteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartSeriesBase).OnPaletteChanged(e);
        }

        private void OnPaletteChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ColorModel != null)
                ColorModel.Palette = this.Palette;
            else
                ColorModel = new ChartColorModel(this.Palette);
            if (this.ActualArea != null)
            {
                this.Segments.Clear();
                UpdateArea();
            }
        }

        /// <summary>
        /// Gets or Sets a value that determines how to calculate value for empty point.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public EmptyPointValue EmptyPointValue
        {
            get { return (EmptyPointValue)GetValue(EmptyPointValueProperty); }
            set { SetValue(EmptyPointValueProperty, value); }
        }


        /// <summary>
        /// Using a DependencyProperty as the backing store for EmptyPointValue.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EmptyPointValueProperty =
            DependencyProperty.Register("EmptyPointValue", typeof(EmptyPointValue), typeof(ChartSeriesBase), new PropertyMetadata(EmptyPointValue.Zero, new PropertyChangedCallback(OnEmptyPointValueChanged)));


        /// <summary>
        /// Gets or Sets EmptyPointStyle for an empty point. It determines how to differentiate empty point from other data points.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public EmptyPointStyle EmptyPointStyle
        {
            get { return (EmptyPointStyle)GetValue(EmptyPointStyleProperty); }
            set { SetValue(EmptyPointStyleProperty, value); }
        }


        /// <summary>
        /// Using a DependencyProperty as the backing store for EmptyPointStyle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EmptyPointStyleProperty =
            DependencyProperty.Register("EmptyPointStyle", typeof(EmptyPointStyle), typeof(ChartSeriesBase), new PropertyMetadata(EmptyPointStyle.Interior, new PropertyChangedCallback(OnEmptyPointStyleChanged)));

        private static void OnEmptyPointStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartSeriesBase).RevalidateEmptyPointsStyle();
        }

        private void RevalidateEmptyPointsStyle()
        {
            if (Segments.Count > 0)
            {
                UpdateArea();
            }
        }


        /// <summary>
        /// Gets or Sets DataTemplate to be used when EmptyPointStyle is set to Symbol/ SymbolAndInterior. By default, an ellipse will be displayed as symbol.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public DataTemplate EmptyPointSymbolTemplate
        {
            get { return (DataTemplate)GetValue(EmptyPointSymbolTemplateProperty); }
            set { SetValue(EmptyPointSymbolTemplateProperty, value); }
        }


        /// <summary>
        ///  Using a DependencyProperty as the backing store for EmptyPointSymbolTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EmptyPointSymbolTemplateProperty =
            DependencyProperty.Register("EmptyPointSymbolTemplate", typeof(DataTemplate), typeof(ChartSeriesBase), new PropertyMetadata(null));



        private static void OnEmptyPointValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartSeriesBase).RevalidateEmptyPointsValue();
        }

    

        /// <summary>
        /// Gets or Sets a value whether to show empty points.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public bool ShowEmptyPoints
        {
            get { return (bool)GetValue(ShowEmptyPointsProperty); }
            set { SetValue(ShowEmptyPointsProperty, value); }
        }


        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowEmptyPoints.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowEmptyPointsProperty =
            DependencyProperty.Register("ShowEmptyPoints", typeof(bool), typeof(ChartSeriesBase), new PropertyMetadata(false, OnShowEmptyPointsChanged));

        private static void OnShowEmptyPointsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartSeriesBase).UpdateArea();
            //if ((bool)e.NewValue && (d as ChartSeriesBase).SeriesYValues != null
            //    && !String.IsNullOrEmpty((d as ChartSeriesBase).XBindingPath.ToString()) && (d as ChartSeriesBase).YPaths != null && (d as ChartSeriesBase).ItemsSource != null)
            //    (d as ChartSeriesBase).ValidateDataPoints((d as ChartSeriesBase).SeriesYValues);
        }

        /// <summary>
        /// Gets or Sets interior color for empty point.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Brush EmptyPointInterior
        {
            get { return (Brush)GetValue(EmptyPointInteriorProperty); }
            set { SetValue(EmptyPointInteriorProperty, value); }
        }


        /// <summary>
        /// Using a DependencyProperty as the backing store for EmptyPointInterior.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EmptyPointInteriorProperty =
            DependencyProperty.Register("EmptyPointInterior", typeof(Brush), typeof(ChartSeriesBase), new PropertyMetadata(new SolidColorBrush(Colors.Green),OnEmptyPointInteriorChanged));

        private static void OnEmptyPointInteriorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartSeriesBase).RevalidateEmptyPointsValue();
        }

        internal List<int>[] EmptyPointIndexes;

        internal int SeriesYCount;

#if WINDOWS_PHONE
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        public override void OnApplyTemplate()
#else
        /// <summary>
        /// Invoke to render chart series.
        /// </summary>
        protected override void OnApplyTemplate()
#endif
        {
            UpdateArea();
            base.OnApplyTemplate();
           
        }
        /// <summary>
        /// Method implementation for Set points to given index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="obj"></param>
        /// <param name="replace"></param>
        protected void SetIndividualPoint(int index, object obj, bool replace)
        {
            if (SeriesYValues != null && YPaths != null && ItemsSource != null)
            {
                object xvalueType = null;
                xvalueType = GetPropertyValue(obj,XComplexPaths);
                if (xvalueType != null)
                    XAxisValueType = GetDataType(xvalueType);
                if (IsMultipleYPathRequired)
                {
                    if (XAxisValueType == ChartValueType.String)
                    {
                        if (!(this.XValues is List<string>))
                            this.XValues = this.ActualXValues = new List<string>();
                        IList<string> xValue = this.XValues as List<string>;
                        object xVal = GetPropertyValue(obj, XComplexPaths);
                        if (replace && xValue.Count > index)
                        {
                            xValue[index] = (string)xVal;
                        }
                        else
                        {
                            xValue.Insert(index, (string)xVal);
                        }
                        
                        for (int i = 0; i < YPaths.Count(); i++)
                        {
                            if (replace && SeriesYValues[i].Count > index)
                            {
                                SeriesYValues[i][index] = Convert.ToDouble(GetPropertyValue(obj, YComplexPaths[i]));
                            }
                            else
                            {
                                SeriesYValues[i].Insert(index, Convert.ToDouble(GetPropertyValue(obj, YComplexPaths[i])));
                            }
                        }
                        ActualData.Add(obj);
                        dataCount = xValue.Count;
                    }
                    else if (XAxisValueType == ChartValueType.Double ||
                       XAxisValueType == ChartValueType.Logarithmic)
                    {
                        if (!(this.XValues is List<double>))
                            this.XValues = this.ActualXValues = new List<double>();
                        IList<double> xValue = this.XValues as List<double>;
                        object xVal = GetPropertyValue(obj, XComplexPaths);
                        if (replace && xValue.Count > index)
                        {
                            xValue[index] = Convert.ToDouble(xVal);
                        }
                        else
                        {
                            xValue.Insert(index, Convert.ToDouble(xVal));
                        }
                        
                        for (int i = 0; i < YPaths.Count(); i++)
                        {
                             if (replace && SeriesYValues[i].Count > index)
                             {
                                SeriesYValues[i][index] = Convert.ToDouble(GetPropertyValue(obj, YComplexPaths[i]));
                             }
                             else
                             {
                                 SeriesYValues[i].Insert(index, Convert.ToDouble(GetPropertyValue(obj, YComplexPaths[i])));
                             }
                        }
                        ActualData.Add(obj);
                        dataCount = xValue.Count;
                    }
                    else if (XAxisValueType == ChartValueType.DateTime)
                    {
                        if (!(this.XValues is List<double>))
                            this.XValues = this.ActualXValues = new List<double>();
                        IList<double> xValue = this.XValues as List<double>;
                        object xVal = GetPropertyValue(obj, XComplexPaths);
                        if (replace && xValue.Count > index)
                        {
                            xValue[index] = Convert.ToDateTime(xVal).ToOADate();
                        }
                        else
                        {
                            xValue.Insert(index, Convert.ToDateTime(xVal).ToOADate());
                        }
                        for (int i = 0; i < YPaths.Count(); i++)
                        {
                            if (replace && SeriesYValues[i].Count > index)
                            {
                               SeriesYValues[i][index] = Convert.ToDouble(GetPropertyValue(obj, YComplexPaths[i]));
                            }
                            else
                            {
                               SeriesYValues[i].Insert(index, Convert.ToDouble(GetPropertyValue(obj, YComplexPaths[i])));
                            }
                        }
                        ActualData.Add(obj);
                        dataCount = xValue.Count;
                    }
                    else if (XAxisValueType == ChartValueType.TimeSpan)
                    {
                        if (!(this.XValues is List<double>))
                            this.XValues = this.ActualXValues = new List<double>();
                        IList<double> xValue = this.XValues as List<double>;
                        object xVal = GetPropertyValue(obj, XComplexPaths);
                        if (replace && xValue.Count > index)
                        {
                            xValue[index] = ((TimeSpan)xVal).TotalMilliseconds;
                        }
                        else
                        {
                            xValue.Insert(index, ((TimeSpan)xVal).TotalMilliseconds);
                        }
                        for (int i = 0; i < YPaths.Count(); i++)
                        {
                            if (replace && SeriesYValues[i].Count > index)
                            {
                                SeriesYValues[i][index] = Convert.ToDouble(GetPropertyValue(obj, YComplexPaths[i]));
                            }
                            else
                            {
                                SeriesYValues[i].Insert(index, Convert.ToDouble(GetPropertyValue(obj, YComplexPaths[i])));
                            }
                        }
                        ActualData.Add(obj);
                        dataCount = xValue.Count;
                    }
                }
                else
                {
                    string[] tempYPath = YComplexPaths[0];
                    IList<double> yValue = SeriesYValues[0];
                    if (XAxisValueType == ChartValueType.String)
                    {
                        if (!(this.XValues is List<string>))
                            this.XValues = this.ActualXValues = new List<string>();
                        IList<string> xValue = this.XValues as List<string>;
                        object xVal = GetPropertyValue(obj, XComplexPaths);
                        object yVal = GetPropertyValue(obj, tempYPath);
                        if (replace && xValue.Count > index)
                        {
                            xValue[index] = Convert.ToString(xVal);
                        }
                        else
                        {
                            xValue.Insert(index, Convert.ToString(xVal));
                        }
                        if (replace && yValue.Count > index)
                        {
                            yValue[index] = Convert.ToDouble(yVal);
                        }
                        else
                        {
                            yValue.Insert(index, Convert.ToDouble(yVal));
                        }
                        ActualData.Add(obj);
                        dataCount = xValue.Count;
                    }
                    else if (XAxisValueType == ChartValueType.Double ||
                        XAxisValueType == ChartValueType.Logarithmic)
                    {
                        if (!(this.XValues is List<double>))
                            this.XValues = this.ActualXValues = new List<double>();
                        IList<double> xValue = this.XValues as List<double>;
                        object xVal = GetPropertyValue(obj, XComplexPaths);
                        object yVal = GetPropertyValue(obj, tempYPath);
                        if (replace && xValue.Count > index)
                        {
                            xValue[index] = Convert.ToDouble(xVal);
                        }
                        else
                        {
                            xValue.Insert(index, Convert.ToDouble(xVal));
                        }
                        if (replace && yValue.Count > index)
                        {
                            yValue[index] = Convert.ToDouble(yVal);
                        }
                        else
                        {
                            yValue.Insert(index, Convert.ToDouble(yVal));
                        }
                        ActualData.Add(obj);
                        dataCount = xValue.Count;
                    }
                    else if (XAxisValueType == ChartValueType.DateTime)
                    {
                        if (!(this.XValues is List<double>))
                            this.XValues = this.ActualXValues = new List<double>();
                        IList<double> xValue = this.XValues as List<double>;
                        object xVal = GetPropertyValue(obj, XComplexPaths);
                        object yVal = GetPropertyValue(obj, tempYPath);
                        if (replace && xValue.Count > index)
                        {
                            xValue[index] = Convert.ToDateTime(xVal).ToOADate();
                        }
                        else
                        {
                            xValue.Insert(index, Convert.ToDateTime(xVal).ToOADate());
                        }
                        if (replace && yValue.Count > index)
                        {
                            yValue[index] = Convert.ToDouble(yVal);
                        }
                        else
                        {
                            yValue.Insert(index, Convert.ToDouble(yVal));
                        }
                        ActualData.Add(obj);
                        dataCount = xValue.Count;
                    }
                    else if (XAxisValueType == ChartValueType.TimeSpan)
                    {
                        if (!(this.XValues is List<double>))
                            this.XValues = this.ActualXValues = new List<double>();
                        IList<double> xValue = this.XValues as List<double>;
                        object xVal = GetPropertyValue(obj, XComplexPaths);
                        object yVal = GetPropertyValue(obj, tempYPath);
                        if (xVal != null && replace && xValue.Count > index)
                        {
                            xValue[index] = ((TimeSpan)xVal).TotalMilliseconds;
                        }
                        else if (xVal != null)
                        {
                            xValue.Insert(index, ((TimeSpan)xVal).TotalMilliseconds);
                        }
                        if (yVal != null && replace && yValue.Count > index)
                        {
                            yValue[index] = Convert.ToDouble(yVal);
                        }
                        else if(yVal != null)
                        {
                            yValue.Insert(index, Convert.ToDouble(yVal));
                        }
                        ActualData.Add(obj);
                        dataCount = xValue.Count;
                    }
                }
                totalCalculated = false;
            }
            UpdateEmptyPoints(index);
        }

        private void UpdateEmptyPoints(int index)
        {
            if (EmptyPointIndexes != null && EmptyPointIndexes.Count() > 0 && ActualArea is SfChart && (ActualArea as SfChart).Series != null && (ActualArea as SfChart).Series.Count > 0)
                foreach (var emptyPointIndex in EmptyPointIndexes[(ActualArea as SfChart).Series.IndexOf(this as ChartSeries)])
                {
                    if (emptyPointIndex == index)
                    {
                        EmptyPointIndexes[(ActualArea as SfChart).Series.IndexOf(this as ChartSeries)].Remove(emptyPointIndex);
                        Segments[index].IsEmptySegmentInterior = false;
                        break;
                    }
                }
            else if (EmptyPointIndexes != null && EmptyPointIndexes.Count() > 0 && ActualArea is SfChart3D && (ActualArea as SfChart3D).Series != null && (ActualArea as SfChart3D).Series.Count > 0)
                foreach (var emptyPointIndex in EmptyPointIndexes[(ActualArea as SfChart3D).Series.IndexOf(this as ChartSeries3D)])
                {
                    if (emptyPointIndex == index)
                    {
                        EmptyPointIndexes[(ActualArea as SfChart3D).Series.IndexOf(this as ChartSeries3D)].Remove(emptyPointIndex);
                        Segments[index].IsEmptySegmentInterior = false;
                        break;
                    }
                }
        }
        private void CreateYValueCollection(int count)
        {
            SeriesYValues = new IList<double>[count];
            for (int i = 0; i < count; i++)
            {
                SeriesYValues[i] = new List<double>();
            }
        }
        /// <summary>
        /// Method implemetation for  GeneratePoints for ChartSeries 
        /// </summary>
        /// <param name="yPaths"></param>
        /// <param name="yValueLists"></param>
        protected void GeneratePoints(string[] yPaths, params IList<double>[] yValueLists)
        {
            IList<double>[] yLists = null;
            bool isComplexYProperty = false;
            YComplexPaths = new string[yPaths.Count()][];
            for (int i = 0; i < yPaths.Count(); i++)
            {
                if (string.IsNullOrEmpty(yPaths[i]))
                    return;
                YComplexPaths[i] = yPaths[i].Split('.');
                if (yPaths[i].Contains("."))
                    isComplexYProperty = true;
            }
            if (IsSortData)
            {
                if (SeriesYValues == null)
                    CreateYValueCollection(yPaths.Length);
                yLists = SeriesYValues;
                ActualSeriesYValues = yValueLists;
            }
            else
            {
                SeriesYValues = ActualSeriesYValues = yLists = yValueLists;
            }

            this.YPaths = yPaths;
            SeriesYCount = yPaths.Length;
            ActualData = new List<object>();
            if (ItemsSource != null && !string.IsNullOrEmpty(XBindingPath))
            {
#if WPF
                if (ItemsSource is DataTable)
                    GenerateDataTablePoints(yPaths, yLists);
                else
#endif
                if (XBindingPath.Contains(".") || isComplexYProperty)
                    GenerateComplexPropertyPoints(yPaths, yLists);
                else
                    GeneratePropertyPoints(yPaths, yLists);
            }
            if (ShowEmptyPoints)
            {
                ValidateDataPoints(SeriesYValues);
                isPointValidated = true;
            }
            else if (!((this is LineSeries) || (this is ColumnSeries) || (this is ScatterSeries) ||(this is BarSeries) || (this is StepLineSeries)
                || (this is BubbleSeries) 
                || (this is StackingBarSeries) || (this is StackingColumnSeries)
                || (this is HiLoSeries) || (this is HiLoOpenCloseSeries) || (this is CandleSeries) || (this is RangeColumnSeries)
                || GetType().ToString().Contains("Bitmap")))
                foreach (var yValues in SeriesYValues)
                {
                    if (yValues.Contains(double.NaN))
                        throw new InvalidOperationException(this.GetType() + " YValues contains empty points. Please set the ShowEmptyPoints property value as true. ");
                }
            
            if (IsSortData)
            {
                SortActualPoints();
            }
        }

        internal ChartValueType GetDataType(IEnumerable itemSource, string[] paths)
        {
            var enumerator = itemSource.GetEnumerator();
            object parentObj = null;
            IPropertyAccessor propertyAccessor = null;
            if (enumerator.MoveNext())
            {
                do
                {
                    parentObj = enumerator.Current;
                    for (int i = 0; i < paths.Length; i++)
                    {
                        var propertyInfo = parentObj.GetType().GetTypeInfo().GetDeclaredProperty(paths[i]);
                        if (propertyInfo != null)
                            propertyAccessor = FastReflectionCaches.PropertyAccessorCache.Get(propertyInfo);
                        if (propertyAccessor == null) return ChartValueType.Double;
                        parentObj = propertyAccessor.GetValue(parentObj);
                    }
                } while (parentObj == null && enumerator.MoveNext());
            }
            return GetDataType(parentObj);
        }

        private ChartValueType GetDataType(IPropertyAccessor propertyAccessor, IEnumerable itemsSource)
        {
            if (itemsSource == null) return ChartValueType.Double;
            var enumerator = itemsSource.GetEnumerator();
            object obj = null;
            if (enumerator.MoveNext())
            {
                do
                {
                    obj = propertyAccessor.GetValue(enumerator.Current);
                }
                while (enumerator.MoveNext() && obj == null);
            }
            return GetDataType(obj);
        }

        private void GeneratePropertyPoints(string[] yPaths, IList<double>[] yLists)
        {
#if WPF
            IEnumerator enumerator = (ItemsSource as IEnumerable).GetEnumerator();
#else
            IEnumerator enumerator = ItemsSource.GetEnumerator();
#endif
            if (enumerator.MoveNext())
            {
                for (int i = 0; i < updateStartedIndex; i++)
                {
                    enumerator.MoveNext();
                }

                var xPropertyInfo = enumerator.Current.GetType().GetTypeInfo().GetDeclaredProperty(this.XBindingPath);
                IPropertyAccessor xPropertyAccessor = null;
                if (xPropertyInfo != null)
                    xPropertyAccessor = FastReflectionCaches.PropertyAccessorCache.Get(xPropertyInfo);
                if (xPropertyAccessor == null) return;
                Func<object, object> xGetMethod = xPropertyAccessor.GetMethod;
                XAxisValueType = GetDataType(xPropertyAccessor, ItemsSource as IEnumerable);

                if (XAxisValueType == ChartValueType.DateTime || XAxisValueType == ChartValueType.Double ||
                    XAxisValueType == ChartValueType.Logarithmic || XAxisValueType == ChartValueType.TimeSpan)
                {
                    if (!(ActualXValues is List<double>))
                        this.ActualXValues = this.XValues = new List<double>();
                }
                else
                {
                    if (!(ActualXValues is List<string>))
                        this.ActualXValues = this.XValues = new List<string>();
                }

                if (IsMultipleYPathRequired)
                {
                    List<IPropertyAccessor> yPropertyAccessor = new List<IPropertyAccessor>();
                    if (string.IsNullOrEmpty(yPaths[0]))
                        return;
                    for (int i = 0; i < yPaths.Count(); i++)
                    {
                        var yPropertyInfo = enumerator.Current.GetType().GetTypeInfo().GetDeclaredProperty(yPaths[i]);
                        if (yPropertyInfo == null) return;
                        var yProperty = FastReflectionCaches.PropertyAccessorCache.Get(yPropertyInfo);
                        if (yProperty == null) return;
                        yPropertyAccessor.Add(yProperty);
                    }
                    if (XAxisValueType == ChartValueType.String)
                    {
                        IList<string> xValue = this.XValues as List<string>;
                        do
                        {
                            object xVal = xGetMethod(enumerator.Current);
                            xValue.Add((string)xVal);
                            for (int i = 0; i < yPropertyAccessor.Count; i++)
                            {
                                yLists[i].Add(Convert.ToDouble(yPropertyAccessor[i].GetValue(enumerator.Current)));
                            }
                            ActualData.Add(enumerator.Current);

                        } while (enumerator.MoveNext());
                        dataCount = xValue.Count;
                    }
                    else if (XAxisValueType == ChartValueType.Double ||
                       XAxisValueType == ChartValueType.Logarithmic)
                    {
                        IList<double> xValue = this.XValues as List<double>;
                        do
                        {
                            object xVal = xGetMethod(enumerator.Current);
                            xValue.Add(Convert.ToDouble(xVal));
                            for (int i = 0; i < yPropertyAccessor.Count; i++)
                            {
                                yLists[i].Add(Convert.ToDouble(yPropertyAccessor[i].GetValue(enumerator.Current)));
                            }
                            ActualData.Add(enumerator.Current);

                        } while (enumerator.MoveNext());
                        dataCount = xValue.Count;
                    }
                    else if (XAxisValueType == ChartValueType.DateTime)
                    {
                        IList<double> xValue = this.XValues as List<double>;
                        do
                        {
                            object xVal = xGetMethod(enumerator.Current);
                            xValue.Add(((DateTime)xVal).ToOADate());
                            for (int i = 0; i < yPropertyAccessor.Count; i++)
                            {
                                yLists[i].Add(Convert.ToDouble(yPropertyAccessor[i].GetValue(enumerator.Current)));
                            }
                            ActualData.Add(enumerator.Current);

                        } while (enumerator.MoveNext());
                        dataCount = xValue.Count;
                    }
                    else if (XAxisValueType == ChartValueType.TimeSpan)
                    {
                        IList<double> xValue = this.XValues as List<double>;
                        do
                        {
                            object xVal = xGetMethod(enumerator.Current);
                            xValue.Add(((TimeSpan)xVal).TotalMilliseconds);
                            for (int i = 0; i < yPropertyAccessor.Count; i++)
                            {
                                yLists[i].Add(Convert.ToDouble(yPropertyAccessor[i].GetValue(enumerator.Current)));
                            }
                            ActualData.Add(enumerator.Current);

                        } while (enumerator.MoveNext());
                        dataCount = xValue.Count;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(yPaths[0]))
                        return;
                    var yPropertyInfo = enumerator.Current.GetType().GetTypeInfo().GetDeclaredProperty(yPaths[0]);
                    IPropertyAccessor yPropertyAccessor = null;
                    if (yPropertyInfo != null)
                        yPropertyAccessor = FastReflectionCaches.PropertyAccessorCache.Get(yPropertyInfo);
                    if (yPropertyAccessor == null) return;
                    IList<double> yValue = yLists[0];
                    if (yPropertyAccessor == null) return;
                    Func<object, object> yGetMethod = yPropertyAccessor.GetMethod;
                    if (XAxisValueType == ChartValueType.String)
                    {
                        IList<string> xValue = this.XValues as List<string>;
                        do
                        {
                            object xVal = xGetMethod(enumerator.Current)!=null?xGetMethod(enumerator.Current):string.Empty;
                            object yVal = yGetMethod(enumerator.Current);
                            xValue.Add((string)xVal);
                            yValue.Add(Convert.ToDouble(yVal));
                            ActualData.Add(enumerator.Current);
                        } while (enumerator.MoveNext());
                        dataCount = xValue.Count;
                    }
                    else if (XAxisValueType == ChartValueType.Double ||
                        XAxisValueType == ChartValueType.Logarithmic)
                    {
                        IList<double> xValue = this.XValues as List<double>;
                        do
                        {
                            object xVal = xGetMethod(enumerator.Current);
                            object yVal = yGetMethod(enumerator.Current);
                            xValue.Add(Convert.ToDouble(xVal));
                            yValue.Add(Convert.ToDouble(yVal));
                            ActualData.Add(enumerator.Current);
                        } while (enumerator.MoveNext());
                        dataCount = xValue.Count;
                    }
                    else if (XAxisValueType == ChartValueType.DateTime)
                    {
                        IList<double> xValue = this.XValues as List<double>;

                        do
                        {
                            object xVal = xGetMethod(enumerator.Current);
                            object yVal = yGetMethod(enumerator.Current);
                            xValue.Add(((DateTime)xVal).ToOADate());
                            yValue.Add(Convert.ToDouble(yVal));
                            ActualData.Add(enumerator.Current);
                        } while (enumerator.MoveNext());
                        dataCount = xValue.Count;
                    }
                    else if (XAxisValueType == ChartValueType.TimeSpan)
                    {
                        IList<double> xValue = this.XValues as List<double>;

                        do
                        {
                            object xVal = xGetMethod(enumerator.Current);
                            object yVal = yGetMethod(enumerator.Current);
                            xValue.Add(((TimeSpan)xVal).TotalMilliseconds);
                            yValue.Add(Convert.ToDouble(yVal));
                            ActualData.Add(enumerator.Current);
                        } while (enumerator.MoveNext());
                        dataCount = xValue.Count;
                    }
                }
                HookPropertyChangedEvent(ListenPropertyChange);
            }
            IsPointGenerated = true;
        }

        private void GenerateComplexPropertyPoints(string[] yPaths, IList<double>[] yLists)
        {
            IEnumerator enumerator = (ItemsSource as IEnumerable).GetEnumerator();
            if (enumerator.MoveNext())
            {
                for (int i = 0; i < updateStartedIndex; i++)
                {
                    enumerator.MoveNext();
                }
                XAxisValueType = GetDataType(ItemsSource as IEnumerable, XComplexPaths);
                if (XAxisValueType == ChartValueType.DateTime || XAxisValueType == ChartValueType.Double ||
                    XAxisValueType == ChartValueType.Logarithmic || XAxisValueType == ChartValueType.TimeSpan)
                {
                    if (!(XValues is List<double>))
                        this.ActualXValues = this.XValues = new List<double>();
                }
                else
                {
                    if (!(XValues is List<string>))
                        this.ActualXValues = this.XValues = new List<string>();
                }

                if (IsMultipleYPathRequired)
                {
                    List<IPropertyAccessor> yPropertyAccessor = new List<IPropertyAccessor>();
                    if (string.IsNullOrEmpty(yPaths[0]))
                        return;
                    int count = yPaths.Count();
                    object xVal = null;
                    xVal = GetPropertyValue(enumerator.Current, XComplexPaths);
                    if (xVal == null) return;
                    for (int i = 0; i < yPaths.Count(); i++)
                    {
                        var yPropertyValue = GetPropertyValue(enumerator.Current, YComplexPaths[i]);
                        if (yPropertyValue == null) return;
                    }
                    if (XAxisValueType == ChartValueType.String)
                    {
                        IList<string> xValue = this.XValues as List<string>;
                        do
                        {
                            xVal = GetPropertyValue(enumerator.Current, XComplexPaths);
                            xValue.Add((string)xVal);
                            for (int i = 0; i < yPaths.Count(); i++)
                            {
                                yLists[i].Add(Convert.ToDouble(GetPropertyValue(enumerator.Current, YComplexPaths[i])));
                            }
                            ActualData.Add(enumerator.Current);
                        } while (enumerator.MoveNext());
                        dataCount = xValue.Count;
                    }
                    else if (XAxisValueType == ChartValueType.Double ||
                       XAxisValueType == ChartValueType.Logarithmic)
                    {
                        IList<double> xValue = this.XValues as List<double>;
                        do
                        {
                            xVal = GetPropertyValue(enumerator.Current, XComplexPaths);
                            xValue.Add(Convert.ToDouble(xVal));
                            for (int i = 0; i < yPaths.Count(); i++)
                            {
                                yLists[i].Add(Convert.ToDouble(GetPropertyValue(enumerator.Current, YComplexPaths[i])));
                            }
                            ActualData.Add(enumerator.Current);
                        } while (enumerator.MoveNext());
                        dataCount = xValue.Count;
                    }
                    else if (XAxisValueType == ChartValueType.DateTime)
                    {
                        IList<double> xValue = this.XValues as List<double>;
                        do
                        {
                            xVal = GetPropertyValue(enumerator.Current, XComplexPaths);
                            xValue.Add(((DateTime)xVal).ToOADate());
                            for (int i = 0; i < yPaths.Count(); i++)
                            {
                                yLists[i].Add(Convert.ToDouble(GetPropertyValue(enumerator.Current, YComplexPaths[i])));
                            }
                            ActualData.Add(enumerator.Current);
                        } while (enumerator.MoveNext());
                        dataCount = xValue.Count;
                    }
                    else if (XAxisValueType == ChartValueType.TimeSpan)
                    {
                        IList<double> xValue = this.XValues as List<double>;
                        do
                        {
                            xVal = GetPropertyValue(enumerator.Current, XComplexPaths);
                            xValue.Add(((TimeSpan)xVal).TotalMilliseconds);
                            for (int i = 0; i < yPaths.Count(); i++)
                            {
                                yLists[i].Add(Convert.ToDouble(GetPropertyValue(enumerator.Current, YComplexPaths[i])));
                            }
                            ActualData.Add(enumerator.Current);
                        } while (enumerator.MoveNext());
                        dataCount = xValue.Count;
                    }
                }
                else
                {
                    string[] tempYPath = YComplexPaths[0];
                    if (string.IsNullOrEmpty(yPaths[0]))
                        return;
                    IList<double> yValue = yLists[0];
                    object xVal = null, yVal = null;
                    if (XAxisValueType == ChartValueType.String)
                    {
                        IList<string> xValue = this.XValues as List<string>;
                        do
                        {
                            xVal = GetPropertyValue(enumerator.Current, XComplexPaths);
                            yVal = GetPropertyValue(enumerator.Current, tempYPath);
                            xValue.Add((string)xVal);
                            yValue.Add(Convert.ToDouble(yVal));
                            ActualData.Add(enumerator.Current);
                        } while (enumerator.MoveNext());
                        dataCount = xValue.Count;
                    }
                    else if (XAxisValueType == ChartValueType.Double ||
                        XAxisValueType == ChartValueType.Logarithmic)
                    {
                        IList<double> xValue = this.XValues as List<double>;
                        do
                        {
                            xVal = GetPropertyValue(enumerator.Current, XComplexPaths);
                            yVal = GetPropertyValue(enumerator.Current, tempYPath);
                            xValue.Add(Convert.ToDouble(xVal));
                            yValue.Add(Convert.ToDouble(yVal));
                            ActualData.Add(enumerator.Current);
                        } while (enumerator.MoveNext());
                        dataCount = xValue.Count;
                    }
                    else if (XAxisValueType == ChartValueType.DateTime)
                    {
                        IList<double> xValue = this.XValues as List<double>;
                        do
                        {
                            xVal = GetPropertyValue(enumerator.Current, XComplexPaths);
                            yVal = GetPropertyValue(enumerator.Current, tempYPath);
                            xValue.Add(((DateTime)xVal).ToOADate());
                            yValue.Add(Convert.ToDouble(yVal));
                            ActualData.Add(enumerator.Current);
                        } while (enumerator.MoveNext());
                        dataCount = xValue.Count;
                    }
                    else if (XAxisValueType == ChartValueType.TimeSpan)
                    {
                        IList<double> xValue = this.XValues as List<double>;
                        do
                        {
                            xVal = GetPropertyValue(enumerator.Current, XComplexPaths);
                            yVal = GetPropertyValue(enumerator.Current, tempYPath);
                            xValue.Add(((TimeSpan)xVal).TotalMilliseconds);
                            yValue.Add(Convert.ToDouble(yVal));
                            ActualData.Add(enumerator.Current);
                        } while (enumerator.MoveNext());
                        dataCount = xValue.Count;
                    }
                }
                HookPropertyChangedEvent(ListenPropertyChange);
            }
            IsPointGenerated = true;
        }
#if WPF
        private void GenerateDataTablePoints(string[] yPaths, IList<double>[] yLists)
        {
            IEnumerator enumerator = (ItemsSource as DataTable).Rows.GetEnumerator();
            if (enumerator.MoveNext())
            {
                object xvalueType = null;
                for (int i = 0; i < updateStartedIndex; i++)
                {
                    enumerator.MoveNext();
                }

                xvalueType = (enumerator.Current as DataRow).Field<object>(this.XBindingPath);
                XAxisValueType = GetDataType(xvalueType);
                if (XAxisValueType == ChartValueType.DateTime || XAxisValueType == ChartValueType.Double ||
                    XAxisValueType == ChartValueType.Logarithmic || XAxisValueType == ChartValueType.TimeSpan)
                {
                    if (!(XValues is List<double>))
                        this.ActualXValues = this.XValues = new List<double>();
                }
                else
                {
                    if (!(XValues is List<string>))
                        this.ActualXValues = this.XValues = new List<string>();
                }

                if (IsMultipleYPathRequired)
                {
                    List<IPropertyAccessor> yPropertyAccessor = new List<IPropertyAccessor>();
                    if (string.IsNullOrEmpty(yPaths[0]))
                        return;
                    int count = yPaths.Count();
                    if (XAxisValueType == ChartValueType.String)
                    {
                        IList<string> xValue = this.XValues as List<string>;
                        do
                        {
                            object xVal = null;
                            xVal = (enumerator.Current as DataRow).Field<object>(this.XBindingPath);
                            xValue.Add((string)xVal);
                            for (int i = 0; i < yPaths.Count(); i++)
                            {
                                yLists[i].Add(Convert.ToDouble((enumerator.Current as DataRow).Field<object>(yPaths[i])));
                            }
                            ActualData.Add(enumerator.Current);
                        } while (enumerator.MoveNext());
                        dataCount = xValue.Count;
                    }
                    else if (XAxisValueType == ChartValueType.Double ||
                       XAxisValueType == ChartValueType.Logarithmic)
                    {
                        IList<double> xValue = this.XValues as List<double>;
                        do
                        {
                            object xVal = null;
                            xVal = (enumerator.Current as DataRow).Field<object>(this.XBindingPath);
                            xValue.Add(Convert.ToDouble(xVal));
                            for (int i = 0; i < yPaths.Count(); i++)
                            {
                                yLists[i].Add(Convert.ToDouble((enumerator.Current as DataRow).Field<object>(yPaths[i])));
                            }
                            ActualData.Add(enumerator.Current);
                        } while (enumerator.MoveNext());
                        dataCount = xValue.Count;
                    }
                    else if (XAxisValueType == ChartValueType.DateTime)
                    {
                        IList<double> xValue = this.XValues as List<double>;
                        do
                        {
                            object xVal = null;
                            xVal = (enumerator.Current as DataRow).Field<object>(this.XBindingPath);
                            xValue.Add(((DateTime)xVal).ToOADate());
                            for (int i = 0; i < yPaths.Count(); i++)
                            {
                                yLists[i].Add(Convert.ToDouble((enumerator.Current as DataRow).Field<object>(yPaths[i])));
                            }
                            ActualData.Add(enumerator.Current);
                        } while (enumerator.MoveNext());
                        dataCount = xValue.Count;
                    }
                    else if (XAxisValueType == ChartValueType.TimeSpan)
                    {
                        IList<double> xValue = this.XValues as List<double>;
                        do
                        {
                            object xVal = null;
                            xVal = (enumerator.Current as DataRow).Field<object>(this.XBindingPath);
                            xValue.Add(((TimeSpan)xVal).TotalMilliseconds);
                            for (int i = 0; i < yPaths.Count(); i++)
                            {
                                yLists[i].Add(Convert.ToDouble((enumerator.Current as DataRow).Field<object>(yPaths[i])));
                            }
                            ActualData.Add(enumerator.Current);
                        } while (enumerator.MoveNext());
                        dataCount = xValue.Count;
                    }
                }
                else
                {
                    string[] tempYPath = YComplexPaths[0].ToArray();
                    if (string.IsNullOrEmpty(yPaths[0]))
                        return;
                    IList<double> yValue = yLists[0];
                    object xVal = null, yVal = null;
                    if (XAxisValueType == ChartValueType.String)
                    {
                        IList<string> xValue = this.XValues as List<string>;
                        do
                        {
                            xVal = (enumerator.Current as DataRow).Field<object>(this.XBindingPath);
                            yVal = (enumerator.Current as DataRow).Field<object>(yPaths[0]);
                            xValue.Add((string)xVal);
                            yValue.Add(Convert.ToDouble(yVal));
                            ActualData.Add(enumerator.Current);
                        } while (enumerator.MoveNext());
                        dataCount = xValue.Count;
                    }
                    else if (XAxisValueType == ChartValueType.Double ||
                        XAxisValueType == ChartValueType.Logarithmic)
                    {
                        IList<double> xValue = this.XValues as List<double>;
                        do
                        {
                            xVal = (enumerator.Current as DataRow).Field<object>(this.XBindingPath);
                            yVal = (enumerator.Current as DataRow).Field<object>(yPaths[0]);
                            xValue.Add(Convert.ToDouble(xVal));
                            yValue.Add(Convert.ToDouble(yVal));
                            ActualData.Add(enumerator.Current);
                        } while (enumerator.MoveNext());
                        dataCount = xValue.Count;
                    }
                    else if (XAxisValueType == ChartValueType.DateTime)
                    {
                        IList<double> xValue = this.XValues as List<double>;
                        do
                        {
                            xVal = (enumerator.Current as DataRow).Field<object>(this.XBindingPath);
                            yVal = (enumerator.Current as DataRow).Field<object>(yPaths[0]);
                            xValue.Add(((DateTime)xVal).ToOADate());
                            yValue.Add(Convert.ToDouble(yVal));
                            ActualData.Add(enumerator.Current);
                        } while (enumerator.MoveNext());
                        dataCount = xValue.Count;
                    }
                    else if (XAxisValueType == ChartValueType.TimeSpan)
                    {
                        IList<double> xValue = this.XValues as List<double>;
                        do
                        {
                            xVal = (enumerator.Current as DataRow).Field<object>(this.XBindingPath);
                            yVal = (enumerator.Current as DataRow).Field<object>(yPaths[0]);
                            xValue.Add(((TimeSpan)xVal).TotalMilliseconds);
                            yValue.Add(Convert.ToDouble(yVal));
                            ActualData.Add(enumerator.Current);
                        } while (enumerator.MoveNext());
                        dataCount = xValue.Count;
                    }
                }
                HookPropertyChangedEvent(ListenPropertyChange);
            }
            IsPointGenerated = true;
        }
#endif
        internal object GetPropertyValue(object obj, string[] paths)
        {
            object parentObj= obj;
            IPropertyAccessor propertyAccessor = null;
            for (int i = 0; i < paths.Length; i++)
            {
                var propertyInfo = parentObj.GetType().GetTypeInfo().GetDeclaredProperty(paths[i]);
                if (propertyInfo != null)
                    propertyAccessor = FastReflectionCaches.PropertyAccessorCache.Get(propertyInfo);
                if (propertyAccessor == null) return null;
                parentObj = propertyAccessor.GetValue(parentObj);
            }
            return parentObj;
        }

        internal void SetPropertyValue(object obj, string[] paths, object data)
        {
                object parentObj=obj;
                IPropertyAccessor propertyAccessor = null;
                for (int i = 0; i < paths.Length; i++)
                {
                    var propertyInfo = parentObj.GetType().GetTypeInfo().GetDeclaredProperty(paths[i]);
                     if (propertyInfo != null)
                        propertyAccessor = FastReflectionCaches.PropertyAccessorCache.Get(propertyInfo);
                     if (propertyAccessor == null) return;
                    if (i == paths.Length - 1)
                    {
                        propertyAccessor.SetValue(parentObj,data);
                        break;
                    }
                   
                    parentObj = propertyAccessor.GetValue(parentObj);
                }
            
        }

       
        private void HookPropertyChangedEvent(bool value)
        {
            if (ItemsSource == null) return;
#if WPF
            IEnumerator enumerator = (ItemsSource is DataTable) ? (ItemsSource as DataTable).Rows.GetEnumerator() : (ItemsSource as IEnumerable).GetEnumerator();
#else
            IEnumerator enumerator = ItemsSource.GetEnumerator();
#endif
            if (!enumerator.MoveNext()) return;
            INotifyPropertyChanged collection = enumerator.Current as INotifyPropertyChanged;
            if (collection != null)
            {
                if (value)
                {
                    do
                    {
                        (enumerator.Current as INotifyPropertyChanged).PropertyChanged -= OnItemPropertyChanged;
                        (enumerator.Current as INotifyPropertyChanged).PropertyChanged += OnItemPropertyChanged;
                    } while (enumerator.MoveNext());
                }
                else
                {
                    do
                        (enumerator.Current as INotifyPropertyChanged).PropertyChanged -= OnItemPropertyChanged;
                    while (enumerator.MoveNext());
                }
            }
        }

        private void SortActualPoints()
        {
            if (XValues is IList<double>)
            {
                ActualXValues = new List<double>();

                List<double> actualXValues = ActualXValues as List<double>;
                List<double> xValues = XValues as List<double>;

                for (int i = 0; i < DataCount; i++)
                {
                    actualXValues.Add(xValues[i]);
                }
            }
            else
            {
                ActualXValues = new List<string>();

                List<string> actualXValues = ActualXValues as List<string>;
                List<string> xValues = XValues as List<string>;

                for (int i = 0; i < DataCount; i++)
                {
                    actualXValues.Add(xValues[i]);
                }
            }



            for (int i = 0; i < YPaths.Length; i++)
            {
                ActualSeriesYValues[i].Clear();
                for (int j = 0; j < SeriesYValues[i].Count; j++)
                {
                    ActualSeriesYValues[i].Add(SeriesYValues[i][j]);
                }
            }

            SortChartPoints();
        }

        void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (XBindingPath == e.PropertyName
                || YPaths != null && YPaths.Contains(e.PropertyName))
            {
                int position = -1;
                IEnumerable itemsSource = (ItemsSource as IEnumerable);
                foreach (object obj in itemsSource)
                {
                    position++;

                    if (obj == sender)
                        break;
                }

                if (position != -1)
                {
                    SetIndividualPoint(position, sender, true);

                    if (IsSortData)
                    {
                        SortActualPoints();
                    }

                    this.UpdateArea();
                }
            }
        }
        /// <summary>
        /// Return collection of double values
        /// </summary>
        /// <returns></returns>
        protected internal List<double> GetXValues()
        {
            double xIndexValues = 0d;
            List<double> xValues = ActualXValues as List<double>;
            if (IsIndexed || xValues == null)
            {
                xValues = xValues != null ? (from val in (xValues) select (xIndexValues++)).ToList()
                      : (from val in (ActualXValues as List<string>) select (xIndexValues++)).ToList();
            }
            return xValues;
        }

        /// <summary>
        /// Sorts the Chart Points 
        /// </summary>
        /// <returns></returns>
        private void SortChartPoints()
        {
            if (xValueType != ChartValueType.String)
            {
                var xValues = ActualXValues as List<double>;
                GetTuple(xValues);
            }
            else
            {
                var xValues = ActualXValues as List<string>;
                GetTuple(xValues);
            }
        }

        private void GetTuple<T>(List<T> xValues) where T : IComparable<T>
        {
            switch (this.SeriesYCount)
            {
                case 1:
                    List<Tuple<T, double>> pair = new List<Tuple<T, double>>();
                    var y0Values = ActualSeriesYValues[0] as List<double>;
                    for (int j = 0; j < DataCount; j++)
                    {
                        pair.Add(Tuple.Create<T, double>(xValues[j], y0Values[j]));
                    }
                    if (pair.Count > 0)
                    {
                        this.Sort<T, double>(pair);
                    }
                    break;
                case 2:
                    List<Tuple<T, double, double>> triple = new List<Tuple<T, double, double>>();
                    var y00Values = ActualSeriesYValues[0] as List<double>;
                    var y1Values = ActualSeriesYValues[1] as List<double>;
                    for (int j = 0; j < DataCount; j++)
                    {
                        triple.Add(Tuple.Create<T, double, double>(xValues[j], y00Values[j], y1Values[j]));
                    }
                    if (triple.Count > 0)
                    {
                        this.Sort<T, double, double>(triple);
                    }
                    break;
                case 4:
                    List<Tuple<T, double, double, double, double>> quintuple = new List<Tuple<T, double, double, double, double>>();
                    var y01Values = ActualSeriesYValues[0] as List<double>;
                    var y11Values = ActualSeriesYValues[1] as List<double>;
                    var y2Values = ActualSeriesYValues[2] as List<double>;
                    var y3Values = ActualSeriesYValues[3] as List<double>;
                    for (int j = 0; j < DataCount; j++)
                    {
                        quintuple.Add(Tuple.Create<T, double, double, double, double>(xValues[j], y01Values[j], y11Values[j], y2Values[j], y3Values[j]));
                    }
                    if (quintuple.Count > 0)
                    {
                        this.Sort<T, double, double, double, double>(quintuple);
                    }
                    break;
                case 5:
                    List<Tuple<T, double, double, double, double, double>> hextuple = new List<Tuple<T, double, double, double, double, double>>();
                    var y02Values = ActualSeriesYValues[0] as List<double>;
                    var y12Values = ActualSeriesYValues[1] as List<double>;
                    var y20Values = ActualSeriesYValues[2] as List<double>;
                    var y30Values = ActualSeriesYValues[3] as List<double>;
                    var y40Values = ActualSeriesYValues[4] as List<double>;
                    for (int j = 0; j < DataCount; j++)
                    {
                        hextuple.Add(Tuple.Create<T, double, double, double, double, double>(xValues[j], y02Values[j], y12Values[j], y20Values[j], y30Values[j], y40Values[j]));
                    }
                    if (hextuple.Count > 0)
                    {
                        this.Sort<T, double, double, double, double, double>(hextuple);
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Sort the ActualXValues and ActualYValues 
        /// </summary>
        /// <typeparam name="T">XValues</typeparam>
        /// <typeparam name="T">YValues</typeparam>
        /// <typeparam name="?">Index</typeparam>
        /// <param name="list"></param>
        private void Sort<T, T1>(List<Tuple<T, T1>> list)
            where T : IComparable<T>
            where T1 : IComparable<T1>
        {
            switch (this.SortBy)
            {
                case SortingAxis.X:
                    {
                        list.Sort((t1, t2) => t1.Item1.CompareTo(t2.Item1));
                        ActualSort<T, T1>(list);
                    }
                    break;
                case SortingAxis.Y:
                    {
                        list.Sort((t1, t2) => t1.Item2.CompareTo(t2.Item2));
                        ActualSort<T, T1>(list);
                    }
                    break;
                default:
                    break;
            }
        }

        private void ActualSort<T, T1>(List<Tuple<T, T1>> list)
        {
            ActualXValues = (from x in list select x.Item1).ToList<T>();
            if (SortDirection == Direction.Descending)
                (ActualXValues as List<T>).Reverse();

            if (ActualSeriesYValues != null)
            {
                var y1 = ActualSeriesYValues[0] as List<T1>;
                int k = 0;
                foreach (var item in list)
                {
                    y1[k] = item.Item2;
                    k++;
                }
                if (SortDirection == Direction.Descending)
                {
                    y1.Reverse();
                }
            }
        }

        /// <summary>
        /// Sort the ActualXValues and ActualYValues 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="list"></param>
        private void Sort<T, T1, T2>(List<Tuple<T, T1, T2>> list)
            where T : IComparable<T>
            where T1 : IComparable<T1>
            where T2 : IComparable<T2>
        {
            switch (this.SortBy)
            {
                case SortingAxis.X:
                    {
                        list.Sort((t1, t2) => t1.Item1.CompareTo(t2.Item1));
                        ActualSort<T, T1, T2>(list);
                    }
                    break;
                case SortingAxis.Y:
                    {
                        list.Sort((t1, t2) => t1.Item2.CompareTo(t2.Item2));
                        ActualSort<T, T1, T2>(list);
                    }
                    break;
                default:
                    break;
            }
        }

        private void ActualSort<T, T1, T2>(List<Tuple<T, T1, T2>> list)
        {
            ActualXValues = (from x in list select x.Item1).ToList<T>();
            if (SortDirection == Direction.Descending)
                (ActualXValues as List<T>).Reverse();

            if (ActualSeriesYValues != null)
            {
                var y1 = ActualSeriesYValues[0] as List<T1>;
                var y2 = ActualSeriesYValues[1] as List<T2>;
                int k = 0;
                foreach (var item in list)
                {
                    y1[k] = item.Item2;
                    y2[k] = item.Item3;
                    k++;
                }
                if (SortDirection == Direction.Descending)
                {
                    y1.Reverse();
                    y2.Reverse();
                }
            }
        }

        /// <summary>
        /// Sort the ActualXValues and ActualYValues 
        /// </summary>
        /// <typeparam name="T">XValues</typeparam>
        /// <typeparam name="T">YValues</typeparam>
        /// <typeparam name="?">Index</typeparam>
        /// <param name="list"></param>
        private void Sort<T, T1, T2, T3, T4>(List<Tuple<T, T1, T2, T3, T4>> list)
            where T : IComparable<T>
            where T1 : IComparable<T1>
            where T2 : IComparable<T2>
            where T3 : IComparable<T3>
            where T4 : IComparable<T4>
        {
            switch (this.SortBy)
            {
                case SortingAxis.X:
                    {
                        list.Sort((t1, t2) => t1.Item1.CompareTo(t2.Item1));
                        ActualSort<T, T1, T2, T3, T4>(list);
                    }
                    break;
                case SortingAxis.Y:
                    {
                        list.Sort((t1, t2) => t1.Item2.CompareTo(t2.Item2));
                        ActualSort<T, T1, T2, T3, T4>(list);
                    }
                    break;
                default:
                    break;
            }
        }

        private void ActualSort<T, T1, T2, T3, T4>(List<Tuple<T, T1, T2, T3, T4>> list)
        {
            ActualXValues = (from x in list select x.Item1).ToList<T>();
            if (SortDirection == Direction.Descending)
                (ActualXValues as List<T>).Reverse();

            if (ActualSeriesYValues != null)
            {
                var y1 = ActualSeriesYValues[0] as List<T1>;
                var y2 = ActualSeriesYValues[1] as List<T2>;
                int k = 0;
                foreach (var item in list)
                {
                    (ActualSeriesYValues[0] as List<T1>)[k] = item.Item2;
                    (ActualSeriesYValues[1] as List<T2>)[k] = item.Item3;
                    (ActualSeriesYValues[2] as List<T3>)[k] = item.Item4;
                    (ActualSeriesYValues[3] as List<T4>)[k] = item.Item5;
                    k++;
                }
                if (SortDirection == Direction.Descending)
                {
                    y1.Reverse();
                    y2.Reverse();
                }
            }
        }

        private void Sort<T, T1, T2, T3, T4, T5>(List<Tuple<T, T1, T2, T3, T4, T5>> list)
            where T : IComparable<T>
            where T1 : IComparable<T1>
            where T2 : IComparable<T2>
        {
            switch (this.SortBy)
            {
                case SortingAxis.X:
                    {
                        list.Sort((t1, t2) => t1.Item1.CompareTo(t2.Item1));
                        ActualSort<T, T1, T2, T3, T4, T5>(list);
                    }
                    break;
                case SortingAxis.Y:
                    {
                        list.Sort((t1, t2) => t1.Item2.CompareTo(t2.Item2));
                        ActualSort<T, T1, T2, T3, T4, T5>(list);
                    }
                    break;
                default:
                    break;
            }
        }

        private void ActualSort<T, T1, T2, T3, T4, T5>(List<Tuple<T, T1, T2, T3, T4, T5>> list)
        {
            ActualXValues = (from x in list select x.Item1).ToList<T>();
            if (SortDirection == Direction.Descending)
                (ActualXValues as List<T>).Reverse();

            if (ActualSeriesYValues != null)
            {
                var y1 = ActualSeriesYValues[0] as List<T1>;
                var y2 = ActualSeriesYValues[1] as List<T2>;
                var y3 = ActualSeriesYValues[1] as List<T3>;
                var y4 = ActualSeriesYValues[1] as List<T4>;
                var y5 = ActualSeriesYValues[1] as List<T5>;
                int k = 0;
                foreach (var item in list)
                {
                    y1[k] = item.Item2;
                    y2[k] = item.Item3;
                    y3[k] = item.Item4;
                    y4[k] = item.Item5;
                    y5[k] = item.Item6;
                    k++;
                }
                if (SortDirection == Direction.Descending)
                {
                    y1.Reverse();
                    y2.Reverse();
                    y3.Reverse();
                    y4.Reverse();
                    y5.Reverse();
                }
            }
        }

        private ChartValueType GetDataType(object xval)
        {
            if (xval is string)
                return ChartValueType.String;
            else if (xval is DateTime)
                return ChartValueType.DateTime;
            else if (xval is TimeSpan)
                return ChartValueType.TimeSpan;
            else
                return ChartValueType.Double;
        }
        /// <summary>
        /// Return IChartTranform value based upon the given size
        /// </summary>
        /// <param name="size"></param>
        /// <param name="create"></param>
        /// <returns></returns>
        protected internal virtual IChartTransformer CreateTransformer(Size size, bool create)
        {
            if (create || ChartTransformer == null)
            {
                ChartTransformer = ChartTransform.CreateCartesian(size, this);
            }

            return ChartTransformer;
        }

        private static void OnBindingPathXChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            (obj as ChartSeriesBase).XComplexPaths = args.NewValue.ToString().Split('.');
            (obj as ChartSeriesBase).OnBindingPathChanged(args);
        }

        private static void OnAppearanceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            (obj as ChartSeriesBase).OnAppearanceChanged(obj as ChartSeriesBase);
        }

        private void OnAppearanceChanged(ChartSeriesBase obj)
        {
            if (obj.GetType().ToString().Contains("Bitmap") || this is ChartSeries3D)
                obj.UpdateArea();
        }

        protected virtual void OnBindingPathChanged(DependencyPropertyChangedEventArgs args)
        {
            this.IsPointGenerated = false;
            canAnimate = true;
            if (ActualXValues != null)
            {
                if (ActualXValues is IList<double>)
                {
                    (XValues as IList<double>).Clear();
                    (ActualXValues as IList<double>).Clear();
                }
                else if (ActualXValues is IList<string>)
                {
                    (XValues as IList<string>).Clear();
                    (ActualXValues as IList<string>).Clear();
                }
            }
            totalCalculated = false;
            Segments.Clear();
            this.dataCount = 0;
            this.UpdateArea();
        }

        private static void OnDataSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ChartSeriesBase series = obj as ChartSeriesBase;

            series.OnDataSourceChanged(args);
        }
        /// <summary>
        /// Called when DataSource property changed
        /// </summary>
        /// <param name="args"></param>
        protected void OnDataSourceChanged(DependencyPropertyChangedEventArgs args)
        {
            canAnimate = true;
            if (ActualXValues != null)
            {
                if (ActualXValues is IList<double>)
                {
                    (XValues as IList<double>).Clear();
                    (ActualXValues as IList<double>).Clear();
                }
                else if (ActualXValues is IList<string>)
                {
                    (XValues as IList<string>).Clear();
                    (ActualXValues as IList<string>).Clear();
                }
            }
            if (args.OldValue is INotifyCollectionChanged)
            {
                (args.OldValue as INotifyCollectionChanged).CollectionChanged -= OnDataCollectionChanged;
            }

            if (args.NewValue is INotifyCollectionChanged)
            {
                (args.NewValue as INotifyCollectionChanged).CollectionChanged += OnDataCollectionChanged;
            }
#if WPF
            if (args.OldValue is IRaiseItemChangedEvents)
            {
                (args.NewValue as IBindingList).ListChanged -= delegate(object sender, ListChangedEventArgs listArgs)
                {
                    OnBindingListChanged(listArgs.ListChangedType, listArgs.NewIndex);
                };
            }

            if (args.NewValue is IRaiseItemChangedEvents)
            {
                (args.NewValue as IBindingList).ListChanged += delegate(object sender, ListChangedEventArgs listArgs)
                {
                    OnBindingListChanged(listArgs.ListChangedType, listArgs.NewIndex);
                };
            }
#endif
            totalCalculated = false;
            Segments.Clear();
            this.dataCount = 0;
#if WPF
            var newTable = args.NewValue as DataTable;
            if (newTable != null)
            {
                newTable.RowChanged += DataTableRowChanged;
                newTable.RowDeleting += DataTableRowChanged;
                var oldTable = args.OldValue as DataTable;
                if (oldTable != null)
                {
                    oldTable.RowChanged -= DataTableRowChanged;
                    newTable.RowDeleting -= DataTableRowChanged;
                }
                OnDataSourceChanged(oldTable == null ? args.OldValue as IEnumerable : oldTable.Rows, newTable.Rows);
            }
            else
#endif
                OnDataSourceChanged(args.OldValue as IEnumerable, args.NewValue as IEnumerable);
            if (ActualXAxis != null)
                ActualXAxis.IsDataChanged = true;
            if (ActualYAxis != null)
                ActualYAxis.IsDataChanged = true;
        }
#if WPF
        private void OnBindingListChanged(ListChangedType listChangedType, int index)
        {
            NotifyCollectionChangedAction action = NotifyCollectionChangedAction.Reset;
            switch (listChangedType)
            {
                case ListChangedType.ItemChanged:
                    if (index > -1 && index < this.DataCount && (ItemsSource as IBindingList)[index] != null)
                    {
                        SetIndividualPoint(index, (ItemsSource as IBindingList)[index], true);

                        if (IsSortData)
                        {
                            SortActualPoints();
                        }
                        this.UpdateSegments(index, NotifyCollectionChangedAction.Replace);
                        action = NotifyCollectionChangedAction.Replace;
                    }
                    break;
                case ListChangedType.ItemDeleted:
                    if (this.ItemsSource != null)
                    {
                        if (XValues is IList<double>)
                        {
                            (XValues as IList<double>).RemoveAt(index);
                            this.dataCount--;
                        }
                        else if (XValues is IList<string>)
                        {
                            (XValues as IList<string>).RemoveAt(index);
                            this.dataCount--;
                        }
                        for (int i = 0; i < SeriesYValues.Count(); i++)
                        {
                            SeriesYValues[i].RemoveAt(index);
                        }

                        if (IsSortData)
                        {
                            SortActualPoints();
                        }
                        this.ActualData.RemoveAt(index);
                        this.UpdateSegments(index, NotifyCollectionChangedAction.Remove);
                        action = NotifyCollectionChangedAction.Remove;
                    }
                    break;
                case ListChangedType.ItemAdded:
                    {
                        if (this.ItemsSource != null)
                        {
                            if (!this.isNotificationSuspended)
                            {
                                this.SetIndividualPoint(index, (ItemsSource as IBindingList)[index], false);
                                if (IsSortData)
                                {
                                    SortActualPoints();
                                }

                                this.UpdateSegments(index, NotifyCollectionChangedAction.Add);
                            }
                            else if (!isUpdateStarted)
                            {
                                updateStartedIndex = index;
                                isUpdateStarted = true;
                            }
                            action = NotifyCollectionChangedAction.Add;
                        }
                        break;
                    }
                    default:
                    {
                        Refresh();
                        break;
                    }
            }
            if (ShowEmptyPoints)
                RevalidateEmptyPointsCollection(action, index, index);
            if (this is AccumulationSeriesBase)
                ActualArea.IsUpdateLegend = true;
            totalCalculated = false;
            if (ActualXAxis != null)
                ActualXAxis.IsDataChanged = true;
            if (ActualYAxis != null)
                ActualYAxis.IsDataChanged = true;
        }


        void DataTableRowChanged(object sender, DataRowChangeEventArgs e)
        {
            var index = (ItemsSource as DataTable).Rows.IndexOf(e.Row);
            switch (e.Action)
            {
                case DataRowAction.Add:
                    SetIndividualDataTablePoint(index, e.Row, false);
                    break;
                case DataRowAction.Change:
                    SetIndividualDataTablePoint(index, e.Row, true);
                    break;
                case DataRowAction.Delete:
                    if (this.ItemsSource != null)
                    {
                        if (XValues is IList<double>)
                        {
                            (XValues as IList<double>).RemoveAt(index);
                            this.dataCount--;
                        }
                        else if (XValues is IList<string>)
                        {
                            (XValues as IList<string>).RemoveAt(index);
                            this.dataCount--;
                        }
                        for (int i = 0; i < SeriesYValues.Count(); i++)
                        {
                            SeriesYValues[i].RemoveAt(index);
                        }

                        if (IsSortData)
                        {
                            SortActualPoints();
                        }
                        this.ActualData.RemoveAt(index);
                    }
                    break;
            }
            if (this is AccumulationSeriesBase)
                ActualArea.IsUpdateLegend = true;
            totalCalculated = false;
            if (ActualXAxis != null)
                ActualXAxis.IsDataChanged = true;
            if (ActualYAxis != null)
                ActualYAxis.IsDataChanged = true;
            UpdateArea();
        }

        /// <summary>
        /// Method implementation for Set points to given index for data table
        /// </summary>
        /// <param name="index"></param>
        /// <param name="obj"></param>
        /// <param name="replace"></param>
        protected void SetIndividualDataTablePoint(int index, object obj, bool replace)
        {
            if (SeriesYValues != null && YPaths != null && ItemsSource != null)
            {
                if (IsMultipleYPathRequired)
                {
                    if (XAxisValueType == ChartValueType.String)
                    {
                        if (!(this.XValues is List<string>))
                            this.XValues = this.ActualXValues = new List<string>();
                        IList<string> xValue = this.XValues as List<string>;
                        object xVal = (obj as DataRow).Field<object>(XBindingPath);
                        if (replace && xValue.Count > index)
                        {
                            xValue[index] = (string)xVal;
                        }
                        else
                        {
                            xValue.Insert(index, (string)xVal);
                        }

                        for (int i = 0; i < YPaths.Count(); i++)
                        {
                            if (replace && SeriesYValues[i].Count > index)
                            {
                                SeriesYValues[i][index] = Convert.ToDouble((obj as DataRow).Field<object>(YComplexPaths[i][0]));
                            }
                            else
                            {
                                SeriesYValues[i].Insert(index, Convert.ToDouble((obj as DataRow).Field<object>(YComplexPaths[i][0])));
                            }
                        }
                        ActualData.Add(obj);
                        dataCount = xValue.Count;
                    }
                    else if (XAxisValueType == ChartValueType.Double ||
                       XAxisValueType == ChartValueType.Logarithmic)
                    {
                        if (!(this.XValues is List<double>))
                            this.XValues = this.ActualXValues = new List<double>();
                        IList<double> xValue = this.XValues as List<double>;
                        object xVal = (obj as DataRow).Field<object>(XBindingPath);
                        if (replace && xValue.Count > index)
                        {
                            xValue[index] = Convert.ToDouble(xVal);
                        }
                        else
                        {
                            xValue.Insert(index, Convert.ToDouble(xVal));
                        }

                        for (int i = 0; i < YPaths.Count(); i++)
                        {
                            if (replace && SeriesYValues[i].Count > index)
                            {
                                SeriesYValues[i][index] = Convert.ToDouble((obj as DataRow).Field<object>(YComplexPaths[i][0]));
                            }
                            else
                            {
                                SeriesYValues[i].Insert(index, Convert.ToDouble((obj as DataRow).Field<object>(YComplexPaths[i][0])));
                            }
                        }
                        ActualData.Add(obj);
                        dataCount = xValue.Count;
                    }
                    else if (XAxisValueType == ChartValueType.DateTime)
                    {
                        if (!(this.XValues is List<double>))
                            this.XValues = this.ActualXValues = new List<double>();
                        IList<double> xValue = this.XValues as List<double>;
                        object xVal = (obj as DataRow).Field<object>(XBindingPath);
                        if (replace && xValue.Count > index)
                        {
                            xValue[index] = Convert.ToDateTime(xVal).ToOADate();
                        }
                        else
                        {
                            xValue.Insert(index, Convert.ToDateTime(xVal).ToOADate());
                        }
                        for (int i = 0; i < YPaths.Count(); i++)
                        {
                            if (replace && SeriesYValues[i].Count > index)
                            {
                                SeriesYValues[i][index] = Convert.ToDouble((obj as DataRow).Field<object>(YComplexPaths[i][0]));
                            }
                            else
                            {
                                SeriesYValues[i].Insert(index, Convert.ToDouble((obj as DataRow).Field<object>(YComplexPaths[i][0])));
                            }
                        }
                        ActualData.Add(obj);
                        dataCount = xValue.Count;
                    }
                    else if (XAxisValueType == ChartValueType.TimeSpan)
                    {
                        if (!(this.XValues is List<double>))
                            this.XValues = this.ActualXValues = new List<double>();
                        IList<double> xValue = this.XValues as List<double>;
                        object xVal = (obj as DataRow).Field<object>(XBindingPath);
                        if (replace && xValue.Count > index)
                        {
                            xValue[index] = ((TimeSpan)xVal).TotalMilliseconds;
                        }
                        else
                        {
                            xValue.Insert(index, ((TimeSpan)xVal).TotalMilliseconds);
                        }
                        for (int i = 0; i < YPaths.Count(); i++)
                        {
                            if (replace && SeriesYValues[i].Count > index)
                            {
                                SeriesYValues[i][index] = Convert.ToDouble((obj as DataRow).Field<object>(YComplexPaths[i][0]));
                            }
                            else
                            {
                                SeriesYValues[i].Insert(index, Convert.ToDouble((obj as DataRow).Field<object>(YComplexPaths[i][0])));
                            }
                        }
                        ActualData.Add(obj);
                        dataCount = xValue.Count;
                    }
                }
                else
                {
                    string[] tempYPath = YComplexPaths[0];
                    IList<double> yValue = SeriesYValues[0];
                    if (XAxisValueType == ChartValueType.String)
                    {
                        if (!(this.XValues is List<string>))
                            this.XValues = this.ActualXValues = new List<string>();
                        IList<string> xValue = this.XValues as List<string>;
                        object xVal = (obj as DataRow).Field<object>(XBindingPath);
                        object yVal = (obj as DataRow).Field<object>(tempYPath[0]);
                        if (replace && xValue.Count > index)
                        {

                            xValue[index] = Convert.ToString(xVal);
                        }
                        else
                        {
                            xValue.Insert(index, Convert.ToString(xVal));
                        }
                        if (replace && yValue.Count > index)
                        {
                            yValue[index] = Convert.ToDouble(yVal);
                        }
                        else
                        {
                            yValue.Insert(index, Convert.ToDouble(yVal));
                        }
                        ActualData.Add(obj);
                        dataCount = xValue.Count;
                    }
                    else if (XAxisValueType == ChartValueType.Double ||
                        XAxisValueType == ChartValueType.Logarithmic)
                    {
                        if (!(this.XValues is List<double>))
                            this.XValues = this.ActualXValues = new List<double>();
                        IList<double> xValue = this.XValues as List<double>;
                        object xVal = (obj as DataRow).Field<object>(XBindingPath);
                        object yVal = (obj as DataRow).Field<object>(tempYPath[0]);
                        if (replace && xValue.Count > index)
                        {
                            xValue[index] = Convert.ToDouble(xVal);
                        }
                        else
                        {
                            xValue.Insert(index, Convert.ToDouble(xVal));
                        }
                        if (replace && yValue.Count > index)
                        {
                            yValue[index] = Convert.ToDouble(yVal);
                        }
                        else
                        {
                            yValue.Insert(index, Convert.ToDouble(yVal));
                        }
                        ActualData.Add(obj);
                        dataCount = xValue.Count;
                    }
                    else if (XAxisValueType == ChartValueType.DateTime)
                    {
                        if (!(this.XValues is List<double>))
                            this.XValues = this.ActualXValues = new List<double>();
                        IList<double> xValue = this.XValues as List<double>;
                        object xVal = (obj as DataRow).Field<object>(XBindingPath);
                        object yVal = (obj as DataRow).Field<object>(tempYPath[0]);
                        if (replace && xValue.Count > index)
                        {
                            xValue[index] = Convert.ToDateTime(xVal).ToOADate();
                        }
                        else
                        {
                            xValue.Insert(index, Convert.ToDateTime(xVal).ToOADate());
                        }
                        if (replace && yValue.Count > index)
                        {
                            yValue[index] = Convert.ToDouble(yVal);
                        }
                        else
                        {
                            yValue.Insert(index, Convert.ToDouble(yVal));
                        }
                        ActualData.Add(obj);
                        dataCount = xValue.Count;
                    }
                    else if (XAxisValueType == ChartValueType.TimeSpan)
                    {
                        if (!(this.XValues is List<double>))
                            this.XValues = this.ActualXValues = new List<double>();
                        IList<double> xValue = this.XValues as List<double>;
                        object xVal = (obj as DataRow).Field<object>(XBindingPath);
                        object yVal = (obj as DataRow).Field<object>(tempYPath[0]);
                        if (xVal != null && replace && xValue.Count > index)
                        {
                            xValue[index] = ((TimeSpan)xVal).TotalMilliseconds;
                        }
                        else if (xVal != null)
                        {
                            xValue.Insert(index, ((TimeSpan)xVal).TotalMilliseconds);
                        }
                        if (yVal != null && replace && yValue.Count > index)
                        {
                            yValue[index] = Convert.ToDouble(yVal);
                        }
                        else if (yVal != null)
                        {
                            yValue.Insert(index, Convert.ToDouble(yVal));
                        }
                        ActualData.Add(obj);
                        dataCount = xValue.Count;
                    }
                }
            }
        }
#endif
        /// <summary>
        /// Called when DataSource changed
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected virtual void OnDataSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {


        }

        void OnDataCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Replace:
                    if (e.NewStartingIndex > -1 && e.NewStartingIndex < this.DataCount && e.NewItems[0] != null)
                    {
                        SetIndividualPoint(e.NewStartingIndex, e.NewItems[0], true);

                        if (IsSortData)
                        {
                            SortActualPoints();
                        }
                        this.UpdateSegments(e.OldStartingIndex, e.Action);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (this.ItemsSource != null)
                    {
                        if (XValues is IList<double>)
                        {
                            (XValues as IList<double>).RemoveAt(e.OldStartingIndex);
                            this.dataCount--;
                        }
                        else if (XValues is IList<string>)
                        {
                            (XValues as IList<string>).RemoveAt(e.OldStartingIndex);
                            this.dataCount--;
                        }
                        for (int i = 0; i < SeriesYValues.Count(); i++)
                        {
                            SeriesYValues[i].RemoveAt(e.OldStartingIndex);
                        }

                        if (IsSortData)
                        {
                            SortActualPoints();
                        }
                        this.ActualData.RemoveAt(e.OldStartingIndex);
                        this.UpdateSegments(e.OldStartingIndex, e.Action);
                    }
                    break;
                case NotifyCollectionChangedAction.Add:
                    {
                        if (this.ItemsSource != null)
                        {
                            if (!this.isNotificationSuspended)
                            {
                                this.SetIndividualPoint(e.NewStartingIndex, e.NewItems[0], false);
                                if (IsSortData)
                                {
                                    SortActualPoints();
                                }
                               
                                this.UpdateSegments(e.NewStartingIndex, e.Action);
                            }
                            else if (!isUpdateStarted)
                            {
                                updateStartedIndex = e.NewStartingIndex;
                                isUpdateStarted = true;
                            }
                        }
                        break;
                    }
                default:
                    {
                        Refresh();
                        break;
                    }
            }
            if (ShowEmptyPoints)
                RevalidateEmptyPointsCollection(e.Action,e.NewStartingIndex, e.OldStartingIndex);
            if (this is AccumulationSeriesBase)
                ActualArea.IsUpdateLegend = true;
            totalCalculated = false;
            if (ActualXAxis != null)
                ActualXAxis.IsDataChanged = true;
            if (ActualYAxis != null)
                ActualYAxis.IsDataChanged = true;
        }

        void Refresh()
        {
            if (ActualXValues is IList<double>)
            {
                (XValues as IList<double>).Clear();
                (ActualXValues as IList<double>).Clear();
            }
            else if (ActualXValues is IList<string>)
            {
                (XValues as IList<string>).Clear();
                (ActualXValues as IList<string>).Clear();
            }

            if (ActualSeriesYValues != null && ActualSeriesYValues.Count() > 0)
            {
                foreach (IList<double> list in ActualSeriesYValues)
                {
                    if (list != null)
                    {
                        list.Clear();
                    }
                }

                foreach (IList<double> list in SeriesYValues)
                {
                    if (list != null)
                    {
                        list.Clear();
                    }
                }
            }

            this.dataCount = 0;

            if (XBindingPath != null && YPaths != null && YPaths.Count() > 0)
            {
                GeneratePoints();
                Segments.Clear();
                if (this is AdornmentSeries)
                {
                    (this as AdornmentSeries).Adornments.Clear();
                }
                UpdateArea();
            }
        }

        /// <summary>
        /// Suspends the series from updating the series data till ResumeNotification is called. 
        /// This is specifically used when we need to append collection of datas.
        /// </summary>        
        [ClassReference(IsReviewed = false)]
        public void SuspendNotification()
        {
            isNotificationSuspended = true;
        }

        /// <summary>
        /// Processes the data that is added to data source after SuspendNotification.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public void ResumeNotification()
        {
            if (isNotificationSuspended)
            {
                isNotificationSuspended = false;

                if (!isUpdateStarted || updateStartedIndex < 0)
                    return;

                if (YPaths != null && ActualSeriesYValues != null && ItemsSource != null)
                {
                    GeneratePoints(YPaths, ActualSeriesYValues);
                    UpdateArea();
                }

                isUpdateStarted = false;
                updateStartedIndex = -1;
            }
        }

        internal virtual void CalculateSegments()
        {
            ApplyTemplate();
            //Segments.Clear();
            if (dataCount > 0)
            {
                if (ShowEmptyPoints && !isPointValidated)
                {
                    ValidateYValues();
                }
                isPointValidated = false;
                CreateSegments();
            }
        }

        private void RevalidateEmptyPointsCollection(NotifyCollectionChangedAction action, int newIndex, int oldIndex)
        {
            if (EmptyPointIndexes != null)
            {
                foreach (var index in EmptyPointIndexes[0])
                {
                    if (Segments.Count > index)
                        Segments[index].IsEmptySegmentInterior = false;
                }

                switch (action)
                {
                    case NotifyCollectionChangedAction.Replace:
                        if (double.IsNaN(SeriesYValues[0][newIndex]))
                        {
                            if (!(EmptyPointIndexes[0].Contains(newIndex)))
                            {
                                EmptyPointIndexes[0].Add(newIndex);
                            }
                        }
                        else if (EmptyPointIndexes[0].Contains(newIndex))
                        {
                            EmptyPointIndexes[0].Remove(newIndex);
                        }
                        break;

                    case NotifyCollectionChangedAction.Add:
                        if (double.IsNaN(SeriesYValues[0][newIndex]))
                        {
                            EmptyPointIndexes[0].Add(newIndex);
                        }
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        var postIndex = new List<int>();
                        foreach (var items in EmptyPointIndexes)
                        {
                            if (items.Contains(oldIndex))
                            {
                                items.Remove(oldIndex);
                            }

                            foreach (var value in items.Where(item => item > oldIndex))
                            {
                                var currY = value;
                                postIndex.Add(items.IndexOf(value));
                            }

                            foreach (var index in postIndex)
                            {
                                items[index] = --items[index];
                            }
                            postIndex.Clear();
                        }
                        break;
                }
            }
            RevalidateEmptyPointsValue();
        }

        private void RevalidateEmptyPointsValue()
        {
            if (Segments.Count > 0)
            {
                var yValues = SeriesYValues;

                var pathIndex = 0;
                if (EmptyPointIndexes !=null)
                foreach (var items in EmptyPointIndexes)
                {
                    switch (EmptyPointValue)
                    {
                        case EmptyPointValue.Zero:
                            foreach (var index in items)
                            {
                                yValues[pathIndex][index] = 0;
                            }
                            break;

                        case EmptyPointValue.Average:
                            var currYValues = yValues[pathIndex];
                            double x, y;
                            var sortedItems = from val in items
                                              orderby val ascending
                                              select val;
                            foreach (var index in sortedItems)
                            {
                                if (index == 0 && currYValues.Count > 1)
                                {
                                    x = double.IsNaN(currYValues[1]) ? 0 :currYValues[1] ;
                                    currYValues[0] = x / 2;
                                }
                                else if (index == currYValues.Count - 1 && currYValues.Count > 1)
                                {
                                    x = double.IsNaN(currYValues[index - 1]) ? 0 : currYValues[index - 1];
                                    currYValues[index] = x / 2;
                                }
                                else
                                {
                                    x = double.IsNaN(currYValues[index - 1]) ? 0 : currYValues[index - 1];
                                    y = double.IsNaN(currYValues[index + 1]) ? 0 : currYValues[index + 1]; 
                                    currYValues[index] = (x + y) / 2;
                                }
                            }
                            break;
                    }
                    pathIndex++;
                }
            }
            UpdateArea();
        }

        internal void ValidateDataPoints(params IList<double>[] yValues)
        {
            if (EmptyPointIndexes == null || EmptyPointIndexes.Count() == 0)
            EmptyPointIndexes = new List<int>[yValues.Length];
            int eindex = 0;
            foreach (var values in yValues)
            {
                if (EmptyPointIndexes[eindex] == null || EmptyPointIndexes[eindex].Count == 0)
                EmptyPointIndexes[eindex] = new List<int>();
                if (values.Count != 0)
                    switch (EmptyPointValue)
                    {
                        case EmptyPointValue.Zero:
                            for (int i = 0; i < values.Count; i++)
                            {
                                if (double.IsNaN(values[i]))
                                {
                                    values[i] = 0;
                                    EmptyPointIndexes[eindex].Add(i);
                                }
                            }
                            break;
                        case EmptyPointValue.Average:
                            int j = 0;
                            if (double.IsNaN(values[j]))
                            {
                                values[j] = (0 + (double.IsNaN(values[j + 1]) ? 0 : values[j + 1])) / 2;
                                EmptyPointIndexes[eindex].Add(0);
                            }
                            for (j = 1; j < values.Count - 1; j++)
                            {
                                if (double.IsNaN(values[j]))
                                {
                                    values[j] = (values[j - 1] + (double.IsNaN(values[j + 1]) ? 0 : values[j + 1])) / 2;
                                    EmptyPointIndexes[eindex].Add(j);
                                }
                            }
                            if (double.IsNaN(values[j]))
                            {
                                values[j] = values[j - 1] / 2;
                                EmptyPointIndexes[eindex].Add(j);
                            }
                            break;
                        default:
                            break;
                    }
                yValues[eindex] = values;
                eindex++;

            }
        }

        internal virtual void UpdateEmptyPointSegments(List<double> xValues)
        {
            int eIndex = 0;
            if (EmptyPointIndexes!=null && EmptyPointIndexes.Count() > 0)
                foreach (var values in ActualSeriesYValues)
                {
                    switch (EmptyPointStyle)
                    {
                        case EmptyPointStyle.Interior:
                            {
                                if(EmptyPointIndexes.Count()>eIndex)
                                foreach (var item in EmptyPointIndexes[eIndex])
                                {
                                    int index = (this is FunnelSeries) ? (this.DataCount - 1) - item : item;
                                    if ((this is LineSeries || this is SplineSeries || this is StepLineSeries
                                        || (this is PolarRadarSeriesBase && (this as PolarRadarSeriesBase).DrawType == ChartSeriesDrawType.Line)) && item != 0)
                                        Segments[index - 1].IsEmptySegmentInterior = true;
                                    Segments[((Segments.Count == index) ? index - 1 : index)].IsEmptySegmentInterior = true;
                                }
                                eIndex++;
                            }
                            break;
                        case EmptyPointStyle.Symbol:
                            {
                                if (EmptyPointIndexes.Count() > eIndex)
                                foreach (var item in EmptyPointIndexes[eIndex])
                                {
                                    int index = (this is FunnelSeries) ? (this.DataCount - 1) - item : item;
                                    Segments[index] = new EmptyPointSegment(xValues[item], values[item], this, false);
                                }
                                eIndex++;
                            }
                            break;
                        case EmptyPointStyle.SymbolAndInterior:
                            {
                                var emptyPointBrush = this.Interior;
                                if (EmptyPointIndexes.Count() > eIndex)
                                foreach (var item in EmptyPointIndexes[eIndex])
                                {
                                    int index = (this is FunnelSeries) ? (this.DataCount - 1) - item : item;
                                    if ((this is LineSeries || this is SplineSeries || this is StepLineSeries
                                       || (this is PolarRadarSeriesBase && (this as PolarRadarSeriesBase).DrawType == ChartSeriesDrawType.Line)) && item != 0)
                                        Segments[index - 1].IsEmptySegmentInterior = true;
                                    Segments[((Segments.Count == index) ? index - 1 : index)].IsEmptySegmentInterior = true;
                                    Segments[index] = new EmptyPointSegment(xValues[index], values[index], this, true);

                                }
                                eIndex++;
                            }
                            break;
                        default:
                            break;
                    }
                }
        }

        internal virtual void UpdateOnSeriesBoundChanged(Size size)
        {
            if(SeriesPanel==null && Segments.Count>0)
            {
                SeriesPanel = Segments[0].Series.GetTemplateChild("seriesPanel") as ChartSeriesPanel;
            }
            if (SeriesPanel != null)
            {
                foreach (ChartSegment segment in Segments)
                {
                    segment.OnSizeChanged(size);
                }
                SeriesPanel.Update(size);
            }
        }

        internal void UpdateLegendIconTemplate(bool iconChanged)
        {
            try
            {
                string legendIcon = LegendIcon.ToString();

                if (LegendIcon == ChartLegendIcon.SeriesType)
                    legendIcon = this.GetType().Name.Replace("Series", "");

                if ((LegendIconTemplate == null || iconChanged)
#if WINDOWS_PHONE
 && ChartDictionaries.GenericLegendDictionary.Contains(legendIcon))
#else
                && ChartDictionaries.GenericLegendDictionary.Keys.Contains(legendIcon))
#endif
                {
                    LegendIconTemplate = ChartDictionaries.GenericLegendDictionary[legendIcon] as DataTemplate;
                }
            }
            catch
            { }
        }



        internal virtual void UpdateRange()
        {

        }

        /// <summary>
        /// Invalidates the Series 
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public void Invalidate()
        {
            CalculateSegments();
        }

        /// <summary>
        /// An abstract method which will be called over to create segments.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public abstract void CreateSegments();

        /// <summary>
       /// Validate the datapoints for segment implementation.
       /// </summary>
        internal virtual void ValidateYValues(){}

        /// <summary>
        /// Method implementation for Clear unused segments
        /// </summary>
        /// <param name="startIndex"></param>
        protected void ClearUnUsedSegments(int startIndex)
        {
            var emptySegments = new List<ChartSegment>();
            foreach (var segment in Segments.Where(item => item is EmptyPointSegment))
            {
                emptySegments.Add(segment);
            }

            foreach (var segment in emptySegments)
            {
                Segments.Remove(segment);
            }

            if (this.Segments.Count > startIndex)
            {
                int count = this.Segments.Count;

                for (int i = startIndex; i < count; i++)
                {
                    this.Segments.RemoveAt(startIndex);
                }
            }
        }

        /// <summary>
        /// An abstract method which will called over each time in its child class to update an segment.
        /// </summary>
        /// <param name="index">The index of the segment</param>
        /// <param name="action">The collection changed action which raises the notification</param>
        [ClassReference(IsReviewed = false)]
        public virtual void UpdateSegments(int index, NotifyCollectionChangedAction action)
        {
            UpdateArea();
        }

        /// <summary>
        /// Removes the Segments
        /// </summary>
        internal void RemoveSegments()
        {
            for (int i = 0; i < Segments.Count; )
            {
                Segments.RemoveAt(i);
            }
        }



        internal Brush GetInteriorColor(int segmentIndex)
        {
            if (Interior != null)
                return Interior;
            if (Palette != ChartColorPalette.None)
            {
                return ColorModel.GetBrush(segmentIndex);
            }
            else if (ActualArea.Palette != ChartColorPalette.None)
            {
                int serIndex = ActualArea.VisibleSeries.IndexOf(this);
                if (serIndex >= 0)
                    return ActualArea.ColorModel.GetBrush(serIndex);
                else if (ActualArea is SfChart)
                {
                    serIndex = (ActualArea as SfChart).TechnicalIndicators.IndexOf(this as ChartSeries);
                    return ActualArea.ColorModel.GetBrush(serIndex);
                }
            }

            return null;
        }


        /// <summary>
        /// Returns the value of side by side position for a series.
        /// </summary>
        /// <param name="currentseries">ChartSeries.</param>
        /// <returns>The DoubleRange side by side Info</returns>
        [ClassReference(IsReviewed = false)]
        public DoubleRange GetSideBySideInfo(ChartSeriesBase currentseries)
        {
            if (this.ActualArea.InternalPrimaryAxis == null || this.ActualArea.InternalSecondaryAxis == null)
                return DoubleRange.Empty;

            if (!this.ActualArea.SBSInfoCalculated)
                CalculateSideBySidePositions();
            double width = 1 - ChartSeriesBase.GetSpacing(this);
            double minWidth = 0d;
            int all = 0;
            if (!double.IsNaN(this.ActualArea.MinPointsDelta))
            {
                minWidth = this.ActualArea.MinPointsDelta;
            }
            if (!this.ActualArea.SideBySideSeriesPlacement)
            {
                return new DoubleRange(-width / 2, width / 2);
            }
            int rowPos = currentseries.IsActualTransposed
                ? ActualArea.GetActualRow(currentseries.ActualXAxis)
                : ActualArea.GetActualRow(currentseries.ActualYAxis);
            int columnPos = currentseries.IsActualTransposed
                ? ActualArea.GetActualColumn(currentseries.ActualYAxis)
                : ActualArea.GetActualColumn(currentseries.ActualXAxis);
           
            var rowID = currentseries.ActualYAxis == null ? 0 : rowPos;
            var colID = currentseries.ActualXAxis == null ? 0 : columnPos;
            if ((rowID < this.ActualArea.SbsSeriesCount.GetLength(0)) && (colID < this.ActualArea.SbsSeriesCount.GetLength(1)))
                all = this.ActualArea.SbsSeriesCount[rowID, colID];
            else
                return DoubleRange.Empty;
            int pos = this.ActualArea.SeriesPosition[currentseries];
            if (all == 0)
            {
                all = 1;
                pos = 1;
            }
            double div = minWidth * width / all;
            double start = div * (pos - 1) - minWidth * width / 2;
            double end = start + div;
            
            // For adding additional space on both ends of side by side info series.
            CalculateSideBySideInfoPadding(minWidth, all, pos);
            
            return new DoubleRange(start, end);
        }

        private void CalculateSideBySideInfoPadding( double minWidth, int all, int pos)
        {
            bool isAlterRange = ((this.ActualXAxis is NumericalAxis && (this.ActualXAxis as NumericalAxis).RangePadding == NumericalPadding.None)
                    || (this.ActualXAxis is DateTimeAxis && (this.ActualXAxis as DateTimeAxis).RangePadding == DateTimeRangePadding.None) || (this.ActualXAxis is NumericalAxis3D && (this.ActualXAxis as NumericalAxis3D).RangePadding == NumericalPadding.None)
                    || (this.ActualXAxis is DateTimeAxis3D && (this.ActualXAxis as DateTimeAxis3D).RangePadding == DateTimeRangePadding.None));
            double space = isAlterRange ? 1 - ChartSeriesBase.GetSpacing(this) : ChartSeriesBase.GetSpacing(this);
            double div = minWidth * space / all;
            double padStart = div * (pos - 1) - minWidth * space / 2;
            double padEnd = padStart + div;
            SideBySideInfoRangePad = new DoubleRange(padStart, padEnd);

        }

        /// <summary>
        /// calculates the side-by-side position for all applicable series.
        /// </summary>
        private void CalculateSideBySidePositions()
        {
            ActualArea.SeriesPosition.Clear();
            int all = -1;
            int rowCount = this.ActualArea.RowDefinitions.Count;
            int columnCount = this.ActualArea.ColumnDefinitions.Count;
            this.ActualArea.SbsSeriesCount = new int[rowCount, columnCount];

            if (rowCount == 0)
            {
                ActualArea.SbsSeriesCount = new int[1, columnCount];
                rowCount = 1;
            }
            if (columnCount == 0)
            {
                ActualArea.SbsSeriesCount = new int[rowCount, 1];
                columnCount = 1;
            }
            var technicalIndicators = new List<ChartSeriesBase>();

            if (ActualArea is SfChart)
            {
                foreach (ChartSeries indicator in (ActualArea as SfChart).TechnicalIndicators)
                {
                    technicalIndicators.Add(indicator as ChartSeriesBase);
                }
            }
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    all = 0;

                    var filteredSeries = (from series in this is ChartSeries ? ActualArea.VisibleSeries.Union(technicalIndicators) : ActualArea.VisibleSeries
                        let rowPos =
                            series.IsActualTransposed
                                ? ActualArea.GetActualRow(series.ActualXAxis)
                                : ActualArea.GetActualRow(series.ActualYAxis)
                        let columnPos =
                            series.IsActualTransposed
                                ? ActualArea.GetActualColumn(series.ActualYAxis)
                                : ActualArea.GetActualColumn(series.ActualXAxis)
                        where columnPos == j && rowPos == i
                        select series).ToList();

                    int currStack = 0;
                    var stackedColumns = new List<ChartSeriesBase>();
                    foreach (ChartSeriesBase item in filteredSeries)
                    {
                        bool stacked = false;
                        if (item.IsSideBySide)
                        {
                            if (item.IsStacked)
                            {
                                if (stackedColumns.Count == 0)
                                {
                                    all++;
                                    ActualArea.SeriesPosition.Add(item, all);
                                    stackedColumns.Add(item);
                                    continue;
                                }

                                foreach (var stackedColumn in stackedColumns.Where(stackedColumn => (stackedColumn.ActualYAxis == item.ActualYAxis) && ((stackedColumn is StackingSeriesBase) ? ((stackedColumn as StackingSeriesBase).GroupingLabel == (item as StackingSeriesBase).GroupingLabel) :
                                    ((stackedColumn as StackingSeriesBase3D).GroupingLabel == (item as StackingSeriesBase3D).GroupingLabel))))
                                {  
                                    stacked = true;
                                    currStack = ActualArea.SeriesPosition[stackedColumn];
                                }

                                stackedColumns.Add(item);

                                if (stacked)
                                {
                                    ActualArea.SeriesPosition.Add(item, currStack);
                                }
                                else
                                {
                                    ActualArea.SeriesPosition.Add(item, ++all);
                                }
                            }
                            else
                            {
                                all++;
                                this.ActualArea.SeriesPosition.Add(item, all);
                            }
                        }
                    }
                    this.ActualArea.SbsSeriesCount[i, j] = all;
                }
            }
            this.ActualArea.SBSInfoCalculated = true;
        }

        protected virtual DependencyObject CloneSeries(DependencyObject obj)
        {
            ChartSeriesBase series = obj as ChartSeriesBase;
            ChartCloning.CloneControl(this, series);
            if(series is CartesianSeries)
            (series as CartesianSeries).IsTransposed = (this as CartesianSeries).IsTransposed;
            series.Palette = this.Palette;
            series.TrackBallLabelTemplate = this.TrackBallLabelTemplate;
            series.VisibilityOnLegend = this.VisibilityOnLegend;
            series.ColorModel = this.ColorModel;
            series.ItemsSource = this.ItemsSource;
            series.XBindingPath = this.XBindingPath;
            series.Stroke = this.Stroke;
            series.StrokeThickness = this.StrokeThickness;
            series.ShowEmptyPoints = this.ShowEmptyPoints;
            series.LegendIconTemplate = this.LegendIconTemplate;
            series.LegendIcon = this.LegendIcon;
            series.Label = this.Label;
            series.Interior = this.Interior;
            series.EmptyPointValue = this.EmptyPointValue;
            series.EmptyPointSymbolTemplate = this.EmptyPointSymbolTemplate;
            series.EmptyPointStyle = this.EmptyPointStyle;
            series.EmptyPointInterior = this.EmptyPointInterior;
            return series;
        }

        public DependencyObject Clone()
        {
            return this.CloneSeries(null);
        }

        internal virtual void Animate() { }

        #endregion
    }
}
