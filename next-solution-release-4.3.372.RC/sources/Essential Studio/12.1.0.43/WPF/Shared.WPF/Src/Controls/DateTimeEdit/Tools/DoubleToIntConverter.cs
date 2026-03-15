// <copyright file="DoubleToIntConverter.cs" company="Syncfusion">
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
    /// This class convert Duration value to Int32.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [ValueConversion(typeof(double), typeof(int))]
    public class DoubleToIntConverter : IValueConverter
    {
        #region Constants

        /// <summary>
        /// Default value for DateTimeEdit.
        /// </summary>
        private const double Duration = 200;

        #endregion Constants

        #region IValueConverter Members

        /// <summary>
        /// Convert Duration value to Int32.
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
            double duration = (Double)value;
            double calculateValue = Duration * duration;

            return int.MaxValue > calculateValue ? (int)calculateValue : int.MaxValue;
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
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion IValueConverter Members
    }
}