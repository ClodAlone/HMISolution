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

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Specifies an activation state of the 3D annotation.
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// //Create a new Document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document.
    /// PdfPage page = document.Pages.Add();
    /// //Create a new rectangle
    /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
    /// //Create a new Pdf3DAnnotation.
    /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
    /// //Create a new Pdf3DActivation
    /// Pdf3DActivation activation = new Pdf3DActivation();
    /// annotation.DeactivationState = Pdf3DDeactivationState.Live;
    /// activation.ShowToolbar = false;
    /// 3DAnnotation.Activation = activation;
    /// //Adds the annotation to a new page.
    /// page.Annoatations.Add(annotation);
    /// //Save the  document to disk.
    /// document.Save("Pdf3DActivation.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document.
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create a new rectangle
    /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
    /// 'Create a new Pdf3DAnnotation.
    /// Dim 3DAnnotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d"
    /// 'Creates a new Pdf3DActivation
    /// Dim activation As Pdf3DActivation = New Pdf3DActivation()
    /// activation.DeactivationState = Pdf3DDeactivationState.Live
    /// 3DAnnotation.Activation = activation
    /// 'Draws a annotation into the new page.
    /// page.Annoatations.Add(3DAnnotation)
    /// 'Save the  document to disk.
    /// document.Save("Pdf3DActivation.pdf")
    /// </code>
    /// </example> 
    public enum Pdf3DActivationState
    {
        /// <summary>
        /// Represents that the state in which the artwork has been read and a run-time instance of 
        /// the artwork has been created. In this state, it can be rendered but script-driven 
        /// real-time modifications (that is, animations) are disabled.
        /// </summary>
        Instantiated,

        /// <summary>
        /// Represents that the artwork is instantiated, and it is being modified in real time to 
        /// achieve some animation effect. In the case of keyframe animation, the artwork is 
        /// live while it is playing and then reverts to an instantiated state when playing 
        /// completes or is stopped.
        /// </summary>
        Live
    }

    /// <summary>
    /// Specifies the available modes for activating a 3D annotation. 
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// //Create a new PdfDocument
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document.
    /// PdfPage page = document.Pages.Add();
    /// //Create a new rectangle
    /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
    /// //Create a new Pdf3DAnnotation.
    /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
    /// //Creaate a new Pdf3DActivation
    /// Pdf3DActivation activation = new Pdf3DActivation();
    /// //Sets an activate mode.
    /// activation.ActivationMode = Pdf3DActivationMode.ExplicitActivation;
    /// annotation.Activation = activation;
    /// //Add the annotation to the new page.
    /// page.Annoatations.Add(annotation);
    /// //Save the document to disk.
    /// document.Save("Pdf3DActivation.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document.
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create a new rectangle
    /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
    /// 'Create a new Pdf3D Annotation.
    /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
    /// 'Creaate a new Pdf3DActivation
    /// Dim activation As Pdf3DActivation = New Pdf3DActivation()
    /// 'Sets an activate mode
    /// activation.ActivationMode = Pdf3DActivationMode.PageVisible
    /// annotation.Activation = activation
    /// 'Add the annotation to the new page
    /// page.Annoatations.Add(annotation)
    /// 'Save the  document to disk.
    /// document.Save("Pdf3DActivation.pdf")
    /// </code>
    /// </example> 
    public enum Pdf3DActivationMode
    {
        /// <summary>
        /// Represents that the annotation should be activated as soon as the page containing 
        /// the annotation is opened.
        /// </summary>
        PageOpen,

        /// <summary>
        /// Represents that the annotation should be activated as soon as any part of the page 
        /// containing the annotation becomes visible.
        /// </summary>
        PageVisible,

        /// <summary>
        /// Represents that the annotation should remain inactive until explicitly activated 
        /// by a script or user action.
        /// </summary>
        ExplicitActivation
    }

    /// <summary>
    /// Specifies the available modes for deactivating a 3D annotation. 
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// //Create a new Document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document.
    /// PdfPage page = document.Pages.Add();
    /// //Create a new rectangle
    /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
    /// //Create a new Pdf3DAnnotation.
    /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
    /// //Create a new Pdf3DActivation
    /// Pdf3DActivation activation = new Pdf3DActivation();
    /// //Sets the deactivation mode.
    /// activation.DeactivationMode = Pdf3DDeactivationMode.PageClose;
    /// annotation.Activation = activation;
    /// //Add this annotation to the new page.
    /// page.Annoatations.Add(annotation);
    /// //Save the  document to disk.
    /// document.Save("Pdf3DActivation.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document.
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create a new rectangle
    /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
    /// 'Create a new Pdf3D Annotation.
    /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
    /// 'Create a new Pdf3DActivation
    /// Dim activation As Pdf3DActivation = New Pdf3DActivation()
    /// 'Sets the deactivation mode
    /// activation.DeactivationMode = Pdf3DDeactivationMode.PageClose
    /// annotation.Activation = activation
    /// 'Draw a annotation into the new page.
    /// page.Annoatations.Add(annotation)
    /// 'Save the  document to disk.
    /// document.Save("Pdf3DActivation.pdf")
    /// </code>
    /// </example> 
    public enum Pdf3DDeactivationMode
    {
        /// <summary>
        /// Represents that the annotation should be deactivated as soon as the page is closed.
        /// </summary>
        PageClose,

        /// <summary>
        /// Represents that the annotation should be deactivated as soon as the page containing 
        /// the annotation becomes invisible.
        /// </summary>
        PageInvisible,

        /// <summary>
        /// Represents that the annotation should remain active until explicitly deactivated by a 
        /// script or user action.
        /// </summary>
        ExplicitDeactivation
    }

    /// <summary>
    /// Specifies the available states upon deactivating a 3D annotation. 
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document.
    /// PdfPage page = document.Pages.Add();
    /// //Create a new rectangle
    /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
    /// //Create a new Pdf3DAnnotation.
    /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
    /// //Create a new Pdf3DActivation
    /// Pdf3DActivation activation = new Pdf3DActivation();
    /// //Sets the deactivate state.
    /// activation.DeactivationState = Pdf3DDeactivationState.Live;
    /// activation.ShowToolbar = false;
    /// annotation.Activation = activation;
    /// page.Annoatations.Add(annotation);
    /// //Save the  document to disk.
    /// document.Save("Pdf3DActivation.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document.
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create a new rectangle
    /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
    /// 'Create a new Pdf3DActivation.
    /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
    /// 'Create a new Pdf3DActivation
    /// Dim activation As Pdf3DActivation = New Pdf3DActivation()
    /// 'Sets the deactivate state
    /// activation.DeactivationState = Pdf3DDeactivationState.Live
    /// annotation.Activation = activation
    /// 'Draw a annotation into the new page.
    /// page.Annoatations.Add(annotation)
    /// 'Save the  document to disk.
    /// document.Save("Pdf3DActivation.pdf")
    /// </code>
    /// </example> 
    public enum Pdf3DDeactivationState
    {
        /// <summary>
        /// Represents the initial state of the artwork before it has been used in any way.
        /// </summary>
        Uninstantiated,

        /// <summary>
        /// Represents that the state in which the artwork has been read and a run-time instance of 
        /// the artwork has been created. In this state, it can be rendered but script-driven 
        /// real-time modifications (that is, animations) are disabled.
        /// </summary>
        Instantiated,

        /// <summary>
        /// Represents that the artwork is instantiated, and it is being modified in real time to 
        /// achieve some animation effect. In the case of keyframe animation, the artwork is 
        /// live while it is playing and then reverts to an instantiated state when playing 
        /// completes or is stopped.
        /// </summary>
        Live
    }

    /// <summary>
    /// Specifies the available styles for applying light to 3D artwork. 
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document.
    /// PdfPage page = document.Pages.Add();
    /// //Create a new rectangle
    /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
    /// //Create a new Pdf3D Annotation.
    /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
    /// //Create a new Pdf3DLighting
    /// Pdf3DLighting lighting = new Pdf3DLighting();
    /// lighting.Style = Pdf3DLightingStyle.CAD;
    /// //Create a new Pdf3DView.
    /// Pdf3DView defaultView = new Pdf3DView();
    /// defaultView.LightingScheme=lighting
    /// annotation.Views.Add(defaultView);
    /// annotation.Views.Add(solidwireframeView);
    /// //Add the pdf Annotation
    /// page.Annotations.Add(annotation);
    /// //Save the  document to disk.
    /// document.Save("3DAnnotation.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document.
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create a new rectangle
    /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
    /// 'Create a new Pdf3DAnnotation.
    /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
    /// 'Create a new Pdf3DLighting
    /// Dim lighting As Pdf3DLighting = New Pdf3DLighting()
    /// lighting.Style = Pdf3DLightingStyle.CAD
    /// 'Create a new Pdf3DView
    /// Dim defaultView As Pdf3DView  = New Pdf3DView()
    /// defaultView.LightingScheme=lighting
    /// 'Add this annotation to a new page.
    /// annotation.Appearance.Normal.Draw(page, New PointF(annotation.Location.X, annotation.Location.Y))
    /// 'Save the  document to disk.
    /// document.Save("3DAnnotation.pdf")
    /// </code>
    /// </example> 
    public enum Pdf3DLightingStyle
    {
        /// <summary>
        /// The Lights as specified in the 3D artwork.
        /// </summary>
        Artwork,

        /// <summary>
        /// The lighting specified in the 3D artwork is ignored.
        /// </summary>
        None,

        /// <summary>
        /// Three blue-grey infinite lights.
        /// </summary>
        White,

        /// <summary>
        /// Three light-grey infinite lights.
        /// </summary>
        Day,

        /// <summary>
        /// One yellow, one aqua, and one blue infinite light.
        /// </summary>
        Night,

        /// <summary>
        /// Three grey infinite lights.
        /// </summary>
        Hard,

        /// <summary>
        /// One red, one green, and one blue infinite light.
        /// </summary>
        Primary,

        /// <summary>
        /// Three blue infinite lights.
        /// </summary>
        Blue,

        /// <summary>
        /// Three red infinite lights.
        /// </summary>
        Red,

        /// <summary>
        /// Six grey infinite lights aligned with the major axes.
        /// </summary>
        Cube,

        /// <summary>
        /// Three grey infinite lights and one light attached to the camera.
        /// </summary>
        CAD,

        /// <summary>
        /// Single infinite light attached to the camera.
        /// </summary>
        Headlamp
    }

    /// <summary>
    /// Specifies the available clipping style of the 3D annotation.
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document.
    /// PdfPage page = document.Pages.Add();
    /// //Create a new rectangle
    /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
    /// //Create a new Pdf3D Annotation.
    /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
    /// //Create a new Pdf3DLighting
    /// Pdf3DLighting lighting = new Pdf3DLighting();
    /// lighting.Style = Pdf3DLightingStyle.CAD;
    /// Create a new Pdf3DProjection
    /// Pdf3DProjection projection = new Pdf3DProjection();
    /// projection.ProjectionType = Pdf3DProjectionType.Perspective;
    /// projection.FieldOfView = 10;
    /// projection.ClipStyle = Pdf3DProjectionClipStyle.ExplicitNearFar;
    /// projection.NearClipDistance = 10;
    /// //Create a new Pdf3DView.
    /// Pdf3DView defaultView = new Pdf3DView();
    /// defaultView.Projection=projection
    /// annotation.Views.Add(defaultView);
    /// annotation.Views.Add(solidwireframeView);
    /// //Add the pdf Annotation
    /// page.Annotations.Add(annotation);
    /// //Save the  document to disk.
    /// document.Save("3DAnnotation.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document.
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create a new rectangle
    /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
    /// 'Create a new Pdf3D Annotation.
    /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
    /// Dim lighting As Pdf3DLighting = New Pdf3DLighting()
    /// lighting.Style = Pdf3DLightingStyle.CAD
    /// Dim projection1 As Pdf3DProjection = New Pdf3DProjection(Pdf3DProjectionType.Perspective)
    /// projection1.FieldOfView = 50
    /// projection1.ClipStyle = Pdf3DProjectionClipStyle.AutomaticNearFar
    /// 'Create a new Pdf3DView
    /// Dim defaultView As Pdf3DView  = New Pdf3DView()
    /// defaultView.Projection=projection
    /// 'Add this annotation to a new page.
    /// annotation.Appearance.Normal.Draw(page, New PointF(annotation.Location.X, annotation.Location.Y))
    /// 'Save the  document to disk.
    /// document.Save("3DAnnotation.pdf")
    /// </code>
    /// </example> 
    public enum Pdf3DProjectionClipStyle
    {
        /// <summary>
        /// Represents the Clipping style.
        /// </summary>
        ExplicitNearFar,

        /// <summary>
        /// Represents the Clipping style.
        /// </summary>
        AutomaticNearFar
    }

    /// <summary>
    /// Specifies the available Ortho projection scaling mode of the 3D annotation.
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document.
    /// PdfPage page = document.Pages.Add();
    /// //Create a new rectangle
    /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
    /// Pdf3DProjection projection = new Pdf3DProjection();
    /// projection.OrthoScaleMode = Pdf3DProjectionOrthoScaleMode.Width
    /// //Create a new Pdf3DView
    /// Pdf3DView view = new Pdf3DView();
    /// view.Projection = projection;
    /// annotation.Views.Add(view);
    /// //Add the pdf Annotation
    /// page.Annotations.Add(annotation);
    /// //Save the  document to disk.
    /// document.Save("3DAnnotation.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document.
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create a new Pdf3D Annotation.
    /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
    /// Dim projection As Pdf3DProjection = New Pdf3DProjection(Pdf3DProjectionType.Perspective)
    /// projection.OrthoScaleMode = Pdf3DProjectionOrthoScaleMode.Width
    /// 'Create a new Pdf3DView
    /// Dim  view As Pdf3DView = New Pdf3DView()
    /// view.Projection = projection
    /// annotation.Views.Add(view)
    /// 'Add this annotation to a new page.
    /// annotation.Appearance.Normal.Draw(page, New PointF(annotation.Location.X, annotation.Location.Y))
    /// 'Save the  document to disk.
    /// document.Save("3DAnnotation.pdf")
    /// </code>
    /// </example> 
    public enum Pdf3DProjectionOrthoScaleMode
    {
        /// <summary>
        /// Scale to fit the width of the annotation.
        /// </summary>
        Width,

        /// <summary>
        /// Scale to fit the height of the annotation.
        /// </summary>
        Height,

        /// <summary>
        /// Scale to fit the lesser of width or height of the annotation.
        /// </summary>
        Min,

        /// <summary>
        /// Scale to fit the greater of width or height of the annotation.
        /// </summary>
        Max,

        /// <summary>
        /// No scaling should occur due to binding.
        /// </summary>
        Absolute
    }

    /// <summary>
    /// Specifies the available projection type of the 3D annotation.
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document.
    /// PdfPage page = document.Pages.Add();
    /// //Create a new rectangle
    /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
    /// //Create a new Pdf3D Annotation.
    /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
    /// Pdf3DLighting lighting = new Pdf3DLighting();
    /// lighting.Style = Pdf3DLightingStyle.CAD;
    /// //Create a new Pdf3DProjection.
    /// Pdf3DProjection projection = new Pdf3DProjection();
    /// projection.ProjectionType = Pdf3DProjectionType.Perspective;
    /// projection.FieldOfView = 10;
    /// projection.ClipStyle = Pdf3DProjectionClipStyle.ExplicitNearFar;
    /// projection.NearClipDistance = 10;
    /// Pdf3DView  Pdf3DView  = new Pdf3DView();
    /// defaultView.Projection=projection
    /// annotation.Views.Add(defaultView);
    /// annotation.Views.Add(solidwireframeView);
    /// //Add the pdf Annotation
    /// page.Annotations.Add(annotation);
    /// //Save the  document to disk.
    /// document.Save("3DAnnotation.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document.
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create a new rectangle
    /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
    /// 'Create a new Pdf3D Annotation.
    /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
    /// Dim lighting As Pdf3DLighting = New Pdf3DLighting()
    /// lighting.Style = Pdf3DLightingStyle.CAD
    /// 'Create a new Pdf3DProjection
    /// Dim projection As Pdf3DProjection = New Pdf3DProjection(Pdf3DProjectionType.Perspective)
    /// projection.FieldOfView = 50
    /// projection.ProjectionType = Pdf3DProjectionType.Perspective
    /// Dim defaultView As Pdf3DView  = New Pdf3DView()
    /// defaultView.Projection=projection
    /// 'Add this annotation to a new page.
    /// annotation.Appearance.Normal.Draw(page, New PointF(annotation.Location.X, annotation.Location.Y))
    /// 'Save the  document to disk.
    /// document.Save("3DAnnotation.pdf")
    /// </code>
    /// </example> 
    public enum Pdf3DProjectionType
    {
        /// <summary>
        /// Represents Orthographic projection
        /// </summary>
        Orthographic = 0,

        /// <summary>
        /// Represents Perspective projection.
        /// </summary>
        Perspective = 1,
    }

    /// <summary>
    /// Specifies the available rendering style of the 3D artwork. 
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document.
    /// PdfPage page = document.Pages.Add();
    /// //Create a new rectangle
    /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
    /// //Create a new Pdf3D Annotation.
    /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
    /// //Create a new Pdf3DLighting
    /// Pdf3DLighting lighting = new Pdf3DLighting();
    /// lighting.Style = Pdf3DLightingStyle.CAD;
    /// Pdf3DProjection projection = new Pdf3DProjection();
    /// projection.ProjectionType = Pdf3DProjectionType.Perspective;
    /// projection.FieldOfView = 10;
    /// projection.ClipStyle = Pdf3DProjectionClipStyle.ExplicitNearFar;
    /// projection.NearClipDistance = 10;
    /// //Create a new Pdf3DRendermode.
    /// Pdf3DRendermode rendermode = new Pdf3DRendermode();
    /// rendermode.Style = Pdf3DRenderStyle.Solid;
    /// //Create a new Pdf3DView
    /// Pdf3DView  defaultView  = new Pdf3DView()
    /// defaultView.Projection=projection1;
    /// annotation.Views.Add(defaultView);
    /// annotation.Views.Add(solidwireframeView);
    /// //Adds the pdf Annotation
    /// page.Annotations.Add(annotation);
    /// //Save the  document to disk.
    /// document.Save("3DAnnotation.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document.
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create a new rectangle
    /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
    /// 'Create a new Pdf3D Annotation.
    /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
    /// Dim lighting As Pdf3DLighting = New Pdf3DLighting()
    /// lighting.Style = Pdf3DLightingStyle.CAD
    /// Dim projection1 As Pdf3DProjection = New Pdf3DProjection(Pdf3DProjectionType.Perspective)
    /// projection1.FieldOfView = 50
    /// projection1.ClipStyle = Pdf3DProjectionClipStyle.AutomaticNearFar
    /// 'Create a new Pdf3DRendermode
    /// Dim rendermode As Pdf3DRendermode = New Pdf3DRendermode()
    /// rendermode.Style = Pdf3DRenderStyle.Solid
    /// 'Create a new Pdf3DView
    /// Dim defaultView As Pdf3DView = New Pdf3DView()
    /// defaultView.Projection=projection1
    /// 'Add this annotation to a new page.
    /// annotation.Appearance.Normal.Draw(page, New PointF(annotation.Location.X, annotation.Location.Y))
    /// 'Save the  document to disk.
    /// document.Save("3DAnnotation.pdf")
    /// </code>
    /// </example> 
    public enum Pdf3DRenderStyle
    {
        /// <summary>
        /// Displays textured and lit geometric shapes. In the case of artwork 
        /// that conforms to the Universal 3D File Format specification, these 
        /// shapes are triangles.
        /// </summary>
        Solid,

        /// <summary>
        /// Displays textured and lit geometric shapes (triangles) with single 
        /// color edges on top of them.
        /// </summary>
        SolidWireframe,

        /// <summary>
        /// Displays textured and lit geometric shapes (triangles) with an added 
        /// level of transparency.
        /// </summary>
        Transparent,

        /// <summary>
        /// Displays textured and lit geometric shapes (triangles) with an added 
        /// level of transparency, with single color opaque edges on top of it.
        /// </summary>
        TransparentWireframe,

        /// <summary>
        /// Displays the bounding box edges of each node, aligned with the axes 
        /// of the local coordinate space for that node.
        /// </summary>
        BoundingBox,

        /// <summary>
        /// Displays bounding boxes faces of each node, aligned with the axes of 
        /// the local coordinate space for that node, with an added level of transparency.
        /// </summary>
        TransparentBoundingBox,

        /// <summary>
        /// Displays bounding boxes edges and faces of each node, aligned with the axes of 
        /// the local coordinate space for that node, with an added level of transparency.
        /// </summary>
        TransparentBoundingBoxOutline,

        /// <summary>
        /// Displays only edges in a single color.
        /// </summary>
        Wireframe,

        /// <summary>
        /// Displays only edges, though interpolates their color between their two vertices 
        /// and applies lighting.
        /// </summary>
        ShadedWireframe,

        /// <summary>
        /// Displays edges in a single color, though removes back-facing and obscured edges.
        /// </summary>
        HiddenWireframe,

        /// <summary>
        /// Displays only vertices in a single color.
        /// </summary>
        Vertices,

        /// <summary>
        /// Displays only vertices, though uses their vertex color and applies lighting.
        /// </summary>
        ShadedVertices,

        /// <summary>
        /// Displays silhouette edges with surfaces, removes obscured lines.
        /// </summary>
        Illustration,

        /// <summary>
        /// Displays silhouette edges with lit and textured surfaces, removes obscured lines.
        /// </summary>
        SolidOutline,

        /// <summary>
        /// Displays silhouette edges with lit and textured surfaces and an additional emissive 
        /// term to remove poorly lit areas of the artwork.
        /// </summary>
        ShadedIllustration
    }

    /// <summary>
    /// Specifies the available animation style for rendering the 3D artwork. 
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// //Create a new Pdf3DAnimation
    /// Pdf3DAnimation animation = new Pdf3DAnimation(PDF3DAnimationType.Linear);
    /// animation.Type = PDF3DAnimationType.Linear;
    /// </code>
    /// <code lang="VB">
    /// 'Create a new Pdf3DAnimation
    /// Dim animation As Pdf3DAnimation  = New Pdf3DAnimation(PDF3DAnimationType.Linear)
    /// animation.Type = PDF3DAnimationType.Linear
    /// </code>
    /// </example> 
    public enum PDF3DAnimationType
    {
        /// <summary>
        /// Represents that the Keyframe animations should not be driven directly by 
        /// the viewer application. This value is used by documents that are intended 
        /// to drive animations through an alternate means, such as JavaScript.
        /// </summary>
        None,

        /// <summary>
        /// Represents that the Keyframe animations are driven linearly from beginning to end. 
        /// This animation style results in a repetitive playthrough of the animation, 
        /// such as in a walking motion.
        /// </summary>
        Linear,

        /// <summary>
        /// Represents that the Keyframe animations should oscillate along their time range. 
        /// This animation style results in a back-and-forth playing of the animation, 
        /// such as exploding or collapsing parts.
        /// </summary>
        Oscillating
    }
}
