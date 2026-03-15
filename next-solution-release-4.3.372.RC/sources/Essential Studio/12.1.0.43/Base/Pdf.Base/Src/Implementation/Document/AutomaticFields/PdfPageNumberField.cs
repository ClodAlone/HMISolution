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
    /// Represents page number field.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// // Creates a new document
    /// PdfDocument doc = new PdfDocument();         
    /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f);            
    /// // Creates page number field
    /// PdfPageNumberField pageNumber = new PdfPageNumberField(font);
    /// pageNumber.NumberStyle = PdfNumberStyle.UpperLatin;
    /// for (int i = 0; i < 2; i++)
    /// {
    /// PdfPage page = doc.Pages.Add();
    /// pageNumber.Draw(page.Graphics);
    /// }
    /// doc.Save("PageNumberField.pdf");
    /// </code>
    /// <code lang="VB">
    /// ' Creates a new document
    /// Dim doc As PdfDocument = New PdfDocument()
    /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f)
    /// ' Creates page number field
    /// Dim pageNumber As PdfPageNumberField = New PdfPageNumberField(font)
    /// pageNumber.NumberStyle = PdfNumberStyle.UpperLatin
    /// For i As Integer = 0 To 1
    ///   Dim page As PdfPage = doc.Pages.Add()
    ///   pageNumber.Draw(page.Graphics)
    /// Next i
    /// doc.Save("PageNumberField.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfMultipleNumberValueField"/> Class    
    public class PdfPageNumberField : PdfMultipleNumberValueField
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfPageNumberField"/> class.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();                   
        /// // Creates page number field
        /// PdfPageNumberField pageNumber = new PdfPageNumberField();
        /// pageNumber.NumberStyle = PdfNumberStyle.UpperLatin;
        /// for (int i = 0; i < 2; i++)
        /// {
        /// PdfPage page = doc.Pages.Add();
        /// pageNumber.Draw(page.Graphics);
        /// }
        /// doc.Save("PageNumberField.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()     
        /// ' Creates page number field
        /// Dim pageNumber As PdfPageNumberField = New PdfPageNumberField()
        /// pageNumber.NumberStyle = PdfNumberStyle.UpperLatin
        /// For i As Integer = 0 To 1
        ///   Dim page As PdfPage = doc.Pages.Add()
        ///   pageNumber.Draw(page.Graphics)
        /// Next i
        /// doc.Save("PageNumberField.pdf")
        /// </code>
        /// </example>
        public PdfPageNumberField()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfPageNumberField"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();         
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f);            
        /// // Creates page number field
        /// PdfPageNumberField pageNumber = new PdfPageNumberField(font);
        /// pageNumber.NumberStyle = PdfNumberStyle.UpperLatin;
        /// for (int i = 0; i < 2; i++)
        /// {
        /// PdfPage page = doc.Pages.Add();
        /// pageNumber.Draw(page.Graphics);
        /// }
        /// doc.Save("PageNumberField.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f)
        /// ' Creates page number field
        /// Dim pageNumber As PdfPageNumberField = New PdfPageNumberField(font)
        /// pageNumber.NumberStyle = PdfNumberStyle.UpperLatin
        /// For i As Integer = 0 To 1
        ///   Dim page As PdfPage = doc.Pages.Add()
        ///   pageNumber.Draw(page.Graphics)
        /// Next i
        /// doc.Save("PageNumberField.pdf")
        /// </code>
        /// </example>
        public PdfPageNumberField(PdfFont font)
            : base(font)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfPageNumberField"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="brush">The brush.</param>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();         
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f);            
        /// // Creates page number field
        /// PdfPageNumberField pageNumber = new PdfPageNumberField(font, PdfBrushes.Beige);
        /// pageNumber.NumberStyle = PdfNumberStyle.UpperLatin;
        /// for (int i = 0; i < 2; i++)
        /// {
        /// PdfPage page = doc.Pages.Add();
        /// pageNumber.Draw(page.Graphics);
        /// }
        /// doc.Save("PageNumberField.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f)
        /// ' Creates page number field
        /// Dim pageNumber As PdfPageNumberField = New PdfPageNumberField(font, PdfBrushes.Beige)
        /// pageNumber.NumberStyle = PdfNumberStyle.UpperLatin
        /// For i As Integer = 0 To 1
        ///   Dim page As PdfPage = doc.Pages.Add()
        ///   pageNumber.Draw(page.Graphics)
        /// Next i
        /// doc.Save("PageNumberField.pdf")
        /// </code>
        /// </example>
        public PdfPageNumberField(PdfFont font, PdfBrush brush)
            : base(font, brush)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfPageNumberField"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="bounds">The bounds.</param>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();         
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f);            
        /// // Creates page number field
        /// PdfPageNumberField pageNumber = new PdfPageNumberField(font, new RectangleF(10, 10, 100, 200));
        /// pageNumber.NumberStyle = PdfNumberStyle.UpperLatin;
        /// for (int i = 0; i < 2; i++)
        /// {
        /// PdfPage page = doc.Pages.Add();
        /// pageNumber.Draw(page.Graphics);
        /// }
        /// doc.Save("PageNumberField.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f)
        /// ' Creates page number field
        /// PdfPageNumberField pageNumber = new PdfPageNumberField(font, new RectangleF(10, 10, 100, 200));
        /// pageNumber.NumberStyle = PdfNumberStyle.UpperLatin
        /// For i As Integer = 0 To 1
        ///   Dim page As PdfPage = doc.Pages.Add()
        ///   pageNumber.Draw(page.Graphics)
        /// Next i
        /// doc.Save("PageNumberField.pdf")
        /// </code>
        /// </example>
        public PdfPageNumberField(PdfFont font, RectangleF bounds)
            : base(font, bounds)
        {
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
                    for (int i = 0; i < n; i++)
                    {
                        if (ldoc.Pages[i] is PdfPage)
                        {
                            PdfPage lpage = ldoc.Pages[i] as PdfPage;
                            if (lpage.Dictionary.Equals(graphics.Page.Dictionary))
                            {
                                int temp = i + 1;
                                result = temp.ToString();
                            }
                        }
                    }
                }
                else
                {
                    result = InternalGetValue(page);
                }
            }
            else if (graphics.Page is PdfLoadedPage)
            {
                PdfLoadedPage page = GetLoadedPageFromGraphics(graphics);
                result = InternalLoadedGetValue(page);
            }

            return result;
        }

        /// <summary>
        /// Internal method to get value of the field.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        protected string InternalGetValue(PdfPage page)
        {
            PdfDocument document = page.Section.Parent.Document;
            int pageIndex = document.Pages.IndexOf(page) + 1;
            return PdfNumbersConvertor.Convert(pageIndex, NumberStyle);
        }

        /// <summary>
        /// Internal method to get value of the field.
        /// </summary>
        /// <param name="page">The page</param>
        /// <returns></returns>
        protected string InternalLoadedGetValue(PdfLoadedPage page)
        {
            PdfLoadedDocument document = page.Document as PdfLoadedDocument;
            int pageIndex = document.Pages.IndexOf(page) + 1;
            return PdfNumbersConvertor.Convert(pageIndex, NumberStyle);
        }
        #endregion
    }
}
