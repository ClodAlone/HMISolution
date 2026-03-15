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
    /// Represents the ordered list.
    /// </summary>
    /// <seealso cref="PdfList"/> Class   
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
    public class PdfOrderedList :
            PdfList
    {
        #region Fields
        /// <summary>
        /// Marker of the list.
        /// </summary>
        private PdfOrderedMarker m_marker;

        /// <summary>
        /// True if user want to use numbering hierarchy, otherwise false.
        /// </summary>
        private bool m_useHierarchy;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets marker of the list items.
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
        /// <seealso cref="PdfDocument"/> Class
        /// <seealso cref="PdfPage"/> Class
        /// <seealso cref="PdfOrderedList"/> Class
        public PdfOrderedMarker Marker
        {
            get
            {
                return m_marker;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("marker");

                m_marker = value;
            }
        }

        /// <summary>
        /// True if user want to use numbering hierarchy, otherwise false.
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
        ///  subList.MarkerHierarchy = true;
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
        ///  subList.MarkerHierarchy = True
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
        /// <seealso cref="PdfDocument"/> Class
        /// <seealso cref="PdfPage"/> Class
        /// <seealso cref="PdfOrderedList"/> Class      
        public bool MarkerHierarchy
        {
            get
            {
                return m_useHierarchy;
            }
            set
            {
                m_useHierarchy = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates ordered list.
        /// </summary>
        public PdfOrderedList()
            : this(CreateMarker(PdfNumberStyle.Numeric))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfOrderedList"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        public PdfOrderedList(PdfFont font)
            : base(font)
        {
            CreateMarker(PdfNumberStyle.Numeric);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfOrderedList"/> class.
        /// </summary>
        /// <param name="style">The style.</param>
        public PdfOrderedList(PdfNumberStyle style)
        {
            Marker = CreateMarker(style);
        }

        /// <summary>
        /// Creates ordered list using items.
        /// </summary>
        /// <param name="items">Items for a list.</param>
        public PdfOrderedList(PdfListItemCollection items)
            : this(items, CreateMarker(PdfNumberStyle.Numeric))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfOrderedList"/> class.
        /// </summary>
        /// <param name="marker">The marker for the list.</param>
        public PdfOrderedList(PdfOrderedMarker marker)
            : base()
        {
            Marker = marker;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfOrderedList"/> class.
        /// </summary>
        /// <param name="items">The item collection.</param>
        /// <param name="marker">The marker for the list.</param>
        public PdfOrderedList(PdfListItemCollection items, PdfOrderedMarker marker)
            : base(items)
        {
            Marker = marker;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfOrderedList"/> class.
        /// </summary>
        /// <param name="text">The formatted text.</param>
        public PdfOrderedList(string text)
            : this(text, CreateMarker(PdfNumberStyle.Numeric))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfOrderedList"/> class
        /// from formatted text that is splitted by new lines.
        /// </summary>
        /// <param name="text">The formatted text.</param>
        /// <param name="marker">The marker.</param>
        public PdfOrderedList(string text, PdfOrderedMarker marker)
            : this(CreateItems(text), marker)
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Creates the marker.
        /// </summary>
        /// <param name="style">The style of the list marker.</param>
        /// <returns>Returns marker with specified style.</returns>
        private static PdfOrderedMarker CreateMarker(PdfNumberStyle style)
        {
            return new PdfOrderedMarker(style, null);
        }
        #endregion
    }
}
