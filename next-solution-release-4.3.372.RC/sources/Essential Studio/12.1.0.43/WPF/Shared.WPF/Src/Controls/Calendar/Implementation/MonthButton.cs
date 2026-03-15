// <copyright file="MonthButton.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Calendar = System.Globalization.Calendar;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Represents month name header.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class MonthButton : ContentControl
    {
        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="MonthButton"/> class.  It overrides some dependency properties.
        /// </summary>
        static MonthButton()
        {
            // This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            // This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MonthButton), new FrameworkPropertyMetadata(typeof(MonthButton)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MonthButton"/> class.
        /// </summary>
        public MonthButton()
        {
        }

        #endregion Initialization

        #region Implementation

        /// <summary>
        /// Initializes MonthButton instance.
        /// </summary>
        /// <param name="data">Current date.</param>
        /// <param name="calendar">Current calendar.</param>
        /// <param name="culture">The <see cref="System.Globalization.DateTimeFormatInfo"/> object that belongs to the current culture.</param>
        /// <param name="isAbbreviated">Indicates whether month name should be abbreviated.</param>
        /// <param name="mode">Visual mode of the <see cref="Syncfusion.Windows.Shared.CalendarEdit"/> control.</param>
        protected internal void Initialize(VisibleDate data, Calendar calendar, CultureInfo culture, bool isAbbreviated, CalendarVisualMode mode)
        {
            int imonth = data.VisibleMonth;
            int iyear = data.VisibleYear;
            DateTimeFormatInfo format = culture.DateTimeFormat;
            Date minDate = new Date(calendar.MinSupportedDateTime, calendar);
            Date maxDate = new Date(calendar.MaxSupportedDateTime, calendar);

            if (mode == CalendarVisualMode.WeekNumbers)
            {
                Content = "Weeks in " + iyear.ToString();
            }

            if (mode == CalendarVisualMode.Days)
            {
                if (isAbbreviated)
                {
                    if (culture.Name == "ja-JP" || culture.Name == "zh-CN")
                    {
                        DateTime DT = new DateTime(iyear, imonth, 1);
                        char[] SplitDT = DT.ToString(format.YearMonthPattern, culture).ToCharArray();
                        Content = iyear.ToString() + SplitDT[4] + " " + format.MonthNames[imonth - 1];
                    }
                    else
                        Content = format.AbbreviatedMonthNames[imonth - 1] + " " + iyear.ToString();
                }
                else
                {
                    if (culture.Name == "ja-JP" || culture.Name == "zh-CN")
                    {
                        DateTime DT = new DateTime(iyear, imonth, 1);
                        Content = DT.ToString(format.YearMonthPattern, culture);
                    }
                    else
                        Content = format.MonthNames[imonth - 1] + " " + iyear.ToString();
                }
            }

            if (mode == CalendarVisualMode.Months)
            {
                Content = iyear.ToString();
            }

            if (mode == CalendarVisualMode.Years)
            {
                int startYear = data.VisibleYear;
                int endYear;

                while (startYear % 10 != 0)
                {
                    startYear--;
                }

                endYear = startYear + 9;

                if (startYear < minDate.Year)
                {
                    startYear = minDate.Year;
                }

                if (endYear > maxDate.Year)
                {
                    endYear = maxDate.Year;
                }

                Content = startYear + "-" + endYear;
            }

            if (mode == CalendarVisualMode.YearsRange)
            {
                int startYear = data.VisibleYear;
                int endYear;

                while (startYear % 10 != 0)
                {
                    startYear--;
                }

                while (startYear % 100 != 0)
                {
                    startYear -= 10;
                }

                endYear = startYear + 99;
                //// TODO Modify algorithm here !
                if (startYear < minDate.Year)
                {
                    startYear = minDate.Year;
                }

                if (endYear > maxDate.Year)
                {
                    endYear = maxDate.Year;
                }

                Content = startYear + "-" + endYear;
            }
        }

        #endregion Implementation
    }
}