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

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// 
    /// </summary>
    public class OverviewCustomPanel : Panel
    {
        /// <summary>
        /// Provides the behavior for the Measure pass of Silverlight layout. Classes can override this method to define their own Measure pass behavior.
        /// </summary>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of the allocated sizes for child objects; or based on other considerations, such as a fixed container size.
        /// </returns>
        /// <param name="availableSize">The available size that this object can give to child objects. Infinity (<see cref="F:System.Double.PositiveInfinity"/>) can be specified as a value to indicate that the object will size to whatever content is available.</param>
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
            return desiredSize;
        }

        /// <summary>
        /// Provides the behavior for the Arrange pass of Silverlight layout. Classes can override this method to define their own Arrange pass behavior.
        /// </summary>
        /// <returns>
        /// The actual size that is used after the element is arranged in layout.
        /// </returns>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
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
            return actualSize;
        }
    }
}
