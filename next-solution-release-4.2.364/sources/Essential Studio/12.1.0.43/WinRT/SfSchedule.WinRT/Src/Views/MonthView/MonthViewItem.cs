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
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Input;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
#else
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Data;
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    /// <summary>
    /// Represents a month view item.
    /// </summary>
    public class MonthViewItem : Control
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Schedule.MonthViewItem">MonthViewItem</see> class.
        /// </summary>
        public MonthViewItem()
        {
            DefaultStyleKey = typeof(MonthViewItem);
        }

        #endregion

        #region Internal Fields

        internal string ResourceName = string.Empty;

        #endregion

        #region Private Fields

        private SfSchedule schedule;
        private ScheduleMonthViewItemsControl monthViewItemsControl;
        private ScheduleMonthAppointmentLayoutItemsControl monthAppointmentLayoutItemsControl;

        #endregion

        #region CLR Properties
        #endregion

        #region DependencyProperties

        #region SelectedDates
        internal ObservableCollection<DateTime> SelectedDates
        {
            get { return (ObservableCollection<DateTime>)GetValue(SelectedDatesProperty); }
            set { SetValue(SelectedDatesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedDates.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty SelectedDatesProperty =
            DependencyProperty.Register("SelectedDates", typeof(ObservableCollection<DateTime>), typeof(MonthViewItem), new PropertyMetadata(null, OnPropertyChanged));

        private  static void OnPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var scheduleMonthView = obj as MonthViewItem;
            if (scheduleMonthView != null && scheduleMonthView.schedule != null && scheduleMonthView.schedule.ScheduleType == ScheduleType.Month && scheduleMonthView.monthViewItemsControl != null)
            {
                scheduleMonthView.SetupAppointments();
            }
        }
        #endregion

        #region Appointments
        /// <summary>
        /// Gets or sets the appointments of date in month view.
        /// </summary>
        public ScheduleAppointmentCollection Appointments
        {
            get { return (ScheduleAppointmentCollection)GetValue(AppointmentsProperty); }
            set { SetValue(AppointmentsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for VisibleAppointments.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AppointmentsProperty =
            DependencyProperty.Register("Appointments", typeof(ScheduleAppointmentCollection), typeof(MonthViewItem), new PropertyMetadata(null, AppointmentsChanged));


        private static void AppointmentsChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)

        {
            var mvi = dpo as MonthViewItem;
            if (mvi != null)
            {
#if WINRT
                if (mvi.schedule != null && mvi.schedule.ScheduleType == ScheduleType.Month && mvi.schedule.VisibleDateChanged == false)
#endif
                {
                    mvi.SetupAppointments();
                }
            }
        }
        #endregion

        #region MonthViewLineStroke

        public Brush MonthViewLineStroke
        {
            get { return (Brush)GetValue(MonthViewLineStrokeProperty); }
            set { SetValue(MonthViewLineStrokeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MonthViewLineStroke.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MonthViewLineStrokeProperty =
            DependencyProperty.Register("MonthViewLineStroke", typeof(Brush), typeof(MonthViewItem), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));
        #endregion

        #region MonthHeaderDateFormat
        public string MonthHeaderDateFormat
        {
            get { return (string)GetValue(MonthHeaderDateFormatProperty); }
            set { SetValue(MonthHeaderDateFormatProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MonthHeaderDateFormat.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MonthHeaderDateFormatProperty =
            DependencyProperty.Register("MonthHeaderDateFormat", typeof(string), typeof(MonthViewItem), new PropertyMetadata("dd")); 
        #endregion
        
        #endregion

        #region Methods

        #region Days in a Year

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

        #region Add Appointment in Layout

        private void AddAppointmentInLayout(ScheduleAppointment app)
        {
            if (SelectedDates.Contains(app.StartTime.Date))
            {
                if (monthAppointmentLayoutItemsControl.Items != null)
                    monthAppointmentLayoutItemsControl.Items.Add(app);
            }
            if (app.StartTime.Date != app.EndTime.Date)
            {
                DateTime WeekStartDate = app.StartTime.Date.StartOfWeek(DayOfWeek.Sunday);
                //
                int diff;
                if (app.EndTime.Year == WeekStartDate.Year)
                {
                    diff = app.EndTime.Date.DayOfYear - WeekStartDate.DayOfYear;
                }
                else
                {
                    diff = (app.EndTime.Date - WeekStartDate.Date).Days;
                }
                WeekStartDate = WeekStartDate.AddDays(7);
                for (int x = 7; x <= diff; x += 7)
                {
                    if (SelectedDates.Contains(WeekStartDate))
                    {
                        if (monthAppointmentLayoutItemsControl.Items != null)
                            monthAppointmentLayoutItemsControl.Items.Add(app);
                    }
                    WeekStartDate = WeekStartDate.AddDays(7);
                }
            }

        }

        #endregion

        #region Set Appointments


#if WINRT
        internal async void SetupAppointments()
#else
        internal  void SetupAppointments()
#endif
        {
            if (monthAppointmentLayoutItemsControl != null)
            {
                if (monthAppointmentLayoutItemsControl.Items != null)
                {
                    monthAppointmentLayoutItemsControl.Items.Clear();
                }
#if WINRT
                if (schedule != null && schedule.VisibleDateChanged)
                    await Task.Delay(900);
#endif
            }

            if (monthAppointmentLayoutItemsControl != null)
            {
                if (monthAppointmentLayoutItemsControl.Items != null)
                {
                    #region No Resource
                   
                    if (Appointments != null)
                    {
                        foreach (var app in Appointments)
                        {
                            if (app != null && !monthAppointmentLayoutItemsControl.Items.Contains(app))
                            {
                                AddAppointmentInLayout(app);
                            }
                        }
                    }
                    #endregion

                    int navigationcount = monthAppointmentLayoutItemsControl.Items.Count;
                    foreach (DateTime dt in SelectedDates)
                    {
                        if (schedule != null && schedule.ProxyAppointments.ContainsKey(dt.Date))
                        {
                            List<ScheduleAppointment> appcollection;
                            if (!string.IsNullOrEmpty(schedule.Resource) && schedule.Resource != string.Empty)
                            {
                                appcollection = (from app in schedule.ProxyAppointments[dt.Date] where (app.ResourceCollection.FirstOrDefault(res => (res.TypeName == schedule.Resource && res.ResourceName == ResourceName)) != null) select app).ToList();
                            }
                            else
                            {
                                appcollection = (from app in schedule.ProxyAppointments[dt.Date] select app).ToList();
                            }
                            if (appcollection.Count > 2)
                            {
                                var navigationcontrol = new CollapsedScheduleAppointment { CorrespondingDate = dt.Date, EventCount = appcollection.Count + " Events", AppointmentBackground = new SolidColorBrush(Colors.Transparent), ReadOnlyVisibility = Visibility.Collapsed };
                                var textForeground = new SolidColorBrush(dt.Date.Equals(DateTime.Now.Date) ? Colors.White : Colors.Gray);
                                var child = new ScheduleMonthAppointmentViewControl { EventCount = appcollection.Count + " " + " Events", TextForeground = textForeground, DataContext = navigationcontrol, BorderThickness = new Thickness(0) };
                                monthAppointmentLayoutItemsControl.Items.Add(child);

                                var scheduleMonthAppointmentViewControl = monthAppointmentLayoutItemsControl.Items[navigationcount] as ScheduleMonthAppointmentViewControl;
                                if (scheduleMonthAppointmentViewControl != null)
#if WINRT
                                    scheduleMonthAppointmentViewControl.PointerPressed += ScheduleMonthView_PointerPressed;
#else
                                    scheduleMonthAppointmentViewControl.MouseLeftButtonDown += scheduleMonthAppointmentViewControl_MouseDown;
#endif
                                navigationcount++;
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region PointerPressed
#if WINRT
        void ScheduleMonthView_PointerPressed(object sender, PointerRoutedEventArgs e)
#else
        void scheduleMonthAppointmentViewControl_MouseDown(object sender, MouseEventArgs e)
#endif
        {
            var scheduleMonthAppointmentViewControl = sender as ScheduleMonthAppointmentViewControl;
            if (scheduleMonthAppointmentViewControl != null)
            {
                var navigator = scheduleMonthAppointmentViewControl.DataContext as CollapsedScheduleAppointment;
                schedule.ScheduleType = ScheduleType.Day;
                if (navigator != null) schedule.MoveToDate(navigator.CorrespondingDate);
            }
        }

        #endregion

        #endregion

        #region Overrides
#if WINRT
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            monthViewItemsControl = GetTemplateChild("MonthViewItemsControl") as ScheduleMonthViewItemsControl;
            monthAppointmentLayoutItemsControl = GetTemplateChild("PART_MonthViewAppointmentLayout") as ScheduleMonthAppointmentLayoutItemsControl;
            schedule = this.FindParentElementOfType<SfSchedule>();
            if (monthViewItemsControl != null)
            {
                monthViewItemsControl.schedule = schedule;
                monthViewItemsControl.GenerateDates();
                var enableAutoFormatBinding = new Binding { Source = schedule, Path = new PropertyPath("EnableAutoFormat") };
                monthViewItemsControl.SetBinding(ScheduleMonthViewItemsControl.EnableAutoFormatProperty, enableAutoFormatBinding);
            }
            SetupAppointments();
        }
        #endregion
    }
}
