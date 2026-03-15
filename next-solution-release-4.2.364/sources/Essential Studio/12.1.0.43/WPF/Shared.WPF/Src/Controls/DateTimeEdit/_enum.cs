// <copyright file="_enum.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

namespace Syncfusion.Windows.Shared
{
    /// <property name="flag" value="Finished" />
    /// <summary>
    /// This enum classifies mode type.
    /// </summary>
    public enum RunMode
    {
        /// <summary>
        /// Indicates that visual part can be changed by type and scroll.
        /// </summary>
        TypeAndScroll = 0,

        /// <summary>
        /// Indicates that visual part can be changed by scroll only.
        /// </summary>
        ScrollOnly,

        /// <summary>
        /// Indicates that visual part cannot be changed manually.
        /// </summary>
        ReadOnly
    }

    /// <summary>
    /// This enum classifies date-time type.
    /// </summary>
    public enum DatePart
    {
        /// <summary>
        /// Specifies values representing that the object holding this
        /// value is year.
        /// </summary>
        Year = 0,

        /// <summary>
        /// Specifies values representing that the object holding this
        /// value is month.
        /// </summary>
        Month,

        /// <summary>
        /// Specifies values representing that the object holding this
        /// value is day.
        /// </summary>
        Day,

        /// <summary>
        /// Specifies values representing that the object holding this
        /// value is hour.
        /// </summary>
        Hour,

        /// <summary>
        /// Specifies values representing that the object holding this
        /// value is minute.
        /// </summary>
        Minute,

        /// <summary>
        /// Specifies values representing that the object holding this
        /// value is second.
        /// </summary>
        Second,

        /// <summary>
        /// Specifies values representing that the object holding this
        /// value is fraction.
        /// </summary>
        Fraction,

        /// <summary>
        /// Specifies values representing that the object holding this
        /// value is separator.
        /// </summary>
        Separator = 100
    }

    /// <summary>
    /// This enum classifies view type.
    /// </summary>
    public enum DateViewMode
    {
        /// <summary>
        /// Specifies values representing that possible view is normal.
        /// </summary>
        Normal = 0,

        /// <summary>
        /// Specifies values representing that possible view is correct.
        /// </summary>
        Correct,

        /// <summary>
        /// Specifies values representing that possible view is
        /// uncertain.
        /// </summary>
        Uncertain,

        /// <summary>
        /// Specifies values representing that possible view is
        /// incorrect.
        /// </summary>
        Incorrect
    }

    /// <summary>
    /// This enum classifies data state.
    /// </summary>
    public enum DataState
    {
        /// <summary>
        /// Specifies values representing possible state which indicates
        /// that date time is correct.
        /// </summary>
        Correct = 0,

        /// <summary>
        /// Specifies values representing possible state which indicates
        /// that date is incorrect.
        /// </summary>
        IncorrectDate,

        /// <summary>
        /// Specifies values representing possible state which indicates
        /// that time is incorrect.
        /// </summary>
        IncorrectTime
    }

    /// <summary>
    /// This enum classifies work result.
    /// </summary>
    public enum WorkResult
    {
        /// <summary>
        /// Indicates that DateTime was not changed while reading.
        /// </summary>
        Read = 0,

        /// <summary>
        /// Indicates that DateTime was correctly changed while typing.
        /// </summary>
        ChangedType,

        /// <summary>
        /// Indicates that DateTime was correctly changed while
        /// scrolling.
        /// </summary>
        ChangedScroll,

        /// <summary>
        /// Indicates that DateTime was incorrectly changed while typing.
        /// </summary>
        IncorrectChangedType,

        /// <summary>
        /// Indicates that DateTime was incorrectly changed while
        /// scrolling.
        /// </summary>
        IncorrectChangedScroll
    }

    /// <summary>
    /// This enum classifies popup state.
    /// </summary>
    public enum PopupState
    {
        /// <summary>
        /// Specifies values representing possible state for pop-up
        /// window that is hidden.
        /// </summary>
        None = 0,

        /// <summary>
        /// Specifies values representing possible state for pop-up
        /// window that shows tab.
        /// </summary>
        ShowTab,

        /// <summary>
        /// Specifies values representing possible state for pop-up
        /// window that shows calendar.
        /// </summary>
        ShowCalendar,

        /// <summary>
        /// Specifies values representing possible state for pop-up
        /// window that shows watch.
        /// </summary>
        ShowWatch
    }

    /// <summary>
    /// This enum classifies close calendar action.
    /// </summary>
    public enum CloseCalendarAction
    {
        /// <summary>
        /// Specifies values representing possible action on which the
        /// calendar will never close.
        /// </summary>
        Never = 0,

        /// <summary>
        /// Specifies values representing possible action on which the
        /// calendar will close on single click.
        /// </summary>
        SingleClick,

        /// <summary>
        /// Specifies values representing possible action on which the
        /// calendar will close on double click.
        /// </summary>
        DoubleClick
    }
}