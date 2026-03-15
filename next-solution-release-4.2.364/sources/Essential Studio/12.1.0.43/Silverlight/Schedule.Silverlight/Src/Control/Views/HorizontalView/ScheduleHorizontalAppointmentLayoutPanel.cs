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
    /// <summary>
    /// Represents Schedule's HorizontalAppointmentLayoutPanel
    /// </summary>
#if SyncfusionFramework4_0 && !SILVERLIGHT
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    
    public sealed class ScheduleHorizontalAppointmentLayoutPanel : Panel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleHorizontalAppointmentLayoutPanel"/> class.
        /// </summary>
        public ScheduleHorizontalAppointmentLayoutPanel()
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
            var itemHeight = availableSize.Height;

            if (double.IsInfinity(availableSize.Width))
            {
                availableSize.Width = model.GetTimeSlotWidth();
            }
            if (double.IsInfinity(availableSize.Height))
            {
                availableSize.Height = model.GetTimeSlotHeight();
            }
            foreach (var selectedDate in model.SelectedDates)
            {
                var currDate = selectedDate;
                var nextDay = calendar.AddDays(currDate, 1);
                var appointments = model.GetDailyAppointments(currDate);
                foreach (var app in appointments)
                {
                    if (app == null) continue;
                    var appCtl = new ScheduleHorizontalAppointmentViewControl();
                    if (app.MultiDayAppointment)
                        appCtl = this.Children.OfType<ScheduleHorizontalAppointmentViewControl>().FirstOrDefault(a => app.MatchWithExists(a.DataContext as ScheduleAppointment, currDate) == true);
                    else
                        appCtl = this.Children.OfType<ScheduleHorizontalAppointmentViewControl>().FirstOrDefault(a => app.MatchWithExists(a.DataContext as ScheduleAppointment) == true);
                   // var appCtl = this.Children.OfType<ScheduleHorizontalAppointmentViewControl>().FirstOrDefault(a => a.DataContext == app);
                    if (appCtl != null)
                    {
                        if (appCtl.MousePosition != ResizePosition.None || appCtl.DragStatus == MousePointerType.Resize) continue;
                        DateTime startTime = app.StartTime >= currDate ? app.StartTime : currDate;
                        DateTime endTime = app.EndTime < nextDay ? app.EndTime : nextDay;
                        TimeSpan span = endTime - startTime;
                        double width = ((availableSize.Width / 24.0) * span.TotalHours);
                        width = (width >= 0) ? width : 0; //Height value should be positive
                        var intersected =
                            from a in appointments
                            where a!= null && a.IsIntersecting(app)
                            orderby a.StartTime
                            select a;
                        Size itemSize = new Size(width, itemHeight / (intersected.Count() + 1));
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

        private Dictionary<ScheduleHorizontalAppointmentViewControl, AppointmentPostionInfo> layoutCache = new Dictionary<ScheduleHorizontalAppointmentViewControl, AppointmentPostionInfo>();
        internal void Reset()
        {
            this.layoutCache.Clear();
        }

        internal void RemoveKey(ScheduleHorizontalAppointmentViewControl appCtl)
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
            var actualfinalSizeWidth = model.GetTimeSlotWidth();
            var finalWidth = finalSize.Width;
            finalWidth = (finalWidth > actualfinalSizeWidth && model.SelectedDates.Count == 1) ? actualfinalSizeWidth : finalWidth;
            //finalWidth = (finalWidth > actualfinalSizeWidth) ? actualfinalSizeWidth : finalWidth;
            double intervalCount = ScheduleHorizontalTimeLineHourControl.IntervalCount[(int)itemsContainer.Model.CurrentTimeInterval];
            var colCount = model.SelectedDates.Count;
            double hourWidth = (finalWidth / colCount) / 24.0;
            double intervalWidth = hourWidth / intervalCount;
            double interval = 60.0 / intervalCount;
            System.Globalization.Calendar calendar = CultureInfo.CurrentCulture.Calendar;
            var baseColIdx = 0;
            var colHeight = finalSize.Height;
            var currentcolWidth = finalWidth / colCount;
            foreach (var selectedDate in model.SelectedDates)
            {
                var currDate = selectedDate;
                var nextDay = calendar.AddDays(currDate, 1);
                var appointments = model.GetDailyAppointments(currDate).Where(app => app != null).ToList();
                foreach (var app in appointments)
                {
                    if (app == null) continue;
                    var appCtl = new ScheduleHorizontalAppointmentViewControl();
                    if (app.MultiDayAppointment)
                        appCtl = this.Children.OfType<ScheduleHorizontalAppointmentViewControl>().FirstOrDefault(a => app.MatchWithExists(a.DataContext as ScheduleAppointment, currDate) == true);
                    else
                        appCtl = this.Children.OfType<ScheduleHorizontalAppointmentViewControl>().FirstOrDefault(a => app.MatchWithExists(a.DataContext as ScheduleAppointment) == true);
                    //var appCtl = this.Children.OfType<ScheduleHorizontalAppointmentViewControl>().FirstOrDefault(a => app.MatchWithExists(a.DataContext as ScheduleAppointment) == true);
                    if (appCtl != null)
                    {
                        if (appCtl.MousePosition != ResizePosition.None || appCtl.DragStatus == MousePointerType.Resize) continue;
                        AppointmentPostionInfo info = null;
                        this.layoutCache.TryGetValue(appCtl, out info);

                        info = this.GetAppointmentVertPosition(app, appCtl, interval, hourWidth, intervalWidth, ref currDate, ref nextDay, baseColIdx);
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
                        info.IsSpanned = app.EndTime >= nextDay;

                        if (!this.layoutCache.ContainsKey(appCtl)) //to avoid same key adding multiple times in dictionary
                            this.layoutCache.Add(appCtl, info);
                        
                        double height = colHeight;
                        height -= ScheduleHorizontalAppointmentViewControl.AddAppointmentPadding;
                        if (info.IsIntersecting)
                        {
                            if (info.IntersectCount > 1) height /= model.GetInterSectedCntToAdjWid(appointments, app, info.IntersectCount);
                            else height /= info.IntersectCount;
                        }
                        var y = 0.00;

                        if (info.IntersectIndex > 0)
                        {
                            y = 0;
                            if (info.IntersectCount > 1) y = y + (height * (model.GetInterSectedCntToAdjWid(appointments, app, info.IntersectCount) - info.IntersectIndex));
                            else y = y + (height * info.IntersectIndex);
                        }
                        if (info.Width <= 3) continue;
                        var currentIndexOfDate = itemsContainer.Model.SelectedDates.IndexOf(selectedDate);
                        currentIndexOfDate = (currentIndexOfDate == -1) ? 0 : currentIndexOfDate;
                        var x = info.X + (currentIndexOfDate * currentcolWidth);
                        Rect rect = new Rect(x, y, info.Width, height);
                        appCtl.Arrange(rect);
                    }
                }
                baseColIdx += 1;
            }

            return finalSize;
        }

        internal AppointmentPostionInfo GetAppointmentVertPosition(ScheduleAppointment app, ScheduleHorizontalAppointmentViewControl appCtl, double interval, double hourWidth, double intervalWidth, ref DateTime currDate, ref DateTime nextDay, int column)
        {
            DateTime startTime = app.StartTime >= currDate ? app.StartTime : currDate;
            double x = GetIntervalWidth(startTime.TimeOfDay, interval, hourWidth, intervalWidth, false);
            TimeSpan endTimeOfDay = app.EndTime.TimeOfDay;

            if (app.EndTime >= nextDay)
            {
                endTimeOfDay = new TimeSpan(24, 0, 0);
            }

            double x2 = GetIntervalWidth(endTimeOfDay, interval, hourWidth, intervalWidth, true);

            return new AppointmentPostionInfo()
            {
                X = x,
                Width = x2 - x,
                Column = column,
            };
        }

        private static double GetIntervalWidth(TimeSpan timeSpan, double interval, double hourWidth, double intervalWidth, bool ceiling)
        {
            double d = timeSpan.Minutes / interval;
            double m = ceiling ? Math.Ceiling(d) : Math.Floor(d);

            return (hourWidth * 24.0 * timeSpan.Days) + (hourWidth * timeSpan.Hours) + (m * intervalWidth);
        }

        private static TimeSpan RoundTimeToInterval(TimeSpan timeSpan, double interval, bool ceiling)
        {
            double d = timeSpan.Minutes / interval;
            int minutes = (int)((ceiling ? Math.Ceiling(d) : Math.Floor(d)) * interval);

            return new TimeSpan(timeSpan.Days, timeSpan.Hours, minutes, 0);
        }
    }

    /// <summary>
    /// Contains Information for Appointment's Position.
    /// </summary>
    public class AppointmentPostionInfo
    {
        /// <summary>
        /// Gets or sets double value for X
        /// </summary>
        public double X { get; set; }
        /// <summary>
        /// Gets or sets double value for Y
        /// </summary>
        public double Y { get; set; }
        /// <summary>
        /// Gets or sets double value for Width
        /// </summary>
        public double Width { get; set; }
        /// <summary>
        /// Gets or sets double value for Height
        /// </summary>
        public double Height { get; set; }
        /// <summary>
        /// Gets or sets integer value for Column
        /// </summary>
        public int Column { get; set; }
        /// <summary>
        /// Gets or sets bool value for IsSpanned
        /// </summary>
        public bool IsSpanned { get; set; }
        /// <summary>
        /// Gets or sets the bool value for intersection
        /// </summary>
        public bool IsIntersecting { get { return this.IntersectCount > 0; } }
        /// <summary>
        /// Gets or sets the double value for intersect count
        /// </summary>
        public double IntersectCount { get; set; }
        /// <summary>
        /// Gets or sets the double value for intersect index
        /// </summary>
        public double IntersectIndex { get; set; }
    }
}
