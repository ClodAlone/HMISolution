#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Path factory
    /// </summary>
    public class PathFactory
    {
        #region ClosedCurve
        /// <summary>
        /// Creates the closed curve.
        /// </summary>
        /// <param name="pts">The points.</param>
        /// <returns>The graphics path.</returns>
        public static GraphicsPath CreateClosedCurve(PointF[] pts)
        {
            GraphicsPath pathToReturn = null;

            if (pts.Length == 2 && (pts[0] != pts[1]))
            {
                pathToReturn = PathFactory.CreateLine(pts[0], pts[1]);
            }
            else if (pts.Length > 2)
            {
                pathToReturn = new GraphicsPath();
                pathToReturn.AddClosedCurve(pts);
            }

            return pathToReturn;
        }

        /// <summary>
        /// Creates the closed curve.
        /// </summary>
        /// <param name="pts">The points.</param>
        /// <returns>The graphics path.</returns>
        public static GraphicsPath CreateClosedCurve(Point[] pts)
        {
            GraphicsPath pathToReturn = null;

            if (pts.Length == 2 && (pts[0] != pts[1]))
            {
                pathToReturn = PathFactory.CreateLine(pts[0], pts[1]);
            }
            else if (pts.Length > 2)
            {
                pathToReturn = new GraphicsPath();
                pathToReturn.AddClosedCurve(pts);
            }

            return pathToReturn;
        }
        #endregion

        #region Curve
        /// <summary>
        /// Creates the curve.
        /// </summary>
        /// <param name="ptStart">The start point.</param>
        /// <param name="ptEnd">The end point.</param>
        /// <param name="ptCtrl">The control point.</param>
        /// <returns>The graphics path.</returns>
        public static GraphicsPath CreateCurve(PointF ptStart, PointF ptEnd, PointF ptCtrl)
        {
            PointF[] pts = new PointF[] { ptStart, ptCtrl, ptEnd };

            return CreateCurve(pts);
        }

        /// <summary>
        /// Creates the curve.
        /// </summary>
        /// <param name="pts">The points.</param>
        /// <returns>The graphics path.</returns>
        public static GraphicsPath CreateCurve(PointF[] pts)
        {
            GraphicsPath pathToReturn = new GraphicsPath();

            pathToReturn.AddCurve(pts);

            return pathToReturn;
        }

        /// <summary>
        /// Creates the curve.
        /// </summary>
        /// <param name="ptStart">The start point.</param>
        /// <param name="ptEnd">The end point.</param>
        /// <param name="ptCtrl">The CTRL point.</param>
        /// <returns>The graphics path.</returns>
        public static GraphicsPath CreateCurve(Point ptStart, Point ptEnd, Point ptCtrl)
        {
            Point[] pts = new Point[] { ptStart, ptCtrl, ptEnd };

            return CreateCurve(pts);
        }

        /// <summary>
        /// Creates the curve.
        /// </summary>
        /// <param name="pts">The points.</param>
        /// <returns>The graphics path.</returns>
        public static GraphicsPath CreateCurve(Point[] pts)
        {
            GraphicsPath pathToReturn = new GraphicsPath();

            pathToReturn.AddCurve(pts);

            return pathToReturn;
        }
        #endregion

        #region Bezier
        /// <summary>
        /// Creates the bezier line.
        /// </summary>
        /// <param name="ptStart">The start point.</param>
        /// <param name="ptStartCtrl">The start control point.</param>
        /// <param name="ptEnd">The end point.</param>
        /// <param name="ptEndCtrl">The end control point.</param>
        /// <returns>The graphics path.</returns>
        public static GraphicsPath CreateBezier(PointF ptStart, PointF ptStartCtrl, PointF ptEnd, PointF ptEndCtrl)
        {
            GraphicsPath pathToReturn = new GraphicsPath();
            pathToReturn.AddBezier(ptStart, ptStartCtrl, ptEndCtrl, ptEnd);

            return pathToReturn;
        }

        /// <summary>
        /// Creates the bezier line.
        /// </summary>
        /// <param name="pts">The array of points.</param>
        public static GraphicsPath CreateBezier(PointF[] pts)
        {
            GraphicsPath pathToReturn = new GraphicsPath();
            pathToReturn.AddBeziers(pts);

            return pathToReturn;
        }
        #endregion

        #region Line
        /// <summary>
        /// Creates GraphicsPath with line shape.
        /// </summary>
        /// <param name="points">points to create GraphicsPath from</param>
        /// <returns>Resulted GraphicsPath</returns>
        /// <remarks>
        /// Only first to points are taken into account.
        /// </remarks>
        /// <exception cref="System.ArgumentOutOfRangeException" />
        public static GraphicsPath CreateLine(Point[] points)
        {
            if ((points == null) && (points.Length < 2))
                throw new ArgumentOutOfRangeException("points");

            GraphicsPath pathToReturn = new GraphicsPath();

            pathToReturn.AddLine(points[0], points[1]);

            return pathToReturn;
        }

        /// <summary>
        /// Creates GraphicsPath with line shape.
        /// </summary>
        /// <param name="points">points to create GraphicsPath from</param>
        /// <returns>Resulted GraphicsPath</returns>
        /// <remarks>
        /// Only first to points are taken into account.
        /// </remarks>
        /// <exception cref="System.ArgumentOutOfRangeException" />
        public static GraphicsPath CreateLine(PointF[] points)
        {
            if ((points == null) && (points.Length < 2))
                throw new ArgumentOutOfRangeException("points");

            GraphicsPath pathToReturn = new GraphicsPath();

            pathToReturn.AddLine(points[0], points[1]);

            return pathToReturn;
        }

        /// <summary>
        /// Creates GraphicsPath with line shape.
        /// </summary>
        /// <param name="ptStart">Line's start point</param>
        /// <param name="ptEnd">Line's end point</param>
        /// <returns>Resulted GraphicsPath</returns>
        /// <exception cref="System.ArgumentException" />
        public static GraphicsPath CreateLine(PointF ptStart, PointF ptEnd)
        {
            if (ptStart == ptEnd)
                throw new ArgumentException("ptStart equals ptEnd");

            GraphicsPath pathToReturn = new GraphicsPath();

            pathToReturn.AddLine(ptStart, ptEnd);

            return pathToReturn;
        }

        /// <summary>
        /// Creates GraphicsPath with line shape.
        /// </summary>
        /// <param name="ptStart">Line's start point</param>
        /// <param name="ptEnd">Line's end point</param>
        /// <returns>Resulted GraphicsPath</returns>
        /// <exception cref="System.ArgumentException" />
        public static GraphicsPath CreateLine(Point ptStart, Point ptEnd)
        {
            if (ptStart == ptEnd)
                throw new ArgumentException("ptStart equals ptEnd");

            GraphicsPath pathToReturn = new GraphicsPath();

            pathToReturn.AddLine(ptStart, ptEnd);

            return pathToReturn;
        }
        #endregion

        #region Orthogonal Line
        /// <summary>
        /// Creates GraphicsPath with line shape.
        /// </summary>
        /// <param name="points">points to create GraphicsPath from</param>
        /// <returns>Resulted GraphicsPath</returns>
        /// <remarks>
        /// Only first to points are taken into account.
        /// </remarks>
        /// <exception cref="System.ArgumentOutOfRangeException" />
        public static GraphicsPath CreateOrthogonalLine(Point[] points)
        {
            if ((points == null) && (points.Length < 2))
                throw new ArgumentOutOfRangeException("points");

            GraphicsPath gpToReturn = new GraphicsPath();
            gpToReturn.AddLines(points);

            return gpToReturn;
        }

        /// <summary>
        /// Creates GraphicsPath with line shape.
        /// </summary>
        /// <param name="points">points to create GraphicsPath from</param>
        /// <returns>Resulted GraphicsPath</returns>
        /// <remarks>
        /// Only first to points are taken into account.
        /// </remarks>
        /// <exception cref="System.ArgumentOutOfRangeException" />
        public static GraphicsPath CreateOrthogonalLine(PointF[] points)
        {
            if ((points == null) && (points.Length < 2))
                throw new ArgumentOutOfRangeException("points");

            GraphicsPath gpToReturn = new GraphicsPath();
            gpToReturn.AddLines(points);

            return gpToReturn;
        }

        /// <summary>
        /// Creates GraphicsPath with line shape.
        /// </summary>
        /// <param name="ptStart">Line's start point</param>
        /// <param name="ptEnd">Line's end point</param>
        /// <returns>Resulted GraphicsPath</returns>
        /// <exception cref="System.ArgumentException" />
        public static GraphicsPath CreateOrthogonalLine(PointF ptStart, PointF ptEnd)
        {
            if (ptStart == ptEnd)
                throw new ArgumentException("ptStart equals ptEnd");

            PointF[] pts = new PointF[]
            { 
                ptStart,
                new PointF( ptStart.X, ptEnd.Y ),
                ptEnd
            };

            return CreateOrthogonalLine(pts);
        }

        /// <summary>
        /// Creates GraphicsPath with line shape.
        /// </summary>
        /// <param name="ptStart">Line's start point</param>
        /// <param name="ptEnd">Line's end point</param>
        /// <returns>Resulted GraphicsPath</returns>
        /// <exception cref="System.ArgumentException" />
        public static GraphicsPath CreateOrthogonalLine(Point ptStart, Point ptEnd)
        {
            if (ptStart == ptEnd)
                throw new ArgumentException("ptStart equals ptEnd");

            Point[] pts = new Point[]
            {
                ptStart,
                new Point( ptStart.X, ptEnd.Y ),
                ptEnd
            };

            return CreateOrthogonalLine(pts);
        }
        #endregion

        #region Arc
        /// <summary>
        /// Creates GraphicsPath with arc shape
        /// </summary>
        /// <param name="point">upper left point of arc's bounding rectangle</param>
        /// <param name="size">arc's bounding rectangle size</param>
        /// <param name="fStartAngle">starting angle of the arc</param>
        /// <param name="fSweepAngle">angle between startAngle and the end of the arc</param>
        /// <returns>Resulted GraphicsPath</returns>
        public static GraphicsPath CreateArc(PointF point, SizeF size, float fStartAngle, float fSweepAngle)
        {
            return CreateArc(new RectangleF(point, size), fStartAngle, fSweepAngle);
        }

        /// <summary>
        /// Creates GraphicsPath with arc shape
        /// </summary>
        /// <param name="fX">upper left X-coordinate of arc's bounding rectangle</param>
        /// <param name="fY">upper left Y-coordinate of arc's bounding rectangle</param>
        /// <param name="fWidth">arc's bounding rectangle width</param>
        /// <param name="fHeight">arc's bounding rectangle height</param>
        /// <param name="fStartAngle">starting angle of the arc</param>
        /// <param name="fSweepAngle">angle between startAngle and the end of the arc</param>
        /// <returns>Resulted GraphicsPath</returns>
        public static GraphicsPath CreateArc(float fX, float fY, float fWidth, float fHeight, float fStartAngle, float fSweepAngle)
        {
            return CreateArc(new RectangleF(fX, fY, fWidth, fHeight), fStartAngle, fSweepAngle);
        }

        /// <summary>
        /// Creates GraphicsPath with arc shape
        /// </summary>
        /// <param name="rectBounding">arc's bounding rectangle</param>
        /// <param name="fStartAngle">starting angle of the arc</param>
        /// <param name="fSweepAngle">angle between startAngle and the end of the arc</param>
        /// <returns>Resulted GraphicsPath</returns>
        /// <exception cref="System.ArgumentOutOfRangeException" />
        public static GraphicsPath CreateArc(RectangleF rectBounding, float fStartAngle, float fSweepAngle)
        {
            if ((rectBounding.Width < 0) || (rectBounding.Height < 0))
                throw new ArgumentOutOfRangeException("rectBounding");

            GraphicsPath pathToReturn = new GraphicsPath();

            pathToReturn.AddArc(rectBounding, fStartAngle, fSweepAngle);
            return pathToReturn;
        }
        #endregion

        #region Rectangle
        /// <summary>
        /// Creates GraphicsPath with rectangle shape
        /// </summary>
        /// <param name="rect">Rectangle to add</param>
        /// <returns>Resulted GraphicsPath</returns>
        public static GraphicsPath CreateRectangle(RectangleF rect)
        {
            if (rect.Width < 0)
            {
                rect.Width = Math.Abs(rect.Width);
                rect.X -= rect.Width;
            }

            if (rect.Height < 0)
            {
                rect.Height = Math.Abs(rect.Height);
                rect.Y -= rect.Height;
            }

            GraphicsPath pathToReturn = new GraphicsPath();

            pathToReturn.AddRectangle(rect);

            return pathToReturn;
        }

        /// <summary>
        /// Creates GraphicsPath with rectangle shape
        /// </summary>
        /// <param name="ptUpperLeft">adding rectangle upperleft point</param>
        /// <param name="ptLowerRight">adding rectangle lowerright point</param>
        /// <returns>Resulted GraphicsPath</returns>
        public static GraphicsPath CreateRectangle(PointF ptUpperLeft, PointF ptLowerRight)
        {
            RectangleF rectBounds = new RectangleF(
                ptUpperLeft,
                new SizeF(ptLowerRight.X - ptUpperLeft.X, ptLowerRight.Y - ptUpperLeft.Y));

            return CreateRectangle(rectBounds);
        }

        /// <summary>
        /// Creates GraphicsPath with rectangle shape
        /// </summary>
        /// <param name="ptLocation">GraphicsPath's location</param>
        /// <param name="szSize">GraphicsPath's size</param>
        /// <returns>Resulted GraphicsPath</returns>
        public static GraphicsPath CreateRectangle(PointF ptLocation, SizeF szSize)
        {
            RectangleF rectBounds = new RectangleF(ptLocation, szSize);

            return CreateRectangle(rectBounds);
        }

        /// <summary>
        /// Creates GraphicsPath with rectangle shape
        /// </summary>
        /// <param name="fX">GraphicsPath's X-coordinate</param>
        /// <param name="fY">GraphicsPath's Y-coordinate</param>
        /// <param name="fWidth">GraphicsPath's width</param>
        /// <param name="fHeight">GraphicsPath's height</param>
        /// <returns>Resulted GraphicsPath</returns>
        public static GraphicsPath CreateRectangle(float fX, float fY, float fWidth, float fHeight)
        {
            RectangleF rectBounds = new RectangleF(fX, fY, fWidth, fHeight);

            return CreateRectangle(rectBounds);
        }

        #endregion

        #region Round Rectangle
        /// <summary>
        /// Create GraphicsPath with round rectangle shape.
        /// </summary>
        /// <param name="ptUpperLeft">Adding rectangle upperleft point.</param>
        /// <param name="ptLowerRight">Adding rectangle lowerright point.</param>
        /// <param name="fCurveRadius">The curve radius.</param>
        /// <returns>Resulted GraphicsPath.</returns>
        public static GraphicsPath CreateRoundRectangle(PointF ptUpperLeft, PointF ptLowerRight, float fCurveRadius)
        {
            RectangleF rectBounds = new RectangleF(
                ptUpperLeft,
                new SizeF(ptLowerRight.X - ptUpperLeft.X, ptLowerRight.Y - ptUpperLeft.Y));

            return CreateRoundRectangle(rectBounds, fCurveRadius);
        }

        /// <summary>
        /// Create GraphicsPath with round rectangle shape.
        /// </summary>
        /// <param name="ptLocation">The location.</param>
        /// <param name="szSize">The size.</param>
        /// <param name="fCurveRadius">The curve radius.</param>
        /// <returns>Resulted GraphicsPath.</returns>
        public static GraphicsPath CreateRoundRectangle(PointF ptLocation, SizeF szSize, float fCurveRadius)
        {
            return CreateRoundRectangle(new RectangleF(ptLocation, szSize), fCurveRadius);
        }

        /// <summary>
        /// Create GraphicsPath with round rectangle shape.
        /// </summary>
        /// <param name="fX">GraphicsPath's X-coordinate.</param>
        /// <param name="fY">GraphicsPath's Y-coordinate.</param>
        /// <param name="fWidth">GraphicsPath's width.</param>
        /// <param name="fHeight">GraphicsPath's height.</param>
        /// <param name="fCurveRadius">The curve radius.</param>
        /// <returns>Resulted GraphicsPath.</returns>
        public static GraphicsPath CreateRoundRectangle(float fX, float fY, float fWidth, float fHeight, float fCurveRadius)
        {
            return CreateRoundRectangle(new RectangleF(fX, fY, fWidth, fHeight), fCurveRadius);
        }

        /// <summary>
        /// Create GraphicsPath with round rectangle shape.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <param name="fCurveRadius">The curve radius.</param>
        /// <returns>Resulted GraphicsPath.</returns>
        public static GraphicsPath CreateRoundRectangle(RectangleF rect, float fCurveRadius)
        {
            // Mimimal dimension
            float fMinDim = (rect.Width > rect.Height) ? rect.Height : rect.Width;
            fCurveRadius = (fCurveRadius > fMinDim / 2) ? fMinDim / 2 : fCurveRadius;

            // If width or height lower zero, get its abs
            if (rect.Width < 0)
            {
                rect.Width = Math.Abs(rect.Width);
                rect.X -= rect.Width;
            }
            if (rect.Height < 0)
            {
                rect.Height = Math.Abs(rect.Height);
                rect.Y -= rect.Height;
            }

            // Create offsets for curve
            float offsetX = fCurveRadius * 2;
            float offsetY = offsetX;

            GraphicsPath pathToReturn = new GraphicsPath();

            // Start to create close figure from four arcs.
            pathToReturn.StartFigure();

            // Calculate arc arounds.
            RectangleF rcLeftUp = new RectangleF(rect.X, rect.Y, offsetX, offsetY);
            RectangleF rcLeftDown = new RectangleF(rect.X, rect.Bottom - offsetY, offsetX, offsetY);
            RectangleF rcRigthUp = new RectangleF(rect.Right - offsetX, rect.Y, offsetX, offsetY);
            RectangleF rcRightDown = new RectangleF(rect.Right - offsetX, rect.Bottom - offsetY, offsetX, offsetY);

            // Create RoundRect's shape GraphicsPath.
            pathToReturn.AddArc(rcLeftUp, 180, 90);
            pathToReturn.AddArc(rcRigthUp, 270, 90);
            pathToReturn.AddArc(rcRightDown, 0, 90);
            pathToReturn.AddArc(rcLeftDown, 90, 90);

            // Close figure generate
            pathToReturn.CloseFigure();

            return pathToReturn;
        }
        #endregion

        #region Ellipse
        /// <summary>
        /// Creates GraphicsPath with ellipse shape
        /// </summary>
        /// <param name="rectBounding">Ellipse bounding rectangle</param>
        /// <returns>Resulted GraphicsPath</returns>
        public static GraphicsPath CreateEllipse(RectangleF rectBounding)
        {
            if (rectBounding.Width < 0)
            {
                rectBounding.Width = Math.Abs(rectBounding.Width);
                rectBounding.X -= rectBounding.Width;
            }

            if (rectBounding.Height < 0)
            {
                rectBounding.Height = Math.Abs(rectBounding.Height);
                rectBounding.Y -= rectBounding.Height;
            }

            GraphicsPath pathToReturn = new GraphicsPath();

            pathToReturn.AddEllipse(rectBounding);

            return pathToReturn;
        }

        /// <summary>
        /// Creates GraphicsPath with ellipse shape
        /// </summary>
        /// <param name="ptUpperLeft">adding rectangle upperleft point</param>
        /// <param name="ptLowerRight">adding rectangle lowerright point</param>
        /// <returns>Resulted GraphicsPath</returns>
        public static GraphicsPath CreateEllipse(PointF ptUpperLeft, PointF ptLowerRight)
        {
            RectangleF rectBounds = new RectangleF(
                ptUpperLeft, 
                new SizeF(ptLowerRight.X - ptUpperLeft.X, ptLowerRight.Y - ptUpperLeft.Y));

            return CreateEllipse(rectBounds);
        }

        /// <summary>
        /// Creates GraphicsPath with ellipse shape
        /// </summary>
        /// <param name="ptLocation">GraphicsPath's location</param>
        /// <param name="szSize">GraphicsPath's size</param>
        /// <returns>Resulted Graphicspath</returns>
        public static GraphicsPath CreateEllipse(PointF ptLocation, SizeF szSize)
        {
            RectangleF rectBounds = new RectangleF(ptLocation, szSize);

            return CreateEllipse(rectBounds);
        }

        /// <summary>
        /// Creates GraphicsPath with ellipse shape
        /// </summary>
        /// <param name="fX">GraphicsPath's X-coordinate</param>
        /// <param name="fY">GraphicsPath's Y-coordinate</param>
        /// <param name="fWidth">GraphicsPath's width</param>
        /// <param name="fHeight">GraphicsPath's height</param>
        /// <returns>Resulted GraphicsPath</returns>
        public static GraphicsPath CreateEllipse(float fX, float fY, float fWidth, float fHeight)
        {
            RectangleF rectBounds = new RectangleF(fX, fY, fWidth, fHeight);

            return CreateEllipse(rectBounds);
        }

        #endregion
    }

    /// <summary>
    /// Decorator shapes.
    /// </summary>
    public enum DecoratorShape
    {
        /// <summary>
        /// No shape.
        /// </summary>
        None = 0,

        /// <summary>
        /// Custom shape.
        /// </summary>
        Custom,

        /// <summary>
        /// 45 degree open end arrow.
        /// </summary>
        Open45Arrow,

        /// <summary>
        /// 45 degree closed end arrow.
        /// </summary>
        Filled45Arrow,

        /// <summary>
        /// Diagram shape.
        /// </summary>
        Diamond,

        /// <summary>
        /// Filled diamond shape.
        /// </summary>
        FilledDiamond,

        /// <summary>
        /// Circle shape.
        /// </summary>
        Circle,

        /// <summary>
        /// Filled circle shape.
        /// </summary>
        FilledCircle,

        /// <summary>
        /// 60 degree open end arrow.
        /// </summary>
        Open60Arrow,

        /// <summary>
        /// 60 degree closed end arrow.
        /// </summary>
        Filled60Arrow,

        /// <summary>
        /// Open fancy arrow shape.
        /// </summary>
        OpenFancyArrow,

        /// <summary>
        /// Filled fancy arrow shape.
        /// </summary>
        FilledFancyArrow,

        /// <summary>
        /// Square shape.
        /// </summary>
        Square,

        /// <summary>
        /// Filled square shape.
        /// </summary>
        FilledSquare,

        /// <summary>
        /// Dimension line shape.
        /// </summary>
        DimensionLine,

        /// <summary>
        /// 45 degrees cross shape.
        /// </summary>
        Cross45,

        /// <summary>
        /// 90 degree cross shape.
        /// </summary>
        Cross90,

        /// <summary>
        /// Double cross shape.
        /// </summary>
        DoubleCross,

        /// <summary>
        /// Reverse arrow shape.
        /// </summary>
        ReverseArrow,

        /// <summary>
        /// Cross reverse arrow.
        /// </summary>
        CrossReverseArrow,

        /// <summary>
        /// Circle cross shape.
        /// </summary>
        CircleCross,

        /// <summary>
        /// Circle reverse arrow shape.
        /// </summary>
        CircleReverseArrow,

        /// <summary>
        /// Double arrow shape.
        /// </summary>
        DoubleArrow,

        /// <summary>
        /// Reverse double arrow shape.
        /// </summary>
        ReverseDoubleArrow
    }

    /// <summary>
    /// Decorator factory.
    /// </summary>
    public class DecoratorFactory
    {
        #region Class static members
        /// <summary>
        /// Default size of decorator
        /// </summary>
        private static float m_size = CommonUsedValues.DEF_MIN_DECORATOR_SIZE;

        /// <summary>
        /// Half size of default decorator size
        /// </summary>
        private static float m_halfSize = CommonUsedValues.DEF_MIN_DECORATOR_SIZE / 2;

        /// <summary>
        /// Double size of default decorator size
        /// </summary>
        private static float m_doubleSize = CommonUsedValues.DEF_MIN_DECORATOR_SIZE * 2;
        #endregion

        #region Class static properties
        /// <summary>
        /// Gets the GraphicsPath of open45 arrow.
        /// </summary>
        /// <value>The GraphicsPath of open45 arrow.</value>
        public static GraphicsPath Open45Arrow
        {
            get
            {
                GraphicsPath gp = new GraphicsPath();

                PointF[] pts = new PointF[]
                {
                    new PointF( 0, m_size ),
                    new PointF( m_size, m_halfSize ),
                    new PointF( 0, 0 ),
                    new PointF( m_size, m_halfSize ),
                    new PointF( 0, m_halfSize )
                };

                gp.AddLines(pts);

                return gp;
            }
        }

        /// <summary>
        /// Gets the GraphicsPath of filled45 arrow.
        /// </summary>
        /// <value>The GraphicsPath of filled45 arrow.</value>
        public static GraphicsPath Filled45Arrow
        {
            get
            {
                GraphicsPath gp = new GraphicsPath();

                PointF[] pts = new PointF[]
                {
                    new PointF( 0, m_size ),
                    new PointF( m_size, m_halfSize ),
                    new PointF( 0, 0 ),
                };

                gp.AddLines(pts);
                gp.CloseFigure();

                return gp;
            }
        }

        /// <summary>
        /// Gets the GraphicsPath of diamond.
        /// </summary>
        /// <value>The GraphicsPath of diamond.</value>
        public static GraphicsPath Diamond
        {
            get
            {
                GraphicsPath gp = new GraphicsPath();

                PointF[] pts = new PointF[]
                {
                    new PointF( 0, m_halfSize ),
                    new PointF( m_halfSize, m_size ),
                    new PointF( m_size, m_halfSize ),
                    new PointF( m_halfSize, 0 ),
                    new PointF( 0, m_halfSize )
                };

                gp.AddLines(pts);

                return gp;
            }
        }

        /// <summary>
        /// Gets the GraphicsPath of filled diamond.
        /// </summary>
        /// <value>The GraphicsPath of filled diamond.</value>
        public static GraphicsPath FilledDiamond
        {
            get
            {
                GraphicsPath gp = new GraphicsPath();

                PointF[] pts = new PointF[]
                {
                    new PointF( 0, m_halfSize ),
                    new PointF( m_halfSize, m_size ),
                    new PointF( m_size, m_halfSize ),
                    new PointF( m_halfSize, 0 ),
                    new PointF( 0, m_halfSize )
                };

                gp.AddPolygon(pts);

                return gp;
            }
        }

        /// <summary>
        /// Gets the GraphicsPath of circle.
        /// </summary>
        /// <value>The GraphicsPath of circle.</value>
        public static GraphicsPath Circle
        {
            get
            {
                GraphicsPath gp = new GraphicsPath();

                // Draw circle range 0 to 359, becouse shape
                // must be open.
                gp.AddArc(0, 0, m_size, m_size, 0, 359);

                return gp;
            }
        }

        /// <summary>
        /// Gets the GraphicsPath of filled circle.
        /// </summary>
        /// <value>The GraphicsPath of filled circle.</value>
        public static GraphicsPath FilledCircle
        {
            get
            {
                GraphicsPath gp = new GraphicsPath();
                gp.AddEllipse(0, 0, m_size, m_size);

                return gp;
            }
        }

        /// <summary>
        /// Gets the GraphicsPath of open60 arrow.
        /// </summary>
        /// <value>The GraphicsPath of open60 arrow.</value>
        public static GraphicsPath Open60Arrow
        {
            get
            {
                GraphicsPath gpPath = new GraphicsPath();

                PointF[] pts = new PointF[]
                {
                    new PointF( 0, 0 ),
                    new PointF( m_halfSize, m_halfSize ),
                    new PointF( 0, m_size ),
                    new PointF( m_halfSize, m_halfSize ),
                    new PointF( 0, m_halfSize )
                };

                gpPath.AddLines(pts);

                return gpPath;
            }
        }

        /// <summary>
        /// Gets the GraphicsPath of filled60 arrow.
        /// </summary>
        /// <value>The GraphicsPath of filled60 arrow.</value>
        public static GraphicsPath Filled60Arrow
        {
            get
            {
                GraphicsPath gpPath = new GraphicsPath();

                PointF[] pts = new PointF[]
                {
                    new PointF( 0, 0 ),
                    new PointF( m_halfSize, m_halfSize ),
                    new PointF( 0, m_size ),
                    new PointF( 0, 0 )
                };

                gpPath.AddPolygon(pts);

                return gpPath;
            }
        }

        /// <summary>
        /// Gets the GraphicsPath of fancy arrow.
        /// </summary>
        /// <value>The GraphicsPath of open fancy arrow.</value>
        public static GraphicsPath OpenFancyArrow
        {
            get
            {
                GraphicsPath gpPath = new GraphicsPath();

                PointF[] pts = new PointF[]
                {
                    new PointF( 0, 0 ),
                    new PointF( m_doubleSize, m_halfSize ),
                    new PointF( 0, m_size ),
                    new PointF( m_doubleSize, m_halfSize ),
                    new PointF( 0, m_halfSize )
                };

                gpPath.AddLines(pts);

                return gpPath;
            }
        }

        /// <summary>
        /// Gets the GraphicsPath of filled fancy arrow.
        /// </summary>
        /// <value>The GraphicsPath of filled fancy arrow.</value>
        public static GraphicsPath FilledFancyArrow
        {
            get
            {
                GraphicsPath gpPath = new GraphicsPath();

                PointF[] pts = new PointF[]
                {
                    new PointF( 0, 0 ),
                    new PointF( m_doubleSize, m_halfSize ),
                    new PointF( 0, m_size ),
                    new PointF( m_halfSize, m_halfSize ),
                    new PointF( 0, m_halfSize ),
                    new PointF( m_halfSize, m_halfSize ),
                    new PointF( 0, 0 )
                };

                gpPath.AddPolygon(pts);

                return gpPath;
            }
        }

        /// <summary>
        /// Gets the GraphicsPath of square.
        /// </summary>
        /// <value>The GraphicsPath of square.</value>
        public static GraphicsPath Square
        {
            get
            {
                GraphicsPath gpPath = new GraphicsPath();

                PointF[] pts = new PointF[]
                {
                    new PointF( 0, 0 ),
                    new PointF( m_size, 0 ),
                    new PointF( m_size, m_size ),
                    new PointF( 0, m_size ),
                    new PointF( 0, 0 )
                };

                gpPath.AddLines(pts);

                return gpPath;
            }
        }

        /// <summary>
        /// Gets the GraphicsPath of filled square.
        /// </summary>
        /// <value>The GraphicsPath of filled square.</value>
        public static GraphicsPath FilledSquare
        {
            get
            {
                GraphicsPath gpPath = new GraphicsPath();

                PointF[] pts = new PointF[]
                {
                    new PointF( 0, 0 ),
                    new PointF( m_size, 0 ),
                    new PointF( m_size, m_size ),
                    new PointF( 0, m_size ),
                    new PointF( 0, 0 )
                };

                gpPath.AddPolygon(pts);

                return gpPath;
            }
        }

        /// <summary>
        /// Gets the GraphicsPath of dimension line.
        /// </summary>
        /// <value>The GraphicsPath of dimension line.</value>
        public static GraphicsPath DimensionLine
        {
            get
            {
                GraphicsPath gpPath = new GraphicsPath();

                gpPath.AddLine(1, 0, 1, m_size);

                return gpPath;
            }
        }

        /// <summary>
        /// Gets the GraphicsPath of cross45.
        /// </summary>
        /// <value>The GraphicsPath of cross45.</value>
        public static GraphicsPath Cross45
        {
            get
            {
                GraphicsPath gpPath = new GraphicsPath();

                PointF[] pts = new PointF[]
                {
                    new PointF( 0, 0 ),
                    new PointF( m_size, m_size ),
                    new PointF( m_halfSize, m_halfSize ),
                    new PointF( 0, m_halfSize )
                };

                gpPath.AddLines(pts);

                return gpPath;
            }
        }

        /// <summary>
        /// Gets the GraphicsPath of cross90.
        /// </summary>
        /// <value>The GraphicsPath of cross90.</value>
        public static GraphicsPath Cross90
        {
            get
            {
                GraphicsPath gpPath = new GraphicsPath();

                gpPath.AddLine(0, 0, 0, m_size);
                gpPath.AddLine(0, m_halfSize, m_halfSize, m_halfSize);

                return gpPath;
            }
        }

        /// <summary>
        /// Gets the GraphicsPath of double cross.
        /// </summary>
        /// <value>The GraphicsPath of double cross.</value>
        public static GraphicsPath DoubleCross
        {
            get
            {
                GraphicsPath gpPath = new GraphicsPath();

                gpPath.AddLine(0, 0, 0, m_size);
                gpPath.CloseFigure();

                gpPath.AddLine(m_halfSize, 0, m_halfSize, m_size);
                gpPath.CloseFigure();

                gpPath.AddLine(0, m_halfSize, m_size, m_halfSize);
                return gpPath;
            }
        }

        /// <summary>
        /// Gets the GraphicsPath of reverse arrow.
        /// </summary>
        /// <value>The GraphicsPath of reverse arrow.</value>
        public static GraphicsPath ReverseArrow
        {
            get
            {
                GraphicsPath gpPath = new GraphicsPath();

                PointF[] pts = new PointF[]
                {
                    new PointF( m_size, 0 ),
                    new PointF( 0, m_halfSize ),
                    new PointF( m_size, m_size )
                };

                gpPath.AddLine(0, m_halfSize, m_size, m_halfSize);
                gpPath.CloseFigure();

                gpPath.AddLines(pts);
                return gpPath;
            }
        }

        /// <summary>
        /// Gets the GraphicsPath of cross reverse arrow.
        /// </summary>
        /// <value>The GraphicsPath of cross reverse arrow.</value>
        public static GraphicsPath CrossReverseArrow
        {
            get
            {
                GraphicsPath gpPath = new GraphicsPath();

                PointF[] pts = new PointF[]
                {
                    new PointF( m_size, 0 ),
                    new PointF( m_halfSize, m_halfSize ),
                    new PointF( m_size, m_size )
                };

                gpPath.AddLine(0, 0, 0, m_size);
                gpPath.CloseFigure();

                gpPath.AddLine(0, m_halfSize, m_size, m_halfSize);
                gpPath.CloseFigure();

                gpPath.AddLines(pts);

                return gpPath;
            }
        }

        /// <summary>
        /// Gets the GraphicsPath of circle cross.
        /// </summary>
        /// <value>The GraphicsPath of circle cross.</value>
        public static GraphicsPath CircleCross
        {
            get
            {
                GraphicsPath gpPath = new GraphicsPath();

                gpPath.AddArc(0, 0, m_size, m_size, 0, 359);
                gpPath.CloseFigure();

                gpPath.AddLine(m_size + m_halfSize, 0, m_size + m_halfSize, m_size);
                gpPath.CloseFigure();

                gpPath.AddLine(m_size, m_halfSize, m_doubleSize, m_halfSize);

                return gpPath;
            }
        }

        /// <summary>
        /// Gets the GraphicsPath of circle reverse arrow.
        /// </summary>
        /// <value>The GraphicsPath of circle reverse arrow.</value>
        public static GraphicsPath CircleReverseArrow
        {
            get
            {
                GraphicsPath gpPath = new GraphicsPath();

                PointF[] pts = new PointF[]
                {
                    new PointF( m_doubleSize, 0 ),
                    new PointF( m_size, m_halfSize ),
                    new PointF( m_doubleSize, m_size )
                };

                gpPath.AddArc(0, 0, m_size, m_size, 0, 359);
                gpPath.CloseFigure();
                gpPath.AddLine(m_size, m_halfSize, m_doubleSize, m_halfSize);
                gpPath.CloseFigure();

                gpPath.AddLines(pts);

                return gpPath;
            }
        }

        /// <summary>
        /// Gets the GraphicsPath of reverse double arrow.
        /// </summary>
        /// <value>The GraphicsPath of reverse double arrow.</value>
        public static GraphicsPath DoubleArrow
        {
            get
            {
                GraphicsPath gpPath = new GraphicsPath();

                PointF[] ptsSecondArrow = new PointF[]
                {
                    new PointF( m_halfSize, 0 ),
                    new PointF( m_size, m_halfSize ),
                    new PointF( m_halfSize, m_size )
                };

                gpPath.AddLine(0, m_halfSize, m_size, m_halfSize);
                gpPath.CloseFigure();

                gpPath.AddLine(0, 0, m_halfSize, m_halfSize);
                gpPath.CloseFigure();

                gpPath.AddLine(m_halfSize, m_halfSize, 0, m_size);
                gpPath.CloseFigure();

                gpPath.AddLines(ptsSecondArrow);

                return gpPath;
            }
        }

        /// <summary>
        /// Gets the GraphicsPath of reverse double arrow.
        /// </summary>
        /// <value>The GraphicsPath of reverse double arrow.</value>
        public static GraphicsPath ReverseDoubleArrow
        {
            get
            {
                GraphicsPath gpPath = new GraphicsPath();

                PointF[] ptsSecondArrow = new PointF[]
                {
                    new PointF( m_size, 0 ),
                    new PointF( m_halfSize, m_halfSize ),
                    new PointF( m_size, m_size )
                };

                gpPath.AddLine(0, m_halfSize, m_size, m_halfSize);
                gpPath.CloseFigure();

                gpPath.AddLine(0, m_halfSize, m_halfSize, 0);
                gpPath.CloseFigure();

                gpPath.AddLine(0, m_halfSize, m_halfSize, m_size);
                gpPath.CloseFigure();

                gpPath.AddLines(ptsSecondArrow);

                return gpPath;
            }
        }
        #endregion

        #region Class Public Methods
        /// <summary>
        /// Creates the decorator from specified shape.
        /// </summary>
        /// <param name="shape">The shape.</param>
        /// <returns>The graphics path.</returns>
        public static GraphicsPath CreateDecorator(DecoratorShape shape)
        {
            GraphicsPath pathToReturn;

            switch (shape)
            {
                case DecoratorShape.Diamond:
                    pathToReturn = Diamond;
                    break;
                case DecoratorShape.FilledDiamond:
                    pathToReturn = FilledDiamond;
                    break;
                case DecoratorShape.Circle:
                    pathToReturn = Circle;
                    break;
                case DecoratorShape.FilledCircle:
                    pathToReturn = FilledCircle;
                    break;
                case DecoratorShape.Open60Arrow:
                    pathToReturn = Open60Arrow;
                    break;
                case DecoratorShape.Filled60Arrow:
                    pathToReturn = Filled60Arrow;
                    break;
                case DecoratorShape.Open45Arrow:
                    pathToReturn = Open45Arrow;
                    break;
                case DecoratorShape.Filled45Arrow:
                    pathToReturn = Filled45Arrow;
                    break;
                case DecoratorShape.OpenFancyArrow:
                    pathToReturn = OpenFancyArrow;
                    break;
                case DecoratorShape.FilledFancyArrow:
                    pathToReturn = FilledFancyArrow;
                    break;
                case DecoratorShape.Square:
                    pathToReturn = Square;
                    break;
                case DecoratorShape.FilledSquare:
                    pathToReturn = FilledSquare;
                    break;
                case DecoratorShape.DimensionLine:
                    pathToReturn = DimensionLine;
                    break;
                case DecoratorShape.Cross45:
                    pathToReturn = Cross45;
                    break;
                case DecoratorShape.Cross90:
                    pathToReturn = Cross90;
                    break;
                case DecoratorShape.DoubleCross:
                    pathToReturn = DoubleCross;
                    break;
                case DecoratorShape.ReverseArrow:
                    pathToReturn = ReverseArrow;
                    break;
                case DecoratorShape.CrossReverseArrow:
                    pathToReturn = CrossReverseArrow;
                    break;
                case DecoratorShape.CircleCross:
                    pathToReturn = CircleCross;
                    break;
                case DecoratorShape.CircleReverseArrow:
                    pathToReturn = CircleReverseArrow;
                    break;
                case DecoratorShape.DoubleArrow:
                    pathToReturn = DoubleArrow;
                    break;
                case DecoratorShape.ReverseDoubleArrow:
                    pathToReturn = ReverseDoubleArrow;
                    break;
                default:
                    pathToReturn = new GraphicsPath();
                    break;
            }

            return pathToReturn;
        }
        #endregion
    }
}
