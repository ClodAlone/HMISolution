using System;
using System.Collections.Generic;
using System.Linq;

using System.Windows;
using System.Windows.Data;

namespace UFSolutionNext
{
    public class ThemeKeyBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var currentKey = value as String;
            var checkedKey = parameter as String;
            if (String.IsNullOrEmpty(currentKey) || String.IsNullOrEmpty(checkedKey))
                return null;

            bool bInverse = false;
            if (checkedKey.StartsWith("!"))
            {
                bInverse = true;
                checkedKey = checkedKey.Substring(1);
            }
            
            return bInverse ? currentKey != checkedKey : currentKey == checkedKey;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
