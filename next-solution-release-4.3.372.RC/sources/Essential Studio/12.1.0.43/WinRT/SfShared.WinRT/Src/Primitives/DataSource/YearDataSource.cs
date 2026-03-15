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
    /// Represents the year datas
    /// </summary>
    [ClassReference(IsReviewed = false, ShouldInclude = false)]
    public class YearDataSource : DataSource
    {
        #region Override Methods

        /// <summary>
        /// Compares the year data with the next.
        /// </summary>
        /// <param name="relativeDate"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        protected override DateTime? GetRelativeTo(DateTime relativeDate, int delta)
        {
            int nextYear = relativeDate.Year + delta;
            if (nextYear != 0 && nextYear<9999)
            {
                int nextDay=Math.Min(relativeDate.Day, DateTime.DaysInMonth(nextYear, relativeDate.Month));
                return new DateTime(nextYear, relativeDate.Month, nextDay, relativeDate.Hour, relativeDate.Minute, relativeDate.Second);
            }
            return null;
        }

        #endregion
    }

}
