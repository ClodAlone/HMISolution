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
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents the loaded unique resource identifier annotation class.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Load an existing document.
    /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
    /// //Gets the annotation from loaded document.
    /// PdfLoadedUriAnnotation UriAnnotation = document.Pages[1].Annotations[5] as PdfLoadedUriAnnotation;
    /// //Sets the uri annotation URI
    /// UriAnnotation.Url="http://www.syncfusion.com";
    /// //Save the document.
    /// document.Save("UriAnnotation.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Load an existing document.
    /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
    /// 'Gets the annotation from loaded document.
    /// Dim UriAnnotation As PdfLoadedUriAnnotation = document.Pages(1).Annotations(5) as PdfLoadedUriAnnotation
    /// 'Sets the uri annotation URI
    /// UriAnnotation.Url="http://www.syncfusion.com"
    /// 'Save the document.
    /// document.Save("UriAnnotation.pdf")
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
    /// <seealso cref="PdfLoadedTextWebLinkAnnotation"/> Class
    public class PdfLoadedUriAnnotation : PdfLoadedStyledAnnotation
    {
        #region Fields
        /// <summary>
        /// CrossTable
        /// </summary>
        private PdfCrossTable m_crossTable;
        /// <summary>
        /// Indicates the unique resource identifier text.
        /// </summary>
        private string m_uri;
        ///// <summary>
        ///// Indicates the action.
        ///// </summary>
        //private PdfAction m_action;

        #endregion

        #region
        /// <summary>
        /// Gets or sets the unique resource identifier text of the annotation.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
        /// //Gets the annotation from loaded document.
        /// PdfLoadedUriAnnotation UriAnnotation = document.Pages[1].Annotations[5] as PdfLoadedUriAnnotation;
        /// //Sets the uri annotation URI
        /// UriAnnotation.Url="http://www.syncfusion.com";
        /// //Save the document.
        /// document.Save("UriAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document.
        /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
        /// 'Gets the annotation from loaded document.
        /// Dim UriAnnotation As PdfLoadedUriAnnotation = document.Pages(1).Annotations(5) as PdfLoadedUriAnnotation
        /// 'Sets the uri annotation URI
        /// UriAnnotation.Url="http://www.syncfusion.com"
        /// 'Save the document.
        /// document.Save("UriAnnotation.pdf")
        /// </code>
        /// </example>
        public string Uri
        {
            get
            {
                return GetUriText();
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("uri");
                }

                if (value.Length == 0)
                {
                    throw new ArgumentException("Uri can not be an empty string");
                }

                if (m_uri != value)
                {
                    m_uri = value;
                    PdfDictionary dic = Dictionary;
                    if (Dictionary.ContainsKey(DictionaryProperties.A))
                    {
                        dic = m_crossTable.GetObject(Dictionary[DictionaryProperties.A]) as PdfDictionary;
                        dic.SetString(DictionaryProperties.URI, m_uri);
                        Dictionary.Modify();
                    }

                }
            }
        }

        //public PdfAction Action
        //{
        //    get
        //    {
        //    }
        //    set
        //    {
        //    }
        //}

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLoadedLineAnnotation"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <param name="rectangle">The rectangle</param>
        /// <param name="text">The text</param>
        internal PdfLoadedUriAnnotation(PdfDictionary dictionary, PdfCrossTable crossTable, RectangleF rectangle, string text)
            : base(dictionary, crossTable)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            Dictionary = dictionary;
            m_crossTable = crossTable;

            Text = text;
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Gets the unique resource identifier text
        /// </summary>
        /// <returns>The unique resource identifier text</returns>
        private string GetUriText()
        {
            string uriText = String.Empty;
            if (Dictionary.ContainsKey(DictionaryProperties.A))
            {
                PdfDictionary dic = m_crossTable.GetObject(Dictionary[DictionaryProperties.A]) as PdfDictionary;
                PdfString text = dic[DictionaryProperties.URI] as PdfString;
                uriText = text.Value.ToString();
            }
            return uriText;
        }

        #endregion
    }
}
