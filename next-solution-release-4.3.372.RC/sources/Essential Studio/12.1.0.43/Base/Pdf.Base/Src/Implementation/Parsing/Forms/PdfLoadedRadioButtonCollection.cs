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
    /// Represents collection of radio box group items.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Load an existing document
    /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
    /// // Getting the 'Gender' radio button field          
    /// PdfLoadedRadioButtonListField radiobuttonField = doc.Form.Fields["Gender"] as PdfLoadedRadioButtonListField;
    /// // Radio button field collection
    /// PdfLoadedRadioButtonItemCollection radiobuttonFieldCollection =  radiobuttonField.Items;
    /// // Radio button field item
    /// PdfLoadedRadioButtonItem radiobuttonItem = radiobuttonFieldCollection[0];
    /// // Selected the item 
    /// radiobuttonItem.Checked = true;         
    /// doc.Save("LoadedForm.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Load an existing document
    /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
    /// ' Getting the  'Gender' radio button field         
    /// Dim radiobuttonField As PdfLoadedRadioButtonListField = TryCast(doc.Form.Fields("Gender"), PdfLoadedRadioButtonListField)
    /// ' Radio button field collection
    /// Dim radiobuttonFieldCollection As PdfLoadedRadioButtonItemCollection = radiobuttonField.Items
    /// ' Radio button field item
    /// Dim radiobuttonItem As PdfLoadedRadioButtonItem = radiobuttonFieldCollection(0)
    /// ' Selected the item 
    /// radiobuttonItem.Checked = True
    /// doc.Save("LoadedForm.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfLoadedStateItemCollection"/> Class    
    /// <seealso cref="PdfLoadedRadioButtonItem"/> Class
    /// <seealso cref="PdfLoadedDocument"/> Class
    public class PdfLoadedRadioButtonItemCollection : PdfLoadedStateItemCollection
    {
        #region Properties
        /// <summary>
        /// Gets the <see cref="Syncfusion.Pdf.Parsing.PdfLoadedRadioButtonItem"/> at the specified index.
        /// </summary>
        /// <returns>Returns <see cref="Syncfusion.Pdf.Parsing.PdfLoadedRadioButtonItem"/> object at the specified index.</returns>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// // Getting the 'Gender' radio button field          
        /// PdfLoadedRadioButtonListField radiobuttonField = doc.Form.Fields["Gender"] as PdfLoadedRadioButtonListField;
        /// // Radio button field collection
        /// PdfLoadedRadioButtonItemCollection radiobuttonFieldCollection =  radiobuttonField.Items;
        /// // Radio button field item
        /// PdfLoadedRadioButtonItem radiobuttonItem = radiobuttonFieldCollection[0];
        /// // Selected the item 
        /// radiobuttonItem.Checked = true;         
        /// doc.Save("LoadedForm.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Getting the  'Gender' radio button field         
        /// Dim radiobuttonField As PdfLoadedRadioButtonListField = TryCast(doc.Form.Fields("Gender"), PdfLoadedRadioButtonListField)
        /// ' Radio button field collection
        /// Dim radiobuttonFieldCollection As PdfLoadedRadioButtonItemCollection = radiobuttonField.Items
        /// ' Radio button field item
        /// Dim radiobuttonItem As PdfLoadedRadioButtonItem = radiobuttonFieldCollection(0)
        /// ' Selected the item 
        /// radiobuttonItem.Checked = True
        /// doc.Save("LoadedForm.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedStateItemCollection"/> Class    
        /// <seealso cref="PdfLoadedRadioButtonItem"/> Class
        /// <seealso cref="PdfLoadedDocument"/> Class
        new public PdfLoadedRadioButtonItem this[int index]
        {
            get
            {
                return (base[index] as PdfLoadedRadioButtonItem);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Index of the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The index of specified item</returns>
        internal int IndexOf(PdfLoadedRadioButtonItem item)
        {
            return base.IndexOf(item);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        internal void Add(PdfLoadedRadioButtonItem item)
        {
            base.Add(item);
        }
        #endregion
    }
}
