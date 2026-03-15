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
using System.Collections.ObjectModel;

namespace Syncfusion.Windows.Controls.Schedule
{
    /// <summary>
    /// Represents Schedule's MonthAppointmentLayoutPanel
    /// </summary>
#if SyncfusionFramework4_0 && !SILVERLIGHT
    [System.ComponentModel.DesignTimeVisible(false)]
#endif

    public sealed class ScheduleMonthAppointmentLayoutPanel : Panel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleMonthAppointmentLayoutPanel"/> class.
        /// </summary>
        public ScheduleMonthAppointmentLayoutPanel()
        {
        }


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
            if (double.IsInfinity(availableSize.Width))
            {
                availableSize.Width = itemsContainer.Model.GetTimeSlotWidth();
            }
            if (double.IsInfinity(availableSize.Height))
            {
                availableSize.Height = itemsContainer.Model.GetTimeSlotHeight();
            }
            //var selectedDates = itemsContainer.Model.SelectedDates;
            //foreach (var selectedDate in selectedDates)
            //{
            //    var appT = itemsContainer.Model.GetDailyAppointments(selectedDate);
            //    foreach (var app in appT)
            //    {
            //        if (app != null)
            //        {
            //            var day = app.StartTime.Day;
            //            var cur = selectedDate.Day;                        
            //        }                    
            //    }
            //}

            return availableSize;
        }

        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a
        /// size for a <see cref="T:System.Windows.FrameworkElement" /> derived class. 
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element
        /// should use to arrange itself and its children.</param>
        /// <returns>
        /// The actual size used.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.Children.Count <= 0)
            {
                return finalSize;
            }

            var itemsContainerLayout = ((FrameworkElement)this.Children[0]).FindParentElementOfType<IScheduleCalendarViewModelHost>();
            if (itemsContainerLayout == null)
            {
                return Size.Empty;
            }
            var model = itemsContainerLayout.Model;
            var itemsContainer = ((FrameworkElement)itemsContainerLayout).FindParentElementOfType<ScheduleMonthView>();
            if (itemsContainer == null)
            {
                return Size.Empty;
            }
            var contentCtl = ((FrameworkElement)itemsContainer).FindElementOfType<ScheduleMonthViewItemsControl>();
            var content = contentCtl.Items[0] as ScheduleMonthDateContentControl;
            
            var contentActualHeight = content.ActualHeight;
            var contentActualWidth = content.ActualWidth;
            if (contentActualHeight <= 0 || contentActualWidth <= 0)
            {
                contentActualHeight = finalSize.Height / (model.SelectedDates.Count / 7);
                contentActualWidth = finalSize.Width / 7;
            }

            int i = 1;
            int check = 0;
            int dateindex = 0;
            foreach (var selectedDate in itemsContainer.Model.SelectedDates)
            {
                var currDate = selectedDate;
                int row = i / 7;
                int col = i % 7;               
                if (col == 0)
                {
                    col = 7;
                    row -= 1;
                }
                dateindex++;
                // 27d is added to yOffset for the header of each monthviewContentcontrol which is of 27d height.

                double yOffset = ((row) * contentActualHeight) + 27d;
                double xOffset = ((col - 1) * contentActualWidth);
                List<ScheduleAppointmentInfo> appointments = new List<ScheduleAppointmentInfo>();
                appointments = GetAppointmentWithProxy(model, selectedDate).ToList();
               // var appointments = itemsContainer.Model.GetCurrentAppointmentsByDate(selectedDate).ToList();
                double height = 0d;
                IEnumerable<ScheduleMonthAppointmentViewControl> elcoll = this.Children.OfType<ScheduleMonthAppointmentViewControl>();
                ScheduleMonthAppointmentViewControl[] SAppctrl = new ScheduleMonthAppointmentViewControl[elcoll.Count()];
                int k = 0;
                foreach (ScheduleMonthAppointmentViewControl _SAppctrl in elcoll)
                {
                    SAppctrl[k] = _SAppctrl;
                    k++;
                }
                foreach (var app in appointments)
                {
                    if (app != null)
                    {
                        ScheduleMonthAppointmentViewControl appCtl = null;
                        foreach (ScheduleMonthAppointmentViewControl spc in SAppctrl)                        
                        {
                            if (app.Appointment.MatchWithExists(spc.DataContext as ScheduleAppointment))                            
                            {
                                appCtl = spc;
                                break;
                            }
                        }
                       // var appCtl = elcoll.FirstOrDefault(a => app.Appointment.MatchWithExists(a.DataContext as ScheduleAppointment) == true);
                        if (appCtl != null)
                        {
                            var positionInfo =  GetPositionInfo(appCtl, selectedDate, selectedDate.AddDays(1), itemsContainer.Model);
                            if (positionInfo.Columns == 0)
                            {
                                positionInfo.Columns = 1;
                            }
                            else if (positionInfo.Columns > 1 && positionInfo.IsSpanned == true)
                            {
                                int yset;
                                if (check == 0 || check % 2 == 0)
                                    yset = 0;
                                else
                                    yset = 1;
                                height = yset * 20d;
                            }

                            //int yset = this.GetCountForSameTimeslot(appCtl.ScheduleAppointment, itemsContainer.Model);
                            //if (!isFirstAppointment)
                            //{
                            //    height = yset * 20d;
                            //    isFirstAppointment = true;
                            //}

                            if (height <= ((contentActualHeight / 10) * 7) - 20d)
                            {
                                appCtl.Arrange(new Rect(xOffset + 5d, yOffset + height, (contentActualWidth * positionInfo.Columns) - 10d, 20d));
                                //if (app.Appointment.StartTime < app.Appointment.EndTime)
                                //{
                                //    height -= 20d;
                                //}
                                //else
                                //{
                                    height += 20d;
                                //}
                                    // Sets the opacity based on the available size in the container 
                                    (contentCtl.Items[dateindex - 1] as ScheduleMonthDateContentControl).NavigatorButtonOpacity = 0d;
                                contentCtl.UnSetOverflowingMonthDate(app.Appointment);
                                check++;
                            }
                            else
                            {
                               appCtl.Arrange(new Rect(xOffset + 5d, yOffset + height, (contentActualWidth * positionInfo.Columns), 0d));
                               // Sets the opacity based on the available size in the container 
                               (contentCtl.Items[dateindex-1] as ScheduleMonthDateContentControl).NavigatorButtonOpacity = 1d;            
                               contentCtl.SetOverflowingMonthDate(app.Appointment);
                               check++;
                            }
                        }
                    }
                }
                i++;
            }

            return finalSize;
        }

        private IEnumerable<ScheduleAppointmentInfo> GetAppointmentWithProxy(ScheduleCalendarViewModel model, DateTime selectedDate)
        {
            var appointments = model.GetCurrentAppointmentsByDate(selectedDate).ToList();
            var appProxy = model.AppointmentProxy.Where(ap => ap.AppointmentProxy.StartTime.Date == selectedDate).OrderBy(ap => ap.AppointmentProxy.StartTime);

            foreach (var item in appProxy)
            {
                appointments.Remove(appointments.Where(res => (res != null && res.Appointment.MatchWithExists(item.ParentAppointment))).FirstOrDefault());
                appointments.Add(new ScheduleAppointmentInfo() { Appointment = item.AppointmentProxy, IsSpanned = true });
            }
            return appointments.Where(ap => ap != null).OrderByDescending(ap => ap.Appointment.Duration.TotalHours);
        }

        private int GetCountForSameTimeslot(ScheduleAppointment scheduleAppointment, ScheduleCalendarViewModel scheduleCalendarViewModel, DateTime selectedDate)
        {
            int totalcnt = 0;

            if (scheduleAppointment.CurrentAppointmentType == AppointmentType.MultiWeek)
            {
                var appCnt1 = from res in scheduleCalendarViewModel.AppointmentProxy
                              where res.AppointmentProxy.MatchWithExists(scheduleAppointment)
                              select res.ParentAppointment;

                if (appCnt1.Count() > 0) scheduleAppointment = appCnt1.FirstOrDefault() as ScheduleAppointment;
            }

            var appCnt = from res in scheduleCalendarViewModel.Appointments
                         where res.StartTime.Date < selectedDate.Date && res.EndTime.Date > selectedDate.Date
                         && (res.StartTime.Date != res.EndTime.Date) && !res.MatchWithExists(scheduleAppointment)
                         select res;
            if (appCnt.Count() > 0) totalcnt += appCnt.Count();

            return totalcnt;
        }

        private int GetCountForSameTimeslot(ScheduleAppointment scheduleAppointment, ScheduleCalendarViewModel scheduleCalendarViewModel)
        {
            int totalcnt = 0;

            var parentscheduleAppointment = scheduleAppointment;

            if (scheduleAppointment.CurrentAppointmentType == AppointmentType.MultiWeek)
            {
                var appCnt1 = from res in scheduleCalendarViewModel.AppointmentProxy
                              where res.AppointmentProxy.MatchWithExists(scheduleAppointment)
                              select res.ParentAppointment;

                if (appCnt1.Count() > 0) parentscheduleAppointment = appCnt1.FirstOrDefault() as ScheduleAppointment;
            }

            var grdAppID = from res in scheduleCalendarViewModel.AppointmentProxy
                           group res by res.ParentAppointment.ID into pt
                           select pt;

            if (grdAppID.Count() > 0)
            {
                List<ScheduleAppointment> appWithoutProxy = scheduleCalendarViewModel.Appointments.ToList();
               
                var remApp = from rs in scheduleCalendarViewModel.Appointments
                             from gid in grdAppID
                             where rs.MatchWithExists(gid.FirstOrDefault().ParentAppointment)
                             select rs;
                if (remApp.Count() > 0)
                {
                    foreach (var remitem in remApp)
                    {
                        appWithoutProxy.Remove(remitem);
                    }
                }

                if (appWithoutProxy.Count() > 0)
                {
                    var appCnt = from res in appWithoutProxy
                                 where res.StartTime < scheduleAppointment.StartTime && res.EndTime >= scheduleAppointment.StartTime
                                 && (res.StartTime != res.EndTime) && !res.MatchWithExists(parentscheduleAppointment)
                                 select res;

                    if (appCnt.Count() > 0) totalcnt += appCnt.Count();
                }
            }
            else
            {
                var appCnt = from res in scheduleCalendarViewModel.Appointments
                             where res.StartTime < scheduleAppointment.StartTime && res.EndTime >= scheduleAppointment.StartTime
                             && (res.StartTime != res.EndTime) && !res.MatchWithExists(parentscheduleAppointment)
                             select res;
                if (appCnt.Count() > 0) totalcnt += appCnt.Count();
            }
            var appCnt2 = from res in scheduleCalendarViewModel.AppointmentProxy
                          where res.AppointmentProxy.StartTime < scheduleAppointment.StartTime && res.AppointmentProxy.EndTime >= scheduleAppointment.StartTime
                         && (res.AppointmentProxy.StartTime != res.AppointmentProxy.EndTime) && !res.AppointmentProxy.MatchWithExists(parentscheduleAppointment)
                         select res;
            if (appCnt2.Count() > 0) totalcnt += appCnt2.Count();

            return totalcnt;
        }

        private PositionInfo GetPositionInfo(ScheduleMonthAppointmentViewControl appCtl, DateTime currDate, DateTime nextDay, ScheduleCalendarViewModel model)
        {
            int columns = 1;
            var app = appCtl.ScheduleAppointment;
            var isSpanned = app.EndTime >= nextDay;
            if (isSpanned)
            {
                if (app.StartTime.Day == currDate.Day)
                {
                    for (DateTime i = currDate.Date; i < app.EndTime.Date; i = i.AddDays(1))
                    {                       
                        columns += 1;
                    }
                }
            }

            return new PositionInfo() { IsSpanned = isSpanned, Columns = columns };
        }

        private class PositionInfo
        {
            public bool IsSpanned { get; set; }
            public int Columns { get; set; }
        }

        private Dictionary<ScheduleMonthAppointmentViewControl, AppointmentPostionInfo> layoutCache = new Dictionary<ScheduleMonthAppointmentViewControl, AppointmentPostionInfo>();

        internal void Reset()
        {
            this.layoutCache.Clear();
        }

        internal void RemoveKey(ScheduleMonthAppointmentViewControl appCtl)
        {
            if (this.layoutCache.ContainsKey(appCtl))
            {
                this.layoutCache.Remove(appCtl);
            }
        }

        private class AppointmentPostionInfo
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Height { get; set; }
            public int Column { get; set; }
            public bool IsSpanned { get; set; }
            public bool IsIntersecting { get { return this.IntersectCount > 0; } }
            public int IntersectCount { get; set; }
            public int IntersectIndex { get; set; }
        }
    }
}
