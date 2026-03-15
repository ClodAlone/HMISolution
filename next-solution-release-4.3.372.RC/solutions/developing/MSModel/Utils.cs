using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSModel
{
    public static class Utils
    {
        public static CalendarDayType GetCalendarDayType(DayOfWeek dow)
        {
            switch (dow)
            {
                case DayOfWeek.Monday:
                    return CalendarDayType.Monday;
                    break;
                case DayOfWeek.Tuesday:
                    return CalendarDayType.Tuesday;
                    break;
                case DayOfWeek.Wednesday:
                    return CalendarDayType.Wednesday;
                    break;
                case DayOfWeek.Thursday:
                    return CalendarDayType.Thursday;
                    break;
                case DayOfWeek.Friday:
                    return CalendarDayType.Friday;
                    break;
                case DayOfWeek.Saturday:
                    return CalendarDayType.Saturday;
                    break;
                case DayOfWeek.Sunday:
                    return CalendarDayType.Sunday;
                    break;
            }
            return CalendarDayType.Any;
        }
    }
}
