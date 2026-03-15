#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
#if !SILVERLIGHT && !WP7
using System.Threading.Tasks;
#endif
#if WinRT
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Input;
#endif

namespace Syncfusion.UI.Xaml.Grid
{
    [TemplatePart(Name = "PART_LeftThumbGripper", Type = typeof(Thumb))]
    [TemplatePart(Name = "PART_RightThumbGripper", Type = typeof(Thumb))]
    public class PopupContentControl : ContentControl, IDisposable
    {
        #region Fields
        Thumb LeftThumb;
        Thumb RightThumb;
        bool isInResize;
        bool canDrop;
        double mouseHorizontalPosition;
        double mouseVerticalPosition;        
        internal PopupContentPositionChanged PopupContentPositionChanged;
        internal PopupContentDropped PopupContentDropped;
        internal PopupContentResizing PopupContentResizing;
        internal PopupContentResized PopupContentResized;
        SfDataGrid dataGrid;
#if WPF
        Point pointFromGrid;
#endif
        #endregion

        #region Property
        
        // Provides the popup is dragged from GroupDropArea or not
        public bool IsDragFromGroupDropArea { get; internal set; }

        // Provides the popup is dragged from ColumnChooser or not
        public bool IsDragFromColumnChooser { get; set; }

        internal Rect InitialRect { get; set; }

        // Provides the popup is in touch or not
        public bool IsOpenInTouch { get; internal set; }
        
        public Visibility LeftResizeThumbVisibility
        {
            get { return (Visibility)GetValue(LeftResizeThumbVisibilityProperty); }
            set { SetValue(LeftResizeThumbVisibilityProperty, value); }
        }

        public static readonly DependencyProperty LeftResizeThumbVisibilityProperty =
            DependencyProperty.Register("LeftResizeThumbVisibility", typeof(Visibility), typeof(PopupContentControl), new PropertyMetadata(Visibility.Visible));

        public Visibility RightResizeThumbVisibility
        {
            get { return (Visibility)GetValue(RightResizeThumbVisibilityProperty); }
            set { SetValue(RightResizeThumbVisibilityProperty, value); }
        }

        public static readonly DependencyProperty RightResizeThumbVisibilityProperty =
            DependencyProperty.Register("RightResizeThumbVisibility", typeof(Visibility), typeof(PopupContentControl), new PropertyMetadata(Visibility.Visible));

        public double ThumbWidth
        {
            get { return (double)GetValue(ThumbWidthProperty); }
            set { SetValue(ThumbWidthProperty, value); }
        }
        
        public static readonly DependencyProperty ThumbWidthProperty =
            DependencyProperty.Register("ThumbWidth", typeof(double), typeof(PopupContentControl), new PropertyMetadata(20d));

        #endregion

        #region Ctor

        public PopupContentControl(SfDataGrid dataGrid)
        {
            this.DefaultStyleKey = typeof(PopupContentControl);
            this.dataGrid = dataGrid;
#if WPF
            this.IsManipulationEnabled = true;
#endif
        }


        #endregion

        #region override methods

#if WinRT 
        protected override void OnApplyTemplate() 
#else
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            if (RightThumb != null)
            {
                this.RightThumb.DragDelta -= OnRightThumbDragDelta;
                this.RightThumb.DragCompleted -= OnRightThumbDragCompleted;
            }
            if (this.LeftThumb != null)
            {
                this.LeftThumb.DragDelta -= OnLeftThumbDragDelta;
                this.LeftThumb.DragCompleted -= OnLeftThumbDragCompleted;
            }

            this.LeftThumb = base.GetTemplateChild("PART_LeftThumbGripper") as Thumb;
            this.RightThumb = base.GetTemplateChild("PART_RightThumbGripper") as Thumb;
            if (RightThumb != null)
            {
                this.RightThumb.DragDelta += OnRightThumbDragDelta;
                this.RightThumb.DragCompleted += OnRightThumbDragCompleted;
            }
            if (this.LeftThumb != null)
            {
                this.LeftThumb.DragDelta += OnLeftThumbDragDelta;
                this.LeftThumb.DragCompleted += OnLeftThumbDragCompleted;
            }
            if (beforeLoaded)
            {
                ApplyState(true);
                beforeLoaded = false;
            }
        }

#if WinRT
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
#elif WPF
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
#else
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
#endif
        {
            if (!isInResize)
            {
#if WinRT
                PointerPoint p = e.GetCurrentPoint(null);
                Point pp = p.Position;
#elif WPF
                Point p = e.GetPosition(null);
                Point pp = this.PointToScreen(p);
#else
                Point pp = e.GetPosition(null);
#endif
                mouseVerticalPosition = pp.Y;
                mouseHorizontalPosition = pp.X;
                e.Handled = true;
            }

#if WinRT
            this.CapturePointer(e.Pointer);
            base.OnPointerPressed(e);
#elif WPF
            this.CaptureMouse();
            base.OnPreviewMouseDown(e);
#else
            this.CaptureMouse();
            base.OnMouseLeftButtonDown(e);
#endif
        }


#if WinRT
        protected override void OnPointerReleased(PointerRoutedEventArgs e)
#elif WPF
        protected override void OnPreviewMouseUp(System.Windows.Input.MouseButtonEventArgs e)
#elif SILVERLIGHT
        
        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
#else
        protected override void OnLostMouseCapture(MouseEventArgs e)
#endif
        {
#if WinRT
            if (!isInResize && (canDrop || e.Pointer.PointerDeviceType == PointerDeviceType.Mouse))
#else
            if (!isInResize && (canDrop))
#endif
            {
                if (this.PopupContentDropped != null)
                {
#if WinRT
                    PointerPoint p = e.GetCurrentPoint(null);
                    Point pp = p.Position;
                    this.PopupContentDropped(pp, e.GetCurrentPoint(this.dataGrid).Position);
#elif WPF
                    Point p = e.GetPosition(this.dataGrid);
                    Point pp = this.dataGrid.PointToScreen(p);
                    this.PopupContentDropped(pp, e.GetPosition(this.dataGrid));
#else
                    Point pp = e.GetPosition(null);
#if WP
                    this.PopupContentDropped(pp);
#else
                    this.PopupContentDropped(pp, e.GetPosition(this.dataGrid));
#endif
#endif

                }
                canDrop = false;
            }
            
#if WinRT
            this.ReleasePointerCapture(e.Pointer);
            base.OnPointerReleased(e);
#elif WPF
            this.ReleaseMouseCapture();
            base.OnPreviewMouseUp(e);
#elif SILVERLIGHT
            this.ReleaseMouseCapture();
            base.OnMouseLeftButtonUp(e);
#else
            base.OnLostMouseCapture(e);
#endif
        }

#if WPF

        protected override void OnManipulationCompleted(ManipulationCompletedEventArgs e)
        {
            if (!isInResize && (canDrop))
            {
                if (this.PopupContentDropped != null)
                {
                    Point p = pointFromGrid;
                    Point pp = this.dataGrid.PointToScreen(p);
                    this.PopupContentDropped(pp, p);
                }
                canDrop = false;
            }
            this.ReleaseAllTouchCaptures();
            this.dataGrid.GridColumnDragDropController.HidePopup();
            this.dataGrid.GridColumnDragDropController.CloseDragIndication();    
            base.OnManipulationCompleted(e);
        }

        protected override void OnManipulationDelta(ManipulationDeltaEventArgs e)
        {
            List<IManipulator> touchDevice = e.Manipulators.ToList();
            if (!isInResize)
            {
                pointFromGrid = touchDevice[0].GetPosition(this.dataGrid);
                Point p = touchDevice[0].GetPosition(this.dataGrid);
                Point pp = this.dataGrid.PointToScreen(p);
                {
                    DragPopup(p, pp);
                    e.Handled = true;
                }
                mouseVerticalPosition = pp.Y;
                mouseHorizontalPosition = pp.X;
            }
            this.CaptureTouch(touchDevice[0] as TouchDevice);
            base.OnManipulationDelta(e);
        }

#endif
#if WinRT
        

        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            if (!isInResize)
            {
                PointerPoint pp = e.GetCurrentPoint(null);
                Point p = e.GetCurrentPoint(this.dataGrid).Position;
                if (pp.Properties.IsLeftButtonPressed)
                {
                    DragPopup(p, pp.Position);
                    e.Handled = true;
                }

                mouseVerticalPosition = pp.Position.Y;
                mouseHorizontalPosition = pp.Position.X;
            }
            this.CapturePointer(e.Pointer);
            base.OnPointerMoved(e);
        }

#else


#if WPF
        protected override void OnPreviewMouseMove(System.Windows.Input.MouseEventArgs e)
#else
        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
#endif
        {
            if (!isInResize)
            {
                Point p = e.GetPosition(this.dataGrid);
#if WPF
                Point pp = this.dataGrid.PointToScreen(p);
                if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
#else
                Point pp = e.GetPosition(null);
#endif
                {
                    //if (mouseVerticalPosition > 0 && mouseHorizontalPosition > 0)
                    {
                        DragPopup(p, pp);
                    }
#if WPF
                    e.Handled = true;
                }
#endif
                mouseVerticalPosition = pp.Y;
                mouseHorizontalPosition = pp.X;
                }
                this.CaptureMouse();
#if WPF
                base.OnPreviewMouseMove(e);
#else
                base.OnMouseMove(e);
            }
#endif
            
        }
            
#endif

        #endregion

        #region internal method

        internal void ResetMousePosition()
        {
            mouseVerticalPosition = 0;
            mouseHorizontalPosition = 0;
            isInResize = false;
        }

        bool beforeLoaded;
        internal void ApplyState(bool isOpen)
        {
            if (LeftThumb == null)
            {
                beforeLoaded = true;
                return;
            }

            VisualStateManager.GoToState(this, isOpen ? "Open" : "Drag", true);
        }

        #endregion

        #region private methods


#if !WP
        private Rect GetRect()
#else
        internal Rect GetRect()
#endif
        {
#if WinRT
            var point = this.TransformToVisual(null).TransformPoint(new Point(0, 0));
#elif WPF
            var point = this.TranslatePoint(new Point(0, 0), null);
#else
            var point = this.TransformToVisual(null).Transform(new Point(0, 0));
#endif
            var width = this.ActualWidth;
            var height = this.ActualHeight;
            var x = Math.Round(point.X + ((width * 0.05) / 2));
            var y = Math.Round(point.Y + ((height * 0.05)) / 2);
            var rect = new Rect(x, y, width, height);
            return rect;
        }



        private void OnRightThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            isInResize = true;
            double deltaHorizontal = Math.Min(-e.HorizontalChange, this.ActualWidth - this.MinWidth);
            bool changesize = false;
            if (this.PopupContentResizing != null)
                changesize = this.PopupContentResizing(false, -deltaHorizontal);
            if (changesize)
                this.Width -= (deltaHorizontal - (deltaHorizontal * 0.05));
        }

        private void OnLeftThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            isInResize = true;
            double deltaHorizontal = Math.Min(e.HorizontalChange, this.ActualWidth - this.MinWidth);
            bool changesize = false;
            if (this.PopupContentResizing != null)
                changesize = this.PopupContentResizing(true, deltaHorizontal);
        }

        private void OnRightThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            isInResize = false;
            double deltaHorizontal = Math.Min(-e.HorizontalChange, this.ActualWidth -this.MinWidth);
            bool changesize = false;
            if (this.PopupContentResized != null)
                changesize = PopupContentResized(false, -deltaHorizontal);
        }

        private void OnLeftThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            isInResize = false;
            double deltaHorizontal = Math.Min(e.HorizontalChange, this.ActualWidth - this.MinWidth);
            bool changesize = false;
            if (this.PopupContentResized != null)
                changesize = PopupContentResized(true, deltaHorizontal);
        }

        private void ChangeThumbVisibility()
        {
            if (this.LeftResizeThumbVisibility == Visibility.Visible)
                this.LeftResizeThumbVisibility = Visibility.Collapsed;
            if (this.RightResizeThumbVisibility == Visibility.Visible)
                this.RightResizeThumbVisibility = Visibility.Collapsed;
        }

        private void DragPopup(Point p, Point pp)
        {
#if WinRT || WP
#if WinRT
            if (((this.LeftThumb != null && this.LeftThumb.Visibility == Windows.UI.Xaml.Visibility.Visible) || (this.RightThumb != null && this.RightThumb.Visibility == Windows.UI.Xaml.Visibility.Visible)) && !canDrop)
#else
             if (((this.LeftThumb != null && this.LeftThumb.Visibility == Visibility.Visible) || (this.RightThumb != null && this.RightThumb.Visibility == Visibility.Visible)) && !canDrop)
#endif
            {
                mouseVerticalPosition = 0;
                mouseHorizontalPosition = 0;
                isInResize = true;
                return;
            }
#if WinRT
            if (mouseVerticalPosition  > 0 && mouseHorizontalPosition > 0)
            {
#endif
#endif
            double deltaH = pp.X;
            double deltaV = pp.Y;
            if (this.PopupContentPositionChanged != null)
                this.PopupContentPositionChanged(deltaH, deltaV, pp, p);
            if (!canDrop && !this.InitialRect.IsEmpty)
            {
                var rect = this.GetRect();
                if ((rect.X >= this.InitialRect.X + 5) || (rect.X <= this.InitialRect.X - 5) ||
                    (rect.Y >= this.InitialRect.Y + 5) || (rect.Y <= this.InitialRect.Y - 5))
                {
                    canDrop = true;
                    ChangeThumbVisibility();
                }
            }
#if WinRT
            }
#endif
        }

        #endregion

        public void Dispose()
        {
            if (this.LeftThumb != null)
            {
                this.LeftThumb.DragDelta -= OnLeftThumbDragDelta;
                this.LeftThumb.DragCompleted -= OnLeftThumbDragCompleted;
                this.LeftThumb = null;
            }
            if (RightThumb != null)
            {
                this.RightThumb.DragDelta -= OnRightThumbDragDelta;
                this.RightThumb.DragCompleted -= OnRightThumbDragCompleted;
                this.RightThumb = null;
            }
            this.PopupContentDropped = null;
            this.PopupContentPositionChanged = null;
            this.PopupContentResized = null;
            this.PopupContentResizing = null;
            this.dataGrid = null;
        }
    }

    public class UpIndicatorContentControl : ContentControl, IDisposable
    {
        public UpIndicatorContentControl()
        {
            base.DefaultStyleKey = typeof(UpIndicatorContentControl);
        }

#if WinRT

        public bool IsOpen { get; set; }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (IsOpen)
                VisualStateManager.GoToState(this, "Open", false);
        }
#endif

        

        void IDisposable.Dispose()
        {
            
        }
    }

    public class DownIndicatorContentControl : ContentControl, IDisposable
    {
        public DownIndicatorContentControl()
        {
            base.DefaultStyleKey = typeof(DownIndicatorContentControl);
        }

#if WinRT

        public bool IsOpen { get; set; }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (IsOpen)
                VisualStateManager.GoToState(this, "Open", false);
        }
#endif

        void IDisposable.Dispose()
        {

        }
    }

    internal delegate void PopupContentPositionChanged(double HorizontalDelta, double VerticalDelta, Point mousePoint, Point mousePointOverGrid);

    internal delegate bool PopupContentResizing(bool IsinLeft, double Width);

    internal delegate bool PopupContentResized(bool IsinLeft, double ActualWidth);
#if WP
    internal delegate void PopupContentDropped(Point point);
#else
    internal delegate void PopupContentDropped(Point point, Point pointOverGrid);
#endif
}
