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
    /// Represents collection of button item.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    ///  //Load an existing document.
    ///  PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
    ///  // Load the form
    ///  PdfLoadedForm form = doc.Form;
    ///  // Load an existing button field
    ///  PdfLoadedButtonField buttonField = doc.Form.Fields["Submit"] as PdfLoadedButtonField;
    ///  // Load an existing button collection
    ///  PdfLoadedButtonItemCollection buttonCollection = buttonField.Items;
    ///  // Load an existing button button item
    ///  PdfLoadedButtonItem buttonItem = buttonCollection[0];
    ///  buttonItem.Bounds = new RectangleF(0, 0, 20, 30);          
    ///  doc.Save("Form.pdf");
    /// </code>
    /// <code lang="VB">
    ///  'Load an existing document.
    ///  Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
    ///  ' Load the form
    ///  Dim form As PdfLoadedForm = doc.Form
    ///  ' Load an existing button field
    ///  Dim buttonField As PdfLoadedButtonField = TryCast(doc.Form.Fields("Submit"), PdfLoadedButtonField)
    ///  ' Load an existing button collection
    ///  Dim buttonCollection As PdfLoadedButtonItemCollection = buttonField.Items
    ///  ' Load an existing button button item
    ///  Dim buttonItem As PdfLoadedButtonItem = buttonCollection(0)
    ///  buttonItem.Bounds = New RectangleF(0, 0, 20, 30)
    ///  doc.Save("Form.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfLoadedDocument"/> Class    
    /// <seealso cref="PdfLoadedButtonField"/> Class 
    /// <seealso cref="PdfCollection"/> Class  
    public class PdfLoadedButtonItemCollection : PdfCollection
    {
        #region Properties
        /// <summary>
        /// Gets the <see cref="Syncfusion.Pdf.Parsing.PdfLoadedTexBoxItem"/> at the specified index.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        ///  //Load an existing document.
        ///  PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        ///  // Load the form
        ///  PdfLoadedForm form = doc.Form;
        ///  // Load an existing button field
        ///  PdfLoadedButtonField buttonField = doc.Form.Fields["Submit"] as PdfLoadedButtonField;
        ///  // Load an existing button collection
        ///  PdfLoadedButtonItemCollection buttonCollection = buttonField.Items;
        ///  // Load an existing button button item
        ///  PdfLoadedButtonItem buttonItem = buttonCollection[0];
        ///  buttonItem.Bounds = new RectangleF(0, 0, 20, 30);          
        ///  doc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        ///  'Load an existing document.
        ///  Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        ///  ' Load the form
        ///  Dim form As PdfLoadedForm = doc.Form
        ///  ' Load an existing button field
        ///  Dim buttonField As PdfLoadedButtonField = TryCast(doc.Form.Fields("Submit"), PdfLoadedButtonField)
        ///  ' Load an existing button collection
        ///  Dim buttonCollection As PdfLoadedButtonItemCollection = buttonField.Items
        ///  ' Load an existing button button item
        ///  Dim buttonItem As PdfLoadedButtonItem = buttonCollection(0)
        ///  buttonItem.Bounds = New RectangleF(0, 0, 20, 30)
        ///  doc.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedDocument"/> Class    
        /// <seealso cref="PdfLoadedButtonField"/> Class 
        public PdfLoadedButtonItem this[int index]
        {
            get
            {
                if ((index < 0) || (index >= Count))
                    throw new IndexOutOfRangeException("index");

                return (List[index] as PdfLoadedButtonItem);
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        internal void Add(PdfLoadedButtonItem item)
        {
            if (item == null)
                throw new NullReferenceException("item");

            List.Add(item);
        }
        #endregion
    }
}
