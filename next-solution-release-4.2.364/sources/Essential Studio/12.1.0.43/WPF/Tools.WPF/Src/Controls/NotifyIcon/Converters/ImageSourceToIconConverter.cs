// <copyright file="ImageSourceToIconConverter.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the Image Source To IconConverter class
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ImageSourceToIconConverter : IValueConverter
    {
        /// <summary>
        /// Converts ImageSource to System.Drawing.Icon
        /// </summary>
        /// <param name="value">ImageSource to be converted.</param>
        /// <param name="targetType">Target type of the object</param>
        /// <param name="parameter">Does not matter.</param>
        /// <param name="culture">Currently used culture. Not used here.</param>
        /// <returns>
        /// Converted icon.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ImageSource imIcon = (ImageSource)value;

            if (imIcon == null)
            {
                return null;
            }

            Uri iconUri = new Uri(imIcon.ToString());
            //System.Drawing.Icon icon = new System.Drawing.Icon(Application.GetResourceStream(iconUri).Stream);
            System.Drawing.Bitmap bm = new System.Drawing.Bitmap(Application.GetResourceStream(iconUri).Stream);
            IntPtr Hicon = bm.GetHicon();
            System.Drawing.Icon icon = System.Drawing.Icon.FromHandle(Hicon);
            

            if (icon == null)
            {
                throw new ArgumentException("ImageSource is invalid. Check if image source is .ico file");
            }

            return icon;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
