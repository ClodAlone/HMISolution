#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Data;
using Syncfusion.Windows.Data;
using System.Globalization;
using System.Windows.Controls;

namespace Syncfusion.Windows.Controls.Schedule
{
    /// <summary>
    /// Represents <see cref="Schedule"/>'s appointment.
    /// </summary>
#if SyncfusionFramework4_0 && !SILVERLIGHT
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ScheduleAppointment :
        DependencyObject
    {
        #region Fields

        //private ScheduleResource owner;
        private ScheduleAppointment recurrenceParent;
        // internal double timeInterval;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleAppointment"/> class.
        /// </summary>
        public ScheduleAppointment()
        {
            this.updating = false;
            this.AppointmentProxy = new ScheduleAppointmentCollection();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleAppointment"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        internal ScheduleAppointment(ScheduleAppointment parent)
        {
            this.recurrenceParent = parent;
            this.AllDay = parent.AllDay;
        }

        #endregion

        #region Dependency properties

        #region CurrentAppointmentType

        private AppointmentType currentAppointmentType = AppointmentType.Normal;

        internal AppointmentType CurrentAppointmentType 
        {
            get { return currentAppointmentType; }
            set { currentAppointmentType = value; }
        }
        /// <summary>
        /// Gets or sets a value whether the instance is an MultiDayAppointment
        /// </summary>
        internal bool MultiDayAppointment { get; set; } 
        /// <summary>
        /// Gets or sets the actual StartTime of Multiday appointment instance
        /// </summary>
        internal DateTime MultiDayAppointmentStartTime { get; set; } 
        /// <summary>
        /// Gets or sets  actual EndTime of an Multiday appointment instance
        /// </summary>
        internal DateTime MultiDayAppointmentEndTime { get; set; }
        #endregion

        #region IsAppointmentProxyCollection
        /// <summary>
        /// Gets or sets a value indicating whether this instance is appointment proxy collection.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is appointment proxy collection; otherwise, <c>false</c>.
        /// </value>
        public bool IsAppointmentProxyCollection
        {
            get
            {
                return (bool)GetValue(IsAppointmentProxyCollectionProperty);
            }

            set
            {
                SetValue(IsAppointmentProxyCollectionProperty, value);
            }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsAppointmentProxyCollection.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsAppointmentProxyCollectionProperty = DependencyProperty.Register("IsAppointmentProxyCollection",
            typeof(bool), typeof(ScheduleAppointment), new PropertyMetadata(false, OnIsAppointmentProxyChanged));

        /// <summary>
        /// method called when IsRecurrenceProxy Changed
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args">An <see
        /// cref="T:System.Windows.DependencyPropertyChangedEventArgs"/> that contains the
        /// event data.</param>
        public static void OnIsAppointmentProxyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ScheduleAppointment instance = (ScheduleAppointment)obj;
            if (instance.IsAppointmentProxyCollection == true)
            {
                instance.AllowDragandDrop = false;
                instance.AllowResize = false;
            }
            else
            {
                instance.AllowDragandDrop = true;
                instance.AllowResize = true;
            }
        }

        #endregion

        #region AppointmentProxy
        /// <summary>
        /// Gets or sets the appointment proxy.
        /// </summary>
        /// <value>The appointment proxy.</value>
        public ScheduleAppointmentCollection AppointmentProxy
        {
            get
            {
                return (ScheduleAppointmentCollection)GetValue(AppointmentProxyProperty);
            }

            set
            {
                SetValue(AppointmentProxyProperty, value);
            }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for AppointmentProxy.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AppointmentProxyProperty = DependencyProperty.Register("AppointmentProxy",
            typeof(ScheduleAppointmentCollection), typeof(ScheduleAppointment), new PropertyMetadata(null));

        #endregion
        
        #region AllowRecurrence
        /// <summary>
        /// Gets or sets a value indicating whether [allow recurrence].
        /// </summary>
        /// <value><c>true</c> if [allow recurrence]; otherwise, <c>false</c>.</value>
        public bool AllowRecurrence
        {
            get
            {
                return (bool)GetValue(AllowRecurrenceProperty);
            }

            set
            {
                SetValue(AllowRecurrenceProperty, value);
            }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for AllowRecurrence.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AllowRecurrenceProperty = DependencyProperty.Register("AllowRecurrence",
            typeof(bool), typeof(ScheduleAppointment), new PropertyMetadata(true));

        #endregion

        #region AllowDragandDrop
        /// <summary>
        /// Gets or sets a value indicating whether [allow dragand drop].
        /// </summary>
        /// <value><c>true</c> if [allow dragand drop]; otherwise, <c>false</c>.</value>
        public bool AllowDragandDrop
        {
            get
            {
                return (bool)GetValue(AllowDragandDropProperty);
            }

            set
            {
                SetValue(AllowDragandDropProperty, value);
            }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for AllowDragandDrop.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AllowDragandDropProperty = DependencyProperty.Register("AllowDragandDrop",
            typeof(bool), typeof(ScheduleAppointment), new PropertyMetadata(true));

        #endregion

        #region AllowResize
        /// <summary>
        /// Gets or sets a value indicating whether [allow resize].
        /// </summary>
        /// <value><c>true</c> if [allow resize]; otherwise, <c>false</c>.</value>
        public bool AllowResize
        {
            get
            {
                return (bool)GetValue(AllowResizeProperty);
            }

            set
            {
                SetValue(AllowResizeProperty, value);
            }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for AllowResize.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AllowResizeProperty = DependencyProperty.Register("AllowResize", 
            typeof(bool), typeof(ScheduleAppointment), new PropertyMetadata(true));

        #endregion

        #region AllDay

        /// <summary>
        /// Gets or sets a value indicating whether appointment extendsfor an entire day ("all day appointment").
        /// </summary>
        public bool AllDay
        {
            get
            {
                return (bool)GetValue(AllDayProperty);
            }

            set
            {
                SetValue(AllDayProperty, value);
            }
        }

        /// <summary>
        /// Event that is raised when <see cref="ScheduleAppointment.AllDay"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback AllDayChanged;

        /// <summary>
        /// The identifier for the <see cref="ScheduleAppointment.AllDay"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty AllDayProperty = DependencyProperty.Register(
            "AllDay", typeof(bool), typeof(ScheduleAppointment), new PropertyMetadata(false, new PropertyChangedCallback(OnAllDayChanged)));

        /// <summary>
        /// Calls OnAllDayChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnAllDayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScheduleAppointment instance = (ScheduleAppointment)d;
            instance.OnAllDayChanged(e);
        }

        /// <summary>
        /// Raises AllDayChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private void OnAllDayChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.AllDayChanged != null)
            {
                this.AllDayChanged(this, e);
            }
        }

        #endregion

        #region StartTime

        /// <summary>
        /// Gets or sets the value indicating the start of the <see cref="ScheduleAppointment"/> duration.
        /// </summary>
        [TypeConverter(typeof(DateTimeTypeConverter))]
        public DateTime StartTime
        {
            get
            {
                return (DateTime)GetValue(StartTimeProperty);
            }

            set
            {
                SetValue(StartTimeProperty, value);
                this.IsDismissed = false;
                this.ReminderTime = this.StartTime.Subtract(this.ThresholdTime);
                this.DueTime = this.StartTime - DateTime.Now;

            }
        }

        /// <summary>
        /// The identifier for the <see cref="ScheduleAppointment.StartTime"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty StartTimeProperty = DependencyProperty.Register("StartTime", typeof(DateTime), typeof(ScheduleAppointment), new PropertyMetadata(null));


        #endregion

        #region EndTime

        /// <summary>
        /// Gets or sets the value indicating the end of the <see cref="ScheduleAppointment"/> duration.
        /// </summary>
        [TypeConverter(typeof(DateTimeTypeConverter))]
        public DateTime EndTime
        {
            get
            {
                return (DateTime)GetValue(EndTimeProperty);
            }

            set
            {
                SetValue(EndTimeProperty, value);
            }
        }

        /// <summary>
        /// The identifier for the <see cref="ScheduleAppointment.EndTime"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty EndTimeProperty = DependencyProperty.Register("EndTime", typeof(DateTime), typeof(ScheduleAppointment), new PropertyMetadata(null));

        #endregion

        #region Subject

        /// <summary>
        /// Gets or sets the subject of the <see cref="ScheduleAppointment"/>.
        /// </summary>
        /// <value>The subject.</value>
        public string Subject
        {
            get
            {
                return (string)GetValue(SubjectProperty);
            }

            set
            {
                SetValue(SubjectProperty, value);
            }
        }

        /// <summary>
        /// Event that is raised when <see cref="ScheduleAppointment.Subject"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback SubjectChanged;

        /// <summary>
        /// The identifier for the <see cref="ScheduleAppointment.Subject"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty SubjectProperty = DependencyProperty.Register(
            "Subject", typeof(string), typeof(ScheduleAppointment), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnSubjectChanged)));

        /// <summary>
        /// Calls OnSubjectChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSubjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScheduleAppointment instance = (ScheduleAppointment)d;
            instance.OnSubjectChanged(e);
        }

        /// <summary>
        /// Raises SubjectChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private void OnSubjectChanged(DependencyPropertyChangedEventArgs e)
        {
            this.HandleSubjectChanged();

            if (this.SubjectChanged != null)
            {
                this.SubjectChanged(this, e);
            }
        }

        #endregion

        #region Location

        /// <summary>
        /// Gets or sets the location of the <see cref="ScheduleAppointment"/>.
        /// </summary>
        /// <value>The location.</value>
        public string Location
        {
            get
            {
                return (string)GetValue(LocationProperty);
            }

            set
            {
                SetValue(LocationProperty, value);
            }
        }

        /// <summary>
        /// Event that is raised when <see cref="ScheduleAppointment.Location"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback LocationChanged;

        /// <summary>
        /// The identifier for the <see cref="ScheduleAppointment.Location"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty LocationProperty = DependencyProperty.Register(
            "Location", typeof(string), typeof(ScheduleAppointment), new PropertyMetadata(null, new PropertyChangedCallback(OnLocationChanged)));

        /// <summary>
        /// Calls OnLocationChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnLocationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScheduleAppointment instance = (ScheduleAppointment)d;
            instance.OnLocationChanged(e);
        }

        /// <summary>
        /// Raises LocationChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private void OnLocationChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.LocationChanged != null)
            {
                this.LocationChanged(this, e);
            }
        }

        #endregion

        #region Record

        /// <summary>
        /// Gets the bound object
        /// </summary>
        public RecordEntry Record
        {
            get
            {
                return (RecordEntry)GetValue(RecordProperty);
            }

            set
            {
                SetValue(RecordProperty, value);
            }
        }

        /// <summary>
        /// Event that is raised when <see cref="ScheduleAppointment.Location"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback RecordChanged;

        /// <summary>
        /// The identifier for the <see cref="ScheduleAppointment.Location"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty RecordProperty = DependencyProperty.Register(
            "Record", typeof(RecordEntry), typeof(ScheduleAppointment), new PropertyMetadata(null, new PropertyChangedCallback(OnRecordChanged)));

        /// <summary>
        /// Calls OnLocationChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnRecordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScheduleAppointment instance = (ScheduleAppointment)d;
            instance.OnRecordChanged(e);
        }

        /// <summary>
        /// Raises LocationChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private void OnRecordChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.RecordChanged != null)
            {
                this.RecordChanged(this, e);
            }
        }

        #endregion

        #region Notes

        /// <summary>
        /// Gets / Sets the Notes property.
        /// </summary>
        public string Notes
        {
            get { return (string)GetValue(NotesProperty); }
            set { SetValue(NotesProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for Notes.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty NotesProperty = DependencyProperty.Register("Notes", typeof(string), typeof(ScheduleAppointment), new PropertyMetadata(string.Empty));

        #endregion

        #region Status

        /// <summary>
        /// Gets or sets the subject of the <see cref="ScheduleAppointment"/>.
        /// </summary>
        /// <value>The subject.</value>
        public ScheduleAppointmentStatus Status
        {
            get
            {
                return (ScheduleAppointmentStatus)GetValue(StatusProperty);
            }

            set
            {
                SetValue(StatusProperty, value);
            }
        }

        /// <summary>
        /// The identifier for the <see cref="ScheduleAppointment.Subject"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register(
            "Status", typeof(ScheduleAppointmentStatus), typeof(ScheduleAppointment), new PropertyMetadata(null));

        #endregion

        #region SnoozeTime

        /// <summary>
        /// Gets or sets the value representing an Appointment to which this refers to.
        /// </summary>  
        private TimeSpan snoozeTime;

        /// <summary>
        /// Gets or sets Sooze time
        /// </summary>
        public TimeSpan SnoozeTime
        {
            get
            {
                return (TimeSpan)snoozeTime;
            }
            set
            {
                snoozeTime = value;
                if (this.ReminderTime < DateTime.Now)
                {
                    this.ReminderTime = DateTime.Now.AddTimeSpan(this.SnoozeTime);
                }
            }
        }

         ////<summary>
         ////The identifier for the <see cref="ScheduleReminder.Appointment"/> dependency property. 
         ////</summary>
        //public static read only DependencyProperty SnoozeTimeProperty = DependencyProperty.Register("SnoozeTime", typeof(TimeSpan), typeof(ScheduleReminder), new PropertyMetadata(null));

        #endregion

        #region ThresholdTime
        /// <summary>
        /// Gets or sets the value representing an Appointment to which this refers to.
        /// /// </summary>              
        private TimeSpan thresholdTime = new TimeSpan(0,15,0);
        /// <summary>
        /// Gets or sets threshold time
        /// </summary>
        public TimeSpan ThresholdTime
        {
            get
            {
                return thresholdTime;
            }
            set
            {
                TimeSpan old = thresholdTime;
                thresholdTime = value;
                if (this.StartTime > DateTime.Now.Add(old))
                {
                    this.ReminderTime = this.StartTime.Subtract(thresholdTime);
                }
            }
        }
         ////<summary>
         //// The identifier for the <see cref="ScheduleReminder.Appointment"/> dependency property. 
         //// </summary>
        //public static read only DependencyProperty DueTimeProperty = DependencyProperty.Register("DueTime", typeof(TimeSpan), typeof(ScheduleReminder), new PropertyMetadata(null));
        #endregion

        #region ReminderTime

        /// <summary>
        /// Gets or sets the value of Time when this has to be invoked.
        /// </summary>       
        private DateTime reminderTime;
        /// <summary>
        /// Gets or sets Reminder time
        /// </summary>
        public DateTime ReminderTime
        {
            get
            {
                return (DateTime)reminderTime;
            }
            set
            {
                reminderTime = value;
            }
        }

         ////<summary>
         ////The identifier for the <see cref="ScheduleReminder.Appointment"/> dependency property. 
         ////</summary>
        //public static readonly DependencyProperty ReminderTimeProperty = DependencyProperty.Register("ReminderTime", typeof(DateTime), typeof(ScheduleReminder), new PropertyMetadata(null));

        #endregion

        #region DueTime
        /// <summary>
        /// Gets or sets the value representing an Appointment to which this refers to.
        /// /// </summary>              
        private TimeSpan dueTime;
        /// <summary>
        /// Gets or sets DueTime
        /// </summary>
        public TimeSpan DueTime
        {
            get
            {
                return (TimeSpan)dueTime;
            }
            set
            {
                dueTime = value;
                this.DueTimeString = SetDueTimeString();
                if (value < new TimeSpan(0, 0, 0))
                {
                    this.DueTimeString = (this.DueTimeString.Replace("-","")) + " OverDue";                 
                }
                else if (value == new TimeSpan(0, 0, 0))
                {
                    this.DueTimeString = "Now";
                }
                else if (value > new TimeSpan(0, 0, 0))
                {
                    this.DueTimeString = this.DueTimeString;
                }
                dueTime = value;
            }
        }

        /// <summary>
        /// //  Using a DependencyProperty as the backing store for DueTime.  This //
        /// enables animation, styling, binding, etc... //
        /// </summary>
        public string SetDueTimeString()
        {
            string retstr = "";
            if (this.DueTime.Days != 0)
            {
                if (this.DueTime.Days == 1)
                {
                    retstr = retstr + this.DueTime.Days + " Day, ";
                }
                else
                {
                    retstr = retstr + this.DueTime.Days + " Days, ";
                }
            }
            if (this.DueTime.Hours != 0)
            {
                if (this.DueTime.Hours == 1)
                {
                    retstr = retstr + this.DueTime.Hours + " Hour, ";
                }
                else
                {
                    retstr = retstr + this.DueTime.Hours + " Hours, ";
                }
            }
            if (this.DueTime.Minutes != 0)
            {
                if (this.DueTime.Minutes == 1)
                {
                    retstr = retstr + this.DueTime.Minutes + " Minute, ";
                }
                else
                {
                    retstr = retstr + this.DueTime.Minutes + " Minutes";
                }
            }
            return retstr;
        }


        #endregion

        //#region Priority

        ///// <summary>
        ///// Gets or sets the value indicating the priority of the <see cref="ScheduleAppointment"/> duration.
        ///// </summary>
       
        //public AppointmentPriority Priority
        //{
        //    get
        //    {
        //        return (AppointmentPriority)GetValue(PriorityProperty);
        //    }

        //    set
        //    {
        //        SetValue(PriorityProperty, value);              
        //    }
        //}
        ///// <summary>
        ///// The identifier for the <see cref="ScheduleAppointment.Priority"/> dependency property. 
        ///// </summary>
        //public static readonly DependencyProperty PriorityProperty = DependencyProperty.Register("Priority", typeof(AppointmentPriority), typeof(ScheduleAppointment), new PropertyMetadata(AppointmentPriority.Busy, OnAppointmentPriorityChanged));

        //private static void OnAppointmentPriorityChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        //{   

        //}

        //#endregion

        #region DueTimeString
        private string dueTimeString;
        /// <summary>
        /// Gets or sets the due time string.
        /// </summary>
        /// <value>The due time string.</value>
        public string DueTimeString
        {
            get
            {
                return dueTimeString;
            }
            internal set
            {
                dueTimeString = value;
            }
        }
        #endregion
        #endregion

        #region Recurrence Dependency Properties

        #region RecurrenceAlertMessage
        /// <summary>
        /// Gets or sets the recurrence alert message.
        /// </summary>
        /// <value>The recurrence alert message.</value>
        public string RecurrenceAlertMessage
        {
            get
            {
                return (string)GetValue(RecurrenceAlertMessageProperty);
            }

            set
            {
                SetValue(RecurrenceAlertMessageProperty, value);
            }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for RecurrenceAlertMessage.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RecurrenceAlertMessageProperty = DependencyProperty.Register("RecurrenceAlertMessage", typeof(string),
            typeof(ScheduleAppointment), new PropertyMetadata(string.Empty));
        #endregion

        #region IsRecurrenceAppointment
        /// <summary>
        /// Gets or sets a value indicating whether this instance is recurrence appointment.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is recurrence appointment; otherwise, <c>false</c>.
        /// </value>
        public bool IsRecurrenceAppointment
        {
            get
            {
                return (bool)GetValue(IsRecurrenceAppointmentProperty);
            }

            set
            {
                SetValue(IsRecurrenceAppointmentProperty, value);
            }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsRecurrenceAppointment.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsRecurrenceAppointmentProperty = DependencyProperty.Register("IsRecurrenceAppointment", typeof(bool),
            typeof(ScheduleAppointment), new PropertyMetadata(false, OnIsRecurrenceAppointmentChanged));

        /// <summary>
        ///  method called when isRecurrence appointment changed
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args">An <see
        /// cref="T:System.Windows.DependencyPropertyChangedEventArgs"/> that contains the
        /// event data.</param>
        public static void OnIsRecurrenceAppointmentChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ScheduleAppointment instance = (ScheduleAppointment)obj;
            if (instance.IsRecurrenceAppointment == true)
            {
                instance.AllowDragandDrop = false;
                instance.AllowResize = false;
            }
            else
            {
                instance.AllowDragandDrop = true;
                instance.AllowResize = true;
            }
        }

        #endregion

        #region CurrentRecurrencePatternMode
        /// <summary>
        /// Gets or sets the current recurrence pattern mode.
        /// </summary>
        /// <value>The current recurrence pattern mode.</value>
        public RecurrencePatternMode CurrentRecurrencePatternMode
        {
            get { return (RecurrencePatternMode)GetValue(CurrentRecurrencePatternModeProperty); }
            set { SetValue(CurrentRecurrencePatternModeProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for CurrentRecurrencePatternMode.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CurrentRecurrencePatternModeProperty = DependencyProperty.Register("CurrentRecurrencePatternMode",
            typeof(RecurrencePatternMode), typeof(ScheduleAppointment), new PropertyMetadata(RecurrencePatternMode.Daily));
        #endregion

        #region EndOccurenceCount
        /// <summary>
        /// Gets or sets the end occurence count.
        /// </summary>
        /// <value>The end occurence count.</value>
        public int EndOccurenceCount
        {
            get
            {
                return (int)GetValue(EndOccurenceCountProperty);
            }

            set
            {
                SetValue(EndOccurenceCountProperty, value);
            }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for EndOccurenceCount.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EndOccurenceCountProperty = DependencyProperty.Register(
            "EndOccurenceCount", typeof(int), typeof(ScheduleAppointment), new PropertyMetadata(10));
        #endregion

        #region MonthlyWeekOrderSelected
        /// <summary>
        /// Gets or sets the monthly week order selected.
        /// </summary>
        /// <value>The monthly week order selected.</value>
        public String MonthlyWeekOrderSelected
        {
            get
            {
                return (String)GetValue(MonthlyWeekOrderProperty);
            }

            set
            {
                SetValue(MonthlyWeekOrderProperty, value);
            }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for MonthlyWeekOrderSelected.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MonthlyWeekOrderProperty = DependencyProperty.Register(
            "MonthlyWeekOrderSelected", typeof(String), typeof(ScheduleAppointment), new PropertyMetadata(""));
        #endregion

        #region MonthlyDaySelected
        /// <summary>
        /// Gets or sets the monthly day selected.
        /// </summary>
        /// <value>The monthly day selected.</value>
        public String MonthlyDaySelected
        {
            get
            {
                return (String)GetValue(MonthlyDaySelectedProperty);
            }

            set
            {
                SetValue(MonthlyDaySelectedProperty, value);
            }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for MonthlyDaySelected.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MonthlyDaySelectedProperty = DependencyProperty.Register(
            "MonthlyDaySelected", typeof(String), typeof(ScheduleAppointment), new PropertyMetadata(""));
        #endregion

        #region YearlyMonthSelected
        /// <summary>
        /// Gets or sets the yearly month selected.
        /// </summary>
        /// <value>The yearly month selected.</value>
        public String YearlyMonthSelected
        {
            get
            {
                return (String)GetValue(YearlyMonthSelectedProperty);
            }

            set
            {
                SetValue(YearlyMonthSelectedProperty, value);
            }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for YearlyMonthSelected.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty YearlyMonthSelectedProperty = DependencyProperty.Register(
            "YearlyMonthSelected", typeof(String), typeof(ScheduleAppointment), new PropertyMetadata(""));
        #endregion

        #region YearlyMultiWeekOrderSelected
        /// <summary>
        /// Gets or sets the yearly multi week order selected.
        /// </summary>
        /// <value>The yearly multi week order selected.</value>
        public String YearlyMultiWeekOrderSelected
        {
            get
            {
                return (String)GetValue(YearlyMultiWeekOrderProperty);
            }

            set
            {
                SetValue(YearlyMultiWeekOrderProperty, value);
            }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for YearlyMultiWeekOrderSelected.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty YearlyMultiWeekOrderProperty = DependencyProperty.Register(
            "YearlyMultiWeekOrderSelected", typeof(String), typeof(ScheduleAppointment), new PropertyMetadata(""));
        #endregion

        #region YearlyMultiDaySelected
        /// <summary>
        /// Gets or sets the yearly multi day selected.
        /// </summary>
        /// <value>The yearly multi day selected.</value>
        public String YearlyMultiDaySelected
        {
            get
            {
                return (String)GetValue(YearlyMultiDaySelectedProperty);
            }

            set
            {
                SetValue(YearlyMultiDaySelectedProperty, value);
            }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for YearlyMultiDaySelected.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty YearlyMultiDaySelectedProperty = DependencyProperty.Register(
            "YearlyMultiDaySelected", typeof(String), typeof(ScheduleAppointment), new PropertyMetadata(""));
        #endregion

        #region YearlyMultiMonthSelected
        /// <summary>
        /// Gets or sets the yearly multi month selected.
        /// </summary>
        /// <value>The yearly multi month selected.</value>
        public String YearlyMultiMonthSelected
        {
            get
            {
                return (String)GetValue(YearlyMultiMonthSelectedProperty);
            }

            set
            {
                SetValue(YearlyMultiMonthSelectedProperty, value);
            }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for YearlyMultiMonthSelected.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty YearlyMultiMonthSelectedProperty = DependencyProperty.Register(
            "YearlyMultiMonthSelected", typeof(String), typeof(ScheduleAppointment), new PropertyMetadata(""));
        #endregion

        #region DailyDays
        /// <summary>
        /// Gets or sets the daily days.
        /// </summary>
        /// <value>The daily days.</value>
        public int DailyDays
        {
            get
            {
                return (int)GetValue(DailyDaysProperty);
            }

            set
            {
                SetValue(DailyDaysProperty, value);
            }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for DailyDays.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DailyDaysProperty = DependencyProperty.Register(
            "DailyDays", typeof(int), typeof(ScheduleAppointment), new PropertyMetadata(1));
        #endregion

        #region WeeklyWeeks
        /// <summary>
        /// Gets or sets the weekly weeks.
        /// </summary>
        /// <value>The weekly weeks.</value>
        public int WeeklyWeeks
        {
            get
            {
                return (int)GetValue(WeeklyWeeksProperty);
            }

            set
            {
                SetValue(WeeklyWeeksProperty, value);
            }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for WeeklyWeeks.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty WeeklyWeeksProperty = DependencyProperty.Register(
            "WeeklyWeeks", typeof(int), typeof(ScheduleAppointment), new PropertyMetadata(1));
        #endregion

        #region MonthlyDays
        /// <summary>
        /// Gets or sets the monthly days.
        /// </summary>
        /// <value>The monthly days.</value>
        public int MonthlyDays
        {
            get
            {
                return (int)GetValue(MonthlyDaysProperty);
            }

            set
            {
                SetValue(MonthlyDaysProperty, value);
            }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for MonthlyDays.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MonthlyDaysProperty = DependencyProperty.Register(
            "MonthlyDays", typeof(int), typeof(ScheduleAppointment), new PropertyMetadata(DateTime.Now.Day));
        #endregion

        #region MonthlyMonth
        /// <summary>
        /// Gets or sets the monthly month.
        /// </summary>
        /// <value>The monthly month.</value>
        public int MonthlyMonth
        {
            get
            {
                return (int)GetValue(MonthlyMonthProperty);
            }

            set
            {
                SetValue(MonthlyMonthProperty, value);
            }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for MonthlyMonth.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MonthlyMonthProperty = DependencyProperty.Register(
            "MonthlyMonth", typeof(int), typeof(ScheduleAppointment), new PropertyMetadata(1));
        #endregion

        #region MonthlyMonthMulti
        /// <summary>
        /// Gets or sets the monthly month multi.
        /// </summary>
        /// <value>The monthly month multi.</value>
        public int MonthlyMonthMulti
        {
            get
            {
                return (int)GetValue(MonthlyMonthMultiProperty);
            }

            set
            {
                SetValue(MonthlyMonthMultiProperty, value);
            }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for MonthlyMonthMultiProperty.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MonthlyMonthMultiProperty = DependencyProperty.Register(
            "MonthlyMonthMulti", typeof(int), typeof(ScheduleAppointment), new PropertyMetadata(1));
        #endregion

        #region YearlyYear
        /// <summary>
        /// Gets or sets the yearly year.
        /// </summary>
        /// <value>The yearly year.</value>
        public int YearlyYear
        {
            get
            {
                return (int)GetValue(YearlyYearProperty);
            }

            set
            {
                SetValue(YearlyYearProperty, value);
            }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for YearlyYear.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty YearlyYearProperty = DependencyProperty.Register(
            "YearlyYear", typeof(int), typeof(ScheduleAppointment), new PropertyMetadata(1));
        #endregion

        #region YearlyDays
        /// <summary>
        /// Gets or sets the yearly days.
        /// </summary>
        /// <value>The yearly days.</value>
        public int YearlyDays
        {
            get
            {
                return (int)GetValue(YearlyDaysProperty);
            }

            set
            {
                SetValue(YearlyDaysProperty, value);
            }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for YearlyYear.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty YearlyDaysProperty = DependencyProperty.Register(
            "YearlyDays", typeof(int), typeof(ScheduleAppointment), new PropertyMetadata(DateTime.Now.Day));
        #endregion

        #region IsNoEndDateRecurrence
        /// <summary>
        /// Gets or sets a value indicating whether this instance is no end date recurrence.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is no end date recurrence; otherwise, <c>false</c>.
        /// </value>
        public bool IsNoEndDateRecurrence
        {
            get { return (bool)GetValue(IsNoEndDateRecurrenceProperty); }
            set { SetValue(IsNoEndDateRecurrenceProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsNoEndDateRecurrence.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsNoEndDateRecurrenceProperty = DependencyProperty.Register("IsNoEndDateRecurrence",
            typeof(bool), typeof(ScheduleAppointment), new PropertyMetadata(true));
        #endregion

        #region IsEndAfter
        /// <summary>
        /// Gets or sets a value indicating whether this instance is end after.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is end after; otherwise, <c>false</c>.
        /// </value>
        public bool IsEndAfter
        {
            get { return (bool)GetValue(IsEndAfterProperty); }
            set { SetValue(IsEndAfterProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsEndAfter.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsEndAfterProperty = DependencyProperty.Register("IsEndAfter",
            typeof(bool), typeof(ScheduleAppointment), new PropertyMetadata(false));
        #endregion

        #region IsEndBy
        /// <summary>
        /// Gets or sets a value indicating whether this instance is end by.
        /// </summary>
        /// <value><c>true</c> if this instance is end by; otherwise, <c>false</c>.</value>
        public bool IsEndBy
        {
            get { return (bool)GetValue(IsEndByProperty); }
            set { SetValue(IsEndByProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsEndBy.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsEndByProperty = DependencyProperty.Register("IsEndBy",
            typeof(bool), typeof(ScheduleAppointment), new PropertyMetadata(false));
        #endregion

        #region IsWeeklySundaySelected
        /// <summary>
        /// Gets or sets a value indicating whether this instance is weekly sunday selected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is weekly sunday selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsWeeklySundaySelected
        {
            get { return (bool)GetValue(IsWeeklySundaySelectedProperty); }
            set { SetValue(IsWeeklySundaySelectedProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsWeeklySundaySelected.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsWeeklySundaySelectedProperty = DependencyProperty.Register("IsWeeklySundaySelected",
            typeof(bool), typeof(ScheduleAppointment), new PropertyMetadata(false));
        #endregion

        #region IsWeeklyMondaySelected
        /// <summary>
        /// Gets or sets a value indicating whether this instance is weekly monday selected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is weekly monday selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsWeeklyMondaySelected
        {
            get { return (bool)GetValue(IsWeeklyMondaySelectedProperty); }
            set { SetValue(IsWeeklyMondaySelectedProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsWeeklyMondaySelected.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsWeeklyMondaySelectedProperty = DependencyProperty.Register("IsWeeklyMondaySelected",
            typeof(bool), typeof(ScheduleAppointment), new PropertyMetadata(false));
        #endregion

        #region IsWeeklyTuesdaySelected
        /// <summary>
        /// Gets or sets a value indicating whether this instance is weekly tuesday selected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is weekly tuesday selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsWeeklyTuesdaySelected
        {
            get { return (bool)GetValue(IsWeeklyTuesdaySelectedProperty); }
            set { SetValue(IsWeeklyTuesdaySelectedProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsWeeklyTuesdaySelected.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsWeeklyTuesdaySelectedProperty = DependencyProperty.Register("IsWeeklyTuesdaySelected",
            typeof(bool), typeof(ScheduleAppointment), new PropertyMetadata(false));
        #endregion

        #region IsWeeklyWednesdaySelected
        /// <summary>
        /// Gets or sets a value indicating whether this instance is weekly wednesday selected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is weekly wednesday selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsWeeklyWednesdaySelected
        {
            get { return (bool)GetValue(IsWeeklyWednesdaySelectedProperty); }
            set { SetValue(IsWeeklyWednesdaySelectedProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsWeeklyWednesdaySelected.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsWeeklyWednesdaySelectedProperty = DependencyProperty.Register("IsWeeklyWednesdaySelected",
            typeof(bool), typeof(ScheduleAppointment), new PropertyMetadata(false));
        #endregion

        #region IsWeeklyThursdaySelected
        /// <summary>
        /// Gets or sets a value indicating whether this instance is weekly thursday selected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is weekly thursday selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsWeeklyThursdaySelected
        {
            get { return (bool)GetValue(IsWeeklyThursdaySelectedProperty); }
            set { SetValue(IsWeeklyThursdaySelectedProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsWeeklyThursdaySelected.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsWeeklyThursdaySelectedProperty = DependencyProperty.Register("IsWeeklyThursdaySelected",
            typeof(bool), typeof(ScheduleAppointment), new PropertyMetadata(false));
        #endregion

        #region IsWeeklyFridaySelected
        /// <summary>
        /// Gets or sets a value indicating whether this instance is weekly friday selected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is weekly friday selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsWeeklyFridaySelected
        {
            get { return (bool)GetValue(IsWeeklyFridaySelectedProperty); }
            set { SetValue(IsWeeklyFridaySelectedProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsWeeklyFridaySelected.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsWeeklyFridaySelectedProperty = DependencyProperty.Register("IsWeeklyFridaySelected",
            typeof(bool), typeof(ScheduleAppointment), new PropertyMetadata(false));
        #endregion

        #region IsWeeklySaturdaySelected
        /// <summary>
        /// Gets or sets a value indicating whether this instance is weekly saturday selected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is weekly saturday selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsWeeklySaturdaySelected
        {
            get { return (bool)GetValue(IsWeeklySaturdaySelectedProperty); }
            set { SetValue(IsWeeklySaturdaySelectedProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsWeeklySaturdaySelected.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsWeeklySaturdaySelectedProperty = DependencyProperty.Register("IsWeeklySaturdaySelected",
            typeof(bool), typeof(ScheduleAppointment), new PropertyMetadata(false));
        #endregion

        #region IsDailyCustomDays
        /// <summary>
        /// Gets or sets a value indicating whether this instance is daily custom days.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is daily custom days; otherwise, <c>false</c>.
        /// </value>
        public bool IsDailyCustomDays
        {
            get { return (bool)GetValue(IsDailyCustomDaysProperty); }
            set { SetValue(IsDailyCustomDaysProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsDailyCustomDays.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsDailyCustomDaysProperty = DependencyProperty.Register("IsDailyCustomDays",
            typeof(bool), typeof(ScheduleAppointment), new PropertyMetadata(true));
        #endregion

        #region IsDailyWeekDays
        /// <summary>
        /// Gets or sets a value indicating whether this instance is daily week days.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is daily week days; otherwise, <c>false</c>.
        /// </value>
        public bool IsDailyWeekDays
        {
            get { return (bool)GetValue(IsDailyWeekDaysProperty); }
            set { SetValue(IsDailyWeekDaysProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsDailyWeekDays.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsDailyWeekDaysProperty = DependencyProperty.Register("IsDailyWeekDays",
            typeof(bool), typeof(ScheduleAppointment), new PropertyMetadata(false));
        #endregion

        #region IsMonthlyCustomDays
        /// <summary>
        /// Gets or sets a value indicating whether this instance is monthly custom days.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is monthly custom days; otherwise, <c>false</c>.
        /// </value>
        public bool IsMonthlyCustomDays
        {
            get { return (bool)GetValue(IsMonthlyCustomDaysProperty); }
            set { SetValue(IsMonthlyCustomDaysProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsMonthlyCustomDays.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsMonthlyCustomDaysProperty = DependencyProperty.Register("IsMonthlyCustomDays",
            typeof(bool), typeof(ScheduleAppointment), new PropertyMetadata(true));
        #endregion

        #region IsMonthlyMultiDays
        /// <summary>
        /// Gets or sets a value indicating whether this instance is monthly multi days.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is monthly multi days; otherwise, <c>false</c>.
        /// </value>
        public bool IsMonthlyMultiDays
        {
            get { return (bool)GetValue(IsMonthlyMultiDaysProperty); }
            set { SetValue(IsMonthlyMultiDaysProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsMonthlyMultiDays.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsMonthlyMultiDaysProperty = DependencyProperty.Register("IsMonthlyMultiDays",
            typeof(bool), typeof(ScheduleAppointment), new PropertyMetadata(false));
        #endregion

        #region IsYearlyCustomDays
        /// <summary>
        /// Gets or sets a value indicating whether this instance is yearly custom days.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is yearly custom days; otherwise, <c>false</c>.
        /// </value>
        public bool IsYearlyCustomDays
        {
            get { return (bool)GetValue(IsYearlyCustomDaysProperty); }
            set { SetValue(IsYearlyCustomDaysProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsYearlyCustomDays.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsYearlyCustomDaysProperty = DependencyProperty.Register("IsYearlyCustomDays",
            typeof(bool), typeof(ScheduleAppointment), new PropertyMetadata(true));
        #endregion

        #region IsYearlyMultiDays
        /// <summary>
        /// Gets or sets a value indicating whether this instance is yearly multi days.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is yearly multi days; otherwise, <c>false</c>.
        /// </value>
        public bool IsYearlyMultiDays
        {
            get { return (bool)GetValue(IsYearlyMultiDaysProperty); }
            set { SetValue(IsYearlyMultiDaysProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsYearlyMultiDays.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsYearlyMultiDaysProperty = DependencyProperty.Register("IsYearlyMultiDays",
            typeof(bool), typeof(ScheduleAppointment), new PropertyMetadata(false));
        #endregion

        #region IsDailySelected
        /// <summary>
        /// Gets or sets a value indicating whether this instance is daily selected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is daily selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsDailySelected
        {
            get { return (bool)GetValue(IsDailySelectedProperty); }
            set { SetValue(IsDailySelectedProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsDailySelected.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsDailySelectedProperty = DependencyProperty.Register("IsDailySelected",
            typeof(bool), typeof(ScheduleAppointment), new PropertyMetadata(true));
        #endregion

        #region IsWeeklySelected
        /// <summary>
        /// Gets or sets a value indicating whether this instance is weekly selected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is weekly selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsWeeklySelected
        {
            get { return (bool)GetValue(IsWeeklySelectedProperty); }
            set { SetValue(IsWeeklySelectedProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsWeeklySelected.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsWeeklySelectedProperty = DependencyProperty.Register("IsWeeklySelected",
            typeof(bool), typeof(ScheduleAppointment), new PropertyMetadata(false));
        #endregion

        #region IsMonthlySelected
        /// <summary>
        /// Gets or sets a value indicating whether this instance is monthly selected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is monthly selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsMonthlySelected
        {
            get { return (bool)GetValue(IsMonthlySelectedProperty); }
            set { SetValue(IsMonthlySelectedProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsMonthlySelected.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsMonthlySelectedProperty = DependencyProperty.Register("IsMonthlySelected",
            typeof(bool), typeof(ScheduleAppointment), new PropertyMetadata(false));
        #endregion

        #region IsYearlySelected
        /// <summary>
        /// Gets or sets a value indicating whether this instance is yearly selected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is yearly selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsYearlySelected
        {
            get { return (bool)GetValue(IsYearlySelectedProperty); }
            set { SetValue(IsYearlySelectedProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsYearlySelected.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsYearlySelectedProperty = DependencyProperty.Register("IsYearlySelected",
            typeof(bool), typeof(ScheduleAppointment), new PropertyMetadata(false));
        #endregion

        #region StartRecurrenceTime

        /// <summary>
        /// Gets or sets the value indicating the start of the <see cref="ScheduleAppointment"/> duration.
        /// </summary>
        [TypeConverter(typeof(DateTimeTypeConverter))]
        public DateTime StartRecurrenceTime
        {
            get
            {
                return (DateTime)GetValue(StartRecurrenceTimeProperty);
            }

            set
            {
                SetValue(StartRecurrenceTimeProperty, value);
            }
        }

        /// <summary>
        /// The identifier for the <see cref="ScheduleAppointment.StartRecurrenceTime"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty StartRecurrenceTimeProperty = DependencyProperty.Register("StartRecurrenceTime", typeof(DateTime), typeof(ScheduleAppointment), new PropertyMetadata(null));


        #endregion

        #region EndRecurrenceTime

        /// <summary>
        /// Gets or sets the value indicating the end of the <see cref="ScheduleAppointment"/> duration.
        /// </summary>
        [TypeConverter(typeof(DateTimeTypeConverter))]
        public DateTime EndRecurrenceTime
        {
            get
            {
                return (DateTime)GetValue(EndRecurrenceTimeProperty);
            }

            set
            {
                SetValue(EndRecurrenceTimeProperty, value);
            }
        }

        /// <summary>
        /// The identifier for the <see cref="ScheduleAppointment.EndRecurrenceTime"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty EndRecurrenceTimeProperty = DependencyProperty.Register("EndRecurrenceTime", typeof(DateTime), typeof(ScheduleAppointment), new PropertyMetadata(null));

        #endregion

        #region IsDismissed
        /// <summary>
        /// Gets or sets the value representing an Appointment to which this refers to.
        /// /// </summary>              
        private bool isDismissed;
        /// <summary>
        /// Gets or sets a value indicating whether this instance .
        /// </summary>
        /// <value>
        /// <see langword="true"/> if this instance ; otherwise, <see langword="false"/>.
        /// </value>
        public bool IsDismissed
        {
            get
            {
                return (bool)isDismissed;
            }
            set
            {
                isDismissed = value;
            }
        }
       
        #endregion
        #endregion

        #region IsPrivate
        /// <summary>
        /// Gets or sets a value indicating whether [is Private].
        /// </summary>
        /// <value><c>true</c> if [Is Private]; otherwise, <c>false</c>.</value>
        public bool IsPrivate
        {
            get
            {
                return (bool)GetValue(IsPrivateProperty);
            }

            set
            {
                SetValue(IsPrivateProperty, value);
            }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsPrivate.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsPrivateProperty = DependencyProperty.Register("IsPrivate",
            typeof(bool), typeof(ScheduleAppointment), new PropertyMetadata(false));
        #endregion

        #region IsLowImportance
        /// <summary>
        /// Gets or sets a value indicating whether [Appointment Importance].
        /// </summary>
        /// <value><c>Low</c> if [Appointment Importance is Low]; otherwise, <c>High</c>.</value>
        public bool IsLowImportance
        {
            get
            {
                return (bool)GetValue(IsLowImportanceProperty);
            }

            set
            {
                SetValue(IsLowImportanceProperty, value);
            }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsLowImportance.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsLowImportanceProperty = DependencyProperty.Register("IsLowImportance",
            typeof(bool), typeof(ScheduleAppointment) , new PropertyMetadata(false));
        #endregion

        #region IsHighImportance
        /// <summary>
        /// Gets or sets a value indicating whether [Appointment Importance].
        /// </summary>
        /// <value><c>Low</c> if [Appointment Importance is Low]; otherwise, <c>High</c>.</value>
        public bool IsHighImportance
        {
            get
            {
                return (bool)GetValue(IsHighImportanceProperty);
            }

            set
            {
                SetValue(IsHighImportanceProperty, value);
            }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsHighImportance.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsHighImportanceProperty = DependencyProperty.Register("IsHighImportance",
            typeof(bool), typeof(ScheduleAppointment), new PropertyMetadata(false));
        #endregion

        #region Properties

        private Int64 iD = 0;
        /// <summary>
        /// Gets or sets integer ID
        /// </summary>
        public Int64 ID
        {
            get { return iD; }
            set { iD = value; }
        }

         ////<summary>
         ////Gets , Sets the ScheduleResource's Owner.
         ////</summary>
        //internal ScheduleResource Owner
        //{
        //    get
        //    {
        //        return this.owner;
        //    }

        //    set
        //    {
        //        this.owner = value;
        //    }
        //}

        /// <summary>
        /// Gets , Sets the ScheduleAppointment's RecurrenceParent.
        /// </summary>
        internal ScheduleAppointment RecurrenceParent
        {
            get
            {
                return this.recurrenceParent;
            }
            set
            {
                this.recurrenceParent = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="ScheduleAppointment"/>'s duration.
        /// </summary>
        public TimeSpan Duration
        {
            get
            {
                return this.EndTime - this.StartTime;
            }

        }

        /// <summary>
        /// Gets or sets a value indicating whether update or not .
        /// </summary>
        /// <value>
        /// <see langword="true"/> if ; otherwise, <see langword="false"/>.
        /// </value>
        public bool updating
        {
            get;
            set;
        }

        internal double timeInterval
        {
            get;
            set;
        }

        #endregion

        #region Methods

        #region Recurrence Methods

        private void SetCurrentWeekBool(DayOfWeek selectedDayofWeek)
        {
            if (this.IsWeeklySundaySelected == true || this.IsWeeklyMondaySelected == true || this.IsWeeklyTuesdaySelected == true ||
                this.IsWeeklyWednesdaySelected == true || this.IsWeeklyThursdaySelected == true || this.IsWeeklyFridaySelected == true ||
                this.IsWeeklySaturdaySelected == true)
            {
                return;
            }
            switch (selectedDayofWeek)
            {
                case DayOfWeek.Sunday:
                    this.IsWeeklySundaySelected = true;
                    break;
                case DayOfWeek.Monday:
                    this.IsWeeklyMondaySelected = true;
                    break;
                case DayOfWeek.Tuesday:
                    this.IsWeeklyTuesdaySelected = true;
                    break;
                case DayOfWeek.Wednesday:
                    this.IsWeeklyWednesdaySelected = true;
                    break;
                case DayOfWeek.Thursday:
                    this.IsWeeklyThursdaySelected = true;
                    break;
                case DayOfWeek.Friday:
                    this.IsWeeklyFridaySelected = true;
                    break;
                case DayOfWeek.Saturday:
                    this.IsWeeklySaturdaySelected = true;
                    break;
            }
        }

        internal ScheduleAppointment CheckForAppointment(DateTime currDate)
        {
            if (this.IsRecurrenceAppointment == false) return null;
            if (this.IsEndBy)
            {
                if (this.EndRecurrenceTime.Subtract(currDate).Days < 0) return null;
            }

            var diff = currDate.Date.Subtract(this.StartRecurrenceTime.Date);
            if (this.IsEndAfter)
            {
                var count = 0;
                for (DateTime todate = this.StartRecurrenceTime.Date; todate < currDate; todate = todate.AddDays(1))
                {
                    var app = CheckForAppointmentAvailablity(todate);
                    if (app != null)
                    {
                        count++;
                    }
                }
                if (count >= this.EndOccurenceCount) return null;
            }
            /*if (this.IsEndAfter)
            {
                var tempOccu = this.EndOccurenceCount;
                tempOccu = (DailyDays > 1) ? diff.Days * DailyDays : tempOccu;
                if (diff.Days >= tempOccu) return null;
            }*/
            switch (this.CurrentRecurrencePatternMode)
            {
                case RecurrencePatternMode.Daily:
                    return this.GetAppointmentsFromDailyMode(currDate);
                case RecurrencePatternMode.Weekly:
                    return this.GetAppointmentsFromWeeklyMode(currDate);
                case RecurrencePatternMode.Monthly:
                    return this.GetAppointmentsFromMonthlyMode(currDate);
                case RecurrencePatternMode.Yearly:
                    return this.GetAppointmentsFromYearlyMode(currDate);
            }
            return null;
        }

        internal ScheduleAppointment CheckForAppointmentAvailablity(DateTime currDate)
        {
            if (this.IsRecurrenceAppointment == false) return null;

            switch (this.CurrentRecurrencePatternMode)
            {
                case RecurrencePatternMode.Daily:
                    return this.GetAppointmentsFromDailyMode(currDate);
                case RecurrencePatternMode.Weekly:
                    return this.GetAppointmentsFromWeeklyMode(currDate);
                case RecurrencePatternMode.Monthly:
                    return this.GetAppointmentsFromMonthlyMode(currDate);
                case RecurrencePatternMode.Yearly:
                    return this.GetAppointmentsFromYearlyMode(currDate);
            }
            return null;
        }

        private ScheduleAppointment GetAppointmentsFromDailyMode(DateTime currDate)
        {
            if (this.StartRecurrenceTime.Date > currDate.Date) return null;

            if (this.IsDailyCustomDays)
            {
                var diff = currDate.Date.Subtract(this.StartRecurrenceTime.Date);
                if ((DailyDays == 1) || (diff.Days >= DailyDays && diff.Days % DailyDays == 0)
                    || (currDate.Date == this.StartRecurrenceTime.Date))
                    return this.GetEquivalentScheduleAppointment(currDate);
            }
            else
            {
                if (currDate.DayOfWeek == DayOfWeek.Sunday || currDate.DayOfWeek == DayOfWeek.Saturday) return null;
                return this.GetEquivalentScheduleAppointment(currDate);
            }
            return null;
        }

        private ScheduleAppointment GetAppointmentsFromWeeklyMode(DateTime currDate)
        {
            if (this.StartRecurrenceTime.Date > currDate.Date) return null;

            var selectedDayofWeek = currDate.DayOfWeek;
            SetCurrentWeekBool(this.StartRecurrenceTime.DayOfWeek);

            if (this.WeeklyWeeks > 1 && currDate.Date != this.StartRecurrenceTime.Date)
            {
                int currenday = GetEquivalentIntegerForDay(this.StartRecurrenceTime.DayOfWeek);
                var diff = currDate.Date.Subtract(this.StartRecurrenceTime.Date).Days;
                currenday = diff - (7 - currenday);
                double diffinWeek = Math.Floor(Convert.ToDouble(currenday) / 7) + ((Convert.ToDouble(currenday) % 7 > 0) ? 1 : 0);
                if (diffinWeek % WeeklyWeeks > 0) return null;
            }

            if (this.IsWeeklySundaySelected && selectedDayofWeek == DayOfWeek.Sunday)
            {
                return this.GetEquivalentScheduleAppointment(currDate);
            }
            else if (this.IsWeeklyMondaySelected && selectedDayofWeek == DayOfWeek.Monday)
            {
                return this.GetEquivalentScheduleAppointment(currDate);
            }
            else if (this.IsWeeklyTuesdaySelected && selectedDayofWeek == DayOfWeek.Tuesday)
            {
                return this.GetEquivalentScheduleAppointment(currDate);
            }
            else if (this.IsWeeklyWednesdaySelected && selectedDayofWeek == DayOfWeek.Wednesday)
            {
                return this.GetEquivalentScheduleAppointment(currDate);
            }
            else if (this.IsWeeklyThursdaySelected && selectedDayofWeek == DayOfWeek.Thursday)
            {
                return this.GetEquivalentScheduleAppointment(currDate);
            }
            else if (this.IsWeeklyFridaySelected && selectedDayofWeek == DayOfWeek.Friday)
            {
                return this.GetEquivalentScheduleAppointment(currDate);
            }
            else if (this.IsWeeklySaturdaySelected && selectedDayofWeek == DayOfWeek.Saturday)
            {
                return this.GetEquivalentScheduleAppointment(currDate);
            }

            return null;
        }

        private ScheduleAppointment GetAppointmentsFromMonthlyMode(DateTime currDate)
        {
            if (this.StartRecurrenceTime.Date > currDate.Date) return null;

            int monthsDiffernce = currDate.Month - StartRecurrenceTime.Month + (12 * (currDate.Year - StartRecurrenceTime.Year));

            if (this.IsMonthlyCustomDays)
            {
                if ((this.MonthlyMonth == 1 && this.MonthlyDays == currDate.Date.Day) ||
                    (MonthlyDays == currDate.Date.Day && monthsDiffernce >= this.MonthlyMonth && monthsDiffernce % this.MonthlyMonth == 0))
                    return this.GetEquivalentScheduleAppointment(currDate);
            }
            else
            {
                if (this.MonthlyMonthMulti == 1 ||
                    (monthsDiffernce >= this.MonthlyMonthMulti && monthsDiffernce % this.MonthlyMonthMulti == 0)
                    || (currDate.Date == this.StartRecurrenceTime.Date))
                {
                    string getcurrweekorder = "";

                    if (this.MonthlyDaySelected == "day")
                    {
                        getcurrweekorder = GetCurrentOrderInIntNormalFormat(currDate);
                    }
                    else if (this.MonthlyDaySelected == "weekday")
                    {
                        getcurrweekorder = this.GetCurrrentWeekDayOrder(currDate, this.YearlyMultiWeekOrderSelected);
                    }
                    else
                    {
                        getcurrweekorder = this.GetCurrrentDayOrder(currDate, this.MonthlyWeekOrderSelected);
                    }

                    if (this.MonthlyWeekOrderSelected != getcurrweekorder) return null;

                    switch (this.MonthlyDaySelected)
                    {
                        case "day":
                            return this.GetEquivalentScheduleAppointment(currDate);
                        case "weekday":
                            if (currDate.Date.DayOfWeek != DayOfWeek.Sunday && currDate.Date.DayOfWeek != DayOfWeek.Saturday)
                            {
                                return this.GetEquivalentScheduleAppointment(currDate);
                            }
                            break;
                        case "weekend day":
                            if (currDate.Date.DayOfWeek == DayOfWeek.Saturday)
                            {
                                return this.GetEquivalentScheduleAppointment(currDate);
                            }
                            break;
                        case "Sunday":
                            if (currDate.Date.DayOfWeek == DayOfWeek.Sunday)
                            {
                                return this.GetEquivalentScheduleAppointment(currDate);
                            }
                            break;
                        case "Monday":
                            if (currDate.Date.DayOfWeek == DayOfWeek.Monday)
                            {
                                return this.GetEquivalentScheduleAppointment(currDate);
                            }
                            break;
                        case "Tuesday":
                            if (currDate.Date.DayOfWeek == DayOfWeek.Tuesday)
                            {
                                return this.GetEquivalentScheduleAppointment(currDate);
                            }
                            break;
                        case "Wednesday":
                            if (currDate.Date.DayOfWeek == DayOfWeek.Wednesday)
                            {
                                return this.GetEquivalentScheduleAppointment(currDate);
                            }
                            break;
                        case "Thursday":
                            if (currDate.Date.DayOfWeek == DayOfWeek.Thursday)
                            {
                                return this.GetEquivalentScheduleAppointment(currDate);
                            }
                            break;
                        case "Friday":
                            if (currDate.Date.DayOfWeek == DayOfWeek.Friday)
                            {
                                return this.GetEquivalentScheduleAppointment(currDate);
                            }
                            break;
                        case "Saturday":
                            if (currDate.Date.DayOfWeek == DayOfWeek.Saturday)
                            {
                                return this.GetEquivalentScheduleAppointment(currDate);
                            }
                            break;
                    }
                }
            }

            return null;
        }

        private ScheduleAppointment GetAppointmentsFromYearlyMode(DateTime currDate)
        {
            if (this.StartRecurrenceTime.Date > currDate.Date) return null;

            var selectedmonth = currDate.ToString("MMMM");
            int yearDiffernce = currDate.Year - StartRecurrenceTime.Year;

            if (this.IsYearlyCustomDays)
            {
                if (((this.YearlyYear == 1) || (yearDiffernce > 0 && yearDiffernce >= this.YearlyYear && yearDiffernce % this.YearlyYear == 0)
                    || (currDate.Date == this.StartRecurrenceTime.Date))
                     && this.YearlyDays == currDate.Date.Day && this.YearlyMonthSelected == selectedmonth)
                {
                    return this.GetEquivalentScheduleAppointment(currDate);
                }
            }
            else if (((this.YearlyYear == 1) || (yearDiffernce > 0 && yearDiffernce >= this.YearlyYear && yearDiffernce % this.YearlyYear == 0)
                    || (currDate.Date == this.StartRecurrenceTime.Date))
                     && this.YearlyMultiMonthSelected == selectedmonth)
            {
                string getcurrweekorder = "";

                if (this.YearlyMultiDaySelected == "day")
                {
                    getcurrweekorder = GetCurrentOrderInIntNormalFormat(currDate);
                }
                else if (this.YearlyMultiDaySelected == "weekday")
                {
                    getcurrweekorder = this.GetCurrrentWeekDayOrder(currDate, this.YearlyMultiWeekOrderSelected);
                }
                else
                {
                    getcurrweekorder = this.GetCurrrentDayOrder(currDate, this.YearlyMultiWeekOrderSelected);
                }

                if (this.YearlyMultiWeekOrderSelected != getcurrweekorder) return null;
                
                switch (this.YearlyMultiDaySelected)
                {
                    case "day":
                        return this.GetEquivalentScheduleAppointment(currDate);
                    case "weekday":
                        if (currDate.Date.DayOfWeek != DayOfWeek.Sunday && currDate.Date.DayOfWeek != DayOfWeek.Saturday)
                        {
                            return this.GetEquivalentScheduleAppointment(currDate);
                        }
                        break;
                    case "weekend day":
                        if(currDate.Date.DayOfWeek == DayOfWeek.Saturday)
                        {
                            return this.GetEquivalentScheduleAppointment(currDate);
                        }
                        break;
                    case "Sunday":
                        if (currDate.Date.DayOfWeek == DayOfWeek.Sunday)
                        {
                            return this.GetEquivalentScheduleAppointment(currDate);
                        }
                        break;
                    case "Monday":
                        if (currDate.Date.DayOfWeek == DayOfWeek.Monday)
                        {
                            return this.GetEquivalentScheduleAppointment(currDate);
                        }
                        break;
                    case "Tuesday":
                        if (currDate.Date.DayOfWeek == DayOfWeek.Tuesday)
                        {
                            return this.GetEquivalentScheduleAppointment(currDate);
                        }
                        break;
                    case "Wednesday":
                        if (currDate.Date.DayOfWeek == DayOfWeek.Wednesday)
                        {
                            return this.GetEquivalentScheduleAppointment(currDate);
                        }
                        break;
                    case "Thursday":
                        if (currDate.Date.DayOfWeek == DayOfWeek.Thursday)
                        {
                            return this.GetEquivalentScheduleAppointment(currDate);
                        }
                        break;
                    case "Friday":
                        if (currDate.Date.DayOfWeek == DayOfWeek.Friday)
                        {
                            return this.GetEquivalentScheduleAppointment(currDate);
                        }
                        break;
                    case "Saturday":
                        if (currDate.Date.DayOfWeek == DayOfWeek.Saturday)
                        {
                            return this.GetEquivalentScheduleAppointment(currDate);
                        }
                        break;
                }
            }

            return null;
        }

        private ScheduleAppointment GetEquivalentScheduleAppointment(DateTime currDate)
        {
            var appointment = new ScheduleAppointment();
            appointment.InitializeFromApp(this);
            appointment.ID = this.ID;
            appointment.StartTime = currDate.AddHours(this.StartTime.Hour).AddMinutes(this.StartTime.Minute).AddSeconds(this.StartTime.Second);
             int dayCount = EndTime.Day - StartTime.Day;
             appointment.EndTime = currDate.AddDays(dayCount).AddHours(this.EndTime.Hour).AddMinutes(this.EndTime.Minute).AddSeconds(this.EndTime.Second);
            return appointment;
        }

        internal string GetCurrrentDayOrder(DateTime currDate, string order)
        {
            int i = 0;
            var currentDayofWeek = currDate.DayOfWeek;
            Dictionary<DateTime, int> ListedDates = new Dictionary<DateTime, int>();

            for (int j = 1; j <= DateTime.DaysInMonth(currDate.Year, currDate.Month); j++)
            {
                var newDate = new DateTime(currDate.Year, currDate.Month, j);
                if (currentDayofWeek == newDate.DayOfWeek)
                {
                    i++;
                    ListedDates.Add(newDate, i);
                }
            }

            i = (from res in ListedDates
                 where res.Key.Date == currDate.Date
                 select res).FirstOrDefault().Value;

            if (order == "last")
            {
                if (i == ListedDates.Count)
                {
                    i = 5;
                }
            }
            switch (i)
            {
                case 1:
                    return "first";
                case 2:
                    return "second";
                case 3:
                    return "third";
                case 4:
                    return "fourth";
                default:
                    return "last";
            }
        }

        internal string GetCurrrentWeekDayOrder(DateTime currDate, string order)
        {
            int i = 0;
            var currentDayofWeek = currDate.DayOfWeek;
            Dictionary<DateTime, int> ListedDates = new Dictionary<DateTime, int>();

            for (int j = 1; j <= DateTime.DaysInMonth(currDate.Year, currDate.Month); j++)
            {
                var newDate = new DateTime(currDate.Year, currDate.Month, j);
                if (newDate.DayOfWeek == DayOfWeek.Sunday || newDate.DayOfWeek == DayOfWeek.Saturday) continue;
                if (i >= 4)
                {
                    newDate = new DateTime(currDate.Year, currDate.Month, DateTime.DaysInMonth(currDate.Year, currDate.Month));
                    if (newDate.DayOfWeek == DayOfWeek.Saturday)
                    {
                        newDate = newDate.AddDays(-1);
                    }
                    else if (newDate.DayOfWeek == DayOfWeek.Sunday)
                    {
                        newDate = newDate.AddDays(-2);
                    }
                    ListedDates.Add(newDate, 5);
                    break;
                }
                i++;
                ListedDates.Add(newDate, i);
            }

            i = (from res in ListedDates
                 where res.Key.Date == currDate.Date
                 select res).FirstOrDefault().Value;

            if (order == "last")
            {
                if (i == ListedDates.Count)
                {
                    i = 5;
                }
            }
            switch (i)
            {
                case 0:
                    return "";
                case 1:
                    return "first";
                case 2:
                    return "second";
                case 3:
                    return "third";
                case 4:
                    return "fourth";
                default:
                    return "last";
            }
        }

        internal string GetCurrentOrderInIntNormalFormat(DateTime selecteddate)
        {
            int i = selecteddate.Day;
            int lastday = DateTime.DaysInMonth(selecteddate.Year, selecteddate.Month);
            if (lastday == i) return "last";
            switch (i)
            {
                case 1:
                    return "first";
                case 2:
                    return "second";
                case 3:
                    return "third";
                case 4:
                    return "fourth";
                default:
                    return "";
            }
        }

        internal Int32 GetCurrentOrderInIntFormat(DateTime selecteddate)
        {
            int i = selecteddate.Day;
            GetWeekValue(selecteddate, ref i);

            switch (i)
            {
                case 1:
                    return 1;
                case 2:
                    return 2;
                case 3:
                    return 3;
                case 4:
                    return 4;
                default:
                    return DateTime.DaysInMonth(selecteddate.Year, selecteddate.Month);
            }
        }

        internal string GetCurrentWeekInStringFormat(DateTime selecteddate)
        {
            int i = selecteddate.Day;

            GetWeekValue(selecteddate, ref i);
            switch (i)
            {
                case 1:
                    return "first";
                case 2:
                    return "second";
                case 3:
                    return "third";
                case 4:
                    return "fourth";
                default:
                    return "last";
            }
        }

        private void GetWeekValue(DateTime selecteddate, ref int i)
        {
            DayOfWeek dywek = CultureInfo.CurrentCulture.Calendar.GetDayOfWeek(selecteddate.AddDays(-(selecteddate.Day - 1)));

            switch (dywek)
            {
                case DayOfWeek.Monday:
                    i += 1;
                    break;
                case DayOfWeek.Tuesday:
                    i += 2;
                    break;
                case DayOfWeek.Wednesday:
                    i += 3;
                    break;
                case DayOfWeek.Thursday:
                    i += 4;
                    break;
                case DayOfWeek.Friday:
                    i += 5;
                    break;
                case DayOfWeek.Saturday:
                    i += 6;
                    break;
            }
            i = (((i) % 7) > 0) ? ((i) / 7) + 1 : ((i) / 7);
        }

        private int GetEquivalentIntegerForDay(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return 1;
                case DayOfWeek.Monday:
                    return 2;
                case DayOfWeek.Tuesday:
                    return 3;
                case DayOfWeek.Wednesday:
                    return 4;
                case DayOfWeek.Thursday:
                    return 5;
                case DayOfWeek.Friday:
                    return 6;
                case DayOfWeek.Saturday:
                    return 7;
            }
            return 1;
        }
        #endregion

        /// <summary>
        /// Check if the appointment is intersecting.
        /// </summary>
        /// <param name="appointment">The Appointment that needs to be checked for intersection.</param>
        /// <returns></returns>
        internal bool IsIntersecting(ScheduleAppointment appointment)
        {
            if (this == appointment)
            {
                return false;
            }

            //return this.StartTime <= appointment.EndTime && appointment.StartTime <= this.EndTime;
            return this.StartTime < appointment.EndTime && appointment.StartTime < this.EndTime;
        }

        internal bool IsAllDayOrSpanned()
        {
            if (this.AllDay)
            {
                return true;
            }
            

            var currentDate = this.StartTime.Date;
            var nextDate = currentDate.AddDays(1);
            return this.EndTime >= nextDate;
        }

        internal bool IsAllDayOrSpanned(DateTime currentDate)
        {
            if (this.AllDay)
            {
                return true;
            }

            var nextDate = currentDate.AddDays(1);
            return this.StartTime >= currentDate && this.StartTime <= nextDate && (this.EndTime >= nextDate || this.AllDay);
        }

        /// <summary>
        /// Verify Start and End Time.
        /// </summary>
        /// <param name="startTime">Start Time of Appointment</param>
        /// <param name="endTime">End Time of Appointment</param>
        /// <returns></returns>
        internal static bool VerifyTime(ref DateTime startTime, ref DateTime endTime)
        {
            bool result = true;

            if (startTime.Ticks > 0 && endTime.Ticks > 0)
            {
                result = startTime < endTime;
            }

            return result;
        }

        internal void InitializeFromApp(ScheduleAppointment app)
        {
            this.AllDay = app.AllDay;
            this.ID = app.ID;
            this.AllowResize = app.AllowResize;
            this.AllowRecurrence = app.AllowRecurrence;
            this.DueTime = app.DueTime;
            this.DueTimeString = app.DueTimeString;
            this.StartTime = app.StartTime;
            this.EndTime = app.EndTime;
            this.IsDismissed = app.IsDismissed;
            this.Location = app.Location;
            this.Notes = app.Notes;
            this.Record = app.Record;
            this.ReminderTime = app.ReminderTime;
            this.SnoozeTime = app.SnoozeTime;
            this.Subject = app.Subject;
            this.ThresholdTime = app.ThresholdTime;
            this.timeInterval = app.timeInterval;
            this.updating = app.updating;
            //this.Priority = app.Priority;
            this.Status = app.Status;
            this.IsPrivate = app.IsPrivate;
            this.IsLowImportance = app.IsLowImportance;
            this.IsHighImportance = app.IsHighImportance;
            //this.AppointmentImportance = app.AppointmentImportance;
            this.MultiDayAppointment = app.MultiDayAppointment;
            this.MultiDayAppointmentStartTime = app.MultiDayAppointmentStartTime;
            this.MultiDayAppointmentEndTime = app.MultiDayAppointmentEndTime; this.AppointmentProxy = app.AppointmentProxy;
            this.IsAppointmentProxyCollection = app.IsAppointmentProxyCollection;

            this.RecurrenceAlertMessage = app.RecurrenceAlertMessage;
            this.IsRecurrenceAppointment = app.IsRecurrenceAppointment;

            this.StartRecurrenceTime = app.StartRecurrenceTime;
            this.EndRecurrenceTime = app.EndRecurrenceTime;

            this.DailyDays = app.DailyDays;
            this.WeeklyWeeks = app.WeeklyWeeks;
            this.MonthlyDays = app.MonthlyDays;
            this.MonthlyMonth = app.MonthlyMonth;
            this.MonthlyMonthMulti = app.MonthlyMonthMulti;
            this.YearlyYear = app.YearlyYear;
            this.YearlyDays = app.YearlyDays;

            this.IsDailySelected = app.IsDailySelected;
            this.IsMonthlySelected = app.IsMonthlySelected;
            this.IsWeeklySelected = app.IsWeeklySelected;
            this.IsYearlySelected = app.IsYearlySelected;

            this.IsDailyCustomDays = app.IsDailyCustomDays;
            this.IsDailyWeekDays = app.IsDailyWeekDays;
            this.IsMonthlyCustomDays = app.IsMonthlyCustomDays;
            this.IsMonthlyMultiDays = app.IsMonthlyMultiDays;
            this.IsYearlyCustomDays = app.IsYearlyCustomDays;
            this.IsYearlyMultiDays = app.IsYearlyMultiDays;

            this.MonthlyWeekOrderSelected = app.MonthlyWeekOrderSelected;
            this.MonthlyDaySelected = app.MonthlyDaySelected;
            this.YearlyMonthSelected = app.YearlyMonthSelected;
            this.YearlyMultiWeekOrderSelected = app.YearlyMultiWeekOrderSelected;
            this.YearlyMultiDaySelected = app.YearlyMultiDaySelected;
            this.YearlyMultiMonthSelected = app.YearlyMultiMonthSelected;

            this.IsNoEndDateRecurrence = app.IsNoEndDateRecurrence;
            this.IsEndAfter = app.IsEndAfter;
            this.IsEndBy = app.IsEndBy;
            this.EndOccurenceCount = app.EndOccurenceCount;

            this.IsWeeklySundaySelected = app.IsWeeklySundaySelected;
            this.IsWeeklyMondaySelected = app.IsWeeklyMondaySelected;
            this.IsWeeklyTuesdaySelected = app.IsWeeklyTuesdaySelected;
            this.IsWeeklyWednesdaySelected = app.IsWeeklyWednesdaySelected;
            this.IsWeeklyThursdaySelected = app.IsWeeklyThursdaySelected;
            this.IsWeeklyFridaySelected = app.IsWeeklyFridaySelected;
            this.IsWeeklySaturdaySelected = app.IsWeeklySaturdaySelected;

            this.CurrentRecurrencePatternMode = app.CurrentRecurrencePatternMode;
        }

        internal void InitializeFromApp(ScheduleHolidays app)
        {
            this.AllDay = app.AllDay;
            this.ID = app.ID;
            this.AllowResize = app.AllowResize;
            this.AllowRecurrence = app.AllowRecurrence;
            this.DueTime = app.DueTime;
            this.DueTimeString = app.DueTimeString;
            this.StartTime = app.StartTime;
            this.EndTime = app.EndTime;
            this.IsDismissed = app.IsDismissed;
            this.Location = app.Location;
            this.Notes = app.Notes;
            this.Record = app.Record;
            this.ReminderTime = app.ReminderTime;
            this.SnoozeTime = app.SnoozeTime;
            this.Subject = app.Subject;
            this.ThresholdTime = app.ThresholdTime;
            this.timeInterval = app.timeInterval;
            this.updating = app.updating;
            this.Status = app.Status;

            this.IsPrivate = app.IsPrivate;
            this.IsLowImportance = app.IsLowImportance;
            this.IsHighImportance = app.IsHighImportance;
            this.AppointmentProxy = app.AppointmentProxy;
            this.IsAppointmentProxyCollection = app.IsAppointmentProxyCollection;

            this.RecurrenceAlertMessage = app.RecurrenceAlertMessage;
            this.IsRecurrenceAppointment = app.IsRecurrenceAppointment;

            this.StartRecurrenceTime = app.StartRecurrenceTime;
            this.EndRecurrenceTime = app.EndRecurrenceTime;

            this.DailyDays = app.DailyDays;
            this.WeeklyWeeks = app.WeeklyWeeks;
            this.MonthlyDays = app.MonthlyDays;
            this.MonthlyMonth = app.MonthlyMonth;
            this.MonthlyMonthMulti = app.MonthlyMonthMulti;
            this.YearlyYear = app.YearlyYear;
            this.YearlyDays = app.YearlyDays;

            this.IsDailySelected = app.IsDailySelected;
            this.IsMonthlySelected = app.IsMonthlySelected;
            this.IsWeeklySelected = app.IsWeeklySelected;
            this.IsYearlySelected = app.IsYearlySelected;

            this.IsDailyCustomDays = app.IsDailyCustomDays;
            this.IsDailyWeekDays = app.IsDailyWeekDays;
            this.IsMonthlyCustomDays = app.IsMonthlyCustomDays;
            this.IsMonthlyMultiDays = app.IsMonthlyMultiDays;
            this.IsYearlyCustomDays = app.IsYearlyCustomDays;
            this.IsYearlyMultiDays = app.IsYearlyMultiDays;

            this.MonthlyWeekOrderSelected = app.MonthlyWeekOrderSelected;
            this.MonthlyDaySelected = app.MonthlyDaySelected;
            this.YearlyMonthSelected = app.YearlyMonthSelected;
            this.YearlyMultiWeekOrderSelected = app.YearlyMultiWeekOrderSelected;
            this.YearlyMultiDaySelected = app.YearlyMultiDaySelected;
            this.YearlyMultiMonthSelected = app.YearlyMultiMonthSelected;

            this.IsNoEndDateRecurrence = app.IsNoEndDateRecurrence;
            this.IsEndAfter = app.IsEndAfter;
            this.IsEndBy = app.IsEndBy;
            this.EndOccurenceCount = app.EndOccurenceCount;

            this.IsWeeklySundaySelected = app.IsWeeklySundaySelected;
            this.IsWeeklyMondaySelected = app.IsWeeklyMondaySelected;
            this.IsWeeklyTuesdaySelected = app.IsWeeklyTuesdaySelected;
            this.IsWeeklyWednesdaySelected = app.IsWeeklyWednesdaySelected;
            this.IsWeeklyThursdaySelected = app.IsWeeklyThursdaySelected;
            this.IsWeeklyFridaySelected = app.IsWeeklyFridaySelected;
            this.IsWeeklySaturdaySelected = app.IsWeeklySaturdaySelected;

            this.CurrentRecurrencePatternMode = app.CurrentRecurrencePatternMode;
        }

        internal void InitializeFrom(ScheduleAppointmentProxy proxy)
        {
            this.AllDay = proxy.AllDay;
            this.ID = proxy.ID;
            this.AllowResize = proxy.AllowResize;
            this.AllowRecurrence = proxy.AllowRecurrence;
            this.DueTime = proxy.DueTime;
            this.DueTimeString = proxy.DueTimeString;
            this.StartTime = proxy.StartTime.Date.AddTimeSpan(proxy.StartTimeSpan);
            this.EndTime = proxy.EndTime.Date.AddTimeSpan(proxy.EndTimeSpan);
            this.IsDismissed = proxy.IsDismissed;
            this.Location = proxy.Location;
            this.Notes = proxy.Notes;
            this.Record = proxy.Record;
            this.ReminderTime = proxy.ReminderTime;
            this.SnoozeTime = proxy.SnoozeTime;
            this.Subject = proxy.Subject;
            this.ThresholdTime = proxy.ThresholdTime;
            this.timeInterval = proxy.timeInterval;
            this.updating = proxy.updating;
            this.AppointmentProxy = proxy.AppointmentProxy;
            this.IsAppointmentProxyCollection = proxy.IsAppointmentProxyCollection;
            this.Status = proxy.Status;
            this.IsPrivate = proxy.IsPrivate;            
            this.IsLowImportance = proxy.IsLowImportance;
            this.IsHighImportance = proxy.IsHighImportance;
            this.RecurrenceAlertMessage = proxy.RecurrenceAlertMessage;
            this.MultiDayAppointment = proxy.MultiDayAppointment;
            this.MultiDayAppointmentEndTime = proxy.EndTime;
            this.MultiDayAppointmentStartTime = proxy.StartTime; this.RecurrenceAlertMessage = proxy.RecurrenceAlertMessage;
            this.IsRecurrenceAppointment = proxy.IsRecurrenceAppointment;
            this.StartRecurrenceTime = proxy.StartRecurrenceTime;
            this.EndRecurrenceTime = proxy.EndRecurrenceTime;

            this.DailyDays = proxy.DailyDays;
            this.WeeklyWeeks = proxy.WeeklyWeeks;
            this.MonthlyDays = proxy.MonthlyDays;
            this.MonthlyMonth = proxy.MonthlyMonth;
            this.MonthlyMonthMulti = proxy.MonthlyMonthMulti;
            this.YearlyYear = proxy.YearlyYear;
            this.YearlyDays = proxy.YearlyDays;

            this.IsDailySelected = proxy.IsDailySelected;
            this.IsMonthlySelected = proxy.IsMonthlySelected;
            this.IsWeeklySelected = proxy.IsWeeklySelected;
            this.IsYearlySelected = proxy.IsYearlySelected;

            this.IsDailyCustomDays = proxy.IsDailyCustomDays;
            this.IsDailyWeekDays = proxy.IsDailyWeekDays;
            this.IsMonthlyCustomDays = proxy.IsMonthlyCustomDays;
            this.IsMonthlyMultiDays = proxy.IsMonthlyMultiDays;
            this.IsYearlyCustomDays = proxy.IsYearlyCustomDays;
            this.IsYearlyMultiDays = proxy.IsYearlyMultiDays;

            this.MonthlyWeekOrderSelected = proxy.MonthlyWeekOrderSelected;
            this.MonthlyDaySelected = proxy.MonthlyDaySelected;
            this.YearlyMonthSelected = proxy.YearlyMonthSelected;
            this.YearlyMultiWeekOrderSelected = proxy.YearlyMultiWeekOrderSelected;
            this.YearlyMultiDaySelected = proxy.YearlyMultiDaySelected;
            this.YearlyMultiMonthSelected = proxy.YearlyMultiMonthSelected;

            this.IsWeeklySundaySelected = proxy.IsWeeklySundaySelected;
            this.IsWeeklyMondaySelected = proxy.IsWeeklyMondaySelected;
            this.IsWeeklyTuesdaySelected = proxy.IsWeeklyTuesdaySelected;
            this.IsWeeklyWednesdaySelected = proxy.IsWeeklyWednesdaySelected;
            this.IsWeeklyThursdaySelected = proxy.IsWeeklyThursdaySelected;
            this.IsWeeklyFridaySelected = proxy.IsWeeklyFridaySelected;
            this.IsWeeklySaturdaySelected = proxy.IsWeeklySaturdaySelected;

            this.IsNoEndDateRecurrence = proxy.IsNoEndDateRecurrence;
            this.IsEndAfter = proxy.IsEndAfter;
            this.IsEndBy = proxy.IsEndBy;
            this.EndOccurenceCount = proxy.EndOccurenceCount;

            this.CurrentRecurrencePatternMode = proxy.CurrentRecurrencePatternMode;
           
        }

        #endregion

        #region Implementation


        /// <summary>
        /// Verify the Start Time is before End Time
        /// </summary>
        private void VerifyTime()
        {
            DateTime startTime = this.StartTime;
            DateTime endTime = this.EndTime;

            if (!VerifyTime(ref startTime, ref endTime) && !updating)
            {
                //throw new InvalidOperationException("Start time must be before end time.");
                this.EndTime = this.StartTime.Add(new TimeSpan(0, Convert.ToInt32(this.timeInterval), 0));



            }
        }

        /// <summary>
        /// Verify Recurrence Pattern
        /// </summary>
        private void VerifyRecurrenceIfNecessary()
        {
            //if (this.AllowRecurrence && this.Recurrence != null)
            //{
            //    this.Recurrence.VerifyRecurrencePattern();
            //}
        }

        /// <summary>
        /// Handles Change of Start Time
        /// </summary>
        private void HandleStartTimeChanged()
        {
            this.VerifyTime();
            this.VerifyRecurrenceIfNecessary();
        }

        /// <summary>
        /// Handles Change of End Time
        /// </summary>
        private void HandleEndTimeChanged()
        {
            this.VerifyTime();
            this.VerifyRecurrenceIfNecessary();
        }

        /// <summary>
        /// Handle Recurrence Changed.
        /// </summary>
        /// <param name="e">Contains Original Recurrence and Changed Recurrence</param>
        private void HandleRecurrenceChanged(DependencyPropertyChangedEventArgs e)
        {
            //ScheduleRecurrence oldValue = e.OldValue as ScheduleRecurrence;
            //ScheduleRecurrence newValue = e.NewValue as ScheduleRecurrence;

            //if (oldValue != null)
            //{
            //    oldValue.Owner = null;
            //}

            //if (newValue != null)
            //{
            //    newValue.Owner = this;
            //    this.Recurrence.VerifyRecurrencePattern();
            //}
        }


        private void HandleSubjectChanged()
        {
            if (String.IsNullOrEmpty(this.Subject))
            {

                //throw new ArgumentException("Subject can't be empty.");
            }
        }

        #endregion

        #region Copy/clone

        /// <summary>
        /// Copies properties from a <see cref="ScheduleAppointment"/> to another one.
        /// </summary>
        /// <param name="source">The source <see cref="ScheduleAppointment"/>.</param>
        /// <param name="target">The target <see cref="ScheduleAppointment"/>.</param>
        /// <remarks>
        // /// This method doesn't copy <see cref="ScheduleAppointment.Owner"/> and <see cref="ScheduleAppointment.Recurrence"/> properties
        /// </remarks>
        internal static void Copy(ScheduleAppointment source, ScheduleAppointment target)
        {
            target.AllDay = source.AllDay;
            //target.AllowDrag = source.AllowDrag;
            //target.AllowRecurrence = source.AllowRecurrence;
            //target.AllowResize = source.AllowResize;
            //target.Background = source.Background;
            //target.Blocked = source.Blocked;
            //target.CustomTooltip = source.CustomTooltip;
            target.StartTime = new DateTime();
            target.EndTime = source.EndTime;
            target.StartTime = source.StartTime;
            target.Subject = source.Subject;
            //target.TimeSpanBrush = source.TimeSpanBrush;
            //target.Tooltip = source.Tooltip;
            target.Location = source.Location;
            //target.owner = source.owner;

        }

        internal ScheduleAppointment Clone()
        {
            ScheduleAppointment app = new ScheduleAppointment();
            app.timeInterval = this.timeInterval;
            Copy(this, app);
            return app;
        }

        #endregion

        internal bool MatchWithExists(ScheduleAppointment scheduleAppointment)
        {
            if (scheduleAppointment == null) return false;
           //if (scheduleAppointment.ID == this.ID && scheduleAppointment.IsRecurrenceAppointment == false && (scheduleAppointment.CurrentAppointmentType == AppointmentType.Normal || scheduleAppointment.currentAppointmentType == AppointmentType.Holiday)&& (this.StartTime == scheduleAppointment.StartTime && this.EndTime == scheduleAppointment.EndTime)) return true;
            if (scheduleAppointment.ID == this.ID &&
                scheduleAppointment.IsRecurrenceAppointment == false &&
                (scheduleAppointment.CurrentAppointmentType == AppointmentType.Normal ||
                scheduleAppointment.currentAppointmentType == AppointmentType.Holiday ||
                scheduleAppointment.CurrentAppointmentType == AppointmentType.MultiDay)) 
                return true;
            else 
                if (scheduleAppointment.ID == this.ID &&
                    (scheduleAppointment.IsRecurrenceAppointment == true ||
                    scheduleAppointment.CurrentAppointmentType == AppointmentType.MultiWeek )&&
                (this.StartTime == scheduleAppointment.StartTime && this.EndTime == scheduleAppointment.EndTime)) return true;
            else return false;
        }

        internal bool MatchWithExists(ScheduleAppointment scheduleAppointment,DateTime date)
        {
            if (scheduleAppointment == null) return false;

            if (scheduleAppointment.ID == this.ID && (scheduleAppointment.CurrentAppointmentType == AppointmentType.MultiDay && (this.StartTime.Date==scheduleAppointment.StartTime.Date ||this.EndTime.Date==scheduleAppointment.EndTime.Date)))
            return true;
            else return false;
        }
    }


    /// <summary>
    /// Represents a collection of <see cref="Schedule"/>'s <see cref="ScheduleAppointment"/>s.
    /// </summary>
#if SyncfusionFramework4_0 && !SILVERLIGHT
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ScheduleAppointmentCollection :
        ObservableCollection<ScheduleAppointment>
    {
        #region Overrides

        /// <summary>
        /// Inserts an item into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert.</param>
        protected override void InsertItem(int index, ScheduleAppointment item)
        {
            if (!Contains(item))
            {
                var nextday = item.StartTime.Date.AddDays(1);
                var diff = item.EndTime.Date - item.StartTime.Date;
                if (diff.Days >=1)
                {
                    item.MultiDayAppointment = true;
                    item.MultiDayAppointmentStartTime = item.StartTime;
                    item.MultiDayAppointmentEndTime = item.EndTime;
                    item.CurrentAppointmentType = AppointmentType.MultiDay;
                }
                base.InsertItem(index, item);
            }
        }

        /// <summary>
        /// Replaces the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to replace.</param>
        /// <param name="item">The new value for the element at the specified index.</param>
        protected override void SetItem(int index, ScheduleAppointment item)
        {
            if (!Contains(item))
            {
                //this.VerifyItem(item, this[index]);

                base.SetItem(index, item);
            }
        }

        #endregion

        #region Implementation

        internal static bool CanContain(IEnumerable<ScheduleAppointment> apps, ScheduleAppointment app, ScheduleAppointment appOrig)
        {
            bool result = false;

            //if (app.Blocked)
            //{
            //    result = CanContainBlockedAppointment(apps, app, appOrig);
            //}
            //else
            {
                result = CanContainAppointment(apps, app, appOrig);
            }

            return result;
        }

        internal bool CanContain(ScheduleAppointment app, ScheduleAppointment appOrig)
        {
            return CanContain(this, app, appOrig);
        }

        private static bool CanContainAppointment(IEnumerable<ScheduleAppointment> apps, ScheduleAppointment app, ScheduleAppointment appOrig)
        {
            return !apps.Any(a => a != app && a != appOrig /*&& a.Blocked*/ && a.IsIntersecting(app));
        }

        private static bool CanContainBlockedAppointment(IEnumerable<ScheduleAppointment> apps, ScheduleAppointment app, ScheduleAppointment appOrig)
        {
            return !apps.Any(a => a != app && a != appOrig && a.IsIntersecting(app));
        }

        private void VerifyItem(ScheduleAppointment item, ScheduleAppointment appOrig)
        {
            //if (item.Blocked)
            //{
            //    if (!CanContainBlockedAppointment(this, item, appOrig))
            //    {
            //        throw new InvalidOperationException("Can't insert blocked appointment.");
            //    }
            //}
            //else
            //{
            if (!CanContainAppointment(this, item, appOrig))
            {
                throw new InvalidOperationException("Can't insert blocking appointment.");
            }
            //}
        }

        #endregion
    }

#if SyncfusionFramework4_0 && !SILVERLIGHT
    /// <summary>
    ///  class that holds schedule appointment mapping
    /// </summary>
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ScheduleAppointmentMapping : DependencyObject
    {
        #region IDMapping (DependencyProperty)

        /// <summary>
        /// Gets or sets the subject mapping.
        /// </summary>
        /// <value>The subject mapping.</value>
        public string IDMapping
        {
            get { return (string)GetValue(IDMappingProperty); }
            set { SetValue(IDMappingProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IDMapping.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IDMappingProperty =
            DependencyProperty.Register("IDMapping", typeof(string), typeof(ScheduleAppointmentMapping),
              new PropertyMetadata(string.Empty));

        #endregion

        #region SubjectMapping (DependencyProperty)

        /// <summary>
        /// Gets or sets the subject mapping.
        /// </summary>
        /// <value>The subject mapping.</value>
        public string SubjectMapping
        {
            get { return (string)GetValue(SubjectMappingProperty); }
            set { SetValue(SubjectMappingProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for SubjectMapping.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SubjectMappingProperty =
            DependencyProperty.Register("SubjectMapping", typeof(string), typeof(ScheduleAppointmentMapping),
              new PropertyMetadata(string.Empty));

        #endregion

        #region StartTimeMapping (DependencyProperty)

        /// <summary>
        /// Gets or sets the start time mapping.
        /// </summary>
        /// <value>The start time mapping.</value>
        public string StartTimeMapping
        {
            get { return (string)GetValue(StartTimeMappingProperty); }
            set { SetValue(StartTimeMappingProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for StartTimeMapping.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StartTimeMappingProperty =
            DependencyProperty.Register("StartTimeMapping", typeof(string), typeof(ScheduleAppointmentMapping),
              new PropertyMetadata(string.Empty));

        #endregion

        #region EndTimeMapping (DependencyProperty)

        /// <summary>
        /// Gets / Sets the EndTimeMapping property for the bound object.
        /// </summary>
        public string EndTimeMapping
        {
            get { return (string)GetValue(EndTimeMappingProperty); }
            set { SetValue(EndTimeMappingProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for EndTimeMapping.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EndTimeMappingProperty =
            DependencyProperty.Register("EndTimeMapping", typeof(string), typeof(ScheduleAppointmentMapping),
              new PropertyMetadata(string.Empty));

        #endregion

        #region LocationMapping (DependencyProperty)

        /// <summary>
        /// Gets / Sets the Location mapping property for the bound object.
        /// </summary>
        public string LocationMapping
        {
            get { return (string)GetValue(LocationMappingProperty); }
            set { SetValue(LocationMappingProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for LocationMapping.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LocationMappingProperty =
            DependencyProperty.Register("LocationMapping", typeof(string), typeof(ScheduleAppointmentMapping),
              new PropertyMetadata(string.Empty));

        #endregion

        #region AllDayMapping (DependencyProperty)

        /// <summary>
        /// Gets / Sets the AllDay mapping property on the bound object.
        /// </summary>
        public string AllDayMapping
        {
            get { return (string)GetValue(AllDayMappingProperty); }
            set { SetValue(AllDayMappingProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for AllDayMapping.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AllDayMappingProperty =
            DependencyProperty.Register("AllDayMapping", typeof(string), typeof(ScheduleAppointmentMapping),
              new PropertyMetadata(string.Empty));

        #endregion

        #region AllowDragandDropMapping (DependencyProperty)

        /// <summary>
        /// Gets / Sets the AllDay mapping property on the bound object.
        /// </summary>
        public string AllowDragandDropMapping
        {
            get { return (string)GetValue(AllowDragandDropMappingProperty); }
            set { SetValue(AllowDragandDropMappingProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for AllowDragandDropMapping.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AllowDragandDropMappingProperty =
            DependencyProperty.Register("AllowDragandDropMapping", typeof(string), typeof(ScheduleAppointmentMapping),
              new PropertyMetadata(string.Empty));

        #endregion

        #region AllowRecurrenceMapping (DependencyProperty)

        /// <summary>
        /// Gets / Sets the AllDay mapping property on the bound object.
        /// </summary>
        public string AllowRecurrenceMapping
        {
            get { return (string)GetValue(AllowRecurrenceMappingProperty); }
            set { SetValue(AllowRecurrenceMappingProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for AllowRecurrenceMapping.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AllowRecurrenceMappingProperty =
            DependencyProperty.Register("AllowRecurrenceMapping", typeof(string), typeof(ScheduleAppointmentMapping),
              new PropertyMetadata(string.Empty));

        #endregion

        #region AllowResizeMapping (DependencyProperty)

        /// <summary>
        /// Gets / Sets the AllDay mapping property on the bound object.
        /// </summary>
        public string AllowResizeMapping
        {
            get { return (string)GetValue(AllowResizeMappingProperty); }
            set { SetValue(AllowResizeMappingProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for AllowResizeMapping.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AllowResizeMappingProperty =
            DependencyProperty.Register("AllowResizeMapping", typeof(string), typeof(ScheduleAppointmentMapping),
              new PropertyMetadata(string.Empty));

        #endregion

        #region DueTimeMapping (DependencyProperty)

        /// <summary>
        /// Gets or sets the start time mapping.
        /// </summary>
        /// <value>The start time mapping.</value>
        public string DueTimeMapping
        {
            get { return (string)GetValue(DueTimeMappingProperty); }
            set { SetValue(DueTimeMappingProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for DueTimeMapping.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DueTimeMappingProperty =
            DependencyProperty.Register("DueTimeMapping", typeof(string), typeof(ScheduleAppointmentMapping),
              new PropertyMetadata(string.Empty));

        #endregion

        #region IsLowImportanceMapping (DependencyProperty)

        /// <summary>
        /// Gets / Sets the AllDay mapping property on the bound object.
        /// </summary>
        public string IsLowImportanceMapping
        {
            get { return (string)GetValue(IsLowImportanceMappingProperty); }
            set { SetValue(IsLowImportanceMappingProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsLowImportanceMapping.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsLowImportanceMappingProperty =
            DependencyProperty.Register("IsLowImportanceMapping", typeof(string), typeof(ScheduleAppointmentMapping),
              new PropertyMetadata(string.Empty));

        #endregion

        #region IsHighImportanceMapping (DependencyProperty)

        /// <summary>
        /// Gets / Sets the AllDay mapping property on the bound object.
        /// </summary>
        public string IsHighImportanceMapping
        {
            get { return (string)GetValue(IsHighImportanceMappingProperty); }
            set { SetValue(IsHighImportanceMappingProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsHighImportanceMapping.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsHighImportanceMappingProperty =
            DependencyProperty.Register("IsHighImportanceMapping", typeof(string), typeof(ScheduleAppointmentMapping),
              new PropertyMetadata(string.Empty));

        #endregion

        #region IsPrivateMapping (DependencyProperty)

        /// <summary>
        /// Gets / Sets the AllDay mapping property on the bound object.
        /// </summary>
        public string IsPrivateMapping
        {
            get { return (string)GetValue(IsPrivateMappingProperty); }
            set { SetValue(IsPrivateMappingProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsPrivateMapping.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsPrivateMappingProperty =
            DependencyProperty.Register("IsPrivateMapping", typeof(string), typeof(ScheduleAppointmentMapping),
              new PropertyMetadata(string.Empty));

        #endregion

        #region IsRecurranceAppointmentMapping (DependencyProperty)

        /// <summary>
        /// Gets / Sets the AllDay mapping property on the bound object.
        /// </summary>
        public string IsRecurranceAppointmentMapping
        {
            get { return (string)GetValue(IsRecurranceAppointmentMappingProperty); }
            set { SetValue(IsRecurranceAppointmentMappingProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsRecurranceAppointmentMapping.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsRecurranceAppointmentMappingProperty =
            DependencyProperty.Register("IsRecurranceAppointmentMapping", typeof(string), typeof(ScheduleAppointmentMapping),
              new PropertyMetadata(string.Empty));

        #endregion

        #region NotestMapping (DependencyProperty)

        /// <summary>
        /// Gets / Sets the AllDay mapping property on the bound object.
        /// </summary>
        public string NotestMapping
        {
            get { return (string)GetValue(NotestMappingProperty); }
            set { SetValue(NotestMappingProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for NotestMapping.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty NotestMappingProperty =
            DependencyProperty.Register("NotestMapping", typeof(string), typeof(ScheduleAppointmentMapping),
              new PropertyMetadata(string.Empty));

        #endregion

        #region RecurrenceAlertMessageMapping (DependencyProperty)

        /// <summary>
        /// Gets / Sets the AllDay mapping property on the bound object.
        /// </summary>
        public string RecurrenceAlertMessageMapping
        {
            get { return (string)GetValue(RecurrenceAlertMessageMappingProperty); }
            set { SetValue(RecurrenceAlertMessageMappingProperty, value); }
        }

        /// <summary>
        ///  This Property Sets or Gets the string value for the recurrence alert message
        /// mapping.
        /// </summary>
        public static readonly DependencyProperty RecurrenceAlertMessageMappingProperty =
            DependencyProperty.Register("RecurrenceAlertMessageMapping", typeof(string), typeof(ScheduleAppointmentMapping),
              new PropertyMetadata(string.Empty));

        #endregion

        #region ReminderTimeMapping (DependencyProperty)

        /// <summary>
        /// Gets / Sets the AllDay mapping property on the bound object.
        /// </summary>
        public string ReminderTimeMapping
        {
            get { return (string)GetValue(ReminderTimeMappingProperty); }
            set { SetValue(ReminderTimeMappingProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for ReminderTimeMapping.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ReminderTimeMappingProperty =
            DependencyProperty.Register("ReminderTimeMapping", typeof(string), typeof(ScheduleAppointmentMapping),
              new PropertyMetadata(string.Empty));

        #endregion

        #region SnoozeTimeMapping (DependencyProperty)

        /// <summary>
        /// Gets / Sets the AllDay mapping property on the bound object.
        /// </summary>
        public string SnoozeTimeMapping
        {
            get { return (string)GetValue(SnoozeTimeMappingProperty); }
            set { SetValue(SnoozeTimeMappingProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for SnoozeTimeMapping.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SnoozeTimeMappingProperty =
            DependencyProperty.Register("SnoozeTimeMapping", typeof(string), typeof(ScheduleAppointmentMapping),
              new PropertyMetadata(string.Empty));

        #endregion

        #region RecurrenceStringMapping (DependencyProperty)

        /// <summary>
        /// Gets / Sets the RecurrenceString Mapping property on the bound object.
        /// </summary>
        public string RecurrenceStringMapping
        {
            get { return (string)GetValue(RecurrenceStringMappingProperty); }
            set { SetValue(RecurrenceStringMappingProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for RecurrenceStringMapping.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RecurrenceStringMappingProperty =
            DependencyProperty.Register("RecurrenceStringMapping", typeof(string), typeof(ScheduleAppointmentMapping),
              new PropertyMetadata(string.Empty));

        #endregion

        #region StatusMapping (DependencyProperty)

        /// <summary>
        /// Gets / Sets the AllDay mapping property on the bound object.
        /// </summary>
        public string StatusMapping
        {
            get { return (string)GetValue(StatusMappingProperty); }
            set { SetValue(StatusMappingProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for StatusMapping.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StatusMappingProperty =
            DependencyProperty.Register("StatusMapping", typeof(string), typeof(ScheduleAppointmentMapping),
              new PropertyMetadata(string.Empty));

        #endregion

        #region ThresholdTimeMapping (DependencyProperty)

        /// <summary>
        /// Gets / Sets the AllDay mapping property on the bound object.
        /// </summary>
        public string ThresholdTimeMapping
        {
            get { return (string)GetValue(ThresholdTimeMappingProperty); }
            set { SetValue(ThresholdTimeMappingProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for ThresholdTimeMapping.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ThresholdTimeMappingProperty =
            DependencyProperty.Register("ThresholdTimeMapping", typeof(string), typeof(ScheduleAppointmentMapping),
              new PropertyMetadata(string.Empty));

        #endregion

    }

#if SyncfusionFramework4_0 && !SILVERLIGHT
    /// <summary>
    ///  class that hold schedule appointment proxy
    /// </summary>
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ScheduleAppointmentProxy : INotifyPropertyChanged
    {
        internal ScheduleAppointmentProxy()
        {
        }

        /// <summary>
        /// this method to Initialize appointments
        /// </summary>
        /// <param name="appointment"></param>
        /// <remarks></remarks>
        public void InitializeFrom(ScheduleAppointment appointment)
        {
            this.AllDay = appointment.AllDay;
            this.ID = appointment.ID;
            this.AllowResize = appointment.AllowResize;
            this.AllowRecurrence = appointment.AllowRecurrence;
            this.DueTime = appointment.DueTime;
            this.DueTimeString = appointment.DueTimeString;
            ResourceWrapper rw = new ResourceWrapper();
            this.RecurrenceAlertMessage = "\"" + appointment.Subject + "\"" + " " + rw.RecurrenceAlertWindowContent;
            this.EndTime = appointment.EndTime;
            this.IsDismissed = appointment.IsDismissed;
            this.Location = appointment.Location;
            this.Notes = appointment.Notes;
            this.Record = appointment.Record;
            this.IsRecurrenceAppointment = appointment.IsRecurrenceAppointment;
            this.ReminderTime = appointment.ReminderTime;
            this.SnoozeTime = appointment.SnoozeTime;
            this.StartTime = appointment.StartTime;
            this.Subject = appointment.Subject;
            this.ThresholdTime = appointment.ThresholdTime;
            this.timeInterval = appointment.timeInterval;
            this.updating = appointment.updating;
            this.StartRecurrenceTime = (appointment.StartRecurrenceTime.Day == 1 && appointment.StartRecurrenceTime.Month == 1 && appointment.StartRecurrenceTime.Year == 1) ? appointment.StartTime : appointment.StartRecurrenceTime;
            this.EndRecurrenceTime = (appointment.EndRecurrenceTime.Day == 1 && appointment.EndRecurrenceTime.Month == 1 && appointment.EndRecurrenceTime.Year == 1) ? appointment.EndTime : appointment.EndRecurrenceTime;
            this.Status = appointment.Status;
            this.IsPrivate = appointment.IsPrivate;            
            this.IsLowImportance = appointment.IsLowImportance;
            this.IsHighImportance = appointment.IsHighImportance;

            this.YearlyYear = appointment.YearlyYear;
            this.YearlyDays = appointment.YearlyDays;
            this.DailyDays = appointment.DailyDays;
            this.WeeklyWeeks = appointment.WeeklyWeeks;
            this.MonthlyDays = appointment.MonthlyDays;
            this.MonthlyMonth = appointment.MonthlyMonth;
            this.MonthlyMonthMulti = appointment.MonthlyMonthMulti;

            this.AppointmentProxy = appointment.AppointmentProxy;
            this.IsAppointmentProxyCollection = appointment.IsAppointmentProxyCollection;

            this.IsDailySelected = appointment.IsDailySelected;
            this.IsMonthlySelected = appointment.IsMonthlySelected;
            this.IsWeeklySelected = appointment.IsWeeklySelected;
            this.IsYearlySelected = appointment.IsYearlySelected;

            this.IsDailyCustomDays = appointment.IsDailyCustomDays;
            this.IsDailyWeekDays = appointment.IsDailyWeekDays;
            this.IsMonthlyCustomDays = appointment.IsMonthlyCustomDays;
            this.IsMonthlyMultiDays = appointment.IsMonthlyMultiDays;
            this.IsYearlyCustomDays = appointment.IsYearlyCustomDays;
            this.IsYearlyMultiDays = appointment.IsYearlyMultiDays;

            this.MonthlyWeekOrderSelected = appointment.MonthlyWeekOrderSelected;
            this.MonthlyDaySelected = appointment.MonthlyDaySelected;
            this.YearlyMonthSelected = appointment.YearlyMonthSelected;
            this.YearlyMultiWeekOrderSelected = appointment.YearlyMultiWeekOrderSelected;
            this.YearlyMultiDaySelected = appointment.YearlyMultiDaySelected;
            this.YearlyMultiMonthSelected = appointment.YearlyMultiMonthSelected;

            this.IsNoEndDateRecurrence = appointment.IsNoEndDateRecurrence;
            this.IsEndAfter = appointment.IsEndAfter;
            this.IsEndBy = appointment.IsEndBy;
            this.EndOccurenceCount = appointment.EndOccurenceCount;

            this.IsWeeklySundaySelected = appointment.IsWeeklySundaySelected;
            this.IsWeeklyMondaySelected = appointment.IsWeeklyMondaySelected;
            this.IsWeeklyTuesdaySelected = appointment.IsWeeklyTuesdaySelected;
            this.IsWeeklyWednesdaySelected = appointment.IsWeeklyWednesdaySelected;
            this.IsWeeklyThursdaySelected = appointment.IsWeeklyThursdaySelected;
            this.IsWeeklyFridaySelected = appointment.IsWeeklyFridaySelected;
            this.IsWeeklySaturdaySelected = appointment.IsWeeklySaturdaySelected;

            this.CurrentRecurrencePatternMode = appointment.CurrentRecurrencePatternMode;

            this.SelectWeeklyCurrentDay(appointment);
        }

        private void SelectWeeklyCurrentDay(ScheduleAppointment appointment)
        {
            this.IsWeeklySundaySelected = appointment.IsWeeklySundaySelected;
            this.IsWeeklyMondaySelected = appointment.IsWeeklyMondaySelected;
            this.IsWeeklyTuesdaySelected = appointment.IsWeeklyTuesdaySelected;
            this.IsWeeklyWednesdaySelected = appointment.IsWeeklyWednesdaySelected;
            this.IsWeeklyThursdaySelected = appointment.IsWeeklyThursdaySelected;
            this.IsWeeklyFridaySelected = appointment.IsWeeklyFridaySelected;
            this.IsWeeklySaturdaySelected = appointment.IsWeeklySaturdaySelected;

            DayOfWeek dywek = CultureInfo.CurrentCulture.Calendar.GetDayOfWeek(appointment.StartTime);

            switch (dywek)
            {
                case DayOfWeek.Sunday:
                    this.IsWeeklySundaySelected = true;
                    break;
                case DayOfWeek.Monday:
                    this.IsWeeklyMondaySelected = true;
                    break;
                case DayOfWeek.Tuesday:
                    this.IsWeeklyTuesdaySelected = true;
                    break;
                case DayOfWeek.Wednesday:
                    this.IsWeeklyWednesdaySelected = true;
                    break;
                case DayOfWeek.Thursday:
                    this.IsWeeklyThursdaySelected = true;
                    break;
                case DayOfWeek.Friday:
                    this.IsWeeklyFridaySelected = true;
                    break;
                case DayOfWeek.Saturday:
                    this.IsWeeklySaturdaySelected = true;
                    break;
            }
        }

        #region Properties

        #region IsAppointmentProxyCollection
        private bool isAppointmentProxyCollection = false;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is appointment proxy collection.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is appointment proxy collection; otherwise, <c>false</c>.
        /// </value>
        public bool IsAppointmentProxyCollection
        {
            get
            {
                return this.isAppointmentProxyCollection;
            }

            set
            {
                if (this.isAppointmentProxyCollection != value)
                {
                    this.isAppointmentProxyCollection = value;
                    this.RaisePropertyChanged("IsAppointmentProxyCollection");
                }
            }
        }
        #endregion

        #region AppointmentProxy
        private ScheduleAppointmentCollection appointmentProxy = null;
        /// <summary>
        /// Gets or sets the appointment proxy.
        /// </summary>
        /// <value>The appointment proxy.</value>
        public ScheduleAppointmentCollection AppointmentProxy
        {
            get
            {
                return this.appointmentProxy;
            }

            set
            {
                if (this.appointmentProxy != value)
                {
                    this.appointmentProxy = value;
                    this.RaisePropertyChanged("AppointmentProxy");
                }
            }
        }
        #endregion

        private string subject = string.Empty;
        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>The subject.</value>
        public string Subject
        {
            get
            {
                return this.subject;
            }

            set
            {
                if (this.subject != value)
                {
                    this.subject = value;
                    this.RaisePropertyChanged("Subject");
                }
            }
        }

        private ScheduleAppointmentStatus status;
        /// <summary>
        /// Gets or sets appointment status
        /// </summary>
        public ScheduleAppointmentStatus Status
        {
            get
            {
                return this.status;
            }

            set
            {
                if (this.status != value)
                {
                    this.status = value;
                    this.RaisePropertyChanged("Status");
                }
            }
        }

        private string location = string.Empty;
        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>The location.</value>
        public string Location
        {
            get
            {
                return this.location;
            }

            set
            {
                if (this.location != value)
                {
                    this.location = value;
                    this.RaisePropertyChanged("Location");
                }
            }
        }
        
        #region DueTimeString
        private string dueTimeString;
        /// <summary>
        /// Gets or sets the due time string.
        /// </summary>
        /// <value>The due time string.</value>
        public string DueTimeString
        {
            get
            {
                return dueTimeString;
            }
            set
            {
                if (this.dueTimeString != value)
                {
                    this.dueTimeString = value;
                    this.RaisePropertyChanged("DueTimeString");
                }
            }
        }
        #endregion

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ScheduleAppointmentProxy"/> is updating.
        /// </summary>
        /// <value><c>true</c> if updating; otherwise, <c>false</c>.</value>
        public bool updating
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the time interval.
        /// </summary>
        /// <value>The time interval.</value>
        internal double timeInterval
        {
            get;
            set;
        }
         /// <summary>
        /// Gets or sets a value whether the instance is an MultiDayAppointment
        /// </summary>
        internal bool MultiDayAppointment { get; set; }
        /// <summary>
        /// Gets or sets the actual StartTime of Multiday appointment instance
        /// </summary>
        internal DateTime MultiDayAppointmentStartTime { get; set; }
        /// <summary>
        /// Gets or sets  actual EndTime of an Multiday appointment instance
        /// </summary>
        internal DateTime MultiDayAppointmentEndTime { get; set; }

        private TimeSpan thresholdTime = new TimeSpan(0, 15, 0);
        /// <summary>
        /// Gets or sets the threshold time.
        /// </summary>
        /// <value>The threshold time.</value>
        public TimeSpan ThresholdTime
        {
            get
            {
                return this.thresholdTime;
            }

            set
            {
                if (this.thresholdTime != value)
                {
                    this.thresholdTime = value;
                    this.RaisePropertyChanged("ThresholdTime");
                }
            }
        }

        private TimeSpan snoozeTime;
        /// <summary>
        /// Gets or sets the snooze time.
        /// </summary>
        /// <value>The snooze time.</value>
        public TimeSpan SnoozeTime
        {
            get
            {
                return this.snoozeTime;
            }

            set
            {
                if (this.snoozeTime != value)
                {
                    this.snoozeTime = value;
                    this.RaisePropertyChanged("SnoozeTime");
                }
            }
        }

        private TimeSpan dueTime;
        /// <summary>
        /// Gets or sets the due time.
        /// </summary>
        /// <value>The due time.</value>
        public TimeSpan DueTime
        {
            get
            {
                return this.dueTime;
            }

            set
            {
                if (this.dueTime != value)
                {
                    this.dueTime = value;
                    this.RaisePropertyChanged("DueTime");
                }
            }
        }

        private DateTime reminderTime;
        /// <summary>
        /// Gets or sets the reminder time.
        /// </summary>
        /// <value>The reminder time.</value>
        public DateTime ReminderTime
        {
            get
            {
                return this.reminderTime;
            }

            set
            {
                if (this.reminderTime != value)
                {
                    this.reminderTime = value;
                    this.RaisePropertyChanged("ReminderTime");
                }
            }
        }

        private DateTime startTime = DateTime.Now.Date;
        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        /// <value>The start time.</value>
        public DateTime StartTime
        {
            get
            {
                return this.startTime;
            }

            set
            {
                if (this.startTime != value)
                {
                    this.startTime = value;
                    this.RaisePropertyChanged("StartTime");
                }
            }
        }

        private DateTime endTime = DateTime.Now.Date;
        /// <summary>
        /// Gets or sets the end time.
        /// </summary>
        /// <value>The end time.</value>
        public DateTime EndTime
        {
            get
            {
                return this.endTime;
            }

            set
            {
                if (this.endTime != value)
                {
                    this.endTime = value;
                    this.RaisePropertyChanged("EndTime");
                }
            }
        }

        private string notes = string.Empty;
        /// <summary>
        /// Gets or sets the notes.
        /// </summary>
        /// <value>The notes.</value>
        public string Notes
        {
            get
            {
                return this.notes;
            }

            set
            {
                if (this.notes != value)
                {
                    this.notes = value;
                    this.RaisePropertyChanged("Notes");
                }
            }
        }

        #region Record

        /// <summary>
        /// Gets the bound object
        /// </summary>
        public RecordEntry Record
        {
            get;
            internal set;
        }

        #endregion

        #region AllowRecurrence
        private bool allowRecurrence = true;
        /// <summary>
        /// Gets or sets a value indicating whether [allow recurrence].
        /// </summary>
        /// <value><c>true</c> if [allow recurrence]; otherwise, <c>false</c>.</value>
        public bool AllowRecurrence
        {
            get
            {
                return allowRecurrence;
            }
            set
            {
                if (value != allowRecurrence)
                {
                    allowRecurrence = value;
                    this.RaisePropertyChanged("AllowRecurrence");
                }
            }
        }
        #endregion

        private bool allowResize = false;
        /// <summary>
        /// Gets or sets a value indicating whether [allow resize].
        /// </summary>
        /// <value><c>true</c> if [allow resize]; otherwise, <c>false</c>.</value>
        public bool AllowResize
        {
            get
            {
                return this.allowResize;
            }

            set
            {
                if (this.allowResize != value)
                {
                    this.allowResize = value;
                    this.RaisePropertyChanged("allowResize");
                }
            }
        }

        private bool isDismissed = false;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is dismissed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is dismissed; otherwise, <c>false</c>.
        /// </value>
        public bool IsDismissed
        {
            get
            {
                return this.isDismissed;
            }

            set
            {
                if (this.isDismissed != value)
                {
                    this.isDismissed = value;
                    this.RaisePropertyChanged("IsDismissed");
                }
            }
        }

        private bool isRecurrenceAppointment = false;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is recurrence appointment.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is recurrence appointment; otherwise, <c>false</c>.
        /// </value>
        public bool IsRecurrenceAppointment
        {
            get
            {
                return this.isRecurrenceAppointment;
            }

            set
            {
                if (this.isRecurrenceAppointment != value)
                {
                    this.isRecurrenceAppointment = value;
                    this.RaisePropertyChanged("IsRecurrenceAppointment");
                }
            }
        }


        private Int64 iD = 0;
        /// <summary>
        /// Gets or sets a value indicating whether [all day].
        /// </summary>
        /// <value><c>true</c> if [all day]; otherwise, <c>false</c>.</value>
        public Int64 ID
        {
            get
            {
                return this.iD;
            }

            set
            {
                if (this.iD != value)
                {
                    this.iD = value;
                    this.RaisePropertyChanged("ID");
                }
            }
        }

        private bool allDay = false;
        /// <summary>
        /// Gets or sets a value indicating whether [all day].
        /// </summary>
        /// <value><c>true</c> if [all day]; otherwise, <c>false</c>.</value>
        public bool AllDay
        {
            get
            {
                return this.allDay;
            }

            set
            {
                if (this.allDay != value)
                {
                    this.allDay = value;
                    this.RaisePropertyChanged("IsAllDay");
                }
            }
        }

        //private AppointmentPriority priority ;
        ///// <summary>
        ///// Gets or sets a value indicating whether [all day].
        ///// </summary>
        ///// <value><c>true</c> if [all day]; otherwise, <c>false</c>.</value>
        //public AppointmentPriority Priority
        //{
        //    get
        //    {
        //        return this.priority;
        //    }

        //    set
        //    {
        //        if (this.priority != value)
        //        {
        //            this.priority = value;
        //            this.RaisePropertyChanged("Priority");
        //        }
        //    }
        //}


        private TimeSpan startTimeSpan = TimeSpan.MinValue;
        /// <summary>
        /// Gets or sets the start time span.
        /// </summary>
        /// <value>The start time span.</value>
        public TimeSpan StartTimeSpan
        {
            get
            {
                return this.startTimeSpan;
            }

            set
            {
                if (this.startTimeSpan != value)
                {
                    this.startTimeSpan = value;
                    this.RaisePropertyChanged("StartTimeSpan");
                }
            }
        }

        private TimeSpan endTimeSpan = TimeSpan.MinValue;
        /// <summary>
        /// Gets or sets the end time span.
        /// </summary>
        /// <value>The end time span.</value>
        public TimeSpan EndTimeSpan
        {
            get
            {
                return this.endTimeSpan;
            }

            set
            {
                if (this.endTimeSpan != value)
                {
                    this.endTimeSpan = value;
                    this.RaisePropertyChanged("EndTimeSpan");
                }
            }
        }
        #endregion

        #region Recurrence Properties

        #region RecurrenceAlertMessage
        private string recurrenceAlertMessage = " is a Recurring Appointment. Do you want to open only this occurrence or the series?";
        /// <summary>
        /// Gets or sets the recurrence alert message.
        /// </summary>
        /// <value>The recurrence alert message.</value>
        public string RecurrenceAlertMessage
        {
            get
            {
                return recurrenceAlertMessage;
            }
            set
            {
                if (this.recurrenceAlertMessage != value)
                {
                    this.recurrenceAlertMessage = value;
                    this.RaisePropertyChanged("RecurrenceAlertMessage");
                }
            }
        }
        #endregion

        #region CurrentRecurrencePatternMode
        private RecurrencePatternMode currentRecurrencePatternMode = RecurrencePatternMode.Daily;
        /// <summary>
        /// Gets or sets the current recurrence pattern mode.
        /// </summary>
        /// <value>The current recurrence pattern mode.</value>
        public RecurrencePatternMode CurrentRecurrencePatternMode
        {
            get
            {
                return this.currentRecurrencePatternMode;
            }

            set
            {
                if (this.currentRecurrencePatternMode != value)
                {
                    this.currentRecurrencePatternMode = value;
                    this.RaisePropertyChanged("CurrentRecurrencePatternMode");
                }
            }
        }

        #endregion

        #region MonthlyWeekOrderSelected
        private String monthlyWeekOrderSelected = "";
        /// <summary>
        /// Gets or sets the monthly week order selected.
        /// </summary>
        /// <value>The monthly week order selected.</value>
        public String MonthlyWeekOrderSelected
        {
            get
            {
                return this.monthlyWeekOrderSelected;
            }

            set
            {
                if (this.monthlyWeekOrderSelected != value)
                {
                    this.monthlyWeekOrderSelected = value;
                    this.RaisePropertyChanged("MonthlyWeekOrderSelected");
                }
            }
        }

        #endregion

        #region MonthlyMonthSelected
        private String monthlyDaySelected = "";
        /// <summary>
        /// Gets or sets the monthly day selected.
        /// </summary>
        /// <value>The monthly day selected.</value>
        public String MonthlyDaySelected
        {
            get
            {
                return this.monthlyDaySelected;
            }

            set
            {
                if (this.monthlyDaySelected != value)
                {
                    this.monthlyDaySelected = value;
                    this.RaisePropertyChanged("MonthlyDaySelected");
                }
            }
        }

        #endregion

        #region YearlyMonthSelected
        private String yearlyMonthSelected = "";
        /// <summary>
        /// Gets or sets the yearly month selected.
        /// </summary>
        /// <value>The yearly month selected.</value>
        public String YearlyMonthSelected
        {
            get
            {
                return this.yearlyMonthSelected;
            }

            set
            {
                if (this.yearlyMonthSelected != value)
                {
                    this.yearlyMonthSelected = value;
                    this.RaisePropertyChanged("YearlyMonthSelected");
                }
            }
        }

        #endregion

        #region YearlyMultiWeekOrderSelected
        private String yearlyMultiWeekOrderSelected = "";
        /// <summary>
        /// Gets or sets the yearly multi week order selected.
        /// </summary>
        /// <value>The yearly multi week order selected.</value>
        public String YearlyMultiWeekOrderSelected
        {
            get
            {
                return this.yearlyMultiWeekOrderSelected;
            }

            set
            {
                if (this.yearlyMultiWeekOrderSelected != value)
                {
                    this.yearlyMultiWeekOrderSelected = value;
                    this.RaisePropertyChanged("YearlyMultiWeekOrderSelected");
                }
            }
        }

        #endregion

        #region YearlyMultiDaySelected
        private String yearlyMultiDaySelected = "";
        /// <summary>
        /// Gets or sets the yearly multi day selected.
        /// </summary>
        /// <value>The yearly multi day selected.</value>
        public String YearlyMultiDaySelected
        {
            get
            {
                return this.yearlyMultiDaySelected;
            }

            set
            {
                if (this.yearlyMultiDaySelected != value)
                {
                    this.yearlyMultiDaySelected = value;
                    this.RaisePropertyChanged("YearlyMultiDaySelected");
                }
            }
        }

        #endregion

        #region YearlyMultiMonthSelected
        private String yearlyMultiMonthSelected = "";
        /// <summary>
        /// Gets or sets the yearly multi month selected.
        /// </summary>
        /// <value>The yearly multi month selected.</value>
        public String YearlyMultiMonthSelected
        {
            get
            {
                return this.yearlyMultiMonthSelected;
            }

            set
            {
                if (this.yearlyMultiMonthSelected != value)
                {
                    this.yearlyMultiMonthSelected = value;
                    this.RaisePropertyChanged("YearlyMultiMonthSelected");
                }
            }
        }
        #endregion

        #region EndOccurenceCount
        private int endOccurenceCount = 10;
        /// <summary>
        /// Gets or sets the end occurence count.
        /// </summary>
        /// <value>The end occurence count.</value>
        public int EndOccurenceCount
        {
            get
            {
                return this.endOccurenceCount;
            }

            set
            {
                if (this.endOccurenceCount != value)
                {
                    this.endOccurenceCount = value;
                    this.RaisePropertyChanged("EndOccurenceCount");
                }
            }
        }
        #endregion

        #region DailyDays
        private int dailyDays = 1;
        /// <summary>
        /// Gets or sets the daily days.
        /// </summary>
        /// <value>The daily days.</value>
        public int DailyDays
        {
            get
            {
                return this.dailyDays;
            }

            set
            {
                if (this.dailyDays != value)
                {
                    this.dailyDays = value;
                    this.RaisePropertyChanged("DailyDays");
                }
            }
        }
        #endregion

        #region WeeklyWeeks
        private int weeklyWeeks = 1;
        /// <summary>
        /// Gets or sets the weekly weeks.
        /// </summary>
        /// <value>The weekly weeks.</value>
        public int WeeklyWeeks
        {
            get
            {
                return this.weeklyWeeks;
            }

            set
            {
                if (this.weeklyWeeks != value)
                {
                    this.weeklyWeeks = value;
                    this.RaisePropertyChanged("WeeklyWeeks");
                }
            }
        }
        #endregion

        #region MonthlyDays
        private int monthlyDays = 1;
        /// <summary>
        /// Gets or sets the monthly days.
        /// </summary>
        /// <value>The monthly days.</value>
        public int MonthlyDays
        {
            get
            {
                return this.monthlyDays;
            }

            set
            {
                if (this.monthlyDays != value)
                {
                    this.monthlyDays = value;
                    this.RaisePropertyChanged("MonthlyDays");
                }
            }
        }
        #endregion

        #region MonthlyMonth
        private int monthlyMonth = 1;
        /// <summary>
        /// Gets or sets the monthly month.
        /// </summary>
        /// <value>The monthly month.</value>
        public int MonthlyMonth
        {
            get
            {
                return this.monthlyMonth;
            }

            set
            {
                if (this.monthlyMonth != value)
                {
                    this.monthlyMonth = value;
                    this.RaisePropertyChanged("MonthlyMonth");
                }
            }
        }
        #endregion

        #region MonthlyMonthMulti
        private int monthlyMonthMulti = 1;
        /// <summary>
        /// Gets or sets the monthly month multi.
        /// </summary>
        /// <value>The monthly month multi.</value>
        public int MonthlyMonthMulti
        {
            get
            {
                return this.monthlyMonthMulti;
            }

            set
            {
                if (this.monthlyMonthMulti != value)
                {
                    this.monthlyMonthMulti = value;
                    this.RaisePropertyChanged("MonthlyMonthMulti");
                }
            }
        }
        #endregion

        #region YearlyYear
        private int yearlyYear = 1;
        /// <summary>
        /// Gets or sets the yearly year.
        /// </summary>
        /// <value>The yearly year.</value>
        public int YearlyYear
        {
            get
            {
                return this.yearlyYear;
            }

            set
            {
                if (this.yearlyYear != value)
                {
                    this.yearlyYear = value;
                    this.RaisePropertyChanged("YearlyYear");
                }
            }
        }
        #endregion

        #region YearlyDays
        private int yearlyDays = 1;
        /// <summary>
        /// Gets or sets the yearly days.
        /// </summary>
        /// <value>The yearly days.</value>
        public int YearlyDays
        {
            get
            {
                return this.yearlyDays;
            }

            set
            {
                if (this.yearlyDays != value)
                {
                    this.yearlyDays = value;
                    this.RaisePropertyChanged("YearlyDays");
                }
            }
        }
        #endregion

        #region IsNoEndDate
        private bool isNoEndDate = false;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is no end date recurrence.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is no end date recurrence; otherwise, <c>false</c>.
        /// </value>
        public bool IsNoEndDateRecurrence
        {
            get
            {
                return this.isNoEndDate;
            }

            set
            {
                if (this.isNoEndDate != value)
                {
                    this.isNoEndDate = value;
                    this.RaisePropertyChanged("IsNoEndDateRecurrence");
                }
            }
        }
        #endregion

        #region IsEndAfter
        private bool isEndAfter = false;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is end after.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is end after; otherwise, <c>false</c>.
        /// </value>
        public bool IsEndAfter
        {
            get
            {
                return this.isEndAfter;
            }

            set
            {
                if (this.isEndAfter != value)
                {
                    this.isEndAfter = value;
                    this.RaisePropertyChanged("IsEndAfter");
                }
            }
        }

        #endregion

        #region IsEndBy
        private bool isEndBy = false;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is end by.
        /// </summary>
        /// <value><c>true</c> if this instance is end by; otherwise, <c>false</c>.</value>
        public bool IsEndBy
        {
            get
            {
                return this.isEndBy;
            }

            set
            {
                if (this.isEndBy != value)
                {
                    this.isEndBy = value;
                    this.RaisePropertyChanged("IsEndBy");
                }
            }
        }

        #endregion

        #region IsWeeklySundaySelected
        private bool isWeeklySundaySelected = false;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is weekly sunday selected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is weekly sunday selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsWeeklySundaySelected
        {
            get
            {
                return this.isWeeklySundaySelected;
            }

            set
            {
                if (this.isWeeklySundaySelected != value)
                {
                    this.isWeeklySundaySelected = value;
                    this.RaisePropertyChanged("IsWeeklySundaySelected");
                }
            }
        }

        #endregion

        #region IsWeeklyMondaySelected
        private bool isWeeklyMondaySelected = false;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is weekly monday selected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is weekly monday selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsWeeklyMondaySelected
        {
            get
            {
                return this.isWeeklyMondaySelected;
            }

            set
            {
                if (this.isWeeklyMondaySelected != value)
                {
                    this.isWeeklyMondaySelected = value;
                    this.RaisePropertyChanged("IsWeeklyMondaySelected");
                }
            }
        }

        #endregion

        #region IsWeeklyTuesdaySelected
        private bool isWeeklyTuesdaySelected = false;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is weekly tuesday selected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is weekly tuesday selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsWeeklyTuesdaySelected
        {
            get
            {
                return this.isWeeklyTuesdaySelected;
            }

            set
            {
                if (this.isWeeklyTuesdaySelected != value)
                {
                    this.isWeeklyTuesdaySelected = value;
                    this.RaisePropertyChanged("IsWeeklyTuesdaySelected");
                }
            }
        }

        #endregion

        #region IsWeeklyWednesdaySelected
        private bool isWeeklyWednesdaySelected = false;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is weekly wednesday selected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is weekly wednesday selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsWeeklyWednesdaySelected
        {
            get
            {
                return this.isWeeklyWednesdaySelected;
            }

            set
            {
                if (this.isWeeklyWednesdaySelected != value)
                {
                    this.isWeeklyWednesdaySelected = value;
                    this.RaisePropertyChanged("IsWeeklyWednesdaySelected");
                }
            }
        }

        #endregion

        #region IsWeeklyThursdaySelected
        private bool isWeeklyThursdaySelected = false;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is weekly thursday selected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is weekly thursday selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsWeeklyThursdaySelected
        {
            get
            {
                return this.isWeeklyThursdaySelected;
            }

            set
            {
                if (this.isWeeklyThursdaySelected != value)
                {
                    this.isWeeklyThursdaySelected = value;
                    this.RaisePropertyChanged("IsWeeklyThursdaySelected");
                }
            }
        }

        #endregion

        #region IsWeeklyFridaySelected
        private bool isWeeklyFridaySelected = false;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is weekly friday selected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is weekly friday selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsWeeklyFridaySelected
        {
            get
            {
                return this.isWeeklyFridaySelected;
            }

            set
            {
                if (this.isWeeklyFridaySelected != value)
                {
                    this.isWeeklyFridaySelected = value;
                    this.RaisePropertyChanged("IsWeeklyFridaySelected");
                }
            }
        }

        #endregion

        #region IsWeeklySaturdaySelected
        private bool isWeeklySaturdaySelected = false;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is weekly saturday selected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is weekly saturday selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsWeeklySaturdaySelected
        {
            get
            {
                return this.isWeeklySaturdaySelected;
            }

            set
            {
                if (this.isWeeklySaturdaySelected != value)
                {
                    this.isWeeklySaturdaySelected = value;
                    this.RaisePropertyChanged("IsWeeklySaturdaySelected");
                }
            }
        }

        #endregion

        #region IsDailyCustomDays
        private bool isDailyCustomDays = true;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is daily custom days.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is daily custom days; otherwise, <c>false</c>.
        /// </value>
        public bool IsDailyCustomDays
        {
            get
            {
                return this.isDailyCustomDays;
            }

            set
            {
                if (this.isDailyCustomDays != value)
                {
                    this.isDailyCustomDays = value;
                    this.RaisePropertyChanged("IsDailyCustomDays");
                }
            }
        }
        #endregion

        #region IsDailyWeekDays
        private bool isDailyWeekDays = false;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is daily week days.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is daily week days; otherwise, <c>false</c>.
        /// </value>
        public bool IsDailyWeekDays
        {
            get
            {
                return this.isDailyWeekDays;
            }

            set
            {
                if (this.isDailyWeekDays != value)
                {
                    this.isDailyWeekDays = value;
                    this.RaisePropertyChanged("IsDailyWeekDays");
                }
            }
        }
        #endregion

        #region IsMonthlyCustomDays
        private bool isMonthlyCustomDays = true;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is monthly custom days.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is monthly custom days; otherwise, <c>false</c>.
        /// </value>
        public bool IsMonthlyCustomDays
        {
            get
            {
                return this.isMonthlyCustomDays;
            }

            set
            {
                if (this.isMonthlyCustomDays != value)
                {
                    this.isMonthlyCustomDays = value;
                    this.RaisePropertyChanged("IsMonthlyCustomDays");
                }
            }
        }
        #endregion

        #region IsMonthlyMultiDays
        private bool isMonthlyMultiDays = false;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is monthly multi days.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is monthly multi days; otherwise, <c>false</c>.
        /// </value>
        public bool IsMonthlyMultiDays
        {
            get
            {
                return this.isMonthlyMultiDays;
            }

            set
            {
                if (this.isMonthlyMultiDays != value)
                {
                    this.isMonthlyMultiDays = value;
                    this.RaisePropertyChanged("IsMonthlyMultiDays");
                }
            }
        }
        #endregion

        #region IsYearlyCustomDays
        private bool isYearlyCustomDays = true;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is yearly custom days.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is yearly custom days; otherwise, <c>false</c>.
        /// </value>
        public bool IsYearlyCustomDays
        {
            get
            {
                return this.isYearlyCustomDays;
            }

            set
            {
                if (this.isYearlyCustomDays != value)
                {
                    this.isYearlyCustomDays = value;
                    this.RaisePropertyChanged("IsYearlyCustomDays");
                }
            }
        }
        #endregion

        #region IsYearlyMultiDays
        private bool isYearlyMultiDays = false;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is yearly multi days.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is yearly multi days; otherwise, <c>false</c>.
        /// </value>
        public bool IsYearlyMultiDays
        {
            get
            {
                return this.isYearlyMultiDays;
            }

            set
            {
                if (this.isYearlyMultiDays != value)
                {
                    this.isYearlyMultiDays = value;
                    this.RaisePropertyChanged("IsYearlyMultiDays");
                }
            }
        }
        #endregion

        #region IsDailySelected
        private bool isDailySelected = true;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is daily selected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is daily selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsDailySelected
        {
            get
            {
                return this.isDailySelected;
            }

            set
            {
                if (this.isDailySelected != value)
                {
                    this.isDailySelected = value;
                    this.RaisePropertyChanged("IsDailySelected");
                }
            }
        }
        #endregion

        #region IsPrivate
        private bool isPrivate = false;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is Private or not.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is Private; otherwise, <c>false</c>.
        /// </value>
        public bool IsPrivate
        {
            get
            {
                return this.isPrivate;
            }

            set
            {
                if (this.isPrivate != value)
                {
                    this.isPrivate = value;
                    //this.RaisePropertyChanged("IsDailySelected");
                }
            }
        }
        #endregion

        //#region AppointmentImportance
        //private AppointmentImportance appointmentImportance = AppointmentImportance.None;
        ///// <summary>
        ///// Gets or sets a value indicating whether this Appointment is Low, High or None Importance.
        ///// </summary>
        ///// <value>
        ///// 	<c>Low</c> if this Appointment is Low Importance; otherwise, <c>High</c>.
        ///// </value>
        //public AppointmentImportance AppointmentImportance
        //{
        //    get 
        //    {
        //        return this.appointmentImportance;
        //    }
        //    set
        //    {
        //        if (this.appointmentImportance != value)
        //        {
        //            this.appointmentImportance = value;
        //        }
        //    }
        //}    
        //#endregion

        #region IsLowImportance
        private bool isLowImportance = false;
        /// <summary>
        /// Gets or sets a value indicating whether this Appointment is Low, High or None Importance.
        /// </summary>
        /// <value>
        /// 	<c>Low</c> if this Appointment is Low Importance; otherwise, <c>High</c>.
        /// </value>
        public bool IsLowImportance
        {
            get
            {
                return this.isLowImportance;
            }
            set
            {
                if (this.isLowImportance != value)
                {
                    this.isLowImportance = value;
                }
            }
        }
        #endregion

        #region IsHighImportance
        private bool isHighImportance = false;
        /// <summary>
        /// Gets or sets a value indicating whether this Appointment is Low, High or None Importance.
        /// </summary>
        /// <value>
        /// 	<c>Low</c> if this Appointment is Low Importance; otherwise, <c>High</c>.
        /// </value>
        public bool IsHighImportance
        {
            get
            {
                return this.isHighImportance;
            }
            set
            {
                if (this.isHighImportance != value)
                {
                    this.isHighImportance = value;
                }
            }
        }
        #endregion

        #region IsWeeklySelected
        private bool isWeeklySelected = false;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is weekly selected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is weekly selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsWeeklySelected
        {
            get
            {
                return this.isWeeklySelected;
            }

            set
            {
                if (this.isWeeklySelected != value)
                {
                    this.isWeeklySelected = value;
                    this.RaisePropertyChanged("IsWeeklySelected");
                }
            }
        }
        #endregion

        #region IsMonthlySelected
        private bool isMonthlySelected = false;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is monthly selected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is monthly selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsMonthlySelected
        {
            get
            {
                return this.isMonthlySelected;
            }

            set
            {
                if (this.isMonthlySelected != value)
                {
                    this.isMonthlySelected = value;
                    this.RaisePropertyChanged("IsMonthlySelected");
                }
            }
        }
        #endregion

        #region IsYearlySelected
        private bool isYearlySelected = false;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is yearly selected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is yearly selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsYearlySelected
        {
            get
            {
                return this.isYearlySelected;
            }

            set
            {
                if (this.isYearlySelected != value)
                {
                    this.isYearlySelected = value;
                    this.RaisePropertyChanged("IsYearlySelected");
                }
            }
        }
        #endregion

        #region StartRecurrenceTime
        private DateTime startRecurrenceTime = DateTime.Now.Date;
        /// <summary>
        /// Gets or sets the start recurrence time.
        /// </summary>
        /// <value>The start recurrence time.</value>
        public DateTime StartRecurrenceTime
        {
            get
            {
                return this.startRecurrenceTime;
            }

            set
            {
                if (this.startRecurrenceTime != value)
                {
                    this.startRecurrenceTime = value;
                    this.RaisePropertyChanged("StartRecurrenceTime");
                }
            }
        }
        #endregion

        #region EndRecurrenceTime
        private DateTime endRecurrenceTime = DateTime.Now.Date;
        /// <summary>
        /// Gets or sets the end recurrence time.
        /// </summary>
        /// <value>The end recurrence time.</value>
        public DateTime EndRecurrenceTime
        {
            get
            {
                return this.endRecurrenceTime;
            }

            set
            {
                if (this.endRecurrenceTime != value)
                {
                    this.endRecurrenceTime = value;
                    this.RaisePropertyChanged("EndRecurrenceTime");
                }
            }
        }
        #endregion       

        #region INotifyPropertyChanged Members

        private void RaisePropertyChanged(string property)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }

        /// <summary>
        /// Occurs when property changed. 
        /// </summary>
        /// <remarks></remarks>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
        #endregion
    }

#if SyncfusionFramework4_0 && !SILVERLIGHT
    /// <summary>
    ///  class that hold appointment wrapper
    /// </summary>
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ScheduleAppointmentWrapper
    {
        private ScheduleCalendarViewModel model;
        internal ScheduleAppointmentWrapper(ScheduleAppointment appointment, ScheduleCalendarViewModel model, bool isEditing)
        {
            if (appointment == null)
            {
                throw new ArgumentNullException("Appointment");
            }

            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            this.model = model;
            this.AppointmentStatusCollection = model.AppointmentStatusCollection;
            var appProxy = new ScheduleAppointmentProxy();
            appProxy.InitializeFrom(appointment);
            appProxy.StartTimeSpan = !isEditing ? model.SelectedStartTimeSpan.TimeOfDay : appointment.StartTime.TimeOfDay;
            appProxy.EndTimeSpan = !isEditing ? model.SelectedEndTimeSpan.TimeOfDay : appointment.EndTime.TimeOfDay;
            this.Appointment = appProxy;
            appProxy.PropertyChanged += new PropertyChangedEventHandler(appProxy_PropertyChanged);
            this.StartTimeIntervalSource = new List<TimeSpan>();
            this.EndTimeIntervalSource = new List<TimeSpan>();
            var intervalCount = ScheduleTimeLineHourControl.IntervalCount[(int)model.CurrentTimeInterval];
            int interval = (int)(60 / intervalCount);
            for (int m = ScheduleTimeLineHourControl.MinValue; m <= ScheduleTimeLineHourControl.MaxValue; m++)
            {
                var ts = new TimeSpan(m, 0, 0);
                for (int i = 0; i < intervalCount; i++)
                {
                    ts = ts.Add(new TimeSpan(0, interval, 0));
                    this.StartTimeIntervalSource.Add(ts);
                    //if (ts >= appProxy.StartTimeSpan)
                    //{
                        this.EndTimeIntervalSource.Add(ts);
                    //}
                }
            }

            TimeSpan last = new TimeSpan(23, 59, 59);
            this.StartTimeIntervalSource[this.StartTimeIntervalSource.Count - 1] = last;
            if (EndTimeIntervalSource.Count == 0)
                this.EndTimeIntervalSource.Add(last);
            this.EndTimeIntervalSource[this.EndTimeIntervalSource.Count - 1] = last;

            this.AllowDelete = this.model.AllowDelete;
            this.BindRecureenceSource();
            appProxy.MonthlyDays = appointment.StartTime.Day;
            appProxy.YearlyMultiDaySelected = (appointment.MonthlyDaySelected == "") ? appointment.StartTime.DayOfWeek.ToString() : appointment.MonthlyDaySelected;
            appProxy.MonthlyDaySelected = (appointment.MonthlyDaySelected == "") ? appointment.StartTime.DayOfWeek.ToString() : appointment.MonthlyDaySelected;

            if (appProxy.MonthlyDaySelected == "weekday")
            {
                appProxy.MonthlyWeekOrderSelected = appointment.GetCurrrentWeekDayOrder(appointment.StartTime, "");
            }
            else if (appProxy.MonthlyDaySelected == "day")
            {
                appProxy.MonthlyWeekOrderSelected = appointment.GetCurrentOrderInIntNormalFormat(appointment.StartTime);
            }
            else
            {
                appProxy.MonthlyWeekOrderSelected = appointment.GetCurrrentDayOrder(appointment.StartTime, "");
            }
            if (appProxy.YearlyMultiDaySelected == "weekday")
            {
                appProxy.YearlyMultiWeekOrderSelected = appointment.GetCurrrentWeekDayOrder(appointment.StartTime, "");
            }
            else if (appProxy.YearlyMultiDaySelected == "day")
            {
                appProxy.YearlyMultiWeekOrderSelected = appointment.GetCurrentOrderInIntNormalFormat(appointment.StartTime);
            }
            else
            {
                appProxy.YearlyMultiWeekOrderSelected = appointment.GetCurrrentDayOrder(appointment.StartTime, "");
            }
            appProxy.YearlyMonthSelected = appointment.StartTime.ToString("MMMM");
            appProxy.YearlyMultiMonthSelected = appointment.StartTime.ToString("MMMM");

//            this.TimeZones = new List<TimeZoneInfo>();
//#if !SILVERLIGHT
//            this.GetTimeZones();
//#endif
        }

        private void BindRecureenceSource()
        {
            string[] MonthNames = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
            MonthCollection = new List<string>(MonthNames.Where(str => !string.IsNullOrEmpty(str)).ToList());

            string[] daysOrderNames = { "first", "second", "third", "fourth", "last" };
            DaysOrderCollection = new List<string>(daysOrderNames.Where(str => !string.IsNullOrEmpty(str)).ToList());

            string[] daysNames = { "day", "weekday", "weekend day", "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
            DaysCollection = new List<string>(daysNames.Where(str => !string.IsNullOrEmpty(str)).ToList());
        }

        void appProxy_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var appProxy = sender as ScheduleAppointmentProxy;
            appProxy.PropertyChanged -= new PropertyChangedEventHandler(appProxy_PropertyChanged);
            this.IsAppointmentModified = true;
        }

        /// <summary>
        /// Gets or sets proxy appointments
        /// </summary>
        public ScheduleAppointmentProxy Appointment
        {
            get;
            private set;
        }



//#if !SILVERLIGHT
//        private void GetTimeZones()
//        {           
//            ReadOnlyCollection<TimeZoneInfo> tz = TimeZoneInfo.GetSystemTimeZones();
//            foreach (TimeZoneInfo timezone in tz)
//                this.TimeZones.Add(timezone);
//        }
//#endif


//        public List<TimeZoneInfo> TimeZones
//        {
//            get;
//            private set;
//        }


        internal bool IsAppointmentModified
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the DaysOrderCollection Source
        /// </summary>
        public List<String> DaysOrderCollection
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the DaysCollection Source
        /// </summary>
        public List<String> DaysCollection
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the MonthCollection Source
        /// </summary>
        public List<String> MonthCollection
        {
            get;
            private set;
        }

        string[] priorityNames = { "Free", "Tentative", "Busy", "Out-Of-Office"};            
        /// <summary>
        /// Gets the MonthCollection Source
        /// </summary>
        public List<String> PriorityCollection
        {
            get
            {
                return (priorityNames.Where(str => !string.IsNullOrEmpty(str)).ToList());
            }
            set
            {
                priorityNames = value.ToArray(); 
            }
        }

        string[] ReminderThresholdTime = { "None", "0 minutes", "5 minutes", "10 minutes", "15 minutes", "30 minutes", "1 hour", "2 hours", "3 hours", "4 hours", "5 hours", "6 hours", "7 hours", "8 hours", "9 hours", "10 hours", "11 hours", "0.5 day", "18 hours", "1 day", "2 days", "3 days", "4 days","1 week", "2 weeks" };
        /// <summary>
        /// Gets the ReminderThresholdTime Source
        /// </summary>
        public List<String> RemiderThresholdTimeCollection
        {
            get 
            {
                return (ReminderThresholdTime.Where(str=> !string.IsNullOrEmpty(str)).ToList());
            }
            set
            {
                ReminderThresholdTime = value.ToArray(); 
            }
        }


        /// <summary>
        /// Gets the StartTimeInterval Source
        /// </summary>
        public List<TimeSpan> StartTimeIntervalSource
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets collection of EndTimeInterval Sources
        /// </summary>
        public List<TimeSpan> EndTimeIntervalSource
        {
            get;
            private set;
        }
        /// <summary>
        /// Gets or sets AllowDelete
        /// </summary>
        public bool AllowDelete
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets AppointmentStatusCollection
        /// </summary>
        public ScheduleAppointmentStatusCollection AppointmentStatusCollection
        {
            get;
            set;
        }

    }   
    

#if SyncfusionFramework4_0 && !SILVERLIGHT
    /// <summary>
    ///  class that hold confirmation for the schedule recurrence
    /// </summary>
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ScheduleRecurrenceConfirmationWindow : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Controls.Schedule.ScheduleRecurrenceConfirmationWindow"/>
        /// class.
        /// </summary>
        public ScheduleRecurrenceConfirmationWindow()
        {
            this.DefaultStyleKey = typeof(ScheduleRecurrenceConfirmationWindow);
        }

        private TextBlock PART_Message;
        private Button PART_OkButton;
        private Button PART_CancelButton;
        private RadioButton PART_Recurrence;
        private RadioButton PART_NonRecurrence;

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.PART_OkButton = this.GetTemplateChild("PART_OkButton") as Button;
            this.PART_CancelButton = this.GetTemplateChild("PART_CancelButton") as Button;
            this.PART_Recurrence = this.GetTemplateChild("PART_Recurrence") as RadioButton;
            this.PART_NonRecurrence = this.GetTemplateChild("PART_NonRecurrence") as RadioButton;
            this.PART_Message = this.GetTemplateChild("PART_Message") as TextBlock;
            this.SetupEvents();
        }

        /// <summary>
        /// Setups the events.
        /// </summary>
        private void SetupEvents()
        {
            this.PART_OkButton.Click += new RoutedEventHandler(PART_OkButton_Click);
            this.PART_CancelButton.Click += new RoutedEventHandler(PART_CancelButton_Click);
        }

        /// <summary>
        /// Occurs when [cancel button click].
        /// </summary>
        public event RoutedEventHandler CancelButtonClick;
        private void PART_CancelButton_Click(object sender, RoutedEventArgs e)
        {
            var handler = this.CancelButtonClick;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when [ok button click].
        /// </summary>
        public event RoutedEventHandler OkButtonClick;
        private void PART_OkButton_Click(object sender, RoutedEventArgs e)
        {
            var handler = this.OkButtonClick;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }

    internal class ScheduleAppointmentProxies
    {
        private ScheduleAppointment parentAppointment;
        public ScheduleAppointment ParentAppointment
        {
            get { return parentAppointment; }
            set { parentAppointment = value; }
        }

        private ScheduleAppointment appointmentProxy;
        public ScheduleAppointment AppointmentProxy
        {
            get { return appointmentProxy; }
            set { appointmentProxy = value; }
        }

        public ScheduleAppointmentProxies()
        {

        }

        public ScheduleAppointmentProxies(ScheduleAppointment app, ScheduleAppointment appProxy)
        {
            this.ParentAppointment = app;
            this.AppointmentProxy = appProxy;
        }
    }

    /// <summary>
    ///  class that holds appointment status
    /// </summary>
    public class ScheduleAppointmentStatus : DependencyObject
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Controls.Schedule.ScheduleAppointmentStatus"/> class.
        /// </summary>
        public ScheduleAppointmentStatus()
        { 
        
        }

        #region Status

        /// <summary>
        /// Gets / Sets the Notes property.
        /// </summary>
        public string Status
        {
            get { return (string)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for Status.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register("Status", typeof(string), typeof(ScheduleAppointmentStatus), new PropertyMetadata(string.Empty));

        #endregion

        #region Brush

        /// <summary>
        /// Gets / Sets the Notes property.
        /// </summary>
        public Brush Brush
        {
            get { return (Brush)GetValue(BrushProperty); }
            set { SetValue(BrushProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for Brush.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty BrushProperty = DependencyProperty.Register("Brush", typeof(Brush), typeof(ScheduleAppointmentStatus), new PropertyMetadata(null));

        #endregion
    }


    /// <summary>
    ///  class that holds collection of appointments status collection
    /// </summary>
    public class ScheduleAppointmentStatusCollection : ObservableCollection<ScheduleAppointmentStatus>
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Controls.Schedule.ScheduleAppointmentStatusCollection"/>
        /// class.
        /// </summary>
        public ScheduleAppointmentStatusCollection()
        {

        }
    }

    /// <summary>
    ///  Class that holds ScheduleHolidays
    /// </summary>
    public class ScheduleHolidays : ScheduleAppointment
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Controls.Schedule.ScheduleHolidays"/> class.
        /// </summary>
        public  ScheduleHolidays()
        {
            this.AllDay = true;
            this.CurrentAppointmentType = AppointmentType.Holiday;
        }

    }

    /// <summary>
    ///  class that holds collection of holidays
    /// </summary>
    public class ScheduleHolidaysCollection : ObservableCollection<ScheduleHolidays>
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Controls.Schedule.ScheduleHolidaysCollection"/>
        /// class.
        /// </summary>
        public  ScheduleHolidaysCollection()
        { 
        
        }
    }
}