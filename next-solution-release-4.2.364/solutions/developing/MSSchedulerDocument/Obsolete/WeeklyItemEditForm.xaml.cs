using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.Xpf.Scheduler;
using DevExpress.XtraScheduler;
using DevExpress.Xpf.Scheduler.UI;
using DevExpress.Xpf.Core;
using DevExpress.XtraScheduler.Localization;
using System.Globalization;
using DevExpress.Xpf.Editors;
using DevExpress.XtraScheduler.UI;
using TranslationHelpers;
using System.ComponentModel;
using Utilities.Converters;

namespace MSSchedulerSettings.Controls {

    //
    // Summary:
    //     Specifies the day of the week.
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum CDayOfWeek
    {
        //
        // Summary:
        //     Indicates Monday.
        Monday = 0,
        //
        // Summary:
        //     Indicates Tuesday.
        Tuesday = 1,
        //
        // Summary:
        //     Indicates Wednesday.
        Wednesday = 2,
        //
        // Summary:
        //     Indicates Thursday.
        Thursday = 3,
        //
        // Summary:
        //     Indicates Friday.
        Friday = 4,
        //
        // Summary:
        //     Indicates Saturday.
        Saturday = 5,
        //
        // Summary:
        //     Indicates Sunday.
        Sunday = 6,
    }

    public class LocalizedEnumConverter : ResourceEnumConverter
    {
        public LocalizedEnumConverter(Type type)
            : base(type, Properties.Resources.ResourceManager)
        {

        }
    }

    /// <summary>
    /// Interaction logic for CustomAppointmentForm.xaml
    /// </summary>
    public partial class WeeklyItemEditForm : UserControl {
        SchedulerControl control;
        Appointment appointment;
        WeeklyItemEditFormController controller;
        RecurrenceVisualController recurrenceVisualController;
        bool hasOccurence;
        bool bInit;
        DateTime _date1 = new DateTime(1991, 1, 7);
        DateTime _date2 = new DateTime(1991, 1, 7);
        String formTitle = Properties.Resources.WeeklyItemEditFormTitle;

        public WeeklyItemEditForm(SchedulerControl control, Appointment appointment, IDictionary<String, String> stringlist, string stringPlaceolder) {
            this.control = control;
            this.appointment = appointment;
            this.controller = new WeeklyItemEditFormController(Control, Appointment);
            //Controller.PrepareToRecurrenceEdit();

            this.recurrenceVisualController = new RecurrenceVisualController(Controller);
            RecurrenceVisualController.EnableYearlyRecurrence = false;
            RecurrenceVisualController.EnableWeeklyRecurrence = false;
            RecurrenceVisualController.EnableNoneRecurrence = false;
            RecurrenceVisualController.EnableDailyRecurrence = true;
            //hasOccurence = !appointment.IsBase;
            RecurrenceVisualController.EnableRecurrence = false; // hasOccurence;
            InitializeComponent();

            //cmbLabel.ItemsSource = Controller.Storage.AppointmentStorage.Labels;
            //cmbSubject.ItemsSource = Controller.Storage.AppointmentStorage.Statuses;
            cmbStartDay.ItemsSource = Enum.GetValues(typeof(CDayOfWeek));
            if ((int)appointment.Start.DayOfWeek == 0)
                cmbStartDay.SelectedIndex = 6;
            else
                cmbStartDay.SelectedIndex = (int)appointment.Start.DayOfWeek - 1;

            cmbEndDay.ItemsSource = Enum.GetValues(typeof(CDayOfWeek));
            if((int)appointment.End.DayOfWeek == 0)
                cmbEndDay.SelectedIndex = 6;
            else
                cmbEndDay.SelectedIndex = (int)appointment.End.DayOfWeek - 1;

            bInit = true;

            if (stringlist != null)
            {
                TranlslateText(stringlist, stringPlaceolder);
            }
            Loaded += new RoutedEventHandler(OnLoaded);
            cancelBtn.IsCancel = true;
        }
        public void TranlslateText(IDictionary<String, String> stringlist, string stringPlaceolder)
        {
            weeklyItemEditFormStartTime.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_WeeklyItemEditFormStartTime", stringlist, Properties.Resources.WeeklyItemEditFormStartTime);
            weeklyItemEditFormEndTime.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_WeeklyItemEditFormEndTime", stringlist, Properties.Resources.WeeklyItemEditFormEndTime);
            chkAllDay.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_WeeklyItemAllDayEvent", stringlist, Properties.Resources.WeeklyItemAllDayEvent);
            okBtn.Content = okBtn.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_WeeklyItemEditFormOk", stringlist, Properties.Resources.WeeklyItemEditFormOk);
            cancelBtn.Content = cancelBtn.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_WeeklyItemEditFormCancel", stringlist, Properties.Resources.WeeklyItemEditFormCancel);
            deleteBtn.Content = deleteBtn.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_WeeklyItemEditFormDelete", stringlist, Properties.Resources.WeeklyItemEditFormDelete);
            formTitle = TranslationHelper.TranlslateText($"_{stringPlaceolder}_WeeklyItemEditFormTitle", stringlist, Properties.Resources.WeeklyItemEditFormTitle);
            chkRecurrence.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_WeeklyItemApplyRecurrence", stringlist, Properties.Resources.WeeklyItemApplyRecurrence);
        }
        public SchedulerControl Control { get { return control; } }
        public Appointment Appointment { get { return appointment; } }
        public WeeklyItemEditFormController Controller { get { return controller; } }
        protected internal bool IsNewAppointment { get { return controller != null ? controller.IsNewAppointment : true; } }
        public string TimeEditMask { get { return CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern/*LongTimePattern*/; } }
        public RecurrenceVisualController RecurrenceVisualController { get { return recurrenceVisualController; } }
        public bool ShouldShowRecurrence { get { return !Appointment.IsOccurrence && Controller.ShouldShowRecurrenceButton; } }
        public TimeZoneHelper TimeZoneHelper { get { return Controller.TimeZoneHelper; } }
        void OnLoaded(object sender, RoutedEventArgs e) {
            SchedulerFormBehavior.SetTitle(this, formTitle);
            Size currentSize = SchedulerFormBehavior.GetSize(this);
            SchedulerFormBehavior.SetSize(this, new Size(currentSize.Width /*+ recurrenceGrid.Width*/, double.NaN));

        }
        void OnOKButtonClick(object sender, RoutedEventArgs e) {
            if(!controller.IsConflictResolved()) {
                DXMessageBox.Show(SchedulerLocalizer.GetString(SchedulerStringId.Msg_Conflict), Properties.Resources.WeeklyConflict, System.Windows.MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            SetDates();
            try
            {

                Controller.Start = _date1;
                Controller.End = _date2;
                Controller.ApplyChanges();

                if (RecurrenceVisualController.EnableRecurrence)
                {
                    try
                    {
                        int delta = RecurrenceVisualController.RecurrenceInfo.WeekDays == WeekDays.WorkDays ? 1 : RecurrenceVisualController.RecurrenceInfo.Periodicity;
                        for (int i = _date1.Day; i < 12; i++)
                        {
                            _date1 = _date1.AddDays(delta);
                            _date2 = _date2.AddDays(delta);
                            var apt = control.Storage.CreateAppointment(AppointmentType.Normal, _date1, _date2);
                            controller = new WeeklyItemEditFormController(Control, apt);
                            Controller.Start = _date1;
                            Controller.End = _date2;
                            Controller.ApplyChanges();
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch (Exception)
            {
            }

            //resize dates
            //DateTime dti = new DateTime(1991, 1, 7);
            //DateTime adt = appointment.Start;
            //for (int i = 0; i < 2; i++)
            //{
            //    dti = new DateTime(1991, 1, 7);
            //    if (i == 0)
            //    {
            //        adt = appointment.Start;
            //        if (adt.DayOfWeek != (DayOfWeek)cmbStartDay.SelectedIndex)
            //        {
            //            adt = adt.AddDays(cmbStartDay.SelectedIndex - (int)adt.DayOfWeek);
            //        }
            //    }
            //    else
            //    {
            //        adt = appointment.End;
            //        if (adt.DayOfWeek != (DayOfWeek)cmbEndDay.SelectedIndex)
            //        {
            //            adt = adt.AddDays(cmbEndDay.SelectedIndex - (int)adt.DayOfWeek);
            //        }
            //    }
            //    switch (adt.DayOfWeek)
            //    {
            //        case DayOfWeek.Monday:
            //            break;
            //        case DayOfWeek.Tuesday:
            //            dti = dti.AddDays(1);
            //            break;
            //        case DayOfWeek.Wednesday:
            //            dti = dti.AddDays(2);
            //            break;
            //        case DayOfWeek.Thursday:
            //            dti = dti.AddDays(3);
            //            break;
            //        case DayOfWeek.Friday:
            //            dti = dti.AddDays(4);
            //            break;
            //        case DayOfWeek.Saturday:
            //            dti = dti.AddDays(5);
            //            break;
            //        case DayOfWeek.Sunday:
            //            dti = dti.AddDays(6);
            //            break;
            //    }
            //    dti = dti.AddHours(adt.Hour);
            //    dti = dti.AddMinutes(adt.Minute);
            //    dti = dti.AddSeconds(adt.Second);
            //    if(i==0)
            //        appointment.Start = dti;
            //    else
            //        appointment.End = dti;
            //}

            SchedulerFormBehavior.Close(this, true);
        }
        void SetDates()
        {
            if (cmbEndDay.SelectedIndex > cmbStartDay.SelectedIndex)
            {
                _date1 = new DateTime(1991, 1, 7 + cmbStartDay.SelectedIndex);
                _date2 = new DateTime(1991, 1, 7 + cmbEndDay.SelectedIndex);


                _date1 = _date1.AddHours(Controller.DisplayStartTime.Hours);
                _date1 = _date1.AddMinutes(Controller.DisplayStartTime.Minutes);
                _date1 = _date1.AddSeconds(Controller.DisplayStartTime.Seconds);

                _date2 = _date2.AddHours(Controller.DisplayEndTime.Hours);
                _date2 = _date2.AddMinutes(Controller.DisplayEndTime.Minutes);
                _date2 = _date2.AddSeconds(Controller.DisplayEndTime.Seconds);
            }
            else if (cmbEndDay.SelectedIndex < cmbStartDay.SelectedIndex)
            {
                _date2 = new DateTime(1991, 1, 7 + cmbStartDay.SelectedIndex);
                _date1 = new DateTime(1991, 1, 7 + cmbEndDay.SelectedIndex);

                _date2 = _date2.AddHours(Controller.DisplayStartTime.Hours);
                _date2 = _date2.AddMinutes(Controller.DisplayStartTime.Minutes);
                _date2 = _date2.AddSeconds(Controller.DisplayStartTime.Seconds);

                _date1 = _date1.AddHours(Controller.DisplayEndTime.Hours);
                _date1 = _date1.AddMinutes(Controller.DisplayEndTime.Minutes);
                _date1 = _date1.AddSeconds(Controller.DisplayEndTime.Seconds);
            }
            else
            {
                _date1 = new DateTime(1991, 1, 7 + cmbStartDay.SelectedIndex);
                _date2 = new DateTime(1991, 1, 7 + cmbStartDay.SelectedIndex);

                if (Controller.DisplayStartTime <= Controller.DisplayEndTime)
                {
                    _date1 = _date1.AddHours(Controller.DisplayStartTime.Hours);
                    _date1 = _date1.AddMinutes(Controller.DisplayStartTime.Minutes);
                    _date1 = _date1.AddSeconds(Controller.DisplayStartTime.Seconds);

                    _date2 = _date2.AddHours(Controller.DisplayEndTime.Hours);
                    _date2 = _date2.AddMinutes(Controller.DisplayEndTime.Minutes);
                    _date2 = _date2.AddSeconds(Controller.DisplayEndTime.Seconds);
                }
                else
                {
                    _date2 = _date2.AddHours(Controller.DisplayStartTime.Hours);
                    _date2 = _date2.AddMinutes(Controller.DisplayStartTime.Minutes);
                    _date2 = _date2.AddSeconds(Controller.DisplayStartTime.Seconds);

                    _date1 = _date1.AddHours(Controller.DisplayEndTime.Hours);
                    _date1 = _date1.AddMinutes(Controller.DisplayEndTime.Minutes);
                    _date1 = _date1.AddSeconds(Controller.DisplayEndTime.Seconds);
                }
            }
        }
        private void OnCancelButtonClick(object sender, RoutedEventArgs e) {
            SchedulerFormBehavior.Close(this, false);
        }
        private void OnDeleteButtonClick(object sender, RoutedEventArgs e) {
            if(IsNewAppointment)
                return;
            Controller.DeleteAppointment();
            if (RecurrenceVisualController.EnableRecurrence)
            {
                try
                {
                    SetDates();
                    int delta = RecurrenceVisualController.RecurrenceInfo.WeekDays == WeekDays.WorkDays ? 1 : RecurrenceVisualController.RecurrenceInfo.Periodicity;
                    for (int i = _date1.Day; i < 12; i++)
                    {
                        _date1 = _date1.AddDays(delta);
                        _date2 = _date2.AddDays(delta);
                        var apt = control.Storage.GetAppointments(_date1, _date2).FirstOrDefault();
                        if (apt != null)
                        {
                            controller = new WeeklyItemEditFormController(Control, apt);
                            Controller.DeleteAppointment();
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
            SchedulerFormBehavior.Close(this, false);
        }
        protected override void OnVisualParentChanged(DependencyObject oldParent) {
            base.OnVisualParentChanged(oldParent);
            if(oldParent == null) {
                UpdateContainerCaption(Controller.Subject);
            }
        }
        void UpdateContainerCaption(string subject) {
            if (Controller.IsNewAppointment)
                SchedulerFormBehavior.SetTitle(this, "New appointment");
            else
                SchedulerFormBehavior.SetTitle(this, "Edit - [" + Appointment.Subject + "]");
        }
        void OnEdtEndTimeValidate(object sender, ValidationEventArgs e) {
            if (e.Value == null)
                return;

            e.IsValid = IsValidInterval(Controller.Start.TimeOfDay, ((DateTime)e.Value).TimeOfDay);
            e.ErrorContent = Properties.Resources.InvalidEndTime;
        }
        private void OnEdtStartTimeValidate(object sender, ValidationEventArgs e)
        {
            if (e.Value == null)
                return;
            e.IsValid = IsValidInterval(((DateTime)e.Value).TimeOfDay, Controller.End.TimeOfDay);
            e.ErrorContent = Properties.Resources.InvalidStartTime;
        }
        private void OnDaySelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            edtStartTime.DoValidate();
            edtEndTime.DoValidate();
        }

        protected internal virtual bool IsValidInterval(TimeSpan startTime, TimeSpan endTime)
        {
            return (cmbEndDay.SelectedIndex > cmbStartDay.SelectedIndex || (cmbEndDay.SelectedIndex == cmbStartDay.SelectedIndex && endTime >= startTime));
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
    }

    public class WeeklyItemEditFormController : AppointmentFormController {

        public string CustomName { get { return (string)EditedAppointmentCopy.CustomFields["CustomName"]; } set { EditedAppointmentCopy.CustomFields["CustomName"] = value; } }
        public string CustomStatus { get { return (string)EditedAppointmentCopy.CustomFields["CustomStatus"]; } set { EditedAppointmentCopy.CustomFields["CustomStatus"] = value; } }

        string SourceCustomName { get { return (string)SourceAppointment.CustomFields["CustomName"]; } set { SourceAppointment.CustomFields["CustomName"] = value; } }
        string SourceCustomStatus { get { return (string)SourceAppointment.CustomFields["CustomStatus"]; } set { SourceAppointment.CustomFields["CustomStatus"] = value; } }

        public WeeklyItemEditFormController(SchedulerControl control, Appointment apt)
            : base(control, apt) {
        }

        public override bool IsAppointmentChanged() {
            if(base.IsAppointmentChanged())
                return true;
            return SourceCustomName != CustomName ||
                SourceCustomStatus != CustomStatus;
        }

        protected override void ApplyCustomFieldsValues() {
            SourceCustomName = CustomName;
            SourceCustomStatus = CustomStatus;
        }
    }
}
