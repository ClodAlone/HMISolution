#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;

namespace Syncfusion.UI.Xaml.Primitives
{
    /// <summary>
    /// Represents the minute datas
    /// </summary>
    [ClassReference(IsReviewed = false, ShouldInclude = false)]
    public class MinuteDataSource : DataSource
    {
        /// <summary>
        /// Initializes a new MinuteDataSource
        /// </summary>
        public MinuteDataSource(int interval)
        {
            Interval = interval;
        }
        private int _interval = 1;

        /// <summary>
        /// Represents a start date of date range
        /// </summary>
        public int Interval
        {
            get { return _interval; }
            set { _interval = value; }
        }

        /// <summary>
        /// Initializes a new MinuteDataSource
        /// </summary>
        public MinuteDataSource()
        {

        }
        
        #region Override Methods

        /// <summary>
        /// Compares the minute data with the next.
        /// </summary>
        /// <param name="relativeDate"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        protected override DateTime? GetRelativeTo(DateTime relativeDate, int delta)
        {
            const int minutesInHour = 60;
            if (Interval != 1)
                delta = Interval;
            int nextMinute = (minutesInHour + relativeDate.Minute + delta) % minutesInHour;
            return new DateTime(relativeDate.Year, relativeDate.Month, relativeDate.Day, relativeDate.Hour, nextMinute, relativeDate.Second);
        }

        #endregion
    }
}
