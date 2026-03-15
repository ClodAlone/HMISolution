using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace ScreenManager
{
    [ValueConversion(typeof(double), typeof(double))]
    public class DivideDoubleByParameterConverter : IValueConverter
    {
        #region IValueConverter Membres
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(double))
                throw new InvalidOperationException("The target must be a double");

            double divider = 1;
            if (parameter is double)
                divider = (double)parameter;
            else if (parameter is String)
                divider = System.Convert.ToDouble(parameter as String);

            double d = (double)value;

            return ((double)d) / divider;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        #endregion
    }
}
