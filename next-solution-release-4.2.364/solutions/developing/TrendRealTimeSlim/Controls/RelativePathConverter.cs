using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace TrendRealTimeSlim.Controls
{
    public class RelativePathConverter : IMultiValueConverter
    {
        internal string GetRelativePath(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                try
                {
                    Opc.Ua.NamespaceTable n = new Opc.Ua.NamespaceTable();
                    UInt16 ns = (UInt16)(n.Count + 2 - 1);
                    string oldChars = string.Format("{0}:", ns);
                    var relative = string.Format("{0}", (value).Replace(oldChars, ""));
                    return relative;

                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }

            return String.Empty;
        }

        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue)
                return string.Empty;

            string HumanReadeable = values[1].ToString();
            string RelativePath = values[0].ToString();

            string res = string.Empty;
            if (RelativePath is string)
            {
                try
                {
                    res = GetRelativePath(RelativePath.ToString());
                }
                catch (Exception ex)
                {

                }
            }

            return res;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            return new object[] { value.ToString(), Binding.DoNothing };
        }
        #endregion
    }

}
