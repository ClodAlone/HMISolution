// <copyright file="TextToDoubleValueConverter.cs" company="Syncfusion">
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

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// TextToDoubleValueConverter class provides us to to convert text to double
    /// </summary>
    /// <remarks>
    /// This class represents type converter from <see cref="String"/> value to <see cref="Double"/> value.
    /// </remarks>
    public class TextToDoubleValueConverter : IValueConverter
    {
        #region IValueConverter Members
        /// <summary>
        /// Method converts <see cref="Double"/> value to <see cref="String"/> value.
        /// </summary>
        /// <param name="value">Object as double value, if type of value is not <see cref="Double"/>,
        /// then there will be raised an <see cref="ArgumentException"/>.
        /// </param>
        /// <param name="targetType">Represents the type in which value should be converted.</param>
        /// <param name="parameter">Is a NumberFormatInfo object.</param>
        /// <param name="culture">Current UI culture.</param>
        /// <returns><see cref="String"/> value that represents double value and formatted accordingly to the parameter</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string))
            {
                throw new ArgumentException("Value of String type is expected.", "value");
            }

            string s = (string)value;
            NumberFormatInfo numberFormat = parameter as NumberFormatInfo;

            double preVlaue = 0;

            if (double.TryParse(s, NumberStyles.Number, numberFormat, out preVlaue))
            {
                return preVlaue;
            }
            else if (string.IsNullOrEmpty(s))
            {
                return null;
            }
            return 0d;
        }

        /// <summary>
        /// Method converts <see cref="String"/> value to <see cref="Double"/> value.
        /// </summary>
        /// <param name="value">Object as string value, if type of value is not <see cref="String"/>,
        /// then there will be raised an <see cref="ArgumentException"/>. </param>
        /// <param name="targetType">Represents the type in which value should be converted.</param>
        /// <param name="parameter">Is a NumberFormatInfo object. Value should be parsed accordingly this object .</param>
        /// <param name="culture">Current UI culture.</param>
        /// <returns>In case if there was correct input string converted <see cref="Double"/> value, otherwise 0.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is double))
            {
                throw new ArgumentException("Value of Double type is expected.", "value");
            }

            double v = (double)value;
            NumberFormatInfo numberFormat = parameter as NumberFormatInfo;

            return v.ToString("N", numberFormat);
        }
        #endregion
    }
}
