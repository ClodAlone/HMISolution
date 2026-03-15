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
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf namespace contains classes for creating PDF document.
/// </summary>
namespace Syncfusion.Pdf
{
    /// <summary>
    /// Represents automatic field to display page number within a section.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// // Creates a new document
    /// PdfDocument doc = new PdfDocument();         
    /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f);            
    /// // Creates section page number field
    /// PdfSectionPageNumberField sectionPageNumber = new PdfSectionPageNumberField(font);
    /// sectionPageNumber.NumberStyle = PdfNumberStyle.LowerRoman;
    /// for (int i = 0; i < 2; i++)
    /// {
    ///   PdfPage page = doc.Pages.Add();
    ///   sectionPageNumber.Draw(page.Graphics);
    /// }
    /// doc.Save("sectionPageNumber.pdf");
    /// </code>
    /// <code lang="VB">
    /// ' Creates a new document
    /// Dim doc As PdfDocument = New PdfDocument()
    /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f)
    /// ' Creates section page number field
    /// Dim sectionPageNumber As PdfSectionPageNumberField = New PdfSectionPageNumberField(font)
    /// sectionPageNumber.NumberStyle = PdfNumberStyle.LowerRoman
    /// For i As Integer = 0 To 1
    ///  Dim page As PdfPage = doc.Pages.Add()
    ///  sectionPageNumber.Draw(page.Graphics)
    /// Next i
    /// doc.Save("sectionPageNumber.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfMultipleNumberValueField"/> Class    
    public class PdfSectionPageNumberField : PdfMultipleNumberValueField
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfSectionPageNumberField"/> class.
        /// </summary>
        public PdfSectionPageNumberField()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfSectionPageNumberField"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        public PdfSectionPageNumberField(PdfFont font)
            : base(font)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfSectionPageNumberField"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="brush">The brush.</param>
        public PdfSectionPageNumberField(PdfFont font, PdfBrush brush)
            : base(font, brush)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfSectionPageNumberField"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="bounds">The bounds.</param>
        public PdfSectionPageNumberField(PdfFont font, RectangleF bounds)
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
        internal protected override string GetValue(PdfGraphics graphics)
        {
            string result = null;
            if (graphics.Page is PdfPage)
            {
                PdfPage page = GetPageFromGraphics(graphics);
                PdfSection section = page.Section;
                int index = section.IndexOf(page) + 1;
                result = PdfNumbersConvertor.Convert(index, NumberStyle);
            }
            else if (graphics.Page is PdfLoadedPage)
            {
                PdfLoadedPage page = GetLoadedPageFromGraphics(graphics);
                PdfLoadedPage loadedPage = graphics.Page as PdfLoadedPage;
                PdfLoadedDocument document = loadedPage.Document as PdfLoadedDocument;
                PdfDictionary dictionary = loadedPage.Document.Catalog as PdfDictionary;
                PdfDictionary m_dictionary = loadedPage.CrossTable.GetObject(dictionary[DictionaryProperties.Pages]) as PdfDictionary;
                PdfArray kids = m_dictionary[DictionaryProperties.Kids] as PdfArray;
                for (int i = 0; i < kids.Count; i++)
                {
                    PdfReferenceHolder pageReference = new PdfReferenceHolder(loadedPage);
                    PdfReferenceHolder kidsReference = kids[i] as PdfReferenceHolder;
                    PdfDictionary kidsDictionary = kidsReference.Object as PdfDictionary;
                    string name = kidsDictionary[DictionaryProperties.Type].ToString();
                    if (name == "/Pages")
                    {
                        PdfArray temp = loadedPage.CrossTable.GetObject(kidsDictionary[DictionaryProperties.Kids]) as PdfArray;
                        for (int j = 0; j < temp.Count; j++)
                        {
                            PdfReferenceHolder currentKidsReference = temp[j] as PdfReferenceHolder;
                            if (pageReference.Object.Equals(currentKidsReference.Object))
                            {
                                int index = j + 1;
                                result = PdfNumbersConvertor.Convert(index, NumberStyle);
                            }
                        }
                    }
                }
            }

            return result;
        }
        #endregion
    }
}
