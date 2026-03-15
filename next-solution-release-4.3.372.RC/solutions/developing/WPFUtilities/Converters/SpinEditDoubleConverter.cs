using System;
using System.Globalization;
using System.Windows.Data;

namespace WPFUtilities.Converters
{
    public class SpinEditDoubleConverter : IMultiValueConverter
    {
        bool bBytesMode;
        Type valueType;
        #region IValueConverter
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2)
            {
                bBytesMode = false;
                valueType = values[0].GetType();
                if (values[1] as bool? != true)
                    return values[0];
                else
                {
                    bBytesMode = true;
                    try
                    {
                        var doubleVal = System.Convert.ToDouble(values[0]);
                        return doubleVal / Math.Pow(1024, 2);
                    }
                    catch { }
                }
            }
            return Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            try
            {
                var doubleVal = System.Convert.ToDecimal(value);
                if (bBytesMode)
                    doubleVal = doubleVal * (Decimal)Math.Pow(1024, 2);
                var type = Nullable.GetUnderlyingType(targetTypes[0]);
                if (type == null)
                    type = targetTypes[0];
                return new object[] { System.Convert.ChangeType(doubleVal, type), Binding.DoNothing };
            }
            catch
            {
                return new object[] { double.NaN, Binding.DoNothing };
            }
        }
        #endregion
    }
}
