using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace MSEditor
{
    public class DateTimeToShortDateStringConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;
            DateTime dateTimeValue = (DateTime)value;

            string param = parameter != null ? parameter.ToString() : string.Empty;
            if (param == string.Empty)
                param = "MM/dd";

            return dateTimeValue.ToString(param);
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
        #endregion
    }
}
