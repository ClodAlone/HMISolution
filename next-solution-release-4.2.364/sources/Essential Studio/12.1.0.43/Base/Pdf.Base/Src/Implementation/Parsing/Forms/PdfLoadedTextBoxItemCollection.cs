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
    /// //Load an existing document
    /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
    /// // Read the text box field item
    /// PdfLoadedTextBoxField textboxField = doc.Form.Fields[0] as PdfLoadedTextBoxField;
    /// // TextBox Item collection
    /// PdfLoadedTextBoxItemCollection textboxFieldCollection = textboxField.Items;
    /// textboxFieldCollection[0].Location = new PointF(10, 20);
    /// doc.Save("LoadedForm.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Load an existing document
    /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
    /// ' Read the text box field item
    /// Dim textboxField As PdfLoadedTextBoxField = TryCast(doc.Form.Fields(0), PdfLoadedTextBoxField)
    /// ' TextBox Item collection
    /// Dim textboxFieldCollection As PdfLoadedTextBoxItemCollection = textboxField.Items
    /// textboxFieldCollection(0).Location = New PointF(10, 20)
    /// doc.Save("LoadedForm.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfCollection"/> Class
    /// <seealso cref="PdfLoadedTextBoxField"/> Class
    /// <seealso cref="PdfLoadedDocument"/> Class
    public class PdfLoadedTextBoxItemCollection : PdfCollection
    {
        #region Properties
        /// <summary>
        /// Gets the <see cref="Syncfusion.Pdf.Parsing.PdfLoadedTexBoxItem"/> at the specified index.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// // Read the text box field item
        /// PdfLoadedTextBoxField textboxField = doc.Form.Fields[0] as PdfLoadedTextBoxField;
        /// // Read the text box item collection
        /// PdfLoadedTextBoxItemCollection textboxFieldCollection = textboxField.Items;
        /// PdfLoadedTexBoxItem textboxItem = textboxFieldCollection[0];
        /// textboxItem.Location = new PointF(10, 20);
        /// doc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Read the text box field item
        /// Dim textboxField As PdfLoadedTextBoxField = TryCast(doc.Form.Fields(0), PdfLoadedTextBoxField)
        /// ' Read the text box item collection
        /// Dim textboxFieldCollection As PdfLoadedTextBoxItemCollection = textboxField.Items
        /// Dim textboxItem As PdfLoadedTexBoxItem = textboxFieldCollection(0)
        /// textboxItem.Location = New PointF(10, 20)
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedDocument"/> Class
        /// <seealso cref="PdfLoadedTextBoxField"/> Class
        /// <seealso cref="PdfLoadedTextBoxItemCollection"/> Class
        public PdfLoadedTexBoxItem this[int index]
        {
            get
            {
                if ((index < 0) || (index >= Count))
                    throw new IndexOutOfRangeException("index");

                return (List[index] as PdfLoadedTexBoxItem);
            }
        }
        #endregion

        #region Implementation
        internal void Add(PdfLoadedTexBoxItem item)
        {
            if (item == null)
                throw new NullReferenceException("item");

            List.Add(item);
        }
        #endregion
    }
}
