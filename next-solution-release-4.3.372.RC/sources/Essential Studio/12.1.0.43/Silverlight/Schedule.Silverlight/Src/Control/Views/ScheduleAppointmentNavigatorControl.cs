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
using System.Linq;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Syncfusion.Windows.Controls.Schedule
{
    /// <summary>
    /// class that holds appointment navigations
    /// </summary>
    public class ScheduleAppointmentNavigatorControl : Control, IScheduleCalendarViewModelHost
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Controls.Schedule.ScheduleAppointmentNavigatorControl"/>
        /// class.
        /// </summary>
        public ScheduleAppointmentNavigatorControl()
        {
            this.DefaultStyleKey = typeof(ScheduleAppointmentNavigatorControl);

        }

        private Button PrevButton;
        private Button NextButton;

        #region Override
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or
        /// internal processes call <see
        /// cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.PrevButton = this.GetTemplateChild("PrevAppointmentButton") as Button;
            this.NextButton = this.GetTemplateChild("NextAppointmentButton") as Button;
            this.PrevButton.Click += new RoutedEventHandler(PrevButton_Click);
            this.NextButton.Click += new RoutedEventHandler(NextButton_Click);
            this.PrevButton.Background = this.model.AppointmentBackground;
            this.NextButton.Background = this.model.AppointmentBackground;
            this.PrevButton.BorderBrush = this.model.StrokeLine;
            this.NextButton.BorderBrush = this.model.StrokeLine;
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            int selectedDatesCount = this.Model.SelectedDates.Count;
            if (selectedDatesCount <= 0) return;
            if (this.model.CurrentScheduleType != ScheduleType.Day || this.model.CurrentScheduleType != ScheduleType.WorkWeek || this.model.CurrentScheduleType != ScheduleType.Week)
            {
                var appsel = from app in this.Model.Appointments
                             where app.StartTime.Date > this.Model.SelectedDates.LastOrDefault()
                             orderby app.StartTime
                             select app;
                if (appsel == null || appsel.Count() <= 0) return;
                var selectedapp = appsel.FirstOrDefault() as ScheduleAppointment;
                ObservableCollection<DateTime> newDates = new ObservableCollection<DateTime>();
                DateTime date = selectedapp.StartTime.Date;
                if (this.Model.CurrentScheduleType == ScheduleType.WorkWeek)
                    date = date.StartOfWeek(this.Model.WorkingDays[0]);
                else if (this.Model.CurrentScheduleType == ScheduleType.Week || this.Model.CurrentScheduleType == ScheduleType.Month)
                    date = date.StartOfWeek(DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek);                
                //else
                //    date.SubractDays(selectedDatesCount);
                for (int i = 0; i < selectedDatesCount; i++)
                {
                    newDates.Add(date);
                    date = date.AddDays(1);
                }
                this.Model.SelectedDates = newDates;
            }
            else
            {
                var appsel = from app in this.Model.Appointments
                             where app.StartTime.Date > this.Model.SelectedDates.LastOrDefault()
                             orderby app.StartTime
                             select app;
                if (appsel == null || appsel.Count() <= 0) return;
                var selectedapp = appsel.FirstOrDefault() as ScheduleAppointment;
                ObservableCollection<DateTime> newDates = new ObservableCollection<DateTime>();
                DateTime date = selectedapp.StartTime.Date.StartOfWeek(DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek);
                date.SubractDays(selectedDatesCount);
                for (int i = 0; i < selectedDatesCount; i++)
                {
                    newDates.Add(date);
                    date = date.AddDays(1);
                }
                this.Model.SelectedDates = newDates;
            }
        }

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            int selectedDatesCount = this.Model.SelectedDates.Count;
            if (selectedDatesCount <= 0) return;
            if (this.model.CurrentScheduleType != ScheduleType.Day || this.model.CurrentScheduleType != ScheduleType.WorkWeek || this.model.CurrentScheduleType != ScheduleType.Week)
            {
                var appsel = from app in this.Model.Appointments
                             where app.StartTime.Date < this.Model.SelectedDates.FirstOrDefault()
                             orderby app.StartTime
                             select app;
                if (appsel == null || appsel.Count() <= 0) return;
                var selectedapp = appsel.LastOrDefault() as ScheduleAppointment;
                ObservableCollection<DateTime> newDates = new ObservableCollection<DateTime>();
                DateTime date = selectedapp.StartTime.Date;
                if (this.Model.CurrentScheduleType == ScheduleType.WorkWeek)
                {
                    date = date.StartOfWeek(this.Model.WorkingDays[0]);
                }
                else if (this.Model.CurrentScheduleType == ScheduleType.Week)
                {
                    date = date.StartOfWeek(DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek);
                }
                else if (this.Model.CurrentScheduleType == ScheduleType.Month)
                {
                    date = date.StartOfWeek(DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek); 
                }
                //else if (this.Model.CurrentScheduleType == ScheduleType.Day)
                //{
                //    date = date.StartOfWeek(DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek);
                //}              
                for (int i = 0; i < selectedDatesCount; i++)
                {
                    newDates.Add(date);
                    date = date.AddDays(1);
                }
                this.Model.SelectedDates = newDates;
            }
            else
            {
                var appsel = from app in this.Model.Appointments
                             where app.StartTime.Date < this.Model.SelectedDates.FirstOrDefault()
                             orderby app.StartTime
                             select app;
                if (appsel == null || appsel.Count() <= 0) return;
                var selectedapp = appsel.FirstOrDefault() as ScheduleAppointment;
                ObservableCollection<DateTime> newDates = new ObservableCollection<DateTime>();
                DateTime date = selectedapp.StartTime.Date.StartOfWeek(DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek);
                for (int i = 0; i < selectedDatesCount; i++)
                {
                    newDates.Add(date);
                    date = date.AddDays(1);
                }
                this.Model.SelectedDates = newDates;
            }
        }

        #endregion

        #region IScheduleCalendarViewModelHost Members

        private ScheduleCalendarViewModel model;
        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>The model.</value>
        public ScheduleCalendarViewModel Model
        {
            get { return this.model; }
        }

        /// <summary>
        /// Sets the calendar view model.
        /// </summary>
        /// <param name="model">The model.</param>
        public void SetCalendarViewModel(ScheduleCalendarViewModel model)
        {
            if (this.model != null)
            {
                this.model.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(Model_PropertyChanged);
            }
            this.model = model;
            this.model.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Model_PropertyChanged);
            this.model.Appointments.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Appointments_CollectionChanged);
        }

        void Appointments_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.CheckForAppointmentsInSelectedDates();
        }

        /// <summary>
        /// Handles the PropertyChanged event of the Model control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedDates" || e.PropertyName == "Appointments")
            {
                this.CheckForAppointmentsInSelectedDates();
            }
            else if (e.PropertyName == "AppointmentBackground")
            {
                this.PrevButton.Background = this.model.AppointmentBackground;
                this.NextButton.Background = this.model.AppointmentBackground;
                this.PrevButton.BorderBrush = this.model.StrokeLine;
                this.NextButton.BorderBrush = this.model.StrokeLine;
            }
        }

        private void CheckForAppointmentsInSelectedDates()
        {
            bool count = false;
            if (this.model.Appointments.Count <= 0)
            {
                this.AppointmentNavigatorButtonVisible = Visibility.Collapsed;
                return;
            }
            foreach (var appointment in this.Model.Appointments)
            {
                if (!appointment.IsRecurrenceAppointment)
                {
                    if (appointment.StartTime >= this.Model.SelectedDates[0].Date && appointment.EndTime < this.Model.SelectedDates.LastOrDefault().AddDays(1).Date)
                    {
                        count = true;
                        break;
                    }
                }
                else
                {                   
                    for(DateTime date = this.Model.SelectedDates[0].Date ;date < this.Model.SelectedDates.LastOrDefault().AddDays(1).Date; date= date.AddDays(1))
                    {
                        ScheduleAppointment app = appointment.CheckForAppointmentAvailablity(date);
                        if (app != null)
                        {
                            count = true;
                            break;
                        }
                    }
                }
            }
            if (count)
            {
                this.AppointmentNavigatorButtonVisible = Visibility.Collapsed;
                return ;
            }
            else
            {
                this.AppointmentNavigatorButtonVisible = Visibility.Visible;
            }
            //foreach (var date in this.Model.SelectedDates)
            //{
            //    var app = this.Model.GetCurrentAppointmentsByDate(date).OfType<ScheduleAppointmentInfo>();
            //    if (app != null)
            //    {
                   
            //        return;
            //    }
            //}
         
        }


        #region AppointmentNavigatorButtonVisible (DependencyProperty)

        /// <summary>
        /// Gets / Sets the AppointmentEditorStyle.
        /// </summary>
        public Visibility AppointmentNavigatorButtonVisible
        {
            get { return (Visibility)GetValue(AppointmentNavigatorButtonVisibleProperty); }
            set { SetValue(AppointmentNavigatorButtonVisibleProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for AppointmentNavigatorButtonVisible.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AppointmentNavigatorButtonVisibleProperty =
            DependencyProperty.Register("AppointmentNavigatorButtonVisible", typeof(Visibility), typeof(ScheduleAppointmentNavigatorControl),
              new PropertyMetadata(Visibility.Collapsed, OnAppointmentNavigatorButtonVisibleChanged));

        private static void OnAppointmentNavigatorButtonVisibleChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var scheduleAppNavigatorcontrol = dpo as ScheduleAppointmentNavigatorControl;
            if ((Visibility)args.NewValue == Visibility.Visible)
                VisualStateManager.GoToState(scheduleAppNavigatorcontrol, "ButtonVisible", false);
            else
                VisualStateManager.GoToState(scheduleAppNavigatorcontrol, "ButtonCollapsed", false);
        }

        #endregion


        #endregion

    }
}
