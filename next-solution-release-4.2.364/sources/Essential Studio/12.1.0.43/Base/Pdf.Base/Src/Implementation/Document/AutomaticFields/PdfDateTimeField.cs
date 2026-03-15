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
    /// Represents date automated field.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// // Creates a new document
    /// PdfDocument doc = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = doc.Pages.Add();           
    /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f);    
    /// // Creates DateTime field
    /// PdfDateTimeField dateTimeField = new PdfDateTimeField(font);
    /// dateTimeField.DateFormatString = "dd'/'MMMM'/'yyyy";
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
    /// ' Creates DateTime field
    /// Dim dateTimeField As PdfDateTimeField = New PdfDateTimeField(font)
    /// dateTimeField.DateFormatString = "dd'/'MMMM'/'yyyy"
    /// For i As Integer = 0 To 2
    ///  page = doc.Pages.Add()
    ///  dateTimeField.Draw(page.Graphics)
    /// Next i
    /// doc.Save("DateTimeField.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfStaticField"/> Class    
    public class PdfDateTimeField : PdfStaticField
    {
        #region Fields
        /// <summary>
        /// Internal variable to store date value of the field.
        /// </summary>
        private DateTime m_date = DateTime.Now;

        /// <summary>
        /// Internal variable to store format of the date.
        /// </summary>
        private string m_formatString = "dd'/'MM'/'yyyy hh':'mm':'ss";
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfDateTimeField"/> class.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();                
        /// // Creates DateTime field
        /// PdfDateTimeField dateTimeField = new PdfDateTimeField();
        /// dateTimeField.DateFormatString = "dd'/'MMMM'/'yyyy";
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
        /// ' Creates DateTime field
        /// Dim dateTimeField As PdfDateTimeField = New PdfDateTimeField()
        /// dateTimeField.DateFormatString = "dd'/'MMMM'/'yyyy"
        /// For i As Integer = 0 To 2
        ///  page = doc.Pages.Add()
        ///  dateTimeField.Draw(page.Graphics)
        /// Next i
        /// doc.Save("DateTimeField.pdf")
        /// </code>
        /// </example>
        public PdfDateTimeField()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfDateTimeField"/> class.
        /// </summary>
        /// <param name="font">A <see cref="PdfFont"/> object that specifies the font attributes (the family name, the size, and the style of the font) to use. </param>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();           
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f);    
        /// // Creates DateTime field
        /// PdfDateTimeField dateTimeField = new PdfDateTimeField(font);
        /// dateTimeField.DateFormatString = "dd'/'MMMM'/'yyyy";
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
        /// ' Creates DateTime field
        /// Dim dateTimeField As PdfDateTimeField = New PdfDateTimeField(font)
        /// dateTimeField.DateFormatString = "dd'/'MMMM'/'yyyy"
        /// For i As Integer = 0 To 2
        ///  page = doc.Pages.Add()
        ///  dateTimeField.Draw(page.Graphics)
        /// Next i
        /// doc.Save("DateTimeField.pdf")
        /// </code>
        /// </example>
        public PdfDateTimeField(PdfFont font)
            : base(font)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfDateTimeField"/> class.
        /// </summary>
        /// <param name="font">A <see cref="PdfFont"/> object that specifies the font attributes (the family name, the size, and the style of the font) to use. </param>
        /// <param name="brush">A <see cref="PdfBrush"/> object that is used to fill the string. </param>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();           
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f);    
        /// // Creates DateTime field
        /// PdfDateTimeField dateTimeField = new PdfDateTimeField(font, PdfBrushes.Aquamarine);
        /// dateTimeField.DateFormatString = "dd'/'MMMM'/'yyyy";
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
        /// ' Creates DateTime field
        /// Dim dateTimeField As PdfDateTimeField = New PdfDateTimeField(font, PdfBrushes.Aquamarine)
        /// dateTimeField.DateFormatString = "dd'/'MMMM'/'yyyy"
        /// For i As Integer = 0 To 2
        ///  page = doc.Pages.Add()
        ///  dateTimeField.Draw(page.Graphics)
        /// Next i
        /// doc.Save("DateTimeField.pdf")
        /// </code>
        /// </example>
        public PdfDateTimeField(PdfFont font, PdfBrush brush)
            : base(font, brush)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfDateTimeField"/> class.
        /// </summary>
        /// <param name="font">A <see cref="PdfFont"/> object that specifies the font attributes (the family name, the size, and the style of the font) to use. </param>
        /// <param name="bounds">Specifies the location and size of the field.</param>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();           
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f);    
        /// // Creates DateTime field
        /// PdfDateTimeField dateTimeField = new PdfDateTimeField(font, new RectangleF(new PointF(10,10), new SizeF(100,200)));
        /// dateTimeField.DateFormatString = "dd'/'MMMM'/'yyyy";
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
        /// ' Creates DateTime field
        /// Dim dateTimeField As PdfDateTimeField = New PdfDateTimeField(font, New RectangleF(New PointF(10,10), New SizeF(100,200)))
        /// dateTimeField.DateFormatString = "dd'/'MMMM'/'yyyy"
        /// For i As Integer = 0 To 2
        ///  page = doc.Pages.Add()
        ///  dateTimeField.Draw(page.Graphics)
        /// Next i
        /// doc.Save("DateTimeField.pdf")
        /// </code>
        /// </example>
        public PdfDateTimeField(PdfFont font, RectangleF bounds)
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
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();           
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f);    
        /// // Creates DateTime field
        /// PdfDateTimeField dateTimeField = new PdfDateTimeField(font, PdfBrushes.Aquamarine);
        /// dateTimeField.DateFormatString = "dd'/'MMMM'/'yyyy";
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
        /// ' Creates DateTime field
        /// Dim dateTimeField As PdfDateTimeField = New PdfDateTimeField(font, PdfBrushes.Aquamarine)
        /// dateTimeField.DateFormatString = "dd'/'MMMM'/'yyyy"
        /// For i As Integer = 0 To 2
        ///  page = doc.Pages.Add()
        ///  dateTimeField.Draw(page.Graphics)
        /// Next i
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
        /// Get the value of the field at the specified graphics.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <returns>value</returns>
        /// <exclude/>
        internal protected override string GetValue(PdfGraphics graphics)
        {
            string value = m_date.ToString(m_formatString);
            return value;
        }
        #endregion
    }
}
