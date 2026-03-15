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
    /// Represents the particular areas of 3D artwork and the opacity and visibility with which individual nodes are displayed.  
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document.
    /// PdfPage page = document.Pages.Add();
    /// //Create a new Pdf3D Annotation.
    /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
    /// 'Create a new Pdf3DView
    /// Pdf3DView  view   = new Pdf3DView();
    /// //Create a new Pdf3DNode.
    /// Pdf3DNode node = new Pdf3DNode();
    /// node.Visible = true;
    /// view.Nodes.Add(node);
    /// //Adds a pdf3d view.
    /// annotation.Views.Add(view);
    /// //Adds a annotation.
    /// page.Annoatations.Add(annotation);
    /// //Save the  document to disk.
    /// document.Save("Pd3DNode.pdf");
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
    /// 'Create a new Pdf3DNode.
    /// Dim node As Pdf3DNode  = New Pdf3DNode()
    /// node.Visible = true
    /// view.Nodes.Add(node)
    /// 'Adds a pdf3d view
    /// annotation.Views.Add(view)
    /// 'Draw a annotation into the new page.
    /// page.Annoatations.Add(annotation)
    /// 'Save the  document to disk.
    /// document.Save("Pd3DNode.pdf")
    /// </code>
    /// </example>
    public class Pdf3DNode : IPdfWrapper
    {
        #region Fields
        private bool m_visible;
        private string m_name;
        private float m_opacity;
        private float[] m_matrix;
        /// <summary>
        /// Internal variable to store dictionary.
        /// </summary>
        private PdfDictionary m_dictionary = new PdfDictionary();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether the node is visible or not. 
        /// </summary>
        /// <value>True if the node is visible. </value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new Pdf3D Annotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// 'Create a new Pdf3DView
        /// Pdf3DView  view   = new Pdf3DView();
        /// //Create a new Pdf3DNode.
        /// Pdf3DNode node = new Pdf3DNode();
        /// node.Visible = true;
        /// view.Nodes.Add(node);
        /// //Adds a pdf3d view.
        /// annotation.Views.Add(view);
        /// //Adds a annotation.
        /// page.Annoatations.Add(annotation);
        /// //Save the  document to disk.
        /// document.Save("Pd3DNode.pdf");
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
        /// 'Create a new Pdf3DNode.
        /// Dim node As Pdf3DNode  = New Pdf3DNode()
        /// node.Visible = true
        /// view.Nodes.Add(node)
        /// 'Adds a pdf3d view
        /// annotation.Views.Add(view)
        /// 'Draw a annotation into the new page.
        /// page.Annoatations.Add(annotation)
        /// 'Save the  document to disk.
        /// document.Save("Pd3DNode.pdf")
        /// </code>
        /// </example>
        public bool Visible
        {
            get
            {
                return this.m_visible;
            }

            set
            {
                this.m_visible = value;
            }
        }

        /// <summary>
        /// Gets or sets the node name. 
        /// </summary>
        /// <value>The name of the 3D node.</value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new Pdf3D Annotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// 'Create a new Pdf3DView
        /// Pdf3DView  view   = new Pdf3DView();
        /// //Create a new Pdf3DNode.
        /// Pdf3DNode node = new Pdf3DNode();
        /// node.Name ="NearView;
        /// view.Nodes.Add(node);
        /// //Adds a pdf3d view.
        /// annotation.Views.Add(view);
        /// //Adds a annotation.
        /// page.Annoatations.Add(annotation);
        /// //Save the  document to disk.
        /// document.Save("Pd3DNode.pdf");
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
        /// 'Create a new Pdf3DNode.
        /// Dim node As Pdf3DNode  = New Pdf3DNode()
        /// node.Name ="NearView"
        /// view.Nodes.Add(node)
        /// 'Adds a pdf3d view
        /// annotation.Views.Add(view)
        /// 'Draw a annotation into the new page.
        /// page.Annoatations.Add(annotation)
        /// 'Save the  document to disk.
        /// document.Save("Pd3DNode.pdf")
        /// </code>
        /// </example>
        public string Name
        {
            get
            {
                return this.m_name;
            }

            set
            {
                this.m_name = value;
            }
        }

        /// <summary>
        /// Gets or sets the cutting plane opacity. 
        /// </summary>
        /// <value>A number indicating the opacity of the cutting plane using a standard additive blend mode. </value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new Pdf3D Annotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// 'Create a new Pdf3DView
        /// Pdf3DView  view   = new Pdf3DView();
        /// //Create a new Pdf3DNode.
        /// Pdf3DNode node = new Pdf3DNode();
        /// node.Opacity =1000f;
        /// view.Nodes.Add(node);
        /// //Adds a pdf3d view.
        /// annotation.Views.Add(view);
        /// //Adds a annotation.
        /// page.Annoatations.Add(annotation);
        /// //Save the  document to disk.
        /// document.Save("Pd3DNode.pdf");
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
        /// 'Create a new Pdf3DNode.
        /// Dim node As Pdf3DNode  = New Pdf3DNode()
        /// node.Opacity =1000f
        /// view.Nodes.Add(node)
        /// 'Adds a pdf3d view
        /// annotation.Views.Add(view)
        /// 'Draw a annotation into the new page.
        /// page.Annoatations.Add(annotation)
        /// 'Save the  document to disk.
        /// document.Save("Pd3DNode.pdf")
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
        /// Gets or sets the 3D transformation matrix. 
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new Pdf3D Annotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// 'Create a new Pdf3DView
        /// Pdf3DView  view   = new Pdf3DView();
        /// //Create a new Pdf3DNode.
        /// Pdf3DNode node = new Pdf3DNode();
        /// node.Matrix = new float[] { -0.382684f, 0.92388f, -0.0000000766026f, 0.18024f, 0.0746579f, 0.980785f, 0.906127f, 0.37533f, -0.19509f, -100, -112.432f, 45.6829f };
        /// view.Nodes.Add(node);
        /// //Adds a pdf3d view.
        /// annotation.Views.Add(view);
        /// //Adds a annotation.
        /// page.Annoatations.Add(annotation);
        /// //Save the  document to disk.
        /// document.Save("Pd3DNode.pdf");
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
        /// 'Create a new Pdf3DNode.
        /// Dim node As Pdf3DNode  = New Pdf3DNode()
        /// node.Matrix =New Single() {-0.382684F, 0.92388F, -0.0000000766026F, 0.18024F, 0.0746579F, 0.980785F, 0.906127F, 0.37533F, -0.19509F, -100, -112.432F, 45.6829F}
        /// view.Nodes.Add(node)
        /// 'Adds a pdf3d view
        /// annotation.Views.Add(view)
        /// 'Draw a annotation into the new page.
        /// page.Annoatations.Add(annotation)
        /// 'Save the  document to disk.
        /// document.Save("Pd3DNode.pdf")
        /// </code>
        /// </example>
        /// <value>A 12-element 3D transformation matrix that specifies the position and orientation of this node, relative to its parent, in world coordinates. </value>
        /// <remarks>If the array has more than 12 elements, only the first 12 will be considered.</remarks>
        public float[] Matrix
        {
            get
            {
                return this.m_matrix;
            }

            set
            {
                this.m_matrix = value;
                if ((this.m_matrix != null) && (this.m_matrix.Length < 12))
                {
                    throw new ArgumentOutOfRangeException("Matrix.Length", "Matrix array must have at least 12 elements.");
                }
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
        /// Initializes a new instance of the <see cref="Pdf3DNode"/> class.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new Pdf3D Annotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// 'Create a new Pdf3DView
        /// Pdf3DView  view   = new Pdf3DView();
        /// //Create a new Pdf3DNode.
        /// Pdf3DNode node = new Pdf3DNode();
        /// node.Visible = true;
        /// view.Nodes.Add(node);
        /// //Adds a pdf3d view.
        /// annotation.Views.Add(view);
        /// //Adds a annotation.
        /// page.Annoatations.Add(annotation);
        /// //Save the  document to disk.
        /// document.Save("Pd3DNode.pdf");
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
        /// 'Create a new Pdf3DNode.
        /// Dim node As Pdf3DNode  = New Pdf3DNode()
        /// node.Visible = true
        /// view.Nodes.Add(node)
        /// 'Adds a pdf3d view
        /// annotation.Views.Add(view)
        /// 'Draw a annotation into the new page.
        /// page.Annoatations.Add(annotation)
        /// 'Save the  document to disk.
        /// document.Save("Pd3DNode.pdf")
        /// </code>
        /// </example>
        public Pdf3DNode()
        {
            this.Initialize();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Initializes annotation object.
        /// </summary>
        protected virtual void Initialize()
        {
            this.m_dictionary.BeginSave += new SavePdfPrimitiveEventHandler(this.Dictionary_BeginSave);
            this.m_dictionary.SetProperty(DictionaryProperties.Type, new PdfName(DictionaryProperties._3DNode));
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
            this.Dictionary.SetProperty(DictionaryProperties.O, new PdfNumber(this.m_opacity / 100f));
            this.Dictionary.SetProperty(DictionaryProperties.V, new PdfBoolean(this.m_visible));

            if (this.m_name != null && m_name.Length > 0)
                Dictionary.SetProperty(DictionaryProperties.N, new PdfName(m_name));

            if (this.m_matrix != null && this.m_matrix.Length >= 12)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendFormat("[{0:0.####} {1:0.####} {2:0.####} {3:0.####} {4:0.####} {5:0.####} {6:0.####} {7:0.####} {8:0.####} {9:0.####} {10:0.####} {11:0.####}]\n",
                    new object[] {m_matrix[0],m_matrix[1],m_matrix[2],m_matrix[3],m_matrix[4],m_matrix[5],m_matrix[6],
                        m_matrix[7],m_matrix[8],m_matrix[9],m_matrix[10],m_matrix[11]});
                this.Dictionary.SetProperty(DictionaryProperties.M, new PdfName(builder.ToString()));
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

    /// <summary>
    /// Represents a collection of <see cref="Pdf3DNode"/> objects. 
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document.
    /// PdfPage page = document.Pages.Add();
    /// //Create a new Pdf3D Annotation.
    /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
    /// 'Create a new Pdf3DView
    /// Pdf3DView  view   = new Pdf3DView();
    /// //Create a new Pdf3DNode.
    /// Pdf3DNode node = new Pdf3DNode();
    /// node.Visible = true;
    /// view.Nodes.Add(node);
    /// //Adds a pdf3d view.
    /// annotation.Views.Add(view);
    /// //Adds a annotation.
    /// page.Annoatations.Add(annotation);
    /// //Save the  document to disk.
    /// document.Save("Pd3DNode.pdf");
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
    /// 'Create a new Pdf3DNode.
    /// Dim node As Pdf3DNode  = New Pdf3DNode()
    /// node.Visible = true
    /// view.Nodes.Add(node)
    /// 'Adds a pdf3d view
    /// annotation.Views.Add(view)
    /// 'Draw a annotation into the new page.
    /// page.Annoatations.Add(annotation)
    /// 'Save the  document to disk.
    /// document.Save("Pd3DNode.pdf")
    /// </code>
    /// </example>
    public class Pdf3DNodeCollection : List<Pdf3DNode>
    {
        #region Methods
        /// <summary>
        /// Adds the specified value.
        /// <param name="value">The value.</param>
        /// </summary>
        /// <returns></returns>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new Pdf3D Annotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// 'Create a new Pdf3DView
        /// Pdf3DView  view   = new Pdf3DView();
        /// //Create a new Pdf3DNode.
        /// Pdf3DNode node = new Pdf3DNode();
        /// node.Visible = true;
        /// view.Nodes.Add(node);
        /// //Adds a pdf3d view.
        /// annotation.Views.Add(view);
        /// //Adds a annotation.
        /// page.Annoatations.Add(annotation);
        /// //Save the  document to disk.
        /// document.Save("Pd3DNode.pdf");
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
        /// 'Create a new Pdf3DNode.
        /// Dim node As Pdf3DNode  = New Pdf3DNode()
        /// node.Visible = true
        /// view.Nodes.Add(node)
        /// 'Adds a pdf3d view
        /// annotation.Views.Add(view)
        /// 'Draw a annotation into the new page.
        /// page.Annoatations.Add(annotation)
        /// 'Save the  document to disk.
        /// document.Save("Pd3DNode.pdf")
        /// </code>
        /// </example>
        public int Add(Pdf3DNode value)
        {
            base.Add(value);
            return base.IndexOf(value);
        }

        /// <summary>
        /// Determines whether [contains] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new Pdf3DNode.
        /// Pdf3DNode node = new Pdf3DNode();
        /// node.Visible = true;
        /// node.Near="Near View";
        /// node.Opacity=100;
        /// node.Matrix = new float[] { -0.382684f, 0.92388f, -0.0000000766026f, 0.18024f, 0.0746579f, 0.980785f, 0.906127f, 0.37533f, -0.19509f, -100, -112.432f, 45.6829f };
        /// Pdf3DNodeCollection nodecollection = new Pdf3DNodeCollection();
        /// nodecollection.Add(node);
        /// bool exist=nodecollection.Contains(node);
        /// </code>
        /// <code lang="VB">
        /// 'Create a new Pdf3DNode.
        /// Dim node As Pdf3DNode  = New Pdf3DNode()
        /// node.Visible = true
        /// node.Near="Near View"
        /// node.Opacity=100
        /// node.Matrix =New Single() {-0.382684F, 0.92388F, -0.0000000766026F, 0.18024F, 0.0746579F, 0.980785F, 0.906127F, 0.37533F, -0.19509F, -100, -112.432F, 45.6829F}
        /// Dim nodecollection Pdf3DNodeCollection  = New Pdf3DNodeCollection();
        /// nodecollection.Add(node);
        /// bool exist=nodecollection.Contains(node)
        /// </code>
        /// </example> 
        /// <returns>
        /// if it contains the specified value, set to <c>true</c>.
        /// </returns>
        public bool Contains(Pdf3DNode value)
        {
            return base.Contains(value);
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new Pdf3DNode.
        /// Pdf3DNode node = new Pdf3DNode();
        /// node.Visible = true;
        /// node.Near="Near View";
        /// node.Opacity=100;
        /// node.Matrix = new float[] { -0.382684f, 0.92388f, -0.0000000766026f, 0.18024f, 0.0746579f, 0.980785f, 0.906127f, 0.37533f, -0.19509f, -100, -112.432f, 45.6829f };
        /// Pdf3DNodeCollection nodecollection = new Pdf3DNodeCollection();
        /// nodecollection.Add(node);
        /// int index=nodecollection.IndexOf(node);
        /// </code>
        /// <code lang="VB">
        /// 'Create a new Pdf3DNode.
        /// Dim node As Pdf3DNode  = New Pdf3DNode()
        /// node.Visible = true
        /// node.Near="Near View"
        /// node.Opacity=100
        /// node.Matrix =New Single() {-0.382684F, 0.92388F, -0.0000000766026F, 0.18024F, 0.0746579F, 0.980785F, 0.906127F, 0.37533F, -0.19509F, -100, -112.432F, 45.6829F}
        /// Dim nodecollection Pdf3DNodeCollection  = New Pdf3DNodeCollection()
        /// nodecollection.Add(node)
        /// Dim index As index=nodecollection.IndexOf(node)
        /// </code>
        /// </example> 
        public int IndexOf(Pdf3DNode value)
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
        /// //Create a new Pdf3DNode.
        /// Pdf3DNode node = new Pdf3DNode();
        /// node.Visible = true;
        /// node.Near="Near View";
        /// node.Opacity=100;
        /// node.Matrix = new float[] { -0.382684f, 0.92388f, -0.0000000766026f, 0.18024f, 0.0746579f, 0.980785f, 0.906127f, 0.37533f, -0.19509f, -100, -112.432f, 45.6829f };
        /// Pdf3DNodeCollection nodecollection = new Pdf3DNodeCollection()
        /// nodecollection.Insert(0,node)
        /// </code>
        /// <code lang="VB">
        /// 'Create a new Pdf3DNode.
        /// Dim node As Pdf3DNode  = New Pdf3DNode()
        /// node.Visible = true
        /// node.Near="Near View"
        /// node.Opacity=100
        /// node.Matrix =New Single() {-0.382684F, 0.92388F, -0.0000000766026F, 0.18024F, 0.0746579F, 0.980785F, 0.906127F, 0.37533F, -0.19509F, -100, -112.432F, 45.6829F}
        /// Dim nodecollection Pdf3DNodeCollection  = New Pdf3DNodeCollection()
        /// nodecollection.Insert(0,node)
        /// </code>
        /// </example> 
        public void Insert(int index, Pdf3DNode value)
        {
            base.Insert(index, value);
        }

        /// <summary>
        /// Removes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new Pdf3DNode.
        /// Pdf3DNode node = new Pdf3DNode();
        /// node.Visible = true;
        /// node.Near="Near View";
        /// node.Opacity=100;
        /// node.Matrix = new float[] { -0.382684f, 0.92388f, -0.0000000766026f, 0.18024f, 0.0746579f, 0.980785f, 0.906127f, 0.37533f, -0.19509f, -100, -112.432f, 45.6829f };
        /// Pdf3DNodeCollection nodecollection = new Pdf3DNodeCollection()
        /// nodecollection.Remove(node)
        /// </code>
        /// <code lang="VB">
        /// 'Create a new Pdf3DNode.
        /// Dim node As Pdf3DNode  = New Pdf3DNode()
        /// node.Visible = true
        /// node.Near="Near View"
        /// node.Opacity=100
        /// node.Matrix =New Single() {-0.382684F, 0.92388F, -0.0000000766026F, 0.18024F, 0.0746579F, 0.980785F, 0.906127F, 0.37533F, -0.19509F, -100, -112.432F, 45.6829F}
        /// Dim nodecollection Pdf3DNodeCollection  = New Pdf3DNodeCollection()
        /// nodecollection.Remove(node)
        /// </code>
        /// </example> 
        public void Remove(Pdf3DNode value)
        {
            base.Remove(value);
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.Pdf.Interactive.Pdf3DNode"/> at the specified index.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new Pdf3DNode.
        /// Pdf3DNode node = new Pdf3DNode();
        /// node.Visible = true;
        /// node.Near="Near View";
        /// node.Opacity=100;
        /// node.Matrix = new float[] { -0.382684f, 0.92388f, -0.0000000766026f, 0.18024f, 0.0746579f, 0.980785f, 0.906127f, 0.37533f, -0.19509f, -100, -112.432f, 45.6829f };
        /// Pdf3DNodeCollection nodecollection = new Pdf3DNodeCollection();
        /// nodecollection[0]=node;
        /// </code>
        /// <code lang="VB">
        /// 'Create a new Pdf3DNode.
        /// Dim node As Pdf3DNode  = New Pdf3DNode()
        /// node.Visible = true
        /// node.Near="Near View"
        /// node.Opacity=100
        /// node.Matrix =New Single() {-0.382684F, 0.92388F, -0.0000000766026F, 0.18024F, 0.0746579F, 0.980785F, 0.906127F, 0.37533F, -0.19509F, -100, -112.432F, 45.6829F}
        /// Dim nodecollection Pdf3DNodeCollection  = New Pdf3DNodeCollection()
        /// nodecollection[0]=node
        /// </code>
        /// </example> 
        public Pdf3DNode this[int index]
        {
            get
            {
                return base[index];
            }

            set
            {
                base[index] = value;
            }
        }

        #endregion
    }
}
