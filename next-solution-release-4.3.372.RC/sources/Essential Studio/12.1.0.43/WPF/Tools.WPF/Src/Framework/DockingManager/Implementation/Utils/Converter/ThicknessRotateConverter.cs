// <copyright file="ThicknessRotateConverter.cs" company="Syncfusion">
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
    /// This class converts thickness of the window when rotate.
    /// </summary>
    /// <exclude/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ThicknessRotateConverter : IMultiValueConverter
    {
        #region Implementation
        /// <summary>
        /// Converts thickness of the window when rotate.
        /// </summary>
        /// <param name="values">Values to be converted.</param>
        /// <param name="targetType">Type, the value is to be converted to.</param>
        /// <param name="parameter">Does not matter.</param>
        /// <param name="culture">Currently used culture. Not used here.</param>
        /// <returns>Converted value.</returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] != DependencyProperty.UnsetValue)
            {
                Thickness thickness = (Thickness)values[0];
                Dock tabStrip = (Dock)values[1];

                double left = thickness.Left;
                double right = thickness.Right;
                double bottom = thickness.Bottom;
                double top = thickness.Top;

                if (Dock.Right == tabStrip)
                {
                    thickness = new Thickness(right, bottom, left, top);
                }
                else if (Dock.Top == tabStrip)
                {
                    thickness = new Thickness(left, bottom, right, top);
                }
                else if (Dock.Left == tabStrip)
                {
                    thickness = new Thickness(left, top, right, bottom);
                }

                return thickness;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// This method does nothing.
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
