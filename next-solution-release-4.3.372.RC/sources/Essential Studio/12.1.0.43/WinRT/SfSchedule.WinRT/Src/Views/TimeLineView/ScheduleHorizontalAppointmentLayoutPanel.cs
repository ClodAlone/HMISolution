#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
#if WINRT
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#else
using System.Windows.Controls;
using System.Windows;
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    /// <summary>
    /// Represents a layout panel for arranging timeline view appointments.
    /// </summary>
    public sealed class ScheduleHorizontalAppointmentLayoutPanel : Panel
    {
        #region Constructor

        public ScheduleHorizontalAppointmentLayoutPanel()
        {
#if WINRT
            ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateInertia;
            ManipulationDelta += ScheduleHorizontalAppointmentLayoutPanel_ManipulationDelta;
#endif
        }

        #endregion

        #region Private Fields

        private readonly Dictionary<ScheduleHorizontalAppointmentViewControl, AppointmentPostionInfo> layoutCache = new Dictionary<ScheduleHorizontalAppointmentViewControl, AppointmentPostionInfo>();

        #endregion

        #region Events

        void app_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == "InternalStartTime" || e.PropertyName == "InternalEndTime" || e.PropertyName == "AppointmentBackground" || e.PropertyName == "Subject" || e.PropertyName == "ReadOnly" || e.PropertyName == "IsRecursive"))
            {
                InvalidateArrange();
            }
        }

#if WINRT
        void ScheduleHorizontalAppointmentLayoutPanel_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (e.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Touch)
                return;
            SfSchedule schedule = this.FindParentElementOfType<SfSchedule>();
            ScheduleTimeLineView timelineview = this.FindParentElementOfType<ScheduleTimeLineView>();
            if (!schedule.ScrollManipulationCompleted)
            {

                double delta = timelineview.timelinescroll.HorizontalOffset + (e.Delta.Translation.X * -1);
                if (delta <= timelineview.timelinescroll.ScrollableWidth)
                {
#if SyncfusionFramework4_5_11
                    timelineview.timelinescroll.ChangeView(delta, null, null);
#else
                    timelineview.timelinescroll.ScrollToHorizontalOffset(delta);
#endif
                    if (delta < 0)
                    {
                        if (e.IsInertial)
                        {
                            //e.Complete();
                        }
                        else
                        {
                            schedule.ScrollManipulationCompleted = true;
                        }
                    }
                }
                else
                {
                    if (e.IsInertial)
                    {
                        // e.Complete();
                    }
                    else
                    {
                        schedule.ScrollManipulationCompleted = true;
                    }
                }

            }
        }
#endif

        #endregion

        #region Override Methods

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Children.Count == 0)
            {
                return new Size();
            }
            var schedule = ((FrameworkElement)Children[0]).FindParentElementOfType<SfSchedule>();
            var timelineview = ((FrameworkElement)Children[0]).FindParentElementOfType<ScheduleTimeLineView>();
            if (schedule == null)
            {
                return Size.Empty;
            }
            double maxWidth = 0.0;
            double maxHeight = 0.0;
            var itemHeight = availableSize.Height;

            if (double.IsInfinity(availableSize.Width))
            {
                availableSize.Width = timelineview.GetTimeSlotWidth(timelineview.SelectedDates.Count);
            }
            if (double.IsInfinity(availableSize.Height))
            {
                availableSize.Height = schedule.GetTimeSlotHeight();
            }
            foreach (var selectedDate in timelineview.SelectedDates)
            {
                var currDate = selectedDate.Date;
                string resourcetype = string.Empty;
                if (schedule.ScheduleResourceType != null)
                {
                    resourcetype = schedule.Resource;
                }
                if (schedule.ProxyAppointments.ContainsKey(currDate))
                {
                    var appointments = schedule.ProxyAppointments[currDate];
                    foreach (var appCtl in Children)
                    {
                        var scheduleHorizontalAppointmentViewControl = appCtl as ScheduleHorizontalAppointmentViewControl;
                        if (scheduleHorizontalAppointmentViewControl != null)
                        {
                            var app = scheduleHorizontalAppointmentViewControl.DataContext as ScheduleAppointment;
                            if (app != null && app.InternalStartTime.Date == selectedDate)
                            {
                                {
                                    DateTime startTime = app.StartTime >= currDate ? app.StartTime : currDate;
                                    DateTime endTime = app.EndTime;//< nextDay ? app.EndTime : nextDay;
                                    TimeSpan span = endTime - startTime;
                                    double width = ((availableSize.Width / 24.0) * span.TotalHours);
                                    width = (width >= 0) ? width : 0; //Height value should be positive
                                    var intersected =
                                        from a in appointments
                                        where a != null && a.IsIntersecting(app, resourcetype)
                                        orderby a.StartTime
                                        select a;
                                    var itemSize = new Size(width, itemHeight / (intersected.Count() + 1));
                                    scheduleHorizontalAppointmentViewControl.Measure(itemSize);
                                    maxWidth = Math.Max(maxWidth, scheduleHorizontalAppointmentViewControl.DesiredSize.Width);
                                    maxHeight = Math.Max(maxHeight, scheduleHorizontalAppointmentViewControl.DesiredSize.Height);
                                }
                            }
                        }
                    }
                }
            }

            if (double.IsInfinity(availableSize.Width))
            {
                availableSize.Width = maxWidth;
            }

            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Children.Count <= 0)
            {
                return finalSize;
            }

            var schedule = ((FrameworkElement)Children[0]).FindParentElementOfType<SfSchedule>();
            var timelineview = ((FrameworkElement)Children[0]).FindParentElementOfType<ScheduleTimeLineView>();
            if (schedule == null)
            {
                return Size.Empty;
            }

            var actualfinalSizeWidth = timelineview.GetTimeSlotWidth(timelineview.SelectedDates.Count);
            var finalWidth = finalSize.Width;
            finalWidth = (finalWidth > actualfinalSizeWidth && timelineview.SelectedDates.Count == 1 && !schedule.EnableAutoFormat) ? actualfinalSizeWidth : finalWidth;
            bool isresourcedefined = false;
            double intervalCount = ScheduleTimeLineItemsControl.IntervalCount[(int)timelineview.TimeInterval];
            var colCount = timelineview.SelectedDates.Count;
            double hourWidth = (finalWidth / colCount) / (ScheduleTimeLineItemsControl.MaxValue - ScheduleTimeLineItemsControl.MinValue);
            double intervalWidth = hourWidth / intervalCount;
            double interval = 60.0 / intervalCount;
            var calendar = CultureInfo.CurrentCulture.Calendar;
            var baseColIdx = 0;
            var colHeight = finalSize.Height;
            var currentcolWidth = finalWidth / colCount;
            string currentresource = string.Empty;
            string resourcetype = string.Empty;
            if (schedule.ScheduleResourceType != null)
            {
                resourcetype = schedule.Resource;
            }
            if (schedule.Resource != string.Empty && schedule.ScheduleResourceType != null && schedule.ScheduleResourceType.ResourceCollection.Count > 0)
            {
                isresourcedefined = true;
                if (Children.Count > 0)
                {
                    var scheduleHorizontalAppointmentViewControl = Children[0] as ScheduleHorizontalAppointmentViewControl;
                    if (scheduleHorizontalAppointmentViewControl != null)
                    {
                        var scheduleAppointment = scheduleHorizontalAppointmentViewControl.DataContext as ScheduleAppointment;
                        if (scheduleAppointment != null)
                        {
                            var firstOrDefault = scheduleAppointment.ResourceCollection.FirstOrDefault(res => res.TypeName == schedule.Resource);
                            if (firstOrDefault != null)
                                currentresource = firstOrDefault.ResourceName;
                        }
                    }
                }
            }
            var appcoll = new List<ScheduleAppointment>();
            if (isresourcedefined)
            {
                foreach (var selDate in timelineview.SelectedDates)
                {
                    if (schedule.ProxyAppointments.ContainsKey(selDate))
                    {
                        var appointments = schedule.ProxyAppointments[selDate];
                        foreach (ScheduleAppointment app in appointments)
                        {
                            Resource Res = app.ResourceCollection.FirstOrDefault(res => res.TypeName == schedule.Resource);
                            if (!appcoll.Contains(app) && Res != null && Res.ResourceName == currentresource)
                                appcoll.Add(app);
                        }
                    }
                }
            }
            else
            {
                foreach (var selDate in timelineview.SelectedDates)
                {
                    if (schedule.ProxyAppointments.ContainsKey(selDate))
                    {
                        var appointments = schedule.ProxyAppointments[selDate];
                        foreach (ScheduleAppointment app in appointments)
                        {
                            if (!appcoll.Contains(app))
                                appcoll.Add(app);
                        }
                    }
                }
            }
            var renderedapp = new List<ScheduleAppointment>();
            foreach (var selectedDate in timelineview.SelectedDates)
            {
                var currDate = selectedDate;
                var nextDay = calendar.AddDays(currDate, 1);


                foreach (var appCtl in Children)
                {
                    var appointmentViewControl = (appCtl as ScheduleHorizontalAppointmentViewControl);
                    var app = appointmentViewControl.DataContext as ScheduleAppointment;
                    if ((app.InternalStartTime.Date == selectedDate || (app.InternalStartTime.Date < selectedDate && app.InternalEndTime.Date >= selectedDate)) && !renderedapp.Contains(app))
                    {
                        if (appointmentViewControl != null)
                        {
                            AppointmentPostionInfo info;
                            layoutCache.TryGetValue(appointmentViewControl, out info);

                            info = GetAppointmentVertPosition(app, appointmentViewControl, interval, hourWidth, intervalWidth, timelineview.SelectedDates, finalWidth, ref currDate, ref nextDay, baseColIdx);
                            var intersected = (from a in appcoll
                                               where a != null && a.IsIntersecting(app, resourcetype)
                                               orderby a.InternalStartTime
                                               select a).ToList();
                            if (intersected.Count > 0)
                            {
                                // add the current appointment in intersected to get the InteresectedIndexedList
                                intersected.Add(app);

                                info.IntersectCount = schedule.GetInterSectedCountValue(appcoll, app);
                                info.IntersectIndex = schedule.GetInterSectedIndexValue(appcoll, intersected, app);
                            }
                            info.IsSpanned = app.EndTime >= nextDay;

                            if (!layoutCache.ContainsKey(appointmentViewControl)) //to avoid same key adding multiple times in dictionary
                                layoutCache.Add(appointmentViewControl, info);

                            double height = colHeight;
                            if (info.IsIntersecting)
                            {
                                height /= (info.IntersectCount);
                            }
                            var y = 0.00;

                            if (info.IntersectIndex > 0)
                            {
                                y = y + (height * info.IntersectIndex);
                            }
                            if (info.Width <= 3) continue;
                            if (double.IsNaN(info.X))
                                info.X = 0;
                            if (double.IsNaN(info.Width))
                                info.Width = 0;
                            if (info.X < 0)
                            {
                                info.Width += info.X;
                                info.X = info.Width > 0 ? 0 : info.Width;
                                info.Width = info.Width < 0 ? 0 : info.Width;
                            }
                            if (info.X > finalSize.Width)
                            {
                                info.X = finalSize.Width;
                            }
                            else if (info.X + info.Width > finalSize.Width)
                            {
                                info.Width = finalSize.Width - info.X;
                            }
                            var currentIndexOfDate = timelineview.SelectedDates.IndexOf(selectedDate);
                            currentIndexOfDate = (currentIndexOfDate == -1) ? 0 : currentIndexOfDate;
                            var x = info.X + (currentIndexOfDate * currentcolWidth);
                            //2 is used to show difference between two appointments when placed near.
                            var rect = new Rect(x, y + 2, info.Width, height - 2);
                            renderedapp.Add(app);
                            appointmentViewControl.AppWidth = info.Width;
                            appointmentViewControl.Arrange(rect);
                            app.PropertyChanged += app_PropertyChanged;
                        }
                    }

                }
                baseColIdx += 1;
            }

            return finalSize;
        }

        #endregion

        #region Implementation Methods

        internal void Reset()
        {
            layoutCache.Clear();
        }

        internal void RemoveKey(ScheduleHorizontalAppointmentViewControl appCtl)
        {
            if (layoutCache.ContainsKey(appCtl))
            {
                layoutCache.Remove(appCtl);
            }
        }

        internal AppointmentPostionInfo GetAppointmentVertPosition(ScheduleAppointment app, ScheduleHorizontalAppointmentViewControl appCtl, double interval, double hourWidth, double intervalWidth, ObservableCollection<DateTime> selectedDates, double finalWidth, ref DateTime currDate, ref DateTime nextDay, int column)
        {
            ScheduleTimeLineView timelineView = this.FindParentElementOfType<ScheduleTimeLineView>();
            DateTime startTime = app.InternalStartTime >= currDate ? app.InternalStartTime : currDate;
            double x = GetIntervalWidth(startTime.TimeOfDay, interval, intervalWidth);
            TimeSpan endTimeOfDay;

            if (app.EndTime >= nextDay)
            {
                var diffmin = (int)(app.InternalEndTime - startTime).TotalMinutes;
                endTimeOfDay = new TimeSpan(0, diffmin, 0) + startTime.TimeOfDay;
            }
            else
            {
                endTimeOfDay = app.InternalEndTime.TimeOfDay;
            }
            if (timelineView != null && !timelineView.ShowNonWorkingHours && 
                timelineView.SelectedDates != null && timelineView.SelectedDates.Count > 1 &&
                app.InternalEndTime.Date != app.InternalStartTime.Date)
            {
                TimeSpan nonWorkingHourTimeSpan;
                if (app.InternalStartTime.Hour >= timelineView.WorkEndHour)
                {
                    var startDateIndex = timelineView.SelectedDates.IndexOf(app.InternalStartTime.Date) + 1;
                    x = startDateIndex * ((timelineView.WorkEndHour - timelineView.WorkStartHour) * intervalWidth);
                }
                if (app.InternalEndTime.Hour < timelineView.WorkStartHour)
                {
                    nonWorkingHourTimeSpan = new TimeSpan(1, app.InternalEndTime.Hour - timelineView.WorkEndHour, app.InternalEndTime.Minute, 0);
                }
                else
                {
                    nonWorkingHourTimeSpan = new TimeSpan(1, timelineView.WorkStartHour - timelineView.WorkEndHour, 0, 0);
                }
                endTimeOfDay = endTimeOfDay - nonWorkingHourTimeSpan;
            }

            double x2 = GetIntervalWidth(endTimeOfDay, interval, intervalWidth);

            return new AppointmentPostionInfo
                {
                    X = x,
                    Width = ((x2 > finalWidth ? finalWidth : x2) - x),
                    Column = column,
                };
        }

        private static double GetIntervalWidth(TimeSpan timeSpan, double interval, double intervalWidth)
        {
            double d = (timeSpan.TotalMinutes - ScheduleTimeLineItemsControl.MinValue * 60) / interval;
            return (d * intervalWidth);
        }

        #endregion
    }
}
