using System;
using System.Text;

namespace WPFUtilities.Converters
{
    public sealed class NullVisibilityConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int hiddenValue = (int)System.Windows.Visibility.Visible;
            if (parameter is String)
                int.TryParse(parameter as String, out hiddenValue);

            if (value == null)
                return System.Windows.Visibility.Visible;
            else if (hiddenValue == 1)
                return System.Windows.Visibility.Hidden;
            else
                return System.Windows.Visibility.Collapsed;
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
