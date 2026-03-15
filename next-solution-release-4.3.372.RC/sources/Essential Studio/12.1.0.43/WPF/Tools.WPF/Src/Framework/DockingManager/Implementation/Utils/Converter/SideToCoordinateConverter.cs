// <copyright file="SideToCoordinateConverter.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// This class converts side to coordinate for transforming window in side panel.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class SideToCoordinateConverter : IMultiValueConverter
    {
        #region Constants
        /// <summary>
        /// Specifies the horizontal coordinate.
        /// </summary>
        private const string HorizontalCoordinate = "XCoordinate";

        /// <summary>
        /// Specifies the vertical coordinate.
        /// </summary>
        private const string VerticalCoordinate = "YCoordinate";

        /// <summary>
        /// Specifies the parent size.
        /// </summary>
        private const double ParentSize = 21;

        private const double TouchParentSize = 35;
        #endregion

        #region Public method
        /// <summary>
        /// Converts side to coordinate for transforming window in side panel.
        /// </summary>
        /// <param name="values">Values to be converted.</param>
        /// <param name="targetType">Type, the value is to be converted to.</param>
        /// <param name="parameter">Does not matter.</param>
        /// <param name="culture">Currently used culture. Not used here.</param>
        /// <returns>Converted value.</returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double result = 0;
            bool touch = false;

            if (DependencyProperty.UnsetValue != values[1])
            {
                Dock side = (Dock)values[0];
                double size = (double)values[1];
                double percent = (double)values[2];
                double shadowSize = (double)values[3];
                Thickness thickness = (Thickness)values[4];
                string paramTostring = parameter.ToString();
                if (values.Length > 5)
                    touch = (bool)values[5];

                switch (paramTostring)
                {
                    case HorizontalCoordinate:
                        result = CalcXCoordinate(side, size, percent, shadowSize, thickness, touch);
                        break;
                    case VerticalCoordinate:
                        result = CalcYCoordinate(side, size, percent, shadowSize, thickness, touch);
                        break;
                    default:
                        throw new ArgumentException("Incorrect parameter set in Binding");
                }
            }

            return result;
        }

        /// <summary>
        /// This method does nothing.
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
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Calcs the X coordinate.
        /// </summary>
        /// <param name="side">The dock side.</param>
        /// <param name="size">The double size.</param>
        /// <param name="percent">The percent value.</param>
        /// <param name="shadowSize">Size of the shadow.</param>
        /// <param name="thickness">The thickness value.</param>
        /// <returns>return double value.</returns>
        private double CalcXCoordinate(Dock side, double size, double percent, double shadowSize, Thickness thickness, bool touch)
        {
            double result = 0;

            switch (side)
            {
                case Dock.Top:
                case Dock.Bottom:
                    break;
                case Dock.Left:
                    if (touch)
                        result = TouchParentSize - (size * percent) + thickness.Left + thickness.Right;
                    else
                        result = ParentSize - (size * percent) + thickness.Left + thickness.Right;
                    break;
                case Dock.Right:
                    result = -size + (size * percent) - shadowSize;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Calcs the Y coordinate.
        /// </summary>
        /// <param name="side">The dock side.</param>
        /// <param name="size">The double size.</param>
        /// <param name="percent">The percent value.</param>
        /// <param name="shadowSize">Size of the shadow.</param>
        /// <param name="thickness">The thickness value.</param>
        /// <returns>return double value.</returns>
        private double CalcYCoordinate(Dock side, double size, double percent, double shadowSize, Thickness thickness, bool touch)
        {
            double result = 0;

            switch (side)
            {
                case Dock.Right:
                case Dock.Left:
                    break;
                case Dock.Bottom:
                    result = -size + (size * percent) - shadowSize;
                    break;
                case Dock.Top:
                    if (touch)
                        result = TouchParentSize - (size * percent) + thickness.Bottom + thickness.Top;
                    else
                        result = ParentSize - (size * percent) + thickness.Bottom + thickness.Top;
                    break;
            }

            return result;
        }
        #endregion
    }
}
