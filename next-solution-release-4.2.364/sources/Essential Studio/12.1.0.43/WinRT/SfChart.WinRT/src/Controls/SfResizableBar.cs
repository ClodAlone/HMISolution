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
using System.Collections.Generic;
#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
#else
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Globalization;
using System.ComponentModel;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using System.Reflection;
#endif
namespace Syncfusion.UI.Xaml.Charts
{
    public class SfChartResizableBar : ResizableScrollBar
    {

        #region Constructor
        public SfChartResizableBar()
        {
        #if NETFX_CORE
                this.ManipulationMode = Windows.UI.Xaml.Input.ManipulationModes.None;
        #endif
        }

       #endregion

        #region Fields
        
        double previousZoomFactor;

        private DispatcherTimer _timer;

        const double interval = 0.5, transformSize=20;

        const double rectCoordinate = 15;

        ChartAxisBase2D axis;

        bool onButtonPressed=false,isValueChanged=false;

        DependencyObject parent;

        ContentControl nearHandContentControl, farHandContentControl;

        #endregion

        #region Properties
        internal ChartAxisBase2D Axis
        {
            get
            {
                return axis;
            }
            set
            {
                axis = value;
                AttachTouchModeEvents();
                BindProperties();
            }
        }

        /// <summary>
        /// Gets or Sets zoom position. Value must fall within 0 to 1. It determines starting value of visible range
        /// </summary>
        internal double ZoomPosition
        {
            get { return (double)GetValue(ZoomPositionProperty); }
            set { SetValue(ZoomPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZoomPosition.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ZoomPositionProperty =
            DependencyProperty.Register("ZoomPosition", typeof(double), typeof(SfChartResizableBar), new PropertyMetadata(0d, OnZoomPositionChanged));

        /// <summary>
        /// Gets or Sets Template For Visible Range Label View.
        /// </summary>
        internal DataTemplate ThumbLabelTemplate
        {
            get { return (DataTemplate)GetValue(ThumbLabelTemplateProperty); }
            set { SetValue(ThumbLabelTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Template. 
        internal static readonly DependencyProperty ThumbLabelTemplateProperty =
            DependencyProperty.Register("ThumbLabelTemplate", typeof(DataTemplate), typeof(SfChartResizableBar), new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets Bool Value To Enable Or Disable The Visible Range Label View.
        /// </summary>
        internal Visibility ThumbLabelVisibility
        {
            get { return (Visibility)GetValue(ThumbLabelVisibilityProperty); }
            set { SetValue(ThumbLabelVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableThumbLabel. 
        internal static readonly DependencyProperty ThumbLabelVisibilityProperty =
            DependencyProperty.Register("EnableThumbLabel", typeof(Visibility), typeof(SfChartResizableBar), new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Gets or Sets zoom factor. Value must fall within 0 to 1. It determines delta of visible range.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        internal double ZoomFactor
        {
            get { return (double)GetValue(ZoomFactorProperty); }
            set { SetValue(ZoomFactorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZoomFactor.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ZoomFactorProperty =
            DependencyProperty.Register("ZoomFactor", typeof(double), typeof(SfChartResizableBar), new PropertyMetadata(1d, OnZoomFactorChanged));

        #endregion
        
        #region Methods

        #if NETFX_CORE
        protected override void OnApplyTemplate()
        #else
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
#if !WINDOWS_PHONE8 && !WINDOWS_PHONE7
            if (Axis != null)
                UpdateResizable(Axis.EnableScrollBarResizing);
#endif
            if (this.EnableTouchMode)
            {
                if (this.Orientation == Orientation.Horizontal)
                {
                    nearHandContentControl = this.GetTemplateChild("HorizontalNearHandContent") as ContentControl;
                    farHandContentControl = this.GetTemplateChild("HorizontalFarHandContent") as ContentControl;
                }
                else
                {
                    nearHandContentControl = this.GetTemplateChild("VerticalNearHandContent") as ContentControl;
                    farHandContentControl = this.GetTemplateChild("VerticalFarHandContent") as ContentControl;
                }
            }
            if (MiddleThumb != null && NearHand != null && FarHand != null)
            {
                MiddleThumb.DragCompleted += DragCompleted;
                NearHand.DragCompleted += DragCompleted;
                FarHand.DragCompleted += DragCompleted;
            }
        }

        /// <summary>
        /// Middles the thumb drag completed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DragCompletedEventArgs"/> instance containing the event data.</param>
        void DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (axis != null)
                axis.IsScrolling = false;
        }

        internal void UpdateResizable(bool isVisible)
        {
            if (NearHand != null && FarHand != null)
            {
                if (isVisible)
                {
                    NearHand.Visibility = Visibility.Visible;
                    FarHand.Visibility = Visibility.Visible;
                }
                else
                {
                    NearHand.Visibility = Visibility.Collapsed;
                    FarHand.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void AttachTouchModeEvents()
        {
#if NETFX_CORE
            Axis.PointerEntered += OnAxisPointerEntered;
            Axis.PointerExited += OnAxisPointerExited;
            Axis.Loaded+=OnAxisLoaded;
            Axis.Unloaded+=OnAxisUnloaded;
#else
            Axis.MouseEnter += OnAxisMouseEnter;
            Axis.MouseLeave += OnAxisMouseLeave;
            Axis.Loaded += OnAxisLoaded;
            Axis.Unloaded += OnAxisUnloaded;

#endif
       }
        
        void OnAxisUnloaded(object sender, RoutedEventArgs e)
        {
#if WPF
            if (parent is Window)
                (parent as Window).RemoveHandler(Window.MouseDownEvent, new MouseButtonEventHandler(OnMouseDown));
#endif
#if SILVERLIGHT
            if (parent is UserControl)
                (parent as UserControl).RemoveHandler(UserControl.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnMouseDown));
#endif
#if NETFX_CORE
            if(parent is Page)
                (parent as Page).RemoveHandler(Page.PointerPressedEvent, new PointerEventHandler(OnPointerPressed));
#endif
        }

        
        void OnAxisLoaded(object sender, RoutedEventArgs e)
        {
            if (Axis.Area != null)
            {
                parent = VisualTreeHelper.GetParent(Axis.Area);
                if (parent != null)
                {
                    while (VisualTreeHelper.GetParent(parent) != null)
                    {
                        parent = VisualTreeHelper.GetParent(parent);
#if NETFX_CORE
                        if (parent is Page)
                            break;
#endif
                    }
#if WPF
            if (parent is Window)
                (parent as Window).AddHandler(Window.MouseDownEvent, new MouseButtonEventHandler(OnMouseDown),true);
#endif
#if SILVERLIGHT
            if (parent is UserControl)
               (parent as UserControl).AddHandler(UserControl.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnMouseDown) , true);
#endif
#if NETFX_CORE
                    if (parent is Page)
                        (parent as Page).AddHandler(Page.PointerPressedEvent, new PointerEventHandler(OnPointerPressed), true);
#endif
                }
            }
        }
 
#if NETFX_CORE
        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Point currentPosition = (Point)e.GetCurrentPoint(Axis.Area).Position;
            if (CheckRegion(currentPosition))
            {
                VisualStateManager.GoToState(this, "OnView", true);
                onButtonPressed = true;
            }
            else
            {
                VisualStateManager.GoToState(this, "OnLostFocus", true);
                onButtonPressed = false;
            }
        }
        private void OnAxisPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (!Axis.isManipulated && !onButtonPressed)
                VisualStateManager.GoToState(this, "OnFocus", true);
        }
        private void OnAxisPointerExited(object sender, PointerRoutedEventArgs e)
        {
            if(!onButtonPressed)
                VisualStateManager.GoToState(this, "OnExit", true);
        }
#else
        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Point currentPosition = (Point)e.GetPosition(Axis.Area);
            if (CheckRegion(currentPosition))
            {
                VisualStateManager.GoToState(this, "OnView", true);
                onButtonPressed = true;
            }
            else
            {
                VisualStateManager.GoToState(this, "OnLostFocus", true);
                onButtonPressed = false;
            }
        }
        private void OnAxisMouseLeave(object sender, MouseEventArgs e)
        {
            if (!onButtonPressed)
                VisualStateManager.GoToState(this, "OnExit", true);
        }

        private void OnAxisMouseEnter(object sender, MouseEventArgs e)
        {
            if (!Axis.isManipulated && !onButtonPressed)
                VisualStateManager.GoToState(this, "OnFocus", true);
        }
       
#endif
        private bool CheckRegion(Point position)
        {
#if !WINDOWS_PHONE8 && !WINDOWS_PHONE7
            if (Axis.EnableScrollBar && Axis.Visibility!= Visibility.Collapsed)
            {
                Rect rect = new Rect();
                rect = Axis.ArrangeRect;
                if (this.Orientation == Orientation.Horizontal)
                {
                    rect.X = rect.X - rectCoordinate;
                    rect.Y = (Axis.OpposedPosition) ? rect.Y + rectCoordinate : rect.Y - rectCoordinate;
                }
                else
                {
                    rect.X = (Axis.OpposedPosition) ? rect.X - rectCoordinate : rect.X + rectCoordinate;
                    rect.Y = rect.Y - rectCoordinate;
                }
                if (rect.Contains(position) || FarHand.IsDragging || NearHand.IsDragging || MiddleThumb.IsDragging)
                    return true;
            }
#endif
            return false;
        }
        private static void OnZoomFactorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SfChartResizableBar).ChangeZoomFactor((double)e.NewValue);
        }

        private static void OnZoomPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
           (d as SfChartResizableBar).ChangeZoomPosition((double)e.NewValue);
        }
        protected override void OnValueChanged()
        {
            base.OnValueChanged();
#if !WINDOWS_PHONE8 && !WINDOWS_PHONE7
            if (!Axis.DeferredScrolling)
            {
                ZoomPosition = RangeStart;
                ZoomFactor = (RangeEnd - RangeStart) >= SmallChange ? (RangeEnd - RangeStart) : ZoomFactor;
            }
            ResetTimer();
            isValueChanged = true;
            previousZoomFactor = 0;
#endif
        }
        private void ChangeZoomPosition(double value)
        {
            IsValueChangedTrigger = false;
            RangeStart = value;
            if(previousZoomFactor==ZoomFactor)
               RangeEnd = RangeStart + ZoomFactor;
            previousZoomFactor = ZoomFactor;
        }

        private void ChangeZoomFactor(double value)
        {
            IsValueChangedTrigger = false;
            RangeEnd = RangeStart + value;
            RangeStart = ZoomPosition;
            previousZoomFactor = ZoomFactor;
        }
        private void BindProperties()
        {
            Binding zoomPositionBind = new Binding();
            zoomPositionBind.Source = Axis;
            zoomPositionBind.Path = new PropertyPath("ZoomPosition");
            zoomPositionBind.Mode = BindingMode.TwoWay;
            this.SetBinding(SfChartResizableBar.ZoomPositionProperty, zoomPositionBind);
            Binding zoomFactorBind = new Binding();
            zoomFactorBind.Source = Axis;
            zoomFactorBind.Path = new PropertyPath("ZoomFactor");
            zoomFactorBind.Mode = BindingMode.TwoWay;
            this.SetBinding(SfChartResizableBar.ZoomFactorProperty, zoomFactorBind);
            Binding enableTouchModeBind = new Binding();
            enableTouchModeBind.Source = Axis;
            enableTouchModeBind.Path = new PropertyPath("EnableTouchMode");
            enableTouchModeBind.Mode = BindingMode.TwoWay;
            this.SetBinding(SfChartResizableBar.EnableTouchModeProperty, enableTouchModeBind);
            Binding thumbLabelBind = new Binding();
            thumbLabelBind.Source = Axis;
            thumbLabelBind.Path = new PropertyPath("ThumbLabelTemplate");
            this.SetBinding(SfChartResizableBar.ThumbLabelTemplateProperty, thumbLabelBind);
            Binding enableThumbLabelBind = new Binding();
            enableThumbLabelBind.Source = Axis;
            enableThumbLabelBind.Path = new PropertyPath("ThumbLabelVisibility");
            this.SetBinding(SfChartResizableBar.ThumbLabelVisibilityProperty, enableThumbLabelBind);
        }

        protected override void OnFarHandDragged(object sender, DragDeltaEventArgs e)
        {
            axis.IsScrolling = true;
            base.OnFarHandDragged(sender, e);
            if(farHandContentControl!=null)
                farHandContentControl.Visibility =(isValueChanged) ?  Visibility.Visible: Visibility.Collapsed;
            onButtonPressed = true;
            if(this.EnableTouchMode)
                Translate(farHandContentControl, RangeEnd);
            isValueChanged = false;
        }

       
        private void Translate(ContentControl contentControl, double rangeValue)
        {
            if (ThumbLabelVisibility == Visibility.Visible)
            {
                contentControl.ContentTemplate = ThumbLabelTemplate;
                contentControl.Content = Axis is NumericalAxis ? Convert.ToDecimal(Axis.GetLabelContent(Axis.CoefficientToActualValue(rangeValue))).ToString("0.##") : Convert.ToString(Axis.GetLabelContent(Axis.CoefficientToActualValue(rangeValue)));
                TranslateTransform translate = new TranslateTransform();
                if (Orientation == Orientation.Horizontal)
                {
                    translate.X = -contentControl.ActualWidth / 2;
                    translate.Y = Axis.OpposedPosition ? transformSize : -transformSize;
                }
                else
                {
                    translate.X = Axis.OpposedPosition ? -(2 * transformSize) : transformSize;
                    translate.Y = -contentControl.ActualHeight / 2;
                }
                contentControl.RenderTransform = translate;
            }
            else
                contentControl.Visibility = Visibility.Collapsed;
        }

        protected override void OnNearHandDragged(object sender, DragDeltaEventArgs e)
        {
            axis.IsScrolling = true;
            base.OnNearHandDragged(sender, e);
            if(nearHandContentControl!=null)
               nearHandContentControl.Visibility =(isValueChanged) ? Visibility.Visible : Visibility.Collapsed;
            onButtonPressed = true;
            if (this.EnableTouchMode)
                Translate(nearHandContentControl, RangeStart);
            isValueChanged = false;
        }
        
        protected override void OnThumbDragged(object sender, DragDeltaEventArgs e)
        {
            axis.IsScrolling = true;
            base.OnThumbDragged(sender, e);
            onButtonPressed = true;
        }

        private void ResetTimer()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Interval = TimeSpan.FromSeconds(interval);
                _timer.Start();
            }
            else
            {
                _timer = new DispatcherTimer();
                _timer.Tick += OnTimeout;
            }
        }

        private void OnTimeout(object sender, object e)
        {
#if !WINDOWS_PHONE8 && !WINDOWS_PHONE7
            if (Axis.DeferredScrolling == true)
            {
                ZoomPosition = RangeStart;
                ZoomFactor = (RangeEnd - RangeStart) >= SmallChange ? (RangeEnd - RangeStart) : (ZoomFactor == Maximum) ? SmallChange : ZoomFactor;
            }
#endif
            if (nearHandContentControl != null && farHandContentControl != null)
            {
                nearHandContentControl.Visibility = Visibility.Collapsed;
                farHandContentControl.Visibility = Visibility.Collapsed;
            }
            if (_timer != null)
                _timer.Stop();
            _timer.Tick -= OnTimeout;
            _timer = null;
        }
       
    }
        #endregion
}
