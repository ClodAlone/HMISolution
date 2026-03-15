using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Opc.Ua;

namespace Converters
{
    public class QualityToBooleanConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return value;
            return (value as string).Equals(StatusCodes.GetBrowseName(StatusCodes.Good)) 
                || (value as string).Equals(StatusCodes.GetBrowseName(StatusCodes.Uncertain)) 
                || (value as string).Equals(StatusCodes.GetBrowseName(StatusCodes.BadTimeout))
                || (value as string).Equals(StatusCodes.GetBrowseName(StatusCodes.BadCommunicationError))
                ? true : false;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
        #endregion
    }
}
