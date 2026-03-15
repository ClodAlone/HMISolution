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
    internal class ActualWidthToSpinButtonWidthConvertercs : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values[1] is Thickness borderThikness && values[0] is double actualwidrth ? actualwidrth - (borderThikness.Left + borderThikness.Right) :
                                                                                             0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
