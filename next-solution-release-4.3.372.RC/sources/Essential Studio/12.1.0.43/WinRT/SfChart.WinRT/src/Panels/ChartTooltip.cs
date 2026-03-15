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
using System.Threading;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Animation;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.Foundation;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

#endif

namespace Syncfusion.UI.Xaml.Charts
{
    public class ChartTooltip : ContentControl
    {

        #region Properties

        new public static readonly DependencyProperty HorizontalAlignmentProperty =
            DependencyProperty.RegisterAttached("HorizontalAlignment", typeof (HorizontalAlignment),
                                                typeof (ChartTooltip),
                                                new PropertyMetadata(HorizontalAlignment.Center));

        new public static readonly DependencyProperty VerticalAlignmentProperty =
            DependencyProperty.RegisterAttached("VerticalAlignment", typeof(VerticalAlignment), typeof(ChartTooltip),
                                                new PropertyMetadata(VerticalAlignment.Top));

        public static readonly DependencyProperty EnableAnimationProperty =
            DependencyProperty.RegisterAttached("EnableAnimation", typeof(bool), typeof(ChartTooltip),
                                                new PropertyMetadata(false));

        public static readonly DependencyProperty TooltipMarginProperty =
            DependencyProperty.RegisterAttached("TooltipMargin", typeof(Thickness), typeof(ChartTooltip),
                                                new PropertyMetadata(new Thickness(0)));

        // Using a DependencyProperty as the backing store for LeftOffset.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty TopOffsetProperty =
            DependencyProperty.Register("TopOffset", typeof (double), typeof (ChartTooltip),
                                        new PropertyMetadata(0d));


        // Using a DependencyProperty as the backing store for LeftOffset.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty LeftOffsetProperty =
            DependencyProperty.Register("LeftOffset", typeof (double), typeof (ChartTooltip),
                                        new PropertyMetadata(0d));

        internal double LeftOffset
        {
            get { return (double) GetValue(LeftOffsetProperty); }
            set { SetValue(LeftOffsetProperty, value); }
        }


        internal double TopOffset
        {
            get { return (double) GetValue(TopOffsetProperty); }
            set { SetValue(TopOffsetProperty, value); }
        }

        #endregion

        #region Methods

        public static bool GetEnableAnimation(UIElement obj)
        {
            return (bool) obj.GetValue(EnableAnimationProperty);
        }

        public static void SetEnableAnimation(UIElement obj, bool value)
        {
            obj.SetValue(EnableAnimationProperty, value);
        }

        public static HorizontalAlignment GetHorizontalAlignment(UIElement obj)
        {
            return (HorizontalAlignment) obj.GetValue(HorizontalAlignmentProperty);
        }

        public static void SetHorizontalAlignment(UIElement obj, HorizontalAlignment value)
        {
            obj.SetValue(HorizontalAlignmentProperty, value);
        }

        public static VerticalAlignment GetVerticalAlignment(UIElement obj)
        {
            return (VerticalAlignment) obj.GetValue(VerticalAlignmentProperty);
        }

        public static void SetVerticalAlignment(UIElement obj, VerticalAlignment value)
        {
            obj.SetValue(VerticalAlignmentProperty, value);
        }

        public static Thickness GetTooltipMargin(UIElement obj)
        {
            return (Thickness) obj.GetValue(TooltipMarginProperty);
        }

        public static void SetTooltipMargin(UIElement obj, Thickness value)
        {
            obj.SetValue(TooltipMarginProperty, value);
        }

        #endregion

        #region ctor
        public ChartTooltip()
        {
            Canvas.SetLeft(this, 0d);
            Canvas.SetTop(this, 0d);
        }
        #endregion
    }
}
