#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Drawing;

using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;

/// <summary>
/// The Syncfusion.Pdf.Lists namespace contains classes for creating structure elements in PDF document.
/// </summary>
namespace Syncfusion.Pdf.Lists
{
    /// <summary>
    /// Represents the list item of the list.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Create a new PDf document
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = document.Pages.Add();
    /// PdfGraphics graphics = page.Graphics;
    /// string[] products = { "Tools", "Grid", "Chart", "Edit", "Diagram", "XlsIO", "Grouping", "Calculate", "PDF", "HTMLUI", "DocIO" };
    /// // Creates an item collection
    /// PdfListItemCollection listItemCollection = new PdfListItemCollection(products);            
    ///  // Creates an item
    ///  PdfListItem item = new PdfListItem("Tools");
    ///  item.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 12);
    ///  // Creates Grid list item
    ///  PdfListItem gridItem = new PdfListItem("Grid");
    ///  gridItem.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 12);
    ///  // Adding items in collection
    ///  listItemCollection.Add(item);
    ///  listItemCollection.Add(gridItem);
    /// //Create a unordered list
    /// PdfUnorderedList list = new PdfUnorderedList(listItemCollection);            
    /// //Set the marker style
    /// list.Marker.Style = PdfUnorderedMarkerStyle.Disk;            
    /// list.Draw(page, new RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height));
    /// document.Save("List.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDf document
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Create a page
    /// Dim page As PdfPage = document.Pages.Add()
    /// Dim graphics As PdfGraphics = page.Graphics
    /// Dim products() As String = { "Tools", "Grid", "Chart", "Edit", "Diagram", "XlsIO", "Grouping", "Calculate", "PDF", "HTMLUI", "DocIO" }
    /// ' Creates an item collection
    /// Dim listItemCollection As PdfListItemCollection = New PdfListItemCollection(products)
    /// ' Creates an item
    /// Dim item As PdfListItem = New PdfListItem("Tools")
    /// item.Font = New PdfStandardFont(PdfFontFamily.TimesRoman, 12)
    /// ' Creates Grid list item
    /// Dim gridItem As PdfListItem = New PdfListItem("Grid")
    /// gridItem.Font = New PdfStandardFont(PdfFontFamily.TimesRoman, 12)
    /// ' Adding items in collection
    /// listItemCollection.Add(item)
    /// listItemCollection.Add(gridItem)
    /// 'Create a unordered list
    /// Dim list As PdfUnorderedList = New PdfUnorderedList(listItemCollection)
    /// 'Set the marker style
    /// list.Marker.Style = PdfUnorderedMarkerStyle.Disk
    /// list.Draw(page, New RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height))
    /// document.Save("List.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfDocument"/> Class  
    /// <seealso cref="PdfPage"/> Class  
    public class PdfListItem
    {
        #region Fields
        /// <summary>
        /// Holds item font.
        /// </summary>
        private PdfFont m_font;

        /// <summary>
        /// Holds item text.
        /// </summary>
        private string m_text;

        /// <summary>
        /// Holds text format.
        /// </summary>
        private PdfStringFormat m_format;

        /// <summary>
        /// Holds pen.
        /// </summary>
        private PdfPen m_pen;

        /// <summary>
        /// Holds brush.
        /// </summary>
        private PdfBrush m_brush;

        /// <summary>
        /// Sub list.
        /// </summary>
        private PdfList m_list;

        /// <summary>
        /// Text indent for current item.
        /// </summary>
        private float m_textIndent;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets item font.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfGraphics graphics = page.Graphics;
        /// string[] products = { "Tools", "Grid", "Chart", "Edit", "Diagram", "XlsIO", "Grouping", "Calculate", "PDF", "HTMLUI", "DocIO" };
        /// // Creates an item collection
        /// PdfListItemCollection listItemCollection = new PdfListItemCollection(products);            
        ///  // Creates an item
        ///  PdfListItem item = new PdfListItem("Tools");
        ///  item.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 12);
        ///  // Creates Grid list item
        ///  PdfListItem gridItem = new PdfListItem("Grid");
        ///  gridItem.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 12);
        ///  // Adding items in collection
        ///  listItemCollection.Add(item);
        ///  listItemCollection.Add(gridItem);
        /// //Create a unordered list
        /// PdfUnorderedList list = new PdfUnorderedList(listItemCollection);            
        /// //Set the marker style
        /// list.Marker.Style = PdfUnorderedMarkerStyle.Disk;            
        /// list.Draw(page, new RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height));
        /// document.Save("List.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim graphics As PdfGraphics = page.Graphics
        /// Dim products() As String = { "Tools", "Grid", "Chart", "Edit", "Diagram", "XlsIO", "Grouping", "Calculate", "PDF", "HTMLUI", "DocIO" }
        /// ' Creates an item collection
        /// Dim listItemCollection As PdfListItemCollection = New PdfListItemCollection(products)
        /// ' Creates an item
        /// Dim item As PdfListItem = New PdfListItem("Tools")
        /// item.Font = New PdfStandardFont(PdfFontFamily.TimesRoman, 12)
        /// ' Creates Grid list item
        /// Dim gridItem As PdfListItem = New PdfListItem("Grid")
        /// gridItem.Font = New PdfStandardFont(PdfFontFamily.TimesRoman, 12)
        /// ' Adding items in collection
        /// listItemCollection.Add(item)
        /// listItemCollection.Add(gridItem)
        /// 'Create a unordered list
        /// Dim list As PdfUnorderedList = New PdfUnorderedList(listItemCollection)
        /// 'Set the marker style
        /// list.Marker.Style = PdfUnorderedMarkerStyle.Disk
        /// list.Draw(page, New RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height))
        /// document.Save("List.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfPage"/> Class 
        public PdfFont Font
        {
            get
            {
                return m_font;
            }
            set
            {
                m_font = value;
            }
        }

        /// <summary>
        /// Gets or sets item text.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfGraphics graphics = page.Graphics;
        /// string[] products = { "Tools", "Grid", "Chart", "Edit", "Diagram", "XlsIO", "Grouping", "Calculate", "PDF", "HTMLUI", "DocIO" };
        /// // Creates an item collection
        /// PdfListItemCollection listItemCollection = new PdfListItemCollection(products);            
        ///  // Creates an item
        ///  PdfListItem item = new PdfListItem("Tools");
        ///  item.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 12);
        ///  // Creates Grid list item
        ///  PdfListItem gridItem = new PdfListItem("Grid");
        /// gridItem.Text = "Grid";
        ///  gridItem.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 12);
        ///  // Adding items in collection
        ///  listItemCollection.Add(item);
        ///  listItemCollection.Add(gridItem);
        /// //Create a unordered list
        /// PdfUnorderedList list = new PdfUnorderedList(listItemCollection);            
        /// //Set the marker style
        /// list.Marker.Style = PdfUnorderedMarkerStyle.Disk;            
        /// list.Draw(page, new RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height));
        /// document.Save("List.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim graphics As PdfGraphics = page.Graphics
        /// Dim products() As String = { "Tools", "Grid", "Chart", "Edit", "Diagram", "XlsIO", "Grouping", "Calculate", "PDF", "HTMLUI", "DocIO" }
        /// ' Creates an item collection
        /// Dim listItemCollection As PdfListItemCollection = New PdfListItemCollection(products)
        /// ' Creates an item
        /// Dim item As PdfListItem = New PdfListItem("Tools")
        /// item.Font = New PdfStandardFont(PdfFontFamily.TimesRoman, 12)
        /// ' Creates Grid list item
        /// Dim gridItem As PdfListItem = New PdfListItem("Grid")
        /// gridItem.Font = New PdfStandardFont(PdfFontFamily.TimesRoman, 12)
        /// gridItem.Text = "Grid"
        /// ' Adding items in collection
        /// listItemCollection.Add(item)
        /// listItemCollection.Add(gridItem)
        /// 'Create a unordered list
        /// Dim list As PdfUnorderedList = New PdfUnorderedList(listItemCollection)
        /// 'Set the marker style
        /// list.Marker.Style = PdfUnorderedMarkerStyle.Disk
        /// list.Draw(page, New RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height))
        /// document.Save("List.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfPage"/> Class       
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
            }
        }

        /// <summary>
        /// Gets or sets item string format.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfGraphics graphics = page.Graphics;
        /// string[] products = { "Tools", "Grid", "Chart", "Edit", "Diagram", "XlsIO", "Grouping", "Calculate", "PDF", "HTMLUI", "DocIO" };
        /// // Creates an item collection
        /// PdfListItemCollection listItemCollection = new PdfListItemCollection(products);            
        ///  // Creates an item
        ///  PdfListItem item = new PdfListItem("Tools");
        ///  item.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 12);
        ///  // Creates Grid list item
        ///  PdfListItem gridItem = new PdfListItem("Grid");
        /// gridItem.StringFormat = new PdfStringFormat(PdfTextAlignment.Left);
        ///  gridItem.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 12);
        ///  // Adding items in collection
        ///  listItemCollection.Add(item);
        ///  listItemCollection.Add(gridItem);
        /// //Create a unordered list
        /// PdfUnorderedList list = new PdfUnorderedList(listItemCollection);            
        /// //Set the marker style
        /// list.Marker.Style = PdfUnorderedMarkerStyle.Disk;            
        /// list.Draw(page, new RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height));
        /// document.Save("List.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim graphics As PdfGraphics = page.Graphics
        /// Dim products() As String = { "Tools", "Grid", "Chart", "Edit", "Diagram", "XlsIO", "Grouping", "Calculate", "PDF", "HTMLUI", "DocIO" }
        /// ' Creates an item collection
        /// Dim listItemCollection As PdfListItemCollection = New PdfListItemCollection(products)
        /// ' Creates an item
        /// Dim item As PdfListItem = New PdfListItem("Tools")
        /// item.Font = New PdfStandardFont(PdfFontFamily.TimesRoman, 12)
        /// ' Creates Grid list item
        /// Dim gridItem As PdfListItem = New PdfListItem("Grid")
        /// gridItem.Font = New PdfStandardFont(PdfFontFamily.TimesRoman, 12)
        /// gridItem.StringFormat = New PdfStringFormat(PdfTextAlignment.Left)
        /// ' Adding items in collection
        /// listItemCollection.Add(item)
        /// listItemCollection.Add(gridItem)
        /// 'Create a unordered list
        /// Dim list As PdfUnorderedList = New PdfUnorderedList(listItemCollection)
        /// 'Set the marker style
        /// list.Marker.Style = PdfUnorderedMarkerStyle.Disk
        /// list.Draw(page, New RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height))
        /// document.Save("List.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfPage"/> Class        
        public PdfStringFormat StringFormat
        {
            get
            {
                return m_format;
            }
            set
            {
                m_format = value;
            }
        }

        /// <summary>
        /// Gets or sets list item pen.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfGraphics graphics = page.Graphics;
        /// string[] products = { "Tools", "Grid", "Chart", "Edit", "Diagram", "XlsIO", "Grouping", "Calculate", "PDF", "HTMLUI", "DocIO" };
        /// // Creates an item collection
        /// PdfListItemCollection listItemCollection = new PdfListItemCollection(products);            
        ///  // Creates an item
        ///  PdfListItem item = new PdfListItem("Tools");
        ///  item.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 12);
        ///  // Creates Grid list item
        ///  PdfListItem gridItem = new PdfListItem("Grid");
        /// gridItem.Pen = new PdfPen(PdfBrushes.Blue);
        ///  gridItem.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 12);
        ///  // Adding items in collection
        ///  listItemCollection.Add(item);
        ///  listItemCollection.Add(gridItem);
        /// //Create a unordered list
        /// PdfUnorderedList list = new PdfUnorderedList(listItemCollection);            
        /// //Set the marker style
        /// list.Marker.Style = PdfUnorderedMarkerStyle.Disk;            
        /// list.Draw(page, new RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height));
        /// document.Save("List.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim graphics As PdfGraphics = page.Graphics
        /// Dim products() As String = { "Tools", "Grid", "Chart", "Edit", "Diagram", "XlsIO", "Grouping", "Calculate", "PDF", "HTMLUI", "DocIO" }
        /// ' Creates an item collection
        /// Dim listItemCollection As PdfListItemCollection = New PdfListItemCollection(products)
        /// ' Creates an item
        /// Dim item As PdfListItem = New PdfListItem("Tools")
        /// item.Font = New PdfStandardFont(PdfFontFamily.TimesRoman, 12)
        /// ' Creates Grid list item
        /// Dim gridItem As PdfListItem = New PdfListItem("Grid")
        /// gridItem.Font = New PdfStandardFont(PdfFontFamily.TimesRoman, 12)
        /// gridItem.Pen = New PdfPen(PdfBrushes.Blue)
        /// ' Adding items in collection
        /// listItemCollection.Add(item)
        /// listItemCollection.Add(gridItem)
        /// 'Create a unordered list
        /// Dim list As PdfUnorderedList = New PdfUnorderedList(listItemCollection)
        /// 'Set the marker style
        /// list.Marker.Style = PdfUnorderedMarkerStyle.Disk
        /// list.Draw(page, New RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height))
        /// document.Save("List.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfPage"/> Class        
        public PdfPen Pen
        {
            get
            {
                return m_pen;
            }
            set
            {
                m_pen = value;
            }
        }

        /// <summary>
        /// Gets or sets list item brush.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfGraphics graphics = page.Graphics;
        /// string[] products = { "Tools", "Grid", "Chart", "Edit", "Diagram", "XlsIO", "Grouping", "Calculate", "PDF", "HTMLUI", "DocIO" };
        /// // Creates an item collection
        /// PdfListItemCollection listItemCollection = new PdfListItemCollection(products);            
        ///  // Creates an item
        ///  PdfListItem item = new PdfListItem("Tools");
        ///  item.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 12);
        ///  // Creates Grid list item
        ///  PdfListItem gridItem = new PdfListItem("Grid");
        /// gridItem.Brush = PdfBrushes.BlueViolet;
        ///  gridItem.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 12);
        ///  // Adding items in collection
        ///  listItemCollection.Add(item);
        ///  listItemCollection.Add(gridItem);
        /// //Create a unordered list
        /// PdfUnorderedList list = new PdfUnorderedList(listItemCollection);            
        /// //Set the marker style
        /// list.Marker.Style = PdfUnorderedMarkerStyle.Disk;            
        /// list.Draw(page, new RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height));
        /// document.Save("List.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim graphics As PdfGraphics = page.Graphics
        /// Dim products() As String = { "Tools", "Grid", "Chart", "Edit", "Diagram", "XlsIO", "Grouping", "Calculate", "PDF", "HTMLUI", "DocIO" }
        /// ' Creates an item collection
        /// Dim listItemCollection As PdfListItemCollection = New PdfListItemCollection(products)
        /// ' Creates an item
        /// Dim item As PdfListItem = New PdfListItem("Tools")
        /// item.Font = New PdfStandardFont(PdfFontFamily.TimesRoman, 12)
        /// ' Creates Grid list item
        /// Dim gridItem As PdfListItem = New PdfListItem("Grid")
        /// gridItem.Font = New PdfStandardFont(PdfFontFamily.TimesRoman, 12)
        /// gridItem.Brush = PdfBrushes.BlueViolet
        /// ' Adding items in collection
        /// listItemCollection.Add(item)
        /// listItemCollection.Add(gridItem)
        /// 'Create a unordered list
        /// Dim list As PdfUnorderedList = New PdfUnorderedList(listItemCollection)
        /// 'Set the marker style
        /// list.Marker.Style = PdfUnorderedMarkerStyle.Disk
        /// list.Draw(page, New RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height))
        /// document.Save("List.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfPage"/> Class           
        public PdfBrush Brush
        {
            get
            {
                return m_brush;
            }
            set
            {
                m_brush = value;
            }
        }

        /// <summary>
        /// Gets or sets sublist for item. 
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfGraphics graphics = page.Graphics;      
        /// // Creates an item collection
        /// PdfListItemCollection listItemCollection = new PdfListItemCollection();            
        /// // Creates an item
        /// PdfListItem item = new PdfListItem("Backoffice");
        /// item.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 12);            
        /// // Creates Grid list item
        /// PdfListItem gridItem = new PdfListItem("Grid");
        /// gridItem.Brush = PdfBrushes.BlueViolet;
        /// gridItem.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 12);          
        /// // Adding items in collection
        /// listItemCollection.Add(item);
        /// listItemCollection.Add(gridItem);
        /// //Create a unordered list
        /// PdfUnorderedList list = new PdfUnorderedList(listItemCollection);            
        /// //Set the marker style
        /// list.Marker.Style = PdfUnorderedMarkerStyle.Disk;
        /// //Create Ordered list as sublist of parent list
        /// PdfOrderedList subList = new PdfOrderedList();
        /// subList.Marker.Brush = PdfBrushes.Black;
        /// subList.Indent = 20;
        /// subList.Items.Add("Essential PDF");
        /// subList.Items.Add("Essential DocIO");
        /// subList.Items.Add("Essrntial XlsIO");            
        /// list.Items[0].SubList = subList;
        /// list.Draw(page, new RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height));
        /// document.Save("List.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim graphics As PdfGraphics = page.Graphics
        /// ' Creates an item collection
        /// Dim listItemCollection As PdfListItemCollection = New PdfListItemCollection()
        /// ' Creates an item
        /// Dim item As PdfListItem = New PdfListItem("Backoffice")
        /// item.Font = New PdfStandardFont(PdfFontFamily.TimesRoman, 12)
        /// ' Creates Grid list item
        /// Dim gridItem As PdfListItem = New PdfListItem("Grid")
        /// gridItem.Brush = PdfBrushes.BlueViolet
        /// gridItem.Font = New PdfStandardFont(PdfFontFamily.TimesRoman, 12)
        /// ' Adding items in collection
        /// listItemCollection.Add(item)
        /// listItemCollection.Add(gridItem)
        /// 'Create a unordered list
        /// Dim list As PdfUnorderedList = New PdfUnorderedList(listItemCollection)
        /// 'Set the marker style
        /// list.Marker.Style = PdfUnorderedMarkerStyle.Disk
        /// 'Create Ordered list as sublist of parent list
        /// Dim subList As PdfOrderedList = New PdfOrderedList()
        /// subList.Marker.Brush = PdfBrushes.Black		
        /// subList.Indent = 20
        /// subList.Items.Add("Essential PDF")
        /// subList.Items.Add("Essential DocIO")
        /// subList.Items.Add("Essrntial XlsIO")
        /// list.Items(0).SubList = subList
        /// list.Draw(page, New RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height))
        /// document.Save("List.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfPage"/> Class         
        public PdfList SubList
        {
            get
            {
                return m_list;
            }
            set
            {
                m_list = value;
            }
        }

        /// <summary>
        /// Gets or sets indent for item.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfGraphics graphics = page.Graphics;      
        /// // Creates an item collection
        /// PdfListItemCollection listItemCollection = new PdfListItemCollection();            
        /// // Creates an item
        /// PdfListItem item = new PdfListItem("Backoffice");
        /// item.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 12);            
        /// // Creates Grid list item
        /// PdfListItem gridItem = new PdfListItem("Grid");
        /// gridItem.TextIndent = 10;
        /// gridItem.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 12);          
        /// // Adding items in collection
        /// listItemCollection.Add(item);
        /// listItemCollection.Add(gridItem);
        /// //Create a unordered list
        /// PdfUnorderedList list = new PdfUnorderedList(listItemCollection);            
        /// //Set the marker style
        /// list.Marker.Style = PdfUnorderedMarkerStyle.Disk;
        /// //Create Ordered list as sublist of parent list
        /// PdfOrderedList subList = new PdfOrderedList();
        /// subList.Marker.Brush = PdfBrushes.Black;
        /// subList.Indent = 20;
        /// subList.Items.Add("Essential PDF");
        /// subList.Items.Add("Essential DocIO");
        /// subList.Items.Add("Essrntial XlsIO");            
        /// list.Items[0].SubList = subList;
        /// list.Draw(page, new RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height));
        /// document.Save("List.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim graphics As PdfGraphics = page.Graphics
        /// ' Creates an item collection
        /// Dim listItemCollection As PdfListItemCollection = New PdfListItemCollection()
        /// ' Creates an item
        /// Dim item As PdfListItem = New PdfListItem("Backoffice")
        /// item.Font = New PdfStandardFont(PdfFontFamily.TimesRoman, 12)
        /// ' Creates Grid list item
        /// Dim gridItem As PdfListItem = New PdfListItem("Grid")
        /// gridItem.TextIndent = 10
        /// gridItem.Font = New PdfStandardFont(PdfFontFamily.TimesRoman, 12)
        /// ' Adding items in collection
        /// listItemCollection.Add(item)
        /// listItemCollection.Add(gridItem)
        /// 'Create a unordered list
        /// Dim list As PdfUnorderedList = New PdfUnorderedList(listItemCollection)
        /// 'Set the marker style
        /// list.Marker.Style = PdfUnorderedMarkerStyle.Disk
        /// 'Create Ordered list as sublist of parent list
        /// Dim subList As PdfOrderedList = New PdfOrderedList()
        /// subList.Marker.Brush = PdfBrushes.Black		
        /// subList.Indent = 20
        /// subList.Items.Add("Essential PDF")
        /// subList.Items.Add("Essential DocIO")
        /// subList.Items.Add("Essrntial XlsIO")
        /// list.Items(0).SubList = subList
        /// list.Draw(page, New RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height))
        /// document.Save("List.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfPage"/> Class         
        public float TextIndent
        {
            get
            {
                return m_textIndent;
            }
            set
            {
                m_textIndent = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates new empty pdf list item.
        /// </summary>
        public PdfListItem()
            : this(string.Empty)
        {
        }

        /// <summary>
        /// Creates new pdf list item with default settings.
        /// </summary>
        public PdfListItem(string text)
            : this(text, null, null, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfListItem"/> class.
        /// </summary>
        /// <param name="text">The text of item.</param>
        /// <param name="font">The font of item.</param>
        public PdfListItem(string text, PdfFont font)
            : this(text, font, null, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfListItem"/> class.
        /// </summary>
        /// <param name="text">The text of item.</param>
        /// <param name="font">The font of item.</param>
        /// <param name="format">The string format.</param>
        public PdfListItem(string text, PdfFont font, PdfStringFormat format)
            : this(text, font, format, null, null)
        {
        }

        /// <summary>
        /// Creates new list item.
        /// </summary>
        /// <param name="text">The item text.</param>
        /// <param name="font">The item font.</param>
        /// <param name="format">The string format of item.</param>
        /// <param name="pen">The item pen.</param>
        /// <param name="brush">The item brush.</param>
        public PdfListItem(string text, PdfFont font, PdfStringFormat format,
                PdfPen pen, PdfBrush brush)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            m_text = text;
            m_font = font;
            m_format = format;
            m_pen = pen;
            m_brush = brush;
        }
        #endregion
    }
}
