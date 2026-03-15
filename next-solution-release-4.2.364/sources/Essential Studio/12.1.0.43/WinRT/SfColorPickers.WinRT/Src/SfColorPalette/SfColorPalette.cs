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
#if !(WINDOWS_PHONE_7 || Silverlight4)
using System.Threading.Tasks;
#endif
#if WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Syncfusion.WP.Controls.Media
#elif SILVERLIGHT
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
namespace Syncfusion.Tools.Controls.Media
#elif WPF
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using Syncfusion.Licensing;
namespace Syncfusion.Windows.Controls.Media
#else
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.Foundation;
using Windows.UI.Xaml.Media;
namespace Syncfusion.UI.Xaml.Controls.Media
#endif
{
    /// <summary>
    /// Represents a control that provides a set of colors in different <see 
    /// cref="T:Syncfusion.UI.Xaml.Controls.Media.ColorSwatches"/> swatches.
    /// </summary>
    /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Media.SfColorPalette"/>
    public class SfColorPalette:Control
    {
        SfNavigator Part_Navigator;
        ColorSwatches Part_ColorSwatches;
        TextBlock Part_Content;
        ColorPaletteButton PART_ColorPaletteButton;
        private FrameworkElement previousElement,currentElement;
#if WINRT
        internal ResourceWrapper resourceWrapper;

#endif
#if!WINRT
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>  
        /// <seealso
        /// cref="N:Syncfusion.UI.Xaml.Controls.Media">Syncfusion.UI.Xaml.Controls.Media
        /// Namespace</seealso>
        [ClassReference(IsReviewed = false)]
#else
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="Syncfusion.UI.Xaml.Controls.Media.SfColorPalette"/> class.
        /// </summary>  
        /// <seealso
        /// cref="N:Syncfusion.UI.Xaml.Controls.Media">Syncfusion.UI.Xaml.Controls.Media
        /// Namespace</seealso>
        [ClassReference(IsReviewed = false)]
#endif
        public SfColorPalette()
        {
#if WPF
            if (EnvironmentTestMedia.IsSecurityGranted)
            {
                EnvironmentTestMedia.StartValidateLicense(typeof(SfColorPalette));
            }
#endif
            DefaultStyleKey = typeof(SfColorPalette);
            Loaded += SfColorPalette_Loaded;
#if WINRT
            resourceWrapper = new ResourceWrapper();
#endif         
            SizeChanged += SfColorPalette_SizeChanged;
            Unloaded += SfColorPalette_Unloaded;   
        }

        void SfColorPalette_Unloaded(object sender, RoutedEventArgs e)
        {
            Loaded -= SfColorPalette_Loaded;
            SizeChanged -= SfColorPalette_SizeChanged;
            Unloaded -= SfColorPalette_Unloaded;
        }

        void SfColorPalette_SizeChanged(object sender, SizeChangedEventArgs e)
        {
#if WPF
            (sender as SfColorPalette).MaxHeight = ((sender as SfColorPalette).Parent as FrameworkElement).ActualHeight;
            (sender as SfColorPalette).MaxWidth = ((sender as SfColorPalette).Parent as FrameworkElement).ActualWidth;
#endif
            if (Part_Navigator != null && e.PreviousSize.ToString() != "0,0")
            {
                if (Part_Navigator.ActualHeight > 0 && Part_Navigator.ActualWidth > 0)
                {
                    var rectangle = new RectangleGeometry();
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                  rectangle.Rect = new System.Windows.Rect {Height = Part_Navigator.ActualHeight, Width = Part_Navigator.ActualWidth, X = 0, Y = 0};
#else
                    rectangle.Rect = new Rect { Height = Part_Navigator.ActualHeight, Width = Part_Navigator.ActualWidth, X = 0, Y = 0 };
#endif
                    Part_Navigator.Clip = rectangle;
                }
                Part_Navigator.UpdateTransform();
            }
        }

        void SfColorPalette_Loaded(object sender, RoutedEventArgs e)
        {
#if WINRT||WINDOWS_PHONE||WINDOWS_PHONE_7
            if (MaxHeight == double.PositiveInfinity)
                MaxHeight = ((FrameworkElement)this.Parent).ActualHeight;
            if (MaxWidth == double.PositiveInfinity)
                MaxWidth = ((FrameworkElement)this.Parent).ActualWidth;
#elif !WPF
            if (MaxHeight == double.PositiveInfinity)
                MaxHeight = Application.Current.Host.Content.ActualHeight;
            if (MaxWidth == double.PositiveInfinity)
                MaxWidth = Application.Current.Host.Content.ActualWidth;
#endif
        }

        #region Events

        /// <summary>
        /// Invoke's an event when the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Media.SfColorPalette"/> conntrol's 
        /// selected color has changed.
        /// </summary>
        public event DependencyPropertyChangedEventHandler SelectedColorChanged;

        #endregion

        /// <summary>
        /// Initializes all the child elements of the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Media.SfColorPalette"/> control.
        /// </summary>
#if !WINRT
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
#if WINRT
            Part_ColorSwatches = GetTemplateChild("PART_ColorSwatches") as ColorSwatches;
#endif
            PART_ColorPaletteButton = GetTemplateChild("PART_ColorPaletteButton") as ColorPaletteButton;
            Part_Navigator = GetTemplateChild("PART_Navigator") as SfNavigator;
            Part_Content = GetTemplateChild("PART_Text") as TextBlock;
            
            if (Part_Navigator != null)
            {
                Part_Navigator.Navigated += Part_Navigator_Navigated;
#if !WINRT
                Part_ColorSwatches = new ColorSwatches();
                Part_ColorSwatches.VerticalAlignment=VerticalAlignment.Stretch;
                Part_ColorSwatches.HorizontalAlignment=HorizontalAlignment.Stretch;
                Part_Navigator.Items.Add(Part_ColorSwatches);
                Part_Navigator.Items.Add(new Apex());
                Part_Content.Text = "Colors";
#else
                Part_Content.Text = resourceWrapper.Colors;
#endif
            }

            if (PART_ColorPaletteButton != null)
            {
                PART_ColorPaletteButton.Click -= OnColorPaletteButtonClicked;
                PART_ColorPaletteButton.Visibility = Visibility.Visible;
                PART_ColorPaletteButton.Click += OnColorPaletteButtonClicked;
            }

            if (Part_ColorSwatches != null)
            {
                Part_ColorSwatches.OnPaletteItemClicked += Part_ColorViewer_OnPaletteItemClicked;
            }
            IsEnabledChanged += SfColorPalette_IsEnabledChanged;
            if (!IsEnabled)
                VisualStateManager.GoToState(this, "Disabled", true);

            base.OnApplyTemplate();
        }

        void SfColorPalette_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((sender as SfColorPalette).IsEnabled)
                VisualStateManager.GoToState(this, "Normal", true);
            else
                VisualStateManager.GoToState(this, "Disabled", true);
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
#if WPF
            if(IsFocused)
#elif SILVERLIGHT
#else
            if (FocusState == FocusState.Keyboard||FocusState == FocusState.Programmatic)
#endif
            VisualStateManager.GoToState(this, "Focused", true);
#if WINRT
            else if(FocusState==FocusState.Pointer)

                VisualStateManager.GoToState(this, "PointerFocused", true);
#endif
#endif
            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
#if WPF
            if(!IsFocused)
#elif SILVERLIGHT
#else
            if (FocusState == FocusState.Unfocused)
#endif
                VisualStateManager.GoToState(this, "Unfocused", true);
#endif
            base.OnLostFocus(e);
        }

        void Part_Navigator_Navigated(object sender, RoutedEventArgs e)
        {
           SfNavigator control = (sender as SfNavigator);
           if (control.ActiveIndex == 0)
           {
               PART_ColorPaletteButton.Visibility = Visibility.Collapsed;
#if WINRT
               Part_Content.Text = resourceWrapper.Swatches;
#else
               Part_Content.Text = "Swatches";
#endif
           }
           else
           {
               PART_ColorPaletteButton.Visibility = Visibility.Visible;
#if WINRT
               Part_Content.Text = resourceWrapper.Colors;
#else
               Part_Content.Text = "Colors";
#endif
           }
        }

        /// <summary>
        /// Returns a color from the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Media.SfColorPalette"/> control that has been selected.
        /// </summary>
        /// <remarks>
        /// The color can be selected from the
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Media.ColorSwatches"/>
        /// </remarks>
        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            internal set { SetValue(SelectedColorProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedColor.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(Color), typeof(SfColorPalette), new PropertyMetadata(Colors.Black,new PropertyChangedCallback(OnSelectedColorChanged)));

        private static void OnSelectedColorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = sender as SfColorPalette;
            if (control != null)
                control.OnSelectedColorChanged(e);
        }

        private void OnSelectedColorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SelectedColorChanged != null)
                SelectedColorChanged(this, e);
        }

        /// <summary>
        /// Returns the focus to the color palette from the Swatches
        /// </summary>
        public void GoBack()
        {
            if (Part_Navigator != null && Part_Navigator.Items.Count > 1)
            {
                Part_Navigator.ActiveIndex = 0;
                Part_Navigator.Items.RemoveAt(1);
                CanGoBack = false;
            }
        }

        /// <summary>
        /// Represents a variable to determine whether the control can navigate back.
        /// </summary>
        /// <value>
        /// <c>true</c> if can navigate back; otherwise, <c>false</c>.
        /// </value>
        public bool CanGoBack = false;

        void OnColorPaletteButtonClicked(object sender, RoutedEventArgs e)
        {
            GoBack();
        }

        private void  Part_ColorViewer_OnPaletteItemClicked(object sender, RoutedEventArgs e)
        {
            ColorItem control = (sender as ColorItem);
            object Swatch=null;
           if (previousElement == null && PART_ColorPaletteButton != null &&
               PART_ColorPaletteButton.Apex.Visibility == Visibility.Visible)
           {
               previousElement = PART_ColorPaletteButton.Apex;
           }
           else
           {
               previousElement.Visibility = Visibility.Collapsed;
           }
           if (control != null)
            {
                switch(control.Tag.ToString())
                {
                    case "Apex":
                        Swatch = new Apex();
                        currentElement = PART_ColorPaletteButton.Apex;
                        break;
                    case "Hardcover":
                        Swatch = new Hardcover();
                        currentElement = PART_ColorPaletteButton.Hardcover;
                        break;
                    case "Metro":
                        Swatch = new Metro();
                        currentElement = PART_ColorPaletteButton.Metro;
                        break;
                    case "Module":
                        Swatch = new Module();
                        currentElement = PART_ColorPaletteButton.Module;
                        break;
                    case "Office":
                        Swatch = new Office();
                        currentElement = PART_ColorPaletteButton.Office;
                        break;
                    case "Paper":
                        Swatch = new Paper();
                        currentElement = PART_ColorPaletteButton.Paper;
                        break;
                    case "Pushpin":
                        Swatch = new Pushpin();
                        currentElement = PART_ColorPaletteButton.Pushpin;
                        break;
                    case "Solstice":
                        Swatch = new Solstice();
                        currentElement = PART_ColorPaletteButton.Solstice;
                        break;
                    case "Urban":
                        Swatch = new Urban();
                        currentElement = PART_ColorPaletteButton.Urban;
                        break;
                    case "Waveform":
                        Swatch = new Waveform();
                        currentElement = PART_ColorPaletteButton.Waveform;
                        break;
                }
                currentElement.Visibility = Visibility.Visible;
                previousElement = currentElement;
                if (Swatch != null)
                {
                    Part_Navigator.Items.Add(Swatch);
                    CanGoBack = true;
                    Part_Navigator.ActiveIndex = 1;
                }
            }
        }
    }
}