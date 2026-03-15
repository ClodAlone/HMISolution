#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;

using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Primitives;
using System.Collections.Generic;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents collection of the Pdf fields.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Create a new PDf document
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = document.Pages.Add();
    /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12f);          
    /// //Create a combo box
    /// PdfComboBoxField positionComboBox = new PdfComboBoxField(page, "positionComboBox"); 
    /// positionComboBox.Bounds = new RectangleF(100, 115, 200, 20);
    /// positionComboBox.Font = font;
    /// positionComboBox.Editable = true;
    /// //Add it to document
    /// document.Form.Fields.Add(positionComboBox);
    /// PdfFieldCollection fieldCollection = document.Form.Fields;
    /// // Flatten the form collection
    /// for (int i = 0; i < fieldCollection.Count; i++)
    /// {
    ///  fieldCollection[i].Flatten = true;
    /// }
    /// document.Save("Form.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDf document
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Create a page
    /// Dim page As PdfPage = document.Pages.Add()
    /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier, 12f)
    /// 'Create a combo box
    /// Dim positionComboBox As PdfComboBoxField = New PdfComboBoxField(page, "positionComboBox")
    /// positionComboBox.Bounds = New RectangleF(100, 115, 200, 20)
    /// positionComboBox.Font = font
    /// positionComboBox.Editable = True
    /// 'Add it to document
    /// document.Form.Fields.Add(positionComboBox)
    /// Dim fieldCollection As PdfFieldCollection = document.Form.Fields
    /// ' Flatten the form collection
    /// For i As Integer = 0 To fieldCollection.Count - 1
    /// fieldCollection(i).Flatten = True
    /// Next i
    /// document.Save("Form.pdf")
    /// </code>
    /// </example>
    ///  <seealso cref="PdfDocument"/> Class  
    /// <seealso cref="PdfComboBoxField"/> Class   
    /// <seealso cref="PdfCollection"/> Class    
    /// <seealso cref="IPdfWrapper"/> Interface   
    public class PdfFieldCollection : PdfCollection, IPdfWrapper
    {
        #region Constants
        /// <summary>
        /// Internal variable to store duplicate field exception message.
        /// </summary>
        internal string c_exisingFieldException = "The field with '{0}' name already exists";
        #endregion

        #region Fields
        /// <summary>
        /// Internal variable to store array of fields.
        /// </summary>
        private PdfArray m_array = new PdfArray();
        ///<summary>
        ///Private variable to store the form field names and thier index
        ///</summary>
        private Dictionary<string, int> m_fieldNames = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfFieldCollection"/> class.
        /// </summary>
        public PdfFieldCollection()
            : base()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the <see cref="Syncfusion.Pdf.Interactive.PdfField"/> at the specified index.
        /// </summary>        
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12f);          
        /// //Create a combo box
        /// PdfComboBoxField positionComboBox = new PdfComboBoxField(page, "positionComboBox");    
        /// positionComboBox.Bounds = new RectangleF(100, 115, 200, 20);
        /// positionComboBox.Font = font;
        /// positionComboBox.Editable = true;
        /// //Add it to document
        /// document.Form.Fields.Add(positionComboBox);
        /// PdfFieldCollection fieldCollection = document.Form.Fields;
        /// // Flatten the form collection
        /// for (int i = 0; i < fieldCollection.Count; i++)
        /// {
        ///  fieldCollection[i].Flatten = true;
        /// }
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier, 12f)
        /// 'Create a combo box
        /// Dim positionComboBox As PdfComboBoxField = New PdfComboBoxField(page, "positionComboBox")
        /// positionComboBox.Editable = True      
        /// positionComboBox.Bounds = New RectangleF(100, 115, 200, 20)
        /// positionComboBox.Font = font
        /// positionComboBox.Editable = True
        /// 'Add it to document
        /// document.Form.Fields.Add(positionComboBox)
        /// Dim fieldCollection As PdfFieldCollection = document.Form.Fields
        /// ' Flatten the form collection
        /// For i As Integer = 0 To fieldCollection.Count - 1
        /// fieldCollection(i).Flatten = True
        /// Next i
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfComboBoxField"/> Class 
        public virtual PdfField this[int index]
        {
            get
            {
                return (PdfField)List[index];
            }
        }
        /// <summary>
        /// Gets the <see cref="Syncfusion.Pdf.Interactive.PdfField"/> with thier field name.
        /// </summary>   
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
        /// Gets the items.
        /// </summary>
        /// <value>The items.</value>
        internal PdfArray Items
        {
            get
            {
                return m_array;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds the specified field.
        /// </summary>
        /// <param name="field">The field item which is added in the PDF form.</param>
        /// <returns>The field to be added on the page. </returns>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12f);                     
        /// PdfFieldCollection fieldCollection = document.Form.Fields;
        /// //Create a combo box
        /// PdfComboBoxField positionComboBox = new PdfComboBoxField(page, "positionComboBox");
        /// positionComboBox.Editable = true;        
        /// positionComboBox.Bounds = new RectangleF(100, 115, 200, 20);
        /// positionComboBox.Font = font;
        /// positionComboBox.Editable = true;
        /// fieldCollection.Add(positionComboBox as PdfField);
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier, 12f)
        /// Dim fieldCollection As PdfFieldCollection = document.Form.Fields
        /// 'Create a combo box
        /// Dim positionComboBox As PdfComboBoxField = New PdfComboBoxField(page, "positionComboBox")
        /// positionComboBox.Editable = True        
        /// positionComboBox.Bounds = New RectangleF(100, 115, 200, 20)
        /// positionComboBox.Font = font
        /// positionComboBox.Editable = True
        /// fieldCollection.Add(TryCast(positionComboBox, PdfField))
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfPage"/> Class  
        /// <seealso cref="PdfFont"/> Class  
        public int Add(PdfField field)
        {
            if (field == null)
            {
                throw new ArgumentNullException("field");
            }

            return DoAdd(field);
        }

        /// <summary>
        /// Inserts the the field at the specified index.
        /// </summary>
        /// <param name="index">The index of the field.</param>
        /// <param name="field">The field which should be inserted at the specified index.</param>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12f);
        /// //Create a check box
        /// PdfCheckBoxField checkBox = new PdfCheckBoxField(page, "C#.NET");
        /// checkBox.Bounds = new RectangleF(100, 290, 20, 20);
        /// document.Form.Fields.Add(checkBox);
        /// checkBox.HighlightMode = PdfHighlightMode.Push;
        /// checkBox.BorderStyle = PdfBorderStyle.Beveled;
        /// //Set the value for the check box
        /// checkBox.Checked = true;
        /// document.Form.Fields.Add(checkBox);
        /// PdfFieldCollection fieldCollection = document.Form.Fields;
        /// //Create a combo box
        /// PdfComboBoxField positionComboBox = new PdfComboBoxField(page, "positionComboBox");
        /// positionComboBox.Editable = true;
        /// positionComboBox.Bounds = new RectangleF(100, 115, 200, 20);
        /// positionComboBox.Font = font;
        /// positionComboBox.Editable = true;
        /// // Insert the field at first position in the collection
        /// fieldCollection.Insert(0, positionComboBox as PdfField);
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier, 12f)
        /// 'Create a check box
        /// Dim checkBox As PdfCheckBoxField = New PdfCheckBoxField(page, "C#.NET")
        /// checkBox.Bounds = New RectangleF(100, 290, 20, 20)
        /// document.Form.Fields.Add(checkBox)
        /// checkBox.HighlightMode = PdfHighlightMode.Push
        /// checkBox.BorderStyle = PdfBorderStyle.Beveled
        /// 'Set the value for the check box
        /// checkBox.Checked = True
        /// document.Form.Fields.Add(checkBox)
        /// Dim fieldCollection As PdfFieldCollection = document.Form.Fields
        /// 'Create a combo box
        /// Dim positionComboBox As PdfComboBoxField = New PdfComboBoxField(page, "positionComboBox")
        /// positionComboBox.Editable = True
        /// positionComboBox.Bounds = New RectangleF(100, 115, 200, 20)
        /// positionComboBox.Font = font
        /// positionComboBox.Editable = True
        /// ' Insert the field at first position in the collection
        /// fieldCollection.Insert(0, TryCast(positionComboBox, PdfField))
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfPage"/> Class  
        /// <seealso cref="PdfFont"/> Class  
        public void Insert(int index, PdfField field)
        {
            if (field == null)
            {
                throw new ArgumentNullException("field");
            }

            DoInsert(index, field);
        }

        /// <summary>
        /// Determines whether field is contained within the collection.
        /// </summary>
        /// <param name="field">Check whether <see cref="PdfField"/> object is present in the field collection or not.</param>
        /// <returns>
        /// <c>true</c> if field is present in the collection, otherwise, <c>false</c>.
        /// </returns>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12f);                     
        /// PdfFieldCollection fieldCollection = document.Form.Fields;
        /// //Create a combo box
        /// PdfComboBoxField positionComboBox = new PdfComboBoxField(page, "positionComboBox");
        /// positionComboBox.Editable = true;        
        /// positionComboBox.Bounds = new RectangleF(100, 115, 200, 20);
        /// positionComboBox.Font = font;
        /// positionComboBox.Editable = true;
        /// if (fieldCollection.Contains(positionComboBox as PdfField))
        ///  MessageBox.Show("Already added field");
        /// else
        ///  fieldCollection.Add(positionComboBox as PdfField);
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier, 12f)
        /// Dim fieldCollection As PdfFieldCollection = document.Form.Fields
        /// 'Create a combo box
        /// Dim positionComboBox As PdfComboBoxField = New PdfComboBoxField(page, "positionComboBox")
        /// positionComboBox.Editable = True
        /// positionComboBox.Bounds = New RectangleF(100, 115, 200, 20)
        /// positionComboBox.Font = font
        /// positionComboBox.Editable = True
        /// If fieldCollection.Contains(TryCast(positionComboBox, PdfField)) Then
        /// MessageBox.Show("Already added field")
        /// Else
        /// fieldCollection.Add(TryCast(positionComboBox, PdfField))
        /// End If
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfPage"/> Class  
        /// <seealso cref="PdfFont"/> Class  
        public bool Contains(PdfField field)
        {
            return List.Contains(field);
        }

        /// <summary>
        /// Gets the index of the field.
        /// </summary>
        /// <param name="field">The <see cref="PdfField"/> object whose index is requested.</param>
        /// <returns>Index of the field in collection.</returns>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12f);
        /// PdfFieldCollection fieldCollection = document.Form.Fields;
        /// //Create a combo box
        /// PdfComboBoxField positionComboBox = new PdfComboBoxField(page, "positionComboBox");
        /// positionComboBox.Editable = true;
        /// positionComboBox.Bounds = new RectangleF(100, 115, 200, 20);
        /// positionComboBox.Font = font;
        /// positionComboBox.Editable = true;
        /// document.Form.Fields.Add(positionComboBox);
        /// int index = fieldCollection.IndexOf(positionComboBox as PdfField);
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier, 12f)
        /// Dim fieldCollection As PdfFieldCollection = document.Form.Fields
        /// 'Create a combo box
        /// Dim positionComboBox As PdfComboBoxField = New PdfComboBoxField(page, "positionComboBox")
        /// positionComboBox.Editable = True
        /// positionComboBox.Bounds = New RectangleF(100, 115, 200, 20)
        /// positionComboBox.Font = font
        /// positionComboBox.Editable = True
        /// document.Form.Fields.Add(positionComboBox)
        /// Dim index As Integer = fieldCollection.IndexOf(TryCast(positionComboBox, PdfField))
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfPage"/> Class  
        /// <seealso cref="PdfFont"/> Class  
        public int IndexOf(PdfField field)
        {
            if (field == null)
            {
                throw new ArgumentNullException("field");
            }

            return List.IndexOf(field);
        }

        /// <summary>
        /// Removes the specified field in the collection.
        /// </summary>
        /// <param name="field">The <see cref="PdfField"/> object to be removed from collection.</param>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12f);
        /// PdfFieldCollection fieldCollection = document.Form.Fields;
        /// //Create a combo box
        /// PdfComboBoxField positionComboBox = new PdfComboBoxField(page, "positionComboBox");
        /// document.Form.Fields.Add(positionComboBox);
        /// positionComboBox.Editable = true;      
        /// positionComboBox.Bounds = new RectangleF(100, 115, 200, 20);
        /// positionComboBox.Font = font;
        /// positionComboBox.Editable = true;          
        /// fieldCollection.Remove(positionComboBox as PdfField);
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier, 12f)
        /// Dim fieldCollection As PdfFieldCollection = document.Form.Fields
        /// 'Create a combo box
        /// Dim positionComboBox As PdfComboBoxField = New PdfComboBoxField(page, "positionComboBox")
        /// document.Form.Fields.Add(positionComboBox)
        /// positionComboBox.Editable = True
        /// positionComboBox.Bounds = New RectangleF(100, 115, 200, 20)
        /// positionComboBox.Font = font
        /// positionComboBox.Editable = True
        /// fieldCollection.Remove(TryCast(positionComboBox, PdfField))
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfPage"/> Class  
        /// <seealso cref="PdfFont"/> Class
        public void Remove(PdfField field)
        {
            if (field == null)
            {
                throw new ArgumentNullException("field");
            }

            DoRemove(field);
        }

        /// <summary>
        /// Removes field at the specified position.
        /// </summary>
        /// <param name="index">The index where to remove the item.</param>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12f);
        /// PdfFieldCollection fieldCollection = document.Form.Fields;
        /// //Create a combo box
        /// PdfComboBoxField positionComboBox = new PdfComboBoxField(page, "positionComboBox");
        /// document.Form.Fields.Add(positionComboBox);
        /// positionComboBox.Editable = true;
        /// positionComboBox.Bounds = new RectangleF(100, 115, 200, 20);
        /// positionComboBox.Font = font;
        /// positionComboBox.Editable = true;
        /// // Remove the first element
        /// fieldCollection.RemoveAt(0);
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier, 12f)
        /// Dim fieldCollection As PdfFieldCollection = document.Form.Fields
        /// 'Create a combo box
        /// Dim positionComboBox As PdfComboBoxField = New PdfComboBoxField(page, "positionComboBox")
        /// document.Form.Fields.Add(positionComboBox)
        /// positionComboBox.Editable = True
        /// positionComboBox.Bounds = New RectangleF(100, 115, 200, 20)
        /// positionComboBox.Font = font
        /// positionComboBox.Editable = True
        /// ' Remove the first element
        /// fieldCollection.RemoveAt(0)
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfPage"/> Class  
        /// <seealso cref="PdfFont"/> Class
        public void RemoveAt(int index)
        {
            DoRemoveAt(index);
        }

        /// <summary>
        /// Clears the form field collection.
        /// </summary>
        public void Clear()
        {
            DoClear();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Adds the specified field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="newPage">The new page.</param>
        /// <returns>Index of added field.</returns>
        internal int Add(PdfField field, PdfPageBase newPage)
        {
            PdfField newField = null;

            if (field is PdfLoadedField)
            {
                newField = InsertLoadedField(field as PdfLoadedField, newPage);
            }

            int index = DoAdd(newField);

            if (field is PdfLoadedField && (newField.ReadOnly != (field as PdfLoadedField).ReadOnly))
                newField.ReadOnly = (field as PdfLoadedField).ReadOnly;

            return index;
        }

        /// <summary>
        /// Adds a field to collection.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns>field.</returns>
        protected virtual int DoAdd(PdfField field)
        {
            m_array.Add(new PdfReferenceHolder(field));

            return List.Add(field);
        }

        /// <summary>
        /// Inserts a filed into collection.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="field">The field.</param>
        protected virtual void DoInsert(int index, PdfField field)
        {
            m_array.Insert(index, new PdfReferenceHolder(field));
            List.Insert(index, field);
        }

        /// <summary>
        /// Removes the field from collection.
        /// </summary>
        /// <param name="field">The field.</param>
        protected virtual void DoRemove(PdfField field)
        {
            int index = List.IndexOf(field);
            m_array.RemoveAt(index);
            List.Remove(field);
        }

        /// <summary>
        /// Removes the field at the specified position.
        /// </summary>
        /// <param name="index">The index.</param>
        protected virtual void DoRemoveAt(int index)
        {
            m_array.RemoveAt(index);
            List.RemoveAt(index);
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        protected virtual void DoClear()
        {
            m_array.Clear();
            List.Clear();
        }

        /// <summary>
        /// Inserts the loaded field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="newPage">The new page.</param>
        /// <returns>field.</returns>
        private PdfField InsertLoadedField(PdfLoadedField field, PdfPageBase newPage)
        {
            if (!(newPage as PdfPage).Section.ParentDocument.EnableMemoryOptimization)
            {
                PdfDictionary fieldDic = field.Dictionary;
                PdfDictionary oldPageDic = field.Page.Dictionary;
                PdfDictionary newPageDic = newPage.Dictionary;

                field = field.Clone(newPage) as PdfLoadedField;

                PdfArray kidsArray = field.CrossTable.GetObject(fieldDic[DictionaryProperties.Kids]) as PdfArray;
                PdfArray array = field.CrossTable.GetObject(oldPageDic[DictionaryProperties.Annots]) as PdfArray;
                PdfArray newArray = field.CrossTable.GetObject(newPageDic[DictionaryProperties.Annots]) as PdfArray;

                if (kidsArray != null)
                {
                    kidsArray = new PdfArray(kidsArray);
                    field.Dictionary[DictionaryProperties.Kids] = kidsArray;
                    UpdateReferences(kidsArray, array, newArray, field);
                    field.Dictionary.Remove(DictionaryProperties.P);
                }
                else
                {
                    // Fix field dictionary reference in case the field dictionary was cloned as annotation.
                    PdfReferenceHolder rh = new PdfReferenceHolder(fieldDic);
                    int index = array.IndexOf(rh);
                    if (index >= 0)
                        field.Dictionary = PdfCrossTable.Dereference(newArray[index]) as PdfDictionary;
                }

                return field;
            }
            else
            {
                PdfArray annots = null;
                int num = 0;
                if (newPage.Dictionary.ContainsKey(DictionaryProperties.Annots))
                {
                    annots = newPage.GetAnnots();
                    num = annots.Count;
                }
                else
                {
                    annots = new PdfArray();
                    newPage.Dictionary.SetProperty(DictionaryProperties.Annots, annots);
                }

                PdfField newField = field.Clone(newPage);

                bool signatureField = false;

#if !SILVERLIGHT && !NETFX_CORE && !WP
                if (newField is PdfLoadedSignatureField)
                    signatureField = true;
# endif

                PdfArray kidsArray = field.CrossTable.GetObject(field.Dictionary[DictionaryProperties.Kids]) as PdfArray;

                if (kidsArray != null && kidsArray.Count > 0 && !signatureField)
                {
                    PdfCrossTable crossTable = (newPage as PdfPage).Section.ParentDocument.CrossTable;

                    for (int k = 0; k < kidsArray.Count; k++)
                    {
                        PdfDictionary dictionary = PdfCrossTable.Dereference(kidsArray[k]) as PdfDictionary;
                        PdfDictionary dict = new PdfDictionary(dictionary);
                        PdfName parent = new PdfName(DictionaryProperties.Parent);
                        PdfName p = new PdfName(DictionaryProperties.P);
                        dict.Remove(parent);
                        dict.Remove(p);

                        PdfDictionary kidsDict = dict.Clone(crossTable) as PdfDictionary;
                        kidsDict[parent] = new PdfReferenceHolder(newField);
                        PdfPageBase kidsPage = null;

                        // Create a new kids dictionary   
                        if (dictionary.ContainsKey(p))
                        {
                            PdfDictionary oldPage = PdfCrossTable.Dereference(dictionary[p]) as PdfDictionary;
                            if (oldPage != null && field.CrossTable.PageCorrespondance.ContainsKey(oldPage) && field.CrossTable.PageCorrespondance[oldPage] != null)
                            {
                                kidsPage = field.CrossTable.PageCorrespondance[oldPage] as PdfPageBase;
                                if (kidsPage == newPage)
                                    kidsDict[p] = new PdfReferenceHolder(kidsPage);
                                else
                                    continue;
                            }
                            else
                                continue;
                        }
                        else
                        {
                            kidsDict[p] = new PdfReferenceHolder(newPage);
                            kidsPage = newPage;
                        }

                        PdfLoadedFieldItem item = (newField as PdfLoadedField).CreateLoadedItem(kidsDict);
                        if (kidsPage != null)
                            item.Page = kidsPage;

                        annots = newPage.GetAnnots();
                        if (num < annots.Count)
                        {
                            for (int r = annots.Count - 1; r >= num; r--)
                                annots.RemoveAt(r);
                        }
                        annots.Add(new PdfReferenceHolder(kidsDict));

                        num++;
                    }
                }
                else
                {
                    annots = newPage.GetAnnots();
                    if (num < annots.Count)
                    {
                        for (int r = annots.Count - 1; r >= num; r--)
                            annots.RemoveAt(r);
                    }
                    annots.Add(new PdfReferenceHolder(newField));
                    num++;
                }
                return newField;
            }
        }

        /// <summary>
        /// Updates the references.
        /// </summary>
        /// <param name="kidsArray">The kids array.</param>
        /// <param name="array">The array.</param>
        /// <param name="newArray">The new array.</param>
        private void UpdateReferences(PdfArray kidsArray, PdfArray array, PdfArray newArray, PdfField field)
        {
            if (kidsArray != null)
            {
                for (int i = 0, count = kidsArray.Count; i < count; ++i)
                {
                    PdfReferenceHolder reference = kidsArray[i] as PdfReferenceHolder;
                    if (array != null)
                    {
                        int index = array.IndexOf(reference);

                        if (index >= 0)
                        {
                            IPdfPrimitive obj = newArray[index];
                            kidsArray.RemoveAt(i);
                            kidsArray.Insert(i, obj);

                            PdfDictionary dic = PdfCrossTable.Dereference(obj) as PdfDictionary;

                            if (dic.ContainsKey(DictionaryProperties.Parent))
                            {
                                dic[DictionaryProperties.Parent] = new PdfReferenceHolder(field);
                            }
                        }
                    }                   
                }
            }
        }
        /// <summary>
        /// Returns the form field index by validating the fieldnames
        /// </summary>
        private int GetFieldIndex(string name)
        {
            int i = -1;

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
            //    if ((i == List.Count - 1) && ((List[List.Count - 1] as PdfLoadedField).ActualFieldName != name))
            //    {
            //        i = -1;
            //    }
            //}

            if (m_fieldNames == null)
            {
                m_fieldNames = new Dictionary<string, int>();

                foreach (PdfField field in List)
                {
                    ++i;
                    string[] fieldNames = field.Name.Split('[');
                    m_fieldNames.Add(fieldNames[0], i);
                }
            }
            int index = -1;
            m_fieldNames.TryGetValue(name, out index);


            return index;
        }        
        #endregion

        #region IPdfWrapper Members
        /// <summary>
        /// Gets the element.
        /// </summary>
        /// <value></value>
        Syncfusion.Pdf.Primitives.IPdfPrimitive IPdfWrapper.Element
        {
            get
            {
                return m_array;
            }
        }
        #endregion
    }
}
