using System;
using System.Globalization;
using System.Windows.Data;

namespace Converters
{
    public class MeasureUnitConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string ret = "";
            if (values.Length != 2)
                return ret;

            if (values[1] != null)
            {
                ret = values[1].ToString();
            }
            else
            {
                ret = values[0]?.ToString();
            }

            return ret;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
