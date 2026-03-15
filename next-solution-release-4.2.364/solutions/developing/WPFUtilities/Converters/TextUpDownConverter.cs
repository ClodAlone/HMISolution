using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WPFUtilities.Converters
{
    public class TextUpDownConverter : IValueConverter
    {
        #region Declarations
        String lastText;

        const string MINUS = "-";
        const string PLUS = "+";
        #endregion

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException("The target must be a string");

            bool useFormat = ShowFormattedValue;

            if (!string.IsNullOrEmpty(lastText))
                return lastText;
            else
            {
                if (ShowFormattedValue)
                    lastText = String.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0.##}", value);
                else
                    lastText = String.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}", value);
                return lastText;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is String)
            {
                var prevText = lastText ?? String.Empty;
                lastText = value as String;

                if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    targetType = Nullable.GetUnderlyingType(targetType);

                if (targetType != typeof(Boolean) &&
                    targetType != typeof(String))
                {
                    var min = MinValue;
                    var max = MaxValue;

                    if (Double.IsNaN(min))
                    {
                        if (targetType == typeof(SByte))
                            min = SByte.MinValue;
                        else if (targetType == typeof(Byte))
                            min = Byte.MinValue;
                        else if (targetType == typeof(Int16))
                            min = Int16.MinValue;
                        else if (targetType == typeof(UInt16))
                            min = UInt16.MinValue;
                        else if (targetType == typeof(Int32))
                            min = Int32.MinValue;
                        else if (targetType == typeof(UInt32))
                            min = UInt32.MinValue;
                        else if (targetType == typeof(Int64))
                            min = Int64.MinValue;
                        else if (targetType == typeof(UInt64))
                            min = UInt64.MinValue;
                        else if (targetType == typeof(Decimal))
                            min = (double)Decimal.MinValue;
                        else if (targetType == typeof(Single))
                            min = Single.MinValue;
                        else if (targetType == typeof(Double))
                            min = Double.MinValue;
                    }

                    if (Double.IsNaN(max))
                    {
                        if (targetType == typeof(SByte))
                            max = SByte.MaxValue;
                        else if (targetType == typeof(Byte))
                            max = Byte.MaxValue;
                        else if (targetType == typeof(Int16))
                            max = Int16.MaxValue;
                        else if (targetType == typeof(UInt16))
                            max = UInt16.MaxValue;
                        else if (targetType == typeof(Int32))
                            max = Int32.MaxValue;
                        else if (targetType == typeof(UInt32))
                            max = UInt32.MaxValue;
                        else if (targetType == typeof(Int64))
                            max = Int64.MaxValue;
                        else if (targetType == typeof(UInt64))
                            max = UInt64.MaxValue;
                        else if (targetType == typeof(Decimal))
                            max = (double)Decimal.MaxValue;
                        else if (targetType == typeof(Single))
                            max = Single.MaxValue;
                        else if (targetType == typeof(Double))
                            max = Double.MaxValue;
                    }

                    try
                    {
                        // Ensure value is inside min and max limits.
                        double dbvalue = 0.0;
                        try
                        {
                            dbvalue = System.Convert.ToDouble(lastText, System.Globalization.CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                            if (lastText != null && lastText != MINUS && lastText != PLUS)
                                lastText = prevText;
                        }
                        if (dbvalue < min)
                        {
                            dbvalue = System.Convert.ToDouble(prevText, System.Globalization.CultureInfo.InvariantCulture);
                            if (dbvalue < min)
                                return null;
                            lastText = prevText;
                        }
                        else if (dbvalue > max)
                        {
                            dbvalue = System.Convert.ToDouble(prevText, System.Globalization.CultureInfo.InvariantCulture);
                            if (dbvalue > max)
                                return null;
                            lastText = prevText;
                        }

                        if (targetType == typeof(SByte))
                            return System.Convert.ToSByte(dbvalue);
                        else if (targetType == typeof(Byte))
                            return System.Convert.ToByte(dbvalue);
                        else if (targetType == typeof(Int16))
                            return System.Convert.ToInt16(dbvalue);
                        else if (targetType == typeof(UInt16))
                            return System.Convert.ToUInt16(dbvalue);
                        else if (targetType == typeof(Int32))
                            return System.Convert.ToInt32(dbvalue);
                        else if (targetType == typeof(UInt32))
                            return System.Convert.ToUInt32(dbvalue);
                        else if (targetType == typeof(Int64))
                            return System.Convert.ToInt64(dbvalue);
                        else if (targetType == typeof(UInt64))
                            return System.Convert.ToUInt64(dbvalue);
                        else if (targetType == typeof(Decimal))
                            return System.Convert.ToSingle(dbvalue);
                        else if (targetType == typeof(Single))
                            return System.Convert.ToSingle(dbvalue);
                        else if (targetType == typeof(Double))
                            return System.Convert.ToDouble(dbvalue);
                    }
                    catch { }
                }
                else
                {
                    try
                    {
                        if (targetType == typeof(Boolean))
                            return System.Convert.ToBoolean(value);
                        else if (targetType == typeof(String))
                            return value;
                    }
                    catch { }
                }
            }

            return null;
        }

        #endregion

        #region Properties
        Double minValue = Double.NaN;
        public Double MinValue
        {
            get
            {
                return minValue;
            }
            set
            {
                if (minValue == value)
                    return;
                minValue = value;
            }
        }

        Double maxValue = Double.NaN;
        public Double MaxValue
        {
            get
            {
                return maxValue;
            }
            set
            {
                if (maxValue == value)
                    return;
                maxValue = value;
            }
        }

        bool showFormattedValue = true;
        public bool ShowFormattedValue
        {
            get
            {
                return showFormattedValue;
            }
            set
            {
                if (showFormattedValue == value)
                    return;
                showFormattedValue = value;
            }
        }

        #endregion

        #region Public Methods

        public void Invalidate()
        {
            lastText = null;
        }

        #endregion
    }
}
