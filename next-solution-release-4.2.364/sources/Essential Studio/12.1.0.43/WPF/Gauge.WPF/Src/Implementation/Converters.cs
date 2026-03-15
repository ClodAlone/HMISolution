// <copyright file="Converters.cs" company="Syncfusion Software">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Syncfusion.Windows.Gauge
{
    #region Converters
    /// <summary>
    /// Given a radius, it returns the diameter.
    /// </summary>
    /// <remarks>
    /// Used in calculating the width and height of the Circular Gauge's border(i.e frames).
    /// </remarks>
    [ValueConversion(typeof(double), typeof(double))]
    public class RadiusConverter : IValueConverter
    {
        /// <summary>
        /// Converts Radius into Diameter
        /// </summary>
        /// <param name="value">Value to be converted</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>Converted value</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value * 2;
        }

        /// <summary>
        /// Empty converter.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>ConverBack is not possible, hence returns "null"</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Given a Thickness, it returns the Thickness's Left value.
    /// </summary>
    /// <remarks>
    /// Used in calculating the <see cref="HalfCircleBorder.BorderWidth"/> from Thickness.
    /// </remarks>
    [ValueConversion(typeof(double), typeof(Thickness))]
    internal class ThicknessConverter : IValueConverter
    {
        /// <summary>
        /// Converts Thickness to double value
        /// </summary>
        /// <param name="value">Value to be converted</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>Converted value</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Thickness)value).Left;
        }

        /// <summary>
        /// Empty converter.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>ConverBack is not possible, hence returns "null"</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Convertor class for converting Boolean value to Visibility property value
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Method for performing the conversion
        /// </summary>
        /// <param name="value">converts this value object into bool type</param>
        /// <param name="targetType">passing target type</param>
        /// <param name="parameter">passing parameter object</param>
        /// <param name="culture">passing culture CultureInfo</param>
        /// <returns>Type : Visibility</returns>
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool bval = (bool)value;
            if (bval)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Method for performing the conversion  in the reverse direction
        /// </summary>
        /// <param name="value">passing value object</param>
        /// <param name="targetType">passing target type</param>
        /// <param name="parameter">passing parameter object</param>
        /// <param name="culture">passing culture CultureInfo</param>
        /// <returns>Type : throw</returns>
        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
    #endregion
}
