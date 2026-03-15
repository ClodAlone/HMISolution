//-------------------------------------------------------------------------------------------------
// <copyright file="ScheduleItemClickEvent.cs" company="syncfusion">
// Copyright (c) syncfusion.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using Syncfusion.Windows.Forms;
using Syncfusion.Schedule;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Schedule
{
	/// <summary>
	/// Declaration of event delegate for the ScheduleAppointmentClick event 
	/// </summary>
	public delegate void ScheduleAppointmentClickEventHandler(object sender, ScheduleAppointmentClickEventArgs e);

	/// <summary>
    /// A ScheduleAppointmentClickEventArgs class holds references 
    /// to the IScheduleAppointment that was clicked as as other information such as whether it was a single
    /// click or double click.Used by the <see cref="ScheduleControl.ScheduleAppointmentClick"/> event. 
	/// </summary>
	public class ScheduleAppointmentClickEventArgs : CancelEventArgs
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public ScheduleAppointmentClickEventArgs()
		{
		}

		/// <summary>
        /// Holds ScheduleAppointmentClick event information such as the item clicked, the type of click and the time of the click.
		/// </summary>
		/// <param name="clickType">The ScheduleAppointmentClickType that describes the click.</param>
		/// <param name="item">The IScheduleAppointment that was clicked. May be null.</param>
		/// <param name="type">The active ScheduleViewType.</param>
		/// <param name="clickDateTime">The calendar time slot of the click.</param>
		public ScheduleAppointmentClickEventArgs(ScheduleAppointmentClickType clickType, IScheduleAppointment item, ScheduleViewType type, DateTime clickDateTime)
		{
			this.item = item;
			this.clickType = clickType;
			this.type = type;
			this.clickDateTime = clickDateTime;
		}

        #region Properties

		ScheduleViewType type;
		
        /// <summary>
		/// A property that gets or sets the Calendar Type.
		/// </summary>
		public ScheduleViewType Type
		{
            get
            {
                return type;
            }

            set
            {
                type = value;
            }
		}

		ScheduleAppointmentClickType clickType;
		
        /// <summary>
		/// A property that gets or sets the type of item click
		/// </summary>
		public ScheduleAppointmentClickType ClickType
		{
            get
            {
                return clickType;
            }

            set
            {
                clickType = value;
            }
		}

		IScheduleAppointment item;
		
        /// <summary>
		/// A property that gets or sets the Schedule item under the click point
		/// </summary>
		public IScheduleAppointment Item
		{
            get
            {
                return item;
            }

            set
            {
                item = value;
            }
		}

		DateTime clickDateTime;

		/// <summary>
		/// A property that gets or sets the time slot that was clicked.
		/// </summary>
		public DateTime ClickDateTime
		{
            get
            {
                return clickDateTime;
            }

            set
            {
                clickDateTime = value;
            }
		}
        #endregion
	}

    /// <summary>
    /// Event delegate for the AdjustingAppointmentWithMouse event.
    /// </summary>
    public delegate void AdjustingAppointmentWithMouseEventHandler(object sender, AdjustingAppointmentMouseWithEventArgs e);

    /// <summary>
    /// A cancelable event that lets you control whether an appointment can be adjusted using the mouse.
    /// </summary>
    public class AdjustingAppointmentMouseWithEventArgs : CancelEventArgs
    {
        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="item">The appointment item to be adjusted.</param>
        public AdjustingAppointmentMouseWithEventArgs(IScheduleAppointment item)
        {
            this.item = item;
        }

        IScheduleAppointment item;
        
        /// <summary>
        /// A read-only property that gets the Schedule appointment item to be adjusted. 
        /// </summary>
        public IScheduleAppointment Item
        {
            get { return item; }
        }
    }

    /// <summary>
    /// Event delegate for the ScheduleGridCreated event.
    /// </summary>
    public delegate void ScheduleGridCreatedEventHandler(object sender, ScheduleGridCreatedEventArgs e);

    /// <summary>
    /// An event that lets you replace the default ScheduleGrid with a derived grid, or subscribe to events
    /// on the ScheduleGrid.
    /// </summary>
    /// <remarks>
    /// If you need to use a derived ScheduleGrid for any reason, set <see cref="Grid"/>
    /// to be an instance of your derived ScheduleGrid and set <see cref="Handled"/> = true. 
    /// If you want to subscribe to events on a ScheduleGrid, create an instance of ScheduleGrid,
    /// subscribe to the events, set <see cref="Grid"/> to the instance, and set <see cref="Handled"/> = true. 
    /// <example>
    /// Here is a sample event handler that subscribes to the CurrentCellKeyDown on the ScheduleGrid.
    /// <code lang="C#">
    ///        void scheduleControl1_ScheduleGridCreated(object sender, ScheduleGridCreatedEventArgs e)
    ///    {
    ///        //subscribe to an event
    ///        ScheduleGrid grid = new ScheduleGrid(e.Calendar, e.Schedule, e.InitialDate);
    ///        grid.CurrentCellKeyDown += new KeyEventHandler(grid_CurrentCellKeyDown);
    ///        e.Grid = grid;
    ///        e.Handled = true;
    ///    }
    /// </code>
    /// Here is a sample event handler that creates an instance of a derived ScheduleGrid.
    /// <code lang="C#">
    ///        void scheduleControl1_ScheduleGridCreated(object sender, ScheduleGridCreatedEventArgs e)
    ///    {
    ///        //create a derived ScheduleGrid
    ///        e.Grid = new MyScheduleGrid(e.Calendar, e.Schedule, e.InitialDate);
    ///        e.Handled = true;
    ///    }
    /// </code>
    /// </example>
    /// </remarks>
    public class ScheduleGridCreatedEventArgs : EventArgs
    {
        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="calendar">The navigation calendar used to create the ScheduleGrid.</param>
        /// <param name="schedule">The ScheduleControl that is the parent of the ScheduleGrid being created.</param>
        /// <param name="initialDate">The date used to create the ScheduleGrid.</param>
        public ScheduleGridCreatedEventArgs(NavigationCalendar calendar, ScheduleControl schedule, DateTime initialDate)
        {
            this.grid = null;
            this.initialDate = initialDate;
            this.schedule = schedule;
            this.calendar = calendar;
        }

        DateTime initialDate;

        /// <summary>
        /// Gets the date used to create the ScheduleGrid.
        /// </summary>
        public DateTime InitialDate
        {
            get { return initialDate; }
        }

        ScheduleGrid grid;
        
        /// <summary>
        /// A property that gets or sets the ScheduleGrid being used by the ScheduleControl. 
        /// </summary>
        /// <remarks>
        /// Make sure you set <see cref="Handled"/> = true if you supply a 
        /// ScheduleGrid object by setting this member.
        /// </remarks>
        public ScheduleGrid Grid
        {
            get { return grid; }
            set { grid = value; }
        }

        NavigationCalendar calendar;

        /// <summary>
        /// Gets the navigation calendar used by the ScheduleGrid being created.
        /// </summary>
        public NavigationCalendar Calendar
        {
            get
            {
                return calendar;
            }
       }

        ScheduleControl schedule;

        /// <summary>
        /// A read-only property that gets the ScheduleControl being used by the ScheduleGrid being created.
        /// </summary>
        public ScheduleControl Schedule
        {
            get { return schedule; }
        }

        private bool handled = false;

        /// <summary>
        /// A property that gets or sets whether a ScheduleGrid is being returned in <see cref="Grid"/>.
        /// </summary>
        /// <remarks>
        /// Use this variable to indicate that your event handler has created a ScheduleGrid object and
        /// assigned it to <see cref="Grid"/> so it can be 
        /// used in your ScheduleControl. This allows you to use derived ScheduleGrid objects,
        /// and also create a Schedulegrid object and subscribe to events on it.
        /// </remarks>
        public bool Handled
        {
            get { return handled; }
            set { handled = value; }
        }
    }

    #region enum ScheduleAppointmentClickType
	/// <summary>
	/// Enumerates possible types of clicks on a calendar.
	/// </summary>
	public enum ScheduleAppointmentClickType
	{
		/// <summary>
		/// A right click.
		/// </summary>
		RightClick,
		
        /// <summary>
		/// A left click.
		/// </summary>
		LeftClick,
		
        /// <summary>
		/// A right double click.
		/// </summary>
		RightDblClick,
		
        /// <summary>
		/// A left double click.
		/// </summary>
		LeftDblClick
	}
    #endregion
}
