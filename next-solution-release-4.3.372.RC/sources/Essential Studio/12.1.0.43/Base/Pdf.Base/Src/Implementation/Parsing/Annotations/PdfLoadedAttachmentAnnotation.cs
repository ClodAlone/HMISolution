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
    /// Represents the attachment annotation from the loaded document.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Load an existing pdf document.
    /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
    /// //Gets the annotation from loaded document.
    /// PdfLoadedAttachmentAnnotation attchmentAnnotation = document.Pages[1].Annotations[3] as PdfLoadedAttachmentAnnotation;
    /// //Gets the annotation flags
    /// PdfAnnotationFlags flag = attchmentAnnotation.AnnotationFlags;
    /// //Gets the attchment annotation border.
    /// PdfAnnotationBorder border = attchmentAnnotation.Border;
    /// //Gets the attchment annotation bounds.
    /// RectangleF rect = attchmentAnnotation.Bounds;
    /// //Sets the attachement icon.
    /// attchmentAnnotation.Icon=PdfAttachmentIcon.PushPin;
    /// //Sets the file name
    /// attchmentAnnotation.FileName=@"..\..\Manual.txt";
    /// //Save the document.
    /// document.Save("LoadedAttachmentAnnotation.pdf");
    /// </code>
    /// <code lang="VB">
    ///'Load an existing document.
    /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
    /// 'Gets the annotation from loaded document.
    /// Dim attchmentAnnotation As PdfLoadedAttachmentAnnotation = document.Pages(1).Annotations(3) as PdfLoadedAttachmentAnnotation
    /// 'Gets the annotation flags
    /// Dim flag As PdfAnnotationFlags = attchmentAnnotation.AnnotationFlags
    /// 'Gets the attchment annotation border.
    /// Dim border As PdfAnnotationBorder = attchmentAnnotation.Border
    /// 'Sets the attachement icon.
    /// attchmentAnnotation.Icon=PdfAttachmentIcon.PushPin
    /// 'Sets the file name
    /// attchmentAnnotation.FileName="..\..\Manual.txt"
    /// 'Save the document.
    /// document.Save("LoadedAttachmentAnnotation.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfLoadedDocumentLinkAnnotation"/> Class
    /// <seealso cref="PdfLoadedFileLinkAnnotation"/> Class
    /// <seealso cref="PdfLoadedLineAnnotation"/> Class
    /// <seealso cref="PdfLoadedPopupAnnotation"/> Class
    /// <seealso cref="PdfLoadedRubberStampAnnotation"/> Class
    /// <seealso cref="PdfLoadedSoundAnnotation"/> Class
    /// <seealso cref="PdfLoadedTextMarkupAnnotation"/> Class
    /// <seealso cref="PdfLoadedTextWebLinkAnnotation"/> Class
    /// <seealso cref="PdfLoadedUriAnnotation"/> Class
    public class PdfLoadedAttachmentAnnotation : PdfLoadedStyledAnnotation
    {
        #region Fields
        private PdfCrossTable m_crossTable;
        private PdfAttachmentIcon m_icon;
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the icon of the annotation.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing pdf document.
        /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
        /// //Gets the annotation from loaded document.
        /// PdfLoadedAttachmentAnnotation attchmentAnnotation = document.Pages[1].Annotations[3] as PdfLoadedAttachmentAnnotation;
        /// attchmentAnnotation.Icon=PdfAttachmentIcon.PushPin;
        /// //Save the document.
        /// document.Save("LoadedAttachmentAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        ///'Load an existing document.
        /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
        /// 'Gets the annotation from loaded document.
        /// Dim attchmentAnnotation As PdfLoadedAttachmentAnnotation = document.Pages(1).Annotations(3) as PdfLoadedAttachmentAnnotation
        /// attchmentAnnotation.Icon=PdfAttachmentIcon.PushPin
        /// 'Save the document.
        /// document.Save("LoadedAttachmentAnnotation.pdf")
        /// </code>
        /// </example>
        public PdfAttachmentIcon Icon
        {
            get
            {
                return m_icon;
            }
            set
            {
                m_icon = value;
                Dictionary.SetName(DictionaryProperties.Name, m_icon.ToString());
            }
        }

#if SILVERLIGHT
        /// <summary>
        /// Security Critical :  Gets the attachment file name of the annotation.
        /// </summary>
        //[System.Security.SecurityCritical]
#else
        /// <summary>
        ///  Gets the attachment file name of the annotation.
        /// </summary>
#endif
        /// <example>
        /// <code lang="C#">
        /// //Load an existing pdf document.
        /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
        /// //Gets the annotation from loaded document.
        /// PdfLoadedAttachmentAnnotation attchmentAnnotation = document.Pages[1].Annotations[3] as PdfLoadedAttachmentAnnotation;
        /// attchmentAnnotation.FileName=@"..\..\Manual.txt";
        /// //Save the document.
        /// document.Save("LoadedAttachmentAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        ///'Load an existing document.
        /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
        /// 'Gets the annotation from loaded document.
        /// Dim attchmentAnnotation As PdfLoadedAttachmentAnnotation = document.Pages(1).Annotations(3) as PdfLoadedAttachmentAnnotation
        /// attchmentAnnotation.FileName="..\..\Manual.txt"
        /// 'Save the document.
        /// document.Save("LoadedAttachmentAnnotation.pdf")
        /// </code>
        /// </example>
        public string FileName
        {
            get
            {
                PdfDictionary dic = m_crossTable.GetObject(Dictionary[DictionaryProperties.FS]) as PdfDictionary;

                PdfString text = dic[DictionaryProperties.Description] as PdfString;

                return text.Value.ToString();
            }
            //set
            //{
            //    PdfDictionary dic = Dictionary;

            //    if (Dictionary.ContainsKey(DictionaryProperties.FS))
            //    {
            //        PdfArray rectangle = Dictionary[ DictionaryProperties.Rect ] as PdfArray; 
            //        dic = m_crossTable.GetObject(Dictionary[DictionaryProperties.FS]) as PdfDictionary;

            //        PdfDictionary attDic = dic[DictionaryProperties.EF] as PdfDictionary;
            //        attDic.Remove(DictionaryProperties.F);


            //        PdfEmbeddedFileSpecification m_fileSpecification = new PdfEmbeddedFileSpecification(value);


            //        //////PdfAttachmentAnnotation att = new PdfAttachmentAnnotation(rectangle.ToRectangle(), value);
            //        PdfReferenceHolder rh = new PdfReferenceHolder(m_fileSpecification as IPdfWrapper);

            //        attDic.SetProperty(DictionaryProperties.F, rh);
            //        ////dic.SetProperty(DictionaryProperties.EF, attDic);
            //        dic.SetProperty(DictionaryProperties.F, new PdfString(value));
            //        dic.SetProperty(DictionaryProperties.Description, new PdfString(value));
            //        dic.SetProperty(DictionaryProperties.UF, new PdfString(value));
            //        Dictionary.Modify();                    
            //    }
            //}
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLoadedAttachmentAnnotation"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <param name="rectangle">The rectangle</param>
        /// <param name="text">The text</param>
        internal PdfLoadedAttachmentAnnotation(PdfDictionary dictionary, PdfCrossTable crossTable, RectangleF rectangle, string text)
            : base(dictionary, crossTable)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            Dictionary = dictionary;
            m_crossTable = crossTable;

            Text = text;
        }
        #endregion
    }
}
