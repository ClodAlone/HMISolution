using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WizardPluginHelpers.Converters
{
    public class RadioButtonCheckedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is bool))

                throw new ArgumentException("Value must be a boolean");

            return value.ToString().Equals(parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is bool))

                throw new ArgumentException("Value must be a boolean");

            return value.Equals(true) ? Boolean.Parse(parameter.ToString()) : Binding.DoNothing;
        }
    }
}
