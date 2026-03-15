using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace AnimatedObjects.Controls
{
    public class EnumToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value == null)
                return string.Empty;
            if (value is AnimationType)
            {
                List<String> lines = Enum.GetNames(typeof(AnimationType)).ToList();
                return lines.ElementAt((int)value);
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Enum.Parse(typeof(AnimationType), (string)value);
        }
    }
}
