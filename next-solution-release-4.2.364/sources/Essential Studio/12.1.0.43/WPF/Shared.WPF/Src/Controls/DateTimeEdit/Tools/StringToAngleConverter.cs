// <copyright file="StringToAngleConverter.cs" company="Syncfusion">
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
    /// This class convert string value to double
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [ValueConversion(typeof(string), typeof(double))]
    public class StringToAngleConverter : IValueConverter
    {
        #region Constants

        /// <summary>
        /// This constanr for define left button angle transform.
        /// </summary>
        private const string Up = "Up";

        /// <summary>
        ///  This constanr for define right button angle transform.
        /// </summary>
        private const string Down = "Down";

        #endregion Constants

        #region IValueConverter Members

        /// <summary>
        /// This method define Angle for RepeatButtonExt.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>Angle for transform.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string str = value.ToString();

            if (Up == str)
            {
                return 0d;
            }
            else if (Down == str)
            {
                return 180d;
            }
            else
            {
                throw new ArgumentException("incorrect argument", str);
            }
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>Always returns null.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion IValueConverter Members
    }
}