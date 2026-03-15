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
    /// Represents automatic field to display
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// // Creates a new document
    /// PdfDocument doc = new PdfDocument();         
    /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f);            
    /// // Creates section number field
    /// PdfSectionNumberField sectionNumber = new PdfSectionNumberField(font);
    /// sectionNumber.NumberStyle = PdfNumberStyle.UpperLatin;
    /// for (int i = 0; i < 2; i++)
    /// {
    ///  PdfPage page = doc.Pages.Add();
    ///  sectionNumber.Draw(page.Graphics);
    /// }
    /// doc.Save("SectionNumberField.pdf");
    /// </code>
    /// <code lang="VB">
    /// ' Creates a new document
    /// Dim doc As PdfDocument = New PdfDocument()
    /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f)
    /// ' Creates section number field
    /// Dim sectionNumber As PdfSectionNumberField = New PdfSectionNumberField(font)
    /// sectionNumber.NumberStyle = PdfNumberStyle.UpperLatin
    /// For i As Integer = 0 To 1
    ///  Dim page As PdfPage = doc.Pages.Add()
    ///  sectionNumber.Draw(page.Graphics)
    /// Next i
    /// doc.Save("SectionNumberField.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfColorSpaces"/> Class    
    public class PdfSectionNumberField : PdfMultipleNumberValueField
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfSectionNumberField"/> class.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();         
        /// // Creates section number field
        /// PdfSectionNumberField sectionNumber = new PdfSectionNumberField();
        /// sectionNumber.NumberStyle = PdfNumberStyle.UpperLatin;
        /// for (int i = 0; i < 2; i++)
        /// {
        ///  PdfPage page = doc.Pages.Add();
        ///  sectionNumber.Draw(page.Graphics);
        /// }
        /// doc.Save("SectionNumberField.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()        
        /// ' Creates section number field
        /// Dim sectionNumber As PdfSectionNumberField = New PdfSectionNumberField()
        /// sectionNumber.NumberStyle = PdfNumberStyle.UpperLatin
        /// For i As Integer = 0 To 1
        ///  Dim page As PdfPage = doc.Pages.Add()
        ///  sectionNumber.Draw(page.Graphics)
        /// Next i
        /// doc.Save("SectionNumberField.pdf")
        /// </code>
        /// </example>
        public PdfSectionNumberField()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfSectionNumberField"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        public PdfSectionNumberField(PdfFont font)
            : base(font)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfSectionNumberField"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="brush">The brush.</param>
        public PdfSectionNumberField(PdfFont font, PdfBrush brush)
            : base(font, brush)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfSectionNumberField"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="bounds">The bounds.</param>
        public PdfSectionNumberField(PdfFont font, RectangleF bounds)
            : base(font, bounds)
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the value of the field at the specified graphics.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <returns>The value of the field.</returns>
        protected internal override string GetValue(PdfGraphics graphics)
        {
            string result = null;
            if (graphics.Page is PdfPage)
            {
                PdfPage page = graphics.Page as PdfPage;
                if (page.Section.m_document is PdfLoadedDocument)
                {
                    PdfReferenceHolder pageReference = page.Dictionary[DictionaryProperties.Parent] as PdfReferenceHolder;
                    PdfDictionary pageDictionary = page.Section.m_document.CrossTable.GetObject(pageReference) as PdfDictionary;
                    PdfLoadedDocument ldoc = page.Section.m_document as PdfLoadedDocument;
                    PdfDictionary dictionary = ldoc.Catalog as PdfDictionary;
                    PdfDictionary m_dictionary = ldoc.CrossTable.GetObject(dictionary[DictionaryProperties.Pages]) as PdfDictionary;
                    PdfArray m_sectionIndex = m_dictionary[DictionaryProperties.Kids] as PdfArray;

                    for (int i = 0; i < m_sectionIndex.Count; i++)
                    {
                        PdfReferenceHolder sectionReference = m_sectionIndex[i] as PdfReferenceHolder;
                        PdfDictionary sectionDictionary = sectionReference.Object as PdfDictionary;
                        if (sectionDictionary.Equals(pageDictionary))
                        {
                            int sectionIndex = i + 1;
                            result = PdfNumbersConvertor.Convert(sectionIndex, NumberStyle);
                        }
                    }
                }
                else
                {
                    PdfDocument document = page.Document;
                    int sectionIndex = document.Sections.IndexOf(page.Section) + 1;
                    result = PdfNumbersConvertor.Convert(sectionIndex, NumberStyle);
                }
            }
            else if (graphics.Page is PdfLoadedPage)
            {
                PdfLoadedPage page = graphics.Page as PdfLoadedPage;
                PdfLoadedDocument document = page.Document as PdfLoadedDocument;
                PdfDictionary dictionary = page.Document.Catalog as PdfDictionary;
                PdfDictionary pageDictionary = page.CrossTable.GetObject(dictionary[DictionaryProperties.Pages]) as PdfDictionary;
                PdfArray m_sectionIndex = pageDictionary[DictionaryProperties.Kids] as PdfArray;
                PdfDictionary p_dictionary = page.Dictionary as PdfDictionary;
                PdfReferenceHolder parentReference = p_dictionary[DictionaryProperties.Parent] as PdfReferenceHolder;
                int objnum = (int)parentReference.Reference.ObjNum;
                for (int i = 0; i < m_sectionIndex.Count; i++)
                {
                    PdfReferenceHolder sectionReference = m_sectionIndex[i] as PdfReferenceHolder;
                    if (sectionReference.Reference != null)
                    {
                        int m_objnum = (int)sectionReference.Reference.ObjNum;
                        if (m_objnum == objnum)
                        {
                            int sectionIndex = i + 1;
                            result = PdfNumbersConvertor.Convert(sectionIndex, NumberStyle);
                        }
                    }
                }
            }
            return result;
        }
        #endregion
    }
}
