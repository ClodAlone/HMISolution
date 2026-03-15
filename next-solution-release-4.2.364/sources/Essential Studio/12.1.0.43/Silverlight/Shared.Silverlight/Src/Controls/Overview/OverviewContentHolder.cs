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
using Syncfusion.Windows.Tools.Controls;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// 
    /// </summary>
    public class OverviewContentHolder : ContentControl, IOverviewPanel
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object GetVisual(DependencyObject obj)
        {
            return (object)obj.GetValue(VisualProperty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetVisual(DependencyObject obj, object value)
        {
            obj.SetValue(VisualProperty, value);
        }

        // Using a DependencyProperty as the backing store for Visual.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty VisualProperty =
            DependencyProperty.RegisterAttached("Visual", typeof(object), typeof(OverviewContentHolder), new PropertyMetadata(null, OnVisualChanged));

        private static void OnVisualChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Point GetStart(DependencyObject obj)
        {
            return (Point)obj.GetValue(StartProperty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetStart(DependencyObject obj, Point value)
        {
            obj.SetValue(StartProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double GetInverseScale(DependencyObject obj)
        {
            return (double)obj.GetValue(InverseScaleProperty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetInverseScale(DependencyObject obj, double value)
        {
            obj.SetValue(InverseScaleProperty, value);
        }



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



        // Using a DependencyProperty as the backing store for InverseScale.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty InverseScaleProperty =
            DependencyProperty.RegisterAttached("InverseScale", typeof(double), typeof(OverviewContentHolder), new PropertyMetadata(1d));

        // Using a DependencyProperty as the backing store for Start.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty StartProperty =
            DependencyProperty.RegisterAttached("Start", typeof(Point), typeof(OverviewContentHolder), new PropertyMetadata(new Point(0,0)));

        /// <summary>
        /// 
        /// </summary>
        public OverviewContentHolder()
        {
            //this.DefaultStyleKey = typeof(OverviewPanel);
            this.OverviewTransform = new CompositeTransform();// new CompositeTransform() { ScaleX = UnitScale, ScaleY = UnitScale };
            this.RenderTransform = this.OverviewTransform;
            ZoomIn = new DelegateCommand<object>(ExecuteZoomInCommand, CanZoomInExecute);
            ZoomOut = new DelegateCommand<object>(ExecuteZoomOutCommand, CanZoomOutExecute);
            ZoomTo = new DelegateCommand<object>(ExecuteZoomToCommand, CanZoomToExecute);
            ZoomReset = new DelegateCommand<object>(ExecuteZoomResetCommand, CanZoomResetExecute);
            //Binding bin = new Binding("Parent.ContentTemplate");
            //bin.Source = this;
            //this.SetBinding(ContentTemplateProperty, bin);
            this.Loaded += new RoutedEventHandler(ZoomPanel_Loaded);
            this.UseLayoutRounding = false;
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
                //ScrollSource.ScrollToHorizontalOffset(SVh - ScrollSource.ViewportWidth * position_percent.X);
                HorizontalOffset = (SVh - ViewportWidth * position_percent.X);
                //ScrollSource.ScrollToVerticalOffset(SVv - ScrollSource.ViewportHeight * position_percent.Y);
                VerticalOffset = (SVv - ViewportHeight * position_percent.Y);
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
                    pan.InternalBinding("MyParent.HorizontalOffset", HorizontalOffsetProperty, BindingMode.TwoWay);
                    pan.InternalBinding("MyParent.VerticalOffset", VerticalOffsetProperty, BindingMode.TwoWay);
                    pan.InternalBinding("MyParent.ViewportWidth", ViewportWidthProperty, BindingMode.OneWay);
                    pan.InternalBinding("MyParent.ViewportHeight", ViewportHeightProperty, BindingMode.OneWay);
                    pan.InternalBinding("MyParent.ExtentWidth", ExtentWidthProperty, BindingMode.OneWay);
                    pan.InternalBinding("MyParent.ExtentHeight", ExtentHeightProperty, BindingMode.OneWay);
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

        internal void ScrollToHorizontrolOffSet(double offx) 
        {
            
        }

        //private void InternalBinding(DependencyProperty dpProp, string sourceProp, DependencyObject source)
        //{
        //    Binding bin = new Binding(sourceProp);
        //    bin.Converter = new ReverseTransformConverter();
        //    bin.Mode = BindingMode.TwoWay;
        //    bin.Source = this;
        //    BindingOperations.SetBinding(source, dpProp, bin);
        //}

        #region ScrollViewer

        internal double HorizontalOffset
        {
            get { return (double)GetValue(HorizontalOffsetProperty); }
            set { SetValue(HorizontalOffsetProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HorizontalOffset.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register("HorizontalOffset", typeof(double), typeof(OverviewContentHolder), new PropertyMetadata(0d, OnHVChanged));
        
        internal double VerticalOffset
        {
            get { return (double)GetValue(VerticalOffsetProperty); }
            set { SetValue(VerticalOffsetProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for VerticalOffset.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register("VerticalOffset", typeof(double), typeof(OverviewContentHolder), new PropertyMetadata(0d, OnHVChanged));

        private static void OnHVChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OverviewContentHolder panel = d as OverviewContentHolder;
            if (panel.MyParent is ScrollViewer)
            {
                panel.MyParent.Dispatcher.BeginInvoke(() =>
                    {
                        (panel.MyParent as ScrollViewer).ScrollToHorizontalOffset(panel.HorizontalOffset);
                        (panel.MyParent as ScrollViewer).ScrollToVerticalOffset(panel.VerticalOffset);
                    }
                );
            }
            else
            {
                panel.InvalidateArrange();
            }
        }

        internal double ViewportWidth
        {
            get { return (double)GetValue(ViewportWidthProperty); }
            set { SetValue(ViewportWidthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ViewportWidth.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ViewportWidthProperty =
            DependencyProperty.Register("ViewportWidth", typeof(double), typeof(OverviewContentHolder), new PropertyMetadata(0d, OnVPSizeChanged));

        internal double ViewportHeight
        {
            get { return (double)GetValue(ViewportHeightProperty); }
            set { SetValue(ViewportHeightProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ViewportHeight.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ViewportHeightProperty =
            DependencyProperty.Register("ViewportHeight", typeof(double), typeof(OverviewContentHolder), new PropertyMetadata(0d, OnVPSizeChanged));

        private static void OnVPSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        internal double ExtentWidth
        {
            get { return (double)GetValue(ExtentWidthProperty); }
            set { SetValue(ExtentWidthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ExtentWidth.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ExtentWidthProperty =
            DependencyProperty.Register("ExtentWidth", typeof(double), typeof(OverviewContentHolder), new PropertyMetadata(0d));

        internal double ExtentHeight
        {
            get { return (double)GetValue(ExtentHeightProperty); }
            set { SetValue(ExtentHeightProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ExtentHeight.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ExtentHeightProperty =
            DependencyProperty.Register("ExtentHeight", typeof(double), typeof(OverviewContentHolder), new PropertyMetadata(0d));

        #endregion

        #region IZoomPanel Implementation

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
                value = value < MinimumZoom ? MinimumZoom : value;
                value = value > MaximumZoom ? MaximumZoom : value;
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

        /// <summary>
        /// Using a DependencyProperty as the backing store for Scale.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(double), typeof(OverviewContentHolder), new PropertyMetadata(1d, OnScaleChanged));

        private static void OnScaleChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            OverviewContentHolder ov = dp as OverviewContentHolder;
            (ov.OverviewTransform as CompositeTransform).ScaleX = (double)e.NewValue;
            (ov.OverviewTransform as CompositeTransform).ScaleY = (double)e.NewValue;
            ov.InvalidateMeasure();
            ov.InvalidateArrange();
            //if (!ov.m_MouseZoom)
            //{
            //    double delta = (double)e.NewValue / (double)e.OldValue;
            //    ov.DeltaScale(delta, new Point(0.5, 0.5));
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsZoomInEnabled
        {
            get { return (bool)GetValue(IsZoomInEnabledProperty); }
            set { SetValue(IsZoomInEnabledProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsZoomInEnabled.  This enables animation, styling, binding, etc...
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

        /// <summary>
        /// Using a DependencyProperty as the backing store for ZoomIn.  This enables animation, styling, binding, etc...
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

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsZoomOutEnabled.  This enables animation, styling, binding, etc...
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

        /// <summary>
        /// Using a DependencyProperty as the backing store for ZoomOut.  This enables animation, styling, binding, etc...
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

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsZoomToEnabled.  This enables animation, styling, binding, etc...
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

        /// <summary>
        /// Using a DependencyProperty as the backing store for ZoomTo.  This enables animation, styling, binding, etc...
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

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsZoomResetEnabled.  This enables animation, styling, binding, etc...
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

       /// <summary>
       /// Using a DependencyProperty as the backing store for ZoomReset.  This enables animation, styling, binding, etc...
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

        /// <summary>
        /// Using a DependencyProperty as the backing store for ZoomFactor.  This enables animation, styling, binding, etc...
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

        /// <summary>
        /// Using a DependencyProperty as the backing store for MinimumZoom.  This enables animation, styling, binding, etc...
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

        /// <summary>
        /// Using a DependencyProperty as the backing store for MaximumZoom.  This enables animation, styling, binding, etc...
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

        /// <summary>
        /// Using a DependencyProperty as the backing store for ZoomMode.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ZoomModeProperty =
            DependencyProperty.Register("ZoomMode", typeof(ZoomMode), typeof(OverviewContentHolder), new PropertyMetadata(ZoomMode.Unit));


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

        private bool CanZoomResetExecute(object parameter)
        {
            return IsZoomResetEnabled;
        }

        private void ExecuteZoomInCommand(object parameter)
        {
            if (parameter is IZoomParameter)
            {
                UnitScale += (parameter as IZoomParameter).ZoomFactor;
            }
            else
            {
                UnitScale += ZoomFactor;
            }
        }

        private void ExecuteZoomOutCommand(object parameter)
        {
            if (parameter is IZoomParameter)
            {
                UnitScale -= (parameter as IZoomParameter).ZoomFactor;
            }
            else
            {
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (!(MyParent is ScrollViewer))
            {
                ViewportHeight = double.IsInfinity(availableSize.Height)? 0d : availableSize.Height;
                ViewportWidth = double.IsInfinity(availableSize.Width) ? 0d : availableSize.Width;
            }
            Size desiredSize = base.MeasureOverride(availableSize);// (new Size(double.PositiveInfinity, double.PositiveInfinity));// new Size(availableSize.Width / UnitScale, availableSize.Height / UnitScale));
            //return new Size(desiredSize.Width * UnitScale, desiredSize.Height * UnitScale);

            //Size desiredSize = base.MeasureOverride(availableSize);
            if (!(MyParent is ScrollViewer))
            {
                ExtentWidth = desiredSize.Width * UnitScale;
                ExtentHeight = desiredSize.Height * UnitScale;
            }
            //if (UnitScale >= 1)
            {
                return (new Size(desiredSize.Width * UnitScale, desiredSize.Height * UnitScale));
            }
            //else
            //{
            //    return (new Size(desiredSize.Width, desiredSize.Height));
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            //Size actualSize = base.ArrangeOverride(new Size(finalSize.Width / UnitScale, finalSize.Height / UnitScale));
            //return new Size(actualSize.Width * UnitScale, actualSize.Height * UnitScale);
            if (!(MyParent is ScrollViewer))
            {
                this.Clip = new RectangleGeometry() { Rect = new Rect(new Point(0, 0), new Size(ViewportWidth / UnitScale, ViewportHeight / UnitScale)) };
            }
            else
            {
                this.ClearValue(ClipProperty);
            }

            if (!(MyParent is ScrollViewer))
            {
                foreach (UIElement ele in GetChildren())
                {
                    ele.Arrange(new Rect(new Point(-HorizontalOffset / UnitScale, -VerticalOffset / UnitScale), new Size(ele.DesiredSize.Width, ele.DesiredSize.Height)));  // new Size(finalSize.Width / UnitScale, finalSize.Height / UnitScale)));
                }
            }
            else
            {
                base.ArrangeOverride(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }
            //return  new Size(finalSize.Width / UnitScale, finalSize.Height / UnitScale);
            return new Size(finalSize.Width, finalSize.Height);
        }

        private IEnumerable GetChildren()
        {
            int count = VisualTreeHelper.GetChildrenCount(this);
            for (int i = 0; i < count; i++)
            {
                yield return VisualTreeHelper.GetChild(this, i);
            }
        }
    }
#if SILVERLIGHT
    /// <summary>
    /// 
    /// </summary>
    public class IScrollOverviewContentHolder : OverviewContentHolder, IScrollInfo
    {
        //public IScrollInfoZoomPanel()
        //{
        //    this.RenderTransform = new ScaleTransform() { ScaleX = Scale, ScaleY = Scale };
        //}

        //public double Scale
        //{
        //    get { return (double)GetValue(ScaleProperty); }
        //    set { SetValue(ScaleProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for Scale.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty ScaleProperty =
        //    DependencyProperty.Register("Scale", typeof(double), typeof(ZoomPanel), new PropertyMetadata(1d, OnScaleChanged));

        //private static void OnScaleChanged(DependencyObject dp, DependencyPropertyChangedEventArgs evtArgs)
        //{
        //    (dp as ZoomPanel).RenderTransform = new ScaleTransform() { ScaleX = (double)evtArgs.NewValue, ScaleY = (double)evtArgs.NewValue };
        //    (dp as ZoomPanel).InvalidateMeasure();
        //    (dp as ZoomPanel).InvalidateArrange();
        //}

        //protected override Size MeasureOverride(Size availableSize)
        //{
        //    Size desiredSize = base.MeasureOverride(new Size(availableSize.Width / Scale, availableSize.Height / Scale));
        //    return new Size(desiredSize.Width * Scale, desiredSize.Height * Scale);
        //}

        //protected override Size ArrangeOverride(Size finalSize)
        //{
        //    Size actualSize = base.ArrangeOverride(new Size(finalSize.Width / Scale, finalSize.Height / Scale));
        //    return new Size(actualSize.Width * Scale, actualSize.Height * Scale);
        //}

        private IScrollInfo Child
        {
            get
            {
                return Content as IScrollInfo;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool CanHorizontallyScroll
        {
            get
            {
                return Child.CanHorizontallyScroll;
            }
            set
            {
                Child.CanHorizontallyScroll = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool CanVerticallyScroll
        {
            get
            {
                return Child.CanVerticallyScroll;
            }
            set
            {
                Child.CanVerticallyScroll = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public new double ExtentHeight
        {
            get { return Child.ExtentHeight; }
            internal set { base.ExtentHeight = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public new double ExtentWidth
        {
            get { return Child.ExtentWidth; }
            internal set { base.ExtentWidth = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public new double HorizontalOffset
        {
            get { return Child.HorizontalOffset; }
            internal set { base.HorizontalOffset = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public void LineDown()
        {
            Child.LineDown();
        }
        /// <summary>
        /// 
        /// </summary>
        public void LineLeft()
        {
            Child.LineLeft();
        }
        /// <summary>
        /// 
        /// </summary>
        public void LineRight()
        {
            Child.LineRight();
        }
        /// <summary>
        /// 
        /// </summary>
        public void LineUp()
        {
            Child.LineUp();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="visual"></param>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        public Rect MakeVisible(UIElement visual, Rect rectangle)
        {
            return Child.MakeVisible(visual, rectangle);
        }
        /// <summary>
        /// 
        /// </summary>
        public void MouseWheelDown()
        {
            Child.MouseWheelDown();
        }
        /// <summary>
        /// 
        /// </summary>
        public void MouseWheelLeft()
        {
            Child.MouseWheelLeft();
        }
        /// <summary>
        /// 
        /// </summary>
        public void MouseWheelRight()
        {
            Child.MouseWheelRight();
        }
        /// <summary>
        /// 
        /// </summary>
        public void MouseWheelUp()
        {
            Child.MouseWheelUp();
        }
        /// <summary>
        /// 
        /// </summary>
        public void PageDown()
        {
            Child.PageDown();
        }
        /// <summary>
        /// 
        /// </summary>
        public void PageLeft()
        {
            Child.PageLeft();
        }
        /// <summary>
        /// 
        /// </summary>
        public void PageRight()
        {
            Child.PageRight();
        }
        /// <summary>
        /// 
        /// </summary>
        public void PageUp()
        {
            Child.PageUp();
        }
        /// <summary>
        /// 
        /// </summary>
        public ScrollViewer ScrollOwner
        {
            get
            {
                return Child.ScrollOwner;
            }
            set
            {
                Child.ScrollOwner = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        public void SetHorizontalOffset(double offset)
        {
            Child.SetHorizontalOffset(offset);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        public void SetVerticalOffset(double offset)
        {
            Child.SetVerticalOffset(offset);
        }
        /// <summary>
        /// 
        /// </summary>
        public new double VerticalOffset
        {
            get { return Child.VerticalOffset; }
            internal set { base.VerticalOffset = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public new double ViewportHeight
        {
            get { return Child.ViewportHeight; }
            internal set { base.ViewportHeight = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public new double ViewportWidth
        {
            get { return Child.ViewportWidth; }
            internal set { base.ViewportWidth = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            return base.MeasureOverride(availableSize);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            return base.ArrangeOverride(finalSize);
        }
    }
    
}


#endif