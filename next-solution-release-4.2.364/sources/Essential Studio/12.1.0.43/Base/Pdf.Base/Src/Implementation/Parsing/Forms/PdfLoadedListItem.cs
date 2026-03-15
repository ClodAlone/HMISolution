#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
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
    /// Represents loaded list item.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Load an existing document
    /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
    /// // Load the list box field          
    /// PdfLoadedListBoxField listField = doc.Form.Fields["Course"] as PdfLoadedListBoxField;
    /// // Get the selected list item
    /// PdfLoadedListItem listItem = listField.SelectedItem;
    /// listItem.Text = "NewText";
    /// doc.Save("Form.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Load an existing document
    /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
    /// ' Load the list box field          
    /// Dim listField As PdfLoadedListBoxField = TryCast(doc.Form.Fields("Course"), PdfLoadedListBoxField)
    /// ' Get the selected list item
    /// Dim listItem As PdfLoadedListItem = listField.SelectedItem
    /// listItem.Text = "NewText"
    /// doc.Save("Form.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfLoadedDocument"/> Class
    /// <seealso cref="PdfLoadedListBoxField"/> Class
    public class PdfLoadedListItem
    {
        #region Fields
        /// <summary>
        /// Text of the item.
        /// </summary>
        private string m_text;

        /// <summary>
        /// Value of the item. 
        /// </summary>
        private string m_value;

        /// <summary>
        /// Field wich item belons to. 
        /// </summary>
        private PdfLoadedChoiceField m_field;

        /// <summary>
        /// CrossTable of document.
        /// </summary>
        private PdfCrossTable m_crossTable;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>A string value representing the display text of the item. </value>
        /// <example>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// // Load the list box field          
        /// PdfLoadedListBoxField listField = doc.Form.Fields["Course"] as PdfLoadedListBoxField;
        /// // Get the selected list item
        /// PdfLoadedListItem listItem = listField.SelectedItem;
        /// listItem.Text = "NewText";
        /// doc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Load the list box field          
        /// Dim listField As PdfLoadedListBoxField = TryCast(doc.Form.Fields("Course"), PdfLoadedListBoxField)
        /// ' Get the selected list item
        /// Dim listItem As PdfLoadedListItem = listField.SelectedItem
        /// listItem.Text = "NewText"
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedDocument"/> Class
        /// <seealso cref="PdfLoadedListBoxField"/> Class
        public string Text
        {
            get
            {
                return m_text;
            }
            set
            {
                SetText(value);
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>A string value representing the value of the item. </value>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// // Load the list box field          
        /// PdfLoadedListBoxField listField = doc.Form.Fields["Course"] as PdfLoadedListBoxField;
        /// // Get the selected list item
        /// PdfLoadedListItem listItem = listField.SelectedItem;
        /// listItem.Value = "C#.NET";
        /// doc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Load the list box field          
        /// Dim listField As PdfLoadedListBoxField = TryCast(doc.Form.Fields("Course"), PdfLoadedListBoxField)
        /// ' Get the selected list item
        /// Dim listItem As PdfLoadedListItem = listField.SelectedItem
        /// listItem.Value = "C#.NET"
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedDocument"/> Class
        /// <seealso cref="PdfLoadedListBoxField"/> Class
        public string Value
        {
            get
            {
                string retval = m_value;

                if (retval == null)
                {
                    retval = Text;
                }

                return retval;
            }
            set
            {
                SetValue(value);
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLoadedListItem"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="value">The value.</param>
        /// <param name="field">The field.</param>
        /// <param name="cTable">The cross table.</param>
        internal PdfLoadedListItem(string text, string value, PdfLoadedChoiceField field, PdfCrossTable cTable)
            : this(text, value)
        {
            m_field = field;
            m_crossTable = cTable;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLoadedListItem"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="value">The value.</param>
        /// <example>
        /// <code lang="C#">
        ///   // Load the list box field    
        /// PdfLoadedListBoxField listField = doc.Form.Fields["Course"] as PdfLoadedListBoxField;
        /// // Get the selected list item
        /// PdfLoadedListItem listItem = new PdfLoadedListItem("C#.Net",".NET Course");
        /// // Add the list item in list field
        /// listField.Values.Add(listItem);
        /// </code>
        /// <code lang="VB">
        ///   ' Load the list box field
        /// Dim listField As PdfLoadedListBoxField = TryCast(doc.Form.Fields("Course"), PdfLoadedListBoxField)
        /// ' Get the selected list item
        /// Dim listItem As PdfLoadedListItem = New PdfLoadedListItem("C#.Net",".NET Course")
        /// ' Add the list item in list field
        /// listField.Values.Add(listItem)
        /// </code>
        /// </example>
        public PdfLoadedListItem(string text, string value)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            m_text = text;
            m_value = value;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Sets the text of the item.
        /// </summary>
        /// <param name="value"></param>
        private void SetText(string value)
        {
            if (value == null)
                throw new ArgumentNullException("text");

            if (m_text != value)
            {
                PdfDictionary fieldDic = m_field.Dictionary;

                if (fieldDic.ContainsKey(DictionaryProperties.Opt))
                {
                    PdfArray array = m_crossTable.GetObject(fieldDic[DictionaryProperties.Opt]) as PdfArray;

                    PdfArray item = new PdfArray();

                    item.Add(new PdfString(m_value));
                    item.Add(new PdfString(value));

                    for (int i = 0, size = array.Count; i < size; ++i)
                    {
                        IPdfPrimitive primitive = m_crossTable.GetObject(array[i]);
                        PdfArray arr = primitive as PdfArray;

                        PdfString text = m_crossTable.GetObject(arr[1]) as PdfString;
                        if (text.Value == m_text)
                        {
                            m_text = value;
                            array.RemoveAt(i);
                            array.Insert(i, item);
                        }
                    }

                    fieldDic.SetProperty(DictionaryProperties.Opt, array);

                    m_field.Changed = true;
                }
            }
        }

        /// <summary>
        /// Sets item value.
        /// </summary>
        /// <param name="value">The item value.</param>
        private void SetValue(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (m_value != value)
            {
                PdfDictionary fieldDic = m_field.Dictionary;

                if (fieldDic.ContainsKey(DictionaryProperties.Opt))
                {
                    PdfArray array = m_crossTable.GetObject(fieldDic[DictionaryProperties.Opt]) as PdfArray;

                    PdfArray item = new PdfArray();

                    item.Add(new PdfString(value));
                    item.Add(new PdfString(m_text));

                    for (int i = 0, size = array.Count; i < size; ++i)
                    {
                        IPdfPrimitive primitive = m_crossTable.GetObject(array[i]);
                        PdfArray arr = primitive as PdfArray;

                        PdfString text = m_crossTable.GetObject(arr[1]) as PdfString;
                        if (text.Value == m_value)
                        {
                            m_value = value;
                            array.RemoveAt(i);
                            array.Insert(i, item);
                        }
                    }

                    fieldDic.SetProperty(DictionaryProperties.Opt, array);

                    m_field.Changed = true;
                }
            }
        }
        #endregion
    }
}
