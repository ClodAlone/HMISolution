// <copyright file="ColorToBrushConverter.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// This class makes relation between <see cref="Color"/> value and <see cref="Brush"/> value.
    /// </summary>
    [ValueConversion(typeof(Color), typeof(Brush))]
    public class ColorToBrushConverter : IValueConverter
    {
        #region Forward conversion
        /// <summary>
        /// Converts <see cref="Color"/> value to <see cref="Brush"/> value.
        /// </summary>
        /// <param name="value">The <see cref="Color"/> value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted <see cref="Brush"/> value.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            if (!(value is Color))
            {
                throw new ArgumentException("Value of Color type is expected.", "value");
            }

            Color color = (Color)value;

            Brush result = new SolidColorBrush(color);
            return result;
        }
        #endregion

        #region Backward conversion
        /// <summary>
        /// Converts <see cref="SolidColorBrush"/> value to <see cref="Color"/> value.
        /// </summary>
        /// <param name="value">The <see cref="SolidColorBrush"/> value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted <see cref="Color"/> value.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            if (!(value is SolidColorBrush))
            {
                throw new ArgumentException("Value of SolidColorBrush type is expected.", "value");
            }

            SolidColorBrush brush = (SolidColorBrush)value;
            return brush.Color;
        }
        #endregion
    }
}
