// <copyright file="DoubleToStringConverter.cs" company="Syncfusion">
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
using System.Globalization;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// This class makes relation between <see cref="Double"/> value and <see cref="String"/> value.
    /// </summary>
    [ValueConversion(typeof(double), typeof(string))]
    public class DoubleToStringConverter : IValueConverter
    {
        #region Forward conversion

        /// <summary>
        /// Converts <see cref="Double"/> value to <see cref="String"/> value.
        /// </summary>
        /// <param name="value">The <see cref="Double"/> value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted <see cref="String"/> value.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string format = "0.00";

            if (targetType != typeof(string))
            {
                throw new ArgumentException("Target type is invalid. Valid type is string.", "targetType");
            }

            if (parameter != null && parameter.GetType() == typeof(string))
            {
                format = (string)parameter;
            }

            if (culture == null)
            {
                culture = CultureInfo.CurrentCulture;
            }

            double valueDouble = (double)value;
            string result = valueDouble.ToString(format, culture.DateTimeFormat);

            return result;
        }
        #endregion

        #region Backward conversion

        /// <summary>
        /// Converts <see cref="String"/> value to <see cref="Double"/> value.
        /// </summary>
        /// <param name="value">The <see cref="String"/> value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted <see cref="Double"/> value.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (culture == null)
            {
                culture = CultureInfo.CurrentCulture;
            }

            double result = double.Parse((string)value, culture.DateTimeFormat);
            return result;
        }
        #endregion
    }
}
