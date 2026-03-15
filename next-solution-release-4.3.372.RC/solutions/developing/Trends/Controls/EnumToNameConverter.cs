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

namespace Trends.Controls
{
    public class EnumToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            
            if(value == null)
                return string.Empty;
            if (value is PredefinedPenKinds)
            {
                List<String> lines = Enum.GetNames(typeof(PredefinedPenKinds)).ToList();
                return lines.ElementAt((int)value);
            }
            else if (value is ViewMode)
            {
                List<String> lines = Enum.GetNames(typeof(ViewMode)).ToList();
                return lines.ElementAt((int)value);
            }
            else
                return string.Empty;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Enum.Parse(typeof(PredefinedPenKinds), (string)value);
        }
    }
}
