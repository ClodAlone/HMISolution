using Opc.Ua;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using TranslationHelpers;

namespace DataloggerViewerControl.Converters
{
    public class EventMessageConverter : IMultiValueConverter
    {
        #region IValueConverter Members
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null || values.Length == 0 || values[0] as String == null)
                return String.Empty;

            if (values.Length < 2 || values[1] == null || !(values[1] is IDictionary<string, string>))
                return values[0];

            var stringlist = values[1] as IDictionary<string, string>;
            var translatedMessage = stringlist.ContainsKey(values[0] as String) ? stringlist[values[0] as String] : values[0] as String;
            if (values.Length > 2 &&
                (values[2] is String &&
                (values[2] as String).Length > 2 && (values[2] as String)[0] == '{' &&
                (values[2] as String)[(values[2] as String).Length - 1] == '}'))
            {
                var stringValues = (values[2] as String).Substring(1, (values[2] as String).Length - 2).Split(new string[] { " |" }, StringSplitOptions.None);
                for (int ii = 0; ii < stringValues.Length; ii++)
                {
                    var fmt = String.Format("{{{0}}}", ii);
                    try
                    {
                        var dValue = System.Convert.ToDouble(stringValues[ii], System.Globalization.CultureInfo.InvariantCulture);
                        translatedMessage = translatedMessage.Replace(fmt, dValue.ToString(System.Globalization.CultureInfo.CurrentCulture));
                    }
                    catch
                    {
                        translatedMessage = translatedMessage.Replace(fmt, stringValues[ii]);
                    }
                }
            }
            return translatedMessage;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[] { value };
        }
        #endregion
    }
}