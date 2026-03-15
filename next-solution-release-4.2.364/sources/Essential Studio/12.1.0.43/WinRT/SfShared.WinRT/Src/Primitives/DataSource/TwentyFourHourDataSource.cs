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
    /// Represents the twwentyfour hour datas
    /// </summary>
    public class TwentyFourHourDataSource : DataSource
    {
        #region Override Methods

        /// <summary>
        /// Compares the twwentyfour hour data with the next.
        /// </summary>
        /// <param name="relativeDate"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        protected override DateTime? GetRelativeTo(DateTime relativeDate, int delta)
        {
            const int hoursInDay = 24;
            int nextHour = (hoursInDay + relativeDate.Hour + delta) % hoursInDay;
            return new DateTime(relativeDate.Year, relativeDate.Month, relativeDate.Day, nextHour, relativeDate.Minute, relativeDate.Second);
        }

        #endregion
    }
}
