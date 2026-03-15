using System;
using System.Windows;
using System.Windows.Controls;
using Utilities.WPF;
using MSModel;
using System.Collections.Generic;
using System.ComponentModel;
using TranslationHelpers;
using DevExpress.Xpf.Editors;

namespace MSSchedulerSettings.Controls
{
    /// <summary>
    /// Interaction logic for ButtonControl.xaml
    /// </summary>
    public partial class WeeklyPlanItemEdit : UserControl, INotifyPropertyChanged
    {
        #region Events
        public event EventHandler<WeeklyCalendarItem> Delete;
        protected void OnDelete()
        {
            Delete?.Invoke(this, null);
        }
        public event EventHandler<WeeklyCalendarItem> Ok;
        protected void OnOk()
        {
            Ok?.Invoke(this, null);
        }
        public event EventHandler<WeeklyCalendarItem> Cancel;
        protected void OnCancel()
        {
            Cancel?.Invoke(this, null);
        }
        #endregion

        #region declaration
        bool bLoaded;
        DateTime _date1 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 7);
        DateTime _date2 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 7);
        WeeklyCalendarItem weeklyCalendarItem;
        bool isNewAppointment;
        WAppointment appointment;
        DateTime workViewStartTime;
        DateTime workViewEndTime;
        DateTime firstMonday = DateTime.MinValue;
        bool limitStartEndTime;
        #endregion
        #region Constructors
        public WeeklyPlanItemEdit(WeeklyCalendarItem weeklyCalendarItem, bool isNewAppointment, IDictionary<String, String> stringlist, string stringPlaceolder,
        DateTime workViewStartTime, DateTime workViewEndTime, bool limitStartEndTime)
        {
            InitializeComponent();

            var now = DateTime.Now;
            var actDay = new DateTime(now.Year, now.Month, 1, 0, 0, 0);
            while (actDay <= now || firstMonday == DateTime.MinValue) {
                if (actDay.DayOfWeek == DayOfWeek.Monday)
                    firstMonday = new DateTime(actDay.Ticks);
                actDay = actDay.AddDays(1);
            }

            this.weeklyCalendarItem = weeklyCalendarItem;
            this.isNewAppointment = isNewAppointment;

            this.workViewStartTime = workViewStartTime;
            this.workViewEndTime = workViewEndTime;
            this.limitStartEndTime = limitStartEndTime;

            Loaded += (o, e) =>
            {
                if (!bLoaded)
                {
                    bLoaded = true;
                    if (isNewAppointment)
                        deleteBtn.IsEnabled = false;
                    if ((int)weeklyCalendarItem.StartDate.DayOfWeek == 0)
                        cmbStartDay.SelectedIndex = 6;
                    else
                        cmbStartDay.SelectedIndex = (int)weeklyCalendarItem.StartDate.DayOfWeek - 1;
                    if ((int)weeklyCalendarItem.EndDate.DayOfWeek == 0)
                        cmbEndDay.SelectedIndex = 6;
                    else
                        cmbEndDay.SelectedIndex = (int)weeklyCalendarItem.EndDate.DayOfWeek - 1;

                    if (weeklyCalendarItem.EndDate.Hour == 0 && weeklyCalendarItem.EndDate.Minute == 0 && weeklyCalendarItem.EndDate.Second == 0)
                    {
                        cmbEndDay.SelectedIndex -= 1;
                        if (cmbEndDay.SelectedIndex == -1)
                            cmbEndDay.SelectedIndex = 6;
                    }

                    if (stringlist != null)
                    {
                        TranlslateText(stringlist, stringPlaceolder);
                    }
                    cancelBtn.IsCancel = true;
                    appointment = new WAppointment()
                    {
                        DisplayStartTime = weeklyCalendarItem.StartDate,
                        DisplayEndTime = weeklyCalendarItem.EndDate,
                        AllDay = weeklyCalendarItem.StartDate.Hour == 0 && weeklyCalendarItem.StartDate.Minute == 0 && weeklyCalendarItem.StartDate.Second == 0 &&
                        weeklyCalendarItem.EndDate.Hour == 0 && weeklyCalendarItem.EndDate.Minute == 0 && weeklyCalendarItem.EndDate.Second == 0
                    };
                    LimitStartTime();
                    LimitEndTime();
                    this.DataContext = appointment;
                }
            };
        }
        #endregion
        #region methods
        public void TranlslateText(IDictionary<String, String> stringlist, string stringPlaceolder)
        {
            weeklyItemEditFormStartTime.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_WeeklyItemEditFormStartTime", stringlist, Properties.Resources.WeeklyItemEditFormStartTime);
            weeklyItemEditFormEndTime.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_WeeklyItemEditFormEndTime", stringlist, Properties.Resources.WeeklyItemEditFormEndTime);
            chkAllDay.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_WeeklyItemAllDayEvent", stringlist, Properties.Resources.WeeklyItemAllDayEvent);
            okBtn.Content = okBtn.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_WeeklyItemEditFormOk", stringlist, Properties.Resources.WeeklyItemEditFormOk);
            cancelBtn.Content = cancelBtn.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_WeeklyItemEditFormCancel", stringlist, Properties.Resources.WeeklyItemEditFormCancel);
            deleteBtn.Content = deleteBtn.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_WeeklyItemEditFormDelete", stringlist, Properties.Resources.WeeklyItemEditFormDelete);
            chkRecurrence.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_WeeklyItemApplyRecurrence", stringlist, Properties.Resources.WeeklyItemApplyRecurrence);
            everyDays.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_RecurrenceFiledEvery", stringlist, Properties.Resources.RecurrenceFiledEvery);
            everyDayNumbersPostFix.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_RecurrenceFiledDays", stringlist, Properties.Resources.RecurrenceFiledDays);
            everyWeekDays.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_RecurrenceFiledEveryWeekDays", stringlist, Properties.Resources.RecurrenceFiledEveryWeekDays);
        }
        void SetDates()
        {
            if (cmbEndDay.SelectedIndex > cmbStartDay.SelectedIndex)
            {
                _date1 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, firstMonday.Day + cmbStartDay.SelectedIndex);
                _date2 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, firstMonday.Day + cmbEndDay.SelectedIndex);

                if(appointment.AllDay)
                {
                    _date2 = _date2.AddHours(24);
                }
                else
                {
                    _date1 = _date1.AddHours(appointment.DisplayStartTime.Hour);
                    _date1 = _date1.AddMinutes(appointment.DisplayStartTime.Minute);
                    _date1 = _date1.AddSeconds(appointment.DisplayStartTime.Second);

                    _date2 = _date2.AddHours(appointment.DisplayEndTime.Hour);
                    _date2 = _date2.AddMinutes(appointment.DisplayEndTime.Minute);
                    _date2 = _date2.AddSeconds(appointment.DisplayEndTime.Second);
                }
            }
            else if (cmbEndDay.SelectedIndex < cmbStartDay.SelectedIndex)
            {
                _date2 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, firstMonday.Day + cmbStartDay.SelectedIndex);
                _date1 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, firstMonday.Day + cmbEndDay.SelectedIndex);

                if (appointment.AllDay)
                {
                    _date1 = _date1.AddHours(24);
                }
                else
                {
                    _date2 = _date2.AddHours(appointment.DisplayStartTime.Hour);
                    _date2 = _date2.AddMinutes(appointment.DisplayStartTime.Minute);
                    _date2 = _date2.AddSeconds(appointment.DisplayStartTime.Second);

                    _date1 = _date1.AddHours(appointment.DisplayEndTime.Hour);
                    _date1 = _date1.AddMinutes(appointment.DisplayEndTime.Minute);
                    _date1 = _date1.AddSeconds(appointment.DisplayEndTime.Second);
                }
                
            }
            else
            {
                _date1 = firstMonday.AddDays(cmbStartDay.SelectedIndex);
                _date2 = firstMonday.AddDays(cmbStartDay.SelectedIndex);

                if (appointment.AllDay)
                {
                    _date2 = _date2.AddHours(24);
                }
                else
                {
                    if (appointment.DisplayStartTime <= appointment.DisplayEndTime)
                    {
                        _date1 = _date1.AddHours(appointment.DisplayStartTime.Hour);
                        _date1 = _date1.AddMinutes(appointment.DisplayStartTime.Minute);
                        _date1 = _date1.AddSeconds(appointment.DisplayStartTime.Second);

                        _date2 = _date2.AddHours(appointment.DisplayEndTime.Hour);
                        _date2 = _date2.AddMinutes(appointment.DisplayEndTime.Minute);
                        _date2 = _date2.AddSeconds(appointment.DisplayEndTime.Second);
                    }
                    else
                    {
                        _date2 = _date2.AddHours(appointment.DisplayStartTime.Hour);
                        _date2 = _date2.AddMinutes(appointment.DisplayStartTime.Minute);
                        _date2 = _date2.AddSeconds(appointment.DisplayStartTime.Second);

                        _date1 = _date1.AddHours(appointment.DisplayEndTime.Hour);
                        _date1 = _date1.AddMinutes(appointment.DisplayEndTime.Minute);
                        _date1 = _date1.AddSeconds(appointment.DisplayEndTime.Second);
                    }
                }                
            }
        }

        void OnEdtEndTimeValidate(object sender, ValidationEventArgs e)
        {
            if (e.Value == null)
                return;

            e.IsValid = IsValidInterval(appointment.DisplayStartTime.TimeOfDay, ((DateTime)e.Value).TimeOfDay);
            if (limitStartEndTime)
                e.ErrorContent = Properties.Resources.InvalidEndTimeInWorkView;
            else
                e.ErrorContent = Properties.Resources.InvalidEndTime;
        }
        private void OnEdtStartTimeValidate(object sender, ValidationEventArgs e)
        {
            if (e.Value == null)
                return;
            e.IsValid = IsValidInterval(((DateTime)e.Value).TimeOfDay, appointment.DisplayEndTime.TimeOfDay);
            if (limitStartEndTime)
                e.ErrorContent = Properties.Resources.InvalidStartTimeInWorkView;
            else
                e.ErrorContent = Properties.Resources.InvalidStartTime;
        }

        void LimitStartTime()
        {
            if (onStartValueChanging)
                return;
            if (limitStartEndTime)
            {
                onStartValueChanging = true;
                try
                {
                    if (appointment.DisplayStartTime.TimeOfDay < workViewStartTime.TimeOfDay)
                    {
                        appointment.DisplayStartTime = ChangeTime(appointment.DisplayStartTime, workViewStartTime.TimeOfDay);
                        edtEndTime.GetBindingExpression(TextEdit.EditValueProperty).UpdateTarget();
                    }
                    else if (appointment.DisplayStartTime.TimeOfDay > workViewEndTime.TimeOfDay)
                    {
                        appointment.DisplayStartTime = ChangeTime(appointment.DisplayStartTime, workViewEndTime.TimeOfDay);
                        edtEndTime.GetBindingExpression(TextEdit.EditValueProperty).UpdateTarget();
                    }
                }
                finally
                {
                    onStartValueChanging = false;
                }
            }
        }

        static DateTime ChangeTime(DateTime dateTime, TimeSpan timeSpan)
        {
            return new DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                timeSpan.Hours,
                timeSpan.Minutes,
                timeSpan.Seconds,
                dateTime.Kind);
        }

        bool onStartValueChanging;
        bool onEndValueChanging;
        void LimitEndTime()
        {
            if (onEndValueChanging)
                return;
            if (limitStartEndTime)
            {
                onEndValueChanging = true;
                try
                {
                    if (appointment.DisplayEndTime.TimeOfDay < workViewStartTime.TimeOfDay)
                    {
                        appointment.DisplayEndTime = ChangeTime(appointment.DisplayEndTime, workViewStartTime.TimeOfDay);
                        edtEndTime.GetBindingExpression(TextEdit.EditValueProperty).UpdateTarget();
                    }
                    else if (appointment.DisplayEndTime.TimeOfDay > workViewEndTime.TimeOfDay)
                    {
                        appointment.DisplayEndTime = ChangeTime(appointment.DisplayEndTime, workViewEndTime.TimeOfDay);
                        edtEndTime.GetBindingExpression(TextEdit.EditValueProperty).UpdateTarget();
                    }
                }
                finally
                {
                    onEndValueChanging = false;
                }
            }
        }

        protected internal virtual bool IsValidInterval(TimeSpan startTime, TimeSpan endTime)
        {
            bool res = (cmbEndDay.SelectedIndex > cmbStartDay.SelectedIndex || cmbEndDay.SelectedIndex < cmbStartDay.SelectedIndex ||
                (cmbEndDay.SelectedIndex == cmbStartDay.SelectedIndex && startTime != endTime));
            if(limitStartEndTime)
                res = res && startTime >= workViewStartTime.TimeOfDay && endTime <= workViewEndTime.TimeOfDay;
            return res;
        }

        internal List<Recurrence> GetRucerrence()
        {
            List<Recurrence> recurrences = new List<Recurrence>();
            if (!(bool)chkRecurrence.IsChecked)
                return recurrences;
            try
            {
                int delta = 1;
                int start = 1;
                if (appointment.EveryDays)
                {
                    delta = appointment.EveryDayNumbers;
                    start = cmbStartDay.SelectedIndex;
                }
                else if (appointment.EveryWeekDay)
                    delta = 1;
                for (int i = 1; i <= (7 - start) / delta; i++)
                {
                    _date1 = _date1.AddDays(delta);
                    _date2 = _date2.AddDays(delta);
                    if(_date1.DayOfWeek == 0 || (_date1.DayOfWeek != 0 && (int)_date1.DayOfWeek - 1 >= cmbStartDay.SelectedIndex))
                        recurrences.Add(new Recurrence() { Date1 = _date1, Date2 = _date2 });
                }
            }
            catch (Exception)
            {
            }
            return recurrences;
        }

        #endregion
        #region EventHandlres
        void OnOKButtonClick(object sender, RoutedEventArgs e)
        {
            SetDates();
            weeklyCalendarItem.StartDate = _date1;
            weeklyCalendarItem.EndDate = _date2;
            CloseEdit(true);
        }

        private void cmbEndDay_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            edtStartTime.DoValidate();
            edtEndTime.DoValidate();
            if(cmbStartDay.SelectedIndex == 6)
            {
                cmbEndDay.SelectedIndex = 6;
                cmbEndDay.IsEnabled = false;
            }
            else
                cmbEndDay.IsEnabled = true;
        }

        private void edtStartTime_SpinDownClick(object sender, RoutedEventArgs e)
        {
            edtStartTime.SpinDown();
        }

        private void edtStartTime_SpinUpClick(object sender, RoutedEventArgs e)
        {
            edtStartTime.SpinUp();
        }

        private void edtEndTime_SpinDownClick(object sender, RoutedEventArgs e)
        {
            edtEndTime.SpinDown();
        }

        private void edtEndTime_SpinUpClick(object sender, RoutedEventArgs e)
        {
            edtEndTime.SpinUp();
        }
        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            CloseEdit(false);
        }
        private void OnDeleteButtonClick(object sender, RoutedEventArgs e)
        {
            if (isNewAppointment)
                return;
            OnDelete();
            CloseEdit(false);
        }
        void CloseEdit(bool returnValue)
        {
            if (returnValue)
                OnOk();
            else
                OnCancel();
        }
        #endregion


        #region INotifyPropertyChanged Members
        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                //DispatcherObject dispatcherObject = handler.Target as DispatcherObject;

                var e = new PropertyChangedEventArgs(propertyName);
                //// If the subscriber is a DispatcherObject and different thread
                //if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                //{
                //    // Invoke handler in the target dispatcher's thread
                //    dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);
                //}
                //else // Execute handler as is
                handler(this, e);
            }
        }
        #endregion
    }

    public class Recurrence
    {
        public DateTime Date1 { get; set; }
        public DateTime Date2 { get; set; }
    }
    
    public class WAppointment: INotifyPropertyChanged
    {
        DateTime displayStartTime;
        DateTime displayEndTime;
        bool allDay;
        bool everyDays = true;
        int everyDayNumbers = 1;
        bool everyWeekDay;
        public DateTime DisplayStartTime 
        { 
            get
            {
                return displayStartTime;
            }
            set
            {
                displayStartTime = value;
                OnPropertyChanged("DisplayStartTime");
            }
        }
        public DateTime DisplayEndTime
        {
            get
            {
                return displayEndTime;
            }
            set
            {
                displayEndTime = value;
                OnPropertyChanged("DisplayEndTime");
            }
        }
        public bool AllDay
        {
            get
            {
                return allDay;
            }
            set
            {
                allDay = value;
                OnPropertyChanged("AllDay");
            }
        }
        public bool EveryDays
        {
            get
            {
                return everyDays;
            }
            set
            {
                everyDays = value;
                OnPropertyChanged("EveryDays");
            }
        }
        public bool EveryWeekDay
        {
            get
            {
                return everyWeekDay;
            }
            set
            {
                everyWeekDay = value;
                OnPropertyChanged("EveryWeekDay");
            }
        }
        public int EveryDayNumbers
        {
            get
            {
                return everyDayNumbers;
            }
            set
            {
                if(everyDayNumbers != value)
                {
                    everyDayNumbers = Math.Min(Math.Max(value, 1), 7);
                    OnPropertyChanged("EveryDayNumbers");
                }
            }
        }

        #region INotifyPropertyChanged Members
        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                //DispatcherObject dispatcherObject = handler.Target as DispatcherObject;

                var e = new PropertyChangedEventArgs(propertyName);
                //// If the subscriber is a DispatcherObject and different thread
                //if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                //{
                //    // Invoke handler in the target dispatcher's thread
                //    dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);
                //}
                //else // Execute handler as is
                handler(this, e);
            }
        }
        #endregion
    }
}
