using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace UFUAEditor.Converters
{
    public class AlarmThresholdNodeNameConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string tagName = null;
            string expression = null;
            string alrName = null;

            if (values != null)
            {
                if (values.Length > 0)
                    tagName = values[0] as String;
                if (values.Length > 1)
                    expression = values[1] as String;
                if (values.Length > 2)
                    alrName = values[2] as String;
            }

            if (tagName == null || alrName == null)
                return UFUAModel.Properties.Resources.InvalidThresholdText;

            var suffix = new System.Text.StringBuilder(tagName);
            if (!String.IsNullOrEmpty(expression))
                suffix.Append(expression);
            suffix.AppendFormat(":{0}", alrName);

            return suffix.ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
