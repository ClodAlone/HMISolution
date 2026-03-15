using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DataAnalisysRTControl.Converters
{
    public class OperatingModeToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Visibility) && targetType != typeof(Visibility?))
                throw new InvalidOperationException("The target must be of type System.Windows.Visibility");

            if (values == null || values.Length == 0)
                return Visibility.Collapsed;

            if (!(values[0] is OperatingMode))
                return Visibility.Collapsed;

            if (values.Length == 1 || !(values[1] is bool))
                return (OperatingMode)values[0] != OperatingMode.OnlyStop ? Visibility.Visible : Visibility.Collapsed;


            return (OperatingMode)values[0] != OperatingMode.OnlyStop && (bool)values[1] ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
