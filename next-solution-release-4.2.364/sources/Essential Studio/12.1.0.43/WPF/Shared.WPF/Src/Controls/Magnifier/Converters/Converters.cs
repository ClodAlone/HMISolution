// <copyright file="Converters.cs" company="Syncfusion">
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
using System.Windows;
using System.Windows.Media;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Converts circle radius value to Width and Height of the ellipse representing it.
    /// Both values are <see cref="double"/>.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class RadiusToWidthHeightConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts circle radius value to Width and Height of the ellipse representing it.
        /// Both values are <see cref="double"/>.
        /// </summary>
        /// <param name="value">The <see cref="object"/> value to be converted.</param>
        /// <param name="targetType">Type, value should be converted to.</param>
        /// <param name="parameter">Parameter, not used.</param>
        /// <param name="culture">Currently used culture. Not used.</param>
        /// <returns>Converted value.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (double)value * 2;
        }

        /// <summary>
        /// This kind of conversion is not supported.
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

        #endregion
    }

    /// <summary>
    /// Converts Thickness value used in BorderThickness to double representing Shape.StrokeThickness.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ThicknessToDoubleConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts Thickness value used in BorderThickness to double representing Shape.StrokeThickness.
        /// </summary>
        /// <param name="value">The <see cref="object"/> value to be converted.</param>
        /// <param name="targetType">Type, value should be converted to.</param>
        /// <param name="parameter">Parameter, not used.</param>
        /// <param name="culture">Currently used culture. Not used.</param>
        /// <returns>Converted value.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Thickness thickness = (Thickness)value;
            return (thickness.Bottom + thickness.Left + thickness.Right + thickness.Top) / 4;
        }

        /// <summary>
        /// This kind of conversion is not supported.
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

        #endregion
    }
}
