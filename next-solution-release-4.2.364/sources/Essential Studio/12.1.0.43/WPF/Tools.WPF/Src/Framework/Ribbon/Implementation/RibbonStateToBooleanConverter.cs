// <copyright file="RibbonStateToBooleanConverter.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the converter that converts RibbonState values to and from Boolean values. 
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class RibbonStateToBooleanConverter : IValueConverter
    {
        #region IValueConverter Members
        /// <summary>
        /// Converts a value. The data binding data engine calls this method
        /// when it propagates a value from the binding data source to the
        /// binding data target.
        /// </summary>
        /// <param name="value">Value to be converted.</param>
        /// <param name="targetType">Value, the width is to be converted
        /// to.</param>
        /// <param name="parameter">Does not matter.</param>
        /// <param name="culture">Currently used culture. Not used
        /// here.</param>
        /// <returns>
        /// Converted value. 
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            RibbonState state = (RibbonState)value;

            if (state == RibbonState.Adorner)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Converts a value.
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
            if (targetType != typeof(RibbonState))
            {
                throw new InvalidOperationException("The target must be a RibbonState!");
            }

            bool isSelected = (bool)value;

            if (isSelected)
            {
                return RibbonState.Normal;
            }
            else
            {
                return RibbonState.Adorner;
            }
        }

        #endregion
    }
}
