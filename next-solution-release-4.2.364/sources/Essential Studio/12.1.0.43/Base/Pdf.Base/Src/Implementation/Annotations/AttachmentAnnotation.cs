#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Drawing;
using System.IO;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents an attachment annotation.
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// //Create a new Pdf Document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document.
    /// PdfPage page = document.Pages.Add();
    /// //Create a new rectangle
    /// RectangleF attachmentRectangle = new RectangleF(10, 40, 30, 30);
    /// //Create a new attachment annotation.
    /// PdfAttachmentAnnotation attachmentAnnotation = new PdfAttachmentAnnotation(attachmentRectangle,@"..\..\Images\business.jpg");
    /// //Set the attachment icon to attachment annotation.
    /// attachmentAnnotation.Icon = PdfAttachmentIcon.PushPin;
    /// //Add this annotation to a new page.
    /// page.Annotations.Add(attachmentAnnotation);
    /// //Save the document to disk.
    /// document.Save("AttachmentAnnotation.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document.
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create a new rectangle
    /// Dim attachmentRectangle As RectangleF = New RectangleF(10, 40, 30, 30)
    /// 'Create a new attachment annotation
    /// Dim attachmentAnnotation As PdfAttachmentAnnotation = New PdfAttachmentAnnotation(attachmentRectangle, "..\..\Images\business.jpg")
    /// 'Set the Attachment Iconto attachmentAnnotation.
    /// attachmentAnnotation.Icon = PdfAttachmentIcon.PushPin
    /// 'Add this annotation to a new page.
    /// page.Annotations.Add(attachmentAnnotation)
    /// 'Save the  document to disk.
    /// document.Save("AttachmentAnnotation.pdf")
    /// </code>
    /// </example> 
    public class PdfAttachmentAnnotation : PdfFileAnnotation
    {
        #region Fields
        /// <summary>
        /// Icon of the annotation.
        /// </summary>
        private PdfAttachmentIcon m_attachmentIcon = PdfAttachmentIcon.PushPin;

        /// <summary>
        /// File specification of the annotation.
        /// </summary>
        private PdfEmbeddedFileSpecification m_fileSpecification = null;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or Sets attachment's icon.
        /// </summary>
        /// <value>A <see cref="PdfAttachmentIcon"/> enumeration member specifying the icon for the annotation when it is displayed in closed state.</value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new Pdf Document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF attachmentRectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new attachment annotation.
        /// PdfAttachmentAnnotation attachmentAnnotation = new PdfAttachmentAnnotation(attachmentRectangle,@"..\..\Images\business.jpg");
        /// //Set the attachment icon to attachment annotation.
        /// attachmentAnnotation.Icon = PdfAttachmentIcon.PushPin;
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(attachmentAnnotation);
        /// //Save the  document to disk.
        /// document.Save("AttachmentAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new attachment annotation
        /// Dim attachmentAnnotation As PdfAttachmentAnnotation = New PdfAttachmentAnnotation(attachmentRectangle, "..\..\Images\business.jpg")
        /// 'Set the attachment icon to attachment annotation.
        /// attachmentAnnotation.Icon = PdfAttachmentIcon.PushPin
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(attachmentAnnotation)
        /// 'Save the  document to disk.
        /// document.Save("AttachmentAnnotation.pdf")
        /// </code>
        /// </example> 
        public PdfAttachmentIcon Icon
        {
            get
            {
                return this.m_attachmentIcon;
            }

            set
            {
                this.m_attachmentIcon = value;
                this.Dictionary.SetName(DictionaryProperties.Name, this.m_attachmentIcon.ToString());
            }
        }

#if SILVERLIGHT
        /// <summary>
        /// Security Critical : Gets or sets the file associated with this annotation. 
        /// </summary>
        //[System.Security.SecurityCritical]
#else
        /// <summary>
        /// Gets or sets the file associated with this annotation. 
        /// </summary>
#endif       
        /// <value>A string value specifying the full path to the file to be embedded in the PDF file.</value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new Pdf Document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new attachment annotation.
        /// PdfAttachmentAnnotation attachmentAnnotation = new PdfAttachmentAnnotation(attachmentRectangle,@"..\..\Images\business.jpg");
        /// //Gets the file name.
        ///  string fileName = PdfAttachmentIcon.FileName;
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(attachmentAnnotation);
        /// //Save the  document to disk.
        /// document.Save("AttachmentAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new attachment annotation
        /// Dim attachmentAnnotation As PdfAttachmentAnnotation = New PdfAttachmentAnnotation(attachmentRectangle, "..\..\Images\business.jpg")
        /// 'Gets the file name.
        /// Dim fileName As string = PdfAttachmentIcon.FileName
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(attachmentAnnotation)
        /// 'Save the  document to disk.
        /// document.Save("AttachmentAnnotation.pdf")
        /// </code>
        /// </example> 
        public override string FileName
        {
            get
            {
                return this.m_fileSpecification.FileName;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("FileName");
                }

                if (value.Length == 0)
                {
                    throw new ArgumentException("FileName can't be empty");
                }

                if (this.m_fileSpecification.FileName != value)
                {
                    this.m_fileSpecification.FileName = value;
                }
            }
        }
        #endregion

        #region Constructors
#if SILVERLIGHT
        /// <summary>
        /// Security Critical :  Initializes a new instance of the <see cref="PdfAttachmentAnnotation"/> class.
        /// </summary>
        [System.Security.SecurityCritical]
#else
        /// <summary>
        ///  Initializes a new instance of the <see cref="PdfAttachmentAnnotation"/> class.
        /// </summary>
#endif       
        /// <param name="rectangle">Bounds of the annotation.</param>
        /// <param name="fileName">A string value specifying the full path to the file to be embedded in the PDF file.</param>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new Pdf Document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new attachment annotation.
        /// PdfAttachmentAnnotation attachmentAnnotation = new PdfAttachmentAnnotation(attachmentRectangle,@"..\..\Images\business.jpg");
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(attachmentAnnotation);
        /// //Save the  document to disk.
        /// document.Save("AttachmentAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new attachment annotation
        /// Dim attachmentAnnotation As PdfAttachmentAnnotation = New PdfAttachmentAnnotation(attachmentRectangle, "..\..\Images\business.jpg")
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(attachmentAnnotation)
        /// 'Save the  document to disk.
        /// document.Save("AttachmentAnnotation.pdf")
        /// </code>
        /// </example> 
        public PdfAttachmentAnnotation(RectangleF rectangle, string fileName)
            : base(rectangle)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            this.m_fileSpecification = new PdfEmbeddedFileSpecification(fileName);
        }

#if SILVERLIGHT
        /// <summary>
        /// Security Critical : Initializes a new instance of the <see cref="PdfAttachmentAnnotation"/> class.
        /// </summary>
        [System.Security.SecurityCritical]
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfAttachmentAnnotation"/> class.
        /// </summary>
#endif     
        /// <param name="rectangle">Bounds of the annotation.</param>
        /// <param name="fileName">A string value specifying the full path to the file to be embedded in the PDF file.</param>
        /// <param name="data">A byte array specifying the content of the annotation's embedded file. </param>
        /// <remarks>If both FileName and FileContent are specified, the FileContent takes precedence. </remarks>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new Pdf Document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new attachment annotation.
        /// PdfAttachmentAnnotation attachmentAnnotation = new PdfAttachmentAnnotation(attachmentRectangle,@"..\..\Images\business.jpg");
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(attachmentAnnotation);
        /// //Save the  document to disk.
        /// document.Save("AttachmentAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new attachment annotation
        /// Dim attachmentAnnotation As PdfAttachmentAnnotation = New PdfAttachmentAnnotation(attachmentRectangle, "..\..\Images\business.jpg")
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(attachmentAnnotation)
        /// 'Save the  document to disk.
        /// document.Save("AttachmentAnnotation.pdf")
        /// </code>
        /// </example> 
        public PdfAttachmentAnnotation(RectangleF rectangle, string fileName, byte[] data)
            : base(rectangle)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            this.m_fileSpecification = new PdfEmbeddedFileSpecification(fileName, data);
        }

#if SILVERLIGHT
        /// <summary>
        /// Security Critical : Initializes a new instance of the <see cref="PdfAttachmentAnnotation"/> class.
        /// </summary>
        [System.Security.SecurityCritical]
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfAttachmentAnnotation"/> class.
        /// </summary>
#endif       
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="fileName">A string value specifying the full path to the file to be embedded in the PDF file.</param>
        /// <param name="stream">The stream specifying the content of the annotation's embedded file. </param>
        /// <remarks>If both FileName and FileContent are specified, the FileContent takes precedence. </remarks>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new Pdf Document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// FileStream stream=new FileStream("..\..\Images\business.jpg",FileMode.Open);
        /// //Create a new attachment annotation.
        /// PdfAttachmentAnnotation attachmentAnnotation = new PdfAttachmentAnnotation(attachmentRectangle, "..\..\Images\Sample.jpg",stream);
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(attachmentAnnotation);
        /// //Save the  document to disk.
        /// document.Save("AttachmentAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// Dim  stream As FileStream =New FileStream("..\..\Images\business.jpg",FileMode.Open);
        /// 'Create a new attachment annotation
        /// Dim attachmentAnnotation As PdfAttachmentAnnotation = New PdfAttachmentAnnotation(attachmentRectangle, "..\..\Images\Sample.jpg",stream)
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(attachmentAnnotation)
        /// 'Save the  document to disk.
        /// document.Save("AttachmentAnnotation.pdf")
        /// </code>
        /// </example> 
        public PdfAttachmentAnnotation(RectangleF rectangle, string fileName, Stream stream)
            : base(rectangle)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            this.m_fileSpecification = new PdfEmbeddedFileSpecification(fileName, stream);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Initializes object.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            this.Dictionary.SetProperty(DictionaryProperties.Subtype, new PdfName(DictionaryProperties.FileAttachment));
        }

        /// <summary>
        /// Saves annotation object.
        /// </summary>
        protected override void Save()
        {
            base.Save();
            this.Dictionary.SetProperty(DictionaryProperties.FS, new PdfReferenceHolder(this.m_fileSpecification));
        }
        #endregion
    }
}
