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
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents the class for text web link annotation.
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document.
    /// PdfPage page = document.Pages.Add();
    /// //Create a new solid brush
    /// PdfBrush brush = new PdfSolidBrush(Color.Black);
    /// //Set the font
    /// float fontSize = 10f;
    /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, fontSize);
    /// //Create a text weblink annotation
    /// PdfTextWebLink webLinkAnnotation = new PdfTextWebLink();
    /// webLinkAnnotation.Url = "http://www.yahoo.com";
    /// webLinkAnnotation.Text = "Yagoo Mail";
    /// webLinkAnnotation.Brush = brush;
    /// webLinkAnnotation.Font = font;
    /// webLinkAnnotation.Pen = PdfPens.Brown;
    /// webLinkAnnotation.DrawTextWebLink(page, new PointF(50, 40));
    /// page.Graphics.DrawString("Go to Yahoo Web Site", font, brush, new PointF(110, 40));
    /// //Add this annotation to a new page.
    /// page.Annotations.Add(webLinkAnnotation);
    /// //Save the document to disk.
    /// document.Save("TextWebLink(.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document.
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create a new solid brush
    /// Dim brush As PdfBrush  = New PdfSolidBrush(Color.Black)
    /// 'Set the font
    /// Dim fontSize As Single  = 10f
    /// Dim font As PdfFont  = New PdfStandardFont(PdfFontFamily.Helvetica, fontSize)
    /// 'Create a text weblink annotation
    /// Dim webLinkAnnotation As PdfTextWebLink  = New PdfTextWebLink()
    /// webLinkAnnotation.Url = "http://www.yahoo.com"
    /// webLinkAnnotation.Text = "Yagoo Mail"
    /// webLinkAnnotation.Brush = brush
    /// webLinkAnnotation.Font = font
    /// webLinkAnnotation.Pen = PdfPens.Brown
    /// webLinkAnnotation.DrawTextWebLink(page, new PointF(50, 40))
    /// page.Graphics.DrawString("Go to Yahoo Web Site", font, brush, new PointF(110, 40))
    /// 'Add this annotation to a new page.
    /// page.Annotations.Add(webLinkAnnotation)
    /// 'Save the document to disk.
    /// document.Save("TextWebLink(.pdf")
    /// </code>
    /// </example> 
    public class PdfTextWebLink : PdfTextElement
    {
        #region Fields
        /// <summary>
        /// Internal variable to store Url.
        /// </summary>
        private string m_url;

        /// <summary>
        /// Internal variable to store Uri Annotation object. 
        /// </summary>
        private PdfUriAnnotation m_uriAnnotation = null;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the Url address.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new pdf textweblink annotation
        /// PdfTextWebLink webLinkAnnotation = new PdfTextWebLink();
        /// webLinkAnnotation.Url = "http://www.yahoo.com";
        /// webLinkAnnotation.DrawTextWebLink(page, new PointF(50, 40));
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(webLinkAnnotation);
        /// //Save the document to disk.
        /// document.Save("TextWebLink(.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a text weblink annotation
        /// Dim webLinkAnnotation As PdfTextWebLink  = New PdfTextWebLink()
        /// webLinkAnnotation.Url = "http://www.yahoo.com"
        /// webLinkAnnotation.DrawTextWebLink(page, new PointF(50, 40))
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(webLinkAnnotation)
        /// 'Save the document to disk.
        /// document.Save("TextWebLink(.pdf")
        /// </code>
        /// </example> 
        public string Url
        {
            get
            {
                return this.m_url;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("url");
                }

                if (value.Length == 0)
                {
                    throw new ArgumentException("Url - string can not be empty");
                }

                this.m_url = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfTextWebLink"/> class.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new solid brush
        /// PdfBrush brush = new PdfSolidBrush(Color.Black);
        /// //Set the font
        /// float fontSize = 10f;
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, fontSize);
        /// //Create a text weblink annotation
        /// PdfTextWebLink webLinkAnnotation = new PdfTextWebLink();
        /// webLinkAnnotation.Url = "http://www.yahoo.com";
        /// webLinkAnnotation.Text = "Yagoo Mail";
        /// webLinkAnnotation.Brush = brush;
        /// webLinkAnnotation.Font = font;
        /// webLinkAnnotation.Pen = PdfPens.Brown;
        /// webLinkAnnotation.DrawTextWebLink(page, new PointF(50, 40));
        /// page.Graphics.DrawString("Go to Yahoo Web Site", font, brush, new PointF(110, 40));
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(webLinkAnnotation);
        /// //Save the document to disk.
        /// document.Save("TextWebLink(.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new solid brush
        /// Dim brush As PdfBrush  = New PdfSolidBrush(Color.Black)
        /// 'Set the font
        /// Dim fontSize As Single  = 10f
        /// Dim font As PdfFont  = New PdfStandardFont(PdfFontFamily.Helvetica, fontSize)
        /// 'Create a text weblink annotation
        /// Dim webLinkAnnotation As PdfTextWebLink  = New PdfTextWebLink()
        /// webLinkAnnotation.Url = "http://www.yahoo.com"
        /// webLinkAnnotation.Text = "Yagoo Mail"
        /// webLinkAnnotation.Brush = brush
        /// webLinkAnnotation.Font = font
        /// webLinkAnnotation.Pen = PdfPens.Brown
        /// webLinkAnnotation.DrawTextWebLink(page, new PointF(50, 40))
        /// page.Graphics.DrawString("Go to Yahoo Web Site", font, brush, new PointF(110, 40))
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(webLinkAnnotation)
        /// 'Save the document to disk.
        /// document.Save("TextWebLink(.pdf")
        /// </code>
        /// </example> 
        public PdfTextWebLink()
            : base()
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Draws a Text Web Link on the Page
        /// </summary>
        /// <param name="page">The page where the annotation should be placed.</param>
        /// <param name="location">The location of the annotation.</param>
        /// <returns>Pdf Layout result</returns>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new solid brush
        /// PdfBrush brush = new PdfSolidBrush(Color.Black);
        /// //Set the font
        /// float fontSize = 10f;
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, fontSize);
        /// //Create a text weblink annotation
        /// PdfTextWebLink webLinkAnnotation = new PdfTextWebLink();
        /// webLinkAnnotation.Url = "http://www.yahoo.com";
        /// webLinkAnnotation.Text = "Yagoo Mail";
        /// webLinkAnnotation.Brush = brush;
        /// webLinkAnnotation.Font = font;
        /// webLinkAnnotation.Pen = PdfPens.Brown;
        /// webLinkAnnotation.DrawTextWebLink(page, new PointF(50, 40));
        /// page.Graphics.DrawString("Go to Yahoo Web Site", font, brush, new PointF(110, 40));
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(webLinkAnnotation);
        /// //Save the document to disk.
        /// document.Save("TextWebLink(.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new solid brush
        /// Dim brush As PdfBrush  = New PdfSolidBrush(Color.Black)
        /// 'Set the font
        /// Dim fontSize As Single  = 10f
        /// Dim font As PdfFont  = New PdfStandardFont(PdfFontFamily.Helvetica, fontSize)
        /// 'Create a text weblink annotation
        /// Dim webLinkAnnotation As PdfTextWebLink  = New PdfTextWebLink()
        /// webLinkAnnotation.Url = "http://www.yahoo.com"
        /// webLinkAnnotation.Text = "Yagoo Mail"
        /// webLinkAnnotation.Brush = brush
        /// webLinkAnnotation.Font = font
        /// webLinkAnnotation.Pen = PdfPens.Brown
        /// webLinkAnnotation.DrawTextWebLink(page, new PointF(50, 40))
        /// page.Graphics.DrawString("Go to Yahoo Web Site", font, brush, new PointF(110, 40))
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(webLinkAnnotation)
        /// 'Save the document to disk.
        /// document.Save("TextWebLink(.pdf")
        /// </code>
        /// </example> 
        public PdfLayoutResult DrawTextWebLink(PdfPage page, PointF location)
        {
            SizeF textSize = Font.MeasureString(Value);
            RectangleF rect = new RectangleF(location, textSize);
            this.m_uriAnnotation = new PdfUriAnnotation(rect, this.Url);
            this.m_uriAnnotation.Border = new PdfAnnotationBorder(0f, 0f, 0f);
            page.Annotations.Add(m_uriAnnotation);
            return Draw(page, location);
        }

        /// <summary>
        /// Draw a Text Web Link on the Graphics
        /// </summary>
        /// <param name="g">The <see cref="PdfGraphics"/> object specifies where annotation should be placed..</param>
        /// <param name="location">The location of the annotation.</param>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new solid brush
        /// PdfBrush brush = new PdfSolidBrush(Color.Black);
        /// //Set the font
        /// float fontSize = 10f;
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, fontSize);
        /// //Create a text weblink annotation
        /// PdfTextWebLink webLinkAnnotation = new PdfTextWebLink();
        /// webLinkAnnotation.Url = "http://www.yahoo.com";
        /// webLinkAnnotation.Text = "Yagoo Mail";
        /// webLinkAnnotation.Brush = brush;
        /// webLinkAnnotation.Font = font;
        /// webLinkAnnotation.Pen = PdfPens.Brown;
        /// webLinkAnnotation.DrawTextWebLink(page.Graphics, new PointF(50, 40));
        /// page.Graphics.DrawString("Go to Yahoo Web Site", font, brush, new PointF(110, 40));
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(webLinkAnnotation);
        /// //Save the document to disk.
        /// document.Save("TextWebLink(.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new solid brush
        /// Dim brush As PdfBrush  = New PdfSolidBrush(Color.Black)
        /// 'Set the font
        /// Dim fontSize As Single  = 10f
        /// Dim font As PdfFont  = New PdfStandardFont(PdfFontFamily.Helvetica, fontSize)
        /// 'Create a text weblink annotation
        /// Dim webLinkAnnotation As PdfTextWebLink  = New PdfTextWebLink()
        /// webLinkAnnotation.Url = "http://www.yahoo.com"
        /// webLinkAnnotation.Text = "Yagoo Mail"
        /// webLinkAnnotation.Brush = brush
        /// webLinkAnnotation.Font = font
        /// webLinkAnnotation.Pen = PdfPens.Brown
        /// webLinkAnnotation.DrawTextWebLink(page.Graphics, new PointF(50, 40))
        /// page.Graphics.DrawString("Go to Yahoo Web Site", font, brush, new PointF(110, 40))
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(webLinkAnnotation)
        /// 'Save the document to disk.
        /// document.Save("TextWebLink(.pdf")
        /// </code>
        /// </example> 
        public void DrawTextWebLink(PdfGraphics graphics, PointF location)
        {
            if (graphics.Page is PdfLoadedPage)
            {
                SizeF textSize = Font.MeasureString(Value);
                RectangleF rect = new RectangleF(new PointF(location.X, (graphics.Page.Size.Height - (location.Y + 40))), textSize);
                //RectangleF rect = new RectangleF(location, textSize);
                this.m_uriAnnotation = new PdfUriAnnotation(rect, this.Url);
                this.m_uriAnnotation.Border = new PdfAnnotationBorder(0f, 0f, 0f);
                graphics.Page.Annotations.Add(this.m_uriAnnotation);
                Draw(graphics, location);
            }
            else
            {
                PdfPage page = new PdfPage();
                SizeF textSize = Font.MeasureString(Value);
                RectangleF rect = new RectangleF(location, textSize);
                this.m_uriAnnotation = new PdfUriAnnotation(rect, this.Url);
                this.m_uriAnnotation.Border = new PdfAnnotationBorder(0f, 0f, 0f);
                page = graphics.Page as PdfPage;
                
                page.Annotations.Add(this.m_uriAnnotation);
                Draw(graphics, location);
            }
        }
        #endregion
    }
}
