using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PropertyControl.Converters
{
    public class BitMaskConverter : IValueConverter
    {
        BitMask target;

        public BitMaskConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BitMask mask = (BitMask)parameter;
            target = (BitMask)Enum.ToObject(typeof(BitMask), value);
            return ((mask & target) != 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BitMask mask = (BitMask)parameter;
            bool bValue = System.Convert.ToBoolean(value);
            if (bValue)
                return (uint)(mask | target);
            else
                return (uint)(mask ^ target);
        }
    }
}
