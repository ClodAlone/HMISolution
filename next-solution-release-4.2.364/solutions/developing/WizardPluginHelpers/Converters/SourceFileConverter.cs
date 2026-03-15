using System;
using System.Windows.Data;
using System.IO;

namespace WizardPluginHelpers.Converters
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

                Uri uri = value as Uri;
                if (uri.IsAbsoluteUri)
                    path = uri.AbsolutePath;
                else
                    path = uri.OriginalString;

                string pathGetDirectoryName = Path.GetDirectoryName(path);
                if (String.IsNullOrEmpty(pathGetDirectoryName))
                    return Path.GetFileName(path);
                else
                    return String.Format("{0}\\{1}", pathGetDirectoryName, Path.GetFileName(path));
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
            if (targetType != typeof(Uri))
                throw new InvalidOperationException("The target must be a Uri");

            Uri uri;
            if (Uri.TryCreate(value as String, UriKind.RelativeOrAbsolute, out uri))
                return uri;

            return null;
        }

        #endregion
    }
}
