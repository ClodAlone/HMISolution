#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Schedule
{
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
    using System.Linq.Expressions;
    using System.Windows.Data;
    using System.Windows.Controls.Primitives;
    /// <summary>
    /// Class For ScheduleAppointment Window.
    /// </summary>
#if SyncfusionFramework4_0 && !SILVERLIGHT
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    
    public class ScheduleAppointmentEditorControl : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Controls.Schedule.ScheduleAppointmentEditorControl"/>
        /// class.
        /// </summary>
        public ScheduleAppointmentEditorControl()
        {
            this.DefaultStyleKey = typeof(ScheduleAppointmentEditorControl);
            this.GotFocus += new RoutedEventHandler(ScheduleAppointmentEditorControl_GotFocus);
      
        }
        bool first = true;

        void ScheduleAppointmentEditorControl_GotFocus(object sender, RoutedEventArgs e)
        {
            if (first)
            {
                this.SubjectTextBox.Focus();
                first = false;
            }
            
        
        }

        #region DependencyProperty

        /// <summary>
        /// Gets or sets a value indicating whether this instance either recurrence
        /// occurred or not
        /// </summary>
        /// <value>
        /// <see langword="true"/> if this instance ; otherwise, <see langword="false"/>.
        /// </value>
        public bool IsRecurrenceOccured
        {
            get { return (bool)GetValue(IsRecurrenceOccuredProperty); }
            set { SetValue(IsRecurrenceOccuredProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsRecurrenceOccured.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsRecurrenceOccuredProperty = DependencyProperty.Register("IsRecurrenceOccured",
            typeof(bool), typeof(ScheduleAppointmentEditorControl), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether this instance determines State changed or
        /// nor
        /// </summary>
        /// <value>
        /// <see langword="true"/> if this instance ; otherwise, <see langword="false"/>.
        /// </value>
        public bool IsStateChanged
        {
            get { return (bool)GetValue(IsStateChangedProperty); }
            set { SetValue(IsStateChangedProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsStateChanged.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsStateChangedProperty = DependencyProperty.Register("IsStateChanged",
            typeof(bool), typeof(ScheduleAppointmentEditorControl), new PropertyMetadata(false));


        /// <summary>
        /// Gets or sets a value indicating whether this instance determines time zone
        /// selected or not
        /// </summary>
        /// <value>
        /// <see langword="true"/> if this instance ; otherwise, <see langword="false"/>.
        /// </value>
        public bool IsTimezoneSelected
        {
            get { return (bool)GetValue(IsTimezoneSelectedProperty); }
            set { SetValue(IsTimezoneSelectedProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsTimezoneSelected.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsTimezoneSelectedProperty = DependencyProperty.Register("IsTimezoneSelected",
            typeof(bool), typeof(ScheduleAppointmentEditorControl), new PropertyMetadata(false));


        #region RecurrenceVisibility (DependencyProperty)

        /// <summary>
        /// Gets / Sets the RecurrenceVisibility property.
        /// </summary>
        public Visibility RecurrenceVisibility
        {
            get { return (Visibility)GetValue(RecurrenceVisibilityProperty); }
            set { SetValue(RecurrenceVisibilityProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for RecurrenceVisibility.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RecurrenceVisibilityProperty = DependencyProperty.Register("RecurrenceVisibility",
            typeof(Visibility), typeof(ScheduleAppointmentEditorControl), new PropertyMetadata(Visibility.Collapsed, OnRecurrenceVisibilityChanged));



        /// <summary>
        ///  method called when recurrence visibility changed
        /// </summary>
        /// <param name="dpo"></param>
        /// <param name="eargs">An <see
        /// cref="T:System.Windows.DependencyPropertyChangedEventArgs"/> that contains the
        /// event data.</param>
        public static void OnRecurrenceVisibilityChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs eargs)
        {
            var schedulectl = dpo as ScheduleAppointmentEditorControl;
            schedulectl.ShowHideRecurrence();
        }

        private void ShowHideRecurrence()
        {
            /*if (this.RecurrenceVisibility == Visibility.Visible)
            {
                VisualStateManager.GoToState(this, "RecurrenceVisible", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "RecurrenceCollapsed", false);
            }*/
        }

        #endregion

        #region DailyRecurrenceVisibility (DependencyProperty)

        /// <summary>
        /// Gets / Sets the DailyRecurrenceVisibility property.
        /// </summary>
        public Visibility DailyRecurrenceVisibility
        {
            get { return (Visibility)GetValue(DailyRecurrenceVisibilityProperty); }
            set { SetValue(DailyRecurrenceVisibilityProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for DailyRecurrenceVisibility.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DailyRecurrenceVisibilityProperty = DependencyProperty.Register("DailyRecurrenceVisibility",
            typeof(Visibility), typeof(ScheduleAppointmentEditorControl), new PropertyMetadata(Visibility.Visible));

        #endregion

        #region MonlyRecurrenceVisibility (DependencyProperty)

        /// <summary>
        /// Gets / Sets the MonlyRecurrenceVisibility property.
        /// </summary>
        public Visibility MonlyRecurrenceVisibility
        {
            get { return (Visibility)GetValue(MonlyRecurrenceVisibilityProperty); }
            set { SetValue(MonlyRecurrenceVisibilityProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for MonlyRecurrenceVisibility.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MonlyRecurrenceVisibilityProperty = DependencyProperty.Register("MonlyRecurrenceVisibility",
            typeof(Visibility), typeof(ScheduleAppointmentEditorControl), new PropertyMetadata(Visibility.Collapsed));

        #endregion

        #region WeeklyRecurrenceVisibility (DependencyProperty)

        /// <summary>
        /// Gets / Sets the WeelyRecurrenceVisibility property.
        /// </summary>
        public Visibility WeeklyRecurrenceVisibility
        {
            get { return (Visibility)GetValue(WeeklyRecurrenceVisibilityProperty); }
            set { SetValue(WeeklyRecurrenceVisibilityProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for WeeklyRecurrenceVisibility.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty WeeklyRecurrenceVisibilityProperty = DependencyProperty.Register("WeeklyRecurrenceVisibility",
            typeof(Visibility), typeof(ScheduleAppointmentEditorControl), new PropertyMetadata(Visibility.Collapsed));

        #endregion

        #region YearlyRecurrenceVisibility (DependencyProperty)

        /// <summary>
        /// Gets / Sets the YearlyRecurrenceVisibility property.
        /// </summary>
        public Visibility YearlyRecurrenceVisibility
        {
            get { return (Visibility)GetValue(YearlyRecurrenceVisibilityProperty); }
            set { SetValue(YearlyRecurrenceVisibilityProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for YearlyRecurrenceVisibility.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty YearlyRecurrenceVisibilityProperty = DependencyProperty.Register("YearlyRecurrenceVisibility",
            typeof(Visibility), typeof(ScheduleAppointmentEditorControl), new PropertyMetadata(Visibility.Collapsed));

        #endregion

        #endregion

        /// <summary>
        /// reference for recurrence pattern
        /// </summary>
        public RecurrencePatternMode RecurrencePattern;
        private RadioButton DailyRadioButton;
        private RadioButton WeeklyRadioButton;
        private RadioButton MonthlyRadioButton;
        private RadioButton YearlyRadioButton;
        private RadioButton EndAfterRadioButton;
        private RadioButton EndByDateRadioButton;
        private Button saveButton;
        private Button deleteButton;
        private ToggleButton recurrenceButton;
        private ToggleButton highImportance;
        private ToggleButton lowImportance;
        private ToggleButton isPrivate;
        private ComboBox starttime;
        private ComboBox endtime;
       
        //private ToggleButton timeZone;
#if SILVERLIGHT       
        private DatePicker endDatePicker;
        private DatePicker startDatePicker;
        private DatePicker EndRecurrenceDatePicker;
        //private ComboBox starttime;
        //private ComboBox endtime;
#else
        private Syncfusion.Windows.Controls.DatePicker endDatePicker;
        private Syncfusion.Windows.Controls.DatePicker startDatePicker;
        private Syncfusion.Windows.Controls.DatePicker EndRecurrenceDatePicker; 
#endif
        private TextBox SubjectTextBox;
        private ComboBox PriorityComboBox;
        //private ComboBox StartTimeZoneComboBox;
        //private ComboBox EndTimeZoneComboBox;
        private TextBlock recurrenceTextBlock;
        private TextBox recurrenceCountTextbox;       

        string recurrenceText
        {
            get { return recurrenceTextBlock.Text; }
            set { recurrenceTextBlock.Text = value; }
        }

        private bool allowRecurrence = true;
        internal bool AllowRecurrence
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
                    EnableDisableRecurrenceButton();
                }
            }
        }

        //private ComboBox startTimeCombo;
        //private ComboBox endTimeCombo;

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.saveButton = this.GetTemplateChild("PART_SaveButton") as Button;
            this.deleteButton = this.GetTemplateChild("PART_DeleteButton") as Button;
            this.recurrenceButton = this.GetTemplateChild("PART_RecurrenceButton") as ToggleButton;
            this.DailyRadioButton = this.GetTemplateChild("PART_DailyRdo") as RadioButton;
            this.WeeklyRadioButton = this.GetTemplateChild("PART_WeeklyRdo") as RadioButton;
            this.MonthlyRadioButton = this.GetTemplateChild("PART_MonthlyRdo") as RadioButton;
            this.YearlyRadioButton = this.GetTemplateChild("PART_YearlyRdo") as RadioButton;
            this.EndAfterRadioButton = this.GetTemplateChild("PART_EndAfterRdo") as RadioButton;
            this.EndByDateRadioButton = this.GetTemplateChild("PART_EndByDateRdo") as RadioButton;
            this.recurrenceTextBlock = this.GetTemplateChild("PART_RecurrenceText") as TextBlock;
            this.recurrenceCountTextbox = this.GetTemplateChild("PART_RecurrenceCountText") as TextBox;
            this.SubjectTextBox = this.GetTemplateChild("PART_SubjectText") as TextBox;
            this.PriorityComboBox = this.GetTemplateChild("PriorityComboBox") as ComboBox;
            this.highImportance = this.GetTemplateChild("PART_HighImportance") as ToggleButton;
            this.lowImportance = this.GetTemplateChild("PART_LowImportance") as ToggleButton;
            this.isPrivate = this.GetTemplateChild("PART_PrivateButton") as ToggleButton;
            this.starttime = this.GetTemplateChild("Start_Time") as ComboBox;
            this.endtime = this.GetTemplateChild("End_Time") as ComboBox;
            //this.timeZone = this.GetTemplateChild("PART_TimeZonesButton") as ToggleButton;
            //this.StartTimeZoneComboBox = this.GetTemplateChild("StartTimeZoneComboBox") as ComboBox;
            //this.EndTimeZoneComboBox = this.GetTemplateChild("EndTimeZoneComboBox") as ComboBox;
            //StartTimeZoneComboBox.DataContext = this;
            //EndTimeZoneComboBox.DataContext = this;
            this.EnableDisableRecurrenceButton();
#if !SILVERLIGHT
            this.EndRecurrenceDatePicker = this.GetTemplateChild("EndRecurrenceDatePicker") as Syncfusion.Windows.Controls.DatePicker;
            this.endDatePicker = this.GetTemplateChild("EndDatePicker") as Syncfusion.Windows.Controls.DatePicker;
            this.startDatePicker = this.GetTemplateChild("StartDatePicker") as Syncfusion.Windows.Controls.DatePicker;
           
            
#else
            this.EndRecurrenceDatePicker = this.GetTemplateChild("EndRecurrenceDatePicker") as DatePicker;
            this.endDatePicker = this.GetTemplateChild("EndDatePicker") as DatePicker;
            this.startDatePicker = this.GetTemplateChild("StartDatePicker") as DatePicker;            
#endif
            this.SetupRadioButtonEvents();
            this.SetUpEventsToFocusCursor();
            this.saveButton.Click += new RoutedEventHandler(saveButton_Click);
            this.deleteButton.Click += new RoutedEventHandler(deleteButton_Click);
            this.recurrenceButton.Click += new RoutedEventHandler(recurrenceButton_Click);
            this.highImportance.Click += new RoutedEventHandler(highImportance_Click);
            this.lowImportance.Click += new RoutedEventHandler(lowImportance_Click);
            this.isPrivate.Click += new RoutedEventHandler(isPrivate_Click);
            this.startDatePicker.SelectedDateChanged += new EventHandler<SelectionChangedEventArgs>(startDatePicker_SelectedDateChanged);
            this.endDatePicker.SelectedDateChanged += new EventHandler<SelectionChangedEventArgs>(endDatePicker_SelectedDateChanged);
            this.SubjectTextBox.TextChanged += new TextChangedEventHandler(SubjectTextBox_TextChanged);
            this.starttime.SelectionChanged +=new SelectionChangedEventHandler(starttime_SelectionChanged);
            this.endtime.SelectionChanged += new SelectionChangedEventHandler(endtime_SelectionChanged);
            //this.timeZone.Click += new RoutedEventHandler(timeZone_Click);
            //this.PriorityComboBox.SelectionChanged += new SelectionChangedEventHandler(PriorityComboBox_SelectionChanged);            
            this.ShowHideRecurrenceAll();
           
           
        }

       

        void isPrivate_Click(object sender, RoutedEventArgs e)
        {
            if (this.isPrivate.IsChecked == false)
            {
                this.isPrivate.IsChecked = false;
            }
            else
            {
                this.isPrivate.IsChecked = true;
            }
            if(!first)
                this.IsStateChanged = true;
        }
    

        void lowImportance_Click(object sender, RoutedEventArgs e)
        {
            this.highImportance.IsChecked = false;
            if (!first)
                this.IsStateChanged = true;
        }

        void highImportance_Click(object sender, RoutedEventArgs e)
        {
            this.lowImportance.IsChecked = false;
            if (!first)
                this.IsStateChanged = true;
        }

        void SubjectTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!first)
                IsStateChanged = true;
        }

        // void PriorityComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
            //ComboBox element = sender as ComboBox;
            //AppointmentPriority priority = AppointmentPriority.Free;
            //if (element.SelectedItem.ToString() == "Free")
            //{
            //   priority = AppointmentPriority.Free;
            //}
            //else if (element.SelectedItem.ToString() == "Busy")
            //{
            //    priority = AppointmentPriority.Busy;
            //}
            //else if (element.SelectedItem.ToString() == "Tentative")
            //{
            //    priority = AppointmentPriority.Tentative;
            //}
            //else if (element.SelectedItem.ToString() == "Out-Of-Office")
            //{
            //    priority = AppointmentPriority.OutOfOffice;
            //}
            // ScheduleAppointmentWrapper val = DataContext as ScheduleAppointmentWrapper;
            // val.Appointment.Priority = priority;

            // if (!first)
            //     IsStateChanged = true;
        //}

        private void EnableDisableRecurrenceButton()
        {
            if (this.recurrenceButton == null) return;

            if (AllowRecurrence == false)
                this.recurrenceButton.IsEnabled = false;
            else
                this.recurrenceButton.IsEnabled = true;
        }

        private void SetUpEventsToFocusCursor()
        {
            this.recurrenceCountTextbox.GotFocus += new RoutedEventHandler(recurrenceCountTextbox_GotFocus);
            this.EndRecurrenceDatePicker.GotFocus += new RoutedEventHandler(EndRecurrenceDatePicker_GotFocus);            
        }
       
        private void EndRecurrenceDatePicker_GotFocus(object sender, RoutedEventArgs e)
        {
            this.EndRecurrenceDatePicker.GotFocus -= new RoutedEventHandler(EndRecurrenceDatePicker_GotFocus);
            this.EndByDateRadioButton.IsChecked = true;
            this.EndRecurrenceDatePicker.GotFocus += new RoutedEventHandler(EndRecurrenceDatePicker_GotFocus);
        }

        private void recurrenceCountTextbox_GotFocus(object sender, RoutedEventArgs e)
        {
            this.recurrenceCountTextbox.GotFocus -= new RoutedEventHandler(recurrenceCountTextbox_GotFocus);
            this.EndAfterRadioButton.IsChecked = true;
            this.recurrenceCountTextbox.GotFocus += new RoutedEventHandler(recurrenceCountTextbox_GotFocus);
        }

        #region Show/HideRecurrence

        private void SetupRadioButtonEvents()
        {
            this.DailyRadioButton.Checked += new RoutedEventHandler(DailyRadioButton_Checked);
            this.WeeklyRadioButton.Checked += new RoutedEventHandler(WeeklyRadioButton_Checked);
            this.MonthlyRadioButton.Checked += new RoutedEventHandler(MonthlyRadioButton_Checked);
            this.YearlyRadioButton.Checked += new RoutedEventHandler(YearlyRadioButton_Checked);
        }

        private void ShowRecurrencePattern()
        {
            this.DailyRecurrenceVisibility = Visibility.Collapsed;
            this.WeeklyRecurrenceVisibility = Visibility.Collapsed;
            this.MonlyRecurrenceVisibility = Visibility.Collapsed;
            this.YearlyRecurrenceVisibility = Visibility.Collapsed;

            switch (this.RecurrencePattern)
            {
                case RecurrencePatternMode.Daily:
                    this.DailyRecurrenceVisibility = Visibility.Visible;
                    break;
                case RecurrencePatternMode.Weekly:
                    this.WeeklyRecurrenceVisibility = Visibility.Visible;
                    break;
                case RecurrencePatternMode.Monthly:
                    this.MonlyRecurrenceVisibility = Visibility.Visible;
                    break;
                case RecurrencePatternMode.Yearly:
                    this.YearlyRecurrenceVisibility = Visibility.Visible;
                    break;
            }
        }

        private void DailyRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            this.RecurrencePattern = RecurrencePatternMode.Daily;
            this.ShowRecurrencePattern();
        }

        private void WeeklyRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            this.RecurrencePattern = RecurrencePatternMode.Weekly;
            this.ShowRecurrencePattern();
        }

        private void MonthlyRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            this.RecurrencePattern = RecurrencePatternMode.Monthly;
            this.ShowRecurrencePattern();
        }

        private void YearlyRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            this.RecurrencePattern = RecurrencePatternMode.Yearly;
            this.ShowRecurrencePattern();
        }

        /// <summary>
        /// Occurs when recurrence button clicked
        /// </summary>
        public event RoutedEventHandler RecurrenceButtonClick;
        private void recurrenceButton_Click(object sender, RoutedEventArgs e)
        {
            ResourceWrapper rw = new ResourceWrapper();
            if (this.RecurrenceVisibility == Visibility.Visible)
            {
                this.RecurrenceVisibility = Visibility.Collapsed;
                this.recurrenceText = rw.AppWindowEnableRecurrence;
                this.IsRecurrenceOccured = false;
            }
            else
            {
                this.RecurrenceVisibility = Visibility.Visible;
                this.recurrenceText = rw.AppWindowRemoveRecurrence;
                this.IsRecurrenceOccured = true;
            }

            
            
            var handler = this.RecurrenceButtonClick;
            if (handler != null)
            {
                handler(this, e);
            }            
        }

        private void ShowHideRecurrenceAll()
        {
            ResourceWrapper rw = new ResourceWrapper();
            if (this.RecurrenceVisibility == Visibility.Collapsed)
            {
                this.IsRecurrenceOccured = false;
                this.recurrenceText = rw.AppWindowEnableRecurrence;
            }
            else
            {
                this.IsRecurrenceOccured = true;
                this.recurrenceText = rw.AppWindowRemoveRecurrence;
            }
        }

        #endregion

        void startDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.endDatePicker.SelectedDate < this.startDatePicker.SelectedDate)
            {
                this.endDatePicker.SelectedDate = this.startDatePicker.SelectedDate;
            }
            else if (this.endDatePicker.SelectedDate == this.startDatePicker.SelectedDate && this.starttime.SelectedIndex > this.endtime.SelectedIndex)
            {
                this.endtime.SelectedIndex = this.starttime.SelectedIndex;
            }
            if (!first)
                IsStateChanged = true;
        }

        void endDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.endDatePicker.SelectedDate < this.startDatePicker.SelectedDate && this.endDatePicker.SelectedDate.Value.Day !=this.startDatePicker.SelectedDate.Value.Day )
            {
                MessageBox.Show("The end date you entered occurs before the start date.", "Syncfusion Schedule", MessageBoxButton.OK);
                this.endDatePicker.SelectedDate = this.startDatePicker.SelectedDate;
            }
            else if(this.endDatePicker.SelectedDate.Value.Day  ==this.startDatePicker.SelectedDate.Value.Day  && this.starttime.SelectedIndex >this.endtime.SelectedIndex)
            {
                this.endtime.SelectedIndex = this.starttime.SelectedIndex; 
            }
            if (!first)
                IsStateChanged = true;
        }
        void starttime_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.endtime.ItemsSource == null) return;
            if (this.starttime.SelectedIndex  > this.endtime.SelectedIndex )
            {
                this.endtime.SelectedIndex = this.starttime.SelectedIndex;
            }
            if (!first)
                IsStateChanged = true;
        }
        void endtime_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.endDatePicker.SelectedDate.Value.Day == this.startDatePicker.SelectedDate.Value.Day && this.endDatePicker.SelectedDate.Value.Month == this.startDatePicker.SelectedDate.Value.Month && this.endDatePicker.SelectedDate.Value.Year == this.startDatePicker.SelectedDate.Value.Year && this.starttime.SelectedIndex > this.endtime.SelectedIndex)
            {
                MessageBox.Show("The end Time you entered occurs before the start Time.", "Syncfusion Schedule", MessageBoxButton.OK);
                this.endtime.SelectedIndex = this.starttime.SelectedIndex;
            }
            if (!first)
                IsStateChanged = true;
        }
        

        /// <summary>
        /// Occurs when [delete button click].
        /// </summary>
        public event RoutedEventHandler DeleteButtonClick;
        void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            var handler = this.DeleteButtonClick;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when [save button click].
        /// </summary>
        public event RoutedEventHandler SaveButtonClick;
        void saveButton_Click(object sender, RoutedEventArgs e)
        {                                 
            var handler = this.SaveButtonClick;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }



}
