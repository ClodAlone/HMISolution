// <copyright file="TabItemsCornerRadiusRotateConverter.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// This class converts TabItemsCornerRadius of the window when rotate.
    /// </summary>
    /// <exclude/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class TabItemsCornerRadiusRotateConverter : IMultiValueConverter
    {
        #region Implementation
        /// <summary>
        /// Converts TabItemsCornerRadius of the window when rotate.
        /// </summary>
        /// <param name="values">Values to be converted.</param>
        /// <param name="targetType">Type, the value is to be converted to.</param>
        /// <param name="parameter">Does not matter.</param>
        /// <param name="culture">Currently used culture. Not used here.</param>
        /// <returns>Converted value.</returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            CornerRadius cornerRadius = (CornerRadius)values[0];
            Dock tabStrip = (Dock)values[1];

            if (Dock.Top == tabStrip)
            {
                double topLeft = cornerRadius.BottomRight;
                double topRight = cornerRadius.BottomLeft;
                double bottomLeft = cornerRadius.TopRight;
                double bottomRight = cornerRadius.TopLeft;

                cornerRadius = new CornerRadius(topLeft, topRight, bottomRight, bottomLeft);
            }
            else if (Dock.Right == tabStrip)
            {
                double topLeft = cornerRadius.BottomLeft;
                double topRight = cornerRadius.BottomRight;
                double bottomLeft = cornerRadius.TopLeft;
                double bottomRight = cornerRadius.TopRight;

                cornerRadius = new CornerRadius(topLeft, topRight, bottomRight, bottomLeft);
            }

            return cornerRadius;
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
            throw new NotImplementedException();
        }
        #endregion
    }
}
