using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using ScreenParameterSettings;
using Utilities;

namespace ScreenParametersEditor.Converters
{
    public class ParameterItemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (String.IsNullOrEmpty(value as String))
                return string.Empty;

            return NamespaceTableConverter.GetSanitizedReadableValue(value as String);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(String))
                throw new InvalidOperationException("The target must be a String");

            var par = value as string;
            if (String.IsNullOrEmpty(par))
                return string.Empty;

            if (!par.StartsWith(NamespaceTableConverter.FormattedNsChars))
                par = par.Insert(0, NamespaceTableConverter.FormattedNsChars);

            int index = -1;
            while (true)
            {
                index = par.IndexOf("/", index + 1);
                if (index == -1)
                    break;
                if (par.IndexOf(string.Format("/{0}", NamespaceTableConverter.FormattedNsChars), index) != index)
                {
                    par = par.Insert(index + 1, NamespaceTableConverter.FormattedNsChars);
                    index += NamespaceTableConverter.FormattedNsChars.Length;
                }
            }

            return par;
        }
    }
}
