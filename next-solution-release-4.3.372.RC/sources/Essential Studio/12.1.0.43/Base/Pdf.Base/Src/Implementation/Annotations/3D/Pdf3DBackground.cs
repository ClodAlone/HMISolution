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
    /// Represents the background appearance for 3D artwork. 
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
    /// PdfColor color = new PdfColor(Color.Silver);
    /// //Creates a new Pdf3DBackground
    /// Pdf3DBackground background = new Pdf3DBackground();
    /// background.ApplyToEntireAnnotation = true;
    /// background.Color = color;
    /// Pdf3DView view = new Pdf3DView();
    /// view.Background = background;
    /// annotation.Views.Add(view);
    /// page.Annoatations.Add(annotation);
    /// //Save the document to disk.
    /// document.Save("Pdf3DBackground.pdf");
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
    /// Dim color As PdfColor = New PdfColor(Color.Silver)
    /// 'Creates a new Pdf3DBackground
    /// Dim background As Pdf3DBackground = New Pdf3DBackground()
    /// background.ApplyToEntireAnnotation = True
    /// background.Color = color
    /// Dim view As Pdf3DView = New Pdf3DView()
    /// view.Background = background;
    /// annotation.Views.Add(defaultView)
    /// 'Draw a annotation into the new page.
    /// page.Annoatations.Add(annotation)
    /// 'Save the document to disk.
    /// document.Save("Pdf3DBackground.pdf")
    /// </code>
    /// </example> 
    public class Pdf3DBackground : IPdfWrapper
    {
        #region Constants
        /// <summary>
        /// Max value of color channel.
        /// </summary>
        private const float MaxColourChannelValue = 255.0f;
        #endregion

        #region Fields
        private PdfColor m_backgroundColor;
        private bool m_applyEntire;

        /// <summary>
        /// Internal variable to store dictionary.
        /// </summary>
        private PdfDictionary m_dictionary = new PdfDictionary();

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the background color.
        /// </summary>
        /// <value>The <see cref="PdfColor"/> object specifying the background color for the 3D artwork. </value>
        /// <example>
        /// <code lang="C#">
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new Pdf3DAnnotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// PdfColor color = new PdfColor(Color.Silver);
        /// //Create a new Pdf3DBackground.
        /// Pdf3DBackground background = new Pdf3DBackground();
        /// background.Color = color;
        /// Pdf3DView view = new Pdf3DView();
        /// view.Background = background;
        /// annotation.Views.Add(view);
        /// page.Annoatations.Add(annotation);
        /// //Save the document to disk.
        /// document.Save("Pdf3DBackground.pdf");
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
        /// Dim color As PdfColor = New PdfColor(Color.Silver)
        /// 'Create a new Pdf3DBackground.
        /// Dim background As Pdf3DBackground = New Pdf3DBackground()
        /// background.Color = color
        /// Dim  view As Pdf3DView = New Pdf3DView()
        /// view.Background = background;
        /// annotation.Views.Add(defaultView)
        /// 'Draw a annotation into the new page.
        /// page.Annoatations.Add(annotation)
        /// 'Save the document to disk.
        /// document.Save("Pdf3DBackground.pdf")
        /// </code>
        /// </example> 
        public PdfColor Color
        {
            get
            {
                return this.m_backgroundColor;
            }

            set
            {
                this.m_backgroundColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating how the background is applied. 
        /// </summary>
        /// <value>True if the background is applied to entire annotation, false if the background is applied to annotation's 3D view box only.</value>      
        /// <example>
        /// <code lang = "C#">
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new Pdf3DAnnotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// PdfColor color = new PdfColor(Color.Silver);
        /// //Create a new Pdf3DBackground.
        /// Pdf3DBackground background = new Pdf3DBackground();
        /// background.ApplyToEntireAnnotation = true;
        /// Pdf3DView view = new Pdf3DView();
        /// view.Background = background;
        /// annotation.Views.Add(view);
        /// page.Annoatations.Add(annotation);
        /// //Save the document to disk.
        /// document.Save("Pdf3DBackground.pdf");
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
        /// Dim color As PdfColor = New PdfColor(Color.Silver)
        /// 'Create a new Pdf3DBackground
        /// Dim background As Pdf3DBackground = New Pdf3DBackground()
        /// background.ApplyToEntireAnnotation = True
        /// Dim  view As Pdf3DView = New Pdf3DView()
        /// view.Background = background;
        /// annotation.Views.Add(defaultView)
        /// 'Draw a annotation into the new page.
        /// page.Annoatations.Add(annotation);
        /// 'Save the document to disk.
        /// document.Save("Pdf3DBackground.pdf")
        /// </code>
        /// </example> 
        public bool ApplyToEntireAnnotation
        {
            get
            {
                return this.m_applyEntire;
            }

            set
            {
                this.m_applyEntire = value;
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
        /// Initializes a new instance of the <see cref="Pdf3DBackground"/> class.
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
        /// PdfColor color = new PdfColor(Color.Silver);
        /// //Creates a new Pdf3DBackground
        /// Pdf3DBackground background = new Pdf3DBackground();
        /// background.ApplyToEntireAnnotation = true;
        /// background.Color = color;
        /// Pdf3DView view = new Pdf3DView();
        /// view.Background = background;
        /// annotation.Views.Add(view);
        /// page.Annoatations.Add(annotation);
        /// //Save the document to disk.
        /// document.Save("Pdf3DBackground.pdf");
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
        /// Dim color As PdfColor = New PdfColor(Color.Silver)
        /// 'Creates a new Pdf3DBackground
        /// Dim background As Pdf3DBackground = New Pdf3DBackground()
        /// background.ApplyToEntireAnnotation = True
        /// background.Color = color
        /// Dim view As Pdf3DView = New Pdf3DView()
        /// view.Background = background;
        /// annotation.Views.Add(defaultView)
        /// 'Draw a annotation into the new page.
        /// page.Annoatations.Add(annotation)
        /// 'Save the document to disk.
        /// document.Save("Pdf3DBackground.pdf")
        /// </code>
        /// </example> 
        public Pdf3DBackground()
        {
            this.Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Pdf3DBackground"/> class.        
        /// </summary>
        /// <param name="color">The <see cref="PdfColor"/> object specifying the background color for the 3D artwork.</param>
        /// <example>
        /// <code lang = "C#">
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new Pdf3DAnnotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// PdfColor color = new PdfColor(Color.Silver);
        /// //Create a new Pdf3DBackground.
        /// Pdf3DBackground background = new Pdf3DBackground(color);
        /// background.ApplyToEntireAnnotation = true;
        /// Pdf3DView view = new Pdf3DView();
        /// view.Background = background;
        /// annotation.Views.Add(view);
        /// page.Annoatations.Add(annotation);
        /// //Save the document to disk.
        /// document.Save("Pdf3DBackground.pdf");
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
        /// Dim color As PdfColor = New PdfColor(Color.Silver)
        /// 'Create a new Pdf3DBackground.
        /// Dim background As Pdf3DBackground = New Pdf3DBackground(color)
        /// background.ApplyToEntireAnnotation = True
        /// Dim  view As Pdf3DView = New Pdf3DView()
        /// view.Background = background;
        /// annotation.Views.Add(defaultView)
        /// 'Draw a annotation into the new page.
        /// page.Annoatations.Add(annotation)
        /// 'Save the document to disk.
        /// document.Save("Pdf3DBackground.pdf")
        /// </code>
        /// </example> 
        public Pdf3DBackground(PdfColor color)
            : this()
        {
            this.m_backgroundColor = color;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Initializes annotation object.
        /// </summary>
        protected virtual void Initialize()
        {
            this.m_dictionary.BeginSave += new SavePdfPrimitiveEventHandler(this.Dictionary_BeginSave);
            this.m_dictionary.SetProperty(DictionaryProperties.Type, new PdfName(DictionaryProperties._3DBG));
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
            this.Dictionary[DictionaryProperties.Subtype] = new PdfName(DictionaryProperties.SC);
            this.Dictionary.SetProperty(DictionaryProperties.CS, new PdfName("DeviceRGB"));
            this.Dictionary.SetProperty(DictionaryProperties.EA, new PdfBoolean(this.m_applyEntire));

            if (this.m_backgroundColor != null)
            {
                PdfArray bgcolor = new PdfArray();
                bgcolor.Insert(0, new PdfNumber(this.m_backgroundColor.R / 255f));
                bgcolor.Insert(1, new PdfNumber(this.m_backgroundColor.G / 255f));
                bgcolor.Insert(2, new PdfNumber(this.m_backgroundColor.B / 255f));
                this.Dictionary[DictionaryProperties.C] = new PdfArray(bgcolor);
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
