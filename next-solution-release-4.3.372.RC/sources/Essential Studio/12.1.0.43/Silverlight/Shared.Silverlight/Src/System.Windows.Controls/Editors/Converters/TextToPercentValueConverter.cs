// <copyright file="TextToPercentValueConverter.cs" company="Syncfusion">
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
    /// Converter that convert text type into Percentage value.
    /// </summary>
    public class TextToPercentValueConverter : IValueConverter
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
        /// <returns><see cref="String"/> value that represents percent double value and formatted accordingly to the parameter</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string))
            {
                throw new ArgumentException("Value of String type is expected.", "value");
            }

            NumberFormatInfo numberFormat = parameter as NumberFormatInfo;

            string strValue = (string)value;
            if (strValue.Length > 0)
            {
                int index = strValue.IndexOf(numberFormat.PercentSymbol);
                if (index > 0)
                {
                    strValue = strValue.Remove(index, numberFormat.PercentSymbol.Length);
                }

                double preVlaue = 0;

                if (double.TryParse(strValue, NumberStyles.Number, numberFormat, out preVlaue))
                {
                    return preVlaue;
                }
            }
            else if (strValue.Length == 0)
            {
                return null;
            }

            return 0;


           
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
            v /= 100.0;

            NumberFormatInfo numberFormat = parameter as NumberFormatInfo;
            return v.ToString("P", numberFormat);
        }
        #endregion
    }
}
