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
#if WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows;
using System.Windows.Controls;

namespace Syncfusion.WP.Controls.Media
#elif SILVERLIGHT
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
namespace Syncfusion.Tools.Controls.Media
#elif WPF
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
namespace Syncfusion.Windows.Controls.Media
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Foundation;

namespace Syncfusion.UI.Xaml.Controls.Media
#endif
{
    /// <summary>
    /// Represents a base class for defining the swatches properties.
    /// </summary>
    public class SwatchesBase:Control
    {
        internal double ColorItemWidth
        {
            get { return (double)GetValue(ColorItemWidthProperty); }
            set
            {
                if (value < 0)
                    value = 0;
                SetValue(ColorItemWidthProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for ColorItemWidth.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ColorItemWidthProperty =
            DependencyProperty.Register("ColorItemWidth", typeof(double), typeof(SwatchesBase), new PropertyMetadata(0.0d));

        internal Thickness ColorItemMargin
        {
            get { return (Thickness)GetValue(ColorItemMarginProperty);}
            set { SetValue(ColorItemMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColorItemWidth.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ColorItemMarginProperty =
            DependencyProperty.Register("ColorItemMargin", typeof(Thickness), typeof(SwatchesBase), new PropertyMetadata(new Thickness(0.5d)));
        
        /// <summary>
        /// Measures the size for overriding
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size availableSize)
        {
#if WPFSILVERLIGHT
            List<ScrollViewer> scrollViewers = VisualTreeEnumeration.GetDescendants<ScrollViewer>(this);
            scrollViewers[0].ApplyTemplate();
#if WPF
            ScrollBar scrollBar = scrollViewers[0].Template.FindName("PART_VerticalScrollBar", scrollViewers[0]) as ScrollBar;
#else
            ScrollBar scrollBar = ((FrameworkElement)VisualTreeHelper.GetChild(scrollViewers[0], 0)).FindName("VerticalScrollBar") as ScrollBar;
#endif
            ColorItemWidth = ((availableSize.Width - (scrollBar.Width + (scrollViewers[0].BorderThickness.Left + scrollViewers[0].BorderThickness.Right + scrollViewers[0].BorderThickness.Top + scrollViewers[0].BorderThickness.Bottom))) / 6) - (ColorItemMargin.Top + ColorItemMargin.Bottom + ColorItemMargin.Left + ColorItemMargin.Right);
#else
            ColorItemWidth = (availableSize.Width / 6) - (ColorItemMargin.Top + ColorItemMargin.Bottom + ColorItemMargin.Left + ColorItemMargin.Right);
#endif
            return base.MeasureOverride(availableSize);
        }
       
    }
#if WPFSILVERLIGHT
    internal static class VisualTreeEnumeration
    {
        public static List<T> GetDescendants<T>(this DependencyObject parent)
               where T : UIElement
        {
            List<T> children = new List<T>();

            int count = VisualTreeHelper.GetChildrenCount(parent);

            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    UIElement child = (UIElement)VisualTreeHelper.GetChild(parent, i);

                    if (child is T)
                    {
                        children.Add((T)child);
                    }

                    children.AddRange(child.GetDescendants<T>());
                }
                return children;
            }
            else
            {
                return new List<T> { };
            }
        }
    }
#endif
}
