#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Drawing;

using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;
using Syncfusion.Pdf.Parsing;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents field of the Pdf document's interactive form.
    /// </summary>
    /// <seealso cref="IPdfWrapper"/> Interface  
    public abstract class PdfField : IPdfWrapper
    {
        #region Fields
        /// <summary>
        /// Internal variable to store field's name.
        /// </summary>
        private string m_name = String.Empty;

        /// <summary>
        /// Internal variable to store page.
        /// </summary>
        private PdfPageBase m_page = null;

        /// <summary>
        /// Internal variable to store flags of the field.
        /// </summary>
        private FieldFlags m_flags = FieldFlags.Default;

        /// <summary>
        /// Internal variable to store form.
        /// </summary>
        private PdfForm m_form = null;

        /// <summary>
        /// Internal variable to store mapping name.
        /// </summary>
        private string m_mappingName = String.Empty;

        /// <summary>
        /// Internal variable to store value whether to export field's data.
        /// </summary>
        private bool m_export = true;

        /// <summary>
        /// Internal variable to store value whether the field is read only.
        /// </summary>
        private bool m_readOnly = false;

        /// <summary>
        /// Internal variable to store value whether the field is required.
        /// </summary>
        private bool m_required = false;

        /// <summary>
        /// Internal variable to store tool tip.
        /// </summary>
        private string m_toolTip = String.Empty;

        /// <summary>
        /// Internal variable to store dictionary.
        /// </summary>
        private PdfDictionary m_dictionary = new PdfDictionary();

        /// <summary>
        /// Form flatten.
        /// </summary>
        private bool m_flatten;

        /// <summary>
        /// Indicates if AutoFormat has to be removed.
        /// </summary>
        private bool m_disableAutoFormat;

        private int m_rotationAngle;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfField"/> class.
        /// </summary>
        /// <param name="page">The page where the field should be placed.</param>
        /// <param name="name">The name.</param>
        public PdfField(PdfPageBase page, string name)
            : base()
        {
            Initialize();

            m_name = name;
            m_page = page;
            m_dictionary.SetProperty(DictionaryProperties.T, new PdfString(name));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfField"/> class.
        /// </summary>
        internal PdfField()
        {
            Initialize();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public virtual string Name
        {
            get
            {
                return m_name;
            }
        }

        /// <summary>
        /// Gets the form.
        /// </summary>
        /// <value>The form.</value>
        public virtual PdfForm Form
        {
            get
            {
                return m_form;
            }
        }

        /// <summary>
        /// Gets or sets the mapping name to be used when exporting interactive form 
        /// field data from the document.
        /// </summary>
        /// <value>The mapping name.</value>
        public virtual string MappingName
        {
            get
            {
                return m_mappingName;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("MappingName");
                }

                if (m_mappingName != value)
                {
                    m_mappingName = value;
                    m_dictionary.SetString(DictionaryProperties.TM, m_mappingName);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PdfField"/> is export.
        /// </summary>
        /// <value><c>true</c> if export; otherwise, <c>false</c>.</value>
        public virtual bool Export
        {
            get
            {
                return m_export;
            }

            set
            {
                if (m_export != value)
                {
                    m_export = value;

                    if (m_export)
                    {
                        Flags -= FieldFlags.NoExport;
                    }
                    else
                    {
                        Flags |= FieldFlags.NoExport;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [read only].
        /// </summary>
        /// <value> if the field is read only, set to <c>true</c>.</value>
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
        /// Gets or sets a value indicating whether this <see cref="PdfField"/> is required.
        /// </summary>
        /// <value><c>true</c> if required; otherwise, <c>false</c>.</value>
        public virtual bool Required
        {
            get
            {
                return m_required;
            }

            set
            {
                if (m_required != value)
                {
                    m_required = value;

                    if (m_required)
                    {
                        Flags |= FieldFlags.Required;
                    }
                    else
                    {
                        Flags -= FieldFlags.Required;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the tool tip.
        /// </summary>
        /// <value>The tool tip.</value>
        public virtual string ToolTip
        {
            get
            {
                return m_toolTip;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("ToolTip");
                }

                if (m_toolTip != value)
                {
                    m_toolTip = value;
                    m_dictionary.SetString(DictionaryProperties.TU, m_toolTip);
                }
            }
        }

        /// <summary>
        /// Gets the page.
        /// </summary>
        /// <value>The page.</value>
        public virtual PdfPageBase Page
        {
            get
            {
                return m_page;
            }
            internal set
            {
                m_page = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PdfField"/> is flatten.
        /// </summary>
        public bool Flatten
        {
            get
            {
                bool flatten = m_flatten;

                if (Form != null)
                {
                    flatten |= Form.Flatten;
                }

                return flatten;
            }

            set
            {
                m_flatten = value;
            }
        }

        /// <summary>
        /// Gets or sets the flags.
        /// </summary>
        /// <value>The flags.</value>
        internal virtual FieldFlags Flags
        {
            get
            {
                return m_flags;
            }

            set
            {
                if (m_flags != value)
                {
                    m_flags = value;
                    m_dictionary.SetNumber(DictionaryProperties.FieldFlags, (int)m_flags);
                }
            }
        }

        /// <summary>
        /// Gets or sets the dictionary.
        /// </summary>
        /// <value>The dictionary.</value>
        internal PdfDictionary Dictionary
        {
            get
            {
                return m_dictionary;
            }

            set
            {
                m_dictionary = value;
            }
        }

        internal int RotationAngle
        {
            get
            {
                return m_rotationAngle;
            }
            set
            {
                m_rotationAngle = value;
            }
        }

        public bool DisableAutoFormat
        {
            get
            {
                bool disableAutoFormat = m_disableAutoFormat;

                if (Form != null)
                {
                    disableAutoFormat |= Form.DisableAutoFormat;
                }

                return disableAutoFormat;
            }
            set
            {
                m_disableAutoFormat = value;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Sets the form.
        /// </summary>
        /// <param name="form">The form.</param>
        internal void SetForm(PdfForm form)
        {
            m_form = form;
            DefineDefaultAppearance();
        }

        /// <summary>
        /// Saves an object.
        /// </summary>
        internal virtual void Save()
        {
            bool formReadOnly = (Form != null && Form.ReadOnly);
            if (m_readOnly || formReadOnly)
            {
                Flags |= FieldFlags.ReadOnly;
            }
        }

        /// <summary>
        /// Draws this instance if it is flatten.
        /// </summary>
        internal abstract void Draw();

        /// <summary>
        ///Sets the proper name to the field.
        /// </summary>
        /// <param name="name">The name.</param>
        internal virtual void ApplyName(string name)
        {
            m_name = name;

            Dictionary.SetProperty(DictionaryProperties.T, new PdfString(name));
        }

        /// <summary>
        /// Clones the field.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>The copy of the field.</returns>
        internal virtual PdfField Clone(PdfPageBase page)
        {
            if (page == null)
            {
                throw new ArgumentNullException("page");
            }
            PdfField field = null;

            if (!(page as PdfPage).Section.ParentDocument.EnableMemoryOptimization)
            {
                field = MemberwiseClone() as PdfField;
                field.Dictionary = new PdfDictionary(Dictionary);
                field.m_page = page;
                field.Dictionary[DictionaryProperties.P] = new PdfReferenceHolder(page);
            }
            else
            {
                if (this is Syncfusion.Pdf.Parsing.PdfLoadedField)
                {
                    PdfDictionary newDict = new PdfDictionary(Dictionary);
                    newDict.Remove(DictionaryProperties.Parent);
                    newDict.Remove(DictionaryProperties.P);
                    newDict.Remove(DictionaryProperties.Kids);

                    PdfDictionary fieldDictionary = newDict.Clone((page as PdfPage).Section.ParentDocument.CrossTable) as PdfDictionary;

                    if (this is PdfLoadedButtonField)
                        field = (this as PdfLoadedButtonField).Clone(fieldDictionary, page as PdfPage);
                    else if (this is PdfLoadedCheckBoxField)
                        field = (this as PdfLoadedCheckBoxField).Clone(fieldDictionary, page as PdfPage);
                    else if (this is PdfLoadedComboBoxField)
                        field = (this as PdfLoadedComboBoxField).Clone(fieldDictionary, page as PdfPage);
                    else if (this is PdfLoadedListBoxField)
                        field = (this as PdfLoadedListBoxField).Clone(fieldDictionary, page as PdfPage);
                    else if (this is PdfLoadedRadioButtonListField)
                        field = (this as PdfLoadedRadioButtonListField).Clone(fieldDictionary, page as PdfPage);
#if !SILVERLIGHT && !NETFX_CORE && !WP
                    else if (this is PdfLoadedSignatureField)
                        field = (this as PdfLoadedSignatureField).Clone(fieldDictionary, page as PdfPage);
# endif
                    else if (this is PdfLoadedTextBoxField)
                        field = (this as PdfLoadedTextBoxField).Clone(fieldDictionary, page as PdfPage);
                    else if (!fieldDictionary.ContainsKey(DictionaryProperties.FT) && (this is PdfLoadedStyledField))
                        field = (this as PdfLoadedStyledField).Clone(fieldDictionary, page as PdfPage);

                    PdfLoadedField oldField = this as PdfLoadedField;

                    field.DisableAutoFormat = oldField.DisableAutoFormat;
                    field.Export = oldField.Export;
                    field.Flags = oldField.Flags;
                    field.Flatten = oldField.Flatten;
                    if (field.MappingName != null)
                        field.MappingName = oldField.MappingName;
                    field.Required = oldField.Required;
                    field.RotationAngle = oldField.RotationAngle;
                    if (oldField.ToolTip != null)
                        field.ToolTip = oldField.ToolTip;
                }
            }

            return field;
        }

        /// <summary>
        /// Defines default appearance.
        /// </summary>
        protected virtual void DefineDefaultAppearance()
        {
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected virtual void Initialize()
        {
            m_dictionary.BeginSave += new SavePdfPrimitiveEventHandler(Dictionary_BeginSave);
        }

        /// <summary>
        /// Handles the BeginSave event of the m_dictionary control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="ars">The <see cref="Syncfusion.Pdf.Primitives.SavePdfPrimitiveEventArgs"/> instance containing the event data.</param>
        private void Dictionary_BeginSave(object sender, SavePdfPrimitiveEventArgs ars)
        {
            Save();
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
    }
}
