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

namespace UFCrossReferenceEditor
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            try
            {
                if (value == null)
                    return new SolidColorBrush(Colors.Transparent);
                if ((bool)value)
                    return parameter?.ToString() == "1" ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
            }
            catch (Exception)
            {
            }

            return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
