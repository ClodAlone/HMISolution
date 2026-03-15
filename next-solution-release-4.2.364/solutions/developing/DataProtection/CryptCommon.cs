using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProtection.Common
{
    internal class CryptCommon
    {
        #region Declarations
        internal static byte[] key = Encoding.ASCII.GetBytes("qlvauypwlarpyczxppoeelasstufhkxm");
        internal static char SeparatorCryptedValuesChar = '~';
        static string ReplacedCryptedValuesString = "£$%&";
        #endregion

        #region Methods
        internal static string GetFlatStringValues(IDictionary<string, object> values)
        {
            var clearText = new StringBuilder();
            if (values != null && values.Count > 0)
            {
                foreach (var value in values.Values)
                {
                    if (clearText.Length > 0)
                        clearText.Append(CryptCommon.SeparatorCryptedValuesChar);
                    var sValue = string.Format(CultureInfo.InvariantCulture, "{0}", value);
                    clearText.Append(sValue.Replace(CryptCommon.SeparatorCryptedValuesChar.ToString(), ReplacedCryptedValuesString));
                }
            }

            return clearText.ToString();
        }

        internal static object GetValue(string value, Type type)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            if (type == typeof(DateTime))
                return DateTime.FromBinary(Convert.ToInt64(value, CultureInfo.InvariantCulture));
            else if (type == typeof(TimeSpan))
                return TimeSpan.FromSeconds(Convert.ToDouble(value, CultureInfo.InvariantCulture));
            else if (type == typeof(Guid))
                return Guid.Parse(value);
            else
                return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
        }

        internal static object GetValue(object value)
        {
            if (value != null)
            {
                var type = value.GetType();
                if (type == typeof(DateTime))
                    return ((DateTime)value).ToBinary();
                else if (type == typeof(TimeSpan))
                    return ((TimeSpan)value).TotalSeconds;
            }

            return value;
        }

        internal static bool ValidateValue(object value1, object value2, TimeSpan dateTimeTolerance)
        {
            Type type = null;
            if (value1 != null)
                type = value1.GetType();
            else if (value2 != null)
                type = value2.GetType();

            if (type != null)
            {
                if (type == typeof(String))
                    return ValidateString((String)value1, (String)value2);
                else if (type == typeof(DateTime))
                    return ValidateDateTime((DateTime)value1, (DateTime)value2, dateTimeTolerance);
                else if (type == typeof(TimeSpan))
                    return ValidateTimeSpan((TimeSpan)value1, (TimeSpan)value2, dateTimeTolerance);
                else
                    return CommonValidation(value1, value2);
            }

            return true;
        }

        static bool ValidateDateTime(DateTime dt1, DateTime dt2, TimeSpan dateTimeTolerance)
        {
            if (dt1 == dt2)
                return true;

            var delta = Math.Abs((dt2 - dt1).TotalMilliseconds);
            return delta < dateTimeTolerance.TotalMilliseconds;
        }

        static bool ValidateTimeSpan(TimeSpan dt1, TimeSpan dt2, TimeSpan dateTimeTolerance)
        {
            if (dt1 == dt2)
                return true;

            var delta = Math.Abs((dt2 - dt1).TotalMilliseconds);
            return delta < dateTimeTolerance.TotalMilliseconds;
        }

        static bool ValidateString(string s1, string s2)
        {
            s1 = s1 != null ? s1.Replace(Environment.NewLine, "\n") : string.Empty;
            s2 = s2 != null ? s2.Replace(Environment.NewLine, "\n").Replace(ReplacedCryptedValuesString, CryptCommon.SeparatorCryptedValuesChar.ToString()) : string.Empty;

            return s1 == s2;
        }

        static bool CommonValidation(object s1, object s2)
        {
            if (s1 == s2)
                return true;

            return string.Format(CultureInfo.InvariantCulture, "{0}", s1) == string.Format(CultureInfo.InvariantCulture, "{0}", s2);
        }
        #endregion
    }
}
