using System;
using System.Windows.Data;
using System.IO;
using Utilities;
using DocumentManager.ComponentService;

namespace WPFUtilities.Converters
{
    /// <summary>
    /// A Value converter
    /// </summary>
    public class UriConverter : IValueConverter
    {
        public IDocument document { get; set; }
        public bool getRenamed { get; set; }
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

            if (value is Uri)
            {
#if !WINDOWS_UWP && !NET_STANDARD
                if (document == null || !getRenamed)
                    return ToString(value as Uri);


                Uri uri = value as Uri;
                Uri renamedUri = document.MakeAbosoluteUri(uri);
                if (uri.IsAbsoluteUri)
                    return ToString(renamedUri);
                return ToString(document.MakeRelativeUri(renamedUri));
#else
                return ToString(value as Uri);
#endif
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
            throw new NotImplementedException();
        }

        #endregion

        #region Static Methods
        public static string ToString(Uri uri, bool removePrefix = false)
        {
            string path;
            if (uri.IsAbsoluteUri)
                path = uri.AbsolutePath;
            else
            {
                path = uri.GetPathString();
                var index = path.IndexOf('/');
                if (index == -1)
                    index = path.IndexOf('\\');
                if (index != -1)
                {
                    path = path.Substring(index + 1);
                    if (removePrefix)
                        return ToString(new Uri(path, UriKind.Relative));
                }
            }

            string pathGetDirectoryName = Path.GetDirectoryName(path);
            if (String.IsNullOrEmpty(pathGetDirectoryName))
                return Path.GetFileNameWithoutExtension(path);
            else
                return String.Format("{0}\\{1}", pathGetDirectoryName, Path.GetFileNameWithoutExtension(path));
        }
        #endregion
    }
}
