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
    using System.IO;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Utilities;

    /// <summary>
    /// A Value converter
    /// </summary>
    public class UriToUriImageConverter : IValueConverter
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
            if (value == null)
                return null;

            if (!(value is Uri))
                throw new InvalidOperationException("Value must be a Uri");

            if (targetType == typeof(Uri))
            {
                var file = System.IO.Path.ChangeExtension((value as Uri).GetPathString(), "png");
                if (!File.Exists(file))
                    // throw new InvalidOperationException(String.Format("Cannot find the image file {0} to convert from", file));
                    return null;
                return new Uri(file);
            }
            else if (targetType == typeof(ImageSource))
            {
                var file = System.IO.Path.ChangeExtension((value as Uri).GetPathString(), "png");
                if (!File.Exists(file))
                    // throw new InvalidOperationException(String.Format("Cannot find the image file {0} to convert from", file));
                    return null;
                var img = new BitmapImage();
                try
                {
                    img.BeginInit();
                    img.UriSource = new Uri(file);
                    img.CacheOption = BitmapCacheOption.OnLoad;
                    img.EndInit();
                }
                catch (Exception ex)
                {

                }

                return img;
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
