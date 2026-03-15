using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Utilities;

namespace LanguagePreferences.Converters
{
    [ValueConversion(typeof(String), typeof(String))]
    internal class LanguageNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(String))
                 throw new InvalidOperationException("The target must be a String");

            var name = value as String;
            var displayName = name;
            if (!String.IsNullOrEmpty(name))
            {
                try
                {
                    var current = new System.Globalization.CultureInfo(name);
                    displayName = current.TextInfo.ToTitleCase(current.NativeName);
                }
                catch (System.Globalization.CultureNotFoundException ex)
                {
                    System.Diagnostics.Debug.WriteLine("Invalid culture name '{0}'", name);
                }
            }
            else
            {
                displayName = Properties.Resources.MatchMicrosoftWindows;
            }

            return displayName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
