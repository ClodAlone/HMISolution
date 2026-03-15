using System;
using System.Linq;
using System.Windows.Data;
using DataReader;

namespace DataReaderEditor.Converters
{
    /// <summary>
    /// A Value converter
    /// </summary>
    public class DataSourceConverter : IValueConverter
    {
        public String projectRoot;

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
            if (value != null)
            {
                if (value is String)
                    return XpoHelpers.XpoHelper.NormalizeConnectionString(value as String, projectRoot);
                DataReaderModelConverter converter = parameter as DataReaderModelConverter;
                try
                {
                    if(value is DataReaderModel)
                    {
                        DataReaderModel uri = value as DataReaderModel;
                        if (converter != null)
                            return XpoHelpers.XpoHelper.NormalizeConnectionString(converter.Convert(uri, typeof(String), parameter, culture) as String, projectRoot);
                        else
                            return XpoHelpers.XpoHelper.NormalizeConnectionString(uri.Connection, projectRoot);
                    }
                    else
                    {
                        DataReaderModelXML uri = value as DataReaderModelXML;
                        if (converter != null)
                            return XpoHelpers.XpoHelper.NormalizeConnectionString(converter.Convert(uri.ReaderModel, typeof(String), parameter, culture) as String, projectRoot);
                        else
                            return XpoHelpers.XpoHelper.NormalizeConnectionString(uri.ReaderModel.Connection, projectRoot);
                    }
                }
                catch
                {
                    return String.Empty;
                }

            }

            return String.Empty;
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
            return value;
        }

        #endregion
    }
}
