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
using System.Globalization;
using System.Linq;
using System.Collections.Generic;

namespace Syncfusion.Windows.Controls.Schedule
{
#if SyncfusionFramework4_0 && !SILVERLIGHT
    /// <summary>
    ///  Represents Schedule's AllDayAppointment Layout panel.
    /// </summary>
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    
    public class ScheduleAllDaysAppointmentLayoutPanel : Panel
    {
        /// <summary>
        ///  all day appointment constant height 30
        /// </summary>
        public const double AllDayAppiontmentHeight = 30d;
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

            var itemsContainer = ((FrameworkElement)this.Children[0]).FindParentElementOfType<IScheduleCalendarViewModelHost>();
            if (itemsContainer == null)
            {
                return Size.Empty;
            }

            var model = itemsContainer.Model;
            var calendar = CultureInfo.CurrentCulture.Calendar;
            var itemWidth = availableSize.Width / model.SelectedDates.Count;
            double maxWidth = 0.0;
            double totalHeight = 0.0;
            foreach (var selectedDate in model.SelectedDates)
            {
                var currDate = selectedDate;
                var nextDay = calendar.AddDays(currDate, 1);
                var appointments = model.GetAllDayOrSpannedAppointments(currDate, nextDay);
                var counterHeight = 0d;
                foreach (var app in appointments)
                {
                    if (app == null) continue;
                    var appCtl = this.Children.OfType<ScheduleDaysAppointmentViewControl>().FirstOrDefault(a => app.MatchWithExists(a.DataContext as ScheduleAppointment) == true);
                    if (appCtl != null)
                    {
                        double height = AllDayAppiontmentHeight;
                        Size itemSize = new Size(itemWidth, height);
                        appCtl.Measure(itemSize);
                        maxWidth = Math.Max(maxWidth, appCtl.DesiredSize.Width);
                        counterHeight += appCtl.DesiredSize.Height;
                    }
                }

                totalHeight = Math.Max(totalHeight, counterHeight);
            }

            if (double.IsInfinity(availableSize.Width))
            {
                availableSize.Width = maxWidth;
            }

            if (double.IsInfinity(availableSize.Height))
            {
                availableSize.Height = totalHeight;
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

            var itemsContainer = ((FrameworkElement)this.Children[0]).FindParentElementOfType<IScheduleCalendarViewModelHost>();
            if (itemsContainer == null)
            {
                return Size.Empty;
            }

            var model = itemsContainer.Model;
            var calendar = CultureInfo.CurrentCulture.Calendar;
            var itemWidth = finalSize.Width / model.SelectedDates.Count;
            var colIdx = 0;
            List<bool[]> PositionMatrix = new List<bool[]>() { new bool[model.SelectedDates.Count] };                
            
            foreach (var selectedDate in model.SelectedDates)
            {
                var currDate = selectedDate;
                var nextDay = calendar.AddDays(currDate, 1);
                var appointments = model.GetAllDayOrSpannedAppointments(currDate, nextDay);
                var rowIdx = 0;
                var headerTextHeight = 20d;
                foreach (var app in appointments)
                {
                    if (app == null) continue;
                    var appCtl = this.Children.OfType<ScheduleDaysAppointmentViewControl>().FirstOrDefault(a => app.MatchWithExists(a.DataContext as ScheduleAppointment) == true);
                    if (appCtl != null)
                    {
                        double height = AllDayAppiontmentHeight;
                        var info = this.GetPositionInfo(appCtl, currDate, nextDay, model);
                        bool rowAvailable = false;
                        while(!rowAvailable)
                        {
                            rowAvailable = true;

                        for(int i = colIdx; i < (colIdx + info.Columns); i++)
                        {
                            if ((PositionMatrix.Count) <= rowIdx) { PositionMatrix.Add(new bool[model.SelectedDates.Count]); }
                            if(PositionMatrix[rowIdx][i])
                            {                                
                                rowAvailable = false;
                                rowIdx = rowIdx +1;
                                break;
                            }
                        }
                        }
                        var itemSize = new Size(info.IsSpanned ? itemWidth * info.Columns : itemWidth, height);
                        var rect = new Rect(itemWidth * colIdx, (itemSize.Height * rowIdx) + headerTextHeight , itemSize.Width , itemSize.Height);
                        for (int i = colIdx; i < (colIdx + info.Columns); i++)
                        {
                            PositionMatrix[rowIdx][i] = true;
                        }                        
                        appCtl.Arrange(rect);
                    }
                    rowIdx += 1;                  
                }              
                colIdx += 1;
                
                
            }

            return finalSize;
        }

        private AppointmentPositionInfo GetPositionInfo(ScheduleDaysAppointmentViewControl appCtl, DateTime currDate, DateTime nextDay, ScheduleCalendarViewModel model)
        {
            int columns = 1;
            var app = appCtl.ScheduleAppointment;
            var isSpanned = app.EndTime >= nextDay;
            var endDay = app.EndTime.Day;
            var selectedEndDate = model.SelectedDates[model.SelectedDates.Count - 1];
            endDay = (app.EndTime <= selectedEndDate) ? endDay : selectedEndDate.Day;
            if (isSpanned)
            {
                /*if (app.StartTime.Day == currDate.Day)
                {
                    for (int i = currDate.Day;i < app.EndTime.Day;i++)
                    {
                        columns += 1;
                    }
                }*/

                var checkforDate = from res in model.SelectedDates
                                   where app.StartTime.Date <= res.Date && app.EndTime.Date >= res.Date
                                   select res;
                if (checkforDate != null && checkforDate.Count() > 0)
                {
                    columns = checkforDate.Count();
                }
            }

            return new AppointmentPositionInfo() { IsSpanned = isSpanned, Columns = columns };
        }

        private class AppointmentPositionInfo
        {
            public bool IsSpanned { get; set; }
            public int Columns { get; set; }
        }
    }
}