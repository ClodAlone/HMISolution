// <copyright file="IsSymbolFontConverter.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Syncfusion.Windows.Tools
{
    /// <summary>
    /// This class is responsible for checking whether font is symbolic.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [ValueConversion(typeof(double), typeof(string))]
    public class IsSymbolFontConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Checks whether the font is symbolic.
        /// </summary>
        /// <param name="value">Font to be checked.</param>
        /// <param name="targetType">Type which represents result.</param>
        /// <param name="parameter">Does not matter.</param>
        /// <param name="culture">Currently used culture. </param>
        /// <exception cref="ArgumentException">The value parameter is not
        /// of type <see cref="System.Windows.Media.FontFamily"/>.
        /// </exception>
        /// <returns>
        /// True if font is symbolic, otherwise false.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is FontFamily))
            {
                throw new ArgumentException("Value of FontFamily type is expected.", "value");
            }

            return Syncfusion.Windows.Tools.Controls.FontChecker.IsFontSymbol((FontFamily)value);
        }

        /// <summary>
        /// This method is not used.
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
            return null;
        }

        #endregion IValueConverter Members
    }
}