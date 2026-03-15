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
using System.Windows.Controls;
using Syncfusion.Windows.Controls.Gantt.Schedule;
using System.Windows;

namespace Syncfusion.Windows.Controls.Gantt.Schedule
{
    /// <summary>
    /// Call that represents the panel that will arrange the schedule cells horizontally.
    /// </summary>
    public class GanttScheduleRowPanel : Panel
    {
        /// <summary>
        /// Gets or sets the parent control.
        /// </summary>
        /// <value>The parent control.</value>
        internal GanttScheduleRow ParentControl { get; set; }

        /// <summary>
        /// Provides the behavior for the Measure pass of Silverlight layout. Classes can override this method to define their own Measure pass behavior.
        /// </summary>
        /// <param name="availableSize">The available size that this object can give to child objects. Infinity (<see cref="F:System.Double.PositiveInfinity"/>) can be specified as a value to indicate that the object will size to whatever content is available.</param>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of the allocated sizes for child objects; or based on other considerations, such as a fixed container size.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            for (int i = 0; i < this.Children.Count; i++)
            {
                GanttScheduleCell child = (GanttScheduleCell)this.Children[i];

                // Measuring the child element
                child.Measure(new Size(child.Width, this.MinHeight));
            }

            return base.MeasureOverride(availableSize);
        }

        /// <summary>
        /// Arranges the override.
        /// </summary>
        /// <param name="arrangeBounds">The arrange bounds.</param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            // Getting the inital location from the parten control.
            double location = this.ParentControl.HorizontalOffest;

            for (int i = 0; i < this.Children.Count; i++)
            {
                UIElement child = this.Children[i];

                //// As of now the row height is fixed so we have calculated the y position by itemIndex * RowHeight. 
                //// In future while supporting row resizing we have to change this implementation
                child.Arrange(new Rect(location, 0, (child as GanttScheduleCell).Width, 20));

                location += (child as GanttScheduleCell).Width;
            }
            return base.ArrangeOverride(arrangeBounds);
        }
    }
}
