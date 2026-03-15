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
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// 
    /// </summary>
    public class OverviewCustomPanel : Panel
    {
#if WPF
        internal Overview Overview
        {
            get { return (Overview)GetValue(OverviewProperty); }
            set { SetValue(OverviewProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Overview.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty OverviewProperty =
            DependencyProperty.Register("Overview", typeof(Overview), typeof(OverviewCustomPanel), new PropertyMetadata(null));
        /// <summary>
        /// 
        /// </summary>
        public OverviewCustomPanel()
        {
            Binding bin = new Binding();
            bin.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(Overview), 1);
            this.SetBinding(OverviewProperty, bin);

            this.Loaded += new RoutedEventHandler(OverviewCustomPanel_Loaded);
        }

        void OverviewCustomPanel_Loaded(object sender, RoutedEventArgs e)
        {
            if (Overview != null)
            {
                this.InvalidateMeasure();
            }
        }
#endif
        /// <summary>
        /// 
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            //return base.MeasureOverride(availableSize);
            Size desiredSize = new Size(0, 0);
            foreach (FrameworkElement element in Children)
            {
                if (element is OverviewResizer)
                {
                    element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                }
                else
                {
                    element.Measure(availableSize);
                }
                if (!(element is OverviewResizer || element is Grid))
                {
                    desiredSize.Width = Math.Max(desiredSize.Width, double.IsNaN(element.DesiredSize.Width) || double.IsInfinity(element.DesiredSize.Width) ? 0d : element.DesiredSize.Width);
                    desiredSize.Height = Math.Max(desiredSize.Height, double.IsNaN(element.DesiredSize.Height) || double.IsInfinity(element.DesiredSize.Height) ? 0d : element.DesiredSize.Height);
                }
            }
#if WPF
            if (Overview != null && Overview.ScrollSource != null)
            {
                return availableSize.GetUniformSize(new Size(Overview.ScrollSource.ExtentWidth, Overview.ScrollSource.ExtentHeight));
            }
#endif
            return desiredSize;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            //return base.ArrangeOverride(finalSize);
            Size actualSize = new Size(0, 0);
            foreach (FrameworkElement element in Children)
            {
                if (element is OverviewResizer)
                {
                    element.Arrange(new Rect(new Point(0, 0), element.DesiredSize));
                }
                else
                {
                    element.Arrange(new Rect(new Point(0, 0), finalSize));
                }
                if (!(element is OverviewResizer || element is Grid))
                {
                    actualSize.Width = Math.Max(actualSize.Width, double.IsNaN(element.ActualWidth) || double.IsInfinity(element.ActualWidth) ? 0d : element.ActualWidth);
                    actualSize.Height = Math.Max(actualSize.Height, double.IsNaN(element.ActualHeight) || double.IsInfinity(element.ActualHeight) ? 0d : element.ActualHeight);
                }
            }
#if WPF
            if (Overview != null && Overview.ScrollSource != null)
            {
                return finalSize.GetUniformSize(new Size(Overview.ScrollSource.ExtentWidth, Overview.ScrollSource.ExtentHeight));
            }
#endif
            return actualSize;
        }
    }
}
