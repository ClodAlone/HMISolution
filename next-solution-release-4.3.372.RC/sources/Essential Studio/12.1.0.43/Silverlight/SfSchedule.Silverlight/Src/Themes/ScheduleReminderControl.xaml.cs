#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Syncfusion.UI.Xaml.Schedule
{
    /// <summary>
    /// Represents a reminder window.
    /// </summary>
    public partial class ScheduleReminderControl
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.UI.Xaml.Schedule.ScheduleReminderControl">ScheduleReminderControl</see> class. 
        /// </summary>
        public ScheduleReminderControl()
        {
            SnoozeCollection = new ObservableCollection<Snooze>
            {
                new Snooze {SnoozeString = "5 Minutes", SnoozeTime = new TimeSpan(0, 5, 0)},
                new Snooze {SnoozeString = "10 Minutes", SnoozeTime = new TimeSpan(0, 10, 0)},
                new Snooze {SnoozeString = "15 Minutes", SnoozeTime = new TimeSpan(0, 15, 0)},
                new Snooze {SnoozeString = "30 Minutes", SnoozeTime = new TimeSpan(0, 30, 0)},
                new Snooze {SnoozeString = "1 hour", SnoozeTime = new TimeSpan(1, 0, 0)},
                new Snooze {SnoozeString = "2 hours", SnoozeTime = new TimeSpan(2, 0, 0)},
                new Snooze {SnoozeString = "4 hours", SnoozeTime = new TimeSpan(4, 0, 0)},
                new Snooze {SnoozeString = "8 hours", SnoozeTime = new TimeSpan(8, 0, 0)},
                new Snooze {SnoozeString = "0.5 days", SnoozeTime = new TimeSpan(12, 0, 0)},
                new Snooze {SnoozeString = "1 day", SnoozeTime = new TimeSpan(1, 0, 0, 0)},
                new Snooze {SnoozeString = "2 days", SnoozeTime = new TimeSpan(2, 0, 0, 0)},
                new Snooze {SnoozeString = "3 days", SnoozeTime = new TimeSpan(3, 0, 0, 0)},
                new Snooze {SnoozeString = "4 days", SnoozeTime = new TimeSpan(4, 0, 0, 0)},
                new Snooze {SnoozeString = "1 week", SnoozeTime = new TimeSpan(7, 0, 0, 0)},
                new Snooze {SnoozeString = "2 weeks", SnoozeTime = new TimeSpan(14, 0, 0, 0)}
            };
            Resources.Add("dueconverter", new DueConverter());
            InitializeComponent();
            Loaded += ScheduleReminderControl_Loaded;
            DataContext = this;


            snooze.ItemsSource = SnoozeCollection;
            snooze.DisplayMemberPath = "SnoozeString";
            snooze.SelectedIndex = 0;
            var listbxbinding = new Binding {Source = this, Path = new PropertyPath("ReminderAppointmentCollection")};
            BindingOperations.SetBinding(listbox, ItemsControl.ItemsSourceProperty, listbxbinding);

        }

        #endregion

        #region Public Fields
        
        /// <summary>
        /// Gets or sets a collection of snooze values.
        /// </summary>
        public ObservableCollection<Snooze> SnoozeCollection; 

        #endregion

        #region Internal Fields

        internal SfSchedule schedule;

        #endregion

        #region Dependency Properties

        #region ReminderAppointmentCollection
        /// <summary>
        /// Gets or sets a collection of appointments with reminder.
        /// </summary>
        public ScheduleAppointmentCollection ReminderAppointmentCollection
        {
            get { return (ScheduleAppointmentCollection)GetValue(ReminderAppointmentCollectionProperty); }
            set { SetValue(ReminderAppointmentCollectionProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ReminderAppointmentCollection.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ReminderAppointmentCollectionProperty =
            DependencyProperty.Register("ReminderAppointmentCollection", typeof(ScheduleAppointmentCollection),
                typeof(ScheduleReminderControl), new PropertyMetadata(null)); 
        #endregion

        #endregion

        #region Events

        private void ScheduleReminderControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (listbox.Items.Count > 0)
            {
                listbox.SelectedIndex = 0;
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox != null)
            {
                var app = listBox.SelectedItem as ScheduleAppointment;
                if (app != null)
                {
                    time.Text = app.StartTime.ToString("hh:mm tt dddd,MMM dd, yyyy");
                    subject.Text = app.Subject;
                    location.Text = app.Location;
                    snooze.IsEnabled = true;
                    Dismiss.IsEnabled = true;
                    DismissAll.IsEnabled = true;
                    setsnooze.IsEnabled = true;
                }
                else
                {
                    subject.Text = string.Empty;
                    time.Text = "0 Reminders are Selected";
                    snooze.IsEnabled = false;
                    Dismiss.IsEnabled = false;
                    DismissAll.IsEnabled = false;
                    setsnooze.IsEnabled = false;
                }
            }
        }

        private void Dismiss_Click(object sender, RoutedEventArgs e)
        {
            if (listbox.SelectedItem != null)
            {
                var app = listbox.SelectedItem as ScheduleAppointment;
                if (app != null)
                {
                    app.ReminderTime = ReminderTimeType.None;
                    ReminderAppointmentCollection.Remove(app);
                }
            }
            if (ReminderAppointmentCollection.Count == 0)
            {
                Close();
                if (schedule != null)
                    schedule.GetReminderControlClosedEvents(new ReminderControlClosedEventArgs());
            }
        }

        private void DismissAll_Click(object sender, RoutedEventArgs e)
        {
            if (ReminderAppointmentCollection.Count > 0)
            {
                foreach (ScheduleAppointment app in ReminderAppointmentCollection)
                {
                    app.ReminderTime = ReminderTimeType.None;

                }
                ReminderAppointmentCollection.Clear();
                Close();
                if (schedule != null)
                    schedule.GetReminderControlClosedEvents(new ReminderControlClosedEventArgs());
            }
        }

        private void setsnooze_Click(object sender, RoutedEventArgs e)
        {
            if (snooze.SelectedItem != null)
            {
                var type = (Snooze)snooze.SelectedItem;
                if (listbox.SelectedItem != null)
                {
                    var app = listbox.SelectedItem as ScheduleAppointment;
                    if (app != null)
                    {
                        app.ReminderDeliveryTime = DateTime.Now.Add(type.SnoozeTime);
                        ReminderAppointmentCollection.Remove(app);
                    }
                }
            }
            if (ReminderAppointmentCollection.Count == 0)
            {
                Close();
                if (schedule != null)
                    schedule.GetReminderControlClosedEvents(new ReminderControlClosedEventArgs());
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Close();
            if (schedule != null)
                schedule.GetReminderControlClosedEvents(new ReminderControlClosedEventArgs());
        }

        #endregion

        #region DueConverter

        /// <summary>
        /// Represents a converter that converts date time value to due time.
        /// </summary>
        public class DueConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                var date = value as DateTime?;
                string str = null;
                if (date != null)
                {
                    TimeSpan? diff = DateTime.Now - date;

                    if (diff.Value.Days != 0)
                    {
                        str = diff.Value.Days + " " + "Days" + " ";
                    }
                    if (diff.Value.Hours != 0)
                    {
                        str = str + diff.Value.Hours + " " + "Hours" + " ";

                    }
                    if (diff.Value.Minutes != 0)
                    {
                        str = str + diff.Value.Minutes + " " + "Minutes " + " ";
                    }
                    if (str != null)
                    {
                        str = str + "Over Due";
                    }
                }
                return str;
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        } 

        #endregion

        #region Snooze

        /// <summary>
        /// Represents a class to denote snooze objects.
        /// </summary>
        public class Snooze
        {
            public string SnoozeString { get; set; }
            public TimeSpan SnoozeTime { get; set; }
        } 

        #endregion
    }
}
