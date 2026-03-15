// <copyright file="DrawingBrushToDrawingConverter.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// This class makes relation between <see cref="DrawingBrush"/> value and <see cref="Drawing"/> value.
    /// </summary>
    [ValueConversion(typeof(DrawingBrush), typeof(Drawing))]
    public class DrawingBrushToDrawingConverter : IValueConverter
    {
        #region IValueConverter Members
        /// <summary>
        /// Converts <see cref="DrawingBrush"/> value to <see cref="Drawing"/> value.
        /// </summary>
        /// <param name="value">The <see cref="DrawingBrush"/> value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted <see cref="Drawing"/> value.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DrawingBrush brush = value as DrawingBrush;

            if (brush != null)
            {
                Drawing drawing = brush.Drawing;
                if (drawing != null)
                {
                    return drawing;
                }
            }

            return null;
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="value">Original drawing value</param>
        /// <param name="targetType">Target type as Drawing</param>
        /// <param name="parameter">Binding parameter</param>
        /// <param name="culture">Current culture info</param>
        /// <returns>Returns original drawing brush</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}