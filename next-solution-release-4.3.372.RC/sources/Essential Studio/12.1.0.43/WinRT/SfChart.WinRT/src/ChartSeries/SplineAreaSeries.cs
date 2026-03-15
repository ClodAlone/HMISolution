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
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// SplineAreaSeries connects it data points using a smooth line segment with the areas below are filled in.
    /// </summary>
    /// <seealso cref="SplineAreaSegment"/>
    /// <seealso cref="SplineSeries"/>
    /// <seealso cref="AreaSeries"/>
    [ClassReference(IsReviewed = false)]
    public class SplineAreaSeries : XyDataSeries
    {
        #region fields

        private SplineAreaSegment Segment { get; set; }

        #endregion

        #region constructor

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
        /// Creates the segments of SplineAreaSeries
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public override void CreateSegments()
        {
            int index = -1;
            double[] yCoef = null;
            var segmentPoints = new List<Point>();
            double Origin = this.ActualXAxis.Origin;
            List<double> xValues = GetXValues();

            if (xValues != null)
            {
                ClearUnUsedAdornments(this.DataCount);
                this.NaturalSpline(xValues,YValues, out yCoef);
                Point initialPoint=new Point(xValues[0],YValues[0]);
                segmentPoints.Add(initialPoint);
                for (int i = 0; i < DataCount; i++)
                {
                    index = i + 1;
                    if (index < DataCount)
                    {
                        Point startPoint = new Point(xValues[i],YValues[i]);
                        Point endPoint = new Point(xValues[index], YValues[index]);
                        Point startControlPoint = new Point();
                        Point endControlPoint = new Point();
                        GetBezierControlPoints(startPoint, endPoint, yCoef[i], yCoef[index], out startControlPoint, out endControlPoint);
                        segmentPoints.AddRange(new Point[] { startControlPoint, endControlPoint, endPoint });
                    }
                }
                if (Segment == null)
                {
                    Segment = new SplineAreaSegment(segmentPoints, xValues, YValues, this);
                    Segments.Add(Segment);
                }
                else
                {
                    (Segment as SplineAreaSegment).SetData(segmentPoints, xValues, YValues);
                }

                if (AdornmentsInfo != null)
                    AddAreaAdornments(YValues);
            }
        }
        /// <summary>
        /// Method implementation for NaturalSpline
        /// </summary>
        /// <param name="xValues"></param>
        /// <param name="yValues"></param>
        /// <param name="ys2"></param>
        protected void NaturalSpline(List<double> xValues, IList<double> yValues, out double[] ys2)
        {
            int count = (int)DataCount;

            ys2 = new double[count];

            double a = 6;
            double[] u = new double[count - 1];
            double p;

            ys2[0] = u[0] = 0;
            ys2[count - 1] = 0;

            for (int i = 1; i < count - 1; i++)
            {
                double d1 = xValues[i] - xValues[i - 1];
                double d2 = xValues[i + 1] - xValues[i - 1];
                double d3 = xValues[i + 1] - xValues[i];
                double dy1 = yValues[i + 1] - yValues[i];
                double dy2 = yValues[i] - yValues[i - 1];

                if (xValues[i] == xValues[i - 1] || xValues[i] == xValues[i + 1])
                {
                    ys2[i] = 0;
                    u[i] = 0;
                }
                else
                {
                    p = 1 / (d1 * ys2[i - 1] + 2 * d2);
                    ys2[i] = -p * d3;
                    u[i] = p * (a * (dy1 / d3 - dy2 / d1) - d1 * u[i - 1]);
                }
            }

            for (int k = count - 2; k >= 0; k--)
            {
                ys2[k] = ys2[k] * ys2[k + 1] + u[k];
            }
        }
        /// <summary>
        /// Method implementation for GetBezierControlPoints
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="ys1"></param>
        /// <param name="ys2"></param>
        /// <param name="controlPoint1"></param>
        /// <param name="controlPoint2"></param>
        protected void GetBezierControlPoints(Point point1, Point point2, double ys1, double ys2, out Point controlPoint1, out Point controlPoint2)
        {
            const double One_thrid = 1 / 3.0d;
            double deltaX2 = point2.X - point1.X;

            deltaX2 = deltaX2 * deltaX2;

            double dx1 = 2 * point1.X + point2.X;
            double dx2 = point1.X + 2 * point2.X;

            double dy1 = 2 * point1.Y + point2.Y;
            double dy2 = point1.Y + 2 * point2.Y;

            double y1 = One_thrid * (dy1 - One_thrid * deltaX2 * (ys1 + 0.5f * ys2));
            double y2 = One_thrid * (dy2 - One_thrid * deltaX2 * (0.5f * ys1 + ys2));

            controlPoint1 = new Point(dx1 * One_thrid, y1);
            controlPoint2 = new Point(dx2 * One_thrid, y2);
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new SplineAreaSeries());
        }

#if !WPF
        private RectAnimation animation;

        internal override bool GetAnimationIsActive()
        {
            return animation != null && animation.IsActive;
        }
#endif

        internal override void Animate()
        {
            var seriesRect = Area.SeriesClipRect;
#if WPF
            RectangleGeometry geometry = new RectangleGeometry();
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

                    this.FindNearestChartPoint(point, out xVal, out yVal, out stackedYValue);
                    if (double.IsNaN(xVal)) return;
                    index = this.GetXValues().IndexOf(xVal);
                    data = this.ActualData[index];
                }
                if (this.Area.Tooltip == null)
                    this.Area.Tooltip = new ChartTooltip();
                var chartTooltip = this.Area.Tooltip as ChartTooltip;
                ((e.OriginalSource as Shape).Tag as SplineAreaSegment).Item = data;
                ((e.OriginalSource as Shape).Tag as SplineAreaSegment).XData = xVal;
                ((e.OriginalSource as Shape).Tag as SplineAreaSegment).YData = yVal;
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
