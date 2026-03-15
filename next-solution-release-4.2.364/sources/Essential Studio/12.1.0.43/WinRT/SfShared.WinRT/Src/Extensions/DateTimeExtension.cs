#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if WINDOWS_PHONE||WINDOWS_PHONE_7
namespace Syncfusion.WP.Controls
#else
namespace Syncfusion.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a class for the DateTime Extension
    /// </summary>
    public static class DateTimeExtension 
    {
        /// <summary>
        /// Calculates each day in the given month
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static IEnumerable<DateTime> EachDayInMonth(this DateTime datetime)
        {
            int days = DateTime.DaysInMonth(datetime.Year, datetime.Month);
            datetime = datetime.AddMonths(-1).LastDay();

            for (int i = 0; i < days; i++)
            {
                datetime = datetime.AddDays(1);
                yield return datetime;
            }
        }

        /// <summary>
        /// Returns the first day of the given DateTime
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime FirstDay(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1);
        }

        /// <summary>
        /// Returns the last day of the given DateTime
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static int StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }
            return diff;
        }

        /// <summary>
        /// Returns the last day of the given DateTime
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime LastDay(this DateTime dateTime)
        {
            DateTime lastDay = dateTime.FirstDay();
            lastDay = lastDay.AddMonths(1).AddDays(-1);
            return lastDay;
        }


        /// <summary>
        /// Returns the DateTime of the given object
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this Object dateTime)
        {
            DateTime value;
            DateTime.TryParse(dateTime.ToString(), out value);
            return value;
        }
    }
}
