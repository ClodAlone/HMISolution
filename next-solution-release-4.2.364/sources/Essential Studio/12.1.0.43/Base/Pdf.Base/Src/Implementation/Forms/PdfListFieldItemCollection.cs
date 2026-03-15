#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;

using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents list field item collection.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Create a new PDf document
    /// PdfDocument document = new PdfDocument();           
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = document.Pages.Add();                         
    /// //Create list box
    /// PdfListBoxField listBox = new PdfListBoxField(page, "list1");
    /// //Add the field to listbox.
    /// document.Form.Fields.Add(listBox);            
    /// //Set the properties.
    /// listBox.Bounds = new RectangleF(100, 350, 100, 50);
    /// listBox.HighlightMode = PdfHighlightMode.Outline;            
    /// // Creates list items
    /// PdfListFieldItemCollection itemCollection = listBox.Items;
    /// //Add the items to the list box
    /// itemCollection.Add(new PdfListFieldItem("English", "English"));
    /// itemCollection.Add(new PdfListFieldItem("French", "French"));
    /// itemCollection.Add(new PdfListFieldItem("German", "German"));
    /// listBox.SelectedIndex = 0;            
    /// document.Save("Form.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDf document
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Create a page
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create list box
    /// Dim listBox As PdfListBoxField = New PdfListBoxField(page, "list1")
    /// 'Add the field to listbox.
    /// document.Form.Fields.Add(listBox)
    /// 'Set the properties.
    /// listBox.Bounds = New RectangleF(100, 350, 100, 50)
    /// listBox.HighlightMode = PdfHighlightMode.Outline
    /// ' Creates list items
    /// Dim itemCollection As PdfListFieldItemCollection = listBox.Items
    /// 'Add the items to the list box
    /// itemCollection.Add(New PdfListFieldItem("English", "English"))
    /// itemCollection.Add(New PdfListFieldItem("French", "French"))
    /// itemCollection.Add(New PdfListFieldItem("German", "German"))
    /// listBox.SelectedIndex = 0
    /// document.Save("Form.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfCollection"/> Class  
    /// <seealso cref="IPdfWrapper"/> Interface 
    /// <seealso cref="PdfDocument"/> Class   
    /// <seealso cref="PdfPage"/> Class 
    /// <seealso cref="PdfListBoxField"/> Class 
    public class PdfListFieldItemCollection : PdfCollection, IPdfWrapper
    {
        #region Fields
        /// <summary>
        /// Internal variable to store array of 
        /// </summary>
        private PdfArray m_items = new PdfArray();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfListFieldItemCollection"/> class.
        /// </summary>
        public PdfListFieldItemCollection()
            : base()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the <see cref="Syncfusion.Pdf.Interactive.PdfListFieldItem"/> at the specified index.
        /// </summary>
        /// <value>The <see cref="PdfListFieldItem"/> object.</value>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();           
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();                       
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12f);
        /// //Create list box
        /// PdfListBoxField listBox = new PdfListBoxField(page, "list1");
        /// //Add the field to listbox.
        /// document.Form.Fields.Add(listBox);            
        /// //Set the properties.
        /// listBox.Bounds = new RectangleF(100, 350, 100, 50);
        /// listBox.HighlightMode = PdfHighlightMode.Outline;            
        /// // Creates list items
        /// PdfListFieldItemCollection itemCollection = listBox.Items;
        /// //Add the items to the list box
        /// itemCollection.Add(new PdfListFieldItem("English", "English"));
        /// itemCollection.Add(new PdfListFieldItem("French", "French"));
        /// itemCollection.Add(new PdfListFieldItem("German", "German"));
        /// // Reading the second item in the collection and assigning new values
        /// PdfListFieldItem item = itemCollection[1];
        /// item.Text = "Arabic";
        /// item.Value = "Arabic";
        /// listBox.SelectedIndex = 0;            
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier, 12f)
        /// 'Create list box
        /// Dim listBox As PdfListBoxField = New PdfListBoxField(page, "list1")
        /// 'Add the field to listbox.
        /// document.Form.Fields.Add(listBox)
        /// 'Set the properties.
        /// listBox.Bounds = New RectangleF(100, 350, 100, 50)
        /// listBox.HighlightMode = PdfHighlightMode.Outline
        /// ' Creates list items
        /// Dim itemCollection As PdfListFieldItemCollection = listBox.Items
        /// 'Add the items to the list box
        /// itemCollection.Add(New PdfListFieldItem("English", "English"))
        /// itemCollection.Add(New PdfListFieldItem("French", "French"))
        /// itemCollection.Add(New PdfListFieldItem("German", "German"))
        /// ' Reading the second item in the collection and assigning new values
        /// Dim item As PdfListFieldItem = itemCollection(1)
        /// item.Text = "Arabic"
        /// item.Value = "Arabic"
        /// listBox.SelectedIndex = 0
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class   
        /// <seealso cref="PdfPage"/> Class 
        /// <seealso cref="PdfListBoxField"/> Class 
        public PdfListFieldItem this[int index]
        {
            get
            {
                return (PdfListFieldItem)List[index];
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds the specified item in the collection.
        /// </summary>
        /// <param name="item">The <see cref="PdfListFieldItem"/> object which to be added in the collection.</param>
        /// <returns>item</returns>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();           
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();    
        /// //Create list box
        /// PdfListBoxField listBox = new PdfListBoxField(page, "list1");
        /// //Add the field to listbox.
        /// document.Form.Fields.Add(listBox);            
        /// //Set the properties.
        /// listBox.Bounds = new RectangleF(100, 350, 100, 50);
        /// listBox.HighlightMode = PdfHighlightMode.Outline;            
        /// // Creates list items
        /// PdfListFieldItemCollection itemCollection = listBox.Items;
        /// //Add the items to the list box
        /// itemCollection.Add(new PdfListFieldItem("English", "English"));
        /// itemCollection.Add(new PdfListFieldItem("French", "French"));
        /// itemCollection.Add(new PdfListFieldItem("German", "German"));
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier, 12f)
        /// 'Create list box
        /// Dim listBox As PdfListBoxField = New PdfListBoxField(page, "list1")
        /// 'Add the field to listbox.
        /// document.Form.Fields.Add(listBox)
        /// 'Set the properties.
        /// listBox.Bounds = New RectangleF(100, 350, 100, 50)
        /// listBox.HighlightMode = PdfHighlightMode.Outline
        /// ' Creates list items
        /// Dim itemCollection As PdfListFieldItemCollection = listBox.Items
        /// 'Add the items to the list box
        /// itemCollection.Add(New PdfListFieldItem("English", "English"))
        /// itemCollection.Add(New PdfListFieldItem("French", "French"))
        /// itemCollection.Add(New PdfListFieldItem("German", "German"))
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class   
        /// <seealso cref="PdfPage"/> Class 
        /// <seealso cref="PdfListBoxField"/> Class 
        public int Add(PdfListFieldItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            return DoAdd(item);
        }

        /// <summary>
        /// Inserts the list item field at the specified index.
        /// </summary>
        /// <param name="index">The index where to insert the new item.</param>
        /// <param name="item">The <see cref="PdfListFieldItem"/> object to be added to collection.</param>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();           
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();    
        /// //Create list box
        /// PdfListBoxField listBox = new PdfListBoxField(page, "list1");
        /// //Add the field to listbox.
        /// document.Form.Fields.Add(listBox);            
        /// //Set the properties.
        /// listBox.Bounds = new RectangleF(100, 350, 100, 50);
        /// listBox.HighlightMode = PdfHighlightMode.Outline;            
        /// // Creates list items
        /// PdfListFieldItemCollection itemCollection = listBox.Items;
        /// //Add the items to the list box
        /// itemCollection.Add(new PdfListFieldItem("English", "English"));
        /// itemCollection.Add(new PdfListFieldItem("French", "French"));
        /// itemCollection.Add(new PdfListFieldItem("German", "German"));
        /// PdfListFieldItem item = new PdfListFieldItem("Arabic", "Arabic");
        /// // Inserting an item at second position
        /// itemCollection.Insert(1, item);
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create list box
        /// Dim listBox As PdfListBoxField = New PdfListBoxField(page, "list1")
        /// 'Add the field to listbox.
        /// document.Form.Fields.Add(listBox)
        /// 'Set the properties.
        /// listBox.Bounds = New RectangleF(100, 350, 100, 50)
        /// listBox.HighlightMode = PdfHighlightMode.Outline
        /// ' Creates list items
        /// Dim itemCollection As PdfListFieldItemCollection = listBox.Items
        /// 'Add the items to the list box
        /// itemCollection.Add(New PdfListFieldItem("English", "English"))
        /// itemCollection.Add(New PdfListFieldItem("French", "French"))
        /// itemCollection.Add(New PdfListFieldItem("German", "German"))
        /// Dim item As PdfListFieldItem = New PdfListFieldItem("Arabic", "Arabic")
        /// ' Inserting an item at second position
        /// itemCollection.Insert(1, item)
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class   
        /// <seealso cref="PdfPage"/> Class 
        /// <seealso cref="PdfListBoxField"/> Class 
        public void Insert(int index, PdfListFieldItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            DoInsert(index, item);
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The <see cref="PdfListFieldItem"/> object which to be removed in the collection.</param>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();           
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();   
        /// //Create list box
        /// PdfListBoxField listBox = new PdfListBoxField(page, "list1");
        /// //Add the field to listbox.
        /// document.Form.Fields.Add(listBox);            
        /// //Set the properties.
        /// listBox.Bounds = new RectangleF(100, 350, 100, 50);            
        /// // Creates list items
        /// PdfListFieldItemCollection itemCollection = listBox.Items;
        /// //Add the items to the list box            
        /// PdfListFieldItem item = new PdfListFieldItem("English", "English");
        /// itemCollection.Add(item);
        /// itemCollection.Add(new PdfListFieldItem("French", "French"));
        /// itemCollection.Add(new PdfListFieldItem("German", "German"));
        /// // Remove an item from collection
        /// itemCollection.Remove(item);
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create list box
        /// Dim listBox As PdfListBoxField = New PdfListBoxField(page, "list1")
        /// 'Add the field to listbox.
        /// document.Form.Fields.Add(listBox)
        /// 'Set the properties.
        /// listBox.Bounds = New RectangleF(100, 350, 100, 50)
        /// ' Creates list items
        /// Dim itemCollection As PdfListFieldItemCollection = listBox.Items
        /// 'Add the items to the list box            
        /// Dim item As PdfListFieldItem = New PdfListFieldItem("English", "English")
        /// itemCollection.Add(item)
        /// itemCollection.Add(New PdfListFieldItem("French", "French"))
        /// itemCollection.Add(New PdfListFieldItem("German", "German"))
        /// ' Remove an item from collection
        /// itemCollection.Remove(item)
        /// document.Save("Form.pdf");
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class   
        /// <seealso cref="PdfPage"/> Class 
        /// <seealso cref="PdfListBoxField"/> Class 
        public void Remove(PdfListFieldItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            if (List.Contains(item))
            {
                DoRemove(item);
            }
        }

        /// <summary>
        /// Removes the item at the specified position.
        /// </summary>
        /// <param name="index">The index where to remove the item.</param>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();           
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();   
        /// //Create list box
        /// PdfListBoxField listBox = new PdfListBoxField(page, "list1");
        /// //Add the field to listbox.
        /// document.Form.Fields.Add(listBox);            
        /// //Set the properties.
        /// listBox.Bounds = new RectangleF(100, 350, 100, 50);            
        /// // Creates list items
        /// PdfListFieldItemCollection itemCollection = listBox.Items;
        /// //Add the items to the list box            
        /// PdfListFieldItem item = new PdfListFieldItem("English", "English");
        /// itemCollection.Add(item);
        /// itemCollection.Add(new PdfListFieldItem("French", "French"));
        /// itemCollection.Add(new PdfListFieldItem("German", "German"));
        /// // Remove an item from collection
        /// itemCollection.RemoveAt(1);
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create list box
        /// Dim listBox As PdfListBoxField = New PdfListBoxField(page, "list1")
        /// 'Add the field to listbox.
        /// document.Form.Fields.Add(listBox)
        /// 'Set the properties.
        /// listBox.Bounds = New RectangleF(100, 350, 100, 50)
        /// ' Creates list items
        /// Dim itemCollection As PdfListFieldItemCollection = listBox.Items
        /// 'Add the items to the list box            
        /// Dim item As PdfListFieldItem = New PdfListFieldItem("English", "English")
        /// itemCollection.Add(item)
        /// itemCollection.Add(New PdfListFieldItem("French", "French"))
        /// itemCollection.Add(New PdfListFieldItem("German", "German"))
        /// ' Remove an item from collection
        /// itemCollection.RemoveAt(1)
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class   
        /// <seealso cref="PdfPage"/> Class 
        /// <seealso cref="PdfListBoxField"/> Class 
        public void RemoveAt(int index)
        {
            if ((index < 0) || (index >= List.Count))
            {
                throw new ArgumentNullException("index");
            }

            DoRemoveAt(index);
        }

        /// <summary>
        /// Determines whether the item is contained by the collection.
        /// </summary>
        /// <param name="item">Check whether <see cref="PdfListFieldItem"/> object is exists in the collection or not.</param>
        /// <returns>
        /// <c>true</c> if the item is contained within the collection; otherwise, <c>false</c>.
        /// </returns>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();           
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();   
        /// //Create list box
        /// PdfListBoxField listBox = new PdfListBoxField(page, "list1");
        /// //Add the field to listbox.
        /// document.Form.Fields.Add(listBox);            
        /// //Set the properties.
        /// listBox.Bounds = new RectangleF(100, 350, 100, 50);            
        /// // Creates list items
        /// PdfListFieldItemCollection itemCollection = listBox.Items;
        /// //Add the items to the list box            
        /// PdfListFieldItem item = new PdfListFieldItem("English", "English");
        /// itemCollection.Add(item);
        /// itemCollection.Add(new PdfListFieldItem("French", "French"));
        /// itemCollection.Add(new PdfListFieldItem("German", "German"));
        /// if (itemCollection.Contains(item))
        ///  MessageBox.Show("Already, item has added!");
        /// else
        ///  itemCollection.Add(item);
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create list box
        /// Dim listBox As PdfListBoxField = New PdfListBoxField(page, "list1")
        /// 'Add the field to listbox.
        /// document.Form.Fields.Add(listBox)
        /// 'Set the properties.
        /// listBox.Bounds = New RectangleF(100, 350, 100, 50)
        /// ' Creates list items
        /// Dim itemCollection As PdfListFieldItemCollection = listBox.Items
        /// 'Add the items to the list box            
        /// Dim item As PdfListFieldItem = New PdfListFieldItem("English", "English")
        /// itemCollection.Add(item)
        /// itemCollection.Add(New PdfListFieldItem("French", "French"))
        /// itemCollection.Add(New PdfListFieldItem("German", "German"))
        /// If itemCollection.Contains(item) Then
        ///  MessageBox.Show("Already, item has added!")
        /// Else
        ///  itemCollection.Add(item)
        /// End If
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class   
        /// <seealso cref="PdfPage"/> Class 
        /// <seealso cref="PdfListBoxField"/> Class 
        public bool Contains(PdfListFieldItem item)
        {
            return List.Contains(item);
        }

        /// <summary>
        /// Gets the index of the specified item.
        /// </summary>
        /// <param name="item">A <see cref="PdfListFieldItem"/> object whose index is requested.</param>
        /// <returns>The index of the given item, -1 if the item does not exist.</returns>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();           
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();   
        /// //Create list box
        /// PdfListBoxField listBox = new PdfListBoxField(page, "list1");
        /// //Add the field to listbox.
        /// document.Form.Fields.Add(listBox);            
        /// //Set the properties.
        /// listBox.Bounds = new RectangleF(100, 350, 100, 50);            
        /// // Creates list items
        /// PdfListFieldItemCollection itemCollection = listBox.Items;
        /// //Add the items to the list box            
        /// PdfListFieldItem item = new PdfListFieldItem("English", "English");
        /// itemCollection.Add(item);
        /// itemCollection.Add(new PdfListFieldItem("French", "French"));
        /// itemCollection.Add(new PdfListFieldItem("German", "German"));
        /// // Gets the index of an item
        /// int index = itemCollection.IndexOf(item);
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create list box
        /// Dim listBox As PdfListBoxField = New PdfListBoxField(page, "list1")
        /// 'Add the field to listbox.
        /// document.Form.Fields.Add(listBox)
        /// 'Set the properties.
        /// listBox.Bounds = New RectangleF(100, 350, 100, 50)
        /// ' Creates list items
        /// Dim itemCollection As PdfListFieldItemCollection = listBox.Items
        /// 'Add the items to the list box            
        /// Dim item As PdfListFieldItem = New PdfListFieldItem("English", "English")
        /// itemCollection.Add(item)
        /// itemCollection.Add(New PdfListFieldItem("French", "French"))
        /// itemCollection.Add(New PdfListFieldItem("German", "German"))
        /// ' Gets the index of an item
        /// Dim index As Integer = itemCollection.IndexOf(item)
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class   
        /// <seealso cref="PdfPage"/> Class 
        /// <seealso cref="PdfListBoxField"/> Class 
        public int IndexOf(PdfListFieldItem item)
        {
            return List.IndexOf(item);
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();           
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();   
        /// //Create list box
        /// PdfListBoxField listBox = new PdfListBoxField(page, "list1");
        /// //Add the field to listbox.
        /// document.Form.Fields.Add(listBox);            
        /// //Set the properties.
        /// listBox.Bounds = new RectangleF(100, 350, 100, 50);            
        /// // Creates list items
        /// PdfListFieldItemCollection itemCollection = listBox.Items;
        /// //Add the items to the list box            
        /// PdfListFieldItem item = new PdfListFieldItem("English", "English");
        /// itemCollection.Add(item);
        /// itemCollection.Add(new PdfListFieldItem("French", "French"));
        /// itemCollection.Add(new PdfListFieldItem("German", "German"));
        /// // Clear the collection
        /// itemCollection.Clear();
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create list box
        /// Dim listBox As PdfListBoxField = New PdfListBoxField(page, "list1")
        /// 'Add the field to listbox.
        /// document.Form.Fields.Add(listBox)
        /// 'Set the properties.
        /// listBox.Bounds = New RectangleF(100, 350, 100, 50)
        /// ' Creates list items
        /// Dim itemCollection As PdfListFieldItemCollection = listBox.Items
        /// 'Add the items to the list box            
        /// Dim item As PdfListFieldItem = New PdfListFieldItem("English", "English")
        /// itemCollection.Add(item)
        /// itemCollection.Add(New PdfListFieldItem("French", "French"))
        /// itemCollection.Add(New PdfListFieldItem("German", "German"))
        /// ' Clear the collection
        /// itemCollection.Clear()
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class   
        /// <seealso cref="PdfPage"/> Class 
        /// <seealso cref="PdfListBoxField"/> Class 
        public void Clear()
        {
            m_items.Clear();
            List.Clear();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Index of the added item.</returns>
        private int DoAdd(PdfListFieldItem item)
        {
            m_items.Add((item as IPdfWrapper).Element);
            return List.Add(item);
        }

        /// <summary>
        /// Inserts the item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        private void DoInsert(int index, PdfListFieldItem item)
        {
            m_items.Insert(index, (item as IPdfWrapper).Element);
            List.Insert(index, item);
        }

        /// <summary>
        /// Removes the element at the specified position.
        /// </summary>
        /// <param name="index">The index.</param>
        private void DoRemoveAt(int index)
        {
            m_items.RemoveAt(index);
            List.RemoveAt(index);
        }

        /// <summary>
        /// Removes the item.
        /// </summary>
        /// <param name="item">The item.</param>
        private void DoRemove(PdfListFieldItem item)
        {
            int index = List.IndexOf(item);
            m_items.RemoveAt(index);
            List.RemoveAt(index);
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
                return m_items;
            }
        }
        #endregion
    }
}
