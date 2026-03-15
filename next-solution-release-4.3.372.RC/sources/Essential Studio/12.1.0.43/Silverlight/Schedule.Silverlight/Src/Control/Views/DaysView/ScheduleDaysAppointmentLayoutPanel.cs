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
using System.Linq;
using System.Collections.Generic;
using System.Globalization;

namespace Syncfusion.Windows.Controls.Schedule
{
  
#if SyncfusionFramework4_0 && !SILVERLIGHT
    /// <summary>
    ///  Represents Schedule's Day appointment layout panel
    /// </summary>
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
   
    public sealed class ScheduleDaysAppointmentLayoutPanel : Panel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleDaysAppointmentLayoutPanel"/> class.
        /// </summary>
        public ScheduleDaysAppointmentLayoutPanel()
        {
           
        }


        /// <summary>
        /// Provides the behavior for the "measure" pass of Silverlight layout. Classes can override this method to define their own measure pass behavior.
        /// </summary>
        /// <param name="availableSize">The available size that this object can give to child objects. Infinity can be specified as a value to indicate that the object will size to whatever content is available.</param>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of child object allotted sizes.
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
            System.Globalization.Calendar calendar = CultureInfo.CurrentCulture.Calendar;
            double maxWidth = 0.0;
            double maxHeight = 0.0;
            var itemWidth = availableSize.Width / model.SelectedDates.Count;
            if (double.IsInfinity(availableSize.Height))
            {
                availableSize.Height = model.GetTimeSlotHeight();
            }

            foreach (var selectedDate in model.SelectedDates)
            {
                var currDate = selectedDate;
                var nextDay = calendar.AddDays(currDate, 1);
                var appointments = model.GetDailyAppointments(currDate).Where(app => app != null);
                foreach (var app in appointments)
                {
                    if (app == null) continue;
                    var appCtl = new ScheduleDaysAppointmentViewControl();
                    if(app.MultiDayAppointment)
                        appCtl = this.Children.OfType<ScheduleDaysAppointmentViewControl>().FirstOrDefault(a => app.MatchWithExists(a.DataContext as ScheduleAppointment,currDate) == true);
                    else
                    appCtl = this.Children.OfType<ScheduleDaysAppointmentViewControl>().FirstOrDefault(a => app.MatchWithExists(a.DataContext as ScheduleAppointment) == true);
                    if (appCtl != null)
                    {
                        if (appCtl.MousePosition != ResizePosition.None || appCtl.DragStatus == MousePointerType.Resize) continue;
                        DateTime startTime = app.StartTime >= currDate ? app.StartTime : currDate;
                        DateTime endTime = app.EndTime < nextDay ? app.EndTime : nextDay;
                        TimeSpan span = endTime - startTime;
                        double height = ((availableSize.Height / 24.0) * span.TotalHours) + ScheduleDaysAppointmentViewControl.ShadowDepth;
                        height = (height >= 0) ? height : 0; //Height value should be positive
                        var intersected =
                            from a in appointments
                            where a != null && a.IsIntersecting(app)
                            orderby a.StartTime
                            select a;
                        Size itemSize = new Size(itemWidth / (intersected.Count() + 1), height);
                        appCtl.Measure(itemSize);
                        maxWidth = Math.Max(maxWidth, appCtl.DesiredSize.Width);
                        maxHeight = Math.Max(maxHeight, appCtl.DesiredSize.Height);
                    }
                }
            }

            if (double.IsInfinity(availableSize.Width))
            {
                availableSize.Width = maxWidth;
            }

            return availableSize;
        }

        private Dictionary<ScheduleDaysAppointmentViewControl, AppointmentPostionInfo> layoutCache = new Dictionary<ScheduleDaysAppointmentViewControl, AppointmentPostionInfo>();
        internal void Reset()
        {
            this.layoutCache.Clear();
        }

        /// <summary>
        /// Removes the key.
        /// </summary>
        /// <param name="appCtl">The app CTL.</param>
        internal void RemoveKey(ScheduleDaysAppointmentViewControl appCtl)
        {
            if (this.layoutCache.ContainsKey(appCtl))
            {
                this.layoutCache.Remove(appCtl);
            }
        }

        /// <summary>
        /// Provides the behavior for the "arrange" pass of Silverlight layout. Classes can override this method to define their own arrange pass behavior.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.Children.Count <= 0)
            {
                return finalSize;
            }

            var itemsContainer = ((FrameworkElement)this.Children[0]).FindParentElementOfType<IScheduleCalendarViewModelHost>();
            if (itemsContainer == null)
            {
                return Size.Empty;
            }
            var model = itemsContainer.Model;
            var actualfinalSizeHeight = model.GetTimeSlotHeight();
            var finalHeight = finalSize.Height;
            finalHeight = (finalHeight > actualfinalSizeHeight) ? actualfinalSizeHeight : finalHeight;
            double intervalCount = ScheduleTimeLineHourControl.IntervalCount[(int)itemsContainer.Model.CurrentTimeInterval] * 2;
            double hourHeight = finalHeight / 24.0;
            double intervalHeight = hourHeight / intervalCount;
            double interval = 60.0 / intervalCount;
            System.Globalization.Calendar calendar = CultureInfo.CurrentCulture.Calendar;
            var baseColIdx = 0;
            var colCount = model.SelectedDates.Count;
            var colWidth = finalSize.Width / colCount;
            foreach (var selectedDate in model.SelectedDates)
            {
                var currDate = selectedDate;
                var nextDay = calendar.AddDays(currDate, 1);
                var appointments = model.GetDailyAppointments(currDate).Where(app => app != null).ToList();
                foreach (var app in appointments)
                {
                    if (app == null) continue;
                    var appCtl = new ScheduleDaysAppointmentViewControl();
                    if (app.MultiDayAppointment)
                        appCtl = this.Children.OfType<ScheduleDaysAppointmentViewControl>().FirstOrDefault(a => app.MatchWithExists(a.DataContext as ScheduleAppointment, currDate) == true);
                    else
                        appCtl = this.Children.OfType<ScheduleDaysAppointmentViewControl>().FirstOrDefault(a => app.MatchWithExists(a.DataContext as ScheduleAppointment) == true);
                    
                    if (appCtl != null)
                    {
                        if (appCtl.MousePosition != ResizePosition.None || appCtl.DragStatus == MousePointerType.Resize) continue;
                        AppointmentPostionInfo info = null;
                        this.layoutCache.TryGetValue(appCtl, out info);

                        info = this.GetAppointmentVertPosition(app, appCtl, interval, hourHeight, intervalHeight, ref currDate, ref nextDay, baseColIdx);
                        var intersected = (from a in appointments
                                           where a != null && a.IsIntersecting(app)
                                           orderby a.StartTime
                                           select a).ToList();
                        if (intersected.Count > 0)
                        {
                            // add the current appointment in intersected to get the InteresectedIndexedList
                            intersected.Add(app);

                            info.IntersectCount = model.GetInterSectedCountValue(appointments, app);
                            info.IntersectIndex = model.GetInterSectedIndexValue(appointments, intersected, app);
                        }
                        //info.IsSpanned = app.EndTime >= nextDay;

                        if (!this.layoutCache.ContainsKey(appCtl)) //to avoid same key adding multiple times in dictionary
                            this.layoutCache.Add(appCtl, info);

                        double width = info.IsSpanned ? colWidth * (colCount - info.Column) : colWidth;
                        width -= ScheduleDaysAppointmentViewControl.AddAppointmentPadding;
                        if (info.IsIntersecting)
                        {
                            if (info.IntersectCount > 1) width /= model.GetInterSectedCntToAdjWid(appointments, app, info.IntersectCount);
                            else width /= info.IntersectCount;
                        }
                        var x = colWidth * info.Column;
                        if (info.IntersectIndex > 0)
                        {
                            if (info.IntersectCount > 1) x = x + (width * (model.GetInterSectedCntToAdjWid(appointments, app, info.IntersectCount) - info.IntersectIndex));
                            else x = x + (width * info.IntersectIndex);
                        }
                        if (width <= 3) continue;
                        Rect rect = new Rect(x, info.Y, width + 3, info.Height);
                        appCtl.Arrange(rect);
                    }
                }
                baseColIdx += 1;
            }

            return finalSize;
        }

        private AppointmentPostionInfo GetAppointmentVertPosition(ScheduleAppointment app, ScheduleDaysAppointmentViewControl appCtl, double interval, double hourHeight, double intervalHeight, ref DateTime currDate, ref DateTime nextDay, int column)
        {
            DateTime startTime = app.StartTime >= currDate ? app.StartTime : currDate;
            double y = GetIntervalHeight(startTime.TimeOfDay, interval, hourHeight, intervalHeight, false);
            TimeSpan endTimeOfDay = app.EndTime.TimeOfDay;

            if (app.EndTime >= nextDay)
            {
                endTimeOfDay = new TimeSpan(24, 0, 0);
            }

            double y2 = GetIntervalHeight(endTimeOfDay, interval, hourHeight, intervalHeight, true);

            return new AppointmentPostionInfo()
            {
                Y = y,
                Height = y2 - y + ScheduleDaysAppointmentViewControl.ShadowDepth,
                Column = column,
            };
        }

        private static double GetIntervalHeight(TimeSpan timeSpan, double interval, double hourHeight, double intervalHeight, bool ceiling)
        {
            double d = timeSpan.Minutes / interval;
            double m = ceiling ? Math.Ceiling(d) : Math.Floor(d);

            return (hourHeight * 24.0 * timeSpan.Days) + (hourHeight * timeSpan.Hours) + (m * intervalHeight);
        }

        private static TimeSpan RoundTimeToInterval(TimeSpan timeSpan, double interval, bool ceiling)
        {
            double d = timeSpan.Minutes / interval;
            int minutes = (int)((ceiling ? Math.Ceiling(d) : Math.Floor(d)) * interval);

            return new TimeSpan(timeSpan.Days, timeSpan.Hours, minutes, 0);
        }
    }
}
