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

namespace Syncfusion.Windows.Diagram
{
    internal static class InternalExtensions
    {
        public static double ToPixel(this TimeSpan time, DateTimeSettings factor)
        {
            return factor.PixelUnit * time.Ticks / factor.TimeSpan.Ticks;
        }

        public static TimeSpan ToTimeSpan(this double pixel, DateTimeSettings factor)
        {
            return new TimeSpan((long)(factor.TimeSpan.Ticks * pixel / factor.PixelUnit));
        }

        public static double ToPixel(this DateTime date, DateTimeSettings factor)
        {
            return  (date - factor.OriginDateX).ToPixel(factor);
        }

        public static DateTime ToDateTime(this double pixel, DateTimeSettings factor)
        {
            return factor.OriginDateX + pixel.ToTimeSpan(factor);
        }

        public static long TotalYears(this TimeSpan timeSpan)
        {
            return ((long)timeSpan.TotalDays % 366);
        }

        public static long TotalMonths(this TimeSpan timeSpan)
        {
            return ((long)timeSpan.TotalDays % 30);
        }

        public static string GetStringFormat(this DateTimeSettings factor)
        {
            TimeSpan ts = ((double)50).ToTimeSpan(factor);
            if (ts.TotalDays >= 1)
            {
                //Year
                if (ts.TotalDays > 364)
                {
                    return "{0:yyyy}";
                }
                //Month & Year
                else if (ts.TotalDays > 183)
                {
                    return "{0:MMM/yy}";
                }
                //Date & Month
                else
                {
                    return "{0:MMM/dd H}";
                }
            }
            //HH:MM
            else if (ts.TotalHours > 1)
            {
                return "{0:H:mm}";
            }
            //HH:MM
            else if (ts.TotalMinutes > 1)
            {
                return "{0:H:mm}";
            }
            //HH:MM:SS
            else if (ts.TotalSeconds > 1)
            {
                return "{0:H:mm:ss}";
            }
            //SS.mm
            else if (ts.TotalMilliseconds > 1)
            {
                return "{0:mm.ff}";
            }
            else
            {
                return "{0:ffff}";
            }
        }
    }
}
