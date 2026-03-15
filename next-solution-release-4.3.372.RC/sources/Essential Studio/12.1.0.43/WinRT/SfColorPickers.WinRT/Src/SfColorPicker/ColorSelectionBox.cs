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
#if !WINDOWS_PHONE_7
using System.Threading.Tasks;
#endif
#if WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using Syncfusion.WP.Controls;

namespace Syncfusion.WP.Controls.Media
#else
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Syncfusion.UI.Xaml.Controls.Media
#endif
{
    /// <summary>
    /// Represents a control that enables the user to select values by moving the thumb
    /// </summary>
    /// <remarks>
    /// The selected color is highlighted by the <see 
    /// cref="T:Syncfusion.UI.Xaml.Controls.Media.ColorToolTip"/>
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public class ColorSelectionBox : Control
    {
 
        private Popup PART_Tooltip;

        private ImageBrush PART_ImageBrush;

        private FrameworkElement PART_Canvas;

        internal FrameworkElement PART_Thumb;

        internal CompositeTransform transform;

        internal Color PreviousColor;

        internal bool pointerPressed;

        #region Construtor
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>  
        /// <seealso
        /// cref="N:Syncfusion.UI.Xaml.Controls.Media">Syncfusion.UI.Xaml.Controls.Media
        /// Namespace</seealso>
        [ClassReference(IsReviewed = false)]
        public ColorSelectionBox()
        {
            DefaultStyleKey = typeof(ColorSelectionBox);
            this.LayoutUpdated += ColorSelectionBoxLayoutUpdated;
            this.SizeChanged += ColorSelectionBox_SizeChanged;
            this.Unloaded += ColorSelectionBox_Unloaded;
        }

        #endregion

        #region DependencyProperties

        /// <summary>
        /// Returns a color that has been selected by the user in the 
        /// <see cref="T:Syncfusion.UI.Xaml.Controls.Media.ColorSelectionBox"/> control.
        /// </summary>
        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TargetColor.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorSelectionBox), new PropertyMetadata(Colors.Transparent, OnSelectedColorChanged));

        /// <summary>
        /// Returns the corresponding hue of the color that has been selected by the user in the 
        /// <see cref="T:Syncfusion.UI.Xaml.Controls.Media.ColorSelectionBox"/> control.
        /// </summary>
        public double Hue
        {
            get { return (double)GetValue(HueProperty); }
            set { SetValue(HueProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Hue.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HueProperty =
            DependencyProperty.Register("Hue", typeof(double), typeof(ColorSelectionBox), new PropertyMetadata(0.0d, OnHueChanged));
        
        #endregion

        #region callbacks

        void ColorSelectionBox_Unloaded(object sender, RoutedEventArgs e)
        {
            this.LayoutUpdated -= ColorSelectionBoxLayoutUpdated;
            this.SizeChanged -= ColorSelectionBox_SizeChanged;
            this.Unloaded -= ColorSelectionBox_Unloaded;
        }

#if WINDOWS_PHONE||WINRT          
        private async void ColorSelectionBoxLayoutUpdated(object sender, object e)
        {
            if (await ApplyHueColor(Hue))
            {
#else
                  private void ColorSelectionBoxLayoutUpdated(object sender, object e)
                {
                if (ApplyHueColor(Hue))
                {
#endif
                var rect = new RectangleGeometry { Rect = new Rect(0, 0, ActualWidth, ActualHeight) };
                this.Clip = rect;
                this.LayoutUpdated -= ColorSelectionBoxLayoutUpdated;
            }
        }
        
#if WINDOWS_PHONE||WINRT  
        private async void ColorSelectionBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (await ApplyHueColor(Hue))
            {
#else
                  private void ColorSelectionBox_SizeChanged(object sender, object e)
                {
                if (ApplyHueColor(Hue))
                {
#endif
                var rect = new RectangleGeometry { Rect = new Rect(0, 0, ActualWidth, ActualHeight) };
                this.Clip = rect;
                this.LayoutUpdated -= ColorSelectionBoxLayoutUpdated;
            }
        }

        private static void OnSelectedColorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = sender as ColorSelectionBox;
            if (control != null)
            {
                control.OnSelectedColorChanged(e);
            }
        }

        private void OnSelectedColorChanged(DependencyPropertyChangedEventArgs e)
        {

            UpdateThumbPoision((Color)e.NewValue);

            if (SelectionChanged != null)
            {
                SelectionChanged(this, e);
            }
        }

        private static void OnHueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var control = sender as ColorSelectionBox;
            if (control != null)
            {
                control.OnHueChanged(args);
            }
        }
#if WINDOWS_PHONE||WINRT
        private async void OnHueChanged(DependencyPropertyChangedEventArgs args)
        {
#else
        private void OnHueChanged(DependencyPropertyChangedEventArgs args)
        {
#endif
#if WINDOWS_PHONE_7||SILVERLIGHT||WPF
            Dispatcher.BeginInvoke(() => ApplyHueColor((double)args.NewValue));
#else
           await ApplyHueColor((double)args.NewValue);
#endif
            //ApplySelectionColor(transform.TranslateX, transform.TranslateY);
        }

#if WINDOWS_PHONE||WINDOWS_PHONE_7
        void PART_Canvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
#else
        private void PartThumbPointerMoved(object sender, PointerRoutedEventArgs e)
#endif
        {
            if (pointerPressed)
            {
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                Point point = e.GetPosition(this);
#else
                Point point = e.GetCurrentPoint(this).Position;
#endif
                double left = point.X - PART_Thumb.ActualWidth / 2;
                double top = point.Y - PART_Thumb.ActualHeight / 2;
                if (PART_Thumb != null)
                {
                    var center = new Point(left + PART_Thumb.ActualWidth / 2, top + PART_Thumb.ActualHeight / 2);

                    if (center.X > 0 && center.X < ActualWidth)
                    {
                        transform.TranslateX = left;
                    }
                    else
                    {
                        if (center.X <= 0)
                            transform.TranslateX = 0 - PART_Thumb.ActualWidth/2;
                        else
                            transform.TranslateX = ActualWidth - PART_Thumb.ActualWidth / 2;
                    }

                    if (center.Y > 0 && center.Y < ActualHeight)
                    {
                        transform.TranslateY = top;
                    }
                    else
                    {
                        if (center.Y <= 0)
                            transform.TranslateY = 0 - PART_Thumb.ActualHeight / 2;
                        else
                            transform.TranslateY = ActualHeight - PART_Thumb.ActualHeight / 2;
                    }

                    ShowTooltip(transform.TranslateX, transform.TranslateY);
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                    Dispatcher.BeginInvoke(() => ApplySelectionColor(transform.TranslateX, transform.TranslateY));
#else
                    Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => ApplySelectionColor(transform.TranslateX, transform.TranslateY)).AsTask();
#endif

                }
            }
        }
        
#if WINDOWS_PHONE||WINDOWS_PHONE_7
        void OnManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
#else
        private void PartCanvasPointerReleased(object sender, PointerRoutedEventArgs e)
#endif
        {
            pointerPressed = false;
            CloseTooltip();
        }

#if WINDOWS_PHONE||WINDOWS_PHONE_7
        void PART_Canvas_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
#else    
        private void PartCanvasPointerPressed(object sender, PointerRoutedEventArgs e)
#endif
        {
            pointerPressed =true;
            PreviousColor = SelectedColor;
#if WINDOWS_PHONE||WINDOWS_PHONE_7
            PART_Canvas.CaptureMouse();
            var point = e.GetPosition(this);
#else
            PART_Canvas.CapturePointer(e.Pointer);
            var point = e.GetCurrentPoint(this).Position;
#endif
            double top = point.Y - PART_Thumb.ActualHeight / 2;
            double left = point.X - PART_Thumb.ActualWidth / 2;
            if (PART_Thumb != null)
            {
                transform.TranslateX = left;
                transform.TranslateY = top;
                ShowTooltip(point.X - PART_Thumb.ActualWidth / 2, point.Y - PART_Thumb.ActualHeight / 2);
                ApplySelectionColor(left, top);
            }
        }

        #endregion

        #region Override methods

        /// <summary>
        /// Initializes all the child elements of the
        /// <see cref="T:Syncfusion.UI.Xaml.Controls.Media.ColorSelectionBox"/> control.
        /// </summary>
#if WINDOWS_PHONE||WINDOWS_PHONE_7
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            PART_ImageBrush = GetTemplateChild("PART_ImageBrush") as ImageBrush;
            PART_Canvas = GetTemplateChild("PART_Canvas") as FrameworkElement;
            PART_Thumb = GetTemplateChild("PART_Thumb") as FrameworkElement;
            PART_Tooltip = GetTemplateChild("PAT_Tooltip") as Popup;

            if (PART_Canvas != null)
            {
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                PART_Canvas.MouseLeftButtonDown -= PART_Canvas_MouseLeftButtonDown;
                PART_Canvas.ManipulationCompleted -= OnManipulationCompleted;
                PART_Canvas.MouseMove -= PART_Canvas_MouseMove;
                PART_Canvas.MouseLeftButtonDown += PART_Canvas_MouseLeftButtonDown;
                PART_Canvas.ManipulationCompleted += OnManipulationCompleted;
                PART_Canvas.MouseMove += PART_Canvas_MouseMove;
#else
                PART_Canvas.PointerPressed -= PartCanvasPointerPressed;
                PART_Canvas.PointerReleased -= PartCanvasPointerReleased;
                PART_Canvas.PointerMoved -= PartThumbPointerMoved;
                PART_Canvas.PointerPressed += PartCanvasPointerPressed;
                PART_Canvas.PointerReleased += PartCanvasPointerReleased;
                PART_Canvas.PointerMoved += PartThumbPointerMoved;
#endif
            }

            if (PART_Thumb != null)
            {
                if (transform == null)
                {
                    transform = new CompositeTransform();
                    PART_Thumb.RenderTransform = transform;
                }
            }

            base.OnApplyTemplate();
        }
        #endregion

        #region Helper methods
#if WINDOWS_PHONE||WINRT
        internal async Task<bool>  ApplyHueColor(double hue)
        {
#else 
               internal bool ApplyHueColor(double hue)
            {
#endif
            if (this.ActualHeight > 0.0 && this.ActualWidth > 0.0)
            {
                var bitmap = new WriteableBitmap((int)this.ActualHeight, (int)this.ActualWidth);
#if WINDOWS_PHONE || WINRT
                await bitmap.RenderSaturationValueAsync(hue);
#else
                bitmap.RenderSaturationValue(hue);
#endif
                if (PART_ImageBrush != null)
                {
                    PART_ImageBrush.ImageSource = bitmap;
                }
                return true;
            }
            return false;
        }

        private void ShowTooltip(double left, double top)
        {
            double _left = left - 90;
            double _top = top - 90;

            if (PART_Tooltip != null)
            {
                PART_Tooltip.HorizontalOffset = _left;
                PART_Tooltip.VerticalOffset = _top;
                PART_Tooltip.IsOpen = true;
            }
        }

        private void CloseTooltip()
        {
            if (PART_Tooltip != null)
            {
                PART_Tooltip.IsOpen = false;
            }
        }

        internal Color ApplySelectionColor(double left, double top)
        {
            var color = ApplyColor(Hue, left, top);
            ((ColorTooltip)PART_Tooltip.Child).SelectedColor = color;
            SelectedColor = color;
            return color;
        }

        internal Color ApplyColor(double hue, double left, double top)
        {
            left = ActualWidth - left + (PART_Thumb.ActualWidth / 2);
            if (left < (PART_Thumb.ActualWidth + 3))
                left = left - PART_Thumb.ActualWidth;
            top = ActualHeight - top - (PART_Thumb.ActualHeight / 2);
            if (top < (PART_Thumb.ActualHeight+3))
                top = top - (PART_Thumb.ActualHeight+1.0);
            double aHeight = ActualHeight;
            double aWidth = ActualWidth;
            double invph = 1.0 / ActualHeight;
            var color = ColorPickerHelper.UpdateColorValue(hue, left, top,aHeight,aWidth, invph);
            return color;
        }

        internal void UpdateThumbPoision(Color color)
        {
            if (!pointerPressed && PART_Thumb!=null)
            {
                HsvColor hsvcolor = color.ToHsv();
                if (hsvcolor.H < 0.0)
                {
                    hsvcolor.H += 360;
                }
                double aHeight = ActualHeight + (PART_Thumb.ActualHeight / 2);
                double aWidth = ActualWidth + (PART_Thumb.ActualWidth / 2);
                Point point = hsvcolor.GetCanvasPosition(aHeight, aWidth);
                
                double top = ActualHeight - (point.Y);
                double left = ActualWidth  - (point.X);

                if (point.Y < 1)
                {
                    top = top - (PART_Thumb.ActualHeight/2);
                }
                if (point.X < 1)
                {
                    left = left - (PART_Thumb.ActualWidth/2);
                }
                
                if (PART_Thumb != null)
                {
                    var center = new Point(left + PART_Thumb.ActualWidth / 2, top + PART_Thumb.ActualHeight / 2);

                    if (center.X >= -(PART_Thumb.ActualWidth / 2) && center.X < ActualWidth + (PART_Thumb.ActualWidth / 2))
                    {
                        transform.TranslateX = left;
                    }
                    if (center.Y > ActualHeight)
                    {
                        center.Y = ActualHeight;
                    }

                    if (center.Y >= -(PART_Thumb.ActualWidth / 2) && center.Y < ActualHeight + (PART_Thumb.ActualHeight / 2))
                    {
                        transform.TranslateY = top;
                    }
                }
            }
        }

        #endregion

        #region event

        /// <summary>
        /// Invokes an event when the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Media.ColorSelectionBox"/> selected color has changed
        /// </summary>
        public event PropertyChangedCallback SelectionChanged;

        #endregion
    }
}
