using System;
using System.Linq;
using System.Windows.Data;
using System.IO;
using PropertyControl.ComponentService;
using DocumentManager.ComponentService;
using System.Net;
using System.Net.Sockets;
using Utilities;

namespace CommonControls.Converters
{
    /// <summary>
    /// A Value converter
    /// </summary>
    [ValueConversion(typeof(Uri), typeof(String))]
    public class SourceFileConverter : IValueConverter
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
            if (targetType != typeof(String))
                throw new InvalidOperationException("The target must be a String");

            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                if (value is String)
                    return value as String;

                string path;

                if(value is Uri)
                {
                    Uri uri = value as Uri;
                    {
                        if (!uri.IsValidFile())
                            return uri.OriginalString;

                        if (uri.IsAbsoluteUri)
                        {
                            if (!uri.IsFile)
                            {
                                path = uri.OriginalString;
                                return path;
                            }
                            else
                                path = uri.GetPathString();
                        }
                        else
                            path = uri.GetPathString();

                        return path;
                    }
                }
                else
                    return value as String;
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
            Uri uri;
            if (Uri.TryCreate(value as String, UriKind.RelativeOrAbsolute, out uri))
                return uri;

            return null;
        }

        #endregion
    }
}
