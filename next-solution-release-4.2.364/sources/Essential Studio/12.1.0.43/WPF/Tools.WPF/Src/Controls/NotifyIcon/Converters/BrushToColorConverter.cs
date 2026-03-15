// <copyright file="BrushToColorConverter.cs" company="Syncfusion">
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
    /// Represents the class for brush to color converter
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class BrushToColorConverter : IValueConverter
    {
        /// <summary>
        /// Converts Brush to Color.
        /// </summary>
        /// <param name="value">Brush to be converted.</param>
        /// <param name="targetType">Target type of the object invoked</param>
        /// <param name="parameter">Does not matter.</param>
        /// <param name="culture">Currently used culture. Not used here.</param>
        /// <returns>
        /// Converted color.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Color retColor = Brushes.Transparent.Color;
            Brush brush = (Brush)value;

            if (brush == null)
            {
                return retColor;
            }

            if (brush is SolidColorBrush)
            {
                retColor = (brush as SolidColorBrush).Color;
            }
            else if (brush is LinearGradientBrush)
            {
                LinearGradientBrush lb = brush as LinearGradientBrush;
                foreach (GradientStop stop in lb.GradientStops)
                {
                    if (stop.Offset == 1.0)
                    {
                        retColor = stop.Color;
                        break;
                    }
                }
            }

            return retColor;
        }

        /// <summary>
        /// Method to convert back.
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
