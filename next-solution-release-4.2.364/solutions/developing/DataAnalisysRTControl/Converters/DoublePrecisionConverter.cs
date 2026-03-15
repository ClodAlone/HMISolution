using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DataAnalisysRTControl.Converters
{
    public class DoublePrecisionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string ret = "";
            if (values.Length != 2 || !(values[0] is double) || !(values[1] is int))
                return ret;
            if (double.IsNaN((double)values[0]))
                return ret;
            try
            {
                ret = ((double)values[0]).ToString(String.Format("F{0}", values[1]), CultureInfo.CurrentCulture);
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
