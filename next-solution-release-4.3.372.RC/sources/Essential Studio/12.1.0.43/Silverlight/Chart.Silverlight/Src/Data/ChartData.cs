#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Reflection;

namespace Syncfusion.Windows.Chart
{
    #region class ChartDataPoint

    /// <summary>
    /// Class implementation for ChartDataPoint
    /// </summary>
    public class ChartDataPoint 
    {
        /// <summary>
        /// Gets or sets the X point value.
        /// </summary>
        /// <value>The X value.</value>
        public double X { get; set; }

        /// <summary>
        /// Gets or sets the Y point value.
        /// </summary>
        /// <value>The Y value.</value>
        public double Y { get; set; }

        /// <summary>
        /// Gets or sets the point values.
        /// </summary>
        /// <remarks>
        /// Values array should be used to represent range of Y values that correspond to one X value.
        /// </remarks>
        /// <value>The values.</value>
        public double[] Values { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is visible.
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        public bool Visible { get; set; }

        /// <summary>
        /// Gets or sets label for point.
        /// </summary>
        public string Label
        {
            get;
            set;
        }

        /// <summary>
        /// Get or Set Tag property
        /// </summary>
        public object Tag { get; set; }
    }

    /// <summary>
    /// Class implementation for RotationProperty
    /// </summary>
    public class Rotation
    {
        /// <summary>
        /// used to find the End point value after given angle Rotate.
        /// </summary>
        /// <param name="originpoint">Origin Point</param>
        /// <param name="endpoint">End Point value</param>
        /// <param name="angle">Rotate Angle</param>
        /// <returns>Point value</returns>
        public static Point GeneralPointRotation(Point originpoint, Point endpoint, double angle)
        {
            double ang = angle * Math.PI / 180;
            Point displacement = new Point(endpoint.X - originpoint.X, endpoint.Y - originpoint.Y);
            endpoint.X = (displacement.X * Math.Cos(ang)) - (displacement.Y * Math.Sin(ang));
            endpoint.Y = (displacement.Y * Math.Cos(ang)) + (displacement.X * Math.Sin(ang));
            endpoint.X += originpoint.X;
            endpoint.Y += originpoint.Y;
            return endpoint;
        }
    }

    #endregion
}
