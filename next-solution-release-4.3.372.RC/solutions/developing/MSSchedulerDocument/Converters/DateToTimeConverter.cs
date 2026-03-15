using MSModel;
using System;
using System.Globalization;
using System.Threading;
using System.Windows.Data;
using WPFUtilities;

namespace MSSchedulerSettings.Converters
{
    public class DateToTimeConverter : IValueConverter //IMultiValueConverter
    {
        //CalendarDayType dayOfWeek;
        //CalendarItemType calItemType;
        //DateTime otherDate;
        //bool bIsWeeklyPlanItem;

        DateTime? inputDate;
        public bool IsWeekNDate { get; set; }
        public bool IsWeeklyPlan { get; set; }

        public DateToTimeConverter()
        {

        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            inputDate = null;
            if (!IsWeekNDate || !(value is DateTime))
                return Binding.DoNothing;
            inputDate = (DateTime)value;
            return ((DateTime)value).ToString("HH:mm:ss");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var doNothing = Binding.DoNothing;

            if (!IsWeekNDate || inputDate == null)
                return doNothing;

            String val;
            var clt = CultureInfo.InvariantCulture;

            if (!String.IsNullOrEmpty(value as String))
            {
                val = value as String;
                if (!IsWeeklyPlan)
                {
                    var today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    val = String.Format("{0} {1}", today.ToString("d", clt), val);
                }
                else
                {
                    switch (((DateTime)inputDate).DayOfWeek)
                    {
                        case DayOfWeek.Sunday:
                            val = String.Format("{0} {1}", new DateTime(1991, 1, 13).ToString("d", clt), val);
                            break;
                        case DayOfWeek.Tuesday:
                            val = String.Format("{0} {1}", new DateTime(1991, 1, 8).ToString("d", clt), val);
                            break;
                        case DayOfWeek.Wednesday:
                            val = String.Format("{0} {1}", new DateTime(1991, 1, 9).ToString("d", clt), val);
                            break;
                        case DayOfWeek.Thursday:
                            val = String.Format("{0} {1}", new DateTime(1991, 1, 10).ToString("d", clt), val);
                            break;
                        case DayOfWeek.Friday:
                            val = String.Format("{0} {1}", new DateTime(1991, 1, 11).ToString("d", clt), val);
                            break;
                        case DayOfWeek.Saturday:
                            val = String.Format("{0} {1}", new DateTime(1991, 1, 12).ToString("d", clt), val);
                            break;
                        case DayOfWeek.Monday:
                        default:
                            val = String.Format("{0} {1}", new DateTime(1991, 1, 7).ToString("d", clt), val);
                            break;
                    }
                }
            }
            else
                return doNothing;

            DateTime date;
            if (!DateTime.TryParseExact(val, "MM/dd/yyyy HH:mm:ss", clt, DateTimeStyles.None, out date))
                return doNothing;
            return date;
        }

        //public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        //{
        //    if (values.Length < 5 || !(values[0] is DateTime) || !(values[1] is DateTime) || !(values[2] is CalendarDayType) || (!(values[3] is CalendarItemType)) || !(values[4] is bool))
        //        return Binding.DoNothing;
        //    bIsWeeklyPlanItem = (bool)values[4];
        //    calItemType = (CalendarItemType)values[3];
        //    if (calItemType != CalendarItemType.WeekNDate)
        //        return Binding.DoNothing;
        //    otherDate = (DateTime)values[1];
        //    dayOfWeek = (CalendarDayType)values[2];
        //    return ((DateTime)values[0]).ToString("HH:mm:ss");
        //}

        //public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        //{
        //    var doNothing = new object[] { Binding.DoNothing, Binding.DoNothing, Binding.DoNothing, Binding.DoNothing, Binding.DoNothing };
        //    if (calItemType != CalendarItemType.WeekNDate || !bIsWeeklyPlanItem)
        //            return doNothing;

        //    String val;
        //    var clt = CultureInfo.InvariantCulture;

        //    if (!String.IsNullOrEmpty(value as String))
        //    {
        //        val = value as String;
        //        switch (dayOfWeek)
        //        {
        //            case CalendarDayType.Sunday:
        //                val = String.Format("{0} {1}", new DateTime(1991, 1, 13).ToString("d", clt), val);
        //                break;
        //            case CalendarDayType.Tuesday:
        //                val = String.Format("{0} {1}", new DateTime(1991, 1, 8).ToString("d", clt), val);
        //                break;
        //            case CalendarDayType.Wednesday:
        //                val = String.Format("{0} {1}", new DateTime(1991, 1, 9).ToString("d", clt), val);
        //                break;
        //            case CalendarDayType.Thursday:
        //                val = String.Format("{0} {1}", new DateTime(1991, 1, 10).ToString("d", clt), val);
        //                break;
        //            case CalendarDayType.Friday:
        //                val = String.Format("{0} {1}", new DateTime(1991, 1, 11).ToString("d", clt), val);
        //                break;
        //            case CalendarDayType.Saturday:
        //                val = String.Format("{0} {1}", new DateTime(1991, 1, 12).ToString("d", clt), val);
        //                break;
        //            case CalendarDayType.Monday:
        //            default:
        //                val = String.Format("{0} {1}", new DateTime(1991, 1, 7).ToString("d", clt), val);
        //                break;
        //        }
        //    }
        //    else
        //        return doNothing;

        //    DateTime date;
        //    if (!DateTime.TryParseExact(val, "MM/dd/yyyy HH:mm:ss", clt, DateTimeStyles.None, out date))
        //        return doNothing;
        //    var newOtherDate = new DateTime(date.Year, date.Month, date.Day, otherDate.Hour, otherDate.Minute, otherDate.Second);
        //    return new object[] { date, newOtherDate, Binding.DoNothing, Binding.DoNothing, Binding.DoNothing };
        //}
    }
}
