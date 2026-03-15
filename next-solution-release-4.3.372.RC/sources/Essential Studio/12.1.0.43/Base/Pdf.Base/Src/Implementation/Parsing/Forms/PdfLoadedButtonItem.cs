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
    /// Represents button group item of an existing PDF document`s form.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Load an existing document
    /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
    /// // Load an existing button field
    /// PdfLoadedButtonField buttonField = doc.Form.Fields["Submit"] as PdfLoadedButtonField;
    /// // Load an existing button item
    /// PdfLoadedButtonItem buttonItem = buttonField.Items[0];
    /// buttonItem.Bounds = new RectangleF(0, 0, 20, 30);
    /// doc.Save("Form.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Load an existing document.
    /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
    /// ' Load an existing button field
    /// Dim buttonField As PdfLoadedButtonField = TryCast(doc.Form.Fields("Submit"), PdfLoadedButtonField)
    /// ' Load an existing button item
    /// Dim buttonItem As PdfLoadedButtonItem = buttonField.Items(0)
    /// buttonItem.Bounds = New RectangleF(0, 0, 20, 30)
    /// doc.Save("Form.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfLoadedDocument"/> Class    
    /// <seealso cref="PdfLoadedButtonField"/> Class 
    public class PdfLoadedButtonItem : PdfLoadedFieldItem
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLoadedButtonItem"/> class.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="index">The index.</param>
        /// <param name="dictionary">The dictionary.</param>
        internal PdfLoadedButtonItem(PdfLoadedStyledField field, int index, PdfDictionary dictionary)
            : base(field, index, dictionary)
        {
        }
        #endregion
    }
}
