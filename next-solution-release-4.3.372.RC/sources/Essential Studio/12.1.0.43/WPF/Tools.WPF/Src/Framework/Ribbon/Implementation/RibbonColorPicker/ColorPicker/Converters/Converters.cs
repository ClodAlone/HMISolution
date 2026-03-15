// <copyright file="Converters.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Syncfusion.Windows.Tools.Controls;

namespace Syncfusion.Windows.Tools
{
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    /// <summary>
    /// Public class for converting color type to brush type.
    /// </summary>
    /// <property name="flag" value="Finished"/>
    [ValueConversion(typeof(Color), typeof(Brush))]
    internal class xColorToBrushConverter : IValueConverter
    {
        #region IValueConverter Members
        /// <summary>
        /// Converts color to solid brush.
        /// </summary>
        /// <param name="value">Color to be converted.</param>
        /// <param name="targetType">Type, the color is to be converted
        /// to.</param>
        /// <param name="parameter">Does not matter.</param>
        /// <param name="culture">Currently used culture. Not used
        /// here.</param>
        /// <returns>Created brush.</returns>
        /// <property name="flag" value="Finished"/>
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

            if (targetType != typeof(Brush))
            {
                throw new InvalidOperationException("Only color to brush conversion is supported by ColorToBrushConverter converter.");
            }

            Color color = (Color)value;
            return new SolidColorBrush(color);

            // return color;
        }

        /// <summary>
        /// Converts solid brush to color brush.
        /// </summary>
        /// <param name="value">Brush to be converted.</param>
        /// <param name="targetType">Type, the color is to be converted
        /// to.</param>
        /// <param name="parameter">Does not matter.</param>
        /// <param name="culture">Currently used culture. Not used
        /// here.</param>
        /// <returns>Created brush.</returns>
        /// <property name="flag" value="Finished"/>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion
    }
    
    /// <summary>
    /// Converts color to value.
    /// </summary>
    /// <property name="flag" value="Finished"/>
    [ValueConversion(typeof(double), typeof(Color))]
    internal class xColorToValueConverter : IValueConverter
    {
        #region IValueConverter Members
        /// <summary>
        /// Converts color to solid brush.
        /// </summary>
        /// <param name="value">Color to be converted.</param>
        /// <param name="targetType">Type, the color is to be converted
        /// to.</param>
        /// <param name="parameter">Does not matter.</param>
        /// <param name="culture">Currently used culture. Not used
        /// here.</param>
        /// <returns>Converted value.</returns>
        /// <property name="flag" value="Finished"/>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Color color = (Color)value;
            return ((int)color.A).ToString("X") + ((int)color.R).ToString("X") + ((int)color.G).ToString("X") + ((int)color.B).ToString("X");
        }

        /// <summary>
        /// Converts solid brush to color brush.
        /// </summary>
        /// <param name="value">Color to be converted.</param>
        /// <param name="targetType">Type, the color is to be converted
        /// to.</param>
        /// <param name="parameter">Does not matter.</param>
        /// <param name="culture">Currently used culture. Not used
        /// here.</param>
        /// <returns>Converted color.</returns>
        /// <property name="flag" value="Finished"/>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }

    /// <summary>
    /// Converts color to word known color.
    /// </summary>
    /// <property name="flag" value="Finished"/>
    [ValueConversion(typeof(double), typeof(Color))]
    internal class xColorToWordKnownColorsConverter : IValueConverter
    {
        #region IValueConverter Members
        /// <summary>
        /// Converts color to word known colors.
        /// </summary>
        /// <param name="value">Color to be converted.</param>
        /// <param name="targetType">Type, the color is to be converted
        /// to.</param>
        /// <param name="parameter">Does not matter.</param>
        /// <param name="culture">Currently used culture. Not used
        /// here.</param>
        /// <returns>Converted word known colors.</returns>
        /// <property name="flag" value="Finished"/>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Color color = (Color)value;

            return Syncfusion.Windows.Tools.Controls.xColorEdit.SuchColor(color)[0];
        }

        /// <summary>
        /// Converts word known colors to colors.
        /// </summary>
        /// <param name="value">Color to be converted.</param>
        /// <param name="targetType">Type, the color is to be converted
        /// to.</param>
        /// <param name="parameter">Does not matter.</param>
        /// <param name="culture">Currently used culture. Not used
        /// here.</param>
        /// <returns>Converted color.</returns>
        /// <property name="flag" value="Finished"/>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }

    /// <summary>
    /// Class implements value to string converter.
    /// </summary>
    /// <property name="flag" value="Finished"/>
    [ValueConversion(typeof(double), typeof(float))]
    internal class xValueToStringConverter : IValueConverter
    {
        #region IValueConverter Members
        /// <summary>
        /// Converts float to string.
        /// </summary>
        /// <param name="value">Float to be converted.</param>
        /// <param name="targetType">Type, the color is to be converted
        /// to.</param>
        /// <param name="parameter">Does not matter.</param>
        /// <param name="culture">Currently used culture. Not used
        /// here.</param>
        /// <returns>Converted color.</returns>
        /// <property name="flag" value="Finished"/>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            float floatValue = (float)value;
            string str = floatValue.ToString("0.00", CultureInfo.InvariantCulture);
            return str;
        }

        /// <summary>
        /// Converts string to float.
        /// </summary>
        /// <param name="value">Float to be converted</param>
        /// <param name="targetType">Type, the color is to be converted
        /// to.</param>
        /// <param name="parameter">Does not matter.</param>
        /// <param name="culture">Currently used culture. Not used
        /// here.</param>
        /// <returns>Converted value.</returns>
        /// <property name="flag" value="Finished"/>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double val,flt;
            bool b = double.TryParse(value as string, out val);
            if (b == true)
            {
                flt = (float)System.Convert.ToDouble(value, CultureInfo.InvariantCulture);
            }
            else
            {
                flt = 0;
            }
            return flt;
        }

        #endregion
    }

    /// <summary>
    /// Implements float to string converter.
    /// Converts float value with range [0..1] to string denoting byte values - the whole numbers in the range [0..255].
    /// </summary>
    /// <remarks>
    /// Is used to show color's ARGB values.
    /// </remarks>
    internal class xRangedFloatToStringConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double doubleVal = System.Convert.ToDouble(value);
            return Math.Round(doubleVal * byte.MaxValue, 0).ToString();
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
            double val,dbl;
            bool b = double.TryParse(value as string, out val);
            if (b == true)
            {
                 dbl = System.Convert.ToDouble(value, CultureInfo.InvariantCulture);
            }
            else
            {
                dbl = 0;
            }
            return (float)(dbl / byte.MaxValue);
        }

        #endregion
    }

    /// <summary>
    /// Public class converts color to string.
    /// </summary>
    /// <property name="flag" value="Finished"/>
    internal class xColorToStringConverter : IValueConverter
    {
        /// <summary>
        /// Stores the current color value.
        /// </summary>
        private Color m_currentColorValue = Colors.White;

        #region IValueConverter Members
        /// <summary>
        /// Converts float to string.
        /// </summary>
        /// <param name="value">Float to be converted.</param>
        /// <param name="targetType">Type, the color is to be converted to.</param>
        /// <param name="parameter">Does not matter.</param>
        /// <param name="culture">Currently used culture. Not used
        /// here.</param>
        /// <returns>Converted string.</returns>
        /// <property name="flag" value="Finished"/>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Color colorValue = (Color)value;
            colorValue = Color.FromArgb(colorValue.A, colorValue.R, colorValue.G, colorValue.B);
            return colorValue.ToString();
        }

        /// <summary>
        /// Converts float to string.
        /// </summary>
        /// <param name="value">Float to be converted.</param>
        /// <param name="targetType">Type, the color is to be converted
        /// to.</param>
        /// <param name="parameter">Does not matter.</param>
        /// <param name="culture">Currently used culture. Not used
        /// here.</param>
        /// <returns>Converted color.</returns>
        /// <property name="flag" value="Finished"/>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string text = (string)value;
            Color color;
            try
            {
                color = (Color)ColorConverter.ConvertFromString(text);
            }
            catch (Exception)
            {
                return m_currentColorValue;
            }

            m_currentColorValue = color;
            return color;
        }

        #endregion
    }

    /// <summary>
    /// Class implements converting RGB to string and back.
    /// </summary>
    /// <property name="flag" value="Finished"/>
    internal class xRGBToStringConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        /// <summary>
        /// Converts RGB to string.
        /// </summary>
        /// <param name="values">The array of values that the source bindings in the <see cref="T:System.Windows.Data.MultiBinding"/> produces. The value <see cref="F:System.Windows.DependencyProperty.UnsetValue"/> indicates that the source binding has no value to provide for conversion.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value.
        /// If the method returns null, the valid null value is used.
        /// A return value of <see cref="T:System.Windows.DependencyProperty"/>.<see cref="F:System.Windows.DependencyProperty.UnsetValue"/> indicates that the converter did not produce a value, and that the binding will use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"/> if it is available, or else will use the default value.
        /// A return value of <see cref="T:System.Windows.Data.Binding"/>.<see cref="F:System.Windows.Data.Binding.DoNothing"/> indicates that the binding does not transfer the value or use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"/> or the default value.
        /// </returns>
        /// <property name="flag" value="Finished"/>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            float floatValue = 0;
            Color color = (Color)values[0];
            if ((bool)values[1])
            {
                switch (parameter.ToString())
                {
                    case "R":
                        floatValue = color.ScR;
                        break;
                    case "G":
                        floatValue = color.ScG;
                        break;
                    case "B":
                        floatValue = color.ScB;
                        break;
                    case "A":
                        floatValue = color.ScA;
                        break;
                }

                string str = floatValue.ToString("0.00", CultureInfo.InvariantCulture);
                return str;
            }
            else
            {
                switch (parameter.ToString())
                {
                    case "R":
                        floatValue = color.R;
                        break;
                    case "G":
                        floatValue = color.G;
                        break;
                    case "B":
                        floatValue = color.B;
                        break;
                    case "A":
                        floatValue = color.A;
                        break;
                }
                
                int intValue = (int)floatValue;
                return intValue.ToString();
            }
        }

        /// <summary>
        /// Converts string to RGB.
        /// </summary>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// An array of values that have been converted from the target value back to the source values.
        /// </returns>
        /// <property name="flag" value="Finished"/>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
