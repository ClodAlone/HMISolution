// <copyright file="DoubleToNegativeDoubleConverter.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Syncfusion.Windows.Tools
{
    /// <summary>
    /// This class implements conversion from positive double to
    /// negative double.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class DoubleToNegativeDoubleConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        /// <summary>
        /// This method converts double-value to negative double-value
        /// taking into consideration top and bottom thickness.
        /// </summary>
        /// <param name="values">Contains list of double value and
        /// thickness.</param>
        /// <param name="targetType">The type in which the value should
        /// be converted.</param>
        /// <param name="parameter">Contains name control part for
        /// painting out.</param>
        /// <param name="culture">The culture for given converting.</param>
        /// <returns>
        /// Negative value.
        /// </returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double height = (double)values[0];
            Thickness thickness = (Thickness)values[1];
            double contentReverseHeight = -height - thickness.Top - thickness.Bottom;
            return contentReverseHeight;
        }

        /// <summary>
        /// Converts a binding target value to the source binding values.
        /// </summary>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// An array of values that have been converted from the target value back to the source values.
        /// </returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }

        #endregion IMultiValueConverter Members
    }
}