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
    /// Implements Bezier curve shape.
    /// </summary>
    public class PdfBezierCurve : PdfDrawElement
    {
        #region Fields
        /// <summary>
        /// Local variable to store the start Point.
        /// </summary>
        private PointF m_startPoint = PointF.Empty;

        /// <summary>
        /// Local variable to store the firstC ontrol Point.
        /// </summary>
        private PointF m_firstControlPoint = PointF.Empty;

        /// <summary>
        /// Local variable to store the second Control Point.
        /// </summary>
        private PointF m_secondControlPoint = PointF.Empty;

        /// <summary>
        /// Local variable to store the end Point.
        /// </summary>
        private PointF m_endPoint = PointF.Empty;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfBezierCurve"/> class.
        /// </summary>
        /// <param name="startPoint">The start point.</param>
        /// <param name="firstControlPoint">The first control point.</param>
        /// <param name="secondControlPoint">The second control point.</param>
        /// <param name="endPoint">The end point.</param>
        public PdfBezierCurve(PointF startPoint, PointF firstControlPoint, PointF secondControlPoint,
            PointF endPoint)
        {
            m_startPoint = startPoint;
            m_firstControlPoint = firstControlPoint;
            m_secondControlPoint = secondControlPoint;
            m_endPoint = endPoint;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfBezierCurve"/> class.
        /// </summary>
        /// <param name="startPointX">The start point X.</param>
        /// <param name="startPointY">The start point Y.</param>
        /// <param name="firstControlPointX">The first control point X.</param>
        /// <param name="firstControlPointY">The first control point Y.</param>
        /// <param name="secondControlPointX">The second control point X.</param>
        /// <param name="secondControlPointY">The second control point Y.</param>
        /// <param name="endPointX">The end point X.</param>
        /// <param name="endPointY">The end point Y.</param>
        public PdfBezierCurve(float startPointX, float startPointY, float firstControlPointX,
            float firstControlPointY, float secondControlPointX, float secondControlPointY,
            float endPointX, float endPointY)
        {
            m_startPoint.X = startPointX;
            m_startPoint.Y = startPointY;
            m_firstControlPoint.X = firstControlPointX;
            m_firstControlPoint.Y = firstControlPointY;
            m_secondControlPoint.X = secondControlPointX;
            m_secondControlPoint.Y = secondControlPointY;
            m_endPoint.X = endPointX;
            m_endPoint.Y = endPointY;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfBezierCurve"/> class.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="startPoint">The start point.</param>
        /// <param name="firstControlPoint">The first control point.</param>
        /// <param name="secondControlPoint">The second control point.</param>
        /// <param name="endPoint">The end point.</param>
        public PdfBezierCurve(PdfPen pen, PointF startPoint, PointF firstControlPoint,
            PointF secondControlPoint, PointF endPoint)
            : base(pen)
        {
            m_startPoint = startPoint;
            m_firstControlPoint = firstControlPoint;
            m_secondControlPoint = secondControlPoint;
            m_endPoint = endPoint;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfBezierCurve"/> class.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="startPointX">The start point X.</param>
        /// <param name="startPointY">The start point Y.</param>
        /// <param name="firstControlPointX">The first control point X.</param>
        /// <param name="firstControlPointY">The first control point Y.</param>
        /// <param name="secondControlPointX">The second control point X.</param>
        /// <param name="secondControlPointY">The second control point Y.</param>
        /// <param name="endPointX">The end point X.</param>
        /// <param name="endPointY">The end point Y.</param>
        public PdfBezierCurve(PdfPen pen, float startPointX, float startPointY, float firstControlPointX,
            float firstControlPointY, float secondControlPointX, float secondControlPointY,
            float endPointX, float endPointY)
            : base(pen)
        {
            m_startPoint.X = startPointX;
            m_startPoint.Y = startPointY;
            m_firstControlPoint.X = firstControlPointX;
            m_firstControlPoint.Y = firstControlPointY;
            m_secondControlPoint.X = secondControlPointX;
            m_secondControlPoint.Y = secondControlPointY;
            m_endPoint.X = endPointX;
            m_endPoint.Y = endPointY;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfBezierCurve"/> class.
        /// </summary>
        protected PdfBezierCurve()
            : base()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the start point.
        /// </summary>
        public PointF StartPoint
        {
            get
            {
                return m_startPoint;
            }

            set
            {
                m_startPoint = value;
            }
        }

        /// <summary>
        /// Gets or sets the first control point.
        /// </summary>
        public PointF FirstControlPoint
        {
            get
            {
                return m_firstControlPoint;
            }

            set
            {
                m_firstControlPoint = value;
            }
        }

        /// <summary>
        /// Gets or sets the second control point.
        /// </summary>
        public PointF SecondControlPoint
        {
            get
            {
                return m_secondControlPoint;
            }

            set
            {
                m_secondControlPoint = value;
            }
        }

        /// <summary>
        /// Gets or sets the end point.
        /// </summary>
        public PointF EndPoint
        {
            get
            {
                return m_endPoint;
            }

            set
            {
                m_endPoint = value;
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
            throw new NotImplementedException();
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

            graphics.DrawBezier(GetPen(), StartPoint, FirstControlPoint, SecondControlPoint, EndPoint);
        }
        #endregion
    }
}
