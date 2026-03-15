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
    /// Represents the 3D annotation for a PDF document.
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document.
    /// PdfPage page = document.Pages.Add();
    /// //Create a new pdf3d annotation.
    /// Pdf3DAnnotation pdf3dAnnotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
    /// //Create pdfFont, pdfFont style and brush
    /// Font font = new Font("Calibri", 11, FontStyle.Regular);
    /// PdfFont pdfFont = new PdfTrueTypeFont(font, false);
    /// PdfBrush brush = new PdfSolidBrush(Color.DarkBlue);
    /// PdfBrush bgbrush = new PdfSolidBrush(Color.WhiteSmoke);
    /// PdfColor color = new PdfColor(Color.Silver);
    /// Pdf3DActivation activation = new Pdf3DActivation();
    /// activation.ActivationMode = Pdf3DActivationMode.PageVisible;
    /// activation.ShowToolbar = true;
    /// pdf3dAnnotation.Activation = activation;
    /// Pdf3DView defaultView = new Pdf3DView();
    /// defaultView.ExternalName="Near View";
    /// defaultView.CameraToWorldMatrix=new float[] { -0.382684f, 0.92388f, -0.0000000766026f, 0.18024f, 0.0746579f, 0.980785f, 0.906127f, 0.37533f, -0.19509f, -100, -112.432f, 45.6829f };
    /// defaultView.CenterOfOrbit=131.695f;
    /// view.Background = background;
    /// view.Projection = projection;
    /// view.RenderMode = renderMode;
    /// view.LightingScheme = lighting;
    /// pdf3dAnnotation.Views.Add(defaultView);
    /// pdf3dAnnotation.Appearance.Normal.Graphics.DrawString("Click to activate", pdfFont, brush, new PointF(40, 40));
    /// //Add this annotation to a new page
    /// pdf3dAnnotation.Appearance.Normal.Draw(page, new PointF(pdf3dAnnotation.Location.X, pdf3dAnnotation.Location.Y));
    /// //Save the  document to disk.
    /// document.Save("3DAnnotation.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document.
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create a new pdf3d annotation.
    /// Dim pdf3dAnnotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
    /// 'Create pdfFont, pdfFont style and brush
    /// Dim font As Font = New Font("Calibri", 11, FontStyle.Regular)
    /// Dim pdfFont As PdfFont = New PdfTrueTypeFont(font, False)
    /// Dim brush As PdfBrush = New PdfSolidBrush(System.Drawing.Color.DarkBlue)
    /// Dim bgbrush As PdfBrush = New PdfSolidBrush(System.Drawing.Color.WhiteSmoke)
    /// Dim color As PdfColor = New PdfColor(System.Drawing.Color.Silver)
    /// 'Create a new pdf3d projection.
    /// Dim projection As Pdf3DProjection  = New Pdf3DProjection()
    /// projection.ProjectionType = Pdf3DProjectionType.Perspective
    /// 'Create a new pdf3d activation.
    /// Pdf3DActivation activation = new Pdf3DActivation()
    /// activation.ActivationMode = Pdf3DActivationMode.PageVisible
    /// activation.ShowToolbar = true
    /// pdf3dAnnotation.Activation = activation
    /// 'Create a new pdf3d view.
    /// Dim defaultView As Pdf3DView  = New Pdf3DView()
    /// defaultView.ExternalName="Near View"
    /// defaultView.CameraToWorldMatrix=new Single() { -0.382684f, 0.92388f, -0.0000000766026f, 0.18024f, 0.0746579f, 0.980785f, 0.906127f, 0.37533f, -0.19509f, -100, -112.432f, 45.6829f }
    /// defaultView.CenterOfOrbit=131.695f
    /// view.Background = background
    /// view.Projection = projection
    /// view.RenderMode = renderMode
    /// view.LightingScheme = lighting
    /// pdf3dAnnotation.Views.Add(defaultView)
    /// 'Add this annotation to a new page.
    /// pdf3dAnnotation.Appearance.Normal.Draw(page, New PointF(pdf3dAnnotation.Location.X, pdf3dAnnotation.Location.Y))
    /// 'Save the  document to disk.
    /// document.Save("3DAnnotation.pdf")
    /// </code>
    /// </example> 
    public class Pdf3DAnnotation : PdfFileAnnotation
    {
        #region Fields
        private Pdf3DActivation m_activation;

        /// <summary>
        /// Internal variable to store U3D.
        /// </summary>
        private Pdf3DBase m_u3d = null;

        /// <summary>
        /// Internal variable to store apperance.
        /// </summary>
        private PdfTemplate m_apperance = null;

        #endregion

        #region Properties
        /// <summary>
        /// Gets the list of available views for the current 3D artwork.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new pdf3d annotation.
        /// Pdf3DAnnotation pdf3dAnnotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// Pdf3DView defaultView = new Pdf3DView();
        /// defaultView.ExternalName="Near View";
        /// defaultView.CameraToWorldMatrix=new float[] { -0.382684f, 0.92388f, -0.0000000766026f, 0.18024f, 0.0746579f, 0.980785f, 0.906127f, 0.37533f, -0.19509f, -100, -112.432f, 45.6829f };
        /// defaultView.CenterOfOrbit=131.695f;
        /// pdf3dAnnotation.Views.Add(defaultView);
        /// page.Annoatations.Add(pdf3dAnnotation);
        /// //Save the  document to disk.
        /// document.Save("3DViews.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new pdf3d annotation..
        /// Dim pdf3dAnnotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Create a new pdf3d view.
        /// Dim defaultView As Pdf3DView  = New Pdf3DView()
        /// defaultView.ExternalName="Near View"
        /// defaultView.CameraToWorldMatrix=new Single() { -0.382684f, 0.92388f, -0.0000000766026f, 0.18024f, 0.0746579f, 0.980785f, 0.906127f, 0.37533f, -0.19509f, -100, -112.432f, 45.6829f }
        /// defaultView.CenterOfOrbit=131.695f
        /// pdf3dAnnotation.Views.Add(defaultView)
        /// 'Draw a pdf3dAnnotation into the new page.
        /// page.Annoatations.Add(pdf3dAnnotation)
        /// 'Save the  document to disk.
        /// document.Save("3DViews.pdf")
        /// </code>
        /// </example> 
        public Pdf3DViewCollection Views
        {
            get
            {
                return this.m_u3d.Stream.Views;
            }
        }

        /// <summary>
        /// Gets or sets the default view.
        /// </summary>
        /// <value>The default view.</value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new pdf3d annotation.
        /// Pdf3DAnnotation pdf3dAnnotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// pdf3dAnnotation.DefaultView = 0;
        /// page.Annoatations.Add(pdf3dAnnotation);
        /// //Save the  document to disk.
        /// document.Save("3DDefaultView.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new pdf3d annotation..
        /// Dim pdf3dAnnotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// pdf3dAnnotation.DefaultView = 0
        /// 'Draw a pdf3dAnnotation into the new page.
        /// page.Annoatations.Add(pdf3dAnnotation)
        /// 'Save the  document to disk.
        /// document.Save("3DDefaultView.pdf")
        /// </code>
        /// </example> 
        public int DefaultView
        {
            get
            {
                return this.m_u3d.Stream.DefaultView;
            }

            set
            {
                this.m_u3d.Stream.DefaultView = value;
            }
        }

        /// <summary>
        /// Gets or sets the code to execute when the 3D artwork is instantiated. 
        /// <value>Javascript code to be executed when the 3D artwork is instantiated.</value>
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new pdf3d annotation.
        /// Pdf3DAnnotation pdf3dAnnotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// //Create a bew pdf3d view.
        /// Pdf3DView defaultView = new Pdf3DView();
        /// defaultView.OnInstantiate="Near View";
        /// defaultView.CameraToWorldMatrix=new float[] { -0.382684f, 0.92388f, -0.0000000766026f, 0.18024f, 0.0746579f, 0.980785f, 0.906127f, 0.37533f, -0.19509f, -100, -112.432f, 45.6829f };
        /// defaultView.CenterOfOrbit=131.695f;
        /// pdf3dAnnotation.Views.Add(defaultView);
        /// //Adds the annotation.
        /// page.Annoatations.Add(pdf3dAnnotation);
        /// //Save the  document to disk.
        /// document.Save("3DViews.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new pdf3d annotation..
        /// Dim pdf3dAnnotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Adds a pdf3d view.
        /// Dim defaultView As Pdf3DView  = New Pdf3DView()
        /// defaultView.OnInstantiate="Near View"
        /// defaultView.CameraToWorldMatrix=new Single() { -0.382684f, 0.92388f, -0.0000000766026f, 0.18024f, 0.0746579f, 0.980785f, 0.906127f, 0.37533f, -0.19509f, -100, -112.432f, 45.6829f }
        /// defaultView.CenterOfOrbit=131.695f
        /// pdf3dAnnotation.Views.Add(defaultView)
        /// 'Draw a pdf3dAnnotation into the new page.
        /// page.Annoatations.Add(pdf3dAnnotation)
        /// 'Save the  document to disk.
        /// document.Save("3DViews.pdf")
        /// </code>
        /// </example> 
        public string OnInstantiate
        {
            get
            {
                return this.m_u3d.Stream.OnInstantiate;
            }

            set
            {
                this.m_u3d.Stream.OnInstantiate = value;
            }
        }

        /// <summary>
        /// Gets or sets the activation options for the annotation. 
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new pdf3d annotation.
        /// Pdf3DAnnotation pdf3dAnnotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// //Creatas a new pdf3d actviation.
        /// Pdf3DActivation activation = new Pdf3DActivation();
        /// activation.ActivationMode = Pdf3DActivationMode.ExplicitActivation;
        /// activation.ShowToolbar = false;
        /// pdf3dAnnotation.Activation = activation;
        /// //Adds the annotation.
        /// page.Annoatations.Add(pdf3dAnnotation);
        /// //Save the  document to disk.
        /// document.Save("3DActivation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new pdf3d annotation.
        /// Dim pdf3dAnnotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Create a new pdf3d activation.
        /// Dim activation As Pdf3DActivation = New Pdf3DActivation()
        /// activation.ActivationMode = Pdf3DActivationMode.PageVisible
        /// activation.ShowToolbar = True
        /// pdf3dAnnotation.Activation = activation
        /// 'Draw a pdf3dAnnotation into the new page.
        /// page.Annoatations.Add(pdf3dAnnotation)
        /// 'Save the  document to disk.
        /// document.Save("3DActivation.pdf")
        /// </code>
        /// </example> 
        /// <remarks>Defines the times at which the annotation should be activated and deactivated and the state of the 3D artwork instance at those times.</remarks>
        public Pdf3DActivation Activation
        {
            get
            {
                return this.m_activation;
            }

            set
            {
                this.m_activation = value;
            }
        }

#if SILVERLIGHT
        /// <summary>
        /// Security Critical : Gets or sets file name of the annotation.
        /// </summary>
        //[System.Security.SecurityCritical]
#else
        /// <summary>
        /// Gets or sets file name of the annotation.
        /// </summary>
#endif
        /// <value>Filename with Full path</value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new pdf3d annotation.
        /// Pdf3DAnnotation pdf3dAnnotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// //Gets the filename.
        /// string fileName=pdf3dAnnotation.FileName;
        /// //Adds the annotation.
        /// page.Annoatations.Add(pdf3dAnnotation);
        /// //Save the  document to disk.
        /// document.Save("3DAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new pdf3d annotation..
        /// Dim pdf3dAnnotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Gets a filename.
        /// Dim fileName As string=pdf3dAnnotation.FileName
        /// 'Draw a pdf3dAnnotation into the new page.
        /// page.Annoatations.Add(pdf3dAnnotation)
        /// 'Save the  document to disk.
        /// document.Save("3DAnnotation.pdf")
        /// </code>
        /// </example> 
        public override string FileName
        {
            get
            {
                return this.m_u3d.FileName;
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

                if (m_u3d.FileName != value)
                {
                    this.m_u3d.FileName = value;
                }
            }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfSoundAnnotation"/> class.
        /// </summary>
        /// <param name="rectangle">Bounds of the annotation.</param>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new pdf3d annotation.
        /// Pdf3DAnnotation pdf3dAnnotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150));
        /// //Adds the annotaiton.
        /// page.Annoatations.Add(pdf3dAnnotation);
        /// //Save the  document to disk.
        /// document.Save("3DAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new pdf3d annotation..
        /// Dim pdf3dAnnotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150))
        /// 'Draw a pdf3dAnnotation into the new page.
        /// page.Annoatations.Add(pdf3dAnnotation)
        /// //Save the  document to disk.
        /// document.Save("3DAnnotation.pdf")
        /// </code>
        /// </example> 
        public Pdf3DAnnotation(RectangleF rectangle)
            : base(rectangle)
        {
        }

#if SILVERLIGHT
        /// <summary>
        /// Security Critical : Initializes a new instance of the <see cref="PdfSoundAnnotation"/> class.
        /// </summary>
        [System.Security.SecurityCritical]
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfSoundAnnotation"/> class.
        /// </summary>
#endif
        /// <param name="rectangle">Bounds of the annotation.</param>
        /// <param name="fileName">Name of the sound file.</param>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new pdf3d annotation.
        /// Pdf3DAnnotation pdf3dAnnotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// //Adds the annotation on a page.
        /// page.Annoatations.Add(pdf3dAnnotation);
        /// //Save the  document to disk.
        /// document.Save("3DAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new pdf3d annotation..
        /// Dim pdf3dAnnotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Draws a pdf3dAnnotation into the new page.
        /// page.Annoatations.Add(pdf3dAnnotation)
        /// 'Save the  document to disk.
        /// document.Save("3DAnnotation.pdf")
        /// </code>
        /// </example> 
        public Pdf3DAnnotation(RectangleF rectangle, string fileName)
            : base(rectangle)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            this.m_u3d = new Pdf3DBase(fileName);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Initializes instance.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            Dictionary.SetProperty(DictionaryProperties.Subtype, new PdfName(DictionaryProperties._3D));
        }

        /// <summary>
        /// Saves instance.
        /// </summary>
        protected override void Save()
        {
            base.Save();
            Dictionary.SetProperty(DictionaryProperties._3DD, new PdfReferenceHolder(this.m_u3d));

            if (this.m_activation != null)
            {
                Dictionary[DictionaryProperties._3DA] = new PdfReferenceHolder(this.m_activation);
            }

            if (this.m_apperance != null)
            {
                Dictionary[DictionaryProperties.APN] = new PdfReferenceHolder(this.m_apperance);
            }
        }
        #endregion
    }
}