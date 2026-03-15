#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.UI.Xaml.Schedule
{
    #region Enum Properties

    #region EditorAction

    /// <summary>
    /// Specifies the action while opening appointment editor.
    /// </summary>
    public enum EditorAction
    {
        Add,
        Edit,
        Delete,
    }

    #endregion

    #region EditorClosedAction

    /// <summary>
    /// Specifies the action while closing appointment editor.
    /// </summary>
    public enum EditorClosedAction
    {
        Save,
        Delete,
        Cancel
    }

    #endregion

    #region ScheduleType

    /// <summary>
    /// Specifies the type of schedule views.
    /// </summary>
    public enum ScheduleType
    {
        Day,
        Week,
        WorkWeek,
        Month,
        TimeLine
    }

    #endregion

    #region TimeInterval

    /// <summary>
    /// Specifies the time interval value.
    /// </summary>
    public enum TimeInterval
    {
        /// <summary>
        /// 5 minutes interval.
        /// </summary>
        FiveMin = 0,

        /// <summary>
        /// 6 minutes interval.
        /// </summary>
        SixMin,

        /// <summary>
        /// 10 minutes interval.
        /// </summary>
        TenMin,

        /// <summary>
        /// 15 minutes interval.
        /// </summary>
        FifteenMin,

        /// <summary>
        /// 20 minutes interval.
        /// </summary>
        TwentyMin,

        /// <summary>
        /// 30 minutes interval.
        /// </summary>
        ThirtyMin,

        /// <summary>
        /// 1 hour interval.
        /// </summary>
        OneHour
    }

    #endregion

    #region RecurrenceType

    /// <summary>
    /// Specifies the type of recurrence in appointment.
    /// </summary>
    public enum RecurrenceType
    {
        /// <summary>
        /// Daily.
        /// </summary>
        Daily,

        /// <summary>
        /// Weekly.
        /// </summary>
        Weekly,

        /// <summary>
        /// Monthly.
        /// </summary>
        Monthly,

        /// <summary>
        /// Yearly.
        /// </summary>
        Yearly,

    }

    #endregion

    #region ReminderTimeType

    /// <summary>
    /// Specifies the type of reminder time for schedule appointment.
    /// </summary>
    public enum ReminderTimeType
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,
        ZeroMin,
        FiveMin,
        TenMin,
        FifteenMin,
        ThirtyMin,
        OneHour,
        TwoHours,
        ThreeHours,
        FourHours,
        FiveHours,
        SixHours,
        SevenHours,
        EightHours,
        NineHours,
        TenHours,
        ElevenHours,
        EighteenHours,
        HalfDay,
        OneDay,
        TwoDays,
        ThreeDays,
        FourDays,
        OneWeek,
        TwoWeeks
    }

    #endregion

    #region DayHeaderOrder

    /// <summary>
    /// Specifies the order of resources in schedule.
    /// </summary>
    public enum DayHeaderOrder
    {
        OrderByDate,
        OrderByResource
    }

    #endregion

    #region TimeMode

    /// <summary>
    /// Specifies the time mode value.
    /// </summary>
    public enum TimeModes
    {
        TwelveHours,
        TwentyFourHours
    }

    #endregion

    #region MenuType

    /// <summary>
    /// Specifies the type of context menu.
    /// </summary>
    public enum MenuType
    {
        Default,
        RadialMenu
    }

    #endregion

    #endregion
}
