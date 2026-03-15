#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.Generic;
using System.Text;

using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    ///  Represents the mapping of 3D camera co-ordinates onto the target coordinate system of the annotation.
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document.
    /// PdfPage page = document.Pages.Add();
    /// //Create a new rectangle
    /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
    /// //Create a new Pdf3DAnnotation.
    /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
    /// //Create a new Pdf3DProjection.
    /// Pdf3DProjection projection = new Pdf3DProjection();
    /// projection.ProjectionType = Pdf3DProjectionType.Perspective;
    /// projection.FieldOfView = 10;
    /// projection.ClipStyle = Pdf3DProjectionClipStyle.ExplicitNearFar;
    /// projection.NearClipDistance = 10;
    /// //Create a new Pdf3DView
    /// Pdf3DView view = new Pdf3DView();
    /// view.Projection = projection;
    /// annotation.Views.Add(view);
    /// //Adds the annotation in a new page.
    /// page.Annoatations.Add(annotation);
    /// //Save the  document to disk.
    /// document.Save("Pdf3DProjection.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document.
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create a new rectangle
    /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
    /// 'Create a new Pdf3DAnnotation
    /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
    /// 'Create a new Pdf3DProjection
    /// Dim projection As Pdf3DProjection = New Pdf3DProjection(Pdf3DProjectionType.Perspective)
    /// projection1.FieldOfView = 50
    /// projection1.ClipStyle = Pdf3DProjectionClipStyle.ExplicitNearFar
    /// projection1.NearClipDistance = 10
    /// 'Create a new Pdf3DView.
    /// Dim view As Pdf3DView = New Pdf3DView()
    /// view.Projection = projection
    /// annotation.Views.Add(view)
    /// 'Draw a annotation into the new page.
    /// page.Annoatations.Add(annotation)
    /// 'Save the  document to disk.
    /// document.Save("Pdf3DProjection.pdf")
    /// </code>
    /// </example> 
    public class Pdf3DProjection : IPdfWrapper
    {
        #region Fields
        /// <summary>
        /// Internal variable to store type.
        /// </summary>
        private Pdf3DProjectionType m_Type;

        /// <summary>
        /// Internal variable to store Clip Style.
        /// </summary>
        private Pdf3DProjectionClipStyle m_ClipStyle;

        /// <summary>
        /// Internal variable to store Ortho Scale mode.
        /// </summary>
        private Pdf3DProjectionOrthoScaleMode m_OrthoScalemode;

        /// <summary>
        /// Internal variable to store far Clip Distance.
        /// </summary>
        private float m_farClipDistance;

        /// <summary>
        /// Internal variable to store field Of View.
        /// </summary>
        private float m_fieldOfView;

        /// <summary>
        /// Internal variable to store near Clip Distance.
        /// </summary>
        private float m_nearClipDistance;

        /// <summary>
        /// Internal variable to store scaling.
        /// </summary>
        private float m_scaling;

        /// <summary>
        /// Internal variable to store dictionary.
        /// </summary>
        private PdfDictionary m_dictionary = new PdfDictionary();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the type of the projection.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new Pdf3DProjection
        /// Pdf3DProjection projection = new Pdf3DProjection();
        /// projection.ProjectionType = Pdf3DProjectionType.Perspective;
        /// //Create a new Pdf3DView
        /// Pdf3DView view = new Pdf3DView();
        /// view.Projection = projection;
        /// //Adds the annotation in a new page.
        /// page.Annoatations.Add(annotation);
        /// //Save the  document to disk.
        /// document.Save("Pdf3DProjection.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new Pdf3DAnnotation
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Create a new Pdf3DProjection
        /// Dim projection As Pdf3DProjection = New Pdf3DProjection(Pdf3DProjectionType.Perspective)
        /// projection.ProjectionType = Pdf3DProjectionType.Perspective
        /// 'Create a new Pdf3DView
        /// Dim  view As Pdf3DView = New Pdf3DView()
        /// view.Projection = projection
        /// 'Draw a annotation into the new page.
        /// page.Annoatations.Add(annotation)
        /// 'Save the  document to disk.
        /// document.Save("Pdf3DProjection.pdf")
        /// </code>
        /// </example> 
        public Pdf3DProjectionType ProjectionType
        {
            get
            {
                return this.m_Type;
            }

            set
            {
                this.m_Type = value;
            }
        }

        /// <summary>
        /// Gets or sets the projection ClipStyle.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new Pdf3DProjection
        /// Pdf3DProjection projection = new Pdf3DProjection();
        /// projection.ClipStyle = Pdf3DProjectionClipStyle.ExplicitNearFar;
        /// //Create a new Pdf3DView
        /// Pdf3DView view = new Pdf3DView();
        /// view.Projection = projection;
        /// //Adds the annotation in a new page.
        /// page.Annoatations.Add(annotation);
        /// //Save the  document to disk.
        /// document.Save("Pdf3DProjection.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new Pdf3DAnnotation
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Create a new Pdf3DProjection
        /// Dim projection As Pdf3DProjection = New Pdf3DProjection(Pdf3DProjectionType.Perspective)
        /// projection.ClipStyle = Pdf3DProjectionClipStyle.ExplicitNearFar
        /// 'Create a new Pdf3DView
        /// Dim  view As Pdf3DView = New Pdf3DView()
        /// view.Projection = projection
        /// 'Draw a annotation into the new page.
        /// page.Annoatations.Add(annotation)
        /// 'Save the  document to disk.
        /// document.Save("Pdf3DProjection.pdf")
        /// </code>
        /// </example> 
        public Pdf3DProjectionClipStyle ClipStyle
        {
            get
            {
                return this.m_ClipStyle;
            }

            set
            {
                this.m_ClipStyle = value;
            }
        }

        /// <summary>
        ///  Gets or sets the scale mode for ortho graphic projections.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new Pdf3DProjection
        /// Pdf3DProjection projection = new Pdf3DProjection();
        /// projection.OrthoScaleMode = Pdf3DProjectionOrthoScaleMode.Width;
        /// //Create a new Pdf3DView
        /// Pdf3DView view = new Pdf3DView();
        /// view.Projection = projection;
        /// //Adds the annotation in a new page.
        /// page.Annoatations.Add(annotation);
        /// //Save the  document to disk.
        /// document.Save("Pdf3DProjection.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new Pdf3DAnnotation
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Create a new Pdf3DProjection
        /// Dim projection As Pdf3DProjection = New Pdf3DProjection(Pdf3DProjectionType.Perspective)
        /// projection.OrthoScaleMode = Pdf3DProjectionOrthoScaleMode.Width
        /// 'Create a new Pdf3DView
        /// Dim  view As Pdf3DView = New Pdf3DView()
        /// view.Projection = projection
        /// 'Draw a annotation into the new page.
        /// page.Annoatations.Add(annotation)
        /// 'Save the  document to disk.
        /// document.Save("Pdf3DProjection.pdf")
        /// </code>
        /// </example> 
        public Pdf3DProjectionOrthoScaleMode OrthoScaleMode
        {
            get
            {
                return this.m_OrthoScalemode;
            }

            set
            {
                this.m_OrthoScalemode = value;
            }
        }

        /// <summary>
        /// Gets or sets the far clipping distance.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new Pdf3DProjection
        /// Pdf3DProjection projection = new Pdf3DProjection();
        /// projection.FarClipDistance = 10;
        /// //Create a new Pdf3DView
        /// Pdf3DView view = new Pdf3DView();
        /// view.Projection = projection;
        /// //Adds the annotation in a new page.
        /// page.Annoatations.Add(annotation);
        /// //Save the  document to disk.
        /// document.Save("Pdf3DProjection.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new Pdf3DAnnotation
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Create a new Pdf3DProjection
        /// Dim projection As Pdf3DProjection = New Pdf3DProjection(Pdf3DProjectionType.Perspective)
        /// projection1.FarClipDistance = 50
        /// 'Create a new Pdf3DView
        /// Dim  view As Pdf3DView = New Pdf3DView()
        /// view.Projection = projection
        /// 'Draw a annotation into the new page.
        /// page.Annoatations.Add(annotation)
        /// 'Save the  document to disk.
        /// document.Save("Pdf3DProjection.pdf")
        /// </code>
        /// </example> 
        public float FarClipDistance
        {
            get
            {
                return this.m_farClipDistance;
            }

            set
            {
                this.m_farClipDistance = value;
            }
        }

        /// <summary>
        /// Gets or sets the field of view.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new Pdf3DProjection
        /// Pdf3DProjection projection = new Pdf3DProjection();
        /// projection.FieldOfView = 10;
        /// //Create a new Pdf3DView
        /// Pdf3DView view = new Pdf3DView();
        /// view.Projection = projection;
        /// //Adds the annotation in a new page.
        /// page.Annoatations.Add(annotation);
        /// //Save the  document to disk.
        /// document.Save("Pdf3DProjection.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new Pdf3DAnnotation
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Create a new Pdf3DProjection
        /// Dim projection As Pdf3DProjection = New Pdf3DProjection(Pdf3DProjectionType.Perspective)
        /// projection.FieldOfView = 10
        /// 'Create a new Pdf3DView
        /// Dim  view As Pdf3DView = New Pdf3DView()
        /// view.Projection = projection
        /// 'Draw a annotation into the new page.
        /// page.Annoatations.Add(annotation)
        /// 'Save the  document to disk.
        /// document.Save("Pdf3DProjection.pdf")
        /// </code>
        /// </example> 
        public float FieldOfView
        {
            get
            {
                return this.m_fieldOfView;
            }

            set
            {
                this.m_fieldOfView = value;
            }
        }

        /// <summary>
        /// Gets or sets the near clipping distance.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new Pdf3DProjection
        /// Pdf3DProjection projection = new Pdf3DProjection();
        /// projection.NearClipDistance = 10;
        /// //Create a new Pdf3DView
        /// Pdf3DView view = new Pdf3DView();
        /// view.Projection = projection;
        /// //Adds the annotation in a new page.
        /// page.Annoatations.Add(annotation);
        /// //Save the  document to disk.
        /// document.Save("Pdf3DProjection.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new Pdf3DAnnotation
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Create a new Pdf3DProjection
        /// Dim projection As Pdf3DProjection = New Pdf3DProjection(Pdf3DProjectionType.Perspective)
        /// projection.NearClipDistance = 10
        /// 'Create a new Pdf3DView
        /// Dim  view As Pdf3DView = New Pdf3DView()
        /// view.Projection = projection
        /// 'Draw a annotation into the new page.
        /// page.Annoatations.Add(annotation)
        /// 'Save the  document to disk.
        /// document.Save("Pdf3DProjection.pdf")
        /// </code>
        /// </example> 
        public float NearClipDistance
        {
            get
            {
                return this.m_nearClipDistance;
            }

            set
            {
                this.m_nearClipDistance = value;
            }
        }

        /// <summary>
        /// Gets or sets the projection scaling.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new Pdf3DProjection
        /// Pdf3DProjection projection = new Pdf3DProjection();
        /// projection.Scaling = 10;
        /// //Create a new Pdf3DView
        /// Pdf3DView view = new Pdf3DView();
        /// view.Projection = projection;
        /// //Adds the annotation in a new page.
        /// page.Annoatations.Add(annotation);
        /// //Save the  document to disk.
        /// document.Save("Pdf3DProjection.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new Pdf3DAnnotation
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Create a new Pdf3DProjection
        /// Dim projection As Pdf3DProjection = New Pdf3DProjection(Pdf3DProjectionType.Perspective)
        /// projection.Scaling = 10
        /// 'Create a new Pdf3DView
        /// Dim  view As Pdf3DView = New Pdf3DView()
        /// view.Projection = projection
        /// 'Draw a annotation into the new page.
        /// page.Annoatations.Add(annotation)
        /// 'Save the  document to disk.
        /// document.Save("Pdf3DProjection.pdf")
        /// </code>
        /// </example> 
        public float Scaling
        {
            get
            {
                return this.m_scaling;
            }

            set
            {
                this.m_scaling = value;
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
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Pdf3DProjection"/> class.
        /// </summary>
        public Pdf3DProjection()
        {
            this.Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Pdf3DProjection"/> class.
        /// </summary>
        /// <param name="type">The Pdf3D Projection Type.</param>
        /// <example>
        /// <code lang = "C#">
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new Pdf3D Annotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// //Create a new Pdf3DProjection
        /// Pdf3DProjection projection = new Pdf3DProjection();
        /// projection.ProjectionType = Pdf3DProjectionType.Perspective;
        /// projection.FieldOfView = 10;
        /// projection.ClipStyle = Pdf3DProjectionClipStyle.ExplicitNearFar;
        /// projection.NearClipDistance = 10;
        /// //Create a new Pdf3DView
        /// Pdf3DView view = new Pdf3DView();
        /// view.Projection = projection;
        /// annotation.Views.Add(view);
        /// page.Annoatations.Add(annotation);
        /// //Save the document to disk.
        /// document.Save("Pdf3DProjection.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new Pdf3DAnnotation
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Create a new Pdf3DProjection
        /// Dim projection As Pdf3DProjection = New Pdf3DProjection(Pdf3DProjectionType.Perspective)
        /// projection1.FieldOfView = 50
        /// projection1.ClipStyle = Pdf3DProjectionClipStyle.ExplicitNearFar
        /// projection1.NearClipDistance = 10
        /// 'Create a new Pdf3DView
        /// Dim  view As Pdf3DView = New Pdf3DView()
        /// view.Projection = projection
        /// annotation.Views.Add(view)
        /// 'Draw a annotation into the new page.
        /// page.Annoatations.Add(annotation)
        /// 'Save the  document to disk.
        /// document.Save("Pdf3DProjection.pdf")
        /// </code>
        /// </example> 
        public Pdf3DProjection(Pdf3DProjectionType type)
            : this()
        {
            this.m_Type = type;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Initializes annotation object.
        /// </summary>
        protected virtual void Initialize()
        {
            this.m_dictionary.BeginSave += new SavePdfPrimitiveEventHandler(this.Dictionary_BeginSave);
            this.m_dictionary.SetProperty(DictionaryProperties.Type, new PdfName(DictionaryProperties.PROJECTION));
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
            if (this.ClipStyle == Pdf3DProjectionClipStyle.ExplicitNearFar)
            {
                this.Dictionary.SetProperty(DictionaryProperties.CLIPPINGSTYLE, new PdfName(DictionaryProperties.XNF));
            }
            else
            {
                this.Dictionary.SetProperty(DictionaryProperties.CLIPPINGSTYLE, new PdfName(DictionaryProperties.ANF));
            }

            if (this.ClipStyle == Pdf3DProjectionClipStyle.ExplicitNearFar && this.m_farClipDistance >= 0f)
            {
                this.Dictionary.SetProperty(DictionaryProperties.F, new PdfNumber(this.m_farClipDistance));
            }

            if (this.m_Type == Pdf3DProjectionType.Perspective && this.m_nearClipDistance >= 0f)
            {
                this.Dictionary.SetProperty(DictionaryProperties.N, new PdfNumber(this.m_nearClipDistance));
            }
            else if (((this.m_Type == Pdf3DProjectionType.Orthographic) && (this.m_ClipStyle == Pdf3DProjectionClipStyle.ExplicitNearFar)) && (this.m_nearClipDistance >= 0f))
            {
                this.Dictionary.SetProperty(DictionaryProperties.N, new PdfNumber(this.m_nearClipDistance));
            }

            if (this.m_Type == Pdf3DProjectionType.Perspective)
            {
                this.Dictionary.SetProperty(DictionaryProperties.FOV, new PdfNumber(this.m_fieldOfView));
            }

            if (this.m_scaling > 0f)
            {
                if (this.m_Type == Pdf3DProjectionType.Perspective)
                {
                    this.Dictionary.SetProperty(DictionaryProperties.PS, new PdfNumber(this.m_scaling));
                }

                if (this.m_Type == Pdf3DProjectionType.Orthographic)
                {
                    this.Dictionary.SetProperty(DictionaryProperties.OS, new PdfNumber(this.m_scaling));
                }
            }

            if (this.m_OrthoScalemode == Pdf3DProjectionOrthoScaleMode.Absolute)
            {
                this.Dictionary.SetProperty(DictionaryProperties.OB, new PdfName("Absolute"));
            }
            else if (this.m_OrthoScalemode == Pdf3DProjectionOrthoScaleMode.Height)
            {
                this.Dictionary.SetProperty(DictionaryProperties.OB, new PdfName("H"));
            }
            else if (this.m_OrthoScalemode == Pdf3DProjectionOrthoScaleMode.Max)
            {
                this.Dictionary.SetProperty(DictionaryProperties.OB, new PdfName("Max"));
            }
            else if (this.m_OrthoScalemode == Pdf3DProjectionOrthoScaleMode.Min)
            {
                this.Dictionary.SetProperty(DictionaryProperties.OB, new PdfName("Min"));
            }
            else if (this.m_OrthoScalemode == Pdf3DProjectionOrthoScaleMode.Width)
            {
                this.Dictionary.SetProperty(DictionaryProperties.OB, new PdfName("W"));
            }
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
