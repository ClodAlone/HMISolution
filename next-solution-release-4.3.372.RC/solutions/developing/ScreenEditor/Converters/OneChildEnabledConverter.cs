using DevExpress.Xpf.Bars;
using System;
using System.Windows.Data;

namespace ScreenManager
{
    public class OneChildEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var items = value as CommonBarItemCollection;
            if (items == null)
                return false;
            foreach (var item in items)
            {
                if ((item as BarItem)?.IsEnabled == true)
                    return true;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
