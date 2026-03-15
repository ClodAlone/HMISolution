using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace AlarmWindow.Converters
{
    public class RowHeightConverter : IMultiValueConverter 
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue)
                return "Auto";

            var rowHeight = (int)values[0];

            if (rowHeight == 0)
                return "Auto";
            else
            {
                var fontSize = (int)values[1];
                if (fontSize * System.Convert.ToDouble(Properties.Settings.Default.FontSizeHeightFactor) >= rowHeight)
                {
                    return "Auto";
                }
                else
                {
                    return (double)rowHeight;
                }
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}