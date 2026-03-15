using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows;

namespace DataReaderEditor.Converters
{
    /// <summary>
    /// This class simply converts a Boolean to a Visibility
    /// This class is kind of obsolete as there is a Standard 
    /// BooleanToVisibilityConverter within the System.Windows.Controls 
    /// namespace provided with the .NET framework, but you can not 
    /// debug that code. So this ValueConverter
    /// was provided in order that it could be debugger
    /// </summary>
    [ValueConversion(typeof(Boolean), typeof(Visibility))]
    public class BoolToVisibilityConverter : IValueConverter
    {
        #region IValueConverter implementation
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Boolean input = (Boolean)value;

            if (parameter != null)
            {
                Boolean invert = Boolean.Parse(parameter.ToString());
                if (invert)
                    input = !input;
            }

            if (input)
                return Visibility.Visible;
            return Visibility.Collapsed;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("This method is intentionally not implemented");
        }
        #endregion
    }
}
