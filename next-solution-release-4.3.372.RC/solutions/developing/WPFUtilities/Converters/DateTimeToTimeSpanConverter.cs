using Mindscape.WpfElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WPFUtilities.Converters
{
    public class DateTimeToTimeSpanConverter : IValueConverter
    {
 
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                DateTime dt = DateTime.MinValue;
                if (value != null && value is DateTime)
                    dt = (DateTime)value;

                return new TimeSpan(dt.Hour, dt.Minute, dt.Second);
            }
            catch 
            { 
            
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (value is TimeSpan)
                {
                    DateTime dt = DateTime.MinValue;
                    return dt.Add(((TimeSpan)value));
                }
                else
                    return null;
            }
            catch (Exception)
            {
            }

            return null;
        }

        #endregion

    }
}
