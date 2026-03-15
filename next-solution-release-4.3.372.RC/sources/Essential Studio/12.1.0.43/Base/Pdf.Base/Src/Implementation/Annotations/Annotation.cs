#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Drawing;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents the base class for annotation objects.
    /// </summary>
    /// <seealso cref="PdfUriAnnotation"/> Class
    /// <seealso cref="SoundAnnotation"/> Class
    /// <seealso cref="RubberStampAnnotation"/> Class
    /// <seealso cref="PopupAnnotation"/> Class
    /// <seealso cref="PdfTextWebLink"/> Class
    /// <seealso cref="PdfTextMarkupAnnotation"/> Class
    /// <seealso cref="PdfLineAnnotation"/> Class
    /// <seealso cref="PdfFileLinkAnnotation"/> Class
    /// <seealso cref="PdfDocumentLinkAnnotation"/> Class
    public abstract class PdfAnnotation : IPdfWrapper
    {
        #region Fields
        /// <summary>
        /// Color of the annotation.
        /// </summary>
        private PdfColor m_color = PdfColor.Empty;
        /// <summary>
        /// Border of the annotation.
        /// </summary>
        private PdfAnnotationBorder m_border;

        /// <summary>
        /// Bounds of the annotation.
        /// </summary>
        private RectangleF m_rectangle = RectangleF.Empty;

        /// <summary>
        /// Parent page of the annotation.
        /// </summary>
        private PdfPage m_page = null;

        /// <summary>
        /// Text of the annotation.
        /// </summary>
        private string m_text = String.Empty;

        /// <summary>
        /// NAnootation's style flags.
        /// </summary>
        private PdfAnnotationFlags m_annotationFlags;

        /// <summary>
        /// Internal variable to store dictionary.
        /// </summary>
        private PdfDictionary m_dictionary = new PdfDictionary();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the background of the annotation�s icon when closed.
        /// The title bar of the annotation�s pop-up window.
        /// The border of a link annotation.
        /// </summary>
        /// <value>The color.</value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle.
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new sound annotation.
        /// PdfSoundAnnotation soundAnnotation = new PdfSoundAnnotation(rectangle, @"...\..\.Data\startup.wav");
        /// //Set the color.
        /// soundAnnotation.Color = new PdfColor(Color.Red);
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(soundAnnotation);
        /// //Save the  document to disk.
        /// document.Save("SoundAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a a new rectangle.
        /// Dim rectangle As RectangleF  = New RectangleF(10, 40, 30, 30);
        /// 'Create a new sound annotation.
        /// Dim soundAnnotation As PdfSoundAnnotation = New PdfSoundAnnotation(rectangle, "..\..\Data\startup.wav")
        /// 'Set the color.
        /// soundAnnotation.Color = New PdfColor(Color.Red)
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(soundAnnotation)
        /// 'Save the document to disk.
        /// document.Save("SoundAnnotation.pdf")
        /// </code>
        /// </example> 
        public PdfColor Color
        {
            get
            {
                return this.m_color;
            }

            set
            {
                if (this.m_color != value)
                {
                    this.m_color = value;

                    PdfColorSpace cs = PdfColorSpace.RGB;

                    if (this.Page != null)
                    {
                        cs = this.Page.Section.Parent.Document.ColorSpace;
                    }

                    PdfArray colours = this.m_color.ToArray(cs);

                    this.m_dictionary.SetProperty(DictionaryProperties.C, colours);
                }
            }
        }

        /// <summary>
        /// Gets or sets annotation's border.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle.
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new sound annotation.
        /// PdfSoundAnnotation soundAnnotation = new PdfSoundAnnotation(rectangle, @"...\..\.Data\startup.wav");
        /// //Assign the border to sound annotation.
        /// soundAnnotation.Border = new PdfAnnotationBorder(5);
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(soundAnnotation);
        /// //Save the document to disk.
        /// document.Save("SoundAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF  = New RectangleF(10, 40, 30, 30)
        /// 'Create a new sound annotation.
        /// Dim soundAnnotation As PdfSoundAnnotation = New PdfSoundAnnotation(rectangle, "..\..\Data\startup.wav")
        /// 'Assign the border to sound annotation.
        /// soundAnnotation.Border = New PdfAnnotationBorder(5)
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(soundAnnotation)
        /// 'Save the  document to disk.
        /// document.Save("SoundAnnotation.pdf")
        /// </code>
        /// </example> 
        public PdfAnnotationBorder Border
        {
            get
            {
                if (this.m_border == null)
                {
                    this.m_border = new PdfAnnotationBorder();
                }

                return this.m_border;
            }

            set
            {
                this.m_border = value;
                this.Dictionary.SetProperty(DictionaryProperties.Border, this.m_border);
            }
        }

        /// <summary>
        /// Gets or sets annotation's bounds. If this property is not set bounds are calculated automatically
        /// based on <see cref="Location">Location</see> property and content of annotation.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle.
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new sound annotation.
        /// PdfSoundAnnotation soundAnnotation = new PdfSoundAnnotation(rectangle, @"...\..\.Data\startup.wav");
        /// //Assign the bounds to sound annotation.
        /// soundAnnotation.Bounds=new RectangleF(50, 100, 30, 30);
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(soundAnnotation);
        /// //Save the  document to disk.
        /// document.Save("SoundAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle.
        /// Dim rectangle As RectangleF  = New RectangleF(10, 40, 30, 30)
        /// 'Create a new sound annotation.
        /// Dim soundAnnotation As PdfSoundAnnotation = New PdfSoundAnnotation(rectangle, "..\..\Data\startup.wav")
        /// 'Assign the bounds to sound annotation.
        /// soundAnnotation.Bounds=New RectangleF(50, 100, 30, 30)
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(soundAnnotation)
        /// 'Save the  document to disk.
        /// document.Save("SoundAnnotation.pdf")
        /// </code>
        /// </example> 
        public RectangleF Bounds
        {
            get
            {
                return this.m_rectangle;
            }

            set
            {
                if (this.m_rectangle != value)
                {
                    this.m_rectangle = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets location of the annotation.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new sound annotation.
        /// PdfSoundAnnotation soundAnnotation = new PdfSoundAnnotation(rectangle, @"...\..\.Data\startup.wav");
        /// //Assign the location to sound annotation.
        /// soundAnnotation.Location=new PointF(50, 100);
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(soundAnnotation);
        /// //Save the  document to disk.
        /// document.Save("PdfAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF  = New RectangleF(10, 40, 30, 30)
        /// 'Create a new sound annotation.
        /// Dim soundAnnotation As PdfSoundAnnotation = New PdfSoundAnnotation(rectangle, "..\..\Data\startup.wav")
        /// 'Assign the location to sound annotation.
        /// soundAnnotation.Location=New PointF(50, 100)
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(soundAnnotation)
        /// 'Save the  document to disk.
        /// document.Save("PdfAnnotation.pdf")
        /// </code>
        /// </example> 
        public PointF Location
        {
            get
            {
                return this.m_rectangle.Location;
            }

            set
            {
                this.m_rectangle = this.Bounds;
                this.m_rectangle.Location = value;
                this.Dictionary.SetProperty(DictionaryProperties.Rect, PdfArray.FromRectangle(this.m_rectangle));
            }
        }

        /// <summary>
        /// Gets or sets size of the annotation.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new sound annotation.
        /// PdfSoundAnnotation soundAnnotation = new PdfSoundAnnotation(rectangle, @"...\..\.Data\startup.wav");
        /// //Assign the size to sound annotation.
        /// soundAnnotation.Size=new SizeF(50, 50);
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(soundAnnotation);
        /// //Save the  document to disk.
        /// document.Save("PdfAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF  = New RectangleF(10, 40, 30, 30)
        /// 'Create a new sound annotation.
        /// Dim soundAnnotation As PdfSoundAnnotation = New PdfSoundAnnotation(rectangle, "..\..\Data\startup.wav")
        /// 'Assign the size to sound annotation.
        /// soundAnnotation.Size=New SizeF(50, 50)
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(soundAnnotation)
        /// 'Save the  document to disk.
        /// document.Save("PdfAnnotation.pdf")
        /// </code>
        /// </example> 
        public SizeF Size
        {
            get
            {
                return this.m_rectangle.Size;
            }

            set
            {
                this.m_rectangle = this.Bounds;
                this.m_rectangle.Size = value;
                this.Dictionary.SetProperty(DictionaryProperties.Rect, PdfArray.FromRectangle(this.m_rectangle));
            }
        }

        /// <summary>
        /// Gets a page which this annotation is connected to.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new sound annotation.
        /// PdfSoundAnnotation soundAnnotation = new PdfSoundAnnotation(rectangle, @"...\..\.Data\startup.wav");
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(soundAnnotation)
        /// //Gets the page.
        /// PdfPage pdfPage=soundAnnotation.page;
        /// //Save the  document to disk.
        /// document.Save("PdfAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF  = New RectangleF(10, 40, 30, 30)
        /// 'Create a new sound annotation.
        /// Dim soundAnnotation As PdfSoundAnnotation = New PdfSoundAnnotation(rectangle, "..\..\Data\startup.wav")
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(soundAnnotation)
        /// 'Gets the page.
        /// Dim pdfPage As PdfPage =soundAnnotation.page;
        /// 'Save the  document to disk.
        /// document.Save("PdfAnnotation.pdf")
        /// </code>
        /// </example> 
        public PdfPage Page
        {
            get
            {
                return this.m_page;
            }
        }

        /// <summary>
        /// Gets or sets content of the annotation.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new sound annotation.
        /// PdfSoundAnnotation soundAnnotation = new PdfSoundAnnotation(rectangle, @"...\..\.Data\startup.wav");
        /// //Set the text to sound annotation.
        /// soundAnnotation.Text="Sound Annotation";
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(soundAnnotation);
        /// //Save the  document to disk.
        /// document.Save("PdfAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim rectangle As RectangleF  = New RectangleF(10, 40, 30, 30)
        /// 'Create a new sound annotation.
        /// Dim soundAnnotation As PdfSoundAnnotation = New PdfSoundAnnotation(rectangle, "..\..\Data\startup.wav")
        /// 'Set the text to sound annotation.
        /// soundAnnotation.Text="Sound Annotation"
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(soundAnnotation)
        /// 'Save the  document to disk.
        /// document.Save("PdfAnnotation.pdf")
        /// </code>
        /// </example> 
        public string Text
        {
            get
            {
                return this.m_text;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Text");
                }

                if (this.m_text != value)
                {
                    this.m_text = value;
                    this.Dictionary.SetString(DictionaryProperties.Contents, this.m_text);
                }
            }
        }

        /// <summary>
        /// Gets or sets annotation flags.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new sound annotation.
        /// PdfSoundAnnotation soundAnnotation = new PdfSoundAnnotation(rectangle, @"...\..\.Data\startup.wav");
        /// //Set the annotation flags to sound annotation.
        /// soundAnnotation.AnnotationFlags = PdfAnnotationFlags.Print;
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(soundAnnotation);
        /// //Save the  document to disk.
        /// document.Save("PdfAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle.
        /// Dim rectangle As RectangleF  = New RectangleF(10, 40, 30, 30)
        /// 'Create a new sound annotation.
        /// Dim soundAnnotation As PdfSoundAnnotation = New PdfSoundAnnotation(rectangle, "..\..\Data\startup.wav")
        /// 'Set the annotation flags to sound annotation.
        /// soundAnnotation.AnnotationFlags = PdfAnnotationFlags.Print
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(soundAnnotation)
        /// 'Save the  document to disk.
        /// document.Save("PdfAnnotation.pdf")
        /// </code>
        /// </example> 
        public PdfAnnotationFlags AnnotationFlags
        {
            get
            {
                return this.m_annotationFlags;
            }

            set
            {
                if (this.m_annotationFlags != value)
                {
                    this.m_annotationFlags = value;
                    this.m_dictionary.SetNumber(DictionaryProperties.F, (int)this.m_annotationFlags);
                }
            }
        }

        /// <summary>
        /// Gets the dictionary.
        /// </summary>
        /// <value>The dictionary.</value>
        internal PdfDictionary Dictionary
        {
            get
            {
                return this.m_dictionary;
            }

            set
            {
                this.m_dictionary = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates the Constructors.
        /// </summary>
        internal PdfAnnotation()
        {
            this.Initialize();
        }

        /// <summary>
        /// Creates new annotation object with the specified bounds.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="text">The text.</param>
        protected PdfAnnotation(PdfPageBase page, string text)
            : base()
        {
            this.Initialize();
            this.m_page = page as PdfPage;
            this.m_text = text;
            this.m_dictionary.SetProperty(DictionaryProperties.Contents, new PdfString(text));
        }

        /// <summary>
        /// Creates new annotation object with the specified bounds.
        /// </summary>
        /// <param name="bounds">Bounds of the annotation.</param>
        protected PdfAnnotation(RectangleF bounds)
            : base()
        {
            this.Initialize();
            this.Bounds = bounds;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfAnnotation"/> class.
        /// </summary>
        /// <param name="dictionary">The annotation's dictionary.</param>
        /// <param name="crossTable">The document cross table.</param>
        /// <param name="bounds">The annotation's bounds.</param>
        internal PdfAnnotation(PdfDictionary dictionary, PdfCrossTable crossTable, RectangleF bounds)
            : base()
        {
            this.Initialize();
            this.Bounds = bounds;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Sets related page of the annotation.
        /// </summary>
        /// <param name="page">The page.</param>
        internal void SetPage(PdfPageBase page)
        {
            this.m_page = page as PdfPage;

            if (this.m_page != null)
            {
                this.m_dictionary.SetProperty(DictionaryProperties.P, new PdfReferenceHolder(this.m_page));
            }
        }

        /// <summary>
        /// Sets the location.
        /// </summary>
        /// <param name="location">The location.</param>
        internal void SetLocation(PointF location)
        {
            this.m_rectangle.Location = location;
        }

        /// <summary>
        /// Sets the proper name to the field.
        /// </summary>
        /// <param name="text">The annotation's text.</param>
        internal virtual void ApplyText(string text)
        {
            this.m_text = text;

            this.Dictionary.SetProperty(DictionaryProperties.Contents, new PdfString(text));
        }

        /// <summary>
        /// Sets the size.
        /// </summary>
        /// <param name="size">The size.</param>
        internal void SetSize(SizeF size)
        {
            this.m_rectangle.Size = size;
        }

        /// <summary>
        /// Initializes annotation object.
        /// </summary>
        protected virtual void Initialize()
        {
            this.m_dictionary.BeginSave += new SavePdfPrimitiveEventHandler(this.Dictionary_BeginSave);
            this.m_dictionary.SetProperty(DictionaryProperties.Type, new PdfName(DictionaryProperties.Annot));
        }

        /// <summary>
        /// Handles the BeginSave event of the Dictionary.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="ars">The <see cref="Syncfusion.Pdf.Primitives.SavePdfPrimitiveEventArgs"/> instance containing the event data.</param>
        private void Dictionary_BeginSave(object sender, SavePdfPrimitiveEventArgs ars)
        {
            this.Save();
        }

        /// <summary>
        /// Saves an annotation.
        /// </summary>
        protected virtual void Save()
        {
#if !SILVERLIGHT && !NETFX_CORE && !WP
            if ((this.GetType().ToString().Contains("Pdf3DAnnotation")
                || this.GetType().ToString().Contains("PdfAttachmentAnnotation")
                || this.GetType().ToString().Contains("PdfSoundAnnotation")
                || this.GetType().ToString().Contains("PdfActionAnnotation")) && (PdfDocument.ConformanceLevel == PdfConformanceLevel.Pdf_A1B || PdfDocument.ConformanceLevel == PdfConformanceLevel.Pdf_X1A2001))
            {
                throw new PdfConformanceException("The specified annotation type is not supported by PDF/A1-B standard document.");
            }
#endif

            if (this.m_border != null)
            {
                this.m_dictionary.SetProperty(DictionaryProperties.Border, this.m_border);
            }

            RectangleF nativeRectangle = new RectangleF(this.m_rectangle.X, this.m_rectangle.Bottom, this.m_rectangle.Width, this.m_rectangle.Height);

            if (this.m_page != null)
            {
                PdfSection section = this.m_page.Section;
                nativeRectangle.Location = section.PointToNativePdf(this.Page, nativeRectangle.Location);
            }

            this.m_dictionary.SetProperty(DictionaryProperties.Rect, PdfArray.FromRectangle(nativeRectangle));
        }

        #endregion

        #region IPdfWrapper Members
        /// <summary>
        /// Gets the element.
        /// </summary>
        /// <value></value>
        IPdfPrimitive IPdfWrapper.Element
        {
            get
            {
                return this.m_dictionary;
            }
        }
        #endregion
    }
}
