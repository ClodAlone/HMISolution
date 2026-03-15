#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using Syncfusion.UI.Xaml.Diagram.Utility;
#if WINRT_USING
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls; 
#else
using System.Windows.Controls; 
#endif

namespace Syncfusion.UI.Xaml.Diagram.Panels
{
#if !WINRT
    [DesignTimeVisible(false)] 
#endif
    public class WrapPanel : Panel
    {
        /// <summary>
        /// Defines the Orientation property.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(WrapPanel), new PropertyMetadata(Orientation.Horizontal));

        /// <summary>
        /// Initializes a new instance of the <see cref="WrapPanel"/> class.
        /// </summary>
        public WrapPanel()
        {
        }

        /// <summary>
        /// Gets or sets the HierarchicalDataTemplate for items.
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Positions child elements and determines a size for the control.
        /// </summary>
        /// <param name="finalSize">The final area within the parent
        /// that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            Point p = new Point(0, 0);
            int count = 1;
            double finalheight = 0;
            double temp = 0;
            if (Orientation == Orientation.Horizontal)
            {
                double maxheight = 0.0;
                foreach (UIElement element in this.Children)
                {
                    double elementwidth = element.DesiredSize.Width;
                    double elementheight = element.DesiredSize.Height;
                    element.Arrange(new Rect(p, new Point(p.X + elementwidth, p.Y + elementheight)));
                    if (elementheight > maxheight)
                    {
                        maxheight = elementheight;
                        temp = elementheight;
                    }

                    p.X = p.X + elementwidth;
                    if (count < this.Children.Count && (p.X + this.Children[count].DesiredSize.Width) > finalSize.Width)
                    {
                        p.X = 0;
                        p.Y = p.Y + maxheight;
                        maxheight = 0;
                        temp = elementheight;
                    }

                    count++;
                    finalheight = Math.Max(p.Y, finalheight);
                }
            }
            else
            {
                double maxwidth = 0.0;
                foreach (UIElement element in this.Children)
                {
                    double elementwidth = element.DesiredSize.Width;
                    double elementheight = element.DesiredSize.Height;
                    element.Arrange(new Rect(p, new Point(p.X + element.DesiredSize.Width, p.Y + element.DesiredSize.Height)));
                    if (elementwidth > maxwidth)
                    {
                        maxwidth = elementwidth;
                    }

                    p.Y = p.Y + elementheight;
                    if (count < this.Children.Count && (p.Y + this.Children[count].DesiredSize.Height) > finalSize.Height)
                    {
                        p.Y = 0;
                        p.X = p.X + maxwidth;
                        maxwidth = 0;
                    }

                    count++;
                    finalheight = Math.Max(p.Y, finalheight);
                }
            }

            return new Size(finalSize.Width, finalheight + temp);
        }

        /// <summary>
        /// Measures elements.
        /// </summary>
        /// <param name="availableSize">The available size</param>
        /// <returns>The available size.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (Orientation == Orientation.Vertical)
            {
                availableSize = new Size(availableSize.Height, availableSize.Width);
            }
            double currentX = 0;
            double currentY = 0;
            double maxX = 0;
            double maxRowHeight = 0;
            bool firstItem = true;
            foreach (UIElement child in Children)
            {
                child.Measure(availableSize);
                Size childSize = child.DesiredSize;
                if (Orientation == Orientation.Vertical)
                {
                    childSize = new Size(childSize.Height, childSize.Width);
                }
                if (!firstItem && currentX + childSize.Width > availableSize.Width)
                {
                    firstItem = true;
                    if (currentX > maxX)
                    {
                        maxX = currentX;
                    }
                    currentX = 0;
                    currentY += maxRowHeight;
                    maxRowHeight = childSize.Height;
                }
                else
                {
                    firstItem = false;
                    currentX += childSize.Width;
                    if (maxRowHeight < childSize.Height)
                    {
                        maxRowHeight = childSize.Height;
                    }
                }
            }
            if (currentX > maxX)
            {
                maxX = currentX;
            }
            currentY += maxRowHeight;
            if (Orientation == Orientation.Vertical)
            {
                return new Size(currentY, maxX);
            }
            return new Size(maxX, currentY);
        }
    }
}
