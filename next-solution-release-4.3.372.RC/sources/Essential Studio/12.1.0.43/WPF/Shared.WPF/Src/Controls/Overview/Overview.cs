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
using System.Windows.Controls.Primitives;
#if WPF
using Syncfusion.Licensing;
#endif 

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
            DependencyProperty.Register("IsPanEnabled", typeof(bool), typeof(Overview), new PropertyMetadata(false));        
        /// <summary>
        /// 
        /// </summary>
        public Overview()
        {
            DefaultStyleKey = typeof(Overview);
#if WPF 
            if (EnvironmentTest.IsSecurityGranted)
            {
                EnvironmentTest.StartValidateLicense(typeof(Overview));
            }
#endif 
            InternalBinding("ScrollSource.ZoomIn", ZoomInProperty);
            InternalBinding("ScrollSource.ZoomOut", ZoomOutProperty);
            InternalBinding("ScrollSource.ZoomTo", ZoomToProperty);
            InternalBinding("ScrollSource.ZoomReset", ZoomResetProperty);
            InternalBinding("ScrollSource.ZoomFactor", ZoomFactorProperty);
            InternalBinding("ScrollSource.MinimumZoom", MinimumZoomProperty);
            InternalBinding("ScrollSource.MaximumZoom", MaximumZoomProperty);           

            //Binding bin = new Binding("ScrollSource.Scale");
            //bin.Mode = BindingMode.TwoWay;
            //bin.Source = this;
            //SetBinding(Overview.ScaleProperty, bin);
        }

        private void InternalBinding(string sourceProp, DependencyProperty dpProp)
        {
            Binding bin = new Binding(sourceProp);
            bin.Source = this;
            SetBinding(dpProp, bin);
        }
        
        #region IZoomPanel Implementation
        /// <summary>
        /// 
        /// </summary>
        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Scale.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(double), typeof(Overview), new PropertyMetadata(1d));
        
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

        // Using a DependencyProperty as the backing store for IsZoomInEnabled.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
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

        // Using a DependencyProperty as the backing store for ZoomIn.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
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

        // Using a DependencyProperty as the backing store for IsZoomOutEnabled.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
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

        // Using a DependencyProperty as the backing store for ZoomOut.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
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

        // Using a DependencyProperty as the backing store for IsZoomToEnabled.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
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

        // Using a DependencyProperty as the backing store for ZoomTo.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
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

        // Using a DependencyProperty as the backing store for IsZoomResetEnabled.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
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

        // Using a DependencyProperty as the backing store for ZoomReset.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
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

        // Using a DependencyProperty as the backing store for ZoomFactor.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
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

        // Using a DependencyProperty as the backing store for MinimumZoom.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
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

        // Using a DependencyProperty as the backing store for MaximumZoom.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
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

        // Using a DependencyProperty as the backing store for ZoomMode.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
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

        // Using a DependencyProperty as the backing store for ContentBackground.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
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

        // Using a DependencyProperty as the backing store for ViewPortBrush.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
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

        // Using a DependencyProperty as the backing store for OverviewSourceAncestor.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty OverviewSourceAncestorProperty =
            DependencyProperty.Register("OverviewSourceAncestor", typeof(DependencyObject), typeof(Overview), new PropertyMetadata(null, OnOverviewSourceAncestorChanged));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dp"></param>
        /// <param name="evtArgs"></param>
        private static void OnOverviewSourceAncestorChanged(DependencyObject dp, DependencyPropertyChangedEventArgs evtArgs)
        {
            if (evtArgs.NewValue != null)
            {
                Overview ov = dp as Overview;
                //ov.ScrollOwner = FindScrollViewer(evtArgs.NewValue as DependencyObject);
                ov.UpdateSource();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="depObj"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 
        /// </summary>
        internal OverviewContentHolder ScrollSource
        {
            get { return (OverviewContentHolder)GetValue(ScrollSourceProperty); }
            set { SetValue(ScrollSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScrollOwner.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        internal static readonly DependencyProperty ScrollSourceProperty =
            DependencyProperty.Register("ScrollSource", typeof(OverviewContentHolder), typeof(Overview), new PropertyMetadata(null, OnScrollSourceChanged));
        /// <summary>
        /// 
        /// </summary>
        internal DependencyObject ScrollContentTarget
        {
            get { return (DependencyObject)GetValue(ScrollContentTargetProperty); }
            set { SetValue(ScrollContentTargetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScrollContentTarget.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        internal static readonly DependencyProperty ScrollContentTargetProperty =
            DependencyProperty.Register("ScrollContentTarget", typeof(DependencyObject), typeof(Overview), new PropertyMetadata(null, OnScrollContentTargetChanged));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Overview_Unloaded(object sender, RoutedEventArgs e)
        {
            (sender as FrameworkElement).Unloaded -= Overview_Unloaded;
            UpdateScrollContentTarget();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dp"></param>
        /// <param name="evtArgs"></param>
        private static void OnScrollSourceChanged(DependencyObject dp, DependencyPropertyChangedEventArgs evtArgs)
        {
            Overview ov = dp as Overview;
           
            ov.UpdateScrollContentTarget();

            if (evtArgs.NewValue != null)
            {
                OverviewContentHolder holder = evtArgs.NewValue as OverviewContentHolder;
                holder._overviewParent = ov;
                Binding bind = new Binding();
                bind.Source = ov;
                bind.Mode = BindingMode.TwoWay;
                bind.Path = new PropertyPath("Scale");
                holder.SetBinding(OverviewContentHolder.ScaleProperty, bind);
#if SILVERLIGHT
                holder.ScrollChanged += new ScrollChangedEventHandler(ov.holder_ScrollChanged);
#endif
#if WPF                
                holder.ScrollOwner.ScrollChanged += new ScrollChangedEventHandler(ov.holder_ScrollChanged);
#endif
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void holder_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            UpdateVp();
        }

        #region ViewPort
        /// <summary>
        /// 
        /// </summary>
        internal double VpOffsetX
        {
            get { return (double)GetValue(VpOffsetXProperty); }
            set
            {
                SetValue(VpOffsetXProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for VpOffsetX.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        internal static readonly DependencyProperty VpOffsetXProperty =
            DependencyProperty.Register("VpOffsetX", typeof(double), typeof(Overview), new PropertyMetadata(0d, OnVpOffsetChanged));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// 
        /// </summary>
        internal double VpOffsetY
        {
            get { return (double)GetValue(VpOffsetYProperty); }
            set
            {
                SetValue(VpOffsetYProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for VpOffsetY.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        internal static readonly DependencyProperty VpOffsetYProperty =
            DependencyProperty.Register("VpOffsetY", typeof(double), typeof(Overview), new PropertyMetadata(0d, OnVpOffsetChanged));
        /// <summary>
        /// 
        /// </summary>
        internal double VpWidth
        {
            get { return (double)GetValue(VpWidthProperty); }
            set
            {
                value = Math.Max(5, value);
                SetValue(VpWidthProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for VpWidth.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        internal static readonly DependencyProperty VpWidthProperty =
            DependencyProperty.Register("VpWidth", typeof(double), typeof(Overview), new PropertyMetadata(0d, OnVpOffsetChanged));
        /// <summary>
        /// 
        /// </summary>
        internal double VpHeight
        {
            get { return (double)GetValue(VpHeightProperty); }
            set
            {
                value = Math.Max(5, value);
                SetValue(VpHeightProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for VpHeight.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        internal static readonly DependencyProperty VpHeightProperty =
            DependencyProperty.Register("VpHeight", typeof(double), typeof(Overview), new PropertyMetadata(0d, OnVpOffsetChanged));
        /// <summary>
        /// 
        /// </summary>
        internal double WindowWidth
        {
            get { return (double)GetValue(WindowWidthProperty); }
            set { SetValue(WindowWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WindowWidth.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        internal static readonly DependencyProperty WindowWidthProperty =
            DependencyProperty.Register("WindowWidth", typeof(double), typeof(Overview), new PropertyMetadata(0d));
        /// <summary>
        /// 
        /// </summary>
        internal double WindowHeight
        {
            get { return (double)GetValue(WindowHeightProperty); }
            set { SetValue(WindowHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WidowHeight.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        internal static readonly DependencyProperty WindowHeightProperty =
            DependencyProperty.Register("WindowHeight", typeof(double), typeof(Overview), new PropertyMetadata(0d));
        /// <summary>
        /// 
        /// </summary>
        internal Transform Trans
        {
            get { return (Transform)GetValue(TransProperty); }
            set { SetValue(TransProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Trans.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        internal static readonly DependencyProperty TransProperty =
            DependencyProperty.Register("Trans", typeof(Transform), typeof(Overview), new PropertyMetadata(null));

        #endregion

        #region ImageUpdate

#if SILVERLIGHT
        internal Image Img { get; set; }
#endif
#if WPF
        /// <summary>
        /// 
        /// </summary>
        internal Panel Img { get; set; }
#endif
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Img_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateVp();
        }

        #endregion

        #region Update VP SV
        /// <summary>
        /// 
        /// </summary>
        private void UpdateVp()
        {
            if (!IsResizing && this.ScrollSource!=null)
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
        /// <summary>
        /// 
        /// </summary>
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

                Size uniformSize = GetUniformImageSize();
                VpOffsetX = (H1 * uniformSize.Width);
                VpOffsetY = (V1 * uniformSize.Height);

                VpWidth = H2 * uniformSize.Width;
                VpHeight = V2 * uniformSize.Height;


                Trans = new TranslateTransform() { X = VpOffsetX, Y = VpOffsetY };
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal void UpdateSourceScroll()
        {
            OverviewContentHolder panel = this.ScrollSource as OverviewContentHolder;
            if (panel != null)
            {
                double _ExtentWidth, _ExtentHeight, _ViewportWidth, _ViewportHeight, _HorizontalOffset, _VerticalOffset;
                Size uniformSize = GetUniformImageSize();
                _ExtentWidth = uniformSize.Width;
                _ExtentHeight = uniformSize.Height;
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
                    ScrollSource.SetHorizontalOffset(SVh);
                    ScrollSource.SetVerticalOffset(SVv);
                }
                else
                {
                    this.UnitScale *= delta;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal void UpdateScrollViewer()
        {
            this.LayoutUpdated -= new EventHandler(Overview_LayoutUpdated_UpdateSV);
            this.LayoutUpdated += new EventHandler(Overview_LayoutUpdated_UpdateSV);
            this.InvalidateArrange();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Overview_LayoutUpdated_UpdateVP(object sender, EventArgs e)
        {
            this.ScrollSource.LayoutUpdated -= new EventHandler(Overview_LayoutUpdated_UpdateVP);

            if (ScrollSource is OverviewContentHolder)
            {
                //Point Space = new Point();
                double H1, H2, H3, V1, V2, V3;
                H1 = ScrollSource.HorizontalOffset / ScrollSource.ExtentWidth;
                H2 = ScrollSource.ViewportWidth / ScrollSource.ExtentWidth;
                H3 = (ScrollSource.ExtentWidth - ScrollSource.HorizontalOffset - ScrollSource.ViewportWidth) / ScrollSource.ExtentWidth;

                V1 = ScrollSource.VerticalOffset / ScrollSource.ExtentHeight;
                V2 = ScrollSource.ViewportHeight / ScrollSource.ExtentHeight;
                V3 = (ScrollSource.ExtentHeight - ScrollSource.VerticalOffset - ScrollSource.ViewportHeight) / ScrollSource.ExtentHeight;

                if (Img == null)
                {
                    return;
                }

                Size uniformSize = GetUniformImageSize();

                VpOffsetX = H1 * uniformSize.Width;
                VpOffsetY = V1 * uniformSize.Height;

                VpWidth = H2 * uniformSize.Width;
                VpHeight = V2 * uniformSize.Height;


                Trans = new TranslateTransform() { X = VpOffsetX, Y = VpOffsetY };
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Overview_LayoutUpdated_UpdateSV(object sender, EventArgs e)
        {
            this.LayoutUpdated -= new EventHandler(Overview_LayoutUpdated_UpdateSV);           
            if (Img == null || Img == null)
            {
                return;
            }

            OverviewContentHolder panel = this.ScrollSource as OverviewContentHolder;
            if (panel != null)
            {
                double _ExtentWidth, _ExtentHeight, _ViewportWidth, _ViewportHeight, _HorizontalOffset, _VerticalOffset;
                Size uniformSize = GetUniformImageSize();
                _ExtentWidth = uniformSize.Width;
                _ExtentHeight = uniformSize.Height;
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
                    ScrollSource.SetHorizontalOffset(SVh);
                    ScrollSource.SetVerticalOffset(SVv);

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

                        //ScrollSource.HorizontalOffset = SVh;
                        //ScrollSource.VerticalOffset = SVv;
                        ScrollSource.SetHorizontalOffset(SVh);
                        ScrollSource.SetVerticalOffset(SVv);
                    };
                    ScrollSource.LayoutUpdated += ScrollViewer_LayoutUpdated;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private EventHandler ScrollViewer_LayoutUpdated = null;
        //private VisualBrush brush = null;

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
            Img = GetTemplateChild("PART_CustomPanel") as Panel;
            //brush = GetTemplateChild("PART_Preview") as VisualBrush;
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
        /// <summary>
        /// 
        /// </summary>
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
                dp = VisualTreeHelper.GetChild(dp, 0);
                ScrollContentTarget = ScrollSource.Content; //dp;

            }
        }
        /// <summary>
        /// 
        /// </summary>
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
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ScrollSource_Loaded(object sender, RoutedEventArgs e)
        {            
            (OverviewSourceAncestor as FrameworkElement).Loaded -= new RoutedEventHandler(ScrollSource_Loaded);
            UpdateSource();
        }

        private Point m_DragDelta = new Point(0, 0);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void drag_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            m_DragDelta.X = VpOffsetX;
            m_DragDelta.Y = VpOffsetY;
            this.IsResizing = true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void drag_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            m_DragDelta.X += e.HorizontalChange;
            m_DragDelta.Y += e.VerticalChange;
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void drag_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            this.IsResizing = false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static T FindScrollViewer<T>(DependencyObject obj) where T : UIElement
        {
            //T sv = FindAttachedScrollViewer<T>(obj);
            //if (sv == null)
            //{
                return FindChildScrollViewer<T>(obj);
            //}
            //return sv;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Size GetUniformImageSize()
        {
#if WPF
            //Size uniformSize = new Size(this.ActualWidth, this.ActualHeight);
            //return uniformSize.GetUniformSize(new Size(ScrollSource.ExtentWidth, ScrollSource.ExtentHeight));
            return new Size(Img.ActualWidth, Img.ActualHeight);
#endif
#if SILVERLIGHT
            return new Size(Img.ActualWidth, Img.ActualHeight);
#endif
        }
    }
    /// <summary>
    /// 
    /// </summary>
    internal static class OverviewExtension
    {
        internal static double GetScaleRatio(this double source, double target)
        {
            return target / source;
        }

        private static Size GetScaleRatioSize(this Size source, Size target)
        {
            return new Size(target.Width / source.Width, target.Height / source.Height);
        }

        internal static double GetScaleRatioDouble(this Size source, Size target)
        {
            Size size = source.GetScaleRatioSize(target);
            return size.Width < size.Height ? size.Height : size.Width;
        }

        internal static Size GetUniformSize(this Size source, Size target)
        {
            double scale = source.GetScaleRatioDouble(target);
            //return new Size(target.Width / scale, target.Height / scale);
            Size returnSize = new Size(target.Width / scale, target.Height / scale);
            return new Size(double.IsNaN(returnSize.Width) ? 0 : returnSize.Width, double.IsNaN(returnSize.Height) ? 0 : returnSize.Height);
        }

        internal static bool IsMouseWheelUp(this MouseWheelEventArgs args, OverviewContentHolder och)
        {
            if (
                // If ZoomGesture is Mouse Wheel or
                (ZoomGesture.None != (ZoomGesture.MouseWheelUp & och.ZoomInGesture) &&
                args.Delta > 0)
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        internal static bool CanZoom_Keys_Or(this OverviewContentHolder och, ZoomGesture gesture)
        {
            ZoomGesture KeyCombination = gesture & (ZoomGesture.Ctrl | ZoomGesture.Shift | ZoomGesture.Alt);

            ModifierKeys KeyboardModifiers = Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt);

            // Dual Keys
            ZoomGesture CtrlShiftUp = ZoomGesture.Ctrl | ZoomGesture.Shift;
            ZoomGesture ShiftAltUp = ZoomGesture.Shift | ZoomGesture.Alt;
            ZoomGesture AltCtrlUp = ZoomGesture.Alt | ZoomGesture.Ctrl;

            // Tripple keys
            ZoomGesture CtrlShiftAltUp = ZoomGesture.Ctrl | ZoomGesture.Shift | ZoomGesture.Alt;

            if (
                // Nothing
                ((CtrlShiftAltUp & KeyCombination) == ZoomGesture.None) ||

                // Single 
                ((ZoomGesture.Ctrl == KeyCombination) &&
                (ModifierKeys.Control & KeyboardModifiers) != ModifierKeys.None) ||
                ((ZoomGesture.Shift == KeyCombination) &&
                (ModifierKeys.Shift & KeyboardModifiers) != ModifierKeys.None) ||
                ((ZoomGesture.Alt == KeyCombination) &&
                (ModifierKeys.Alt & KeyboardModifiers) != ModifierKeys.None) ||

                // Double
                ((CtrlShiftUp == KeyCombination) &&
                ((ModifierKeys.Control | ModifierKeys.Shift) & KeyboardModifiers)!= ModifierKeys.None) ||
                ((ShiftAltUp == KeyCombination) &&
                ((ModifierKeys.Shift | ModifierKeys.Alt) & KeyboardModifiers) != ModifierKeys.None) ||
                ((AltCtrlUp == KeyCombination) &&
                ((ModifierKeys.Alt | ModifierKeys.Control) & KeyboardModifiers) != ModifierKeys.None) ||

                // Tripple
                ((CtrlShiftAltUp == KeyCombination) &&
                ((ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt) & KeyboardModifiers) != ModifierKeys.None)
            )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal static bool CanZoom_Keys_And(this OverviewContentHolder och, ZoomGesture gesture)
        {
            ZoomGesture KeyCombination = gesture & (ZoomGesture.Ctrl | ZoomGesture.Shift | ZoomGesture.Alt);

            ModifierKeys CtrlModfier = Keyboard.Modifiers & ModifierKeys.Control;
            ModifierKeys ShiftModifier = Keyboard.Modifiers & ModifierKeys.Shift;
            ModifierKeys AltModifier = Keyboard.Modifiers & ModifierKeys.Alt;

            ModifierKeys CtrlShiftModifier = Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Shift);
            ModifierKeys ShiftAltModifier = Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Alt);
            ModifierKeys AltCtrlModifier = Keyboard.Modifiers & (ModifierKeys.Alt | ModifierKeys.Control);

            ModifierKeys CtrlShiftAltModifier = Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt);

            // Dual Keys
            ZoomGesture CtrlShiftUp = ZoomGesture.Ctrl | ZoomGesture.Shift;
            ZoomGesture ShiftAltUp = ZoomGesture.Shift | ZoomGesture.Alt;
            ZoomGesture AltCtrlUp = ZoomGesture.Alt | ZoomGesture.Ctrl;

            // Tripple keys
            ZoomGesture CtrlShiftAltUp = ZoomGesture.Ctrl | ZoomGesture.Shift | ZoomGesture.Alt;

            if (
                // Nothing
                ((CtrlShiftAltUp & KeyCombination) == ZoomGesture.None) ||

                // Single 
                ((ZoomGesture.Ctrl == KeyCombination) &&
                ModifierKeys.Control == CtrlModfier) ||
                ((ZoomGesture.Shift == KeyCombination) &&
                ModifierKeys.Shift == ShiftModifier) ||
                ((ZoomGesture.Alt == KeyCombination) &&
                ModifierKeys.Alt == AltModifier) ||

                // Double
                ((CtrlShiftUp == KeyCombination) &&
                (ModifierKeys.Control | ModifierKeys.Shift) == CtrlShiftModifier) ||
                ((ShiftAltUp == KeyCombination) &&
                (ModifierKeys.Shift | ModifierKeys.Alt) == ShiftAltModifier) ||
                ((AltCtrlUp == KeyCombination) &&
                (ModifierKeys.Alt | ModifierKeys.Control) == AltCtrlModifier) ||

                // Tripple
                ((CtrlShiftAltUp == KeyCombination) &&
                (ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt) == CtrlShiftAltModifier)
            )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal static bool CanZoom(this MouseWheelEventArgs args, OverviewContentHolder och, ZoomGesture gesture)
        {
            if (
                ((gesture & ZoomGesture.MouseWheelUp) != ZoomGesture.None && args.Delta > 0) ||
                ((gesture & ZoomGesture.MouseWheelDown) != ZoomGesture.None && args.Delta < 0)
            )
            {
                if (((gesture & ZoomGesture.And) != ZoomGesture.None))
                {
                    return och.CanZoom_Keys_And(gesture);
                }
                else
                {
                    return och.CanZoom_Keys_Or(gesture);
                }
            }
            else
            {
                return false;
            }
        }

        internal static bool CanZoom(this MouseButtonEventArgs args, OverviewContentHolder och, ZoomGesture gesture)
        {
            if (//(gesture & (ZoomGesture.LeftDoubleClick | ZoomGesture.RightDoubleClick | ZoomGesture.LeftClick | ZoomGesture.RightClick))!= ZoomGesture.None &&
                (((gesture & ZoomGesture.LeftClick) != ZoomGesture.None) && (och.m_MouseState == OverviewMouseState.LeftClick || och.m_MouseState == OverviewMouseState.LeftDoubleClick)) ||
                (((gesture & ZoomGesture.RightClick) != ZoomGesture.None) && (och.m_MouseState == OverviewMouseState.RightClick || och.m_MouseState == OverviewMouseState.RightDoubleClick)) ||
                (((gesture & ZoomGesture.LeftDoubleClick) != ZoomGesture.None) && (och.m_MouseState == OverviewMouseState.LeftDoubleClick)) ||
                (((gesture & ZoomGesture.RightDoubleClick) != ZoomGesture.None) && (och.m_MouseState == OverviewMouseState.RightDoubleClick))
            )
            {
                if (((gesture & ZoomGesture.And) != ZoomGesture.None))
                {
                    return och.CanZoom_Keys_And(gesture);
                }
                else
                {
                    return och.CanZoom_Keys_Or(gesture);
                }
            }
            else
            {
                return false;
            }
        }

        internal static bool CanZoomIn(this MouseWheelEventArgs args, OverviewContentHolder och)
        {
            if (och.IsZoomInEnabled)
            {
                return args.CanZoom(och, och.ZoomInGesture);
            }
            else
            {
                return false;
            }
        }

        internal static bool CanZoomOut(this MouseWheelEventArgs args, OverviewContentHolder och)
        {
            if (och.IsZoomOutEnabled)
            {
                return args.CanZoom(och, och.ZoomOutGesture);
            }
            else
            {
                return false;
            }
        }

        internal static bool CanZoomIn(this MouseButtonEventArgs args, OverviewContentHolder och)
        {
            if (och.IsZoomInEnabled)
            {
                return args.CanZoom(och, och.ZoomInGesture);
            }
            else
            {
                return false;
            }
        }

        internal static bool CanZoomOut(this MouseButtonEventArgs args, OverviewContentHolder och)
        {
            if (och.IsZoomOutEnabled)
            {
                return args.CanZoom(och, och.ZoomOutGesture);
            }
            else
            {
                return false;
            }
        }

        internal static bool CanZoomIn(this MouseEventArgs args, OverviewContentHolder och)
        {
            if (args is MouseWheelEventArgs)
            {
                return (args as MouseWheelEventArgs).CanZoomIn(och);
            }
            else if (args is MouseButtonEventArgs)
            {
                return (args as MouseButtonEventArgs).CanZoomIn(och);
            }
            else
            {
                return false;
            }
        }

        internal static bool CanZoomOut(this MouseEventArgs args, OverviewContentHolder och)
        {
            if (args is MouseWheelEventArgs)
            {
                return (args as MouseWheelEventArgs).CanZoomOut(och);
            }
            else if (args is MouseButtonEventArgs)
            {
                return (args as MouseButtonEventArgs).CanZoomOut(och);
            }
            else
            {
                return false;
            }
        }
    }
}
