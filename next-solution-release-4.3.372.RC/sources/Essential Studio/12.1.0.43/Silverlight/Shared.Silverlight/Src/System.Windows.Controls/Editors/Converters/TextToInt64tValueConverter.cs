// <copyright file="TextToInt64tValueConverter.cs" company="Syncfusion">
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
    /// TextToInt64ValueConverter class provides us to convert text to int64
    /// </summary>
    /// <remarks>
    /// This class represents type converter from <see cref="String"/> value to <see cref="Int64"/> value.
    /// </remarks>
    public class TextToInt64ValueConverter : IValueConverter
    {
        #region Private Members
        /// <summary>
        /// Define integer textbox
        /// </summary>
        private IntegerTextBox m_tb = null;
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="TextToInt64ValueConverter"/> class.
        /// </summary>
        /// <param name="tb">The tb IntegerTextBox.</param>
        internal TextToInt64ValueConverter(IntegerTextBox tb)
        {
            m_tb = tb;
        }
        #endregion

        #region IValueConverter Members
        /// <summary>
        /// Method converts <see cref="String"/> value to <see cref="Int64"/> value.
        /// </summary>
        /// <param name="value">Object as string value, if type of value is not <see cref="String"/>,
        /// then there will be raised an <see cref="ArgumentException"/>.
        /// </param>
        /// <param name="targetType">Represents the type in which value should be converted.</param>
        /// <param name="parameter">Is a NumberFormatInfo object.</param>
        /// <param name="culture">Current UI culture.</param>
        /// <returns><see cref="String"/> value that represents integer value and formatted accordingly to the parameter</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string) && value != null)
            {
                throw new ArgumentException("Value of String type is expected.", "value");
            }

            string s = value as string;
            NumberFormatInfo numberFormat = parameter as NumberFormatInfo;

            Int64 preVlaue = 0L;

            if (Int64.TryParse(s, NumberStyles.Number, numberFormat, out preVlaue))
            {
                return preVlaue;
            }
            else if (string.IsNullOrEmpty(s))
            {
                return null;
            }


            return 0L;
        }

        /// <summary>
        /// Method converts <see cref="Int64"/> value to <see cref="String"/> value.
        /// </summary>
        /// <param name="value">Object as string value, if type of value is not <see cref="String"/>,
        /// then there will be raised an <see cref="ArgumentException"/>. </param>
        /// <param name="targetType">Represents the type in which value should be converted.</param>
        /// <param name="parameter">Is a NumberFormatInfo object. Value should be parsed accordingly this object .</param>
        /// <param name="culture">Current UI culture.</param>
        /// <returns>In case if there was correct input string converted <see cref="Int64"/> value, otherwise 0.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Int64))
            {
                throw new ArgumentException("Value of Int64 type is expected.", "value");
            }

            Int64 v = (Int64)value;
            string str = string.Empty;
            NumberFormatInfo numberFormat = parameter as NumberFormatInfo;

            if (!(m_tb != null && m_tb.UseNullOption && m_tb.NullState))
            {
                str = v.ToString("N", numberFormat);
            }

            return str;
        }
        #endregion
    }
}
