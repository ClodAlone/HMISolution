#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Drawing;
using Syncfusion.Pdf.IO;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents annotation object with holds link on another location within a document.
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document.
    /// PdfPage page = document.Pages.Add();
    /// //Create a new rectangle
    /// RectangleF docLinkAnnotationRectangle = new RectangleF(10, 40, 30, 30);
    /// //Create a new document link annotation.
    ///  PdfDocumentLinkAnnotation documentAnnotation = new PdfDocumentLinkAnnotation(docLinkAnnotationRectangle);
    /// //Set the annotation flags.
    /// documentAnnotation.AnnotationFlags = PdfAnnotationFlags.NoRotate;
    /// //Set the annotation text.
    /// documentAnnotation.Text = "Document link annotation";
    /// //Set the annotation's color.
    /// documentAnnotation.Color = new PdfColor(Color.Navy);
    /// //Creates a new page and adds it as the last page of the document.
    /// PdfPage page2 = document.Pages.Add();
    /// //Set the pdf destination.
    /// documentAnnotation.Destination = new PdfDestination(page2);
    /// //Set the documentlink annotation location.
    /// documentAnnotation.Destination.Location = new Point(10, 0);
    /// //Set the document annotation zool level
    /// documentAnnotation.Destination.Zoom = 5;
    /// //Add this annotation to a new page.
    /// page.Annotations.Add(attachmentAnnotation);
    /// //Save the  document to disk.
    /// document.Save("DocumentLinkAnnotation.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document.
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create a new rectangle
    /// Dim docLinkAnnotationRectangle As RectangleF = New RectangleF(10, 40, 30, 30)
    /// 'Create a new document link annotation.
    /// Dim documentAnnotation As PdfDocumentLinkAnnotation = New PdfDocumentLinkAnnotation(docLinkAnnotationRectangle)
    /// 'Set the annotation flags.
    /// documentAnnotation.AnnotationFlags = PdfAnnotationFlags.NoRotate\
    /// 'Set the annotation text.
    /// documentAnnotation.Text = "Document link annotation"
    /// 'Set the color.
    /// documentAnnotation.Color = New PdfColor(Color.Navy)
    /// 'Creates a new page and adds it as the last page of the document.
    /// Dim page2 As PdfPage = document.Pages.Add()
    /// 'Set the pdf destination.
    /// documentAnnotation.Destination = New PdfDestination(page2)
    /// 'Set the document annotation destination location.
    /// documentAnnotation.Destination.Location = New Point(10, 0)
    /// 'Set the document annotation zool level
    /// documentAnnotation.Destination.Zoom = 5
    /// 'Add this annotation to a new page.
    /// page.Annotations.Add(documentAnnotation)
    /// 'Save the  document to disk.
    /// document.Save("DocumentLinkAnnotation.pdf")
    /// </code>
    /// </example> 
    public class PdfDocumentLinkAnnotation : PdfLinkAnnotation
    {
        #region Fields
        /// <summary>
        /// Destination of the annotation.
        /// </summary>
        private PdfDestination m_destination = null;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the destination of the annotation.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF docLinkAnnotationRectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new documentlink annotation.
        ///  PdfDocumentLinkAnnotation documentAnnotation = new PdfDocumentLinkAnnotation(docLinkAnnotationRectangle);
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page2 = document.Pages.Add();
        /// //Set the pdf destination.
        /// documentAnnotation.Destination = new PdfDestination(page2);
        /// //Set the pdf destination location.
        /// documentAnnotation.Destination.Location = new Point(10, 0);
        /// //Set the document link annotation zool level
        /// documentAnnotation.Destination.Zoom = 5;
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(attachmentAnnotation);
        /// //Save the  document to disk.
        /// document.Save("Destination.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim docLinkAnnotationRectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new document link annotation.
        /// Dim documentAnnotation As PdfDocumentLinkAnnotation = New PdfDocumentLinkAnnotation(docLinkAnnotationRectangle)
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page2 As PdfPage = document.Pages.Add()
        /// 'Set the pdf destination.
        /// documentAnnotation.Destination = New PdfDestination(page2)
        /// 'Set the document link annotation destination location.
        /// documentAnnotation.Destination.Location = New Point(10, 0)
        /// 'Set the document link annotation zool level
        /// documentAnnotation.Destination.Zoom = 5
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(documentAnnotation)
        /// 'Save the  document to disk.
        /// document.Save("Destination.pdf")
        /// </code>
        /// </example> 
        public PdfDestination Destination
        {
            get
            {
                return this.m_destination;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Destination");
                }

                if (this.m_destination != value)
                {
                    this.m_destination = value;
                }
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes new <see cref="PdfDocumentLinkAnnotation"/> instance.
        /// </summary>
        /// <param name="rectangle">Bounds of the annotation.</param>
        /// <example>
        /// <code lang = "C#">
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF docLinkAnnotationRectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new document link annotation.
        /// PdfDocumentLinkAnnotation documentAnnotation = new PdfDocumentLinkAnnotation(docLinkAnnotationRectangle);
        /// //Set the Document Annotation Text.
        /// documentAnnotation.Text = "Document link annotation";
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page2 = document.Pages.Add();
        /// //Set the document link annotation destination.
        /// documentAnnotation.Destination = new PdfDestination(page2);
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(attachmentAnnotation);
        /// //Save the  document to disk.
        /// document.Save("DocumentLinkAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim docLinkAnnotationRectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new document link annotation.
        /// Dim documentAnnotation As PdfDocumentLinkAnnotation = New PdfDocumentLinkAnnotation(docLinkAnnotationRectangle)
        /// 'Set the annotation Text.
        /// documentAnnotation.Text = "Document link annotation"
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page2 As PdfPage = document.Pages.Add()
        /// 'Set the document link annotation destination.
        /// documentAnnotation.Destination = New PdfDestination(page2)
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(documentAnnotation)
        /// 'Save the document to disk.
        /// document.Save("DocumentLinkAnnotation.pdf")
        /// </code>
        /// </example> 
        public PdfDocumentLinkAnnotation(RectangleF rectangle)
            : base(rectangle)
        {
        }

        /// <summary>
        /// Initializes new <see cref="PdfDocumentLinkAnnotation"/> instance.
        /// </summary>
        /// <param name="rectangle">Bounds of the annotation.</param>
        /// <param name="destination">Destination of the annotation.</param>
        /// <example>
        /// <code lang = "C#">
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page2 = document.Pages.Add();
        /// //Create a new pdf destination.
        /// PdfDestination destination=new new PdfDestination(page2);
        /// //Create a new document link annotation.
        ///  PdfDocumentLinkAnnotation documentAnnotation = new PdfDocumentLinkAnnotation(docLinkAnnotationRectangle,destination);
        /// //Set the annotation text.
        /// documentAnnotation.Text = "Document link annotation";
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(attachmentAnnotation);
        /// //Save the document to disk.
        /// document.Save("DocumentLinkAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page2 As PdfPage = document.Pages.Add()
        /// 'Create a new pdf destination.
        /// Dim destination AS PdfDestination =New PdfDestination(page2)
        /// 'Create a new document link annotation.
        /// Dim documentAnnotation As PdfDocumentLinkAnnotation = New PdfDocumentLinkAnnotation(docLinkAnnotationRectangle,destination)
        /// 'Set the annotation text.
        /// documentAnnotation.Text = "Document link annotation"
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(documentAnnotation)
        /// 'Save the  document to disk.
        /// document.Save("DocumentLinkAnnotation.pdf")
        /// </code>
        /// </example> 
        public PdfDocumentLinkAnnotation(RectangleF rectangle, PdfDestination destination)
            : base(rectangle)
        {
            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }

            this.Destination = destination;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Saves annotation object.
        /// </summary>
        protected override void Save()
        {
            base.Save();

            if (this.m_destination != null)
            {
                Dictionary.SetProperty(DictionaryProperties.Dest, this.m_destination);
            }
        }
        #endregion
    }
}
