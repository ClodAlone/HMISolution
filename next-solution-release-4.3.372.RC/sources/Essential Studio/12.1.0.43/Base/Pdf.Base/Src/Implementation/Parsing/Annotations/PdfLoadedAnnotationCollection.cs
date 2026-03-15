#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
using System.Drawing;
using System.Text;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;
using System.Collections.Generic;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Parsing
{
    /// <summary>
    /// Represents the loaded annotation colllection.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Load an existing pdf document.
    /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
    /// //Gets the annotation collection.
    /// PdfLoadedAnnotationCollection annotationCollection=document.Pages[1].Annotations;
    /// //Gets the sound annotion.
    /// PdfLoadedSoundAnnotation soundAnnotation = annotationCollection.Annotations[5] as PdfLoadedSoundAnnotation;
    /// //Sets the sound annotation border.
    /// soundAnnotation.Border.Width = 4;
    /// soundAnnotation.Border.HorizontalRadius = 20;
    /// soundAnnotation.Border.VerticalRadius = 30;
    /// //Set the pdf rubberstamp annotation icon.
    /// soundAnnotation.Icon = PdfSoundIcon.Speaker;
    /// //Sets the pdf sound.
    /// PdfSound sound = new PdfSound("Startup.wav");
    /// soundAnnotation.Sound=sound;
    /// //Save the document.
    /// document.Save("SoundAnnotation.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Load an existing pdf document.
    /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
    /// 'Gets the annotation collection.
    /// Dim annotationCollection As PdfLoadedAnnotationCollection = document.Pages[1].Annotations
    /// 'Gets the pdf sound annotation.
    /// Dim soundAnnotation As PdfLoadedSoundAnnotation = dannotationCollection.Annotations(5) as PdfLoadedSoundAnnotation
    /// 'Sets the sound annotation border.
    /// soundAnnotation.Border.Width = 4
    /// soundAnnotation.Border.HorizontalRadius = 20
    /// soundAnnotation.Border.VerticalRadius = 30
    /// 'Set the pdf rubberstamp annotation icon.
    /// soundAnnotation.Icon = PdfSoundIcon.Speaker
    /// 'Sets the pdf sound.
    /// Dim sound As PdfSound  = New PdfSound("Startup.wav")
    /// soundAnnotation.Sound=sound
    /// 'Save the document.
    /// document.Save("SoundAnnotation.pdf")
    /// </code>
    /// </example>
    public class PdfLoadedAnnotationCollection : PdfAnnotationCollection
    {
        #region Field
        /// <summary>
        /// Loaded page, wich collection belongs to.
        /// </summary>
        private PdfLoadedPage m_page;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the <see cref="Syncfusion.Pdf.Interactive.PdfAnnotation"/> at the specified index.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
        /// //Gets the annotation collection.
        /// PdfLoadedAnnotationCollection annotationCollection=document.Pages[1].Annotations;
        /// //Gets the pdf sound annotation.
        /// PdfLoadedSoundAnnotation soundAnnotation = annotationCollection.Annotations[5] as PdfLoadedSoundAnnotation;
        /// //Sets the sound annotation border
        /// soundAnnotation.Border.Width = 4;
        /// //Save the document.
        /// document.Save("SoundAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document.
        /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
        /// 'Gets the annotation from loaded document.
        /// Dim annotationCollection As PdfLoadedAnnotationCollection = document.Pages[1].Annotations
        /// 'Gets the pdf sound annotation.
        /// Dim soundAnnotation As PdfLoadedSoundAnnotation = dannotationCollection.Annotations(5) as PdfLoadedSoundAnnotation
        /// 'Sets the sound annotation border.
        /// soundAnnotation.Border.Width = 4
        /// 'Save the document.
        /// document.Save("SoundAnnotation.pdf")
        /// </code>
        /// </example>
        public override PdfAnnotation this[int index]
        {
            get
            {
                int k = List.Count;

                if ((k < 0) || (index >= k))
                    throw new IndexOutOfRangeException("index");

                PdfAnnotation Annot = List[index] as PdfAnnotation;

                PdfLoadedAnnotation ldAnnot = Annot as PdfLoadedAnnotation;

                ldAnnot.Page = this.Page;
                return Annot;
            }
        }

        /// <summary>
        /// Represents the annotation with specified name.
        /// </summary>
        /// <param name="name">The specified annotation name.</param>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
        /// //Gets the annotation collection.
        /// PdfLoadedAnnotationCollection annotationCollection=document.Pages[1].Annotations;\
        /// 'Gets the pfd sound annotation.
        /// PdfLoadedSoundAnnotation soundAnnotation = annotationCollection.Annotations["SoundAnnotation"] as PdfLoadedSoundAnnotation;
        /// //Sets the sound annotation border
        /// soundAnnotation.Border.Width = 4;
        /// //Save the document.
        /// document.Save("SoundAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document.
        /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
        /// 'Gets the annotation collection.
        /// Dim annotationCollection As PdfLoadedAnnotationCollection = document.Pages[1].Annotations
        /// 'Gets the pfd sound annotation.
        /// Dim soundAnnotation As PdfLoadedSoundAnnotation = dannotationCollection.Annotations("SoundAnnotation") as PdfLoadedSoundAnnotation
        /// 'Sets the sound annotation border
        /// soundAnnotation.Border.Width = 4
        /// 'Save the document.
        /// document.Save("SoundAnnotation.pdf")
        /// </code>
        /// </example>
        public PdfAnnotation this[string text]
        {
            get
            {
                if (text == null)
                    throw new ArgumentNullException("text");

                if (text == string.Empty)
                    throw new ArgumentException("Annotation text can't be empty");

                int index = GetAnnotationIndex(text);

                if (index == -1)
                    throw new ArgumentException("Incorrect field name");

                return this[index];
            }
        }

        /// <summary>
        /// Gets or sets the page.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
        /// //Gets the pfd sound annotation.
        /// PdfLoadedAnnotationCollection annotationCollection=document.Pages[1].Annotations;
        /// //Gets the pdf sound annotation.
        /// PdfLoadedSoundAnnotation soundAnnotation = annotationCollection.Annotations[5] as PdfLoadedSoundAnnotation;
        /// 'Gets the sound pdf loaded page.
        /// PdfLoadedPage page =soundAnnotation.Page;
        /// //Save the document.
        /// document.Save("SoundAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document.
        /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
        /// 'Gets the pfd sound annotation.
        /// Dim annotationCollection As PdfLoadedAnnotationCollection = document.Pages[1].Annotations
        /// 'Gets the pdf sound annotation.
        /// Dim soundAnnotation As PdfLoadedSoundAnnotation = dannotationCollection.Annotations(5) as PdfLoadedSoundAnnotation
        /// 'Gets the pdf loaded page.
        /// Dim page As PdfLoadedPage=soundAnnotation.Page
        /// 'Save the document.
        /// document.Save("SoundAnnotation.pdf")
        /// </code>
        /// </example>
        public PdfLoadedPage Page
        {
            get
            {
                return m_page;
            }
            set
            {
                m_page = value;
            }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLoadedAnnotationCollection"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        internal PdfLoadedAnnotationCollection(PdfLoadedPage page)
            : base()
        {
            if (page == null)
                throw new ArgumentException("page");

            m_page = page;

            for (int i = 0, size = m_page.TerminalAnnotation.Count; i < size; ++i)
            {
                PdfAnnotation annot = GetAnnotation(i);
                if (annot != null)
                    DoAdd(annot);
            }
            this.Page = m_page;
            // m_page.TerminalAnnotation.Clear();
        }

        /// <summary>
        /// NameChanged evant handler.
        /// </summary>
        /// <param name="name">New Name of the annotation.</param>
        private void ldAnnotation_NameChanded(string name)
        {
            if (!IsValidName(name))
                throw new ArgumentException("Annotation with the same name already exist");
        }


        #endregion


        #region Implementation

        /// <summary>
        /// Adds annotation to collection.
        /// </summary>
        /// <param name="annotation">Annotation to be added to collection.</param>
        /// <returns>Position of the annotation in collection.</returns>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
        /// //Gets the pfd sound annotation.
        /// PdfLoadedAnnotationCollection annotationCollection=document.Pages[1].Annotations;
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new Uri Annotation.
        /// PdfUriAnnotation uriAnnotation = new PdfUriAnnotation(rectangle, "http://www.google.com");
        /// //Set Text to uriAnnotation.
        /// uriAnnotation.Text = "Uri Annotation";
        /// annotationCollection.Add(uriAnnotation);
        /// //Save the document.
        /// document.Save("SoundAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document.
        /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
        /// 'Gets the pfd sound annotation.
        /// Dim annotationCollection As PdfLoadedAnnotationCollection = document.Pages[1].Annotations
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new uri Annotation.
        /// Dim uriAnnotation As PdfUriAnnotation = New PdfUriAnnotation(rectangle, "http://www.google.com")
        /// 'Set the Text to uriAnnotation.
        /// uriAnnotation.Text = "Uri Annotation"
        /// annotationCollection.Add(uriAnnotation)
        /// 'Save the document.
        /// document.Save("SoundAnnotation.pdf")
        /// </code>
        /// </example>
        public override int Add(PdfAnnotation annotation)
        {
            if (annotation == null)
            {
                throw new ArgumentNullException("annotation");
            }
            if (annotation is PdfTextMarkupAnnotation)
            {
                PdfTextMarkupAnnotation markupAnnot = annotation as PdfTextMarkupAnnotation;
                markupAnnot.SetQuadPoints(m_page.Size);
            }
            return DoAdd(annotation);
        }


        /// <summary>
        /// Adds a annotation to collection.
        /// </summary>
        /// <param name="annot">The annotation.</param>
        /// <returns></returns>
        protected override int DoAdd(PdfAnnotation annot)
        {
            int index = -1;

            if (annot == null)
                throw new ArgumentNullException("annotation");

            annot.SetPage(m_page);

            PdfArray array = null;
            if (m_page.Dictionary.ContainsKey(DictionaryProperties.Annots))
            {
                array = m_page.CrossTable.GetObject(m_page.Dictionary[DictionaryProperties.Annots]) as PdfArray;
            }
            else
            {
                array = new PdfArray();
            }

            PdfReferenceHolder reference = new PdfReferenceHolder(annot);

            if (!array.Contains(reference))
            {
                array.Add(new PdfReferenceHolder(annot));
                m_page.Dictionary.SetProperty(DictionaryProperties.Annots, array);
            }

            index = base.DoAdd(annot);
            return index;
        }

        /// <summary>
        /// Gets the new name of the annotation.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The annotation name.</returns>
        internal string GetCorrectName(string name)
        {
            List<string> list = new List<string>();

            foreach (PdfAnnotation annot in List)
            {
                list.Add(annot.Text);
            }

            string correctName = name;

            int index = 0;

            while (list.IndexOf(correctName) != -1)
            {
                correctName = name + index;
                ++index;
            }

            return correctName;
        }
        /// <summary>
        /// Check whether the annotation with the same name already exists.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        /// <c>true</c> if there are no annotation with the same name within the collection; 
        /// otherwise <c>false</c>.
        /// </returns>
        internal bool IsValidName(string name)
        {
            foreach (PdfAnnotation annot in List)
            {
                if (annot.Text == name)
                {
                    return false;
                }
            }

            return true;
        }
        /// <summary>
        /// Gets the index of the annotation.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The index of the annotation.</returns>
        private int GetAnnotationIndex(string text)
        {
            int i = -1;

            foreach (PdfAnnotation annot in List)
            {
                ++i;
                if (annot.Text == text)
                    break;
                else
                {
                    if (annot.Text != null || annot.Text != string.Empty)
                    {
                        string[] annotText = annot.Text.Split('(');
                        if (annotText[0] == text)
                            return i;
                    }
                }
            }
            if ((i == List.Count - 1) &&
                ((List[List.Count - 1] as PdfLoadedAnnotation).Text != text))
            {
                i = -1;
            }

            return i;
        }

        /// <summary>
        /// Gets the annotation.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The created annotation.</returns>
        private PdfAnnotation GetAnnotation(int index)
        {
            PdfDictionary dictionary = m_page.TerminalAnnotation[index] as PdfDictionary;
            PdfCrossTable crossTable = m_page.CrossTable;
            PdfAnnotation annot = null;

            PdfName name = PdfLoadedAnnotation.GetValue(dictionary, crossTable, DictionaryProperties.Subtype, true) as PdfName;

            PdfLoadedAnnotationTypes type = GetAnnotationType(name, dictionary, crossTable);

            PdfArray rectValue = PdfCrossTable.Dereference(dictionary[DictionaryProperties.Rect]) as PdfArray;

            if (rectValue != null)
            {
                RectangleF rect = rectValue.ToRectangle();

                string text = String.Empty;

                if (dictionary.ContainsKey(DictionaryProperties.Contents))
                {
                    PdfString str = dictionary[DictionaryProperties.Contents] as PdfString;
                    text = str.Value.ToString();
                }


                
                switch (type)
                {
                    case PdfLoadedAnnotationTypes.TextWebLinkAnnotation:
                        annot = CreateTextWebLinkAnnotation(dictionary, crossTable, text);
                        break;
                    case PdfLoadedAnnotationTypes.DocumentLinkAnnotation:
                        annot = CreateDocumentLinkAnnotation(dictionary, crossTable, rect);
                        break;
                    case PdfLoadedAnnotationTypes.FileLinkAnnotation:
                        PdfReferenceHolder fHolder = dictionary[DictionaryProperties.A] as PdfReferenceHolder;
                        PdfDictionary fDic;
                        if (fHolder!= null)
                        {
                            fDic = fHolder.Object as PdfDictionary;
                            fDic = PdfCrossTable.Dereference(fDic[DictionaryProperties.F]) as PdfDictionary;
                            if (fDic == null)
                                fDic = fHolder.Object as PdfDictionary;
                            PdfString fName;
                            if (fDic.ContainsKey(DictionaryProperties.F))
                            {
                                if (fDic[DictionaryProperties.F] is PdfString)
                                    fName = fDic[DictionaryProperties.F] as PdfString;
                                else
                                    fName = (fDic[DictionaryProperties.F] as PdfReferenceHolder).Object as PdfString;
                                annot = CreateFileLinkAnnotation(dictionary, crossTable, rect, fName.Value.ToString());
                            }
                            else if (fDic.ContainsKey(DictionaryProperties.UF))
                            {
                                if (fDic[DictionaryProperties.UF] is PdfString)
                                    fName = fDic[DictionaryProperties.UF] as PdfString;
                                else
                                    fName = (fDic[DictionaryProperties.UF] as PdfReferenceHolder).Object as PdfString;
                                annot = CreateFileLinkAnnotation(dictionary, crossTable, rect, fName.Value.ToString());
                            }
                            
                        }
                        else if (dictionary.ContainsKey(DictionaryProperties.A))
                        {
                            fDic = dictionary[DictionaryProperties.A] as PdfDictionary;
                            if (fDic.ContainsKey(DictionaryProperties.F))
                            {
                                if (fDic[DictionaryProperties.F] is PdfDictionary)
                                    fDic = PdfCrossTable.Dereference(fDic[DictionaryProperties.F]) as PdfDictionary;
                                else if (fDic[DictionaryProperties.F] is PdfReferenceHolder)
                                    fDic = (fDic[DictionaryProperties.F] as PdfReferenceHolder).Object as PdfDictionary;
                                PdfString fName = fDic[DictionaryProperties.F] as PdfString;
                                annot = CreateFileLinkAnnotation(dictionary, crossTable, rect, fName.Value.ToString());
                            }    
                        }                        
                        break;

                    case PdfLoadedAnnotationTypes.AnnotationStates:
                        annot = CreateAnnotationStates(dictionary, crossTable);
                        break;
                    case PdfLoadedAnnotationTypes.CaretAnnotation:
                        annot = CreateCaretAnnotation(dictionary, crossTable);
                        break;
                    case PdfLoadedAnnotationTypes.FileAttachmentAnnotation:
                        PdfDictionary dic = PdfCrossTable.Dereference(dictionary[DictionaryProperties.FS]) as PdfDictionary;
                        PdfString filename = PdfCrossTable.Dereference(dic[DictionaryProperties.F]) as PdfString;
                        annot = CreateFileAttachmentAnnotation(dictionary, crossTable, rect, filename.Value.ToString());
                        break;
                    case PdfLoadedAnnotationTypes.FreeTextAnnotation:
                        annot = CreateFreeTextAnnotation(dictionary, crossTable);
                        break;
                    case PdfLoadedAnnotationTypes.LineAnnotation:
                        annot = CreateLineAnnotation(dictionary, crossTable, rect, text);
                        break;
                    case PdfLoadedAnnotationTypes.LinkAnnotation:
                        filename = null;
                        if (dictionary.ContainsKey(DictionaryProperties.A))
                        {
                            PdfDictionary dict = new PdfDictionary();
                            PdfArray destinationArray = new PdfArray();
                            PdfDictionary remoteLinkDic = PdfCrossTable.Dereference(dictionary[DictionaryProperties.A]) as PdfDictionary;
                            if (remoteLinkDic.ContainsKey(DictionaryProperties.S))
                            {
                                destinationArray = PdfCrossTable.Dereference(remoteLinkDic[DictionaryProperties.D]) as PdfArray;
                                PdfName gotor;
                                gotor = PdfCrossTable.Dereference(remoteLinkDic[DictionaryProperties.S]) as PdfName;
                                if (gotor.Value == "GoToR")
                                {

                                    if ((PdfCrossTable.Dereference(remoteLinkDic[DictionaryProperties.F]) as PdfString)!=null)
                                    {
                                        filename = PdfCrossTable.Dereference(remoteLinkDic[DictionaryProperties.F]) as PdfString;
                                        annot = CreateFileRemoteGoToLinkAnnotation(dictionary, crossTable, filename, destinationArray, rect);
                                    }
                                    else
                                    {
                                        if ((PdfCrossTable.Dereference(remoteLinkDic[DictionaryProperties.F]) as PdfDictionary) != null)
                                        {
                                            dict = PdfCrossTable.Dereference(remoteLinkDic[DictionaryProperties.F]) as PdfDictionary;

                                            if (dict.ContainsKey(DictionaryProperties.F))
                                            {
                                                filename = dict[DictionaryProperties.F] as PdfString;

                                                annot = CreateFileRemoteGoToLinkAnnotation(dictionary, crossTable, filename, destinationArray, rect);
                                            }
                                        }
                                    }
                                 
                                      
                                    
                                
                                }
                            }
                        }
                        else
                        {
                            annot = CreateLinkAnnotation(dictionary, crossTable, rect, text);
                        }
                        break;
                    case PdfLoadedAnnotationTypes.LnkAnnotation:
                        filename = null;
                        if (dictionary.ContainsKey(DictionaryProperties.A))
                        {
                            PdfDictionary linkDic = PdfCrossTable.Dereference(dictionary[DictionaryProperties.A]) as PdfDictionary;
                            dic = PdfCrossTable.Dereference(linkDic[DictionaryProperties.F]) as PdfDictionary;
                            filename = dic[DictionaryProperties.F] as PdfString;
                        }
                        annot = CreateLnkAnnotation(dictionary, crossTable, rect, filename.Value.Substring(1));
                        break;
                    case PdfLoadedAnnotationTypes.Highlight:
                    case PdfLoadedAnnotationTypes.Squiggly:
                    case PdfLoadedAnnotationTypes.StrikeOut:
                    case PdfLoadedAnnotationTypes.Underline:
                        annot = CreateMarkupAnnotation(dictionary, crossTable, rect);
                        break;
                    case PdfLoadedAnnotationTypes.MovieAnnotation:
                        annot = CreateMovieAnnotation(dictionary, crossTable);
                        break;
                    case PdfLoadedAnnotationTypes.PolygonandPolylineAnnotation:
                        annot = CreatePolygonandPolylineAnnotation(dictionary, crossTable);
                        break;
                    case PdfLoadedAnnotationTypes.PopupAnnotation:
                        annot = CreatePopupAnnotation(dictionary, crossTable, rect, text);
                        break;
                    case PdfLoadedAnnotationTypes.PrinterMarkAnnotation:
                        annot = CreatePrinterMarkAnnotation(dictionary, crossTable);
                        break;
                    case PdfLoadedAnnotationTypes.RubberStampAnnotation:
                        annot = CreateRubberStampAnnotation(dictionary, crossTable, rect, text);
                        break;
                    case PdfLoadedAnnotationTypes.ScreenAnnotation:
                        annot = CreateScreenAnnotation(dictionary, crossTable, rect);
                        break;
                    case PdfLoadedAnnotationTypes.SoundAnnotation:
                        dic = PdfCrossTable.Dereference(dictionary[DictionaryProperties.Sound]) as PdfDictionary;
                        //  filename = PdfCrossTable.Dereference(dic[DictionaryProperties.T]) as PdfString;
                        annot = CreateSoundAnnotation(dictionary, crossTable, rect);
                        break;
                    case PdfLoadedAnnotationTypes.SquareandCircleAnnotation:
                        annot = CreateSquareandCircleAnnotation(dictionary, crossTable);
                        break;
                    case PdfLoadedAnnotationTypes.TextAnnotation:
                        annot = CreateTextAnnotation(dictionary, crossTable);
                        break;
                    case PdfLoadedAnnotationTypes.TextMarkupAnnotation:
                        annot = CreateTextMarkupAnnotation(dictionary, crossTable);
                        break;
                    case PdfLoadedAnnotationTypes.TrapNetworkAnnotation:
                        annot = CreateTrapNetworkAnnotation(dictionary, crossTable);
                        break;
                    case PdfLoadedAnnotationTypes.WatermarkAnnotation:
                        annot = CreateWatermarkAnnotation(dictionary, crossTable);
                        break;
                    case PdfLoadedAnnotationTypes.WidgetAnnotation:
                        annot = CreateWidgetAnnotation(dictionary, crossTable,rect);
                        break;
                    case PdfLoadedAnnotationTypes.InkAnnotation:
                        annot = CreateInkAnnotation(dictionary, crossTable, rect);
                        break;
                }

                PdfLoadedAnnotation ldAnnotation = annot as PdfLoadedAnnotation;

                if (ldAnnotation != null)
                {
                    ldAnnotation.BeforeNameChanges += new PdfLoadedAnnotation.BeforeNameChangesEventHandler(ldAnnotation_NameChanded);
                }

                return annot;
            }
            else
            {
                return annot;
            }
        }

        /// <summary>
        /// Gets the type of the annotation.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <returns>The annotation type.</returns>
        private PdfLoadedAnnotationTypes GetAnnotationType(PdfName name, PdfDictionary dictionary, PdfCrossTable crossTable)
        {
            string str = name.Value;
            PdfLoadedAnnotationTypes type = PdfLoadedAnnotationTypes.Null;

            PdfNumber number =
                PdfLoadedAnnotation.GetValue(dictionary, crossTable, DictionaryProperties.Subtype, true) as PdfNumber;

            int AnnotFlags = 0;

            if (number != null)
            {
                AnnotFlags = number.IntValue;
            }

            switch (str.ToLower())
            {
                case "sound":
                    type = PdfLoadedAnnotationTypes.SoundAnnotation;
                    break;

                case "text":                
                    type = PdfLoadedAnnotationTypes.PopupAnnotation;
                    break;

                case "link":
                    if (dictionary.ContainsKey(DictionaryProperties.A))
                    {
                        PdfDictionary linkDic = PdfCrossTable.Dereference(dictionary[DictionaryProperties.A]) as PdfDictionary;
                        name = linkDic[DictionaryProperties.S] as PdfName;

                        PdfArray arr = dictionary[DictionaryProperties.Border] as PdfArray;

                        bool m_type = FindAnnotation(arr);

                        if (name.Value.ToString() == "URI")
                        {
                            if (!m_type)
                                type = PdfLoadedAnnotationTypes.LinkAnnotation;
                            else
                                type = PdfLoadedAnnotationTypes.TextWebLinkAnnotation;
                        }
                        else if (name.Value.ToString() == "Launch")
                        {
                            type = PdfLoadedAnnotationTypes.FileLinkAnnotation;
                        }
                        else if (name.Value.ToString() == "GoToR")
                        {
                            type = PdfLoadedAnnotationTypes.LinkAnnotation;
                        }
                        else if (name.Value.ToString() == "GoTo")
                        {
                            type = PdfLoadedAnnotationTypes.DocumentLinkAnnotation;
                        }
                    }
                    else //if (dictionary.ContainsKey(DictionaryProperties.Contents))
                    {
                        PdfName strText = dictionary[DictionaryProperties.Subtype] as PdfName;
                        switch (strText.Value.ToString())
                        {
                            case "Link":
                                type = PdfLoadedAnnotationTypes.DocumentLinkAnnotation;
                                break;

                        }
                        //type = PdfLoadedAnnotationTypes.Highlight;
                    }
                    break;

                case "fileattachment":
                    type = PdfLoadedAnnotationTypes.FileAttachmentAnnotation;
                    break;
                case "line":
                    type = PdfLoadedAnnotationTypes.LineAnnotation;
                    break;
                case "widget":
                    type = PdfLoadedAnnotationTypes.WidgetAnnotation;
                    break;
                case "highlight":
                    type = PdfLoadedAnnotationTypes.Highlight;
                    break;
                case "underline":
                    type = PdfLoadedAnnotationTypes.Underline;
                    break;
                case "strikeout":
                    type = PdfLoadedAnnotationTypes.StrikeOut;
                    break;
                case "squiggly":
                    type = PdfLoadedAnnotationTypes.Squiggly;
                    break;
                case "stamp":
                    type = PdfLoadedAnnotationTypes.RubberStampAnnotation;
                    break;
                case "ink":
                    type = PdfLoadedAnnotationTypes.InkAnnotation;
                    break;
            }
            return type;
        }
        /// <summary>
        /// Create file remotegoto link annotation
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="crossTable"></param>
        /// <param name="fileName"></param>
        /// <param name="destination"></param>
        /// <param name="rect"></param>
        /// <returns>The created file remotegoto link annotation</returns>
        private PdfAnnotation CreateFileRemoteGoToLinkAnnotation(PdfDictionary dictionary, PdfCrossTable crossTable, PdfString fileName, PdfArray destination, RectangleF rect)
        {

            PdfAnnotation annot = new PdfLoadedFileLinkAnnotation(dictionary, crossTable, destination, rect, fileName.Value.ToString());
            annot.SetPage(m_page);
            return annot;
        }
        /// <summary>
        /// Creates the text web link annotation.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <returns>The created file link annotation.</returns>
        private PdfAnnotation CreateTextWebLinkAnnotation(PdfDictionary dictionary, PdfCrossTable crossTable, string text)
        {
            PdfAnnotation annot = new PdfLoadedTextWebLinkAnnotation(dictionary, crossTable, text);
            annot.SetPage(m_page);
            return annot;
        }

        /// <summary>
        /// Creates the file link annotation.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <returns>The created file link annotation.</returns>
        private PdfAnnotation CreateDocumentLinkAnnotation(PdfDictionary dictionary, PdfCrossTable crossTable, RectangleF rect)
        {
            PdfAnnotation annot = new PdfLoadedDocumentLinkAnnotation(dictionary, crossTable, rect);
            annot.SetPage(m_page);
            return annot;
        }

        /// <summary>
        /// Creates the document link annotation.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <returns>The created widget annotation.</returns>
        private PdfAnnotation CreateFileLinkAnnotation(PdfDictionary dictionary, PdfCrossTable crossTable, RectangleF rect, string filename)
        {
            PdfAnnotation annot = new PdfLoadedFileLinkAnnotation(dictionary, crossTable, rect, filename);
            annot.SetPage(m_page);
            return annot;
        }

        /// <summary>
        /// Creates the widget annotation.
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="crossTable"></param>
        /// <param name="rect"></param>
        /// <returns>The created the widget annotation.</returns>
        private PdfAnnotation CreateWidgetAnnotation(PdfDictionary dictionary, PdfCrossTable crossTable,RectangleF rect)
        {
            PdfAnnotation annot = new PdfLoadedWidgetAnnotation(dictionary, crossTable, rect); 
            annot.SetPage(m_page);
            return annot;
        }
        /// <summary>
        /// Creates the Ink annotation.
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="crossTable"></param>
        /// <param name="rect"></param>
        /// <returns>Created the Ink annotation.</returns>
        private PdfAnnotation CreateInkAnnotation(PdfDictionary dictionary, PdfCrossTable crossTable, RectangleF rect)
        {
            PdfAnnotation annot = new PdfLoadedInkAnnotation(dictionary, crossTable, rect);
            annot.SetPage(m_page);
            return annot;
        }
        /// <summary>
        /// Creates the watermark annotation.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <returns>The created watermark annotation.</returns>
        private PdfAnnotation CreateWatermarkAnnotation(PdfDictionary dictionary, PdfCrossTable crossTable)
        {
            PdfAnnotation annot = new PdfTextMarkupAnnotation();
            annot.SetPage(m_page);
            return annot;
        }
        /// <summary>
        /// Creates the Trap Network Annotation.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <returns>The created Trap Network Annotation.</returns>
        private PdfAnnotation CreateTrapNetworkAnnotation(PdfDictionary dictionary, PdfCrossTable crossTable)
        {
            PdfAnnotation annot = new PdfTextMarkupAnnotation();
            annot.SetPage(m_page);
            return annot;
        }
        /// <summary>
        /// Creates the Text Markup Annotation.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <returns>The created Text Markup Annotation.</returns>
        private PdfAnnotation CreateTextMarkupAnnotation(PdfDictionary dictionary, PdfCrossTable crossTable)
        {
            PdfAnnotation annot = new PdfTextMarkupAnnotation();
            annot.SetPage(m_page);
            return annot;
        }
        /// <summary>
        /// Creates the Text Annotation.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <returns>The created Text Annotation.</returns>
        private PdfAnnotation CreateTextAnnotation(PdfDictionary dictionary, PdfCrossTable crossTable)
        {
            PdfAnnotation annot = new PdfTextMarkupAnnotation();
            annot.SetPage(m_page);
            return annot;
        }
        /// <summary>
        /// Creates the Square and Circle Annotation.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <returns>The created Square and Circle Annotation.</returns>
        private PdfAnnotation CreateSquareandCircleAnnotation(PdfDictionary dictionary, PdfCrossTable crossTable)
        {
            PdfAnnotation annot = new PdfTextMarkupAnnotation();
            annot.SetPage(m_page);
            return annot;
        }
        /// <summary>
        /// Creates the Sound Annotation.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <param name="rect">The RectangleF.</param>
        /// <param name="fileName">The Filename.</param>
        /// <returns>The created Sound Annotation.</returns>
        private PdfAnnotation CreateSoundAnnotation(PdfDictionary dictionary, PdfCrossTable crossTable, RectangleF rect)
        {
            PdfLoadedAnnotation annot = new PdfLoadedSoundAnnotation(dictionary, crossTable, rect);
            annot.SetPage(m_page);
            return annot;
        }
        /// <summary>
        /// Creates the Screen Annotation.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <returns>The created Screen Annotation.</returns>
        private PdfAnnotation CreateScreenAnnotation(PdfDictionary dictionary, PdfCrossTable crossTable, RectangleF rect)
        {
            PdfAnnotation annot = new PdfLoadedTextMarkupAnnotation(dictionary, crossTable, rect);
            annot.SetPage(m_page);
            return annot;
        }
        /// <summary>
        /// Creates the Rubber Stamp Annotation.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <returns>The created Rubber Stamp Annotation.</returns>
        private PdfAnnotation CreateRubberStampAnnotation(PdfDictionary dictionary, PdfCrossTable crossTable, RectangleF rect, string text)
        {
            PdfAnnotation annot = new PdfLoadedRubberStampAnnotation(dictionary, crossTable, rect, text);
            annot.SetPage(m_page);
            return annot;
        }
        /// <summary>
        /// Creates the Printer Mark Annotation.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <returns>The created Printer Mark Annotation.</returns>
        private PdfAnnotation CreatePrinterMarkAnnotation(PdfDictionary dictionary, PdfCrossTable crossTable)
        {
            PdfAnnotation annot = new PdfTextMarkupAnnotation();
            annot.SetPage(m_page);
            return annot;
        }
        /// <summary>
        /// Creates the Popup Annotation.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <param name="rect">The RectangleF.</param>
        /// <param name="text">The Text.</param>
        /// <returns>The created Popup Annotation.</returns>
        private PdfAnnotation CreatePopupAnnotation(PdfDictionary dictionary, PdfCrossTable crossTable, RectangleF rect, string text)
        {
            PdfLoadedAnnotation annot = new PdfLoadedPopupAnnotation(dictionary, crossTable, rect, text);
            annot.SetPage(m_page);
            return annot;
        }
        /// <summary>
        /// Creates the Polygon and Polyline Annotation.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <returns>The created Polygon and Polyline Annotation.</returns>
        private PdfAnnotation CreatePolygonandPolylineAnnotation(PdfDictionary dictionary, PdfCrossTable crossTable)
        {
            PdfAnnotation annot = new PdfTextMarkupAnnotation();
            annot.SetPage(m_page);
            return annot;
        }
        /// <summary>
        /// Creates the Movie Annotation.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <returns>The created Movie Annotation.</returns>
        private PdfAnnotation CreateMovieAnnotation(PdfDictionary dictionary, PdfCrossTable crossTable)
        {
            PdfAnnotation annot = new PdfTextMarkupAnnotation();
            annot.SetPage(m_page);
            return annot;
        }
        /// <summary>
        /// Creates the Markup Annotation.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <param name="rect">The RectangleF.</param>
        /// <returns>The created Markup Annotation.</returns>
        private PdfAnnotation CreateMarkupAnnotation(PdfDictionary dictionary, PdfCrossTable crossTable, RectangleF rect)
        {
            PdfLoadedAnnotation annot = new PdfLoadedTextMarkupAnnotation(dictionary, crossTable, rect);
            annot.SetPage(m_page);
            return annot;
        }
        /// <summary>
        /// Creates the Lnk Annotation.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <param name="rect">The RectangleF.</param>
        /// <param name="filename">The Filename.</param>
        /// <returns>The created Lnk Annotation.</returns>
        private PdfAnnotation CreateLnkAnnotation(PdfDictionary dictionary, PdfCrossTable crossTable, RectangleF rect, string filename)
        {
            PdfLoadedAnnotation annot = new PdfLoadedFileLinkAnnotation(dictionary, crossTable, rect, filename);
            annot.SetPage(m_page);
            return annot;
        }
        /// <summary>
        /// Creates the Link Annotation.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <param name="rect">The RectangleF.</param>
        /// <param name="text">The Text.</param>
        /// <returns>The created Link Annotation.</returns>
        private PdfAnnotation CreateLinkAnnotation(PdfDictionary dictionary, PdfCrossTable crossTable, RectangleF rect, string text)
        {
            PdfLoadedAnnotation annot = new PdfLoadedUriAnnotation(dictionary, crossTable, rect, text);
            annot.SetPage(m_page);
            return annot;
        }
        /// <summary>
        /// Creates the Line Annotation.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <param name="rect">The RectangleF.</param>
        /// <param name="text">The Text.</param>
        /// <returns>The created Line Annotation.</returns>
        private PdfAnnotation CreateLineAnnotation(PdfDictionary dictionary, PdfCrossTable crossTable, RectangleF rect, string text)
        {
            PdfLoadedAnnotation annot = new PdfLoadedLineAnnotation(dictionary, crossTable, rect, text);
            annot.SetPage(m_page);
            return annot;
        }
        /// <summary>
        /// Creates the Free Text Annotation.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <returns>The created Free Text Annotation.</returns>
        private PdfAnnotation CreateFreeTextAnnotation(PdfDictionary dictionary, PdfCrossTable crossTable)
        {
            PdfAnnotation annot = new PdfTextMarkupAnnotation();
            annot.SetPage(m_page);
            return annot;
        }
        /// <summary>
        /// Creates the File Attachment Annotation.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <param name="rect">The RectangleF.</param>
        /// <param name="filename">The Filename.</param>
        /// <returns>The created File Attachment Annotation.</returns>
        private PdfAnnotation CreateFileAttachmentAnnotation(PdfDictionary dictionary, PdfCrossTable crossTable, RectangleF rect, string filename)
        {
            PdfLoadedAnnotation annot = new PdfLoadedAttachmentAnnotation(dictionary, crossTable, rect, filename);
            annot.SetPage(m_page);
            return annot;
        }
        /// <summary>
        /// Creates the Caret Annotation.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <returns>The created Caret Annotation.</returns>
        private PdfAnnotation CreateCaretAnnotation(PdfDictionary dictionary, PdfCrossTable crossTable)
        {
            PdfAnnotation annot = new PdfTextMarkupAnnotation();
            annot.SetPage(m_page);
            return annot;
        }
        /// <summary>
        /// Creates the Annotation States.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <returns>The created Annotation States.</returns>
        private PdfAnnotation CreateAnnotationStates(PdfDictionary dictionary, PdfCrossTable crossTable)
        {
            PdfAnnotation annot = new PdfTextMarkupAnnotation();
            annot.SetPage(m_page);
            return annot;
        }

        /// <summary>
        /// Inserts a annotation into collection.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="field">The annotation.</param>
        protected override void DoInsert(int index, PdfAnnotation annot)
        {
            if (index < 0 || index > List.Count)
                throw new IndexOutOfRangeException();

            if (annot == null)
                throw new ArgumentNullException("annotation");

            annot.SetPage(m_page);

            if (!(annot is PdfLoadedAnnotation))
            {
                PdfArray array = null;
                if (m_page.Dictionary.ContainsKey(DictionaryProperties.Annots))
                {
                    array = m_page.CrossTable.GetObject(m_page.Dictionary[DictionaryProperties.Annots]) as PdfArray;
                }
                else
                {
                    array = new PdfArray();
                }
                array.Insert(index, new PdfReferenceHolder(annot));
                m_page.Dictionary.SetProperty(DictionaryProperties.Annots, array);
            }

            base.DoInsert(index, annot);
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        protected override void DoClear()
        {
            for (int i = 0, size = List.Count; i < size; ++i)
            {
                PdfLoadedAnnotation annot = List[i] as PdfLoadedAnnotation;
                if (annot != null)
                {
                    m_page.RemoveFromDictionaries(annot);
                }
            }
            //  m_page.Annotations.Clear();
            // base.DoClear();
        }

        /// <summary>
        /// Removes the annotation at the specified position.
        /// </summary>
        /// <param name="index">The index.</param>
        protected override void DoRemoveAt(int index)
        {
            if (index < 0 || index > List.Count)
                throw new IndexOutOfRangeException();

            PdfAnnotation annot = List[index] as PdfAnnotation;

            if (annot is PdfLoadedAnnotation)
            {
                m_page.RemoveFromDictionaries(annot);
            }

            base.DoRemoveAt(index);
        }

        /// <summary>
        /// Removes the annotation from collection.
        /// </summary>
        /// <param name="field">The annotation.</param>
        protected override void DoRemove(PdfAnnotation annot)
        {
            if (annot == null)
                throw new ArgumentNullException("annotation");

            m_page.RemoveFromDictionaries(annot);

            base.DoRemove(annot);
        }

        /// <summary>
        /// Find the annotation from collection.
        /// </summary>
        /// <param name="arr">The annotation.</param>

        internal bool FindAnnotation(PdfArray arr)
        {
            if (arr == null)
                return false;
            for (int i = 0; i < arr.Count; i++)
            {
                if (arr[i] is PdfArray)
                {
                    PdfArray temp = arr[i] as PdfArray;
                    for (int j = 0; j < temp.Count; j++)
                    {
                        int val = (temp[j] as PdfNumber).IntValue;
                        if (val > 0)
                            return false;
                    }
                }
                else
                {
                    int val = (arr[i] as PdfNumber).IntValue;
                    if (val > 0)
                        return false;
                }
            }
            return true;
        }
        #endregion
    }
}
