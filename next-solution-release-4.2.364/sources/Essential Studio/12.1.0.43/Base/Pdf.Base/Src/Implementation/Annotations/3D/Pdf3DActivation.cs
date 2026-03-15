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
    /// Represents the activation states for the 3D annotation. 
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
    /// //Create a new Pdf3DActivation.
    /// Pdf3DActivation activation = new Pdf3DActivation();
    /// activation.ActivationMode = Pdf3DActivationMode.ExplicitActivation;
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
    /// 'Create a new Pdf3DAnnotation.
    /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
    /// 'Create a new Pdf3DActivation.
    /// Dim activation As Pdf3DActivation = New Pdf3DActivation()
    /// activation.ActivationMode = Pdf3DActivationMode.PageVisible
    /// activation.ShowToolbar = True
    /// 'Draw a annotation into the new page.
    /// page.Annoatations.Add(annotation)
    /// 'Save the  document to disk.
    /// document.Save("Pdf3DActivation.pdf")
    /// </code>
    /// </example> 
    public class Pdf3DActivation : IPdfWrapper
    {
        #region Fields
        /// <summary>
        /// Internal variable to store the activation mode.
        /// </summary>
        private Pdf3DActivationMode m_activationMode;

        /// <summary>
        /// Internal variable to store the activation state.
        /// </summary>
        private Pdf3DActivationState m_activationState;

        /// <summary>
        /// Internal variable to store the deactivation mode.
        /// </summary>
        private Pdf3DDeactivationMode m_deactivationMode;

        /// <summary>
        /// Internal variable to store the deactivation state.
        /// </summary>
        private Pdf3DDeactivationState m_deactivationState;

        /// <summary>
        /// Internal variable reprsents whether to show toolbar or not.
        /// </summary>
        private bool m_showToolbar = true;

        /// <summary>
        /// Internal variable reprsents whether to show UI or not.
        /// </summary>
        private bool m_showUI;

        /// <summary>
        /// Internal variable to store dictionary.
        /// </summary>
        private PdfDictionary m_dictionary = new PdfDictionary();

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the activation mode for the annotation.
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
        /// //Create a new Pdf3DActivation.
        /// Pdf3DActivation activation = new Pdf3DActivation();
        /// activation.ActivationMode = Pdf3DActivationMode.PageOpen;
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
        /// 'Create a new Pdf3DAnnotation.
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// //Create a new Pdf3DActivation.
        /// Dim activation As Pdf3DActivation = New Pdf3DActivation()
        /// activation.ActivationMode = Pdf3DActivationMode.PageOpen
        /// activation.ShowToolbar = True
        /// annotation.Activation = activation
        /// 'Draw a annotation into the new page
        /// page.Annoatations.Add(annotation)
        /// 'Save the  document to disk.
        /// document.Save("Pdf3DActivation.pdf")
        /// </code>
        /// </example> 
        public Pdf3DActivationMode ActivationMode
        {
            get
            {
                return this.m_activationMode;
            }

            set
            {
                this.m_activationMode = value;
            }
        }

        /// <summary>
        /// Gets or sets the deactivation mode for the annotation.
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
        /// //Create a new Pdf3DActivation.
        /// Pdf3DActivation activation = new Pdf3DActivation();
        /// //Sets the DeactivationState.
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
        /// 'Create a new Pdf3DAnnotation.
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Create a new Pdf3DActivation
        /// Dim activation As Pdf3DActivation = New Pdf3DActivation()
        /// activation.DeactivationState = Pdf3DDeactivationState.Live
        /// annotation.Activation = activation
        /// 'Draw a annotation into the new page.
        /// page.Annoatations.Add(annotation)
        /// 'Save the  document to disk.
        /// document.Save("Pdf3DActivation.pdf")
        /// </code>
        /// </example> 
        public Pdf3DDeactivationMode DeactivationMode
        {
            get
            {
                return this.m_deactivationMode;
            }

            set
            {
                this.m_deactivationMode = value;
            }
        }

        /// <summary>
        /// Gets or sets the activation state for the annotation.
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
        /// //Create a new Pdf3DActivation.
        /// Pdf3DActivation activation = new Pdf3DActivation();
        /// aannotation.ActivationState = Pdf3DActivationState.Instantiated;
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
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Create a new Pdf3DActivation
        /// Dim activation As Pdf3DActivation = New Pdf3DActivation()
        /// annotation.ActivationState = Pdf3DActivationState.Instantiated
        /// 'Draw a annotation into the new page.
        /// page.Annoatations.Add(annotation)
        /// 'Save the  document to disk.
        /// document.Save("Pdf3DActivation.pdf")
        /// </code>
        /// </example> 
        public Pdf3DActivationState ActivationState
        {
            get
            {
                return this.m_activationState;
            }

            set
            {
                this.m_activationState = value;
            }
        }

        /// <summary>
        /// Gets or sets the deactivation state for the annotation.
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
        /// //Create a new Pdf3DActivation
        /// Pdf3DActivation activation = new Pdf3DActivation();
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
        /// 'Create a new Pdf3DAnnotation.
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Create a new Pdf3DActivation
        /// Dim activation As Pdf3DActivation = New Pdf3DActivation()
        /// activation.DeactivationState = Pdf3DDeactivationState.Live
        /// annotation.Activation = activation
        /// 'Draw a annotation into the new page.
        /// page.Annoatations.Add(annotation)
        /// 'Save the  document to disk.
        /// document.Save("Pdf3DActivation.pdf")
        /// </code>
        /// </example> 
        public Pdf3DDeactivationState DeactivationState
        {
            get
            {
                return this.m_deactivationState;
            }

            set
            {
                this.m_deactivationState = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the toolbar should be displayed when the annotation is activated or not. 
        /// </summary>
        /// <value>If true, a toolbar should be displayed by default when the annotation is activated and given focus. If false, a toolbar should not be displayed by default. </value>
        /// <example>
        /// <code lang = "C#">
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new Pdf3D Annotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// //Create a new Pdf3DActivation
        /// Pdf3DActivation activation = new Pdf3DActivation();
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
        /// 'Create a new Pdf3DAnnotation.
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Create a new Pdf3DActivation
        /// Dim activation As Pdf3DActivation = New Pdf3DActivation()
        /// activation.ShowToolbar = True
        /// annotation.Activation = activation
        /// 'Draw a annotation into the new page.
        /// page.Annoatations.Add(annotation)
        /// 'Save the  document to disk.
        /// document.Save("Pdf3DActivation.pdf")
        /// </code>
        /// </example> 
        public bool ShowToolbar
        {
            get
            {
                return this.m_showToolbar;
            }

            set
            {
                this.m_showToolbar = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the UI for managing the 3D artwork should be displayed when the annotation is activated. 
        /// </summary>
        /// <value>If true, the user interface should be made visible when the annotation is activated. If false, the user interface should not be made visible by default.</value>
        /// <example>
        /// <code lang = "C#">
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new Pdf3D Annotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// //Create a new Pdf3DActivation
        /// Pdf3DActivation activation = new Pdf3DActivation();
        /// activation.ShowUI = false;
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
        /// 'Create a new Pdf3DAnnotation.
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Create a new Pdf3DActivation
        /// Dim activation As Pdf3DActivation = New Pdf3DActivation()
        /// activation.ShowUI = True
        /// annotation.Activation = activation
        /// 'Draw a annotation into the new page.
        /// page.Annoatations.Add(annotation)
        /// 'Save the  document to disk.
        /// document.Save("Pdf3DActivation.pdf")
        /// </code>
        /// </example> 
        public bool ShowUI
        {
            get
            {
                return this.m_showUI;
            }

            set
            {
                this.m_showUI = value;
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

        #region Constructor
        /// <summary>
        /// Initializes the new instance of <see cref="Pdf3DActivation"/> class.
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
        /// //Create a new Pdf3DActivation
        /// Pdf3DActivation activation = new Pdf3DActivation();
        /// activation.ActivationMode = Pdf3DActivationMode.ExplicitActivation;
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
        /// 'Create a new Pdf3DAnnotation.
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Create a new Pdf3DActivation
        /// Dim activation As Pdf3DActivation = New Pdf3DActivation()
        /// activation.ActivationMode = Pdf3DActivationMode.PageVisible
        /// activation.ShowToolbar = True
        /// 'Draws a annotation into the new page.
        /// page.Annoatations.Add(annotation)
        /// 'Save the  document to disk.
        /// document.Save("Pdf3DActivation.pdf")
        /// </code>
        /// </example> 
        public Pdf3DActivation()
        {
            this.Initialize();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Initializes the instance.
        /// </summary>
        protected virtual void Initialize()
        {
            this.m_dictionary.BeginSave += new SavePdfPrimitiveEventHandler(this.Dictionary_BeginSave);
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
            if (this.m_activationMode == Pdf3DActivationMode.PageVisible)
            {
                this.Dictionary.SetProperty(DictionaryProperties.A, new PdfName(DictionaryProperties.PV));
            }
            else if (this.m_activationMode == Pdf3DActivationMode.PageOpen)
            {
                this.Dictionary.SetProperty(DictionaryProperties.A, new PdfName(DictionaryProperties.PO));
            }
            else if (this.m_activationMode == Pdf3DActivationMode.ExplicitActivation)
            {
                this.Dictionary.SetProperty(DictionaryProperties.A, new PdfName(DictionaryProperties.XA));
            }

            if (this.m_activationState == Pdf3DActivationState.Instantiated)
            {
                this.Dictionary.SetProperty(DictionaryProperties.AIS, new PdfName(DictionaryProperties.I));
            }
            else
            {
                this.Dictionary.SetProperty(DictionaryProperties.AIS, new PdfName(DictionaryProperties.L));
            }

            if (this.m_deactivationMode == Pdf3DDeactivationMode.PageClose)
            {
                this.Dictionary.SetProperty(DictionaryProperties.D, new PdfName(DictionaryProperties.PC));
            }
            else if (this.m_deactivationMode == Pdf3DDeactivationMode.PageInvisible)
            {
                this.Dictionary.SetProperty(DictionaryProperties.D, new PdfName(DictionaryProperties.PI));
            }
            else if (this.m_deactivationMode == Pdf3DDeactivationMode.ExplicitDeactivation)
            {
                this.Dictionary.SetProperty(DictionaryProperties.D, new PdfName(DictionaryProperties.XD));
            }

            if (this.m_deactivationState == Pdf3DDeactivationState.Uninstantiated)
            {
                this.Dictionary.SetProperty(DictionaryProperties.DIS, new PdfName(DictionaryProperties.U));
            }
            else if (this.m_deactivationState == Pdf3DDeactivationState.Instantiated)
            {
                this.Dictionary.SetProperty(DictionaryProperties.DIS, new PdfName(DictionaryProperties.I));
            }
            else if (this.m_deactivationState == Pdf3DDeactivationState.Live)
            {
                this.Dictionary.SetProperty(DictionaryProperties.DIS, new PdfName(DictionaryProperties.L));
            }

            this.Dictionary.SetProperty(DictionaryProperties.TB, new PdfBoolean(this.m_showToolbar));

            this.Dictionary.SetProperty(DictionaryProperties.NP, new PdfBoolean(this.m_showUI));
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
