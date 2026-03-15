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
using Syncfusion.Documentation;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Implements methods to compute the 'nice' data range/
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal sealed class ChartDateTimeNiceRangeMaker
    {
        #region Internal types
        /// <summary>
        /// Resperents the seek direction.
        /// </summary>
        /// <internalonly/>
        private enum SeekDirection
        {
            /// <internalonly/>
            Forward,

            /// <internalonly/>
            Reverse,
        }

        /// <summary>
        /// Implemenets the methods to compute the 'nice' weeks range.
        /// </summary>
        /// <internalonly/>
        private class ImplWeeks : NiceRangeMaker
        {
            #region Implementation
            /// <summary>
            /// Simple logic for creating 'nice' numbers that are close to the numbers passed in.
            /// </summary>
            /// <param name="val">Value whose equivalent 'nice' number is to be found.</param>
            /// <returns>Returns double.</returns>
            protected override double MakeNiceNumber(double val)
            {
                if (val < 10)
                {
                    return 10;
                }
                /*

                      else if(val < 20)
                          return 20;

                      else if(val < 25)
                          return 25;*/
                else
                {
                    return 100;
                }
            }
            #endregion
        }
        #endregion

        #region Members
        private INiceRangeMaker m_niceRangeMaker;
        private INiceRangeMaker m_niceWeeksRangeMaker;
        private Calendar m_calendar;
        private IChartDateTimeDefaults m_chartDateTimeDefaults;
        private ChartDateTimeIntervalType _desiredIntervalType = ChartDateTimeIntervalType.Auto;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartDateTimeNiceRangeMaker"/> class.
        /// </summary>
        /// <param name="chartDateTimeDefaults">The chart date time defaults.</param>
        /// <param name="niceRangeMaker">The nice range maker.</param>
        /// <internalonly/>
        public ChartDateTimeNiceRangeMaker(IChartDateTimeDefaults chartDateTimeDefaults, INiceRangeMaker niceRangeMaker)
        {
            m_chartDateTimeDefaults = chartDateTimeDefaults;
            m_niceRangeMaker = niceRangeMaker;
            m_niceWeeksRangeMaker = new ImplWeeks();

            m_calendar = chartDateTimeDefaults.GetCalendar();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartDateTimeNiceRangeMaker"/> class.
        /// </summary>
        /// <param name="chartDateTimeDefaults">The chart date time defaults.</param>
        /// <internalonly/>
        public ChartDateTimeNiceRangeMaker(IChartDateTimeDefaults chartDateTimeDefaults)
            : this(chartDateTimeDefaults, new NiceRangeMaker())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartDateTimeNiceRangeMaker"/> class.
        /// </summary>
        /// <internalonly/>
        public ChartDateTimeNiceRangeMaker()
            : this(new ChartDateTimeDefaults(), new NiceRangeMaker())
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the defaults.
        /// </summary>
        /// <value>The defaults.</value>
        /// <internalonly/>
        public IChartDateTimeDefaults Defaults
        {
            get
            {
                return m_chartDateTimeDefaults;
            }
        }

        /// <summary>
        /// Gets the calendar.
        /// </summary>
        /// <value>The calendar.</value>
        /// <internalonly/>
        public Calendar Calendar
        {
            get
            {
                return m_calendar;
            }
        }

        /// <summary>
        /// Gets or sets the desired intervals count.
        /// </summary>
        /// <value>The desired intervals count.</value>
        /// <internalonly/>
        public int DesiredIntervals
        {
            get
            {
                return m_niceRangeMaker.DesiredIntervals;
            }

            set
            {
                m_niceRangeMaker.DesiredIntervals = value;
                m_niceWeeksRangeMaker.DesiredIntervals = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of the range padding.
        /// </summary>
        /// <value>The type of the range padding.</value>
        /// <internalonly/>
        public ChartAxisRangePaddingType RangePaddingType
        {
            get
            {
                return m_niceRangeMaker.RangePaddingType;
            }

            set
            {
                m_niceRangeMaker.RangePaddingType = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether zero is "forced".
        /// </summary>
        /// <value><c>true</c> if zero is "forced"; otherwise, <c>false</c>.</value>
        /// <internalonly/>
        public bool ForceZero
        {
            get
            {
                return m_niceRangeMaker.ForceZero;
            }

            set
            {
                m_niceRangeMaker.ForceZero = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether zero is preferred.
        /// </summary>
        /// <value><c>true</c> if zero is preferred; otherwise, <c>false</c>.</value>
        /// <internalonly/>
        public bool PreferZero
        {
            get
            {
                return m_niceRangeMaker.PreferZero;
            }

            set
            {
                m_niceRangeMaker.PreferZero = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of the desired interval.
        /// </summary>
        /// <value>The type of the desired interval.</value>
        /// <internalonly/>
        public ChartDateTimeIntervalType DesiredIntervalType
        {
            get
            {
                return this._desiredIntervalType;
            }

            set
            {
                this._desiredIntervalType = value;
            }
        }
        #endregion

        #region Public metdhos
        /// <summary>
        /// Makes the nice range.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>Returns ChartDateTimeRange instance.</returns>
        public ChartDateTimeRange MakeNiceRange(DateTime start, DateTime end)
        {
            ChartDateTimeIntervalType intervalType = _desiredIntervalType;

            if (intervalType == ChartDateTimeIntervalType.Auto)
            {
                intervalType = this.CalculateIntervalType(end.Subtract(start));
            }

            return this.MakeNiceRange(start, end, intervalType, m_niceRangeMaker.RangePaddingType);
        }

        /// <summary>
        /// Makes the nice range.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="type">The type.</param>
        /// <returns>Returns ChartDateTimeRange instance.</returns>
        /// <internalonly/>
        public ChartDateTimeRange MakeNiceRange(DateTime start, DateTime end, ChartDateTimeIntervalType type)
        {
            return MakeNiceRange(start, end, type, m_niceRangeMaker.RangePaddingType);
        }

        /// <summary>
        /// Makes the nice range.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="type">The type.</param>
        /// <param name="rangePaddingType">Type of the range padding.</param>
        /// <returns>Returns ChartDateTimeRange instance.</returns>
        /// <internalonly/>
        public ChartDateTimeRange MakeNiceRange(DateTime start, DateTime end, ChartDateTimeIntervalType type, ChartAxisRangePaddingType rangePaddingType)
        {
            if (end <= start)
            {
                throw new ArgumentOutOfRangeException("End needs to be later than start.");
            }

            // some extra padding
            TimeSpan diff = end - start;

            double padding = 0;
            if (rangePaddingType == ChartAxisRangePaddingType.Calculate)
                padding = (diff.TotalMilliseconds) / (this.DesiredIntervals);

            end = end.AddMilliseconds(padding);
            start = start.AddMilliseconds(-padding);

            switch (type)
            {
                case ChartDateTimeIntervalType.Years:
                    return this.MakeNiceYearsRange(start, end);

                case ChartDateTimeIntervalType.Months:
                    return this.MakeNiceMonthsRange(start, end);

                case ChartDateTimeIntervalType.Weeks:
                    return this.MakeNiceWeeksRange(start, end);

                case ChartDateTimeIntervalType.Days:
                    return this.MakeNiceDaysRange(start, end);

                case ChartDateTimeIntervalType.Hours:
                    return this.MakeNiceHoursRange(start, end);

                case ChartDateTimeIntervalType.Minutes:
                    return this.MakeNiceMinutesRange(start, end);

                case ChartDateTimeIntervalType.Seconds:
                    return this.MakeNiceSecondsRange(start, end);

                case ChartDateTimeIntervalType.MilliSeconds:
                    return this.MakeNiceMilliSecondsRange(start, end);
                default:
                    {
                        Trace.Fail("Invalid DateTimeIntervalType");
                        throw new ArgumentOutOfRangeException("Invalid DateTimeIntervalType");
                    }
            }
        }

        /// <summary>
        /// Makes the nice years range.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>Returns ChartDateTimeRange instance.</returns>
        /// <internalonly/>
        private ChartDateTimeRange MakeNiceYearsRange(DateTime start, DateTime end)
        {
            TimeSpan diff = end - start;

            DateTime niceStart = DateTime.MinValue;
            DateTime niceEnd = DateTime.MinValue;

            double niceInterval = 0;
            ChartDateTimeIntervalType niceIntervalType = ChartDateTimeIntervalType.Years;

            double min = start.Year;
            double max = end.Year;

            MinMaxInfo info = m_niceRangeMaker.MakeNiceRange(min, max + 1, m_niceRangeMaker.RangePaddingType);
            niceStart = new DateTime((int)info.Min, 1, 1, 0, 0, 0, 0, m_calendar);
            niceEnd = new DateTime((int)info.Max, 1, 1, 0, 0, 0, 0, m_calendar);
            niceInterval = info.Interval;

            return new ChartDateTimeRange(niceStart, niceEnd, niceInterval, niceIntervalType, m_calendar);
        }

        /// <summary>
        /// Makes the nice months range.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>Returns ChartDateTimeRange instance.</returns>
        /// <internalonly/>
        private ChartDateTimeRange MakeNiceMonthsRange(DateTime start, DateTime end)
        {
            TimeSpan diff = end - start;

            DateTime niceStart = DateTime.MinValue;
            DateTime niceEnd = DateTime.MinValue;

            double niceInterval = 0;
            ChartDateTimeIntervalType niceIntervalType = ChartDateTimeIntervalType.Months;

            m_niceRangeMaker.ForceZero = true;

            double min = 0;

            double max = (int)Math.Ceiling(diff.TotalDays / this.Defaults.GetMinDaysInMonth()); // approx number of months
            // based on approx min days in month

            MinMaxInfo info = m_niceRangeMaker.MakeNiceRange(min, max + 1, this.RangePaddingType);
            niceStart = new DateTime(start.Year, start.Month, 1, 0, 0, 0, 0, m_calendar);
            niceEnd = m_calendar.AddMonths(niceStart, (int)info.Max);
            niceInterval = info.Interval;

            return new ChartDateTimeRange(niceStart, niceEnd, niceInterval, niceIntervalType, m_calendar);
        }

        /// <summary>
        /// Makes the nice weeks range.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>Returns ChartDateTimeRange instance.</returns>
        /// <internalonly/>
        private ChartDateTimeRange MakeNiceWeeksRange(DateTime start, DateTime end)
        {
            TimeSpan diff = end - start;
            DateTime niceStart = DateTime.MinValue;
            DateTime niceEnd = DateTime.MinValue;

            m_niceRangeMaker.ForceZero = true;

            double min = 0;
            double max = (int)Math.Ceiling(diff.TotalDays / this.Defaults.GetDaysInWeek());

            MinMaxInfo info = m_niceWeeksRangeMaker.MakeNiceRange(min, max, this.RangePaddingType);

            niceStart = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0, 0, m_calendar);
            niceEnd = m_calendar.AddDays(niceStart, (int)Math.Ceiling(this.Defaults.GetDaysInWeek() * info.Max));

            niceStart = this.AdjustToWeekStart(niceStart, SeekDirection.Reverse);
            niceEnd = this.AdjustToWeekStart(niceEnd, SeekDirection.Forward);

            return new ChartDateTimeRange(niceStart, niceEnd, info.Interval,
                ChartDateTimeIntervalType.Weeks, m_calendar);
        }

        /// <summary>
        /// Makes the nice days range.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>Returns ChartDateTimeRange instance.</returns>
        /// <internalonly/>
        private ChartDateTimeRange MakeNiceDaysRange(DateTime start, DateTime end)
        {
            TimeSpan diff = end - start;

            DateTime niceStart = DateTime.MinValue;
            DateTime niceEnd = DateTime.MinValue;

            double niceInterval = 0;
            ChartDateTimeIntervalType niceIntervalType = ChartDateTimeIntervalType.Days;

            double min = 0;

            double max = (int)Math.Ceiling(diff.TotalDays);

            MinMaxInfo info = m_niceRangeMaker.MakeNiceRange(min - 1, max + 1, this.RangePaddingType);
            niceStart = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0, 0, m_calendar);
            niceStart -= new TimeSpan(1, 0, 0, 0, 0);

            niceEnd = m_calendar.AddDays(niceStart, (int)Math.Ceiling(info.Max));
            niceInterval = info.Interval;
            //      niceInterval = info.Max/(DesiredIntervals+1);

            return new ChartDateTimeRange(niceStart, niceEnd, niceInterval, niceIntervalType, m_calendar);
        }

        /// <summary>
        /// Makes the nice hours range.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>Returns ChartDateTimeRange instance.</returns>
        /// <internalonly/>
        private ChartDateTimeRange MakeNiceHoursRange(DateTime start, DateTime end)
        {
            TimeSpan diff = end - start;
            MinMaxInfo info = m_niceRangeMaker.MakeNiceRange(0, Math.Ceiling(diff.TotalHours), m_niceRangeMaker.RangePaddingType);
            DateTime zeroTime = new DateTime(start.Year, start.Month, start.Day, start.Hour, 0, 0, 0, m_calendar);
            DateTime niceStart = zeroTime.AddHours(info.min);
            DateTime niceEnd = zeroTime.AddHours(info.max);

            return new ChartDateTimeRange(niceStart, niceEnd, info.Interval, ChartDateTimeIntervalType.Hours, m_calendar);
                
        }

        /// <summary>
        /// Makes the nice minutes range.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>Returns ChartDateTimeRange instance.</returns>
        /// <internalonly/>
        private ChartDateTimeRange MakeNiceMinutesRange(DateTime start, DateTime end)
        {
            TimeSpan diff = end - start;
            MinMaxInfo info = m_niceRangeMaker.MakeNiceRange(0, Math.Ceiling(diff.TotalMinutes), m_niceRangeMaker.RangePaddingType);
            DateTime zeroTime = new DateTime(start.Year, start.Month, start.Day, start.Hour, start.Minute, 0, 0, m_calendar);
            DateTime niceStart = zeroTime.AddMinutes(info.min);
            DateTime niceEnd = zeroTime.AddMinutes(info.max);

            return new ChartDateTimeRange(niceStart, niceEnd, info.Interval, ChartDateTimeIntervalType.Minutes, m_calendar);               
        }

        /// <summary>
        /// Makes the nice seconds range.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>Returns ChartDateTimeRange instance.</returns>
        /// <internalonly/>
        private ChartDateTimeRange MakeNiceSecondsRange(DateTime start, DateTime end)
        {
            TimeSpan diff = end - start;
            MinMaxInfo info = m_niceRangeMaker.MakeNiceRange(0, Math.Ceiling(diff.TotalSeconds), m_niceRangeMaker.RangePaddingType);
            DateTime zeroTime = new DateTime(start.Year, start.Month, start.Day, start.Hour, start.Minute, start.Second, 0, m_calendar);
            DateTime niceStart = zeroTime.AddSeconds(info.min);
            DateTime niceEnd = zeroTime.AddSeconds(info.max);

            return new ChartDateTimeRange(niceStart, niceEnd,
                info.Interval, ChartDateTimeIntervalType.Seconds, m_calendar);
        }

        /// <summary>
        /// Makes the nice milli seconds range.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>Returns ChartDateTimeRange instance.</returns>
        /// <internalonly/>
        private ChartDateTimeRange MakeNiceMilliSecondsRange(DateTime start, DateTime end)
        {
            TimeSpan diff = end - start;
            MinMaxInfo info = m_niceRangeMaker.MakeNiceRange(0, Math.Ceiling(diff.TotalMilliseconds), m_niceRangeMaker.RangePaddingType);
            DateTime zeroTime = new DateTime(start.Year, start.Month, start.Day, start.Hour, start.Minute, start.Second, start.Millisecond, m_calendar);
            DateTime niceStart = zeroTime.AddSeconds(info.min);
            DateTime niceEnd = zeroTime.AddSeconds(info.max);

            return new ChartDateTimeRange(niceStart, niceEnd,
                info.Interval, ChartDateTimeIntervalType.MilliSeconds, m_calendar);
        }

        /// <summary>
        /// Calculates the type of the interval.
        /// </summary>
        /// <param name="diff">The diff.</param>
        /// <returns>Returns ChartDateTimeRange instance.</returns>
        /// <internalonly/>
        private ChartDateTimeIntervalType CalculateIntervalType(TimeSpan diff)
        {
            if (diff.Days > this.Defaults.GetDaysInYear())
            {
                return ChartDateTimeIntervalType.Years;
            }
            else if (diff.Days > this.Defaults.GetDaysInMonth())
            {
                return ChartDateTimeIntervalType.Months;
            }
            else if (diff.Days > this.Defaults.GetDaysInWeek() * 2)
            {
                return ChartDateTimeIntervalType.Weeks;
            }
            else if (diff.Days > 1)
            {
                return ChartDateTimeIntervalType.Days;
            }
            else if (diff.TotalHours > 1)
            {
                return ChartDateTimeIntervalType.Hours;
            }
            else if (diff.TotalMinutes > 1)
            {
                return ChartDateTimeIntervalType.Minutes;
            }
            else if (diff.TotalSeconds > 1)
            {
                return ChartDateTimeIntervalType.Seconds;
            }
            else
            {
                return ChartDateTimeIntervalType.MilliSeconds;
            }
        }

        /// <summary>
        /// Adjusts to week start.
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <param name="direction">The direction.</param>
        /// <returns>Returns ChartDateTimeRange instance.</returns>
        /// <internalonly/>
        private DateTime AdjustToWeekStart(DateTime dt, SeekDirection direction)
        {
            int correction = (int)dt.DayOfWeek;
            int day = dt.Day;

            if (dt.DayOfWeek != DayOfWeek.Sunday)
            {
                if (direction == SeekDirection.Forward)
                {
                    dt = m_calendar.AddDays(dt, 7 - correction);
                }
                else
                {
                    dt = m_calendar.AddDays(dt, -correction);
                }
            }

            return dt;
        }
        #endregion
    }
}