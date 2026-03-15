#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows.Data;
using System.Windows;
using System;

namespace Syncfusion.Windows.Tools.Controls
{   
    /// <summary>
    /// Convertor class for converting Boolean value to Visibility property value
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Method for performing the conversion
        /// </summary>
        /// <param name="value">converts this value object into bool type</param>
        /// <param name="targetType">passing target type</param>
        /// <param name="parameter">passing parameter object</param>
        /// <param name="culture">passing culture CultureInfo</param>
        /// <returns>Type : Visibility</returns>
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool bval = (bool)value;
            if (bval)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Method for performing the conversion  in the reverse direction
        /// </summary>
        /// <param name="value">passing value object</param>
        /// <param name="targetType">passing target type</param>
        /// <param name="parameter">passing parameter object</param>
        /// <param name="culture">passing culture CultureInfo</param>
        /// <returns>Type : throw</returns>
        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {            
            if (value is Visibility)
            {
                Visibility visible = (Visibility)value;
                if (visible == Visibility.Collapsed)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }

    /// <summary>
    /// Convertor class for converting Visibility value to Boolean value
    /// </summary>
    public class VisibilityToBooleanConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Method for performing the conversion
        /// </summary>
        /// <param name="value">converts this value object into bool type</param>
        /// <param name="targetType">passing target type</param>
        /// <param name="parameter">passing parameter object</param>
        /// <param name="culture">passing culture CultureInfo</param>
        /// <returns>Type : bool</returns>
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility val = (Visibility)value;
            if (val == Visibility.Visible)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Method for performing the conversion  in the reverse direction
        /// </summary>
        /// <param name="value">passing value object</param>
        /// <param name="targetType">passing target type</param>
        /// <param name="parameter">passing parameter object</param>
        /// <param name="culture">passing culture CultureInfo</param>
        /// <returns>Type : Visibility</returns>
        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool bval = (bool)value;
            if (bval)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }
        #endregion
    }

    /// <summary>
    /// Convertor class for converting Slider value to Text property value
    /// </summary>
    public class ValueToTextConverter : IValueConverter
    {
        
        #region IValueConverter Members

        /// <summary>
        /// Method for performing the conversion
        /// </summary>
        /// <param name="value">converts this value object into double type with two decimal points</param>
        /// <param name="targetType">passing target type</param>
        /// <param name="parameter">passing parameter object</param>
        /// <param name="culture">passing culture CultureInfo</param>
        /// <returns>Type : Double</returns>
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double txt = Math.Round(System.Convert.ToDouble(value), 2);
            return txt;

        }

        /// <summary>
        /// Method for performing the conversion  in the reverse direction
        /// </summary>
        /// <param name="value">passing value object</param>
        /// <param name="targetType">passing target type</param>
        /// <param name="parameter">passing parameter object</param>
        /// <param name="culture">passing culture CultureInfo</param>
        /// <returns>Type : Double</returns>
        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.ToString() == String.Empty)
            {
                return 0.0;
            }
            else
            {
                return System.Convert.ToDouble(value);
            }
        }

        #endregion
    }

    /// <summary>
    /// Convertor class for converting Color to Brush.
    /// </summary>
    public class ColorToBrushConverter : IValueConverter
    {

        #region IValueConverter Members

        /// <summary>
        /// Method for performing the conversion
        /// </summary>
        /// <param name="value">converts this value object into brush type</param>
        /// <param name="targetType">passing target type</param>
        /// <param name="parameter">passing parameter object</param>
        /// <param name="culture">passing culture CultureInfo</param>
        /// <returns>Type : Brush</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)value);
        }

        /// <summary>
        /// Method for performing the conversion  in the reverse direction. Not implemented.
        /// </summary>
        /// <param name="value">passing value object</param>
        /// <param name="targetType">passing target type</param>
        /// <param name="parameter">passing parameter object</param>
        /// <param name="culture">passing culture CultureInfo</param>
        /// <returns>Type : Object</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}