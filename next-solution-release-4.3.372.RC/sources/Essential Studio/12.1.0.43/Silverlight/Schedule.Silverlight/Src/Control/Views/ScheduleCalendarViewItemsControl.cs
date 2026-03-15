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
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Windows.Controls.Primitives;
    using System.ComponentModel;
    using System.Windows.Data;
    using System.Collections.ObjectModel;
    using System.Collections.Generic;
    using System.Globalization;
#if !SILVERLIGHT
    using Syncfusion.Windows.Controls.Primitives;
    /// <summary>
    ///  class that holds calendar view
    /// </summary>
    [StyleTypedProperty(Property = "CalendarStyle", StyleTargetType = typeof(Syncfusion.Windows.Controls.Calendar))]
    [StyleTypedProperty(Property = "CalendarDayButtonStyle", StyleTargetType = typeof(Syncfusion.Windows.Controls.Primitives.CalendarDayButton))]
#else
    /// <summary>
    /// Represents Schedule's Calender view.
    /// </summary>
    [StyleTypedProperty(Property = "CalendarStyle", StyleTargetType = typeof(System.Windows.Controls.Calendar))]
    [StyleTypedProperty(Property = "CalendarDayButtonStyle", StyleTargetType = typeof(CalendarDayButton))]
#endif
#if SyncfusionFramework4_0 && !SILVERLIGHT
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ScheduleCalendarViewItemsControl : ItemsControl, IScheduleCalendarViewModelHost
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleCalendarViewItemsControl"/> class.
        /// </summary>
        public ScheduleCalendarViewItemsControl()
        {
            this.DefaultStyleKey = typeof(ScheduleCalendarViewItemsControl);
            Loaded += new RoutedEventHandler(ScheduleCalendarViewItemsControl_Loaded);
this.MouseRightButtonUp += new MouseButtonEventHandler(ScheduleCalendarViewItemsControl_MouseRightButtonUp);
        }

        void ScheduleCalendarViewItemsControl_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void ScheduleCalendarViewItemsControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetAppointmentDateBold();
        }

        /// <summary>
        /// Sets the appointment date bold.
        /// </summary>
        public void SetAppointmentDateBold()
        {
            if (this.Items.Count <= 0) return;
            foreach (var item in this.Items)
            {
#if !SILVERLIGHT
                var calend = item as Syncfusion.Windows.Controls.Calendar;
#else
                var calend = item as System.Windows.Controls.Calendar;
#endif
                if (calend != null) this.SetAppointmentDateStyle(calend);
            }
        }

        private ScheduleCalendarViewModel model;
        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>The model.</value>
        public ScheduleCalendarViewModel Model
        {
            get
            {
                return this.model;
            }
        }

        /// <summary>
        /// Sets the calendar view model.
        /// </summary>
        /// <param name="model">The model.</param>
        public void SetCalendarViewModel(ScheduleCalendarViewModel model)
        {
            if (this.model != null)
            {
                this.model.SelectedDates.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(SelectedDates_CollectionChanged);
                this.model.PropertyChanged -= new PropertyChangedEventHandler(model_PropertyChanged);
            }

            this.model = model;
            this.model.PropertyChanged += new PropertyChangedEventHandler(model_PropertyChanged);
            this.model.SelectedDates.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(SelectedDates_CollectionChanged);
        }

        /// <summary>
        /// Handles the PropertyChanged event of the model control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        void model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!this.isTemplateApplied || this.isInSuspend)
            {
                return;
            }

            if (e.PropertyName == "SelectedDates")
            {
                this.isInSuspend = true;
                this.currentCalendar.SelectedDates.Clear();
                //foreach (var selectedDate in this.model.SelectedDates)
                //{
                //    this.currentCalendar.SelectedDates.Add(selectedDate);
                //}
                PopulateView();

                this.PrepareSelectedDatesToCalendar();
                this.isInSuspend = false;
            }
        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>
        /// The element that is used to display the given item.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
#if !SILVERLIGHT
            var calendar = new Syncfusion.Windows.Controls.Calendar();
#else
            var calendar = new System.Windows.Controls.Calendar();
#endif
            if (this.CalendarStyle != null)
            {
                calendar.Style = this.CalendarStyle;
            }
            if (this.CalendarDayButtonStyle != null)
            {
                calendar.CalendarDayButtonStyle = this.CalendarDayButtonStyle;
            }
            return calendar;
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>
        /// true if the item is (or is eligible to be) its own container; otherwise, false.
        /// </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
#if !SILVERLIGHT
            return (item is Syncfusion.Windows.Controls.Calendar);
#else
            return (item is System.Windows.Controls.Calendar);
#endif

        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for CalendarStyle.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CalendarStyleProperty = DependencyProperty.Register("CalendarStyle", typeof(Style), typeof(ScheduleCalendarViewItemsControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the calendar style.
        /// </summary>
        /// <value>The calendar style.</value>
        public Style CalendarStyle
        {
            get
            {
                return (Style)this.GetValue(ScheduleCalendarViewItemsControl.CalendarStyleProperty);
            }

            set
            {
                this.SetValue(ScheduleCalendarViewItemsControl.CalendarStyleProperty, value);
            }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for CalendarDayButtonStyle.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CalendarDayButtonStyleProperty = DependencyProperty.Register("CalendarDayButtonStyle", typeof(Style), typeof(ScheduleCalendarViewItemsControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the calendar day button style.
        /// </summary>
        /// <value>The calendar day button style.</value>
        public Style CalendarDayButtonStyle
        {
            get
            {
                return (Style)this.GetValue(ScheduleCalendarViewItemsControl.CalendarDayButtonStyleProperty);
            }

            set
            {
                this.SetValue(ScheduleCalendarViewItemsControl.CalendarDayButtonStyleProperty, value);
            }
        }

        #region CalendarItemsCount

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CalendarItemsCountProperty = DependencyProperty.Register("CalendarItemsCount", typeof(int), typeof(ScheduleCalendarViewItemsControl), new PropertyMetadata(1, OnCalendarItemsCountChanged));

        /// <summary>
        /// Called when [calendar items count changed].
        /// </summary>
        /// <param name="dpo">The dpo.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCalendarItemsCountChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var calendarView = dpo as ScheduleCalendarViewItemsControl;
            calendarView.PopulateView();
        }

        /// <summary>
        /// Gets or sets the calendar items count.
        /// </summary>
        /// <value>The calendar items count.</value>
        public int CalendarItemsCount
        {
            get
            {
                return (int)this.GetValue(ScheduleCalendarViewItemsControl.CalendarItemsCountProperty);
            }

            set
            {
                this.SetValue(ScheduleCalendarViewItemsControl.CalendarItemsCountProperty, value);
            }
        }

        /// <summary>
        /// Populates the view.
        /// </summary>
        private void PopulateView()
        {
            if (!this.isTemplateApplied)
            {
                return;
            }
#if !SILVERLIGHT
            foreach (Syncfusion.Windows.Controls.Calendar cal in this.Items)
            {
                cal.SelectedDatesChanged -= new EventHandler<SelectionChangedEventArgs>(calendar_SelectedDatesChanged);
            }
#else
            foreach (System.Windows.Controls.Calendar cal in this.Items)
            {
                cal.SelectedDatesChanged -= new EventHandler<SelectionChangedEventArgs>(calendar_SelectedDatesChanged);
            }
#endif
            DateTime todayDate;
            this.Items.Clear();
            if(model.CurrentScheduleType != ScheduleType.Month)
              todayDate = model.SelectedDates[0];
            else
             todayDate = model.SelectedDates[7];


            var cultureCalendar = System.Globalization.CultureInfo.CurrentCulture.Calendar;

            for (int i = 0; i < this.CalendarItemsCount; i++)
            {
#if !SILVERLIGHT
                var calendar = (Syncfusion.Windows.Controls.Calendar)this.GetContainerForItemOverride();
#else
                var calendar = (System.Windows.Controls.Calendar)this.GetContainerForItemOverride();
#endif
                var monthStart = new DateTime(todayDate.Year, todayDate.Month, 1);
                var monthEnd = new DateTime(todayDate.Year, todayDate.Month, cultureCalendar.GetDaysInMonth(todayDate.Year, todayDate.Month));
                calendar.DisplayDate = new DateTime(monthStart.Year, monthStart.Month, monthStart.Day);               
#if !SILVERLIGHT
                calendar.SelectionMode = Syncfusion.Windows.Controls.CalendarSelectionMode.MultipleRange;
#else
                calendar.SelectionMode = CalendarSelectionMode.MultipleRange;
#endif
                if (todayDate.Month == monthStart.Month)
                {
                    calendar.SelectedDates.Add(todayDate);
                }
                this.Items.Add(calendar);
                todayDate = monthEnd.Date.AddDays(1);
                //SetAppointmentDateStyle(calendar);
                SetAppointmentDateBold();
                this.Model.RaiseCalendarAddedEvents(calendar);
                calendar.SelectedDatesChanged += new EventHandler<SelectionChangedEventArgs>(calendar_SelectedDatesChanged);
            }

            if (this.Items.Count > 0)
            {
#if !SILVERLIGHT
                this.SetCalendar((Syncfusion.Windows.Controls.Calendar)this.Items[0]);
#else
                this.SetCalendar((System.Windows.Controls.Calendar)this.Items[0]);
#endif
            }
        }
#if !SILVERLIGHT
        private Syncfusion.Windows.Controls.Calendar currentCalendar = null;
        private Syncfusion.Windows.Controls.Calendar CalendarToMakeBold = null;
#else
        private System.Windows.Controls.Calendar currentCalendar = null;
        private System.Windows.Controls.Calendar CalendarToMakeBold = null;
#endif

        /// <summary>
        /// Sets the appointment date style.
        /// </summary>
        /// <param name="calObj">The cal obj.</param>
        public void SetAppointmentDateStyle(UIElement calObj)
        {           
            
#if !SILVERLIGHT
            if (calObj as Syncfusion.Windows.Controls.Calendar != null)
                CalendarToMakeBold = calObj as Syncfusion.Windows.Controls.Calendar;
#else
            if (calObj as System.Windows.Controls.Calendar != null)
                CalendarToMakeBold = calObj as System.Windows.Controls.Calendar;
#endif
            this.UpdateLayout();

            IEnumerable<DateTime> displayDates = this.GetDisplayedDates();
            ScheduleAppointmentCollection sac = this.Model.Appointments;
            DateTime[] _Starttime = new DateTime[sac.Count];
            DateTime[] _Endtime = new DateTime[sac.Count];
            bool[] _Isrecurrence = new bool[sac.Count];
            int k = 0;
            foreach (ScheduleAppointment sp in sac)
            {
                _Starttime[k] = sp.StartTime.Date;
                _Endtime[k] = sp.EndTime.Date;
                _Isrecurrence[k] = sp.IsRecurrenceAppointment;
                k++;
            }
            ObservableCollection<DateTime> boldDates = new ObservableCollection<DateTime>();           
            foreach (DateTime item in displayDates)
            {
                DateTime idt = item.Date;
                int j = 0;
                
                foreach (ScheduleAppointment sp in sac)
                {                    
                    if ((_Starttime[j] <= idt && _Endtime[j] >= idt) || (_Isrecurrence[j] == true && sp.CheckForAppointment(item) != null))
                    {
                        boldDates.Add(item);
                    }
                    j++;
                }
            }           
            int calItemsCnt = VisualTreeHelper.GetChildrenCount(calObj);


            if (calItemsCnt > 0)
            {
                for (int i = 0; i < calItemsCnt; i++)
                {
                    UIElement calItems = (UIElement)VisualTreeHelper.GetChild(calObj, i);
#if!SILVERLIGHT
                    if (calItems.GetType() == typeof( Syncfusion.Windows.Controls.Primitives.CalendarDayButton))
                    {
                        ((Syncfusion.Windows.Controls.Primitives.CalendarDayButton)calItems).FontWeight = FontWeights.Normal;
                        if((calItems as Syncfusion.Windows.Controls.Primitives.CalendarDayButton).FindElementOfType<Button>() != null)
                        {
                        DateTime m = Convert.ToDateTime((calItems as Syncfusion.Windows.Controls.Primitives.CalendarDayButton).FindElementOfType<Button>().DataContext.ToString());
                        if (boldDates.Contains(m))
                            ((Syncfusion.Windows.Controls.Primitives.CalendarDayButton)calItems).FontWeight = FontWeights.Bold;   
                        }
                    } 
#else
                        if (calItems.GetType() == typeof(System.Windows.Controls.Primitives.CalendarDayButton))
                    {
                        ((System.Windows.Controls.Primitives.CalendarDayButton)calItems).FontWeight = FontWeights.Normal;
                        DateTime m = Convert.ToDateTime((calItems as System.Windows.Controls.Primitives.CalendarDayButton).FindElementOfType<Button>().DataContext.ToString());
                        if (boldDates.Contains(m))
                            ((System.Windows.Controls.Primitives.CalendarDayButton)calItems).FontWeight = FontWeights.Bold;   
                    } 

#endif
                    else
                    {
                        SetAppointmentDateStyle(calItems);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the displayed dates.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<DateTime> GetDisplayedDates()
        {
            if (this.CalendarToMakeBold != null)
            {
                if (this.CalendarToMakeBold.DisplayDateStart == null || this.CalendarToMakeBold.DisplayDateEnd == null)
                {
                    var disDate = (this.CalendarToMakeBold.SelectedDates.Count == 0) ? this.CalendarToMakeBold.DisplayDate: this.CalendarToMakeBold.SelectedDates[0];
                    var startdisplayDt = new DateTime(disDate.Year, disDate.Month, 1);
                    startdisplayDt = startdisplayDt.AddDays(-15);
                    var enddisplayDt = startdisplayDt.AddDays(60);
                    for (DateTime dates = startdisplayDt; dates <= enddisplayDt; dates = dates.AddDays(1))
                    {
                        yield return dates;
                    }
                }
                else
                {
                    var startdisplayDt = this.CalendarToMakeBold.DisplayDateStart.Value;
                    var enddisplayDt = this.CalendarToMakeBold.DisplayDateEnd.Value;
                    for (DateTime dates = startdisplayDt; dates <= enddisplayDt; dates = dates.AddDays(1))
                    {
                        yield return dates;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the SelectedDatesChanged event of the calendar control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        void calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {

#if !SILVERLIGHT
            var calendar = (Syncfusion.Windows.Controls.Calendar)sender;
#else
            var calendar = (System.Windows.Controls.Calendar)sender;
#endif
            if (this.currentCalendar != calendar)
            {
                // clear the dates in previous calendar
                this.ResetPreviousCalendar(this.currentCalendar);
                this.SetCalendar(calendar);
            }

            if (!this.isInSuspend && this.currentCalendar.SelectedDates.Count >= 8)
            {
                this.isInSuspend = true;
                var count = calendar.SelectedDates.Count;
                var firstDate = calendar.SelectedDates[0];
                var lastDate = calendar.SelectedDates[calendar.SelectedDates.Count - 1];
                if (lastDate < firstDate)
                {
                    firstDate = lastDate;
                    lastDate = calendar.SelectedDates[0];
                }
                var startDate = firstDate.StartOfWeek(DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek);
                if (lastDate.DayOfWeek != DayOfWeek.Saturday)
                {
                    lastDate = lastDate.AddDays(7).StartOfWeek(DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek).SubractDays(1);
                }
                var endDate = lastDate;
                calendar.SelectedDates.Clear();
                DateTime index = startDate;
                calendar.SelectedDates.Clear();
                for (int i = 0; index != endDate; i++)
                {
                    calendar.SelectedDates.Add(index);
                    index = index.AddDays(1);
                }
                calendar.SelectedDates.Add(index);
                this.isInSuspend = false;
            }

            if (!this.isInSuspend)
            {
                PrepareSelectedDatesToModel();
            }

            SelectOtherCalendarsDate(calendar);
        }
#if !SILVERLIGHT
        private void SelectOtherCalendarsDate(Syncfusion.Windows.Controls.Calendar calendar)
        {
            foreach (Syncfusion.Windows.Controls.Calendar cal in this.Items)
            {
                cal.SelectedDatesChanged -= new EventHandler<SelectionChangedEventArgs>(calendar_SelectedDatesChanged);
            }
            foreach (Syncfusion.Windows.Controls.Calendar cal in this.Items)
            {
                if (cal == calendar) continue;
                foreach (var item in Model.SelectedDates)
                {
                    cal.SelectedDates.Add(item);
                }
            }
            foreach (Syncfusion.Windows.Controls.Calendar cal in this.Items)
            {
                cal.SelectedDatesChanged += new EventHandler<SelectionChangedEventArgs>(calendar_SelectedDatesChanged);
            }
#else
        private void SelectOtherCalendarsDate(System.Windows.Controls.Calendar calendar)
        {
            foreach (System.Windows.Controls.Calendar cal in this.Items)
            {
                cal.SelectedDatesChanged -= new EventHandler<SelectionChangedEventArgs>(calendar_SelectedDatesChanged);
            }
            foreach (System.Windows.Controls.Calendar cal in this.Items)
            {
                if (cal == calendar) continue;
                foreach (var item in Model.SelectedDates)
                {
                    cal.SelectedDates.Add(item);
                }
            }
            foreach (System.Windows.Controls.Calendar cal in this.Items)
            {
                cal.SelectedDatesChanged += new EventHandler<SelectionChangedEventArgs>(calendar_SelectedDatesChanged);
            }
#endif
        }

        /// <summary>
        /// Prepares the selected dates to model.
        /// </summary>
        private void PrepareSelectedDatesToModel()
        {
            if (this.Model == null)
            {
                return;
            }

            // refresh selection state as in Outlook, when calendar days are changed we persist the selection over different days
            var firstDate = this.currentCalendar.SelectedDates[0];
            var lastDate = this.currentCalendar.SelectedDates[this.currentCalendar.SelectedDates.Count - 1];
            if (firstDate != lastDate)
            {
                this.UpdateSelectedState(firstDate, lastDate);
            }
            else
            {
                this.UpdateSelectedState(firstDate, firstDate);
            }

            var selectedDates = new ObservableCollection<DateTime>();
            foreach (var dateTime in this.currentCalendar.SelectedDates)
            {
                selectedDates.Add(dateTime);
            }
            this.Model.SelectedDates = selectedDates;
        }

        /// <summary>
        /// Prepares the selected dates to calendar.
        /// </summary>
        private void PrepareSelectedDatesToCalendar()
        {
            if (this.Model == null || this.currentCalendar == null)
            {
                return;
            }

            this.isInSuspend = true;
            this.currentCalendar.SelectedDates.Clear();

            int TotalSelectedDates = this.model.SelectedDates.Count;
            this.currentCalendar.SelectedDates.AddRange(this.model.SelectedDates[0], this.model.SelectedDates[TotalSelectedDates - 1]);
           

            //foreach (var dateTime in this.Model.SelectedDates)
            //{
            //    this.currentCalendar.SelectedDates.Add(dateTime);
            //}
            this.isInSuspend = false;
        }

        /// <summary>
        /// Updates the state of the selected.
        /// </summary>
        /// <param name="firstDate">The first date.</param>
        /// <param name="lastDate">The last date.</param>
        private void UpdateSelectedState(DateTime firstDate, DateTime lastDate)
        {
            var firstDateDiff = this.Model.SelectedStartTimeSpan > firstDate ? this.Model.SelectedStartTimeSpan.Day - firstDate.Day : firstDate.Day - this.Model.SelectedStartTimeSpan.Day;
            this.Model.SelectedStartTimeSpan = this.Model.SelectedStartTimeSpan.AddDays(firstDateDiff);

            var lastDateDiff = this.Model.SelectedEndTimeSpan > lastDate ? this.Model.SelectedEndTimeSpan.Day - lastDate.Day : lastDate.Day - this.Model.SelectedEndTimeSpan.Day;
            this.Model.SelectedEndTimeSpan = this.Model.SelectedEndTimeSpan.AddDays(lastDateDiff);
        }
#if !SILVERLIGHT
        private void SetCalendar(Syncfusion.Windows.Controls.Calendar calendar)
#else
        /// <summary>
        /// Sets the calendar.
        /// </summary>
        /// <param name="calendar">The calendar.</param>
        private void SetCalendar(System.Windows.Controls.Calendar calendar)
#endif
        {
            if (this.Model == null)
            {
                return;
            }

            this.currentCalendar = calendar;
        }

#if !SILVERLIGHT
        private void ResetPreviousCalendar(Syncfusion.Windows.Controls.Calendar calendar)
#else
        /// <summary>
        /// Resets the previous calendar.
        /// </summary>
        /// <param name="calendar">The calendar.</param>
        private void ResetPreviousCalendar(System.Windows.Controls.Calendar calendar)
#endif

        {
            this.isInSuspend = true;
            calendar.SelectedDates.Clear();
            calendar.SelectedDate = null;
            this.isInSuspend = false;
        }

        private bool isInSuspend = false;
        /// <summary>
        /// Suspends the events.
        /// </summary>
        public void SuspendEvents()
        {
            this.isInSuspend = true;
        }

        /// <summary>
        /// Resumes the events.
        /// </summary>
        public void ResumeEvents()
        {
            this.isInSuspend = false;
        }

        /// <summary>
        /// Handles the CollectionChanged event of the SelectedDates control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        void SelectedDates_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (this.currentCalendar == null || this.Model.IsInSuspend)
            {
                return;
            }

            this.isInSuspend = true;
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (DateTime selectedDate in e.NewItems)
                {
                    this.currentCalendar.SelectedDates.Add(selectedDate);
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (DateTime item in e.OldItems)
                {
                    this.currentCalendar.SelectedDates.Remove(item);
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                this.currentCalendar.SelectedDates.Clear();
            }
            this.isInSuspend = false;
        }

        #endregion

        private bool isTemplateApplied = false;
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.isTemplateApplied = true;
            this.PopulateView();
            this.PrepareSelectedDatesToCalendar();
        }
    }
}
