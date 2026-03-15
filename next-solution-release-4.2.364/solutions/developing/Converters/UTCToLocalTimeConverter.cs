
using System;
using System.Windows.Data;
using System.IO;
using Utilities;
using System.Collections;

namespace Converters
{
    /// <summary>
    /// A Value converter
    /// </summary>
    public class UTCToLocalTimeConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI. 
        /// </summary>
        /// <param name="value">The source data being passed to the target </param>
        /// <param name="targetType">The Type of data expected by the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>The value to be passed to the target dependency property. </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (value != null && value is DateTime)
                {
                    if ((DateTime)value <= new DateTime(2, 1, 1))
                        return null;
                    if (parameter is int)
                    {
                        var timeZoneOffset = (int)parameter;
                        return ((DateTime)value).ToUniversalTime().AddMinutes(timeZoneOffset).ToString(System.Globalization.CultureInfo.CurrentCulture);
                    }
                    else
                    {
                        bool bOnlyFormat = parameter is bool && (bool)parameter;
                        return bOnlyFormat ? ((DateTime)value).ToString(System.Globalization.CultureInfo.CurrentCulture) : ((DateTime)value).ToLocalTime().ToString(System.Globalization.CultureInfo.CurrentCulture);
                    }
                }

                return value;
            }
            catch
            {
                return value;
            }
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object. This method is called only in TwoWay bindings. 
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The Type of data expected by the source object.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic. </param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>The value to be passed to the source object.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
