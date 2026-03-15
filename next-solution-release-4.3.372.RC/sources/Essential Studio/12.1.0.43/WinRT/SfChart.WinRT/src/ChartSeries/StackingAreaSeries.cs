#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

#else
using Windows.UI.Xaml;
using Windows.Foundation;
using System.Threading.Tasks;
using System.Collections;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;

#endif

namespace Syncfusion.UI.Xaml.Charts
{
    ///<summary>
    /// StackingAreaSeries is typically preferred in cases of multiple series of type <see cref="AreaSeries"/>.
    /// Each Series is then stacked vertically one above the other.
    /// If there exists only single series ,it will resemble like a simple <see cref="AreaSeries"/> chart.
    /// </summary>
    /// <seealso cref="StackingAreaSegment"/>
    /// <seealso cref="StackingColumnSeries"/>
    /// <seealso cref="StackingBarSeries"/>
    /// <seealso cref="AreaSeries"/>
    [ClassReference(IsReviewed = false)]
    public class StackingAreaSeries : StackingSeriesBase
    {
        #region fields

        private StackingAreaSegment Segment { get; set; }

        #endregion

        #region Properties

        protected override bool IsStacked
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region Constructor

      

        #endregion

        #region methods

        /// <summary>
        /// Called when DataSource property changed
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected override void OnDataSourceChanged(System.Collections.IEnumerable oldValue, System.Collections.IEnumerable newValue)
        {
            Segment = null;
            base.OnDataSourceChanged(oldValue, newValue);
        }

        /// <summary>
        /// Creates the segments of StackingAreaSeries
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public override void CreateSegments()
        {
            ClearUnUsedAdornments(this.DataCount);
            List<double> xValues = new List<double>();
            List<double> drawingListXValues=new List<double>();
            List<double> drawingListYValues=new List<double>();
            double Origin = this.ActualXAxis.Origin;
            xValues = GetXValues();
            var stackingValues = GetCumulativeStackValues(this);
            if (stackingValues != null)
            {
                YRangeStartValues = stackingValues.StartValues;
                YRangeEndValues = stackingValues.EndValues;

                if (YRangeStartValues != null)
                {
                    drawingListXValues.AddRange(xValues);
                    drawingListYValues.AddRange(YRangeStartValues);
                }
                else
                {
                    drawingListXValues.AddRange(xValues);
                    drawingListYValues = (from val in xValues select Origin).ToList();
                }

                drawingListXValues.AddRange((from val in xValues select val).Reverse().ToList());
                drawingListYValues.AddRange((from val in YRangeEndValues select val).Reverse().ToList());

                if (Segment == null)
                {
                    Segment = new StackingAreaSegment(drawingListXValues, drawingListYValues, this);
                    Segments.Add(Segment);
                }
                else
                    Segment.SetData(drawingListXValues, drawingListYValues);

                if (ShowEmptyPoints)
                    UpdateEmptyPointSegments(drawingListXValues);

                if (AdornmentsInfo != null)
                    AddStackingAreaAdornments(YRangeEndValues);
            }
        }

        private void AddStackingAreaAdornments(IList<double> yValues)
        {
            double adornX = 0d, adornY = 0d;
            int i = 0;
            List<double> xValues = GetXValues();
            for (i = 0; i < xValues.Count; i++)
            {
                adornX = xValues[i];
                adornY = yValues[i];
                if (i < Adornments.Count)
                {
                    Adornments[i].SetData(adornX, YValues[i],adornX, adornY);
                }
                else
                {
                    Adornments.Add(this.CreateAdornment(this, adornX, YValues[i], adornX, adornY));
                }
                Adornments[i].Item = ActualData[i];
            }
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new StackingAreaSeries());
        }

#if !WPF
        private RectAnimation animation;

        internal override bool GetAnimationIsActive()
        {
            return animation != null && animation.IsActive;
        }
#else
        RectangleGeometry geometry;
        internal override bool GetAnimationIsActive()
        {
            return geometry != null;
        }
#endif

        internal override void Animate()
        {
            var seriesRect = Area.SeriesClipRect;
#if WPF
            geometry = new RectangleGeometry();
            SeriesRootPanel.Clip = geometry;
            System.Windows.Media.Animation.RectAnimation animation = new System.Windows.Media.Animation.RectAnimation()
#else
            if (animation != null)
            {
                animation.Stop();
            }
            animation = new RectAnimation()
#endif
            {
                From = (IsActualTransposed) ? new Rect(0, seriesRect.Bottom, seriesRect.Width, seriesRect.Height) : new Rect(0, seriesRect.Y, 0, seriesRect.Height),
                To = (IsActualTransposed) ? new Rect(0, seriesRect.Y, 0, seriesRect.Height) : new Rect(0, seriesRect.Y, seriesRect.Width, seriesRect.Height),
#if NETFX_CORE || SILVERLIGHT
                Duration = AnimationDuration.TotalSeconds == 1 ? TimeSpan.FromSeconds(0.4) : AnimationDuration
#else
                Duration = AnimationDuration
#endif
            };
#if WPF
            Rect start = IsActualTransposed ? new Rect(0, seriesRect.Bottom, seriesRect.Width, seriesRect.Height) : new Rect(0, seriesRect.Y, 0, seriesRect.Height);
            Rect end = IsActualTransposed ? new Rect(0, seriesRect.Y, seriesRect.Width, seriesRect.Height) : new Rect(0, seriesRect.Y, seriesRect.Width, seriesRect.Height);
            RectAnimationUsingKeyFrames keyFrames = new RectAnimationUsingKeyFrames();
            SplineRectKeyFrame keyFrame = new SplineRectKeyFrame(start, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)));
            keyFrames.KeyFrames.Add(keyFrame);
            keyFrame = new SplineRectKeyFrame(end, KeyTime.FromTimeSpan(AnimationDuration));
            keyFrames.KeyFrames.Add(keyFrame);
            keyFrame.KeySpline = new KeySpline(0.65, 0.84, 0.67, 0.95);
            AnimationClock clock = keyFrames.CreateClock();
            clock.Completed += animationclock_Completed;
            geometry.BeginAnimation(RectangleGeometry.RectProperty, keyFrames);
#else
            animation.SetTarget(SeriesRootPanel);
            animation.Begin();
#endif
            if (this.AdornmentsInfo != null)
            {
                Storyboard sb = new Storyboard();
                double secondsPerPoint = (AnimationDuration.TotalSeconds / YValues.Count);
#if !WPF
                secondsPerPoint *= 2;
#endif
                int i = 0;
                foreach (ContentControl label in this.AdornmentsInfo.LabelPresenters)
                {
                    label.RenderTransform = new ScaleTransform() { ScaleY = 0, ScaleX = 0 };
                    label.RenderTransformOrigin = new Point(0.5, 0.5);
                    DoubleAnimation keyFrames1 = new DoubleAnimation()
                    {
                        From = 0.3,
                        To = 1,
                        Duration = TimeSpan.FromSeconds(AnimationDuration.TotalSeconds / 2),
                        BeginTime = TimeSpan.FromSeconds(i * secondsPerPoint)
                    };

#if !WINDOWS_PHONE
                    keyFrames1.EnableDependentAnimation = true;
                    Storyboard.SetTargetProperty(keyFrames1, "(UIElement.RenderTransform).(ScaleTransform.ScaleX)");
#else
                    Storyboard.SetTargetProperty(keyFrames1, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleX)"));
#endif
                    Storyboard.SetTarget(keyFrames1, label);
                    sb.Children.Add(keyFrames1);
                    keyFrames1 = new DoubleAnimation()
                    {
                        From = 0.3,
                        To = 1,
                        Duration = TimeSpan.FromSeconds(AnimationDuration.TotalSeconds / 2),
                        BeginTime = TimeSpan.FromSeconds(i * secondsPerPoint)
                    };
#if !WINDOWS_PHONE
                    keyFrames1.EnableDependentAnimation = true;
                    Storyboard.SetTargetProperty(keyFrames1, "(UIElement.RenderTransform).(ScaleTransform.ScaleY)");
#else
                    Storyboard.SetTargetProperty(keyFrames1, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleY)"));
#endif
                    Storyboard.SetTarget(keyFrames1, label);
                    sb.Children.Add(keyFrames1);
                    i++;
                }
                sb.Begin();
            }

        }

#if WPF
        void animationclock_Completed(object sender, EventArgs e)
        {
            (sender as AnimationClock).Completed -= animationclock_Completed;
            geometry = null;
        }
#endif

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
                double yVal = 0;
                double stackedYValue = double.NaN;
                object data = null;
                int index = 0;
                if (this.Area.SeriesClipRect.Contains(mousePos))
                {
                    var point = new Point(mousePos.X - this.Area.SeriesClipRect.Left
                                      , mousePos.Y - this.Area.SeriesClipRect.Top);

                    bool isStackedSeries = this is StackingSeriesBase;
                    this.FindNearestChartPoint(point, out xVal, out yVal, out stackedYValue);
                    if (double.IsNaN(xVal)) return;
                    index = this.GetXValues().IndexOf(xVal);
                    data = this.ActualData[index];
                }
                if (this.Area.Tooltip == null)
                    this.Area.Tooltip = new ChartTooltip();
                var chartTooltip = this.Area.Tooltip as ChartTooltip;
                ((e.OriginalSource as Shape).Tag as StackingAreaSegment).Item = data;
                ((e.OriginalSource as Shape).Tag as StackingAreaSegment).XData = xVal;
                ((e.OriginalSource as Shape).Tag as StackingAreaSegment).YData = yVal;
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


        #endregion
    }
}
