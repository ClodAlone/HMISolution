using System;
using WPFUtilities;

namespace WPFPenHelpers
{
    public struct TimeRangeDates
    {
        public DateTime start;
        public DateTime end;
    }

    public static class TimeRangeHelper
    {
        const int daysPerWeek = 7;
        const int daysPerMonth = 30;
        const int daysPerYears = 365;

        public static TimeRangeDates GetSelectedTimeRangeCombo(DateTime dateTimeStart, DateTime dateTimeEnd, DateSpan dateSpan, bool useAbsoluteRanges, bool prev, out TimeRangeDates dates, bool bKeepTimeFrame = false)
        {
            dates.start = dateTimeStart;
            dates.end = dateTimeEnd;
            
            switch (dateSpan)
            {
                case DateSpan.Minute:
                    dates.start = dateTimeStart + (prev ? -TimeSpan.FromMinutes(1) : TimeSpan.FromMinutes(1));
                    dates.end = dateTimeEnd + (prev ? -TimeSpan.FromMinutes(1) : TimeSpan.FromMinutes(1));
                    break;
                case DateSpan.Hour:
                    dates.start = dateTimeStart + (prev ? -TimeSpan.FromHours(1) : TimeSpan.FromHours(1));
                    dates.end = dateTimeEnd + (prev ? -TimeSpan.FromHours(1) : TimeSpan.FromHours(1));
                    break;
                case DateSpan.Day:
                    dates.start = dateTimeStart + (prev ? -TimeSpan.FromDays(1) : TimeSpan.FromDays(1));
                    dates.end = dateTimeEnd + (prev ? -TimeSpan.FromDays(1) : TimeSpan.FromDays(1));
                    break;
                case DateSpan.Week:
                    dates.start = dateTimeStart + (prev ? -TimeSpan.FromDays(daysPerWeek) : TimeSpan.FromDays(daysPerWeek));
                    dates.end = dateTimeEnd + (prev ? -TimeSpan.FromDays(daysPerWeek) : TimeSpan.FromDays(daysPerWeek));
                    break;
                case DateSpan.Month:
                    if (bKeepTimeFrame)
                    {
                        int newSYear;
                        int newSMonth;
                        int newEYear;
                        int newEMonth;
                        if (prev) //subtract 1 month to dateTimeStart & dateTimeEnd
                        {
                            newSYear = dateTimeStart.Year;
                            newSMonth = dateTimeStart.Month == 1 ? 12 : dateTimeStart.Month - 1;
                            if (newSMonth == 12)
                                newSYear = dateTimeStart.Year - 1;
                            newEYear = dateTimeEnd.Year;
                            newEMonth = dateTimeEnd.Month == 1 ? 12 : dateTimeEnd.Month - 1;
                            if (newEMonth == 12)
                                newEYear = dateTimeEnd.Year - 1;
                        }
                        else //add 1 month to dateTimeStart & dateTimeEnd
                        {
                            newSYear = dateTimeStart.Year;
                            newSMonth = dateTimeStart.Month == 12 ? 1 : dateTimeStart.Month + 1;
                            if (newSMonth == 1)
                                newSYear = dateTimeStart.Year + 1;
                            newEYear = dateTimeEnd.Year;
                            newEMonth = dateTimeEnd.Month == 12 ? 1 : dateTimeEnd.Month + 1;
                            if (newEMonth == 1)
                                newEYear = dateTimeEnd.Year + 1;
                        }
                        try
                        {
                            dates.start = new DateTime(newSYear, newSMonth, dates.start.Day, dates.start.Hour, dates.start.Minute, dates.start.Second);
                            dates.end = new DateTime(newEYear, newEMonth, dates.end.Day, dates.end.Hour, dates.end.Minute, dates.end.Second);
                        }
                        catch (Exception)
                        {
                            dates.start = dates.end - TimeSpan.FromDays(daysPerMonth);
                            dates.end = dates.start + TimeSpan.FromDays(daysPerMonth);
                        }
                        break;
                    }
                    int newMonth;
                    var newYear = dateTimeStart.Year;
                    if (prev)
                    {
                        newMonth = dateTimeStart.Month == 1 ? 12 : dateTimeStart.Month - 1;
                        if (newMonth == 12)
                            newYear = dateTimeStart.Year - 1;
                    }
                    else
                    {
                        newMonth = dateTimeStart.Month == 12 ? 1 : dateTimeStart.Month + 1;
                        if (newMonth == 1)
                            newYear = dateTimeStart.Year + 1;
                    }
                    if (useAbsoluteRanges)
                    {
                        var endYear = newYear;
                        var endMonth = newMonth + 1;
                        if (endMonth == 13)
                        {
                            endMonth = 1;
                            endYear++;
                        }
                        dates.start = new DateTime(newYear, newMonth, 1);
                        dates.end = new DateTime(endYear, endMonth, 1, 0, 0, 0);
                    }
                    else
                    {
                        var newY = dates.start.Year;
                        var newM = dates.start.Month + (prev ? -1 : 1);
                        if (newM == 0)
                        {
                            newM = 12;
                            newY = newY - 1;
                        }
                        if (newM == 13)
                        {
                            newM = 1;
                            newY = newY + 1;
                        }
                        if (prev)
                        {
                            dates.end = dateTimeStart;
                            try
                            {
                                dates.start = new DateTime(newY, newM, dates.end.Day, dates.end.Hour, dates.end.Minute, dates.end.Second);
                            }
                            catch (Exception)
                            {
                                dates.start = dates.end - TimeSpan.FromDays(daysPerMonth);
                            }
                        }
                        else
                        {
                            dates.start = dateTimeEnd;
                            newY = dates.start.Year;
                            newM = dates.start.Month + 1;
                            if (newM == 13)
                            {
                                newM = 1;
                                newY = newY + 1;
                            }
                            try
                            {
                                dates.end = new DateTime(newY, newM, dates.start.Day, dates.start.Hour, dates.start.Minute, dates.start.Second);
                            }
                            catch (Exception)
                            {
                                dates.end = dates.start + TimeSpan.FromDays(daysPerMonth);
                            }
                        }
                    }
                    break;
                case DateSpan.Year:
                    if (bKeepTimeFrame) //add/subtract 1 year to dateTimeStart & dateTimeEnd
                    {
                        int newSYear = dateTimeStart.Year + (prev ? -1 : 1);
                        int newEYear = dateTimeEnd.Year + (prev ? -1 : 1);
                        try
                        {
                            dates.start = new DateTime(newSYear, dates.start.Month, dates.start.Day, dates.start.Hour, dates.start.Minute, dates.start.Second);
                            dates.end = new DateTime(newEYear, dates.end.Month, dates.end.Day, dates.end.Hour, dates.end.Minute, dates.end.Second);
                        }
                        catch (Exception)
                        {
                            dates.start = dates.end - TimeSpan.FromDays(daysPerYears);
                            dates.end = dates.start + TimeSpan.FromDays(daysPerYears);
                        }
                        break;
                    }
                    var nYear = dateTimeStart.Year + (prev ? -1 : 1);
                    if (useAbsoluteRanges)
                    {
                        dates.start = new DateTime(nYear, 1, 1);
                        dates.end = new DateTime(nYear + 1, 1, 1, 0, 0, 0);
                    }
                    else
                    {
                        if (prev)
                        {
                            dates.end = dateTimeStart;
                            try
                            {
                                dates.start = new DateTime(dates.end.Year - 1, dates.end.Month, dates.end.Day, dates.end.Hour, dates.end.Minute, dates.end.Second);
                            }
                            catch (Exception)
                            {
                                dates.start = dates.end - TimeSpan.FromDays(daysPerYears);
                            }
                        }
                        else
                        {
                            dates.start = dateTimeEnd;
                            try
                            {
                                dates.end = new DateTime(dates.start.Year + 1, dates.start.Month, dates.start.Day, dates.start.Hour, dates.start.Minute, dates.start.Second);
                            }
                            catch (Exception)
                            {
                                dates.end = dates.start + TimeSpan.FromDays(daysPerYears);
                            }
                        }
                    }
                    break;
            }
            return dates;
        }
    }
}
