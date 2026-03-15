// <copyright file="StringToImageTypeConverter.cs" company="Syncfusion">
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
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Globalization;
using System.Windows.Controls;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Converter class that converts string type into image type.
    /// </summary>
    public sealed class StringToImageTypeConverter : TypeConverter
    {
        #region Public methods
        /// <summary>
        /// Converts the <see cref="string"/> object to the <see cref="CultureInfo"/>.
        /// </summary>
        /// <param name="context">Do not used.</param>
        /// <param name="culture">do not used.</param>
        /// <param name="value">The <see cref="string"/> object to convert.</param>
        /// <returns>The converted value.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            BitmapImage source = new BitmapImage();
            source.BeginInit();
            source.UriSource = new Uri((string)value, UriKind.Relative);
            source.EndInit();

            Image converted = new Image();
            converted.Source = source;

            return converted;
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified
        /// context and culture information.
        /// </summary>
        /// <param name="context">A <see cref="System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">A <see cref="System.Globalization.CultureInfo"/>.</param>
        /// <param name="value">The <see cref="StringToImageTypeConverter.ConvertTo"/>.</param>
        /// <param name="destinationType"> The <see cref="System.Type"/> to convert the value parameter to.</param>
        /// <returns>The converted value.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion
    }
}
