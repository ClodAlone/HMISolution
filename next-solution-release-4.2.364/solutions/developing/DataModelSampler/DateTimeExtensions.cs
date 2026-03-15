using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataModelSampler
{
    public static class DateTimeExtensions
    {
        // Methods
        public static DateTime EndOfDay(this DateTime dt)
        {
            return dt.StartOfDay().AddDays(1.0);
        }

        public static DateTime EndOfMonth(this DateTime dt)
        {
            return dt.StartOfMonth().AddMonths(1);
        }

        public static DateTime EndOfNextDay(this DateTime dt)
        {
            return dt.StartOfNextDay().AddDays(1.0);
        }

        public static DateTime EndOfNextMonth(this DateTime dt)
        {
            return dt.StartOfNextMonth().AddMonths(1);
        }

        public static DateTime EndOfNextYear(this DateTime dt)
        {
            return dt.StartOfNextYear().AddYears(1);
        }

        public static DateTime EndOfPreviousDay(this DateTime dt)
        {
            return dt.StartOfDay();
        }

        public static DateTime EndOfPreviousMonth(this DateTime dt)
        {
            return dt.StartOfMonth();
        }

        public static DateTime EndOfPreviousYear(this DateTime dt)
        {
            return dt.StartOfYear();
        }

        public static DateTime EndOfYear(this DateTime dt)
        {
            return dt.StartOfYear().AddYears(1);
        }

        public static DateTime StartOfDay(this DateTime dt)
        {
            return dt.Date;
        }

        public static DateTime StartOfMonth(this DateTime dt)
        {
            return dt.Date.AddDays((double)((-1 * dt.Day) + 1));
        }

        public static DateTime StartOfNextDay(this DateTime dt)
        {
            return dt.EndOfDay();
        }

        public static DateTime StartOfNextMonth(this DateTime dt)
        {
            return dt.EndOfMonth();
        }

        public static DateTime StartOfNextYear(this DateTime dt)
        {
            return dt.EndOfYear();
        }

        public static DateTime StartOfPreviousDay(this DateTime dt)
        {
            return dt.StartOfDay().AddDays(-1.0);
        }

        public static DateTime StartOfPreviousMonth(this DateTime dt)
        {
            return dt.StartOfMonth().AddMonths(-1);
        }

        public static DateTime StartOfPreviousYear(this DateTime dt)
        {
            return dt.StartOfYear().AddYears(-1);
        }

        public static DateTime StartOfYear(this DateTime dt)
        {
            return new DateTime(dt.Year, 1, 1);
        }
    }

}
