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
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents a Base class for popup annotation which can be either in open or closed state.
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document.
    /// PdfPage page = document.Pages.Add();
    /// //Create a new rectangle
    /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
    /// //Create a new popup annotation.
    /// PdfPopupAnnotation popupAnnotation = new PdfPopupAnnotation(rectangle,"Test popup annotation");
    /// popupAnnotation.Border.Width = 4;
    /// popupAnnotation.Border.HorizontalRadius = 20;
    /// popupAnnotation.Border.VerticalRadius = 30;
    /// //Set the pdf popup icon.
    /// popupAnnotation.Icon = PdfPopupIcon.NewParagraph;
    /// //Add this annotation to a new page.
    /// page.Annotations.Add(popupAnnotation);
    /// //Save the document to disk.
    /// document.Save("PopupAnnotation.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document.
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create a new rectangle
    /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
    /// 'Create a new popup annotation.
    /// Dim popupAnnotation As PdfPopupAnnotation = New PdfPopupAnnotation(rectangle, "Test popup annotation")
    /// popupAnnotation.Border.Width = 4
    /// popupAnnotation.Border.HorizontalRadius = 20
    /// popupAnnotation.Border.VerticalRadius = 30
    /// 'Set the pdf popup icon.
    /// popupAnnotation.Icon = PdfPopupIcon.NewParagraph
    /// 'Add this annotation to a new page.
    /// page.Annotations.Add(popupAnnotation)
    /// 'Save the document to disk.
    /// document.Save("PopupAnnotation.pdf")
    /// </code>
    /// </example> 
    public class PdfPopupAnnotation : PdfAnnotation
    {
        #region Fields
        /// <summary>
        /// Indicates whether annotation is open or not.
        /// </summary>
        private bool m_open = false;

        /// <summary>
        /// Type of the icon of the annotation.
        /// </summary>
        private PdfPopupIcon m_icon = PdfPopupIcon.Note;

        /// <summary>
        /// Annotation's appearance.
        /// </summary>
        private PdfAppearance m_appearance = null;

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets icon style.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new popup annotation.
        /// PdfPopupAnnotation popupAnnotation = new PdfPopupAnnotation(rectangle,"Test popup annotation");
        /// //Set the pdf popup icon.
        /// popupAnnotation.Icon = PdfPopupIcon.NewParagraph;
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(popupAnnotation);
        /// //Save the document to disk.
        /// document.Save("PopupIcon.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new popup annotation.
        /// Dim popupAnnotation As PdfPopupAnnotation = New PdfPopupAnnotation(rectangle, "Test popup annotation")
        /// 'Set the pdf popup icon.
        /// popupAnnotation.Icon = PdfPopupIcon.NewParagraph
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(popupAnnotation)
        /// 'Save the document to disk.
        /// document.Save("PopupIcon.pdf")
        /// </code>
        /// </example> 
        public PdfPopupIcon Icon
        {
            get
            {
                return this.m_icon;
            }

            set
            {
                if (this.m_icon != value)
                {
                    this.m_icon = value;
                    Dictionary.SetName(DictionaryProperties.Name, this.m_icon.ToString());
                }
            }
        }

        /// <summary>
        /// Gets or sets value whether annotation is initially open or closed
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new popup annotation.
        /// PdfPopupAnnotation popupAnnotation = new PdfPopupAnnotation(rectangle,"Test popup annotation");
        /// //Set the Open to popupAnnotation.
        /// popupAnnotation.Open = true;
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(popupAnnotation);
        /// //Save the document to disk.
        /// document.Save("PopupOpen.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new popup annotation.
        /// Dim popupAnnotation As PdfPopupAnnotation = New PdfPopupAnnotation(rectangle, "Test popup annotation")
        /// 'Set the Open to popupAnnotation.
        /// popupAnnotation.Open = True
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(popupAnnotation)
        /// 'Save the document to disk.
        /// document.Save("PopupOpen.pdf")
        /// </code>
        /// </example> 
        public bool Open
        {
            get
            {
                return this.m_open;
            }
            set
            {
                if (this.m_open != value)
                {
                    this.m_open = value;
                    Dictionary.SetBoolean(DictionaryProperties.Open, this.m_open);
                }
            }
        }

        /// <summary>
        /// Gets or sets appearance of the annotation.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new popup annotation.
        /// PdfPopupAnnotation popupAnnotation = new PdfPopupAnnotation(rectangle,"Test popup annotation");
        /// //Gets the appreance of popup annotation.
        /// PdfAppearance appreance=popupAnnotation.PdfAppearance;
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(popupAnnotation);
        /// //Save the document to disk.
        /// document.Save("PopupAppearance.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new popup annotation.
        /// Dim popupAnnotation As PdfPopupAnnotation = New PdfPopupAnnotation(rectangle, "Test popup annotation")
        /// 'Gets the appreance of popup annotation
        /// Dim appreance As PdfAppearance  = popupAnnotation.PdfAppearance
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(popupAnnotation)
        /// 'Save the document to disk.
        /// document.Save("PopupAppearance.pdf")
        /// </code>
        /// </example> 
        public PdfAppearance Appearance
        {
            get
            {
                if (this.m_appearance == null)
                {
                    this.m_appearance = new PdfAppearance(this);
                }
                return this.m_appearance;
            }

            set
            {
                if (this.m_appearance != value)
                {
                    this.m_appearance = value;
                }
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfPopupAnnotation"/> class.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new popup annotation.
        /// PdfPopupAnnotation popupAnnotation = new PdfPopupAnnotation();
        /// popupAnnotation.Text="Test popup annotation";
        /// popupAnnotation.Bounds=rectangle;
        /// popupAnnotation.Border.Width = 4;
        /// popupAnnotation.Border.HorizontalRadius = 20;
        /// popupAnnotation.Border.VerticalRadius = 30;
        /// //Set the pdf popup icon.
        /// popupAnnotation.Icon = PdfPopupIcon.NewParagraph;
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(popupAnnotation);
        /// //Save the document to disk.
        /// document.Save("PopupAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new popup annotation.
        /// Dim popupAnnotation As PdfPopupAnnotation = New PdfPopupAnnotation()
        /// popupAnnotation.Text="Test popup annotation"
        /// popupAnnotation.Bounds=rectangle
        /// popupAnnotation.Border.Width = 4
        /// popupAnnotation.Border.HorizontalRadius = 20
        /// popupAnnotation.Border.VerticalRadius = 30
        /// 'Set the pdf popup icon.
        /// popupAnnotation.Icon = PdfPopupIcon.NewParagraph
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(popupAnnotation)
        /// 'Save the document to disk.
        /// document.Save("PopupAnnotation.pdf")
        /// </code>
        /// </example> 
        public PdfPopupAnnotation()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfPopupAnnotation"/> class.
        /// </summary>
        /// <param name="rectangle">RectangleF structure that specifies the bounds of the annotation.</param>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new popup annotation.
        /// PdfPopupAnnotation popupAnnotation = new PdfPopupAnnotation(rectangle);
        /// popupAnnotation.Text="Test popup annotation";
        /// popupAnnotation.Border.Width = 4;
        /// popupAnnotation.Border.HorizontalRadius = 20;
        /// popupAnnotation.Border.VerticalRadius = 30;
        /// //Set the pdf popup icon.
        /// popupAnnotation.Icon = PdfPopupIcon.NewParagraph;
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(popupAnnotation);
        /// //Save the document to disk.
        /// document.Save("PopupAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new popup annotation.
        /// Dim popupAnnotation As PdfPopupAnnotation = New PdfPopupAnnotation(rectangle()
        /// popupAnnotation.Text="Test popup annotation"
        /// popupAnnotation.Border.Width = 4
        /// popupAnnotation.Border.HorizontalRadius = 20
        /// popupAnnotation.Border.VerticalRadius = 30
        /// 'Set the pdf popup icon.
        /// popupAnnotation.Icon = PdfPopupIcon.NewParagraph
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(popupAnnotation)
        /// 'Save the document to disk.
        /// document.Save("PopupAnnotation.pdf")
        /// </code>
        /// </example> 
        public PdfPopupAnnotation(RectangleF rectangle)
            : base(rectangle)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfPopupAnnotation"/> class.
        /// </summary>
        /// <param name="rectangle">RectangleF structure that specifies the bounds of the annotation.</param>
        /// <param name="text">The string specifies the annotation text.</param>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new popup annotation.
        /// PdfPopupAnnotation popupAnnotation = new PdfPopupAnnotation(rectangle,"Test popup annotation");
        /// popupAnnotation.Border.Width = 4;
        /// popupAnnotation.Border.HorizontalRadius = 20;
        /// popupAnnotation.Border.VerticalRadius = 30;
        /// //Set the pdf popup icon.
        /// popupAnnotation.Icon = PdfPopupIcon.NewParagraph;
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(popupAnnotation);
        /// //Save the document to disk.
        /// document.Save("PopupAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New Rect angleF(10, 40, 30, 30)
        /// 'Create a new popup annotation.
        /// Dim popupAnnotation As PdfPopupAnnotation = New PdfPopupAnnotation(rectangle, "Test popup annotation")
        /// popupAnnotation.Border.Width = 4
        /// popupAnnotation.Border.HorizontalRadius = 20
        /// popupAnnotation.Border.VerticalRadius = 30
        /// 'Set the pdf popup icon.
        /// popupAnnotation.Icon = PdfPopupIcon.NewParagraph
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(popupAnnotation)
        /// 'Save the document to disk.
        /// document.Save("PopupAnnotation.pdf")
        /// </code>
        /// </example> 
        public PdfPopupAnnotation(RectangleF rectangle, string text)
            : base(rectangle)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            Text = text;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Initializes annotation object.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            Dictionary.SetProperty(DictionaryProperties.Subtype, new PdfName(DictionaryProperties.Text));
        }

        /// <summary>
        /// Saves an annotation.
        /// </summary>
        protected override void Save()
        {
            base.Save();

            if (m_appearance != null && m_appearance.Normal != null)
            {
                Dictionary.SetProperty(DictionaryProperties.AP, m_appearance);
            }
        }
        #endregion
    }
}
