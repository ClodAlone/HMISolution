using Opc.Ua;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Utilities.Converters;

namespace WatchControl.Converters
{
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum DisplayStringFormat
    {
        Default,
        Hex,
        Bin
    }

    public class LocalizedEnumConverter : ResourceEnumConverter
    {
        public LocalizedEnumConverter(Type type)
            : base(type, Properties.Resources.ResourceManager)
        {

        }
    }

    public class NullToEmptyStringMultiValueConverter : IMultiValueConverter
    {
        DisplayStringFormat viewType;
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values[0] == DependencyProperty.UnsetValue)
                return String.Empty;

            if (values[0] == null || values[1] == DependencyProperty.UnsetValue)
            {
                if (values[0] == null)
                    return String.Empty;
                return values[0];
            }

            try
            {
                DataValue dataValue = values[2] as DataValue;
                bool hasDataType = dataValue != null && dataValue.Value != null;

                if (values[0] is bool)
                    return bool.Parse(values[0].ToString());
                decimal value;
                int intvalue;
                string res = (string)values[0];
                //Format viewType = (Format)values[1];
                viewType = (DisplayStringFormat)values[1];
                int padFactor = 1;
                switch (viewType)
                {
                    case DisplayStringFormat.Hex:
                        res = System.Convert.ToString(Int64.Parse(values[0].ToString()), 16).ToUpper();
                        padFactor = 4;
                        break;
                    case DisplayStringFormat.Bin:
                        res = System.Convert.ToString(Int64.Parse(values[0].ToString()), 2);
                        break;
                    default:
                        break;
                }

                if (hasDataType && viewType != DisplayStringFormat.Default)
                {
                    Type type = dataValue.Value.GetType();
                    if (type == typeof(SByte) ||
                        type == typeof(Byte))
                        res = res.PadLeft(8 / padFactor, '0');
                    else if (type == typeof(Int16) ||
                        type == typeof(UInt16))
                        res = res.PadLeft(16 / padFactor, '0');
                    else if (type == typeof(Int32) ||
                        type == typeof(UInt32) ||
                        type == typeof(float))
                        res = res.PadLeft(32 / padFactor, '0');
                    else if (type == typeof(Int64) ||
                        type == typeof(UInt64) ||
                        type == typeof(Double))
                        res = res.PadLeft(64 / padFactor, '0');
                }
                return res;
            }
            catch (Exception ex)
            {
                return values[0];
            }
            return values[0];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return new object[] { String.Empty, DependencyProperty.UnsetValue };
            try
            {
                Type type = targetTypes[0];
                switch (viewType)
                {
                    case DisplayStringFormat.Default:
                        var ret = System.Convert.ChangeType(value, type);
                        return new object[] { ret };
                    case DisplayStringFormat.Hex:
                        var retHex = System.Convert.ToInt64((string)value, 16).ToString();
                        return new object[] { System.Convert.ChangeType(retHex, type) };
                    case DisplayStringFormat.Bin:
                        var retBin = System.Convert.ToInt64((string)value, 2).ToString();
                        return new object[] { System.Convert.ChangeType(retBin, type) };
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                return new object[] { value };
            }

            return new object[] { value };
        }
    }
}
