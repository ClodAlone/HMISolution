using MSModel;
using System;
using System.Globalization;
using System.Threading;
using System.Windows.Data;
using WPFUtilities;

namespace MSSchedulerSettings.Converters
{
    public class RowHeightConverter : IValueConverter //IMultiValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is double))
                return Binding.DoNothing;
            try
            {
                return (double)value / 7 - 3.6;
            }
            catch (Exception)
            {
                return Binding.DoNothing;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
