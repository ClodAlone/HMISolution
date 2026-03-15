using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using UFInstallWebClient;

namespace UFInstallWebClient.Converters
{
    /// <summary>
    /// This class simply converts a Boolean to a Visibility
    /// This class is kind of obsolete as there is a Standard 
    /// BooleanToVisibilityConverter within the System.Windows.Controls 
    /// namespace provided with the .NET framework, but you can not 
    /// debug that code. So this ValueConverter
    /// was provided in order that it could be debugger
    /// </summary>
    [ValueConversion(typeof(String), typeof(Boolean))]
    internal class IdentityToBooleanConverter : IValueConverter
    {
        #region IValueConverter implementation
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            String input = value as String;
            Boolean invert = Boolean.Parse(parameter.ToString());

            if (!String.IsNullOrEmpty(input))
                return invert ? false : true;
            return invert ? true : false;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("This method is intentionally not implemented");
        }
        #endregion
    }
}
