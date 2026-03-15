// <copyright file="_enums.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Specifies visual mode of <see cref="CalendarEdit"/> control.
    /// </summary>
    public enum CalendarVisualMode
    {
        /// <summary>
        /// Days are displayed in <see cref="CalendarEdit"/> control.
        /// </summary>
        Days,

        /// <summary>
        /// Months are displayed in <see cref="CalendarEdit"/> control.
        /// </summary>
        Months,

        /// <summary>
        /// Years are displayed in <see cref="CalendarEdit"/> control.
        /// </summary>
        Years,

        /// <summary>
        /// Ten year ranges are displayed in <see cref="CalendarEdit"/> control.
        /// </summary>
        YearsRange,

        /// <summary>
        /// All the WeekNumbers in a year are displayed in <see cref="CalendarEdit"/> control.
        /// </summary>
        WeekNumbers
    }

    /// <summary>
    /// Defines direction of month change animation.
    /// </summary>
    public enum AnimationDirection
    {
        /// <summary>
        /// Horizontal direction of month change animation.
        /// </summary>
        Horizontal,

        /// <summary>
        /// Vertical direction of month change animation.
        /// </summary>
        Vertical
    }

    /// <summary>
    /// Defines selection range when click on <see cref="Syncfusion.Windows.Shared.DayNameCell"/>
    /// with pressed Ctrl button.
    /// </summary>
    public enum SelectionRangeMode
    {
        /// <summary>
        /// The whole column is selected.
        /// </summary>
        WholeColumn,

        /// <summary>
        /// Only days belonging to the current month from the column are selected.
        /// </summary>
        CurrentMonth
    }

    /// <summary>
    /// Defines calendar style.
    /// </summary>
    public enum CalendarStyle
    {
        /// <summary>
        /// Standard calendar style.
        /// </summary>
        Standard,

        /// <summary>
        /// Vista calendar style.
        /// </summary>
        Vista
    }
}