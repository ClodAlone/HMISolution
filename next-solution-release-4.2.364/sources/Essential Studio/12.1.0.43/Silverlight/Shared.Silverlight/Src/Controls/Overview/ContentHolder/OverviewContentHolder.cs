#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Collections;
using System.Diagnostics;

namespace Syncfusion.Windows.Shared
{
    public class OverviewContentHolder : ContentControl, IOverviewPanel,IScrollInfo
    {
        public bool AllowResize
        {
            get { return (bool)GetValue(AllowResizeProperty); }
            set { SetValue(AllowResizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllowResize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AllowResizeProperty =
            DependencyProperty.Register("AllowResize", typeof(bool), typeof(OverviewContentHolder), new PropertyMetadata(false));

        public bool EnableFitToPage
        {
            get { return (bool)GetValue(EnableFitToPageProperty); }
            set { SetValue(EnableFitToPageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllowResize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableFitToPageProperty =
            DependencyProperty.Register("EnableFitToPage", typeof(bool), typeof(OverviewContentHolder), new PropertyMetadata(false, new PropertyChangedCallback(OnFitToPageEnabled)));

        private static void OnFitToPageEnabled(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OverviewContentHolder contentholder = (OverviewContentHolder)d;
            contentholder.ZoomReset.Execute(contentholder);
        }
        // Using a DependencyProperty as the backing store for InverseScale.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InverseScaleProperty =
            DependencyProperty.RegisterAttached("InverseScale", typeof(double), typeof(OverviewContentHolder), new PropertyMetadata(1d));

        // Using a DependencyProperty as the backing store for Start.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartProperty =
            DependencyProperty.RegisterAttached("Start", typeof(Point), typeof(OverviewContentHolder), new PropertyMetadata(new Point(0,0)));

        public OverviewContentHolder()
        {
            //this.DefaultStyleKey = typeof(OverviewPanel);
            this.OverviewTransform = new ScaleTransform();// new CompositeTransform() { ScaleX = UnitScale, ScaleY = UnitScale };
            this.LayoutTransform = this.OverviewTransform;
            ZoomIn = new DelegateCommand(ExecuteZoomInCommand, CanZoomInExecute);
            ZoomOut = new DelegateCommand(ExecuteZoomOutCommand, CanZoomOutExecute);
            ZoomTo = new DelegateCommand(ExecuteZoomToCommand, CanZoomToExecute);
            ZoomReset = new DelegateCommand(ExecuteZoomResetCommand, CanZoomResetExecute);
            FitToPageCommand = new DelegateCommand(ExecuteFitPageCommand, CanFitPageExecute);
            //Binding bin = new Binding("Parent.ContentTemplate");
            //bin.Source = this;
            //this.SetBinding(ContentTemplateProperty, bin);
            this.Loaded += new RoutedEventHandler(ZoomPanel_Loaded);
            //this.UseLayoutRounding = false;
            CanHorizontallyScroll = true;
            CanVerticallyScroll = true;
            this.MouseLeftButtonDown += new MouseButtonEventHandler(OverviewContentHolder_MouseLeftButtonDown);
            this.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(OverviewContentHolder_PreviewMouseLeftButtonUp);
            this.Loaded += new RoutedEventHandler(OverviewContentHolder_Loaded);
        }

        void OverviewContentHolder_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            IsDragging = false;
        }

        void OverviewContentHolder_Loaded(object sender, RoutedEventArgs e)
        {
            this.ScrollOwner.ScrollChanged += new ScrollChangedEventHandler(ScrollOwner_ScrollChanged);
        }
        bool evenctcheck = true;
        bool horizontalcheck = true;
        bool verticalcheck = true;
        
        internal static ScrollBar GetScrollBarControl(FrameworkElement element, int whichone)
        {
            DependencyObject parent = VisualTreeHelper.GetChild(element, 0);
            while (parent != null)
            {
                if (parent is ScrollBar)
                {
                    return parent as ScrollBar;
                }
                DependencyObject temp = VisualTreeHelper.GetChild(parent, whichone);
                if (temp == null && parent is FrameworkElement)
                {
                    parent = (parent as FrameworkElement).Parent;
                }
                else
                {
                    parent = temp;
                }
            }

            return null;
        }
       
        void ScrollOwner_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (evenctcheck)
            {
                if (horizontalcheck)
                {
                    ScrollBar Horizontalthumb = (ScrollBar)(VisualTreeHelper.GetChild(sender as ScrollViewer, 0) as FrameworkElement).FindName("PART_HorizontalScrollBar"); //OverviewContentHolder.GetScrollBarControl(_ScrollOwner, 3);
                    if (Horizontalthumb.Track != null)
                    {
                        Horizontalthumb.Track.Thumb.DragDelta += new DragDeltaEventHandler(Thumb_DragDelta);
                        horizontalcheck = false;
                    }
                }

                if (verticalcheck)
                {
                    ScrollBar VerticalThumb = (ScrollBar)(VisualTreeHelper.GetChild(sender as ScrollViewer, 0) as FrameworkElement).FindName("PART_VerticalScrollBar"); //OverviewContentHolder.GetScrollBarControl(_ScrollOwner, 3);
                    if (VerticalThumb.Track != null)
                    {
                        VerticalThumb.Track.Thumb.DragDelta += new DragDeltaEventHandler(Thumb_DragDeltaVertical);
                        verticalcheck = false;
                    }
                }
                if (!horizontalcheck && !verticalcheck)
                { evenctcheck = false; }

            }
            if (this._ParentView != null)
            {
                if (!_ParentView.IsResizing)
                {
                    _ParentView.UpdatePreview();
                    if (!_ParentView.IsResized)
                    {
                        _ParentView.InvalidateScroll();
                        _ParentView.IsResized=false;
                    }
                }
                //_ParentView.HorizontalOffset = this.HorizontalOffset;
                //_ParentView.VerticalOffset = this.VerticalOffset;
            }
        }

        void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            horizontalcheck = false;
            if(_ParentView!=null)
            _ParentView.IsResized = false;
        }
        void Thumb_DragDeltaVertical(object sender, DragDeltaEventArgs e)
        {
            verticalcheck = false;
            if (_ParentView != null)
            _ParentView.IsResized = false;
        }

        Point PanStartPoint;
        internal  Overview _ParentView;
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (IsPanEnabled && PanStartPoint != null)
            {
                Point panPoint = e.MouseDevice.GetPosition(this);
                //_Offset.X = panPoint.X - PanStartPoint.X;
                //_Offset.Y= panPoint.Y - PanStartPoint.Y;
                //Pan(new Point(panPoint.X - this.PanStartPoint.X, panPoint.Y - this.PanStartPoint.Y));
            }
            base.OnMouseMove(e);
        }
        Point panPoint = new Point(0, 0);
        //internal void Pan(Point delta)
        //{
        //    this._Offset.X = -delta.X;
        //    this._Offset.Y = -delta.Y;
        //    if (this._Offset.X < 0)
        //    {
        //        panPoint.X = -this._Offset.X;
        //    }
        //    if (this._Offset.Y < 0)
        //    {
        //        panPoint.Y = -this._Offset.Y;
        //    }
        //    Top = panPoint.X;
        //    Left=panPoint.Y
        //    this.InvalidateArrange();
        //}

        public bool IsDragging = false;
        void OverviewContentHolder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IsDragging = true;
            if (IsPanEnabled)
            {
                PanStartPoint = e.MouseDevice.GetPosition(this);
            }
        }

        private static object GetFindTemplatedParent(DependencyObject obj)
        {
            return (object)obj.GetValue(FindTemplatedParentProperty);
        }

        private static void SetFindTemplatedParent(DependencyObject obj, object value)
        {
            obj.SetValue(FindTemplatedParentProperty, value);
        }

        // Using a DependencyProperty as the backing store for FindTemplatedParent.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty FindTemplatedParentProperty =
            DependencyProperty.RegisterAttached("FindTemplatedParent", typeof(object), typeof(OverviewContentHolder), new PropertyMetadata(null));
        public bool IsPanEnabled
        {
            get { return (bool)GetValue(IsPanEnabledProperty); }
            set { SetValue(IsPanEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsZoomInEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPanEnabledProperty =
            DependencyProperty.Register("IsPanEnabled", typeof(bool), typeof(OverviewContentHolder), new PropertyMetadata(false));

        void ZoomPanel_Loaded(object sender, RoutedEventArgs e)
        {
            DependencyObject obj = VisualTreeHelper.GetParent(this);
            
            Binding bin = new Binding();
            //bin.Source = obj;
            bin.RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent);
            (obj as FrameworkElement).SetBinding(OverviewContentHolder.FindTemplatedParentProperty, bin);

            obj = OverviewContentHolder.GetFindTemplatedParent(obj) as DependencyObject;

            if (obj != null)
            {
                MyParent = obj;
            }
            else
            {
                MyParent = VisualTreeHelper.GetParent(this);
            }

        }

        private EventHandler ScrollViewer_LayoutUpdated = null;
       

        //protected override void OnMouseWheel(MouseWheelEventArgs e)
        //{
        //    base.OnMouseWheel(e);
        //    //void ZoomPanel_MouseWheel(object sender, MouseWheelEventArgs e)
        //    {
        //        e.Handled = true;
        //        double x = (e.GetPosition(this).X * Scale - HorizontalOffset) / ViewportWidth;
        //        double y = (e.GetPosition(this).Y * Scale - VerticalOffset) / ViewportHeight;
        //        //this.Dispatcher.BeginInvoke(() =>
        //        {
        //            m_MouseZoom = true;
        //            if (e.Delta > 0)
        //            {
        //                DeltaScale((Scale + ZoomFactor) / Scale, new Point(x, y));
        //                Scale += ZoomFactor;
        //            }
        //            else
        //            {
        //                DeltaScale((Scale - ZoomFactor) / Scale, new Point(x, y));
        //                Scale -= ZoomFactor;
        //            }
        //            m_MouseZoom = false;
        //        }
        //        //);
        //        //}
        //    }
        //}

        private void DeltaScale(double increment, Point position_percent)
        {
            double SVh = (HorizontalOffset + ViewportWidth * position_percent.X) * increment; //ov.ScrollOwner.HorizontalOffset - (ov.ScrollOwner.ViewportWidth * ((double)e.NewValue - (double)e.OldValue + 1) - ov.ScrollOwner.ViewportWidth) / 2;
            //ov.ScrollOwner.ScrollToHorizontalOffset(delta);
            double SVv = (VerticalOffset + ViewportHeight * position_percent.Y) * increment;// - (ov.ScrollOwner.ViewportHeight * ((double)e.NewValue - (double)e.OldValue + 1) - ov.ScrollOwner.ViewportHeight) / 2;
            //ov.ScrollOwner.ScrollToVerticalOffset(delta);

            if (ScrollViewer_LayoutUpdated != null)
            {
                LayoutUpdated -= ScrollViewer_LayoutUpdated;
            }
            ScrollViewer_LayoutUpdated = (s, evt) =>
            {
                LayoutUpdated -= ScrollViewer_LayoutUpdated;
                ScrollToHorizontalOffset(SVh - ViewportWidth * position_percent.X);
                //HorizontalOffset = (SVh - ViewportWidth * position_percent.X);
                ScrollToVerticalOffset(SVv - ViewportHeight * position_percent.Y);
                //VerticalOffset = (SVv - ViewportHeight * position_percent.Y);
            };
            LayoutUpdated += ScrollViewer_LayoutUpdated;
            InvalidateArrange();
        }

        //private CompositeTransform ZoomPanTransform = new CompositeTransform();
        
        internal Transform OverviewTransform
        {
            get { return (Transform)GetValue(OverviewTransformProperty); }
            set { SetValue(OverviewTransformProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OverviewTransform.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty OverviewTransformProperty =
            DependencyProperty.Register("OverviewTransform", typeof(Transform), typeof(OverviewContentHolder), new PropertyMetadata(null));


        //private void UpdateTransform()
        //{
        //    ZoomPanTransform.ScaleX = ZoomPanTransform.ScaleY = UnitScale;
        //    if (!(MyParent is ScrollViewer))
        //    {
        //        ZoomPanTransform.TranslateX = HorizontalOffset;
        //        ZoomPanTransform.TranslateY = VerticalOffset;
        //    }
        //}

        internal DependencyObject MyParent
        {
            get { return (DependencyObject)GetValue(MyParentProperty); }
            set { SetValue(MyParentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyParent.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty MyParentProperty =
            DependencyProperty.Register("MyParent", typeof(DependencyObject), typeof(OverviewContentHolder), new PropertyMetadata(null, OnMyParentChanged));

        private static void OnMyParentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OverviewContentHolder pan = d as OverviewContentHolder;
            //pan.UpdateTransform();
            if (e.NewValue != null)
            {
                //pan.InternalBinding("RenderTransform.ScaleX", ScaleProperty);
                //pan.InternalBinding("RenderTransform.ScaleY", ScaleProperty);
                if (e.NewValue is ScrollViewer)
                {
                    ScrollViewer sv = e.NewValue as ScrollViewer;
                    //pan.InternalBinding("MyParent.HorizontalOffset", HorizontalOffsetProperty, BindingMode.TwoWay);
                    //pan.InternalBinding("MyParent.VerticalOffset", VerticalOffsetProperty, BindingMode.TwoWay);
                    //pan.InternalBinding("MyParent.ViewportWidth", ViewportWidthProperty, BindingMode.OneWay);
                    //pan.InternalBinding("MyParent.ViewportHeight", ViewportHeightProperty, BindingMode.OneWay);
                    //pan.InternalBinding("MyParent.ExtentWidth", ExtentWidthProperty, BindingMode.OneWay);
                    //pan.InternalBinding("MyParent.ExtentHeight", ExtentHeightProperty, BindingMode.OneWay);
                }
                else
                {
                    //    pan.InternalBinding("RenderTransform.TranslateX", HorizontalOffsetProperty);
                    //    pan.InternalBinding("RenderTransform.TranslateY", VerticalOffsetProperty);

                    //(pan.MyParent as FrameworkElement).SizeChanged -= new SizeChangedEventHandler(pan.MyParent_SizeChanged);
                    //(pan.MyParent as FrameworkElement).SizeChanged += new SizeChangedEventHandler(pan.MyParent_SizeChanged);

                    //pan.InternalBinding("MyParent.DesiredSize.Width", ViewportWidthProperty);
                    //pan.InternalBinding("MyParent.DesiredSize.Height", ViewportHeightProperty);
                    //pan.InternalBinding("DesiredSize.Width", ExtentWidthProperty);
                    //pan.InternalBinding("DesiredSize.Height", ExtentHeightProperty);

                    //pan.InternalBinding(CompositeTransform.TranslateXProperty, "HorizontalOffset", pan.RenderTransform);
                    //pan.InternalBinding(CompositeTransform.TranslateYProperty, "VerticalOffset", pan.RenderTransform);

                    //pan.InternalBinding(FrameworkElement.ActualWidthProperty, "ViewportWidth", pan.MyParent);
                    //pan.InternalBinding(FrameworkElement.ActualHeightProperty, "ViewportHeight", pan.MyParent);
                    //pan.InternalBinding(FrameworkElement.ActualWidthProperty, "ExtentWidth", pan);
                    //pan.InternalBinding(FrameworkElement.ActualHeightProperty, "ExtentHeight", pan);
                    //(pan.MyParent as Grid).DesiredSize.Width
                }
            }
            //pan._ScrollOwner = pan.MyParent as ScrollViewer;
            //pan.VerifyScrollData(new Size(pan.ScrollOwner.ViewportWidth,pan.ScrollOwner.ViewportHeight),new Size(pan.ScrollOwner.ExtentWidth,pan.ScrollOwner.ExtentHeight));
            pan.InvalidateMeasure();
        }
        
        private void InternalBinding(string sourceProp, DependencyProperty dpProp, BindingMode mode)
        {
            Binding bin = new Binding(sourceProp);
            bin.Mode = mode;
            if (mode == BindingMode.TwoWay)
            {
                //bin.Converter = new ScrollViewerOffsetConverter();                
            }
            bin.Source = this;
            SetBinding(dpProp, bin);
        }

        //private void InternalBinding(DependencyProperty dpProp, string sourceProp, DependencyObject source)
        //{
        //    Binding bin = new Binding(sourceProp);
        //    bin.Converter = new ReverseTransformConverter();
        //    bin.Mode = BindingMode.TwoWay;
        //    bin.Source = this;
        //    BindingOperations.SetBinding(source, dpProp, bin);
        //}

        //#region ScrollViewer

        //public virtual double HorizontalOffset
        //{
        //    get { return (double)GetValue(HorizontalOffsetProperty); }
        //    //private set { SetValue(HorizontalOffsetProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for HorizontalOffset.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty HorizontalOffsetProperty =
        //    DependencyProperty.Register("HorizontalOffset", typeof(double), typeof(OverviewContentHolder), new PropertyMetadata(0d, OnHVChanged));

        //internal virtual void ScrollToHorizontalOffset(double offset)
        //{
        //    if (MyParent is ScrollViewer)
        //    {
        //        (MyParent as ScrollViewer).SetSafeHorizontalOffset(offset);
        //    }
        //    else
        //    {
        //        this.SetValue(HorizontalOffsetProperty, offset);
        //    }
        //}
        
        //public virtual double VerticalOffset
        //{
        //    get { return (double)GetValue(VerticalOffsetProperty); }
        //    //private set { SetValue(VerticalOffsetProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for VerticalOffset.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty VerticalOffsetProperty =
        //    DependencyProperty.Register("VerticalOffset", typeof(double), typeof(OverviewContentHolder), new PropertyMetadata(0d, OnHVChanged));

        //internal virtual void ScrollToVerticalOffset(double offset)
        //{
        //    if (MyParent is ScrollViewer)
        //    {
        //        (MyParent as ScrollViewer).SetSafeVerticalOffset(offset);
        //    }
        //    else
        //    {
        //        this.SetValue(VerticalOffsetProperty, offset);
        //    }
        //}

        //private static void OnHVChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    OverviewContentHolder panel = d as OverviewContentHolder;
        //    if (panel.MyParent is ScrollViewer)
        //    {
        //        panel.MyParent.Dispatcher.BeginInvoke(new Action(() =>
        //            {
        //                (panel.MyParent as ScrollViewer).SetSafeHorizontalOffset(panel.HorizontalOffset);
        //                (panel.MyParent as ScrollViewer).SetSafeVerticalOffset(panel.VerticalOffset);
        //            }
        //        ));
        //    }
        //    else
        //    {
        //        panel.InvalidateArrange();
        //    }
        //}

        //public virtual double ViewportWidth
        //{
        //    get { return (double)GetValue(ViewportWidthProperty); }
        //    private set { SetValue(ViewportWidthProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for ViewportWidth.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty ViewportWidthProperty =
        //    DependencyProperty.Register("ViewportWidth", typeof(double), typeof(OverviewContentHolder), new PropertyMetadata(0d, OnVPSizeChanged));
        
        //public virtual double ViewportHeight
        //{
        //    get { return (double)GetValue(ViewportHeightProperty); }
        //    private set { SetValue(ViewportHeightProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for ViewportHeight.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty ViewportHeightProperty =
        //    DependencyProperty.Register("ViewportHeight", typeof(double), typeof(OverviewContentHolder), new PropertyMetadata(0d, OnVPSizeChanged));
        
        //private static void OnVPSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //}

        //public virtual double ExtentWidth
        //{
        //    get { return (double)GetValue(ExtentWidthProperty); }
        //    private set { SetValue(ExtentWidthProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for ExtentWidth.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty ExtentWidthProperty =
        //    DependencyProperty.Register("ExtentWidth", typeof(double), typeof(OverviewContentHolder), new PropertyMetadata(0d));

        //public virtual double ExtentHeight
        //{
        //    get { return (double)GetValue(ExtentHeightProperty); }
        //    private set { SetValue(ExtentHeightProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for ExtentHeight.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty ExtentHeightProperty =
        //    DependencyProperty.Register("ExtentHeight", typeof(double), typeof(OverviewContentHolder), new PropertyMetadata(0d));

        //#endregion

        #region IZoomPanel Implementation

        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        internal double UnitScale
        {
            get
            {
                if (this.ZoomMode == Shared.ZoomMode.Unit)
                {
                    return Scale;
                }
                else
                {
                    return Scale / 100;
                }
            }
            set
            {
                if (this.ZoomMode == Shared.ZoomMode.Unit)
                {
                    Scale = value;
                }
                else
                {
                    Scale = value * 100;
                }
            }
        }

        // Using a DependencyProperty as the backing store for Scale.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(double), typeof(OverviewContentHolder), new PropertyMetadata(1d, OnScaleChanged));

        private static void OnScaleChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            OverviewContentHolder ov = dp as OverviewContentHolder;
            (ov.OverviewTransform as ScaleTransform).ScaleX = (double)e.NewValue;
            (ov.OverviewTransform as ScaleTransform).ScaleY = (double)e.NewValue;
            ov.InvalidateMeasure();
            ov.InvalidateArrange();
            //if (!ov.m_MouseZoom)
            //{
            //    double delta = (double)e.NewValue / (double)e.OldValue;
            //    ov.DeltaScale(delta, new Point(0.5, 0.5));
            //}
        }


        public bool IsZoomInEnabled
        {
            get { return (bool)GetValue(IsZoomInEnabledProperty); }
            set { SetValue(IsZoomInEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsZoomInEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsZoomInEnabledProperty =
            DependencyProperty.Register("IsZoomInEnabled", typeof(bool), typeof(OverviewContentHolder), new PropertyMetadata(true));

        public ICommand ZoomIn
        {
            get { return (ICommand)GetValue(ZoomInProperty); }
            set { SetValue(ZoomInProperty, value); }
        }

        public ICommand FitToPageCommand
        {
            get { return (ICommand)GetValue(FitToPageCommandProperty); }
            set { SetValue(FitToPageCommandProperty, value); }
        }
        // Using a DependencyProperty as the backing store for ZoomIn.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FitToPageCommandProperty =
            DependencyProperty.Register("FitToPageCommand", typeof(ICommand), typeof(OverviewContentHolder), new PropertyMetadata(null));


        // Using a DependencyProperty as the backing store for ZoomIn.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZoomInProperty =
            DependencyProperty.Register("ZoomIn", typeof(ICommand), typeof(OverviewContentHolder), new PropertyMetadata(null));

        public bool IsZoomOutEnabled
        {
            get { return (bool)GetValue(IsZoomOutEnabledProperty); }
            set { SetValue(IsZoomOutEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsZoomOutEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsZoomOutEnabledProperty =
            DependencyProperty.Register("IsZoomOutEnabled", typeof(bool), typeof(OverviewContentHolder), new PropertyMetadata(true));

        public ICommand ZoomOut
        {
            get { return (ICommand)GetValue(ZoomOutProperty); }
            set { SetValue(ZoomOutProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZoomOut.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZoomOutProperty =
            DependencyProperty.Register("ZoomOut", typeof(ICommand), typeof(OverviewContentHolder), new PropertyMetadata(null));

        public bool IsZoomToEnabled
        {
            get { return (bool)GetValue(IsZoomToEnabledProperty); }
            set { SetValue(IsZoomToEnabledProperty, value); }
        }

        bool isfitpage=true;
        // Using a DependencyProperty as the backing store for IsZoomToEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsZoomToEnabledProperty =
            DependencyProperty.Register("IsZoomToEnabled", typeof(bool), typeof(OverviewContentHolder), new PropertyMetadata(true));

        public ICommand ZoomTo
        {
            get { return (ICommand)GetValue(ZoomToProperty); }
            set { SetValue(ZoomToProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZoomTo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZoomToProperty =
            DependencyProperty.Register("ZoomTo", typeof(ICommand), typeof(OverviewContentHolder), new PropertyMetadata(null));

        public bool IsZoomResetEnabled
        {
            get { return (bool)GetValue(IsZoomResetEnabledProperty); }
            set { SetValue(IsZoomResetEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsZoomResetEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsZoomResetEnabledProperty =
            DependencyProperty.Register("IsZoomResetEnabled", typeof(bool), typeof(OverviewContentHolder), new PropertyMetadata(true));

        public ICommand ZoomReset
        {
            get { return (ICommand)GetValue(ZoomResetProperty); }
            set { SetValue(ZoomResetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZoomReset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZoomResetProperty =
            DependencyProperty.Register("ZoomReset", typeof(ICommand), typeof(OverviewContentHolder), new PropertyMetadata(null));

        public double ZoomFactor
        {
            get { return (double)GetValue(ZoomFactorProperty); }
            set { SetValue(ZoomFactorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZoomFactor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZoomFactorProperty =
            DependencyProperty.Register("ZoomFactor", typeof(double), typeof(OverviewContentHolder), new PropertyMetadata(0.2d));        

        public double MinimumZoom
        {
            get { return (double)GetValue(MinimumZoomProperty); }
            set { SetValue(MinimumZoomProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinimumZoom.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinimumZoomProperty =
            DependencyProperty.Register("MinimumZoom", typeof(double), typeof(OverviewContentHolder), new PropertyMetadata(0.2d));

        public double MaximumZoom
        {
            get { return (double)GetValue(MaximumZoomProperty); }
            set { SetValue(MaximumZoomProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaximumZoom.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximumZoomProperty =
            DependencyProperty.Register("MaximumZoom", typeof(double), typeof(OverviewContentHolder), new PropertyMetadata(10d));

        public ZoomMode ZoomMode
        {
            get { return (ZoomMode)GetValue(ZoomModeProperty); }
            set { SetValue(ZoomModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZoomMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZoomModeProperty =
            DependencyProperty.Register("ZoomMode", typeof(ZoomMode), typeof(OverviewContentHolder), new PropertyMetadata(ZoomMode.Unit));
        public double Left
        {
            get { return (double)GetValue(LeftProperty); }
            set { SetValue(LeftProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Left.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty LeftProperty =
            DependencyProperty.Register("Left", typeof(double), typeof(OverviewContentHolder), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsParentArrange | FrameworkPropertyMetadataOptions.AffectsParentMeasure, OnLeftChanged));

        private static void OnLeftChanged(DependencyObject d, DependencyPropertyChangedEventArgs evtArgs)
        {
            OverviewContentHolder pan = d as OverviewContentHolder;
            pan.TopLeft = new Point(pan.Left, pan.Top);
            pan._Offset.X += (double)evtArgs.NewValue - (double)evtArgs.OldValue;
             
                //((page.dview.Scrollviewer as ScrollViewer).Content as OverviewContentHolder).TopLeft = new Point(((page.dview.Scrollviewer as ScrollViewer).Content as OverviewContentHolder).TopLeft.X + (double)evtArgs.NewValue - (double)evtArgs.OldValue,((page.dview.Scrollviewer as ScrollViewer).Content as OverviewContentHolder).TopLeft.Y);
       
        }

        public double Top
        {
            get { return (double)GetValue(TopProperty); }
            set { SetValue(TopProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Top.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty TopProperty =
            DependencyProperty.Register("Top", typeof(double), typeof(OverviewContentHolder), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsParentArrange | FrameworkPropertyMetadataOptions.AffectsParentMeasure, OnTopChanged));
        private static void OnTopChanged(DependencyObject d, DependencyPropertyChangedEventArgs evtArgs)
        {
            OverviewContentHolder pan = d as OverviewContentHolder;
            pan.TopLeft = new Point(pan.Left, pan.Top);
            pan._Offset.Y += (double)evtArgs.NewValue - (double)evtArgs.OldValue;
            OverViewRoutedEventArgs newevent = new OverViewRoutedEventArgs((double)evtArgs.NewValue, "Top");
            newevent.RoutedEvent = OverviewContentHolder.PropertyChangeEvent;
            pan.RaiseEvent(newevent);
            //((page.dview.Scrollviewer as ScrollViewer).Content as OverviewContentHolder).TopLeft=new Point(((page.dview.Scrollviewer as ScrollViewer).Content as OverviewContentHolder).TopLeft.X, ((page.dview.Scrollviewer as ScrollViewer).Content as OverviewContentHolder).TopLeft.Y + (double)evtArgs.NewValue - (double)evtArgs.OldValue);
         
        }
        public Point TopLeft
        {
            get { return (Point)GetValue(TopLeftProperty); }
            set { SetValue(TopLeftProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TopLeft.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TopLeftProperty =
            DependencyProperty.Register("TopLeft", typeof(Point), typeof(OverviewContentHolder), new PropertyMetadata(new Point(0, 0), OnTopLeftChanged));

        private static void OnTopLeftChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OverviewContentHolder pan = d as OverviewContentHolder;
            //pan.m_VScroll.Minimum = pan.TopLeft.Y;
            //pan.m_HScroll.Minimum = pan.TopLeft.X;
            //pan._Offset.X = pan.TopLeft.X;
            //pan._Offset.Y = pan._Offset.Y + pan.TopLeft.Y;
            (pan.Content as FrameworkElement).InvalidateMeasure();
            pan.InvalidateMeasure();
        }

        #endregion

        private bool CanZoomInExecute(object parameter)
        {
            return IsZoomInEnabled;
        }

        private bool CanZoomOutExecute(object parameter)
        {
            return IsZoomOutEnabled;
        }

        private bool CanZoomToExecute(object parameter)
        {
            return IsZoomToEnabled;
        }

        private bool CanFitPageExecute(object parameter)
        {
            return isfitpage;
        }
        private bool CanZoomResetExecute(object parameter)
        {
            return IsZoomResetEnabled;
        }

        private void ExecuteZoomInCommand(object parameter)
        {
            if (parameter is IZoomParameter)
            {
                if (UnitScale + (parameter as IZoomParameter).ZoomFactor <= 30)
                {
                    double x = ScrollOwner.ScrollableWidth;
                    double y = ScrollOwner.ScrollableHeight;
                    UnitScale += (parameter as IZoomParameter).ZoomFactor;
                    double xchange = (((parameter as IZoomParameter).ZoomPoint.X * Scale) - (parameter as IZoomParameter).ZoomPoint.X - HorizontalOffset * Scale);
                    double ychange = (((parameter as IZoomParameter).ZoomPoint.Y * Scale) - (parameter as IZoomParameter).ZoomPoint.Y - VerticalOffset * Scale);
                    SetHorizontalOffset(xchange);
                    SetVerticalOffset(ychange);
                }
            }
            else
            {
                if (UnitScale +ZoomFactor <= 30)
                     UnitScale += ZoomFactor;
            }

        }

        private void ExecuteFitPageCommand(object parameter)
        {
            
        }
        private void ExecuteZoomOutCommand(object parameter)
        {
                if (parameter is IZoomParameter)
                {
                    if (UnitScale - (parameter as IZoomParameter).ZoomFactor >= MinimumZoom)
                    {
                        UnitScale -= (parameter as IZoomParameter).ZoomFactor;
                    }
                }
                else
                {
                    if (UnitScale - ZoomFactor >= MinimumZoom)
                    UnitScale -= ZoomFactor;
                }
        }

        private void ExecuteZoomResetCommand(object parameter)
        {
            UnitScale = 1d;
        }

        private void ExecuteZoomToCommand(object parameter)
        {
            if (parameter is IZoomParameter)
            {
                UnitScale = (parameter as IZoomParameter).ZoomTo;
            }
        }
        Size INeed;
        Size ButIHave;
        //internal Size Constraint;
        protected override Size MeasureOverride(Size availableSize)
        {
            INeed = base.MeasureOverride(availableSize);     
            foreach (FrameworkElement ele in GetChildren())
            {
                ele.Measure(INeed);  
                // new Size(finalSize.Width / UnitScale, finalSize.Height / UnitScale)))
                //if (ButIHave.Width < ele.DesiredSize.Width)
                //{
                //    ButIHave.Width = ele.DesiredSize.Width;
                //    ButIHave.Height = ele.DesiredSize.Height;
                //}  
                if ((ele as ContentPresenter).Content.ToString().Contains("FourQuadrantPanel"))
                {
                    ele.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                }
                else
                {
                    ele.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                }              
                   
                INeed = new Size(ele.DesiredSize.Width, ele.DesiredSize.Height);
            }
            ButIHave = availableSize;
            //INeed.Height = ButIHave.Height > INeed.Height ? ButIHave.Height : INeed.Height;
            //INeed.Width = ButIHave.Width > INeed.Width ? ButIHave.Width : INeed.Width;
            VerifyScrollData(ButIHave, INeed);           
            //if (!(MyParent is ScrollViewer))
            //{
            //    ButIHave.Height = double.IsInfinity(availableSize.Height)? 0d : availableSize.Height;
            //    ButIHave.Width = double.IsInfinity(availableSize.Width) ? 0d : availableSize.Width;
            //}
            //Size desiredSize = base.MeasureOverride(new Size(ExtentWidth,ExtentHeight));// new Size(availableSize.Width / UnitScale, availableSize.Height / UnitScale));
            ////return new Size(desiredSize.Width * UnitScale, desiredSize.Height * UnitScale);
            //Size desiredSize = base.MeasureOverride(availableSize);
            //if (!(MyParent is ScrollViewer))
            //{
            //    //ExtentWidth = desiredSize.Width * UnitScale;
            //    //ExtentHeight = desiredSize.Height * UnitScale;
            //}
            ////if (UnitScale >= 1)
            //{
            //    return desiredSize;// (new Size(desiredSize.Width * UnitScale, desiredSize.Height * UnitScale));
            //}
            //else
            //{
            //    return (new Size(desiredSize.Width, desiredSize.Height));
            //}
            return INeed;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            //Size actualSize = base.ArrangeOverride(new Size(finalSize.Width / UnitScale, finalSize.Height / UnitScale));
            //return new Size(actualSize.Width * UnitScale, actualSize.Height * UnitScale);
            //ButIHave = finalSize; 
            foreach (UIElement ele in GetChildren())
            {
                double width = ExtentWidth > ViewportWidth ? ExtentWidth : ViewportWidth;
                double height = ExtentHeight > ViewportHeight ? ExtentHeight : ViewportHeight;
                //ele.Arrange(new Rect(new Point(Left - HorizontalOffset, -VerticalOffset), new Size(ExtentWidth, ExtentHeight)));
                ele.Arrange(new Rect(new Point(-HorizontalOffset, -VerticalOffset), new Size(width+Left, height+Top)));
                //ele.Arrange(new Rect(new Point(-HorizontalOffset / UnitScale, -VerticalOffset / UnitScale), new Size(ele.DesiredSize.Width, ele.DesiredSize.Height)));  // new Size(finalSize.Width / UnitScale, finalSize.Height / UnitScale)));
            }

            VerifyScrollData(ButIHave, INeed); 
            OverViewFitToPageEventArgs newevent = new OverViewFitToPageEventArgs(true);
            newevent.RoutedEvent = OverviewContentHolder.FitToPageEvent;
            newevent.Source = this;
            RaiseEvent(newevent);
            if (newevent.Cancel&&this.EnableFitToPage)
            {
                this.FitToPage();
            }
            //else
            //{
            //base.ArrangeOverride(new Size(ExtentWidth,ExtentHeight));

            //}
            //return  new Size(finalSize.Width / UnitScale, finalSize.Height / UnitScale);
            double finalwidth = ExtentWidth > ViewportWidth ? ExtentWidth : ViewportWidth;
            double fianlheight = ExtentHeight > ViewportHeight ? ExtentHeight : ViewportHeight;
            return new Size(finalwidth, fianlheight);
        }

        private IEnumerable GetChildren()
        {
            int count = VisualTreeHelper.GetChildrenCount(this);
            for (int i = 0; i < count; i++)
            {
                yield return VisualTreeHelper.GetChild(this, i);
            }
        }

        internal void ScrollToHorizontalOffset(double offx)
        {
            SetHorizontalOffset(offx);
        }
        internal void ScrollToVerticalOffset(double offy)
        {
            SetVerticalOffset(offy);
        }
        
        public void FitToPage()
        {

            double width = (ViewportWidth*Scale) / (ExtentWidth);
            double height = (ViewportHeight*Scale) / (ExtentHeight);            
            // Set the value of daigramView's CurrentZoom property
            // to get Fit to Screen            
            this.SetValue(ScaleProperty, width < height ? width : height);
            SetHorizontalOffset(0);
            SetVerticalOffset(0);
        }

        #region Iscroll 
        public void LineDown()
        {
            
            SetVerticalOffset(VerticalOffset + LineSize);
        }

        public void LineUp()
        {
            
            SetVerticalOffset(VerticalOffset - LineSize);
        }

        public void LineLeft()
        {
           
            SetHorizontalOffset(HorizontalOffset - LineSize);
        }

        public void LineRight()
        {
            
            SetHorizontalOffset(HorizontalOffset + LineSize);
        }

        public void MouseWheelDown()
        {
            
            SetVerticalOffset(VerticalOffset + WheelSize);
        }

        public void MouseWheelUp()
        {

            SetVerticalOffset(VerticalOffset - WheelSize);
        }

        public void MouseWheelLeft()
        {
            
            SetHorizontalOffset(HorizontalOffset - WheelSize);
        }

        public void MouseWheelRight()
        {
            
            SetHorizontalOffset(HorizontalOffset + WheelSize);
        }

        public void PageDown()
        {
           
            SetVerticalOffset(VerticalOffset + ViewportHeight);
        }

        public void PageUp()
        {
            
            SetVerticalOffset(VerticalOffset - ViewportHeight);
        }

        public void PageLeft()
        {
            
            SetHorizontalOffset(HorizontalOffset - ViewportWidth);

        }

        public void PageRight()
        {
            
            SetHorizontalOffset(HorizontalOffset + ViewportWidth);
        }

        public ScrollViewer ScrollOwner
        {
            get { return _ScrollOwner; }
            set
            {
                if (value != null)
                _ScrollOwner = value;
            }
        }

        public bool CanHorizontallyScroll
        {
            get { return _CanHorizontallyScroll; }
            set { _CanHorizontallyScroll = value; }
        }

        public bool CanVerticallyScroll
        {
            get { return _CanVerticallyScroll; }
            set { _CanVerticallyScroll = value; }
        }

        public double ExtentHeight
        {
            get
            {
                if (TopLeft.Y > 0)
                {
                    return Math.Max(_Viewport.Height + _Offset.Y, _Extent.Height);
                }
                else
                {
                    return _Extent.Height;
                }
                
            }
        }

        public double ExtentWidth
        {
            get
            {
                if (TopLeft.X > 0)
                {
                    return Math.Max(_Viewport.Width + _Offset.X, _Extent.Width);
                }
                else
                {
                    return _Extent.Width;
                }
                
            }
        }

        public double HorizontalOffset
        {
            get
            {
               
                return _Offset.X;
            }
        }

        public double VerticalOffset
        {
            get
            {
                
                return _Offset.Y;
            }
        }

        public double ViewportHeight
        { get { return _Viewport.Height; } }

        public double ViewportWidth
        { get { return _Viewport.Width; } }

       

        protected void VerifyScrollData(Size viewport, Size extent)
        {
            if (double.IsInfinity(viewport.Width))
            { viewport.Width = extent.Width; }

            if (double.IsInfinity(viewport.Height))
            { viewport.Height = extent.Height; }

            _Extent = extent;
            _Viewport = viewport;

            _Offset.X = Math.Max(0,
              Math.Min(_Offset.X, ExtentWidth - ViewportWidth));
            _Offset.Y = Math.Max(0,
              Math.Min(_Offset.Y, ExtentHeight - ViewportHeight));
            
            if (ScrollOwner != null)
            { ScrollOwner.InvalidateScrollInfo(); }
        }

        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            if (rectangle.IsEmpty || visual == null
              || visual == this || !base.IsAncestorOf(visual))
            { return Rect.Empty; }
            rectangle = visual.TransformToAncestor(this).TransformBounds(rectangle);
            Rect viewRect = new Rect(HorizontalOffset, VerticalOffset, ViewportWidth, ViewportHeight);
            rectangle.X += viewRect.X;
            rectangle.Y += viewRect.Y;
            viewRect.X = CalculateNewVisibleArea(viewRect.Left, viewRect.Right, rectangle.Left, rectangle.Right);
            viewRect.Y = CalculateNewVisibleArea(viewRect.Top, viewRect.Bottom, rectangle.Top, rectangle.Bottom);
            SetHorizontalOffset(viewRect.X);
            SetVerticalOffset(viewRect.Y);
            rectangle.Intersect(viewRect);
            rectangle.X -= viewRect.X;
            rectangle.Y -= viewRect.Y;
            return rectangle;
        }

        internal void ZoomVisible(Point zoompoint)
        {
            Rect viewRect = new Rect(HorizontalOffset, VerticalOffset, ViewportWidth, ViewportHeight);

            SetHorizontalOffset(viewRect.X);
            SetVerticalOffset(viewRect.Y);
        }
        private bool _CanHorizontallyScroll;
        private bool _CanVerticallyScroll;
        private ScrollViewer _ScrollOwner;
        [CLSCompliant(false)]
        public Vector _Offset;
        private Size _Extent;
        private Size _Viewport;
        private const double LineSize = 16;
        private const double WheelSize = 3 * LineSize;

       
        private double CalculateNewVisibleArea(double top1, double bottom1, double top2, double bottom2)
        {
            bool offBottom = top2 < top1 && bottom2 < bottom1;
            bool offTop = bottom2 > bottom1 && top2 > top1;
            bool tooLarge = (bottom2 - top2) > (bottom1 - top1);

            if (!offBottom && !offTop)
            { return top1; }

            if ((offBottom && !tooLarge) || (offTop && tooLarge))
            { return top2; }

            return (bottom2 - (bottom1 - top1));
        }
        public void SetHorizontalOffset(double offset)
        {
            offset = Math.Max(0,
              Math.Min(offset, ExtentWidth - ViewportWidth));
            if (offset != _Offset.X)
            {
                _Offset.X = offset;
                InvalidateArrange();
            }
        }

        public void SetVerticalOffset(double offset)
        {
            offset = Math.Max(0,
              Math.Min(offset, ExtentHeight - ViewportHeight));
            if (offset != _Offset.Y)
            {
                _Offset.Y = offset;
                InvalidateArrange();
            }
        }
        #endregion

        /// <summary>
        ///  PropertyChange Routed event. 
        /// </summary>
         public static readonly RoutedEvent PropertyChangeEvent = EventManager.RegisterRoutedEvent(
          "PropertyChange", RoutingStrategy.Bubble, typeof(OverviewEventHandler), typeof(OverviewContentHolder));
        public event OverviewEventHandler PropertyChange
        {
            add { AddHandler(PropertyChangeEvent, value); }
            remove { RemoveHandler(PropertyChangeEvent, value); }
        }

        public delegate void OverviewEventHandler(object sender, OverViewRoutedEventArgs evtArgs);
        public class OverViewRoutedEventArgs : RoutedEventArgs
        {
           

            public string PropertyStringValue
            {
                get;
                set;
            }

            public OverViewRoutedEventArgs(double value,String property)
            {
                this.Value = value;
                this.PropertyStringValue = property;
            }
            double _value;

            /// <summary>
            /// Gets or sets the Node object.
            /// <value>
            /// Node object.<see cref="Value"/>
            /// </value>
            /// </summary>
            public double Value
            {
                get
                {
                    return _value;
                }

                set
                {
                    _value = value;
                }
            }
        }


        /// <summary>
        ///  FitToPage Routed event. 
        /// </summary>
        public static readonly RoutedEvent FitToPageEvent = EventManager.RegisterRoutedEvent(
         "UpdateFitToPage", RoutingStrategy.Bubble, typeof(OverviewFitPageEventHandler), typeof(OverviewContentHolder));
        public event OverviewFitPageEventHandler UpdateFitToPage
        {
            add { AddHandler(FitToPageEvent, value); }
            remove { RemoveHandler(FitToPageEvent, value); }
        }
        public delegate void OverviewFitPageEventHandler(object sender, OverViewFitToPageEventArgs evtArgs);
        public class OverViewFitToPageEventArgs : RoutedEventArgs
        {
            bool cc=false;
            public bool Cancel
            {
                get
                {
                    return cc;
                }
                set
                {
                    if (value&&this.Source!=null &&(this.Source as OverviewContentHolder).EnableFitToPage)
                    {
                        (this.Source as OverviewContentHolder).FitToPage();
                    }
                    cc=value;
                }
            }
            public OverViewFitToPageEventArgs(bool fittopage)
            {

                this.Cancel = fittopage;
            }

        }
    }

}
