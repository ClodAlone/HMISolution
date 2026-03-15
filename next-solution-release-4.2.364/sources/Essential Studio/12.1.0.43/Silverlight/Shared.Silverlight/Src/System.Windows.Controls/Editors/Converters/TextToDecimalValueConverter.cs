// <copyright file="TextToDecimalValueConverter.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// This class represents type converter from <see cref="String"/> value to <see cref="Decimal"/> value.
    /// </summary>    
    public class TextToDecimalValueConverter : IValueConverter
    {
        #region IValueConverter Members
        /// <summary>
        /// Method converts <see cref="Decimal"/> value to <see cref="String"/> value.
        /// </summary>
        /// <param name="value">Object as decimal value, if type of value is not <see cref="Decimal"/>,
        /// then there will be raised an <see cref="ArgumentException"/>.
        /// </param>
        /// <param name="targetType">Represents the type in which value should be converted.</param>
        /// <param name="parameter">Is a NumberFormatInfo object.</param>
        /// <param name="culture">Current UI culture.</param>
        /// <returns><see cref="String"/> value that represents decimal value and formatted accordingly to the parameter</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is string))
            {
                throw new ArgumentException("Value of String type is expected.", "value");
            }

            NumberFormatInfo numberFormat = parameter as NumberFormatInfo;

            string strValue = (string)value;
            if (strValue.Length > 0)
            {
                decimal preVlaue = 0;

                if (decimal.TryParse(strValue, NumberStyles.Currency, numberFormat, out preVlaue))
                {
                    return preVlaue;
                }
                else if (string.IsNullOrEmpty(strValue))
                {
                    return null;
                }

            }
            else if (string.IsNullOrEmpty(strValue))
            {
                return null;
            }
            return 0m;

        }

        /// <summary>
        /// Method converts <see cref="String"/> value to <see cref="Decimal"/> value.
        /// </summary>
        /// <param name="value">Object as string value, if type of value is not <see cref="String"/>,
        /// then there will be raised an <see cref="ArgumentException"/>. </param>
        /// <param name="targetType">Represents the type in which value should be converted.</param>
        /// <param name="parameter">Is a NumberFormatInfo object. Value should be parsed accordingly this object .</param>
        /// <param name="culture">Current UI culture.</param>
        /// <returns>In case if there was correct input string converted <see cref="Decimal"/> value, otherwise 0.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is decimal))
            {
                throw new ArgumentException("Value of Double type is expected.", "value");
            }

            decimal v = (decimal)value;
            NumberFormatInfo numberFormat = parameter as NumberFormatInfo;

            return v.ToString("C", numberFormat);
        }
        #endregion
    }
}
