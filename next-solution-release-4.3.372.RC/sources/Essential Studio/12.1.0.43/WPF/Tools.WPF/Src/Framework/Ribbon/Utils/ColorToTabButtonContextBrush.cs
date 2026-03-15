// <copyright file="ColorToTabButtonContextBrush.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the class for the color to Tab button context brush.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ColorToTabButtonContextBrush : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding data source.</param>
        /// <param name="targetType">The type of the binding data target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
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

            if (!(parameter is LinearGradientBrush))
            {
                throw new ArgumentException("Value of LinearGradientBrush type is expected.", "parameter");
            }

            LinearGradientBrush baseBrush = parameter as LinearGradientBrush;
            Color color = (Color)value;

            RibbonColorScheme colorScheme = new RibbonColorScheme(color);

            LinearGradientBrush brush = new LinearGradientBrush();

            brush.StartPoint = baseBrush.StartPoint;
            brush.EndPoint = baseBrush.EndPoint;

            //foreach (GradientStop stop in baseBrush.GradientStops)
            //{
            //    GradientStop newStop = new GradientStop(RibbonColorScheme.GetColor(stop.Color, colorScheme.m_blendColor), stop.Offset);
            //    brush.GradientStops.Add(newStop);
            //}

            return brush;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding data target.</param>
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

        #endregion
    }
}
