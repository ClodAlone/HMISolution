using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using UFInterfaces;
using Utilities.Converters;

namespace MSModel
{
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum CalendarItemType
    {
        SingleDate,
        DateRange,
        WeekNDate
    }
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum CalendarMonthType
    {
        Any,
        January,
        February,
        March,
        April,
        May,
        June,
        July,
        August,
        September,
        October,
        November,
        December,
        Odd,
        Even
    }
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum CalendarWeekType
    {
        First,
        Second,
        Third,
        Fourth,
        Fifth,
        Last,
        Any
    }
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum CalendarDayType
    {
        Sunday,
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        Any
    }
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum MonthDayAny
    {
        NoDate,
        First,
        Last
    }

    public class LocalizedEnumConverter : ResourceEnumConverter
    {
        public LocalizedEnumConverter(Type type)
            : base(type, Properties.Resources.ResourceManager)
        {

        }
    }

    public class CalendarItem : XPObject, IDataErrorInfo
    {
        #region Ctor
        CalendarItem()
        { }
        public CalendarItem(Session session)
            : base(session)
        {

        }
        protected CalendarItem(Session session, XPClassInfo classInfo)
            : base(session, classInfo)
        {

        }

        #endregion

        #region Properties Default Values
        // List of constant default values for each property where you want handle a default value.
        //const int defaultPropertyName = -1;

        /// <summary>
        /// Adds inside this method the nullable property where you want handle a default value.
        /// </summary>
        private void EnsureDefaultValues()
        {
            // Examples of how to handle a default value
            //if (!PropertyName.HasValue)
            //    PropertyName = defaultPropertyName;
            //if (TimeSpanPropertyName == TimeSpan.Zero)
            //    TimeSpanPropertyName = TimeSpan.FromMinutes(1);
            //if (DateTimePropertyName == DateTime.MinValue)
            //    DateTimePropertyName = DateTime.UtcNow;
        }
        #endregion

        #region Properties
        private DateTime _StartDate;
        public DateTime StartDate
        {
            get
            {
                return _StartDate;
            }
            set
            {
                SetPropertyValue("StartDate", ref _StartDate, value);
                this.RaisePropertyChangedEvent("EndDate");
            }
        }
        private DateTime _EndDate;
        public DateTime EndDate
        {
            get
            {
                return _EndDate;
            }
            set
            {
                SetPropertyValue("EndDate", ref _EndDate, value);
                this.RaisePropertyChangedEvent("StartDate");
            }
        }
        private CalendarItemType _Type;
        public CalendarItemType Type
        {
            get
            {
                return _Type;
            }
            set
            {
                SetPropertyValue("Type", ref _Type, value);
                this.RaisePropertyChangedEvent("StartDate");
                this.RaisePropertyChangedEvent("EndDate");
            }
        }
        private CalendarMonthType _Month;
        public CalendarMonthType Month
        {
            get
            {
                return _Month;
            }
            set
            {
                SetPropertyValue("Month", ref _Month, value);
            }
        }
        private int _DayOfMonth;
        public int DayOfMonth
        {
            get { return _DayOfMonth; }
            set { SetPropertyValue("DayOfMonth", ref _DayOfMonth, value); }
        }
        private CalendarWeekType _WeekOfMonth;
        public CalendarWeekType WeekOfMonth
        {
            get
            {
                return _WeekOfMonth;
            }
            set
            {
                SetPropertyValue("WeekOfMonth", ref _WeekOfMonth, value);
            }
        }
        private CalendarDayType _DayOfWeek;
        public CalendarDayType DayOfWeek
        {
            get
            {
                return _DayOfWeek;
            }
            set
            {
                SetPropertyValue("DayOfWeek", ref _DayOfWeek, value);
            }
        }

        private MSScheduledAction _MSScheduledAction;
        [Association("MSScheduledAction-CalendarItems")]
        public MSScheduledAction MSScheduledAction
        {
            get
            {
                return _MSScheduledAction;
            }
            set
            {
                SetPropertyValue("MSScheduledAction", ref _MSScheduledAction, value);
            }
        }
        private MSScheduledActionRuntime _MSScheduledActionRuntime;
        [Association("MSScheduledActionRuntime-CalendarItems")]
        public MSScheduledActionRuntime MSScheduledActionRuntime
        {
            get
            {
                return _MSScheduledActionRuntime;
            }
            set
            {
                SetPropertyValue("MSScheduledActionRuntime", ref _MSScheduledActionRuntime, value);
            }
        }
        #endregion

        #region Override Methods

        public override void AfterConstruction()
        {
            base.AfterConstruction();

            EnsureDefaultValues();
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();

            EnsureDefaultValues();
        }

        #endregion

        #region IDataErrorInfo
        [Browsable(false)]
        public string Error
        {
            get
            {
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null);
                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

                return !System.ComponentModel.DataAnnotations.Validator.TryValidateObject(this, context, results)
                    ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                    : null;
            }
        }

        public string this[string propertyName]
        {
            get
            {
                String s = PerformValidation(propertyName);
                if (!String.IsNullOrEmpty(s))
                    return s;
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null)
                {
                    MemberName = propertyName
                };

                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                var propertyInfo = GetType().GetProperty(propertyName);
                if (propertyInfo != null)
                {
                    var value = propertyInfo.GetValue(this, null);

                    return !System.ComponentModel.DataAnnotations.Validator.TryValidateProperty(value, context, results)
                        ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                        : null;
                }

                return null;
            }
        }
        #endregion

        #region Methods

        protected String PerformValidation(String propertyName)
        {
            if (propertyName == "StartDate" || propertyName == "EndDate")
            {
                if ((_Type == CalendarItemType.DateRange && _StartDate >= _EndDate))
                    return Properties.Resources.StartEndDateInvalid;
                else if (_Type == CalendarItemType.WeekNDate && _StartDate.TimeOfDay >= _EndDate.TimeOfDay)
                    return Properties.Resources.StartEndTimeInvalid;
            }
            return null;
        }

        public bool CompareCalendar(CalendarItem sd)
        {
            return DayOfWeek == sd.DayOfWeek &&
                EndDate == sd.EndDate &&
                Month == sd.Month &&
                StartDate == sd.StartDate &&
                Type == sd.Type &&
                WeekOfMonth == sd.WeekOfMonth &&
                DayOfMonth == sd.DayOfMonth;
        }

        #endregion

    }

    public class ExceptionsCalendarItem : XPObject, IDataErrorInfo
    {
        #region Ctor
        public ExceptionsCalendarItem()
        { }
        public ExceptionsCalendarItem(Session session)
            : base(session)
        {

        }
        protected ExceptionsCalendarItem(Session session, XPClassInfo classInfo)
            : base(session, classInfo)
        {

        }

        #endregion

        #region Properties Default Values
        // List of constant default values for each property where you want handle a default value.
        //const int defaultPropertyName = -1;

        /// <summary>
        /// Adds inside this method the nullable property where you want handle a default value.
        /// </summary>
        private void EnsureDefaultValues()
        {
            // Examples of how to handle a default value
            //if (!PropertyName.HasValue)
            //    PropertyName = defaultPropertyName;
            //if (TimeSpanPropertyName == TimeSpan.Zero)
            //    TimeSpanPropertyName = TimeSpan.FromMinutes(1);
            //if (DateTimePropertyName == DateTime.MinValue)
            //    DateTimePropertyName = DateTime.UtcNow;
        }
        #endregion

        #region Properties
        private DateTime _StartDate;
        public DateTime StartDate
        {
            get
            {
                return _StartDate;
            }
            set
            {
                SetPropertyValue("StartDate", ref _StartDate, value);
                this.RaisePropertyChangedEvent("EndDate");
            }
        }
        private DateTime _EndDate;
        public DateTime EndDate
        {
            get
            {
                return _EndDate;
            }
            set
            {
                SetPropertyValue("EndDate", ref _EndDate, value);
                this.RaisePropertyChangedEvent("StartDate");
            }
        }
        private CalendarItemType _Type;
        public CalendarItemType Type
        {
            get
            {
                return _Type;
            }
            set
            {
                SetPropertyValue("Type", ref _Type, value);
                this.RaisePropertyChangedEvent("StartDate");
                this.RaisePropertyChangedEvent("EndDate");
            }
        }
        private CalendarMonthType _Month;
        public CalendarMonthType Month
        {
            get
            {
                return _Month;
            }
            set
            {
                SetPropertyValue("Month", ref _Month, value);
            }
        }
        private int _DayOfMonth;
        public int DayOfMonth
        {
            get { return _DayOfMonth; }
            set { SetPropertyValue("DayOfMonth", ref _DayOfMonth, value); }
        }
        private CalendarWeekType _WeekOfMonth;
        public CalendarWeekType WeekOfMonth
        {
            get
            {
                return _WeekOfMonth;
            }
            set
            {
                SetPropertyValue("WeekOfMonth", ref _WeekOfMonth, value);
            }
        }
        private CalendarDayType _DayOfWeek;
        public CalendarDayType DayOfWeek
        {
            get
            {
                return _DayOfWeek;
            }
            set
            {
                SetPropertyValue("DayOfWeek", ref _DayOfWeek, value);
            }
        }

        protected MSScheduledAction _MSScheduledAction;
        [Association("MSScheduledAction-ExceptionsCalendarItems")]
        public MSScheduledAction MSScheduledAction
        {
            get
            {
                return _MSScheduledAction;
            }
            set
            {
                SetPropertyValue("MSScheduledAction", ref _MSScheduledAction, value);
            }
        }
        private MSScheduledActionRuntime _MSScheduledActionRuntime;
        [Association("MSScheduledActionRuntime-ExceptionsCalendarItems")]
        public MSScheduledActionRuntime MSScheduledActionRuntime
        {
            get
            {
                return _MSScheduledActionRuntime;
            }
            set
            {
                SetPropertyValue("MSScheduledActionRuntime", ref _MSScheduledActionRuntime, value);
            }
        }
        #endregion

        #region Override Methods

        public override void AfterConstruction()
        {
            base.AfterConstruction();

            EnsureDefaultValues();
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();

            EnsureDefaultValues();
        }

        #endregion

        #region IDataErrorInfo
        [Browsable(false)]
        public string Error
        {
            get
            {
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null);
                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

                return !System.ComponentModel.DataAnnotations.Validator.TryValidateObject(this, context, results)
                    ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                    : null;
            }
        }

        public string this[string propertyName]
        {
            get
            {
                String s = PerformValidation(propertyName);
                if (!String.IsNullOrEmpty(s))
                    return s;
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null)
                {
                    MemberName = propertyName
                };

                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                var propertyInfo = GetType().GetProperty(propertyName);
                if (propertyInfo != null)
                {
                    var value = propertyInfo.GetValue(this, null);

                    return !System.ComponentModel.DataAnnotations.Validator.TryValidateProperty(value, context, results)
                        ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                        : null;
                }

                return null;
            }
        }
        #endregion

        #region Methods

        protected String PerformValidation(String propertyName)
        {
            if (propertyName == "StartDate" || propertyName == "EndDate")
            {
                if ((_Type == CalendarItemType.DateRange && _StartDate >= _EndDate))
                    return Properties.Resources.StartEndDateInvalid;
                else if (_Type == CalendarItemType.WeekNDate && _StartDate.TimeOfDay >= _EndDate.TimeOfDay)
                    return Properties.Resources.StartEndTimeInvalid;
            }
            return null;
        }

        public bool CompareCalendar(ExceptionsCalendarItem sd)
        {
            return DayOfWeek == sd.DayOfWeek &&
                EndDate == sd.EndDate &&
                Month == sd.Month &&
                StartDate == sd.StartDate &&
                Type == sd.Type &&
                WeekOfMonth == sd.WeekOfMonth &&
                DayOfMonth == sd.DayOfMonth;
        }

        #endregion

    }
}