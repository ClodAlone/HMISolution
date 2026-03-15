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
    /// Represents an item in a text box field collection.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Load an existing document
    /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
    /// // Load the text box field
    /// PdfLoadedTextBoxField textBoxField = doc.Form.Fields["EmployeeName"] as PdfLoadedTextBoxField;
    /// // Read the first text box field item from the collection
    /// PdfLoadedTexBoxItem textBoxItem = textBoxField.Items[0];
    /// textBoxItem.Location = new PointF(10, 20);
    /// doc.Save("LoadedForm.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Load an existing document
    /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
    /// ' Load the text box field
    /// Dim textBoxField As PdfLoadedTextBoxField = TryCast(doc.Form.Fields("EmployeeName"), PdfLoadedTextBoxField)
    /// ' Read the first text box field item from the collection
    /// Dim textBoxItem As PdfLoadedTexBoxItem = textBoxField.Items(0)
    /// textBoxItem.Location = New PointF(10, 20)
    /// doc.Save("LoadedForm.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfLoadedTextBoxField"/> Class
    /// <seealso cref="PdfLoadedDocument"/> Class
    /// <seealso cref="PdfLoadedFieldItem"/> Class
    public class PdfLoadedTexBoxItem : PdfLoadedFieldItem
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLoadedTexBoxItem"/> class.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="index">The index.</param>
        /// <param name="dictionary">The dictionary.</param>
        internal PdfLoadedTexBoxItem(PdfLoadedStyledField field, int index, PdfDictionary dictionary)
            : base(field, index, dictionary)
        {
        }
        #endregion
    }
}
