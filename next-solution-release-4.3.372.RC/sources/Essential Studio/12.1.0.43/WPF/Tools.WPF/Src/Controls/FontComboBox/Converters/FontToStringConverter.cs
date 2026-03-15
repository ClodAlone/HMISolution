// <copyright file="FontToStringConverter.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using Syncfusion.Windows.Tools.Controls;

namespace Syncfusion.Windows.Tools
{
    /// <summary>
    /// Converts FontFamily to string objects.
    /// </summary>
    /// <remarks>
    /// Converts <see cref="System.Windows.Media.FontFamily"/> objects
    /// to <see cref="string"/> objects.
    /// </remarks>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class FontToStringConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts <see cref="System.Windows.Media.FontFamily"/> object
        /// to <see cref="string"/> object.
        /// </summary>
        /// <param name="value">FontFamily object to be converted.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">Does not matter.</param>
        /// <param name="culture">Currently used culture.</param>
        /// <exception cref="ArgumentException">The value parameter is not
        /// of type <see cref="System.Windows.Media.FontFamily"/>.
        /// </exception>
        /// <returns>A converted value.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }

            if (!(value is FontFamily))
            {
                throw new ArgumentException("Value of FontFamily type expected", "value");
            }

            XmlLanguage userLanguage = XmlLanguage.GetLanguage("en-US");

            FontFamily font = (FontFamily)value;

            if (font is Syncfusion.Windows.Tools.Controls.ThemeFontFamily)
            {
                ThemeFontFamily themeFont = (ThemeFontFamily)font;
                return themeFont.FamilyNames[userLanguage] + " " + themeFont.Purpose;
            }

            return font.FamilyNames[userLanguage];
        }

        /// <summary>
        /// Converts <see cref="string"/> object
        /// to <see cref="System.Windows.Media.FontFamily"/> object .
        /// </summary>
        /// <param name="value">The <see cref="string"/> object to be converted.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">Does not matter.</param>
        /// <param name="culture">Currently used culture.</param>
        /// <returns>A converted value.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((string)value == string.Empty)
            {
                return new FontFamily();
            }

            FontFamily font = new FontFamily((string)value);
            return font;
        }

        #endregion IValueConverter Members
    }
}