using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AlarmWindow.Converters
{
    public class TitleToVisibilityCoverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2 || !(values[0] is string) || !(values[1] is string))
                return Visibility.Collapsed;

            string childTitle =(string)values[0];
            string fatherTitle = (string)values[1];

            return string.Equals(childTitle, fatherTitle, StringComparison.OrdinalIgnoreCase) ? Visibility.Collapsed : Visibility.Visible;
        }

       

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        
    }
}
