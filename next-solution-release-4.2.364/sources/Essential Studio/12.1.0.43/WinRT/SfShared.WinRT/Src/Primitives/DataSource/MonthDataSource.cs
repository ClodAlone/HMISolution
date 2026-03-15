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
    /// Represents the month datas
    /// </summary>
    [ClassReference(IsReviewed = false, ShouldInclude = false)]
    public class MonthDataSource : DataSource
    {
        #region Override Methods

        /// <summary>
        /// Compares the month data with the next.
        /// </summary>
        /// <param name="relativeDate"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        protected override DateTime? GetRelativeTo(DateTime relativeDate, int delta)
        {
            const int monthsInYear = 12;
            int nextMonth = ((monthsInYear + relativeDate.Month - 1 + delta) % monthsInYear) + 1;
            int nextDay = Math.Min(relativeDate.Day, DateTime.DaysInMonth(relativeDate.Year, nextMonth));
            return new DateTime(relativeDate.Year, nextMonth, nextDay, relativeDate.Hour, relativeDate.Minute, relativeDate.Second);
        }

        #endregion
    }
}
