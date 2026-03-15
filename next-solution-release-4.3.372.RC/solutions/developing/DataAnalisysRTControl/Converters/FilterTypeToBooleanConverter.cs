using System;
using System.Collections.Generic;
using System.Linq;

using System.Windows;
using System.Windows.Data;
using WPFUtilities;

namespace DataAnalisysRTControl.Converters
{
    public class FilterTypeToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(Boolean) && targetType != typeof(Nullable<Boolean>))
                throw new InvalidOperationException("The target must be a Boolean");

            if (!(value is DateSpan) || !(parameter is DateSpan))
                return false;

            return (DateSpan)value == (DateSpan)parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(DateSpan))
                throw new InvalidOperationException("The target must be a WPFUtilities.DateSpan");

            if (value is bool && (bool)value && (parameter is DateSpan))
                return (DateSpan)parameter;

            return DateSpan.None;
        }
    }
}
