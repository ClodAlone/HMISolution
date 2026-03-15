using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PropertyControl
{
    public class UpDownConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(double) && targetType != typeof(decimal))
                throw new InvalidOperationException("The target must be a double or decimal");

            try
            {
                if (targetType == typeof(decimal))
                    return System.Convert.ToDecimal(value);
                else
                    return System.Convert.ToDouble(value);
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
                if (targetType == typeof(Boolean))
                    return System.Convert.ToBoolean(value);
                else if (targetType == typeof(SByte))
                    return System.Convert.ToSByte(value);
                else if (targetType == typeof(Byte))
                    return System.Convert.ToByte(value);
                else if (targetType == typeof(Int16))
                    return System.Convert.ToInt16(value);
                else if (targetType == typeof(UInt16))
                    return System.Convert.ToUInt16(value);
                else if (targetType == typeof(Int32))
                    return System.Convert.ToInt32(value);
                else if (targetType == typeof(UInt32))
                    return System.Convert.ToUInt32(value);
                else if (targetType == typeof(Int64))
                    return System.Convert.ToInt64(value);
                else if (targetType == typeof(UInt64))
                    return System.Convert.ToUInt64(value);
                else if (targetType == typeof(Decimal))
                    return System.Convert.ToSingle(value);
                else if (targetType == typeof(Single))
                    return System.Convert.ToSingle(value);
                else if (targetType == typeof(Double))
                    return System.Convert.ToDouble(value);
                else if (targetType == typeof(String))
                    return System.Convert.ToString(value, culture);
            }
            catch
            {
            }

            return null;
        }

        #endregion
    }
}
