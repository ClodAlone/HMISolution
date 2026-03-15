using MSModel;
using System;
using System.Globalization;
using System.Threading;
using System.Windows.Data;
using WPFUtilities;

namespace MSSchedulerSettings.Converters
{
    public class InvariantCultureDateConverter : IValueConverter //IMultiValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is DateTime))
                return Binding.DoNothing;

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is String))
                return Binding.DoNothing;

            try
            {
                return DateTime.Parse((String)value, Thread.CurrentThread.CurrentCulture);
            }
            catch
            {
                return Binding.DoNothing;
            }
        }
    }
}
