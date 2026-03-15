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
    /// Represents the clipping portion of the 3D artwork for the purpose of showing artwork cross sections.
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document.
    /// PdfPage page = document.Pages.Add();
    /// //Create a new Pdf3DAnnotation.
    /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
    /// //Creates a new Pdf3DCrossSection.
    /// Pdf3DCrossSection crossSection = new Pdf3DCrossSection();
    /// crossSection.Color = new PdfColor(Color.Blue);
    /// crossSection.IntersectionIsVisible = true;
    /// crossSection.IntersectionColor = new PdfColor(Color.Red);
    /// //Create a new Pdf3DView
    /// Pdf3DView view = new Pdf3DView();
    /// view.CrossSections.Add(crossSection);
    /// annotation.Views.Add(view);
    /// page.Annoatations.Add(annotation);
    /// //Save the  document to disk.
    /// document.Save("Pdf3DCrossSection.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document.
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create a new Pdf3DAnnotation.
    /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
    /// 'Creates a new Pdf3DCrossSection.
    /// Dim crossSection As Pdf3DCrossSection  = New Pdf3DCrossSection()
    /// crossSection.Color = New PdfColor(Color.Blue)
    /// crossSection.IntersectionIsVisible = True
    /// crossSection.IntersectionColor = New PdfColor(Color.Red)
    /// 'Create a new Pdf3DView
    /// Dim  view As Pdf3DView = New Pdf3DView()
    /// view.CrossSections.Add(crossSection)
    /// annotation.Views.Add(view)
    /// 'Save the  document to disk.
    /// document.Save("Pdf3DCrossSection.pdf")
    /// </code>
    /// </example> 
    public class Pdf3DCrossSection : IPdfWrapper
    {
        #region Fields
        private float[] m_center;
        private PdfColor m_color;
        private PdfColor m_intersectionColor;
        private bool m_intersectionIsVisible;
        private object[] m_orientation;
        private float m_opacity;

        /// <summary>
        /// Internal variable to store dictionary.
        /// </summary>
        private PdfDictionary m_dictionary = new PdfDictionary();
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Pdf3DCrossSection"/> class.
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
        /// //Creates a new Pdf3DCrossSection.
        /// Pdf3DCrossSection crossSection = new Pdf3DCrossSection();
        /// crossSection.Color = new PdfColor(Color.Blue);
        /// crossSection.IntersectionIsVisible = true;
        /// crossSection.IntersectionColor = new PdfColor(Color.Red);
        /// //Create a new Pdf3DView
        /// Pdf3DView view = new Pdf3DView();
        /// view.CrossSections.Add(crossSection);
        /// annotation.Views.Add(view);
        /// page.Annoatations.Add(annotation);
        /// //Save the  document to disk.
        /// document.Save("Pdf3DCrossSection.pdf");
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
        /// 'Creates a new Pdf3DCrossSection
        /// Dim crossSection As Pdf3DCrossSection  = New Pdf3DCrossSection()
        /// crossSection.Color = New PdfColor(Color.Blue)
        /// crossSection.IntersectionIsVisible = True
        /// crossSection.IntersectionColor = New PdfColor(Color.Red)
        /// 'Create a new Pdf3DView
        /// Dim  view As Pdf3DView = New Pdf3DView()
        /// view.CrossSections.Add(crossSection)
        /// annotation.Views.Add(view)
        /// 'Save the  document to disk.
        /// document.Save("Pdf3DCrossSection.pdf")
        /// </code>
        /// </example> 
        public Pdf3DCrossSection()
        {
            this.Initialize();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the center of the cutting plane. 
        /// <remarks>A three element array specifying the center of rotation on the cutting plane in world space coordinates.</remarks>
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new Pdf3DAnnotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// //Creates a new Pdf3DCrossSection.
        /// Pdf3DCrossSection crossSection = new Pdf3DCrossSection();
        /// crossSection.Center = new float[] {40,40,40};
        /// //Create a new Pdf3DView
        /// Pdf3DView view = new Pdf3DView();
        /// view.CrossSections.Add(crossSection);
        /// annotation.Views.Add(view);
        /// page.Annoatations.Add(annotation);
        /// //Save the  document to disk.
        /// document.Save("Pdf3DCrossSection.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new Pdf3DAnnotation.
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Creates a new Pdf3DCrossSection.
        /// Dim crossSection As Pdf3DCrossSection  = New Pdf3DCrossSection()
        /// crossSection.Center = New Single() {60,60,60}
        /// 'Create a new Pdf3DView
        /// Dim  view As Pdf3DView = New Pdf3DView()
        /// view.CrossSections.Add(crossSection)
        /// annotation.Views.Add(view)
        /// 'Save the  document to disk.
        /// document.Save("Pdf3DCrossSection.pdf")
        /// </code>
        /// </example> 
        public float[] Center
        {
            get
            {
                return this.m_center;
            }

            set
            {
                this.m_center = value;
                if (this.m_center == null || this.m_center.Length < 3)
                {
                    throw new ArgumentOutOfRangeException("Center.Length", "Center Array must have atleast 3 elements.");
                }
            }
        }

        /// <summary>
        /// Gets or sets the cutting plane color. 
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new Pdf3DAnnotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// //Creates a new Pdf3DCrossSection.
        /// Pdf3DCrossSection crossSection = new Pdf3DCrossSection();
        /// crossSection.Color = new PdfColor(Color.Blue);
        /// //Create a new Pdf3DView
        /// Pdf3DView view = new Pdf3DView();
        /// view.CrossSections.Add(crossSection);
        /// annotation.Views.Add(view);
        /// page.Annoatations.Add(annotation);
        /// //Save the  document to disk.
        /// document.Save("Pdf3DCrossSection.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new Pdf3DAnnotation.
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Creates a new Pdf3DCrossSection.
        /// Dim crossSection As Pdf3DCrossSection  = New Pdf3DCrossSection()
        /// crossSection.Color = New PdfColor(Color.Blue)
        /// 'Create a new Pdf3DView
        /// Dim  view As Pdf3DView = New Pdf3DView()
        /// view.CrossSections.Add(crossSection)
        /// annotation.Views.Add(view)
        /// 'Save the  document to disk.
        /// document.Save("Pdf3DCrossSection.pdf")
        /// </code>
        /// </example> 
        public PdfColor Color
        {
            get
            {
                return this.m_color;
            }

            set
            {
                this.m_color = value;
            }
        }

        /// <summary>
        /// Gets or sets the intersection color.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new Pdf3DAnnotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// //Creates a new Pdf3DCrossSection.
        /// Pdf3DCrossSection crossSection = new Pdf3DCrossSection();
        /// crossSection.IntersectionColor = new PdfColor(Color.Red);
        /// //Create a new Pdf3DView
        /// Pdf3DView view = new Pdf3DView();
        /// view.CrossSections.Add(crossSection);
        /// annotation.Views.Add(view);
        /// page.Annoatations.Add(annotation);
        /// //Save the  document to disk.
        /// document.Save("Pdf3DCrossSection.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new Pdf3DAnnotation.
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Creates a new Pdf3DCrossSection.
        /// Dim crossSection As Pdf3DCrossSection  = New Pdf3DCrossSection()
        /// crossSection.IntersectionColor = New PdfColor(Color.Red)
        /// 'Create a new Pdf3DView
        /// Dim  view As Pdf3DView = New Pdf3DView()
        /// view.CrossSections.Add(crossSection)
        /// annotation.Views.Add(view)
        /// 'Save the  document to disk.
        /// document.Save("Pdf3DCrossSection.pdf")
        /// </code>
        /// </example> 
        public PdfColor IntersectionColor
        {
            get
            {
                return this.m_intersectionColor;
            }

            set
            {
                this.m_intersectionColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the intersection of cutting plane with 3D artwork is visible.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new Pdf3DAnnotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// //Creates a new Pdf3DCrossSection.
        /// Pdf3DCrossSection crossSection = new Pdf3DCrossSection();
        /// crossSection.IntersectionIsVisible = true;
        /// //Create a new Pdf3DView
        /// Pdf3DView view = new Pdf3DView();
        /// view.CrossSections.Add(crossSection);
        /// annotation.Views.Add(view);
        /// page.Annoatations.Add(annotation);
        /// //Save the  document to disk.
        /// document.Save("Pdf3DCrossSection.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new Pdf3DAnnotation.
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Creates a new Pdf3DCrossSection.
        /// Dim crossSection As Pdf3DCrossSection  = New Pdf3DCrossSection()
        /// crossSection.IntersectionIsVisible = True
        /// 'Create a new Pdf3DView
        /// Dim  view As Pdf3DView = New Pdf3DView()
        /// view.CrossSections.Add(crossSection)
        /// annotation.Views.Add(view)
        /// 'Save the  document to disk.
        /// document.Save("Pdf3DCrossSection.pdf")
        /// </code>
        /// </example> 
        public bool IntersectionIsVisible
        {
            get
            {
                return this.m_intersectionIsVisible;
            }

            set
            {
                this.m_intersectionIsVisible = value;
            }
        }

        /// <summary>
        /// Gets or sets the cutting plane opacity. 
        /// <remarks>The opacity is given in percents, 100 is full opacity, 0 is no opacity.</remarks>
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new Pdf3DAnnotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// //Creates a new Pdf3DCrossSection.
        /// Pdf3DCrossSection crossSection = new Pdf3DCrossSection();
        ///crossSection.Opacity =100;
        /// //Create a new Pdf3DView
        /// Pdf3DView view = new Pdf3DView();
        /// view.CrossSections.Add(crossSection);
        /// annotation.Views.Add(view);
        /// page.Annoatations.Add(annotation);
        /// //Save the  document to disk.
        /// document.Save("Pdf3DCrossSection.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new Pdf3DAnnotation.
        /// Dim annotation As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Creates a new Pdf3DCrossSection.
        /// Dim crossSection As Pdf3DCrossSection  = New Pdf3DCrossSection()
        /// crossSection.Opacity =100
        /// 'Create a new Pdf3DView
        /// Dim  view As Pdf3DView = New Pdf3DView()
        /// view.CrossSections.Add(crossSection)
        /// annotation.Views.Add(view)
        /// 'Save the  document to disk.
        /// document.Save("Pdf3DCrossSection.pdf")
        /// </code>
        /// </example> 
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
        /// Gets or sets the orientation of the cutting plane. 
        /// <value>A three-element array specifying the orientation of the cutting plane in world space, where each value represents the orientation in relation to the X, Y, and Z axes, respectively. </value>
        /// <remarks>If the array has more than 3 elements, only the first 3 will be considered. Exactly one of the values must be null, indicating an initial state of the cutting plane that is perpendicular to the corresponding axis and clipping all geometry on the positive side of that axis. The other two values must be numbers indicating the rotation of the plane, in degrees, around their corresponding axes. The order in which these rotations are applied should match the order in which the values appear in the array. </remarks>
        /// </summary>
        public object[] Orientation
        {
            get
            {
                return this.m_orientation;
            }

            set
            {
                this.m_orientation = value;
                if (this.m_orientation == null || this.m_orientation.Length < 3)
                {
                    throw new ArgumentOutOfRangeException("Orientation.Length", "Orientation Array must have atleast 3 elements.");
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

        #region Implementation
        /// <summary>
        /// Initializes annotation object.
        /// </summary>
        protected virtual void Initialize()
        {
            this.m_dictionary.BeginSave += new SavePdfPrimitiveEventHandler(this.Dictionary_BeginSave);
            this.m_dictionary.SetProperty(DictionaryProperties.Type, new PdfName(DictionaryProperties._3DCrossSection));
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
            if (this.m_center != null)
            {
                this.Dictionary.SetProperty(DictionaryProperties.C, new PdfArray(this.m_center));
            }

            if (this.m_orientation != null)
            {
                PdfArray orientation = new PdfArray();

                if (this.m_orientation[0] == null)
                {
                    orientation.Insert(0, new PdfName("null"));
                }
                else
                {
                    orientation.Insert(0, new PdfName((string)this.m_orientation[0]));
                }

                if (this.m_orientation[1] == null)
                {
                    orientation.Insert(1, new PdfName("null"));
                }
                else
                {
                    orientation.Insert(1, new PdfName((string)this.m_orientation[1]));
                }

                if (this.m_orientation[2] == null)
                {
                    orientation.Insert(2, new PdfName("null"));
                }
                else
                {
                    orientation.Insert(2, new PdfName((string)this.m_orientation[2]));
                }
                this.Dictionary[DictionaryProperties.O] = new PdfArray(orientation);
            }

            this.Dictionary.SetProperty(DictionaryProperties.PO, new PdfNumber(this.m_opacity));

            if (this.m_color != null)
            {
                float red = this.m_color.R / 255f;
                float green = this.m_color.G / 255f;
                float blue = this.m_color.B / 255f;

                PdfArray color = new PdfArray();
                color.Insert(0, new PdfName("DeviceRGB"));
                color.Insert(1, new PdfNumber(red));
                color.Insert(2, new PdfNumber(green));
                color.Insert(3, new PdfNumber(blue));

                this.Dictionary[DictionaryProperties.PC] = new PdfArray(color);
            }

            if (this.m_intersectionColor != null)
            {
                float red = this.m_intersectionColor.R / 255f;
                float green = this.m_intersectionColor.G / 255f;
                float blue = this.m_intersectionColor.B / 255f;

                PdfArray intersectioncolor = new PdfArray();
                intersectioncolor.Insert(0, new PdfName("DeviceRGB"));
                intersectioncolor.Insert(1, new PdfNumber(red));
                intersectioncolor.Insert(2, new PdfNumber(green));
                intersectioncolor.Insert(3, new PdfNumber(blue));

                this.Dictionary[DictionaryProperties.IC] = new PdfArray(intersectioncolor);
            }
            this.Dictionary.SetProperty(DictionaryProperties.IV, new PdfBoolean(this.m_intersectionIsVisible));
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
    /// Represents the collection of <see cref="Pdf3DCrossSection"/> objects. 
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document.
    /// PdfPage page = document.Pages.Add();
    /// //Create a new Pdf3DAnnotation.
    /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
    /// //Create a new Pdf3DView
    /// Pdf3DView view = new Pdf3DView();
    /// //Creates a new Pdf3DCrossSectionCollection.
    /// Pdf3DCrossSectionCollection crossSectionCollection=view.CrossSections;
    /// annotation.Views.Add(view);
    /// page.Annoatations.Add(annotation);
    /// //Save the  document to disk.
    /// document.Save("3DCrossSectionCollection.pdf");
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
    /// 'Creates a new Pdf3DCrossSectionCollection.
    /// Dim crossSectionCollection As Pdf3DCrossSectionCollection=view.CrossSections
    /// annotation.Views.Add(view)
    /// 'Save the  document to disk.
    /// document.Save("3DCrossSectionCollection.pdf")
    /// </code>
    /// </example> 
    public class Pdf3DCrossSectionCollection : List<Pdf3DCrossSection>
    {
        #region Methods
        /// <summary>
        /// Adds the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <example>
        /// <code lang = "C#">
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new Pdf3DAnnotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// //Creates a new Pdf3DCrossSection.
        /// Pdf3DCrossSection crossSection = new Pdf3DCrossSection();
        /// crossSection.Color = new PdfColor(Color.Blue);
        /// crossSection.IntersectionIsVisible = true;
        /// crossSection.IntersectionColor = new PdfColor(Color.Red);
        /// //Create a new Pdf3DView
        /// Pdf3DView view = new Pdf3DView();
        /// view.CrossSections.Add(crossSection);
        /// annotation.Views.Add(view);
        /// page.Annoatations.Add(annotation);
        /// //Save the  document to disk.
        /// document.Save("Pdf3DCrossSection.pdf");
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
        /// 'Creates a new Pdf3DCrossSection.
        /// Dim crossSection As Pdf3DCrossSection  = New Pdf3DCrossSection()
        /// crossSection.Color = New PdfColor(Color.Blue)
        /// crossSection.IntersectionIsVisible = True
        /// crossSection.IntersectionColor = New PdfColor(Color.Red)
        /// 'Create a new Pdf3DView
        /// Dim  view As Pdf3DView = New Pdf3DView()
        /// view.CrossSections.Add(crossSection)
        /// annotation.Views.Add(view)
        /// 'Save the  document to disk.
        /// document.Save("Pdf3DCrossSection.pdf")
        /// </code>
        /// </example> 
        public int Add(Pdf3DCrossSection value)
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
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new Pdf3DAnnotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// //Creates a new Pdf3DCrossSection.
        /// Pdf3DCrossSection crossSection = new Pdf3DCrossSection();
        /// crossSection.Color = new PdfColor(Color.Blue);
        /// crossSection.IntersectionIsVisible = true;
        /// crossSection.IntersectionColor = new PdfColor(Color.Red);
        /// //Create a new Pdf3DView
        /// Pdf3DView view = new Pdf3DView();
        /// bool isExist= view.CrossSections.Contains(crossSection);
        /// annotation.Views.Add(view);
        /// page.Annoatations.Add(annotation);
        /// //Save the  document to disk.
        /// document.Save("Pdf3DCrossSection.pdf");
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
        /// 'Creates a new Pdf3DCrossSection.
        /// Dim crossSection As Pdf3DCrossSection  = New Pdf3DCrossSection()
        /// crossSection.Color = New PdfColor(Color.Blue)
        /// crossSection.IntersectionIsVisible = True
        /// crossSection.IntersectionColor = New PdfColor(Color.Red)
        /// 'Create a new Pdf3DView
        /// Dim  view As Pdf3DView = New Pdf3DView()
        /// Dim Boolean As isExist= view.CrossSections.Contains(crossSection)
        /// annotation.Views.Add(view)
        /// 'Save the  document to disk.
        /// document.Save("Pdf3DCrossSection.pdf")
        /// </code>
        /// </example> 
        public bool Contains(Pdf3DCrossSection value)
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
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new Pdf3DAnnotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// //Creates a new Pdf3DCrossSection.
        /// Pdf3DCrossSection crossSection = new Pdf3DCrossSection();
        /// crossSection.Color = new PdfColor(Color.Blue);
        /// crossSection.IntersectionIsVisible = true;
        /// crossSection.IntersectionColor = new PdfColor(Color.Red);
        /// //Create a new Pdf3DView
        /// Pdf3DView view = new Pdf3DView();
        /// view.CrossSections.Add(crossSection);
        /// annotation.Views.Add(view);
        /// page.Annoatations.Add(annotation);
        /// //Save the  document to disk.
        /// document.Save("Pdf3DCrossSection.pdf");
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
        /// 'Creates a new Pdf3DCrossSection.
        /// Dim crossSection As Pdf3DCrossSection  = New Pdf3DCrossSection()
        /// crossSection.Color = New PdfColor(Color.Blue)
        /// crossSection.IntersectionIsVisible = True
        /// crossSection.IntersectionColor = New PdfColor(Color.Red)
        /// 'Create a new Pdf3DView
        /// Dim  view As Pdf3DView = New Pdf3DView()
        /// view.CrossSections.Add(crossSection)
        /// annotation.Views.Add(view)
        /// 'Save the  document to disk.
        /// document.Save("Pdf3DCrossSection.pdf")
        /// </code>
        /// </example> 
        public int IndexOf(Pdf3DCrossSection value)
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
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new Pdf3DAnnotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// //Creates a new Pdf3DCrossSection.
        /// Pdf3DCrossSection crossSection = new Pdf3DCrossSection();
        /// crossSection.Color = new PdfColor(Color.Blue);
        /// crossSection.IntersectionIsVisible = true;
        /// crossSection.IntersectionColor = new PdfColor(Color.Red);
        /// //Create a new Pdf3DView
        /// Pdf3DView view = new Pdf3DView();
        /// view.CrossSections.Insert(0,crossSection);
        /// annotation.Views.Add(view);
        /// page.Annoatations.Add(annotation);
        /// //Save the  document to disk.
        /// document.Save("Pdf3DCrossSection.pdf");
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
        /// 'Creates a new Pdf3DCrossSection.
        /// Dim crossSection As Pdf3DCrossSection  = New Pdf3DCrossSection()
        /// crossSection.Color = New PdfColor(Color.Blue)
        /// crossSection.IntersectionIsVisible = True
        /// crossSection.IntersectionColor = New PdfColor(Color.Red)
        /// 'Create a new Pdf3DView
        /// Dim  view As Pdf3DView = New Pdf3DView()
        /// view.CrossSections.Insert(0,crossSection)
        /// annotation.Views.Add(view)
        /// 'Save the  document to disk.
        /// document.Save("Pdf3DCrossSection.pdf")
        /// </code>
        /// </example> 
        public void Insert(int index, Pdf3DCrossSection value)
        {
            base.Insert(index, value);
        }

        /// <summary>
        /// Removes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <example>
        /// <code lang = "C#">
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new Pdf3DAnnotation.
        /// Pdf3DAnnotation annotation = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// //Creates a new Pdf3DCrossSection.
        /// Pdf3DCrossSection crossSection = new Pdf3DCrossSection();
        /// crossSection.Color = new PdfColor(Color.Blue);
        /// crossSection.IntersectionIsVisible = true;
        /// crossSection.IntersectionColor = new PdfColor(Color.Red);
        /// //Create a new Pdf3DView
        /// Pdf3DView view = new Pdf3DView();
        /// view.CrossSections.Add(crossSection);
        /// annotation.Views.Add(view);
        /// page.Annoatations.Add(annotation);
        /// page.Annoatations.Remove(annotation);
        /// //Save the  document to disk.
        /// document.Save("Pdf3DCrossSection.pdf");
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
        /// 'Creates a new Pdf3DCrossSection.
        /// Dim crossSection As Pdf3DCrossSection  = New Pdf3DCrossSection()
        /// crossSection.Color = New PdfColor(Color.Blue)
        /// crossSection.IntersectionIsVisible = True
        /// crossSection.IntersectionColor = New PdfColor(Color.Red)
        /// 'Create a new Pdf3DView
        /// Dim  view As Pdf3DView = New Pdf3DView()
        /// view.CrossSections.Add(crossSection)
        /// view.CrossSections.Remove(crossSection)
        /// annotation.Views.Add(view)
        /// 'Save the  document to disk.
        /// document.Save("Pdf3DCrossSection.pdf")
        /// </code>
        /// </example> 
        public void Remove(Pdf3DCrossSection value)
        {
            base.Remove(value);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.Pdf.Interactive.Pdf3DCrossSection"/> at the specified index.
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
        /// //Creates a new Pdf3DCrossSection.
        /// Pdf3DCrossSection crossSection = new Pdf3DCrossSection();
        /// crossSection.Color = new PdfColor(Color.Blue);
        /// crossSection.IntersectionIsVisible = true;
        /// crossSection.IntersectionColor = new PdfColor(Color.Red);
        /// //Create a new Pdf3DView
        /// Pdf3DView view = new Pdf3DView();
        /// view.CrossSections.Add(crossSection);
        /// Pdf3DCrossSection crosssection =view.CrossSections[0];
        /// annotation.Views.Add(view);
        /// page.Annoatations.Add(annotation);
        /// //Save the  document to disk.
        /// document.Save("Pdf3DCrossSection.pdf");
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
        /// 'Creates a new Pdf3DCrossSection.
        /// Dim crossSection As Pdf3DCrossSection  = New Pdf3DCrossSection()
        /// crossSection.Color = New PdfColor(Color.Blue)
        /// crossSection.IntersectionIsVisible = True
        /// crossSection.IntersectionColor = New PdfColor(Color.Red)
        /// 'Create a new Pdf3DView
        /// Dim  view As Pdf3DView = New Pdf3DView()
        /// view.CrossSections.Add(crossSection)
        /// Dim crosssection As Pdf3DCrossSection=view.CrossSections[0]
        /// annotation.Views.Add(view)
        /// 'Save the  document to disk.
        /// document.Save("Pdf3DCrossSection.pdf")
        /// </code>
        /// </example> 
        public Pdf3DCrossSection this[int index]
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
