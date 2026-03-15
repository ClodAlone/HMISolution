using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Utilities;

namespace LanguagePreferences.Converters
{
    [ValueConversion(typeof(String), typeof(System.Windows.FontWeights))]
    internal class LanguageNameBoldConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(System.Windows.FontWeight))
                throw new InvalidOperationException("The target must be a System.Windows.FontWeight");

            var name = value as String;
            var current = LocalizationHelper.ReadCurrentLanguage();
            if (String.IsNullOrEmpty(name) && String.IsNullOrEmpty(current) || current == name)
                return System.Windows.FontWeights.Bold;

            return System.Windows.FontWeights.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
