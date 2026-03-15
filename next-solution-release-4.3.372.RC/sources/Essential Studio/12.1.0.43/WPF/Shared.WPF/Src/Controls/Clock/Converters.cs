// <copyright file="Converters.cs" company="Syncfusion">
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
    #region Converters
    /// <summary>
    /// Represents a SecondsConverter class.
    /// <remarks>
    /// SecondsConverter is used to convert seconds value to rotate corner of the SecondsHand.
    /// </remarks>
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [ValueConversion(typeof(DateTime), typeof(int))]
    public class SecondsConverter : IValueConverter
    {
        /// <summary>
        /// Converts seconds value to rotate corner of the SecondsHand.
        /// </summary>
        /// <param name="value">current date/time value</param>
        /// <param name="targetType">This parameter is not used. The TargetType</param>
        /// <param name="parameter">This parameter is not used. The object</param>
        /// <param name="culture">This parameter is not used. The culture</param>
        /// <returns>converted value</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime date = (DateTime)value;
            return date.Second * 6;
        }

        /// <summary>
        /// Empty converter.
        /// </summary>
        /// <param name="value">This parameter is not used</param>
        /// <param name="targetType">This parameter is not used. The TargetType</param>
        /// <param name="parameter">This parameter is not used. The object</param>
        /// <param name="culture">This parameter is not used. The culture</param>
        /// <returns>Return null value</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Represents a MinutesConverter class.
    /// <remarks>
    /// MinutesConverter is used to convert minutes value to rotate corner of the MinutesHand.
    /// </remarks>
    /// </summary>
    [ValueConversion(typeof(DateTime), typeof(int))]
    public class MinutesConverter : IValueConverter
    {
        /// <summary>
        /// Converts minutes value to rotate corner of the MinutesHand.
        /// </summary>
        /// <param name="value">current date/time value</param>
        /// <param name="targetType">This parameter is not used. The TargetType</param>
        /// <param name="parameter">This parameter is not used. The object</param>
        /// <param name="culture">This parameter is not used. The culture</param>
        /// <returns>converted value</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime date = (DateTime)value;
            return date.Minute * 6;
        }

        /// <summary>
        /// Empty converter.
        /// </summary>
        /// <param name="value">This parameter is not used</param>
        /// <param name="targetType">This parameter is not used. The TargetType</param>
        /// <param name="parameter">This parameter is not used. The object</param>
        /// <param name="culture">This parameter is not used. The culture</param>
        /// <returns>Return null value</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Represents a HoursConverter class.
    /// <remarks>
    /// HoursConverter is used to convert hours value to rotate corner of the HoursHand.
    /// </remarks>
    /// </summary>
    [ValueConversion(typeof(DateTime), typeof(int))]
    public class HoursConverter : IValueConverter
    {
        /// <summary>
        /// Converts hours value to rotate corner of the HoursHand.
        /// </summary>
        /// <param name="value">current date/time value</param>
        /// <param name="targetType">This parameter is not used. The TargetType</param>
        /// <param name="parameter">This parameter is not used. The object</param>
        /// <param name="culture">This parameter is not used. The culture</param>
        /// <returns>converted value</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime date = (DateTime)value;
            return (date.Hour * 30) + (date.Minute / 2);
        }

        /// <summary>
        /// Empty converter.
        /// </summary>
        /// <param name="value">This parameter is not used</param>
        /// <param name="targetType">This parameter is not used. The TargetType</param>
        /// <param name="parameter">This parameter is not used. The object</param>
        /// <param name="culture">This parameter is not used. The culture</param>
        /// <returns>Return null value</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
    #endregion
}
