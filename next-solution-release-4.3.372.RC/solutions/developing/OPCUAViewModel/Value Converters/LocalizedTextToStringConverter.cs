using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using Opc.Ua;

namespace OPCUAViewModel
{
    /// <summary>
    /// This class simply converts a Boolean to a Visibility
    /// This class is kind of obsolete as there is a Standard 
    /// BooleanToVisibilityConverter within the System.Windows.Controls 
    /// namespace provided with the .NET framework, but you can not 
    /// debug that code. So this ValueConverter
    /// was provided in order that it could be debugger
    /// </summary>
    [ValueConversion(typeof(LocalizedText), typeof(String))]
    public class LocalizedTextToStringConverter : IValueConverter
    {
        #region IValueConverter implementation
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(String) || !(value is LocalizedText))
                return null;

            LocalizedText input = value as LocalizedText;

            return input.Text;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("This method is intentionally not implemented");
        }
        #endregion
    }
}
