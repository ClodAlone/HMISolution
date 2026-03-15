#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Globalization;

namespace Syncfusion.UI.Xaml.Schedule
{
    /// <summary>
    ///  Represents a wrapper for the resources
    /// </summary>
    public class ResourceWrapper
    {
        #region Constants

        const string AppointmentEditValue = "AppointmentEdit";
        const string ErrorMessageValue = "ErrorMessage";
        const string AllDayValue = "AllDay";
        const string AprilValue = "April";
        const string AugustValue = "August";
        const string BusyValue = "Busy";
        const string CancelValue = "Cancel";
        const string ChooseDateValue = "ChooseDate";
        const string DailyValue = "Daily";
        const string DayValue = "Day";
        const string DaysValue = "Day(s)";
        const string DecemberValue = "December";
        const string DeleteValue = "Delete";
        const string AddValue = "Add";
        const string EditValue = "Edit";
        const string CopyValue = "Copy";
        const string PasteValue = "Paste";
        const string ResizeValue = "Resize";
        const string EighteenHoursValue = "EighteenHours";
        const string EightHoursValue = "EightHours";
        const string ElevenHoursValue = "ElevenHours";
        const string EndValue = "End";
        const string EndAfterValue = "EndAfter";
        const string EndByValue = "EndBy";
        const string EveryValue = "Every";
        const string EveryDayValue = "EveryDay";
        const string EveryMonthValue = "EveryMonth";
        const string EveryWeekValue = "EveryWeek";
        const string EveryWeekdayValue = "EveryWeekday";
        const string EveryWeekDaysValue = "EveryWeekDays";
        const string EveryYearValue = "EveryYear";
        const string FebruaryValue = "February";
        const string FifteenMinValue = "FifteenMin";
        const string FirstValue = "First";
        const string FiveHoursValue = "FiveHours";
        const string FiveMinValue = "FiveMin";
        const string FourDaysValue = "FourDays";
        const string FourHoursValue = "FourHours";
        const string FourthValue = "Fourth";
        const string FreeValue = "Free";
        const string FridayValue = "Friday";
        const string HalfDayValue = "HalfDay";
        const string HowOftenValue = "HowOften";
        const string JanuaryValue = "January";
        const string JulyValue = "July";
        const string JuneValue = "June";
        const string LocationValue = "Location";
        const string MarchValue = "March";
        const string MayValue = "May";
        const string MondayValue = "Monday";
        const string MonthsonValue = "Month(s)on";
        const string MonthlyValue = "Monthly";
        const string NineHoursValue = "NineHours";
        const string NoEndDateValue = "NoEndDate";
        const string NoneValue = "None";
        const string NotesValue = "Notes";
        const string NovemberValue = "November";
        const string OctoberValue = "October";
        const string ofValue = "of";
        const string onValue = "on";
        const string OnceValue = "Once";
        const string OneDayValue = "OneDay";
        const string OneHourValue = "OneHour";
        const string OneWeekValue = "OneWeek";
        const string OutOfOfficeValue = "Out Of Office";
        const string RangeOfRecurrenceValue = "RangeOfRecurrence";
        const string ReadOnlyValue = "ReadOnly";
        const string ReadOnlyOffValue = "ReadOnlyOff";
        const string ReadOnlyOnValue = "ReadOnlyOn";
        const string RecurEveryValue = "RecurEvery";
        const string RecurrenceValue = "Recurrence";
        const string RecurrenceDisabledValue = "RecurrenceDisabled";
        const string RecurrenceEnabledValue = "RecurrenceEnabled";
        const string ReminderValue = "Reminder";
        const string SaturdayValue = "Saturday";
        const string SaveValue = "Save";
        const string SecondValue = "Second";
        const string SeptemberValue = "September";
        const string SevenHoursValue = "SevenHours";
        const string ShowMoreValue = "ShowMore";
        const string SixHoursValue = "SixHours";
        const string StartValue = "Start";
        const string StartDateValue = "StartDate";
        const string StatusValue = "Status";
        const string SubjectValue = "Subject";
        const string SundayValue = "Sunday";
        const string TenHoursValue = "TenHours";
        const string TenMinValue = "TenMin";
        const string TentativeValue = "Tentative";
        const string TheValue = "The";
        const string ThirdValue = "Third";
        const string ThirtyMinValue = "ThirtyMin";
        const string ThreeDaysValue = "ThreeDays";
        const string ThreeHoursValue = "ThreeHours";
        const string ThursdayValue = "Thursday";
        const string TuesdayValue = "Tuesday";
        const string TweleveHoursValue = "TweleveHours";
        const string TwoDaysValue = "TwoDays";
        const string TwoHoursValue = "TwoHours";
        const string TwoWeeksValue = "TwoWeeks";
        const string WednesdayValue = "Wednesday";
        const string WeeksonValue = "Week(s)on";
        const string WeeklyValue = "Weekly";
        const string WhereValue = "Where";
        const string YearsonValue = "Year(s)on";
        const string YearlyValue = "Yearly";
        const string ZeroMinValue = "ZeroMin";
        const string EditRecurrenceValue = "EditRecurrence";
        const string RecurrencePatternValue = "RecurrencePattern";
        const string OKValue = "OK";
        const string RemoveRecurrenceValue = "RemoveRecurrence";

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.UI.Xaml.Schedule.ResourceWrapper">ResourceWrapper</see> class. 
        /// </summary>
        public ResourceWrapper()
        {
            CultureInfo ci = CultureInfo.CurrentUICulture;
            AppointmentEdit = SRSchedule.GetString(ci, AppointmentEditValue);
            ErrorMessage = SRSchedule.GetString(ci, ErrorMessageValue);
            AllDay = SRSchedule.GetString(ci, AllDayValue);
            August = SRSchedule.GetString(ci, AugustValue);
            April = SRSchedule.GetString(ci, AprilValue);
            Busy = SRSchedule.GetString(ci, BusyValue);
            Cancel = SRSchedule.GetString(ci, CancelValue);
            ChooseDate = SRSchedule.GetString(ci, ChooseDateValue);
            Daily = SRSchedule.GetString(ci, DailyValue);
            Day = SRSchedule.GetString(ci, DayValue);
            Days = SRSchedule.GetString(ci, DaysValue);
            December = SRSchedule.GetString(ci, DecemberValue);
            Delete = SRSchedule.GetString(ci, DeleteValue);
            Add = SRSchedule.GetString(ci, AddValue);
            Edit = SRSchedule.GetString(ci, EditValue);
            Copy = SRSchedule.GetString(ci, CopyValue);
            Paste = SRSchedule.GetString(ci, PasteValue);
            Resize = SRSchedule.GetString(ci, ResizeValue);
            EighteenHours = SRSchedule.GetString(ci, EighteenHoursValue);
            EightHours = SRSchedule.GetString(ci, EightHoursValue);
            ElevenHours = SRSchedule.GetString(ci, ElevenHoursValue);
            End = SRSchedule.GetString(ci, EndValue);
            EndAfter = SRSchedule.GetString(ci, EndAfterValue);
            EndBy = SRSchedule.GetString(ci, EndByValue);
            Every = SRSchedule.GetString(ci, EveryValue);
            EveryDay = SRSchedule.GetString(ci, EveryDayValue);
            EveryMonth = SRSchedule.GetString(ci, EveryMonthValue);
            EveryWeek = SRSchedule.GetString(ci, EveryWeekValue);
            EveryWeekday = SRSchedule.GetString(ci, EveryWeekdayValue);
            EveryWeekDays = SRSchedule.GetString(ci, EveryWeekDaysValue);
            EveryYear = SRSchedule.GetString(ci, EveryYearValue);
            February = SRSchedule.GetString(ci, FebruaryValue);
            FifteenMin = SRSchedule.GetString(ci, FifteenMinValue);
            First = SRSchedule.GetString(ci, FirstValue);
            FiveHours = SRSchedule.GetString(ci, FiveHoursValue);
            FiveMin = SRSchedule.GetString(ci, FiveMinValue);
            FourDays = SRSchedule.GetString(ci, FourDaysValue);
            FourHours = SRSchedule.GetString(ci, FourHoursValue);
            Fourth = SRSchedule.GetString(ci, FourthValue);
            Free = SRSchedule.GetString(ci, FreeValue);
            Friday = SRSchedule.GetString(ci, FridayValue);
            HalfDay = SRSchedule.GetString(ci, HalfDayValue);
            HowOften = SRSchedule.GetString(ci, HowOftenValue);
            January = SRSchedule.GetString(ci, JanuaryValue);
            July = SRSchedule.GetString(ci, JulyValue);
            June = SRSchedule.GetString(ci, JuneValue);
            Location = SRSchedule.GetString(ci, LocationValue);
            March = SRSchedule.GetString(ci, MarchValue);
            May = SRSchedule.GetString(ci, MayValue);
            Monday = SRSchedule.GetString(ci, MondayValue);
            Monthson = SRSchedule.GetString(ci, MonthsonValue);
            Monthly = SRSchedule.GetString(ci, MonthlyValue);
            NineHours = SRSchedule.GetString(ci, NineHoursValue);
            NoEndDate = SRSchedule.GetString(ci, NoEndDateValue);
            None = SRSchedule.GetString(ci, NoneValue);
            Notes = SRSchedule.GetString(ci, NotesValue);
            November = SRSchedule.GetString(ci, NovemberValue);
            October = SRSchedule.GetString(ci, OctoberValue);
            Of = SRSchedule.GetString(ci, ofValue);
            On = SRSchedule.GetString(ci, onValue);
            Once = SRSchedule.GetString(ci, OnceValue);
            OneDay = SRSchedule.GetString(ci, OneDayValue);
            OneHour = SRSchedule.GetString(ci, OneHourValue);
            OneWeek = SRSchedule.GetString(ci, OneWeekValue);
            OutOfOffice = SRSchedule.GetString(ci, OutOfOfficeValue);
            RangeOfRecurrence = SRSchedule.GetString(ci, RangeOfRecurrenceValue);
            ReadOnly = SRSchedule.GetString(ci, ReadOnlyValue);
            ReadOnlyOff = SRSchedule.GetString(ci, ReadOnlyOffValue);
            ReadOnlyOn = SRSchedule.GetString(ci, ReadOnlyOnValue);
            RecurEvery = SRSchedule.GetString(ci, RecurEveryValue);
            Recurrence = SRSchedule.GetString(ci, RecurrenceValue);
            RecurrenceDisabled = SRSchedule.GetString(ci, RecurrenceDisabledValue);
            RecurrenceEnabled = SRSchedule.GetString(ci, RecurrenceEnabledValue);
            Reminder = SRSchedule.GetString(ci, ReminderValue);
            Saturday = SRSchedule.GetString(ci, SaturdayValue);
            Save = SRSchedule.GetString(ci, SaveValue);
            Second = SRSchedule.GetString(ci, SecondValue);
            September = SRSchedule.GetString(ci, SeptemberValue);
            SevenHours = SRSchedule.GetString(ci, SevenHoursValue);
            ShowMore = SRSchedule.GetString(ci, ShowMoreValue);
            SixHours = SRSchedule.GetString(ci, SixHoursValue);
            Start = SRSchedule.GetString(ci, StartValue);
            StartDate = SRSchedule.GetString(ci, StartDateValue);
            Status = SRSchedule.GetString(ci, StatusValue);
            Subject = SRSchedule.GetString(ci, SubjectValue);
            Sunday = SRSchedule.GetString(ci, SundayValue);
            TenHours = SRSchedule.GetString(ci, TenHoursValue);
            TenMin = SRSchedule.GetString(ci, TenMinValue);
            Tentative = SRSchedule.GetString(ci, TentativeValue);
            The = SRSchedule.GetString(ci, TheValue);
            Third = SRSchedule.GetString(ci, ThirdValue);
            ThirtyMin = SRSchedule.GetString(ci, ThirtyMinValue);
            ThreeDays = SRSchedule.GetString(ci, ThreeDaysValue);
            ThreeHours = SRSchedule.GetString(ci, ThreeHoursValue);
            Thursday = SRSchedule.GetString(ci, ThursdayValue);
            Tuesday = SRSchedule.GetString(ci, TuesdayValue);
            TweleveHours = SRSchedule.GetString(ci, TweleveHoursValue);
            TwoDays = SRSchedule.GetString(ci, TwoDaysValue);
            TwoHours = SRSchedule.GetString(ci, TwoHoursValue);
            TwoWeeks = SRSchedule.GetString(ci, TwoWeeksValue);
            Wednesday = SRSchedule.GetString(ci, WednesdayValue);
            Weekson = SRSchedule.GetString(ci, WeeksonValue);
            Weekly = SRSchedule.GetString(ci, WeeklyValue);
            Where = SRSchedule.GetString(ci, WhereValue);
            Yearson = SRSchedule.GetString(ci, YearsonValue);
            Yearly = SRSchedule.GetString(ci, YearlyValue);
            ZeroMin = SRSchedule.GetString(ci, ZeroMinValue);
            EditRecurrence = SRSchedule.GetString(ci, EditRecurrenceValue);
            RecurrencePattern = SRSchedule.GetString(ci, RecurrencePatternValue);
            OK = SRSchedule.GetString(ci, OKValue);
            RemoveRecurrence = SRSchedule.GetString(ci, RemoveRecurrenceValue);
        }

        #endregion

        #region Properties

        public string AppointmentEdit { get; set; }

        public string ErrorMessage { get; set; }

        public string AllDay { get; set; }

        public string April { get; set; }

        public string August { get; set; }

        public string Busy { get; set; }

        public string Cancel { get; set; }

        public string ChooseDate { get; set; }

        public string Daily { get; set; }

        public string Day { get; set; }

        public string Days { get; set; }

        public string December { get; set; }

        public string Delete { get; set; }

        public string Add { get; set; }

        public string Edit { get; set; }

        public string Copy { get; set; }

        public string Paste { get; set; }

        public string Resize { get; set; }

        public string EighteenHours { get; set; }

        public string EightHours { get; set; }

        public string ElevenHours { get; set; }

        public string End { get; set; }

        public string EndAfter { get; set; }

        public string EndBy { get; set; }

        public string Every { get; set; }

        public string EveryDay { get; set; }

        public string EveryMonth { get; set; }

        public string EveryWeek { get; set; }

        public string EveryWeekday { get; set; }

        public string EveryWeekDays { get; set; }

        public string EveryYear { get; set; }

        public string February { get; set; }

        public string FifteenMin { get; set; }

        public string First { get; set; }

        public string FiveHours { get; set; }

        public string FiveMin { get; set; }

        public string FourDays { get; set; }

        public string FourHours { get; set; }

        public string Fourth { get; set; }

        public string Free { get; set; }

        public string Friday { get; set; }

        public string HalfDay { get; set; }

        public string HowOften { get; set; }

        public string January { get; set; }

        public string July { get; set; }

        public string June { get; set; }

        public string Location { get; set; }

        public string March { get; set; }

        public string May { get; set; }

        public string Monday { get; set; }

        public string Monthson { get; set; }

        public string Monthly { get; set; }

        public string NineHours { get; set; }

        public string NoEndDate { get; set; }

        public string None { get; set; }

        public string Notes { get; set; }

        public string November { get; set; }

        public string October { get; set; }

        public string Of { get; set; }

        public string On { get; set; }

        public string Once { get; set; }

        public string OneDay { get; set; }

        public string OneHour { get; set; }

        public string OneWeek { get; set; }

        public string OutOfOffice { get; set; }

        public string RangeOfRecurrence { get; set; }

        public string ReadOnly { get; set; }

        public string ReadOnlyOff { get; set; }

        public string ReadOnlyOn { get; set; }

        public string RecurEvery { get; set; }

        public string Recurrence { get; set; }

        public string RecurrenceDisabled { get; set; }

        public string RecurrenceEnabled { get; set; }

        public string Reminder { get; set; }

        public string Saturday { get; set; }

        public string Save { get; set; }

        public string Second { get; set; }

        public string September { get; set; }

        public string SevenHours { get; set; }

        public string ShowMore { get; set; }

        public string SixHours { get; set; }

        public string Start { get; set; }

        public string StartDate { get; set; }

        public string Status { get; set; }

        public string Subject { get; set; }

        public string Sunday { get; set; }

        public string TenHours { get; set; }

        public string TenMin { get; set; }

        public string Tentative { get; set; }

        public string The { get; set; }

        public string Third { get; set; }

        public string ThirtyMin { get; set; }

        public string ThreeDays { get; set; }

        public string ThreeHours { get; set; }

        public string Thursday { get; set; }

        public string Tuesday { get; set; }

        public string TweleveHours { get; set; }

        public string TwoDays { get; set; }

        public string TwoHours { get; set; }

        public string TwoWeeks { get; set; }

        public string Wednesday { get; set; }

        public string Weekson { get; set; }

        public string Weekly { get; set; }

        public string Where { get; set; }

        public string Yearson { get; set; }

        public string Yearly { get; set; }

        public string ZeroMin { get; set; }

        public string EditRecurrence { get; set; }

        public string RecurrencePattern { get; set; }

        public string OK { get; set; }

        public string RemoveRecurrence { get; set; }

        #endregion
    }
}
