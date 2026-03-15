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
#if WINRT
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using System.Windows.Controls;
using System.Windows;
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    /// <summary>
    /// Represents a panel for arranging non working days in timeline view.
    /// </summary>
    public class ScheduleTimeLineNonWorkingDayPanel : Panel
    {
        #region Override Methods

        #region ArrangeOverride

        protected override Size ArrangeOverride(Size finalSize)
        {
            var schedule = this.FindParentElementOfType<SfSchedule>();
            var scheduledaysview = this.FindParentElementOfType<ScheduleTimeLineView>();
            double widthofpanel = (scheduledaysview.FindElementOfType<ScheduleTimelineTimeSlotItemsControl>().Width / 24) / scheduledaysview.SelectedDates.Count;
            double heightofpanel = scheduledaysview.FindElementOfType<ScheduleTimelineTimeSlotItemsControl>().ActualHeight;
            double starthourHight = schedule.WorkStartHour * widthofpanel;
            double endhourHight = (24 - schedule.WorkEndHour) * widthofpanel;
            double x = 0;
            const double y = 0;
            double index = 0;
            double widthofthegrid = starthourHight;
            List<DateTime> sorteddates = scheduledaysview.SelectedDates.OrderBy(s => s).ToList();
            foreach (UIElement item in Children)
            {
                var grid = item as Grid;
                if (grid != null && grid.DataContext == null)
                {
                    item.Arrange(new Rect(x, y, widthofthegrid, heightofpanel));
                    if ((index % 2).Equals(0))
                    {
                        x += schedule.WorkEndHour * widthofpanel;
                        widthofthegrid = endhourHight;
                    }
                    else
                    {
                        x += widthofthegrid;
                        widthofthegrid = starthourHight;
                    }
                    index++;
                }
                else
                {
                    var grid1 = item as Grid;
                    double nonworkingindex = sorteddates.IndexOf(sorteddates.FirstOrDefault(m => grid1 != null && m.DayOfWeek.ToString().Equals(grid1.DataContext.ToString())));
                    item.Arrange(new Rect(nonworkingindex * widthofpanel * 24, 0, widthofpanel * 24, heightofpanel));
                }
            }


            return base.ArrangeOverride(finalSize);
        }

        #endregion

        #endregion
    }
}

