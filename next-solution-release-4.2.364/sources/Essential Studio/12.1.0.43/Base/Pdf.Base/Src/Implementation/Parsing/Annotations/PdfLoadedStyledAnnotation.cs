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
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents the PdfLoadedStyledAnnotation.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Load an existing document.
    /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
    /// //Gets the annotation from loaded document.
    /// PdfLoadedFileLinkAnnotation fileLinkAnnotation = document.Pages[1].Annotations[6] as PdfLoadedFileLinkAnnotation;
    /// //Sets the annotation flags
    /// attchmentAnnotation.AnnotationFlags=PdfAnnotationFlags.Default;
    /// //Sets the file name.
    /// fileLinkAnnotation.FileName = @"..\..\Data\Manual.txt";
    /// // Set the file link annotation border.
    /// fileLinkAnnotation.Border=new PdfAnnotationBorder(4, 0, 0);
    /// //Set the file link annotation color.
    /// fileLinkAnnotation.Color=new PdfColor(Color.Blue);
    /// //Sets the file link annotation text.
    /// fileLinkAnnotation.Text = "File Link Annotation";
    /// //Save the document.
    /// document.Save("fileLinkAnnotation.pdf");
    /// </code>
    /// <code lang="VB">
    ///'Load an existing document.
    /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
    /// 'Gets the annotation from loaded document.
    /// Dim fileLinkAnnotation As PdfLoadedFileLinkAnnotation = document.Pages(1).Annotations(6) as PdfLoadedFileLinkAnnotation
    /// 'Sets the annotation flags
    /// attchmentAnnotation.AnnotationFlags=PdfAnnotationFlags.Default
    /// 'Sets the file name.
    /// fileLinkAnnotation.FileName = @"..\..\Data\Manual.txt"
    /// ' Set the file link annotation border.
    /// fileLinkAnnotation.Border=New PdfAnnotationBorder(4, 0, 0)
    /// 'Set the file link annotation color.
    /// fileLinkAnnotation.Color=New PdfColor(Color.Blue)
    /// 'Sets the file link annotation text.
    /// fileLinkAnnotation.Text = "File Link Annotation"
    /// 'Save the document.
    /// document.Save("fileLinkAnnotation.pdf")
    /// </code>
    /// </example>
    public class PdfLoadedStyledAnnotation : PdfLoadedAnnotation
    {
        #region Fields
        private PdfSound m_sound;
        private PdfDictionary m_dictionary;
        private PdfCrossTable m_crossTable;
        private PdfColor m_color;
        private string m_text;
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
        /// //Gets the annotation from loaded document.
        /// PdfLoadedFileLinkAnnotation fileLinkAnnotation = document.Pages[1].Annotations[6] as PdfLoadedFileLinkAnnotation;
        /// //Set the file link annotation color.
        /// fileLinkAnnotation.Color=new PdfColor(Color.Blue);
        /// //Save the document.
        /// document.Save("fileLinkAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        ///'Load an existing document.
        /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
        /// 'Gets the annotation from loaded document.
        /// Dim fileLinkAnnotation As PdfLoadedFileLinkAnnotation = document.Pages(1).Annotations(6) as PdfLoadedFileLinkAnnotation
        /// 'Set the file link annotation color.
        /// fileLinkAnnotation.Color=New PdfColor(Color.Blue)
        /// 'Save the document.
        /// document.Save("fileLinkAnnotation.pdf")
        /// </code>
        /// </example>
        public PdfColor Color
        {
            get
            {
                return GetColor();
            }
            set
            {
                base.Color = value;
                m_color = value;
            }
        }
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
        /// //Gets the annotation from loaded document.
        /// PdfLoadedFileLinkAnnotation fileLinkAnnotation = document.Pages[1].Annotations[6] as PdfLoadedFileLinkAnnotation;
        /// //Sets the file link annotation text.
        /// fileLinkAnnotation.Text = "File Link Annotation";
        /// //Save the document.
        /// document.Save("fileLinkAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        ///'Load an existing document.
        /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
        /// 'Gets the annotation from loaded document.
        /// Dim fileLinkAnnotation As PdfLoadedFileLinkAnnotation = document.Pages(1).Annotations(6) as PdfLoadedFileLinkAnnotation
        /// 'Sets the file link annotation text.
        /// fileLinkAnnotation.Text = "File Link Annotation"
        /// 'Save the document.
        /// document.Save("fileLinkAnnotation.pdf")
        /// </code>
        /// </example>
        public string Text
        {
            get
            {
                return GetText();
            }
            set
            {
                base.Text = value;
                m_text = value;
            }
        }
        /// <summary>
        /// Gets or sets the annotation's bounds. If this property is not set, bounds are calculated automatically
        /// based on <see cref="Location">Location</see> property and content of annotation.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
        /// //Gets the annotation from loaded document.
        /// PdfLoadedFileLinkAnnotation fileLinkAnnotation = document.Pages[1].Annotations[6] as PdfLoadedFileLinkAnnotation;
        /// 'Sets the file link annotation bounds.
        /// fileLinkAnnotation.Bounds = new RectangleF(100,100,50,50);
        /// //Save the document.
        /// document.Save("fileLinkAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        ///'Load an existing document.
        /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
        /// 'Gets the annotation from loaded document.
        /// Dim fileLinkAnnotation As PdfLoadedFileLinkAnnotation = document.Pages(1).Annotations(6) as PdfLoadedFileLinkAnnotation
        /// 'Sets the file link annotation bounds.
        /// fileLinkAnnotation.Bounds = New RectangleF(100,100,50,50)
        /// 'Save the document.
        /// document.Save("fileLinkAnnotation.pdf")
        /// </code>
        /// </example>
        public RectangleF Bounds
        {
            get
            {
                RectangleF rect = GetBounds(Dictionary, CrossTable);
                if (Page != null)
                    rect.Y = Page.Size.Height - (rect.Y + rect.Height);
                else
                    rect.Y = (rect.Y + rect.Height);

                return rect;
            }
            set
            {
                RectangleF rect = value;

                if (rect == RectangleF.Empty)
                    throw new ArgumentNullException("rectangle");

                float height = Page.Size.Height;

                PdfNumber[] values = new PdfNumber[]{
					new PdfNumber(rect.X),
					new PdfNumber(height-(rect.Y+rect.Height)),
					new PdfNumber(rect.X+rect.Width),
					new PdfNumber(height - rect.Y)};

                PdfDictionary dic = Dictionary;

                if (!dic.ContainsKey(DictionaryProperties.Rect))
                {
                    dic = GetWidgetAnnotation(Dictionary, CrossTable);
                }

                dic.SetArray(DictionaryProperties.Rect, values);

                Changed = true;
            }
        }
        /// <summary>
        /// Gets or sets the annotation's border.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
        /// //Gets the annotation from loaded document.
        /// PdfLoadedFileLinkAnnotation fileLinkAnnotation = document.Pages[1].Annotations[6] as PdfLoadedFileLinkAnnotation;
        /// //Sets the annotation flags
        /// attchmentAnnotation.AnnotationFlags=PdfAnnotationFlags.Default;
        /// // Set the file link annotation border.
        /// fileLinkAnnotation.Border=new PdfAnnotationBorder(4, 0, 0);
        /// //Save the document.
        /// document.Save("fileLinkAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        ///'Load an existing document.
        /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
        /// 'Gets the annotation from loaded document.
        /// Dim fileLinkAnnotation As PdfLoadedFileLinkAnnotation = document.Pages(1).Annotations(6) as PdfLoadedFileLinkAnnotation
        /// ' Set the file link annotation border.
        /// fileLinkAnnotation.Border=New PdfAnnotationBorder(4, 0, 0)
        /// 'Save the document.
        /// document.Save("fileLinkAnnotation.pdf")
        /// </code>
        /// </example>
        public PdfAnnotationBorder Border
        {
            get
            {
                return GetBorder();
            }
            set
            {
                base.Border = value;
                Changed = true;
            }
        }
        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
        /// //Gets the annotation from loaded document.
        /// PdfLoadedFileLinkAnnotation fileLinkAnnotation = document.Pages[1].Annotations[6] as PdfLoadedFileLinkAnnotation;
        /// //Sets the file link Annotation location.
        /// fileLinkAnnotation.Location new PointF(100,100);
        /// //Save the document.
        /// document.Save("fileLinkAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        ///'Load an existing document.
        /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
        /// 'Gets the annotation from loaded document.
        /// Dim fileLinkAnnotation As PdfLoadedFileLinkAnnotation = document.Pages(1).Annotations(6) as PdfLoadedFileLinkAnnotation
        /// 'Sets the file link Annotation location
        /// fileLinkAnnotation.Location =New PointF(100,100)
        /// 'Save the document.
        /// document.Save("fileLinkAnnotation.pdf")
        /// </code>
        /// </example>
        public PointF Location
        {
            get
            {
                return Bounds.Location;
            }
            set
            {
                Bounds = new RectangleF(value, Bounds.Size);
            }
        }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
        /// //Gets the annotation from loaded document.
        /// PdfLoadedFileLinkAnnotation fileLinkAnnotation = document.Pages[1].Annotations[6] as PdfLoadedFileLinkAnnotation;
        /// //Sets the size
        /// attchmentAnnotation.Size=new SizeF(100,50)
        /// //Save the document.
        /// document.Save("fileLinkAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        ///'Load an existing document.
        /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
        /// 'Gets the annotation from loaded document.
        /// Dim fileLinkAnnotation As PdfLoadedFileLinkAnnotation = document.Pages(1).Annotations(6) as PdfLoadedFileLinkAnnotation
        /// 'Sets the size
        /// attchmentAnnotation.Size=New SizeF(100,50)
        /// 'Save the document.
        /// document.Save("fileLinkAnnotation.pdf")
        /// </code>
        /// </example>
        public SizeF Size
        {
            get
            {
                return Bounds.Size;
            }
            set
            {
                Bounds = new RectangleF(Bounds.Location, value);
            }
        }

        /// <summary>
        /// Gets or sets the annotation flags.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
        /// //Gets the annotation from loaded document.
        /// PdfLoadedFileLinkAnnotation fileLinkAnnotation = document.Pages[1].Annotations[6] as PdfLoadedFileLinkAnnotation;
        /// //Sets the annotation flags
        /// attchmentAnnotation.AnnotationFlags=PdfAnnotationFlags.Default;
        /// //Save the document.
        /// document.Save("fileLinkAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        ///'Load an existing document.
        /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
        /// 'Gets the annotation from loaded document.
        /// Dim fileLinkAnnotation As PdfLoadedFileLinkAnnotation = document.Pages(1).Annotations(6) as PdfLoadedFileLinkAnnotation
        /// 'Sets the annotation flags
        /// attchmentAnnotation.AnnotationFlags=PdfAnnotationFlags.Default
        /// 'Save the document.
        /// document.Save("fileLinkAnnotation.pdf")
        /// </code>
        /// </example>
        public PdfAnnotationFlags AnnotationFlags
        {
            get
            {
                return GetAnnotationFlags();
            }
            set
            {
                base.AnnotationFlags = value;
                Changed = true;
            }
        }
        ///// <summary>
        ///// Gets a page which this annotation is connected to.
        ///// </summary>
        //public override PdfPage Page
        //{
        //    get
        //    {
        //        return GetLoadedPage();
        //    }
        //}
        #endregion

        #region Implementations
        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <returns>The text.</returns>
        private string GetText()
        {
            string text = null;
            if (Dictionary.ContainsKey(DictionaryProperties.Contents))
            {
                PdfString m_text = Dictionary[DictionaryProperties.Contents] as PdfString;
                text = m_text.Value.ToString();
                text = text.Trim('/');
                return text;
            }
            else
            {
                text = " ";
                return text;
            }
        }

        /// <summary>
        /// Gets the bounds.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <returns>The bounds.</returns>
        private RectangleF GetBounds(PdfDictionary dictionary, PdfCrossTable crossTable)
        {
            PdfArray array = null;

            if (dictionary.ContainsKey(DictionaryProperties.Kids))
            {
                PdfDictionary widget = GetWidgetAnnotation(dictionary, crossTable);

                if (widget.ContainsKey(DictionaryProperties.Rect))
                {
                    array = crossTable.GetObject(widget[DictionaryProperties.Rect]) as PdfArray;
                }
            }
            else
            {
                if (dictionary.ContainsKey(DictionaryProperties.Rect))
                {
                    array = crossTable.GetObject(dictionary[DictionaryProperties.Rect]) as PdfArray;
                }
            }

            RectangleF bounds = array.ToRectangle();

            return bounds;
        }

        /// <summary>
        /// Gets the border.
        /// </summary>
        /// <returns>The border.</returns>    
        private PdfAnnotationBorder GetBorder()
        {
            PdfAnnotationBorder border = null;
            if (Dictionary.ContainsKey(DictionaryProperties.Border))
            {
                PdfArray m_border = Dictionary[DictionaryProperties.Border] as PdfArray;
                float width = (m_border[0] as PdfNumber).FloatValue;
                float hRadius = (m_border[1] as PdfNumber).FloatValue;
                float vRadius = (m_border[2] as PdfNumber).FloatValue;
                border = new PdfAnnotationBorder(width, hRadius, vRadius);
                //border = new PdfAnnotationBorder(vRadius, width, hRadius);
                border.Width = vRadius;
                border.HorizontalRadius = width;
                border.VerticalRadius = hRadius;
            }
            return border;
        }

        /// <summary>
        /// Gets the color.
        /// </summary>
        /// <returns>The color.</returns>   
        private PdfColor GetColor()
        {
            PdfColorSpace cs = PdfColorSpace.RGB;
            PdfColor color;
            PdfArray colours = null;
            if (Dictionary.ContainsKey(DictionaryProperties.C))
            {
                colours = Dictionary[DictionaryProperties.C] as PdfArray;
            }
            else
            {
                colours = m_color.ToArray(cs);
            }
            float red = (colours[0] as PdfNumber).FloatValue;
            float green = (colours[1] as PdfNumber).FloatValue;
            float blue = (colours[2] as PdfNumber).FloatValue;
            color = new PdfColor(red, green, blue);
            return color;

        }

        /// <summary>
        /// Gets the loaded page.
        /// </summary>
        /// <returns>The loaded page in which annotation draw.</returns>
        private PdfPage GetLoadedPage()
        {
            PdfPageBase page = base.Page;

            if (page == null)
            {
                PdfLoadedDocument doc = CrossTable.Document as PdfLoadedDocument;

                PdfDictionary widget = GetWidgetAnnotation(Dictionary, CrossTable);

                if (widget == null)
                {
                    widget = Dictionary;
                }

                if (widget.ContainsKey(DictionaryProperties.P))
                {
                    IPdfPrimitive pageRef = CrossTable.GetObject(widget[DictionaryProperties.P]);
                    PdfDictionary pageDic = pageRef as PdfDictionary;

                    if (pageDic != null)
                        page = doc.Pages.GetPage(pageDic);
                }
                else
                {
                    PdfReference widgetReference = CrossTable.GetReference(widget);
                    foreach (PdfLoadedPage loadedpage in doc.Pages)
                    {
                        PdfArray lAnnots = loadedpage.GetAnnots();

                        if (lAnnots != null)
                        {
                            for (int i = 0; i < lAnnots.Count; i++)
                            {
                                PdfReferenceHolder holder = lAnnots[i] as PdfReferenceHolder;
                                if (holder.Reference == widgetReference)
                                {
                                    page = loadedpage;
                                    return page as PdfPage;
                                }
                            }
                        }
                    }
                }
            }

            return page as PdfPage;
        }

        /// <summary>
        /// Gets the annotation flags.
        /// </summary>
        /// <returns>The annotation flags.</returns>   
        private PdfAnnotationFlags GetAnnotationFlags()
        {
            PdfAnnotationFlags annotationFlags = PdfAnnotationFlags.Default;
            if (Dictionary.ContainsKey(DictionaryProperties.F))
            {
                PdfNumber annotFlags = GetValue(Dictionary, m_crossTable, DictionaryProperties.F, false) as PdfNumber;
                annotationFlags = (PdfAnnotationFlags)annotFlags.IntValue;
            }
            return annotationFlags;
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLoadedStyledField"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        internal PdfLoadedStyledAnnotation(PdfDictionary dictionary, PdfCrossTable crossTable)
            : base(dictionary, crossTable)
        {
            m_dictionary = dictionary;
            m_crossTable = crossTable;
        }
        #endregion
    }
}
