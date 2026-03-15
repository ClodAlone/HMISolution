#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System.Collections.Generic;
using System.Drawing;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Parsing;

/// <summary>
/// The Syncfusion.Pdf namespace contains classes for creating PDF document.
/// </summary>
namespace Syncfusion.Pdf
{
    /// <summary>
    /// Represents automatic field which has the same value 
    /// in the whole document.
    /// </summary>
    /// <seealso cref="PdfDynamicField"/> Class    
    public abstract class PdfSingleValueField : PdfDynamicField
    {
        #region Fields
        /// <summary>
        /// Internal variable to store templates associated with the document.
        /// </summary>
        private Dictionary<PdfDocumentBase, PdfTemplateValuePair> m_list = new Dictionary<PdfDocumentBase, PdfTemplateValuePair>();

        /// <summary>
        /// Internal variable to array of graphics.
        /// </summary>
        private List<PdfGraphics> m_painterGraphics = new List<PdfGraphics>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfSingleValueField"/> class.
        /// </summary>
        public PdfSingleValueField()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfSingleValueField"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        public PdfSingleValueField(PdfFont font)
            : base(font)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfSingleValueField"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="brush">The brush.</param>
        public PdfSingleValueField(PdfFont font, PdfBrush brush)
            : base(font, brush)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfSingleValueField"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="bounds">The bounds.</param>
        public PdfSingleValueField(PdfFont font, RectangleF bounds)
            : base(font, bounds)
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Performs draw.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="location">The location.</param>
        /// <param name="scalingX">The scaling X.</param>
        /// <param name="scalingY">The scaling Y.</param>
        protected internal override void PerformDraw(PdfGraphics graphics, PointF location, float scalingX, float scalingY)
        {
            if (graphics.Page is PdfPage)
            {
                base.PerformDraw(graphics, location, scalingX, scalingY);

                PdfPage page = GetPageFromGraphics(graphics);
                if (page.Section.m_document is PdfLoadedDocument)
                {
                    PdfLoadedDocument document = page.Section.m_document as PdfLoadedDocument;
                    base.PerformDraw(graphics, location, scalingX, scalingY);

                    PdfPage currentPage = GetPageFromGraphics(graphics);
                    PdfLoadedDocument currentDocument = page.Section.m_document as PdfLoadedDocument;
                    string value = GetValue(graphics);

                    if (m_list.ContainsKey(currentDocument))
                    {
                        PdfTemplateValuePair pair = (PdfTemplateValuePair)m_list[document];

                        if (pair.Value != value)
                        {
                            SizeF size = GetSize();
                            pair.Template.Reset(size);
                            pair.Template.Graphics.DrawString(value, GetFont(), Pen, GetBrush(), new RectangleF(PointF.Empty, size), StringFormat);
                        }

                        if (!m_painterGraphics.Contains(graphics))
                        {
                            PointF drawLocation = new PointF(location.X + Location.X, location.Y + Location.Y);
                            graphics.DrawPdfTemplate(pair.Template, drawLocation, new SizeF(pair.Template.Width * scalingX, pair.Template.Height * scalingY));
                            m_painterGraphics.Add(graphics);
                        }
                    }
                    else
                    {
                        PdfTemplate template = new PdfTemplate(GetSize());
                        m_list[document] = new PdfTemplateValuePair(template, value);
                        template.Graphics.DrawString(value, GetFont(), Pen, GetBrush(), new RectangleF(PointF.Empty, GetSize()), StringFormat);
                        PointF drawLocation = new PointF(location.X + Location.X, location.Y + Location.Y);
                        graphics.DrawPdfTemplate(template, drawLocation, new SizeF(template.Width * scalingX, template.Height * scalingY));
                        m_painterGraphics.Add(graphics);
                    }
                }
                else
                {
                    PdfDocument document = page.Document;
                    string value = GetValue(graphics);

                    if (m_list.ContainsKey(document))
                    {
                        PdfTemplateValuePair pair = (PdfTemplateValuePair)m_list[document];

                        if (pair.Value != value)
                        {
                            SizeF size = GetSize();
                            pair.Template.Reset(size);
                            pair.Template.Graphics.DrawString(value, GetFont(), Pen, GetBrush(), new RectangleF(PointF.Empty, size), StringFormat);
                        }

                        if (!m_painterGraphics.Contains(graphics))
                        {
                            PointF drawLocation = new PointF(location.X + Location.X, location.Y + Location.Y);
                            graphics.DrawPdfTemplate(pair.Template, drawLocation, new SizeF(pair.Template.Width * scalingX, pair.Template.Height * scalingY));
                            m_painterGraphics.Add(graphics);
                        }
                    }
                    else
                    {
                        PdfTemplate template = new PdfTemplate(GetSize());
                        m_list[document] = new PdfTemplateValuePair(template, value);
                        template.Graphics.DrawString(value, GetFont(), Pen, GetBrush(), new RectangleF(PointF.Empty, GetSize()), StringFormat);
                        PointF drawLocation = new PointF(location.X + Location.X, location.Y + Location.Y);
                        graphics.DrawPdfTemplate(template, drawLocation, new SizeF(template.Width * scalingX, template.Height * scalingY));
                        m_painterGraphics.Add(graphics);
                    }
                }
            }
            else if (graphics.Page is PdfLoadedPage)
            {
                base.PerformDraw(graphics, location, scalingX, scalingY);

                PdfLoadedPage page = GetLoadedPageFromGraphics(graphics);
                PdfLoadedDocument document = page.Document as PdfLoadedDocument;
                string value = GetValue(graphics);

                if (m_list.ContainsKey(document))
                {
                    PdfTemplateValuePair pair = (PdfTemplateValuePair)m_list[document];

                    if (pair.Value != value)
                    {
                        SizeF size = GetSize();
                        pair.Template.Reset(size);
                        pair.Template.Graphics.DrawString(value, GetFont(), Pen, GetBrush(), new RectangleF(PointF.Empty, size), StringFormat);
                    }

                    if (!m_painterGraphics.Contains(graphics))
                    {
                        PointF drawLocation = new PointF(location.X + Location.X, location.Y + Location.Y);
                        graphics.DrawPdfTemplate(pair.Template, drawLocation, new SizeF(pair.Template.Width * scalingX, pair.Template.Height * scalingY));
                        m_painterGraphics.Add(graphics);
                    }
                }
                else
                {
                    PdfTemplate template = new PdfTemplate(GetSize());
                    m_list[document] = new PdfTemplateValuePair(template, value);
                    template.Graphics.DrawString(value, GetFont(), Pen, GetBrush(), new RectangleF(PointF.Empty, GetSize()), StringFormat);
                    PointF drawLocation = new PointF(location.X + Location.X, location.Y + Location.Y);
                    graphics.DrawPdfTemplate(template, drawLocation, new SizeF(template.Width * scalingX, template.Height * scalingY));
                    m_painterGraphics.Add(graphics);
                }
            }
        }
        #endregion
    }
}
