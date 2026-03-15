#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Drawing;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// The base class of arc and pie shapes.
    /// </summary>
    public abstract class PdfEllipsePart : PdfRectangleArea
    {
        #region Fields
        private float m_startAngle = 0.0f;
        private float m_sweepAngle = 0.0f;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the start angle.
        /// </summary>
        public float StartAngle
        {
            get
            {
                return m_startAngle;
            }
            set
            {
                m_startAngle = value;
            }
        }

        /// <summary>
        /// Gets or sets the sweep angle.
        /// </summary>
        public float SweepAngle
        {
            get
            {
                return m_sweepAngle;
            }
            set
            {
                m_sweepAngle = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:EllipsePart"/> class.
        /// </summary>
        protected PdfEllipsePart()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:EllipsePart"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="sweepAngle">The sweep angle.</param>
        protected PdfEllipsePart(float x, float y, float width, float height, float startAngle, float sweepAngle)
            : base(x, y, width, height)
        {
            m_startAngle = startAngle;
            m_sweepAngle = sweepAngle;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:EllipsePart"/> class.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="sweepAngle">The sweep angle.</param>
        protected PdfEllipsePart(RectangleF rectangle, float startAngle, float sweepAngle)
            : base(rectangle)
        {
            m_startAngle = startAngle;
            m_sweepAngle = sweepAngle;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:EllipsePart"/> class.
        /// </summary>
        /// <param name="pen">A pen for this element.</param>
        /// <param name="brush">A Brush for this element.</param>
        /// <param name="x">X co-ordinate of the element.</param>
        /// <param name="y">Y co-ordinate of the element.</param>
        /// <param name="width">Width of the element.</param>
        /// <param name="height">Height of the element.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="sweepAngle">The sweep angle.</param>
        protected PdfEllipsePart(PdfPen pen, PdfBrush brush, float x, float y, float width, float height, float startAngle, float sweepAngle)
            : base(pen, brush, x, y, width, height)
        {
            m_startAngle = startAngle;
            m_sweepAngle = sweepAngle;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:EllipsePart"/> class.
        /// </summary>
        /// <param name="pen">A pen for this element.</param>
        /// <param name="brush">A Brush for this element.</param>
        /// <param name="rectangle">Bounds of the element.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="sweepAngle">The sweep angle.</param>
        protected PdfEllipsePart(PdfPen pen, PdfBrush brush, RectangleF rectangle, float startAngle, float sweepAngle)
            : base(pen, brush, rectangle)
        {
            m_startAngle = startAngle;
            m_sweepAngle = sweepAngle;
        }
        #endregion

    }
}
