using MSModel;
using System;
using System.Globalization;
using System.Threading;
using System.Windows.Data;
using WPFUtilities;
using System.Windows.Media;

namespace MSSchedulerSettings.Converters
{
    public class DayOfWeekToBrushConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (!(values[0] is DayOfWeek))
                    return Binding.DoNothing;
                if ((DayOfWeek)values[0] == DateTime.Now.DayOfWeek)
                    return values[1];
                else
                    return values[2];
            }
            catch (Exception)
            {
                return Binding.DoNothing;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
