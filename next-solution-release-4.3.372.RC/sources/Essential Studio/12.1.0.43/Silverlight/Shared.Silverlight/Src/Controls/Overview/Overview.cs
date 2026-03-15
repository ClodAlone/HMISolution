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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;
using System.ComponentModel;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// 
    /// </summary>
    public class Overview : Control, IOverviewPanel
    {
        internal bool IsResizing = false;
        internal bool IsResized = false;
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
            DependencyProperty.Register("AllowResize", typeof(bool), typeof(Overview), new PropertyMetadata(true));

        /// <summary>
        /// 
        /// </summary>
        public Overview()
        {
            this.DefaultStyleKey = typeof(Overview);
#if WPF
            this.SizeChanged += new SizeChangedEventHandler(Overview_SizeChanged);
#endif
            this.MouseLeftButtonDown += new MouseButtonEventHandler(Overview_MouseLeftButtonDown);
            this.AddHandler(FrameworkElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(Overview_MouseLeftButtonDown), true);

            //InternalBinding("ScrollSource.Content", ScrollContentProperty);
            //InternalBinding("ScrollSource.ContentTemplate", ScrollContentTemplateProperty);
            InternalBinding("ScrollSource.ZoomIn", ZoomInProperty);
            InternalBinding("ScrollSource.ZoomOut", ZoomOutProperty);
            InternalBinding("ScrollSource.ZoomTo", ZoomToProperty);
            InternalBinding("ScrollSource.ZoomReset", ZoomResetProperty);
            InternalBinding("ScrollSource.ZoomFactor", ZoomFactorProperty);
            InternalBinding("ScrollSource.MinimumZoom", MinimumZoomProperty);
            InternalBinding("ScrollSource.MaximumZoom", MaximumZoomProperty);

            InternalBinding("ScrollSource.HorizontalOffset", HorizontalOffsetProperty);
            InternalBinding("ScrollSource.VerticalOffset", VerticalOffsetProperty);
            InternalBinding("ScrollSource.ViewportWidth", ViewportWidthProperty);
            InternalBinding("ScrollSource.ViewportHeight", ViewportHeightProperty);
            InternalBinding("ScrollSource.ExtentWidth", ExtentWidthProperty);
            InternalBinding("ScrollSource.ExtentHeight", ExtentHeightProperty);

            Binding bin = new Binding("ScrollSource.Scale");
            bin.Mode = BindingMode.TwoWay;
            bin.Source = this;
            SetBinding(Overview.ScaleProperty, bin);
            this.Loaded += new RoutedEventHandler(Overview_Loaded);
            //SourceChange();

            //bin = new Binding("ScrollOwner.HorizontalOffset");
            //bin.Source = this;
            //SetBinding(Overview.HorizontalOffsetProperty, bin);

            //bin = new Binding("ScrollOwner.VerticalOffset");
            //bin.Source = this;
            //SetBinding(Overview.VerticalOffsetProperty, bin);

            //bin = new Binding("ScrollOwner.ViewportWidth");
            //bin.Source = this;
            //SetBinding(Overview.ViewportWidthProperty, bin);

            //bin = new Binding("ScrollOwner.ViewportHeight");
            //bin.Source = this;
            //SetBinding(Overview.ViewportHeightProperty, bin);

        }

        void Overview_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice.GetStylusPoints(sender as Overview)[0].X - (VpWidth / 2) > 0 && (e.StylusDevice.GetStylusPoints(sender as Overview)[0].X+this.VpWidth - (VpWidth / 2) )<=Img.ActualWidth)
            {
                this.VpOffsetX = e.StylusDevice.GetStylusPoints(sender as Overview)[0].X - (VpWidth / 2);
            }
            else if (e.StylusDevice.GetStylusPoints(sender as Overview)[0].X - (VpWidth / 2) < 0)
            {
                this.VpOffsetX = 0;
            }
            else if ((e.StylusDevice.GetStylusPoints(sender as Overview)[0].X + this.VpWidth - (VpWidth / 2)) > Img.ActualWidth)
            {
                this.VpOffsetX = Img.ActualWidth - this.VpWidth;
            }

            if (e.StylusDevice.GetStylusPoints(sender as Overview)[0].Y - (VpHeight) > 0 && (e.StylusDevice.GetStylusPoints(sender as Overview)[0].Y +this.VpHeight - (VpHeight)) <= Img.ActualHeight)
            {
                this.VpOffsetY = e.StylusDevice.GetStylusPoints(sender as Overview)[0].Y - (VpHeight);
            }
            else if (e.StylusDevice.GetStylusPoints(sender as Overview)[0].Y - (VpHeight) < 0)
            {
                this.VpOffsetY = 0;
            }
            else if ((e.StylusDevice.GetStylusPoints(sender as Overview)[0].Y + this.VpHeight - (VpHeight)) > Img.ActualHeight)
            {
                this.VpOffsetY = Img.ActualHeight - this.VpHeight;
            }
            this.Trans = new TranslateTransform() { X = VpOffsetX, Y = VpOffsetY };
            UpdateScrollViewer();
        }


#if WPF
        void Overview_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            UpdatePreview();

        }
#endif
        void OverviewHOlder_SizeChanged(object sender, SizeChangedEventArgs e)
        {
#if WPF
            UpdatePreview();
#endif
        }
        void Overview_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        //private void SourceChange()
        //{
        //    if (OverviewSourceMode == OverviewSourceMode.ScrollViewerMode)
        //    {
        //        InternalBinding("ScrollSource.Content", ScrollContentProperty);
        //        InternalBinding("ScrollSource.ContentTemplate", ScrollContentTemplateProperty);
        //        InternalBinding("ScrollSource.Content.ZoomIn", ZoomInProperty);
        //        InternalBinding("ScrollSource.Content.ZoomOut", ZoomOutProperty);
        //        InternalBinding("ScrollSource.Content.ZoomTo", ZoomToProperty);
        //        InternalBinding("ScrollSource.Content.ZoomReset", ZoomResetProperty);
        //        InternalBinding("ScrollSource.Content.ZoomFactor", ZoomFactorProperty);

        //        InternalBinding("ScrollSource.HorizontalOffset", HorizontalOffsetProperty);
        //        InternalBinding("ScrollSource.VerticalOffset", VerticalOffsetProperty);
        //        InternalBinding("ScrollSource.ViewportWidth", ViewportWidthProperty);
        //        InternalBinding("ScrollSource.ViewportHeight", ViewportHeightProperty);
        //        InternalBinding("ScrollSource.ExtentWidth", ExtentWidthProperty);
        //        InternalBinding("ScrollSource.ExtentHeight", ExtentHeightProperty);

        //        Binding bin = new Binding("ScrollSource.Content.Scale");
        //        bin.Mode = BindingMode.TwoWay;
        //        bin.Source = this;
        //        SetBinding(Overview.ScaleProperty, bin);
        //    }
        //    else if (OverviewSourceMode == Shared.OverviewSourceMode.ControlMode)
        //    {
        //        InternaAttachedlBinding("Content", ScrollContentProperty);
        //        InternaAttachedlBinding("ControlTemplate", ControlContentTemplateProperty);
        //        InternaAttachedlBinding("ZoomIn", ZoomInProperty);
        //        InternaAttachedlBinding("ZoomOut", ZoomOutProperty);
        //        InternaAttachedlBinding("ZoomTo", ZoomToProperty);
        //        InternaAttachedlBinding("ZoomReset", ZoomResetProperty);
        //        InternaAttachedlBinding("ZoomFactor", ZoomFactorProperty);

        //        InternaAttachedlBinding("HorizontalOffset", HorizontalOffsetProperty);
        //        InternaAttachedlBinding("VerticalOffset", VerticalOffsetProperty);
        //        InternaAttachedlBinding("ViewportWidth", ViewportWidthProperty);
        //        InternaAttachedlBinding("ViewportHeight", ViewportHeightProperty);
        //        InternaAttachedlBinding("ExtentWidth", ExtentWidthProperty);
        //        InternaAttachedlBinding("ExtentHeight", ExtentHeightProperty);

        //        //InternaAttachedlBinding("Scale", ScaleProperty);
        //        Binding bin = attachedBinding["ScrollSource.Content.Scale"] as Binding;
        //        bin.Mode = BindingMode.TwoWay;
        //        bin.Source = this;
        //        SetBinding(Overview.ScaleProperty, bin);
        //    }
        //}

        private void InternalBinding(string sourceProp, DependencyProperty dpProp)
        {
            Binding bin = new Binding(sourceProp);
            bin.Source = this;
            SetBinding(dpProp, bin);
        }

        //ResourceDictionary attachedBinding = new ResourceDictionary() { Source = new Uri("/Syncfusion.Shared.Silverlight;component/Themes/AttachedBinding.xaml", UriKind.RelativeOrAbsolute) };

        //private void InternaAttachedlBinding(string sourceProp, DependencyProperty dpProp)
        //{
        //    Binding bind = attachedBinding[sourceProp] as Binding;
        //    bind.Source = this;
        //    SetBinding(dpProp, bind);
        //}

        #region IZoomPanel Implementation
        /// <summary>
        /// 
        /// </summary>
        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Scale.  This enables animation, styling, binding, etc...
        /// </summary>
        internal static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(double), typeof(Overview), new PropertyMetadata(1d, OnScaleChanged));

        private static void OnScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Overview ov = d as Overview;
            double delta = (double)e.NewValue / (double)e.OldValue;
            //ov.DeltaScale(delta, new Point(0.5, 0.5));

        }

        private void DeltaScale(double increment, Point position_percent)
        {
            double SVh = (ScrollSource.HorizontalOffset + ScrollSource.ViewportWidth * position_percent.X) * increment; //ov.ScrollOwner.HorizontalOffset - (ov.ScrollOwner.ViewportWidth * ((double)e.NewValue - (double)e.OldValue + 1) - ov.ScrollOwner.ViewportWidth) / 2;
            //ov.ScrollOwner.ScrollToHorizontalOffset(delta);
            double SVv = (ScrollSource.VerticalOffset + ScrollSource.ViewportHeight * position_percent.Y) * increment;// - (ov.ScrollOwner.ViewportHeight * ((double)e.NewValue - (double)e.OldValue + 1) - ov.ScrollOwner.ViewportHeight) / 2;
            //ov.ScrollOwner.ScrollToVerticalOffset(delta);

            if (ScrollViewer_LayoutUpdated != null)
            {
                ScrollSource.LayoutUpdated -= ScrollViewer_LayoutUpdated;
            }
#if WPF
            ScrollViewer_LayoutUpdated = (s, evt) =>
            {
                ScrollSource.LayoutUpdated -= ScrollViewer_LayoutUpdated;
                ScrollSource.ScrollToHorizontalOffset(SVh - ScrollSource.ViewportWidth * position_percent.X);
                //ScrollSource.HorizontalOffset = (SVh - ScrollSource.ViewportWidth * position_percent.X);
                ScrollSource.ScrollToVerticalOffset(SVv - ScrollSource.ViewportHeight * position_percent.Y);
                //ScrollSource.VerticalOffset = (SVv - ScrollSource.ViewportHeight * position_percent.Y);
            };
#endif
            ScrollSource.LayoutUpdated += ScrollViewer_LayoutUpdated;
            ScrollSource.InvalidateArrange();
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
            DependencyProperty.Register("IsZoomInEnabled", typeof(bool), typeof(Overview), new PropertyMetadata(true));
       
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
            DependencyProperty.Register("ZoomIn", typeof(ICommand), typeof(Overview), new PropertyMetadata(null));
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
            DependencyProperty.Register("IsZoomOutEnabled", typeof(bool), typeof(Overview), new PropertyMetadata(true));
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
            DependencyProperty.Register("ZoomOut", typeof(ICommand), typeof(Overview), new PropertyMetadata(null));
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
            DependencyProperty.Register("IsZoomToEnabled", typeof(bool), typeof(Overview), new PropertyMetadata(true));
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
            DependencyProperty.Register("ZoomTo", typeof(ICommand), typeof(Overview), new PropertyMetadata(null));
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
            DependencyProperty.Register("IsZoomResetEnabled", typeof(bool), typeof(Overview), new PropertyMetadata(true));
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
            DependencyProperty.Register("ZoomReset", typeof(ICommand), typeof(Overview), new PropertyMetadata(null));
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
            DependencyProperty.Register("ZoomFactor", typeof(double), typeof(Overview), new PropertyMetadata(0.2d));
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
            DependencyProperty.Register("MinimumZoom", typeof(double), typeof(Overview), new PropertyMetadata(0.2d));
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
            DependencyProperty.Register("MaximumZoom", typeof(double), typeof(Overview), new PropertyMetadata(10d));

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
            DependencyProperty.Register("ZoomMode", typeof(ZoomMode), typeof(Overview), new PropertyMetadata(ZoomMode.Unit));


        #endregion

        /// <summary>
        /// 
        /// </summary>
        public Brush ContentBackground
        {
            get { return (Brush)GetValue(ContentBackgroundProperty); }
            set { SetValue(ContentBackgroundProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ContentBackground.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ContentBackgroundProperty =
            DependencyProperty.Register("ContentBackground", typeof(Brush), typeof(Overview), new PropertyMetadata(null));
        /// <summary>
        /// 
        /// </summary>
        public Brush ViewPortBrush
        {
            get { return (Brush)GetValue(ViewPortBrushProperty); }
            set { SetValue(ViewPortBrushProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ViewPortBrush.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ViewPortBrushProperty =
            DependencyProperty.Register("ViewPortBrush", typeof(Brush), typeof(Overview), new PropertyMetadata(null));
        /// <summary>
        /// 
        /// </summary>
        public DependencyObject OverviewSourceAncestor
        {
            get { return (DependencyObject)GetValue(OverviewSourceAncestorProperty); }
            set { SetValue(OverviewSourceAncestorProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for OverviewSourceAncestor.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty OverviewSourceAncestorProperty =
            DependencyProperty.Register("OverviewSourceAncestor", typeof(DependencyObject), typeof(Overview), new PropertyMetadata(null, OnOverviewSourceAncestorChanged));

        private static void OnOverviewSourceAncestorChanged(DependencyObject dp, DependencyPropertyChangedEventArgs evtArgs)
        {
            if (evtArgs.NewValue != null)
            {
                Overview ov = dp as Overview;
                //ov.ScrollOwner = FindScrollViewer(evtArgs.NewValue as DependencyObject);
                ov.UpdateSource();
            }
        }

        private static T FindChildScrollViewer<T>(DependencyObject depObj) where T : UIElement
        {
            if (depObj is T)
            {
                return depObj as T;
            }
            int count = VisualTreeHelper.GetChildrenCount(depObj);
            while (count > 0)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, count - 1);
                count--;
                if (child is T)
                {
                    return child as T;
                }
                else if (child == null)
                {
                    return null;
                }
                else
                {
                    child = FindChildScrollViewer<T>(child);
                    if (child is T)
                    {
                        return child as T;
                    }
                }
            }
            return null;
        }

        //private static ScrollViewer FindChildScrollViewer(DependencyObject depObj)
        //{
        //    if (depObj is ScrollViewer)
        //    {
        //        return depObj as ScrollViewer;
        //    }
        //    int count = VisualTreeHelper.GetChildrenCount(depObj);
        //    while (count > 0)
        //    {
        //        DependencyObject child = VisualTreeHelper.GetChild(depObj, count - 1);
        //        count--;
        //        if (child is ScrollViewer)
        //        {
        //            return child as ScrollViewer;
        //        }
        //        else if (child == null)
        //        {
        //            return null;
        //        }
        //        else
        //        {
        //            child = FindChildScrollViewer(child);
        //            if (child is ScrollViewer)
        //            {
        //                return child as ScrollViewer;
        //            }
        //        }
        //    }
        //    return null;
        //}

        //private static T FindAttachedScrollViewer<T>(DependencyObject depObj) where T : UIElement
        //{
        //    T sv = Overview.GetOverviewSource(depObj) as T;
        //    if (sv != null)
        //    {
        //        return sv;
        //    }
        //    int count = VisualTreeHelper.GetChildrenCount(depObj);
        //    while (count > 0)
        //    {
        //        DependencyObject child = VisualTreeHelper.GetChild(depObj, count - 1);
        //        T attachedSV = Overview.GetOverviewSource(child) as T;
        //        count--;
        //        if (attachedSV is ScrollViewer)
        //        {
        //            return attachedSV;
        //        }
        //        else if (child == null)
        //        {
        //            return null;
        //        }
        //        else
        //        {
        //            attachedSV = FindAttachedScrollViewer<T>(child);
        //            if (attachedSV is ScrollViewer)
        //            {
        //                return attachedSV as T;
        //            }
        //        }
        //    }
        //    return null;
        //}

        //private static ScrollViewer FindAttachedScrollViewer(DependencyObject depObj)
        //{
        //    ScrollViewer sv = Overview.GetOverviewSource(depObj) as ScrollViewer;
        //    if (sv != null)
        //    {
        //        return sv;
        //    }
        //    int count = VisualTreeHelper.GetChildrenCount(depObj);
        //    while (count > 0)
        //    {
        //        DependencyObject child = VisualTreeHelper.GetChild(depObj, count - 1);
        //        ScrollViewer attachedSV = Overview.GetOverviewSource(child) as ScrollViewer;
        //        count--;
        //        if (attachedSV is ScrollViewer)
        //        {
        //            return attachedSV;
        //        }
        //        else if (child == null)
        //        {
        //            return null;
        //        }
        //        else
        //        {
        //            attachedSV = FindAttachedScrollViewer(child);
        //            if (attachedSV is ScrollViewer)
        //            {
        //                return attachedSV as ScrollViewer;
        //            }
        //        }
        //    }
        //    return null;
        //}

        //internal Control ControlSource
        //{
        //    get { return (Control)GetValue(ControlSourceProperty); }
        //    set { SetValue(ControlSourceProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for ControlOwner.  This enables animation, styling, binding, etc...
        //internal static readonly DependencyProperty ControlSourceProperty =
        //    DependencyProperty.Register("ControlSource", typeof(Control), typeof(Overview), new PropertyMetadata(null, OnControlSourceChanged));

        //private static void OnControlSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    Overview ov = d as Overview;
        //    if (e.NewValue is Control)
        //    {
        //        Overview.SetContent((e.NewValue as Control), VisualTreeHelper.GetChild(e.NewValue as Control, 0));
        //    }
        //    ov.SourceChange();
        //}

        //public static DependencyObject GetContent(DependencyObject obj)
        //{
        //    return (DependencyObject)obj.GetValue(ContentProperty);
        //}

        //public static void SetContent(DependencyObject obj, DependencyObject value)
        //{
        //    obj.SetValue(ContentProperty, value);
        //}

        //// Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty ContentProperty =
        //    DependencyProperty.RegisterAttached("Content", typeof(DependencyObject), typeof(Overview), new PropertyMetadata(null));

        internal OverviewContentHolder ScrollSource
        {
            get { return (OverviewContentHolder)GetValue(ScrollSourceProperty); }
            set { SetValue(ScrollSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScrollOwner.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ScrollSourceProperty =
            DependencyProperty.Register("ScrollSource", typeof(OverviewContentHolder), typeof(Overview), new PropertyMetadata(null, OnScrollSourceChanged));

        internal DependencyObject ScrollContentTarget
        {
            get { return (DependencyObject)GetValue(ScrollContentTargetProperty); }
            set { SetValue(ScrollContentTargetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScrollContentTarget.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ScrollContentTargetProperty =
            DependencyProperty.Register("ScrollContentTarget", typeof(DependencyObject), typeof(Overview), new PropertyMetadata(null, OnScrollContentTargetChanged));

        private static void OnScrollContentTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null && e.OldValue is FrameworkElement)
            {
                (e.OldValue as FrameworkElement).Unloaded -= new RoutedEventHandler((d as Overview).Overview_Unloaded);
            }
            if (e.NewValue != null &&e.NewValue is FrameworkElement)
            {
                (e.NewValue as FrameworkElement).Unloaded += new RoutedEventHandler((d as Overview).Overview_Unloaded);
            }
        }

        void Overview_Unloaded(object sender, RoutedEventArgs e)
        {

            (sender as FrameworkElement).Unloaded -= Overview_Unloaded;
            UpdateScrollContentTarget();
        }

        private static void OnScrollSourceChanged(DependencyObject dp, DependencyPropertyChangedEventArgs evtArgs)
        {
            Overview ov = dp as Overview;
#if WPF
            ov.ScrollSource._ParentView = ov;
#endif
            //ov.SourceChange();
            //if (evtArgs.OldValue != null)
            //{
            //    ov.ScrollOwner.LayoutUpdated -= new EventHandler(ov.ScrollOwner_LayoutUpdated);
            //    //if (ov.ScrollOwner.Content is ZoomPanel)
            //    //{
            //    //    ov.ScrollOwner.Content = (ov.ScrollOwner.Content as ZoomPanel).Content;
            //    //}
            //}
            //if (evtArgs.NewValue != null)
            //{
            //    //ov.InjectZoomPanel();
            //    ov.ScrollOwner.LayoutUpdated += new EventHandler(ov.ScrollOwner_LayoutUpdated);
            //}
            ov.UpdateScrollContentTarget();
#if WPF
            ov.UpdatePreview();
#endif
        }
#if WPF
        internal void UpdatePreview()
        {
            if (ScrollSource != null)
            {
                double WidthX = (ScrollSource.ExtentWidth-ScrollSource.Left) / this.ActualWidth;
                double HeigthY = (ScrollSource.ExtentHeight-ScrollSource.Top) / this.ActualHeight;
                Img.Margin = ScrollSource.Margin;
                if (WidthX > HeigthY)
                {
                    this.Img.Width = this.ActualWidth;//this.ActualHeight * (ScrollSource.ExtentWidth / this.ScrollSource.ExtentHeight);
                    this.Img.Height = this.ActualWidth * ((ScrollSource.ExtentHeight - ScrollSource.Top) /( ScrollSource.ExtentWidth - ScrollSource.Left));
                }
                else
                {
                    this.Img.Height = this.ActualHeight;
                    this.Img.Width = this.ActualHeight * ((ScrollSource.ExtentWidth - ScrollSource.Left)/ (this.ScrollSource.ExtentHeight - ScrollSource.Top));
                }
            }
        }
#endif
        //private void InjectZoomPanel()
        //{
        //    if (ScrollSource != null && !(ScrollSource.Content is OverviewPanel))
        //    {
        //        OverviewPanel panel = null;
        //        if (ScrollSource.Content is System.Windows.Controls.Primitives.IScrollInfo)
        //        {
        //            panel = new IScrollInfoOverviewPanel();
        //        }
        //        else
        //        {
        //            panel = new OverviewPanel();

        //        }
        //        object cont = ScrollSource.Content;
        //        DataTemplate dt = ScrollSource.ContentTemplate;
        //        ScrollSource.Content = null;
        //        ScrollSource.ContentTemplate = null;
        //        panel.Content = cont;
        //        panel.ContentTemplate = dt;
        //        ScrollSource.Content = panel;
        //    }

        //    if (ScrollSource != null && ScrollSource.Content is OverviewPanel)
        //    {
        //        (ScrollSource.Content as OverviewPanel).MouseWheel -= new MouseWheelEventHandler(ZoomPanel_MouseWheel);
        //        (ScrollSource.Content as OverviewPanel).MouseWheel += new MouseWheelEventHandler(ZoomPanel_MouseWheel);
        //    }

        //    if (ScrollContent!=null && !(ScrollContent is OverviewPanel))
        //    {
        //        OverviewPanel panel = null;
        //        //if (ScrollSource.Content is System.Windows.Controls.Primitives.IScrollInfo)
        //        //{
        //        //    panel = new IScrollInfoZoomPanel();
        //        //}
        //        //else
        //        //{
        //        panel = new OverviewPanel();
        //        //}
        //        //object cont = ScrollSource.Content;
        //        //DataTemplate dt = ScrollSource.ContentTemplate;
        //        //ScrollSource.Content = null;
        //        //ScrollSource.ContentTemplate = null;
        //        //panel.Content = cont;
        //        //panel.ContentTemplate = dt;
        //        //ScrollSource.Content = panel;
        //    }
        //}

        

        //private void RemoveZoomPanel()
        //{
        //    if (ScrollSource.Content is OverviewPanel)
        //    {
        //        ScrollSource.Content = (ScrollSource.Content as OverviewPanel).Content;
        //    }
        //}

        #region ScrollViewer

        //internal object ScrollContent
        //{
        //    get { return (object)GetValue(ScrollContentProperty); }
        //    set { SetValue(ScrollContentProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for ScollContent.  This enables animation, styling, binding, etc...
        //internal static readonly DependencyProperty ScrollContentProperty =
        //    DependencyProperty.Register("ScrollContent", typeof(object), typeof(Overview), new PropertyMetadata(null, OnScrollContentChanged));

        //private static void OnScrollContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    Overview ov = d as Overview;
        //    if (e.NewValue != null)
        //    {
        //        if (ov.ScrollSource != null || ov.ControlSource != null)
        //        {
        //            ov.InjectZoomPanel();
        //        }
        //    }

        //    if (e.OldValue != null)
        //    {
        //        if (ov.ScrollSource != null || ov.ControlSource != null)
        //        {
        //            ov.RemoveZoomPanel();
        //        }
        //    }
        //}

        //internal DataTemplate ScrollContentTemplate
        //{
        //    get { return (DataTemplate)GetValue(ScrollContentTemplateProperty); }
        //    set { SetValue(ScrollContentTemplateProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for ScrollContentTemplate.  This enables animation, styling, binding, etc...
        //internal static readonly DependencyProperty ScrollContentTemplateProperty =
        //    DependencyProperty.Register("ScrollContentTemplate", typeof(DataTemplate), typeof(Overview), new PropertyMetadata(OnScrollContentTemplateChanged));

        //private static void OnScrollContentTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    Overview ov = d as Overview;
        //    if (e.NewValue != null)
        //    {
        //        if (ov.ScrollSource != null)
        //        {
        //            ov.InjectZoomPanel();
        //        }
        //    }

        //    if (e.OldValue != null)
        //    {
        //        if (ov.ScrollSource != null)
        //        {
        //            ov.RemoveZoomPanel();
        //        }
        //    }
        //}

        //internal ControlTemplate ControlContentTemplate
        //{
        //    get { return (ControlTemplate)GetValue(ControlContentTemplateProperty); }
        //    set { SetValue(ControlContentTemplateProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for ControlContentTemplate.  This enables animation, styling, binding, etc...
        //internal static readonly DependencyProperty ControlContentTemplateProperty =
        //    DependencyProperty.Register("ControlContentTemplate", typeof(ControlTemplate), typeof(Overview), new PropertyMetadata(null, OnControlContentTemplateChanged));

        //private static void OnControlContentTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //}

        internal double HorizontalOffset
        {
            get { return (double)GetValue(HorizontalOffsetProperty); }
            set { SetValue(HorizontalOffsetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HorizontalOffset.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register("HorizontalOffset", typeof(double), typeof(Overview), new PropertyMetadata(0d, OnHorizontalOffsetChanged));

        static private void OnHorizontalOffsetChanged(DependencyObject dp, DependencyPropertyChangedEventArgs evtArgs)
        {
            Overview over = dp as Overview; 
            (dp as Overview).UpdateVp();
            //if (over.ScrollSource != null)
            //{
            //    over.ScrollSource._Offset.X = over.HorizontalOffset;
            //}
        }

        internal double VerticalOffset
        {
            get { return (double)GetValue(VerticalOffsetProperty); }
            set { SetValue(VerticalOffsetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VerticalOffset.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register("VerticalOffset", typeof(double), typeof(Overview), new PropertyMetadata(0d, OnVerticalIffsetChanged));

        static private void OnVerticalIffsetChanged(DependencyObject dp, DependencyPropertyChangedEventArgs evtArgs)
        {
            (dp as Overview).UpdateVp();
        }

        internal double ViewportWidth
        {
            get { return (double)GetValue(ViewportWidthProperty); }
            set { SetValue(ViewportWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewportWidth.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ViewportWidthProperty =
            DependencyProperty.Register("ViewportWidth", typeof(double), typeof(Overview), new PropertyMetadata(0d, OnViewportWidthChanged));

        private static void OnViewportWidthChanged(DependencyObject dp, DependencyPropertyChangedEventArgs evtArgs)
        {
            (dp as Overview).UpdateVp();
        }

        internal double ViewportHeight
        {
            get { return (double)GetValue(ViewportHeightProperty); }
            set { SetValue(ViewportHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewportHeight.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ViewportHeightProperty =
            DependencyProperty.Register("ViewportHeight", typeof(double), typeof(Overview), new PropertyMetadata(0d, OnViewportHeightChanged));

        private static void OnViewportHeightChanged(DependencyObject dp, DependencyPropertyChangedEventArgs evtArgs)
        {
            (dp as Overview).UpdateVp();
        }

        internal double ExtentWidth
        {
            get { return (double)GetValue(ExtentWidthProperty); }
            set { SetValue(ExtentWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExtentWidth.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ExtentWidthProperty =
            DependencyProperty.Register("ExtentWidth", typeof(double), typeof(Overview), new PropertyMetadata(0d, OnExtentWidthChanged));

        private static void OnExtentWidthChanged(DependencyObject dp, DependencyPropertyChangedEventArgs evtArgs)
        {
            (dp as Overview).UpdateVp();
        }

        internal double ExtentHeight
        {
            get { return (double)GetValue(ExtentHeightProperty); }
            set { SetValue(ExtentHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExtentHeight.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ExtentHeightProperty =
            DependencyProperty.Register("ExtentHeight", typeof(double), typeof(Overview), new PropertyMetadata(0d, OnExtentHeightChanged));

        private static void OnExtentHeightChanged(DependencyObject dp, DependencyPropertyChangedEventArgs evtArgs)
        {
            (dp as Overview).UpdateVp();
        }

        #endregion

        #region ViewPort

        internal double VpOffsetX
        {
            get { return (double)GetValue(VpOffsetXProperty); }
            set
            {
                //value = Math.Min(/*Space.X + */Img.ActualWidth - VpWidth, value);
                //value = Math.Max(/*Space.X*/0, value);
                SetValue(VpOffsetXProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for VpOffsetX.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty VpOffsetXProperty =
            DependencyProperty.Register("VpOffsetX", typeof(double), typeof(Overview), new PropertyMetadata(0d, OnVpOffsetChanged));

        private static void OnVpOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Overview ov = d as Overview;
            if (ov.VpOffsetX < 0)
            {
                ov.WindowWidth = ov.VpWidth + ov.VpOffsetX;
            }
            else
            {
                ov.WindowWidth = ov.VpWidth;
            }
            if (ov.VpOffsetY < 0)
            {
                ov.WindowHeight = ov.VpHeight + ov.VpOffsetY;
            }
            else
            {
                ov.WindowHeight = ov.VpHeight;
            }
        }

        internal double VpOffsetY
        {
            get { return (double)GetValue(VpOffsetYProperty); }
            set
            {
                //value = Math.Min(/*Space.Y + */Img.ActualHeight - VpHeight, value);
                //value = Math.Max(/*Space.Y*/0, value);
                SetValue(VpOffsetYProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for VpOffsetY.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty VpOffsetYProperty =
            DependencyProperty.Register("VpOffsetY", typeof(double), typeof(Overview), new PropertyMetadata(0d, OnVpOffsetChanged));

        internal double VpWidth
        {
            get { return (double)GetValue(VpWidthProperty); }
            set
            {
                //value = Math.Min(value, Img.ActualWidth - VpOffsetX /*+ Space.X*/);
                value = Math.Max(5, value);
                SetValue(VpWidthProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for VpWidth.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty VpWidthProperty =
            DependencyProperty.Register("VpWidth", typeof(double), typeof(Overview), new PropertyMetadata(0d, OnVpOffsetChanged));

        internal double VpHeight
        {
            get { return (double)GetValue(VpHeightProperty); }
            set
            {
                //value = Math.Min(value, Img.ActualHeight - VpOffsetY /*+ Space.Y*/);
                value = Math.Max(5, value);
                SetValue(VpHeightProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for VpHeight.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty VpHeightProperty =
            DependencyProperty.Register("VpHeight", typeof(double), typeof(Overview), new PropertyMetadata(0d, OnVpOffsetChanged));

        internal double WindowWidth
        {
            get { return (double)GetValue(WindowWidthProperty); }
            set { SetValue(WindowWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WindowWidth.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty WindowWidthProperty =
            DependencyProperty.Register("WindowWidth", typeof(double), typeof(Overview), new PropertyMetadata(0d));

        internal double WindowHeight
        {
            get { return (double)GetValue(WindowHeightProperty); }
            set { SetValue(WindowHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WidowHeight.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty WindowHeightProperty =
            DependencyProperty.Register("WindowHeight", typeof(double), typeof(Overview), new PropertyMetadata(0d));

        internal Transform Trans
        {
            get { return (Transform)GetValue(TransProperty); }
            set { SetValue(TransProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Trans.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty TransProperty =
            DependencyProperty.Register("Trans", typeof(Transform), typeof(Overview), new PropertyMetadata(null));

        #endregion

        #region ImageUpdate

        //internal VisualImage VImg { get; set; }
#if SILVERLIGHT
        internal Image Img { get; set; }
#endif
#if WPF
        internal Rectangle Img { get; set; }
#endif

        //bool Invalidate = false;

        void Img_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateVp();
        }

        #endregion

        #region Update VP SV

        private void UpdateVp()
        {
            if (!IsResizing)
            {
                this.ScrollSource.LayoutUpdated -= new EventHandler(Overview_LayoutUpdated_UpdateVP);
                this.ScrollSource.LayoutUpdated += new EventHandler(Overview_LayoutUpdated_UpdateVP);
                this.ScrollSource.InvalidateArrange();
            }
            else
            {
                //Invalidate = true;
            }
        }

        internal void InvalidateScroll()
        {
            if (ScrollSource != null)
            {
                //Point Space = new Point();
                double H1, H2, H3, V1, V2, V3;
                H1 = ScrollSource.HorizontalOffset / ScrollSource.ExtentWidth;
                H2 = ScrollSource.ViewportWidth / ScrollSource.ExtentWidth;
                H3 = (ScrollSource.ExtentWidth - ScrollSource.HorizontalOffset - ScrollSource.ViewportWidth) / ScrollSource.ExtentWidth;

                V1 = ScrollSource.VerticalOffset / ScrollSource.ExtentHeight;
                V2 = ScrollSource.ViewportHeight / ScrollSource.ExtentHeight;
                V3 = (ScrollSource.ExtentHeight - ScrollSource.VerticalOffset - ScrollSource.ViewportHeight) / ScrollSource.ExtentHeight;

                if (Img == null || Img == null)
                {
                    return;
                }
#if SILVERLIGHT
                //if (ImageThread.IsBusy)
                //{
                //    Invalidate = true;
                //}
#endif
                VpOffsetX =(H1 * Img.ActualWidth);
                VpOffsetY = (V1 * Img.ActualHeight);

                VpWidth = H2 * Img.ActualWidth;
                VpHeight = V2 * Img.ActualHeight;


                Trans = new TranslateTransform() { X = VpOffsetX, Y = VpOffsetY };
            }
        }

        internal void UpdateSourceScroll()
        {
            OverviewContentHolder panel = this.ScrollSource as OverviewContentHolder;
            if (panel != null)
            {
                double _ExtentWidth, _ExtentHeight, _ViewportWidth, _ViewportHeight, _HorizontalOffset, _VerticalOffset;
                _ExtentWidth = Img.ActualWidth;
                _ExtentHeight = Img.ActualHeight;
                _ViewportHeight = VpHeight;
                _ViewportWidth = VpWidth;
                _HorizontalOffset = VpOffsetX; 
                _VerticalOffset = VpOffsetY; 

                double SVeWidth = ScrollSource.ViewportWidth * _ExtentWidth / _ViewportWidth;
                double delta = (SVeWidth / ScrollSource.ExtentWidth);
                double SVh = SVeWidth * _HorizontalOffset / _ExtentWidth;
                double SVeHeight = ScrollSource.ViewportHeight * _ExtentHeight / _ViewportHeight;
                double SVv = SVeHeight * _VerticalOffset / _ExtentHeight;

                if (0.99 < delta && delta < 1.01)
                {
#if WPF
                    ScrollSource.ScrollToHorizontalOffset(SVh);
                    ScrollSource.ScrollToVerticalOffset(SVv);

#endif
                }
                else
                {
                    this.UnitScale *= delta;
                    if (ScrollViewer_LayoutUpdated != null)
                    {
                        ScrollSource.LayoutUpdated -= ScrollViewer_LayoutUpdated;
                    }
                    ScrollViewer_LayoutUpdated = (s, evt) =>
                    {
                        ScrollSource.LayoutUpdated -= ScrollViewer_LayoutUpdated;
#if WPF
                        ScrollSource.ScrollToHorizontalOffset(SVh);
                        ScrollSource.ScrollToVerticalOffset(SVv);

#endif
                    };
                }
            }
        }
        internal void UpdateScrollViewer()
        {
            this.LayoutUpdated -= new EventHandler(Overview_LayoutUpdated_UpdateSV);
            this.LayoutUpdated += new EventHandler(Overview_LayoutUpdated_UpdateSV);
            this.InvalidateArrange();
        }

        void Overview_LayoutUpdated_UpdateVP(object sender, EventArgs e)
        {
            this.ScrollSource.LayoutUpdated -= new EventHandler(Overview_LayoutUpdated_UpdateVP);
#if SILVERLIGHT
            IScrollOverviewContentHolder IScrollSource = this.ScrollSource as IScrollOverviewContentHolder;
            if (ScrollSource is IScrollOverviewContentHolder)
            {
                //Point Space = new Point();
                double H1, H2, H3, V1, V2, V3;
                H1 = IScrollSource.HorizontalOffset / IScrollSource.ExtentWidth;
                H2 = IScrollSource.ViewportWidth / IScrollSource.ExtentWidth;
                H3 = (IScrollSource.ExtentWidth - IScrollSource.HorizontalOffset - IScrollSource.ViewportWidth) / IScrollSource.ExtentWidth;

                V1 = IScrollSource.VerticalOffset / IScrollSource.ExtentHeight;
                V2 = IScrollSource.ViewportHeight / IScrollSource.ExtentHeight;
                V3 = (IScrollSource.ExtentHeight - IScrollSource.VerticalOffset - IScrollSource.ViewportHeight) / IScrollSource.ExtentHeight;

                if (Img == null || Img == null)
                {
                    return;
                }
                //if (ImageThread.IsBusy)
                //{
                //    Invalidate = true;
                //}

                VpOffsetX = /*Space.X + */(H1 * Img.ActualWidth);
                VpOffsetY = /*Space.Y + */(V1 * Img.ActualHeight);

                VpWidth = H2 * Img.ActualWidth;
                VpHeight = V2 * Img.ActualHeight;


                Trans = new TranslateTransform() { X = VpOffsetX, Y = VpOffsetY };
            }
#endif
             if (ScrollSource != null)
            {
                //Point Space = new Point();
                double H1, H2, H3, V1, V2, V3;
                H1 = ScrollSource.HorizontalOffset / ScrollSource.ExtentWidth;
                H2 = ScrollSource.ViewportWidth / ScrollSource.ExtentWidth;
                H3 = (ScrollSource.ExtentWidth - ScrollSource.HorizontalOffset - ScrollSource.ViewportWidth) / ScrollSource.ExtentWidth;

                V1 = ScrollSource.VerticalOffset / ScrollSource.ExtentHeight;
                V2 = ScrollSource.ViewportHeight / ScrollSource.ExtentHeight;
                V3 = (ScrollSource.ExtentHeight - ScrollSource.VerticalOffset - ScrollSource.ViewportHeight) / ScrollSource.ExtentHeight;
                 
                if (Img == null || Img == null)
                {
                    return;
                }
#if SILVERLIGHT
                //if (ImageThread.IsBusy)
                //{
                //    Invalidate = true;
                //}
#endif
                VpOffsetX = /*Space.X + */(H1 * Img.ActualWidth);
                VpOffsetY = /*Space.Y + */(V1 * Img.ActualHeight);

                VpWidth = H2 * Img.ActualWidth;
                VpHeight = V2 *Img.ActualHeight;


                Trans = new TranslateTransform() { X = VpOffsetX, Y = VpOffsetY };
            }
        }
        
        void Overview_LayoutUpdated_UpdateSV(object sender, EventArgs e)
        {
            this.LayoutUpdated -= new EventHandler(Overview_LayoutUpdated_UpdateSV);           
            if (Img == null || Img == null)
            {
                return;
            }
#if WPF
            UpdatePreview();
#endif
            OverviewContentHolder panel = this.ScrollSource as OverviewContentHolder;
            if (panel != null)
            {
                double _ExtentWidth, _ExtentHeight, _ViewportWidth, _ViewportHeight, _HorizontalOffset, _VerticalOffset;

                _ExtentWidth = Img.ActualWidth;
                _ExtentHeight = Img.ActualHeight;
                _ViewportHeight = VpHeight;
                _ViewportWidth = VpWidth;
                _HorizontalOffset = VpOffsetX; //- Space.X;
                _VerticalOffset = VpOffsetY; //- Space.Y;
                
                double SVeWidth = ScrollSource.ViewportWidth * _ExtentWidth / _ViewportWidth;
                double delta = (SVeWidth / ScrollSource.ExtentWidth);
                double SVh = SVeWidth * _HorizontalOffset / _ExtentWidth;
                double SVeHeight = ScrollSource.ViewportHeight * _ExtentHeight / _ViewportHeight;
                double SVv = SVeHeight * _VerticalOffset / _ExtentHeight;

                //double SVeHeight = ScrollOwner.ViewportHeight * _ExtentHeight / _ViewportHeight;
                
                if (0.99 < delta && delta < 1.01)
                {
#if SILVERLIGHT
                ScrollSource.HorizontalOffset = SVh;
                ScrollSource.VerticalOffset = SVv;
#endif
#if WPF
                    ScrollSource.ScrollToHorizontalOffset(SVh);                
                    ScrollSource.ScrollToVerticalOffset(SVv);
                  
#endif
                }
                else
                {
                    this.UnitScale *= delta;
                    /* Update Horizontal and Vertical Offset
                     * Solution 1:
                     * Gives a small flicker while update
                     */
                    //ScrollOwner.Dispatcher.BeginInvoke(() =>
                    //{
                    //    ScrollOwner.ScrollToHorizontalOffset(SVh);
                    //    ScrollOwner.ScrollToVerticalOffset(SVv);
                    //});

                    /* Update Horizontal and Vertical Offset
                     * Solution 2:
                     * Gives proper update but frequent new delegate
                     */
                    if (ScrollViewer_LayoutUpdated != null)
                    {
                        ScrollSource.LayoutUpdated -= ScrollViewer_LayoutUpdated;
                    }
                    ScrollViewer_LayoutUpdated = (s, evt) =>
                    {
                        ScrollSource.LayoutUpdated -= ScrollViewer_LayoutUpdated;
#if SILVERLIGHT
                        ScrollSource.HorizontalOffset = SVh;
                        ScrollSource.VerticalOffset = SVv;
#endif
#if WPF
                        ScrollSource.ScrollToHorizontalOffset(SVh);                      
                        ScrollSource.ScrollToVerticalOffset(SVv);
                      
#endif
                    };
                    ScrollSource.LayoutUpdated += ScrollViewer_LayoutUpdated;
                }
            }
        }

        private EventHandler ScrollViewer_LayoutUpdated = null;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
#if SILVERLIGHT
            Img = GetTemplateChild("PART_PreviewImage") as Image;
#endif
#if WPF
            Img = GetTemplateChild("PART_PreviewImage") as Rectangle;
#endif
            if (Img != null)
            {
                Img.SizeChanged += new SizeChangedEventHandler(Img_SizeChanged);
            }

            System.Windows.Controls.Primitives.Thumb drag = GetTemplateChild("PART_DragResizer") as System.Windows.Controls.Primitives.Thumb;
            drag.DragStarted += new System.Windows.Controls.Primitives.DragStartedEventHandler(drag_DragStarted);
            drag.DragDelta += new System.Windows.Controls.Primitives.DragDeltaEventHandler(drag_DragDelta);
            drag.DragCompleted += new System.Windows.Controls.Primitives.DragCompletedEventHandler(drag_DragCompleted);
            UpdateSource();

            UpdateScrollContentTarget();
        }

        private void UpdateScrollContentTarget()
        {
            if (ScrollSource != null && VisualTreeHelper.GetChildrenCount(ScrollSource) > 0)
            {
                DependencyObject dp = VisualTreeHelper.GetChild(ScrollSource, 0);
                if (dp is ContentPresenter)
                {
                    dp = (dp as ContentPresenter).Content as DependencyObject;
                }
                if (dp.ToString().Contains("FourQuadrantPanel"))
                {
                    dp = VisualTreeHelper.GetChild(dp as DependencyObject, 2);
                    if (dp is ContentPresenter)
                    {
                        dp = (dp as ContentPresenter).Content as DependencyObject;
                    }
                }
                ScrollContentTarget = dp;
            }
        }

        internal void UpdateSource()
        {
            if (OverviewSourceAncestor != null && ScrollSource == null)
            {
                //if (OverviewSourceMode == Shared.OverviewSourceMode.ScrollViewerMode)
                {
                    ScrollSource = FindScrollViewer<OverviewContentHolder>(OverviewSourceAncestor);
                    if (ScrollSource == null && OverviewSourceAncestor is FrameworkElement)
                    {
                        (OverviewSourceAncestor as FrameworkElement).Loaded += new RoutedEventHandler(ScrollSource_Loaded);
                    }
                }
            }
            //if (OverviewSourceAncestor != null && ControlSource == null)
            //{
            //    if (OverviewSourceMode == Shared.OverviewSourceMode.ControlMode)
            //    {
            //        ControlSource = FindScrollViewer<Control>(OverviewSourceAncestor);
            //    }
            //}
        }

        void ScrollSource_Loaded(object sender, RoutedEventArgs e)
        {
            this.SizeChanged += new SizeChangedEventHandler(OverviewHOlder_SizeChanged);
            (OverviewSourceAncestor as FrameworkElement).Loaded -= new RoutedEventHandler(ScrollSource_Loaded);
            UpdateSource();
        }

        private Point m_DragDelta = new Point(0, 0);

        void drag_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            m_DragDelta.X = VpOffsetX;
            m_DragDelta.Y = VpOffsetY;
            this.IsResizing = true;
        }

        void drag_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if ((m_DragDelta.X + e.HorizontalChange) > 0 && (m_DragDelta.X + e.HorizontalChange+VpWidth) < Img.ActualWidth)
            {
                m_DragDelta.X += e.HorizontalChange;
            }
            if ((m_DragDelta.Y + e.VerticalChange) > 0 && (m_DragDelta.Y + e.VerticalChange + VpHeight) < Img.ActualHeight)
            {
                m_DragDelta.Y += e.VerticalChange;
            }
            //if ( m_DragDelta.Y < 0)
            //{
            //    ScrollSource.Top = -m_DragDelta.Y;
            //    ScrollSource.InvalidateArrange();
            //    ScrollSource.InvalidateArrange();
            //}
            this.VpOffsetX = m_DragDelta.X;// e.HorizontalChange;
            this.VpOffsetY = m_DragDelta.Y;// e.VerticalChange;            
            this.Trans = new TranslateTransform() { X = VpOffsetX, Y = VpOffsetY };
            UpdateScrollViewer();
        }

        void drag_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            this.IsResizing = false;
        }

        private static T FindScrollViewer<T>(DependencyObject obj) where T : UIElement
        {
            //T sv = FindAttachedScrollViewer<T>(obj);
            //if (sv == null)
            //{
                return FindChildScrollViewer<T>(obj);
            //}
            //return sv;
        }
    }    
}
