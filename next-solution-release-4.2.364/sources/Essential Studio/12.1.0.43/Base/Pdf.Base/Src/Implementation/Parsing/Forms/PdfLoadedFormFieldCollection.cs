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
    /// Represents field collection of loaded form.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    ///  //Load an existing document
    ///  PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
    ///  // PDF loaded field collection
    ///  PdfLoadedFormFieldCollection fieldCollection = doc.Form.Fields;
    ///  // Remove the first field 
    ///  fieldCollection.RemoveAt(0);
    ///  doc.Save("Form.pdf");
    /// </code>
    /// <code lang="VB">
    ///  'Load an existing document
    ///  Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
    ///  ' PDF loaded field collection
    ///  Dim fieldCollection As PdfLoadedFormFieldCollection = doc.Form.Fields
    ///  ' Remove the first field 
    ///  fieldCollection.RemoveAt(0)
    ///  doc.Save("Form.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfFieldCollection"/> Class
    public class PdfLoadedFormFieldCollection
        : PdfFieldCollection
    {
        #region Field
        /// <summary>
        /// Loaded form, wich collection belongs to.
        /// </summary>
        private PdfLoadedForm m_form;

        private List<string> m_fieldNames;// = new List<string>();
        private List<string> m_indexedFieldNames;
        private List<string> m_actualFieldNames;
        private List<string> m_indexedActualFieldNames;
        private List<string> m_addedFieldNames = new List<string>();
        #endregion

        #region Properties
        /// <summary>
        /// Gets the <see cref="Syncfusion.Pdf.Interactive.PdfField"/> at the specified index.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// //load the form fields
        /// foreach (PdfField field in doc.Form.Fields)
        /// {
        ///   // Flatten the form
        ///   field.Flatten = true;
        /// }
        /// doc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// 'load the form fields
        /// For Each field As PdfField In doc.Form.Fields
        ///   ' Flatten the form
        ///   field.Flatten = True
        /// Next field
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedDocument"/> Class
        public override PdfField this[int index]
        {
            get
            {
                int k = List.Count;

                if ((k < 0) || (index >= k))
                    throw new IndexOutOfRangeException("index");

                PdfField field = List[index] as PdfField;

                PdfLoadedField ldField = field as PdfLoadedField;

                return field;
            }
        }

        /// <summary>
        /// Returns field with specified name.
        /// </summary>
        /// <param name="name">The specified field name.</param>
        /// <example>
        /// <code lang="C#">
        /// // Loads an existing PDF document
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// // Load the FirstTextBox field
        /// PdfField field = doc.Form.Fields["FirstTextBox"];
        /// field.Flatten = true;
        /// doc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        ///  // Loads an existing PDF document
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Load the FirstTextBox field
        /// Dim field As PdfField = doc.Form.Fields("FirstTextBox")
        /// field.Flatten = True
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        public PdfField this[string name]
        {
            get
            {
                if (name == null)
                    throw new ArgumentNullException("name");

                if (name == string.Empty)
                    throw new ArgumentException("Field name can't be empty");

                int index = GetFieldIndex(name);

                if (index == -1)
                    throw new ArgumentException("Incorrect field name");

                return this[index];
            }
        }

        /// <summary>
        /// Gets or sets the form.
        /// </summary>      
        public PdfLoadedForm Form
        {
            get
            {
                return m_form;
            }
            set
            {
                m_form = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLoadedFormFieldCollection"/> class.
        /// </summary>
        /// <param name="form">The form.</param>
        public PdfLoadedFormFieldCollection(PdfLoadedForm form)
            : base()
        {
            if (form == null)
                throw new ArgumentException("form");

            m_form = form;

            for (int i = 0, size = m_form.TerminalFields.Count; i < size; ++i)
            {
                PdfField field = GetField(i);
                if (field != null)
                    DoAdd(field);
            }
           // m_form.TerminalFields.Clear();
        }
        public PdfLoadedFormFieldCollection()
        {
        }
       
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the field.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The created field.</returns>
        private PdfField GetField(int index)
        {
            PdfDictionary dictionary = m_form.TerminalFields[index] as PdfDictionary;
            PdfCrossTable crossTable = m_form.CrossTable;
            PdfField field = null;

            PdfName name = PdfLoadedField.GetValue(dictionary, crossTable, DictionaryProperties.FT, true) as PdfName;

            PdfLoadedFieldTypes type = PdfLoadedFieldTypes.Null;

            if (name != null)
                type = GetFieldType(name, dictionary, crossTable);

            switch (type)
            {
                case PdfLoadedFieldTypes.PushButton:
                    field = CreatePushButton(dictionary, crossTable);
                    break;

                case PdfLoadedFieldTypes.CheckBox:
                    field = CreateCheckBox(dictionary, crossTable);
                    break;

                case PdfLoadedFieldTypes.RadioButton:
                    field = CreateRadioButton(dictionary, crossTable);
                    break;

                case PdfLoadedFieldTypes.TextField:
                    field = CreateTextField(dictionary, crossTable);
                    break;

                case PdfLoadedFieldTypes.ComboBox:
                    field = CreateComboBox(dictionary, crossTable);
                    break;

                case PdfLoadedFieldTypes.ListBox:
                    field = CreateListBox(dictionary, crossTable);
                    break;
#if !SILVERLIGHT && !NETFX_CORE && !WP
                case PdfLoadedFieldTypes.SignatureField:
                    field = CreateSignatureField(dictionary, crossTable);
                    break;
#endif
                case PdfLoadedFieldTypes.Null:
                    field = new PdfLoadedStyledField(dictionary, crossTable);
                    field.SetForm(Form);
                    break;
            }

            PdfLoadedField ldField = field as PdfLoadedField;

            if (ldField != null)
            {
                ldField.SetForm(this.Form);
                ldField.BeforeNameChanges += new PdfLoadedField.BeforeNameChangesEventHandler(ldField_NameChanded);
            }

            return field;
        }

#if !SILVERLIGHT && !NETFX_CORE && !WP
        /// <summary>
        /// Creates the signature field.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <returns>The created signature field.</returns>
        private PdfField CreateSignatureField(PdfDictionary dictionary, PdfCrossTable crossTable)
        {
            PdfLoadedField field = new PdfLoadedSignatureField(dictionary, crossTable);
            field.SetForm(Form);

            return field;
        }
#endif
        /// <summary>
        /// Creates the list box.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <returns>The created list box.</returns>
        private PdfField CreateListBox(PdfDictionary dictionary, PdfCrossTable crossTable)
        {
            PdfLoadedField field = new PdfLoadedListBoxField(dictionary, crossTable);
            field.SetForm(Form);

            return field;
        }

        /// <summary>
        /// Creates the combo box.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <returns>The created combo box.</returns>
        private PdfField CreateComboBox(PdfDictionary dictionary, PdfCrossTable crossTable)
        {
            PdfLoadedField field = new PdfLoadedComboBoxField(dictionary, crossTable);
            field.SetForm(Form);

            return field;
        }

        /// <summary>
        /// Creates the text field.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <returns>The created text field.</returns>
        private PdfField CreateTextField(PdfDictionary dictionary, PdfCrossTable crossTable)
        {
            PdfLoadedField field = new PdfLoadedTextBoxField(dictionary, crossTable);
            field.SetForm(Form);

            return field;
        }

        /// <summary>
        /// Creates the radio button.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <returns>The created radio button.</returns>
        private PdfField CreateRadioButton(PdfDictionary dictionary, PdfCrossTable crossTable)
        {
            PdfLoadedField field = new PdfLoadedRadioButtonListField(dictionary, crossTable);
            field.SetForm(Form);

            return field;
        }

        /// <summary>
        /// Creates the check box.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <returns>The created check box.</returns>
        private PdfField CreateCheckBox(PdfDictionary dictionary, PdfCrossTable crossTable)
        {
            PdfLoadedField field = new PdfLoadedCheckBoxField(dictionary, crossTable);
            field.SetForm(Form);

            return field;
        }

        /// <summary>
        /// Creates the push button.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <returns>The created push button.</returns>
        private PdfField CreatePushButton(PdfDictionary dictionary, PdfCrossTable crossTable)
        {
            PdfLoadedField field = new PdfLoadedButtonField(dictionary, crossTable);
            field.SetForm(Form);

            return field;
        }

        /// <summary>
        /// Gets the type of the field.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <returns>The field type.</returns>
        private PdfLoadedFieldTypes GetFieldType(PdfName name, PdfDictionary dictionary, PdfCrossTable crossTable)
        {
            string str = name.Value;
            PdfLoadedFieldTypes type = PdfLoadedFieldTypes.Null;

            PdfNumber number =
                PdfLoadedField.GetValue(dictionary, crossTable, DictionaryProperties.FieldFlags, true) as PdfNumber;

            int fieldFlags = 0;

            if (number != null)
            {
                fieldFlags = number.IntValue;
            }

            switch (str.ToLower())
            {
                case "btn":
                    if ((fieldFlags & (int)FieldFlags.PushButton) != 0)
                    {
                        type = PdfLoadedFieldTypes.PushButton;
                    }
                    else if ((fieldFlags & (int)FieldFlags.Radio) != 0)
                    {
                        type = PdfLoadedFieldTypes.RadioButton;
                    }
                    else
                    {
                        type = PdfLoadedFieldTypes.CheckBox;
                    }
                    break;

                case "tx":
                    type = PdfLoadedFieldTypes.TextField;
                    break;

                case "ch":
                    if ((fieldFlags & (int)FieldFlags.Combo) != 0)
                    {
                        type = PdfLoadedFieldTypes.ComboBox;
                    }
                    else
                    {
                        type = PdfLoadedFieldTypes.ListBox;
                    }
                    break;

                case "sig":
                    type = PdfLoadedFieldTypes.SignatureField;
                    break;
            }

            return type;
        }

        /// <summary>
        /// Adds a field to collection.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns></returns>
        protected override int DoAdd(PdfField field)
        {
            int index = -1;

            if (field == null)
                throw new ArgumentNullException("field");

            field.SetForm(m_form);

            PdfArray array = null;
            if (m_form.Dictionary.ContainsKey(DictionaryProperties.Fields))
            {
                array = m_form.CrossTable.GetObject(m_form.Dictionary[DictionaryProperties.Fields]) as PdfArray;
            }
            else
            {
                array = new PdfArray();
            }

            PdfReferenceHolder reference = new PdfReferenceHolder(field);

            if (!array.Contains(reference))
            {
                if (IsValidName(field.Name))
                {
                    array.Add(new PdfReferenceHolder(field));
                    m_form.Dictionary.SetProperty(DictionaryProperties.Fields, array);
                }
                else
                {
                    if (m_form.FieldAutoNaming)
                    {
                        string newName = GetCorrectName(field.Name);

                        field.ApplyName(newName);
                        array.Add(new PdfReferenceHolder(field));
                        m_form.Dictionary.SetProperty(DictionaryProperties.Fields, array);
                    }
                    else
                    {
                        //TODO: Implement corect field adding to group.
                        throw new PdfDocumentException(String.Format(c_exisingFieldException, field.Name));
                    }
                }
            }
            m_addedFieldNames.Add(field.Name);
            index = base.DoAdd(field);
            return index;
        }

        /// <summary>
        /// Inserts a filed into collection.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="field">The field.</param>
        protected override void DoInsert(int index, PdfField field)
        {
            if (index < 0 || index > List.Count)
                throw new IndexOutOfRangeException();

            if (field == null)
                throw new ArgumentNullException("field");

            field.SetForm(m_form);

            if (!(field is PdfLoadedField))
            {
                PdfArray array = null;
                if (m_form.Dictionary.ContainsKey(DictionaryProperties.Fields))
                {
                    array = m_form.CrossTable.GetObject(m_form.Dictionary[DictionaryProperties.Fields]) as PdfArray;
                }
                else
                {
                    array = new PdfArray();
                }
                array.Insert(index, new PdfReferenceHolder(field));
                m_form.Dictionary.SetProperty(DictionaryProperties.Fields, array);
            }

            base.DoInsert(index, field);
        }

        /// <summary>
        /// Removes the field from collection.
        /// </summary>
        /// <param name="field">The field.</param>
        protected override void DoRemove(PdfField field)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            m_form.RemoveFromDictionaries(field);

            base.DoRemove(field);
        }

        /// <summary>
        /// Removes the field at the specified position.
        /// </summary>
        /// <param name="index">The index.</param>
        protected override void DoRemoveAt(int index)
        {
            if (index < 0 || index > List.Count)
                throw new IndexOutOfRangeException();

            PdfField field = List[index] as PdfField;

            if (field is PdfLoadedField)
            {
                m_form.RemoveFromDictionaries(field);
            }

            base.DoRemoveAt(index);
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        protected override void DoClear()
        {
            for (int i = 0, size = List.Count; i < size; ++i)
            {
                PdfLoadedField field = List[i] as PdfLoadedField;
                if (field != null)
                {
                    m_form.RemoveFromDictionaries(field);
                }
            }

            m_addedFieldNames.Clear();
            m_form.TerminalFields.Clear();

            base.DoClear();
        }

        /// <summary>
        /// Check whether the field with the same name already exists.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        /// <c>true</c> if there are no fields with the same name within the collection; 
        /// otherwise <c>false</c>.
        /// </returns>
        internal bool IsValidName(string name)
        {
            //foreach (PdfField field in List)
            //{
            //    if (field.Name == name)
            //    {
            //        return false;
            //    }
            //}

            //return true;

            return(!m_addedFieldNames.Contains(name));
        }

        /// <summary>
        /// Gets the new name of the field.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The field name.</returns>
        internal string GetCorrectName(string name)
        {
            List<string> list = new List<string>();

            foreach (PdfField field in List)
            {
                list.Add(field.Name);
            }

            string correctName = name;

            int index = 0;

            while (list.IndexOf(correctName) != -1)
            {
                correctName = name + index;
                ++index;
            }

            return correctName;
        }

        /// <summary>
        /// Adds the field dictionary.
        /// </summary>
        /// <param name="field">The field.</param>
        internal void AddFieldDictionary(PdfDictionary field)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            List.Add(field);
            Items.Add(new PdfReferenceHolder(field));
        }

        /// <summary>
        /// NameChanged evant handler.
        /// </summary>
        /// <param name="name">New Name of the field.</param>
        private void ldField_NameChanded(string name)
        {
            if (!IsValidName(name))
                throw new ArgumentException("Field with the same name already exist");
        }

        /// <summary>
        /// Gets the index of the field.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The index of the field.</returns>
        private int GetFieldIndex(string name)
        {
            int i = -1;

            if (m_fieldNames == null)
            {
                m_fieldNames = new List<string>();
                m_indexedFieldNames = new List<string>();
                foreach (PdfField field in List)
                {

                    m_fieldNames.Add(field.Name);
                    m_indexedFieldNames.Add(field.Name.Split('[')[0]);
                }

            }
           
            if (m_fieldNames.Contains(name))
                i = m_fieldNames.IndexOf(name);
            else if (m_indexedFieldNames.Contains(name))
                i = m_indexedFieldNames.IndexOf(name);
            

            if (i < 0)
            {
                if (m_actualFieldNames == null)
                {
                    m_actualFieldNames = new List<string>();
                    m_indexedActualFieldNames = new List<string>();

                    foreach (PdfLoadedField field in List)
                    {

                        m_actualFieldNames.Add(field.ActualFieldName);
                        m_indexedActualFieldNames.Add(field.ActualFieldName.Split('[')[0]);
                    }
                }
               
                if (m_actualFieldNames.Contains(name))
                    i = m_actualFieldNames.IndexOf(name);
                else if (m_indexedActualFieldNames.Contains(name))
                    i = m_indexedActualFieldNames.IndexOf(name);
                

            }
            //foreach (PdfField field in List)
            //{
            //    ++i;
            //    if (field.Name == name)
            //        break;
            //    else
            //    {
            //        if (field.Name != null || field.Name != string.Empty)
            //        {
            //            string[] fieldNames = field.Name.Split('[');
            //            if (fieldNames[0] == name)
            //                return i;
            //        }
            //    }

            //}

            //if ((i == List.Count - 1) &&
            //    ((List[List.Count - 1] as PdfLoadedField).Name != name))
            //{
            //    i = -1;
            //}

            //if (i == -1)
            //{
            //    foreach (PdfLoadedField field in List)
            //    {
            //        ++i;
            //        if (field.ActualFieldName == name)
            //            break;
            //        else
            //        {
            //            if (field.ActualFieldName != null || field.ActualFieldName != string.Empty)
            //            {
            //                string[] fieldNames = field.ActualFieldName.Split('[');

            //                if (fieldNames[0] == name)
            //                    return i;
            //            }
            //        }
            //    }

            //    if ((i == List.Count - 1) &&
            //   ((List[List.Count - 1] as PdfLoadedField).ActualFieldName != name))
            //    {
            //        i = -1;
            //    }
            //}
            return i;
        }

        /// <summary>
        /// Gets the named field.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The field with specified name.</returns>
        private PdfField GetNamedField(string name)
        {
            PdfField nField = null;

            foreach (PdfField field in List)
            {
                if (field.Name == name)
                {
                    nField = field;
                }
            }
            return nField;
        }

        /// <summary>
        /// Gets the form field with the given field name
        /// </summary>
        /// <param name="fieldName">Name of the field</param>
        /// <param name="field">Loaded Form Field</param>
        /// <returns>Loaded Form Field</returns>
        public bool TryGetField(string fieldName,out PdfLoadedField field)
        {
            field = null;
            int index = GetFieldIndex(fieldName);

            if (index > -1)
            {
                field = List[index] as PdfLoadedField;
                return true;
            }
            return false;

        }
        
        /// <summary>
        /// Gets the filed value from the given filed name
        /// </summary>
        /// <param name="fieldName">Name of the loaded form filed</param>
        /// <param name="fieldValue">Value of the field</param>
        /// <returns>vale of the field</returns>
        public bool TryGetValue(string fieldName, out string fieldValue)
        {
            fieldValue = string.Empty;
            int index = GetFieldIndex(fieldName);

            if (index > -1)
            {
                fieldValue = (List[index] as PdfLoadedTextBoxField).Text;
                return true;
            }
            return false;

        }
        #endregion
    }
}
