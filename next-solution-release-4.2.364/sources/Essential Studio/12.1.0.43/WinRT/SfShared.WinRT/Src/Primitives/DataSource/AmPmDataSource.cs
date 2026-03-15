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
    /// Source for AMPM Data
    /// </summary>
    [ClassReference(IsReviewed = false, ShouldInclude = false)]
    public class AmPmDataSource : DataSource
    {
        #region Override Methods

        /// <summary>
        /// Gets the relative DateTime
        /// </summary>
        /// <param name="relativeDate"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        protected override DateTime? GetRelativeTo(DateTime relativeDate, int delta)
        {
            const int hoursInDay = 24;
            int nextHour = relativeDate.Hour + (delta * (hoursInDay / 2));
            if ((nextHour < 0) || (hoursInDay <= nextHour))
            {
                return null;
            }
            return new DateTime(relativeDate.Year, relativeDate.Month, relativeDate.Day, nextHour, relativeDate.Minute, relativeDate.Second);
        }

        #endregion
    }
}
