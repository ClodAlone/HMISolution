using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace WPFUtilities.Controls
{
    public class ValueFormatter : MarkupExtension, IMultiValueConverter {
        public ValueFormatter() { }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            var format = "{0:" + String.Format("{0}", values[1]) + "}";
            string result = "ERROR";
            try { result = string.Format(CultureInfo.InvariantCulture, format, new object[] { values[0] }); }
            catch { }

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }

    }
}
