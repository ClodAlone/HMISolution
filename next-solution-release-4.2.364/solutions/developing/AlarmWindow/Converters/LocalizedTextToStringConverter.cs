using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Opc.Ua;

namespace AlarmWindow.Converters
{
    
    [ValueConversion(typeof(LocalizedText), typeof(String))]
    public class LocalizedTextToStringConverter : IValueConverter
    {
        #region IValueConverter implementation
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (targetType != typeof(String) || !(value is LocalizedText))
            //    return null;
            if (value == null || value == DependencyProperty.UnsetValue)
                return null;

            LocalizedText input = value as LocalizedText;

            if (input == null)
                return null;
            
            return input.Text;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (!(targetType is LocalizedText)  || (value != typeof(String)))
            //    return null;
            if (value == null)
                return null;

            LocalizedText input = new LocalizedText(culture.Name, value.ToString());

            return input;
        }
        #endregion
    }
}
