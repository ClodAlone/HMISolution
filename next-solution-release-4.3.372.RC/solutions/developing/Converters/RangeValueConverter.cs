using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Opc.Ua;

namespace Converters
{
    public class RangeValueConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return value;

            if (value is Double)
            {
                if (Double.MinValue == System.Convert.ToDouble(value))
                    return 0d;
                else if (Double.MaxValue == System.Convert.ToDouble(value))
                    return 100d;
                else
                    return value;
            }

            return value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
        #endregion
    }
}
