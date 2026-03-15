// <copyright file="PopupHeightToThumbOffsetConverter.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Windows.Data;

namespace Syncfusion.Windows.Tools
{
    /// <summary>
    /// Converts height of the Popup and offset of BottomThumb to each other in FontListConmboBox control.
    /// Both values are <see cref="double"/>
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class PopupHeightToThumbOffsetConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts height of the Popup to the offset of BottomThumb.
        /// </summary>
        /// <param name="value">The <see cref="double"/> value of Pop-up's height.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The <see cref="double"/> value of difference between the Pop-up's height and BottomThumb's offset.</param>
        /// <param name="culture">Currently used culture.</param>
        /// <returns>A converted value.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || !(value is Double))
            {
                throw new ArgumentException("The value argument is null or has incorrect type (must be double).");
            }

            double height = (double)value;
            if (double.IsNaN(height))
            {
                return height;
            }
            else
            {
                string param = parameter as string;
                if (parameter == null)
                {
                    throw new ArgumentException("The parameter argument is null or has incorrect type (must be string).");
                }

                double shift = double.Parse(param);
                return height - shift;
            }
        }

        /// <summary>
        /// Converts offset of the BottomThumb to height of the Popup.
        /// </summary>
        /// <param name="value">The <see cref="double"/> value of BottomThumb's offset.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The <see cref="double"/> value of difference between the Pop-up's height and BottomThumb's offset.</param>
        /// <param name="culture">Currently used culture.</param>
        /// <returns>A converted value.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || !(value is Double))
            {
                throw new ArgumentException("The value argument is null or has incorrect type (must be double).");
            }

            double offset = (double)value;
            if (double.IsNaN(offset))
            {
                return offset;
            }
            else
            {
                string param = parameter as string;
                if (parameter == null)
                {
                    throw new ArgumentException("The parameter argument is null or has incorrect type (must be string).");
                }

                double shift = double.Parse(param);
                return offset + shift;
            }
        }

        #endregion IValueConverter Members
    }
}