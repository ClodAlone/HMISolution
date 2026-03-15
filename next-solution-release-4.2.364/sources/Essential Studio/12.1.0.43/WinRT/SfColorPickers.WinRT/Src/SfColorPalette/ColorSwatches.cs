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
using System.Windows.Controls;
using System.Windows;

namespace Syncfusion.WP.Controls.Media
#elif SILVERLIGHT
using System.Windows.Controls;
using System.Windows;
namespace Syncfusion.Tools.Controls.Media
#elif WPF
using System.Windows.Controls;
using System.Windows;
namespace Syncfusion.Windows.Controls.Media
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Foundation;


namespace Syncfusion.UI.Xaml.Controls.Media
#endif
{
    /// <summary>
    /// Represents a set of swatches to allow the user to select colors from <see 
    /// cref="T:Syncfusion.UI.Xaml.Controls.Media.SfColorPalette"/>
    /// </summary>
    /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Media.SwatchesBase"/>
    public class ColorSwatches: Control
    {
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
        /// cref="Syncfusion.UI.Xaml.Controls.Media.ColorSwatches"/> class.
        /// </summary>  
        /// <seealso
        /// cref="N:Syncfusion.UI.Xaml.Controls.Media">Syncfusion.UI.Xaml.Controls.Media
        /// Namespace</seealso>
        [ClassReference(IsReviewed = false)]
#endif
        public ColorSwatches()
        {
            DefaultStyleKey = typeof(ColorSwatches);
        }



        void OnColorItemClicked(object sender, RoutedEventArgs e)
        {
            if (OnPaletteItemClicked != null)
                OnPaletteItemClicked(sender, e);
        }

        internal event RoutedEventHandler OnPaletteItemClicked;

        /// <summary>
        /// Initializes all the child elements of the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Media.ColorSwatches"/> control.
        /// </summary>
#if !WINRT
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            ColorItem item;

            for (int i = 0; i < 10; i++)
            {
                item = GetTemplateChild("PART_Item" + i) as ColorItem;
                if (item != null)
                {
                    item.Click -= OnColorItemClicked;
                    item.Click += OnColorItemClicked;
                }
            }
            base.OnApplyTemplate();
        }

        internal double ColorItemWidth
        {
            get { return (double)GetValue(ColorItemWidthProperty); }
            set { SetValue(ColorItemWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColorItemWidth.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ColorItemWidthProperty =
            DependencyProperty.Register("ColorItemWidth", typeof(double), typeof(ColorSwatches), new PropertyMetadata(0.0d));

        internal Thickness ColorItemMargin
        {
            get { return (Thickness)GetValue(ColorItemMarginProperty); }
            set { SetValue(ColorItemMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColorItemWidth.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ColorItemMarginProperty =
            DependencyProperty.Register("ColorItemMargin", typeof(Thickness), typeof(ColorSwatches), new PropertyMetadata(new Thickness(5d)));

        /// <summary>
        /// Reeturns a value regrding the size of the swatches buttons.
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            ColorItemWidth = (availableSize.Width / 3) - (ColorItemMargin.Top + ColorItemMargin.Bottom + ColorItemMargin.Left + ColorItemMargin.Right);
            return base.MeasureOverride(availableSize);
        }
    }
}
