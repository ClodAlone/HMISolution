using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using UFUAModel;

namespace UFUAEditor.Converters
{
    [ValueConversion(typeof(AlarmType?), typeof(Visibility))]
    public class TripAlarmTypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;

            if (targetType != typeof(Visibility))
                throw new InvalidOperationException("The target must be a System.Windows.Visibility");

            var alarmType = value as AlarmType?;
            if (alarmType != null)
            {
                bool invert = false;
                if (parameter != null)
                    invert = Boolean.Parse(parameter.ToString());

                if (alarmType == AlarmType.TripAlarm)
                    return invert ? Visibility.Collapsed : Visibility.Visible;
                else
                    return invert ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
