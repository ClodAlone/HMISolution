#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
#if WINRT
using Windows.Foundation;
#else
using System.Windows;
#endif

namespace Syncfusion.UI.Xaml.Maps
{
    internal class KmlBoundingBox
    {
        #region Internal Fields

        internal bool hasBoundingValues;

        #endregion

        #region CLR Properties

        #region East
        private double east = 180;
        internal double East
        {
            get
            {
                return east;
            }
            set
            {
                east = value;
            }
        }
        #endregion

        #region West
        private double west = -180;
        internal double West
        {
            get
            {
                return west;
            }
            set
            {
                west = value;
            }
        }
        #endregion

        #region North
        private double north = 90;
        internal double North
        {
            get
            {
                return north;
            }
            set
            {
                north = value;
            }
        }
        #endregion

        #region South
        private double south = -90;
        internal double South
        {
            get
            {
                return south;
            }
            set
            {
                south = value;
            }
        }
        #endregion

        #endregion

        #region Implementation

        internal void SetBoundingBoxValues(bool isBaseLayer, ShapeFileKmlReader kmlReader)
        {
            if (!hasBoundingValues)
            {
                List<KmlPolygon> polygonList = kmlReader.PolygonList;
                List<KmlPoint> pointList = kmlReader.PointList;
                double minX = 0, maxX = 0, minY = 0, maxY = 0;
                if (polygonList.Count > 0)
                {
                    minX = maxX = polygonList[0].OuterBoundary.CoordinatePoints[0].X;
                    minY = maxY = polygonList[0].OuterBoundary.CoordinatePoints[0].Y;
                    foreach (KmlPolygon polygon in polygonList)
                    {
                        foreach (Point point in polygon.OuterBoundary.CoordinatePoints)
                        {
                            minX = Math.Min(minX, point.X);
                            minY = Math.Min(minY, point.Y);
                            maxX = Math.Max(maxX, point.X);
                            maxY = Math.Max(maxY, point.Y);
                        }
                    }
                }
                else if (pointList.Count > 0)
                {
                    minX = maxX = pointList[0].Point.X;
                    minY = maxY = pointList[0].Point.Y;
                    foreach (KmlPoint kmlPoint in pointList)
                    {
                        minX = Math.Min(minX, kmlPoint.Point.X);
                        minY = Math.Min(minY, kmlPoint.Point.Y);
                        maxX = Math.Max(maxX, kmlPoint.Point.X);
                        maxY = Math.Max(maxY, kmlPoint.Point.Y);
                    }
                }
                minX = Math.Floor(minX) - 1;
                minY = Math.Floor(minY) - 1;
                maxX = Math.Ceiling(maxX) + 1;
                maxY = Math.Ceiling(maxY) + 1;

                if (minX < -180)
                    minX = -180;
                if (maxX > 180)
                    maxX = 180;
                if (minY < -90)
                    minY = -90;
                if (maxY > 90)
                    maxY = 90;
                if (isBaseLayer)
                {
                    east = maxX;
                    west = minX;
                    north = maxY;
                    south = minY;
                }
            }
        }

        #endregion
    }
}
