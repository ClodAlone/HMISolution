// <copyright file="HeightToWidthConverter.cs" company="Syncfusion">
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Syncfusion.Windows.Diagram
{
    /// <summary>
    /// This class converts a value from Height to Width.
    /// </summary>
   public class HeightToWidthConverter : IMultiValueConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeightToWidthConverter"/> class.
        /// </summary>
        public HeightToWidthConverter()
        { 
        }

        #region IMultiValueConverter Members

        /// <summary>
        /// Converts a value from Height to Width.
        /// </summary>
        /// <param name="values">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Orientation orientation = (Orientation)values[0];
            double thickness = double.Parse(values[1].ToString());
            double width = double.Parse(values[2].ToString());
            double height = double.Parse(values[3].ToString());
            if (orientation == Orientation.Horizontal)
            {
                if (parameter.ToString() == "TickBarHeight")
                {
                    return thickness * 0.75;
                }

                if (parameter.ToString() == "Height")
                {
                    return thickness;
                }
                else
                {
                    return width;
                }
            }
            else
            {
                if (parameter.ToString() == "TickBarWidth")
                {
                    return thickness * 0.65;
                }

                if (parameter.ToString() == "LabelsWidth")
                {
                    return thickness * 0.35;
                }

                if (parameter.ToString() == "Width")
                {
                    return thickness;
                }
                else
                {
                    return height;
                }
            }
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetTypes">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
