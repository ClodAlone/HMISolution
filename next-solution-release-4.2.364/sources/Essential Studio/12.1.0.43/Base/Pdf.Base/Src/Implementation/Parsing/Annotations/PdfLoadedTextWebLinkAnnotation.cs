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
using System.Text;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents the loaded text web link annotation class.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Load an existing document.
    /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
    /// //Gets the annotation from loaded document.
    /// PdfLoadedTextWebLinkAnnotation textWeblinkAnnotation = document.Pages[1].Annotations[5] as PdfLoadedTextWebLinkAnnotation;
    /// //Sets the text weblink annotation URI
    /// textWeblinkAnnotation.Url="http://www.syncfusion.com";
    /// //Save the document.
    /// document.Save("TextWebLinkAnnotation.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Load an existing document.
    /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
    /// 'Gets the annotation from loaded document.
    /// Dim textWeblinkAnnotation As PdfLoadedTextWebLinkAnnotation = document.Pages(1).Annotations(5) as PdfLoadedTextWebLinkAnnotation
    /// 'Sets the text weblink annotation URI
    /// textWeblinkAnnotation.Url="http://www.syncfusion.com"
    /// 'Save the document.
    /// document.Save("TextWebLinkAnnotation.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfLoadedAttachmentAnnotation"/> Class
    /// <seealso cref="PdfLoadedDocumentLinkAnnotation"/> Class
    /// <seealso cref="PdfLoadedFileLinkAnnotation"/> Class
    /// <seealso cref="PdfLoadedLineAnnotation"/> Class
    /// <seealso cref="PdfLoadedPopupAnnotation"/> Class
    /// <seealso cref="PdfLoadedRubberStampAnnotation"/> Class
    /// <seealso cref="PdfLoadedSoundAnnotation"/> Class
    /// <seealso cref="PdfLoadedTextMarkupAnnotation"/> Class
    /// <seealso cref="PdfLoadedUriAnnotation"/> Class
    public class PdfLoadedTextWebLinkAnnotation : PdfLoadedStyledAnnotation
    {
        #region Fields
        /// <summary>
        /// Crosstable
        /// </summary>
        private PdfCrossTable m_crossTable;

        private string m_url;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the Url.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
        /// //Gets the annotation from loaded document.
        /// PdfLoadedTextWebLinkAnnotation textWeblinkAnnotation = document.Pages[1].Annotations[5] as PdfLoadedTextWebLinkAnnotation;
        /// //Sets the text weblink annotation URI
        /// textWeblinkAnnotation.Url="http://www.syncfusion.com";
        /// //Save the document.
        /// document.Save("TextWebLinkAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document.
        /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
        /// 'Gets the annotation from loaded document.
        /// Dim textWeblinkAnnotation As PdfLoadedTextWebLinkAnnotation = document.Pages(1).Annotations(5) as PdfLoadedTextWebLinkAnnotation
        /// 'Sets the text weblink annotation URI
        /// textWeblinkAnnotation.Url="http://www.syncfusion.com"
        /// 'Save the document.
        /// document.Save("TextWebLinkAnnotation.pdf")
        /// </code>
        /// </example>
        public string Url
        {
            get
            {
                return GetUrl();
            }
            set
            {
                m_url = value;
                if (Dictionary.ContainsKey(DictionaryProperties.A))
                {
                    PdfDictionary Dic = m_crossTable.GetObject(Dictionary[DictionaryProperties.A]) as PdfDictionary;
                    Dic.SetString(DictionaryProperties.URI, m_url);
                    Dictionary.Modify();
                }
            }
        }

        #endregion

        #region Implementations
        /// <summary>
        /// Gets the web link.
        /// </summary>
        /// <returns>Web link</returns>
        private string GetUrl()
        {
            string URL = String.Empty;
            if (Dictionary.ContainsKey(DictionaryProperties.A))
            {
                PdfDictionary Dic = m_crossTable.GetObject(Dictionary[DictionaryProperties.A]) as PdfDictionary;
                PdfString text = Dic[DictionaryProperties.URI] as PdfString;
                URL = text.Value.ToString();
            }
            return URL;
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLoadedLineAnnotation"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <param name="rectangle">The rectangle</param>
        /// <param name="text">The text</param>
        internal PdfLoadedTextWebLinkAnnotation(PdfDictionary dictionary, PdfCrossTable crossTable, string text)
            : base(dictionary, crossTable)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            Dictionary = dictionary;
            m_crossTable = crossTable;

        }
        #endregion
    }
}
