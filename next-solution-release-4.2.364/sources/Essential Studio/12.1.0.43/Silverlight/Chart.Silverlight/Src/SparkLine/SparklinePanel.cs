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

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class implementation for sparkLinePanel
    /// </summary>
    public class SparklinePanel : Panel
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
            if (this.Children.Count == 0)
            {
                return Size.Empty;
            }

            //var totalHeight = 0d;
            foreach (UIElement child in this.Children)
            {
                //var appControl = child as ScheduleAllDaysAppointmentItemsControl;
                //var appControlItemsCount = 0;
                //if (appControl != null)
                //{
                //    appControlItemsCount = appControl.Items.Count;
                (child as ContentPresenter).Height = availableSize.Height;
                (child as ContentPresenter).Width = availableSize.Width;
                child.Measure(availableSize);
                  //  totalHeight += child.DesiredSize.Height ;
                //    continue;
                //}
                //this.DesiredSize = new Size(ava

                //child.Measure(availableSize);
                //totalHeight += child.DesiredSize.Height;
            }

            // Adding 20 to add extra padding to the overall height
            //availableSize.Height = totalHeight > 0 ? totalHeight + 20 : availableSize.Height;
            //availableSize.Height = totalHeight > 0 ? totalHeight + 20 : 0;

            return availableSize;
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
            if (this.Children.Count == 0)
            {
                return Size.Empty;
            }

            foreach (UIElement child in this.Children)
            {
                ContentPresenter path = child as ContentPresenter;
                if (child.DesiredSize.Width <= 0 || child.DesiredSize.Height <= 0) continue;
                {
                    //path.Height = finalSize.Height;
                    //path.Width = finalSize.Width;

                    child.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
                }
            }
            return finalSize;
        }
    }
}
