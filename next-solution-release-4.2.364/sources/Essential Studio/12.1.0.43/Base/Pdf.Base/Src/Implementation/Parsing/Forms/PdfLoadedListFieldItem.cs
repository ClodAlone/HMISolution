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
using System.Globalization;
using System.IO;
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
    /// Represents group item for list field.
    /// </summary>
    /// <seealso cref="PdfLoadedFieldItem"/> Class
    /// <example>
    /// <code lang="C#">
    /// // Loads an existing document
    /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
    /// // Load the list box field 
    /// PdfLoadedListBoxField listField = doc.Form.Fields["Course"] as PdfLoadedListBoxField;
    /// // Loaded list box field items
    /// PdfLoadedListFieldItem listFieldItem = listField.Items[0];
    /// listFieldItem.Location = new PointF(100, 200);
    /// doc.Save("Sample.pdf");
    /// </code>
    /// <code lang="VB">
    /// ' Loads an existing document
    /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
    /// ' Load the list box field 
    /// Dim listField As PdfLoadedListBoxField = TryCast(doc.Form.Fields("Course"), PdfLoadedListBoxField)
    /// ' Loaded list box field items
    /// Dim listFieldItem As PdfLoadedListFieldItem = listField.Items(0)
    /// listFieldItem.Location = New PointF(100, 200)
    /// doc.Save("Sample.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfLoadedFieldItem"/> Class  
    /// <seealso cref="PdfLoadedDocument"/> Class  
    /// <seealso cref="PdfLoadedListBoxField"/> Class  
    public class PdfLoadedListFieldItem : PdfLoadedFieldItem
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLoadedListFieldItem"/> class.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="index">The index.</param>
        /// <param name="dictionary">The dictionary.</param>
        internal PdfLoadedListFieldItem(PdfLoadedStyledField field, int index, PdfDictionary dictionary)
            : base(field, index, dictionary)
        {
        }
        #endregion
    }
}
