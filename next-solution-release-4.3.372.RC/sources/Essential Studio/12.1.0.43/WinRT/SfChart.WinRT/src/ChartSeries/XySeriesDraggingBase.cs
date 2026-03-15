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
using System.Reflection;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Collections;
#else
using Windows.Devices.Input;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

#endif

namespace Syncfusion.UI.Xaml.Charts
{
    public abstract class XySeriesDraggingBase : XySegmentDraggingBase
    {
        #region fields

        protected Storyboard EllipseAnimation { get; set; }
        internal Ellipse DraggingPointIndicator { get; set; }
        ContentControl animationEllips;
        internal UIElement PreviewSeries { get; set; }
        internal ChartSegment DraggingSegment { get; set; }
        #endregion

        #region property

        public bool EnableSeriesDragging
        {
            get { return (bool)GetValue(EnableSeriesDraggingProperty); }
            set { SetValue(EnableSeriesDraggingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableSeriesDragging.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableSeriesDraggingProperty =
            DependencyProperty.Register("EnableSeriesDragging", typeof(bool), typeof(XySeriesDraggingBase), new PropertyMetadata(false, OnEnableDraggingChanged));

        #endregion

        #region methods

        protected void UpdateUnderLayingModel(string path, IList<double> updatedDatas)
        {
#if WPF
            var enumerator = (ItemsSource as IEnumerable).GetEnumerator();
#else
            var enumerator = ItemsSource.GetEnumerator();
#endif

            if (enumerator.MoveNext())
            {
                var yPropertyInfo =
                    enumerator.Current.GetType().GetTypeInfo().GetDeclaredProperty(path);
                IPropertyAccessor yPropertyAccessor = null;
                if (yPropertyInfo != null)
                    yPropertyAccessor = FastReflectionCaches.PropertyAccessorCache.Get(yPropertyInfo);
                int i = 0;
                do
                {
                    yPropertyAccessor.SetValue(enumerator.Current, updatedDatas[i]);
                    i++;
                } while (enumerator.MoveNext());
            }
        }

        internal void UpdateSeriesDragValueToolTip(Point pos, Brush brush, double newValue, double baseValue, double offsetX)
        {
            if(!EnableDragTooltip)return;
            double start;
            if (Tooltip == null)
            {
                DragInfo = new ChartDragSeriesInfo();
                Tooltip = new ContentControl();
                Tooltip.Content = DragInfo;
                Tooltip.ContentTemplate = IsActualTransposed ? ChartDictionaries.GenericCommonDictionary["SeriesDragInfoHorizontal"] as DataTemplate : 
                    ChartDictionaries.GenericCommonDictionary["SeriesDragInfoVertical"] as DataTemplate;
                SeriesPanel.Children.Add(Tooltip);
            }
            if (IsActualTransposed)
            {
                double offset = 50;
                start = Area.ValueToLogPoint(ActualXAxis, baseValue);
                var end = Area.ValueToLogPoint(ActualXAxis, newValue + baseValue);
                ((ChartDragSeriesInfo)DragInfo).OffsetY = Tooltip.Width = Math.Abs(end- start);
                DragInfo.IsNegative = !(newValue < 0);
                DragInfo.Delta = newValue;
                DragInfo.Brush = brush;
                if(DragInfo.IsNegative)
                    Canvas.SetLeft(Tooltip, offsetX);
                else
                   Canvas.SetLeft(Tooltip, offsetX - Tooltip.Width);
                Canvas.SetTop(Tooltip, start-offset);
            }
            else
            {
                start = Area.ValueToLogPoint(ActualYAxis, baseValue);
                var end = Area.ValueToLogPoint(ActualYAxis, newValue + baseValue);
                ((ChartDragSeriesInfo)DragInfo).OffsetY = Tooltip.Height = Math.Abs(start - end);
                DragInfo.IsNegative = !(newValue > 0);
                DragInfo.Delta = newValue;
                DragInfo.Brush = brush;
                if (DragInfo.IsNegative)
                    Canvas.SetTop(Tooltip, start);
                else
                    Canvas.SetTop(Tooltip, start - Tooltip.Height);
                Canvas.SetLeft(Tooltip, offsetX);
            }
        }

        private static void OnEnableDraggingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == false)
                ((XySeriesDraggingBase) d).ResetDraggingElements("OnPropertyChanged", false);
        }

        internal void EllipseIdealAnimation(UIElement ellipse)
        {
            EllipseAnimation = new Storyboard();
#if NETFX_CORE
            var ellipseAnimation = new DoubleAnimationUsingKeyFrames { RepeatBehavior = new RepeatBehavior { Type = RepeatBehaviorType.Forever } };
            Storyboard.SetTargetProperty(ellipseAnimation, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
#else
            var ellipseAnimation = new DoubleAnimationUsingKeyFrames() { RepeatBehavior = System.Windows.Media.Animation.RepeatBehavior.Forever };
            Storyboard.SetTargetProperty(ellipseAnimation, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleX)"));
#endif
            Storyboard.SetTarget(ellipseAnimation, ellipse);

            ellipseAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0)), Value = 1 });
            ellipseAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 600)), Value = 3.6, EasingFunction = new CircleEase() });
            EllipseAnimation.Children.Add(ellipseAnimation);
#if NETFX_CORE
            ellipseAnimation = new DoubleAnimationUsingKeyFrames { RepeatBehavior = new RepeatBehavior { Type = RepeatBehaviorType.Forever } };
            Storyboard.SetTargetProperty(ellipseAnimation, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");
#else
            ellipseAnimation = new DoubleAnimationUsingKeyFrames() { RepeatBehavior = System.Windows.Media.Animation.RepeatBehavior.Forever };
            Storyboard.SetTargetProperty(ellipseAnimation, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleY)"));
#endif
            Storyboard.SetTarget(ellipseAnimation, ellipse);

            ellipseAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0)), Value = 1 });
            ellipseAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 600)), Value = 3.6, EasingFunction = new CircleEase() });
            EllipseAnimation.Children.Add(ellipseAnimation);
#if NETFX_CORE
            ellipseAnimation = new DoubleAnimationUsingKeyFrames { RepeatBehavior = new RepeatBehavior { Type = RepeatBehaviorType.Forever } };
            Storyboard.SetTargetProperty(ellipseAnimation, "(UIElement.Opacity)");
#else
            ellipseAnimation = new DoubleAnimationUsingKeyFrames() { RepeatBehavior = System.Windows.Media.Animation.RepeatBehavior.Forever };
            Storyboard.SetTargetProperty(ellipseAnimation, new PropertyPath("(UIElement.Opacity)"));
#endif
            Storyboard.SetTarget(ellipseAnimation, ellipse);

            ellipseAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0)), Value = 1 });
            ellipseAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 600)), Value = 0, EasingFunction = new CircleEase() });
            EllipseAnimation.Children.Add(ellipseAnimation);
        }

#if NETFX_CORE

        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            if ((EnableSegmentDragging || EnableSeriesDragging) && PreviewSeries == null && DraggingSegment == null && e.Pointer.PointerDeviceType == PointerDeviceType.Touch)
            {
                var mousePos = e.GetCurrentPoint(SeriesPanel).Position;
                var element = e.OriginalSource as FrameworkElement;
                if (element != null && element.Tag is ChartSegment)
                {
                    if (Math.Abs(mousePos.X - mousePos.X) < 20 && Math.Abs(mousePos.Y - mousePos.Y) < 20)
                        ActivateDragging(mousePos, e.OriginalSource);
                }
            }
            else if (EnableSeriesDragging)
            {
                OnChartDragDelta(e.GetCurrentPoint(SeriesPanel).Position, e.OriginalSource);
            }
            else
                base.OnPointerMoved(e);
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            if (EnableSeriesDragging)
            {
                SeriesPanel.CapturePointer(e.Pointer);
                OnChartDragStart(e.GetCurrentPoint(SeriesPanel).Position, e.OriginalSource);
            }
            else
                base.OnPointerPressed(e);
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            if (EnableSeriesDragging)
                OnChartDragEnd(e.GetCurrentPoint(SeriesPanel).Position, e.OriginalSource);
            else
                base.OnPointerReleased(e);
        }

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            if (EnableSeriesDragging)
                OnChartDragEntered(e.GetCurrentPoint(SeriesPanel).Position, e.OriginalSource);
            else
                base.OnPointerEntered(e);
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            if (EnableSeriesDragging)
                OnChartDragExited(e.GetCurrentPoint(SeriesPanel).Position, e.OriginalSource);
            else
                base.OnPointerExited(e);
        }

#endif

#if WPF || SILVERLIGHT_UNCOMMON

        protected override void OnMouseMove(MouseEventArgs e)
        {

            if (EnableSeriesDragging)
            {
                OnChartDragDelta(e.GetPosition(SeriesPanel), e.OriginalSource);
            }
            else
                base.OnMouseMove(e);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (EnableSeriesDragging)
            {
                SeriesPanel.CaptureMouse();
                OnChartDragStart(e.GetPosition(SeriesPanel), e.OriginalSource);
            }
            else
                base.OnMouseLeftButtonDown(e);
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            SeriesPanel.ReleaseMouseCapture();
            if (EnableSeriesDragging)
                OnChartDragEnd(e.GetPosition(SeriesPanel), e.OriginalSource);
            else
                base.OnMouseLeftButtonUp(e);
            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (EnableSeriesDragging)
                OnChartDragEntered(e.GetPosition(SeriesPanel), e.OriginalSource);
            else
                base.OnMouseEnter(e);
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (EnableSeriesDragging)
                OnChartDragExited(e.GetPosition(SeriesPanel), e.OriginalSource);
            else
                base.OnMouseLeave(e);
            base.OnMouseLeave(e);
        }

#endif

#if WINDOWS_PHONE7 || WINDOWS_PHONE8

        private Point initialPos;
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (EnableSeriesDragging)
            {
                initialPos = e.GetPosition(SeriesPanel);
                CaptureMouse();
                OnChartDragStart(initialPos, e.OriginalSource);
            }
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnManipulationCompleted(ManipulationCompletedEventArgs e)
        {
            if (EnableSeriesDragging)
            {
                ReleaseMouseCapture();
                OnChartDragEnd(new Point(initialPos.X + e.ManipulationOrigin.X, initialPos.Y + e.ManipulationOrigin.Y), e.OriginalSource);
            }
            else
                base.OnManipulationCompleted(e);
        }

        protected override void OnManipulationDelta(ManipulationDeltaEventArgs e)
        {
            if ((EnableSegmentDragging || EnableSeriesDragging) && PreviewSeries == null && DraggingSegment == null)
            {
                var mousePos = e.ManipulationOrigin;
                var element = e.OriginalSource as FrameworkElement;
                if (element != null && element.Tag is ChartSegment)
                {
                    if (Math.Abs(mousePos.X - mousePos.X) < 20 && Math.Abs(mousePos.Y - mousePos.Y) < 20)
                        ActivateDragging(mousePos, e.OriginalSource);
                }
            }
            else if (EnableSeriesDragging && (e.OriginalSource as FrameworkElement).Tag is ChartSegment)
            {
                OnChartDragDelta(e.ManipulationOrigin, e.OriginalSource);
            }
            else
                base.OnManipulationDelta(e);
        }
#endif

        private void AddSegmentIndicator()
        {
            DraggingPointIndicator = new Ellipse
            {
                Height = 15,
                Width = 15,
                Fill = Segments[SegmentIndex == 0 ? 0 : SegmentIndex - 1].Interior,
            };
            SeriesPanel.Children.Add(DraggingPointIndicator);
        }

        private void UpdatePreviewIndicatorPosition(Point mousePos)
        {
            if (PreviewSeries == null && DraggingSegment == null && EnableSegmentDragging)
            {
                double x, y, stackedValue, positionY;
                FindNearestChartPoint(mousePos, out x, out y, out stackedValue);
                if (double.IsNaN(y))
                    return;
                SegmentIndex  = (int)(IsIndexed ? x : ((IList<double>)ActualXValues).IndexOf(x));
               
                XySegmentEnterEventArgs args = new XySegmentEnterEventArgs
                {
                    XValue = GetActualXValue(SegmentIndex),
                    SegmentIndex = SegmentIndex,
                    CanDrag = true,
                    YValue = YValues[SegmentIndex]
                };
                RaiseDragEnter(args);
                if (!args.CanDrag) return;
                positionY = Area.ValueToLogPoint(ActualYAxis, y);
                var positionX = Area.ValueToLogPoint(ActualXAxis, x);

                if (AdornmentsInfo == null)
                {
                    if (DraggingPointIndicator == null)
                        AddSegmentIndicator();
                    if (this.IsActualTransposed)
                    {
                        Canvas.SetTop(DraggingPointIndicator, positionX - DraggingPointIndicator.Width / 2);
                        Canvas.SetLeft(DraggingPointIndicator, positionY - DraggingPointIndicator.Height / 2);
                    }
                    else
                    {
                        Canvas.SetLeft(DraggingPointIndicator, positionX - DraggingPointIndicator.Width / 2);
                        Canvas.SetTop(DraggingPointIndicator, positionY - DraggingPointIndicator.Height / 2);
                    }
                    DraggingPointIndicator.Tag = Math.Abs(Segments.Count - SegmentIndex) > 0
                        ? Segments[(int)SegmentIndex]
                        : Segments[(int)SegmentIndex - 1];
                    AddAnimationEllipse(
                        ChartDictionaries.GenericSymbolDictionary["AnimationEllipse"] as ControlTemplate,
                        DraggingPointIndicator.Height, DraggingPointIndicator.Width, positionX, positionY);
                }
                else if (AdornmentsInfo != null && AdornmentsInfo.Symbol != ChartSymbol.Custom)
                {
                    AddAnimationEllipse(
                        ChartDictionaries.GenericSymbolDictionary["Animation" + AdornmentsInfo.Symbol.ToString()] as
                            ControlTemplate, AdornmentsInfo.SymbolHeight, AdornmentsInfo.SymbolWidth, positionX,
                        positionY);
                }
            }
        }

        private void AddAnimationEllipse(ControlTemplate template, double height, double width, double left, double top)
        {
            if (animationEllips == null)
            {
                animationEllips = new ContentControl
                {
                    Background = Segments[SegmentIndex == 0 ? 0 : SegmentIndex - 1].Interior,
                    RenderTransform = new ScaleTransform(),
                    RenderTransformOrigin = new Point(0.5, 0.5),
                    DataContext = this,
                    Template = template,
                };
                EllipseIdealAnimation(animationEllips);
                SeriesPanel.Children.Add(animationEllips);
                animationEllips.Height = height -height / 2;
                animationEllips.Width = width - width / 2;
                EllipseAnimation.Begin();
            }
            if (this.IsActualTransposed)
            {
                Canvas.SetTop(animationEllips, left - animationEllips.Width / 2);
                Canvas.SetLeft(animationEllips, top - animationEllips.Height / 2);
            }
            else
            {
                Canvas.SetLeft(animationEllips, left - animationEllips.Width / 2);
                Canvas.SetTop(animationEllips, top - animationEllips.Height / 2);
            }
            
        }

        protected override void ResetDraggingElements(string reason, bool dragEndEvent)
        {
            ResetDraggingindicators();
            base.ResetDraggingElements(reason, dragEndEvent);
        }

        internal override void ActivateDragging(Point mousePos, object element)
        {
#if NETFX_CORE
            Focus(FocusState.Keyboard);
#elif SILVERLIGHT
            Focus();
#elif WPF
            Keyboard.Focus(this);
#endif
            KeyDown += CoreWindow_KeyDown;
            double x, y, stackedValue;
            FindNearestChartPoint(mousePos, out x, out y, out stackedValue);
            SegmentIndex = (int)(IsIndexed ? x : ((IList<double>)ActualXValues).IndexOf(x));

            var dragEventArgs = new ChartDragStartEventArgs { BaseXValue = GetActualXValue(SegmentIndex) };
            if (EmptyPointIndexes != null)
            {
                var emptyPointIndex = EmptyPointIndexes[0];
                for (var i = 0; i < emptyPointIndex.Count; i++)
                    if (SegmentIndex == emptyPointIndex[i])
                    {
                        dragEventArgs.EmptyPoint = true;
                        break;
                    }
            }
            RaiseDragStart(dragEventArgs);
            if (dragEventArgs.Cancel)
            {
                ResetDraggingElements("Cancel", true);
                SegmentIndex = -1;
                return;
            }

            var eumerator = Area.Behaviors.OfType<ChartZoomPanBehavior>();
            foreach (var behavior in eumerator)
            {
                behavior.InternalEnablePanning = false;
                behavior.InternalEnableSelectionZooming = false;
            }
        }

        protected override void OnChartDragDelta(Point mousePos, object originalSource)
        {
            if (PreviewSeries != null)
            {
                ResetDraggingindicators();
                UpdatePreivewSeriesDragging(mousePos);
            }
            else if (DraggingSegment != null)
            {
                ResetDraggingindicators();
                UpdatePreviewSegemntDragging(mousePos);
            }

        }

        protected override void OnChartDragEntered(Point mousePos, object originalSource)
        {
            if ((EnableSegmentDragging || EnableSeriesDragging) &&
                (((FrameworkElement) originalSource).Tag is ChartSegment ||
                 ((FrameworkElement) originalSource).DataContext is ChartAdornmentContainer))
                UpdatePreviewIndicatorPosition(mousePos);
            base.OnChartDragEntered(mousePos, originalSource);
        }

        private void ResetDraggingindicators()
        {
            if (DraggingPointIndicator != null)
            {
                SeriesPanel.Children.Remove(DraggingPointIndicator);
                DraggingPointIndicator = null;
            }
            if (animationEllips != null && SeriesPanel.Children.Contains(animationEllips))
            {
                SeriesPanel.Children.Remove(animationEllips);
                animationEllips = null;
            }
            DraggingPointIndicator = null;
        }

        protected override void OnChartDragExited(Point mousePos, object originalSource)
        {
            if (EnableSegmentDragging || EnableSeriesDragging)
                ResetDraggingindicators();
            base.OnChartDragExited(mousePos, originalSource);
        }

        internal virtual void UpdatePreivewSeriesDragging(Point mousePos)
        { 
        
        }

        internal virtual void UpdatePreviewSegemntDragging(Point mousePos)
        {

        }

        #endregion
    }

    public class XySeriesDragEventArgs : DragDelta
    {
        public object BaseXValue { get; set; }
    }

    public class ChartDragSeriesInfo : ChartDragPointinfo
    {

        private double offsetY;

        public double OffsetY
        {
            get { return offsetY; }
            set
            {
                offsetY = value;
                OnPropertyChanged("OffsetY");
            }
        }
    }

    public class XySegmentEnterEventArgs : EventArgs
    {
        public object XValue { get; set; }
        public object YValue { get; set; }
        public bool CanDrag { get; set; }
        public int SegmentIndex { get; set; }
    }
}
