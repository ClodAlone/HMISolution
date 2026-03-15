// <copyright file="Converters.cs" company="Syncfusion">
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
    /// Responsible for <see cref="double"/> to <see cref="string"/> converting.
    /// </summary>
    [ValueConversion(typeof(double), typeof(string))]
    public class DoublToStringConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts <see cref="double"/> value to <see cref="string"/> value.
        /// </summary>
        /// <param name="value">The <see cref="double"/> value to be converted.</param>
        /// <param name="targetType">Type, value should be converted to.</param>
        /// <param name="parameter">Is not used.</param>
        /// <param name="culture">Currently used culture. Is not used.</param>
        /// <returns>Converted value.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            if (value == DependencyProperty.UnsetValue)
            {
                return Binding.DoNothing;
            }

            double strValue = (double)value;
            return strValue.ToString();
        }

        /// <summary>
        /// Converts <see cref="string"/> value to <see cref="double"/> value.
        /// </summary>
        /// <param name="value">The <see cref="string"/> value to be converted.</param>
        /// <param name="targetType">Type, value should be converted to.</param>
        /// <param name="parameter">Is not used.</param>
        /// <param name="culture">Currently used culture. Is not used.</param>
        /// <returns>Converted value.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double result;
            if(double.TryParse((string)value,out result))
            {
                return double.Parse((string)value);
            }
            return result;
        }

        #endregion IValueConverter Members
    }
}