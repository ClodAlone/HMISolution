#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Media;
#else 
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    public class ArrowLine  : DependencyObject
    {
        protected PathGeometry pathGeometry;
        protected PathFigure pathFigureLine;
#if WINDOWS_PHONE
        System.Windows.Media.LineSegment segmentLine;
#else
        Windows.UI.Xaml.Media.LineSegment segmentLine;
#endif
        PathFigure pathFigureHead;
        PolyLineSegment polySegmentHead;
        double arrowAngle = 45, arrowLength = 12.0;
 
        /// <summary>
        ///     Identifies the X1 dependency property.
        /// </summary>
        public static readonly DependencyProperty X1Property =
            DependencyProperty.Register("X1",
                typeof(double), typeof(ArrowLine),
                new PropertyMetadata(0.0));

        /// <summary>
        ///     Gets or sets the x-coordinate of the ArrowLine start point.
        /// </summary>
        public double X1
        {
            set { SetValue(X1Property, value); }
            get { return (double)GetValue(X1Property); }
        }

        /// <summary>
        ///     Identifies the Y1 dependency property.
        /// </summary>
        public static readonly DependencyProperty Y1Property =
            DependencyProperty.Register("Y1",
                typeof(double), typeof(ArrowLine),
                new PropertyMetadata(0.0));

        /// <summary>
        ///     Gets or sets the y-coordinate of the ArrowLine start point.
        /// </summary>
        public double Y1
        {
            set { SetValue(Y1Property, value); }
            get { return (double)GetValue(Y1Property); }
        }

        /// <summary>
        ///     Identifies the X2 dependency property.
        /// </summary>
        public static readonly DependencyProperty X2Property =
            DependencyProperty.Register("X2",
                typeof(double), typeof(ArrowLine),
                new PropertyMetadata(0.0));

        /// <summary>
        ///     Gets or sets the x-coordinate of the ArrowLine end point.
        /// </summary>
        public double X2
        {
            set { SetValue(X2Property, value); }
            get { return (double)GetValue(X2Property); }
        }

        /// <summary>
        ///     Identifies the Y2 dependency property.
        /// </summary>
        public static readonly DependencyProperty Y2Property =
            DependencyProperty.Register("Y2",
                typeof(double), typeof(ArrowLine),
                new PropertyMetadata(0.0));

        /// <summary>
        ///     Gets or sets the y-coordinate of the ArrowLine end point.
        /// </summary>
        public double Y2
        {
            set { SetValue(Y2Property, value); }
            get { return (double)GetValue(Y2Property); }
        }

        public ArrowLine()
        {
            pathGeometry = new PathGeometry();

            pathFigureLine = new PathFigure();
#if WINDOWS_PHONE
            segmentLine = new System.Windows.Media.LineSegment();
#else
            segmentLine = new Windows.UI.Xaml.Media.LineSegment();
#endif
            pathFigureLine.Segments.Add(segmentLine);

            pathFigureHead = new PathFigure();
            polySegmentHead = new PolyLineSegment();
            pathFigureHead.Segments.Add(polySegmentHead);
        }

         
        public Geometry GetGeometry()
        {
            Point point1 = new Point(X1, Y1);
            Point point2 = new Point(X2, Y2);
            pathGeometry.Figures.Clear();
            pathFigureLine.StartPoint = point1;
            segmentLine.Point = point2;
            pathGeometry.Figures.Add(pathFigureLine);
            pathGeometry.Figures.Add(CalculateArrow(pathFigureHead, point1, point2));
            return pathGeometry;
         }

        PathFigure CalculateArrow(PathFigure pathfigure, Point point1, Point point2)
        {
            double length = 0;
            double lengthSquared = 0;

            Matrix matx = new Matrix();
            //To made the matx as identity matrix
            matx.M11 = 1;
            matx.M22 = 1;

            //Find the width and height 
            Point vectPoint = new Point(point1.X - point2.X, point1.Y - point2.Y);

            //To find the vector normalization for the width and height , arrow length
            vectPoint = FindNormalization(vectPoint, arrowLength);
            length = 1 * arrowLength;
            lengthSquared = length * length;

            PolyLineSegment polyseg = pathfigure.Segments[0] as PolyLineSegment;
            polyseg.Points.Clear();

            //Rotation matrix calculation and start point calculation of the arrow
            var angleRadians = (2 * Math.PI * (arrowAngle / 2)) / 360;
            var sine = Math.Sin(angleRadians);
            var cosine = Math.Cos(angleRadians);
            var matrix = new Matrix(cosine, sine, -sine, cosine, 0, 0);
            matx = MultiplyMatrixes(matx, matrix);
            Point tempPoint = MultiplyMatrixVector(vectPoint, matx);
            pathfigure.StartPoint = new Point(point2.X + tempPoint.X, point2.Y + tempPoint.Y);

            polyseg.Points.Add(point2);

            //Rotation matrix calculation and end point calculation of the arrow
            angleRadians = (2 * Math.PI * -arrowAngle) / 360;
            sine = Math.Sin(angleRadians);
            cosine = Math.Cos(angleRadians);
            matrix = new Matrix(cosine, sine, -sine, cosine, 0, 0);
            matx = MultiplyMatrixes(matx, matrix);
            tempPoint = MultiplyMatrixVector(vectPoint, matx);
            polyseg.Points.Add(new Point(point2.X + tempPoint.X, point2.Y + tempPoint.Y));
            pathfigure.IsClosed = true;
            return pathfigure;
        }

        //Vector and matrix multiplication
        Point MultiplyMatrixVector(Point point, Matrix mat)
        {
            double x = mat.M11 * point.X + mat.M12 * point.Y;
            double y = mat.M21 * point.X + mat.M22 * point.Y;
            return new Point(x, y);
        }

        //Matrixes multiplication
        Matrix MultiplyMatrixes(Matrix mat1, Matrix mat2)
        {
            double m11 = mat1.M11 * mat2.M11 + mat1.M12 * mat2.M21;
            double m12 = mat1.M11 * mat2.M12 + mat1.M12 * mat2.M22;
            double m21 = mat1.M21 * mat2.M11 + mat1.M22 * mat2.M21;
            double m22 = mat1.M21 * mat2.M12 + mat1.M22 * mat2.M22;
            return new Matrix(m11, m12, m21, m22, 0, 0);
        }

        //Vector Normalization
        private Point FindNormalization(Point vectPoint, double len)
        {
            double length = Math.Sqrt((vectPoint.X * vectPoint.X) + (vectPoint.Y * vectPoint.Y));
            return new Point((vectPoint.X / length) * len, (vectPoint.Y / length) * len);
        }
    }
}
