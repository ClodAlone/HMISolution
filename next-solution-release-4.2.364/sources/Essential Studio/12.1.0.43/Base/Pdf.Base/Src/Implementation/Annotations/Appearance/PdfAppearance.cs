#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents the appearance of an annotation.
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document.
    /// PdfPage page = document.Pages.Add();
    /// //Create a new rectangle
    /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
    /// //Create a new Pdf3D Annotation.
    /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
    /// annotation.Appearance = new PdfAppearance(annotation);
    /// annotation.Appearance.Normal.Draw(page, new PointF(annotation.Location.X, annotation.Location.Y));
    /// page.Annoatations.Add(annotation);
    /// //Save the document to disk.
    /// document.Save("PdfAppearance.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document.
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create a new rectangle
    /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
    /// 'Create a new Sound Annotation.
    /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
    /// 'Pdf 3D Apperance
    /// annotation.Appearance = New PdfAppearance(annotation)
    /// annotation.Appearance.Normal.Draw(page, New PointF(annotation.Location.X, annotation.Location.Y))
    /// 'Draw a annotation into the new page.
    /// page.Annoatations.Add(annotation)
    /// 'Save the document to disk.
    /// document.Save("PdfAppearance.pdf")
    /// </code>
    /// </example> 
    public class PdfAppearance : IPdfWrapper
    {
        #region Fields
        /// <summary>
        /// Normal appearance.
        /// </summary>
        private PdfTemplate m_templateNormal;

        /// <summary>
        /// Mouse hover appearance.
        /// </summary>
        private PdfTemplate m_templateMouseHover;

        /// <summary>
        /// Mouse pressed appearance.
        /// </summary>
        private PdfTemplate m_templatePressed;

        /// <summary>
        /// Internal variable to store annotation.
        /// </summary>
        private PdfAnnotation m_annotation = null;

        /// <summary>
        /// Internal variable to store dictionary.
        /// </summary>
        private PdfDictionary m_dictionary = new PdfDictionary();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets <see cref="PdfTemplate"/> object which applied to annotation in normal state.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new Pdf3D Annotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// annotation.Appearance = new PdfAppearance(annotation);
        /// annotation.Appearance.Normal.Draw(page, new PointF(annotation.Location.X, annotation.Location.Y));
        /// page.Annoatations.Add(annotation);
        /// //Save the document to disk.
        /// document.Save("3DViews.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new Pdf3DAnnotation.
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Pdf 3D Apperance
        /// annotation.Appearance = New PdfAppearance(annotation)
        /// annotation.Appearance.Normal.Draw(page, New PointF(annotation.Location.X, annotation.Location.Y))
        /// 'Draw a annotation into the new page.
        /// page.Annoatations.Add(annotation)
        /// 'Save the document to disk.
        /// document.Save("3DViews.pdf")
        /// </code>
        /// </example> 
        public PdfTemplate Normal
        {
            get
            {
                if (this.m_templateNormal == null)
                {
                    this.m_templateNormal = new PdfTemplate(this.m_annotation.Size);
                    this.m_dictionary.SetProperty(DictionaryProperties.N, new PdfReferenceHolder(this.m_templateNormal));
                }

                return this.m_templateNormal;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Normal");
                }

                if (this.m_templateNormal != value)
                {
                    this.m_templateNormal = value;
                    this.m_dictionary.SetProperty(DictionaryProperties.N, new PdfReferenceHolder(this.m_templateNormal));
                }
            }
        }

        /// <summary>
        /// Gets or sets <see cref="PdfTemplate"/> object which applied to the annotation on hovering the mouse.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new Pdf3D Annotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// annotation.Appearance = new PdfAppearance(annotation);
        /// annotation.Appearance.MouseHover.Draw(page, new PointF(annotation.Location.X, annotation.Location.Y));
        /// page.Annoatations.Add(annotation);
        /// 'Save the document to disk.
        /// document.Save("3DViews.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new Pdf3DAnnotation.
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Pdf 3D Apperance
        /// annotation.Appearance = New PdfAppearance(annotation)
        /// annotation.Appearance.MouseHover.Draw(page, New PointF(annotation.Location.X, annotation.Location.Y))
        /// 'Draw a annotation into the new page.
        /// page.Annoatations.Add(annotation)
        /// 'Save the document to disk.
        /// document.Save("3DViews.pdf")
        /// </code>
        /// </example> 
        public PdfTemplate MouseHover
        {
            get
            {
                if (this.m_templateMouseHover == null)
                {
                    this.m_templateMouseHover = new PdfTemplate(this.m_annotation.Size);
                    this.m_dictionary.SetProperty(DictionaryProperties.R, new PdfReferenceHolder(this.m_templateMouseHover));
                }

                return this.m_templateMouseHover;
            }

            set
            {
                if (this.m_templateMouseHover != value)
                {
                    this.m_templateMouseHover = value;
                    this.m_dictionary.SetProperty(DictionaryProperties.R, new PdfReferenceHolder(this.m_templateMouseHover));
                }
            }
        }

        /// <summary>
        /// Gets or sets <see cref="PdfTemplate"/> object which applied to an annotation when mouse button is pressed.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create font, font style and brush
        /// //Create a new Pdf3D Annotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// annotation.Appearance = new PdfAppearance(annotation);
        /// annotation.Appearance.Pressed.Draw(page, new PointF(annotation.Location.X, annotation.Location.Y));
        /// page.Annoatations.Add(annotation);
        /// //Save the document to disk.
        /// document.Save("3DViews.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new Pdf3DAnnotation.
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Creates a new Pdf 3D Apperance
        /// annotation.Appearance = New PdfAppearance(annotation)
        /// annotation.Appearance.Pressed.Draw(page, New PointF(annotation.Location.X, annotation.Location.Y))
        /// 'Draw a annotation into the new page.
        /// page.Annoatations.Add(annotation)
        /// 'Save the document to disk.
        /// document.Save("3DViews.pdf")
        /// </code>
        /// </example> 
        public PdfTemplate Pressed
        {
            get
            {
                if (this.m_templatePressed == null)
                {
                    this.m_templatePressed = new PdfTemplate(this.m_annotation.Size);
                    this.m_dictionary.SetProperty(DictionaryProperties.D, new PdfReferenceHolder(this.m_templatePressed));
                }

                return this.m_templatePressed;
            }

            set
            {
                if (value != this.m_templatePressed)
                {
                    this.m_templatePressed = value;
                    this.m_dictionary.SetProperty(DictionaryProperties.D, new PdfReferenceHolder(this.m_templatePressed));
                }
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfAppearance"/> class.
        /// </summary>
        /// <param name="annotation">The <see cref="PdfAnnotation"/> object specifies the annotation.</param>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new Pdf3D Annotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// annotation.Appearance = new PdfAppearance(annotation);
        /// annotation.Appearance.Normal.Draw(page, new PointF(annotation.Location.X, annotation.Location.Y));
        /// page.Annoatations.Add(annotation);
        /// //Save the document to disk.
        /// document.Save("3DViews.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new Pdf3DAnnotation.
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// annotation.Appearance = New PdfAppearance(annotation)
        /// annotation.Appearance.Normal.Draw(page, New PointF(annotation.Location.X, annotation.Location.Y))
        /// 'Draw a annotation into the new page.
        /// page.Annoatations.Add(annotation);
        /// Save the document to disk.
        /// document.Save("3DViews.pdf")
        /// </code>
        /// </example> 
        public PdfAppearance(PdfAnnotation annotation)
            : base()
        {
            this.m_annotation = annotation;
            PdfTemplate t = this.Normal;
            PdfGraphics g = t.Graphics;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the normal template.
        /// </summary>
        /// <returns>Normal appearance template.</returns>
        internal PdfTemplate GetNormalTemplate()
        {
            return this.m_templateNormal;
        }

        /// <summary>
        /// Gets the pressed template.
        /// </summary>
        /// <returns>Pdf Template</returns>
        internal PdfTemplate GetPressedTemplate()
        {
            return this.m_templatePressed;
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
