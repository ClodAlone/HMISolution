using System;

#if WP8
using System.Windows.Data;
using System.Windows;
#else
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
#endif

namespace NextAR.Converters
{
    public class VisibilityConverter : IValueConverter
    {
        #if WP8
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        #else 
        public object Convert(object value, Type targetType, object parameter, string language)
        #endif
        {
            if (value != null)
            {
                if (value is bool)
                {
                    return (bool)value ? Visibility.Visible : Visibility.Collapsed;
                }

                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
