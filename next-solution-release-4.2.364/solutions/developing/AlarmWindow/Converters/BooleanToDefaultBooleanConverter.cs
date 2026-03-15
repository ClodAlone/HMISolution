using System;
using System.Collections.Generic;
using System.Windows.Data;

namespace AlarmWindow.Converters
{
    public class BooleanToDefaultBooleanConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || !(value is Boolean))
                return DevExpress.Utils.DefaultBoolean.Default;

            return (bool)value ? DevExpress.Utils.DefaultBoolean.True : DevExpress.Utils.DefaultBoolean.False;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
