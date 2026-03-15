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
    /// Represents the loaded pop up annotation class.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Load an existing document.
    /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
    /// //Gets the annotation from loaded document.
    /// PdfLoadedPopupAnnotation popupAnnotation = document.Pages[1].Annotations[5] as PdfLoadedPopupAnnotation;
    /// //Sets the popup annotation border
    /// popupAnnotation.Border.Width = 4;
    /// popupAnnotation.Border.HorizontalRadius = 20;
    /// popupAnnotation.Border.VerticalRadius = 30;
    /// //Set the popup icon
    /// popupAnnotation.Icon = PdfPopupIcon.Key;
    /// //Save the document.
    /// document.Save("popupAnnotation.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Load an existing document.
    /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
    /// 'Gets the annotation from loaded document.
    /// Dim popupAnnotation As PdfLoadedPopupAnnotation = document.Pages(1).Annotations(5) as PdfLoadedPopupAnnotation
    /// 'Sets the popup annotation border
    /// popupAnnotation.Border.Width = 4
    /// popupAnnotation.Border.HorizontalRadius = 20
    /// popupAnnotation.Border.VerticalRadius = 30
    /// 'Set the popup icon
    /// popupAnnotation.Icon = PdfPopupIcon.Key
    /// 'Save the document.
    /// document.Save("popupAnnotation.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfLoadedAttachmentAnnotation"/> Class
    /// <seealso cref="PdfLoadedDocumentLinkAnnotation"/> Class
    /// <seealso cref="PdfLoadedFileLinkAnnotation"/> Class
    /// <seealso cref="PdfLoadedLineAnnotation"/> Class
    /// <seealso cref="PdfLoadedRubberStampAnnotation"/> Class
    /// <seealso cref="PdfLoadedSoundAnnotation"/> Class
    /// <seealso cref="PdfLoadedTextMarkupAnnotation"/> Class
    /// <seealso cref="PdfLoadedTextWebLinkAnnotation"/> Class
    /// <seealso cref="PdfLoadedUriAnnotation"/> Class
    public class PdfLoadedPopupAnnotation : PdfLoadedStyledAnnotation
    {
        #region Fields
        /// <summary>
        /// CroosTable
        /// </summary>
        private PdfCrossTable m_crossTable;
        /// <summary>
        /// Indicates the open the popup window or not.
        /// </summary>        
        private bool m_open;
        /// <summary>
        /// Indicates the icon name.
        /// </summary>
        private PdfPopupIcon m_name;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the open option of the popup annotation.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
        /// //Gets the annotation from loaded document.
        /// PdfLoadedPopupAnnotation popupAnnotation = document.Pages[1].Annotations[5] as PdfLoadedPopupAnnotation;
        /// //Set the popup annotation open option
        /// popupAnnotation.Open = true
        /// //Save the document.
        /// document.Save("popupAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document.
        /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
        /// 'Gets the annotation from loaded document.
        /// Dim popupAnnotation As PdfLoadedPopupAnnotation = document.Pages(1).Annotations(5) as PdfLoadedPopupAnnotation
        /// 'Set the popup annotation open option
        /// popupAnnotation.Open = True
        /// 'Save the document.
        /// document.Save("popupAnnotation.pdf")
        /// </code>
        /// </example>
        public bool Open
        {
            get
            {
                return GetOpen();
            }
            set
            {
                m_open = value;
                Dictionary.SetBoolean(DictionaryProperties.Open, m_open);
            }
        }
        /// <summary>
        /// Gets or sets the icon of the annotation.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
        /// //Gets the annotation from loaded document.
        /// PdfLoadedPopupAnnotation popupAnnotation = document.Pages[1].Annotations[5] as PdfLoadedPopupAnnotation;
        /// //Set the pdf popup icon
        /// popupAnnotation.Icon = PdfPopupIcon.Key;
        /// //Save the document.
        /// document.Save("popupAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document.
        /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
        /// 'Gets the annotation from loaded document.
        /// Dim popupAnnotation As PdfLoadedPopupAnnotation = document.Pages(1).Annotations(5) as PdfLoadedPopupAnnotation
        /// 'Set the pdf popup icon
        /// popupAnnotation.Icon = PdfPopupIcon.Key
        /// 'Save the document.
        /// document.Save("popupAnnotation.pdf")
        /// </code>
        /// </example>
        public PdfPopupIcon Icon
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
        ///// <summary>
        ///// Gets the appearance
        ///// </summary>
        //public PdfAppearance Appearance
        //{
        //    get
        //    {
        //        return GetAppearance();
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
        internal PdfLoadedPopupAnnotation(PdfDictionary dictionary, PdfCrossTable crossTable, RectangleF rectangle, string text)
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
        /// Gets the boolean value ( if it's true popup window is opened otherwise closed.
        /// </summary>
        /// <returns></returns>
        private bool GetOpen()
        {
            bool m_open = false;
            if (Dictionary.ContainsKey(DictionaryProperties.Open))
            {
                PdfBoolean open = Dictionary[DictionaryProperties.Open] as PdfBoolean;
                m_open = open.Value;
            }
            return m_open;
        }

        /// <summary>
        /// Gets the popup icon type.
        /// </summary>
        /// <returns></returns>
        private PdfPopupIcon GetIcon()
        {
            PdfPopupIcon m_icon = PdfPopupIcon.NewParagraph;
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
        private PdfPopupIcon GetIconName(string name)
        {
            PdfPopupIcon icon = PdfPopupIcon.NewParagraph;
            switch (name)
            {
                case "Note":
                    icon = PdfPopupIcon.Note;
                    break;
                case "Comment":
                    icon = PdfPopupIcon.Comment;
                    break;
                case "Help":
                    icon = PdfPopupIcon.Help;
                    break;
                case "Insert":
                    icon = PdfPopupIcon.Insert;
                    break;
                case "Key":
                    icon = PdfPopupIcon.Key;
                    break;
                case "NewParagraph":
                    icon = PdfPopupIcon.NewParagraph;
                    break;
                case "Paragraph":
                    icon = PdfPopupIcon.Paragraph;
                    break;
            }
            return icon;
        }

        //private PdfAppearance GetAppearance()
        //{
        //    if (Dictionary.ContainsKey(DictionaryProperties.AP))
        //    {
        //        PdfDictionary appDic = Dictionary[DictionaryProperties.AP] as PdfDictionary;
        //    }
        //}
        #endregion
    }
}
