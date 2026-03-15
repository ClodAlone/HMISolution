#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Drawing;

using Syncfusion.Pdf.Graphics;

/// <summary>
/// The Syncfusion.Pdf namespace contains classes for creating PDF document.
/// </summary>
namespace Syncfusion.Pdf
{
    /// <summary>
    /// Represents a fields which is calculated before the document saves.
    /// </summary>
    /// <seealso cref="PdfGraphicsElement"/> Class    
    public abstract class PdfAutomaticField : PdfGraphicsElement
    {
        #region Fields
        /// <summary>
        /// Internal variable to store field's bounds.
        /// </summary>
        private RectangleF m_bounds = RectangleF.Empty;

        /// <summary>
        /// Internal variable to store font.
        /// </summary>
        private PdfFont m_font;

        /// <summary>
        /// Internal variable to store brush.
        /// </summary>
        private PdfBrush m_brush;

        /// <summary>
        /// Internal variable to store pen.
        /// </summary>
        private PdfPen m_pen;

        /// <summary>
        /// Internal variable to store string format.
        /// </summary>
        private PdfStringFormat m_stringFormat;

        /// <summary>
        /// Internal variable to store template size.
        /// </summary>
        private SizeF m_templateSize = SizeF.Empty;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfAutomaticField"/> class.
        /// </summary>
        protected PdfAutomaticField()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfAutomaticField"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        protected PdfAutomaticField(PdfFont font)
            : base()
        {
            Font = font;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfAutomaticField"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="brush">The brush.</param>
        protected PdfAutomaticField(PdfFont font, PdfBrush brush)
            : base()
        {
            Font = font;
            Brush = brush;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfAutomaticField"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="bounds">The bounds.</param>
        protected PdfAutomaticField(PdfFont font, RectangleF bounds)
            : base()
        {
            Font = font;
            Bounds = bounds;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the bounds of the field.
        /// </summary>
        /// <value>The bounds value.</value>
        public RectangleF Bounds
        {
            get
            {
                return m_bounds;
            }

            set
            {
                m_bounds = value;
            }
        }

        /// <summary>
        /// Gets or sets the size of the field.
        /// </summary>
        /// <value>The size of the field.</value>
        public SizeF Size
        {
            get
            {
                return m_bounds.Size;
            }

            set
            {
                m_bounds.Size = value;
            }
        }

        /// <summary>
        /// Gets or sets the location of the field.
        /// </summary>
        /// <value>The location.</value>
        public PointF Location
        {
            get
            {
                return m_bounds.Location;
            }

            set
            {
                m_bounds.Location = value;
            }
        }

        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        /// <value>The font.</value>
        public PdfFont Font
        {
            get
            {
                return m_font;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Font");
                }
                m_font = value;
            }
        }

        /// <summary>
        /// Gets or sets the brush.
        /// </summary>
        /// <value>The brush.</value>
        public PdfBrush Brush
        {
            get
            {
                return m_brush;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Brush");
                }

                m_brush = value;
            }
        }

        /// <summary>
        /// Gets or sets the pen.
        /// </summary>
        /// <value>The pen.</value>
        public PdfPen Pen
        {
            get
            {
                return m_pen;
            }

            set
            {
                m_pen = value;
            }
        }

        /// <summary>
        /// Gets or sets the string format.
        /// </summary>
        /// <value>The string format.</value>
        public PdfStringFormat StringFormat
        {
            get
            {
                return m_stringFormat;
            }

            set
            {
                m_stringFormat = value;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Draws an element on the Graphics.
        /// </summary>
        /// <param name="graphics">Graphics context where the element should be printed.</param>
        /// <param name="x">X co-ordinate of the element.</param>
        /// <param name="y">Y co-ordinate of the element.</param>
        /// <exclude/>
        public override void Draw(PdfGraphics graphics, float x, float y)
        {
            base.Draw(graphics, x, y);
            graphics.AutomaticFields.Add(new PdfAutomaticFieldInfo(this, new PointF(x, y)));
        }

        /// <summary>
        /// Gets the value of the field at the specified graphics.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <exclude/>
        internal protected abstract string GetValue(PdfGraphics graphics);

        /// <summary>
        /// Performs draw.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="location">The location.</param>
        /// <param name="scalingX">The scaling X.</param>
        /// <param name="scalingY">The scaling Y.</param>
        /// <exclude/>
        internal virtual protected void PerformDraw(PdfGraphics graphics, PointF location,
            float scalingX, float scalingY)
        {
            if (Bounds.Height == 0 || Bounds.Width == 0)
            {
                string text = GetValue(graphics);
                m_templateSize = GetFont().MeasureString(text, Size, StringFormat);
            }
        }

        /// <summary>
        /// Gets the template size.
        /// </summary>
        /// <returns>The template size.</returns>
        /// <exclude/>
        protected SizeF GetSize()
        {
            if (Bounds.Height == 0 || Bounds.Width == 0)
            {
                return m_templateSize;
            }
            else
            {
                return Size;
            }
        }

        /// <summary>
        /// Draws an element on the Graphics.
        /// </summary>
        /// <param name="graphics">Graphics context where the element should be printed.</param>
        /// <exclude/>
        protected override void DrawInternal(PdfGraphics graphics)
        {
        }

        /// <summary>
        /// Gets the brush. If brush is undefined default black brush will be used.
        /// </summary>
        /// <returns>The brush </returns>
        /// <exclude/>
        protected PdfBrush GetBrush()
        {
            return m_brush == null ? PdfBrushes.Black : m_brush;
        }

        /// <summary>
        /// Gets the font. If font is undefined default font will be used.
        /// </summary>
        /// <returns></returns>
        /// <exclude/>
        protected PdfFont GetFont()
        {
            return m_font == null ? PdfDocument.DefaultFont : m_font;
        }


        #endregion
    }
}
