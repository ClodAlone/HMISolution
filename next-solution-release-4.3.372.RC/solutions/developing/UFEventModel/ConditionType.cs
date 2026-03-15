using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFEventModel
{
    public enum ConditionType
    {
        Equals,
        GreaterThan,
        LessThan,
        GreaterThanOrEqual,
        LessThanOrEqual,
        NotEqual,
        OnChange
    }
    public enum EventType
    {
        Tag,
        Schedule
    }
    public enum ScheduleType
    {
        everyMinute,
        everyHour,
        everyDay,
        everySunday,
        everyMonday,
        everyTuesday,
        everyWednesday,
        everyThursday,
        everyFriday,
        everySaturday,
        everyMonth,
        everyYear
    }
}
