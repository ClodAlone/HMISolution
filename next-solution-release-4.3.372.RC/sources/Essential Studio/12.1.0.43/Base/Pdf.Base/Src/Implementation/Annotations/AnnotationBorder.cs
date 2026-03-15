#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents the appearance of an annotation's border.
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// //Create a new Pdf Document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document.
    /// PdfPage page = document.Pages.Add();
    /// //Create a new rectangle
    /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
    /// //Create a new popup annotation.
    /// PdfPopupAnnotation popupAnnotation = new PdfPopupAnnotation(popupAnnotationRectangle,"Test popup annotation");
    /// //Set the annotation border to popup annotation.
    /// popupAnnotation.Border = new PdfAnnotationBorder(4, 0, 0);
    /// //Add this annotation to a new page.
    /// page.Annotations.Add(popupAnnotation);
    /// //Save the  document to disk.
    /// document.Save("AnnotationBorder.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document.
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create a new rectangle
    /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
    /// 'Create a new popup annotation.
    /// Dim popupAnnotation As PdfPopupAnnotation = New PdfPopupAnnotation(popupAnnotationRectangle, "Test popup annotation")
    /// 'Set the annotation border to popup annotation.
    /// popupAnnotation.Border = New PdfAnnotationBorder(4, 0, 0)
    /// 'Add this annotation to a new page.
    /// page.Annotations.Add(popupAnnotation)
    /// 'Save the document to disk.
    /// document.Save("AnnotationBorder.pdf")
    /// </code>
    /// </example> 
    public class PdfAnnotationBorder : IPdfWrapper
    {
        #region Fields
        /// <summary>
        /// Horizontal corner radius.
        /// </summary>
        private float m_horizontalRadius = 0;

        /// <summary>
        /// Vertical corner radius.
        /// </summary>
        private float m_verticalRadius = 0;

        /// <summary>
        /// Width of the border.
        /// </summary>
        private float m_borderWidth = 1;

        /// <summary>
        /// Pdf primitive representing this object.
        /// </summary>
        private PdfArray m_array = new PdfArray();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a horizontal corner radius.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new Pdf Document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new popup annotation.
        /// PdfPopupAnnotation popupAnnotation = new PdfPopupAnnotation(popupAnnotationRectangle,"Test popup annotation");
        /// //Set the horizontal radius to popup annotation border.
        /// popupAnnotation.Border.HorizontalRadius = 0;
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(popupAnnotation);
        /// //Save the  document to disk.
        /// document.Save("HorizontalRadius.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new popup annotation.
        /// Dim popupAnnotation As PdfPopupAnnotation = New PdfPopupAnnotation(popupAnnotationRectangle, "Test popup annotation")
        /// 'Set the horizontal radius to popup annotation border.
        /// popupAnnotation.Border.HorizontalRadius = 0
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(popupAnnotation)
        /// 'Save the  document to disk.
        /// document.Save("HorizontalRadius.pdf")
        /// </code>
        /// </example> 
        public float HorizontalRadius
        {
            get
            {
                return this.m_horizontalRadius;
            }

            set
            {
                if (this.m_horizontalRadius != value)
                {
                    this.m_horizontalRadius = value;
                    this.SetNumber(0, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets a vertical corner radius.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new Pdf Document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new popup annotation.
        /// PdfPopupAnnotation popupAnnotation = new PdfPopupAnnotation(popupAnnotationRectangle,"Test popup annotation");
        /// //Set the vertical radius to popup annotation border.
        /// popupAnnotation.Border.VerticalRadius = 0;
        /// //Add popup annotation annotation to a new page.
        /// page.Annotations.Add(popupAnnotation);
        /// //Save the document to disk.
        /// document.Save("VerticalRadius.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new popup annotation.
        /// Dim popupAnnotation As PdfPopupAnnotation = New PdfPopupAnnotation(popupAnnotationRectangle, "Test popup annotation")
        /// 'Set the vertical radius to popup annotation border.
        /// popupAnnotation.Border.VerticalRadius = 0
        /// 'Add popup annotation annotation to a new page.
        /// page.Annotations.Add(popupAnnotation)
        /// Save the document to disk.
        /// document.Save("VerticalRadius.pdf")
        /// </code>
        /// </example> 
        public float VerticalRadius
        {
            get
            {
                return this.m_verticalRadius;
            }

            set
            {
                if (this.m_verticalRadius != value)
                {
                    this.m_verticalRadius = value;
                    this.SetNumber(1, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the width of annotation's border. 
        /// </summary>
        /// <value>A float value specifying the width of the annotation's border. </value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new Pdf Document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new popup annotation.
        /// PdfPopupAnnotation popupAnnotation = new PdfPopupAnnotation(popupAnnotationRectangle,"Test popup annotation");
        /// //Set the width to popup anotation border.
        ///  popupAnnotation.Border.Width = 4;
        /// //Add popup annotation annotation to a new page.
        /// page.Annotations.Add(popupAnnotation);
        /// //Save the document to disk.
        /// document.Save("BorderWidth.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new popup annotation.
        /// Dim popupAnnotation As PdfPopupAnnotation = New PdfPopupAnnotation(popupAnnotationRectangle, "Test popup annotation")
        /// 'Set the width to popup anotation border.
        /// popupAnnotation.Border.Width = 4
        /// 'Add popup annotation annotation to a new page.
        /// page.Annotations.Add(popupAnnotation)
        /// 'Save the document to disk.
        /// document.Save("BorderWidth.pdf")
        /// </code>
        /// </example> 
        public float Width
        {
            get
            {
                return this.m_borderWidth;
            }

            set
            {
                if (this.m_borderWidth != value)
                {
                    this.m_borderWidth = value;
                    this.SetNumber(2, value);
                }
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfAnnotationBorder"/> class.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new Pdf Document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new popup annotation.
        /// PdfPopupAnnotation popupAnnotation = new PdfPopupAnnotation(popupAnnotationRectangle,"Test popup annotation");
        /// //Set the annotation border.
        /// popupAnnotation.Border = new PdfAnnotationBorder();
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(popupAnnotation);
        /// //Save the  document to disk.
        /// document.Save("AnnotationBorder.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new popup annotation.
        /// Dim popupAnnotation As PdfPopupAnnotation = New PdfPopupAnnotation(popupAnnotationRectangle, "Test popup annotation")
        /// 'Set the annotation border.
        /// popupAnnotation.Border = new PdfAnnotationBorder()
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(popupAnnotation)
        /// 'Save the  document to disk.
        /// document.Save("AnnotationBorder.pdf")
        /// </code>
        /// </example> 
        public PdfAnnotationBorder()
        {
            this.Initialize(this.m_borderWidth, this.m_horizontalRadius, this.m_verticalRadius);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfAnnotationBorder"/> class.
        /// </summary>
        /// <param name="borderWidth">A float value specifying the width of the annotation's border.</param>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new Pdf Document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new popup annotation.
        /// PdfPopupAnnotation popupAnnotation = new PdfPopupAnnotation(popupAnnotationRectangle,"Test popup annotation");
        /// //Set the annotation border.
        /// popupAnnotation.Border = new PdfAnnotationBorder(4);
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(popupAnnotation);
        /// //Save the  document to disk.
        /// document.Save("AnnotationBorder.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new popup annotation.
        /// Dim popupAnnotation As PdfPopupAnnotation = New PdfPopupAnnotation(popupAnnotationRectangle, "Test popup annotation")
        /// 'Set the annotation border.
        /// popupAnnotation.Border = new PdfAnnotationBorder(4)
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(popupAnnotation)
        /// 'Save the document to disk.
        /// document.Save("AnnotationBorder.pdf")
        /// </code>
        /// </example> 
        public PdfAnnotationBorder(float borderWidth)
        {
            this.Initialize(borderWidth, this.m_horizontalRadius, this.m_verticalRadius);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfAnnotationBorder"/> class.
        /// </summary>
        /// <param name="borderWidth">A float value specifying the width of the annotation's border.</param>
        /// <param name="horizontalRadius">A float value specifying the horizontal corner radius value.</param>
        /// <param name="verticalRadius">A float value specifying the vertical corner radius value.</param>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new Pdf Document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new popup annotation.
        /// PdfPopupAnnotation popupAnnotation = new PdfPopupAnnotation(popupAnnotationRectangle,"Test popup annotation");
        /// //Set the annotation border.
        /// popupAnnotation.Border = new PdfAnnotationBorder(4,0,0);
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(popupAnnotation);
        /// //Save the  document to disk.
        /// document.Save("AnnotationBorder.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new popup annotation.
        /// Dim popupAnnotation As PdfPopupAnnotation = New PdfPopupAnnotation(popupAnnotationRectangle, "Test popup annotation")
        /// 'Set the annotation border.
        /// popupAnnotation.Border = new PdfAnnotationBorder(4,0,0)
        /// 'Add popup annotation annotation to a new page.
        /// page.Annotations.Add(popupAnnotation)
        /// 'Save the document to disk.
        /// document.Save("AnnotationBorder.pdf")
        /// </code>
        /// </example> 
        public PdfAnnotationBorder(float borderWidth, float horizontalRadius, float verticalRadius)
        {
            this.Initialize(borderWidth, horizontalRadius, verticalRadius);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Initializes instance.
        /// </summary>
        /// <param name="borderWidth">Width of the border.</param>
        /// <param name="horizontalRadius">The horizontal radius.</param>
        /// <param name="verticalRadius">The vertical radius.</param>
        private void Initialize(float borderWidth, float horizontalRadius, float verticalRadius)
        {
            this.m_array.Add(new PdfNumber(horizontalRadius), new PdfNumber(verticalRadius), new PdfNumber(borderWidth));
        }

        /// <summary>
        /// Sets the number.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        private void SetNumber(int index, float value)
        {
            PdfNumber number = this.m_array[index] as PdfNumber;
            number.FloatValue = value;
        }
        #endregion

        #region IPdfWrapper Members
        /// <summary>
        /// Gets Pdf primitive representing this object.
        /// </summary>
        IPdfPrimitive IPdfWrapper.Element
        {
            get
            {
                return this.m_array;
            }
        }
        #endregion
    }
}
