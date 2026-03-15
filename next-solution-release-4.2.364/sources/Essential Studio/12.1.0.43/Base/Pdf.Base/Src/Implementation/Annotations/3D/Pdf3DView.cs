#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents a attributes to be applied to the virtual camera associated with a 3D annotation. 
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document.
    /// PdfPage page = document.Pages.Add();
    /// //Create a new Pdf3DAnnotation.
    /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
    /// //Create a new Pdf3DRendermode
    /// Pdf3DRendermode rendermode = new Pdf3DRendermode();
    /// rendermode.Style = Pdf3DRenderStyle.Solid;
    /// rendermode.AuxilaryColor = new PdfColor(Color.Green);
    /// rendermode.FaceColor = new PdfColor(Color.Black);
    /// PdfColor color = new PdfColor(Color.Silver);
    /// //Create a new Pdf3DBackground
    /// Pdf3DBackground background = new Pdf3DBackground();
    /// background.ApplyToEntireAnnotation = true;
    /// background.Color = color;
    /// //Creates a new Pdf3DLighting
    /// Pdf3DLighting lighting = new Pdf3DLighting();
    /// lighting.Style = Pdf3DLightingStyle.CAD;
    /// //Create a new Pdf3DView
    /// Pdf3DView view = new Pdf3DView();
    /// view.Background = background;
    /// view.LightingScheme = lighting;
    /// view.RenderMode=rendermode;
    /// annotation.Views.Add(view);
    /// page.Annoatations.Add(annotation);
    /// //Save the  document to disk.
    /// document.Save("3DView.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document.
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create a new Pdf3DAnnotation.
    /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
    /// 'Create a new Pdf3DRendermode
    /// Dim rendermode AsPdf3DRendermode  = New Pdf3DRendermode()
    /// rendermode.Style = Pdf3DRenderStyle.Solid
    /// rendermode.AuxilaryColor = New PdfColor(Color.Green)
    /// rendermode.FaceColor = New PdfColor(Color.Black)
    /// Dim color As PdfColor = New PdfColor(Color.Silver)
    /// 'Creates a new Pdf3DBackground
    /// Dim background As Pdf3DBackground = New Pdf3DBackground()
    /// background.ApplyToEntireAnnotation = true
    /// background.Color = color
    /// 'Creates a new Pdf3DLighting
    /// Dim lighting As Pdf3DLighting = New Pdf3DLighting()
    /// lighting.Style = Pdf3DLightingStyle.Headlamp
    /// 'Create a new Pdf3DView
    /// Dim  view As Pdf3DView = New Pdf3DView()
    /// view.RenderMode=rendermode
    /// view.Background = background
    /// view.LightingScheme = lighting
    /// annotation.Views.Add(view)
    /// 'Draw a annotation into the new page.
    /// page.Annoatations.Add(annotation)
    /// 'Save the  document to disk.
    /// document.Save("3DView.pdf")
    /// </code>
    /// </example> 
    public class Pdf3DView : IPdfWrapper
    {
        #region Fields
        /// <summary>
        /// Internal variable to store 3d Background.
        /// </summary>
        private Pdf3DBackground m_3dBackground;

        /// <summary>
        /// Internal variable to store 3d Cross Section Collection.
        /// </summary>
        private Pdf3DCrossSectionCollection m_3dCrossSectionCollection;

        /// <summary>
        /// Internal variable to store centre to World Matrix.
        /// </summary>
        private float[] m_centretoWorldMatrix;

        /// <summary>
        /// Internal variable to store 3d Lighting.
        /// </summary>
        private Pdf3DLighting m_3dLighting;

        /// <summary>
        /// Internal variable to store 3d Node Collection.
        /// </summary>
        private Pdf3DNodeCollection m_3dNodeCollection;

        /// <summary>
        /// Internal variable to store 3d Projection.
        /// </summary>
        private Pdf3DProjection m_3dProjection;

        /// <summary>
        /// Internal variable to store 3d Render mode.
        /// </summary>
        private Pdf3DRendermode m_3dRendermode;

        /// <summary>
        /// Internal variable to store reset Nodes State.
        /// </summary>
        private bool m_resetNodesState;

        /// <summary>
        /// Internal variable to store centre of orbit.
        /// </summary>
        private float m_centreOfOrbit;

        /// <summary>
        /// Internal variable to store external name.
        /// </summary>
        private string m_externalName;

        /// <summary>
        /// Internal variable to store internal name.
        /// </summary>
        private string m_internalName;

        /// <summary>
        /// Internal variable to store view node name.
        /// </summary>
        private string m_viewNodeName;

        /// <summary>
        /// Internal variable to store dictionary.
        /// </summary>
        private PdfDictionary m_dictionary = new PdfDictionary();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the background for this view.  
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new Pdf3DAnnotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// //Creates a new Pdf3DBackground
        /// Pdf3DBackground background = new Pdf3DBackground();
        /// background.ApplyToEntireAnnotation = true;
        /// background.Color = color;
        /// //Create a new Pdf3DView
        /// Pdf3DView view = new Pdf3DView();
        /// view.Background = background;
        /// //Adds a pdf3d view.
        /// annotation.Views.Add(view);
        /// //Adds a annotation.
        /// page.Annoatations.Add(annotation);
        /// //Save the  document to disk.
        /// document.Save("3DView.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new Pdf3DAnnotation.
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Creates a new Pdf3DBackground
        /// Dim background As Pdf3DBackground = New Pdf3DBackground()
        /// background.ApplyToEntireAnnotation = true
        /// background.Color = color
        /// 'Create a new Pdf3DView
        /// Dim  view As Pdf3DView = New Pdf3DView()
        /// view.Background = background
        /// 'Adds a pdf3d view
        /// annotation.Views.Add(view)
        /// 'Draw a annotation into the new page.
        /// page.Annoatations.Add(annotation)
        /// 'Save the  document to disk.
        /// document.Save("3DView.pdf")
        /// </code>
        /// </example>
        public Pdf3DBackground Background
        {
            get
            {
                return this.m_3dBackground;
            }

            set
            {
                this.m_3dBackground = value;
            }
        }

        /// <summary>
        /// Gets or sets the 3D transformation matrix. 
        /// </summary>
        /// <value>A 12-element 3D transformation matrix that specifies a position and orientation of the camera in world coordinates. </value>
        /// <remarks>If the array has more than 12 elements, only the first 12 will be considered.</remarks>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new Pdf3DAnnotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// //Creates a new Pdf3DBackground
        /// Pdf3DBackground background = new Pdf3DBackground();
        /// background.ApplyToEntireAnnotation = true;
        /// background.Color = color;
        /// float[] matrix = new float[] { -0.382684f, 0.92388f, -0.0000000766026f, 0.18024f, 0.0746579f, 0.980785f, 0.906127f, 0.37533f, -0.19509f, -300.669f, -112.432f, 45.6829f };
        /// //Create a new Pdf3DView
        /// Pdf3DView view = new Pdf3DView();
        /// view.CameraToWorldMatrix = matrix;
        /// view.Background = background;
        /// //Adds a pdf3d view.
        /// annotation.Views.Add(view);
        /// //Adds a annotation.
        /// page.Annoatations.Add(annotation);
        /// //Save the  document to disk.
        /// document.Save("3DView.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new Pdf3DAnnotation.
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Creates a new Pdf3DBackground
        /// Dim background As Pdf3DBackground = New Pdf3DBackground()
        /// background.ApplyToEntireAnnotation = true
        /// background.Color = color
        /// Dim matrix As Single() = New Single() {-1.382684F, 0.92388F, -0.0000000766026F, 2.18024F, 0.0746579F, 0.980785F, 0.906127F, 0.37533F, -0.19509F, -162.669F, -112.432F, 45.6829F}
        /// 'Create a new Pdf3DView
        /// Dim  view As Pdf3DView = New Pdf3DView()
        /// view.CameraToWorldMatrix = matrix
        /// view.Background = background
        /// 'Adds a pdf3d view
        /// annotation.Views.Add(view)
        /// 'Draw a annotation into the new page.
        /// page.Annoatations.Add(annotation)
        /// 'Save the  document to disk.
        /// document.Save("3DView.pdf")
        /// </code>
        /// </example>
        public float[] CameraToWorldMatrix
        {
            get
            {
                return this.m_centretoWorldMatrix;
            }

            set
            {
                this.m_centretoWorldMatrix = value;

                if ((this.m_centretoWorldMatrix != null) && (this.m_centretoWorldMatrix.Length < 12))
                {
                    throw new ArgumentOutOfRangeException("CameraToWorldMatrix.Length", "CameraToWorldMatrix array must have at least 12 elements.");
                }
            }
        }

        /// <summary>
        /// Gets or sets the center of orbit for 3D artwork. 
        /// </summary>
        /// <value>A non-negative number indicating a distance in the camera coordinate system along the z axis to the center of orbit for this view. </value>
        /// <remarks>If this value is negative, the viewer application must determine the center of orbit.</remarks>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new Pdf3DAnnotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// //Create a new Pdf3DView
        /// Pdf3DView view = new Pdf3DView();
        /// view.CenterOfOrbit = 10f;
        /// //Adds a pdf3d view.
        /// annotation.Views.Add(view);
        /// //Adds a annotation.
        /// page.Annoatations.Add(annotation);
        /// //Save the  document to disk.
        /// document.Save("3DView.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new Pdf3DAnnotation.
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Create a new Pdf3DView
        /// Dim  view As Pdf3DView = New Pdf3DView()
        /// view.CenterOfOrbit = 10f
        /// 'Adds a pdf3d view
        /// annotation.Views.Add(view)
        /// 'Draw a annotation into the new page.
        /// page.Annoatations.Add(annotation)
        /// 'Save the  document to disk.
        /// document.Save("3DView.pdf")
        /// </code>
        /// </example>
        public float CenterOfOrbit
        {
            get
            {
                return this.m_centreOfOrbit;
            }

            set
            {
                this.m_centreOfOrbit = value;
            }
        }

        /// <summary>
        /// Gets the list of cross sections for this view. 
        /// <value>A list of PDF3DCrossSection objects available for this view.</value>
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new Pdf3DAnnotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// //Create a new Pdf3DView
        /// Pdf3DView view = new Pdf3DView();
        /// Pdf3DCrossSectionCollection crossSectionCollection=view.CrossSections;
        /// //Adds a pdf3d view.
        /// annotation.Views.Add(view);
        /// //Adds a annotation.
        /// page.Annoatations.Add(annotation);
        /// //Save the  document to disk.
        /// document.Save("3DView.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new Pdf3DAnnotation.
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Create a new Pdf3DView
        /// Dim  view As Pdf3DView = New Pdf3DView()
        /// Dim crossSectionCollection As Pdf3DCrossSectionCollection=view.CrossSections
        /// 'Adds a pdf3d view
        /// annotation.Views.Add(view)
        /// 'Draw a annotation into the new page.
        /// page.Annoatations.Add(annotation)
        /// 'Save the  document to disk.
        /// document.Save("3DView.pdf")
        /// </code>
        /// </example>
        public Pdf3DCrossSectionCollection CrossSections
        {
            get
            {
                return this.m_3dCrossSectionCollection;
            }
        }

        /// <summary>
        /// Gets or sets the view's external name.
        /// </summary>
        /// <value>The external name of the view, suitable for presentation in a user interface.</value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new Pdf3DAnnotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// //Create a new Pdf3DView
        /// Pdf3DView view = new Pdf3DView();
        /// view.ExternalName="Near View";
        /// //Adds a pdf3d view.
        /// annotation.Views.Add(view);
        /// //Adds a annotation.
        /// page.Annoatations.Add(annotation);
        /// //Save the  document to disk.
        /// document.Save("3DView.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new Pdf3DAnnotation.
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Create a new Pdf3DView
        /// Dim  view As Pdf3DView = New Pdf3DView()
        /// view.ExternalName="Near View"
        /// 'Adds a pdf3d view
        /// annotation.Views.Add(view)
        /// 'Draw a annotation into the new page.
        /// page.Annoatations.Add(annotation)
        /// 'Save the  document to disk.
        /// document.Save("3DView.pdf")
        /// </code>
        /// </example>
        public string ExternalName
        {
            get
            {
                return this.m_externalName;
            }

            set
            {
                this.m_externalName = value;
            }
        }

        /// <summary>
        /// Gets or sets the view's internal name.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new Pdf3DAnnotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// //Create a new Pdf3DView
        /// Pdf3DView view = new Pdf3DView();
        /// view.InternalName=Guid.NewGuid().ToString("N");
        /// //Adds a pdf3d view.
        /// annotation.Views.Add(view);
        /// //Adds a annotation.
        /// page.Annoatations.Add(annotation);
        /// //Save the  document to disk.
        /// document.Save("3DView.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new Pdf3DAnnotation.
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Create a new Pdf3DView
        /// Dim  view As Pdf3DView = New Pdf3DView()
        /// view.InternalName=Guid.NewGuid().ToString("N")
        /// 'Adds a pdf3d view
        /// annotation.Views.Add(view)
        /// 'Draw a annotation into the new page.
        /// page.Annoatations.Add(annotation)
        /// 'Save the  document to disk.
        /// document.Save("3DView.pdf")
        /// </code>
        /// </example>
        public string InternalName
        {
            get
            {
                return this.m_internalName;
            }

            set
            {
                this.m_internalName = value;
            }
        }

        /// <summary>
        /// Gets or sets the Creates a new page and adds it as the last page of the document scheme for this view. 
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new Pdf3DAnnotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// Pdf3DLighting lighting = new Pdf3DLighting();
        /// lighting.Style = Pdf3DLightingStyle.CAD;
        /// //Create a new Pdf3DView
        /// Pdf3DView view = new Pdf3DView();
        /// view.LightingScheme = lighting;
        /// //Adds a pdf3d view.
        /// annotation.Views.Add(view);
        /// //Adds a annotation.
        /// page.Annoatations.Add(annotation);
        /// //Save the  document to disk.
        /// document.Save("3DView.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new Pdf3DAnnotation.
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// Dim lighting As Pdf3DLighting = New Pdf3DLighting()
        /// lighting.Style = Pdf3DLightingStyle.Headlamp
        /// 'Create a new Pdf3DView
        /// Dim  view As Pdf3DView = New Pdf3DView()
        /// view.LightingScheme = lighting
        /// 'Adds a pdf3d view
        /// annotation.Views.Add(view)
        /// 'Draw a annotation into the new page.
        /// page.Annoatations.Add(annotation)
        /// 'Save the  document to disk.
        /// document.Save("3DView.pdf")
        /// </code>
        /// </example>
        public Pdf3DLighting LightingScheme
        {
            get
            {
                return this.m_3dLighting;
            }

            set
            {
                this.m_3dLighting = value;
            }
        }

        /// <summary>
        /// Gets the list of 3D nodes for this view. 
        /// </summary>
        /// <value>A list of PDF3DNode objects available for this view.</value>        
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new Pdf3DAnnotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// 'Create a new Pdf3DView
        /// Pdf3DView  view   = new Pdf3DView();
        /// Pdf3DNodeCollection nodes = view.Nodes;
        /// //Adds a pdf3d view.
        /// annotation.Views.Add(view);
        /// //Adds a annotation.
        /// page.Annoatations.Add(annotation);
        /// //Save the  document to disk.
        /// document.Save("3DView.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new Pdf3DAnnotation.
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// Dim lighting As Pdf3DLighting = New Pdf3DLighting()
        /// lighting.Style = Pdf3DLightingStyle.Headlamp
        /// 'Create a new Pdf3DView
        /// Dim  view As Pdf3DView = New Pdf3DView()
        /// Dim nodes As Pdf3DNodeCollection = view.Nodes
        /// 'Adds a pdf3d view
        /// annotation.Views.Add(view)
        /// 'Draw a annotation into the new page.
        /// page.Annoatations.Add(annotation)
        /// 'Save the  document to disk.
        /// document.Save("3DView.pdf")
        /// </code>
        /// </example>
        public Pdf3DNodeCollection Nodes
        {
            get
            {
                return this.m_3dNodeCollection;
            }
        }

        /// <summary>
        /// Gets or sets the projection for this view. 
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new Pdf3DAnnotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// //Creates a new Pdf3DProjection
        /// Pdf3DProjection projection = new Pdf3DProjection();
        /// projection.ProjectionType = Pdf3DProjectionType.Perspective;
        /// projection.FieldOfView = 10;
        /// projection.ClipStyle = Pdf3DProjectionClipStyle.ExplicitNearFar;
        /// projection.NearClipDistance = 10;
        /// //Create a new Pdf3DView
        /// Pdf3DView view = new Pdf3DView();
        /// view.Projection = projection;
        /// //Adds a pdf3d view into the new page.
        /// annotation.Views.Add(view);
        /// page.Annoatations.Add(annotation);
        /// //Save the  document to disk.
        /// document.Save("3DView.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new Pdf3DAnnotation.
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Creates a new Pdf3DProjection.
        /// Dim projection As Pdf3DProjection = New Pdf3DProjection(Pdf3DProjectionType.Perspective)
        /// projection1.FieldOfView = 50
        /// projection1.ClipStyle = Pdf3DProjectionClipStyle.ExplicitNearFar
        /// projection1.NearClipDistance = 10
        /// 'Create a new Pdf3DView
        /// Dim  view As Pdf3DView = New Pdf3DView()
        /// view.Projection = projection
        /// 'Adds a pdf3d view into the new page.
        /// annotation.Views.Add(view)
        /// 'Save the  document to disk.
        /// document.Save("3DView.pdf")
        /// </code>
        /// </example>
        public Pdf3DProjection Projection
        {
            get
            {
                return this.m_3dProjection;
            }

            set
            {
                this.m_3dProjection = value;
            }
        }

        /// <summary>
        /// Gets or sets the rendering mode for this view. 
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new Pdf3DAnnotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// //Create a new Pdf3DRendermode
        /// Pdf3DRendermode rendermode = new Pdf3DRendermode();
        /// rendermode.Style = Pdf3DRenderStyle.Solid;
        /// rendermode.AuxilaryColor = new PdfColor(Color.Green);
        /// rendermode.FaceColor = new PdfColor(Color.Black);
        /// //Create a new Pdf3DView
        /// Pdf3DView view = new Pdf3DView();
        /// view.RenderMode=rendermode;
        /// //Adds a pdf3d view into the new page.
        /// annotation.Views.Add(view);
        /// page.Annoatations.Add(annotation);
        /// //Save the  document to disk.
        /// document.Save("3DView.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new Pdf3DAnnotation.
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Create a new Pdf3DRendermode
        /// Dim rendermode As Pdf3DRendermode  = New Pdf3DRendermode()
        /// rendermode.Style = Pdf3DRenderStyle.Solid
        /// rendermode.AuxilaryColor = New PdfColor(Color.Green)
        /// rendermode.FaceColor = New PdfColor(Color.Black)
        /// 'Create a new Pdf3DView
        /// Dim  view As Pdf3DView = New Pdf3DView()
        /// view.RenderMode=rendermode
        /// 'Adds a pdf3d view into the new page.
        /// annotation.Views.Add(view)
        /// 'Save the  document to disk.
        /// document.Save("3DView.pdf")
        /// </code>
        /// </example>
        public Pdf3DRendermode RenderMode
        {
            get
            {
                return this.m_3dRendermode;
            }

            set
            {
                this.m_3dRendermode = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether nodes specified in the Nodes collection are returned to their original states (as specified in the 3D artwork) before applying transformation matrices and opacity settings specified in the node dictionaries. 
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new Pdf3DAnnotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// //Create a new Pdf3DView
        /// Pdf3DView view = new Pdf3DView();
        /// view.ResetNodesState=true;
        /// //Adds a pdf3d view into the new page.
        /// annotation.Views.Add(view);
        /// page.Annoatations.Add(annotation);
        /// //Save the  document to disk.
        /// document.Save("3DView.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new Pdf3DAnnotation.
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Create a new Pdf3DView
        /// Dim  view As Pdf3DView = New Pdf3DView()
        /// view.ResetNodesState=True
        /// 'Adds a pdf3d view into the new page.
        /// annotation.Views.Add(view)
        /// 'Save the  document to disk.
        /// document.Save("3DView.pdf")
        /// </code>
        /// </example>
        public bool ResetNodesState
        {
            get
            {
                return this.m_resetNodesState;
            }

            set
            {
                this.m_resetNodesState = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the view node.         
        /// </summary>
        /// <remarks>The view node in the content stream defines all the properties for viewing the 3D artwork. If both ViewNodeName and CameraToWorldMatrix are specified, then ViewNodeName takes precedence.</remarks>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new Pdf3DAnnotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// //Create a new Pdf3DView
        /// Pdf3DView view = new Pdf3DView();
        /// string name=view.ViewNodeName;
        /// //Adds a pdf3d view into the new page.
        /// annotation.Views.Add(view);
        /// page.Annoatations.Add(annotation);
        /// //Save the  document to disk.
        /// document.Save("3DView.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new Pdf3DAnnotation.
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Create a new Pdf3DView
        /// Dim  view As Pdf3DView = New Pdf3DView()
        /// Dim name As String = view.ViewNodeName
        /// view.RenderMode=rendermode
        /// 'Adds a pdf3d view into the new page.
        /// annotation.Views.Add(view)
        /// 'Save the  document to disk.
        /// document.Save("3DView.pdf")
        /// </code>
        /// </example>
        public string ViewNodeName
        {
            get
            {
                return this.m_viewNodeName;
            }

            set
            {
                this.m_viewNodeName = value;
            }
        }

        /// <summary>
        /// Gets the dictionary.
        /// </summary>
        /// <value>The pdf3d view dictionary.</value>
        internal PdfDictionary Dictionary
        {
            get
            {
                return this.m_dictionary;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Pdf3DView"/> class.
        /// </summary>
        public Pdf3DView()
        {
            this.Initialize();
            this.m_3dNodeCollection = new Pdf3DNodeCollection();
            this.m_3dCrossSectionCollection = new Pdf3DCrossSectionCollection();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Initializes annotation object.
        /// </summary>
        protected virtual void Initialize()
        {
            this.m_dictionary.BeginSave += new SavePdfPrimitiveEventHandler(this.Dictionary_BeginSave);
            this.Dictionary[DictionaryProperties.Type] = new PdfName(DictionaryProperties._3DView);
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
            if (this.m_externalName != null && this.m_externalName.Length > 0)
            {
                this.Dictionary[DictionaryProperties.XN] = new PdfString(this.m_externalName);
            }

            if (this.m_internalName != null && this.m_internalName.Length > 0)
            {
                this.Dictionary[DictionaryProperties.IN] = new PdfString(this.m_internalName);
            }

            if (this.m_viewNodeName != null && this.m_viewNodeName.Length > 0)
            {
                this.Dictionary[DictionaryProperties.MS] = new PdfName(DictionaryProperties.U3D);
                this.Dictionary[DictionaryProperties.U3DPath] = new PdfName(this.m_internalName);
            }
            else
            {
                if (this.m_centretoWorldMatrix == null)
                {
                    throw new ArgumentNullException("CameraToWorldMatrix", "Either ViewNodeName or CameraToWorldMatrix properties must be specified.");
                }

                this.Dictionary[DictionaryProperties.MS] = new PdfName(DictionaryProperties.M);

                PdfArray centerMatrix = new PdfArray();
                for (int i = 0; i < this.m_centretoWorldMatrix.Length; i++)
                {
                    centerMatrix.Insert(i, new PdfNumber(this.m_centretoWorldMatrix[i]));
                }

                this.Dictionary.SetProperty(DictionaryProperties.C2W, new PdfArray(centerMatrix));
            }

            if (this.m_centreOfOrbit > 0f)
            {
                this.Dictionary.SetProperty(DictionaryProperties.CO, new PdfNumber(this.m_centreOfOrbit));
            }

            if (this.m_3dProjection != null)
            {
                this.Dictionary[DictionaryProperties.P] = new PdfReferenceHolder(this.m_3dProjection);
            }

            if (this.m_3dBackground != null)
            {
                this.Dictionary[DictionaryProperties.BG] = new PdfReferenceHolder(this.m_3dBackground);
            }

            if (this.m_3dRendermode != null)
            {
                this.Dictionary[DictionaryProperties.RM] = new PdfReferenceHolder(this.m_3dRendermode);
            }

            if (this.m_3dLighting != null)
            {
                this.Dictionary[DictionaryProperties.LS] = new PdfReferenceHolder(this.m_3dLighting);
            }

            if (this.m_3dCrossSectionCollection != null && this.m_3dCrossSectionCollection.Count > 0)
            {
                PdfArray crossSection = new PdfArray();
                for (int i = 0; i < this.m_3dCrossSectionCollection.Count; i++)
                {
                    crossSection.Insert(i, new PdfReferenceHolder(this.m_3dCrossSectionCollection[i]));
                }

                this.Dictionary.SetProperty(DictionaryProperties.SA, new PdfArray(crossSection));
            }

            if (this.m_3dNodeCollection != null && this.m_3dNodeCollection.Count > 0)
            {
                PdfArray nodes = new PdfArray();
                for (int i = 0; i < this.m_3dNodeCollection.Count; i++)
                {
                    nodes.Insert(i, new PdfReferenceHolder(this.m_3dNodeCollection[i]));
                }

                this.Dictionary.SetProperty(DictionaryProperties.NA, new PdfArray(nodes));
            }

            this.Dictionary.SetProperty(DictionaryProperties.NR, new PdfBoolean(this.m_resetNodesState));
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

    /// <summary>
    /// Represents a collection of Pdf3DView objects. 
    /// <example>
    /// <code lang = "C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document.
    /// PdfPage page = document.Pages.Add();
    /// //Create a new Pdf3DAnnotation.
    /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
    /// //Creates a new Pdf3DView
    /// Pdf3DView defaultView = new Pdf3DView();
    /// defaultView.ExternalName="Near View";
    /// defaultView.CameraToWorldMatrix=new float[] { -0.382684f, 0.92388f, -0.0000000766026f, 0.18024f, 0.0746579f, 0.980785f, 0.906127f, 0.37533f, -0.19509f, -100, -112.432f, 45.6829f };
    /// defaultView.CenterOfOrbit=131.695f;
    /// view.LightingScheme = Creates a new page and adds it as the last page of the document
    /// annotation.Views.Add(defaultView);
    /// page.Annoatations.Add(annotation);
    /// //Save the  document to disk.
    /// document.Save("3DViews.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document.
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create a new Pdf3DAnnotation.
    /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
    /// 'Creates a new Pdf3DView
    /// Dim defaultView As Pdf3DView  = New Pdf3DView()
    /// defaultView.ExternalName="Near View"
    /// defaultView.CameraToWorldMatrix=new Single() { -0.382684f, 0.92388f, -0.0000000766026f, 0.18024f, 0.0746579f, 0.980785f, 0.906127f, 0.37533f, -0.19509f, -100, -112.432f, 45.6829f }
    /// defaultView.CenterOfOrbit=131.695f
    /// annotation.Views.Add(defaultView)
    /// 'Draw a annotation into the new page.
    /// page.Annoatations.Add(annotation)
    /// 'Save the  document to disk.
    /// document.Save("3DViews.pdf")
    /// </code>
    /// </example> 
    /// </summary>
    public class Pdf3DViewCollection : List<Pdf3DView>
    {
        #region Methods
        /// <summary>
        /// Adds the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Pdf3DView</returns>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new Pdf3DAnnotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// Dim defaultView As Pdf3DView  = New Pdf3DView();
        /// defaultView.ExternalName="Near View";
        /// defaultView.CameraToWorldMatrix=new Single() { -0.382684f, 0.92388f, -0.0000000766026f, 0.18024f, 0.0746579f, 0.980785f, 0.906127f, 0.37533f, -0.19509f, -100, -112.432f, 45.6829f };
        /// defaultView.CenterOfOrbit=131.695f;
        /// annotation.Views.Add(defaultView);
        /// page.Annoatations.Add(annotation);
        /// </code>
        /// <code lang="VB">
        /// 'Create a new Pdf3DAnnotation.
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// Dim defaultView As Pdf3DView  = New Pdf3DView()
        /// defaultView.ExternalName="Near View"
        /// defaultView.CameraToWorldMatrix=new Single() { -0.382684f, 0.92388f, -0.0000000766026f, 0.18024f, 0.0746579f, 0.980785f, 0.906127f, 0.37533f, -0.19509f, -100, -112.432f, 45.6829f }
        /// defaultView.CenterOfOrbit=131.695f
        /// annotation.Views.Add(defaultView)
        /// </code>
        /// </example> 
        public int Add(Pdf3DView value)
        {
            base.Add(value);
            return base.IndexOf(value);
        }

        /// <summary>
        /// Determines whether [contains] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// if it contains the specified value, set to <c>true</c>.
        /// </returns>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new Pdf3DAnnotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// Dim defaultView As Pdf3DView  = New Pdf3DView();
        /// defaultView.ExternalName="Near View";
        /// defaultView.CameraToWorldMatrix=new Single() { -0.382684f, 0.92388f, -0.0000000766026f, 0.18024f, 0.0746579f, 0.980785f, 0.906127f, 0.37533f, -0.19509f, -100, -112.432f, 45.6829f };
        /// defaultView.CenterOfOrbit=131.695f;
        /// annotation.Views.Add(defaultView);
        /// bool exist=annotation.Views.Contains(defaultView);
        /// page.Annoatations.Add(annotation);
        /// </code>
        /// <code lang="VB">
        /// 'Create a new Pdf3DAnnotation.
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// Dim defaultView As Pdf3DView  = New Pdf3DView()
        /// defaultView.ExternalName="Near View"
        /// defaultView.CameraToWorldMatrix=new Single() { -0.382684f, 0.92388f, -0.0000000766026f, 0.18024f, 0.0746579f, 0.980785f, 0.906127f, 0.37533f, -0.19509f, -100, -112.432f, 45.6829f }
        /// defaultView.CenterOfOrbit=131.695f
        /// annotation.Views.Add(defaultView);
        /// Dim exist As bool =annotation.Views.Contains(defaultView)
        /// </code>
        /// </example> 
        public bool Contains(Pdf3DView value)
        {
            return base.Contains(value);
        }

        /// <summary>
        /// Indexes the of the Pdf3DView object.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Pdf3DView</returns>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new Pdf3DAnnotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// Dim defaultView As Pdf3DView  = New Pdf3DView();
        /// defaultView.ExternalName="Near View";
        /// defaultView.CameraToWorldMatrix=new Single() { -0.382684f, 0.92388f, -0.0000000766026f, 0.18024f, 0.0746579f, 0.980785f, 0.906127f, 0.37533f, -0.19509f, -100, -112.432f, 45.6829f };
        /// defaultView.CenterOfOrbit=131.695f;
        /// annotation.Views.Add(defaultView);
        /// int index=annotation.Views.IndexOf(defaultView);
        /// page.Annoatations.Add(annotation);
        /// </code>
        /// <code lang="VB">
        /// 'Create a new Pdf3DAnnotation.
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// Dim defaultView As Pdf3DView  = New Pdf3DView()
        /// defaultView.ExternalName="Near View"
        /// defaultView.CameraToWorldMatrix=new Single() { -0.382684f, 0.92388f, -0.0000000766026f, 0.18024f, 0.0746579f, 0.980785f, 0.906127f, 0.37533f, -0.19509f, -100, -112.432f, 45.6829f }
        /// defaultView.CenterOfOrbit=131.695f
        /// annotation.Views.Add(defaultView);
        /// Dim index As int=annotation.Views.IndexOf(defaultView)
        /// </code>
        /// </example> 
        public int IndexOf(Pdf3DView value)
        {
            return base.IndexOf(value);
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new Pdf3DAnnotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// Dim defaultView As Pdf3DView  = New Pdf3DView();
        /// defaultView.ExternalName="Near View";
        /// defaultView.CameraToWorldMatrix=new Single() { -0.382684f, 0.92388f, -0.0000000766026f, 0.18024f, 0.0746579f, 0.980785f, 0.906127f, 0.37533f, -0.19509f, -100, -112.432f, 45.6829f };
        /// defaultView.CenterOfOrbit=131.695f;
        /// annotation.Views.Add(defaultView);
        /// page.Annoatations.Insert(1,annotation);
        /// </code>
        /// <code lang="VB">
        /// 'Create a new Pdf3DAnnotation.
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// Dim defaultView As Pdf3DView  = New Pdf3DView()
        /// defaultView.ExternalName="Near View"
        /// defaultView.CameraToWorldMatrix=new Single() { -0.382684f, 0.92388f, -0.0000000766026f, 0.18024f, 0.0746579f, 0.980785f, 0.906127f, 0.37533f, -0.19509f, -100, -112.432f, 45.6829f }
        /// defaultView.CenterOfOrbit=131.695f
        /// annotation.Views.Insert(1,defaultView);
        /// </code>
        /// </example> 
        public void Insert(int index, Pdf3DView value)
        {
            base.Insert(index, value);
        }

        /// <summary>
        /// Removes the specified value.
        /// </summary>
        /// <param name="value">The Pdf3DView object.</param>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new Pdf3DAnnotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// Dim defaultView As Pdf3DView  = New Pdf3DView();
        /// defaultView.ExternalName="Near View";
        /// defaultView.CameraToWorldMatrix=new Single() { -0.382684f, 0.92388f, -0.0000000766026f, 0.18024f, 0.0746579f, 0.980785f, 0.906127f, 0.37533f, -0.19509f, -100, -112.432f, 45.6829f };
        /// defaultView.CenterOfOrbit=131.695f;
        /// annotation.Views.Add(defaultView);
        /// annotation.Views.Remove(defaultView);
        /// page.Annoatations.Add(annotation);
        /// </code>
        /// <code lang="VB">
        /// 'Create a new Pdf3DAnnotation.
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// Dim defaultView As Pdf3DView  = New Pdf3DView()
        /// defaultView.ExternalName="Near View"
        /// defaultView.CameraToWorldMatrix=new Single() { -0.382684f, 0.92388f, -0.0000000766026f, 0.18024f, 0.0746579f, 0.980785f, 0.906127f, 0.37533f, -0.19509f, -100, -112.432f, 45.6829f }
        /// defaultView.CenterOfOrbit=131.695f
        /// annotation.Views.Add(defaultView)
        /// annotation.Views.Remove(defaultView)
        /// </code>
        /// </example> 
        
        public void Remove(Pdf3DView value)
        {
            base.Remove(value);
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.Pdf.Interactive.Pdf3DView"/> at the specified index.
        /// </summary>
        /// <value>Pdf3DView</value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new Pdf3DAnnotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// Pdf3DView defaultView = CreateView("Near View",new float[] { -0.382684f, 0.92388f, -0.0000000766026f, 0.18024f, 0.0746579f, 0.980785f, 0.906127f, 0.37533f, -0.19509f, -100, -112.432f, 45.6829f }, 131.695f, background, projection, rendermode, Creates a new page and adds it as the last page of the document);
        /// annotation.Views.Add(defaultView);
        /// page.Annoatations.Add(annotation);
        /// Pdf3DView view=annotation.Views[0] as Pdf3DView;
        /// </code>
        /// <code lang="VB">
        /// 'Create a new Pdf3DAnnotation.
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// Pdf3DView defaultView = CreateView("Near View", new float[] { -0.382684f, 0.92388f, -0.0000000766026f, 0.18024f, 0.0746579f, 0.980785f, 0.906127f, 0.37533f, -0.19509f, -100, -112.432f, 45.6829f },131.695f, background, projection, rendermode, Creates a new page and adds it as the last page of the document);annotation.Views.Add(defaultView);
        /// annotation.Views.Add(defaultView)
        /// Dim view As Pdf3DView =annotation.Views[0]
        /// </code>
        /// </example> 
        public Pdf3DView this[int index]
        {
            get
            {
                return (Pdf3DView)base[index];
            }

            set
            {
                base[index] = value;
            }
        }

        #endregion
    }
}
