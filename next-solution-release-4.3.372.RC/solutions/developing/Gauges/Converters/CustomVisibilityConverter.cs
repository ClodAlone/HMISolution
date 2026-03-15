using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace Gauges.Converters
{
    public class CustomVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                return typeof(DevExpress.Xpf.Gauges.SymbolState).GetProperty("Symbol", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(value);
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
