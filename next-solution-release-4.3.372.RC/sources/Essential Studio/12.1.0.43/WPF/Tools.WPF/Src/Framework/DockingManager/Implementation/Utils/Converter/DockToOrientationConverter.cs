// <copyright file="DockToOrientationConverter.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// This class convert from Dock to Orientation as the situation requires.
    /// </summary>
    /// <exclude/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class DockToOrientationConverter : IValueConverter
    {
        #region Public method
        /// <summary>
        /// This method converts form Dock to Orientation as the situation requires.
        /// </summary>
        /// <param name="value">Value to be converted.</param>
        /// <param name="targetType">Type, the value is to be converted to.</param>
        /// <param name="parameter">Does not matter.</param>
        /// <param name="culture">Currently used culture. Not used here.</param>
        /// <returns>Converted value.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Dock panelSide = (Dock)value;

            switch (panelSide)
            {
                case Dock.Left:
                case Dock.Right:
                    return Orientation.Vertical;
                case Dock.Bottom:
                case Dock.Top:
                    return Orientation.Horizontal;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// This method does nothing.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion
    }
}
