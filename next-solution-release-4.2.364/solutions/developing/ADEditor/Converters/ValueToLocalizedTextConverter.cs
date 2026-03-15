using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace ADEditor.Converters
{
    /// <summary>
    /// This class simply converts a Boolean to a Visibility
    /// This class is kind of obsolete as there is a Standard 
    /// BooleanToVisibilityConverter within the System.Windows.Controls 
    /// namespace provided with the .NET framework, but you can not 
    /// debug that code. So this ValueConverter
    /// was provided in order that it could be debugger
    /// </summary>
    [ValueConversion(typeof(string), typeof(string))]
    public class ValueToLocalizedTextConverter : IMultiValueConverter
    {
        #region IValueConverter implementation

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Count() == 0)
                return null;
            try
            {
                if(values.Count() >= 2)
                {
                    string displayName = GetLocalizedText((values[1] as string).Replace(".dll", $"_{(parameter as string)}"), values[0] as string);
                    if (!string.IsNullOrEmpty(displayName))
                        return displayName;
                    return values[0];
                }
                return values[0];
            }
            catch
            {
                return values[0];
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        string GetLocalizedText(string value, string defaultValue)
        {
            string _value = Properties.PluginDescriptionResources.ResourceManager.GetString(value);
            return !string.IsNullOrEmpty(_value) ? _value : defaultValue;
        }
        #endregion
    }
}
