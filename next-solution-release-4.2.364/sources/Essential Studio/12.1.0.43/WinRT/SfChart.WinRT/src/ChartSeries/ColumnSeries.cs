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
using System.Windows.Media.Animation;
using System.Windows.Controls;
#else
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Animation;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// ColumnSeries displays its data points using a set of vertical bars.
    /// </summary>
    /// <seealso cref="ColumnSegment"/>
    /// <seealso cref="BarSeries"/>
    /// <seealso cref="RangeColumnSeries"/>
    /// <seealso cref="StackingColumnSeries"/>
    [ClassReference(IsReviewed = false)]
    public class ColumnSeries : XySegmentDraggingBase, ISegmentSelectable
    {
        #region fields

        double initialHeight;
        private Rectangle previewRect;
        double delta = 0d;
        private bool dragged;

        #endregion

        #region ctor

       

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
        /// Gets or Sets selection brush.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Brush SegmentSelectionBrush
        {
            get { return (Brush)GetValue(SegmentSelectionBrushProperty); }
            set { SetValue(SegmentSelectionBrushProperty, value); }
        }

        
        /// <summary>
        ///  Using a DependencyProperty as the backing store for SegmentSelectionBrush.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SegmentSelectionBrushProperty =
            DependencyProperty.Register("SegmentSelectionBrush", typeof(Brush), typeof(ColumnSeries), new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets DataTemplate for column segment.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public DataTemplate CustomTemplate
        {
            get { return (DataTemplate)GetValue(CustomTemplateProperty); }
            set { SetValue(CustomTemplateProperty, value); }
        }

        
        /// <summary>
        ///  Using a DependencyProperty as the backing store for Template.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CustomTemplateProperty =
            DependencyProperty.Register("CustomTemplate", typeof(DataTemplate), typeof(ColumnSeries), new PropertyMetadata(null));


        #endregion

        #region methods

        /// <summary>
        /// Creates the segments of ColumnSeries.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public override void CreateSegments()
        {
            double x1, x2, y1, y2;
            List<double> xValues = GetXValues();
            double median = 0d;
            if (xValues != null)
            {
                ClearUnUsedSegments(this.DataCount);
                ClearUnUsedAdornments(this.DataCount);

                DoubleRange sbsInfo = this.GetSideBySideInfo(this);
                median = sbsInfo.Delta / 2;
                for (int i = 0; i < this.DataCount; i++)
                {
                    if (i < this.DataCount)
                    {
                        x1 = xValues[i] + sbsInfo.Start;
                        x2 = xValues[i] + sbsInfo.End;
                        y1 = YValues[i];
                        y2 = 0;
                        if (i < Segments.Count)
                        {
                            (Segments[i]).SetData(x1, y1, x2, y2);
                            (Segments[i] as ColumnSegment).XData = xValues[i];
                            (Segments[i] as ColumnSegment).YData = YValues[i];
                            (Segments[i] as ColumnSegment).Item = ActualData[i];
                        }
                        else
                        {
                            Segments.Add(new ColumnSegment(x1, y1, x2, y2, this) { XData = xValues[i], YData = YValues[i], Item = ActualData[i] });
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
        }

        private Storyboard sb;

        internal override bool GetAnimationIsActive()
        {
            return sb != null && sb.GetCurrentState() == ClockState.Active;
        }

        internal override void Animate()
        {
            int i = 0;
            if (sb != null)
                sb.Stop();
            sb = new Storyboard();
            string path = IsActualTransposed ? "(UIElement.RenderTransform).(ScaleTransform.ScaleX)" : "(UIElement.RenderTransform).(ScaleTransform.ScaleY)";
            string adornTransPath = IsActualTransposed ? "(UIElement.RenderTransform).(TranslateTransform.X)" : "(UIElement.RenderTransform).(TranslateTransform.Y)";
            foreach (ChartSegment segment in Segments)
            {
                double elementHeight =0d;
                var element = (FrameworkElement)segment.GetRenderedVisual();
                if (segment is EmptyPointSegment && !(segment as EmptyPointSegment).IsEmptySegmentInterior)
                    elementHeight = IsActualTransposed ? ((EmptyPointSegment)segment).EmptyPointSymbolWidth : ((EmptyPointSegment)segment).EmptyPointSymbolHeight;
                else
                   elementHeight = IsActualTransposed ? ((ColumnSegment)segment).Width : ((ColumnSegment)segment).Height;
                if (!double.IsNaN(elementHeight))
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
                        keyFrame1.Value = (YValues[i] > 0) ? (elementHeight * 10) / 100 : -(elementHeight * 10) / 100;
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

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new ColumnSeries() { CustomTemplate = this.CustomTemplate, SegmentSelectionBrush = this.SegmentSelectionBrush });
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

        internal override void ActivateDragging(Point mousePos, object element)
        {
            try
            {
                if (previewRect != null) return;
                var rectangle = element as Rectangle;
                if (rectangle == null) return;
                if (!(rectangle.Tag is ColumnSegment)) return;

                base.ActivateDragging(mousePos, element);
                if (SegmentIndex < 0) return;
                initialHeight = Canvas.GetTop(rectangle);
                var brush = rectangle.Fill as SolidColorBrush;
                previewRect = new Rectangle
                {
                    Fill = brush != null
                        ? new SolidColorBrush(Color.FromArgb(brush.Color.A, (byte) (brush.Color.R*0.6),
                            (byte) (brush.Color.G*0.6), (byte) (brush.Color.B*0.6)))
                        : rectangle.Fill,
                    Opacity = 0.5,
                    Stroke = rectangle.Stroke,
                    StrokeThickness = rectangle.StrokeThickness
                };
                previewRect.SetValue(Canvas.LeftProperty, Canvas.GetLeft(rectangle));
                previewRect.SetValue(Canvas.TopProperty, initialHeight);
                previewRect.Height = rectangle.ActualHeight;
                previewRect.Width = rectangle.ActualWidth;
                SeriesPanel.Children.Add(previewRect);
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
                DraggedValue = GetSnapToPoint(DraggedValue);
                var baseValue = YValues[SegmentIndex];
                var dragPreviewEnd = new XyPreviewEndEventArgs { BaseYValue = baseValue, NewYValue = DraggedValue };
                RaisePreviewEnd(dragPreviewEnd);
              
                if (dragPreviewEnd.Cancel)
                {
                    ResetDraggingElements("CaptureReleased", false);
                    return;
                }
                YValues[SegmentIndex] = DraggedValue;
                if (UpdateSource && !IsSortData) // we wont update the underlayingmodel if issort data is true
                    UpdateUnderLayingModel(YBindingPath, SegmentIndex, DraggedValue);
                UpdateArea();
                ResetDraggingElements("CaptureReleased", false);
                dragged = false;
                RaiseDragEnd(new ChartDragEndEventArgs { BaseYValue = baseValue , NewYValue = DraggedValue});
            }
            catch
            {
                ResetDraggingElements("CaptureReleased", true);
            }
        }

        protected override void ResetDraggingElements(string reason, bool dragEndEvent)
        {
            dragged = false;
            base.ResetDraggingElements(reason, dragEndEvent);
            if (previewRect == null) return;
            SeriesPanel.Children.Remove(previewRect);
            previewRect = null;
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
            if (EnableSegmentDragging)
            {
                var rect = originalSource as Rectangle;
                if (rect != null && rect.Tag is ColumnSegment)
                {
                    if (!IsActualTransposed)
                        UpdateDragSpliter(rect, YValues[Segments.IndexOf(rect.Tag as ChartSegment)] < 0 ? "Bottom" : "Top");
                    else
                        UpdateDragSpliter(rect, YValues[Segments.IndexOf(rect.Tag as ChartSegment)] < 0 ? "Left" : "Right");
                }
            }
            base.OnChartDragEntered(mousePos, originalSource);
        }

        #endregion
    }
}
