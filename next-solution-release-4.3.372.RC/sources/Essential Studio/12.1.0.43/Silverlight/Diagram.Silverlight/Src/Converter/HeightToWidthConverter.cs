// <copyright file="HeightToWidthConverter.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Diagram
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    /// <summary>
    /// This class converts a value from Height to Width.
    /// </summary>
    public class HeightToWidthConverter : IValueConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeightToWidthConverter"/> class.
        /// </summary>
        public HeightToWidthConverter()
        {
        }

        /// <summary>
        /// Converts a value from Height to Width.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double width = double.Parse(value.ToString());
            double height = double.Parse(value.ToString());          
            if (parameter.ToString() == "TickBarHeight")
            {
            }
            else
            {
                if (parameter.ToString() == "TickBarWidth")
                {
                }
            }

            return null;
        }

        /// <summary>
        /// Does nothing.
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

        #region IMultiValueConverter Members

        #endregion
    }
}
