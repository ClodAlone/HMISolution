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

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Specifies the start and end dates and interval time for the axis. Use this if the data points are of datetime type.
    /// </summary>
    public sealed class ChartDateTimeRange
    {
        #region Members
        private DateTime start;
        private DateTime end;
        private Calendar calendar;
        private ChartIntervalCollection intervalCollection;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartDateTimeRange"/> class.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="interval">The interval.</param>
        /// <param name="type">The type.</param>
        public ChartDateTimeRange(DateTime start, DateTime end, double interval,
            ChartDateTimeIntervalType type)
            : this(start, end, interval, type, CultureInfo.CurrentCulture.Calendar)
        {
        }

        /// <summary>
        ///Initializes a new instance of the <see cref="ChartDateTimeRange"/> class.
        /// </summary>
        /// <param name="start" type="System.DateTime">
        ///     <para>
        ///     The start of this range.
        ///     </para>
        /// </param>
        /// <param name="end" type="System.DateTime">
        ///     <para>
        ///     The end of this range.
        ///     </para>
        /// </param>
        /// <param name="interval" type="double">
        ///     <para>
        ///     The value of the default interval that is to be associated with this range.
        ///     </para>
        /// </param>
        /// <param name="type" type="Syncfusion.Windows.Forms.Chart.ChartDateTimeIntervalType">
        ///     <para>
        ///      The type of the default interval that is to be associated with this range.
        ///     </para>
        /// </param>
        /// <param name="calendar" type="System.Globalization.Calendar">
        ///     <para>
        ///     The calendar that is to be associated with this range.
        ///     </para>
        /// </param>
        public ChartDateTimeRange(DateTime start, DateTime end, double interval,
            ChartDateTimeIntervalType type, Calendar calendar)
        {
            this.start = start;
            this.end = end;
            this.calendar = calendar;
            this.intervalCollection = new ChartIntervalCollection(this);

            // Register the default interval.
            ChartDateTimeInterval defaultInterval = new ChartDateTimeInterval(type, interval, ChartDateTimeIntervalType.Auto, 0);
            this.Intervals.Register("default", defaultInterval);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the start boundary of this range.
        /// </summary>
        public DateTime Start
        {
            get
            {
                return this.start;
            }
        }

        /// <summary>
        /// gets the end boundary of this range.
        /// </summary>
        public DateTime End
        {
            get
            {
                return this.end;
            }
        }

        /// <summary>
        /// Gets the default interval associated with this range.
        /// <seealso cref="ChartDateTimeRange.Intervals"/>
        /// </summary>
        public ChartDateTimeInterval DefaultInterval
        {
            get
            {
                return this.Intervals["default"];
            }
        }

        /// <summary>
        /// Gets the Collection of registered intervals. (<see cref="ChartDateTimeInterval"/>) of several types can be registered with this range. 
        /// Intervals afford an easy way to partition and iterate through a date range.
        /// </summary>
        public ChartIntervalCollection Intervals
        {
            get
            {
                return this.intervalCollection;
            }
        }

        /// <summary>
        /// gets an instance of the <see cref="Calendar"/> associated with this date range.
        /// </summary>
        public Calendar Calendar
        {
            get
            {
                return this.calendar;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Overridden. Returns a string representation of this range.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Start: {0}\r\nEnd: {1}\r\nIntervals\r\n{2}\r\n", this.start.ToString(),
                this.end.ToString(),
                this.Intervals.ToString());
        }
        #endregion
    }
}