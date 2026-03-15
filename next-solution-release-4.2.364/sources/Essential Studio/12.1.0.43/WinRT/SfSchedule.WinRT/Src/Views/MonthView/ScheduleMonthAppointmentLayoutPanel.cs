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
    #region ScheduleMonthAppointmentLayoutPanel

    /// <summary>
    /// Represents a layout panel for arranging month view appointments.
    /// </summary>
    public class ScheduleMonthAppointmentLayoutPanel : Panel
    {
        #region Private Fields

        private SfSchedule schedule;
        private ScheduleMonthView parent;
        #endregion

        #region Overrides

        #region MeasureOverride

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Children.Count == 0)
            {
                return new Size();
            }

            var itemsContainer = ((FrameworkElement)Children[0]).FindParentElementOfType<SfSchedule>();
            if (itemsContainer == null)
            {
                return new Size();
            }

            if (double.IsInfinity(availableSize.Height))
            {
                availableSize.Height = availableSize.Width;
            }
            return availableSize;
        }

        #endregion

        #region ArrangeOverride

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Children.Count <= 0)
            {
                return finalSize;
            }

            var itemsContainerLayout = ((FrameworkElement)Children[0]).FindParentElementOfType<ScheduleMonthAppointmentLayoutItemsControl>();
            schedule = ((FrameworkElement)Children[0]).FindParentElementOfType<SfSchedule>();
            if (itemsContainerLayout == null)
            {
                return new Size();
            }
            var itemsContainer = itemsContainerLayout.FindParentElementOfType<ScheduleMonthView>();
            if (itemsContainer == null)
            {
                return new Size();
            }
            var contentCtl = itemsContainer.FindElementOfType<ScheduleMonthViewItemsControl>();
            if (contentCtl.Items != null)
            {
                var content = contentCtl.Items[0] as ScheduleMonthDateContentControl;

                if (content != null)
                {
                    var contentActualHeight = content.ActualHeight;
                    var contentActualWidth = content.ActualWidth;
                    if (contentActualHeight <= 0 || contentActualWidth <= 0)
                    {
                        contentActualHeight = finalSize.Height * 7 / (itemsContainer.SelectedDates.Count);
                        contentActualWidth = finalSize.Width / 7;
                    }

                    int i = 1;
                    int check = 0;

                    IEnumerable<ScheduleMonthAppointmentViewControl> elcoll = Children.OfType<ScheduleMonthAppointmentViewControl>();
                    var scheduleMonthAppointmentViewControls = elcoll as ScheduleMonthAppointmentViewControl[] ?? elcoll.ToArray();
                    var SAppctrl = new ScheduleMonthAppointmentViewControl[scheduleMonthAppointmentViewControls.Count()];

                    int k = 0;
                    foreach (ScheduleMonthAppointmentViewControl _SAppctrl in scheduleMonthAppointmentViewControls)
                    {

                        SAppctrl[k] = _SAppctrl;
                        k++;
                    }
                    double childrenheight = Math.Floor((contentActualHeight) / 3);
                    double headerheight = Math.Floor((contentActualHeight) - (childrenheight * 2)) - 1;
                    var childcache = new ObservableCollection<ScheduleAppointment>();
                    var inVisibleApps = new ObservableCollection<ScheduleAppointment>();
                    var selecteddates = new ObservableCollection<DateTime>(itemsContainer.SelectedDates.OrderBy(s => s));
                    int freeRowIndex = 0;
                    int maxColumnCount = 1;
                    int tempMaxColumnCount = 1;
                    foreach (var selectedDate in selecteddates)
                    {
                        maxColumnCount = maxColumnCount <= 0 ? 0 : maxColumnCount - 1;
                        tempMaxColumnCount = tempMaxColumnCount < 0 ? 0 : tempMaxColumnCount - 1;
                        int row = i / 7;
                        int col = i % 7;
                        if (col == 0)
                        {
                            col = 7;
                            row -= 1;
                        }

                        bool sameDay = false;
                        double yOffset = ((row) * contentActualHeight) + headerheight;
                        double xOffset = ((col - 1) * contentActualWidth);
                        var appointments = new List<ScheduleAppointment>();
                        var nonCacheApp = new List<ScheduleAppointment>();
                        if (schedule.ProxyAppointments.ContainsKey(selectedDate.Date))
                        {
                            appointments = schedule.ProxyAppointments[selectedDate.Date].ToList();
                        }
                        appointments = appointments.OrderBy(app => (app.InternalEndTime - app.InternalStartTime)).ToList();
                        appointments.Reverse();
                        double rowsOccupied = 0d;
                        double height = 0d;
                        int appointmentscount = 0;
                        foreach (var noncache in appointments)
                        {
                            if (childcache.Contains(noncache))
                            {
                                rowsOccupied++;
                            }
                            else
                                nonCacheApp.Add(noncache);

                        }
                        foreach (var app in nonCacheApp)
                        {
                            if (!childcache.Contains(app))
                            {
                                var appCtls = new ObservableCollection<ScheduleMonthAppointmentViewControl>();
                                ScheduleMonthAppointmentViewControl Navigator = null;
                                foreach (ScheduleMonthAppointmentViewControl spc in SAppctrl)
                                {
                                    if (spc.DataContext is ScheduleAppointment)
                                    {
                                        if (app.Equals(spc.DataContext as ScheduleAppointment))
                                        {
                                            appCtls.Add(spc);
                                        }
                                    }
                                    else
                                    {
                                        var collapsedScheduleAppointment = spc.DataContext as CollapsedScheduleAppointment;
                                        if (collapsedScheduleAppointment != null && app.InternalStartTime.Date == collapsedScheduleAppointment.CorrespondingDate)
                                        {
                                            Navigator = spc;
                                        }
                                    }
                                }
                                ScheduleAppointment scheduleAppointment = null;
                                if (appCtls.Count > 0)
                                {
                                    scheduleAppointment = appCtls[0].DataContext as ScheduleAppointment;
                                }
                                if (scheduleAppointment != null && (appointmentscount < 2 && appCtls.Count > 0 && (scheduleAppointment.InternalStartTime.Date == selectedDate || (scheduleAppointment.InternalStartTime.Date <= selecteddates[0]) && scheduleAppointment.InternalEndTime.Date >= selecteddates[0])))
                                {
                                    if (appCtls.Count > 0)
                                    {
                                        DateTime Appctrlstarttime = selectedDate;
                                        DateTime startofweek = Appctrlstarttime.StartOfWeek(DayOfWeek.Sunday);

                                        int diff;
                                        if (Appctrlstarttime.Year == startofweek.Year)
                                        {
                                            diff = Appctrlstarttime.DayOfYear - startofweek.DayOfYear;
                                        }
                                        else
                                        {
                                            diff = (Appctrlstarttime.DayOfYear + GetDaysInAYear(startofweek.Year)) - startofweek.DayOfYear;
                                        }
                                        //7 number of days in a week.
                                        int diffcol = 7 - diff;
                                        var positionInfo = GetPositionInfo(appCtls[0], selectedDate, selectedDate.AddDays(1));
                                        if (positionInfo.Columns == 0)
                                        {
                                            positionInfo.Columns = 1;
                                        }
                                        else if (positionInfo.Columns > 1 && positionInfo.IsSpanned)
                                        {
                                            int yset;
                                            if (check == 0 || check % 2 == 0)
                                                yset = 0;
                                            else
                                                yset = 1;
                                            height = yset * childrenheight;
                                        }
                                        int rowcount = 0;
                                        int remainingcol = positionInfo.Columns - diffcol;
                                        if (rowsOccupied < 2)
                                        {
                                            if (tempMaxColumnCount <= positionInfo.Columns)
                                            {
                                                tempMaxColumnCount = positionInfo.Columns;
                                                if (maxColumnCount == 0)
                                                {
                                                    maxColumnCount = tempMaxColumnCount;
                                                    freeRowIndex = freeRowIndex == 0 ? 1 : 0;
                                                }
                                            }
                                            else
                                            {
                                                if (maxColumnCount == 0)
                                                {
                                                    maxColumnCount = tempMaxColumnCount;
                                                    freeRowIndex = freeRowIndex == 0 ? 1 : 0;
                                                }
                                            }
                                            if (!(rowsOccupied.Equals(0)) && (!sameDay))
                                            {
                                                height = freeRowIndex * childrenheight;
                                            }
                                            foreach (ScheduleMonthAppointmentViewControl appCtl in appCtls)
                                            {

                                                if (rowcount > 0)
                                                {
                                                    appCtl.AppWidth = (contentActualWidth * remainingcol) - 1d;
                                                    appCtl.Arrange(new Rect(0 + 1d, (((row + rowcount) * contentActualHeight) + headerheight) + height, (contentActualWidth * remainingcol) - 1d, childrenheight));
#if !WINRT
                                                    appCtl.Height = childrenheight; 
#endif
                                                    remainingcol = remainingcol - 7;
                                                    rowcount++;
                                                }
                                                else
                                                {
                                                    appCtl.AppWidth = (contentActualWidth * positionInfo.Columns) - 1d;
                                                    appCtl.Arrange(new Rect(xOffset + 1d, yOffset + height, (contentActualWidth * positionInfo.Columns) - 1d, childrenheight));
#if !WINRT
                                                    appCtl.Height = childrenheight;
#endif
                                                    rowcount++;
                                                    sameDay = true;
                                                    if (rowsOccupied.Equals(1))
                                                        rowsOccupied++;
                                                }
                                            }
                                            childcache.Add(app);
                                        }
                                        else
                                        {
                                            inVisibleApps.Add(app);
                                        }
                                        if (Navigator != null)
                                        {
                                            Navigator.Arrange(new Rect(xOffset + (contentActualWidth / 2), yOffset - headerheight + 1, (contentActualWidth / 2) - 1d, childrenheight));
                                        }

                                        app.PropertyChanged += app_PropertyChanged;
                                        if (sameDay && rowsOccupied.Equals(0))
                                            height += childrenheight;
                                        if (positionInfo.Columns > 1 && positionInfo.IsSpanned)
                                            check++;
                                    }
                                    appointmentscount++;
                                }
                                else if (appointmentscount >= 2)
                                {
                                    if (Navigator != null)
                                    {
                                        Navigator.Arrange(new Rect(xOffset + (contentActualWidth / 2), yOffset - headerheight + 1, (contentActualWidth / 2) - 1d, childrenheight));
                                    }
                                    if (appCtls.Count > 0 && Children.Contains(appCtls[0]))
                                        appCtls[0].Visibility = Visibility.Collapsed;
                                }
                            }
                            else
                            {
                                appointmentscount++;
                                //check++;
                                //height += childrenheight;
                            }

                        }
                        i++;

                    }
                }
            }

            return finalSize;
        }

        #endregion

        #endregion

        #region Methods

        public ScheduleMonthAppointmentLayoutPanel()
        {
#if WINRT
            ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY | ManipulationModes.TranslateInertia;
            ManipulationDelta += ScheduleMonthAppointmentViewControl_ManipulationDelta;
#endif
            Loaded += ScheduleMonthAppointmentLayoutPanel_Loaded;
        }

        void ScheduleMonthAppointmentLayoutPanel_Loaded(object sender, RoutedEventArgs e)
        {
            parent = this.FindParentElementOfType<ScheduleMonthView>();
            schedule = this.FindParentElementOfType<SfSchedule>();
        }

        #region GetDaysInYear

        private int GetDaysInAYear(int year)
        {

            int days = 0;
            for (int i = 1; i <= 12; i++)
            {
                days += DateTime.DaysInMonth(year, i);
            }
            return days;
        }

        #endregion

        #region GetPosition Info

        private PositionInfo GetPositionInfo(ScheduleMonthAppointmentViewControl appCtl, DateTime currDate, DateTime nextDay)
        {
            int columns = 1;
            var app = appCtl.DataContext as ScheduleAppointment;
            var isSpanned = app != null && app.EndTime >= nextDay;
            if (isSpanned)
            {
                for (DateTime i = currDate.Date; i < app.EndTime.Date; i = i.AddDays(1))
                {
                    columns += 1;
                }
            }

            return new PositionInfo { IsSpanned = isSpanned, Columns = columns };
        }

        #endregion

        #endregion

        #region Events

        #region Property Changed

        void app_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == "InternalStartTime" || e.PropertyName == "InternalEndTime" || e.PropertyName == "AppointmentBackground" || e.PropertyName == "Subject" || e.PropertyName == "ReadOnly" || e.PropertyName == "IsRecursive"))
            {
                InvalidateArrange();
            }
        }

        #endregion

#if WINRT
        void ScheduleMonthAppointmentViewControl_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (e.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Touch)
                return;
            if (Math.Abs(e.Delta.Translation.X) > Math.Abs(e.Delta.Translation.Y))
            {
                double delta = parent.ResourceScrollViewer.HorizontalOffset + (e.Delta.Translation.X * -1);
                if (delta <= parent.ResourceScrollViewer.ScrollableWidth)
                {
#if SyncfusionFramework4_5_11
                    parent.ResourceScrollViewer.ChangeView(delta, null, null);
#else
                    parent.ResourceScrollViewer.ScrollToHorizontalOffset(delta);
#endif
                    if (delta <= 0)
                    {
                        schedule.ScrollManipulationCompleted = true;
                    }
                }
                else
                {
                    schedule.ScrollManipulationCompleted = true;
                }

            }
            else
            {
                if (parent.ResourceScrollViewer.VerticalOffset + (e.Delta.Translation.Y * -1) <= parent.ResourceScrollViewer.ScrollableHeight && parent.ResourceScrollViewer.VerticalOffset + (e.Delta.Translation.Y * -1) >= 0)
                {
#if SyncfusionFramework4_5_11
                    parent.ResourceScrollViewer.ChangeView(null, parent.ResourceScrollViewer.VerticalOffset + (e.Delta.Translation.Y * -1), null);
#else
                    parent.ResourceScrollViewer.ScrollToVerticalOffset(parent.ResourceScrollViewer.VerticalOffset + (e.Delta.Translation.Y * -1));
#endif
                }
            }


        }
#endif
        #endregion

        #region InnerClass PositionInfo

        private class PositionInfo
        {
            #region Public Fields

            public bool IsSpanned { get; set; }
            public int Columns { get; set; }

            #endregion
        }

        #endregion
    }

    #endregion

    #region ScheduleAppointmentInfo

    internal class ScheduleAppointmentInfo
    {
        #region Public Fields

        public bool IsSpanned { get; set; }
        public ScheduleAppointment Appointment { get; set; }

        #endregion
    }

    #endregion
}
