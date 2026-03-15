// <copyright file="StringToPoints.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Syncfusion.Windows.Diagram
{
    /// <summary>
    /// This class converts string to points.
    /// </summary>
    [ValueConversion(typeof(String), typeof(List<Point>))]
    public class StringToPoints : IValueConverter
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
            List<Point> intPoints = new List<Point>();

            if (value == null)
            {
                return null;
            }

            if (value == DependencyProperty.UnsetValue)
            {
                return Binding.DoNothing;
            }

            if (targetType != typeof(List<Point>))
            {
                throw new InvalidOperationException("Only String to List<Point> conversion is supported by StringToPointsConverter converter.");
            }

            try
            {
                String s = value.ToString();
                List<String> pts = new List<String>();
                char[] c = { ' ' };
                pts.AddRange(s.Split(c, StringSplitOptions.RemoveEmptyEntries).ToList());
                foreach (String pt in pts)
                {
                    char[] t = { ',' };
                    string[] xy = pt.Split(t, StringSplitOptions.RemoveEmptyEntries);
                    intPoints.Add(new Point(double.Parse(xy[0]), double.Parse(xy[1])));
                }
            }
            catch 
            {
                throw new InvalidOperationException("Points should be specified in \"X1,Y1 X2,Y2 …\" format.");
            }

            return intPoints;
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
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
