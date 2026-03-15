using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;

namespace UnitConverterManager.Converters
{
    public class TextToVisibilityConverter : IValueConverter
    {
        const String NameMacth = @"^[a-zA-Z][a-zA-Z0-9_]*$";
        public static bool IsNotValidName(string name)
        {
            if (String.IsNullOrEmpty(name))
                return true;

            return !Regex.IsMatch(name, NameMacth);
        }
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return Visibility.Visible; 

            return IsNotValidName(value.ToString()) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
   
}
