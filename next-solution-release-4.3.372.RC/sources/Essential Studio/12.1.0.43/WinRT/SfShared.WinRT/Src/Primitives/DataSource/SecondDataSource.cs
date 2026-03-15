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
    /// Represents the seconds data.
    /// </summary>
    [ClassReference(IsReviewed = false, ShouldInclude = false)]
    public class SecondDataSource : DataSource
    {
        /// <summary>
        /// Initializes a new SecondDataSource
        /// </summary>
        public SecondDataSource(int interval)
        {
            Interval = interval;
        }
        private int _interval = 1;

        /// <summary>
        /// Represents a Interval
        /// </summary>
        public int Interval
        {
            get { return _interval; }
            set { _interval = value; }
        }
        /// <summary>
        /// Initializes a new SecondDataSource
        /// </summary>
        public SecondDataSource()
        {

        }
        #region Override Methods

        /// <summary>
        /// Compares the second data with the next.
        /// </summary>
        /// <param name="relativeDate"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        protected override DateTime? GetRelativeTo(DateTime relativeDate, int delta)
        {
            const int secondsInMinute = 60;
            if (Interval != 1)
                delta = Interval;
            int nextSecond = (secondsInMinute + relativeDate.Second + delta) % secondsInMinute;
            return new DateTime(relativeDate.Year, relativeDate.Month, relativeDate.Day, relativeDate.Hour, relativeDate.Minute, nextSecond);
        }

        #endregion
        
    }
}
