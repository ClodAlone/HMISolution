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
    /// Represents unordered list.
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
    /// graphics.DrawString("List Features", font, PdfBrushes.DarkBlue, new PointF(225, 10));
    /// string[] products = { "Tools", "Grid", "Chart", "Edit", "Diagram", "XlsIO", "Grouping", "Calculate", "PDF", "HTMLUI", "DocIO" };
    /// string[] IO = { "XlsIO", "PDF", "DocIO" };
    /// font = new PdfStandardFont(PdfFontFamily.Helvetica, 12, PdfFontStyle.Regular);
    /// graphics.DrawString("This sample demonstrates various features of bullets and lists. A list can be ordered and Unordered. Essential PDF provides support for creating and formatting ordered and unordered lists.", font, PdfBrushes.Black, new RectangleF(0, 50, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height));
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
    /// graphics.DrawString("List Features", font, PdfBrushes.DarkBlue, New PointF(225, 10))
    /// Dim products() As String = { "Tools", "Grid", "Chart", "Edit", "Diagram", "XlsIO", "Grouping", "Calculate", "PDF", "HTMLUI", "DocIO" }
    /// Dim IO() As String = { "XlsIO", "PDF", "DocIO" }
    /// font = New PdfStandardFont(PdfFontFamily.Helvetica, 12, PdfFontStyle.Regular)
    /// graphics.DrawString("This sample demonstrates various features of bullets and lists. A list can be ordered and Unordered. Essential PDF provides support for creating and formatting ordered and unordered lists.", font, PdfBrushes.Black, New RectangleF(0, 50, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height))
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
    /// <seealso cref="PdfList"/> Class
    public class PdfUnorderedList :
            PdfList
    {
        #region Fields
        /// <summary>
        /// Marker for the list.
        /// </summary>
        private PdfUnorderedMarker m_marker;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the marker.
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
        /// graphics.DrawString("List Features", font, PdfBrushes.DarkBlue, new PointF(225, 10));
        /// string[] products = { "Tools", "Grid", "Chart", "Edit", "Diagram", "XlsIO", "Grouping", "Calculate", "PDF", "HTMLUI", "DocIO" };
        /// string[] IO = { "XlsIO", "PDF", "DocIO" };
        /// font = new PdfStandardFont(PdfFontFamily.Helvetica, 12, PdfFontStyle.Regular);
        /// graphics.DrawString("This sample demonstrates various features of bullets and lists. A list can be ordered and Unordered. Essential PDF provides support for creating and formatting ordered and unordered lists.", font, PdfBrushes.Black, new RectangleF(0, 50, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height));
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
        /// graphics.DrawString("List Features", font, PdfBrushes.DarkBlue, New PointF(225, 10))
        /// Dim products() As String = { "Tools", "Grid", "Chart", "Edit", "Diagram", "XlsIO", "Grouping", "Calculate", "PDF", "HTMLUI", "DocIO" }
        /// Dim IO() As String = { "XlsIO", "PDF", "DocIO" }
        /// font = New PdfStandardFont(PdfFontFamily.Helvetica, 12, PdfFontStyle.Regular)
        /// graphics.DrawString("This sample demonstrates various features of bullets and lists. A list can be ordered and Unordered. Essential PDF provides support for creating and formatting ordered and unordered lists.", font, PdfBrushes.Black, New RectangleF(0, 50, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height))
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
        /// <seealso cref="PdfDocument"/> Class
        /// <seealso cref="PdfPage"/> Class
        /// <seealso cref="PdfOrderedList"/> Class
        public PdfUnorderedMarker Marker
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
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfUnorderedList"/> class.
        /// </summary>
        public PdfUnorderedList()
            : this(CreateMarker(PdfUnorderedMarkerStyle.Disk))
        {
        }

        /// <summary>
        /// Creates unordered list using items.
        /// </summary>
        /// <param name="items">Items for a list.</param>
        public PdfUnorderedList(PdfListItemCollection items)
            : this(items, CreateMarker(PdfUnorderedMarkerStyle.Disk))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfUnorderedList"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        public PdfUnorderedList(PdfFont font)
            : base(font)
        {
            CreateMarker(PdfUnorderedMarkerStyle.Disk);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfUnorderedList"/> class.
        /// </summary>
        /// <param name="marker">The marker for the list.</param>
        public PdfUnorderedList(PdfUnorderedMarker marker)
            : base()
        {
            Marker = marker;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfUnorderedList"/> class.
        /// </summary>
        /// <param name="items">The items collection.</param>
        /// <param name="marker">The marker for the list.</param>
        public PdfUnorderedList(PdfListItemCollection items, PdfUnorderedMarker marker)
            : base(items)
        {
            Marker = marker;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfUnorderedList"/> class.
        /// </summary>
        /// <param name="text">The formatted text.</param>
        public PdfUnorderedList(string text)
            : this(text, CreateMarker(PdfUnorderedMarkerStyle.Disk))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfUnorderedList"/> class
        /// from formatted text that is splitted by new lines.
        /// </summary>
        /// <param name="text">The formatted text.</param>
        /// <param name="marker">The marker.</param>
        public PdfUnorderedList(string text, PdfUnorderedMarker marker)
            : this(CreateItems(text), marker)
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Creates the marker.
        /// </summary>
        /// <param name="style">The style marker of the marker.</param>
        /// <returns>Returns marker with specified style.</returns>
        private static PdfUnorderedMarker CreateMarker(PdfUnorderedMarkerStyle style)
        {
            return new PdfUnorderedMarker(style);
        }
        #endregion
    }
}
