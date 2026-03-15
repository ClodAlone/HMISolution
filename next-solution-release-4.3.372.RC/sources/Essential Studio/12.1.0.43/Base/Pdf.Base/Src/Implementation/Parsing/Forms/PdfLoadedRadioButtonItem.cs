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
using System.Collections.Generic;

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
    /// // Getting the 'Gender' radio button field      
    /// PdfLoadedRadioButtonListField radiobuttonField = doc.Form.Fields["Gender"] as PdfLoadedRadioButtonListField;
    /// // Radio button field collection
    /// PdfLoadedRadioButtonItemCollection radiobuttonFieldCollection =  radiobuttonField.Items;
    /// // Radio button field item
    /// PdfLoadedRadioButtonItem radiobuttonItem = radiobuttonFieldCollection[0];    
    /// radiobuttonItem.Checked = true;         
    /// doc.Save("LoadedForm.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Load an existing document
    /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
    /// ' Getting the 'Gender' radio button field            
    /// Dim radiobuttonField As PdfLoadedRadioButtonListField = TryCast(doc.Form.Fields("Gender"), PdfLoadedRadioButtonListField)
    /// ' Radio button field collection
    /// Dim radiobuttonFieldCollection As PdfLoadedRadioButtonItemCollection = radiobuttonField.Items
    /// ' Radio button field item
    /// Dim radiobuttonItem As PdfLoadedRadioButtonItem = radiobuttonFieldCollection(0)    
    /// radiobuttonItem.Checked = True
    /// doc.Save("LoadedForm.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfLoadedStateItem"/> Class
    /// <seealso cref="PdfLoadedDocument"/> Class
    /// <seealso cref="PdfLoadedRadioButtonListField"/> Class
    public class PdfLoadedRadioButtonItem : PdfLoadedStateItem
    {
        #region Properties
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value of the radio button item.</value>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// // Getting the 'Gender' radio button field   
        /// PdfLoadedRadioButtonListField radiobuttonField = doc.Form.Fields["Gender"] as PdfLoadedRadioButtonListField;
        /// // Radio button field collection
        /// PdfLoadedRadioButtonItemCollection radiobuttonFieldCollection = radiobuttonField.Items;
        /// // Radio button field item
        /// PdfLoadedRadioButtonItem radiobuttonItem = radiobuttonFieldCollection[0];
        /// // Set the value of the item 
        /// radiobuttonItem.Value = "Male";
        /// doc.Save("Form.pdf");    
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Getting the 'Gender' radio button field   
        /// Dim radiobuttonField As PdfLoadedRadioButtonListField = TryCast(doc.Form.Fields("Gender"), PdfLoadedRadioButtonListField)
        /// ' Radio button field collection
        /// Dim radiobuttonFieldCollection As PdfLoadedRadioButtonItemCollection = radiobuttonField.Items
        /// ' Radio button field item
        /// Dim radiobuttonItem As PdfLoadedRadioButtonItem = radiobuttonFieldCollection(0)
        /// ' Set the value of the item 
        /// radiobuttonItem.Value = "Male"
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedDocument"/> Class
        /// <seealso cref="PdfLoadedRadioButtonListField"/> Class
        public string Value
        {
            get
            {
                string value = GetItemValue();

                return value;
            }
            set
            {
                SetItemValue(value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PdfLoadedRadioButtonItem"/> is selected.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// // Getting the 'Gender' radio button field   
        /// PdfLoadedRadioButtonListField radiobuttonField = doc.Form.Fields["Gender"] as PdfLoadedRadioButtonListField;
        /// // Radio button field collection
        /// PdfLoadedRadioButtonItemCollection radiobuttonFieldCollection = radiobuttonField.Items;
        /// // Radio button field item
        /// PdfLoadedRadioButtonItem radiobuttonItem = radiobuttonFieldCollection[0];
        /// // Set the first item as selected item
        /// radiobuttonItem.Selected = true;
        /// doc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Getting the 'Gender' radio button field   
        /// Dim radiobuttonField As PdfLoadedRadioButtonListField = TryCast(doc.Form.Fields("Gender"), PdfLoadedRadioButtonListField)
        /// ' Radio button field collection
        /// Dim radiobuttonFieldCollection As PdfLoadedRadioButtonItemCollection = radiobuttonField.Items
        /// ' Radio button field item
        /// Dim radiobuttonItem As PdfLoadedRadioButtonItem = radiobuttonFieldCollection(0)
        /// ' Set the first item as selected item
        /// radiobuttonItem.Selected = True
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedDocument"/> Class
        /// <seealso cref="PdfLoadedRadioButtonListField"/> Class
        public bool Selected
        {
            get
            {
                int index = Parent.Items.IndexOf(this);

                return (index == Parent.SelectedIndex);
            }
            set
            {
                if (value)
                {
                    int index = Parent.Items.IndexOf(this);

                    Parent.SelectedIndex = index;
                }
            }
        }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        internal new PdfLoadedRadioButtonListField Parent
        {
            get
            {
                return (base.Parent as PdfLoadedRadioButtonListField);
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLoadedRadioButtonItem"/> class.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="index">The index.</param>
        /// <param name="dictionary">The dictionary.</param>
        internal PdfLoadedRadioButtonItem(PdfLoadedStyledField field, int index, PdfDictionary dictionary)
            : base(field, index, dictionary)
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the item value.
        /// </summary>
        /// <returns>The value of the item.</returns>
        private string GetItemValue()
        {
            string value = String.Empty;
            PdfName name = null;

            if (Dictionary.ContainsKey(DictionaryProperties.AS))
            {
                name = CrossTable.GetObject(Dictionary[DictionaryProperties.AS]) as PdfName;

                if (name != null && name.Value != DictionaryProperties.Off)
                {
                    value = name.Value;
                }
            }
            if (value == String.Empty)
            {
                {
                    if (Dictionary.ContainsKey(DictionaryProperties.AP))
                    {
                        PdfDictionary dic = CrossTable.GetObject(Dictionary[DictionaryProperties.AP]) as PdfDictionary;

                        if (dic.ContainsKey(DictionaryProperties.N))
                        {
                            PdfReference reference = CrossTable.GetReference(dic[DictionaryProperties.N]) as PdfReference;

                            PdfDictionary normalAppearance = CrossTable.GetObject(reference) as PdfDictionary;

                            List<object> list = new List<object>();
                            foreach (PdfName pdfName in normalAppearance.Keys)
                            {
                                list.Add(pdfName);
                            }

                            for (int i = 0, size = list.Count; i < size; ++i)
                            {
                                name = list[i] as PdfName;

                                if (name.Value != DictionaryProperties.Off)
                                {
                                    value = name.Value;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return value;
        }

        /// <summary>
        /// Stes item value.
        /// </summary>
        /// <param name="value">The item value.</param>
        private void SetItemValue(string value)
        {
            string str = value;

            if (Dictionary.ContainsKey(DictionaryProperties.AP))
            {
                PdfDictionary dic = CrossTable.GetObject(Dictionary[DictionaryProperties.AP]) as PdfDictionary;

                if (dic.ContainsKey(DictionaryProperties.N))
                {
                    PdfReference normal = CrossTable.GetReference(dic[DictionaryProperties.N]) as PdfReference;

                    dic = CrossTable.GetObject(normal) as PdfDictionary;

                    string dicValue = GetItemValue();

                    if (dic.ContainsKey(dicValue))
                    {
                        PdfReference valRef = CrossTable.GetReference(dic[dicValue]) as PdfReference;

                        dic.Remove(Value);
                        dic.SetProperty(str, new PdfReferenceHolder(valRef, CrossTable));
                    }
                }
            }

            if (str == Parent.SelectedValue)
            {
                Dictionary.SetName(DictionaryProperties.AS, str);
            }
            else
            {
                Dictionary.SetName(DictionaryProperties.AS, DictionaryProperties.Off);
            }
        }
        #endregion
    }
}
