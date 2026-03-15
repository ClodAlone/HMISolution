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
    /// Gets the twelve hour datas
    /// </summary>
    [ClassReference(IsReviewed = false, ShouldInclude = false)]
    public class TwelveHourDataSource : DataSource
    {
        #region Override Methods

        /// <summary>
        /// Compares the twelve hour data with the next.
        /// </summary>
        /// <param name="relativeDate"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        protected override DateTime? GetRelativeTo(DateTime relativeDate, int delta)
        {
            const int hoursInHalfDay = 12;
            int nextHour = (hoursInHalfDay + relativeDate.Hour + delta) % hoursInHalfDay;
            nextHour += hoursInHalfDay <= relativeDate.Hour ? hoursInHalfDay : 0;
            return new DateTime(relativeDate.Year, relativeDate.Month, relativeDate.Day, nextHour, relativeDate.Minute, relativeDate.Second);
        }

        #endregion
    }
}
