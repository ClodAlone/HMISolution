#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Globalization;

namespace Syncfusion.UI.Xaml.Schedule
{
    /// <summary>
    /// Contains date time extension methods.
    /// </summary>
    public static class DateTimeExtensions
    {
        #region Static Fields

        static readonly Calendar calendar = CultureInfo.CurrentCulture.Calendar;

        #endregion

        #region Methods

        #region Find Starting Week

        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-1 * diff).Date;
        }

        #endregion

        #region Add Time Span

        public static DateTime AddTimeSpan(this DateTime dt, TimeSpan timeSpan)
        {
            return dt.AddHours(timeSpan.Hours).AddMinutes(timeSpan.Minutes).AddSeconds(timeSpan.Seconds).AddMilliseconds(timeSpan.Milliseconds);
        }

        #endregion

        #region Subtract days

        public static DateTime SubractDays(this DateTime dt, int days)
        {
            return dt.Subtract(new TimeSpan(days, 0, 0, 0));
        }

        #endregion

        #region GetWeekOfMonth & GetWeekOfYear

        public static int GetWeekOfMonth(this DateTime time)
        {
            var first = new DateTime(time.Year, time.Month, 1);
            return time.GetWeekOfYear() - first.GetWeekOfYear() + 1;
        }

        static int GetWeekOfYear(this DateTime time)
        {

            return calendar.GetWeekOfYear(time, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        }

        #endregion

        #region WeeksInMonth

        public static int WeeksInMonth(this DateTime time)
        {
            int year = time.Year;
            int month = time.Month;
            const DayOfWeek wkstart = DayOfWeek.Monday;
            var first = new DateTime(year, month, 1);
            var firstwkday = (int)first.DayOfWeek;
            const int otherwkday = (int)wkstart;
            int offset = ((otherwkday + 7) - firstwkday) % 7;
            double weeks = (DateTime.DaysInMonth(year, month) - offset) / 7d;
            return (int)Math.Ceiling(weeks);
        }

        #endregion

        #endregion
    }
}
