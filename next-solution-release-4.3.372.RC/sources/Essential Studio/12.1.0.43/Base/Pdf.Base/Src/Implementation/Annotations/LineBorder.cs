#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents the border style of the Line annotation.
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document.
    /// PdfPage page = document.Pages.Add();
    /// //To specify the line end points
    /// int[] points = new int[] { 80, 420, 150, 420 };
    /// //Create a new line annotation.
    /// PdfLineAnnotation lineannotation = new PdfLineAnnotation(points, "Line Annoation");
    /// //Create pdf line border
    /// LineBorder lineBorder = new LineBorder();
    /// lineBorder.BorderStyle = PdfBorderStyle.Solid;
    /// lineBorder.BorderWidth = 1;
    /// lineannotation.lineBorder = lineBorder;
    /// //Add this annotation to a new page.
    /// page.Annotations.Add(lineannotation);
    /// //Save document to disk.
    /// document.Save("LineBorder.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new pdf document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document.
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'To specify the line end points
    /// Dim points As Integer() = New Integer() {80, 420, 150, 420}
    /// 'Create a new line annotation.
    /// Dim lineannotation As PdfLineAnnotation = New PdfLineAnnotation(points, "Line Annoation")
    /// 'Create pdf line border
    /// Dim lineBorder As LineBorder = New LineBorder()
    /// lineBorder.BorderStyle = PdfBorderStyle.Solid
    /// 'lineBorder.DashArray = 1;
    /// lineBorder.BorderWidth = 1
    /// lineannotation.lineBorder = lineBorder
    /// 'Add this annotation to a new page.
    /// page.Annotations.Add(lineannotation)
    /// 'Save document to disk.
    /// document.Save("LineBorder.pdf")
    /// </code>
    /// </example> 
    public class LineBorder : IPdfWrapper
    {
        #region Fields
        /// <summary>
        /// Internal variable to store border width.
        /// </summary>
        private int m_borderWidth;

        /// <summary>
        /// Internal variable to store Border Dash.
        /// </summary>
        private int m_dashArray;

        /// <summary>
        /// Internal variable to store border style;
        /// </summary>
        private PdfBorderStyle m_borderStyle;

        /// <summary>
        /// Internal variable to store dictionary.
        /// </summary>
        private PdfDictionary m_dictionary = new PdfDictionary();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The line border width.</value>
        /// <example>
        /// <code lang = "C#">
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //To specify the line end points
        /// int[] points = new int[] { 80, 420, 150, 420 };
        /// //Create a new line annotation.
        /// PdfLineAnnotation lineannotation = new PdfLineAnnotation(points, "Line Annoation");
        /// //Create pdf line border
        /// LineBorder lineBorder = new LineBorder();
        /// lineBorder.BorderWidth = 1;
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(lineannotation);
        /// //Save document to disk.
        /// document.Save("LineBorder.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new pdf document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'To specify the line end points
        /// Dim points As Integer() = New Integer() {80, 420, 150, 420}
        /// 'Create a new line annotation.
        /// Dim lineannotation As PdfLineAnnotation = New PdfLineAnnotation(points, "Line Annoation")
        /// 'Create pdf line border
        /// Dim lineBorder As LineBorder = New LineBorder()
        /// lineBorder.BorderWidth = 1
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(lineannotation)
        /// 'Save document to disk.
        /// document.Save("LineBorder.pdf")
        /// </code>
        /// </example> 
        public int BorderWidth
        {
            get
            {
                return this.m_borderWidth;
            }

            set
            {
                this.m_borderWidth = value;
                this.m_dictionary.SetNumber(DictionaryProperties.W, this.m_borderWidth);
            }
        }

        /// <summary>
        /// Gets or sets the border style.
        /// </summary>
        /// <value>The line border style.</value>
        /// <example>
        /// <code lang = "C#">
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //To specify the line end points
        /// int[] points = new int[] { 80, 420, 150, 420 };
        /// //Create a new line annotation.
        /// PdfLineAnnotation lineannotation = new PdfLineAnnotation(points, "Line Annoation");
        /// //Create pdf line border
        /// LineBorder lineBorder = new LineBorder();
        /// lineBorder.BorderStyle = PdfBorderStyle.Solid;
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(lineannotation);
        /// //Save document to disk.
        /// document.Save("LineBorder.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new pdf document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'To specify the line end points
        /// Dim points As Integer() = New Integer() {80, 420, 150, 420}
        /// 'Create a new line annotation.
        /// Dim lineannotation As PdfLineAnnotation = New PdfLineAnnotation(points, "Line Annoation")
        /// 'Create pdf line border
        /// Dim lineBorder As LineBorder = New LineBorder()
        /// lineBorder.BorderStyle = PdfBorderStyle.Solid
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(lineannotation)
        /// 'Save document to disk.
        /// document.Save("LineBorder.pdf")
        /// </code>
        /// </example> 
        public PdfBorderStyle BorderStyle
        {
            get
            {
                return this.m_borderStyle;
            }

            set
            {
                this.m_borderStyle = value;
                this.m_dictionary.SetName(DictionaryProperties.S, this.StyleToString(this.m_borderStyle));
            }
        }

        /// <summary>
        /// Gets or sets the Line Dash
        /// </summary>
        /// <value>The line border dash array.</value>
        /// <example>
        /// <code lang = "C#">
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //To specify the line end points
        /// int[] points = new int[] { 80, 420, 150, 420 };
        /// //Create a new line annotation.
        /// PdfLineAnnotation lineannotation = new PdfLineAnnotation(points, "Line Annoation");
        /// //Create pdf line border
        /// LineBorder lineBorder = new LineBorder();
        /// lineBorder.DashArray = 1;
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(lineannotation);
        /// //Save document to disk.
        /// document.Save("LineBorder.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new pdf document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'To specify the line end points
        /// Dim points As Integer() = New Integer() {80, 420, 150, 420}
        /// //Create a new line annotation.
        /// Dim lineannotation As PdfLineAnnotation = New PdfLineAnnotation(points, "Line Annoation")
        /// 'Create pdf line border
        /// Dim lineBorder As LineBorder = New LineBorder()
        /// 'lineBorder.DashArray = 1
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(lineannotation)
        /// 'Save document to disk.
        /// document.Save("LineBorder.pdf")
        /// </code>
        /// </example> 
        public int DashArray
        {
            get
            {
                return this.m_dashArray;
            }

            set
            {
                this.m_dashArray = value;
                PdfArray m_dasharray = new PdfArray();
                m_dasharray.Insert(0, new PdfNumber(this.m_dashArray));
                m_dasharray.Insert(1, new PdfNumber(this.m_dashArray));
                this.m_dictionary.SetProperty(DictionaryProperties.D, m_dasharray);
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="LineBorder"/> class.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //To specify the line end points
        /// int[] points = new int[] { 80, 420, 150, 420 };
        /// //Create a new line annotation.
        /// PdfLineAnnotation lineannotation = new PdfLineAnnotation(points, "Line Annoation");
        /// //Create pdf line border
        /// LineBorder lineBorder = new LineBorder();
        /// lineBorder.BorderStyle = PdfBorderStyle.Solid;
        /// lineBorder.BorderWidth = 1;
        /// lineannotation.lineBorder = lineBorder;
        /// </code>
        /// <code lang="VB">
        /// 'Create a new pdf document.
        /// 'Create a new line annotation.
        /// Dim points As Integer() = New Integer() {80, 420, 150, 420}
        /// Dim lineannotation As PdfLineAnnotation = New PdfLineAnnotation(points, "Line Annoation")
        /// 'Create pdf line border
        /// Dim lineBorder As LineBorder = New LineBorder()
        /// lineBorder.BorderStyle = PdfBorderStyle.Solid
        /// 'lineBorder.DashArray = 1
        /// lineBorder.BorderWidth = 1
        /// lineannotation.lineBorder = lineBorder
        /// </code>
        /// </example> 
        public LineBorder()
            : base()
        {
            this.m_dictionary.SetProperty(DictionaryProperties.Type, new PdfName(DictionaryProperties.Border));
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Converts border style to string.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <returns></returns>
        private string StyleToString(PdfBorderStyle style)
        {
            switch (style)
            {
                case PdfBorderStyle.Solid:
                default:
                    return "S";

                case PdfBorderStyle.Beveled:
                    return "B";

                case PdfBorderStyle.Dashed:
                    return "D";

                case PdfBorderStyle.Inset:
                    return "I";

                case PdfBorderStyle.Underline:
                    return "U";
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
