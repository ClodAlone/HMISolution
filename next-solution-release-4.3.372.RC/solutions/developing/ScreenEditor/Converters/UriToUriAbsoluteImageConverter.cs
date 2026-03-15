// <copyright file="UriConverter.cs" company="$registerdorganization$">
// Copyright (c) 2010 Microsoft. All Right Reserved
// </copyright>
// <author>Claudio</author>
// <email></email>
// <date>2010-07-23</date>
// <summary>A value converter for WPF and Silverlight data binding</summary>

namespace ScreenManager
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using UFInterfaces.Converters;
    using Utilities;
    using VFS;

    /// <summary>
    /// A Value converter
    /// </summary>
    /// 
    [Obsolete("Use WPFUtilities.Converters.UriToUriAbsoluteImageConverter.")]
    public class UriToUriAbsoluteImageConverter : IUriToUriAbsoluteImageConverter
    {
        #region Converters.IAbsoluteImageConverter
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FileSystemProviderBase FileSystemProviderBase { get; set; }
        public Uri AbsolutePath { get; set; }
        public Uri AbsolutePath2 { get; set; }
        #endregion

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
            var name = parameter as String;
            if (name == null || (AbsolutePath == null && AbsolutePath2 == null))
                return null;

            Uri uri = null;
            try
            {
                uri = new Uri(AbsolutePath, name);
            }
            catch
            { }

            if (targetType == typeof(Uri))
            {
                return uri;
            }
            else if (targetType == typeof(ImageSource))
            {
                if (uri == null && FileSystemProviderBase != null)
                {
                    var vfsFile = new FileManagerFile(FileSystemProviderBase, new FileManagerFolder(FileSystemProviderBase, AbsolutePath.GetPathString()), name);
                    if (!FileSystemProviderBase.Exists(vfsFile))
                        vfsFile = new FileManagerFile(FileSystemProviderBase, new FileManagerFolder(FileSystemProviderBase, AbsolutePath2.GetPathString()), name);
                    if (!FileSystemProviderBase.Exists(vfsFile))
                        return null;

                    try
                    {
                        var img = new BitmapImage();
                        img.BeginInit();
                        img.StreamSource = new MemoryStream(FileSystemProviderBase.ReadFile(vfsFile));
                        img.CacheOption = BitmapCacheOption.OnLoad;
                        img.EndInit();
                        return img;
                    }
                    catch
                    {
                        return null;
                    }
                }

                try
                {
                    var img = new BitmapImage();
                    img.BeginInit();
                    img.UriSource = uri;
                    img.CacheOption = BitmapCacheOption.OnLoad;
                    img.EndInit();
                    return img;
                }
                catch
                {
                    try
                    {
                        uri = new Uri(AbsolutePath2, name);
                        var img = new BitmapImage();
                        img.BeginInit();
                        img.UriSource = uri;
                        img.CacheOption = BitmapCacheOption.OnLoad;
                        img.EndInit();
                        return img;
                    }
                    catch
                    {
                        return null;
                    }
                }
            }
            else
                throw new InvalidOperationException("Target Type must be a Uri or a ImageSource");
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
