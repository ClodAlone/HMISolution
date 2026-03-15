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
    public class BitMaskConverter : IValueConverter
    {
        BitMask target;

        public BitMaskConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                target = BitMask.Default;
                if (Nullable.GetUnderlyingType(targetType) != null)
                    return null;
                else
                    return false;
            }

            BitMask mask = (BitMask)parameter;
            target = (BitMask)Enum.ToObject(typeof(BitMask), value);
            return ((mask & target) != 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null && Nullable.GetUnderlyingType(targetType) != null)
                return null;

            BitMask mask = (BitMask)parameter;
            bool bValue = System.Convert.ToBoolean(value);
            return GetNewVisibilityLevel(mask, target, bValue);
        }

        static public uint GetNewVisibilityLevel(BitMask mask, BitMask target, bool bAdd)
        {
            if (bAdd)
                return (uint)(mask | target);
            else
                return (uint)(mask ^ target);
        }
    }
}
