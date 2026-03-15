using DataValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace DataValidation.Converters
{
    [ValueConversion(typeof(ValidationResults), typeof(Visibility))]
    public class ValidationResultToInvertionVisiblityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(Visibility))
                throw new InvalidOperationException("The target must be System.Windows.Visibility");

            if (value == null || !(value is ValidationResults))
                return Visibility.Collapsed;

            var result = (ValidationResults)value;
            if (parameter == null || !(parameter is ValidationResults))
                return result == ValidationResults.Successful;

            var checkResult = (ValidationResults)parameter;
            return checkResult != result ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

