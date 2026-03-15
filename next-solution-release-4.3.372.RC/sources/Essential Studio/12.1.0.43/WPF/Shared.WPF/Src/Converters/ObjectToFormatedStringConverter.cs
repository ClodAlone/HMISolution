// <copyright file="ObjectToFormatedStringConverter.cs" company="Syncfusion">
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
using System.Windows;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Represents the converter that converts <see cref="Object"/> values to the formatted <see cref="String"/>. Use parameter for to set the format.
    /// </summary>
    [ValueConversion(typeof(object), typeof(string))]
    public class ObjectToFormatedStringConverter : IValueConverter
    {
        #region Constants

        /// <summary>
        /// Default string format.
        /// </summary>
        private const string DEFAULT_FORMAT = "{0}";

        #endregion

        #region Implemenation

        /// <summary>
        /// Converts a value. The data binding engine calls this method when it propagates a value from the binding source to the binding target.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value.If the method returns null, the valid null value is used.A return value of <see cref="T:System.Windows.DependencyProperty"></see>.<see cref="F:System.Windows.DependencyProperty.UnsetValue"></see> indicates that the converter produced no value and that the binding uses the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"></see>, if available, or the default value instead.A return value of <see cref="T:System.Windows.Data.Binding"></see>.<see cref="F:System.Windows.Data.Binding.DoNothing"></see> indicates that the binding does not transfer the value or use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"></see> or default value.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string format = parameter as string;

            if (targetType != typeof(object) && targetType != typeof(string))
            {
                throw new ArgumentException("Target type is invalid. Valid type is string.", "targetType");
            }

            return string.Format(culture, format == null ? DEFAULT_FORMAT : format, value);
        }

        /// <summary>
        /// Converts a value. The data binding engine calls this method when it propagates a value from the binding target to the binding source.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value.If the method returns null, the valid null value is used.A return value of <see cref="T:System.Windows.DependencyProperty"></see>.<see cref="F:System.Windows.DependencyProperty.UnsetValue"></see> indicates that the converter produced no value and that to the binding uses the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"></see>, if available, or the default value instead.A return value of <see cref="T:System.Windows.Data.Binding"></see>.<see cref="F:System.Windows.Data.Binding.DoNothing"></see> indicates that the binding does not transfer the value or use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"></see> or default value.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Backward conversion is not supported.");
        }
        #endregion
    }

    /// <summary>
    /// Represents the converter that converts array of Object to the formatted String. Use parameter for to set the format.
    /// </summary>
    [ValueConversion(typeof(object[]), typeof(string))]
    public class MultiObjectToFormatedStringConverter : IMultiValueConverter
    {
        #region Constants
        /// <summary>
        /// Default string format.
        /// </summary>
        private const string DEFAULT_FORMAT = "{0}";

        #endregion

        #region Implemenation

        /// <summary>
        /// Converts source values to a value for the binding target. The data binding engine calls this method when it propagates the values from source bindings to the binding target.
        /// </summary>
        /// <param name="values">The array of values that the source bindings in the <see cref="T:System.Windows.Data.MultiBinding"></see> produces. The value <see cref="F:System.Windows.DependencyProperty.UnsetValue"></see> indicates that the source binding has no value to provide for conversion.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value.If the method returns null, the valid null value is used.A return value of <see cref="T:System.Windows.DependencyProperty"></see>.<see cref="F:System.Windows.DependencyProperty.UnsetValue"></see> indicates that the converter did not produce a value, and that the binding will use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"></see> if it is available, or else will use the default value.A return value of <see cref="T:System.Windows.Data.Binding"></see>.<see cref="F:System.Windows.Data.Binding.DoNothing"></see> indicates that the binding does not transfer the value or use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"></see> or the default value.
        /// </returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string format = parameter as string;

            if (targetType != typeof(object) && targetType != typeof(string))
            {
                throw new ArgumentException("Target type is invalid. Valid type is string.", "targetType");
            }

            return string.Format(culture, format == null ? DEFAULT_FORMAT : format, values);
        }

        /// <summary>
        /// Converts a binding target value to the source binding values.
        /// </summary>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// An array of values that have been converted from the target value back to the source values.
        /// </returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Backward conversion is not supported.");
        }

        #endregion
    }
}
