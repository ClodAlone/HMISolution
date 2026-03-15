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
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Animation;
#else
using Windows.UI;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// BarSeries represents its datapoint using a set of horizontal rectangles.
    /// </summary>
    /// <seealso cref="BarSegment"/>
    /// <seealso cref="ColumnSeries"/>
    /// <seealso cref="StackingBarSeries"/>
    /// <seealso cref="StackingColumnSeries"/> 
    [ClassReference(IsReviewed = false)]
    public class BarSeries : XySegmentDraggingBase, ISegmentSelectable
    {
        #region fields

        double initialWidth;
        private Rectangle previewRect;
        double delta = 0d;
        private bool dragged;

        #endregion

        #region ctor
        public BarSeries()
        {
            IsActualTransposed = true;
        }
        #endregion

        #region Properties

        protected internal override bool IsSideBySide
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets or Sets the selection brush
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Brush SegmentSelectionBrush
        {
            get { return (Brush)GetValue(SegmentSelectionBrushProperty); }
            set { SetValue(SegmentSelectionBrushProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for SegmentSelectionBrush.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SegmentSelectionBrushProperty =
            DependencyProperty.Register("SegmentSelectionBrush", typeof(Brush), typeof(BarSeries), new PropertyMetadata(null));

        #endregion

        #region methods

        internal override void OnTransposeChanged(bool val)
        {
            IsActualTransposed = !val;
        }

        /// <summary>
        /// Creates the segments of BarSeries.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public override void CreateSegments()
        {
            double x1, x2, y1, y2;
            List<double> xValues = GetXValues();
            DoubleRange sbsInfo = this.GetSideBySideInfo(this);            
            double median = sbsInfo.Delta / 2;
            if (this.ActualXValues != null)
            {
                ClearUnUsedSegments(DataCount);
                ClearUnUsedAdornments(DataCount);

                for (int i = 0; i < DataCount; i++)
                {
                    x1 = xValues[i] + sbsInfo.Start;
                    x2 = xValues[i] + sbsInfo.End;
                    y1 = YValues[i];
                    y2 = 0;
                    if (i < Segments.Count)
                    {
                        (Segments[i]).SetData(x1, y1, x2, y2);
                        (Segments[i] as BarSegment).XData = xValues[i];
                        (Segments[i] as BarSegment).YData = YValues[i];
                        (Segments[i] as BarSegment).Item = ActualData[i];
                    }
                    else
                    {
                        BarSegment barSegment = new BarSegment(x1, y1, x2, y2, this) { Item = ActualData[i]};
                        barSegment.XData = xValues[i];
                        barSegment.YData = YValues[i];
                        Segments.Add(barSegment);
                    }
                    if (AdornmentsInfo != null)
                    {
                        if (this.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
                            AddColumnAdornments(xValues[i], YValues[i], x1, y1, i, median);
                        else if (this.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom)
                            AddColumnAdornments(xValues[i], YValues[i], x1, y2, i, median);
                        else
                            AddColumnAdornments(xValues[i], YValues[i], x1, y1 + (y2 - y1) / 2, i, median);
                    }
                }
            }

            if (ShowEmptyPoints)
                UpdateEmptyPointSegments(xValues);
        }


        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new BarSeries() { SegmentSelectionBrush = this.SegmentSelectionBrush });
        }

        internal override bool GetAnimationIsActive()
        {
            return sb != null && sb.GetCurrentState() == ClockState.Active;
        }

        private Storyboard sb;

        internal override void Animate()
        {
            int i = 0;
            if (sb != null)
                sb.Stop();
            sb = new Storyboard();
            string path = IsActualTransposed ? "(UIElement.RenderTransform).(ScaleTransform.ScaleX)" : "(UIElement.RenderTransform).(ScaleTransform.ScaleY)";
            string adornTransPath = IsActualTransposed ? "(UIElement.RenderTransform).(TranslateTransform.X)" : "(UIElement.RenderTransform).(TranslateTransform.Y)";
            foreach (BarSegment segment in Segments)
            {
                var element = (FrameworkElement)segment.GetRenderedVisual();
                double elementSize = IsActualTransposed ? segment.segmentSize.Width : segment.segmentSize.Height;
                if (!double.IsNaN(elementSize))
                {
                    element.RenderTransform = new ScaleTransform();
                    if (YValues[i] < 0 && IsActualTransposed)
                       element.RenderTransformOrigin = new Point(1, 1);
                    else if (YValues[i] > 0 && !IsActualTransposed)
                       element.RenderTransformOrigin = new Point(1, 1);
                    
                    DoubleAnimationUsingKeyFrames keyFrames1 = new DoubleAnimationUsingKeyFrames();
                    SplineDoubleKeyFrame keyFrame1 = new SplineDoubleKeyFrame();
                    keyFrame1.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0));
                    keyFrame1.Value = 0;
                    keyFrames1.KeyFrames.Add(keyFrame1);
                    keyFrame1 = new SplineDoubleKeyFrame();
                    keyFrame1.KeyTime = KeyTime.FromTimeSpan(AnimationDuration);

                    KeySpline keySpline1 = new KeySpline();
                    keySpline1.ControlPoint1 = new Point(0.64, 0.84);
                    keySpline1.ControlPoint2 = new Point(0.67, 0.95);
                    keyFrame1.KeySpline = keySpline1;
                    keyFrames1.KeyFrames.Add(keyFrame1);
                    keyFrame1.Value = 1;
#if !WINDOWS_PHONE
                    keyFrames1.EnableDependentAnimation = true;
                    Storyboard.SetTargetProperty(keyFrames1, path);
#else
                    Storyboard.SetTargetProperty(keyFrames1, new PropertyPath(path));
#endif
                    Storyboard.SetTarget(keyFrames1, element);
                    sb.Children.Add(keyFrames1);
                    if (this.AdornmentsInfo != null && AdornmentsInfo.ShowLabel)
                    {
                        FrameworkElement label = this.AdornmentsInfo.LabelPresenters[i];
                        label.RenderTransform = new TranslateTransform() { };
                        keyFrames1 = new DoubleAnimationUsingKeyFrames();
                        keyFrame1 = new SplineDoubleKeyFrame();
                        keyFrame1.KeyTime =
                            KeyTime.FromTimeSpan(TimeSpan.FromSeconds((AnimationDuration.TotalSeconds * 80) / 100));
                        keyFrame1.Value = (YValues[i] > 0) ? -(elementSize * 10) / 100 : (elementSize * 10) / 100;
                        keyFrames1.KeyFrames.Add(keyFrame1);
                        keyFrame1 = new SplineDoubleKeyFrame();
                        keyFrame1.KeyTime = KeyTime.FromTimeSpan(AnimationDuration);

                        keySpline1 = new KeySpline();
                        keySpline1.ControlPoint1 = new Point(0.64, 0.84);
                        keySpline1.ControlPoint2 = new Point(0.67, 0.95);
                        keyFrame1.KeySpline = keySpline1;
                        keyFrames1.KeyFrames.Add(keyFrame1);
                        keyFrame1.Value = 0;
#if !WINDOWS_PHONE
                        keyFrames1.EnableDependentAnimation = true;
                        Storyboard.SetTargetProperty(keyFrames1, adornTransPath);
#else
                        Storyboard.SetTargetProperty(keyFrames1,
                                                     new PropertyPath(
                                                         adornTransPath));
#endif
                        Storyboard.SetTarget(keyFrames1, label);
                        sb.Children.Add(keyFrames1);
                        label.Opacity = 0;

                        DoubleAnimation animation = new DoubleAnimation()
                        {
                            From = 0,
                            To = 1,
                            Duration = TimeSpan.FromSeconds((AnimationDuration.TotalSeconds * 20) / 100),
                            BeginTime = TimeSpan.FromSeconds((AnimationDuration.TotalSeconds * 80) / 100)
                        };

                        Storyboard.SetTarget(animation, label);
#if WINDOWS_PHONE
                        Storyboard.SetTargetProperty(animation, new PropertyPath(UIElement.OpacityProperty));
#else
                        Storyboard.SetTargetProperty(animation, "(UIElement.Opacity)");
#endif
                        sb.Children.Add(animation);
                    }
                   
                    i++;
                }
            }
            sb.Begin();
        }

        protected override void ResetDraggingElements(string reason, bool dragEndEvent)
        {
            dragged = false;
            base.ResetDraggingElements(reason, dragEndEvent);
            if (previewRect == null) return;
            SeriesPanel.Children.Remove(previewRect);
            previewRect = null;
        }

        private void UpdateDraggedSource()
        {
            try
            {
                DraggedValue = GetSnapToPoint(DraggedValue);
                var baseValue = YValues[SegmentIndex];
                var dragPreviewEnd = new XyPreviewEndEventArgs { BaseYValue = baseValue, NewYValue = DraggedValue };
                RaisePreviewEnd(dragPreviewEnd);
                if (dragPreviewEnd.Cancel)
                {
                    ResetDraggingElements("", false);
                    return;
                }
                YValues[SegmentIndex] = DraggedValue;
                if (UpdateSource && !IsSortData)
                    UpdateUnderLayingModel(YBindingPath, SegmentIndex, DraggedValue);
                UpdateArea();
                dragged = false;
                RaiseDragEnd(new ChartDragEndEventArgs { BaseYValue = baseValue, NewYValue = DraggedValue });
            }
            catch
            {
                ResetDraggingElements("Exception", true);
            }
        }

        internal override void ActivateDragging(Point mousePos, object element)
        {
            try
            {
                if (previewRect != null) return;
                var rectangle = element as Rectangle;
                if (rectangle == null) return;
                if (!(rectangle.Tag is BarSegment)) return;
                base.ActivateDragging(mousePos, element);
                if (SegmentIndex < 0) return;
                initialWidth = Canvas.GetLeft(rectangle);
                var brush = rectangle.Fill as SolidColorBrush;
                previewRect = new Rectangle
                {
                    Fill = brush != null ?
                    new SolidColorBrush(Color.FromArgb(brush.Color.A, (byte)(brush.Color.R * 0.6),
                                                       (byte)(brush.Color.G * 0.6), (byte)(brush.Color.B * 0.6))) : rectangle.Fill,
                    Opacity = 0.5,
                    Stroke = rectangle.Stroke,
                    StrokeThickness = rectangle.StrokeThickness
                };
                previewRect.SetValue(Canvas.TopProperty, Canvas.GetTop(rectangle));
                previewRect.SetValue(Canvas.LeftProperty, initialWidth);
                previewRect.Height = rectangle.ActualHeight;
                previewRect.Width = rectangle.ActualWidth;
                SeriesPanel.Children.Add(previewRect);
            }
            catch
            {
                ResetDraggingElements("Exception", true);
            }
        }

        private void SegmentPreview(Point mousePos)
        {
            try
            {
                if (previewRect == null) return;
                DraggedValue = Area.PointToValue(ActualYAxis, mousePos);
                var dragEvent = new XySegmentDragEventArgs { NewYValue = DraggedValue, BaseYValue = YValues[SegmentIndex], Segment = Segments[SegmentIndex], Delta = delta };
                RaiseDragDelta(dragEvent);
                if (dragEvent.Cancel)
                {
                    ResetDraggingElements("Cancel", true);
                    return;
                }
                if (!this.IsActualTransposed)
                {
                    double currPos = mousePos.Y;

                    double movingOffset = Canvas.GetTop(previewRect) - currPos;
                    if (currPos > Area.ValueToPoint(ActualYAxis, 0))
                    {
                        previewRect.Height = Math.Abs(movingOffset);
                    }
                    else
                    {
                        previewRect.SetValue(Canvas.TopProperty, currPos);
                        previewRect.Height += movingOffset;
                    }
                    delta = DraggedValue - delta;
                    double originalPos = Canvas.GetTop(Segments[SegmentIndex].GetRenderedVisual() as Rectangle);
                    double posY = originalPos > mousePos.Y ? mousePos.Y : originalPos + 20;
                    UpdateSegmentDragValueToolTip(new Point(Canvas.GetLeft(previewRect) + previewRect.Width / 2, posY), Segments[SegmentIndex], DraggedValue, previewRect.Width / 2);
                }
                else
                {
                    var currPos = mousePos.X;

                    var movingOffset = Canvas.GetLeft(previewRect) - currPos;
                    if (currPos > Area.ValueToPoint(ActualYAxis, 0))
                    {
                        previewRect.SetValue(Canvas.LeftProperty, Canvas.GetLeft(previewRect));
                        previewRect.Width = Math.Abs(movingOffset);
                    }
                    else
                    {
                        previewRect.SetValue(Canvas.LeftProperty, currPos);
                        previewRect.Width += movingOffset;
                    }
                    delta = DraggedValue - delta;

                    var rect = Segments[SegmentIndex].GetRenderedVisual() as Rectangle;
                    double originalPos = Canvas.GetLeft(rect) + rect.Width;
                    double posX = originalPos < mousePos.X ? mousePos.X : originalPos;

                    UpdateSegmentDragValueToolTip(new Point(posX, Canvas.GetTop(previewRect) + previewRect.Height / 2), Segments[SegmentIndex], DraggedValue, previewRect.Height / 2);
                }
                ResetDragSpliter();
                dragged = true;
            }
            catch
            {
                ResetDraggingElements("Exception", true);
            }
        }

        protected override void OnChartDragStart(Point mousePos, object originalSource)
        {
            ActivateDragging(mousePos, originalSource);
        }

        protected override void OnChartDragDelta(Point mousePos, object originalSource)
        {
            SegmentPreview(mousePos);
        }

        protected override void OnChartDragEnd(Point mousePos, object originalSource)
        {
            if (dragged)
                UpdateDraggedSource();
            ResetDraggingElements("", false);
        }

        protected override void OnChartDragEntered(Point mousePos, object originalSource)
        {
            var rect = originalSource as Rectangle;
            if (rect != null && rect.Tag is BarSegment)
            {
                if (IsActualTransposed)
                    UpdateDragSpliter(rect, YValues[Segments.IndexOf(rect.Tag as ChartSegment)] < 0 ? "Left" : "Right");
                else
                    UpdateDragSpliter(rect, YValues[Segments.IndexOf(rect.Tag as ChartSegment)] < 0 ? "Bottom" : "Top");
            }
            base.OnChartDragEntered(mousePos, originalSource);
        }

        #endregion
    }
}
