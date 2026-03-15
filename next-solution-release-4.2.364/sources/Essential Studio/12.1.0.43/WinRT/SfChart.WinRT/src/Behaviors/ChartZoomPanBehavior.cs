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
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;
using System.Windows.Input;
#else
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.Foundation;
using Windows.Devices.Input;
using Windows.System;
using Windows.UI.Input;
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// ChartZoomPanBehavior enables zooming and panning operations over a Chart.
    /// </summary>
    /// <remarks>
    /// Zooming and panning operations can be initiated and can be restored backed to
    /// the original position by performing zoom out operation or by enabling <see
    /// cref="ChartZoomPanBehavior.ResetOnDoubleTap"/> property. 
    /// <para>Chart can also be zoomed, without adding ChartZoomPanBehavior, by
    /// specifying following properties <see cref="ChartAxis.ZoomFactor"/> and <see
    /// cref="ChartAxis.ZoomPosition"/> for the ChartAxis. By specifying zooming mode
    /// using <see cref="ChartZoomPanBehavior.ZoomMode"/> property, zooming operation
    /// can be performed along horizontal or along vertical or along both directions in
    /// a Chart.</para>
    /// </remarks>
    public class ChartZoomPanBehavior: ChartBehavior
    {
        #region fields

        private Rectangle selectionRectangle;

        private Point startPoint;

        private bool isZooming;

        Rect zoomRect;

        private double previousScale;
#if WPF || SILVERLIGHT_UNCOMMON
         private Point previousPoint;
#endif
#if WINDOWS_PHONE
        private bool isLeftButtonPressed;
        private bool mouseCapture;
#endif
        #endregion

        #region ctor

        /// <summary>
        /// Called when instance created for ChartZoomPanBehavior
        /// </summary>
        public ChartZoomPanBehavior()
        {
            selectionRectangle = new Rectangle { IsHitTestVisible = false };
            Binding binding = new Binding();
            binding.Path = new PropertyPath("StrokeThickness");
            binding.Source = this;
            selectionRectangle.SetBinding(Rectangle.StrokeThicknessProperty,binding);

            binding = new Binding();
            binding.Path = new PropertyPath("Fill");
            binding.Source = this;
            selectionRectangle.SetBinding(Rectangle.FillProperty, binding);

            binding = new Binding();
            binding.Path = new PropertyPath("Stroke");
            binding.Source = this;
            selectionRectangle.SetBinding(Rectangle.StrokeProperty, binding);
        }

        #endregion

        #region properties

        private bool enableSelectionZooming = true;

        internal bool InternalEnableSelectionZooming
        {
            get { return enableSelectionZooming && EnableSelectionZooming; }
            set { enableSelectionZooming = value; }
        }
        private bool enablePanning = true;

        internal bool InternalEnablePanning
        {
            get { return enablePanning && EnablePanning; }
            set { enablePanning = value; }
        }

#if !WINDOWS_PHONE8 && !WINDOWS_PHONE7

        /// <summary>
        /// Gets or sets a value indicating whether [zoom relative to mouse pointer] and this is applicable only for mouse wheel zooming.
        /// </summary>
        /// <value>
        /// <c>true</c> if [zoom relative to mouse pointer]; otherwise, <c>false</c>.
        /// </value>
        public bool ZoomRelativeToCursor
        {
            get { return (bool)GetValue(ZoomRelativeToCursorProperty); }
            set { SetValue(ZoomRelativeToCursorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZoomRelativeToCursor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZoomRelativeToCursorProperty =
            DependencyProperty.Register("ZoomRelativeToCursor", typeof(bool), typeof(ChartZoomPanBehavior), new PropertyMetadata(true));

#endif

#if !SILVERLIGHT_UNCOMMON

        /// <summary>
        /// Gets or sets a value indicating whether [enable pinch zooming].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable pinch zooming]; otherwise, <c>false</c>.
        /// </value>
        public bool EnablePinchZooming
        {
            get { return (bool)GetValue(EnablePinchZoomingProperty); }
            set { SetValue(EnablePinchZoomingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnablePinchZooming.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnablePinchZoomingProperty =
            DependencyProperty.Register("EnablePinchZooming", typeof(bool), typeof(ChartZoomPanBehavior), new PropertyMetadata(true));

#endif

        /// <summary>
        /// Gets or Sets the zoom mode.
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
            DependencyProperty.Register("ZoomMode", typeof(ZoomMode), typeof(ChartZoomPanBehavior), new PropertyMetadata(ZoomMode.XY));

        /// <summary>
        /// Enables/Disables panning. 
        /// </summary>
        public bool EnablePanning
        {
            get { return (bool)GetValue(EnablePanningProperty); }
            set { SetValue(EnablePanningProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for EnablePanning.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EnablePanningProperty =
            DependencyProperty.Register("EnablePanning", typeof(bool), typeof(ChartZoomPanBehavior), new PropertyMetadata(true));

        /// <summary>
        /// Gets or Sets stroke thickness for selection rectangle.
        /// </summary>
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty = 
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(ChartZoomPanBehavior), new PropertyMetadata(1d));

        /// <summary>
        /// Gets or Sets Maximum Zoom Level.
        /// </summary>
        public double MaximumZoomLevel
        {
            get { return (double)GetValue(MaximumZoomLevelProperty); }
            set { SetValue(MaximumZoomLevelProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for MaximumZoomLevel.
        /// </summary>
        public static readonly DependencyProperty MaximumZoomLevelProperty =
            DependencyProperty.Register("MaximumZoomLevel", typeof(double), typeof(ChartZoomPanBehavior), new PropertyMetadata(double.NaN));

        private bool IsMaxZoomLevel { get { return !double.IsNaN(MaximumZoomLevel); } }

        /// <summary>
        /// Gets or Sets stroke for selection rectangle.
        /// </summary>
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for Stroke.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(ChartZoomPanBehavior), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        /// <summary>
        /// Gets or Sets the background for selection rectangle.
        /// </summary>
        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

      
        /// <summary>
        /// Using a DependencyProperty as the backing store for Fill.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(ChartZoomPanBehavior), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(100, 150, 200, 200))));

        /// <summary>
        ///Gets or Sets a value indicating whether to enable zooming chart using selection rectangle.
        /// </summary>
        public bool EnableSelectionZooming
        {
            get { return (bool)GetValue(EnableSelectionZoomingProperty); }
            set { SetValue(EnableSelectionZoomingProperty, value); }
        }

       
        /// <summary>
        ///  Using a DependencyProperty as the backing store for EnableSelectionZooming.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EnableSelectionZoomingProperty =
            DependencyProperty.Register("EnableSelectionZooming", typeof(bool), typeof(ChartZoomPanBehavior), new PropertyMetadata(false));

        /// <summary>
        /// Gets or Sets whether to reset zooming.
        /// </summary>
        public bool ResetOnDoubleTap
        {
            get { return (bool)GetValue(ResetOnDoubleTapProperty); }
            set { SetValue(ResetOnDoubleTapProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for ResetOnDoubleTap.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ResetOnDoubleTapProperty =
            DependencyProperty.Register("ResetOnDoubleTap", typeof(bool), typeof(ChartZoomPanBehavior), new PropertyMetadata(true));

#if !WINDOWS_PHONE || WPF || SILVERLIGHT_UNCOMMON

        /// <summary>
        /// Gets or Sets whether mouse wheel zooming is enabled.
        /// </summary>
        public bool EnableMouseWheelZooming
        {
            get { return (bool)GetValue(EnableMouseWheelZoomingProperty); }
            set { SetValue(EnableMouseWheelZoomingProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for EnableMouseWheelZooming.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EnableMouseWheelZoomingProperty =
            DependencyProperty.Register("EnableMouseWheelZooming", typeof(bool), typeof(ChartZoomPanBehavior), new PropertyMetadata(true));
#endif
#if !WINDOWS_PHONE
        /// <summary>
        /// Gets or Sets the key modeifier used for zooming.
        /// </summary>
        public VirtualKeyModifiers KeyModifiers
        {
            get { return (VirtualKeyModifiers)GetValue(KeyModifiersProperty); }
            set { SetValue(KeyModifiersProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty KeyModifiersProperty =
            DependencyProperty.Register("KeyModifiers", typeof(VirtualKeyModifiers), typeof(ChartZoomPanBehavior), new PropertyMetadata(VirtualKeyModifiers.None));

#endif

        #endregion

        #region methods

        /// <summary>
        /// Method implementation for AttachElements
        /// </summary>
        protected override void AttachElements()
        {
            if (this.AdorningCanvas != null &&
                !this.AdorningCanvas.Children.Contains(selectionRectangle))
            {
                this.AdorningCanvas.Children.Add(selectionRectangle);
            }
        }

#if !WPF

#if WINDOWS_PHONE

        /// <summary>
        /// Called when Double tapped in Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnDoubleTap(GestureEventArgs e)
#else
        /// <summary>
        /// Method implementation for OnDoubleTapped
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnDoubleTapped(DoubleTappedRoutedEventArgs e)
#endif
        {
            if (ResetOnDoubleTap && ChartArea.SeriesClipRect.Contains(e.GetPosition(AdorningCanvas)))
            {
                Reset();
            }
        }

#endif

#if SILVERLIGHT_UNCOMMON || WPF
        protected internal override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (ChartArea.AreaType != ChartAreaType.CartesianAxes)
                return;
            if (EnableMouseWheelZooming)
                MouseWheelZoom(e.GetPosition(AdorningCanvas), e.Delta > 0 ? 1 : -1);
        }
#endif

#if !WINDOWS_PHONE
        /// <summary>
        /// Occurs PointerWheel changed in Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnPointerWheelChanged(PointerRoutedEventArgs e)
        {
            if (ChartArea.AreaType != ChartAreaType.CartesianAxes)
                return;

            if (EnableMouseWheelZooming
                && e.Pointer.PointerDeviceType == PointerDeviceType.Mouse && this.KeyModifiers == e.KeyModifiers)
            {
                var point = e.GetCurrentPoint(AdorningCanvas);
                double direction = point.Properties.MouseWheelDelta > 0 ? 1 : -1;
                MouseWheelZoom(point.Position, direction);
            }
        }
#endif

#if !WINDOWS_PHONE8 && !WINDOWS_PHONE7

        private void MouseWheelZoom(Point mousePoint, double direction)
        {
            double origin;
            bool canUpdate = false;
            var seriesClipRect = ChartArea.SeriesClipRect;
            mousePoint = new Point(mousePoint.X - seriesClipRect.Left, mousePoint.Y - seriesClipRect.Top);

            bool canZoom = false;
            foreach (ChartAxisBase2D axis in this.ChartArea.Axes)
            {
                if ((axis.RegisteredSeries[0] as CartesianSeries) != null && (axis.RegisteredSeries[0] as CartesianSeries).IsActualTransposed)
                {
                    if ((axis.Orientation == Orientation.Horizontal && (ZoomMode == ZoomMode.Y || ZoomMode == ZoomMode.XY)) ||
                        (axis.Orientation == Orientation.Vertical && (ZoomMode == ZoomMode.X || ZoomMode == ZoomMode.XY)))
                    {
                        canZoom = true;
                    }
                }
                else
                {
                    if ((axis.Orientation == Orientation.Vertical && (ZoomMode == ZoomMode.Y || ZoomMode == ZoomMode.XY)) ||
                        (axis.Orientation == Orientation.Horizontal && (ZoomMode == ZoomMode.X || ZoomMode == ZoomMode.XY)))
                    {
                        canZoom = true;
                    }
                }
                if (canZoom)
                {
                    origin = 0.5;
                    //double currentScale = ChartMath.MinMax(1 / ChartMath.MinMax(axis.ZoomFactor, 0, 1), 1, 4);

                    double currentScale = Math.Max(1 / ChartMath.MinMax(axis.ZoomFactor, 0, 1), 1);

                    //double cumulativeScale = ChartMath.MinMax(currentScale + (0.25 * direction), 1, 4);

                    double cumulativeScale = ValMaxScaleLevel(Math.Max(currentScale + (0.25 * direction), 1));

                    if (ZoomRelativeToCursor)
                    {
                        if (axis.Orientation == Orientation.Horizontal)
                            origin = mousePoint.X / seriesClipRect.Width;
                        else
                            origin = 1d - (mousePoint.Y / seriesClipRect.Height);
                    }

                    canUpdate = canUpdate | Zoom(cumulativeScale, origin > 1d ? 1d : origin < 0d ? 0d : origin, axis);
                }
                canZoom = false;
            }
            if (canUpdate)
                UpdateArea();
        }

#endif

#if WINDOWS_PHONE

        /// <summary>
        /// Called when MouseLeftButtonDown in Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
#else
        /// <summary>
        /// Called when PointerPressed in Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnPointerPressed(PointerRoutedEventArgs e)
#endif
        {
            if (ChartArea.AreaType != ChartAreaType.CartesianAxes)
                return;

#if WPF || SILVERLIGHT_UNCOMMON

            isLeftButtonPressed = true;
            previousPoint = e.GetPosition(this.AdorningCanvas);
          
          
#if  !Silverlight4

            if (e.ClickCount == 2 && ResetOnDoubleTap && ChartArea.SeriesClipRect.Contains(previousPoint))
            {
                Reset();
            }
#endif
#endif
            foreach (var axis in ChartArea.Axes)
            {
                axis.IsScrolling = true;
            }
           
            if (InternalEnableSelectionZooming)
            {
#if WINDOWS_PHONE
                Point point = e.GetPosition(this.AdorningCanvas);

                if (ChartArea.SeriesClipRect.Contains(point))
                {
                    zoomRect = Rect.Empty;
                    Canvas.SetLeft(selectionRectangle, point.X);
                    Canvas.SetTop(selectionRectangle, point.Y);
                    startPoint = point;
                    isZooming = true;
                }
#else
                PointerPoint point = e.GetCurrentPoint(this.AdorningCanvas);

                if (ChartArea.SeriesClipRect.Contains(point.Position))
                {
                    Canvas.SetLeft(selectionRectangle, point.Position.X);
                    Canvas.SetTop(selectionRectangle, point.Position.Y);
                    startPoint = point.Position;
                    AdorningCanvas.CapturePointer(e.Pointer);
                    isZooming = true;
                }
#endif
            }
        }

#if WINDOWS_PHONE
        /// <summary>
        /// Called when MouseMove in chart
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnMouseMove(MouseEventArgs e)
#else
        /// <summary>
        /// Called when PointerMoved in Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnPointerMoved(PointerRoutedEventArgs e)
#endif
        {
#if WPF || SILVERLIGHT_UNCOMMON
            if (isLeftButtonPressed && !isZooming && InternalEnablePanning)
            {

                if (!mouseCapture)
                    mouseCapture = AdorningCanvas.CaptureMouse();
                Point currentPoint = e.GetPosition(this.AdorningCanvas);
                TranslateTransform transform = ChartMath.Translate(previousPoint, currentPoint);
                foreach (ChartAxisBase2D axis in ChartArea.Axes)
                {
                    double currentScale = Math.Max(1 / ChartMath.MinMax(axis.ZoomFactor, 0, 1), 1);
                    if (axis.EnableTouchMode)
                    {
                        if (axis.Area.SeriesClipRect.Contains(currentPoint))
                        {
                            Translate(axis, transform.X, transform.Y, currentScale);
                        }
                    }
                    else
                    {
                        Translate(axis, transform.X, transform.Y, currentScale);
                    }

                }
                previousPoint = currentPoint;
            }
#endif
            if (isZooming)
            {
                Rect areaRect = ChartArea.SeriesClipRect;
#if WINDOWS_PHONE
                if (!mouseCapture)
                    mouseCapture = AdorningCanvas.CaptureMouse();
                Point pt = e.GetPosition(this.AdorningCanvas);
#else
                PointerPoint point = e.GetCurrentPoint(this.AdorningCanvas);
                Point pt = new Point(point.Position.X, point.Position.Y);

#endif
                
                pt = ValidatePoint(pt, areaRect);
                pt.X = ChartMath.MinMax(pt.X, 0, this.AdorningCanvas.Width);
                pt.Y = ChartMath.MinMax(pt.Y, 0, this.AdorningCanvas.Height);

                if (ZoomMode == ZoomMode.X)
                {
                    zoomRect = new Rect(new Point(startPoint.X, ChartArea.SeriesClipRect.Top), new Point(pt.X, ChartArea.SeriesClipRect.Bottom));
                }
                else if (ZoomMode == ZoomMode.Y)
                {
                    zoomRect = new Rect(new Point(ChartArea.SeriesClipRect.Left, startPoint.Y), new Point(ChartArea.SeriesClipRect.Right, pt.Y));
                }
                else
                {
                    zoomRect = new Rect(startPoint, pt);
                }

                Canvas.SetLeft(selectionRectangle, zoomRect.X);
                Canvas.SetTop(selectionRectangle, zoomRect.Y);

                selectionRectangle.Height = zoomRect.Height;
                selectionRectangle.Width = zoomRect.Width;
            }
        }

        private Point ValidatePoint(Point point, Rect rect)
        {
            Point pt = new Point();
            if (point.X < rect.Left)
                pt.X = rect.Left;
            else if (point.X > rect.Right)
                pt.X = rect.Right;
            else
                pt.X = point.X;

            if (point.Y < rect.Top)
                pt.Y = rect.Top;
            else if (point.Y > rect.Bottom)
                pt.Y = rect.Bottom;
            else
                pt.Y = point.Y;

            return pt;
        }

#if WINDOWS_PHONE
        /// <summary>
        /// Called when MouseLeave from Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnMouseLeave(MouseEventArgs e)
        {
            if (isZooming)
            {
                if (!isLeftButtonPressed)
                    AdorningCanvas.ReleaseMouseCapture();
                isZooming = false;
                selectionRectangle.Width = 0;
                selectionRectangle.Height = 0;

                if (zoomRect.Width > 0 && zoomRect.Height > 0)
                {
                    foreach (ChartAxisBase2D axis in this.ChartArea.Axes)
                    {
                        double previousZoomFactor = axis.ZoomFactor;
                        double previousZoomPosition = axis.ZoomPosition;
                        double currentZoomFactor = 0;
                        if (axis.Orientation == Orientation.Horizontal)
                        {
                            currentZoomFactor = previousZoomFactor * (zoomRect.Width / ChartArea.SeriesClipRect.Width);
                            currentZoomFactor = IsMaxZoomLevel ? ValMaxZoomLevel(currentZoomFactor) : currentZoomFactor;
                            axis.ZoomFactor = ZoomMode != ZoomMode.Y
                                ? currentZoomFactor : 1;
                            if (currentZoomFactor != previousZoomFactor)
                                axis.ZoomPosition = ZoomMode != ZoomMode.Y
                                    ? previousZoomPosition + Math.Abs((zoomRect.X - ChartArea.SeriesClipRect.Left) / ChartArea.SeriesClipRect.Width) * previousZoomFactor : 0;
                        }
                        else
                        {
                            currentZoomFactor = previousZoomFactor * zoomRect.Height / ChartArea.SeriesClipRect.Height;
                            currentZoomFactor = IsMaxZoomLevel ? ValMaxZoomLevel(currentZoomFactor) : currentZoomFactor;
                            axis.ZoomFactor = ZoomMode != ZoomMode.X
                                ? currentZoomFactor : 1;
                            if (currentZoomFactor != previousZoomFactor)
                                axis.ZoomPosition = ZoomMode != ZoomMode.X
                                    ? previousZoomPosition + (1 - Math.Abs((zoomRect.Height + zoomRect.Y) / ChartArea.SeriesClipRect.Height)) * previousZoomFactor : 0;
                        }
                    }
                }
            }
        }
#endif


#if WINDOWS_PHONE
        /// <summary>
        /// Called when OnMouse
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
#else
        /// <summary>
        /// Called when pointer released in chart
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnPointerReleased(PointerRoutedEventArgs e)
#endif
        {
            foreach (var axis in ChartArea.Axes)
            {
                axis.IsScrolling = false;
            }
#if WINDOWS_PHONE

            isLeftButtonPressed = false;
            AdorningCanvas.ReleaseMouseCapture();
            mouseCapture = false;
#else
            AdorningCanvas.ReleasePointerCapture(e.Pointer);
#endif
            if (isZooming)
            {
                isZooming = false;
                selectionRectangle.Width = 0;
                selectionRectangle.Height = 0;


                if (zoomRect.Width>0 && zoomRect.Height>0)
                {
                    foreach (ChartAxisBase2D axis in this.ChartArea.Axes)
                    {
                        double previousZoomFactor = axis.ZoomFactor;
                        double previousZoomPosition = axis.ZoomPosition;
                        double currentZoomFactor=0d;
                        if (axis.Orientation == Orientation.Horizontal)
                        {
                            currentZoomFactor=previousZoomFactor * (zoomRect.Width / ChartArea.SeriesClipRect.Width);
                            currentZoomFactor= IsMaxZoomLevel? ValMaxZoomLevel(currentZoomFactor) : currentZoomFactor; 
                            axis.ZoomFactor = ZoomMode != ZoomMode.Y
                                ? currentZoomFactor : 1;
                            if(currentZoomFactor!= previousZoomFactor)
                                axis.ZoomPosition = ZoomMode != ZoomMode.Y
                                    ? previousZoomPosition + Math.Abs((zoomRect.X - ChartArea.SeriesClipRect.Left) / ChartArea.SeriesClipRect.Width) * previousZoomFactor : 0;
                        }
                        else
                        {
                            currentZoomFactor=previousZoomFactor * zoomRect.Height / ChartArea.SeriesClipRect.Height;
                            currentZoomFactor= IsMaxZoomLevel? ValMaxZoomLevel(currentZoomFactor) : currentZoomFactor; 
                            axis.ZoomFactor = ZoomMode != ZoomMode.X
                                ? currentZoomFactor : 1;
                            if (currentZoomFactor != previousZoomFactor)
                                axis.ZoomPosition = ZoomMode != ZoomMode.X
                                    ? previousZoomPosition + (1 - Math.Abs((zoomRect.Height + zoomRect.Y) / ChartArea.SeriesClipRect.Height)) * previousZoomFactor : 0;
                        }
                    }                   

                }
            }           
        }

        double ValMaxZoomLevel(double currentZoomFactor)
        {
            return ((1/currentZoomFactor)<=MaximumZoomLevel ? currentZoomFactor : 1/MaximumZoomLevel);
        }

        double ValMaxScaleLevel(double cumulativeScale)
        {
            if (IsMaxZoomLevel)
                return((cumulativeScale <= MaximumZoomLevel) ? cumulativeScale : MaximumZoomLevel);
            return cumulativeScale;
        }

#if WINDOWS_PHONE

        /// <summary>
        /// Called when ManipulationStarted
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnManipulationStarted(ManipulationStartedEventArgs e)
        {
 	        previousScale = 1;
            foreach (var axis in ChartArea.Axes)
            {
                axis.IsScrolling = true;
            }
        }

        protected internal override void OnManipulationCompleted(ManipulationCompletedEventArgs e)
        {
            base.OnManipulationCompleted(e);
            foreach (var axis in ChartArea.Axes)
            {
                axis.IsScrolling = false;
            }
        }

        /// <summary>
        /// Called when ManipulationDelta is changed
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnManipulationDelta(ManipulationDeltaEventArgs e)
        {
            bool canUpdate = false;
            if (ChartArea.HoldUpdate || ChartArea.AreaType != ChartAreaType.CartesianAxes)
                return;

            double cumulativesManipulationScale = Math.Max(e.CumulativeManipulation.Scale.X, e.CumulativeManipulation.Scale.Y);
            
                foreach (ChartAxisBase2D axis in this.ChartArea.Axes)
                {
                    //double currentScale = ChartMath.MinMax(1 / ChartMath.MinMax(axis.ZoomFactor, 0, 1), 1, 4);

                    double currentScale = Math.Max(1 / ChartMath.MinMax(axis.ZoomFactor, 0, 1), 1);

                    double cumulativeScale = 0;

                    bool isScaled = e.CumulativeManipulation.Scale.X != 0
                        || e.CumulativeManipulation.Scale.Y != 0;

                    double factor = currentScale * ((cumulativesManipulationScale - previousScale) / previousScale);
                    if (
#if !SILVERLIGHT_UNCOMMON
                        EnablePinchZooming &&
#endif
                         isScaled && cumulativesManipulationScale != previousScale && ((axis.Orientation == Orientation.Vertical && (ZoomMode == ZoomMode.Y || ZoomMode == ZoomMode.XY)) ||
                           (axis.Orientation == Orientation.Horizontal && (ZoomMode == ZoomMode.X || ZoomMode == ZoomMode.XY))))
                    {
                        //cumulativeScale = ChartMath.MinMax(currentScale + factor, 1, 4);
                        if(!double.IsNaN(factor) && !double.IsInfinity(factor))
                        {
                        cumulativeScale = ValMaxScaleLevel(Math.Max(currentScale + factor, 1));
                     
                        //if (cumulativeScale >= 1d && cumulativeScale <= 4d)
                        if (cumulativeScale >= 1d)
                        {
                            double origin = 0.5;

                            if (axis.Orientation == Orientation.Horizontal)
                            {
                                origin = e.ManipulationOrigin.X / this.ChartArea.ActualWidth;
                            }
                            else
                            {
                                origin = 1 - (e.ManipulationOrigin.Y / this.ChartArea.ActualHeight);
                            }

                            canUpdate = canUpdate | Zoom(cumulativeScale, origin, axis);
                        }
                        }
                    }
                    else if (InternalEnablePanning)
                    {
                        Translate(axis, e.DeltaManipulation.Translation.X, e.DeltaManipulation.Translation.Y, currentScale);

                        canUpdate = true;
                    }
                }


                if (canUpdate)
                {
                    UpdateArea();
                }
                previousScale = cumulativesManipulationScale;
            
           
        }

#endif

#if !WINDOWS_PHONE

        /// <summary>
        /// Called when Manipulation Started
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnManipulationStarted(ManipulationStartedRoutedEventArgs e)
        {
            previousScale = e.Cumulative.Scale;
        }

        /// <summary>
        /// Called when Manipulation delta is changed in Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnManipulationDelta(ManipulationDeltaRoutedEventArgs e)

        {
            bool canUpdate = false;
            if (ChartArea.HoldUpdate || ChartArea.AreaType != ChartAreaType.CartesianAxes)
                return;
            foreach (ChartAxisBase2D axis in this.ChartArea.Axes)
            {
                //double currentScale = ChartMath.MinMax(1 / ChartMath.MinMax(axis.ZoomFactor, 0, 1), 1, 4);
                
                    double currentScale = Math.Max(1 / ChartMath.MinMax(axis.ZoomFactor, 0, 1), 1);

                    double cumulativeScale = 0;

                    double factor = currentScale * ((e.Cumulative.Scale - previousScale) / previousScale);
                    if (EnablePinchZooming && e.Cumulative.Scale != previousScale && ((axis.Orientation == Orientation.Vertical && (ZoomMode == ZoomMode.Y || ZoomMode == ZoomMode.XY)) ||
                           (axis.Orientation == Orientation.Horizontal && (ZoomMode == ZoomMode.X || ZoomMode == ZoomMode.XY))))
                    {
                        //cumulativeScale = ChartMath.MinMax(currentScale + factor, 1, 4);
                        if(!double.IsNaN(factor) && !double.IsInfinity(factor))
                        {
                        cumulativeScale =ValMaxScaleLevel(Math.Max(currentScale + factor, 1));
                        
                        if (cumulativeScale >= 1d)
                        {
                            double origin = 0.5;

                            if (axis.Orientation == Orientation.Horizontal)
                            {
                                origin = e.Position.X / this.ChartArea.ActualWidth;
                            }
                            else
                            {
                                origin = 1 - (e.Position.Y / this.ChartArea.ActualHeight);
                            }

                            canUpdate = canUpdate | Zoom(cumulativeScale, origin, axis);
                        }
                        }
                    }
                    else if (InternalEnablePanning && !isZooming)
                    {
                        if (axis.EnableTouchMode)
                        {
                            if (axis.Area.SeriesClipRect.Contains(e.Position))
                            {
                                Translate(axis, e.Delta.Translation.X, e.Delta.Translation.Y, currentScale);
                                canUpdate = true;
                                axis.isManipulated = true;
                             }
                        }
                        else
                        {
                            Translate(axis, e.Delta.Translation.X, e.Delta.Translation.Y, currentScale);
                            canUpdate = true;
                        }
                    }
            }

            if (canUpdate)
                UpdateArea();

            previousScale = e.Cumulative.Scale;
        }
        protected internal override void OnManipulationCompleted(ManipulationCompletedRoutedEventArgs e)
        {
            foreach (ChartAxis axis in this.ChartArea.Axes)
            {
                axis.isManipulated = false;
            }
        }

#endif

        private void Translate(ChartAxisBase2D axis, double translateX, double translateY, double currentScale)
        {
            double offset = axis.Orientation == Orientation.Horizontal
                                           ? translateX / this.AdorningCanvas.ActualWidth / currentScale
                                           : translateY / this.AdorningCanvas.ActualHeight / currentScale;

            axis.ZoomPosition = axis.Orientation == Orientation.Horizontal 
                ? ChartMath.MinMax(axis.ZoomPosition - offset, 0, (1 - axis.ZoomFactor)) 
                : ChartMath.MinMax(axis.ZoomPosition + offset, 0, (1 - axis.ZoomFactor));
        }

        /// <summary>
        /// Zooms the specified cumulative scale.
        /// </summary>
        /// <param name="cumulativeScale">The cumulative scale.</param>
        /// <param name="axis">The axis.</param>
        /// <returns></returns>
        public bool Zoom(double cumulativeScale, ChartAxisBase2D axis)
        {
            return Zoom(cumulativeScale, 0.5, axis);
        }

        /// <summary>
        /// Return bool value from the given ChartAxis
        /// </summary>
        /// <param name="cumulativeScale"></param>
        /// <param name="origin"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public virtual bool Zoom(double cumulativeScale, double origin, ChartAxisBase2D axis)
        {
            //if (cumulativeScale >= 1d && cumulativeScale <= 4d)
            if (cumulativeScale >= 1d && axis != null)
            {
                double calcZoomPos = 0;
                double calcZoomFactor = 0;

                CalZoomFactors(cumulativeScale, origin, axis.ZoomFactor, axis.ZoomPosition, ref calcZoomPos, ref calcZoomFactor);
                //calcZoomPos = Math.Round(calcZoomPos, 2);
                if (axis.ZoomPosition != calcZoomPos || axis.ZoomFactor != calcZoomFactor)
                {
                    axis.ZoomPosition = calcZoomPos;
                    axis.ZoomFactor = (calcZoomPos + calcZoomFactor) > 1 ? 1 - calcZoomPos : calcZoomFactor;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Resets the zoom factor and zoom position for all the axis.
        /// </summary>
        public void Reset()
        {
            if (ChartArea != null)
            {
                foreach (ChartAxisBase2D axis in ChartArea.Axes)
                {
                    axis.ZoomPosition = 0d;
                    axis.ZoomFactor = 1d;
                }
            }
        }

        /// <summary>
        /// Calculates ZoomFactor and ZoomPosition using the cumulative scale..
        /// </summary>
        /// <param name="cumulativeScale">Cumulative scale since the starting of the manipulation</param>
        /// <param name="origin">center of manipulation</param>
        /// <param name="currentZoomFactor">Current axis's ZoomFactor</param>
        /// <param name="currentZoomPos">Current axis's ZoomPosition</param>
        /// <param name="calcZoomPos">Calculated ZoomPosition</param>
        /// <param name="calcZoomFactor">Calculated ZoomFactor</param>
        private void CalZoomFactors(double cumulativeScale, double origin, double currentZoomFactor
            , double currentZoomPos, ref double calcZoomPos, ref double calcZoomFactor)
        {
            if (cumulativeScale == 1)
            {
                calcZoomFactor = 1;
                calcZoomPos = 0;
            }
            else
            {
                calcZoomFactor = ChartMath.MinMax(1 / cumulativeScale, 0, 1);
                calcZoomPos = currentZoomPos + ((currentZoomFactor - calcZoomFactor) * origin);
            }
        }

        #endregion
    }
}
