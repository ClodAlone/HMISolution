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
    /// Represents total page count automatic field.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// // Creates a new document
    /// PdfDocument doc = new PdfDocument();         
    /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f);
    /// PdfBrush brush = PdfBrushes.Black;
    /// // Creates page count field
    /// PdfPageCountField pageCount = new PdfPageCountField(font);
    /// pageCount.NumberStyle = PdfNumberStyle.Numeric;
    /// for (int i = 0; i < 2; i++)
    /// {
    ///  PdfPage page = doc.Pages.Add();
    ///  pageCount.Draw(page.Graphics);
    /// }
    /// doc.Save("PageCountField.pdf");
    /// </code>
    /// <code lang="VB">
    /// ' Creates a new document
    /// Dim doc As PdfDocument = New PdfDocument()
    /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f)
    /// Dim brush As PdfBrush = PdfBrushes.Black
    /// ' Creates page count field
    /// Dim pageCount As PdfPageCountField = New PdfPageCountField(font)
    /// pageCount.NumberStyle = PdfNumberStyle.Numeric
    /// For i As Integer = 0 To 1
    ///  Dim page As PdfPage = doc.Pages.Add()
    ///  pageCount.Draw(page.Graphics)
    /// Next i
    /// doc.Save("PageCountField.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfSingleValueField"/> Class    
    public class PdfPageCountField : PdfSingleValueField
    {
        #region Fields
        /// <summary>
        /// Internal variable to store numbering style.
        /// </summary>
        private PdfNumberStyle m_numberStyle = PdfNumberStyle.Numeric;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfPageCountField"/> class.
        /// </summary>
        public PdfPageCountField()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfPageCountField"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        public PdfPageCountField(PdfFont font)
            : base(font)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfPageCountField"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="brush">The brush.</param>
        public PdfPageCountField(PdfFont font, PdfBrush brush)
            : base(font, brush)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfPageCountField"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="bounds">The bounds.</param>
        public PdfPageCountField(PdfFont font, RectangleF bounds)
            : base(font, bounds)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the number style.
        /// </summary>
        /// <value>The number style.</value>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();         
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f);
        /// PdfBrush brush = PdfBrushes.Black;
        /// // Creates page count field
        /// PdfPageCountField pageCount = new PdfPageCountField(font);
        /// pageCount.NumberStyle = PdfNumberStyle.Numeric;
        /// for (int i = 0; i < 2; i++)
        /// {
        ///  PdfPage page = doc.Pages.Add();
        ///  pageCount.Draw(page.Graphics);
        /// }
        /// doc.Save("PageCountField.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f)
        /// Dim brush As PdfBrush = PdfBrushes.Black
        /// ' Creates page count field
        /// Dim pageCount As PdfPageCountField = New PdfPageCountField(font)
        /// pageCount.NumberStyle = PdfNumberStyle.Numeric
        /// For i As Integer = 0 To 1
        ///  Dim page As PdfPage = doc.Pages.Add()
        ///  pageCount.Draw(page.Graphics)
        /// Next i
        /// doc.Save("PageCountField.pdf")
        /// </code>
        /// </example>
        public PdfNumberStyle NumberStyle
        {
            get
            {
                return m_numberStyle;
            }

            set
            {
                m_numberStyle = value;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the value of the field at the specified graphics.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <returns>result</returns>
        internal protected override string GetValue(Syncfusion.Pdf.Graphics.PdfGraphics graphics)
        {
            string result = null;
            if (graphics.Page is PdfPage)
            {
                PdfPage page = GetPageFromGraphics(graphics);
                if (page.Section.m_document is PdfLoadedDocument)
                {
                    PdfLoadedDocument ldoc = page.Section.m_document as PdfLoadedDocument;
                    int n = (page.Section.m_document as PdfLoadedDocument).Pages.Count;
                    result = n.ToString();
                }
                else
                {
                    PdfDocument document = page.Section.Parent.Document;
                    int number = document.Pages.Count;
                    result = PdfNumbersConvertor.Convert(number, NumberStyle);
                }
            }
            else if (graphics.Page is PdfLoadedPage)
            {
                PdfLoadedPage page = GetLoadedPageFromGraphics(graphics);
                PdfLoadedDocument document = page.Document as PdfLoadedDocument;
                int number = document.Pages.Count;
                result = PdfNumbersConvertor.Convert(number, NumberStyle);
            }

            return result;
        }
        #endregion
    }
}
