using System;
using System.Collections.Generic;
using System.Linq;

using System.Windows;
using System.Windows.Data;

namespace Converters
{
    public class DateFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern; 

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
