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
using System.Globalization;
using System.Linq;
using Syncfusion.UI.Xaml.Controls.Input;
using Windows.ApplicationModel.Resources.Core;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Input;

namespace Syncfusion.UI.Xaml.Schedule
{
    #region ScheduleAppointmentEditor

    /// <summary>
    /// Represents the Schedule Appointment Editor.
    /// </summary>
    public class ScheduleAppointmentEditor : Control, IDisposable
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.UI.Xaml.Schedule.ScheduleAppointmentEditor">ScheduleAppointmentEditor</see> class. 
        /// </summary>
        /// <remarks></remarks>
        public ScheduleAppointmentEditor()
        {
            DefaultStyleKey = typeof(ScheduleAppointmentEditor);
            EditedAppointment = new ScheduleAppointment();
            closedEventArgs = new AppointmentEditorClosedEventArgs();
            LoadLocalizedData();
            DataContext = this;
        }

        #endregion

        #region Private Fields

        private Border StartBorder;
        private Border EndBorder;
        private Button CloseAppointment;
        private Button DoneAppointment;
        private Button DeleteAppointment;
        private Grid ShowMore;
        private Grid StartGrid;
        private Grid EndGrid;
        private SfDatePicker Startdate;
        private SfTimePicker Starttime;
        private SfDatePicker Enddate;
        private SfTimePicker Endtime;
        private AppointmentEditorClosedEventArgs closedEventArgs;

        #endregion

        #region Internal Fields

        internal ComboBox AppointmentStatus;
        internal ComboBox starttimecombo;
        internal ComboBox endtimecombo;
        internal ComboBox ReminderTimeComboBox;
        internal Grid EmptySpace;
        internal SfTextBoxExt notes;
        internal SfTextBoxExt subject;
        internal SfTextBoxExt location;
        internal ToggleSwitch readOnly;
        internal ToggleSwitch isRecursive;
        internal SfNumericUpDown EveryDayGap;
        internal SfNumericUpDown EveryWeekGap;
        internal SfNumericUpDown EveryMonthGap;
        internal SfDatePicker RangeStartdate;
        internal SfDatePicker RangeEnddate;
        internal SfNumericUpDown EveryYearGap;
        internal SfNumericUpDown EndAfterCount;
        internal CheckBox sunday;
        internal CheckBox monday;
        internal CheckBox tuesday;
        internal CheckBox wednesday;
        internal CheckBox thursday;
        internal CheckBox friday;
        internal CheckBox saturday;
        internal RadioButton EveryXMonths;
        internal RadioButton EveryMonth;
        internal ComboBox EveryMonthDay;
        internal ComboBox Howmanyth;
        internal ComboBox Weekday;
        internal RadioButton SpecificDate;
        internal RadioButton EveryYear;
        internal ComboBox YrSpMonth;
        internal ComboBox YrSpDate;
        internal ComboBox YrHowmanyth;
        internal ComboBox YrWeekday;
        internal ComboBox YrMonth;
        internal RadioButton EndAfter;
        internal RadioButton EndBy;
        internal RadioButton NoEndDate;
        internal CheckBox allday;
        internal RadioButton EveryXDays;
        internal int recursiveModified;
        internal bool isTemplateApplied;
        internal bool needUpdate;

        #endregion

        #region Public Fields

#if !WINRT
        public bool Check = false;
#endif

        #endregion

        #region Dependency Properties

        #region ScheduleResourceTypeCollection
        internal ObservableCollection<ResourceType> ScheduleResourceTypeCollection
        {
            get { return (ObservableCollection<ResourceType>)GetValue(ScheduleResourceTypeCollectionProperty); }
            set { SetValue(ScheduleResourceTypeCollectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScheduleResourceTypeCollection.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ScheduleResourceTypeCollectionProperty =
            DependencyProperty.Register("ScheduleResourceTypeCollection", typeof(ObservableCollection<ResourceType>), typeof(ScheduleAppointmentEditor), new PropertyMetadata(null));
        #endregion

        #region start
        internal DateTime start
        {
            get { return (DateTime)GetValue(startProperty); }
            set { SetValue(startProperty, value); }
        }

        // Using a DependencyProperty as the backing store for start.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty startProperty =
            DependencyProperty.Register("start", typeof(DateTime), typeof(ScheduleAppointmentEditor), new PropertyMetadata(DateTime.Today, OnStartTimeChanged));

        private static void OnStartTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                var scheduleAppointmentEditor = d as ScheduleAppointmentEditor;
                if (scheduleAppointmentEditor != null)
                    scheduleAppointmentEditor.InternalStartTime = scheduleAppointmentEditor.start;
            }
        }
        #endregion

        #region end
        internal DateTime end
        {
            get { return (DateTime)GetValue(endProperty); }
            set { SetValue(endProperty, value); }
        }

        // Using a DependencyProperty as the backing store for end.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty endProperty =
            DependencyProperty.Register("end", typeof(DateTime), typeof(ScheduleAppointmentEditor), new PropertyMetadata(DateTime.Today, OnEndTimeChanged));

        private static void OnEndTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                var scheduleAppointmentEditor = d as ScheduleAppointmentEditor;
                if (scheduleAppointmentEditor != null)
                    scheduleAppointmentEditor.InternalEndTime = scheduleAppointmentEditor.end;
            }
        }
        #endregion

        #region RecurrenceDaily
        internal bool RecurrenceDaily
        {
            get { return (bool)GetValue(RecurrenceDailyProperty); }
            set { SetValue(RecurrenceDailyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RecurrenceOptionDaily.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty RecurrenceDailyProperty =
            DependencyProperty.Register("RecurrenceDaily", typeof(bool), typeof(ScheduleAppointmentEditor), new PropertyMetadata(true));
        #endregion

        #region RecurrenceWeekly
        internal bool RecurrenceWeekly
        {
            get { return (bool)GetValue(RecurrenceWeeklyProperty); }
            set { SetValue(RecurrenceWeeklyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RecurrenceOptionWeekly.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty RecurrenceWeeklyProperty =
            DependencyProperty.Register("RecurrenceWeekly", typeof(bool), typeof(ScheduleAppointmentEditor), new PropertyMetadata(false));
        #endregion

        #region RecurrenceMonthly
        internal bool RecurrenceMonthly
        {
            get { return (bool)GetValue(RecurrenceMonthlyProperty); }
            set { SetValue(RecurrenceMonthlyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RecurrenceMonthly.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty RecurrenceMonthlyProperty =
            DependencyProperty.Register("RecurrenceMonthly", typeof(bool), typeof(ScheduleAppointmentEditor), new PropertyMetadata(false));
        #endregion

        #region RecurrenceYearly
        internal bool RecurrenceYearly
        {
            get { return (bool)GetValue(RecurrenceYearlyProperty); }
            set { SetValue(RecurrenceYearlyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RecurrenceYearly.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty RecurrenceYearlyProperty =
            DependencyProperty.Register("RecurrenceYearly", typeof(bool), typeof(ScheduleAppointmentEditor), new PropertyMetadata(false));
        #endregion

        #region AppointmentResourceName
        internal List<Resource> AppointmentResourceName
        {
            get { return (List<Resource>)GetValue(AppointmentResourceNameProperty); }
            set { SetValue(AppointmentResourceNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AppointmentResourceName.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty AppointmentResourceNameProperty =
            DependencyProperty.Register("AppointmentResourceName", typeof(List<Resource>), typeof(ScheduleAppointmentEditor), new PropertyMetadata(null));
        #endregion

        #region InternalStartTime
        internal DateTime InternalStartTime
        {
            get { return ScheduleAppointment.ConvertToLocalTime((DateTime)GetValue(InternalStartTimeProperty), "StartTime", StartTimeZone, null); }
            set { SetValue(InternalStartTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartTime.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty InternalStartTimeProperty =
            DependencyProperty.Register("InternalStartTime", typeof(DateTime), typeof(ScheduleAppointmentEditor), new PropertyMetadata(DateTime.Today));
        #endregion

        #region InternalEndTime
        internal DateTime InternalEndTime
        {
            get { return ScheduleAppointment.ConvertToLocalTime((DateTime)GetValue(InternalEndTimeProperty), "EndTime", null, EndTimeZone); }
            set { SetValue(InternalEndTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EndTime.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty InternalEndTimeProperty =
            DependencyProperty.Register("InternalEndTime", typeof(DateTime), typeof(ScheduleAppointmentEditor), new PropertyMetadata(DateTime.Today));
        #endregion

        #region ReminderTime
        internal ReminderTimeType ReminderTime
        {
            get { return (ReminderTimeType)GetValue(ReminderTimeIntervalProperty); }
            set { SetValue(ReminderTimeIntervalProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ReminderTime.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ReminderTimeIntervalProperty =
            DependencyProperty.Register("ReminderTime", typeof(ReminderTimeType), typeof(ScheduleAppointmentEditor), new PropertyMetadata(ReminderTimeType.None, OnReminderTimeChanged));

        private static void OnReminderTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                var scheduleAppointmentEditor = d as ScheduleAppointmentEditor;
                if (scheduleAppointmentEditor != null && scheduleAppointmentEditor.ReminderTimeComboBox != null &&
                    scheduleAppointmentEditor.ReminderTimeComboBox.Items != null)
                {
                    for (int i = 0; i < scheduleAppointmentEditor.ReminderTimeComboBox.Items.Count; i++)
                    {
                        scheduleAppointmentEditor.ReminderTimeComboBox.SelectedIndex = i;
                        if (scheduleAppointmentEditor.ReminderTimeComboBox.SelectedValue != null &&
                            scheduleAppointmentEditor.ReminderTimeComboBox.SelectedValue.ToString() == scheduleAppointmentEditor.ReminderTime.ToString())
                        {
                            scheduleAppointmentEditor.ReminderTimeComboBox.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
        }

        #endregion

        #region ReminderDeliveryTime
        internal DateTime ReminderDeliveryTime
        {
            get { return (DateTime)GetValue(ReminderTimeProperty); }
            set { SetValue(ReminderTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ReminderDeliveryTime.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ReminderTimeProperty =
            DependencyProperty.Register("ReminderDeliveryTime", typeof(DateTime), typeof(ScheduleAppointmentEditor), new PropertyMetadata(DateTime.Now));
        #endregion

        #region AllDay
        internal bool AllDay
        {
            get { return (bool)GetValue(AllDayProperty); }
            set { SetValue(AllDayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllDay.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty AllDayProperty =
            DependencyProperty.Register("AllDay", typeof(bool), typeof(ScheduleAppointmentEditor), new PropertyMetadata(false, OnAllDayPropertyChanged));

        private static void OnAllDayPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var AppEditor = d as ScheduleAppointmentEditor;

            if (AppEditor != null)
            {
                if ((bool)e.NewValue)
                    AppEditor.EndDateEnable = false;
                else
                    AppEditor.EndDateEnable = true;
            }
        }
        #endregion

        #region EndDateEnable
        internal bool EndDateEnable
        {
            get { return (bool)GetValue(EndDateEnableProperty); }
            set { SetValue(EndDateEnableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EndDateEnable.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty EndDateEnableProperty =
            DependencyProperty.Register("EndDateEnable", typeof(bool), typeof(ScheduleAppointmentEditor), new PropertyMetadata(true));
        #endregion

        #region AppointmentEditorBackground
        internal Brush AppointmentEditorBackground
        {
            get { return (Brush)GetValue(AppointmentEditorBackgroundProperty); }
            set { SetValue(AppointmentEditorBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AppointmentEditorBackground.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty AppointmentEditorBackgroundProperty =
            DependencyProperty.Register("AppointmentEditorBackground", typeof(Brush), typeof(ScheduleAppointmentEditor), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x3A, 0x3A, 0x3A))));
        #endregion

        #region StartAccordionBackground
        internal Brush StartAccordionBackground
        {
            get { return (Brush)GetValue(StartAccordionBackgroundProperty); }
            set { SetValue(StartAccordionBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartAccordionBackground.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty StartAccordionBackgroundProperty =
            DependencyProperty.Register("StartAccordionBackground", typeof(Brush), typeof(ScheduleAppointmentEditor), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x3A, 0x3A, 0x3A))));
        #endregion

        #region EndAccordionBackground
        internal Brush EndAccordionBackground
        {
            get { return (Brush)GetValue(EndAccordionBackgroundProperty); }
            set { SetValue(EndAccordionBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EndAccordionBackground.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty EndAccordionBackgroundProperty =
            DependencyProperty.Register("EndAccordionBackground", typeof(Brush), typeof(ScheduleAppointmentEditor), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x3A, 0x3A, 0x3A))));
        #endregion

        #region RecurrenceAccordionBackground
        internal Brush RecurrenceAccordionBackground
        {
            get { return (Brush)GetValue(RecurrenceAccordionBackgroundProperty); }
            set { SetValue(RecurrenceAccordionBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RecurrenceAccordionBackground.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty RecurrenceAccordionBackgroundProperty =
            DependencyProperty.Register("RecurrenceAccordionBackground", typeof(Brush), typeof(ScheduleAppointmentEditor), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x3A, 0x3A, 0x3A))));
        #endregion

        #region AppointmentEditorForeground
        internal Brush AppointmentEditorForeground
        {
            get { return (Brush)GetValue(AppointmentEditorForegroundProperty); }
            set { SetValue(AppointmentEditorForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AppointmentEditorForeground.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty AppointmentEditorForegroundProperty =
            DependencyProperty.Register("AppointmentEditorForeground", typeof(Brush), typeof(ScheduleAppointmentEditor), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0xCE, 0xCE, 0xCE))));
        #endregion

        #region ShowMoreVisibility
        internal Visibility ShowMoreVisibility
        {
            get { return (Visibility)GetValue(ShowMoreVisibilityProperty); }
            set { SetValue(ShowMoreVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowMoreVisibility.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ShowMoreVisibilityProperty =
            DependencyProperty.Register("ShowMoreVisibility", typeof(Visibility), typeof(ScheduleAppointmentEditor), new PropertyMetadata(Visibility.Visible));
        #endregion

        #region ShowMoreStackPanelVisibility
        internal Visibility ShowMoreStackPanelVisibility
        {
            get { return (Visibility)GetValue(ShowMoreStackPanelVisibilityProperty); }
            set { SetValue(ShowMoreStackPanelVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowMoreStackPanelVisibility.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ShowMoreStackPanelVisibilityProperty =
            DependencyProperty.Register("ShowMoreStackPanelVisibility", typeof(Visibility), typeof(ScheduleAppointmentEditor), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #region StartStackPanelVisibility
        internal Visibility StartStackPanelVisibility
        {
            get { return (Visibility)GetValue(StartStackPanelVisibilityProperty); }
            set { SetValue(StartStackPanelVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartStackPanelVisibility.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty StartStackPanelVisibilityProperty =
            DependencyProperty.Register("StartStackPanelVisibility", typeof(Visibility), typeof(ScheduleAppointmentEditor), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #region EndStackPanelVisibility
        internal Visibility EndStackPanelVisibility
        {
            get { return (Visibility)GetValue(EndStackPanelVisibilityProperty); }
            set { SetValue(EndStackPanelVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EndStackPanelVisibility.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty EndStackPanelVisibilityProperty =
            DependencyProperty.Register("EndStackPanelVisibility", typeof(Visibility), typeof(ScheduleAppointmentEditor), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #region RecursiveStockPanelVisibility
        internal Visibility RecursiveStockPanelVisibility
        {
            get { return (Visibility)GetValue(RecursiveStockPanelVisibilityProperty); }
            set { SetValue(RecursiveStockPanelVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RecursiveStockPanelVisibility.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty RecursiveStockPanelVisibilityProperty =
            DependencyProperty.Register("RecursiveStockPanelVisibility", typeof(Visibility), typeof(ScheduleAppointmentEditor), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #region DailyStackPanelVisibility
        internal Visibility DailyStackPanelVisibility
        {
            get { return (Visibility)GetValue(DailyStackPanelVisibilityProperty); }
            set { SetValue(DailyStackPanelVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DailyStackPanelVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DailyStackPanelVisibilityProperty =
            DependencyProperty.Register("DailyStackPanelVisibility", typeof(Visibility), typeof(ScheduleAppointmentEditor), new PropertyMetadata(Visibility.Visible));
        #endregion

        #region WeeklyStackPanelVisibility
        internal Visibility WeeklyStackPanelVisibility
        {
            get { return (Visibility)GetValue(WeeklyStackPanelVisibilityProperty); }
            set { SetValue(WeeklyStackPanelVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WeeklyStackPanelVisibility.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty WeeklyStackPanelVisibilityProperty =
            DependencyProperty.Register("WeeklyStackPanelVisibility", typeof(Visibility), typeof(ScheduleAppointmentEditor), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #region MonthlyStackPanelVisibility
        internal Visibility MonthlyStackPanelVisibility
        {
            get { return (Visibility)GetValue(MonthlyStackPanelVisibilityProperty); }
            set { SetValue(MonthlyStackPanelVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MonthlyStackPanelVisibility.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty MonthlyStackPanelVisibilityProperty =
            DependencyProperty.Register("MonthlyStackPanelVisibility", typeof(Visibility), typeof(ScheduleAppointmentEditor), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #region YearlyStackPanelVisibility
        internal Visibility YearlyStackPanelVisibility
        {
            get { return (Visibility)GetValue(YearlyStackPanelVisibilityProperty); }
            set { SetValue(YearlyStackPanelVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for YearlyStackPanelVisibility.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty YearlyStackPanelVisibilityProperty =
            DependencyProperty.Register("YearlyStackPanelVisibility", typeof(Visibility), typeof(ScheduleAppointmentEditor), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #region IsWarningMsgVisible
        internal Visibility IsWarningMsgVisible
        {
            get { return (Visibility)GetValue(IsWarningMsgVisibleProperty); }
            set { SetValue(IsWarningMsgVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsWarningMsgVisible.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty IsWarningMsgVisibleProperty =
            DependencyProperty.Register("IsWarningMsgVisible", typeof(Visibility), typeof(ScheduleAppointmentEditor), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #region IsRecurrenceTypeEnabled
        internal bool IsRecurrenceTypeEnabled
        {
            get { return (bool)GetValue(IsRecurrenceTypeEnabledProperty); }
            set { SetValue(IsRecurrenceTypeEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RecurrenceTypeVisibility.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty IsRecurrenceTypeEnabledProperty =
            DependencyProperty.Register("IsRecurrenceTypeEnabled", typeof(bool), typeof(ScheduleAppointmentEditor), new PropertyMetadata(true));
        #endregion

        #region DeleteButtonVisibility
        internal Visibility DeleteButtonVisibility
        {
            get { return (Visibility)GetValue(DeleteButtonVisibilityProperty); }
            set { SetValue(DeleteButtonVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DeleteButtonVisibility.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty DeleteButtonVisibilityProperty =
            DependencyProperty.Register("DeleteButtonVisibility", typeof(Visibility), typeof(ScheduleAppointmentEditor), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #region AppointmentStatusCollection
        internal ScheduleAppointmentStatusCollection AppointmentStatusCollection
        {
            get { return (ScheduleAppointmentStatusCollection)GetValue(AppointmentStatusCollectionProperty); }
            set { SetValue(AppointmentStatusCollectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AppointmentStatusCollection.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty AppointmentStatusCollectionProperty =
            DependencyProperty.Register("AppointmentStatusCollection", typeof(ScheduleAppointmentStatusCollection), typeof(ScheduleAppointmentEditor), new PropertyMetadata(null, OnAppointmentStatusCollectionChanged));

        private static void OnAppointmentStatusCollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleAppointmentEditor)
            {
                ScheduleAppointmentEditor appointmentEditor = d as ScheduleAppointmentEditor;
                if (appointmentEditor.AppointmentStatus != null)
                {
                    appointmentEditor.AppointmentStatus.ItemsSource = appointmentEditor.GetLocalizedArrayValues(appointmentEditor.AppointmentStatusCollection.Select(appointmentStatus => appointmentStatus.Status).ToArray());
                }
            }
        }
        #endregion

        #region StartTimeZone
        internal TimeZone StartTimeZone
        {
            get { return (TimeZone)GetValue(StartTimeZoneProperty); }
            set { SetValue(StartTimeZoneProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TimeZones.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty StartTimeZoneProperty =
            DependencyProperty.Register("StartTimeZone", typeof(TimeZone), typeof(ScheduleAppointmentEditor), new PropertyMetadata(null, OnStartTimeZoneChanged));

        private static void OnStartTimeZoneChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var Editor = obj as ScheduleAppointmentEditor;
            if (Editor != null && Editor.TimeZoneCollection != null)
            {
                if (Editor.starttimecombo != null)
                {
                    if (Editor.StartTimeZone != null)
                        Editor.starttimecombo.SelectedItem = Editor.StartTimeZone;
                    else
                        Editor.starttimecombo.SelectedIndex = 10;
                }
            }
        }
        #endregion

        #region EndTimeZone
        internal TimeZone EndTimeZone
        {
            get { return (TimeZone)GetValue(EndTimeZoneProperty); }
            set { SetValue(EndTimeZoneProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty EndTimeZoneProperty =
            DependencyProperty.Register("EndTimeZone", typeof(TimeZone), typeof(ScheduleAppointmentEditor), new PropertyMetadata(TimeZoneInfo.Local.ToString()));
        #endregion

        #region TimeZoneCollection
        internal TimeZoneCollection TimeZoneCollection
        {
            get { return (TimeZoneCollection)GetValue(TimeZoneCollectionProperty); }
            set { SetValue(TimeZoneCollectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TimeZoneCollection.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty TimeZoneCollectionProperty =
            DependencyProperty.Register("TimeZoneCollection", typeof(TimeZoneCollection), typeof(ScheduleAppointmentEditor), new PropertyMetadata(null));
        #endregion

        #region RecurrenceType
        internal RecurrenceType RecurrenceType
        {
            get { return (RecurrenceType)GetValue(RecurrenceTypeProperty); }
            set { SetValue(RecurrenceTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RecurrenceType.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty RecurrenceTypeProperty =
            DependencyProperty.Register("RecurrenceType", typeof(RecurrenceType), typeof(ScheduleAppointmentEditor), new PropertyMetadata(null));
        #endregion

        #region EditedAppointment
        internal ScheduleAppointment EditedAppointment
        {
            get { return (ScheduleAppointment)GetValue(EditedAppointmentProperty); }
            set { SetValue(EditedAppointmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EditedAppointment.  This enables animation, styling, binding, etc...
#if WPF
        internal static readonly DependencyProperty EditedAppointmentProperty =
            DependencyProperty.Register("EditedAppointment", typeof(ScheduleAppointment), typeof(ScheduleAppointmentEditor), new PropertyMetadata(null));
#else
        internal static readonly DependencyProperty EditedAppointmentProperty =
           DependencyProperty.Register("EditedAppointment", typeof(ScheduleAppointment), typeof(ScheduleAppointmentEditor), new PropertyMetadata(new ScheduleAppointment()));
#endif
        #endregion

        #region IsRecursiveModified
        internal bool IsRecursiveModified
        {
            get { return (bool)GetValue(IsRecursiveModifiedProperty); }
            set { SetValue(IsRecursiveModifiedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsRecursiveModified.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty IsRecursiveModifiedProperty =
            DependencyProperty.Register("IsRecursiveModified", typeof(bool), typeof(ScheduleAppointmentEditor), new PropertyMetadata(false));
        #endregion

        #region AppointmentData
        internal Dictionary<string, string> AppointmentData
        {
            get { return (Dictionary<string, string>)GetValue(AppointmentDataProperty); }
            set { SetValue(AppointmentDataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AppointmentData.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty AppointmentDataProperty =
            DependencyProperty.Register("AppointmentData", typeof(Dictionary<string, string>), typeof(ScheduleAppointmentEditor), new PropertyMetadata(null));
        #endregion

        #endregion

        #region Events

        void EndDay_Checked(object sender, RoutedEventArgs e)
        {
            if (EndAfterCount != null && EndAfter != null && EndAfter.IsChecked != null)
                EndAfterCount.IsEnabled = (bool)EndAfter.IsChecked;
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

        void EveryYear_Checked(object sender, RoutedEventArgs e)
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

        void RangeStartdate_ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (((DateTime)Startdate.Value).Date != ((DateTime)e.NewValue).Date)
            {
                var existingStartDate = (DateTime)Startdate.Value;
                var existingEndDate = (DateTime)Enddate.Value;
                var rangeStartDate = (DateTime)e.NewValue;
                int daysDiff = (rangeStartDate.Date - existingStartDate.Date).Days;

                DateTime reqStartDate = existingStartDate.AddDays(daysDiff);
                DateTime reqEndDate = existingEndDate.AddDays(daysDiff);

                Startdate.Value = reqStartDate;
                Enddate.Value = reqEndDate;
                RangeEnddate.Value = rangeStartDate.AddDays(1);
            }
        }

        void Startdate_ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (RangeStartdate.Value != null && ((DateTime)RangeStartdate.Value).Date != ((DateTime)e.NewValue).Date && EditedAppointment != null &&
                EditedAppointment.RecurrenceProperites != null && !EditedAppointment.RecurrenceProperites.hasRangeStartDate)
            {
                var startDate = (DateTime)e.NewValue;
                var existingStartDate = (DateTime)e.OldValue;
                var existingEndDate = (DateTime)Enddate.Value;

                RangeStartdate.Value = startDate;
                RangeEnddate.Value = startDate.AddDays(1);

                var ticksDiff = existingEndDate.Ticks - existingStartDate.Ticks;
                var reqEndDate = startDate.AddTicks(ticksDiff);
                Enddate.Value = reqEndDate;
            }
        }

        void EndBorder_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            EndAccordionBackground = EndStackPanelVisibility == Visibility.Visible ? new SolidColorBrush(Colors.Black) : AppointmentEditorBackground;
        }

        void EndBorder_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            EndAccordionBackground = EndStackPanelVisibility == Visibility.Visible ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.Gray);
        }

        void StartBorder_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            StartAccordionBackground = StartStackPanelVisibility == Visibility.Visible ? new SolidColorBrush(Colors.Black) : AppointmentEditorBackground;
            VisualStateManager.GoToState(this, "up", true);
        }

        void StartBorder_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            StartAccordionBackground = StartStackPanelVisibility == Visibility.Visible ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.Gray);
            VisualStateManager.GoToState(this, "down", true);
        }

        void isRecursive_Toggled(object sender, RoutedEventArgs e)
        {
            if (isRecursive.IsOn)
            {
                RecursiveStockPanelVisibility = Visibility.Visible;
                VisualStateManager.GoToState(this, "RecurrenceOn", true);
                RecurrenceAccordionBackground = new SolidColorBrush(Color.FromArgb(0x34, 0x00, 0x00, 0x00));
                if (RangeStartdate != null)
                    RangeStartdate.Value = start;
                if (RangeEnddate != null)
                    RangeEnddate.Value = start.AddDays(1);
            }
            else
            {
                RecursiveStockPanelVisibility = Visibility.Collapsed;
                VisualStateManager.GoToState(this, "MouseLeft", true);
                RecurrenceAccordionBackground = AppointmentEditorBackground;
            }
        }

        #region PointerPressed

        void EmptySpace_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            ResetEditor();
            var schedule = this.FindParentElementOfType<SfSchedule>();
            closedEventArgs.Action = EditorClosedAction.Cancel;
            closedEventArgs.OriginalAppointment = schedule.SelectedAppointment;
            closedEventArgs.EditedAppointment = null;
            schedule.GetAppointmentEditorClosedEvents(closedEventArgs);
            schedule.CloseAnimation();
        }

        void ShowMore_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            ShowMoreVisibility = Visibility.Collapsed;
            ShowMoreStackPanelVisibility = Visibility.Visible;
        }

        void StartGrid_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (StartStackPanelVisibility == Visibility.Collapsed)
            {
                StartStackPanelVisibility = Visibility.Visible;
                VisualStateManager.GoToState(this, "StartOnClick", true);
                StartAccordionBackground = new SolidColorBrush(Colors.Black);
                if (EndStackPanelVisibility == Visibility.Visible)
                {
                    EndStackPanelVisibility = Visibility.Collapsed;
                    VisualStateManager.GoToState(this, "MouseLeft", true);
                    EndAccordionBackground = AppointmentEditorBackground;
                }
            }
            else
            {

                StartStackPanelVisibility = Visibility.Collapsed;
                VisualStateManager.GoToState(this, "MouseLeft", true);
                StartAccordionBackground = AppointmentEditorBackground;
            }

        }

        void EndGrid_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (EndStackPanelVisibility == Visibility.Collapsed)
            {
                EndStackPanelVisibility = Visibility.Visible;
                VisualStateManager.GoToState(this, "EndOnClick", true);
                EndAccordionBackground = new SolidColorBrush(Colors.Black);
                if (StartStackPanelVisibility == Visibility.Visible)
                {
                    StartStackPanelVisibility = Visibility.Collapsed;
                    VisualStateManager.GoToState(this, "MouseLeft", true);
                    StartAccordionBackground = AppointmentEditorBackground;
                }
            }
            else
            {
                EndStackPanelVisibility = Visibility.Collapsed;
                VisualStateManager.GoToState(this, "MouseLeft", true);
                EndAccordionBackground = AppointmentEditorBackground;
            }
        }

        #endregion

        #region Delete Appointment

        void DeleteAppointment_Click(object sender, RoutedEventArgs e)
        {
            var schedule = this.FindParentElementOfType<SfSchedule>();
            schedule.SelectedAppointment.IsRecursiveModified = IsRecursiveModified;
            closedEventArgs.Action = EditorClosedAction.Delete;
            closedEventArgs.OriginalAppointment = schedule.SelectedAppointment;
            closedEventArgs.EditedAppointment = null;
            schedule.GetAppointmentEditorClosedEvents(closedEventArgs);
            if (!closedEventArgs.Cancel)
            {
                if (schedule.SelectedAppointment.IsRecursive)
                {
                    if (schedule.Appointments.Contains(schedule.SelectedAppointment))
                    {
                        schedule.Appointments.Remove(schedule.SelectedAppointment);
                    }
                    else
                    {
                        schedule.RemoveRecursiveAppointment(schedule.SelectedAppointment);
                    }
                }
                else
                {
                    schedule.Appointments.Remove(schedule.SelectedAppointment);
                }
            }
            ResetEditor();
            schedule.CloseAnimation();
        }

        #endregion

        #region DoneAppointment

        void DoneAppointment_Click(object sender, RoutedEventArgs e)
        {
            var schedule = this.FindParentElementOfType<SfSchedule>();
            closedEventArgs.Action = EditorClosedAction.Save;
            closedEventArgs.OriginalAppointment = schedule.SelectedAppointment;
            var stDate = (DateTime)Startdate.Value;
            var stTime = (DateTime)Starttime.Value;
            var stspan = stTime.TimeOfDay;
            stDate = stDate.Date.AddTimeSpan(stspan);
            start = stDate;
            var edDate = (DateTime)Enddate.Value;
            var edTime = (DateTime)Endtime.Value;
            if (starttimecombo.SelectedItem != null)
            {
                StartTimeZone = (starttimecombo.SelectedItem as TimeZone);
            }
            if (endtimecombo.SelectedItem != null)
            {
                EndTimeZone = (endtimecombo.SelectedItem as TimeZone);
            }
            TimeSpan edspan = edTime.TimeOfDay;
            edDate = edDate.Date.AddTimeSpan(edspan);
            end = edDate;
            if (InternalStartTime < InternalEndTime || (AllDay && InternalStartTime <= InternalEndTime))
            {
                var rp = new RecurrenceProperties();
                if (schedule.SelectedAppointment != null)
                {
                    var clonedAppointment = schedule.CloneSelectedAppointment() as ScheduleAppointment;
                    EditSelectedAppointment(schedule, clonedAppointment, rp);
                    closedEventArgs.EditedAppointment = clonedAppointment;
                    schedule.GetAppointmentEditorClosedEvents(closedEventArgs);
                    if (!closedEventArgs.Cancel)
                        EditSelectedAppointment(schedule, schedule.SelectedAppointment, rp);
                }
                else
                {
                    var NewAppointment = new ScheduleAppointment
                    {
                        StartTime = start,
                        EndTime = end,
                        Status = AppointmentStatusCollection.First(appointmentStatus => appointmentStatus.Status.Equals(AppointmentStatus.SelectedValue.ToString())),
                        Notes = notes.Text,
                        Subject = subject.Text,
                        AllDay = allday.IsChecked != null && ((bool)(allday.IsChecked)),
                        Location = location.Text,
                        ReadOnly = readOnly.IsOn,
                        StartTimeZone = StartTimeZone,
                        EndTimeZone = EndTimeZone,
                        IsRecursive = isRecursive.IsOn
                    };
                    #region Recurrence

                    if (NewAppointment.IsRecursive)
                    {
                        //NewAppointment.RecurrenceProperites = new RecurrenceProperties();
                        if (RecurrenceDaily)
                        {
                            int everyNDays = 1;
                            double days;
                            bool intCheck = false;
                            bool isDouble = Double.TryParse((EveryDayGap.Value).ToString(), out days);
                            if (isDouble)
                                intCheck = int.TryParse((Math.Round(days)).ToString(), out everyNDays);
                            if (!intCheck)
                            {
                                string NDays = (EveryDayGap.Value).ToString();
                                rp.DailyNDays = int.Parse(NDays);
                            }
                            else
                            {
                                rp.DailyNDays = everyNDays;
                            }
                            rp.RecurrenceType = RecurrenceType.Daily;
                            if (EveryXDays.IsChecked != null)
                                rp.IsDailyEveryNDays = (bool)EveryXDays.IsChecked;
                        }
                        else if (RecurrenceWeekly)
                        {
                            int everyNWeeks = 1;
                            double weeks;
                            bool intCheck = false;
                            bool isDouble = Double.TryParse((EveryWeekGap.Value).ToString(), out weeks);
                            if (isDouble)
                                intCheck = int.TryParse((Math.Round(weeks)).ToString(), out everyNWeeks);
                            if (!intCheck)
                            {
                                string NWeeks = (EveryDayGap.Value).ToString();
                                rp.WeeklyEveryNWeeks = int.Parse(NWeeks);
                            }
                            else
                            {
                                rp.WeeklyEveryNWeeks = everyNWeeks;
                            }
                            rp.RecurrenceType = RecurrenceType.Weekly;
                            if (sunday.IsChecked != null)
                                rp.IsWeeklySunday = (bool)sunday.IsChecked;
                            if (monday.IsChecked != null)
                                rp.IsWeeklyMonday = (bool)monday.IsChecked;
                            if (tuesday.IsChecked != null)
                                rp.IsWeeklyTuesday = (bool)tuesday.IsChecked;
                            if (wednesday.IsChecked != null)
                                rp.IsWeeklyWednesday = (bool)wednesday.IsChecked;
                            if (thursday.IsChecked != null)
                                rp.IsWeeklyThursday = (bool)thursday.IsChecked;
                            if (friday.IsChecked != null)
                                rp.IsWeeklyFriday = (bool)friday.IsChecked;
                            if (saturday.IsChecked != null)
                                rp.IsWeeklySaturday = (bool)saturday.IsChecked;

                        }
                        else if (RecurrenceMonthly)
                        {
                            int everyNMonths = 1;
                            double months;
                            bool intCheck = false;
                            bool isDouble = Double.TryParse((EveryMonthGap.Value).ToString(), out months);
                            if (isDouble)
                                intCheck = int.TryParse((Math.Round(months)).ToString(), out everyNMonths);
                            if (!intCheck)
                            {
                                string NMonths = (EveryDayGap.Value).ToString();
                                rp.MonthlyEveryNMonths = int.Parse(NMonths);
                            }
                            else
                            {
                                rp.MonthlyEveryNMonths = everyNMonths;
                            }
                            rp.RecurrenceType = RecurrenceType.Monthly;
                            if (EveryXMonths.IsChecked != null)
                                rp.IsMonthlySpecific = (bool)EveryXMonths.IsChecked;
                            if (rp.IsMonthlySpecific)
                            {
                                rp.MonthlySpecificMonthDay = EveryMonthDay.SelectedIndex + 1;
                            }
                            else
                            {
                                rp.MonthlyNthWeek = Howmanyth.SelectedIndex + 1;
                                rp.MonthlyWeekDay = Weekday.SelectedIndex + 1;
                            }
                        }
                        else if (RecurrenceYearly)
                        {
                            int everyNYears = 1;
                            double years;
                            bool intCheck = false;
                            bool isDouble = Double.TryParse((EveryYearGap.Value).ToString(), out years);
                            if (isDouble)
                                intCheck = int.TryParse((Math.Round(years)).ToString(), out everyNYears);
                            if (!intCheck)
                            {
                                string NYears = (EveryDayGap.Value).ToString();
                                rp.YearlyEveryNYears = int.Parse(NYears);
                            }
                            else
                            {
                                rp.YearlyEveryNYears = everyNYears;
                            }
                            rp.RecurrenceType = RecurrenceType.Yearly;
                            if (SpecificDate.IsChecked != null)
                                rp.IsYearlySpecific = (bool)SpecificDate.IsChecked;
                            if (rp.IsYearlySpecific)
                            {
                                rp.YearlySpecificMonth = YrSpMonth.SelectedIndex + 1;
                                rp.YearlySpecificMonthDay = YrSpDate.SelectedIndex + 1;
                            }
                            else
                            {
                                rp.YearlyNthWeek = YrHowmanyth.SelectedIndex + 1;
                                rp.YearlyWeekDay = YrWeekday.SelectedIndex + 1;
                                rp.YearlyGenericMonth = YrMonth.SelectedIndex + 1;
                            }
                        }
                        rp.RangeStartDate = ((DateTime)RangeStartdate.Value).Date;
                        rp.IsRangeRecurrenceCount = (bool)EndAfter.IsChecked;
                        if (rp.IsRangeRecurrenceCount)
                        {
                            int recCount = 1;
                            double count;
                            bool intCheck = false;
                            bool isDouble = Double.TryParse((EndAfterCount.Value).ToString(), out count);
                            if (isDouble)
                                intCheck = int.TryParse((Math.Round(count)).ToString(), out recCount);
                            if (!intCheck)
                            {
                                string strCount = (EveryDayGap.Value).ToString();
                                rp.RangeRecurrenceCount = int.Parse(strCount);
                            }
                            else
                            {
                                rp.RangeRecurrenceCount = recCount;
                            }
                            //rp.RangeRecurrenceCount = int.TryParse((Math.Round((double)EndAfterCount.Value)).ToString(), out recCount) ? int.Parse((Math.Round((double)EndAfterCount.Value)).ToString()) : (int)EndAfterCount.Value;
                        }
                        rp.IsRangeEndDate = (bool)EndBy.IsChecked;
                        if (rp.IsRangeEndDate)
                        {
                            rp.RangeEndDate = (DateTime)RangeEnddate.Value;
                        }
                        rp.IsRangeNoEndDate = (bool)NoEndDate.IsChecked;

                        schedule.IsRRuleSetInternally = true;
                        NewAppointment.hasInternalRule = true;
                        NewAppointment.RecurrenceRule = ScheduleHelper.RRuleGenerator(rp, NewAppointment.InternalStartTime, NewAppointment.InternalEndTime);
                        schedule.IsRRuleSetInternally = false;
                        NewAppointment.hasInternalRule = false;
                        NewAppointment.RecurrenceProperites = rp;
                    }

                    #endregion
                    if (AppointmentResourceName != null)
                    {
                        foreach (Resource selectedresrc in AppointmentResourceName)
                        {
                            NewAppointment.ResourceCollection.Add(selectedresrc);
                        }
                    }
                    if (ReminderTimeComboBox.SelectedValue != null)
                        NewAppointment.ReminderTime = (ReminderTimeType)ReminderTimeComboBox.SelectedValue;
                    if (NewAppointment.ReminderTime == ReminderTimeType.None)
                    {
                        NewAppointment.ReminderDeliveryTime = null;
                    }
                    else
                    {
                        NewAppointment.ReminderDeliveryTime = NewAppointment.InternalStartTime - NewAppointment.MeasureTime(NewAppointment.ReminderTime);
                    }

                    if (schedule.Appointments == null)
                    {
                        schedule.Appointments = new ScheduleAppointmentCollection();
                    }
                    closedEventArgs.EditedAppointment = NewAppointment;
                    schedule.GetAppointmentEditorClosedEvents(closedEventArgs);
                    if (!closedEventArgs.Cancel)
                        schedule.Appointments.Add(NewAppointment);
                }
                ResetEditor();
                schedule.CloseAnimation();
            }
            else
            {
                IsWarningMsgVisible = Visibility.Visible;
            }
        }

        private void EditSelectedAppointment(SfSchedule schedule, ScheduleAppointment selectedAppointment, RecurrenceProperties rp)
        {
            //Checking weather the new value is not equal to old endtime value
            if (end != selectedAppointment.EndTime)
            {
                schedule.stopUpdate = true;
            }
            selectedAppointment.StartTime = start;
            schedule.stopUpdate = false;
            selectedAppointment.EndTime = end;
            if(AppointmentStatus.SelectedValue != null)
                selectedAppointment.Status = AppointmentStatusCollection.First(appointmentStatus => appointmentStatus.Status.Equals(AppointmentStatus.SelectedValue.ToString()));
            selectedAppointment.Notes = notes.Text;
            selectedAppointment.Subject = subject.Text;
            if (allday.IsChecked != null)
                selectedAppointment.AllDay = (bool)(allday.IsChecked);
            selectedAppointment.Location = location.Text;
            selectedAppointment.ReadOnly = readOnly.IsOn;
            selectedAppointment.IsRecursive = false;
            selectedAppointment.IsRecursiveModified = IsRecursiveModified;
            if (starttimecombo.SelectedItem != null)
            {
                selectedAppointment.StartTimeZone = (starttimecombo.SelectedItem as TimeZone);
            }
            if (endtimecombo.SelectedItem != null)
            {
                selectedAppointment.EndTimeZone = (endtimecombo.SelectedItem as TimeZone);
            }
            if (ReminderTimeComboBox.SelectedValue != null)
                selectedAppointment.ReminderTime = (ReminderTimeType)ReminderTimeComboBox.SelectedValue;
            if (selectedAppointment.ReminderTime == ReminderTimeType.None)
            {
                selectedAppointment.ReminderDeliveryTime = null;
            }
            else
            {
                selectedAppointment.ReminderDeliveryTime = selectedAppointment.InternalStartTime - selectedAppointment.MeasureTime(selectedAppointment.ReminderTime);
            }
            #region Recurrence

            if (isRecursive.IsOn)
            {
                #region Daily
                if (RecurrenceDaily)
                {
                    int everyNDays = 1;
                    double days;
                    bool intCheck = false;
                    bool isDouble = Double.TryParse((EveryDayGap.Value).ToString(), out days);
                    if (isDouble)
                        intCheck = int.TryParse((Math.Round(days)).ToString(), out everyNDays);
                    if (!intCheck)
                    {
                        string NDays = (EveryDayGap.Value).ToString();
                        rp.DailyNDays = int.Parse(NDays);
                    }
                    else
                    {
                        rp.DailyNDays = everyNDays;
                    }
                    rp.RecurrenceType = RecurrenceType.Daily;
                    if (EveryXDays.IsChecked != null)
                        rp.IsDailyEveryNDays = (bool)EveryXDays.IsChecked;
                }
                #endregion
                #region Weekly
                else if (RecurrenceWeekly)
                {
                    int everyNWeeks = 1;
                    double weeks;
                    bool intCheck = false;
                    bool isDouble = Double.TryParse((EveryWeekGap.Value).ToString(), out weeks);
                    if (isDouble)
                        intCheck = int.TryParse((Math.Round(weeks)).ToString(), out everyNWeeks);
                    if (!intCheck)
                    {
                        string NWeeks = (EveryDayGap.Value).ToString();
                        rp.WeeklyEveryNWeeks = int.Parse(NWeeks);
                    }
                    else
                    {
                        rp.WeeklyEveryNWeeks = everyNWeeks;
                    }
                    rp.RecurrenceType = RecurrenceType.Weekly;
                    //rp.WeeklyEveryNWeeks = int.TryParse((Math.Round((double)EveryWeekGap.Value)).ToString(), out everyNWeeks) ? int.Parse((Math.Round((double)EveryWeekGap.Value)).ToString()) : (int)EveryWeekGap.Value;
                    if (sunday.IsChecked != null)
                        rp.IsWeeklySunday = (bool)sunday.IsChecked;
                    if (monday.IsChecked != null)
                        rp.IsWeeklyMonday = (bool)monday.IsChecked;
                    if (tuesday.IsChecked != null)
                        rp.IsWeeklyTuesday = (bool)tuesday.IsChecked;
                    if (wednesday.IsChecked != null)
                        rp.IsWeeklyWednesday = (bool)wednesday.IsChecked;
                    if (thursday.IsChecked != null)
                        rp.IsWeeklyThursday = (bool)thursday.IsChecked;
                    if (friday.IsChecked != null)
                        rp.IsWeeklyFriday = (bool)friday.IsChecked;
                    if (saturday.IsChecked != null)
                        rp.IsWeeklySaturday = (bool)saturday.IsChecked;

                }
                #endregion
                #region Monthly
                else if (RecurrenceMonthly)
                {
                    int everyNMonths = 1;
                    double months;
                    bool intCheck = false;
                    bool isDouble = Double.TryParse((EveryMonthGap.Value).ToString(), out months);
                    if (isDouble)
                        intCheck = int.TryParse((Math.Round(months)).ToString(), out everyNMonths);
                    if (!intCheck)
                    {
                        string NMonths = (EveryDayGap.Value).ToString();
                        rp.MonthlyEveryNMonths = int.Parse(NMonths);
                    }
                    else
                    {
                        rp.MonthlyEveryNMonths = everyNMonths;
                    }
                    rp.RecurrenceType = RecurrenceType.Monthly;
                    if (EveryXMonths.IsChecked != null)
                        rp.IsMonthlySpecific = (bool)EveryXMonths.IsChecked;
                    if (rp.IsMonthlySpecific)
                    {
                        rp.MonthlySpecificMonthDay = EveryMonthDay.SelectedIndex + 1;
                    }
                    else
                    {
                        rp.MonthlyNthWeek = Howmanyth.SelectedIndex + 1;
                        rp.MonthlyWeekDay = Weekday.SelectedIndex + 1;
                    }
                }
                #endregion
                #region Yearly
                else if (RecurrenceYearly)
                {
                    int everyNYears = 1;
                    double years;
                    bool intCheck = false;
                    bool isDouble = Double.TryParse((EveryYearGap.Value).ToString(), out years);
                    if (isDouble)
                        intCheck = int.TryParse((Math.Round(years)).ToString(), out everyNYears);
                    if (!intCheck)
                    {
                        string NYears = (EveryDayGap.Value).ToString();
                        rp.YearlyEveryNYears = int.Parse(NYears);
                    }
                    else
                    {
                        rp.YearlyEveryNYears = everyNYears;
                    }
                    rp.RecurrenceType = RecurrenceType.Yearly;
                    if (SpecificDate.IsChecked != null) rp.IsYearlySpecific = (bool)SpecificDate.IsChecked;
                    if (rp.IsYearlySpecific)
                    {
                        rp.YearlySpecificMonth = YrSpMonth.SelectedIndex + 1;
                        rp.YearlySpecificMonthDay = YrSpDate.SelectedIndex + 1;
                    }
                    else
                    {
                        rp.YearlyNthWeek = YrHowmanyth.SelectedIndex + 1;
                        rp.YearlyWeekDay = YrWeekday.SelectedIndex + 1;
                        rp.YearlyGenericMonth = YrMonth.SelectedIndex + 1;
                    }
                }
                #endregion

                rp.RangeStartDate = (DateTime)RangeStartdate.Value;
                rp.IsRangeRecurrenceCount = (bool)EndAfter.IsChecked;
                if (rp.IsRangeRecurrenceCount)
                {
                    int recCount = 1;
                    double count;
                    bool intCheck = false;
                    bool isDouble = Double.TryParse((EndAfterCount.Value).ToString(), out count);
                    if (isDouble)
                        intCheck = int.TryParse((Math.Round(count)).ToString(), out recCount);
                    if (!intCheck)
                    {
                        string strCount = (EveryDayGap.Value).ToString();
                        rp.RangeRecurrenceCount = int.Parse(strCount);
                    }
                    else
                    {
                        rp.RangeRecurrenceCount = recCount;
                    }
                }
                rp.IsRangeEndDate = (bool)EndBy.IsChecked;
                if (rp.IsRangeEndDate)
                {
                    rp.RangeEndDate = (DateTime)RangeEnddate.Value;
                }
                rp.IsRangeNoEndDate = (bool)NoEndDate.IsChecked;
                selectedAppointment.IsRecursive = isRecursive.IsOn;
                schedule.IsRRuleSetInternally = true;
                selectedAppointment.hasInternalRule = true;
                selectedAppointment.RecurrenceRule = ScheduleHelper.RRuleGenerator(rp, selectedAppointment.InternalStartTime, selectedAppointment.InternalEndTime);
                schedule.IsRRuleSetInternally = false;
                selectedAppointment.hasInternalRule = false;
                selectedAppointment.RecurrenceProperites = rp;
            }
            else
                selectedAppointment.RecurrenceProperites = new RecurrenceProperties();

            #endregion
        }

        #endregion

        #region Close Appointment

        void CloseAppointment_Click(object sender, RoutedEventArgs e)
        {
            ResetEditor();
            var schedule = this.FindParentElementOfType<SfSchedule>();
            schedule.CloseAnimation();
            closedEventArgs.OriginalAppointment = schedule.SelectedAppointment;
            closedEventArgs.EditedAppointment = null;
            closedEventArgs.Action = EditorClosedAction.Cancel;
            schedule.GetAppointmentEditorClosedEvents(closedEventArgs);
        }

        #endregion

        #endregion

        #region Methods

        #region ResetEditor

        void ResetEditor()
        {
            subject.Text = "";
            notes.Text = "";
            location.Text = "";
            EveryYearGap.Value = 1;
            isRecursive.IsOn = false;
            EveryDayGap.Value = 1;
            EndAfterCount.Value = 1;
            EveryMonthGap.Value = 1;
            RangeStartdate.Value = DateTime.Today;
            RangeEnddate.Value = DateTime.Today.AddDays(1);
            EveryWeekGap.Value = 1;
            readOnly.IsOn = false;
            Startdate.Value = DateTime.Today;
            Starttime.Value = DateTime.Today;
            Enddate.Value = DateTime.Today.AddDays(1);
            Endtime.Value = DateTime.Today.AddDays(1);
            allday.IsChecked = false;
            if (AppointmentStatus.Items != null && AppointmentStatus.Items.Count > 0)
                AppointmentStatus.SelectedIndex = 0;
            ReminderTimeComboBox.SelectedIndex = 0;
            starttimecombo.SelectedIndex = 32;
            endtimecombo.SelectedIndex = 32;
            EveryXDays.IsChecked = true;
            sunday.IsChecked = true;
            monday.IsChecked = false;
            tuesday.IsChecked = false;
            wednesday.IsChecked = false;
            thursday.IsChecked = false;
            friday.IsChecked = false;
            saturday.IsChecked = false;
            EveryXMonths.IsChecked = true;
            EveryMonthDay.SelectedIndex = 0;
            Howmanyth.SelectedIndex = 0;
            Weekday.SelectedIndex = 0;
            SpecificDate.IsChecked = true;
            YrSpMonth.SelectedIndex = 0;
            YrSpDate.SelectedIndex = 0;
            YrHowmanyth.SelectedIndex = 0;
            YrWeekday.SelectedIndex = 0;
            YrMonth.SelectedIndex = 0;
            EndAfter.IsChecked = true;
            EndBy.IsChecked = false;
            NoEndDate.IsChecked = false;
            EndStackPanelVisibility = Visibility.Collapsed;
            StartStackPanelVisibility = Visibility.Collapsed;
        }

        #endregion

        #region Localization Related Methods

        public object this[string key]
        {
            get
            {
                if (AppointmentData.Keys.Contains(key))
                {
                    return AppointmentData[key];
                }
                return null;
            }
            set
            {
                if (AppointmentData.Keys.Contains(key))
                {
                    AppointmentData[key] = value.ToString();
                }
                else
                {
                    AppointmentData.Add(key, value.ToString());
                }
            }
        }

        private void LoadLocalizedData()
        {
            AppointmentData = new Dictionary<string, string>
                {
                    {"AddEvent", GetLocalizedString("Add Event")},
                    {"Subject", GetLocalizedString("Subject")},
                    {"Notes", GetLocalizedString("Notes")},
                    {"Start", GetLocalizedString("Start")},
                    {"All_Day", GetLocalizedString("All Day")},
                    {"End", GetLocalizedString("End")},
                    {"Where", GetLocalizedString("Where")},
                    {"Location", GetLocalizedString("Location")},
                    {"Show_More", GetLocalizedString("Show More")},
                    {"How_Often", GetLocalizedString("How Often")},
                    {"Reminder", GetLocalizedString("Reminder")},
                    {"Status", GetLocalizedString("Status")},
                    {"ReadOnly_On", GetLocalizedString("ReadOnly On")},
                    {"ReadOnly_Off", GetLocalizedString("ReadOnly Off")},
                    {"Save", GetLocalizedString("Save")},
                    {"Delete", GetLocalizedString("Delete")},
                    {"Cancel", GetLocalizedString("Cancel")},
                    {"RecurrenceEnabled", GetLocalizedString("Recurrence Enabled")},
                    {"RecurrenceDisabled", GetLocalizedString("Recurrence Disabled")},
                    {"Daily", GetLocalizedString("Daily")},
                    {"Weekly", GetLocalizedString("Weekly")},
                    {"Monthly", GetLocalizedString("Monthly")},
                    {"Yearly", GetLocalizedString("Yearly")},
                    {"Every", GetLocalizedString("Every")},
                    {"Day(s)", GetLocalizedString("Day(s)")},
                    {"EveryWeekDays", GetLocalizedString("EveryWeekDays")},
                    {"RecurEvery", GetLocalizedString("Recur Every")},
                    {"Week(s)on", GetLocalizedString("Week(s) on")},
                    {"on", GetLocalizedString("on")},
                    {"of", GetLocalizedString("of")},
                    {"Sunday", GetLocalizedString("Sunday")},
                    {"Monday", GetLocalizedString("Monday")},
                    {"Tuesday", GetLocalizedString("Tuesday")},
                    {"Wednesday", GetLocalizedString("Wednesday")},
                    {"Thursday", GetLocalizedString("Thursday")},
                    {"Friday", GetLocalizedString("Friday")},
                    {"Saturday", GetLocalizedString("Saturday")},
                    {"Month(s)on", GetLocalizedString("Month(s) on")},
                    {"Day", GetLocalizedString("Day")},
                    {"The", GetLocalizedString("The")},
                    {"First", GetLocalizedString("First")},
                    {"Second", GetLocalizedString("Second")},
                    {"Third", GetLocalizedString("Third")},
                    {"Fourth", GetLocalizedString("Fourth")},
                    {"Year(s)on", GetLocalizedString("Year(s) on")},
                    {"January", GetLocalizedString("January")},
                    {"February", GetLocalizedString("February")},
                    {"March", GetLocalizedString("March")},
                    {"April", GetLocalizedString("April")},
                    {"May", GetLocalizedString("May")},
                    {"June", GetLocalizedString("June")},
                    {"July", GetLocalizedString("July")},
                    {"August", GetLocalizedString("August")},
                    {"September", GetLocalizedString("September")},
                    {"October", GetLocalizedString("October")},
                    {"November", GetLocalizedString("November")},
                    {"December", GetLocalizedString("December")},
                    {"Range_Of_Recurrence", GetLocalizedString("Range Of Recurrence")},
                    {"Start_Date", GetLocalizedString("Start Date")},
                    {"End_After", GetLocalizedString("End After")},
                    {"Recurrence", GetLocalizedString("Recurrence")},
                    {"End_By", GetLocalizedString("End By")},
                    {"No_End_Date", GetLocalizedString("No End Date")},
                    {"Deleted", GetLocalizedString("Deleted")}
                };
        }

        private string GetLocalizedString(string localString)
        {
            string result;
            var resourceStringMap = ResourceManager.Current.MainResourceMap.GetSubtree(CultureInfo.CurrentCulture.Name);
            if (resourceStringMap != null)
            {
                result = resourceStringMap.Keys.Contains(localString) ? resourceStringMap.GetValue(localString, new ResourceContext()).ValueAsString : localString;
            }
            else
            {
                result = localString;
            }
            return result;
        }

        private ObservableCollection<LocalizationMember> GetLocalizedArrayValues(Array array)
        {
            var recurrenceMembers = new ObservableCollection<LocalizationMember>();
            foreach (object obj in array)
            {
                recurrenceMembers.Add(new LocalizationMember { ActualValue = obj, DisplayMember = GetLocalizedString(obj.ToString()) });

            }
            return recurrenceMembers;
        }

        #endregion

        #region Setting New Appointment Properties

        internal void SetNewAppointmentProperties(SfSchedule sfSchedule, DateTime startDate, DateTime endDate)
        {
            closedEventArgs.IsNew = true;
            var newApp = new ScheduleAppointment { StartTime = startDate, EndTime = endDate, ReadOnly = false };
            start = sfSchedule.allDayFlag ? startDate.Date : startDate;
            end = sfSchedule.allDayFlag ? startDate.Date : endDate;
            if (RangeStartdate != null)
                RangeStartdate.Value = startDate;
            if (RangeEnddate != null)
                RangeEnddate.Value = startDate.AddDays(1);

            if (sfSchedule.ScheduleType == ScheduleType.Month || sfSchedule.allDayFlag)
            {
                newApp.AllDay = true;
                AllDay = true;
                sfSchedule.allDayFlag = false;
            }
            else
                newApp.AllDay = false;
            if (starttimecombo != null)
            {
                TimeZone tmzo = sfSchedule.TimeZoneCollection.FirstOrDefault(tz => tz.TimeZoneValue == newApp.StartTimeZone.TimeZoneValue);
                starttimecombo.SelectedIndex = sfSchedule.TimeZoneCollection.IndexOf(tmzo);
            }
            if (endtimecombo != null)
            {
                TimeZone tmzo = sfSchedule.TimeZoneCollection.FirstOrDefault(tz => tz.TimeZoneValue == newApp.EndTimeZone.TimeZoneValue);
                endtimecombo.SelectedIndex = sfSchedule.TimeZoneCollection.IndexOf(tmzo);
            }
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
            if (sfSchedule.ScheduleResourceType != null && sfSchedule.ScheduleResourceType.ResourceCollection.Count > 1)
            {
                // need to add an appointment editor property that hides the Resource combo box when the SchduleRecource count is less than one.
                //Resources = ScheduleResourceType.ResourceCollection;
            }
            InternalStartTime = startDate;
            InternalEndTime = endDate;
            var args = new AppointmentEditorOpeningEventArgs
            {
                StartTime = startDate,
                SelectedResource = sfSchedule.selectedResourcename,
                Action = EditorAction.Add
            };
            sfSchedule.GetAppointmentEditorOpeningEvents(args);
        }

        #endregion

        #region Updating Appointment Properties

        internal void UpdateAppointmentProperties(SfSchedule sfSchedule, ScheduleAppointment appointment)
        {
            closedEventArgs.IsNew = false;
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
            if (subject != null)
                subject.Text = appointment.Subject;
            if (notes != null)
                notes.Text = appointment.Notes;
            if (location != null)
                location.Text = appointment.Location;
            if (isRecursive != null)
                isRecursive.IsOn = appointment.IsRecursive;
            if (readOnly != null)
                readOnly.IsOn = appointment.ReadOnly;
            if (starttimecombo != null)
            {
                TimeZone tmzo = sfSchedule.TimeZoneCollection.FirstOrDefault(tz => tz.TimeZoneValue == appointment.StartTimeZone.TimeZoneValue);
                starttimecombo.SelectedIndex = sfSchedule.TimeZoneCollection.IndexOf(tmzo);
            }
            if (endtimecombo != null)
            {
                TimeZone tmzo = sfSchedule.TimeZoneCollection.FirstOrDefault(tz => tz.TimeZoneValue == appointment.EndTimeZone.TimeZoneValue);
                endtimecombo.SelectedIndex = sfSchedule.TimeZoneCollection.IndexOf(tmzo);
            }
            if (appointment.RecurrenceProperites != null)
            {
                if (appointment.RecurrenceProperites.RecurrenceType == RecurrenceType.Daily)
                {
                    RecurrenceDaily = true;
                    RecurrenceWeekly = false;
                    RecurrenceMonthly = false;
                    RecurrenceYearly = false;

                    if (EveryXDays != null)
                        EveryXDays.IsChecked = appointment.RecurrenceProperites.IsDailyEveryNDays;
                    if (EveryDayGap != null)
                        EveryDayGap.Value = appointment.RecurrenceProperites.DailyNDays;
                }
                else if (appointment.RecurrenceProperites.RecurrenceType == RecurrenceType.Weekly)
                {
                    RecurrenceDaily = false;
                    RecurrenceWeekly = true;
                    RecurrenceMonthly = false;
                    RecurrenceYearly = false;

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
                    RecurrenceDaily = false;
                    RecurrenceWeekly = false;
                    RecurrenceMonthly = true;
                    RecurrenceYearly = false;

                    if (EveryXMonths != null)
                        EveryXMonths.IsChecked = appointment.RecurrenceProperites.IsMonthlySpecific;
                    if (EveryMonthGap != null)
                        EveryMonthGap.Value = appointment.RecurrenceProperites.MonthlyEveryNMonths;
                    if (appointment.RecurrenceProperites.IsMonthlySpecific)
                    {
                        if (EveryMonthDay != null)
                            EveryMonthDay.SelectedIndex = appointment.RecurrenceProperites.MonthlySpecificMonthDay - 1;
                    }
                    if (!appointment.RecurrenceProperites.IsMonthlySpecific)
                    {
                        if (Howmanyth != null)
                            Howmanyth.SelectedIndex = appointment.RecurrenceProperites.MonthlyNthWeek - 1;
                    }
                    if (Weekday != null)
                        Weekday.SelectedIndex = appointment.RecurrenceProperites.MonthlyWeekDay - 1;
                }
                else if (appointment.RecurrenceProperites.RecurrenceType == RecurrenceType.Yearly)
                {
                    RecurrenceDaily = false;
                    RecurrenceWeekly = false;
                    RecurrenceMonthly = false;
                    RecurrenceYearly = true;

                    if (SpecificDate != null)
                        SpecificDate.IsChecked = appointment.RecurrenceProperites.IsYearlySpecific;
                    if (EveryYearGap != null)
                        EveryYearGap.Value = appointment.RecurrenceProperites.YearlyEveryNYears;
                    if (appointment.RecurrenceProperites.IsYearlySpecific)
                    {
                        if (YrSpDate != null)
                            YrSpDate.SelectedIndex = appointment.RecurrenceProperites.YearlySpecificMonthDay - 1;
                        if (YrSpMonth != null)
                            YrSpMonth.SelectedIndex = appointment.RecurrenceProperites.YearlySpecificMonth - 1;
                    }
                    if (!appointment.RecurrenceProperites.IsYearlySpecific)
                    {
                        if (YrHowmanyth != null)
                            YrHowmanyth.SelectedIndex = appointment.RecurrenceProperites.YearlyNthWeek - 1;
                        if (YrMonth != null)
                            YrMonth.SelectedIndex = appointment.RecurrenceProperites.YearlyGenericMonth - 1;
                    }
                    if (YrWeekday != null)
                        YrWeekday.SelectedIndex = appointment.RecurrenceProperites.YearlyWeekDay - 1;

                }
                RecurrenceType = appointment.RecurrenceProperites.RecurrenceType;
                if (RangeStartdate != null)
                    RangeStartdate.Value = appointment.RecurrenceProperites.RangeStartDate;
                if (RangeEnddate != null)
                    RangeEnddate.Value = appointment.RecurrenceProperites.RangeEndDate;
                if (EndAfter != null)
                    EndAfter.IsChecked = appointment.RecurrenceProperites.IsRangeRecurrenceCount;
                if (EndBy != null)
                    EndBy.IsChecked = appointment.RecurrenceProperites.IsRangeEndDate;
                if (NoEndDate != null)
                    NoEndDate.IsChecked = appointment.RecurrenceProperites.IsRangeNoEndDate;
                if (EndAfterCount != null)
                    EndAfterCount.Value = appointment.RecurrenceProperites.RangeRecurrenceCount;

            }

            DeleteButtonVisibility = Visibility.Visible;
            InternalStartTime = appointment.InternalStartTime;
            InternalEndTime = appointment.InternalEndTime;
            TimeZoneCollection = sfSchedule.TimeZoneCollection;
            AllDay = appointment.AllDay;
            AppointmentStatusCollection = sfSchedule.AppointmentStatusCollection;
            start = appointment.StartTime;
            end = appointment.EndTime;
            StartTimeZone = appointment.StartTimeZone;
            EndTimeZone = appointment.EndTimeZone;
            IsRecursiveModified = false;
            IsRecurrenceTypeEnabled = true;
            IsWarningMsgVisible = Visibility.Collapsed;
            ShowMoreVisibility = Visibility.Visible;
            ShowMoreStackPanelVisibility = Visibility.Collapsed;
            ReminderTime = appointment.ReminderTime;
            if (AppointmentStatus != null && AppointmentStatusCollection != null && AppointmentStatusCollection.Count > 0 && AppointmentStatus.Items.Count > 0)
                AppointmentStatus.SelectedIndex = AppointmentStatusCollection.IndexOf(appointment.Status);
            if (recursiveModified == 1)
            {
                IsRecursiveModified = true;
                IsRecurrenceTypeEnabled = false;
            }
            if (sfSchedule.ScheduleResourceType != null && sfSchedule.ScheduleResourceType.ResourceCollection.Count > 1)
            {
                // need to add an appointment editor property that hides the Resource combo box when the SchduleRecource count is less than one.
                //Resources = ScheduleResourceType.ResourceCollection;
            }
            DeleteButtonVisibility = Visibility.Visible;
            var args = new AppointmentEditorOpeningEventArgs { Appointment = appointment };
            if (sfSchedule.ItemsSource != null)
            {
                var source = sfSchedule.ItemsSource as IEnumerable<object>;
                object obj = source.FirstOrDefault(x => x != null && x.GetHashCode() == (int)appointment.ObjectID);
                if (obj != null)
                {
                    args.Appointment = obj;
                }
            }

            args.Action = EditorAction.Edit;
            args.SelectedResource = appointment.ResourceCollection.ToList();
            args.StartTime = appointment.StartTime;
            sfSchedule.GetAppointmentEditorOpeningEvents(args);
        }

        #endregion

        public void Dispose()
        {
            if (StartBorder != null)
            {
                StartBorder.PointerEntered -= StartBorder_PointerEntered;
                StartBorder.PointerExited -= StartBorder_PointerExited;
            }
            if (EndBorder != null)
            {
                EndBorder.PointerEntered -= EndBorder_PointerEntered;
                EndBorder.PointerExited -= EndBorder_PointerExited;
            }
            if (EmptySpace != null)
            {
                EmptySpace.PointerPressed -= EmptySpace_PointerPressed;
            }
            if (Startdate != null)
                Startdate.ValueChanged -= Startdate_ValueChanged;
            if (EndAfter != null)
                EndAfter.Checked -= EndDay_Checked;
            if (EndBy != null)
                EndBy.Checked -= EndDay_Checked;
            if (NoEndDate != null)
                NoEndDate.Checked -= EndDay_Checked;
            if (EveryMonth != null)
                EveryMonth.Checked -= EveryMonths_Checked;
            if (EveryXMonths != null)
                EveryXMonths.Checked -= EveryMonths_Checked;
            if (RangeStartdate != null)
                RangeStartdate.ValueChanged -= RangeStartdate_ValueChanged;
            if (ShowMore != null)
                ShowMore.PointerPressed -= ShowMore_PointerPressed;
            if (StartGrid != null)
                StartGrid.PointerPressed -= StartGrid_PointerPressed;
            if (EndGrid != null)
                EndGrid.PointerPressed -= EndGrid_PointerPressed;
            if (isRecursive != null)
                isRecursive.Toggled -= isRecursive_Toggled;
            if (CloseAppointment != null)
                CloseAppointment.Click -= CloseAppointment_Click;
            if (DoneAppointment != null)
                DoneAppointment.Click -= DoneAppointment_Click;
            if (DeleteAppointment != null)
                DeleteAppointment.Click -= DeleteAppointment_Click;
        }

        #endregion

        #region Override Methods

        #region OnApplyTemplate

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            StartBorder = GetTemplateChild("StartBorder") as Border;
            if (StartBorder != null)
            {
                StartBorder.PointerEntered += StartBorder_PointerEntered;
                StartBorder.PointerExited += StartBorder_PointerExited;
            }
            EndBorder = GetTemplateChild("EndBorder") as Border;
            if (EndBorder != null)
            {
                EndBorder.PointerEntered += EndBorder_PointerEntered;
                EndBorder.PointerExited += EndBorder_PointerExited;
            }
            starttimecombo = GetTemplateChild("starttimezone") as ComboBox;
            if (starttimecombo != null)
            {
                if (EditedAppointment.StartTimeZone != null)
                {
                    TimeZone tmzo = TimeZoneCollection.FirstOrDefault(tz => tz.TimeZoneValue == EditedAppointment.StartTimeZone.TimeZoneValue);
                    starttimecombo.SelectedIndex = TimeZoneCollection.IndexOf(tmzo);
                }
            }
            endtimecombo = GetTemplateChild("endtimezone") as ComboBox;
            if (endtimecombo != null)
            {
                if (EditedAppointment.EndTimeZone != null)
                {
                    TimeZone tmzo = TimeZoneCollection.FirstOrDefault(tz => tz.TimeZoneValue == EditedAppointment.EndTimeZone.TimeZoneValue);
                    endtimecombo.SelectedIndex = TimeZoneCollection.IndexOf(tmzo);
                }
            }

            EmptySpace = GetTemplateChild("EmptySpace") as Grid;
            if (EmptySpace != null)
            {
                EmptySpace.PointerPressed += EmptySpace_PointerPressed;
            }
            allday = GetTemplateChild("AllDay") as CheckBox;
            CloseAppointment = GetTemplateChild("Close") as Button;
            DoneAppointment = GetTemplateChild("Done") as Button;
            DeleteAppointment = GetTemplateChild("Delete") as Button;
            readOnly = GetTemplateChild("ReadOnly") as ToggleSwitch;
            Startdate = GetTemplateChild("Startdate") as SfDatePicker;
            if (Startdate != null)
                Startdate.ValueChanged += Startdate_ValueChanged;
            Starttime = GetTemplateChild("Starttime") as SfTimePicker;
            Endtime = GetTemplateChild("Endtime") as SfTimePicker;
            Enddate = GetTemplateChild("Enddate") as SfDatePicker;
            notes = GetTemplateChild("Notes") as SfTextBoxExt;
            location = GetTemplateChild("Location") as SfTextBoxExt;
            subject = GetTemplateChild("Subject") as SfTextBoxExt;
            EveryDayGap = GetTemplateChild("EveryDayGap") as SfNumericUpDown;
            EveryWeekGap = GetTemplateChild("EveryWeekGap") as SfNumericUpDown;
            EveryMonthGap = GetTemplateChild("EveryMonthGap") as SfNumericUpDown;
            EveryYearGap = GetTemplateChild("EveryYearGap") as SfNumericUpDown;
            EndAfterCount = GetTemplateChild("EndAfterCount") as SfNumericUpDown;
            EndAfter = GetTemplateChild("EndAfter") as RadioButton;
            if (EndAfter != null)
            {
                EndAfter.IsChecked = true;
                EndAfter.Checked += EndDay_Checked;
            }

            EndBy = GetTemplateChild("EndBy") as RadioButton;
            if (EndBy != null)
                EndBy.Checked += EndDay_Checked;

            NoEndDate = GetTemplateChild("NoEndDate") as RadioButton;
            if (NoEndDate != null)
                NoEndDate.Checked += EndDay_Checked;

            RangeStartdate = GetTemplateChild("RangeStartdate") as SfDatePicker;
            if (RangeStartdate != null)
                RangeStartdate.ValueChanged += RangeStartdate_ValueChanged;
            RangeEnddate = GetTemplateChild("RangeEnddate") as SfDatePicker;
            if (RangeEnddate != null && RangeEnddate.Value != null)
            {
                RangeEnddate.IsEnabled = (bool)EndBy.IsChecked;
                RangeEnddate.Value = ((DateTime)(RangeStartdate.Value)).AddDays(1);
            }
            ShowMore = GetTemplateChild("ShowMore") as Grid;
            StartGrid = GetTemplateChild("StartGrid") as Grid;
            EndGrid = GetTemplateChild("EndGrid") as Grid;
            EveryXDays = GetTemplateChild("EveryXDays") as RadioButton;
            if (EveryXDays != null)
                EveryXDays.IsChecked = true;

            monday = GetTemplateChild("monday") as CheckBox;
            tuesday = GetTemplateChild("tuesday") as CheckBox;
            wednesday = GetTemplateChild("wednesday") as CheckBox;
            thursday = GetTemplateChild("thursday") as CheckBox;
            friday = GetTemplateChild("friday") as CheckBox;
            saturday = GetTemplateChild("saturday") as CheckBox;
            sunday = GetTemplateChild("sunday") as CheckBox;
            if (EditedAppointment.RecurrenceProperites == null && sunday != null)
            {
                sunday.IsChecked = true;
            }

            #region Monthly not specific

            Howmanyth = GetTemplateChild("Howmanyth") as ComboBox;
            if (EditedAppointment.RecurrenceProperites == null && Howmanyth != null)
            {
                Howmanyth.SelectedIndex = 0;
            }

            Weekday = GetTemplateChild("Weekday") as ComboBox;
            if (EditedAppointment.RecurrenceProperites == null && Weekday != null)
            {
                Weekday.SelectedIndex = 0;
            }

            EveryMonth = GetTemplateChild("EveryMonth") as RadioButton;
            if (EveryMonth != null)
                EveryMonth.Checked += EveryMonths_Checked;   

            #endregion

            #region Monthly specific

            EveryMonthDay = GetTemplateChild("EveryMonthDay") as ComboBox;
            if (EditedAppointment.RecurrenceProperites == null && EveryMonthDay != null)
            {
                EveryMonthDay.SelectedIndex = 0;
            }

            EveryXMonths = GetTemplateChild("EveryXMonths") as RadioButton;
            if (EveryXMonths != null)
            {
                EveryXMonths.Checked += EveryMonths_Checked;
                EveryXMonths.IsChecked = true;
            }

            #endregion                  

            #region Yearly not specific

            YrHowmanyth = GetTemplateChild("YrHowmanyth") as ComboBox;
            if (EditedAppointment.RecurrenceProperites == null && YrHowmanyth != null)
            {
                YrHowmanyth.SelectedIndex = 0;
            }

            YrWeekday = GetTemplateChild("YrWeekday") as ComboBox;
            if (EditedAppointment.RecurrenceProperites == null && YrWeekday != null)
            {
                YrWeekday.SelectedIndex = 0;
            }

            YrMonth = GetTemplateChild("YrMonth") as ComboBox;
            if (EditedAppointment.RecurrenceProperites == null && YrMonth != null)
            {
                YrMonth.SelectedIndex = 0;
            }

            EveryYear = GetTemplateChild("EveryYear") as RadioButton;
            if (EveryYear != null)
                EveryYear.Checked += EveryYear_Checked;     

            #endregion  

            #region Yearly specific

            YrSpMonth = GetTemplateChild("YrSpMonth") as ComboBox;
            if (EditedAppointment.RecurrenceProperites == null && YrSpMonth != null)
            {
                YrSpMonth.SelectedIndex = 0;
            }

            YrSpDate = GetTemplateChild("YrSpDate") as ComboBox;
            if (EditedAppointment.RecurrenceProperites == null && YrSpDate != null)
            {
                YrSpDate.SelectedIndex = 0;
            }

            SpecificDate = GetTemplateChild("SpecificDate") as RadioButton;
            if (SpecificDate != null)
            {
                SpecificDate.Checked += EveryYear_Checked;
                SpecificDate.IsChecked = true;
            }

            #endregion

            if (ShowMore != null)
                ShowMore.PointerPressed += ShowMore_PointerPressed;

            if (StartGrid != null)
                StartGrid.PointerPressed += StartGrid_PointerPressed;
            if (EndGrid != null)
                EndGrid.PointerPressed += EndGrid_PointerPressed;
            isRecursive = GetTemplateChild("isRecursive") as ToggleSwitch;
            if (isRecursive != null)
                isRecursive.Toggled += isRecursive_Toggled;
            if (EditedAppointment != null && isRecursive != null)
            {
                isRecursive.IsOn = EditedAppointment.IsRecursive;
            }

            AppointmentStatus = GetTemplateChild("Status") as ComboBox;
            if (AppointmentStatus != null && AppointmentStatusCollection != null)
            {
                AppointmentStatus.ItemsSource = GetLocalizedArrayValues(AppointmentStatusCollection.Select(appointmentStatus => appointmentStatus.Status).ToArray());
                if (AppointmentStatus.Items.Count > 0)
                    AppointmentStatus.SelectedIndex = 0;
            }
            ReminderTimeComboBox = GetTemplateChild("ReminderDeliveryTime") as ComboBox;
            if (ReminderTimeComboBox != null)
            {
                ReminderTimeComboBox.ItemsSource = GetLocalizedArrayValues(Enum.GetValues(typeof(ReminderTimeType)));
                ReminderTimeComboBox.DisplayMemberPath = "DisplayMember";
                ReminderTimeComboBox.SelectedValuePath = "ActualValue";
                for (int i = 0; i < ReminderTimeComboBox.Items.Count; i++)
                {
                    ReminderTimeComboBox.SelectedIndex = i;
                    if (ReminderTimeComboBox.SelectedValue.ToString() == ReminderTime.ToString())
                    {
                        ReminderTimeComboBox.SelectedIndex = i;
                        i = ReminderTimeComboBox.Items.Count;
                    }
                    else
                    {
                        ReminderTimeComboBox.SelectedIndex = 0;
                    }
                }
            }
            if (CloseAppointment != null)
                CloseAppointment.Click += CloseAppointment_Click;
            if (DoneAppointment != null)
                DoneAppointment.Click += DoneAppointment_Click;
            if (DeleteAppointment != null)
                DeleteAppointment.Click += DeleteAppointment_Click;
            isTemplateApplied = true;
            if (needUpdate)
            {
                SfSchedule schedule = this.FindParentElementOfType<SfSchedule>();
                if (schedule != null)
                {
                    UpdateAppointmentProperties(schedule, EditedAppointment);
                    needUpdate = false;
                }
            }
        }        

        #endregion

        #endregion
    }

    #endregion

    #region LocalizationMember

    internal class LocalizationMember
    {
        public object ActualValue
        {
            get;
            set;
        }

        public object DisplayMember
        {
            get;
            set;
        }
    }

    #endregion
}
