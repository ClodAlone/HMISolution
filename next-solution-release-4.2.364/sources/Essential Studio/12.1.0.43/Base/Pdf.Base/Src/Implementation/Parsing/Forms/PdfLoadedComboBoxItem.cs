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
    /// Represents group for combo box field.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Load an existing document
    /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
    /// // Load an existing combo field
    /// PdfLoadedComboBoxField comboField = doc.Form.Fields["EmployeeCombo"] as PdfLoadedComboBoxField;
    /// // Load combo field collection
    /// PdfLoadedComboBoxItemCollection comboCollection = comboField.Items;
    /// // Load combo field item
    /// PdfLoadedComboBoxItem comboItem = comboCollection[0];
    /// comboItem.Bounds = new RectangleF(10,20,200,300);
    /// doc.Save("Form.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Load an existing document
    /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
    /// ' Load an existing combo field
    /// Dim comboField As PdfLoadedComboBoxField = TryCast(doc.Form.Fields("EmployeeCombo"), PdfLoadedComboBoxField)
    /// ' Load combo field collection
    /// Dim comboCollection As PdfLoadedComboBoxItemCollection = comboField.Items
    ///' Load combo field item
    /// Dim comboItem As PdfLoadedComboBoxItem = comboCollection(0)
    /// comboItem.Bounds = New RectangleF(10,20,200,300)
    /// doc.Save("Form.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfLoadedFieldItem"/> Class   
    public class PdfLoadedComboBoxItem : PdfLoadedFieldItem
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLoadedComboBoxItem"/> class.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="index">The index.</param>
        /// <param name="dictionary">The dictionary.</param>
        internal PdfLoadedComboBoxItem(PdfLoadedStyledField field, int index, PdfDictionary dictionary)
            : base(field, index, dictionary)
        {
        }
        #endregion
    }
}
