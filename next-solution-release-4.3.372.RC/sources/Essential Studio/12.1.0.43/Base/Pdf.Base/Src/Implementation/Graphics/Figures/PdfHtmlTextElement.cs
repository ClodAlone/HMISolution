#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !SILVERLIGHT && !NETFX_CORE && !WP

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Graphics.Fonts;
using Syncfusion.Pdf.Graphics.Images;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Class that represent HTML text area with the ability to span several pages.
    /// </summary>
#if AllowUnsafeCode
    public class PdfHTMLTextElement
#else
    internal class PdfHTMLTextElement
#endif
    {
#region Fields
        /// <summary>
        /// The font.
        /// </summary>
        private PdfFont m_font;

        /// <summary>
        /// The brush.
        /// </summary>
        private PdfBrush m_brush;

        /// <summary>
        /// The HTML text.
        /// </summary>
        private string m_htmlText;

        /// <summary>
        /// The Text Alignment.
        /// </summary>
        private TextAlign m_textAlign;
        #endregion

#region constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfHTMLTextElement"/> class.
        /// </summary>
        public PdfHTMLTextElement()
        {
            m_font = new PdfStandardFont(PdfFontFamily.Helvetica, 3f);
            m_brush = PdfBrushes.Black;
            m_htmlText = "";
            m_textAlign = TextAlign.Left;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfHTMLTextElement"/> class.
        /// </summary>
        /// <param name="htmlText">The HTML text.</param>
        /// <param name="font">The font.</param>
        /// <param name="brush">The brush.</param>
        public PdfHTMLTextElement(string htmlText, PdfFont font, PdfBrush brush)
        {
            m_htmlText = htmlText;
            m_font = font;
            m_brush = brush;
        }
        #endregion

#region Properties
        /// <summary>
        /// Gets or sets the base font.
        /// </summary>
        public PdfFont Font
        {
            get
            {
                return m_font;
            }

            set
            {
                m_font = value;
            }
        }

        /// <summary>
        /// Gets or sets the default brush.
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
        /// Gets or sets the HTML Text.
        /// </summary>
        public string HTMLText
        {
            get
            {
                return m_htmlText;
            }

            set
            {
                m_htmlText = value;
            }
        }

        /// <summary>
        /// Gets or sets the text alignment.
        /// </summary>
        public TextAlign TextAlign
        {
            get
            {
                return m_textAlign;
            }

            set
            {
                m_textAlign = value;
            }
        }
        #endregion

#region Implementation

        /// <summary>
        /// Draws the text on the page.
        /// </summary>
        /// <param name="page">Current page where the text should be drawn.</param>
        /// <param name="layoutRectangle">RectangleF structure that specifies the bounds of the text.</param>
        /// <param name="format">Layout format.</param>
        /// <returns>Layout result.</returns>
        public PdfLayoutResult Draw(PdfPage page, RectangleF layoutRectangle, PdfMetafileLayoutFormat format)
        {
            if (page == null)
            {
                throw new ArgumentNullException("page");
            }

            if (layoutRectangle.Height < 0)
            {
                throw new ArgumentNullException("height");
            }

            RichTextBoxExt richtxtBox = new RichTextBoxExt();
            richtxtBox.RenderHTML(m_htmlText, m_font, m_brush);
            richtxtBox.SelectAll();
            richtxtBox.SelectionAlignment = m_textAlign;

            richtxtBox.Width = (int)layoutRectangle.Width;

            PdfUnitConvertor convertor = new PdfUnitConvertor();
            float width = convertor.ConvertUnits(layoutRectangle.Width, PdfGraphicsUnit.Point, PdfGraphicsUnit.Pixel);
            float height = convertor.ConvertUnits(layoutRectangle.Height, PdfGraphicsUnit.Point, PdfGraphicsUnit.Pixel);

            Image image = RtfToImage.ConvertToMetafile(richtxtBox, width, height);
            PdfImage img = PdfImage.FromImage(image);

            richtxtBox.Dispose();
            return img.Draw(page, new PointF(layoutRectangle.X, layoutRectangle.Y), format);
        }

        /// <summary>
        /// Draws the text on the graphics.
        /// </summary>
        /// <param name="page">Graphics context where the text should be drawn</param>
        /// <param name="layoutRectangle">RectangleF structure that specifies the bounds of the text.</param>       
        public void Draw(PdfGraphics graphics, RectangleF layoutRectangle)
        {
            if (graphics == null)
            {
                throw new ArgumentNullException("graphics");
            }

            if (layoutRectangle.Height < 0)
            {
                throw new ArgumentNullException("height");
            }

            RichTextBoxExt richtxtBox = new RichTextBoxExt();
            richtxtBox.RenderHTML(m_htmlText, m_font, m_brush);
            richtxtBox.SelectAll();
            richtxtBox.SelectionAlignment = m_textAlign;

            richtxtBox.Width = (int)layoutRectangle.Width;

            PdfUnitConvertor convertor = new PdfUnitConvertor();
            float width = convertor.ConvertUnits(layoutRectangle.Width, PdfGraphicsUnit.Point, PdfGraphicsUnit.Pixel);
            float height = convertor.ConvertUnits(layoutRectangle.Height, PdfGraphicsUnit.Point, PdfGraphicsUnit.Pixel);

            Image image = RtfToImage.ConvertToMetafile(richtxtBox, width, height);
            PdfImage img = PdfImage.FromImage(image);

            richtxtBox.Dispose();
            img.Draw(graphics,new PointF(layoutRectangle.X, layoutRectangle.Y));
        }

        /// <summary>
        /// Draws the text on the page.
        /// </summary>
        /// <param name="page">Current page where the text should be drawn.</param>
        /// <param name="location">Start location on the page.</param>
        /// <param name="width">Width of the text bounds.</param>
        /// <param name="format">Layout format.</param>
        /// <returns>Lay outing result.</returns>
        public PdfLayoutResult Draw(PdfPage page, PointF location, float width, PdfMetafileLayoutFormat format)
        {
            if (page == null)
            {
                throw new ArgumentNullException("page");
            }
            PdfUnitConvertor convertor = new PdfUnitConvertor();
            width = convertor.ConvertToPixels(width, PdfGraphicsUnit.Point);
            RichTextBoxExt richtxtBox = new RichTextBoxExt();
            richtxtBox.RenderHTML(m_htmlText, m_font, m_brush);
            richtxtBox.SelectAll();
            richtxtBox.SelectionAlignment = m_textAlign;
            Image image = RtfToImage.ConvertToImage(richtxtBox.Rtf, width, -1, PdfImageType.Metafile);
            RectangleF layoutRectangle = new RectangleF(location, new SizeF(convertor.ConvertFromPixels(width, PdfGraphicsUnit.Point), convertor.ConvertFromPixels(image.Height, PdfGraphicsUnit.Point)));
            richtxtBox.Dispose();
            return Draw(page, layoutRectangle, format);
        }

        /// <summary>
        /// Draws the text on the graphics.
        /// </summary>
        /// <param name="page">Graphics context where the text should be drawn.</param>
        /// <param name="location">Start location on the page.</param>
        /// <param name="width">Width of the text bounds.</param>
        /// <param name="height">Height of the text bounds.</param>      
        public void Draw(PdfGraphics graphics, PointF location, float width, float height)
        {
            RectangleF layoutRectangle = new RectangleF(location, new SizeF(width, height));
            Draw(graphics, layoutRectangle);
        }

        /// <summary>
        /// Draws the text on the page.
        /// </summary>
        /// <param name="page">Current page where the text should be drawn.</param>
        /// <param name="location">Start location on the page.</param>
        /// <param name="width">Width of the text bounds.</param>
        /// <param name="height">Height of the text bounds.</param>
        /// <param name="format">Lay outing format.</param>
        /// <returns>Lay outing result.</returns>
        public PdfLayoutResult Draw(PdfPage page, PointF location, float width, float height, PdfMetafileLayoutFormat format)
        {
            RectangleF layoutRectangle = new RectangleF(location, new SizeF(width, height));
            return Draw(page, layoutRectangle, format);
        }
        #endregion
    }
}
#endif