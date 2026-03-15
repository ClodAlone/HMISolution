using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TrendRealTimeSlim.Converters
{
    public class DoublePrecisionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string ret = "";
            if (values.Length != 3 || !(values[0] is double) || !(values[1] is int) || !(values[2] is int))
                return ret;
            try
            {
                var precision = int.Parse(values[2].ToString()) == -1 ? values[1] : values[2];

                ret = ((double)values[0]).ToString(String.Format("F{0}", precision), CultureInfo.CurrentCulture);
                ret = ret.Contains(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator) ? ret.TrimEnd('0').TrimEnd(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.ToCharArray()) : ret;
            }
            catch { }
            return ret;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
