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
    /// Represents the Rubber Stamp annotation for a PDF document.
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document.
    /// PdfPage page = document.Pages.Add();    
    /// //Create a new pdf rubber stamp annotation.
    /// RectangleF rectangle = new RectangleF(40, 60, 80, 20);
    /// PdfRubberStampAnnotation rubberstampAnnotation = new PdfRubberStampAnnotation(rectangle, " Text Rubber Stamp Annotation");
    /// rubberstampAnnotation.Icon = PdfRubberStampAnnotationIcon.Draft;
    /// rubberstampAnnotation.Text = "Text Properties Rubber Stamp Annotation";
    /// page.Annotations.Add(rubberstampAnnotation);
    /// //Save the document to disk.
    /// document.Save("Rubberstamp.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document.
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create a new pdf rubber stamp annotation.
    /// Dim Rubberstampannotation As RectangleF  = New RectangleF(40, 60, 80, 20)
    /// Dom rubberstampAnnotation As PdfRubberStampAnnotation  = New PdfRubberStampAnnotation(Rubberstampannotation, " Text Rubber Stamp Annotation")
    /// rubberstampAnnotation.Icon = PdfRubberStampAnnotationIcon.Draft
    /// rubberstampAnnotation.Text = "Text Properties Rubber Stamp Annotation"
    /// 'Add this annotation to a new page.
    /// page.Annotations.Add(popupAnnotation)
    /// 'Save the document to disk.
    /// document.Save("Rubberstamp.pdf")
    /// </code>
    /// </example> 
    public class PdfRubberStampAnnotation : PdfAnnotation
    {
        #region Fields

        /// <summary>
        /// internal varable for the rubberstamp annotation icon
        /// </summary>
        private PdfRubberStampAnnotationIcon m_rubberStampAnnotaionIcon = PdfRubberStampAnnotationIcon.Draft;

        /// <summary>
        /// Annotation's appearance.
        /// </summary>
        private PdfAppearance m_appearance = null;

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the annotation's icon. 
        /// </summary>        
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new pdf rubber stamp annotation.
        /// RectangleF Rubberstampannotation = new RectangleF(40, 60, 80, 20);
        /// PdfRubberStampAnnotation rubberstampAnnotation = new PdfRubberStampAnnotation(Rubberstampannotation, " Text Rubber Stamp Annotation");
        /// //Sets the rubber stamp icon.
        /// rubberstampAnnotation.Icon = PdfRubberStampAnnotationIcon.Draft;
        /// page.Annotations.Add(rubberstampAnnotation);
        /// //Save the document to disk.
        /// document.Save("Rubberstamp.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new pdf rubber stamp annotation.
        /// Dim Rubberstampannotation As RectangleF  = New RectangleF(40, 60, 80, 20)
        /// Dom rubberstampAnnotation As PdfRubberStampAnnotation  = New PdfRubberStampAnnotation(Rubberstampannotation, " Text Rubber Stamp Annotation")
        /// Sets the rubber stamp icon
        /// rubberstampAnnotation.Icon = PdfRubberStampAnnotationIcon.Draft
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(popupAnnotation)
        /// 'Save the document to disk.
        /// document.Save("Rubberstamp.pdf")
        /// </code>
        /// </example> 
        /// <value>A <see cref="PdfRubberStampAnnotationIcon"/> enumeration member specifying the icon for the annotation when it is displayed in closed state. </value>
        public PdfRubberStampAnnotationIcon Icon
        {
            get
            {
                return this.m_rubberStampAnnotaionIcon;
            }

            set
            {
                this.m_rubberStampAnnotaionIcon = value;
                Dictionary.SetName(DictionaryProperties.Name, this.m_rubberStampAnnotaionIcon.ToString());
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
        /// //Create a new pdf rubber stamp annotation.
        /// RectangleF Rubberstampannotation = new RectangleF(40, 60, 80, 20);
        /// PdfRubberStampAnnotation rubberstampAnnotation = new PdfRubberStampAnnotation(Rubberstampannotation, " Text Rubber Stamp Annotation");
        /// //Sets the pdfappreance.
        /// rubberstampAnnotation.Appearance = new PdfAppearance(annotation);
        /// annotation.Appearance.Normal.Draw(page, New PointF(rubberstampAnnotation.Location.X, rubberstampAnnotation.Location.Y));
        /// page.Annotations.Add(rubberstampAnnotation);
        /// //Save the document to disk.
        /// document.Save("Rubberstamp.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new pdf rubber stamp annotation.
        /// Dim Rubberstampannotation As RectangleF  = New RectangleF(40, 60, 80, 20)
        /// Dom rubberstampAnnotation As PdfRubberStampAnnotation  = New PdfRubberStampAnnotation(Rubberstampannotation, " Text Rubber Stamp Annotation")
        /// 'Sets the pdfappreance.
        /// rubberstampAnnotation.Appearance = New PdfAppearance(annotation)
        /// rubberstampAnnotation.Appearance.Normal.Draw(page, New PointF(rubberstampAnnotation.Location.X, rubberstampAnnotation.Location.Y))
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(popupAnnotation)
        /// 'Save the document to disk.
        /// document.Save("Rubberstamp.pdf")
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
        /// Initializes a new instance of the <see cref="PdfRubberStampAnnotation"/> class.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new pdf rubber stamp annotation.
        /// RectangleF Rubberstampannotation = new RectangleF(40, 60, 80, 20);
        /// PdfRubberStampAnnotation rubberstampAnnotation = new PdfRubberStampAnnotation();
        /// rubberstampAnnotation.Icon = PdfRubberStampAnnotationIcon.Draft;
        /// rubberstampAnnotation.Text = "Text Properties Rubber Stamp Annotation";
        /// page.Annotations.Add(rubberstampAnnotation);
        /// //Save the document to disk.
        /// document.Save("Rubberstamp.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new pdf rubber stamp annotation.
        /// Dim Rubberstampannotation As RectangleF  = New RectangleF(40, 60, 80, 20)
        /// Dom rubberstampAnnotation As PdfRubberStampAnnotation  = New PdfRubberStampAnnotation()
        /// rubberstampAnnotation.Icon = PdfRubberStampAnnotationIcon.Draft
        /// rubberstampAnnotation.Text = "Text Properties Rubber Stamp Annotation"
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(popupAnnotation)
        /// 'Save the document to disk.
        /// document.Save("Rubberstamp.pdf")
        /// </code>
        /// </example> 
        public PdfRubberStampAnnotation()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfRubberStampAnnotation"/> class.
        /// <param name="rectangle">RectangleF structure that specifies the bounds of the annotation.</param>
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new pdf rubber stamp annotation.
        /// RectangleF Rubberstampannotation = new RectangleF(40, 60, 80, 20);
        /// PdfRubberStampAnnotation rubberstampAnnotation = new PdfRubberStampAnnotation(Rubberstampannotation);
        /// rubberstampAnnotation.Icon = PdfRubberStampAnnotationIcon.Draft;
        /// rubberstampAnnotation.Text = "Text Properties Rubber Stamp Annotation";
        /// page.Annotations.Add(rubberstampAnnotation);
        /// //Save the document to disk.
        /// document.Save("Rubberstamp.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new pdf rubber stamp annotation.
        /// Dim Rubberstampannotation As RectangleF  = New RectangleF(40, 60, 80, 20)
        /// Dom rubberstampAnnotation As PdfRubberStampAnnotation  = New PdfRubberStampAnnotation(Rubberstampannotation)
        /// rubberstampAnnotation.Icon = PdfRubberStampAnnotationIcon.Draft
        /// rubberstampAnnotation.Text = "Text Properties Rubber Stamp Annotation"
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(popupAnnotation)
        /// 'Save the document to disk.
        /// document.Save("Rubberstamp.pdf")
        /// </code>
        /// </example> 
        public PdfRubberStampAnnotation(RectangleF rectangle)
            : base(rectangle)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfRubberStampAnnotation"/> class.
        /// </summary>
        /// <param name="rectangle">RectangleF structure that specifies the bounds of the annotation.</param>
        /// <param name="text">Text of the rubber stamp annotation.</param>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new pdf rubber stamp annotation.
        /// RectangleF Rubberstampannotation = new RectangleF(40, 60, 80, 20);
        /// PdfRubberStampAnnotation rubberstampAnnotation = new PdfRubberStampAnnotation(Rubberstampannotation, " Text Rubber Stamp Annotation");
        /// rubberstampAnnotation.Icon = PdfRubberStampAnnotationIcon.Draft;
        /// rubberstampAnnotation.Text = "Text Properties Rubber Stamp Annotation";
        /// page.Annotations.Add(rubberstampAnnotation);
        /// //Save the document to disk.
        /// document.Save("Rubberstamp.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new pdf rubber stamp annotation.
        /// Dim Rubberstampannotation As RectangleF  = New RectangleF(40, 60, 80, 20)
        /// Dom rubberstampAnnotation As PdfRubberStampAnnotation  = New PdfRubberStampAnnotation(Rubberstampannotation, " Text Rubber Stamp Annotation")
        /// rubberstampAnnotation.Icon = PdfRubberStampAnnotationIcon.Draft
        /// rubberstampAnnotation.Text = "Text Properties Rubber Stamp Annotation"
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(popupAnnotation)
        /// 'Save the document to disk.
        /// document.Save("Rubberstamp.pdf")
        /// </code>
        /// </example> 
        public PdfRubberStampAnnotation(RectangleF rectangle, string text)
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
            Dictionary.SetProperty(DictionaryProperties.Subtype, new PdfName(DictionaryProperties.Stamp));
        }

        /// <summary>
        /// Saves an annotation.
        /// </summary>
        protected override void Save()
        {
            base.Save();

            if (this.m_appearance != null && this.m_appearance.Normal != null)
            {
                Dictionary.SetProperty(DictionaryProperties.AP, this.m_appearance);
            }
        }

        #endregion
    }
}
