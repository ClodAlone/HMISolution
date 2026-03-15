using System;
using System.Windows.Data;
using System.IO;
using Utilities;
using System.Globalization;
using DocumentManager.ComponentService;
using System.Windows;

namespace WPFUtilities.Converters
{
    /// <summary>
    /// A Value converter
    /// </summary>
    public class UriToRelativeConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(String))
                throw new InvalidOperationException("The target must be a String");

            if (values != null && values.Length > 1 && values[0] != DependencyProperty.UnsetValue)
            {
                string path;

                Uri uri = values[0] as Uri;
                if(values[1] is IDocument)
                    uri = (values[1] as IDocument).MakeRelativeUri(uri);

                if (uri.IsAbsoluteUri)
                    path = uri.AbsolutePath;
                else
                {
                    path = uri.GetPathString();
                    var index = path.IndexOf('/');
                    if (index == -1)
                        index = path.IndexOf('\\');
                    if (index != -1)
                        path = path.Substring(index + 1);
                }

                string pathGetDirectoryName = Path.GetDirectoryName(path);
                if (String.IsNullOrEmpty(pathGetDirectoryName))
                    return Path.GetFileNameWithoutExtension(path);
                else
                    return String.Format("{0}\\{1}", pathGetDirectoryName, Path.GetFileNameWithoutExtension(path));
            }

            return String.Empty;
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
