// <copyright file="CultureToCalendarConverter.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Globalization;
using System.Windows.Data;

namespace Syncfusion.Windows.Tools
{
    /// <summary>
    /// Converts <see cref="System.Globalization.CultureInfo"/> objects
    /// to <see cref="System.Globalization.Calendar"/> objects.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class CultureToCalendarConverter : IValueConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CultureToCalendarConverter"/> class.
        /// </summary>
        public CultureToCalendarConverter()
        {
        }

        #region IValueConverter Members

        /// <summary>
        /// Converts <see cref="System.Globalization.CultureInfo"/> object
        /// to <see cref="System.Globalization.Calendar"/> object.
        /// </summary>
        /// <param name="value">The <see cref="System.Globalization.CultureInfo"/> object to be converted.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">Does not matter.</param>
        /// <param name="culture">Currently used culture.</param>
        /// <returns>A converted value.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as CultureInfo).Calendar;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("It's only one way converter.");
        }

        #endregion IValueConverter Members
    }
}