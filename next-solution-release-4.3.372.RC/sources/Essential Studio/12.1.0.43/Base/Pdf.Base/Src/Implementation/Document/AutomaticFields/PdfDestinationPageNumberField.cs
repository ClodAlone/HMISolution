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
    /// Represents class which displays destination page's number.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// // Creates a new document
    /// PdfDocument doc = new PdfDocument();
    /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f);
    /// // Creates page number field
    /// PdfDestinationPageNumberField pageNumber = new PdfDestinationPageNumberField(font);                        
    /// for (int i = 0; i < 2; i++)
    /// {
    ///  PdfPage page = doc.Pages.Add();
    ///  // Draws the page number only on the second page
    ///  if (i == 1)
    ///  {
    ///    pageNumber.Page = page;
    ///    pageNumber.Draw(page.Graphics);
    ///  }
    /// }
    /// doc.Save("PageNumberField.pdf");
    /// </code>
    /// <code lang="VB">
    /// ' Creates a new document
    /// Dim doc As PdfDocument = New PdfDocument()
    /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f)
    /// ' Creates page number field
    /// Dim pageNumber As PdfDestinationPageNumberField = New PdfDestinationPageNumberField(font)
    /// For i As Integer = 0 To 1
    ///   Dim page As PdfPage = doc.Pages.Add()
    ///   ' Draws the page number only on the second page
    ///   If i = 1 Then
    ///     pageNumber.Page = page
    ///     pageNumber.Draw(page.Graphics)
    ///   End If
    /// Next i
    /// doc.Save("PageNumberField.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfPageNumberField"/> Class    
    public class PdfDestinationPageNumberField : PdfPageNumberField
    {
        #region Fields
        /// <summary>
        /// Internal variable to store destination page.
        /// </summary>
        private PdfPage m_page = null;

        /// <summary>
        /// Internal variable to store the Loaded Page.
        /// </summary>
        private PdfLoadedPage m_loadedPage = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfDestinationPageNumberField"/> class.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();
        /// // Creates page number field
        /// PdfDestinationPageNumberField pageNumber = new PdfDestinationPageNumberField();                        
        /// for (int i = 0; i < 2; i++)
        /// {
        ///  PdfPage page = doc.Pages.Add();
        ///  // Draws the page number only on the second page
        ///  if (i == 1)
        ///  {
        ///    pageNumber.Page = page;
        ///    pageNumber.Draw(page.Graphics);
        ///  }
        /// }
        /// doc.Save("PageNumberField.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Creates page number field
        /// Dim pageNumber As PdfDestinationPageNumberField = New PdfDestinationPageNumberField()
        /// For i As Integer = 0 To 1
        ///   Dim page As PdfPage = doc.Pages.Add()
        ///   ' Draws the page number only on the second page
        ///   If i = 1 Then
        ///     pageNumber.Page = page
        ///     pageNumber.Draw(page.Graphics)
        ///   End If
        /// Next i
        /// doc.Save("PageNumberField.pdf")
        /// </code>
        /// </example>
        public PdfDestinationPageNumberField()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfDestinationPageNumberField"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f);
        /// // Creates page number field
        /// PdfDestinationPageNumberField pageNumber = new PdfDestinationPageNumberField(font);                        
        /// for (int i = 0; i < 2; i++)
        /// {
        ///  PdfPage page = doc.Pages.Add();
        ///  // Draws the page number only on the second page
        ///  if (i == 1)
        ///  {
        ///    pageNumber.Page = page;
        ///    pageNumber.Draw(page.Graphics);
        ///  }
        /// }
        /// doc.Save("PageNumberField.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f)
        /// ' Creates page number field
        /// Dim pageNumber As PdfDestinationPageNumberField = New PdfDestinationPageNumberField(font)
        /// For i As Integer = 0 To 1
        ///   Dim page As PdfPage = doc.Pages.Add()
        ///   ' Draws the page number only on the second page
        ///   If i = 1 Then
        ///     pageNumber.Page = page
        ///     pageNumber.Draw(page.Graphics)
        ///   End If
        /// Next i
        /// doc.Save("PageNumberField.pdf")
        /// </code>
        /// </example>
        public PdfDestinationPageNumberField(PdfFont font)
            : base(font)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfDestinationPageNumberField"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="brush">The brush.</param>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f);
        /// // Creates page number field
        /// PdfDestinationPageNumberField pageNumber = new PdfDestinationPageNumberField(font, PdfBrushes.Azure);                        
        /// for (int i = 0; i < 2; i++)
        /// {
        ///  PdfPage page = doc.Pages.Add();
        ///  // Draws the page number only on the second page
        ///  if (i == 1)
        ///  {
        ///    pageNumber.Page = page;
        ///    pageNumber.Draw(page.Graphics);
        ///  }
        /// }
        /// doc.Save("PageNumberField.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f)
        /// ' Creates page number field
        /// Dim pageNumber As PdfDestinationPageNumberField = New PdfDestinationPageNumberField(font, PdfBrushes.Azure)
        /// For i As Integer = 0 To 1
        ///   Dim page As PdfPage = doc.Pages.Add()
        ///   ' Draws the page number only on the second page
        ///   If i = 1 Then
        ///     pageNumber.Page = page
        ///     pageNumber.Draw(page.Graphics)
        ///   End If
        /// Next i
        /// doc.Save("PageNumberField.pdf")
        /// </code>
        /// </example>
        public PdfDestinationPageNumberField(PdfFont font, PdfBrush brush)
            : base(font, brush)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfDestinationPageNumberField"/> class.
        /// </summary>
        /// <param name="font">A <see cref="PdfFont"/> object that specifies the font attributes (the family name, the size, and the style of the font) to use. </param>
        /// <param name="bounds">Specifies the location and size of the field.</param>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f);
        /// // Creates page number field
        /// PdfDestinationPageNumberField pageNumber = new PdfDestinationPageNumberField(font, new RectangleF(10, 10, 100, 200));                        
        /// for (int i = 0; i < 2; i++)
        /// {
        ///  PdfPage page = doc.Pages.Add();
        ///  // Draws the page number only on the second page
        ///  if (i == 1)
        ///  {
        ///    pageNumber.Page = page;
        ///    pageNumber.Draw(page.Graphics);
        ///  }
        /// }
        /// doc.Save("PageNumberField.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f)
        /// ' Creates page number field
        /// Dim pageNumber As PdfDestinationPageNumberField = New PdfDestinationPageNumberField(font, New RectangleF(10, 10,100,200))
        /// For i As Integer = 0 To 1
        ///   Dim page As PdfPage = doc.Pages.Add()
        ///   ' Draws the page number only on the second page
        ///   If i = 1 Then
        ///     pageNumber.Page = page
        ///     pageNumber.Draw(page.Graphics)
        ///   End If
        /// Next i
        /// doc.Save("PageNumberField.pdf")
        /// </code>
        /// </example>
        public PdfDestinationPageNumberField(PdfFont font, RectangleF bounds)
            : base(font, bounds)
        {
        }
        #endregion

        #region Properties

        /// <summary>
        /// Get and sets the PdfLoadedPage
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// // Loads an existing document
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SrcDocument.pdf");
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f);
        /// // Creates page number field
        /// PdfDestinationPageNumberField pageNumber = new PdfDestinationPageNumberField(font, new RectangleF(10, 10,100,200));                        
        /// for (int i = 0; i < doc.Pages.Count; i++)
        /// {
        ///    // Draws the page number only on the second page
        ///    if (i == 1)
        ///    {
        ///      pageNumber.LoadedPage = doc.Pages[1] as PdfLoadedPage;
        ///      pageNumber.Draw(doc.Pages[1].Graphics);
        ///    }
        /// }
        /// doc.Save("DestinationPageNumberField.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Loads an existing document
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SrcDocument.pdf")
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f)
        /// ' Creates page number field
        /// Dim pageNumber As PdfDestinationPageNumberField = New PdfDestinationPageNumberField(font, New RectangleF(10, 10,100,200))
        /// For i As Integer = 0 To doc.Pages.Count - 1
        ///   ' Draws the page number only on the second page
        ///   If i = 1 Then
        ///    pageNumber.LoadedPage = TryCast(doc.Pages(1), PdfLoadedPage)
        ///    pageNumber.Draw(doc.Pages(1).Graphics)
        ///   End If
        /// Next i
        /// doc.Save("DestinationPageNumberField.pdf")
        /// </code>
        /// </example>
        public PdfLoadedPage LoadedPage
        {
            get
            {
                return m_loadedPage;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Page");
                }

                m_loadedPage = value;
            }
        }

        /// <summary>
        /// Gets or sets the page.
        /// </summary>
        /// <value>The page.</value>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f);
        /// // Creates page number field
        /// PdfDestinationPageNumberField pageNumber = new PdfDestinationPageNumberField(font);                        
        /// for (int i = 0; i < 2; i++)
        /// {
        ///  PdfPage page = doc.Pages.Add();
        ///  // Draws the page number only on the second page
        ///  if (i == 1)
        ///  {
        ///    pageNumber.Page = page;
        ///    pageNumber.Draw(page.Graphics);
        ///  }
        /// }
        /// doc.Save("PageNumberField.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f)
        /// ' Creates page number field
        /// Dim pageNumber As PdfDestinationPageNumberField = New PdfDestinationPageNumberField(font)
        /// For i As Integer = 0 To 1
        ///   Dim page As PdfPage = doc.Pages.Add()
        ///   ' Draws the page number only on the second page
        ///   If i = 1 Then
        ///     pageNumber.Page = page
        ///     pageNumber.Draw(page.Graphics)
        ///   End If
        /// Next i
        /// doc.Save("PageNumberField.pdf")
        /// </code>
        /// </example>
        public PdfPage Page
        {
            get
            {
                return m_page;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Page");
                }

                m_page = value;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the value of the field at the specified graphics.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <returns></returns>
        protected internal override string GetValue(Syncfusion.Pdf.Graphics.PdfGraphics graphics)
        {
            if (m_loadedPage != null)
            {
                return InternalLoadedGetValue(m_loadedPage);
            }
            else
            {
                return InternalGetValue(m_page);
            }
        }
        #endregion
    }
}
