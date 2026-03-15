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
    /// Represents radio button field of an existing PDF document`s form.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Load an existing document
    /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
    /// // Read the 'Gender' radio button field         
    /// PdfLoadedRadioButtonListField radiobuttonField = doc.Form.Fields["Gender"] as PdfLoadedRadioButtonListField;
    /// // Flatten the radio button field
    /// radiobuttonField.Flatten = true;
    /// doc.Save("LoadedForm.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Load an existing document
    /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
    /// ' Read the 'Gender' radio button field         
    /// Dim radiobuttonField As PdfLoadedRadioButtonListField = TryCast(doc.Form.Fields("Gender"), PdfLoadedRadioButtonListField)
    /// ' Flatten the radio button field
    /// radiobuttonField.Flatten = True
    /// doc.Save("LoadedForm.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfLoadedStateField"/> Class
    /// <seealso cref="PdfLoadedDocument"/> Class
    public class PdfLoadedRadioButtonListField : PdfLoadedStateField
    {
        #region Constants
        /// <summary>
        /// Symbol for check state.
        /// </summary>
        private const string CHECK_SYMBOL = "l";
        #endregion

        #region Properties
        /// <summary>
        /// Gets the collection of radio button items.
        /// </summary>
        /// <value>A <see cref="PdfLoadedRadioButtonItemCollection"/> that represents the items within the list.</value>
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
        /// <seealso cref="PdfLoadedDocument"/> Class
        /// <seealso cref="PdfLoadedRadioButtonListField"/> Class
        /// <seealso cref="PdfLoadedRadioButtonItem"/> Class
        new public PdfLoadedRadioButtonItemCollection Items
        {
            get
            {
                return (base.Items as PdfLoadedRadioButtonItemCollection);
            }
        }

        /// <summary>
        /// Gets or sets the index of the selected item in the list.
        /// </summary>
        /// <value>The lowest ordinal index of the selected items in the list. The default is -1, which indicates that nothing is selected. </value>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// // Read the 'Gender' radio button field   
        /// PdfLoadedRadioButtonListField radiobuttonField = doc.Form.Fields["Gender"] as PdfLoadedRadioButtonListField;
        /// // Set the selected index as 1
        /// radiobuttonField.SelectedIndex = 1;
        /// // Save the document to a disk
        /// doc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Read the 'Gender' radio button field   
        /// Dim radiobuttonField As PdfLoadedRadioButtonListField = TryCast(doc.Form.Fields("Gender"), PdfLoadedRadioButtonListField)
        /// ' Set the selected index as 1
        /// radiobuttonField.SelectedIndex = 1
        /// ' Save the document to a disk
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedDocument"/> Class
        /// <seealso cref="PdfLoadedRadioButtonListField"/> Class
        public int SelectedIndex
        {
            get
            {
                int index = GetSelectedIndex();

                return index;
            }
            set
            {
                SetSelectedIndex(value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the first selected item in the list. 
        /// </summary>
        /// <value>A string value specifying the value of the first selected item, null (Nothing in VB.NET) if there is no selected item.</value>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// // Read the 'Gender' radio button field   
        /// PdfLoadedRadioButtonListField radiobuttonField = doc.Form.Fields["Gender"] as PdfLoadedRadioButtonListField;
        /// // Set the selected index as 1
        /// radiobuttonField.SelectedValue = "Female";
        /// // Save the document to a disk
        /// doc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Read the 'Gender' radio button field   
        /// Dim radiobuttonField As PdfLoadedRadioButtonListField = TryCast(doc.Form.Fields("Gender"), PdfLoadedRadioButtonListField)
        /// ' Set the selected index as 1
        /// radiobuttonField.SelectedValue = "Female"
        /// ' Save the document to a disk
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedDocument"/> Class
        /// <seealso cref="PdfLoadedRadioButtonListField"/> Class
        public string SelectedValue
        {
            get
            {
                int index = SelectedIndex;

                string value = (index > -1) ? Items[index].Value : null;

                return value;
            }
            set
            {
                SetSelectedValue(value);
            }
        }

        /// <summary>
        /// Gets the selected item.
        /// </summary>
        /// <value>Return the item as PdfLoadedRadioButtonItem class</value>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// // Read the 'Gender' radio button field   
        /// PdfLoadedRadioButtonListField radiobuttonField = doc.Form.Fields["Gender"] as PdfLoadedRadioButtonListField;
        /// // Read the selected item of the radio button
        /// PdfLoadedRadioButtonItem radiobuttonItem = radiobuttonField.SelectedItem;
        /// // Uncheck the selected item
        /// radiobuttonItem.Checked = false;
        /// // Save the document to a disk
        /// doc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Read the 'Gender' radio button field   
        /// Dim radiobuttonField As PdfLoadedRadioButtonListField = TryCast(doc.Form.Fields("Gender"), PdfLoadedRadioButtonListField)
        /// ' Read the selected item of the radio button
        /// Dim radiobuttonItem As PdfLoadedRadioButtonItem = radiobuttonField.SelectedItem
        /// ' Uncheck the selected item
        /// radiobuttonItem.Checked = False
        /// ' Save the document to a disk
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedDocument"/> Class
        /// <seealso cref="PdfLoadedRadioButtonListField"/> Class
        public PdfLoadedRadioButtonItem SelectedItem
        {
            get
            {
                int index = SelectedIndex;

                PdfLoadedRadioButtonItem item = null;

                if (index > -1)
                {
                    item = Items[index];
                }

                return item;
            }
        }

        /// <summary>
        /// Gets or sets the value of specified item.
        /// </summary>
        /// <value>A string value representing the value of the item.</value>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// // Read the 'Gender' radio button field   
        /// PdfLoadedRadioButtonListField radiobuttonField = doc.Form.Fields["Gender"] as PdfLoadedRadioButtonListField;
        /// // Set the radio box value as Male
        /// radiobuttonField.Value = "Male";
        /// // Save the document to a disk
        /// doc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Read the 'Gender' radio button field   
        /// Dim radiobuttonField As PdfLoadedRadioButtonListField = TryCast(doc.Form.Fields("Gender"), PdfLoadedRadioButtonListField)
        /// ' Set the radio box value as Male
        /// radiobuttonField.Value = "Male"
        /// ' Save the document to a disk
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedDocument"/> Class
        /// <seealso cref="PdfLoadedRadioButtonListField"/> Class
        public string Value
        {
            get
            {
                return Items[DefaultIndex].Value;
            }
            set
            {
                Items[DefaultIndex].Value = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLoadedRadioButtonListField"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        internal PdfLoadedRadioButtonListField(PdfDictionary dictionary, PdfCrossTable crossTable)
            : base(dictionary, crossTable, new PdfLoadedRadioButtonItemCollection())
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
            PdfLoadedRadioButtonItem item = new PdfLoadedRadioButtonItem(this, index, itemDictionary);

            return item;
        }

        ///// <summary>
        ///// Gets the items collection.
        ///// </summary>
        ///// <returns>The colection of loaded radio button items.</returns>
        //private PdfLoadedRadioButtonCollection GetItemCollection()
        //{
        //  PdfLoadedRadioButtonCollection items = new PdfLoadedRadioButtonCollection( this );

        //  if( Dictionary.ContainsKey( DictionaryProperties.Kids ) )
        //  {
        //    PdfArray array = GetValue( Dictionary, CrossTable, DictionaryProperties.Kids, false ) as PdfArray;

        //    if( array.Count != 0 )
        //    {
        //      for( int i = 0, size = array.Count; i < size; ++i )
        //      {
        //        PdfDictionary widget = CrossTable.GetObject( array[ i ] ) as PdfDictionary;

        //        PdfLoadedRadioButtonItem item = new PdfLoadedRadioButtonItem( widget, CrossTable, this );
        //        items.AddItem( item );
        //      }
        //    }
        //  }

        //  return items;
        //}

        /// <summary>
        /// Gets the index of the selected.
        /// </summary>
        /// <returns>The index of first selected item.</returns>
        private int GetSelectedIndex()
        {
            int index = -1;

            PdfLoadedRadioButtonItemCollection items = Items;

            for (int i = 0, size = items.Count; i < size; ++i)
            {
                PdfLoadedRadioButtonItem item = items[i];

                PdfDictionary dic = item.Dictionary;

                PdfName checkName = PdfLoadedField.SearchInParents(dic, CrossTable, DictionaryProperties.V) as PdfName;

                if (dic.ContainsKey(DictionaryProperties.AS) && checkName != null)
                {
                    PdfName name = CrossTable.GetObject(dic[DictionaryProperties.AS]) as PdfName;

                    if (name.Value == checkName.Value)
                    {
                        index = i;

                        break;
                    }
                }
            }

            return index;
        }

        /// <summary>
        /// Sets selected index of the radio button.
        /// </summary>
        /// <param name="value">Selected index.</param>
        private void SetSelectedIndex(int value)
        {
            int index = SelectedIndex;
            if (index != value)
            {
                PdfLoadedRadioButtonItemCollection items = Items;

                PdfLoadedRadioButtonItem item = items[value];

                UncheckOthers(item, GetItemValue(item.Dictionary, CrossTable), true);

                item.Checked = true;
                Dictionary.SetName(DictionaryProperties.V, item.Value);
                Dictionary.SetName(DictionaryProperties.DV, item.Value);
            }
        }

        /// <summary>
        /// Sets selected value.
        /// </summary>
        /// <param name="value">Selected value.</param>
        private void SetSelectedValue(string value)
        {
            if (value == null)
                throw new ArgumentNullException("SelectedValue");

            UncheckOthers(null, value, true);

            Dictionary.SetName(DictionaryProperties.V, value);
            Dictionary.SetName(DictionaryProperties.DV, value);
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
                    PdfLoadedRadioButtonItem item = Items[i];

                    PdfCheckFieldState state = (item.Selected) ?
                        PdfCheckFieldState.Checked :
                        PdfCheckFieldState.Unchecked;

                    DrawStateItem(item.Page.Graphics, state, item);
                }
            }
            else
            {
                PdfCheckFieldState state = (SelectedIndex == DefaultIndex) ?
                    PdfCheckFieldState.Checked :
                    PdfCheckFieldState.Unchecked;

                DrawStateItem(Page.Graphics, state, null);
            }
        }

        /// <summary>
        /// Creates a copy of PdfLoadedRadioButtonListField.
        /// </summary>
        internal PdfField Clone(PdfDictionary dictionary, PdfPage page)
        {
            PdfCrossTable newTable = page.Section.ParentDocument.CrossTable;
            PdfLoadedRadioButtonListField field = new PdfLoadedRadioButtonListField(dictionary, newTable);
            field.Page = page;
            field.SetName(GetFieldName());
            field.Widget.Dictionary = Widget.Dictionary.Clone(newTable) as PdfDictionary;

            return field;
        }

        /// <summary>
        /// Creates a copy of PdfLoadedRadioButtonItem.
        /// </summary>
        internal override PdfLoadedFieldItem CreateLoadedItem(PdfDictionary dictionary)
        {
            base.CreateLoadedItem(dictionary);

            PdfLoadedRadioButtonItem item = null;
            if (Items != null)
            {
                item = new PdfLoadedRadioButtonItem(this, Items.Count, dictionary);
                Items.Add(item);
            }

            if (Kids == null)
                Dictionary[DictionaryProperties.Kids] = new PdfArray();

            Kids.Add(new PdfReferenceHolder(dictionary));

            return item;
        }
        #endregion
    }
}
