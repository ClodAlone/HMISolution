using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Opc.Ua;

namespace Converters
{
    public class DoubleToLatitudeDegreesConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return value;
            try
            {
                var dvalue = System.Convert.ToDouble(value.ToString());
                return ParseDoubleToStringValue(dvalue);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ParseStringToDoubleValue(value.ToString());
        }
        #endregion
        private static string ParseDoubleToStringValue(double value)
        {
            if (Double.IsNaN(value))
                return string.Empty;

            double degrees = 0;
            double minutes = 0;
            double seconds = 0;

            string direction = string.Empty;

            try
            {
                direction = value < 0 ? "S" : "N";
                value = Math.Abs(value);
                degrees = Math.Floor(value);
                value = (value - degrees) * 60;
                minutes = Math.Floor(value);
                value = (value - minutes) * 60;
                seconds = value;
            }
            catch (Exception)
            {
            }

            return string.Format("{0:000}° {1:00}' {2:00}'' {3}", (int)degrees, (int)minutes, (int)seconds, direction);
        }
        private static double ParseStringToDoubleValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return double.NaN;

            double ret = double.NaN;
            try
            {
                if (string.IsNullOrEmpty(value) || value.Length != 15)
                    return ret;

                char direction = value.LastOrDefault();
                double degrees = System.Convert.ToDouble(value.Substring(0, 3));
                double minutes = System.Convert.ToDouble(value.Substring(5, 2));
                double seconds = System.Convert.ToDouble(value.Substring(9, 2));

                ret = degrees + minutes / 60 + seconds / 3600;

                if (direction == 'S' || direction == 'W')
                    ret = -ret;
            }
            catch (Exception)
            {
            }

            return ret;
        }

    }
}
