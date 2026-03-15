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
using System.Windows.Markup;
using System.Collections.Generic;
using Syncfusion.Windows.Tools.Controls;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// 
    /// </summary>
    [ContentProperty("Content")]
    public class OverviewContentHolder : Control, IOverviewPanel, IScrollInfo
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Point GetOrigin(DependencyObject obj)
        {
            return (Point)obj.GetValue(OriginProperty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetOrigin(DependencyObject obj, Point value)
        {
            obj.SetValue(OriginProperty, value);
        }

        // Using a DependencyProperty as the backing store for Start.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty OriginProperty =
            DependencyProperty.RegisterAttached("Origin", typeof(Point), typeof(OverviewContentHolder), new PropertyMetadata(new Point(0,0)));

        /// <summary>
        /// 
        /// </summary>
        public bool EnableFitToPage
        {
            get { return (bool)GetValue(EnableFitToPageProperty); }
            set { SetValue(EnableFitToPageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllowResize.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty EnableFitToPageProperty =
            DependencyProperty.Register("EnableFitToPage", typeof(bool), typeof(OverviewContentHolder), new PropertyMetadata(false, new PropertyChangedCallback(OnFitToPageEnabled)));
       
        
        private static void OnFitToPageEnabled(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OverviewContentHolder contentholder = (OverviewContentHolder)d;
            contentholder.ZoomReset.Execute(contentholder);
            contentholder.FitToPage();
        }


        internal Point TopLeft
        {
            get { return (Point)GetValue(TopLeftProperty); }
            set { SetValue(TopLeftProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TopLeft.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty TopLeftProperty =
            DependencyProperty.Register("TopLeft", typeof(Point), typeof(OverviewContentHolder), new PropertyMetadata(new Point(0, 0), OnTopLeftChanged));

        static private void OnTopLeftChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OverviewContentHolder ov = d as OverviewContentHolder;
            Point oldValue = (Point)e.OldValue;
            Point newValue = (Point)e.NewValue;
            double xChange = oldValue.X - newValue.X;
            double yChange = oldValue.Y - newValue.Y;
            ov.m_HorizontalOffset -= xChange;
            ov.m_VerticalOffset -= yChange;
            ov.UpdatePageBackground();
        }
        /// <summary>
        /// 
        /// </summary>
        public Brush PageBackground
        {
            get { return (Brush)GetValue(PageBackgroundProperty); }
            set { SetValue(PageBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PageBackground.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty PageBackgroundProperty =
            DependencyProperty.Register("PageBackground", typeof(Brush), typeof(OverviewContentHolder), new PropertyMetadata(null));
        /// <summary>
        /// 
        /// </summary>
        public bool AllowResize
        {
            get { return (bool)GetValue(AllowResizeProperty); }
            set { SetValue(AllowResizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllowResize.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty AllowResizeProperty =
            DependencyProperty.Register("AllowResize", typeof(bool), typeof(OverviewContentHolder), new PropertyMetadata(false));
        /// <summary>
        /// 
        /// </summary>
        public OverviewContentHolder()
        {
            this.DefaultStyleKey = typeof(OverviewContentHolder);

            ZoomIn = new DelegateCommand<object>(ExecuteZoomInCommand, CanZoomInExecute);
            ZoomOut = new DelegateCommand<object>(ExecuteZoomOutCommand, CanZoomOutExecute);
            ZoomTo = new DelegateCommand<object>(ExecuteZoomToCommand, CanZoomToExecute);
            ZoomReset = new DelegateCommand<object>(ExecuteZoomResetCommand, CanZoomResetExecute);
            #if SyncfusionFramework4_0
            this.UseLayoutRounding = false;
            this.Loaded += new RoutedEventHandler(OverviewContentHolder_Loaded);
            #endif

        }
        /// <summary>
        /// 
        /// </summary>
        internal Overview _overviewParent;
        void OverviewContentHolder_Loaded(object sender, RoutedEventArgs e)
        {
            if(_overviewParent!=null)
            _overviewParent.InvalidateScroll();
        }

        //internal event ScrollChangedEventHandler ScrollChanged;
        /// <summary>
        /// 
        /// </summary>
        public UIElement Content
        {
            get { return (UIElement)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(UIElement), typeof(OverviewContentHolder), new PropertyMetadata(null, OnContentChanged));

        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as OverviewContentHolder).OnContentChanged(e.OldValue, e.NewValue);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldContent"></param>
        /// <param name="newContent"></param>
        protected virtual void OnContentChanged(object oldContent, object newContent)
        {
            if (this.Content != null)
            {
#if SILVERLIGHT
                Binding customBinding = (Binding)System.Windows.Markup.XamlReader.Load(
                                        "<Binding xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"" +
                                                    " xmlns:mns=\"clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Shared.Silverlight\"" +
                                                    " Path=\"(mns:OverviewContentHolder.Origin)\" />");
#endif
#if WPF
                Binding customBinding = (Binding)XamlReader.Parse(
                                        "<Binding xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"" +
                                                    " xmlns:mns=\"clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Shared.Wpf\"" +
                                                    " Path=\"(mns:OverviewContentHolder.Origin)\" />");
#endif
                customBinding.Source = this.Content;
                //customBinding.Path = new PropertyPath("OverviewContentHolder.Origin");
                customBinding.Converter = new Inverter();
                this.SetBinding(TopLeftProperty, customBinding);
                //Content.RenderTransform = this.OverviewTransform;
            }
        }

        internal class Inverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                if (value is Point)
                {
                    Point pt = (Point)value;
                    return new Point(-pt.X, -pt.Y);
                }
                else
                {
                    return null;
                }
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        private Stack<ZoomOperation> zoomStack = new Stack<ZoomOperation>();

        internal class ZoomOperation
        {
            internal Point mousePos;
            internal Point ratio;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            PerformZoom(e);
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsZoomEnabled
        {
            get { return (bool)GetValue(IsZoomEnabledProperty); }
            set { SetValue(IsZoomEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsZoomEnabled.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsZoomEnabledProperty =
            DependencyProperty.Register("IsZoomEnabled", typeof(bool), typeof(OverviewContentHolder), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(IsZoomEnabledChanged)));

        private static void IsZoomEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private void PerformZoom(MouseEventArgs e)
        {
            //ZoomOperation oper = new ZoomOperation();
            //if (zoomStack.Count == 0)
            //{
            //    oper.mousePos = GetMousePosition(e, UnitScale);
            //    oper.ratio = new Point(oper.mousePos.X / ExtentWidth, oper.mousePos.Y / ExtentHeight);
            //}     

            if (e.CanZoomIn(this))
            {
                if (IsZoomInEnabled && IsZoomEnabled)
                {
                    ZoomIn.Execute(new ZoomParamenter() { FocusPoint = GetMousePosition(e, 1) });
                }
                else
                {
                    return;
                }
            }
            else if (e.CanZoomOut(this))
            {
                if (IsZoomOutEnabled && IsZoomEnabled)
                {
                    ZoomOut.Execute(new ZoomParamenter() { FocusPoint = GetMousePosition(e, 1) });
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
            MouseWheelEventArgs wheel = e as MouseWheelEventArgs;
            if (wheel != null)
            {
                wheel.Handled = true;
            }
            //if (zoomStack.Count == 0)
            //{
            //    zoomStack.Push(oper);
            //}
        }

        private Point GetMousePosition(MouseEventArgs e, double Scale)
        {
            Point mousePos = e.GetPosition(this.Content);
            mousePos = new Point((mousePos.X + TopLeft.X) * Scale, (mousePos.Y + TopLeft.Y) * Scale);
            return mousePos;
        }

        Point? m_PanStartPosition = null;
        Point m_PreviousMousePosition = new Point(0, 0);
        DateTime m_LastLeftClick = DateTime.Now;
        DateTime m_LastRightClick = DateTime.Now;
        internal OverviewMouseState m_MouseState = OverviewMouseState.None;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (IsPanEnabled)
            {
                this.CaptureMouse();
                m_PanStartPosition = e.GetPosition(this);// GetMousePosition(e);
                m_PreviousMousePosition = m_PanStartPosition.Value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            m_LastLeftClick -= new TimeSpan(1, 0, 0);
            m_LastRightClick -= new TimeSpan(1, 0, 0);
            if (IsPanEnabled && m_PanStartPosition.HasValue)
            {
                m_MouseState = OverviewMouseState.Pan;
                Point currentPosition = e.GetPosition(this); //GetMousePosition(e);
                SetHorizontalOffset(m_HorizontalOffset + m_PreviousMousePosition.X - currentPosition.X);
                SetVerticalOffset(m_VerticalOffset + m_PreviousMousePosition.Y - currentPosition.Y);
                VerifyAndInvalidateScrollData();
                m_PreviousMousePosition = currentPosition;
            }
            else
            {
                m_MouseState = OverviewMouseState.None;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (m_MouseState == OverviewMouseState.Pan)
            {
                m_MouseState = OverviewMouseState.None;
            }
            if ((DateTime.Now - m_LastLeftClick).TotalMilliseconds < 300)
            {
                m_MouseState = OverviewMouseState.LeftDoubleClick;
                m_LastLeftClick = DateTime.Now - new TimeSpan(0, 0, 0, 2);
                PerformZoom(e);
            }
            else
            {
                m_MouseState = OverviewMouseState.LeftClick;
                m_LastLeftClick = DateTime.Now;
                PerformZoom(e);
            }
            m_MouseState = OverviewMouseState.None;
            this.ReleaseMouseCapture();
            m_PanStartPosition = null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonUp(e);
            if (m_MouseState == OverviewMouseState.Pan)
            {
                m_MouseState = OverviewMouseState.None;
            }
            else if ((DateTime.Now - m_LastRightClick).TotalMilliseconds < 300)
            {
                m_MouseState = OverviewMouseState.RightDoubleClick;
                m_LastRightClick = DateTime.Now - new TimeSpan(0, 0, 0, 2);
                PerformZoom(e);
            }
            else
            {
                m_MouseState = OverviewMouseState.RightClick;
                m_LastRightClick = DateTime.Now;
                PerformZoom(e);
            }
            m_MouseState = OverviewMouseState.None;
        }

        private ScaleTransform _scaleTransform = new ScaleTransform();
        private TranslateTransform _translateTransform = new TranslateTransform();
        private DoubleAnimation _scaleXAnimation = new DoubleAnimation();
        private DoubleAnimation _scaleYAnimation = new DoubleAnimation();
        private DoubleAnimation _translateXAnimation = new DoubleAnimation();
        private DoubleAnimation _translateYAnimation = new DoubleAnimation();
        private Duration _animationDuration = new Duration(new TimeSpan(0, 0, 0, 0, 200));
        private Storyboard _storyboard = new Storyboard();
        
        #region ScrollViewer

        //internal double HorizontalOffset
        //{
        //    get { return (double)GetValue(HorizontalOffsetProperty); }
        //    set { SetValue(HorizontalOffsetProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for HorizontalOffset.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty HorizontalOffsetProperty =
        //    DependencyProperty.Register("HorizontalOffset", typeof(double), typeof(OverviewContentHolder), new PropertyMetadata(0d, OnHVChanged));
        
        //internal double VerticalOffset
        //{
        //    get { return (double)GetValue(VerticalOffsetProperty); }
        //    set { SetValue(VerticalOffsetProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for VerticalOffset.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty VerticalOffsetProperty =
        //    DependencyProperty.Register("VerticalOffset", typeof(double), typeof(OverviewContentHolder), new PropertyMetadata(0d, OnHVChanged));

        //private static void OnHVChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    OverviewContentHolder panel = d as OverviewContentHolder;
        //    if (panel.MyParent is ScrollViewer)
        //    {
        //        panel.MyParent.Dispatcher.BeginInvoke(() =>
        //            {
        //                (panel.MyParent as ScrollViewer).ScrollToHorizontalOffset(panel.HorizontalOffset);
        //                (panel.MyParent as ScrollViewer).ScrollToVerticalOffset(panel.VerticalOffset);
        //            }
        //        );
        //    }
        //    else
        //    {
        //        panel.InvalidateArrange();
        //    }
        //}

        //internal double ViewportWidth
        //{
        //    get { return (double)GetValue(ViewportWidthProperty); }
        //    set { SetValue(ViewportWidthProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for ViewportWidth.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty ViewportWidthProperty =
        //    DependencyProperty.Register("ViewportWidth", typeof(double), typeof(OverviewContentHolder), new PropertyMetadata(0d, OnVPSizeChanged));

        //internal double ViewportHeight
        //{
        //    get { return (double)GetValue(ViewportHeightProperty); }
        //    set { SetValue(ViewportHeightProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for ViewportHeight.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty ViewportHeightProperty =
        //    DependencyProperty.Register("ViewportHeight", typeof(double), typeof(OverviewContentHolder), new PropertyMetadata(0d, OnVPSizeChanged));

        //private static void OnVPSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //}

        //internal double ExtentWidth
        //{
        //    get { return (double)GetValue(ExtentWidthProperty); }
        //    set { SetValue(ExtentWidthProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for ExtentWidth.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty ExtentWidthProperty =
        //    DependencyProperty.Register("ExtentWidth", typeof(double), typeof(OverviewContentHolder), new PropertyMetadata(0d));

        //internal double ExtentHeight
        //{
        //    get { return (double)GetValue(ExtentHeightProperty); }
        //    set { SetValue(ExtentHeightProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for ExtentHeight.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty ExtentHeightProperty =
        //    DependencyProperty.Register("ExtentHeight", typeof(double), typeof(OverviewContentHolder), new PropertyMetadata(0d));

        #endregion
        /// <summary>
        /// 
        /// </summary>
        public bool AnimationEnabled
        {
            get { return (bool)GetValue(AnimationEnabledProperty); }
            set { SetValue(AnimationEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZoomAnimationEnabled.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty AnimationEnabledProperty =
            DependencyProperty.Register("AnimationEnabled", typeof(bool), typeof(OverviewContentHolder), new PropertyMetadata(true));
        
        #region IZoomPanel Implementation
        /// <summary>
        /// 
        /// </summary>
        public ZoomGesture ZoomInGesture
        {
            get { return (ZoomGesture)GetValue(ZoomInGestureProperty); }
            set { SetValue(ZoomInGestureProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZoomGesture.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ZoomInGestureProperty =
            DependencyProperty.Register("ZoomInGesture", typeof(ZoomGesture), typeof(OverviewContentHolder), new PropertyMetadata(ZoomGesture.MouseWheelUp | ZoomGesture.Ctrl));
        /// <summary>
        /// 
        /// </summary>
        public ZoomGesture ZoomOutGesture
        {
            get { return (ZoomGesture)GetValue(ZoomOutGestureProperty); }
            set { SetValue(ZoomOutGestureProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZoomOutGesture.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ZoomOutGestureProperty =
            DependencyProperty.Register("ZoomOutGesture", typeof(ZoomGesture), typeof(OverviewContentHolder), new PropertyMetadata(ZoomGesture.MouseWheelDown | ZoomGesture.Ctrl));
        /// <summary>
        /// 
        /// </summary>
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
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(double), typeof(OverviewContentHolder), new PropertyMetadata(1d, OnScaleChanged));

        private static void OnScaleChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            OverviewContentHolder ov = dp as OverviewContentHolder;
            if (ov.Scale < ov.MinimumZoom && ov.EnableFitToPage)
            {
            }
            else
            {
                ov.Scale = Math.Max(ov.MinimumZoom, Math.Min(ov.MaximumZoom, ov.Scale));
            }
            ov.InvalidateMeasure();
            ov.InvalidateArrange();
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsZoomInEnabled
        {
            get { return (bool)GetValue(IsZoomInEnabledProperty); }
            set { SetValue(IsZoomInEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsZoomInEnabled.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsZoomInEnabledProperty =
            DependencyProperty.Register("IsZoomInEnabled", typeof(bool), typeof(OverviewContentHolder), new PropertyMetadata(true));
        /// <summary>
        /// 
        /// </summary>
        public ICommand ZoomIn
        {
            get { return (ICommand)GetValue(ZoomInProperty); }
            set { SetValue(ZoomInProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZoomIn.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ZoomInProperty =
            DependencyProperty.Register("ZoomIn", typeof(ICommand), typeof(OverviewContentHolder), new PropertyMetadata(null));
        /// <summary>
        /// 
        /// </summary>
        public bool IsZoomOutEnabled
        {
            get { return (bool)GetValue(IsZoomOutEnabledProperty); }
            set { SetValue(IsZoomOutEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsZoomOutEnabled.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsZoomOutEnabledProperty =
            DependencyProperty.Register("IsZoomOutEnabled", typeof(bool), typeof(OverviewContentHolder), new PropertyMetadata(true));
        /// <summary>
        /// 
        /// </summary>
        public ICommand ZoomOut
        {
            get { return (ICommand)GetValue(ZoomOutProperty); }
            set { SetValue(ZoomOutProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZoomOut.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ZoomOutProperty =
            DependencyProperty.Register("ZoomOut", typeof(ICommand), typeof(OverviewContentHolder), new PropertyMetadata(null));
        /// <summary>
        /// 
        /// </summary>
        public bool IsZoomToEnabled
        {
            get { return (bool)GetValue(IsZoomToEnabledProperty); }
            set { SetValue(IsZoomToEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsZoomToEnabled.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsZoomToEnabledProperty =
            DependencyProperty.Register("IsZoomToEnabled", typeof(bool), typeof(OverviewContentHolder), new PropertyMetadata(true));
        /// <summary>
        /// 
        /// </summary>
        public ICommand ZoomTo
        {
            get { return (ICommand)GetValue(ZoomToProperty); }
            set { SetValue(ZoomToProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZoomTo.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ZoomToProperty =
            DependencyProperty.Register("ZoomTo", typeof(ICommand), typeof(OverviewContentHolder), new PropertyMetadata(null));
        /// <summary>
        /// 
        /// </summary>
        public bool IsZoomResetEnabled
        {
            get { return (bool)GetValue(IsZoomResetEnabledProperty); }
            set { SetValue(IsZoomResetEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsZoomResetEnabled.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsZoomResetEnabledProperty =
            DependencyProperty.Register("IsZoomResetEnabled", typeof(bool), typeof(OverviewContentHolder), new PropertyMetadata(true));
        /// <summary>
        /// 
        /// </summary>
        public ICommand ZoomReset
        {
            get { return (ICommand)GetValue(ZoomResetProperty); }
            set { SetValue(ZoomResetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZoomReset.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ZoomResetProperty =
            DependencyProperty.Register("ZoomReset", typeof(ICommand), typeof(OverviewContentHolder), new PropertyMetadata(null));
        /// <summary>
        /// 
        /// </summary>
        public double ZoomFactor
        {
            get { return (double)GetValue(ZoomFactorProperty); }
            set { SetValue(ZoomFactorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZoomFactor.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ZoomFactorProperty =
            DependencyProperty.Register("ZoomFactor", typeof(double), typeof(OverviewContentHolder), new PropertyMetadata(0.2d));        
        /// <summary>
        /// 
        /// </summary>
        public double MinimumZoom
        {
            get { return (double)GetValue(MinimumZoomProperty); }
            set { SetValue(MinimumZoomProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinimumZoom.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty MinimumZoomProperty =
            DependencyProperty.Register("MinimumZoom", typeof(double), typeof(OverviewContentHolder), new PropertyMetadata(0.2d));
        /// <summary>
        /// 
        /// </summary>
        public double MaximumZoom
        {
            get { return (double)GetValue(MaximumZoomProperty); }
            set { SetValue(MaximumZoomProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaximumZoom.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty MaximumZoomProperty =
            DependencyProperty.Register("MaximumZoom", typeof(double), typeof(OverviewContentHolder), new PropertyMetadata(10d));
        /// <summary>
        /// 
        /// </summary>
        public ZoomMode ZoomMode
        {
            get { return (ZoomMode)GetValue(ZoomModeProperty); }
            set { SetValue(ZoomModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZoomMode.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ZoomModeProperty =
            DependencyProperty.Register("ZoomMode", typeof(ZoomMode), typeof(OverviewContentHolder), new PropertyMetadata(ZoomMode.Unit));
        /// <summary>
        /// 
        /// </summary>
        public bool IsPanEnabled
        {
            get { return (bool)GetValue(IsPanEnabledProperty); }
            set { SetValue(IsPanEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPanEnabled.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsPanEnabledProperty =
            DependencyProperty.Register("IsPanEnabled", typeof(bool), typeof(OverviewContentHolder), new PropertyMetadata(false, OnIsPanEnabledChanged));

                
        private static void OnIsPanEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d as OverviewContentHolder).IsPanEnabled)
            (d as OverviewContentHolder).m_PanStartPosition = null;
        }
        #endregion

        #region Command

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

        private bool CanZoomResetExecute(object parameter)
        {
            return IsZoomResetEnabled;
        }

        private void ExecuteZoomInCommand(object parameter)
        {
            CheckZoomStack(parameter);
            if (parameter is IZoomParameter)
            {
                UnitScale += (parameter as IZoomParameter).ZoomFactor ?? this.ZoomFactor;
            }
            else
            {
                UnitScale += ZoomFactor;
            }
        }

        private void ExecuteZoomOutCommand(object parameter)
        {
            CheckZoomStack(parameter);
            if (parameter is IZoomParameter)
            {
                UnitScale -= (parameter as IZoomParameter).ZoomFactor ?? this.ZoomFactor;
            }
            else
            {
                UnitScale -= ZoomFactor;
            }
        }

        private void CheckZoomStack(object parameter)
        {
            Point? focusPoint = null;

            if (parameter is IZoomParameter)
            {
                focusPoint = (parameter as IZoomParameter).FocusPoint;
                focusPoint = new Point(
                    focusPoint.Value.X * UnitScale,
                    focusPoint.Value.Y * UnitScale
                    );
            }

            if (zoomStack.Count == 0)
            {
                ZoomOperation oper = new ZoomOperation();
                if (zoomStack.Count == 0)
                {
                    oper.mousePos = focusPoint ?? new Point(
                        m_HorizontalOffset + (m_ViewportWidth / 2),
                        m_VerticalOffset + (m_ViewportHeight / 2)
                        );

                    oper.ratio = new Point(oper.mousePos.X / ExtentWidth, oper.mousePos.Y / ExtentHeight);
                }
                zoomStack.Push(oper);
            }
        }

        private void ExecuteZoomResetCommand(object parameter)
        {
            CheckZoomStack(parameter);
            UnitScale = 1d;
        }

        private void ExecuteZoomToCommand(object parameter)
        {
            CheckZoomStack(parameter);
            if (parameter is IZoomParameter)
            {
                UnitScale = (parameter as IZoomParameter).ZoomTo ?? 1d;
            }
        }

        #endregion

        private void UpdatePageBackground()
        {
            if (PART_PageBackground != null)
            {
                PART_PageBackground.Margin = new Thickness(-TopLeft.X, -TopLeft.Y, 0, 0);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void FitToPage()
        {
            double width = (ViewportWidth * Scale) / (ExtentWidth);
            double height = (ViewportHeight * Scale) / (ExtentHeight);
            // Set the value of daigramView's CurrentZoom property
            // to get Fit to Screen            
            this.SetValue(ScaleProperty, width < height ? width : height);
            SetHorizontalOffset(0);
            SetVerticalOffset(0);
        }


        private Grid PART_Grid;
        private Rectangle PART_PageBackground;
        private ContentControl diagramView;
        /// <summary>
        /// 
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_Grid = this.GetTemplateChild("PART_Grid") as Grid;
            PART_PageBackground = this.GetTemplateChild("PART_PageBackground") as Rectangle;
            UpdatePageBackground();

            if(PART_Grid.Resources!=null && PART_Grid.Resources.Count>0)
            {
                _storyboard = PART_Grid.Resources["ZoomPanStoryboard"] as Storyboard;
                _scaleXAnimation = _storyboard.Children[0] as DoubleAnimation;
                _scaleYAnimation = _storyboard.Children[1] as DoubleAnimation;
                _translateXAnimation = _storyboard.Children[2] as DoubleAnimation;
                _translateYAnimation = _storyboard.Children[3] as DoubleAnimation;
            }

            _translateTransform = this.GetTemplateChild("PART_PanTransform") as TranslateTransform;
            _scaleTransform = this.GetTemplateChild("PART_ZoomTransform") as ScaleTransform;

            // Get DiagramView reference 
            diagramView = GetDiagramView(this);
            if (diagramView != null)
            {
                diagramView.PreviewMouseWheel += new MouseWheelEventHandler(diagramView_PreviewMouseWheel);
            }
        }

        private ContentControl GetDiagramView(DependencyObject element)
        {
            while (element != null && !(element.DependencyObjectType.Name == "DiagramView"))
            {
                element = VisualTreeHelper.GetParent(element);
            }

            return element as ContentControl;
        }

        void diagramView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            PerformZoom(e);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            SetViewportWidth(availableSize.Width);
            SetViewportHeight(availableSize.Height);

            Size desiredSize =  base.MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity));           
            Size scaledSize = new Size(desiredSize.Width * UnitScale, desiredSize.Height * UnitScale);
                 SetExtentWidth((TopLeft.X * UnitScale) + scaledSize.Width);
                 SetExtentHeight((TopLeft.Y * UnitScale) + scaledSize.Height);
          
            VerifyScrollData();
            ScrollOwner.InvalidateScrollInfo();
#if WPF
            OverViewFitToPageEventArgs newevent = new OverViewFitToPageEventArgs(true);
            newevent.RoutedEvent = OverviewContentHolder.FitToPageEvent;
            newevent.Source = this;
            RaiseEvent(newevent);

            if (newevent.Cancel && this.EnableFitToPage)
            {
                this.FitToPage();
            }
#endif
            double finalwidth = ExtentWidth > ViewportWidth ? ExtentWidth : ViewportWidth;
            double fianlheight = ExtentHeight > ViewportHeight ? ExtentHeight : ViewportHeight;

            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this as DependencyObject))
                return desiredSize;

            return new Size(finalwidth, fianlheight); // desiredSize;
        }

     /// <summary>
     /// 
     /// </summary>
     /// <param name="finalSize"></param>
     /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            Size actualSize = base.ArrangeOverride(finalSize);
            //VerifyScrollData();            

            ApplyAnimation(actualSize);
            VerifyScrollData();
            ScrollOwner.InvalidateScrollInfo();
            double finalwidth = ExtentWidth > ViewportWidth ? ExtentWidth : ViewportWidth;
            double fianlheight = ExtentHeight > ViewportHeight ? ExtentHeight : ViewportHeight;
            return new Size(finalwidth, fianlheight); // actualSize;
        }

        private void ApplyAnimation(Size actualSize)
        {
            if (zoomStack.Count > 0)
            {
                ZoomOperation oper = zoomStack.Pop();
                SetHorizontalOffset(m_HorizontalOffset + (this.ExtentWidth * oper.ratio.X) - oper.mousePos.X);
                SetVerticalOffset(m_VerticalOffset + (this.ExtentHeight * oper.ratio.Y) - oper.mousePos.Y);
            }

            Rect arrangeRect = new Rect(
                -(HorizontalOffset / UnitScale - TopLeft.X) * UnitScale,
                -(VerticalOffset / UnitScale - TopLeft.Y) * UnitScale,
                actualSize.Width,
                actualSize.Height
            );

            _scaleXAnimation.From = _scaleTransform.ScaleX;
            _scaleXAnimation.To = UnitScale;

            _scaleYAnimation.From = _scaleTransform.ScaleY;
            _scaleYAnimation.To = UnitScale;

            if (AnimationEnabled)
            {
                _scaleYAnimation.Duration = _scaleXAnimation.Duration = _animationDuration;
            }
            else
            {
                _scaleYAnimation.Duration = _scaleXAnimation.Duration = new Duration(new TimeSpan(0));
            }

            _translateXAnimation.From = _translateTransform.X;
            _translateXAnimation.To = arrangeRect.Left;

            _translateYAnimation.From = _translateTransform.Y;
            _translateYAnimation.To = arrangeRect.Top;

            if (AnimationEnabled)
            {
                _translateXAnimation.Duration = _translateYAnimation.Duration = _animationDuration;
            }
            else
            {
                _translateXAnimation.Duration = _translateYAnimation.Duration = new Duration(new TimeSpan(0));
            }

            _storyboard.Begin();
        }
        #region IScrollInfo
        
        #region Fields

        private const double LineSize = 16;
        private const double WheelSize = 3 * LineSize;

        private bool m_CanHorizontallyScroll;
        private bool m_CanVerticallyScroll;
        private double m_HorizontalOffset;
        private double m_VerticalOffset;
        private double m_ViewportWidth;
        private double m_ViewportHeight;
        private double m_ExtentWidth;
        private double m_ExtentHeight;
        private ScrollViewer m_ScrollOwner; 
        #endregion
        /// <summary>
        /// 
        /// </summary>
        public bool CanHorizontallyScroll
        {
            get
            {
                return m_CanHorizontallyScroll;
            }
            set
            {
                m_CanHorizontallyScroll = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool CanVerticallyScroll
        {
            get
            {
                return m_CanVerticallyScroll;
            }
            set
            {
                m_CanVerticallyScroll = value;
            }
        }

        #region Line
        /// <summary>
        /// 
        /// </summary>
        public void LineDown()
        {
            SetVerticalOffset(VerticalOffset + LineSize);
            VerifyAndInvalidateScrollData();
        }
        /// <summary>
        /// 
        /// </summary>
        public void LineLeft()
        {
            SetHorizontalOffset(HorizontalOffset - LineSize);
            VerifyAndInvalidateScrollData();
        }
        /// <summary>
        /// 
        /// </summary>
        public void LineRight()
        {
            SetHorizontalOffset(HorizontalOffset + LineSize);
            VerifyAndInvalidateScrollData();
        }
        /// <summary>
        /// 
        /// </summary>
        public void LineUp()
        {
            SetVerticalOffset(VerticalOffset - LineSize);
            VerifyAndInvalidateScrollData();
        } 
        #endregion

        #region Mouse
        /// <summary>
        /// 
        /// </summary>
        public void MouseWheelDown()
        {
            SetVerticalOffset(VerticalOffset + WheelSize);
            VerifyAndInvalidateScrollData();
        }
        /// <summary>
        /// 
        /// </summary>
        public void MouseWheelLeft()
        {
            SetHorizontalOffset(HorizontalOffset - WheelSize);
            VerifyAndInvalidateScrollData();
        }
        /// <summary>
        /// 
        /// </summary>
        public void MouseWheelRight()
        {
            SetHorizontalOffset(HorizontalOffset + WheelSize);
            VerifyAndInvalidateScrollData();
        }
        /// <summary>
        /// 
        /// </summary>
        public void MouseWheelUp()
        {
            SetVerticalOffset(VerticalOffset - WheelSize);
            VerifyAndInvalidateScrollData();
        }

        #endregion

        #region Page
        /// <summary>
        /// 
        /// </summary>
        public void PageDown()
        {
            SetVerticalOffset(VerticalOffset + ViewportHeight);
        }
        /// <summary>
        /// 
        /// </summary>
        public void PageLeft()
        {
            SetHorizontalOffset(HorizontalOffset - ViewportWidth);
        }
        /// <summary>
        /// 
        /// </summary>
        public void PageRight()
        {
            SetHorizontalOffset(HorizontalOffset + ViewportWidth);
        }
        /// <summary>
        /// 
        /// </summary>
        public void PageUp()
        {
            SetVerticalOffset(VerticalOffset - ViewportHeight);
        }

        #endregion
        /// <summary>
        /// 
        /// </summary>
        public ScrollViewer ScrollOwner
        {
            get
            {
                return m_ScrollOwner;
            }
            set
            {
                m_ScrollOwner = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public double ExtentHeight
        {
            get { return m_ExtentHeight; }
        }
        /// <summary>
        /// 
        /// </summary>
        public double ExtentWidth
        {
            get { return m_ExtentWidth; }
        }
        /// <summary>
        /// 
        /// </summary>
        public double HorizontalOffset
        {
            get { return m_HorizontalOffset; }
        }
        /// <summary>
        /// 
        /// </summary>
        public double VerticalOffset
        {
            get { return m_VerticalOffset; }
        }
        /// <summary>
        /// 
        /// </summary>
        public double ViewportHeight
        {
            get { return m_ViewportHeight; }
        }
        /// <summary>
        /// 
        /// </summary>
        public double ViewportWidth
        {
            get { return m_ViewportWidth; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        public void SetHorizontalOffset(double offset)
        {
            if (m_HorizontalOffset != offset)
            {
                double oldValue = m_HorizontalOffset;
                m_HorizontalOffset = offset;
                VerifyAndInvalidateScrollData();
                InvokeScrollChangedEvent(ScrollParamether.HorizontalOffset, oldValue, offset);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        public void SetVerticalOffset(double offset)
        {
            if (m_VerticalOffset != offset)
            {
                double oldValue = m_VerticalOffset;
                m_VerticalOffset = offset;
                VerifyAndInvalidateScrollData();
                InvokeScrollChangedEvent(ScrollParamether.VerticalOffset, oldValue, offset);
            }
        }

        private void SetViewportWidth(double newValue)
        {
            double oldValue = m_ViewportWidth;
            m_ViewportWidth = newValue;
            InvokeScrollChangedEvent(ScrollParamether.ViewportWidth, oldValue, newValue);
        }

        private void SetViewportHeight(double newValue)
        {
            double oldValue = m_ViewportHeight;
            m_ViewportHeight = newValue;
            InvokeScrollChangedEvent(ScrollParamether.ViewportHeight, oldValue, newValue);
        }

        private void SetExtentWidth(double newValue)
        {
            double oldValue = m_ExtentWidth;
            m_ExtentWidth = newValue;
            InvokeScrollChangedEvent(ScrollParamether.ExtentWidth, oldValue, newValue);
        }

        private void SetExtentHeight(double newValue)
        {
            double oldValue = m_ExtentHeight;
            m_ExtentHeight = newValue;
            InvokeScrollChangedEvent(ScrollParamether.ExtentHeight, oldValue, newValue);
        }

        private void VerifyScrollData()
        {
            if (IsPanEnabled && m_PanStartPosition.HasValue)
            {
                if (m_HorizontalOffset < 0 && m_VerticalOffset < 0)
                {
                    Point oldOrig = GetOrigin(this.Content);
                    SetOrigin(this.Content, new Point(oldOrig.X + m_HorizontalOffset, oldOrig.Y + m_VerticalOffset));
                }
                else if (m_HorizontalOffset < 0)
                {
                    Point oldOrig = GetOrigin(this.Content);
                    SetOrigin(this.Content, new Point(oldOrig.X + m_HorizontalOffset, oldOrig.Y));
                }
                else if (m_VerticalOffset < 0)
                {
                    Point oldOrig = GetOrigin(this.Content);
                    SetOrigin(this.Content, new Point(oldOrig.X, oldOrig.Y + m_VerticalOffset));
                }
                else 
                {
                    Point oldOrig = GetOrigin(this.Content);
                    double m_extraheight = (m_VerticalOffset + m_ViewportHeight) - ExtentHeight;
                    double m_extraWidth = (m_HorizontalOffset + m_ViewportWidth) - ExtentWidth;
                    if (m_extraWidth>0 ||m_extraheight>0)
                    {
                        if (m_extraWidth < 0)
                            m_extraWidth = 0;
                        if (m_extraheight < 0)
                            m_extraheight = 0;
                        ExtraPanningEventEventArgs newargs = new ExtraPanningEventEventArgs(true); 
                        newargs.RoutedEvent = OverviewContentHolder.ExtraPanningEvent;
                        newargs.Source = this;
                        newargs.ExtraSize = new Size(m_extraWidth, m_extraheight);
                        RaiseEvent(newargs);
                    }
                }
            }
            //if (m_HorizontalOffset < 0 || m_VerticalOffset < 0)
            //{
            //    this.Padding = new Thickness(
            //        Padding.Left - (m_HorizontalOffset < 0 ? m_HorizontalOffset : 0),
            //        Padding.Top - (m_VerticalOffset < 0 ? m_VerticalOffset : 0),
            //        Padding.Right,
            //        Padding.Bottom
            //        );
            //}
            //else
            //{
            //}

            double tempHor = m_HorizontalOffset;
            tempHor = Math.Max(0, tempHor);
            tempHor = Math.Min(Math.Max(0, ExtentWidth - ViewportWidth), tempHor);
            if (double.IsNaN(m_HorizontalOffset) || double.IsInfinity(m_HorizontalOffset))
            {
                tempHor = 0;
            }

            if (tempHor != m_HorizontalOffset)
            {
                SetHorizontalOffset(tempHor);
            }

            double tempVer = m_VerticalOffset;

            tempVer = Math.Max(0, tempVer);
            tempVer = Math.Min(Math.Max(0, ExtentHeight - ViewportHeight), tempVer);
            if (double.IsNaN(tempVer) || double.IsInfinity(tempVer))
            {
                tempVer = 0;
            }

            if (tempVer != m_VerticalOffset)
            {
                SetVerticalOffset(tempVer);
            }

            ScrollOwner.InvalidateScrollInfo();
        }

        private void VerifyAndInvalidateScrollData()
        {
            VerifyScrollData();
            ScrollOwner.InvalidateScrollInfo();            
            //this.InvalidateMeasure();
            this.InvalidateArrange();
        }


#if WPF

        /// <summary>
        ///  FitToPage Routed event. 
        /// </summary>
        public static readonly RoutedEvent FitToPageEvent = EventManager.RegisterRoutedEvent(
         "UpdateFitToPage", RoutingStrategy.Bubble, typeof(OverviewFitPageEventHandler), typeof(OverviewContentHolder));
        /// <summary>
        /// 
        /// </summary>
        public event OverviewFitPageEventHandler UpdateFitToPage
        {
            add { AddHandler(FitToPageEvent, value); }
            remove { RemoveHandler(FitToPageEvent, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="evtArgs"></param>
        public delegate void OverviewFitPageEventHandler(object sender, OverViewFitToPageEventArgs evtArgs);
        /// <summary>
        /// 
        /// </summary>
        public class OverViewFitToPageEventArgs : RoutedEventArgs
        {
            bool cc = false;
            /// <summary>
            /// 
            /// </summary>
            public bool Cancel
            {
                get
                {
                    return cc;
                }
                set
                {
                    if (value && this.Source != null && (this.Source as OverviewContentHolder).EnableFitToPage)
                    {
                        (this.Source as OverviewContentHolder).FitToPage();
                    }
                    cc = value;
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="fittopage"></param>
            public OverViewFitToPageEventArgs(bool fittopage)
            {

                this.Cancel = fittopage;
            }

        }

#endif



#if WPF
        /// <summary>
        ///  ExtraPanningEvent Routed event. 
        /// </summary>
        public static readonly RoutedEvent ExtraPanningEvent = EventManager.RegisterRoutedEvent(
         "ExtraPanning", RoutingStrategy.Bubble, typeof(ExtraPanningEventEventHandler), typeof(OverviewContentHolder));
        /// <summary>
        /// 
        /// </summary>
        public event ExtraPanningEventEventHandler ExtraPanning
        {
            add { AddHandler(ExtraPanningEvent, value); }
            remove { RemoveHandler(ExtraPanningEvent, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="evtArgs"></param>
        public delegate void ExtraPanningEventEventHandler(object sender, ExtraPanningEventEventArgs evtArgs);
        /// <summary>
        /// 
        /// </summary>
        public class ExtraPanningEventEventArgs : RoutedEventArgs
        {
            Size cc;
            /// <summary>
            /// 
            /// </summary>
            public Size ExtraSize
            {
                get
                {
                    return cc;
                }
                set
                {
                    
                    cc = value;
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="ExtraSize"></param>
            public ExtraPanningEventEventArgs(bool ExtraSize)
            {

                //this.ExtraSize = fittopage;
            }

        }
#endif
        private void InvokeScrollChangedEvent(ScrollParamether param, double oldValue, double newValue)
        { 
#if SILVERLIGHT
            ScrollChangedEventArgs args = new ScrollChangedEventArgs(this, param, oldValue, newValue);
            if (ScrollChanged != null)
            {
                this.ScrollChanged.Invoke(this, args);
            }
#endif
        }

#if WPF
        /// <summary>
        /// 
        /// </summary>
        /// <param name="visual"></param>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
           // this.TransformToAncestor(visual as UIElement).TransformBounds(new Rect());
            return new Rect(visual.TransformToAncestor(this).Transform(new Point(0,0)), new Size());
        }
#endif

#if SILVERLIGHT        
        public Rect MakeVisible(UIElement visual, Rect rectangle)
        {
            return new Rect();
        }
#endif

        #endregion
    }
    
}