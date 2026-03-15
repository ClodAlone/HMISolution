using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace UFUAEditor.Converters
{
    public class ViewsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value != null ? value.ToString().Split(new string[] { ", " }, StringSplitOptions.None).Cast<object>().ToList() : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is List<object>)
                return String.Join(", ", ((List<object>)value).ToArray());
            return string.Empty;
        }
    }
}
