using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace AlarmWindow.Converters
{
    public class TimeSpanToDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            try
            {
                TimeSpan delay;
                if (parameter != null && parameter is string)
                {
                    if ((parameter as string) == "TotalDays")
                        delay = TimeSpan.FromDays(System.Convert.ToDouble(value));
                    else if ((parameter as string) == "TotalHours")
                        delay = TimeSpan.FromHours(System.Convert.ToDouble(value));
                    else if ((parameter as string) == "TotalMilliseconds")
                        delay = TimeSpan.FromMilliseconds(System.Convert.ToDouble(value));
                    else if ((parameter as string) == "TotalMinutes")
                        delay = TimeSpan.FromMinutes(System.Convert.ToDouble(value));
                    else
                        delay = TimeSpan.FromSeconds(System.Convert.ToDouble(value));
                }
                else
                    delay = TimeSpan.FromSeconds(System.Convert.ToDouble(value));
                return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, delay.Hours, delay.Minutes, delay.Seconds);
            }
            catch
            {
                return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is DateTime))
                return 0;
            DateTime dateTime = (DateTime)value;
            TimeSpan result = new TimeSpan(dateTime.Hour, dateTime.Minute, dateTime.Second);
            if (parameter != null && parameter is string)
            {
                if ((parameter as string) == "TotalDays")
                    return result.TotalDays;
                else if ((parameter as string) == "TotalHours")
                    return result.TotalHours;
                else if ((parameter as string) == "TotalMilliseconds")
                    return result.TotalMilliseconds;
                else if ((parameter as string) == "TotalMinutes")
                    return result.TotalMinutes;
                else 
                    return result.TotalSeconds;
            }
            else
                return result.TotalSeconds;
        }
    }
}
