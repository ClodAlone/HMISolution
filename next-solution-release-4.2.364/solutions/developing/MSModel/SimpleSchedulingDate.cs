using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSModel
{
    public class SimpleSchedulingDate
    {

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
                _StartDate = value;
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
                _EndDate = value;
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
                _Type = value;
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
                _Month = value;
            }
        }
        private int _DayOfMonth;
        public int DayOfMonth
        {
            get { return _DayOfMonth; }
            set { _DayOfMonth = value; }
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
                _WeekOfMonth = value;
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
                _DayOfWeek = value;
            }
        }
        #endregion

        #region Methods
        public bool CompareWeekly(SimpleSchedulingDate sd)
        {
            return DayOfWeek == sd.DayOfWeek &&
                EndDate == sd.EndDate &&
                StartDate == sd.StartDate;
        }
        public bool CompareWeekly(WeeklyCalendarItem sd)
        {
            return EndDate == sd.EndDate &&
                StartDate == sd.StartDate;
        }
        public bool CompareCalendar(SimpleSchedulingDate sd)
        {
            return DayOfWeek == sd.DayOfWeek &&
                EndDate == sd.EndDate &&
                Month == sd.Month &&
                StartDate == sd.StartDate &&
                Type == sd.Type &&
                WeekOfMonth == sd.WeekOfMonth &&
                DayOfMonth == sd.DayOfMonth;
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
