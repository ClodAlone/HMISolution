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

namespace RangeBaseControl.Converters
{
    public class MajorIntervalConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue || values[2] == DependencyProperty.UnsetValue)
                    return Properties.Settings.Default.MajorIntervalCount;
                if (targetType != typeof(double))
                    return values[0];
                if (!(values[0] is double) || !(values[1] is double) || !(values[2] is double))
                    return values[0];
                if ((double)values[0] > 0)
                    return ((double)values[1] - (double)values[2]) / (double)values[0];
                return values[0];
            }
            catch (Exception)
            {
                return null;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
