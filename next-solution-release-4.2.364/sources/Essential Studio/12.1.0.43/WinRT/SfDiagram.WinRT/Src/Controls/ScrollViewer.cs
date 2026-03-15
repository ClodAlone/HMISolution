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
using System.Text;
using Syncfusion.UI.Xaml.Diagram.Panels;
using Syncfusion.UI.Xaml.Diagram.Utility;
#if WINRT_USING
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Devices.Input;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input; 
#else
using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input; 
#endif

namespace Syncfusion.UI.Xaml.Diagram.Controls
{
    public partial class ScrollViewer : ContentControl
    {
        #region DPs

        #region Extended Size

        public double MinimumX
        {
            get { return (double)GetValue(MinimumXProperty); }
            set { SetValue(MinimumXProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinimumX.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinimumXProperty =
            DependencyProperty.Register("MinimumX", typeof(double), typeof(ScrollViewer), new PropertyMetadata(0d));

        public double MinimumY
        {
            get { return (double)GetValue(MinimumYProperty); }
            set { SetValue(MinimumYProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinimumY.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinimumYProperty =
            DependencyProperty.Register("MinimumY", typeof(double), typeof(ScrollViewer), new PropertyMetadata(0d));

        public double MaximumY
        {
            get { return (double)GetValue(MaximumYProperty); }
            private set { SetValue(MaximumYProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScrollableHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximumYProperty =
            DependencyProperty.Register("MaximumY", typeof(double), typeof(ScrollViewer), new PropertyMetadata(0d));

        public double MaximumX
        {
            get { return (double)GetValue(MaximumXProperty); }
            private set { SetValue(MaximumXProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScrollableWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximumXProperty =
            DependencyProperty.Register("MaximumX", typeof(double), typeof(ScrollViewer), new PropertyMetadata(0d));

        public double Left { get; private set; }

        public double Top { get; private set; }

        public double Right { get; private set; }

        public double Bottom { get; private set; }

        #endregion

        public Visibility ComputedVerticalScrollBarVisibility
        {
            get { return (Visibility)GetValue(ComputedVerticalScrollBarVisibilityProperty); }
            private set { SetValue(ComputedVerticalScrollBarVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ComputedVerticalScrollBarVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ComputedVerticalScrollBarVisibilityProperty =
            DependencyProperty.Register("ComputedVerticalScrollBarVisibility", typeof(Visibility), typeof(ScrollViewer), new PropertyMetadata(Visibility.Collapsed));

        public Visibility ComputedHorizontalScrollBarVisibility
        {
            get { return (Visibility)GetValue(ComputedHorizontalScrollBarVisibilityProperty); }
            private set { SetValue(ComputedHorizontalScrollBarVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ComputedHorizontalScrollBarVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ComputedHorizontalScrollBarVisibilityProperty =
            DependencyProperty.Register("ComputedHorizontalScrollBarVisibility", typeof(Visibility), typeof(ScrollViewer), new PropertyMetadata(Visibility.Collapsed));

        #region Viewport

        public double HorizontalOffset
        {
            get { return (double)GetValue(HorizontalOffsetProperty); }
            private set { SetValue(HorizontalOffsetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HorizontalOffset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register("HorizontalOffset", typeof(double), typeof(ScrollViewer), new PropertyMetadata(0d, InValidate));

        public double VerticalOffset
        {
            get { return (double)GetValue(VerticalOffsetProperty); }
            private set { SetValue(VerticalOffsetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VerticalOffset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register("VerticalOffset", typeof(double), typeof(ScrollViewer), new PropertyMetadata(0d, InValidate));


        public double CurrentZoom
        {
            get { return (double)GetValue(CurrentZoomProperty); }
            set { SetValue(CurrentZoomProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentZoom.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentZoomProperty =
            DependencyProperty.Register("CurrentZoom", typeof(double), typeof(ScrollViewer), new PropertyMetadata(1.0d,OnZoomChanged));

        private static void OnZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            double delta = (double)e.NewValue / (d as ScrollViewer).ZoomPanTransform.ScaleX;
            //////ViewportWidth /= delta;
            //////ViewportHeight /= delta;
            //////HorizontalOffset /= delta;
            //////VerticalOffset /= delta;
            ////Left *= delta;
            ////Top *= delta;
            ////Right *= delta;
            ////Bottom *= delta;
            (d as ScrollViewer).ZoomPanTransform.ScaleX = (double)e.NewValue;
            (d as ScrollViewer).ZoomPanTransform.ScaleY = (double)e.NewValue;
        }

        

        private static void InValidate(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScrollViewer sv = d as ScrollViewer;
            //sv.Update();
            //double scale = sv.CurrentZoom;
            sv.InvalidateArrange();
        }

        public double ViewportWidth
        {
            get { return (double)GetValue(ViewportWidthProperty); }
            private set { SetValue(ViewportWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewportWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewportWidthProperty =
            DependencyProperty.Register("ViewportWidth", typeof(double), typeof(ScrollViewer), new PropertyMetadata(0d, InValidate));

        public double ViewportHeight
        {
            get { return (double)GetValue(ViewportHeightProperty); }
            private set { SetValue(ViewportHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewportHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewportHeightProperty =
            DependencyProperty.Register("ViewportHeight", typeof(double), typeof(ScrollViewer), new PropertyMetadata(0d, InValidate));


        public Rect Viewport { get; private set; }

        #endregion
        
        public Panel Page
        {
            get { return (Panel)GetValue(PageProperty); }
            set { SetValue(PageProperty, value); }
        }

        public double MinZoom
        {
            get { return (double)GetValue(MinZoomProperty); }
            set { SetValue(MinZoomProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinZoom.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinZoomProperty =
            DependencyProperty.Register("MinZoom", typeof(double), typeof(ScrollViewer), new PropertyMetadata(0.3, OnPropertyChanged));

        public double MaxZoom
        {
            get { return (double)GetValue(MaxZoomProperty); }
            set { SetValue(MaxZoomProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxZoom.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxZoomProperty =
            DependencyProperty.Register("MaxZoom", typeof(double), typeof(ScrollViewer), new PropertyMetadata(30.0, OnPropertyChanged));


        public double ZoomFactor
        {
            get { return (double)GetValue(ZoomFactorProperty); }
            set { SetValue(ZoomFactorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZoomFactor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZoomFactorProperty =
            DependencyProperty.Register("ZoomFactor", typeof(double), typeof(ScrollViewer), new PropertyMetadata(0.2, OnPropertyChanged));


        public double? ScrollFactor
        {
            get { return (double?)GetValue(ScrollFactorProperty); }
            set { SetValue(ScrollFactorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScrollFactor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScrollFactorProperty =
            DependencyProperty.Register("ScrollFactor", typeof(double?), typeof(ScrollViewer), new PropertyMetadata(null, OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScrollViewer scrollviewer = d as ScrollViewer;
            ScrollChanged current = scrollviewer._mCurrentState;
            current.MinZoom = scrollviewer.MinZoom;
            current.MaxZoom = scrollviewer.MaxZoom;
            current.ZoomFactor = scrollviewer.ZoomFactor;
            current.ScrollFactor = scrollviewer.ScrollFactor;
            scrollviewer.InvokeViewportChangedEvent(current);
        }

        internal void InvokeViewportChangedEvent(ScrollChanged newstate)
        {
            ScrollChanged old = _mCurrentState;
            _mCurrentState = newstate;
            if (_mSharedData.NeededEvents.NeedViewportChangedEvent && !old.Equals(newstate))
            {
                _mSharedData.Graph.OnViewPortChangedEvent(new ChangeEventArgs<object, ScrollChanged>(_mSharedData.Graph.Source, ref old, ref _mCurrentState));
            }
        }


        // Using a DependencyProperty as the backing store for Page.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PageProperty =
            DependencyProperty.Register("Page", typeof(Panel), typeof(ScrollViewer), new PropertyMetadata(null, OnPageChanged));

        private static void OnPageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramPage page = e.NewValue as DiagramPage;
            if (page != null)
            {
                (d as ScrollViewer).SetSharedData(page.SharedData);
            }
        }

        #endregion

        private void Update()
        {
            double scale = CurrentZoom;

            Viewport = new Rect(HorizontalOffset,
                                VerticalOffset,
                                ViewportWidth,
                                ViewportHeight);

            _mViewportChangedEvent.Publish(new ChangeArgs<Rect>(null, Rect.Empty,
                                                                new Rect(HorizontalOffset, VerticalOffset, ViewportWidth, ViewportHeight)));
            IPageSettings settings = _mSharedData.PageSettingsController;
            double left = _mSharedData.SpatialSearch._pageLeft;
            double top = _mSharedData.SpatialSearch._pageTop;
            double right = _mSharedData.SpatialSearch._pageRight;
            double bottom = _mSharedData.SpatialSearch._pageBottom;
            if (left > right)
            {
                left = right = 0;
            }
            if (top > bottom)
            {
                top = bottom = 0;
            }

            if (settings != null)
            {
                double pageWidth = settings.PageWidth;
                double pageHeight = settings.PageHeight;
#if SyncfusionFramework4_5_1 && WINRT
                
            if (_mSharedData.PageSettingsController.PageSetup==PageSetup.PrintandPageProperties)
            {
                //if (settings.PageWidth == 0)
                //{
                //    pageWidth = _mSharedData.Graph.PrintingService.PreviewSize.Width;
                //}
                //else
                //{
                //    pageWidth = settings.PageWidth;
                //}
                //if (settings.PageHeight == 0)
                //{
                //    pageHeight = _mSharedData.Graph.PrintingService.PreviewSize.Height;
                //}
                //else
                //{
                //    pageHeight = settings.PageHeight;
                //}
            }
#endif
                if (settings.PageWidth.IsValid() && settings.PageHeight.IsValid() && settings.PageWidth > 0 && settings.PageHeight > 0)
                {
                    if (settings.MultiplePage)
                    {
                        left = Math.Floor(left / pageWidth) * pageWidth;
                        top = Math.Floor(top / pageHeight) * pageHeight;
                        right = Math.Ceiling(right / pageWidth) * pageWidth;
                        bottom = Math.Ceiling(bottom / pageHeight) * pageHeight;
                    }
                    else
                    {
                        left = 0;
                        top = 0;
                        right = pageWidth;
                        bottom = pageHeight;
                    }
                }
            }

            //_backgroundTrans.X = left;
            //_backgroundTrans.Y = top;
            
            //_pageBackground.Width = right - left;
            //_pageBackground.Height = bottom - top;

            left = left * scale;
            top = top * scale;
            right = right * scale;
            bottom = bottom * scale;

            if (settings != null)
            {
                Thickness margin = settings.OffPageMinMargin ?? new Thickness(0);
                left -= margin.Left;
                top -= margin.Top;
                right += margin.Right;
                bottom += margin.Bottom;

                if (settings.OffPageMaxMargin.HasValue)
                {
                    margin = settings.OffPageMaxMargin.Value;
                    left = Math.Min(left, margin.Left);
                    top = Math.Min(top, margin.Top);
                    right = Math.Min(right, margin.Right);
                    bottom = Math.Min(bottom, margin.Bottom);
                }
            }

            Rect diagramArea = Rect.Empty;
            //Point pt = new Point(0, 0);
            //pt.X = Math.Min(0, left);
            //pt.Y = Math.Min(0, top);
            //diagramArea.Union(pt);
            //pt.X = Math.Max(ViewportWidth, right);
            //pt.Y = Math.Max(ViewportHeight, bottom);
            //diagramArea.Union(pt);
            diagramArea.Union(new Rect(0, 0, ViewportWidth, ViewportHeight));
            diagramArea.Union(new Rect(new Point(left, top), new Point(right, bottom)));

            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;

            diagramArea.Width -= ViewportWidth ;
            diagramArea.Height -= ViewportHeight;

            if (diagramArea.Left > HorizontalOffset)
            {
                double diff = diagramArea.Left - HorizontalOffset;
                diagramArea.X -= diff;
                diagramArea.Width += diff;
            }
            if (HorizontalOffset > diagramArea.Right)
            {
                diagramArea.Width = HorizontalOffset - diagramArea.Left;
            }
            MinimumX = diagramArea.Left;
            MaximumX = Math.Max(0, diagramArea.Right);

            if (diagramArea.Top > VerticalOffset)
            {
                double diff = diagramArea.Top - VerticalOffset;
                diagramArea.Y -= diff;
                diagramArea.Height += diff;
            }
            if (VerticalOffset > diagramArea.Bottom)
            {
                diagramArea.Height = VerticalOffset - diagramArea.Top;
            }
            MinimumY = diagramArea.Top;
            MaximumY = Math.Max(0, diagramArea.Bottom);


            if (Left >= HorizontalOffset && Right <= ViewportWidth + HorizontalOffset)
            {
                ComputedHorizontalScrollBarVisibility = Visibility.Collapsed;
            }
            else
            {
                ComputedHorizontalScrollBarVisibility = Visibility.Visible;
            }

            if (Top >= VerticalOffset && Bottom <= ViewportHeight + VerticalOffset)
            {
                ComputedVerticalScrollBarVisibility = Visibility.Collapsed;
            }
            else
            {
                ComputedVerticalScrollBarVisibility = Visibility.Visible;
            }

            ZoomPanTransform.TranslateX = -HorizontalOffset;
            ZoomPanTransform.TranslateY = -VerticalOffset;
            _mSharedData.Adorner.UpdateAdornerTransform();
        }

#if WINRT
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);
            UpdateVisualState(e);
        }

        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            base.OnPointerMoved(e);
            UpdateVisualState(e);
        }
        //DispatcherTimer t;
#if WINRT
		private async void UpdateVisualState(PointerRoutedEventArgs e)  
#else
        private void UpdateVisualState(PointerRoutedEventArgs e)
#endif
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                VisualStateManager.GoToState(this, "MouseIndicator", false);
            }
            else if (e.Pointer.PointerDeviceType == PointerDeviceType.Touch)
            {
                VisualStateManager.GoToState(this, "TouchIndicator", false);
            }
            //VisualStateManager.GoToState(this, "NoIndicator", true);

            //if (t == null)
            //{
            //    t = new DispatcherTimer();
            //    t.Interval = new TimeSpan(0, 0, 3);
            //    t.Start();
            //    t.Tick += (s, evt) =>
            //        {
            //            VisualStateManager.GoToState(this, "NoIndicator", true);
            //            t.Stop();
            //            t = null;
            //        };
            //}
            //else
            //{
            //    t.Stop();
            //    t.Start();
            //}
#if WINRT
            await
#endif
 Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low,
                () => VisualStateManager.GoToState(this, "NoIndicator", true));
        }
#else

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            UpdateVisualState(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            UpdateVisualState(e);
        }

        private void UpdateVisualState(MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "MouseIndicator", true);
            //await 
            //VisualStateManager.GoToState(this, "NoIndicator", true);
            this.Dispatcher.BeginInvoke(new Action(
                                            () => { VisualStateManager.GoToState(this, "NoIndicator", true); }));
        }
#endif

        private ScrollBar VerticalScrollBar = null;
        private ScrollBar HorizontalScrollBar = null;
        
        //protected override Size MeasureOverride(Size availableSize)
        //{
        //    Size desiredSize = base.MeasureOverride(availableSize);
        //    desiredSize = new Size(availableSize.Width.IsValid() ? availableSize.Width : desiredSize.Width,
        //                           availableSize.Height.IsValid() ? availableSize.Height : desiredSize.Height);
        //    ScrollableHeight = 10000;
        //    ScrollableWidth = 10000;
        //    return desiredSize;
        //}

        //protected override Size ArrangeOverride(Size finalSize)
        //{
        //    base.ArrangeOverride(finalSize);
        //    ViewportWidth = finalSize.Width;
        //    ViewportHeight = finalSize.Height;
        //    return new Size(finalSize.Width + 1, finalSize.Height + 1);
        //}
    }
}
