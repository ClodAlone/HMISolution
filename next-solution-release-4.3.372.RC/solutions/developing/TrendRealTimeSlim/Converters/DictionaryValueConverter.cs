using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;

namespace TrendRealTimeSlim.Converters
{
    public class DictionaryValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var dict = value as Dictionary<string, double>;
            var key = parameter as String;
            if (dict != null && key != null && dict.ContainsKey(key))
                return Math.Round(dict[key], 2);
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
