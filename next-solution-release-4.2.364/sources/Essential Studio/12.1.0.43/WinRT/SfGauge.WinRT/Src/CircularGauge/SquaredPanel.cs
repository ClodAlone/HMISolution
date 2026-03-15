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

#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using System.Threading.Tasks;
#else
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
#endif

namespace Syncfusion.UI.Xaml.Gauges
{
    /// <summary>
    ///  SquaredPanel is used to arrange the SfCircularGauge.
    /// </summary>
    public class SquaredPanel: Panel
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Gauges.SquaredPanel"/> class.
        /// </summary>
        public SquaredPanel()
        {
            
        }



        /// <summary>
        ///  Returns the SpacingProperty value of the DependencyObject that used to arrange
        /// and position the DependencyObject
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>
        /// <para>Type: double</para>
        /// <para>It returns the SpacingProperty value of the DependencyObject.</para>
        /// </returns>
        public static double GetSpacing(DependencyObject obj)
        {
            return (double)obj.GetValue(SpacingProperty);
        }

        /// <summary>
        ///  Sets the value of a SpacingProperty, specified by its SpacingProperty
        /// identifier.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetSpacing(DependencyObject obj, double value)
        {
            obj.SetValue(SpacingProperty, value);
        }

        // Using a DependencyProperty as the backing store for Spacing.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SpacingProperty =
            DependencyProperty.RegisterAttached("Spacing", typeof(double), typeof(SquaredPanel), new PropertyMetadata(1d, OnSpacingPropertyChanged));

        private static void OnSpacingPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            UIElement child = obj as UIElement;
            if (child != null)
            {
                SquaredPanel panel = VisualTreeHelper.GetParent(child) as SquaredPanel;
                if (panel != null)
                {
                    panel.InvalidateMeasure();
                }
            }
        }

        private static T FindAnchestor<T>(DependencyObject current)
               where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }
        double childDimension = 0;
        protected override Size MeasureOverride(Size availableSize)
        {
            double mindimension = Math.Min(availableSize.Width , availableSize.Height);
            Size minsize = new Size(mindimension,mindimension);
            double spacing=0;
            
            if (Children.Count > 0)
            {
                if (this.Parent == null)
                {
                    foreach (UIElement child in Children)
                    {
                        double localspacing = GetSpacing(child);
                        double localchildDimension = mindimension * localspacing;
                        child.Measure(new Size(localchildDimension, localchildDimension));
                    }
                }
                else
                {
                    spacing = GetSpacing(Children[0]);
                    childDimension = mindimension * spacing;
                }
            }
            
           
            return minsize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double mindimension = Math.Min(finalSize.Width, finalSize.Height);
            Size minsize = new Size(mindimension, mindimension);
            if (Children.Count > 0)
            {
                
                if (this.Parent == null)
                {
                    foreach (UIElement child in Children)
                    {                       
                        double childspacing = GetSpacing(child) * mindimension;
                        double arrangechildDimension = Math.Min(childspacing, child.DesiredSize.Width);
                        child.Arrange(new Rect((mindimension - arrangechildDimension) / 2, (mindimension - arrangechildDimension) / 2, arrangechildDimension, arrangechildDimension));
                    }
                }
                else
                {
                Children[0].Measure(new Size(childDimension, childDimension));
                Children[1].Measure(new Size(childDimension, childDimension));
                Size commonsize = Children[0].DesiredSize;
                foreach (UIElement child in Children)
                {
                    double childspacing = GetSpacing(child) * mindimension;
                    double arrangechildDimension = Math.Min(childspacing, commonsize.Width);
                    child.Arrange(new Rect((mindimension - arrangechildDimension) / 2, (mindimension - arrangechildDimension) / 2, arrangechildDimension, arrangechildDimension));
                }
                }
            }
            return minsize;
        }
    }
}
