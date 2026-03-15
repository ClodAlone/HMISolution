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
    /// Represents the text area with the ability to span several pages.
    /// </summary>
    public class PdfTextElement : PdfLayoutElement
    {
        #region Fields
        /// <summary>
        /// Text data.
        /// </summary>
        private string m_text = string.Empty;

        /// <summary>
        /// Text data.
        /// </summary>
        private string m_value = string.Empty;
        /// <summary>
        /// Pen for text drawing.
        /// </summary>
        private PdfPen m_pen;

        /// <summary>
        /// Brush for text drawing.
        /// </summary>
        private PdfBrush m_brush;

        /// <summary>
        /// Font for text drawing.
        /// </summary>
        private PdfFont m_font;

        /// <summary>
        /// Text format.
        /// </summary>
        private PdfStringFormat m_format;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfTextElement"/> class.
        /// </summary>
        public PdfTextElement()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfTextElement"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        public PdfTextElement(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            m_text = text;
            text = PdfStandardFont.Convert(text);
            m_value = text;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfTextElement"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        public PdfTextElement(string text, PdfFont font)
            : this(text)
        {
            if (font == null)
            {
                throw new ArgumentNullException("font");
            }

            m_font = font;

            if (m_font is PdfStandardFont)
                m_value = PdfStandardFont.Convert(m_text);
            else
                m_value = m_text;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfTextElement"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="pen">The pen.</param>
        public PdfTextElement(string text, PdfFont font, PdfPen pen)
            : this(text, font)
        {
            m_pen = pen;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfTextElement"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="brush">The brush.</param>
        public PdfTextElement(string text, PdfFont font, PdfBrush brush)
            : this(text, font)
        {
            m_brush = brush;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfTextElement"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="format">The format.</param>
        public PdfTextElement(string text, PdfFont font, PdfPen pen, PdfBrush brush, PdfStringFormat format)
            : this(text, font, pen)
        {
            m_brush = brush;
            m_format = format;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating the text that should be printed.
        /// </summary>
        public string Text
        {
            get
            {
                return m_text;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Text");
                }
                if (m_font == null || m_font is PdfStandardFont)
                    m_value = PdfStandardFont.Convert(value);
                else
                    m_value = value;
                m_text = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the text that should be printed.
        /// </summary>
        internal string Value
        {
            get
            {
                return m_value;
            }
        }
        /// <summary>
        /// Gets or sets a pen that will be used to draw the text.
        /// </summary>
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
        /// Gets or sets the brush that will be used to draw the text.
        /// </summary>
        public PdfBrush Brush
        {
            get
            {
                return m_brush;
            }

            set
            {
                m_brush = value;
            }
        }

        /// <summary>
        /// Gets or sets a font that will be used to draw the text.
        /// </summary>
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

                if (m_font is PdfStandardFont && m_text != null)
                    m_value = PdfStandardFont.Convert(m_text);
                else
                    m_value = m_text;
            }
        }

        /// <summary>
        /// Gets or sets text settings that will be used to draw the text.
        /// </summary>
        public PdfStringFormat StringFormat
        {
            get
            {
                return m_format;
            }

            set
            {
                m_format = value;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Draws the text on the page.
        /// </summary>
        /// <param name="page">Current page where the text should be drawn.</param>
        /// <param name="location">Start location on the page.</param>
        /// <param name="format">Lay outing format.</param>
        /// <returns>Lay outing result.</returns>
        new public PdfTextLayoutResult Draw(PdfPage page, PointF location, PdfLayoutFormat format)
        {
            RectangleF layoutRectangle = new RectangleF(location, SizeF.Empty);
            return Draw(page, layoutRectangle, format);
        }

        /// <summary>
        /// Draws the text on the page.
        /// </summary>
        /// <param name="page">Current page where the text should be drawn.</param>
        /// <param name="location">Start location on the page.</param>
        /// <param name="width">Width of the text bounds.</param>
        /// <param name="format">Lay outing format.</param>
        /// <returns>Lay outing result.</returns>
        public PdfTextLayoutResult Draw(PdfPage page, PointF location, float width, PdfLayoutFormat format)
        {
            RectangleF layoutRectangle = new RectangleF(location.X, location.Y, width, 0);
            return Draw(page, layoutRectangle, format);
        }

        /// <summary>
        /// Draws the text on the page.
        /// </summary>
        /// <param name="page">Current page where the text should be drawn.</param>
        /// <param name="layoutRectangle">RectangleF structure that specifies the bounds of the text.</param>
        /// <param name="format">Lay outing format.</param>
        /// <returns>Lay outing result.</returns>
        new public PdfTextLayoutResult Draw(PdfPage page, RectangleF layoutRectangle, PdfLayoutFormat format)
        {
            if (page == null)
            {
                throw new ArgumentNullException("page");
            }

            PdfLayoutParams param = new PdfLayoutParams();

            param.Page = page;
            param.Bounds = layoutRectangle;
            param.Format = (format != null) ? format : new PdfLayoutFormat();

            PdfLayoutResult result = Layout(param);

            return result as PdfTextLayoutResult;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets a brush for drawing.
        /// </summary>
        /// <returns>Gets a brush for drawing.</returns>
        internal PdfBrush GetBrush()
        {
            return (m_brush == null) ? PdfBrushes.Black : m_brush;
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

            if (Font == null)
            {
                throw new ArgumentNullException("Font can't be null");
            }

            graphics.DrawString(Value, Font, Pen, GetBrush(), PointF.Empty, StringFormat);
        }

        /// <summary>
        /// Layouts the element.
        /// </summary>
        /// <param name="param">Lay outing parameters.</param>
        /// <returns>Returns lay outing results.</returns>
        #if !NETFX_CORE  && !WP
        [Syncfusion.Documentation.DocumentationExclude()] 
        #endif
        protected override PdfLayoutResult Layout(PdfLayoutParams param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            if (Font == null)
            {
                throw new ArgumentNullException("Font can't be null");
            }

            TextLayouter layouter = new TextLayouter(this);
            PdfTextLayoutResult result = (PdfTextLayoutResult)layouter.Layout(param);

            return result;
        }
        #endregion
    }
}
