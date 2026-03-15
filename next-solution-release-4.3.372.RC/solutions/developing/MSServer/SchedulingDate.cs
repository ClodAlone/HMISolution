using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MSModel;

namespace MSServer
{
    public class SchedulingDate
    {

        #region Properties
        private bool _Elapsed = false;
        public bool Elapsed
        {
            get { return _Elapsed; }
            set { _Elapsed = value; }
        }
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
    }
}
