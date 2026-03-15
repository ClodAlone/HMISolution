using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SQLDatabaseConfiguration.Converters
{
    [ValueConversion(typeof(OperationType), typeof(Visibility))]
    public class OperationTypeToVisibilityConverter : IValueConverter
    {
        #region IValueConverter
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is OperationType))
                return Visibility.Visible;

            var operationType = (OperationType)value;

            var bInvert = false;
            if (parameter != null)
                bInvert = Boolean.Parse(parameter.ToString());

            if (bInvert)
                return operationType != OperationType.None ? Visibility.Visible : Visibility.Collapsed;
            else
                return operationType == OperationType.None ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
