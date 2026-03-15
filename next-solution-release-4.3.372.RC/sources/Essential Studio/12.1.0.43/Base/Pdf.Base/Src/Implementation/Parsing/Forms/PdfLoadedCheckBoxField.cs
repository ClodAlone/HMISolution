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
    /// Represents check box of an existing PDF document`s form. 
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Load an existing document.
    /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
    /// // Load an existing Check field
    /// PdfLoadedCheckBoxField checkField = doc.Form.Fields["Java"] as PdfLoadedCheckBoxField;
    /// checkField.Checked = true;
    /// doc.Save("Form.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Load an existing document.
    /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
    /// ' Load an existing Check field
    /// Dim checkField As PdfLoadedCheckBoxField = TryCast(doc.Form.Fields("Java"), PdfLoadedCheckBoxField)
    /// checkField.Checked = True
    /// doc.Save("Form.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfLoadedStateField"/> Class 
    /// <seealso cref="PdfLoadedDocument"/> Class     
    public class PdfLoadedCheckBoxField : PdfLoadedStateField
    {
        #region Constants
        /// <summary>
        /// Symbol for check state.
        /// </summary>
        private const string CHECK_SYMBOL = "4";
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PdfLoadedCheckBoxField"/> is checked.
        /// </summary>
        /// <value>True if the check box is checked, false otherwise. </value>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// // Load an existing Check field
        /// PdfLoadedCheckBoxField checkField = doc.Form.Fields["Java"] as PdfLoadedCheckBoxField;
        /// checkField.Checked = true;
        /// doc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document.
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Load an existing Check field
        /// Dim checkField As PdfLoadedCheckBoxField = TryCast(doc.Form.Fields("Java"), PdfLoadedCheckBoxField)
        /// checkField.Checked = True
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedCheckBoxField"/> Class 
        /// <seealso cref="PdfLoadedDocument"/> Class     
        public bool Checked
        {
            get
            {
                bool check = false;

                if (Items.Count > 0)
                {
                    check = Items[DefaultIndex].Checked;
                }

                return check;
            }
            set
            {
                bool readOnly = ((FieldFlags.ReadOnly & Flags) != 0);
                //Form.NeedAppearances = true;

                if (!readOnly)
                {
                    if (Items.Count > 0)
                    {
                        Items[DefaultIndex].Checked = value;
                    }
                    else
                    {
                        SetCheckedStatus(value);
                    }

                    (this as PdfField).Form.SetAppearanceDictionary = true;
                }
            }
        }

        /// <summary>
        /// Gets the collection check box items.
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
        new public PdfLoadedCheckBoxItemCollection Items
        {
            get
            {
                return base.Items as PdfLoadedCheckBoxItemCollection;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLoadedCheckBoxField"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        internal PdfLoadedCheckBoxField(PdfDictionary dictionary, PdfCrossTable crossTable)
            : base(dictionary, crossTable, new PdfLoadedCheckBoxItemCollection())
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="itemDictionary">The item dictionary.</param>
        /// <returns>The proper state item.</returns>
        internal override PdfLoadedStateItem GetItem(int index, PdfDictionary itemDictionary)
        {
            PdfLoadedCheckBoxItem item = new PdfLoadedCheckBoxItem(this, index, itemDictionary);
            return item;
        }

        /// <summary>
        /// Draws this instance if it is flatten.
        /// </summary>
        internal override void Draw()
        {
            base.Draw();
            PdfArray kids = Kids;
            if ((kids != null))
            {
                for (int i = 0; i < kids.Count; ++i)
                {
                    PdfLoadedCheckBoxItem item = Items[i];
                    PdfCheckFieldState style = item.Checked ? PdfCheckFieldState.Checked : PdfCheckFieldState.Unchecked;
                    DrawStateItem(item.Page.Graphics, style, item);
                }
            }
            else
            {
                PdfCheckFieldState style = Checked ? PdfCheckFieldState.Checked : PdfCheckFieldState.Unchecked;
                DrawStateItem(Page.Graphics, style, null);
            }
        }

        /// <summary>
        /// Begins the save.
        /// </summary>
        internal override void BeginSave()
        {
            base.BeginSave();

            PdfArray kids = Kids;
            if ((kids != null))
            {
                for (int i = 0; i < kids.Count; ++i)
                {
                    PdfDictionary widget = CrossTable.GetObject(kids[i]) as PdfDictionary;
                    ApplyAppearance(widget, Items[i]);
                }
            }
            else
            {
                PdfDictionary widget = GetWidgetAnnotation(Dictionary, CrossTable);
                ApplyAppearance(widget, null);
            }
        }

        /// <summary>
        /// Creates a copy of PdfLoadedCheckBoxField.
        /// </summary>
        internal PdfField Clone(PdfDictionary dictionary, PdfPage page)
        {
            PdfCrossTable newTable = page.Section.ParentDocument.CrossTable;
            PdfLoadedCheckBoxField field = new PdfLoadedCheckBoxField(dictionary, newTable);
            field.Page = page;
            field.SetName(GetFieldName());
            field.Widget.Dictionary = Widget.Dictionary.Clone(newTable) as PdfDictionary;

            return field;
        }

        /// <summary>
        /// Creates a copy of PdfLoadedCheckBoxItem.
        /// </summary>
        internal override PdfLoadedFieldItem CreateLoadedItem(PdfDictionary dictionary)
        {
            base.CreateLoadedItem(dictionary);

            PdfLoadedCheckBoxItem item = null;
            if (Items != null)
            {
                item = new PdfLoadedCheckBoxItem(this, Items.Count, dictionary);
                Items.Add(item);
            }

            if(Kids == null)
                Dictionary[DictionaryProperties.Kids] = new PdfArray();

            Kids.Add(new PdfReferenceHolder(dictionary));

            return item;
        }
        #endregion
    }
}
