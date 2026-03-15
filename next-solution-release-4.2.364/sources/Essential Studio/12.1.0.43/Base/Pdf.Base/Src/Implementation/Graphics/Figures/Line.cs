#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Drawing;
using System.Text;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Represents a line shape.
    /// </summary>
    public class PdfLine : PdfDrawElement
    {
        #region Fields
        /// <summary>
        /// Local variable to store x2.
        /// </summary>
        private float m_x1 = 0.0f;

        /// <summary>
        /// Local variable to store Y1.
        /// </summary>
        private float m_y1 = 0.0f;

        /// <summary>
        /// Local variable to store x1.
        /// </summary>
        private float m_x2 = 0.0f;

        /// <summary>
        /// Local variable to store Y2.
        /// </summary>
        private float m_y2 = 0.0f;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLine"/> class.
        /// </summary>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="y2">The y2.</param>
        public PdfLine(float x1, float y1, float x2, float y2)
        {
            m_x1 = x1;
            m_y1 = y1;
            m_x2 = x2;
            m_y2 = y2;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLine"/> class.
        /// </summary>
        /// <param name="point1">The point1.</param>
        /// <param name="point2">The point2.</param>
        public PdfLine(PointF point1, PointF point2)
            : this(point1.X, point1.Y, point2.X, point2.Y)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLine"/> class.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="y2">The y2.</param>
        public PdfLine(PdfPen pen, float x1, float y1, float x2, float y2)
            : base(pen)
        {
            m_x1 = x1;
            m_y1 = y1;
            m_x2 = x2;
            m_y2 = y2;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLine"/> class.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="point1">The point1.</param>
        /// <param name="point2">The point2.</param>
        public PdfLine(PdfPen pen, PointF point1, PointF point2)
            : base(pen)
        {
            m_x1 = point1.X;
            m_y1 = point1.Y;
            m_x2 = point2.X;
            m_y2 = point2.Y;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLine"/> class.
        /// </summary>
        private PdfLine()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the x coordinate of the start point.
        /// </summary>
        public float X1
        {
            get
            {
                return m_x1;
            }

            set
            {
                m_x1 = value;
            }
        }

        /// <summary>
        /// Gets or sets the y coordinate of the start point.
        /// </summary>
        public float Y1
        {
            get
            {
                return m_y1;
            }

            set
            {
                m_y1 = value;
            }
        }

        /// <summary>
        /// Gets or sets the x coordinate of the end point.
        /// </summary>
        public float X2
        {
            get
            {
                return m_x2;
            }

            set
            {
                m_x2 = value;
            }
        }

        /// <summary>
        /// Gets or sets the y coordinate of the end point.
        /// </summary>
        public float Y2
        {
            get
            {
                return m_y2;
            }

            set
            {
                m_y2 = value;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Overloaded. Returns a rectangle that bounds this element.
        /// </summary>
        /// <returns>Returns a rectangle that bounds this element.</returns>
        #if !NETFX_CORE  && !WP
        [Syncfusion.Documentation.DocumentationExclude()] 
        #endif
        protected override RectangleF GetBoundsInternal()
        {
            float left = Math.Min(X1, X2);
            float right = Math.Max(X1, X2);
            float top = Math.Min(Y1, Y2);
            float bottom = Math.Max(Y1, Y2);

            RectangleF bounds = new RectangleF(left, top, right - left, bottom - top);

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

            graphics.DrawLine(GetPen(), X1, Y1, X2, Y2);
        }
        #endregion
    }
}
