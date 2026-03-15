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
using Syncfusion.Pdf.Parsing;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents the loaded document link annotation class.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Load an existing document.
    /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
    /// //Gets the annotation from loaded document.
    /// PdfLoadedDocumentLinkAnnotation documentLinkAnnotation = document.Pages[1].Annotations[4] as PdfLoadedDocumentLinkAnnotation;
    /// //Gets the annotation flags
    /// PdfAnnotationFlags flag = attchmentAnnotation.AnnotationFlags;
    /// //Sets the pdf destination.
    /// documentLinkAnnotation.Destination = new PdfDestination(document.Pages[0], new PointF(10, 10));
    /// //Gets the document link annotation border.
    /// PdfAnnotationBorder border = documentLinkAnnotation.Border;
    /// //Gets the document link annotation bounds.
    /// RectangleF rectangle = documentLinkAnnotation.Bounds;
    /// //Gets the document link annotation color.
    /// PdfColor color = documentLinkAnnotation.Color;
    /// //Gets the document link annotation location.
    /// PointF point = documentLinkAnnotation.Location;
    /// //Gets the document link annotation size.
    /// SizeF size = documentLinkAnnotation.Size;
    /// //Gets the document link annotation text.
    /// string text = documentLinkAnnotation.Text;
    /// //Save the document.
    /// document.Save("documentLinkAnnotation.pdf");
    /// </code>
    /// <code lang="VB">
    ///'Load an existing document.
    /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
    /// 'Gets the annotation from loaded document.
    /// Dim documentLinkAnnotation As PdfLoadedDocumentLinkAnnotation = document.Pages(1).Annotations(4) as PdfLoadedDocumentLinkAnnotation
    /// 'Sets the destination.
    /// documentLinkAnnotation.Destination = New PdfDestination(document.Pages[0], New PointF(10, 10));
    /// 'Gets the annotation flags
    /// Dim flag As PdfAnnotationFlags = documentLinkAnnotation.AnnotationFlags
    /// 'Gets the document link annotation border.
    /// Dim border As PdfAnnotationBorder = documentLinkAnnotation.Border
    /// 'Gets the document link annotation bounds.
    /// Dim rect As RectangleF = documentLinkAnnotation.Bounds
    /// 'Gets the document link annotation color.
    /// Dim color As PdfColor = documentLinkAnnotation.Color
    /// 'Gets the document link annotation location.
    /// Dim point As PointF = documentLinkAnnotation.Location
    /// 'Gets the document link annotation size.
    /// Dim size As SizeF = documentLinkAnnotation.Size
    /// 'Gets the document link annotation text.
    /// Dim text As string = documentLinkAnnotation.Text
    /// 'Save the document.
    /// document.Save("documentLinkAnnotation.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfLoadedAttachmentAnnotation"/> Class
    /// <seealso cref="PdfLoadedFileLinkAnnotation"/> Class
    /// <seealso cref="PdfLoadedLineAnnotation"/> Class
    /// <seealso cref="PdfLoadedPopupAnnotation"/> Class
    /// <seealso cref="PdfLoadedRubberStampAnnotation"/> Class
    /// <seealso cref="PdfLoadedSoundAnnotation"/> Class
    /// <seealso cref="PdfLoadedTextMarkupAnnotation"/> Class
    /// <seealso cref="PdfLoadedTextWebLinkAnnotation"/> Class
    /// <seealso cref="PdfLoadedUriAnnotation"/> Class
    public class PdfLoadedDocumentLinkAnnotation : PdfLoadedStyledAnnotation
    {
        #region Fields
        /// <summary>
        /// Cross Table
        /// </summary>
        private PdfCrossTable m_crossTable;
        #endregion

        #region Properties
        /// <summary>
        /// Sets the destination of the annotation.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
        /// //Gets the annotation from loaded document.
        /// PdfLoadedDocumentLinkAnnotation documentLinkAnnotation = document.Pages[1].Annotations[4] as PdfLoadedDocumentLinkAnnotation;
        /// //Gets the annotation flags
        /// PdfAnnotationFlags flag = attchmentAnnotation.AnnotationFlags;
        /// //Sets the destination.
        /// documentLinkAnnotation.Destination = new PdfDestination(document.Pages[0], new PointF(10, 10));
        /// //Save the document.
        /// document.Save("documentLinkAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        ///'Load an existing document.
        /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
        /// 'Gets the annotation from loaded document.
        /// Dim documentLinkAnnotation As PdfLoadedDocumentLinkAnnotation = document.Pages(1).Annotations(4) as PdfLoadedDocumentLinkAnnotation
        /// 'Sets the destination.
        /// documentLinkAnnotation.Destination = New PdfDestination(document.Pages[0], New PointF(10, 10));
        /// 'Save the document.
        /// document.Save("documentLinkAnnotation.pdf")
        /// </code>
        /// </example>
        public PdfDestination Destination
        {
            get
            {
                return GetDestination();
            }
            set
            {
                Dictionary.SetProperty(DictionaryProperties.Dest, value);
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLoadedDocumentLinkAnnotation"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <param name="rectangle">The rectangle</param>
        internal PdfLoadedDocumentLinkAnnotation(PdfDictionary dictionary, PdfCrossTable crossTable, RectangleF rectangle)
            : base(dictionary, crossTable)
        {
            Dictionary = dictionary;
            m_crossTable = crossTable;
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Gets the destination.
        /// </summary>
        /// <returns>The destination</returns>
        private PdfDestination GetDestination()
        {
            PdfDestination m_destination = null;
            if (Dictionary.ContainsKey(DictionaryProperties.Dest))
            {
                IPdfPrimitive obj = CrossTable.GetObject(Dictionary[DictionaryProperties.Dest]);
                PdfArray array = obj as PdfArray;
                PdfName name = obj as PdfName;
                PdfString str = obj as PdfString;
                PdfLoadedDocument ldDoc = CrossTable.Document as PdfLoadedDocument;

                if (ldDoc != null)
                {
                    if (name != null)
                    {
                        array = ldDoc.GetNamedDestination(name);
                    }
                    else if (str != null)
                    {
                        array = ldDoc.GetNamedDestination(str);
                    }
                }
                PdfReferenceHolder holder = array[0] as PdfReferenceHolder;
                PdfPageBase page = null;

                if (holder == null && array[0] is PdfNumber)
                {
                    PdfNumber pageNo = array[0] as PdfNumber;
                    page = (CrossTable.Document as PdfLoadedDocument).Pages[pageNo.IntValue];

                    PdfName mode = array[1] as PdfName;
                    if (mode != null)
                    {
                        if (mode.Value == "XYZ")
                        {
                            PdfNumber left = array[2] as PdfNumber;
                            PdfNumber top = array[3] as PdfNumber;
                            PdfNumber zoom = array[4] as PdfNumber;

                            float topValue = (top == null) ? 0 : page.Size.Height - top.FloatValue;
                            float leftValue = (left == null) ? 0 : left.FloatValue;

                            m_destination = new PdfDestination(page, new PointF(leftValue, topValue));
                            if (zoom != null)
                            {
                                m_destination.Zoom = zoom.FloatValue;
                            }

                            if ((left == null) || (top == null) || (zoom == null))
                            {
                                m_destination.SetValidation(false);
                            }
                        }
                    }
                    else
                    {
                        if (page != null)
                        {
                            m_destination = new PdfDestination(page);
                            m_destination.Mode = PdfDestinationMode.FitToPage;
                        }
                    }
                }
                if (holder != null)
                {
                    PdfDictionary dic = CrossTable.GetObject(holder) as PdfDictionary;
                    page = (CrossTable.Document as PdfLoadedDocument).Pages.GetPage(dic);
                    PdfName mode = array[1] as PdfName;
                    if (mode != null)
                    {
                        if (mode.Value == "XYZ")
                        {
                            PdfNumber left = array[2] as PdfNumber;
                            PdfNumber top = array[3] as PdfNumber;
                            PdfNumber zoom = array[4] as PdfNumber;

                            float topValue = (top == null) ? 0 : page.Size.Height - top.FloatValue;
                            float leftValue = (left == null) ? 0 : left.FloatValue;

                            m_destination = new PdfDestination(page, new PointF(leftValue, topValue));
                            if (zoom != null)
                            {
                                m_destination.Zoom = zoom.FloatValue;
                            }

                            if ((left == null) || (top == null) || (zoom == null))
                            {
                                m_destination.SetValidation(false);
                            }
                        }
                    }
                    else
                    {
                        if (page != null && mode.Value == "Fit")
                        {
                            m_destination = new PdfDestination(page);
                            m_destination.Mode = PdfDestinationMode.FitToPage;
                        }
                    }
                }
            }
            else if (Dictionary.ContainsKey(DictionaryProperties.A) && (m_destination == null))
            {
                IPdfPrimitive obj = CrossTable.GetObject(Dictionary[DictionaryProperties.A]);
                PdfDictionary destDic = obj as PdfDictionary;
                obj = destDic[DictionaryProperties.D];
                if (obj is PdfReferenceHolder)
                    obj = (obj as PdfReferenceHolder).Object;
                PdfArray array = obj as PdfArray;
                PdfName name = obj as PdfName;
                PdfString str = obj as PdfString;
                PdfLoadedDocument ldDoc = CrossTable.Document as PdfLoadedDocument;

                if (ldDoc != null)
                {
                    if (name != null)
                    {
                        array = ldDoc.GetNamedDestination(name);
                    }
                    else if (str != null)
                    {
                        array = ldDoc.GetNamedDestination(str);
                    }
                }

                if (array != null)
                {
                    PdfReferenceHolder holder = array[0] as PdfReferenceHolder;

                    PdfPageBase page = null;

                    if (holder != null)
                    {
                        PdfDictionary dic = CrossTable.GetObject(holder) as PdfDictionary;
                        page = (CrossTable.Document as PdfLoadedDocument).Pages.GetPage(dic);
                    }

                    PdfName mode = array[1] as PdfName;

                    if (mode.Value == "FitBH" || mode.Value == "FitH")
                    {
                        PdfNumber top = array[2] as PdfNumber;

                        float topValue = (top == null) ? 0 : page.Size.Height - top.FloatValue;
                        m_destination = new PdfDestination(page, new PointF(0, topValue));
                        if (top == null)
                        {
                            m_destination.SetValidation(false);
                        }
                    }
                    else if (mode.Value == "XYZ")
                    {
                        PdfNumber left = array[2] as PdfNumber;
                        PdfNumber top = array[3] as PdfNumber;
                        PdfNumber zoom = array[4] as PdfNumber;

                        if (page != null)
                        {
                            float topValue = (top == null) ? 0 : page.Size.Height - top.FloatValue;
                            float leftValue = (left == null) ? 0 : left.FloatValue;

                            m_destination = new PdfDestination(page, new PointF(leftValue, topValue));
                            if (zoom != null)
                            {
                                m_destination.Zoom = zoom.FloatValue;
                            }

                            if ((left == null) || (top == null) || (zoom == null))
                            {
                                m_destination.SetValidation(false);
                            }
                        }
                    }
                    else if (mode.Value == "FitR")
                    {
                        if (array.Count==6)
                        {
                            PdfNumber left = array[2] as PdfNumber;
                            PdfNumber bottom = array[3] as PdfNumber;
                            PdfNumber right = array[4] as PdfNumber;
                            PdfNumber top = array[5] as PdfNumber;
                            m_destination = new PdfDestination(page, new RectangleF(left.FloatValue, bottom.FloatValue, right.FloatValue, top.FloatValue));
                        }
                    }
                    else
                    {
                        if (page != null && mode.Value == "Fit")
                        {
                            m_destination = new PdfDestination(page);
                            m_destination.Mode = PdfDestinationMode.FitToPage;
                        }
                    }
                }
            }

            return m_destination;
        }

        #endregion
    }
}
