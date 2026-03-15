#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
using System.IO;
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
    /// Represents base class for loaded fields.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Create a new document.
    /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
    /// //load the form fields
    /// foreach (PdfLoadedField field in doc.Form.Fields)
    /// {
    ///   // Flatten the form
    ///   field.Flatten = true;
    /// }
    /// doc.Save("Form.pdf");
    /// </code>
    /// <code lang="VB">
    ///  'Create a new document
    /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
    /// 'load the form fields
    /// For Each field As PdfLoadedField In doc.Form.Fields
    ///   ' Flatten the form
    /// field.Flatten = True
    /// Next field
    ///doc.Save("Form.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfField"/> Class    
    public abstract class PdfLoadedField
        : PdfField
    {
        #region Internal declarations
        /// <summary>
        /// NameChanged event handler.
        /// </summary>
        /// <param name="name">New name of the field.</param>
        internal delegate void BeforeNameChangesEventHandler(string name);
        #endregion

        #region Fields

        /// <summary>
        /// Form field identifier
        /// </summary>
        public int ObjectID;
        /// <summary>
        /// Cross Table.
        /// </summary>
        private PdfCrossTable m_crossTable;

        /// <summary>
        /// Indicates was field changed or not.
        /// </summary>
        private bool m_Changed;

        /// <summary>
        /// Represents index used to default annotation.
        /// </summary>
        private int m_defaultIndex;

        /// <summary>
        /// Represent's the field name.
        /// </summary>
        private string m_name;

        /// <summary>
        /// Represent's the field page.
        /// </summary>
        private PdfPageBase m_page;

        /// <summary>
        /// Internal variable to store form.
        /// </summary>
        private PdfLoadedForm m_form = null;

        #endregion

        #region Events
        /// <summary>
        /// Raises when user manually changes the name of the field.
        /// </summary>
        internal event BeforeNameChangesEventHandler BeforeNameChanges;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the name of the field.
        /// </summary>
        /// <value>A string value specifying the name of the field.</value>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// //load the form field
        /// PdfLoadedField field = doc.Form.Fields[0] as PdfLoadedField;
        /// // Read the field name
        /// String fieldName = field.Name;            
        /// doc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document.
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// 'load the form field
        /// Dim field As PdfLoadedField = TryCast(doc.Form.Fields(0), PdfLoadedField)
        /// ' Read the field name
        /// Dim fieldName As String = field.Name
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedField"/> Class   
        /// <seealso cref="PdfLoadedDocument"/> Class   
        public override string Name
        {
            get
            {
                //if (m_name == null)
                m_name = GetFieldName();
                return m_name;
            }
        }
       
        /// <summary>
        /// Gets or sets the mapping name to be used when exporting interactive form
        /// field data from the document.
        /// </summary>
        /// <value>A string value specifying the mapping name of the field. </value>        
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// //load the form field
        /// PdfLoadedField field = doc.Form.Fields[0] as PdfLoadedField;
        /// // Sets the Mapping name as 'FirstField'
        /// field.MappingName = "FirstField";            
        /// doc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document.
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// 'load the form field
        /// Dim field As PdfLoadedField = TryCast(doc.Form.Fields(0), PdfLoadedField)
        /// ' Sets the Mapping name as 'FirstField'
        /// field.MappingName = "FirstField"
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedField"/> Class   
        /// <seealso cref="PdfLoadedDocument"/> Class   
        public override string MappingName
        {
            get
            {
                string mappingName = base.MappingName;

                if ((mappingName == null) || (mappingName == String.Empty))
                {
                    PdfString str = GetValue(Dictionary, m_crossTable, DictionaryProperties.TM, false) as PdfString;

                    if (str != null)
                    {
                        mappingName = str.Value;
                    }
                }

                return mappingName;
            }
            set
            {
                base.MappingName = value;

                Changed = true;
            }
        }

        /// <summary>
        /// Gets or sets the tool tip.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// //load the form field
        /// PdfLoadedField field = doc.Form.Fields[0] as PdfLoadedField;
        /// // Sets the tooltip of the field
        /// field.ToolTip = "FirstField";       
        /// doc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document.
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// 'load the form field
        /// Dim field As PdfLoadedField = TryCast(doc.Form.Fields(0), PdfLoadedField)
        /// ' Sets the tooltip of the field
        /// field.ToolTip = "FirstField"
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedField"/> Class   
        /// <seealso cref="PdfLoadedDocument"/> Class   
        public override string ToolTip
        {
            get
            {
                PdfString str = GetValue(Dictionary, m_crossTable, DictionaryProperties.TU, false) as PdfString;

                string toolTip = null;

                if (str != null)
                {
                    toolTip = str.Value;
                }

                return toolTip;
            }
            set
            {
                base.ToolTip = value;

                Changed = true;
            }
        }

        /// <summary>
        /// Gets the page.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// //load the form field
        /// PdfLoadedField field = doc.Form.Fields[0] as PdfLoadedField;
        /// PdfPageBase page = field.Page;  
        /// doc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document.
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// 'load the form field
        /// Dim field As PdfLoadedField = TryCast(doc.Form.Fields(0), PdfLoadedField)
        /// Dim page As PdfPageBase = field.Page
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedField"/> Class   
        /// <seealso cref="PdfPageBase"/> Class  
        public override PdfPageBase Page
        {
            get
            {
                if (m_page == null)
                {
                    m_page = GetLoadedPage();

                }
                else
                {
                    if ((m_page != null) && (m_page is PdfLoadedPage))
                    {
                        if (this.Changed || this.Form.Flatten || this.Flatten)
                        {
                            m_page = GetLoadedPage();

                        }
                    }

                }
                return m_page;
            }
            internal set
            {
                m_page = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [read only].
        /// </summary>
        /// <value>True if the field is read-only, false otherwise. Default is false.</value>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// //load the form field
        /// PdfLoadedField field = doc.Form.Fields[0] as PdfLoadedField;
        /// // Set the form field as read only
        /// field.ReadOnly = true;
        /// doc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document.
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// 'load the form field
        /// Dim field As PdfLoadedField = TryCast(doc.Form.Fields(0), PdfLoadedField)
        /// ' Set the form field as read only
        /// field.ReadOnly = True
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedDocument"/> Class   
        /// <seealso cref="PdfLoadedField"/> Class  
        public override bool ReadOnly
        {
            get
            {
                bool readOnly = ((FieldFlags.ReadOnly & Flags) != 0);

                return (readOnly || Form.ReadOnly);
            }
            set
            {
                bool readOnly = (value || Form.ReadOnly);

                if (readOnly)
                {
                    Flags |= FieldFlags.ReadOnly;
                }
                else
                {
                    //Flags &= ~FieldFlags.ReadOnly;
                    Flags = FieldFlags.Edit;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PdfField"/> is required.
        /// </summary>
        /// <value>True if the field is required, false otherwise. Default is false.</value>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// //load the form field
        /// PdfLoadedField field = doc.Form.Fields[0] as PdfLoadedField;
        /// field.Required = true;
        /// doc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document.
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// 'load the form field
        /// Dim field As PdfLoadedField = TryCast(doc.Form.Fields(0), PdfLoadedField)
        /// field.Required = True
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        public override bool Required
        {
            get
            {
                bool required = ((FieldFlags.Required & Flags) != 0);

                return required;
            }
            set
            {
                bool required = value;

                if (required)
                {
                    Flags |= FieldFlags.Required;
                }
                else
                {
                    Flags &= ~FieldFlags.Required;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PdfField"/> is export.
        /// </summary>
        /// <value><c>true</c> if export; otherwise, <c>false</c>.</value>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// //load the form field
        /// PdfLoadedField field = doc.Form.Fields[0] as PdfLoadedField;
        /// field.Export = true;
        /// doc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document.
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// 'load the form field
        /// Dim field As PdfLoadedField = TryCast(doc.Form.Fields(0), PdfLoadedField)
        /// field.Export = True
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        public override bool Export
        {
            get
            {
                bool export = !((FieldFlags.NoExport & Flags) != 0);

                return export;
            }
            set
            {
                bool export = value;

                if (export)
                {
                    Flags &= ~FieldFlags.NoExport;
                }
                else
                {
                    Flags |= FieldFlags.NoExport;
                }
            }
        }

        /// <summary>
        /// Gets or sets the flags.
        /// </summary>
        internal override FieldFlags Flags
        {
            get
            {
                FieldFlags flags = base.Flags;

                if (flags == FieldFlags.Default)
                {
                    PdfNumber number = GetValue(Dictionary, m_crossTable, DictionaryProperties.FieldFlags, true) as PdfNumber;

                    if (number != null)
                    {
                        flags = (FieldFlags)number.IntValue;
                    }
                }

                return flags;
            }
            set
            {
                base.Flags = value;

                Changed = true;
            }
        }

        /// <summary>
        /// Gets the actual field name.
        /// </summary>
        /// <remarks>This returns the field name alone, where Name property returns the field name along with its parent name.</remarks>
        internal string ActualFieldName
        {
            get
            {
                string name = null;

                PdfString str = GetValue(Dictionary, m_crossTable, DictionaryProperties.T, false) as PdfString;

                if (str != null)
                {
                    name = str.Value;
                }
                return name;
            }
        }

        /// <summary>
        /// Gets the form.
        /// </summary>
        /// <value>The form.</value>
        public PdfForm Form
        {
            get
            {
                if (m_form != null)
                    return m_form;
                return base.Form;
            }
        }

        /// <summary>
        /// Gets or sets the cross table.
        /// </summary>
        internal PdfCrossTable CrossTable
        {
            get
            {
                return m_crossTable;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("CrossTable");

                if (m_crossTable != value)
                {
                    m_crossTable = value;
                }
            }
        }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>The parent.</value>
        internal PdfDictionary Parent
        {
            get
            {
                PdfDictionary parent = null;

                if (Dictionary.ContainsKey(DictionaryProperties.Parent))
                {
                    parent = m_crossTable.GetObject(Dictionary[DictionaryProperties.Parent]) as PdfDictionary;
                }

                return parent;
            }
        }

        /// <summary>
        /// Gets or sets the changed.
        /// </summary>
        internal bool Changed
        {
            get
            {
                return m_Changed;
            }
            set
            {
                m_Changed = value;
            }
        }

        /// <summary>
        /// Gets or sets the index of the default.
        /// </summary>
        internal int DefaultIndex
        {
            get
            {
                return m_defaultIndex;
            }
            set
            {
                m_defaultIndex = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLoadedField"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        internal PdfLoadedField(PdfDictionary dictionary, PdfCrossTable crossTable)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            if (crossTable == null)
                throw new ArgumentNullException("crossTable");

            Dictionary = dictionary;
            m_crossTable = crossTable;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Sets the name of the field.
        /// </summary>
        /// <param name="name">New name of the field.</param>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// //load the form field
        /// PdfLoadedField field = doc.Form.Fields[0] as PdfLoadedField;
        /// // Sets new name of the first field
        /// field.SetName("fieldFirstName");
        /// doc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document.
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// 'load the form field
        /// Dim field As PdfLoadedField = TryCast(doc.Form.Fields(0), PdfLoadedField)
        /// ' Sets new name of the first field
        /// field.SetName("fieldFirstName")
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        public void SetName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (name == String.Empty)
                throw new ArgumentException("The name can't be empty");

            if (Name != null && Name != name)
            {
                string[] nameParts = Name.Split('.');
                int index = nameParts.Length;
                if (nameParts[index - 1] == name)
                    return;
                else
                {
                    PdfString str = new PdfString(name);
                    if (m_form != null)
                        BeforeNameChanges(name);
                    Dictionary.SetProperty(DictionaryProperties.T, str);
                    Changed = true;
                }
            }
        }

        /// <summary>
        /// Searches the in parents.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <param name="value">The value.</param>
        /// <returns>Searched primitive.</returns>
        internal static IPdfPrimitive SearchInParents(PdfDictionary dictionary, PdfCrossTable crossTable, string value)
        {
            IPdfPrimitive primitive = null;
            PdfDictionary dic = dictionary;

            while ((primitive == null) && (dic != null))
            {
                if (dic.ContainsKey(value))
                {
                    primitive = crossTable.GetObject(dic[value]);
                }
                else
                {
                    if (dic.ContainsKey(DictionaryProperties.Parent))
                    {
                        dic = crossTable.GetObject(dic[DictionaryProperties.Parent]) as PdfDictionary;
                    }
                    else
                    {
                        dic = null;
                    }
                }
            }

            return primitive;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <param name="value">The value.</param>
        /// <param name="inheritable">if it is inheritable, set to <c>true</c>.</param>
        /// <returns>The founded value.</returns>
        internal static IPdfPrimitive GetValue(PdfDictionary dictionary, PdfCrossTable crossTable,
            string value, bool inheritable)
        {
            IPdfPrimitive primitive = null;

            if (dictionary.ContainsKey(value))
            {
                primitive = crossTable.GetObject(dictionary[value]);
            }
            else
            {
                if (inheritable)
                {
                    primitive = SearchInParents(dictionary, crossTable, value);
                }
            }

            return primitive;
        }

        /// <summary>
        /// Gets the widget annotation.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <returns>The widget annotation dictionary.</returns>
        internal PdfDictionary GetWidgetAnnotation(PdfDictionary dictionary, PdfCrossTable crossTable)
        {
            PdfDictionary dic = null;

            if (dictionary.ContainsKey(DictionaryProperties.Kids))
            {
                PdfArray array = crossTable.GetObject(dictionary[DictionaryProperties.Kids]) as PdfArray;
                if (array.Count > 0)
                {
                    PdfReference reference = crossTable.GetReference(array[m_defaultIndex]) as PdfReference;
                    dic = crossTable.GetObject(reference) as PdfDictionary;
                }
            }

            if (dictionary.ContainsKey(DictionaryProperties.Subtype))
            {
                PdfName type = CrossTable.GetObject(dictionary[DictionaryProperties.Subtype]) as PdfName;

                if (type.Value == DictionaryProperties.Widget)
                {
                    dic = dictionary;
                }
            }
            if(dic == null)
            {
                dic = dictionary;
            }
        
            return dic;
        }

        /// <summary>
        /// Gets the high light.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <returns>The HighLIght mode.</returns>
        internal PdfHighlightMode GetHighLight(PdfDictionary dictionary, PdfCrossTable crossTable)
        {
            PdfName name = null;

            if (dictionary.ContainsKey(DictionaryProperties.Kids))
            {
                PdfDictionary widget = GetWidgetAnnotation(dictionary, crossTable);

                if (widget.ContainsKey(DictionaryProperties.H))
                {
                    name = crossTable.GetObject(widget[DictionaryProperties.H]) as PdfName;
                }
            }
            else
            {
                if (dictionary.ContainsKey(DictionaryProperties.H))
                {
                    name = crossTable.GetObject(dictionary[DictionaryProperties.H]) as PdfName;
                }
            }

            PdfHighlightMode hMode = PdfHighlightMode.NoHighlighting;

            if (name != null)
            {

                switch (name.Value)
                {
                    case "I":
                        hMode = PdfHighlightMode.Invert;
                        break;

                    case "O":
                        hMode = PdfHighlightMode.Outline;
                        break;

                    case "P":
                        hMode = PdfHighlightMode.Push;
                        break;
                }
            }

            return hMode;
        }

        /// <summary>
        /// Draws this instance if it is flatten.
        /// </summary>
        internal override abstract void Draw();

        /// <summary>
        /// Creates a copy of loaded field item.
        /// </summary>
        internal abstract PdfLoadedFieldItem CreateLoadedItem(PdfDictionary dictionary);

        /// <summary>
        /// Applies field name
        /// </summary>
        /// <param name="name">specified field name</param>
        internal override void ApplyName(string name)
        {
            SetName(name);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Begins the save.
        /// </summary>
        internal virtual void BeginSave()
        {
        }

        /// <summary>
        /// Gets the loaded page.
        /// </summary>
        /// <returns>The loaded page in which field draw.</returns>
        private PdfPageBase GetLoadedPage()
        {
            PdfPageBase page = base.Page;

            if (page == null)
            {
                PdfLoadedDocument doc = CrossTable.Document as PdfLoadedDocument;

                PdfDictionary widget = GetWidgetAnnotation(Dictionary, CrossTable);

                if (widget == null)
                {
                    widget = Dictionary;
                }

                if (widget.ContainsKey(DictionaryProperties.P))
                {
                    IPdfPrimitive pageRef = CrossTable.GetObject(widget[DictionaryProperties.P]);
                    PdfDictionary pageDic = pageRef as PdfDictionary;

                    if (pageDic != null)
                        page = doc.Pages.GetPage(pageDic);
                }
                else
                {
                    PdfReference widgetReference = CrossTable.GetReference(widget);
                    foreach (PdfLoadedPage loadedpage in doc.Pages)
                    {
                        PdfArray lAnnots = loadedpage.GetAnnots();

                        if (lAnnots != null)
                        {
                            for (int i = 0; i < lAnnots.Count; i++)
                            {
                                PdfReferenceHolder holder = lAnnots[i] as PdfReferenceHolder;
                                if (holder.Reference == widgetReference)
                                {
                                    page = loadedpage;
                                    return page;
                                }
                            }
                        }
                    }
                }
            }

            return page;
        }

#if !SILVERLIGHT && !NETFX_CORE && !WP
        /// <summary>
        /// Exports the form fields.
        /// </summary>
        /// <param name="textWriter"></param>
        internal void ExportField(System.Xml.XmlTextWriter textWriter)
        {
            PdfName name = GetValue(this.Dictionary, this.CrossTable, DictionaryProperties.FT, true) as PdfName;

            switch (name.Value)
            {
                case "Tx":
                    PdfString str = GetValue(this.Dictionary, this.CrossTable, DictionaryProperties.V, true) as PdfString;
                    if (str != null)
                    {
                        textWriter.WriteStartElement(this.Name, "");
                        textWriter.WriteString(str.Value);
                        textWriter.WriteEndElement();
                    }
                    break;
                case "Ch":

                    PdfName str1 = GetValue(this.Dictionary, this.CrossTable, DictionaryProperties.V, true) as PdfName;
                    if (str1 != null)
                    {
                        textWriter.WriteStartElement(this.Name, "");
                        textWriter.WriteString(str1.Value);
                        textWriter.WriteEndElement();
                    }
                    break;

                case "Btn":
                    PdfName str2 = GetValue(this.Dictionary, this.CrossTable, DictionaryProperties.V, true) as PdfName;
                    if (str2 != null)
                    {
                        textWriter.WriteStartElement(this.Name, "");
                        textWriter.WriteString(str2.Value);
                        textWriter.WriteEndElement();
                    }
                    else
                    {
                        PdfDictionary holder = GetWidgetAnnotation(this.Dictionary, this.CrossTable) as PdfDictionary;
                        if ((holder["AS"] as PdfName) != null)
                        {
                            textWriter.WriteStartElement(this.Name, "");
                            textWriter.WriteString((holder["AS"] as PdfName).Value);
                            textWriter.WriteEndElement();
                        }
                    }
                    break;
            }

        }
#endif

        /// <summary>
        /// Exports the form fields.
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <param name="objectid">Object identifier.</param>

        internal void ExportField(Stream stream, ref int objectid)
        {
            bool flag = false;
            PdfArray kids = null;

            if (this.Dictionary.ContainsKey(DictionaryProperties.Kids))
            {
                kids = CrossTable.GetObject(Dictionary[DictionaryProperties.Kids]) as PdfArray;

                if (kids != null)
                {
                    for (int i = 0; i < kids.Count; i++)
                    {
                        flag = flag || (kids[i] is PdfLoadedField);
                    }
                }
            }

            PdfName name = GetValue(this.Dictionary, this.CrossTable, DictionaryProperties.FT, true) as PdfName;
            string strValue = "";
            switch (name.Value)
            {
                case "Tx":
                    PdfString tempname = GetValue(this.Dictionary, this.CrossTable, DictionaryProperties.V, true) as PdfString;
                    if (tempname != null)
                        strValue = tempname.Value;
                    break;
                case "Ch":
                    PdfName str1 = GetValue(this.Dictionary, this.CrossTable, DictionaryProperties.V, true) as PdfName;
                    if (str1 != null)
                        strValue = str1.Value;
                    break;
                case "Btn":
                    PdfName str2 = GetValue(this.Dictionary, this.CrossTable, DictionaryProperties.V, true) as PdfName;
                    if (str2 != null)
                    {
                        strValue = str2.Value;
                    }
                    else
                    {
                        PdfDictionary holder = GetWidgetAnnotation(this.Dictionary, this.CrossTable) as PdfDictionary;
                        if ((holder["AS"] as PdfName) != null)
                        {
                            strValue = (holder["AS"] as PdfName).Value;
                        }
                    }
                    break;
            }

            if (!validateString(strValue) || flag)
            {
                if (flag)
                {
                    for (int i = 0; i < kids.Count; i++)
                    {
                        PdfLoadedField field = kids[i] as PdfLoadedField;
                        if ((field != null) && field.Export)
                        {
                            field.ExportField(stream, ref objectid);
                        }
                    }

                    this.ObjectID = objectid;
                    objectid++;

                    StringBuilder builder = new StringBuilder();

                    PdfString stringValue = new PdfString(strValue);
                    stringValue.Encode = PdfString.ForceEncoding.ASCII;
                    byte[] tempbuffer = Encoding.GetEncoding("windows-1252").GetBytes(stringValue.Value);

                    builder.AppendFormat("{0} 0 obj<</T <{1}> /Kids [", this.ObjectID, PdfString.BytesToHex(tempbuffer));

                    for (int i = 0; i < kids.Count; i++)
                    {
                        PdfLoadedField field = kids[i] as PdfLoadedField;
                        if (((field != null) && field.Export) && (field.ObjectID != 0))
                        {
                            builder.AppendFormat("{0} 0 R ", field.ObjectID);
                        }
                    }
                    builder.Append("]>>endobj\n");

                    PdfString builderString = new PdfString(builder.ToString());
                    builderString.Encode = PdfString.ForceEncoding.ASCII;
                    byte[] tempbuffer1 = Encoding.GetEncoding("windows-1252").GetBytes(builderString.Value);

                    stream.Write(tempbuffer1, 0, tempbuffer1.Length);
                }
                else
                {
                    this.ObjectID = objectid;
                    objectid++;

                    if ((this.GetType().Name == "PdfLoadedCheckBoxField") || (this.GetType().Name == "PdfLoadedRadioButtonListField"))
                    {
                        strValue = "/" + strValue;
                    }
                    else
                    {

                        PdfString stringFieldValue = new PdfString(strValue);
                        stringFieldValue.Encode = PdfString.ForceEncoding.ASCII;
                        byte[] tempbuffer2 = Encoding.GetEncoding("windows-1252").GetBytes(stringFieldValue.Value);

                        strValue = "<" + PdfString.BytesToHex(tempbuffer2) + ">";
                    }
                    StringBuilder builder2 = new StringBuilder();


                    PdfString stringFieldName = new PdfString(this.Name);
                    stringFieldName.Encode = PdfString.ForceEncoding.ASCII;
                    byte[] tempbuffer3 = Encoding.GetEncoding("windows-1252").GetBytes(stringFieldName.Value);

                    builder2.AppendFormat("{0} 0 obj<</T <{1}> /V {2} >>endobj\n", this.ObjectID, PdfString.BytesToHex(tempbuffer3), strValue);

                    PdfString buildString = new PdfString(builder2.ToString());
                    buildString.Encode = PdfString.ForceEncoding.ASCII;
                    byte[] tempbuffer4 = Encoding.GetEncoding("windows-1252").GetBytes(buildString.Value);

                    stream.Write(tempbuffer4, 0, tempbuffer4.Length);
                }
            }
        }

        /// <summary>
        /// Imports the form fields.
        /// </summary>
        /// <param name="textWriter"></param>
        internal void ImportFieldValue(string FieldValue)
        {
            PdfName name = GetValue(this.Dictionary, this.CrossTable, DictionaryProperties.FT, true) as PdfName;

            switch (name.Value)
            {
                case "Tx":
                    if (FieldValue != null)
                    {
                        (this as PdfLoadedTextBoxField).Text = FieldValue;
                    }
                    break;
                case "Ch":

                    if (this.GetType().Name == "PdfLoadedListBoxField")
                    {
                        PdfLoadedListBoxField temp = (this as PdfLoadedListBoxField);
                        temp.SelectedValue =new string[] {FieldValue};
                    }
                    else if (this.GetType().Name == "PdfLoadedComboBoxField")
                    {
                        PdfLoadedComboBoxField temp1 = (this as PdfLoadedComboBoxField);                      
                        temp1.SelectedValue = FieldValue;
                    }
                    break;

                case "Btn":

                    PdfLoadedCheckBoxField field1 = this as PdfLoadedCheckBoxField;
                    if (field1 != null)
                    {
                        if (FieldValue.ToUpper() == "off".ToUpper() || FieldValue.ToUpper() == "no".ToUpper())
                            field1.Checked = false;
                        else
                            field1.Checked = true;
                    }
                    else if (this.GetType().Name == "PdfLoadedRadioButtonListField")
                    {
                        PdfLoadedRadioButtonListField field2 = this as PdfLoadedRadioButtonListField;
                        field2.SelectedValue = FieldValue;
                    }

                    break;
            }

        }

        /// <summary>
        /// Validates the string.
        /// </summary>
        /// <param name="text1">The text1.</param>
        /// <returns></returns>
        internal static bool validateString(string text1)
        {
            if (text1 != null)
            {
                return (text1.Length == 0);
            }
            return true;
        }


        /// <summary>
        /// Gets the name of the field.
        /// </summary>
        /// <returns></returns>
        internal string GetFieldName()
        {
            string name = null;
            PdfString str = null;

            if (!Dictionary.ContainsKey(DictionaryProperties.Parent))
                str = GetValue(Dictionary, m_crossTable, DictionaryProperties.T, false) as PdfString;
            else
            {
                PdfDictionary dic = m_crossTable.GetObject(Dictionary[DictionaryProperties.Parent]) as PdfDictionary;

                while (dic.ContainsKey(DictionaryProperties.Parent))
                {
                    if (dic.ContainsKey(DictionaryProperties.T))
                        name = (name == null ? (GetValue(dic, m_crossTable, DictionaryProperties.T, false) as PdfString).Value : (string.Format("{0}.{1}", (GetValue(dic, m_crossTable, DictionaryProperties.T, false) as PdfString).Value, name)));

                    dic = m_crossTable.GetObject(dic[DictionaryProperties.Parent]) as PdfDictionary;
                }

                if (dic.ContainsKey(DictionaryProperties.T))
                {
                    name = (name == null ? (GetValue(dic, m_crossTable, DictionaryProperties.T, false) as PdfString).Value : (string.Format("{0}.{1}", (GetValue(dic, m_crossTable, DictionaryProperties.T, false) as PdfString).Value, name)));
                    name += string.Format(".{0}", (GetValue(Dictionary, m_crossTable, DictionaryProperties.T, false) as PdfString).Value);
                }
                else
                if(Dictionary.ContainsKey(DictionaryProperties.T))
                {
                    str = GetValue(Dictionary, m_crossTable, DictionaryProperties.T, false) as PdfString;
                }

            }

            if (str != null)
            {
                name = str.Value;
            }

            return name;
        }
        #endregion
    }
}
