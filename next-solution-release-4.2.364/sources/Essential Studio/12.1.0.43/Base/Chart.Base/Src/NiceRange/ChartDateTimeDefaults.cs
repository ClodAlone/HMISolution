#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Syncfusion.Documentation;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Contains the methods to return the default information of calendar.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal interface IChartDateTimeDefaults
    {
        /// <summary>
        /// Gets the calendar.
        /// </summary>
        /// <returns>Returns calender.</returns>
        /// <internalonly/>
        Calendar GetCalendar();

        /// <summary>
        /// Gets the days in year.
        /// </summary>
        /// <returns></returns>
        /// <internalonly/>
        int GetDaysInYear();

        /// <summary>
        /// Gets the days in month.
        /// </summary>
        /// <returns></returns>
        /// <internalonly/>
        int GetDaysInMonth();

        /// <summary>
        /// Gets the min days in month.
        /// </summary>
        /// <returns>Gets minimum days in week.</returns>
        /// <internalonly/>
        int GetMinDaysInMonth();

        /// <summary>
        /// Gets the days in week.
        /// </summary>
        /// <returns>Returns no of days in week.</returns>
        /// <internalonly/>
        int GetDaysInWeek();

        /// <summary>
        /// Gets the first day of week.
        /// </summary>
        /// <returns>Gets first day of week.</returns>
        /// <internalonly/>
        DayOfWeek GetFirstDayOfWeek();
    }

    /// <summary>
    /// Implements the <see cref="IChartDateTimeDefaults"/> interface;
    /// </summary>
    internal class ChartDateTimeDefaults : IChartDateTimeDefaults
    {
        #region Constants
        public const int DaysInYear = 365;
        public const int DaysInMonth = 31;
        public const int MinDaysInMonth = 28;
        public const int DaysInWeek = 7;
        public const DayOfWeek FirstDayOfWeek = DayOfWeek.Sunday;
        #endregion

        #region Members
        private Calendar calendar = new GregorianCalendar();
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the calendar.
        /// </summary>
        /// <returns>Returns calender object.</returns>
        /// <internalonly/>
        public virtual Calendar GetCalendar()
        {
            return Thread.CurrentThread.CurrentUICulture.Calendar;
        }

        /// <summary>
        /// Gets the days in year.
        /// </summary>
        /// <returns>Returns no of days in year.</returns>
        /// <internalonly/>
        public virtual int GetDaysInYear()
        {
            return ChartDateTimeDefaults.DaysInYear;
        }

        /// <summary>
        /// Gets the days in month.
        /// </summary>
        /// <returns>Returns the no of days in month.</returns>
        /// <internalonly/>
        public virtual int GetDaysInMonth()
        {
            return ChartDateTimeDefaults.DaysInMonth;
        }

        /// <summary>
        /// Gets the min days in month.
        /// </summary>
        /// <returns>Returns no of days in month.</returns>
        /// <internalonly/>
        public virtual int GetMinDaysInMonth()
        {
            return ChartDateTimeDefaults.MinDaysInMonth;
        }

        /// <summary>
        /// Gets the days in week.
        /// </summary>
        /// <returns>Returns no of days in week.</returns>
        /// <internalonly/>
        public virtual int GetDaysInWeek()
        {
            return ChartDateTimeDefaults.DaysInWeek;
        }

        /// <summary>
        /// Gets the first day of week.
        /// </summary>
        /// <returns>Returns first day of the week.</returns>
        /// <internalonly/>
        public virtual DayOfWeek GetFirstDayOfWeek()
        {
            return ChartDateTimeDefaults.FirstDayOfWeek;
        }
        #endregion
    }
}