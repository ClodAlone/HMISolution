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
    /// Represent automatic field which contains document's author name.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// // Creates a new document
    /// PdfDocument doc = new PdfDocument();
    /// // Set the document`s information
    /// doc.DocumentInformation.Author = "Syncfusion";
    /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f);
    /// PdfBrush brush = PdfBrushes.Black;
    /// PdfDocumentAuthorField documentAuthorField = new PdfDocumentAuthorField(font);
    /// for (int i = 0; i < 2; i++)    
    /// {
    ///  PdfPage page = doc.Pages.Add();
    ///  documentAuthorField.Draw(page.Graphics);
    /// }
    /// doc.Save("DocumentAuthorField.pdf");
    /// </code>
    /// <code lang="VB">
    /// ' Creates a new document
    /// Dim doc As PdfDocument = New PdfDocument()
    /// ' Set the document`s information
    /// doc.DocumentInformation.Author = "Syncfusion"
    /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f)
    /// Dim brush As PdfBrush = PdfBrushes.Black
    /// Dim documentAuthorField As PdfDocumentAuthorField = New PdfDocumentAuthorField(font)  
    /// For i As Integer = 0 To 1
    ///  Dim page As PdfPage = doc.Pages.Add()
    ///  documentAuthorField.Draw(page.Graphics)
    /// Next i
    /// doc.Save("DocumentAuthorField.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfSingleValueField"/> Class    
    public class PdfDocumentAuthorField : PdfSingleValueField
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfDocumentAuthorField"/> class.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();
        /// // Set the document`s information
        /// doc.DocumentInformation.Author = "Syncfusion";      
        /// PdfDocumentAuthorField documentAuthorField = new PdfDocumentAuthorField();            
        /// for (int i = 0; i < 2; i++)
        /// {
        ///   PdfPage page = doc.Pages.Add();
        ///   documentAuthorField.Draw(page.Graphics);
        /// }
        /// doc.Save("DocumentAuthorField.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Set the document`s information
        /// doc.DocumentInformation.Author = "Syncfusion"
        /// Dim documentAuthorField As PdfDocumentAuthorField = New PdfDocumentAuthorField()
        /// For i As Integer = 0 To 1
        ///  Dim page As PdfPage = doc.Pages.Add()
        ///  documentAuthorField.Draw(page.Graphics)
        /// Next i
        /// doc.Save("DocumentAuthorField.pdf")
        /// </code>
        /// </example>
        public PdfDocumentAuthorField()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfDocumentAuthorField"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();
        /// // Set the document`s information
        /// doc.DocumentInformation.Author = "Syncfusion";
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f);
        /// PdfBrush brush = PdfBrushes.Black;
        /// PdfDocumentAuthorField documentAuthorField = new PdfDocumentAuthorField(font);
        /// for (int i = 0; i < 2; i++)    
        /// {
        ///  PdfPage page = doc.Pages.Add();
        ///  documentAuthorField.Draw(page.Graphics);
        /// }
        /// doc.Save("DocumentAuthorField.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Set the document`s information
        /// doc.DocumentInformation.Author = "Syncfusion"
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f)
        /// Dim brush As PdfBrush = PdfBrushes.Black
        /// Dim documentAuthorField As PdfDocumentAuthorField = New PdfDocumentAuthorField(font)  
        /// For i As Integer = 0 To 1
        ///  Dim page As PdfPage = doc.Pages.Add()
        ///  documentAuthorField.Draw(page.Graphics)
        /// Next i
        /// doc.Save("DocumentAuthorField.pdf")
        /// </code>
        /// </example>
        public PdfDocumentAuthorField(PdfFont font)
            : base(font)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfDocumentAuthorField"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="brush">The brush.</param>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();
        /// // Set the document`s information
        /// doc.DocumentInformation.Author = "Syncfusion";
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f);
        /// PdfBrush brush = PdfBrushes.Black;
        /// PdfDocumentAuthorField documentAuthorField = new PdfDocumentAuthorField(font, brush);
        /// for (int i = 0; i < 2; i++)    
        /// {
        ///  PdfPage page = doc.Pages.Add();
        ///  documentAuthorField.Draw(page.Graphics);
        /// }
        /// doc.Save("DocumentAuthorField.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Set the document`s information
        /// doc.DocumentInformation.Author = "Syncfusion"
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f)
        /// Dim brush As PdfBrush = PdfBrushes.Black
        /// Dim documentAuthorField As PdfDocumentAuthorField = New PdfDocumentAuthorField(font, brush)  
        /// For i As Integer = 0 To 1
        ///  Dim page As PdfPage = doc.Pages.Add()
        ///  documentAuthorField.Draw(page.Graphics)
        /// Next i
        /// doc.Save("DocumentAuthorField.pdf")
        /// </code>
        /// </example>
        public PdfDocumentAuthorField(PdfFont font, PdfBrush brush)
            : base(font, brush)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfDocumentAuthorField"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="bounds">The bounds.</param>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();
        /// // Set the document`s information
        /// doc.DocumentInformation.Author = "Syncfusion";
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f);
        /// PdfBrush brush = PdfBrushes.Black;
        /// PdfDocumentAuthorField documentAuthorField = new PdfDocumentAuthorField(font, new RectangleF(0, 0, 100, 200));            
        /// for (int i = 0; i < 2; i++)    
        /// {
        ///  PdfPage page = doc.Pages.Add();
        ///  documentAuthorField.Draw(page.Graphics);
        /// }
        /// doc.Save("DocumentAuthorField.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Set the document`s information
        /// doc.DocumentInformation.Author = "Syncfusion"
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f)
        /// Dim brush As PdfBrush = PdfBrushes.Black
        /// Dim documentAuthorField As PdfDocumentAuthorField = New PdfDocumentAuthorField(font, New RectangleF(0,0,100,200))
        /// For i As Integer = 0 To 1
        ///  Dim page As PdfPage = doc.Pages.Add()
        ///  documentAuthorField.Draw(page.Graphics)
        /// Next i
        /// doc.Save("DocumentAuthorField.pdf")
        /// </code>
        /// </example>
        public PdfDocumentAuthorField(PdfFont font, RectangleF bounds)
            : base(font, bounds)
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the value of the field at the specified graphics.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <returns></returns>
        /// <exclude/>
        internal protected override string GetValue(PdfGraphics graphics)
        {
            string value = null;
            if (graphics.Page is PdfPage)
            {
                PdfPage page = GetPageFromGraphics(graphics);
                value = page.Document.DocumentInformation.Author;
            }
            else if (graphics.Page is PdfLoadedPage)
            {
                PdfLoadedPage page = GetLoadedPageFromGraphics(graphics);
                value = page.Document.DocumentInformation.Author;
            }

            return value;
        }
        #endregion
    }
}
