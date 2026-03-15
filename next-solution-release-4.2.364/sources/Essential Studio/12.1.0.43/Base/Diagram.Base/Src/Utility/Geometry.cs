#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Contains static declarations for functions and constants used for
    /// performing calculations on points, lines, and rectangles.
    /// </summary>
    public class Geometry
    {
        /// <summary>
        /// Angle of quadrant 1 for a unit circle in radians.
        /// </summary>
        public static double RadiansQuadrant1 = Math.PI / 2.0;

        /// <summary>
        /// Angle of quadrant 2 for a unit circle in radians.
        /// </summary>
        public static double RadiansQuadrant2 = Math.PI;

        /// <summary>
        /// Angle of quadrant 3 for a unit circle in radians.
        /// </summary>
        public static double RadiansQuadrant3 = (3.0 * Math.PI) / 2.0;

        /// <summary>
        /// Rotate given point around anchor point.
        /// </summary>
        /// <param name="angle">The angle to rotate.</param>
        /// <param name="ptAnchor">The anchor point.</param>
        /// <param name="ptPoint">The point to rotate.</param>
        /// <remarks>
        /// This rotate method used only double numbers to get a tru-running.
        /// </remarks>
        public static void RotatePointAt(float angle, PointF ptAnchor, ref PointF ptPoint)
        {
            double dAnchorX = ptAnchor.X;
            double dAnchorY = ptAnchor.Y;
            double dPointX = ptPoint.X;
            double dPointY = ptPoint.Y;

            // degrees to radians
            angle *= (float)(Math.PI / 180.0);
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            double dAnchorOffsetX = -dAnchorX * cos + dAnchorY * sin + dAnchorX;
            double dAnchorOffsetY = -dAnchorX * sin - dAnchorY * cos + dAnchorY;

            // transform points
            ptPoint.X = (float)(cos * dPointX - sin * dPointY + dAnchorOffsetX);
            ptPoint.Y = (float)(sin * dPointX + cos * dPointY + dAnchorOffsetY);
        }

        /// <summary>
        /// Angle of quadrant 4 for a unit circle in radians.
        /// </summary>
        public static double RadiansQuadrant4 = 2.0 * Math.PI;

        /// <summary>
        /// Converts from angle range [-180 ; 180 ] , to angle range [0; 360].
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <returns>The angle value.</returns>
        public static float ConvertToFullCircle(float angle)
        {
            return (angle < 0) ? CommonUsedValues.CIRCLE + angle : angle;
        }

        /// <summary>
        /// Converts from angle range [0 ; 360 ] , to angle range [-180; 180].
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <returns>The angle value.</returns>
        public static float ConvertToPartCircle(float angle)
        {
            bool bNegative = angle < 0;
            float fAngleToReturn = angle;

            if (Math.Abs(angle) > CommonUsedValues.CIRCLE / 2)
            {
                fAngleToReturn = angle + (bNegative ? CommonUsedValues.CIRCLE : -CommonUsedValues.CIRCLE);
            }

            return fAngleToReturn;
        }

        /// <summary>
        /// Creates the rhomb shape.
        /// </summary>
        /// <param name="ptCenter">The center point.</param>
        /// <param name="szBoundingRect">The bounding rectangle.</param>
        /// <returns>The points</returns>
        public static PointF[] CreateRhomb(PointF ptCenter, SizeF szBoundingRect)
        {
            PointF[] ptsToReturn = new PointF[4];

            ptsToReturn[0] = new PointF(ptCenter.X, ptCenter.Y - szBoundingRect.Height / 2); // North
            ptsToReturn[1] = new PointF(ptCenter.X + szBoundingRect.Width / 2, ptCenter.Y); // West
            ptsToReturn[2] = new PointF(ptCenter.X, ptCenter.Y + szBoundingRect.Height / 2); // South
            ptsToReturn[3] = new PointF(ptCenter.X - szBoundingRect.Width / 2, ptCenter.Y); // East

            return ptsToReturn;
        }

        /// <summary>
        /// Create a rectangle from two points.
        /// </summary>
        /// <param name="pt1">First point.</param>
        /// <param name="pt2">Second point.</param>
        /// <returns>The Rectangle</returns>
        public static System.Drawing.Rectangle CreateRect(Point pt1, Point pt2)
        {
            int swapVal;
            System.Drawing.Point ptUpperLeft = pt1;
            System.Drawing.Point ptLowerRight = pt2;
            if (ptUpperLeft.X > ptLowerRight.X)
            {
                swapVal = ptUpperLeft.X;
                ptUpperLeft.X = ptLowerRight.X;
                ptLowerRight.X = swapVal;
            }
            if (ptUpperLeft.Y > ptLowerRight.Y)
            {
                swapVal = ptUpperLeft.Y;
                ptUpperLeft.Y = ptLowerRight.Y;
                ptLowerRight.Y = swapVal;
            }
            return new System.Drawing.Rectangle(ptUpperLeft.X, ptUpperLeft.Y, ptLowerRight.X - ptUpperLeft.X, ptLowerRight.Y - ptUpperLeft.Y);
        }

        /// <summary>
        /// Create right rectangle with converting negative size to offset location.
        /// </summary>
        /// <param name="ptCenter">Rectangle's center point.</param>
        /// <param name="szRect">Size for current rectangle. Can be negative. Later it was converted.</param>
        /// <returns>Right rectangle without negative size.</returns>
        public static RectangleF CreateRect(PointF ptCenter, SizeF szRect)
        {
            float fHalfWidth = szRect.Width / 2;
            float fHalfHeight = szRect.Height / 2;

            return new RectangleF(ptCenter.X - fHalfWidth, ptCenter.Y - fHalfHeight, szRect.Width, szRect.Height);
        }

        /// <summary>
        /// Creates the <see cref="System.Drawing.Rectangle"/>.
        /// </summary>
        /// <param name="pt1">The start point.</param>
        /// <param name="pt2">The second point.</param>
        /// <returns>The rect</returns>
        public static System.Drawing.RectangleF CreateRect(PointF pt1, PointF pt2)
        {
            float swapVal;
            PointF ptUpperLeft = pt1;
            PointF ptLowerRight = pt2;
            if (ptUpperLeft.X > ptLowerRight.X)
            {
                swapVal = ptUpperLeft.X;
                ptUpperLeft.X = ptLowerRight.X;
                ptLowerRight.X = swapVal;
            }
            if (ptUpperLeft.Y > ptLowerRight.Y)
            {
                swapVal = ptUpperLeft.Y;
                ptUpperLeft.Y = ptLowerRight.Y;
                ptLowerRight.Y = swapVal;
            }
            return new System.Drawing.RectangleF(ptUpperLeft.X, ptUpperLeft.Y, ptLowerRight.X - ptUpperLeft.X, ptLowerRight.Y - ptUpperLeft.Y);
        }

        /// <summary>
        /// Calculate centre point in rectangle.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <returns>The center point.</returns>
        public static System.Drawing.PointF CenterPoint(RectangleF rect)
        {
            float left = rect.Left;
            float right = rect.Right;
            float top = rect.Top;
            float bottom = rect.Bottom;

            float x;
            float y;

            x = (left + right) / 2.0f;
            y = (top + bottom) / 2.0f;

            return new PointF(x, y);
        }

        /// <summary>
        /// Calculate center point in rectangle.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <returns>The center point</returns>
        public static System.Drawing.Point CenterPoint(System.Drawing.Rectangle rect)
        {
            int left = rect.Left;
            int right = rect.Right;
            int top = rect.Top;
            int bottom = rect.Bottom;

            int x;
            int y;

            x = (left + right) / 2;
            y = (top + bottom) / 2;

            return new Point(x, y);
        }

        /// <summary>
        /// Calculate centre point between start and end points.
        /// </summary>
        /// <param name="ptStart">The start point.</param>
        /// <param name="ptEnd">The end point.</param>
        /// <returns>Centre point between start and end point.</returns>
        public static System.Drawing.Point CenterPoint(Point ptStart, Point ptEnd)
        {
            return ConvertPoint(CenterPoint(ptStart, ptEnd));
        }

        /// <summary>
        /// Calculate centre point between start and end points.
        /// </summary>
        /// <param name="ptStart">The start point.</param>
        /// <param name="ptEnd">The end point.</param>
        /// <returns>Centre point between start and end point.</returns>
        public static System.Drawing.PointF CenterPoint(PointF ptStart, PointF ptEnd)
        {
            float newX = ptStart.X + (ptEnd.X - ptStart.X) / 2;
            float newY = ptStart.Y + (ptEnd.Y - ptStart.Y) / 2;

            return new PointF(newX, newY);
        }

        /// <summary>
        /// Gets the anchor point.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <param name="anchor">The anchor.</param>
        /// <returns>The anchor point.</returns>
        public static System.Drawing.PointF GetAnchorPoint(RectangleF rect, BoxPosition anchor)
        {
            float left = rect.Left;
            float right = rect.Right;
            float top = rect.Top;
            float bottom = rect.Bottom;

            float x = 0.0f;
            float y = 0.0f;

            switch (anchor)
            {
                case BoxPosition.TopLeft:
                    x = left;
                    y = top;
                    break;

                case BoxPosition.TopCenter:
                    x = (left + right) / 2.0f;
                    y = top;
                    break;

                case BoxPosition.TopRight:
                    x = right;
                    y = top;
                    break;

                case BoxPosition.MiddleLeft:
                    x = left;
                    y = (top + bottom) / 2.0f;
                    break;

                case BoxPosition.Center:
                    x = (left + right) / 2.0f;
                    y = (top + bottom) / 2.0f;
                    break;

                case BoxPosition.MiddleRight:
                    x = right;
                    y = (top + bottom) / 2.0f;
                    break;

                case BoxPosition.BottomLeft:
                    x = left;
                    y = bottom;
                    break;

                case BoxPosition.BottomCenter:
                    x = (left + right) / 2.0f;
                    y = bottom;
                    break;

                case BoxPosition.BottomRight:
                    x = right;
                    y = bottom;
                    break;
            }

            return new PointF(x, y);
        }

        /// <summary>
        /// Gets the box anchors.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="right">The right.</param>
        /// <param name="bottom">The bottom.</param>
        /// <returns>The value.</returns>
        internal static float[] GetBoxAnchors(float left, float top, float right, float bottom)
        {
            float[] boxAnchors = new float[6];
            boxAnchors[0] = left;
            boxAnchors[1] = top;
            boxAnchors[2] = right;
            boxAnchors[3] = bottom;
            boxAnchors[4] = (top + bottom) / 2.0f;
            boxAnchors[5] = (left + right) / 2.0f;
            return boxAnchors;
        }

        /// <summary>
        /// Gets the anchor point.
        /// </summary>
        /// <param name="boxAnchors">The box anchors.</param>
        /// <param name="anchor">The anchor.</param>
        /// <returns>The anchor point.</returns>
        internal static System.Drawing.PointF GetAnchorPoint(float[] boxAnchors, BoxPosition anchor)
        {
            float left = boxAnchors[0];
            float top = boxAnchors[1];
            float right = boxAnchors[2];
            float bottom = boxAnchors[3];
            float middle = boxAnchors[4];
            float center = boxAnchors[5];

            float x = 0.0f;
            float y = 0.0f;

            switch (anchor)
            {
                case BoxPosition.TopLeft:
                    x = left;
                    y = top;
                    break;

                case BoxPosition.TopCenter:
                    x = center;
                    y = top;
                    break;

                case BoxPosition.TopRight:
                    x = right;
                    y = top;
                    break;

                case BoxPosition.MiddleLeft:
                    x = left;
                    y = middle;
                    break;

                case BoxPosition.Center:
                    x = center;
                    y = middle;
                    break;

                case BoxPosition.MiddleRight:
                    x = right;
                    y = middle;
                    break;

                case BoxPosition.BottomLeft:
                    x = left;
                    y = bottom;
                    break;

                case BoxPosition.BottomCenter:
                    x = center;
                    y = bottom;
                    break;

                case BoxPosition.BottomRight:
                    x = right;
                    y = bottom;
                    break;
            }

            return new PointF(x, y);
        }

        /// <summary>
        /// Creates the <see cref="System.Drawing.RectangleF"/> from points array.
        /// </summary>
        /// <param name="pts">The points.</param>
        /// <returns>The rect.</returns>
        public static System.Drawing.RectangleF CreateRect(PointF[] pts)
        {
            float left = float.MaxValue;
            float top = float.MaxValue;
            float right = float.MinValue;
            float bottom = float.MinValue;

            int numPts = pts.Length;
            float curX, curY;
            for (int ptIdx = 0; ptIdx < numPts; ptIdx++)
            {
                curX = pts[ptIdx].X;
                curY = pts[ptIdx].Y;
                if (curX < left)
                {
                    left = curX;
                }
                if (curX > right)
                {
                    right = curX;
                }
                if (curY < top)
                {
                    top = curY;
                }
                if (curY > bottom)
                {
                    bottom = curY;
                }
            }

            return new System.Drawing.RectangleF(left, top, right - left, bottom - top);
        }

        /// <summary>
        /// Creates the <see cref="System.Drawing.RectangleF"/> from points array.
        /// </summary>
        /// <param name="pts">The point array.</param>
        /// <returns>The rect.</returns>
        public static System.Drawing.Rectangle CreateRect(Point[] pts)
        {
            int left = int.MaxValue;
            int top = int.MaxValue;
            int right = int.MinValue;
            int bottom = int.MinValue;

            int numPts = pts.Length;
            int curX, curY;
            for (int ptIdx = 0; ptIdx < numPts; ptIdx++)
            {
                curX = pts[ptIdx].X;
                curY = pts[ptIdx].Y;
                if (curX < left)
                {
                    left = curX;
                }
                if (curX > right)
                {
                    right = curX;
                }
                if (curY < top)
                {
                    top = curY;
                }
                if (curY > bottom)
                {
                    bottom = curY;
                }
            }

            return new System.Drawing.Rectangle(left, top, right - left, bottom - top);
        }

        /// <summary>
        /// Creates an arc given two points.
        /// </summary>
        /// <param name="pt1">First point.</param>
        /// <param name="pt2">Second point.</param>
        /// <param name="rcBounds">Output parameter to receive the bounds of the arc created.</param>
        /// <param name="startAngle">Output parameter to receive the start angle of the arc created.</param>
        /// <param name="sweepAngle">Output parameter to receive the end angle of the arc created.</param>
        /// <remarks>
        /// <para>
        /// The two points passed in are used to calculate the bounds of the arc and the
        /// start and end angle.
        /// </para>
        /// </remarks>
        public static void ArcFromPoints(
            System.Drawing.Point pt1,
            System.Drawing.Point pt2,
            out System.Drawing.Rectangle rcBounds,
            out float startAngle,
            out float sweepAngle)
        {
            rcBounds = new System.Drawing.Rectangle(0, 0, 0, 0);
            int diffX = pt2.X - pt1.X;
            int diffY = pt2.Y - pt1.Y;
            int width = diffX * 2;
            int height = diffY * 2;
            if (width > 0 && height > 0)
            {
                rcBounds.Location = new System.Drawing.Point(pt1.X - diffX, pt1.Y);
                rcBounds.Size = new System.Drawing.Size(width, height);
                startAngle = 0;
                sweepAngle = -90;
            }
            else if (width > 0 && height <= 0)
            {
                rcBounds.Location = new System.Drawing.Point(pt1.X, pt2.Y);
                rcBounds.Size = new System.Drawing.Size(width, -height);
                startAngle = 180;
                sweepAngle = 90;
            }
            else if (width <= 0 && height > 0)
            {
                rcBounds.Location = new System.Drawing.Point(pt2.X + diffX, pt1.Y - diffY);
                rcBounds.Size = new System.Drawing.Size(-width, height);
                startAngle = 0;
                sweepAngle = 90;
            }
            else
            {
                rcBounds.Location = new System.Drawing.Point(pt2.X, pt2.Y + diffY);
                rcBounds.Size = new System.Drawing.Size(-width, -height);
                startAngle = 180;
                sweepAngle = -90;
            }
        }

        /// <summary>
        /// Create arc from given points.
        /// </summary>
        /// <param name="pt1">The start point.</param>
        /// <param name="pt2">The end point.</param>
        /// <param name="rcBounds">The bound rectangle.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="sweepAngle">The sweep angle.</param>
        public static void ArcFromPoints(
            System.Drawing.PointF pt1,
            System.Drawing.PointF pt2,
            out System.Drawing.RectangleF rcBounds,
            out float startAngle,
            out float sweepAngle)
        {
            rcBounds = new System.Drawing.RectangleF(0.0f, 0.0f, 0.0f, 0.0f);
            float diffX = pt2.X - pt1.X;
            float diffY = pt2.Y - pt1.Y;
            float width = diffX * 2;
            float height = diffY * 2;
            if (width > 0.0f && height > 0.0f)
            {
                rcBounds.Location = new System.Drawing.PointF(pt1.X - diffX, pt1.Y);
                rcBounds.Size = new System.Drawing.SizeF(width, height);
                startAngle = 0.0f;
                sweepAngle = -90.0f;
            }
            else if (width > 0.0f && height <= 0.0f)
            {
                rcBounds.Location = new System.Drawing.PointF(pt1.X, pt2.Y);
                rcBounds.Size = new System.Drawing.SizeF(width, -height);
                startAngle = 180.0f;
                sweepAngle = 90.0f;
            }
            else if (width <= 0.0f && height > 0.0f)
            {
                rcBounds.Location = new System.Drawing.PointF(pt2.X + diffX, pt1.Y - diffY);
                rcBounds.Size = new System.Drawing.SizeF(-width, height);
                startAngle = 0.0f;
                sweepAngle = 90.0f;
            }
            else
            {
                rcBounds.Location = new System.Drawing.PointF(pt2.X, pt2.Y + diffY);
                rcBounds.Size = new System.Drawing.SizeF(-width, -height);
                startAngle = 180.0f;
                sweepAngle = -90.0f;
            }
        }

        /// <summary>
        /// Calculate distance between two points.
        /// </summary>
        /// <param name="pt1">The start point.</param>
        /// <param name="pt2">The end point.</param>
        /// <returns>Distance between the points.</returns>
        public static double PointDistance(PointF pt1, PointF pt2)
        {
            double d = 0;
            double dx = pt2.X - pt1.X;
            double dy = pt2.Y - pt1.Y;
            double t = (dx * dx) + (dy * dy);
            if (t > 0)
            {
                d = Math.Sqrt(t);
            }
            return d;
        }

        /// <summary>
        /// Gets the point by offset.
        /// </summary>
        /// <param name="ptStart">The start point.</param>
        /// <param name="ptEnd">The end point.</param>
        /// <param name="fOffsetFromEnd">The offset from end to calculate new point.</param>
        /// <returns>Point that contain line.</returns>
        public static PointF GetPointByOffset(PointF ptStart, PointF ptEnd, float fOffsetFromEnd)
        {
            // Fixed d9211
            // if( ptStart == ptEnd )
            //    throw new ArgumentOutOfRangeException( "ptStart == ptEnd" );
            double fAngleRadian = LineAngle(ptStart, ptEnd);
            float fX = ((float)Math.Cos(fAngleRadian)) * fOffsetFromEnd;
            float fY = ((float)Math.Sin(fAngleRadian)) * fOffsetFromEnd;

            return new PointF(ptEnd.X - fX, ptEnd.Y - fY);
        }

        /// <summary>
        /// Calculate line slope.
        /// </summary>
        /// <param name="pt1">The start point.</param>
        /// <param name="pt2">The end point.</param>
        /// <returns>The line slope.</returns>
        public static double LineSlope(PointF pt1, PointF pt2)
        {
            double m = 0;
            double dx = pt2.X - pt1.X;
            double dy = pt2.Y - pt1.Y;
            if (dx != 0)
            {
                m = dy / dx;
            }
            else
            {
                throw new SlopeUndefinedException();
            }
            return m;
        }

        /// <summary>
        /// Calculate line slope.
        /// </summary>
        /// <param name="pt1">The start point.</param>
        /// <param name="pt2">The end point.</param>
        /// <returns>The line slope.</returns>
        public static double LineSlope(System.Drawing.Point pt1, System.Drawing.Point pt2)
        {
            double m = 0;
            double dx = (double)pt2.X - (double)pt1.X;
            double dy = (double)pt2.Y - (double)pt1.Y;
            if (dx != 0)
            {
                m = dy / dx;
            }
            else
            {
                throw new SlopeUndefinedException();
            }
            return m;
        }

        /// <summary>
        /// Hit test line with slop.
        /// </summary>
        /// <param name="pt1">The start point.</param>
        /// <param name="pt2">The end point.</param>
        /// <param name="ptTest">The point to test.</param>
        /// <param name="slop">The slop value.</param>
        /// <returns>If line contain test point.</returns>
        public static bool HitTestLine(PointF pt1, PointF pt2, PointF ptTest, float slop)
        {
            bool hit = false;

            GraphicsPath grfxPath = new GraphicsPath();
            grfxPath.AddLine(pt1, pt2);

            // GraphicsPath.Widen throws an OutOfMemory exception when the path is empty. This is a documented bug.
            if ((slop > 0.0f) && (pt1 != pt2))
            {
                Pen pen = new Pen(Color.Black, slop);
                try
                {
                    grfxPath.Widen(pen);
                }
                catch
                {
                }
                pen.Dispose();
            }

            Region rgn = new Region(grfxPath);
            hit = rgn.IsVisible(ptTest);
            rgn.Dispose();

            grfxPath.Dispose();

            return hit;
        }

        /// <summary>
        /// Gets the all line intersect points.
        /// </summary>
        /// <param name="pt1">The start point.</param>
        /// <param name="pt2">The end point.</param>
        /// <param name="rect">The bounding rectangle.</param>
        /// <param name="ptsIntersect">The intersect points.</param>
        /// <returns>Line intersect count.</returns>
        public static int GetLineIntersect(PointF pt1, PointF pt2, RectangleF rect, out PointF[] ptsIntersect)
        {
            int intersectCount = 0;
            ptsIntersect = null;

            bool isVerticalLine = false;
            double m = 0;
            float xIntercept;
            float yIntercept;
            PointF[] ptsFound = new PointF[4];
            float rcTop = rect.Top;
            float rcBottom = rect.Bottom;
            float rcLeft = rect.Left;
            float rcRight = rect.Right;

            if ((pt2.X - pt1.X) != 0)
            {
                m = LineSlope(pt1, pt2);
            }
            else
            {
                isVerticalLine = true;
            }

            // Test top side.
            if ((pt1.Y <= rcTop && pt2.Y >= rcTop) || (pt2.Y <= rcTop && pt1.Y >= rcTop))
            {
                if (isVerticalLine)
                {
                    xIntercept = pt1.X;
                }
                else
                {
                    xIntercept = (float)((rcTop + (m * pt1.X - pt1.Y)) / m);
                }

                if (xIntercept >= rcLeft && xIntercept <= rcRight)
                {
                    ptsFound[intersectCount] = new PointF(xIntercept, rcTop);
                    intersectCount++;
                }
            }

            // Test bottom side.
            if ((pt1.Y <= rcBottom && pt2.Y >= rcBottom) || (pt2.Y <= rcBottom && pt1.Y >= rcBottom))
            {
                if (isVerticalLine)
                {
                    xIntercept = pt1.X;
                }
                else
                {
                    xIntercept = (float)((rcBottom + (m * pt1.X - pt1.Y)) / m);
                }

                if (xIntercept >= rcLeft && xIntercept <= rcRight)
                {
                    ptsFound[intersectCount] = new PointF(xIntercept, rcBottom);
                    intersectCount++;
                }
            }

            // Test left side.
            if (!isVerticalLine && (pt1.X <= rcLeft && pt2.X >= rcLeft) || (pt2.X <= rcLeft && pt1.X >= rcLeft))
            {
                yIntercept = (float)(m * (rcLeft - pt1.X) + pt1.Y);

                if (yIntercept >= rcTop && yIntercept <= rcBottom)
                {
                    ptsFound[intersectCount] = new PointF(rcLeft, yIntercept);
                    intersectCount++;
                }
            }

            // Test right side.
            if (!isVerticalLine && (pt1.X <= rcRight && pt2.X >= rcRight) || (pt2.X <= rcRight && pt1.X >= rcRight))
            {
                yIntercept = (float)(m * (rcRight - pt1.X) + pt1.Y);

                if (yIntercept >= rcTop && yIntercept <= rcBottom)
                {
                    ptsFound[intersectCount] = new PointF(rcRight, yIntercept);
                    intersectCount++;
                }
            }

            if (intersectCount > 0)
            {
                ptsIntersect = new PointF[intersectCount];
                for (int ptIdx = 0; ptIdx < intersectCount; ptIdx++)
                {
                    ptsIntersect[ptIdx] = ptsFound[ptIdx];
                }
            }

            return intersectCount;
        }

        /// <summary>
        /// Gets counters the clockwise.
        /// </summary>
        /// <param name="pt0">The first point.</param>
        /// <param name="pt1">The second point.</param>
        /// <param name="pt2">The third point.</param>
        /// <returns>The counter value.</returns>
        public static int CounterClockwise(PointF pt0, PointF pt1, PointF pt2)
        {
            float dx1 = pt1.X - pt0.X;
            float dx2 = pt2.X - pt0.X;
            float dy1 = pt1.Y - pt0.Y;
            float dy2 = pt2.Y - pt0.Y;
            return ((dx1 * dy2 > dy1 * dx2) ? 1 : -1);
        }

        /// <summary>
        /// Check for intersecting between two lines.
        /// </summary>
        /// <param name="pt0">The first start point.</param>
        /// <param name="pt1">The first end point.</param>
        /// <param name="pt2">The second start point.</param>
        /// <param name="pt3">The second end point.</param>
        /// <returns>true, if line intersect.</returns>
        public static bool LinesIntersect(PointF pt0, PointF pt1, PointF pt2, PointF pt3)
        {
            return (((Geometry.CounterClockwise(pt0, pt1, pt2) * Geometry.CounterClockwise(pt0, pt1, pt3)) <= 0) &&
                ((Geometry.CounterClockwise(pt2, pt3, pt0) * Geometry.CounterClockwise(pt2, pt3, pt1) <= 0)));
        }

        /// <summary>
        /// Intersect lines.
        /// </summary>
        /// <param name="gfxNodePath">The graphics node path.</param>
        /// <param name="ptStart">The start point.</param>
        /// <param name="ptEnd">The end point.</param>
        /// <returns>true, if lines intersect</returns>
        public static bool LinesIntersect(GraphicsPath gfxNodePath, PointF ptStart, PointF ptEnd)
        {
            bool bSuccess = false;
            PointF[] ptsPath = gfxNodePath.PathPoints;

            for (int i = 0, nLength = ptsPath.Length; i < nLength && !bSuccess; i++)
            {
                bSuccess = LinesIntersect(ptsPath[i], ptsPath[(i + 1) % nLength], ptStart, ptEnd);
            }

            return bSuccess;
        }

        /// <summary>
        /// Calcs the line segment intersect.
        /// </summary>
        /// <param name="seg1">The first segment.</param>
        /// <param name="seg2">The second segment.</param>
        /// <param name="ptIntersect">The intersect point.</param>
        /// <returns>true, if line segment intersect.</returns>
        public static bool CalcLineSegmentIntersect(LineSegment seg1, LineSegment seg2, out PointF ptIntersect)
        {
            bool intersect = false;

            float xi = 0.0f;
            float yi = 0.0f;

            intersect = CalcLineSegmentIntersect(
                seg1.Point1.X,
                seg1.Point1.Y,
                seg1.Point2.X,
                seg1.Point2.Y,
                seg2.Point1.X,
                seg2.Point1.Y,
                seg2.Point2.X,
                seg2.Point2.Y,
                ref xi,
                ref yi);

            ptIntersect = new PointF(xi, yi);

            return intersect;
        }

        /// <summary>
        /// Calcs the line segment intersect.
        /// </summary>
        /// <param name="ptStart1">The first start point.</param>
        /// <param name="ptEnd1">The first end point.</param>
        /// <param name="ptStart2">The second start point.</param>
        /// <param name="ptEnd2">The second end point.</param>
        /// <param name="ptIntersect">The intersect point.</param>
        /// <returns>true, line segment intersect.</returns>
        public static bool CalcLineSegmentIntersect(PointF ptStart1, PointF ptEnd1, PointF ptStart2, PointF ptEnd2, ref PointF ptIntersect)
        {
            bool bIntrrsect = false;

            float ua = (ptEnd2.X - ptStart2.X) * (ptStart1.Y - ptStart2.Y) -
                (ptEnd2.Y - ptStart2.Y) * (ptStart1.X - ptStart2.X);
            ua /= (ptEnd2.Y - ptStart2.Y) * (ptEnd1.X - ptStart1.X) -
                (ptEnd2.X - ptStart2.X) * (ptEnd1.Y - ptStart1.Y);

            if (0 <= ua && ua <= 1)
            {
                bIntrrsect = true;
            }

            if (bIntrrsect)
            {
                float ub = (ptEnd1.X - ptStart1.X) * (ptStart1.Y - ptStart2.Y) -
                    (ptEnd1.Y - ptStart1.Y) * (ptStart1.X - ptStart2.X);
                ub /= (ptEnd2.Y - ptStart2.Y) * (ptEnd1.X - ptStart1.X) -
                    (ptEnd2.X - ptStart2.X) * (ptEnd1.Y - ptStart1.Y);

                if (0 <= ub && ub <= 1)
                {
                    bIntrrsect = true;
                }
                else
                {
                    bIntrrsect = false;
                }

                float fX = ptStart1.X + ua * (ptEnd1.X - ptStart1.X);
                float fY = ptStart1.Y + ua * (ptEnd1.Y - ptStart1.Y);

                ptIntersect = new PointF(fX, fY);
            }

            return bIntrrsect;
        }

        /// <summary>
        /// Calcs the line segment intersect.
        /// </summary>
        /// <param name="x0">The first x.</param>
        /// <param name="y0">The first y.</param>
        /// <param name="x1">The second x.</param>
        /// <param name="y1">The second y.</param>
        /// <param name="x2">The third x.</param>
        /// <param name="y2">The third y.</param>
        /// <param name="x3">The four x.</param>
        /// <param name="y3">The four y.</param>
        /// <param name="xi">The intersection x.</param>
        /// <param name="yi">The intersection y.</param>
        /// <returns>true, if line segment intersect.</returns>
        public static bool CalcLineSegmentIntersect(
            float x0,
            float y0,
            float x1,
            float y1,
            float x2,
            float y2,
            float x3,
            float y3,
            ref float xi,
            ref float yi)
        {
            bool intersect = false;

            float a1, b1, c1;   // constants of linear equations
            float a2, b2, c2;
            float det_inv;    // inverse of the determinant of the coefficient matrix
            float m1, m2;

            if (x1 - x0 != 0)
            {
                m1 = (y1 - y0) / (x1 - x0);
            }
            else
            {
                m1 = float.MaxValue;
            }

            if ((x3 - x2) != 0)
            {
                m2 = (y3 - y2) / (x3 - x2);
            }
            else
            {
                m2 = float.MaxValue;
            }

            if (m1 != m2)
            {
                a1 = m1;
                a2 = m2;

                b1 = -1;
                b2 = -1;

                c1 = (y0 - m1 * x0);
                c2 = (y2 - m2 * x2);

                det_inv = 1 / (a1 * b2 - a2 * b1);

                xi = (b1 * c2 - b2 * c1) * det_inv;
                yi = (a2 * c1 - a1 * c2) * det_inv;
            }

            return intersect;
        }

        /// <summary>
        /// Calcs the vertical intercept.
        /// </summary>
        /// <param name="pt1">The first point.</param>
        /// <param name="pt2">The second point.</param>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <returns>true, if calculate vertical intercept.</returns>
        public static bool CalcVerticalIntercept(PointF pt1, PointF pt2, float x, out float y)
        {
            bool interceptFound = false;

            y = 0;

            float m = 0.0f;
            float dy = pt2.Y - pt1.Y;
            float dx = pt2.X - pt1.X;
            if (dx != 0)
            {
                m = dy / dx;
                y = m * x;
                interceptFound = true;
            }

            return interceptFound;
        }

        /// <summary>
        /// Calcs the horizontal intercept.
        /// </summary>
        /// <param name="pt1">The first point.</param>
        /// <param name="pt2">The second point.</param>
        /// <param name="y">The y position.</param>
        /// <param name="x">The x position.</param>
        /// <returns>true, if calculate horizontal intercept</returns>
        public static bool CalcHorizontalIntercept(PointF pt1, PointF pt2, float y, out float x)
        {
            bool interceptFound = false;

            x = 0;

            float m = 0.0f;
            float dy = pt2.Y - pt1.Y;
            float dx = pt2.X - pt1.X;
            if (dx != 0)
            {
                m = dy / dx;
                if (m != 0)
                {
                    x = y / m;
                    interceptFound = true;
                }
            }

            return interceptFound;
        }

        /// <summary>
        /// Gets the intersecting point of two orthogonal line segments.
        /// </summary>
        /// <param name="pt1">First point in first line segment</param>
        /// <param name="pt2">Second point in first line segment</param>
        /// <param name="pt3">First point in second line segment</param>
        /// <param name="pt4">Second point in second line segment</param>
        /// <param name="ptIntersect">Point at which the orthogonal line segments intersect</param>
        /// <returns>true if an intersection exists; otherwise false</returns>
        /// <remarks>
        /// <para>
        /// This method only returns an intersection point if one of the line
        /// segments is horizontal and the other is vertical. It does not
        /// calculate the intersection of lines that are not orthogonal to
        /// each other and parallel with the X and Y axes.
        /// </para>
        /// </remarks>
        public static bool GetOrthogonalIntersect(PointF pt1, PointF pt2, PointF pt3, PointF pt4, out PointF ptIntersect)
        {
            bool intersectFound = false;

            ptIntersect = new PointF(0, 0);

            if (pt1.X == pt2.X && pt3.Y == pt4.Y)
            {
                // Line segment 1 is vertical and line segment 2 is horizontal.
                // Test to see if line segments cross.
                float horz = pt3.Y;
                float vert = pt1.X;
                if (((horz >= pt1.Y && horz <= pt2.Y) || (horz >= pt2.Y && horz <= pt1.Y)) &&
                    ((vert >= pt3.X && vert <= pt4.X) || (vert >= pt4.X && vert <= pt3.X)))
                {
                    intersectFound = true;
                    ptIntersect.X = vert;
                    ptIntersect.Y = horz;
                }
            }
            else if (pt3.X == pt4.X && pt1.Y == pt2.Y)
            {
                // Line segment 2 is vertical and line segment 1 is horizontal.
                // Test to see if line segments cross.
                float horz = pt1.Y;
                float vert = pt3.X;
                if (((horz >= pt3.Y && horz <= pt4.Y) || (horz >= pt4.Y && horz <= pt3.Y)) &&
                    ((vert >= pt1.X && vert <= pt2.X) || (vert >= pt2.X && vert <= pt1.X)))
                {
                    intersectFound = true;
                    ptIntersect.X = vert;
                    ptIntersect.Y = horz;
                }
            }

            return intersectFound;
        }

        /// <summary>
        /// Test two orthogonal line segments to determine if they intersect.
        /// </summary>
        /// <param name="pt1">First point in first line segment</param>
        /// <param name="pt2">Second point in first line segment</param>
        /// <param name="pt3">First point in second line segment</param>
        /// <param name="pt4">Second point in second line segment</param>
        /// <returns>true if an intersection exists; otherwise false</returns>
        /// <remarks>
        /// <para>
        /// This method only returns true if one of the line
        /// segments is horizontal and the other is vertical and
        /// the two line segments cross.
        /// </para>
        /// </remarks>
        public static bool TestOrthogonalIntersect(PointF pt1, PointF pt2, PointF pt3, PointF pt4)
        {
            bool intersectFound = false;

            if (pt1.X == pt2.X && pt3.Y == pt4.Y)
            {
                // Line segment 1 is vertical and line segment 2 is horizontal.
                // Test to see if line segments cross.
                float horz = pt3.Y;
                float vert = pt1.X;
                if (((horz > pt1.Y && horz < pt2.Y) || (horz > pt2.Y && horz < pt1.Y)) &&
                    ((vert > pt3.X && vert < pt4.X) || (vert > pt4.X && vert < pt3.X)))
                {
                    intersectFound = true;
                }
            }
            else if (pt3.X == pt4.X && pt1.Y == pt2.Y)
            {
                // Line segment 2 is vertical and line segment 1 is horizontal.
                // Test to see if line segments cross.
                float horz = pt1.Y;
                float vert = pt3.X;
                if (((horz > pt3.Y && horz < pt4.Y) || (horz > pt4.Y && horz < pt3.Y)) &&
                    ((vert > pt1.X && vert < pt2.X) || (vert > pt2.X && vert < pt1.X)))
                {
                    intersectFound = true;
                }
            }

            return intersectFound;
        }

        /// <summary>
        /// Tests the given orthogonal line segment to see if it intersects the given
        /// rectangle.
        /// </summary>
        /// <param name="pt1">First point on the line segment</param>
        /// <param name="pt2">Second point on the line segment</param>
        /// <param name="rect">Rectangle to test</param>
        /// <returns>true if they intersect, otherwise false</returns>
        /// <remarks>
        /// <para>
        /// This method only works if the line segment is either horizontal
        /// or vertical. In other words, the line segment must be parallel to
        /// either the X or Y axis.
        /// </para>
        /// </remarks>
        public static bool TestOrthogonalIntersect(PointF pt1, PointF pt2, RectangleF rect)
        {
            PointF pt3 = new PointF(0, 0);
            PointF pt4 = new PointF(0, 0);

            pt3.X = rect.Left;
            pt3.Y = rect.Top;
            pt4.X = rect.Right;
            pt4.Y = rect.Top;

            if (TestOrthogonalIntersect(pt1, pt2, pt3, pt4))
            {
                return true;
            }

            pt4.X = rect.Left;
            pt4.Y = rect.Bottom;

            if (TestOrthogonalIntersect(pt1, pt2, pt3, pt4))
            {
                return true;
            }

            pt3.X = rect.Right;
            pt3.Y = rect.Bottom;

            if (TestOrthogonalIntersect(pt1, pt2, pt4, pt3))
            {
                return true;
            }

            pt4.X = rect.Right;
            pt4.Y = rect.Top;

            if (TestOrthogonalIntersect(pt1, pt2, pt4, pt3))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if line segment intersects with rectangle.
        /// </summary>
        /// <param name="rect">The rectangle to check.</param>
        /// <param name="pt1">The first point in line segment.</param>
        /// <param name="pt2">The second point in line segment.</param>
        /// <returns>true, if intersect with line segment.</returns>
        public static bool RectIntersectsWithLineSegment(RectangleF rect, PointF pt1, PointF pt2)
        {
            rect.Inflate(-1, -1);

            if (rect.Contains(pt1) || rect.Contains(pt2)) return true;

            if (pt1.X == pt2.X || pt1.Y == pt2.Y)
            {
                // Line segment is either horizontal or vertical
                PointF pt3 = new PointF(0, 0);
                PointF pt4 = new PointF(0, 0);

                pt3.X = rect.Left;
                pt3.Y = rect.Top;
                pt4.X = rect.Right;
                pt4.Y = rect.Top;

                if (TestOrthogonalIntersect(pt1, pt2, pt3, pt4))
                {
                    return true;
                }

                pt4.X = rect.Left;
                pt4.Y = rect.Bottom;

                if (TestOrthogonalIntersect(pt1, pt2, pt3, pt4))
                {
                    return true;
                }

                pt3.X = rect.Right;
                pt3.Y = rect.Bottom;

                if (TestOrthogonalIntersect(pt1, pt2, pt4, pt3))
                {
                    return true;
                }

                pt4.X = rect.Right;
                pt4.Y = rect.Top;

                if (TestOrthogonalIntersect(pt1, pt2, pt4, pt3))
                {
                    return true;
                }

                return false;
            }

            throw new ArgumentException();
        }

        /// <summary>
        /// Gets angle between two points.
        /// </summary>
        /// <param name="pt1">The first point.</param>
        /// <param name="pt2">The second point.</param>
        /// <returns>The angle.</returns>
        public static double LineAngle(PointF pt1, PointF pt2)
        {
            double radians = 0;

            double vx = pt2.X - pt1.X;
            double vy = pt2.Y - pt1.Y;

            if (vx == 0)
            {
                // vertical line - either 90 or 270
                if (vy <= 0)
                {
                    radians = RadiansQuadrant3;
                }
                else
                {
                    radians = RadiansQuadrant1;
                }
            }
            else if (vy == 0)
            {
                // horizontal line - either 0 or 180
                if (vx < 0)
                {
                    radians = RadiansQuadrant2;
                }
                else
                {
                    radians = 0;
                }
            }
            else
            {
                radians = Math.Atan(vy / vx);

                if (vx < 0 && vy > 0)
                {
                    // quandrant 2
                    radians = RadiansQuadrant2 + radians;
                }
                else if (vx < 0 && vy < 0)
                {
                    // quandrant 3
                    radians = RadiansQuadrant2 + radians;
                }
                else if (vx > 0 && vy < 0)
                {
                    // quandrant 4
                    radians = RadiansQuadrant4 + radians;
                }
            }

            return radians;
        }

        /// <summary>
        /// Gets angle between two points.
        /// </summary>
        /// <param name="pt1">The first point.</param>
        /// <param name="pt2">The second point.</param>
        /// <returns>The angle.</returns>
        public static double LineAngle(System.Drawing.Point pt1, System.Drawing.Point pt2)
        {
            double radians;
            double vx = (double)pt2.X - pt1.X;
            double vy = (double)pt2.Y - pt1.Y;

            radians = Math.Atan(vy / vx);

            if (vx < 0 && vy > 0)
            {
                // quandrant 2
                radians = RadiansQuadrant2 + radians;
            }
            else if (vx < 0 && vy < 0)
            {
                // quandrant 3
                radians = RadiansQuadrant2 + radians;
            }
            else if (vx > 0 && vy < 0)
            {
                // quandrant 4
                radians = RadiansQuadrant4 + radians;
            }
            return radians;
        }

        /// <summary>
        /// Gets angle between three points.
        /// </summary>
        /// <param name="ptA">The point A.</param>
        /// <param name="ptO">The origin point.</param>
        /// <param name="ptB">The point B.</param>
        /// <returns>The angle.</returns>
        public static double LineAngle(System.Drawing.Point ptA, System.Drawing.Point ptO, System.Drawing.Point ptB)
        {
            float fXA = ptA.X - ptO.X;
            float fYA = ptA.Y - ptO.Y;
            float fXB = ptB.X - ptO.X;
            float fYB = ptB.Y - ptO.Y;

            float fTemp1 = (float)(Math.Sqrt(Math.Pow(fXA, 2) + Math.Pow(fYA, 2)));
            float fTemp2 = (float)(Math.Sqrt(Math.Pow(fXB, 2) + Math.Pow(fYB, 2)));
            float fTemp3 = ((fXA * fXB) + (fYA * fYB));
            float fTemp4 = fTemp3 / (fTemp1 * fTemp2);
            if (fTemp4 < -1.0f)
                fTemp4 = -1.0f;
            else if (fTemp4 > 1.0f)
                fTemp4 = 1.0f;

            float fRadian = (float)Math.Acos(fTemp4);

            // define angle sign
            float fSign = ((ptO.Y - ptA.Y) * (ptB.X - ptA.X)) - ((ptB.Y - ptA.Y) * (ptO.X - ptA.X));
            if (fSign < 0)
                fRadian = -fRadian;

            return fRadian;
        }

        /// <summary>
        /// Gets angle between three points.
        /// </summary>
        /// <param name="ptA">The point A.</param>
        /// <param name="ptO">The origin point.</param>
        /// <param name="ptB">The point B.</param>
        /// <returns>The angle.</returns>
        public static double LineAngle(PointF ptA, PointF ptO, PointF ptB)
        {
            Point pt1 = new Point((int)ptA.X, (int)ptA.Y);
            Point pt2 = new Point((int)ptO.X, (int)ptO.Y);
            Point pt3 = new Point((int)ptB.X, (int)ptB.Y);

            return LineAngle(pt1, pt2, pt3);
        }

        /// <summary>
        /// Creates a regions from given a graphics path.
        /// </summary>
        /// <param name="gfxPath">Graphics path to create region from</param>
        /// <returns>Region the graphics path occupies</returns>
        /// <remarks>
        /// <para>
        /// This version of CreateRegionFromGraphicsPath does no conversion between
        /// logical units and pixels. It assumes that the caller has already
        /// performed any necessary conversion.
        /// </para>
        /// </remarks>
        public static Region[] CreateRegionFromGraphicsPath(GraphicsPath gfxPath)
        {
            ArrayList regions = new ArrayList();

            // max region scan rectangles count
            int nCount = CommonUsedValues.MAX_REGION_SUBPATH;

            GraphicsPathIterator itPath = new GraphicsPathIterator(gfxPath);
            itPath.Rewind();
            GraphicsPath curSubPath = new GraphicsPath();
            bool isSubPathClosed;

            for (int idxSubPath = 0; idxSubPath < itPath.SubpathCount; idxSubPath++)
            {
                itPath.NextSubpath(curSubPath, out isSubPathClosed);
                int nRegionIndex = idxSubPath / nCount;

                if (regions.Count < nRegionIndex + 1)
                {
                    regions.Add(new Region(curSubPath));
                }
                else
                {
                    ((Region)regions[nRegionIndex]).Union(curSubPath);
                }
            }

            curSubPath.Dispose();
            itPath.Dispose();

            return (Region[])regions.ToArray(typeof(Region));
        }

        /// <summary>
        /// Creates a region given a graphics path and measurement units.
        /// </summary>
        /// <param name="grfxPath">Graphics path to create region from.</param>
        /// <param name="units">GraphicsPath's PathPoints measure units.</param>
        /// <returns>Region the graphics path occupies</returns>
        /// <remarks>
        /// <para>
        /// This method generates a region given a graphics path. The region
        /// created is always in pixel units. If the incoming graphics path
        /// is not in pixel units, the points in the graphics path are
        /// converted before generating the region.
        /// </para>
        /// </remarks>
        public static Region CreateRegionFromGraphicsPath(GraphicsPath grfxPath, MeasureUnits units)
        {
            Region rgn = null;

            GraphicsPathIterator itPath = new GraphicsPathIterator(grfxPath);
            itPath.Rewind();
            GraphicsPath curSubPath = new GraphicsPath();
            bool isSubPathClosed;

            for (int idxSubPath = 0; idxSubPath < itPath.SubpathCount; idxSubPath++)
            {
                itPath.NextSubpath(curSubPath, out isSubPathClosed);

                if (units != MeasureUnits.Pixel)
                {
                    PointF[] subPathPts = curSubPath.PathPoints;
                    PointF[] subPathPtsPixels = new PointF[subPathPts.Length];

                    for (int ptIdx = 0; ptIdx < subPathPts.Length; ptIdx++)
                    {
                        subPathPtsPixels[ptIdx] = MeasureUnitsConverter.Convert(subPathPts[ptIdx], units, MeasureUnits.Pixel);
                    }

                    curSubPath = new GraphicsPath(subPathPtsPixels, curSubPath.PathTypes);
                }

                if (rgn == null)
                {
                    rgn = new Region(curSubPath);
                }
                else
                {
                    rgn.Union(curSubPath);
                }
            }

            curSubPath.Dispose();
            itPath.Dispose();

            return rgn;
        }

        /// <summary>
        /// Gets the box side of endpoint to start point.
        /// </summary>
        /// <param name="ptStartPoint">The start point.</param>
        /// <param name="ptEndPoint">The end point.</param>
        /// <returns>The box side.</returns>
        public static BoxSide GetBoxSide(PointF ptStartPoint, PointF ptEndPoint)
        {
            BoxSide side = BoxSide.Top;

            if (ptEndPoint.X > ptStartPoint.X)
            {
                side = BoxSide.Left;
            }
            else if (ptEndPoint.X < ptStartPoint.X)
            {
                side = BoxSide.Right;
            }
            else if (ptEndPoint.Y < ptStartPoint.Y)
            {
                side = BoxSide.Bottom;
            }
            else if (ptEndPoint.Y > ptStartPoint.Y)
            {
                side = BoxSide.Top;
            }

            return side;
        }

        /// <summary>
        /// Widens the point.
        /// </summary>
        /// <param name="ptPoint">The given point.</param>
        /// <param name="side">The box side.</param>
        /// <param name="width">The widen width.</param>
        /// <returns>The widen point.</returns>
        public static PointF WidenPoint(PointF ptPoint, BoxSide side, float width)
        {
            PointF ptToRetrun = ptPoint;

            switch (side)
            {
                case BoxSide.Left:
                    ptToRetrun.X -= width;
                    break;
                case BoxSide.Right:
                    ptToRetrun.X += width;
                    break;
                case BoxSide.Top:
                    ptToRetrun.Y -= width;
                    break;
                case BoxSide.Bottom:
                    ptToRetrun.Y += width;
                    break;
            }

            return ptToRetrun;
        }

        /// <summary>
        /// Scale rectangle to given width.
        /// </summary>
        /// <param name="rcRect">The given rectangle.</param>
        /// <param name="fWidth">The widen width .</param>
        /// <returns>The widen rect.</returns>
        public static RectangleF WidenRect(RectangleF rcRect, float fWidth)
        {
            RectangleF rcToReturn = rcRect;

            // offset location
            rcToReturn.X -= fWidth;
            rcToReturn.Y -= fWidth;

            // scale size
            rcToReturn.Width += fWidth * 2;
            rcToReturn.Height += fWidth * 2;

            return rcToReturn;
        }

        #region Boundary intersects
        /// <summary>
        /// The Region class internally rounds off floating point values to the nearest integer. This causes problems when using GraphicsUnit.Inches.
        /// Using Pixel units for calculating the region bounds helps avoid this round off error.
        /// </summary>
        /// <param name="grfxPath">The node graphics path to intercept..</param>
        /// <param name="units">The units.</param>
        /// <param name="pt1">The first point.</param>
        /// <param name="pt2">The second point.</param>
        /// <param name="ptIntercept">The given point.</param>
        /// <returns>true, if boundary intercept.</returns>
        public static bool GetBoundaryIntercept(GraphicsPath grfxPath, MeasureUnits units, PointF pt1, PointF pt2, out PointF ptIntercept)
        {
            // Convert points to pixel units and generate region
            PointF pt1Pixels = MeasureUnitsConverter.Convert(pt1, units, MeasureUnits.Pixel);
            PointF pt2Pixels = MeasureUnitsConverter.Convert(pt2, units, MeasureUnits.Pixel);
            Region rgn = CreateRegionFromGraphicsPath(grfxPath, units);

            if (rgn == null)
            {
                throw new InvalidOperationException("Unable to create region from graphics path");
            }

            // Gets the boundary intercept
            bool found = GetBoundaryIntercept(rgn, pt1Pixels, pt2Pixels, out ptIntercept);

            if (found)
            {
                // Convert intercept point back to original units
                ptIntercept = MeasureUnitsConverter.Convert(ptIntercept, MeasureUnits.Pixel, units);
            }

            rgn.Dispose();

            return found;
        }

        /// <summary>
        /// Gets the boundary intercept.
        /// </summary>
        /// <param name="grfxPath">The node to intercept.</param>
        /// <param name="ptCentralPoint">Central point.</param>
        /// <param name="side">The side of box.</param>
        /// <param name="ptIntercept">The intercept point.</param>
        /// <returns>true, if boundary intercept.</returns>
        public static bool GetBoundaryIntercept(GraphicsPath grfxPath, PointF ptCentralPoint, BoxSide side, out PointF ptIntercept)
        {
            SizeF szSize = grfxPath.GetBounds().Size;
            PointF ptStart = ptCentralPoint;

            switch (side)
            {
                case BoxSide.Left:
                    szSize.Width = -szSize.Width;
                    szSize.Height = 0;
                    break;
                case BoxSide.Right:
                    szSize.Height = 0;
                    break;
                case BoxSide.Top:
                    szSize.Width = 0;
                    szSize.Height = -szSize.Height;
                    break;
                case BoxSide.Bottom:
                    szSize.Width = 0;
                    break;
            }

            ptStart.X += szSize.Width;
            ptStart.Y += szSize.Height;

            return GetBoundaryIntercept(grfxPath, MeasureUnits.Pixel, ptCentralPoint, ptStart, out ptIntercept);
        }

        /// <summary>
        /// Gets the boundary intercept.
        /// </summary>
        /// <param name="grphPath">The grphPath to intercept.</param>
        /// <param name="units">The units.</param>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <param name="heading">The heading.</param>
        /// <param name="ptIntsct">The given point.</param>
        /// <returns>true, if boundary intercept.</returns>
        public static bool GetBoundaryIntercept(GraphicsPath grphPath, MeasureUnits units, PointF first, PointF second, CompassHeading heading, out PointF ptIntsct)
        {
            if (heading != CompassHeading.None)
            {
                RectangleF rcBounds = grphPath.GetBounds();
                SizeF szHeadingVector = CompassHeadingToVector(heading);

                second = new PointF(
                    first.X + szHeadingVector.Width * rcBounds.Width,
                    first.Y + szHeadingVector.Height * rcBounds.Height);
            }

            return GetBoundaryIntercept(grphPath, units, first, second, out ptIntsct);
        }

        /// <summary>
        /// Gets the boundary intercepting.
        /// </summary>
        /// <param name="rgn">The shape region.</param>
        /// <param name="pt1">The start point.</param>
        /// <param name="pt2">The end point.</param>
        /// <param name="ptIntercept">The intercept point.</param>
        /// <returns>true, if boundary intercept.</returns>
        public static bool GetBoundaryIntercept(System.Drawing.Region rgn, PointF pt1, PointF pt2, out PointF ptIntercept)
        {
            bool hasIntercept = false;
            ptIntercept = new PointF(0, 0);

            bool pt1InRegion = rgn.IsVisible(pt1);
            bool pt2InRegion = rgn.IsVisible(pt2);

            if ((pt1InRegion && !pt2InRegion) || (pt2InRegion && !pt1InRegion))
            {
                PointF ptInside;
                PointF ptOutside;

                if (pt1InRegion)
                {
                    ptInside = pt1;
                    ptOutside = pt2;
                }
                else
                {
                    ptInside = pt2;
                    ptOutside = pt1;
                }

                RectangleF[] rcScans = rgn.GetRegionScans(new Matrix());
                hasIntercept = GetBoundaryIntercept(rcScans, ptInside, ptOutside, out ptIntercept);
            }
            else
            {
                PointF ptInside = pt1;
                PointF ptOutside = pt2;
                RectangleF[] rcScans = rgn.GetRegionScans(new Matrix());
                hasIntercept = GetBoundaryIntercept(rcScans, ptInside, ptOutside, out ptIntercept);
            }

            return hasIntercept;
        }

        /// <summary>
        /// Gets the boundary intercepting.
        /// </summary>
        /// <param name="rcScans">The region scans.</param>
        /// <param name="ptInside">The inside point.</param>
        /// <param name="ptOutside">The outside point.</param>
        /// <param name="ptIntercept">The intercept point.</param>
        /// <returns>true, if boundary intercept.</returns>
        public static bool GetBoundaryIntercept(RectangleF[] rcScans, PointF ptInside, PointF ptOutside, out PointF ptIntercept)
        {
            bool hasIntercept = false;
            ptIntercept = new PointF(0, 0);

            RectangleF rcCur;
            PointF[] ptsIntersect;
            int numIntersects;
            double minDist = double.MaxValue;
            double curDist;
            PointF curPt;
            int rcIdx;
            int ptIdx;

            for (rcIdx = 0; rcIdx < rcScans.Length; rcIdx++)
            {
                rcCur = rcScans[rcIdx];
                numIntersects = Geometry.GetLineIntersect(ptOutside, ptInside, rcCur, out ptsIntersect);

                for (ptIdx = 0; ptIdx < numIntersects; ptIdx++)
                {
                    curPt = ptsIntersect[ptIdx];
                    curDist = Geometry.PointDistance(ptOutside, curPt);
                    if (curDist < minDist)
                    {
                        ptIntercept = curPt;
                        minDist = curDist;
                        hasIntercept = true;
                    }
                }
            }

            return hasIntercept;
        }

        /// <summary>
        /// Returns the four points that intersect the given region along the X and Y
        /// axis extending from the given point.
        /// </summary>
        /// <param name="rgn">Region to find intercepts for</param>
        /// <param name="ptTarget">Point inside the region from which the X and Y axes extend</param>
        /// <param name="ptIntercepts">Array to receive values</param>
        /// <remarks>
        /// This method is used to determine where to dock orthogonal lines at the boundaries of shapes. The
        /// points are returned in the following order: north, south, east, west.
        /// </remarks>
        public static void GetBoundaryIntercepts(System.Drawing.Region rgn, PointF ptTarget, PointF[] ptIntercepts)
        {
            if (!rgn.IsVisible(ptTarget))
            {
                throw new ArgumentException("Target point must lie within region", "ptTarget");
            }

            RectangleF[] rcScans = rgn.GetRegionScans(new Matrix());
            RectangleF rcBounds = Geometry.Union(rcScans);

            PointF ptOutside = new PointF(ptTarget.X, rcBounds.Top - 2);
            if (!Geometry.GetBoundaryIntercept(rcScans, ptTarget, ptOutside, out ptIntercepts[(int)CompassHeading.North - 1]))
            {
                throw new InvalidOperationException("GetBoundaryIntercepts failed");
            }

            ptOutside.Y = rcBounds.Bottom + 2;
            if (!Geometry.GetBoundaryIntercept(rcScans, ptTarget, ptOutside, out ptIntercepts[(int)CompassHeading.South - 1]))
            {
                throw new InvalidOperationException("GetBoundaryIntercepts failed");
            }

            ptOutside.X = rcBounds.Right + 2;
            ptOutside.Y = ptTarget.Y;
            if (!Geometry.GetBoundaryIntercept(rcScans, ptTarget, ptOutside, out ptIntercepts[(int)CompassHeading.East - 1]))
            {
                throw new InvalidOperationException("GetBoundaryIntercepts failed");
            }

            ptOutside.X = rcBounds.Left - 2;
            if (!Geometry.GetBoundaryIntercept(rcScans, ptTarget, ptOutside, out ptIntercepts[(int)CompassHeading.West - 1]))
            {
                throw new InvalidOperationException("GetBoundaryIntercepts failed");
            }
        }

        /// <summary>
        /// Convert integer value to compass heading enum.
        /// </summary>
        /// <param name="val">The val.</param>
        /// <returns>Compass heading.</returns>
        public static CompassHeading IntToCompassHeading(int val)
        {
            foreach (CompassHeading curHeading in System.Enum.GetValues(typeof(CompassHeading)))
            {
                if ((int)curHeading == val)
                {
                    return curHeading;
                }
            }

            throw new ArgumentException(String.Format("Cannot convert the integer value {0} to a CompassHeading. Value out of range.", val), "val");
        }

        /// <summary>
        /// Returns the four points that intersect the given region along the X and Y
        /// axis extending from the given point.
        /// </summary>
        /// <param name="grfxPath">The GRFX path.</param>
        /// <param name="units">The units.</param>
        /// <param name="ptTarget">Point inside the region from which the X and Y axes extend</param>
        /// <param name="ptIntercepts">Array to receive values</param>
        /// <remarks>
        /// This method is used to determine where to dock orthogonal lines at the boundaries of shapes. The
        /// points are returned in the following order: north, east, south, west, northeast, northwest, southeast, southwest.
        /// </remarks>
        public static void GetBoundaryIntercepts(GraphicsPath grfxPath, MeasureUnits units, PointF ptTarget, PointF[] ptIntercepts)
        {
            Region rgn = CreateRegionFromGraphicsPath(grfxPath, units);
            MeasureUnits unitDocument = MeasureUnits.Pixel;

            if (rgn == null)
            {
                throw new InvalidOperationException("Unable to create region from graphics path");
            }

            RectangleF[] rcScans = rgn.GetRegionScans(new Matrix());
            RectangleF rcBounds = Geometry.Union(rcScans);

            PointF ptTargetPixels = MeasureUnitsConverter.Convert(ptTarget, unitDocument, units);
            PointF curIntercept;

            if (!rgn.IsVisible(ptTargetPixels))
            {
                throw new InvalidOperationException("Target point must lie within the interior of the region");
            }

            Array values = Enum.GetValues(typeof(CompassHeading));

            foreach (CompassHeading heading in values)
            {
                if (heading == CompassHeading.None)
                    continue;

                curIntercept = GetBoundaryIntercept(rcScans, rcBounds, ptTargetPixels, heading);
                ptIntercepts[(int)heading - 1] = MeasureUnitsConverter.Convert(curIntercept, unitDocument, units);
            }
        }

        /// <summary>
        /// Gets the boundary intercept.
        /// </summary>
        /// <param name="rcScans">The region scans.</param>
        /// <param name="rcBounds">The bounds rectangle.</param>
        /// <param name="ptTarget">The target point.</param>
        /// <param name="compassHeading">The compass heading.</param>
        /// <returns>The point where it intercepts.</returns>
        private static PointF GetBoundaryIntercept(RectangleF[] rcScans, RectangleF rcBounds, PointF ptTarget, CompassHeading compassHeading)
        {
            PointF ptIntercept = PointF.Empty;
            PointF ptOutside = ptTarget;
            int nLengthOutside = 2;

            switch (compassHeading)
            {
                case CompassHeading.North:
                    ptOutside = new PointF(ptTarget.X, rcBounds.Top - nLengthOutside);
                    break;
                case CompassHeading.South:
                    ptOutside = new PointF(ptTarget.X, rcBounds.Bottom + nLengthOutside);
                    break;
                case CompassHeading.East:
                    ptOutside = new PointF(rcBounds.Right + nLengthOutside, ptTarget.Y);
                    break;
                case CompassHeading.West:
                    ptOutside = new PointF(rcBounds.Left - nLengthOutside, ptTarget.Y);
                    break;
                case CompassHeading.Northeast:
                    ptOutside = new PointF(rcBounds.Right + nLengthOutside, rcBounds.Top - nLengthOutside);
                    break;
                case CompassHeading.Northwest:
                    ptOutside = new PointF(rcBounds.Left - nLengthOutside, rcBounds.Top - nLengthOutside);
                    break;
                case CompassHeading.Southeast:
                    ptOutside = new PointF(rcBounds.Right + nLengthOutside, rcBounds.Bottom + nLengthOutside);
                    break;
                case CompassHeading.Southwest:
                    ptOutside = new PointF(rcBounds.Left - nLengthOutside, rcBounds.Bottom + nLengthOutside);
                    break;
            }

            if (!Geometry.GetBoundaryIntercept(rcScans, ptTarget, ptOutside, out ptIntercept))
            {
                throw new InvalidOperationException("GetBoundaryIntercepts failed");
            }

            return ptIntercept;
        }

        /// <summary>
        /// Gets the boundary intercepts.
        /// </summary>
        /// <param name="rcBounds">The rectangle for interception check.</param>
        /// <param name="ptLocation">The end point what contains in rectangle.</param>
        /// <param name="heading">endpoint container commpassHeading.</param>
        /// <returns>Array of interception points. Array contains eight points of eight commpasHeadng directions.</returns>
        public static PointF[] GetBoundaryIntercepts(RectangleF rcBounds, PointF ptLocation, CompassHeading heading)
        {
            PointF[] ptsIntercepts;
            RectangleF[] rcsBounds = new RectangleF[] { rcBounds };
            PointF ptIntercept;

            if (heading == CompassHeading.None)
            {
                ptsIntercepts = new PointF[4];
                Array arr = Enum.GetValues(typeof(CompassHeading));
                int n = 0;

                foreach (CompassHeading headingTmp in arr)
                {
                    if (CompassHeading.None == headingTmp)
                        continue;

                    ptsIntercepts[(int)headingTmp - 1] = GetBoundaryIntercept(rcsBounds, rcBounds, ptLocation, headingTmp);

                    n++;

                    if (n == 4)
                        break;
                }
            }
            else
            {
                ptIntercept = GetBoundaryIntercept(rcsBounds, rcBounds, ptLocation, heading);
                ptsIntercepts = new PointF[] { ptIntercept };
            }

            return ptsIntercepts;
        }
        #endregion

        /// <summary>
        /// Unions the specified rectangle.
        /// </summary>
        /// <param name="rect1">The first rectangle.</param>
        /// <param name="rect2">The second rectangle.</param>
        /// <returns>The rect.</returns>
        public static RectangleF Union(RectangleF rect1, RectangleF rect2)
        {
            float left;
            float top;
            float right;
            float bottom;

            if (rect1.Left < rect2.Left)
            {
                left = rect1.Left;
            }
            else
            {
                left = rect2.Left;
            }

            if (rect1.Right > rect2.Right)
            {
                right = rect1.Right;
            }
            else
            {
                right = rect2.Right;
            }

            if (rect1.Top < rect2.Top)
            {
                top = rect1.Top;
            }
            else
            {
                top = rect2.Top;
            }

            if (rect1.Bottom > rect2.Bottom)
            {
                bottom = rect1.Bottom;
            }
            else
            {
                bottom = rect2.Bottom;
            }

            return new RectangleF(left, top, right - left, bottom - top);
        }

        /// <summary>
        /// Unions the specified rectangle.
        /// </summary>
        /// <param name="rect1">The first rectangle.</param>
        /// <param name="rect2">The second rectangle.</param>
        /// <returns>The rect.</returns>
        public static System.Drawing.Rectangle Union(System.Drawing.Rectangle rect1, System.Drawing.Rectangle rect2)
        {
            int left;
            int top;
            int right;
            int bottom;

            if (rect1.Left < rect2.Left)
            {
                left = rect1.Left;
            }
            else
            {
                left = rect2.Left;
            }

            if (rect1.Right > rect2.Right)
            {
                right = rect1.Right;
            }
            else
            {
                right = rect2.Right;
            }

            if (rect1.Top < rect2.Top)
            {
                top = rect1.Top;
            }
            else
            {
                top = rect2.Top;
            }

            if (rect1.Bottom > rect2.Bottom)
            {
                bottom = rect1.Bottom;
            }
            else
            {
                bottom = rect2.Bottom;
            }

            return new System.Drawing.Rectangle(left, top, right - left, bottom - top);
        }

        /// <summary>
        /// Unions the specified rectangles.
        /// </summary>
        /// <param name="rects">The rectangle array.</param>
        /// <returns>The rect.</returns>
        public static System.Drawing.RectangleF Union(System.Drawing.RectangleF[] rects)
        {
            float left = float.MaxValue;
            float top = float.MaxValue;
            float right = float.MinValue;
            float bottom = float.MinValue;

            if (rects != null)
            {
                foreach (System.Drawing.RectangleF curRect in rects)
                {
                    if (curRect.Left < left)
                    {
                        left = curRect.Left;
                    }

                    if (curRect.Right > right)
                    {
                        right = curRect.Right;
                    }

                    if (curRect.Top < top)
                    {
                        top = curRect.Top;
                    }

                    if (curRect.Bottom > bottom)
                    {
                        bottom = curRect.Bottom;
                    }
                }
            }

            return new RectangleF(left, top, right - left, bottom - top);
        }

        /// <summary>
        /// Gets the aggregate bounds.
        /// </summary>
        /// <param name="nodes">The node collection.</param>
        /// <returns>The rect.</returns>
        public static System.Drawing.RectangleF GetAggregateBounds(NodeCollection nodes)
        {
            RectangleF rcBounds = new RectangleF(0, 0, 0, 0);
            bool firstNode = true;

            foreach (Node node in nodes)
            {
                if (firstNode)
                {
                    rcBounds = node.BoundingRectangle;
                    firstNode = false;
                }
                else
                {
                    rcBounds = Geometry.Union(rcBounds, node.BoundingRectangle);
                }
            }

            return rcBounds;
        }

        /// <summary>
        /// Calcs the length of the line.
        /// </summary>
        /// <param name="pts">The point array.</param>
        /// <returns>The line length</returns>
        public static double CalcLineLength(PointF[] pts)
        {
            if (pts == null)
            {
                throw new ArgumentNullException("pts", Resources.Strings.Messages.Get("ArgumentNull"));
            }

            double length = 0.0f;
            int vertexIdx1 = 0;
            int vertexIdx2 = 1;
            PointF vertex1;
            PointF vertex2;
            int numPts = pts.Length;

            while (vertexIdx2 < numPts)
            {
                vertex1 = pts[vertexIdx1];
                vertex2 = pts[vertexIdx2];
                length += Geometry.PointDistance(vertex1, vertex2);
                vertexIdx1++;
                vertexIdx2++;
            }

            return length;
        }

        /// <summary>
        /// Compasses the heading to vector.
        /// </summary>
        /// <param name="heading">The heading.</param>
        /// <returns>The size.</returns>
        public static Size CompassHeadingToVector(CompassHeading heading)
        {
            Size szVector = new Size(0, 0);

            switch (heading)
            {
                case CompassHeading.East:
                    szVector.Width = 1;
                    break;

                case CompassHeading.West:
                    szVector.Width = -1;
                    break;

                case CompassHeading.North:
                    szVector.Height = -1;
                    break;

                case CompassHeading.South:
                    szVector.Height = 1;
                    break;

                case CompassHeading.Northeast:
                    szVector.Height = -1;
                    szVector.Width = 1;
                    break;

                case CompassHeading.Northwest:
                    szVector.Height = -1;
                    szVector.Width = -1;
                    break;

                case CompassHeading.Southeast:
                    szVector.Height = 1;
                    szVector.Width = 1;
                    break;

                case CompassHeading.Southwest:
                    szVector.Height = 1;
                    szVector.Width = -1;
                    break;
            }

            return szVector;
        }

        /// <summary>
        /// Determines whether points array creates orthogonal line.
        /// </summary>
        /// <param name="pts">The points.</param>
        /// <returns>
        /// <c>true</c> if the points array creates orthogonal line; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsOrthogonalLine(PointF[] pts)
        {
            bool orthogonal = true;

            if (pts != null && pts.Length > 0)
            {
                PointF ptPrev = pts[0];
                PointF ptCur;

                for (int ptIdx = 1; orthogonal && ptIdx < pts.Length; ptIdx++)
                {
                    ptCur = pts[ptIdx];

                    if (ptCur.X != ptPrev.X && ptCur.Y != ptPrev.Y)
                    {
                        orthogonal = false;
                    }
                    else
                    {
                        ptPrev = ptCur;
                    }
                }
            }

            return orthogonal;
        }

        private static BoxPosition[,] orthogonalCtlPts = new BoxPosition[,]
        {
           { BoxPosition.TopLeft, BoxPosition.TopLeft, BoxPosition.TopCenter, BoxPosition.TopRight, BoxPosition.TopRight },
           { BoxPosition.TopLeft, BoxPosition.TopLeft, BoxPosition.TopCenter, BoxPosition.TopRight, BoxPosition.TopRight },
           { BoxPosition.MiddleLeft, BoxPosition.MiddleLeft, BoxPosition.Center, BoxPosition.MiddleRight, BoxPosition.MiddleRight },
           { BoxPosition.BottomLeft, BoxPosition.BottomLeft, BoxPosition.BottomCenter, BoxPosition.BottomRight, BoxPosition.BottomRight },
           { BoxPosition.BottomLeft, BoxPosition.BottomLeft, BoxPosition.BottomCenter, BoxPosition.BottomRight, BoxPosition.BottomRight }
        };

#if false
		/// <summary>
		/// Returns a point orthogonal to the two endpoints of the line.
		/// </summary>
		/// <param name="ptBegin"></param>
		/// <param name="ptEnd"></param>
		/// <param name="row">Row number of point to calculate.</param>
		/// <param name="col">Column number of point to calculate.</param>
		/// <param name="padLeft"></param>
		/// <param name="padRight"></param>
		/// <param name="padTop"></param>
		/// <param name="padBottom"></param>
		/// <returns>A logical point that is orthogonal to the two line endpoints.</returns>
		/// <remarks>
		/// The row and column are used to index a 5x5 matrix of points that
		/// surround the two endpoints. All of the points in the matrix are orthogonal
		/// to the endpoints. The matrix can be thought of as two rectangles and a
		/// point in the center. The outer rectangle contains 16 points and the inner
		/// rectangle contains 8 points (16+8+1=25). The endpoints always lie in
		/// either the 2nd or 4th column and the 2nd or 4th row. In other words, the
		/// endpoints are always two of the four corners of the inner rectangle. The
		/// outer rectangle is calculated by inflating the inner rectangle using the
		/// padding value passed in.		
		/// </remarks>
		public static PointF GetOrthogonalPoint(PointF ptBegin, PointF ptEnd, int row, int col, float padLeft, float padRight, float padTop, float padBottom)
		{
			PointF orthogonalPt = new PointF(0,0);

			RectangleF rcInner = Geometry.CreateRect(ptBegin, ptEnd);
			float outerLeft = rcInner.Left - padLeft;
			float outerTop = rcInner.Top - padTop;
			float outerWidth = rcInner.Width + padLeft + padRight;
			float outerHeight = rcInner.Height + padTop + padBottom;
			RectangleF rcOuter = new RectangleF(outerLeft, outerTop, outerWidth, outerHeight);
			RectangleF rcOuterHorz = new RectangleF(outerLeft, rcInner.Top, outerWidth, rcInner.Height);
			RectangleF rcOuterVert = new RectangleF(rcInner.Left, outerTop, rcInner.Width, outerHeight);
			BoxPosition ctlPt = Geometry.orthogonalCtlPts[row,col];

			if ((row < 1 || row > 3) && (col < 1 || col > 3))
			{
				// One of the outer corners.
				orthogonalPt = Geometry.GetAnchorPoint(rcOuter, ctlPt);
			}
			else if (row < 1 || row > 3)
			{
				// Top or bottom row.
				orthogonalPt = Geometry.GetAnchorPoint(rcOuterVert, ctlPt);
			}
			else if (col < 1 || col > 3)
			{
				// Left or right column.
				orthogonalPt = Geometry.GetAnchorPoint(rcOuterHorz, ctlPt);
			}
			else
			{
				// Inner rectangle.
				orthogonalPt = Geometry.GetAnchorPoint(rcInner, ctlPt);
			}

			return orthogonalPt;
		}
#endif

        /// <summary>
        /// Calculate the orthogonal points.
        /// </summary>
        /// <param name="ptEnd1">The tail end point position.</param>
        /// <param name="endPt1Heading">The tail end heading.</param>
        /// <param name="ptEnd2">The head end point position.</param>
        /// <param name="endPt2Heading">The head end point heading.</param>
        /// <param name="padLeft">The pad left.</param>
        /// <param name="padRight">The pad right.</param>
        /// <param name="padTop">The pad top.</param>
        /// <param name="padBottom">The pad bottom.</param>
        /// <returns>The points.</returns>
        public static PointF[] CalcOrthogonalPoints(PointF ptEnd1, CompassHeading endPt1Heading, PointF ptEnd2, CompassHeading endPt2Heading, float padLeft, float padRight, float padTop, float padBottom)
        {
            System.Collections.ArrayList ptsOut = new System.Collections.ArrayList();

            System.Drawing.Size frontHeadingVector = Geometry.CompassHeadingToVector(endPt1Heading);
            System.Drawing.Size backHeadingVector = Geometry.CompassHeadingToVector(endPt2Heading);

            // Maximum of 6 points from a 25x25 grid.
            int[] row = new int[] { 2, -1, -1, -1, -1, 2 };
            int[] col = new int[] { 2, -1, -1, -1, -1, 2 };
            int front = 0;
            int back = 5;

            // Determine the row and column in the grid
            // for each endpoint.
            if (ptEnd1.X < ptEnd2.X)
            {
                col[front] = 1;
                col[back] = 3;
            }
            else if (ptEnd1.X > ptEnd2.X)
            {
                col[front] = 3;
                col[back] = 1;
            }
            else
            {
                col[front] = col[front] + (1 * frontHeadingVector.Width);
                col[back] = col[back] + (1 * backHeadingVector.Width);
            }

            if (ptEnd1.Y < ptEnd2.Y)
            {
                row[front] = 1;
                row[back] = 3;
            }
            else if (ptEnd1.Y > ptEnd2.Y)
            {
                row[front] = 3;
                row[back] = 1;
            }
            else
            {
                row[front] = row[front] + (1 * frontHeadingVector.Height);
                row[back] = row[back] + (1 * backHeadingVector.Height);
            }

            row[front + 1] = row[front] + (1 * frontHeadingVector.Height);
            col[front + 1] = col[front] + (1 * frontHeadingVector.Width);
            front++;

            row[back - 1] = row[back] + (1 * backHeadingVector.Height);
            col[back - 1] = col[back] + (1 * backHeadingVector.Width);
            back--;

            bool hasMoved = true;
            bool isOrthogonal = (row[front] == row[back]) || (col[front] == col[back]);

            while (!isOrthogonal && front <= back)
            {
                // Determine if vectors will intersect.
                PointF ptFront0 = new PointF(col[front], row[front]);
                PointF ptFront1 = new PointF(col[front] + (10 * frontHeadingVector.Width), row[front] + (10 * frontHeadingVector.Height));
                PointF ptBack0 = new PointF(col[back], row[back]);
                PointF ptBack1 = new PointF(col[back] + (10 * backHeadingVector.Width), row[back] + (10 * backHeadingVector.Height));
                bool vectorsIntersect = Geometry.LinesIntersect(ptFront0, ptFront1, ptBack0, ptBack1);

                // Determine if vectors are pointed towards each other.
                SizeF szShift = frontHeadingVector + backHeadingVector;
                bool vectorsOpposite = (szShift.Width == 0 && szShift.Height == 0);
                bool vectorsSame = (frontHeadingVector == backHeadingVector);

                if (!hasMoved || (!vectorsIntersect && !vectorsSame))
                {
                    if (vectorsOpposite || vectorsSame)
                    {
                        int turnDirection = 1;  // 1 == counterclockwise, -1 = clockwise

                        // Change direction so that vectors are at right angles.
                        if (frontHeadingVector.Width > 0)
                        {
                            // Pointing right.
                            if (row[front] < row[back])
                            {
                                turnDirection = 1;
                            }
                            else
                            {
                                turnDirection = -1;
                            }
                        }
                        else if (frontHeadingVector.Width < 0)
                        {
                            // Pointing left.
                            if (row[front] < row[back])
                            {
                                turnDirection = -1;
                            }
                            else
                            {
                                turnDirection = 1;
                            }
                        }
                        else if (frontHeadingVector.Height > 0)
                        {
                            // Pointing down.
                            if (col[front] < col[back])
                            {
                                turnDirection = 1;
                            }
                            else
                            {
                                turnDirection = -1;
                            }
                        }
                        else if (frontHeadingVector.Height < 0)
                        {
                            // Pointing up.
                            if (col[front] < col[back])
                            {
                                turnDirection = -1;
                            }
                            else
                            {
                                turnDirection = 1;
                            }
                        }

                        System.Drawing.Size szTmp = frontHeadingVector;
                        frontHeadingVector.Width = szTmp.Height * turnDirection;
                        frontHeadingVector.Height = szTmp.Width * turnDirection;
                    }
                    else
                    {
                        frontHeadingVector = backHeadingVector;
                    }

                    // Go to next front point.
                    row[front + 1] = row[front];
                    col[front + 1] = col[front];
                    front++;
                }

                hasMoved = false;

                // Calculate next front grid point and test to see if
                // the line segments are orthogonal yet.
                int rowFront = row[front] + (1 * frontHeadingVector.Height);
                rowFront = rowFront < 4 ? rowFront : 4;
                rowFront = rowFront > 0 ? rowFront : 0;
                if (rowFront != row[front])
                {
                    hasMoved = true;
                    row[front] = rowFront;
                }

                int colFront = col[front] + (1 * frontHeadingVector.Width);
                colFront = colFront < 4 ? colFront : 4;
                colFront = colFront > 0 ? colFront : 0;
                if (colFront != col[front])
                {
                    hasMoved = true;
                    col[front] = colFront;
                }

                isOrthogonal = (row[front] == row[back]) || (col[front] == col[back]);
            }

            int numEmptySlots = back - front - 1;
            numEmptySlots = numEmptySlots > 0 ? numEmptySlots : 0;
            int numPoints = 6 - numEmptySlots;

            float innerLeft = (ptEnd1.X < ptEnd2.X) ? ptEnd1.X : ptEnd2.X;
            float innerTop = (ptEnd1.Y < ptEnd2.Y) ? ptEnd1.Y : ptEnd2.Y;
            float innerRight = (ptEnd1.X > ptEnd2.X) ? ptEnd1.X : ptEnd2.X;
            float innerBottom = (ptEnd1.Y > ptEnd2.Y) ? ptEnd1.Y : ptEnd2.Y;
            float[] innerAnchors = Geometry.GetBoxAnchors(innerLeft, innerTop, innerRight, innerBottom);

            float outerLeft = innerLeft - padLeft;
            float outerTop = innerTop - padTop;
            float outerRight = innerRight + padRight;
            float outerBottom = innerBottom + padBottom;
            float[] outerAnchors = Geometry.GetBoxAnchors(outerLeft, outerTop, outerRight, outerBottom);

            float outerHorzLeft = outerLeft;
            float outerHorzTop = innerTop;
            float outerHorzRight = outerRight;
            float outerHorzBottom = innerBottom;
            float[] outerHorzAnchors = Geometry.GetBoxAnchors(outerHorzLeft, outerHorzTop, outerHorzRight, outerHorzBottom);

            float outerVertLeft = innerLeft;
            float outerVertTop = outerTop;
            float outerVertRight = innerRight;
            float outerVertBottom = outerBottom;
            float[] outerVertAnchors = Geometry.GetBoxAnchors(outerVertLeft, outerVertTop, outerVertRight, outerVertBottom);

            // First point is always endpoint 1.
            ptsOut.Add(ptEnd1);

            // Compress array and elimate unused points.
            while ((front < back) && (back < 6))
            {
                if (row[front] == -1 || col[front] == -1)
                {
                    row[front] = row[back];
                    col[front] = col[back];
                    row[back] = -1;
                    col[back] = -1;
                    back++;
                }
                front++;
            }

            // Look at the other four points and see if they should
            // be added.
            for (int addIdx = 1; addIdx < numPoints - 1; addIdx++)
            {
                if ((row[addIdx - 1] != row[addIdx + 1]) && (col[addIdx - 1] != col[addIdx + 1]))
                {
                    // PointF ptAdd = Geometry.GetOrthogonalPoint(ptEnd1, ptEnd2, row[addIdx], col[addIdx], padLeft, padRight, padTop, padBottom);
                    PointF ptAdd;
                    int curRow = row[addIdx];
                    int curCol = col[addIdx];
                    BoxPosition ctlPt = Geometry.orthogonalCtlPts[curRow, curCol];

                    if ((curRow < 1 || curRow > 3) && (curCol < 1 || curCol > 3))
                    {
                        // One of the outer corners.
                        ptAdd = Geometry.GetAnchorPoint(outerAnchors, ctlPt);
                    }
                    else if (curRow < 1 || curRow > 3)
                    {
                        // Top or bottom row.
                        ptAdd = Geometry.GetAnchorPoint(outerVertAnchors, ctlPt);
                    }
                    else if (curCol < 1 || curCol > 3)
                    {
                        // Left or right column.
                        ptAdd = Geometry.GetAnchorPoint(outerHorzAnchors, ctlPt);
                    }
                    else
                    {
                        // Inner rectangle.
                        ptAdd = Geometry.GetAnchorPoint(innerAnchors, ctlPt);
                    }

                    bool found = false;
                    for (int ptIdx = 0; !found && ptIdx < ptsOut.Count; ptIdx++)
                    {
                        found = (ptAdd == (PointF)ptsOut[ptIdx]);
                    }

                    if (!found)
                    {
                        ptsOut.Add(ptAdd);
                    }
                }
            }

            // Last point is always endpoint 2.
            ptsOut.Add(ptEnd2);

            return (PointF[])ptsOut.ToArray(typeof(PointF));
        }

        /// <summary>
        /// Calculate the optimal directions for the two specified points.
        /// </summary>
        /// <param name="pt1">First point in the line.</param>
        /// <param name="pt2">Second point in the line.</param>
        /// <param name="pt1Heading">Heading calculated for the first point.</param>
        /// <param name="pt2Heading">Heading calculated for the second point.</param>
        /// <remarks>
        /// This method is used for orthogonal lines. Given two points, this method
        /// determines the compass headings for the two points. The compass heading
        /// for each point determines which direction the line will attach to the
        /// point.
        /// </remarks>
        public static void CalcEndpointDirections(PointF pt1, PointF pt2, out CompassHeading pt1Heading, out CompassHeading pt2Heading)
        {
            float horzDiff = pt2.X - pt1.X;
            float vertDiff = pt2.Y - pt1.Y;

            if (Math.Abs(horzDiff) > Math.Abs(vertDiff))
            {
                // Endpoints are further apart horizontally than vertically so make
                // the line segments attached to the endpoints horizontal.
                if (horzDiff > 0)
                {
                    pt1Heading = CompassHeading.East;
                    pt2Heading = CompassHeading.West;
                }
                else
                {
                    pt1Heading = CompassHeading.West;
                    pt2Heading = CompassHeading.East;
                }
            }
            else
            {
                // Endpoints are further apart vertically than horizontally so make
                // the line segments attached to the endpoints vertical.
                if (vertDiff > 0)
                {
                    pt1Heading = CompassHeading.South;
                    pt2Heading = CompassHeading.North;
                }
                else
                {
                    pt1Heading = CompassHeading.North;
                    pt2Heading = CompassHeading.South;
                }
            }
        }

        #region Class merge methods
        /// <summary>
        /// Merges the points in line.
        /// </summary>
        /// <param name="ptsPath">The array of PointF.</param>
        /// <param name="nIndex">Start index of the range.</param>
        /// <param name="nLength">Range length .</param>
        /// <returns>The points.</returns>
        public static PointF[] MergePointsInLine(PointF[] ptsPath, int nIndex, int nLength)
        {
            ArrayList pts = new ArrayList(ptsPath);
            pts.RemoveRange(nIndex, nLength);

            PointF[] ptsPoints = new PointF[nLength];
            Array.Copy(ptsPath, nIndex, ptsPoints, 0, nLength);
            ptsPath = MergePointsInLine(ptsPoints);

            pts.InsertRange(nIndex, ptsPath);

            return (PointF[])pts.ToArray(typeof(PointF));
        }

        /// <summary>
        /// Merges the points in line.
        /// </summary>
        /// <param name="pts">The array of PointF.</param>
        /// <returns>The points.</returns>
        public static PointF[] MergePointsInLine(PointF[] pts)
        {
            ArrayList lstPts = new ArrayList(pts);

            for (int i = 0, nLength = lstPts.Count - 2; i < nLength; i++)
            {
                if (CheckPointOnLine((PointF)lstPts[i], (PointF)lstPts[i + 2], 0))
                {
                    // update path points
                    lstPts.RemoveAt(i + 1);

                    // convert to pointF array
                    pts = (PointF[])lstPts.ToArray(typeof(PointF));

                    // call recursively itself
                    pts = MergePointsInLine(pts);
                    break;
                }
            }

            return pts;
        }

        /// <summary>
        /// Updates the path points of the polyline connector.
        /// </summary>
        /// <param name="pts">Path points of the polyline connector.</param>
        public static PointF[] UpdatePointsOfPolyLine(PointF[] pts)
        {
            ArrayList lstPts = new ArrayList(pts);

            for (int i = 0, nLength = lstPts.Count - 1; i < nLength; i++)
            {
                if ((PointF)lstPts[i] == (PointF)lstPts[i + 1])
                {
                    // update path points
                    lstPts.RemoveAt(i + 1);

                    // convert to pointF array
                    pts = (PointF[])lstPts.ToArray(typeof(PointF));

                    // call recursively itself
                    pts = UpdatePointsOfPolyLine(pts);
                    break;
                }
            }
            return pts;
        }

        /// <summary>
        /// Checks the point on line.
        /// </summary>
        /// <param name="ptStart">The first point.</param>
        /// <param name="ptEnd">The second point.</param>
        /// <param name="nDigits">The point number to be routed.</param>
        /// <returns>true, if point is on the line.</returns>
        public static bool CheckPointOnLine(PointF ptStart, PointF ptEnd, int nDigits)
        {
            Point ptStartPoint = new Point((int)Math.Round(ptStart.X, nDigits), (int)Math.Round(ptStart.Y, nDigits));
            Point ptEndtPoint = new Point((int)Math.Round(ptEnd.X, nDigits), (int)Math.Round(ptEnd.Y, nDigits));

            return (ptStartPoint.X == ptEndtPoint.X) || (ptStartPoint.Y == ptEndtPoint.Y) || (ptStartPoint == ptEndtPoint);
        }

        /// <summary>
        /// Merge the duplicate line segments.
        /// </summary>
        /// <param name="pts">The array of Point.</param>
        /// <param name="mergeThreshold">The merge threshold.</param>
        public static void MergeDuplicateLineSegments(ArrayList pts, int mergeThreshold)
        {
            if (pts == null)
            {
                throw new ArgumentNullException("pts");
            }

            if (pts.Count < 3)
            {
                return;
            }

            Point ptCur = (Point)pts[0];
            int startIdx = 0;
            int endIdx = startIdx;
            float curX = ptCur.X;
            int numDups = 0;
            bool segDone = false;

            for (int curIdx = 1; curIdx < pts.Count; curIdx++)
            {
                ptCur = (Point)pts[curIdx];
                if (Math.Abs(ptCur.X - curX) <= mergeThreshold)
                {
                    endIdx = curIdx;
                    if (curIdx == pts.Count - 1)
                    {
                        segDone = true;
                    }
                }
                else
                {
                    segDone = true;
                }

                if (segDone)
                {
                    numDups = endIdx - startIdx - 1;
                    if (numDups > 0)
                    {
                        Point ptStart = (Point)pts[startIdx];
                        Point ptEnd = (Point)pts[endIdx];

                        if (startIdx == 0)
                        {
                            ptEnd.X = ptStart.X;
                        }
                        else
                        {
                            ptStart.X = ptEnd.X;
                        }

                        pts[startIdx] = ptStart;
                        pts[endIdx] = ptEnd;

                        pts.RemoveRange(startIdx + 1, numDups);
                    }
                    curX = ptCur.X;
                    startIdx = curIdx;
                    endIdx = startIdx;
                    segDone = false;
                }
            }

            ptCur = (Point)pts[0];
            startIdx = 0;
            endIdx = startIdx;
            float curY = ptCur.Y;

            for (int curIdx = 1; curIdx < pts.Count; curIdx++)
            {
                ptCur = (Point)pts[curIdx];
                if (Math.Abs(ptCur.Y - curY) <= mergeThreshold)
                {
                    endIdx = curIdx;
                    if (curIdx == pts.Count - 1)
                    {
                        segDone = true;
                    }
                }
                else
                {
                    segDone = true;
                }

                if (segDone)
                {
                    numDups = endIdx - startIdx - 1;
                    if (numDups > 0)
                    {
                        Point ptStart = (Point)pts[startIdx];
                        Point ptEnd = (Point)pts[endIdx];

                        if (startIdx == 0)
                        {
                            ptEnd.Y = ptStart.Y;
                        }
                        else
                        {
                            ptStart.Y = ptEnd.Y;
                        }

                        pts[startIdx] = ptStart;
                        pts[endIdx] = ptEnd;

                        pts.RemoveRange(startIdx + 1, numDups);
                    }
                    curY = ptCur.Y;
                    startIdx = curIdx;
                    endIdx = startIdx;
                    segDone = false;
                }
            }
        }

        /// <summary>
        /// Merge the duplicate line segments.
        /// </summary>
        /// <param name="pts">The array of PointF.</param>
        /// <param name="mergeThreshold">The merge threshold.</param>
        public static void MergeDuplicateLineSegmentsF(ArrayList pts, float mergeThreshold)
        {
            if (pts == null)
            {
                throw new ArgumentNullException("pts");
            }

            if (pts.Count < 3)
            {
                return;
            }

            PointF ptCur = (PointF)pts[0];
            int startIdx = 0;
            int endIdx = startIdx;
            float curX = ptCur.X;
            bool segDone = false;

            for (int curIdx = 1; curIdx < pts.Count; curIdx++)
            {
                ptCur = (PointF)pts[curIdx];
                if (Math.Abs(ptCur.X - curX) <= mergeThreshold)
                {
                    endIdx = curIdx;
                    if (curIdx == pts.Count - 1)
                    {
                        segDone = true;
                    }
                }
                else
                {
                    segDone = true;
                }

                if (segDone)
                {
                    int numDups = endIdx - startIdx - 1;
                    if (numDups > 0)
                    {
                        PointF ptStart = (PointF)pts[startIdx];
                        PointF ptEnd = (PointF)pts[endIdx];

                        if (startIdx == 0)
                        {
                            ptEnd.X = ptStart.X;
                        }
                        else
                        {
                            ptStart.X = ptEnd.X;
                        }

                        pts[startIdx] = ptStart;
                        pts[endIdx] = ptEnd;

                        pts.RemoveRange(startIdx + 1, numDups);
                    }
                    curX = ptCur.X;
                    startIdx = curIdx;
                    endIdx = startIdx;
                    segDone = false;
                }
            }

            ptCur = (PointF)pts[0];
            startIdx = 0;
            endIdx = startIdx;
            float curY = ptCur.Y;

            for (int curIdx = 1; curIdx < pts.Count; curIdx++)
            {
                ptCur = (PointF)pts[curIdx];
                if (Math.Abs(ptCur.Y - curY) <= mergeThreshold)
                {
                    endIdx = curIdx;
                    if (curIdx == pts.Count - 1)
                    {
                        segDone = true;
                    }
                }
                else
                {
                    segDone = true;
                }

                if (segDone)
                {
                    int numDups = endIdx - startIdx - 1;
                    if (numDups > 0)
                    {
                        PointF ptStart = (PointF)pts[startIdx];
                        PointF ptEnd = (PointF)pts[endIdx];

                        if (startIdx == 0)
                        {
                            ptEnd.Y = ptStart.Y;
                        }
                        else
                        {
                            ptStart.Y = ptEnd.Y;
                        }

                        pts[startIdx] = ptStart;
                        pts[endIdx] = ptEnd;

                        pts.RemoveRange(startIdx + 1, numDups);
                    }
                    curY = ptCur.Y;
                    startIdx = curIdx;
                    endIdx = startIdx;
                    segDone = false;
                }
            }
        }

        #endregion

        /// <summary>
        /// Moves the line segment.
        /// </summary>
        /// <param name="pts">The points.</param>
        /// <param name="segIdx">The segment index.</param>
        /// <param name="distance">The distance.</param>
        /// <param name="mergeThreshold">The merge threshold.</param>
        /// <returns>The line segments.</returns>
        public static PointF[] MoveLineSegment(PointF[] pts, int segIdx, float distance, float mergeThreshold)
        {
            if (pts == null)
            {
                throw new ArgumentNullException("pts");
            }

            if (segIdx < 0 || segIdx >= (pts.Length - 1))
            {
                throw new ArgumentException("Segment index is out of range", "segIdx");
            }

            ArrayList ptsOut = new ArrayList();
            int actualSegIdx = segIdx;
            bool isVertical = (pts[segIdx].X == pts[segIdx + 1].X);

            if (pts.Length == 2)
            {
                // Split the segment
                ptsOut.Add(pts[0]);

                if (isVertical)
                {
                    // Segment is vertical
                    float lineLen = pts[1].Y - pts[0].Y;
                    ptsOut.Add(new PointF(pts[0].X, pts[0].Y + (lineLen / 3.0f)));
                    ptsOut.Add(new PointF(pts[0].X + distance, pts[0].Y + (lineLen / 3.0f)));
                    ptsOut.Add(new PointF(pts[0].X + distance, pts[0].Y + ((lineLen * 2.0f) / 3.0f)));
                    ptsOut.Add(new PointF(pts[0].X, pts[0].Y + ((lineLen * 2.0f) / 3.0f)));
                }
                else
                {
                    // Segment is horizontal
                    float lineLen = pts[1].X - pts[0].X;
                    ptsOut.Add(new PointF(pts[0].X + (lineLen / 3.0f), pts[0].Y));
                    ptsOut.Add(new PointF(pts[0].X + (lineLen / 3.0f), pts[0].Y + distance));
                    ptsOut.Add(new PointF(pts[0].X + ((lineLen * 2.0f) / 3.0f), pts[0].Y + distance));
                    ptsOut.Add(new PointF(pts[0].X + ((lineLen * 2.0f) / 3.0f), pts[0].Y));
                }

                ptsOut.Add(pts[1]);
            }
            else
            {
                if (segIdx == 0)
                {
                    // Split the segment
                    actualSegIdx = 2;
                    ptsOut.Add(pts[0]);

                    if (isVertical)
                    {
                        // Segment is vertical
                        ptsOut.Add(new PointF(pts[0].X, (pts[0].Y + pts[1].Y) / 2.0f));
                    }
                    else
                    {
                        // Segment is horizontal
                        ptsOut.Add(new PointF((pts[0].X + pts[1].X) / 2.0f, pts[0].Y));
                    }

                    ptsOut.Add(ptsOut[ptsOut.Count - 1]);

                    for (int ptIdx = 1; ptIdx < pts.Length; ptIdx++)
                    {
                        ptsOut.Add(pts[ptIdx]);
                    }
                }
                else if (segIdx == (pts.Length - 2))
                {
                    // Split the segment
                    actualSegIdx = segIdx;

                    for (int ptIdx = 0; ptIdx < pts.Length - 1; ptIdx++)
                    {
                        ptsOut.Add(pts[ptIdx]);
                    }

                    if (isVertical)
                    {
                        // Segment is vertical
                        ptsOut.Add(new PointF(pts[pts.Length - 1].X, (pts[pts.Length - 2].Y + pts[pts.Length - 1].Y) / 2.0f));
                    }
                    else
                    {
                        // Segment is horizontal
                        ptsOut.Add(new PointF((pts[pts.Length - 2].X + pts[pts.Length - 1].X) / 2.0f, pts[pts.Length - 2].Y));
                    }

                    ptsOut.Add(ptsOut[ptsOut.Count - 1]);
                    ptsOut.Add(pts[pts.Length - 1]);
                }
                else
                {
                    ptsOut.AddRange(pts);
                }

                if (isVertical)
                {
                    // Vertical
                    PointF curPt = (PointF)ptsOut[actualSegIdx];
                    curPt.X = curPt.X + distance;
                    ptsOut[actualSegIdx] = curPt;
                    curPt = (PointF)ptsOut[actualSegIdx + 1];
                    curPt.X = curPt.X + distance;
                    ptsOut[actualSegIdx + 1] = curPt;
                }
                else
                {
                    // Horizontal
                    PointF curPt = (PointF)ptsOut[actualSegIdx];
                    curPt.Y = curPt.Y + distance;
                    ptsOut[actualSegIdx] = curPt;
                    curPt = (PointF)ptsOut[actualSegIdx + 1];
                    curPt.Y = curPt.Y + distance;
                    ptsOut[actualSegIdx + 1] = curPt;
                }

                MergeDuplicateLineSegmentsF(ptsOut, mergeThreshold);
            }

            return (PointF[])ptsOut.ToArray(typeof(PointF));
        }

        /// <summary>
        /// Moves the line segment.
        /// </summary>
        /// <param name="pts">The point array.</param>
        /// <param name="segIdx">The segment idx.</param>
        /// <param name="distance">The distance.</param>
        /// <returns>The line segments.</returns>
        public static Point[] MoveLineSegment(Point[] pts, int segIdx, int distance)
        {
            if (pts == null)
            {
                throw new ArgumentNullException("pts");
            }

            if (segIdx < 0 || segIdx >= (pts.Length - 1))
            {
                throw new ArgumentException("Segment index is out of range", "segIdx");
            }

            ArrayList ptsOut = new ArrayList();
            int actualSegIdx = segIdx;
            bool isVertical = (pts[segIdx].X == pts[segIdx + 1].X);

            if (pts.Length <= 2)
            {
                // Split the segment
                ptsOut.Add(pts[0]);

                if (isVertical)
                {
                    // Segment is vertical
#if false
					ptsOut.Add(new Point(pts[0].X, (pts[0].Y + pts[1].Y) / 3));
					ptsOut.Add(new Point(pts[0].X + distance, (pts[0].Y + pts[1].Y) / 3));
					ptsOut.Add(new Point(pts[0].X + distance, ((pts[0].Y + pts[1].Y) * 2) / 3));
					ptsOut.Add(new Point(pts[0].X, ((pts[0].Y + pts[1].Y) * 2) / 3));
#else
                    int lineLen = pts[1].Y - pts[0].Y;
                    ptsOut.Add(new Point(pts[0].X, pts[0].Y + (lineLen / 3)));
                    ptsOut.Add(new Point(pts[0].X + distance, pts[0].Y + (lineLen / 3)));
                    ptsOut.Add(new Point(pts[0].X + distance, pts[0].Y + ((lineLen * 2) / 3)));
                    ptsOut.Add(new Point(pts[0].X, pts[0].Y + ((lineLen * 2) / 3)));
#endif
                }
                else
                {
                    // Segment is horizontal
                    int lineLen = pts[1].X - pts[0].X;
                    ptsOut.Add(new Point(pts[0].X + (lineLen / 3), pts[0].Y));
                    ptsOut.Add(new Point(pts[0].X + (lineLen / 3), pts[0].Y + distance));
                    ptsOut.Add(new Point(pts[0].X + ((lineLen * 2) / 3), pts[0].Y + distance));
                    ptsOut.Add(new Point(pts[0].X + ((lineLen * 2) / 3), pts[0].Y));
                }

                ptsOut.Add(pts[0]);
            }
            else
            {
                if (segIdx == 0)
                {
                    // Split the segment
                    actualSegIdx = 2;
                    ptsOut.Add(pts[0]);

                    if (isVertical)
                    {
                        // Segment is vertical
                        ptsOut.Add(new Point(pts[0].X, (pts[0].Y + pts[1].Y) / 2));
                    }
                    else
                    {
                        // Segment is horizontal
                        ptsOut.Add(new Point((pts[0].X + pts[1].X) / 2, pts[0].Y));
                    }

                    ptsOut.Add(ptsOut[ptsOut.Count - 1]);

                    for (int ptIdx = 1; ptIdx < pts.Length; ptIdx++)
                    {
                        ptsOut.Add(pts[ptIdx]);
                    }
                }
                else if (segIdx == (pts.Length - 2))
                {
                    // Split the segment
                    actualSegIdx = segIdx;

                    for (int ptIdx = 0; ptIdx < pts.Length - 1; ptIdx++)
                    {
                        ptsOut.Add(pts[ptIdx]);
                    }

                    if (isVertical)
                    {
                        // Segment is vertical
                        ptsOut.Add(new Point(pts[pts.Length - 1].X, (pts[pts.Length - 2].Y + pts[pts.Length - 1].Y) / 2));
                    }
                    else
                    {
                        // Segment is horizontal
                        ptsOut.Add(new Point((pts[pts.Length - 2].X + pts[pts.Length - 1].X) / 2, pts[pts.Length - 2].Y));
                    }

                    ptsOut.Add(ptsOut[ptsOut.Count - 1]);
                    ptsOut.Add(pts[pts.Length - 1]);
                }
                else
                {
                    ptsOut.AddRange(pts);
                }

                if (isVertical)
                {
                    // Vertical
                    Point curPt = (Point)ptsOut[actualSegIdx];
                    curPt.X = curPt.X + distance;
                    ptsOut[actualSegIdx] = curPt;
                    curPt = (Point)ptsOut[actualSegIdx + 1];
                    curPt.X = curPt.X + distance;
                    ptsOut[actualSegIdx + 1] = curPt;
                }
                else
                {
                    // Horizontal
                    Point curPt = (Point)ptsOut[actualSegIdx];
                    curPt.Y = curPt.Y + distance;
                    ptsOut[actualSegIdx] = curPt;
                    curPt = (Point)ptsOut[actualSegIdx + 1];
                    curPt.Y = curPt.Y + distance;
                    ptsOut[actualSegIdx + 1] = curPt;
                }

                MergeDuplicateLineSegments(ptsOut, 2);
            }

            return (Point[])ptsOut.ToArray(typeof(Point));
        }

        /// <summary>
        /// Updates given rect with another.
        /// </summary>
        /// <param name="rcUpdating">Updating rectangle.</param>
        /// <param name="rcToUpdateWith">Rectangle to update with.</param>
        /// <returns>The rect</returns>
        /// <remarks>
        /// If updating rectangle is empty it is replaced with rectangle to update with.
        /// </remarks>
        public static System.Drawing.Rectangle UpdateRectWith(System.Drawing.Rectangle rcUpdating, System.Drawing.Rectangle rcToUpdateWith)
        {
            if (rcUpdating.Size.IsEmpty)
            {
                rcUpdating = rcToUpdateWith;
            }
            else if (!rcUpdating.Size.IsEmpty)
            {
                rcUpdating = System.Drawing.Rectangle.Union(rcUpdating, rcToUpdateWith);
            }

            return rcUpdating;
        }

        /// <summary>
        /// Updates given rect with another.
        /// </summary>
        /// <param name="rcUpdating">Updating rectangle.</param>
        /// <param name="rcToUpdateWith">Rectangle to update with.</param>
        /// <returns>The rect.</returns>
        /// <remarks>
        /// If updating rectangle is empty it is replaced with rectangle to update with.
        /// </remarks>
        public static RectangleF UpdateRectWith(RectangleF rcUpdating, RectangleF rcToUpdateWith)
        {
            if (rcUpdating.Size.IsEmpty)
            {
                rcUpdating = rcToUpdateWith;
            }
            else if (!rcUpdating.Size.IsEmpty)
            {
                rcUpdating = RectangleF.Union(rcUpdating, rcToUpdateWith);
            }

            return rcUpdating;
        }

        /// <summary>
        /// Checks whether point to test lies on line
        /// formed with start and end points.
        /// </summary>
        /// <param name="ptStart">Start point.</param>
        /// <param name="ptEnd">End point.</param>
        /// <param name="ptTest">Point to test.</param>
        /// <returns>true, if point is on the line.</returns>
        /// <remarks>Used with orthogonal lines
        /// to reduce the overhead of graphics path and region creation.</remarks>
        public static bool CheckOrthogonalLine(PointF ptStart, PointF ptEnd, PointF ptTest)
        {
            Point ptStartPoint = new Point((int)Math.Round(ptStart.X), (int)Math.Round(ptStart.Y));
            Point ptEndtPoint = new Point((int)Math.Round(ptEnd.X), (int)Math.Round(ptEnd.Y));

            return (ptStartPoint.X == ptTest.X && ptTest.X == ptEndtPoint.X)
                   || (ptStartPoint.Y == ptTest.Y && ptTest.Y == ptEndtPoint.Y);
        }

        /// <summary>
        /// Checks whether point to test lies on line 
        /// formed with start and end points.
        /// </summary>
        /// <param name="ptStart">Start point.</param>
        /// <param name="ptEnd">End point.</param>
        /// <param name="ptTest">Point to test.</param>
        /// <returns>The line.</returns>
        /// <remarks>Used with orthogonal lines
        /// to reduce the overhead of graphics path and region creation.</remarks>
        public static bool CheckOrthogonalLine(Point ptStart, Point ptEnd, Point ptTest)
        {
            return (ptStart.X == ptTest.X && ptTest.X == ptEnd.X)
                || (ptStart.Y == ptTest.Y && ptTest.Y == ptEnd.Y);
        }

        /// <summary>
        /// Gets the endPoint index in given path points.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="nPathLength">Length of the points path in PathNode.</param>
        /// <param name="nDepthOffset">The offset to inside points range..</param>
        /// <returns>The end point index.</returns>
        public static int GetEndPointIndex(EndPoint endPoint, int nPathLength, int nDepthOffset)
        {
            int nIndex = -1;
            HeadEndPoint headPoint = endPoint as HeadEndPoint;
            TailEndPoint tailPoint = endPoint as TailEndPoint;

            if (headPoint != null)
            {
                nIndex = nPathLength - nDepthOffset - 1;
            }
            else if (tailPoint != null)
            {
                nIndex = nDepthOffset;
            }

            return nIndex;
        }

        /// <summary>
        /// Get point projection to line.
        /// </summary>
        /// <param name="ptPoint">Point to projection.</param>
        /// <param name="ptLineStart">Line start point.</param>
        /// <param name="ptLineEnd">Line end point.</param>
        /// <returns>Projection point that place in line.</returns>
        public static PointF GetProjection(PointF ptPoint, PointF ptLineStart, PointF ptLineEnd)
        {
            double dAngle = LineAngle(ptPoint, ptLineStart, ptLineEnd);
            double dAngleRadian = LineAngle(ptLineStart, ptLineEnd);
            double dDistance = PointDistance(ptPoint, ptLineStart) * Math.Cos(dAngle);

            double fX = ptLineStart.X + Math.Cos(dAngleRadian) * dDistance;
            double fY = ptLineStart.Y + Math.Sin(dAngleRadian) * dDistance;

            return new PointF((float)fX, (float)fY);
        }

        #region Class converted methods
        /// <summary>
        /// Translates points to grid origin (0,0)
        /// </summary>
        /// <param name="pts">points to move.</param>
        public static void TranslateToGridOrigin(PointF[] pts)
        {
            RectangleF rcBounds = CreateRect(pts);
            TranslateToOrigin(pts, rcBounds.Location);
        }

        /// <summary>
        /// Translates points to given origin.
        /// </summary>
        /// <param name="pts">Points to move.</param>
        /// <param name="ptOrigin">Origin to move points to.</param>
        public static void TranslateToOrigin(PointF[] pts, PointF ptOrigin)
        {
            // Transform points to positive position.
            for (int i = 0, nLenght = pts.Length; i < nLenght; i++)
            {
                pts[i].X -= ptOrigin.X;
                pts[i].Y -= ptOrigin.Y;
            }
        }

        /// <summary>
        /// Convert the <c>System.Drawing.Point</c> to <c>System.Drawing.PointF</c>
        /// </summary>
        /// <param name="ptPoint">The given point.</param>
        /// <returns>The point.</returns>
        public static Point ConvertPoint(PointF ptPoint)
        {
            return new Point((int)Math.Ceiling(ptPoint.X), (int)Math.Ceiling(ptPoint.Y));
        }

        /// <summary>
        /// Converts the points.
        /// </summary>
        /// <param name="pts">The point array.</param>
        /// <returns>The converted points.</returns>
        public static PointF[] ConvertPoints(Point[] pts)
        {
            int nLenght = pts.Length;
            PointF[] ptsToReturn = new PointF[nLenght];

            for (int i = 0; i < nLenght; i++)
            {
                ptsToReturn[i] = ConvertPoint(pts[i]);
            }

            return ptsToReturn;
        }

        /// <summary>
        /// Converts the points.
        /// </summary>
        /// <param name="pts">The point array.</param>
        /// <returns>The converted points.</returns>
        public static Point[] ConvertPoints(PointF[] pts)
        {
            int nLenght = pts.Length;
            Point[] ptsToReturn = new Point[nLenght];

            for (int i = 0; i < nLenght; i++)
            {
                ptsToReturn[i] = ConvertPoint(pts[i]);
            }

            return ptsToReturn;
        }

        /// <summary>
        /// Converts the size.
        /// </summary>
        /// <param name="szSize">The size.</param>
        /// <returns>The converted size.</returns>
        public static Size ConvertSize(SizeF szSize)
        {
            return new Size((int)Math.Ceiling(szSize.Width), (int)Math.Ceiling(szSize.Height));
        }

        /// <summary>
        /// Converts the rectangle.
        /// </summary>
        /// <param name="rcRect">The rectangle.</param>
        /// <returns>The converted rect.</returns>
        public static System.Drawing.Rectangle ConvertRectangle(RectangleF rcRect)
        {
            return new System.Drawing.Rectangle(ConvertPoint(rcRect.Location), ConvertSize(rcRect.Size));
        }
        #endregion

        #region Class matrix methods
        /// <summary>
        /// Appends the matrix to rectangle.
        /// </summary>
        /// <param name="rcRect">The rectangle.</param>
        /// <param name="mtxTransform">The matrix transform.</param>
        /// <returns>The rect.</returns>
        public static RectangleF AppendMatrix(RectangleF rcRect, Matrix mtxTransform)
        {
            PointF[] pts = new PointF[4]
            {
                rcRect.Location,
                new PointF( rcRect.Right, rcRect.Top ),
                new PointF( rcRect.Left, rcRect.Bottom ),
                new PointF( rcRect.Right, rcRect.Bottom )
            };

            if (mtxTransform != null)
            {
                mtxTransform.TransformPoints(pts);
            }

            return CreateRect(pts);
        }

        /// <summary>
        /// Appends the matrix to point.
        /// </summary>
        /// <param name="ptPoint">The point.</param>
        /// <param name="mtxTransform">The matrix transform.</param>
        /// <returns>The matrix point.</returns>
        public static PointF AppendMatrix(PointF ptPoint, Matrix mtxTransform)
        {
            PointF[] pts = new PointF[1] { ptPoint };

            if (mtxTransform != null)
            {
                mtxTransform.TransformPoints(pts);
            }

            return pts[0];
        }
        #endregion

        #region Class equal methods
        /// <summary>
        /// Equals the two points.
        /// </summary>
        /// <param name="ptFirst">The first point.</param>
        /// <param name="ptSecond">The second point.</param>
        /// <param name="nDigits">The digits count to be rounded.</param>
        /// <returns>true, if points are equal.</returns>
        public static bool EqualPoints(PointF ptFirst, PointF ptSecond, int nDigits)
        {
            return (Math.Round(ptFirst.X, nDigits) == Math.Round(ptSecond.X, nDigits))
                && (Math.Round(ptFirst.Y, nDigits) == Math.Round(ptSecond.Y, nDigits));
        }

        #endregion

        #region Class routing methods
        /// <summary>
        /// Route the head end point to current heading.
        /// </summary>
        /// <param name="ptsPath">The last path points.</param>
        /// <param name="endPoint">The end point.</param>
        /// <param name="fRouteDistance">The route distance.</param>
        /// <returns>true, if head end point is routed to current heading.</returns>
        public static bool RouteEndPointToHeading(ref PointF[] ptsPath, EndPoint endPoint, float fRouteDistance)
        {
            bool bSuccess = false;

            // route head end point
            if (endPoint.Port != null)
            {
                ConnectionPoint port = endPoint.Port;
                CompassHeading portHeading = endPoint.GetHeading();

                // route only horizontal and vertical direction
                if ((int)portHeading > 0 && (int)portHeading < 5)
                {
                    ArrayList pathPoints = new ArrayList(ptsPath);
                    int nCount = pathPoints.Count;
                    int nIndex = GetEndPointIndex(endPoint, nCount, 0);
                    int nStartIndex = GetEndPointIndex(endPoint, nCount, 1);

                    PointF ptStartPoint = (PointF)pathPoints[nStartIndex];
                    PointF ptPort = (PointF)pathPoints[nIndex];

                    // get route bounds
                    RectangleF rcBounds = GetRouteBounds(port);

                    // get route path 
                    PointF[] ptsRoute = RouteOrthogonal(rcBounds, ptStartPoint, ptPort, portHeading, fRouteDistance);
                    pathPoints.RemoveAt(nIndex);

                    // insert route ogtogonal path
                    if (nIndex == pathPoints.Count)
                    {
                        pathPoints.AddRange(ptsRoute);
                    }
                    else
                    {
                        Array.Reverse(ptsRoute);
                        pathPoints.InsertRange(nIndex, ptsRoute);
                    }

                    // transform path to node coordinates
                    ptsPath = (PointF[])pathPoints.ToArray(typeof(PointF));
                    bSuccess = true;
                }
            }

            return bSuccess;
        }

        /// <summary>
        /// Gets the boundary point by compass heading checking only horizontal and vertical direction.
        /// </summary>
        /// <param name="rcBounds">The bounds to intersect.</param>
        /// <param name="ptInnerPoint">The point inside bounds.</param>
        /// <param name="heading">The intersect direction.</param>
        /// <returns>Boundary side point.</returns>
        public static PointF GetBoundaryPoint(RectangleF rcBounds, PointF ptInnerPoint, CompassHeading heading)
        {
            PointF ptReturn = ptInnerPoint;

            switch (heading)
            {
                case CompassHeading.North:
                    ptReturn = new PointF(ptInnerPoint.X, rcBounds.Y);
                    break;
                case CompassHeading.South:
                    ptReturn = new PointF(ptInnerPoint.X, rcBounds.Bottom);
                    break;
                case CompassHeading.East:
                    ptReturn = new PointF(rcBounds.Right, ptInnerPoint.Y);
                    break;
                case CompassHeading.West:
                    ptReturn = new PointF(rcBounds.X, ptInnerPoint.Y);
                    break;
            }

            return ptReturn;
        }

        /// <summary>
        /// Gets the route container bounds by connection point.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <returns>Route bounds.</returns>
        public static RectangleF GetRouteBounds(ConnectionPoint port)
        {
            float fDistance = CommonUsedValues.DEF_ROUTE_DISTANCE;
            RectangleF rcBounds;

            // get current port position in model coordinates
            Matrix mtxTransform;

            // get port container bounds 
            // and convert to model coordinates
            if (port is CentralPort)
            {
                GraphicsPath path = port.Container.GraphicsPath;
                mtxTransform = HandlesHitTesting.GetParentsTransformations(port.Container, true);
                rcBounds = path.GetBounds(mtxTransform);
            }
            else
            {
                mtxTransform = HandlesHitTesting.GetParentsTransformations(port.Container, true);
                PointF[] ptsPort = new PointF[] { port.GetPosition() };
                mtxTransform.TransformPoints(ptsPort);

                // create port around bounds
                rcBounds = new RectangleF(ptsPort[0].X - fDistance / 2, ptsPort[0].Y - fDistance / 2, fDistance, fDistance);
            }

            // return finded bounds
            return rcBounds;
        }

        /// <summary>
        /// Route the orthogonal from given outsidePoint to port.
        /// </summary>
        /// <param name="rcBounds">The route bounds.</param>
        /// <param name="ptOutside">The end point position.</param>
        /// <param name="ptPortPosition">The port position.</param>
        /// <param name="outHeading">The out heading.</param>
        /// <param name="fDistance">The route distance.</param>
        /// <returns>
        /// Orthogonal path routed from outside point to given port.
        /// </returns>
        private static PointF[] RouteOrthogonal(RectangleF rcBounds, PointF ptOutside, PointF ptPortPosition, CompassHeading outHeading, float fDistance)
        {
            // create path list
            ArrayList lstPath = new ArrayList();
            CompassHeading inHeading = GetCompassHeading(new RectangleF(ptPortPosition, SizeF.Empty), ptOutside);
            PointF ptEnd = ptPortPosition;
            PointF ptFirst = ptOutside;

            // find outside points
            rcBounds.Inflate(fDistance, fDistance);

            if (inHeading != outHeading)
            {
                ptOutside = GetBoundaryPoint(rcBounds, ptOutside, inHeading);
                ptPortPosition = GetBoundaryPoint(rcBounds, ptPortPosition, outHeading);

                // add start outside point
                lstPath.Add(ptOutside);

                // add part of route path
                int nStart = GetRectSegmentIndex(inHeading);
                int nEnd = GetRectSegmentIndex(outHeading);
                lstPath.AddRange(RouteAroudRect(rcBounds, nStart, nEnd));
            }
            else
            {
                ptFirst = GetBoundaryPoint(rcBounds, ptPortPosition, outHeading);
                ptFirst = GetBoundaryPoint(rcBounds, ptFirst, outHeading);
                lstPath.Add(ptFirst);
                lstPath.Add(ptFirst);
            }

            // add last point
            lstPath.Add(ptPortPosition);

            if (inHeading != outHeading)
                lstPath.Add(ptEnd);

            return (PointF[])lstPath.ToArray(typeof(PointF));
        }

        /// <summary>
        /// Gets the compass heading.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <returns>Compass heading.</returns>
        public static CompassHeading GetCompassHeading(EndPoint endPoint)
        {
            CompassHeading heading = CompassHeading.None;

            if (endPoint.Container != null && endPoint.Port != null && endPoint.Port.Container != null)
            {
                PointF[] ptsPath = endPoint.Container.GetPoints();
                int nIndex = GetEndPointIndex(endPoint, ptsPath.Length, 1);
                PointF ptControlPoint = ptsPath[nIndex];

                Matrix mtxNode = endPoint.Container.GetTransformations();
                endPoint.Container.AppendFlipTransforms(mtxNode);
                ptControlPoint = AppendMatrix(ptControlPoint, mtxNode);

                heading = GetCompassHeading(new RectangleF(endPoint.Location, SizeF.Empty), ptControlPoint);
            }

            return heading;
        }

        /// <summary>
        /// Gets the compass heading direction.
        /// Return only North, West, South, East or None value.
        /// </summary>
        /// <param name="rcBounds">The round area.</param>
        /// <param name="ptPoint">The outside point.</param>
        /// <returns>Heading direction.</returns>
        public static CompassHeading GetCompassHeading(RectangleF rcBounds, PointF ptPoint)
        {
            float fTop = ptPoint.Y - rcBounds.Y;
            float fBottom = rcBounds.Bottom - ptPoint.Y;
            float fLeft = ptPoint.X - rcBounds.X;
            float fRight = rcBounds.Right - ptPoint.X;

            // find inHeading direction
            CompassHeading heading = CompassHeading.None;

            // bottom
            if (fBottom <= fTop && fBottom <= fLeft && fBottom <= fRight)
                heading = CompassHeading.South;

            // left
            else if (fLeft <= fTop && fLeft <= fBottom && fLeft <= fRight)
                heading = CompassHeading.West;

            // top
            else if (fTop <= fLeft && fTop <= fBottom && fTop <= fRight)
                heading = CompassHeading.North;

            // right
            else if (fRight <= fLeft && fRight <= fBottom && fRight <= fTop)
                heading = CompassHeading.East;

            return heading;
        }

        /// <summary>
        /// Gets rectangle segment index by compass heading.
        /// </summary>
        /// <param name="heading">The compass heading.</param>
        /// <returns>The rect segment index.</returns>
        private static int GetRectSegmentIndex(CompassHeading heading)
        {
            // by default CompassHeading.North
            int nReturn = 0;

            if (heading == CompassHeading.East)
                nReturn = 1;
            else if (heading == CompassHeading.South)
                nReturn = 2;
            else if (heading == CompassHeading.West)
                nReturn = 3;

            return nReturn;
        }

        /// <summary>
        /// Route orthogonal line the aroud rectangle from start to end segment.
        /// </summary>
        /// <param name="rcBounds">The rectangle.</param>
        /// <param name="nStart">Index of the start side.</param>
        /// <param name="nEnd">Index of the end side.</param>
        /// <returns>The points.</returns>
        private static PointF[] RouteAroudRect(RectangleF rcBounds, int nStart, int nEnd)
        {
            ArrayList lstPoints = new ArrayList();
            ArrayList ptsPath = new ArrayList();
            ptsPath.Add(rcBounds.Location);
            ptsPath.Add(new PointF(rcBounds.Right, rcBounds.Y));
            ptsPath.Add(new PointF(rcBounds.Right, rcBounds.Bottom));
            ptsPath.Add(new PointF(rcBounds.X, rcBounds.Bottom));

            int nLength = ptsPath.Count;
            int nIncrease = 1;

            if (((nEnd - nStart) < 0 && (nEnd - nStart) > -3) || (nStart == 0 && nEnd == 3))
            {
                nIncrease = -1;
            }

            int nIndex = nStart;

            while ((nLength + nIndex) % nLength != nEnd)
            {
                nIndex += nIncrease;
                int idx = (nLength + nIndex) % nLength;
                idx = (nIncrease < 0) ? idx + 1 : idx;
                idx = idx % nLength;
                lstPoints.Add(ptsPath[idx]);
            }

            return (PointF[])lstPoints.ToArray(typeof(PointF));
        }
        #endregion
    }
}