#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Schedule
{
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
    using System.Linq;
    using System.Collections.Generic;

#if SyncfusionFramework4_0 && !SILVERLIGHT
    /// <summary>
    ///  Class that hold a panel for the maximum height of the schedule
    /// </summary>
    [System.ComponentModel.DesignTimeVisible(false)]
#endif

    public class MaxHeightPanel : Panel
    {
        /// <summary>
        /// When overridden in a derived class, measures the size in layout required for
        /// child elements and determines a size for the <see
        /// cref="T:System.Windows.FrameworkElement"/>-derived class.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to
        /// child elements. Infinity can be specified as a value to indicate that the
        /// element will size to whatever content is available.</param>
        /// <returns>
        /// The size that this element determines it needs during layout, based on its
        /// calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.Children.Count == 0)
            {
                return Size.Empty;
            }

            var totalHeight = 0d;
            foreach (UIElement child in this.Children)
            {
                var appControl = child as ScheduleAllDaysAppointmentItemsControl;
                var appControlItemsCount = 0;
                if (appControl != null)
                {
                    appControlItemsCount = appControl.Items.Count;
                    child.Measure(availableSize);
                    totalHeight += child.DesiredSize.Height + (appControlItemsCount * 3);
                    continue;
                }
                child.Measure(availableSize);
                totalHeight += child.DesiredSize.Height;           
            }

            // Adding 20 to add extra padding to the overall height
            //availableSize.Height = totalHeight > 0 ? totalHeight + 20 : availableSize.Height;
            availableSize.Height = totalHeight > 0 ? totalHeight + 20 : 0;
            if (double.IsInfinity(availableSize.Width))
            {
                double maxwidth = 0;
                foreach (UIElement child in this.Children)
                {
                    child.Measure(availableSize);
                    maxwidth += child.DesiredSize.Width;
                }
                availableSize.Width = maxwidth;
                // availableSize.Width = SystemParameters.PrimaryScreenWidth;
            }
            return availableSize;
        }

        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a
        /// size for a <see cref="T:System.Windows.FrameworkElement"/> derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element
        /// should use to arrange itself and its children.</param>
        /// <returns>
        /// The actual size used.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.Children.Count == 0)
            {
                return Size.Empty;
            }

            foreach (UIElement child in this.Children)
            {
                if (child.DesiredSize.Width <= 0 || child.DesiredSize.Height <= 0) continue;
                child.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
            }
            return finalSize;
        }
    }
}
