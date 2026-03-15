using System;
using System.Windows;
using System.Windows.Data;

namespace TrendRealTimeSlim.Converters
{
    public class OperatingModeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == DependencyProperty.UnsetValue)
                return Visibility.Visible;
            return (TrendRealTimeSlim.RunMode)value == TrendRealTimeSlim.RunMode.RunStop ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
