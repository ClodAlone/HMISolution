using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Converters
{
    public class TextValueConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return value;

            if (targetType != typeof(String))
                return value;

            try
            {
                return String.Format(culture, "{0}", value);
            }
            catch (Exception ex)
            {
                try
                {
                    return value;
                }
                catch
                {
                    return null;
                }
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return String.Empty;

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
                        return value;
                    }
                    catch
                    {
                        return null;
                    }
                }
            }

        }
        #endregion
    }
}
