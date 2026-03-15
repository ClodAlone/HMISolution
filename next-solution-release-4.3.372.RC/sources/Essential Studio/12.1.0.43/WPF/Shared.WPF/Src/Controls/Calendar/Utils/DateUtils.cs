// <copyright file="DateUtils.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Globalization;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Represents date.
    /// </summary>
    public struct Date : IEquatable<Date>, IComparable<Date>
    {
        #region Private members

        /// <summary>
        /// Year component.
        /// </summary>
        private int m_year;

        /// <summary>
        /// Month component.
        /// </summary>
        private int m_month;

        /// <summary>
        /// Day component.
        /// </summary>
        private int m_day;

        #endregion Private members

        #region Properties

        /// <summary>
        /// Gets or sets the year component.
        /// </summary>
        public int Year
        {
            get
            {
                return m_year;
            }

            set
            {
                m_year = value;
            }
        }

        /// <summary>
        /// Gets or sets the month component.
        /// </summary>
        public int Month
        {
            get
            {
                return m_month;
            }

            set
            {
                m_month = value;
            }
        }

        /// <summary>
        /// Gets or sets the day component.
        /// </summary>
        public int Day
        {
            get
            {
                return m_day;
            }

            set
            {
                m_day = value;
            }
        }

        #endregion Properties

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the Date structure.
        /// </summary>
        /// <param name="year">Year component of date.</param>
        /// <param name="month">Month component of date.</param>
        /// <param name="day">Day component of date.</param>
        public Date(int year, int month, int day)
        {
            m_year = year;
            m_month = month;
            m_day = day;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Date"/> struct.
        /// </summary>
        /// <param name="date">The current date.</param>
        /// <param name="calendar">The calendar.</param>
        public Date(DateTime date, Calendar calendar)
        {
            m_year = calendar.GetYear(date);
            m_month = calendar.GetMonth(date);
            m_day = calendar.GetDayOfMonth(date);
        }

        #endregion Initialization

        #region Public methods

        /// <summary>
        /// Toes the date time.
        /// </summary>
        /// <param name="calendar">The calendar control.</param>
        /// <returns>Return the DateTime</returns>
        public DateTime ToDateTime(Calendar calendar)
        {
            if (calendar == null)
            {
                throw new ArgumentNullException("Calendar object cannot be null");
            }

            return new DateTime(m_year, m_month, m_day, calendar);
        }

        /// <summary>
        /// Adds specified number of months to the date.
        /// </summary>
        /// <param name="month">Number of months. Can be negative or positive.</param>
        /// <returns>The computed date.</returns>
        public Date AddMonthToDate(int month)
        {
            Date result = this;
            int months = 0;
            int years = 0;
            int count = Math.Abs(month);

            if (count > 12)
            {
                years = month / 12;
                months = month % 12;
            }
            else
            {
                months = month;
            }

            int tempMonth = result.Month + months;
            result.Year += years;

            if (tempMonth > 12)
            {
                result.Year += tempMonth / 12;
                result.Month = tempMonth % 12;
            }

            if (tempMonth < 1)
            {
                result.Month = 12 + tempMonth;
                result.Year--;
            }

            if (tempMonth >= 1 && tempMonth <= 12)
            {
                result.Month = tempMonth;
            }

            result.Day = month > 0 ? 1 : 31;
            return result;
        }

        #endregion Public methods

        #region Overrides

        /// <summary>
        /// Returns a value indicating whether this instance is equal to
        /// a specified object.
        /// </summary>
        /// <param name="obj">Object to be compared.</param>
        /// <returns>
        /// True if obj and this instance represent the same date; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is Date && this == (Date)obj;
        }

        /// <summary>
        /// Converts the value of the current <see cref="Date"/> object to its
        /// equivalent string representation.
        /// </summary>
        /// <returns>
        /// Date string representation.
        /// </returns>
        public override string ToString()
        {
            string result = Day.ToString() + " " + Month.ToString() + " " + Year.ToString();
            return result;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// Hash code of this instance.
        /// </returns>
        public override int GetHashCode()
        {
            return m_day ^ m_month ^ m_year;
        }

        #endregion Overrides

        #region Operators

        /// <summary>
        /// Determines whether two specified instances of <see cref="Date"/> are equal.
        /// </summary>
        /// <param name="a">First operand of comparison.</param>
        /// <param name="b">Second operand of comparison.</param>
        /// <returns>True if a and b represent the same date; otherwise, false.</returns>
        public static bool operator ==(Date a, Date b)
        {
            return a.Year == b.Year && a.Month == b.Month && a.Day == b.Day;
        }

        /// <summary>
        /// Determines whether two specified instances of <see cref="Date"/> are not
        /// equal.
        /// </summary>
        /// <param name="a">First operand of comparison.</param>
        /// <param name="b">Second operand of comparison.</param>
        /// <returns>
        /// True if a and b do not represent the same date; otherwise,
        /// false.
        /// </returns>
        public static bool operator !=(Date a, Date b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Determines whether one specified <see cref="Date"/> is greater than another
        /// specified <see cref="Date"/>.
        /// </summary>
        /// <param name="a">First operand of comparison.</param>
        /// <param name="b">Second operand of comparison.</param>
        /// <returns>
        /// True if a is greater than b; otherwise, false.
        /// </returns>
        public static bool operator >(Date a, Date b)
        {
            if (a.Year > b.Year)
            {
                return true;
            }

            if (a.Year < b.Year)
            {
                return false;
            }
            else
            {
                if (a.Month > b.Month)
                {
                    return true;
                }

                if (a.Month < b.Month)
                {
                    return false;
                }
                else
                {
                    if (a.Day > b.Day)
                    {
                        return true;
                    }

                    if (a.Day < b.Day)
                    {
                        return false;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether one specified <see cref="Date"/> is less than another
        /// specified <see cref="Date"/>.
        /// </summary>
        /// <param name="a">First operand of comparison.</param>
        /// <param name="b">Second operand of comparison.</param>
        /// <returns>True if a is less than b; otherwise, false.</returns>
        public static bool operator <(Date a, Date b)
        {
            if (!(a > b) && a != b)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether one specified <see cref="Date"/> is greater than or
        /// equal to another specified <see cref="Date"/>.
        /// </summary>
        /// <param name="a">First operand of comparison.</param>
        /// <param name="b">Second operand of comparison.</param>
        /// <returns>True if a is less than b; otherwise, false. </returns>
        public static bool operator >=(Date a, Date b)
        {
            if (a > b || a == b)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether one specified <see cref="Date"/> is less than or equal
        /// to another specified <see cref="Date"/>.
        /// </summary>
        /// <param name="a">First operand of comparison.</param>
        /// <param name="b">Second operand of comparison.</param>
        /// <returns>True if a is less than b; otherwise, false. </returns>
        public static bool operator <=(Date a, Date b)
        {
            if (a < b || a == b)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion Operators

        #region IEquatable<Date> Members

        /// <summary>
        /// Gets a value indicating whether the current date is equal to other date.
        /// </summary>
        /// <param name="other">The date to compare the current date to.</param>
        /// <returns>True if the current date is equal to the given date; otherwise, false.</returns>
        public bool Equals(Date other)
        {
            return this == other;
        }

        #endregion IEquatable<Date> Members

        #region IComparable<Date> Members

        /// <summary>
        /// Compares current date with other date.
        /// </summary>
        /// <param name="other">The date to compare the current date with.</param>
        /// <returns>-1 if the current date is less than given date; 1 if the current date is more than given date; otherwise, 0.</returns>
        public int CompareTo(Date other)
        {
            if (this < other)
            {
                return -1;
            }

            if (this > other)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        #endregion IComparable<Date> Members
    }

    /// <summary>
    /// Visible date settings.
    /// </summary>
    public struct VisibleDate
    {
        #region Public members

        /// <summary>
        /// Represents visible year.
        /// </summary>
        public int VisibleYear;

        /// <summary>
        /// Represents visible Day.
        /// </summary>
        public int VisibleDay;

        /// <summary>
        /// Represents visible month.
        /// </summary>
        public int VisibleMonth;

        #endregion Public members

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the VisibleDate struct.
        /// </summary>
        /// <param name="year">Sets visible year.</param>
        /// <param name="month">Sets visible month.</param>
        public VisibleDate(int year, int month, int day)
        {
            VisibleYear = year;
            VisibleMonth = month;
            VisibleDay = day;
        }

        #endregion Initialization
    }

    /// <summary>
    /// Represents year range.
    /// </summary>
    public struct YearsRange
    {
        /// <summary>
        /// Start of the year range.
        /// </summary>
        public int StartYear;

        /// <summary>
        /// End of the year range.
        /// </summary>
        public int EndYear;

        /// <summary>
        /// Initializes a new instance of the <see cref="YearsRange"/> struct.
        /// </summary>
        /// <param name="startYear">Start of the year range.</param>
        /// <param name="endYear">End of the year range.</param>
        public YearsRange(int startYear, int endYear)
        {
            StartYear = startYear;
            EndYear = endYear;
        }
    }

    /// <summary>
    /// Used for working with Date.
    /// </summary>
    internal class DateUtils
    {
        #region Initilization

        /// <summary>
        /// Prevents a default instance of the <see cref="DateUtils"/> class from being created.
        /// </summary>
        private DateUtils()
        {
        }

        #endregion Initilization

        #region Implementation

        /// <summary>
        /// Calculates correct month of the year, after adding some
        /// number of months.
        /// </summary>
        /// <param name="month">Current month.</param>
        /// <param name="param">Number of months to be added.</param>
        /// <returns>
        /// Month after adding.
        /// </returns>
        public static int AddMonth(int month, int param)
        {
            int result = 0;
            int tempMonth = month + param;

            if (tempMonth > 12)
            {
                result = tempMonth % 12;
            }

            if (tempMonth < 1)
            {
                result = 12 + tempMonth;
            }

            if (tempMonth >= 1 && tempMonth <= 12)
            {
                result = tempMonth;
            }

            return result;
        }

        /// <summary>
        /// Returns <see cref="DateTime"/> object, in which current day is a first day
        /// of month.
        /// </summary>
        /// <param name="year">Year value.</param>
        /// <param name="month">Month value.</param>
        /// <param name="calendar">The <see cref="System.Globalization.Calendar"/> used for
        /// getting the <see cref="DateTime"/> object.</param>
        /// <returns>
        /// The <see cref="DateTime"/> object.
        /// </returns>
        public static DateTime GetFirstDayOfMonth(int year, int month, Calendar calendar)
        {
            DateTime dateMonthStart = calendar.ToDateTime(year, month, 1, 0, 0, 0, 0);
            return dateMonthStart;
        }

        /// <summary>
        /// Generates calendar matrix that represents the current month.
        /// </summary>
        /// <param name="month">The current month.</param>
        /// <param name="year">The current year.</param>
        /// <param name="format">The <see cref="System.Globalization.DateTimeFormatInfo"/> object used for
        /// generating the calendar matrix.</param>
        /// <param name="calendar">The <see cref="System.Globalization.Calendar"/> object used for
        /// generating the calendar matrix.</param>
        /// <returns>array of integer which contains matrix points.</returns>
        public static int[,] GenerateMatrix(int month, int year, DateTimeFormatInfo format, Calendar calendar)
        {
            DateTime dateMonthStart = GetFirstDayOfMonth(year, month, calendar);
            DayOfWeek dw = calendar.GetDayOfWeek(dateMonthStart);
            int[,] matrix = new int[6, 7];
            int firstDayOfMonth = (int)dw;
            int firstDayOfWeek = (int)format.FirstDayOfWeek;
            int iDaysInMonth = calendar.GetDaysInMonth(year, month);
            int first = (6 + firstDayOfMonth - firstDayOfWeek) % 7 + 1;
            int start = first;

            for (int i = 0, k = 1; i < 6; i++)
            {
                if (i > 0)
                {
                    start = 0;
                }

                for (int j = start; j < 7; j++, k++)
                {
                    matrix[i, j] = k;
                }
            }

            start = first;

            for (int i = 0, k = 0, c = 1; i < 6; i++)
            {
                if (i > 0)
                {
                    start = 0;
                }

                for (int j = start; j < 7; j++, k++)
                {
                    if (k >= iDaysInMonth)
                    {
                        matrix[i, j] = c;
                        c++;
                    }
                }
            }

            start = first;

            if (month == 1)
            {
                year--;
            }

            Date minDate = new Date(calendar.MinSupportedDateTime, calendar);
            Date nextDate = new Date(year, AddMonth(month, -1), 1);

            if (!(nextDate < minDate))
            {
                int daysInPrevMonth = calendar.GetDaysInMonth(year, AddMonth(month, -1));

                for (int i = 0, k = daysInPrevMonth - first + 1; i < first; i++, k++)
                {
                    matrix[0, i] = k;
                }
            }

            return matrix;
        }

        #endregion Implementation
    }
}