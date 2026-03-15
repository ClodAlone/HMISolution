using System;
using System.Globalization;
using System.Windows.Data;

namespace Converters
{
    public class DoubleToPercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is double))
                return "";
            return String.Format("{0}%", ((double)value).ToString("F3", CultureInfo.CurrentCulture));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
