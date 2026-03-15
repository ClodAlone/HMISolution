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

namespace Gauges.Converters
{
    public class ValueFormatStringConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue)
                return (0.0).ToString(culture);
            try 
	        {	        
                if (values.Length == 1)
                    return System.Convert.ChangeType(values[0], targetType, culture);
                if (values.Length >= 2 && values[0] is IFormattable)
                    return (values[0] as IFormattable).ToString((string)values[1], culture);
	        }
	        catch (Exception)
	        {
                return  (0.0).ToString(culture);
	        }
            return  (0.0).ToString(culture);

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
