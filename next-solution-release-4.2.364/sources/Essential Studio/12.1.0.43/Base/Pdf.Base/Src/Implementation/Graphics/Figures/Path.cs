#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Collections.Generic;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Implements graphics path, which is a sequence of primitive graphics elements.
    /// </summary>
    public class PdfPath : PdfFillElement
    {
        #region Fields
        /// <summary>
        /// Local varaible to store the points.
        /// </summary>
        private List<PointF> m_points = null;

        /// <summary>
        /// Local varaible to store the path Types.
        /// </summary>
        private List<byte> m_pathTypes = null;

        /// <summary>
        /// Local varaible to store the Start Figure.
        /// </summary>
        private bool m_bStartFigure = true;

        /// <summary>
        /// Local varaible to store the fill Mode.
        /// </summary>
        private PdfFillMode m_fillMode = PdfFillMode.Winding;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfPath"/> class.
        /// </summary>
        public PdfPath()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfPath"/> class.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="pathTypes">The path types.</param>
        public PdfPath(PointF[] points, byte[] pathTypes)
        {
            AddPath(points, pathTypes);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfPath"/> class.
        /// </summary>
        /// <param name="pen">The pen.</param>
        public PdfPath(PdfPen pen)
            : base(pen)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfPath"/> class.
        /// </summary>
        /// <param name="brush">The brush.</param>
        public PdfPath(PdfBrush brush)
            : base(brush)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfPath"/> class.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="fillMode">The fill mode.</param>
        public PdfPath(PdfBrush brush, PdfFillMode fillMode)
            : this(brush)
        {
            FillMode = fillMode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfPath"/> class.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="points">The points.</param>
        /// <param name="pathTypes">The path types.</param>
        public PdfPath(PdfPen pen, PointF[] points, byte[] pathTypes)
            : base(pen)
        {
            AddPath(points, pathTypes);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfPath"/> class.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="fillMode">The fill mode.</param>
        /// <param name="points">The points.</param>
        /// <param name="pathTypes">The path types.</param>
        public PdfPath(PdfBrush brush, PdfFillMode fillMode, PointF[] points, byte[] pathTypes)
            : base(brush)
        {
            AddPath(points, pathTypes);
            FillMode = fillMode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfPath"/> class.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="fillMode">The fill mode.</param>
        public PdfPath(PdfPen pen, PdfBrush brush, PdfFillMode fillMode)
            : base(pen, brush)
        {
            FillMode = fillMode;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the fill mode.
        /// </summary>
        public PdfFillMode FillMode
        {
            get
            {
                return m_fillMode;
            }

            set
            {
                m_fillMode = value;
            }
        }

        /// <summary>
        /// Gets the path points.
        /// </summary>
        public PointF[] PathPoints
        {
            get
            {
                return Points.ToArray();
            }
        }

        /// <summary>
        /// Gets the path point types.
        /// </summary>
        public byte[] PathTypes
        {
            get
            {
                return Types.ToArray();
            }
        }

        /// <summary>
        /// Gets the point count.
        /// </summary>
        public int PointCount
        {
            get
            {
                int count = 0;

                if (m_points != null)
                {
                    count = m_points.Count;
                }

                return count;
            }
        }

        /// <summary>
        /// Gets the last point.
        /// </summary>
        public PointF LastPoint
        {
            get
            {
                return GetLastPoint();
            }
        }

        /// <summary>
        /// Gets the points list.
        /// </summary>
        /// <value>The points.</value>
        internal List<PointF> Points
        {
            get
            {
                if (m_points == null)
                {
                    m_points = new List<PointF>();
                }

                return m_points;
            }
        }

        /// <summary>
        /// Gets the types.
        /// </summary>
        /// <value>The types.</value>
        internal List<byte> Types
        {
            get
            {
                if (m_pathTypes == null)
                {
                    m_pathTypes = new List<byte>();
                }

                return m_pathTypes;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds an arc.
        /// </summary>
        /// <param name="rectangle">The boundaries of the arc.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="sweepAngle">The sweep angle.</param>
        public void AddArc(RectangleF rectangle, float startAngle, float sweepAngle)
        {
            AddArc(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, startAngle, sweepAngle);
        }

        /// <summary>
        /// Adds an arc.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="sweepAngle">The sweep angle.</param>
        public void AddArc(float x, float y, float width, float height,
            float startAngle, float sweepAngle)
        {
            List<float[]> points =
                PdfGraphics.GetBezierArcPoints(x, y, x + width, y + height, startAngle, sweepAngle);
            List<float> list = new List<float>(8);

            for (int i = 0; i < points.Count; ++i)
            {
                float[] pt = (float[])points[i];
                list.Clear();
                list.AddRange(pt);

                AddPoints(list, PathPointType.Bezier3);
            }
        }

        /// <summary>
        /// Adds a bezier curve.
        /// </summary>
        /// <param name="startPoint">The start point.</param>
        /// <param name="firstControlPoint">The first control point.</param>
        /// <param name="secondControlPoint">The second control point.</param>
        /// <param name="endPoint">The end point.</param>
        public void AddBezier(PointF startPoint, PointF firstControlPoint, PointF secondControlPoint,
            PointF endPoint)
        {
            AddBezier(startPoint.X, startPoint.Y, firstControlPoint.X, firstControlPoint.Y,
                secondControlPoint.X, secondControlPoint.Y, endPoint.X, endPoint.Y);
        }

        /// <summary>
        /// Adds a bezier curve.
        /// </summary>
        /// <param name="startPointX">The start point X.</param>
        /// <param name="startPointY">The start point Y.</param>
        /// <param name="firstControlPointX">The first control point X.</param>
        /// <param name="firstControlPointY">The first control point Y.</param>
        /// <param name="secondControlPointX">The second control point X.</param>
        /// <param name="secondControlPointY">The second control point Y.</param>
        /// <param name="endPointX">The end point X.</param>
        /// <param name="endPointY">The end point Y.</param>
        public void AddBezier(float startPointX, float startPointY, float firstControlPointX,
            float firstControlPointY, float secondControlPointX, float secondControlPointY,
            float endPointX, float endPointY)
        {
            List<float> points = new List<float>(8);

            points.Add(startPointX);
            points.Add(startPointY);
            points.Add(firstControlPointX);
            points.Add(firstControlPointY);
            points.Add(secondControlPointX);
            points.Add(secondControlPointY);
            points.Add(endPointX);
            points.Add(endPointY);

            AddPoints(points, PathPointType.Bezier3);
        }

        /// <summary>
        /// Adds an ellipse.
        /// </summary>
        /// <param name="rectangle">The boundaries of the ellipse.</param>
        public void AddEllipse(RectangleF rectangle)
        {
            AddEllipse(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        /// <summary>
        /// Adds an ellipse.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void AddEllipse(float x, float y, float width, float height)
        {
            StartFigure();
            AddArc(x, y, width, height, 0, 360);
            CloseFigure();
        }

        /// <summary>
        /// Adds a line.
        /// </summary>
        /// <param name="point1">The point1.</param>
        /// <param name="point2">The point2.</param>
        public void AddLine(PointF point1, PointF point2)
        {
            AddLine(point1.X, point1.Y, point2.X, point2.Y);
        }

        /// <summary>
        /// Adds a line.
        /// </summary>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="y2">The y2.</param>
        public void AddLine(float x1, float y1, float x2, float y2)
        {
            List<float> points = new List<float>(4);

            points.Add(x1);
            points.Add(y1);
            points.Add(x2);
            points.Add(y2);
            AddPoints(points, PathPointType.Line);
        }

        /// <summary>
        /// Appends the path specified to this one.
        /// </summary>
        /// <param name="path">The path, which should be appended.</param>
        public void AddPath(PdfPath path)
        {
            AddPath(path.PathPoints, path.PathTypes);
        }

        /// <summary>
        /// Appends the path specified by the points and their types to this one.
        /// </summary>
        /// <param name="pathPoints">The points.</param>
        /// <param name="pathTypes">The path point types.</param>
        public void AddPath(PointF[] pathPoints, byte[] pathTypes)
        {
            if (pathPoints == null)
            {
                throw new ArgumentNullException("pathPoints");
            }

            if (pathTypes == null)
            {
                throw new ArgumentNullException("pathTypes");
            }

            int count = pathPoints.Length;

            if (count != pathTypes.Length)
            {
                throw new ArgumentException("The argument arrays should be of equal length.");
            }

            Points.AddRange(pathPoints);
            Types.AddRange(pathTypes);
        }

        /// <summary>
        /// Appends the pie to this path.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="sweepAngle">The sweep angle.</param>
        public void AddPie(RectangleF rectangle, float startAngle, float sweepAngle)
        {
            AddPie(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, startAngle, sweepAngle);
        }

        /// <summary>
        /// Appends the pie to this path.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="sweepAngle">The sweep angle.</param>
        public void AddPie(float x, float y, float width, float height,
            float startAngle, float sweepAngle)
        {
            StartFigure();
            AddArc(x, y, width, height, startAngle, sweepAngle);
            AddPoint(new PointF(x + width / 2, y + height / 2), PathPointType.Line);
            CloseFigure();
        }

        /// <summary>
        /// Append the closed polygon to this path.
        /// </summary>
        /// <param name="points">The points of the polygon.</param>
        public void AddPolygon(PointF[] points)
        {
            int count = points.Length * 2;
            List<float> p = new List<float>(count);

            StartFigure();

            foreach (PointF point in points)
            {
                p.Add(point.X);
                p.Add(point.Y);
            }

            AddPoints(p, PathPointType.Line);
            CloseFigure();
        }

        /// <summary>
        /// Appends the rectangle to this path.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        public void AddRectangle(RectangleF rectangle)
        {
            AddRectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        /// <summary>
        /// Appends the rectangle to this path.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void AddRectangle(float x, float y, float width, float height)
        {
            List<float> points = new List<float>();

            StartFigure();
            points.Add(x);
            points.Add(y);

            points.Add(x + width);
            points.Add(y);

            points.Add(x + width);
            points.Add(y + height);

            points.Add(x);
            points.Add(y + height);

            AddPoints(points, PathPointType.Line);
            CloseFigure();
        }

        /// <summary>
        /// Starts a new figure.
        /// </summary>
        /// <remarks>The next added primitive will start a new figure.</remarks>
        public void StartFigure()
        {
            m_bStartFigure = true;
        }

        /// <summary>
        /// Closes the last figure.
        /// </summary>
        public void CloseFigure()
        {
            if (PointCount > 0)
            {
                CloseFigure(PointCount - 1);
            }

            StartFigure();
        }

        /// <summary>
        /// Closes all non-closed figures.
        /// </summary>
        public void CloseAllFigures()
        {
            for (int i = 0, count = m_pathTypes.Count; i < count; ++i)
            {
                PathPointType pt = (PathPointType)((byte)Types[i]);

                if (i != 0 && pt == PathPointType.Start)
                {
                    CloseFigure(i - 1);
                }
            }
        }

        /// <summary>
        /// Gets the last point.
        /// </summary>
        /// <returns>The last point.</returns>
        public PointF GetLastPoint()
        {
            PointF lastPoint = PointF.Empty;
            int count = PointCount;

            if (count > 0 && m_points != null)
            {
                lastPoint = (PointF)m_points[count - 1];
            }

            return lastPoint;
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Overloaded. Returns a rectangle that bounds this element.
        /// </summary>
        /// <returns>Returns a rectangle that bounds this element.</returns>
        #if !NETFX_CORE  && !WP
        [Syncfusion.Documentation.DocumentationExclude()] 
        #endif
        protected override RectangleF GetBoundsInternal()
        {
            PointF min = PointF.Empty;
            PointF max = PointF.Empty;
            PointF[] points = PathPoints;

            for (int i = 0, cnt = points.Length; i < cnt; ++i)
            {
                PointF point = points[i];

                min.X = Math.Min(point.X, min.X);
                min.Y = Math.Min(point.Y, min.Y);
                max.X = Math.Max(point.X, min.X);
                max.Y = Math.Max(point.Y, min.Y);
            }

            RectangleF bounds = new RectangleF(min.X, min.Y, max.X - min.X, max.Y - min.Y);

            return bounds;
        }

        /// <summary>
        /// Draws an element on the Graphics.
        /// </summary>
        /// <param name="graphics">Graphics context where the element should be printed.</param>
        #if !NETFX_CORE  && !WP
        [Syncfusion.Documentation.DocumentationExclude()]
        #endif
        protected override void DrawInternal(PdfGraphics graphics)
        {
            if (graphics == null)
            {
                throw new ArgumentNullException("graphics");
            }

            graphics.DrawPath(GetPen(), Brush, this);
        }

        /// <summary>
        /// Adds the points along with their type to the path.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="pointType">Type of the points.</param>
        private void AddPoints(List<float> points, PathPointType pointType)
        {
            AddPoints(points, pointType, 0, points.Count);
        }

        /// <summary>
        /// Adds the points along with their type to the path.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="pointType">Type of the points.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        private void AddPoints(List<float> points, PathPointType pointType, int startIndex, int endIndex)
        {
            for (int i = startIndex; i < endIndex; ++i)
            {
                PointF point = new PointF(points[i], points[i + 1]);

                if (i == startIndex)
                {
                    if (PointCount <= 0 || m_bStartFigure)
                    {
                        AddPoint(point, PathPointType.Start);
                        m_bStartFigure = false;
                    }
                    else if (point != LastPoint)
                    {
                        AddPoint(point, PathPointType.Line);
                    }
                }
                else
                {
                    AddPoint(point, pointType);
                }

                ++i;
            }
        }

        /// <summary>
        /// Adds a point and its type.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="pointType">Type of the point.</param>
        private void AddPoint(PointF point, PathPointType pointType)
        {
            Points.Add(point);
            Types.Add((byte)pointType);
        }

        /// <summary>
        /// Closes the figure.
        /// </summary>
        /// <param name="index">The index of the last figure point.</param>
        private void CloseFigure(int index)
        {
            if (index < 0)
            {
                throw new IndexOutOfRangeException();
            }

            PathPointType pt = (PathPointType)((byte)Types[index]);

            pt |= PathPointType.CloseSubpath;
            Types[index] = (byte)pt;
        }
        #endregion
    }
}
