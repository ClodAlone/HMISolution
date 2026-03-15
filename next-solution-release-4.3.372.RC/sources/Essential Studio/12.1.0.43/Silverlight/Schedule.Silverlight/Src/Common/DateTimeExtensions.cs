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

namespace Syncfusion.Windows.Controls.Schedule
{
#if SyncfusionFramework4_0 && !SILVERLIGHT
    /// <summary>
    ///  class that holds extensions for DateTime
    /// </summary>
    [System.ComponentModel.DesignTimeVisible(false)]
#endif

    public static class DateTimeExtensions
    {
        /// <summary>
        ///  the method which returns start  of the week in DateTime
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="startOfWeek"></param>
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-1 * diff).Date;
        }

        /// <summary>
        ///  method to AddTimeSpan
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="timeSpan"></param>
        public static DateTime AddTimeSpan(this DateTime dt, TimeSpan timeSpan)
        {
            return dt.AddHours(timeSpan.Hours).AddMinutes(timeSpan.Minutes).AddSeconds(timeSpan.Seconds).AddMilliseconds(timeSpan.Milliseconds);
        }

        /// <summary>
        /// method to subtract days
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="days"></param>
        public static DateTime SubractDays(this DateTime dt, int days)
        {
            return dt.Subtract(new TimeSpan(days, 0, 0, 0));
        }
    }
}
