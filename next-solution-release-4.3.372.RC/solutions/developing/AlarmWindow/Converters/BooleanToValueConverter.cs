using System;
using System.Collections.Generic;
using System.Linq;

using System.Windows;
using System.Windows.Data;

namespace AlarmWindow.Converters
{
    public class BooleanToValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value ? (string)"*" : (string)"0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
