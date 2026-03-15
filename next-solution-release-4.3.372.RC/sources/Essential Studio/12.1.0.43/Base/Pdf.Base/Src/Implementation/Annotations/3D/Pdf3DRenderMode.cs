#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
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
    /// Represents the rendering mode of the 3D artwork. 
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document.
    /// PdfPage page = document.Pages.Add();
    /// //Create a new Pdf3D Annotation.
    /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
    /// //Creates a new Pdf3DRendermode.
    /// Pdf3DRendermode renderMode = new Pdf3DRendermode();
    /// renderMode.Style = Pdf3DRenderStyle.Solid;
    /// renderMode.AuxilaryColor = new PdfColor(Color.Green);
    /// renderMode.FaceColor = new PdfColor(Color.Black);
    /// //Creates a new Pdf3DView
    /// Pdf3DView view = new Pdf3DView();
    /// view.RenderMode=renderMode;
    /// annotation.Views.Add(view);
    /// page.Annoatations.Add(annotation);
    /// //Save the  document to disk.
    /// document.Save("PDF3DRendermode.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document.
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create a new Pdf3DAnnotation.
    /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
    /// 'Create a new Pdf3DRendermode
    /// Dim renderMode As Pdf3DRendermode  = New Pdf3DRendermode()
    /// renderMode.Style = Pdf3DRenderStyle.Solid
    /// renderMode.AuxilaryColor = New PdfColor(Color.Green)
    /// renderMode.FaceColor = New PdfColor(Color.Black)
    /// 'Create a new Pdf3DView
    /// Dim  view As Pdf3DView = New Pdf3DView()
    /// view.RenderMode=renderMode
    /// annotation.Views.Add(view)
    /// 'Draw a annotation into the new page.
    /// page.Annoatations.Add(annotation)
    /// 'Save the  document to disk.
    /// document.Save("PDF3DRendermode.pdf")
    /// </code>
    /// </example> 
    public class Pdf3DRendermode : IPdfWrapper
    {
        #region Fields
        /// <summary>
        /// Internal variable to store style.
        /// </summary>
        private Pdf3DRenderStyle m_style;

        /// <summary>
        /// Internal variable to store face Color.
        /// </summary>
        private PdfColor m_faceColor;

        /// <summary>
        /// Internal variable to store auxilary Color.
        /// </summary>
        private PdfColor m_auxilaryColor;

        /// <summary>
        /// Internal variable to store opacity.
        /// </summary>
        private float m_opacity;

        /// <summary>
        /// Internal variable to store crease Value.
        /// </summary>
        private float m_creaseValue;

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
        /// //Create a new Pdf3D Annotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// //Create a new Pdf3DRendermode
        /// Pdf3DRendermode renderMode = new Pdf3DRendermode();
        /// renderMode.Style = Pdf3DRenderStyle.Solid;
        /// //Creates a new Pdf3DView
        /// Pdf3DView view = new Pdf3DView();
        /// view.RenderMode=renderMode;
        /// annotation.Views.Add(view);
        /// page.Annoatations.Add(annotation);
        /// //Save the  document to disk.
        /// document.Save("PDF3DRendermode.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new Pdf3DAnnotation.
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Create a new Pdf3DRendermode
        /// Dim renderMode As Pdf3DRendermode  = New Pdf3DRendermode()
        /// renderMode.Style = Pdf3DRenderStyle.Solid
        /// 'Create a new Pdf3DView
        /// Dim  view As Pdf3DView = New Pdf3DView()
        /// view.RenderMode=renderMode
        /// annotation.Views.Add(view)
        /// 'Draw a annotation into the new page.
        /// page.Annoatations.Add(annotation)
        /// 'Save the  document to disk.
        /// document.Save("PDF3DRendermode.pdf")
        /// </code>
        /// </example> 
        public Pdf3DRenderStyle Style
        {
            get
            {
                return this.m_style;
            }

            set
            {
                this.m_style = value;
            }
        }

        /// <summary>
        /// Gets or sets the Auxiliary color.
        /// </summary> 
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new Pdf3D Annotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// //Creates a new Pdf3DRendermode.
        /// Pdf3DRendermode renderMode = new Pdf3DRendermode();
        /// renderMode.AuxilaryColor = new PdfColor(Color.Green);
        /// //Creates a new Pdf3DView
        /// Pdf3DView view = new Pdf3DView();
        /// view.RenderMode=renderMode;
        /// annotation.Views.Add(view);
        /// page.Annoatations.Add(annotation);
        /// //Save the  document to disk.
        /// document.Save("PDF3DRendermode.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new Pdf3DAnnotation.
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Create a new Pdf3DRendermode
        /// Dim renderMode As Pdf3DRendermode  = New Pdf3DRendermode()
        /// renderMode.AuxilaryColor = New PdfColor(Color.Green)
        /// 'Create a new Pdf3DView
        /// Dim  view As Pdf3DView = New Pdf3DView()
        /// view.RenderMode=renderMode
        /// annotation.Views.Add(view)
        /// 'Draw a annotation into the new page.
        /// page.Annoatations.Add(annotation)
        /// 'Save the  document to disk.
        /// document.Save("PDF3DRendermode.pdf")
        /// </code>
        /// </example> 
        public PdfColor AuxilaryColor
        {
            get
            {
                return this.m_auxilaryColor;
            }

            set
            {
                this.m_auxilaryColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the Face color.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new Pdf3D Annotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// //Creates a new Pdf3DRendermode.
        /// Pdf3DRendermode renderMode = new Pdf3DRendermode();
        /// renderMode.FaceColor = new PdfColor(Color.Black);
        /// //Creates a new Pdf3DView
        /// Pdf3DView view = new Pdf3DView();
        /// view.RenderMode=renderMode;
        /// annotation.Views.Add(view);
        /// page.Annoatations.Add(annotation);
        /// //Save the  document to disk.
        /// document.Save("PDF3DRendermode.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new Pdf3DAnnotation.
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Create a new Pdf3DRendermode
        /// Dim renderMode As Pdf3DRendermode  = New Pdf3DRendermode()
        /// renderMode.FaceColor = New PdfColor(Color.Black)
        /// 'Create a new Pdf3DView
        /// Dim  view As Pdf3DView = New Pdf3DView()
        /// view.RenderMode=renderMode
        /// annotation.Views.Add(view)
        /// 'Draw a annotation into the new page.
        /// page.Annoatations.Add(annotation)
        /// 'Save the  document to disk.
        /// document.Save("PDF3DRendermode.pdf")
        /// </code>
        /// </example> 
        public PdfColor FaceColor
        {
            get
            {
                return this.m_faceColor;
            }

            set
            {
                this.m_faceColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the crease value. 
        /// <remarks>The crease value is specified in degrees, from 0 to 360.</remarks>
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new Pdf3D Annotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// //Creates a new Pdf3DRendermode.
        /// Pdf3DRendermode renderMode = new Pdf3DRendermode();
        /// renderMode.CreaseValue =10f;
        /// //Creates a new Pdf3DView
        /// Pdf3DView view = new Pdf3DView();
        /// view.RenderMode=renderMode;
        /// annotation.Views.Add(view);
        /// page.Annoatations.Add(annotation);
        /// //Save the  document to disk.
        /// document.Save("PDF3DRendermode.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new Pdf3DAnnotation.
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Create a new Pdf3DRendermode
        /// Dim renderMode As Pdf3DRendermode  = New Pdf3DRendermode()
        /// renderMode.CreaseValue =10f
        /// 'Create a new Pdf3DView
        /// Dim  view As Pdf3DView = New Pdf3DView()
        /// view.RenderMode=renderMode
        /// annotation.Views.Add(view)
        /// 'Draw a annotation into the new page.
        /// page.Annoatations.Add(annotation)
        /// 'Save the  document to disk.
        /// document.Save("PDF3DRendermode.pdf")
        /// </code>
        /// </example> 
        public float CreaseValue
        {
            get
            {
                return this.m_creaseValue;
            }

            set
            {
                this.m_creaseValue = value;
            }
        }

        /// <summary>
        /// Gets or sets the rendering opacity. 
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new Pdf3D Annotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// //Creates a new Pdf3DRendermode.
        /// Pdf3DRendermode renderMode = new Pdf3DRendermode();
        /// renderMode.Opacity = 100f;
        /// //Creates a new Pdf3DView
        /// Pdf3DView view = new Pdf3DView();
        /// view.RenderMode=renderMode;
        /// annotation.Views.Add(view);
        /// page.Annoatations.Add(annotation);
        /// //Save the  document to disk.
        /// document.Save("PDF3DRendermode.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new Pdf3DAnnotation.
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Create a new Pdf3DRendermode
        /// Dim renderMode As Pdf3DRendermode  = New Pdf3DRendermode()
        /// renderMode.Opacity = 100f
        /// 'Create a new Pdf3DView
        /// Dim  view As Pdf3DView = New Pdf3DView()
        /// view.RenderMode=renderMode
        /// annotation.Views.Add(view)
        /// 'Draw a annotation into the new page.
        /// page.Annoatations.Add(annotation)
        /// 'Save the  document to disk.
        /// document.Save("PDF3DRendermode.pdf")
        /// </code>
        /// </example> 
        /// <remarks>The opacity is given in percents, 100 is full opacity, 0 is no opacity.</remarks>
        public float Opacity
        {
            get
            {
                return this.m_opacity;
            }

            set
            {
                this.m_opacity = value;
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
        /// Initializes a new instance of the <see cref="Pdf3DRendermode"/> class.
        /// </summary>
        public Pdf3DRendermode()
        {
            this.Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Pdf3DRendermode"/> class.
        /// </summary>
        /// <param name="style">The <see cref="Pdf3DRenderStyle"/> object specifies the rendering style of the 3D artwork.</param>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new Pdf3D Annotation.
        /// //Create a new Pdf3D Annotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// //Create a new Pdf3DRendermode
        ///  Pdf3DRendermode renderMode = new Pdf3DRendermode(Pdf3DRenderStyle.Sloid);
        /// renderMode.Style = Pdf3DRenderStyle.Solid;
        /// renderMode.AuxilaryColor = new PdfColor(Color.Green);
        /// renderMode.FaceColor = new PdfColor(Color.Black);
        /// //Create a new Pdf3DView
        /// Pdf3DView view = new Pdf3DView();
        /// view.RenderMode=renderMode;
        /// annotation.Views.Add(view);
        /// page.Annoatations.Add(annotation);
        /// //Save the  document to disk.
        /// document.Save("PDF3DRendermode.pdf");
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
        /// 'Create a new Pdf3DRendermode
        /// Dim renderMode As Pdf3DRendermode  = New Pdf3DRendermode(Pdf3DRenderStyle.Sloid)
        /// renderMode.Style = Pdf3DRenderStyle.Solid
        /// renderMode.AuxilaryColor = New PdfColor(Color.Green)
        /// renderMode.FaceColor = New PdfColor(Color.Black)
        /// 'Create a new Pdf3DView
        /// Dim  view As Pdf3DView = New Pdf3DView()
        /// view.RenderMode=renderMode;
        /// annotation.Views.Add(view)
        /// 'Draw a annotation into the new page.
        /// page.Annoatations.Add(annotation)
        /// 'Save the  document to disk.
        /// document.Save("PDF3DRendermode.pdf")
        /// </code>
        /// </example> 
        public Pdf3DRendermode(Pdf3DRenderStyle style)
            : this()
        {
            this.m_style = style;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Initializes annotation object.
        /// </summary>
        protected virtual void Initialize()
        {
            this.m_dictionary.BeginSave += new SavePdfPrimitiveEventHandler(this.Dictionary_BeginSave);
            this.m_dictionary.SetProperty(DictionaryProperties.Type, new PdfName(DictionaryProperties._3DRenderMode));
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
            this.Dictionary[DictionaryProperties.Subtype] = new PdfName(this.m_style);

            if (this.m_auxilaryColor != null)
            {
                PdfArray accolor = new PdfArray();
                accolor.Insert(0, new PdfName("DeviceRGB"));
                accolor.Insert(1, new PdfNumber(this.m_auxilaryColor.R / 255f));
                accolor.Insert(2, new PdfNumber(this.m_auxilaryColor.G / 255f));
                accolor.Insert(3, new PdfNumber(this.m_auxilaryColor.B / 255f));

                this.Dictionary[DictionaryProperties.AC] = new PdfArray(accolor);
            }

            if (this.m_faceColor != null)
            {
                PdfArray facecolor = new PdfArray();
                facecolor.Insert(0, new PdfName("DeviceRGB"));
                facecolor.Insert(1, new PdfNumber(this.m_faceColor.R / 255f));
                facecolor.Insert(2, new PdfNumber(this.m_faceColor.G / 255f));
                facecolor.Insert(3, new PdfNumber(this.m_faceColor.B / 255f));

                this.Dictionary[DictionaryProperties.FC] = new PdfArray(facecolor);
            }

            this.Dictionary.SetProperty(DictionaryProperties.O, new PdfNumber(this.m_opacity));
            this.Dictionary.SetProperty(DictionaryProperties.CV, new PdfNumber(this.m_creaseValue));
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
