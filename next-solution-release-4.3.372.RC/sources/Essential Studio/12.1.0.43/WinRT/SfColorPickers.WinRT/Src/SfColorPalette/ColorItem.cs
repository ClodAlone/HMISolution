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
using System.Windows.Controls.Primitives;
namespace Syncfusion.Windows.Controls.Media
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
namespace Syncfusion.UI.Xaml.Controls.Media
#endif
{
    /// <summary>
    /// Represents a color item in the
    /// <see cref="T:Syncfusion.UI.Xaml.Controls.Media.SfColorPalette"/> control.
    /// </summary>
    /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Media.ColorSwatches"/>    
    [ClassReference(IsReviewed = false)]
    public class ColorItem:Button
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
        /// cref="Syncfusion.UI.Xaml.Controls.Media.ColorItem"/> class.
        /// </summary>  
        /// <seealso
        /// cref="N:Syncfusion.UI.Xaml.Controls.Media">Syncfusion.UI.Xaml.Controls.Media
        /// Namespace</seealso>
        [ClassReference(IsReviewed = false)]
#endif
        public ColorItem()
        {
            DefaultStyleKey = typeof(ColorItem);
            this.Click += ColorItem_Click;
        }

        void ColorItem_Click(object sender, RoutedEventArgs e)
        {
            if (TooltipVisibility == Visibility.Visible)
            {
                var palette = FindAncestor(this) as SfColorPalette;
                if (palette != null)
                {
                    var solidColorBrush = (SolidColorBrush) this.Background;
                    if (solidColorBrush != null)
                        palette.SelectedColor = solidColorBrush.Color;
                }
            }
        }

        private DependencyObject FindAncestor(DependencyObject element)
        {
            var _element = VisualTreeHelper.GetParent(element);
            if (!(_element is SfColorPalette))
            {
               return FindAncestor(_element);
            }
            else
            {
                return _element;
            }
        }

        /// <summary>
        /// Returns the visibility status of the Tool Tip
        /// </summary>
        public Visibility TooltipVisibility
        {
            get { return (Visibility)GetValue(TooltipVisibilityProperty); }
            set { SetValue(TooltipVisibilityProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TooltipVisibility.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TooltipVisibilityProperty =
            DependencyProperty.Register("TooltipVisibility", typeof(Visibility), typeof(ColorItem), new PropertyMetadata(Visibility.Visible));

    }

}
