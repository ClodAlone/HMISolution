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
using System.Diagnostics;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Media;

#else
using Windows.UI.Xaml;
using Windows.Foundation;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media;

#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// A line chart displays series as a set of points connected by using a straight line.
    /// Line charts are used to represent large amounts of data observed over a continuous period of time.
    /// </summary>
    /// <remarks>
    /// LineChart appearance can be customized by using <see cref="LineSeries.CustomTemplate"/> property.
    /// </remarks>
    /// <seealso cref="LineSegment"/>
    /// <seealso cref="FastLineSeries"/>
    /// <seealso cref="FastLineBitmapSeries"/>
    /// <seealso cref="SplineSeries"/>
    [ClassReference(IsReviewed = false)]
    public class LineSeries : XySeriesDraggingBase
    {
        #region Fields
#if WPF || NETFX_CORE
        readonly Stopwatch stopwatch = new Stopwatch();
#endif
        Line previewCurrLine, previewPreLine;
        Point initialPoint;

        double offsetPosition, initialPosition;
        PointCollection pointCollection;
        bool isReversed, dragged;
        #endregion

        #region Properties

        /// <summary>
        /// Gets or Sets the DataTemplate for line segment.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public DataTemplate CustomTemplate
        {
            get { return (DataTemplate)GetValue(CustomTemplateProperty); }
            set { SetValue(CustomTemplateProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for Template.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CustomTemplateProperty =
            DependencyProperty.Register("CustomTemplate", typeof(DataTemplate), typeof(LineSeries), new PropertyMetadata(null));


        #endregion

        #region ctor
                
        #endregion

        #region methods

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
                timer.Interval = new TimeSpan(0, 0, 0, 1);
                var canvas = this.Area.GetAdorningCanvas();
#if !NETFX_CORE
                Point mousePos = e.GetPosition(canvas);
#else
                Point mousePos = e.GetCurrentPoint(canvas).Position;
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
                        if (double.IsNaN(xVal)) return;
                        var lineSegment = ((Shape)e.OriginalSource).Tag as LineSegment;
                        (((Shape)e.OriginalSource).Tag as LineSegment).YData = yVal == lineSegment.Y1Value
                                                                                    ? lineSegment.Y1Value
                                                                                    : lineSegment.Y2Value;
                        
                        chartTooltip.Content = ((Shape) e.OriginalSource).Tag;
                        
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
                            stopwatch.Start();
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
                        if (double.IsNaN(xVal)) return;
                        var lineSegment = ((Shape)e.OriginalSource).Tag as LineSegment;
                        (((Shape) e.OriginalSource).Tag as LineSegment).YData = yVal == lineSegment.Y1Value
                                                                                    ? lineSegment.Y1Value
                                                                                    : lineSegment.Y2Value;
                        chartTooltip.Content = ((Shape)e.OriginalSource).Tag;
                        chartTooltip.ContentTemplate = this.GetTooltipTemplate();     
                        chartTooltip.LeftOffset = Position(mousePos, ref chartTooltip).X;
                        chartTooltip.TopOffset = Position(mousePos, ref chartTooltip).Y;
                        chartTooltip.Margin = ChartTooltip.GetTooltipMargin(this);
#if NETFX_CORE
                        if (stopwatch.ElapsedMilliseconds > 100)
                        {
#endif
                        if (ChartTooltip.GetEnableAnimation(this)
#if WPF 
 && stopwatch.ElapsedMilliseconds > 100
#endif
)
                        {

#if WPF || NETFX_CORE
                                stopwatch.Restart();
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
        /// Creates the segments of LineSeries.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public override void CreateSegments()
        {
            int index=-1;

            List<double> xValues = GetXValues();
            
            if (xValues != null)
            {
                ClearUnUsedSegments(this.DataCount);
                ClearUnUsedAdornments(this.DataCount);

                for (int i = 0; i < this.DataCount; i++)
                {
                    index = i + 1;
                    if (i < Segments.Count)
                    {
                        Segments[i].Item = ActualData[i];
                        if (index < this.DataCount)
                            (Segments[i]).SetData(xValues[i], YValues[i], xValues[index], YValues[index]);
                        else
                            Segments.RemoveAt(i);
                    }
                    else
                    {
                        if (index < this.DataCount)
                            Segments.Add(new LineSegment(xValues[i], YValues[i], xValues[index], YValues[index], this, ActualData[i]));
                    }

                    if (AdornmentsInfo != null)
                    {
                        if (i < Adornments.Count)
                        {
                            Adornments[i].SetData(xValues[i], YValues[i], xValues[i], YValues[i]);
                        }
                        else
                        {
                            Adornments.Add(this.CreateAdornment(this, xValues[i], YValues[i], xValues[i], YValues[i]));
                        }
                        Adornments[i].Item = ActualData[i];
                    }
                }

                if (ShowEmptyPoints)
                    UpdateEmptyPointSegments(xValues);
            }
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new LineSeries() { CustomTemplate = this.CustomTemplate });
        }

#if !WPF
        private RectAnimation animation;

        internal override bool GetAnimationIsActive()
        {
            return animation!=null && animation.IsActive;
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
            Rect end = IsActualTransposed ? new Rect(0, seriesRect.Y-seriesRect.Top, seriesRect.Width, seriesRect.Height) : new Rect(0, seriesRect.Y, seriesRect.Width, seriesRect.Height);
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
                double secondsPerPoint = AnimationDuration.TotalSeconds / YValues.Count;
#if !WPF
                secondsPerPoint *= 2;
#endif
                int i = 0;
                foreach (ContentControl label in this.AdornmentsInfo.LabelPresenters)
                {
                    label.RenderTransform = new ScaleTransform() { ScaleX = 0, ScaleY = 0 };
                    label.RenderTransformOrigin = new Point(0.5, 0.5);
                    DoubleAnimation keyFrames1 = new DoubleAnimation()
                    {
                        From = 0.6,
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
                        From = 0.6,
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

        internal override void ActivateDragging(Point mousePos, object element)
        {
            try
            {
                if (previewCurrLine != null || PreviewSeries != null) return;
                var chartElement = element as FrameworkElement;
                isReversed = false;
                if (chartElement == null)
                    return;

                DraggingSegment = chartElement.Tag as LineSegment;
                base.ActivateDragging(mousePos, chartElement);
                if (SegmentIndex < 0) return;
                if (chartElement is Line && EnableSeriesDragging && DraggingSegment != null)
                {
                    var line = Segments[0].GetRenderedVisual() as Line;
                    if (pointCollection == null)
                    {
                        PreviewSeries = new Polyline { Opacity = 0.6, Stroke = line.Stroke, StrokeThickness = line.StrokeThickness };
                        SeriesPanel.Children.Add(PreviewSeries);
                        pointCollection = new PointCollection();
                        (PreviewSeries as Polyline).Points = pointCollection;
                    }
                    pointCollection.Clear();
                    pointCollection.Add(new Point(line.X1, line.Y1));
                    foreach (var segment in Segments)
                    {
                        line = segment.GetRenderedVisual() as Line;
                        pointCollection.Add(new Point(line.X2, line.Y2));
                    }

                    offsetPosition = initialPosition = IsActualTransposed ? mousePos.X : mousePos.Y;
                }
                else if (DraggingSegment != null || chartElement is Line ||
                         chartElement.DataContext is ChartAdornmentContainer)
                {
                    double segmentPosition = Area.ValueToPoint(ActualXAxis, SegmentIndex);
                    if (mousePos.X <= segmentPosition)
                        isReversed = true;
                    if (SegmentIndex == Segments.Count)
                        DraggingSegment = Segments[SegmentIndex - 1] as LineSegment;
                    else
                        DraggingSegment = Segments[SegmentIndex] as LineSegment;
                    isReversed = false;
                }
            }
            catch
            {
                ResetDraggingElements("Exception", true);
            }
        }

        internal override void UpdatePreivewSeriesDragging(Point mousePos)
        {
            if (IsActualTransposed)
            {
                double newValue = Area.PointToValue(ActualXAxis, new Point(mousePos.Y,mousePos.X));
                double baseValue = Area.PointToValue(ActualXAxis, new Point(0,initialPosition));
                DraggedValue = baseValue- newValue;
                var dragEvent = new XySeriesDragEventArgs { Delta = DraggedValue, BaseXValue = SegmentIndex };
                RaiseDragDelta(dragEvent);
                if (dragEvent.Cancel)
                {
                    ResetDraggingElements("Cancel", true);
                    return;
                }
                var offset = mousePos.X - offsetPosition;
                for (var i = 0; i < pointCollection.Count; i++)
                {
                    var x = pointCollection[i].X;
                    var y = pointCollection[i].Y;
                    pointCollection[i] = new Point(x + offset, y);
                }
                offsetPosition = mousePos.X;
                dragged = true;
                UpdateSeriesDragValueToolTip(mousePos, Segments[0].Interior, DraggedValue, 0, Area.ValueToPoint(ActualYAxis, YValues[0]));
            }
            else
            {
                double newValue = Area.PointToValue(ActualYAxis, mousePos);
                double baseValue = Area.PointToValue(ActualYAxis, new Point(0, initialPosition));
                DraggedValue = newValue - baseValue;
                var dragEvent = new XySeriesDragEventArgs { Delta = DraggedValue, BaseXValue = SegmentIndex };
                RaiseDragDelta(dragEvent);
                if (dragEvent.Cancel)
                {
                    ResetDraggingElements("Cancel", true);
                    return;
                }
                var offset = mousePos.Y - offsetPosition;
                for (var i = 0; i < pointCollection.Count; i++)
                {
                    var x = pointCollection[i].X;
                    var y = pointCollection[i].Y;
                    pointCollection[i] = new Point(x, y + offset);
                }
                offsetPosition = mousePos.Y;
                dragged = true;
                UpdateSeriesDragValueToolTip(mousePos, Segments[0].Interior, DraggedValue, YValues[0], pointCollection[0].X);
            }
        }

        internal override void UpdatePreviewSegemntDragging(Point mousePos)
        {
            try
            {
                var dragEvent = new XySegmentDragEventArgs { BaseYValue = YValues[SegmentIndex], NewYValue = Area.PointToValue(ActualYAxis, mousePos), Segment = DraggingSegment };
                RaiseDragDelta(dragEvent);
                if (dragEvent.Cancel)
                    return;
                var currentLine = DraggingSegment.GetRenderedVisual() as Line;
                double newHeight=0d;
                if (previewCurrLine == null)
                {
                    previewCurrLine = new Line { Opacity = 0.6, Stroke = currentLine.Stroke, StrokeThickness = currentLine.StrokeThickness };
                    SeriesPanel.Children.Add(previewCurrLine);
                    previewPreLine = new Line { Opacity = 0.6, Stroke = currentLine.Stroke, StrokeThickness = currentLine.StrokeThickness };
                    SeriesPanel.Children.Add(previewPreLine);
                }
                if (!this.IsActualTransposed)
                {
                    if (SegmentIndex == 0)
                    {
                        previewCurrLine.Y2 = currentLine.Y2;
                        previewCurrLine.Y1 = newHeight = mousePos.Y;
                    }
                    else if (SegmentIndex == Segments.Count)
                    {
                        previewCurrLine.Y2 = newHeight = mousePos.Y;
                        previewCurrLine.Y1 = currentLine.Y1;
                    }
                    else
                    {
                        Line nextLine = null;
                        if (!isReversed)
                        {
                            nextLine = Segments[SegmentIndex - 1].GetRenderedVisual() as Line;
                            previewCurrLine.Y1 = previewPreLine.Y2 = newHeight = mousePos.Y;
                            previewCurrLine.Y2 = currentLine.Y2;
                            previewPreLine.Y1 = nextLine.Y1;
                        }
                        else
                        {
                            nextLine = Segments[SegmentIndex].GetRenderedVisual() as Line;
                            previewCurrLine.Y1 = currentLine.Y1;
                            previewCurrLine.Y2 = previewPreLine.Y1 = newHeight = mousePos.Y;
                            previewPreLine.Y2 = nextLine.Y2;
                        }
                        previewPreLine.X2 = nextLine.X2;
                        previewPreLine.X1 = nextLine.X1;
                    }
                    previewCurrLine.X1 = currentLine.X1;
                    previewCurrLine.X2 = currentLine.X2;
                }
                else
                {
                    if (SegmentIndex == 0)
                    {
                        previewCurrLine.X2 = currentLine.X2;
                        previewCurrLine.X1 = newHeight = mousePos.X;
                    }
                    else if (SegmentIndex == Segments.Count)
                    {
                        previewCurrLine.X2 = newHeight = mousePos.X;
                        previewCurrLine.X1 = currentLine.X1;
                    }
                    else
                    {
                        Line nextLine = null;
                        if (!isReversed)
                        {
                            nextLine = Segments[SegmentIndex - 1].GetRenderedVisual() as Line;
                            previewCurrLine.X1 = previewPreLine.X2 = newHeight = mousePos.X;
                            previewCurrLine.X2 = currentLine.X2;
                            previewPreLine.X1 = nextLine.X1;
                        }
                        else
                        {
                            nextLine = Segments[SegmentIndex].GetRenderedVisual() as Line;
                            previewCurrLine.X1 = currentLine.X1;
                            previewCurrLine.X2 = previewPreLine.X1 = newHeight = mousePos.X;
                            previewPreLine.X2 = nextLine.X2;
                        }
                        previewPreLine.Y2 = nextLine.Y2;
                        previewPreLine.Y1 = nextLine.Y1;
                    }
                    previewCurrLine.Y1 = currentLine.Y1;
                    previewCurrLine.Y2 = currentLine.Y2;
                }
                

                dragged = true;
                DraggedValue = Area.PointToValue(ActualYAxis, mousePos);
                if (DraggingPointIndicator != null)
                {
                    var segmentHeight = DraggingPointIndicator.ActualHeight / 2;
                    Canvas.SetTop(DraggingPointIndicator, newHeight - segmentHeight);
                    UpdateSegmentDragValueToolTip(new Point(currentLine.X1, mousePos.Y), DraggingSegment, DraggedValue, segmentHeight);
                    return;
                }
                if (IsActualTransposed)
                    UpdateSegmentDragValueToolTip(new Point(mousePos.X, Area.ValueToPoint(ActualXAxis, SegmentIndex)), DraggingSegment, DraggedValue, 0);
                else
                    UpdateSegmentDragValueToolTip(new Point(Area.ValueToPoint(ActualXAxis, SegmentIndex), mousePos.Y), DraggingSegment, DraggedValue, 0);
            }
            catch
            {

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
                    if (PreviewSeries != null)
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
                        YValues[SegmentIndex] = DraggedValue;
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

        protected override void ResetDraggingElements(string reason, bool dragEndEvent)
        {
            if (SeriesPanel == null) return;
            base.ResetDraggingElements(reason, dragEndEvent);
            if (SeriesPanel.Children.Contains(previewPreLine))
            {
                SeriesPanel.Children.Remove(previewPreLine);
                SeriesPanel.Children.Remove(previewCurrLine);
            }
            if (SeriesPanel.Children.Contains(PreviewSeries))
            {
                (PreviewSeries as Polyline).Points.Clear();
                SeriesPanel.Children.Remove(PreviewSeries);
            }

            pointCollection = null;
            previewPreLine = null;
            previewCurrLine = null;
            DraggingSegment = null;
            PreviewSeries = null;
            dragged = false;
            DraggedValue = 0;
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