using System;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;

namespace Converters
{
    public class ConditionalStyleConverter : IValueConverter
    {
        #region IValueConverter implementation
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("This method is intentionally not implemented");
        }
        #endregion
    }
}
