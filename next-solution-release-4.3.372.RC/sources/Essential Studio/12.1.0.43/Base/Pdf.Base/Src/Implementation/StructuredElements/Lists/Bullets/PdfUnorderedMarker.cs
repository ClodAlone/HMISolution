#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Drawing;

using Syncfusion.Pdf.Graphics;

/// <summary>
/// The Syncfusion.Pdf.Lists namespace contains classes for creating structure elements in PDF document.
/// </summary>
namespace Syncfusion.Pdf.Lists
{
    /// <summary>
    /// Represents bullet for the list.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Create a new PDf document
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = document.Pages.Add();
    /// PdfGraphics graphics = page.Graphics;
    /// //Create a unordered list
    /// PdfUnorderedList list = new PdfUnorderedList();            
    /// //Set the marker style
    /// list.Marker.Style = PdfUnorderedMarkerStyle.Disk;
    /// //Create a font and write title
    /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold);
    /// //Create string format
    /// PdfStringFormat format = new PdfStringFormat();
    /// format.LineSpacing = 20f;
    /// font = new PdfStandardFont(PdfFontFamily.TimesRoman, 10, PdfFontStyle.Bold);
    /// //Apply formattings to list
    /// list.Font = font;
    /// list.StringFormat = format;
    /// //Set list indent
    /// list.Indent = 10;
    /// //Add items to the list
    /// list.Items.Add("List of Essential Studio products");
    /// list.Items.Add("IO products");
    /// //Set text indent
    /// list.TextIndent = 10;
    /// //Draw list
    /// list.Draw(page, new RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height));
    /// document.Save("UnOrderList.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDf document
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Create a page
    /// Dim page As PdfPage = document.Pages.Add()
    /// Dim graphics As PdfGraphics = page.Graphics
    /// 'Create a unordered list
    /// Dim list As PdfUnorderedList = New PdfUnorderedList()
    /// 'Set the marker style
    /// list.Marker.Style = PdfUnorderedMarkerStyle.Disk
    /// 'Create a font and write title
    /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold)
    /// 'Create string format
    /// Dim format As PdfStringFormat = New PdfStringFormat()
    /// format.LineSpacing = 20f
    /// font = New PdfStandardFont(PdfFontFamily.TimesRoman, 10, PdfFontStyle.Bold)
    /// 'Apply formattings to list
    /// list.Font = font
    /// list.StringFormat = format
    /// 'Set list indent
    /// list.Indent = 10
    /// 'Add items to the list
    /// list.Items.Add("List of Essential Studio products")
    /// list.Items.Add("IO products")
    /// 'Set text indent
    /// list.TextIndent = 10
    /// 'Draw list
    /// list.Draw(page, New RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height))
    /// document.Save("UnOrderList.pdf")
    /// </code>
    /// </example>   
    /// <seealso cref="PdfMarker"/> Class    
    /// <seealso cref="PdfDocument"/> Class   
    /// <seealso cref="PdfFont"/> Class   
    /// <seealso cref="PdfUnorderedList"/> Class 
    public class PdfUnorderedMarker
        : PdfMarker
    {
        #region Fields
        /// <summary>
        /// Holds the marker text.
        /// </summary>
        private string m_text;

        /// <summary>
        /// Holds the marker style.
        /// </summary>
        private PdfUnorderedMarkerStyle m_style;

        /// <summary>
        /// Holds the marker image.
        /// </summary>
        private PdfImage m_image;

        /// <summary>
        /// Marker temlapte.
        /// </summary>
        private PdfTemplate m_template;

        /// <summary>
        /// Marker size.
        /// </summary>
        private SizeF m_size;

        /// <summary>
        /// Font used when draws styled marker
        /// </summary>
        private PdfFont m_unicodeFont;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets template of the marker.
        /// </summary>
        public PdfTemplate Template
        {
            get
            {
                return m_template;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("template");

                m_template = value;

                m_style = PdfUnorderedMarkerStyle.CustomTemplate;
            }
        }

        /// <summary>
        /// Gets or sets image of the marker.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfGraphics graphics = page.Graphics;
        /// //Create a unordered list
        /// PdfUnorderedList list = new PdfUnorderedList();            
        /// //Set the list image
        /// list.Marker.Image = new PdfBitmap("Bullet.jpg");
        /// //Create a font and write title
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold);
        /// //Create string format
        /// PdfStringFormat format = new PdfStringFormat();
        /// format.LineSpacing = 20f;
        /// font = new PdfStandardFont(PdfFontFamily.TimesRoman, 10, PdfFontStyle.Bold);
        /// //Apply formattings to list
        /// list.Font = font;
        /// list.StringFormat = format;
        /// //Set list indent
        /// list.Indent = 10;
        /// //Add items to the list
        /// list.Items.Add("List of Essential Studio products");
        /// list.Items.Add("IO products");
        /// //Set text indent
        /// list.TextIndent = 10;
        /// //Draw list
        /// list.Draw(page, new RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height));
        /// document.Save("UnOrderList.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim graphics As PdfGraphics = page.Graphics
        /// 'Create a unordered list
        /// Dim list As PdfUnorderedList = New PdfUnorderedList()
        /// 'Set the list image
        /// list.Marker.Image = new PdfBitmap("Bullet.jpg")
        /// 'Create a font and write title
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold)
        /// 'Create string format
        /// Dim format As PdfStringFormat = New PdfStringFormat()
        /// format.LineSpacing = 20f
        /// font = New PdfStandardFont(PdfFontFamily.TimesRoman, 10, PdfFontStyle.Bold)
        /// 'Apply formattings to list
        /// list.Font = font
        /// list.StringFormat = format
        /// 'Set list indent
        /// list.Indent = 10
        /// 'Add items to the list
        /// list.Items.Add("List of Essential Studio products")
        /// list.Items.Add("IO products")
        /// 'Set text indent
        /// list.TextIndent = 10
        /// 'Draw list
        /// list.Draw(page, New RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height))
        /// document.Save("UnOrderList.pdf")
        /// </code>
        /// </example>   
        /// <seealso cref="PdfMarker"/> Class    
        /// <seealso cref="PdfDocument"/> Class   
        /// <seealso cref="PdfFont"/> Class   
        /// <seealso cref="PdfBitmap"/> Class 
        public PdfImage Image
        {
            get
            {
                return m_image;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("image");

                m_image = value;
                m_style = PdfUnorderedMarkerStyle.CustomImage;
            }
        }

        /// <summary>
        /// Gets or sets marker text.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfGraphics graphics = page.Graphics;
        /// //Create a unordered list
        /// PdfUnorderedList list = new PdfUnorderedList();  
        /// //Set the marker Text
        /// list.Marker.Text = "List: ";
        /// //Create a font and write title
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold);
        /// //Create string format
        /// PdfStringFormat format = new PdfStringFormat();
        /// format.LineSpacing = 20f;
        /// font = new PdfStandardFont(PdfFontFamily.TimesRoman, 10, PdfFontStyle.Bold);
        /// //Apply formattings to list
        /// list.Font = font;
        /// list.StringFormat = format;
        /// //Set list indent
        /// list.Indent = 10;
        /// //Add items to the list
        /// list.Items.Add("List of Essential Studio products");
        /// list.Items.Add("IO products");
        /// //Set text indent
        /// list.TextIndent = 10;
        /// //Draw list
        /// list.Draw(page, new RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height));
        /// document.Save("UnOrderList.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim graphics As PdfGraphics = page.Graphics
        /// 'Create a unordered list
        /// Dim list As PdfUnorderedList = New PdfUnorderedList()
        /// 'Set the marker Text
        /// list.Marker.Text = "List: "
        /// 'Create a font and write title
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold)
        /// 'Create string format
        /// Dim format As PdfStringFormat = New PdfStringFormat()
        /// format.LineSpacing = 20f
        /// font = New PdfStandardFont(PdfFontFamily.TimesRoman, 10, PdfFontStyle.Bold)
        /// 'Apply formattings to list
        /// list.Font = font
        /// list.StringFormat = format
        /// 'Set list indent
        /// list.Indent = 10
        /// 'Add items to the list
        /// list.Items.Add("List of Essential Studio products")
        /// list.Items.Add("IO products")
        /// 'Set text indent
        /// list.TextIndent = 10
        /// 'Draw list
        /// list.Draw(page, New RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height))
        /// document.Save("UnOrderList.pdf")
        /// </code>
        /// </example>   
        /// <seealso cref="PdfMarker"/> Class    
        /// <seealso cref="PdfDocument"/> Class   
        /// <seealso cref="PdfFont"/> Class         
        public string Text
        {
            get
            {
                return m_text;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("text");

                m_text = value;
                m_style = PdfUnorderedMarkerStyle.CustomString;
            }
        }

        /// <summary>
        /// Gets or sets the style.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfGraphics graphics = page.Graphics;
        /// //Create a unordered list
        /// PdfUnorderedList list = new PdfUnorderedList();            
        /// //Set the marker style
        /// list.Marker.Style = PdfUnorderedMarkerStyle.Disk;
        /// //Create a font and write title
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold);
        /// //Create string format
        /// PdfStringFormat format = new PdfStringFormat();
        /// format.LineSpacing = 20f;
        /// font = new PdfStandardFont(PdfFontFamily.TimesRoman, 10, PdfFontStyle.Bold);
        /// //Apply formattings to list
        /// list.Font = font;
        /// list.StringFormat = format;
        /// //Set list indent
        /// list.Indent = 10;
        /// //Add items to the list
        /// list.Items.Add("List of Essential Studio products");
        /// list.Items.Add("IO products");
        /// //Set text indent
        /// list.TextIndent = 10;
        /// //Draw list
        /// list.Draw(page, new RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height));
        /// document.Save("UnOrderList.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim graphics As PdfGraphics = page.Graphics
        /// 'Create a unordered list
        /// Dim list As PdfUnorderedList = New PdfUnorderedList()
        /// 'Set the marker style
        /// list.Marker.Style = PdfUnorderedMarkerStyle.Disk
        /// 'Create a font and write title
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold)
        /// 'Create string format
        /// Dim format As PdfStringFormat = New PdfStringFormat()
        /// format.LineSpacing = 20f
        /// font = New PdfStandardFont(PdfFontFamily.TimesRoman, 10, PdfFontStyle.Bold)
        /// 'Apply formattings to list
        /// list.Font = font
        /// list.StringFormat = format
        /// 'Set list indent
        /// list.Indent = 10
        /// 'Add items to the list
        /// list.Items.Add("List of Essential Studio products")
        /// list.Items.Add("IO products")
        /// 'Set text indent
        /// list.TextIndent = 10
        /// 'Draw list
        /// list.Draw(page, New RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height))
        /// document.Save("UnOrderList.pdf")
        /// </code>
        /// </example>   
        /// <seealso cref="PdfMarker"/> Class    
        /// <seealso cref="PdfDocument"/> Class   
        /// <seealso cref="PdfFont"/> Class   
        /// <seealso cref="PdfUnorderedList"/> Class 
        public PdfUnorderedMarkerStyle Style
        {
            get
            {
                return m_style;
            }
            set
            {
                m_style = value;
            }
        }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        internal SizeF Size
        {
            get
            {
                return m_size;
            }
            set
            {
                m_size = value;
            }
        }

        /// <summary>
        /// Gets or sets the unicode font.
        /// </summary>
        internal PdfFont UnicodeFont
        {
            get
            {
                return m_unicodeFont;
            }
            set
            {
                m_unicodeFont = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfUnorderedMarker"/> class.
        /// </summary>
        /// <param name="text">The text of the marker.</param>
        /// <param name="font">Marker font.</param>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfGraphics graphics = page.Graphics;
        /// //Create a font and write title
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold);
        /// //Create a unordered list
        /// PdfUnorderedList list = new PdfUnorderedList();           
        /// list.Marker = new PdfUnorderedMarker("list", font);            
        /// //Create string format
        /// PdfStringFormat format = new PdfStringFormat();
        /// format.LineSpacing = 20f;
        /// font = new PdfStandardFont(PdfFontFamily.TimesRoman, 10, PdfFontStyle.Bold);
        /// //Apply formattings to list
        /// list.Font = font;
        /// list.StringFormat = format;
        /// //Set list indent
        /// list.Indent = 10;
        /// //Add items to the list
        /// list.Items.Add("List of Essential Studio products");
        /// list.Items.Add("IO products");
        /// //Set text indent
        /// list.TextIndent = 10;
        /// //Draw list
        /// list.Draw(page, new RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height));
        /// document.Save("UnOrderList.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim graphics As PdfGraphics = page.Graphics
        /// 'Create a font and write title
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold)
        /// 'Create a unordered list
        /// Dim list As PdfUnorderedList = New PdfUnorderedList()
        /// list.Marker = New PdfUnorderedMarker("list", font)
        /// 'Create string format
        /// Dim format As PdfStringFormat = New PdfStringFormat()
        /// format.LineSpacing = 20f
        /// font = New PdfStandardFont(PdfFontFamily.TimesRoman, 10, PdfFontStyle.Bold)
        /// 'Apply formattings to list
        /// list.Font = font
        /// list.StringFormat = format
        /// 'Set list indent
        /// list.Indent = 10
        /// 'Add items to the list
        /// list.Items.Add("List of Essential Studio products")
        /// list.Items.Add("IO products")
        /// 'Set text indent
        /// list.TextIndent = 10
        /// 'Draw list
        /// list.Draw(page, New RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height))
        /// document.Save("UnOrderList.pdf")
        /// </code>
        /// </example>   
        /// <seealso cref="PdfMarker"/> Class    
        /// <seealso cref="PdfDocument"/> Class   
        /// <seealso cref="PdfFont"/> Class   
        /// <seealso cref="PdfUnorderedList"/> Class 
        public PdfUnorderedMarker(string text, PdfFont font)
        {
            Font = font;
            Text = text;
            m_style = PdfUnorderedMarkerStyle.CustomString;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfUnorderedMarker"/> class.
        /// </summary>
        /// <param name="style">The style of the marker.</param>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfGraphics graphics = page.Graphics;
        /// //Create a unordered list
        /// PdfUnorderedList list = new PdfUnorderedList();    
        /// list.Markerlist.Marker = new PdfUnorderedMarker(PdfUnorderedMarkerStyle.Asterisk);
        /// //Create a font and write title
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold);
        /// //Create string format
        /// PdfStringFormat format = new PdfStringFormat();
        /// format.LineSpacing = 20f;
        /// font = new PdfStandardFont(PdfFontFamily.TimesRoman, 10, PdfFontStyle.Bold);
        /// //Apply formattings to list
        /// list.Font = font;
        /// list.StringFormat = format;
        /// //Set list indent
        /// list.Indent = 10;
        /// //Add items to the list
        /// list.Items.Add("List of Essential Studio products");
        /// list.Items.Add("IO products");
        /// //Set text indent
        /// list.TextIndent = 10;
        /// //Draw list
        /// list.Draw(page, new RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height));
        /// document.Save("UnOrderList.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim graphics As PdfGraphics = page.Graphics
        /// 'Create a unordered list
        /// Dim list As PdfUnorderedList = New PdfUnorderedList()
        /// list.Marker.Style = PdfUnorderedMarkerStyle.Disk
        /// 'Create a font and write title
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold)
        /// 'Create string format
        /// Dim format As PdfStringFormat = New PdfStringFormat()
        /// format.LineSpacing = 20f
        /// font = New PdfStandardFont(PdfFontFamily.TimesRoman, 10, PdfFontStyle.Bold)
        /// 'Apply formattings to list
        /// list.Font = font
        /// list.StringFormat = format
        /// 'Set list indent
        /// list.Indent = 10
        /// 'Add items to the list
        /// list.Items.Add("List of Essential Studio products")
        /// list.Items.Add("IO products")
        /// 'Set text indent
        /// list.TextIndent = 10
        /// 'Draw list
        /// list.Draw(page, New RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height))
        /// document.Save("UnOrderList.pdf")
        /// </code>
        /// </example>   
        /// <seealso cref="PdfMarker"/> Class    
        /// <seealso cref="PdfDocument"/> Class   
        /// <seealso cref="PdfFont"/> Class   
        /// <seealso cref="PdfUnorderedList"/> Class 
        public PdfUnorderedMarker(PdfUnorderedMarkerStyle style)
        {
            m_style = style;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfUnorderedMarker"/> class.
        /// </summary>
        /// <param name="image">The image of the marker.</param>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfGraphics graphics = page.Graphics;
        /// //Create a unordered list
        /// PdfUnorderedList list = new PdfUnorderedList();                  
        /// list.Marker.Style = PdfUnorderedMarkerStyle.Disk;
        /// //Create a font and write title
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold);
        /// //Create string format
        /// PdfStringFormat format = new PdfStringFormat();
        /// format.LineSpacing = 20f;
        /// font = new PdfStandardFont(PdfFontFamily.TimesRoman, 10, PdfFontStyle.Bold);
        /// //Apply formattings to list
        /// list.Font = font;
        /// list.StringFormat = format;
        /// //Set list indent
        /// list.Indent = 10;
        /// //Add items to the list
        /// list.Items.Add("List of Essential Studio products");
        /// list.Items.Add("IO products");
        /// //Set text indent
        /// list.TextIndent = 10;
        /// //Draw list
        /// list.Draw(page, new RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height));
        /// document.Save("UnOrderList.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim graphics As PdfGraphics = page.Graphics
        /// 'Create a unordered list
        /// Dim list As PdfUnorderedList = New PdfUnorderedList()
        /// list.Marker.Style = PdfUnorderedMarkerStyle.Disk
        /// 'Create a font and write title
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold)
        /// 'Create string format
        /// Dim format As PdfStringFormat = New PdfStringFormat()
        /// format.LineSpacing = 20f
        /// font = New PdfStandardFont(PdfFontFamily.TimesRoman, 10, PdfFontStyle.Bold)
        /// 'Apply formattings to list
        /// list.Font = font
        /// list.StringFormat = format
        /// 'Set list indent
        /// list.Indent = 10
        /// 'Add items to the list
        /// list.Items.Add("List of Essential Studio products")
        /// list.Items.Add("IO products")
        /// 'Set text indent
        /// list.TextIndent = 10
        /// 'Draw list
        /// list.Draw(page, New RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height))
        /// document.Save("UnOrderList.pdf")
        /// </code>
        /// </example>   
        /// <seealso cref="PdfMarker"/> Class    
        /// <seealso cref="PdfDocument"/> Class   
        /// <seealso cref="PdfFont"/> Class   
        /// <seealso cref="PdfUnorderedList"/> Class 
        public PdfUnorderedMarker(PdfImage image)
        {
            Image = image;
            m_style = PdfUnorderedMarkerStyle.CustomImage;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfUnorderedMarker"/> class.
        /// </summary>
        /// <param name="template">Template of the marker.</param>
        public PdfUnorderedMarker(PdfTemplate template)
        {
            Template = template;
            m_style = PdfUnorderedMarkerStyle.CustomTemplate;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Draws the specified graphics.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="point">The point.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="pen">The pen.</param>
        internal void Draw(PdfGraphics graphics, PointF point, PdfBrush brush, PdfPen pen)
        {
            PdfTemplate templete = new PdfTemplate(m_size);

            switch (m_style)
            {
                case PdfUnorderedMarkerStyle.CustomTemplate:
                    templete = new PdfTemplate(m_size);
                    templete.Graphics.DrawPdfTemplate(m_template, PointF.Empty, m_size);
                    break;

                case PdfUnorderedMarkerStyle.CustomImage:
                    templete.Graphics.DrawImage(m_image, 1, 1, m_size.Width - 2, m_size.Height - 2);
                    break;

                default:
                    PointF location = PointF.Empty;

                    if (pen != null)
                    {
                        location.X += pen.Width;
                        location.Y += pen.Width;
                    }

                    templete.Graphics.DrawString(GetStyledText(), m_unicodeFont, pen, brush, location);
                    break;
            }

            graphics.DrawPdfTemplate(templete, point);
        }

        /// <summary>
        /// Draws the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="point">The point.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="pen">The pen.</param>
        internal void Draw(PdfPage page, PointF point, PdfBrush brush, PdfPen pen)
        {
            Draw(page.Graphics, point, brush, pen);
        }

        /// <summary>
        /// Gets the styled text.
        /// </summary>
        /// <returns>Returns symbol represented of style.</returns>
        internal string GetStyledText()
        {
            string text = string.Empty;

            switch (m_style)
            {
                case PdfUnorderedMarkerStyle.Disk:
                    text = "\x6C";
                    break;
                case PdfUnorderedMarkerStyle.Square:
                    text = "\x6E";
                    break;

                case PdfUnorderedMarkerStyle.Asterisk:
                    text = "\x5D";
                    break;

                case PdfUnorderedMarkerStyle.Circle:
                    text = "\x6D";
                    break;
            }

            return text;
        }
        #endregion
    }
}
