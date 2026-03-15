// <copyright file="Converters.cs" company="Syncfusion">
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
using System.Windows.Media;

namespace Syncfusion.Windows.Tools
{
    /// <summary>
    /// Public class for converting color type to brush type.
    /// </summary>
    /// <property name="flag" value="Finished"/>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [ValueConversion(typeof(Color), typeof(Brush))]
    public class ColorToBrushConverter : IValueConverter
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

        #endregion IValueConverter Members
    }

    /// <summary>
    /// Class for double point converter
    /// </summary>
    [ValueConversion(typeof(Point), typeof(double))]
    public class DoubleToPointConverter : IValueConverter
    {
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
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new Point((double)value, (double)parameter);
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
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

    /// <summary>
    /// Class for double point converter for Y corrdinate
    /// </summary>
    [ValueConversion(typeof(Point), typeof(double))]
    public class DoubleToPointConverterY : IValueConverter
    {
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
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new Point((double)parameter, (double)value);
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
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

    /// <summary>
    /// Converts color to value.
    /// </summary>
    /// <property name="flag" value="Finished"/>
    [ValueConversion(typeof(double), typeof(Color))]
    public class ColorToValueConverter : IValueConverter
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

        #endregion IValueConverter Members
    }

    /// <summary>
    /// Converts color to word known color.
    /// </summary>
    /// <property name="flag" value="Finished"/>
    [ValueConversion(typeof(double), typeof(Color))]
    public class ColorToWordKnownColorsConverter : IValueConverter
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

            return Syncfusion.Windows.Shared.ColorEdit.SuchColor(color)[0];
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

        #endregion IValueConverter Members
    }

    /// <summary>
    /// Class implements value to string converter.
    /// </summary>
    /// <property name="flag" value="Finished"/>
    [ValueConversion(typeof(double), typeof(float))]
    public class ValueToStringConverter : IValueConverter
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
            try
            {
                float flt = (float)System.Convert.ToDouble(value, CultureInfo.InvariantCulture);
                return flt;
            }
            catch
            {
                value = 1f;
                float flt = (float)System.Convert.ToDouble(value, CultureInfo.InvariantCulture);
                return flt;
            }
        }

        #endregion IValueConverter Members
    }

    /// <summary>
    /// Implements float to string converter.
    /// Converts float value with range [0..1] to string denoting byte values - the whole numbers in the range [0..255].
    /// </summary>
    /// <remarks>
    /// Is used to show color's ARGB values.
    /// </remarks>
    public class RangedFloatToStringConverter : IValueConverter
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
            try
            {
                if (value.ToString() != "")
                {
                    double dbl = System.Convert.ToDouble(value, CultureInfo.InvariantCulture);

                    return (float)(dbl / byte.MaxValue);
                }
                else
                {
                    value = 255;
                    double dbl = System.Convert.ToDouble(value, CultureInfo.InvariantCulture);
                    return (float)(dbl / byte.MaxValue);
                }
            }
            catch
            {
                value = 255;
                double dbl = System.Convert.ToDouble(value, CultureInfo.InvariantCulture);
                return (float)(dbl / byte.MaxValue);
            }
        }

        #endregion IValueConverter Members
    }

    /// <summary>
    /// Public class converts color to string.
    /// </summary>
    /// <property name="flag" value="Finished"/>
    public class ColorToStringConverter : IValueConverter
    {
        /// <summary>
        /// Stores the current color value.
        /// </summary>
        private Color m_currentColorValue;

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

        #endregion IValueConverter Members
    }

    /// <summary>
    /// Class implements converting RGB to string and back.
    /// </summary>
    /// <property name="flag" value="Finished"/>
    public class RGBToStringConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        private Color color;

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
            color = (Color)values[0];
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
            object[] output = new object[2];
            float val = -1.0f;
            if (float.TryParse(value.ToString(), out val) && color != null)
            {
                switch (parameter.ToString())
                {
                    case "R":
                        color.R = (byte)val;
                        break;

                    case "G":
                        color.G = (byte)val;
                        break;

                    case "B":
                        color.B = (byte)val;
                        break;

                    case "A":
                        color.A = (byte)val;
                        break;
                }
            }
            output[0] = color;
            output[1] = false;
            return output;
        }

        #endregion IMultiValueConverter Members
    }
}