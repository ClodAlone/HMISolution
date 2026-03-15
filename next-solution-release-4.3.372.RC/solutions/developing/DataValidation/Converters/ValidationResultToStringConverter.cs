using DataValidation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace DataValidation.Converters
{
    [ValueConversion(typeof(ValidationResults), typeof(String))]
    public class ValidationResultToStringConverter : IValueConverter
    {
        #region IValueConverter
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(String))
                throw new InvalidOperationException("The target must be String");

            if (value == null || !(value is ValidationResults))
                return Properties.Resources.ValidationResults_Unknow;

            switch ((ValidationResults)value)
            {
                case ValidationResults.Successful:
                    return Properties.Resources.ValidationResults_Successful;
                case ValidationResults.Failed:
                    return Properties.Resources.ValidationResults_Failed;
                case ValidationResults.Aborted:
                    return Properties.Resources.ValidationResults_Aborted;
                default:
                    return Properties.Resources.ValidationResults_Unknow;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Static
        public static string Convert(ValidationResults result)
        {
            var converter = new ValidationResultToStringConverter();
            return converter.Convert(result, typeof(String), null, CultureInfo.CurrentUICulture) as String;
        }
        #endregion
    }
}
