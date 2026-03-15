using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
#if !WINDOWS_UWP
using System.Windows.Data;
#else
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
#endif

namespace Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
#if !WINDOWS_UWP
            System.Globalization.CultureInfo culture)
#else
            String culture)
#endif
        {
            bool bNotVisibleIfNull = false;
            bool bInverse = false;

            int param;
            if (int.TryParse(parameter as String, out param))
            {
                bNotVisibleIfNull = (param & 1) != 0;
                bInverse = (param & 2) != 0;
            }

            if (value == null)
                return bNotVisibleIfNull ? Visibility.Collapsed : value;

            if (bInverse)
                value = !(bool)value;

            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
#if !WINDOWS_UWP
            System.Globalization.CultureInfo culture)
#else
            String culture)
#endif
        {
            throw new NotImplementedException();
        }
    }
    public class BooleanToInvertionVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
#if !WINDOWS_UWP
            System.Globalization.CultureInfo culture)
#else
            String culture)
#endif
        {
            return (bool)value ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
#if !WINDOWS_UWP
            System.Globalization.CultureInfo culture)
#else
            String culture)
#endif
        {
            throw new NotImplementedException();
        }
    }
}
