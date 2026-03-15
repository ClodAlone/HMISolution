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
using System.Windows;
using System.Windows.Controls;
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    /// <summary>
    /// Represents a panel for arranging non working days in day view.
    /// </summary>
    public class ScheduleNonWorkingDayPanel : Panel
    {
        #region Overrides

        #region ArrangeOverride

        protected override Size ArrangeOverride(Size finalSize)
        {
            double i = 0;
            double count = 0;
            double gridheight = 0;
#if !WINRT
            if (Children.Count > 0)
            {
#endif
                var schedule = ((FrameworkElement)Children[0]).FindParentElementOfType<SfSchedule>();
                var scheduledaysview = ((FrameworkElement)Children[0]).FindParentElementOfType<ScheduleDaysView>();
                double starthourHight = 0;
                double endhourHight = 0;
                int interval = 0;
                double width = finalSize.Width;
                bool Isresourceenabled = false;
                double resourcecount = 1;
                if (schedule != null)
                {
                    Isresourceenabled = schedule.ScheduleResourceType != null && schedule.ScheduleResourceType.ResourceCollection.Count > 0;
                    if (Isresourceenabled)
                    {
                        resourcecount = schedule.CalculateLeafCount(schedule.ScheduleResourceType);
                    }                   
                    if (scheduledaysview != null)
                    {
                        interval = ScheduleTimeLineItemsControl.IntervalCount[(int)scheduledaysview.TimeInterval] * 24;
                        count = scheduledaysview.SelectedDates.Count;
                    }
                    if (Isresourceenabled)
                    {
                        if (schedule.DayHeaderOrder == DayHeaderOrder.OrderByResource)
                        {
                            count = count * resourcecount;
                        }
                    }
                    starthourHight = schedule.WorkStartHour * (interval / 24) * (finalSize.Height / interval);
                    endhourHight = schedule.WorkEndHour * (interval / 24) * (finalSize.Height / interval);
                }
                double nonworkingdaygridcount = 0;
                double resourceposition = 1;
                var sorteddates = new List<DateTime>();
                if (scheduledaysview != null)
                {
                    sorteddates = scheduledaysview.SelectedDates.OrderBy(s => s).ToList();
                }
                foreach (UIElement item in Children)
                {
                    var grid = item as Grid;
                    if (grid != null && (grid.DataContext is string))
                    {

                        //Here 2 reperesents the non working hours grid.
                        if (Children.Count > 2 & Isresourceenabled)
                        {
                            if (nonworkingdaygridcount.Equals(schedule.NonWorkingDateCollection.Count))
                            {
                                resourceposition++;
                                nonworkingdaygridcount = 0;
                            }
                        }
                        if (scheduledaysview != null)
                        {
                            for (int k = 0; k < scheduledaysview.SelectedDates.Count; k++)
                            {
                                if (schedule != null && sorteddates[k].Date == schedule.SelectedDate && scheduledaysview.RectVisibility == Visibility.Visible)
                                {
                                    double selectionRectHeight = finalSize.Height / interval;
                                    double selectionRectWidth = width / (scheduledaysview.SelectedDates.Count * resourcecount);
                                    if (scheduledaysview.RectHeight != selectionRectHeight || scheduledaysview.RectWidth != selectionRectWidth)
                                    {
                                        scheduledaysview.RectVisibility = Visibility.Collapsed;
                                        scheduledaysview.RectHeight = selectionRectHeight;
                                        scheduledaysview.RectWidth = selectionRectWidth;
                                        scheduledaysview.RectVisibility = Visibility.Visible;
                                    }
                                }
                                if (sorteddates[k].DayOfWeek.ToString().Equals((item as Grid).DataContext.ToString()))
                                {
                                    i = k;
                                }
                            }
                        }
                        if (schedule != null)
                            item.Arrange(new Rect(((i * (finalSize.Width / count)) + ((resourceposition - 1) * ((finalSize.Width / count) * schedule.SelectedDates.Count))), 0, finalSize.Width / count, finalSize.Height));
                        nonworkingdaygridcount++;
                    }
                    else
                    {
                        if (scheduledaysview != null)
                            for (int k = 0; k < scheduledaysview.SelectedDates.Count; k++)
                            {
                                if (schedule != null && sorteddates[k].Date == schedule.SelectedDate && scheduledaysview.RectVisibility == Visibility.Visible)
                                {
                                    double selectionRectHeight = finalSize.Height / interval;
                                    double selectionRectWidth = finalSize.Width / (scheduledaysview.SelectedDates.Count * resourcecount);
                                    if (scheduledaysview.RectHeight != selectionRectHeight || scheduledaysview.RectWidth != selectionRectWidth)
                                    {
                                        scheduledaysview.RectVisibility = Visibility.Collapsed;
                                        scheduledaysview.RectHeight = selectionRectHeight;
                                        scheduledaysview.RectWidth = selectionRectWidth;
                                        //scheduledaysview.RectXPosition = k * scheduledaysview.RectWidth * resourcecount;
                                        scheduledaysview.RectVisibility = Visibility.Visible;
                                    }
                                }
                            }

                        item.Arrange(new Rect(0, gridheight, finalSize.Width, starthourHight));
                        gridheight = endhourHight;
                        starthourHight = finalSize.Height - gridheight;


                    }
                }
#if !WINRT
            }
#endif
            return base.ArrangeOverride(finalSize);
        }

        #endregion

        #endregion
    }
}
