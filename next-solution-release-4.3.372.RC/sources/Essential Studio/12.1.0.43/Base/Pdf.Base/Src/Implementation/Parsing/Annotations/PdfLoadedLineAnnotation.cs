#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
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
    /// Represents the loaded line annotation class.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Load an existing document.
    /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
    /// //Gets the annotation from loaded document.
    /// PdfLoadedLineAnnotation lineAnnotation = document.Pages[1].Annotations[5] as PdfLoadedLineAnnotation;
    /// int[] points = new int[] { 100, 100, 200, 100 };
    /// //Create a new pdf line border.
    /// LineBorder lineBorder = new LineBorder();
    /// lineBorder.BorderStyle = PdfBorderStyle.Solid;
    /// lineBorder.DashArray = 1;
    /// lineBorder.BorderWidth =3;
    /// lineBorder.DashArray = 8;
    /// //Sets the line border.
    /// lineAnnotation.LineBorder = lineBorder;
    /// //Sets the line indent.
    /// lineAnnotation.LineIntent = PdfLineIntent.LineArrow;
    /// //Assign the line ending style
    /// lineAnnotation.BeginLineStyle = PdfLineEndingStyle.Slash;
    /// lineAnnotation.EndLineStyle = PdfLineEndingStyle.Circle;
    /// lineAnnotation.AnnotationFlags = PdfAnnotationFlags.Locked;
    /// //Assign the line color
    /// lineAnnotation.InnerLineColor = new PdfColor(Color.Blue);
    /// lineAnnotation.BackColor = new PdfColor(Color.Red);
    /// //Assign the leader line
    /// lineAnnotation.LeaderExt = 20;
    /// lineAnnotation.LeaderLine = 20;
    /// lineAnnotation.Size = new SizeF(100, 200);
    /// //Assign the line caption
    /// lineAnnotation.LineCaption = true;
    /// lineAnnotation.Text = "Syncfusion";
    /// lineAnnotation.CaptionType = PdfLineCaptionType.Top;
    /// //Save the document.
    /// document.Save("lineAnnotation.pdf");
    /// </code>
    /// <code lang="VB">
    ///   'Load an existing document.
    /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
    /// 'Gets the annotation from loaded document.
    /// Dim lineAnnotation As PdfLoadedLineAnnotation = document.Pages(1).Annotations(5) as PdfLoadedLineAnnotation
    /// Dim points As Integer() = { 100, 100, 200, 100 }
    /// 'Create a new pdf line border.
    /// Dim lineBorder As New LineBorder()
    /// lineBorder.BorderStyle = PdfBorderStyle.Solid
    /// lineBorder.DashArray = 1
    /// lineBorder.BorderWidth = 3
    /// lineBorder.DashArray = 8
    /// 'Sets the line border
    /// lineAnnotation.LineBorder = lineBorder
    /// 'Sets the line indent
    /// lineAnnotation.LineIntent = PdfLineIntent.LineArrow
    /// 'Assign the line ending style
    /// lineAnnotation.BeginLineStyle = PdfLineEndingStyle.Slash
    /// lineAnnotation.EndLineStyle = PdfLineEndingStyle.Circle
    /// lineAnnotation.AnnotationFlags = PdfAnnotationFlags.Locked
    /// 'Assign the line color
    /// lineAnnotation.InnerLineColor = New PdfColor(Color.Blue)
    /// lineAnnotation.BackColor = New PdfColor(Color.Red)
    /// 'Assign the leader line
    /// lineAnnotation.LeaderExt = 20
    /// lineAnnotation.LeaderLine = 20
    /// lineAnnotation.Size = New SizeF(100, 200)
    /// 'Assign the line caption
    /// lineAnnotation.LineCaption = True
    /// lineAnnotation.Text = "Syncfusion"
    /// lineAnnotation.CaptionType = PdfLineCaptionType.Top
    /// 'Save the document.
    /// document.Save("lineAnnotation.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfLoadedAttachmentAnnotation"/> Class
    /// <seealso cref="PdfLoadedDocumentLinkAnnotation"/> Class
    /// <seealso cref="PdfLoadedFileLinkAnnotation"/> Class
    /// <seealso cref="PdfLoadedPopupAnnotation"/> Class
    /// <seealso cref="PdfLoadedRubberStampAnnotation"/> Class
    /// <seealso cref="PdfLoadedSoundAnnotation"/> Class
    /// <seealso cref="PdfLoadedTextMarkupAnnotation"/> Class
    /// <seealso cref="PdfLoadedTextWebLinkAnnotation"/> Class
    /// <seealso cref="PdfLoadedUriAnnotation"/> Class
    public class PdfLoadedLineAnnotation : PdfLoadedStyledAnnotation
    {
        #region Fields
        /// <summary>
        /// Cross Table
        /// </summary>
        private PdfCrossTable m_crossTable;
        /// <summary>
        /// Indicates the back color.
        /// </summary>
        private PdfColor m_backcolor;
        /// <summary>
        /// Indicates the line border.
        /// </summary>
        private LineBorder m_lineborder;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the back color of the annotation.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
        /// //Gets the annotation from loaded document.
        /// PdfLoadedLineAnnotation lineAnnotation = document.Pages[1].Annotations[5] as PdfLoadedLineAnnotation;
        /// //Sets the line annotation backcolor.
        /// lineAnnotation.BackColor = new PdfColor(Color.Red);
        /// //Save the document.
        /// document.Save("lineAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        ///   'Load an existing document.
        /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
        /// 'Gets the annotation from loaded document.
        /// Dim lineAnnotation As PdfLoadedLineAnnotation = document.Pages(1).Annotations(5) as PdfLoadedLineAnnotation
        /// 'Sets the line annotation backcolor
        /// lineAnnotation.BackColor = New PdfColor(Color.Red)
        /// 'Save the document.
        /// document.Save("lineAnnotation.pdf")
        /// </code>
        /// </example>
        public PdfColor BackColor
        {
            get
            {
                return GetBackColor();
            }
            set
            {
                PdfArray bgcolor = new PdfArray();
                m_backcolor = value;
                bgcolor.Insert(0, new PdfNumber(m_backcolor.R / 255f));
                bgcolor.Insert(1, new PdfNumber(m_backcolor.G / 255f));
                bgcolor.Insert(2, new PdfNumber(m_backcolor.B / 255f));
                Dictionary.SetProperty(DictionaryProperties.C, bgcolor);
            }
        }
        /// <summary>
        /// Gets or sets the begin line style of the annotation.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
        /// //Gets the annotation from loaded document.
        /// PdfLoadedLineAnnotation lineAnnotation = document.Pages[1].Annotations[5] as PdfLoadedLineAnnotation;
        /// //Assign the line ending style
        /// lineAnnotation.BeginLineStyle = PdfLineEndingStyle.Slash;
        /// //Save the document.
        /// document.Save("lineAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document.
        /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
        /// 'Gets the annotation from loaded document.
        /// Dim lineAnnotation As PdfLoadedLineAnnotation = document.Pages(1).Annotations(5) as PdfLoadedLineAnnotation
        /// 'Assign the line ending style
        /// lineAnnotation.BeginLineStyle = PdfLineEndingStyle.Slash
        /// 'Save the document.
        /// document.Save("lineAnnotation.pdf")
        /// </code>
        /// </example>
        public PdfLineEndingStyle BeginLineStyle
        {
            get
            {
                return GetLineStyle(0);
            }
            set
            {
                PdfArray m_lineStyle = GetLineStyle();
                if (m_lineStyle == null)
                {
                    m_lineStyle.Insert(1, new PdfName(PdfLineEndingStyle.Square));
                }
                else
                {
                    m_lineStyle.RemoveAt(0);
                }
                m_lineStyle.Insert(0, new PdfName(GetLineStyle(value.ToString())));

                Dictionary.SetProperty(DictionaryProperties.LE, m_lineStyle);
            }
        }

        /// <summary>
        /// Gets or sets the caption type of the annotation.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
        /// //Gets the annotation from loaded document.
        /// PdfLoadedLineAnnotation lineAnnotation = document.Pages[1].Annotations[5] as PdfLoadedLineAnnotation;
        /// //Sets the line caption type.
        /// lineAnnotation.CaptionType = PdfLineCaptionType.Top;
        /// //Save the document.
        /// document.Save("lineAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document.
        /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
        /// 'Gets the annotation from loaded document.
        /// Dim lineAnnotation As PdfLoadedLineAnnotation = document.Pages(1).Annotations(5) as PdfLoadedLineAnnotation
        /// 'Sets the line caption type.
        /// lineAnnotation.CaptionType = PdfLineCaptionType.Top
        /// 'Save the document.
        /// document.Save("lineAnnotation.pdf")
        /// </code>
        /// </example>
        public PdfLineCaptionType CaptionType
        {
            get
            {
                return GetCaptionType();
            }
            set
            {
                Dictionary.SetProperty(DictionaryProperties.CP, new PdfName(GetCaptionType(value.ToString())));
            }
        }

        /// <summary>
        /// Gets or sets the end line style of the annotation.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
        /// //Gets the annotation from loaded document.
        /// PdfLoadedLineAnnotation lineAnnotation = document.Pages[1].Annotations[5] as PdfLoadedLineAnnotation;
        /// //Assign the line ending style
        /// lineAnnotation.EndLineStyle = PdfLineEndingStyle.Circle;
        /// //Save the document.
        /// document.Save("lineAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document.
        /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
        /// 'Gets the annotation from loaded document.
        /// Dim lineAnnotation As PdfLoadedLineAnnotation = document.Pages(1).Annotations(5) as PdfLoadedLineAnnotation
        /// 'Assign the line ending style
        /// lineAnnotation.EndLineStyle = PdfLineEndingStyle.Circle
        /// 'Save the document.
        /// document.Save("lineAnnotation.pdf")
        /// </code>
        /// </example>
        public PdfLineEndingStyle EndLineStyle
        {
            get
            {
                return GetLineStyle(1);
            }
            set
            {
                PdfArray m_lineStyle = GetLineStyle();
                if (m_lineStyle == null)
                {
                    m_lineStyle.Insert(0, new PdfName(PdfLineEndingStyle.Square));
                }
                else
                {
                    m_lineStyle.RemoveAt(1);
                }
                m_lineStyle.Insert(1, new PdfName(GetLineStyle(value.ToString())));
                Dictionary.SetProperty(DictionaryProperties.LE, m_lineStyle);
            }
        }

        /// <summary>
        /// Gets or sets the inner line color of the annotation.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
        /// //Gets the annotation from loaded document.
        /// PdfLoadedLineAnnotation lineAnnotation = document.Pages[1].Annotations[5] as PdfLoadedLineAnnotation;
        /// //Assign the line color
        /// lineAnnotation.InnerLineColor = new PdfColor(Color.Blue);
        /// //Save the document.
        /// document.Save("lineAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        ///   'Load an existing document.
        /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
        /// 'Gets the annotation from loaded document.
        /// Dim lineAnnotation As PdfLoadedLineAnnotation = document.Pages(1).Annotations(5) as PdfLoadedLineAnnotation
        /// 'Assign the line color
        /// lineAnnotation.InnerLineColor = New PdfColor(Color.Blue)
        /// 'Save the document.
        /// document.Save("lineAnnotation.pdf")
        /// </code>
        /// </example>
        public PdfColor InnerLineColor
        {
            get
            {
                return GetBackColor();
            }
            set
            {
                PdfArray innerlinecolor = new PdfArray();
                m_backcolor = value;
                innerlinecolor.Insert(0, new PdfNumber(m_backcolor.R / 255f));
                innerlinecolor.Insert(1, new PdfNumber(m_backcolor.G / 255f));
                innerlinecolor.Insert(2, new PdfNumber(m_backcolor.B / 255f));
                Dictionary.SetProperty(DictionaryProperties.IC, innerlinecolor);
            }
        }

        /// <summary>
        /// Gets or sets the leader line of the annotation.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
        /// //Gets the annotation from loaded document.
        /// PdfLoadedLineAnnotation lineAnnotation = document.Pages[1].Annotations[5] as PdfLoadedLineAnnotation;
        /// //Sets the leader line.
        /// lineAnnotation.LeaderLine = 20;
        /// //Save the document.
        /// document.Save("lineAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        ///   'Load an existing document.
        /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
        /// 'Gets the annotation from loaded document.
        /// Dim lineAnnotation As PdfLoadedLineAnnotation = document.Pages(1).Annotations(5) as PdfLoadedLineAnnotation
        /// 'Sets the leader line.
        /// lineAnnotation.LeaderLine = 20
        /// 'Save the document.
        /// document.Save("lineAnnotation.pdf")
        /// </code>
        /// </example>
        public int LeaderLine
        {
            get
            {
                return GetLeaderLine();
            }
            set
            {
                Dictionary.SetNumber(DictionaryProperties.LL, value);
            }
        }

        /// <summary>
        /// Gets or sets the leader ext of the annotation.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
        /// //Gets the annotation from loaded document.
        /// PdfLoadedLineAnnotation lineAnnotation = document.Pages[1].Annotations[5] as PdfLoadedLineAnnotation;
        /// //Assign the leader line
        /// lineAnnotation.LeaderExt = 20;
        /// //Save the document.
        /// document.Save("lineAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        ///   'Load an existing document.
        /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
        /// 'Gets the annotation from loaded document.
        /// Dim lineAnnotation As PdfLoadedLineAnnotation = document.Pages(1).Annotations(5) as PdfLoadedLineAnnotation
        /// 'Assign the leader line
        /// lineAnnotation.LeaderExt = 20
        /// 'Save the document.
        /// document.Save("lineAnnotation.pdf")
        /// </code>
        /// </example>
        public int LeaderExt
        {
            get
            {
                return GetLeaderExt();
            }
            set
            {
                Dictionary.SetNumber(DictionaryProperties.LLE, value);
            }
        }

        /// <summary>
        /// Gets the line border of the annotation.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
        /// //Gets the annotation from loaded document.
        /// PdfLoadedLineAnnotation lineAnnotation = document.Pages[1].Annotations[5] as PdfLoadedLineAnnotation;
        /// //Create a new pdf line border.
        /// LineBorder lineBorder = new LineBorder();
        /// lineBorder.BorderStyle = PdfBorderStyle.Solid;
        /// lineBorder.DashArray = 1;
        /// lineBorder.BorderWidth =3;
        /// lineBorder.DashArray = 8;
        /// lineAnnotation.LineBorder = lineBorder;
        /// //Save the document.
        /// document.Save("lineAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        ///   'Load an existing document.
        /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
        /// 'Gets the annotation from loaded document.
        /// Dim lineAnnotation As PdfLoadedLineAnnotation = document.Pages(1).Annotations(5) as PdfLoadedLineAnnotation
        /// 'Create a new pdf line border.
        /// Dim lineBorder As New LineBorder()
        /// lineBorder.BorderStyle = PdfBorderStyle.Solid
        /// lineBorder.DashArray = 1
        /// lineBorder.BorderWidth = 3
        /// lineBorder.DashArray = 8
        /// lineAnnotation.LineBorder = lineBorder
        /// 'Save the document.
        /// document.Save("lineAnnotation.pdf")
        /// </code>
        /// </example>
        public LineBorder LineBorder
        {
            get
            {
                return GetLineBorder();
            }
            set
            {
                m_lineborder = value;
                Dictionary.SetProperty(DictionaryProperties.BS, m_lineborder);
            }
        }

        /// <summary>
        /// Gets or sets the line caption of the annotation.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
        /// //Gets the annotation from loaded document.
        /// PdfLoadedLineAnnotation lineAnnotation = document.Pages[1].Annotations[5] as PdfLoadedLineAnnotation;
        /// //Assign the line caption
        /// lineAnnotation.LineCaption = true;
        /// lineAnnotation.CaptionType = PdfLineCaptionType.Top;
        /// //Save the document.
        /// document.Save("lineAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        ///   'Load an existing document.
        /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
        /// 'Gets the annotation from loaded document.
        /// Dim lineAnnotation As PdfLoadedLineAnnotation = document.Pages(1).Annotations(5) as PdfLoadedLineAnnotation
        /// 'Assign the line caption
        /// lineAnnotation.LineCaption = True
        /// lineAnnotation.CaptionType = PdfLineCaptionType.Top
        /// 'Save the document.
        /// document.Save("lineAnnotation.pdf")
        /// </code>
        /// </example>
        public bool LineCaption
        {
            get
            {
                return GetLineCaption();
            }
            set
            {
                Dictionary.SetBoolean(DictionaryProperties.Cap, value);
            }
        }
        /// <summary>
        /// Gets or sets the line intent of the annotation.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
        /// //Gets the annotation from loaded document.
        /// PdfLoadedLineAnnotation lineAnnotation = document.Pages[1].Annotations[5] as PdfLoadedLineAnnotation;
        /// lineAnnotation.LineIntent = PdfLineIntent.LineArrow;
        /// //Save the document.
        /// document.Save("lineAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document.
        /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
        /// 'Gets the annotation from loaded document.
        /// Dim lineAnnotation As PdfLoadedLineAnnotation = document.Pages(1).Annotations(5) as PdfLoadedLineAnnotation
        /// lineAnnotation.LineIntent = PdfLineIntent.LineArrow
        /// 'Save the document.
        /// document.Save("lineAnnotation.pdf")
        /// </code>
        /// </example>
        public PdfLineIntent LineIntent
        {
            get
            {
                return GetLineIntent();
            }
            set
            {
                Dictionary.SetName(DictionaryProperties.IT, value.ToString());
            }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLoadedLineAnnotation"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <param name="rectangle">The rectangle</param>
        /// <param name="text">The text</param>
        internal PdfLoadedLineAnnotation(PdfDictionary dictionary, PdfCrossTable crossTable, RectangleF rectangle, string text)
            : base(dictionary, crossTable)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            Dictionary = dictionary;
            m_crossTable = crossTable;

            Text = text;
        }
        #endregion

        #region Implementations

        /// <summary>
        /// Gets the line intent of the annotation.
        /// </summary>
        /// <returns>The line intent.</returns>
        private PdfLineIntent GetLineIntent()
        {
            PdfLineIntent m_lineintent = PdfLineIntent.LineArrow;
            if (Dictionary.ContainsKey(DictionaryProperties.IT))
            {
                PdfName lineintent = m_crossTable.GetObject(Dictionary[DictionaryProperties.IT]) as PdfName;
                m_lineintent = GetLineIntentText(lineintent.Value.ToString());
            }
            return m_lineintent;
        }

        /// <summary>
        /// Gets line style of the annotation.
        /// </summary>
        /// <returns>The line style.</returns>
        private PdfArray GetLineStyle()
        {
            PdfArray array = null;
            if (Dictionary.ContainsKey(DictionaryProperties.LE))
            {
                array = m_crossTable.GetObject(Dictionary[DictionaryProperties.LE]) as PdfArray;
            }
            return array;
        }

        /// <summary>
        /// Gets line style of the annotation.
        /// </summary>
        /// <param name="Ch">choice</param>
        /// <returns>The line style.</returns>
        private PdfLineEndingStyle GetLineStyle(int Ch)
        {
            PdfLineEndingStyle m_linestyle = PdfLineEndingStyle.Square;
            PdfArray array = GetLineStyle();
            if (array != null)
            {
                PdfName style = array[Ch] as PdfName;
                m_linestyle = GetLineStyle(style.Value);
            }
            return m_linestyle;
        }

        /// <summary>
        /// Gets line style of the annotation.
        /// </summary>
        /// <param name="style">Type of line style</param>
        /// <returns>The line style.</returns>
        private PdfLineEndingStyle GetLineStyle(string style)
        {
            PdfLineEndingStyle linestyle = PdfLineEndingStyle.None;
            switch (style)
            {
                case "Square":
                    linestyle = PdfLineEndingStyle.Square;
                    break;
                case "Circle":
                    linestyle = PdfLineEndingStyle.Circle;
                    break;
                case "Diamond":
                    linestyle = PdfLineEndingStyle.Diamond;
                    break;
                case "OpenArrow":
                    linestyle = PdfLineEndingStyle.OpenArrow;
                    break;
                case "ClosedArrow":
                    linestyle = PdfLineEndingStyle.ClosedArrow;
                    break;
                case "None":
                    linestyle = PdfLineEndingStyle.None;
                    break;
                case "ROpenArrow":
                    linestyle = PdfLineEndingStyle.ROpenArrow;
                    break;
                case "Butt":
                    linestyle = PdfLineEndingStyle.Butt;
                    break;
                case "RClosedArrow":
                    linestyle = PdfLineEndingStyle.RClosedArrow;
                    break;
                case "Slash":
                    linestyle = PdfLineEndingStyle.Slash;
                    break;
            }
            return linestyle;
        }

        /// <summary>
        /// Gets back color of the annotation.
        /// </summary>
        /// <returns>The back color.</returns>
        private PdfColor GetBackColor()
        {
            PdfColorSpace cs = PdfColorSpace.RGB;
            PdfColor color = PdfColor.Empty;
            PdfArray colours = null;
            if (Dictionary.ContainsKey(DictionaryProperties.C))
            {
                colours = Dictionary[DictionaryProperties.C] as PdfArray;
            }
            else
            {
                colours = color.ToArray(cs);
            }
            float red = (colours[0] as PdfNumber).FloatValue;
            float green = (colours[1] as PdfNumber).FloatValue;
            float blue = (colours[2] as PdfNumber).FloatValue;
            color = new PdfColor(red, green, blue);
            return color;
        }

        /// <summary>
        /// Gets caption type of the annotation.
        /// </summary>
        /// <returns>The caption type.</returns>
        private PdfLineCaptionType GetCaptionType()
        {
            PdfLineCaptionType m_captiontype = PdfLineCaptionType.Inline;
            if (Dictionary.ContainsKey(DictionaryProperties.CP))
            {
                PdfName cType = Dictionary[DictionaryProperties.CP] as PdfName;
                m_captiontype = GetCaptionType(cType.Value.ToString());
            }
            return m_captiontype;
        }
        /// <summary>
        /// Gets caption type of the annotation.
        /// </summary>
        /// <param name="cType">caption type</param>
        /// <returns>The caption type.</returns>
        private PdfLineCaptionType GetCaptionType(string cType)
        {
            PdfLineCaptionType captiontype = PdfLineCaptionType.Inline;
            if (cType == "Inline")
                captiontype = PdfLineCaptionType.Inline;
            else
                captiontype = PdfLineCaptionType.Top;
            return captiontype;
        }

        /// <summary>
        /// Gets line caption of the annotation.
        /// </summary>
        /// <returns>The line caption.</returns> 
        private bool GetLineCaption()
        {
            bool lCaption = false;
            if (Dictionary.ContainsKey(DictionaryProperties.Cap))
            {
                PdfBoolean lCap = Dictionary[DictionaryProperties.Cap] as PdfBoolean;
                lCaption = lCap.Value;
            }
            return lCaption;
        }
        /// <summary>
        /// Gets leader line of the annotation.
        /// </summary>
        /// <returns>The leader line.</returns> 
        private int GetLeaderLine()
        {
            int lLine = 0;
            if (Dictionary.ContainsKey(DictionaryProperties.LL))
            {
                PdfNumber ll = Dictionary[DictionaryProperties.LL] as PdfNumber;
                lLine = ll.IntValue;
            }
            return lLine;
        }

        /// <summary>
        /// Gets leader ext of the annotation.
        /// </summary>
        /// <returns>The leader ext.</returns>
        private int GetLeaderExt()
        {
            int lLineExt = 0;
            if (Dictionary.ContainsKey(DictionaryProperties.LLE))
            {
                PdfNumber lExt = Dictionary[DictionaryProperties.LLE] as PdfNumber;
                lLineExt = lExt.IntValue;
            }
            return lLineExt;
        }

        /// <summary>
        /// Gets line border of the annotation.
        /// </summary>
        /// <returns>The line border.</returns>
        private LineBorder GetLineBorder()
        {
            LineBorder lBorder = new LineBorder();
            if (Dictionary.ContainsKey(DictionaryProperties.BS))
            {
                PdfDictionary lbDic = m_crossTable.GetObject(Dictionary[DictionaryProperties.BS]) as PdfDictionary;

                if (lbDic.ContainsKey(DictionaryProperties.W))
                    lBorder.BorderWidth = (lbDic[DictionaryProperties.W] as PdfNumber).IntValue;

                if (lbDic.ContainsKey(DictionaryProperties.S))
                {
                    PdfName bstr = lbDic[DictionaryProperties.S] as PdfName;
                    lBorder.BorderStyle = GetBorderStyle(bstr.Value.ToString());
                }
                if (lbDic.ContainsKey(DictionaryProperties.D))
                {
                    PdfArray m_dasharray = lbDic[DictionaryProperties.D] as PdfArray;

                    int m_dashArray = (m_dasharray[0] as PdfNumber).IntValue;
                    m_dasharray.Clear();
                    m_dasharray.Insert(0, new PdfNumber(m_dashArray));
                    m_dasharray.Insert(1, new PdfNumber(m_dashArray));
                    lBorder.DashArray = m_dashArray;
                }
            }
            return lBorder;
        }
        /// <summary>
        /// Gets border style of the annotation.
        /// </summary>
        /// <param name="bstyle">border style</param>
        /// <returns>The border style.</returns>
        private PdfBorderStyle GetBorderStyle(string bstyle)
        {
            PdfBorderStyle style = PdfBorderStyle.Solid;
            switch (bstyle)
            {
                case "S":
                    style = PdfBorderStyle.Solid;
                    break;
                case "D":
                    style = PdfBorderStyle.Dashed;
                    break;
                case "B":
                    style = PdfBorderStyle.Beveled;
                    break;
                case "I":
                    style = PdfBorderStyle.Inset;
                    break;
                case "U":
                    style = PdfBorderStyle.Underline;
                    break;
            }
            return style;
        }

        /// <summary>
        /// Get the Line Intent Text.
        /// </summary>
        /// <param name="lintent">Line Intent Text</param>
        /// <returns>Line Intent Text<Line Intent Text/returns>
        private PdfLineIntent GetLineIntentText(string lintent)
        {
            PdfLineIntent lineintent = PdfLineIntent.LineArrow;
            switch (lintent)
            {
                case "LineArrow":
                    lineintent = PdfLineIntent.LineArrow;
                    break;
                case "LineDimension":
                    lineintent = PdfLineIntent.LineDimension;
                    break;
            }
            return lineintent;
        }
        #endregion
    }
}
