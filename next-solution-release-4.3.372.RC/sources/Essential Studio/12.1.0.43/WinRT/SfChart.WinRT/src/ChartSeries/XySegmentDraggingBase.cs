#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
#if WPF
using System.Data;
#endif
#if WINDOWS_PHONE
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Input;

#endif

namespace Syncfusion.UI.Xaml.Charts
{
    public abstract class XySegmentDraggingBase : XyDataSeries
    {
        #region fields

        DataTemplate oppRightTootip, normalTooltip, oppLeftTooltip;

        protected int SegmentIndex { get; set; }

        protected ContentControl DragSpliter { get; set; }

        protected double DraggedValue { get; set; }

        protected ContentControl Tooltip { get; set; }

        protected ChartDragPointinfo DragInfo { get; set; }

        #endregion

        #region events

        /// <summary>
        /// Occurs when [segment enter].
        /// </summary>
        public event EventHandler<XySegmentEnterEventArgs> SegmentEnter;

        /// <summary>
        /// Occurs when [drag start].
        /// </summary>
        public event EventHandler<ChartDragStartEventArgs> DragStart;

        /// <summary>
        /// Occurs when [drag delta].
        /// </summary>
        public event EventHandler<DragDelta> DragDelta;

        /// <summary>
        /// Occurs when [drag end].
        /// </summary>
        public event EventHandler<ChartDragEndEventArgs> DragEnd;

        /// <summary>
        /// Occurs when [preview drag end].
        /// </summary>
        public event EventHandler<XyPreviewEndEventArgs> PreviewDragEnd;

        #endregion

        #region properties

        public bool EnableDragTooltip
        {
            get { return (bool)GetValue(EnableDragTooltipProperty); }
            set { SetValue(EnableDragTooltipProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableDragTooltip.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableDragTooltipProperty =
            DependencyProperty.Register("EnableDragTooltip", typeof(bool), typeof(XySegmentDraggingBase), new PropertyMetadata(true));

        public DataTemplate DragTooltipTemplate
        {
            get { return (DataTemplate)GetValue(DragTooltipTemplateProperty); }
            set { SetValue(DragTooltipTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DragTooltipTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DragTooltipTemplateProperty =
            DependencyProperty.Register("DragTooltipTemplate", typeof(DataTemplate), typeof(XySegmentDraggingBase), new PropertyMetadata(null));

        public int RoundToDecimal
        {
            get { return (int)GetValue(RoundToDecimalProperty); }
            set { SetValue(RoundToDecimalProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RoundToDecimal.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RoundToDecimalProperty =
            DependencyProperty.Register("RoundToDecimal", typeof(int), typeof(XySegmentDraggingBase), new PropertyMetadata(0));

        public SnapToPoint SnapToPoint
        {
            get { return (SnapToPoint)GetValue(SnapToPointProperty); }
            set { SetValue(SnapToPointProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SnapToPoint.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SnapToPointProperty =
            DependencyProperty.Register("SnapToPoint", typeof(SnapToPoint), typeof(XySegmentDraggingBase), new PropertyMetadata(SnapToPoint.None));
        
        public bool EnableSegmentDragging
        {
            get { return (bool)GetValue(EnableSegmentDraggingProperty); }
            set { SetValue(EnableSegmentDraggingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableSegmentDragging.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableSegmentDraggingProperty =
            DependencyProperty.Register("EnableSegmentDragging", typeof(bool), typeof(XySegmentDraggingBase), new PropertyMetadata(false));

        public bool UpdateSource
        {
            get { return (bool)GetValue(UpdateSourceProperty); }
            set { SetValue(UpdateSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UpdateSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UpdateSourceProperty =
            DependencyProperty.Register("UpdateSource", typeof(bool), typeof(XySegmentDraggingBase), new PropertyMetadata(false));

#if WINDOWS_PHONE
        public ModifierKeys DragCancelKeyModifiers
        {
            get { return (ModifierKeys)GetValue(DragCancelKeyModifiersProperty); }
            set { SetValue(DragCancelKeyModifiersProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DragCancelKeyModifiers.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DragCancelKeyModifiersProperty =
            DependencyProperty.Register("DragCancelKeyModifiers", typeof(ModifierKeys), typeof(XySegmentDraggingBase), new PropertyMetadata(ModifierKeys.None));
#else
        public VirtualKeyModifiers DragCancelKeyModifiers
        {
            get { return (VirtualKeyModifiers)GetValue(DragCancelKeyModifiersProperty); }
            set { SetValue(DragCancelKeyModifiersProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DragCancelKeyModifiers.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DragCancelKeyModifiersProperty =
            DependencyProperty.Register("DragCancelKeyModifiers", typeof(VirtualKeyModifiers), typeof(XySegmentDraggingBase), new PropertyMetadata(VirtualKeyModifiers.None));
#endif
        #endregion

        #region methods

        /// <summary>
        /// Updates the under laying model.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="index">The index.</param>
        /// <param name="updatedData">The updated data.</param>
        protected void UpdateUnderLayingModel(string path, int index, object updatedData)
        {
#if WPF
            var enumerator = (ItemsSource is DataTable) ? (ItemsSource as DataTable).Rows.GetEnumerator() : (ItemsSource as IEnumerable).GetEnumerator();
#else
            var enumerator = ItemsSource.GetEnumerator();
#endif
            if (enumerator.MoveNext())
            {
                int i = 0;
                do
                {
                    if (i == index)
                    {
                        SetPropertyValue(enumerator.Current, path.Split('.'), updatedData);
                        break;
                    }
                    i++;
                } while (enumerator.MoveNext());
            }
        }

#if WINDOWS_PHONE
        internal void CoreWindow_KeyDown(object sender, KeyEventArgs e)
#else
        internal void CoreWindow_KeyDown(object sender, KeyRoutedEventArgs e)
#endif
        {
            var value = false;
#if WINDOWS_PHONE
            if (e.Key == Key.Escape && Keyboard.Modifiers == DragCancelKeyModifiers)
                value = true;

#elif NETFX_CORE
            if (e.Key == VirtualKey.Escape)
            {
                switch (DragCancelKeyModifiers)
                {
                    case VirtualKeyModifiers.Shift:
                        {
                            if (Window.Current.CoreWindow.GetAsyncKeyState(VirtualKey.Shift) ==
                                CoreVirtualKeyStates.Down)
                                value = true;
                        }
                        break;
                    case VirtualKeyModifiers.Menu:
                        {
                            if (Window.Current.CoreWindow.GetAsyncKeyState(VirtualKey.Menu) == CoreVirtualKeyStates.Down)
                                value = true;
                        }
                        break;
                    case VirtualKeyModifiers.Windows:
                        {
                            if (Window.Current.CoreWindow.GetAsyncKeyState(VirtualKey.LeftWindows) ==
                                CoreVirtualKeyStates.Down)
                                value = true;
                        }
                        break;
                    case VirtualKeyModifiers.Control:
                        {
                            if (Window.Current.CoreWindow.GetAsyncKeyState(VirtualKey.Control) ==
                                CoreVirtualKeyStates.Down)
                                value = true;
                        }
                        break;
                    case VirtualKeyModifiers.None:
                        value = true;
                        break;
                }
            }
#endif
            if (!value) return;
#if WPF
            Mouse.OverrideCursor = Cursors.Arrow;
#elif NETFX_CORE
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 1);
#endif
            ResetDraggingElements("EscapeKey", true);
        }

        /// <summary>
        /// Raises the drag start.
        /// </summary>
        /// <param name="args">The <see cref="ChartDragStartEventArgs"/> instance containing the event data.</param>
        protected void RaiseDragStart(ChartDragStartEventArgs args)
        {
            if (DragStart != null)
                DragStart(this, args);
        }

        /// <summary>
        /// Raises the drag end.
        /// </summary>
        /// <param name="args">The <see cref="ChartDragEndEventArgs"/> instance containing the event data.</param>
        protected void RaiseDragEnd(ChartDragEndEventArgs args)
        {
            if (DragEnd != null)
                DragEnd(this, args);
        }

        /// <summary>
        /// Raises the drag delta.
        /// </summary>
        /// <param name="args">The arguments.</param>
        protected void RaiseDragDelta(DragDelta args)
        {
            if (DragDelta != null)
                DragDelta(this, args);
        }

        /// <summary>
        /// Raises the drag enter.
        /// </summary>
        /// <param name="args">The <see cref="XySegmentEnterEventArgs"/> instance containing the event data.</param>
        protected void RaiseDragEnter(XySegmentEnterEventArgs args)
        {
            if (SegmentEnter != null)
                SegmentEnter(this, args);
        }

        /// <summary>
        /// Raises the preview end.
        /// </summary>
        /// <param name="args">The <see cref="XyPreviewEndEventArgs"/> instance containing the event data.</param>
        protected void RaisePreviewEnd(XyPreviewEndEventArgs args)
        {
            if (PreviewDragEnd != null)
                PreviewDragEnd(this, args);   
        }

        /// <summary>
        /// Resets the dragging elements.
        /// </summary>
        /// <param name="reason">The reason.</param>
        /// <param name="dragEndEvent">if set to <c>true</c> [drag end event].</param>
        protected virtual void ResetDraggingElements(string reason, bool dragEndEvent)
        {
            KeyDown -= CoreWindow_KeyDown;
            UnHoldPanning(true);
            if (dragEndEvent)
                RaiseDragEnd(new ChartDragEndEventArgs());
            ResetSegmentDragTooltipInfo();
        }

        private void UnHoldPanning(bool value)
        {
            var eumerator = Area.Behaviors.OfType<ChartZoomPanBehavior>();
            foreach (var behavior in eumerator)
            {
                behavior.InternalEnablePanning = value;
                behavior.InternalEnableSelectionZooming = value;
            }
        }

        /// <summary>
        /// Updates the drag spliter.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <param name="position">The position.</param>
        protected virtual void UpdateDragSpliter(Rectangle rect, string position)
        {
            int index = Segments.IndexOf(rect.Tag as ChartSegment);
            XySegmentEnterEventArgs args = new XySegmentEnterEventArgs
            {

                XValue = GetActualXValue(index),
                SegmentIndex = index,
                CanDrag = true,
                YValue = YValues[index]
            };
            RaiseDragEnter(args);
            if(!args.CanDrag) return;
           
            if (DragSpliter == null)
            {
                DragSpliter = new ContentControl();
                SeriesPanel.Children.Add(DragSpliter);
                if (position == "Left" || position == "Right")
                    DragSpliter.Template = ChartDictionaries.GenericCommonDictionary["DragSpliterLeft"] as ControlTemplate;
                else
                    DragSpliter.Template = ChartDictionaries.GenericCommonDictionary["DragSpliterTop"] as ControlTemplate;
            }

            double canvasLeft = 0d, canvasTop = 0d, spliterHeight = 0d, spliterWidth = 0d, margin;
            if (position == "Top")
            {
                var top = Canvas.GetTop(rect);
                var width = rect.Width;
                margin = width / 3;
                spliterWidth = width - margin * 2;
                DragSpliter.Margin = new Thickness(margin, 0, margin, 0);
                canvasLeft = Canvas.GetLeft(rect);
                canvasTop = top + 7;
                spliterHeight = spliterWidth / 5;
            }
            else if (position == "Bottom")
            {
                var bottom = Canvas.GetTop(rect) + rect.Height;
                var width = rect.Width;
                margin = width / 3;
                spliterWidth = width - margin * 2;
                DragSpliter.Margin = new Thickness(margin, 0, margin, 0);

                canvasLeft = Canvas.GetLeft(rect);
                canvasTop = bottom - 7;

                spliterHeight = spliterWidth / 5;
            }
            else if (position == "Right")
            {
                var left = Canvas.GetLeft(rect) + rect.Width;
                var height = rect.Height;
                margin = height / 3;
                spliterHeight = height - margin * 2;
                DragSpliter.Margin = new Thickness(0, margin, 0, margin);
                canvasTop = Canvas.GetTop(rect);
                canvasLeft = left - 20;
                spliterWidth = spliterHeight / 5;
            }
            else if (position == "Left")
            {
                var left = Canvas.GetLeft(rect);
                var height = rect.Height;
                margin = height / 3;
                spliterHeight = height - margin * 2;
                DragSpliter.Margin = new Thickness(0, margin, 0, margin);
                canvasTop = Canvas.GetTop(rect);
                canvasLeft = left + 10;
                spliterWidth = spliterHeight / 5;
            }
            DragSpliter.SetValue(Canvas.LeftProperty, canvasLeft);
            DragSpliter.SetValue(Canvas.TopProperty, canvasTop);
            DragSpliter.Height = spliterHeight;
            DragSpliter.Width = spliterWidth;
        }

        /// <summary>
        /// Resets the drag spliter.
        /// </summary>
        protected virtual void ResetDragSpliter()
        {
            if (DragSpliter != null)
            {
                SeriesPanel.Children.Remove(DragSpliter);
                DragSpliter = null;
            }
        }

        /// <summary>
        /// Activates the dragging.
        /// </summary>
        /// <param name="mousePos">The mouse position.</param>
        /// <param name="element">The element.</param>
        internal virtual void ActivateDragging(Point mousePos, object element)
        {

#if NETFX_CORE
            Focus(FocusState.Keyboard);
#elif SILVERLIGHT
            Focus();
#elif WPF
            Keyboard.Focus(this);
#endif
            SegmentIndex = Segments.IndexOf(((FrameworkElement) element).Tag as ChartSegment);
            KeyDown += CoreWindow_KeyDown;
            var dragEventArgs = new ChartDragStartEventArgs { BaseXValue = GetActualXValue(SegmentIndex) };
            if (EmptyPointIndexes != null)
            {
                var emptyPointIndex = EmptyPointIndexes[0];
                foreach (var index in emptyPointIndex)
                    if (SegmentIndex == index)
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
            }
            UnHoldPanning(false);
        }

#if NETFX_CORE

        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            if (EnableSegmentDragging)
                OnChartDragDelta(e.GetCurrentPoint(SeriesPanel).Position, e.OriginalSource);
            base.OnPointerMoved(e);
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            if (EnableSegmentDragging)
            {
                OnChartDragStart(e.GetCurrentPoint(SeriesPanel).Position, e.OriginalSource);
                SeriesPanel.CapturePointer(e.Pointer);
            }
            base.OnPointerPressed(e);
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            if (EnableSegmentDragging)
            {
                OnChartDragEnd(e.GetCurrentPoint(SeriesPanel).Position, e.OriginalSource);
                SeriesPanel.ReleasePointerCapture(e.Pointer);
            }
            base.OnPointerReleased(e);
        }

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            if (EnableSegmentDragging)
                OnChartDragEntered(e.GetCurrentPoint(SeriesPanel).Position, e.OriginalSource);
            base.OnPointerEntered(e);
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            if (EnableSegmentDragging)
                OnChartDragExited(e.GetCurrentPoint(SeriesPanel).Position, e.OriginalSource);
            base.OnPointerExited(e);
        }

#endif

#if WPF || SILVERLIGHT_UNCOMMON
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (EnableSegmentDragging)
                OnChartDragDelta(e.GetPosition(SeriesPanel), e.OriginalSource);
            base.OnMouseMove(e);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (EnableSegmentDragging)
            {
                SeriesPanel.CaptureMouse();
                OnChartDragStart(e.GetPosition(SeriesPanel), e.OriginalSource);
                base.OnMouseLeftButtonDown(e);
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (EnableSegmentDragging)
            {
                SeriesPanel.ReleaseMouseCapture();
                OnChartDragEnd(e.GetPosition(SeriesPanel), e.OriginalSource);
            }
            base.OnMouseLeftButtonUp(e);
        }
#if WPF
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (EnableSegmentDragging)
                OnChartDragEntered(e.GetPosition(SeriesPanel), e.MouseDevice.DirectlyOver);
            base.OnMouseEnter(e);

        }
#else
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (EnableSegmentDragging)
                OnChartDragEntered(e.GetPosition(SeriesPanel),  e.OriginalSource);
            base.OnMouseEnter(e);

        }
#endif

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (EnableSegmentDragging)
                OnChartDragExited(e.GetPosition(SeriesPanel), e.OriginalSource);
            base.OnMouseLeave(e);
        }
#endif

#if WINDOWS_PHONE7 || WINDOWS_PHONE8
        private Point initialPos;
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (EnableSegmentDragging)
            {
                initialPos = e.GetPosition(SeriesPanel);
                CaptureMouse();  
                OnChartDragStart(initialPos, e.OriginalSource);
            }
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnManipulationCompleted(ManipulationCompletedEventArgs e)
        {
            if (EnableSegmentDragging)
            {
                ReleaseMouseCapture();
                OnChartDragEnd(new Point(initialPos.X + e.ManipulationOrigin.X, initialPos.Y + e.ManipulationOrigin.Y),
                    e.OriginalSource);
            }
            base.OnManipulationCompleted(e);
        }

        protected override void OnManipulationDelta(ManipulationDeltaEventArgs e)
        {
            if (EnableSegmentDragging)
                OnChartDragDelta(new Point(initialPos.X + e.CumulativeManipulation.Translation.X, initialPos.Y + e.CumulativeManipulation.Translation.Y), e.OriginalSource);
            base.OnManipulationDelta(e);
        }
#endif

        protected virtual void OnChartDragStart(Point mousePos, object originalSource)
        { 
        
        }

        protected virtual void OnChartDragDelta(Point mousePos, object originalSource)
        { 
        
        }

        protected virtual void OnChartDragEnd(Point mousePos, object originalSource)
        {

        }

        protected virtual void OnChartDragEntered(Point mousePos, object originalSource)
        {

        }

        protected virtual void OnChartDragExited(Point mousePos, object originalSource)
        {
            ResetDragSpliter();
           
        }

        internal double GetSnapToPoint(double actualValue)
        {
            var outValue = actualValue;
            switch (SnapToPoint)
            {
                case SnapToPoint.Round:
                {
                    outValue = Math.Round(actualValue, RoundToDecimal);
                    break;
                }
                case SnapToPoint.Ceil:
                {
                    outValue = Math.Ceiling(actualValue);
                    break;
                }
                case SnapToPoint.Floor:
                {
                    outValue = Math.Floor(actualValue);
                    break;
                }
            }
            return outValue;
        }

        private void ResetSegmentDragTooltipInfo()
        {
            if (Tooltip != null)
            {
                SeriesPanel.Children.Remove(Tooltip);
                Tooltip = null;
                DragInfo = null;
            }
        }
        
        internal void UpdateSegmentDragValueToolTip(Point pos, ChartSegment segment, double newValue, double offsetY)
        {
            if (!EnableDragTooltip) return;
            if (Tooltip == null)
            {
                DragInfo = new ChartDragSegmentInfo();
                Tooltip = new ContentControl {Content = DragInfo};
                SeriesPanel.Children.Add(Tooltip);
                if (DragTooltipTemplate != null)
                {
                    normalTooltip = oppLeftTooltip = oppRightTootip = DragTooltipTemplate;
                }
                else
                {
                    normalTooltip = ChartDictionaries.GenericCommonDictionary["SegmentDragInfo"] as DataTemplate;
                    oppLeftTooltip =
                        ChartDictionaries.GenericCommonDictionary["SegmentDragInfoOppLeft"] as DataTemplate;
                    oppRightTootip =
                        ChartDictionaries.GenericCommonDictionary["SegmentDragInfoOppRight"] as DataTemplate;
                }
                DragInfo.PrefixLabelTemplate = ActualYAxis.PrefixLabelTemplate;
                DragInfo.PostfixLabelTemplate = ActualYAxis.PostfixLabelTemplate;
                Tooltip.ContentTemplate = IsActualTransposed ? oppRightTootip : normalTooltip;
            }
            DragInfo.Segment = segment;
            DragInfo.Brush = segment.Interior;
            DragInfo.ScreenCoordinates = pos;
            ((ChartDragSegmentInfo) DragInfo).NewValue = newValue;
            DragInfo.Segment = segment;
            Tooltip.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            var posY = pos.Y - Tooltip.DesiredSize.Height;
            if (IsActualTransposed)
            {
                if (pos.X + Tooltip.DesiredSize.Width < ActualWidth)
                {
                    Tooltip.ContentTemplate = oppRightTootip;
                    Canvas.SetLeft(Tooltip, pos.X);
                    Canvas.SetTop(Tooltip, pos.Y - Tooltip.DesiredSize.Height/2);
                }
                else
                {
                    Canvas.SetLeft(Tooltip, ActualWidth - Tooltip.DesiredSize.Width);
                    Canvas.SetTop(Tooltip, pos.Y - Tooltip.DesiredSize.Height/2);
                }
            }
            else if (posY < 0)
            {
                var posX = pos.X - Tooltip.DesiredSize.Width - offsetY;
                if (posX < 0)
                {
                    Tooltip.ContentTemplate = oppRightTootip;
                    Canvas.SetLeft(Tooltip, pos.X + offsetY);
                }
                else
                {
                    Tooltip.ContentTemplate = oppLeftTooltip;
                    Canvas.SetLeft(Tooltip, posX);
                }
                var adjPos = pos.Y - Tooltip.DesiredSize.Height/2;
                Canvas.SetTop(Tooltip, adjPos < 0 ? 0 : adjPos);

            }
            else
            {
                Tooltip.ContentTemplate = normalTooltip;
                Canvas.SetTop(Tooltip, posY);
                Canvas.SetLeft(Tooltip, pos.X - Tooltip.DesiredSize.Width/2);
            }
        }
     
        #endregion

    }

    public class DragDelta : EventArgs
    {
       public double Delta { get; set; }
       public bool Cancel { get; set; }
    }

    public class ChartDragStartEventArgs : EventArgs
    {
        public bool EmptyPoint { get; set; }
        public bool Cancel { get; set; }
        public object BaseXValue { get; set; }
    }

    public class ChartDragEndEventArgs : EventArgs
    {
        public double BaseYValue { get; set; }
        public double NewYValue { get; set; }
    }

    public class ChartDragSegmentInfo : ChartDragPointinfo
    {
        private double newValue;

        public double NewValue
        {
            get { return newValue; }
            set
            {
                newValue = value;
                OnPropertyChanged("NewValue");
            }
        }

        private double baseValue;

        public double BaseValue
        {
            get { return baseValue; }
            set
            {
                baseValue = value;
                OnPropertyChanged("BaseValue");
            }
        }
    }

    public class ChartDragPointinfo : INotifyPropertyChanged
    {

        public Brush Brush { get; set; }

        private ChartSegment segment;

        public ChartSegment Segment
        {
            get { return segment; }
            set
            {
                segment = value;
                OnPropertyChanged("Segment");
            }
        }

        private Point screenCoordinates;

        private double delta;

        public double Delta
        {
            get { return delta; }
            set
            {
                delta = value;
                OnPropertyChanged("Delta");
            }
        }

        private bool isNegative;

        public bool IsNegative
        {
            get { return isNegative; }
            set
            {
                isNegative = value;
                OnPropertyChanged("IsNegative");
            }
        }

        public Point ScreenCoordinates
        {
            get { return screenCoordinates; }
            set
            {
                screenCoordinates = value;
                OnPropertyChanged("ScreenCoordinates");
            }
        }

        public DataTemplate PrefixLabelTemplate { get; set; }

        public DataTemplate PostfixLabelTemplate { get; set; }


        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Called when Property changed 
        /// </summary>
        /// <param name="name"></param>
        protected virtual void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }

    public class XySegmentDragEventArgs : DragDelta
    {
        public double BaseYValue { get; set; }
        public double NewYValue { get; set; }
        public ChartSegment Segment { get; set; }
    }

    public class XyPreviewEndEventArgs : CancelEventArgs
    {
        public double BaseYValue { get; set; }
        public double NewYValue { get; set; }
    }
}
