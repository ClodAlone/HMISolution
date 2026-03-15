using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using OPCUAViewModel;

namespace ReportParameters.Converters
{
    [ValueConversion(typeof(OPCUAEntityReference), typeof(String))]
    public class OPCUAEntityReferenceDisplayNameConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(String))
                throw new InvalidOperationException("The target must be a String");

            var entity = value as OPCUAEntityReference;
            if (entity != null)
            {
                return entity.HumanReadable;
            }

            return String.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
