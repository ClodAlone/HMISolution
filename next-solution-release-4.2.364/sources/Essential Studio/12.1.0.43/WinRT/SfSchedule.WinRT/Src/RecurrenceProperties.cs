#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
#if WINRT
using Windows.UI.Xaml;
#else
using System.Windows;
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    /// <summary>
    /// Represents the Recurrence Properties
    /// </summary>
    public class RecurrenceProperties : DependencyObject
    {
        #region Internal Properties

        internal bool hasRangeStartDate;
        internal bool canUpdateOtherProperty = true;
        #endregion

        #region Dependency Properties

        #region Recurrence type
        /// <summary>
        /// Gets or sets the recurrence type.
        /// </summary>
        public RecurrenceType RecurrenceType
        {
            get { return (RecurrenceType)GetValue(RecurrenceTypeProperty); }
            set { SetValue(RecurrenceTypeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RecurrenceType.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RecurrenceTypeProperty =
            DependencyProperty.Register("RecurrenceType", typeof(RecurrenceType), typeof(RecurrenceProperties), new PropertyMetadata(RecurrenceType.Daily));
        #endregion

        #region RecurrenceRule
        /// <summary>
        /// Gets or sets the recurrence rule.
        /// </summary>
        public string RecurrenceRule
        {
            get { return (string)GetValue(RecurrenceRuleProperty); }
            set { SetValue(RecurrenceRuleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RecurrenceRule.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RecurrenceRuleProperty =
            DependencyProperty.Register("RecurrenceRule", typeof(string), typeof(RecurrenceProperties), new PropertyMetadata(string.Empty));
        #endregion

        #region IsRangeRecurrenceCount
        /// <summary>
        /// Gets or sets a value indicating whether the count of recurrence should be set.
        /// </summary>
        public bool IsRangeRecurrenceCount
        {
            get { return (bool)GetValue(IsRangeRecurrenceCountProperty); }
            set { SetValue(IsRangeRecurrenceCountProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsRangeRecurrenceCount.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsRangeRecurrenceCountProperty =
            DependencyProperty.Register("IsRangeRecurrenceCount", typeof(bool), typeof(RecurrenceProperties), new PropertyMetadata(true, OnRecurrenceRangeChanged));
        #endregion

        #region IsRangeEndDate
        /// <summary>
        /// Gets or sets a value indicating whether the date should be specified for ending the recurrence.
        /// </summary>
        public bool IsRangeEndDate
        {
            get { return (bool)GetValue(IsRangeEndDateProperty); }
            set { SetValue(IsRangeEndDateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsRangeEndDate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsRangeEndDateProperty =
            DependencyProperty.Register("IsRangeEndDate", typeof(bool), typeof(RecurrenceProperties), new PropertyMetadata(false, OnRecurrenceRangeChanged));

        private static void OnRecurrenceRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var RecProp = d as RecurrenceProperties;
            if (RecProp != null)
            {
                if (RecProp.IsRangeEndDate)
                {
                    RecProp.IsRangeRecurrenceCount = false;
                    RecProp.IsRangeNoEndDate = false;
                }
                else if (RecProp.IsRangeNoEndDate)
                {
                    RecProp.IsRangeRecurrenceCount = false;
                    RecProp.IsRangeEndDate = false;
                }
                else if (RecProp.IsRangeRecurrenceCount)
                {
                    RecProp.IsRangeNoEndDate = false;
                    RecProp.IsRangeEndDate = false;
                }
            }
        }
        #endregion

        #region IsRangeNoEndDate
        /// <summary>
        /// Gets or sets a value indicating whether the recurrence should be ended.
        /// </summary>
        public bool IsRangeNoEndDate
        {
            get { return (bool)GetValue(IsRangeNoEndDateProperty); }
            set { SetValue(IsRangeNoEndDateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsRangeNoEndDate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsRangeNoEndDateProperty =
            DependencyProperty.Register("IsRangeNoEndDate", typeof(bool), typeof(RecurrenceProperties), new PropertyMetadata(false, OnRecurrenceRangeChanged));
        #endregion

        #region RangeStartDate
        /// <summary>
        /// Gets or sets the date to start the recurrence.
        /// </summary>
        public DateTime RangeStartDate
        {
            get { return (DateTime)GetValue(RangeStartDateProperty); }
            set { SetValue(RangeStartDateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RangeStartDate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RangeStartDateProperty =
            DependencyProperty.Register("RangeStartDate", typeof(DateTime), typeof(RecurrenceProperties), new PropertyMetadata(DateTime.Now.Date, OnRangeStartDateChanged));

        private static void OnRangeStartDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue)
            {
                (d as RecurrenceProperties).hasRangeStartDate = true;
            }
        }
        #endregion

        #region RangeEndDate
        /// <summary>
        /// Gets or sets the date to end the recurrence.
        /// </summary>
        public DateTime RangeEndDate
        {
            get { return (DateTime)GetValue(RangeEndDateProperty); }
            set { SetValue(RangeEndDateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RangeEndDate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RangeEndDateProperty =
            DependencyProperty.Register("RangeEndDate", typeof(DateTime), typeof(RecurrenceProperties), new PropertyMetadata(DateTime.Today.AddDays(1), OnRangeEndDateChanged));

        private static void OnRangeEndDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is RecurrenceProperties && (d as RecurrenceProperties).canUpdateOtherProperty)
            {
                (d as RecurrenceProperties).IsRangeEndDate = true;
                (d as RecurrenceProperties).IsRangeRecurrenceCount = false;
                (d as RecurrenceProperties).IsRangeNoEndDate = false;
            }
        }
        #endregion

        #region RangeRecurrenceCount
        /// <summary>
        /// Gets or sets the count for recurring appointment.
        /// </summary>
        public int RangeRecurrenceCount
        {
            get { return (int)GetValue(RangeRecurrenceCountProperty); }
            set { SetValue(RangeRecurrenceCountProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RangeRecurrenceCount.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RangeRecurrenceCountProperty =
            DependencyProperty.Register("RangeRecurrenceCount", typeof(int), typeof(RecurrenceProperties), new PropertyMetadata(1, OnRangeRecurrenceCountChanged));

        private static void OnRangeRecurrenceCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RecurrenceProperties)
            {
                (d as RecurrenceProperties).IsRangeRecurrenceCount = true;
                (d as RecurrenceProperties).IsRangeEndDate = false;
                (d as RecurrenceProperties).IsRangeNoEndDate = false;
            }
        }
        #endregion

        #region IsDailyEveryNDays
        /// <summary>
        /// Gets or sets a value indicating whether the recurrence should be set based on specified day interval.
        /// </summary>
        public bool IsDailyEveryNDays
        {
            get { return (bool)GetValue(IsDailyEveryNDaysProperty); }
            set { SetValue(IsDailyEveryNDaysProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsDailyEveryNDays.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsDailyEveryNDaysProperty =
            DependencyProperty.Register("IsDailyEveryNDays", typeof(bool), typeof(RecurrenceProperties), new PropertyMetadata(true));
        #endregion

        #region DailyNDays
        /// <summary>
        /// Gets or sets the day interval on which recurrence has to be set.
        /// </summary>
        public int DailyNDays
        {
            get { return (int)GetValue(DailyNDaysProperty); }
            set { SetValue(DailyNDaysProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DailyNDays.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DailyNDaysProperty =
            DependencyProperty.Register("DailyNDays", typeof(int), typeof(RecurrenceProperties), new PropertyMetadata(1));
        #endregion

        #region WeeklyEveryNWeeks
        /// <summary>
        /// Gets or sets the week interval on which recurrence has to be set.
        /// </summary>
        public int WeeklyEveryNWeeks
        {
            get { return (int)GetValue(WeeklyEveryNWeeksProperty); }
            set { SetValue(WeeklyEveryNWeeksProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for WeeklyEveryNWeeks.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty WeeklyEveryNWeeksProperty =
            DependencyProperty.Register("WeeklyEveryNWeeks", typeof(int), typeof(RecurrenceProperties), new PropertyMetadata(1));
        #endregion

        #region IsWeeklySunday
        /// <summary>
        /// Gets or sets a value indicating whether the recurrence should be applied on Sundays with specified week interval.
        /// </summary>
        public bool IsWeeklySunday
        {
            get { return (bool)GetValue(IsWeeklySundayProperty); }
            set { SetValue(IsWeeklySundayProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsWeeklySunday.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsWeeklySundayProperty =
            DependencyProperty.Register("IsWeeklySunday", typeof(bool), typeof(RecurrenceProperties), new PropertyMetadata(true));
        #endregion

        #region IsWeeklyMonday
        /// <summary>
        /// Gets or sets a value indicating whether the recurrence should be applied on Mondays with specified week interval.
        /// </summary>
        public bool IsWeeklyMonday
        {
            get { return (bool)GetValue(IsWeeklyMondayProperty); }
            set { SetValue(IsWeeklyMondayProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsWeeklyMonday.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsWeeklyMondayProperty =
            DependencyProperty.Register("IsWeeklyMonday", typeof(bool), typeof(RecurrenceProperties), new PropertyMetadata(false));
        #endregion

        #region IsWeeklyTuesday
        /// <summary>
        /// Gets or sets a value indicating whether the recurrence should be applied on Tuesdays with specified week interval.
        /// </summary>
        public bool IsWeeklyTuesday
        {
            get { return (bool)GetValue(IsWeeklyTuesdayProperty); }
            set { SetValue(IsWeeklyTuesdayProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsWeeklyTuesday.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsWeeklyTuesdayProperty =
            DependencyProperty.Register("IsWeeklyTuesday", typeof(bool), typeof(RecurrenceProperties), new PropertyMetadata(false));
        #endregion

        #region IsWeeklyWednesday
        /// <summary>
        /// Gets or sets a value indicating whether the recurrence should be applied on Wednesdays with specified week interval.
        /// </summary>
        public bool IsWeeklyWednesday
        {
            get { return (bool)GetValue(IsWeeklyWednesdayProperty); }
            set { SetValue(IsWeeklyWednesdayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsWeeklyWednesday.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsWeeklyWednesdayProperty =
            DependencyProperty.Register("IsWeeklyWednesday", typeof(bool), typeof(RecurrenceProperties), new PropertyMetadata(false));
        #endregion

        #region IsWeeklyThursday
        /// <summary>
        /// Gets or sets a value indicating whether the recurrence should be applied on Thursdays with specified week interval.
        /// </summary>
        public bool IsWeeklyThursday
        {
            get { return (bool)GetValue(IsWeeklyThursdayProperty); }
            set { SetValue(IsWeeklyThursdayProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsWeeklyThursday.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsWeeklyThursdayProperty =
            DependencyProperty.Register("IsWeeklyThursday", typeof(bool), typeof(RecurrenceProperties), new PropertyMetadata(false));
        #endregion

        #region IsWeeklyFriday
        /// <summary>
        /// Gets or sets a value indicating whether the recurrence should be applied on Fridays with specified week interval.
        /// </summary>
        public bool IsWeeklyFriday
        {
            get { return (bool)GetValue(IsWeeklyFridayProperty); }
            set { SetValue(IsWeeklyFridayProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsWeeklyFriday.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsWeeklyFridayProperty =
            DependencyProperty.Register("IsWeeklyFriday", typeof(bool), typeof(RecurrenceProperties), new PropertyMetadata(false));
        #endregion

        #region IsWeeklySaturday
        /// <summary>
        /// Gets or sets a value indicating whether the recurrence should be applied on Saturdays with specified week interval.
        /// </summary>
        public bool IsWeeklySaturday
        {
            get { return (bool)GetValue(IsWeeklySaturdayProperty); }
            set { SetValue(IsWeeklySaturdayProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsWeeklySaturday.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsWeeklySaturdayProperty =
            DependencyProperty.Register("IsWeeklySaturday", typeof(bool), typeof(RecurrenceProperties), new PropertyMetadata(false));
        #endregion

        #region MonthlyEveryNMonths
        /// <summary>
        /// Gets or sets the month interval on which recurrence has to be set.
        /// </summary>
        public int MonthlyEveryNMonths
        {
            get { return (int)GetValue(MonthlyEveryNMonthsProperty); }
            set { SetValue(MonthlyEveryNMonthsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MonthlyEveryNMonths.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MonthlyEveryNMonthsProperty =
            DependencyProperty.Register("MonthlyEveryNMonths", typeof(int), typeof(RecurrenceProperties), new PropertyMetadata(1));
        #endregion

        #region IsMonthlySpecific
        /// <summary>
        /// Gets or sets a value indicating whether the recurrence has to be set for particular month day i.e. MonthlySpecificMonthDay
        /// </summary>
        public bool IsMonthlySpecific
        {
            get { return (bool)GetValue(IsMonthlySpecificProperty); }
            set { SetValue(IsMonthlySpecificProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsMonthlySpecific.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsMonthlySpecificProperty =
            DependencyProperty.Register("IsMonthlySpecific", typeof(bool), typeof(RecurrenceProperties), new PropertyMetadata(true));
        #endregion

        #region MonthlySpecificMonthDay
        /// <summary>
        /// Gets or sets the day on which recurrence has to be set for every month.
        /// </summary>
        public int MonthlySpecificMonthDay
        {
            get { return (int)GetValue(MonthlySpecificMonthDayProperty); }
            set { SetValue(MonthlySpecificMonthDayProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MonthlySpecificMonthDay.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MonthlySpecificMonthDayProperty =
            DependencyProperty.Register("MonthlySpecificMonthDay", typeof(int), typeof(RecurrenceProperties), new PropertyMetadata(1));
        #endregion

        #region MonthlyNthWeek
        /// <summary>
        /// Gets or sets the week of month on which recurrence has to be set.
        /// </summary>
        public int MonthlyNthWeek
        {
            get { return (int)GetValue(MonthlyNthWeekProperty); }
            set { SetValue(MonthlyNthWeekProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MonthlyNthWeek.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MonthlyNthWeekProperty =
            DependencyProperty.Register("MonthlyNthWeek", typeof(int), typeof(RecurrenceProperties), new PropertyMetadata(0));
        #endregion

        #region MonthlyWeekDay
        /// <summary>
        /// Gets or sets the day of week on which monthly recurrence has to be set.
        /// </summary>
        public int MonthlyWeekDay
        {
            get { return (int)GetValue(MonthlyWeekDayProperty); }
            set { SetValue(MonthlyWeekDayProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MonthlyWeekDay.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MonthlyWeekDayProperty =
            DependencyProperty.Register("MonthlyWeekDay", typeof(int), typeof(RecurrenceProperties), new PropertyMetadata(1, OnMonthlyWeekDayChanged));

        private static void OnMonthlyWeekDayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RecurrenceProperties)
            {
                RecurrenceProperties properties = d as RecurrenceProperties;
                properties.MonthlyWeekDay = ((int)e.NewValue < 1) ? 1 : ((int)e.NewValue > 7) ? 7 : (int)e.NewValue;
            }
        }
        #endregion

        #region YearlyEveryNYears
        /// <summary>
        /// Gets or sets the year interval on which recurrence has to be set.
        /// </summary>
        public int YearlyEveryNYears
        {
            get { return (int)GetValue(YearlyEveryNYearsProperty); }
            set { SetValue(YearlyEveryNYearsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for YearlyEveryNYears.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty YearlyEveryNYearsProperty =
            DependencyProperty.Register("YearlyEveryNYears", typeof(int), typeof(RecurrenceProperties), new PropertyMetadata(1));
        #endregion

        #region IsYearlySpecific
        /// <summary>
        /// Gets or sets a value indicating whether the recurrence should be set based on specific year interval.
        /// </summary>
        public bool IsYearlySpecific
        {
            get { return (bool)GetValue(IsYearlySpecificProperty); }
            set { SetValue(IsYearlySpecificProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsYearlySpecific.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsYearlySpecificProperty =
            DependencyProperty.Register("IsYearlySpecific", typeof(bool), typeof(RecurrenceProperties), new PropertyMetadata(true));
        #endregion

        #region YearlySpecificMonth
        /// <summary>
        /// Gets or sets the specific month of year on which recurrence has to be set.
        /// </summary>
        public int YearlySpecificMonth
        {
            get { return (int)GetValue(YearlySpecificMonthProperty); }
            set { SetValue(YearlySpecificMonthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for YearlySpecificMonth.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty YearlySpecificMonthProperty =
            DependencyProperty.Register("YearlySpecificMonth", typeof(int), typeof(RecurrenceProperties), new PropertyMetadata(1, OnYearlySpecificMonthChanged));

        private static void OnYearlySpecificMonthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RecurrenceProperties)
            {
                RecurrenceProperties properties = d as RecurrenceProperties;
                properties.YearlySpecificMonth = ((int)e.NewValue < 1) ? 1 : ((int)e.NewValue > 12) ? 12 : (int)e.NewValue;
            }
        }
        #endregion

        #region YearlySpecificMonthDay
        /// <summary>
        /// Gets or sets the specific day of month on which yearly recurrence has to be set.
        /// </summary>
        public int YearlySpecificMonthDay
        {
            get { return (int)GetValue(YearlySpecificMonthDayProperty); }
            set { SetValue(YearlySpecificMonthDayProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for YearlySpecificMonthDay.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty YearlySpecificMonthDayProperty =
            DependencyProperty.Register("YearlySpecificMonthDay", typeof(int), typeof(RecurrenceProperties), new PropertyMetadata(1, OnYearlySpecificMonthDayChanged));

        private static void OnYearlySpecificMonthDayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RecurrenceProperties)
            {
                RecurrenceProperties properties = d as RecurrenceProperties;
                properties.YearlySpecificMonthDay = ((int)e.NewValue < 1) ? 1 : (int)e.NewValue;
            }
        }
        #endregion

        #region YearlyNthWeek
        /// <summary>
        /// Gets or sets the week of year on which recurrence has to be set.
        /// </summary>
        public int YearlyNthWeek
        {
            get { return (int)GetValue(YearlyNthWeekProperty); }
            set { SetValue(YearlyNthWeekProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for YearlyNthWeek.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty YearlyNthWeekProperty =
            DependencyProperty.Register("YearlyNthWeek", typeof(int), typeof(RecurrenceProperties), new PropertyMetadata(0));
        #endregion

        #region YearlyWeekDay
        /// <summary>
        /// Gets or sets the day of week on which yearly recurrence has to be set.
        /// </summary>
        public int YearlyWeekDay
        {
            get { return (int)GetValue(YearlyWeekDayProperty); }
            set { SetValue(YearlyWeekDayProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for YearlyWeekDay.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty YearlyWeekDayProperty =
            DependencyProperty.Register("YearlyWeekDay", typeof(int), typeof(RecurrenceProperties), new PropertyMetadata(1, OnYearlyWeekDayChanged));

        private static void OnYearlyWeekDayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RecurrenceProperties)
            {
                RecurrenceProperties properties = d as RecurrenceProperties;
                properties.YearlyWeekDay = ((int)e.NewValue < 1) ? 1 : ((int)e.NewValue > 7) ? 7 : (int)e.NewValue;
            }
        }
        #endregion

        #region YearlyGenericMonth
        /// <summary>
        /// Gets or sets the generic month of year on which recurrence has to be set.
        /// </summary>
        public int YearlyGenericMonth
        {
            get { return (int)GetValue(YearlyGenericMonthProperty); }
            set { SetValue(YearlyGenericMonthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for YearlyGenericMonth.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty YearlyGenericMonthProperty =
            DependencyProperty.Register("YearlyGenericMonth", typeof(int), typeof(RecurrenceProperties), new PropertyMetadata(1, OnYearlyGenericMonthChanged));

        private static void OnYearlyGenericMonthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RecurrenceProperties)
            {
                RecurrenceProperties properties = d as RecurrenceProperties;
                properties.YearlyGenericMonth = ((int)e.NewValue < 1) ? 1 : ((int)e.NewValue > 12) ? 12 : (int)e.NewValue;
            }
        }
        #endregion

        #endregion
    }
}
