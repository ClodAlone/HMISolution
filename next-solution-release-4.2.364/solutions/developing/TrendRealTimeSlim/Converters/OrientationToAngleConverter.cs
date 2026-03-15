using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace TrendRealTimeSlim.Converters
{
    public class OrientationToAngleConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == DependencyProperty.UnsetValue)
                return 0.0;

            if (parameter != null && parameter.ToString() == "1")
                return (TrendRealTimeSlim.TrendMode)value == TrendRealTimeSlim.TrendMode.Horizontal ? 0.0 : -90.0;

            return (TrendRealTimeSlim.TrendMode)value == TrendRealTimeSlim.TrendMode.Horizontal ? 0.0 : 90.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
