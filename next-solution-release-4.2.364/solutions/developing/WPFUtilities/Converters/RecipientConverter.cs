using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using WPFUtilities.PropertyDataTemplate;

namespace WPFUtilities.Converters
{
    public class RecipientConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null)
                return string.Empty;

            string uFRole = value.ToString().Split(':').ElementAt(0);
            if (uFRole != null)
                return uFRole;

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
