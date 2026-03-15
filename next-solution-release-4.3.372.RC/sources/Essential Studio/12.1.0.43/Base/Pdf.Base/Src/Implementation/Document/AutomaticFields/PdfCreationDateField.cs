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
using Syncfusion.Pdf.Parsing;

/// <summary>
/// The Syncfusion.Pdf namespace contains classes for creating PDF document.
/// </summary>
namespace Syncfusion.Pdf
{
    /// <summary>
    /// Represents class to display creation date of the document.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// // Creates a new document
    /// PdfDocument doc = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = doc.Pages.Add();           
    /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f);
    /// PdfBrush brush = PdfBrushes.Black;
    /// // Creates Date time  field
    /// PdfCreationDateField dateTimeField = new PdfCreationDateField(font);      
    /// for (int i = 0; i < 3; i++)
    /// {
    ///  page = doc.Pages.Add();
    ///  dateTimeField.Draw(page.Graphics);
    /// }
    /// doc.Save("DateTimeField.pdf");
    /// </code>
    /// <code lang="VB">
    /// ' Creates a new document
    /// Dim doc As PdfDocument = New PdfDocument()
    /// ' Create a page
    /// Dim page As PdfPage = doc.Pages.Add()
    /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f)
    /// Dim brush As PdfBrush = PdfBrushes.Black
    /// ' Creates  Date time field
    /// Dim dateTimeField As PdfCreationDateField = New PdfCreationDateField(font)
    /// For i As Integer = 0 To 2
    ///  page = doc.Pages.Add()
    ///  dateTimeField.Draw(page.Graphics)
    /// Next i
    /// doc.Save("DateTimeField.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfSingleValueField"/> Class    
    public class PdfCreationDateField : PdfSingleValueField
    {
        #region Fields
        /// <summary>
        /// Internal variable to store format of the date.
        /// </summary>
        private string m_formatString = "dd'/'MM'/'yyyy";
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCreationDateField"/> class.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();                           
        /// // Creates Date time  field
        /// PdfCreationDateField dateTimeField = new PdfCreationDateField();      
        /// for (int i = 0; i < 3; i++)
        /// {
        ///  page = doc.Pages.Add();
        ///  dateTimeField.Draw(page.Graphics);
        /// }
        /// doc.Save("DateTimeField.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()     
        /// ' Creates  Date time field
        /// Dim dateTimeField As PdfCreationDateField = New PdfCreationDateField()
        /// For i As Integer = 0 To 2
        ///  page = doc.Pages.Add()
        ///  dateTimeField.Draw(page.Graphics)
        /// Next i
        /// doc.Save("DateTimeField.pdf")
        /// </code>
        /// </example>
        public PdfCreationDateField()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCreationDateField"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();           
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f);     
        /// // Creates Date time  field
        /// PdfCreationDateField dateTimeField = new PdfCreationDateField(font);      
        /// for (int i = 0; i < 3; i++)
        /// {
        ///  page = doc.Pages.Add();
        ///  dateTimeField.Draw(page.Graphics);
        /// }
        /// doc.Save("DateTimeField.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f)
        /// ' Creates  Date time field
        /// Dim dateTimeField As PdfCreationDateField = New PdfCreationDateField(font)
        /// For i As Integer = 0 To 2
        ///  page = doc.Pages.Add()
        ///  dateTimeField.Draw(page.Graphics)
        /// Next i
        /// doc.Save("DateTimeField.pdf")
        /// </code>
        /// </example>
        public PdfCreationDateField(PdfFont font)
            : base(font)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCreationDateField"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="brush">The brush.</param>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();           
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f);
        /// PdfBrush brush = PdfBrushes.Black;
        /// // Creates Date time  field
        /// PdfCreationDateField dateTimeField = new PdfCreationDateField(font, brush);      
        /// for (int i = 0; i < 3; i++)
        /// {
        ///  page = doc.Pages.Add();
        ///  dateTimeField.Draw(page.Graphics);
        /// }
        /// doc.Save("DateTimeField.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f)
        /// Dim brush As PdfBrush = PdfBrushes.Black
        /// ' Creates  Date time field
        /// Dim dateTimeField As PdfCreationDateField = New PdfCreationDateField(font, brush)
        /// For i As Integer = 0 To 2
        ///  page = doc.Pages.Add()
        ///  dateTimeField.Draw(page.Graphics)
        /// Next i
        /// doc.Save("DateTimeField.pdf")
        /// </code>
        /// </example>
        public PdfCreationDateField(PdfFont font, PdfBrush brush)
            : base(font, brush)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCreationDateField"/> class.
        /// </summary>
        /// <param name="font">A <see cref="PdfFont"/>object that specifies the font attributes (the family name, the size, and the style of the font) to use. </param>
        /// <param name="bounds">Specifies the location and size of the field.</param>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();               
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f);             
        /// // Creates Date time  field
        /// PdfCreationDateField dateTimeField = new PdfCreationDateField(font, new RectangleF(new PointF(10,10), new SizeF(100,200)));      
        /// for (int i = 0; i < 3; i++)
        /// {
        ///  //Creates a new page and adds it as the last page of the document
        ///  PdfPage page = doc.Pages.Add();        
        ///  dateTimeField.Draw(page.Graphics);
        /// }
        /// // Saves the document
        /// doc.Save("DateTimeField.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f)
        /// ' Creates Date time  field
        /// Dim dateTimeField As PdfCreationDateField = New PdfCreationDateField(font, New RectangleF(New PointF(10,10), New SizeF(100,200)))
        /// For i As Integer = 0 To 2
        ///  ' Create a page
        ///  Dim page As PdfPage = doc.Pages.Add()
        ///  dateTimeField.Draw(page.Graphics)
        /// Next i
        /// ' Saves the document
        /// doc.Save("DateTimeField.pdf")
        /// </code>
        /// </example>
        public PdfCreationDateField(PdfFont font, RectangleF bounds)
            : base(font, bounds)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the format string.
        /// </summary>
        /// <value>The format string.</value>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();               
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f);             
        /// // Creates Date time  field
        /// PdfCreationDateField dateTimeField = new PdfCreationDateField(font, new RectangleF(new PointF(10,10), new SizeF(100,200)));
        /// dateTimeField.DateFormatString = "dd'/'MMMM'/'yyyy";
        /// for (int i = 0; i < 3; i++)
        /// {
        ///  //Creates a new page and adds it as the last page of the document
        ///  PdfPage page = doc.Pages.Add();        
        ///  dateTimeField.Draw(page.Graphics);
        /// }
        /// // Saves the document
        /// doc.Save("DateTimeField.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f)
        /// ' Creates Date time  field
        /// Dim dateTimeField As PdfCreationDateField = New PdfCreationDateField(font, New RectangleF(New PointF(10,10), New SizeF(100,200)))
        /// dateTimeField.DateFormatString = "dd'/'MMMM'/'yyyy"
        /// For i As Integer = 0 To 2
        ///  ' Create a page
        ///  Dim page As PdfPage = doc.Pages.Add()
        ///  dateTimeField.Draw(page.Graphics)
        /// Next i
        /// ' Saves the document
        /// doc.Save("DateTimeField.pdf")
        /// </code>
        /// </example> 
        public string DateFormatString
        {
            get
            {
                return m_formatString;
            }

            set
            {
                m_formatString = value;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the value of the field at the specified graphics.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <returns></returns>
        /// <exclude/>
        protected internal override string GetValue(Syncfusion.Pdf.Graphics.PdfGraphics graphics)
        {
            string result = null;
            if (graphics.Page is PdfPage)
            {
                PdfPage page = GetPageFromGraphics(graphics);
                if (page.Section.m_document is PdfLoadedDocument)
                {
                    PdfLoadedDocument document = page.Section.m_document as PdfLoadedDocument;
                    DateTime creationDate = document.DocumentInformation.CreationDate;
                    result = creationDate.ToString(m_formatString);
                }
                else
                {
                    PdfDocument document = page.Document;
                    DateTime creationDate = document.DocumentInformation.CreationDate;
                    result = creationDate.ToString(m_formatString);
                }
            }
            else if (graphics.Page is PdfLoadedPage)
            {
                PdfLoadedPage page = GetLoadedPageFromGraphics(graphics);
                PdfLoadedDocument document = page.Document as PdfLoadedDocument;
                DateTime creationDate = document.DocumentInformation.CreationDate;
                result = creationDate.ToString(m_formatString);
            }
            return result;
        }
        #endregion
    }
}
