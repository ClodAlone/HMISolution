#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE
using System.Windows;
using System.Collections;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

#else
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
using System.Collections;
using Windows.UI.Xaml.Shapes;

#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Class implementation for PolarRadarSeriesBase
    /// </summary>
    public abstract class PolarRadarSeriesBase:AdornmentSeries ,ISupportAxes2D
    {
        #region Properties

        /// <summary>
        /// Get or Set YBindingPathProperty
        /// </summary>
        public string YBindingPath
        {
            get { return (string)GetValue(YBindingPathProperty); }
            set { SetValue(YBindingPathProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for YBindingPath.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty YBindingPathProperty =
            DependencyProperty.Register("YBindingPath", typeof(string), typeof(PolarRadarSeriesBase), new PropertyMetadata(null, OnYPathChanged));

        private static void OnYPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as PolarRadarSeriesBase).OnBindingPathChanged(e);
        }


        /// <summary>
        /// Gets or sets a value that indicates whether the first and last segments are connected.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public bool IsClosed
        {
            get { return (bool)GetValue(IsClosedProperty); }
            set { SetValue(IsClosedProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for IsClosed.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsClosedProperty =
            DependencyProperty.Register("IsClosed", typeof(bool), typeof(PolarRadarSeriesBase), new PropertyMetadata(true, OnDrawValueChanged));


        /// <summary>
        /// Gets or Sets DrawType.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public ChartSeriesDrawType DrawType
        {
            get { return (ChartSeriesDrawType)GetValue(DrawTypeProperty); }
            set { SetValue(DrawTypeProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for DrawType.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DrawTypeProperty =
            DependencyProperty.Register("DrawType", typeof(ChartSeriesDrawType), typeof(PolarRadarSeriesBase), new PropertyMetadata(ChartSeriesDrawType.Area, OnDrawValueChanged));


        /// <summary>
        /// Get or Set YValues property
        /// </summary>
        protected IList<double> YValues { get; set; }
        /// <summary>
        /// Get or Set Segment property
        /// </summary>
        protected ChartSegment Segment { get; set; }
        /// <summary>
        /// Get or Set XRange property
        /// </summary>
        public DoubleRange XRange { get; internal set; }

        /// <summary>
        /// Get or Set YRange property
        /// </summary>
        public DoubleRange YRange { get; internal set; }
        /// <summary>
        /// Get or Set XAxis property
        /// </summary>
        public ChartAxisBase2D XAxis
        {
            get { return (ChartAxisBase2D)GetValue(XAxisProperty); }
            set { SetValue(XAxisProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for XAxis.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty XAxisProperty =
            DependencyProperty.Register("XAxis", typeof(ChartAxisBase2D), typeof(PolarRadarSeriesBase), new PropertyMetadata(null, OnXAxisChanged));

        /// <summary>
        /// Get or Set YAxisProperty
        /// </summary>
        public RangeAxisBase YAxis
        {
            get { return (RangeAxisBase)GetValue(YAxisProperty); }
            set { SetValue(YAxisProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for YAxis.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty YAxisProperty =
            DependencyProperty.Register("YAxis", typeof(RangeAxisBase), typeof(PolarRadarSeriesBase), new PropertyMetadata(null, OnYAxisChanged));

        ChartAxis ISupportAxes.ActualXAxis
        {
            get { return ActualXAxis; }
        }

        ChartAxis ISupportAxes.ActualYAxis
        {
            get { return ActualYAxis; }
        }

        #endregion

        #region Ctor

        /// <summary>
        /// Called when instance created for PolarRadarSeriesBase
        /// </summary>
        public PolarRadarSeriesBase()
        {
            YValues = new List<double>();
        }
        #endregion

        #region Methods

        private static void OnDrawValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as PolarRadarSeriesBase).UpdateArea();
        }

        private static void OnYAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as PolarRadarSeriesBase).OnYAxisChanged(e.OldValue as ChartAxis, e.NewValue as ChartAxis);
        }

        private static void OnXAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as PolarRadarSeriesBase).OnXAxisChanged(e.OldValue as ChartAxis, e.NewValue as ChartAxis);
        }
        /// <summary>
        /// Called when YAxis property changed
        /// </summary>
        /// <param name="oldAxis"></param>
        /// <param name="newAxis"></param>
        protected virtual void OnYAxisChanged(ChartAxis oldAxis, ChartAxis newAxis)
        {
            if (newAxis != null && !newAxis.RegisteredSeries.Contains(this))
            {
                newAxis.Area = Area;
                if (Area != null && !Area.Axes.Contains(newAxis))
                    Area.Axes.Add(newAxis);
                newAxis.Orientation = Orientation.Vertical;
                newAxis.RegisteredSeries.Add(this);
            }

            if (oldAxis != null)
            {
                if (oldAxis.RegisteredSeries.Contains(this))
                    oldAxis.RegisteredSeries.Remove(this);
                if (Area != null && oldAxis.RegisteredSeries.Count == 0)
                {
                    if (Area.Axes.Contains(oldAxis) && Area.InternalPrimaryAxis != oldAxis && Area.InternalSecondaryAxis != oldAxis)
                        Area.Axes.Remove(oldAxis);
                }
            }

            if (Area != null) Area.ScheduleUpdate();
        }
        /// <summary>
        /// Called when XAxis property changed
        /// </summary>
        /// <param name="oldAxis"></param>
        /// <param name="newAxis"></param>
        protected virtual void OnXAxisChanged(ChartAxis oldAxis, ChartAxis newAxis)
        {
            if (newAxis != null)
            {
                if (Area != null && !Area.Axes.Contains(newAxis))
                    Area.Axes.Add(newAxis);
                newAxis.Orientation = Orientation.Horizontal;
                if (!newAxis.RegisteredSeries.Contains(this))
                    newAxis.RegisteredSeries.Add(this);
            }
           
            if (oldAxis != null)
            {
                if (oldAxis.RegisteredSeries.Contains(this))
                    oldAxis.RegisteredSeries.Remove(this);

                if (Area != null && oldAxis.RegisteredSeries.Count > 0)
                {
                    if (Area.Axes.Contains(oldAxis) && Area.InternalPrimaryAxis != oldAxis && Area.InternalSecondaryAxis != oldAxis)
                        Area.Axes.Remove(oldAxis);
                }
            }
            if (Area != null) Area.ScheduleUpdate();
        }

        /// <summary>
        /// Called when DataSource property changed 
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected override void OnDataSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            YValues.Clear();
            Segment = null;
            GeneratePoints(new string[] { YBindingPath }, YValues);
            this.UpdateArea();
        }


        protected override void OnBindingPathChanged(DependencyPropertyChangedEventArgs args)
        {
            YValues.Clear();
            Segment = null;
            base.OnBindingPathChanged(args);
        }
        /// <summary>
        /// Method implementation  for GeneratePoints for Adornments
        /// </summary>
        protected internal override void GeneratePoints()
        {
            GeneratePoints(new string[] { YBindingPath }, YValues);
        }

        internal override void UpdateRange()
        {
            XRange = DoubleRange.Empty;
            YRange = DoubleRange.Empty;

            foreach (ChartSegment segment in Segments)
            {
                XRange += segment.XRange;
                YRange += segment.YRange;
            }
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            (obj as PolarRadarSeriesBase).IsClosed = this.IsClosed;
            (obj as PolarRadarSeriesBase).YBindingPath = this.YBindingPath;
            (obj as PolarRadarSeriesBase).DrawType = this.DrawType;
            return base.CloneSeries(obj);
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

        /// <summary>
        /// Validate the datapoints for segment implementation.
        /// </summary>
        internal override void ValidateYValues()
        {
            foreach (var yValue in YValues)
            {
                if (double.IsNaN(yValue) && ShowEmptyPoints)
                    ValidateDataPoints(YValues); break;
            }
        }

        internal override void RemoveTooltip()
        {
            var canvas = this.Area.GetAdorningCanvas();
            if (canvas.Children.Contains((this.Area.Tooltip as ChartTooltip)))
                canvas.Children.Remove(this.Area.Tooltip as ChartTooltip);
        }

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
            if (ShowTooltip)
            {
                if (e.OriginalSource as Shape == null || (e.OriginalSource as Shape).Tag == null)
                    return;
                timer.Start();
                timer.Interval = new TimeSpan(0, 0, 0, 1);
                var canvas = this.Area.GetAdorningCanvas();
#if !NETFX_CORE
                Point mousePos = e.GetPosition(canvas);
#else
                Point mousePos = e.GetCurrentPoint(canvas).Position;
#endif
                double xVal = 0;
                object data = null;
                double xStart = ActualXAxis.VisibleRange.Start;
                double xEnd = ActualXAxis.VisibleRange.End;
                int index = 0;
                if (this.Area.SeriesClipRect.Contains(mousePos))
                {
                    var point = new Point(mousePos.X - this.Area.SeriesClipRect.Left
                                       , mousePos.Y - this.Area.SeriesClipRect.Top);
                    double center = 0.5 * Math.Min(this.Area.SeriesClipRect.Width, this.Area.SeriesClipRect.Height);
                    double radian=ChartTransform.PointToPolarRadian(point, center);
                    double coeff = ChartTransform.RadianToPolarCoefficient(radian);
                    xVal = Math.Round(this.Area.InternalPrimaryAxis.PolarCoefficientToValue(coeff));
                    if (xVal <= xEnd && xVal >= xStart)
                    index = this.GetXValues().IndexOf((int)xVal);
                    data = this.ActualData[index];
                }
                var chartTooltip = this.Area.Tooltip as ChartTooltip;
                if (this.DrawType == ChartSeriesDrawType.Area)
                {
                    ((e.OriginalSource as Shape).Tag as AreaSegment).Item = data;
                    ((e.OriginalSource as Shape).Tag as AreaSegment).XData = xVal;
                    ((e.OriginalSource as Shape).Tag as AreaSegment).YData = this.YValues[(int)index];
                }
                else
                {
                    ((e.OriginalSource as Shape).Tag as LineSegment).Item = data;
                    ((e.OriginalSource as Shape).Tag as LineSegment).YData = this.YValues[(int)index];
                }
                if (chartTooltip != null)
                {
                    if (canvas.Children.Count == 0 || (canvas.Children.Count > 0 && !IsTooltipAvailable(canvas)))
                    {
                        chartTooltip.Content = (e.OriginalSource as Shape).Tag;
                        if (chartTooltip.Content == null)
                            return;
                        canvas.Children.Add(chartTooltip);
                        chartTooltip.ContentTemplate = this.GetTooltipTemplate();
                        chartTooltip.LeftOffset = Position(mousePos, ref chartTooltip).X;
                        chartTooltip.TopOffset = Position(mousePos, ref chartTooltip).Y;
                        chartTooltip.Margin = ChartTooltip.GetTooltipMargin(this);

                        if (ChartTooltip.GetEnableAnimation(this))
                        {
                            SetDoubleAnimation(chartTooltip);
                            storyBoard.Children.Add(leftDoubleAnimation);
                            storyBoard.Children.Add(topDoubleAnimation);
                            Canvas.SetLeft(chartTooltip, chartTooltip.LeftOffset);
                            Canvas.SetTop(chartTooltip, chartTooltip.TopOffset);
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
                        chartTooltip.Content = (e.OriginalSource as Shape).Tag;
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
#endif
                    }
                }
            }
        }

        #endregion
    }
}
