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
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents the text markup annotation.
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document.
    /// PdfPage page = document.Pages.Add();
    /// //Create pdf font and pdf font style .
    /// Font font = new Font("Calibri", 10, FontStyle.Bold);
    /// PdfFont pdfFont = new PdfTrueTypeFont(font, false);
    /// //Create a new pdfbrush.
    /// PdfBrush pdfBrush = new PdfSolidBrush(Color.Black);
    /// //Draw text in the new page.
    /// page.Graphics.DrawString("Text Markup Annotation Demo", pdfFont, pdfBrush, new PointF(150, 10));
    /// string markupText = "Text Markup";
    /// SizeF size = pdfFont.MeasureString(markupText);
    /// RectangleF rectangle = new RectangleF(175, 40, size.Width, size.Height);
    /// page.Graphics.DrawString(markupText, pdfFont, pdfBrush, rectangle);
    /// //Create a pdf textmarkup annotation .
    /// PdfTextMarkupAnnotation markupAnnotation = new PdfTextMarkupAnnotation("Markup annotation", "Markup annotation with highlight style", markupText, new PointF(175, 40), pdfFont);
    /// markupAnnotation.TextMarkupColor = new PdfColor(Color.BlueViolet);
    /// markupAnnotation.TextMarkupAnnotationType = PdfTextMarkupAnnotationType.Highlight;
    /// //Add this annotation to a new page.
    /// page.Annotations.Add(markupAnnotation);
    /// //Save the document to disk.
    /// document.Save("TextMarkup.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document.
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create pdffont and pdffont style .
    /// Dim pdfFont As Font = New Font("Calibri", 10, FontStyle.Bold)
    /// Dim pdfFont As PdfFont = New PdfTrueTypeFont(font, false)
    /// 'Create a new pdfbrush.
    /// Dim pdfBrush As PdfBrush = New PdfSolidBrush(Color.Black)
    /// Dim text As string  = "Text Markup"
    /// Dim size As SizeF  = pdfFont.MeasureString(text)
    /// 'Create a new rectangle.
    /// Dim rectangle As RectangleF = New RectangleF(175, 40, size.Width, size.Height)
    /// page.Graphics.DrawString(text, pdfFont, pdfBrush, rectangle)
    /// 'Create a new pdf textmarkup annotation .
    /// Dim  markupAnnotation As PdfTextMarkupAnnotation  = New PdfTextMarkupAnnotation("Markup annotation", "Markup annotation with highlight style", markupText, new PointF(175, 40), pdfFont)
    /// markupAnnotation.TextMarkupColor = New PdfColor(Color.BlueViolet)
    /// markupAnnotation.TextMarkupAnnotationType = PdfTextMarkupAnnotationType.Highlight
    /// 'Add this annotation to a new page.
    /// page.Annotations.Add(popupAnnotation)
    /// 'Save the document to disk.
    /// document.Save("TextMarkup.pdf")
    /// </code>
    /// </example> 
    public class PdfTextMarkupAnnotation : PdfAnnotation
    {
        #region Fields
        /// <summary>
        /// To specifying the TextMarkupAnnotationType .
        /// </summary>
        private PdfTextMarkupAnnotationType m_textMarkupAnnotationType;

        /// <summary>
        /// To specifying the QuadPoints .
        /// </summary>
        private int[] m_quadPoints = new int[8];

        /// <summary>
        /// To store the QuadPoints to the PdfArray
        /// </summary>
        private PdfArray m_points;

        /// <summary>
        /// To specifying the Text Markup Color .
        /// </summary>
        private PdfColor m_textMarkupColor;

        /// <summary>
        /// To specifying the Text Markup Annotation Title .
        /// </summary>
        private string m_text;

        /// <summary>
        /// To specifying the Text Size .
        /// </summary>
        private SizeF m_textSize;

        /// <summary>
        /// To specifying the Text Location .
        /// </summary>
        private PointF m_textPoint;

        /// <summary>
        /// To specifying the Text Font.
        /// </summary>
        private PdfFont m_font;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets TextMarkupAnnotationType .
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create pdffont and pdffont style .
        /// Font font = new Font("Calibri", 10, FontStyle.Bold);
        /// PdfFont pdfFont = new PdfTrueTypeFont(font, false);
        /// //Create a new pdf brush.
        /// PdfBrush pdfBrush = new PdfSolidBrush(Color.Black);
        /// page.Graphics.DrawString("Text Markup Annotation Demo", pdfFont, pdfBrush, new PointF(150, 10));
        /// string markupText = "Text Markup";
        /// SizeF size = pdfFont.MeasureString(markupText);
        /// RectangleF rectangle = new RectangleF(175, 40, size.Width, size.Height);
        /// page.Graphics.DrawString(markupText, pdfFont, pdfBrush, rectangle);
        /// //Create a pdf text markup annotation  .
        /// PdfTextMarkupAnnotation markupAnnotation = new PdfTextMarkupAnnotation("Markup annotation", "Markup annotation with highlight style", markupText, new PointF(175, 40), pdfFont);
        /// markupAnnotation.TextMarkupAnnotationType = PdfTextMarkupAnnotationType.Highlight;
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(markupAnnotation);
        /// //Save the document to disk.
        /// document.Save("TextMarkup.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create pdffont and pdffont style .
        /// Dim pdfFont As Font  = New Font("Calibri", 10, FontStyle.Bold)
        /// Dim pdfFont As PdfFont  = New PdfTrueTypeFont(font, false)
        /// 'Create a new pdf brush.
        /// Dim pdfBrush As PdfBrush  = New PdfSolidBrush(Color.Black)
        /// Dim text As string  = "Text Markup"
        /// Dim size As SizeF  = pdfFont.MeasureString(text)
        /// 'Create a new rectangle.
        /// Dim rectangle As RectangleF  = New RectangleF(175, 40, size.Width, size.Height)
        /// page.Graphics.DrawString(text, pdfFont, pdfBrush, rectangle)
        /// 'Create a pdf text markup annotation  .
        /// Dim  markupAnnotation As PdfTextMarkupAnnotation  = New PdfTextMarkupAnnotation("Markup annotation", "Markup annotation with highlight style", markupText, new PointF(175, 40), pdfFont)
        /// markupAnnotation.TextMarkupAnnotationType = PdfTextMarkupAnnotationType.Highlight
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(popupAnnotation)
        /// 'Save the document to disk.
        /// document.Save("TextMarkup.pdf")
        /// </code>
        /// </example> 
        public PdfTextMarkupAnnotationType TextMarkupAnnotationType
        {
            get
            {
                return this.m_textMarkupAnnotationType;
            }

            set
            {
                this.m_textMarkupAnnotationType = value;
            }
        }

        /// <summary>
        /// Gets or sets text markup color.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create pdffont and pdffont style .
        /// Font font = new Font("Calibri", 10, FontStyle.Bold);
        /// PdfFont pdfFont = new PdfTrueTypeFont(font, false);
        /// //Create a new pdfbrush.
        /// PdfBrush pdfBrush = new PdfSolidBrush(Color.Black);
        /// page.Graphics.DrawString("Text Markup Annotation Demo", pdfFont, pdfBrush, new PointF(150, 10));
        /// string markupText = "Text Markup";
        /// SizeF size = pdfFont.MeasureString(markupText);
        /// RectangleF rectangle = new RectangleF(175, 40, size.Width, size.Height);
        /// page.Graphics.DrawString(markupText, pdfFont, pdfBrush, rectangle);
        /// //Create a pdf text markup annotation  .
        /// PdfTextMarkupAnnotation markupAnnotation = new PdfTextMarkupAnnotation("Markup annotation", "Markup annotation with highlight style", markupText, new PointF(175, 40), pdfFont);
        /// markupAnnotation.TextMarkupColor = new PdfColor(Color.BlueViolet);
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(markupAnnotation);
        /// //Save the document to disk.
        /// document.Save("TextMarkup.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create pdffont and pdffont style .
        /// Dim pdfFont As Font  = New Font("Calibri", 10, FontStyle.Bold)
        /// Dim pdfFont As PdfFont  = New PdfTrueTypeFont(font, false)
        /// 'Create a new pdfbrush.
        /// Dim pdfBrush As PdfBrush  = New PdfSolidBrush(Color.Black)
        /// Dim text As string  = "Text Markup"
        /// Dim size As SizeF  = pdfFont.MeasureString(text)
        /// 'Create a new rectangle.
        /// Dim rectangle As RectangleF  = New RectangleF(175, 40, size.Width, size.Height)
        /// page.Graphics.DrawString(text, pdfFont, pdfBrush, rectangle)
        /// 'Create a pdf text markup annotation  .
        /// Dim  markupAnnotation As PdfTextMarkupAnnotation  = New PdfTextMarkupAnnotation("Markup annotation", "Markup annotation with highlight style", markupText, new PointF(175, 40), pdfFont)
        /// markupAnnotation.TextMarkupColor = New PdfColor(Color.BlueViolet)
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(popupAnnotation)
        /// 'Save the document to disk.
        /// document.Save("TextMarkup.pdf")
        /// </code>
        /// </example> 
        public PdfColor TextMarkupColor
        {
            get
            {
                return this.m_textMarkupColor;
            }

            set
            {
                this.m_textMarkupColor = value;
            }
        }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes new instance of <see cref="PdfTextMarkupAnnotation"/> class.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create pdffont and pdffont style .
        /// Font font = new Font("Calibri", 10, FontStyle.Bold);
        /// PdfFont pdfFont = new PdfTrueTypeFont(font, false);
        /// //Create a new pdfbrush.
        /// PdfBrush pdfBrush = new PdfSolidBrush(Color.Black);
        /// page.Graphics.DrawString("Text Markup Annotation Demo", pdfFont, pdfBrush, new PointF(150, 10));
        /// string markupText = "Text Markup";
        /// SizeF size = pdfFont.MeasureString(markupText);
        /// RectangleF rectangle = new RectangleF(175, 40, size.Width, size.Height);
        /// page.Graphics.DrawString(markupText, pdfFont, pdfBrush, rectangle);
        /// //Create a pdf text markup annotation  .
        /// PdfTextMarkupAnnotation markupAnnotation = new PdfTextMarkupAnnotation("Markup annotation", "Markup annotation with highlight style", markupText, new PointF(175, 40), pdfFont);
        /// markupAnnotation.TextMarkupColor = new PdfColor(Color.BlueViolet);
        /// markupAnnotation.TextMarkupAnnotationType = PdfTextMarkupAnnotationType.Highlight;
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(markupAnnotation);
        /// //Save the document to disk.
        /// document.Save("TextMarkup.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create pdffont and pdffont style .
        /// Dim pdfFont As Font = New Font("Calibri", 10, FontStyle.Bold)
        /// Dim pdfFont As PdfFont = New PdfTrueTypeFont(font, false)
        /// 'Create a new pdfbrush.
        /// Dim pdfBrush As PdfBrush = New PdfSolidBrush(Color.Black)
        /// Dim text As string = "Text Markup"
        /// Dim size As SizeF = pdfFont.MeasureString(text)
        /// 'Create a new rectangle.
        /// Dim rectangle As RectangleF = New RectangleF(175, 40, size.Width, size.Height)
        /// page.Graphics.DrawString(text, pdfFont, pdfBrush, rectangle)
        /// 'Create a pdf text markup annotation  .
        /// Dim  markupAnnotation As PdfTextMarkupAnnotation  = New PdfTextMarkupAnnotation()
        /// markupAnnotation.TextMarkupColor = New PdfColor(Color.BlueViolet)
        /// markupAnnotation.TextMarkupAnnotationType = PdfTextMarkupAnnotationType.Highlight
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(popupAnnotation)
        /// 'Save the document to disk.
        /// document.Save("TextMarkup.pdf")
        /// </code>
        /// </example> 
        public PdfTextMarkupAnnotation()
            : base()
        {
        }

        /// <summary>
        /// Initializes new instance of <see cref="PdfTextMarkupAnnotation"/> class.
        /// </summary>
        /// <param name="markupTitle">The markup annotation title.</param>
        /// <param name="text">The string specifies the text of the annotation.</param>
        /// <param name="markupText">The string specifies the markup text of the annotation.</param>
        /// <param name="point">The location of the markup text annotation.</param>
        /// <param name="pdfFont">The <see cref="PdfFont"/> specifies the text appearance of the markup text annotation.</param>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create pdffont and pdffont style .
        /// Font font = new Font("Calibri", 10, FontStyle.Bold);
        /// PdfFont pdfFont = new PdfTrueTypeFont(font, false);
        /// //Create a new pdfbrush .
        /// PdfBrush pdfBrush = new PdfSolidBrush(Color.Black);
        /// page.Graphics.DrawString("Text Markup Annotation Demo", pdfFont, pdfBrush, new PointF(150, 10));
        /// string markupText = "Text Markup";
        /// SizeF size = pdfFont.MeasureString(markupText);
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(175, 40, size.Width, size.Height);
        /// page.Graphics.DrawString(markupText, pdfFont, pdfBrush, rectangle);
        /// //Create a pdf text markup annotation  .
        /// PdfTextMarkupAnnotation markupAnnotation = new PdfTextMarkupAnnotation();
        /// markupAnnotation.TextMarkupColor = new PdfColor(Color.BlueViolet);
        /// markupAnnotation.TextMarkupAnnotationType = PdfTextMarkupAnnotationType.Highlight;
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(markupAnnotation);
        /// //Save the document to disk.
        /// document.Save("TextMarkup.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create pdffont and pdffont style .
        /// Dim font As Font  = New Font("Calibri", 10, FontStyle.Bold)
        /// Dim pdfFont As PdfFont  = New PdfTrueTypeFont(font, false)
        /// 'Create a new pdfbrush .
        /// Dim pdfBrush As PdfBrush  = New PdfSolidBrush(Color.Black)
        /// Dim text As string  = "Text Markup"
        /// Dim size As SizeF  = pdfFont.MeasureString(text)
        /// 'Create a new rectangle.
        /// Dim rectangle As RectangleF  = New RectangleF(175, 40, size.Width, size.Height)
        /// page.Graphics.DrawString(text, pdfFont, pdfBrush, rectangle)
        /// 'Create a pdf text markup annotation .
        /// Dim  markupAnnotation As PdfTextMarkupAnnotation  = New PdfTextMarkupAnnotation("Markup annotation", "Markup annotation with highlight style", markupText, new PointF(175, 40), pdfFont)
        /// markupAnnotation.TextMarkupColor = New PdfColor(Color.BlueViolet)
        /// markupAnnotation.TextMarkupAnnotationType = PdfTextMarkupAnnotationType.Highlight
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(popupAnnotation)
        /// 'Save the document to disk.
        /// document.Save("TextMarkup.pdf")
        /// </code>
        /// </example> 
        public PdfTextMarkupAnnotation(string markupTitle, string text, string markupText, PointF point, PdfFont pdfFont)
            : base()
        {
            Text = text;
            this.m_text = markupTitle;
            this.m_font = pdfFont;
            this.Location = point;
            this.m_textSize = this.m_font.MeasureString(markupText);
            this.m_textPoint = point;
            this.m_textPoint.X += 25;
            this.m_textPoint.Y = 800 - this.m_textPoint.Y;
            this.Initialize();
        }

        /// <summary>
        /// Initializes new instance of <see cref="PdfTextMarkupAnnotation"/> class.
        /// </summary>
        /// <param name="rectangle">The bounds of the annotation.</param>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create pdffont and pdffont style .
        /// Font font = new Font("Calibri", 10, FontStyle.Bold);
        /// PdfFont pdfFont = new PdfTrueTypeFont(font, false);
        /// //Create a new pdfbrush .
        /// PdfBrush pdfBrush = new PdfSolidBrush(Color.Black);
        /// page.Graphics.DrawString("Text Markup Annotation Demo", pdfFont, pdfBrush, new PointF(150, 10));
        /// string markupText = "Text Markup";
        /// SizeF size = pdfFont.MeasureString(markupText);
        /// RectangleF rectangle = new RectangleF(175, 40, size.Width, size.Height);
        /// page.Graphics.DrawString(markupText, pdfFont, pdfBrush, rectangle);
        /// //Create a pdf text markup annotation  .
        /// PdfTextMarkupAnnotation markupAnnotation = new PdfTextMarkupAnnotation(rectangle);
        /// markupAnnotation.TextMarkupColor = new PdfColor(Color.BlueViolet);
        /// markupAnnotation.TextMarkupAnnotationType = PdfTextMarkupAnnotationType.Highlight;
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(markupAnnotation);
        /// //Save the document to disk.
        /// document.Save("TextMarkup.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create pdffont and pdffont style .
        /// Dim font As Font  = New Font("Calibri", 10, FontStyle.Bold)
        /// Dim pdfFont As PdfFont  = New PdfTrueTypeFont(font, false)
        /// 'Create a new pdfBrush.
        /// Dim pdfBrush As PdfBrush  = New PdfSolidBrush(Color.Black)
        /// Dim text As string  = "Text Markup"
        /// Dim size As SizeF  = pdfFont.MeasureString(text)
        /// Dim rectangle As RectangleF  = New RectangleF(175, 40, size.Width, size.Height)
        /// page.Graphics.DrawString(text, pdfFont, pdfBrush, rectangle)
        /// 'Create a pdf text markup annotation .
        /// Dim  markupAnnotation As PdfTextMarkupAnnotation  = New PdfTextMarkupAnnotation(rectangle)
        /// markupAnnotation.TextMarkupColor = New PdfColor(Color.BlueViolet)
        /// markupAnnotation.TextMarkupAnnotationType = PdfTextMarkupAnnotationType.Highlight
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(popupAnnotation)
        /// 'Save the document to disk.
        /// document.Save("TextMarkup.pdf")
        /// </code>
        /// </example> 
        public PdfTextMarkupAnnotation(RectangleF rectangle)
            : base(rectangle)
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Initializes Annotation object.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Saves an Text Markup Annotation .
        /// </summary>
        protected override void Save()
        {
            base.Save();
            PdfArray textMarkupColor = new PdfArray();
            if (!this.TextMarkupColor.IsEmpty)
            {
                float red = this.TextMarkupColor.R / 255f;
                float green = this.TextMarkupColor.G / 255f;
                float blue = this.TextMarkupColor.B / 255f;
                textMarkupColor.Insert(0, new PdfNumber(red));
                textMarkupColor.Insert(1, new PdfNumber(green));
                textMarkupColor.Insert(2, new PdfNumber(blue));
            }
            else
            {
                throw new Exception("TextMarkupColor is not null");
            }

            Dictionary.SetProperty(DictionaryProperties.Subtype, new PdfName(this.m_textMarkupAnnotationType));
            Dictionary.SetProperty(DictionaryProperties.QuadPoints, this.m_points);
            Dictionary.SetProperty(DictionaryProperties.C, textMarkupColor);
            Dictionary.SetString(DictionaryProperties.T, this.m_text);
            Dictionary.SetNumber(DictionaryProperties.CA, 0.50f);
        }
        internal void SetQuadPoints(SizeF pageSize)
        {
            float[] textQuadLocation = new float[8];
            float locationX = this.Location.X, locationY = this.Location.Y;
            float pageWidth = pageSize.Width, pageHeight = pageSize.Height;
            textQuadLocation[0] = locationX;
            textQuadLocation[1] = pageHeight - locationY;
            textQuadLocation[2] = locationX + this.m_textSize.Width;
            textQuadLocation[3] = pageHeight - locationY;
            textQuadLocation[4] = locationX;
            textQuadLocation[5] = textQuadLocation[1] - this.m_textSize.Height;
            textQuadLocation[6] = locationX + this.m_textSize.Width;
            textQuadLocation[7] = textQuadLocation[5];
            this.m_points = new PdfArray(textQuadLocation);
        }
        #endregion
    }
}
