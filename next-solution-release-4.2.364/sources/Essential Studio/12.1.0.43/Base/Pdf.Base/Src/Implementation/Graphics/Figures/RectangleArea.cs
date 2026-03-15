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
    /// Represents an area bound by a rectangle.
    /// </summary>
    public abstract class PdfRectangleArea : PdfFillElement
    {
        #region Fields
        /// <summary>
        /// Bounds of the element.
        /// </summary>
        private RectangleF m_rect;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfRectangleArea"/> class.
        /// </summary>
        protected PdfRectangleArea()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfRectangleArea"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        protected PdfRectangleArea(float x, float y, float width, float height)
            : this()
        {
            m_rect = new RectangleF(x, y, width, height);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfRectangleArea"/> class.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        protected PdfRectangleArea(RectangleF rectangle)
            : this()
        {
            m_rect = rectangle;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfRectangleArea"/> class.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        protected PdfRectangleArea(PdfPen pen, PdfBrush brush, float x, float y, float width, float height)
            : base(pen, brush)
        {
            m_rect = new RectangleF(x, y, width, height);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfRectangleArea"/> class.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="rectangle">The rectangle.</param>
        protected PdfRectangleArea(PdfPen pen, PdfBrush brush, RectangleF rectangle)
            : base(pen, brush)
        {
            m_rect = rectangle;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the X co-ordinate of the upper-left corner of this the element.
        /// </summary>
        public float X
        {
            get
            {
                return m_rect.X;
            }

            set
            {
                m_rect.X = value;
            }
        }

        /// <summary>
        /// Gets or sets the Y co-ordinate of the upper-left corner of this the element.
        /// </summary>
        public float Y
        {
            get
            {
                return m_rect.Y;
            }

            set
            {
                m_rect.Y = value;
            }
        }

        /// <summary>
        /// Gets or sets the width of this element.
        /// </summary>
        public float Width
        {
            get
            {
                return m_rect.Width;
            }

            set
            {
                m_rect.Width = value;
            }
        }

        /// <summary>
        /// Gets or sets the height of this element.
        /// </summary>
        public float Height
        {
            get
            {
                return m_rect.Height;
            }

            set
            {
                m_rect.Height = value;
            }
        }

        /// <summary>
        /// Gets or sets the size of this element.
        /// </summary>
        public SizeF Size
        {
            get
            {
                return m_rect.Size;
            }

            set
            {
                m_rect.Size = value;
            }
        }

        /// <summary>
        /// Gets or sets bounds of this element.
        /// </summary>
        public RectangleF Bounds
        {
            get
            {
                return m_rect;
            }

            set
            {
                m_rect = value;
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
            return Bounds;
        }
        #endregion
    }
}
