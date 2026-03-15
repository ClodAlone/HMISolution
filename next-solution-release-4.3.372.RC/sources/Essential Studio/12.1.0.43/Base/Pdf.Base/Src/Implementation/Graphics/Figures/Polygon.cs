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
using System.Text;
using System.Collections.Generic;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Represents a set of points connected with lines, could be drawn and filled.
    /// </summary>
    public class PdfPolygon : PdfFillElement
    {
        #region Fields
        /// <summary>
        /// Array of the points.
        /// </summary>
        private List<PointF> m_points;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfPolygon"/> class.
        /// </summary>
        /// <param name="points">The points.</param>
        public PdfPolygon(PointF[] points)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }

            Points = points;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfPolygon"/> class.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="points">The points.</param>
        public PdfPolygon(PdfPen pen, PointF[] points)
            : base(pen)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }

            Points = points;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfPolygon"/> class.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="points">The points.</param>
        public PdfPolygon(PdfBrush brush, PointF[] points)
            : base(brush)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }

            Points = points;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfPolygon"/> class.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="points">The points.</param>
        public PdfPolygon(PdfPen pen, PdfBrush brush, PointF[] points)
            : base(pen, brush)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }

            Points = points;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfPolygon"/> class.
        /// </summary>
        protected PdfPolygon()
            : base()
        {
            m_points = new List<PointF>();
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the points of the polygon.
        /// </summary>
        public PointF[] Points
        {
            get
            {
                if (m_points == null)
                {
                    m_points = new List<PointF>();
                }

                PointF[] points = m_points.ToArray();

                return points;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Points");
                }

                if (m_points == null)
                {
                    m_points = new List<PointF>();
                }

                m_points.Clear();
                m_points.AddRange(value);
            }
        }

        /// <summary>
        /// Gets a number of the points in the polygon.
        /// </summary>
        public int Count
        {
            get
            {
                return Points.Length;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Adds a point to the polygon.
        /// </summary>
        /// <param name="point">The last point of the polygon.</param>
        public void AddPoint(PointF point)
        {
            m_points.Add(point);
        }

        /// <summary>
        /// Overloaded. Returns a rectangle that bounds this element.
        /// </summary>
        /// <returns>Returns a rectangle that bounds this element.</returns>
        #if !NETFX_CORE  && !WP
        [Syncfusion.Documentation.DocumentationExclude()] 
        #endif
        protected override RectangleF GetBoundsInternal()
        {
            RectangleF bounds = RectangleF.Empty;

            if (Points.Length > 0)
            {
                PointF[] points = Points;
                float left = points[0].X;
                float right = points[0].X;
                float top = points[0].Y;
                float bottom = points[0].Y;

                for (int i = 1; i < points.Length; i++)
                {
                    PointF point = points[i];

                    left = Math.Min(left, point.X);
                    right = Math.Max(right, point.X);
                    top = Math.Min(top, point.Y);
                    bottom = Math.Max(bottom, point.Y);
                }

                bounds = new RectangleF(left, top, right - left, bottom - top);
            }

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

            graphics.DrawPolygon(GetPen(), Brush, Points);
        }
        #endregion
    }
}
