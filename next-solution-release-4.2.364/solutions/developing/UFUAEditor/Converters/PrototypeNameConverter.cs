using System;
using System.Globalization;
using System.Windows.Data;

namespace UFUAEditor.Converters
{
    [ValueConversion(typeof(String), typeof(String))]
    public class PrototypeNameConverter : IValueConverter
    {
        #region IValueConverter
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(String))
                throw new InvalidOperationException("The target must be a string");

            var prototypeName = value as String;
            if (prototypeName != null)
            {
                Guid guid;
                if (Guid.TryParse(prototypeName, out guid))
                    return Properties.Resources.PrototypeNameUndefined;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
        #endregion
    }
}
