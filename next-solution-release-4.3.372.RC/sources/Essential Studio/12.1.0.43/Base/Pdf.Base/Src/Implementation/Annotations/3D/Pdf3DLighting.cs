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
    /// Represents the lighting scheme for the 3D artwork.
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document.
    /// PdfPage page = document.Pages.Add();
    /// //Create a new Pdf3DAnnotation.
    /// Pdf3DAnnotation annot = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
    /// //Creates a new Pdf3DLighting.
    /// Pdf3DLighting lighting = new Pdf3DLighting();
    /// lighting.Style = Pdf3DLightingStyle.CAD;
    /// //Create a new Pdf3DView.
    /// Pdf3DView view = new Pdf3DView();
    /// view.LightingScheme = lighting;
    /// annot.Views.Add(view);
    /// page.Annoatations.Add(annot);
    /// //Save document to disk.
    /// document.Save("Pdf3DLighting.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document.
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create a new Pdf3DAnnotation.
    /// Dim annot As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
    /// 'Create a new Pdf3DLighting.
    /// Dim lighting As Pdf3DLighting = New Pdf3DLighting()
    /// lighting.Style = Pdf3DLightingStyle.Headlamp
    /// 'Create a new Pdf3DView.
    /// Dim  view As Pdf3DView = New Pdf3DView()
    /// view.LightingScheme = lighting;
    /// annot.Views.Add(defaultView)
    /// 'Draw a annot into the new page.
    /// page.Annoatations.Add(annot)
    /// 'Save document to disk.
    /// document.Save("Pdf3DLighting.pdf")
    /// </code>
    /// </example> 
    public class Pdf3DLighting : IPdfWrapper
    {
        #region Fields
        private Pdf3DLightingStyle m_lightingStyle;

        /// <summary>
        /// Internal variable to store dictionary.
        /// </summary>
        private PdfDictionary m_dictionary = new PdfDictionary();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the Lighting style of the 3D artwork.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new Pdf3DAnnotation.
        /// Pdf3DAnnotation annot = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// //Creates a new Pdf3DLighting.
        /// Pdf3DLighting lighting = new Pdf3DLighting();
        /// lighting.Style = Pdf3DLightingStyle.CAD;
        /// Pdf3DView view = new Pdf3DView();
        /// view.LightingScheme = lighting;
        /// annot.Views.Add(view);
        /// page.Annoatations.Add(annot);
        /// //Save document to disk.
        /// document.Save("Pdf3DLighting.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new Pdf3DAnnotation.
        /// Dim annot As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Create a new Pdf3DLighting.
        /// Dim lighting As Pdf3DLighting = New Pdf3DLighting()
        /// lighting.Style = Pdf3DLightingStyle.Headlamp
        /// Dim  view As Pdf3DView = New Pdf3DView()
        /// view.LightingScheme = lighting
        /// annot.Views.Add(defaultView)
        /// 'Draw a annot into the new page.
        /// page.Annoatations.Add(annot)
        /// 'Save document to disk.
        /// document.Save("Pdf3DLighting.pdf")
        /// </code>
        /// </example> 
        public Pdf3DLightingStyle Style
        {
            get
            {
                return this.m_lightingStyle;
            }

            set
            {
                this.m_lightingStyle = value;
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
        /// Initializes a new instance of the <see cref="Pdf3DLighting"/> class.
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
        /// Pdf3DAnnotation annot = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// Pdf3DLighting lighting = new Pdf3DLighting();
        /// lighting.Style = Pdf3DLightingStyle.CAD;
        /// Pdf3DView view = new Pdf3DView();
        /// view.LightingScheme = lighting;
        /// annot.Views.Add(view);
        /// page.Annoatations.Add(annot);
        /// //Save document to disk.
        /// document.Save("Pdf3DLighting.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new Pdf3DAnnotation.
        /// Dim annot As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Creates a new Pdf3DLighting.
        /// Dim lighting As Pdf3DLighting = New Pdf3DLighting()
        /// lighting.Style = Pdf3DLightingStyle.Headlamp
        /// Dim  view As Pdf3DView = New Pdf3DView()
        /// view.LightingScheme = lighting
        /// annot.Views.Add(defaultView)
        /// 'Draw a annot into the new page.
        /// page.Annoatations.Add(annot)
        /// 'Save document to disk.
        /// document.Save("Pdf3DLighting.pdf")
        /// </code>
        /// </example> 
        public Pdf3DLighting()
        {
            this.Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Pdf3DLighting"/> class.
        /// </summary>
        /// <param name="style">The <see cref="Pdf3DLightingStyle"/> object specifies the style of the 3D artwork.</param>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new Pdf3DAnnotation.
        /// Pdf3DAnnotation annot = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// //Creates a new Pdf3DLighting.
        /// Pdf3DLighting lighting = new Pdf3DLighting(Pdf3DLightingStyle.None);
        /// lighting.Style = Pdf3DLightingStyle.CAD;
        /// Pdf3DView view = new Pdf3DView();
        /// view.LightingScheme = lighting;
        /// annot.Views.Add(view);
        /// page.Annoatations.Add(annot);
        /// //Save document to disk.
        /// document.Save("Pdf3DLighting.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new Pdf3DAnnotation.
        /// Dim annot As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Creates a new Pdf3DLighting.
        /// Dim lighting As Pdf3DLighting = New Pdf3DLighting(Pdf3DLightingStyle.None)
        /// lighting.Style = Pdf3DLightingStyle.Headlamp
        /// Dim  view As Pdf3DView = New Pdf3DView()
        /// view.LightingScheme = lighting
        /// annot.Views.Add(defaultView)
        /// 'Draw a annot into the new page.
        /// page.Annoatations.Add(annot)
        /// 'Save document to disk.
        /// document.Save("Pdf3DLighting.pdf")
        /// </code>
        /// </example> 
        public Pdf3DLighting(Pdf3DLightingStyle style)
            : this()
        {
            this.m_lightingStyle = style;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Initializes annotation object.
        /// </summary>
        protected virtual void Initialize()
        {
            this.m_dictionary.BeginSave += new SavePdfPrimitiveEventHandler(this.Dictionary_BeginSave);
            this.m_dictionary.SetProperty(DictionaryProperties.Type, new PdfName(DictionaryProperties._3DLightingScheme));
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
            this.Dictionary.SetProperty(DictionaryProperties.Subtype, new PdfName(this.m_lightingStyle));
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
