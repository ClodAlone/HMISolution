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
    /// Represents marker for ordered list.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    ///  //Create a new PDf document
    ///  PdfDocument document = new PdfDocument();
    ///  //Creates a new page and adds it as the last page of the document
    ///  PdfPage page = document.Pages.Add();
    ///  PdfGraphics graphics = page.Graphics;
    ///  //Create a font and write title
    ///  PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 14,PdfFontStyle.Bold);            
    ///  //Create a unordered list
    ///  PdfOrderedMarker list = new PdfOrderedMarker(PdfNumberStyle.LowerRoman, font);
    ///  //Create Ordered list as sublist of parent list
    ///  PdfOrderedList subList = new PdfOrderedList();
    ///  subList.Marker = list;
    ///  //Add items to the list
    ///  subList.Items.Add("List of Essential Studio products");
    ///  subList.Items.Add("IO products");
    ///  subList.Items.Add("Grid products");
    ///  subList.Items.Add("Tools products");
    ///  //Draw list
    ///  subList.Draw(page, new RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height));
    ///  document.Save("List.pdf");
    /// </code>
    /// <code lang="VB">
    ///  'Create a new PDf document
    ///  Dim document As PdfDocument = New PdfDocument()
    ///  'Create a page
    ///  Dim page As PdfPage = document.Pages.Add()
    ///  Dim graphics As PdfGraphics = page.Graphics
    ///  'Create a font and write title
    ///  Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 14,PdfFontStyle.Bold)
    ///  'Create a unordered list
    ///  Dim list As PdfOrderedMarker = New PdfOrderedMarker(PdfNumberStyle.LowerRoman, font)
    ///  'Create Ordered list as sublist of parent list
    ///  Dim subList As PdfOrderedList = New PdfOrderedList()
    ///  subList.Marker = list
    ///  'Add items to the list
    ///  subList.Items.Add("List of Essential Studio products")
    ///  subList.Items.Add("IO products")
    ///  subList.Items.Add("Grid products")
    ///  subList.Items.Add("Tools products")
    ///  'Draw list
    ///  subList.Draw(page, New RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height))
    ///  document.Save("List.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfMarker"/> Class  
    /// <seealso cref="PdfPage"/> Class  
    /// <seealso cref="PdfOrderedMarker"/> Class  
    public class PdfOrderedMarker
        : PdfMarker
    {
        #region Fields
        /// <summary>
        /// Holds numbering style.
        /// </summary>
        private PdfNumberStyle m_style;

        /// <summary>
        /// Start number for ordered list.
        /// </summary>
        private int m_startNumber = 1;

        /// <summary>
        /// Delimiter for numbers.
        /// </summary>
        private string m_delimiter;

        /// <summary>
        /// Finalizer for numbers.
        /// </summary>
        private string m_suffix;

        /// <summary>
        /// Current index of item.
        /// </summary>
        private int m_currentIndex;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the list numbering style.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfGraphics graphics = page.Graphics;
        /// //Create a font and write title
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold);           
        /// //Create Ordered list as sublist of parent list
        /// PdfOrderedList subList = new PdfOrderedList();
        /// subList.Marker.Alignment = PdfListMarkerAlignment.Right;
        /// subList.Marker.Delimiter = ".";
        /// subList.Marker.StartNumber = 2;
        /// subList.Marker.Style = PdfNumberStyle.UpperRoman;
        /// //Add items to the list
        /// subList.Items.Add("List of Essential Studio products");
        /// subList.Items.Add("IO products");
        /// subList.Items.Add("Grid products");
        /// subList.Items.Add("Tools products");
         /// //Draw list
         /// subList.Draw(page, new RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height));
         /// document.Save("List.pdf");
         /// </code>
         /// <code lang="VB">
         /// 'Create a new PDf document
         /// Dim document As PdfDocument = New PdfDocument()
         /// 'Creates a new page and adds it as the last page of the document
         /// Dim page As PdfPage = document.Pages.Add()
         /// Dim graphics As PdfGraphics = page.Graphics
         /// 'Create a font and write title
         /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold)
         /// 'Create Ordered list as sublist of parent list
         /// Dim subList As PdfOrderedList = New PdfOrderedList()
         /// subList.Marker.Alignment = PdfListMarkerAlignment.Right
         /// subList.Marker.Delimiter = "."
         /// subList.Marker.StartNumber = 2
         /// subList.Marker.Style = PdfNumberStyle.UpperRoman
         /// 'Add items to the list
         /// subList.Items.Add("List of Essential Studio products")
         /// subList.Items.Add("IO products")
         /// subList.Items.Add("Grid products")
         /// subList.Items.Add("Tools products")
         /// 'Draw list
         /// subList.Draw(page, New RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height))
         /// document.Save("List.pdf")
         /// </code>
          /// </example>
        /// <seealso cref="PdfDocument"/> Class
        /// <seealso cref="PdfPage"/> Class
        /// <seealso cref="PdfOrderedList"/> Class
        public PdfNumberStyle Style
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
        /// Gets ar sets start number for ordered list. Default value is 1.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfGraphics graphics = page.Graphics;
        /// //Create a font and write title
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold);           
        /// //Create Ordered list as sublist of parent list
        /// PdfOrderedList subList = new PdfOrderedList();
        /// subList.Marker.Alignment = PdfListMarkerAlignment.Right;
        /// subList.Marker.Delimiter = ".";
        /// subList.Marker.StartNumber = 2;
        /// subList.Marker.Style = PdfNumberStyle.UpperRoman;
        /// //Add items to the list
        /// subList.Items.Add("List of Essential Studio products");
        /// subList.Items.Add("IO products");
        /// subList.Items.Add("Grid products");
        /// subList.Items.Add("Tools products");
        /// //Draw list
        /// subList.Draw(page, new RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height));
        /// document.Save("List.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim graphics As PdfGraphics = page.Graphics
        /// 'Create a font and write title
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold)
        /// 'Create Ordered list as sublist of parent list
        /// Dim subList As PdfOrderedList = New PdfOrderedList()
        /// subList.Marker.Alignment = PdfListMarkerAlignment.Right
        /// subList.Marker.Delimiter = "."
        /// subList.Marker.StartNumber = 2
        /// subList.Marker.Style = PdfNumberStyle.UpperRoman
        /// 'Add items to the list
        /// subList.Items.Add("List of Essential Studio products")
        /// subList.Items.Add("IO products")
        /// subList.Items.Add("Grid products")
        /// subList.Items.Add("Tools products")
        /// 'Draw list
        /// subList.Draw(page, New RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height))
        /// document.Save("List.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class
        /// <seealso cref="PdfPage"/> Class
        /// <seealso cref="PdfOrderedList"/> Class
        public int StartNumber
        {
            get
            {
                return m_startNumber;
            }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Start number should be greater than 0");

                m_startNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets the delimiter.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfGraphics graphics = page.Graphics;
        /// //Create a font and write title
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold);           
        /// //Create Ordered list as sublist of parent list
        /// PdfOrderedList subList = new PdfOrderedList();
        /// subList.Marker.Alignment = PdfListMarkerAlignment.Right;
        /// subList.Marker.Delimiter = ".";
        /// subList.Marker.StartNumber = 2;
        /// subList.Marker.Style = PdfNumberStyle.UpperRoman;
        /// //Add items to the list
        /// subList.Items.Add("List of Essential Studio products");
        /// subList.Items.Add("IO products");
        /// subList.Items.Add("Grid products");
        /// subList.Items.Add("Tools products");
        /// //Draw list
        /// subList.Draw(page, new RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height));
        /// document.Save("List.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim graphics As PdfGraphics = page.Graphics
        /// 'Create a font and write title
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold)
        /// 'Create Ordered list as sublist of parent list
        /// Dim subList As PdfOrderedList = New PdfOrderedList()
        /// subList.Marker.Alignment = PdfListMarkerAlignment.Right
        /// subList.Marker.Delimiter = "."
        /// subList.Marker.StartNumber = 2
        /// subList.Marker.Style = PdfNumberStyle.UpperRoman
        /// 'Add items to the list
        /// subList.Items.Add("List of Essential Studio products")
        /// subList.Items.Add("IO products")
        /// subList.Items.Add("Grid products")
        /// subList.Items.Add("Tools products")
        /// 'Draw list
        /// subList.Draw(page, New RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height))
        /// document.Save("List.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class
        /// <seealso cref="PdfPage"/> Class
        /// <seealso cref="PdfOrderedList"/> Class
        public string Delimiter
        {
            get
            {
                if (m_delimiter == string.Empty || m_delimiter == null)
                {
                    return ".";
                }

                return m_delimiter;
            }
            set
            {
                m_delimiter = value;
            }
        }

        /// <summary>
        /// Gets or sets the suffix of the marker.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfGraphics graphics = page.Graphics;
        /// //Create a font and write title
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold);           
        /// //Create Ordered list as sublist of parent list
        /// PdfOrderedList subList = new PdfOrderedList();
        /// subList.Marker.Alignment = PdfListMarkerAlignment.Right;
        /// subList.Marker.Delimiter = ".";
        /// subList.Marker.StartNumber = 2;
        /// subList.Marker.Style = PdfNumberStyle.UpperRoman;
        /// //Add items to the list
        /// subList.Items.Add("List of Essential Studio products");
        /// subList.Items.Add("IO products");
        /// subList.Items.Add("Grid products");
        /// subList.Items.Add("Tools products");
        /// //Draw list
        /// subList.Draw(page, new RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height));
        /// document.Save("List.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim graphics As PdfGraphics = page.Graphics
        /// 'Create a font and write title
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold)
        /// 'Create Ordered list as sublist of parent list
        /// Dim subList As PdfOrderedList = New PdfOrderedList()
        /// subList.Marker.Alignment = PdfListMarkerAlignment.Right
        /// subList.Marker.Delimiter = "."
        /// subList.Marker.StartNumber = 2
        /// subList.Marker.Style = PdfNumberStyle.UpperRoman
        /// 'Add items to the list
        /// subList.Items.Add("List of Essential Studio products")
        /// subList.Items.Add("IO products")
        /// subList.Items.Add("Grid products")
        /// subList.Items.Add("Tools products")
        /// 'Draw list
        /// subList.Draw(page, New RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height))
        /// document.Save("List.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class
        /// <seealso cref="PdfPage"/> Class
        /// <seealso cref="PdfOrderedList"/> Class
        public string Suffix
        {
            get
            {
                if (m_suffix == null || m_suffix == string.Empty)
                {
                    return ".";
                }

                return m_suffix;
            }
            set
            {
                m_suffix = value;
            }
        }

        /// <summary>
        /// Gets or sets the index of the current item.
        /// </summary>
        internal int CurrentIndex
        {
            get
            {
                return m_currentIndex;
            }
            set
            {
                m_currentIndex = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfOrderedMarker"/> class.
        /// </summary>
        /// <param name="style">Number style of marker.</param>
        /// <param name="delimiter">Number delimiter of marker.</param>
        /// <param name="suffix">Number suffix of marker.</param>
        /// <param name="font">Number font of marker.</param>
        public PdfOrderedMarker(PdfNumberStyle style, string delimiter, string suffix, PdfFont font)
        {
            m_style = style;
            m_delimiter = delimiter;
            m_suffix = suffix;
            Font = font;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfOrderedMarker"/> class.
        /// </summary>
        /// <param name="style">Number style of marker.</param>
        /// <param name="suffix">Number suffix of the marker.</param>
        /// <param name="font">Number font of marker.</param>
        public PdfOrderedMarker(PdfNumberStyle style, string suffix, PdfFont font)
            : this(style, string.Empty, suffix, font)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfOrderedMarker"/> class.
        /// </summary>
        /// <param name="style">Number style of marker.</param>
        /// <param name="font">Number font of marker.</param>
        public PdfOrderedMarker(PdfNumberStyle style, PdfFont font)
            : this(style, string.Empty, string.Empty, font)
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Draw marker in specified point at graphics.
        /// </summary>
        /// <param name="graphics"> Pdf graphics.</param>
        /// <param name="point">The location point.</param>
        internal void Draw(PdfGraphics graphics, PointF point)
        {
            string number = GetNumber();

            graphics.DrawString(number + Suffix, Font, Brush, point);
        }

        /// <summary>
        /// Draw marker in specified point at page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="point">The point.</param>
        internal void Draw(PdfPage page, PointF point)
        {
            Draw(page.Graphics, point);
        }

        /// <summary>
        /// Gets the marker number.
        /// </summary>
        /// <returns>Number in string.</returns>
        internal string GetNumber()
        {
            return PdfNumbersConvertor.Convert(m_startNumber + m_currentIndex, m_style);
        }
        #endregion
    }
}
