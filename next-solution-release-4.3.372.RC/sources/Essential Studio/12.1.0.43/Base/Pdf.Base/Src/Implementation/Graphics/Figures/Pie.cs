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
    /// Represents a pie shape.
    /// </summary>
    public class PdfPie : PdfEllipsePart
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfPie"/> class.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="sweepAngle">The sweep angle.</param>
        public PdfPie(float width, float height, float startAngle, float sweepAngle)
            : this(0, 0, width, height, startAngle, sweepAngle)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfPie"/> class.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="sweepAngle">The sweep angle.</param>
        public PdfPie(PdfPen pen, float width, float height, float startAngle, float sweepAngle)
            : this(pen, 0, 0, width, height, startAngle, sweepAngle)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfPie"/> class.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="sweepAngle">The sweep angle.</param>
        public PdfPie(PdfBrush brush, float width, float height, float startAngle, float sweepAngle)
            : this(brush, 0, 0, width, height, startAngle, sweepAngle)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfPie"/> class.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="sweepAngle">The sweep angle.</param>
        public PdfPie(PdfPen pen, PdfBrush brush, float width, float height, float startAngle, float sweepAngle)
            : this(pen, brush, 0, 0, width, height, startAngle, sweepAngle)
        {
        }

        public PdfPie(float x, float y, float width, float height, float startAngle, float sweepAngle)
            : base(x, y, width, height, startAngle, sweepAngle)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfPie"/> class.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="sweepAngle">The sweep angle.</param>
        public PdfPie(RectangleF rectangle, float startAngle, float sweepAngle)
            : base(rectangle, startAngle, sweepAngle)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfPie"/> class.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="sweepAngle">The sweep angle.</param>
        public PdfPie(PdfPen pen, float x, float y, float width, float height,
            float startAngle, float sweepAngle)
            : base(pen, null, x, y, width, height, startAngle, sweepAngle)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfPie"/> class.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="sweepAngle">The sweep angle.</param>
        public PdfPie(PdfPen pen, RectangleF rectangle, float startAngle, float sweepAngle)
            : base(pen, null, rectangle, startAngle, sweepAngle)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfPie"/> class.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="sweepAngle">The sweep angle.</param>
        public PdfPie(PdfBrush brush, float x, float y, float width, float height,
            float startAngle, float sweepAngle)
            : this(x, y, width, height, startAngle, sweepAngle)
        {
            Brush = brush;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfPie"/> class.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="sweepAngle">The sweep angle.</param>
        public PdfPie(PdfBrush brush, RectangleF rectangle, float startAngle, float sweepAngle)
            : this(rectangle, startAngle, sweepAngle)
        {
            Brush = brush;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfPie"/> class.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="sweepAngle">The sweep angle.</param>
        public PdfPie(PdfPen pen, PdfBrush brush, float x, float y, float width, float height,
            float startAngle, float sweepAngle)
            : this(x, y, width, height, startAngle, sweepAngle)
        {
            Pen = pen;
            Brush = brush;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfPie"/> class.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="sweepAngle">The sweep angle.</param>
        public PdfPie(PdfPen pen, PdfBrush brush, RectangleF rectangle,
            float startAngle, float sweepAngle)
            : this(rectangle, startAngle, sweepAngle)
        {
            Pen = pen;
            Brush = brush;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfPie"/> class.
        /// </summary>
        protected PdfPie()
        {
        }
        #endregion

        #region Implementation
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

            graphics.DrawPie(GetPen(), Brush, Bounds, StartAngle, SweepAngle);
        }
        #endregion
    }
}
