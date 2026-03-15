// <copyright file="ColorToTransparentBrushConverter.cs" company="Syncfusion">
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

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Color to Transparent Brush converter
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ColorToTransparentBrushConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding data source.</param>
        /// <param name="targetType">The type of the binding data target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            if (!(value is Color))
            {
                throw new ArgumentException("Value of Color type is expected.", "value");
            }

            Color color = (Color)value;

            Point startPoint = new Point(0.5, 0);
            Point endPoint = new Point(0.5, 1);

            LinearGradientBrush resultBrush = new LinearGradientBrush();

            resultBrush.StartPoint = startPoint;
            resultBrush.EndPoint = endPoint;
            resultBrush.GradientStops.Add(new GradientStop(Colors.Transparent, 1.0));
            Color clr = (Color) ColorConverter.ConvertFromString(value.ToString().Replace("#FF","#99"));
            resultBrush.GradientStops.Add(new GradientStop(clr, 0.565));
            clr = (Color)ColorConverter.ConvertFromString(value.ToString().Replace("#FF", "#19"));
            resultBrush.GradientStops.Add(new GradientStop(clr, 0.935));
            clr = (Color)ColorConverter.ConvertFromString(value.ToString().Replace("#FF", "#7F"));
            resultBrush.GradientStops.Add(new GradientStop(clr, 0.717));
            clr = (Color)ColorConverter.ConvertFromString(value.ToString().Replace("#FF", "#4C"));
            resultBrush.GradientStops.Add(new GradientStop(clr, 0.817));
            clr = (Color)ColorConverter.ConvertFromString(value.ToString().Replace("#FF", "#34"));
            resultBrush.GradientStops.Add(new GradientStop(clr, 0.874));
            clr = (Color)ColorConverter.ConvertFromString(value.ToString().Replace("#FF", "#66"));
            resultBrush.GradientStops.Add(new GradientStop(clr, 0.774));

            return resultBrush;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding data target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
    public class ColorToTransparentBrushConverter1 : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding data source.</param>
        /// <param name="targetType">The type of the binding data target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            if (!(value is Color))
            {
                throw new ArgumentException("Value of Color type is expected.", "value");
            }

            Color color = (Color)value;
            Color color1 = (Color)ColorConverter.ConvertFromString("#4B000000");
            float percent = (float)100.00;
            float amountFrom = 1.0f - 100;

            return new SolidColorBrush(Color.FromArgb((byte)(color.A * amountFrom + color1.A * percent), (byte)(color.R * amountFrom + color1.R * percent), (byte)(color.G * amountFrom + color1.G * percent), (byte)(color.B * amountFrom + color1.B * percent)));

            
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding data target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        #endregion
    }

    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            if (!(value is Color))
            {
                throw new ArgumentException("Value of Color type is expected.", "value");
            }

            Color color = (Color)value;
            SolidColorBrush scolorBrush = new SolidColorBrush(color);
            return scolorBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
    /// <summary>
    /// this converter is used in ribbon Blendtheme for tabbutton mouseover brush with in the contexttabgroup
    /// </summary>
    public class Blend_ColorToTransparentBrushConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding data source.</param>
        /// <param name="targetType">The type of the binding data target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            if (!(value is Color))
            {
                throw new ArgumentException("Value of Color type is expected.", "value");
            }

            Color color = (Color)value;

            Point startPoint = new Point(0.5, 1);
            Point endPoint = new Point(0.5, 0);

            LinearGradientBrush resultBrush = new LinearGradientBrush();

            resultBrush.StartPoint = startPoint;
            resultBrush.EndPoint = endPoint;
            resultBrush.GradientStops.Add(new GradientStop((Color)(ColorConverter.ConvertFromString("#FF2F2E30")), 0.73));
            Color clr = (Color)ColorConverter.ConvertFromString(value.ToString().Replace("#FF", "#99"));
            resultBrush.GradientStops.Add(new GradientStop(clr, 1));
          

            

            return resultBrush;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding data target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
    /// <summary>
    /// This converter is used for Contexttabgorup backgorund brush in Ribbon Blend theme
    /// 
    /// </summary>
      public class Blend_ColorToTransparentBrushConverter1 : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding data source.</param>
        /// <param name="targetType">The type of the binding data target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            if (!(value is Color))
            {
                throw new ArgumentException("Value of Color type is expected.", "value");
            }

            Color color = (Color)value;

            Point startPoint = new Point(0.5,0);
            Point endPoint = new Point(0.5, 1);

            LinearGradientBrush resultBrush = new LinearGradientBrush();

            resultBrush.StartPoint = startPoint;
            resultBrush.EndPoint = endPoint;
            resultBrush.GradientStops.Add(new GradientStop(Colors.Transparent , 1));
            Color clr = (Color)ColorConverter.ConvertFromString(value.ToString().Replace("#FF", "#BD"));
            resultBrush.GradientStops.Add(new GradientStop(clr, 0.077));
          

            

            return resultBrush;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding data target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
}
