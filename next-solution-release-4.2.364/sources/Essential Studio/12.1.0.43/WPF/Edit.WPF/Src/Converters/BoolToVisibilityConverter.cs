// <copyright file="BoolToVisibilityConverter.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Windows;
using System.Windows.Data;

namespace Syncfusion.Windows.Edit
{
    /// <summary>
    /// IValueConverter implemented class that return Visibility value based on bool
    /// input
    /// </summary>
    /// <remarks>
    /// The BoolToVisibilityConverter used to contert Boolean value into the Visibility value
    /// converter.
    /// </remarks>
#if SyncfusionFramework4_0

    using System.ComponentModel;

    [DesignTimeVisible(false)]
#endif
    public class BoolToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts bool value to visibility enumeration. returns Visible when the value passed is true else it returns hidden.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>Returns a converted value.</returns>
        /// <remarks>This converter is used to check the boolean value and convert that boolean into visibility enumeration.</remarks>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value)
            {
                return Visibility.Visible;
            }
            else
            {
                if (parameter.ToString() == "Hide")
                {
                    return Visibility.Hidden;
                }
                return Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Converts back the bool value.
        /// </summary>
        /// <remarks>
        /// <para>ConvertBack method used to converts back the boolean value from the source
        /// value.</para>
        /// </remarks>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// <para>Returns a converted value. If the method returns null, the valid null
        /// value is used.</para>
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion IValueConverter Members
    }
}