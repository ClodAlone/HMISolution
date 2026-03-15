using System;
using System.Windows;
using System.Windows.Data;

namespace DataValidation.Converters
{
    [ValueConversion(typeof(ValidationResults), typeof(Boolean))]
    public class ValidationResultToInvertionBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(Boolean))
                throw new InvalidOperationException("The target must be System.Boolean");

            if (value == null || !(value is ValidationResults))
                return false;

            var result = (ValidationResults)value;
            if (parameter == null || !(parameter is ValidationResults))
                return result == ValidationResults.Successful;

            var checkResult = (ValidationResults)parameter;
            return checkResult != result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

