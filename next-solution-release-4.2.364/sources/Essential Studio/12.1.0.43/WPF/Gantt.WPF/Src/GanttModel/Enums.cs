#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Gantt
{
    /// <summary>
    /// Represents the dependency relationship between the tasks/activities.
    /// </summary>
    public enum GanttTaskRelationship
    {
        /// <summary>
        /// Represent Finish - to - Finish relation.
        /// </summary>
        FinishToFinish,
        /// <summary>
        /// Represent Finish - to - Start relation.
        /// </summary>
        FinishToStart,
        /// <summary>
        /// Represent Start - to - Finish relation.
        /// </summary>
        StartToFinish,
        /// <summary>
        /// Represent Start - to - Start relation.
        /// </summary>
        StartToStart
    }

    /// <summary>
    /// Represents the Built in Visual Styles
    /// </summary>
    public enum VisualStyle
    {
        /// <summary>
        /// Office 2010 Blue Style.
        /// </summary>
        Office2010Blue = 0,
        /// <summary>
        /// Office 2010 Black Style.
        /// </summary>
        Office2010Black = 1,
        /// <summary>
        /// Office 2010 Silver Style.
        /// </summary>
        Office2010Silver = 2,
        /// <summary>
        /// Metro Style.
        /// </summary>
        Metro,
        /// <summary>
        /// Blend Style.
        /// </summary>
        Blend,
        /// <summary>
        /// VS2010 Style.
        /// </summary>
        VS2010,
    }

    /// <summary>
    /// Represents the Calendar Months.
    /// </summary>
    public enum Month
    {
        January = 1,
        February = 2,
        March = 3,
        April = 4,
        May = 5,
        June = 6,
        July = 7,
        August = 8, 
        September = 9, 
        October = 10, 
        November = 11, 
        December = 12
    }

    /// <summary>
    /// Represents the schedule type.
    /// </summary>
    public enum ScheduleType
    {
        /// <summary>
        /// Schedule with Year and month header.
        /// </summary>
        YearWithMonths,
        /// <summary>
        /// Schedule with Year, month, week and day header.
        /// </summary>
        YearWithDays,
        /// <summary>
        /// Scheule with Month, week, and day header
        /// </summary>
        MonthWithDays,
        /// <summary>
        /// Schedule with Week and day header.
        /// </summary>
        WeekWithDays,
        /// <summary>
        /// Schedule that requires external source to draw date time headers.
        /// </summary>
        CustomDateTime,
        /// <summary>
        /// Schedule that requires external source to draw numeric headers.
        /// </summary>
        CustomNumeric,

        // Because of some rendering issue in Silverlight we are blocking the Hours/Minutes Schedule.
#if !SILVERLIGHT
        /// <summary>
        /// Schedule with Month, week, day and hour header.
        /// </summary>
        MonthWithHours,        
        /// <summary>
        /// Schedule with day, and hour header
        /// </summary>
        DayWithHours,
        /// <summary>
        /// Schedule with Day, hour and minutes header.
        /// </summary>
        DayWithMinutes,
#endif        
    }

    /// <summary>
    /// Represents the Time units.
    /// </summary>
    public enum TimeUnit
    {
        // Because of some rendering issue in Silverlight we are blocking the Hours/Minutes Schedule.
#if !SILVERLIGHT
        /// <summary>
        /// Represents the Minutes unit
        /// </summary>
        Minutes,
        /// <summary>
        /// Represents the Hour unit
        /// </summary>
        Hours,
#endif
        /// <summary>
        /// Represents the Day unit
        /// </summary>
        Days,
        /// <summary>
        /// Represents the Week unit
        /// </summary>
        Weeks,
        /// <summary>
        /// Represents the Month unit
        /// </summary>
        Months,
        /// <summary>
        /// Represents the Year unit
        /// </summary>
        Years,
    }

    /// <summary>
    /// Represents the Layout mode of Gantt
    /// </summary>
    public enum GanttLayoutMode
    {
        /// <summary>
        /// Represents the default lay out
        /// </summary>
        Default,
        /// <summary>
        /// Fit the layout to Grid's width
        /// </summary>
        FitToGrid,
        /// <summary>
        /// Fit the layout to Chart's width
        /// </summary>
        FitToChart,
    }

    /// <summary>
    /// Represent the Gantt IndicatorType
    /// </summary>
    public enum CurentDateLinePositions
    {
        /// <summary>
        /// CurrentDateLine will be positioned at Current Date Begining
        /// </summary>
        Today,
        /// <summary>
        /// CurrentDtaeLine will be positioned at system time when the GanttControl loaded.
        /// </summary>
        LoadedTime,
        /// <summary>
        /// CurrentDtaeLine will be positioned at system time when the GanttControl loaded.The position will be changed as per the system time changes.
        /// </summary>
        DynamicTime,
        /// <summary>
        /// CurrentDateLine will be postion at any where using custom.
        /// </summary>
        Absolute,
        /// <summary>
        /// CurrentDateLine is not displayed.
        /// </summary>
        None
    }

    /// <summary>
    /// Represents the stripline repeat options
    /// </summary>
    public enum Repeat
    {
        /// <summary>
        /// In Default the repeat mode is set as none
        /// </summary>
        None,

        /// <summary>
        /// Stripline repeated in year basis
        /// </summary>
        Year,

        /// <summary>
        /// Stripline repeated in month basis
        /// </summary>
        Month,

        /// <summary>
        /// Stripline repeated in week basis
        /// </summary>
        Week,

        /// <summary>
        /// Stripline repeated in day basis
        /// </summary>
        Day,

#if !SILVERLIGHT

        /// <summary>
        /// Strripline repeated in hour basis
        /// </summary>
        Hour,

        /// <summary>
        /// Stripline repeated in minute basis
        /// </summary>
        Minute
#endif
    }

    /// <summary>
    /// Represents the Stripline type
    /// </summary>
    public enum StriplineType
    {
        /// <summary>
        /// Represents the regular stripline
        /// </summary>
        Regular,

        /// <summary>
        /// Represents the Absolute stripline(User can place the sripline in custom position with custom size
        /// </summary>
        Absolute
    }

	/// <summary>
    /// Represents the Resizing Direction
    /// </summary>
    public enum Directions
    {
        /// <summary>
        /// Represents that node resizing in Left Direction
        /// </summary>
        Left,
        /// <summary>
        /// Represents that node resizing in Right Direction
        /// </summary>
        Right
    }

    /// <summary>
    /// Enum for Placement mode of Resource Name
    /// </summary>
    public enum PlacementMode
    {
        /// <summary>
        /// Place the Resource Name in right side of Gantt Node
        /// </summary>
        Right,
        /// <summary>
        /// Place the Resource Name in left side of Gantt Node
        /// </summary>
        Left,
        /// <summary>
        /// Place the Resource Name in bottom of Gantt Node
        /// </summary>
        Bottom,
        /// <summary>
        /// Place the Resource Name in top of Gantt Node
        /// </summary>
        Top
    }
}
