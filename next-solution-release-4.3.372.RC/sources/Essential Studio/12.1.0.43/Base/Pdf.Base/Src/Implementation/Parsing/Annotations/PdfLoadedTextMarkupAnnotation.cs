#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents the loaded text markup annotation class.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Load an existing document.
    /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
    /// //Gets the annotation from loaded document.
    /// PdfLoadedTextMarkupAnnotation textMarkupAnnotation = document.Pages[1].Annotations[5] as PdfLoadedTextMarkupAnnotation;
    /// //Sets the pdf text markup annotation type
    /// textMarkupAnnotation.TextMarkupAnnotationType=PdfTextMarkupAnnotationType.Highlight
    /// //Sets the text markup color
    /// textMarkupAnnotation.TextMarkupColor=new PdfColor(Color.Blue);
    /// //Save the document.
    /// document.Save("TextMarkupAnnotation.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Load an existing document.
    /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
    /// 'Gets the annotation from loaded document.
    /// Dim textMarkupAnnotation As PdfLoadedTextMarkupAnnotation = document.Pages(1).Annotations(5) as PdfLoadedTextMarkupAnnotation
    /// 'Sets the pdf text markup annotation type
    ///  textMarkupAnnotation.TextMarkupAnnotationType=PdfTextMarkupAnnotationType.Highlight
    /// 'Sets the text markup color
    /// textMarkupAnnotation.TextMarkupColor=New PdfColor(Color.Blue)
    /// 'Save the document.
    /// document.Save("TextMarkupAnnotation.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfLoadedAttachmentAnnotation"/> Class
    /// <seealso cref="PdfLoadedDocumentLinkAnnotation"/> Class
    /// <seealso cref="PdfLoadedFileLinkAnnotation"/> Class
    /// <seealso cref="PdfLoadedLineAnnotation"/> Class
    /// <seealso cref="PdfLoadedPopupAnnotation"/> Class
    /// <seealso cref="PdfLoadedRubberStampAnnotation"/> Class
    /// <seealso cref="PdfLoadedSoundAnnotation"/> Class
    /// <seealso cref="PdfLoadedTextWebLinkAnnotation"/> Class
    /// <seealso cref="PdfLoadedUriAnnotation"/> Class
    public class PdfLoadedTextMarkupAnnotation : PdfLoadedStyledAnnotation
    {
        #region Fields
        /// <summary>
        /// CrossTable
        /// </summary>
        private PdfCrossTable m_crossTable;
        /// <summary>
        /// Dictionary
        /// </summary>
        private PdfDictionary m_dictionary;
        /// <summary>
        /// Type of the annotation
        /// </summary>
        private PdfTextMarkupAnnotationType m_TextMarkupAnnotationType;
        /// <summary>
        /// Indicates the color.
        /// </summary>
        private PdfColor m_color;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the annotation Type.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
        /// //Gets the annotation from loaded document.
        /// PdfLoadedTextMarkupAnnotation textMarkupAnnotation = document.Pages[1].Annotations[5] as PdfLoadedTextMarkupAnnotation;
        /// //Sets the pdf text markup annotation type
        /// textMarkupAnnotation.TextMarkupAnnotationType=PdfTextMarkupAnnotationType.Highlight
        /// //Save the document.
        /// document.Save("TextMarkupAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document.
        /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
        /// 'Gets the annotation from loaded document.
        /// Dim textMarkupAnnotation As PdfLoadedTextMarkupAnnotation = document.Pages(1).Annotations(5) as PdfLoadedTextMarkupAnnotation
        /// 'Sets the pdf text markup annotation type
        /// textMarkupAnnotation.TextMarkupAnnotationType=PdfTextMarkupAnnotationType.Highlight
        /// 'Save the document.
        /// document.Save("TextMarkupAnnotation.pdf")
        /// </code>
        /// </example>
        public PdfTextMarkupAnnotationType TextMarkupAnnotationType
        {
            get
            {
                return GetTextMarkupAnnotationType();
            }
            set
            {
                m_TextMarkupAnnotationType = value;
                Dictionary.SetName(DictionaryProperties.Subtype, m_TextMarkupAnnotationType.ToString());
            }
        }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
        /// //Gets the annotation from loaded document.
        /// PdfLoadedTextMarkupAnnotation textMarkupAnnotation = document.Pages[1].Annotations[5] as PdfLoadedTextMarkupAnnotation;
        /// //Sets the text markup color
        /// textMarkupAnnotation.TextMarkupColor=new PdfColor(Color.Blue);
        /// //Save the document.
        /// document.Save("TextMarkupAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document.
        /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
        /// 'Gets the annotation from loaded document.
        /// Dim textMarkupAnnotation As PdfLoadedTextMarkupAnnotation = document.Pages(1).Annotations(5) as PdfLoadedTextMarkupAnnotation
        /// 'Sets the text markup color
        /// textMarkupAnnotation.TextMarkupColor=New PdfColor(Color.Blue)
        /// 'Save the document.
        /// document.Save("TextMarkupAnnotation.pdf")
        /// </code>
        /// </example>
        public PdfColor TextMarkupColor
        {
            get
            {
                return GetTextMarkupColor();
            }
            set
            {
                PdfArray markupcolor = new PdfArray();
                m_color = value;
                markupcolor.Insert(0, new PdfNumber(m_color.R / 255f));
                markupcolor.Insert(1, new PdfNumber(m_color.G / 255f));
                markupcolor.Insert(2, new PdfNumber(m_color.B / 255f));
                Dictionary.SetProperty(DictionaryProperties.C, markupcolor);
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLoadedLineAnnotation"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <param name="rectangle">The rectangle</param>
        internal PdfLoadedTextMarkupAnnotation(PdfDictionary dictionary, PdfCrossTable crossTable, RectangleF rectagle)
            : base(dictionary, crossTable)
        {
            m_dictionary = dictionary;
            m_crossTable = crossTable;
        }
        #endregion

        #region Implementations

        /// <summary>
        /// Sets the name of the field.
        /// </summary>
        /// <param name="name">New name of the field.</param>
        public void SetTitleText(string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            if (text == String.Empty)
                throw new ArgumentException("The text can't be empty");

            if (Text != text)
            {
                PdfString str = new PdfString(text);

                //BeforeTextChanges(text);
                Dictionary.SetString(DictionaryProperties.T, text);
                Changed = true;
            }
        }


        private PdfTextMarkupAnnotationType GetTextMarkupAnnotationType()
        {
            PdfName annotType = Dictionary[DictionaryProperties.Subtype] as PdfName;
            string aType = annotType.Value.ToString();
            return GetTextMarkupAnnotation(aType);
        }

        private PdfTextMarkupAnnotationType GetTextMarkupAnnotation(string aType)
        {
            PdfTextMarkupAnnotationType m_annotType = PdfTextMarkupAnnotationType.Highlight;
            switch (aType)
            {
                case "Highlight":
                    m_annotType = PdfTextMarkupAnnotationType.Highlight;
                    break;
                case "Squiggly":
                    m_annotType = PdfTextMarkupAnnotationType.Squiggly;
                    break;
                case "StrikeOut":
                    m_annotType = PdfTextMarkupAnnotationType.StrikeOut;
                    break;
                case "Underline":
                    m_annotType = PdfTextMarkupAnnotationType.Underline;
                    break;
            }
            return m_annotType;
        }

        /// <summary>
        /// Gets back color of the annotation.
        /// </summary>
        /// <returns>The back color.</returns>
        private PdfColor GetTextMarkupColor()
        {
            PdfColorSpace cs = PdfColorSpace.RGB;
            PdfColor color = PdfColor.Empty;
            PdfArray colours = null;
            if (Dictionary.ContainsKey(DictionaryProperties.C))
            {
                colours = Dictionary[DictionaryProperties.C] as PdfArray;
            }
            else
            {
                colours = color.ToArray(cs);
            }
            float red = (colours[0] as PdfNumber).FloatValue;
            float green = (colours[1] as PdfNumber).FloatValue;
            float blue = (colours[2] as PdfNumber).FloatValue;
            color = new PdfColor(red, green, blue);
            return color;
        }

        #endregion
    }
}
