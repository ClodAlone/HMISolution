#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Syncfusion.UI.Xaml.Schedule
{
    /// <summary>
    /// Represents an appointment editor window.
    /// </summary>
    public partial class AppointmentEditor
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.UI.Xaml.Schedule.AppointmentEditor">AppointmentEditor</see> class. 
        /// </summary>
        public AppointmentEditor()
        {
            InitializeComponent();
            DataContext = this;
            Startdate.SelectedDate = DateTime.Now;
            Reminder.ItemsSource = GetLocalizedArrayValues(Enum.GetValues(typeof(ReminderTimeType)));
            EveryXDays.IsChecked = true;
            sunday.IsChecked = true;
            EveryMonthDay.SelectedIndex = 0;
            Howmanyth.SelectedIndex = 0;
            Weekday.SelectedIndex = 0;
            YrSpMonth.SelectedIndex = 0;
            YrSpDate.SelectedIndex = 0;
            YrHowmanyth.SelectedIndex = 0;
            YrWeekday.SelectedIndex = 0;
            YrMonth.SelectedIndex = 0;
            EndAfter.IsChecked = true;
        }

        #endregion

        #region Internal Fields

        internal int recursiveModified;
        internal bool isNew;

        #endregion

        #region Dependency Properties

        #region ScheduleResourceTypeCollection
        /// <summary>
        /// Gets or sets the collection of resource type.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Schedule.ResourceType"></seealso>
        public ObservableCollection<ResourceType> ScheduleResourceTypeCollection
        {
            get { return (ObservableCollection<ResourceType>)GetValue(ScheduleResourceTypeCollectionProperty); }
            set { SetValue(ScheduleResourceTypeCollectionProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ScheduleResourceTypeCollection.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ScheduleResourceTypeCollectionProperty =
            DependencyProperty.Register("ScheduleResourceTypeCollection", typeof(ObservableCollection<ResourceType>), typeof(AppointmentEditor), new PropertyMetadata(null));

        #endregion

        #region start
        /// <summary>
        /// Gets or sets the start time value of appointment editor.
        /// </summary>
        public DateTime start
        {
            get { return (DateTime)GetValue(startProperty); }
            set { SetValue(startProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for start.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty startProperty =
            DependencyProperty.Register("start", typeof(DateTime), typeof(AppointmentEditor), new PropertyMetadata(DateTime.Today, OnStartTimeChanged));

        private static void OnStartTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                var scheduleAppointmentEditor = d as AppointmentEditor;
                if (scheduleAppointmentEditor != null)
                    scheduleAppointmentEditor.InternalStartTime = scheduleAppointmentEditor.start.Date.Add(scheduleAppointmentEditor.start1.TimeOfDay);

            }
        }

        internal DateTime start1
        {
            get { return (DateTime)GetValue(start1Property); }
            set { SetValue(start1Property, value); }
        }

        // Using a DependencyProperty as the backing store for start.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty start1Property =
            DependencyProperty.Register("start1", typeof(DateTime), typeof(AppointmentEditor), new PropertyMetadata(DateTime.Today));

        #endregion

        #region end
        /// <summary>
        /// Gets or sets the end time value of appointment editor.
        /// </summary>
        public DateTime end
        {
            get { return (DateTime)GetValue(endProperty); }
            set { SetValue(endProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for end.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty endProperty =
            DependencyProperty.Register("end", typeof(DateTime), typeof(AppointmentEditor), new PropertyMetadata(DateTime.Today, OnEndTimeChanged));

        private static void OnEndTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                var scheduleAppointmentEditor = d as AppointmentEditor;
                if (scheduleAppointmentEditor != null)
                    scheduleAppointmentEditor.InternalEndTime = scheduleAppointmentEditor.end.Date.Add(scheduleAppointmentEditor.end1.TimeOfDay);
            }
        }

        public DateTime end1
        {
            get { return (DateTime)GetValue(end1Property); }
            set { SetValue(end1Property, value); }
        }

        // Using a DependencyProperty as the backing store for end.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty end1Property =
            DependencyProperty.Register("end1", typeof(DateTime), typeof(AppointmentEditor), new PropertyMetadata(DateTime.Today));

        #endregion

        #region ReminderTime
        /// <summary>
        /// Gets or sets the reminder time for appointment using appointment editor.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Schedule.ReminderTimeType"></seealso>
        public ReminderTimeType ReminderTime
        {
            get { return (ReminderTimeType)GetValue(ReminderTimeIntervalProperty); }
            set
            {
                SetValue(ReminderTimeIntervalProperty, value);
                OnReminderTimeChanged(this);
            }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ReminderTime.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ReminderTimeIntervalProperty =
            DependencyProperty.Register("ReminderTime", typeof(ReminderTimeType), typeof(AppointmentEditor), new PropertyMetadata(ReminderTimeType.None));

        private void OnReminderTimeChanged(DependencyObject d)
        {
            var scheduleAppointmentEditor = d as AppointmentEditor;
            if (scheduleAppointmentEditor != null && scheduleAppointmentEditor.Reminder != null)
                for (int i = 0; i < scheduleAppointmentEditor.Reminder.Items.Count; i++)
                {
                    scheduleAppointmentEditor.Reminder.SelectedIndex = i;
                    if (scheduleAppointmentEditor.Reminder.SelectedValue != null && scheduleAppointmentEditor.Reminder.SelectedValue.ToString() == scheduleAppointmentEditor.ReminderTime.ToString())
                    {
                        scheduleAppointmentEditor.Reminder.SelectedIndex = i;
                        break;
                    }
                }
        }
        #endregion

        #region EndDateEnable
        /// <summary>
        /// Gets or sets a value indicating whether the end date should be enabled.
        /// </summary>
        public bool EndDateEnable
        {
            get { return (bool)GetValue(EndDateEnableProperty); }
            set { SetValue(EndDateEnableProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for EndDateEnable.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EndDateEnableProperty =
            DependencyProperty.Register("EndDateEnable", typeof(bool), typeof(AppointmentEditor), new PropertyMetadata(true));

        #endregion

        #region AppointmentResourceName
        /// <summary>
        /// Gets or sets the resource name for appointment using appointment editor.
        /// </summary>
        public List<Resource> AppointmentResourceName
        {
            get { return (List<Resource>)GetValue(AppointmentResourceNameProperty); }
            set { SetValue(AppointmentResourceNameProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AppointmentResourceName.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AppointmentResourceNameProperty =
            DependencyProperty.Register("AppointmentResourceName", typeof(List<Resource>), typeof(AppointmentEditor), new PropertyMetadata(null));
        #endregion

        #region AllDay
        /// <summary>
        /// Gets or sets a value indicating whether the appointment should be all day appointment using appointment editor.
        /// </summary>
        public bool AllDay
        {
            get { return (bool)GetValue(AllDayProperty); }
            set { SetValue(AllDayProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AllDay.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AllDayProperty =
            DependencyProperty.Register("AllDay", typeof(bool), typeof(AppointmentEditor), new PropertyMetadata(false, OnAllDayPropertyChanged));

        private static void OnAllDayPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var AppEditor = d as AppointmentEditor;

            if (AppEditor != null)
            {
                if ((bool)e.NewValue)
                    AppEditor.EndDateEnable = false;
                else
                    AppEditor.EndDateEnable = true;
            }
        }

        #endregion

        #region AppointmentEditorBackground
        /// <summary>
        /// Gets or sets the background color for appointment editor.
        /// </summary>
        public Brush AppointmentEditorBackground
        {
            get { return (Brush)GetValue(AppointmentEditorBackgroundProperty); }
            set { SetValue(AppointmentEditorBackgroundProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AppointmentEditorBackground.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AppointmentEditorBackgroundProperty =
            DependencyProperty.Register("AppointmentEditorBackground", typeof(Brush), typeof(AppointmentEditor), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x3A, 0x3A, 0x3A))));
        #endregion

        #region StartAccordionBackground
        /// <summary>
        /// Gets or sets the background color for accordion which specifies start time.
        /// </summary>
        public Brush StartAccordionBackground
        {
            get { return (Brush)GetValue(StartAccordionBackgroundProperty); }
            set { SetValue(StartAccordionBackgroundProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AccordionBackground.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StartAccordionBackgroundProperty =
            DependencyProperty.Register("StartAccordionBackground", typeof(Brush), typeof(AppointmentEditor), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x3A, 0x3A, 0x3A))));
        #endregion

        #region EndAccordionBackground
        /// <summary>
        /// Gets or sets the background color for accordion which specifies end time.
        /// </summary>
        public Brush EndAccordionBackground
        {
            get { return (Brush)GetValue(EndAccordionBackgroundProperty); }
            set { SetValue(EndAccordionBackgroundProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AccordionBackground.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EndAccordionBackgroundProperty =
            DependencyProperty.Register("EndAccordionBackground", typeof(Brush), typeof(AppointmentEditor), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x3A, 0x3A, 0x3A))));

        #endregion

        #region RecurrenceAccordionBackground
        /// <summary>
        /// Gets or sets the background color for accordion which specifies recurrence properties.
        /// </summary>
        public Brush RecurrenceAccordionBackground
        {
            get { return (Brush)GetValue(RecurrenceAccordionBackgroundProperty); }
            set { SetValue(RecurrenceAccordionBackgroundProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AccordionBackground.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RecurrenceAccordionBackgroundProperty =
            DependencyProperty.Register("RecurrenceAccordionBackground", typeof(Brush), typeof(AppointmentEditor), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x3A, 0x3A, 0x3A))));

        #endregion

        #region AppointmentEditorForeground
        /// <summary>
        /// Gets or sets the foreground for appointment editor.
        /// </summary>
        public Brush AppointmentEditorForeground
        {
            get { return (Brush)GetValue(AppointmentEditorForegroundProperty); }
            set { SetValue(AppointmentEditorForegroundProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AppointmentEditorForeground.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AppointmentEditorForegroundProperty =
            DependencyProperty.Register("AppointmentEditorForeground", typeof(Brush), typeof(AppointmentEditor), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0xCE, 0xCE, 0xCE))));
        #endregion

        #region ShowMoreVisibility
        /// <summary>
        /// Gets or sets the visibility of show more panel in appointment editor.
        /// </summary>
        public Visibility ShowMoreVisibility
        {
            get { return (Visibility)GetValue(ShowMoreVisibilityProperty); }
            set { SetValue(ShowMoreVisibilityProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowMoreVisibility.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowMoreVisibilityProperty =
            DependencyProperty.Register("ShowMoreVisibility", typeof(Visibility), typeof(AppointmentEditor), new PropertyMetadata(Visibility.Visible));
        #endregion

        #region IsWarningMsgVisible
        /// <summary>
        /// Gets or sets the visibility of warning message in appointment editor.
        /// </summary>
        public Visibility IsWarningMsgVisible
        {
            get { return (Visibility)GetValue(IsWarningMsgVisibleProperty); }
            set { SetValue(IsWarningMsgVisibleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsWarningMsgVisible.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsWarningMsgVisibleProperty =
            DependencyProperty.Register("IsWarningMsgVisible", typeof(Visibility), typeof(AppointmentEditor), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #region IsRecursive
        /// <summary>
        /// Gets or sets a value indicating whether the appointment is recurring appointment.
        /// </summary>
        public bool IsRecursive
        {
            get { return (bool)GetValue(IsRecursiveProperty); }
            set { SetValue(IsRecursiveProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsRecursive.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsRecursiveProperty =
            DependencyProperty.Register("IsRecursive", typeof(bool), typeof(AppointmentEditor), new PropertyMetadata(false));
        #endregion

        #region IsRecurrenceTypeEnabled
        /// <summary>
        /// Gets or sets a value indicating whether the recurrence properties for appointment should be enabled in editor.
        /// </summary>
        public bool IsRecurrenceTypeEnabled
        {
            get { return (bool)GetValue(IsRecurrenceTypeEnabledProperty); }
            set { SetValue(IsRecurrenceTypeEnabledProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RecurrenceTypeVisibility.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsRecurrenceTypeEnabledProperty =
            DependencyProperty.Register("IsRecurrenceTypeEnabled", typeof(bool), typeof(AppointmentEditor), new PropertyMetadata(true));
        #endregion

        #region DeleteButtonVisibility
        /// <summary>
        /// Gets or sets the visibility of delete button in appointment editor.
        /// </summary>
        public Visibility DeleteButtonVisibility
        {
            get { return (Visibility)GetValue(DeleteButtonVisibilityProperty); }
            set { SetValue(DeleteButtonVisibilityProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DeleteButtonVisibility.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DeleteButtonVisibilityProperty =
            DependencyProperty.Register("DeleteButtonVisibility", typeof(Visibility), typeof(AppointmentEditor), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #region AppointmentStatusCollection
        /// <summary>
        /// Gets or sets the collection of appointment status in appointment editor.
        /// </summary>
        public ScheduleAppointmentStatusCollection AppointmentStatusCollection
        {
            get { return (ScheduleAppointmentStatusCollection)GetValue(AppointmentStatusCollectionProperty); }
            set
            {
                SetValue(AppointmentStatusCollectionProperty, value);
                Status.ItemsSource = GetLocalizedArrayValues(AppointmentStatusCollection.Select(appointmentStatus => appointmentStatus.Status).ToArray());
            }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AppointmentStatusCollection.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AppointmentStatusCollectionProperty =
            DependencyProperty.Register("AppointmentStatusCollection", typeof(ScheduleAppointmentStatusCollection), typeof(AppointmentEditor), new PropertyMetadata(null));
        #endregion

        #region StartTimeZone
        /// <summary>
        /// Gets or sets the time zone for start time value in appointment editor.
        /// </summary>
        public TimeZone StartTimeZone
        {
            get { return (TimeZone)GetValue(StartTimeZoneProperty); }
            set { SetValue(StartTimeZoneProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for StartTimeZone.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StartTimeZoneProperty =
            DependencyProperty.Register("StartTimeZone", typeof(TimeZone), typeof(AppointmentEditor), new PropertyMetadata(null, OnStartTimeZoneChanged));

        private static void OnStartTimeZoneChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var Editor = obj as AppointmentEditor;
            if (Editor != null && Editor.TimeZoneCollection != null)
            {
                if (Editor.Starttimezone != null)
                {
                    if (Editor.StartTimeZone != null)
                        Editor.Starttimezone.SelectedItem = Editor.StartTimeZone;
                    else
                        Editor.Starttimezone.SelectedIndex = 10;
                }
            }
        }
        #endregion

        #region EndTimeZone
        /// <summary>
        /// Gets or sets the time zone for end time value in appointment editor.
        /// </summary>
        public TimeZone EndTimeZone
        {
            get { return (TimeZone)GetValue(EndTimeZoneProperty); }
            set { SetValue(EndTimeZoneProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for EndTimeZone.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EndTimeZoneProperty =
            DependencyProperty.Register("EndTimeZone", typeof(TimeZone), typeof(AppointmentEditor), new PropertyMetadata(null));

        #endregion

        #region TimeZoneCollection
        /// <summary>
        /// Gets or sets the collection of time zones in appointment editor.
        /// </summary>
        public TimeZoneCollection TimeZoneCollection
        {
            get { return (TimeZoneCollection)GetValue(TimeZoneCollectionProperty); }
            internal set
            {
                SetValue(TimeZoneCollectionProperty, value);
                Starttimezone.ItemsSource = value;
                Endtimezone.ItemsSource = value;
            }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TimeZoneCollection.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TimeZoneCollectionProperty =
            DependencyProperty.Register("TimeZoneCollection", typeof(TimeZoneCollection), typeof(AppointmentEditor), new PropertyMetadata(null));
        #endregion

        #region RecurrenceType
        /// <summary>
        /// Gets or sets the type of recurrence.
        /// </summary>
        public RecurrenceType RecurrenceType
        {
            get { return (RecurrenceType)GetValue(RecurrenceTypeProperty); }
            set { SetValue(RecurrenceTypeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RecurrenceType.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RecurrenceTypeProperty =
            DependencyProperty.Register("RecurrenceType", typeof(RecurrenceType), typeof(AppointmentEditor), new PropertyMetadata(null));
        #endregion

        #region EditedAppointment
        /// <summary>
        /// Gets or sets the appointment that is edited.
        /// </summary>
        public ScheduleAppointment EditedAppointment
        {
            get { return (ScheduleAppointment)GetValue(EditedAppointmentProperty); }
            set { SetValue(EditedAppointmentProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for EditedAppointment.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EditedAppointmentProperty =
            DependencyProperty.Register("EditedAppointment", typeof(ScheduleAppointment), typeof(AppointmentEditor), new PropertyMetadata(null));
        #endregion

        #region IsRecursiveModified
        /// <summary>
        /// Gets or sets a value indicating whether the option IsRecursive has been modified or not.
        /// </summary>
        public bool IsRecursiveModified
        {
            get { return (bool)GetValue(IsRecursiveModifiedProperty); }
            set { SetValue(IsRecursiveModifiedProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsRecursiveModified.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsRecursiveModifiedProperty =
            DependencyProperty.Register("IsRecursiveModified", typeof(bool), typeof(AppointmentEditor), new PropertyMetadata(false));

        #endregion

        #region RecurrenceDaily
        internal bool RecurrenceDaily
        {
            get { return (bool)GetValue(RecurrenceDailyProperty); }
            set { SetValue(RecurrenceDailyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RecurrenceOptionDaily.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty RecurrenceDailyProperty =
            DependencyProperty.Register("RecurrenceDaily", typeof(bool), typeof(AppointmentEditor), new PropertyMetadata(true));
        #endregion

        #region RecurrenceWeekly
        internal bool RecurrenceWeekly
        {
            get { return (bool)GetValue(RecurrenceWeeklyProperty); }
            set { SetValue(RecurrenceWeeklyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RecurrenceOptionWeekly.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty RecurrenceWeeklyProperty =
            DependencyProperty.Register("RecurrenceWeekly", typeof(bool), typeof(AppointmentEditor), new PropertyMetadata(false));
        #endregion

        #region RecurrenceMonthly
        internal bool RecurrenceMonthly
        {
            get { return (bool)GetValue(RecurrenceMonthlyProperty); }
            set { SetValue(RecurrenceMonthlyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RecurrenceMonthly.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty RecurrenceMonthlyProperty =
            DependencyProperty.Register("RecurrenceMonthly", typeof(bool), typeof(AppointmentEditor), new PropertyMetadata(false));
        #endregion

        #region RecurrenceYearly
        internal bool RecurrenceYearly
        {
            get { return (bool)GetValue(RecurrenceYearlyProperty); }
            set { SetValue(RecurrenceYearlyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RecurrenceYearly.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty RecurrenceYearlyProperty =
            DependencyProperty.Register("RecurrenceYearly", typeof(bool), typeof(AppointmentEditor), new PropertyMetadata(false));
        #endregion

        #region InternalStartTime
        internal DateTime InternalStartTime
        {
            get { return ScheduleAppointment.ConvertToLocalTime((DateTime)GetValue(InternalStartTimeProperty), "StartTime", StartTimeZone, null); }
            set { SetValue(InternalStartTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartTime.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty InternalStartTimeProperty =
            DependencyProperty.Register("InternalStartTime", typeof(DateTime), typeof(AppointmentEditor), new PropertyMetadata(DateTime.Today));
        #endregion

        #region InternalEndTime
        internal DateTime InternalEndTime
        {
            get { return ScheduleAppointment.ConvertToLocalTime((DateTime)GetValue(InternalEndTimeProperty), "EndTime", null, EndTimeZone); }
            set { SetValue(InternalEndTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EndTime.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty InternalEndTimeProperty =
            DependencyProperty.Register("InternalEndTime", typeof(DateTime), typeof(AppointmentEditor), new PropertyMetadata(DateTime.Today));
        #endregion

        #region ReminderDeliveryTime
        internal DateTime ReminderDeliveryTime
        {
            get { return (DateTime)GetValue(ReminderTimeProperty); }
            set { SetValue(ReminderTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ReminderDeliveryTime.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ReminderTimeProperty =
            DependencyProperty.Register("ReminderDeliveryTime", typeof(DateTime), typeof(AppointmentEditor), new PropertyMetadata(DateTime.Now));
        #endregion

        #region ShowMoreStackPanelVisibility
        internal Visibility ShowMoreStackPanelVisibility
        {
            get { return (Visibility)GetValue(ShowMoreStackPanelVisibilityProperty); }
            set { SetValue(ShowMoreStackPanelVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowMoreStackPanelVisibility.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ShowMoreStackPanelVisibilityProperty =
            DependencyProperty.Register("ShowMoreStackPanelVisibility", typeof(Visibility), typeof(AppointmentEditor), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #region StartStackPanelVisibility
        internal Visibility StartStackPanelVisibility
        {
            get { return (Visibility)GetValue(StartStackPanelVisibilityProperty); }
            set { SetValue(StartStackPanelVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartStackPanelVisibility.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty StartStackPanelVisibilityProperty =
            DependencyProperty.Register("StartStackPanelVisibility", typeof(Visibility), typeof(AppointmentEditor), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #region EndStackPanelVisibility
        internal Visibility EndStackPanelVisibility
        {
            get { return (Visibility)GetValue(EndStackPanelVisibilityProperty); }
            set { SetValue(EndStackPanelVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EndStackPanelVisibility.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty EndStackPanelVisibilityProperty =
            DependencyProperty.Register("EndStackPanelVisibility", typeof(Visibility), typeof(AppointmentEditor), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #region RecursiveStockPanelVisibility
        internal Visibility RecursiveStockPanelVisibility
        {
            get { return (Visibility)GetValue(RecursiveStockPanelVisibilityProperty); }
            set { SetValue(RecursiveStockPanelVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RecursiveStockPanelVisibility.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty RecursiveStockPanelVisibilityProperty =
            DependencyProperty.Register("RecursiveStockPanelVisibility", typeof(Visibility), typeof(AppointmentEditor), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #region DailyStackPanelVisibility
        internal Visibility DailyStackPanelVisibility
        {
            get { return (Visibility)GetValue(DailyStackPanelVisibilityProperty); }
            set { SetValue(DailyStackPanelVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DailyStackPanelVisibility.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty DailyStackPanelVisibilityProperty =
            DependencyProperty.Register("DailyStackPanelVisibility", typeof(Visibility), typeof(AppointmentEditor), new PropertyMetadata(Visibility.Visible));
        #endregion

        #region WeeklyStackPanelVisibility
        internal Visibility WeeklyStackPanelVisibility
        {
            get { return (Visibility)GetValue(WeeklyStackPanelVisibilityProperty); }
            set { SetValue(WeeklyStackPanelVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WeeklyStackPanelVisibility.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty WeeklyStackPanelVisibilityProperty =
            DependencyProperty.Register("WeeklyStackPanelVisibility", typeof(Visibility), typeof(AppointmentEditor), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #region MonthlyStackPanelVisibility
        internal Visibility MonthlyStackPanelVisibility
        {
            get { return (Visibility)GetValue(MonthlyStackPanelVisibilityProperty); }
            set { SetValue(MonthlyStackPanelVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MonthlyStackPanelVisibility.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty MonthlyStackPanelVisibilityProperty =
            DependencyProperty.Register("MonthlyStackPanelVisibility", typeof(Visibility), typeof(AppointmentEditor), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #region YearlyStackPanelVisibility
        internal Visibility YearlyStackPanelVisibility
        {
            get { return (Visibility)GetValue(YearlyStackPanelVisibilityProperty); }
            set { SetValue(YearlyStackPanelVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for YearlyStackPanelVisibility.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty YearlyStackPanelVisibilityProperty =
            DependencyProperty.Register("YearlyStackPanelVisibility", typeof(Visibility), typeof(AppointmentEditor), new PropertyMetadata(Visibility.Collapsed));
        #endregion
        
        #endregion

        #region Methods

        #region Setting New Appointment Properties

        internal void SetNewAppointmentProperties(SfSchedule sfSchedule, DateTime startDate, DateTime endDate)
        {
            isNew = true;
            Subject.Focus();          

            var newApp = new ScheduleAppointment { StartTime = startDate, EndTime = endDate, ReadOnly = false };

            start = sfSchedule.allDayFlag ? startDate.Date : startDate;
            start1 = sfSchedule.allDayFlag ? startDate.Date : startDate;
            end = sfSchedule.allDayFlag ? startDate.Date : endDate;
            end1 = sfSchedule.allDayFlag ? startDate.Date : endDate;
            if (sfSchedule.ScheduleType == ScheduleType.Month || sfSchedule.allDayFlag || sfSchedule.allDayTouch)
            {
                newApp.AllDay = true;
                AllDay = true;
                sfSchedule.allDayFlag = false;
                sfSchedule.allDayTouch = false;
            }
            else
                newApp.AllDay = false;
            if (Starttimezone != null)
            {
                TimeZone tmzo = sfSchedule.TimeZoneCollection.FirstOrDefault(tz => tz.TimeZoneValue == newApp.StartTimeZone.TimeZoneValue);
                Starttimezone.SelectedIndex = sfSchedule.TimeZoneCollection.IndexOf(tmzo);
            }
            if (Endtimezone != null)
            {
                TimeZone tmzo = sfSchedule.TimeZoneCollection.FirstOrDefault(tz => tz.TimeZoneValue == newApp.EndTimeZone.TimeZoneValue);
                Endtimezone.SelectedIndex = sfSchedule.TimeZoneCollection.IndexOf(tmzo);
            }
            Delete.Visibility = Visibility.Collapsed;
            AppointmentStatusCollection = sfSchedule.AppointmentStatusCollection;
            TimeZoneCollection = sfSchedule.TimeZoneCollection;
            StartTimeZone = new TimeZone { TimeZoneValue = TimeZoneInfo.Local.ToString() };
            EndTimeZone = new TimeZone { TimeZoneValue = TimeZoneInfo.Local.ToString() };
            IsRecurrenceTypeEnabled = true;
            RecurrenceType = RecurrenceType.Daily;
            DeleteButtonVisibility = Visibility.Collapsed;
            IsWarningMsgVisible = Visibility.Collapsed;
            ShowMoreVisibility = Visibility.Visible;
            ShowMoreStackPanelVisibility = Visibility.Collapsed;
            EditedAppointment = newApp;
            AppointmentResourceName = sfSchedule.selectedResourcename;
            InternalStartTime = startDate;
            InternalEndTime = endDate;
            Reminder.SelectedIndex = 0;
            Status.SelectedIndex = 0;
            var args = new AppointmentEditorOpeningEventArgs
            {
                StartTime = sfSchedule.Currentselecteddate,
                Action = EditorAction.Add,
                SelectedResource = sfSchedule.selectedResourcename
            };
            sfSchedule.GetAppointmentWindowOpeningEvents(args);
        }

        #endregion

        #region Updating Appointment Properties

        internal void UpdateAppointmentProperties(SfSchedule sfSchedule, ScheduleAppointment appointment)
        {
            isNew = false;
            Subject.Focus();        

            EditedAppointment = appointment;
            IsRecursiveModified = false;
            IsRecurrenceTypeEnabled = true;
            if (recursiveModified == 1)
            {
                IsRecursiveModified = true;
                IsRecurrenceTypeEnabled = false;
            }
            if (appointment.RecurrenceRule != "")
            {
                if (appointment.RecurrenceProperites == null)
                {
                    RecurrenceProperties RecProp = ScheduleHelper.RRuleParser(appointment.RecurrenceRule, appointment.StartTime);
                    appointment.RecurrenceProperites = RecProp;
                }
            }
            if (Subject != null)
                Subject.Text = appointment.Subject;
            if (Notes != null)
                Notes.Text = appointment.Notes;
            if (Location != null)
                Location.Text = appointment.Location;
            IsRecurrenceTypeEnabled = appointment.IsRecursive;

            if (Starttimezone != null)
            {
                TimeZone tmzo = sfSchedule.TimeZoneCollection.FirstOrDefault(tz => tz.TimeZoneValue == appointment.StartTimeZone.TimeZoneValue);
                Starttimezone.SelectedIndex = sfSchedule.TimeZoneCollection.IndexOf(tmzo);
            }
            if (Endtimezone != null)
            {
                TimeZone tmzo = sfSchedule.TimeZoneCollection.FirstOrDefault(tz => tz.TimeZoneValue == appointment.EndTimeZone.TimeZoneValue);
                Endtimezone.SelectedIndex = sfSchedule.TimeZoneCollection.IndexOf(tmzo);
            }
            if (appointment.RecurrenceProperites != null)
            {
                if (appointment.RecurrenceProperites.RecurrenceType == RecurrenceType.Daily)
                {
                    RecurrenceDaily = true;
                    if (EveryXDays != null)
                        EveryXDays.IsChecked = appointment.RecurrenceProperites.IsDailyEveryNDays;
                    if (EveryDayGap != null)
                        EveryDayGap.Value = appointment.RecurrenceProperites.DailyNDays;
                }
                else if (appointment.RecurrenceProperites.RecurrenceType == RecurrenceType.Weekly)
                {
                    RecurrenceWeekly = true;

                    if (EveryWeekGap != null)
                        EveryWeekGap.Value = appointment.RecurrenceProperites.WeeklyEveryNWeeks;
                    if (sunday != null)
                        sunday.IsChecked = appointment.RecurrenceProperites.IsWeeklySunday;
                    if (monday != null)
                        monday.IsChecked = appointment.RecurrenceProperites.IsWeeklyMonday;
                    if (tuesday != null)
                        tuesday.IsChecked = appointment.RecurrenceProperites.IsWeeklyTuesday;
                    if (wednesday != null)
                        wednesday.IsChecked = appointment.RecurrenceProperites.IsWeeklyWednesday;
                    if (thursday != null)
                        thursday.IsChecked = appointment.RecurrenceProperites.IsWeeklyThursday;
                    if (friday != null)
                        friday.IsChecked = appointment.RecurrenceProperites.IsWeeklyFriday;
                    if (saturday != null)
                        saturday.IsChecked = appointment.RecurrenceProperites.IsWeeklySaturday;

                }
                else if (appointment.RecurrenceProperites.RecurrenceType == RecurrenceType.Monthly)
                {
                    RecurrenceMonthly = true;
                    if (EveryXMonths != null)
                        EveryXMonths.IsChecked = appointment.RecurrenceProperites.IsMonthlySpecific;
                    if (EveryMonthGap != null)
                        EveryMonthGap.Value = appointment.RecurrenceProperites.MonthlyEveryNMonths;
                    if (EveryMonthDay != null)
                        EveryMonthDay.SelectedIndex = appointment.RecurrenceProperites.MonthlySpecificMonthDay - 1 >= 0 ? appointment.RecurrenceProperites.MonthlySpecificMonthDay - 1 : 0;
                    if (Howmanyth != null)
                        Howmanyth.SelectedIndex = appointment.RecurrenceProperites.MonthlyNthWeek - 1 >= 0 ? appointment.RecurrenceProperites.MonthlyNthWeek - 1 : 0;
                    if (Weekday != null)
                        Weekday.SelectedIndex = appointment.RecurrenceProperites.MonthlyWeekDay - 1;
                }
                else if (appointment.RecurrenceProperites.RecurrenceType == RecurrenceType.Yearly)
                {
                    RecurrenceYearly = true;
                    if (SpecificDate != null)
                        SpecificDate.IsChecked = appointment.RecurrenceProperites.IsYearlySpecific;
                    if (EveryYearGap != null)
                        EveryYearGap.Value = appointment.RecurrenceProperites.YearlyEveryNYears;
                    if (YrSpDate != null)
                        YrSpDate.SelectedIndex = appointment.RecurrenceProperites.YearlySpecificMonthDay - 1 >= 0 ? appointment.RecurrenceProperites.YearlySpecificMonthDay - 1 : 0;
                    if (YrSpMonth != null)
                        YrSpMonth.SelectedIndex = appointment.RecurrenceProperites.YearlySpecificMonth - 1 >= 0 ? appointment.RecurrenceProperites.YearlySpecificMonth - 1 : 0;
                    if (YrHowmanyth != null)
                        YrHowmanyth.SelectedIndex = appointment.RecurrenceProperites.YearlyNthWeek - 1 >= 0 ? appointment.RecurrenceProperites.YearlyNthWeek - 1 : 0;
                    if (YrWeekday != null)
                        YrWeekday.SelectedIndex = appointment.RecurrenceProperites.YearlyWeekDay - 1;
                    if (YrMonth != null)
                        YrMonth.SelectedIndex = appointment.RecurrenceProperites.YearlyGenericMonth - 1 >= 0 ? appointment.RecurrenceProperites.YearlyGenericMonth - 1 : 0;
                }

                RecurrenceType = appointment.RecurrenceProperites.RecurrenceType;
                if (RangeStartdate != null)
                    RangeStartdate.DisplayDate = appointment.RecurrenceProperites.RangeStartDate;
                if (RangeEnddate != null)
                    RangeEnddate.DisplayDate = appointment.RecurrenceProperites.RangeEndDate;
                if (EndAfterCount != null)
                    EndAfterCount.Value = appointment.RecurrenceProperites.RangeRecurrenceCount;

                if (EndAfter != null)
                    EndAfter.IsChecked = appointment.RecurrenceProperites.IsRangeRecurrenceCount;
                if (EndBy != null)
                    EndBy.IsChecked = appointment.RecurrenceProperites.IsRangeEndDate;
                if (NoEndDate != null)
                    NoEndDate.IsChecked = appointment.RecurrenceProperites.IsRangeNoEndDate;
            }
            DeleteButtonVisibility = Visibility.Visible;
            InternalStartTime = appointment.InternalStartTime;
            InternalEndTime = appointment.InternalEndTime;
            TimeZoneCollection = sfSchedule.TimeZoneCollection;
            AllDay = appointment.AllDay;
            AppointmentStatusCollection = sfSchedule.AppointmentStatusCollection;
            AppointmentResourceName = sfSchedule.selectedResourcename;
            start = appointment.StartTime;
            start1 = appointment.StartTime;
            end = appointment.EndTime;
            end1 = appointment.EndTime;
            IsRecursive = appointment.IsRecursive;
            StartTimeZone = appointment.StartTimeZone;
            EndTimeZone = appointment.EndTimeZone;
            IsRecursiveModified = false;
            IsRecurrenceTypeEnabled = true;
            IsWarningMsgVisible = Visibility.Collapsed;
            ShowMoreVisibility = Visibility.Visible;
            ShowMoreStackPanelVisibility = Visibility.Collapsed;
            ReminderTime = appointment.ReminderTime;
            Delete.Visibility = Visibility.Visible;
            if (recursiveModified == 1)
            {
                IsRecursiveModified = true;
                IsRecurrenceTypeEnabled = false;
            }
            DeleteButtonVisibility = Visibility.Visible;
            if (appointment.Status != null)
            {
                for (int i = 0; i < AppointmentStatusCollection.Count; i++)
                {
                    if (appointment.Status.Status == AppointmentStatusCollection[i].Status)
                    {
                        Status.SelectedIndex = i;
                        break;
                    }
                }
            }
            else
            {
                Status.SelectedIndex = 0;
            }
            if (appointment.AllDay)
            {
                Starttime.IsEnabled = false;
                Endtime.IsEnabled = false;
            }
            var args = new AppointmentEditorOpeningEventArgs { Appointment = appointment };
            if (sfSchedule.ItemsSource != null)
            {
                var source = sfSchedule.ItemsSource as IEnumerable<object>;
                if (source != null)
                {
                    object obj = source.FirstOrDefault(x => x.GetHashCode() == (int)appointment.ObjectID);
                    if (obj != null)
                    {
                        args.Appointment = obj;
                    }
                }
            }
            args.Action = EditorAction.Edit;
            args.SelectedResource = appointment.ResourceCollection.ToList();
            args.StartTime = sfSchedule.Currentselecteddate;
            sfSchedule.GetAppointmentWindowOpeningEvents(args);
        }

        #endregion

        #region GetLocalizedArrayValues

        private ObservableCollection<LocalizationMember> GetLocalizedArrayValues(Array array)
        {
            var localizationMembers = new ObservableCollection<LocalizationMember>();
            foreach (object obj in array)
            {
                localizationMembers.Add(new LocalizationMember { ActualValue = obj, DisplayMember = SRSchedule.GetString(obj.ToString()) });
            }
            return localizationMembers;
        }

        #endregion

        #endregion

        #region Events

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Close_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Close_MouseEnter(object sender, MouseEventArgs e)
        {
            CloseBorder.Opacity = 1;
        }

        private void Close_MouseLeave(object sender, MouseEventArgs e)
        {
            CloseBorder.Opacity = 0.6;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            if (Startdate.SelectedDate != null)
            {
                var stDate = (DateTime)Startdate.SelectedDate;
                if (Starttime.DateTime != null)
                {
                    var stTime = (DateTime)Starttime.DateTime;
                    var stspan = stTime.TimeOfDay;
                    stDate = stDate.Date.AddTimeSpan(stspan);
                }
                if (stDate == start)
                    InternalStartTime = start;
                start1 = stDate;
                start = stDate;
            }
            if (Enddate.SelectedDate != null)
            {
                var edDate = (DateTime)Enddate.SelectedDate;
                if (Endtime.DateTime != null)
                {
                    var edTime = (DateTime)Endtime.DateTime;
                    TimeSpan edspan = edTime.TimeOfDay;
                    edDate = edDate.Date.AddTimeSpan(edspan);
                }
                if (edDate == end)
                    InternalEndTime = end;
                end1 = edDate;
                end = edDate;
            }

            if (InternalStartTime < InternalEndTime || (AllDay && InternalStartTime <= InternalEndTime))
            {
                ErrorMsg.Visibility = Visibility.Collapsed;
                Close();
            }
            else
            {
                ErrorMsg.Visibility = Visibility.Visible;
            }
        }

        private void EditRec_Click(object sender, RoutedEventArgs e)
        {
            RecApp.Visibility = Visibility.Visible;
            AppProp.Visibility = Visibility.Collapsed;
            SaveRec.Visibility = Visibility.Visible;
            CancelRec.Visibility = Visibility.Visible;
            RecCancel.Visibility = Visibility.Visible;
            EditRec.Visibility = Visibility.Collapsed;
            if (EditedAppointment.RecurrenceProperites == null && !IsRecursive)
            {
                var recurrenceProperties = new RecurrenceProperties();
                RecurrenceDaily = true;
                EveryXDays.IsChecked = recurrenceProperties.IsDailyEveryNDays;
                EveryDayGap.Value = recurrenceProperties.DailyNDays;
                EveryWeekGap.Value = recurrenceProperties.WeeklyEveryNWeeks;
                sunday.IsChecked = recurrenceProperties.IsWeeklySunday;
                monday.IsChecked = recurrenceProperties.IsWeeklyMonday;
                tuesday.IsChecked = recurrenceProperties.IsWeeklyTuesday;
                wednesday.IsChecked = recurrenceProperties.IsWeeklyWednesday;
                thursday.IsChecked = recurrenceProperties.IsWeeklyThursday;
                friday.IsChecked = recurrenceProperties.IsWeeklyFriday;
                saturday.IsChecked = recurrenceProperties.IsWeeklySaturday;
                EveryXMonths.IsChecked = recurrenceProperties.IsMonthlySpecific;
                EveryMonthGap.Value = recurrenceProperties.MonthlyEveryNMonths;
                EveryMonthDay.SelectedIndex = recurrenceProperties.MonthlySpecificMonthDay - 1 >= 0 ? recurrenceProperties.MonthlySpecificMonthDay - 1 : 0;
                Howmanyth.SelectedIndex = recurrenceProperties.MonthlyNthWeek - 1 >= 0 ? recurrenceProperties.MonthlyNthWeek - 1 : 0;
                Weekday.SelectedIndex = recurrenceProperties.MonthlyWeekDay - 1;
                SpecificDate.IsChecked = recurrenceProperties.IsYearlySpecific;
                EveryYearGap.Value = recurrenceProperties.YearlyEveryNYears;
                YrSpDate.SelectedIndex = recurrenceProperties.YearlySpecificMonthDay - 1 >= 0 ? recurrenceProperties.YearlySpecificMonthDay - 1 : 0;
                YrSpMonth.SelectedIndex = recurrenceProperties.YearlySpecificMonth - 1 >= 0 ? recurrenceProperties.YearlySpecificMonth - 1 : 0;
                YrHowmanyth.SelectedIndex = recurrenceProperties.YearlyNthWeek - 1 >= 0 ? recurrenceProperties.YearlyNthWeek - 1 : 0;
                YrWeekday.SelectedIndex = recurrenceProperties.YearlyWeekDay - 1;
                YrMonth.SelectedIndex = recurrenceProperties.YearlyGenericMonth - 1 >= 0 ? recurrenceProperties.YearlyGenericMonth - 1 : 0;
                RecurrenceType = recurrenceProperties.RecurrenceType;
                RangeStartdate.SelectedDate = EditedAppointment.StartTime.Date;
                RangeEnddate.SelectedDate = EditedAppointment.StartTime.Date.AddDays(1);
                EndAfterCount.Value = recurrenceProperties.RangeRecurrenceCount;
                EndAfter.IsChecked = recurrenceProperties.IsRangeRecurrenceCount;
                EndBy.IsChecked = recurrenceProperties.IsRangeEndDate;
                NoEndDate.IsChecked = recurrenceProperties.IsRangeNoEndDate;
            }
            else
            {
                if (EditedAppointment.RecurrenceProperites != null)
                {
                    if (EditedAppointment.RecurrenceProperites.RecurrenceType == RecurrenceType.Daily)
                    {
                        RecurrenceDaily = true;
                        if (EveryXDays != null)
                            EveryXDays.IsChecked = EditedAppointment.RecurrenceProperites.IsDailyEveryNDays;

                        if (EveryDayGap != null)
                            EveryDayGap.Value = EditedAppointment.RecurrenceProperites.DailyNDays;

                    }
                    else if (EditedAppointment.RecurrenceProperites.RecurrenceType == RecurrenceType.Weekly)
                    {
                        RecurrenceWeekly = true;

                        if (EveryWeekGap != null)
                            EveryWeekGap.Value = EditedAppointment.RecurrenceProperites.WeeklyEveryNWeeks;

                        if (sunday != null)
                            sunday.IsChecked = EditedAppointment.RecurrenceProperites.IsWeeklySunday;
                        if (monday != null)
                            monday.IsChecked = EditedAppointment.RecurrenceProperites.IsWeeklyMonday;
                        if (tuesday != null)
                            tuesday.IsChecked = EditedAppointment.RecurrenceProperites.IsWeeklyTuesday;
                        if (wednesday != null)
                            wednesday.IsChecked = EditedAppointment.RecurrenceProperites.IsWeeklyWednesday;
                        if (thursday != null)
                            thursday.IsChecked = EditedAppointment.RecurrenceProperites.IsWeeklyThursday;
                        if (friday != null)
                            friday.IsChecked = EditedAppointment.RecurrenceProperites.IsWeeklyFriday;
                        if (saturday != null)
                            saturday.IsChecked = EditedAppointment.RecurrenceProperites.IsWeeklySaturday;

                    }
                    else if (EditedAppointment.RecurrenceProperites.RecurrenceType == RecurrenceType.Monthly)
                    {
                        RecurrenceMonthly = true;
                        if (EveryXMonths != null)
                            EveryXMonths.IsChecked = EditedAppointment.RecurrenceProperites.IsMonthlySpecific;

                        if (EveryMonthGap != null)
                            EveryMonthGap.Value = EditedAppointment.RecurrenceProperites.MonthlyEveryNMonths;

                        if (EveryMonthDay != null)
                            EveryMonthDay.SelectedIndex = EditedAppointment.RecurrenceProperites.MonthlySpecificMonthDay - 1 >= 0 ? EditedAppointment.RecurrenceProperites.MonthlySpecificMonthDay - 1 : 0;
                        if (Howmanyth != null)
                            Howmanyth.SelectedIndex = EditedAppointment.RecurrenceProperites.MonthlyNthWeek - 1 >= 0 ? EditedAppointment.RecurrenceProperites.MonthlyNthWeek - 1 : 0;
                        if (Weekday != null)
                            Weekday.SelectedIndex = EditedAppointment.RecurrenceProperites.MonthlyWeekDay - 1;
                    }
                    else if (EditedAppointment.RecurrenceProperites.RecurrenceType == RecurrenceType.Yearly)
                    {
                        RecurrenceYearly = true;
                        if (SpecificDate != null)
                            SpecificDate.IsChecked = EditedAppointment.RecurrenceProperites.IsYearlySpecific;

                        if (EveryYearGap != null)
                            EveryYearGap.Value = EditedAppointment.RecurrenceProperites.YearlyEveryNYears;

                        if (YrSpDate != null)
                            YrSpDate.SelectedIndex = EditedAppointment.RecurrenceProperites.YearlySpecificMonthDay - 1 >= 0 ? EditedAppointment.RecurrenceProperites.YearlySpecificMonthDay - 1 : 0;
                        if (YrSpMonth != null)
                            YrSpMonth.SelectedIndex = EditedAppointment.RecurrenceProperites.YearlySpecificMonth - 1 >= 0 ? EditedAppointment.RecurrenceProperites.YearlySpecificMonth - 1 : 0;
                        if (YrHowmanyth != null)
                            YrHowmanyth.SelectedIndex = EditedAppointment.RecurrenceProperites.YearlyNthWeek - 1 >= 0 ? EditedAppointment.RecurrenceProperites.YearlyNthWeek - 1 : 0;
                        if (YrWeekday != null)
                            YrWeekday.SelectedIndex = EditedAppointment.RecurrenceProperites.YearlyWeekDay - 1;
                        if (YrMonth != null)
                            YrMonth.SelectedIndex = EditedAppointment.RecurrenceProperites.YearlyGenericMonth - 1 >= 0 ? EditedAppointment.RecurrenceProperites.YearlyGenericMonth - 1 : 0;
                    }

                    RecurrenceType = EditedAppointment.RecurrenceProperites.RecurrenceType;

                    if (RangeStartdate != null)
                        RangeStartdate.DisplayDate = EditedAppointment.RecurrenceProperites.RangeStartDate;
                    if (RangeEnddate != null)
                        RangeEnddate.DisplayDate = EditedAppointment.RecurrenceProperites.RangeEndDate;
                    if (EndAfterCount != null)
                        EndAfterCount.Value = EditedAppointment.RecurrenceProperites.RangeRecurrenceCount;

                    if (EndAfter != null)
                        EndAfter.IsChecked = EditedAppointment.RecurrenceProperites.IsRangeRecurrenceCount;
                    if (EndBy != null)
                        EndBy.IsChecked = EditedAppointment.RecurrenceProperites.IsRangeEndDate;
                    if (NoEndDate != null)
                        NoEndDate.IsChecked = EditedAppointment.RecurrenceProperites.IsRangeNoEndDate;
                }
            }
        }

        private void SaveRec_Click(object sender, RoutedEventArgs e)
        {
            RecApp.Visibility = Visibility.Collapsed;
            AppProp.Visibility = Visibility.Visible;
            SaveRec.Visibility = Visibility.Collapsed;
            CancelRec.Visibility = Visibility.Collapsed;
            RecCancel.Visibility = Visibility.Collapsed;
            EditRec.Visibility = Visibility.Visible;
            IsRecursive = true;
            RecurrenceEnabled.Visibility = Visibility.Visible;
        }

        private void CancelRec_Click(object sender, RoutedEventArgs e)
        {
            RecApp.Visibility = Visibility.Collapsed;
            AppProp.Visibility = Visibility.Visible;
            SaveRec.Visibility = Visibility.Collapsed;
            CancelRec.Visibility = Visibility.Collapsed;
            RecCancel.Visibility = Visibility.Collapsed;
            EditRec.Visibility = Visibility.Visible;
            //EditedAppointment.RecurrenceProperites = null;
            RecurrenceEnabled.Visibility = Visibility.Collapsed;
            IsRecursive = false;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void EndDay_Checked(object sender, RoutedEventArgs e)
        {
            if (EndAfterCount != null && EndAfter != null && EndAfter.IsChecked != null)
                EndAfterCount.IsEnabled =  (bool)EndAfter.IsChecked;
            if (RangeEnddate != null && EndBy != null && EndBy.IsChecked != null)
                RangeEnddate.IsEnabled = (bool)EndBy.IsChecked;
        }

        void EveryMonths_Checked(object sender, RoutedEventArgs e)
        {
            if (EveryMonthDay != null && EveryXMonths != null && EveryXMonths.IsChecked != null)
                EveryMonthDay.IsEnabled = (bool)EveryXMonths.IsChecked;
            if (EveryMonth != null && EveryMonth.IsChecked != null)
            {
                if (Howmanyth != null)
                    Howmanyth.IsEnabled = (bool)EveryMonth.IsChecked;
                if (Weekday != null)
                    Weekday.IsEnabled = (bool)EveryMonth.IsChecked;
            }
        }

        private void EveryYear_Checked(object sender, RoutedEventArgs e)
        {
            if (SpecificDate != null && SpecificDate.IsChecked != null)
            {
                if (YrSpMonth != null)
                    YrSpMonth.IsEnabled = (bool)SpecificDate.IsChecked;
                if (YrSpDate != null)
                    YrSpDate.IsEnabled = (bool)SpecificDate.IsChecked;
            }
            if (EveryYear != null && EveryYear.IsChecked != null)
            {
                if (YrHowmanyth != null)
                    YrHowmanyth.IsEnabled = (bool)EveryYear.IsChecked;
                if (YrWeekday != null)
                    YrWeekday.IsEnabled = (bool)EveryYear.IsChecked;
                if (YrMonth != null)
                    YrMonth.IsEnabled = (bool)EveryYear.IsChecked;
            }
        }

        private void RecCancel_Click(object sender, RoutedEventArgs e)
        {
            RecApp.Visibility = Visibility.Collapsed;
            AppProp.Visibility = Visibility.Visible;
            SaveRec.Visibility = Visibility.Collapsed;
            CancelRec.Visibility = Visibility.Collapsed;
            RecCancel.Visibility = Visibility.Collapsed;
            if (!IsRecursive)
                EditedAppointment.RecurrenceProperites = null;
            EditRec.Visibility = Visibility.Visible;
            RecurrenceEnabled.Visibility = Visibility.Collapsed;
        }

        #endregion        
    }


    #region LocalizationMember

    public class LocalizationMember
    {
        public object ActualValue
        {
            get;
            internal set;
        }

        public object DisplayMember
        {
            get;
            internal set;
        }
    }

    #endregion
}
