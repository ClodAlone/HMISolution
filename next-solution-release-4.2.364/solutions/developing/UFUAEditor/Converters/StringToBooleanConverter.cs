using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace UFUAEditor.Converters
{
    /// <summary>
    /// This class simply converts a string to a constant value
    /// return Yes if value is not null and not empty
    /// </summary>
    [ValueConversion(typeof(Boolean), typeof(Visibility))]
    public class StringToBooleanConverter : IValueConverter
    {
        #region IValueConverter implementation
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return !string.IsNullOrEmpty(value?.ToString()) ? Properties.Resources.Yes : Properties.Resources.No;
            }
            catch
            {
                return Properties.Resources.No;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("This method is intentionally not implemented");
        }
        #endregion
    }
}
