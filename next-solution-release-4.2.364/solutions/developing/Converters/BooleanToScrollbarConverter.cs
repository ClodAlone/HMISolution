using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
#if !WINDOWS_UWP
using System.Windows.Data;
#else
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
#endif

namespace Converters
{
    public class BooleanToScrollbarConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
#if !WINDOWS_UWP
            System.Globalization.CultureInfo culture)
#else
            String culture)
#endif
        {
            if (value == null)
                return value; 

            return (bool)value ? ScrollBarVisibility.Visible : ScrollBarVisibility.Hidden;
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
    public class BooleanToInvertionScrollbarConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
#if !WINDOWS_UWP
            System.Globalization.CultureInfo culture)
#else
            String culture)
#endif
        {
            return (bool)value ? ScrollBarVisibility.Hidden : ScrollBarVisibility.Visible;
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
