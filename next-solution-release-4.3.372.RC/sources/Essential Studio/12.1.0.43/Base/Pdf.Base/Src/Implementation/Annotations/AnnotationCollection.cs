#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
using System.Text;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents collection of <see cref="PdfAnnotation"/> objects.
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// //Create a new Pdf Document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document.
    /// PdfPage page = document.Pages.Add();
    /// //Create a new rectangle
    /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
    /// //Create a new sound annotation.
    /// PdfSoundAnnotation soundAnnotation = new PdfSoundAnnotation(rectangle, @"...\..\.Data\startup.wav");
    /// //Sets the pdf sound.
    /// soundAnnotation.Sound = new PdfSound("@..\..\Data\endsup.wav");
    /// PdfAnnotationCollection annotationCollection=page.Annotations;
    /// //Add this annotation to a new page.
    /// annotationCollection.Add(soundAnnotation);
    /// //Save the document to disk.
    /// document.Save("PdfSoundAnnotation.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document.
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create a new rectangle
    /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
    /// 'Create a new sound annotation.
    /// Dim soundAnnotation As PdfSoundAnnotation = New PdfSoundAnnotation(rectangle, "..\..\Data\startup.wav")
    /// 'Sets the pdf sound.
    /// soundAnnotation.Sound = New PdfSound("..\..\Data\endsup.wav")
    /// Dim annotationCollection As PdfAnnotationCollection =page.Annotations
    /// annotationCollection.Add(soundAnnotation)
    /// 'Save the document to disk.
    /// document.Save("PdfSoundAnnotation.pdf")
    /// </code>
    /// </example> 
    public class PdfAnnotationCollection :
        PdfCollection,
        IPdfWrapper
    {
        #region Constants
        /// <summary>
        /// Error constant message.
        /// </summary>
        private string AlreadyExistsAnnotationError =
            "This annotatation had been already added to page";

        /// <summary>
        /// Error constant message.
        /// </summary>
        private string MissingAnnotationException =
            "Annotation is not contained in collection.";
        #endregion

        #region Fields
        /// <summary>
        /// Parent page of the collection.
        /// </summary>
        private PdfPage m_page;

        /// <summary>
        /// Array of the annotations.
        /// </summary>
        private PdfArray m_annotations = new PdfArray();
        #endregion

        #region Properties
        /// <summary>
        /// Gets the <see cref="PdfAnnotation"/> object at the specified position.
        /// </summary>
        /// <param name="index">The index value of the annotation in the collection. </param>
        /// <returns>Annotation object at the specified position.</returns>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new Pdf Document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new sound annotation.
        /// PdfSoundAnnotation soundAnnotation = new PdfSoundAnnotation(rectangle, @"...\..\.Data\startup.wav");
        /// //Sets the pdf sound
        /// soundAnnotation.Sound = new PdfSound("@..\..\Data\endsup.wav");
        /// //Add this annotation to a new page.
        /// PdfAnnotationCollection annotationCollection=page.Annotations;
        /// annotationCollection.Add(soundAnnotation);
        /// PdfAnnotation annotation=annotationCollection[0] as PdfAnnotation;
        /// //Save the document to disk.
        /// document.Save("PdfSoundAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle.
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new sound annotation.
        /// Dim soundAnnotation As PdfSoundAnnotation = New PdfSoundAnnotation(rectangle, "..\..\Data\startup.wav")
        /// 'Sets the pdf sound.
        /// soundAnnotation.Sound = New PdfSound("..\..\Data\endsup.wav")
        /// Dim annotationCollection As PdfAnnotationCollection =page.Annotations
        /// annotationCollection.Add(soundAnnotation)
        /// Dim annotation As PdfAnnotation = annotationCollection[0]
        /// 'Save the  document to disk.
        /// document.Save("PdfSoundAnnotation.pdf")
        /// </code>
        /// </example> 
        public virtual PdfAnnotation this[int index]
        {
            get
            {
                if (index < 0 || index > Count - 1)
                {
                    throw new ArgumentOutOfRangeException("index", "Index is out of range.");
                }

                PdfAnnotation annotation = (PdfAnnotation)List[index];
                return annotation;
            }
        }

        /// <summary>
        /// Gets the annotations array.
        /// </summary>
        /// <value>The annotations.</value>
        internal PdfArray Annotations
        {
            get
            {
                return this.m_annotations;
            }
            set
            {
                this.m_annotations = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfAnnotationCollection"/> class.
        /// </summary>
        public PdfAnnotationCollection()
            : base()
        { }

        /// <summary>
        /// Creates new annotation collection for the specified page.
        /// </summary>
        /// <param name="page">Page which collection is created for.</param>
        public PdfAnnotationCollection(PdfPage page)
            : base()
        {
            if (page == null)
                throw new ArgumentNullException("page");

            this.m_page = page;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds a new annotation to collection. 
        /// </summary>
        /// <param name="annotation">The new annotation to be added to collection.</param>
        /// <returns>Position of the annotation in collection.</returns>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new Pdf Document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle.
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new sound annotation.
        /// PdfSoundAnnotation soundAnnotation = new PdfSoundAnnotation(rectangle, @"...\..\.Data\startup.wav");
        /// //Sets the pdf sound.
        /// soundAnnotation.Sound = new PdfSound("@..\..\Data\endsup.wav");
        /// //Add this annotation to a new page.
        /// PdfAnnotationCollection annotationCollection=page.Annotations;
        /// annotationCollection.Add(soundAnnotation);
        /// //Save this document to disk.
        /// document.Save("PdfSoundAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new sound annotation.
        /// Dim soundAnnotation As PdfSoundAnnotation = New PdfSoundAnnotation(rectangle, "..\..\Data\startup.wav")
        /// 'Sets the pdf sound.
        /// soundAnnotation.Sound = New PdfSound("..\..\Data\endsup.wav")
        /// Dim annotationCollection As PdfAnnotationCollection =page.Annotations
        /// annotationCollection.Add(soundAnnotation)
        /// 'Save the document to disk.
        /// document.Save("PdfSoundAnnotation.pdf")
        /// </code>
        /// </example> 
        public virtual int Add(PdfAnnotation annotation)
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
            SetPrint(annotation);            
            return DoAdd(annotation);
        }

        /// <summary>
        /// Cleares the collection.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new Pdf Document.
        /// PdfDocument document=new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new sound annotation.
        /// PdfSoundAnnotation soundAnnotation = new PdfSoundAnnotation(rectangle, @"...\..\.Data\startup.wav");
        /// //Sets the pdf sound.
        /// soundAnnotation.Sound = new PdfSound("@..\..\Data\endsup.wav");
        /// //Add this annotation to a new page.
        /// PdfAnnotationCollection annotationCollection=page.Annotations;
        /// annotationCollection.Add(soundAnnotation);
        /// //Clear the annotation collection.
        /// annotationCollection.Clear();
        /// //Save the  document to disk.
        /// document.Save("PdfSoundAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new sound annotation.
        /// Dim soundAnnotation As PdfSoundAnnotation = New PdfSoundAnnotation(rectangle, "..\..\Data\startup.wav")
        /// 'Sets the pdf sound.
        /// soundAnnotation.Sound = New PdfSound("..\..\Data\endsup.wav")
        /// Dim annotationCollection As PdfAnnotationCollection =page.Annotations
        /// annotationCollection.Add(soundAnnotation)
        /// 'Clear the annotation collection.
        /// annotationCollection.Clear()
        /// 'Save the  document to disk.
        /// document.Save("PdfSoundAnnotation.pdf")
        /// </code>
        /// </example> 
        public void Clear()
        {
            DoClear();
        }

        /// <summary>
        /// Searches the collection for the specified annotation. 
        /// </summary>
        /// <param name="annotation">The annotation to search for.</param>
        /// <returns>True, if annotation is contained in collection. Otherwise - false.</returns>
        /// <example>
        /// <code lang = "C#">
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new sound annotation.
        /// PdfSoundAnnotation soundAnnotation = new PdfSoundAnnotation(rectangle, @"...\..\.Data\startup.wav");
        /// //Sets the pdf sound.
        /// soundAnnotation.Sound = new PdfSound("@..\..\Data\endsup.wav");
        /// //Add this annotation to a new page.
        /// PdfAnnotationCollection annotationCollection=page.Annotations;
        /// //Addes the sound annotation to annotation collection.
        /// annotationCollection.Add(soundAnnotation);
        /// bool exist = annotationCollection.Contains(soundAnnotation);
        /// </code>
        /// <code lang="VB">
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new sound annotation.
        /// Dim soundAnnotation As PdfSoundAnnotation = New PdfSoundAnnotation(rectangle, "..\..\Data\startup.wav")
        /// 'Sets the pdf sound.
        /// soundAnnotation.Sound = New PdfSound("..\..\Data\endsup.wav")
        /// Dim annotationCollection As PdfAnnotationCollection =page.Annotations
        /// 'Addes the sound annotation to annotation collection.
        /// annotationCollection.Add(soundAnnotation)
        /// Dim exist As bool=annotationCollection.Contains(soundAnnotation)
        /// </code>
        /// </example> 
        public bool Contains(PdfAnnotation annotation)
        {
            if (annotation == null)
                throw new ArgumentNullException("annotation");

            bool contains = List.Contains(annotation);
            return contains;
        }

        /// <summary>
        /// Searches the collection for the specified annotation. 
        /// </summary>
        /// <param name="annotation">The Annotation to search.</param>
        /// <returns>Index of the element in the collection, if exists, or -1 if the element does not exist in the collection.</returns>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new Pdf Document.
        /// PdfDocument document=new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new sound annotation.
        /// PdfSoundAnnotation soundAnnotation = new PdfSoundAnnotation(rectangle, @"...\..\.Data\startup.wav");
        /// //Sets the pdf sound.
        /// soundAnnotation.Sound = new PdfSound("@..\..\Data\endsup.wav");
        /// //Add this annotation to a new page.
        /// PdfAnnotationCollection annotationCollection=page.Annotations;
        /// annotationCollection.Add(soundAnnotation);
        /// int index =annotationCollection.IndexOf(soundAnnotation);
        /// //Save the document to disk.
        /// document.Save("PdfSoundAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new sound annotation.
        /// Dim soundAnnotation As PdfSoundAnnotation = New PdfSoundAnnotation(rectangle, "..\..\Data\startup.wav")
        /// 'Sets the pdf sound.
        /// soundAnnotation.Sound = New PdfSound("..\..\Data\endsup.wav")
        /// Dim annotationCollection As PdfAnnotationCollection =page.Annotations
        /// annotationCollection.Add(soundAnnotation)
        /// Dim index As int=annotationCollection.IndexOf(soundAnnotation)
        /// 'Save the document to disk.
        /// document.Save("PdfSoundAnnotation.pdf")
        /// </code>
        /// </example> 
        public int IndexOf(PdfAnnotation annotation)
        {
            if (annotation == null)
                throw new ArgumentNullException("annotation");

            int index = List.IndexOf(annotation);
            return index;
        }

        /// <summary>
        /// Inserts annotation to the collection at the specified index.
        /// </summary>
        /// <param name="index">Index where to insert the element.</param>
        /// <param name="annotation">The annotation to insert in the collection.</param>
        /// <example>
        /// <code lang = "C#">
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new sound annotation.
        /// PdfSoundAnnotation soundAnnotation = new PdfSoundAnnotation(rectangle, @"...\..\.Data\startup.wav");
        /// //Sets the pdf sound.
        /// soundAnnotation.Sound = new PdfSound("@..\..\Data\endsup.wav");
        /// //Add this annotation to a new page.
        /// PdfAnnotationCollection annotationCollection=page.Annotations;
        /// annotationCollection.Insert(1,soundAnnotation);
        /// </code>
        /// <code lang="VB">
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new sound annotation.
        /// Dim soundAnnotation As PdfSoundAnnotation = New PdfSoundAnnotation(rectangle, "..\..\Data\startup.wav")
        /// 'Sets the pdf sound.
        /// soundAnnotation.Sound = New PdfSound("..\..\Data\endsup.wav")
        /// Dim annotationCollection As PdfAnnotationCollection =page.Annotations
        /// annotationCollection.Insert(1,soundAnnotation)
        /// </code>
        /// </example> 
        public void Insert(int index, PdfAnnotation annotation)
        {
            DoInsert(index, annotation);
        }

        /// <summary>
        /// Removes the element at the specified field.
        /// </summary>
        /// <param name="index">The index of the element to remove.</param>
        /// <example>
        /// <code lang = "C#">
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new sound annotation.
        /// PdfSoundAnnotation soundAnnotation = new PdfSoundAnnotation(rectangle, @"...\..\.Data\startup.wav");
        /// //Sets the pdf sound.
        /// soundAnnotation.Sound = new PdfSound("@..\..\Data\endsup.wav");
        /// //Add this annotation to a new page.
        /// PdfAnnotationCollection annotationCollection=page.Annotations;
        /// annotationCollection.Add(soundAnnotation);
        /// annotationCollection.RemoveAt(0)
        /// </code>
        /// <code lang="VB">
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new sound annotation.
        /// Dim soundAnnotation As PdfSoundAnnotation = New PdfSoundAnnotation(rectangle, "..\..\Data\startup.wav")
        /// 'Sets the pdf sound.
        /// soundAnnotation.Sound = New PdfSound("..\..\Data\endsup.wav")
        /// Dim annotationCollection As PdfAnnotationCollection =page.Annotations
        /// annotationCollection.Add(soundAnnotation)
        /// annotationCollection.RemoveAt(0)
        /// </code>
        /// </example> 
        public void RemoveAt(int index)
        {
            if (index < 0 || index > Count - 1)
                throw new ArgumentOutOfRangeException("index", "Index is out of range.");

            RemoveAnnotationAt(index);
        }

        /// <summary>
        /// Removes the element from the collection. 
        /// </summary>
        /// <param name="field">The element to remove.</param>
        /// <example>
        /// <code lang = "C#">
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new sound annotation.
        /// PdfSoundAnnotation soundAnnotation = new PdfSoundAnnotation(rectangle, @"...\..\.Data\startup.wav");
        /// //Sets the pdf sound.
        /// soundAnnotation.Sound = new PdfSound("@..\..\Data\endsup.wav");
        /// //Add this annotation to a new page.
        /// PdfAnnotationCollection annotationCollection=page.Annotations;
        /// annotationCollection.Add(soundAnnotation);
        /// //Removes a sound annotation.
        /// annotationCollection.Remove(soundAnnotation);
        /// </code>
        /// <code lang="VB">
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new sound annotation.
        /// Dim soundAnnotation As PdfSoundAnnotation = New PdfSoundAnnotation(rectangle, "..\..\Data\startup.wav")
        /// 'Sets the pdf sound.
        /// soundAnnotation.Sound = New PdfSound("..\..\Data\endsup.wav")
        /// Dim annotationCollection As PdfAnnotationCollection =page.Annotations
        /// annotationCollection.Add(soundAnnotation)
        /// 'Removes a sound annotation.
        /// annotationCollection.Remove(soundAnnotation)
        /// </code>
        /// </example> 
        public void Remove(PdfAnnotation annot)
        {
            if (annot == null)
            {
                throw new ArgumentNullException("annotation");
            }

            DoRemove(annot);
        }

        public void SetPrint(PdfAnnotation annot)
        {
            if (m_page.Document.Conformance == PdfConformanceLevel.Pdf_A1B)
                annot.Dictionary.SetNumber(DictionaryProperties.F, (int)PdfAnnotationFlags.Print);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Adds annotation to collection.
        /// </summary>
        /// <param name="annotation">Annotation to be added to collection.</param>
        /// <returns>Position of the annotation in collection.</returns>
        private int AddAnnotation(PdfAnnotation annotation)
        {
            annotation.SetPage(m_page);
            int index = List.Add(annotation);
            m_annotations.Add(new PdfReferenceHolder(annotation));

            return index;
        }

        /// <summary>
        /// Inserts annotation to the collection at the specified position.
        /// </summary>
        /// <param name="index">Position.</param>
        /// <param name="annotation">Annotation object.</param>
        private void InsertAnnotation(int index, PdfAnnotation annotation)
        {
            annotation.SetPage(m_page);
            List.Insert(index, annotation);
            m_annotations.Insert(index, new PdfReferenceHolder(annotation));
        }

        /// <summary>
        /// Removes annotation from collection.
        /// </summary>
        /// <param name="annotation">Annotation to be removed.</param>
        private void RemoveAnnotation(PdfAnnotation annotation)
        {
            int index = List.IndexOf(annotation);
            annotation.SetPage(null);
            List.Remove(annotation);
            m_annotations.RemoveAt(index);
        }

        /// <summary>
        /// Removes item from collection at the specified index.
        /// </summary>
        /// <param name="index">Index of element to be removed.</param>
        private void RemoveAnnotationAt(int index)
        {
            DoRemoveAt(index);
        }

        /// <summary>
        /// Adds a Annotation to collection.
        /// </summary>
        /// <param name="field">The Annotation.</param>
        /// <returns></returns>
        protected virtual int DoAdd(PdfAnnotation annot)
        {
            annot.SetPage(m_page);
            m_annotations.Add(new PdfReferenceHolder(annot));
            return List.Add(annot);
        }

        /// <summary>
        /// Inserts a annotation into collection.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="field">The annotation.</param>
        protected virtual void DoInsert(int index, PdfAnnotation annot)
        {
            m_annotations.Insert(index, new PdfReferenceHolder(annot));
            List.Insert(index, annot);
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        protected virtual void DoClear()
        {
            m_annotations.Clear();
            List.Clear();
        }

        /// <summary>
        /// Removes the annotation at the specified position.
        /// </summary>
        /// <param name="index">The index.</param>
        protected virtual void DoRemoveAt(int index)
        {
            m_annotations.RemoveAt(index);
            List.RemoveAt(index);
        }
        /// <summary>
        /// Removes the annotation.
        /// </summary>
        /// <param name="annot"></param>
        protected virtual void DoRemove(PdfAnnotation annot)
        {
            int index = List.IndexOf(annot);
            m_annotations.RemoveAt(index);
            List.RemoveAt(index);
        }

        #endregion

        #region IPdfWrapper Members
        /// <summary>
        /// Gets Pdf primitive representing this object.
        /// </summary>
        IPdfPrimitive IPdfWrapper.Element
        {
            get
            {
                return m_annotations;
            }
        }
        #endregion
    }
}
