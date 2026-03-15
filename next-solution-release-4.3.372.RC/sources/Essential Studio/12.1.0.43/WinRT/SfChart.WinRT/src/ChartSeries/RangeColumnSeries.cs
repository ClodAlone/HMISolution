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
#if WINDOWS_PHONE
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Controls;
#else
using Windows.UI;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Controls;
using Windows.Foundation;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    ///RangeColumnSeries displays data points as a set of vertical bars of varying heights,starting at different points within a area of<see cref="SfChart"/>.
    ///</summary>
    ///<seealso cref="RangeColumnSegment"/>
    ///<seealso cref="RangeAreaSeries"/>
    ///<seealso cref="ColumnSeries"/>
    ///<seealso cref="BarSeries"/>
    [ClassReference(IsReviewed = false)]
    public class RangeColumnSeries : RangeSegmentDraggingBase, ISegmentSelectable
    {
        #region fields

        double initialHeight, initialValue;
        int draggingMode;
        private Rectangle previewRect;
        private RangeColumnSegment selectedSegment;
        private bool dragged;

        #endregion

        #region properties

        internal override bool IsMultipleYPathRequired
        {
            get
            {
                return true;
            }
        }

        protected internal override bool IsSideBySide
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets or Sets selection brush
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
            DependencyProperty.Register("SegmentSelectionBrush", typeof(Brush), typeof(RangeColumnSeries), new PropertyMetadata(null));

       
        #endregion

        #region constructor

        #endregion

        #region methods

        /// <summary>
        /// Creates the segments of RangeColumn Series.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public override void CreateSegments()
        {
            double x1, x2, y1, y2;
            List<double> xValues = GetXValues();

            if (xValues != null)
            {
                if (Segments.Count > this.DataCount)
                {
                    ClearUnUsedSegments(this.DataCount);
                }

                if (AdornmentsInfo != null)
                {
                    if (AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                        ClearUnUsedAdornments(this.DataCount * 2);
                    else
                        ClearUnUsedAdornments(this.DataCount);
                }

                DoubleRange sbsInfo = this.GetSideBySideInfo(this);
                double center = sbsInfo.Median;
                xValues = (from val in xValues select (val + center)).ToList<double>();
                for (int i = 0; i < this.DataCount; i++)
                {
                    x1 = xValues[i] + sbsInfo.Start;
                    x2 = xValues[i] + sbsInfo.End;
                    y1 = HighValues[i];
                    y2 = LowValues[i];
                    if (i < Segments.Count)
                    {
                        (Segments[i]).SetData(x1, y1, x2, y2);
                    }
                    else
                    {
                        RangeColumnSegment rangeColumn = new RangeColumnSegment(x1, y1, x2, y2, this, ActualData[i]);
                        rangeColumn.High = HighValues[i];
                        rangeColumn.Low = LowValues[i];
                        Segments.Add(rangeColumn);
                    }

                    if (AdornmentsInfo != null)
                        AddAdornments(xValues[i],x1, y1, y2, i,Median:sbsInfo.Delta/2);
                }
                if (ShowEmptyPoints)
                    UpdateEmptyPointSegments(xValues);
            }
           
        }

        private void AddAdornments(double xVal,double x, double high, double low, int i,double Median)
        {
            double adornX = 0d, adornHigh = 0d, adornLow = 0d;
            if (this.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
            {
                adornX = x + Median;
                adornHigh = high;
                if (i < Adornments.Count)
                {
                    Adornments[i].SetData(xVal, adornHigh,adornX, adornHigh);
                }
                else
                {
                    Adornments.Add(this.CreateAdornment(this,xVal, adornHigh, adornX, adornHigh));
                }
                Adornments[i].Item = ActualData[i];
            }
            else if (this.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom)
            {
                adornX = x + Median;
                adornLow = low;
                if (i < Adornments.Count)
                {
                    Adornments[i].SetData(xVal, adornLow, adornX, adornLow);
                }
                else
                {
                    Adornments.Add(this.CreateAdornment(this, xVal, adornLow, adornX, adornLow));
                }
                Adornments[i].Item = ActualData[i];
            }
            else
            {
                adornX = x + Median;
                adornHigh = high;
                adornLow = low;
                if (i < Adornments.Count/2)
                {
                    int j = 2*i;
                    Adornments[j++].SetData(xVal, adornHigh, adornX, adornHigh);
                    Adornments[j].SetData(xVal, adornLow, adornX, adornLow);
                }
                else
                {
                    Adornments.Add(this.CreateAdornment(this, xVal, adornHigh, adornX, adornHigh));
                    Adornments.Add(this.CreateAdornment(this, xVal, adornLow, adornX, adornLow));
                }
                int k = 2 * i;
                Adornments[k++].Item = ActualData[i];
                Adornments[k].Item = ActualData[i];
            }
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new RangeColumnSeries() { SegmentSelectionBrush = this.SegmentSelectionBrush });
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
            foreach (ChartSegment segment in Segments)
            {
                var element = (FrameworkElement)segment.GetRenderedVisual();
                element.RenderTransform = new ScaleTransform();
                double elementSize = IsActualTransposed ? ((ColumnSegment)segment).Width : ((ColumnSegment)segment).Height;
                if (!double.IsNaN(elementSize))
                {
                    double canvasPos = IsActualTransposed ? Canvas.GetLeft(element) : Canvas.GetTop(element);
                    element.RenderTransformOrigin = new Point(0.5, 0.5);
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
                        for (int j = this.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom ? 0 : 1; j < 2; j++)
                        {
                            FrameworkElement label = this.AdornmentsInfo.LabelPresenters[i];
                            label.RenderTransform = new TranslateTransform() { };
                            keyFrames1 = new DoubleAnimationUsingKeyFrames();
                            keyFrame1 = new SplineDoubleKeyFrame();
                            keyFrame1.KeyTime =
                                KeyTime.FromTimeSpan(TimeSpan.FromSeconds((AnimationDuration.TotalSeconds * 80) / 100));
                            if (this.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom)
                                keyFrame1.Value = -(elementSize * 10) / 100;
                            else if (this.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
                                keyFrame1.Value = (elementSize * 10) / 100;
                            else
                                keyFrame1.Value = i % 2 == 0 ? (elementSize * 10) / 100 : -(elementSize * 10) / 100;
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
                            i++;
                        }
                    }
                   
                }
            }
            sb.Begin();
        }

        private int GetSegmentMousePosition(RangeColumnSegment rangeColumnSegment, Point mousePos)
        {
            var high = rangeColumnSegment.High;
            var low = rangeColumnSegment.Low;
            var currentValue = Area.PointToValue(ActualYAxis, mousePos);
            var diffPercentValue = (Math.Abs(high - low) * 25 / 100);
            if (low < high)
            {
                var finalHigh = high - diffPercentValue;
                var finalLow = low + diffPercentValue;

                if (currentValue > finalHigh && high > currentValue)
                    return 1;
                if (currentValue < finalLow && currentValue > low)
                    return 2;
                return 3;
            }
            else
            {
                var finalHigh = high + diffPercentValue;
                var finalLow = low - diffPercentValue;

                if (currentValue < finalHigh && high < currentValue)
                    return 1;
                if (currentValue > finalLow && currentValue < low)
                    return 2;
                return 3;
            }
        }

        private void SegmentPreview(Point mousePos)
        {
            try
            {
                if (previewRect == null) return;
                DraggedValue = Area.PointToValue(ActualYAxis, mousePos);
                var currPos = IsActualTransposed ? mousePos.X : mousePos.Y;
                var selectedRect = selectedSegment.GetRenderedVisual() as Rectangle;
                double segmentLeft = 0d, segmentTop = 0d, segmentWidth = 0d, newHigh = double.NaN, newLow = double.NaN;
                ResetDragSpliter();
                dragged = true;
                if (IsActualTransposed)
                {
                    switch (draggingMode)
                    {
                        case 1:
                            {
                                segmentWidth = Canvas.GetLeft(selectedRect);
                                var movingOffset = segmentWidth - currPos;
                                if (currPos < segmentWidth)
                                {
                                    previewRect.SetValue(Canvas.LeftProperty, segmentWidth - movingOffset);
                                    previewRect.Width = Math.Abs(movingOffset);
                                }
                                else
                                {
                                    previewRect.SetValue(Canvas.LeftProperty, segmentWidth);
                                    previewRect.Width = Math.Abs(movingOffset);
                                }
                                newHigh = DraggedValue;
                                newLow = double.NaN;
                                UpdateSegmentDragValueToolTipHigh(new Point(mousePos.X, Canvas.GetTop(previewRect) + previewRect.Height / 2), Segments[SegmentIndex], DraggedValue, previewRect.Height / 2);
                            }
                            break;
                        case 2:
                            {
                                if (selectedRect != null)
                                    segmentWidth = Canvas.GetLeft(selectedRect) + selectedRect.ActualWidth;
                                var movingOffset = segmentWidth - currPos;
                                if (currPos < segmentWidth)
                                {
                                    previewRect.SetValue(Canvas.LeftProperty, segmentWidth - movingOffset);
                                    previewRect.Width = Math.Abs(movingOffset);
                                }
                                else
                                {
                                    previewRect.SetValue(Canvas.LeftProperty, segmentWidth);
                                    previewRect.Width = Math.Abs(movingOffset);
                                }
                                newLow = DraggedValue;
                                newHigh = double.NaN;
                                UpdateSegmentDragValueToolTipLow(new Point(mousePos.X, Canvas.GetTop(previewRect) + previewRect.Height / 2), Segments[SegmentIndex], DraggedValue);
                            }
                            break;
                        case 3:
                            {
                                segmentLeft = Canvas.GetLeft(previewRect);
                                var movingOffset = mousePos.X- initialHeight;
                                previewRect.SetValue(Canvas.LeftProperty, segmentLeft + movingOffset);
                                initialHeight = mousePos.X;
                                newHigh = HighValues[SegmentIndex] + DraggedValue - initialValue;
                                newLow = LowValues[SegmentIndex] + DraggedValue - initialValue;
                                UpdateSegmentDragValueToolTipHigh(new Point(Canvas.GetLeft(previewRect) + previewRect.Width, Canvas.GetTop((previewRect)) + previewRect.Height / 2), Segments[SegmentIndex], newHigh, previewRect.Width / 2);
                                UpdateSegmentDragValueToolTipLow(new Point(Canvas.GetLeft(previewRect)  , Canvas.GetTop(previewRect) + previewRect.Height/2), Segments[SegmentIndex], newLow);
                            }
                            break;
                    }
                }
                else
                {
                    switch (draggingMode)
                    {
                        case 1:
                            {
                                if (selectedRect != null)
                                    segmentTop = Canvas.GetTop(selectedRect) + selectedRect.ActualHeight;
                                var movingOffset = segmentTop - currPos;
                                if (currPos < segmentTop)
                                {
                                    previewRect.SetValue(Canvas.TopProperty, segmentTop - movingOffset);
                                    previewRect.Height = Math.Abs(movingOffset);
                                }
                                else
                                {
                                    previewRect.SetValue(Canvas.TopProperty, segmentTop);
                                    previewRect.Height = Math.Abs(movingOffset);
                                }
                                newHigh = DraggedValue;
                                newLow = double.NaN;
                                UpdateSegmentDragValueToolTipHigh(new Point(Canvas.GetLeft(previewRect) + previewRect.Width / 2, mousePos.Y), Segments[SegmentIndex], DraggedValue, previewRect.Width / 2);
                            }
                            break;
                        case 2:
                            {
                                segmentTop = Canvas.GetTop(selectedRect);
                                var movingOffset = segmentTop - currPos;
                                if (currPos < segmentTop)
                                {
                                    previewRect.SetValue(Canvas.TopProperty, segmentTop - movingOffset);
                                    previewRect.Height = Math.Abs(movingOffset);
                                }
                                else
                                {
                                    previewRect.SetValue(Canvas.TopProperty, segmentTop);
                                    previewRect.Height = Math.Abs(movingOffset);
                                }
                                newLow = DraggedValue;
                                newHigh = double.NaN;
                                UpdateSegmentDragValueToolTipLow(new Point(Canvas.GetLeft(previewRect) + previewRect.Width / 2, mousePos.Y), Segments[SegmentIndex], DraggedValue);
                            }
                            break;
                        case 3:
                            {
                                segmentTop = Canvas.GetTop(previewRect);
                                var movingOffset = initialHeight - mousePos.Y;
                                previewRect.SetValue(Canvas.TopProperty, segmentTop - movingOffset);
                                initialHeight = mousePos.Y;
                                newHigh = HighValues[SegmentIndex] + DraggedValue - initialValue;
                                newLow = LowValues[SegmentIndex] + DraggedValue - initialValue;
                                UpdateSegmentDragValueToolTipHigh(new Point(Canvas.GetLeft(previewRect) + previewRect.Width / 2, Canvas.GetTop((previewRect))), Segments[SegmentIndex], newHigh, previewRect.Width / 2);
                                UpdateSegmentDragValueToolTipLow(new Point(Canvas.GetLeft(previewRect) + previewRect.Width / 2, Canvas.GetTop(previewRect) + previewRect.Height), Segments[SegmentIndex], newLow);
                            }
                            break;
                    }
                }
                var dragEvent = new RangeDragEventArgs { NewHighValue = newHigh, NewLowValue = newLow, BaseHighValue = HighValues[SegmentIndex], BaseLowValue = LowValues[SegmentIndex] };
                RaiseDragDelta(dragEvent);
                if (dragEvent.Cancel)
                    ResetDraggingElements("Cancel", true);
            }
            catch
            {
                ResetDraggingElements("Exception", true);
            }
        }

        protected override void OnChartDragStart(Point mousePos, object originalSource)
        {
            if (EnableSegmentDragging)
                ActivateDragging(mousePos, originalSource);
            base.OnChartDragStart(mousePos, originalSource);
        }

        protected override void OnChartDragEnd(Point mousePos, object originalSource)
        {
            if (dragged)
                UpdateDraggedSource();
            ResetDraggingElements("", false);
            base.OnChartDragEnd(mousePos, originalSource);
        }

        protected override void OnChartDragEntered(Point mousePos, object originalSource)
        {
            if (originalSource is Shape && (originalSource as Shape).Tag is RangeColumnSegment)
            {
                UpdateDragSpliterHigh(originalSource as Rectangle);
                UpdateDragSpliterLow(originalSource as Rectangle);
            }
            base.OnChartDragEntered(mousePos, originalSource);
        }

        protected override void OnChartDragDelta(Point mousePos, object originalSource)
        {
            if (EnableSegmentDragging)
                SegmentPreview(mousePos);
            base.OnChartDragDelta(mousePos, originalSource);
        }

        private void ActivateDragging(Point mousePos, object element)
        {
            try
            {
                if (previewRect != null) return;
                var rectangle = element as Rectangle;
                if (rectangle == null || !EnableSegmentDragging) return;
                var rangeColumnSegment = rectangle.Tag as RangeColumnSegment;
                if (rangeColumnSegment == null) return;
                initialHeight = Canvas.GetTop(rectangle);
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
                previewRect.SetValue(Canvas.LeftProperty, Canvas.GetLeft(rectangle));
                previewRect.SetValue(Canvas.TopProperty, initialHeight);
                previewRect.Height = rectangle.ActualHeight;
                previewRect.Width = rectangle.ActualWidth;
                SeriesPanel.Children.Add(previewRect);
                SegmentIndex = Segments.IndexOf(rectangle.Tag as ChartSegment);
                draggingMode = GetSegmentMousePosition(rangeColumnSegment, mousePos);
                selectedSegment = rangeColumnSegment;
                initialHeight = IsActualTransposed ? mousePos.X : mousePos.Y;
                initialValue = Area.PointToValue(ActualYAxis, mousePos);
                var dragEventArgs = new ChartDragStartEventArgs
                {
                    BaseXValue = GetActualXValue(SegmentIndex)
                };
                RaiseDragStart(dragEventArgs);
                if (dragEventArgs.Cancel)
                    ResetDraggingElements("Cancel", true);
#if NETFX_CORE
                Focus(FocusState.Keyboard);
#elif SILVERLIGHT
            Focus();
#elif WPF
            Keyboard.Focus(this);
#endif
               UnHoldPanning(false);
            }
            catch
            {
                ResetDraggingElements("Exception", true);
            }
        }

        internal override void ResetDraggingElements(string reason, bool dragEndEvent)
        {
            base.ResetDraggingElements(reason, dragEndEvent);
            if (SeriesPanel.Children.Contains(previewRect))
                SeriesPanel.Children.Remove(previewRect);
            previewRect = null;
            draggingMode = -1;
        }

        private void UpdateDraggedSource()
        {
            try
            {
                double high = HighValues[SegmentIndex], low = LowValues[SegmentIndex];
                double baseHigh = high, baseLow = low;

                DraggedValue = GetSnapToPoint(DraggedValue);
                var offset = GetSnapToPoint(DraggedValue - initialValue);
                switch (draggingMode)
                {
                    case 1:
                        high = DraggedValue;
                        break;
                    case 2:
                        low = DraggedValue;
                        break;
                    case 3:
                        high = GetSnapToPoint(HighValues[SegmentIndex] + offset);
                        low = GetSnapToPoint(LowValues[SegmentIndex] + offset);
                        break;
                }
                var args = new RangeDragEventArgs { BaseHighValue = baseHigh, BaseLowValue = baseLow, NewHighValue = high, NewLowValue = low };
                RaisePreviewEnd(args);

                if (args.Cancel)
                {
                    ResetDraggingElements("", false);
                    return;
                }
                HighValues[SegmentIndex] = high;
                LowValues[SegmentIndex] = low;

                if (UpdateSource && !IsSortData)
                {
                    UpdateUnderLayingModel(Low, SegmentIndex, LowValues[SegmentIndex]);
                    UpdateUnderLayingModel(High, SegmentIndex, HighValues[SegmentIndex]);
                }
                dragged = false;
                UpdateArea();
                var dragEvent = new RangeDragEndEventArgs { BaseHighValue = baseHigh, BaseLowValue = baseLow, NewHighValue = high, NewLowValue = low };
                RaiseDragEnd(dragEvent);
            }
            catch
            {
                ResetDraggingElements("", false);
            }
        }

        #endregion

    }
}
