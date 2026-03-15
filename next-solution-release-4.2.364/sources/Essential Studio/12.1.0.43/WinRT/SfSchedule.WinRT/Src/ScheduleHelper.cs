#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Globalization;
#if WINRT
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using System.Collections.ObjectModel;
#else
using System.IO;
using System.Text;
using System.Collections.ObjectModel;
#if SILVERLIGHT
using System.Windows.Controls;
#else
using Microsoft.Win32;
#endif
#endif
namespace Syncfusion.UI.Xaml.Schedule
{
    /// <summary>
    /// Represents a helper for handling recurrence appointments.
    /// </summary>
    public class ScheduleHelper
    {
        #region Internal Fields

        internal static Dictionary<string, List<string>> ICalTimeZones = AddTimeZones();
        internal static List<string> valueList = new List<string>();
        internal static Dictionary<string, RecurrenceProperties> RecPropertiesDict = new Dictionary<string, RecurrenceProperties>();

        #endregion

        #region Methods

        public static IEnumerable<DateTime> GetRecurrenceDateTimeCollection(string RRule, DateTime RecStartDate)
        {
            var RecDateCollection = new ObservableRangeCollection<DateTime>();
            DateTime startDate = RecStartDate;
            var ruleSeperator = new[] { '=', ';', ',' };
            var weeklySeperator = new[] { ';' };
            string[] ruleArray = RRule.Split(ruleSeperator);
            string[] weeklyRule = RRule.Split(weeklySeperator);
            if (ruleArray.Length != 0 && RRule != "")
            {
                DateTime addDate = startDate;
                int recCount = int.Parse(ruleArray[3]);
                #region DAILY
                if (ruleArray[1] == "DAILY")
                {

                    if ((ruleArray.Length > 4 && ruleArray[4] == "INTERVAL") || ruleArray.Length == 4)
                    {
                        int DyDayGap = ruleArray.Length == 4 ? 1 : int.Parse(ruleArray[5]);
                        for (int i = 0; i < recCount; i++)
                        {
                            RecDateCollection.Add(addDate.Date);
                            addDate = addDate.AddDays(DyDayGap);
                        }
                    }
                    else if (ruleArray.Length > 4 && ruleArray[4] == "BYDAY")
                    {
                        while (RecDateCollection.Count < recCount)
                        {
                            if (addDate.DayOfWeek != DayOfWeek.Sunday && addDate.DayOfWeek != DayOfWeek.Saturday)
                            {

                                RecDateCollection.Add(addDate.Date);
                            }
                            addDate = addDate.AddDays(1);
                        }
                    }
                }
                #endregion

                #region WEEKLY
                else if (ruleArray[1] == "WEEKLY")
                {
                    int WyWeekGap = ruleArray.Length > 4 && ruleArray[4] == "INTERVAL" ? int.Parse(ruleArray[5]) : 1;
                    int position = ruleArray.Length > 4 && ruleArray[4] == "INTERVAL" ? 3 : 2;
                    bool isweeklyselected = weeklyRule[position].Length > 6;
                    while (RecDateCollection.Count < recCount && isweeklyselected)
                    {
                        switch (addDate.DayOfWeek)
                        {
                            case DayOfWeek.Sunday:
                                {
                                    if (weeklyRule[position].Contains("SU") && RecDateCollection.Count < recCount)
                                    {
                                        RecDateCollection.Add(addDate.Date);
                                    }
                                    break;
                                }
                            case DayOfWeek.Monday:
                                {
                                    if (weeklyRule[position].Contains("MO") && RecDateCollection.Count < recCount)
                                    {
                                        RecDateCollection.Add(addDate.Date);
                                    }
                                    break;
                                }
                            case DayOfWeek.Tuesday:
                                {
                                    if (weeklyRule[position].Contains("TU") && RecDateCollection.Count < recCount)
                                    {
                                        RecDateCollection.Add(addDate.Date);
                                    }
                                    break;
                                }
                            case DayOfWeek.Wednesday:
                                {
                                    if (weeklyRule[position].Contains("WE") && RecDateCollection.Count < recCount)
                                    {
                                        RecDateCollection.Add(addDate.Date);
                                    }
                                    break;
                                }
                            case DayOfWeek.Thursday:
                                {
                                    if (weeklyRule[position].Contains("TH") && RecDateCollection.Count < recCount)
                                    {
                                        RecDateCollection.Add(addDate.Date);
                                    }
                                    break;
                                }
                            case DayOfWeek.Friday:
                                {
                                    if (weeklyRule[position].Contains("FR") && RecDateCollection.Count < recCount)
                                    {
                                        RecDateCollection.Add(addDate.Date);
                                    }
                                    break;
                                }
                            case DayOfWeek.Saturday:
                                {
                                    if (weeklyRule[position].Contains("SA") && RecDateCollection.Count < recCount)
                                    {
                                        RecDateCollection.Add(addDate.Date);
                                    }
                                    break;
                                }
                        }
                        addDate = addDate.DayOfWeek == DayOfWeek.Saturday ? addDate.AddDays(((WyWeekGap - 1) * 7) + 1) : addDate.AddDays(1);
                    }
                }
                #endregion

                #region MONTHLY
                else if (ruleArray[1] == "MONTHLY")
                {
                    int MyMonthGap = ruleArray.Length > 4 && ruleArray[4] == "INTERVAL" ? int.Parse(ruleArray[5]) : 1;
                    int position = ruleArray.Length > 4 && ruleArray[4] == "INTERVAL" ? 6 : 4;
                    if (ruleArray[position] == "BYMONTHDAY")
                    {
                        int monthDate = int.Parse(ruleArray[position + 1]);
                        if (monthDate < 30)
                        {
                            int currDate = int.Parse(startDate.Day.ToString());
                            var temp = new DateTime(addDate.Year, addDate.Month, monthDate);
                            addDate = monthDate < currDate ? temp.AddMonths(1) : temp;
                            for (int i = 0; i < recCount; i++)
                            {
                                if (addDate.Month == 2 && monthDate > 28)
                                {
                                    addDate = new DateTime(addDate.Year, addDate.Month, DateTime.DaysInMonth(addDate.Year, 2));
                                    RecDateCollection.Add(addDate.Date);
                                    addDate = addDate.AddMonths(MyMonthGap);
                                    addDate = new DateTime(addDate.Year, addDate.Month, monthDate);
                                }
                                else
                                {
                                    RecDateCollection.Add(addDate.Date);
                                    addDate = addDate.AddMonths(MyMonthGap);
                                }
                            }
                        }
                        else
                        {
                            addDate = new DateTime(addDate.Year, addDate.Month, DateTime.DaysInMonth(addDate.Year, addDate.Month));
                            for (int i = 0; i < recCount; i++)
                            {
                                RecDateCollection.Add(addDate.Date);
                                addDate = addDate.AddMonths(MyMonthGap);
                                addDate = new DateTime(addDate.Year, addDate.Month, DateTime.DaysInMonth(addDate.Year, addDate.Month));
                            }
                        }

                    }
                    else if (ruleArray[position] == "BYDAY")
                    {
                        while (RecDateCollection.Count < recCount)
                        {
                            var monthStart = new DateTime(addDate.Year, addDate.Month, 1);
                            DateTime weekStartDate = monthStart.AddDays(-(int)(monthStart.DayOfWeek));
                            var monthStartWeekday = (int)(monthStart.DayOfWeek);
                            int nthweekDay = GetWeekDay(ruleArray[position + 1]) - 1;
                            int nthWeek;
                            if (monthStartWeekday <= nthweekDay)
                            {
                                nthWeek = int.Parse(ruleArray[position + 3]) - 1;
                            }
                            else
                            {
                                nthWeek = int.Parse(ruleArray[position + 3]);
                            }
                            addDate = weekStartDate.AddDays((nthWeek) * 7);
                            addDate = addDate.AddDays(nthweekDay);
                            if (addDate.CompareTo(startDate) < 0)
                            {
                                addDate = addDate.AddMonths(1);
                                continue;
                            }

                            RecDateCollection.Add(addDate.Date);
                            addDate = addDate.AddMonths(MyMonthGap);
                        }
                    }
                }
                #endregion

                #region YEARLY
                else if (ruleArray[1] == "YEARLY")
                {
                    int YyYearGap = ruleArray.Length > 4 && ruleArray[4] == "INTERVAL" ? int.Parse(ruleArray[5]) : 1;
                    int position = ruleArray.Length > 4 && ruleArray[4] == "INTERVAL" ? 6 : 4;
                    if (ruleArray[position] == "BYMONTHDAY")
                    {
                        int monthIndex = int.Parse(ruleArray[position + 3]);
                        int dayIndex = int.Parse(ruleArray[position + 1]);
                        if (monthIndex > 0 && monthIndex <= 12)
                        {
                            int bound = DateTime.DaysInMonth(addDate.Year, monthIndex);
                            if (bound >= dayIndex)
                            {
                                var specificDate = new DateTime(addDate.Year, monthIndex, dayIndex);
                                if (specificDate.Date < addDate.Date)
                                {
                                    addDate = specificDate;
                                    addDate = addDate.AddYears(1);

                                }
                                else
                                {
                                    addDate = specificDate;
                                }

                                for (int i = 0; i < recCount; i++)
                                {


                                    RecDateCollection.Add(addDate.Date);
                                    addDate = addDate.AddYears(YyYearGap);
                                }
                            }
                        }
                    }
                    else if (ruleArray[position] == "BYDAY")
                    {
                        int monthIndex = int.Parse(ruleArray[position + 3]);
                        while (RecDateCollection.Count < recCount)
                        {
                            var monthStart = new DateTime(addDate.Year, monthIndex, 1);
                            DateTime weekStartDate = monthStart.AddDays(-(int)(monthStart.DayOfWeek));
                            var monthStartWeekday = (int)(monthStart.DayOfWeek);
                            int nthweekDay = GetWeekDay(ruleArray[position + 1]) - 1;
                            int nthWeek;
                            if (monthStartWeekday <= nthweekDay)
                            {
                                nthWeek = int.Parse(ruleArray[position + 5]) - 1;
                            }
                            else
                            {
                                nthWeek = int.Parse(ruleArray[position + 5]);
                            }
                            addDate = weekStartDate.AddDays((nthWeek) * 7);
                            addDate = addDate.AddDays(nthweekDay);
                            if (addDate.CompareTo(startDate) < 0)
                            {
                                addDate = addDate.AddYears(1);
                                continue;
                            }

                            RecDateCollection.Add(addDate.Date);
                            addDate = addDate.AddYears(YyYearGap);

                        }
                    }
                }
                #endregion
            }

            return RecDateCollection;

        }

        public static string RRuleGenerator(RecurrenceProperties RecProp, DateTime AppStartTime, DateTime AppEndTime)
        {
            DateTime startDate = (!RecProp.hasRangeStartDate) ? AppStartTime.Date : RecProp.RangeStartDate;
            DateTime endDate = RecProp.RangeEndDate;
            DateTime addDate;
            TimeSpan diffTimeSpan = AppEndTime - AppStartTime;
            TimeSpan tempTimeSpan;
            int recCount = 0;
            int DyDayGap;
            int WyWeekGap;
            int MyMonthGap;
            int YyYearGap;
            string RRule = string.Empty;
            var prevDate = new DateTime();
            bool isValidRecurrence = true;
            if (RecProp.IsRangeRecurrenceCount)
                recCount = RecProp.RangeRecurrenceCount;
            // Check recurrence compatibility
            if (recCount > 0 && RecProp.IsRangeRecurrenceCount)
            {

                addDate = startDate;

                #region Daily
                if (RecProp.RecurrenceType == RecurrenceType.Daily)
                {
                    RRule = "FREQ=DAILY;COUNT=" + recCount;
                    if (RecProp.IsDailyEveryNDays)
                    {
                        DyDayGap = RecProp.DailyNDays;
                        if (diffTimeSpan.TotalHours >= DyDayGap * 24)
                        {
                            isValidRecurrence = false;
                        }
                        if (DyDayGap > 1)
                        {
                            //FREQ=DAILY;COUNT=5;INTERVAL=2
                            RRule = RRule + ";INTERVAL=" + DyDayGap;
                        }
                    }
                    else
                    {
                        if (diffTimeSpan.TotalHours > 24)
                        {
                            isValidRecurrence = false;
                        }
                        //FREQ=WEEKLY;COUNT=5;BYDAY=MO,TU,WE,TH,FR
                        RRule = RRule + ";BYDAY=MO,TU,WE,TH,FR";
                    }
                }
                #endregion

                #region Weekly
                else if (RecProp.RecurrenceType == RecurrenceType.Weekly)
                {
                    //FREQ=WEEKLY;COUNT=6;INTERVAL=2;BYDAY=SU,MO,WE,FR;WKST=SU
                    RRule = "FREQ=WEEKLY;COUNT=" + recCount;
                    string byDay = "";
                    int su = 0, mo = 0, tu = 0, we = 0, th = 0, fr = 0, sa = 0;
                    WyWeekGap = RecProp.WeeklyEveryNWeeks;
                    if (WyWeekGap > 1)
                    {
                        //FREQ=DAILY;COUNT=5;INTERVAL=2
                        RRule = RRule + ";INTERVAL=" + WyWeekGap;
                    }
                    RRule = RRule + ";BYDAY=";
                    int count = 0;
                    bool isweeklyday_selected = RecProp.IsWeeklyFriday || RecProp.IsWeeklyMonday || RecProp.IsWeeklySaturday || RecProp.IsWeeklySunday || RecProp.IsWeeklyThursday || RecProp.IsWeeklyTuesday || RecProp.IsWeeklyWednesday;
                    while (count < recCount && isValidRecurrence && isweeklyday_selected)
                    {
                        switch (addDate.DayOfWeek)
                        {
                            case DayOfWeek.Sunday:
                                {
                                    if (RecProp.IsWeeklySunday && count < recCount)
                                    {
                                        if (count != 0)
                                        {
                                            tempTimeSpan = addDate - prevDate;
                                            if (tempTimeSpan <= diffTimeSpan)
                                            {
                                                isValidRecurrence = false;
                                            }
                                            else
                                            {
                                                prevDate = addDate;
                                                if (su == 1)
                                                {
                                                    count = recCount;
                                                    break;
                                                }
                                                byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",SU," : byDay + "SU,";
                                                su++;
                                            }
                                        }
                                        else
                                        {
                                            prevDate = addDate;
                                            count++;
                                            byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",SU," : byDay + "SU,";
                                            su++;
                                        }
                                    }
                                    break;
                                }
                            case DayOfWeek.Monday:
                                {
                                    if (RecProp.IsWeeklyMonday && count < recCount)
                                    {
                                        if (count != 0)
                                        {
                                            tempTimeSpan = addDate - prevDate;
                                            if (tempTimeSpan <= diffTimeSpan)
                                            {
                                                isValidRecurrence = false;
                                            }
                                            else
                                            {
                                                prevDate = addDate;
                                                if (mo == 1)
                                                {
                                                    count = recCount;
                                                    break;
                                                }
                                                byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",MO," : byDay + "MO,";
                                                mo++;
                                            }
                                        }
                                        else
                                        {
                                            prevDate = addDate;
                                            count++;
                                            byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",MO," : byDay + "MO,";
                                            mo++;
                                        }
                                    }
                                    break;
                                }
                            case DayOfWeek.Tuesday:
                                {
                                    if (RecProp.IsWeeklyTuesday && count < recCount)
                                    {
                                        if (count != 0)
                                        {
                                            tempTimeSpan = addDate - prevDate;
                                            if (tempTimeSpan <= diffTimeSpan)
                                            {
                                                isValidRecurrence = false;
                                            }
                                            else
                                            {
                                                prevDate = addDate;
                                                if (tu == 1)
                                                {
                                                    count = recCount;
                                                    break;
                                                }
                                                byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",TU," : byDay + "TU,";
                                                tu++;
                                            }
                                        }
                                        else
                                        {
                                            prevDate = addDate;
                                            count++;
                                            byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",TU," : byDay + "TU,";
                                            tu++;
                                        }
                                    }
                                    break;
                                }
                            case DayOfWeek.Wednesday:
                                {
                                    if (RecProp.IsWeeklyWednesday && count < recCount)
                                    {
                                        if (count != 0)
                                        {
                                            tempTimeSpan = addDate - prevDate;
                                            if (tempTimeSpan <= diffTimeSpan)
                                            {
                                                isValidRecurrence = false;
                                            }
                                            else
                                            {
                                                prevDate = addDate;
                                                if (we == 1)
                                                {
                                                    count = recCount;
                                                    break;
                                                }
                                                byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",WE," : byDay + "WE,";
                                                we++;
                                            }
                                        }
                                        else
                                        {
                                            prevDate = addDate;
                                            count++;
                                            byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",WE," : byDay + "WE,";
                                            we++;
                                        }
                                    }
                                    break;
                                }
                            case DayOfWeek.Thursday:
                                {
                                    if (RecProp.IsWeeklyThursday && count < recCount)
                                    {
                                        if (count != 0)
                                        {
                                            tempTimeSpan = addDate - prevDate;
                                            if (tempTimeSpan <= diffTimeSpan)
                                            {
                                                isValidRecurrence = false;
                                            }
                                            else
                                            {
                                                prevDate = addDate;
                                                if (th == 1)
                                                {
                                                    count = recCount;
                                                    break;
                                                }
                                                byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",TH," : byDay + "TH,";
                                                th++;
                                            }
                                        }
                                        else
                                        {
                                            prevDate = addDate;
                                            count++;
                                            byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",TH," : byDay + "TH,";
                                            th++;
                                        }
                                    }
                                    break;
                                }
                            case DayOfWeek.Friday:
                                {
                                    if (RecProp.IsWeeklyFriday && count < recCount)
                                    {
                                        if (count != 0)
                                        {
                                            tempTimeSpan = addDate - prevDate;
                                            if (tempTimeSpan <= diffTimeSpan)
                                            {
                                                isValidRecurrence = false;
                                            }
                                            else
                                            {
                                                prevDate = addDate;
                                                if (fr == 1)
                                                {
                                                    count = recCount;
                                                    break;
                                                }
                                                byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",FR," : byDay + "FR,";
                                                fr++;
                                            }
                                        }
                                        else
                                        {
                                            prevDate = addDate;
                                            count++;
                                            byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",FR," : byDay + "FR,";
                                            fr++;
                                        }
                                    }
                                    break;
                                }
                            case DayOfWeek.Saturday:
                                {
                                    if (RecProp.IsWeeklySaturday && count < recCount)
                                    {
                                        if (count != 0)
                                        {
                                            tempTimeSpan = addDate - prevDate;
                                            if (tempTimeSpan <= diffTimeSpan)
                                            {
                                                isValidRecurrence = false;
                                            }
                                            else
                                            {
                                                prevDate = addDate;
                                                if (sa == 1)
                                                {
                                                    count = recCount;
                                                    break;
                                                }
                                                byDay = byDay + "SA";
                                                sa++;
                                            }
                                        }
                                        else
                                        {
                                            prevDate = addDate;
                                            count++;
                                            byDay = byDay + "SA";
                                            sa++;
                                        }
                                    }
                                    break;
                                }
                        }
                        addDate = addDate.DayOfWeek == DayOfWeek.Saturday ? addDate.AddDays(((WyWeekGap - 1) * 7) + 1) : addDate.AddDays(1);
                    }
                    RRule = RRule + byDay;
                }
                #endregion

                #region Monthly
                else if (RecProp.RecurrenceType == RecurrenceType.Monthly)
                {

                    RRule = "FREQ=MONTHLY;COUNT=" + recCount;
                    int count = 0;
                    MyMonthGap = RecProp.MonthlyEveryNMonths;
                    if (MyMonthGap > 1)
                    {
                        RRule = RRule + ";INTERVAL=" + MyMonthGap;
                    }


                    if (RecProp.IsMonthlySpecific)
                    {
                        //FREQ=MONTHLY;COUNT=6;INTERVAL=2;BYMONTHDAY=3
                        RRule = RRule + ";BYMONTHDAY=" + (RecProp.MonthlySpecificMonthDay);
                        int monthDate = RecProp.MonthlySpecificMonthDay;
                        int currDate = int.Parse(startDate.Day.ToString());
                        int diffDate = monthDate - currDate;
                        addDate = addDate.AddDays(diffDate);
                        if (monthDate < currDate)
                        {
                            addDate = addDate.AddMonths(1);
                        }
                        for (int i = 0; i < recCount; i++)
                        {

                            //RecDateCollection.Add(addDate.Date);
                            //addDate = addDate.AddMonths(MyMonthGap);

                            if (count != 0)
                            {
                                tempTimeSpan = addDate - prevDate;
                                if (tempTimeSpan >= diffTimeSpan)
                                {
                                    prevDate = addDate;
                                    count++;
                                }
                                else
                                {
                                    isValidRecurrence = false;
                                    break;
                                }
                            }
                            else
                            {
                                prevDate = addDate;
                                count++;
                            }
                            //RecDateCollection.Add(addDate.Date);
                            addDate = addDate.AddMonths(MyMonthGap);
                        }
                    }

                    else
                    {
                        //FREQ=MONTHLY;COUNT=6;INTERVAL=2;BYDAY=WE;BYSETPOS=1
                        RRule = RRule + ";BYDAY=" + (CultureInfo.CurrentCulture.DateTimeFormat.ShortestDayNames[RecProp.MonthlyWeekDay - 1]).ToUpper() + ";BYSETPOS=" + RecProp.MonthlyNthWeek;

                        //MyMonthGap = RecProp.MonthlyEveryNMonths;
                        while (count < recCount)
                        {
                            var monthStart = new DateTime(addDate.Year, addDate.Month, 1);
                            var weekStartDate = monthStart.AddDays(-(int)(monthStart.DayOfWeek));
                            var monthStartWeekday = (int)(monthStart.DayOfWeek);
                            int nthweekDay = RecProp.MonthlyWeekDay;
                            int nthWeek;
                            if (monthStartWeekday <= nthweekDay)
                            {
                                nthWeek = RecProp.MonthlyNthWeek - 1;
                            }
                            else
                            {
                                nthWeek = RecProp.MonthlyNthWeek;
                            }
                            addDate = weekStartDate.AddDays((nthWeek) * 7);
                            addDate = addDate.AddDays(nthweekDay);
                            if (addDate.CompareTo(startDate) < 0)
                            {
                                addDate = addDate.AddMonths(1);
                                continue;
                            }

                            if (count != 0)
                            {
                                tempTimeSpan = addDate - prevDate;
                                if (tempTimeSpan >= diffTimeSpan)
                                {
                                    prevDate = addDate;
                                    count++;
                                }
                                else
                                {
                                    isValidRecurrence = false;
                                    break;
                                }
                            }
                            else
                            {
                                prevDate = addDate;
                                count++;
                            }
                            addDate = addDate.AddMonths(MyMonthGap);
                        }
                    }

                }
                #endregion

                #region Yearly

                else if (RecProp.RecurrenceType == RecurrenceType.Yearly)
                {

                    RRule = "FREQ=YEARLY;COUNT=" + recCount;
                    int count = 0;
                    YyYearGap = RecProp.YearlyEveryNYears;
                    if (YyYearGap > 1)
                    {
                        RRule = RRule + ";INTERVAL=" + YyYearGap;
                    }

                    if (RecProp.IsYearlySpecific)
                    {
                        //FREQ=YEARLY;COUNT=6;INTERVAL=2;BYMONTHDAY=3;BYMONTH=4
                        RRule = RRule + ";BYMONTHDAY=" + (RecProp.YearlySpecificMonthDay) + ";BYMONTH=" + (RecProp.YearlySpecificMonth);
                        int monthIndex = RecProp.YearlySpecificMonth;
                        int dayIndex = RecProp.YearlySpecificMonthDay;
                        if (monthIndex > 0 && monthIndex <= 12)
                        {
                            int bound = DateTime.DaysInMonth(addDate.Year, monthIndex);
                            if (bound >= dayIndex)
                            {
                                var specificDate = new DateTime(addDate.Year, monthIndex, dayIndex);
                                if (specificDate.Date < addDate.Date)
                                {
                                    addDate = specificDate;
                                    addDate = addDate.AddYears(1);

                                }
                                else
                                {
                                    addDate = specificDate;
                                }

                                for (int i = 0; i < recCount; i++)
                                {
                                    if (count != 0)
                                    {
                                        tempTimeSpan = addDate - prevDate;
                                        if (tempTimeSpan >= diffTimeSpan)
                                        {
                                            prevDate = addDate;
                                            count++;
                                        }
                                        else
                                        {
                                            isValidRecurrence = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        prevDate = addDate;
                                        count++;
                                    }
                                    addDate = addDate.AddYears(YyYearGap);
                                }
                            }
                        }
                    }

                    else
                    {
                        //FREQ=YEARLY;COUNT=6;INTERVAL=2;BYDAY=WE;BYMONTH=4;BYSETPOS=1
                        RRule = RRule + ";BYDAY=" + (CultureInfo.CurrentCulture.DateTimeFormat.ShortestDayNames[RecProp.YearlyWeekDay - 1]).ToUpper() + ";BYMONTH=" + (RecProp.YearlyGenericMonth) + ";BYSETPOS=" + RecProp.YearlyNthWeek;
                        int monthIndex = RecProp.YearlyGenericMonth;
                        while (count < recCount)
                        {
                            var monthStart = new DateTime(addDate.Year, monthIndex, 1);
                            DateTime weekStartDate = monthStart.AddDays(-(int)(monthStart.DayOfWeek));
                            var monthStartWeekday = (int)(monthStart.DayOfWeek);
                            int nthweekDay = RecProp.YearlyWeekDay;
                            int nthWeek;
                            if (monthStartWeekday <= nthweekDay)
                            {
                                nthWeek = RecProp.YearlyNthWeek - 1;
                            }
                            else
                            {
                                nthWeek = RecProp.YearlyNthWeek;
                            }
                            addDate = weekStartDate.AddDays((nthWeek) * 7);
                            addDate = addDate.AddDays(nthweekDay);
                            if (addDate.CompareTo(startDate) < 0)
                            {
                                addDate = addDate.AddYears(1);
                                continue;
                            }

                            if (count != 0)
                            {
                                tempTimeSpan = addDate - prevDate;
                                if (tempTimeSpan >= diffTimeSpan)
                                {
                                    prevDate = addDate;
                                    count++;
                                }
                                else
                                {
                                    isValidRecurrence = false;
                                    break;
                                }
                            }
                            else
                            {
                                prevDate = addDate;
                                count++;
                            }
                            addDate = addDate.AddYears(YyYearGap);

                        }
                    }
                }
                #endregion

            }
            else if (startDate <= endDate && RecProp.IsRangeEndDate)
            {

                addDate = startDate;

                #region Daily

                if (RecProp.RecurrenceType == RecurrenceType.Daily)
                {
                    RRule = "FREQ=DAILY";
                    if (RecProp.IsDailyEveryNDays)
                    {
                        DyDayGap = RecProp.DailyNDays;
                        if (diffTimeSpan.TotalHours >= DyDayGap * 24)
                        {
                            isValidRecurrence = false;
                        }
                        else
                        {
                            while (addDate.Date <= endDate.Date)
                            {
                                addDate = addDate.AddDays(DyDayGap);
                                recCount++;
                            }
                        }
                        RRule = RRule + ";COUNT=" + recCount;
                        if (DyDayGap > 1)
                        {
                            //FREQ=DAILY;COUNT=5;INTERVAL=2
                            RRule = RRule + ";INTERVAL=" + DyDayGap;
                        }
                    }

                    else
                    {
                        if (diffTimeSpan.TotalHours > 24)
                        {
                            isValidRecurrence = false;
                        }
                        else
                        {
                            while (addDate.Date <= endDate.Date)
                            {
                                if (addDate.DayOfWeek != DayOfWeek.Sunday && addDate.DayOfWeek != DayOfWeek.Saturday)
                                {
                                    recCount++;
                                }
                                addDate = addDate.AddDays(1);
                            }
                        }
                        //FREQ=WEEKLY;COUNT=8;BYDAY=MO,TU,WE,TH,FR
                        RRule = RRule + ";COUNT=" + recCount + ";BYDAY=MO,TU,WE,TH,FR";
                    }
                }
                #endregion

                #region Weekly


                else if (RecProp.RecurrenceType == RecurrenceType.Weekly)
                {
                    //FREQ=WEEKLY;COUNT=6;INTERVAL=2;BYDAY=SU,MO,WE,FR;WKST=SU
                    RRule = "FREQ=WEEKLY";
                    WyWeekGap = RecProp.WeeklyEveryNWeeks;
                    string byDay = "";
                    int su = 0, mo = 0, tu = 0, we = 0, th = 0, fr = 0, sa = 0;
                    addDate = startDate;
                    while (addDate.Date <= endDate.Date)
                    {
                        switch (addDate.DayOfWeek)
                        {
                            case DayOfWeek.Sunday:
                                {
                                    if (RecProp.IsWeeklySunday && addDate.Date <= endDate.Date)
                                    {
                                        if (recCount != 0)
                                        {
                                            tempTimeSpan = addDate - prevDate;
                                            if (tempTimeSpan <= diffTimeSpan)
                                            {
                                                isValidRecurrence = false;
                                            }
                                            else
                                            {
                                                prevDate = addDate;
                                                recCount++;
                                                if (su == 0)
                                                {
                                                    byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",SU," : byDay + "SU,";
                                                    su++;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            prevDate = addDate;
                                            recCount++;
                                            if (su == 0)
                                            {
                                                byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",SU," : byDay + "SU,";
                                                su++;
                                            }

                                        }
                                    }
                                    break;
                                }
                            case DayOfWeek.Monday:
                                {
                                    if (RecProp.IsWeeklyMonday && addDate.Date <= endDate.Date)
                                    {
                                        if (recCount != 0)
                                        {
                                            tempTimeSpan = addDate - prevDate;
                                            if (tempTimeSpan <= diffTimeSpan)
                                            {
                                                isValidRecurrence = false;
                                            }
                                            else
                                            {
                                                prevDate = addDate;
                                                recCount++;
                                                if (mo == 0)
                                                {
                                                    byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",MO," : byDay + "MO,";
                                                    mo++;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            prevDate = addDate;
                                            recCount++;
                                            if (mo == 0)
                                            {
                                                byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",MO," : byDay + "MO,";
                                                mo++;
                                            }
                                        }
                                    }
                                    break;
                                }
                            case DayOfWeek.Tuesday:
                                {
                                    if (RecProp.IsWeeklyTuesday && addDate.Date <= endDate.Date)
                                    {
                                        if (recCount != 0)
                                        {
                                            tempTimeSpan = addDate - prevDate;
                                            if (tempTimeSpan <= diffTimeSpan)
                                            {
                                                isValidRecurrence = false;
                                            }
                                            else
                                            {
                                                prevDate = addDate;
                                                recCount++;
                                                if (tu == 0)
                                                {
                                                    byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",TU," : byDay + "TU,";
                                                    tu++;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            prevDate = addDate;
                                            recCount++;
                                            if (tu == 0)
                                            {
                                                byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",TU," : byDay + "TU,";
                                                tu++;
                                            }
                                        }
                                    }
                                    break;
                                }
                            case DayOfWeek.Wednesday:
                                {
                                    if (RecProp.IsWeeklyWednesday && addDate.Date <= endDate.Date)
                                    {
                                        if (recCount != 0)
                                        {
                                            tempTimeSpan = addDate - prevDate;
                                            if (tempTimeSpan <= diffTimeSpan)
                                            {
                                                isValidRecurrence = false;
                                            }
                                            else
                                            {
                                                prevDate = addDate;
                                                recCount++;
                                                if (we == 0)
                                                {
                                                    byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",WE," : byDay + "WE,";
                                                    we++;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            prevDate = addDate;
                                            recCount++;
                                            if (we == 0)
                                            {
                                                byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",WE," : byDay + "WE,";
                                                we++;
                                            }
                                        }
                                    }
                                    break;
                                }
                            case DayOfWeek.Thursday:
                                {
                                    if (RecProp.IsWeeklyThursday && addDate.Date <= endDate.Date)
                                    {
                                        if (recCount != 0)
                                        {
                                            tempTimeSpan = addDate - prevDate;
                                            if (tempTimeSpan <= diffTimeSpan)
                                            {
                                                isValidRecurrence = false;
                                            }
                                            else
                                            {
                                                prevDate = addDate;
                                                recCount++;
                                                if (th == 0)
                                                {
                                                    byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",TH," : byDay + "TH,";
                                                    th++;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            prevDate = addDate;
                                            recCount++;
                                            if (th == 0)
                                            {
                                                byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",TH," : byDay + "TH,";
                                                th++;
                                            }
                                        }
                                    }
                                    break;
                                }
                            case DayOfWeek.Friday:
                                {
                                    if (RecProp.IsWeeklyFriday && addDate.Date <= endDate.Date)
                                    {
                                        if (recCount != 0)
                                        {
                                            tempTimeSpan = addDate - prevDate;
                                            if (tempTimeSpan <= diffTimeSpan)
                                            {
                                                isValidRecurrence = false;
                                            }
                                            else
                                            {
                                                prevDate = addDate;
                                                recCount++;
                                                if (fr == 0)
                                                {
                                                    byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",FR," : byDay + "FR,";
                                                    fr++;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            prevDate = addDate;
                                            recCount++;
                                            if (fr == 0)
                                            {
                                                byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",FR," : byDay + "FR,";
                                                fr++;
                                            }
                                        }
                                    }
                                    break;
                                }
                            case DayOfWeek.Saturday:
                                {
                                    if (RecProp.IsWeeklySaturday && addDate.Date <= endDate.Date)
                                    {
                                        if (recCount != 0)
                                        {
                                            tempTimeSpan = addDate - prevDate;
                                            if (tempTimeSpan <= diffTimeSpan)
                                            {
                                                isValidRecurrence = false;
                                            }
                                            else
                                            {
                                                prevDate = addDate;
                                                recCount++;
                                                if (sa == 0)
                                                {
                                                    byDay = byDay + "SA";
                                                    sa++;
                                                }

                                            }
                                        }
                                        else
                                        {
                                            prevDate = addDate;
                                            recCount++;
                                            if (sa == 0)
                                            {
                                                byDay = byDay + "SA";
                                                sa++;
                                            }
                                        }
                                    }

                                    break;
                                }
                        }
                        addDate = addDate.DayOfWeek == DayOfWeek.Saturday ? addDate.AddDays(((WyWeekGap - 1) * 7) + 1) : addDate.AddDays(1);
                    }
                    RRule = RRule + ";COUNT=" + recCount;
                    if (WyWeekGap > 1)
                    {
                        RRule = RRule + ";INTERVAL=" + WyWeekGap;
                    }
                    if (byDay != "" && byDay != string.Empty)
                    {
                        RRule = RRule + ";BYDAY=" + byDay;
                    }
                    else
                    {
                        isValidRecurrence = false;
                    }
                }

                #endregion

                #region monthly

                else if (RecProp.RecurrenceType == RecurrenceType.Monthly)
                {
                    RRule = "FREQ=MONTHLY";

                    if (RecProp.IsMonthlySpecific)
                    {
                        addDate = startDate;
                        MyMonthGap = RecProp.MonthlyEveryNMonths;
                        int monthDate = RecProp.MonthlySpecificMonthDay;
                        int currDate = int.Parse(startDate.Day.ToString());
                        int diffDate = monthDate - currDate;
                        addDate = addDate.AddDays(diffDate);

                        if (monthDate < currDate)
                        {
                            addDate = addDate.AddMonths(1);
                        }

                        while (addDate.Date <= endDate.Date)
                        {
                            if (recCount != 0)
                            {
                                tempTimeSpan = addDate - prevDate;
                                if (tempTimeSpan >= diffTimeSpan)
                                {
                                    prevDate = addDate;
                                    recCount++;
                                }
                                else
                                {
                                    isValidRecurrence = false;
                                    break;
                                }
                            }
                            else
                            {
                                prevDate = addDate;
                                recCount++;
                            }

                            addDate = addDate.AddMonths(MyMonthGap);
                        }
                        RRule = RRule + ";COUNT=" + recCount;
                        if (MyMonthGap > 1)
                        {
                            RRule = RRule + ";INTERVAL=" + MyMonthGap;
                        }
                        //FREQ=MONTHLY;COUNT=6;INTERVAL=2;BYMONTHDAY=3
                        RRule = RRule + ";BYMONTHDAY=" + (RecProp.MonthlySpecificMonthDay);
                    }

                    else
                    {
                        addDate = startDate;
                        MyMonthGap = RecProp.MonthlyEveryNMonths;
                        while (addDate.Date <= endDate.Date)
                        {
                            var monthStart = new DateTime(addDate.Year, addDate.Month, 1);
                            DateTime weekStartDate = monthStart.AddDays(-(int)(monthStart.DayOfWeek));
                            var monthStartWeekday = (int)(monthStart.DayOfWeek);
                            int nthweekDay = RecProp.MonthlyWeekDay;
                            int nthWeek;
                            if (monthStartWeekday <= nthweekDay)
                            {
                                nthWeek = RecProp.MonthlyNthWeek - 1;
                            }
                            else
                            {
                                nthWeek = RecProp.MonthlyNthWeek;
                            }
                            addDate = weekStartDate.AddDays((nthWeek) * 7);
                            addDate = addDate.AddDays(nthweekDay);
                            if (addDate.CompareTo(startDate) < 0)
                            {
                                addDate = addDate.AddMonths(1);
                                continue;
                            }

                            if (recCount != 0)
                            {
                                tempTimeSpan = addDate - prevDate;
                                if (tempTimeSpan >= diffTimeSpan)
                                {
                                    prevDate = addDate;
                                    recCount++;
                                }
                                else
                                {
                                    isValidRecurrence = false;
                                    break;
                                }
                            }
                            else
                            {
                                prevDate = addDate;
                                recCount++;
                            }
                            addDate = addDate.AddMonths(MyMonthGap);
                        }
                        RRule = RRule + ";COUNT=" + recCount;
                        if (MyMonthGap > 1)
                        {
                            RRule = RRule + ";INTERVAL=" + MyMonthGap;
                        }
                        //FREQ=MONTHLY;COUNT=6;INTERVAL=2;BYDAY=WE;BYSETPOS=1
                        RRule = RRule + ";BYDAY=" + (CultureInfo.CurrentCulture.DateTimeFormat.ShortestDayNames[RecProp.MonthlyWeekDay - 1]).ToUpper() + ";BYSETPOS=" + RecProp.MonthlyNthWeek;
                    }
                }
                #endregion

                #region Yearly
                else if (RecProp.RecurrenceType == RecurrenceType.Yearly)
                {

                    YyYearGap = RecProp.YearlyEveryNYears;
                    RRule = "FREQ=YEARLY";
                    addDate = startDate;
                    if (RecProp.IsYearlySpecific)
                    {
                        int monthIndex = RecProp.YearlySpecificMonth;
                        int dayIndex = RecProp.YearlySpecificMonthDay;
                        int daysInMonth = DateTime.DaysInMonth(addDate.Year, monthIndex);
                        if (dayIndex <= daysInMonth)
                        {
                            var specificDate = new DateTime(addDate.Year, monthIndex, dayIndex);
                            if (specificDate.Date < addDate.Date)
                            {
                                addDate = specificDate;
                                addDate = addDate.AddYears(1);
                            }
                            else
                            {
                                addDate = specificDate;
                            }
                            while (addDate.Date <= endDate.Date)
                            {

                                if (recCount != 0)
                                {
                                    tempTimeSpan = addDate - prevDate;
                                    if (tempTimeSpan >= diffTimeSpan)
                                    {
                                        prevDate = addDate;
                                        recCount++;
                                    }
                                    else
                                    {
                                        isValidRecurrence = false;
                                        break;
                                    }
                                }
                                else
                                {
                                    prevDate = addDate;
                                    recCount++;
                                }
                                addDate = addDate.AddYears(YyYearGap);
                            }
                        }
                        RRule = RRule + ";COUNT=" + recCount;
                        if (YyYearGap > 1)
                        {
                            RRule = RRule + ";INTERVAL=" + YyYearGap;
                        }
                        //FREQ=YEARLY;COUNT=6;INTERVAL=2;BYMONTHDAY=3;BYMONTH=4
                        RRule = RRule + ";BYMONTHDAY=" + (RecProp.YearlySpecificMonthDay) + ";BYMONTH=" + (RecProp.YearlySpecificMonth);
                    }

                    else
                    {
                        addDate = startDate;
                        int monthIndex = RecProp.YearlyGenericMonth;
                        while (addDate.Date <= endDate.Date)
                        {
                            var monthStart = new DateTime(addDate.Year, monthIndex, 1);
                            DateTime weekStartDate = monthStart.AddDays(-(int)(monthStart.DayOfWeek));
                            var monthStartWeekday = (int)(monthStart.DayOfWeek);
                            int nthweekDay = RecProp.YearlyWeekDay;
                            int nthWeek;
                            if (monthStartWeekday <= nthweekDay)
                            {
                                nthWeek = RecProp.YearlyNthWeek - 1;
                            }
                            else
                            {
                                nthWeek = RecProp.YearlyNthWeek;
                            }
                            addDate = weekStartDate.AddDays((nthWeek) * 7);
                            addDate = addDate.AddDays(nthweekDay);
                            if (addDate.CompareTo(startDate) < 0)
                            {
                                addDate = addDate.AddYears(1);
                                continue;
                            }
                            if (recCount != 0)
                            {
                                tempTimeSpan = addDate - prevDate;
                                if (tempTimeSpan >= diffTimeSpan)
                                {
                                    prevDate = addDate;
                                    recCount++;
                                }
                                else
                                {
                                    isValidRecurrence = false;
                                    break;
                                }
                            }
                            else
                            {
                                prevDate = addDate;
                                recCount++;
                            }
                            addDate = addDate.AddYears(YyYearGap);
                        }
                        RRule = RRule + ";COUNT=" + recCount;
                        if (YyYearGap > 1)
                        {
                            RRule = RRule + ";INTERVAL=" + YyYearGap;
                        }
                        //FREQ=YEARLY;COUNT=6;INTERVAL=2;BYDAY=WE;BYMONTH=4;BYSETPOS=1
                        RRule = RRule + ";BYDAY=" + (CultureInfo.CurrentCulture.DateTimeFormat.ShortestDayNames[RecProp.YearlyWeekDay - 1]).ToUpper() + ";BYMONTH=" + (RecProp.YearlyGenericMonth) + ";BYSETPOS=" + RecProp.YearlyNthWeek;
                    }
                }
                #endregion

            }
            else if (RecProp.IsRangeNoEndDate)
            {
                addDate = startDate;

                #region Daily
                if (RecProp.RecurrenceType == RecurrenceType.Daily)
                {
                    RRule = "FREQ=DAILY";
                    if (RecProp.IsDailyEveryNDays)
                    {
                        DyDayGap = RecProp.DailyNDays;
                        if (diffTimeSpan.TotalHours >= DyDayGap * 24)
                        {
                            isValidRecurrence = false;
                        }
                        if (DyDayGap > 1)
                        {
                            //FREQ=DAILY;COUNT=5;INTERVAL=2
                            RRule = RRule + ";INTERVAL=" + DyDayGap;
                        }
                    }
                    else
                    {
                        if (diffTimeSpan.TotalHours > 24)
                        {
                            isValidRecurrence = false;
                        }
                        //FREQ=WEEKLY;COUNT=5;BYDAY=MO,TU,WE,TH,FR
                        RRule = RRule + ";BYDAY=MO,TU,WE,TH,FR";
                    }
                }
                #endregion

                #region Weekly
                else if (RecProp.RecurrenceType == RecurrenceType.Weekly)
                {
                    //FREQ=WEEKLY;COUNT=6;INTERVAL=2;BYDAY=SU,MO,WE,FR;WKST=SU
                    RRule = "FREQ=WEEKLY";
                    string byDay = "";
                    int su = 0, mo = 0, tu = 0, we = 0, th = 0, fr = 0, sa = 0;
                    WyWeekGap = RecProp.WeeklyEveryNWeeks;
                    if (WyWeekGap > 1)
                    {
                        //FREQ=DAILY;COUNT=5;INTERVAL=2
                        RRule = RRule + ";INTERVAL=" + WyWeekGap;
                    }
                    RRule = RRule + ";BYDAY=";
                    int count = 0;
                    bool isweeklyday_selected = RecProp.IsWeeklyFriday || RecProp.IsWeeklyMonday || RecProp.IsWeeklySaturday || RecProp.IsWeeklySunday || RecProp.IsWeeklyThursday || RecProp.IsWeeklyTuesday || RecProp.IsWeeklyWednesday;
                    while (count < recCount && isValidRecurrence && isweeklyday_selected)
                    {
                        switch (addDate.DayOfWeek)
                        {
                            case DayOfWeek.Sunday:
                                {
                                    if (RecProp.IsWeeklySunday && count < recCount)
                                    {
                                        if (count != 0)
                                        {
                                            tempTimeSpan = addDate - prevDate;
                                            if (tempTimeSpan <= diffTimeSpan)
                                            {
                                                isValidRecurrence = false;
                                            }
                                            else
                                            {
                                                prevDate = addDate;
                                                if (su == 1)
                                                {
                                                    count = recCount;
                                                    break;
                                                }
                                                byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",SU," : byDay + "SU,";
                                                su++;
                                            }
                                        }
                                        else
                                        {
                                            prevDate = addDate;
                                            count++;
                                            byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",SU," : byDay + "SU,";
                                            su++;
                                        }
                                    }
                                    break;
                                }
                            case DayOfWeek.Monday:
                                {
                                    if (RecProp.IsWeeklyMonday && count < recCount)
                                    {
                                        if (count != 0)
                                        {
                                            tempTimeSpan = addDate - prevDate;
                                            if (tempTimeSpan <= diffTimeSpan)
                                            {
                                                isValidRecurrence = false;
                                            }
                                            else
                                            {
                                                prevDate = addDate;
                                                if (mo == 1)
                                                {
                                                    count = recCount;
                                                    break;
                                                }
                                                byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",MO," : byDay + "MO,";
                                                mo++;
                                            }
                                        }
                                        else
                                        {
                                            prevDate = addDate;
                                            count++;
                                            byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",MO," : byDay + "MO,";
                                            mo++;
                                        }
                                    }
                                    break;
                                }
                            case DayOfWeek.Tuesday:
                                {
                                    if (RecProp.IsWeeklyTuesday && count < recCount)
                                    {
                                        if (count != 0)
                                        {
                                            tempTimeSpan = addDate - prevDate;
                                            if (tempTimeSpan <= diffTimeSpan)
                                            {
                                                isValidRecurrence = false;
                                            }
                                            else
                                            {
                                                prevDate = addDate;
                                                if (tu == 1)
                                                {
                                                    count = recCount;
                                                    break;
                                                }
                                                byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",TU," : byDay + "TU,";
                                                tu++;
                                            }
                                        }
                                        else
                                        {
                                            prevDate = addDate;
                                            count++;
                                            byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",TU," : byDay + "TU,";
                                            tu++;
                                        }
                                    }
                                    break;
                                }
                            case DayOfWeek.Wednesday:
                                {
                                    if (RecProp.IsWeeklyWednesday && count < recCount)
                                    {
                                        if (count != 0)
                                        {
                                            tempTimeSpan = addDate - prevDate;
                                            if (tempTimeSpan <= diffTimeSpan)
                                            {
                                                isValidRecurrence = false;
                                            }
                                            else
                                            {
                                                prevDate = addDate;
                                                if (we == 1)
                                                {
                                                    count = recCount;
                                                    break;
                                                }
                                                byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",WE," : byDay + "WE,";
                                                we++;
                                            }
                                        }
                                        else
                                        {
                                            prevDate = addDate;
                                            count++;
                                            byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",WE," : byDay + "WE,";
                                            we++;
                                        }
                                    }
                                    break;
                                }
                            case DayOfWeek.Thursday:
                                {
                                    if (RecProp.IsWeeklyThursday && count < recCount)
                                    {
                                        if (count != 0)
                                        {
                                            tempTimeSpan = addDate - prevDate;
                                            if (tempTimeSpan <= diffTimeSpan)
                                            {
                                                isValidRecurrence = false;
                                            }
                                            else
                                            {
                                                prevDate = addDate;
                                                if (th == 1)
                                                {
                                                    count = recCount;
                                                    break;
                                                }
                                                byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",TH," : byDay + "TH,";
                                                th++;
                                            }
                                        }
                                        else
                                        {
                                            prevDate = addDate;
                                            count++;
                                            byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",TH," : byDay + "TH,";
                                            th++;
                                        }
                                    }
                                    break;
                                }
                            case DayOfWeek.Friday:
                                {
                                    if (RecProp.IsWeeklyFriday && count < recCount)
                                    {
                                        if (count != 0)
                                        {
                                            tempTimeSpan = addDate - prevDate;
                                            if (tempTimeSpan <= diffTimeSpan)
                                            {
                                                isValidRecurrence = false;
                                            }
                                            else
                                            {
                                                prevDate = addDate;
                                                if (fr == 1)
                                                {
                                                    count = recCount;
                                                    break;
                                                }
                                                byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",FR," : byDay + "FR,";
                                                fr++;
                                            }
                                        }
                                        else
                                        {
                                            prevDate = addDate;
                                            count++;
                                            byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",FR," : byDay + "FR,";
                                            fr++;
                                        }
                                    }
                                    break;
                                }
                            case DayOfWeek.Saturday:
                                {
                                    if (RecProp.IsWeeklySaturday && count < recCount)
                                    {
                                        if (count != 0)
                                        {
                                            tempTimeSpan = addDate - prevDate;
                                            if (tempTimeSpan <= diffTimeSpan)
                                            {
                                                isValidRecurrence = false;
                                            }
                                            else
                                            {
                                                prevDate = addDate;
                                                if (sa == 1)
                                                {
                                                    count = recCount;
                                                    break;
                                                }
                                                byDay = byDay + "SA";
                                                sa++;
                                            }
                                        }
                                        else
                                        {
                                            prevDate = addDate;
                                            count++;
                                            byDay = byDay + "SA";
                                            sa++;
                                        }
                                    }
                                    break;
                                }
                        }
                        addDate = addDate.DayOfWeek == DayOfWeek.Saturday ? addDate.AddDays(((WyWeekGap - 1) * 7) + 1) : addDate.AddDays(1);
                    }
                    RRule = RRule + byDay;
                }
                #endregion

                #region Monthly
                else if (RecProp.RecurrenceType == RecurrenceType.Monthly)
                {

                    RRule = "FREQ=MONTHLY";
                    int count = 0;
                    MyMonthGap = RecProp.MonthlyEveryNMonths;
                    if (MyMonthGap > 1)
                    {
                        RRule = RRule + ";INTERVAL=" + MyMonthGap;
                    }


                    if (RecProp.IsMonthlySpecific)
                    {
                        //FREQ=MONTHLY;COUNT=6;INTERVAL=2;BYMONTHDAY=3
                        RRule = RRule + ";BYMONTHDAY=" + (RecProp.MonthlySpecificMonthDay);
                        int monthDate = RecProp.MonthlySpecificMonthDay;
                        int currDate = int.Parse(startDate.Day.ToString());
                        int diffDate = monthDate - currDate;
                        addDate = addDate.AddDays(diffDate);
                        if (monthDate < currDate)
                        {
                            addDate = addDate.AddMonths(1);
                        }
                        for (int i = 0; i < recCount; i++)
                        {

                            //RecDateCollection.Add(addDate.Date);
                            //addDate = addDate.AddMonths(MyMonthGap);

                            if (count != 0)
                            {
                                tempTimeSpan = addDate - prevDate;
                                if (tempTimeSpan >= diffTimeSpan)
                                {
                                    prevDate = addDate;
                                    count++;
                                }
                                else
                                {
                                    isValidRecurrence = false;
                                    break;
                                }
                            }
                            else
                            {
                                prevDate = addDate;
                                count++;
                            }
                            //RecDateCollection.Add(addDate.Date);
                            addDate = addDate.AddMonths(MyMonthGap);
                        }
                    }

                    else
                    {
                        //FREQ=MONTHLY;COUNT=6;INTERVAL=2;BYDAY=WE;BYSETPOS=1
                        RRule = RRule + ";BYDAY=" + (CultureInfo.CurrentCulture.DateTimeFormat.ShortestDayNames[RecProp.MonthlyWeekDay - 1]).ToUpper() + ";BYSETPOS=" + RecProp.MonthlyNthWeek;

                        //MyMonthGap = RecProp.MonthlyEveryNMonths;
                        while (count < recCount)
                        {
                            var monthStart = new DateTime(addDate.Year, addDate.Month, 1);
                            var weekStartDate = monthStart.AddDays(-(int)(monthStart.DayOfWeek));
                            var monthStartWeekday = (int)(monthStart.DayOfWeek);
                            int nthweekDay = RecProp.MonthlyWeekDay;
                            int nthWeek;
                            if (monthStartWeekday <= nthweekDay)
                            {
                                nthWeek = RecProp.MonthlyNthWeek - 1;
                            }
                            else
                            {
                                nthWeek = RecProp.MonthlyNthWeek;
                            }
                            addDate = weekStartDate.AddDays((nthWeek) * 7);
                            addDate = addDate.AddDays(nthweekDay);
                            if (addDate.CompareTo(startDate) < 0)
                            {
                                addDate = addDate.AddMonths(1);
                                continue;
                            }

                            if (count != 0)
                            {
                                tempTimeSpan = addDate - prevDate;
                                if (tempTimeSpan >= diffTimeSpan)
                                {
                                    prevDate = addDate;
                                    count++;
                                }
                                else
                                {
                                    isValidRecurrence = false;
                                    break;
                                }
                            }
                            else
                            {
                                prevDate = addDate;
                                count++;
                            }
                            addDate = addDate.AddMonths(MyMonthGap);
                        }
                    }

                }
                #endregion

                #region Yearly

                else if (RecProp.RecurrenceType == RecurrenceType.Yearly)
                {

                    RRule = "FREQ=YEARLY";
                    int count = 0;
                    YyYearGap = RecProp.YearlyEveryNYears;
                    if (YyYearGap > 1)
                    {
                        RRule = RRule + ";INTERVAL=" + YyYearGap;
                    }

                    if (RecProp.IsYearlySpecific)
                    {
                        //FREQ=YEARLY;COUNT=6;INTERVAL=2;BYMONTHDAY=3;BYMONTH=4
                        RRule = RRule + ";BYMONTHDAY=" + (RecProp.YearlySpecificMonthDay) + ";BYMONTH=" + (RecProp.YearlySpecificMonth);
                        int monthIndex = RecProp.YearlySpecificMonth;
                        int dayIndex = RecProp.YearlySpecificMonthDay;
                        int bound = DateTime.DaysInMonth(addDate.Year, monthIndex);
                        if (bound >= dayIndex)
                        {
                            var specificDate = new DateTime(addDate.Year, monthIndex, dayIndex);
                            if (specificDate.Date < addDate.Date)
                            {
                                addDate = specificDate;
                                addDate = addDate.AddYears(1);

                            }
                            else
                            {
                                addDate = specificDate;
                            }

                            for (int i = 0; i < recCount; i++)
                            {
                                if (count != 0)
                                {
                                    tempTimeSpan = addDate - prevDate;
                                    if (tempTimeSpan >= diffTimeSpan)
                                    {
                                        prevDate = addDate;
                                        count++;
                                    }
                                    else
                                    {
                                        isValidRecurrence = false;
                                        break;
                                    }
                                }
                                else
                                {
                                    prevDate = addDate;
                                    count++;
                                }
                                addDate = addDate.AddYears(YyYearGap);
                            }
                        }
                    }
                    else
                    {
                        //FREQ=YEARLY;COUNT=6;INTERVAL=2;BYDAY=WE;BYMONTH=4;BYSETPOS=1
                        RRule = RRule + ";BYDAY=" + (CultureInfo.CurrentCulture.DateTimeFormat.ShortestDayNames[RecProp.YearlyWeekDay - 1]).ToUpper() + ";BYMONTH=" + (RecProp.YearlyGenericMonth) + ";BYSETPOS=" + RecProp.YearlyNthWeek;

                        int monthIndex = RecProp.YearlyGenericMonth;
                        while (count < recCount)
                        {
                            var monthStart = new DateTime(addDate.Year, monthIndex, 1);
                            DateTime weekStartDate = monthStart.AddDays(-(int)(monthStart.DayOfWeek));
                            var monthStartWeekday = (int)(monthStart.DayOfWeek);
                            int nthweekDay = RecProp.YearlyWeekDay;
                            int nthWeek;
                            if (monthStartWeekday <= nthweekDay)
                            {
                                nthWeek = RecProp.YearlyNthWeek - 1;
                            }
                            else
                            {
                                nthWeek = RecProp.YearlyNthWeek;
                            }
                            addDate = weekStartDate.AddDays((nthWeek) * 7);
                            addDate = addDate.AddDays(nthweekDay);
                            if (addDate.CompareTo(startDate) < 0)
                            {
                                addDate = addDate.AddYears(1);
                                continue;
                            }

                            if (count != 0)
                            {
                                tempTimeSpan = addDate - prevDate;
                                if (tempTimeSpan >= diffTimeSpan)
                                {
                                    prevDate = addDate;
                                    count++;
                                }
                                else
                                {
                                    isValidRecurrence = false;
                                    break;
                                }
                            }
                            else
                            {
                                prevDate = addDate;
                                count++;
                            }
                            addDate = addDate.AddYears(YyYearGap);

                        }
                    }

                }
                #endregion
            }
            if (!isValidRecurrence)
                RRule = string.Empty;
            if (!RecPropertiesDict.ContainsKey(RRule))
                RecPropertiesDict.Add(RRule, RecProp);
            else
                RecPropertiesDict[RRule] = RecProp;
            return RRule;
        }

        internal static string InternalRRuleGenerator(RecurrenceProperties RecProp, DateTime AppStartTime, DateTime AppEndTime)
        {
            DateTime startDate = RecProp.RangeStartDate;
            DateTime endDate = RecProp.RangeEndDate;
            DateTime addDate;
            TimeSpan diffTimeSpan = AppEndTime - AppStartTime;
            TimeSpan tempTimeSpan;
            int recCount = 0;
            int DyDayGap;
            int WyWeekGap;
            int MyMonthGap;
            int YyYearGap;
            string RRule = string.Empty;
            var prevDate = new DateTime();
            bool isValidRecurrence = true;
            if (RecProp.IsRangeRecurrenceCount)
                recCount = RecProp.RangeRecurrenceCount;
            // Check recurrence compatibility
            if (recCount > 0 && RecProp.IsRangeRecurrenceCount)
            {

                addDate = startDate;

                #region Daily
                if (RecProp.RecurrenceType == RecurrenceType.Daily)
                {
                    RRule = "FREQ=DAILY;COUNT=" + recCount;
                    if (RecProp.IsDailyEveryNDays)
                    {
                        DyDayGap = RecProp.DailyNDays;
                        if (diffTimeSpan.TotalHours >= DyDayGap * 24)
                        {
                            isValidRecurrence = false;
                        }
                        if (DyDayGap > 1)
                        {
                            //FREQ=DAILY;COUNT=5;INTERVAL=2
                            RRule = RRule + ";INTERVAL=" + DyDayGap;
                        }
                    }
                    else
                    {
                        if (diffTimeSpan.TotalHours > 24)
                        {
                            isValidRecurrence = false;
                        }
                        //FREQ=WEEKLY;COUNT=5;BYDAY=MO,TU,WE,TH,FR
                        RRule = RRule + ";BYDAY=MO,TU,WE,TH,FR";
                    }
                }
                #endregion

                #region Weekly
                else if (RecProp.RecurrenceType == RecurrenceType.Weekly)
                {
                    //FREQ=WEEKLY;COUNT=6;INTERVAL=2;BYDAY=SU,MO,WE,FR;WKST=SU
                    RRule = "FREQ=WEEKLY;COUNT=" + recCount;
                    string byDay = "";
                    int su = 0, mo = 0, tu = 0, we = 0, th = 0, fr = 0, sa = 0;
                    WyWeekGap = RecProp.WeeklyEveryNWeeks;
                    if (WyWeekGap > 1)
                    {
                        //FREQ=DAILY;COUNT=5;INTERVAL=2
                        RRule = RRule + ";INTERVAL=" + WyWeekGap;
                    }
                    RRule = RRule + ";BYDAY=";
                    int count = 0;
                    bool isweeklyday_selected = RecProp.IsWeeklyFriday || RecProp.IsWeeklyMonday || RecProp.IsWeeklySaturday || RecProp.IsWeeklySunday || RecProp.IsWeeklyThursday || RecProp.IsWeeklyTuesday || RecProp.IsWeeklyWednesday;
                    while (count < recCount && isValidRecurrence && isweeklyday_selected)
                    {
                        switch (addDate.DayOfWeek)
                        {
                            case DayOfWeek.Sunday:
                                {
                                    if (RecProp.IsWeeklySunday && count < recCount)
                                    {
                                        if (count != 0)
                                        {
                                            tempTimeSpan = addDate - prevDate;
                                            if (tempTimeSpan <= diffTimeSpan)
                                            {
                                                isValidRecurrence = false;
                                            }
                                            else
                                            {
                                                prevDate = addDate;
                                                if (su == 1)
                                                {
                                                    count = recCount;
                                                    break;
                                                }
                                                byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",SU," : byDay + "SU,";
                                                su++;
                                            }
                                        }
                                        else
                                        {
                                            prevDate = addDate;
                                            count++;
                                            byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",SU," : byDay + "SU,";
                                            su++;
                                        }
                                    }
                                    break;
                                }
                            case DayOfWeek.Monday:
                                {
                                    if (RecProp.IsWeeklyMonday && count < recCount)
                                    {
                                        if (count != 0)
                                        {
                                            tempTimeSpan = addDate - prevDate;
                                            if (tempTimeSpan <= diffTimeSpan)
                                            {
                                                isValidRecurrence = false;
                                            }
                                            else
                                            {
                                                prevDate = addDate;
                                                if (mo == 1)
                                                {
                                                    count = recCount;
                                                    break;
                                                }
                                                byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",MO," : byDay + "MO,";
                                                mo++;
                                            }
                                        }
                                        else
                                        {
                                            prevDate = addDate;
                                            count++;
                                            byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",MO," : byDay + "MO,";
                                            mo++;
                                        }
                                    }
                                    break;
                                }
                            case DayOfWeek.Tuesday:
                                {
                                    if (RecProp.IsWeeklyTuesday && count < recCount)
                                    {
                                        if (count != 0)
                                        {
                                            tempTimeSpan = addDate - prevDate;
                                            if (tempTimeSpan <= diffTimeSpan)
                                            {
                                                isValidRecurrence = false;
                                            }
                                            else
                                            {
                                                prevDate = addDate;
                                                if (tu == 1)
                                                {
                                                    count = recCount;
                                                    break;
                                                }
                                                byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",TU," : byDay + "TU,";
                                                tu++;
                                            }
                                        }
                                        else
                                        {
                                            prevDate = addDate;
                                            count++;
                                            byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",TU," : byDay + "TU,";
                                            tu++;
                                        }
                                    }
                                    break;
                                }
                            case DayOfWeek.Wednesday:
                                {
                                    if (RecProp.IsWeeklyWednesday && count < recCount)
                                    {
                                        if (count != 0)
                                        {
                                            tempTimeSpan = addDate - prevDate;
                                            if (tempTimeSpan <= diffTimeSpan)
                                            {
                                                isValidRecurrence = false;
                                            }
                                            else
                                            {
                                                prevDate = addDate;
                                                if (we == 1)
                                                {
                                                    count = recCount;
                                                    break;
                                                }
                                                byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",WE," : byDay + "WE,";
                                                we++;
                                            }
                                        }
                                        else
                                        {
                                            prevDate = addDate;
                                            count++;
                                            byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",WE," : byDay + "WE,";
                                            we++;
                                        }
                                    }
                                    break;
                                }
                            case DayOfWeek.Thursday:
                                {
                                    if (RecProp.IsWeeklyThursday && count < recCount)
                                    {
                                        if (count != 0)
                                        {
                                            tempTimeSpan = addDate - prevDate;
                                            if (tempTimeSpan <= diffTimeSpan)
                                            {
                                                isValidRecurrence = false;
                                            }
                                            else
                                            {
                                                prevDate = addDate;
                                                if (th == 1)
                                                {
                                                    count = recCount;
                                                    break;
                                                }
                                                byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",TH," : byDay + "TH,";
                                                th++;
                                            }
                                        }
                                        else
                                        {
                                            prevDate = addDate;
                                            count++;
                                            byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",TH," : byDay + "TH,";
                                            th++;
                                        }
                                    }
                                    break;
                                }
                            case DayOfWeek.Friday:
                                {
                                    if (RecProp.IsWeeklyFriday && count < recCount)
                                    {
                                        if (count != 0)
                                        {
                                            tempTimeSpan = addDate - prevDate;
                                            if (tempTimeSpan <= diffTimeSpan)
                                            {
                                                isValidRecurrence = false;
                                            }
                                            else
                                            {
                                                prevDate = addDate;
                                                if (fr == 1)
                                                {
                                                    count = recCount;
                                                    break;
                                                }
                                                byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",FR," : byDay + "FR,";
                                                fr++;
                                            }
                                        }
                                        else
                                        {
                                            prevDate = addDate;
                                            count++;
                                            byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",FR," : byDay + "FR,";
                                            fr++;
                                        }
                                    }
                                    break;
                                }
                            case DayOfWeek.Saturday:
                                {
                                    if (RecProp.IsWeeklySaturday && count < recCount)
                                    {
                                        if (count != 0)
                                        {
                                            tempTimeSpan = addDate - prevDate;
                                            if (tempTimeSpan <= diffTimeSpan)
                                            {
                                                isValidRecurrence = false;
                                            }
                                            else
                                            {
                                                prevDate = addDate;
                                                if (sa == 1)
                                                {
                                                    count = recCount;
                                                    break;
                                                }
                                                byDay = byDay + "SA";
                                                sa++;
                                            }
                                        }
                                        else
                                        {
                                            prevDate = addDate;
                                            count++;
                                            byDay = byDay + "SA";
                                            sa++;
                                        }
                                    }
                                    break;
                                }
                        }
                        addDate = addDate.DayOfWeek == DayOfWeek.Saturday ? addDate.AddDays(((WyWeekGap - 1) * 7) + 1) : addDate.AddDays(1);
                    }
                    RRule = RRule + byDay;
                }
                #endregion

                #region Monthly
                else if (RecProp.RecurrenceType == RecurrenceType.Monthly)
                {

                    RRule = "FREQ=MONTHLY;COUNT=" + recCount;
                    int count = 0;
                    MyMonthGap = RecProp.MonthlyEveryNMonths;
                    if (MyMonthGap > 1)
                    {
                        RRule = RRule + ";INTERVAL=" + MyMonthGap;
                    }


                    if (RecProp.IsMonthlySpecific)
                    {
                        //FREQ=MONTHLY;COUNT=6;INTERVAL=2;BYMONTHDAY=3
                        RRule = RRule + ";BYMONTHDAY=" + (RecProp.MonthlySpecificMonthDay);
                        int monthDate = RecProp.MonthlySpecificMonthDay;
                        int currDate = int.Parse(startDate.Day.ToString());
                        int diffDate = monthDate - currDate;
                        addDate = addDate.AddDays(diffDate);
                        if (monthDate < currDate)
                        {
                            addDate = addDate.AddMonths(1);
                        }
                        for (int i = 0; i < recCount; i++)
                        {

                            //RecDateCollection.Add(addDate.Date);
                            //addDate = addDate.AddMonths(MyMonthGap);

                            if (count != 0)
                            {
                                tempTimeSpan = addDate - prevDate;
                                if (tempTimeSpan >= diffTimeSpan)
                                {
                                    prevDate = addDate;
                                    count++;
                                }
                                else
                                {
                                    isValidRecurrence = false;
                                    break;
                                }
                            }
                            else
                            {
                                prevDate = addDate;
                                count++;
                            }
                            //RecDateCollection.Add(addDate.Date);
                            addDate = addDate.AddMonths(MyMonthGap);
                        }
                    }

                    else
                    {
                        //FREQ=MONTHLY;COUNT=6;INTERVAL=2;BYDAY=WE;BYSETPOS=1
                        RRule = RRule + ";BYDAY=" + (CultureInfo.CurrentCulture.DateTimeFormat.ShortestDayNames[RecProp.MonthlyWeekDay - 1]).ToUpper() + ";BYSETPOS=" + RecProp.MonthlyNthWeek;
                        //MyMonthGap = RecProp.MonthlyEveryNMonths;
                        while (count < recCount)
                        {
                            var monthStart = new DateTime(addDate.Year, addDate.Month, 1);
                            var weekStartDate = monthStart.AddDays(-(int)(monthStart.DayOfWeek));
                            var monthStartWeekday = (int)(monthStart.DayOfWeek);
                            int nthweekDay = RecProp.MonthlyWeekDay;
                            int nthWeek;
                            if (monthStartWeekday <= nthweekDay)
                            {
                                nthWeek = RecProp.MonthlyNthWeek - 1;
                            }
                            else
                            {
                                nthWeek = RecProp.MonthlyNthWeek;
                            }
                            addDate = weekStartDate.AddDays((nthWeek) * 7);
                            addDate = addDate.AddDays(nthweekDay);
                            if (addDate.CompareTo(startDate) < 0)
                            {
                                addDate = addDate.AddMonths(1);
                                continue;
                            }

                            if (count != 0)
                            {
                                tempTimeSpan = addDate - prevDate;
                                if (tempTimeSpan >= diffTimeSpan)
                                {
                                    prevDate = addDate;
                                    count++;
                                }
                                else
                                {
                                    isValidRecurrence = false;
                                    break;
                                }
                            }
                            else
                            {
                                prevDate = addDate;
                                count++;
                            }
                            addDate = addDate.AddMonths(MyMonthGap);
                        }
                    }

                }
                #endregion

                #region Yearly

                else if (RecProp.RecurrenceType == RecurrenceType.Yearly)
                {

                    RRule = "FREQ=YEARLY;COUNT=" + recCount;
                    int count = 0;
                    YyYearGap = RecProp.YearlyEveryNYears;
                    if (YyYearGap > 1)
                    {
                        RRule = RRule + ";INTERVAL=" + YyYearGap;
                    }

                    if (RecProp.IsYearlySpecific)
                    {
                        //FREQ=YEARLY;COUNT=6;INTERVAL=2;BYMONTHDAY=3;BYMONTH=4
                        RRule = RRule + ";BYMONTHDAY=" + (RecProp.YearlySpecificMonthDay) + ";BYMONTH=" + (RecProp.YearlySpecificMonth);
                        int monthIndex = RecProp.YearlySpecificMonth;
                        int dayIndex = RecProp.YearlySpecificMonthDay;
                        int bound = DateTime.DaysInMonth(addDate.Year, monthIndex);
                        if (bound >= dayIndex)
                        {
                            var specificDate = new DateTime(addDate.Year, monthIndex, dayIndex);
                            if (specificDate.Date < addDate.Date)
                            {
                                addDate = specificDate;
                                addDate = addDate.AddYears(1);

                            }
                            else
                            {
                                addDate = specificDate;
                            }

                            for (int i = 0; i < recCount; i++)
                            {
                                if (count != 0)
                                {
                                    tempTimeSpan = addDate - prevDate;
                                    if (tempTimeSpan >= diffTimeSpan)
                                    {
                                        prevDate = addDate;
                                        count++;
                                    }
                                    else
                                    {
                                        isValidRecurrence = false;
                                        break;
                                    }
                                }
                                else
                                {
                                    prevDate = addDate;
                                    count++;
                                }
                                addDate = addDate.AddYears(YyYearGap);
                            }
                        }
                    }

                    else
                    {
                        //FREQ=YEARLY;COUNT=6;INTERVAL=2;BYDAY=WE;BYMONTH=4;BYSETPOS=1
                        RRule = RRule + ";BYDAY=" + (CultureInfo.CurrentCulture.DateTimeFormat.ShortestDayNames[RecProp.YearlyWeekDay - 1]).ToUpper() + ";BYMONTH=" + (RecProp.YearlyGenericMonth) + ";BYSETPOS=" + RecProp.YearlyNthWeek;
                        int monthIndex = RecProp.YearlyGenericMonth;
                        while (count < recCount)
                        {
                            var monthStart = new DateTime(addDate.Year, monthIndex, 1);
                            DateTime weekStartDate = monthStart.AddDays(-(int)(monthStart.DayOfWeek));
                            var monthStartWeekday = (int)(monthStart.DayOfWeek);
                            int nthweekDay = RecProp.YearlyWeekDay;
                            int nthWeek;
                            if (monthStartWeekday <= nthweekDay)
                            {
                                nthWeek = RecProp.YearlyNthWeek - 1;
                            }
                            else
                            {
                                nthWeek = RecProp.YearlyNthWeek;
                            }
                            addDate = weekStartDate.AddDays((nthWeek) * 7);
                            addDate = addDate.AddDays(nthweekDay);
                            if (addDate.CompareTo(startDate) < 0)
                            {
                                addDate = addDate.AddYears(1);
                                continue;
                            }

                            if (count != 0)
                            {
                                tempTimeSpan = addDate - prevDate;
                                if (tempTimeSpan >= diffTimeSpan)
                                {
                                    prevDate = addDate;
                                    count++;
                                }
                                else
                                {
                                    isValidRecurrence = false;
                                    break;
                                }
                            }
                            else
                            {
                                prevDate = addDate;
                                count++;
                            }
                            addDate = addDate.AddYears(YyYearGap);

                        }
                    }
                }
                #endregion

            }
            else if (startDate <= endDate)
            {

                addDate = startDate;

                #region Daily

                if (RecProp.RecurrenceType == RecurrenceType.Daily)
                {
                    RRule = "FREQ=DAILY";
                    if (RecProp.IsDailyEveryNDays)
                    {
                        DyDayGap = RecProp.DailyNDays;
                        if (diffTimeSpan.TotalHours >= DyDayGap * 24)
                        {
                            isValidRecurrence = false;
                        }
                        else
                        {
                            while (addDate.Date <= endDate.Date)
                            {
                                addDate = addDate.AddDays(DyDayGap);
                                recCount++;
                            }
                        }
                        RRule = RRule + ";COUNT=" + recCount;
                        if (DyDayGap > 1)
                        {
                            //FREQ=DAILY;COUNT=5;INTERVAL=2
                            RRule = RRule + ";INTERVAL=" + DyDayGap;
                        }
                    }

                    else
                    {
                        if (diffTimeSpan.TotalHours > 24)
                        {
                            isValidRecurrence = false;
                        }
                        else
                        {
                            while (addDate.Date <= endDate.Date)
                            {
                                if (addDate.DayOfWeek != DayOfWeek.Sunday && addDate.DayOfWeek != DayOfWeek.Saturday)
                                {
                                    recCount++;
                                }
                                addDate = addDate.AddDays(1);
                            }
                        }
                        //FREQ=WEEKLY;COUNT=8;BYDAY=MO,TU,WE,TH,FR
                        RRule = RRule + ";COUNT=" + recCount + ";BYDAY=MO,TU,WE,TH,FR";
                    }
                }
                #endregion

                #region Weekly


                else if (RecProp.RecurrenceType == RecurrenceType.Weekly)
                {
                    //FREQ=WEEKLY;COUNT=6;INTERVAL=2;BYDAY=SU,MO,WE,FR;WKST=SU
                    RRule = "FREQ=WEEKLY";
                    WyWeekGap = RecProp.WeeklyEveryNWeeks;
                    string byDay = "";
                    int su = 0, mo = 0, tu = 0, we = 0, th = 0, fr = 0, sa = 0;
                    addDate = startDate;
                    while (addDate.Date <= endDate.Date)
                    {
                        switch (addDate.DayOfWeek)
                        {
                            case DayOfWeek.Sunday:
                                {
                                    if (RecProp.IsWeeklySunday && addDate.Date <= endDate.Date)
                                    {
                                        if (recCount != 0)
                                        {
                                            tempTimeSpan = addDate - prevDate;
                                            if (tempTimeSpan <= diffTimeSpan)
                                            {
                                                isValidRecurrence = false;
                                            }
                                            else
                                            {
                                                prevDate = addDate;
                                                recCount++;
                                                if (su == 0)
                                                {
                                                    byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",SU," : byDay + "SU,";
                                                    su++;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            prevDate = addDate;
                                            recCount++;
                                            if (su == 0)
                                            {
                                                byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",SU," : byDay + "SU,";
                                                su++;
                                            }

                                        }
                                    }
                                    break;
                                }
                            case DayOfWeek.Monday:
                                {
                                    if (RecProp.IsWeeklyMonday && addDate.Date <= endDate.Date)
                                    {
                                        if (recCount != 0)
                                        {
                                            tempTimeSpan = addDate - prevDate;
                                            if (tempTimeSpan <= diffTimeSpan)
                                            {
                                                isValidRecurrence = false;
                                            }
                                            else
                                            {
                                                prevDate = addDate;
                                                recCount++;
                                                if (mo == 0)
                                                {
                                                    byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",MO," : byDay + "MO,";
                                                    mo++;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            prevDate = addDate;
                                            recCount++;
                                            if (mo == 0)
                                            {
                                                byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",MO," : byDay + "MO,";
                                                mo++;
                                            }
                                        }
                                    }
                                    break;
                                }
                            case DayOfWeek.Tuesday:
                                {
                                    if (RecProp.IsWeeklyTuesday && addDate.Date <= endDate.Date)
                                    {
                                        if (recCount != 0)
                                        {
                                            tempTimeSpan = addDate - prevDate;
                                            if (tempTimeSpan <= diffTimeSpan)
                                            {
                                                isValidRecurrence = false;
                                            }
                                            else
                                            {
                                                prevDate = addDate;
                                                recCount++;
                                                if (tu == 0)
                                                {
                                                    byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",TU," : byDay + "TU,";
                                                    tu++;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            prevDate = addDate;
                                            recCount++;
                                            if (tu == 0)
                                            {
                                                byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",TU," : byDay + "TU,";
                                                tu++;
                                            }
                                        }
                                    }
                                    break;
                                }
                            case DayOfWeek.Wednesday:
                                {
                                    if (RecProp.IsWeeklyWednesday && addDate.Date <= endDate.Date)
                                    {
                                        if (recCount != 0)
                                        {
                                            tempTimeSpan = addDate - prevDate;
                                            if (tempTimeSpan <= diffTimeSpan)
                                            {
                                                isValidRecurrence = false;
                                            }
                                            else
                                            {
                                                prevDate = addDate;
                                                recCount++;
                                                if (we == 0)
                                                {
                                                    byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",WE," : byDay + "WE,";
                                                    we++;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            prevDate = addDate;
                                            recCount++;
                                            if (we == 0)
                                            {
                                                byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",WE," : byDay + "WE,";
                                                we++;
                                            }
                                        }
                                    }
                                    break;
                                }
                            case DayOfWeek.Thursday:
                                {
                                    if (RecProp.IsWeeklyThursday && addDate.Date <= endDate.Date)
                                    {
                                        if (recCount != 0)
                                        {
                                            tempTimeSpan = addDate - prevDate;
                                            if (tempTimeSpan <= diffTimeSpan)
                                            {
                                                isValidRecurrence = false;
                                            }
                                            else
                                            {
                                                prevDate = addDate;
                                                recCount++;
                                                if (th == 0)
                                                {
                                                    byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",TH," : byDay + "TH,";
                                                    th++;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            prevDate = addDate;
                                            recCount++;
                                            if (th == 0)
                                            {
                                                byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",TH," : byDay + "TH,";
                                                th++;
                                            }
                                        }
                                    }
                                    break;
                                }
                            case DayOfWeek.Friday:
                                {
                                    if (RecProp.IsWeeklyFriday && addDate.Date <= endDate.Date)
                                    {
                                        if (recCount != 0)
                                        {
                                            tempTimeSpan = addDate - prevDate;
                                            if (tempTimeSpan <= diffTimeSpan)
                                            {
                                                isValidRecurrence = false;
                                            }
                                            else
                                            {
                                                prevDate = addDate;
                                                recCount++;
                                                if (fr == 0)
                                                {
                                                    byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",FR," : byDay + "FR,";
                                                    fr++;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            prevDate = addDate;
                                            recCount++;
                                            if (fr == 0)
                                            {
                                                byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",FR," : byDay + "FR,";
                                                fr++;
                                            }
                                        }
                                    }
                                    break;
                                }
                            case DayOfWeek.Saturday:
                                {
                                    if (RecProp.IsWeeklySaturday && addDate.Date <= endDate.Date)
                                    {
                                        if (recCount != 0)
                                        {
                                            tempTimeSpan = addDate - prevDate;
                                            if (tempTimeSpan <= diffTimeSpan)
                                            {
                                                isValidRecurrence = false;
                                            }
                                            else
                                            {
                                                prevDate = addDate;
                                                recCount++;
                                                if (sa == 0)
                                                {
                                                    byDay = byDay + "SA";
                                                    sa++;
                                                }

                                            }
                                        }
                                        else
                                        {
                                            prevDate = addDate;
                                            recCount++;
                                            if (sa == 0)
                                            {
                                                byDay = byDay + "SA";
                                                sa++;
                                            }
                                        }
                                    }

                                    break;
                                }
                        }
                        addDate = addDate.DayOfWeek == DayOfWeek.Saturday ? addDate.AddDays(((WyWeekGap - 1) * 7) + 1) : addDate.AddDays(1);
                    }
                    RRule = RRule + ";COUNT=" + recCount;
                    if (WyWeekGap > 1)
                    {
                        RRule = RRule + ";INTERVAL=" + WyWeekGap;
                    }
                    if (byDay != "" && byDay != string.Empty)
                    {
                        RRule = RRule + ";BYDAY=" + byDay;
                    }
                    else
                    {
                        isValidRecurrence = false;
                    }
                }

                #endregion

                #region monthly

                else if (RecProp.RecurrenceType == RecurrenceType.Monthly)
                {
                    RRule = "FREQ=MONTHLY";

                    if (RecProp.IsMonthlySpecific)
                    {
                        addDate = startDate;
                        MyMonthGap = RecProp.MonthlyEveryNMonths;
                        int monthDate = RecProp.MonthlySpecificMonthDay;
                        int currDate = int.Parse(startDate.Day.ToString());
                        int diffDate = monthDate - currDate;
                        addDate = addDate.AddDays(diffDate);

                        if (monthDate < currDate)
                        {
                            addDate = addDate.AddMonths(1);
                        }

                        while (addDate.Date <= endDate.Date)
                        {
                            if (recCount != 0)
                            {
                                tempTimeSpan = addDate - prevDate;
                                if (tempTimeSpan >= diffTimeSpan)
                                {
                                    prevDate = addDate;
                                    recCount++;
                                }
                                else
                                {
                                    isValidRecurrence = false;
                                    break;
                                }
                            }
                            else
                            {
                                prevDate = addDate;
                                recCount++;
                            }

                            addDate = addDate.AddMonths(MyMonthGap);
                        }
                        RRule = RRule + ";COUNT=" + recCount;
                        if (MyMonthGap > 1)
                        {
                            RRule = RRule + ";INTERVAL=" + MyMonthGap;
                        }
                        //FREQ=MONTHLY;COUNT=6;INTERVAL=2;BYMONTHDAY=3
                        RRule = RRule + ";BYMONTHDAY=" + (RecProp.MonthlySpecificMonthDay);
                    }

                    else
                    {
                        addDate = startDate;
                        MyMonthGap = RecProp.MonthlyEveryNMonths;
                        while (addDate.Date <= endDate.Date)
                        {
                            var monthStart = new DateTime(addDate.Year, addDate.Month, 1);
                            DateTime weekStartDate = monthStart.AddDays(-(int)(monthStart.DayOfWeek));
                            var monthStartWeekday = (int)(monthStart.DayOfWeek);
                            int nthweekDay = RecProp.MonthlyWeekDay;
                            int nthWeek;
                            if (monthStartWeekday <= nthweekDay)
                            {
                                nthWeek = RecProp.MonthlyNthWeek - 1;
                            }
                            else
                            {
                                nthWeek = RecProp.MonthlyNthWeek;
                            }
                            addDate = weekStartDate.AddDays((nthWeek) * 7);
                            addDate = addDate.AddDays(nthweekDay);
                            if (addDate.CompareTo(startDate) < 0)
                            {
                                addDate = addDate.AddMonths(1);
                                continue;
                            }

                            if (recCount != 0)
                            {
                                tempTimeSpan = addDate - prevDate;
                                if (tempTimeSpan >= diffTimeSpan)
                                {
                                    prevDate = addDate;
                                    recCount++;
                                }
                                else
                                {
                                    isValidRecurrence = false;
                                    break;
                                }
                            }
                            else
                            {
                                prevDate = addDate;
                                recCount++;
                            }
                            addDate = addDate.AddMonths(MyMonthGap);
                        }
                        RRule = RRule + ";COUNT=" + recCount;
                        if (MyMonthGap > 1)
                        {
                            RRule = RRule + ";INTERVAL=" + MyMonthGap;
                        }
                        //FREQ=MONTHLY;COUNT=6;INTERVAL=2;BYDAY=WE;BYSETPOS=1
                        RRule = RRule + ";BYDAY=" + (CultureInfo.CurrentCulture.DateTimeFormat.ShortestDayNames[RecProp.MonthlyWeekDay - 1]).ToUpper() + ";BYSETPOS=" + RecProp.MonthlyNthWeek;
                    }
                }
                #endregion

                #region Yearly
                else if (RecProp.RecurrenceType == RecurrenceType.Yearly)
                {

                    YyYearGap = RecProp.YearlyEveryNYears;
                    RRule = "FREQ=YEARLY";
                    addDate = startDate;
                    if (RecProp.IsYearlySpecific)
                    {
                        int monthIndex = RecProp.YearlySpecificMonth;
                        int dayIndex = RecProp.YearlySpecificMonthDay;
                        int daysInMonth = DateTime.DaysInMonth(addDate.Year, monthIndex);
                        if (dayIndex <= daysInMonth)
                        {
                            var specificDate = new DateTime(addDate.Year, monthIndex, dayIndex);
                            if (specificDate.Date < addDate.Date)
                            {
                                addDate = specificDate;
                                addDate = addDate.AddYears(1);
                            }
                            else
                            {
                                addDate = specificDate;
                            }
                            while (addDate.Date <= endDate.Date)
                            {

                                if (recCount != 0)
                                {
                                    tempTimeSpan = addDate - prevDate;
                                    if (tempTimeSpan >= diffTimeSpan)
                                    {
                                        prevDate = addDate;
                                        recCount++;
                                    }
                                    else
                                    {
                                        isValidRecurrence = false;
                                        break;
                                    }
                                }
                                else
                                {
                                    prevDate = addDate;
                                    recCount++;
                                }
                                addDate = addDate.AddYears(YyYearGap);
                            }
                        }
                        RRule = RRule + ";COUNT=" + recCount;
                        if (YyYearGap > 1)
                        {
                            RRule = RRule + ";INTERVAL=" + YyYearGap;
                        }
                        //FREQ=YEARLY;COUNT=6;INTERVAL=2;BYMONTHDAY=3;BYMONTH=4
                        RRule = RRule + ";BYMONTHDAY=" + (RecProp.YearlySpecificMonthDay) + ";BYMONTH=" + (RecProp.YearlySpecificMonth);
                    }

                    else
                    {
                        addDate = startDate;
                        int monthIndex = RecProp.YearlyGenericMonth;
                        while (addDate.Date <= endDate.Date)
                        {
                            var monthStart = new DateTime(addDate.Year, monthIndex, 1);
                            DateTime weekStartDate = monthStart.AddDays(-(int)(monthStart.DayOfWeek));
                            var monthStartWeekday = (int)(monthStart.DayOfWeek);
                            int nthweekDay = RecProp.YearlyWeekDay;
                            int nthWeek;
                            if (monthStartWeekday <= nthweekDay)
                            {
                                nthWeek = RecProp.YearlyNthWeek - 1;
                            }
                            else
                            {
                                nthWeek = RecProp.YearlyNthWeek;
                            }
                            addDate = weekStartDate.AddDays((nthWeek) * 7);
                            addDate = addDate.AddDays(nthweekDay);
                            if (addDate.CompareTo(startDate) < 0)
                            {
                                addDate = addDate.AddYears(1);
                                continue;
                            }
                            if (recCount != 0)
                            {
                                tempTimeSpan = addDate - prevDate;
                                if (tempTimeSpan >= diffTimeSpan)
                                {
                                    prevDate = addDate;
                                    recCount++;
                                }
                                else
                                {
                                    isValidRecurrence = false;
                                    break;
                                }
                            }
                            else
                            {
                                prevDate = addDate;
                                recCount++;
                            }
                            addDate = addDate.AddYears(YyYearGap);
                        }
                        RRule = RRule + ";COUNT=" + recCount;
                        if (YyYearGap > 1)
                        {
                            RRule = RRule + ";INTERVAL=" + YyYearGap;
                        }
                        RRule = RRule + ";BYDAY=" + (CultureInfo.CurrentCulture.DateTimeFormat.ShortestDayNames[RecProp.YearlyWeekDay - 1]).ToUpper() + ";BYMONTH=" + (RecProp.YearlyGenericMonth) + ";BYSETPOS=" + RecProp.YearlyNthWeek;
                    }
                }
                #endregion
            }
            else if (RecProp.IsRangeNoEndDate)
            {
                addDate = startDate;

                #region Daily
                if (RecProp.RecurrenceType == RecurrenceType.Daily)
                {
                    RRule = "FREQ=DAILY";
                    if (RecProp.IsDailyEveryNDays)
                    {
                        DyDayGap = RecProp.DailyNDays;
                        if (diffTimeSpan.TotalHours >= DyDayGap * 24)
                        {
                            isValidRecurrence = false;
                        }
                        if (DyDayGap > 1)
                        {
                            //FREQ=DAILY;COUNT=5;INTERVAL=2
                            RRule = RRule + ";INTERVAL=" + DyDayGap;
                        }
                    }
                    else
                    {
                        if (diffTimeSpan.TotalHours > 24)
                        {
                            isValidRecurrence = false;
                        }
                        //FREQ=WEEKLY;COUNT=5;BYDAY=MO,TU,WE,TH,FR
                        RRule = RRule + ";BYDAY=MO,TU,WE,TH,FR";
                    }
                }
                #endregion

                #region Weekly
                else if (RecProp.RecurrenceType == RecurrenceType.Weekly)
                {
                    //FREQ=WEEKLY;COUNT=6;INTERVAL=2;BYDAY=SU,MO,WE,FR;WKST=SU
                    RRule = "FREQ=WEEKLY";
                    string byDay = "";
                    int su = 0, mo = 0, tu = 0, we = 0, th = 0, fr = 0, sa = 0;
                    WyWeekGap = RecProp.WeeklyEveryNWeeks;
                    if (WyWeekGap > 1)
                    {
                        //FREQ=DAILY;COUNT=5;INTERVAL=2
                        RRule = RRule + ";INTERVAL=" + WyWeekGap;
                    }
                    RRule = RRule + ";BYDAY=";
                    int count = 0;
                    bool isweeklyday_selected = RecProp.IsWeeklyFriday || RecProp.IsWeeklyMonday || RecProp.IsWeeklySaturday || RecProp.IsWeeklySunday || RecProp.IsWeeklyThursday || RecProp.IsWeeklyTuesday || RecProp.IsWeeklyWednesday;
                    while (count < recCount && isValidRecurrence && isweeklyday_selected)
                    {
                        switch (addDate.DayOfWeek)
                        {
                            case DayOfWeek.Sunday:
                                {
                                    if (RecProp.IsWeeklySunday && count < recCount)
                                    {
                                        if (count != 0)
                                        {
                                            tempTimeSpan = addDate - prevDate;
                                            if (tempTimeSpan <= diffTimeSpan)
                                            {
                                                isValidRecurrence = false;
                                            }
                                            else
                                            {
                                                prevDate = addDate;
                                                if (su == 1)
                                                {
                                                    count = recCount;
                                                    break;
                                                }
                                                byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",SU," : byDay + "SU,";
                                                su++;
                                            }
                                        }
                                        else
                                        {
                                            prevDate = addDate;
                                            count++;
                                            byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",SU," : byDay + "SU,";
                                            su++;
                                        }
                                    }
                                    break;
                                }
                            case DayOfWeek.Monday:
                                {
                                    if (RecProp.IsWeeklyMonday && count < recCount)
                                    {
                                        if (count != 0)
                                        {
                                            tempTimeSpan = addDate - prevDate;
                                            if (tempTimeSpan <= diffTimeSpan)
                                            {
                                                isValidRecurrence = false;
                                            }
                                            else
                                            {
                                                prevDate = addDate;
                                                if (mo == 1)
                                                {
                                                    count = recCount;
                                                    break;
                                                }
                                                byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",MO," : byDay + "MO,";
                                                mo++;
                                            }
                                        }
                                        else
                                        {
                                            prevDate = addDate;
                                            count++;
                                            byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",MO," : byDay + "MO,";
                                            mo++;
                                        }
                                    }
                                    break;
                                }
                            case DayOfWeek.Tuesday:
                                {
                                    if (RecProp.IsWeeklyTuesday && count < recCount)
                                    {
                                        if (count != 0)
                                        {
                                            tempTimeSpan = addDate - prevDate;
                                            if (tempTimeSpan <= diffTimeSpan)
                                            {
                                                isValidRecurrence = false;
                                            }
                                            else
                                            {
                                                prevDate = addDate;
                                                if (tu == 1)
                                                {
                                                    count = recCount;
                                                    break;
                                                }
                                                byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",TU," : byDay + "TU,";
                                                tu++;
                                            }
                                        }
                                        else
                                        {
                                            prevDate = addDate;
                                            count++;
                                            byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",TU," : byDay + "TU,";
                                            tu++;
                                        }
                                    }
                                    break;
                                }
                            case DayOfWeek.Wednesday:
                                {
                                    if (RecProp.IsWeeklyWednesday && count < recCount)
                                    {
                                        if (count != 0)
                                        {
                                            tempTimeSpan = addDate - prevDate;
                                            if (tempTimeSpan <= diffTimeSpan)
                                            {
                                                isValidRecurrence = false;
                                            }
                                            else
                                            {
                                                prevDate = addDate;
                                                if (we == 1)
                                                {
                                                    count = recCount;
                                                    break;
                                                }
                                                byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",WE," : byDay + "WE,";
                                                we++;
                                            }
                                        }
                                        else
                                        {
                                            prevDate = addDate;
                                            count++;
                                            byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",WE," : byDay + "WE,";
                                            we++;
                                        }
                                    }
                                    break;
                                }
                            case DayOfWeek.Thursday:
                                {
                                    if (RecProp.IsWeeklyThursday && count < recCount)
                                    {
                                        if (count != 0)
                                        {
                                            tempTimeSpan = addDate - prevDate;
                                            if (tempTimeSpan <= diffTimeSpan)
                                            {
                                                isValidRecurrence = false;
                                            }
                                            else
                                            {
                                                prevDate = addDate;
                                                if (th == 1)
                                                {
                                                    count = recCount;
                                                    break;
                                                }
                                                byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",TH," : byDay + "TH,";
                                                th++;
                                            }
                                        }
                                        else
                                        {
                                            prevDate = addDate;
                                            count++;
                                            byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",TH," : byDay + "TH,";
                                            th++;
                                        }
                                    }
                                    break;
                                }
                            case DayOfWeek.Friday:
                                {
                                    if (RecProp.IsWeeklyFriday && count < recCount)
                                    {
                                        if (count != 0)
                                        {
                                            tempTimeSpan = addDate - prevDate;
                                            if (tempTimeSpan <= diffTimeSpan)
                                            {
                                                isValidRecurrence = false;
                                            }
                                            else
                                            {
                                                prevDate = addDate;
                                                if (fr == 1)
                                                {
                                                    count = recCount;
                                                    break;
                                                }
                                                byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",FR," : byDay + "FR,";
                                                fr++;
                                            }
                                        }
                                        else
                                        {
                                            prevDate = addDate;
                                            count++;
                                            byDay = byDay.Length != 0 && byDay.Substring(byDay.Length - 1) == "A" ? byDay + ",FR," : byDay + "FR,";
                                            fr++;
                                        }
                                    }
                                    break;
                                }
                            case DayOfWeek.Saturday:
                                {
                                    if (RecProp.IsWeeklySaturday && count < recCount)
                                    {
                                        if (count != 0)
                                        {
                                            tempTimeSpan = addDate - prevDate;
                                            if (tempTimeSpan <= diffTimeSpan)
                                            {
                                                isValidRecurrence = false;
                                            }
                                            else
                                            {
                                                prevDate = addDate;
                                                if (sa == 1)
                                                {
                                                    count = recCount;
                                                    break;
                                                }
                                                byDay = byDay + "SA";
                                                sa++;
                                            }
                                        }
                                        else
                                        {
                                            prevDate = addDate;
                                            count++;
                                            byDay = byDay + "SA";
                                            sa++;
                                        }
                                    }
                                    break;
                                }
                        }
                        addDate = addDate.DayOfWeek == DayOfWeek.Saturday ? addDate.AddDays(((WyWeekGap - 1) * 7) + 1) : addDate.AddDays(1);
                    }
                    RRule = RRule + byDay;
                }
                #endregion

                #region Monthly
                else if (RecProp.RecurrenceType == RecurrenceType.Monthly)
                {

                    RRule = "FREQ=MONTHLY";
                    int count = 0;
                    MyMonthGap = RecProp.MonthlyEveryNMonths;
                    if (MyMonthGap > 1)
                    {
                        RRule = RRule + ";INTERVAL=" + MyMonthGap;
                    }


                    if (RecProp.IsMonthlySpecific)
                    {
                        //FREQ=MONTHLY;COUNT=6;INTERVAL=2;BYMONTHDAY=3
                        RRule = RRule + ";BYMONTHDAY=" + (RecProp.MonthlySpecificMonthDay);
                        int monthDate = RecProp.MonthlySpecificMonthDay;
                        int currDate = int.Parse(startDate.Day.ToString());
                        int diffDate = monthDate - currDate;
                        addDate = addDate.AddDays(diffDate);
                        if (monthDate < currDate)
                        {
                            addDate = addDate.AddMonths(1);
                        }
                        for (int i = 0; i < recCount; i++)
                        {
                            if (count != 0)
                            {
                                tempTimeSpan = addDate - prevDate;
                                if (tempTimeSpan >= diffTimeSpan)
                                {
                                    prevDate = addDate;
                                    count++;
                                }
                                else
                                {
                                    isValidRecurrence = false;
                                    break;
                                }
                            }
                            else
                            {
                                prevDate = addDate;
                                count++;
                            }
                            //RecDateCollection.Add(addDate.Date);
                            addDate = addDate.AddMonths(MyMonthGap);
                        }
                    }

                    else
                    {
                        //FREQ=MONTHLY;COUNT=6;INTERVAL=2;BYDAY=WE;BYSETPOS=1
                        RRule = RRule + ";BYDAY=" + RecProp.MonthlyWeekDay + ";BYSETPOS=" + RecProp.MonthlyNthWeek;

                        //MyMonthGap = RecProp.MonthlyEveryNMonths;
                        while (count < recCount)
                        {
                            var monthStart = new DateTime(addDate.Year, addDate.Month, 1);
                            var weekStartDate = monthStart.AddDays(-(int)(monthStart.DayOfWeek));
                            var monthStartWeekday = (int)(monthStart.DayOfWeek);
                            int nthweekDay = RecProp.MonthlyWeekDay;
                            int nthWeek;
                            if (monthStartWeekday <= nthweekDay)
                            {
                                nthWeek = RecProp.MonthlyNthWeek - 1;
                            }
                            else
                            {
                                nthWeek = RecProp.MonthlyNthWeek;
                            }
                            addDate = weekStartDate.AddDays((nthWeek) * 7);
                            addDate = addDate.AddDays(nthweekDay);
                            if (addDate.CompareTo(startDate) < 0)
                            {
                                addDate = addDate.AddMonths(1);
                                continue;
                            }

                            if (count != 0)
                            {
                                tempTimeSpan = addDate - prevDate;
                                if (tempTimeSpan >= diffTimeSpan)
                                {
                                    prevDate = addDate;
                                    count++;
                                }
                                else
                                {
                                    isValidRecurrence = false;
                                    break;
                                }
                            }
                            else
                            {
                                prevDate = addDate;
                                count++;
                            }
                            addDate = addDate.AddMonths(MyMonthGap);
                        }
                    }

                }
                #endregion

                #region Yearly

                else if (RecProp.RecurrenceType == RecurrenceType.Yearly)
                {

                    RRule = "FREQ=YEARLY";
                    int count = 0;
                    YyYearGap = RecProp.YearlyEveryNYears;
                    if (YyYearGap > 1)
                    {
                        RRule = RRule + ";INTERVAL=" + YyYearGap;
                    }

                    if (RecProp.IsYearlySpecific)
                    {
                        RRule = RRule + ";BYMONTHDAY=" + (RecProp.YearlySpecificMonthDay) + ";BYMONTH=" + (RecProp.YearlySpecificMonth);
                        int monthIndex = RecProp.YearlySpecificMonth;
                        int dayIndex = RecProp.YearlySpecificMonthDay;
                        int bound = DateTime.DaysInMonth(addDate.Year, monthIndex);
                        if (bound >= dayIndex)
                        {
                            var specificDate = new DateTime(addDate.Year, monthIndex, dayIndex);
                            if (specificDate.Date < addDate.Date)
                            {
                                addDate = specificDate;
                                addDate = addDate.AddYears(1);

                            }
                            else
                            {
                                addDate = specificDate;
                            }

                            for (int i = 0; i < recCount; i++)
                            {
                                if (count != 0)
                                {
                                    tempTimeSpan = addDate - prevDate;
                                    if (tempTimeSpan >= diffTimeSpan)
                                    {
                                        prevDate = addDate;
                                        count++;
                                    }
                                    else
                                    {
                                        isValidRecurrence = false;
                                        break;
                                    }
                                }
                                else
                                {
                                    prevDate = addDate;
                                    count++;
                                }
                                addDate = addDate.AddYears(YyYearGap);
                            }
                        }
                    }
                    else
                    {
                        //FREQ=YEARLY;COUNT=6;INTERVAL=2;BYDAY=WE;BYMONTH=4;BYSETPOS=1
                        RRule = RRule + ";BYDAY=" + (CultureInfo.CurrentCulture.DateTimeFormat.ShortestDayNames[RecProp.YearlyWeekDay - 1]).ToUpper() + ";BYMONTH=" + (RecProp.YearlyGenericMonth) + ";BYSETPOS=" + RecProp.YearlyNthWeek;
                        int monthIndex = RecProp.YearlyGenericMonth;
                        while (count < recCount)
                        {
                            var monthStart = new DateTime(addDate.Year, monthIndex, 1);
                            DateTime weekStartDate = monthStart.AddDays(-(int)(monthStart.DayOfWeek));
                            var monthStartWeekday = (int)(monthStart.DayOfWeek);
                            int nthweekDay = RecProp.YearlyWeekDay;
                            int nthWeek;
                            if (monthStartWeekday <= nthweekDay)
                            {
                                nthWeek = RecProp.YearlyNthWeek - 1;
                            }
                            else
                            {
                                nthWeek = RecProp.YearlyNthWeek;
                            }
                            addDate = weekStartDate.AddDays((nthWeek) * 7);
                            addDate = addDate.AddDays(nthweekDay);
                            if (addDate.CompareTo(startDate) < 0)
                            {
                                addDate = addDate.AddYears(1);
                                continue;
                            }

                            if (count != 0)
                            {
                                tempTimeSpan = addDate - prevDate;
                                if (tempTimeSpan >= diffTimeSpan)
                                {
                                    prevDate = addDate;
                                    count++;
                                }
                                else
                                {
                                    isValidRecurrence = false;
                                    break;
                                }
                            }
                            else
                            {
                                prevDate = addDate;
                                count++;
                            }
                            addDate = addDate.AddYears(YyYearGap);

                        }
                    }

                }
                #endregion
            }
            if (isValidRecurrence)
            {
                return RRule;
            }
            return string.Empty;
        }

        public static RecurrenceProperties RRuleParser(string RRule, DateTime RecStartDate)
        {
            var RecProp = new RecurrenceProperties();

            DateTime startDate = RecStartDate;
            RecProp.RangeStartDate = RecStartDate;
            var ruleSeperator = new[] { '=', ';', ',' };
            var weeklySeperator = new[] { ';' };
            string[] ruleArray = RRule.Split(ruleSeperator);
            string[] weeklyRule = RRule.Split(weeklySeperator);
            int recCount = 0;
            if (ruleArray.Length != 0 && RRule != "")
            {
                DateTime addDate = startDate;
                if (!(RRule.Contains("COUNT")))
                {
                    RecProp.IsRangeNoEndDate = true;
                    RecProp.IsRangeRecurrenceCount = false;
                    RecProp.IsRangeEndDate = false;
                }
                else
                {
                    recCount = int.Parse(ruleArray[3]);
                    RecProp.IsRangeRecurrenceCount = true;
                    RecProp.IsRangeNoEndDate = false;
                    RecProp.IsRangeEndDate = false;
                    RecProp.RangeRecurrenceCount = recCount;
                }

                RecProp.RangeStartDate = RecStartDate;

                #region DAILY
                if (ruleArray[1] == "DAILY")
                {
                    RecProp.RecurrenceType = RecurrenceType.Daily;
                    if (!(RRule.Contains("COUNT")))
                    {
                        if (RRule.Contains("INTERVAL"))
                        {
                            RecProp.DailyNDays = int.Parse(ruleArray[3]);
                        }
                        else if (RRule.Contains("BYDAY"))
                        {
                            RecProp.IsDailyEveryNDays = false;
                        }
                        else
                        {
                            RecProp.DailyNDays = 1;
                        }
                    }
                    else
                    {
                        if ((ruleArray.Length > 4 && ruleArray[4] == "INTERVAL") || ruleArray.Length == 4)
                        {
                            RecProp.DailyNDays = ruleArray.Length == 4 ? 1 : int.Parse(ruleArray[5]);
                            RecProp.IsDailyEveryNDays = true;
                        }
                        else if (ruleArray.Length > 4 && ruleArray[4] == "BYDAY")
                        {
                            RecProp.IsDailyEveryNDays = false;
                        }
                    }
                }
                #endregion

                #region WEEKLY
                else if (ruleArray[1] == "WEEKLY")
                {
                    RecProp.RecurrenceType = RecurrenceType.Weekly;
                    RecProp.WeeklyEveryNWeeks = ruleArray.Length > 4 && ruleArray[4] == "INTERVAL" ? int.Parse(ruleArray[5]) : 1;
                    int WyWeekGap = RecProp.WeeklyEveryNWeeks;
                    int position = ruleArray.Length > 4 && ruleArray[4] == "INTERVAL" ? 3 : 2;
                    int i = 0;
                    if (RecProp.IsRangeNoEndDate)
                    {
                        recCount = 7;
                        position = 1;
                    }
                    RecProp.IsWeeklySunday = false;
                    while (i < recCount)
                    {
                        switch (addDate.DayOfWeek)
                        {
                            case DayOfWeek.Sunday:
                                {
                                    if (weeklyRule[position].Contains("SU"))
                                    {
                                        RecProp.IsWeeklySunday = true;
                                    }
                                    break;
                                }
                            case DayOfWeek.Monday:
                                {
                                    if (weeklyRule[position].Contains("MO"))
                                    {
                                        RecProp.IsWeeklyMonday = true;
                                    }
                                    break;
                                }
                            case DayOfWeek.Tuesday:
                                {
                                    if (weeklyRule[position].Contains("TU"))
                                    {
                                        RecProp.IsWeeklyTuesday = true;
                                    }
                                    break;
                                }
                            case DayOfWeek.Wednesday:
                                {
                                    if (weeklyRule[position].Contains("WE"))
                                    {
                                        RecProp.IsWeeklyWednesday = true;
                                    }
                                    break;
                                }
                            case DayOfWeek.Thursday:
                                {
                                    if (weeklyRule[position].Contains("TH"))
                                    {
                                        RecProp.IsWeeklyThursday = true;
                                    }
                                    break;
                                }
                            case DayOfWeek.Friday:
                                {
                                    if (weeklyRule[position].Contains("FR"))
                                    {
                                        RecProp.IsWeeklyFriday = true;
                                    }
                                    break;
                                }
                            case DayOfWeek.Saturday:
                                {
                                    if (weeklyRule[position].Contains("SA"))
                                    {
                                        RecProp.IsWeeklySaturday = true;
                                    }
                                    break;
                                }
                        }
                        addDate = addDate.DayOfWeek == DayOfWeek.Saturday ? addDate.AddDays(((WyWeekGap - 1) * 7) + 1) : addDate.AddDays(1);
                        i = i + 1;
                    }
                }
                #endregion

                #region MONTHLY
                else if (ruleArray[1] == "MONTHLY")
                {
                    RecProp.RecurrenceType = RecurrenceType.Monthly;
                    if (!(RRule.Contains("COUNT")))
                    {
                        if (RRule.Contains("BYMONTHDAY"))
                        {
                            RecProp.IsMonthlySpecific = true;
                            RecProp.MonthlySpecificMonthDay = int.Parse(ruleArray[3]);
                        }
                        else if (RRule.Contains("BYDAY"))
                        {
                            RecProp.IsMonthlySpecific = false;
                            RecProp.MonthlyNthWeek = int.Parse(ruleArray[5]);
                            RecProp.MonthlyWeekDay = GetWeekDay(ruleArray[3]);
                        }
                    }
                    else
                    {
                        RecProp.MonthlyEveryNMonths = ruleArray.Length > 4 && ruleArray[4] == "INTERVAL" ? int.Parse(ruleArray[5]) : 1;
                        int position = ruleArray.Length > 4 && ruleArray[4] == "INTERVAL" ? 6 : 4;
                        position = ruleArray.Length == 4 ? 2 : position;
                        if (ruleArray[position] == "BYMONTHDAY")
                        {
                            RecProp.IsMonthlySpecific = true;
                            RecProp.MonthlySpecificMonthDay = int.Parse(ruleArray[position + 1]);
                        }
                        else if (ruleArray[position] == "BYDAY")
                        {
                            RecProp.IsMonthlySpecific = false;
                            RecProp.MonthlyNthWeek = int.Parse(ruleArray[position + 3]);
                            RecProp.MonthlyWeekDay = GetWeekDay(ruleArray[position + 1]);
                        }
                    }
                }
                #endregion

                #region YEARLY
                else if (ruleArray[1] == "YEARLY")
                {
                    RecProp.RecurrenceType = RecurrenceType.Yearly;
                    if (!(RRule.Contains("COUNT")))
                    {
                        if (RRule.Contains("BYMONTHDAY"))
                        {
                            RecProp.IsYearlySpecific = true;
                            RecProp.YearlySpecificMonth = int.Parse(ruleArray[5]);
                            RecProp.YearlySpecificMonthDay = int.Parse(ruleArray[3]);
                        }
                        else if (RRule.Contains("BYDAY"))
                        {
                            RecProp.IsYearlySpecific = false;
                            RecProp.YearlyGenericMonth = int.Parse(ruleArray[5]);
                            RecProp.YearlyNthWeek = int.Parse(ruleArray[7]);
                            RecProp.YearlyWeekDay = GetWeekDay(ruleArray[3]);
                        }
                    }
                    else
                    {
                        RecProp.YearlyEveryNYears = ruleArray.Length > 4 && ruleArray[4] == "INTERVAL" ? int.Parse(ruleArray[5]) : 1;
                        int position = ruleArray.Length > 4 && ruleArray[4] == "INTERVAL" ? 6 : 4;
                        if (ruleArray[position] == "BYMONTHDAY")
                        {
                            RecProp.IsYearlySpecific = true;
                            RecProp.YearlySpecificMonth = int.Parse(ruleArray[position + 3]);
                            RecProp.YearlySpecificMonthDay = int.Parse(ruleArray[position + 1]);
                        }
                        else if (ruleArray[position] == "BYDAY")
                        {
                            RecProp.IsYearlySpecific = false;
                            RecProp.YearlyGenericMonth = int.Parse(ruleArray[position + 3]);
                            RecProp.YearlyNthWeek = int.Parse(ruleArray[position + 5]);
                            RecProp.YearlyWeekDay = GetWeekDay(ruleArray[position + 1]);
                        }
                    }
                }
                #endregion
            }



            return RecProp;
        }

        internal static void Import(String fileContent, SfSchedule schedule)
        {
            fileContent = fileContent.Replace("\r", " ");
            char[] newline = { '\n', ':' };
            char[] newlinetZone = { '\n' };
            char[] timezon = { '"', '=' };
            string fileContentCopy = fileContent.Replace("\r", " ");
            if (fileContentCopy.Contains("BEGIN:VTIMEZONE"))
            {
                string timeZone = string.Empty;
                string tZoneContainer = fileContentCopy.Substring(fileContentCopy.IndexOf("BEGIN:VTIMEZONE", StringComparison.Ordinal),
                                        (fileContentCopy.IndexOf("END:VTIMEZONE", StringComparison.Ordinal) + 13 -
                                        (fileContentCopy.IndexOf("BEGIN:VTIMEZONE", StringComparison.Ordinal))));
                string[] propertiesTZone = tZoneContainer.Split(newlinetZone);
                foreach (string prop in propertiesTZone)
                {
                    if (prop.Contains("TZID"))
                    {
                        timeZone = prop.Substring(5);
                        timeZone = timeZone.Trim();
                        break;
                    }
                }
                foreach (var item in ICalTimeZones.Values)
                {
                    var list = item;
                    if (list.Contains(timeZone))
                    {
                        string result = list[1];
                        fileContentCopy = fileContentCopy.Replace(timeZone, result);
                        fileContentCopy = fileContentCopy.Replace("BEGIN:VTIMEZONE", "BEGIN:VTIMEZON_");
                        break;
                    }
                }

            }
            while (fileContentCopy.Contains("BEGIN:VEVENT"))
            {
                var schedule_appointment = new ScheduleAppointment();
                var End = new DateTime();
                string currentEvent = fileContentCopy.Substring(fileContentCopy.IndexOf("BEGIN:VEVENT", StringComparison.Ordinal),
                                        (fileContentCopy.IndexOf("END:VEVENT", StringComparison.Ordinal) + 10 -
                                        (fileContentCopy.IndexOf("BEGIN:VEVENT", StringComparison.Ordinal))));
                string[] properties = currentEvent.Split(newline);

                if (!currentEvent.Contains("RRULE"))
                {
                    schedule_appointment.IsRecursive = false;
                }
                for (int i = 0; i < properties.Length; i++)
                {

                    if (properties[i] == "LOCATION")
                    {
                        string location = properties[i + 1];
                        schedule_appointment.Location = location;
                    }
                    else if (properties[i].StartsWith("SUMMARY"))
                    {
                        string subject = properties[i + 1];
                        schedule_appointment.Subject = subject;
                    }
                    else if (properties[i].StartsWith("DESCRIPTION"))
                    {
                        string notes = properties[i + 1];
                        if (schedule_appointment.Notes == string.Empty)
                        {
                            schedule_appointment.Notes = notes;
                        }
                    }
                    else if (properties[i].Contains("BUSYSTATUS"))
                    {
                        string status = properties[i + 1];
                        status = status.Trim();
                        if (status.Equals("Working Elsewhere"))
                        {
                            schedule_appointment.Status = schedule.AppointmentStatusCollection[4];
                        }
                        foreach (ScheduleAppointmentStatus appStatus in schedule.AppointmentStatusCollection)
                        {
                            if (status.Equals((appStatus.Status).ToUpper()))
                            {
                                schedule_appointment.Status = appStatus;
                                break;
                            }
                        }
                    }
                    else if (properties[i].StartsWith("DTEND"))
                    {
                        string endDate = properties[i + 1];

                        if (properties[i].Contains("TZID"))
                        {
                            //HAS TIMEZONE
                            string[] endTimeZone = properties[i].Split(timezon);
                            string endTimeZoneValue = endTimeZone.Length == 2 ? endTimeZone[1] : endTimeZone[2];
                            foreach (var item in ICalTimeZones.Values)
                            {
                                var list = item;
                                if (list.Contains(endTimeZoneValue))
                                {
                                    string result = list[0];
                                    schedule_appointment.EndTimeZone = new TimeZone { TimeZoneValue = result };
                                }
                            }
                            schedule_appointment.AllDay = false;
                            End = parseDateTime(endDate, false, true);
                        }
                        else
                        {
                            schedule_appointment.AllDay = true;
                            End = parseDateTime(endDate, true, true).AddDays(-1);
                        }
                    }
                    else if (properties[i].StartsWith("DTSTART"))
                    {
                        string startDate = properties[i + 1];
                        if (properties[i].Contains("TZID"))
                        {
                            //HAS TIMEZONE
                            string[] startTimeZone = properties[i].Split(timezon);
                            string startTimeZoneValue = startTimeZone.Length == 2 ? startTimeZone[1] : startTimeZone[2];
                            foreach (var item in ICalTimeZones.Values)
                            {
                                var list = item;
                                if (list.Contains(startTimeZoneValue))
                                {
                                    string result = list[0];
                                    schedule_appointment.StartTimeZone = new TimeZone { TimeZoneValue = result };
                                }
                            }
                            schedule_appointment.AllDay = false;
                            schedule_appointment.StartTime = parseDateTime(startDate, false, false);
                        }
                        else
                        {
                            schedule_appointment.AllDay = true;
                            schedule_appointment.StartTime = parseDateTime(startDate, true, false);
                        }
                    }
                    else if (properties[i].StartsWith("RRULE"))
                    {
                        schedule_appointment.IsRecursive = true;
                        if (!(currentEvent.Contains("COUNT")))
                        {

                        }
                        schedule_appointment.RecurrenceProperites = RRuleParser(properties[i + 1], schedule_appointment.StartTime.Date);
                    }
                    else if (properties[i].StartsWith("TRIGGER"))
                    {
                        if (properties[i + 5].Trim() == "Reminder")
                        {
                            string remind = properties[i + 1];
                            remind = remind.Trim();
                            string time = remind.Trim() == "PT0M" ? "0" : remind.Substring(3, remind.Length - 4);
                            int intTime = int.Parse(time);
                            int minutes = 0;
                            int hours = 0;
                            int days = 0;
                            int weeks = 0;
                            if (intTime >= 0)
                            {
                                minutes = intTime;
                                if (minutes >= 60)
                                {
                                    hours = minutes / 60;
                                    minutes = 0;
                                    if (hours >= 24)
                                    {
                                        days = hours / 24;
                                        hours = 0;
                                        if (days >= 7)
                                        {
                                            weeks = days / 7;
                                            days = 0;
                                        }
                                    }

                                }
                            }
                            schedule_appointment.ReminderTime = MinutesToReminder(minutes, hours, days, weeks);
                        }
                    }
                }
                schedule_appointment.EndTime = End;
                schedule.Appointments.Add(schedule_appointment);
                fileContentCopy = fileContentCopy.Replace(currentEvent, "");
            }
        }

        internal static string Export(SfSchedule schedule)
        {
            String contents = null;
            string timeZones = string.Empty;
            const string header = "BEGIN:VCALENDAR\nPRODID:-//Microsoft Corporation//Outlook 15.0 MIMEDIR//EN\nVERSION:2.0\nMETHOD:PUBLISH\nX-WR-CALNAME:" + "Schedule" + " Appointments\n\n";
            foreach (ScheduleAppointment SchAppointment in schedule.Appointments)
            {
                contents += "BEGIN:VEVENT\n";
                if (SchAppointment.Notes != string.Empty)
                {
                    contents += "DESCRIPTION:";
                    contents += SchAppointment.Notes + "\n";
                }
                if (SchAppointment.Location != string.Empty)
                {
                    contents += "LOCATION:";
                    contents += SchAppointment.Location + "\n";
                }
                if (SchAppointment.Subject != string.Empty)
                {
                    contents += "SUMMARY:";
                    contents += SchAppointment.Subject + "\n";
                }
                if (SchAppointment.AllDay)
                {
                    contents += "DTEND;VALUE=DATE:";
                    contents += SchAppointment.EndTime.ToString("yyyyMMdd") + "\n";
                    contents += "DTSTART;VALUE=DATE:";
                    contents += SchAppointment.StartTime.ToString("yyyyMMdd") + "\n";
                }
                else
                {
                    if (SchAppointment.AllDay)
                    {
                        contents += "DTEND;VALUE=DATE:";
                        string endtime = string.Format("{0:0000}{1:00}{2:00}T{3:00}{4:00}{5:00}", SchAppointment.EndTime.Year, SchAppointment.EndTime.Month, SchAppointment.EndTime.Day, SchAppointment.EndTime.TimeOfDay.Hours, SchAppointment.EndTime.TimeOfDay.Minutes, SchAppointment.EndTime.TimeOfDay.Seconds);
                        contents += endtime + "\n";
                        contents += "DTSTART;VALUE=DATE:";
                        string starttime = string.Format("{0:0000}{1:00}{2:00}T{3:00}{4:00}{5:00}", SchAppointment.StartTime.Year, SchAppointment.StartTime.Month, SchAppointment.StartTime.Day, SchAppointment.StartTime.TimeOfDay.Hours, SchAppointment.StartTime.TimeOfDay.Minutes, SchAppointment.StartTime.TimeOfDay.Seconds);
                        contents += starttime + "\n";
                    }
                    else
                    {
                        contents += "DTEND;TZID=";
                        //string[] startTimeZone = properties[i].Split(timezon);
                        string startTimeZoneValue = SchAppointment.StartTimeZone.TimeZoneValue;
                        string endTimeZoneValue = SchAppointment.EndTimeZone.TimeZoneValue;
                        var icalStartCode = string.Empty;
                        var icalStartTimeZone = string.Empty;
                        var icalEndCode = string.Empty;
                        var icalEndTimeZone = string.Empty;
                        foreach (var item in ICalTimeZones.Values)
                        {
                            var list = item;
                            if (list.Contains(startTimeZoneValue))
                            {
                                icalStartCode = list[2];
                                icalStartTimeZone = list[1];
                                break;
                            }
                        }
                        foreach (var item in ICalTimeZones.Values)
                        {
                            var list = item;
                            if (list.Contains(endTimeZoneValue))
                            {
                                icalEndCode = list[2];
                                icalEndTimeZone = list[1];
                                break;
                            }
                        }
                        if (!timeZones.Contains(icalStartCode))
                            timeZones += icalStartCode;
                        if (!timeZones.Contains(icalEndCode))
                            timeZones += icalEndCode;
                        contents += icalEndTimeZone + ":";
                        DateTime startTime = SchAppointment.StartTime;
                        DateTime endTime = SchAppointment.EndTime;
                        if (SchAppointment.IsRecursive)
                        {
                            if (schedule.RecursiveAddedDates.Count > 0 && schedule.RecursiveAddedDates.ContainsKey(SchAppointment.GetHashCode()))
                            {
                                ObservableCollection<DateTime> recursiveDates = schedule.RecursiveAddedDates[SchAppointment.GetHashCode()] as ObservableCollection<DateTime>;
                                if (recursiveDates.Count > 0)
                                {
                                    if (startTime < recursiveDates[0])
                                        startTime = recursiveDates[0];
                                    if (endTime < recursiveDates[0])
                                        endTime = recursiveDates[0];
                                }
                            }
                        }
                        string endtime = string.Format("{0:0000}{1:00}{2:00}T{3:00}{4:00}{5:00}", endTime.Year, endTime.Month, endTime.Day, SchAppointment.EndTime.TimeOfDay.Hours, SchAppointment.EndTime.TimeOfDay.Minutes, SchAppointment.EndTime.TimeOfDay.Seconds);
                        contents += endtime + "\n";
                        contents += "DTSTART;TZID=";
                        contents += icalStartTimeZone + ":";
                        string starttime = string.Format("{0:0000}{1:00}{2:00}T{3:00}{4:00}{5:00}", startTime.Year, startTime.Month, startTime.Day, SchAppointment.StartTime.TimeOfDay.Hours, SchAppointment.StartTime.TimeOfDay.Minutes, SchAppointment.StartTime.TimeOfDay.Seconds);
                        contents += starttime + "\n";
                    }
                }
                if (SchAppointment.IsRecursive)
                {
                    contents += "RRULE:";
                    contents += InternalRRuleGenerator(SchAppointment.RecurrenceProperites, SchAppointment.StartTime, SchAppointment.EndTime) + "\n";
                }
                if (SchAppointment.Status != null)
                {
                    contents += "X-MICROSOFT-CDO-BUSYSTATUS:";
                    if (SchAppointment.Status.Status == "Out Of Office")
                    {
                        contents += "OOF\n";
                    }
                    else if (SchAppointment.Status.Status == "Temporary")
                    {
                        contents += "WORKINGELSEWHERE\n";
                    }
                    else
                    {
                        contents += SchAppointment.Status.Status.ToUpper() + "\n";
                    }
                }
                if (SchAppointment.ReminderTime != ReminderTimeType.None)
                {
                    int minutes = ReminderToMinutes(SchAppointment.ReminderTime);
                    contents += "BEGIN:VALARM\nTRIGGER:-PT" + minutes + "M\n" + "ACTION:DISPLAY\nDESCRIPTION:Reminder\nEND:VALARM\n";
                }
                contents += "END:VEVENT\n\n";
            }
            const string footer = "END:VCALENDAR\n";
            string exportCode = header + timeZones + contents + footer;
            return exportCode;
        }

#if WPF
        internal static void Export(SfSchedule schedule, String fileName)
        {
            string directory = fileName.Remove(fileName.LastIndexOf("\\"));
            if (Directory.Exists(directory))
            {
                var sd = new SaveFileDialog { DefaultExt = ".ics", Filter = "ICalendar|*.ics" };

                sd.FileName = fileName;
                string contents = Export(schedule);

                var ascii = Encoding.Unicode;
                byte[] contents_byte = ascii.GetBytes(contents);

                using (Stream fs = (Stream)sd.OpenFile())
                {
                    foreach (byte content in contents_byte)
                    {
                        if (content != 0 && content != 34)
                            fs.WriteByte(content);
                    }
                    fs.Close();
                }
            }

        }
        
        internal static void Import(SfSchedule schedule, String fileName)
        {

            string Icsextension = new FileInfo(fileName).Extension.ToLower(CultureInfo.InvariantCulture);
            if (File.Exists(fileName))
                using (Stream stream = File.OpenRead(fileName))
                {
                    if (Icsextension == ".ics")
                    {
                        var opendialog = new OpenFileDialog { Multiselect = false, Filter = "ICalendar|*.ics" };

                        opendialog.FileName = fileName;
                        StreamReader reader = new StreamReader(opendialog.OpenFile());
                        while (!reader.EndOfStream)
                        {
                            string fileContent = string.Empty;
                            fileContent = reader.ReadToEnd();
                            Import(fileContent, schedule);
                        }
                    }
                }
        }

#endif

#if WINRT
        internal static async void ImportICS(SfSchedule schedule)
#else
        internal static void ImportICS(SfSchedule schedule)
#endif
        {
            string fileContent = string.Empty;
#if WINRT
            var fileOpenPicker = new FileOpenPicker { ViewMode = PickerViewMode.List };
            fileOpenPicker.FileTypeFilter.Add(".ics");
            StorageFile file = await fileOpenPicker.PickSingleFileAsync();

            if (file != null)
            {
                IBuffer buffer = await FileIO.ReadBufferAsync(file);
                using (DataReader dataReader = DataReader.FromBuffer(buffer))
                {
                    fileContent = dataReader.ReadString(buffer.Length);
                }
            }

#else
            var opendialog = new OpenFileDialog { Multiselect = false, Filter = "ICalendar|*.ics" };
            bool? dialogResult = opendialog.ShowDialog();

            if (dialogResult == true)
            {
#if SILVERLIGHT

                var reader = new StreamReader(opendialog.File.OpenRead());
#else
                var reader = new StreamReader(opendialog.OpenFile());
#endif
                fileContent = reader.ReadToEnd();
            }

#endif
            Import(fileContent, schedule);
        }

#if WINRT
        internal static async void ExportICS(SfSchedule schedule)
#else
        internal static void ExportICS(SfSchedule schedule)
#endif
        {

#if WINRT
            var fileSavePicker = new FileSavePicker();
            fileSavePicker.FileTypeChoices.Add("ics", new[] { ".ics" });
            fileSavePicker.CommitButtonText = "Export";
            fileSavePicker.SuggestedFileName = "Syncfusion Schedule";
            StorageFile file = await fileSavePicker.PickSaveFileAsync();

            if (file != null)
            {
                string exportCode = Export(schedule);
                await FileIO.WriteTextAsync(file, exportCode);

            }
#else
            var sd = new SaveFileDialog { DefaultExt = ".ics", Filter = "ICalendar|*.ics" };
            bool? dialogResult = sd.ShowDialog();

            if (dialogResult == true)
            {
                string contents = Export(schedule);

                var ascii = Encoding.Unicode;
                byte[] contents_byte = ascii.GetBytes(contents);

                using (Stream fs = sd.OpenFile())
                {
                    foreach (byte content in contents_byte)
                    {
                        if (content != 0 && content != 34)
                            fs.WriteByte(content);
                    }
                    fs.Close();
                }
            }
#endif
        }

        private static DateTime parseDateTime(String date, bool allday, bool enddate)
        {
            int day, month, year;
            DateTime ret_date;
            if (!allday)
            {
                year = Convert.ToInt32(date.Substring(0, 4));
                month = Convert.ToInt32(date.Substring(4, 2));
                day = Convert.ToInt32(date.Substring(6, 2));
                int hour = Convert.ToInt32(date.Substring(9, 2));
                int min = Convert.ToInt32(date.Substring(11, 2));
                int sec = Convert.ToInt32(date.Substring(13, 2));
                ret_date = new DateTime(year, month, day, hour, min, sec);
            }
            else
            {
                if (!enddate)
                {
                    year = Convert.ToInt32(date.Substring(0, 4));
                    month = Convert.ToInt32(date.Substring(4, 2));
                    day = Convert.ToInt32(date.Substring(6, 2));
                    ret_date = new DateTime(year, month, day, 0, 0, 0);
                }
                else
                {
                    year = Convert.ToInt32(date.Substring(0, 4));
                    month = Convert.ToInt32(date.Substring(4, 2));
                    day = Convert.ToInt32(date.Substring(6, 2));
                    ret_date = new DateTime(year, month, day, 0, 0, 0);
                }
            }

            return ret_date;
        }

        private static ReminderTimeType MinutesToReminder(int minutes, int hours, int days, int weeks)
        {
            if (minutes == 0 && hours == 0 && days == 0 && weeks == 0)
            {
                return ReminderTimeType.ZeroMin;
            }
            if (minutes != 0)
            {
                switch (minutes)
                {
                    case 5:
                        {
                            return ReminderTimeType.FiveMin;
                        }
                    case 10:
                        {
                            return ReminderTimeType.TenMin;
                        }
                    case 15:
                        {
                            return ReminderTimeType.FifteenMin;
                        }
                    case 30:
                        {
                            return ReminderTimeType.ThirtyMin;
                        }
                }
            }
            else if (hours != 0)
            {
                switch (hours)
                {
                    case 1:
                        {
                            return ReminderTimeType.OneHour;
                        }
                    case 2:
                        {
                            return ReminderTimeType.TwoHours;
                        }
                    case 3:
                        {
                            return ReminderTimeType.ThreeHours;
                        }
                    case 4:
                        {
                            return ReminderTimeType.FourHours;
                        }
                    case 5:
                        {
                            return ReminderTimeType.FiveHours;
                        }
                    case 6:
                        {
                            return ReminderTimeType.SixHours;
                        }
                    case 7:
                        {
                            return ReminderTimeType.SevenHours;
                        }
                    case 8:
                        {
                            return ReminderTimeType.EightHours;
                        }
                    case 9:
                        {
                            return ReminderTimeType.NineHours;
                        }
                    case 10:
                        {
                            return ReminderTimeType.TenHours;
                        }
                    case 11:
                        {
                            return ReminderTimeType.ElevenHours;
                        }
                    case 12:
                        {
                            return ReminderTimeType.HalfDay;
                        }
                    case 18:
                        {
                            return ReminderTimeType.EighteenHours;
                        }
                }
            }
            else if (days != 0)
            {
                switch (days)
                {
                    case 1:
                        {
                            return ReminderTimeType.OneDay;
                        }
                    case 2:
                        {
                            return ReminderTimeType.TwoDays;
                        }
                    case 3:
                        {
                            return ReminderTimeType.ThreeDays;
                        }
                    case 4:
                        {
                            return ReminderTimeType.FourDays;
                        }
                }
            }
            else if (weeks != 0)
            {
                switch (weeks)
                {
                    case 1:
                        {
                            return ReminderTimeType.OneWeek;
                        }
                    case 2:
                        {
                            return ReminderTimeType.TwoWeeks;
                        }
                }
            }
            return ReminderTimeType.None;
        }

        private static int ReminderToMinutes(ReminderTimeType reminder)
        {
            switch (reminder)
            {
                case ReminderTimeType.ZeroMin:
                    {
                        return 0;
                    }
                case ReminderTimeType.FiveMin:
                    {
                        return 5;
                    }
                case ReminderTimeType.TenMin:
                    {
                        return 10;
                    }
                case ReminderTimeType.FifteenMin:
                    {
                        return 15;
                    }
                case ReminderTimeType.ThirtyMin:
                    {
                        return 30;
                    }
                case ReminderTimeType.OneHour:
                    {
                        return 60;
                    }
                case ReminderTimeType.TwoHours:
                    {
                        return 120;
                    }
                case ReminderTimeType.ThreeHours:
                    {
                        return 180;
                    }
                case ReminderTimeType.FourHours:
                    {
                        return 240;
                    }
                case ReminderTimeType.FiveHours:
                    {
                        return 300;
                    }
                case ReminderTimeType.SixHours:
                    {
                        return 360;
                    }
                case ReminderTimeType.SevenHours:
                    {
                        return 420;
                    }
                case ReminderTimeType.EightHours:
                    {
                        return 480;
                    }
                case ReminderTimeType.NineHours:
                    {
                        return 540;
                    }
                case ReminderTimeType.TenHours:
                    {
                        return 600;
                    }
                case ReminderTimeType.ElevenHours:
                    {
                        return 660;
                    }
                case ReminderTimeType.HalfDay:
                    {
                        return 720;
                    }
                case ReminderTimeType.EighteenHours:
                    {
                        return 1080;
                    }
                case ReminderTimeType.OneDay:
                    {
                        return 1440;
                    }
                case ReminderTimeType.TwoDays:
                    {
                        return 2880;
                    }
                case ReminderTimeType.ThreeDays:
                    {
                        return 4320;
                    }
                case ReminderTimeType.FourDays:
                    {
                        return 5760;
                    }
                case ReminderTimeType.OneWeek:
                    {
                        return 10080;
                    }
                case ReminderTimeType.TwoWeeks:
                    {
                        return 20160;
                    }
            }

            return 0;
        }

        private static int GetWeekDay(string weekDay)
        {
            switch (weekDay)
            {
                case "SU":
                    {
                        return 1;
                    }
                case "MO":
                    {
                        return 2;
                    }
                case "TU":
                    {
                        return 3;
                    }
                case "WE":
                    {
                        return 4;
                    }
                case "TH":
                    {
                        return 5;
                    }
                case "FR":
                    {
                        return 6;
                    }
                case "SA":
                    {
                        return 7;
                    }
            }
            return 8;
        }

        internal static Dictionary<string, List<string>> AddTimeZones()
        {
            ICalTimeZones = new Dictionary<string, List<string>>
            {
                {"UTC-1200", valueList = new List<string>{"(UTC-12:00) International Date Line West","Dateline Standard Time","BEGIN:VTIMEZONE\nTZID:Dateline Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:-1200\nTZOFFSETTO:-1200\nEND:STANDARD\nEND:VTIMEZONE\n"}},

                {"UTC-1100", valueList = new List<string>{"(UTC-11:00) Coordinated Universal Time-11","UTC-11","BEGIN:VTIMEZONE\nTZID:UTC-11\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:-1100\nTZOFFSETTO:-1100\nEND:STANDARD\nEND:VTIMEZONE\n"}},

                {"UTC-1000", valueList = new List<string>{"(UTC-10:00) Hawaii","Hawaiian Standard Time","BEGIN:VTIMEZONE\nTZID:Hawaiian Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:-1000\nTZOFFSETTO:-1000\nEND:STANDARD\nEND:VTIMEZONE\n"}},

                {"UTC-0900", valueList = new List<string>{"(UTC-09:00) Alaska","Alaskan Standard Time","BEGIN:VTIMEZONE\nTZID:Alaskan Standard Time\nBEGIN:STANDARD\nDTSTART:16011104T020000\nRRULE:FREQ=YEARLY;BYDAY=1SU;BYMONTH=11\nTZOFFSETFROM:-0800\nTZOFFSETTO:-0900\nEND:STANDARD\nBEGIN:DAYLIGHT\nDTSTART:16010311T020000\nRRULE:FREQ=YEARLY;BYDAY=2SU;BYMONTH=3\nTZOFFSETFROM:-0900\nTZOFFSETTO:-0800\nEND:DAYLIGHT\nEND:VTIMEZONE\n"}},


                {"UTC-0800", valueList = new List<string>{"(UTC-08:00) Baja California","Pacific Standard Time (Mexico)","BEGIN:VTIMEZONE\nTZID:Pacific Standard Time (Mexico)\nBEGIN:STANDARD\nDTSTART:16011028T020000\nRRULE:FREQ=YEARLY;BYDAY=-1SU;BYMONTH=10\nTZOFFSETFROM:-0700\nTZOFFSETTO:-0800\nEND:STANDARD\nBEGIN:DAYLIGHT\nDTSTART:16010401T020000\nRRULE:FREQ=YEARLY;BYDAY=1SU;BYMONTH=4\nTZOFFSETFROM:-0800\nTZOFFSETTO:-0700\nEND:DAYLIGHT\nEND:VTIMEZONE\n"}},


                {"UTC-0800_2", valueList = new List<string>{"(UTC-08:00) Pacific Time (US & Canada)","Pacific Standard Time","BEGIN:VTIMEZONE\nTZID:Pacific Standard Time\nBEGIN:STANDARD\nDTSTART:16011104T020000\nRRULE:FREQ=YEARLY;BYDAY=1SU;BYMONTH=11\nTZOFFSETFROM:-0700\nTZOFFSETTO:-0800\nEND:STANDARD\nBEGIN:DAYLIGHT\nDTSTART:16010311T020000\nRRULE:FREQ=YEARLY;BYDAY=2SU;BYMONTH=3\nTZOFFSETFROM:-0800\nTZOFFSETTO:-0700\nEND:DAYLIGHT\nEND:VTIMEZONE\n"}},


                {"UTC-0700", valueList = new List<string>{"(UTC-07:00) Arizona","US Mountain Standard Time","BEGIN:VTIMEZONE\nTZID:US Mountain Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:-0700\nTZOFFSETTO:-0700\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC-0700_2", valueList = new List<string>{"(UTC-07:00) Chihuahua, La Paz, Mazatlan","Mountain Standard Time (Mexico)","BEGIN:VTIMEZONE\nTZID:Mountain Standard Time (Mexico)\nBEGIN:STANDARD\nDTSTART:16011028T020000\nRRULE:FREQ=YEARLY;BYDAY=-1SU;BYMONTH=10\nTZOFFSETFROM:-0600\nTZOFFSETTO:-0700\nEND:STANDARD\nBEGIN:DAYLIGHT\nDTSTART:16010401T020000\nRRULE:FREQ=YEARLY;BYDAY=1SU;BYMONTH=4\nTZOFFSETFROM:-0700\nTZOFFSETTO:-0600\nEND:DAYLIGHT\nEND:VTIMEZONE\n"}},


                {"UTC-0700_3", valueList = new List<string>{"(UTC-07:00) Mountain Time (US & Canada)","Mountain Standard Time","BEGIN:VTIMEZONE\nTZID:Mountain Standard Time\nBEGIN:STANDARD\nDTSTART:16011104T020000\nRRULE:FREQ=YEARLY;BYDAY=1SU;BYMONTH=11\nTZOFFSETFROM:-0600\nTZOFFSETTO:-0700\nEND:STANDARD\nBEGIN:DAYLIGHT\nDTSTART:16010311T020000\nRRULE:FREQ=YEARLY;BYDAY=2SU;BYMONTH=3\nTZOFFSETFROM:-0700\nTZOFFSETTO:-0600\nEND:DAYLIGHT\nEND:VTIMEZONE\n"}},


                {"UTC-0600", valueList = new List<string>{"(UTC-06:00) Central America","Central America Standard Time","BEGIN:VTIMEZONE\nTZID:Central America Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:-0600\nTZOFFSETTO:-0600\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC-0600_2", valueList = new List<string>{"(UTC-06:00) Central Time (US & Canada)","Central Standard Time","BEGIN:VTIMEZONE\nTZID:Central Standard Time\nBEGIN:STANDARD\nDTSTART:16011104T020000\nRRULE:FREQ=YEARLY;BYDAY=1SU;BYMONTH=11\nTZOFFSETFROM:-0500\nTZOFFSETTO:-0600\nEND:STANDARD\nBEGIN:DAYLIGHT\nDTSTART:16010311T020000\nRRULE:FREQ=YEARLY;BYDAY=2SU;BYMONTH=3\nTZOFFSETFROM:-0600\nTZOFFSETTO:-0500\nEND:DAYLIGHT\nEND:VTIMEZONE\n"}},


                {"UTC-0600_3", valueList = new List<string>{"(UTC-06:00) Guadalajara, Mexico City, Monterrey","Central Standard Time (Mexico)","BEGIN:VTIMEZONE\nTZID:Central Standard Time (Mexico)\nBEGIN:STANDARD\nDTSTART:16011028T020000\nRRULE:FREQ=YEARLY;BYDAY=-1SU;BYMONTH=10\nTZOFFSETFROM:-0500\nTZOFFSETTO:-0600\nEND:STANDARD\nBEGIN:DAYLIGHT\nDTSTART:16010401T020000\nRRULE:FREQ=YEARLY;BYDAY=1SU;BYMONTH=4\nTZOFFSETFROM:-0600\nTZOFFSETTO:-0500\nEND:DAYLIGHT\nEND:VTIMEZONE\n"}},


                {"UTC-0600_4", valueList = new List<string>{"(UTC-06:00) Saskatchewan","Canada Central Standard Time","BEGIN:VTIMEZONE\nTZID:Canada Central Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:-0600\nTZOFFSETTO:-0600\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC-0500", valueList = new List<string>{"(UTC-05:00) Bogota, Lima, Quito","SA Pacific Standard Time","BEGIN:VTIMEZONE\nTZID:SA Pacific Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:-0500\nTZOFFSETTO:-0500\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC-0500_2", valueList = new List<string>{"(UTC-05:00) Eastern Time (US & Canada)","Eastern Standard Time","BEGIN:VTIMEZONE\nTZID:Eastern Standard Time\nBEGIN:STANDARD\nDTSTART:16011104T020000\nRRULE:FREQ=YEARLY;BYDAY=1SU;BYMONTH=11\nTZOFFSETFROM:-0400\nTZOFFSETTO:-0500\nEND:STANDARD\nBEGIN:DAYLIGHT\nDTSTART:16010311T020000\nRRULE:FREQ=YEARLY;BYDAY=2SU;BYMONTH=3\nTZOFFSETFROM:-0500\nTZOFFSETTO:-0400\nEND:DAYLIGHT\nEND:VTIMEZONE\n"}},


                {"UTC-0500_3", valueList = new List<string>{"(UTC-05:00) Indiana (East)","US Eastern Standard Time","BEGIN:VTIMEZONE\nTZID:US Eastern Standard Time\nBEGIN:STANDARD\nDTSTART:16011104T020000\nRRULE:FREQ=YEARLY;BYDAY=1SU;BYMONTH=11\nTZOFFSETFROM:-0400\nTZOFFSETTO:-0500\nEND:STANDARD\nBEGIN:DAYLIGHT\nDTSTART:16010311T020000\nRRULE:FREQ=YEARLY;BYDAY=2SU;BYMONTH=3\nTZOFFSETFROM:-0500\nTZOFFSETTO:-0400\nEND:DAYLIGHT\nEND:VTIMEZONE\n"}},


                {"UTC-0430", valueList = new List<string>{"(UTC-04:30) Caracas","Venezuela Standard Time","BEGIN:VTIMEZONE\nTZID:Venezuela Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:-0430\nTZOFFSETTO:-0430\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC-0400", valueList = new List<string>{"(UTC-04:00) Asuncion","Paraguay Standard Time","BEGIN:VTIMEZONE\nTZID:Paraguay Standard Time\nBEGIN:STANDARD\nDTSTART:16010414T235959\nRRULE:FREQ=YEARLY;BYDAY=2SA;BYMONTH=4\nTZOFFSETFROM:-0300\nTZOFFSETTO:-0400\nEND:STANDARD\nBEGIN:DAYLIGHT\nDTSTART:16011006T235959\nRRULE:FREQ=YEARLY;BYDAY=1SA;BYMONTH=10\nTZOFFSETFROM:-0400\nTZOFFSETTO:-0300\nEND:DAYLIGHT\nEND:VTIMEZONE\n"}},


                {"UTC-0400_2", valueList = new List<string>{"(UTC-04:00) Atlantic Time (Canada)","Atlantic Standard Time","BEGIN:VTIMEZONE\nTZID:Atlantic Standard Time\nBEGIN:STANDARD\nDTSTART:16011104T020000\nRRULE:FREQ=YEARLY;BYDAY=1SU;BYMONTH=11\nTZOFFSETFROM:-0300\nTZOFFSETTO:-0400\nEND:STANDARD\nBEGIN:DAYLIGHT\nDTSTART:16010311T020000\nRRULE:FREQ=YEARLY;BYDAY=2SU;BYMONTH=3\nTZOFFSETFROM:-0400\nTZOFFSETTO:-0300\nEND:DAYLIGHT\nEND:VTIMEZONE\n"}},


                {"UTC-0400_3", valueList = new List<string>{"(UTC-04:00) Cuiaba","Central Brazilian Standard Time","BEGIN:VTIMEZONE\nTZID:Central Brazilian Standard Time\nBEGIN:STANDARD\nDTSTART:16010217T235959\nRRULE:FREQ=YEARLY;BYDAY=3SA;BYMONTH=2\nTZOFFSETFROM:-0300\nTZOFFSETTO:-0400\nEND:STANDARD\nBEGIN:DAYLIGHT\nDTSTART:16011020T235959\nRRULE:FREQ=YEARLY;BYDAY=3SA;BYMONTH=10\nTZOFFSETFROM:-0400\nTZOFFSETTO:-0300\nEND:DAYLIGHT\nEND:VTIMEZONE\n"}},


                {"UTC-0400_4", valueList = new List<string>{"(UTC-04:00) Georgetown, La Paz, San Juan","SA Western Standard Time","BEGIN:VTIMEZONE\nTZID:SA Western Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:-0400\nTZOFFSETTO:-0400\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC-0400_5", valueList = new List<string>{"(UTC-04:00) Santiago","Pacific SA Standard Time","BEGIN:VTIMEZONE\nTZID:Pacific SA Standard Time\nBEGIN:STANDARD\nDTSTART:16010310T235959\nRRULE:FREQ=YEARLY;BYDAY=2SA;BYMONTH=3\nTZOFFSETFROM:-0300\nTZOFFSETTO:-0400\nEND:STANDARD\nBEGIN:DAYLIGHT\nDTSTART:16011013T235959\nRRULE:FREQ=YEARLY;BYDAY=2SA;BYMONTH=10\nTZOFFSETFROM:-0400\nTZOFFSETTO:-0300\nEND:DAYLIGHT\nEND:VTIMEZONE\n"}},


                {"UTC-0330", valueList = new List<string>{"(UTC-03:30) Newfoundland","Newfoundland Standard Time","BEGIN:VTIMEZONE\nTZID:Newfoundland Standard Time\nBEGIN:STANDARD\nDTSTART:16011104T020000\nRRULE:FREQ=YEARLY;BYDAY=1SU;BYMONTH=11\nTZOFFSETFROM:-0230\nTZOFFSETTO:-0330\nEND:STANDARD\nBEGIN:DAYLIGHT\nDTSTART:16010311T020000\nRRULE:FREQ=YEARLY;BYDAY=2SU;BYMONTH=3\nTZOFFSETFROM:-0330\nTZOFFSETTO:-0230\nEND:DAYLIGHT\nEND:VTIMEZONE\n"}},


                {"UTC-0300", valueList = new List<string>{"(UTC-03:00) Brasilia","E. South America Standard Time","BEGIN:VTIMEZONE\nTZID:E. South America Standard Time\nBEGIN:STANDARD\nDTSTART:16010217T235959\nRRULE:FREQ=YEARLY;BYDAY=3SA;BYMONTH=2\nTZOFFSETFROM:-0200\nTZOFFSETTO:-0300\nEND:STANDARD\nBEGIN:DAYLIGHT\nDTSTART:16011020T235959\nRRULE:FREQ=YEARLY;BYDAY=3SA;BYMONTH=10\nTZOFFSETFROM:-0300\nTZOFFSETTO:-0200\nEND:DAYLIGHT\nEND:VTIMEZONE\n"}},


                {"UTC-0300_2", valueList = new List<string>{"(UTC-03:00) Buenos Aires","Argentina Standard Time","BEGIN:VTIMEZONE\nTZID:Argentina Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:-0300\nTZOFFSETTO:-0300\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC-0300_3", valueList = new List<string>{"(UTC-03:00) Cayenne, Fortaleza","SA Eastern Standard Time","BEGIN:VTIMEZONE\nTZID:SA Eastern Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:-0300\nTZOFFSETTO:-0300\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC-0300_4", valueList = new List<string>{"(UTC-03:00) Greenland","Greenland Standard Time","BEGIN:VTIMEZONE\nTZID:Greenland Standard Time\nBEGIN:STANDARD\nDTSTART:16011027T230000\nRRULE:FREQ=YEARLY;BYDAY=-1SA;BYMONTH=10\nTZOFFSETFROM:-0200\nTZOFFSETTO:-0300\nEND:STANDARD\nBEGIN:DAYLIGHT\nDTSTART:16010331T220000\nRRULE:FREQ=YEARLY;BYDAY=-1SA;BYMONTH=3\nTZOFFSETFROM:-0300\nTZOFFSETTO:-0200\nEND:DAYLIGHT\nEND:VTIMEZONE\n"}},


                {"UTC-0300_5", valueList = new List<string>{"(UTC-03:00) Montevideo","Montevideo Standard Time","BEGIN:VTIMEZONE\nTZID:Montevideo Standard Time\nBEGIN:STANDARD\nDTSTART:16010311T020000\nRRULE:FREQ=YEARLY;BYDAY=2SU;BYMONTH=3\nTZOFFSETFROM:-0200\nTZOFFSETTO:-0300\nEND:STANDARD\nBEGIN:DAYLIGHT\nDTSTART:16011007T020000\nRRULE:FREQ=YEARLY;BYDAY=1SU;BYMONTH=10\nTZOFFSETFROM:-0300\nTZOFFSETTO:-0200\nEND:DAYLIGHT\nEND:VTIMEZONE\n"}},


                {"UTC-0300_6", valueList = new List<string>{"(UTC-03:00) Salvador","Bahia Standard Time","BEGIN:VTIMEZONE\nTZID:Bahia Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:-0300\nTZOFFSETTO:-0300\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC-0200", valueList = new List<string>{"(UTC-02:00) Mid-Atlantic","Mid-Atlantic Standard Time","BEGIN:VTIMEZONE\nTZID:Mid-Atlantic Standard Time\nBEGIN:STANDARD\nDTSTART:16010930T020000\nRRULE:FREQ=YEARLY;BYDAY=-1SU;BYMONTH=9\nTZOFFSETFROM:-0100\nTZOFFSETTO:-0200\nEND:STANDARD\nBEGIN:DAYLIGHT\nDTSTART:16010325T020000\nRRULE:FREQ=YEARLY;BYDAY=-1SU;BYMONTH=3\nTZOFFSETFROM:-0200\nTZOFFSETTO:-0100\nEND:DAYLIGHT\nEND:VTIMEZONE\n"}},


                {"UTC-0200_2", valueList = new List<string>{"(UTC-02:00) Coordinated Universal Time-02","UTC-02","BEGIN:VTIMEZONE\nTZID:UTC-02\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:-0200\nTZOFFSETTO:-0200\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC-0100", valueList = new List<string>{"(UTC-01:00) Azores","Azores Standard Time","BEGIN:VTIMEZONE\nTZID:Azores Standard Time\nBEGIN:STANDARD\nDTSTART:16011028T010000\nRRULE:FREQ=YEARLY;BYDAY=-1SU;BYMONTH=10\nTZOFFSETFROM:-0000\nTZOFFSETTO:-0100\nEND:STANDARD\nBEGIN:DAYLIGHT\nDTSTART:16010325T000000\nRRULE:FREQ=YEARLY;BYDAY=-1SU;BYMONTH=3\nTZOFFSETFROM:-0100\nTZOFFSETTO:-0000\nEND:DAYLIGHT\nEND:VTIMEZONE\n"}},


                {"UTC-0100_2", valueList = new List<string>{"(UTC-01:00) Cape Verde Is.","Cape Verde Standard Time","BEGIN:VTIMEZONE\nTBEGIN:VTIMEZONE\nTZID:Cape Verde Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:-0100\nTZOFFSETTO:-0100\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC-0000", valueList = new List<string>{"(UTC) Casablanca","Morocco Standard Time","BEGIN:VTIMEZONE\nTZID:Morocco Standard Time\nBEGIN:STANDARD\nDTSTART:16010930T030000\nRRULE:FREQ=YEARLY;BYDAY=-1SU;BYMONTH=9\nTZOFFSETFROM:+0100\nTZOFFSETTO:-0000\nEND:STANDARD\nBEGIN:DAYLIGHT\nDTSTART:16010429T020000\nRRULE:FREQ=YEARLY;BYDAY=-1SU;BYMONTH=4\nTZOFFSETFROM:-0000\nTZOFFSETTO:+0100\nEND:DAYLIGHT\nEND:VTIMEZONE\n"}},


                {"UTC-0000_2", valueList = new List<string>{"(UTC) Coordinated Universal Time","UTC","BEGIN:VTIMEZONE\nTZID:UTC\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:-0000\nTZOFFSETTO:-0000\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC-0000_3", valueList = new List<string>{"(UTC) Dublin, Edinburgh, Lisbon, London","GMT Standard Time","BEGIN:VTIMEZONE\nTZID:GMT Standard Time\nBEGIN:STANDARD\nDTSTART:16011028T020000\nRRULE:FREQ=YEARLY;BYDAY=-1SU;BYMONTH=10\nTZOFFSETFROM:+0100\nTZOFFSETTO:-0000\nEND:STANDARD\nBEGIN:DAYLIGHT\nDTSTART:16010325T010000\nRRULE:FREQ=YEARLY;BYDAY=-1SU;BYMONTH=3\nTZOFFSETFROM:-0000\nTZOFFSETTO:+0100\nEND:DAYLIGHT\nEND:VTIMEZONE\n"}},


                {"UTC-0000_4", valueList = new List<string>{"(UTC) Monrovia, Reykjavik","Greenwich Standard Time","BEGIN:VTIMEZONE\nTZID:Greenwich Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:-0000\nTZOFFSETTO:-0000\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC+0100", valueList = new List<string>{"(UTC+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna","W. Europe Standard Time","BEGIN:VTIMEZONE\nTZID:W. Europe Standard Time\nBEGIN:STANDARD\nDTSTART:16011028T030000\nRRULE:FREQ=YEARLY;BYDAY=-1SU;BYMONTH=10\nTZOFFSETFROM:+0200\nTZOFFSETTO:+0100\nEND:STANDARD\nBEGIN:DAYLIGHT\nDTSTART:16010325T020000\nRRULE:FREQ=YEARLY;BYDAY=-1SU;BYMONTH=3\nTZOFFSETFROM:+0100\nTZOFFSETTO:+0200\nEND:DAYLIGHT\nEND:VTIMEZONE\n"}},


                {"UTC+0100_2", valueList = new List<string>{"(UTC+01:00) Belgrade, Bratislava, Budapest, Ljubljana, Prague","Central Europe Standard Time","BEGIN:VTIMEZONE\nTZID:Central Europe Standard Time\nBEGIN:STANDARD\nDTSTART:16011028T030000\nRRULE:FREQ=YEARLY;BYDAY=-1SU;BYMONTH=10\nTZOFFSETFROM:+0200\nTZOFFSETTO:+0100\nEND:STANDARD\nBEGIN:DAYLIGHT\nDTSTART:16010325T020000\nRRULE:FREQ=YEARLY;BYDAY=-1SU;BYMONTH=3\nTZOFFSETFROM:+0100\nTZOFFSETTO:+0200\nEND:DAYLIGHT\nEND:VTIMEZONE\n"}},


                {"UTC+0100_3", valueList = new List<string>{"(UTC+01:00) Brussels, Copenhagen, Madrid, Paris","Romance Standard Time","BEGIN:VTIMEZONE\nTZID:Romance Standard Time\nBEGIN:STANDARD\nDTSTART:16011028T030000\nRRULE:FREQ=YEARLY;BYDAY=-1SU;BYMONTH=10\nTZOFFSETFROM:+0200\nTZOFFSETTO:+0100\nEND:STANDARD\nBEGIN:DAYLIGHT\nDTSTART:16010325T020000\nRRULE:FREQ=YEARLY;BYDAY=-1SU;BYMONTH=3\nTZOFFSETFROM:+0100\nTZOFFSETTO:+0200\nEND:DAYLIGHT\nEND:VTIMEZONE\n"}},


                {"UTC+0100_4", valueList = new List<string>{"(UTC+01:00) Sarajevo, Skopje, Warsaw, Zagreb","Central European Standard Time","BEGIN:VTIMEZONE\nTZID:Central European Standard Time\nBEGIN:STANDARD\nDTSTART:16011028T030000\nRRULE:FREQ=YEARLY;BYDAY=-1SU;BYMONTH=10\nTZOFFSETFROM:+0200\nTZOFFSETTO:+0100\nEND:STANDARD\nBEGIN:DAYLIGHT\nDTSTART:16010325T020000\nRRULE:FREQ=YEARLY;BYDAY=-1SU;BYMONTH=3\nTZOFFSETFROM:+0100\nTZOFFSETTO:+0200\nEND:DAYLIGHT\nEND:VTIMEZONE\n"}},


                {"UTC+0100_5", valueList = new List<string>{"(UTC+01:00) West Central Africa","W. Central Africa Standard Time","BEGIN:VTIMEZONE\nTZID:W. Central Africa Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:+0100\nTZOFFSETTO:+0100\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC+0100_6", valueList = new List<string>{"(UTC+01:00) Windhoek","Namibia Standard Time","BEGIN:VTIMEZONE\nTZID:Namibia Standard Time\nBEGIN:STANDARD\nDTSTART:16010401T020000\nRRULE:FREQ=YEARLY;BYDAY=1SU;BYMONTH=4\nTZOFFSETFROM:+0200\nTZOFFSETTO:+0100\nEND:STANDARD\nBEGIN:DAYLIGHT\nDTSTART:16010902T020000\nRRULE:FREQ=YEARLY;BYDAY=1SU;BYMONTH=9\nTZOFFSETFROM:+0100\nTZOFFSETTO:+0200\nEND:DAYLIGHT\nEND:VTIMEZONE\n"}},


                {"UTC+0100_7", valueList = new List<string>{"(UTC+02:00) Athens, Bucharest","GTB Standard Time","BEGIN:VTIMEZONE\nTZID:GTB Standard Time\nBEGIN:STANDARD\nDTSTART:16011028T040000\nRRULE:FREQ=YEARLY;BYDAY=-1SU;BYMONTH=10\nTZOFFSETFROM:+0300\nTZOFFSETTO:+0200\nEND:STANDARD\nBEGIN:DAYLIGHT\nDTSTART:16010325T030000\nRRULE:FREQ=YEARLY;BYDAY=-1SU;BYMONTH=3\nTZOFFSETFROM:+0200\nTZOFFSETTO:+0300\nEND:DAYLIGHT\nEND:VTIMEZONE\n"}},


                {"UTC+0200", valueList = new List<string>{"(UTC+02:00) Beirut","Middle East Standard Time","BEGIN:VTIMEZONE\nTZID:Middle East Standard Time\nBEGIN:STANDARD\nDTSTART:16011027T235959\nRRULE:FREQ=YEARLY;BYDAY=-1SA;BYMONTH=10\nTZOFFSETFROM:+0300\nTZOFFSETTO:+0200\nEND:STANDARD\nBEGIN:DAYLIGHT\nDTSTART:16010331T235959\nRRULE:FREQ=YEARLY;BYDAY=-1SA;BYMONTH=3\nTZOFFSETFROM:+0200\nTZOFFSETTO:+0300\nEND:DAYLIGHT\nEND:VTIMEZONE\n"}},


                {"UTC+0200_2", valueList = new List<string>{"(UTC+02:00) Cairo","Egypt Standard Time","BEGIN:VTIMEZONE\nTZID:Egypt Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:+0200\nTZOFFSETTO:+0200\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC+0200_3", valueList = new List<string>{"(UTC+02:00) Damascus","Syria Standard Time","BEGIN:VTIMEZONE\nTZID:Syria Standard Time\nBEGIN:STANDARD\nDTSTART:16011025T235959\nRRULE:FREQ=YEARLY;BYDAY=-1TH;BYMONTH=10\nTZOFFSETFROM:+0300\nTZOFFSETTO:+0200\nEND:STANDARD\nBEGIN:DAYLIGHT\nDTSTART:16010405T235959\nRRULE:FREQ=YEARLY;BYDAY=1TH;BYMONTH=4\nTZOFFSETFROM:+0200\nTZOFFSETTO:+0300\nEND:DAYLIGHT\nEND:VTIMEZONE\n"}},


                {"UTC+0200_4", valueList = new List<string>{"(UTC+02:00) E.Europe","E. Europe Standard Time","BEGIN:VTIMEZONE\nTZID:E. Europe Standard Time\nBEGIN:STANDARD\nDTSTART:16011028T030000\nRRULE:FREQ=YEARLY;BYDAY=-1SU;BYMONTH=10\nTZOFFSETFROM:+0300\nTZOFFSETTO:+0200\nEND:STANDARD\nBEGIN:DAYLIGHT\nDTSTART:16010325T020000\nRRULE:FREQ=YEARLY;BYDAY=-1SU;BYMONTH=3\nTZOFFSETFROM:+0200\nTZOFFSETTO:+0300\nEND:DAYLIGHT\nEND:VTIMEZONE\n"}},


                {"UTC+0200_5", valueList = new List<string>{"(UTC+02:00) Harare, Pretoria","South Africa Standard Time","BEGIN:VTIMEZONE\nTZID:South Africa Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:+0200\nTZOFFSETTO:+0200\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC+0200_6", valueList = new List<string>{"(UTC+02:00) Helsinki, Kyiv, Riga, Sofia, Tallinn, Vilnius","FLE Standard Time","BEGIN:VTIMEZONE\nTZID:FLE Standard Time\nBEGIN:STANDARD\nDTSTART:16011028T040000\nRRULE:FREQ=YEARLY;BYDAY=-1SU;BYMONTH=10\nTZOFFSETFROM:+0300\nTZOFFSETTO:+0200\nEND:STANDARD\nBEGIN:DAYLIGHT\nDTSTART:16010325T030000\nRRULE:FREQ=YEARLY;BYDAY=-1SU;BYMONTH=3\nTZOFFSETFROM:+0200\nTZOFFSETTO:+0300\nEND:DAYLIGHT\nEND:VTIMEZONE\n"}},


                {"UTC+0200_7", valueList = new List<string>{"(UTC+02:00) Istanbul","Turkey Standard Time","BEGIN:VTIMEZONE\nTZID:Turkey Standard Time\nBEGIN:STANDARD\nDTSTART:16011028T040000\nRRULE:FREQ=YEARLY;BYDAY=-1SU;BYMONTH=10\nTZOFFSETFROM:+0300\nTZOFFSETTO:+0200\nEND:STANDARD\nBEGIN:DAYLIGHT\nDTSTART:16010325T030000\nRRULE:FREQ=YEARLY;BYDAY=-1SU;BYMONTH=3\nTZOFFSETFROM:+0200\nTZOFFSETTO:+0300\nEND:DAYLIGHT\nEND:VTIMEZONE\n"}},


                {"UTC+0200_8", valueList = new List<string>{"(UTC+02:00) Jerusalem","Israel Standard Time","BEGIN:VTIMEZONE\nTZID:Israel Standard Time\nBEGIN:STANDARD\nDTSTART:16010909T020000\nRRULE:FREQ=YEARLY;BYDAY=2SU;BYMONTH=9\nTZOFFSETFROM:+0300\nTZOFFSETTO:+0200\nEND:STANDARD\nBEGIN:DAYLIGHT\nDTSTART:16010330T020000\nRRULE:FREQ=YEARLY;BYDAY=-1FR;BYMONTH=3\nTZOFFSETFROM:+0200\nTZOFFSETTO:+0300\nEND:DAYLIGHT\nEND:VTIMEZONE\n"}},


                {"UTC+0300", valueList = new List<string>{"(UTC+03:00) Amman","Jordan Standard Time","BEGIN:VTIMEZONE\nTZID:Jordan Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:+0300\nTZOFFSETTO:+0300\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC+0300_2", valueList = new List<string>{"(UTC+03:00) Baghdad","Arabic Standard Time","BEGIN:VTIMEZONE\nTZID:Arabic Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:+0300\nTZOFFSETTO:+0300\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC+0300_3", valueList = new List<string>{"(UTC+03:00) Kaliningrad, Minsk","Kaliningrad Standard Time","BEGIN:VTIMEZONE\nTZID:Kaliningrad Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:+0300\nTZOFFSETTO:+0300\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC+0300_4", valueList = new List<string>{"(UTC+03:00) Kuwait, Riyadh","Arab Standard Time","BEGIN:VTIMEZONE\nTZID:Arab Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:+0300\nTZOFFSETTO:+0300\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC+0300_5", valueList = new List<string>{"(UTC+03:00) Nairobi","E. Africa Standard Time","BEGIN:VTIMEZONE\nTZID:E. Africa Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:+0300\nTZOFFSETTO:+0300\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC+0330", valueList = new List<string>{"(UTC+03:30) Tehran","Iran Standard Time","BEGIN:VTIMEZONE\nTZID:Iran Standard Time\nBEGIN:STANDARD\nDTSTART:16010917T235959\nRRULE:FREQ=YEARLY;BYDAY=3MO;BYMONTH=9\nTZOFFSETFROM:+0430\nTZOFFSETTO:+0330\nEND:STANDARD\nBEGIN:DAYLIGHT\nDTSTART:16010317T235959\nRRULE:FREQ=YEARLY;BYDAY=3SA;BYMONTH=3\nTZOFFSETFROM:+0330\nTZOFFSETTO:+0430\nEND:DAYLIGHT\nEND:VTIMEZONE\n"}},


                {"UTC+0400", valueList = new List<string>{"(UTC+04:00) Abu Dhabi, Muscat","Arabian Standard Time","BEGIN:VTIMEZONE\nTZID:Arabian Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:+0400\nTZOFFSETTO:+0400\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC+0400_2", valueList = new List<string>{"(UTC+04:00) Baku","Azerbaijan Standard Time","BEGIN:VTIMEZONE\nTZID:Azerbaijan Standard Time\nBEGIN:STANDARD\nDTSTART:16011028T050000\nRRULE:FREQ=YEARLY;BYDAY=-1SU;BYMONTH=10\nTZOFFSETFROM:+0500\nTZOFFSETTO:+0400\nEND:STANDARD\nBEGIN:DAYLIGHT\nDTSTART:16010325T040000\nRRULE:FREQ=YEARLY;BYDAY=-1SU;BYMONTH=3\nTZOFFSETFROM:+0400\nTZOFFSETTO:+0500\nEND:DAYLIGHT\nEND:VTIMEZONE\n"}},


                {"UTC+0400_3", valueList = new List<string>{"(UTC+04:00) Moscow, St. Petersburg, Volgograd","Russian Standard Time","BEGIN:VTIMEZONE\nTZID:Russian Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:+0400\nTZOFFSETTO:+0400\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC+0400_4", valueList = new List<string>{"(UTC+04:00) Port Louis","Mauritius Standard Time","BEGIN:VTIMEZONE\nTZID:Mauritius Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:+0400\nTZOFFSETTO:+0400\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC+0400_5", valueList = new List<string>{"(UTC+04:00) Tbilisi","Georgian Standard Time","BEGIN:VTIMEZONE\nTZID:Georgian Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:+0400\nTZOFFSETTO:+0400\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC+0400_6", valueList = new List<string>{"(UTC+04:00) Yerevan","Caucasus Standard Time","BEGIN:VTIMEZONE\nTZID:Caucasus Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:+0400\nTZOFFSETTO:+0400\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC+0430", valueList = new List<string>{"(UTC+04:30) Kabul","Afghanistan Standard Time","BEGIN:VTIMEZONE\nTZID:Afghanistan Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:+0430\nTZOFFSETTO:+0430\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC+0500", valueList = new List<string>{"(UTC+05:00) Islamabad, Karachi","Pakistan Standard Time","BEGIN:VTIMEZONE\nTZID:Pakistan Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:+0500\nTZOFFSETTO:+0500\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC+0500_2", valueList = new List<string>{"(UTC+05:00) Tashkent","West Asia Standard Time","BEGIN:VTIMEZONE\nTZID:West Asia Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:+0500\nTZOFFSETTO:+0500\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC+0530", valueList = new List<string>{"(UTC+05:30) Chennai, Kolkata, Mumbai, New Delhi","India Standard Time","BEGIN:VTIMEZONE\nTZID:India Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:+0530\nTZOFFSETTO:+0530\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC+0530_2", valueList = new List<string>{"(UTC+05:30) Sri Jayawardenepura","Sri Lanka Standard Time","BEGIN:VTIMEZONE\nTZID:Sri Lanka Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:+0530\nTZOFFSETTO:+0530\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC+0545", valueList = new List<string>{"(UTC+05:45) Kathmandu","Nepal Standard Time","BEGIN:VTIMEZONE\nTZID:Nepal Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:+0545\nTZOFFSETTO:+0545\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC+0600", valueList = new List<string>{"(UTC+06:00) Astana","Central Asia Standard Time","BEGIN:VTIMEZONE\nTZID:Central Asia Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:+0600\nTZOFFSETTO:+0600\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC+0600_2", valueList = new List<string>{"(UTC+06:00) Dhaka","Bangladesh Standard Time","BEGIN:VTIMEZONE\nTZID:Bangladesh Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:+0600\nTZOFFSETTO:+0600\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC+0600_3", valueList = new List<string>{"(UTC+06:00) Ekaterinburg","Ekaterinburg Standard Time","BEGIN:VTIMEZONE\nTZID:Ekaterinburg Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:+0600\nTZOFFSETTO:+0600\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC+0630", valueList = new List<string>{"(UTC+06:30) Yangon (Rangoon)","Myanmar Standard Time","BEGIN:VTIMEZONE\nTZID:Myanmar Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:+0630\nTZOFFSETTO:+0630\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC+0700", valueList = new List<string>{"(UTC+07:00) Bangkok, Hanoi, Jakarta","SE Asia Standard Time","BEGIN:VTIMEZONE\nTZID:SE Asia Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:+0700\nTZOFFSETTO:+0700\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC+0700_2", valueList = new List<string>{"(UTC+07:00) Novosibirsk","N. Central Asia Standard Time","BEGIN:VTIMEZONE\nTZID:N. Central Asia Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:+0700\nTZOFFSETTO:+0700\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC+0800", valueList = new List<string>{"(UTC+08:00) Beijing, Chongqing, Hong Kong, Urumqi","China Standard Time","BEGIN:VTIMEZONE\nTZID:China Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:+0800\nTZOFFSETTO:+0800\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC+0800_2", valueList = new List<string>{"(UTC+08:00) Krasnoyarsk","North Asia Standard Time","BEGIN:VTIMEZONE\nTZID:North Asia Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:+0800\nTZOFFSETTO:+0800\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC+0800_3", valueList = new List<string>{"(UTC+08:00) Kuala Lumpur, Singapore","Singapore Standard Time","BEGIN:VTIMEZONE\nTZID:Singapore Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:+0800\nTZOFFSETTO:+0800\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC+0800_4", valueList = new List<string>{"(UTC+08:00) Perth","W. Australia Standard Time","BEGIN:VTIMEZONE\nTZID:W. Australia Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:+0800\nTZOFFSETTO:+0800\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC+0800_5", valueList = new List<string>{"(UTC+08:00) Taipei","Taipei Standard Time","BEGIN:VTIMEZONE\nTZID:Taipei Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:+0800\nTZOFFSETTO:+0800\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC+0800_6", valueList = new List<string>{"(UTC+08:00) Ulaanbaatar","Ulaanbaatar Standard Time","BEGIN:VTIMEZONE\nTZID:Ulaanbaatar Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:+0800\nTZOFFSETTO:+0800\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC+0900", valueList = new List<string>{"(UTC+09:00) Irkutsk","North Asia East Standard Time","BEGIN:VTIMEZONE\nTZID:North Asia East Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:+0900\nTZOFFSETTO:+0900\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC+0900_2", valueList = new List<string>{"(UTC+09:00) Osaka, Sapporo, Tokyo","Tokyo Standard Time","BEGIN:VTIMEZONE\nTZID:Tokyo Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:+0900\nTZOFFSETTO:+0900\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC+0900_3", valueList = new List<string>{"(UTC+09:00) Seoul","Korea Standard Time","BEGIN:VTIMEZONE\nTZID:Korea Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:+0900\nTZOFFSETTO:+0900\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC+0930", valueList = new List<string>{"(UTC+09:30) Adelaide","Cen. Australia Standard Time","BEGIN:VTIMEZONE\nTZID:Cen. Australia Standard Time\nBEGIN:STANDARD\nDTSTART:16010401T030000\nRRULE:FREQ=YEARLY;BYDAY=1SU;BYMONTH=4\nTZOFFSETFROM:+1030\nTZOFFSETTO:+0930\nEND:STANDARD\nBEGIN:DAYLIGHT\nDTSTART:16011007T020000\nRRULE:FREQ=YEARLY;BYDAY=1SU;BYMONTH=10\nTZOFFSETFROM:+0930\nTZOFFSETTO:+1030\nEND:DAYLIGHT\nEND:VTIMEZONE\n"}},


                {"UTC+0930_2", valueList = new List<string>{"(UTC+09:30) Darwin","AUS Central Standard Time","BEGIN:VTIMEZONE\nTZID:AUS Central Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:+0930\nTZOFFSETTO:+0930\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC+1000", valueList = new List<string>{"(UTC+10:00) Brisbane","E. Australia Standard Time","BEGIN:VTIMEZONE\nTZID:E. Australia Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:+1000\nTZOFFSETTO:+1000\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC+1000_2", valueList = new List<string>{"(UTC+10:00) Canberra, Melbourne, Sydney","AUS Eastern Standard Time","BEGIN:VTIMEZONE\nTZID:AUS Eastern Standard Time\nBEGIN:STANDARD\nDTSTART:16010401T030000\nRRULE:FREQ=YEARLY;BYDAY=1SU;BYMONTH=4\nTZOFFSETFROM:+1100\nTZOFFSETTO:+1000\nEND:STANDARD\nBEGIN:DAYLIGHT\nDTSTART:16011007T020000\nRRULE:FREQ=YEARLY;BYDAY=1SU;BYMONTH=10\nTZOFFSETFROM:+1000\nTZOFFSETTO:+1100\nEND:DAYLIGHT\nEND:VTIMEZONE\n"}},


                {"UTC+1000_3", valueList = new List<string>{"(UTC+10:00) Guam, Port Moresby","West Pacific Standard Time","BEGIN:VTIMEZONE\nTZID:West Pacific Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:+1000\nTZOFFSETTO:+1000\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC+1000_4", valueList = new List<string>{"(UTC+10:00) Hobart","Tasmania Standard Time","BEGIN:VTIMEZONE\nTZID:Tasmania Standard Time\nBEGIN:STANDARD\nDTSTART:16010401T030000\nRRULE:FREQ=YEARLY;BYDAY=1SU;BYMONTH=4\nTZOFFSETFROM:+1100\nTZOFFSETTO:+1000\nEND:STANDARD\nBEGIN:DAYLIGHT\nDTSTART:16011007T020000\nRRULE:FREQ=YEARLY;BYDAY=1SU;BYMONTH=10\nTZOFFSETFROM:+1000\nTZOFFSETTO:+1100\nEND:DAYLIGHT\nEND:VTIMEZONE\n"}},


                {"UTC+1000_5", valueList = new List<string>{"(UTC+10:00) Yakutsk","Yakutsk Standard Time","BEGIN:VTIMEZONE\nTZID:Yakutsk Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:+1000\nTZOFFSETTO:+1000\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC+1100", valueList = new List<string>{"(UTC+11:00) Solomon Is., New Caledonia","Central Pacific Standard Time","BEGIN:VTIMEZONE\nTZID:Central Pacific Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:+1100\nTZOFFSETTO:+1100\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC+1100_2", valueList = new List<string>{"(UTC+11:00) Vladivostok","Vladivostok Standard Time","BEGIN:VTIMEZONE\nTZID:Vladivostok Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:+1100\nTZOFFSETTO:+1100\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC+1200", valueList = new List<string>{"(UTC+12:00) Auckland, Wellington","New Zealand Standard Time","BEGIN:VTIMEZONE\nTZID:New Zealand Standard Time\nBEGIN:STANDARD\nDTSTART:16010401T030000\nRRULE:FREQ=YEARLY;BYDAY=1SU;BYMONTH=4\nTZOFFSETFROM:+1300\nTZOFFSETTO:+1200\nEND:STANDARD\nBEGIN:DAYLIGHT\nDTSTART:16010930T020000\nRRULE:FREQ=YEARLY;BYDAY=-1SU;BYMONTH=9\nTZOFFSETFROM:+1200\nTZOFFSETTO:+1300\nEND:DAYLIGHT\nEND:VTIMEZONE\n"}},


                {"UTC+1200_2", valueList = new List<string>{"(UTC+12:00) Coordinated Universal Time+12","UTC+12","BEGIN:VTIMEZONE\nTZID:UTC+12\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:+1200\nTZOFFSETTO:+1200\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC+1200_3", valueList = new List<string>{"(UTC+12:00) Fiji","Fiji Standard Time","BEGIN:VTIMEZONE\nTZID:Fiji Standard Time\nBEGIN:STANDARD\nDTSTART:16010121T030000\nRRULE:FREQ=YEARLY;BYDAY=3SU;BYMONTH=1\nTZOFFSETFROM:+1300\nTZOFFSETTO:+1200\nEND:STANDARD\nBEGIN:DAYLIGHT\nDTSTART:16011028T020000\nRRULE:FREQ=YEARLY;BYDAY=4SU;BYMONTH=10\nTZOFFSETFROM:+1200\nTZOFFSETTO:+1300\nEND:DAYLIGHT\nEND:VTIMEZONE\n"}},


                {"UTC+1200_4", valueList = new List<string>{"(UTC+12:00) Magadan","Magadan Standard Time","BEGIN:VTIMEZONE\nTZID:Magadan Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:+1200\nTZOFFSETTO:+1200\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC+1200_5", valueList = new List<string>{"(UTC+12:00) Petropavlovsk-Kamchatsky - Old","Kamchatka Standard Time","BEGIN:VTIMEZONE\nTZID:Kamchatka Standard Time\nBEGIN:STANDARD\nDTSTART:16011028T030000\nRRULE:FREQ=YEARLY;BYDAY=-1SU;BYMONTH=10\nTZOFFSETFROM:+1300\nTZOFFSETTO:+1200\nEND:STANDARD\nBEGIN:DAYLIGHT\nDTSTART:16010325T020000\nRRULE:FREQ=YEARLY;BYDAY=-1SU;BYMONTH=3\nTZOFFSETFROM:+1200\nTZOFFSETTO:+1300\nEND:DAYLIGHT\nEND:VTIMEZONE\n"}},


                {"UTC+1300", valueList = new List<string>{"(UTC+13:00) Nuku'alofa","Tonga Standard Time","BEGIN:VTIMEZONE\nTZID:Tonga Standard Time\nBEGIN:STANDARD\nDTSTART:16010101T000000\nTZOFFSETFROM:+1300\nTZOFFSETTO:+1300\nEND:STANDARD\nEND:VTIMEZONE\n"}},


                {"UTC+1300_2", valueList = new List<string>{"(UTC+13:00) Samoa","Samoa Standard Time","BEGIN:VTIMEZONE\nTZID:Samoa Standard Time\nBEGIN:STANDARD\nDTSTART:16010401T010000\nRRULE:FREQ=YEARLY;BYDAY=1SU;BYMONTH=4\nTZOFFSETFROM:+1400\nTZOFFSETTO:+1300\nEND:STANDARD\nBEGIN:DAYLIGHT\nDTSTART:16010930T000000\nRRULE:FREQ=YEARLY;BYDAY=-1SU;BYMONTH=9\nTZOFFSETFROM:+1300\nTZOFFSETTO:+1400\nEND:DAYLIGHT\nEND:VTIMEZONE\n"}},


            };
            return ICalTimeZones;

        }

        #endregion
    }
}
