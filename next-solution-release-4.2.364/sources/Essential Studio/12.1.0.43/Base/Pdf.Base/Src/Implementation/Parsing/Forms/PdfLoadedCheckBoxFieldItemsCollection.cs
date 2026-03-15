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
using System.Text;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.Parsing namespace contains classes, which are used to load or modify an existing PDF document.
/// </summary>
namespace Syncfusion.Pdf.Parsing
{
    /// <summary>
    /// Represents collection of text box group items.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Load an existing document.
    /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
    /// // Load an existing Check field
    /// PdfLoadedCheckBoxField checkField = doc.Form.Fields["Java"] as PdfLoadedCheckBoxField;
    /// // Loads the check box items collection.
    /// PdfLoadedCheckBoxItemCollection checkCollection = checkField.Items;
    /// checkCollection[0].Checked = false;            
    /// doc.Save("Form.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Load an existing document.
    /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
    /// ' Load an existing Check field
    /// Dim checkField As PdfLoadedCheckBoxField = TryCast(doc.Form.Fields("Java"), PdfLoadedCheckBoxField)
    /// ' Loads the check box items collection.
    /// Dim checkCollection As PdfLoadedCheckBoxItemCollection = checkField.Items
    /// checkCollection(0).Checked = False
    /// doc.Save("Form.pdf"
    /// </code>
    /// </example>
    /// <seealso cref="PdfLoadedStateField"/> Class 
    /// <seealso cref="PdfLoadedCheckBoxField"/> Class  
    /// <seealso cref="PdfLoadedStateItemCollection"/> Class 
    public class PdfLoadedCheckBoxItemCollection : PdfLoadedStateItemCollection
    {
        #region Properties
        /// <summary>
        /// Gets the <see cref="Syncfusion.Pdf.Parsing.PdfLoadedTexBoxItem"/> at the specified index.
        /// </summary>
        new public PdfLoadedCheckBoxItem this[int index]
        {
            get
            {
                return (base[index] as PdfLoadedCheckBoxItem);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Index of the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The index of specified item</returns>
        internal int IndexOf(PdfLoadedCheckBoxItem item)
        {
            return base.IndexOf(item);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        internal void Add(PdfLoadedCheckBoxItem item)
        {
            base.Add(item);
        }
        #endregion
    }
}
