#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.Generic;

using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Primitives;
using Syncfusion.Pdf.Security;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents interactive form of the Pdf document.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Create a new PDf document
    /// PdfDocument document = new PdfDocument();           
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = document.Pages.Add();
    /// // Creates a form
    /// PdfForm form = document.Form;
    /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12f);
    /// //Create list box
    /// PdfListBoxField listBox = new PdfListBoxField(page, "list1");
    /// //Add the field to listbox.
    /// form.Fields.Add(listBox);            
    /// //Set the properties.
    /// listBox.Bounds = new RectangleF(100, 350, 100, 50);
    /// listBox.HighlightMode = PdfHighlightMode.Outline;
    /// //Add the items to the list box
    /// listBox.Items.Add(new PdfListFieldItem("English", "English"));
    /// listBox.Items.Add(new PdfListFieldItem("French", "French"));
    /// listBox.Items.Add(new PdfListFieldItem("German", "German"));
    /// //Select the item
    /// listBox.SelectedIndex = 2;
    /// //Set the multiselect option
    /// listBox.MultiSelect = true;                    
    /// document.Save("Form.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDf document
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Create a page
    /// Dim page As PdfPage = document.Pages.Add()
    /// ' Creates a form
    /// Dim form As PdfForm = document.Form
    /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier, 12f)
    /// 'Create list box
    /// Dim listBox As PdfListBoxField = New PdfListBoxField(page, "list1")
    /// 'Add the field to listbox.
    /// form.Fields.Add(listBox)
    /// 'Set the properties.
    /// listBox.Bounds = New RectangleF(100, 350, 100, 50)
    /// listBox.HighlightMode = PdfHighlightMode.Outline
    /// 'Add the items to the list box
    /// listBox.Items.Add(New PdfListFieldItem("English", "English"))
    /// listBox.Items.Add(New PdfListFieldItem("French", "French"))
    /// listBox.Items.Add(New PdfListFieldItem("German", "German"))
    /// 'Select the item
    /// listBox.SelectedIndex = 2
    /// 'Set the multiselect option
    /// listBox.MultiSelect = True
    /// document.Save("Form.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="IPdfWrapper"/> Interface    
    public class PdfForm : IPdfWrapper
    {
        #region Fields
        /// <summary>
        /// Internal variable to store form's collection of fields.
        /// </summary>
        private PdfFormFieldCollection m_fields = new PdfFormFieldCollection();

        /// <summary>
        /// Internal variable to store resources.
        /// </summary>
        private PdfResources m_resources = null;

        /// <summary>
        /// Internal value indicating whether the form is read only.
        /// </summary>
        private bool m_readOnly = false;

        /// <summary>
        /// Internal variable to store signature flags.
        /// </summary>
        private SignatureFlags m_signatureFlags = SignatureFlags.None;

        /// <summary>
        /// Internal variable to store dictionary.
        /// </summary>
        private PdfDictionary m_dictionary = new PdfDictionary();

        /// <summary>
        /// Internal variable to store value whether to generate default appearancesof the fields.
        /// </summary>
        private bool m_needAppearances = true;

        /// <summary>
        /// Indicates is form flatten or not.
        /// </summary>
        private bool m_flatten;

        /// <summary>
        /// Internal variable to store value whether to change the name of the field.
        /// </summary>
        private bool m_changeName = true;

        /// <summary>
        /// Holds the fields names of the form.
        /// </summary>
        private List<string> m_fieldName = new List<string>();
        
        /// <summary>
        /// Internal values to hold form fields values.
        /// </summary>
        private List<string> m_fieldNames = new List<string>();

        /// <summary>
        /// Internal variable to ensure the form is XFA or not
        /// </summary>
        private bool m_isXFA = false;

        /// <summary>
        /// Indicates if AutoFormat has to be removed.
        /// </summary>
        private bool m_disableAutoFormat;

        /// <summary>
        /// Holds the reference of the loaded page and the cloned page
        /// </summary>
        internal Dictionary<PdfDictionary, PdfPageBase> m_pageMap = new Dictionary<PdfDictionary, PdfPageBase>();

        /// <summary>
        /// Internal field to check whether any field is modified.
        /// </summary>
        private bool m_setAppearanceDictionary = false;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfForm"/> class.
        /// </summary>
        public PdfForm()
            : base()
        {
            m_fields.Form = this;

            m_dictionary.SetProperty(DictionaryProperties.Fields, m_fields);

            m_dictionary.BeginSave += new SavePdfPrimitiveEventHandler(Dictionary_BeginSave);
            m_setAppearanceDictionary = true;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the field names.
        /// </summary>
        /// <value>The field names.</value>
        internal List<string> FieldNames
        {
            get
            {
                return m_fieldName;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the form is XFA.
        /// </summary>
        internal bool IsXFA
        {
            get
            {
                return m_isXFA;
            }
            set
            {
                m_isXFA = true;
            }
        }

        /// <summary>
        /// Gets the fields.
        /// </summary>
        /// <value>The Form fields.</value>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12f);          
        /// //Create a combo box
        /// PdfComboBoxField positionComboBox = new PdfComboBoxField(page, "positionComboBox");
        /// positionComboBox.Editable = true;
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
        public PdfFormFieldCollection Fields
        {
            get
            {
                return m_fields;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PdfForm"/> is flatten.
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
        /// positionComboBox.Editable = true;
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
        /// <seealso cref="PdfFieldCollection"/> Class 
        /// <seealso cref="PdfDocument"/> Class 
        /// <seealso cref="PdfComboBoxField"/> Class 
        public bool Flatten
        {
            get
            {
                return m_flatten;
            }

            set
            {
                m_flatten = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the form is read only.
        /// </summary>
        /// <value><c>true</c> if the form is read only; otherwise, <c>false</c>.</value>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();
        /// PdfForm form = document.Form;
        /// // Sets the form as read only
        /// form.ReadOnly = true;
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12f);          
        /// //Create a combo box
        /// PdfComboBoxField positionComboBox = new PdfComboBoxField(page, "positionComboBox");
        /// positionComboBox.Editable = true;
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
        /// ' Creates a form
        /// Dim form As PdfForm = document.Form
        /// ' Sets the form as read only
        /// form.ReadOnly = True
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
        /// <seealso cref="PdfFieldCollection"/> Class 
        /// <seealso cref="PdfDocument"/> Class 
        /// <seealso cref="PdfComboBoxField"/> Class      
        public virtual bool ReadOnly
        {
            get
            {
                return m_readOnly;
            }

            set
            {
                m_readOnly = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [field auto naming].
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();
        /// // Creates a form
        /// PdfForm form = document.Form;
        /// // Sets the form fields as auto naming.
        /// form.FieldAutoNaming = true;
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12f);          
        /// //Create a combo box
        /// PdfComboBoxField positionComboBox = new PdfComboBoxField(page, "positionComboBox");
        /// positionComboBox.Editable = true;
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
        /// ' Creates a form
        /// Dim form As PdfForm = document.Form
        /// ' Sets the form fields as auto naming.
        /// form.FieldAutoNaming = True
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
        /// <seealso cref="PdfFieldCollection"/> Class 
        /// <seealso cref="PdfDocument"/> Class 
        /// <seealso cref="PdfComboBoxField"/> Class
        public bool FieldAutoNaming
        {
            get
            {
                return m_changeName;
            }

            set
            {
                m_changeName = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the viewer must generate appearances for fields.
        /// </summary>
        /// <value><c>true</c> if viewer must generate appearance; otherwise, <c>false</c>.</value>
        internal virtual bool NeedAppearances
        {
            get
            {
                return m_needAppearances;
            }

            set
            {
                if (m_needAppearances != value)
                {
                    m_needAppearances = value;
                    //m_dictionary.SetBoolean(DictionaryProperties.NeedAppearances, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the signature flags.
        /// </summary>
        /// <value>The signature flags.</value>
        internal virtual SignatureFlags SignatureFlags
        {
            get
            {
                return m_signatureFlags;
            }

            set
            {
                if (m_signatureFlags != value)
                {
                    m_signatureFlags = value;
                    m_dictionary.SetNumber(DictionaryProperties.SigFlags, (int)m_signatureFlags);
                }
            }
        }

        /// <summary>
        /// Gets or sets the resources.
        /// </summary>
        /// <value>The resources.</value>
        internal virtual PdfResources Resources
        {
            get
            {
                if (m_resources == null)
                {
                    m_resources = new PdfResources();
                    m_dictionary.SetProperty(DictionaryProperties.DR, m_resources);
                }

                return m_resources;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("resources");
                }

                m_resources = value;
            }
        }

        /// <summary>
        /// Gets or sets the dictionary.
        /// </summary>
        /// <value>The dictionary.</value>
        internal virtual PdfDictionary Dictionary
        {
            get
            {
                return m_dictionary;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Dictionary");
                }

                m_dictionary = value;
            }
        }

        public bool DisableAutoFormat
        {
            get
            {
                return m_disableAutoFormat;
            }
            set
            {
                m_disableAutoFormat = value;
            }
        }

        /// <summary>
        /// Specifies if any formfield is changed since loading that would affect the appearance. 
        /// </summary>
        internal bool SetAppearanceDictionary
        {
            get
            {
                return m_setAppearanceDictionary;
            }
            set
            {
                m_setAppearanceDictionary = value;
            }
        }
        #endregion

        #region IPdfWrapper Members
        /// <summary>
        /// Gets the element.
        /// </summary>
        /// <value></value>
        IPdfPrimitive IPdfWrapper.Element
        {
            get
            {
                return m_dictionary;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Handles the BeginSave event of the Dictionary control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="ars">The <see cref="Syncfusion.Pdf.Primitives.SavePdfPrimitiveEventArgs"/> instance containing the event data.</param>
        internal virtual void Dictionary_BeginSave(object sender, SavePdfPrimitiveEventArgs ars)
        {
            if (m_signatureFlags != SignatureFlags.None)
               NeedAppearances = false;

            CheckFlatten();
            if (m_fields.Count > 0 && SetAppearanceDictionary)
                m_dictionary.SetBoolean(DictionaryProperties.NeedAppearances, m_needAppearances);
        }

        /// <summary>
        /// Clears PdfForm.
        /// </summary>
        internal virtual void Clear()
        {
            if (m_fields != null)
            {
                m_fields.Clear();
                m_fields = null;
            }
            if (m_dictionary != null)
            {
                m_dictionary.Clear();
                m_dictionary = null;
            }
            m_fieldName.Clear();
            m_fieldNames.Clear();
            m_pageMap.Clear();
        }

        /// <summary>
        /// Checks for the flatten fields.
        /// </summary>
        private void CheckFlatten()
        {
            int i = 0;
            while (i < m_fields.Count)
            {
                PdfField field = m_fields[i];

                if (field.DisableAutoFormat)
                {
                    if (field.Dictionary.ContainsKey("AA"))
                        field.Dictionary.Remove("AA");
                }
                if (field.Flatten)
                {
                    field.Draw();
                    m_fields.Remove(field);
                    DeleteFromPages(field);
                    DeleteAnnotation(field);
                    --i;
                }
                else if (field is PdfLoadedField)
                {
                    if (field is PdfLoadedTextBoxField)
                    {
                        if (field.Dictionary.ContainsKey(DictionaryProperties.AP) || (field as PdfLoadedTextBoxField).Items.Count > 0)
                        {
                            if (this.IsXFA)
                            {
                                //PdfDictionary ap = new PdfDictionary();
                                //ap = new PdfDictionary();
                                //  field.Dictionary[DictionaryProperties.AP] = new PdfReferenceHolder(ap);
                                if (field.Dictionary.ContainsKey(DictionaryProperties.MK))
                                {
                                    PdfDictionary mk = field.Dictionary[DictionaryProperties.MK] as PdfDictionary;
                                    if (mk.ContainsKey(DictionaryProperties.BG))
                                        mk.Remove(DictionaryProperties.BG);
                                }
                            }

                            if (!this.IsXFA)
                                (field as PdfLoadedField).BeginSave();
                        }
                    }
#if !SILVERLIGHT && !NETFX_CORE && !WP
                    if (field is PdfLoadedField && (SignatureFlags == Security.SignatureFlags.None && field is PdfLoadedSignatureField))
                        field.Save();
                    else
                        if (field is PdfLoadedField && !ReadOnly)
                            (field as PdfLoadedField).BeginSave();
                        else
                            field.Save();
#else
                    if (field is PdfLoadedField && SignatureFlags == Security.SignatureFlags.None)
                        field.Save();
                    else
                        (field as PdfLoadedField).BeginSave();
#endif
                }

                ++i;
            }
        }

        /// <summary>
        /// Checks the form is XFA Form or AcroForm
        /// </summary>
        /// <param name="field">Form field</param>
        /// <param name="xfa"></param>
        /// <returns></returns>
        //private bool CheckXFA(PdfField field, bool xfa)
        //{
        //    if (!xfa)
        //    {
        //        }
        //    }
        //    return xfa;
        //}

        /// <summary>
        /// Deletes from pages.
        /// </summary>
        /// <param name="field">The field.</param>
        internal void DeleteFromPages(PdfField field)
        {
            PdfDictionary dic = field.Dictionary;

            if (dic.ContainsKey(DictionaryProperties.Kids))
            {
                PdfArray array = (dic[DictionaryProperties.Kids]) as PdfArray;

                for (int i = 0, size = array.Count; i < size; ++i)
                {
                    PdfReferenceHolder holder = array[i] as PdfReferenceHolder;

                    PdfDictionary widget = holder.Object as PdfDictionary;

                    PdfReferenceHolder pageRef = (widget[DictionaryProperties.P]) as PdfReferenceHolder;

                    PdfDictionary page = (pageRef.Object) as PdfDictionary;

                    if (page.ContainsKey(DictionaryProperties.Annots))
                    {
                        PdfArray annots;
                        if (page[DictionaryProperties.Annots] is PdfReferenceHolder)
                        {
                            PdfReferenceHolder annotReference = (page[DictionaryProperties.Annots]) as PdfReferenceHolder;
                            annots = (annotReference.Object) as PdfArray;
                            annots.Remove(holder);
                            annots.MarkChanged();
                            page.SetProperty(DictionaryProperties.Annots, annots);
                        }
                        else
                        if(page[DictionaryProperties.Annots] is PdfArray)
                        {
                            annots = (page[DictionaryProperties.Annots]) as PdfArray;
                            annots.Remove(holder);
                            annots.MarkChanged();
                            page.SetProperty(DictionaryProperties.Annots, annots);
                        }
                    }
                }
            }
            else
            {
                PdfReferenceHolder pageRef = null;

                if (dic.ContainsKey(DictionaryProperties.P))
                    pageRef = dic[DictionaryProperties.P] as PdfReferenceHolder;
                else
                    pageRef = new PdfReferenceHolder(field.Page.Dictionary);

                PdfDictionary page = pageRef.Object as PdfDictionary;

                if (page.ContainsKey(DictionaryProperties.Annots))
                {
                    PdfArray annots = (page[DictionaryProperties.Annots]) as PdfArray;
                    annots.Remove(new PdfReferenceHolder(dic));
                    annots.MarkChanged();
                    page.SetProperty(DictionaryProperties.Annots, annots);
                }
            }
        }

        /// <summary>
        /// Deletes the annotation from the page dictionary.
        /// </summary>
        /// <param name="field">The field.</param>
        internal void DeleteAnnotation(PdfField field)
        {
            PdfDictionary dic = field.Dictionary;

            if (dic.ContainsKey(DictionaryProperties.Kids))
            {
                PdfArray array = (dic[DictionaryProperties.Kids]) as PdfArray;

                array.Clear();

                dic.SetProperty(DictionaryProperties.Kids, array);
            }
        }




        /// <summary>
        /// Gets the new name of the field.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The field name.</returns>
       internal virtual string GetCorrectName(string name)
        {
            string correctName = name;

            m_fieldNames.Add(correctName);

            if (m_fieldName.Contains(name))
            {
                int firstIndex = m_fieldName.IndexOf(name);
                int lastIndex = m_fieldName.LastIndexOf(name);

                if (firstIndex != lastIndex)
                {
                    string[] suffix = Guid.NewGuid().ToString().Split('-');
                    correctName = name + "_" + suffix[4];
                    m_fieldName.RemoveAt(lastIndex);
                    m_fieldName.Add(correctName);
                }
            }
            return correctName;
        }

        /// <summary>
        /// Specifies whether to set the default appearance for the form or not.
        /// </summary>
        /// <param name="applyDefault"></param>
        public void SetDefaultAppearance(bool applyDefault)
        {
            NeedAppearances = applyDefault;
            SetAppearanceDictionary = true;
        }
        #endregion
    }
}
