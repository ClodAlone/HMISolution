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
    /// Represents the loaded rubber stamp annotation class.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Load an existing document.
    /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
    /// //Gets the annotation from loaded document.
    /// PdfLoadedRubberStampAnnotation rubberStampAnnotation = document.Pages[1].Annotations[5] as PdfLoadedRubberStampAnnotation;
    /// //Sets the rubber stamp annotation border
    /// rubberStampAnnotation.Border.Width = 4;
    /// rubberStampAnnotation.Border.HorizontalRadius = 20;
    /// rubberStampAnnotation.Border.VerticalRadius = 30;
    /// //Set the pdf rubber stamp annotation icon
    /// rubberStampAnnotation.Icon = PdfRubberStampAnnotationIcon.Approved;
    /// //Save the document.
    /// document.Save("RubberStampAnnotation.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Load an existing document.
    /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
    /// 'Gets the annotation from loaded document.
    /// Dim popupAnnotation As PdfLoadedRubberStampAnnotation = document.Pages(1).Annotations(5) as PdfLoadedRubberStampAnnotation
    /// 'Sets the rubber stamp annotation border
    /// rubberStampAnnotation.Border.Width = 4
    /// rubberStampAnnotation.Border.HorizontalRadius = 20
    /// rubberStampAnnotation.Border.VerticalRadius = 30
    /// 'Set the pdf rubber stamp annotation icon
    /// rubberStampAnnotation.Icon = PdfRubberStampAnnotationIcon.Approved
    /// 'Save the document.
    /// document.Save("RubberStampAnnotation.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfLoadedAttachmentAnnotation"/> Class
    /// <seealso cref="PdfLoadedDocumentLinkAnnotation"/> Class
    /// <seealso cref="PdfLoadedFileLinkAnnotation"/> Class
    /// <seealso cref="PdfLoadedLineAnnotation"/> Class
    /// <seealso cref="PdfLoadedPopupAnnotation"/> Class
    /// <seealso cref="PdfLoadedSoundAnnotation"/> Class
    /// <seealso cref="PdfLoadedTextMarkupAnnotation"/> Class
    /// <seealso cref="PdfLoadedTextWebLinkAnnotation"/> Class
    /// <seealso cref="PdfLoadedUriAnnotation"/> Class
    public class PdfLoadedRubberStampAnnotation : PdfLoadedStyledAnnotation
    {
        #region Fields
        /// <summary>
        /// Crosstable
        /// </summary>
        private PdfCrossTable m_crossTable;
        /// <summary>
        /// Rubber and Stamp Annotation name.
        /// </summary>
        private PdfRubberStampAnnotationIcon m_name;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the icon of the annotation.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
        /// //Gets the annotation from loaded document.
        /// PdfLoadedRubberStampAnnotation rubberStampAnnotation = document.Pages[1].Annotations[5] as PdfLoadedRubberStampAnnotation;
        /// //Set the pdf rubber stamp annotation icon
        /// rubberStampAnnotation.Icon = PdfRubberStampAnnotationIcon.Approved;
        /// //Save the document.
        /// document.Save("RubberStampAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document.
        /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
        /// 'Gets the annotation from loaded document.
        /// Dim popupAnnotation As PdfLoadedRubberStampAnnotation = document.Pages(1).Annotations(5) as PdfLoadedRubberStampAnnotation
        /// 'Set the pdf rubber stamp annotation icon
        /// rubberStampAnnotation.Icon = PdfRubberStampAnnotationIcon.Approved
        /// 'Save the document.
        /// document.Save("RubberStampAnnotation.pdf")
        /// </code>
        /// </example>
        public PdfRubberStampAnnotationIcon Icon
        {
            get
            {
                return GetIcon();
            }
            set
            {
                m_name = value;
                Dictionary.SetName(DictionaryProperties.Name, m_name.ToString());
            }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLoadedRubberStampAnnotation"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <param name="rectangle">The rectangle</param>
        /// <param name="text">The text</param>
        internal PdfLoadedRubberStampAnnotation(PdfDictionary dictionary, PdfCrossTable crossTable, RectangleF rectangle, string text)
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
        /// Gets the popup icon type.
        /// </summary>
        /// <returns></returns>
        private PdfRubberStampAnnotationIcon GetIcon()
        {
            PdfRubberStampAnnotationIcon m_icon = PdfRubberStampAnnotationIcon.Draft;
            if (Dictionary.ContainsKey(DictionaryProperties.Name))
            {
                PdfName icon = Dictionary[DictionaryProperties.Name] as PdfName;
                m_icon = GetIconName(icon.Value.ToString());
            }
            return m_icon;
        }

        /// <summary>
        /// Gets the popup icon name
        /// </summary>
        /// <param name="name">Icon name</param>
        /// <returns>Icon type</returns>
        private PdfRubberStampAnnotationIcon GetIconName(string name)
        {
            PdfRubberStampAnnotationIcon icon = PdfRubberStampAnnotationIcon.Draft;
            switch (name)
            {
                case "Approved":
                    icon = PdfRubberStampAnnotationIcon.Approved;
                    break;
                case "AsIs":
                    icon = PdfRubberStampAnnotationIcon.AsIs;
                    break;
                case "Confidential":
                    icon = PdfRubberStampAnnotationIcon.Confidential;
                    break;
                case "Departmental":
                    icon = PdfRubberStampAnnotationIcon.Departmental;
                    break;
                case "Draft":
                    icon = PdfRubberStampAnnotationIcon.Draft;
                    break;
                case "Experimental":
                    icon = PdfRubberStampAnnotationIcon.Experimental;
                    break;
                case "Expired":
                    icon = PdfRubberStampAnnotationIcon.Expired;
                    break;
                case "Final":
                    icon = PdfRubberStampAnnotationIcon.Final;
                    break;
                case "ForComment":
                    icon = PdfRubberStampAnnotationIcon.ForComment;
                    break;
                case "ForPublicRelease":
                    icon = PdfRubberStampAnnotationIcon.ForPublicRelease;
                    break;
                case "NotApproved":
                    icon = PdfRubberStampAnnotationIcon.NotApproved;
                    break;
                case "NotForPublicRelease":
                    icon = PdfRubberStampAnnotationIcon.NotForPublicRelease;
                    break;
                case "Sold":
                    icon = PdfRubberStampAnnotationIcon.Sold;
                    break;
                case "TopSecret":
                    icon = PdfRubberStampAnnotationIcon.TopSecret;
                    break;
            }
            return icon;
        }

        #endregion
    }
}
