
using System;
using System.Windows.Data;
using System.IO;
using Utilities;
using System.Collections;

namespace WPFUtilities.Converters
{
    /// <summary>
    /// A Value converter
    /// </summary>
    public class ItemsCountConverter : IValueConverter
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
                int counter = 0;
                if (value != null)
                {
                    if (value is IList)
                    {
                        IList uri = value as IList;
                        if (uri != null)
                            counter = uri.Count;
                    }
                    else if (value is String && parameter is Char)
                    {
                        var separator = (Char)parameter;
                        var text = value as String;
                        if (!String.IsNullOrEmpty(text))
                            counter = text.Split(separator).Length;
                    }
                    else
                        return String.Format(Properties.Resources.ItemsCountFormat, "?");
                }

                return String.Format(Properties.Resources.ItemsCountFormat, counter);
            }
            catch
            {
                return String.Format(Properties.Resources.ItemsCountFormat, "?");
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
