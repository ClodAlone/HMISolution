#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Controls.Primitives;
using Syncfusion.UI.Xaml.Controls.Input;
#else
using System.Windows.Controls;
using System.Windows;
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    /// <summary>
    /// Represents a header.
    /// </summary>
    public class HeaderTitleBarView : Control
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.UI.Xaml.Schedule.HeaderTitleBarView">HeaderTitleBarView</see> class. 
        /// </summary>
        public HeaderTitleBarView()
        {
            DefaultStyleKey = typeof(HeaderTitleBarView);
#if WINRT
            SizeChanged += HeaderTitleBarView_SizeChanged;
            ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateInertia;
            ManipulationDelta += HeaderTitleBarView_ManipulationDelta;
#endif
        }

#if WINRT

        void HeaderTitleBarView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (monthCalendar != null)
            {
                monthCalendar.Width = Math.Floor(e.NewSize.Width / 4);
                monthCalendar.Height = monthCalendar.Width + (monthCalendar.Width / 10);
            }
            if (yearCalendar != null)
            {
                yearCalendar.Width = Math.Floor(e.NewSize.Width / 4);
                yearCalendar.Height = yearCalendar.Width - (yearCalendar.Width / 10);
            }
        }

        void HeaderTitleBarView_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (e.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Touch)
                return;
            SfSchedule schedule = this.FindParentElementOfType<SfSchedule>();
            if (!schedule.ScrollManipulationCompleted)
                schedule.ScrollManipulationCompleted = true;
        }

#endif
        #endregion

        #region Dependency Properties

        #region HeaderText
        /// <summary>
        /// Gets the header as text.
        /// </summary>
        public string HeaderText
        {
            get { return (string)GetValue(HeaderTextProperty); }
            internal set { SetValue(HeaderTextProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HeaderText.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderTextProperty =
            DependencyProperty.Register("HeaderText", typeof(string), typeof(HeaderTitleBarView), new PropertyMetadata(""));
        #endregion

        #region HeaderNavigationButtonVisibility



        internal Visibility HeaderNavigationButtonVisibility
        {
            get { return (Visibility)GetValue(HeaderNavigationButtonVisibilityProperty); }
            set { SetValue(HeaderNavigationButtonVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderNavigationButtonVisibility.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty HeaderNavigationButtonVisibilityProperty =
            DependencyProperty.Register("HeaderNavigationButtonVisibility", typeof(Visibility), typeof(HeaderTitleBarView), new PropertyMetadata(Visibility.Visible));


        #endregion

        #region CurrentScheduleType
        /// <summary>
        /// Gets the schedule type of current view.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Schedule.ScheduleType"></seealso>
        public ScheduleType CurrentScheduleType
        {
            get { return (ScheduleType)GetValue(CurrentScheduleTypeProperty); }
            internal set { SetValue(CurrentScheduleTypeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CurrentScheduleType.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CurrentScheduleTypeProperty =
            DependencyProperty.Register("CurrentScheduleType", typeof(ScheduleType), typeof(HeaderTitleBarView), new PropertyMetadata(ScheduleType.Day, OnCurrentScheduleTypeChanged));

        private static void OnCurrentScheduleTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is HeaderTitleBarView)
            {
                (d as HeaderTitleBarView).EnableYearCalendar = (d as HeaderTitleBarView).CurrentScheduleType == ScheduleType.Month;
            }
        }

        #endregion

        #region SelectedDates
        /// <summary>
        /// Gets the collection of selected dates.
        /// </summary>
        public ObservableCollection<DateTime> SelectedDates
        {
            get { return (ObservableCollection<DateTime>)GetValue(SelectedDatesProperty); }
            internal set { SetValue(SelectedDatesProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedDates.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedDatesProperty =
            DependencyProperty.Register("SelectedDates", typeof(ObservableCollection<DateTime>), typeof(HeaderTitleBarView), new PropertyMetadata(null, OnSelectedDatesChanged));

        private static void OnSelectedDatesChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var view = obj as HeaderTitleBarView;
            if (view != null)
            {
                HeaderTitleBarView headerView = view;
                headerView.OnSelectedDatesChanged();
            }
        }

        private void OnSelectedDatesChanged()
        {
            RefreshHeaderText();
            SelectedDates.CollectionChanged -= SelectedDatesCollectionChanged;
            SelectedDates.CollectionChanged += SelectedDatesCollectionChanged;
        }

        #endregion

        #region ShowCalendar
        public bool ShowCalendar
        {
            get { return (bool)GetValue(ShowCalendarProperty); }
            internal set { SetValue(ShowCalendarProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowCalendar.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowCalendarProperty =
            DependencyProperty.Register("ShowCalendar", typeof(bool), typeof(HeaderTitleBarView), new PropertyMetadata(true));
        #endregion

        #region EnableYearCalendar
        public bool EnableYearCalendar
        {
            get { return (bool)GetValue(EnableYearCalendarProperty); }
            set { SetValue(EnableYearCalendarProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableYearCalendar.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableYearCalendarProperty =
            DependencyProperty.Register("EnableYearCalendar", typeof(bool), typeof(HeaderTitleBarView), new PropertyMetadata(false));
        #endregion

        #endregion

        #region Private Fields

#if !WINRT
        Button prevButton, nextButton;
#else
        SfCalendar monthCalendar;
        YearCalendar yearCalendar; 
#endif
        StackPanel headerPanel;


        #endregion

        #region Internal Members

        internal SfSchedule schedule;
#if WINRT
        internal Popup calendarPopup; 
#endif

        #endregion

        #region Override Methods

#if WINRT
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {            
            schedule = this.FindParentElementOfType<SfSchedule>();
#if !WINRT
            prevButton = GetTemplateChild("PART_PrevButton") as Button;
            nextButton = GetTemplateChild("PART_NextButton") as Button;
            if (prevButton != null)
                prevButton.Click += prevButton_Click;
            if (nextButton != null)
                nextButton.Click += nextButton_Click;
#else
            headerPanel = GetTemplateChild("PART_HeaderPanel") as StackPanel;
            if (headerPanel != null)
            {
                headerPanel.PointerReleased += headerPanel_PointerReleased;
                headerPanel.DataContext = this;
            }
            calendarPopup = GetTemplateChild("PART_CalendarPopup") as Popup;
            monthCalendar = GetTemplateChild("PART_MonthCalendar") as SfCalendar;
            if (monthCalendar != null)
            {
                monthCalendar.SelectionChanged += monthcalendar_SelectionChanged;
                monthCalendar.Loaded += monthCalendar_Loaded;
            }
            yearCalendar = GetTemplateChild("PART_YearCalendar") as YearCalendar;
            if (yearCalendar != null)
                yearCalendar.SelectedMonthChanged += yearCalendar_SelectedMonthChanged;
#endif
            base.OnApplyTemplate();
        }

        #endregion

        #region Events

        #region Dates Collection Changed

        private void SelectedDatesCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RefreshHeaderText();
        }

#if !WINRT
        void nextButton_Click(object sender, RoutedEventArgs e)
        {
            if (schedule != null)
                schedule.MoveToNextView();
        }

        void prevButton_Click(object sender, RoutedEventArgs e)
        {
            if (schedule != null)
                schedule.MoveToPreviousView();
        } 
#endif
        #endregion

#if WINRT
        void headerPanel_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (calendarPopup != null)
            {
                calendarPopup.IsOpen = !calendarPopup.IsOpen;
                if (calendarPopup.IsOpen)
                    RefreshCalendar();
                if (schedule != null)
                    schedule.calendarPopup = calendarPopup;
            }
        }

        void monthCalendar_Loaded(object sender, RoutedEventArgs e)
        {
            var todayButton = this.FindElementOfTypeWithName<ContentPresenter>("PART_TodayPresenter");
            if (todayButton != null)
                todayButton.PointerPressed += todayButton_PointerPressed;
        }

        void todayButton_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            ChangeScheduleDate(DateTime.Now.Date);
        }

        void monthcalendar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (monthCalendar != null)
            {
                ChangeScheduleDate((DateTime)monthCalendar.SelectedDate);
            }
        }

        void yearCalendar_SelectedMonthChanged(object sender, SelectedMonthChangedEventArgs e)
        {
            if (yearCalendar != null)
            {
                int day = (schedule.SelectedDates != null && schedule.SelectedDates.Count > 0 &&
                            DateTime.DaysInMonth(yearCalendar.SelectedYear, yearCalendar.SelectedMonth) >= schedule.SelectedDates[0].Day) ?
                            schedule.SelectedDates[0].Day : 1;
                ChangeScheduleDate(new DateTime(yearCalendar.SelectedYear, yearCalendar.SelectedMonth, day));
            }
        }

        private void ChangeScheduleDate(DateTime date)
        {
            if (schedule == null)
                schedule = this.FindParentElementOfType<SfSchedule>();
            if (schedule != null)
            {
                schedule.currentDate = date;
                schedule.MoveToDate(schedule.currentDate);
            }
            if (calendarPopup != null)
                calendarPopup.IsOpen = false;
        } 
#endif

        #endregion

        #region Methods

        #region Refresh Header Text

        private void RefreshHeaderText()
        {
            CultureInfo currentCulture = CultureInfo.CurrentCulture;
            var selectedDates = SelectedDates;
            if (selectedDates == null || selectedDates.Count <= 0)
            {
                return;
            }
            var firstDate = selectedDates.OrderByDescending(x => x).Last();
            var lastDate = selectedDates.OrderByDescending(x => x).First();
            string result;
            if (selectedDates.Count <= 7)
            {
                if (firstDate.Month == lastDate.Month && firstDate.Year == lastDate.Year)
                    result = currentCulture.DateTimeFormat.GetMonthName(currentCulture.Calendar.GetMonth(firstDate)) + " " + lastDate.Year;
                else if (firstDate.Year == lastDate.Year)
                    result = currentCulture.DateTimeFormat.GetMonthName(currentCulture.Calendar.GetMonth(firstDate)) + " - " + currentCulture.DateTimeFormat.GetMonthName(currentCulture.Calendar.GetMonth(lastDate)) + " " + lastDate.Year;
                else
                    result = currentCulture.DateTimeFormat.GetMonthName(currentCulture.Calendar.GetMonth(firstDate)) + " " + firstDate.Year + " - " + currentCulture.DateTimeFormat.GetMonthName(currentCulture.Calendar.GetMonth(lastDate)) + " " + lastDate.Year;
            }
            else
            {
                var month = currentCulture.DateTimeFormat.GetMonthName(currentCulture.Calendar.GetMonth(selectedDates[selectedDates.Count / 2].Date));
                result = month + " " + selectedDates[selectedDates.Count / 2].Year;
            }
            HeaderText = result;
        }

        #endregion

#if WINRT
        #region Refresh Calendar

        private void RefreshCalendar()
        {
            if (EnableYearCalendar)
            {
                if (yearCalendar != null)
                {
                    if (SelectedDates != null && SelectedDates.Count > 0)
                    {
                        if (CurrentScheduleType == ScheduleType.Month)
                        {
                            yearCalendar.DisplayMonth = SelectedDates[SelectedDates.Count / 2].Month;
                            yearCalendar.DisplayYear = SelectedDates[SelectedDates.Count / 2].Year;
                        }
                        else
                        {
                            yearCalendar.DisplayMonth = SelectedDates[0].Month;
                            yearCalendar.DisplayYear = SelectedDates[0].Year;
                        }
                    }
                    yearCalendar.Refresh();
                }
            }
            else
            {
                if (monthCalendar != null && SelectedDates != null && SelectedDates.Count > 0)
                {
                    monthCalendar.VisibleMinDate = DateTime.MinValue;
                    monthCalendar.VisibleMaxDate = DateTime.MaxValue;
                    monthCalendar.DisplayDate = SelectedDates[0];
                    monthCalendar.Refresh();
                }
            }
        }

        #endregion 
#endif

        #endregion
    }
}
