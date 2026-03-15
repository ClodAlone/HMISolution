#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
#if WINRT
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#else
using System.Windows;
using System.Windows.Media;
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    /// <summary>
    /// Represents a schedule appointment.
    /// </summary>
    public class ScheduleAppointment : DependencyObject, INotifyPropertyChanged
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Schedule.ScheduleAppointment">ScheduleAppointment</see> class.
        /// </summary>
        public ScheduleAppointment()
        {
            RecurrenceID = GetHashCode();
            InternalStartTime = DateTime.Now;
            InternalEndTime = DateTime.Now;
            ResourceCollection = new ObservableCollection<Resource>();
            StartTimeZone = new TimeZone { TimeZoneValue = TimeZoneInfo.Local.ToString() };
            EndTimeZone = new TimeZone { TimeZoneValue = TimeZoneInfo.Local.ToString() };
        }

        #endregion

        #region Internal Fields

        internal bool hasInternalRule;

        #endregion

        #region DependencyProperties

        #region Public Properties

        #region IsRecursive
        /// <summary>
        /// Gets or sets a value indicating whether the appointment should be recursive.
        /// </summary>
        /// <example>
        /// using System;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleWinRT
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.ScheduleType = ScheduleType.Month;
        ///             ScheduleAppointment appointment = new ScheduleAppointment
        ///             {
        ///                 Subject = "Meeting",
        ///                 StartTime = DateTime.Now.AddHours(1),
        ///                 EndTime = DateTime.Now.AddHours(2)
        ///             };
        ///             RecurrenceProperties RecProp = new RecurrenceProperties
        ///             {
        ///                 RecurrenceType = RecurrenceType.Daily,
        ///                 IsDailyEveryNDays = false,
        ///                 DailyNDays = 2,
        ///                 IsRangeNoEndDate = true,
        ///                 RangeEndDate = DateTime.Now.AddDays(10),
        ///                 RangeRecurrenceCount = 100
        ///             };
        ///             // Generating RRule using ScheduleHelper
        ///             appointment.RecurrenceRule = ScheduleHelper.RRuleGenerator(RecProp, appointment.StartTime, appointment.EndTime);
        ///             appointment.IsRecursive = true;
        ///             schedule.Appointments.Add(appointment);
        ///             Grid.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public bool IsRecursive
        {
            get { return (bool)GetValue(IsRecursiveProperty); }
            set { SetValue(IsRecursiveProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsRecursive.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsRecursiveProperty =
            DependencyProperty.Register("IsRecursive", typeof(bool), typeof(ScheduleAppointment), new PropertyMetadata(false));
        #endregion

        #region RecurrenceRule
        /// <summary>
        /// Gets or sets the rule for recurrence appointments.
        /// </summary>
        /// <remarks>
        /// To set recurring appointment, IsRecursive of schedule appointment must be set as true.
        /// </remarks>
        /// <example>
        /// using System;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleWinRT
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.ScheduleType = ScheduleType.Month;
        ///             ScheduleAppointment appointment = new ScheduleAppointment
        ///             {
        ///                 Subject = "Meeting",
        ///                 StartTime = DateTime.Now.AddHours(1),
        ///                 EndTime = DateTime.Now.AddHours(2)
        ///             };
        ///             RecurrenceProperties RecProp = new RecurrenceProperties
        ///             {
        ///                 RecurrenceType = RecurrenceType.Daily,
        ///                 IsDailyEveryNDays = false,
        ///                 DailyNDays = 2,
        ///                 IsRangeNoEndDate = true,
        ///                 RangeEndDate = DateTime.Now.AddDays(10),
        ///                 RangeRecurrenceCount = 100
        ///             };
        ///             // Generating RRule using ScheduleHelper
        ///             appointment.RecurrenceRule = ScheduleHelper.RRuleGenerator(RecProp, appointment.StartTime, appointment.EndTime);
        ///             appointment.IsRecursive = true;
        ///             schedule.Appointments.Add(appointment);
        ///             Grid.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public string RecurrenceRule
        {
            get { return (string)GetValue(RecurrenceRuleProperty); }
            set
            {
                SetValue(RecurrenceRuleProperty, value);
                if (!hasInternalRule && ScheduleHelper.RecPropertiesDict.Count > 0 && ScheduleHelper.RecPropertiesDict.ContainsKey(value))
				    RecurrenceProperites = ScheduleHelper.RecPropertiesDict[value];
				else
                    RecurrenceProperites = ScheduleHelper.RRuleParser(value, StartTime);
                OnPropertyChanged("RecurrenceRule");
            }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RecurrenceRule.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RecurrenceRuleProperty =
            DependencyProperty.Register("RecurrenceRule", typeof(string), typeof(ScheduleAppointment), new PropertyMetadata(string.Empty));
        #endregion

        #region StartTimeZone
        /// <summary>
        /// Gets or sets the time zone for appointment's start time.
        /// </summary>
        /// <example>
        /// using System;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             ScheduleAppointment appointment = new ScheduleAppointment();
        ///             appointment.StartTime = DateTime.Now.AddHours(1);
        ///             appointment.StartTimeZone = new TimeZone { TimeZoneValue = TimeZoneInfo.Local.ToString() };
        ///             appointment.EndTime = DateTime.Now.AddHours(2);
        ///             schedule.Appointments.Add(appointment);
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public TimeZone StartTimeZone
        {
            get { return (TimeZone)GetValue(StartTimeZoneProperty); }
            set { SetValue(StartTimeZoneProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for StartTimeZone.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StartTimeZoneProperty =
            DependencyProperty.Register("StartTimeZone", typeof(TimeZone), typeof(ScheduleAppointment), new PropertyMetadata(null));
        #endregion

        #region EndTimeZone
        /// <summary>
        /// Gets or sets the time zone for appointment's end time.
        /// </summary>
        /// <example>
        /// using System;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             ScheduleAppointment appointment = new ScheduleAppointment();
        ///             appointment.StartTime = DateTime.Now.AddHours(1);
        ///             appointment.EndTime = DateTime.Now.AddHours(2);
        ///             appointment.EndTimeZone = new TimeZone { TimeZoneValue = TimeZoneInfo.Local.ToString() };
        ///             schedule.Appointments.Add(appointment);
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public TimeZone EndTimeZone
        {
            get { return (TimeZone)GetValue(EndTimeZoneProperty); }
            set { SetValue(EndTimeZoneProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for EndTimeZone.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EndTimeZoneProperty =
            DependencyProperty.Register("EndTimeZone", typeof(TimeZone), typeof(ScheduleAppointment), new PropertyMetadata(null));
        #endregion

        #region ReminderTime
        /// <summary>
        /// Gets or sets the reminder time for appointments
        /// </summary>
        /// <example>
        /// using System;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             ScheduleAppointment appointment = new ScheduleAppointment();
        ///             appointment.StartTime = DateTime.Now.AddHours(1);
        ///             appointment.EndTime = DateTime.Now.AddHours(2);
        ///             appointment.ReminderTime = ReminderTimeType.OneHour;
        ///             schedule.Appointments.Add(appointment);
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public ReminderTimeType ReminderTime
        {
            get { return (ReminderTimeType)GetValue(ReminderTimeProperty); }
            set { SetValue(ReminderTimeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ReminderTime.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ReminderTimeProperty =
            DependencyProperty.Register("ReminderTime", typeof(ReminderTimeType), typeof(ScheduleAppointment), new PropertyMetadata(ReminderTimeType.None, OnReminderTimeChanged));

        private static void OnReminderTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var scheduleAppointment = d as ScheduleAppointment;
            if (scheduleAppointment != null) scheduleAppointment.OnPropertyChanged("ReminderTime");
        }
        #endregion

        #region AllDay
        /// <summary>
        /// Gets or sets the value indicating whether the appointment should be extended for an entire day.
        /// </summary>
        /// <example>
        /// using System;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             ScheduleAppointment appointment = new ScheduleAppointment();
        ///             appointment.StartTime = DateTime.Now.AddHours(1);
        ///             appointment.EndTime = DateTime.Now.AddHours(2);
        ///             appointment.AllDay = true;
        ///             schedule.Appointments.Add(appointment);
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public bool AllDay
        {
            get { return (bool)GetValue(AllDayProperty); }
            set { SetValue(AllDayProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AllDay.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AllDayProperty =
            DependencyProperty.Register("AllDay", typeof(bool), typeof(ScheduleAppointment), new PropertyMetadata(false, OnAllDayChanged));

        private static void OnAllDayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (ScheduleAppointment)d;
            instance.OnAllDayChanged(e);
            instance.OnPropertyChanged("AllDay");
        }

        private void OnAllDayChanged(DependencyPropertyChangedEventArgs e)
        {
            if (AllDayChanged != null)
            {
                AllDayChanged(this, e);
            }
        }

        #endregion

        #region StartTime
        /// <summary>
        /// Gets the sets the appointment's start time.
        /// </summary>
        /// <example>
        /// using System;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             ScheduleAppointment appointment = new ScheduleAppointment();
        ///             appointment.StartTime = DateTime.Now.AddHours(1);
        ///             appointment.EndTime = DateTime.Now.AddHours(2);
        ///             schedule.Appointments.Add(appointment);
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public DateTime StartTime
        {
            get { return (DateTime)GetValue(StartTimeProperty); }
            set { SetValue(StartTimeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for StartTime.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StartTimeProperty =
            DependencyProperty.Register("StartTime", typeof(DateTime), typeof(ScheduleAppointment), new PropertyMetadata(DateTime.Now, OnStartTimeChanged));

        private static void OnStartTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                var scheduleAppointment = d as ScheduleAppointment;
                if (scheduleAppointment != null)
                    scheduleAppointment.InternalStartTime = scheduleAppointment.StartTime;
            }
        }
        #endregion

        #region EndTime
        /// <summary>
        /// Gets the sets the appointment's end time.
        /// </summary>
        /// <example>
        /// using System;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             ScheduleAppointment appointment = new ScheduleAppointment();
        ///             appointment.StartTime = DateTime.Now.AddHours(1);
        ///             appointment.EndTime = DateTime.Now.AddHours(2);
        ///             schedule.Appointments.Add(appointment);
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public DateTime EndTime
        {
            get { return (DateTime)GetValue(EndTimeProperty); }
            set { SetValue(EndTimeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for EndTime.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EndTimeProperty =
            DependencyProperty.Register("EndTime", typeof(DateTime), typeof(ScheduleAppointment), new PropertyMetadata(DateTime.Now, OnEndTimeChanged));

        private static void OnEndTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var app = d as ScheduleAppointment;
            if (app != null)
            {
                if (app.EndTime < app.StartTime)
                {
                    app.EndTime = app.StartTime.AddMinutes(30);
                }
                app.InternalEndTime = app.EndTime;
                if (app.InternalStartTime > app.InternalEndTime)
                {
                    DateTime diff = ConvertToLocalTime(app.InternalStartTime, "EndTime", null, app.EndTimeZone);
                    TimeSpan span = app.InternalStartTime - diff - (app.EndTime - app.StartTime);
                    if (span.TotalHours < 0)
                    {
                        DateTime diff1 = ConvertToLocalTime(app.InternalEndTime, "StartTime", app.StartTimeZone, null);
                        TimeSpan span1 = diff1 - app.InternalEndTime;
                        app.StartTime = app.InternalEndTime.Subtract(span1).AddMinutes(-30);
                    }
                    else
                    {
                        app.EndTime = app.EndTime.Add(span).AddMinutes(30);
                        app.InternalEndTime = app.EndTime;
                    }

                }
            }
        }
        #endregion

        #region Status
        /// <summary>
        /// Gets or sets the appointment's status.
        /// </summary>
        /// <example>
        /// using System;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             ScheduleAppointment appointment = new ScheduleAppointment();
        ///             appointment.StartTime = DateTime.Now.AddHours(1);
        ///             appointment.EndTime = DateTime.Now.AddHours(2);
        ///             appointment.Status = schedule.AppointmentStatusCollection[0];
        ///             schedule.Appointments.Add(appointment);
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public ScheduleAppointmentStatus Status
        {
            get { return (ScheduleAppointmentStatus)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Status.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(ScheduleAppointmentStatus), typeof(ScheduleAppointment), new PropertyMetadata(null, OnStatusChanged));

        private static void OnStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var scheduleAppointment = d as ScheduleAppointment;
            if (scheduleAppointment != null) scheduleAppointment.OnPropertyChanged("Status");
        }
        #endregion

        #region EventCount



        internal string EventCount
        {
            get { return (string)GetValue(EventCountProperty); }
            set { SetValue(EventCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EventCount.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty EventCountProperty =
            DependencyProperty.Register("EventCount", typeof(string), typeof(ScheduleAppointment), new PropertyMetadata(string.Empty));

        

        #endregion

        #region Subject
        /// <summary>
        /// Gets or sets the subject for an appointment.
        /// </summary>
        /// <example>
        /// using System;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             ScheduleAppointment appointment = new ScheduleAppointment();
        ///             appointment.StartTime = DateTime.Now.AddHours(1);
        ///             appointment.EndTime = DateTime.Now.AddHours(2);
        ///             appointment.Subject = "Meeting";
        ///             schedule.Appointments.Add(appointment);
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public string Subject
        {
            get { return (string)GetValue(SubjectProperty); }
            set { SetValue(SubjectProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Subject.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SubjectProperty =
            DependencyProperty.Register("Subject", typeof(string), typeof(ScheduleAppointment), new PropertyMetadata(string.Empty, OnSubjectChanged));

        private static void OnSubjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (ScheduleAppointment)d;
            instance.OnSubjectChanged(e);
            instance.OnPropertyChanged("Subject");
        }

        private void OnSubjectChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SubjectChanged != null)
            {
                SubjectChanged(this, e);
            }
        }
        #endregion

        #region Location
        /// <summary>
        /// Gets or sets the location for an appointment.
        /// </summary>
        /// <example>
        /// using System;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             ScheduleAppointment appointment = new ScheduleAppointment();
        ///             appointment.StartTime = DateTime.Now.AddHours(1);
        ///             appointment.EndTime = DateTime.Now.AddHours(2);
        ///             appointment.Location = "Chennai";
        ///             schedule.Appointments.Add(appointment);
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public string Location
        {
            get { return (string)GetValue(LocationProperty); }
            set { SetValue(LocationProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Location.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LocationProperty =
            DependencyProperty.Register("Location", typeof(string), typeof(ScheduleAppointment), new PropertyMetadata(string.Empty, OnLocationChanged));

        private static void OnLocationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (ScheduleAppointment)d;
            instance.OnLocationChanged(e);
            instance.OnPropertyChanged("Location");
        }

        private void OnLocationChanged(DependencyPropertyChangedEventArgs e)
        {
            if (LocationChanged != null)
            {
                LocationChanged(this, e);
            }
        }
        #endregion

        #region Notes
        /// <summary>
        /// Gets or sets the notes for an appointment.
        /// </summary>
        /// <example>
        /// using System;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             ScheduleAppointment appointment = new ScheduleAppointment();
        ///             appointment.StartTime = DateTime.Now.AddHours(1);
        ///             appointment.EndTime = DateTime.Now.AddHours(2);
        ///             appointment.Notes = "Inauguration";
        ///             schedule.Appointments.Add(appointment);
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public string Notes
        {
            get { return (string)GetValue(NotesProperty); }
            set { SetValue(NotesProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Notes.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty NotesProperty =
            DependencyProperty.Register("Notes", typeof(string), typeof(ScheduleAppointment), new PropertyMetadata(string.Empty, OnNotesChanged));

        private static void OnNotesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var scheduleAppointment = d as ScheduleAppointment;
            if (scheduleAppointment != null) scheduleAppointment.OnPropertyChanged("Notes");
        }
        #endregion

        #region AppointmentBackground
        /// <summary>
        /// Gets or sets the background for an appointment.
        /// </summary>
        /// <example>
        /// using System;
        /// using Windows.UI;
        /// using Windows.UI.Xaml.Media;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             ScheduleAppointment appointment = new ScheduleAppointment();
        ///             appointment.StartTime = DateTime.Now.AddHours(1);
        ///             appointment.EndTime = DateTime.Now.AddHours(2);
        ///             appointment.AppointmentBackground = new
        /// SolidColorBrush(Colors.Orange);
        ///             schedule.Appointments.Add(appointment);
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public Brush AppointmentBackground
        {
            get { return (Brush)GetValue(AppointmentBackgroundProperty); }
            set { SetValue(AppointmentBackgroundProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AppointmentBackground.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AppointmentBackgroundProperty =
            DependencyProperty.Register("AppointmentBackground", typeof(Brush), typeof(ScheduleAppointment), new PropertyMetadata((new SolidColorBrush(Color.FromArgb(0xFF, 0x1B, 0xA1, 0xE2))), OnAppointmentBackgroundChanged));

        private static void OnAppointmentBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var scheduleAppointment = d as ScheduleAppointment;
            if (scheduleAppointment != null) scheduleAppointment.OnPropertyChanged("AppointmentBackground");
        }
        #endregion

        #region ReadOnly
        /// <summary>
        /// Gets or sets a value indicating whether the appointment should be modified.
        /// </summary>
        /// <example>
        /// using System;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             ScheduleAppointment appointment = new ScheduleAppointment();
        ///             appointment.StartTime = DateTime.Now.AddHours(1);
        ///             appointment.EndTime = DateTime.Now.AddHours(2);
        ///             appointment.ReadOnly = true;
        ///             schedule.Appointments.Add(appointment);
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public bool ReadOnly
        {
            get { return (bool)GetValue(ReadOnlyProperty); }
            set { SetValue(ReadOnlyProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ReadOnly.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ReadOnlyProperty =
            DependencyProperty.Register("ReadOnly", typeof(bool), typeof(ScheduleAppointment), new PropertyMetadata(false, OnReadOnlyChanged));

        private static void OnReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var scheduleApp = (ScheduleAppointment)d;
            scheduleApp.ReadOnlyVisibility = scheduleApp.ReadOnly ? Visibility.Visible : Visibility.Collapsed;
            scheduleApp.OnPropertyChanged("ReadOnly");
        }
        #endregion

        #region ResourceCollection
        /// <summary>
        /// Gets or sets the collection of resources for an appointment.
        /// </summary>
        /// <example>
        /// using System;
        /// using System.Collections.ObjectModel;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             ResourceType resourceType = new ResourceType { TypeName = "Doctors" };
        ///             resourceType.ResourceCollection.Add(new Resource { ResourceName = "Resource1", DisplayName = "Dr.John, M.B.B.S" });
        ///             resourceType.ResourceCollection.Add(new Resource { ResourceName = "Resource2", DisplayName = "Dr.Jessie, M.B.B.S" });
        ///             schedule.ScheduleResourceTypeCollection = new ObservableCollection<ResourceType> { resourceType };
        ///             schedule.Resource = "Doctors";
        ///             ScheduleAppointment appointment = new ScheduleAppointment();
        ///             appointment.StartTime = DateTime.Now.AddHours(1);
        ///             appointment.EndTime = DateTime.Now.AddHours(2);
        ///             appointment.ResourceCollection.Add(new Resource { ResourceName = "Resource1", TypeName = "Doctors", DisplayName = "Dr.John, M.B.B.S" });
        ///             schedule.Appointments.Add(appointment);
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public ObservableCollection<Resource> ResourceCollection
        {
            get { return (ObservableCollection<Resource>)GetValue(ResourceCollectionProperty); }
            set { SetValue(ResourceCollectionProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ResourceCollection.  This enables animation, styling, binding, etc...
        /// </summary>
         public static readonly DependencyProperty ResourceCollectionProperty =
            DependencyProperty.Register("ResourceCollection", typeof(ObservableCollection<Resource>), typeof(ScheduleAppointment), new PropertyMetadata(null,OnResourceCollectionPropertyChanged));

        private static void OnResourceCollectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (ScheduleAppointment)d;
            
            instance.OnPropertyChanged("ResourceCollection");
        }        
        #endregion

        #endregion

        #region Read-only Properties

        #region IsSelected
        /// <summary>
        /// Gets a value indicating whether the appointment gets selected.
        /// </summary>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            internal set { SetValue(IsSelectedProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsSelected.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(ScheduleAppointment), new PropertyMetadata(false));
        #endregion

        #region ReadOnlyVisibility
        /// <summary>
        /// Gets the visibility of read only appointment.
        /// </summary>
        public Visibility ReadOnlyVisibility
        {
            get { return (Visibility)GetValue(ReadOnlyVisibilityProperty); }
            internal set { SetValue(ReadOnlyVisibilityProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ReadOnlyVisibility.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ReadOnlyVisibilityProperty =
            DependencyProperty.Register("ReadOnlyVisibility", typeof(Visibility), typeof(ScheduleAppointment), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #region RecurrenceID
        /// <summary>
        /// Gets an unique ID for referring recurrence appointment.
        /// </summary>
        public double RecurrenceID
        {
            get { return (double)GetValue(RecurrenceIDProperty); }
            internal set { SetValue(RecurrenceIDProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RecurrenceID.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RecurrenceIDProperty =
            DependencyProperty.Register("RecurrenceID", typeof(double), typeof(ScheduleAppointment), new PropertyMetadata(0d));
        #endregion

        #region ObjectID
        /// <summary>
        /// Gets an unique ID for referring items in ItemsSource of Schedule.
        /// </summary>
        public double ObjectID
        {
            get { return (double)GetValue(ObjectIDProperty); }
            internal set { SetValue(ObjectIDProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ObjectID.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ObjectIDProperty =
            DependencyProperty.Register("ObjectID", typeof(double), typeof(ScheduleAppointment), new PropertyMetadata(0d));
        #endregion

        #endregion

        #region Internal Properties

        #region RecurrenceProperty
        internal RecurrenceProperties RecurrenceProperites
        {
            get { return (RecurrenceProperties)GetValue(RecurrenceProperitesProperty); }
            set { SetValue(RecurrenceProperitesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RecurrenceProperites.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty RecurrenceProperitesProperty =
            DependencyProperty.Register("RecurrenceProperites", typeof(RecurrenceProperties), typeof(ScheduleAppointment), new PropertyMetadata(null, OnRecurrenceProperitesChanged));

        private static void OnRecurrenceProperitesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var scheduleAppointment = d as ScheduleAppointment;
            if (scheduleAppointment != null)
            {
                scheduleAppointment.OnPropertyChanged("RecurrenceProperites");
                if (scheduleAppointment.RecurrenceProperites != null)
                {
                    RecurrenceProperties properties = e.NewValue as RecurrenceProperties;
                    if (!properties.hasRangeStartDate && scheduleAppointment.StartTime != properties.RangeStartDate)
                    {
                        properties.RangeStartDate = scheduleAppointment.StartTime;
                    }
                }
            }
        }
        #endregion

        #region AppointmentBorderThickness
        internal Thickness AppointmentBorderThickness
        {
            get { return (Thickness)GetValue(AppointmentBorderThicknessProperty); }
            set { SetValue(AppointmentBorderThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AppointmentBorderThickness.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty AppointmentBorderThicknessProperty =
            DependencyProperty.Register("AppointmentBorderThickness", typeof(Thickness), typeof(ScheduleAppointment), new PropertyMetadata(new Thickness(0)));
        #endregion

        #region IsSet
        internal bool IsSet
        {
            get { return (bool)GetValue(IsSetProperty); }
            set { SetValue(IsSetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSet.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty IsSetProperty =
            DependencyProperty.Register("IsSet", typeof(bool), typeof(ScheduleAppointment), new PropertyMetadata(false));
        #endregion

        #region ReminderDeliveryTime
        internal DateTime? ReminderDeliveryTime
        {
            get { return (DateTime?)GetValue(ReminderDeliveryTimeProperty); }
            set { SetValue(ReminderDeliveryTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ReminderDeliveryTime.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ReminderDeliveryTimeProperty =
            DependencyProperty.Register("ReminderDeliveryTime", typeof(DateTime?), typeof(ScheduleAppointment), new PropertyMetadata(null));
        #endregion

        #region InternalStartTime
        internal DateTime InternalStartTime
        {
            get { return ConvertToLocalTime((DateTime)GetValue(InternalStartTimeProperty), "StartTime", StartTimeZone, null); }
            set { SetValue(InternalStartTimeProperty, value); }
        }

#if WINRT
        internal static readonly DependencyProperty InternalStartTimeProperty =
            DependencyProperty.Register("InternalStartTime", typeof(DateTime), typeof(ScheduleAppointment), new PropertyMetadata(null, OnInternalStartTimeChanged));
#else
        internal static readonly DependencyProperty InternalStartTimeProperty =
            DependencyProperty.Register("InternalStartTime", typeof(DateTime), typeof(ScheduleAppointment), new PropertyMetadata(DateTime.Now, OnInternalStartTimeChanged));
#endif

        private static void OnInternalStartTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var scheduleAppointment = d as ScheduleAppointment;
            if (scheduleAppointment != null) scheduleAppointment.OnPropertyChanged("InternalStartTime");
        }
        #endregion

        #region InternalEndTime
        internal DateTime InternalEndTime
        {
            get { return ConvertToLocalTime((DateTime)GetValue(InternalEndTimeProperty), "EndTime", null, EndTimeZone); }
            set { SetValue(InternalEndTimeProperty, value); }
        }

#if WINRT
        internal static readonly DependencyProperty InternalEndTimeProperty =
            DependencyProperty.Register("InternalEndTime", typeof(DateTime), typeof(ScheduleAppointment), new PropertyMetadata(null, OnInternalEndTimeChanged));
#else
        internal static readonly DependencyProperty InternalEndTimeProperty =
            DependencyProperty.Register("InternalEndTime", typeof(DateTime), typeof(ScheduleAppointment), new PropertyMetadata(DateTime.Now, OnInternalEndTimeChanged));
#endif

        private static void OnInternalEndTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var scheduleAppointment = d as ScheduleAppointment;
            if (scheduleAppointment != null) scheduleAppointment.OnPropertyChanged("InternalEndTime");
        }
        #endregion

        #region IsRecursiveModified
        internal bool IsRecursiveModified
        {
            get { return (bool)GetValue(IsRecursiveModifiedProperty); }
            set { SetValue(IsRecursiveModifiedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsRecursiveModified.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty IsRecursiveModifiedProperty =
            DependencyProperty.Register("IsRecursiveModified", typeof(bool), typeof(ScheduleAppointment), new PropertyMetadata(false));
        #endregion

        #region AppointmentSelectionBrush
        internal Brush AppointmentSelectionBrush
        {
            get { return (Brush)GetValue(AppointmentSelectionBrushProperty); }
            set { SetValue(AppointmentSelectionBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AppointmentSelectionBrush.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty AppointmentSelectionBrushProperty =
            DependencyProperty.Register("AppointmentSelectionBrush", typeof(Brush), typeof(ScheduleAppointment), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));
        #endregion

        #endregion

        #endregion

        #region Events

        public event PropertyChangedCallback AllDayChanged;
        public event PropertyChangedCallback LocationChanged;
        public event PropertyChangedCallback SubjectChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            var eventHandler = PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region Methods

        #region Convert Local Time

        internal static DateTime ConvertToLocalTime(DateTime date, string property, TimeZone StartTimeZone, TimeZone EndTimeZone)
        {
            DateTime newdate = date;
            
                if (property.Equals("StartTime"))
                {
                    if (StartTimeZone != null && !TimeZoneInfo.Local.DisplayName.Equals(StartTimeZone.TimeZoneValue))
                    {
                        int hrs;
                        int min;
                        int.TryParse(StartTimeZone.TimeZoneValue.Substring(4, 3), out hrs);
                        int.TryParse(StartTimeZone.TimeZoneValue.Substring(8, 2), out min);
                        var span = new TimeSpan(hrs, min, 0);
                        TimeSpan diff = span - TimeZoneInfo.Local.BaseUtcOffset;
                        newdate = newdate.Add(diff);

                    }
                }
                else if (EndTimeZone != null && property.Equals("EndTime"))
                {
                    if (!TimeZoneInfo.Local.DisplayName.Equals(EndTimeZone.TimeZoneValue))
                    {
                        int hrs;
                        int min;
                        int.TryParse(EndTimeZone.TimeZoneValue.Substring(4, 3), out hrs);
                        int.TryParse(EndTimeZone.TimeZoneValue.Substring(8, 2), out min);
                        var span = new TimeSpan(hrs, min, 0);
                        TimeSpan diff = span - TimeZoneInfo.Local.BaseUtcOffset;
                        newdate = newdate.Add(diff);

                    }
                }            
            return newdate;
        }

        internal static DateTime ConvertToActualTime(DateTime date, string property, TimeZone StartTimeZone, TimeZone EndTimeZone)
        {
            DateTime newdate = date;
            if (property.Equals("StartTime"))
            {
                if (StartTimeZone != null && !TimeZoneInfo.Local.DisplayName.Equals(StartTimeZone.TimeZoneValue))
                {
                    int hrs;
                    int min;
                    int.TryParse(StartTimeZone.TimeZoneValue.Substring(4, 3), out hrs);
                    int.TryParse(StartTimeZone.TimeZoneValue.Substring(8, 2), out min);
                    var span = new TimeSpan(hrs, min, 0);
                    TimeSpan diff = span - TimeZoneInfo.Local.BaseUtcOffset;
                    newdate = newdate.Subtract(diff);

                }
            }
            else if (property.Equals("EndTime"))
            {
                if (EndTimeZone != null && !TimeZoneInfo.Local.DisplayName.Equals(EndTimeZone.TimeZoneValue))
                {
                    int hrs;
                    int min;
                    int.TryParse(EndTimeZone.TimeZoneValue.Substring(4, 3), out hrs);
                    int.TryParse(EndTimeZone.TimeZoneValue.Substring(8, 2), out min);
                    var span = new TimeSpan(hrs, min, 0);
                    TimeSpan diff = span - TimeZoneInfo.Local.BaseUtcOffset;
                    newdate = newdate.Subtract(diff);

                }
            }
            return newdate;
        }

        #endregion

        #region FindingIntersection Appointments

        internal bool IsIntersecting(ScheduleAppointment appointment, string scheduleresource)
        {
            if (Equals(appointment))
            {
                return false;
            }
            if (scheduleresource == string.Empty)
            {
                return InternalStartTime < appointment.InternalEndTime && appointment.InternalStartTime < InternalEndTime;
            }
            Resource localAppRes = ResourceCollection.FirstOrDefault(res => res.TypeName == scheduleresource);
            Resource currentAppRes = appointment.ResourceCollection.FirstOrDefault(res => res.TypeName == scheduleresource);
            if (localAppRes != null && currentAppRes != null && localAppRes.ResourceName == currentAppRes.ResourceName)
            {
                return InternalStartTime < appointment.InternalEndTime && appointment.InternalStartTime < InternalEndTime;
            }
            return false;
        }

        internal bool IsIntersectingInDayView(ScheduleAppointment appointment, string scheduleresource)
        {
            if (Equals(appointment))
            {
                return false;
            }
            if (scheduleresource == string.Empty)
            {
                return InternalStartTime < appointment.InternalEndTime && appointment.InternalStartTime < InternalEndTime;
            }
            Resource localAppRes = ResourceCollection.FirstOrDefault(res => res.TypeName == scheduleresource);
            Resource currentAppRes = appointment.ResourceCollection.FirstOrDefault(res => res.TypeName == scheduleresource);
            if (localAppRes != null && currentAppRes != null && localAppRes.ResourceName == currentAppRes.ResourceName)
            {
                return InternalStartTime < appointment.InternalEndTime && appointment.InternalStartTime < InternalEndTime;
            }
            return false;
        }

        #endregion

        #region MeasureTime

        internal TimeSpan MeasureTime(ReminderTimeType time)
        {
            var date = new TimeSpan();

            if (time == ReminderTimeType.ZeroMin)
            {
                date = new TimeSpan(0, 0, 0, 0);
            }
            else if (time == ReminderTimeType.FiveMin)
            {
                date = new TimeSpan(0, 0, 5, 0);
            }
            else if (time == ReminderTimeType.TenMin)
            {
                date = new TimeSpan(0, 0, 10, 0);
            }
            else if (time == ReminderTimeType.FifteenMin)
            {
                date = new TimeSpan(0, 0, 15, 0);
            }
            else if (time == ReminderTimeType.ThirtyMin)
            {
                date = new TimeSpan(0, 0, 30, 0);
            }
            else if (time == ReminderTimeType.OneHour)
            {
                date = new TimeSpan(0, 1, 0, 0);
            }
            else if (time == ReminderTimeType.TwoHours)
            {
                date = new TimeSpan(0, 2, 0, 0);
            }
            else if (time == ReminderTimeType.ThreeHours)
            {
                date = new TimeSpan(0, 3, 0, 0);
            }
            else if (time == ReminderTimeType.FourHours)
            {
                date = new TimeSpan(0, 4, 0, 0);
            }
            else if (time == ReminderTimeType.FiveHours)
            {
                date = new TimeSpan(0, 5, 0, 0);
            }
            else if (time == ReminderTimeType.SixHours)
            {
                date = new TimeSpan(0, 6, 0, 0);
            }
            else if (time == ReminderTimeType.SevenHours)
            {
                date = new TimeSpan(0, 7, 0, 0);
            }
            else if (time == ReminderTimeType.EightHours)
            {
                date = new TimeSpan(0, 8, 0, 0);
            }
            else if (time == ReminderTimeType.NineHours)
            {
                date = new TimeSpan(0, 9, 0, 0);
            }
            else if (time == ReminderTimeType.TenHours)
            {
                date = new TimeSpan(0, 10, 0, 0);
            }
            else if (time == ReminderTimeType.ElevenHours)
            {
                date = new TimeSpan(0, 11, 0, 0);
            }
            else if (time == ReminderTimeType.EighteenHours)
            {
                date = new TimeSpan(0, 18, 0, 0);
            }
            else if (time == ReminderTimeType.HalfDay)
            {
                date = new TimeSpan(0, 12, 0, 0);
            }
            else if (time == ReminderTimeType.OneDay)
            {
                date = new TimeSpan(1, 0, 0, 0);
            }
            else if (time == ReminderTimeType.TwoDays)
            {
                date = new TimeSpan(2, 0, 0, 0);
            }
            else if (time == ReminderTimeType.ThreeDays)
            {
                date = new TimeSpan(3, 0, 0, 0);
            }
            else if (time == ReminderTimeType.FourDays)
            {
                date = new TimeSpan(4, 0, 0, 0);
            }
            else if (time == ReminderTimeType.OneWeek)
            {
                date = new TimeSpan(7, 0, 0, 0);
            }
            else if (time == ReminderTimeType.TwoWeeks)
            {
                date = new TimeSpan(14, 0, 0, 0);
            }

            return date;
        }

        #endregion

        #endregion
    }

    #region ScheduleAppointmentCollection

    /// <summary>
    /// Represents a collection of Schedule appointment
    /// </summary>
    /// <seealso cref="T:Syncfusion.UI.Xaml.Schedule.ScheduleAppointment"/>
    public class ScheduleAppointmentCollection : ObservableCollection<ScheduleAppointment>
    {
    }
    #endregion

    #region ScheduleAppointmentMapping

    /// <summary>
    /// Represents a schedule appointment mapping.
    /// </summary>
    public class ScheduleAppointmentMapping : DependencyObject
    {
        #region DependencyProperties
        #region AllDayMapping

        /// <summary>
        /// Gets / Sets the AllDay mapping property on the bound object.
        /// </summary>
        public string AllDayMapping
        {
            get { return (string)GetValue(AllDayMappingProperty); }
            set { SetValue(AllDayMappingProperty, value); }
        }

        public static readonly DependencyProperty AllDayMappingProperty =
            DependencyProperty.Register("AllDayMapping", typeof(string), typeof(ScheduleAppointmentMapping),
              new PropertyMetadata(string.Empty));

        #endregion

        #region SubjectMapping

        /// <summary>
        /// Gets or sets the subject mapping.
        /// </summary>
        /// <value>The subject mapping.</value>
        public string SubjectMapping
        {
            get { return (string)GetValue(SubjectMappingProperty); }
            set { SetValue(SubjectMappingProperty, value); }
        }
        public static readonly DependencyProperty SubjectMappingProperty =
            DependencyProperty.Register("SubjectMapping", typeof(string), typeof(ScheduleAppointmentMapping),
              new PropertyMetadata(string.Empty));

        #endregion

        #region StartTimeMapping

        /// <summary>
        /// Gets or sets the start time mapping.
        /// </summary>
        /// <value>The start time mapping.</value>
        public string StartTimeMapping
        {
            get { return (string)GetValue(StartTimeMappingProperty); }
            set { SetValue(StartTimeMappingProperty, value); }
        }
        public static readonly DependencyProperty StartTimeMappingProperty =
            DependencyProperty.Register("StartTimeMapping", typeof(string), typeof(ScheduleAppointmentMapping),
              new PropertyMetadata(string.Empty));

        #endregion

        #region EndTimeMapping

        /// <summary>
        /// Gets / Sets the EndTimeMapping property for the bound object.
        /// </summary>
        public string EndTimeMapping
        {
            get { return (string)GetValue(EndTimeMappingProperty); }
            set { SetValue(EndTimeMappingProperty, value); }
        }
        public static readonly DependencyProperty EndTimeMappingProperty =
            DependencyProperty.Register("EndTimeMapping", typeof(string), typeof(ScheduleAppointmentMapping),
              new PropertyMetadata(string.Empty));

        #endregion

        #region LocationMapping

        /// <summary>
        /// Gets / Sets the Location mapping property for the bound object.
        /// </summary>
        public string LocationMapping
        {
            get { return (string)GetValue(LocationMappingProperty); }
            set { SetValue(LocationMappingProperty, value); }
        }
        public static readonly DependencyProperty LocationMappingProperty =
            DependencyProperty.Register("LocationMapping", typeof(string), typeof(ScheduleAppointmentMapping),
              new PropertyMetadata(string.Empty));

        #endregion

        #region NotesMapping

        /// <summary>
        /// Gets / Sets the AllDay mapping property on the bound object.
        /// </summary>
        public string NotesMapping
        {
            get { return (string)GetValue(NotesMappingProperty); }
            set { SetValue(NotesMappingProperty, value); }
        }

        public static readonly DependencyProperty NotesMappingProperty =
            DependencyProperty.Register("NotesMapping", typeof(string), typeof(ScheduleAppointmentMapping),
              new PropertyMetadata(string.Empty));

        #endregion

        #region StatusMapping

        /// <summary>
        /// Gets / Sets the AllDay mapping property on the bound object.
        /// </summary>
        public string StatusMapping
        {
            get { return (string)GetValue(StatusMappingProperty); }
            set { SetValue(StatusMappingProperty, value); }
        }

        public static readonly DependencyProperty StatusMappingProperty =
            DependencyProperty.Register("StatusMapping", typeof(string), typeof(ScheduleAppointmentMapping),
              new PropertyMetadata(string.Empty));

        #endregion

        #region AppointmentBackgroundMapping

        /// <summary>
        /// Gets / Sets the AllDay mapping property on the bound object.
        /// </summary>
        public string AppointmentBackgroundMapping
        {
            get { return (string)GetValue(AppointmentBackgroundMappingProperty); }
            set { SetValue(AppointmentBackgroundMappingProperty, value); }
        }

        public static readonly DependencyProperty AppointmentBackgroundMappingProperty =
            DependencyProperty.Register("AppointmentBackgroundMapping", typeof(string), typeof(ScheduleAppointmentMapping),
              new PropertyMetadata(string.Empty));

        #endregion

        #region ReminderTimeMapping



        public string ReminderTimeMapping
        {
            get { return (string)GetValue(ReminderTimeMappingProperty); }
            set { SetValue(ReminderTimeMappingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ReminderTimeMapping.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ReminderTimeMappingProperty =
            DependencyProperty.Register("ReminderTimeMapping", typeof(string), typeof(ScheduleAppointmentMapping), new PropertyMetadata(string.Empty));



        #endregion

        #region RecurrenceTypeMapping


        public string RecurrenceTypeMapping
        {
            get { return (string)GetValue(RecurrenceTypeMappingProperty); }
            set { SetValue(RecurrenceTypeMappingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RecurrenceTypeMapping.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RecurrenceTypeMappingProperty =
            DependencyProperty.Register("RecurrenceTypeMapping", typeof(string), typeof(ScheduleAppointmentMapping), new PropertyMetadata(string.Empty));



        #endregion

        #region RecurrenceRuleMapping


        public string RecurrenceRuleMapping
        {
            get { return (string)GetValue(RecurrenceRuleMappingProperty); }
            set { SetValue(RecurrenceRuleMappingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RecurrenceRule.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RecurrenceRuleMappingProperty =
            DependencyProperty.Register("RecurrenceRuleMapping", typeof(string), typeof(ScheduleAppointmentMapping), new PropertyMetadata(string.Empty));


        #endregion

        #region RecurrenceProperitesMapping
        public string RecurrenceProperitesMapping
        {
            get { return (string)GetValue(RecurrenceProperitesMappingProperty); }
            set { SetValue(RecurrenceProperitesMappingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RecurrenceProperites.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RecurrenceProperitesMappingProperty =
            DependencyProperty.Register("RecurrenceProperitesMapping", typeof(string), typeof(ScheduleAppointmentMapping), new PropertyMetadata(string.Empty));

        #endregion

        #region StartTimeZoneMapping



        public string StartTimeZoneMapping
        {
            get { return (string)GetValue(StartTimeZoneMappingProperty); }
            set { SetValue(StartTimeZoneMappingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartTimeZone.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartTimeZoneMappingProperty =
            DependencyProperty.Register("StartTimeZoneMapping", typeof(string), typeof(ScheduleAppointmentMapping), new PropertyMetadata(string.Empty));


        #endregion

        #region EndTimeZoneMapping

        public string EndTimeZoneMapping
        {
            get { return (string)GetValue(EndTimeZoneMappingProperty); }
            set { SetValue(EndTimeZoneMappingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EndTimeZone.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndTimeZoneMappingProperty =
            DependencyProperty.Register("EndTimeZoneMapping", typeof(string), typeof(ScheduleAppointmentMapping), new PropertyMetadata(string.Empty));

        #endregion

        #region IsRecursiveMapping



        public string IsRecursiveMapping
        {
            get { return (string)GetValue(IsRecursiveMappingProperty); }
            set { SetValue(IsRecursiveMappingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsRecursive.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty IsRecursiveMappingProperty =
            DependencyProperty.Register("IsRecursiveMapping", typeof(string), typeof(ScheduleAppointmentMapping), new PropertyMetadata(string.Empty));



        #endregion

        #region ReadOnlyMapping

        public string ReadOnlyMapping
        {
            get { return (string)GetValue(ReadOnlyMappingProperty); }
            set { SetValue(ReadOnlyMappingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ReadOnly.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ReadOnlyMappingProperty =
            DependencyProperty.Register("ReadOnlyMapping", typeof(string), typeof(ScheduleAppointmentMapping), new PropertyMetadata(string.Empty));

        #endregion

        #region ResourceCollectionMapping


        public string ResourceCollectionMapping
        {
            get { return (string)GetValue(ResourceCollectionMappingProperty); }
            set { SetValue(ResourceCollectionMappingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Resources.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ResourceCollectionMappingProperty =
            DependencyProperty.Register("ResourceCollectionMapping", typeof(string), typeof(ScheduleAppointmentMapping), new PropertyMetadata(string.Empty));



        #endregion

        #endregion
    }
    #endregion
}
