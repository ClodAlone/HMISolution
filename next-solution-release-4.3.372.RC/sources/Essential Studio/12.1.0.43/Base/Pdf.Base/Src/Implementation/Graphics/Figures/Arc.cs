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
    /// Represents an arc shape.
    /// </summary>
    /// <remarks>It ignores brush setting.</remarks>
    public class PdfArc : PdfEllipsePart
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfArc"/> class.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="sweepAngle">The sweep angle.</param>
        public PdfArc(float width, float height, float startAngle, float sweepAngle)
            : this(0, 0, width, height, startAngle, sweepAngle)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfArc"/> class.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="sweepAngle">The sweep angle.</param>
        public PdfArc(PdfPen pen, float width, float height, float startAngle, float sweepAngle)
            : this(pen, 0, 0, width, height, startAngle, sweepAngle)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfArc"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="sweepAngle">The sweep angle.</param>
        public PdfArc(float x, float y, float width, float height, float startAngle, float sweepAngle)
            : base(x, y, width, height, startAngle, sweepAngle)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfArc"/> class.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="sweepAngle">The sweep angle.</param>
        public PdfArc(RectangleF rectangle, float startAngle, float sweepAngle)
            : base(rectangle, startAngle, sweepAngle)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfArc"/> class.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="sweepAngle">The sweep angle.</param>
        public PdfArc(PdfPen pen, float x, float y, float width, float height,
            float startAngle, float sweepAngle)
            : base(pen, null, x, y, width, height, startAngle, sweepAngle)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfArc"/> class.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="sweepAngle">The sweep angle.</param>
        public PdfArc(PdfPen pen, RectangleF rectangle, float startAngle, float sweepAngle)
            : base(pen, null, rectangle, startAngle, sweepAngle)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfArc"/> class.
        /// </summary>
        protected PdfArc()
            : base()
        {
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

            graphics.DrawArc(GetPen(), Bounds, StartAngle, SweepAngle);
        }
        #endregion
    }
}
