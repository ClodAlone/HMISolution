#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Syncfusion.UI.Xaml.Primitives
{
    /// <summary>
    /// An ElementWrapper class that can serve as the basis for Atom Date Construct
    /// based extensions.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class DateTimeWrapper
    {
        #region Variables

        /// <summary>
        /// Gets the DateTime being wrapped.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public DateTime DateTime { get; internal set; }

        /// <summary>
        /// Gets the 4-digit year as a string.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public string YearNumber { get { return DateTime.ToString("yyyy", CultureInfo.CurrentCulture); } }

        /// <summary>
        /// Gets the 2-digit month as a string.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public string MonthNumber { get { return DateTime.ToString("MM", CultureInfo.CurrentCulture); } }

        /// <summary>
        /// Gets the month name as a string.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public string MonthName { get { return DateTime.ToString("MMMM", CultureInfo.CurrentCulture); } }

        /// <summary>
        /// Gets the 2-digit day as a string.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public string DayNumber { get { return DateTime.ToString("dd", CultureInfo.CurrentCulture); } }

        /// <summary>
        /// Gets the day name as a string.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public string DayName { get { return DateTime.ToString("dddd", CultureInfo.CurrentCulture); } }

        /// <summary>
        /// Gets the hour as a string.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public string HourNumber { get { return DateTime.ToString(CurrentCultureUsesTwentyFourHourClock() ? "%H" : "%h", CultureInfo.CurrentCulture); } }

        /// <summary>
        /// Gets and sets the IsTwentyFourHourtimeline as a bool.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public static bool IsTwentyFourHourtimeline { get; set; }

        /// <summary>
        /// Gets the 2-digit minute as a string.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public string MinuteNumber { get { return DateTime.ToString("mm", CultureInfo.CurrentCulture); } }

        /// <summary>
        /// Gets the 2-digit second as a string.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public string SecondNumber { get { return DateTime.ToString("ss", CultureInfo.CurrentCulture); } }

        /// <summary>
        /// Gets the AM/PM designator as a string.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pm", Justification = "Clearest way of expressing the concept.")]
        [ClassReference(IsReviewed = false)]
        public string AmPmString { get { return DateTime.ToString("tt", CultureInfo.CurrentCulture); } }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the DateTimeWrapper class.
        /// </summary>
        /// <param name="dateTime">DateTime to wrap.</param>
        [ClassReference(IsReviewed = false)]
        public DateTimeWrapper(DateTime dateTime)
        {
            DateTime = dateTime;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a value indicating whether the current culture uses a 24-hour clock.
        /// </summary>
        /// <returns>True if it uses a 24-hour clock; false otherwise.</returns>
        [ClassReference(IsReviewed = false)]
        public static bool CurrentCultureUsesTwentyFourHourClock()
        {
            return IsTwentyFourHourtimeline;
        }

        #endregion
    }
}
