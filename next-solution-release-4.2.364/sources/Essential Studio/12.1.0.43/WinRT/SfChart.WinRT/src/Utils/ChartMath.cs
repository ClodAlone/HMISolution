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
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Contains static methods for performing certain mathematical calculations.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public static class ChartMath
    {
        #region Constants

        internal const float MARGINS_RATIO = 0.03f;

        /// <summary>
        /// Initializes ToDegree
        /// </summary>
        public const double ToDegree = 180 / Math.PI;

        /// <summary>
        /// Initializes ToRadial
        /// </summary>
        public const double ToRadial = Math.PI / 180;

        /// <summary>
        /// Initializes Percent
        /// </summary>
        public const double Percent = 0.01d;

        /// <summary>
        /// Initializes DoublePI
        /// </summary>
        public const double DoublePI = 2 * Math.PI;

        /// <summary>
        /// Initializes HalfPI
        /// </summary>
        public const double HalfPI = 0.5 * Math.PI;

        /// <summary>
        /// Initializes OneAndHalfPI
        /// </summary>
        public const double OneAndHalfPI = 1.5 * Math.PI;

        /// <summary>
        /// The epsilon
        /// </summary>
        public const double Epsilon = 0.00001;
        #endregion

        #region Public methods

        public static bool IntersectWith(this IList<Rect> rectCollection, Rect newRect)
        {
            foreach (var existingRect in rectCollection.Reverse())
            {
                if (existingRect.IntersectsWith(newRect))
                {
                    return true;
                }
            }
            return false;
        }

        public static Vector3D GetNormal(Vector3D v1, Vector3D v2, Vector3D v3)
        {
            Vector3D n = (v1 - v2) * (v3 - v2);
            double l = n.GetLength();

            if (l < Epsilon)
            {
                l = 0;
            }

            return new Vector3D(n.X / l, n.Y / l, n.Z / l);
        }

#if WPF || SILVERLIGHT_UNCOMMON

        public static TranslateTransform Translate(Point startPoint, Point currentPoint)
        {
            return new TranslateTransform()
                {
                    X = currentPoint.X - startPoint.X,
                    Y = currentPoint.Y - startPoint.Y
                };
        }

#endif

        /// <summary>
        /// Solves quadratic equation in form a*x^2 + b*x + c = 0
        /// </summary>
        /// <param name="a">The A component</param>
        /// <param name="b">The B component</param>
        /// <param name="c">The C component</param>
        /// <param name="root1">First root.</param>
        /// <param name="root2">Second root.</param>
        /// <returns>Bool value</returns>
        public static bool SolveQuadraticEquation(double a, double b, double c, out double root1, out double root2)
        {
            root1 = 0;
            root2 = 0;

            if (a != 0)
            {
                double d = b * b - 4 * a * c;

                if (d >= 0)
                {
                    double sd = Math.Sqrt(d);

                    root1 = (-b - sd) / (2 * a);
                    root2 = (-b + sd) / (2 * a);

                    return true;
                }
            }
            else if (b != 0)
            {
                root1 = -c / b;
                root2 = -c / b;

                return true;
            }

            return false;
        }

       
        //public static Vector SolveSimultaneousEquations(Vector a, Vector b, Vector c)
        //{
        //    double d = a.X * b.Y - a.Y * b.X;
        //    double d1 = c.X * b.Y - c.Y * b.X;
        //    double d2 = a.X * c.Y - a.Y * c.X;

        //    return new Vector(d1 / d, d2 / d);
        //}

        /// <summary>
        /// Gets minimal value from <c>value</c> or <c>min</c> and maximal from <c>value</c> or <c>max</c>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimal value.</param>
        /// <param name="max">The maximal value.</param>
        /// <returns>The MinMax value</returns>
        public static double MinMax(double value, double min, double max)
        {
            return value > max ? max : (value < min ? min : value);
        }

        /// <summary>
        /// Gets minimal value from parameters.
        /// </summary>
        /// <param name="values">The parameters</param>
        /// <returns>The minimal value.</returns>
        public static double Min(params double[] values)
        {
            double result = values[0];

            for (int i = 1; i < values.Length; i++)
            {
                result = Math.Min(result, values[i]);
            }

            return result;
        }

        /// <summary>
        /// Gets maximal value from parameters.
        /// </summary>
        /// <param name="values">The parameters</param>
        /// <returns>The maximal value.</returns>
        public static double Max(params double[] values)
        {
            double result = values[0];

            for (int i = 1; i < values.Length; i++)
            {
                result = Math.Max(result, values[i]);
            }

            return result;
        }

        /// <summary>
        /// Gets maximal value from parameter or zero.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The double value</returns>
        public static double MaxZero(double value)
        {
            return value > 0d ? value : 0d;
        }

        /// <summary>
        /// Gets minimal value from parameter or zero.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The MinZero value</returns>
        public static double MinZero(double value)
        {
            return value < 0d ? value : 0d;
        }

        /// <summary>
        /// Rounds the specified value.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="div">The divider.</param>
        /// <param name="up">if set to <c>true</c> value will be rounded up.</param>
        /// <returns>The Round off value</returns>
        public static double Round(double x, double div, bool up)
        {
            return (int)(up ? Math.Ceiling(x / div) : Math.Floor(x / div)) * div;
        }

#if !WINDOWS_PHONE

        /// <summary>
        /// Reduces the points using Douglas-Peucker line approximation algorithm.
        /// </summary>
        /// <param name="totalPoints"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static PointCollection ReducePointsUsingDPAlg(PointCollection totalPoints, double tolerance)
        {
            PointCollection reducedPoints = new PointCollection();

            if (totalPoints == null || totalPoints.Count < 3)
                return totalPoints;

            SortedDictionary<int, Point> sortedPoints = new SortedDictionary<int, Point>();

            //List<int> indexes = new List<int>();

            ReducePointsRecursively(totalPoints, 0, totalPoints.Count - 1, tolerance, sortedPoints);

            reducedPoints.Concat<Point>(sortedPoints.Values);

            sortedPoints.Clear();

            return reducedPoints;
        }

        /// <summary>
        /// Eliminates the points by calculating the perpendicular distance of a point from a line.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="firstPoint"></param>
        /// <param name="lastPoint"></param>
        /// <param name="tolerance"></param>
        /// <param name="sortedPoints"></param>
        private static void ReducePointsRecursively(PointCollection points, int firstPoint, int lastPoint, double tolerance
            , SortedDictionary<int, Point> sortedPoints)
        {

            double maxDistance = 0;
            int maxDistanceIndex = 0;

            for (int index = firstPoint; index < lastPoint; index++)
            {
                double distance = CalcPerpendicularDistance
                    (points[firstPoint], points[lastPoint], points[index]);

                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    maxDistanceIndex = index;
                }
            }

            if (maxDistance > tolerance && maxDistanceIndex != 0)
            {
                sortedPoints.Add(maxDistanceIndex, points[maxDistanceIndex]);

                //sortedPoints.Add(maxDistanceIndex);

                ReducePointsRecursively(points, firstPoint,
                maxDistanceIndex, tolerance, sortedPoints);

                ReducePointsRecursively(points, maxDistanceIndex,
                lastPoint, tolerance, sortedPoints);
            }

        }

#endif

        /// <summary>
        /// Calculates the perpendicular distance of point from a line.
        /// </summary>
        /// <param name="Point1">Starting point of the line.</param>
        /// <param name="Point2">Ending point of the line</param>
        /// <param name="Point">The point</param>
        /// <returns></returns>
        private static double CalcPerpendicularDistance(Point Point1, Point Point2, Point Point)
        {
            //Calculating the area of the tringle
            double area = Math.Abs(.5 * (Point1.X * Point2.Y + Point2.X *
            Point.Y + Point.X * Point1.Y - Point2.X * Point1.Y - Point.X *
            Point2.Y - Point1.X * Point.Y));

            //Calculating the base of the triangle
            double bottom = Math.Sqrt(Math.Pow(Point1.X - Point2.X, 2) +
            Math.Pow(Point1.Y - Point2.Y, 2));

            //Calculating the height of the triangle i.e., the perpendicular distance...
            double height = area / bottom * 2;

            return height;
        }

        /// <summary>
        /// return point values from the given origin,end and angle points
        /// </summary>
        /// <param name="originpoint"></param>
        /// <param name="endpoint"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Point GeneralPointRotation(Point originpoint, Point endpoint, double angle)
        {
            double ang = angle * Math.PI / 180;
            double radius = endpoint.X / 2;
            endpoint.X = ((radius) * Math.Cos(ang));
            endpoint.Y = ((radius) * Math.Sin(ang));
            return endpoint;
        }

        #endregion
    }

    internal static class ChartLayoutUtils
    {
        #region Constants
        /// <summary>
        /// Initializes c_half
        /// </summary>
        private const double C_half = 0.5d;
        #endregion

        #region Public methods

        /// <summary>
        /// Gets the rect by center.
        /// </summary>
        /// <param name="center">The center.</param>
        /// <param name="size">The size value.</param>
        /// <returns>The Rect value</returns>
        public static Rect GetRectByCenter(Point center, Size size)
        {
            return new Rect(center.X - size.Width / 2, center.Y - size.Height / 2, size.Width, size.Height);
        }

        /// <summary>
        /// Gets the rect by center.
        /// </summary>
        /// <param name="cx">The cx value.</param>
        /// <param name="cy">The cy value.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>The Rect value</returns>
        public static Rect GetRectByCenter(double cx, double cy, double width, double height)
        {
            return new Rect(cx - width / 2, cy - height / 2, width, height);
        }

        /// <summary>
        /// Gets the center.
        /// </summary>
        /// <param name="size">The size value.</param>
        /// <returns>The vector center value</returns>
        public static Point GetCenter(Size size)
        {
            return new Point(C_half * size.Width, C_half * size.Height);
        }

        /// <summary>
        /// Gets the center.
        /// </summary>
        /// <param name="rect">The rect value.</param>
        /// <returns>The center point value</returns>
        public static Point GetCenter(Rect rect)
        {
            Point centerPoint = GetCenter(new Size(rect.Width, rect.Height));
            return new Point(centerPoint.X + rect.Left, centerPoint.Y + rect.Top);
        }

        /// <summary>
        /// Subtracts the thickness.
        /// </summary>
        /// <param name="rect">The rect value.</param>
        /// <param name="thickness">The thickness.</param>
        /// <returns>The Rectangle</returns>
        public static Rect Subtractthickness(Rect rect, Thickness thickness)
        {
            rect.X += thickness.Left;
            rect.Y += thickness.Top;
            if (rect.Width > thickness.Left + thickness.Right)
            {
                rect.Width -= thickness.Left + thickness.Right;
            }
            else
                rect.Width = 0;

            if (rect.Height > (thickness.Top + thickness.Bottom))
            {
                rect.Height -= thickness.Top + thickness.Bottom;
            }
            else
                rect.Height = 0;

            return rect;
        }

        /// <summary>
        /// Subtracts the thickness.
        /// </summary>
        /// <param name="size">The size value.</param>
        /// <param name="thickness">The thickness.</param>
        /// <returns>Returns the size</returns>
        public static Size Subtractthickness(Size size, Thickness thickness)
        {
            size.Width = Math.Max(size.Width - thickness.Left - thickness.Right, 0);
            size.Height = Math.Max(size.Height - thickness.Top - thickness.Bottom, 0);

            return size;
        }

        /// <summary>
        /// The Addthickness method
        /// </summary>
        /// <param name="rect">The Rect value</param>
        /// <param name="thickness">The thickness</param>
        /// <returns>The rectangle</returns>
        /// <seealso cref="ChartLayoutUtils"/>
        public static Rect Addthickness(Rect rect, Thickness thickness)
        {
            rect.X -= thickness.Left;
            rect.Y -= thickness.Top;
            rect.Width += thickness.Left + thickness.Right;
            rect.Height += thickness.Top + thickness.Bottom;

            return rect;
        }

        /// <summary>
        /// The Addthickness method
        /// </summary>
        /// <param name="size">The size value</param>
        /// <param name="thickness">The thickness value</param>
        /// <returns>Returns the size</returns>
        ///  <seealso cref="ChartLayoutUtils"/>
        public static Size Addthickness(Size size, Thickness thickness)
        {
            if (thickness.Left >= 0 && thickness.Right >= 0)
                size.Width += thickness.Left + thickness.Right;
            if (thickness.Top >= 0 && thickness.Bottom >= 0)
                size.Height += thickness.Top + thickness.Bottom;
            return size;
        }

        /// <summary>
        /// Checks the members of size by infinity.
        /// </summary>
        /// <param name="size">The size value.</param>
        /// <returns>Returns the size</returns>
        public static Size CheckSize(Size size)
        {
            size.Width = double.IsInfinity(size.Width) ? 0d : size.Width;
            size.Height = double.IsInfinity(size.Height) ? 0d : size.Height;

            return size;
        }
        #endregion
    }
}
