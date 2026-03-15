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
using Syncfusion.Pdf.Security;

/// <summary>
/// The Syncfusion.Pdf.Parsing namespace contains classes, which are used to load or modify an existing PDF document.
/// </summary>
namespace Syncfusion.Pdf.Parsing
{
    /// <summary>
    /// Represents loaded check box item.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Load an existing document.
    /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
    /// // Load an existing Check field
    /// PdfLoadedCheckBoxField checkField = doc.Form.Fields["Java"] as PdfLoadedCheckBoxField;
    /// // Loads the check box items collection.
    /// PdfLoadedCheckBoxItemCollection checkCollection = checkField.Items;
    /// // Read the first item of the collection
    /// PdfLoadedCheckBoxItem checkItem = checkCollection[0];
    /// checkItem.Checked = false;            
    /// doc.Save("Form.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Load an existing document.
    /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
    /// ' Load an existing Check field
    /// Dim checkField As PdfLoadedCheckBoxField = TryCast(doc.Form.Fields("Java"), PdfLoadedCheckBoxField)
    /// ' Loads the check box items collection.
    /// Dim checkCollection As PdfLoadedCheckBoxItemCollection = checkField.Items
    /// ' Read the first item of the collection
    /// Dim checkItem As PdfLoadedCheckBoxItem = checkCollection(0)
    /// checkItem.Checked = False
    /// doc.Save("Form.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfLoadedStateItem"/> Class    
    /// <seealso cref="PdfLoadedCheckBoxItemCollection"/> Class
    /// <seealso cref="PdfLoadedDocument"/> Class
    public class PdfLoadedCheckBoxItem : PdfLoadedStateItem
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLoadedCheckBoxItem"/> class.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="index">The index.</param>
        /// <param name="dictionary">The dictionary.</param>
        internal PdfLoadedCheckBoxItem(PdfLoadedStyledField field, int index, PdfDictionary dictionary)
            : base(field, index, dictionary)
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Sets checked status of the field.
        /// </summary>
        /// <param name="value">Checked status.</param>
        private void SetCheckedStatus(bool value)
        {
            bool check = value;
            string val = PdfLoadedCheckBoxField.GetItemValue(Dictionary, CrossTable);

            if (check)
            {
                (Parent as PdfLoadedCheckBoxField).UncheckOthers(this, val, value);

                Parent.Dictionary.SetName(DictionaryProperties.V, val);
                Dictionary.SetProperty(DictionaryProperties.AS, new PdfName(val));
            }
            else
            {
                PdfName v = PdfCrossTable.Dereference(Parent.Dictionary[DictionaryProperties.V]) as PdfName;

                if (v != null && val == v.Value)
                {
                    Parent.Dictionary.Remove(DictionaryProperties.V);
                }

                Dictionary.SetProperty(DictionaryProperties.AS, new PdfName(DictionaryProperties.Off));
            }

            Parent.Changed = true;
        }
        #endregion
    }
}
