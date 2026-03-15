using DataValidation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace DataValidation.Converters
{
    [ValueConversion(typeof(ValidationResults), typeof(Brush))]
    public class ValidationResultToColorConverter : IValueConverter
    {
        #region IValueConverter
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Brush))
                throw new InvalidOperationException("The target must be System.Windows.Media.Brush");

            if (value == null || !(value is ValidationResults))
                return Properties.Resources.ValidationResults_Unknow;

            switch ((ValidationResults)value)
            {
                case ValidationResults.Successful:
                    return new SolidColorBrush(Colors.Green);
                case ValidationResults.Failed:
                    return new SolidColorBrush(Colors.Red);
                default:
                    return new SolidColorBrush(Colors.Gray);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Static
        public static SolidColorBrush Convert(ValidationResults result)
        {
            var converter = new ValidationResultToColorConverter();
            return converter.Convert(result, typeof(Brush), null, CultureInfo.CurrentUICulture) as SolidColorBrush;
        }
        #endregion
    }
}
