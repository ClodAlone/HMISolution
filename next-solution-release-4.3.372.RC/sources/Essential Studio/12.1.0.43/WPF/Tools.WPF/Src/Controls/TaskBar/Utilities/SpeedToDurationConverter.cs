// <copyright file="SpeedToDurationConverter.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Windows;
using System.Windows.Data;

namespace Syncfusion.Windows.Tools
{
    /// <summary>
    /// This class implements conversion from <see cref="Int32"/> type to <see cref="Duration"/>
    /// type.
    /// </summary>
    /// <remarks>
    /// Calculate optimal <see cref="Duration"/> for current control height.
    /// </remarks>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class SpeedToDurationConverter : IMultiValueConverter
    {
        #region Constants

        /// <summary>
        /// This constant is used for calculating speed for expanded
        /// event.
        /// </summary>
        private const string AnimationTypeExpanding = "Expanded";

        /// <summary>
        /// This constant is used for calculating speed for collapsed
        /// event.
        /// </summary>
        private const string AnimationTypeCollapsing = "Collapsed";

        #endregion Constants

        #region IMultiValueConverter Members

        /// <summary>
        /// This method converts speed value to duration value.
        /// </summary>
        /// <param name="values">Contains speed and the value of the
        /// height.</param>
        /// <param name="targetType">The type in which the value should
        /// be converted.</param>
        /// <param name="parameter">Contains name control part for
        /// painting out.</param>
        /// <param name="culture">The culture for given converting.</param>
        /// <returns>
        /// Duration of expanded/collapsed item.
        /// </returns>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(DependencyProperty.UnsetValue == values[0]))
            {
                double speed = (double)values[0];

                if (0 != speed)
                {
                    return new Duration(new TimeSpan(0, 0, 0, 0, 100 + (int)Math.Ceiling(speed)));
                }
            }
            return new Duration(new TimeSpan(0, 0, 0, 0, 0));
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
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion IMultiValueConverter Members
    }
}