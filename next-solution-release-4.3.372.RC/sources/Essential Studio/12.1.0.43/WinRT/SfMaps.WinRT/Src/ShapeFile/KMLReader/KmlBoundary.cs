#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Text.RegularExpressions;
#if WINRT
using Windows.Foundation;
using Windows.UI.Xaml.Media;
#else
using System.Windows.Media;
using System.Windows;
#endif

namespace Syncfusion.UI.Xaml.Maps
{
    internal class KmlBoundary
    {
        #region CLR Properties

        internal string LinearString { get; set; }
        internal PointCollection CoordinatePoints { get; set; }

        #endregion

        internal void SetCoordinates()
        {
            string coordinateString = Regex.Replace(LinearString, @"[^0-9,.][\s]", " ");
            string[] coordinates = coordinateString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (coordinates != null && coordinates.Length > 0)
            {
                CoordinatePoints = new PointCollection();
                foreach (var coordinate in coordinates)
                {
                    if (coordinate.Contains(","))
                    {
                        string[] points = coordinate.Split(new[] { ',' }, StringSplitOptions.None);
                        if (points != null && points.Length > 1)
                        {
                            double x = Double.Parse(points[0]);
                            double y = Double.Parse(points[1]);
                            CoordinatePoints.Add(new Point(x, y));
                        }
                    }
                }
            }
        }
    }
}
