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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Shapes;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// SplineSeries is similar to that of <see cref="LineSeries"/> except that the points here are connected using smooth Bezier curves.
    /// </summary>
    /// <seealso cref="SplineSegment"/>
    [ClassReference(IsReviewed = false)]
    public class SplineSeries : XySeriesDraggingBase
    {
        #region fields

        double offsetPosition, initialPosition;
        bool dragged, isSeriesCaptured;
        Point initialPoint;
        List<SplineSegment> segments;
        List<double> previewYValues;

        #endregion

        #region constructor

        #endregion

        #region methods

        /// <summary>
        /// Event to show tooltip
        /// </summary>
        /// <param name="e"> Event Arguments</param>
#if WPF 
        protected override void OnMouseMove(MouseEventArgs e)
        {
        base.OnMouseMove(e);
#elif WINDOWS_PHONE && !SILVERLIGHT_UNCOMMON
        protected override void OnTap(GestureEventArgs e)
        {
        base.OnTap(e);
#elif SILVERLIGHT
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
#else
        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            base.OnPointerMoved(e);
#endif
            if (ShowTooltip)
            {
                if (e.OriginalSource as Shape == null || (e.OriginalSource as Shape).Tag == null)
                    return;
                timer.Start();
                timer.Interval = new TimeSpan(0, 0, 0, 2);
                var canvas = this.Area.GetAdorningCanvas();
#if !NETFX_CORE
                Point mousePos = e.GetPosition(this.Area);
#else
                Point mousePos = e.GetCurrentPoint(this.Area).Position;
#endif
                if (this.Area.Tooltip == null)
                    this.Area.Tooltip = new ChartTooltip();
                var chartTooltip = this.Area.Tooltip as ChartTooltip;
                if (chartTooltip != null)
                {

                    if (canvas.Children.Count == 0 || (canvas.Children.Count > 0 && !IsTooltipAvailable(canvas)))
                    {
                        double xVal = 0;
                        double yVal = 0;
                        double stackValue = double.NaN;
                        var point = new Point(mousePos.X - this.Area.SeriesClipRect.Left, mousePos.Y - this.Area.SeriesClipRect.Top);

                        FindNearestChartPoint(point, out xVal, out yVal, out stackValue);
                        var lineSegment = ((Shape)e.OriginalSource).Tag as SplineSegment;
                        (((Shape)e.OriginalSource).Tag as SplineSegment).YData = yVal == lineSegment.Y1
                                                                                    ? lineSegment.Y1
                                                                                    : lineSegment.Y2;

                        chartTooltip.Content = ((Shape)e.OriginalSource).Tag;

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
                        }
                    }
                    else
                    {
                        foreach (var child in canvas.Children)
                        {
                            if (child is ChartTooltip)
                                chartTooltip = child as ChartTooltip;
                        }
                        double xVal = 0;
                        double yVal = 0;
                        double stackValue = double.NaN;
                        var point = new Point(mousePos.X - this.Area.SeriesClipRect.Left, mousePos.Y - this.Area.SeriesClipRect.Top);

                        FindNearestChartPoint(point, out xVal, out yVal, out stackValue);
                        var lineSegment = ((Shape)e.OriginalSource).Tag as SplineSegment;
                        (((Shape)e.OriginalSource).Tag as SplineSegment).YData = yVal == lineSegment.Y1
                                                                                    ? lineSegment.Y1
                                                                                    : lineSegment.Y2;
                        chartTooltip.Content = ((Shape)e.OriginalSource).Tag;
                        chartTooltip.ContentTemplate = this.GetTooltipTemplate();
                        chartTooltip.LeftOffset = Position(mousePos, ref chartTooltip).X;
                        chartTooltip.TopOffset = Position(mousePos, ref chartTooltip).Y;
                        chartTooltip.Margin = ChartTooltip.GetTooltipMargin(this);
#if NETFX_CORE
                        if (_stopwatch.ElapsedMilliseconds > 100)
                        {
#endif
                        if (ChartTooltip.GetEnableAnimation(this)
#if WPF 
 && _stopwatch.ElapsedMilliseconds > 100
#endif
)
                        {

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
                        }
                        else
                        {
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

        protected override void SetDoubleAnimation(ChartTooltip chartTooltip)
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

        /// <summary>
        /// Creates the segments of SplineSeries.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public override void CreateSegments()
        {
            int index = -1;
            double[] yCoef = null;
            List<double> xValues = GetXValues();

            if (xValues != null)
            {
                ClearUnUsedSegments(this.DataCount);
                ClearUnUsedAdornments(this.DataCount);

                this.NaturalSpline(xValues, YValues, out yCoef);

                for (int i = 0; i < DataCount; i++)
                {
                    index = i + 1;
                    Point startPoint = new Point(xValues[i], YValues[i]);
                    if (index < DataCount)
                    {
                        Point endPoint = new Point(xValues[index], YValues[index]);
                        Point startControlPoint = new Point();
                        Point endControlPoint = new Point();
                        GetBezierControlPoints(startPoint, endPoint, yCoef[i], yCoef[index], out startControlPoint, out endControlPoint);

                        if (i < Segments.Count)
                        {
                            (Segments[i]).SetData(startPoint, startControlPoint, endControlPoint, endPoint);
                            (Segments[i] as SplineSegment).X1 = xValues[i];
                            (Segments[i] as SplineSegment).X2 = xValues[index];
                            (Segments[i] as SplineSegment).Y1 = YValues[i];
                            (Segments[i] as SplineSegment).Y2 = YValues[index];
                            (Segments[i] as SplineSegment).Item = this.ActualData[i]; 
                        }
                        else
                        {
                            SplineSegment splineSegment = new SplineSegment(startPoint, startControlPoint, endControlPoint, endPoint, this);
                            splineSegment.X1 = xValues[i];
                            splineSegment.X2 = xValues[index];
                            splineSegment.Y1 = YValues[i];
                            splineSegment.Y2 = YValues[index];
                            splineSegment.Item = this.ActualData[i];
                            Segments.Add(splineSegment);
                        }
                    }
                    else if (index == Segments.Count)
                        Segments.RemoveAt(i);
                    if (AdornmentsInfo != null)
                        AddAdornmentAtXY(startPoint.X, startPoint.Y, i);
                }
                if (ShowEmptyPoints)
                    UpdateEmptyPointSegments(xValues);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="xValues"></param>
        /// <param name="yValues"></param>
        /// <param name="ys2"></param>
        protected void NaturalSpline(List<double> xValues, IList<double> yValues, out double[] ys2)
        {
            int count = (int)DataCount;

            ys2 = new double[count];

            double a = 6;
            double[] u = new double[count];
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
        /// Returns the controlPoints of the curve
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

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new SplineSeries());
        }

        protected override void ResetDraggingElements(string reason, bool dragEndEvent)
        {
            if (SeriesPanel == null) return;
            base.ResetDraggingElements(reason, dragEndEvent);
            if (segments != null)
            {
                foreach (var segment in segments)
                {
                    SeriesPanel.Children.Remove(segment.GetRenderedVisual());
                }
                segments.Clear();
                segments = null;
            }
            isSeriesCaptured = false;
            DraggingSegment = null;
            dragged = false;
        }

        private void UpdatePreviewSegemntAndSeries(Point mousePos)
        {
            try
            {
                Brush brush = ((Path)Segments[0].GetRenderedVisual()).Stroke;
                var xValues = GetXValues();
                double[] yCoef;

                if (segments == null)
                {
                    segments = new List<SplineSegment>();
                    previewYValues = new List<double>();
                    NaturalSpline(xValues, YValues, out yCoef);
                    var chartTransformer = CreateTransformer(GetAvialableSize(), true);
                    double strokeThickness = ((Path)Segments[0].GetRenderedVisual()).StrokeThickness;
                    for (var i = 0; i < DataCount; i++)
                    {
                        var index = i + 1;
                        var startPoint = new Point(xValues[i], YValues[i]);
                        previewYValues.Add(YValues[i]);
                        if (index >= DataCount) continue;
                        var endPoint = new Point(xValues[index], YValues[index]);
                        Point startControlPoint;
                        Point endControlPoint;
                        GetBezierControlPoints(startPoint, endPoint, yCoef[i], yCoef[index], out startControlPoint, out endControlPoint);
                        var splineSegment = new SplineSegment(startPoint, startControlPoint, endControlPoint, endPoint, this);
                        var segemnt = splineSegment.CreateVisual(Size.Empty) as Path;
                        splineSegment.Update(chartTransformer);
                        segemnt.Stroke = brush;
                        segemnt.StrokeThickness = this.StrokeThickness;
                        segemnt.Opacity = strokeThickness;
                        segemnt.Opacity = 0.5;
                        segments.Add(splineSegment);
                        SeriesPanel.Children.Add(segemnt);
                    }
                }
                else
                {
                    if (isSeriesCaptured)
                    {
                        var newValue = Area.PointToValue(ActualYAxis, new Point(mousePos.X, mousePos.Y));
                        var baseValue = Area.PointToValue(ActualYAxis, IsActualTransposed ? new Point(offsetPosition, mousePos.Y) 
                                                                      : new Point(mousePos.X, offsetPosition));
                        var offset = newValue - baseValue;
                        
                        for (var i = 0; i < previewYValues.Count; i++)
                        {
                            previewYValues[i] = previewYValues[i] + offset;
                        }
                        offsetPosition = IsActualTransposed ? mousePos.X : mousePos.Y;
                        if (IsActualTransposed)
                            DraggedValue = Area.PointToValue(ActualXAxis, new Point(0, initialPosition)) - Area.PointToValue(ActualXAxis, new Point(mousePos.Y, mousePos.X));
                        else
                            DraggedValue = Area.PointToValue(ActualYAxis, mousePos) - Area.PointToValue(ActualYAxis, new Point(0, initialPosition));

                        

                        var dragEvent = new XySeriesDragEventArgs { Delta = DraggedValue, BaseXValue = SegmentIndex };
                        RaiseDragDelta(dragEvent);
                        if (dragEvent.Cancel)
                        {
                            ResetDraggingElements("Cancel", true);
                            return;
                        }
                        if (!IsActualTransposed)
                            UpdateSeriesDragValueToolTip(mousePos, brush, DraggedValue, YValues[0], Area.ValueToPoint(ActualXAxis, 0));
                        else
                            UpdateSeriesDragValueToolTip(mousePos, brush, DraggedValue, 0, Area.ValueToPoint(ActualYAxis, YValues[0]));

                        
                    }
                    else
                    {
                        DraggedValue = Area.PointToValue(ActualYAxis, mousePos);
                        previewYValues[SegmentIndex] = Area.PointToValue(ActualYAxis, mousePos);
                        var dragEvent = new XySegmentDragEventArgs { BaseYValue = YValues[SegmentIndex], NewYValue = Area.PointToValue(ActualYAxis, mousePos), Segment = DraggingSegment };
                        RaiseDragDelta(dragEvent);
                        if (dragEvent.Cancel)
                            return;
                        if (IsActualTransposed)
                            UpdateSegmentDragValueToolTip(new Point(mousePos.X, Area.ValueToPoint(ActualXAxis, SegmentIndex)), DraggingSegment, DraggedValue, 0);
                        else 
                            UpdateSegmentDragValueToolTip(new Point(Area.ValueToPoint(ActualXAxis, SegmentIndex), mousePos.Y), DraggingSegment, DraggedValue, 0);
                    }
                    NaturalSpline(xValues, previewYValues, out yCoef);
                    var chartTransformer = CreateTransformer(GetAvialableSize(), true);
                    for (var i = 0; i < DataCount; i++)
                    {
                        var index = i + 1;
                        var startPoint = new Point(xValues[i], previewYValues[i]);
                        if (index >= DataCount) continue;
                        var endPoint = new Point(xValues[index], previewYValues[index]);
                        Point startControlPoint;
                        Point endControlPoint;
                        GetBezierControlPoints(startPoint, endPoint, yCoef[i], yCoef[index], out startControlPoint, out endControlPoint);
                        segments[i].SetData(startPoint, startControlPoint, endControlPoint, endPoint);
                        segments[i].Update(chartTransformer);
                    }
                }
                dragged = true;
            }
            catch
            {
                ResetDraggingElements("Exception", true);
            }
        }

        private void UpdateDraggedSource()
        {
            try
            {
                if (dragged)
                {
                    var baseValue = YValues[SegmentIndex];
                    var dragPreviewEnd = new XyPreviewEndEventArgs { BaseYValue = baseValue, NewYValue = DraggedValue };
                    RaisePreviewEnd(dragPreviewEnd);

                    if (dragPreviewEnd.Cancel)
                    {
                        ResetDraggingElements("", false);
                        return;
                    }
                    if (isSeriesCaptured)
                    {
                        for (var i = 0; i < YValues.Count; i++)
                        {
                            YValues[i] = GetSnapToPoint(YValues[i] + DraggedValue);
                        }
                        if (UpdateSource)
                            UpdateUnderLayingModel(YBindingPath, YValues);
                    }
                    else
                    {
                        DraggedValue = GetSnapToPoint(DraggedValue);
                        ActualSeriesYValues[0][SegmentIndex] = DraggedValue;
                        if (UpdateSource && !IsSortData)
                            UpdateUnderLayingModel(YBindingPath, SegmentIndex, DraggedValue);
                    }
                    UpdateArea();
                    RaiseDragEnd(new ChartDragEndEventArgs { BaseYValue = baseValue, NewYValue = DraggedValue });
                }
                ResetDraggingElements("", false);
            }
            catch
            {
                ResetDraggingElements("Exception", true);
            }
        }

        internal override void ActivateDragging(Point mousePos, object chartElement)
        {
            try
            {
                if (DraggingSegment != null || segments != null) return;
                var element = chartElement as FrameworkElement;
                if (element == null)
                    return;
                base.ActivateDragging(mousePos, chartElement);
                if (SegmentIndex < 0) return;
                DraggingSegment = element.Tag as SplineSegment;
                if (element is Path && EnableSeriesDragging && DraggingSegment != null)
                {
                    isSeriesCaptured = true;
                    offsetPosition = initialPosition = IsActualTransposed ? mousePos.X : mousePos.Y;
                }
                else if (DraggingSegment != null || element.DataContext is ChartAdornmentContainer)
                {
                    double x, y, stackedValue;
                    FindNearestChartPoint(mousePos, out x, out y, out stackedValue);
                    Area.ValueToPoint(ActualXAxis, x);

                    SegmentIndex = (int)x;
                    if (SegmentIndex == Segments.Count)
                        DraggingSegment = Segments[SegmentIndex - 1] as SplineSegment;
                    else
                        DraggingSegment = Segments[SegmentIndex] as SplineSegment;
                }
            }
            catch
            {
                ResetDraggingElements("Exception", true);
            }
        }

        internal override void UpdatePreviewSegemntDragging(Point mousePos)
        {
            UpdatePreviewSegemntAndSeries(mousePos);
            base.UpdatePreviewSegemntDragging(mousePos);
        }

        internal override void UpdatePreivewSeriesDragging(Point mousePos)
        {
            UpdatePreviewSegemntAndSeries(mousePos);
            base.UpdatePreivewSeriesDragging(mousePos);
        }

        protected override void OnChartDragStart(Point mousePos, object originalSource)
        {
            if (EnableSeriesDragging || EnableSegmentDragging)
            {
                ActivateDragging(mousePos, originalSource);
                initialPoint = mousePos;
            }
        }

        protected override void OnChartDragEnd(Point mousePos, object originalSource)
        {
            UpdateDraggedSource();
            base.OnChartDragEnd(mousePos, originalSource);
        }

        #endregion
    }
}
