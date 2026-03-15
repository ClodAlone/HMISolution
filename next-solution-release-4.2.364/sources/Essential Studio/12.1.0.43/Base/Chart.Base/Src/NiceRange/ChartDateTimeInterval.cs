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
using System.Collections;
using System.Diagnostics;
using System.Globalization;

using Syncfusion.Documentation;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Types of DateTime intervals that are supported by Essential Chart.
    /// </summary>
    public enum ChartDateTimeIntervalType
    {
        /// <summary>
        /// The interval defaults to the most appropriate for the range of values being considered. For example, if the range
        /// is a few years, the interval would be internally set to years.
        /// </summary>
        Auto,

        /// <summary>
        /// Interval is set to years.
        /// </summary>
        Years,

        /// <summary>
        /// Interval is set to months.
        /// </summary>
        Months,

        /// <summary>
        /// Interval is set to weeks.
        /// </summary>
        Weeks,

        /// <summary>
        /// Interval is set to days.
        /// </summary>
        Days,

        /// <summary>
        /// Interval is set to hours.
        /// </summary>
        Hours,

        /// <summary>
        /// Interval is set to minutes.
        /// </summary>
        Minutes,

        /// <summary>
        /// Interval is set to seconds.
        /// </summary>
        Seconds,

        /// <summary>
        /// Interval is set to milliseconds.
        /// </summary>
        MilliSeconds,
    }

    /// <summary>
    ///   Represents a DateTime interval value.
    /// </summary>
    public class ChartDateTimeInterval
    {
        #region Delegates
        /// <summary>
        /// Delegate that is to be used during interaction on the range associated with an instance of ChartDateTimeInterval. If this
        /// delegate returns False, then that position is not used.
        /// <seealso cref="ChartDateTimeInterval"/>
        /// </summary>
        /// <param name="dt" type="System.DateTime">
        ///     <para>
        ///     The date that is to be included or not included.
        ///     </para>
        /// </param>
        public delegate bool IterationFilter(DateTime dt);

        /// <summary>
        /// Delegate that is to be used during iteration on the range associated with an instance of ChartDateTimeInterval. This delegate can
        /// change the date that gets passed in during iteration.
        /// </summary>
        /// <param name="dt" type="System.DateTime">
        ///     <para>
        ///     The date; that is a position along the associated range during iteration. This date can be changed by this callback.
        ///     </para>
        /// </param>
        public delegate DateTime IterationModifier(DateTime dt);
        #endregion

        #region Constants
        /// <summary>
        /// Name of default interval. 
        /// </summary>
        /// <seealso cref="ChartIntervalCollection"/>
        /// <seealso cref="ChartDateTimeRange.Intervals"/>
        /// <internalonly/>
        [DocumentationExclude()]
        public const string DefaultIntervalName = "default";
        #endregion

        #region Members
        private ChartDateTimeIntervalType m_type;
        private double m_value;
        private ChartDateTimeIntervalType m_offsetType;
        private double m_offset;
        private ChartDateTimeRange m_parent;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartDateTimeInterval"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="value">The value.</param>
        /// <param name="offsetType">Type of the offset.</param>
        /// <param name="offset">The offset.</param>
        internal ChartDateTimeInterval(ChartDateTimeIntervalType type, double value,
            ChartDateTimeIntervalType offsetType, double offset)
        {
            m_type = type;
            m_value = value;
            m_offsetType = offsetType;
            m_offset = offset;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartDateTimeInterval"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="value">The value.</param>
        internal ChartDateTimeInterval(ChartDateTimeIntervalType type, double value)
            : this(type, value, ChartDateTimeIntervalType.Auto, 0)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the type of this interval.
        /// </summary>
        public ChartDateTimeIntervalType Type
        {
            get
            {
                if (m_value == 0)
                {
                    return ChartDateTimeIntervalType.Auto;
                }

                return m_type;
            }

            set
            {
                m_type = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of this interval. Interval values should be interpreted in the context of <see cref="ChartDateTimeInterval.Type"/>.
        /// </summary>
        public double Value
        {
            get
            {
                return m_value;
            }

            set
            {
                m_value = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of the offset. Intervals can have offsets. Offsets merely affect the first position when an interval
        /// is applied to a range. They translate the first position by the value of the offset.
        /// </summary>
        public ChartDateTimeIntervalType OffsetType
        {
            get
            {
                return m_offsetType;
            }

            set
            {
                m_offsetType = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of the offset. Intervals can have offsets. Offsets merely affect the first position when an interval
        /// is applied to a range. They translate the first position by the value of the offset.
        /// </summary>
        public double Offset
        {
            get
            {
                return m_offset;
            }

            set
            {
                m_offset = value;
            }
        }

        /// <summary>
        /// The <see cref="ChartDateTimeRange"/> object with which this interval is associated. Intervals are not created stand alone but
        /// in the context of a range.
        /// </summary>
        public ChartDateTimeRange Parent
        {
            get
            {
                return m_parent;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Overloaded. Creates and returns a default iterator that will iterate over the associated range (<see cref="ChartDateTimeInterval.Parent"/>.
        /// </summary>
        /// <returns>Returns IEnumerable object.</returns>
        public IEnumerable Iterator()
        {
            return new Enumerator(this, m_parent.Start, m_parent.End, m_parent.Calendar);
        }

        /// <summary>
        /// Creates and returns an iterator that will iterate over the associated range (<see cref="ChartDateTimeInterval.Parent"/>. The
        /// IterationFilter callback will be called for each position in this range to check if the position should be included.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>Returns IEnumerable object.</returns>
        public IEnumerable Iterator(IterationFilter filter)
        {
            return new Enumerator(this, m_parent.Start, m_parent.End, m_parent.Calendar, filter, null);
        }

        /// <summary>
        /// Creates and returns an iterator that will iterate over the associated range (<see cref="ChartDateTimeInterval.Parent"/>. The
        /// IterationModifier callback will be called for each position in this range to allow the DateTime value of each position to
        /// be modified.
        /// </summary>
        /// <param name="modifier">The modifier.</param>
        /// <returns>Returns IEnumerable object.</returns>
        public IEnumerable Iterator(IterationModifier modifier)
        {
            return new Enumerator(this, m_parent.Start, m_parent.End, m_parent.Calendar, null, modifier);
        }

        /// <summary>
        /// Creates and returns an iterator that will iterate over the associated range (<see cref="ChartDateTimeInterval.Parent"/>. The
        /// IterationModifier callback will be called for each position in this range to allow the DateTime value of each position to
        /// be modified. The IterationFilter callback will be called for each position in this range to check if the position should be included.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="modifier">The modifier.</param>
        /// <returns>Returns IEnumerable object.</returns>
        public IEnumerable Iterator(IterationFilter filter, IterationModifier modifier)
        {
            return new Enumerator(this, m_parent.Start, m_parent.End, m_parent.Calendar, filter, modifier);
        }

        /// <summary>
        /// Creates and returns a default iterator that will iterate over the associated range (<see cref="ChartDateTimeInterval.Parent"/>.
        /// Only values that are between rangeStart and rangeEnd will be used.
        /// </summary>
        /// <param name="rangeStart">The range start.</param>
        /// <param name="rangeEnd">The range end.</param>
        /// <returns>Returns IEnumerable object.</returns>
        public IEnumerable Iterator(DateTime rangeStart, DateTime rangeEnd)
        {
            return new RangeEnumerator(this, m_parent.Start, m_parent.End, rangeStart, rangeEnd, m_parent.Calendar);
        }

        /// <summary>
        /// Creates and returns an iterator that will iterate over the associated range (<see cref="ChartDateTimeInterval.Parent"/>.
        /// Only values that are between rangeStart and rangeEnd will be used. In this range, the IterationFilter callback will be called for
        /// each position in this range to check if the position should be included.
        /// </summary>
        /// <param name="rangeStart">The range start.</param>
        /// <param name="rangeEnd">The range end.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>Returns IEnumerable object.</returns>
        public IEnumerable Iterator(DateTime rangeStart, DateTime rangeEnd, IterationFilter filter)
        {
            return new RangeEnumerator(this, m_parent.Start, m_parent.End, rangeStart, rangeEnd, m_parent.Calendar, filter, null);
        }

        /// <summary>
        /// Creates and returns an iterator that will iterate over the associated range (<see cref="ChartDateTimeInterval.Parent"/>.
        /// Only values that are between rangeStart and rangeEnd will be used. In this range, the IterationModifier callback will be called for
        /// each position in this range to allow it to be modified.
        /// </summary>
        /// <param name="rangeStart">The range start.</param>
        /// <param name="rangeEnd">The range end.</param>
        /// <param name="modifier">The modifier.</param>
        /// <returns>Returns IEnumerable object.</returns>
        public IEnumerable Iterator(DateTime rangeStart, DateTime rangeEnd, IterationModifier modifier)
        {
            return new RangeEnumerator(this, m_parent.Start, m_parent.End, rangeStart, rangeEnd, m_parent.Calendar, null, modifier);
        }

        /// <summary>
        /// Creates and returns an iterator that will iterate over the associated range (<see cref="ChartDateTimeInterval.Parent"/>.
        /// Only values that are between rangeStart and rangeEnd will be used. In this range, the IterationModifier callback will be called for
        /// each position in this range to allow it to be modified. For each modified value, the IterationFilter callback will be called
        /// to check if the position should be included.
        /// </summary>
        /// <param name="rangeStart">The range start.</param>
        /// <param name="rangeEnd">The range end.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="modifier">The modifier.</param>
        /// <returns>Returns IEnumerable object.</returns>
        public IEnumerable Iterator(DateTime rangeStart, DateTime rangeEnd, IterationFilter filter, IterationModifier modifier)
        {
            return new RangeEnumerator(this, m_parent.Start, m_parent.End, rangeStart, rangeEnd, m_parent.Calendar,
                filter, modifier);
        }

        /// <summary>
        /// Given a ChartDateTimeInterval.Enumerator instance, this method simply loops through and calculates the number of distinct positions
        /// in the range that the iterator represents.
        /// <seealso cref="ChartDateTimeRange"/>
        /// </summary>
        /// <param name="enumerable" type="System.Collections.IEnumerable">
        ///     <para>
        ///     An instance of the ChartDateTimeInterval.Enumerator.
        ///     </para>
        /// </param>
        /// <returns>
        ///     Number of distinct positions.
        /// </returns>
        public static int GetIntervalCount(IEnumerable enumerable)
        {
            Enumerator e = enumerable as Enumerator;

            if (e == null)
            {
                throw new ArgumentException("Invalid argument. Enumerator needs to be of type ChartDateTimeInterval.Enumerator.");
            }

            int count = 0;

            foreach (DateTime dt in enumerable)
            {
                count++;
            }
            if (count > 1)
            {
                return count - 1;
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// Overridden. Returns a string representation of this interval.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Type: {0}, Value: {1}, OffsetType: {2}, Offset: {3}", m_type, m_value, m_offsetType, m_offset);                
        }

        /// <summary>
        /// Adds the specified interval to the specified date.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <param name="calendar">The calendar.</param>
        /// <param name="type">The type of interval.</param>
        /// <param name="interval">The interval value.</param>
        /// <returns></returns>
        /// <internalonly/>
        protected virtual DateTime ApplyInterval(DateTime dateTime, Calendar calendar, ChartDateTimeIntervalType type, double interval)
        {
            DateTime result = dateTime;
            double days = 0;

            switch (type)
            {
                case ChartDateTimeIntervalType.Years:
                    {
                        int years = (int)Math.Floor(interval);
                        result = dateTime.AddYears(years);
                        days = (interval - years) * calendar.GetDaysInYear(result.Year);
                        result = result.AddDays(days);
                        break;
                    }

                case ChartDateTimeIntervalType.Months:
                    {
                        int months = (int)Math.Floor(interval);
                        result = dateTime.AddMonths(months);
                        days = (interval - months) * calendar.GetDaysInMonth(result.Year, result.Month);
                        result = result.AddDays(days);
                        break;
                    }

                case ChartDateTimeIntervalType.Weeks:
                    {
                        result = dateTime.AddDays(7 * interval);
                        break;
                    }

                case ChartDateTimeIntervalType.Days:
                    {
                        result = dateTime.AddDays(interval);
                        break;
                    }

                case ChartDateTimeIntervalType.Hours:
                    {
                        result = dateTime.AddHours(interval);
                        break;
                    }

                case ChartDateTimeIntervalType.Minutes:
                    {
                        result = dateTime.AddMinutes(interval);
                        break;
                    }

                case ChartDateTimeIntervalType.Seconds:
                    {
                        result = dateTime.AddSeconds(interval);
                        break;
                    }

                case ChartDateTimeIntervalType.MilliSeconds:
                    {
                        result = dateTime.AddMilliseconds(interval);
                        break;
                    }

                default:
                    {
                        if (interval != 0)
                        {
                            Trace.Fail("Invalid DateTimeIntervalType");
                            throw new ArgumentOutOfRangeException("Invalid DateTimeIntervalType");
                        }

                        break;
                    }
            }

            return result;
        }

        /// <summary>
        /// Sets the owner of interval.
        /// </summary>
        /// <param name="parent">The parent <see cref="ChartDateTimeRange"/>.</param>
        internal void SetParent(ChartDateTimeRange parent)
        {
            m_parent = parent;
        }

        /// <summary>
        /// Applies the interval to the specified date/time.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <param name="calendar">The calendar.</param>
        /// <param name="first">This value indicates if is the first value of axis.</param>
        /// <returns></returns>
        internal DateTime ApplyInterval(DateTime dateTime, Calendar calendar, bool first)
        {
            if (first)
            {
                return this.ApplyInterval(dateTime, calendar, this.OffsetType, this.Offset);
            }
            else
            {
                return this.ApplyInterval(dateTime, calendar, this.Type, this.Value);
            }
        }
        #endregion

        #region Internal types
        /// <summary>
        /// The Enumerator class which implements IEnumerable, IEnumerator.
        /// </summary>
        internal class Enumerator : IEnumerable, IEnumerator
        {
            #region Members
            protected IterationFilter iterationFilter;
            protected IterationModifier iterationModifier;
            protected ChartDateTimeInterval interval;
            protected DateTime start;
            protected DateTime end;
            protected int position = -1;
            protected DateTime lastDate;
            protected Calendar calendar;
            #endregion

            #region Constructor

            /// <summary>
            /// Initializes a new instance of the <see cref="Enumerator"/> class.
            /// </summary>
            /// <param name="interval">The interval.</param>
            /// <param name="start">The start.</param>
            /// <param name="end">The end.</param>
            /// <param name="calendar">The calendar.</param>
            public Enumerator(ChartDateTimeInterval interval, DateTime start, DateTime end, Calendar calendar)
                : this(interval, start, end, calendar, null, null)
            {
            }
            
            /// <summary>
            /// Initializes a new instance of the <see cref="Enumerator"/> class.
            /// </summary>
            /// <param name="interval">The interval.</param>
            /// <param name="start">The start.</param>
            /// <param name="end">The end.</param>
            /// <param name="calendar">The calendar.</param>
            /// <param name="iterationFilter">The iteration filter.</param>
            /// <param name="iterationModifier">The iteration modifier.</param>
            public Enumerator(ChartDateTimeInterval interval, DateTime start, DateTime end, Calendar calendar,
                IterationFilter iterationFilter, IterationModifier iterationModifier)
            {
                this.interval = interval;
                this.start = start;
                this.end = end;
                this.calendar = calendar;
                this.lastDate = start;
                this.iterationFilter = iterationFilter == null ? new IterationFilter(Enumerator.DefaultIterationFilter) : iterationFilter;
                this.iterationModifier = iterationModifier == null ? new IterationModifier(Enumerator.DefaultIterationModifier) : iterationModifier;
            }
            #endregion

            #region IMplementation
            /// <summary>
            /// Returns an enumerator that iterates through a collection.
            /// </summary>
            /// <returns>
            /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
            /// </returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this as IEnumerator;
            }

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>
            /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
            /// </returns>
            /// <exception cref="T:System.InvalidOperationException">
            /// The collection was modified after the enumerator was created.
            /// </exception>
            bool IEnumerator.MoveNext()
            {
                DateTime dt = this.GetNextDate(this.lastDate, this.position == -1);

                if (this.IsPastEnd(dt) == false)
                {
                    this.lastDate = dt;
                    this.position++;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            /// <exception cref="T:System.InvalidOperationException">
            /// The collection was modified after the enumerator was created.
            /// </exception>
            void IEnumerator.Reset()
            {
                this.position = -1;
                this.lastDate = start;
            }

            /// <summary>
            /// Gets the current element in the collection.
            /// </summary>
            /// <value></value>
            /// <returns>
            /// The current element in the collection.
            /// </returns>
            /// <exception cref="T:System.InvalidOperationException">
            /// The enumerator is positioned before the first element of the collection or after the last element.
            /// </exception>
            object IEnumerator.Current
            {
                get
                {
                    if (this.position == -1)
                    {
                        return null;
                    }
                    else
                    {
                        return this.lastDate;
                    }
                }
            }

            /// <summary>
            /// Adjusts the date.
            /// </summary>
            /// <param name="dt">The dt.</param>
            /// <returns>Returns datetime object.</returns>
            protected virtual DateTime AdjustDate(DateTime dt)
            {
                return dt;
            }

            /// <summary>
            /// Determines whether [is past end] [the specified dt].
            /// </summary>
            /// <param name="dt">The dt.</param>
            /// <returns>
            /// 	<c>true</c> if [is past end] [the specified dt]; otherwise, <c>false</c>.
            /// </returns>
            protected virtual bool IsPastEnd(DateTime dt)
            {
                return dt > this.end;
            }

            /// <summary>
            /// Gets the next date.
            /// </summary>
            /// <param name="currentDate">The current date.</param>
            /// <param name="first">if set to <c>true</c> [first].</param>
            /// <returns>Returns datetime object.</returns>
            private DateTime GetNextDate(DateTime currentDate, bool first)
            {
                DateTime dt = this.interval.ApplyInterval(currentDate, this.calendar, first);

                dt = this.AdjustDate(dt);
                dt = this.DoIterationModifier(dt);

                if (this.DoIterationFilter(dt) == false && this.IsPastEnd(dt) == false)
                {
                    return this.GetNextDate(dt, false);
                }
                else
                {
                    return dt;
                }
            }

            /// <summary>
            /// Does the iteration filter.
            /// </summary>
            /// <param name="dt">The dt.</param>
            /// <returns>Returns boolean.</returns>
            private bool DoIterationFilter(DateTime dt)
            {
                return this.iterationFilter(dt);
            }

            /// <summary>
            /// Does the iteration modifier.
            /// </summary>
            /// <param name="dt">The dt.</param>
            /// <returns>Returns datetime object.</returns>
            private DateTime DoIterationModifier(DateTime dt)
            {
                return this.iterationModifier(dt);
            }

            /// <summary>
            /// Defaults the iteration filter.
            /// </summary>
            /// <param name="dt">The dt.</param>
            /// <returns>Returns bool.</returns>
            private static bool DefaultIterationFilter(DateTime dt)
            {
                return true;
            }

            /// <summary>
            /// Defaults the iteration modifier.
            /// </summary>
            /// <param name="dt">The dt.</param>
            /// <returns>Returns DateTime object</returns>
            private static DateTime DefaultIterationModifier(DateTime dt)
            {
                return dt;
            }
            #endregion
        }

        /// <summary>
        /// The RangeEnumerator class.
        /// </summary>
        internal class RangeEnumerator : Enumerator
        {
            #region Members
            protected DateTime rangeStart;
            protected DateTime rangeEnd;
            #endregion

            #region Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="RangeEnumerator"/> class.
            /// </summary>
            /// <param name="interval">The interval.</param>
            /// <param name="start">The start.</param>
            /// <param name="end">The end.</param>
            /// <param name="rangeStart">The range start.</param>
            /// <param name="rangeEnd">The range end.</param>
            /// <param name="calendar">The calendar.</param>
            /// <param name="iterationFilter">The iteration filter.</param>
            /// <param name="iterationModifier">The iteration modifier.</param>
            public RangeEnumerator(ChartDateTimeInterval interval, DateTime start, DateTime end,
               DateTime rangeStart, DateTime rangeEnd, Calendar calendar,
                IterationFilter iterationFilter, IterationModifier iterationModifier)
                : base(interval, rangeStart, rangeEnd, calendar, iterationFilter, iterationModifier)
            {
                this.rangeStart = rangeStart;
                this.rangeEnd = rangeEnd;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="RangeEnumerator"/> class.
            /// </summary>
            /// <param name="interval">The interval.</param>
            /// <param name="start">The start.</param>
            /// <param name="end">The end.</param>
            /// <param name="rangeStart">The range start.</param>
            /// <param name="rangeEnd">The range end.</param>
            /// <param name="calendar">The calendar.</param>
            public RangeEnumerator(ChartDateTimeInterval interval, DateTime start, DateTime end, DateTime rangeStart, DateTime rangeEnd, Calendar calendar)
                : this(interval, start, end, rangeStart, rangeEnd, calendar, null, null)
            {
            }
            #endregion

            #region Implementation
            /// <summary>
            /// Adjusts the date.
            /// </summary>
            /// <param name="dt">The dt.</param>
            /// <returns></returns>
            protected override DateTime AdjustDate(DateTime dt)
            {
                if (position == -1)
                {
                    while (dt < this.rangeStart)
                    {
                        dt = this.interval.ApplyInterval(dt, this.calendar, false);
                    }
                }

                return dt;
            }

            /// <summary>
            /// Determines whether is the specified date most past by the end date.
            /// </summary>
            /// <param name="dt">The dt.</param>
            /// <returns>
            ///     <c>true</c> if [is past end] [the specified dt]; otherwise, <c>false</c>.
            /// </returns>
            protected override bool IsPastEnd(DateTime dt)
            {
                return dt > this.rangeEnd;
            }
            #endregion
        }
        #endregion
    }
}