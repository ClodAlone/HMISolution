#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System.Drawing;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents a line annotation. 
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document.
    /// PdfPage page = document.Pages.Add();
    /// //To specify the line end points
    /// int[] points = new int[] { 80, 420, 150, 420 };
    /// //Create a new line annotation.
    /// PdfLineAnnotation lineAnnotation = new PdfLineAnnotation(points, "Line Annoation");
    /// //Create pdf line border
    /// LineBorder lineBorder = new LineBorder();
    /// lineBorder.BorderStyle = PdfBorderStyle.Solid;
    /// lineBorder.BorderWidth = 1;
    /// lineAnnotation.lineBorder = lineBorder;
    /// lineAnnotation.LineIntent = PdfLineIntent.LineDimension;
    /// //Assign the line ending style
    /// lineAnnotation.BeginLineStyle = PdfLineEndingStyle.Butt;
    /// lineAnnotation.EndLineStyle = PdfLineEndingStyle.Diamond;
    /// lineAnnotation.AnnotationFlags = PdfAnnotationFlags.Default;
    /// //Assign the line color
    /// lineAnnotation.InnerLineColor = new PdfColor(Color.Green);
    /// lineAnnotation.BackColor = new PdfColor(Color.Green);
    /// //Assign the leader line
    /// lineAnnotation.LeaderLineExt = 0;
    /// lineAnnotation.LeaderLine = 0;
    /// //Assign the Line caption type
    /// lineAnnotation.LineCaption = true;
    /// lineAnnotation.CaptionType = PdfLineCaptionType.Inline;
    /// //Add this annotation to a new page.
    /// page.Annotations.Add(lineAnnotation);
    /// //Save the document to disk.
    /// document.Save("LineAnnotation.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document.
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'To specify the line end points
    /// Dim points As Integer() = New Integer() {80, 420, 150, 420}
    /// //Create a new line annotation.
    /// Dim lineAnnotation As PdfLineAnnotation = New PdfLineAnnotation(points, "Line Annoation")
    /// 'Create pdf line border
    /// Dim lineBorder As LineBorder = New LineBorder()
    /// lineBorder.BorderStyle = PdfBorderStyle.Solid
    /// 'lineBorder.DashArray = 1;
    /// lineBorder.BorderWidth = 1
    /// lineAnnotation.lineBorder = lineBorder
    /// 'Sets the line indent.
    /// lineAnnotation.LineIntent = PdfLineIntent.LineDimension
    /// 'Assign the line ending style
    /// lineAnnotation.BeginLineStyle = PdfLineEndingStyle.Butt
    /// lineAnnotation.EndLineStyle = PdfLineEndingStyle.Diamond
    /// lineAnnotation.AnnotationFlags = PdfAnnotationFlags.Default
    /// 'Assign the line color
    /// lineAnnotation.InnerLineColor = New PdfColor(Color.Green)
    /// lineAnnotation.BackColor = New PdfColor(Color.Green)
    /// 'Assign the leader line
    /// lineAnnotation.LeaderLineExt = 0
    /// lineAnnotation.LeaderLine = 0
    /// 'Assign the Line caption type
    /// lineAnnotation.LineCaption = True
    /// 'Add this annotation to a new page.
    /// page.Annotations.Add(lineAnnotation)
    /// 'Save the document to disk.
    /// document.Save("LineAnnotation.pdf")
    /// </code>
    /// </example> 
    public class PdfLineAnnotation : PdfAnnotation
    {
        #region Fields
        /// <summary>
        /// Indicates PdfLine Begin style 
        /// </summary>     
        private PdfLineEndingStyle m_beginLine;

        /// <summary>
        /// Indicates PdfLine End style 
        /// </summary>
        private PdfLineEndingStyle m_endLine;

        /// <summary>
        /// To Specify the Line Border
        /// </summary>
        private LineBorder m_lineBorder = new LineBorder();

        /// <summary>
        /// An array of four numbers specifying the starting and ending coordinates
        /// </summary>
        internal PdfArray m_linePoints;

        /// <summary>
        /// An array of two names specifying the line ending styles
        /// </summary>
        internal PdfArray m_lineStyle;

        /// <summary>
        /// To specifying the Inner Line color color with which to fill the annotation’s line endings
        /// </summary>
        private PdfColor m_innerLineColor;

        /// <summary>
        /// To specifying the Background Color
        /// </summary>
        private PdfColor m_backgroundColor;

        /// <summary>
        /// To specifying the Leader Line Extension size
        /// </summary>
        private int m_leaderLineExt = 0;

        /// <summary>
        /// To specifying the Leader Line size
        /// </summary>
        private int m_leaderLine = 0;

        /// <summary>
        /// To specifying the caption in the appearance of the line,
        /// </summary>
        private bool m_lineCaption;

        /// <summary>
        /// To specifying the intent of the line annotation
        /// </summary>
        private PdfLineIntent m_lineIntent;

        /// <summary>
        /// To specifying Caption Type
        /// </summary>
        public PdfLineCaptionType m_captionType;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets whether the line annotation caption should be displayed.
        /// </summary>
        /// <value><c>true</c> if the line caption should be displayed, otherwise <c>false</c>.</value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new pdf document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //To specify the line end points
        /// int[] points = new int[] { 80, 420, 150, 420 };
        /// //Create a new line annotation.
        /// PdfLineAnnotation lineAnnotation = new PdfLineAnnotation(points, "Line Annoation");
        /// //Set the line caption.
        /// lineAnnotation.LineCaption = true;
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(lineAnnotation);
        /// //Save the document to disk.
        /// document.Save("LineAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'To specify the line end points
        /// Dim points As Integer() = New Integer() {80, 420, 150, 420}
        /// 'Create a new line annotation.
        /// Dim lineAnnotation As PdfLineAnnotation = New PdfLineAnnotation(points, "Line Annoation")
        /// 'Set the line caption.
        /// lineAnnotation.LineCaption = True
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(lineAnnotation)
        /// 'Save the document to disk.
        /// document.Save("LineAnnotation.pdf")
        /// </code>
        /// </example> 
        public bool LineCaption
        {
            get
            {
                return this.m_lineCaption;
            }

            set
            {
                this.m_lineCaption = value;
            }
        }

        /// <summary>
        /// Gets or sets Leader Line 
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //To specify the line end points
        /// int[] points = new int[] { 80, 420, 150, 420 };
        /// //Create a new line annotation.
        ///  PdfLineAnnotation lineAnnotation = new PdfLineAnnotation(points, "Line Annoation");
        /// //Set the leader line.
        /// lineAnnotation.LeaderLine = 10;
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(lineAnnotation);
        /// //Save the document to disk.
        /// document.Save("LeaderLine.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'To specify the line end points
        /// Dim points As Integer() = New Integer() {80, 420, 150, 420}
        /// 'Create a new line annotation.
        /// Dim lineAnnotation As PdfLineAnnotation = New PdfLineAnnotation(points, "Line Annoation")
        /// 'Set the LeaderLine to popupAnnotation.
        /// lineAnnotation.LeaderLine = 10
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(lineAnnotation)
        /// 'Save the document to disk.
        /// document.Save("LeaderLine.pdf")
        /// </code>
        /// </example> 
        public int LeaderLine
        {
            get
            {
                return this.m_leaderLine;
            }

            set
            {
                if (this.m_leaderLineExt != 0)
                {
                    this.m_leaderLine = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets Leader Line Extension
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //To specify the line end points
        /// int[] points = new int[] { 80, 420, 150, 420 };
        /// //Create a new line annotation.
        ///  PdfLineAnnotation lineAnnotation = new PdfLineAnnotation(points, "Line Annoation");
        /// //Set the leader line extension.
        /// lineAnnotation.LeaderLineExt = 10;
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(lineAnnotation);
        /// //Save the document to disk.
        /// document.Save("LeaderLineExt.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'To specify the line end points
        /// Dim points As Integer() = New Integer() {80, 420, 150, 420}
        /// 'Create a new line annotation.
        /// Dim lineAnnotation As PdfLineAnnotation = New PdfLineAnnotation(points, "Line Annoation")
        /// 'Set the leader line extension.
        /// lineAnnotation.LeaderLineExt = 10
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(lineAnnotation)
        /// 'Save the document to disk.
        /// document.Save("LeaderLineExt.pdf")
        /// </code>
        /// </example> 
        public int LeaderLineExt
        {
            get
            {
                return this.m_leaderLineExt;
            }

            set
            {
                this.m_leaderLineExt = value;
            }
        }

        /// <summary>
        /// Gets or sets Border style of the Line Annotation.
        /// </summary>
        /// <value>A <see cref="LineBorder"/> enumeration member specifying the border style for the line.</value>
        /// <example>
        /// <code lang = "C#">
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //To specify the line end points
        /// int[] points = new int[] { 80, 420, 150, 420 };
        /// //Create a new line annotation.
        /// PdfLineAnnotation lineAnnotation = new PdfLineAnnotation(points, "Line Annoation");
        /// //Create pdf line border
        /// LineBorder lineBorder = new LineBorder();
        /// lineBorder.BorderStyle = PdfBorderStyle.Solid;
        /// lineBorder.BorderWidth = 1;
        /// lineAnnotation.lineBorder = lineBorder;
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(lineAnnotation);
        /// //Save the document to disk.
        /// document.Save("LineAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'To specify the line end points
        /// Dim points As Integer() = New Integer() {80, 420, 150, 420}
        /// 'Create a new line annotation.
        /// Dim lineAnnotation As PdfLineAnnotation = New PdfLineAnnotation(points, "Line Annoation")
        /// 'Create pdf line border
        /// Dim lineBorder As LineBorder = New LineBorder()
        /// lineBorder.BorderStyle = PdfBorderStyle.Solid
        /// 'lineBorder.DashArray = 1
        /// lineBorder.BorderWidth = 1
        /// lineAnnotation.lineBorder = lineBorder
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(lineAnnotation)
        /// 'Save the document to disk.
        /// document.Save("LineAnnotation.pdf")
        /// </code>
        /// </example> 
        public LineBorder lineBorder
        {
            get
            {
                return this.m_lineBorder;
            }

            set
            {
                this.m_lineBorder = value;
            }
        }

        /// <summary>
        /// Gets or sets the style used for the beginning of the line. 
        /// </summary>
        /// <value>A <see cref="PdfLineEndingStyle"/> enumeration member specifying the begin style for the line.</value>
        /// <example>
        /// <code lang = "C#">
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //To specify the line end points
        /// int[] points = new int[] { 80, 420, 150, 420 };
        /// //Create a new line annotation.
        /// PdfLineAnnotation lineAnnotation = new PdfLineAnnotation(points, "Line Annoation");
        /// //Sets the begin line style.
        /// lineAnnotation.BeginLineStyle = PdfLineEndingStyle.Butt;
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(lineAnnotation);
        /// //Save the document to disk.
        /// document.Save("LineAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'To specify the line end points
        /// Dim points As Integer() = New Integer() {80, 420, 150, 420}
        /// 'Create a new line annotation.
        /// Dim lineAnnotation As PdfLineAnnotation = New PdfLineAnnotation(points, "Line Annoation")
        /// 'Assign the line ending style
        /// lineAnnotation.BeginLineStyle = PdfLineEndingStyle.Butt
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(lineAnnotation)
        /// 'Save the document to disk.
        /// document.Save("LineAnnotation.pdf")
        /// </code>
        /// </example> 
        public PdfLineEndingStyle BeginLineStyle
        {
            get
            {
                return this.m_beginLine;
            }

            set
            {
                if (this.m_beginLine != value)
                {
                    this.m_beginLine = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the style used for the end of the line. 
        /// </summary>
        /// <value>A <see cref="PdfLineEndingStyle"/> enumeration member specifying the end style for the line.</value>
        /// <example>
        /// <code lang = "C#">
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //To specify the line end points
        /// int[] points = new int[] { 80, 420, 150, 420 };
        /// //Create a new line annotation.
        /// PdfLineAnnotation lineAnnotation = new PdfLineAnnotation(points, "Line Annoation");
        /// //Sets the line ending style.
        /// lineAnnotation.EndLineStyle = PdfLineEndingStyle.Diamond;
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(lineAnnotation);
        /// //Save the document to disk.
        /// document.Save("LineAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// To specify the line end points
        /// Dim points As Integer() = New Integer() {80, 420, 150, 420}
        /// 'Create a new line annotation.
        /// Dim lineAnnotation As PdfLineAnnotation = New PdfLineAnnotation(points, "Line Annoation")
        /// 'Sets the line ending style.
        /// lineAnnotation.EndLineStyle = PdfLineEndingStyle.Diamond
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(lineAnnotation)
        /// 'Save the document to disk.
        /// document.Save("LineAnnotation.pdf")
        /// </code>
        /// </example> 
        public PdfLineEndingStyle EndLineStyle
        {
            get
            {
                return this.m_endLine;
            }

            set
            {
                if (this.m_endLine != value)
                {
                    this.m_endLine = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the line caption text type.
        /// </summary>
        /// <value>A <see cref="PdfLineCaptionType"/> enumeration member specifying the line caption type.</value>
        /// <example>
        /// <code lang = "C#">
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //To specify the line end points
        /// int[] points = new int[] { 80, 420, 150, 420 };
        /// //Create a new line annotation.
        /// PdfLineAnnotation lineAnnotation = new PdfLineAnnotation(points, "Line Annoation");
        /// //Sets the line caption type.
        /// lineAnnotation.CaptionType = PdfLineCaptionType.Inline;
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(lineAnnotation);
        /// //Save the document to disk.
        /// document.Save("LineAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'To specify the line end points
        /// Dim points As Integer() = New Integer() {80, 420, 150, 420}
        /// 'Create a new line annotation.
        /// Dim lineAnnotation As PdfLineAnnotation = New PdfLineAnnotation(points, "Line Annoation")
        /// 'Sets the line caption type.
        /// lineAnnotation.CaptionType = PdfLineCaptionType.Inline
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(lineAnnotation)
        /// 'Save the document to disk.
        /// document.Save("LineAnnotation.pdf")
        /// </code>
        /// </example> 
        public PdfLineCaptionType CaptionType
        {
            get
            {
                return this.m_captionType;
            }

            set
            {
                this.m_captionType = value;
            }
        }

        /// <summary>
        /// Gets or sets LineIntent
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //To specify the line end points
        /// int[] points = new int[] { 80, 420, 150, 420 };
        /// //Create a new line annotation.
        /// PdfLineAnnotation lineAnnotation = new PdfLineAnnotation(points, "Line Annoation");
        /// //Sets the line intent.
        ///  ineAnnotation.LineIntent = PdfLineIntent.LineDimension;
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(lineAnnotation);
        /// //Save the document to disk.
        /// document.Save("LineAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'To specify the line end points
        /// Dim points As Integer() = New Integer() {80, 420, 150, 420}
        /// 'Create a new line annotation.
        /// Dim lineAnnotation As PdfLineAnnotation = New PdfLineAnnotation(points, "Line Annoation")
        /// 'Sets the line intent.
        /// lineAnnotation.LineIntent = PdfLineIntent.LineDimension
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(lineAnnotation)
        /// 'Save the document to disk.
        /// document.Save("LineAnnotation.pdf")
        /// </code>
        /// </example> 
        public PdfLineIntent LineIntent
        {
            get
            {
                return this.m_lineIntent;
            }

            set
            {
                this.m_lineIntent = value;
            }
        }

        /// <summary>
        /// Gets or sets Inner Color of the PdfLine
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //To specify the line end points
        /// int[] points = new int[] { 80, 420, 150, 420 };
        /// //Create a new line annotation.
        /// PdfLineAnnotation lineAnnotation = new PdfLineAnnotation(points, "Line Annoation");
        /// //Sets the inner line color.
        /// linkannotation.InnerLineColor = new PdfColor(Color.Green);
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(lineAnnotation);
        /// //Save the document to disk.
        /// document.Save("LineAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'To specify the line end points
        /// Dim points As Integer() = New Integer() {80, 420, 150, 420}
        /// 'Create a new line annotation.
        /// Dim lineAnnotation As PdfLineAnnotation = New PdfLineAnnotation(points, "Line Annoation")
        /// 'Sets the inner line color.
        /// linkannotation.InnerLineColor = new PdfColor(Color.Green)
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(lineAnnotation)
        /// 'Save the document to disk.
        /// document.Save("LineAnnotation.pdf")
        /// </code>
        /// </example> 
        public PdfColor InnerLineColor
        {
            get
            {
                return this.m_innerLineColor;
            }

            set
            {
                this.m_innerLineColor = value;
            }
        }

        /// <summary>
        /// Gets or sets Background Color of the PdfLine
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //To specify the line end points
        /// int[] points = new int[] { 80, 420, 150, 420 };
        /// //Create a new line annotation.
        /// PdfLineAnnotation lineAnnotation = new PdfLineAnnotation(points, "Line Annoation");
        /// //Sets the line back color.
        /// linkannotation.BackColor = new PdfColor(Color.Green);
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(lineAnnotation);
        /// //Save the document to disk.
        /// document.Save("LineAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'To specify the line end points
        /// Dim points As Integer() = New Integer() {80, 420, 150, 420}
        /// 'Create a new line annotation.
        /// Dim lineAnnotation As PdfLineAnnotation = New PdfLineAnnotation(points, "Line Annoation")
        /// 'Sets the line back color.
        /// linkannotation.BackColor = new PdfColor(Color.Green)
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(lineAnnotation)
        /// 'Save the document to disk.
        /// document.Save("LineAnnotation.pdf")
        /// </code>
        /// </example> 
        public PdfColor BackColor
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
        #endregion

        #region constructor
        /// <summary>
        /// Initializes new instance of <see cref="PdfLineAnnotation"/> class.
        /// </summary>
        /// <param name="linePoints">The line points.</param>
        /// <example>
        /// <code lang = "C#">
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //To specify the line end points
        /// int[] points = new int[] { 80, 420, 150, 420 };
        /// //Create a new line annotation.
        /// PdfLineAnnotation lineAnnotation = new PdfLineAnnotation(points);
        /// //Create pdf line border
        /// LineBorder lineBorder = new LineBorder();
        /// lineBorder.BorderStyle = PdfBorderStyle.Solid;
        /// lineBorder.BorderWidth = 1;
        /// lineAnnotation.lineBorder = lineBorder;
        /// lineAnnotation.LineIntent = PdfLineIntent.LineDimension;
        /// //Assign the line ending style
        /// lineAnnotation.BeginLineStyle = PdfLineEndingStyle.Butt;
        /// lineAnnotation.EndLineStyle = PdfLineEndingStyle.Diamond;
        /// lineAnnotation.AnnotationFlags = PdfAnnotationFlags.Default;
        /// //Assign the line color
        /// lineAnnotation.InnerLineColor = new PdfColor(Color.Green);
        /// lineAnnotation.BackColor = new PdfColor(Color.Green);
        /// //Assign the leader line
        /// lineAnnotation.LeaderLineExt = 0;
        /// lineAnnotation.LeaderLine = 0;
        /// //Assign the line caption
        /// lineAnnotation.LineCaption = true;
        /// lineAnnotation.CaptionType = PdfLineCaptionType.Inline;
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(lineAnnotation);
        /// //Save the document to disk.
        /// document.Save("LineAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'To specify the line end points
        /// Dim points As Integer() = New Integer() {80, 420, 150, 420}
        /// 'Create a new line annotation.
        /// Dim lineAnnotation As PdfLineAnnotation = New PdfLineAnnotation(points)
        /// 'Create pdf line border
        /// Dim lineBorder As LineBorder = New LineBorder()
        /// lineBorder.BorderStyle = PdfBorderStyle.Solid
        /// 'lineBorder.DashArray = 1;
        /// lineBorder.BorderWidth = 1
        /// lineAnnotation.lineBorder = lineBorder
        /// lineAnnotation.LineIntent = PdfLineIntent.LineDimension
        /// 'Assign the line ending style
        /// lineAnnotation.BeginLineStyle = PdfLineEndingStyle.Butt
        /// lineAnnotation.EndLineStyle = PdfLineEndingStyle.Diamond
        /// lineAnnotation.AnnotationFlags = PdfAnnotationFlags.Default
        /// 'Assign the line color
        /// lineAnnotation.InnerLineColor = New PdfColor(Color.Green)
        /// lineAnnotation.BackColor = New PdfColor(Color.Green)
        /// 'Assign the leader line
        /// lineAnnotation.LeaderLineExt = 0
        /// lineAnnotation.LeaderLine = 0
        /// 'Assign the line caption type.
        /// lineAnnotation.LineCaption = True
        /// lineAnnotation.CaptionType = PdfLineCaptionType.Inline
        /// 'Set the PopupIcon to popupAnnotation.
        ///  lineAnnotation.EndLineStyle = PdfLineEndingStyle.Diamond
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(lineAnnotation)
        /// 'Save the document to disk.
        /// document.Save("LineAnnotation.pdf")
        /// </code>
        /// </example> 
        public PdfLineAnnotation(int[] linePoints)
            : base()
        {
            this.m_linePoints = new PdfArray(linePoints);
        }

        /// <summary>
        /// Initializes new instance of <see cref="PdfLineAnnotation"/> class.
        /// </summary>
        /// <param name="linePoints">The line points.</param>
        /// <param name="text">The line caption text.</param>
        /// <example>
        /// <code lang = "C#">
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //To specify the line end points
        /// int[] points = new int[] { 80, 420, 150, 420 };
        /// //Create a new line annotation.
        /// PdfLineAnnotation lineAnnotation = new PdfLineAnnotation(points, "Line Annoation");
        /// //Create pdf line border
        /// LineBorder lineBorder = new LineBorder();
        /// lineBorder.BorderStyle = PdfBorderStyle.Solid;
        /// lineBorder.BorderWidth = 1;
        /// lineAnnotation.lineBorder = lineBorder;
        /// lineAnnotation.LineIntent = PdfLineIntent.LineDimension;
        /// //Assign the line ending style
        /// lineAnnotation.BeginLineStyle = PdfLineEndingStyle.Butt;
        /// lineAnnotation.EndLineStyle = PdfLineEndingStyle.Diamond;
        /// lineAnnotation.AnnotationFlags = PdfAnnotationFlags.Default;
        /// //Assign the line color
        /// lineAnnotation.InnerLineColor = new PdfColor(Color.Green);
        /// lineAnnotation.BackColor = new PdfColor(Color.Green);
        /// //Assign the leader line
        /// lineAnnotation.LeaderLineExt = 0;
        /// lineAnnotation.LeaderLine = 0;
        /// //Assign the Line caption type
        /// lineAnnotation.LineCaption = true;
        /// lineAnnotation.CaptionType = PdfLineCaptionType.Inline;
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(lineAnnotation);
        /// //Save the document to disk.
        /// document.Save("LineAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// //To specify the line end points
        /// Dim points As Integer() = New Integer() {80, 420, 150, 420}
        /// //Create a new line annotation.
        /// Dim lineAnnotation As PdfLineAnnotation = New PdfLineAnnotation(points, "Line Annoation")
        /// 'Create pdf line border
        /// Dim lineBorder As LineBorder = New LineBorder()
        /// lineBorder.BorderStyle = PdfBorderStyle.Solid
        /// 'lineBorder.DashArray = 1;
        /// lineBorder.BorderWidth = 1
        /// lineAnnotation.lineBorder = lineBorder
        /// lineAnnotation.LineIntent = PdfLineIntent.LineDimension
        /// 'Assign the line ending style
        /// lineAnnotation.BeginLineStyle = PdfLineEndingStyle.Butt
        /// lineAnnotation.EndLineStyle = PdfLineEndingStyle.Diamond
        /// lineAnnotation.AnnotationFlags = PdfAnnotationFlags.Default
        /// 'Assign the line color
        /// lineAnnotation.InnerLineColor = New PdfColor(Color.Green)
        /// lineAnnotation.BackColor = New PdfColor(Color.Green)
        /// 'Assign the leader line
        /// lineAnnotation.LeaderLineExt = 0
        /// lineAnnotation.LeaderLine = 0
        /// 'Assign the Line caption type
        /// lineAnnotation.LineCaption = True
        /// lineAnnotation.CaptionType = PdfLineCaptionType.Inline
        /// //Set the line ending style.
        /// lineAnnotation.EndLineStyle = PdfLineEndingStyle.Diamond
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(lineAnnotation)
        /// //Save the document to disk.
        /// document.Save("LineAnnotation.pdf")
        /// </code>
        /// </example> 
        public PdfLineAnnotation(int[] linePoints, string text)
            : base()
        {
            this.m_linePoints = new PdfArray(linePoints);
            Text = text;
        }

        /// <summary>
        /// Initializes new instance of <see cref="PdfLineAnnotation"/> class.
        /// </summary>
        /// <param name="rectangle">Bounds of the annotation.</param>
        /// <example>
        /// <code lang = "C#">
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //To specify the line end points
        /// int[] points = new int[] { 80, 420, 150, 420 };
        /// //Create a new line annotation.
        /// PdfLineAnnotation lineAnnotation = new PdfLineAnnotation(new RectangleF(80, 420, 150, 420));
        /// //Create pdf line border
        /// LineBorder lineBorder = new LineBorder();
        /// lineBorder.BorderStyle = PdfBorderStyle.Solid;
        /// lineBorder.BorderWidth = 1;
        /// lineAnnotation.lineBorder = lineBorder;
        /// lineAnnotation.LineIntent = PdfLineIntent.LineDimension;
        /// //Assign the line ending style
        /// lineAnnotation.BeginLineStyle = PdfLineEndingStyle.Butt;
        /// lineAnnotation.EndLineStyle = PdfLineEndingStyle.Diamond;
        /// lineAnnotation.AnnotationFlags = PdfAnnotationFlags.Default;
        /// //Assign the line color
        /// lineAnnotation.InnerLineColor = new PdfColor(Color.Green);
        /// lineAnnotation.BackColor = new PdfColor(Color.Green);
        /// //Assign the leader line
        /// lineAnnotation.LeaderLineExt = 0;
        /// lineAnnotation.LeaderLine = 0;
        /// //Assign the Line caption type
        /// lineAnnotation.LineCaption = true;
        /// lineAnnotation.CaptionType = PdfLineCaptionType.Inline;
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(lineAnnotation);
        /// //Save the document to disk.
        /// document.Save("LineAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'To specify the line end points
        /// Dim points As Integer() = New Integer() {80, 420, 150, 420}
        /// //Create a new line annotation.
        /// Dim lineAnnotation As PdfLineAnnotation = New PdfLineAnnotation(New RectangleF(80, 420, 150, 420))
        /// 'Create pdf line border
        /// Dim lineBorder As LineBorder = New LineBorder()
        /// lineBorder.BorderStyle = PdfBorderStyle.Solid
        /// 'lineBorder.DashArray = 1;
        /// lineBorder.BorderWidth = 1
        /// lineAnnotation.lineBorder = lineBorder
        /// lineAnnotation.LineIntent = PdfLineIntent.LineDimension
        /// 'Assign the line ending style
        /// lineAnnotation.BeginLineStyle = PdfLineEndingStyle.Butt
        /// lineAnnotation.EndLineStyle = PdfLineEndingStyle.Diamond
        /// lineAnnotation.AnnotationFlags = PdfAnnotationFlags.Default
        /// 'Assign the line color
        /// lineAnnotation.InnerLineColor = New PdfColor(Color.Green)
        /// lineAnnotation.BackColor = New PdfColor(Color.Green)
        /// 'Assign the leader line
        /// lineAnnotation.LeaderLineExt = 0
        /// lineAnnotation.LeaderLine = 0
        /// 'Assign the Line caption type
        /// lineAnnotation.LineCaption = True
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(lineAnnotation)
        /// 'Save the document to disk.
        /// document.Save("LineAnnotation.pdf")
        /// </code>
        /// </example> 
        public PdfLineAnnotation(RectangleF rectangle)
            : base(rectangle)
        {
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Initializes annotation object.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            Dictionary.SetProperty(DictionaryProperties.Subtype, new PdfName(DictionaryProperties.Line));
        }

        /// <summary>
        /// Saves an annotation.
        /// </summary>
        protected override void Save()
        {
            base.Save();
            this.m_lineStyle = new PdfArray();
            this.m_lineStyle.Insert(0, new PdfName(this.BeginLineStyle));
            this.m_lineStyle.Insert(1, new PdfName(this.EndLineStyle));
            Dictionary.SetProperty(DictionaryProperties.LE, this.m_lineStyle);
            Dictionary.SetProperty(DictionaryProperties.L, this.m_linePoints);
            Dictionary.SetProperty(DictionaryProperties.BS, this.m_lineBorder);
            float red = this.InnerLineColor.R / 255f;
            float green = this.InnerLineColor.G / 255f;
            float blue = this.InnerLineColor.B / 255f;
            PdfArray m_innercolor = new PdfArray();
            m_innercolor.Insert(0, new PdfNumber(red));
            m_innercolor.Insert(1, new PdfNumber(green));
            m_innercolor.Insert(2, new PdfNumber(blue));
            Dictionary.SetProperty(DictionaryProperties.IC, m_innercolor);

            PdfArray m_bgcolor = new PdfArray();
            m_bgcolor.Insert(0, new PdfNumber(this.m_backgroundColor.R / 255f));
            m_bgcolor.Insert(1, new PdfNumber(this.m_backgroundColor.G / 255f));
            m_bgcolor.Insert(2, new PdfNumber(this.m_backgroundColor.B / 255f));
            Dictionary[DictionaryProperties.C] = new PdfArray(m_bgcolor);

            Dictionary.SetProperty(DictionaryProperties.IT, new PdfName(this.m_lineIntent));

            Dictionary.SetProperty(DictionaryProperties.LLE, new PdfNumber(this.m_leaderLineExt));

            Dictionary.SetProperty(DictionaryProperties.LL, new PdfNumber(this.m_leaderLine));

            Dictionary.SetProperty(DictionaryProperties.CP, new PdfName(this.m_captionType));

            Dictionary.SetProperty(DictionaryProperties.Cap, new PdfBoolean(this.m_lineCaption));

        }
        #endregion
    }
}
