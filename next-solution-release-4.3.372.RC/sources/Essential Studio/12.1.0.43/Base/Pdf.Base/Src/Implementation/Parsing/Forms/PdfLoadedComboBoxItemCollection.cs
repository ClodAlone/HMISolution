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
    /// Represents collection of Combo box items.
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
    /// <seealso cref="PdfCollection"/> Class  
    /// <seealso cref="PdfLoadedComboBoxField"/> Class  
    /// <seealso cref="PdfLoadedDocument"/> Class  
    public class PdfLoadedComboBoxItemCollection : PdfCollection
    {
        #region Properties
        /// <summary>
        /// Gets the <see cref="Syncfusion.Pdf.Parsing.PdfLoadedTexBoxItem"/> at the specified index.
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
        /// <seealso cref="PdfCollection"/> Class  
        /// <seealso cref="PdfLoadedComboBoxField"/> Class  
        /// <seealso cref="PdfLoadedDocument"/> Class  
        public PdfLoadedComboBoxItem this[int index]
        {
            get
            {
                if ((index < 0) || (index >= Count))
                    throw new IndexOutOfRangeException("index");

                return (List[index] as PdfLoadedComboBoxItem);
            }
        }
        #endregion

        #region Implementation
        internal void Add(PdfLoadedComboBoxItem item)
        {
            if (item == null)
                throw new NullReferenceException("item");

            List.Add(item);
        }
        #endregion
    }
}
