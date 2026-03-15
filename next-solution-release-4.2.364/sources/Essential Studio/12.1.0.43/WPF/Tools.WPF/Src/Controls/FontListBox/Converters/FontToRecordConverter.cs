// <copyright file="FontToRecordConverter.cs" company="Syncfusion">
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
    /// Converts <see cref="System.Windows.Media.FontFamily"/> objects
    /// to <see cref="Syncfusion.Windows.Tools.Controls.FontFamilyRecord"/> objects.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [ValueConversion(typeof(FontFamily), typeof(Syncfusion.Windows.Tools.Controls.FontListBoxInternalItem))]
    public class FontToRecordConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts <see cref="System.Windows.Media.FontFamily"/> object
        /// to <see cref="Syncfusion.Windows.Tools.Controls.FontFamilyRecord"/> object.
        /// </summary>
        /// <param name="value">FontFamily object to be converted.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">Does not matter.</param>
        /// <param name="culture">Currently used culture.</param>
        /// <exception cref="ArgumentException">The value parameter is not
        /// of type <see cref="System.Windows.Media.FontFamily"/>.
        /// </exception>
        /// <returns>
        /// A converted value.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            if (!(value is FontFamily))
            {
                throw new ArgumentException("Value of FontFamily type is expected.", "value");
            }

            FontFamily fontFamily = (FontFamily)value;
            FontFamilyRecord record = new FontFamilyRecord();

            XmlLanguage userLanguage = XmlLanguage.GetLanguage("en-US");

            record.Family = fontFamily;
            record.Name = fontFamily.FamilyNames[userLanguage];

            return record;
        }

        /// <summary>
        /// Converts <see cref="Syncfusion.Windows.Tools.Controls.FontFamilyRecord"/> object
        /// to <see cref="System.Windows.Media.FontFamily"/> object .
        /// </summary>
        /// <param name="value">FontFamilyRecord object to be converted.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">Does not matter.</param>
        /// <param name="culture">Currently used culture. </param>
        /// <exception cref="ArgumentException">The value parameter is not
        /// of type <see cref="Syncfusion.Windows.Tools.Controls.FontFamilyRecord"/>.
        /// </exception>
        /// <returns>
        /// A converted value.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            if (!(value is FontFamilyRecord))
            {
                throw new ArgumentException("Value of FontFamilyRecord type is expected.", "value");
            }

            FontFamilyRecord record = (FontFamilyRecord)value;

            FontFamily font;

            if (record.Type == FontFamilyRecordType.Theme)
            {
                font = new ThemeFontFamily(record.Name, record.Purpose);
            }
            else
            {
                font = new FontFamily(record.Name);
            }

            return font;
        }

        #endregion IValueConverter Members
    }
}