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
using Syncfusion.Pdf.Graphics.Fonts;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;
using Syncfusion.Pdf.Security;
using System.Collections.Generic;

/// <summary>
/// The Syncfusion.Pdf.Parsing namespace contains classes, which are used to load or modify an existing PDF document.
/// </summary>
namespace Syncfusion.Pdf.Parsing
{
    /// <summary>
    /// Represents a choice field of an existing PDF document`s form.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    ///  //Load an existing document
    /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
    /// // Load an existing choice field
    /// PdfLoadedChoiceField choiceField = doc.Form.Fields["Java"] as PdfLoadedChoiceField;
    /// choiceField.SelectedIndex = 0;
    /// doc.Save("Form.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Load an existing document
    /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
    /// ' Load an existing Check field
    /// Dim choiceField As PdfLoadedChoiceField = TryCast(doc.Form.Fields("Java"), PdfLoadedChoiceField)
    /// choiceField.SelectedIndex = 0
    /// doc.Save("Form.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfLoadedStyledField"/> Class 
    /// <seealso cref="PdfLoadedDocument"/> Class 
    public class PdfLoadedChoiceField : PdfLoadedStyledField
    {
        #region Properties
        /// <summary>
        /// Gets the collection of choice items.
        /// </summary>
        public PdfLoadedListItemCollection Values
        {
            get
            {
                PdfLoadedListItemCollection items = GetListItemCollection();

                return items;
            }
        }

        /// <summary>
        /// Gets or sets the first selected item in the list.
        /// </summary>
        /// <example>
        /// <value>An integer value specifying the choice item in the list.</value>
        /// <code lang="C#">
        ///  //Load an existing document
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// // Load an existing choice field
        /// PdfLoadedChoiceField choiceField = doc.Form.Fields["Java"] as PdfLoadedChoiceField;
        /// choiceField.SelectedIndex = 0;
        /// doc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Load an existing Check field
        /// Dim choiceField As PdfLoadedChoiceField = TryCast(doc.Form.Fields("Java"), PdfLoadedChoiceField)
        /// choiceField.SelectedIndex = 0
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedStyledField"/> Class 
        /// <seealso cref="PdfLoadedDocument"/> Class 
        public int[] SelectedIndex
        {
            get
            {
                return GetSelectedIndex();
            }
            set
            {
                SetSelectedIndex(value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the first selected item in the list.
        /// </summary>
        /// <example>
        /// <value>A string value specifying the value of the selected item.</value>
        /// <code lang="C#">
        /// //Load an existing document
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// // Load an existing choice field
        /// PdfLoadedChoiceField choiceField = doc.Form.Fields["Java"] as PdfLoadedChoiceField;
        /// choiceField.SelectedValue = "Employee";
        /// doc.Save("Form.pdf");          
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Load an existing choice field
        /// Dim choiceField As PdfLoadedChoiceField = TryCast(doc.Form.Fields("Java"), PdfLoadedChoiceField)
        /// choiceField.SelectedValue = "Employee"
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedDocument"/> Class   
        /// <seealso cref="PdfLoadedChoiceField"/> Class   
        public string[] SelectedValue
        {
            get
            {
                return GetSelectedValue();
            }
            set
            {
                SetSelectedValue(value);
            }
        }

        /// <summary>
        /// Gets the first selected item in the list.
        /// </summary>
        /// <example>
        /// <value>A <see cref="PdfLoadedListItem"/>object specifying the selected item.</value>
        /// <code lang="C#">
        /// //Load an existing document
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// // Load an existing Check field
        /// PdfLoadedChoiceField choiceField = doc.Form.Fields["Java"] as PdfLoadedChoiceField;
        /// // Change the selected item
        /// PdfLoadedListItem item = choiceField.SelectedItem;
        /// item.Text = "New Text";
        /// doc.Save("Form.pdf");          
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Load an existing Check field
        /// Dim choiceField As PdfLoadedChoiceField = TryCast(doc.Form.Fields("Java"), PdfLoadedChoiceField)
        /// ' Change the selected item
        /// Dim item As PdfLoadedListItem = choiceField.SelectedItem
        /// item.Text = "New Text"
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedDocument"/> Class   
        /// <seealso cref="PdfLoadedChoiceField"/> Class   
        public PdfLoadedListItemCollection SelectedItem
        {
            get
            {
                PdfLoadedListItemCollection items = new PdfLoadedListItemCollection(this);

                //int index = SelectedIndex;
                foreach (int index in SelectedIndex)
                {
                    if (index > -1)
                    {
                        items.Add(Values[index]);
                    }
                }
                return items;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLoadedChoiceField"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        internal PdfLoadedChoiceField(PdfDictionary dictionary, PdfCrossTable crossTable)
            : base(dictionary, crossTable)
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets selected index.
        /// </summary>
        /// <returns>Selected index.</returns>
        protected int[] GetSelectedIndex()
        {
            List<int> selectedIndex =new List<int>();

            if (Dictionary.ContainsKey(DictionaryProperties.I))
            {
                PdfArray array = CrossTable.GetObject(Dictionary[DictionaryProperties.I]) as PdfArray;
                if (array != null)
                {
                    if (array.Count > 0)
                    {
                        for (int i = 0; i < array.Count; i++)
                        {
                            PdfNumber number = CrossTable.GetObject(array[i]) as PdfNumber;
                            selectedIndex.Add(number.IntValue);
                        }
                    }
                }
                else
                {
                    PdfNumber number = CrossTable.GetObject(Dictionary[DictionaryProperties.I]) as PdfNumber;
                    if(number!=null)
                    selectedIndex.Add(number.IntValue);
                }
            }
            if (selectedIndex.Count == 0)
            {
                selectedIndex.Add(-1);
            }
            return selectedIndex.ToArray();
        }

        /// <summary>
        /// Sets selected index.
        /// </summary>
        /// <param name="value">Selected index.</param>
        protected void SetSelectedIndex(int[] value)
        {
            if ((value.Length==0) || (value.Length > Values.Count))
            {
                throw new ArgumentOutOfRangeException("SelectedIndex");
            }
            foreach (int index in value)
            {
                if ((index<0) || (index>=Values.Count))
                {
                    throw new ArgumentOutOfRangeException("SelectedIndex");
                }
            }
            if (this.ReadOnly == false)
            {
                Dictionary.SetProperty(DictionaryProperties.I, new PdfArray(value));
                List<string> selectedValues = new List<string>();
                foreach (int index in value)
                {
                    selectedValues.Add(Values[index].Value);
                }
                SetSelectedValue(selectedValues.ToArray());
                Changed = true;
            }


        }

        /// <summary>
        /// Gets selected value.
        /// </summary>
        /// <returns>Selected value.</returns>
        protected string[] GetSelectedValue()
        {
            List<string> value = new List<string>();

            //int index = SelectedIndex;

            if (Dictionary.ContainsKey(DictionaryProperties.V))
            {
                IPdfPrimitive primitive = CrossTable.GetObject(Dictionary[DictionaryProperties.V]);

                if (primitive is PdfString)
                {
                    value.Add((primitive as PdfString).Value);
                }
                else
                {
                    PdfArray array = primitive as PdfArray;
                    for (int i = 0; i < array.Count; i++)
                    {
                        PdfString stringValue = array[i] as PdfString;
                        value.Add(stringValue.Value);
                    }
                }
            }
            else
            {
                foreach (int index in SelectedIndex)
                {
                    if (index > -1)
                    {
                        value.Add(Values[index].Value);
                    }
                }
            }

            return value.ToArray();
        }

        /// <summary>
        /// Sets selected value.
        /// </summary>
        /// <param name="value">Selected value.</param>
        protected void SetSelectedValue(string[] values)
        {
            List<int> selectedIndexes = new List<int>();
            List<string> collectionValues=new List<string>();
            PdfLoadedListItemCollection collection = Values;
            foreach (string value in values)
            {
                int count = 0;
                foreach (PdfLoadedListItem item in collection)
                {
                    collectionValues.Add(item.Value);
                    if (item.Value == value)
                    {
                        selectedIndexes.Add(count);
                        break;
                    }
                    count++;
                }
                if (!collectionValues.Contains(value))
                {
                    throw new ArgumentOutOfRangeException("index");
                }
            }

            //if (index == -1)
            //    throw new ArgumentOutOfRangeException("index");
            int[] selected = GetSelectedIndex();
            bool equal=false;
            if(selected.Length==selectedIndexes.Count)
            {
                for(int i=0;i<selected.Length;i++)
                {
                    if(selected[i]==selectedIndexes.ToArray()[i])
                    {
                        equal=true;
                    }
                    else
                    {
                        equal=false;
                        break;
                    }
                }
            if (!equal)
            {
                SetSelectedIndex(selectedIndexes.ToArray());
            }
            }
            //string selectedValue = value;

            if (Dictionary.ContainsKey(DictionaryProperties.V))
            {
                IPdfPrimitive primitive = CrossTable.GetObject(Dictionary[DictionaryProperties.V]);

                if ((primitive == null) || (primitive is PdfString))
                {
                    Dictionary.SetString(DictionaryProperties.V,values[0]);
                }
                else
                {
                    PdfArray array = primitive as PdfArray;

                    array.Clear();
                    foreach (string selectedValue in values)
                    {
                        array.Add(new PdfString(selectedValue));
                    }
                    Dictionary.SetProperty(DictionaryProperties.V, array);
                }
            }
            else
            {
                PdfArray array = new PdfArray();
                foreach (string selectedValue in values)
                {
                    array.Add(new PdfString(selectedValue));
                }
                Dictionary.SetProperty(DictionaryProperties.V, array);
            }

            Changed = true;
        }

        /// <summary>
        /// Gets the list item.
        /// </summary>
        /// <returns>The list item collection</returns>
        internal PdfLoadedListItemCollection GetListItemCollection()
        {
            PdfLoadedListItemCollection items = new PdfLoadedListItemCollection(this);

            PdfArray array =
                GetValue(Dictionary, CrossTable, DictionaryProperties.Opt, true) as PdfArray;

            for (int i = 0, size = array.Count; i < size; i++)
            {
                IPdfPrimitive primitive = CrossTable.GetObject(array[i]);

                PdfLoadedListItem item = null;

                if (primitive is PdfString)
                {
                    PdfString str = primitive as PdfString;
                    item = new PdfLoadedListItem(str.Value, null, this, CrossTable);
                }
                else
                {
                    PdfArray arr = primitive as PdfArray;

                    PdfString value = CrossTable.GetObject(arr[0]) as PdfString;
                    PdfString text = CrossTable.GetObject(arr[1]) as PdfString;

                    item = new PdfLoadedListItem(text.Value, value.Value, this, CrossTable);
                }

                items.AddItem(item);
            }

            return items;
        }
        #endregion
    }
}
