using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Converters
{
    public class UpDownValueConverter : IValueConverter
    {
        static readonly String nullString = "(null)";

        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is String && String.Compare(value as String, nullString, true) == 0)
            {
                if (targetType != typeof(String))
                    return Double.NaN;
                return value;
            }

            if (value == null || value is String && String.IsNullOrEmpty(value as String))
                return 0;

            //if (targetType != typeof(Nullable<double>) && targetType != typeof(double))
            //    throw new InvalidCastException();

            if (value is String && string.IsNullOrEmpty(value.ToString()))
                return 0;

            try
            {
                return System.Convert.ToDouble(value);
            }
            catch (Exception ex)
            {
                try
                {
                    return System.Convert.ToBoolean(value);
                }
                catch (Exception exx)
                {
                    try
                    {
                        if (targetType != typeof(String))
                            return value.ToString();
                        else
                            return 0;
                    }
                    catch (Exception exxx)
                    {
                        return value;
                    }
                }
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return String.Empty;

            return String.Format(culture, "{0}", value);
        }
        #endregion
    }
}
