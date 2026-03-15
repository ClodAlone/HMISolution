using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using MSModel;
using WPFUtilities;
using UFInterfaces;
using Utilities.Converters;

namespace MSSchedulerSettings
{
    public class SimpleSchedulingDateToVisual : IMultiValueConverter
    {
        public object Convert(
            object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2 || values[0] == null || values[1] == null)
                return "-";

            var field = parameter as string;
            var type = (CalendarItemType) values[1];
            
            switch (field)
            { 
                case "StartDate":
                    return ((type == CalendarItemType.DateRange || type == CalendarItemType.SingleDate) ?
                        ((DateTime)values[0]).ToString() : (type == CalendarItemType.WeekNDate ? ((DateTime)values[0]).ToLongTimeString() : "-"));
                case "EndDate":
                    return ((type == CalendarItemType.DateRange) ?
                        ((DateTime)values[0]).ToString() : (type == CalendarItemType.WeekNDate ? ((DateTime)values[0]).ToLongTimeString() : "-"));
                case "Month":
                    ResourceEnumConverter converterCalendarMonthType = System.ComponentModel.TypeDescriptor.GetConverter(typeof(CalendarMonthType)) as ResourceEnumConverter;
                    if (converterCalendarMonthType != null)
                        values[0] = converterCalendarMonthType.ConvertTo((CalendarMonthType)values[0], typeof(string)) as String;

                    return ((type == CalendarItemType.WeekNDate) ?
                        values[0] : "-");
                case "WeekOfMonth":
                    ResourceEnumConverter converterCalendarWeekType = System.ComponentModel.TypeDescriptor.GetConverter(typeof(CalendarWeekType)) as ResourceEnumConverter;
                    if (converterCalendarWeekType != null)
                        values[0] = converterCalendarWeekType.ConvertTo((CalendarWeekType)values[0], typeof(string)) as String;

                    return ((type == CalendarItemType.WeekNDate && (values.Count() < 3 || (int)values[2] == 0)) ?
                        //((CalendarWeekType)values[0]).ToString() : "-");
                        values[0] : "-");
                case "DayOfWeek":
                    ResourceEnumConverter converterCalendarDayType = System.ComponentModel.TypeDescriptor.GetConverter(typeof(CalendarDayType)) as ResourceEnumConverter;
                    if (converterCalendarDayType != null)
                        values[0] = converterCalendarDayType.ConvertTo((CalendarDayType)values[0], typeof(string)) as String;

                    return ((type == CalendarItemType.WeekNDate && (values.Count() < 3 || (int)values[2] == 0)) ?
                        values[0] : "-");
                case "DayOfMonth":
                    if (type != CalendarItemType.WeekNDate)
                        return "-";
                    else if ((int)values[0] > 2)
                        return ((int)values[0] - 2).ToString();
                    else
                    {
                        ResourceEnumConverter converterMonthDayAny = System.ComponentModel.TypeDescriptor.GetConverter(typeof(MonthDayAny)) as ResourceEnumConverter;
                        if (converterMonthDayAny != null)
                        {
                            var stringList = ResourceEnumConverter.StringTable;
                            switch ((int)values[0])
                            {
                                case 0:
                                    return TranslationHelpers.TranslationHelper.TranlslateText($"_{MSSchedulerSettings.Controls.NewEvent.stringPlaceolder}_MonthDayAny_NoDate", stringList, Properties.Resources.MonthDayAny_NoDate);
                                case 1:
                                    return TranslationHelpers.TranslationHelper.TranlslateText($"_{MSSchedulerSettings.Controls.NewEvent.stringPlaceolder}_MonthDayAny_First", stringList, Properties.Resources.MonthDayAny_NoDate);
                                case 2:
                                    return TranslationHelpers.TranslationHelper.TranlslateText($"_{MSSchedulerSettings.Controls.NewEvent.stringPlaceolder}_MonthDayAny_Last", stringList, Properties.Resources.MonthDayAny_NoDate);
                                default:
                                    return (int)values[0];
                            }
                        }
                
                        return values[0];
                    }
                default:
                    break;
            }
            return values[0];
        }

        public object[] ConvertBack(
            object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
