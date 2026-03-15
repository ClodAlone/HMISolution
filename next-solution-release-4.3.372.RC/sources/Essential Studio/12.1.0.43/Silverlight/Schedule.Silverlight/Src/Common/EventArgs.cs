#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Syncfusion.Windows.Controls.Schedule
{
    /// <summary>
    /// EventArgs Class for handling events respective to Appointment rescheduling.
    /// </summary>
#if SyncfusionFramework4_0 && !SILVERLIGHT
    [System.ComponentModel.DesignTimeVisible(false)]
#endif

   
    public class ScheduleAppointmentEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Controls.Schedule.ScheduleAppointmentEventArgs"/>
        /// class.
        /// </summary>
        /// <param name="argappointment"></param>
        public ScheduleAppointmentEventArgs(ScheduleAppointment argappointment)
        {
            this.Appointment = (ScheduleAppointment)argappointment;
        }
        /// <summary>
        /// Gets and Sets the value for schedule appointment
        /// </summary>
        public ScheduleAppointment Appointment
        {
            get;
            private set;
        }
    }
    /// <summary>
    /// EventArgs Class for handling events respective to drag Appointment.
    /// </summary>
    public class ScheduleAppointmentDragEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Controls.Schedule.ScheduleAppointmentDragEventArgs"/>
        /// class.
        /// </summary>
        /// <param name="argAppointment"></param>
        public ScheduleAppointmentDragEventArgs(ScheduleAppointment argAppointment)
        {
            this.Appointment = (ScheduleAppointment)argAppointment;
        }

        /// <summary>
        /// Gets or sets the Schedule appointment value
        /// </summary>
        public ScheduleAppointment Appointment
        {
            get;
            private set;
        }
    }
    /// <summary>
    /// EventArgs Class for handling events respective to drop Appointment.
    /// </summary>
    public class ScheduleAppointmentDropEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Controls.Schedule.ScheduleAppointmentDropEventArgs"/>
        /// class.
        /// </summary>
        /// <param name="selectedAppointment"></param>
        public ScheduleAppointmentDropEventArgs(ScheduleAppointment selectedAppointment)
        {
            this.SelectedAppointment = (ScheduleAppointment)selectedAppointment;
        }

        /// <summary>
        /// Gets or sets the value for Schedule Appointment
        /// </summary>
        public ScheduleAppointment SelectedAppointment
        {
            get;
            private set;
        }
    }

    /// <summary>
    /// EventArgs Class for handling events respective to cancel Appointment.
    /// </summary>
    public class ScheduleAppointmentCancelEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Controls.Schedule.ScheduleAppointmentCancelEventArgs"/>
        /// class.
        /// </summary>
        /// <param name="argappointment"></param>
        public ScheduleAppointmentCancelEventArgs(ScheduleAppointment argappointment)
        {
            this.Appointment = (ScheduleAppointment)argappointment;
        }
        /// <summary>
        /// Gets or sets the value for Schedule Appointment
        /// </summary>
        public ScheduleAppointment Appointment
        {
            get;
            private set;
        }
    }

    /// <summary>
    /// EventArgs Class for handling events respective to reminder Appointment.
    /// </summary>
#if SyncfusionFramework4_0 && !SILVERLIGHT
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
 
    public class ScheduleReminderEventArgs : EventArgs
    {
        private ObservableCollection<ScheduleAppointment> reminderAppointments;
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Controls.Schedule.ScheduleReminderEventArgs"/> class.
        /// </summary>
        /// <param name="argappointments"></param>
        public ScheduleReminderEventArgs(ObservableCollection<ScheduleAppointment> argappointments)
        {
            this.reminderAppointments = (ObservableCollection<ScheduleAppointment>)argappointments;
        }

        /// <summary>
        /// Gets remainder appointments collection
        /// </summary>
        public ObservableCollection<ScheduleAppointment> ReminderAppointments
        {
            get
            {
                return reminderAppointments;
            }
        }
    }
    /// <summary>
    /// EventArgs Class for handling events respective to calendar added.
    /// </summary>
    public class ScheduleCalendarAddedEventArgs : EventArgs
    {
#if SILVERLIGHT
        /// <summary>
        /// Gets or sets the calendar.
        /// </summary>
        /// <value>The calendar.</value>
        public System.Windows.Controls.Calendar calendar { get; private set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleCalendarAddedEventArgs"/> class.
        /// </summary>
        /// <param name="calendaritem">The calendaritem.</param>
        public ScheduleCalendarAddedEventArgs(System.Windows.Controls.Calendar calendaritem)
        {
#else
        /// <summary>
        /// Gets or sets calendar
        /// </summary>
	        public Syncfusion.Windows.Controls.Calendar calendar { get; private set; }
            /// <summary>
            /// Initializes a new instance of the <see
            /// cref="T:Syncfusion.Windows.Controls.Schedule.ScheduleCalendarAddedEventArgs"/>
            /// class.
            /// </summary>
            /// <param name="calendaritem"></param>
	        public ScheduleCalendarAddedEventArgs(Syncfusion.Windows.Controls.Calendar calendaritem)
	        {
#endif
            this.calendar = calendaritem;
        }
    }


    /// <summary>
    /// Event Args class for appointmentvresizing events. 
    /// </summary>
    public class ScheduleAppointmentResizedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        /// <value>The start time.</value>
        public DateTime StartTime { get; private set; }
        /// <summary>
        /// Gets or sets the end time.
        /// </summary>
        /// <value>The end time.</value>
        public DateTime EndTime { get; private set; }
        /// <summary>
        /// Gets or sets the Selected Appointment
        /// </summary>
        public ScheduleAppointment SelectedAppointment
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleAppointmentResizedEventArgs"/> class.
        /// </summary>
        public ScheduleAppointmentResizedEventArgs()
        {
            this.StartTime = new DateTime();
            this.EndTime = new DateTime();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleAppointmentResizedEventArgs"/> class.
        /// </summary>
        /// /// <param name="selectedAppointment">The selected appointment.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time.</param>
        public ScheduleAppointmentResizedEventArgs(ScheduleAppointment selectedAppointment,DateTime startTime, DateTime endTime)
        {
            this.StartTime = startTime;
            this.EndTime = endTime;
            this.SelectedAppointment = selectedAppointment;
        }
    }

    /// <summary>
    /// Event Args class for appointmentvresizing cancel events. 
    /// </summary>
    public class ScheduleAppointmentResizingEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        /// <value>The start time.</value>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// Gets or sets the end time.
        /// </summary>
        /// <value>The end time.</value>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleAppointmentResizingEventArgs"/> class.
        /// </summary>
        public ScheduleAppointmentResizingEventArgs()
        {
            this.StartTime = new DateTime();
            this.EndTime = new DateTime();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleAppointmentResizingEventArgs"/> class.
        /// </summary>
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time.</param>
        public ScheduleAppointmentResizingEventArgs(DateTime startTime, DateTime endTime)
        {
            this.StartTime = startTime;
            this.EndTime = endTime;
        }
    }

   


    
}
