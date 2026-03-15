using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;

namespace DataValidation.Converters
{
    class DynamicColumnDataExtractor : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null || values.Count() != 2 || values[1] as string == null)
                return string.Empty;
            var propName = values[1] as string;
            var obj = values[0] as IDictionary<string, object>;
            if (obj == null || !obj.ContainsKey(propName))
                return string.Empty;
            return String.Format("{0}", obj[propName]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
