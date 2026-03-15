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

/// <summary>
/// The Syncfusion.Pdf.Parsing namespace contains classes, which are used to load or modify an existing PDF document.
/// </summary>
namespace Syncfusion.Pdf.Parsing
{
    /// <summary>
    /// Represents loaded item collection.
    /// </summary>
    /// <seealso cref="PdfCollection"/> Class
    /// <example>
    /// <code lang="C#">
    /// //Load an existing document
    /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
    /// // Getting the 'course' list box field          
    /// PdfLoadedListBoxField listField = doc.Form.Fields["Course"] as PdfLoadedListBoxField;       
    /// // list field item Collection
    /// PdfLoadedListFieldItemCollection listItemCollection = listField.Items;
    /// listItemCollection[0].Bounds = new RectangleF(0, 0, 20, 30);            
    /// doc.Save("LoadedForm.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Load an existing document
    /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
    /// ' Getting the 'course' list box field          
    /// Dim listField As PdfLoadedListBoxField = TryCast(doc.Form.Fields("Course"), PdfLoadedListBoxField)
    /// ' list field item Collection
    /// Dim listItemCollection As PdfLoadedListFieldItemCollection = listField.Items
    /// listItemCollection(0).Bounds = New RectangleF(0, 0, 20, 30)
    /// doc.Save("LoadedForm.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfCollection"/> Class  
    public class PdfLoadedListFieldItemCollection : PdfCollection
    {
        #region Properties
        /// <summary>
        /// Gets the <see cref="Syncfusion.Pdf.Parsing.PdfLoadedListFieldItem"/> at the specified index.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// // Getting the 'course' list box field          
        /// PdfLoadedListBoxField listField = doc.Form.Fields["Course"] as PdfLoadedListBoxField;       
        /// // List field item Collection
        /// PdfLoadedListFieldItemCollection listItemCollection = listField.Items;
        /// // Reading the first item in the list items collection
        /// PdfLoadedListFieldItem listItem = listItemCollection[0];
        /// // Relocate the list item
        /// listItem.Location = new PointF(10, 20);
        /// doc.Save("LoadedForm.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Getting the 'course' list box field          
        /// Dim listField As PdfLoadedListBoxField = TryCast(doc.Form.Fields("Course"), PdfLoadedListBoxField)
        /// ' List field item Collection
        /// Dim listItemCollection As PdfLoadedListFieldItemCollection = listField.Items
        /// ' Reading the first item in the list items collection
        /// Dim listItem As PdfLoadedListFieldItem = listItemCollection(0)
        /// ' Relocate the list item
        /// listItem.Location = New PointF(10, 20)
        /// doc.Save("LoadedForm.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedDocument"/> Class  
        public PdfLoadedListFieldItem this[int index]
        {
            get
            {
                if ((index < 0) || (index >= Count))
                    throw new IndexOutOfRangeException("index");

                return (List[index] as PdfLoadedListFieldItem);
            }
        }
        #endregion

        #region Implementation
        internal void Add(PdfLoadedListFieldItem item)
        {
            if (item == null)
                throw new NullReferenceException("item");

            List.Add(item);
        }
        #endregion
    }
}
