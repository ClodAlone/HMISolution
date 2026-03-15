#region Copyright Syncfusion Inc. 2001 - 2014
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
using System.Globalization;
using Syncfusion.Windows.Controls.Schedule.Resources;

namespace Syncfusion.Windows.Controls.Schedule
{
    /// <summary>
    ///  class that holds the wrapper for the resources
    /// </summary>
    public class ResourceWrapper
    {
        //message box
        const string AppointmentDeleteMessageBoxContentValue = "AppointmentDeleteMessageBoxContent";
        const string AppointmentSaveChangesMessageBoxContentValue = "AppointmentSaveChangesMessageBoxContent";
        const string AppointmentDeleteMessageBoxHeaderValue = "AppointmentDeleteMessageBoxHeader";
        const string AppointmentSaveChangesMessageBoxHeaderValue = "AppointmentSaveChangesMessageBoxHeader";
        
        //common
        const string OkValue = "Ok";
        const string CancelValue = "Cancel";
        
        //Goto Date Window
        const string GoToDateWindowHeaderValue = "GoToDateWindowHeader";
        const string GoToDateWindowDateValue = "GoToDateWindowDate";
        const string GoToDateWindowShowInValue = "GoToDateWindowShowIn";

        //recurrence alert window
        const string RecurrenceAlertWindowHeaderValue = "RecurrenceAlertWindowHeader";
        const string RecurrenceAlertWindowContentValue = "RecurrenceAlertWindowContent";
        const string RecurrenceAlertWindowOpenValue = "RecurrenceAlertWindowOpen";
        const string RecurrenceAlertWindowOpenSeriesValue = "RecurrenceAlertWindowOpenSeries";

        //Appointment Window Basics
        const string AppWindowSaveValue = "AppWindowSave";
        const string AppWindowDeleteValue = "AppWindowDelete";
        const string AppWindowShowAsValue = "AppWindowShowAs";
        const string AppWindowReminderValue = "AppWindowReminder";
        const string AppWindowEnableRecurrenceValue = "AppWindowEnableRecurrence";
        const string AppWindowRemoveRecurrenceValue = "AppWindowRemoveRecurrence";
        const string AppWindowPrivateValue = "AppWindowPrivate";
        const string AppWindowHighImportanceValue = "AppWindowHighImportance";
        const string AppWindowLowImportanceValue = "AppWindowLowImportance";
        const string AppWindowSubjectValue = "AppWindowSubject";
        const string AppWindowLocationValue = "AppWindowLocation";
        const string AppWindowStartTimeValue = "AppWindowStartTime";
        const string AppWindowEndTimeValue = "AppWindowEndTime";
        const string AppWindowAllDayValue = "AppWindowAllDay";

        //Appointment Window Recurrence
        const string RecurrencePatternValue = "RecurrencePattern";
        const string RecurrenceDailyValue = "RecurrenceDaily";
        const string RecurrenceWeeklyValue = "RecurrenceWeekly";
        const string RecurrenceMonthlyValue = "RecurrenceMonthly";
        const string RecurrenceYearlyValue = "RecurrenceYearly";
        const string RecurrenceRangeValue = "RecurrenceRange";
        const string RecurrenceStartValue = "RecurrenceStart";
        const string RecurrenceNoEndDateValue = "RecurrenceNoEndDate";
        const string RecurrenceEndAfterValue = "RecurrenceEndAfter";
        const string RecurrenceEndByValue = "RecurrenceEndBy";
        const string RecurrenceOccurencesValue = "RecurrenceOccurences";
        const string RecurrenceEveryValue = "RecurrenceEvery";
        const string RecurrenceDayValue = "RecurrenceDay";
        const string RecurrenceDaysValue = "RecurrenceDays";
        const string RecurrenceEveryWeekdayValue = "RecurrenceEveryWeekday";
        const string RecurrenceRecurEveryValue = "RecurrenceRecurEvery";
        const string RecurrenceWeeksOnValue = "RecurrenceWeeksOn";
        const string RecurrenceSundayValue = "RecurrenceSunday";
        const string RecurrenceMondayValue = "RecurrenceMonday";
        const string RecurrenceTuesdayValue = "RecurrenceTuesday";
        const string RecurrenceWednesdayValue = "RecurrenceWednesday";
        const string RecurrenceThursdayValue = "RecurrenceThursday";
        const string RecurrenceFridayValue = "RecurrenceFriday";
        const string RecurrenceSaturdayValue = "RecurrenceSaturday";
        const string RecurrenceOfEveryValue = "RecurrenceOfEvery";
        const string RecurrenceMonthsValue = "RecurrenceMonths";
        const string RecurrenceTheValue = "RecurrenceThe";
        const string RecurrenceOnValue = "RecurrenceOn";
        const string RecurrenceOnTheValue = "RecurrenceOnThe";
        const string RecurrenceOfValue = "RecurrenceOf";
        const string RecurrenceYearsValue = "RecurrenceYears";

        // Next / Previous appointment navigation button
        const string NextAppointmentValue = "NextAppointment";
        const string PreviousAppointmentValue = "PreviousAppointment";

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Controls.Schedule.ResourceWrapper"/> class.
        /// </summary>
        public ResourceWrapper()
        {
            CultureInfo ci = CultureInfo.CurrentUICulture;
            //Appointment Window Basics
            AppWindowSave = SRSchedule.GetString(ci, AppWindowSaveValue);
            AppWindowDelete = SRSchedule.GetString(ci, AppWindowDeleteValue);
            AppWindowShowAs = SRSchedule.GetString(ci, AppWindowShowAsValue);
            AppWindowReminder = SRSchedule.GetString(ci, AppWindowReminderValue);
            AppWindowEnableRecurrence = SRSchedule.GetString(ci, AppWindowEnableRecurrenceValue);
            AppWindowRemoveRecurrence = SRSchedule.GetString(ci, AppWindowRemoveRecurrenceValue);
            AppWindowPrivate = SRSchedule.GetString(ci, AppWindowPrivateValue);
            AppWindowHighImportance = SRSchedule.GetString(ci, AppWindowHighImportanceValue);
            AppWindowLowImportance = SRSchedule.GetString(ci, AppWindowLowImportanceValue);
            AppWindowSubject = SRSchedule.GetString(ci, AppWindowSubjectValue);
            AppWindowLocation = SRSchedule.GetString(ci, AppWindowLocationValue);
            AppWindowStartTime = SRSchedule.GetString(ci, AppWindowStartTimeValue);
            AppWindowEndTime = SRSchedule.GetString(ci, AppWindowEndTimeValue);
            AppWindowAllDay = SRSchedule.GetString(ci, AppWindowAllDayValue);
    
            //Appointment Window Recurrence
            RecurrencePattern = SRSchedule.GetString(ci, RecurrencePatternValue);
            RecurrenceDaily = SRSchedule.GetString(ci, RecurrenceDailyValue);
            RecurrenceWeekly = SRSchedule.GetString(ci, RecurrenceWeeklyValue);
            RecurrenceMonthly = SRSchedule.GetString(ci, RecurrenceMonthlyValue);
            RecurrenceYearly = SRSchedule.GetString(ci, RecurrenceYearlyValue);
            RecurrenceRange = SRSchedule.GetString(ci, RecurrenceRangeValue);
            RecurrenceStart = SRSchedule.GetString(ci, RecurrenceStartValue);
            RecurrenceNoEndDate = SRSchedule.GetString(ci, RecurrenceNoEndDateValue);
            RecurrenceEndAfter = SRSchedule.GetString(ci, RecurrenceEndAfterValue);
            RecurrenceEndBy = SRSchedule.GetString(ci, RecurrenceEndByValue);
            RecurrenceOccurences = SRSchedule.GetString(ci, RecurrenceOccurencesValue);
            RecurrenceEvery = SRSchedule.GetString(ci, RecurrenceEveryValue);
            RecurrenceDay = SRSchedule.GetString(ci, RecurrenceDayValue);
            RecurrenceDays = SRSchedule.GetString(ci, RecurrenceDaysValue);
            RecurrenceEveryWeekday = SRSchedule.GetString(ci, RecurrenceEveryWeekdayValue);
            RecurrenceRecurEvery = SRSchedule.GetString(ci, RecurrenceRecurEveryValue);
            RecurrenceWeeksOn = SRSchedule.GetString(ci, RecurrenceWeeksOnValue);
            RecurrenceSunday = SRSchedule.GetString(ci, RecurrenceSundayValue);
            RecurrenceMonday = SRSchedule.GetString(ci, RecurrenceMondayValue);
            RecurrenceTuesday = SRSchedule.GetString(ci, RecurrenceTuesdayValue);
            RecurrenceWednesday = SRSchedule.GetString(ci, RecurrenceWednesdayValue);
            RecurrenceThursday = SRSchedule.GetString(ci, RecurrenceThursdayValue);
            RecurrenceFriday = SRSchedule.GetString(ci, RecurrenceFridayValue);
            RecurrenceSaturday = SRSchedule.GetString(ci, RecurrenceSaturdayValue);
            RecurrenceOfEvery = SRSchedule.GetString(ci, RecurrenceOfEveryValue);
            RecurrenceMonths = SRSchedule.GetString(ci, RecurrenceMonthsValue);
            RecurrenceThe = SRSchedule.GetString(ci, RecurrenceTheValue);
            RecurrenceOn = SRSchedule.GetString(ci, RecurrenceOnValue);
            RecurrenceOnThe = SRSchedule.GetString(ci, RecurrenceOnTheValue);
            RecurrenceOf = SRSchedule.GetString(ci, RecurrenceOfValue);
            RecurrenceYears = SRSchedule.GetString(ci, RecurrenceYearsValue);

            //Message Box
            AppointmentDeleteMessageBoxContent = SRSchedule.GetString(ci, AppointmentDeleteMessageBoxContentValue);
            AppointmentSaveChangesMessageBoxContent = SRSchedule.GetString(ci, AppointmentSaveChangesMessageBoxContentValue);
            AppointmentDeleteMessageBoxHeader = SRSchedule.GetString(ci, AppointmentDeleteMessageBoxHeaderValue);
            AppointmentSaveChangesMessageBoxHeader = SRSchedule.GetString(ci, AppointmentSaveChangesMessageBoxHeaderValue);

            //goto date window
            GoToDateWindowDate = SRSchedule.GetString(ci, GoToDateWindowDateValue);
            GoToDateWindowHeader = SRSchedule.GetString(ci, GoToDateWindowHeaderValue);
            GoToDateWindowShowIn = SRSchedule.GetString(ci, GoToDateWindowShowInValue);

            //ok cancel
            Ok = SRSchedule.GetString(ci, OkValue);
            cancel = SRSchedule.GetString(ci, CancelValue);

            //recurrence alert window
            RecurrenceAlertWindowContent = SRSchedule.GetString(ci, RecurrenceAlertWindowContentValue);
            RecurrenceAlertWindowHeader = SRSchedule.GetString(ci, RecurrenceAlertWindowHeaderValue);
            RecurrenceAlertWindowOpen = SRSchedule.GetString(ci, RecurrenceAlertWindowOpenValue);
            RecurrenceAlertWindowOpenSeries = SRSchedule.GetString(ci, RecurrenceAlertWindowOpenSeriesValue);

            // next / previous appointment
            NextAppointment = SRSchedule.GetString(ci, NextAppointmentValue);
            PreviousAppointment = SRSchedule.GetString(ci, PreviousAppointmentValue);

            
        }

        // Message Box
        /// <summary>
        ///  Gets and Sets the string value for the delete message box content
        /// </summary>
        public string AppointmentDeleteMessageBoxContent
        {
            get { return appointmentDeleteMessageBoxContent; }
            set { appointmentDeleteMessageBoxContent = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for save changes message box content
        /// </summary>
        public string AppointmentSaveChangesMessageBoxContent
        {
            get { return appointmentSaveChangesMessageBoxContent; }
            set { appointmentSaveChangesMessageBoxContent = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for delete message box header
        /// </summary>
        public string AppointmentDeleteMessageBoxHeader
        {
            get { return appointmentDeleteMessageBoxHeader; }
            set { appointmentDeleteMessageBoxHeader = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for save changes message box header
        /// </summary>
        public string AppointmentSaveChangesMessageBoxHeader
        {
            get { return appointmentSaveChangesMessageBoxHeader; }
            set { appointmentSaveChangesMessageBoxHeader = value; }
        }


        //Recurrence Alert Window
        /// <summary>
        ///  Gets and Sets the string value for recurrence alert window content
        /// </summary>
        public string RecurrenceAlertWindowContent
        {
            get { return recurrenceAlertWindowContent; }
            set { recurrenceAlertWindowContent = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for recurrence alert window header
        /// </summary>
        public string RecurrenceAlertWindowHeader
        {
            get { return recurrenceAlertWindowHeader; }
            set { recurrenceAlertWindowHeader = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for recurrence alert window open
        /// </summary>
        public string RecurrenceAlertWindowOpen
        {
            get { return recurrenceAlertWindowOpen; }
            set { recurrenceAlertWindowOpen = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for recurrence alert window series
        /// </summary>
        public string RecurrenceAlertWindowOpenSeries
        {
            get { return recurrenceAlertWindowOpenSeries; }
            set { recurrenceAlertWindowOpenSeries = value; }
        }

        //go to date window
        /// <summary>
        ///  Gets and Sets the string value for GoTo  window header
        /// </summary>
        public string GoToDateWindowHeader
        {
            get { return goToDateWindowHeader; }
            set { goToDateWindowHeader = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for GoTo  window date
        /// </summary>
        public string GoToDateWindowDate
        {
            get { return goToDateWindowDate; }
            set { goToDateWindowDate = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for GoTo  window show in
        /// </summary>
        public string GoToDateWindowShowIn
        {
            get { return goToDateWindowShowIn; }
            set { goToDateWindowShowIn = value; }
        }

        // Common - Ok Cancel
        /// <summary>
        ///  Gets and Sets the string value 
        /// </summary>
        public string Ok 
        {
            get { return ok; }
            set { ok = value; } 
        }
        /// <summary>
        ///  Gets and Sets the string value 
        /// </summary>
        public string Cancel
        {
            get { return cancel; }
            set { cancel = value; }
        }

        //Appointment Window Basics
        /// <summary>
        ///  Gets and Sets the string value for  app window save
        /// </summary>
        public string AppWindowSave
        {
            get { return appWindowSave; }
            set { appWindowSave = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for  app window delete
        /// </summary>
        public string AppWindowDelete
        {
            get { return appWindowDelete; }
            set { appWindowDelete = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value  for app window Show
        /// </summary>
        public string AppWindowShowAs
        {
            get { return appWindowShowAs; }
            set { appWindowShowAs = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for  app window remainder
        /// </summary>
        public string AppWindowReminder
        {
            get { return appWindowReminder; }
            set { appWindowReminder = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value  for app window enable recurrence
        /// </summary>
        public string AppWindowEnableRecurrence
        {
            get { return appWindowEnableRecurrence; }
            set { appWindowEnableRecurrence = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for app window remove recurrence
        /// </summary>
        public string AppWindowRemoveRecurrence
        {
            get { return appWindowRemoveRecurrence; }
            set { appWindowRemoveRecurrence = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for app window private
        /// </summary>
        public string AppWindowPrivate
        {
            get { return appWindowPrivate; }
            set { appWindowPrivate = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for app window high importance
        /// </summary>
        public string AppWindowHighImportance
        {
            get { return appWindowHighImportance; }
            set { appWindowHighImportance = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for app window low importance
        /// </summary>
        public string AppWindowLowImportance
        {
            get { return appWindowLowImportance; }
            set { appWindowLowImportance = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for app window subject
        /// </summary>
        public string AppWindowSubject
        {
            get { return appWindowSubject; }
            set { appWindowSubject = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for app window location
        /// </summary>
        public string AppWindowLocation
        {
            get { return appWindowLocation; }
            set { appWindowLocation = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for app window start time
        /// </summary>
        public string AppWindowStartTime
        {
            get { return appWindowStartTime; }
            set { appWindowStartTime = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for app window end time
        /// </summary>
        public string AppWindowEndTime
        {
            get { return appWindowEndTime; }
            set { appWindowEndTime = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for  app window all day
        /// </summary>
        public string AppWindowAllDay
        {
            get { return appWindowAllDay; }
            set { appWindowAllDay = value; }
        }


        //Appointment Window Recurrence
        /// <summary>
        ///  Gets and Sets the string value  for  recurrence pattern
        /// </summary>
        public string RecurrencePattern
        {
            get { return recurrencePattern; }
            set { recurrencePattern = value; }
        }
         /// <summary>
        ///  Gets and Sets the string value for  recurrence daily
        /// </summary>
     
        public string RecurrenceDaily
        {
            get { return recurrenceDaily; }
            set { recurrenceDaily = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for  recurrence weekly
        /// </summary>
     
        public string RecurrenceWeekly
        {
            get { return recurrenceWeekly; }
            set { recurrenceWeekly = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for  recurrence monthly
        /// </summary>
     
        public string RecurrenceMonthly
        {
            get { return recurrenceMonthly; }
            set { recurrenceMonthly = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for  recurrence yearly
        /// </summary>
     
        public string RecurrenceYearly
        {
            get { return recurrenceYearly; }
            set { recurrenceYearly = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for  recurrence range
        /// </summary>
     
        public string RecurrenceRange
        {
            get { return recurrenceRange; }
            set { recurrenceRange = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for  recurrence start
        /// </summary>
     
        public string RecurrenceStart
        {
            get { return recurrenceStart; }
            set { recurrenceStart = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for  recurrence end date
        /// </summary>
     
        public string RecurrenceNoEndDate
        {
            get { return recurrenceNoEndDate; }
            set { recurrenceNoEndDate = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for  recurrence end after
        /// </summary>
     
        public string RecurrenceEndAfter
        {
            get { return recurrenceEndAfter; }
            set { recurrenceEndAfter = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for  recurrence by
        /// </summary>
     
        public string RecurrenceEndBy
        {
            get { return recurrenceEndBy; }
            set { recurrenceEndBy = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for  recurrence occurrence
        /// </summary>
     
        public string RecurrenceOccurences
        {
            get { return recurrenceOccurences; }
            set { recurrenceOccurences = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for  recurrence every
        /// </summary>
     
        public string RecurrenceEvery
        {
            get { return recurrenceEvery; }
            set { recurrenceEvery = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for  recurrence day
        /// </summary>
     
        public string RecurrenceDay
        {
            get { return recurrenceDay; }
            set { recurrenceDay = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for  recurrence days
        /// </summary>
     
        public string RecurrenceDays
        {
            get { return recurrenceDays; }
            set { recurrenceDays = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for  recurrence weekdays
        /// </summary>
     
        public string RecurrenceEveryWeekday
        {
            get { return recurrenceEveryWeekday; }
            set { recurrenceEveryWeekday = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for  recurrence every
        /// </summary>
     
        public string RecurrenceRecurEvery
        {
            get { return recurrenceRecurEvery; }
            set { recurrenceRecurEvery = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for  recurrence weeks on
        /// </summary>
     
        public string RecurrenceWeeksOn
        {
            get { return recurrenceWeeksOn; }
            set { recurrenceWeeksOn = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for  recurrence Sunday
        /// </summary>
     
        public string RecurrenceSunday
        {
            get { return recurrenceSunday; }
            set { recurrenceSunday = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for  recurrence Monday
        /// </summary>
     
        public string RecurrenceMonday
        {
            get { return recurrenceMonday; }
            set { recurrenceMonday = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for  recurrence Tuesday
        /// </summary>
     
        public string RecurrenceTuesday
        {
            get { return recurrenceTuesday; }
            set { recurrenceTuesday = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for  recurrence Wednesday
        /// </summary>
     
        public string RecurrenceWednesday
        {
            get { return recurrenceWednesday; }
            set { recurrenceWednesday = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for  recurrence Thursday
        /// </summary>
     
        public string RecurrenceThursday
        {
            get { return recurrenceThursday; }
            set { recurrenceThursday = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for  recurrence Friday
        /// </summary>
     
        public string RecurrenceFriday
        {
            get { return recurrenceFriday; }
            set { recurrenceFriday = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for  recurrence Saturday
        /// </summary>
     
        public string RecurrenceSaturday
        {
            get { return recurrenceSaturday; }
            set { recurrenceSaturday = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for  recurrence of every someday.
        /// </summary>
     
        public string RecurrenceOfEvery
        {
            get { return recurrenceOfEvery; }
            set { recurrenceOfEvery = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for  recurrence months
        /// </summary>
     
        public string RecurrenceMonths
        {
            get { return recurrenceMonths; }
            set { recurrenceMonths = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for  recurrence the
        /// </summary>
     
        public string RecurrenceThe
        {
            get { return recurrenceThe; }
            set { recurrenceThe = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for  recurrence on
        /// </summary>
     
        public string RecurrenceOn
        {
            get { return recurrenceOn; }
            set { recurrenceOn = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for  recurrence on the
        /// </summary>
     
        public string RecurrenceOnThe
        {
            get { return recurrenceOnThe; }
            set { recurrenceOnThe = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for  recurrence of
        /// </summary>
     
        public string RecurrenceOf
        {
            get { return recurrenceOf; }
            set { recurrenceOf = value; }
        }
        /// <summary>
        ///  Gets and Sets the string value for  recurrence years
        /// </summary>
     
        public string RecurrenceYears
        {
            get { return recurrenceYears; }
            set { recurrenceYears = value; }
        }

        /// <summary>
        ///  Gets and Sets the string value for  next appointment
        /// </summary>
     
        public string NextAppointment
        {
            get { return nextAppointment; }
            set { nextAppointment = value; }
        }

        /// <summary>
        /// Gets and Sets the string value for previous appointment
        /// </summary>
        public string PreviousAppointment
        {
            get { return previousAppointment; }
            set { previousAppointment = value; }
        }


        //recurrence alert window
        string recurrenceAlertWindowContent;
        string recurrenceAlertWindowHeader;
        string recurrenceAlertWindowOpen;
        string recurrenceAlertWindowOpenSeries;

        //Go to Date Window
        string goToDateWindowHeader;
        string goToDateWindowDate;
        string goToDateWindowShowIn;

        //common ok cancel
        string ok;
        string cancel;

        //message box
        string appointmentDeleteMessageBoxContent;
        string appointmentSaveChangesMessageBoxContent;
        string appointmentDeleteMessageBoxHeader;
        string appointmentSaveChangesMessageBoxHeader;

        //next / previous Appointment
        string nextAppointment;
        string previousAppointment;

        //Appointment Window Basics
        string appWindowSave;
        string appWindowDelete;
        string appWindowShowAs;
        string appWindowReminder;
        string appWindowEnableRecurrence;
        string appWindowRemoveRecurrence;
        string appWindowPrivate;
        string appWindowHighImportance;
        string appWindowLowImportance;
        string appWindowSubject;
        string appWindowLocation;
        string appWindowStartTime;
        string appWindowEndTime;
        string appWindowAllDay;

        //Appointment Window Recurrence
        string recurrencePattern;
        string recurrenceDaily;
        string recurrenceWeekly;
        string recurrenceMonthly;
        string recurrenceYearly;
        string recurrenceRange;
        string recurrenceStart;
        string recurrenceNoEndDate;
        string recurrenceEndAfter;
        string recurrenceEndBy;
        string recurrenceOccurences;
        string recurrenceEvery;
        string recurrenceDay;
        string recurrenceDays;
        string recurrenceEveryWeekday;
        string recurrenceRecurEvery;
        string recurrenceWeeksOn;
        string recurrenceSunday;
        string recurrenceMonday;
        string recurrenceTuesday;
        string recurrenceWednesday;
        string recurrenceThursday;
        string recurrenceFriday;
        string recurrenceSaturday;
        string recurrenceOfEvery;
        string recurrenceMonths;
        string recurrenceThe;
        string recurrenceOn;
        string recurrenceOnThe;
        string recurrenceOf;
        string recurrenceYears;

    }
}
