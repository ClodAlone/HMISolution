#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.ComponentModel;

namespace Syncfusion.Windows.Controls.Schedule
{
	/// <summary>
	/// Schedule view types to displays appointments. 
	/// </summary>
	public enum ScheduleType
	{
		/// <summary>
		/// Day view schedule type.
		/// </summary>
		Day = 0,
		
		/// <summary>
		/// Week view schedule type.
		/// </summary>
		Week,
		
		/// <summary>
		/// Work week view schedule type.
		/// </summary>
        WorkWeek,

        /// <summary>
        /// Month view schedule type.
        /// </summary>
        Month,

        /// <summary>
        /// Horizontal view schedule type.
        /// </summary>
        ScheduleView
	}

	/// <summary>
	/// <see cref="Schedule"/> control view modes.
	/// </summary>
	public enum ViewMode
	{
		/// <summary>
		/// Vertical mode.
		/// </summary>
		Vertical,

		/// <summary>
		/// Horizontal mode.
		/// </summary>
		Horizontal
	}

	/// <summary>
	/// Schedule control's calendar part placement.
	/// </summary>
	public enum CalendarPosition
	{
		/// <summary>
		/// Calendar is not displayed.
		/// </summary>
		None,

		/// <summary>
		/// Calendar is placed on the left side.
		/// </summary>
		Left,

		/// <summary>
		/// Calendar is placed on the right side.
		/// </summary>
		Right		
	}

	/// <summary>
	/// Specifies the type of the tooltip text.
	/// </summary>
	public enum TooltipType
	{
		/// <summary>
		/// Location text is displayed.
		/// </summary>
		Location,

		/// <summary>
		/// Resource name text is displayed.
		/// </summary>
		ResourceName,

		/// <summary>
		/// StartTime text is displayed.
		/// </summary>
		StartTime,

		/// <summary>
		/// EndTime text is displayed.
		/// </summary>		
		EndTime,
		
		/// <summary>
		/// StartAndEndTime text is displayed.
		/// </summary>
		StartAndEndTime,

		/// <summary>
		/// All text is displayed.
		/// </summary>
		All,
		
		/// <summary>
		/// Custom text is displayed.
		/// </summary>
		Custom
	}

	/// <summary>
	/// Indicators to specify the time interval.
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

	/// <summary>
	/// <see cref="Schedule"/> header's date/time format type.
	/// </summary>
	public enum HeaderFormat
	{
		/// <summary>
		/// Long date format (dddddd).
		/// </summary>
		Long,

		/// <summary>
		/// Short date format (ddddd).
		/// </summary>
		Short,

		/// <summary>
		/// Custom date format.
		/// </summary>
		Custom
	}

	/// <summary>
	/// Specifies the navigator button position.
	/// </summary>
	public enum DayNavigatorPosition
	{
		/// <summary>
		/// Navigator is placed to the left of <see cref="Schedule"/>'s page.
		/// </summary>
		HeaderLeft,
		
		/// <summary>
		/// Navigator is placed to the left and right of <see cref="Schedule"/>'s page.
		/// </summary>
		HeaderLeftRight
	}

	/// <summary>
	/// The row and column lines for the appointments.
	/// </summary>
	[Flags]
	public enum GridLinesType
	{
		/// <summary>
		/// Grid lines are not displayed.
		/// </summary>
		None = 0,

		/// <summary>
		/// Horizontal lines are displayed.
		/// </summary>
		Horizontal = 1,

		/// <summary>
		/// Vertical lines are displayed.
		/// </summary>
		Vertical = 2,

		/// <summary>
		/// Both horizontal and vertical lines are displayed.
		/// </summary>
		Both = Horizontal | Vertical
	}

	/// <summary>
	/// Pattern of <see cref="ScheduleAppointment"/>'s recurrence.
	/// </summary>
	public enum RecurrencePatternType
	{
		/// <summary>
		/// Daily recurrence.
		/// </summary>
		Daily = 0,

		/// <summary>
		/// Weekly recurrence.
		/// </summary>
		Weekly,

		/// <summary>
		/// Monthly recurrence.
		/// </summary>
		Monthly,

		/// <summary>
		/// Yearly recurrence.
		/// </summary>
		Yearly,

		/// <summary>
		/// Maximum values.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		Max
	}

	/// <summary>
	/// Specifies the days that are not to be displayed in <see cref="ScheduleType.Month"/> mode.
	/// </summary>
	public enum HiddenDays
	{
		/// <summary>
		/// All days are displayed.
		/// </summary>
		None,

		/// <summary>
		/// Sunday is hidden.
		/// </summary>
		Sunday,

		/// <summary>
		/// Saturday and sunday are hidden.
		/// </summary>
		WeekEnd
	}

    /// <summary>
    /// Specifies the AppointmentType that are to be used in <see cref="ScheduleAppointment"/> mode.
    /// </summary>
    public enum AppointmentType
    {
        /// <summary>
        /// General Appointment
        /// </summary>
        Normal,
        /// <summary>
        /// Recurrence Appointment
        /// </summary>
        Recurrence,
        /// <summary>
        /// Recurrence Appointment but splitted from Recurrence Appointment
        /// </summary>
        RecurrenceProxy,
        /// <summary>
        /// MultiDay Appointment
        /// </summary>
        MultiDay,
        /// <summary>
        /// MultiWeek Appointment
        /// </summary>
        MultiWeek,
        /// <summary>
        /// Holiday Appointment
        /// </summary>
        Holiday
    }

    /// <summary>
    /// Indicators to specify the types for the context menu
    /// </summary>
 public enum ContextMenuType
    {
        /// <summary>
        /// Only the Default Context Menu Items
        /// </summary>
        Default,
        /// <summary>
        /// Only the Custom Context Menu Items
        /// </summary>
        Custom,
        /// <summary>
        /// Default and Custom Context Menu Items
        /// </summary>
        CustomWithDefault
    }
 /// <summary>
 /// Indicators to specify the mode of recurrence pattern
 /// </summary>
    public enum RecurrencePatternMode
    {
        /// <summary>
        /// Daily mode
        /// </summary>
        Daily,
        /// <summary>
        /// Weekly mode
        /// </summary>
        Weekly,
        /// <summary>
        /// Monthly mode
        /// </summary>
        Monthly,
        /// <summary>
        /// Yearly mode
        /// </summary>
        Yearly
    }

    /// <summary>
    ///  Indicators to specify the priority level for the Appointment
    /// </summary>
    public enum AppointmentImportance
    {
        /// <summary>
        /// priority none 
        /// </summary>
        None,
        /// <summary>
        /// priority low 
        /// </summary>
        Low,
        /// <summary>
        /// priority high 
        /// </summary>
        High
    }
    /// <summary>
    /// Indicators to specify the style 
    /// </summary>
    public enum VisualStyle
    {
        /// <summary>
        /// Blend Style  
        /// </summary>
        Blend,
        /// <summary>
        /// Office14Silver Style  
        /// </summary>
        Office14Silver,
        /// <summary>
        /// Office14Blue Style  
        /// </summary>
        Office14Blue,
        /// <summary>
        /// Office14Black Style 
        /// </summary>
        Office14Black,
        /// <summary>
        /// VS2010 Style 
        /// </summary>
        VS2010,
        /// <summary>
        /// Metro Style 
        /// </summary>
        Metro
    }
    /// <summary>
    /// Indicators to specify the color palette value  
    /// </summary>
    public enum ColorPalette
    {
        /// <summary>
        ///  
        /// </summary>
        ColorButton1,
        /// <summary>
        ///  
        /// </summary>
        ColorButton2,
        /// <summary>
        ///  
        /// </summary>
        ColorButton3,
        /// <summary>
        ///  
        /// </summary>
        ColorButton4,
        /// <summary>
        ///  
        /// </summary>
        ColorButton5,
        /// <summary>
        ///  
        /// </summary>
        ColorButton6,
        /// <summary>
        ///  
        /// </summary>
        ColorButton7,
        /// <summary>
        ///  
        /// </summary>
        ColorButton8,
        /// <summary>
        ///  
        /// </summary>
        ColorButton9,
        /// <summary>
        ///  
        /// </summary>
        ColorButton10,
        /// <summary>
        ///  
        /// </summary>
        ColorButton11,
        /// <summary>
        ///  
        /// </summary>
        ColorButton12,
        /// <summary>
        ///  
        /// </summary>
        ColorButton13,
        /// <summary>
        ///  
        /// </summary>
        ColorButton14,
        /// <summary>
        ///  
        /// </summary>
        ColorButton15,
        /// <summary>
        ///  
        /// </summary>
        Custom
    }

    /// <summary>
    /// Values representing Resizing of Appointments
    /// </summary>
    public enum ResizePosition
    {
        /// <summary>
        /// 
        /// </summary>
        None,
        /// <summary>
        /// 
        /// </summary>
        Top,
        /// <summary>
        /// 
        /// </summary>
        Bottom,
        /// <summary>
        /// 
        /// </summary>
        Left,
        /// <summary>
        /// 
        /// </summary>
        Right
    }

    /// <summary>
    /// Valus Representing Mouse Pointer Events.
    /// </summary>
    public enum MousePointerType
    {
        /// <summary>
        /// 
        /// </summary>
        None,
        /// <summary>
        /// 
        /// </summary>
        Resize
    }
}
