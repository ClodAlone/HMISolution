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
    /// Represents the Uri annotation
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = document.Pages.Add();
    /// //Create a new rectangle
    /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
    /// //Create a new Uri Annotation.
    /// PdfUriAnnotation uriAnnotation = new PdfUriAnnotation(rectangle, "http://www.google.com");
    /// //Set Text to uriAnnotation.
    /// uriAnnotation.Text = "Uri Annotation";
    /// //Add this annotation to a new page.
    /// page.Annotations.Add(uriAnnotation);
    /// //Save the document to disk.
    /// document.Save("UriAnnotation.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create a new rectangle
    /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
    /// 'Create a new uri Annotation.
    /// Dim uriAnnotation As PdfUriAnnotation = New PdfUriAnnotation(rectangle, "http://www.google.com")
    /// 'Set the Text to uriAnnotation.
    /// uriAnnotation.Text = "Uri Annotation"
    /// 'Add this annotation to a new page.
    /// page.Annotations.Add(uriAnnotation)
    /// 'Save the document to disk.
    /// document.Save("UriAnnotation.pdf")
    /// </code>
    /// </example> 
    public class PdfUriAnnotation : PdfActionLinkAnnotation
    {
        #region Fields
        /// <summary>
        /// Internal variable to store acton for the annotation.
        /// </summary>
        private PdfUriAction m_uriAction = new PdfUriAction();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the Uri address.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new Uri Annotation.
        /// PdfUriAnnotation uriAnnotation = new PdfUriAnnotation(rectangle);
        /// //Set Text to uriAnnotation.
        /// uriAnnotation.Text = "Uri Annotation";
        /// // Set Uri to uriAnnotation
        /// uriAnnotation.Uri="http://www.google.com";
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(uriAnnotation);
        /// //Save the document to disk.
        /// document.Save("UriAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new uri Annotation.
        /// Dim uriAnnotation As PdfUriAnnotation = New PdfUriAnnotation(rectangle)
        /// 'Set the Text to uriAnnotation.
        /// uriAnnotation.Text = "Uri Annotation"
        /// 'Set Uri to uriAnnotation;
        /// uriAnnotation.Uri="http://www.google.com"
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(uriAnnotation)
        /// 'Save the document to disk.
        /// document.Save("UriAnnotation.pdf")
        /// </code>
        /// </example> 
        public string Uri
        {
            get
            {
                return this.m_uriAction.Uri;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("uri");
                }

                if (this.m_uriAction.Uri != value)
                {
                    this.m_uriAction.Uri = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>The <see cref="PdfAction"/> object specifies the action of the annotation.</value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new Uri Annotation.
        /// PdfUriAnnotation uriAnnotation = new PdfUriAnnotation(rectangle);
        /// //Set Text to uriAnnotation.
        /// uriAnnotation.Text = "Uri Annotation";
        /// // Set Uri to uriAnnotation
        /// uriAnnotation.Uri="http://www.google.com";
        ///  //Creates a new Sound action
        /// PdfSoundAction soundAction = new PdfSoundAction(@"..\..\..\..\..\..\..\..\..\Common\Data\PDF\startup.wav");
        /// soundAction.Sound.Bits = 16;
        /// soundAction.Sound.Channels = PdfSoundChannels.Stereo;
        /// soundAction.Sound.Encoding = PdfSoundEncoding.Signed;
        /// soundAction.Volume = 0.9f;
        /// uriAnnotation.Action=soundAction;
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(uriAnnotation);
        /// //Save the document to disk.
        /// document.Save("UriAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new uri Annotation.
        /// Dim uriAnnotation As PdfUriAnnotation = New PdfUriAnnotation(rectangle)
        /// 'Set the Text to uriAnnotation.
        /// uriAnnotation.Text = "Uri Annotation"
        /// 'Set Uri to uriAnnotation;
        /// uriAnnotation.Uri="http://www.google.com"
        /// 'Creates a new sound annotation
        /// Dim soundAction As PdfSoundAction = New PdfSoundAction("..\..\..\..\..\..\..\..\..\Common\Data\PDF\startup.wav")
        /// soundAction.Sound.Bits = 16
        /// soundAction.Sound.Channels = PdfSoundChannels.Stereo
        /// soundAction.Sound.Encoding = PdfSoundEncoding.Signed
        /// soundAction.Volume = 0.9F
        /// soundAction.Mix = True
        /// uriAnnotation.Action=soundAction
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(uriAnnotation)
        /// 'Save the document to disk.
        /// document.Save("UriAnnotation.pdf")
        /// </code>
        /// </example> 
        public override PdfAction Action
        {
            get
            {
                return base.Action;
            }

            set
            {
                base.Action = value;
                this.m_uriAction.Next = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfUriAnnotation"/> class.
        /// </summary>        
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new Uri Annotation.
        /// PdfUriAnnotation uriAnnotation = new PdfUriAnnotation(rectangle);
        /// //Set Text to uriAnnotation.
        /// uriAnnotation.Text = "Uri Annotation";
        /// // Set Uri to uriAnnotation
        /// uriAnnotation.Uri="http://www.google.com"
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(uriAnnotation);
        /// //Save the document to disk.
        /// document.Save("UriAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new uri Annotation.
        /// Dim uriAnnotation As PdfUriAnnotation = New PdfUriAnnotation(rectangle)
        /// 'Set the Text to uriAnnotation.
        /// uriAnnotation.Text = "Uri Annotation"
        /// 'Set Uri to uriAnnotation;
        /// uriAnnotation.Uri="http://www.google.com"
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(uriAnnotation)
        /// 'Save the document to disk.
        /// document.Save("UriAnnotation.pdf")
        /// </code>
        /// </example> 
        /// <param name="rectangle">RectangleF structure that specifies the bounds of the annotation.</param>
        public PdfUriAnnotation(RectangleF rectangle)
            : base(rectangle)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfUriAnnotation"/> class.
        /// </summary>
        /// <param name="rectangle">RectangleF structure that specifies the bounds of the annotation.</param>
        /// <param name="uri">unique resource identifier path.</param>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new Uri Annotation.
        /// PdfUriAnnotation uriAnnotation = new PdfUriAnnotation(rectangle, "http://www.google.com");
        /// //Set Text to uriAnnotation.
        /// uriAnnotation.Text = "Uri Annotation";
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(uriAnnotation);
        /// //Save the document to disk.
        /// document.Save("UriAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new uri Annotation.
        /// Dim uriAnnotation As PdfUriAnnotation = New PdfUriAnnotation(rectangle, "http://www.google.com")
        /// 'Set the Text to uriAnnotation.
        /// uriAnnotation.Text = "Uri Annotation"
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(uriAnnotation)
        /// 'Save the document to disk.
        /// document.Save("UriAnnotation.pdf")
        /// </code>
        /// </example> 
        public PdfUriAnnotation(RectangleF rectangle, string uri)
            : base(rectangle)
        {
            if (uri == null)
            {
                throw new ArgumentNullException("uri");
            }

            this.Uri = uri;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Initializes annotation object.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            Dictionary.SetProperty(DictionaryProperties.Subtype, new PdfName(DictionaryProperties.Link));
            Dictionary.SetProperty(DictionaryProperties.A, this.m_uriAction);
        }
        #endregion
    }
}
