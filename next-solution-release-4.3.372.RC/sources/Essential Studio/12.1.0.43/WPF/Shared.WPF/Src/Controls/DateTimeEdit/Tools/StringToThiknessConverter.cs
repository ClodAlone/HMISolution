// <copyright file="StringToThiknessConverter.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Syncfusion.Windows.Tools
{
    /// <summary>
    /// This class convert string value to Thickness
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [ValueConversion(typeof(string), typeof(Thickness))]
    public class StringToThiknessConverter : IValueConverter
    {
        #region Constants

        /// <summary>
        /// This constanr for define left button Thickness.
        /// </summary>
        private const string Up = "Up";

        /// <summary>
        /// This constanr for define right button Thickness.
        /// </summary>
        private const string Down = "Down";

        #endregion Constants

        #region IValueConverter Members

        /// <summary>
        /// This method define Thickness for RepeatButtonExt.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>Thickness for border RepeatButtonExt.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = value.ToString();

            if (Up == str)
            {
                return new Thickness(0, 0, 1, 0);
            }
            else if (Down == str)
            {
                return new Thickness(1, 0, 0, 0);
            }
            else
            {
                throw new ArgumentException("Incorrect argument", str);
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
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        #endregion IValueConverter Members
    }
}