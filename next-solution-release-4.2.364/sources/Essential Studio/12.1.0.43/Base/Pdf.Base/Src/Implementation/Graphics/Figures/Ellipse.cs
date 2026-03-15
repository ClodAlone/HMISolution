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
    /// Describes an ellipse shape.
    /// </summary>
    public class PdfEllipse : PdfRectangleArea
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfEllipse"/> class.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public PdfEllipse(float width, float height)
            : this(0, 0, width, height)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfEllipse"/> class.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public PdfEllipse(PdfPen pen, float width, float height)
            : this(pen, 0, 0, width, height)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfEllipse"/> class.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public PdfEllipse(PdfBrush brush, float width, float height)
            : this(brush, 0, 0, width, height)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfEllipse"/> class.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public PdfEllipse(PdfPen pen, PdfBrush brush, float width, float height)
            : this(pen, brush, 0, 0, width, height)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfEllipse"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public PdfEllipse(float x, float y, float width, float height)
            : base(x, y, width, height)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfEllipse"/> class.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        public PdfEllipse(RectangleF rectangle)
            : base(rectangle)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfEllipse"/> class.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public PdfEllipse(PdfPen pen, float x, float y, float width, float height)
            : base(pen, null, x, y, width, height)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfEllipse"/> class.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="rectangle">The rectangle.</param>
        public PdfEllipse(PdfPen pen, RectangleF rectangle)
            : base(pen, null, rectangle)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfEllipse"/> class.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public PdfEllipse(PdfBrush brush, float x, float y, float width, float height)
            : base(null, brush, x, y, width, height)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfEllipse"/> class.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="rectangle">The rectangle.</param>
        public PdfEllipse(PdfBrush brush, RectangleF rectangle)
            : base(null, brush, rectangle)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfEllipse"/> class.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public PdfEllipse(PdfPen pen, PdfBrush brush, float x, float y, float width, float height)
            : base(pen, brush, x, y, width, height)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfEllipse"/> class.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="rectangle">The rectangle.</param>
        public PdfEllipse(PdfPen pen, PdfBrush brush, RectangleF rectangle)
            : base(pen, brush, rectangle)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfEllipse"/> class.
        /// </summary>
        protected PdfEllipse()
            : base()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the radius X.
        /// </summary>
        public float RadiusX
        {
            get
            {
                return Width / 2;
            }
        }

        /// <summary>
        /// Gets the radius Y.
        /// </summary>
        public float RadiusY
        {
            get
            {
                return Height / 2;
            }
        }

        /// <summary>
        /// Gets the center point.
        /// </summary>
        public PointF Center
        {
            get
            {
                return new PointF(X + RadiusX, Y + RadiusY);
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Draws an element on the Graphics.
        /// </summary>
        /// <param name="graphics">Graphics context where the element should be printed.</param>
        #if !NETFX_CORE && !WP
        [Syncfusion.Documentation.DocumentationExclude()] 
        #endif
        protected override void DrawInternal(PdfGraphics graphics)
        {
            if (graphics == null)
            {
                throw new ArgumentNullException("graphics");
            }

            graphics.DrawEllipse(GetPen(), Brush, Bounds);
        }
        #endregion
    }
}
