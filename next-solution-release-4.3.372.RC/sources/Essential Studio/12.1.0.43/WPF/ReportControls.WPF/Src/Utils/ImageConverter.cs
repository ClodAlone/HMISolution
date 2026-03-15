//-------------------------------------------------------------------------------------------------
// <copyright file="ImageConverter.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Syncfusion.RDL.Internal
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

#if WINRT
    using Windows.UI.Xaml.Media.Imaging;
    using Windows.Storage.Streams;
#else
    using System.Windows.Data;
    using System.Windows.Media.Imaging;
#endif

#if WINRT
    internal sealed class Base64ImageConverter
#else
    internal sealed class Base64ImageConverter : IValueConverter
#endif
    {
        /// <summary>
        /// Helper method converts the base 64 string to bitmap image.
        /// </summary>
        /// <param name="value">Base 64 string as object.</param>
        /// <param name="targetType">BitmapImage type.</param>
        /// <param name="parameter">Object parameter.</param>
        /// <param name="culture">Globalization information.</param>
        /// <returns>Bitmap Image</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            byte[] imageData = null;

            string s = value as string;

            if (s != null)
            {
                imageData = System.Convert.FromBase64String(s);
            }
            if (value is byte[])
            {
                imageData = value as byte[];
            }

            if (imageData != null)
            {
                BitmapImage bi = new BitmapImage();
#if WINRT
                try
                {
                byte[] imageBytes = (byte[])value;
                using (var randomAccessStream = new InMemoryRandomAccessStream())
                {
                    var writeStream = randomAccessStream.AsStreamForWrite();
                    writeStream.WriteAsync(imageBytes, 0, imageBytes.Length);
                    writeStream.FlushAsync();
                    randomAccessStream.Seek(0L);
                    bi.SetSource(randomAccessStream);
                }
                }
                catch
                {
                }
#elif SILVERLIGHT
                bi.SetSource(new MemoryStream(imageData));                
#else
                bi.BeginInit();
                bi.StreamSource = new MemoryStream(imageData);
                bi.EndInit();
#endif
                return bi;
            }
            return null;
        }

        /// <summary>
        /// Helper method converts Image to Base 64.
        /// </summary>
        /// <param name="value">Bitmap image.</param>
        /// <param name="targetType">Base 64 string.</param>
        /// <param name="parameter">Object parameter.</param>
        /// <param name="culture">Globalization culture information.</param>
        /// <returns>Base 64 string</returns>
        /// <remarks>Not implemented</remarks>
        /// <exception cref="System.NotImplementedException">System.NotImplementedException</exception>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts the Base 64 string to bitmap image
        /// </summary>
        /// <param name="imageData">Base 64 string.</param>
        /// <returns>Bitmap Image.</returns>
        public BitmapImage ConvertToImage(object imageData)
        {
            return (BitmapImage)this.Convert(imageData, typeof(BitmapImage), null, System.Globalization.CultureInfo.InvariantCulture);
        }

        public BitmapImage CovertByteToImage(object data)
        {
            Byte[] byData = data as Byte[];

            if (byData != null)
            {
                BitmapImage bi = new BitmapImage();
#if WINRT
                try
                {
                byte[] imageBytes = byData;
                using (var randomAccessStream = new InMemoryRandomAccessStream())
                {
                    var writeStream = randomAccessStream.AsStreamForWrite();
                    writeStream.WriteAsync(imageBytes, 0, imageBytes.Length);
                    writeStream.FlushAsync();
                    randomAccessStream.Seek(0L);
                    bi.SetSource(randomAccessStream);
                }
                }
                catch
                {
                }
#elif SILVERLIGHT
                bi.SetSource(new MemoryStream(byData));                
#else
                bi.BeginInit();
                bi.StreamSource = new MemoryStream(byData);
                bi.EndInit();
#endif
                return bi;
            }
            return null;
        }
    }
}
