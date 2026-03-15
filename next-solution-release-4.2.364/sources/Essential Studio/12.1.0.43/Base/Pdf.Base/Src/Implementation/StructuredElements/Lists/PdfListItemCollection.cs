#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
using System.Drawing;
using Syncfusion.Pdf.Graphics;

/// <summary>
/// The Syncfusion.Pdf.Lists namespace contains classes for creating structure elements in PDF document.
/// </summary>
namespace Syncfusion.Pdf.Lists
{
    /// <summary>
    /// Represents collection of list items.
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
    /// 'Create a unordered list
    /// Dim list As PdfUnorderedList = New PdfUnorderedList(listItemCollection)
    /// 'Set the marker style
    /// list.Marker.Style = PdfUnorderedMarkerStyle.Disk
    /// list.Draw(page, New RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height))
    /// document.Save("List.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfCollection"/> Class    
    public class PdfListItemCollection :
            PdfCollection
    {
        #region Properties
        /// <summary>
        /// Gets the PdfListItem from collection at the specified index.
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
        /// 'Create a unordered list
        /// Dim list As PdfUnorderedList = New PdfUnorderedList(listItemCollection)
        /// 'Set the marker style
        /// list.Marker.Style = PdfUnorderedMarkerStyle.Disk
        /// list.Draw(page, New RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height))
        /// document.Save("List.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfListItemCollection"/> Class  
        public PdfListItem this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new IndexOutOfRangeException(
                        "The index should be less than item's count or more or equel to 0");

                return (PdfListItem)List[index];
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfListItemCollection"/> class.
        /// </summary>
        public PdfListItemCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfListItemCollection"/> class.
        /// </summary>
        /// <param name="items">A string array that contains items separated by the new line character.</param>
        public PdfListItemCollection(string[] items)
            : this()
        {
            if (items == null)
                throw new ArgumentNullException("items");

            foreach (string line in items)
            {
                Add(line);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The item index in collection.</returns>
        /// <example>
        /// <code lang="C#">
        /// // Creates an item collection
        /// PdfListItemCollection listItemCollection = new PdfListItemCollection();     
        /// // Creates an item
        /// PdfListItem item = new PdfListItem("Backoffice");
        /// item.TextIndent = 10;
        /// item.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 12);            
        /// // Creates Grid list item
        /// PdfListItem gridItem = new PdfListItem("Grid");
        /// gridItem.Brush = PdfBrushes.BlueViolet;
        /// gridItem.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 12);          
        /// // Adding items in collection
        /// listItemCollection.Add(item);
        /// listItemCollection.Add(gridItem);
        /// </code>
        /// <code lang="VB">
        /// ' Creates an item collection
        /// Dim listItemCollection As PdfListItemCollection = New PdfListItemCollection()
        /// ' Creates an item
        /// Dim item As PdfListItem = New PdfListItem("Backoffice")
        /// item.TextIndent = 10
        /// item.Font = New PdfStandardFont(PdfFontFamily.TimesRoman, 12)
        /// ' Creates Grid list item
        /// Dim gridItem As PdfListItem = New PdfListItem("Grid")
        /// gridItem.Brush = PdfBrushes.BlueViolet
        /// gridItem.Font = New PdfStandardFont(PdfFontFamily.TimesRoman, 12)
        /// ' Adding items in collection
        /// listItemCollection.Add(item)
        /// listItemCollection.Add(gridItem)
        /// </code>
        /// </example>
        public int Add(PdfListItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            return List.Add(item);
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="itemIndent">The item indent.</param>
        /// <example>
        /// <code lang="C#">
        /// // Creates an item collection
        /// PdfListItemCollection listItemCollection = new PdfListItemCollection();     
        /// // Creates an item
        /// PdfListItem item = new PdfListItem("Backoffice");
        /// item.TextIndent = 10;
        /// item.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 12);            
        /// // Creates Grid list item
        /// PdfListItem gridItem = new PdfListItem("Grid");
        /// gridItem.Brush = PdfBrushes.BlueViolet;
        /// gridItem.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 12);          
        /// // Adding items in collection
        /// listItemCollection.Add(item);
        /// listItemCollection.Add(gridItem);
        /// </code>
        /// <code lang="VB">
        /// ' Creates an item collection
        /// Dim listItemCollection As PdfListItemCollection = New PdfListItemCollection()
        /// ' Creates an item
        /// Dim item As PdfListItem = New PdfListItem("Backoffice")
        /// item.TextIndent = 10
        /// item.Font = New PdfStandardFont(PdfFontFamily.TimesRoman, 12)
        /// ' Creates Grid list item
        /// Dim gridItem As PdfListItem = New PdfListItem("Grid")
        /// gridItem.Brush = PdfBrushes.BlueViolet
        /// gridItem.Font = New PdfStandardFont(PdfFontFamily.TimesRoman, 12)
        /// ' Adding items in collection
        /// listItemCollection.Add(item)
        /// listItemCollection.Add(gridItem)
        /// </code>
        /// </example>
        public int Add(PdfListItem item, float itemIndent)
        {
            item.TextIndent = itemIndent;
            return Add(item);
        }

        /// <summary>
        /// Adds the item with a specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        /// <example>
        /// <code lang="C#">
        /// // Creates an item collection
        /// PdfListItemCollection listItemCollection = new PdfListItemCollection();     
        /// // Creates an item
        /// PdfListItem item = new PdfListItem("Backoffice");
        /// item.TextIndent = 10;
        /// item.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 12);            
        /// // Creates Grid list item
        /// PdfListItem gridItem = new PdfListItem("Grid");
        /// gridItem.Brush = PdfBrushes.BlueViolet;
        /// gridItem.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 12);          
        /// // Adding items in collection
        /// listItemCollection.Add(item, 10);
        /// listItemCollection.Add(gridItem, 8);
        /// </code>
        /// <code lang="VB">
        /// ' Creates an item collection
        /// Dim listItemCollection As PdfListItemCollection = New PdfListItemCollection()
        /// ' Creates an item
        /// Dim item As PdfListItem = New PdfListItem("Backoffice")
        /// item.TextIndent = 10
        /// item.Font = New PdfStandardFont(PdfFontFamily.TimesRoman, 12)
        /// ' Creates Grid list item
        /// Dim gridItem As PdfListItem = New PdfListItem("Grid")
        /// gridItem.Brush = PdfBrushes.BlueViolet
        /// gridItem.Font = New PdfStandardFont(PdfFontFamily.TimesRoman, 12)
        /// ' Adding items in collection
        /// listItemCollection.Add(item, 10)
        /// listItemCollection.Add(gridItem, 8)
        /// </code>
        /// </example>
        public PdfListItem Add(string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            PdfListItem item = new PdfListItem(text);

            List.Add(item);

            return item;
        }

        /// <summary>
        /// Adds the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="itemIndent">The item indent.</param>
        /// <returns>List item.</returns>
        /// <example>
        /// <code lang="C#">
        /// // Creates an item collection
        /// PdfListItemCollection listItemCollection = new PdfListItemCollection();
        /// // Adding items in collection
        /// listItemCollection.Add("Backoffice", 10);
        /// listItemCollection.Add("Grid", 8);
        /// </code>
        /// <code lang="VB">
        /// ' Creates an item collection
        /// Dim listItemCollection As PdfListItemCollection = New PdfListItemCollection()
        /// ' Adding items in collection
        /// listItemCollection.Add("Backoffice", 10)
        /// listItemCollection.Add("Grid", 8)
        /// </code>
        /// </example>
        public PdfListItem Add(string text, float itemIndent)
        {
            PdfListItem item = Add(text);
            item.TextIndent = itemIndent;

            return item;
        }

        /// <summary>
        /// Adds the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <returns>The item index in collection.</returns>
        /// <example>
        /// <code lang="C#">
        /// // Creates an item collection
        /// PdfListItemCollection listItemCollection = new PdfListItemCollection();
        /// // Adding items in collection
        /// listItemCollection.Add("Backoffice", new PdfStandardFont( PdfFontFamily.TimesRoman,10));
        /// listItemCollection.Add("Grid", new PdfStandardFont(PdfFontFamily.TimesRoman, 11));
        /// </code>
        /// <code lang="VB">
        /// ' Creates an item collection
        /// Dim listItemCollection As PdfListItemCollection = New PdfListItemCollection()
        /// ' Adding items in collection
        /// listItemCollection.Add("Backoffice", New PdfStandardFont(PdfFontFamily.TimesRoman,10))
        /// listItemCollection.Add("Grid", New PdfStandardFont(PdfFontFamily.TimesRoman, 11))
        /// </code>
        /// </example>
        public PdfListItem Add(string text, PdfFont font)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            if (font == null)
                throw new ArgumentNullException("font");

            PdfListItem item = new PdfListItem(text, font);

            List.Add(item);

            return item;
        }

        /// <summary>
        /// Adds the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="itemIndent">The item indent.</param>
        /// <returns>List item.</returns>
        /// <example>
        /// <code lang="C#">
        /// // Creates an item collection
        /// PdfListItemCollection listItemCollection = new PdfListItemCollection();
        /// // Adding items in collection
        /// listItemCollection.Add("Backoffice", new PdfStandardFont( PdfFontFamily.TimesRoman,10), 10);
        /// listItemCollection.Add("Grid", new PdfStandardFont(PdfFontFamily.TimesRoman, 11), 8);
        /// </code>
        /// <code lang="VB">
        /// ' Creates an item collection
        /// Dim listItemCollection As PdfListItemCollection = New PdfListItemCollection()
        /// ' Adding items in collection
        /// listItemCollection.Add("Backoffice", New PdfStandardFont(PdfFontFamily.TimesRoman,10), 10)
        /// listItemCollection.Add("Grid", New PdfStandardFont(PdfFontFamily.TimesRoman, 11), 8)
        /// </code>
        /// </example>
        public PdfListItem Add(string text, PdfFont font, float itemIndent)
        {
            PdfListItem item = Add(text, font);
            item.TextIndent = itemIndent;

            return item;
        }

        /// <summary>
        /// Inserts item at the specified index.
        /// </summary>
        /// <param name="index">The specified index.</param>
        /// <param name="item">The item.</param>
        /// <returns>The item index </returns>
        /// <example>
        /// <code lang="C#">
        /// string[] products = { "Tools", "Grid", "Chart", "Edit", "Diagram", "XlsIO", "Grouping", "Calculate", "PDF", "HTMLUI", "DocIO" };                          
        /// // Creates an item collection
        /// PdfListItemCollection listItemCollection = new PdfListItemCollection(products);
        /// PdfListItem newItem = new PdfListItem("PDF");
        /// // Insert a new item in the collection
        /// listItemCollection.Insert(0, newItem);
        /// </code>
        /// <code lang="VB">
        /// Dim products() As String = { "Tools", "Grid", "Chart", "Edit", "Diagram", "XlsIO", "Grouping", "Calculate", "PDF", "HTMLUI", "DocIO" }
        /// ' Creates an item collection
        /// Dim listItemCollection As PdfListItemCollection = New PdfListItemCollection(products)
        /// Dim newItem As PdfListItem = New PdfListItem("PDF")
        /// ' Insert a new item in the collection
        /// listItemCollection.Insert(0, newItem)
        /// </code>
        /// </example>
        public void Insert(int index, PdfListItem item)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentException(
                    "The index should be less than item's count or more or equal to 0", "index");

            if (item == null)
                throw new ArgumentNullException("item");

            List.Insert(index, item);
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        /// <param name="itemIndent">The item indent.</param>
        /// <example>
        /// <code lang="C#">
        /// string[] products = { "Tools", "Grid", "Chart", "Edit", "Diagram", "XlsIO", "Grouping", "Calculate", "PDF", "HTMLUI", "DocIO" };                          
        /// // Creates an item collection
        /// PdfListItemCollection listItemCollection = new PdfListItemCollection(products);
        /// PdfListItem newItem = new PdfListItem("PDF");
        /// // Insert a new item in the collection
        /// listItemCollection.Insert(0, newItem, 10);
        /// </code>
        /// <code lang="VB">
        /// Dim products() As String = { "Tools", "Grid", "Chart", "Edit", "Diagram", "XlsIO", "Grouping", "Calculate", "PDF", "HTMLUI", "DocIO" }
        /// ' Creates an item collection
        /// Dim listItemCollection As PdfListItemCollection = New PdfListItemCollection(products)
        /// Dim newItem As PdfListItem = New PdfListItem("PDF")
        /// ' Insert a new item in the collection
        /// listItemCollection.Insert(0, newItem, 10)
        /// </code>
        /// </example>
        public void Insert(int index, PdfListItem item, float itemIndent)
        {
            item.TextIndent = itemIndent;

            List.Insert(index, item);
        }

        /// <summary>
        /// Removes the specified item from the list.
        /// </summary>
        /// <param name="item">The specified item.</param>
        /// <example>
        /// <code lang="C#">
        /// string[] products = { "Tools", "Grid", "Chart", "Edit", "Diagram", "XlsIO", "Grouping", "Calculate", "PDF", "HTMLUI", "DocIO" };                          
        /// // Creates an item collection
        /// PdfListItemCollection listItemCollection = new PdfListItemCollection(products);
        /// PdfListItem toolsItem = new PdfListItem("Tools");
        /// // Remove 'Tools' list item
        /// listItemCollection.Remove(toolsItem);
        /// </code>
        /// <code lang="VB">
        /// Dim products() As String = { "Tools", "Grid", "Chart", "Edit", "Diagram", "XlsIO", "Grouping", "Calculate", "PDF", "HTMLUI", "DocIO" }
        /// ' Creates an item collection
        /// Dim listItemCollection As PdfListItemCollection = New PdfListItemCollection(products)
        /// Dim toolsItem As PdfListItem = New PdfListItem("Tools")
        /// ' Remove 'Tools' list item
        /// listItemCollection.Remove(toolsItem)
        /// </code>
        /// </example>
        public void Remove(PdfListItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (!List.Contains(item))
                throw new ArgumentException("The list doesn\'t contain this item", "item");

            List.Remove(item);
        }

        /// <summary>
        /// Removes the item at the specified index from the list.
        /// </summary>
        /// <param name="index">he specified index.</param>
        /// <example>
        /// <code lang="C#">
        /// string[] products = { "Tools", "Grid", "Chart", "Edit", "Diagram", "XlsIO", "Grouping", "Calculate", "PDF", "HTMLUI", "DocIO" };                          
        /// // Creates an item collection
        /// PdfListItemCollection listItemCollection = new PdfListItemCollection(products);         
        /// // Remove the firse item from the collection
        /// listItemCollection.RemoveAt(0);
        /// </code>
        /// <code lang="VB">
        /// Dim products() As String = { "Tools", "Grid", "Chart", "Edit", "Diagram", "XlsIO", "Grouping", "Calculate", "PDF", "HTMLUI", "DocIO" }
        /// ' Creates an item collection
        /// Dim listItemCollection As PdfListItemCollection = New PdfListItemCollection(products)
        /// ' Remove the firse item from the collection
        /// listItemCollection.RemoveAt(0)
        /// </code>
        /// </example>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentException(
                    "The index should be less than item's count or more or equal to 0", "index");

            List.RemoveAt(index);
        }

        /// <summary>
        /// Determines the index of a specific item in the list.
        /// </summary>
        /// <param name="item">The item to locate in the list. </param>
        /// <returns>The index of item if found in the list; otherwise, -1. </returns>
        /// <example>
        /// <code lang="C#">
        /// string[] products = { "Tools", "Grid", "Chart", "Edit", "Diagram", "XlsIO", "Grouping", "Calculate", "PDF", "HTMLUI", "DocIO" };                          
        /// // Creates an item collection
        /// PdfListItemCollection listItemCollection = new PdfListItemCollection(products);
        /// // Creates 'Tools' list item 
        /// PdfListItem toolsItem = new PdfListItem("Tools");
        /// int indexOf = listItemCollection.IndexOf(toolsItem);
        /// </code>
        /// <code lang="VB">
        /// Dim products() As String = { "Tools", "Grid", "Chart", "Edit", "Diagram", "XlsIO", "Grouping", "Calculate", "PDF", "HTMLUI", "DocIO" }
        /// ' Creates an item collection
        /// Dim listItemCollection As PdfListItemCollection = New PdfListItemCollection(products)
        /// ' Creates 'Tools' list item 
        /// Dim toolsItem As PdfListItem = New PdfListItem("Tools")
        /// Dim indexOf As Integer = listItemCollection.IndexOf(toolsItem)
        /// </code>
        /// </example>
        public int IndexOf(PdfListItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            return List.IndexOf(item);
        }

        /// <summary>
        /// Clears collection.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// string[] products = { "Tools", "Grid", "Chart", "Edit", "Diagram", "XlsIO", "Grouping", "Calculate", "PDF", "HTMLUI", "DocIO" };                          
        /// // Creates an item collection
        /// PdfListItemCollection listItemCollection = new PdfListItemCollection(products);         
        /// // Clears the list
        /// listItemCollection.Clear();
        /// </code>
        /// <code lang="VB">
        /// Dim products() As String = { "Tools", "Grid", "Chart", "Edit", "Diagram", "XlsIO", "Grouping", "Calculate", "PDF", "HTMLUI", "DocIO" }
        /// ' Creates an item collection
        /// Dim listItemCollection As PdfListItemCollection = New PdfListItemCollection(products)
        /// ' Clears the list
        /// listItemCollection.Clear()
        /// </code>
        /// </example>
        public void Clear()
        {
            List.Clear();
        }
        #endregion
    }
}
