#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
using System.Text;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;
using Syncfusion.Pdf.Security;

/// <summary>
/// The Syncfusion.Pdf.Parsing namespace contains classes, which are used to load or modify an existing PDF document.
/// </summary>
namespace Syncfusion.Pdf.Parsing
{
    /// <summary>
    /// Represents a collection of list box field items.
    /// </summary>   
    /// <example>
    /// <code lang="C#">
    /// //Load an existing document.
    /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
    /// // Getting the 'course' list box field          
    /// PdfLoadedListBoxField listField = doc.Form.Fields["Course"] as PdfLoadedListBoxField;       
    /// // List field item Collection
    /// PdfLoadedListItemCollection listItemCollection = listField.Values;
    /// // Create a new list item
    /// PdfLoadedListItem listItem = new PdfLoadedListItem("Oracle", "Oracle");
    /// // Adding item in collection
    /// listItemCollection.Add(listItem);            
    /// doc.Save("LoadedForm.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Load an existing document
    /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
    /// ' Getting the 'course' list box field          
    /// Dim listField As PdfLoadedListBoxField = TryCast(doc.Form.Fields("Course"), PdfLoadedListBoxField)
    /// ' List field item Collection
    /// Dim listItemCollection As PdfLoadedListItemCollection = listField.Values
    /// ' Create a new list item
    /// Dim listItem As PdfLoadedListItem = New PdfLoadedListItem("Oracle", "Oracle")
    /// ' Adding item in collection
    /// listItemCollection.Add(listItem)
    /// doc.Save("LoadedForm.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfCollection"/> Class
    /// <seealso cref="PdfLoadedListBoxField"/> Class
    /// <seealso cref="PdfLoadedDocument"/> Class
    public class PdfLoadedListItemCollection : PdfCollection
    {
        #region Fields
        /// <summary>
        /// Parents field.
        /// </summary>
        private PdfLoadedChoiceField m_field;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the <see cref="Syncfusion.Pdf.Parsing.PdfLoadedRadioButtonItem"/> at the specified index.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");         
        /// // Getting the 'course' list box field
        /// PdfLoadedListBoxField listField = doc.Form.Fields["Course"] as PdfLoadedListBoxField;
        /// // List field item Collection
        /// PdfLoadedListItemCollection listItemCollection = listField.Values;
        /// // Getting the first item from the list item collection
        /// PdfLoadedListItem listItem = listItemCollection[0];
        /// listItem.Value = "C#.NET";
        /// doc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document.
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Getting the 'course' list box field
        /// Dim listField As PdfLoadedListBoxField = TryCast(doc.Form.Fields("Course"), PdfLoadedListBoxField)
        /// ' List field item Collection
        /// Dim listItemCollection As PdfLoadedListItemCollection = listField.Values
        /// ' Getting the first item from the list item collection
        /// Dim listItem As PdfLoadedListItem = listItemCollection(0)
        /// listItem.Value = "C#.NET"
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>        
        /// <seealso cref="PdfLoadedListBoxField"/> Class
        /// <seealso cref="PdfLoadedDocument"/> Class
        public PdfLoadedListItem this[int index]
        {
            get
            {
                if (index < 0 || index >= List.Count)
                    throw new IndexOutOfRangeException("Index");

                return List[index] as PdfLoadedListItem;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLoadedListItemCollection"/> class.
        /// </summary>
        /// <param name="field">The field.</param>
        internal PdfLoadedListItemCollection(PdfLoadedChoiceField field)
            : base()
        {
            m_field = field;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Inserts an item at the end of the collection. 
        /// </summary>
        /// <param name="item">a <see cref="PdfLoadedListItem"/>object to be added to collection.</param>
        /// <returns>The index of item.</returns>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// // Getting the 'course' list box field          
        /// PdfLoadedListBoxField listField = doc.Form.Fields["Course"] as PdfLoadedListBoxField;       
        /// // List field item Collection
        /// PdfLoadedListItemCollection listItemCollection = listField.Values;
        /// // Create a new list item
        /// PdfLoadedListItem listItem = new PdfLoadedListItem("Oracle", "Oracle");
        /// // Adding item in collection
        /// listItemCollection.Add(listItem);            
        /// doc.Save("LoadedForm.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Getting the 'course' list box field          
        /// Dim listField As PdfLoadedListBoxField = TryCast(doc.Form.Fields("Course"), PdfLoadedListBoxField)
        /// ' List field item Collection
        /// Dim listItemCollection As PdfLoadedListItemCollection = listField.Values
        /// ' Create a new list item
        /// Dim listItem As PdfLoadedListItem = New PdfLoadedListItem("Oracle", "Oracle")
        /// ' Adding item in collection
        /// listItemCollection.Add(listItem)
        /// doc.Save("LoadedForm.pdf")
        /// </code>
        /// </example>        
        /// <seealso cref="PdfLoadedListBoxField"/> Class
        /// <seealso cref="PdfLoadedDocument"/> Class
        public int Add(PdfLoadedListItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            PdfArray list = GetItems();
            PdfArray itemArray = GetArray(item);
            list.Add(itemArray);
            m_field.Dictionary.SetProperty(DictionaryProperties.Opt, list);

            return List.Add(item);
        }

        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The index of added item.</returns>
        internal int AddItem(PdfLoadedListItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            return List.Add(item);
        }

        /// <summary>
        /// Inserts the list item at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");         
        /// // Getting the 'course' list box field
        /// PdfLoadedListBoxField listField = doc.Form.Fields["Course"] as PdfLoadedListBoxField;
        /// // List field item Collection
        /// PdfLoadedListItemCollection listItemCollection = listField.Values;
        /// // Getting the first item from the list item collection
        /// PdfLoadedListItem listItem = listItemCollection[0];
        /// // Insert the item at first index
        /// listItemCollection.Insert(0, listItem);   
        /// doc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document.
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Getting the 'course' list box field
        /// Dim listField As PdfLoadedListBoxField = TryCast(doc.Form.Fields("Course"), PdfLoadedListBoxField)
        /// ' List field item Collection
        /// Dim listItemCollection As PdfLoadedListItemCollection = listField.Values
        /// ' Getting the first item from the list item collection
        /// Dim listItem As PdfLoadedListItem = listItemCollection(0)
        /// ' Insert the item at first index
        /// listItemCollection.Insert(0, listItem)
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedListBoxField"/> Class
        /// <seealso cref="PdfLoadedDocument"/> Class
        public void Insert(int index, PdfLoadedListItem item)
        {
            if (index < 0 || index > List.Count)
                throw new IndexOutOfRangeException("index");

            if (item == null)
                throw new ArgumentNullException("item");

            PdfArray list = GetItems();
            PdfArray itemArray = GetArray(item);
            list.Insert(index, itemArray);
            m_field.Dictionary.SetProperty(DictionaryProperties.Opt, list);

            List.Insert(index, item);
        }

        /// <summary>
        /// Removes the element at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <remarks>Throws IndexOutOfRange exception if the index is out of bounds.</remarks>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");         
        /// // Getting the 'course' list box field
        /// PdfLoadedListBoxField listField = doc.Form.Fields["Course"] as PdfLoadedListBoxField;
        /// // List field item Collection
        /// PdfLoadedListItemCollection listItemCollection = listField.Values;
        /// // Remove the first item
        /// listItemCollection.RemoveAt(0);
        /// doc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document.
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Getting the 'course' list box field
        /// Dim listField As PdfLoadedListBoxField = TryCast(doc.Form.Fields("Course"), PdfLoadedListBoxField)
        /// ' List field item Collection
        /// Dim listItemCollection As PdfLoadedListItemCollection = listField.Values
        /// ' Remove the first item
        /// listItemCollection.RemoveAt(0)
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedListBoxField"/> Class
        /// <seealso cref="PdfLoadedDocument"/> Class
        public void RemoveAt(int index)
        {
            if (index < 0 || index > List.Count)
                throw new IndexOutOfRangeException("index");

            PdfArray list = GetItems();
            list.RemoveAt(index);
            m_field.Dictionary.SetProperty(DictionaryProperties.Opt, list);

            List.RemoveAt(index);
        }

        /// <summary>
        /// Clears the item collection.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");         
        /// // Getting the 'course' list box field
        /// PdfLoadedListBoxField listField = doc.Form.Fields["Course"] as PdfLoadedListBoxField;
        /// // List field item Collection
        /// PdfLoadedListItemCollection listItemCollection = listField.Values;
        /// // Clears the collection
        /// listItemCollection.Clear();
        /// doc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document.
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Getting the 'course' list box field
        /// Dim listField As PdfLoadedListBoxField = TryCast(doc.Form.Fields("Course"), PdfLoadedListBoxField)
        /// ' List field item Collection
        /// Dim listItemCollection As PdfLoadedListItemCollection = listField.Values
        /// ' Clears the collection
        /// listItemCollection.Clear()
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedListBoxField"/> Class
        /// <seealso cref="PdfLoadedDocument"/> Class
        public void Clear()
        {
            PdfArray list = GetItems();
            list.Clear();
            m_field.Dictionary.SetProperty(DictionaryProperties.Opt, list);
            List.Clear();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the items.
        /// </summary>
        private PdfArray GetItems()
        {
            PdfArray array = new PdfArray();

            if (m_field.Dictionary.ContainsKey(DictionaryProperties.Opt))
            {
                array = m_field.CrossTable.GetObject(m_field.Dictionary[DictionaryProperties.Opt]) as PdfArray;
            }

            return array;
        }

        /// <summary>
        /// Gets the array.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The array of item value and text.</returns>
        private PdfArray GetArray(PdfLoadedListItem item)
        {
            PdfArray array = new PdfArray();

            if (item.Value != String.Empty)
            {
                array.Add(new PdfString(item.Value));
            }

            if (item.Text != String.Empty)
            {
                array.Add(new PdfString(item.Text));
            }

            return array;
        }
        #endregion
    }
}
