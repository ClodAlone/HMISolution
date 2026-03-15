#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;

namespace Syncfusion.WP.Controls.Media
#else
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.Foundation;

namespace Syncfusion.UI.Xaml.Controls.Media
#endif
{
    /// <summary>
    /// Represents a control that enables the user to select values by moving the slider
    /// </summary>
    /// <remarks>
    /// The selected color is highlighted by the <see 
    /// cref="T:Syncfusion.UI.Xaml.Controls.Media.ColorToolTip"/>
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public class ColorSlider: Control
    {
        #region private member

        private ImageBrush ColorBarBackground;

        internal double previousHue;

        internal bool IsColorChanged = true;

        internal List<Pixel> pixelsCollection = new List<Pixel>();

        private double Maximum = 360, Minimum = 0;

        private PixelBufferInfo pixels;

        private int imageWidth = 360;

        private int imageHeight = 1;

        internal bool IsThumbMoved = false;

        private FrameworkElement PART_Root;

        private bool ispressed = false;

        private FrameworkElement PART_Thumb;

        private Popup PART_Tooltip;

        private CompositeTransform transform;

        private bool IsValueSetInternally = false;

        #endregion

        #region Construtor
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>  
        /// <seealso
        /// cref="N:Syncfusion.UI.Xaml.Controls.Media">Syncfusion.UI.Xaml.Controls.Media
        /// Namespace</seealso>
        [ClassReference(IsReviewed = false)]
        public ColorSlider()
        {
            DefaultStyleKey = typeof(ColorSlider);
            this.Loaded += ColorSliderLoaded;
        }
        #endregion

        #region Override methods

        /// <summary>
        /// Initializes the child elements of the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Media.ColorSlider"/> control.
        /// </summary>
#if WINDOWS_PHONE||WINDOWS_PHONE_7
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            ColorBarBackground = GetTemplateChild("ColorBarBackground") as ImageBrush;
            PART_Root = GetTemplateChild("PART_Root") as FrameworkElement;
            PART_Tooltip = GetTemplateChild("PART_Tooltip") as Popup;
            PART_Thumb = GetTemplateChild("PART_Thumb") as FrameworkElement;

            if (ColorBarBackground != null)
            {
                CreatePallete();
            }

            if (PART_Root != null)
            {
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                PART_Root.MouseLeftButtonDown -= PART_Root_MouseLeftButtonDown;
                PART_Root.MouseMove -= PART_Root_MouseMove;
                PART_Root.ManipulationCompleted -= OnManipulationCompleted;
                PART_Root.MouseLeftButtonDown += PART_Root_MouseLeftButtonDown;
                PART_Root.MouseMove += PART_Root_MouseMove;
                PART_Root.ManipulationCompleted += OnManipulationCompleted;
#else
                PART_Root.PointerPressed -= PartRootPointerPressed;
                PART_Root.PointerMoved -= PartRootPointerMoved;
                PART_Root.PointerReleased -= PartRootPointerReleased;
                PART_Root.PointerPressed += PartRootPointerPressed;
                PART_Root.PointerMoved += PartRootPointerMoved;
                PART_Root.PointerReleased += PartRootPointerReleased;
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

        #region DependencyProperies
        /// <summary>
        /// Returns a value on moving the slider in the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Media.ColorSlider"/> control.
        /// </summary>
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set {SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(ColorSlider), new PropertyMetadata(0.0d, OnValueChanged));
        #endregion

        #region CallBacks

        private void ColorSliderLoaded(object sender, RoutedEventArgs e)
         {
             var rect = new RectangleGeometry { Rect = new Rect(0, 0, ActualWidth, ActualHeight) };
             PART_Root.Clip = rect;
         }

        private static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var control = sender as ColorSlider;
            if (control != null)
            {
                control.OnValueChanged(args);
            }
        }

        private void OnValueChanged(DependencyPropertyChangedEventArgs e)
        {
            double newvalue= (double)e.NewValue;
            UpdateSliderPosition(newvalue);
            if (ValueChanged != null && !IsValueSetInternally)
            {
                ValueChanged(this, e);
            }
        }

#if WINDOWS_PHONE||WINDOWS_PHONE_7
        void OnManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
#else
        private void PartRootPointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#endif
        {
            ispressed = false;
            CloseTooltip();

#if WINDOWS_PHONE||WINDOWS_PHONE_7
            PART_Root.ReleaseMouseCapture();
#else
            PART_Root.ReleasePointerCaptures();
#endif

        }

#if WINDOWS_PHONE||WINDOWS_PHONE_7
        void PART_Root_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
#else
        private void PartRootPointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#endif
        {
            if (ispressed)
            {
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                var point=e.GetPosition(this);
#else
                var point = e.GetCurrentPoint(this).Position;
#endif
                if (PART_Thumb != null && transform!=null)
                {
                    double left = point.X;
                    if (left > 0.0 && left < this.ActualWidth)
                    {
                        transform.TranslateX = left;
                        UpdateValue(left);
                        ApplySelectionColor(left, 0);
                    }
                    else
                    {
                        if (left <= 0)
                            transform.TranslateX = 0;
                        else
                            transform.TranslateX = ActualWidth;
                    }
                    ShowTooltip(transform.TranslateX + 30, 20);
                }
            }
        }

#if WINDOWS_PHONE||WINDOWS_PHONE_7
        void PART_Root_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
#else
        private void PartRootPointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#endif
        {
            ispressed = true;
            previousHue = Value;
#if WINDOWS_PHONE||WINDOWS_PHONE_7
            PART_Root.CaptureMouse();
            var point = e.GetPosition(this);
#else
            PART_Root.CapturePointer(e.Pointer);
            var point = e.GetCurrentPoint(this).Position;
#endif
            if (PART_Thumb != null && transform!=null)
            {
                double left = point.X;
                if (left >= 0.0 && left <= this.ActualWidth)
                {
                    transform.TranslateX = left;
                    ShowTooltip(point.X + 30, 20);
                    UpdateValue(left);
                    ApplySelectionColor(left, 0);

                }
            }
        }

        #endregion

        #region helper methods

        private void CreatePallete()
         {
             var xmax = imageWidth - 1;
             WriteableBitmap bitmap = new WriteableBitmap(imageWidth, imageHeight);
#if WINDOWS_PHONE||WINDOWS_PHONE_7
             pixels = bitmap.Pixels.GetPixels();
#else
             pixels = bitmap.PixelBuffer.GetPixels();
#endif
             for (int x = 0; x < imageWidth; x++)
             {
                 double hue = 360.0 * x / xmax;
                 var c = ColorPickerHelper.FromHsl(hue, 1.0, 0.5);

                 for (int y = 0; y < imageHeight; y++)
                 {
                     Pixel p = new Pixel(c.R, c.G, c.B, c.A);
                     pixelsCollection.Add(p);
                     pixels[imageWidth * y + x] = c.AsInt();
                 }
             }
             ColorBarBackground.ImageSource = bitmap;
             bitmap.Invalidate();
         }

        private void UpdateValue(double newvalue)
        {
            double factor = ActualWidth / Maximum;
            Value = newvalue / factor;

            if (Value > Maximum)
                Value = Maximum;
            if (Value < Minimum)
                Value = Minimum;
        }

        /// <summary>
        /// Updates the position of the thumb on moving the slider
        /// </summary>
        /// <param name="color"></param>
        public void UpdateSliderPosition(Color color)
        {
            HsvColor hsvcolor = color.ToHsv();
            if (hsvcolor.H < 0.0)
            {
                hsvcolor.H += 360;
            }
            if (Value != hsvcolor.H)
            {
                IsValueSetInternally = true;
                Value = hsvcolor.H;
                IsValueSetInternally = false;
            }
            else
                UpdateSliderPosition(hsvcolor.H);
        }

        /// <summary>
        /// Updates the position of the thumb based on the value on moving the slider
        /// </summary>
        /// <param name="value"></param>
        public void UpdateSliderPosition(double value)
        {
            if (value > Maximum)
                value = Maximum;
            if (value < Minimum)
                value = Minimum;
            transform.TranslateX = value * (ActualWidth / Maximum);
        }

        private void ApplySelectionColor(double left, double top)
        {
            double invph = 1.0 / ActualHeight;
            byte A = pixelsCollection[(int)Value].A;
            byte R = pixelsCollection[(int)Value].R;
            byte G = pixelsCollection[(int)Value].G;
            byte B = pixelsCollection[(int)Value].B;

            ((ColorTooltip)PART_Tooltip.Child).SelectedColor = new Color { A = A, R = R, G = G, B = B };
        }

        private void ShowTooltip(double left, double top)
        {
            double _left = left - 120;
            double _top = top - 120;

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

        #endregion

        #region event

        /// <summary>
        /// Invokes an event when the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Media.ColorSlider"/> value has changed
        /// </summary>
        public event PropertyChangedCallback ValueChanged;

        #endregion
    }
}
